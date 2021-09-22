; (function (VIS, $) {

    var baseUrl = VIS.Application.contextUrl;
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var executeQuery = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls, param: params };

        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
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

    var MUserQuery = {

        alreadyExist: false,
        id: 0,

        getData: function (ctx, AD_Tab_ID, AD_Table_ID, valueColumnName) {
            var AD_Client_ID = ctx.getAD_Client_ID();
            var dr = null;
            //var sql = "SELECT Name," + valueColumnName + ", AD_UserQuery_ID FROM AD_UserQuery WHERE"
            //    + " AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y'"
            //    + " AND (AD_Tab_ID=" + AD_Tab_ID + " OR AD_Table_ID=" + AD_Table_ID + ")"
            //    + " ORDER BY Upper(Name), AD_UserQuery_ID";
            try {
                //dr = VIS.DB.executeDataReader(sql, null, null);
                dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "ASearch/GetData", { "ColumnName": valueColumnName, "Tab_ID": AD_Tab_ID, "Table_ID": AD_Table_ID }, null);
            }
            catch (ex) {
                //if (dr != null) {
                //    dr = null;
                //}
                //_log.Log(Level.SEVERE, sql, ex);
                console.log(ex);
            }
            return dr;
        },

        getQueryLines: function (AD_UserQuery_ID) {
            var dr = null;
            var lines = [];
            //var sql = "SELECT KEYNAME,KEYVALUE,OPERATOR AS OPERATORNAME,VALUE1NAME," +
            //    "VALUE1VALUE,VALUE2NAME,VALUE2VALUE,AD_USERQUERYLINE_ID FROM AD_UserQueryLine WHERE AD_UserQuery_ID=" +
            //    AD_UserQuery_ID + " ORDER BY SeqNo";
            try {
                //dr = VIS.DB.executeDataReader(sql, null, null);

                dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "ASearch/GetQueryLines", { "UserQuery_ID": AD_UserQuery_ID }, null);
                var optrName = "";
                var optr = "";
                //while (dr.read()) {
                //    var obj = {};
                //    obj["KEYNAME"] = dr.get("KEYNAME");
                //    obj["KEYVALUE"] = dr.get("KEYVALUE");
                //    obj["OPERATORNAME"] = dr.get("OPERATORNAME");
                //    obj["VALUE1NAME"] = dr.get("VALUE1NAME");
                //    obj["VALUE1VALUE"] = dr.get("VALUE1VALUE");
                //    obj["VALUE2NAME"] = dr.get("VALUE2NAME");
                //    obj["VALUE2VALUE"] = dr.get("VALUE2VALUE");
                //    obj["AD_USERQUERYLINE_ID"] = dr.get("AD_USERQUERYLINE_ID");

                //    optrName = dr.get("OPERATORNAME").toString();
                //    optr = VIS.Query.prototype.OPERATORS[optrName];
                //    obj["OPERATOR"] = optr;
                //    lines.push(obj);
                //}

                if (dr != null) {
                    for (var i in dr) {
                        var obj = {};
                        obj["KEYNAME"] = dr[i]["KEYNAME"];
                        obj["KEYVALUE"] = dr[i]["KEYVALUE"];
                        obj["OPERATORNAME"] = dr[i]["OPERATORNAME"];
                        obj["VALUE1NAME"] = dr[i]["VALUE1NAME"];
                        obj["VALUE1VALUE"] = dr[i]["VALUE1VALUE"];
                        obj["VALUE2NAME"] = dr[i]["VALUE2NAME"];
                        obj["VALUE2VALUE"] = dr[i]["VALUE2VALUE"];
                        obj["AD_USERQUERYLINE_ID"] = dr[i]["AD_USERQUERYLINE_ID"];
                        obj["FULLDAY"] = dr[i]["FULLDAY"];
                        optrName = dr[i]["OPERATORNAME"];
                        optr = VIS.Query.prototype.OPERATORS[optrName];
                        obj["OPERATOR"] = optr;
                        lines.push(obj);
                    }
                }
            }
            catch (ex) {
            }
            return lines;
        },

        deleteLines: function (AD_UserQuery_ID) {                                      // Not used any where


            var sqlQry = "VIS_147";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_UserQuery_ID", AD_UserQuery_ID);

            var no = executeQuery(sqlQry, param, null);
            //log.Info("#" + no);
            //_lines = null;
            return no >= 0;
        },

        deleteUserQuery: function (AD_UserQuery_ID) {
            var no = -1;
            $.ajax({
                url: VIS.Application.contextUrl + 'ASearch/DeleteQuery',
                type: "POST",
                datatype: "json",
                async: false,
                data: { id: AD_UserQuery_ID }
            }).done(function (json) {
                no = parseInt(json);
            })
            return no >= 0;
        },

        insertOrUpdate: function (value, name, where, AD_Tab_ID, AD_Table_ID, dsAdvanceData, getID) {
            var no = -1;
            $.ajax({
                url: VIS.Application.contextUrl + 'ASearch/InsertOrUpdateQuery',
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify({ id: value, name: name, where: where, tabid: AD_Tab_ID, tid: AD_Table_ID, qLines: dsAdvanceData })
            }).done(function (json) {
                no = parseInt(json);
                if (no == -5) {
                    MUserQuery.alreadyExist = true;
                }
            })
            if (no > 0) {
                MUserQuery.id = no;
            }
            return no >= 0;
        },

    };

    function Find(windowNo, curTab, minRecord) {
        var title = curTab.getName();
        var AD_Tab_ID = curTab.getAD_Tab_ID();
        var AD_Table_ID = curTab.getAD_Table_ID();
        var tableName = curTab.getTableName();
        var whereExtended = curTab.getWhereClause();
        var findFields = curTab.getFields();
        this.btnfields = [];

        var $root = $("<div  class='vis-forms-container' style='height:100%'>");
        var $busy = null;

        var $self = this;
        var ch = null;
        var btnOk, btnCancel, btnDelete, btnSave, btnRefresh;
        var txtQryName, drpSavedQry, drpColumns, drpOp, drpDynamicOp, chkDynamic, txtYear, txtMonth, txtDay, txtStatus, chkFullDay, spanAddFilter, btnBack;
        var ulQryList, divDynamic, divYear, divMonth, divDay, divValue1, divValue2, tblGrid, tblBody, divFullDay, inputWarps, lblQryValue;

        var FIELDLENGTH = 20, TABNO = 99;

        var total = 0, isLoadError = false, isSaveError = false, isBusy = false;
        var dsAdvanceData = null;
        var log = VIS.Logging.VLogger.getVLogger("Find");

        this.onClose = null, this.created = false, this.days = 0, this.okPressed = false, this.okBtnPressed = false, this.needRefreshWindow = false;

        var control1, control2, ulListStaticHtml = "";;
        this.saveQueryID = -1;
        var saveChanges = false;
        var savedFiltersCount = 0;

        var query = new VIS.Query(tableName); //query
        query.addRestriction(whereExtended); // restriction

        //$root.load(VIS.Application.contextUrl + 'ASearch/Index/?windowNo=' + windowNo, function (evt) {
        //    initUI();
        //    initFind();
        //    bindEvents();

        //});
        setView();
        function setView() {
            var dStyle = '';
            var isRTL = false;
            //if (VIS.Application.isRTL) {
            //    isRTL = true;
            //    dStyle = "border-left: 3px solid #d9e3e7;";
            //}
            //else {
            //    dStyle = "border-right: 3px solid #d9e3e7;";
            //}


            var html = '<div class="vis-advancedSearch-contentWrap"> <div class="vis-advancedSearchContentArea vis-pull-left" style="' + dStyle + '">'
                + ' <div class="vis-advancedSearchContentArea-up"> <div class="vis-advanedSearch-InputsWrap"><div class="vis-as-topfieldswrap">'
                + '<div style="display:none" class="vis-form-group vis-advancedSearchInput vis-adsearchgroup1">'
                + '<input readonly id="txtQryName_' + windowNo + '" type="text" name="QueryName" maxlength="60">'
                + '<label id="lblQryName_' + windowNo + '" for="QueryName">' + VIS.Msg.getMsg("AddNameToSaveSearch") + '</label>'
                + '</div> <div  class="vis-form-group vis-advancedSearchInput vis-adsearchgroup2">'
                + '<label id="lblSavedQry_' + windowNo + '" for="GetSavedQuery">' + VIS.Msg.getMsg("GetSavedSearch") + '</label>'
                + '<select id="drpSavedQry_' + windowNo + '"></select>'
                + '</div>'

                + ' <div class="vis-advancedSearch-Icons vis-pull-left vis-adsearchgroup2">'
                + '   <ul>'

                + '   <li class="vis-pull-left"><button disabled id="btnDelete_' + windowNo + '" class="vis-advancedSearchActionIcon vis-advancedSearch-delIcon"><i class="vis vis-delete" aria-hidden="true"></i></button></li>'
                + '</ul>'
                + '</div></div>'
                + '<div class="vis-advanedSearch-AddFilterWrap vis-pull-left">'
                + '<label id="spnAddFilter_' + windowNo + '" class="vis-advancedSearch-AddFilter">' + VIS.Msg.getMsg("AddFilter") + '</label>'
                + '</div>'
                + '  <div class="vis-as-backbtn"><button id="btnArowBack_' + windowNo + '" class="vis-ads-icon"><i class="fa fa-arrow-left" aria-hidden="true"></i></button></div> '
                + '</div>'

                + '<div class="vis-advanedSearch-InputsWrap vis-advs-inputwraps vis-pull-left" data-show="N" style="display:none">'
                + '  <div class="vis-form-group vis-advancedSearchInput vis-advancedSearchInput-v">'
                + '    <label id="lblColumn_' + windowNo + '"  for="Column">' + VIS.Msg.getMsg("Column") + '</label>'
                + '  <select id="drpColumn_' + windowNo + '">'
                + '</select>'
                + '</div>'

                + '<div class="vis-form-group vis-advancedSearchInput vis-advancedSearchInput-op">'
                + '<label id="lblOperator_' + windowNo + '" for="Oprator">' + VIS.Msg.getMsg("Operator") + '</label>'
                + '<select id="drpOperator_' + windowNo + '">'
                + '</select>'
                + '</div>'

                + ' <div class="vis-form-group vis-advancedSearchInput vis-advancedSearchInput-v" id="divValue1_' + windowNo + '">'
                + '   <label  id="lblQryValue_' + windowNo + '" for="QueryValue">' + VIS.Msg.getMsg("QueryValue") + '</label>'
                + ' <input  id="txtQryValue_' + windowNo + '" type="text" name="QueryValue">'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchInput vis-advancedSearchInput-v" id="divValue2_' + windowNo + '">'
                + '<label for="QueryName"  id="lblToQryValue_' + windowNo + '">' + VIS.Msg.getMsg("ToQueryValue") + '</label>'
                + '<input  id="txtToQryValue_' + windowNo + '" type="text" name="QueryName">'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchInput vis-advancedSearchInput-v" style="display:none;" id="divFullDay_' + windowNo + '">'

                + '<input style="width: auto;float: left;" id="checkFullDay_' + windowNo + '" type="checkbox" name="QueryName">'
                + '<label for="QueryName"  id="lblToQryValue_' + windowNo + '">' + VIS.Msg.getMsg("FullDay") + '</label>'
                + '</div>'

                + '<div class="vis-advancedSearch-calender-Icon vis-pull-left">'
                + '<ul>'
                + '<li class="vis-pull-left"><button id="btnSave_' + windowNo + '" disabled class="vis-ads-icon"><i class="vis vis-plus" aria-hidden="true"></i></button></li>'
                + '</ul>'
                + '</div>'

                + '</div>'
                + '<div id="divDynamic_' + windowNo + '">'

                + '<div class="vis-advanedSearch-InputsWrap vis-advancedSearchMrgin">'
                + '<div class="vis-form-group vis-advancedSearchInput1">'
                + '<input type="checkbox"  id="chkDynamic_' + windowNo + '"  name="IsDynamic" class="vis-pull-left">'
                + '<label for="IsDynamic" >' + VIS.Msg.getMsg("IsDynamic") + '</label>'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchInput">'
                + '<select id="drpDynamicOp_' + windowNo + '" disabled>'
                + '<option>' + VIS.Msg.getMsg("Today") + '</option>'
                + '<option>' + VIS.Msg.getMsg("lastxDays") + '</option>'
                + '<option>' + VIS.Msg.getMsg("lastxMonth") + '</option>'
                + '<option>' + VIS.Msg.getMsg("lastxYears") + '</option>'
                + '</select>'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchHorigontal vis-pull-left" id="divYear_' + windowNo + '">'
                + '<input id="txtYear_' + windowNo + '" type="number" min="1" max="99" />'
                + '<label for="Year">' + VIS.Msg.getMsg("Year") + '</label>'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchHorigontal vis-pull-left" id="divMonth_' + windowNo + '">'
                + '<input id="txtMonth_' + windowNo + '" type="number" min="0" max="12" />'
                + '<label for="Month">' + VIS.Msg.getMsg("Month") + '</label>'
                + '</div>'
                + '<div class="vis-form-group vis-advancedSearchHorigontal vis-pull-left" id="divDay_' + windowNo + '">'
                + '<input id="txtDay_' + windowNo + '" type="number" min="0" max="31" />'
                + ' <label for="Day">' + VIS.Msg.getMsg("Day") + '</label>'
                + '</div>'
                + '</div>'
                + '</div>'

                + '</div>'

                + '<div class="vis-advancedSearchContentArea-down">'
                + '<div class="vis-advancedSearchTableWrap vis-table-responsive vis-pull-left">'

                + '<table id="tblQry_' + windowNo + '" class="vis-advancedSearchTable">'
                + '<thead>'
                + '<tr class="vis-advancedSearchTableHead">'
                + '<th>' + VIS.Msg.translate(VIS.Env.getCtx(), "AD_Column_ID") + '</th>'
                + '<th style="display:none">' + VIS.Msg.translate(VIS.Env.getCtx(), "KEYVALUE") + '</th>'
                + '<th>' + VIS.Msg.translate(VIS.Env.getCtx(), "OperatorName") + '</th>'
                + '<th>' + VIS.Msg.translate(VIS.Env.getCtx(), "QueryValue") + '</th>'
                + '<th style="display:none">' + VIS.Msg.translate(VIS.Env.getCtx(), "VALUE1VALUE") + '</th>'
                + '<th>' + VIS.Msg.translate(VIS.Env.getCtx(), "QueryValue2") + '</th>'
                + '<th>' + VIS.Msg.getMsg("FullDay") + '</th>'
                + '<th style="display:none">' + VIS.Msg.translate(VIS.Env.getCtx(), "VALUE2VALUE") + '</th>'
                + '<th style="display:none">' + VIS.Msg.translate(VIS.Env.getCtx(), "AD_USERQUERYLINE_ID") + '</th>'
                + '<th style="display:none">' + VIS.Msg.translate(VIS.Env.getCtx(), "Operator") + '</th>'
                + '<th>' + VIS.Msg.translate(VIS.Env.getCtx(), "Action") + '</th>'
                + '</tr>'
                + '</thead>'

                + '<tbody class="vis-advancedSearchTableBody">'

                + '</tbody>'
                + '</table>'

                + '</div>'
                + '</div>'

                + '<div class="vis-advancedSearchContentArea-button">'
                + '<div class="vis-advcedfooterBtn">';



            dStyle = isRTL ? "float:right" : "float:left";

            html += '<button id="btnRefresh_' + windowNo + '" class="ui-button ui-corner-all ui-widget">' + VIS.Msg.getMsg("Refresh") + '</button>'
                + '<div class="vis-pull-right">'
                + '<button id="btnOk_' + windowNo + '" class="ui-button ui-corner-all ui-widget" >' + VIS.Msg.getMsg("Apply") + '</button>'
                + '  <button id="btnCancel_' + windowNo + '" class="ui-button ui-corner-all ui-widget"  style="margin: 0 10px;">' + VIS.Msg.getMsg("close") + '</button>'

                + '</div>'
                + '</div>'
                + '</div>'
                //<!-- end of advancedSearchTableWrap -->
                + '</div>'
                //<!-- end of advancedSearchContentArea -->
                + '<div class="vis-advancedSearch-RecentRecords">'
                + '  <div class="vis-RecentRecords-Heading">';

            //dStyle = isRTL ? "margin-right:15px" : "margin-left:15px";

            html += '<h4>' + VIS.Msg.getMsg("VHistory") + '</h4>'


                + '</div>'
                + '<div class="vis-RecentRecords-listWrap">'
                + ' <ul id="ulQry_' + windowNo + '" >'
                + '<li data-value="0" title="' + VIS.Msg.getMsg("All") + '" >' + VIS.Msg.getMsg("All") + '</li>'
                + '<li data-value="365" title="' + VIS.Msg.getMsg("YearAll") + '">' + VIS.Msg.getMsg("YearAll") + '</li>'
                + '<li data-value="365 | C" title="' + VIS.Msg.getMsg("YearCreated") + '">' + VIS.Msg.getMsg("YearCreated") + '</li>'
                + '<li data-value="31" title="' + VIS.Msg.getMsg("MonthAll") + '">' + VIS.Msg.getMsg("MonthAll") + '</li>'
                + '<li data-value="31 | C" title="' + VIS.Msg.getMsg("MonthCreated") + '">' + VIS.Msg.getMsg("MonthCreated") + '</li>'
                + '<li data-value="7" title="' + VIS.Msg.getMsg("WeekAll") + '">' + VIS.Msg.getMsg("WeekAll") + '</li>'
                + '<li data-value="7 | C" title="' + VIS.Msg.getMsg("WeekCreated") + '">' + VIS.Msg.getMsg("WeekCreated") + '</li>'
                + '<li data-value="1" title="' + VIS.Msg.getMsg("DayAll") + '">' + VIS.Msg.getMsg("DayAll") + '</li>'
                + '<li data-value="1 | C" title="' + VIS.Msg.getMsg("DayCreated") + '">' + VIS.Msg.getMsg("DayCreated") + '</li>'
                + '</ul>'
                + '</div>'
                + '<div class="vis-advancedSearchFooter vis-pull-right"> '
                + '<p id="pstatus_' + windowNo + '" >16 / 16</p>'
                + '</div>'
                + '</div>'

                + '</div>'

                //<!-- end of advancedSearch-GrayWrap -->

                + '<div class="vis-apanel-busy vis-advancedSearchbusy" id="divBusy_' + windowNo + '" >'
                + '<p style="text-align:center"> ' + VIS.Msg.getMsg("Loading") + '</p>'
                + '</div>'


            $root.append(html);

            initUI();
            initFind();
            bindEvents();



        };

        function initUI() {
            //right side list
            ulQryList = $root.find("#ulQry_" + windowNo);
            drpSavedQry = $root.find("#drpSavedQry_" + windowNo);
            drpColumns = $root.find("#drpColumn_" + windowNo);
            drpOp = $root.find("#drpOperator_" + windowNo);
            divValue1 = $root.find("#divValue1_" + windowNo);
            lblQryValue = $root.find("#lblQryValue_" + windowNo);
            divValue2 = $root.find("#divValue2_" + windowNo);
            divFullDay = $root.find("#divFullDay_" + windowNo);
            txtQryName = $root.find("#txtQryName_" + windowNo);

            //actions
            btnOk = $root.find("#btnOk_" + windowNo);
            btnCancel = $root.find("#btnCancel_" + windowNo);
            btnSave = $root.find("#btnSave_" + windowNo);
            btnDelete = $root.find("#btnDelete_" + windowNo);
            btnRefresh = $root.find("#btnRefresh_" + windowNo);
            //dynamic
            divDynamic = $root.find("#divDynamic_" + windowNo);
            chkDynamic = $root.find("#chkDynamic_" + windowNo);
            drpDynamicOp = $root.find("#drpDynamicOp_" + windowNo);
            txtYear = $root.find("#txtYear_" + windowNo);
            txtMonth = $root.find("#txtMonth_" + windowNo);
            txtDay = $root.find("#txtDay_" + windowNo);
            divYear = $root.find("#divYear_" + windowNo);
            divMonth = $root.find("#divMonth_" + windowNo);
            divDay = $root.find("#divDay_" + windowNo);
            chkFullDay = $root.find('#checkFullDay_' + windowNo);
            spanAddFilter = $root.find('#spnAddFilter_' + windowNo);
            btnBack = $root.find('#btnArowBack_' + windowNo);
            inputWarps = $($root.find('.vis-advs-inputwraps')[0]);
            divYear.hide();
            divMonth.hide();
            divDay.hide();
            divDynamic.hide();

            //if (VIS.Application.isRTL) {
            //    btnOk.css("margin-left", "-142px");
            //    btnCancel.css("margin-left", "70px");
            //}


            //grid 
            tblGrid = $root.find("#tblQry_" + windowNo);
            tblBody = tblGrid.find("tbody");
            txtStatus = $root.find("#pstatus_" + windowNo);
            $busy = $root.find("#divBusy_" + windowNo);
        };

        function initFind() {
            total = getNoOfRecords(null, false);
            var drListQueries = MUserQuery.getData(VIS.context, AD_Tab_ID, AD_Table_ID, "Code");

            setStatusDB(total);
            ulListStaticHtml = ulQryList.html();
            fillList(drListQueries);
            //SetBusy(false);

            var html = '<option value="-1"> </option>';
            var sortedFields = [];
            for (var c = 0; c < findFields.length; c++) {
                // get field
                var field = findFields[c];
                if (field.getIsEncrypted())
                    continue;
                // get field's column name
                var columnName = field.getColumnName();
                if (field.getDisplayType() == VIS.DisplayType.Button) {
                    if (field.getAD_Reference_Value_ID() == 0)
                        // change done here to display textbox for search in case where buttons don't have Reference List bind with Column
                        //continue;
                        field.setDisplayType(VIS.DisplayType.String);
                    else {
                        if (columnName.endsWith("_ID"))
                            field.setDisplayType(VIS.DisplayType.Table);
                        else {
                            field.setDisplayType(VIS.DisplayType.List);
                            // bind lookup for buttons having Reference List bind with column
                            field.lookup = new VIS.MLookupFactory.getMLookUp(VIS.context, windowNo, field.getAD_Column_ID(), VIS.DisplayType.List);
                        }
                        //field.loadLookUp();
                    }
                }
                // get text to be displayed
                var header = field.getHeader();
                if (header == null || header.length == 0) {
                    // get text according to the language selected
                    header = VIS.Msg.getElement(VIS.context, columnName);
                    if (header == null || header.Length == 0)
                        continue;
                }
                // if given field is any key, then add "(ID)" to it
                if (field.getIsKey())
                    header += (" (ID)");

                // add a new row in datatable and set values
                //dr = dt.NewRow();
                //dr[0] = header; // Name
                //dr[1] = columnName; // DB_ColName
                //dt.Rows.Add(dr);
                //html += '<option value="' + columnName + '">' + header + '</option>';
                sortedFields.push({ 'value': columnName, 'text': header });
            }

            // sort by text
            sortedFields.sort(function (a, b) {
                var n1 = a.text.toUpperCase();
                var n2 = b.text.toUpperCase();
                if (n1 > n2) return 1;
                if (n1 < n2) return -1;
                return 0;
            });

            for (var col = 0; col < sortedFields.length; col++) {
                html += '<option value="' + sortedFields[col].value + '">' + sortedFields[col].text + '</option>';
            }
            drpColumns.html(html);
            setBusy(false);
        };

        function bindEvents() {

            btnCancel.on("click", function () {
                if (isBusy) return;
                $self.okBtnPressed = false;
                ch.close();
            });

            chkDynamic.on("change", function () {
                var enable = chkDynamic.prop("checked");
                drpDynamicOp.prop("disabled", !enable);
                drpOp.prop("disabled", enable);
                setValueEnabled(!enable);
                setValue2Enabled(!enable);
                setEnabledFullDay(enable);
                //setFullDayState(!enable);
                if (enable) {
                    setDynamicQryControls($self.getIsUserColumn(drpColumns.val()));
                }
                else {
                    divYear.hide();
                    divMonth.hide();
                    divDay.hide();
                }
            });

            drpDynamicOp.on("change", function () {

                setDynamicQryControls();
            });

            drpColumns.on("change", function () {
                if (isBusy) return;
                chkDynamic.prop("disabled", true);
                chkDynamic.prop("checked", false);
                chkDynamic.trigger("change");
                divDynamic.hide();

                // set control at value1 position according to the column selected
                var columnName = drpColumns.val();
                setControlNullValue(true);
                if (columnName && columnName != "-1") {
                    var dsOp = null;
                    var dsOpDynamic = null;
                    // if column name is of ant ID
                    if (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")) {
                        // fill dataset with operators of type ID
                        dsOp = $self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_ID);
                    }
                    else if (columnName.startsWith("Is")) {
                        // fill dataset with operators of type Yes No
                        dsOp = $self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_YN);
                    }
                    else {
                        // fill dataset with all operators available
                        dsOp = $self.getOperatorsQuery(VIS.Query.prototype.OPERATORS);
                    }

                    var f = curTab.getField(columnName);
                    $root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 150px)');
                    if (f != null && VIS.DisplayType.IsDate(f.getDisplayType())) {
                        drpDynamicOp.html($self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DATE_DYNAMIC, true));
                        divDynamic.show();
                        chkDynamic.prop("disabled", false);
                        setDynamicQryControls();
                        $root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 195px)');

                        if (f.getDisplayType() == VIS.DisplayType.DateTime)// If Datetime, then on = operator, show full day checkbox.
                        {
                            showValue2(false);
                            showFullDay(true);
                        }
                    }
                    else if ($self.getIsUserColumn(columnName)) {
                        drpDynamicOp.html($self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DYNAMIC_ID, true));
                        divDynamic.show();
                        $root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 195px)');
                        chkDynamic.prop("disabled", false);
                        setDynamicQryControls(true);
                    }

                    if (f.getDisplayType() != VIS.DisplayType.DateTime)// If Datetime, then on = operator, show full day checkbox.
                    {
                        showValue2(true);
                        showFullDay(false);
                    }

                    if (f.getDisplayType() == VIS.DisplayType.YesNo) {
                        lblQryValue.hide();
                    }
                    else {
                        lblQryValue.show();
                    }

                    drpOp.html(dsOp);
                    drpOp[0].SelectedIndex = 0;
                    // get field
                    var field = getTargetMField(columnName);
                    // set control at value1 position
                    setControl(true, field);
                    // enable the save row button
                    setEnableButton(btnSave, true);//silverlight comment
                    drpOp.prop("disabled", false);
                }
                else {
                    showFullDay(false);
                    showValue2(true);
                }
                // enable control at value1 position
                setValueEnabled(true);
                // disable control at value2 position
                setValue2Enabled(false);

            });

            drpOp.on("change", function () {
                if (isBusy) return;
                var selOp = drpOp.val();

                // set control at value2 position according to the operator selected
                if (!selOp) {
                    selOp = "";
                }

                var columnName = drpColumns.val();
                // get field
                var field = getTargetMField(columnName);

                if (selOp && selOp != "0") {
                    //if user selects between operator
                    if (VIS.Query.prototype.BETWEEN.equals(selOp)) {

                        // set control at value2 position
                        setControl(false, field);



                        // enable the control at value2 position
                        setValue2Enabled(true);
                    }
                    else {
                        columnName = drpColumns.val();
                        field = getTargetMField(columnName);
                        if (field.getDisplayType() == VIS.DisplayType.DateTime && VIS.Query.prototype.EQUAL.equals(selOp)) {
                            showValue2(false);
                            showFullDay(true);
                        }
                        else {
                            showValue2(true);
                            showFullDay(false);
                            setValue2Enabled(false);
                        }
                    }
                }
                else {
                    setEnableButton(btnSave, false);//
                    setValue2Enabled(false);
                    setControlNullValue(true);
                }
            });

            drpSavedQry.on("change", function () {
                if (isBusy) return;
                // binds grid according to the query selected in combobox
                var val = drpSavedQry.val();
                // Done By Karan, to ask user to save save serach criteria created by User.

                if (saveChanges && tblBody.find('.vis-advancedSearchTableRow').length > 0) {
                    VIS.ADialog.confirm("SaveSearch?", true, "", "Confirm", function (result) {
                        if (!result) {
                            selectedSavedQuery(val);
                        }
                        else {
                            drpSavedQry.val($self.saveQueryID);
                        }
                    });
                }
                else {
                    selectedSavedQuery(val);
                }
            });

            tblGrid.on("click", function (e) {
                if (isBusy) return;
                // if (e.target.nodeName === "IMG") {
                if ($(e.target).hasClass('vis-delete')) {
                    var index = $(e.target).data("index");
                    dsAdvanceData.splice(index, 1);//  .Tables[0].Rows.RemoveAt(index);
                    MUserQuery.deleteLines($(e.target).data("userquery"));
                    bindGrid(dsAdvanceData);
                    savedFiltersCount--;
                    if (savedFiltersCount == 0) {
                        txtQryName.prop("readonly", true);
                        txtQryName.val("");
                    }
                }
            });

            ulQryList.on("click", "LI", function (e) {
                if (isBusy) return;


                var val = $(this).data("value");
                //try for double click list
                if (val != null) {
                    setBusy(true);

                    var ii = $(this).index();
                    setTimeout(function () {

                        if (ii < 9) {
                            var valSplit = val.toString().split('|');
                            var cnt = valSplit.length;
                            if (cnt == 1) {
                                $self.created = false;
                            }
                            else {
                                $self.created = true;
                            }

                            $self.days = parseInt(valSplit[0]);
                            val = "";
                        }
                        else {
                            $self.days = 0;
                        }

                        query = new VIS.Query(tableName);
                        query.addRestriction(val);

                        var no = 0;

                        no = getNoOfRecords(query, true);
                        query.setRecordCount(no);

                        setBusy(false);
                        if (no != 0) {
                            $self.okBtnPressed = true;
                            ch.close();
                            //Close(_isOkButtonPressed);
                        }
                    }, 10);
                }
                else {
                    // SetBusy(false);
                    $self.okBtnPressed = false;
                    ch.close();
                    // Close(_isOkButtonPressed);
                }
            });

            btnSave.on("click", saveRowTemp);

            btnDelete.on("click", function () {
                if (isBusy) return;
                var obj = drpSavedQry.val();
                if (obj == null || obj.toString() == "" || parseInt(obj) < 1)
                    return;



                VIS.ADialog.confirm("DeleteConfirm", true, "", "Confirm", function (result) {
                    if (result) {
                        setBusy(true);
                        var uq;
                        window.setTimeout(function () {

                            //var sql = "SELECT Count(*) FROM AD_DefaultUserQuery WHERE AD_UserQuery_ID=" + obj + " AND AD_User_ID!=" + VIS.Env.getCtx().getAD_User_ID();
                            //var count = VIS.DB.executeScalar(sql);
                            var count = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "ASearch/GetQueryDefault", { "UserQuery_ID": obj }, null);
                            if (count > 0) {
                                VIS.ADialog.warn("QueryDefaultForOther");
                                setBusy(false);
                                return;
                            }

                            if (curTab.userQueryID == obj) {
                                $self.needRefreshWindow = true;
                            }

                            // get name of the query
                            var name = drpSavedQry.find("option:selected").text();
                            // delete query

                            if (MUserQuery.deleteUserQuery(obj)) {
                                var drListQueries = MUserQuery.getData(VIS.context, AD_Tab_ID, AD_Table_ID, "Code");

                                ulQryList.empty();
                                ulQryList.html(ulListStaticHtml);
                                fillList(drListQueries);
                                drpSavedQry[0].selectedIndex = 0;
                                //// show message to user
                                VIS.ADialog.info("Deleted", true, name, "");
                                txtQryName.val("");
                                txtQryName.prop("readony", true);
                                setBusy(false);
                                drpSavedQry.trigger("change");
                            }
                            else {
                                VIS.ADialog.info("DeleteError", true, name, "");
                            }

                            setBusy(false);
                        }, 10);
                    }
                    else {
                        setBusy(false);
                    }
                });
            });

            btnOk.on("click", function () {
                if (isBusy) return;
                //setBusy(true);
                $self.okPressed = true;
                $self.okBtnPressed = true;
                //	Save pending
                saveAdvanced();
            });

            btnRefresh.on("click", function () {
                if (isBusy) return;
                //setBusy(true);
                // save row
                saveRowTemp();	//	unsaved 
                // get query
                var temp = getQueryAdvanced();
                var records = 0;

                records = getNoOfRecords(temp, true);
                setStatusDB(records);
                //setBusy(false);
            });

            spanAddFilter.on("click", function () {
                if (inputWarps && inputWarps.data('show') == "N") {
                    inputWarps.data('show', 'Y');
                    $(this).hide();
                    inputWarps.show();
                    btnBack.show();
                    $('.vis-adsearchgroup2').hide();
                    $('.vis-adsearchgroup1').show();
                    $('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 150px)');
                }
            });

            btnBack.on("click", function () {
                inputWarps.data('show', 'N');
                inputWarps.hide();
                spanAddFilter.show();
                btnBack.hide();
                toggleDisplay();
                //if (savedFiltersCount == 0) {
                $('.vis-adsearchgroup1').hide();
                $('.vis-adsearchgroup2').show();
                drpSavedQry[0].selectedIndex = 0;
                tblBody.empty();
                dsAdvanceData = [];
                txtQryName.val("");
                //}
            });

            txtQryName.on("input", function () {
                if (txtQryName.length > 0) {
                    btnOk.text(VIS.Msg.getMsg("SaveAndApply"));
                }
                else {
                    btnOk.text(VIS.Msg.getMsg("Apply"));
                }
            });
        };

        function toggleDisplay() {

            drpColumns[0].selectedIndex = 0;
            drpOp[0].selectedIndex = 0;
            setControlNullValue();
            setControlNullValue(true);
            showFullDay(false);
            showValue2(true);
            chkDynamic.prop('checked', false);
            chkFullDay.prop('checked', false);
            divDynamic.hide();
            $root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 100px)');
        }

        function unBindEvents() {

            if (!btnCancel)
                return;

            btnCancel.off("click");
            chkDynamic.off("change");
            drpDynamicOp.off("change");
            drpColumns.off("change");
            drpOp.off("change");
            drpSavedQry.off("change");
            tblGrid.off("click");
            ulQryList.off("click");
            btnSave.off("click");
            btnDelete.off("click");
            btnOk.off("click");
            btnRefresh.off("click");
        };

        function selectedSavedQuery(val) {
            $self.saveQueryID = val;
            saveChanges = false;
            if (val && val != "-1") {
                //setBusy(true);
                var obj = null;
                dsAdvanceData = MUserQuery.getQueryLines(val);
                bindGrid(dsAdvanceData);
                txtQryName.val(drpSavedQry.find("option:selected").text());
                txtQryName.prop("readonly", false);
                //setBusy(false);
                setEnableButton(btnDelete, true);
            }
            else {
                // if nothing is selected
                //ibtnDelQuery.Enabled = false;
                setEnableButton(btnDelete, false);//silverlight comment
                txtQryName.val("");
                txtQryName.prop("readonly", true);
                dsAdvanceData = null;
                bindGrid(null);
            }
        };

        function fillList(dr) {
            var html = "";
            var html1 = "";
            //while (dr.read()) {

            //    var query = VIS.Utility.encodeText(dr.get(0));
            //    var title = query;
            //    if (query.length > 25) {
            //        query = query.substr(0, 25) + "...";
            //    }

            //    html += '<li data-value="' + dr.get(1) + '" title="' + title + '"> ' + query + '</li>';
            //    html1 += '<option value="' + dr.get("AD_UserQuery_ID") + '" title="' + title + '"> ' + query + '</option>';
            //}

            if (dr != null) {
                for (var i in dr) {
                    var query = VIS.Utility.encodeText(dr[i].Name);
                    var title = query;
                    if (query.length > 25) {
                        query = query.substr(0, 25) + "...";
                    }

                    html += '<li data-value="' + dr[i]["Code"] + '" title="' + title + '">' + query + '</li>';
                    html1 += '<option value="' + dr[i]["AD_UserQuery_ID"] + '" title="' + title + '">' + query + '</option>';
                }
            }

            if (html.length > 0) {
                ulQryList.append(html);

                //drpSavedQry.html("<option value='-1' > </option>" + html1);   

                //Commented by karan, Because if last item deleted from dropdown,
                //then html.length is 0 and our dropdown doesn't get refreshed.. hence plased his code outside of check...
            }

            drpSavedQry.html("<option value='-1' > </option>" + html1);


        };

        /* show hide Dynamic div area */
        function setDynamicQryControls(isUser) {
            var index = drpDynamicOp[0].selectedIndex;
            if (isUser) {
                divYear.hide();
                divMonth.hide();
                divDay.hide();
                return;
            }
            divYear.show();
            divMonth.show();
            if (chkDynamic.is(':checked')) {
                divDay.show();
            }
            else {
                divDay.hide();
            }
            txtDay.prop("readonly", false);
            txtMonth.prop("min", 1);
            if (index == 3 || index == 6) {
                txtMonth.prop("min", 0);
                txtDay.val(0);
                txtMonth.val(0);
                txtYear.val(1);
            }

            else if (index == 2 || index == 5) {
                divYear.hide();
                txtYear.val("");
                txtMonth.val(1);
                txtDay.val(0);
            }
            else if (index == 1 || index == 4) {
                divYear.hide();
                divMonth.hide();
                txtDay.val(0);
            }
            else if (index == 0) {
                txtDay.prop("readonly", true);
                divYear.hide();
                divMonth.hide();
                txtDay.val(0);
                //divDay.hide();
            }
        };

        function getIsDyanamicVisible() {
            return divDay.is(':visible') || divYear.is(':visible') || divDay.is(':visible');
        };

        function getDynamicText(index) {
            var text = "";
            var timeUnit;
            if (index == 3 || index == 6) {
                timeUnit = Math.round((getTotalDays(index) / 365), 1);
                text = "Last " + timeUnit.toString() + " Years";
            }
            else if (index == 2 || index == 5) {
                timeUnit = Math.round((getTotalDays(index) / 31), 1);
                text = "Last " + timeUnit.toString() + " Month";
            }
            else {
                if (getTotalDays() != 0) {
                    text = "Last " + getTotalDays() + " Days";
                }
                else {
                    text = "This Day";
                }
            }
            return text;
        };

        function getDynamicValue(index) {
            var text = "";
            text = " adddays(sysdate, - " + getTotalDays(index) + ") ";
            return text;
        };

        function getTotalDays(index) {
            var totasldays = 0;
            if (index == 3 || index == 6) {
                var y = txtYear.val(), m = txtMonth.val(), d = txtDay.val();

                y = (y && y != "") ? parseInt(y) : 0;
                m = (m && m != "") ? parseInt(m) : 0;
                d = (d && d != "") ? parseInt(d) : 0;

                totasldays = (y * 365) + (m * 31) + (d);
            }
            else if (index == 2 || index == 5) {
                var m = txtMonth.val(), d = txtDay.val();

                m = (m && m != "") ? parseInt(m) : 0;
                d = (d && d != "") ? parseInt(d) : 0;

                totasldays = (m * 31) + (d);
            }
            else {
                var d = d = txtDay.val();
                d = (d && d != "") ? parseInt(d) : 0;
                totasldays = d;
            }
            return totasldays;
        };

        function setValueEnabled(isEnabled) {
            // get control
            var ctrl = divValue1.children()[1];
            var btn = null;
            if (divValue1.children().length > 2)
                btn = divValue1.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setValue2Enabled(isEnabled) {
            var ctrl = divValue2.children()[1];
            var btn = null;
            if (divValue2.children().length > 2)
                btn = divValue2.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        /*
        *   Show OR Hide QueryTo value 
        *   Added By Karan
        */
        function showValue2(show) {
            divValue2.css("display", show ? "block" : "none");
        };

        /*
        *   Show OR Hide Full Day value
        *   Visible only if Column is of datetime time and operator is EQUAL
        *   Added By Karan
        */
        function showFullDay(show) {
            divFullDay.css('display', show ? "flex" : "none");
        };

        /*
        *   Set Enable or disable full day icon
        *   Added By Karan
        */
        function setEnabledFullDay(enable) {
            if (enable) {
                chkFullDay.prop('disabled', true);
            }
            else {
                chkFullDay.prop('disabled', false);
            }
        };

        //function setFullDayState(state)
        //{
        //    if (state) {
        //        chkFullDay.prop("checked", true)
        //    }
        //    else {
        //        chkFullDay.prop("checked", false);
        //    }
        //}


        function setEnableButton(btn, isEnable) {
            btn.prop("disabled", !isEnable);
        };

        function setControl(isValue1, field) {
            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[1];
                if (divValue1.children().length > 2)
                    ctrl2 = divValue1.children()[2];
            }
            else {
                ctrl = divValue2.children()[1];
                if (divValue2.children().length > 2)
                    ctrl2 = divValue2.children()[2];
            }

            //var eList = from child in tblpnlA.Children
            //where Grid.GetRow((FrameworkElement)child) == row && Grid.GetColumn((FrameworkElement)child) == col
            //select child;

            //Remove any elements in the list
            if (ctrl != null) {
                $(ctrl).remove();
                if (ctrl2 != null)
                    $(ctrl2).remove();
                ctrl = null;
            }
            /**********************************/
            var crt = null;
            // if any filed is given
            if (field != null) {
                // if field id any key, then show number textbox 
                if (field.getIsKey()) {
                    crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                        field.getColumnName());
                }
                else {
                    crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                }
            }
            else {
                // if no field is given show an empty disabled textbox
                crt = new VIS.Controls.VTextBox("columnName", false, true, false, 20, 20, "format",
                    "GetObscureType", false);// VAdvantage.Controls.VTextBox.TextType.Text, DisplayType.String);
            }
            if (crt != null) {
                //crt.SetIsMandatory(false);
                crt.setReadOnly(false);
                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    crt.hideButton(false);
                    crt.setReadOnlyTextbox(false);
                }
                if (VIS.DisplayType.Text == field.getDisplayType() || VIS.DisplayType.TextLong == field.getDisplayType()) {
                    crt.getControl().attr("rows", "1");
                    crt.getControl().css("width", "100%");
                }
                else if (VIS.DisplayType.YesNo == field.getDisplayType()) {
                    crt.getControl().css("clear", "both");
                }
                else if (VIS.DisplayType.IsDate(field.getDisplayType())) {
                    crt.getControl().css("line-height", "1");
                }

                var btn = null;
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {

                    divValue1.append(crt.getControl());
                    control1 = crt;
                    if (btn) {
                        divValue1.append(btn);
                        crt.getControl().css("width", "calc(100% - 30px)");
                        btn.css("max-width", "30px");
                    }
                }
                else {
                    divValue2.append(crt.getControl());
                    control2 = crt;
                    if (btn) {
                        divValue2.append(btn);
                        crt.getControl().css("width", "calc(100% - 30px)");
                        btn.css("max-width", "30px");
                    }
                }

                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    crt.getControl().css("width", "100%");
                }
            }
        };

        function bindGrid(list) {
            tblBody.empty();
            var html = "";
            var htm = "", obj = null;

            if (list) {
                for (var i = 0, j = list.length; i < j; i++) {
                    htm = '<tr class="vis-advancedSearchTableRow">';
                    obj = list[i];
                    htm += '<td>' + obj["KEYNAME"] + '</td><td style="display:none">' + obj["KEYVALUE"] + '</td><td>' + obj["OPERATORNAME"] +
                        '</td><td>' + obj["VALUE1NAME"] + '</td><td style="display:none">' + obj["VALUE1VALUE"] + '</td><td>' + obj["VALUE2NAME"] +
                        '</td><td style="display:none">' + obj["VALUE2VALUE"] + '</td><td>' + obj["FULLDAY"] +
                        '</td><td style="display:none">' + obj["AD_USERQUERYLINE_ID"] + '</td><td style="display:none">' + obj["OPERATOR"] +
                        '</td><td><i style="cursor:pointer" data-userQuery="' + obj["AD_USERQUERYLINE_ID"] + '" data-index = "' + i + '" class="vis vis-delete"></i></td>';
                    htm += '</tr>';
                    html += htm;
                }
            }
            tblBody.html(html);
        };

        /* get total number of record */
        function getNoOfRecords(query, alertZeroRecords) {
            // make query
            var sql = "SELECT COUNT(*) FROM ";
            sql += tableName;
            var hasWhere = false;
            // add where clause if already exists
            if (whereExtended != null && whereExtended.length > 0) {
                if (whereExtended.indexOf("@") == -1) {
                    sql += " WHERE " + whereExtended;
                }
                else {
                    sql += " WHERE " + VIS.Env.parseContext(VIS.context, windowNo, whereExtended, false);
                }
                hasWhere = true;
            }
            // if user has given any query
            if (query != null && query.getIsActive()) {
                // if where clause is started, then add "AND"
                if (hasWhere) {
                    sql += " AND ";
                }
                // add "WHERE"
                else {
                    sql += " WHERE ";
                }
                sql += VIS.Env.parseContext(VIS.context, windowNo, query.getWhereClause(true), false);
            }
            //	Add Access
            var finalSQL = VIS.MRole.getDefault().addAccessSQL(sql.toString(), tableName,
                VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);
            // finalSQL = VIS.Env.parseContext(VIS.context, windowNo, finalSQL, false);

            VIS.context.setContext(windowNo, TABNO, "FindSQL", finalSQL);

            //  Execute Query
            total = 999999;

            //System.Threading.ThreadPool.QueueUserWorkItem(delegate
            //{
            //    try
            //    {
            //        //_total = int.Parse(ExecuteQuery.ExecuteScalar(finalSQL));
            //        _total = DataBase.DB.GetSQLValue(null, finalSQL, null);
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Log(Level.SEVERE, finalSQL, ex);
            //    }
            //    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => GetTotal(_total, query, alertZeroRecords));

            //});
            try {

                //total = VIS.DB.executeScalar(finalSQL, null);

                var _sql = VIS.secureEngine.encrypt(finalSQL);
                total = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "ASearch/GetNoOfRecrds", { "RecQuery": _sql }, null);
            }
            catch (ex) {
                log.Log(VIS.Level.SEVERE, finalSQL, ex);
            }
            var role = VIS.MRole.getDefault();
            //	No Records
            $self.okPressed = false;
            if (total == 0 || total == null) {
                total = 0;
                if (alertZeroRecords) {
                    VIS.ADialog.info("FindZeroRecords", true, "");
                }
            }
            //	More then allowed
            else if (query != null && role.getIsQueryMax(total)) {
                VIS.ADialog.error("FindOverMax", true, total + " > " + role.getMaxQueryRecords());//silverlight
                //MessageBox.Show("FindZeroRecords " + _total + " > " + role.GetMaxQueryRecords());
            }
            else {
                $self.okPressed = true;
                //log.Config("#" + _total);
            }
            // show query's where clause on status bar
            if (query != null) {
                //CommonFunctions.ShowMessage(query.GetWhereClause(), lblStatusBar);
            }
            return total;
        };

        function setStatusDB(currentCount) {
            var text = " " + currentCount + " / " + total + " ";
            // show records on status bar
            txtStatus.text(text);
            if (total < minRecord) {
                isLoadError = true;
                return;
            }
        };

        function getTargetMField(columnName) {
            // if no column name, then return null
            if (columnName == null || columnName.length == 0)
                return null;
            // else find field for the given column
            for (var c = 0; c < findFields.length; c++) {
                var field = findFields[c];
                if (columnName.equals(field.getColumnName()))
                    return field;
            }
            return null;
        }; addRow

        function saveRowTemp() {
            // set column name

            var cVal = drpColumns.val();

            if (!cVal || cVal == "-1")
                return false;

            var colName = drpColumns.find("option:selected").text();
            var colValue = "";
            if (colName == null || colName.trim().length == 0) {
                return false;
            }
            else {
                // set column value
                colValue = cVal.toString();
            }

            saveChanges = true;

            var dCheck = chkDynamic.prop("checked");

            if (dCheck) {

                if (getIsDyanamicVisible()) {
                    var opValueD = ">=";
                    var opNameD = " >= ";
                    var controlText = getDynamicText(drpDynamicOp[0].selectedIndex);
                    var controlValue = getDynamicValue(drpDynamicOp[0].selectedIndex);
                    addRow(colName, colValue, opNameD, opValueD, controlText, controlValue, null, null, getFullDay());
                }
                else {
                    var opValueD = "=";
                    var opNameD = " = ";
                    var controlText = drpDynamicOp.find("option:selected").text();
                    var controlValue = drpDynamicOp.val();
                    addRow(colName, colValue, opNameD, opValueD, controlText, controlValue, null, null, getFullDay());
                }

            }
            else {

                // set operator name
                var opName = drpOp.val();

                if (drpOp[0].selectedIndex > -1)
                    opName = drpDynamicOp.find("option:selected").text();;;// vcmbOperator.Text;//silverlight comment
                // set operator (sign)
                var opValue = drpOp.val();
                // add row in dataset
                addRow(colName, colValue, opName, opValue, getControlText(true), getControlValue(true), getControlText(false), getControlValue(false), getFullDay());
            }
            //reset column & operator comboBox
            txtQryName.prop("readonly", false);
            toggleDisplay();
            savedFiltersCount++;
            if (inputWarps && inputWarps.data('show') == "Y") {
                inputWarps.data('show', 'N');
                inputWarps.hide();
                spanAddFilter.show();
                btnBack.hide();
                $('.vis-adsearchgroup2').hide();
                $('.vis-adsearchgroup1').show();
            }
            return true;
        };

        /* Gets control's value*/

        function getControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            else {
                //  crtlObj = (IControl)tblpnlA2.GetControlFromPosition(3, 1);
                crtlObj = control2;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                // return control's value
                if (crtlObj.displayType == VIS.DisplayType.AmtDimension) {
                    return crtlObj.getText();
                }
                else {
                    return crtlObj.getValue();
                }
            }
            return "";
        };

        /* <param name="isValue1">true if get control's text at value1 position else false</param>
         */
        function getControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            else {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(3, 1);
                crtlObj = control2;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text

                if (crtlObj.displayType == VIS.DisplayType.AmtDimension) {
                    return crtlObj.getText();
                }
                else {
                    return crtlObj.getDisplay();
                }
            }
            return "";
        };

        function getFullDay() {
            if (chkFullDay) {
                return chkFullDay.is(':checked') ? 'Y' : 'N';
            }
        };

        function addRow(colName, colValue, optr, optrName,
            value1Name, value1Value, value2Name, value2Value, fullDay) {

            if (dsAdvanceData == null)
                dsAdvanceData = [];

            var obj = {};
            obj["KEYNAME"] = colName;
            //dsAdvanceData.Tables[0].Columns.Add(dc);
            obj["KEYVALUE"] = colValue;

            obj["OPERATORNAME"] = optrName;
            obj["VALUE1NAME"] = VIS.Utility.encodeText(value1Name);

            obj["FULLDAY"] = fullDay;
            if (value1Name == "")
                obj["VALUE1VALUE"] = "";
            else {
                if (value1Value == null)
                    obj["VALUE1VALUE"] = "NULL";
                else
                    obj["VALUE1VALUE"] = VIS.Utility.encodeText(VIS.Utility.Util.getValueOfString(value1Value));
            }

            obj["VALUE2NAME"] = VIS.Utility.encodeText(value2Name);
            if (value2Value == null)
                obj["VALUE2VALUE"] = "NULL";
            else
                obj["VALUE2VALUE"] = VIS.Utility.encodeText(VIS.Utility.Util.getValueOfString(value2Value));
            obj["AD_USERQUERYLINE_ID"] = 0;
            obj["OPERATOR"] = optr;
            dsAdvanceData.push(obj);
            bindGrid(dsAdvanceData);//for the time beeing commented today 3Dec.2010
        };


        function setControlNullValue(isValue2) {
            var crtlObj = null;
            if (isValue2) {
                crtlObj = control2;
            }
            else {
                crtlObj = control1;
            }

            // if control exists
            if (crtlObj != null) {
                crtlObj.setValue(null);
            }

        };

        function getQueryAdvanced() {
            var _query = new VIS.Query(tableName);
            // check if dataset have any table
            // for every row in dataset
            if (dsAdvanceData) {

                for (var i = 0; i < dsAdvanceData.length; i++) {
                    var dtRowObj = dsAdvanceData[i];
                    //	Column
                    var infoName = dtRowObj["KEYNAME"].toString();
                    var columnName = dtRowObj["KEYVALUE"].toString();
                    var field = getTargetMField(columnName);
                    var columnSQL = field.getColumnSQL(); //


                    //	Operator
                    var optr = dtRowObj["OPERATORNAME"].toString(); //dtRowObj["OPERATOR"].ToString()
                    //	Value

                    var value = dtRowObj["VALUE1VALUE"];
                    var parsedValue = null;
                    if (value != null && value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@")) {
                        ;
                    }
                    else {
                        parsedValue = parseValue(field, value);
                    }
                    //string infoDisplay = dtRowObj["VALUE1NAME"].ToString();
                    var infoDisplay = null;

                    if (value == null || value.toString().length < 1) {
                        if (VIS.Query.prototype.BETWEEN.equals(optr))
                            continue;	//	no null in between
                        parsedValue = VIS.Env.NULLString;
                        infoDisplay = "NULL";
                    }
                    else {
                        infoDisplay = dtRowObj["VALUE1NAME"].toString();
                    }

                    var fullDay = dtRowObj["FULLDAY"].toString();

                    if (field.getIsVirtualColumn()) {
                        columnSQL = field.vo.ColumnSQL;
                        columnName = field.vo.ColumnSQL;

                        if (VIS.Query.prototype.BETWEEN.equals(optr)) {
                            var value2 = dtRowObj["VALUE2VALUE"].toString();
                            if (value2 == null || value2.toString().trim().length < 1)
                                continue;

                            var parsedValue2 = parseValue(field, value2);
                            var infoDisplay_to = dtRowObj["VALUE2NAME"].toString();
                            if (parsedValue2 == null)
                                continue;

                            //var Where = "UPPER( " + columnName + ") BETWEEN UPPER('" + parsedValue + "') AND UPPER('" + parsedValue2 + "')";

                            var Where = createDirectSql(parsedValue, parsedValue2, columnName, optr, true,true);

                            where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                            _query.addRestriction(Where);
                        }
                        else {
                            //var Where = columnName + optr + value;

                            var Where = createDirectSql(parsedValue, parsedValue2, columnName, optr, true,true);
                            where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                            _query.addRestriction(Where);
                        }
                    }
                    else {

                        var tabName = "C_DimAmt";
                        var isAct = "IsActive";
                        var amt = "Amount";
                        var S = "S";
                        var E = "E";
                        var L = "L";
                        var elt = "ECT";


                        var F = "F";
                        var R = "R";
                        var OM = "OM";

                        var WH = "WH";


                        if (field.getDisplayType() == VIS.DisplayType.DateTime && VIS.Query.prototype.EQUAL.equals(optr) && parsedValue && fullDay == 'Y') {

                            var yearr = parsedValue.getFullYear();
                            var month = parsedValue.getMonth();
                            var date = parsedValue.getDate();
                            parsedValue2 = new Date(yearr, month, date, 24, 00, 00);
                            parsedValue = new Date(yearr, month, date, 00, 00, 00);
                            dtRowObj["VALUE2VALUE"] = parsedValue2.toUTCString();
                            optr = VIS.Query.prototype.BETWEEN;
                        }



                        //	Value2
                        // if "BETWEEN" selected
                        if (VIS.Query.prototype.BETWEEN.equals(optr)) {

                            var value2 = dtRowObj["VALUE2VALUE"].toString();
                            if (value2 == null || value2.toString().trim().length < 1)
                                continue;
                            if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {


                                var sqlAmount = S + E + L + elt + " " + tabName + "_ID " + F + R + OM + " " + tabName + " " + WH + E + R + E + " " + isAct + "='Y' AND " + amt + " " + optr + value + " AND " + value2;
                                parsedValue = VIS.MRole.getDefault().addAccessSQL(sqlAmount.toString(), "C_DimAmt",
                                    VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

                                optr = VIS.Query.prototype.IN;

                            }


                            if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                                var Where = createDirectSql(parsedValue, parsedValue2, columnSQL, optr, false,false);
                                where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                                _query.addRestriction(Where);
                            }
                            else {
                                var parsedValue2 = parseValue(field, value2);
                                var infoDisplay_to = dtRowObj["VALUE2NAME"].toString();
                                if (parsedValue2 == null)
                                    continue;
                                // else add restriction where clause to query with between operator
                                _query.addRangeRestriction(columnSQL, parsedValue, parsedValue2, infoName,
                                    infoDisplay, infoDisplay_to);
                            }
                        }
                        else {
                            // else add simple restriction where clause to query

                            if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                                var sqlAmount = S + E + L + elt + " " + tabName + "_ID " + F + R + OM + " " + tabName + " " + WH + E + R + E + " " + isAct + "='Y' AND " + amt + " " + optr + value;
                                parsedValue = VIS.MRole.getDefault().addAccessSQL(sqlAmount.toString(), "C_DimAmt",
                                    VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

                                optr = VIS.Query.prototype.IN;

                            }
                            if (parsedValue == null && value != null && (value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@"))) {
                                var Where = tableName + "." + columnName + optr + value;
                                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {

                                    Where = columnName + VIS.Query.prototype.IN + '(' + parsedValue + ')';
                                }

                                where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                                _query.addRestriction(Where);
                            }
                            else {
                                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                                    var Where = createDirectSql(parsedValue, parsedValue2, columnSQL, optr, false,false);
                                    where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                                    _query.addRestriction(Where);
                                }
                                else {


                                    _query.addRestriction(columnSQL, optr, parsedValue, infoName, infoDisplay);
                                }
                            }
                        }
                    }
                }
            }
            return _query;
        };

        function createDirectSql(code, code_to, column, operator, convertToString, isVirtualCol) {
            var sb = "";
            var isoDateRegx = /(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})/;
            if (typeof code == "string") {
                sb += " UPPER( ";
            }

            if (!isVirtualCol)
                sb += tableName + ".";
            sb += column;


            if (typeof code == "string") {
                sb += " ) ";
            }

            if (code == null || "NULL".equals(code.toString().toUpper()) || ("NullValue").toUpper().equals(code.toString().toUpper())) {
                if (operator.equals(VIS.Query.prototype.EQUAL))
                    sb += " IS NULL ";
                else
                    sb += " IS NOT NULL ";
            }
            else {
                sb += operator;
                if (VIS.Query.prototype.IN.equals(operator) || VIS.Query.prototype.NOT_IN.equals(operator)) {
                    sb += "(";
                }

                if (code instanceof Date || (code && (isoDateRegx.test(code.toString())))) {//  endsWith('Z') && this.code.toString().contains('T')))) {
                    sb += VIS.DB.to_date(tcode, false);
                }

                else if ("string" == typeof code) {
                    if (convertToString) {
                        sb += " UPPER( ";
                        sb += VIS.DB.to_string(code.toString());

                        sb += " ) ";
                    }
                    else {
                        sb += code.toString();
                    }
                }

                else
                    sb += code;

                //	Between
                if (VIS.Query.prototype.BETWEEN.equals(operator)) {
                    //	if (Code_to != null && InfoDisplay_to != null)
                    sb += " AND ";

                    if (code_to instanceof Date || (code_to && (isoDateRegx.test(code_to.toString())))) {//  endsWith('Z') ||  this.code.toString().contains('T')))) {
                        sb += VIS.DB.to_date(code_to, false);
                    }

                    else if (typeof (code_to) == "string") {
                        sb += " UPPER( ";
                        sb += VIS.DB.to_string(code_to.toString());
                        sb += " ) ";
                    }

                    else
                        sb += code_to;
                }
                else if (VIS.Query.prototype.IN.equals(operator) || VIS.Query.prototype.NOT_IN.equals(operator))
                    sb += ")";
            }

            return sb;

        };

        function parseValue(field, pp) {
            if (pp == null)
                return null;
            var dt = field.getDisplayType();
            var inStr = pp.toString();
            if (inStr == null || inStr.equals(VIS.Env.NULLString) || inStr == "" || inStr.toUpper() == "NULL")
                return null;
            try {
                //	Return Integer
                if (dt == VIS.DisplayType.Integer
                    || (VIS.DisplayType.IsID(dt) && field.getColumnName().endsWith("_ID"))) {
                    //i = int.Parse(inStr);
                    return parseInt(inStr);
                    // return i;
                }
                //	Return BigDecimal
                else if (VIS.DisplayType.IsNumeric(dt)) {
                    return parseFloat(inStr);       //DisplayType.GetNumberFormat(dt).GetFormatedValue(inStr);
                }
                //	Return Timestamp
                else if (VIS.DisplayType.IsDate(dt)) {
                    var time = "";
                    try {
                        return new Date(inStr);
                    }
                    catch (e) {
                        //log.Log(Level.WARNING, inStr + "(" + inStr.GetType().FullName + ")" + e);
                        time = "";//DisplayType.GetDateFormat(dt).Format(inStr);
                    }
                    try {
                        return Date.Parse(time);
                    }
                    catch (ee) {
                        return null;
                    }
                }
            }
            catch (ex) {
                //     log.Log(Level.WARNING, "Object=" + inStr, ex);
                var error = ex.message;
                if (error == null || error.length == 0)
                    error = ex.toString();
                var errMsg = "";
                errMsg += field.getColumnName() + " = " + inStr + " - " + error;
                //
                //if(pp != null && pp.ToString().Trim().StartsWith("adddays") || pp.ToString().Trim().StartsWith("adddays")
                VIS.ADialog.error("ValidationError", true, errMsg.toString());
                //MessageBox.Show("ValidationError " + errMsg.ToString());
                return null;
            }

            return inStr;
        };	//	parseValue

        function saveAdvanced() {
            // save all query lines temporarily

            setBusy(true);

            saveRowTemp();	//	unsaved 
            // get query
            query = getQueryAdvanced();
            if (query.getRestrictionCount() == 0) {
                setBusy(false);
                ch.close();
                return;
            }


            // get where clause
            var where = query.getWhereClause(true);
            // get query name entered by the user
            var name = VIS.Utility.encodeText(txtQryName.val());// vtxtQueryName.Text.Trim();
            if (name != null && name.trim().length == 0)
                name = null;
            else
                $self.needRefreshWindow = true;

            // get the selected value
            var value = drpSavedQry.val();// vcmbQueryA.SelectedValue;
            var s = "";// vcmbQueryA.Text;//silverlight comment
            //	Update Existing Query
            var qMessage = "";

            value = (value != null && value.toString() != "-1" && parseInt(value) > 0) ? value : 0;

            window.setTimeout(function () {

                if (value != 0 || name != null) {

                    if (MUserQuery.insertOrUpdate(value, name, where, AD_Tab_ID, AD_Table_ID, dsAdvanceData, $self.getID)) {
                        isSaveError = false;
                        //ShowMessage.Info("Updated", true, uq.GetName(), "");
                        qMessage = (value > 0 ? "Updated" : "Saved");
                        $self.saveQueryID = MUserQuery.id;
                    }
                    else {
                        if (MUserQuery.alreadyExist) {
                            VIS.ADialog.error("SearchExist");
                            setBusy(false);
                            isSaveError = true;
                            return;
                        }
                        else {
                            isSaveError = true;
                            //ShowMessage.Info("Updated", true, uq.GetName(), "");
                            qMessage = (value > 0 ? "UpdatedError" : "SaveError");
                        }

                    }
                }

                var result = false;
                if (getNoOfRecords(query, true) != 0) {
                    result = true;
                }
                setBusy(false);
                if (qMessage != "") {
                    //MessageBox.Show(qMessage);
                    VIS.ADialog.info("", true, VIS.Msg.getMsg(qMessage) + " " + name, null);
                }
                if (result) {
                    ch.close();
                }
            }, 10);
        };

        function setBusy(busy) {
            isBusy = busy;
            $busy.css("visibility", isBusy ? "visible" : "hidden");
            btnOk.prop("disabled", busy);
            btnCancel.prop("disabled", busy);
            btnRefresh.prop("disabled", busy);
        };

        this.getID = function (id) {
            this.saveQueryID = id;
        };

        this.getSavedID = function (id) {
            return this.saveQueryID;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();

            ch.setHeight(550);
            ch.setWidth(860);
            ch.setTitle(VIS.Msg.getMsg("Find"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {
                //$self.okBtnPressed = false;
                if ($self.onClose) $self.onClose();
                $self.dispose();
            };

            ch.show();
            ch.setContent($root);
            ch.hideButtons();
            setBusy(false);
            btnCancel.focus();
            //$root.focus();
            //$root.css('border', '1px solid white');


            //window.setTimeout(function () {
            //    $(document).on('keydown', function (e) {
            //        console.log('1');
            //        e.preventDefault();
            //        e.stopPropagation();
            //        return;
            //    });
            //}, 100);

            //  bindEvents();
        };

        this.getSavedQueryName = function () {
            if (txtQryName.val()) {
                return VIS.Utility.encodeText(txtQryName.val());
            }
            else {
                return "";
            }
        };

        this.getQuery = function () {
            var role = VIS.MRole.getDefault();
            if (role.getIsQueryMax(total)) {
                query = VIS.Query.prototype.getNoRecordQuery(tableName, false);
                total = 0;
                log.warning("Query - over max");
            }
            else
                log.info("Query=" + query);
            return query;
        };

        this.disposeComponent = function () {
            unBindEvents();
            btnOk = btnCancel = btnDelete = btnSave = btnRefresh = null;
            txtQryName = drpSavedQry = drpColumns = drpOp = drpDynamicOp = chkDynamic = txtYear = txtMonth = txtDay = null;
            ulQryList = divDynamic = divYear = divMonth = divDay = null;
            if ($root)
                $root.remove();
            $root = null;
            total = isLoadError = isSaveError = null;
            dsAdvanceData = null;
            log = null;
            this.created = this.days = 0, this.okPressed = this.okBtnPressed = null;
            control1 = control2 = ulListStaticHtml = null;
            query = null;
        };
    };

    Find.prototype.getOperatorsQuery = function (vnpObj, translate) {
        var html = "";
        var val = "";
        for (var p in vnpObj) {
            val = vnpObj[p];
            if (translate)
                val = VIS.Msg.getMsg(val);
            html += '<option value="' + p + '">' + val + '</option>';
        }
        return html;
    };

    Find.prototype.getIsUserColumn = function (columnName) {
        if (columnName.endsWith("atedBy") || columnName.equals("AD_User_ID"))
            return true;
        if (columnName.equals("SalesRep_ID"))
            return true;
        return false;
    };

    Find.prototype.getCurrentDays = function () {
        return this.days;
    };

    Find.prototype.getIsCreated = function () {
        return this.created;
    };

    Find.prototype.getIsOKPressed = function () {
        return this.okPressed && this.okBtnPressed;
    };

    Find.prototype.needRefresh = function () {
        return this.needRefreshWindow;
    };


    Find.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.Find = Find;

}(VIS, jQuery));