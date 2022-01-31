; (function (VIS, $) {

    var baseUrl = VIS.Application.contextUrl;
    //Window's Url
    var windowUrl = baseUrl + "JsonData/GetGridWindow";
    var windowInsertOrUpdateUrl = baseUrl + "JsonData/InsertOrUpdateWRecords";
    var windowDeleteUrl = baseUrl + "JsonData/DeleteWRecords";
    var windowRecordsUrl = baseUrl + "JsonData/GetWindowRecords";
    var windowRecordsForTreeNodeUrl = baseUrl + "JsonData/GetWindowRecordsForTreeNode";

    //Form's Url
    var formUrl = baseUrl + "JsonData/GetFormInfo";

    //Process's Url
    var processInfoUrl = baseUrl + "JsonData/GetProcessInfo";
    var processUrl = baseUrl + "JsonData/Process";  //Try execute Process ,return if process has Parameter
    var executeProcessUrl = baseUrl + "JsonData/ExecuteProcess"; //Finish Process if has parameters

    var personalLockUrl = baseUrl + "JsonData/UpdateOrInsertPersonalLock";

    //DataSet's Url
    var dataSetUrl = baseUrl + "JsonData/JDataSet";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var cardViewUrl = baseUrl + "JsonData/GetCardViewDetail";
    var mapViewUrl = baseUrl + "JsonData/GetLocLatLng";


    var dataContext = function () {

        //Ctx

        function updateClientCtx(data, callback) {
            $.ajax({
                url: baseUrl + "JsonData/UpdateCtx",
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify({ dCtx: data })
            }).done(function (json) {
                if (callback) {
                    callback(json);
                }
            })
        };


        function getCardViewInfo(AD_Window_ID, AD_Tab_ID,AD_CardView_ID,sql, callback) {
            var data = { AD_Window_ID: AD_Window_ID, AD_Tab_ID: AD_Tab_ID, AD_CardView_ID: AD_CardView_ID, SQL: sql };
            $.ajax({
                url: cardViewUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify(data)
            }).done(function (jString) {
                if (callback) {
                    var cv = null;
                    if (jString) {
                        cv = jString;
                        if (typeof jString === "string") {
                            cv = JSON.parse(jString);
                        }
                    }
                    callback(cv);
                }
            })
        };

        function getLocLatLng(lstLocIds, callback) {
            var data = { locIds: lstLocIds };
            $.ajax({
                url: mapViewUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify(data)
            }).done(function (jString) {
                if (callback) {
                    var cv = null;
                    if (jString) {
                        cv = jString;
                        if (typeof jString === "string") {
                            cv = JSON.parse(jString);
                        }
                    }
                    callback(cv);
                }
            })
        };





        //Windows
        function getWindowJString(data, callback) {
            $.ajax({
                url: windowUrl,
                type: "GET",
                datatype: "json",
                async: true,
                data: data
            }).done(function (json) {
                if (callback) {
                    callback(json);
                }
            })
        };
        function insertOrUpdateWRecord(data) {

            var result = null;
            $.ajax({
                url: windowInsertOrUpdateUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                //return result;
            })
            return result;
        };
        function deleteWRecords(data) {
            return new Promise(function (resolve, reject) {
                var result = null;
                $.ajax({
                    url: windowDeleteUrl,
                    type: "POST",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(data)
                }).done(function (json) {
                    result = json;
                    resolve(result);
                    //return result;
                })
                return result;
            })
        };

        function getWindowRecords(sqlIn, fields, rowCount, SQL_Count, AD_Table_ID, obscureFields, callback) {
            var data = { fields: fields, sqlIn: sqlIn, rowCount: rowCount, sqlCount: SQL_Count, AD_Table_ID: AD_Table_ID, obscureFields: obscureFields };

            $.ajax({
                url: windowRecordsUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify(data)
            }).done(function (jString) {
                if (callback) {
                    var retObj = JSON.parse(jString);
                    var dataSet = null;
                    var lookupDirect = null;
                    var cardViewData = null;
                    if (retObj) {
                        dataSet = new VIS.DB.DataSet().toJson(retObj.Tables);
                        lookupDirect = retObj.LookupDirect;
                        cardViewData = retObj.CardViewTpl;
                    }
                    callback(dataSet, lookupDirect, cardViewData);
                }
            })
        };


        function getWindowRecordsForTreeNode(sqlIn, fields, rowCount, SQL_Count, AD_Table_ID, treeID, treeNode_ID, callback) {
            var data = { fields: fields, sqlIn: sqlIn, rowCount: rowCount, sqlCount: SQL_Count, AD_Table_ID: AD_Table_ID, treeID: treeID, treeNodeID: treeNode_ID };

            $.ajax({
                url: windowRecordsForTreeNodeUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify(data)
            }).done(function (jString) {
                if (callback) {
                    var dataSet = null;
                    if (jString)
                        dataSet = new VIS.DB.DataSet().toJson(jString);
                    callback(dataSet);
                }
            })
        };


        function getWindowRecord(sql, fields, obscureFields) {
            var result = null;
            $.ajax({
                url: baseUrl + "JsonData/GetWindowRecord",
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify({ sql: sql, fields: fields,obscureFields: obscureFields })
            }).done(function (jString) {
                if (jString)
                    result = new VIS.DB.DataReader().toJson(jString);
            })
            return result;
        }

        function updateInsertLocks(AD_User_ID, AD_Table_ID, Record_ID, locked) {

            var result = null;
            var data = { AD_User_ID: AD_User_ID, AD_Table_ID: AD_Table_ID, Record_ID: Record_ID, locked: locked };
            $.ajax({
                url: personalLockUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                //return result;
            })
            return result;
        };



        function getJSONRecord(actionUrl, fields) {
            var result = null;
            if (actionUrl.indexOf(VIS.Application.contextUrl) == -1)
                actionUrl = VIS.Application.contextUrl + actionUrl;

            $.ajax({
                url: actionUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify({ fields: fields })
            }).done(function (jString) {
                if (jString)
                    result = JSON.parse(jString);
            }).fail(function (jqXHR, textStatus, et) {
                alert(et);
            });
            return result;
        };

        //Form
        function getFormDataJString(data, callback) {
            $.ajax({
                url: formUrl,
                type: "GET",
                datatype: "json",
                async: true,
                data: data
            }).done(function (json) {
                if (callback) {
                    callback(json);
                }
            })
        };

        //process
        function getProcessInfoJString(data, callback) {
            $.ajax({
                url: processInfoUrl,
                type: "GET",
                datatype: "json",
                async: true,
                data: data
            }).done(function (json) {
                if (callback) {
                    callback(json);
                }
            })
        };
        // updated by vinay for ftpt coa work
        function process(data, callback) {
            var asyn = callback ? true : false;
            var jRet = null;
            $.ajax({
                url: processUrl,
                type: "POST",
                datatype: "json",
                async: asyn,
                data: data
            }).done(function (json) {
                jRet = json;
                if (callback) {
                    callback(json);
                }
            })
            return jRet;
        };

        function processAsyncFalse(data, callback) {
            $.ajax({
                url: processUrl,
                type: "GET",
                datatype: "json",
                async: false,
                data: data
            }).done(function (json) {
                if (callback) {
                    callback(json);
                }
            })
        };
        // updated by vinay for frpt coa work
        function executeProcess(data, callback) {

            var asyn = callback ? true : false;
            var jRet = null;
            $.ajax({
                url: executeProcessUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async:asyn,
                data: JSON.stringify(data)
            }).done(function (json) {
                jRet = json;
                if (callback) {
                    callback(json);
                }
            })
            return jRet;
        };

        //DataSet String
        function getDataSetJString(data, async, callback) {
            var result = null;
           
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





        /* Execute Non Query */

        function executeQuery(data, async, callback) {
            var result = null;
            
            $.ajax({
                url: nonQueryUrl + 'y',
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
            });
            return result;
        };

        function executeQueries(data, async, callback) {
            var result = null;
         
            $.ajax({
                url: nonQueryUrl + 'ies',
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
            });
            return result;
        };

        function getJSONData(actionUrl, data, callback) {
            var asyc = callback ? true : false;
            var result = null;
            $.ajax({
                url: actionUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: asyc,
                data: JSON.stringify(data)
            }).done(function (jString) {

                if (jString) {
                    result = jString;
                    if (typeof jString === "string") {
                        result = JSON.parse(jString);
                    }
                }
                if (asyc) {
                    callback(result);
                }
            })
            return result;
        };

        function postJSONData(actionUrl, data, callback) {
            var asyc = callback ? true : false;
            var result = null;
            $.ajax({
                url: actionUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: asyc,
                data: JSON.stringify(data)
            }).done(function (jString) {

                if (jString) {
                    result = jString;
                    if (typeof jString === "string") {
                        result = JSON.parse(jString);
                    }
                }
                if (asyc) {
                    callback(result);
                }
            })
            return result;
        };

        function getChatRecords(data, callback) {
            $.ajax({
                url: VIS.Application.contextUrl + 'Chat/LoadChat',
                data: data,
                type: 'GET',
                cache: false,
                datatype: 'Json',
                success: function (json) {
                    if (callback) {
                        callback(json);
                    }
                }
            });
        };

        function saveChat(prop) {
            $.ajax({

                url: VIS.Application.contextUrl + 'Chat/SaveChat',
                type: 'POST',
                dataType: 'Json',
                data: prop,
                success: function (data) {
                    if (data) {
                        console.log("Chat Saved");
                    }
                    else
                        console.log("Chat Not Saved");
                },
                error: function (data) {
                    console.log("Chat Not Saved");
                }


            });
        };

        function getTreeAsString(data, callback) {
            var result = null;
            var async = callback ? true : false;
            $.ajax({
                url: baseUrl + "Tree/GetTreeAsString",
                async: async,
                data: data
            }).done(function (str) {
                result = str;
                if (callback) {
                    callback(str);
                }
            });
            return result;
        };

        function subscribeUnsubscribeRecords(CM_SubScribedID, AD_Window_ID, Record_ID, AD_Table_ID, reloadSubscribe) {
            var url;
            var data;
            if (CM_SubScribedID == 0) {
                url = VIS.Application.contextUrl + 'Subscribe/Subscribe';
                data = { AD_Window_ID: AD_Window_ID, Record_ID: Record_ID, AD_Table_ID: AD_Table_ID };
            }
            else {
                url = VIS.Application.contextUrl + 'Subscribe/UnSubscribe';
                data = { AD_Window_ID: AD_Window_ID, Record_ID: Record_ID, AD_Table_ID: AD_Table_ID };
            }
            $.ajax({

                url: url,
                type: 'GET',
                dataType: 'Json',
                data: data,
                success: function (result) {
                    if (result == 0) {
                        if (CM_SubScribedID == 0) {
                            //VIS.ADialog.error(VIS.Msg.getMsg("SubscriptionFailed"));
                            VIS.ADialog.error("SubscriptionFailed");
                        }
                        else {
                            //VIS.ADialog.error(VIS.Msg.getMsg("UnSubscriptionFailed"));
                            VIS.ADialog.error("UnSubscriptionFailed");
                        }
                    }
                    else if (result == 2) {
                        //VIS.ADialog.info(VIS.Msg.getMsg("AlreadyUnsubscribed"));
                        VIS.ADialog.info("AlreadyUnsubscribed");
                    }
                    reloadSubscribe();
                },
                error: function (r) {
                    VIS.ADialog.error(VIS.Msg.getMsg("Error") + r.statusText);
                }
            });
        };


        //object
        var dc = {

            insertOrUpdateWRecord: insertOrUpdateWRecord,
            getWindowJString: getWindowJString,
            deleteWRecords: deleteWRecords,
            getWindowRecords: getWindowRecords,
            getWindowRecordsForTreeNode: getWindowRecordsForTreeNode,
            getWindowRecord: getWindowRecord,
            updateInsertLocks: updateInsertLocks,

            getDataSetJString: getDataSetJString,
            getFormDataString: getFormDataJString,

            getProcessDataString: getProcessInfoJString,
            process: process,
            processAsyncFalse:processAsyncFalse,
            executeProcess: executeProcess,

            updateClientCtx: updateClientCtx,

            executeQueries: executeQueries,
            executeQuery: executeQuery,
            getJSONRecord: getJSONRecord,
            getJSONData: getJSONData,
            subscribeUnsubscribeRecords: subscribeUnsubscribeRecords,
            getChatRecords: getChatRecords,
            saveChat: saveChat,

            getTreeAsString: getTreeAsString,
            getCardViewInfo: getCardViewInfo,
            postJSONData: postJSONData
        };
        return dc;
    }();

    VIS.dataContext = dataContext;

}(VIS, jQuery));