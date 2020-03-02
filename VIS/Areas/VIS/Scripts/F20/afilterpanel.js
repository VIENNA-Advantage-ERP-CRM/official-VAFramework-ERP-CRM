; (function (VIS, $) {
    function FilterPanel(width, $parentRoot) {
        this.selectionfields;
        this.curTabfields;
        var $root = $('<div class="main-fp-wrap">');
        this.curTab;
        var bodyDiv, divStatic;
        var that = this;
        this.inputWrapContainer = $('<div class="vis-fp-outerwrp"></div>');
        var listOfFilterQueries = [];

        this.addRootToParent = function () {
            $root.append(this.inputWrapContainer);
            $parentRoot.append($root);
        };

        this.getRoot = function () {
            return $root;
        };

        this.setLayout = function () {
            bodyDiv = $(' <div class="vis-fp-bodywrap"> </div>');

            //<div class="vis-fp-custcolumns" id="accordion">'
            //    + '<div class="card"><div class="card-header"><span>' + VIS.Msg.getMsg('Custom Condition') + '</span>'
            //    + '       <a class="card-link" data-toggle="collapse" href="#collapseOne"><i class="vis vis-arrow-up"></i>'
            //    + '</a></div><div id="collapseOne" class="collapse show" data-parent="#accordion"><div class="card-body">'
            //    + 'Lorem ipsum..</div></div></div>
            //var cardBodyDiv = bodyDiv.find('.card-body');

            var header = $('<div class="vis-fp-header"><h4> ' + VIS.Msg.getMsg("Filter") + '</h4><i class="vis vis-mark"></i></div>');
            this.inputWrapContainer.append(header);
            this.inputWrapContainer.append(bodyDiv);
            var that = this;

            //var cusLogDiv = $('<div class="vis-fp-custcoltag">');

            var divSpan = $(' <div class="vis-fp-viwall">');
            var $spanViewAll = $('<span>' + VIS.Msg.getMsg("ViewAll") + '</span>');
            divSpan.append($spanViewAll);

            divStatic = $('<div class="vis-fp-static-ctrlwrp">')
            bodyDiv.append(divStatic.append(divSpan));

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
                    label = VIS.VControlFactory.getLabel(field); //get label
                    crt.addVetoableChangeListener(this);
                    //field.setPropertyChangeListener(crt);
                    var inputWrap = $('<div class="vis-fp-inputgroupSeprtr" data-ColumnName="' + crt.getName() + '" data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID() + '"></div>');
                    divStatic.append(inputWrap.append(label.getControl()).append(crt.getControl()));
                    if (crt.getBtnCount() > 1) {
                        var btn = crt.getBtn(0);
                        if (btn) {
                            var $divInputGroupAppend = $('<div>');
                            $divInputGroupAppend.append(btn);
                            inputWrap.append(btn);
                        }
                    }
                    updateSuggestions(field)

                    //var btn2 = crt.getBtn(1);
                    //if (btn2) {
                    //    var $divInputGroupAppend1 = $('<div class="input-group-append">');
                    //    $divInputGroupAppend1.append(btn2);
                    //    inputWrap.append($divInputGroupAppend1);
                    //}
                }
                bodyDiv.on("click", function (e) {
                    $target = $(e.target);
                    if ($target.is('input') && $target.hasClass('vis-fp-chboxInput')) {
                        var currentColumnName = $target.data('column');
                        changeVal(false, $target, currentColumnName);
                    }
                });
            }
        };

        function updateSuggestions(field, whereClause) {
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

                var finalWhere = that.curTab.getWhereClause();
                if (whereClause)
                    finalWhere += " AND " + whereClause;

                $.ajax({
                    type: "GET",
                    url: VIS.Application.contextUrl + "JsonData/GetRecordForFilter",
                    data: {
                        keyCol: keyCol, displayCol: displayCol, validationCode: validationCode
                        , tableName: lookupTableName, AD_Referencevalue_ID: field.getAD_Reference_Value_ID(), pTableName: that.curTab.getTableName(), pColumnName: field.getColumnName(), whereClause: finalWhere
                    },
                    success: function (data) {
                        data = JSON.parse(data);
                        var key = data["keyCol"];
                        data = data["list"];
                        if (data && data.length > 0) {
                            var fields;
                            var wrapper = divStatic.find('[data-cid="' + key + '_' + that.curTab.getAD_Tab_ID() + '"]');
                            if (wrapper && wrapper.length > 0) {
                                fields = wrapper.find('.vis-fp-lst-searchrcrds');
                                var inputs = wrapper.find('input');
                                if (inputs && inputs.length > 0) {
                                    for (var a = 0; a < inputs.length; a++) {
                                        if (!$(inputs[a]).is(':checked')) {
                                            $(inputs[a]).parents('.vis-fp-inputspan').remove();
                                        }
                                    }
                                }

                            }
                            if (!fields || fields.length == 0) {
                                fields = $('<div class="vis-fp-lst-searchrcrds"></div>');
                                wrapper.append(fields);
                            }

                            for (var i = 0; i < data.length; i++) {
                                var divinpuspanWrapper = $('<div class="vis-fp-inputspan">');
                                divinpuspanWrapper.append('<input class="vis-fp-chboxInput vis-fp-inputvalueForUpdate" type="checkbox" data-column="'+key+'" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"><span data-id="' + data[i].ID + '">' + data[i].Name + '</span><span class="vis-fp-spanCount">' + data[i].Count + '</span>');
                                fields.append(divinpuspanWrapper);
                            }
                        }

                    },
                    error: function () { }
                });
            }
        };


        //CurrentColumn is column whose filter is just clicked...
        function getQueryToUpdateSuggestion(currentColumnName) {
            for (var i = 0; i < that.selectionfields.length; i++) {
                if (that.selectionfields[i].getShowFilterOption()) {
                    var field = that.selectionfields[i];
                    if (field.getColumnName() != currentColumnName) {
                        var whereClause = '';
                        for (var j = 0; j < listOfFilterQueries.length; j++) {
                            var query = listOfFilterQueries[j];
                            if (query.columnName != field.getColumnName()) {
                                whereClause += query.whereClause;
                            }
                        }
                        updateSuggestions(field, whereClause);
                    }

                }
            }
        };

        function changeVal($target, ignoreTarget, currentColumnName) {
            if (ignoreTarget || $target.hasClass('vis-fp-inputvalueForUpdate')) {

                var finalWhereClause = '';
                var listOfDiv = bodyDiv.find('.vis-fp-inputgroupSeprtr');
                if (listOfDiv && listOfDiv.length > 0) {
                    for (var i = 0; i < listOfDiv.length; i++) {

                        var selectedDiv = $(listOfDiv[i]);
                        var listOfSelectedIDs = selectedDiv.find('.vis-fp-inputvalueForUpdate');
                        var col = selectedDiv.data('columnname');

                        if (listOfSelectedIDs && listOfSelectedIDs.length > 0) {
                            //if (i == 0)
                            //    whereClause += '(';
                            //else
                            //    whereClause += ') AND (';

                            if (finalWhereClause.length > 2) {
                                finalWhereClause += ' AND ';
                            }
                            var whereClause = '';
                            var appendOR = false;
                            for (var j = 0; j < listOfSelectedIDs.length; j++) {
                                var inputType = $(listOfSelectedIDs[j]);
                                if (inputType.is('input') && !inputType.is(':checked')) {
                                    if (j == listOfSelectedIDs.length - 1 && whereClause.length > 2) {
                                        whereClause += ")";
                                        if (listOfFilterQueries.length > 0) {
                                            for (var k = 0; k < listOfFilterQueries.length; k++) {
                                                if (listOfFilterQueries[k].columnName == col) {
                                                    listOfFilterQueries[k].columnName = whereClause;
                                                }
                                            }
                                        }
                                        else {
                                            listOfFilterQueries.push({ 'columnName': col, 'whereClause': whereClause });
                                        }
                                        finalWhereClause += whereClause;
                                    }
                                    continue;
                                }

                                if (appendOR) {
                                    whereClause += ' OR ' + col + ' =' + inputType.data('id');
                                }
                                else {
                                    whereClause += "(" + col + '=' + inputType.data('id');
                                    appendOR = true;
                                }

                                if (j == listOfSelectedIDs.length - 1) {
                                    whereClause += ")";
                                    if (listOfFilterQueries.length > 0) {
                                        for (var k = 0; k < listOfFilterQueries.length; k++) {
                                            if (listOfFilterQueries[i].columnName == col) {
                                                listOfFilterQueries[i].columnName = whereClause;
                                            }
                                        }
                                    }
                                    else {
                                        listOfFilterQueries.push({ 'columnName': col, 'whereClause': whereClause });
                                    }
                                    finalWhereClause += whereClause;
                                }
                            }
                        }

                    }
                    if (finalWhereClause.length > 2)
                        finalWhereClause += ')';
                    else
                        finalWhereClause = "";
                }
                var query = new VIS.Query(that.curTab.getTableName(), true);
                query.addRestriction(finalWhereClause);
                that.curTab.setQuery(query);
                that.curGC.query(0, 0, null);

                getQueryToUpdateSuggestion(currentColumnName);
            }
        }



        this.vetoablechange = function (evt) {
            //data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID()
            var wrapper = this.inputWrapContainer.find('[data-cid="' + evt.propertyName + '_' + this.curTab.getAD_Tab_ID() + '"]');
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

            var spann = $('<span data-id="' + evt.newValue + '" class="vis-fp-inputvalueForUpdate" >' + displayVal + '</span>');
            var iconCross = $('<i data-id="' + evt.newValue + '" data-keyval="' + evt.propertyName + "_" + evt.newValue + '" class="vis vis-mark vis-fp-inputvalueForUpdate"></i></div></div>');
            wrapper.append($('<div class="vis-fp-currntrcrdswrap">').append($('<div class="vis-fp-currntrcrds">').append(spann).append(iconCross)));
            changeVal(false, spann, evt.propertyName);
            iconCross.on("click", function () {
                ($(this).parents('.vis-fp-currntrcrdswrap')[0]).remove();
                changeVal(true, spann, evt.propertyName);
            });
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
        this.addRootToParent();
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

    VIS.FilterPanel = FilterPanel;
}(VIS, jQuery));