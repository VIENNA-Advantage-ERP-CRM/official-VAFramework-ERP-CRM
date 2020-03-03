; (function (VIS, $) {


    var tmpfp = document.querySelector('#vis-ad-fptmp').content;// $("#vis-ad-windowtmp");

    function FilterPanel(width, $parentRoot) {

        var clone = document.importNode(tmpfp, true);

        var $outerwrap = $(clone.querySelector(".vis-fp-outerwrp"));
        var bodyDiv = $outerwrap.find(".vis-fp-bodywrap");
        var headerDiv = $outerwrap.find(".vis-fp-header");
        var btnclose = headerDiv.find("vis vis-mark");
       
        var divStatic = $outerwrap.find(".vis-fp-static-ctrlwrp");
        var spnViewAll = divStatic.find(".vis-fp-static-ctrlwrp");


        //Translation 
        headerDiv.find('h4').text(VIS.Msg.getMsg("Filter"));
        spnViewAll.text(VIS.Msg.getMsg("ViewAll"));
       
       

        this.selectionfields = null;
        this.curTabfields = null;
        this.curTab = null;
        this.listOfFilterQueries = [];
        this.ctrlObjects = {};


        this.getRoot = function () {
            return $outerwrap;
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
                    var inputWrap = $('<div class="vis-fp-inputgroupseprtr" data-ColumnName="' + crt.getName() + '" data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID() + '"></div>');
                    divStatic.append(inputWrap.append(label.getControl()).append(crt.getControl()));
                    if (crt.getBtnCount() > 1) {
                        var btn = crt.getBtn(0);
                        if (btn) {
                            var $divInputGroupAppend = $('<div>');
                            $divInputGroupAppend.append(btn);
                            inputWrap.append(btn);
                        }
                    }
                    this.getFilterOption(field);
                }
            }
        };

        //CurrentColumn is column whose filter is just clicked...
        function QueryToUpdateSuggestion(currentColumnName) {
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
                        //updateSuggestions(field, whereClause);
                        this.getFilterOption(field,whereClause);
                    }

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
                            if (inputType[0].type == 'checkbox' && !inputType.is(":checked"))
                                continue;

                            if (whereClause !='') {
                                whereClause += " OR " + col + " ='" + inputType.data('id') + "'";
                            }
                            else {
                                whereClause += "(" + col + "='" + inputType.data('id') + "'";
                            }
                        }
                        if (whereClause !='') {
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
                                    context.listOfFilterQueries.splice(k,1);
                            }
                        }
                        if (!found && whereClause !='')
                            context.listOfFilterQueries.push({ 'columnName': col, 'whereClause': whereClause });
                    }
                }
            }
            return finalWhereClause;
        };

        this.fireValChanged = function(colName) {
           // if (ignoreTarget || $target.hasClass('vis-fp-inputvalueforupdate')) {
            this.refreshAll(colName,prepareWhereClause(this));
        };

        this.vetoablechange = function (evt) {
            //data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID()
            var wrapper = $outerwrap.find('[data-cid="' + evt.propertyName + '_' + this.curTab.getAD_Tab_ID() + '"]');
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

        this.setFilterOptions = function (data,key) {
            var fields;
            var wrapper = divStatic.find('[data-cid="' + key + '_' + this.curTab.getAD_Tab_ID() + '"]');
            if (wrapper && wrapper.length > 0) {
                fields = wrapper.find('.vis-fp-lst-searchrcrds');
                var inputs = fields.find('input');
                if (inputs && inputs.length > 0) {
                    for (var a = 0; a < inputs.length; a++) {
                        if (!$(inputs[a]).is(':checked')) {
                            $(inputs[a]).parent().remove();
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
                divinpuspanWrapper.append('<input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + key + '" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"><span data-id="' + data[i].ID + '">' + data[i].Name + '</span><span class="vis-fp-spanCount">' + data[i].Count + '</span>');
                fields.append(divinpuspanWrapper);
            }
        };

        this.disposeComponent = function () {
            $outerwrap.remove();
            this.listOfFilterQueries = [];
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

        btnclose.on("click", function (e) {
            $parentRoot.width(0);
        });

        bodyDiv.on("click", function (e) {
            $target = $(e.target);
            if ($target.is('input') && $target.hasClass('vis-fp-chboxInput')) {
                var currentColumnName = $target.data('column');
                that.fireValChanged(currentColumnName);
            }
        });

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

FilterPanel.prototype.refreshAll = function (colName,finalWhereClause) {
    var query = new VIS.Query(this.curTab.getTableName(), true);
    query.addRestriction(finalWhereClause);
    this.curTab.setQuery(query);
    this.curGC.query(0, 0, null);
    this.refreshFilterOptions(colName);
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