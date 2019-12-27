/********************************************************
 * Module Name    :     Application
 * Purpose        :     Get And Process ZoomTarget
 * Author         :     Lakhwinder
 * Date           :     3-Aug-2014
  ******************************************************/
; (function (VIS, $) {


    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";

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

    function AZoomAcross(btn, tableName, qry, curWindowID, busy, container, KeyCol, Record_ID) {

        this.init = function () {
            
            //	See What is there
            getZoomTargets();
            // set recordcount of query, so that lookup window does not opens while opening the window
            // query.SetRecordCount(1);
        };

        var list = [];
        var getZoomTargets = function () {
           

            //var sql = "SELECT DISTINCT t.AD_Table_ID, t.TableName "
            //    + "FROM AD_Table t "
            //    + "WHERE EXISTS (SELECT 1 FROM AD_Tab tt "
            //        + "WHERE tt.AD_Table_ID = t.AD_Table_ID AND tt.SeqNo=10) "
            //    + " AND t.AD_Table_ID IN "
            //        + "(SELECT AD_Table_ID FROM AD_Column "
            //        + "WHERE ColumnName='" + tableName + "_ID') "
            //    + "AND TableName NOT LIKE 'I%'"
            //    + "AND TableName NOT LIKE '" + tableName + "' "
            //    + "ORDER BY 1";
            //var dr = executeReader(sql, null);

            var dr = null;
            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Form/GetZoomTargets",
                data: { tableName: tableName },
                success: function (data) {
                    dr = JSON.parse(data);
                },
            });


            // while (dr.read()) {
            if (dr != null && dr.length > 0) {
                for (var a = 0; a < dr.length; a++) {
                // get table name
                    var targetTableName = dr[a];
                // get target table names for above table
                    // var zoomList = getZoomTarget(targetTableName, curWindowID, qry.getWhereClause());

                    $.ajax({
                        type: 'Get',
                        async: false,
                        url: VIS.Application.contextUrl + "Form/GetZoomTarget",
                        data: { targetTableName: targetTableName, curWindow_ID: curWindowID, targetWhereClause: qry.getWhereClause() },
                        success: function (data) {
                            zoomList = JSON.parse(data);
                        },
                    });


                if (zoomList != null) {
                    for (var i in zoomList) {
                        var pp = zoomList[i];
                        var pushData = true;
                        for (var itm in list) {
                            if (list[itm].Key == zoomList[i].Key) {
                              
                                pushData = false;
                                break;
                            }
                        }
                        if (pushData) {
                            list.push(pp);
                            //var windowName = pp.toString();

                        }
                    }
                }
            }
            }
            // close data reader
            dr = null;



            if (list.length == 0) {

                VIS.ADialog.info("NoZoomTarget");
            }
            else {
                var $root = $("<div>");
                var ul = $('<ul class=vis-apanel-rb-ul>');
                $root.append(ul);
                for (var i in list) {    
                    var li = $("<li data-id='" + list[i].Key + "'>");
                    li.append(list[i].Name);
                    li.on('click', function (e) {
                        e.stopImmediatePropagation();
                       
                        var ad_window_Id = $(this).data('id');
                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction(KeyCol, VIS.Query.prototype.EQUAL, Record_ID);
                        VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                        var overlay = $('#w2ui-overlay-main');
                        overlay.hide();
                        overlay = null;
                    });
                    ul.append(li);
                }
                //container.append(ul);
                container.w2overlay($root.clone(true), { css: { height: '200px' } });
                
            }

        };

        
        //var getZoomTarget = function (targetTableName, curWindow_ID, targetWhereClause) {
         
        //    //The Option List					
        //    var zoomList = [];
        //    var columns = [];
        //    var ZoomWindow_ID = 0;
        //    var PO_Window_ID;
        //    var zoom_WindowName = "";
        //    var whereClause = "";
        //    var windowFound = false;
        //    var ctx = VIS.context.ctx;
        //    var windowList = [];
        //    // Find windows where the first tab is based on the table
        //    //var sql = "SELECT DISTINCT w.AD_Window_ID, w.Name, tt.WhereClause, t.TableName, " +
        //    //        "wp.AD_Window_ID, wp.Name, ws.AD_Window_ID, ws.Name "
        //    //    + "FROM AD_Table t "
        //    //    + "INNER JOIN AD_Tab tt ON (tt.AD_Table_ID = t.AD_Table_ID) ";

        //    //var baseLanguage = VIS.Env.isBaseLanguage();// GlobalVariable.IsBaseLanguage();
        //    //if (baseLanguage) {
        //    //    sql += "INNER JOIN AD_Window w ON (tt.AD_Window_ID=w.AD_Window_ID)";
        //    //    sql += " LEFT OUTER JOIN AD_Window ws ON (t.AD_Window_ID=ws.AD_Window_ID)"
        //    //        + " LEFT OUTER JOIN AD_Window wp ON (t.PO_Window_ID=wp.AD_Window_ID)";
        //    //}
        //    //else {
        //    //    sql += "INNER JOIN AD_Window_Trl w ON (tt.AD_Window_ID=w.AD_Window_ID AND w.AD_Language='" + VIS.Env.getAD_Language() + "')";
        //    //    sql += " LEFT OUTER JOIN AD_Window_Trl ws ON (t.AD_Window_ID=ws.AD_Window_ID AND ws.AD_Language='" + VIS.Env.getAD_Language() + "')"
        //    //        + " LEFT OUTER JOIN AD_Window_Trl wp ON (t.PO_Window_ID=wp.AD_Window_ID AND wp.AD_Language='" + VIS.Env.getAD_Language() + "')";
        //    //}
        //    //sql += "WHERE t.TableName ='" + targetTableName
        //    //    + "' AND w.AD_Window_ID <>" + curWindow_ID
        //    //    + " AND tt.SeqNo=10"
        //    //    + " AND (wp.AD_Window_ID IS NOT NULL "
        //    //            + "OR EXISTS (SELECT 1 FROM AD_Tab tt2 WHERE tt2.AD_Window_ID = ws.AD_Window_ID AND tt2.AD_Table_ID=t.AD_Table_ID AND tt2.SeqNo=10))"
        //    //    + " ORDER BY 2";


        //    var ds = null;
        //    $.ajax({
        //        type: 'Get',
        //        async: false,
        //        url: VIS.Application.contextUrl + "Form/GetZoomTarget",
        //        data: { targetTableName: targetTableName, curWindow_ID: curWindow_ID },
        //        success: function (data) {
        //            ds = new VIS.DB.DataSet().toJson(data);
        //        },
        //    });

        //    if (ds != null && ds.getTables()[0].getRows().length > 0) {
        //        for (var j = 0; j < ds.getTables()[0].getRows().length; j++) {

        //            windowFound = true;
        //            ZoomWindow_ID = ds.getTables()[0].getRows()[j].getCell(6);//int.Parse(ds.Tables[0].Rows[i][6].ToString());
        //            zoom_WindowName = ds.getTables()[0].getRows()[j].getCell(7);//ds.Tables[0].Rows[i][7].ToString();
        //            PO_Window_ID = ds.getTables()[0].getRows()[j].getCell(4);//ds.Tables[0].Rows[i][4].ToString();
        //            whereClause = ds.getTables()[0].getRows()[j].getCell(2);//ds.Tables[0].Rows[i][2].ToString();
        //            // Multiple window support only for Order, Invoice, Shipment/Receipt which have PO windows
        //            if (PO_Window_ID == null || PO_Window_ID.Length == 0)
        //                break;

        //            var windowClause = {};
        //            windowClause.AD_Window_ID = ds.getTables()[0].getRows()[j].getCell(0);
        //            windowClause.windowName = ds.getTables()[0].getRows()[j].getCell(1);
        //            windowClause.whereClause = whereClause;

        //            windowList.push(windowClause);
        //        }
        //        ds = null;
        //        var sql1 = "";
        //        if (!windowFound || (windowList.length <= 1 && ZoomWindow_ID == 0)) {
        //            return zoomList;
        //        }
        //        //If there is a single window for the table, no parsing is neccessary
        //        if (windowList.length <= 1) {
        //            //Check if record exists in target table
        //            sql1 = "SELECT count(*) FROM " + targetTableName + " WHERE " + targetWhereClause;
        //            if (whereClause != null && whereClause.Length != 0) {
        //                sql1 += " AND " + VIS.Evaluator.replaceVariables(whereClause, VIS.Env.getCtx(), null);
        //            }
        //        }
        //        else if (windowList.length > 1) {
        //            // Get the columns used in the whereClause and stores in an arraylist
        //            for (var i in windowList) {
        //                parseColumns(columns, windowList[i].whereClause);
        //            }
        //            // Get the distinct values of the columns from the table if record exists
        //            sql1 = "SELECT DISTINCT ";
        //            for (var i in columns) {
        //                if (i != 0)
        //                    sql1 += ",";
        //                sql1 += columns[i].toString();
        //            }

        //            if (columns.length == 0) {
        //                sql1 += "count(*) ";
        //            }

        //            sql1 += " FROM " + targetTableName + " WHERE " + targetWhereClause;
        //        }


        //        var columnValues = [];
        //        ds = executeDataSet(sql1, null);

        //        if (ds != null && ds.getTables()[0].getRows().length > 0) {
        //            for (var cnt = 0; cnt < ds.getTables()[0].getRows().length; cnt++) {
        //                if (columns.length > 0) {
        //                    columnValues = [];
        //                    // store column names with their values in the variable
        //                    for (var i in columns) {
        //                        var colVal = {}
        //                        colVal.columnName = columns[i].toString();
        //                        colVal.columnValue = ds.getTables()[0].getRows()[cnt].getCell(columns[i].toString());//(String)ds.Tables[0].Rows[cnt][columnName].ToString();
        //                        //log.Fine(columnName + " = " + columnValue);
        //                        //columnValues.Add(new ValueNamePair(columnValue, columnName));
        //                        columnValues.push(colVal);
        //                    }

        //                    // Find matching windows
        //                    for (var i in windowList) {
        //                        //log.Fine("Window : " + windowList[i].windowName + " WhereClause : " + windowList[i].whereClause);
        //                        if (evaluateWhereClause(columnValues, windowList[i].whereClause)) {
        //                            //log.Fine("MatchFound : " + windowList[i].windowName);
        //                            //KeyNamePair pp = new KeyNamePair(windowList[i].AD_Window_ID, windowList[i].windowName);
        //                            var zoomitm = {};
        //                            zoomitm.ID = windowList[i].AD_Window_ID;
        //                            zoomitm.Name = windowList[i].windowName;
        //                            //zoomList.Add(pp);
        //                            zoomList.push(zoomitm);
        //                            // Use first window found. Ideally there should be just one matching
        //                            break;
        //                        }
        //                    }

        //                }
        //                else {
        //                    // get total number of records
        //                    var rowCount = ds.getTables()[0].getRows()[cnt].getCell(0);// int.Parse(ds.Tables[0].Rows[cnt][0].ToString());
        //                    if (rowCount != 0) {
        //                        // make a key name pair
        //                        //KeyNamePair pp = new KeyNamePair(ZoomWindow_ID, zoom_WindowName);
        //                        var zoomitm = {};
        //                        zoomitm.ID = ZoomWindow_ID;
        //                        zoomitm.Name = zoom_WindowName;
        //                        zoomList.push(zoomitm);
        //                    }
        //                }
        //            }
        //        }//end for


        //    }//end if


        //    return zoomList;
           
        //};



        var parseColumns = function (list, parseString) {

            if (parseString == null || parseString.Length == 0) {
                return;
            }

            //	//log.fine(parseString);
            var s = parseString;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.indexOf(" EXISTS ") > 0 || s.indexOf(" IN ") > 0 || s.indexOf("(") > 0 || s.indexOf(")") > 0) {
                return;
            }
            //  while we have columns
            while (s.indexOf("=") != -1) {
                var endIndex = s.indexOf("=");
                var beginIndex = s.lastIndexOf(' ', endIndex);
                var len = endIndex - (beginIndex + 1);

                //String variable = s.Substring(beginIndex + 1, endIndex);
                var variable = s.substr(beginIndex + 1, len);

                if (variable.indexOf(".") != -1) {
                    beginIndex = variable.indexOf(".") + 1;
                    //variable = variable.Substring(beginIndex, endIndex);
                    len = endIndex - beginIndex;
                    variable = variable.substr(beginIndex, len);
                }

                if (list.indexOf(variable) < 0)
                    list.push(variable);

                s = s.substr(endIndex + 1);
            }
        };
        /// <summary>
        /// Evaluate where clause
        /// </summary>
        /// <param name="columnValues">columns with the values</param>
        /// <param name="whereClause">where clause</param>
        /// <returns>bool type true if where clause evaluates to true</returns>
        var evaluateWhereClause = function (columnValues, whereClause) {

            if (whereClause == null || whereClause.Length == 0) {
                return true;
            }

            var s = whereClause;
            var result = true;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.indexOf(" EXISTS ") > 0 || s.indexOf(" IN ") > 0 || s.indexOf("(") > 0 || s.indexOf(")") > 0) {
                return false;
            }
            //  while we have variables
            while (s.indexOf("=") != -1) {
                var endIndex = s.indexOf("=");
                var beginIndex = s.lastIndexOf(' ', endIndex);

                var variable = s.substr(beginIndex + 1, endIndex - (beginIndex + 1));
                var operand1 = "";
                var operand2 = "";
                var operator1 = "=";

                if (variable.indexOf(".") != -1) {
                    beginIndex = variable.indexOf(".");
                    variable = variable.substr(beginIndex, endIndex);
                }

                for (var i in columnValues) {
                    if (variable == (columnValues[i].columnName)) {
                        operand1 = "'" + columnValues[i].columnValue + "'";
                        break;
                    }
                }

                s = s.substr(endIndex + 1);
                beginIndex = 0;
                endIndex = s.indexOf(' ');
                if (endIndex == -1)
                    operand2 = s.substr(beginIndex);
                else
                    operand2 = s.substr(beginIndex, endIndex);

                /* //log.fine("operand1:"+operand1+ 
                        " operator1:"+ operator1 +
                //        " operand2:"+operand2); */
                if (!VIS.Evaluator.evaluateLogicTuple(operand1, operator1, operand2)) {
                    result = false;
                    break;
                }
            }
            return result;
        };


    };
    VIS.AZoomAcross = AZoomAcross;
})(VIS, jQuery);