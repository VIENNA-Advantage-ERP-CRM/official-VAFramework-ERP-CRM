; (function (VIS, $) {
    function FilterPanel(width, $parentRoot) {
        this.selectionfields;
        this.curTabfields;
        var $root = $('<div class="main-fp-wrap">');
        this.curTab;

        this.inputWrapContainer = $('<div class="vis-fp-outerwrp"></div>');


        this.addRootToParent = function () {
            $root.append(this.inputWrapContainer);
            $parentRoot.append($root);
        };

        this.getRoot = function () {
            return $root;
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

    FilterPanel.prototype.setLayout = function () {
        var bodyDiv = $(' <div class="vis-fp-bodywrap"> </div>');

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

        var divStatic = $('<div class="vis-fp-static-ctrlwrp">')
        bodyDiv.append(divStatic.append(divSpan));


        //this.inputWrapContainer.append(divSpan.append(cusLogDiv));

        //<span>Invoice To Customer = Aleesha Stephen</span>
        //<i class="vis vis-mark"></i>

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
                    var that = this;

                    $.ajax({
                        type: "GET",
                        url: VIS.Application.contextUrl + "JsonData/GetRecordForFilter",
                        data: {
                            keyCol: field.getLookup().info.keyColumn, displayCol: field.getLookup().info.displayColSubQ, validationCode: field.getLookup().info.validationCode
                            , tableName: field.getLookup().info.tableName, AD_Referencevalue_ID: field.getAD_Reference_Value_ID(), pTableName: field.gridTab.getTableName(), pColumnName: field.getColumnName()
                        },
                        success: function (data) {
                            data = JSON.parse(data);
                            var key = data["keyCol"];
                            data = data["list"];
                            if (data && data.length > 0) {
                                var fields = $('<div class="vis-fp-lst-searchrcrds"></div>')
                                var wrapper = divStatic.find('[data-cid="' + key + '_' + that.curTab.getAD_Tab_ID() + '"]');
                                wrapper.append(fields);
                                for (var i = 0; i < data.length; i++) {
                                    var divinpuspanWrapper = $('<div class="vis-fp-inputspan">');
                                    divinpuspanWrapper.append('<input class="vis-fp-chboxInput vis-fp-inputvalueForUpdate" type="checkbox" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"><span data-id="' + data[i].ID + '">' + data[i].Name + '</span><span class="vis-fp-spanCount">' + data[i].Count + '</span>');
                                    fields.append(divinpuspanWrapper);
                                }
                            }

                        },
                        error: function () { }
                    });

                    //var btn2 = crt.getBtn(1);
                    //if (btn2) {
                    //    var $divInputGroupAppend1 = $('<div class="input-group-append">');
                    //    $divInputGroupAppend1.append(btn2);
                    //    inputWrap.append($divInputGroupAppend1);
                    //}
                }
            }
            bodyDiv.on("click", function (e) {
                $target = $(e.target);
                if ($target.hasClass('vis-fp-inputvalueForUpdate')) {
                    var whereClause = '';
                    var listOfDiv = bodyDiv.find('.vis-fp-inputgroupSeprtr');
                    if (listOfDiv && listOfDiv.length > 0) {
                        for (var i = 0; i < listOfDiv.length; i++) {

                            var selectedDiv = $(listOfDiv[i]);
                            var listOfSelectedIDs = selectedDiv.find('.vis-fp-inputvalueForUpdate');
                            var col = selectedDiv.data('columnname');

                            if (listOfSelectedIDs && listOfSelectedIDs.length > 0) {
                                if (i == 0)
                                    whereClause += '(';
                                else
                                    whereClause += ') AND (';
                                var appendOR = false;
                                for (var j = 0; j < listOfSelectedIDs.length; j++) {
                                    var inputType = $(listOfSelectedIDs[j]);
                                    if (inputType.is('input') && !inputType.is(':checked')) {
                                        continue;
                                    }

                                    if (appendOR) {
                                        whereClause += ' OR ' + col + ' =' + inputType.data('id');
                                    }
                                    else {
                                        whereClause += col + '=' + inputType.data('id');
                                        appendOR = true;
                                    }
                                }
                            }

                        }
                        if (whereClause.length > 2)
                            whereClause += ')';
                        else
                            whereClause = "";
                    }
                    var query = new VIS.Query(that.curTab.getTableName(), true);
                    query.addRestriction(whereClause);
                    that.curTab.setQuery(query);
                    that.curGC.query(0, 0, null);
                }

            });
        }
    };

    FilterPanel.prototype.vetoablechange = function (evt) {
        //data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID()
        var wrapper = this.inputWrapContainer.find('[data-cid="' + evt.propertyName + '_' + this.curTab.getAD_Tab_ID() + '"]');
        //wrapper.append('<span >' + evt.newValue + '</span>');
        var field = $.grep(this.selectionfields, function (field, index) {
            if (field.getColumnName() == evt.propertyName)
                return field;
        });
        var displayVal = field[0].lookup.getDisplay(evt.newValue);
        var html = '  <div class="vis-fp-currntrcrdswrap"><div class="vis-fp-currntrcrds"><span >' + displayVal
            + '</span><i data-id="' + evt.newValue + '" data-keyval="' + evt.propertyName + "_" + evt.newValue + '" class="vis vis-mark vis-fp-inputvalueForUpdate"></i></div></div>';
        wrapper.append(html);
        bodyDiv.trigger('click');
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