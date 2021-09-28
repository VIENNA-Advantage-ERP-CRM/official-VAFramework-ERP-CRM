; (function (VIS, $) {

    // var tmpfp = document.querySelector('#vis-ad-fptmp').content;// $("#vis-ad-windowtmp");

    function getTemplate(winNo) {
        var str =
            ' <div class="vis-fp-bodycontent vis-formouterwrpdiv">                                                 ' +
            '     <div class="vis-fp-viwall" >                                                  ' +
            '         <span>' + VIS.Msg.getMsg("ViewMore") + '</span>                               ' +
            '     </div>                                                                       ' +
            '  <div class="vis-fp-datawrap"> ' +
            '     <div class="vis-fp-static-ctrlwrp">                                          ' +
            ' <div class="vis-fp-static-ctrlinnerwrp"></div>    ' +
            '     </div>                                                                       ' +
            '     <div class="vis-fp-custcolumns" id="accordion_' + winNo + '"">                ' +
            '         <div class="card">                                                       ' +
            '             <div class="card-header" style="cursor:pointer" data-toggle="collapse" href="#collapseOne_' + winNo + '">                                            ' +
            '                 <span>' + VIS.Msg.getMsg("CustomCondition") + '</span>                                  ' +
            '                 <a class="card-link" > ' +
            '                     <i class="vis vis-arrow-up"></i>                             ' +
            '                 </a>                                                             ' +
            '             </div>                                                               ' +
            '             <div id="collapseOne_' + winNo + '"" class="collapse" data-parent="#accordion_' + winNo + '" >' +
            '                 <div class="card-body">                                          ' +
            '                     <div class="input-group vis-input-wrap">                     ' +
            '                         <div class="vis-control-wrap">                           ' +
            '                             <select class="vis-fp-cols">                         ' +
            '                             </select>                                            ' +
            '                             <label class="vis-fp-lblcols">' + VIS.Msg.getMsg("Column") + '</label>         ' +
            '                         </div>                                                   ' +
            '                     </div>                                                       ' +
            '                     <div class="input-group vis-input-wrap">                     ' +
            '                         <div class="vis-control-wrap">                           ' +
            '                             <select class="vis-fp-op">                           ' +
            '                             </select>                                            ' +
            '                             <label class="vis-fp-lblop">' + VIS.Msg.getMsg("Operator") + '</label>         ' +
            '                         </div>                                                   ' +
            '                     </div>                                                       ' +
            '                     <div class="vis-fp-valueone">                                ' +
            '                     </div>                                                       ' +
            '                     <div class="vis-fp-valuetwo">                                ' +
            '                     </div>                                                       ' +
            '                     <div class="vis-fp-valuethree">                              ' +
            '                     </div>                                                       ' +
            '                     <div class="vis-fp-cc-addbtnwrp">                            ' +
            '                         <span class="vis-fp-cc-addbutton">' + VIS.Msg.getMsg("Add") + '</span>             ' +
            '                     </div>                                                       ' +
            '                 </div>                                                           ' +
            '             </div>                                                               ' +
            '         </div>                                                                   ' +
            '         <div class="vis-fp-custcoltagswrp">                                      ' +
            '             <div class="vis-fp-custcoltag">                                      ' +
            '             </div>                                                               ' +
            '         </div><!-- vis-fp-custcoltagswrp -->                                     ' +
            '     </div>  </div>                                                                     ' +
            ' </div>';
        return str;
    };

    //AdvanceSearch
    function FilterPanel(windowNo, gc) {

        var tmp = getTemplate(windowNo);

        var control1, control2;
        var dsAdvanceData = null;

        var bodyDiv = $(tmp);
        var divCtrlWrap = bodyDiv.find(".vis-fp-datawrap");
        var divStatic = bodyDiv.find(".vis-fp-static-ctrlwrp");
        var divStaticInner = bodyDiv.find(".vis-fp-static-ctrlinnerwrp");
        var btnViewAll = bodyDiv.find(".vis-fp-viwall");
        var spnViewAll = $(btnViewAll.find('span')[0]);
        var divDynamic = bodyDiv.find(".vis-fp-custcolumns");
        var cmbColumns = divDynamic.find('.vis-fp-cols');
        var cmbOp = divDynamic.find('.vis-fp-op');
        var btnAdd = divDynamic.find('.vis-fp-cc-addbtnwrp');
        var divValue1 = divDynamic.find('.vis-fp-valueone');
        var divValue2 = divDynamic.find('.vis-fp-valuetwo');
        var divDynFilters = divDynamic.find('.vis-fp-custcoltag');

        spnViewAll.text(VIS.Msg.getMsg("ViewMore"));

        this.curGC = gc;
        this.winNo = windowNo;
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
                    var fieldorg = this.selectionfields[i];
                    if (!fieldorg.getIsDisplayed())
                        continue;

                    var field = {};
                    if (VIS.DisplayType.IsLookup(fieldorg.getDisplayType()) || VIS.DisplayType.ID == fieldorg.getDisplayType()) {
                        field = jQuery.extend(true, {}, fieldorg);
                        field.lookup = jQuery.extend(true, {}, fieldorg.lookup);
                        field.lookup.initialize();
                    }
                    else {
                        field = fieldorg;
                    }

                    this.selectionfields[i] = field;

                    if (field.getIsKey()) {
                        crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                            field.getColumnName());
                    }

                    else {
                        crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                    }

                    crt.setReadOnly(false);
                    crt.setMandatory(false);
                    this.ctrlObjects[field.getColumnName()] = crt;



                    var inputWrapGroup = $('<div class="vis-fp-inputgroupseprtr" data-ColumnName="' + crt.getName() + '" data-cid="' + crt.getName() + '_' + this.curTab.getAD_Tab_ID() + '"></div>');
                    var inputWrap = $('<div class="vis-control-wrap">');
                    var grp = $('<div class="input-group vis-input-wrap">');

                    if (field.getDisplayType() == VIS.DisplayType.YesNo) {

                        var htm = [];
                        htm.push('<div class="vis-fp-lst-searchrcrds">');
                        htm.push('<div class="vis-fp-inputspan">');
                        htm.push('<div class="vis-fp-istagwrap"><input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + crt.getName() + '" data-keyval="' + crt.getName() + '_Y" data-id="Y"');
                        htm.push('><span data-id="Y">' + VIS.Msg.getMsg("Yes") + '</span> </div>');
                        htm.push('</div>');
                        htm.push('<div class="vis-fp-inputspan">');
                        htm.push('<div class="vis-fp-istagwrap"><input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + crt.getName() + '" data-keyval="' + crt.getName() + '_N" data-id="N"');
                        htm.push('><span data-id="N">' + VIS.Msg.getMsg("No") + '</span> </div>');
                        htm.push('</div>');
                        htm.push('</div>');

                        inputWrap.append('<label>' + field.getHeader() + '</label>');
                        grp.append(inputWrap);
                        grp.append(htm.join(''));
                    }

                    else {

                        label = VIS.VControlFactory.getLabel(field); //get label
                        crt.addVetoableChangeListener(this);
                        inputWrap.append(crt.getControl());
                        if (label) {
                            label.getControl().find('sup').hide();

                            if (label)
                                inputWrap.append(label.getControl());
                        }
                        grp.append(inputWrap);
                    }
                    if (crt && crt.getBtnCount() > 1) {
                        var btn = crt.getBtn(0);
                        if (btn) {
                            var $divInputGroupAppend = $('<div class="input-group-append">');
                            $divInputGroupAppend.append(btn);
                            grp.append($divInputGroupAppend);
                        }
                    }
                    inputWrapGroup.append(grp);

                    divStaticInner.append(inputWrapGroup);

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
                        //if (finalWhereClause.length > 2) {
                        //    finalWhereClause += ' AND ';      //Append and in main where
                        //}

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
                            if (finalWhereClause != "")
                                finalWhereClause += " AND " + whereClause;
                            else
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

            var dynFilter = self.getDynamicFilter();
            if (dynFilter != '') {
                if (finalWhereClause != '')
                    finalWhereClause += ' AND ' + dynFilter;
                else
                    finalWhereClause = dynFilter;
            }
            return finalWhereClause;
        };

        this.fireValChanged = function (colName) {
            // if (ignoreTarget || $target.hasClass('vis-fp-inputvalueforupdate')) {
            // this.refreshAll(colName, prepareWhereClause(this));
            this.refreshAll(colName);
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
            if (field[0].lookup && field[0].lookup.getDisplay)
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
                            ctr.parent().parent().find('.vis-fp-spanCount').text("(0)");
                            selItems.push(ctr.parent().parent());
                        }
                        //else
                        ctr.parent().parent().remove();
                    }
                }
            }
            if (!fields || fields.length == 0) {
                fields = $('<div class="vis-fp-lst-searchrcrds vis-fp-lst-searchrcrdswrp"></div>');
                wrapper.append(fields);
            }


            for (var i = 0; i < data.length; i++) {
                var htm = [];
                var dId = data[i].ID;

                if (!isNaN(dId)) {
                    dId = parseFloat(dId);
                }

                var index = selIds.indexOf(dId);

                if (index > -1) {
                    selItems[index].find('.vis-fp-spanCount').text("(" + data[i].Count + ")");
                    fields.append(selItems[index]);
                    selItems.splice(index, 1);
                    selIds.splice(index, 1);
                    continue;
                }
                if (i < 5) {
                    htm.push('<div class="vis-fp-inputspan">');
                    htm.push('<div class="vis-fp-istagwrap"><input class="vis-fp-chboxInput vis-fp-inputvalueforupdate" type="checkbox" data-column="' + key + '" data-keyval="' + key + '_' + data[i].ID + '" data-id="' + data[i].ID + '"');
                    htm.push('><span data-id="' + data[i].ID + '">' + data[i].Name + '</span> </div><span class="vis-fp-spanCount">(' + data[i].Count + ')</span>');
                    htm.push('</div>');
                    fields.append(htm.join(''));
                }
            }

            for (i = 0; i < selItems.length; i++) {
                fields.append(selItems[i]);
            }

            selItems = [];
            selIds = [];
        };

        this.hardRefreshFilterPanel = function () {
            divStatic.find('.vis-fp-lst-searchrcrdswrp').remove();
            divStatic.find('.vis-fp-currntrcrdswrap').remove();
            divDynFilters.find('.vis-fp-currntrcrds').remove();
            dsAdvanceData = [];
            cmbColumns.val(-1);
            cmbOp.val(-1);
            setControlNullValue(false);
        };

        this.getFilterClause = function () {
            return prepareWhereClause(this);
        };

        //dynamic
        this.fillColumns = function (htm) {
            cmbColumns.append(htm);
        };

        var self = this;

        //Events ... 
        divStatic.on("click", "i", function (e) {
            var tgt = $(this);
            if (tgt.hasClass("vis-mark")) {

                tgt.parent().parent().remove();
                self.fireValChanged(tgt.data('keyval'));// evt.propertyName);
            }
        });

        divDynFilters.on("click", "i", function (e) {
            var tgt = $(this);
            if (tgt.hasClass("vis-mark")) {
                var colName = tgt.parent().data("id");
                deleteDynRow(colName);// evt.propertyName);
            }
        });

        bodyDiv.on("click", function (e) {
            $target = $(e.target);
            if ($target.is('input') && $target.hasClass('vis-fp-chboxInput')) {
                var currentColumnName = $target.data('column');
                self.fireValChanged(currentColumnName);
            }
        });

        btnAdd.on("click", function (e) {
            saveDynFilter();
        });

        btnViewAll.on("click", "span", function (e) {
            divStatic.toggleClass('vis-fp-static-ctrlwrp-auto');
            if (spnViewAll.text() == VIS.Msg.getMsg("ViewMore")) {
                spnViewAll.text(VIS.Msg.getMsg('ViewLess'));
            }
            else {
                spnViewAll.text(VIS.Msg.getMsg('ViewMore'));
            }
        });

        //dynamic
        cmbColumns.on('change', function (e) {
            // if (isBusy) return;
            //chkDynamic.prop("disabled", true);
            //chkDynamic.prop("checked", false);
            // chkDynamic.trigger("change");
            // divDynamic.hide();

            // set control at value1 position according to the column selected
            var columnName = cmbColumns.val();
            if (columnName && columnName != "-1") {
                var dsOp = null;
                // if column name is of ant ID
                if (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")) {
                    // fill dataset with operators of type ID
                    dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_ID);
                }
                else if (columnName.startsWith("Is")) {
                    // fill dataset with operators of type Yes No
                    dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_YN);
                }
                else {
                    // fill dataset with all operators available
                    dsOp = self.getOperatorsQuery(VIS.Query.prototype.CVOPERATORS);
                }

                var f = self.getTargetMField(columnName);

                if (f != null && VIS.DisplayType.IsDate(f.getDisplayType())) {
                    //drpDynamicOp.html(self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DATE_DYNAMIC, true));
                    //divDynamic.show();
                    // chkDynamic.prop("disabled", false);
                    // setDynamicQryControls();
                    // $root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 195px)');

                    // if (f.getDisplayType() == VIS.DisplayType.DateTime)// If Datetime, then on = operator, show full day checkbox.
                    // {
                    //   showValue2(false);
                    // showFullDay(true);
                    // }
                }
                //else if (self.getIsUserColumn(columnName)) {
                //drpDynamicOp.html($self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DYNAMIC_ID, true));
                //divDynamic.show();
                //$root.find('.vis-advancedSearchContentArea-down').css('height', 'calc(100% - 195px)');
                //chkDynamic.prop("disabled", false);
                //setDynamicQryControls(true);
                //}

                if (f.getDisplayType() != VIS.DisplayType.DateTime)// If Datetime, then on = operator, show full day checkbox.
                {
                    //  showValue2(true);
                    // showFullDay(false);
                }



                cmbOp.html(dsOp);
                cmbOp[0].SelectedIndex = 0;
                // get field
                //var field = getTargetMField(columnName);
                // set control at value1 position
                setControl(true, f);
                // enable the save row button
                //setEnableButton(btnSave, true);//silverlight comment
                cmbOp.prop("disabled", false);
            }
            else {
                // showFullDay(false);
                // showValue2(true);
            }
            // enable control at value1 position
            setValueEnabled(true);
            // disable control at value2 position
            setValue2Enabled(false);
        });

        bodyDiv.on("mouseover", function () {

            if ((divStaticInner.height() > (divStatic.parent().height() - 60)) && (divStaticInner.height() + 15) >= divStatic.height()) {

                btnViewAll.css('visibility', 'visible');
            }
            else {
                btnViewAll.css('visibility', 'hidden');
            }
        });

        bodyDiv.on("mouseout", function () {
            if (spnViewAll.text() == VIS.Msg.getMsg("ViewMore"))
                btnViewAll.css('visibility', 'hidden');
        });

        this.disposeComponent = function () {
            bodyDiv.remove();
            this.listOfFilterQueries = [];
            self = null;
        };

        //functions
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

        function setControl(isValue1, field) {
            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[0];
            }
            else {
                ctrl = divValue2.children()[0];
            }

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
                crt.setMandatory(false);
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
                var valueGrp = $('<div class="input-group vis-input-wrap">');
                var valueInputWrap = $('<div class="vis-control-wrap">');
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {
                    divValue1.append(valueGrp);
                    valueGrp.append(valueInputWrap);
                    valueInputWrap.append(crt.getControl());
                    control1 = crt;
                    if (btn) {
                        var $divInputGroupBtn = $('<div class="input-group-append">');
                        valueGrp.append($divInputGroupBtn);
                        $divInputGroupBtn.append(btn);

                    }
                    if (field.getDisplayType() == VIS.DisplayType.YesNo) {
                        ;
                    }
                    else {
                        valueInputWrap.append('<label>' + VIS.Msg.getMsg("QueryValue") + '</label>');
                    }
                }
                else {
                    //divValue2.append(valueGrp);
                    //valueGrp.append(valueInputWrap);
                    //valueInputWrap.append(crt.getControl());
                    //control2 = crt;
                    //if (btn) {
                    //    var $divInputGroupBtn = $('<div class="input-group-append">');
                    //    valueGrp.append($divInputGroupBtn);
                    //    $divInputGroupBtn.append(btn);
                    //    //crt.getControl().css("width", "calc(100% - 30px)");
                    //    //btn.css("max-width", "30px");
                    //}
                    //if (field.getDisplayType() == VIS.DisplayType.YesNo) {
                    //    ;
                    //}
                    //else {
                    //    var $InputLabel1 = $('<label>Query Value</label>');
                    //    valueInputWrap.append($InputLabel1);
                    //}
                }

                if (field.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    //crt.getControl().css("width", "100%");
                }
            }
        };

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

        this.getDynamicFilter = function () {
            var dynWhere = '';
            for (var col in dsAdvanceData) {
                var arr = dsAdvanceData[col];
                var arrCondition = [];
                for (var i = 0; i < arr.length; i++) {
                    if (i == 0) {
                        arrCondition.push(" ( ");
                        arrCondition.push(arr[i].Where);
                    }
                    else {
                        arrCondition.push(' OR ' + arr[i].Where);
                    }
                }
                if (arrCondition.length > 0)
                    arrCondition.push(" ) ");
                if (dynWhere != '')
                    dynWhere += ' AND ';
                dynWhere += arrCondition.join(' ');
            }
            return dynWhere;
        };

        function saveDynFilter() {
            // set column name
            var cVal = cmbColumns.val();

            if (!cVal || cVal == "-1")
                return false;

            var colName = cmbColumns.find("option:selected").text();
            var colValue = "";
            if (colName == null || colName.trim().length == 0) {
                return false;
            }
            else {
                // set column value
                colValue = cVal.toString();
            }

            // set operator name
            var opName = cmbOp.val();

            if (opName == null || opName == undefined)
                opName = cmbOp.find("option:selected").text();;;// vcmbOperator.Text;//silverlight comment
            // set operator (sign)
            var opValue = cmbOp.val();

            // add row in dataset
            addDynRow(colName, colValue, opName, opValue, getControlText(true), getControlValue(true), getControlText(false), getControlValue(false), true);

        }

        function addDynRow(colName, colValue, optr, optrName,
            value1Name, value1Value, value2Name, value2Value) {

            if (dsAdvanceData == null)
                dsAdvanceData = {};


            if (!(colValue in dsAdvanceData))
                dsAdvanceData[colValue] = [];

            dsAdvanceData[colValue].push({
                'Name': colName, 'Value': value1Value, 'Text': value1Name, 'Optr': optr,
                'Where': self.parseWhereCondition(colValue, optr, value1Value, value2Value)
            });
            refreshDynFiltersUI(colValue);
        };

        function deleteDynRow(colValue) {

            if (dsAdvanceData == null)
                dsAdvanceData = {};
            if (colValue in dsAdvanceData) {
                //var values = dsAdvanceData[col];
                //for (var i = 0; i < values.length; i++) {
                //    if (colValue == values[i]['Value']) {
                //        values.splice(i, 1);
                //        break;
                //    }
                //}
                //if (values.length < 1)
                delete dsAdvanceData[colValue];
            }
            refreshDynFiltersUI(colValue);
        };

        function refreshDynFiltersUI(colValue) {
            var selDiv = divDynFilters.find('[data-id="' + colValue + '"]');
            if (selDiv.length > 0) {
                selDiv.remove();
            }

            if (colValue in dsAdvanceData) {
                var htm = [];
                var arrVal = dsAdvanceData[colValue];
                htm.push('<div class="vis-fp-currntrcrds" data-id="' + colValue + '"><span  class="vis-fp-inputvalueforupdate">')
                for (var i = 0; i < arrVal.length; i++) {
                    if (i != 0)
                        htm.push(' | ');
                    else {
                        htm.push(arrVal[i]['Name']);
                        htm.push(arrVal[i]['Optr']);
                    }
                    htm.push(arrVal[i]['Text']);
                }
                htm.push('</span> <i class="vis vis-mark"></i></div>');
                divDynFilters.append(htm.join(' '));
            }

            self.fireValChanged();
            setControlNullValue();
        }
    };

    FilterPanel.prototype.init = function () {

        if (this.initialzed)
            return;


        this.curTab = this.curGC.getMTab();
        var curTabfieldlist = this.curTab.getFields();
        this.selectionfields = [];
        this.curTabfields = [];

        var html = '<option value="-1"> </option>';
        var sortedFields = [];
        //Fill Dynamic Column List 
        for (var c = 0; c < curTabfieldlist.length; c++) {
            // get field
            var fieldorg = curTabfieldlist[c];
            var field = jQuery.extend(true, {}, fieldorg);
            if (VIS.DisplayType.IsLookup(fieldorg.getDisplayType()) || VIS.DisplayType.ID == fieldorg.getDisplayType()) {
                field.lookup = jQuery.extend(true, {}, fieldorg.lookup);
                if (field.lookup.initialize)
                    field.lookup.initialize();
            }



            this.curTabfields.push(field);

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
                        field.lookup = new VIS.MLookupFactory.getMLookUp(VIS.context, this.winNo, field.getAD_Column_ID(), VIS.DisplayType.List);
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

            if (field.getIsSelectionColumn())
                this.selectionfields.push(field);
            else
                sortedFields.push({ 'value': columnName, 'text': header });
            // html += '<option value="' + columnName + '">' + header + '</option>';
        }
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
        //Add this html in Dynamic created column
        this.fillColumns(html);

        this.selectionfields.sort(function (a, b) { return a.getSelectionSeqNo() - b.getSelectionSeqNo() });
        this.getFixedColumns();
        this.setLayout();
        this.initialzed = true;

    };

    FilterPanel.prototype.getFixedColumns = function () {
        if (!this.selectionfields || this.selectionfields.length == 0) {
            this.selectionfields = $.grep(this.curTabfields, function (field, index) {
                if (field.getColumnName() == "Name"
                    || field.getColumnName() == "Value" || field.getColumnName() == "DocumentNo") {
                    return field;
                }
            });
        }
    };

    FilterPanel.prototype.getFilterOption = function (field, whereClause) {
        if (!whereClause)
            whereClause = "";
        if (field && field.getShowFilterOption()) {

            var keyCol = "";
            var displayCol = "";
            var validationCode = "";
            var lookupTableName = "";

            if (field.getLookup()) {
                keyCol = field.getLookup().info.keyColumn;
                displayCol = field.getLookup().info.displayColSubQ;
                validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), this.winNo, this.curTab.getTabNo(), field.getLookup().info.validationCode, false);
                lookupTableName = field.getLookup().info.tableName;
            }
            //if (!displayCol || displayCol == '')
            //    return;

            var tabWhere = this.curTab.getWhereClause();
            tabWhere = VIS.Env.parseContext(VIS.Env.getCtx(), this.winNo, this.curTab.getTabNo(), tabWhere, false);

            var linkWhere = this.curTab.getLinkWhereClause();

            if (linkWhere && linkWhere.length > 0) {
                if (whereClause != "")
                    whereClause += " AND " + linkWhere;
                else
                    whereClause += " " + linkWhere;
            }

            if (tabWhere && tabWhere.length > 0) {
                if (whereClause != "")
                    whereClause += " AND " + tabWhere;
                else
                    whereClause += " " + tabWhere;
            }
            var dynFilter = this.getDynamicFilter();
            if (dynFilter && dynFilter.length > 0) {
                if (whereClause != "")
                    whereClause += " AND " + dynFilter;
                else
                    whereClause += " " + dynFilter;
            }

            //Remove query which will fetch image.. Only display test in Filter option.
            if (displayCol.indexOf("||'^^'|| NVL((SELECT NVL(ImageURL,'')") > 0
                && displayCol.indexOf("thing.png^^') ||' '||") > 0) {
                var displayCol1 = displayCol.substr(0, displayCol.indexOf("||'^^'|| NVL((SELECT NVL(Imag"));
                displayCol = displayCol.substr(displayCol.indexOf("othing.png^^') ||' '||") + 22);
                displayCol = displayCol1 + "||'_'||" + displayCol;
            }
            if (displayCol.indexOf("||'^^'|| NVL((SELECT NVL(ImageURL,'')") > 0) {
                displayCol = displayCol.replace(displayCol.substr(displayCol.indexOf("||'^^'|| NVL((SELECT NVL(Imag"), displayCol.indexOf("Images/nothing.png^^')") + 21), '');
            }
            else if (displayCol.indexOf("nothing.png") > -1) {
                displayCol = displayCol.replace(displayCol.substr(displayCol.indexOf("NVL((SELECT NVL(ImageURL,'')"), displayCol.indexOf("thing.png^^') ||' '||") + 21), '')
            }


            var data = {
                keyCol: keyCol, displayCol: displayCol, validationCode: validationCode
                , tableName: lookupTableName, AD_Referencevalue_ID: field.getAD_Reference_Value_ID(), pTableName: this.curTab.getTableName(),
                pColumnName: field.getColumnName(), whereClause: whereClause,
            };
            var tht = this;

            filterContext.getFilters(data).then(function (data) {

                var key = data["keyCol"];
                data = data["list"];
                //if (data && data.length > 0) {
                tht.setFilterOptions(data, key);
                //}
                tht = null;
            });
        }
    };

    FilterPanel.prototype.refreshFilterOptions = function (colName, hardRefresh) {
        if (hardRefresh) {
            this.hardRefreshFilterPanel();
        }
        if (!this.selectionfields)
            return;

        for (var i = 0; i < this.selectionfields.length; i++) {
            if (this.selectionfields[i].getShowFilterOption()) {
                var field = this.selectionfields[i];
                if (field.getColumnName() != colName) {
                    var whereClause = '';
                    for (var j = 0; j < this.listOfFilterQueries.length; j++) {
                        var query = this.listOfFilterQueries[j];
                        if (query.columnName != field.getColumnName()) {
                            if (whereClause.length > 1)
                                whereClause += " AND ";
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
        //query.addRestriction(finalWhereClause);
        //this.curTab.setQuery(query);
        //this.curGC.query(0, 0, null);
        this.curGC.applyFilters(query);
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

        sb += this.curTab.getTableName() + '.' + column;


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

        if (value != null && value.length > 0 && VIS.DisplayType.IsText(field.getDisplayType()) && optr == VIS.Query.prototype.EQUAL) {
            optr = VIS.Query.prototype.LIKE;
            value = '%' + value + '%';
        }

        var whereCondition = '';

        var parsedValue = null;
        if (value != null && (value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@"))) {
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
                    whereCondition = this.createDirectSql(parsedValue, parsedValue2, columnSQL, optr, true);

                }
            }
        }
        return whereCondition;
    };

    FilterPanel.prototype.dispose = function () {
        this.disposeComponent();
        this.curGC = this.curTab = this.curTabfields = this.selectionfields = null;
    };

    FilterPanel.prototype.getOperatorsQuery = function (vnpObj, translate) {
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