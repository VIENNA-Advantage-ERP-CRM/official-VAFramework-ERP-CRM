; (function (VIS, $) {
    //form declaretion
    function AccountForm(header, account, cAcctSchemaId, tblID_s) {
        this.onClose = null;
        var root = $("<div style='position:relative;'>");
        //var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var $self = this;

        var title = header;
        var mAccount = account;
        var C_AcctSchema_ID = cAcctSchemaId;
        var windowNo = VIS.Env.getWindowNo();
        var _comb = null;
        var f_Description = new VIS.Controls.VLabel();
        //  Editors for Query
        var f_Alias = null, f_Combination = null, f_AD_Org_ID = null, f_Account_ID = null, f_SubAcct_ID = null,
            f_M_Product_ID = null, f_C_BPartner_ID = null, f_C_Campaign_ID = null, f_C_LocFrom_ID = null, f_C_LocTo_ID = null,
            f_C_Project_ID = null, f_C_SalesRegion_ID = null, f_AD_OrgTrx_ID = null, f_C_Activity_ID = null,
            f_User1_ID = null, f_User2_ID = null, f_UserElement1_ID = null, f_UserElement2_ID = null, f_UserElement3_ID = null,
            f_UserElement4_ID = null, f_UserElement5_ID = null, f_UserElement6_ID = null, f_UserElement7_ID = null, f_UserElement8_ID = null, f_UserElement9_ID = null;

        var userEle1 = null;
        var userEle2 = null;
        var userEle3 = null;
        var userEle4 = null;
        var userEle5 = null;
        var userEle6 = null;
        var userEle7 = null;
        var userEle8 = null;
        var userEle9 = null;

        var eleFlag1 = false;
        var eleFlag2 = false;
        var eleFlag3 = false;
        var eleFlag4 = false;
        var eleFlag5 = false;
        var eleFlag6 = false;
        var eleFlag7 = false;
        var eleFlag8 = false;
        var eleFlag9 = false;

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

        var executeScalar = function (sql, params, callback) {
            var async = callback ? true : false;
            var dataIn = { sql: sql, page: 1, pageSize: 0 };
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

        var executeQuery = function (sqls, params, callback) {
            var async = callback ? true : false;
            var ret = null;
            var dataIn = { sql: sqls, param: params };
            $.ajax({
                url: nonQueryUrl + 'yWithCode',
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


        //DataSet String
        function getDataSetJString(data, async, callback) {
            var result = null;
            // data.sql = VIS.secureEngine.encrypt(data.sql);
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

        this.log = VIS.Logging.VLogger.getVLogger("AccountForm");
        this.log.config("C_AcctSchema_ID=" + C_AcctSchema_ID + ", C_ValidCombination_ID=" + mAccount.C_ValidCombination_ID);
        //control on form


        /** ElementType AD_Reference_ID=181 */
        var eLEMENTTYPE_AD_Reference_ID = 181;
        /** Account = AC */
        var eLEMENTTYPE_Account = "AC";
        /** Activity = AY */
        var eLEMENTTYPE_Activity = "AY";
        /** BPartner = BP */
        var eLEMENTTYPE_BPartner = "BP";
        /** Location From = LF */
        var eLEMENTTYPE_LocationFrom = "LF";
        /** Location To = LT */
        var eLEMENTTYPE_LocationTo = "LT";
        /** Campaign = MC */
        var eLEMENTTYPE_Campaign = "MC";
        /** Organization = OO */
        var eLEMENTTYPE_Organization = "OO";
        /** Org Trx = OT */
        var eLEMENTTYPE_OrgTrx = "OT";
        /** Project = PJ */
        var eLEMENTTYPE_Project = "PJ";
        /** Product = PR */
        var eLEMENTTYPE_Product = "PR";
        /** Sub Account = SA */
        var eLEMENTTYPE_SubAccount = "SA";
        /** Sales Region = SR */
        var eLEMENTTYPE_SalesRegion = "SR";
        /** User List 1 = U1 */
        var eLEMENTTYPE_UserList1 = "U1";
        /** User List 2 = U2 */
        var eLEMENTTYPE_UserList2 = "U2";
        /** User Element 1 = X1 */
        var eLEMENTTYPE_UserElement1 = "X1";
        /** User Element 2 = X2 */
        var eLEMENTTYPE_UserElement2 = "X2";
        /** User Element 3 = X3 */
        var eLEMENTTYPE_UserElement3 = "X3";
        /** User Element 4 = X4 */
        var eLEMENTTYPE_UserElement4 = "X4";
        /** User Element 5 = X5 */
        var eLEMENTTYPE_UserElement5 = "X5";
        /** User Element 6 = X6 */
        var eLEMENTTYPE_UserElement6 = "X6";
        /** User Element 7 = X7 */
        var eLEMENTTYPE_UserElement7 = "X7";
        /** User Element 8 = X8 */
        var eLEMENTTYPE_UserElement8 = "X8";
        /** User Element 9 = X9 */
        var eLEMENTTYPE_UserElement9 = "X9";

        var tableSArea = $("<table>");
        tableSArea.css("width", "100%");
        var tr = $("<tr>");

        var Okbtn = null;
        var cancelbtn = null;
        var btnRefresh = null;
        var btnUndo = null;
        var btnSave = null;
        var lblParameter = null;
        var parameterDiv = null;
        var bottumDiv = null;
        var discriptionDiv = null;
        var accDiv = null;
        var acctbl = null;
        var gridController = null;
        var changed = false;
        var C_ValidCombination_ID = null;
        var query = null;
        var _mTab = null;
        var lblbottumMsg = null;
        var lblCount = null;
        var lblbottumMsg2 = null;
        var accountSchemaElements = null;

        this.load = function () {
            root.load(VIS.Application.contextUrl + 'AccountForm/Index/?windowno=' + windowNo, function (event) {
                $self.setBusy(true);
                $self.init(root);
                $self.setBusy(false);
            });
        };

        this.setBusy = function (isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.init = function (root) {
            //Buttons
            Okbtn = root.find("#btnOk_" + windowNo);
            cancelbtn = root.find("#btnCancel_" + windowNo);
            btnRefresh = root.find("#btnRefresh_" + windowNo);
            btnUndo = root.find("#btnUndo_" + windowNo);
            btnSave = root.find("#btnSave_" + windowNo);

            //labels
            lblParameter = root.find("#lblParameter_" + windowNo);
            parameterDiv = root.find("#parameterDiv_" + windowNo);

            bottumDiv = root.find("#bottumDiv_" + windowNo);
            discriptionDiv = root.find("#discriptionDiv_" + windowNo);
            accDiv = root.find("#accDiv_" + windowNo);
            acctbl = root.find("#acctbl_" + windowNo);
            lblbottumMsg = root.find("#lblbottumMsg_" + windowNo);
            lblCount = root.find("#lblCount_" + windowNo);
            lblbottumMsg2 = root.find("#lblbottumMsg2_" + windowNo);

            if (VIS.Application.isRTL) {
                Okbtn.css("margin-right", "-128px");
                cancelbtn.css("margin-right", "55px");
            }

            // bottumDiv.style.display = 'hidden';
            // accDiv.height = "50%;";

            //Calture on label
            Okbtn.val(VIS.Msg.getMsg("OK"));
            cancelbtn.val(VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Cancel")));
            lblParameter.val(VIS.Msg.getMsg("Parameter"));

            loadParameters();

            function loadAcctSchemaRecords(arrOfarr) {
                var length = arrOfarr.length;
                var textToInsert = "";
                for (var a = 0; a < length; a += 1) {
                    textToInsert += "<tr>";
                    for (var i = 0; i < arrOfarr[a].length; i += 1) {
                        var obj = arrOfarr[a][i];
                        if (i == 0) {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {

                                textToInsert += "<td class='VIS_Pref_table-row1' style='border-left: 1px solid #ECECEC;'>" + obj + "</td>";
                            }
                        }
                        else if (i == arrOfarr[a].length - 1) {
                            if (a % 2) {

                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                        else {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                    }
                    textToInsert += " </tr>";
                }
                $error.find('tbody > tr').eq(0).after($.parseHTML(textToInsert));
                arr = null;
                textToInsert = null;
            };

            function loadParameters() {
                VIS.Env.getCtx().setContext(windowNo, "C_AcctSchema_ID", C_AcctSchema_ID);

                $.ajax({
                    url: VIS.Application.contextUrl + "AccountForm/LoadControls",
                    dataType: "json",
                    data: {
                        windowNo: windowNo,
                        C_AcctSchema_ID: C_AcctSchema_ID
                    },
                    success: function (data) {
                        returnValue = data.result;
                        designSchema(returnValue);
                        accountSchemaElements = returnValue;

                    }
                });
            };

            function designSchema(obj) {
                //  Model
                var AD_Window_ID = 153;		//	Mavarain Account Combinations 
                VIS.AEnv.getGridWindow(windowNo, AD_Window_ID, function (json) {
                    if (json.error != null) {
                        VIS.ADialog.error(json.error);    //log error
                        self.dispose();
                        self = null;
                        return;
                    }
                    var jsonData = $.parseJSON(json.result); // widow json
                    VIS.context.setContextOfWindow($.parseJSON(json.wCtx), windowNo);// set window context
                    var GridWindow = new VIS.GridWindow(jsonData);
                    if (GridWindow == null) {
                        return;
                    }
                    _mTab = GridWindow.getTabs()[0];
                    //  ParameterPanel restrictions
                    _mTab.getField("Alias").setDisplayLength(15);
                    _mTab.getField("Combination").setDisplayLength(15);
                    //  Grid restrictions
                    _mTab.getField("AD_Client_ID").setDisplayed(false);
                    _mTab.getField("C_AcctSchema_ID").setDisplayed(false);
                    _mTab.getField("IsActive").setDisplayed(false);
                    _mTab.getField("IsFullyQualified").setDisplayed(false);
                    //  don't show fields not being displayed in this environment

                    for (var i = 0; i < _mTab.getFieldCount() ; i++) {
                        var field = _mTab.getField(i);
                        if (!field.getIsDisplayed(true))      //  check context
                        {
                            field.setDisplayed(false);
                        }

                        //var tdChild = $("<td style='display: none;' class='VIS_Pref_table-row'>");
                        ////add column varo grid
                        //if (field.getIsDisplayed()) {
                        //    tdChild = $("<td style='width: auto' class='VIS_Pref_table-row'>");
                        //}

                        //tdChild.concat(field.getHeader());
                        //tr.concat(tdChild);
                    }

                    var id = windowNo + "_" + C_AcctSchema_ID; //uniqueID
                    gridController = new VIS.GridController(false, false, id);
                    gridController.initGrid(true, windowNo, $self, _mTab);
                    gridController.setVisible(true);
                    // gridController.sizeChanged(530, 400);
                    if (window.innerHeight > 700) {
                        gridController.sizeChanged(window.innerHeight - 230, window.innerWidth);
                    }
                    else {
                        gridController.sizeChanged(window.innerHeight - 215, window.innerWidth);
                    }

                    accDiv.append(gridController.getRoot());
                    gridController.activate();

                    //  GridController
                    if (obj.IsHasAlies) {
                        var alias = _mTab.getField("Alias");
                        f_Alias = VIS.VControlFactory.getControl(_mTab, alias, false);
                        addLine(alias, f_Alias, false);
                    }	//	Alias

                    //	Combination
                    var combination = _mTab.getField("Combination");
                    f_Combination = VIS.VControlFactory.getControl(_mTab, combination, false);
                    addLine(combination, f_Combination, false);

                    //Create Fields in Element Order
                    for (var i = 0; i < obj.Elements.length; i++) {
                        var type = returnValue.Elements[i].Type;
                        var isMandatory = returnValue.Elements[i].IsMandatory;
                        var isHeavyData = returnValue.Elements[i].IsHeavyData;
                        var lblNames = returnValue.Elements[i].Name;
                        if (type.equals(eLEMENTTYPE_Organization)) {
                            var field = _mTab.getField("AD_Org_ID");
                            f_AD_Org_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_AD_Org_ID, isMandatory, lblNames, eLEMENTTYPE_Organization);

                            // On Change of Organization validate Trx Organization based on organization 
                            f_AD_Org_ID.fireValueChanged = locationChangedOrg;
                            function locationChangedOrg() {
                                var org_ID = f_AD_Org_ID.getValue();
                                VIS.Env.getCtx().setContext(windowNo, "AcctOrg_ID", org_ID);
                            };
                        }
                        else if (type.equals(eLEMENTTYPE_Account)) {
                            var field = _mTab.getField("Account_ID");
                            f_Account_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            f_Account_ID.setValue(C_AcctSchema_ID);
                            addLine(field, f_Account_ID, isMandatory, lblNames, eLEMENTTYPE_Account);
                            // f_Account_ID.VetoableChangeListener += new EventHandler(f_Account_ID_VetoableChangeListener);
                        }
                        else if (type.equals(eLEMENTTYPE_SubAccount)) {
                            var field = _mTab.getField("C_SubAcct_ID");
                            f_SubAcct_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_SubAcct_ID, isMandatory, lblNames, eLEMENTTYPE_SubAccount);
                        }
                        else if (type.equals(eLEMENTTYPE_Product)) {
                            var field = _mTab.getField("M_Product_ID");
                            f_M_Product_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            f_M_Product_ID.getBtn();
                            addLine(field, f_M_Product_ID, isMandatory, lblNames, eLEMENTTYPE_Product);
                        }
                        else if (type.equals(eLEMENTTYPE_BPartner)) {
                            var field = _mTab.getField("C_BPartner_ID");
                            f_C_BPartner_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_BPartner_ID, isMandatory, lblNames, eLEMENTTYPE_BPartner);
                        }
                        else if (type.equals(eLEMENTTYPE_Campaign)) {
                            var field = _mTab.getField("C_Campaign_ID");
                            f_C_Campaign_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Campaign_ID, isMandatory, lblNames, eLEMENTTYPE_Campaign);
                        }
                        else if (type.equals(eLEMENTTYPE_LocationFrom)) {
                            var field = _mTab.getField("C_LocFrom_ID");
                            f_C_LocFrom_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_LocFrom_ID, isMandatory, lblNames, eLEMENTTYPE_LocationFrom);
                        }
                        else if (type.equals(eLEMENTTYPE_LocationTo)) {
                            var field = _mTab.getField("C_LocTo_ID");
                            f_C_LocTo_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_LocTo_ID, isMandatory, lblNames, eLEMENTTYPE_LocationTo);
                        }
                        else if (type.equals(eLEMENTTYPE_Project)) {
                            var field = _mTab.getField("C_Project_ID");
                            f_C_Project_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Project_ID, isMandatory, lblNames, eLEMENTTYPE_Project);
                        }
                        else if (type.equals(eLEMENTTYPE_SalesRegion)) {
                            var field = _mTab.getField("C_SalesRegion_ID");
                            f_C_SalesRegion_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_SalesRegion_ID, isMandatory, lblNames, eLEMENTTYPE_SalesRegion);
                        }
                        else if (type.equals(eLEMENTTYPE_OrgTrx)) {
                            var field = _mTab.getField("AD_OrgTrx_ID");
                            f_AD_OrgTrx_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_AD_OrgTrx_ID, isMandatory, lblNames, eLEMENTTYPE_OrgTrx);
                        }
                        else if (type.equals(eLEMENTTYPE_Activity)) {
                            var field = _mTab.getField("C_Activity_ID");
                            f_C_Activity_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Activity_ID, isMandatory, lblNames, eLEMENTTYPE_Activity);
                        }
                            //	User1
                        else if (type.equals(eLEMENTTYPE_UserList1)) {
                            var field = _mTab.getField("User1_ID");
                            f_User1_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_User1_ID, isMandatory, lblNames, eLEMENTTYPE_UserList1);
                        }
                        else if (type.equals(eLEMENTTYPE_UserList2)) {
                            var field = _mTab.getField("User2_ID");
                            f_User2_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_User2_ID, isMandatory, lblNames, eLEMENTTYPE_UserList2);
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement1)) {
                            var field = _mTab.getField("UserElement1_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                // var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));
                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);



                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement1_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement1_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement1);

                                    f_UserElement1_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle1 = f_UserElement1_ID.getValue();
                                        eleFlag1 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement1_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement1_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement1);
                                    eleFlag1 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement2)) {
                            var field = _mTab.getField("UserElement2_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement2_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement2_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement2);

                                    f_UserElement2_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle2 = f_UserElement2_ID.getValue();
                                        eleFlag2 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement2_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement2_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement2);
                                    eleFlag2 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement3)) {
                            var field = _mTab.getField("UserElement3_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement3_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement3_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement3);

                                    f_UserElement3_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle3 = f_UserElement3_ID.getValue();
                                        eleFlag3 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement3_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement3_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement3);
                                    eleFlag3 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement4)) {
                            var field = _mTab.getField("UserElement4_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement4_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement4_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement4);

                                    f_UserElement4_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle4 = f_UserElement4_ID.getValue();
                                        eleFlag4 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement4_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement4_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement4);
                                    eleFlag4 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement5)) {
                            var field = _mTab.getField("UserElement5_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement5_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement5_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement5);

                                    f_UserElement5_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle5 = f_UserElement5_ID.getValue();
                                        eleFlag5 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement5_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement5_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement5);
                                    eleFlag5 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement6)) {
                            var field = _mTab.getField("UserElement6_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement6_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement6_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement6);

                                    f_UserElement6_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle6 = f_UserElement6_ID.getValue();
                                        eleFlag6 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement6_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement6_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement6);
                                    eleFlag6 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement7)) {
                            var field = _mTab.getField("UserElement7_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement7_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement7_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement7);

                                    f_UserElement7_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle7 = f_UserElement7_ID.getValue();
                                        eleFlag7 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement7_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement7_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement7);
                                    eleFlag7 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement8)) {
                            var field = _mTab.getField("UserElement8_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);

                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement8_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement8_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement8);

                                    f_UserElement8_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle8 = f_UserElement8_ID.getValue();
                                        eleFlag8 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement8_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement8_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement8);
                                    eleFlag8 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                        else if (type.equals(eLEMENTTYPE_UserElement9)) {
                            var field = _mTab.getField("UserElement9_ID");
                            if (obj.Elements[i].AD_Column_ID > 0) {
                                //var qry = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = " + obj.Elements[i].AD_Column_ID;
                                //var column = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(qry));

                                var qry = "VIS_125";
                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Column_ID", obj.Elements[i].AD_Column_ID);
                                var column = executeScalar(qry, param);


                                if (isHeavyData) {
                                    var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.Search);
                                    f_UserElement9_ID = new VIS.Controls.VTextBoxButton(column, isMandatory, false, true, VIS.DisplayType.Search, value);
                                    addLine(field, f_UserElement9_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement9);

                                    f_UserElement9_ID.fireValueChanged = locationChangedProduct;
                                    function locationChangedProduct() {
                                        userEle9 = f_UserElement9_ID.getValue();
                                        eleFlag9 = true;
                                    };
                                }
                                else {
                                    var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, obj.Elements[i].AD_Column_ID, VIS.DisplayType.TableDir);
                                    f_UserElement9_ID = new VIS.Controls.VComboBox(column, false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
                                    addLine(field, f_UserElement9_ID, isMandatory, lblNames, eLEMENTTYPE_UserElement9);
                                    eleFlag9 = false;
                                }
                            }
                            else {
                                continue;
                            }
                        }
                    }

                    //Discription in label
                    // tr = $("<tr>");
                    // tableSArea.append(tr);
                    // var child = $("<td>");
                    // tr.append(child);
                    //f_Description.height = 21;
                    // child.append(f_Description.getControl().addClass('VIS_Pref_Label_Font'));


                    //discriptionDiv.append(f_Description.getControl().addClass('VIS_Pref_Label_Font'));
                    lblbottumMsg2.css("display", "none");
                    //f_Description.Visible = false;

                    parameterDiv.html(tableSArea);


                    query = new VIS.Query();
                    query.addRestriction("C_AcctSchema_ID", VIS.Query.prototype.EQUAL, C_AcctSchema_ID);

                    // Manish 17/03/2017, Requested By Mukesh sir
                    //var sqlQry = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + tblID_s;
                    //var wIDName = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sqlQry));

                    var qry = "VIS_126";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@tblID_s", tblID_s);
                    var wIDName = executeScalar(qry, param);

                    if (wIDName == "GL_JournalLine") {
                        query.addRestriction("IsFullyQualified", VIS.Query.prototype.EQUAL, "Y");
                    }
                    // end 17/03/2017, Requested By Mukesh sir


                    if (mAccount.C_ValidCombination_ID == 0)
                        _mTab.setQuery(VIS.Query.prototype.getEqualQuery("1", "2"));
                    else {
                        var _query = new VIS.Query();
                        _query.addRestriction("C_AcctSchema_ID", VIS.Query.prototype.EQUAL, C_AcctSchema_ID);
                        _query.addRestriction("C_ValidCombination_ID", VIS.Query.prototype.EQUAL, mAccount.C_ValidCombination_ID);
                        _mTab.setQuery(_query);
                    }

                    gridController.query(0, 0, false);

                    lblbottumMsg.text(obj.Description);
                    //lblCount.val("?");

                    var getOrgDiv = tableSArea.find('[name="AD_Org_ID"]');
                    window.setTimeout(function () {
                        // if (window.innerHeight <= 700) {
                        accDiv.css("width", "73%");
                        parameterDiv.parent().css({ "width": "27%", "padding": "0 10px 0 0" });

                        var sizeCrtl = $(tableSArea.find("button").parent().find("input")[0]).width();
                        //$(tableSArea.find("select")).width(sizeCrtl + 35);
                        //$(tableSArea.find("input")[0]).width(sizeCrtl + 32);
                        //$(tableSArea.find("input")[1]).width(sizeCrtl + 32);

                        // }
                        //if (window.innerHeight >= 700) {
                        //    tableSArea.find("button").parent().find("input").css({ "width": "90%" });
                        //    parameterDiv.parent().css({ "width": "27%", "padding": "0 10px 0 0" });
                        //}

                        //var sqlQry = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + tblID_s;
                        //var wIDName = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sqlQry));

                        var qry = "VIS_126";
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@tblID_s", tblID_s);
                        var wIDName = executeScalar(qry, param);

                        if (wIDName == "GL_JournalLine") {
                            getOrgDiv.on("click", function () {
                                if (getOrgDiv.find('option[value=0]').length != 0) {
                                    getOrgDiv.find('option[value=0]').remove();
                                }
                            });
                        }
                    }, 200);

                });
            };



            function addLine(field, editor, mandatory, lblName, type) {
                $self.log.fine("Field=" + field);
                //new row
                tr = $("<tr>");
                tableSArea.append(tr);

                var label = VIS.VControlFactory.getLabel(field);
                editor.setReadOnly(false);
                editor.setMandatory(mandatory);
                field.setPropertyChangeListener(editor);
                //new column
                var tdChild1 = $("<td>");
                //	label
                tr.append(tdChild1.css("padding", "4px 0px 2px 0px"));
                tdChild1.append(label.getControl().addClass('VIS_Pref_Label_Font'));

                if (type != undefined && (type == "X1" || type == "X2" || type == "X3" || type == "X4" || type == "X5" || type == "X6" || type == "X7" || type == "X8" || type == "X9")) {
                    tdChild1.find('label').text(lblName);
                }

                //new row
                tr = $("<tr>");
                tableSArea.append(tr);
                var tdChild2 = $("<td>");
                //	Field

                if (editor.getBtnCount() >= 2) {

                    tr.append(tdChild2);
                    var div = $("<Div class='vis-act-frm-parm'>");
                    tdChild2.append(div);
                    if (window.innerHeight > 700) {
                        div.append(editor.getControl().css('width', '85%'));
                        //div.append(editor.getBtn(0).css('width', '10%'));
                        div.append(editor.getBtn(0));
                    }
                    else {
                        div.append(editor.getControl().css('width', '85%'));
                        div.append(editor.getBtn(0).css('width', '30px').css('-webkit-appearance', 'none').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
                    }

                }
                else {
                    tr.append(tdChild2);
                    tdChild2.append(editor.getControl().css("width", "100%"));
                }
            }

            function isNull(value) {
                if (value == null || value == 0) {
                    return true;
                }
                return false;
            }

            function saveSelection() {
                C_ValidCombination_ID = _mTab.getValue("C_ValidCombination_ID");

                if ($self.onClose)
                    $self.onClose(C_ValidCombination_ID);
                root.dialog('close');
            }

            function actionFind(includeAliasCombination) {
                var localquery = null;
                if (query != null) {
                    //localquery = query;//query.DeepCopy();
                    localquery = jQuery.extend(true, {}, query);
                }
                else {
                    localquery = new VIS.Query();
                }

                //	Alias
                if (includeAliasCombination && f_Alias != null && f_Alias.getValue().toString().length > 0) {
                    var value = f_Alias.getValue().toString().toUpperCase();
                    if (!value.endsWith("%")) {
                        value += "%";
                    }
                    localquery.addRestriction("UPPER(Alias)", localquery.LIKE, value);
                }
                //	Combination (mandatory)
                if (includeAliasCombination && f_Combination.getValue().toString().length > 0) {
                    var value = f_Combination.getValue().toString().toUpperCase();
                    if (!value.endsWith("%"))
                        value += "%";
                    localquery.addRestriction("UPPER(Combination)", localquery.LIKE, value);
                }
                //	Org (mandatory)
                if (f_AD_Org_ID != null && f_AD_Org_ID.getValue() != null)// && !isNull(f_AD_Org_ID.getValue()))
                    localquery.addRestriction("AD_Org_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_AD_Org_ID.getValue()));
                //	Account (mandatory)
                if (f_Account_ID != null && !isNull(f_Account_ID.getValue()))
                    localquery.addRestriction("Account_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_Account_ID.getValue()));

                if (f_SubAcct_ID != null && !isNull(f_SubAcct_ID.getValue()))
                    localquery.addRestriction("C_SubAcct_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_SubAcct_ID.getValue()));

                //	Product
                if (f_M_Product_ID != null && !isNull(f_M_Product_ID.getValue()))
                    localquery.addRestriction("M_Product_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_M_Product_ID.getValue()));
                //	BPartner
                if (f_C_BPartner_ID != null && !isNull(f_C_BPartner_ID.getValue()))
                    localquery.addRestriction("C_BPartner_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_BPartner_ID.getValue()));
                //	Campaign
                if (f_C_Campaign_ID != null && !isNull(f_C_Campaign_ID.getValue()))
                    localquery.addRestriction("C_Campaign_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_Campaign_ID.getValue()));
                //	Loc From
                if (f_C_LocFrom_ID != null && !isNull(f_C_LocFrom_ID.getValue()))
                    localquery.addRestriction("C_LocFrom_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_LocFrom_ID.getValue()));
                //	Loc To
                if (f_C_LocTo_ID != null && !isNull(f_C_LocTo_ID.getValue()))
                    localquery.addRestriction("C_LocTo_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_LocTo_ID.getValue()));
                //	Project
                if (f_C_Project_ID != null && !isNull(f_C_Project_ID.getValue()))
                    localquery.addRestriction("C_Project_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_Project_ID.getValue()));
                //	SRegion
                if (f_C_SalesRegion_ID != null && !isNull(f_C_SalesRegion_ID.getValue()))
                    localquery.addRestriction("C_SalesRegion_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_SalesRegion_ID.getValue()));
                //	Org Trx
                if (f_AD_OrgTrx_ID != null && !isNull(f_AD_OrgTrx_ID.getValue()))
                    localquery.addRestriction("AD_OrgTrx_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_AD_OrgTrx_ID.getValue()));
                //	Activity
                if (f_C_Activity_ID != null && !isNull(f_C_Activity_ID.getValue()))
                    localquery.addRestriction("C_Activity_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_C_Activity_ID.getValue()));
                //	User 1
                if (f_User1_ID != null && !isNull(f_User1_ID.getValue()))
                    localquery.addRestriction("User1_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_User1_ID.getValue()));
                //	User 2
                if (f_User2_ID != null && !isNull(f_User2_ID.getValue()))
                    localquery.addRestriction("User2_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_User2_ID.getValue()));
                //	User Element 1
                if (f_UserElement1_ID != null && !isNull(f_UserElement1_ID.getValue()))
                    localquery.addRestriction("UserElement1_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement1_ID.getValue()));
                //	User Element 2
                if (f_UserElement2_ID != null && !isNull(f_UserElement2_ID.getValue()))
                    localquery.addRestriction("UserElement2_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement2_ID.getValue()));
                //	User Element 3
                if (f_UserElement3_ID != null && !isNull(f_UserElement3_ID.getValue()))
                    localquery.addRestriction("UserElement3_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement3_ID.getValue()));
                //	User Element 4
                if (f_UserElement4_ID != null && !isNull(f_UserElement4_ID.getValue()))
                    localquery.addRestriction("UserElement4_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement4_ID.getValue()));
                //	User Element 5
                if (f_UserElement5_ID != null && !isNull(f_UserElement5_ID.getValue()))
                    localquery.addRestriction("UserElement5_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement5_ID.getValue()));
                //	User Element 6
                if (f_UserElement6_ID != null && !isNull(f_UserElement6_ID.getValue()))
                    localquery.addRestriction("UserElement6_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement6_ID.getValue()));
                //	User Element 7
                if (f_UserElement7_ID != null && !isNull(f_UserElement7_ID.getValue()))
                    localquery.addRestriction("UserElement7_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement7_ID.getValue()));
                //	User Element 8
                if (f_UserElement8_ID != null && !isNull(f_UserElement8_ID.getValue()))
                    localquery.addRestriction("UserElement8_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement8_ID.getValue()));
                //	User Element 9
                if (f_UserElement9_ID != null && !isNull(f_UserElement9_ID.getValue()))
                    localquery.addRestriction("UserElement9_ID", localquery.EQUAL, VIS.Utility.Util.getValueOfInt(f_UserElement9_ID.getValue()));
                //	Query
                _mTab.setQuery(localquery);
                gridController.query(0, 0, false);
            }

            function actionIgnore() {
                if (f_Alias != null) {
                    f_Alias.setValue("");
                }
                f_Combination.setValue("");
                f_Description.Content = "";
                f_Description.Visible = false;
                lblbottumMsg2.val("");


                //
                //	Org (mandatory)
                f_AD_Org_ID.setValue(null);
                //	Account (mandatory)
                f_Account_ID.setValue(null);
                if (f_SubAcct_ID != null)
                    f_SubAcct_ID.setValue(null);

                //	Product
                if (f_M_Product_ID != null)
                    f_M_Product_ID.setValue(null);
                //	BPartner
                if (f_C_BPartner_ID != null)
                    f_C_BPartner_ID.setValue(null);
                //	Campaign
                if (f_C_Campaign_ID != null)
                    f_C_Campaign_ID.setValue(null);
                //	Loc From
                if (f_C_LocFrom_ID != null)
                    f_C_LocFrom_ID.setValue(null);
                //	Loc To
                if (f_C_LocTo_ID != null)
                    f_C_LocTo_ID.setValue(null);
                //	Project
                if (f_C_Project_ID != null)
                    f_C_Project_ID.setValue(null);
                //	SRegion
                if (f_C_SalesRegion_ID != null)
                    f_C_SalesRegion_ID.setValue(null);
                //	Org Trx
                if (f_AD_OrgTrx_ID != null)
                    f_AD_OrgTrx_ID.setValue(null);
                //	Activity
                if (f_C_Activity_ID != null)
                    f_C_Activity_ID.setValue(null);
                //	User 1
                if (f_User1_ID != null)
                    f_User1_ID.setValue(null);
                //	User 2
                if (f_User2_ID != null)
                    f_User2_ID.setValue(null);
                //	User Element 1
                if (f_UserElement1_ID != null)
                    f_UserElement1_ID.setValue(null);
                //	User Element 2
                if (f_UserElement2_ID != null)
                    f_UserElement2_ID.setValue(null);
                //	User Element 3
                if (f_UserElement3_ID != null)
                    f_UserElement3_ID.setValue(null);
                //	User Element 4
                if (f_UserElement4_ID != null)
                    f_UserElement4_ID.setValue(null);
                //	User Element 5
                if (f_UserElement5_ID != null)
                    f_UserElement5_ID.setValue(null);
                //	User Element 6
                if (f_UserElement6_ID != null)
                    f_UserElement6_ID.setValue(null);
                //	User Element 7
                if (f_UserElement7_ID != null)
                    f_UserElement7_ID.setValue(null);
                //	User Element 8
                if (f_UserElement8_ID != null)
                    f_UserElement8_ID.setValue(null);
                //	User Element 9
                if (f_UserElement9_ID != null)
                    f_UserElement9_ID.setValue(null);
            }

            function actionSave() {
                var sb = null;
                var sql = "";
                var value = null;

                if (accountSchemaElements.IsHasAlies) {
                    value = f_Alias.getValue();
                    if (value == null && sb != null)
                        sb = sb.concat(VIS.Msg.translate(VIS.Env.getCtx(), "Alias")).concat(", ");
                }

                var aseList = [];
                for (var i = 0; i < accountSchemaElements.Elements.length; i++) {
                    var ase = accountSchemaElements.Elements[i];
                    aseList.push(ase.Name);
                }
                //for (var i = 0; i < accountSchemaElements.Elements.length; i++) {
                //var ase = accountSchemaElements.Elements[i];
                //    var isMandatory = accountSchemaElements.Elements[i].IsMandatory;
                //    var type = accountSchemaElements.Elements[i].Type;
                //    //
                //    if (type.equals(eLEMENTTYPE_Organization)) {
                //        value = f_AD_Org_ID.getValue();
                //        sql = sql.concat("AD_Org_ID");
                //        if (value != null) {
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //        }
                //        //if (isNull(value))
                //        //    sql = sql.concat(" IS NULL AND ");
                //        //else
                //        //    sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_Account)) {
                //        value = f_Account_ID.getValue();
                //        sql = sql.concat("Account_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_SubAccount)) {
                //        value = f_SubAcct_ID.getValue();
                //        sql = sql.concat("C_SubAcct_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_Product)) {
                //        value = f_M_Product_ID.getValue();
                //        sql = sql.concat("M_Product_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_BPartner)) {
                //        value = f_C_BPartner_ID.getValue();
                //        sql = sql.concat("C_BPartner_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_Campaign)) {
                //        value = f_C_Campaign_ID.getValue();
                //        sql = sql.concat("C_Campaign_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_LocationFrom)) {
                //        value = f_C_LocFrom_ID.getValue();
                //        sql = sql.concat("C_LocFrom_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_LocationTo)) {
                //        value = f_C_LocTo_ID.getValue();
                //        sql = sql.concat("C_LocTo_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_Project)) {
                //        value = f_C_Project_ID.getValue();
                //        sql = sql.concat("C_Project_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_SalesRegion)) {
                //        value = f_C_SalesRegion_ID.getValue();
                //        sql = sql.concat("C_SalesRegion_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_OrgTrx)) {
                //        value = f_AD_OrgTrx_ID.getValue();
                //        sql = sql.concat("AD_OrgTrx_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_Activity)) {
                //        value = f_C_Activity_ID.getValue();
                //        sql = sql.concat("C_Activity_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserList1)) {
                //        value = f_User1_ID.getValue();
                //        sql = sql.concat("User1_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserList2)) {
                //        value = f_User2_ID.getValue();
                //        sql = sql.concat("User2_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement1)) {
                //        value = f_UserElement1_ID.getValue();
                //        sql = sql.concat("UserElement1_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement2)) {
                //        value = f_UserElement2_ID.getValue();
                //        sql = sql.concat("UserElement2_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement3)) {
                //        value = f_UserElement3_ID.getValue();
                //        sql = sql.concat("UserElement3_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement4)) {
                //        value = f_UserElement4_ID.getValue();
                //        sql = sql.concat("UserElement4_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement5)) {
                //        value = f_UserElement5_ID.getValue();
                //        sql = sql.concat("UserElement5_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement6)) {
                //        value = f_UserElement6_ID.getValue();
                //        sql = sql.concat("UserElement6_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement7)) {
                //        value = f_UserElement7_ID.getValue();
                //        sql = sql.concat("UserElement7_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement8)) {
                //        value = f_UserElement8_ID.getValue();
                //        sql = sql.concat("UserElement8_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    else if (type.equals(eLEMENTTYPE_UserElement9)) {
                //        value = f_UserElement9_ID.getValue();
                //        sql = sql.concat("UserElement9_ID");
                //        if (isNull(value))
                //            sql = sql.concat(" IS NULL AND ");
                //        else
                //            sql = sql.concat("=").concat(value).concat(" AND ");
                //    }
                //    //  
                //    if (isMandatory && (value == null) && sb != null) {
                //        sb = sb.concat(ase.getName()).concat(", ");
                //    }
                //}

                $.ajax({
                    url: VIS.Application.contextUrl + "Form/GetValidCombination",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    data: {
                        accountSchemaElements: JSON.stringify(accountSchemaElements),
                        Elements: JSON.stringify(accountSchemaElements.Elements),
                        aseList: JSON.stringify(aseList),
                        value: value,
                        strinB: sb
                    },
                    success: function (data) {
                        sql = data;
                    },
                    error: function (e) {
                        console.log(e)
                    }

                });

                if (sb != null) {
                    VIS.ADialog.info("FillMandatory", true, sb.ToString().Substring(0, sb.Length - 2), null);
                    return;
                }
                if (f_AD_Org_ID == null || f_AD_Org_ID.getValue() == null) {// || isNull(f_AD_Org_ID.getValue())) {
                    VIS.ADialog.info("FillMandatory", true, VIS.Msg.getElement(VIS.Env.getCtx(), "AD_Org_ID"), null);
                    return;
                }
                if (f_Account_ID == null || isNull(f_Account_ID.getValue())) {
                    VIS.ADialog.info("FillMandatory", true, VIS.Msg.getElement(VIS.Env.getCtx(), "Account_ID"), null);
                    $self.setBusy(false);
                    return;
                }

                //Check if already exists
                sql = sql.concat("AD_Client_ID=" + VIS.Env.getCtx().getAD_Client_ID() + " AND C_AcctSchema_ID=" + C_AcctSchema_ID);
                $self.log.fine("Check = " + sql.toString());

                //Check Alies Value
                var alies = f_Alias.getValue().toString();
                var IDvalue = 0;
                var Alias = null;
                var f_alies = f_Alias.getValue().toString();
                var dr = null;
                try {
                    dr = executeDReader(sql, null);
                    if (dr.read()) {
                        IDvalue = dr.tables[0].rows[0].cells["c_validcombination_id"];
                        Alias = dr.tables[0].rows[0].cells["alias"];
                    }
                    dr.close();
                    dr = null;
                }
                catch (e) {
                    //$self.log.Log(Level.SEVERE, sql.ToString(), e);
                    IDvalue = 0;
                    if (dr != null) {
                        dr.close();
                        dr = null;
                    }
                }

                $self.log.fine("ID=" + IDvalue + ", Alias=" + Alias);
                var fAlie
                if (Alias == null)
                    Alias = "";
                //	We have an account like this already - check alias
                if (IDvalue != 0 && accountSchemaElements.IsHasAlies && f_alies.equals(Alias)) {

                    var param = [];


                    if (f_alies.toString().length == 0) {
                        // sql = "UPDATE C_ValidCombination SET Alias=NULL WHERE C_ValidCombination_ID=" + IDvalue;
                        sql = "VIS_127";
                        param[0] = new VIS.DB.SqlParam("@IDvalue", IDvalue);
                    }
                    else {
                        sql = "VIS_128";
                        param[0] = new VIS.DB.SqlParam("@f_alies", f_alies);
                        param[1] = new VIS.DB.SqlParam("@IDvalue", IDvalue);
                    }

                    //sql = sql.concat(" WHERE C_ValidCombination_ID=").concat(IDvalue);





                    var i = 0;
                    try {
                        //i = VIS.DB.executeQuery(sql);
                        executeQuery(sql, param);

                    }
                    catch (e) {
                        //$self.log.Log(Level.SEVERE, sql.ToString(), e);
                    }
                }

                if (IDvalue != 0) {
                    loadInfo(IDvalue, C_AcctSchema_ID);
                    return;
                }

                //	load and display

                $self.log.config("New");
                Alias = null;
                if (f_Alias != null)
                    Alias = f_Alias.getValue().toString();
                var C_SubAcct_ID = 0;
                if (f_SubAcct_ID != null && (f_SubAcct_ID.getValue() != null))
                    C_SubAcct_ID = f_SubAcct_ID.getValue();
                var M_Product_ID = 0;
                if (f_M_Product_ID != null && !isNull(f_M_Product_ID.getValue()))
                    M_Product_ID = f_M_Product_ID.getValue();
                var C_BPartner_ID = 0;
                if (f_C_BPartner_ID != null && !isNull(f_C_BPartner_ID.getValue()))
                    C_BPartner_ID = f_C_BPartner_ID.getValue();
                var AD_OrgTrx_ID = 0;
                if (f_AD_OrgTrx_ID != null && !isNull(f_AD_OrgTrx_ID.getValue()))
                    AD_OrgTrx_ID = f_AD_OrgTrx_ID.getValue();
                var C_LocFrom_ID = 0;
                if (f_C_LocFrom_ID != null && !isNull(f_C_LocFrom_ID.getValue()))
                    C_LocFrom_ID = f_C_LocFrom_ID.getValue();
                var C_LocTo_ID = 0;
                if (f_C_LocTo_ID != null && !isNull(f_C_LocTo_ID.getValue()))
                    C_LocTo_ID = f_C_LocTo_ID.getValue();
                var C_SRegion_ID = 0;
                if (f_C_SalesRegion_ID != null && !isNull(f_C_SalesRegion_ID.getValue()))
                    C_SRegion_ID = f_C_SalesRegion_ID.getValue();
                var C_Project_ID = 0;
                if (f_C_Project_ID != null && !isNull(f_C_Project_ID.getValue()))
                    C_Project_ID = f_C_Project_ID.getValue();
                var C_Campaign_ID = 0;
                if (f_C_Campaign_ID != null && !isNull(f_C_Campaign_ID.getValue()))
                    C_Campaign_ID = f_C_Campaign_ID.getValue();
                var C_Activity_ID = 0;
                if (f_C_Activity_ID != null && !isNull(f_C_Activity_ID.getValue()))
                    C_Activity_ID = f_C_Activity_ID.getValue();
                var User1_ID = 0;
                if (f_User1_ID != null && !isNull(f_User1_ID.getValue()))
                    User1_ID = f_User1_ID.getValue();
                var User2_ID = 0;
                if (f_User2_ID != null && !isNull(f_User2_ID.getValue()))
                    User2_ID = f_User2_ID.getValue();
                var UserElement1_ID = 0;
                if (f_UserElement1_ID != null && !isNull(f_UserElement1_ID.getValue()))
                    UserElement1_ID = f_UserElement1_ID.getValue();
                var UserElement2_ID = 0;
                if (f_UserElement2_ID != null && !isNull(f_UserElement2_ID.getValue()))
                    UserElement2_ID = f_UserElement2_ID.getValue();
                var UserElement3_ID = 0;
                if (f_UserElement3_ID != null && !isNull(f_UserElement3_ID.getValue()))
                    UserElement3_ID = f_UserElement3_ID.getValue();
                var UserElement4_ID = 0;
                if (f_UserElement4_ID != null && !isNull(f_UserElement4_ID.getValue()))
                    UserElement4_ID = f_UserElement4_ID.getValue();
                var UserElement5_ID = 0;
                if (f_UserElement5_ID != null && !isNull(f_UserElement5_ID.getValue()))
                    UserElement5_ID = f_UserElement5_ID.getValue();
                var UserElement6_ID = 0;
                if (f_UserElement6_ID != null && !isNull(f_UserElement6_ID.getValue()))
                    UserElement6_ID = f_UserElement6_ID.getValue();
                var UserElement7_ID = 0;
                if (f_UserElement7_ID != null && !isNull(f_UserElement7_ID.getValue()))
                    UserElement7_ID = f_UserElement7_ID.getValue();
                var UserElement8_ID = 0;
                if (f_UserElement8_ID != null && !isNull(f_UserElement8_ID.getValue()))
                    UserElement8_ID = f_UserElement8_ID.getValue();
                var UserElement9_ID = 0;
                if (f_UserElement9_ID != null && !isNull(f_UserElement9_ID.getValue()))
                    UserElement9_ID = f_UserElement9_ID.getValue();

                var AD_Org_ID = f_AD_Org_ID.getValue();
                var AD_Account_ID = f_Account_ID.getValue();


                //Ajex to save Account onto database 

                if (eleFlag1 && userEle1 != null) {
                    UserElement1_ID = userEle1;
                }
                if (eleFlag2 && userEle2 != null) {
                    UserElement2_ID = userEle2;
                }
                if (eleFlag3 && userEle3 != null) {
                    UserElement3_ID = userEle3;
                }
                if (eleFlag4 && userEle4 != null) {
                    UserElement4_ID = userEle4;
                }
                if (eleFlag5 && userEle5 != null) {
                    UserElement5_ID = userEle5;
                }
                if (eleFlag6 && userEle6 != null) {
                    UserElement6_ID = userEle6;
                }
                if (eleFlag7 && userEle7 != null) {
                    UserElement7_ID = userEle7;
                }
                if (eleFlag8 && userEle8 != null) {
                    UserElement8_ID = userEle8;
                }
                if (eleFlag9 && userEle9 != null) {
                    UserElement9_ID = userEle9;
                }

                $.ajax({
                    url: VIS.Application.contextUrl + "AccountForm/Save",
                    dataType: "json",
                    data: {
                        AD_Client_ID: VIS.Env.getCtx().getAD_Client_ID(),
                        AD_Org_ID: AD_Org_ID,
                        C_AcctSchema_ID: C_AcctSchema_ID,
                        AD_Account_ID: AD_Account_ID,
                        C_SubAcct_ID: C_SubAcct_ID,
                        M_Product_ID: M_Product_ID,
                        C_BPartner_ID: C_BPartner_ID,
                        AD_OrgTrx_ID: AD_OrgTrx_ID,
                        C_LocFrom_ID: C_LocFrom_ID,
                        C_LocTo_ID: C_LocTo_ID,
                        C_SRegion_ID: C_SRegion_ID,
                        C_Project_ID: C_Project_ID,
                        C_Campaign_ID: C_Campaign_ID,
                        C_Activity_ID: C_Activity_ID,
                        User1_ID: User1_ID,
                        User2_ID: User2_ID,
                        UserElement1_ID: UserElement1_ID,
                        UserElement2_ID: UserElement2_ID,
                        UserElement3_ID: UserElement3_ID,
                        UserElement4_ID: UserElement4_ID,
                        UserElement5_ID: UserElement5_ID,
                        UserElement6_ID: UserElement6_ID,
                        UserElement7_ID: UserElement7_ID,
                        UserElement8_ID: UserElement8_ID,
                        UserElement9_ID: UserElement9_ID,
                        Alias: Alias
                    },
                    success: function (data) {
                        returnValue = data.result;
                        //load control
                        loadInfo(returnValue.C_ValidCombination_ID, returnValue.C_AcctSchema_ID);
                    }
                });
            };

            function loadInfo(C_ValidCombination_ID, C_AcctSchema_ID) {
                // this.log.fine("C_ValidCombination_ID=" + C_ValidCombination_ID);
                var sql = "VIS_124";
                var dr = null;
                try {
                    // dr = VIS.DB.executeReader(sql, null);

                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@C_ValidCombination_ID", C_ValidCombination_ID);
                    param[1] = new VIS.DB.SqlParam("@C_AcctSchema_ID", C_AcctSchema_ID);

                    dr = executeReader(sql, param);

                    if (dr.read()) {
                        if (f_Alias != null)
                            f_Alias.setValue(dr.getString("Alias"));
                        if (f_Combination != null)
                            f_Combination.setValue(dr.getString("Combination"));
                        if (f_AD_Org_ID != null)
                            f_AD_Org_ID.setValue(dr.getInt("AD_Org_ID"));
                        if (f_Account_ID != null)
                            f_Account_ID.setValue(dr.getInt("Account_ID"));
                        if (f_SubAcct_ID != null)
                            f_SubAcct_ID.setValue(dr.getInt("C_SubAcct_ID"));
                        if (f_M_Product_ID != null) {
                            if (dr.getInt("M_Product_ID") != 0) {
                                f_M_Product_ID.setValue(dr.getInt("M_Product_ID"));
                            }
                        }
                        if (f_C_BPartner_ID != null) {
                            if (dr.getInt("C_BPartner_ID") != 0) {
                                f_C_BPartner_ID.setValue(dr.getInt("C_BPartner_ID"));
                            }
                        }
                        if (f_C_Campaign_ID != null)
                            f_C_Campaign_ID.setValue(dr.getInt("C_Campaign_ID"));
                        if (f_C_LocFrom_ID != null)
                            f_C_LocFrom_ID.setValue(dr.getInt("C_LocFrom_ID"));
                        if (f_C_LocTo_ID != null)
                            f_C_LocTo_ID.setValue(dr.getInt("C_LocTo_ID"));
                        if (f_C_Project_ID != null)
                            f_C_Project_ID.setValue(dr.getInt("C_Project_ID"));
                        if (f_C_SalesRegion_ID != null)
                            f_C_SalesRegion_ID.setValue(dr.getInt("C_SalesRegion_ID"));
                        if (f_AD_OrgTrx_ID != null)
                            f_AD_OrgTrx_ID.setValue(dr.getInt("AD_OrgTrx_ID"));
                        if (f_C_Activity_ID != null)
                            f_C_Activity_ID.setValue(dr.getInt("C_Activity_ID"));
                        if (f_User1_ID != null)
                            f_User1_ID.setValue(dr.getInt("User1_ID"));
                        if (f_User2_ID != null)
                            f_User2_ID.setValue(dr.getInt("User2_ID"));
                        if (f_UserElement1_ID != null) {
                            if (dr.getInt("UserElement1_ID") != 0) {
                                f_UserElement1_ID.setValue(dr.getInt("UserElement1_ID"));
                            }
                        }
                        if (f_UserElement2_ID != null) {
                            if (dr.getInt("UserElement2_ID") != 0) {
                                f_UserElement2_ID.setValue(dr.getInt("UserElement2_ID"));
                            }
                        }
                        if (f_UserElement3_ID != null) {
                            if (dr.getInt("UserElement3_ID") != 0) {
                                f_UserElement3_ID.setValue(dr.getInt("UserElement3_ID"));
                            }
                        }
                        if (f_UserElement4_ID != null) {
                            if (dr.getInt("UserElement4_ID") != 0) {
                                f_UserElement4_ID.setValue(dr.getInt("UserElement4_ID"));
                            }
                        }
                        if (f_UserElement5_ID != null) {
                            if (dr.getInt("UserElement5_ID") != 0) {
                                f_UserElement5_ID.setValue(dr.getInt("UserElement5_ID"));
                            }
                        }
                        if (f_UserElement6_ID != null) {
                            if (dr.getInt("UserElement6_ID") != 0) {
                                f_UserElement6_ID.setValue(dr.getInt("UserElement6_ID"));
                            }
                        }
                        if (f_UserElement7_ID != null) {
                            if (dr.getInt("UserElement7_ID") != 0) {
                                f_UserElement7_ID.setValue(dr.getInt("UserElement7_ID"));
                            }
                        }
                        if (f_UserElement8_ID != null) {
                            if (dr.getInt("UserElement8_ID") != 0) {
                                f_UserElement8_ID.setValue(dr.getInt("UserElement8_ID"));
                            }
                        }
                        if (f_UserElement9_ID != null) {
                            if (dr.getInt("UserElement9_ID") != 0) {
                                f_UserElement9_ID.setValue(dr.getInt("UserElement9_ID"));
                            }
                        }
                        if (lblbottumMsg2 != null)
                            lblbottumMsg2.text(dr.getString("Description"));

                    }
                    dr.close();
                    dr = null;
                }
                catch (e) {
                    if (dr != null) {
                        dr.close();
                        dr = null;
                    }
                }

                actionFind(false);
            }

            //Events
            Okbtn.on("click", function () {
                $self.setBusy(true);
                saveSelection();
                $self.setBusy(false);
            });

            cancelbtn.on("click", function () {
                root.dialog('close');
            });

            btnRefresh.on("click", function () {
                $self.setBusy(true);
                actionFind(true);
                $self.setBusy(false);
            });

            btnUndo.on("click", function () {
                actionIgnore();
            });

            btnSave.on("click", function () {
                $self.setBusy(true);
                actionSave();
                $self.setBusy(false);
            });

        };

        this.showDialog = function () {
            var w = $(window).width() - 50;
            var h = $(window).height() - 60;
            $busyDiv.height(h);
            $busyDiv.width(w);
            root.append($busyDiv);
            root.dialog({
                modal: false,
                resizable: false,
                title: title,
                width: w,
                height: h,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    root.dialog("destroy");
                    root = null;
                    $self = null;
                }
            });
        };

        this.dataStatusChanged = function (e) {
            var info = _mTab.getValue("Description");
            //f_Description.getControl().val(info);

            _comb = _mTab.getValue("COMBINATION");

            if (info != null && info.length > 0) {
                lblbottumMsg2.css("display", "inline-block");
            }
            else {
                lblbottumMsg2.css("display", "none");
            }
            lblbottumMsg2.text(info);
            lblCount.text(e.totalRecords);
        };

        this.disposeComponent = function () {
            if (Okbtn) {
                Okbtn.off("click");
            }
            if (cancelbtn)
                cancelbtn.off("click");
            if (btnRefresh)
                btnRefresh.off("click");
            if (btnUndo)
                btnUndo.off("click");
            if (btnSave) {
                btnSave.off("click");
            }

            title = null;
            mAccount = null;
            C_AcctSchema_ID = null;
            windowNo = null;
            _comb = null;
            f_Description = null;
            f_Alias = null;
            f_Combination = null;
            f_AD_Org_ID = null; f_Account_ID = null; f_SubAcct_ID = null;
            f_M_Product_ID = null; f_C_BPartner_ID = null; f_C_Campaign_ID = null; f_C_LocFrom_ID = null; f_C_LocTo_ID = null;
            f_C_Project_ID = null; f_C_SalesRegion_ID = null; f_AD_OrgTrx_ID = null; f_C_Activity_ID = null;
            f_User1_ID = null; f_User2_ID = null;
            this.log = null;
            eLEMENTTYPE_AD_Reference_ID = null;
            eLEMENTTYPE_Account = null;
            eLEMENTTYPE_Activity = null;
            eLEMENTTYPE_BPartner = null;
            eLEMENTTYPE_LocationFrom = null;
            eLEMENTTYPE_LocationTo = null;
            eLEMENTTYPE_Campaign = null;
            eLEMENTTYPE_Organization = null;
            eLEMENTTYPE_OrgTrx = null;
            eLEMENTTYPE_Project = null;
            eLEMENTTYPE_Product = null;
            eLEMENTTYPE_SubAccount = null;
            eLEMENTTYPE_SalesRegion = null;
            eLEMENTTYPE_UserList1 = null;
            eLEMENTTYPE_UserList2 = null;
            eLEMENTTYPE_UserElement1 = null;
            eLEMENTTYPE_UserElement2 = null;
            eLEMENTTYPE_UserElement3 = null;
            eLEMENTTYPE_UserElement4 = null;
            eLEMENTTYPE_UserElement5 = null;
            eLEMENTTYPE_UserElement6 = null;
            eLEMENTTYPE_UserElement7 = null;
            eLEMENTTYPE_UserElement8 = null;
            eLEMENTTYPE_UserElement9 = null;
            tableSArea = null;
            tr = null;
            Okbtn = null;
            cancelbtn = null;
            btnRefresh = null;
            btnUndo = null;
            btnSave = null;
            lblParameter = null;
            parameterDiv = null;
            bottumDiv = null;
            discriptionDiv = null;
            accDiv = null;
            acctbl = null;
            gridController = null;
            changed = false;
            C_ValidCombination_ID = null;
            query = null;
            _mTab = null;
            lblbottumMsg = null;
            lblCount = null;
            lblbottumMsg2 = null;
            accountSchemaElements = null;
        };
    };

    AccountForm.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.AccountForm = AccountForm;

})(VIS, jQuery);