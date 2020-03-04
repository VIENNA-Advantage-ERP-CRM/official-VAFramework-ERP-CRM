; (function (VIS, $) {

    var tmpfp = document.querySelector('#vis-ad-fptmp').content;// $("#vis-ad-windowtmp");

    function FilterPanel(width, $parentRoot) {

        var clone = document.importNode(tmpfp, true);

        //var $outerwrap = $(clone.querySelector("vis-fp-bodycontent);
        var bodyDiv = $(clone.querySelector(".vis-fp-bodycontent"));
       // var headerDiv = $outerwrap.find(".vis-fp-header");
        //var btnclose = headerDiv.find(".vis-mark");

        var divStatic = bodyDiv.find(".vis-fp-static-ctrlwrp");
        var spnViewAll = divStatic.find(".vis-fp-static-ctrlwrp");

        //Translation 
        //headerDiv.find('h4').text(VIS.Msg.getMsg("Filter"));
        spnViewAll.text(VIS.Msg.getMsg("ViewAll"));

        this.selectionfields = null;
        this.curTabfields = null;
        this.curTab = null;
        this.listOfFilterQueries = [];
        this.ctrlObjects = {};

        this.getRoot = function () {
            return bodyDiv;
        };

        this.setLayout = function () {
            if (this.selectionfields && this.selectionfields.length > 0) {
                for (var i = 0; i < this.selectionfields.length; i++) {
                    var crt;
                    var label;
                    var field = this.selectionfields[i];
                    if (field.getIsKey()) {
                        crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                            field.getColumnName());
                    }
                    else {
                        crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                    }
                    this.ctrlObjects[field.getColumnName()] = crt;

                    label = VIS.VControlFactory.getLabel(field); //get label
                    crt.addVetoableChangeListener(this);
                    //field.setPropertyChangeListener(crt);
                    var inputWrapGroup = $('<div class="vis-fp-inputgroupseprtr" data-ColumnName="' + crt.getName() + '" data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID() + '"></div>');
                    var inputWrap = $('<div class="vis-control-wrap">');
                    inputWrap.append(crt.getControl()).append(label.getControl());

                    var grp = $('<div class="input-group vis-input-wrap">');
                    grp.append(inputWrap);
                    if (crt.getBtnCount() > 1) {
                        var btn = crt.getBtn(0);
                        if (btn) {
                            var $divInputGroupAppend = $('<div class="input-group-append">');
                            $divInputGroupAppend.append(btn);
                            grp.append($divInputGroupAppend);
                        }
                    }
                    inputWrapGroup.append(grp);
                    divStatic.append(inputWrapGroup);
                    this.getFilterOption(field);
                }
            }
        };
      
        function prepareWhereClause(context) {
            var finalWhereClause = '';
            var listOfDiv = bodyDiv.find('.vis-fp-inputgroupseprtr');

            if (listOfDiv && listOfDiv.length > 0) { //allcontrols
                for (var i = 0; i < listOfDiv.length; i++) {

                    var selectedDiv = $(listOfDiv[i]);
                    var listOfSelectedIDs = selectedDiv.find('.vis-fp-inputvalueforupdate');
                    var col = selectedDiv.data('columnname');

                    if (listOfSelectedIDs && listOfSelectedIDs.length > 0) {
                        if (finalWhereClause.length > 2) {
                            finalWhereClause += ' AND ';      //Append and in main where
                        }

                        var whereClause = '';
                        for (var j = 0; j < listOfSelectedIDs.length; j++) {
                            var inputType = $(listOfSelectedIDs[j]);
                            if (inputType[0].type == 'checkbox' && !inputType.is(":checked")) {
                                continue;
                            }
                            var pasedVal = context.parseWhereCondition(col, VIS.Query.prototype.EQUAL, inputType.data('id'), null);

                            if (whereClause != '') {
                                whereClause += " OR " + pasedVal;
                            }
                            else {
                                whereClause += "(" + pasedVal;
                            }
                        }
                        if (whereClause != '') {
                            whereClause += ")";
                            finalWhereClause += whereClause;
                        }

                        var found = false;
                        for (var k = 0; k < context.listOfFilterQueries.length; k++) {
                            if (context.listOfFilterQueries[k].columnName == col) {
                                found = true;
                                if (whereClause != '')
                                    context.listOfFilterQueries[k].whereClause = whereClause;
                                else
                                    context.listOfFilterQueries.splice(k, 1);
                            }
                        }
                        if (!found && whereClause != '')
                            context.listOfFilterQueries.push({ 'columnName': col, 'whereClause': whereClause });
                    }
                    else { //delete consiftion of exist
                        for (var k = 0; k < context.listOfFilterQueries.length; k++) {
                            if (context.listOfFilterQueries[k].columnName == col) {
                                context.listOfFilterQueries.splice(k, 1);
                            }
                        }
                    }
                }
            }
            return finalWhereClause;
        };

        this.fireValChanged = function (colName) {
            // if (ignoreTarget || $target.hasClass('vis-fp-inputvalueforupdate')) {
            this.refreshAll(colName, prepareWhereClause(this));
        };

        this.vetoablechange = function (evt) {
            //data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID()
            var wrapper = bodyDiv.find('[data-cid="' + evt.propertyName + '_' + this.curTab.getAD_Tab_ID() + '"]');
            //wrapper.append('<span >' + evt.newValue + '</span>');
            var field = $.grep(this.selectionfields, function (field, index) {
                if (field.getColumnName() == evt.propertyName)
                    return field;
            });
            var displayVal;
            if (field[0].lookup)
                displayVal = field[0].lookup.getDisplay(evt.newValue);
            else
                displayVal = evt.newValue;
            if (this.ctrlObjects[evt.propertyName])
                this.ctrlObjects[evt.propertyName].setValue(evt.newValue);

            var spann = $('<span data-id="' + evt.newValue + '" class="vis-fp-inputvalueforupdate" >' + displayVal + '</span>');
            var iconCross = $('<i data-id="' + evt.newValue + '" data-keyval="' + evt.propertyName + "_" + evt.newValue + '" class="vis vis-mark"></i></div></div>');
            wrapper.append($('<div class="vis-fp-currntrcrdswrap">').append($('<div class="vis-fp-currntrcrds">').append(spann).append(iconCross)));

            if (this.ctrlObjects[evt.propertyName])
                this.ctrlObjects[evt.propertyName].setValue(null);
            this.fireValChanged(evt.propertyName);

        };

        this.setFilterOptions = function (data, key) {
            var fields;
            var selIds = [];
            var selItems = [];
            var wrapper = divStatic.find('[data-cid="' + key + '_' + this.curTab.getAD_Tab_ID() + '"]');
            if (wrapper && wrapper.length > 0) {
                fields = wrapper.find('.vis-fp-lst-searchrcrds');
                var inputs = fields.find('input');
                if (inputs && inputs.length > 0) {
                    for (var a = 0; a < inputs.length; a++) {
                        var ctr = $(inputs[a]);
                        if (ctr.is(':checked')) {
                            selIds.push(ctr.data("id"));
                            ctr.parent().find('.vis-fp-spanCount').text("0");
                            selItems.push(ctr.parent());
                        }
                        //else
                         ctr.parent().remove();
                    }
                }
            }
            if (!fields || fields.length == 0) {
                fields = $('<div class="vis-fp-lst-searchrcrds"></div>');
                wrapper.append(fields);
            }

            
            for (var i = 0; i < data.length; i++) {
                var htm = [];
                var index = selIds.indexOf(parseInt(data[i].ID));

                if (index > -1) {
                    selItems[index].find('.vis-fp-spanCount').text(data[i].Count);
                    fields.append(selItems[index]);
                    selItems.splice(index, 1);
                    selIds.splice(index, 1);
                    continue;
                }
                htm.push('<div class="vis-fp-inputspan">');
                htm.push('<div class="vis-fp-istagwrap"><input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + key + '" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"');
                htm.push('><span data-id="' + data[i].ID + '">' + data[i].Name + '</span> </div><span class="vis-fp-spanCount">(' + data[i].Count + ')</span>');
                htm.push('</div>');
                fields.append(htm.join(''));
            }
            for (i = 0; i < selItems.length; i++) {
                fields.append(selItems[i]);
            }

            selItems = [];
            selIds = [];
        };

        

        var that = this;
        //Events ... 
        divStatic.on("click", "i", function (e) {
            var tgt = $(this);
            if (tgt.hasClass("vis-mark")) {

                tgt.parent().parent().remove();
                that.fireValChanged(tgt.data('keyval'));// evt.propertyName);
            }
        });

        

        bodyDiv.on("click", function (e) {
            $target = $(e.target);
            if ($target.is('input') && $target.hasClass('vis-fp-chboxInput')) {
                var currentColumnName = $target.data('column');
                that.fireValChanged(currentColumnName);
            }
        });

        this.disposeComponent = function () {
            bodyDiv.remove();
            this.listOfFilterQueries = [];
            that = null;
        };
        //Dispose and remove votable events too...
    };

    FilterPanel.prototype.init = function (curTab, curGC) {
        this.curGC = curGC;
        this.curTab = curTab;
        this.curTabfields = curTab.getFields();
        this.selectionfields = $.grep(this.curTabfields, function (field, index) {
            if (field.getIsSelectionColumn())
                return field;
        });
        this.selectionfields.sort(function (a, b) { return a.getSelectionSeqNo() - b.getSelectionSeqNo() });
        this.getFixedColumns();
        this.setLayout();
        //this.addRootToParent();
    };

    FilterPanel.prototype.getFixedColumns = function () {
        if (!this.selectionfields || this.selectionfields.length == 0) {
            this.selectionfields = $.grep(this.curTabfields, function (field, index) {
                if (field.getColumnName() == "Name" || field.getColumnName() == "Description"
                    || field.getColumnName() == "Value" || field.getColumnName() == "DocumentNo") {
                    return field;
                }
            });
        }
    };

    FilterPanel.prototype.getFilterOption = function (field, whereClause) {
        if (field && field.getShowFilterOption()) {

            var keyCol;
            var displayCol;
            var validationCode;
            var lookupTableName;
            if (field.getLookup()) {
                keyCol = field.getLookup().info.keyColumn;
                displayCol = field.getLookup().info.displayColSubQ;
                validationCode = field.getLookup().info.validationCode;
                lookupTableName = field.getLookup().info.tableName;
            }
            else {
                keyCol = "";
                displayCol = "";
                validationCode = "";
                lookupTableName = "";
            }

            var finalWhere = this.curTab.getWhereClause();
            if (whereClause)
                finalWhere += " AND " + whereClause;

            var data = {
                keyCol: keyCol, displayCol: displayCol, validationCode: validationCode
                , tableName: lookupTableName, AD_Referencevalue_ID: field.getAD_Reference_Value_ID(), pTableName: this.curTab.getTableName(),
                pColumnName: field.getColumnName(), whereClause: finalWhere
            };
            var tht = this;

            filterContext.getFilters(data).then(function (data) {

                var key = data["keyCol"];
                data = data["list"];
                if (data && data.length > 0) {
                    tht.setFilterOptions(data, key);
                }
                tht = null;
            });
        }
    };

    FilterPanel.prototype.refreshFilterOptions = function (colName) {
        for (var i = 0; i < this.selectionfields.length; i++) {
            if (this.selectionfields[i].getShowFilterOption()) {
                var field = this.selectionfields[i];
                if (field.getColumnName() != colName) {
                    var whereClause = '';
                    for (var j = 0; j < this.listOfFilterQueries.length; j++) {
                        var query = this.listOfFilterQueries[j];
                        if (query.columnName != field.getColumnName()) {
                            whereClause += query.whereClause;
                        }
                    }
                    this.getFilterOption(field, whereClause);
                }
            }
        }
    };

    FilterPanel.prototype.refreshAll = function (colName, finalWhereClause) {
        var query = new VIS.Query(this.curTab.getTableName(), true);
        query.addRestriction(finalWhereClause);
        this.curTab.setQuery(query);
        this.curGC.query(0, 0, null);
        this.refreshFilterOptions(colName);
    };

    FilterPanel.prototype.getTargetMField = function (columnName) {
        // if no column name, then return null
        if (columnName == null || columnName.length == 0)
            return null;
        // else find field for the given column
        for (var c = 0; c < this.curTabfields.length; c++) {
            var field = this.curTabfields[c];
            if (columnName.equals(field.getColumnName()))
                return field;
        }
        return null;
    };

    FilterPanel.prototype.parseValue = function (field, pp) {
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
    };	//	pa

    FilterPanel.prototype.createDirectSql = function (code, code_to, column, operator, convertToString) {
        var sb = "";
        var isoDateRegx = /(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})/;
        if (typeof code == "string") {
            sb += " UPPER( ";
        }

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
                sb += VIS.DB.to_date(code, false);
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

    FilterPanel.prototype.parseWhereCondition = function (columnName, optr, value, value2) {
        //	Column
        var field = this.getTargetMField(columnName);
        var columnSQL = field.getColumnSQL(); //

        var whereCondition = '';

        var parsedValue = null;
        if (value != null && value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@")) {
            ;
        }
        else {
            parsedValue = this.parseValue(field, value);
        }

        if (value == null || value.toString().length < 1) {
            if (VIS.Query.prototype.BETWEEN.equals(optr))
                return whereCondition;
            parsedValue = VIS.Env.NULLString;

        }
        if (field.getIsVirtualColumn()) {
            columnSQL = field.vo.ColumnSQL;
            columnName = field.vo.ColumnSQL;
            if (VIS.Query.prototype.BETWEEN.equals(optr)) {

                if (value2 == null || value2.toString().trim().length < 1)
                    return whereCondition;
                var parsedValue2 = this.parseValue(field, value2);
                if (parsedValue2 == null)
                    return whereCondition;
                whereCondition = this.createDirectSql(parsedValue, parsedValue2, columnName, optr, true);
            }
            else {
                whereCondition = this.createDirectSql(parsedValue, parsedValue2, columnName, optr, true);
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

            if (field.getDisplayType() == VIS.DisplayType.DateTime && VIS.Query.prototype.EQUAL.equals(optr) && parsedValue) {

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
                value2 = parsedValue2;
                if (value2 == null || value2.toString().trim().length < 1)
                    return whereCondition;
                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    var sqlAmount = S + E + L + elt + " " + tabName + "_ID " + F + R + OM + " " + tabName + " " + WH + E + R + E + " " + isAct + "='Y' AND " + amt + " " + optr + value + " AND " + value2;
                    parsedValue = VIS.MRole.getDefault().addAccessSQL(sqlAmount.toString(), "C_DimAmt",
                        VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);
                    optr = VIS.Query.prototype.IN;
                }
                whereCondition = this.createDirectSql(parsedValue, parsedValue2, columnSQL, optr, false);
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
                    whereCondition = columnName + optr + value;
                    if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {

                        whereCondition = columnName + VIS.Query.prototype.IN + '(' + parsedValue + ')';
                    }
                }
                else {
                    whereCondition = this.createDirectSql(parsedValue, parsedValue2, columnSQL, optr, false);

                }
            }
        }
        return whereCondition;
    };

    FilterPanel.prototype.dispose = function () {
        this.disposeComponent();
        this.curGC = this.curTab = this.curTabfields = this.selectionfields = null;
    };

    var filterContext = {

        getFilters: function (data) {

            return new Promise(function (resolve, reject) {
                var result = null;

                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GetRecordForFilter",
                    type: "POST",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(data)
                }).done(function (json) {
                    result = json;
                    result = JSON.parse(result);
                    resolve(result);
                    //return result;
                });




                //$.ajax({
                //    type: "GET",
                //    url: VIS.Application.contextUrl + "JsonData/GetRecordForFilter",
                //    data: {
                //        keyCol: keyCol, displayCol: displayCol, validationCode: validationCode
                //        , tableName: lookupTableName, AD_Referencevalue_ID: field.getAD_Reference_Value_ID(), pTableName: that.curTab.getTableName(), pColumnName: field.getColumnName(), whereClause: finalWhere
                //    },
                //    success: function (data) {
                //        data = JSON.parse(data);
                //        var key = data["keyCol"];
                //        data = data["list"];
                //        if (data && data.length > 0) {
                //            var fields;
                //            var wrapper = divStatic.find('[data-cid="' + key + '_' + that.curTab.getAD_Tab_ID() + '"]');
                //            if (wrapper && wrapper.length > 0) {
                //                fields = wrapper.find('.vis-fp-lst-searchrcrds');
                //                var inputs = wrapper.find('input');
                //                if (inputs && inputs.length > 0) {
                //                    for (var a = 0; a < inputs.length; a++) {
                //                        if (!$(inputs[a]).is(':checked')) {
                //                            $(inputs[a]).parents('.vis-fp-inputspan').remove();
                //                        }
                //                    }
                //                }

                //            }
                //            if (!fields || fields.length == 0) {
                //                fields = $('<div class="vis-fp-lst-searchrcrds"></div>');
                //                wrapper.append(fields);
                //            }

                //            for (var i = 0; i < data.length; i++) {
                //                var divinpuspanWrapper = $('<div class="vis-fp-inputspan">');
                //                divinpuspanWrapper.append('<input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + key + '" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"><span data-id="' + data[i].ID + '">' + data[i].Name + '</span><span class="vis-fp-spanCount">' + data[i].Count + '</span>');
                //                fields.append(divinpuspanWrapper);
                //            }
                //        }

                //    },
                //    error: function () { }
                //});

            })
        },
    };

    VIS.FilterPanel = FilterPanel;
}(VIS, jQuery));