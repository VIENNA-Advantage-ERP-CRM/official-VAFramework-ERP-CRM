/**
 * AD Window Model classes
   Include 
  - GridWindow
  - GridTab
     - GridTable
  - GridField
 */

; (function (VIS, $) {

    var Level = VIS.Logging.Level; /* Logger Level Object*/


    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var dSetUrl = baseUrl + "Form/JDataSet";

    var executeReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        var dr = null;
        getDataSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };

    //executeDataSet
    var executeDataSet = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }

        var dataSet = null;

        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            if (callback) {
                callback(dataSet);
            }
        });

        return dataSet;
    };


    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        if (params) {
            dataIn.param = params;
        }
        var value = null;


        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            var dataSet = new VIS.DB.DataSet().toJson(jString);
            if (dataSet.getTable(0).getRows().length > 0) {
                value = dataSet.getTable(0).getRow(0).getCell(0);

            }
            else { value = null; }
            dataSet.dispose();
            dataSet = null;
            if (async) {
                callback(value);
            }
        });

        return value;
    };

    var executeDScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        if (params) {
            dataIn.param = params;
        }
        var value = null;


        getDSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            var dataSet = new VIS.DB.DataSet().toJson(jString);
            if (dataSet.getTable(0).getRows().length > 0) {
                value = dataSet.getTable(0).getRow(0).getCell(0);

            }
            else { value = null; }
            dataSet.dispose();
            dataSet = null;
            if (async) {
                callback(value);
            }
        });

        return value;
    };

    var executeDReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        var dr = null;
        getDSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };

    var executeQueries = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls.join("/"), param: params };

        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        $.ajax({
            url: nonQueryUrl + 'ies',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(dataIn)
        }).done(function (json) {
            ret = json;
            if (callback) {
                callback(json);
            }
        });

        return ret;
    };

    //DataSet String
    function getDataSetJString(data, async, callback) {
        var result = null;
        //data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dataSetUrl,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            result = json;
            if (callback) {
                callback(json);
            }
            //return result;
        });
        return result;
    };

    //DataSet String
    function getDSetJString(data, async, callback) {
        var result = null;
        data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dSetUrl,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            result = json;
            if (callback) {
                callback(json);
            }
            //return result;
        });
        return result;
    };
    //****************************************************//
    //**             GRID WINDOW                       **//
    //**************************************************//

    /**
     *	Window Model
     */
    function GridWindow(json) {

        this.vo = json._vo;

        this.tabs = [];
        for (var i = 0; i < json._tabs.length; i++) {
            var gridTab = new VIS.GridTab(json._tabs[i], this.vo);
            gridTab.setGridWindow(this);
            this.tabs.push(gridTab);
        }

        // console.log(this);

        json = null;
    };



    GridWindow.prototype.getTabs = function () {
        return this.tabs;
    };

    /**
     *	Get tab by index 
     *  @param i index
     *  @return MTab
     */
    GridWindow.prototype.getTab = function (index) {
        if (index < 0 || index + 1 > this.tabs.length)
            return null;
        return this.tabs[index];
    };


    GridWindow.prototype.getIsAppointment = function () {
        return this.vo.IsAppointment;
    };

    GridWindow.prototype.getIsTask = function () {
        return this.vo.IsTask;
    };

    GridWindow.prototype.getIsEmail = function () {
        return this.vo.IsEmail;
    };

    GridWindow.prototype.getIsLetter = function () {
        return this.vo.IsLetter;
    };

    GridWindow.prototype.getIsSms = function () {
        return this.vo.IsSms;
    };

    GridWindow.prototype.getIsChat = function () {
        return this.vo.IsChat;
    };

    GridWindow.prototype.getIsAttachment = function () {
        return this.vo.IsAttachment;
    };

    GridWindow.prototype.getIsHistory = function () {
        return this.vo.IsHistory;
    };

    GridWindow.prototype.getIsCheckRequest = function () {
        return this.vo.IsCheckRequest;
    };



    GridWindow.prototype.getIsCopyReocrd = function () {
        return this.vo.IsCopyReocrd;
    };

    GridWindow.prototype.getIsSubscribedRecord = function () {
        return this.vo.IsSubscribedRecord;
    };

    GridWindow.prototype.getIsZoomAcross = function () {
        return this.vo.IsZoomAcross;
    };

    GridWindow.prototype.getIsCreatedDocument = function () {
        return this.vo.IsCreatedDocument;
    };

    GridWindow.prototype.getIsUploadedDocument = function () {
        return this.vo.IsUploadedDocument;
    };

    GridWindow.prototype.getIsViewDocument = function () {
        return this.vo.IsViewDocument;
    };

    GridWindow.prototype.getIsAttachDocumentFrom = function () {
        return this.vo.IsAttachDocumentFrom;
    };



    GridWindow.prototype.getIsFaxEmail = function () {
        return this.vo.IsFaxEmail;
    };


    GridWindow.prototype.getIsMarkToExport = function () {
        return this.vo.IsMarkToExport;
    };

    GridWindow.prototype.getIsArchive = function () {
        return this.vo.IsArchive;
    };


    GridWindow.prototype.getIsImportMap = function () {
        return this.vo.IsImportMap;
    };


    GridWindow.prototype.getIsAttachmail = function () {
        return this.vo.IsAttachmail;
    };


    GridWindow.prototype.getIsRoleCenterView = function () {
        return this.vo.IsRoleCenterView;
    };


    GridWindow.prototype.getIsSOTrx = function () {
        return this.vo.IsSOTrx;
    };

    GridWindow.prototype.getIsTransaction = function () {
        return this.vo.IsTransaction;
    };

    GridWindow.prototype.getName = function () {
        return this.vo.Name;
    };

    //GridWindow.prototype.getIsCall = function () {
    //    return this.vo.IsCall;
    //};

    GridWindow.prototype.getWindowWidth = function () {
        return this.vo.WinWidth;
    };

    GridWindow.prototype.getHasPanel = function () {
        return this.vo.hasPanel;
    }

    GridWindow.prototype.getFontName = function () {
        return this.vo.FontName;
    }

    GridWindow.prototype.getAD_Image_ID = function () {
        return this.vo.AD_Image_ID;
    }

    GridWindow.prototype.getImageUrl = function () {
        return this.vo.ImageUrl;
    }

    GridWindow.prototype.getIsCompositeView = function () {
        return this.vo.IsCompositeView;
    }

    GridWindow.prototype.dispose = function () {

        originalLength = this.tabs.length;
        var gTab;
        for (var i = originalLength; i > 0; i--) {
            gtab = this.tabs.pop();
            gtab.dispose();
            gtab = null;
        }
        this.tabs.length = 0;
        this.vo = null;
        this.tabs = null;
    };
    //***************** END ******************************//



    //****************************************************//
    //**             GRID TAB                          **//
    //**************************************************//
    /**
 *	Tab Model.
 *  - a combination of AD_Tab (the display attributes) and AD_Table information.
 *  The Tab owns also it's Table model
 *  and listens to data changes to update the Field values.
 *
 *  
 *  - The Tab maintains the bound property: CurrentRow
 *
 *  Event Hierarchies:
 *      - dataChanged (from MTable)
 *          - setCurrentRow
 *              - Update all Field Values
 *
 *      - setValue
 *          - Update Field Value
 *          - Callout
 *  
 */
    function GridTab(gTab, windowVo) {
        this.gTab = gTab;
        this.vo = gTab._vo;
        this.gridTable = new VIS.GridTable(gTab._gridTable);
        this.gridTable.onlyCurrentDays = this.vo.onlyCurrentDays;
        // Maintain version on approval property on tab
        this.gridTable.MaintainVerOnApproval = this.vo.MaintainVerOnApproval;
        this.gridTable.IsMaintainVersions = this.vo.IsMaintainVersions;

        this.parents = [];
        this.orderBys = [];
        this.depOnFieldColumn = [];
        this.depOnField = []; //Fields against columnname
        this.tabPanels = [];
        this.linkColumnName = gTab._linkColumnName;
        this.extendedWhere = gTab._extendedWhere;
        this.keyColumnName = "";
        this.defaultFocusField;
        this.isThroughRoleCenter = false;

        this.query = new VIS.Query();
        this.oldQuery = "0=9";
        //this.oldCardQuery = "0=9";
        this.linkValue = "999999";
        this.currentRow = -1;
        this.hasPanel = false;
        this.isHeaderPanel = false;
        this.headerPanel = null;
        this.isHPanelNotShowInMultiRow = false;
        this.mDataStatusEvent;
        this.mDataListenerList = [];

        this.loadFields();
        this.loadTabPanels();
        this.loadHeaderPanelItems();
        this.gridTable.addDataStatusListener(this);
        this.gridTable.setAD_Tab_ID(this.vo.AD_Tab_ID);
        this.log = VIS.Logging.VLogger.getVLogger("VIS.GridTab");

        this.loadData(windowVo);
        windowVo = null;
        this.gridWindow = null;
        this.outerWhereCondition = null;
    };

    /**
     * set Grid Window 
     * @param {any} gWindow
     */
    GridTab.prototype.setGridWindow = function (gWindow) {
        this.gridWindow = gWindow;
    };

    GridTab.prototype.getParentTab = function () {
        var pTabNo = this.getParentTabNo();
        if (pTabNo < 0 || pTabNo == this.vo.tabNo)
            return null;
        return this.gridWindow.getTab(pTabNo);
    };

    /**
     * get Parent Tab No
     * @return Tab No
     */
    GridTab.prototype.getParentTabNo = function () {
        var tabNo = this.vo.tabNo;
        var currentLevel = this.vo.TabLevel;

        var parentLevel = currentLevel - 1;

        if (parentLevel < 0)
            return tabNo;

        while (parentLevel < currentLevel && tabNo > 0) {
            tabNo--;
            currentLevel = VIS.context.getContextAsInt(this.vo.windowNo, tabNo, "TabLevel"); //Replace this magic string 
        }
        return tabNo;
    };

    GridTab.prototype.getAD_Tab_ID = function () {
        return this.vo.AD_Tab_ID;
    };

    GridTab.prototype.getAD_Window_ID = function () {
        return this.vo.AD_Window_ID;
    };

    /**
      *	Get Table ID
      *  @return Table ID
      */
    GridTab.prototype.getAD_Table_ID = function () {
        return this.vo.AD_Table_ID;
    };	//	getAD_Table_ID

    GridTab.prototype.getTableName = function () {
        return this.vo.TableName;
    };

    GridTab.prototype.getName = function () {
        return this.vo.Name;
    };

    GridTab.prototype.getDescription = function () {
        return this.vo.Description;
    };

    GridTab.prototype.getIsSortTab = function () {
        return this.vo.IsSortTab;
    };

    GridTab.prototype.getDependentOn = function () {

        var list = [];
        //  Display
        VIS.Evaluator.parseDepends(list, this.vo.DisplayLogic);
        //
        //if (list.Count > 0 && VLogMgt.IsLevelFiner())
        //{
        //  StringBuilder sb = new StringBuilder();
        // for (int i = 0; i < list.Count; i++)
        // sb.Append(list[i]).Append(" ");
        // log.Finer("(" + _vo.Name + ") " + sb.ToString());
        // }
        return list;
    };

    GridTab.prototype.getTabLevel = function () {
        return this.vo.TabLevel;
    };

    GridTab.prototype.getWindowNo = function () {
        return this.vo.windowNo;
    };

    GridTab.prototype.getTabNo = function () {
        return this.vo.tabNo;
    };

    GridTab.prototype.getIsSingleRow = function () {
        return this.vo.IsSingleRow;
    }

    GridTab.prototype.getTabLayout = function () {
        return this.vo.TabLayout;
    }

    //NewRecordView
    GridTab.prototype.getNewRecordView = function () {
        return this.vo.NewRecordView;
    }

    GridTab.prototype.getIsDisplayed = function (initialSetup) {
        //  no restrictions

        var dl = this.vo.DisplayLogic;
        if (dl == null || dl.equals(""))
            return true;

        if (initialSetup) {
            if (dl.indexOf("@#") != -1)		//	global variable
            {
                var parsed = VIS.Env.parseContext(VIS.context, 0, dl, false, true).trim();
                if (parsed.Length != 0)	//	variable defined
                    return VIS.Evaluator.evaluateLogic(this, dl);
            }
            return true;
        }
        //
        var retValue = VIS.Evaluator.evaluateLogic(this, dl);
        ////log.Config(_vo.Name + " (" + dl + ") => " + retValue);
        return retValue;

    };	//	i

    GridTab.prototype.getValueAsString = function (variableName) {
        var value = VIS.context.getWindowContext(this.vo.windowNo, this.vo.tabNo, variableName, true);
        if (!value) {
            return '';
        }
        return value.toString();
    };

    GridTab.prototype.getIsCurrent = function () {
        /// <summary>
        /*
         *	Is the tab current?.
         *  <pre>
         *	Yes 	- Table must be open
         *			- Query String is the same
         *			- Not Detail
         *			- Old link column value is same as current one
         *  </pre>
         *  @return true if current
         */
        /// </summary>
        /// <returns></returns>

        if (!this.gridTable.getIsOpen())
            return false;
        //	Same Query
        if (!this.oldQuery.equals(this.query.getWhereClause()))
            return false;
        //	Detail?
        if (!this.getIsDetail())
            return true;
        ////	Same link column value
        var value = VIS.context.getWindowContext(this.vo.windowNo, this.getLinkColumnName());
        return this.linkValue.equals(value);
        //return false;
    };//	isCurrent

    GridTab.prototype.getIsDetail = function () {
        /// <summary>
        /// Returns true if this is a detail record
        /// </summary>
        /// <returns></returns>
        //	We have IsParent columns and/or a link column
        if (this.parents.length > 0 || this.vo.AD_Column_ID != 0)
            return true;
        return false;
    };	// Details

    GridTab.prototype.getField = function (columnName) {
        return this.gridTable.getField(columnName);
    };

    GridTab.prototype.getFieldById = function (fid) {
        return this.gridTable.getFieldById(fid);
    };

    GridTab.prototype.getFields = function (columnName) {
        return this.gridTable.getFields();
    };

    GridTab.prototype.getLinkColumnName = function () {
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        if (this.linkColumnName)
            return this.linkColumnName;
        return "";
    };

    GridTab.prototype.getOnlyCurrentDays = function () {
        // if zoom enabled- return 0, no need to check any thing regarding transaction window.
        if (this.getIsZoomAction())
            return 0;
        return this.vo.onlyCurrentDays;
    };

    /* <summary>
      Show Summary Level Nodes Only
     </summary>
   */
    GridTab.prototype.getShowSummaryLevel = function () {
        return this.vo.ShowSummaryLevel;
    };

    GridTab.prototype.getIsTPBottomAligned = function () {
        return this.vo.TabPanelAlignment == "H";
    };


    /* <summary>
       Has Tree
      </summary>
    */
    GridTab.prototype.getIsTreeTab = function () {
        return this.vo.HasTree;
    };


    GridTab.prototype.getRowCount = function () {
        return this.gridTable.getRowCount();
    };

    GridTab.prototype.getRecords = function () {
        return this.gridTable.getDataTable();
    };

    GridTab.prototype.getCurrentRow = function () {
        //if (m_currentRow != verifyRow(m_currentRow))
        //  setCurrentRow(m_mTable.getRowCount()-1, true);
        return this.currentRow;
    }   //  getCurrentRow

    GridTab.prototype.getOrderByClause = function (onlyCurrentDays) {
        //	First Prio: Tab Order By
        if (this.vo.orderByClause.length > 0)
            return this.vo.OrderByClause;

        //	Second Prio: Fields (save it)
        this.vo.orderByClause = "";
        for (var i = 0; i < 3; i++) {
            var order = this.orderBys[i];
            if (order && order != null && order != "" && order.length > 0) {
                if (this.vo.orderByClause.length > 0)
                    this.vo.OrderByClause += ",";
                this.vo.orderByClause += order;
            }
        }
        if (this.vo.orderByClause.Length > 0)
            return this.vo.orderByClause;

        //	Third Prio: onlyCurrentRows
        this.vo.orderByClause = "Created";
        if (onlyCurrentDays > 0)
            this.vo.orderByClause += " DESC";
        return this.vo.orderByClause;
    };	//	getOrderByClause

    GridTab.prototype.getTableModel = function () {
        return this.gridTable;
    }; //returm GridTable 

    GridTab.prototype.getIsOpen = function () {
        if (this.gridTable != null)
            return this.gridTable.getIsOpen();
        return false;
    }; //IsOpen

    GridTab.prototype.getIsQueryActive = function () {
        if (this.query != null)
            return this.query.getIsActive();
        return false;
    };

    GridTab.prototype.getDependantFields = function (columnName) {
        var list = [];
        if (this.depOnFieldColumn.indexOf(columnName) != -1) {
            var size = this.depOnFieldColumn.length;
            for (var i = 0; i < size; i++) {
                if (this.depOnFieldColumn[i].equals(columnName))
                    if (list.indexOf(this.depOnField[i]) < 0)
                        list.push(this.depOnField[i]);
            }
        }
        return list;
    };

    GridTab.prototype.getIsReadOnly = function () {
        if (this.vo.IsReadOnly)
            return true;

        //  no restrictions
        if (this.vo.ReadOnlyLogic == null || this.vo.ReadOnlyLogic.equals(""))
            return this.vo.IsReadOnly;

        //  ** dynamic content **  uses get_ValueAsString
        var retValue = VIS.Evaluator.evaluateLogic(this, this.vo.ReadOnlyLogic);
        //log.finest(m_vo.Name
        //		+ " (" + m_vo.ReadOnlyLogic + ") => " + retValue);
        return retValue;
    };	//	isReadOnly

    /**
     *	Is Query New Record
     *  @return true if query active
     */
    GridTab.prototype.getIsQueryNewRecord = function () {
        if (this.query != null)
            return this.query.getIsNewRecordQuery();
        return false;
    };	//

    GridTab.prototype.getIsInsertRecord = function () {
        if (this.getIsReadOnly())
            return false;
        return this.vo.IsInsertRecord;
    };

    GridTab.prototype.getIncluded_Tab_ID = function () {
        return this.vo.Included_Tab_ID;
    };	//	getIncluded_Tab_ID

    GridTab.prototype.getIsAlwaysUpdateField = function () {
        for (var i = 0; i < this.gridTable.getColumnCount(); i++) {
            var field = this.gridTable.getField(i);
            if (field.getIsAlwaysUpdateable())
                return true;
        }
        return false;
    };	//	isAlwaysUpdateField

    GridTab.prototype.getValue = function (columnName) {
        if (columnName == null)
            return null;
        var field = this.gridTable.getField(columnName);
        return this.getValueOfField(field);
    };

    GridTab.prototype.getValueOfField = function (field) {
        if (field == null)
            return null;
        return field.getValue();
    };

    /**
     *	Get Process ID
     *  @return Process ID
     */
    GridTab.prototype.getAD_Process_ID = function () {
        return this.vo.AD_Process_ID;
    };	//	getAD_Process_ID

    /**
     *  Get Current Table Key ID
     *  @return Record_ID
     */
    GridTab.prototype.getRecord_ID = function () {
        return this.gridTable.getKeyID(this.currentRow);
    };   //  getRecord_ID

    /**
     *  Get Key ID of row
     *  @param  row row number
     *  @return The Key ID of the row or -1 if not found
     */
    GridTab.prototype.getKeyID = function (row) {
        return this.gridTable.getKeyID(row);
    };   //  get

    /**
     *  Get addition info message for status for for seleted tables
     * @param {any} tableName 
     * @param {any} ctx   Record context
     * @param {any} windowNo 
     * @param {any} tabNo  
     * @param {any} rec_id  primary key Id
     */

    GridTab.prototype.getFooterInfo = function (tableName, ctx, windowNo, tabNo, rec_id) {
        return new Promise(function (resolve, reject) {
            if (tableName.startsWith("C_Order") || tableName.startsWith("C_Invoice")) {
                var Record_ID;
                var isOrder = tableName.startsWith("C_Order");

                var mf = null;
                var mfMC = null;
                try {
                    mf = new VIS.MessageFormat(VIS.Msg.getMsg("OrderSummary"));
                    mfMC = new VIS.MessageFormat(VIS.Msg.getMsg("OrderSummaryMC"));
                }
                catch (e) {
                    reject("");
                    return;
                    //log.log(Level.SEVERE, "OrderSummary/MC", e);
                }
                if (mf == null || mfMC == null) {
                    resolve("");
                    return;
                }
                /**********************************************************************
                 *	** Message: OrderSummary/MC **
                 *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00} = {5}{4,number,#,##0.00}
                 *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00}
                 *
                 *	{0} - Number of lines
                 *	{1} - Line toral
                 *	{2} - Grand total (including tax, etc.)
                 *	{3} - Source Currency
                 *	(4) - Grand total converted to local currency
                 *	{5} - Base Currency
                 */

                //

                var Record_ID = 0;
                if (isOrder) {
                    Record_ID = ctx.getContextAsInt(windowNo, "C_Order_ID");
                }
                else {
                    Record_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
                }

                if (Record_ID < 1 && rec_id > 0)
                    Record_ID = rec_id;


                // var dr = null;
                $.ajax({
                    type: 'Get',
                    async: true,
                    url: VIS.Application.contextUrl + "Form/GetTrxInfo",
                    data: { Record_ID: Record_ID, isOrder: isOrder },
                    success: function (data) {
                        try {
                            var arguments = [];//new Object[6];
                            var filled = false;
                            var dr = new VIS.DB.DataReader().toJson(data);
                            var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
                            //dr = executeReader(sql.toString());
                            if (dr.read()) {
                                //	{0} - Number of lines
                                var lines = dr.getInt(0);
                                arguments[0] = lines;
                                //	{1} - Line toral
                                arguments[1] = format.getLocaleAmount(dr.getDecimal(2));
                                //	{2} - Grand total (including tax, etc.)

                                arguments[2] = format.getLocaleAmount(dr.getDecimal(3));
                                //	{3} - Currency
                                var currency = dr.getString(1);
                                arguments[3] = currency;
                                //	(4) - Grand total converted to Base

                                arguments[4] = format.getLocaleAmount(dr.getDecimal(4));
                                arguments[5] = ctx.getContext("$CurrencyISO");
                                filled = true;
                            }
                        }
                        catch (e) {
                            reject("");//log.log(Level.SEVERE, tableName + "\nSQL=" + sql, e);
                        }

                        if (filled) {
                            if (arguments[2] === arguments[4])
                                resolve(mf.format(arguments));
                            else
                                resolve(mfMC.format(arguments));
                        }
                        else
                            resolve(" ");
                    },
                    error: function (e) {
                        reject("");
                    }
                });
            }	//	Order || Invoice
        });
    };


    GridTab.prototype.getTrxInfo = function (tableName, ctx, windowNo, tabNo) {
        if (tableName.startsWith("C_Order") || tableName.startsWith("C_Invoice")) {
            var Record_ID;
            var isOrder = tableName.startsWith("C_Order");
            //
            //var sql = new StringBuilder("SELECT COUNT(*) AS Lines,c.ISO_Code,o.TotalLines,o.GrandTotal,"
            //		+ "CURRENCYBASEWITHCONVERSIONTYPE(o.GrandTotal,o.C_Currency_ID,o.DateAcct, o.AD_Client_ID,o.AD_Org_ID, o.C_CONVERSIONTYPE_ID) AS ConvAmt ");
            //if (isOrder) {
            //    Record_ID = ctx.getContextAsInt(windowNo, "C_Order_ID");
            //    sql.append("FROM C_Order o"
            //			+ " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
            //			+ " INNER JOIN C_OrderLine l ON (o.C_Order_ID=l.C_Order_ID) "
            //			+ "WHERE o.C_Order_ID=" + Record_ID + "");
            //}
            //else {
            //    Record_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
            //    sql.append("FROM C_Invoice o"
            //			+ " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
            //			+ " INNER JOIN C_InvoiceLine l ON (o.C_Invoice_ID=l.C_Invoice_ID) "
            //			+ "WHERE o.C_Invoice_ID=" + Record_ID + "");
            //}
            //sql.append("GROUP BY o.C_Currency_ID, c.ISO_Code, o.TotalLines, o.GrandTotal, o.DateAcct, o.AD_Client_ID, o.AD_Org_ID,o.C_CONVERSIONTYPE_ID");

            //log.fine(tableName + " - " + Record_ID);
            var mf = null;
            var mfMC = null;
            try {
                mf = new VIS.MessageFormat(VIS.Msg.getMsg("OrderSummary"));
                mfMC = new VIS.MessageFormat(VIS.Msg.getMsg("OrderSummaryMC"));
            }
            catch (e) {
                //log.log(Level.SEVERE, "OrderSummary/MC", e);
            }
            if (mf == null || mfMC == null)
                return " ";
            /**********************************************************************
             *	** Message: OrderSummary/MC **
             *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00} = {5}{4,number,#,##0.00}
             *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00}
             *
             *	{0} - Number of lines
             *	{1} - Line toral
             *	{2} - Grand total (including tax, etc.)
             *	{3} - Source Currency
             *	(4) - Grand total converted to local currency
             *	{5} - Base Currency
             */
            var arguments = [];//new Object[6];
            var filled = false;
            //

            var dr = null;

            try {

                var Record_ID = 0;
                if (isOrder) {
                    Record_ID = ctx.getContextAsInt(windowNo, "C_Order_ID");
                }
                else {
                    Record_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
                }

                var dr = null;
                $.ajax({
                    type: 'Get',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetTrxInfo",
                    data: { Record_ID: Record_ID, isOrder: isOrder },
                    success: function (data) {
                        dr = new VIS.DB.DataReader().toJson(data);
                    },
                });


                //dr = executeReader(sql.toString());
                if (dr.read()) {
                    //	{0} - Number of lines
                    var lines = dr.getInt(0);
                    arguments[0] = lines;
                    //	{1} - Line toral
                    var lineTotal = dr.getDecimal(2).toLocaleString();//.toFixed(2);
                    arguments[1] = lineTotal;
                    //	{2} - Grand total (including tax, etc.)
                    var grandTotal = dr.getDecimal(3).toLocaleString();//.toFixed(2);
                    arguments[2] = grandTotal;
                    //	{3} - Currency
                    var currency = dr.getString(1);
                    arguments[3] = currency;
                    //	(4) - Grand total converted to Base
                    var grandBase = dr.getDecimal(4).toLocaleString();//.toFixed(2);
                    arguments[4] = grandBase;
                    arguments[5] = ctx.getContext("$CurrencyISO");
                    filled = true;
                }
            }
            catch (e) {
                //log.log(Level.SEVERE, tableName + "\nSQL=" + sql, e);
            }
            finally {
                if (dr != null)
                    dr.dispose();
            }
            if (filled) {
                if (arguments[2] === arguments[4])
                    return mf.format(arguments);
                else
                    return mfMC.format(arguments);
            }
            return " ";
        }	//	Order || Invoice
    };

    /**
     * 	Is Processed
     *	@return true if current record is processed
     */
    GridTab.prototype.getIsProcessed = function () {
        var index = this.gridTable.findColumn("Processed");
        if (index != -1) {
            var oo = this.gridTable.getValueAt(this.currentRow, index);
            if (typeof (oo) == "string")
                return "Y".equals(oo);
            if (typeof (oo) == "boolean")
                return oo;
        }
        return "Y".equals(VIS.context.getWindowContext(this.vo.windowNo, this.vo.tabNo, "Processed"));
    };	//	

    GridTab.prototype.getKeyColumnName = function () {
        return this.keyColumnName;
    };

    /**
     * 	Get Order column for sort tab
     * 	@return AD_Column_ID
     */
    GridTab.prototype.getAD_ColumnSortOrder_ID = function () {
        return this.vo.AD_ColumnSortOrder_ID;
    };	//	getAD_ColumnSortOrder_ID




    /**
     * 	Get Order column for sort tab
     * 	@return AD_Column_ID
     */
    GridTab.prototype.getIsMapView = function () {
        if (this.vo.locationCols && this.vo.locationCols.length > 0)
            return true;
        return false;
    };

    GridTab.prototype.getMapColumns = function () {
        return this.vo.locationCols;
    };


    /**
     * 	Get Yes/No column for sort tab
     * 	@return AD_Column_ID
     */
    GridTab.prototype.getAD_ColumnSortYesNo_ID = function () {
        return this.vo.AD_ColumnSortYesNo_ID;
    };	//	getAD_ColumnSortYesNo_ID

    /* 
     * Get Tab Where Clause
     *
     */
    GridTab.prototype.getWhereClause = function () {
        return this.vo.WhereClause;
    };

    /*
     * Set where condition from outside 
     * @param {any} conition
     */
    //GridTab.prototype.setOuterWhereClause = function (conition) {
    //    this.outerWhereCondition = conition;
    //}

    /*
     *Reset outside condition
     */
    // GridTab.prototype.resetCard = function () {
    //    //this.setOuterWhereClause("");
    //    //this.getTableModel().setOuterOrderClause("");           
    //};

    GridTab.prototype.getSearchQuery = function (val) {
        var query = null;
        var fields = this.getFields();
        var f = null;
        for (var i = 0; i < fields.length; i++) {
            f = fields[i];
            if (f.getIsVirtualColumn())
                continue;
            if (f.getColumnName().toLowerCase() == "export_id")
                continue;

            if (VIS.DisplayType.IsText(f.getDisplayType())) {
                if (!query)
                    query = new VIS.Query(this.getTableName(), true);
                query.addRestriction(f.getColumnName(), VIS.Query.prototype.LIKE, val, f.getColumnName(), val);
            }
        }
        fields = null;
        f = null;
        return query;
    };

    GridTab.prototype.getParentColumnNames = function () {
        return this.parents;
    };



    GridTab.prototype.clearSelectedRow = function () {
        var size = this.gridTable.getFields().length;
        for (var i = 0; i < size; i++) {
            var mField = this.gridTable.getField(i);
            if (mField.getIsKey()) //refresh the context if the field is Key. This makes sure that Tab Level 2 and above works fine.
                mField.setValue(null, false);
            else
                mField.setNullValue();
        }
    };


    GridTab.prototype.setCurrentRow = function (tRow, fireEvents) {
        var recid = -1;
        var newRow = this.verifyRow(tRow);
        //if (newRow === this.currentRow) {
        //    return recid;
        //}
        this.currentRow = newRow;
        var rData = this.gridTable.getRow(this.currentRow);
        if (rData) {
            recid = rData.recid;
        }

        var size = this.gridTable.getFields().length;
        for (var i = 0; i < size; i++) {
            var mField = this.gridTable.getField(i);
            if (rData) {
                // var value = m_mTable.getValueAt(m_currentRow, i);
                var value = rData[mField.getColumnName().toLowerCase()];
                mField.setValue(value, this.gridTable.getIsInserting());
                //mField.validateValue();
            }
            else {
                if (mField.getIsKey()) //refresh the context if the field is Key. This makes sure that Tab Level 2 and above works fine.
                    mField.setValue(null, false);
                else
                    mField.setNullValue();
            }
        }
        if (!fireEvents)    //  prevents informing twice
            return recid;


        ////  inform VTable/..    -> rowChanged
        //m_propertyChangeSupport.firePropertyChange(PROPERTY, oldCurrentRow, m_currentRow);

        //  inform APanel/..    -> dataStatus with row updated
        if (this.mDataStatusEvent == null) {
            this.mDataStatusEvent = new DataStatusEvent(null, this.getRowCount(),
                this.gridTable.getIsInserting(),		//	changed
                //Env.getCtx().isAutoCommit(m_vo.WindowNo), m_mTable.isInserting());
                false, this.gridTable.getIsInserting());

            this.mDataStatusEvent.AD_Table_ID = this.getAD_Table_ID();

            this.mDataStatusEvent.setPageInfo(this.gridTable.currentPage, this.gridTable.rowCount, this.gridTable.pazeSize);
        }

        this.mDataStatusEvent.setCurrentRow(this.currentRow);
        var status = this.mDataStatusEvent.getAD_Message();
        if (status == null || status.length == 0)
            this.mDataStatusEvent.setInfo("NavigateOrUpdate", null, false, false);
        this.fireDataStatusChanged(this.mDataStatusEvent);
        return recid;
    };


    GridTab.prototype.fireDataStatusEventOnly = function () {
        //  inform APanel/..    -> dataStatus with row updated
        if (this.mDataStatusEvent == null) {
            this.mDataStatusEvent = new DataStatusEvent(null, this.getRowCount(),
                this.gridTable.getIsInserting(),		//	changed
                //Env.getCtx().isAutoCommit(m_vo.WindowNo), m_mTable.isInserting());
                false, this.gridTable.getIsInserting());
            this.mDataStatusEvent.AD_Table_ID = this.getAD_Table_ID();

            this.mDataStatusEvent.setPageInfo(this.gridTable.currentPage, this.gridTable.rowCount, this.gridTable.pazeSize);
        }

        this.mDataStatusEvent.setCurrentRow(this.currentRow);
        var status = this.mDataStatusEvent.getAD_Message();
        if (status == null || status.length == 0)
            this.mDataStatusEvent.setInfo("NavigateOrUpdate", null, false, false);
        this.fireDataStatusChanged(this.mDataStatusEvent);
    };

    //Set Query Object
    GridTab.prototype.setQuery = function (query) {
        if (query == null)
            this.query = new VIS.Query();
        else {
            this.query = query;
            this.vo.onlyCurrentDays = 0;
        }
    };

    /**
     *  Set New Value & call Callout
     *  @param field field
     *  @param value value
     *  @return error message or ""
     */
    GridTab.prototype.setValue = function (field, value) {

        if (!(field instanceof VIS.GridField)) {
            if (field == null)
                return "NoColumn";
            field = this.gridTable.getField(field);// field is columnname in this case
        }

        if (field == null)
            return "NoField";
        //log.fine(field.getColumnName() + "=" + value + " - Row=" + m_currentRow);
        var dispalyType = field.getDisplayType();

        if (value == null || value.toString() == "") {
            value = null;
        }
        else if (value === 0 && (VIS.DisplayType.IsID(dispalyType) || dispalyType == VIS.DisplayType.List) && !this.getIsStdColumn(field.getColumnName().toUpper())) {
            value = null;
        }

        var col = this.gridTable.findColumn(field.getColumnName());
        this.gridTable.setValueAt(value, this.currentRow, col, false);
        //
        return this.processFieldChange(field);
    };   //  setValue

    GridTab.prototype.getIsStdColumn = function (colName) {
        if (colName == "AD_USER_ID" || colName == "AD_ROLE_ID" || colName == "AD_CLIENT_ID" || colName == "AD_ORG_ID") {
            return true;
        }
        return false;
    };

    GridTab.prototype.addDataStatusListener = function (listner) {
        this.mDataListenerList.push(listner)
    };

    GridTab.prototype.removeDataStatusListener = function (listner) {
        for (var i = this.mDataListenerList.length - 1; i >= 0; i--) {
            if (this.mDataListenerList[i] === listner) {
                this.mDataListenerList[i] = null;
                this.mDataListenerList.splice(i, 1);
            }
        }
    };

    /**
     *  Do we need to Save?
     *  @param rowChange row change
     *  @param  onlyRealChange if true the value of a field was actually changed
     *  (e.g. for new records, which have not been changed) - default false
     *	@return true it needs to be saved
     */
    GridTab.prototype.needSave = function (rowChange, onlyRealChange) {
        if (rowChange) {
            return this.gridTable.needSave(-2, onlyRealChange);
        }
        else {
            if (onlyRealChange)
                return this.gridTable.needSave();
            else
                return this.gridTable.needSave(onlyRealChange);
        }
    };   //  isDataChanged

    GridTab.prototype.loadFields = function () {
        for (var i = 0; i < this.gTab._gridTable.m_fields.length; i++) {
            var gridField = new GridField(this.gTab._gridTable.m_fields[i]);
            gridField.setGridTab(this);
            this.gridTable.gridFields.push(gridField);
            if (gridField.getIsDefaultFocus()) {
                this.defaultFocusField = gridField
            }

            var columnName = gridField.getColumnName();
            //	Record Info
            if (gridField.getIsKey()) {
                this.keyColumnName = columnName;
                this.gridTable.setKeyColumnName(columnName);
            }
            //	Parent Column(s)
            if (gridField.getIsParentColumn())
                this.parents.push(columnName);
            //	Order By
            var sortNo = gridField.getSortNo();
            if (sortNo == 0) { }
            else if (Math.abs(sortNo) == 1) {
                this.orderBys[0] = columnName;
                if (sortNo < 0)
                    this.orderBys[0] += " DESC";
            }
            else if (Math.abs(sortNo) == 2) {
                this.orderBys[1] = columnName;
                if (sortNo < 0)
                    this.orderBys[1] += " DESC";
            }
            else if (Math.abs(sortNo) == 3) {
                this.orderBys[2] = columnName;
                if (sortNo < 0)
                    this.orderBys[2] += " DESC";
            }

            //  List of ColumnNames, this field is dependent on
            var list = gridField.getDependentOn();
            for (var ii = 0; ii < list.length; ii++) {
                this.depOnFieldColumn.push(list[ii]);   //  ColumnName, Field
                this.depOnField.push(gridField);
            }
            //  Add fields all fields are dependent on
            if (columnName.equals("IsActive")
                || columnName.equals("Processed")
                || columnName.equals("Processing")) {
                this.depOnFieldColumn.push(columnName);   //  ColumnName, Field
                this.depOnField.push(null);
            }

            //this.gridTable.updateTableModel(gridField);
            gridField = null;
        }
    };

    GridTab.prototype.loadTabPanels = function () {
        if (this.gTab._panels && this.gTab._panels.length > 0) {
            this.hasPanel = true;
            for (var i = 0; i < this.gTab._panels.length; i++) {
                var gridTabPanel = new GridTabPanel(this.gTab._panels[i]);
                this.tabPanels.push(gridTabPanel);
            }
        }
        else {
            this.hasPanel = false;
        }
    };

    GridTab.prototype.loadHeaderPanelItems = function () {
        if (this.vo.HeaderItems) {
            this.isHeaderPanel = true;
            headerPanel = this.vo.HeaderItems;
            this.isHPanelNotShowInMultiRow = this.vo.HPanelNotShowInMultiRow;
        }
    };

    GridTab.prototype.getIsHeaderPanel = function () {
        return this.isHeaderPanel;
    };

    GridTab.prototype.getHeaderPanelItems = function () {
        return headerPanel;
    }

    GridTab.prototype.getHeaderHeight = function () {
        if (this.vo.HeaderHeight && this.vo.HeaderHeight > 0) {

            return this.vo.HeaderHeight + 'px';
        }
        else {
            return '200px';
        }
    };

    GridTab.prototype.getHeaderWidth = function () {
        if (this.vo.HeaderWidth && this.vo.HeaderWidth > 0) {
            return this.vo.HeaderWidth + 'px';
        }
        else {
            return '250px';
        }

    };

    GridTab.prototype.getHeaderBackColor = function () {
        return this.vo.HeaderBackColor;
    };

    GridTab.prototype.getHeaderPadding = function () {
        return this.vo.HeaderPadding;
    };

    GridTab.prototype.getHeaderHorizontal = function () {
        if (this.vo.HeaderAlignment.equals("H")) {
            return true
        }
        else {
            return false;
        }
    };

    GridTab.prototype.getHasPanel = function () {
        return this.hasPanel;
    }

    GridTab.prototype.getTabPanels = function () {
        return this.tabPanels;
    };

    GridTab.prototype.validateQuery = function (query) {
        if (query == null || query.getRestrictionCount() == 0)
            return null;

        //	Check: only one restriction
        if (query.getRestrictionCount() != 1) {
            //log.Fine("Ignored(More than 1 Restriction): " + query);
            return query.getWhereClause();
        }

        var colName = query.getColumnName(0);
        if (colName == null) {
            //log.Fine("Ignored(No Column): " + query);
            return query.getWhereClause();
        }
        //	a '(' in the name = function - don't try to resolve
        if (colName.indexOf('(') != -1) {
            //log.Fine("Ignored(Function): " + colName);
            return query.getWhereClause();
        }
        ////	OK - Query is valid 

        //	Zooms to the same Window (Parents, ..)
        var refColName = null;
        if (colName.equals("R_RequestRelated_ID"))
            refColName = "R_Request_ID";
        else if (colName.startsWith("C_DocType"))
            refColName = "C_DocType_ID";
        else if (colName.equals("CreatedBy") || colName.equals("UpdatedBy"))
            refColName = "AD_User_ID";
        else if (colName.equals("Orig_Order_ID"))
            refColName = "C_Order_ID";
        else if (colName.equals("Orig_InOut_ID"))
            refColName = "M_InOut_ID";
        if (refColName != null) {

            if (this.getField(refColName) != null) {
                query.setColumnName(0, refColName);
                //log.Fine("Column " + colName + " replaced with synonym " + refColName);
                return query.getWhereClause();
            }
            refColName = null;
        }

        ////	Simple Query. 
        if (this.getField(colName) != null) {
            //log.Fine("Field Found: " + colName);
            return query.getWhereClause();
        }

        //	Find Refernce Column e.g. BillTo_ID -> C_BPartner_Location_ID
        //var sql = "SELECT cc.ColumnName "
        //	+ "FROM AD_Column c"
        //	+ " INNER JOIN AD_Ref_Table r ON (c.AD_Reference_Value_ID=r.AD_Reference_ID)"
        //	+ " INNER JOIN AD_Column cc ON (r.Column_Key_ID=cc.AD_Column_ID) "
        //	+ "WHERE c.AD_Reference_ID IN (18,30)" 	//	Table/Search
        //	+ " AND c.ColumnName='" + colName + "'";
        var sql = "VIS_104";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@colName", colName);

        var dr = null;

        try {

            dr = executeReader(sql, param);
            if (dr.read())
                refColName = dr.getString(0);
        }
        catch (e) {
            this.log.log(Level.SEVERE, "(ref) - Column=" + colName, e);
            return query.getWhereClause();
        }
        finally {
            if (dr != null) {
                dr.dispose();
            }
        }
        //	Reference Column found
        if (refColName != null) {
            query.setColumnName(0, refColName);
            if (this.getField(refColName) != null) {
                this.log.fine("Column " + colName + " replaced with " + refColName);
                return query.getWhereClause();
            }
            colName = refColName;
        }

        //	Column NOT in Tab - create EXISTS subquery
        var tableName = null;
        var tabKeyColumn = this.getKeyColumnName();
        //	Column=SalesRep_ID, Key=AD_User_ID, Query=SalesRep_ID=101

        //sql = "SELECT t.TableName "
        //    + "FROM AD_Column c"
        //    + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
        //    + "WHERE c.ColumnName='" + colName + "' AND IsKey='Y'"		//	#1 Link Column
        //    + " AND EXISTS (SELECT * FROM AD_Column cc"
        //    + " WHERE cc.AD_Table_ID=t.AD_Table_ID AND cc.ColumnName='" + tabKeyColumn + "')";	//	#2 Tab Key Column

        sql = "VIS_105";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@colName", colName);
        param[1] = new VIS.DB.SqlParam("@tabKeyColumn", tabKeyColumn);


        try {
            dr = executeReader(sql, param);
            if (dr.read())
                tableName = dr.getString(0);
        }
        catch (e) {
            this.log.log(Level.SEVERE, "Column=" + colName + ", Key=" + tabKeyColumn, e);
            return null;
        }
        finally {
            if (dr != null) {
                dr.dispose();
            }
        }

        //	Special Reference Handling
        if (tabKeyColumn.equals("AD_Reference_ID")) {
            //	Column=AccessLevel, Key=AD_Reference_ID, Query=AccessLevel='6'
            //sql = "SELECT AD_Reference_ID FROM AD_Column WHERE ColumnName='" + colName + "'";

            sql = "VIS_106";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@colName", colName);
            var AD_Reference_ID = executeScalar(sql, param);

            return "AD_Reference_ID=" + AD_Reference_ID;
        }

        //	Causes could be functions in query
        //	e.g. Column=UPPER(Name), Key=AD_Element_IDS, Query=UPPER(AD_Element.Name) LIKE '%CUSTOMER%'
        if (tableName == null) {
            this.log.info("Not successfull - Column="
                + colName + ", Key=" + tabKeyColumn
                + ", Query=" + query);
            return query.getWhereClause();
        }
        //else if(tableName.equals("M_WorkOrderComponent") && tabKeyColumn.equals("M_WorkOrder_ID")){
        //Object[][] results = QueryUtil.executeQuery((Trx)null, "SELECT M_WorkOrderOperation_ID FROM M_WorkOrderComponent WHERE "+query.getWhereClause(),new Object[]{});
        //int M_WorkOrderOperation_ID = 0;
        //if(results != null && results.length>0 && results[0].length>0)
        //    if(results[0][0] instanceof BigDecimal)
        //        M_WorkOrderOperation_ID = ((BigDecimal)results[0][0]).intValue();
        //    else if(results[0][0] instanceof Integer)
        //        M_WorkOrderOperation_ID = (Integer)results[0][0];
        //query.setTableName("xx");
        //String result = "EXISTS (SELECT * FROM M_WorkOrderOperation xx WHERE xx.M_WorkOrderOperation_ID="+M_WorkOrderOperation_ID+" AND xx.M_WorkOrder_ID=M_WorkOrder.M_WorkOrder_ID)";
        //return result;
        //}

        query.setTableName("xx");
        var result = "EXISTS (SELECT * FROM "
            + tableName + " xx WHERE "
            + (query.getWhereClause(true))
            + " AND xx." + tabKeyColumn + "="
            + this.getTableName() + "." + tabKeyColumn + ")";
        this.log.fine(result);
        return result;
        // return "";
    };




    GridTab.prototype.setTreeNodeID = function (nodeID) {
        this.treeNode_ID = nodeID;
        this.gridTable.treeNode_ID = nodeID;
    };

    GridTab.prototype.setTreeID = function (treeID) {
        this.treeID = treeID;
        this.gridTable.treeID = treeID;
    };

    GridTab.prototype.setTreeTable = function (tableID) {
        this.treeTable_ID = tableID;
        this.gridTable.treeTable_ID = tableID;
    };

    GridTab.prototype.setShowSummaryNodes = function (ShowSummaryNodes) {
        this.ShowSummaryNodes = ShowSummaryNodes;
        this.gridTable.ShowSummaryNodes = ShowSummaryNodes;
    };

    GridTab.prototype.setIsZoomAction = function (ZoomAction) {
        this.isZoomAction = ZoomAction;
        this.gridTable.isZoomAction = ZoomAction;
    };
    GridTab.prototype.getIsZoomAction = function () {
        return this.isZoomAction;

    };

    /**
     * 
     * @param {any} onlyCurrentDays
     * @param {any} maxRows
     * @param {any} created
     * @param {any} isVisualEdtr
     */
    GridTab.prototype.prepareQuery = function (onlyCurrentDays, maxRows, created, isVisualEdtr) {

        var success = true;
        var queryDetailAll = false;

        VIS.context.clearTabContext(this.vo.windowNo, this.vo.tabNo);

        //	is it same query?
        var refresh = this.oldQuery.equals(this.query.getWhereClause())
            && this.vo.onlyCurrentDays == onlyCurrentDays;

        if (this.getIsTreeTab()) {

            if (!this.getShowSummaryLevel()) {
                this.gridTable.setDoPaging(false);// _gridTable.DoPaging = false;
            }
            if (this.getShowSummaryLevel()) {
                this.gridTable.setDoPaging(true);// _gridTable.DoPaging = false;
                refresh = false;
            }
        }

        this.oldQuery = this.query.getWhereClause();
        this.vo.onlyCurrentDays = onlyCurrentDays;

        var where = "";
        if (!isVisualEdtr) //show all tab in visual editor
        {
            where += this.vo.WhereClause;
        }


        if (!this.ShowSummaryNodes && this.getShowSummaryLevel()) {
            if (where.length > 0) {
                where += " AND IsSummary='N'";
            }
            else {
                where += " IsSummary='N'";
            }
        }

        //    _vo.WhereClause);

        if (this.getOnlyCurrentDays() > 0) {
            if (where.length > 0)
                where += " AND ";

            var showNotProcessed = this.findColumn("Processed") != -1;
            //	Show only unprocessed or the one updated within x days
            if (showNotProcessed)
                where += "(Processed='N' OR ";
            if (created)
                where += "Created>=";
            else
                where += "Updated>=";
            //	where.append("addDays(current_timestamp, -");
            where += "addDays(SysDate, -" +
                this.vo.onlyCurrentDays + ")";
            if (showNotProcessed)
                where += ")";
        }

        if (!this.isThroughRoleCenter) {
            if (this.getIsDetail()) {
                var lc = this.getLinkColumnName();
                if (lc.equals("")) {
                    //log.Warning("No link column");
                    if (where.length > 0)
                        where += " AND ";
                    where += " 2=3";
                    success = false;
                }
                else {
                    var value = "";
                    if (this.vo.tabNo > 0)
                        var value = VIS.context.getTabRecordContext(this.vo.windowNo, this.getParentTabNo(), lc, true);
                    else
                        var value = VIS.context.getTabRecordContext(this.vo.windowNo, this.getParentTabNo(), lc);

                    if (refresh) {
                        refresh = this.linkValue.equals(value);
                        queryDetailAll = !refresh;
                    }
                    this.linkValue = value;
                    //	Check validity
                    if (value.length == 0) {
                        //log.Warning("No value for link column " + lc);
                        if (where.length > 0)
                            where += " AND ";
                        where += " 2=4";
                        success = false;
                    }
                    else {
                        //	we have column and value
                        if (where.length > 0)
                            where += " AND ";
                        if ("NULL".equals(value.toUpper())) {
                            where += lc + " IS NULL ";
                            //log.Severe("Null Value of link column " + lc);
                        }
                        else {
                            where += lc + "=";
                            if (lc.endsWith("_ID"))
                                where += value;
                            else
                                where += "'" + value + "'";
                        }
                    }
                }
            }	//	isDetail
        }



        //	Final Query
        if (this.query.getIsActive()) {

            var q = this.validateQuery(this.query);
            if (q != null && !queryDetailAll) {
                if (where.length > 0)
                    where += " AND ";
                where += q;
            }

        }
        this.extendedWhere = where.toString();
        //if (this.oldCardQuery != this.cardWhereCondition) {
        //    refresh = false;
        //}
        //this.oldCardQuery = this.cardWhereCondition;
        //if (this.outerWhereCondition && this.outerWhereCondition.length>0) {           
        //    if (where.length > 0)
        //        where += " AND ";
        //    where += this.outerWhereCondition;
        //    refresh = false;
        //}

        /* Query */
        this.mDataStatusEvent = null; //reset 
        if (this.gridTable.getIsOpen()) {
            if (refresh) {
                this.gridTable.dataRefreshAll();
                //console.log("refreshAll");
            }
            else {
                this.gridTable.dataRequery(where.toString());
                //console.log("data requery");
            }
        }
        else {

            if (!this.isZoomAction) {

                //   var sqlDefaultSearch = "SELECT  AD_UserQuery.Code,ad_defaultuserquery.ad_user_id,ad_defaultuserquery.ad_tab_id FROM AD_UserQuery AD_UserQuery JOIN AD_DefaultUserQuery ad_defaultuserquery ON AD_UserQuery.AD_UserQuery_ID=ad_defaultuserquery.AD_UserQuery_ID WHERE AD_UserQuery.IsActive                 ='Y'" +
                //"AND ad_defaultuserquery.AD_User_ID=" + VIS.Env.getCtx().getAD_User_ID() + " AND AD_UserQuery.AD_Client_ID =" + VIS.Env.getCtx().getAD_Client_ID() + " AND (AD_UserQuery.AD_Tab_ID = " + this.gridTable.AD_Tab_ID + " OR AD_UserQuery.AD_Table_ID                 = " + this.gridTable.AD_Table_ID + " )";

                var sqlDefaultSearch = "VIS_107";
                var param = [];
                param[0] = new VIS.DB.SqlParam("@AD_User_ID", VIS.Env.getCtx().getAD_User_ID());
                param[1] = new VIS.DB.SqlParam("@AD_Client_ID", VIS.Env.getCtx().getAD_Client_ID());
                param[2] = new VIS.DB.SqlParam("@AD_Tab_ID", this.gridTable.AD_Tab_ID);
                param[3] = new VIS.DB.SqlParam("@AD_Table_ID", this.gridTable.AD_Table_ID);

                var queryCode = "";
                var data = executeDataSet(sqlDefaultSearch, param);
                if (data && data.tables[0].rows && data.tables[0].rows.length > 0) {
                    for (var i = 0; i < data.tables[0].rows.length; i++) {

                        if (VIS.Env.getCtx().getAD_User_ID() == data.tables[0].rows[i].cells["ad_user_id"] && this.gridTable.AD_Tab_ID == data.tables[0].rows[i].cells["ad_tab_id"]) {
                            queryCode = data.tables[0].rows[i].cells["code"];
                        }

                    }

                    //var queryCode = VIS.DB.executeScalar(sqlDefaultSearch);

                    if (queryCode) {
                        if (where.length > 0) {
                            where = where + " AND " + queryCode;
                        }
                        else {
                            where = queryCode;
                        }

                    }
                }
                //this.gridTable.setSelectWhereClause(where.toString());
                //this.gridTable.open(maxRows);
                //    // console.log("open");
                //}
                //else {
                //    this.gridTable.setSelectWhereClause(where.toString());
                //    this.gridTable.open(maxRows);
                //}
                // console.log("open");
            }

            //this.currentRow = -1;//reset
            this.gridTable.setSelectWhereClause(where.toString());
            this.gridTable.open(maxRows);
        }

        return success;
    };

    GridTab.prototype.dataRefresh = function () {
        var row = this.currentRow;
        var record = this.gridTable.dataRefresh(row);
        this.setCurrentRow(row, true);
        return record;
    };
    GridTab.prototype.dataRefreshAll = function () {
        /* Query */
        this.mDataStatusEvent = null; //reset 
        this.getTableModel().dataRefreshAll();
    };

    GridTab.prototype.dataIgnore = function () {
        if (this.gridTable.dataIgnore(true)) {
            this.setCurrentRow(this.currentRow, false);    //  re-load data
            if (this.currentRow < 0)
                this.currentRow = 0;
            this.gridTable.fireDataStatusIEvent("Ignored", "");
        }

        //log.fine("#" + m_vo.TabNo + "- fini");
    };   //  dataIgnore


    GridTab.prototype.getLinkWhereClause = function () {
        var where = "";
        if (this.getIsDetail()) {
            var lc = this.getLinkColumnName();
            if (lc.equals("")) {
                //log.Warning("No link column");
                if (where.length > 0)
                    where += " AND ";
                where += " 2=3";
            }
            else {

                var value = VIS.context.getTabRecordContext(this.vo.windowNo, this.getParentTabNo(), lc, true);

                lc = this.getTableName() + "." + lc;

                //	Check validity
                if (value.length == 0) {
                    //log.Warning("No value for link column " + lc);
                    if (where.length > 0)
                        where += " AND ";
                    where += " 2=4";
                }
                else {
                    //	we have column and value
                    if (where.length > 0)
                        where += " AND ";
                    if ("NULL".equals(value.toUpper())) {
                        where += lc + " IS NULL ";
                        //log.Severe("Null Value of link column " + lc);
                    }
                    else {
                        where += lc + "=";
                        if (lc.endsWith("_ID"))
                            where += value;
                        else
                            where += "'" + value + "'";
                    }
                }
            }
        }	//	isDetail
        return where;
    };

    GridTab.prototype.SetSelectedNode = function (selectedNode) {
        this.gridTable.selectedTreeNode = selectedNode;
    };

    GridTab.prototype.dataSave = function (manualCmd) {
        try {
            //check if current tab and its parents have changed.
            // if (hasChangedCurrentTabAndParents())
            ///   return false;

            var retValue = (this.gridTable.dataSave(manualCmd) == VIS.GridTable.prototype.SAVE_OK);
            if (manualCmd)
                this.setCurrentRow(this.currentRow, false);
            if (retValue) {

                // If the parent of the tab is also based on the same table, then we
                // need to refresh the parent tab. 
                // an example is BPartner->Vendor->Location tab. 
                // When Vendor is changed, the BPartner needs to be refreshed,
                // otherwise, the resultSet of BPartner and its DB will be out of Sync 
                // when Location is being updated.
                //refreshParentsSameTable();
            }
            return retValue;
        }
        catch (e) {
            this.log.log(VIS.Logging.Level.SEVERE, "#" + this.vo.tabNo + " - row=" + this.currentRow, e);
        }
        return false;
    };  //  dataSave

    GridTab.prototype.dataNew = function (copy) {

        if (!this.getIsInsertRecord()) {
            this.log.warning("Insert Not allowed in TabNo=" + this.vo.tabNo);
            return null;
        }
        //	Prevent New Where Main Record is processed
        if (this.vo.tabNo > 0) {
            var processed = "Y".equals(VIS.context.getWindowContext(this.vo.windowNo, "Processed"));
            //	boolean active = "Y".equals(m_vo.ctx.getContext( m_vo.WindowNo, "IsActive"));
            if (processed) {
                this.log.warning("Not allowed in TabNo=" + this.vo.tabNo + " -> Processed=" + processed);
                return null;
            }
            this.log.finest("Processed=" + processed);
        }
        var retValue = this.gridTable.dataNew(this.currentRow, copy);
        if (!retValue)
            return retValue;
        this.setCurrentRow(this.currentRow + 1, true);
        //  process all Callouts (no dependency check - assumed that settings are valid)
        var count = this.getFieldCount();
        this.gridTable.setDisableNotification(true);
        for (var i = 0; i < count; i++)
            this.processCallout(this.getField(i));
        //  check validity of defaults
        for (var i = 0; i < count; i++) {
            //this.getField(i).refreshLookup();
            // getField(i).validateValue();
            this.getField(i).setError(false);
        }
        this.gridTable.setDisableNotification(false);
        this.gridTable.fireDataStatusIEvent(copy ? "UpdateCopied" : "Insertdata", "");
        return retValue;

    }; // dataNew
    GridTab.prototype.dataDelete = function (indices) {
        var retValue = this.gridTable.dataDelete(indices, this.currentRow);
        this.setCurrentRow(this.currentRow, true);
        return retValue;
    };




    GridTab.prototype.findColumn = function (columnName) {
        return this.gridTable.findColumn(columnName);
    }; //findColumn

    GridTab.prototype.navigate = function (tRow, fireEvents, force) {

        var recid = -1;
        var newRow = null;
        if (!force) {
            if (tRow === this.currentRow) {
                return recid;
            }
            newRow = this.verifyRow(tRow);
            if (newRow === this.currentRow) {
                return recid;
            }
        }
        newRow = newRow != null ? newRow : this.currentRow;
        //  Check, if we have old uncommitted data
        if (this.gridTable.dataSave(newRow, false)) {
            recid = this.setCurrentRow(newRow, fireEvents)
        }
        return recid;
    }; //navigate

    GridTab.prototype.verifyRow = function (targetRow) {

        var newRow = targetRow;
        //  Table Open?
        if (!this.gridTable.getIsOpen()) {
            //log.severe ("Table " + m_mTable.getTableName() + " not open (Tab " + m_vo.AD_Tab_ID + ")");
            return -1;
        }
        //  Row Count
        var rows = this.getRowCount();
        if (rows == 0) {
            //log.fine("No Rows");
            return -1;
        }
        if (newRow >= rows) {
            newRow = rows - 1;
            //log.fine("Set to max Row: " + newRow);
        }
        else if (newRow < 0) {
            newRow = 0;
            //log.fine("Set to first Row");
        }
        return newRow;
    };   //  verifyRow

    GridTab.prototype.navigateRelative = function (rowChange) {
        return this.navigate(this.currentRow + rowChange);
    };   //  navigateRelative

    GridTab.prototype.hasDependants = function (columnName) {
        return this.depOnFieldColumn.indexOf(columnName) !== -1;
    }; //hasDependend

    GridTab.prototype.dataStatusChanged = function (e) {

        var oldCurrentRow = e.getCurrentRow();
        //  save it
        //  when sorted set current row to 0
        var msg = e.getAD_Message();
        //if (msg != null && msg.equals("Sorted"))
        //  setCurrentRow(0, true);
        //  set current row
        e.setCurrentRow(this.currentRow);

        this.mDataStatusEvent = e;

        //  Same row - update value
        if (oldCurrentRow == this.currentRow) {
            var field = this.gridTable.getField(e.getChangedColumn());
            if (field != null) {
            }
        }
        else    //  Redistribute Info with current row info
            this.fireDataStatusChanged(this.mDataStatusEvent);
    }; //dataStatusChanged

    GridTab.prototype.fireDataStatusChanged = function (e) {
        var listeners = this.mDataListenerList;
        if (listeners.length == 0)
            return;

        //  WHO Info
        if (e.getCurrentRow() >= 0) {
            e.Created = this.getValue("Created");
            e.CreatedBy = this.getValue("CreatedBy");
            e.Updated = this.getValue("Updated");
            e.UpdatedBy = this.getValue("UpdatedBy");
            e.Record_ID = this.getKeyID(e.getCurrentRow());  //this.getValue(this.keyColumnName);
            //  Info
            var info = new StringBuilder(this.getTableName());
            //  We have a key column
            if (this.keyColumnName != null && this.keyColumnName.length > 0) {
                info.append(" - ")
                    .append(this.keyColumnName).append("=").append(e.Record_ID);
            }
            else    //  we have multiple parents
            {
                for (var i = 0; i < this.parents.length; i++) {
                    var keyCol = this.parents[i];
                    info.append(" - ")
                        .append(keyCol).append("=").append(this.getValue(keyCol));
                }
            }
            e.Info = info.toString();
        }
        e.setInserting(this.gridTable.getIsInserting());
        //  Distribute/fire it
        for (var j = 0; j < listeners.length; j++) {//  DataStatusListener element : listeners)
            listeners[j].dataStatusChanged(e);
        }
    }; //fire


    /**
     *  Process Field Change - evaluate Dependencies and process Callouts.
     *
     *  called from MTab.setValue or GridController.dataStatusChanged
     *  @param changedField changed field
     *  @return error message or ""
     */
    GridTab.prototype.processFieldChange = function (changedField) {
        this.processDependencies(changedField);
        return this.processCallout(changedField);
    };   //  processFieldChange

    /**
     *  Evaluate Dependencies
     *  @param changedField changed field
     */
    GridTab.prototype.processDependencies = function (changedField) {
        var columnName = changedField.getColumnName();
        //	log.trace(log.l4_Data, "Changed Column", columnName);

        //  when column name is not in list of DependentOn fields - fini
        if (!this.hasDependants(columnName))
            return;

        //  Get dependent MFields (may be because of display or dynamic lookup)
        var list = this.getDependantFields(columnName);
        for (var i = 0; i < list.length; i++) {
            var dependentField = list[i];
            //	log.trace(log.l5_DData, "Dependent Field", dependentField==null ? "null" : dependentField.getColumnName());
            //  if the field has a lookup
            if (dependentField != null && dependentField.getLookup() instanceof VIS.MLookup) {
                var mLookup = dependentField.getLookup();
                //	log.trace(log.l6_Database, "Lookup Validation", mLookup.getValidation());
                //  if the lookup is dynamic (i.e. contains this columnName as variable)
                if (mLookup.getValidation().indexOf("@" + columnName + "@") != -1) {


                    // If C_BPartner_ID and C_BPartner_LOcation_ID is used  in same window, 
                    //then on change C_BPartner_ID system clears value of C_BPartner_LOcation_ID, bcoz C_BPartner_LOcation_ID is depednet on C_BPartner_ID.
                    // then in this case we will not refresh lookup of Location.
                    if (columnName == "C_BPartner_ID" && (dependentField.getColumnName() == "C_BPartner_Location_ID")) {
                        continue;
                    }
                    else {

                        if (dependentField.getValue() == null) {
                            continue;
                        }

                        this.log.fine(columnName + " changed - "
                            + dependentField.getColumnName() + " set to null");
                        if (dependentField.getValue() == null) {
                            return;
                        }
                        //  invalidate current selection
                        this.setValue(dependentField, null);
                    }
                }
            }
            if (dependentField != null && dependentField.getLookup() instanceof VIS.MLocatorLookup) {

                //var locLookup = dependentField.getLookup();
                //var valueAsInt = 0;
                //if (changedField.getValue() != null && changedField.getValue() instanceof Number)
                //    valueAsInt = changedField.getValue();
                //if( columnName.equals( "M_Warehouse_ID" ) )
                //{
                //    locLookup.setOnly_Warehouse_ID( valueAsInt );
                //}
                //if( columnName.equals( "M_Product_ID" ) )
                //{
                //    locLookup.setOnly_Product_ID( valueAsInt );
                //}
                //locLookup.setOnly_Outgoing(Env.getCtx().isSOTrx(m_vo.WindowNo ));
                //locLookup.refresh();
                //if( !locLookup.isValid( dependentField.getValue() ) )
                //    setValue(dependentField, null);
            }
        }   //  for all dependent fields
    };   //  processDe

    /**************************************************************************
     *  Process Callout(s).
     *  <p>
     *  The Callout is in the string of
     *  "class.method;class.method;"
     * If there is no class name, i.e. only a method name, the class is regarded
     * as CalloutSystem.
     *
     * @param field field
     * @return error message or ""
     * @see VIS.app.CalloutEngine
     */
    GridTab.prototype.processCallout = function (field) {
        var callout = field.getCallout();
        if (callout.length == 0)
            return "";
        //
        if (this.getIsProcessed())		//	only active records
            return "";			//	"DocProcessed";

        var value = field.getValue();
        var oldValue = field.getOldValue();
        //log.fine(field.getColumnName() + "=" + value
        //		+ " (" + callout + ") - old=" + oldValue);

        var st = new VIS.StringTokenizer(callout, ";,", false);
        while (st.hasMoreTokens())      //  for each callout
        {
            var cmd = st.nextToken().trim();
            var call = null;
            var method = null;
            var cClass = cmd;

            var methodStart = cmd.lastIndexOf(".");
            try {
                if (methodStart != -1)      //  no class
                {

                    cClass = cmd.substring(0, methodStart).replace("ViennaAdvantage", "VIS").replace("VAdvantage", "VIS");
                    var type = VIS.Utility.getFunctionByName(cClass, window);

                    if (!type && !cClass.startsWith("VIS")) {
                        //if callout is not found in module then try to run from ViennaAdvantage or VIS
                        // and callout class has module  prefix
                        cClass = "VIS" + cClass.substring(cClass.indexOf("."));
                        type = VIS.Utility.getFunctionByName(cClass, window);
                    }
                    call = new type();
                    method = cmd.substring(methodStart + 1);
                }
            }
            catch (e) {
                this.log.log(Level.SEVERE, cClass, e);
                return "Callout Invalid: " + cClass + " (" + e.toString() + ")";
            }


            if (call == null || method == null || method.length == 0)
                return "Callout Invalid: " + method;

            var retValue = "";
            try {
                retValue = call.start(VIS.context, method, this.vo.windowNo, this, field, value, oldValue);
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, "start", e);
                retValue = "Callout Invalid: " + e.toString();
                return retValue;
            }
            if (retValue && !retValue.toString().equals(""))		//	interrupt on first error
            {
                if (retValue instanceof Error) {
                    retValue = retValue.toString() + "-> " + cmd;
                }
                this.log.warning(retValue);
                return retValue;
            }
        }   //  for each callout
        return "";
    };

    /// <summary>
    /// //return Total Fields in Tab
    /// </summary>
    /// <returns></returns>
    // <author>Karan</author>
    GridTab.prototype.getFieldCount = function () {
        return this.gridTable.getColumnCount();
    }

    /// <summary>
    ///Get Chat_ID for this record.
    /// </summary>
    /// <returns>return ID or 0, if not found</returns>
    /// <author>Karan</author>
    GridTab.prototype.getCM_ChatID = function () {


        if (this.chats == null || this.chats.length == 0)
            this.loadChats();//call chat function
        if (this.chats == null)
            return 0;
        //get chatId
        var key = this.getRecord_ID();// _gridTable.GetKeyID(CurrentRow);
        //The given key was not present in the dictionary. Error
        //int value = Utility.Util.GetValueOfInt(this.chats[key].ToString());
        if (this.hasKey(this.chats, key)) {
            //get chat key value
            var value = VIS.Utility.Util.getValueOfInt(this.getKeyValue(this.chats, key));
            return value;
        }
        return 0;
    }

    /// <summary>
    ///Get CM_SubScribedID for this record.
    /// </summary>
    /// <returns>return ID or 0, if not found</returns>
    /// <author>Karan</author>
    GridTab.prototype.getCM_SubScribedID = function () {
        if (this._subscribe == null || this._subscribe.length == 0)
            this.loadSubscribe();//call subscribe function
        if (this._subscribe == null)
            return 0;
        //get subscribeId
        var key = this.getRecord_ID();//

        if (this.hasKey(this._subscribe, key)) {
            //get subscribe key value
            var value = VIS.Utility.Util.getValueOfInt(this.getKeyValue(this._subscribe, key));
            return value;
        }
        return 0;
    };



    /// <summary>
    ///Get Chat_ID for this record.
    /// </summary>
    /// <returns>return ID or 0, if not found</returns>
    /// <author>Karan</author>
    ///<date>26-may-2014<date>
    GridTab.prototype.hasKey = function (array, key) {
        for (var i = 0; i < array.length; i++) {
            if (array[i].ID === key)
                return true;
        }
        return false;

        //var ret = array[key];
        //if (ret) { return true; }

        //return false;s

    };

    /// <summary>
    ///Get Chat_ID for this record.
    /// </summary>
    /// <returns>return ID or 0, if not found</returns>
    /// <author>Karan</author>
    ///<date>26-may-2014<date>
    GridTab.prototype.getKeyValue = function (array, key) {
        for (var i = 0; i < array.length; i++) {
            if (array[i].ID === key)
                return array[i].value;
        }
        return 0;
    };

    /**
     *  Get Commit Warning
     *  @return commit warning
     */
    GridTab.prototype.getCommitWarning = function () {
        return this.vo.CommitWarning;
    };

    /// <summary>
    /// return filed by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    GridTab.prototype.getField = function (index) {
        return this.gridTable.getField(index);
    };


    GridTab.prototype.dispose = function () {
        this.gTab = null;
        this.vo = null;
        this.gridTable.removeDataStatusListener(this);
        this.gridTable.dispose();
        this.gridTable = null;
        if (this.mDataListenerList != null) {
            for (var i = this.mDataListenerList.length - 1; i >= 0; i--) {
                this.mDataListenerList[i] = null;
                this.mDataListenerList.splice(i, 1);
            }
        }
    };

    /// <summary>
    ///Record Is Locked
    /// </summary>
    /// <returns></returns>
    GridTab.prototype.getIsLocked = function () {
        if (!VIS.MRole.getIsPersonalLock())
            return false;

        if (this.isDataLoading)
            return false;

        if (this._Lock == null)
            this.loadLocks();
        if (this._Lock == null || (this._Lock.length == 0))
            return false;
        //
        var key = this.getRecord_ID();
        return this._Lock.indexOf(key) > -1;
    };

    /**************************************************************************
    * 	Load Locks for Table and User
    */
    GridTab.prototype.loadLocks = function () {
        var AD_User_ID = VIS.context.getAD_User_ID();
        //log.Fine("#" + _vo.tabNo + " - AD_User_ID=" + AD_User_ID);
        if (!this.canHaveAttachment())
            return;

        var sql = "VIS_108";

        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_User_ID", AD_User_ID.toString());
        param[1] = new VIS.DB.SqlParam("@AD_Table_ID", this.vo.AD_Table_ID.toString());

        var dr = null;
        try {
            if (this._Lock == null)
                this._Lock = []
            else
                this._Lock.length = 0;
            dr = executeReader(sql, param);
            while (dr.read()) {
                var key = VIS.Utility.Util.getValueOfInt(dr.get("Record_ID"));
                this._Lock.push(key);
            }
            dr.close();
            dr = null;
        }
        catch (e) {
            this.log.log(Level.SEVERE, sql, e);
        }
        this.log.fine("#" + this._Lock.length);
    };

    /// <summary>
    /// Load Subscribed record's IDs for this table
    /// </summary>
    /// <author>Karan</author>
    ///Date: 10-July-2014

    GridTab.prototype.loadSubscribe = function () {
        //if doesn't have attachment
        if (!this.canHaveAttachment())
            return;//return nothing
        //set query
        //var sql = "SELECT CM_Subscribe_ID, Record_ID FROM CM_Subscribe WHERE AD_User_ID=" + VIS.context.getAD_User_ID() + " AND AD_Table_ID=" + this.getAD_Table_ID();
        var sql = "VIS_109";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_User_ID", VIS.context.getAD_User_ID());
        param[1] = new VIS.DB.SqlParam("@AD_Table_ID", this.getAD_Table_ID());

        var dr = null;
        try {
            this._subscribe = [];


            if (this._subscribe == null)
            //create new list for chat
            {
                this._subscribe = {};
            }
            else
            //if contain chat then clear list
            {
                this._subscribe.length = 0;
            }



            //execute query
            dr = executeReader(sql, param);


            var key, value;//for recordId and chatId
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(1));
                value = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                this._subscribe.push({ ID: key, value: value });
            }

            dr = null;


            //int key, value;//for recordId and chatId
            //while (dr.Read())
            //{
            //    key = Util.GetValueOfInt(dr["Record_ID"].ToString());
            //    value = Util.GetValueOfInt(dr["CM_Subscribe_ID"].ToString());
            //    _subscribe[key] = value;
            //}
            //dr.Close();
            //dr = null;
        }
        catch (e) {
            //if (dr != null)
            //{
            //    dr.Close();
            //    dr = null;
            //}
            //// MessageBox.Show("No Chat");
            //log.Log(Level.SEVERE, sql, e);
        }
    };

    /// <summary>
    /// Load Chats for this table
    /// </summary>
    /// <author>Karan</author>
    ///Date: 23-May-2014
    GridTab.prototype.loadDocuments = function () {
        //if doesn't have attachment
        if (!this.canHaveAttachment())
            return;//return nothing
        //set query
        //var sql = "SELECT vadms_windowdoclink_id,record_id FROM vadms_windowdoclink WHERE ad_table_id=" + this.getAD_Table_ID();
        //var sql = "SELECT vadms_windowdoclink_id,record_id FROM vadms_windowdoclink wdl JOIN vadms_document doc "
        //         + " ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID  WHERE doc.vadms_docstatus!='DD' AND ad_table_id=" + this.getAD_Table_ID();

        var sql = "VIS_110";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@ad_table_id", this.getAD_Table_ID());

        var dr = null;
        try {

            this.viewDocument = [];


            if (this.viewDocument == null)
            //create new list for chat
            {
                this.viewDocument = {};
            }
            else
            //if contain chat then clear list
            {
                this.viewDocument.length = 0;
            }



            dr = executeReader(sql, param);

            var key, value;//for recordId and chatId
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(1));
                value = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                this.viewDocument.push({ ID: key, value: value });
            }

            dr = null;
        }
        catch (e) {

        };



    };


    GridTab.prototype.loadChats = function () {
        //if doesn't have attachment
        if (!this.canHaveAttachment())
            return;//return nothing
        //set query
        //var sql = "SELECT CM_Chat_ID, Record_ID FROM CM_Chat WHERE AD_Table_ID=" + this.getAD_Table_ID();
        var sql = "VIS_111";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Table_ID", this.getAD_Table_ID());

        var dr = null;
        try {

            this.chats = [];


            if (this.chats == null)
            //create new list for chat
            {
                this.chats = {};
            }
            else
            //if contain chat then clear list
            {
                this.chats.length = 0;
            }



            dr = executeReader(sql, param);

            var key, value;//for recordId and chatId
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(1));
                value = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                this.chats.push({ ID: key, value: value });
            }

            dr = null;
        }
        catch (e) {

        };



    };

    /// <summary>
    /// Can this tab have Attachments?.
    /// </summary>
    /// <returns>bool type true if record can have attachment</returns>
    /// <author>Karan</author>
    ///Date: 23-May-2014
    GridTab.prototype.canHaveAttachment = function () {
        if (this.keyColumnName.endsWith("_ID"))
            return true;
        return false;
    };


    /// <summary>
    /// Returns true, if current row has a Chat
    /// </summary>
    /// <author>Karan</author>
    /// <returns>Bool Type (return true if record has chat)</returns>
    ///<Date> 27-May-2014 </Date> 
    GridTab.prototype.hasChat = function () {

        if (this.isDataLoading)
            return false;
        if (this.chats == null)
            this.loadChats();//call load chat function
        if (this.chats == null)
            return false;
        //get chat id
        var key = this.getRecord_ID();//ridTable.GetKeyID(CurrentRow);


        return this.hasKey(this.chats, key);//return chatId
    };


    GridTab.prototype.hasDocument = function () {

        if (this.isDataLoading)
            return false;
        if (this.viewDocument == null)
            this.loadDocuments();//call load chat function
        if (this.viewDocument == null)
            return false;
        //get chat id
        var key = this.getRecord_ID();//ridTable.GetKeyID(CurrentRow);


        return this.hasKey(this.viewDocument, key);//return chatId
    };

    /// <summary>
    /// Returns true, if current row has a Subscribe
    /// </summary>
    /// <author>Karan</author>
    /// <returns>Bool Type (return true if record has chat)</returns>
    ///<Date> 10-July-2014 </Date> 
    GridTab.prototype.HasSubscribed = function () {

        if (this.isDataLoading)
            return false;
        if (this._subscribe == null)
            this.loadSubscribe();//call load chat function
        if (this._subscribe == null)
            return false;
        //get chat id
        var key = this.getRecord_ID();//ridTable.GetKeyID(CurrentRow);
        return this.hasKey(this._subscribe, key);//return subscribeId
    };

    //Lakhwinder
    GridTab.prototype.loadAttachments = function () {
        //log.fine("#" + m_vo.TabNo);
        if (!this.canHaveAttachment())
            return;

        //var sqlQry = "SELECT distinct att.AD_Attachment_ID, att.Record_ID FROM AD_Attachment att"
        //       + " INNER JOIN AD_Attachmentline al ON (al.AD_Attachment_id=att.AD_Attachment_id)"
        //       + " WHERE att.AD_Table_ID=" + this.getAD_Table_ID();


        var sqlQry = "VIS_112";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Table_ID", this.getAD_Table_ID());

        var dr = null;
        try {
            this._attachments = [];

            dr = executeReader(sqlQry, param);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(1));
                value = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                this._attachments.push({ ID: key, value: value });
            }

            dr = null;

        }
        catch (ex) {
        }

    };
    GridTab.prototype.loadMarking = function () {
        //log.fine("#" + m_vo.TabNo);


        // var sqlQry = "SELECT AD_ExportData_ID, Record_ID FROM AD_ExportData WHERE AD_Table_ID=" + this.getAD_Table_ID()
        var sqlQry = "VIS_113";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Table_ID", this.getAD_Table_ID());
        var dr = null;
        try {
            this._marking = [];

            dr = executeReader(sqlQry, param);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(1));
                value = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                this._marking.push({ ID: key, value: value });
            }

            dr = null;

        }
        catch (ex) {
        }

    };

    GridTab.prototype.hasAttachment = function () {
        if (this.isDataLoading)
            return false;
        if (this._attachments == null)
            this.loadAttachments();
        if (this._attachments == null)
            return false;
        //
        var key = this.getRecord_ID();// _gridTable.GetKeyID(CurrentRow);
        return this.hasKey(this._attachments, key);
    };
    GridTab.prototype.hasMarked = function () {
        if (this.isDataLoading)
            return false;
        if (this._marking == null)
            this.loadMarking();
        if (this._marking == null)
            return false;
        //
        var key = this.getRecord_ID();// _gridTable.GetKeyID(CurrentRow);
        return this.hasKey(this._marking, key);
    };

    /// <summary>
    /// 
    /// </summary>
    /// <author>Karan</author>
    /// <returns> </returns>
    ///<Date> 27-May-2014 </Date> 
    GridTab.prototype.loadData = function (windowVo) {

        //this.isDataLoading = true;
        //this.isExportLoading = true;
        //var sb = [];
        this.isDataLoading = false;
        this.isExportLoading = false;



        this._attachments = this.gTab._attachments;
        this.chats = this.gTab._chats;
        this._lock = this.gTab._pAccess;
        this._subscribe = this.gTab._subscribe;
        this.viewDocument = this.gTab._documents;

        //var tableIndex = {};
        //var ServerValues = {};
        //ServerValues.IsExportData = false;
        //ServerValues.IsAttachment = false;
        //ServerValues.IsChat = false;
        //ServerValues.IsPLock = false;
        //ServerValues.IsSubscribeRecord = false;
        //ServerValues.IsViewDocument = false;

        //if (windowVo.IsExportData) {
        //    tableIndex.AD_EXPORTDATA = sb.length;
        //    ServerValues.IsExportData = true;
        //    sb.push("");
        //    //sb.push("SELECT AD_EXPORTDATA_ID,RECORD_ID, AD_ColOne_ID FROM AD_ExportData WHERE AD_Table_ID=" + this.getAD_Table_ID());
        //    //sb.push("SELECT AD_EXPORTDATA_ID,RECORD_ID, AD_ColOne_ID FROM AD_ExportData WHERE AD_Table_ID=" + this.getAD_Table_ID());

        //}

        //if (this.canHaveAttachment()) {

        //    if (windowVo.IsAttachment) {
        //        ServerValues.IsAttachment = true;
        //        tableIndex.AD_Attachment = sb.length;
        //        sb.push("");
        //        //sb.push("SELECT distinct att.AD_Attachment_ID, att.Record_ID FROM AD_Attachment att  INNER JOIN AD_AttachmentLine al ON (al.AD_Attachment_id=att.AD_Attachment_id)  WHERE att.AD_Table_ID=" + this.getAD_Table_ID());
        //    }
        //    if (windowVo.IsChat) {
        //        ServerValues.IsChat = true;
        //        tableIndex.CM_Chat = sb.length;
        //        sb.push("");
        //        //sb.push("SELECT CM_Chat_ID, Record_ID FROM CM_Chat WHERE AD_Table_ID=" + this.getAD_Table_ID());
        //    }
        //    if (VIS.MRole.getIsPersonalLock()) {
        //        ServerValues.IsPLock = true;
        //        tableIndex.AD_Private_Access = sb.length;
        //        sb.push("");
        //        //sb.push("SELECT Record_ID FROM AD_Private_Access WHERE AD_User_ID=" + VIS.context.getAD_User_ID() + " AND AD_Table_ID=" + this.getAD_Table_ID() + " AND IsActive='Y' ORDER BY Record_ID");
        //    }
        //    if (windowVo.IsSubscribeRecord) {
        //        ServerValues.IsSubscribeRecord = true;
        //        tableIndex.Cm_Subscribe = sb.length;
        //        sb.push("");
        //        //sb.push("Select cm_Subscribe_ID,Record_ID from CM_Subscribe where AD_User_ID=" + VIS.context.getAD_User_ID() + " AND ad_Table_ID=" + this.getAD_Table_ID());
        //    }
        //    if (windowVo.IsViewDocument && window.VADMS) {
        //        ServerValues.IsViewDocument = true;
        //        tableIndex.vadms_windowdoclink = sb.length;
        //        sb.push("");
        //        //sb.push("SELECT vadms_windowdoclink_id, record_id FROM VADMS_WindowDocLink wdl JOIN vadms_document doc ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID WHERE doc.vadms_docstatus!='DD' AND AD_Table_ID =" + this.getAD_Table_ID());
        //    }
        //}
        //ServerValues.AD_Table_ID = this.getAD_Table_ID();
        //ServerValues.AD_User_ID = VIS.context.getAD_User_ID();


        // sb = sb.join("~");


        //var ds = null;
        //try {
        //ds = executeDataSet(sb);


        //$.ajax({
        //    type: 'Get',
        //    //async: false,
        //    url: VIS.Application.contextUrl + "Form/LoadData",
        //    data: { value: JSON.stringify(ServerValues) },
        //    success: function (data) {
        //        dataSet = new VIS.DB.DataSet().toJson(data);
        //    },
        //});



        //if (ds != null && ds.getTables().length > 0) {
        //    //LoadExtraData(ds.Tables[0]);
        //    if (this.canHaveAttachment()) {

        //        var key, value;
        //        if (windowVo.IsAttachment) {



        //            var _dtAttachment = ds.getTables()[tableIndex.AD_Attachment];
        //            elements = null;
        //            //Attachment

        //            if (this._attachments == null)
        //                this._attachments = [];
        //            else
        //                this._attachments.length = 0;



        //            for (var i = 0; i < _dtAttachment.getRows().length; i++) {
        //                key = VIS.Utility.Util.getValueOfInt(_dtAttachment.getRows()[i].getCell("Record_ID"));
        //                value = VIS.Utility.Util.getValueOfInt(_dtAttachment.getRows()[i].getCell("AD_Attachment_ID"));
        //                this._attachments.push({ ID: key, value: value });
        //            }
        //        }

        //        if (windowVo.IsChat) {


        //            var _dtChat = ds.getTables()[tableIndex.CM_Chat];
        //            //Chat
        //            if (this.chats == null)
        //                //create new list for chat
        //                this.chats = [];
        //            else
        //                //if contain chat then clear list
        //                this.chats.length = 0;
        //            //execute query
        //            for (var i = 0; i < _dtChat.getRows().length; i++) {
        //                key = VIS.Utility.Util.getValueOfInt(_dtChat.getRows()[i].getCell("Record_ID"));
        //                value = VIS.Utility.Util.getValueOfInt(_dtChat.getRows()[i].getCell("CM_Chat_ID"));
        //                this.chats.push({ ID: key, value: value });
        //            }
        //        }

        //        if (VIS.MRole.getIsPersonalLock()) {

        //            var _dtLocked = ds.getTables()[tableIndex.AD_Private_Access];
        //            //Locks
        //            //this._Lock = [];
        //            if (this._Lock == null)
        //                this._Lock = [];
        //            else
        //                this._Lock.length = 0;

        //            for (var i = 0; i < _dtLocked.getRows().length; i++) {
        //                key = VIS.Utility.Util.getValueOfInt(_dtLocked.getRows()[i].getCell("Record_ID"));
        //                this._Lock.push(key);
        //            }
        //            //end Lock
        //        }
        //        if (windowVo.IsSubscribeRecord) {

        //            var _dtSubscribe = ds.getTables()[tableIndex.Cm_Subscribe];
        //            //Subscricbe
        //            if (this._subscribe == null)
        //                this._subscribe = [];
        //            else
        //                this._subscribe.length = 0;

        //            for (var i = 0; i < _dtSubscribe.getRows().length; i++) {
        //                key = VIS.Utility.Util.getValueOfInt(_dtSubscribe.getRows()[i].getCell("Record_ID"));
        //                value = VIS.Utility.Util.getValueOfInt(_dtSubscribe.getRows()[i].getCell("CM_Subscribe_ID"));
        //                this._subscribe.push({ ID: key, value: value });
        //            }
        //        }

        //        if (windowVo.IsViewDocument) {

        //            //var _dtViewDocument = ds.getTables()[tableIndex.vadms_windowdoclink];
        //            //  //ViewDocument
        //            //if (this.viewDocument == null)
        //            //    this.viewDocument = [];
        //            //else
        //            //    this.viewDocument.length=0;

        //            //for (var i = 0; i < _dtViewDocument.getRows().length; i++) {
        //            //    key = VIS.Utility.Util.getValueOfInt(_dtViewDocument.getRows()[i].getCell("Record_ID"));
        //            //    value = VIS.Utility.Util.getValueOfInt(_dtViewDocument.getRows()[i].getCell("vadms_windowdoclink_id"));
        //            //    this.viewDocument.push({ ID: key, value: value });
        //            //}

        //        }

        //        // this._attachments = [];







        //        this.isDataLoading = false;
        //        this.isExportLoading = false;
        //        tableIndex = null;
        //        // sb = null;
        //    }


        //}
        //else {
        //    this.isDataLoading = false;
        //    this.isExportLoading = false;
        //}
        //}
        //catch (e) {
        //    this.isDataLoading = false;
        //    this.isExportLoading = false;
        //}

    };

    /// <summary>
    /// Lock Record
    /// </summary>
    /// <param name="ctx">ctx context</param>
    /// <param name="Record_ID"></param>
    /// <param name="locks">true if lock, otherwise unlock</param>
    GridTab.prototype.locks = function (ctx, Record_ID, locks) {
        var AD_User_ID = ctx.getAD_User_ID();
        //log.Finer("Lock=" + locks + ", AD_User_ID=" + AD_User_ID
        //+ ", AD_Table_ID=" + _vo.AD_Table_ID + ", Record_ID=" + Record_ID);

        VIS.dataContext.updateInsertLocks(AD_User_ID, this.vo.AD_Table_ID, Record_ID, locks);
        this.loadLocks();
    };

    GridTab.prototype.setRecordStatus = function (data) {
        var aaa = data;

    };




    //********************* END *************************//



    //****************************************************//
    //**             GRID TABLE                        **//
    //**************************************************//

    /**
 *	Grid Table Model for data access including buffering.
 *  - The following data types are handled
 *			Integer		for all IDs
 *			Number	for all Numbers
 *			date	for all Dates
 *			String		for all others
 *  - The data is read via ajax request   and cached in m_buffer. Writes/updates
 *   are via dynamically constructed SQL INSERT/UPDATE statements. The record
 *   is re-read via the ajax request record only to get results of triggers.
 *
 *  - The model maintains and fires the requires TableModelEvent changes,
 *   the DataChanged events (loading, changed, etc.)
 *   as well as Vetoable Change event "RowChange"
 *   (for row changes initiated by moving the row in the table grid).
 *
 */

    function GridTable(gTable) {
        this.gTable = gTable;
        this.readOnly = this.gTable._readOnly;
        this.AD_Table_ID = gTable._AD_Table_ID;
        this.deleteable = this.gTable._deleteable;
        this.gridFields = [];
        this.keyColumnName = "";
        this.isOpen = false;
        this.inserting = false;
        this.SQL_Select = "";
        this.SQL_Count = "";
        this.SQL = "";
        this.bufferList = [];
        this.defaultModel = {};
        this.pazeSize = VIS.Env.getWINDOW_PAGE_SIZE();
        this.newRow = -1;
        this.compareDB = true;
        this.treeNode_ID = 0;
        this.treeTable_ID = 0;
        this.treeID = 0;
        this.selectedTreeNode = 0;
        this.ShowSummaryNodes = false;
        this.isZoomAction = false;
        /* Paging */
        this.currentPage;
        this.dopaging;

        gTable = null;
        this.mSortList;
        this.AD_Tab_ID = 0;
        this.log = VIS.Logging.VLogger.getVLogger("VIS.GridTable");
        //this.outerOrderClause = "";
        this.card_ID = 0;
    };

    GridTable.prototype.ctx = VIS.context;			//	the only OK condition

    GridTable.prototype.SAVE_OK = 'O';			//	the only OK condition
    /** Save Error - E	*/
    GridTable.prototype.SAVE_ERROR = 'E';
    /** Save Access Error - A	*/
    GridTable.prototype.SAVE_ACCESS = 'A';
    /** Save Mandatory Error - M	*/
    GridTable.prototype.SAVE_MANDATORY = 'M';
    /** Save Abort Error - U	*/
    GridTable.prototype.SAVE_ABORT = 'U';
    /** Save Regex Error - R	*/
    GridTable.prototype.SAVE_REGEX_ERROR = 'R';

    GridTable.prototype.getReadOnly = function () {
        return false;
    };	//

    GridTable.prototype.setAD_Tab_ID = function (tabid) {
        this.AD_Tab_ID = tabid;
    };

    GridTable.prototype.getField = function (identifier) {

        if (isNaN(identifier)) {
            if (identifier == null || identifier.length == 0)
                return null;
            var cols = this.gridFields.length;
            for (var i = 0; i < cols; i++) {
                var field = this.gridFields[i];
                if (identifier.equals(field.getColumnName(), true)) {
                    return field;
                }
            }
            //log.Log(Level.WARNING, "Not found: '" + identifier + "'");
            return null;
        }

        else {

            if (identifier < 0 || identifier >= this.gridFields.length)
                return null;
            return this.gridFields[identifier];
        }
    };

    GridTable.prototype.getFieldById = function (fid) {
        if (fid < 1)
            return null;
        var cols = this.gridFields.length;
        for (var i = 0; i < cols; i++) {
            var field = this.gridFields[i];
            if (fid === field.getAD_Field_ID()) {
                return field;
            }
        }
        return null;
    };


    GridTable.prototype.getIsInserting = function () {
        return this.inserting;
    };   //  isInserting

    GridTable.prototype.getColumnCount = function () {
        return this.gridFields.length;
    };	//

    GridTable.prototype.getFields = function () {
        return this.gridFields;
    };

    GridTable.prototype.getWhereClause = function (rowData) {
        var size = this.gridFields.length;
        var singleRowWHERE = null;
        var multiRowWHERE = null;
        for (var col = 0; col < size; col++) {
            var field = this.gridFields[col];
            if (field.getIsKey()) {
                var columnName = field.getColumnName().toLowerCase();
                var value = rowData[columnName];
                if (value == null) {
                    this.log.log(VIS.Logging.Level.WARNING, "PK data is null - " + columnName);
                    return null;
                }
                if (columnName.toUpperCase().endsWith("_ID"))
                    singleRowWHERE = new StringBuilder(columnName)
                        .append("=").append(value);
                else
                    singleRowWHERE = new StringBuilder(columnName)
                        .append("=").append(VIS.DB.to_string(value.toString()));
            }
            else if (field.getIsParentColumn()) {
                var columnName = field.getColumnName().toLowerCase();
                var value = rowData[columnName];
                if (value == null) {
                    this.log.log(Level.INFO, "FK data is null - " + columnName);
                    continue;
                }
                if (multiRowWHERE == null)
                    multiRowWHERE = new StringBuilder();
                else
                    multiRowWHERE.append(" AND ");
                if (columnName.toUpperCase().endsWith("_ID"))
                    multiRowWHERE.append(columnName)
                        .append("=").append(value);
                else
                    multiRowWHERE.append(columnName)
                        .append("=").append(VIS.DB.to_string(value.toString()));
            }
        }	//	for all columns
        if (singleRowWHERE != null)
            return singleRowWHERE.toString();
        if (multiRowWHERE != null)
            return multiRowWHERE.toString();
        this.log.log(Level.WARNING, "No key Found");
        return null;
    };

    GridTable.prototype.getIsOpen = function () {
        return this.isOpen;
    };	//

    GridTable.prototype.getTableID = function () {
        return this.gTable._AD_Table_ID;
    };

    GridTable.prototype.getDataTable = function () {
        return this.bufferList;
    };

    GridTable.prototype.getSortModel = function (mSort) {
        return this.mSortList;
    };

    GridTable.prototype.getRow = function (index) {
        return this.mSortList[index];
    };


    GridTable.prototype.getRowCount = function () {
        if (this.mSortList) {
            return this.mSortList.length;
        }
        if (this.bufferList) {
            return this.bufferList.length;
        }
        return 0;
    };

    GridTable.prototype.getTotalRowCount = function () {
        return this.rowCount;
    };

    GridTable.prototype.getValueAt = function (row, colName) {

        var rowData = this.getRow(row);
        if (rowData && (colName in rowData)) {
            return rowData[colName];
        }
        return null;
    };

    GridTable.prototype.checkMaxMin = function () {
        //  see also => ProcessParameter.saveParameter
        var sb = new StringBuilder();

        //	Check all columns
        var size = this.gridFields.length;
        var maxValue = "";
        var minValue = "";
        for (var i = 0; i < size; i++) {
            var field = this.gridFields[i];


            if (VIS.DisplayType.IsNumeric(field.getDisplayType())) {
                maxValue = field.getMaxValue();
                minValue = field.getMinValue();
                if ($.isNumeric(maxValue) && $.isNumeric(minValue)) {
                    if (Number(field.getValue()) > Number(field.getMaxValue()) || Number(field.getValue()) < Number(field.getMinValue())) {
                        VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_MustBetween") + " " + field.getMinValue() + " " + VIS.Msg.getMsg("VIS_And") + " " + field.getMaxValue());
                        return false;
                    }
                }
                else if ($.isNumeric(maxValue)) {
                    if (Number(field.getValue()) > Number(maxValue)) {
                        VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_Lessthan") + " " + maxValue);
                        return false;
                    }
                }
                else if ($.isNumeric(minValue)) {
                    if (Number(field.getValue()) < Number(minValue)) {
                        VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_Greaterthan") + " " + minValue);
                        return false;
                    }
                }
            }
        }

        //if (field.getIsMandatory(true))        //  check context
        //{
        //    var colName = field.getColumnName().toLowerCase();
        //    if (rowData[colName] == null || rowData[colName].toString().length == 0) {
        //        field.setInserting(true);  //  set editable otherwise deadlock
        //        field.setError(true);
        //        if (sb.length() > 0)
        //            sb.append(", ");
        //        sb.append(field.getHeader());
        //    }
        //}

        return true;
    };	//	getManda

    //GridTable.prototype.checkAmountDivisionValue = function () {

    //    var sb = new StringBuilder();

    //    //	Check all columns
    //    var size = this.gridFields.length;
    //    var maxValue = "";
    //    var minValue = "";

    //    var val = $.grep(this.gridFields, function (e) {
    //        if (e.getDisplayType() == VIS.DisplayType.AmtDimension) {
    //            return e;
    //        }
    //    });

    //    if (val) {
    //        for (var i = 0; i < size; i++) {
    //            var field = this.gridFields[i];
    //            if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
    //                var defValue = field.vo.DefaultValue;

    //                if (defValue) {
    //                    defValue = defValue.replace(/@/g, "").trim();
    //                    defValue = VIS.Env.getCtx().getContextAsInt(this.gTable._windowNo, defValue, false);

    //                    var fieldValue = field.getValue();
    //                    if (fieldValue && fieldValue > 0) {
    //                        //var str = "SELECT amount FROM C_DimAmt where C_DimAmt_ID=" + fieldValue;
    //                        //fieldValue = VIS.DB.executeScalar(str);
    //                        fieldValue = field.lookup.get(field.getValue());//.Name;
    //                        if (fieldValue) {
    //                            fieldValue = field.lookup.get(field.getValue()).Name;

    //                            if (parseFloat(defValue) < parseFloat(fieldValue)) {
    //                                VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + VIS.Msg.translate(VIS.Env.getCtx(), field.vo.DefaultValue.replace(/@/g, "").trim()) + VIS.Msg.getMsg("VIS_CannotBeLessThan") + field.getHeader());
    //                                return false;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return true;
    //    //for (var i = 0; i < size; i++) {
    //    //    var field = this.gridFields[i];



    //    //if (VIS.DisplayType.IsNumeric(field.getDisplayType())) {
    //    //    maxValue = field.getMaxValue();
    //    //    minValue = field.getMinValue();
    //    //    if ($.isNumeric(maxValue) && $.isNumeric(minValue)) {
    //    //        if (field.getValue() > field.getMaxValue() || field.getValue() < field.getMinValue()) {
    //    //            VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_MustBetween") + " " + field.getMinValue() + " " + VIS.Msg.getMsg("VIS_And") + field.getMaxValue());
    //    //            return false;
    //    //        }
    //    //    }
    //    //    else if ($.isNumeric(maxValue)) {
    //    //        if (field.getValue() > maxValue) {
    //    //            VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_Lessthan") + " " + maxValue);
    //    //            return false;
    //    //        }
    //    //    }
    //    //    else if ($.isNumeric(minValue)) {
    //    //        if (field.getValue() < minValue) {
    //    //            VIS.ADialog.error("ValidationError", true, ": " + VIS.Msg.getMsg("VIS_ValueOf") + " " + field.getHeader() + " " + VIS.Msg.getMsg("VIS_Greaterthan") + " " + minValue);
    //    //            return false;
    //    //        }
    //    //    }
    //    //}
    //    //}

    //    //this.gridFields[i].
    //};

    GridTable.prototype.getMandatory = function (rowData) {
        //  see also => ProcessParameter.saveParameter
        var sb = new StringBuilder();

        //	Check all columns
        var size = this.gridFields.length;
        for (var i = 0; i < size; i++) {
            var field = this.gridFields[i];
            if (field.getIsMandatory(true))        //  check context
            {
                var colName = field.getColumnName().toLowerCase();
                if (rowData[colName] == null || rowData[colName].toString().length == 0) {
                    field.setInserting(true);  //  set editable otherwise deadlock
                    field.setError(true);
                    if (sb.length() > 0)
                        sb.append(", ");
                    sb.append(field.getHeader());
                }
            }
        }

        if (sb.length() == 0)
            return "";
        return sb.toString();
    };	//	getManda

    GridTable.prototype.getErrorColumns = function () {
        //  see also => ProcessParameter.saveParameter
        var sb = new StringBuilder();

        //	Check all columns
        var size = this.gridFields.length;
        for (var i = 0; i < size; i++) {
            var field = this.gridFields[i];
            if (field.getIsError())        //  check context
            {
                if (sb.length() > 0)
                    sb.append(", ");
                sb.append(field.getHeader());
            }
        }

        if (sb.length() == 0)
            return "";
        return sb.toString();
    };	//	get

    GridTable.prototype.getClientOrg = function (row) {
        var AD_Client_ID = -1;
        if (this.gTable._indexClientColumn != -1) {
            var ii = this.getValueAt(row, 'ad_client_id');
            if (ii != null)
                AD_Client_ID = ii;
        }
        var AD_Org_ID = 0;
        if (this.gTable._indexOrgColumn != -1) {
            var oo = this.getValueAt(row, 'ad_org_id');
            if (oo != null)
                AD_Org_ID = oo;
        }
        return [AD_Client_ID, AD_Org_ID];
    };

    GridTable.prototype.getKeyID = function (row) {

        if (this.gTable._indexKeyColumn != -1) {
            try {
                var ii = this.getValueAt(row, this.keyColumnName.toLowerCase());
                if (ii == null || isNaN(ii))
                    return -1;
                return ii;
            }
            catch (e)     //  Alpha Key
            {
                return -1;
            }
        }
        return -1;
    };	//	getKeyID

    GridTable.prototype.setKeyColumnName = function (keyName) {
        this.keyColumnName = keyName;
    };

    GridTable.prototype.setTableName = function (newTableName) {
        if (this.isOpen) {
            //log.Fine("Table already open - ignored");
            return;
        }
        if (newTableName == null || newTableName.length == 0)
            return;
        this.gTable._tableName = newTableName;
    };

    GridTable.prototype.setReadOnly = function (value) {
        this.readOnly = value;
    };

    GridTable.prototype.setDeleteable = function (value) {
        this.gTable._deleteable = value;
    };

    GridTable.prototype.setOrderClause = function (newOrderClause) {
        this.gTable._orderClause = newOrderClause;
        if (this.gTable._orderClause == null)
            this.gTable._orderClause = "";
    };

    //GridTable.prototype.setOuterOrderClause = function (newOrderClause) {
    //    this.outerOrderClause = newOrderClause;
    //    if (this.outerOrderClause == null)
    //        this.outerOrderClause = "";
    //};

    GridTable.prototype.setSelectWhereClause = function (newWhereClause) {
        if (this.isOpen) {
            //log.Log(Level.SEVERE, "Table already open - ignored");
            return false;
        }
        //
        this.gTable._whereClause = newWhereClause;
        if (this.gTable._whereClause == null)
            this.gTable._whereClause = "";
        return true;
    };

    GridTable.prototype.setDoPaging = function (doPaging) {
        this.dopaging = doPaging;
    };

    GridTable.prototype.setCurrentPage = function (page) {
        this.currentPage = page;
    };

    GridTable.prototype.setCurrentPageRelative = function (newPage) {
        var cPage = this.currentPage;
        if (isNaN(newPage)) { //last page
            cPage = Math.ceil(this.rowCount / this.pazeSize);
        } else {
            if (newPage !== 0) {
                cPage += newPage;
            }
            else {
                cPage = 1;
            }
        }
        this.currentPage = cPage;
    };

    GridTable.prototype.setSortModel = function (mSort) {
        this.mSortList = null;
        this.mSortList = mSort;
    };
    /**
     * Set Card ID for featch card data
     * @param {any} cardID
     */
    GridTable.prototype.setCardID = function (cardID) {
        this.card_ID = cardID;
    };

    /**
     * Reset card details for other view
     * */
    GridTable.prototype.resetCard = function () {
        this.cardTempalte = null;
        this.setCardID(0);
    };

    GridTable.prototype.setValueAt = function (value, row, col, force) {
        //	Can we edit?
        if (!this.isOpen //  not accessible
            || row < 0 || col < 0   //  invalid index
            || this.rowCount == 0)     //  no rows
        {
            //console.finest("r=" + row + " c=" + col + " - R/O=" + m_readOnly + ", Rows=" + m_rowCount + " - Ignored");
            return;
        }

        var field = this.getField(col);

        var oldValue = this.getValueAt(row, field.getColumnName().toLowerCase());
        //  update MField

        if (!force && (
            oldValue == null && value == null
            || oldValue === value)) {
            // log.finest("r=" + row + " c=" + col + " - New=" + value + "==Old=" + oldValue + " - Ignored");
            return;
        }

        //dataSave(row, false);

        //	Has anything changed?
        //Object oldValue = getValueAt(row, col);
        //	Set Data item
        var _rowData = this.getRow(row);
        this.rowChanged = row;

        //	save original value - shallow copy
        if (this.rowData == null) {
            this.rowData = {};
            $.extend(this.rowData, _rowData);
        }

        //	save & update



        field.setValue(value, this.inserting);
        _rowData[field.getColumnName().toLowerCase()] = value;

        //  inform
        if (!this.disableNotification) {
            var evt = this.createDSE();
            evt.setChangedColumn(col, field.getColumnName());
            this.fireDataStatusChanged(evt);
        }
    };

    /**
     * set this flag to disable notification loop (callout) for new record inserting
     * @param {any} disable
     */
    GridTable.prototype.setDisableNotification = function (disable) {
        this.disableNotification = disable;
    };

    /**
     *	Check if the row needs to be saved.
     *  - only when row changed
     *  - only if nothing was changed
     *	@param	newRow to check
     *  @param  onlyRealChange if true the value of a field was actually changed
     *  (e.g. for new records, which have not been changed) - default false
     *	@return true it needs to be saved
     */
    GridTable.prototype.needSave = function (nRow, onlyChange) {
        //log.fine("Row=" + newRow +
        //		", Changed=" + m_rowChanged + "/" + m_changed);  //  m_rowChanged set in setValueAt

        var newRow = nRow;
        var onlyRealChange = onlyChange;

        if (arguments.length == 0) {
            newRow = this.rowChanged;
            onlyRealChange = false;
        }
        else if (arguments.length == 1) {
            newRow = arguments[0];
            onlyRealChange = false;
        }
        else {
            newRow = arguments[0];
            onlyRealChange = arguments[1];
        }

        //  nothing done
        if (typeof (this.changed) == "undefined" || (!this.changed && (this.rowChanged == -1 || typeof (this.rowChanged) == "undefined")))
            return false;
        //  E.g. New unchanged records
        if (this.changed && (typeof (this.rowChanged) == "undefined" || this.rowChanged == -1) && onlyRealChange)
            return false;
        //  same row
        if (newRow == this.rowChanged)
            return false;

        return true;
    }	//	needSave

    GridTable.prototype.findColumn = function (columnName) {
        var m_fields = this.gridFields;
        for (var i = 0; i < m_fields.length; i++) {
            var field = m_fields[i];
            if (columnName.equals(field.getColumnName(), true))
                return i;
        }
        return -1;
    };  //  

    GridTable.prototype.createSelectSql = function () {

        var gt = this.gTable;

        if (this.gridFields.length == 0 || gt._tableName == null || gt._tableName.equals(""))
            return "";

        //	Create SELECT Part
        var select = new StringBuilder("SELECT ");
        var selectDirect = null;
        var selectSql = null;
        var hasImage = false;
        var imgColName = [];
        for (var i = 0; i < this.gridFields.length; i++) {
            if (i > 0) {
                select.append(", ");
            }
            var field = this.gridFields[i];
            selectSql = field.getColumnSQL(true);
            if (selectSql.indexOf("@") == -1) {
                select.append(selectSql);	//	ColumnName or Virtual Column
            }
            else {
                select.append(VIS.Env.parseContext(this.ctx, gt._windowNo, selectSql, false));
            }

            if (field.getDisplayType() == VIS.DisplayType.Image) {
                imgColName.push(selectSql);
                hasImage = true;
                // select.append(", (SELECT ImageURL from AD_Image img where img.AD_Image_ID=" + gt._tableName+"."+ selectSql+") as imgUrlColumn");
            }

            if (field.getLookup() != null && field.getLookup() instanceof VIS.MLookup) {
                var lInfo = field.getLookup().info;
                if (lInfo.displayColSubQ && lInfo.displayColSubQ != "" && gt._tableName.toLowerCase() != lInfo.tableName.toLowerCase()) {


                    if (selectDirect == null)
                        selectDirect = new StringBuilder("SELECT ");
                    else
                        selectDirect.append(",");

                    var qryDirect = lInfo.queryDirect.substring(lInfo.queryDirect.lastIndexOf(' FROM ' + lInfo.tableName + ' '));

                    if (!field.getIsVirtualColumn())
                        qryDirect = qryDirect.replace('@key', gt._tableName + '.' + field.getColumnSQL());
                    else
                        qryDirect = qryDirect.replace('@key', field.getColumnSQL(false));


                    selectDirect.append("( SELECT (").append(lInfo.displayColSubQ).append(') ').append(qryDirect)
                        .append(" ) AS ").append(field.getColumnSQL() + '_T')
                        .append(',').append(field.getColumnSQL(true));
                }
            }

            else if (field.getLookup() != null && field.getLookup() instanceof VIS.MAccountLookup) {
                if (selectDirect == null)
                    selectDirect = new StringBuilder("SELECT ");
                else
                    selectDirect.append(",");

                selectDirect.append("( SELECT C_ValidCombination.Combination FROM C_ValidCombination WHERE C_ValidCombination.C_ValidCombination_ID=")
                    .append(gt._tableName + '.' + field.getColumnSQL()).append(" ) AS ").append(field.getColumnSQL() + '_T')
                    .append(',').append(field.getColumnSQL(true));

            }
        }

        selectSql = null;

        var randomNo = Math.random();
        if (hasImage) {
            for (var im = 0; im < imgColName.length; im++)
                select.append(", (SELECT ImageURL||'?random=" + randomNo + "' from AD_Image img where img.AD_Image_ID=CAST(" + gt._tableName + "." + imgColName[im] + " AS INTEGER)) as imgUrlColumn" + imgColName[im]);
        }

        //
        select.append(" FROM ").append(gt._tableName);


        this.SQL_Select = select.toString();
        this.SQL_Count = "SELECT COUNT(*) FROM " + gt._tableName;
        //

        var m_SQL_Where = new StringBuilder("");





        //	WHERE
        var _whereClause = gt._whereClause;

        if (_whereClause.length > 0) {
            m_SQL_Where.append(" WHERE ");
            if (_whereClause.indexOf("@") == -1) {
                m_SQL_Where.append(_whereClause);
            }
            else    //  replace variables
                m_SQL_Where.append(VIS.Env.parseContext(this.ctx, gt._windowNo, _whereClause, false));
            //
            if (_whereClause.toUpperCase().indexOf("=NULL") > 0) {
                this.log.Severe("Invalid NULL - " + _tableName + "=" + _whereClause);
            }
        }






        this.SQL = this.SQL_Select + m_SQL_Where.toString();
        this.SQL_Count += m_SQL_Where.toString();





        //List<GridField> jj = new List<GridField>();

        //	RO/RW Access

        if (gt._withAccessControl) {

            this.SQL = VIS.MRole.addAccessSQL(this.SQL,
                gt._tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO);

            //this.SQL_Count = "SELECT COUNT(*) FROM " + gt._tableName + this.SQL.replaceAll(this.SQL_Select, '');
            this.SQL_Count = "SELECT COUNT(*) FROM " + gt._tableName + this.SQL.substring(this.SQL_Select.length);
            // this.SQL_Count = VIS.MRole.addAccessSQL(this.SQL_Count,
            // gt._tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO);
        }

        if (selectDirect != null)
            this.SQL_Direct = selectDirect.toString() + ' ' + this.SQL_Count.substring(this.SQL_Count.indexOf(" COUNT(*) FROM") + 9);
        else
            this.SQL_Direct = "";

        //	ORDER BY

        //if (!this.outerOrderClause.equals("")) {
        //    this.SQL += " ORDER BY " + this.outerOrderClause;
        //    this.SQL_Direct += " ORDER BY " + this.outerOrderClause;
        //} else
        if (!gt._orderClause.equals("")) {
            this.SQL += " ORDER BY " + gt._orderClause;
            this.SQL_Direct += " ORDER BY " + gt._orderClause;
        }


        //
        ////log.fine(_SQL_Count);
        this.ctx.setWindowTabContext(gt._windowNo, gt._tabNo, "SQL", this.SQL);
        return this.SQL;
    };	//	createSelectSql

    GridTable.prototype.open = function (maxRows) {
        //TODO
        //log.Info("MaxRows=" + maxRows);
        this.maxRows = maxRows;
        if (this.isOpen) {
            //log.Fine("already open");
            this.dataRefreshAll();
            return true;
        }
        //	create _SQL and m_countSQL
        this.createSelectSql();
        if (this.SQL == null || this.SQL.equals("")) {
            //log.Log(Level.SEVERE, "No SQL");
            return false;
        }

        this.rowCount = 0;

        var rCount = 0;

        if (this.treeNode_ID > 0) {
            $.ajax({
                url: VIS.Application.contextUrl + "jsonData/GetRecordCountForTreeNode",
                data: { sqlIn: this.gTable._whereClause, AD_Table_ID: this.AD_Table_ID, treeID: this.treeID, treeNodeID: this.treeNode_ID, windowNo: this.gTable._windowNo, summaryOnly: this.ShowSummaryNodes },
                type: 'POST',
                async: false,
                success: function (resultt) {
                    rCount = JSON.parse(resultt);
                },
                error: function (e) { }
            });
        }
        else {
            if (this.card_ID > 0) {
                $.ajax({
                    url: VIS.Application.contextUrl + "jsonData/GetRecordCountWithCard",
                    data: { sql: VIS.secureEngine.encrypt(this.SQL_Count), cardID: this.card_ID },
                    type: 'POST',
                    async: false,
                    success: function (resultt) {
                        rCount = JSON.parse(resultt);
                    },
                    error: function (e) { }
                });
            } else {
                rCount = executeDScalar(this.SQL_Count, null);
            }

        }

        this.rowCount = rCount;

        if (this.rowCount > 0) {

            this.fillData();
            this.isOpen = true;
            //
            this.changed = false;
            this.rowChanged = -1;
        }
        else {
            //console.log("clear");
            if (this.bufferList != null) { this.bufferList.length = 0; };
            if (this.mSortList != null) {
                this.mSortList = null;
            }
            this.currentPage = 1;

            this.isOpen = true;
            //
            this.changed = false;
            this.rowChanged = -1;

            //call Query Complete
            window.setTimeout(function (t) {
                t.fireQueryCompleted(true);//Inform GridController   
            }, 300, this);
        }
        return true;
    };

    GridTable.prototype.fillData = function () {

        this.pazeSize = VIS.Env.getWINDOW_PAGE_SIZE();


        if (this.maxRows > 0 && this.rowCount > this.maxRows) {
            /************************ Set Max Rows ***********************************/
            // m_pstmt.setMaxRows(maxRows);
            this.pazeSize = this.maxRows;
            //info.Append(" - MaxRows=").Append(_maxRows);
            this.rowChanged = this.maxRows;
        }

        else if (!this.dopaging || (this.pazeSize > this.rowCount)) {
            this.pazeSize = this.rowCount;
        }

        /* Multi Delete may Decrease pages */
        if (this.dopaging) {
            if ((this.rowCount + this.pazeSize) <= (this.currentPage * this.pazeSize)) {
                --this.currentPage;
            }
        }


        this.bufferList.length = 0;
        var that = this;


        //prepare josn 


        this.SQL_Count = VIS.secureEngine.encrypt(this.SQL_Count);

        var gFieldsIn = this.createGridFieldArr(this.gridFields, true);
        var dataIn = { sql: this.SQL, page: this.dopaging ? this.currentPage : 0, pageSize: this.dopaging ? this.pazeSize : 0, treeID: 0, treeNode_ID: 0, card_ID: this.card_ID, ad_Tab_ID: this.AD_Tab_ID, tableName: this.gTable._tableName };

        var obscureFields = this.createObsecureFields(this.gridFields);


        dataIn.sqlDirect = VIS.secureEngine.encrypt(this.SQL_Direct);
        dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        // VIS.dataContext.getWindowRecords(dataIn, gFieldsIn, function (buffer) {
        if (this.treeNode_ID > 0) {
            //For On demand tree  add these parameter
            // Fetch window records based on selected node of tree.
            dataIn.treeID = this.treeID, dataIn.treeNode_ID = this.treeNode_ID;
            VIS.dataContext.getWindowRecordsForTreeNode(dataIn, gFieldsIn, this.rowCount, this.SQL_Count, this.AD_Table_ID, this.treeID, this.treeNode_ID, function (buffer) {

                try {

                    if (buffer != null) {
                        var count = 0;

                        if (buffer.getTables().length != 0) {

                            var rows = buffer.getTable(0).getRows();

                            var columns = buffer.getTable(0).getColumnsName();
                            for (var row = 0; row < rows.length; row++) {
                                var cells = rows[row].getJSCells();
                                for (var cell = 0; cell < columns.length; cell++) {

                                    cells[columns[cell]] = that.readDataOfColumn(columns[cell], cells[columns[cell]]);
                                }
                                //cells.recid = row;
                                that.bufferList[row] = cells;
                                count++;
                                //break;
                            }

                            //console.log(this.bufferList);
                        }
                        buffer.dispose();
                        buffer = null;
                    }
                }
                catch (e) {
                    //alert(e);
                    this.log.Log(Level.SEVERE, that.SQL, e);
                }
                that.fireQueryCompleted(true); // inform gridcontroller
                that = null;
            });
        }
        else {

            VIS.dataContext.getWindowRecords(dataIn, gFieldsIn, this.rowCount, this.SQL_Count, this.AD_Table_ID, obscureFields, function (buffer, lookupDirect, cardViewData) {

                try {

                    if (buffer != null) {
                        var count = 0;

                        if (buffer.getTables().length != 0) {

                            var rows = buffer.getTable(0).getRows();
                            var columns = buffer.getTable(0).getColumnsName();
                            for (var row = 0; row < rows.length; row++) {
                                var cells = rows[row].getJSCells();
                                for (var cell = 0; cell < columns.length; cell++) {

                                    cells[columns[cell]] = that.readDataOfColumn(columns[cell], cells[columns[cell]]);
                                }
                                //cells.recid = row;
                                that.bufferList[row] = cells;
                                count++;
                                //break;
                            }
                            console.log(buffer.getTable(0).lookupDirect);
                        }
                        buffer.dispose();
                        buffer = null;
                    }
                    if (lookupDirect)
                        VIS.MLookupCache.addRecordLookup(that.gTable._windowNo, that.gTable._tabNo, lookupDirect);

                    that.cardTempalte = cardViewData;
                    if (dataIn.card_ID > 0 && cardViewData && cardViewData.DisableWindowPageSize) {
                        that.pazeSize = that.rowCount;
                        that.currentPage = 1;
                    }
                }
                catch (e) {
                    //alert(e);
                    that.log.Log(Level.SEVERE, that.SQL, e);
                }
                that.fireQueryCompleted(true); // inform gridcontroller
                that = null;
            });
        }
    };
    /**
     * Get Card tempatate for cardview
     * */
    GridTable.prototype.getCardTemplate = function () {
        return this.cardTempalte;
    }


    GridTable.prototype.readDataOfColumn = function (colName, colValue) {


        if (colValue == null) {
            return colValue;
        }
        else {
            //	Column Info
            try {
                var field = this.getField(colName);
                if (field == null)
                    return colValue;
                var columnName = field.getColumnName();
                var displayType = field.getDisplayType();
                //	Integer, ID, Lookup (UpdatedBy is a numeric column)
                if ((VIS.DisplayType.IsID(displayType) // JJ: don't touch!
                    && (columnName.endsWith("_ID") || columnName.endsWith("_Acct")))
                    || columnName.endsWith("atedBy") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2")
                    || columnName.endsWith("_ID_3") || displayType == VIS.DisplayType.Integer) {
                    colValue = VIS.Utility.Util.getValueOfInt(colValue);	//	Integer
                }
                //	Number
                else if (VIS.DisplayType.IsNumeric(displayType)) {
                    colValue = VIS.Utility.Util.getValueOfDecimal(colValue);			//	BigDecimal
                }
                //	Date
                else if (VIS.DisplayType.IsDate(displayType)) {



                    //var dte = new Date(colValue);
                    //  dte.setMinutes(+dte.getTimezoneOffset() + dte.getMinutes());
                    //colValue = dte.toISOString();
                    dte = null;
                }
                //	RowID or Key (and Selection)
                else if (displayType == VIS.DisplayType.RowID)
                    colValue = null;
                //	YesNo
                else if (displayType == VIS.DisplayType.YesNo) {
                    var str = colValue.toString();
                    if (field.getIsEncryptedColumn())
                        str = this.decrypt(str);
                    colValue = str.equals("Y");	//	Boolean
                }
                //	LOB
                else if (VIS.DisplayType.IsLOB(displayType)) {
                    var value = colValue;
                    if (value == null)
                        colValue = null;

                    if (typeof value == typeof string) {
                        colValue = value.toString();
                    }
                    else {
                        colValue = value;
                    }
                }
                //	String
                else
                    colValue = colValue.toString();//string
                //	Encrypted
                if (field.getIsEncryptedColumn() && displayType != VIS.DisplayType.YesNo)
                    colValue = this.decrypt(colValue);

                if (field.getObscureType() && displayType != VIS.DisplayType.YesNo)
                    colValue = this.decrypt(colValue);
            }
            catch (e) {
                //log.Severe("Decryption =>" + e.Message);
                console.log(e);
            }
            return colValue;
        }
    };



    GridTable.prototype.encrypt = function (xx) {
        if (xx == null || xx.length < 1)
            return xx;
        return VIS.secureEngine.encrypt(xx);
    };

    /// <summary>
    /// Decrypt
    /// </summary>
    /// <param name="yy"></param>
    /// <returns></returns>
    GridTable.prototype.decrypt = function (yy) {
        if (yy == null || yy.length < 1)
            return yy;
        if (VIS.secureEngine.isEncrypted(yy))
            return VIS.secureEngine.decrypt(yy);
        return yy;
    };


    /**
     *	Check if it needs to be saved and save it.
     *  @param newRow row
     *  @param manualCmd manual command to save
     *	@return true if not needed to be saved or successful saved
     */
    GridTable.prototype.dataSave = function (newRow, manualCmd) {

        if (arguments.length == 2) {



            //log.fine("Row=" + newRow +
            //	", Changed=" + m_rowChanged + "/" + m_changed);  //  m_rowChanged set in setValueAt
            //  nothing done
            if (!this.changed && (typeof (this.rowChanged) == "undefined" || this.rowChanged == -1))
                return true;
            //  same row, don't save yet
            if (newRow == this.rowChanged)
                return true;

            return this.dataSaveL(manualCmd) == GridTable.prototype.SAVE_OK;
        }
        else {

            return this.dataSaveL(manualCmd);
        }
    }


    GridTable.prototype.dataSaveL = function (manualCmd) {
        //	cannot save
        if (!this.isOpen) {
            this.log.warning("Error - Open=" + this.isOpen);
            return this.SAVE_ERROR;
        }
        //	no need - not changed - row not positioned - no Value changed
        if (this.rowChanged == -1) {

            this.log.config("NoNeed - Changed=" + this.changed + ", Row=" + this.rowChanged);
            //	return SAVE_ERROR;
            if (!manualCmd)
                return this.SAVE_OK;
        }
        //  Value not changed
        if (this.rowData == null) {
            this.log.fine("No Changes");
            return this.SAVE_ERROR;
        }

        if (this.readOnly)
        //	If Processed - not editable (Find always editable)  -> ok for changing payment terms, etc.
        {
            log.warning("IsReadOnly - ignored");
            this.dataIgnore();
            return this.SAVE_ACCESS;
        }

        //	row not positioned - no Value changed
        if (this.rowChanged == -1) {
            if (this.newRow != -1)     //  new row and nothing changed - might be OK
                this.rowChanged = this.newRow;
            else {
                this.fireDataStatusEEvent("SaveErrorNoChange", "", false);
                return this.SAVE_ERROR;
            }
        }

        //	Can we change?
        var co = this.getClientOrg(this.rowChanged);
        var AD_Client_ID = co[0];
        var AD_Org_ID = co[1];
        var createError = true;
        if (!VIS.MRole.canUpdate(AD_Client_ID, AD_Org_ID, this.AD_Table_ID, 0, createError)) {
            // this.fireDataStatusEEvent("cant-update","",true);//CLogger.retrieveError());
            this.fireDataStatusEEvent("AccessTableNoUpdate", "", true);//CLogger.retrieveError());
            this.dataIgnore();
            return this.SAVE_ACCESS;
        }

        //  inform about data save action, if not manually initiated
        //try
        //{
        //    if (!manualCmd)
        //        m_vetoableChangeSupport.fireVetoableChange(PROPERTY, -1, m_rowChanged);
        //}
        //catch (PropertyVetoException pve)
        //{
        //    log.warning(pve.getMessage());
        //    dataIgnore();
        //    return SAVE_ABORT;
        //}

        //	get updated row data
        var rowDataNew = this.getRow(this.rowChanged);


        // regex validation
        for (var i = 0; i < this.gridFields.length; i++) {
            var rexfield = this.gridFields[i];
            if (VIS.DisplayType.IsString(rexfield.getDisplayType())) {
                if (rexfield.value != null) {
                    if (rexfield.value.toString().trim().length > 0) {
                        if (!new RegExp(rexfield.getVFormat()).test(rexfield.value)) {
                            this.fireDataStatusEEvent("RegexFailed", (rexfield.getHeader() == '' ? rexfield.getColumnName() : rexfield.getHeader()) + "\n", true);
                            //Work DOne to set focus in field whose value does not match with regular expression.
                            rexfield.propertyChangeListner.ctrl.focus();
                            return this.SAVE_ERROR;
                        }
                    }
                }
            }
        }
        //	Check Mandatory
        var missingColumns = this.getMandatory(rowDataNew);
        if (missingColumns.length != 0) {
            //	Trace.printStack(false, false);
            this.fireDataStatusEEvent("FillMandatory", missingColumns + "\n", true);
            return this.SAVE_MANDATORY;
        }


        if (!this.checkMaxMin()) {
            return;
        }

        //if (!this.checkAmountDivisionValue()) {
        //    return;
        //}


        //	Check miscellaneous errors
        var errorColumns = this.getErrorColumns();
        if (errorColumns.length != 0) {
            //	Trace.printStack(false, false);
            this.fireDataStatusEEvent("Error", errorColumns + "\n", true);
            return this.SAVE_ERROR;
        }

        var Record_ID = 0;
        if (!this.inserting)
            Record_ID = this.getKeyID(this.rowChanged);
        //try {
        //    if (!this.tableName.endsWith("_Trl"))	//	translation tables have no model
        //        return this.dataSavePO(Record_ID);
        //}
        //catch (e) {
        //    //if (e instanceof ClassNotFoundException)
        //    //    log.warning(m_tableName + " - " + e.getLocalizedMessage());
        //    //else {
        //    this.log.log(VIS.Logging.Level.SEVERE, "Persistency Issue - "
        //            + this.tableName + ": " + e.message, e);
        //    return this.SAVE_ERROR;
        //}

        /* Prepare Json */
        var gFields = [];
        var m_fields = this.gridFields;
        var size = m_fields.length;

        gFields = this.createGridFieldArr(m_fields);

        var encryptedCol = this.createGridFieldArr(m_fields, true);

        var obscureFields = this.createObsecureFields(m_fields);

        var RowData = {}, OldRowData = {};

        $.extend(true, RowData, rowDataNew);
        $.extend(true, OldRowData, this.rowData);

        if (encryptedCol && encryptedCol.length > 0) {
            var len = encryptedCol.length;
            for (var i = 0; i < len; i++) {
                if (RowData[encryptedCol[i]]) {
                    RowData[encryptedCol[i]] = this.encrypt(RowData[encryptedCol[i]]);
                }

                if (OldRowData[encryptedCol[i]]) {
                    OldRowData[encryptedCol[i]] = this.encrypt(OldRowData[encryptedCol[i]]);
                }
            }
        }

        if (obscureFields && obscureFields.length > 0) {
            var len = obscureFields.length;
            for (var i = 0; i < len; i++) {
                if (RowData[obscureFields[i]] || RowData[obscureFields[i]] == 0) {
                    RowData[obscureFields[i]] = this.encrypt(RowData[obscureFields[i]]);
                }

                if (OldRowData[obscureFields[i]] || OldRowData[obscureFields[i]] == 0) {
                    OldRowData[obscureFields[i]] = this.encrypt(OldRowData[obscureFields[i]]);
                }
            }
        }

        //var RowData = this.encryptRow(this.rowDataNew);

        //for (var i = 0; i < size; i++) {
        //    var field = m_fields[i];
        //    gFields.push({
        //        IsVirtualColumn: field.getIsVirtualColumn(),
        //        DisplayType: field.getDisplayType(),
        //        ColumnName: field.getColumnName(),
        //        IsKey: field.getIsKey(),
        //        ColumnSQL: field.getColumnSQL(true),
        //        IsEncrypted: field.getIsEncrypted(),
        //        IsParentColumn: field.getIsParentColumn()

        //    });
        //}

        var gridTableIn = {
            Record_ID: Record_ID,
            AD_Table_ID: this.AD_Table_ID,
            TableName: this.gTable._tableName,
            Inserting: this.inserting,
            CompareDB: this.compareDB,
            RowData: RowData,
            OldRowData: OldRowData,
            ManualCmd: manualCmd,
            WhereClause: this.getWhereClause(rowDataNew),
            Fields: gFields,
            AD_Client_ID: AD_Client_ID,
            AD_Org_ID: AD_Org_ID,
            SelectSQL: VIS.secureEngine.encrypt(this.SQL_Select),
            AD_WIndow_ID: m_fields[0].getAD_Window_ID(), // vinay bhatt window id
            MaintainVersions: false,
            UnqFields: this.gFieldUnique
            //ImmediateSave: true,
            //ValidFrom: new Date().toISOString(),
        };

        if (this.selectedTreeNode > 0) {
            gridTableIn['SelectedTreeNodeID'] = this.selectedTreeNode;
        }

        if (this.treeNode_ID > 0) {
            gridTableIn['ParentNodeID'] = this.treeNode_ID;
            gridTableIn['TreeID'] = this.treeID;
            gridTableIn['TreeTableID'] = this.treeTable_ID;
        }

        // check if this is master window and if there is change in maintain version field
        if (this.onlyCurrentDays == 0 && (this.IsMaintainVersions || this.maintainVersionFieldChanged(RowData, OldRowData))) {
            var self = this;
            // in case of new record in Master Version window
            if (OldRowData["updatedby"] == null) {
                if (!this.MaintainVerOnApproval || (this.MaintainVerOnApproval && VIS.context.getWindowContext(this.gTable._windowNo, VIS.Env.approveCol) == 'Y')) {
                    gridTableIn.MaintainVersions = true;
                    gridTableIn.ImmediateSave = true;
                    gridTableIn.ValidFrom = new Date().toISOString();
                    self.showVersions(self, Record_ID, gridTableIn, rowDataNew, true);
                    return;
                }
                var out = self.dataSaveDB(gridTableIn, rowDataNew);
                // check if there is workflow linked on version table
                // then do not save in Master window and reset 
                // and display message to user
                if (out.Status == "E") {
                    if (!(out.FireEEvent || out.FireIEvent))
                        VIS.ADialog.info(out.ErrorMsg);
                }
                else if (out.Status == "W") {
                    VIS.ADialog.info("SentForApproval");
                    self.dataRefreshAll();
                }
                return out.Status;
            }
            else if (this.MaintainVerOnApproval && VIS.context.getWindowContext(this.gTable._windowNo, VIS.Env.approveCol) != 'Y') {
                var out = this.dataSaveDB(gridTableIn, rowDataNew);
                return out.Status;
            }
            else {
                // applied check for checking future versions
                // if found any records then ask for confirmation to proceed
                var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Common/CheckVersions", { RowData: gridTableIn });
                if (res.result) {
                    VIS.ADialog.confirm("FoundVersions", true, "", "Confirm", function (result) {
                        if (result) {
                            self.showVersions(self, Record_ID, gridTableIn, rowDataNew, false);
                        }
                    });
                }
                else {
                    // in case of update display UI to user, 
                    // whether user want to save immediately or for future
                    self.showVersions(self, Record_ID, gridTableIn, rowDataNew, false);
                }
            }
        }
        else {
            var out = this.dataSaveDB(gridTableIn, rowDataNew);
            return out.Status;
        }
    };

    GridTable.prototype.showVersions = function (slf, rec_ID, gTblIn, rdNew, newRecord) {
        // in case of update display UI to user, 
        // whether user want to save immediately or for future
        var msVer = new VIS.MasterDataVersion(slf.gTable._tableName, slf.gridFields, rec_ID, gTblIn.WhereClause, slf.IsMaintainVersions, newRecord, function (immediate, valFrom, verRecID) {
            gTblIn.MaintainVersions = true;
            gTblIn.ImmediateSave = immediate;
            gTblIn.ValidFrom = new Date(valFrom).toISOString();
            gTblIn.VerRecID = verRecID;
            var out = slf.dataSaveDB(gTblIn, rdNew);
            // if Stauts is not OK
            if (out.Status != "O") {
                // if there is any error then display error message
                if (out.Status == "E") {
                    if (!(out.FireEEvent || out.FireIEvent))
                        VIS.ADialog.info(out.ErrorMsg);
                }
                else {
                    // in case of sucess and if not saved for immediate refresh UI
                    if (!immediate && !out.LatestVersion)
                        slf.dataRefreshAll();
                    // if sent for WF Approval then display Message
                    if (out.Status == "W")
                        VIS.ADialog.info("SentForApproval");
                    // if saved for future then display Message and refresh UI
                    else if (out.Status == "F")
                        VIS.ADialog.info("SavedForFuture");
                    else if (out.Status == "B")
                        VIS.ADialog.info("SavedForBackDate");
                }
            }
            return out.Status;
        });
        msVer.show();
    };

    GridTable.prototype.dataSaveDB = function (gridTableIn, rowDataNew) {
        var out = VIS.dataContext.insertOrUpdateWRecord(gridTableIn);

        out = JSON.parse(out);

        if (out.IsError) {
            //
        }
        else {
            if (out.RowData) {
                $.extend(true, rowDataNew, this.readDataOfObject(out.RowData));
            }

            this.fireTableModelChanged(VIS.VTable.prototype.ROW_REFRESH, rowDataNew, this.rowChanged);

            this.fireRowChanged(true, this.getKeyID(this.rowChanged));

            //	everything ok
            this.rowData = null;
            this.changed = false;
            this.compareDB = true;
            this.rowChanged = -1;
            this.newRow = -1;
            this.inserting = false;
        }

        if (out.ErrorMsg != null) {
            this.log.log(VIS.Logging.Level.SEVERE, out.ErrorMsg);
        }

        if (out.FireEEvent) {
            this.fireDataStatusEEvent(out.EventParam.Msg, out.EventParam.Info, out.EventParam.IsError, out.IsWarning);
        }
        else if (out.FireIEvent) {
            this.fireDataStatusIEvent(out.EventParam.Msg, out.EventParam.Info, out.EventParam.IsError);
        }

        return out;
    };

    GridTable.prototype.createGridFieldArr = function (m_fields, lessdata) {

        var size = m_fields.length;

        if (lessdata) {
            if (this.gFieldLessData)
                return this.gFieldLessData;
            else {
                this.gFieldLessData = [];

                for (var i = 0; i < size; i++) {
                    var field = m_fields[i];
                    var colName = field.getColumnName().toLowerCase();
                    if (field.getIsEncryptedColumn()) {
                        this.gFieldLessData.push(
                            //{
                            //IsVirtualColumn: field.getIsVirtualColumn(),
                            // DisplayType: field.getDisplayType(),
                            colName
                            // IsKey: field.getIsKey(),
                            // ColumnSQL: field.getColumnSQL(true),
                            // IsEncrypted: field.getIsEncrypted(),
                            // IsParentColumn: field.getIsParentColumn()
                        );
                    }

                }

                return this.gFieldLessData;
            }
        }
        else {
            if (this.gFieldData)
                return this.gFieldData;
            else {
                this.gFieldData = [];
                this.gFieldUnique = [];
                for (var i = 0; i < size; i++) {
                    var field = m_fields[i];
                    this.gFieldData.push({
                        IsVirtualColumn: field.getIsVirtualColumn(),
                        DisplayType: field.getDisplayType(),
                        ColumnName: field.getColumnName(),
                        IsKey: field.getIsKey(),
                        ColumnSQL: field.getColumnSQL(true),
                        IsEncryptedColumn: field.getIsEncryptedColumn(),
                        IsParentColumn: field.getIsParentColumn(),
                        Name: field.getHeader(),
                        IsUnique: field.getIsUnique(),
                        IsObscure: field.getObscureType() ? true : false
                    });
                    if (field.getIsUnique() && !field.getIsVirtualColumn()
                        && field.getIsDisplayed()) {
                        this.gFieldUnique.push(field.getColumnName());
                    }
                }
                return this.gFieldData;
            }
        }
    };

    GridTable.prototype.createObsecureFields = function (mfields) {
        var size = mfields.length;
        if (this.gFieldObscureList)
            return this.gFieldObscureList;
        else {
            this.gFieldObscureList = [];
            for (var i = 0; i < size; i++) {
                var field = mfields[i];
                if (field.getObscureType()) {
                    this.gFieldObscureList.push(field.getColumnName().toLowerCase());
                }

            }
            return this.gFieldObscureList;
        }
    };

    GridTable.prototype.dataRefreshAll = function () {
        this.inserting = false;	//	should not happen
        this.dataIgnore();
        this.close(false);
        this.open(this.maxRows);
        //	Info
        //this.rowData = null;
        //this.changed = false;
        //this.rowChanged = -1;
        //this.inserting = false;
        //fireTableDataChanged();
        this.dseEvent = "Refreshed";
        //this.fireDataStatusIEvent("Refreshed", "");
    };

    GridTable.prototype.dataRequery = function (whereClause) {
        this.close(false);
        this.setSelectWhereClause(whereClause);

        this.rowData = null;
        this.changed = false;
        this.rowChanged = -1;
        this.inserting = false;

        this.open(this.maxRows);
        //  Info

        //fireTableDataChanged();
        this.dseEvent = "Refreshed";
        //this.fireDataStatusIEvent("Refreshed", "");
        return true;
    };

    GridTable.prototype.dataRefresh = function (row) {
        if (row < 0 || this.getRowCount() == 0 || this.inserting)
            return null;

        var rData = this.getRow(row);
        //  ignore
        this.dataIgnore();

        //	Create SQL
        var where = this.getWhereClause(rData);
        if (where == null || where.length == 0)
            where = "1=2";
        var sql = this.SQL_Select + " WHERE " + where;

        var rowDataDB = null;
        var dr = null;
        try {

            sql = VIS.secureEngine.encrypt(sql);
            dr = VIS.dataContext.getWindowRecord(sql, this.createGridFieldArr(this.gridFields, true), this.createObsecureFields(this.gridFields));
            //	only one row
            if (dr.read())
                rowDataDB = this.readData(dr);
            rowDataDB.recid = rData.recid; //set record ID 
        }
        catch (e) {
            sql = VIS.secureEngine.decrypt(sql);
            this.log.log(Level.SEVERE, sql, e);
            this.fireDataStatusEEvent("RefreshError", sql, true);
            return null;
        }
        finally {
            if (dr != null) {
                dr.dispose();
            }
        }



        this.rowData = null;
        this.changed = false;
        this.rowChanged = -1;
        this.inserting = false;
        //fireTableRowsUpdated(row, row);
        this.fireTableModelChanged(VIS.VTable.prototype.ROW_REFRESH, rowDataDB);
        this.fireDataStatusIEvent("Refreshed", "");
        return rowDataDB;
    };

    /*
     *	New Record after current Row
     *  @param currentRow row
     *  @param copyCurrent copy
     *  @return true if success -
     *  Error info (Access*, AccessCannotInsert) is saved in the log
     */
    GridTable.prototype.dataNew = function (currentRow, copyCurrent) {
        this.log.info("Current=" + currentRow + ", Copy=" + copyCurrent);
        //  Read only
        if (this.readOnly) {
            this.fireDataStatusEEvent("AccessCannotInsert", "", true);
            return false;
        }

        /** @todo No TableLevel */
        //  || !Access.canViewInsert(m_ctx, m_WindowNo, tableLevel, true, true))
        //  fireDataStatusEvent(Log.retrieveError());

        //  see if we need to save

        //todo dataSave(-2, false);


        this.inserting = true;
        //	Create default data
        var size = this.gridFields.length;// .size();
        this.rowData = {}; // //	"original" data
        var rowData = {};

        var tempWindowNo = this.gTable._windowNo + VIS.EnvConstants.WINDOW_TEMP;
        //	fill data
        if (copyCurrent) {
            //MSort sort = m_sort.get(currentRow);
            var origData = this.getRow(currentRow);
            for (var i = 0; i < size; i++) {
                var field = this.gridFields[i];
                var columnName = field.getColumnName();
                var colNameLower = columnName.toLowerCase();

                if (field.getIsVirtualColumn())
                    ;
                else if (field.getIsKey()
                    || columnName.equals("AD_Client_ID")
                    || columnName.equals("IsActive")
                    //
                    || columnName.startsWith("Created") || columnName.startsWith("Updated")
                    || columnName.equals("EntityType") || columnName.equals("DocumentNo")
                    || columnName.equals("Processed") || columnName.equals("IsSelfService")
                    || columnName.equals("DocAction") || columnName.equals("DocStatus")
                    || columnName.startsWith("Ref_")
                    || columnName.equals("Posted")
                    //	Order/Invoice
                    || columnName.equals("GrandTotal") || columnName.equals("TotalLines")
                    || columnName.equals("C_CashLine_ID") || columnName.equals("C_Payment_ID")
                    || columnName.equals("IsPaid") || columnName.equals("IsAllocated")
                    || columnName.equalsIgnoreCase("C_Location_ID")
                    || columnName.equals("IsApproved") || columnName.equals("IsDelivered")
                    || columnName.equals("IsInvoiced") || columnName.equals("TotalDr")
                    || columnName.equals("TotalCr")
                    || columnName.equalsIgnoreCase("I_IsImported")) {
                    var oo = field.getDefault(this.ctx, tempWindowNo);
                    rowData[colNameLower] = oo;

                    if (oo != null)
                        this.ctx.setWindowContext(tempWindowNo, field.getColumnName(), oo.toString());
                }
                else if (field.getIsCopy()) {
                    var oo1 = origData[columnName.toLowerCase()];
                    rowData[colNameLower] = oo1;

                    if (oo1 != null)
                        this.ctx.setWindowContext(tempWindowNo, field.getColumnName(), oo1.toString());
                }
                else {
                    var oo = field.getDefault(this.ctx, tempWindowNo);
                    rowData[colNameLower] = oo;
                    if (oo != null)
                        this.ctx.setWindowContext(tempWindowNo, field.getColumnName(), oo.toString());
                }
            }
        }
        else	//	new
        {
            for (var i = 0; i < size; i++) {
                var field = this.gridFields[i];
                var oo = field.getDefault(this.ctx, tempWindowNo);
                rowData[field.getColumnName().toLowerCase()] = oo;
                if (oo != null)
                    this.ctx.setWindowContext(tempWindowNo, field.getColumnName(), oo.toString());
            }
        }


        //this.mQueryCompletedListener
        this.ctx.removeWindow(tempWindowNo);
        this.changed = true;
        this.compareDB = true;
        //		m_rowChanged = -1;  //  only changed in setValueAt
        this.newRow = currentRow + 1;

        //  if there is no record, the current row could be 0 (and not -1)
        if (this.getRowCount() < this.newRow)
            this.newRow = this.getRowCount();

        this.rowChanged = this.newRow;  //  force save checking on new record
        //	add Data at end of buffer

        //if (!this.mSortList) {

        //}
        //this.mSortList.splice(this.newRow, 0, rowData);
        //this.mSortList.inser .push(rowData);
        //	add Sort pointer
        //m_sort.add(m_newRow, sort);
        this.rowCount++;
        rowData.recid = this.rowCount;
        this.pazeSize++;
        //	inform
        //log.finer("Current=" + currentRow + ", New=" + m_newRow);
        this.fireTableModelChanged(VIS.VTable.prototype.ROW_ADD, rowData, this.newRow, null);
        //this.fireDataStatusIEvent(copyCurrent ? "UpdateCopied" : "Inserted", "");
        this.log.fine("Current=" + this.currentRow + ", New=" + this.newRow + " - complete");
        return true;
    };//	dataNew

    GridTable.prototype.dataDelete = function (selIndices, currentRow) {

        if (!selIndices || selIndices.length < 0)
            return false;

        //	Tab R/O
        if (this.readOnly) {
            this.fireDataStatusEEvent("AccessCannotDelete", "", true);	//	privileges
            return false;
        }

        //	Is this record deletable?
        if (!this.deleteable) {
            this.fireDataStatusEEvent("AccessNotDeleteable", "", true);	//	audit
            return false;
        }


        var hasKeyColumn = this.gTable._indexKeyColumn != -1;
        var hasProcessedColumn = this.gTable._indexProcessedColumn > 0 && !this.gTable._tableName.startsWith("I_");
        var hasProcessedRecord = [];

        //PrepareJson Object
        var recIds = [];
        var singleKeyWhere = [];
        var multiKeyWhere = [];
        var AD_Table_ID = this.AD_Table_ID;



        var rowData = null;

        //prepare postive list 
        //collect all records, exclude processed records
        for (var sel = 0; sel < selIndices.length; sel++) {

            rowData = this.mSortList[selIndices[sel]];

            if (hasProcessedColumn) {

                var processed = this.getValueAt(selIndices[sel], "processed");
                if (processed != null && processed) {
                    //fireDataStatusEEvent("CannotDeleteTrx", "", true);
                    hasProcessedRecord.push(rowData.recid);
                    continue;
                }
            }

            recIds.push(rowData.recid);
            if (hasKeyColumn) {
                singleKeyWhere.push(rowData[this.keyColumnName.toLowerCase()]);
            }
            else {
                multiKeyWhere.push(this.getWhereClause(rowData));
            }
        } //end

        var out = null;
        if (recIds.length > 0) //has records to delete
        {
            var inn = {
                RecIds: recIds,
                HasKeyColumn: hasKeyColumn,
                TableName: this.gTable._tableName,
                AD_Table_ID: AD_Table_ID
            }
            if (hasKeyColumn)
                inn.SingleKeyWhere = singleKeyWhere;
            else
                inn.MultiKeyWhere = multiKeyWhere;
            //call Delete
            out = VIS.dataContext.deleteWRecords(inn);
            out = JSON.parse(out);
        }
        else if (hasProcessedRecord.length > 0) //Single Record
        {
            this.fireDataStatusEEvent("CannotDeleteTrx", "", true);
            return false;
        }

        if (out) {



            var selIndexOrIds = null;
            if (out.UnDeletedRecIds && out.UnDeletedRecIds.length > 0) {
                selIndexOrId = out.UnDeletedRecIds;
            }
            else if (hasProcessedRecord.length > 0) {
                selIndexOrId = hasProcessedRecord;
            }
            else {
                selIndexOrId = currentRow;
            }

            if (out.DeletedRecIds && out.DeletedRecIds.length > 0) {
                this.rowCount -= out.DeletedRecIds.length;
                this.fireTableModelChanged(VIS.VTable.prototype.ROW_DELETE, out.DeletedRecIds, selIndexOrId);

                if (out.RecIds && out.RecIds.length > 0)
                    this.fireRowChanged(false, out.RecIds);

            }

            if (out.ErrorMsg) {
                this.log.log(Level.SEVERE, out.ErrorMsg);
            }
            if (out.InfoMsg) {
                this.log.log(Level.Info, out.InfoMsg);
            }

            if (out.IsError) {

                if (out.FireEEvent)
                    this.fireDataStatusEEvent(out.EventParam.Msg, out.EventParam.Info, out.EventParam.IsError);
                return false;
            }
            else {

                this.changed = false;
                this.rowChanged = -1;
                this.fireDataStatusIEvent("Deleted", "");
            }
        }
        return true;
    };

    GridTable.prototype.dataDeleteAsync = function (selIndices, currentRow) {
        var localthis = this;
        return new Promise(function (resolve, reject) {
            if (!selIndices || selIndices.length < 0) {
                resolve(false);
                return;
            }

            //	Tab R/O
            if (localthis.readOnly) {
                localthis.fireDataStatusEEvent("AccessCannotDelete", "", true);	//	privileges
                resolve(false);
                return;
            }

            //	Is this record deletable?
            if (!localthis.deleteable) {
                localthis.fireDataStatusEEvent("AccessNotDeleteable", "", true);	//	audit
                resolve(false);
                return;
            }


            var hasKeyColumn = localthis.gTable._indexKeyColumn != -1;
            var hasProcessedColumn = localthis.gTable._indexProcessedColumn > 0 && !localthis.gTable._tableName.startsWith("I_");
            var hasProcessedRecord = [];

            //PrepareJson Object
            var recIds = [];
            var singleKeyWhere = [];
            var multiKeyWhere = [];
            var AD_Table_ID = localthis.AD_Table_ID;



            var rowData = null;

            //prepare postive list 
            //collect all records, exclude processed records
            for (var sel = 0; sel < selIndices.length; sel++) {

                rowData = localthis.mSortList[selIndices[sel]];

                if (hasProcessedColumn) {

                    var processed = localthis.getValueAt(selIndices[sel], "processed");
                    if (processed != null && processed) {
                        //fireDataStatusEEvent("CannotDeleteTrx", "", true);
                        hasProcessedRecord.push(rowData.recid);
                        continue;
                    }
                }

                recIds.push(rowData.recid);
                if (hasKeyColumn) {
                    singleKeyWhere.push(rowData[localthis.keyColumnName.toLowerCase()]);
                }
                else {
                    multiKeyWhere.push(localthis.getWhereClause(rowData));
                }
            } //end

            var out = null;
            if (recIds.length > 0) //has records to delete
            {
                var inn = {
                    RecIds: recIds,
                    HasKeyColumn: hasKeyColumn,
                    TableName: localthis.gTable._tableName,
                    AD_Table_ID: AD_Table_ID
                }
                if (hasKeyColumn)
                    inn.SingleKeyWhere = singleKeyWhere;
                else
                    inn.MultiKeyWhere = multiKeyWhere;

                //var that = this;

                //call Delete
                out = VIS.dataContext.deleteWRecords(inn).then(function (out) {
                    out = JSON.parse(out);
                    if (out) {

                        var selIndexOrIds = null;
                        if (out.UnDeletedRecIds && out.UnDeletedRecIds.length > 0) {
                            selIndexOrId = out.UnDeletedRecIds;
                        }
                        else if (hasProcessedRecord.length > 0) {
                            selIndexOrId = hasProcessedRecord;
                        }
                        else {
                            selIndexOrId = currentRow;
                        }

                        if (out.DeletedRecIds && out.DeletedRecIds.length > 0) {
                            localthis.rowCount -= out.DeletedRecIds.length;
                            localthis.fireTableModelChanged(VIS.VTable.prototype.ROW_DELETE, out.DeletedRecIds, selIndexOrId);

                            if (out.RecIds && out.RecIds.length > 0)
                                localthis.fireRowChanged(false, out.RecIds);

                        }

                        if (out.ErrorMsg) {
                            localthis.log.log(Level.SEVERE, out.ErrorMsg);
                        }
                        if (out.InfoMsg) {
                            localthis.log.log(Level.Info, out.InfoMsg);
                        }

                        if (out.IsError) {

                            if (out.FireEEvent)
                                localthis.fireDataStatusEEvent(out.EventParam.Msg, out.EventParam.Info, out.EventParam.IsError);
                            resolve(false);
                            return;
                        }
                        else {

                            localthis.changed = false;
                            localthis.rowChanged = -1;
                            localthis.fireDataStatusIEvent("Deleted", "");
                        }
                        resolve(true);
                        return;
                    }
                });

            }
            else if (hasProcessedRecord.length > 0) //Single Record
            {
                localthis.fireDataStatusEEvent("CannotDeleteTrx", "", true);
                resolve(false);
                return;
            }
        });
    };

    GridTable.prototype.readData = function (dr) {
        var rowDB = {};
        var size = this.gridFields.length;
        var columnName = null;
        //	Types see also MField.createDefault
        try {
            //	get row data
            for (var j = 0; j < size; j++) {
                columnName = this.gridFields[j].getColumnName().toLowerCase();
                rowDB[columnName] = this.readDataOfColumn(columnName, dr.getCell(columnName));
                columnName = null;
            }
            return rowDB;
        }
        catch (e) {
            throw e;
        }

    };

    GridTable.prototype.readDataOfObject = function (obj) {
        var rowDB = {};
        var size = this.gridFields.length;
        var columnName = null;
        var imgColList = [];
        //	Types see also MField.createDefault
        try {
            //	get row data
            for (var j = 0; j < size; j++) {
                columnName = this.gridFields[j].getColumnName().toLowerCase();
                rowDB[columnName] = this.readDataOfColumn(columnName, obj[columnName]);
                if (this.gridFields[j].getDisplayType() == VIS.DisplayType.Image) {
                    imgColList.push("imgurlcolumn" + columnName)
                }
                columnName = null;
            }
            if (imgColList && imgColList.length > 0) {
                for (k = 0; k < imgColList.length; k++) {
                    rowDB[imgColList[k]] = this.readDataOfColumn(imgColList[k], obj[imgColList[k]]);
                }
            }

            return rowDB;
        }
        catch (e) {
            throw e;
        }

    };


    GridTable.prototype.close = function (finalCall) {
        if (!this.isOpen)
            return;
        //  remove listeners
        if (finalCall) {

        }
        if (!this.inserting) {
            //DataSave(false);	//	not manual
        }
        if (this.bufferList != null) {
            this.bufferList.length = 0;
            this.bufferList = null;
            this.bufferList = [];
            this.mSortList = null;
        }
        if (this.mSortList != null) {
            this.mSortList = null;
        }
        if (finalCall) {
            this.dispose();
        }
        this.isOpen = false;
    };	//	close

    GridTable.prototype.dataIgnore = function (skipNotify) {

        if (!this.inserting && !this.changed && this.rowChanged < 0) {
            return;
        }

        if (this.inserting) {

            //	Get Sort
            // MSort sort = m_sort.get(m_newRow);
            //int bufferRow = sort.index;
            //this.mSortList.splice(this.newRow, 1);

            //	Delete row in Buffer and shifts all below up
            //m_buffer.remove(bufferRow);
            this.rowCount--;
            this.pazeSize--;
            //	Delete row in Sort
            //m_sort.remove(m_newRow);	//	pintint to the last column, so no adjustment
            //
            this.changed = false;
            //this.changed = false;
            this.rowChanged = -1;
            this.inserting = false;
            //	inform
            // var isLastRecord = this.newRow >= this.rowCount && this.rowCount > 0;
            this.fireTableModelChanged(VIS.VTable.prototype.ROW_UNDO, this.mSortList[this.newRow].recid, this.newRow);
        }
        else {
            if (this.rowData != null) {
                var row = this.getRow(this.rowChanged);
                $.extend(row, this.rowData);
            }
        }

        this.changed = false;
        this.rowChanged = -1;
        this.rowData = null;
        this.inserting = false;
        this.newRow = -1;
        //return true;
        if (!skipNotify) {
            this.fireDataStatusIEvent("Ignored", "");
        }
        return true;
    };

    GridTable.prototype.createDSE = function () {

        var changed = this.changed;
        if (this.rowChanged != -1)
            changed = true;
        var dse = new DataStatusEvent(null, this.getRowCount(), changed,
            //m_ctx.isAutoCommit(m_WindowNo), m_inserting);
            false, this.inserting);

        dse.setPageInfo(this.currentPage, this.rowCount, this.pazeSize);
        dse.AD_Table_ID = this.gTable._AD_Table_ID;
        dse.Record_ID = null;
        return dse;

    };

    GridTable.prototype.fireDataStatusChanged = function (e) {
        if (this.mDataListener) {
            this.mDataListener.dataStatusChanged(e);
        }
        e = null;
    };

    GridTable.prototype.fireTableModelChanged = function (type, record, index, fireSelect) {
        if (this.mCardModelListener) {
            this.mCardModelListener.tableModelChanged(type, record, index, fireSelect);
        }
        if (this.mTableModelListener) {
            this.mTableModelListener.tableModelChanged(type, record, index, fireSelect);
        }

    };

    GridTable.prototype.fireRowChanged = function (save, keyID) {
        if (this.mRowChangedListener) {
            this.mRowChangedListener.rowChanged(save, keyID);
        }
    };

    GridTable.prototype.fireDataStatusIEvent = function (AD_Message, info) {
        var e = this.createDSE();
        e.setInfo(AD_Message, info, false, false);
        this.fireDataStatusChanged(e);
    };

    GridTable.prototype.fireQueryCompleted = function (args) {
        // this.fireDataStatusIEvent(this.dseEvent);
        if (this.mQueryCompletedListener)
            this.mQueryCompletedListener.queryCompleted(args);
        args = null;
    };

    //AD_Message, info, isError
    //errorLog
    GridTable.prototype.fireDataStatusEEvent = function (AD_Message, info, isError, isWarn) {

        if (arguments.length === 1) {
            this.fireDataStatusEEvent(arguments[0].value, arguments[0].name, true, false);
        }
        else {
            var e = this.createDSE();
            if (!isWarn)
                isWarn = !isError;
            else {
                isError = !isWarn;
            }

            e.setInfo(AD_Message, info, isError, isWarn);
            //if (isError)
            //    log.saveWarning(AD_Message, info);
            this.fireDataStatusChanged(e);
        }
    };

    //GridTable.prototype.fireDataStatusEEvent = function (errorLog) {
    //    if (errorLog != null)
    //        this.fireDataStatusEEvent(errorLog.value, errorLog.name, true);
    //};

    GridTable.prototype.addDataStatusListener = function (listner) {
        this.mDataListener = listner
    };

    GridTable.prototype.removeDataStatusListener = function (listner) {
        this.mDataListener = null;
    };

    GridTable.prototype.addQueryCompleteListner = function (listner) {
        this.mQueryCompletedListener = listner
    };

    GridTable.prototype.removeQueryCompleteListner = function (listner) {
        this.mQueryCompletedListener = null;
    };

    GridTable.prototype.addTableModelListener = function (listner) {
        this.mTableModelListener = listner
    };

    GridTable.prototype.removeTableModelListener = function (listner) {
        this.mTableModelListener = null;
        listner = null;
    };

    GridTable.prototype.addCardModelListener = function (listner) {
        this.mCardModelListener = listner
    };

    GridTable.prototype.removeCardModelListener = function (listner) {
        this.mCardModelListener = null;
        listner = null;
    };

    GridTable.prototype.addRowChangedListener = function (listner) {
        this.mRowChangedListener = listner
    };

    GridTable.prototype.removeRowChangedListener = function (listner) {
        this.mRowChangedListener = null;
        listner = null;
    };

    GridTable.prototype.updateTableModel = function (field) {

        var columnName = field.getColumnName();
        var displayType = field.getDisplayType();
        //	Integer, ID, Lookup (UpdatedBy is a numeric column)
        if (displayType == VIS.DisplayType.Integer
            || (VIS.DisplayType.IsID(displayType) // JJ: don't touch!
                && (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") ||
                    columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")))
            || columnName.endsWith("atedBy")) {
            //dc = new DataColumn(columnName, typeof(Int32));
            this.defaultModel[columnName.toLowerCase()] = new Number();
        }

        //	Number
        else if (VIS.DisplayType.IsNumeric(displayType)) {
            //dc = new DataColumn(columnName, typeof(Decimal));			//	BigDecimal
            this.defaultModel[columnName.toLowerCase()] = new Number();
        }
        //	Date
        else if (VIS.DisplayType.IsDate(displayType)) {
            //dc = new DataColumn(columnName, typeof(DateTime));	//	Timestamp
            this.defaultModel[columnName.toLowerCase()] = new Date();
        }
        //	RowID or Key (and Selection)
        else if (displayType == VIS.DisplayType.RowID) {
            dc = new DataColumn(columnName, typeof (Object));
            this.defaultModel[columnName.toLowerCase()] = new Object();
        }
        //	YesNo
        else if (displayType == VIS.DisplayType.YesNo) {
            //if (field.isEncryptedColumn())
            //	str = (String)decrypt(str);
            //dc = new DataColumn(columnName, typeof(String));	//	Boolean
            this.defaultModel[columnName.toLowerCase()] = new Boolean();
        }
        //	LOB
        else if (VIS.DisplayType.IsLOB(displayType)) {
            //if (DisplayType.Binary == displayType)
            // {
            //     dc = new DataColumn(columnName, typeof(Byte[]));
            // }
            // else
            // {
            // dc = new DataColumn(columnName, typeof (String));
            this.defaultModel[columnName.toLowerCase()] = new String();
            // }
        }
        // For EnterpriseDB (Vienna Type Long Text is stored as Text in EDB)
        //	String
        else {
            //dc = new DataColumn(columnName, typeof(String));
            this.defaultModel[columnName.toLowerCase()] = new String();
        }

        //defaultModel.Columns.Add(dc);

    };

    GridTable.prototype.dispose = function () {

        originalLength = this.gridFields.length;
        var gField;
        for (var i = originalLength; i > 0; i--) {
            gField = this.gridFields.pop();
            gField.dispose();
            gField = null;
        }
        this.gridFields.length = 0;
        this.gTable = null;
        this.gridFields = null;
        this.gFieldLessData = null;
        this.gFieldData = null;
        this.gFieldUnique = null;
    };

    GridTable.prototype.maintainVersionFieldChanged = function (rowData, oldRowData) {
        // return false if new Record is inserted
        // do not ask for date if new Record
        // if (oldRowData["updatedby"]) {
        if (this.gridFields && this.gridFields.length > 0) {
            for (var i = 0; i < this.gridFields.length; i++) {
                if (this.gridFields[i].vo.IsMaintainVersions) {
                    var colName = this.gridFields[i].vo.ColumnName.toLowerCase();
                    if (rowData[colName] != oldRowData[colName])
                        return true;
                }
            }
        }
        // }
        return false;
    };

    //********************* END *****************//



    //****************************************************//
    //**             GRID FIELD                        **//
    //**************************************************//
    /**
    *  Grid Field Model.
    * 
    *  - Fields are a combination of AD_Field (the display attributes)
    *    and AD_Column (the storage attributes).
    * 
    *  - The Field maintains the current edited value. If the value is changed,
    *  - it fire PropertyChange "FieldValue".
    *  <br>
    *  Usually editors listen to their fields.
    *
    */
    function GridField(gField) {
        this.gfield = gField;
        this.vo = gField._vo;
        this.vo["orginalDispaly"] = gField._vo.displayType;
        this.oldValue;
        this.value;
        this.inserting;
        this.error;
        this.valueNoFire = true;

        this.gridTab = null;

        var m_lookup = null;
        /* Load Lookup */
        if (this.vo.IsDisplayedf || this.vo.ColumnName.toLower().equals("createdby") || gField._vo.ColumnName.toLower().equals("updatedby")
            || this.vo.IsHeaderPanelitem) {
            if (gField._vo.lookupInfo != null && VIS.DisplayType.IsLookup(gField._vo.displayType)) {
                if (VIS.DisplayType.IsLookup(gField._vo.displayType)) {

                    m_lookup = new VIS.MLookup(gField._vo.lookupInfo, gField._lookup);
                    if (this.vo.tabNo > 0)//&& !_vo._isThroughRoleCenter)  // defer load lookup data of tab ' control except first tab
                    {
                        m_lookup.initialize(true);
                        VIS.MLookupCache.addWindowLookup(this.vo.windowNo, m_lookup);
                    }
                    else {
                        m_lookup.initialize();
                    }
                }
            }
            else if (gField._vo.displayType == VIS.DisplayType.Location) {
                m_lookup = new VIS.MLocationLookup(VIS.context, gField._vo.windowNo);
            }
            else if (gField._vo.displayType == VIS.DisplayType.Locator) {
                m_lookup = new VIS.MLocatorLookup(VIS.context, gField._vo.windowNo);
                //this.vo.DefaultValue = m_lookup.getDefault();
            }
            else if (gField._vo.displayType == VIS.DisplayType.Account) {
                m_lookup = new VIS.MAccountLookup(VIS.context, gField._vo.windowNo);
                m_lookup.setTabNo(this.vo.tabNo);
            }
            else if (gField._vo.displayType == VIS.DisplayType.PAttribute) {
                m_lookup = new VIS.MPAttributeLookup(VIS.context, gField._vo.windowNo);
            }
            else if (gField._vo.displayType == VIS.DisplayType.GAttribute) {
                m_lookup = new VIS.MGAttributeLookup(VIS.context, gField._vo.windowNo);
            }
            else if (gField._vo.displayType == VIS.DisplayType.AmtDimension) {
                m_lookup = new VIS.MAmtDivLookup(VIS.context, gField._vo.windowNo);
            }
            else if (gField._vo.displayType == VIS.DisplayType.ProductContainer) {
                m_lookup = new VIS.MProductContainerLookup(VIS.context, gField._vo.windowNo);
            }
        }

        this.lookup = m_lookup;
        /*value Change Listner */
        this.propertyChangeListner;

        this.log = VIS.Logging.VLogger.getVLogger("VIS.GridField");

        gField = null;
    };

    GridField.prototype.PROPERTY = 'FieldValue';
    /** Indicator for new Value				*/
    GridField.prototype.INSERTING = 'FieldValueInserting';

    GridField.prototype.setGridTab = function (gridTab) {
        this.gridTab = gridTab;
    };

    GridField.prototype.getIsInserting = function () {
        return this.inserting;
    };

    GridField.prototype.getIsDisplayed = function (checkContext) {
        if (arguments.length == 0) {
            return this.vo.IsDisplayedf;
        }
        else {

            if (!this.vo.IsDisplayedf)
                return false;
            //  no restrictions
            if (this.vo.DisplayLogic == null || this.vo.DisplayLogic.equals(""))
                return true;
            ////  ** dynamic content **

            if (checkContext) { //To-Do
                var retValue = false;
                var checkAll = [];
                var logical = [];
                //changes done by Jagmohan Bhatt on 5-May-2010
                //purpose: display logic defined by multiple field action
                //best way: [@fieldName1 = <someValue>] &/| [@fieldName2 = <someValue]
                //currently it will support condition from only two fields.
                var bracket = new VIS.StringTokenizer(this.vo.DisplayLogic, "[]");
                while (bracket.hasMoreTokens()) {
                    var currentToken = bracket.nextToken();
                    if (currentToken.trim() != " & ".trim()) {
                        retValue = VIS.Evaluator.evaluateLogic(this, currentToken);
                        checkAll.push(retValue);
                    }
                    else {
                        logical.push(currentToken);
                    }
                }

                if (checkAll.length > 1) {
                    if (logical[0].trim().equals("&")) {
                        if (checkAll.indexOf(false) > -1) //conatins
                            retValue = false;
                        else
                            retValue = true;
                    }
                    else if (logical[0].trim().equals("|")) {
                        if (checkAll.indexOf(true) > -1) // contains()
                            retValue = true;
                    }
                }
                //    //log.finest(_vo.COLUMNNAME
                //        + " (" + _vo.DisplayLogic + ") => " + retValue);
                return retValue;
            }
            return true;
        }
    };

    GridField.prototype.getIsDisplayedMR = function (checkContext) {

        if (arguments.length == 0) {
            return this.vo.IsDisplayedMR;
        }
        else {

            if (!this.vo.IsDisplayedMR)
                return false;
            //  no restrictions
            if (this.vo.DisplayLogic == null || this.vo.DisplayLogic.equals(""))
                return true;
            ////  ** dynamic content **

            if (checkContext) { //To-Do
                var retValue = false;
                var checkAll = [];
                var logical = [];
                //changes done by Jagmohan Bhatt on 5-May-2010
                //purpose: display logic defined by multiple field action
                //best way: [@fieldName1 = <someValue>] &/| [@fieldName2 = <someValue]
                //currently it will support condition from only two fields.
                var bracket = new VIS.StringTokenizer(this.vo.DisplayLogic, "[]");
                while (bracket.hasMoreTokens()) {
                    var currentToken = bracket.nextToken();
                    if (currentToken.trim() != " & ".trim()) {
                        retValue = VIS.Evaluator.evaluateLogic(this, currentToken);
                        checkAll.push(retValue);
                    }
                    else {
                        logical.push(currentToken);
                    }
                }

                if (checkAll.length > 1) {
                    if (logical[0].trim().equals("&")) {
                        if (checkAll.indexOf(false) > -1) //conatins
                            retValue = false;
                        else
                            retValue = true;
                    }
                    else if (logical[0].trim().equals("|")) {
                        if (checkAll.indexOf(true) > -1) // contains()
                            retValue = true;
                    }
                }
                //    //log.finest(_vo.COLUMNNAME
                //        + " (" + _vo.DisplayLogic + ") => " + retValue);
                return retValue;
            }
            return true;
        }
    };

    GridField.prototype.getColumnName = function () {
        return this.vo.ColumnName;
    };

    GridField.prototype.getIsLongField = function () {
        //	if (m_vo.displayType == DisplayType.String 
        //		|| m_vo.displayType == DisplayType.Text 
        //		|| m_vo.displayType == DisplayType.Memo
        //		|| m_vo.displayType == DisplayType.TextLong
        //		|| m_vo.displayType == DisplayType.Image)
        return (this.vo.DisplayLength >= 30);
    };

    GridField.prototype.getFieldColSpan = function () {
        //	if (m_vo.displayType == DisplayType.String 
        //		|| m_vo.displayType == DisplayType.Text 
        //		|| m_vo.displayType == DisplayType.Memo
        //		|| m_vo.displayType == DisplayType.TextLong
        //		|| m_vo.displayType == DisplayType.Image)
        if (this.vo.DisplayLength <= 9)
            return 1;
        if (this.vo.DisplayLength <= 19)
            return 2;
        if (this.vo.DisplayLength <= 29)
            return 3;
        return 4;
    };

    GridField.prototype.getIsMandatory = function (checkContext) {

        //  Do we have mandatory logic
        if (checkContext && (this.vo.mandatoryLogic != null) && (this.vo.mandatoryLogic.length > 0)) {
            var retValue = VIS.Evaluator.evaluateLogic(this, this.vo.mandatoryLogic);
            //log.Finest(_vo.ColumnName + " MandatoryLogic(" + _vo.mandatoryLogic + ") => " + retValue);
            if (retValue)
                return true;
        }


        //  Not mandatory
        if (!this.vo.IsMandatoryUI || this.getIsVirtualColumn())
            return false;
        //  Numeric Keys and Created/Updated as well as 
        //	DocumentNo/Value/ASI ars not mandatory (persistency layer manages them)
        var _vo = this.vo;
        if ((_vo.IsKey && _vo.ColumnName.endsWith("_ID"))
            || _vo.ColumnName.startsWith("Created") || _vo.ColumnName.startsWith("Updated")
            // || _vo.ColumnName.equals("Value")
            //|| _vo.ColumnName.equals("DocumentNo")
            || _vo.ColumnName.equals("M_AttributeSetInstance_ID"))	//	0 is valid
            return false;

        //  Mandatory if displayed
        return this.getIsDisplayed(checkContext);
    };

    GridField.prototype.getDisplayType = function () {
        return this.vo.displayType;
    };

    GridField.prototype.getOrginalDisplayType = function () {
        return this.vo.orginalDispaly;
    };

    GridField.prototype.getIsVirtualColumn = function () {
        if (this.vo.ColumnSQL && this.vo.ColumnSQL.length > 0)
            return true;
        return false;
    };

    GridField.prototype.getIsReadOnly = function () {
        if (this.getIsVirtualColumn())
            return true;
        return this.vo.IsReadOnly;
    };

    /**
     * set readonly property of field 
     * @param {any} value
     */
    GridField.prototype.setReadOnly = function (value) {
        this.vo.IsReadOnly = value;
    };

    /**
     * check is 
     * @param {any} checkContext
     * @param {any} isMR
     */
    GridField.prototype.getIsEditable = function (checkContext, isMR) {
        //TODO:
        var _vo = this.vo;
        if (this.getIsVirtualColumn())
            return false;
        //  Fields always enabled (are usually not updateable)

        if (_vo.ColumnName.equals("Posted")
            || (_vo.ColumnName.equals("Record_ID") && _vo.displayType == VIS.DisplayType.Button))	//  Zoom
            return true;

        var hasMRDisplayLogic = isMR && !(this.vo.DisplayLogic == null || this.vo.DisplayLogic.equals(""))

        //  Fields always updateable
        if (_vo.IsAlwaysUpdateable && !_vo.tabReadOnly)      //  Zoom single 
        {
            if (!hasMRDisplayLogic)
                return true;
        }


        //  Tab or field is R/O
        if (_vo.tabReadOnly || _vo.IsReadOnly) {
            return false;
        }

        //	Not Updateable - only editable if new updateable row
        if (!_vo.IsUpdateable && !this.inserting) {
            //log.finest(m_vo.ColumnName + " NO - FieldUpdateable=" + m_vo.IsUpdateable);
            return false;
        }

        //	Field is the Link Column of the tab
        if (_vo.ColumnName.equals(VIS.context.getWindowTabContext(_vo.windowNo, _vo.tabNo, "LinkColumnName"))) {
            //log.finest(m_vo.ColumnName + " NO - LinkColumn");
            return false;
        }

        var ctx = VIS.context;
        if (checkContext) {
            var AD_Client_ID = parseInt(ctx.getTabRecordContext(_vo.windowNo, _vo.tabNo, "AD_Client_ID"));
            // If the AD_Org_ID is null then set it to default value (from global Context) as it may cause 
            // the window to be rendered read only.
            if (_vo.ColumnName.equals("AD_Org_ID")) {
                if (ctx.getWindowContext(_vo.windowNo, "AD_Org_ID") == null ||
                    ctx.getWindowContext(_vo.windowNo, "AD_Org_ID").length == 0) {
                    ctx.setWindowContext(_vo.windowNo, "AD_Org_ID", ctx.getContext("#AD_Org_ID"));
                }
            }
            var AD_Org_ID = parseInt(ctx.getTabRecordContext(_vo.windowNo, _vo.tabNo, "AD_Org_ID"));
            var keyColumn = ctx.getWindowTabContext(_vo.windowNo, _vo.tabNo, "KeyColumnName");
            var AD_Window_ID = _vo.AD_Window_ID;

            if ("EntityType".equals(keyColumn))
                keyColumn = "AD_EntityType_ID";
            if (!keyColumn.toUpperCase().endsWith("_ID"))
                keyColumn += "_ID";			//	AD_Language_ID
            var Record_ID = ctx.getWindowTabContext(_vo.windowNo, _vo.tabNo, keyColumn);
            if (Record_ID == "")
                Record_ID = 0;

            var AD_Table_ID = _vo.AD_Table_ID;


            if (!VIS.MRole.canUpdate(
                AD_Client_ID, AD_Org_ID, AD_Table_ID, Record_ID, false)
                || !VIS.MRole.getWindowAccess(AD_Window_ID))
                return false;
            if (!VIS.MRole.getIsColumnAccess(AD_Table_ID, this.vo.AD_Column_ID, false))
                return false;
        }

        //  Do we have a readonly rule
        if (checkContext && (_vo.ReadOnlyLogic.length > 0)) {
            var retValue = !VIS.Evaluator.evaluateLogic(this, _vo.ReadOnlyLogic);
            //log.finest(m_vo.ColumnName + " R/O(" + m_vo.ReadOnlyLogic + ") => R/W-" + retValue);
            if (!retValue)
                return false;
        }

        //  Always editable if Active
        if (_vo.ColumnName.equals("Processing")
            || _vo.ColumnName.equals("DocAction")
            || _vo.ColumnName.equals("GenerateTo"))
            return true;

        //  Record is Processed	***
        if (checkContext
            && (ctx.getWindowContext(_vo.windowNo, _vo.tabNo, "Processed").equals("Y")
                || ctx.getWindowContext(_vo.windowNo, _vo.tabNo, "Processing").equals("Y"))) {
            if (!hasMRDisplayLogic)
                return false;
        }

        //  IsActive field is editable, if record not processed
        if (_vo.ColumnName.equals("IsActive"))
            return true;

        //  Record is not Active
        if (checkContext && !ctx.getWindowContext(_vo.windowNo, _vo.tabNo, "IsActive").equals("Y"))
            return false;

        if (!isMR)
            return this.getIsDisplayed(checkContext);
        return this.getIsDisplayedMR(checkContext);

    };

    GridField.prototype.getWindowNo = function () {
        return this.vo.windowNo;
    };

    GridField.prototype.getIsHeading = function () {
        return this.vo.IsHeading;
    };

    GridField.prototype.getIsSameLine = function () {
        return this.vo.IsSameLine;
    };

    GridField.prototype.getHeader = function () {
        return this.vo.Header;
    };

    GridField.prototype.getDescription = function () {
        return this.vo.Description;
    };

    GridField.prototype.getHelp = function () {
        return this.vo.Help;
    };

    GridField.prototype.getAD_Process_ID = function () {
        return this.vo.AD_Process_ID;
    };

    GridField.prototype.getAD_Form_ID = function () {
        return this.vo.AD_Form_ID;
    };

    GridField.prototype.getAD_Column_ID = function () {
        return this.vo.AD_Column_ID;
    };

    GridField.prototype.getAD_Field_ID = function () {
        return this.vo.AD_Field_ID;
    };

    GridField.prototype.getFieldGroup = function () {
        return this.vo.FieldGroup;
    };

    GridField.prototype.getIsFieldOnly = function () {
        return this.vo.IsFieldOnly;
    };

    GridField.prototype.getShowLabel = function () {
        return true;
    };

    //GridField.prototype.getShowIcon = function () {
    //    return true;
    //};

    GridField.prototype.getIsKey = function () {
        return this.vo.IsKey;
    };

    GridField.prototype.getIsParentColumn = function () {
        return this.vo.IsParent;
    };

    GridField.prototype.getSortNo = function () {
        return this.vo.SortNo;
    };

    GridField.prototype.getMRSeqNo = function () {
        return this.vo.mrSeqNo;
    };

    GridField.prototype.getIsSelectionColumn = function () {
        return this.vo.IsSelectionColumn;
    };

    GridField.prototype.getSelectionSeqNo = function () {
        return this.vo.SelectionSeqNo;
    };

    GridField.prototype.getIsIncludedColumn = function () {
        return this.vo.IsIncludedColumn;
    };

    GridField.prototype.getDependentOn = function () {
        var list = [];
        //	Implicit Dependencies
        var colName = this.getColumnName();
        if (colName.equals("M_AttributeSetInstance_ID"))
            list.push("M_Product_ID");

        else if (colName.equals("M_Locator_ID") || colName.equals("M_LocatorTo_ID")) {
            list.push("M_Product_ID");
            list.push("M_Warehouse_ID");
        }
        //  Display dependent
        VIS.Evaluator.parseDepends(list, this.vo.DisplayLogic);
        VIS.Evaluator.parseDepends(list, this.vo.ReadOnlyLogic);
        //  Lookup
        if (this.lookup != null) {
            VIS.Evaluator.parseDepends(list, this.lookup.getValidation());// _lookup.getv .getValidation());
        }

        //if (list.length > 0 && Logging.VLogMgt.IsLevelFiner())//  CLogMgt.isLevelFiner())
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < list.Count; i++)
        //    sb.Append(list[i]).Append(" - ");
        //    log.Finer("(" + _vo.ColumnName + ") " + sb.ToString());
        //}
        return list;
    };

    GridField.prototype.getAD_Window_ID = function () {
        return this.vo.AD_Window_ID;
    };

    GridField.prototype.getAD_InfoWindow_ID = function () {
        return this.vo.AD_InfoWindow_ID;
    };

    GridField.prototype.getValueAsString = function (variableName) {
        var value = VIS.context.getWindowContext(this.vo.windowNo, this.vo.tabNo, variableName, true);
        if (!value) {
            return '';
        }
        return value.toString();
    };

    GridField.prototype.getColumnSQL = function (withAS) {

        if (arguments.length == 0) {
            return this.vo.ColumnName;
        }
        else {
            if (this.vo.ColumnSQL != null && this.vo.ColumnSQL.length > 0) {
                if (withAS) {
                    if (this.vo.displayType == VIS.DisplayType.YesNo) {
                        //return " (case " + _vo.ColumnSQL + " when 'Y' then 'True' else 'False' end) " + " AS " + _vo.ColumnName;
                    }
                    return this.vo.ColumnSQL + " AS " + this.vo.ColumnName;
                }
                else {
                    if (this.vo.displayType == VIS.DisplayType.YesNo) {
                        //return " (case " + _vo.ColumnSQL + " when 'Y' then 'True' else 'False' end) ";
                    }
                    return this.vo.ColumnSQL;
                }
            }
            if (this.vo.displayType == VIS.DisplayType.YesNo) {
                //return " (case " + _vo.ColumnName + " when 'Y' then 'True' else 'False' end) AS " + _vo.ColumnName;
            }
            //if (this.getObscureType().length > 0) {

            //}
            return this.vo.ColumnName;
        }
    };

    GridField.prototype.getIsLink = function () {
        return this.vo.isLink;
    }

    GridField.prototype.getIsRightPaneLink = function () {
        return this.vo.isRightPaneLink;
    }

    GridField.prototype.getColumnWidth = function () {
        return this.vo.ColumnWidth;
    }

    GridField.prototype.getIsAlwaysUpdateable = function () {
        if (this.getIsVirtualColumn() || !this.vo.IsUpdateable)
            return false;
        return this.vo.IsAlwaysUpdateable;
    };

    /**
     * 	Is Encrypted Field (display)
     *	@return encrypted field
     */
    GridField.prototype.getIsEncryptedField = function () {
        return this.vo.IsEncryptedField;
    };

    GridField.prototype.getIsEncrypted = function () {
        if (this.vo.IsEncryptedField)
            return true;
        var ob = this.getObscureType();
        if (ob != null && ob.length > 0)
            return true;
        return this.vo.ColumnName.equals("Password");
    };

    GridField.prototype.getObscureType = function () {
        return this.vo.ObscureType;
    };

    GridField.prototype.getDisplayLength = function () {
        return this.vo.DisplayLength;
    };

    /**
     * Get Field Length
     * @return fieldLength
     */
    GridField.prototype.getFieldLength = function () {
        return this.vo.FieldLength;
    };

    /**
    * Get Field Length
    * @return fieldLength
    */
    GridField.prototype.getMinValue = function () {
        return this.vo.ValueMin;
    };

    /**
    * Get Field Length
    * @return fieldLength
    */
    GridField.prototype.getMaxValue = function () {
        return this.vo.ValueMax;
    };
    /**
     * Get VFormat
     * @return vformat string
     */
    GridField.prototype.getVFormat = function () {
        return this.vo.VFormat;
    };

    /**
     * get format error message
     * */
    GridField.prototype.getVFormatError = function () {
        return this.vo.VFormatError;
    };

    GridField.prototype.getLookup = function () {
        return this.lookup;
    };

    GridField.prototype.getIsError = function () {
        return this.error;
    };

    GridField.prototype.getIsCopy = function () {
        return this.vo.IsCopy;
    };

    GridField.prototype.getCallout = function () {
        return this.vo.Callout;
    };

    GridField.prototype.getValue = function () {
        return this.value;
    };

    GridField.prototype.getIsIdentifier = function () {
        return this.vo.IsIdentifier;
    };

    /**
     *  Get old/previous Value.
     * 	Called from MTab.processCallout
     *  @return old value
     */
    GridField.prototype.getOldValue = function () {
        return this.oldValue;
    };   //

    GridField.prototype.getIsRange = function () {
        return this.vo.isRange;
    };

    GridField.prototype.getIsRange = function () {
        return this.vo.isRange;
    };

    GridField.prototype.getIsParentValue = function () {
        var ctx = VIS.context;
        if (this.parentValue != null)
            return this.parentValue;
        if (!VIS.DisplayType.IsID(this.vo.displayType) || (this.vo.tabNo == 0))
            this.parentValue = false;
        else {
            var LinkColumnName = ctx.getWindowTabContext(this.vo.windowNo, this.vo.tabNo, "LinkColumnName");
            if (LinkColumnName.length == 0)
                this.parentValue = false;
            else
                this.parentValue = this.vo.ColumnName.equals(LinkColumnName);
            if (this.parentValue)
                this.log.config(this.parentValue
                    + " - Link(" + LinkColumnName + ", W=" + this.vo.windowNo + ",T=" + this.vo.tabNo
                    + ") = " + this.vo.ColumnName);
        }
        return this.parentValue;
    };	//	isParentValue

    GridField.prototype.getDefault = function (ctx, windowNo) {
        /**************************************************************************
     *	Create default value.
     *  <pre>
     *		(a) Key/Parent/IsActive/SystemAccess
     *      (b) SQL Default
     *		(c) Column Default		//	system integrity
     *      (d) User Preference
     *		(e) System Preference
     *		(f) DataType Defaults
     *
     *  Don't default from Context => use explicit defaultValue
     *  (would otherwise copy previous record)
     *  </pre>
     *  @return default value or null
     */

        /**
         *  (a) Key/Parent/IsActive/SystemAccess
         */
        var vo = this.vo;
        //	No defaults for these fields
        if (vo.IsKey || (vo.displayType == VIS.DisplayType.RowID)
            || VIS.DisplayType.IsLOB(vo.displayType))
            return null;
        //	Set Parent to context if not explicitly set
        if (this.getIsParentValue()
            && VIS.Utility.Util.isEmpty(vo.DefaultValue)) {
            //fix parent value
            var parent = ctx.getWindowContext(vo.windowNo, vo.ColumnName);
            if (this.gridTab) {
                parent = ctx.getWindowContext(vo.windowNo, this.gridTab.getParentTabNo(), vo.ColumnName);
            }

            this.log.fine("[Parent] " + vo.ColumnName + "=" + parent);
            return this.createDefault(vo.ColumnName, parent);
        }
        //	Always Active
        if (vo.ColumnName.equals("IsActive")) {
            this.log.fine("[IsActive] " + vo.ColumnName + "=Y");
            return true;
        }

        //	Set Client & Org to System, if System access
        if (VIS.Consts.ACCESSLEVEL_SystemOnly.equals(ctx.getWindowTabContext(vo.windowNo, vo.tabNo, "AccessLevel"))
            && (vo.ColumnName.equals("AD_Client_ID") || vo.ColumnName.equals("AD_Org_ID"))) {
            this.log.fine("[SystemAccess] " + vo.ColumnName + "=0");
            return 0;
        }
        //	Set Org to System, if Client access
        else if (VIS.Consts.ACCESSLEVEL_SystemPlusClient.equals(ctx.getWindowTabContext(vo.windowNo, vo.tabNo, "AccessLevel"))
            && vo.ColumnName.equals("AD_Org_ID")) {
            this.log.fine("[ClientAccess] " + vo.ColumnName + "=0");
            return 0;
        }

        /**
         *  (b) SQL Statement (for data integity & consistency)
         */
        var defStr = "";
        if (vo.DefaultValue.startsWith("@SQL=")) {
            var sql0 = vo.DefaultValue.substring(5);			//	w/o tag
            var sql = VIS.Env.parseContext(ctx, vo.windowNo, sql0, false, true);	//	replace variables
            var sqlTest = sql.toUpperCase();
            if ((sqlTest.indexOf("DELETE ") != -1) && (sqlTest.indexOf("UPDATE ") != -1) && (sqlTest.indexOf("DROP ") != -1))
                sql = "";	//	Potential security issue
            if (sql.equals("")) {
                this.log.log(Level.WARNING, "(" + vo.ColumnName + ") - Default SQL variable parse failed: "
                    + sql0);
            }
            else {
                var dr = null;
                try {
                    //dr = executeReader(sql);
                    dr = executeDReader(sql);

                    if (dr.read())
                        defStr = dr.getString(0);
                    else
                        this.log.log(Level.WARNING, "(" + vo.ColumnName + ") - no Result: " + sql);
                    dr.close();
                }
                catch (e) {
                    if (sql.endsWith("="))	//	Variable Resolved empty
                        this.log.log(Level.SEVERE, "(" + vo.ColumnName + ") " + sql0, e);
                    else
                        this.log.log(Level.WARNING, "(" + vo.ColumnName + ") " + sql, e);
                }
                finally {
                    //		DB.closeResultSet(rs);
                    //    DB.closeStatement(stmt);
                    if (dr != null) {
                        dr.dispose();
                    }
                }

            }
            if ((defStr != null) && (defStr.length > 0)) {
                this.log.fine("[SQL] " + vo.ColumnName + "=" + defStr);
                return this.createDefault("", defStr);
            }
        }	//	SQL Statement


        /**
         * 	(c) Field DefaultValue		=== similar code in AStartRPDialog.getDefault ===
         */
        if (!vo.DefaultValue.equals("") && !vo.DefaultValue.startsWith("@SQL=")) {
            defStr = "";		//	problem is with texts like 'sss;sss'
            //	It is one or more variables/constants
            var st = new VIS.StringTokenizer(vo.DefaultValue, ",;", false);
            while (st.hasMoreTokens()) {
                var variable = st.nextToken().trim();
                if (variable.equals("@SysDate@") || variable.equals("@Now@"))	//	System Time
                {
                    var d = new Date();
                    d.setMilliseconds(0);
                    d.setSeconds(0);
                    var n = d.toISOString();
                    //console.log(vo.ColumnName + " <==>" + variable +"==>"+ n);
                    return n;
                }
                else if (variable.indexOf('@') != -1)			//	it is a variable
                    defStr = ctx.getWindowContext(vo.windowNo, variable.replaceAll('@', ' ').trim());
                else if (variable.startsWith("'") && variable.endsWith("'"))	//	it is a 'String'
                {
                    if (variable.length - 2 > 0)
                        defStr = variable.substring(1, variable.length - 1);
                    else
                        defStr = variable.replaceAll("'", "");
                }
                else
                    defStr = variable;

                if (defStr.length > 0) {
                    this.log.fine("[DefaultValue] " + vo.ColumnName + "=" + defStr);
                    return this.createDefault(variable, defStr);
                }
            }	//	while more Tokens
        }	//	Default value


        //	No default for Dependent fields of IDs (if defined - assumed to be correct)
        if ((this.lookup != null) && !VIS.Utility.Util.isEmpty(this.lookup.getValidation())) {

            var code = this.lookup.getValidation();
            var vars = VIS.Evaluator.getVariables(code);
            var setNull = false;
            for (var j = 0; j < vars.length; j++)//  (String var : vars)
            {
                var token = vars[j];
                if (!token.startsWith("#")	//	Global variables OK
                    && token.toUpperCase().endsWith("_ID")
                    && !token.equals(vo.ColumnName)) {	//	assumes that parent value is already defined in ctx
                    var ctxValue = ctx.getWindowContext(windowNo, token);
                    setNull = VIS.Utility.Util.isEmpty(ctxValue);
                    if (setNull)
                        break;
                }
            }
            //	if (vars.size() > 0)
            //		log.warning(getColumnName() + ": " + setNull + " - " + vars
            //			+ " - " + code);
            if (setNull) {
                //if (CLogMgt.isLevelFiner())
                //  log.fine("[Dependent] " + m_vo.ColumnName + "=NULL - " + code);
                //else
                //  log.fine("[Dependent] " + m_vo.ColumnName + "=NULL");
                this.lookup.clear();
                return null;
            }
        }	//	dependent

        /**
         *	(d) Preference (user) - P|
         */
        defStr = VIS.Env.getPreference(ctx, vo.AD_Window_ID, vo.ColumnName, false);
        if (defStr !== "") {
            this.log.fine("[UserPreference] " + vo.ColumnName + "=" + defStr);
            return this.createDefault("", defStr);
        }

        /**
         *	(e) Preference (System) - # $
         */
        defStr = VIS.Env.getPreference(ctx, vo.AD_Window_ID, vo.ColumnName, true);
        if (defStr !== "") {
            this.log.fine("[SystemPreference] " + vo.ColumnName + "=" + defStr);
            return this.createDefault("", defStr);
        }

        /**
         *	(f) DataType defaults
         */

        //	Button to N
        if ((vo.displayType == VIS.DisplayType.Button) && !vo.ColumnName.toUpperCase().endsWith("_ID")) {
            this.log.fine("[Button=N] " + vo.ColumnName);
            return "N";
        }
        //	CheckBoxes default to No
        if (this.vo.displayType == VIS.DisplayType.YesNo) {
            this.log.fine("[YesNo=N] " + vo.ColumnName);
            return false;
        }
        //  lookups with one value
        //	if (DisplayType.isLookup(m_vo.displayType) && m_lookup.getSize() == 1)
        //	{
        //		/** @todo default if only one lookup value */
        //	}
        //  IDs remain null
        if (vo.ColumnName.toUpperCase().endsWith("_ID")) {
            this.log.fine("[ID=null] " + vo.ColumnName);
            return null;
        }
        //  actual Numbers default to zero
        if (VIS.DisplayType.IsNumeric(vo.displayType)) {
            this.log.fine("[Number=0] " + vo.ColumnName);
            return this.createDefault("", "0");
        }

        /**
         *  No resolution
         */
        this.log.fine("[NONE] " + vo.ColumnName);
        return null;
    };	//	getDefault

    GridField.prototype.getDefault2 = function (ctx, windowNo) {
        /**************************************************************************
     *	Create default value.
     *  <pre>
     *		(a) Key/Parent/IsActive/SystemAccess
     *      (b) SQL Default
     *		(c) Column Default		//	system integrity
     *      (d) User Preference
     *		(e) System Preference
     *		(f) DataType Defaults
     *
     *  Don't default from Context => use explicit defaultValue2
     *  (would otherwise copy previous record)
     *  </pre>
     *  @return default value or null
     */

        /**
         *  (a) Key/Parent/IsActive/SystemAccess
         */
        var vo = this.vo;
        //	No defaults for these fields
        if (vo.IsKey || (vo.displayType == VIS.DisplayType.RowID)
            || VIS.DisplayType.IsLOB(vo.displayType))
            return null;
        //	Set Parent to context if not explicitly set
        if (this.getIsParentValue()
            && VIS.Utility.Util.isEmpty(vo.DefaultValue2)) {
            var parent = ctx.getWindowContext(vo.windowNo, vo.ColumnName);
            this.log.fine("[Parent] " + vo.ColumnName + "=" + parent);
            return this.createDefault(vo.ColumnName, parent);
        }
        //	Always Active
        if (vo.ColumnName.equals("IsActive")) {
            this.log.fine("[IsActive] " + vo.ColumnName + "=Y");
            return true;
        }

        //	Set Client & Org to System, if System access
        if (VIS.Consts.ACCESSLEVEL_SystemOnly.equals(ctx.getWindowTabContext(vo.windowNo, vo.tabNo, "AccessLevel"))
            && (vo.ColumnName.equals("AD_Client_ID") || vo.ColumnName.equals("AD_Org_ID"))) {
            this.log.fine("[SystemAccess] " + vo.ColumnName + "=0");
            return 0;
        }
        //	Set Org to System, if Client access
        else if (VIS.Consts.ACCESSLEVEL_SystemPlusClient.equals(ctx.getWindowTabContext(vo.windowNo, vo.tabNo, "AccessLevel"))
            && vo.ColumnName.equals("AD_Org_ID")) {
            this.log.fine("[ClientAccess] " + vo.ColumnName + "=0");
            return 0;
        }

        /**
         *  (b) SQL Statement (for data integity & consistency)
         */
        var defStr = "";
        if (vo.DefaultValue2.startsWith("@SQL=")) {
            var sql0 = vo.DefaultValue2.substring(5);			//	w/o tag
            var sql = VIS.Env.parseContext(ctx, vo.windowNo, sql0, false, true);	//	replace variables
            var sqlTest = sql.toUpperCase();
            if ((sqlTest.indexOf("DELETE ") != -1) && (sqlTest.indexOf("UPDATE ") != -1) && (sqlTest.indexOf("DROP ") != -1))
                sql = "";	//	Potential security issue
            if (sql.equals("")) {
                this.log.log(Level.WARNING, "(" + vo.ColumnName + ") - Default SQL variable parse failed: "
                    + sql0);
            }
            else {
                var dr = null;
                try {
                    // dr = executeReader(sql);
                    dr = executeDReader(sql);
                    if (dr.read())
                        defStr = dr.getString(0);
                    else
                        this.log.log(Level.WARNING, "(" + vo.ColumnName + ") - no Result: " + sql);
                    dr.close();
                }
                catch (e) {
                    if (sql.endsWith("="))	//	Variable Resolved empty
                        this.log.log(Level.SEVERE, "(" + vo.ColumnName + ") " + sql0, e);
                    else
                        this.log.log(Level.WARNING, "(" + vo.ColumnName + ") " + sql, e);
                }
                finally {
                    //		DB.closeResultSet(rs);
                    //    DB.closeStatement(stmt);
                    if (dr != null) {
                        dr.dispose();
                    }
                }

            }
            if ((defStr != null) && (defStr.length > 0)) {
                this.log.fine("[SQL] " + vo.ColumnName + "=" + defStr);
                return this.createDefault("", defStr);
            }
        }	//	SQL Statement


        /**
         * 	(c) Field DefaultValue		=== similar code in AStartRPDialog.getDefault ===
         */
        if (!vo.DefaultValue2.equals("") && !vo.DefaultValue2.startsWith("@SQL=")) {
            defStr = "";		//	problem is with texts like 'sss;sss'
            //	It is one or more variables/constants
            var st = new VIS.StringTokenizer(vo.DefaultValue2, ",;", false);
            while (st.hasMoreTokens()) {
                var variable = st.nextToken().trim();
                if (variable.equals("@SysDate@") || variable.equals("@Now@"))	//	System Time
                {
                    var d = new Date();
                    var n = d.toISOString();
                    //console.log(vo.ColumnName + " <==>" + variable +"==>"+ n);
                    return n;
                }
                else if (variable.indexOf('@') != -1)			//	it is a variable
                    defStr = ctx.getWindowContext(vo.windowNo, variable.replaceAll('@', ' ').trim());
                else if (variable.startsWith("'") && variable.endsWith("'"))	//	it is a 'String'
                {
                    if (variable.length - 2 > 0)
                        defStr = variable.substring(1, variable.length - 1);
                    else
                        defStr = variable.replaceAll("'", "");
                }
                else
                    defStr = variable;

                if (defStr.length > 0) {
                    this.log.fine("[DefaultValue2] " + vo.ColumnName + "=" + defStr);
                    return this.createDefault(variable, defStr);
                }
            }	//	while more Tokens
        }	//	Default value


        //	No default for Dependent fields of IDs (if defined - assumed to be correct)
        if ((this.lookup != null) && !VIS.Utility.Util.isEmpty(this.lookup.getValidation())) {

            var code = this.lookup.getValidation();
            var vars = VIS.Evaluator.getVariables(code);
            var setNull = false;
            for (var j = 0; j < vars.length; j++)//  (String var : vars)
            {
                var token = vars[j];
                if (!token.startsWith("#")	//	Global variables OK
                    && token.toUpperCase().endsWith("_ID")
                    && !token.equals(vo.ColumnName)) {	//	assumes that parent value is already defined in ctx
                    var ctxValue = ctx.getWindowContext(windowNo, token);
                    setNull = VIS.Utility.Util.isEmpty(ctxValue);
                    if (setNull)
                        break;
                }
            }
            //	if (vars.size() > 0)
            //		log.warning(getColumnName() + ": " + setNull + " - " + vars
            //			+ " - " + code);
            if (setNull) {
                //if (CLogMgt.isLevelFiner())
                //  log.fine("[Dependent] " + m_vo.ColumnName + "=NULL - " + code);
                //else
                //  log.fine("[Dependent] " + m_vo.ColumnName + "=NULL");
                this.lookup.clear();
                return null;
            }
        }	//	dependent

        /**
         *	(d) Preference (user) - P|
         */
        defStr = VIS.Env.getPreference(ctx, vo.AD_Window_ID, vo.ColumnName, false);
        if (defStr !== "") {
            this.log.fine("[UserPreference] " + vo.ColumnName + "=" + defStr);
            return this.createDefault("", defStr);
        }

        /**
         *	(e) Preference (System) - # $
         */
        defStr = VIS.Env.getPreference(ctx, vo.AD_Window_ID, vo.ColumnName, true);
        if (defStr !== "") {
            this.log.fine("[SystemPreference] " + vo.ColumnName + "=" + defStr);
            return this.createDefault("", defStr);
        }

        /**
         *	(f) DataType defaults
         */

        //	Button to N
        if ((vo.displayType == VIS.DisplayType.Button) && !vo.ColumnName.toUpperCase().endsWith("_ID")) {
            this.log.fine("[Button=N] " + vo.ColumnName);
            return "N";
        }
        //	CheckBoxes default to No
        if (this.vo.displayType == VIS.DisplayType.YesNo) {
            this.log.fine("[YesNo=N] " + vo.ColumnName);
            return false;
        }
        //  lookups with one value
        //	if (DisplayType.isLookup(m_vo.displayType) && m_lookup.getSize() == 1)
        //	{
        //		/** @todo default if only one lookup value */
        //	}
        //  IDs remain null
        if (vo.ColumnName.toUpperCase().endsWith("_ID")) {
            this.log.fine("[ID=null] " + vo.ColumnName);
            return null;
        }
        //  actual Numbers default to zero
        if (VIS.DisplayType.IsNumeric(vo.displayType)) {
            this.log.fine("[Number=0] " + vo.ColumnName);
            return this.createDefault("", "0");
        }

        /**
         *  No resolution
         */
        this.log.fine("[NONE] " + vo.ColumnName);
        return null;
    };	//	getDefault

    GridField.prototype.getIsEncryptedColumn = function () {
        return this.vo.IsEncryptedColumn;
    };

    GridField.prototype.getZoomWindow_ID = function () {
        return this.vo.ZoomWindow_ID;
    };

    GridField.prototype.getIsDefaultFocus = function () {
        return this.vo.IsDefaultFocus;
    };

    GridField.prototype.getStyleLogic = function () {
        if (!this.vo.StyleLogic)
            this.vo.StyleLogic = '';
        return this.vo.StyleLogic;
    };

    GridField.prototype.createDefault = function (variable, value) {

        //	true NULL
        if ((value == null) || (value.length == 0))
            return null;
        //	see also MTable.readData
        try {
            //	IDs & Integer & CreatedBy/UpdatedBy
            if (this.vo.ColumnName.endsWith("atedBy")
                || this.vo.ColumnName.toUpperCase().endsWith("_ID")) {
                try	//	defaults -1 => null
                {
                    var ii = parseInt(value);
                    if (ii < 0)
                        return null;
                    return ii;//  Integer.valueOf(ii);
                }
                catch (e) {
                    this.log.warning("Cannot parse: " + value + " - " + e.message);
                }
                return 0;
            }
            //	Integer
            if (this.vo.displayType == VIS.DisplayType.Integer)
                return parseInt(value);

            //	Number
            if (VIS.DisplayType.IsNumeric(this.vo.displayType))
                return parseFloat(value);

            //	Timestamps
            if (VIS.DisplayType.IsDate(this.vo.displayType)) {
                //	Time
                try {
                    var d = null;
                    if (isNaN(Number(value))) {
                        d = new Date(value);
                    }
                    else {
                        d = new Date();//parseInt(value)); Set current date 
                        console.log(this.vo.ColumnName + "==>[1n] " + d);
                        d.setMinutes(-d.getTimezoneOffset() + d.getMinutes());
                        // console.log(this.vo.ColumnName + "==>[2] " + d);
                        console.log(this.vo.ColumnName + "==>[3n] " + d.toISOString());
                    }
                    d.setMilliseconds(0);
                    d.setSeconds(0);
                    return d.toISOString();
                }
                catch (ex) {
                }
                //	Date yyyy-mm-dd hh:mm:ss.fffffffff
                var tsString = value
                    + "2014-01-01T01:00:05Z".substring(value.length);
                try {
                    new Date(tsString).toISOString();
                }
                catch (exx) {
                    this.log.warning("Cannot convert to Timestamp: " + tsString);
                }
                var d = new Date();
                d.setMilliseconds(0);
                d.setSeconds(0);
                return d.toISOString();
            }

            //	Boolean
            if (this.vo.displayType == VIS.DisplayType.YesNo)
                return "Y".equals(value);

            //	Default - String
            if (variable.equals("@#Date@")) {
                try {	//	2007-06-27 00:00:00.0
                    //var date1 = new Date(value);
                    // date1.setHours(0, 0, 0, 0);
                    var date1 = new Date(parseInt(value));
                    // date1.setHours(0, 0, 0, 0);

                    //long time = Long.parseLong (value);
                    //value = new Timestamp(time).toString();
                    //value = value.substring(0, 10);
                    value = date1.toLocaleDateString();
                    //long time = Long.parseLong (value);
                    //value = new Timestamp(time).toString();
                    //value = value.substring(0, 10);
                    //value = date1.toISOString();
                }
                catch (eex) {
                }
            }
            return value;
        }
        catch (error) {
            this.log.log(Level.SEVERE, this.vo.ColumnName + " - " + error.message);
        }
        return null;
    };	//	createDefaul

    GridField.prototype.setValue = function (newValue, inserting) {

        if (!this.inserting)      //  set the old value
            this.oldValue = this.value;
        this.value = newValue;
        this.inserting = inserting;
        this.error = false;        //  reset error

        this.updateContext();


        //  Does not fire, if same value
        var oldValue = this.oldValue;
        if (inserting) {
            oldValue = this.INSERTING;
        }
        //console.log(_vo.ColumnName + " ===> " + newValue)
        if ((oldValue !== newValue || this.forcefirepropchange) && this.propertyChangeListner) {
            //console.log(_vo.ColumnName + " ===> " + newValue);
            this.propertyChangeListner.setValue(newValue, inserting);
        }
        this.forcefirepropchange = this.inserting || this.vo.displayType == VIS.DisplayType.YesNo
    };

    GridField.prototype.setCardViewSeqNo = function (seqNo) {
        this.cardViewSeq = seqNo;
    }

    GridField.prototype.getCardViewSeqNo = function () {
        return this.cardViewSeq;
    }

    GridField.prototype.setCardFieldStyle = function (style) {
        this.cardFieldStyle = style;
    }

    GridField.prototype.getCardFieldStyle = function () {
        return this.cardFieldStyle;
    }

    GridField.prototype.setCardIconHide = function (hide) {
        this.cardHideIcon = hide;
    }

    GridField.prototype.isCardIconHide = function () {
        return this.cardHideIcon;
    }

    GridField.prototype.setCardTextHide = function (hide) {
        this.cardTextHide = hide;
    }

    GridField.prototype.isCardTextHide = function () {
        return this.cardTextHide;
    }



    GridField.prototype.updateContext = function () {

        //	Set Context
        var _vo = this.vo;
        var DisplayType = VIS.DisplayType;
        var ctx = VIS.context;
        var newValue = this.value;

        if (_vo.displayType == DisplayType.Text
            || _vo.displayType == DisplayType.Memo
            || _vo.displayType == DisplayType.TextLong
            || _vo.displayType == DisplayType.Binary
            || _vo.displayType == DisplayType.RowID
            || this.getIsEncrypted()) {

        }
        else if (newValue == 'undefined' || newValue == null) {
            if (!this.getIsParentTabField()) {
                ctx.setWindowContext(_vo.windowNo, _vo.ColumnName, null);
            }
            else if (this.gridTab)
                ctx.setTabRecordContext(_vo.windowNo, _vo.tabNo, _vo.ColumnName,
                    null);
        }
        else if (typeof newValue == typeof Boolean || _vo.displayType == DisplayType.YesNo) {

            var newVal = newValue.toString().toLowerCase() == "true" || newValue.toString() == "Y" ? "Y" : "N";
            if (!this.getIsParentTabField()) {
                ctx.setWindowContext(_vo.windowNo, _vo.ColumnName,
                    newVal);
            }
            else if (this.gridTab)
                ctx.setTabRecordContext(_vo.windowNo, _vo.tabNo, _vo.ColumnName,
                    newVal);
        }
        else if (typeof newValue == typeof date || DisplayType.IsDate(_vo.displayType)) {
            if (!this.getIsParentTabField()) {
                ctx.setWindowContext(_vo.windowNo, _vo.ColumnName, newValue);
            }
            else if (this.gridTab)
                ctx.setTabRecordContext(_vo.windowNo, _vo.tabNo, _vo.ColumnName,
                    newValue);
        }

        else {
            if (!this.getIsParentTabField()) {
                ctx.setWindowContext(_vo.windowNo, _vo.ColumnName, newValue == null ? null : newValue.toString());
            }
            else if (this.gridTab)
                ctx.setTabRecordContext(_vo.windowNo, _vo.tabNo, _vo.ColumnName,
                    newValue.toString());
        }
    };

    /**
     * check field exist in parent tab 
     * @param {any} colName column name 
     */
    GridField.prototype.getIsParentTabField = function (colName) {
        if (!this.gridTab)
            return false;
        if (!colName)
            colName = this.vo.ColumnName;

        var parentTab = this.gridTab.getParentTab();
        if (parentTab == null)
            return false;
        return parentTab.getField(colName) != null;
    };

    GridField.prototype.setNullValue = function () {
        if (this.valueNoFire)      //  set the old value
            this.oldValue = this.value;
        this.value = null;
        this.inserting = false;
        this.error = false;        //  reset error

        if (this.oldValue !== this.value && this.propertyChangeListner) {
            //console.log(_vo.ColumnName + " ===> " + newValue);
            this.propertyChangeListner.setValue(this.value);
        }

    };

    GridField.prototype.setPropertyChangeListener = function (listner) {
        this.propertyChangeListner = listner
    };

    GridField.prototype.setError = function (error) {
        this.error = error;
    };

    GridField.prototype.setInserting = function (inserting) {
        this.inserting = inserting;
    };

    GridField.prototype.getAD_Reference_Value_ID = function () {
        return this.vo.AD_Reference_Value_ID;
    };

    GridField.prototype.setDisplayType = function (displayType) {
        this.vo.displayType = displayType;
    };

    GridField.prototype.setDisplayLength = function (length) {
        this.vo.DisplayLength = length;
    }

    GridField.prototype.setDisplayed = function (displayed) {
        this.vo.IsDisplayedf = displayed;
    };

    GridField.prototype.setValueNoFire = function (value) {
        this.valueNoFire = value;
    };


    GridField.prototype.getLoadRecursiveData = function () {
        return this.vo.LoadRecursiveData;
    };

    GridField.prototype.getShowChildOfSelected = function () {
        return this.vo.ShowChildOfSelected;
    };

    GridField.prototype.getIsBackgroundProcess = function () {
        return this.vo.IsBackgroundProcess;
    };

    GridField.prototype.getAskUserBGProcess = function () {
        return this.vo.AskUserBGProcess;
    };

    /****************************************/

    GridField.prototype.getIsHeaderPanelitem = function () {
        return this.vo.IsHeaderPanelitem;
    };

    GridField.prototype.getHeaderOverrideReference = function () {
        return this.vo.HeaderOverrideReference;
    };

    GridField.prototype.getHeaderStyle = function () {
        return this.vo.HeaderStyle;
    };

    GridField.prototype.getHeaderHeadingOnly = function () {
        return this.vo.HeaderHeadingOnly;
    };

    GridField.prototype.getHeaderSeqno = function () {
        return this.vo.HeaderSeqno;
    };

    GridField.prototype.getHeaderIconOnly = function () {
        return this.vo.HeaderIconOnly;
    };

    GridField.prototype.getHtmlStyle = function () {
        return this.vo.HtmlStyle;
    };

    GridField.prototype.getGridImageStyle = function () {
        return this.vo.GridImageStyle;
    };

    GridField.prototype.getShowIcon = function () {
        return this.vo.ShowIcon;
    };

    GridField.prototype.getAD_Image_ID = function () {
        return this.vo.AD_Image_ID;
    };

    GridField.prototype.getFontClass = function () {
        return this.vo.FontClass;
    };

    GridField.prototype.getPlaceHolder = function () {
        return this.vo.PlaceHolder;
    };

    GridField.prototype.getImageName = function () {
        return this.vo.ImageName;
    };

    GridField.prototype.getCellSpace = function () {
        return this.vo.CellSpace;
    };

    GridField.prototype.getFieldBreadth = function () {
        return this.vo.FieldBreadth;
    };

    GridField.prototype.getIsLineBreak = function () {
        return this.vo.LineBreak;
    };

    GridField.prototype.getIsFieldgroupDefault = function () {
        return this.vo.FieldGroupDefault;
    };
    //
    GridField.prototype.getShowFilterOption = function () {
        return this.vo.ShowFilterOption;
    };

    GridField.prototype.getIsUnique = function () {
        return this.vo.IsUnique;
    };
    GridField.prototype.getIsSwitch = function () {
        return this.vo.IsSwitch;
    };

    /**
     *  Refresh Lookup if the lookup is unstable
     *  @return true if lookup is validated
     */
    GridField.prototype.refreshLookup = function () {
        //  if there is a validation string, the lookup is unstable
        if ((!this.lookup) || (this.lookup.getValidation().length == 0))
            return true;
        //
        this.lookup.refresh();
        return this.lookup.getIsValidated();
    };

    GridField.prototype.dispose = function () {
        if (this.lookup != null) {
            this.lookup.dispose();
            this.lookup = null;
        }
        this.gfield = null;
        this.vo = null;
        this.propertyChangeListner = null;

    };
    //**************** END **********************//
    //DataStatusEvent

    function GridTabPanel(tPanel) {
        this.tPanel = tPanel;
        this.vo = tPanel._vo;
    };

    GridTabPanel.prototype.getName = function () {
        return this.vo.Name;
    }

    GridTabPanel.prototype.getAD_Tab_ID = function () {
        return this.vo.AD_Tab_ID;
    }

    GridTabPanel.prototype.getIconPath = function () {
        return this.vo.IconPath;
    }

    GridTabPanel.prototype.getIsDefault = function () {
        return this.vo.IsDefault;
    }

    GridTabPanel.prototype.getClassName = function () {
        return this.vo.Classname;
    }

    GridTabPanel.prototype.getAD_TabPanel_ID = function () {
        return this.vo.AD_TabPanel_ID;
    }

    GridTabPanel.prototype.getSeqNo = function () {
        return this.vo.SeqNo;
    }

    GridTabPanel.prototype.getExtraInfo = function () {
        return this.vo.ExtraInfo;
    }


    function DataStatusEvent(source1, totalRows, changed, autoSave, inserting) {

        this.mTotalPage = 1;
        this.mCurrentPage = 1;
        this.mPageSize = VIS.Env.getWINDOW_PAGE_SIZE();
        this.totalRecords = 1;

        this.m_totalRows = totalRows;
        this.m_changed = changed;
        this.m_autoSave = autoSave;
        this.m_inserting = inserting;
        //
        this.m_AD_Message
        this.m_info = null;
        this.m_isError = false;
        this.m_isWarning = false;
        this.m_confirmed = false;
        //
        this.m_allLoaded = true;
        this.m_loadedRows = -1;
        this.m_currentRow = -1;
        //
        this.m_changedColumn = -1;
        this.m_columnName = null;

        /** Created 				*/
        this.Created;
        /** Created By				*/
        this.CreatedBy;
        /** Updated					*/
        this.Updated;
        /** Updated By				*/
        this.UpdatedBy;
        /** Info					*/
        this.Info;
        /** Table ID				*/
        this.AD_Table_ID;
        /** Record ID				*/
        this.Record_ID;
        this.source = source1;
    };

    /** 
     *  Get Primar Id of Record
     *  */
    DataStatusEvent.prototype.getRecord_ID = function () {
        return this.Record_ID;
    };

    DataStatusEvent.prototype.setLoading = function (loadedRows) {
        this.m_allLoaded = false;
        this.m_loadedRows = loadedRows;
    };

    DataStatusEvent.prototype.getIsLoading = function () {
        return !this.m_allLoaded;
    };

    DataStatusEvent.prototype.getLoadedRows = function () {
        return this.m_loadedRows;
    };

    DataStatusEvent.prototype.setCurrentRow = function (currentRow) {
        this.m_currentRow = currentRow;
    };

    DataStatusEvent.prototype.setCurrentPage = function (currentPage) {
        this.mCurrentPage = currentPage;
    };

    DataStatusEvent.prototype.getCurrentRow = function () {
        return this.m_currentRow;
    };

    DataStatusEvent.prototype.getTotalRows = function () {
        return this.m_totalRows;
    };

    DataStatusEvent.prototype.getTotalRecords = function () {
        return this.totalRecords;
    };

    DataStatusEvent.prototype.getCurrentPage = function () {
        return this.mCurrentPage;
    };

    DataStatusEvent.prototype.getTotalPage = function () {
        return this.mTotalPage;
    }

    DataStatusEvent.prototype.getPageSize = function () {
        return this.mPageSize;
    }

    DataStatusEvent.prototype.setInfo = function (AD_Message, info, isError, isWarning) {
        this.m_AD_Message = AD_Message;
        this.m_info = info;
        this.m_isError = isError;
        this.m_isWarning = isWarning;
    };

    DataStatusEvent.prototype.setPageInfo = function (currentPage, totalReocrds, pageSize) {
        this.mCurrentPage = currentPage;

        this.mPageSize = pageSize;


        if (totalReocrds < 1 || totalReocrds < pageSize) {
            this.mcurrentPage = 1;
            this.mTotalPage = 1;
        }
        else if (pageSize == 0) {
            this.mTotalPage = 1;
        }
        else {
            this.mTotalPage = Math.ceil(totalReocrds / pageSize);// + ((totalRows % pageSize) != 0 ? 1 : 0);
        }
        this.totalRecords = totalReocrds;
    };

    DataStatusEvent.prototype.setInserting = function (inserting) {
        this.m_inserting = inserting;
    };

    DataStatusEvent.prototype.getIsInserting = function () {
        return this.m_inserting;
    };

    DataStatusEvent.prototype.getAD_Message = function () {
        return this.m_AD_Message;
    };

    DataStatusEvent.prototype.getInfo = function () {
        return this.m_info;
    };

    DataStatusEvent.prototype.getIsError = function () {
        return this.m_isError;
    };

    DataStatusEvent.prototype.getIsWarning = function () {
        return this.m_isWarning;
    };

    DataStatusEvent.prototype.getMessage = function () {
        var retValue = new StringBuilder();
        if (this.m_inserting)
            retValue.append("+");
        retValue.append(this.m_changed ? (this.m_autoSave ? "*" : "?") : " ");
        //  current row
        if (this.m_totalRows == 0)
            retValue.append(this.m_currentRow);
        else
            retValue.append(this.m_currentRow + 1);
        //  of
        retValue.append("/");
        if (this.m_allLoaded)
            retValue.append(this.m_totalRows);
        else
            retValue.append(this.m_loadedRows).append("->").append(this.m_totalRows);
        //
        return retValue.toString();
    };	//	getMessage

    DataStatusEvent.prototype.getIsChanged = function () {
        return this.m_changed;
    };

    DataStatusEvent.prototype.getIsFirstRow = function () {
        if (this.m_totalRows == 0)
            return true;
        return this.m_currentRow == 0;
    };

    DataStatusEvent.prototype.getIsLastRow = function () {
        if (this.m_totalRows == 0)
            return true;
        return this.m_currentRow == this.m_totalRows - 1;
    };

    DataStatusEvent.prototype.getIsFirstPage = function () {
        if (this.mTotalPage < 2)
            return true;
        return this.mCurrentPage < 2;
    };

    DataStatusEvent.prototype.getIsLastPage = function () {
        if (this.mTotalPage < 2)
            return true;
        return this.mCurrentPage >= this.mTotalPage;
    };

    DataStatusEvent.prototype.setChangedColumn = function (col, columnName) {
        this.m_changedColumn = col;
        this.m_columnName = columnName;
    };

    DataStatusEvent.prototype.getChangedColumn = function () {
        return this.m_changedColumn;
    };

    DataStatusEvent.prototype.getColumnName = function () {
        return this.m_columnName;
    };

    DataStatusEvent.prototype.setConfirmed = function (confirmed) {
        this.m_confirmed = confirmed;
    };

    DataStatusEvent.prototype.getIsConfirmed = function () {
        return this.m_confirmed;
    };



    //Global Assignment

    VIS.GridWindow = GridWindow;
    VIS.GridTab = GridTab;
    VIS.GridTable = GridTable;
    VIS.GridField = GridField;


})(VIS, jQuery);