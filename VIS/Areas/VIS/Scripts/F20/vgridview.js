; (function (VIS, $) {
    //****************************************************//
    //**             VTable                            **//
    //**************************************************//
    function VTable() {

        this.grid = null;
        this.id = null;
        this.$container = null;
        this.rendered = false;

        this.onSelect = null;
        this.onCellEditing = null;
        this.onCellValueChanged = null;


        this.onSort = null;
        this.onEdit = null;
        this.onAdd = null;

        this.editColumnIndex = -1;
        var clickCount = 0;


        var self = this;
        var editColumn = {
            caption: "",
            sortable: false,
            render: function (record) {
                return '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/pencil.png"  />';
            },
            size: '25px'
        };

        function toggleToSingleView(evt) {
            try {
                if (self.grid.columns[evt.column].columnName.toLowerCase() == 'value' || self.grid.columns[evt.column].columnName.toLowerCase() == 'documentno' || self.grid.columns[evt.column].columnName.toLowerCase() == 'name') {
                    self.grid.select(Number(evt.recid));
                    var isCompositView = $('#AS_' + self.mTab.getWindowNo() + '_' + self.mTab.getAD_Window_ID()).find('[name=' + evt.target + ']').closest('.vis-ad-w-p-center-inctab');
                    if (isCompositView.length > 0) {
                        if (isCompositView.find('.vis-multi').length > 0) {
                            isCompositView.find('.vis-multi').click();
                        } else {
                            isCompositView.find('.vis-edit').click();
                        }
                       
                    } else {
                        $('#AS_' + self.mTab.getWindowNo() + '_' + self.mTab.getAD_Window_ID()).find(' .vis-multi:first').click();
                    }

                }
            } catch (err) {

            }
        }

        this.getEditColumn = function () {
            return editColumn;
        }

        this.onClick = function (evt) {
            clickCount++;
            // console.log(evt);
            if (this.readOnly)
                return;

            self.skipEditing = false;
            if (this.records.length > 0) {
                if (this.records[self.mTab.getCurrentRow()].recid != evt.recid) {
                    self.skipEditing = true;
                }
            };

            //if (isNaN(evt)) {
            //    if (evt.column === self.editColumnIndex) {
            //        var recids = self.grid.getSelection();
            //        if (recids.indexOf(parseInt(evt.recid)) > -1) {
            //            if (self.onEdit) {
            //                self.onEdit(evt.recid);
            //            }
            //        }
            //        //evt.isCancelled = true;

            //    }
            // }
            //else alert(evt);
            //console.log(" double click");
        };

        this.onSingleClick = function (evt) {
            //this.cRecid = evt.recid;
            clickCount++;
            singleClickTimer = setTimeout(function () {
                if (clickCount === 1) {
                    clickCount = 0;
                    toggleToSingleView(evt);
                } else if (clickCount === 2) {
                    clearTimeout(singleClickTimer);
                    clickCount = 0;
                }
            }, 400);
        };

        this.onSelectLocal = function (evt) {

            if (self.blockSelect) {
                self.blockSelect = false;
                return;
            }
            if (self.onSelect) {
                self.onSelect(evt);
            }
        };

        this.checkCellEditable = true;

        function isCellEditable(column) {

            if (self.readOnly)
                return false;

            var col = self.grid.columns[column];

            if (!self.mTab.getTableModel().getIsInserting() && col.readOnly) {
                return false;
            }
            else {
                var field = col.gridField; // .Column.  CurrentCell.OwningColumn.Name);
                if (field.getIsEditable(true, true))//|| _headerClicked)
                {
                    return true;
                }
            }
            return false;
        };

        this.onEditField = function (evt) {

            if (self.skipEditing) {
                self.skipEditing = false;
                evt.isCancelled = true;
                this.select({ recid: evt.recid });
                return;
            }

            evt.isCancelled = !isCellEditable(evt.column) || (evt.originalEvent && evt.originalEvent.altKey);
            //self.checkCellEditable = false; // if cell is checked for edit then mark flag to false to skip repeateable check
            self.blockSelect = !evt.isCancelled
        };

        this.onChange = function (evt) {

            if (self.grid.columns[evt.column].editable.type == 'checkbox') { // check box field on fire on edit field event
                if (this.records.length > 0) {
                    if (this.records[self.mTab.getCurrentRow()].recid != evt.recid) {
                        evt.isCancelled = true;
                        //window.setTimeout(function(t){
                        //    t.onChange(evt);
                        //},1,this);
                        return;
                    }
                };
                evt.isCancelled = !isCellEditable(evt.column);
            }
            //self.checkCellEditable = true;

            evt.onComplete = function (event) {

                // if (event.value_original != event.value_new) {
                var evta = { newValue: event.value_new, propertyName: self.grid.columns[event.column].field };

                if (self.onCellValueChanged) {
                    self.onCellValueChanged(evta, self.grid.columns[event.column].editable.type == 'checkbox');
                }
                //  }
            }
        };

        this.onUnSelect = function (evt) {
            //var recids = self.grid.getSelection();
            //if (recids.length == 1 && this.cRecid === recids[0]) {
            //    evt.isCancelled = true;
            //}
        };

        this.onRowAdd = function (evt) {
            self.paintRow(evt.index);
        };

        this.paintRow = function (index) {

            var rec = this.grid.records[index];
            if (!checkRowEditable(index, 0)) {
                rec.style = "background-color:rgba(var(--v-c-secondary), .7)";
                return;
            }

            if (rec && rec.style)
                delete rec['style'];
        };

        function checkRowEditable(index, col) {
            if (self.readOnly)
                return false;

            //  IsActive Column always editable if no processed exists
            if (col == self.indexActiveColumn && self.indexProcessedColumn == -1)
                return true;
            //	Row
            if (!isRowEditable(index))
                return false;

            return true;
        };

        function isRowEditable(row) {
            if (self.readOnly || row < 0)
                return false;
            //	If not Active - not editable
            if (self.indexActiveColumn && self.indexActiveColumn > 0)		//	&& m_TabNo != Find.s_TabNo)
            {
                //Object value = GetCellValue(this.Rows[row], _indexActiveColumn);
                var value = self.grid.getCellValue(row, self.indexActiveColumn);

                if (typeof value == "boolean") {
                    if (!value)
                        return value;
                }

                else if (!"True".equals(value))// instanceof Boolean)
                {
                    //if (!((Boolean)value).booleanValue())
                    return false;
                }
                else if ("N".equals(value))
                    return false;
            }
            //	If Processed - not editable (Find always editable)
            if (self.indexProcessedColumn && self.indexProcessedColumn > 0)		//	&& m_TabNo != Find.s_TabNo)
            {
                //Object processed = GetCellValue(this.Rows[row], _indexProcessedColumn);
                var processed = self.grid.getCellValue(row, self.indexProcessedColumn);
                if (typeof processed == "boolean") {
                    if (processed)
                        return false;
                }

                else if ("True".equals(processed))// instanceof Boolean)
                {
                    return false;
                }
                else if ("Y".equals(processed))
                    return false;
            }
            //
            //int[] co = GetClientOrgRecordID(this.Rows[row]);
            var co = getClientOrgRecordID(row);
            var AD_Client_ID = co[0];
            var AD_Org_ID = co[1];
            var Record_ID = co[2];

            return VIS.MRole.canUpdate
                (AD_Client_ID, AD_Org_ID, self.AD_Table_ID, Record_ID, false);

        };

        function getClientOrgRecordID(row) {
            var AD_Client_ID = -1;
            if (typeof self.indexClientColumn != "undefined" && self.indexClientColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexClientColumn);//].Value;
                if (ii != null && ii !== "")
                    AD_Client_ID = VIS.Utility.Util.getValueOfInt(ii);
            }
            var AD_Org_ID = 0;
            if (typeof self.indexOrgColumn != "undefined" && self.indexOrgColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexOrgColumn);
                if (ii != null && ii !== "")
                    AD_Org_ID = VIS.Utility.Util.getValueOfInt(ii);
            }
            var Record_ID = 0;
            if (typeof self.indexKeyColumn != "undefined" && self.indexKeyColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexKeyColumn);
                if (ii != null && ii !== "")
                    Record_ID = VIS.Utility.Util.getValueOfInt(ii);
            }

            return [AD_Client_ID, AD_Org_ID, Record_ID];
        };      

        //this.onToolBarClick = function (target, data) {
        //    //self.$editBtn.img = "icon-reload";
        //    if (target.startsWith("Edit")) {
        //        if (self.onEdit) {
        //            self.onEdit();
        //        }
        //    }
        //    else if (target.startsWith("Add")) {
        //        if (self.onAdd) {
        //            self.onAdd();
        //        }
        //    }
        //};

        this.disposeComponenet = function () {
            self = null;
            editColumn = null;
            this.getEditColumn = null;
            this.onClick = null;
        };
    };

    VTable.prototype.ROW_ADD = 'A';
    VTable.prototype.ROW_DELETE = 'D';
    VTable.prototype.ROW_REFRESH = 'F';
    VTable.prototype.ROW_UNDO = 'U';

    VTable.prototype.setupGridTable = function (aPanel, grdFields, $container, name, mTab, gc) {

        if (!mTab.getIsDisplayed(true))
            return 0;

        this.id = name;
        this.$container = $container;
        this.mTab = mTab;
        this.AD_Table_ID = this.mTab.getAD_Table_ID();


        var oColumns = [];
        var mField = null;
        var size = grdFields.length;
        var visibleFields = 0;

        var mFields = grdFields.slice(0);

        mFields.sort(function (a, b) { return a.getMRSeqNo() - b.getMRSeqNo() });

        var j = -1;
        for (var i = 0; i < mFields.length; i++) {
            mField = mFields[i];
            if (mField == null)
                continue;
            var columnName = mField.getColumnName();
            var displayType = mField.getDisplayType();

            //if (VIS.DisplayType.ID == displayType || columnName == "Created" || columnName == "CreatedBy"
            //                                    || columnName == "Updated" || columnName == "UpdatedBy") {
            //    if (!mField.getIsDisplayed()) {
            //        continue;
            //    }
            //}

            ++j;
            if (mField.getIsKey())
                this.indexKeyColumn = j;
            else if (columnName.equals("IsActive"))
                this.indexActiveColumn = j;
            else if (columnName.equals("Processed"))
                this.indexProcessedColumn = j;
            else if (columnName.equals("AD_Client_ID"))
                this.indexClientColumn = j;
            else if (columnName.equals("AD_Org_ID"))
                this.indexOrgColumn = j;

            var isDisplayed = mField.getIsDisplayedMR ? mField.getIsDisplayedMR() : mField.getIsDisplayed();

            var mandatory = mField.getIsMandatory(false);      //  no context check
            var readOnly = mField.getIsReadOnly();
            var updateable = mField.getIsEditable(false);      //  no context check
            //int WindowNo = mField.getWindowNo();

            //  Not a Field
            if (mField.getIsHeading())
                continue;

            var oColumn = {

                resizable: true
            }

            oColumn.gridField = mField;

            if (readOnly || !updateable) {
                oColumn.readOnly = true;   //
            }

            oColumn.caption = mField.getHeader();
            if (mandatory) {
                oColumn.caption += '<sup style="color:red;font-size: 11px;top: 0;">*</sup>';
            }
            oColumn.field = columnName.toLowerCase();
            oColumn.hidden = !isDisplayed;
            var columnWidth = oColumn.gridField.getColumnWidth();

            if (columnWidth) {
                oColumn.size = columnWidth + 'px';
            }
            else {
                oColumn.size = '100px';
            }

            if (displayType == VIS.DisplayType.Amount) {
                oColumn.sortable = true;
                oColumn.customFormat = VIS.DisplayType.GetNumberFormat(displayType);
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (!val) {
                        val = 0; // show zero if null
                    }
                    //if (record.changes && typeof record.changes[f] != 'undefined') val = record.changes[f];
                    val = parseFloat(val).toLocaleString(undefined, {
                        'minimumFractionDigits': oColumns[colIndex].customFormat.getMinFractionDigit(),
                        'maximumFractionDigits': oColumns[colIndex].customFormat.getMaxFractionDigit()
                    });
                    return '<div data-type="int">' + val + '</div>';
                };
                //oColumn.caption = 'class="vis-control-wrap-int-amount"';
            }
            else if (displayType == VIS.DisplayType.Integer) {
                oColumn.sortable = true;
                oColumn.customFormat = VIS.DisplayType.GetNumberFormat(displayType);
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];

                    if (!val)
                        return;

                    //if (record.changes && typeof record.changes[f] != 'undefined') val = record.changes[f];
                    //val = parseFloat(val).toLocaleString(undefined, {
                    //    'minimumFractionDigits': oColumns[colIndex].customFormat.getMinFractionDigit(),
                    //    'maximumFractionDigits': oColumns[colIndex].customFormat.getMaxFractionDigit()
                    //});
                    return '<div data-type="int">' + val + '</div>';
                };
                //oColumn.caption = 'class="vis-control-wrap-int-amount"';
            }
            else if (VIS.DisplayType.IsNumeric(displayType)) {
                oColumn.sortable = true;
                oColumn.customFormat = VIS.DisplayType.GetNumberFormat(displayType);
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (!val)
                        return;
                    //if (record.changes && typeof record.changes[f] != 'undefined') val = record.changes[f];
                    // return  Globalize.format(Number(oColumns[colIndex].customFormat.GetFormatedValue(val)));
                    val = parseFloat(val).toLocaleString(undefined, {
                        'minimumFractionDigits': oColumns[colIndex].customFormat.getMinFractionDigit(),
                        'maximumFractionDigits': oColumns[colIndex].customFormat.getMaxFractionDigit()
                    });

                    return '<div data-type="int">' + val + '</div>';
                };
                // oColumn.style = 'text-align: right';
                // oColumn.caption = 'class="vis-control-wrap-int-amount"';
            }
            //	YesNo
            else if (displayType == VIS.DisplayType.YesNo) {

                oColumn.sortable = true;
                var lCol = columnName.toLowerCase();
                //oColumn.render = function (record, index, colIndex) {
                //    var chk = (record[oColumns[colIndex].field]) ? "checked" : "";
                //    //console.log(chk);
                //    return '<input type="checkbox" ' + chk + ' disabled="disabled" >';
                //}
                oColumn.editable = { type: 'checkbox' };
            }
            //	String (clear/password)
            else if (displayType == VIS.DisplayType.String
                || displayType == VIS.DisplayType.Text || displayType == VIS.DisplayType.TextLong
                || displayType == VIS.DisplayType.Memo) {


                oColumn.sortable = true;
                if (columnName.toLowerCase() == "value" || columnName.toLowerCase() == "name" || columnName.toLowerCase() == "documentno") {
                    oColumn.style = 'text-decoration:underline; color:blue !important; cursor:pointer';
                }

                if (mField.getIsEncryptedField()) {
                    oColumn.render = function (record, index, colIndex) {
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }

                        var val = record[oColumns[colIndex].field];
                        if (val || (val === 0))
                            return val.replace(/\w|\W/g, "*");
                        return "";
                    }
                }
                else if (mField.getObscureType()) {
                    oColumn.render = function (record, index, colIndex) {
                        var val = record[oColumns[colIndex].field];
                        if (val || (val === 0))
                            return VIS.Env.getObscureValue(oColumns[colIndex].gridField.getObscureType(), val);
                        return "";
                    }
                }
                else {
                    oColumn.render = function (record, index, colIndex) {
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }



                        if (val || val == 0) {
                            //if (d.toString().indexOf('<') > -1)
                            //    return "";
                            val = w2utils.encodeTags(val);
                            return val;
                        }
                        return "";
                    }
                }

            }

            else if (VIS.DisplayType.IsLookup(displayType) || displayType == VIS.DisplayType.ID) {

                oColumn.sortable = true;


                oColumn.lookup = mField.getLookup();

                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;

                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            //val = record.changes[f];
                        }
                        var d;
                        if (l) {
                            d = l.getDisplay(val, true);
                            //if (d.startsWith("<"))
                            //  d = l.getDisplay(nd, false);
                            //d = w2utils.encodeTags(d);
                        }

                        return d;
                        //return '<span>' + d + '</span>';
                    }
                }
            }
            //Date /////////
            else if (VIS.DisplayType.IsDate(displayType)) {

                oColumn.sortable = true;
                oColumn.displayType = displayType;

                //oColumn.render = 'date';
                oColumn.render = function (record, index, colIndex) {
                    var col = oColumns[colIndex];
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }



                    if (val)
                        if (col.displayType == VIS.DisplayType.Date) {
                            var d = new Date(val);
                            d.setMinutes(d.getTimezoneOffset() + d.getMinutes());
                            //val = Globalize.format(d, 'd');
                            val = d.toLocaleDateString();
                        }
                        else if (col.displayType == VIS.DisplayType.DateTime)
                            val = new Date(val).toLocaleString();
                        //val = Globalize.format(new Date(val), 'f');
                        else
                            val = new Date(val).toTimeString();
                    //val = Globalize.format(new Date(val), 't');
                    else val = "";
                    return val;
                }
            }

            else if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator) {

                oColumn.sortable = true;

                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.AmtDimension) {

                oColumn.sortable = true;
                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.ProductContainer) {

                oColumn.sortable = true;
                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }


            else if (displayType == VIS.DisplayType.Account || displayType == VIS.DisplayType.PAttribute) {

                oColumn.sortable = true;

                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }



                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }
                        return val;
                    }
                }
            }



            else if (displayType == VIS.DisplayType.PAttribute) {

                oColumn.sortable = true;

                oColumn.render = 'int';
            }

            else if (displayType == VIS.DisplayType.Button) {

                oColumn.sortable = true;

                //oColumn.render = function (record) {
                //    return '<div>button</div>';
                //}
            }

            else if (displayType == VIS.DisplayType.Image) {

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (!val) {
                        val = "-";
                        return val;
                    }
                    return VIS.Msg.getElement1('AD_Image_ID') + '-' + val;
                    //var img = $('img').error(function () {
                    //    $(this).attr("src", VIS.Application.contextUrl + "/Images/Thumb32x32/" + val + ".jpeg ");
                    //}).attr("src", VIS.Application.contextUrl + "/Images/Thumb32x32/" + val+".png ");

                    //return img.html();
                }
            }

            else if (VIS.DisplayType.IsLOB(displayType)) {

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (!val) {
                        val = "";
                    }
                    return "#" + val.toString().length;
                }
            }

            else { //all text Type Columns

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (val || val == 0) {
                        //if (d.toString().indexOf('<') > -1)
                        //    return "";
                        val = w2utils.encodeTags(val);
                        return val;
                    }
                    return "";
                }
            }


            if (mField.getHtmlStyle() != "") {
                oColumn.style = mField.getHtmlStyle();
            }

            if (!oColumn.hidden) {
                visibleFields++;
            }
            oColumns.push(oColumn);
            oColumn.columnName = columnName;


            var iControl = VIS.VControlFactory.getControl(mTab, mField, false, false, false);
            iControl.setReadOnly(false);

            if (!oColumn.editable)
                oColumn.editable = { type: 'custom', ctrl: iControl };

            iControl.addVetoableChangeListener(gc);

            if (iControl instanceof VIS.Controls.VButton) {
                iControl.addActionListner(aPanel);
            }
        }

        if (visibleFields > 0) {

            var w = Math.floor(100 / visibleFields);
            for (var p in oColumns) {
                if (oColumns[p].hidden) {
                }
                else {
                    if (!oColumns[p].size < 0) {

                        oColumns[p].size = w + '%';
                        //oColumns[p].size = 150+'px';
                        //oColumns[p].size = w + '%';
                        oColumns[p].min = 100;
                    }

                }
            }
        }


        //oColumns[oColumns.length - 1].size = "100%";
        this.grid = $().w2grid({
            name: name,
            autoLoad: false,
            columns: oColumns,
            records: [],
            show: {

                toolbar: false,  // indicates if toolbar is v isible
                columnHeaders: true,   // indicates if columns is visible
                lineNumbers: false,  // indicates if line numbers column is visible
                selectColumn: true,  // indicates if select column is visible
                toolbarReload: false,   // indicates if toolbar reload button is visible
                toolbarColumns: true,   // indicates if toolbar columns button is visible
                toolbarSearch: false,   // indicates if toolbar search controls are visible
                toolbarAdd: false,   // indicates if toolbar add new button is visible
                toolbarDelete: false,   // indicates if toolbar delete button is visible
                toolbarSave: false,   // indicates if toolbar save button is visible
                selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                recordTitles: false	 // indicates if to define titles for records
            },
            //toolbar: {
            //    items: [
            //        //{ type: 'spacer' },
            //        { type: 'break' },
            //        //{ type: 'button', id: 'Add_' + name,  img: 'icon-Add' },
            //        { type: 'button', id: 'Edit_' + name, img: 'icon-edit' },
            //        { type: 'break' },
            //        { type: 'button', id: 'Add_' + name, img: 'icon-add' }
            //    ],
            //    onClick: this.onToolBarClick
            //    //{

            //    //    console.log(data);
            //    //}
            //},
            recordHeight: 41,
            onSelect: this.onSelectLocal,
            onUnselect: this.onUnSelect,
            onSort: this.onSort,
            onClick: this.onSingleClick,
            onDblClick: this.onClick,
            onEditField: this.onEditField,
            onChange: this.onChange,
            onRowAdd: this.onRowAdd
            //onResize: function () { alert('resize') }

        });
        //this.grid.selectType = 'cell';;
        return size;
    };

    VTable.prototype.get = function (recid, isIndex) {
        return this.grid.get(recid, isIndex);
    };

    VTable.prototype.activate = function () {
        if (this.grid && !this.rendred) {
            this.$container.w2render(this.grid['name']);
            this.rendred = true;
        }
        else {
            //this.grid.refresh();
            //this.grid.resize();

        }
    };

    VTable.prototype.setReadOnly = function (ro) {
        this.readOnly = ro;
    };

    VTable.prototype.getGrid = function () {
        return this.grid;
    };

    VTable.prototype.select = function (recid) {

        var selIds = this.grid.getSelection();
        if (selIds.indexOf(recid) != -1)
            return;
        if (selIds.length == 1) {
            this.grid.unselect(selIds[0]);
        }
        else if (selIds.length > 1) {
            while (selIds.length > 0) {
                this.grid.unselect(selIds.pop());
            }
        }
        return this.grid.select(recid);
    };

    VTable.prototype.add = function (records) {
        this.grid.add(records);
    };

    VTable.prototype.clear = function () {
        this.grid.clear(true);
        this.grid.reset();
    };

    VTable.prototype.refresh = function () {
        this.grid.refresh();
    };

    VTable.prototype.resize = function () {
        this.grid.resize();//
    };

    VTable.prototype.setRow = function (record) {
        if (isNaN(record)) {
            this.grid.set(record.recid, record);
        }
        else {
            this.grid.set(record);
        }
    };

    VTable.prototype.refreshRow = function (row, isRecid) {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        if (sel < 0) {
            return;
        }

        if (typeof row != 'undefined')
            sel = row;

        this.paintRow(isRecid ? this.grid.get(row, true) : sel);

        this.grid.clearRowChanges(isRecid ? row : this.grid.records[sel].recid);

    };

    VTable.prototype.refreshCells = function () {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        this.paintRow(sel);

        this.grid.refreshRow(this.grid.records[sel].recid);
    };

    VTable.prototype.refreshUndo = function () {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        this.grid.clearRowChanges(this.grid.records[sel].recid);

        this.paintRow(sel);
        this.grid.refreshRow(this.grid.records[sel].recid);
    };

    VTable.prototype.getSelection = function (retIndex) {
        return this.grid.getSelection(retIndex);
    };

    VTable.prototype.getSelectedRows = function () {
        var indexs = this.grid.getSelection(true);
        var rows = [];
        for (var i = 0, j = indexs.length; i < j; i++) {
            rows.push(this.grid.records[indexs[i]]);
        }
        return rows;
    };

    VTable.prototype.getColumnNames = function () {
        var cols = this.grid.columns;
        var colObj = {};
        for (var i = 0, j = cols.length; i < j; i++) {
            colObj[cols[i].columnName] = cols[i].caption;
        }
        return colObj;
    };

    VTable.prototype.scrollInView = function (index) {
        this.grid.scrollIntoView(index);
    };

    VTable.prototype.tableModelChanged = function (action, args, actionIndexOrId) {

        this.blockSelect = true;

        if (action === VIS.VTable.prototype.ROW_REFRESH) {
            this.setRow(args); //record 
            this.refreshRow(args.recid, true);
        }

        else {

            var id = null;
            if (action === VIS.VTable.prototype.ROW_UNDO) {
                this.grid.unselect(args);
                this.grid.remove(args);
                if (actionIndexOrId >= (this.grid.records.length - 1) && this.grid.records.length > 0) {
                    this.blockSelect = false;  //fire select event to update single layout
                    id = this.grid.records[this.grid.records.length - 1].recid;
                }
                else if (actionIndexOrId < this.grid.records.length) {
                    id = this.grid.records[actionIndexOrId].recid; // just select grid row
                }
            }
            else if (action === VIS.VTable.prototype.ROW_ADD) {
                this.grid.records.splice(actionIndexOrId, 0, args);// add at index
                id = args.recid; // row to select
                this.grid.refresh(); //refresh Grid
                this.blockSelect = true; // forcefully block select changed event
            }

            else if (action === VIS.VTable.prototype.ROW_DELETE) {
                //var size = args.length;
                var argsL = args.slice();
                while (argsL.length > 0) {
                    var recid = argsL.pop();
                    this.grid.unselect(recid);
                    this.grid.remove(recid);
                }
                if (isNaN(actionIndexOrId)) //recid array In this case
                {
                    id = actionIndexOrId[0];
                }
                else {
                    if (this.grid.records.length > 0)
                        id = this.grid.records[(this.grid.records.length - 1) < actionIndexOrId ? (this.grid.records.length - 1) : actionIndexOrId].recid;
                }
            }



            if (id) {
                this.select(id); //Select Row
            }
        }

        this.blockSelect = false;
    };

    //Set Default Focus for grid... Not in use Yet.
    VTable.prototype.setDefaultFocus = function (colName) {
        var selIndices = this.grid.getSelection();  //this.grid.getSelection(true);
        var colIndex = this.grid.getColumn(colName.toLower(), true)
        if (selIndices && selIndices.length > 0) {
            ///this.grid.editField(selIndices[0], colIndex);
            this.grid.dblClick(selIndices[0], { metaKey: true });
        }
    };

    VTable.prototype.dispose = function () {
        this.grid.off("select", this.onSelect);
        this.grid.off("sort", this.onSort);
        this.grid.off("click", this.onClick);
        this.onSelect = null;
        this.onSort = null;

        var cols = this.grid.columns;

        for (var col in cols) {
            if (cols[col].editable.ctrl)
                cols[col].editable.ctrl.dispose();
        }

        // console.log(this.grid);
        this.grid.destroy();
        // console.log(this.grid);
        this.grid = null;
        this.id = null;
        this.$container = null;
        this.rendered = null;
        this.disposeComponenet();
    };

    //*************** END VTABLE   *********************//
    //Assignment Gobal Namespace


    VIS.VTable = VTable;



}(VIS, jQuery));