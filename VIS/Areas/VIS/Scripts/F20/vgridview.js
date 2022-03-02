; (function (VIS, $) {
    //****************************************************//
    //**             VTable                            **//
    //**************************************************//
    function VTable() {

        this.grid = null;
        this.id = null;
        this.$container = null;
        this.aPanel = null;
        this.rendered = false;

        this.onSelect = null;
        this.onCellEditing = null;
        this.onCellValueChanged = null;


        this.onSort = null;
        this.onEdit = null;
        this.onAdd = null;
        this.hyperLinkCell = null;

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
                if (self.grid.columns[evt.column].columnName == self.hyperLinkCell) {
                    self.grid.select(Number(evt.recid));
                    var isCompositView = self.aPanel.getRoot().find('[name=' + evt.target + ']').closest('.vis-ad-w-p-center-inctab');
                    if (isCompositView.length > 0) {
                        if (isCompositView.find('.vis-multi').length > 0) {
                            isCompositView.find('.vis-multi').click();
                        } else {
                            isCompositView.find('.vis-edit').click();
                        }
                    } else {
                        // main view
                        //self.aPanel.getRoot().find(' .vis-multi:first').click();
                        self.aPanel.actionPerformedCallback(self.aPanel, "Multi");
                        self.aPanel.setLastView("Multi");
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

        /**
         * cell render handler 
         * @param {any} rIndex row index
         * @param {any} cIndex cell index
         */
        this.cellStyleRender = function (rIndex, cIndex) {
            var col = self.grid.columns[cIndex];
            if (!col.selfCellStyleRender && col.gridField && col.gridField.getStyleLogic() != '') // get from property
            {
                return self.evaluateStyleLogic(rIndex, col.gridField.getStyleLogic());
            }
            return null;
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

    /**
     * Evaluate style logic
     * @param {any} rIndex row Index
     * @param {any} styleLogic logic string {expression [,]}
     */
    VTable.prototype.evaluateStyleLogic = function (rIndex, styleLogic) {
        this.cellRowIndex = rIndex;
        var arr = styleLogic.split(',');

        //this.cellColumnName = col.field;
        var ret = null;
        for (var j = 0; j < arr.length; j++) {
            var cArr = arr[j].split("?");
            if (cArr.length != 2)
                continue;
            if (VIS.Evaluator.evaluateLogic(this, cArr[0])) {
                ret = cArr[1];
                break;
            }
        }
        return ret;

    };


    VTable.prototype.setupGridTable = function (aPanel, grdFields, $container, name, mTab, gc) {

        if (!mTab.getIsDisplayed(true))
            return 0;

        this.id = name;
        this.aPanel = aPanel;
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

                resizable: true,
                selfCellStyleRender: false  /* self evalauate Style conditions*/
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

            if (mField.getIsIdentifier() && mField.getDisplayType() != VIS.DisplayType.Image && this.hyperLinkCell == null) {
                if (oColumn.hidden == false) {
                    this.hyperLinkCell = columnName;
                    oColumn.style = 'text-decoration:underline; color:rgba(var(--v-c-primary), 1) !important; cursor:pointer';
                }
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
            else if (displayType == VIS.DisplayType.ProgressBar) {
                oColumn.sortable = true;
                oColumn.selfCellStyleRender = true;
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;

                    var gField = oColumns[colIndex].gridField;
                    var style = '';
                    if (gField.getStyleLogic() != '')
                        style = self.evaluateStyleLogic(index, gField.getStyleLogic());
                    if (!style) style = '';

                    var val = record[f];
                    //var maxVal = gField.getMaxValue();
                    //var minVal = gField.getMinValue();
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }
                    //return '<input id="rng' + index + '" type="range" min="' + minVal + '" max="' + maxVal + '" disabled="disabled" value="' + val + '" /><div style="position: absolute"><output class="vis-grid_progress_output"> ' + val+'</output></div>';

                    return '<div class="vis-progress-gridbar" style="' + style + '">' +
                        '<div class="vis-progress-percent-bar" style = "width:' + (val || 0) + '%;' + style + '" ></div>' +
                        '<div class="vis-progress-gridoutput" > ' + (val || '') + '</div></div >';
                }
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
                if (oColumn.gridField.getIsSwitch()) {
                    oColumn.render = "switch";
                    //oColumn.render = function (record, index, colIndex) {

                    //    var chk = (record[oColumns[colIndex].field]) ? "checked" : "";
                    //    //console.log(chk);
                    //   // return '<input type="checkbox" ' + chk + ' onclick="var obj = w2ui[\'' + name + '\'];     obj.editChange.call(obj, this, ' + index + ', ' + colIndex +', event)" class="vis-switch"><i for="switch" onclick="$(this).prev().click();"   class="vis-switchSlider">Toggle</i></div>';
                    //}
                }

                oColumn.editable = { type: 'checkbox' };

            }
            //	String (clear/password)
            else if (displayType == VIS.DisplayType.String
                || displayType == VIS.DisplayType.Text || displayType == VIS.DisplayType.TextLong
                || displayType == VIS.DisplayType.Memo) {


                oColumn.sortable = true;
                //if (oColumn.hidden == false && (this.hyperLinkCell[name] == "undefined" || this.hyperLinkCell[name] == null)) {
                //    if (columnName.toLowerCase() == "value" || columnName.toLowerCase() == "name" || columnName.toLowerCase() == "documentno") {
                //        this.hyperLinkCell[name] = columnName;
                //        oColumn.style = 'text-decoration:underline; color:rgba(var(--v-c-primary), 1) !important; cursor:pointer';
                //    }
                //}

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

                        var customStyle = oColumns[colIndex].gridField.getGridImageStyle();
                        var winNo = oColumns[colIndex].gridField.getWindowNo();
                        var customClass;
                        if (customStyle) {
                            customClass = oColumns[colIndex]['customClass'];
                            if (!customClass) {
                                oColumns[colIndex]['customClass'] = 'vis-grd-custom-' + oColumns[colIndex].gridField.getAD_Column_ID() + winNo;
                                customClass = '.vis-grd-custom-' + oColumns[colIndex].gridField.getAD_Column_ID() + winNo + "{" + customStyle + "}";
                                var styleTag = document.createElement('style');
                                styleTag.type = 'text/css';
                                styleTag.innerHTML = customClass;
                                $($('head')[0]).append(styleTag);
                            }
                        }

                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            //val = record.changes[f];
                        }
                        var d;
                        if (l) {
                            d = l.getDisplay(val, true, true);
                            //if (d.startsWith("<"))
                            //  d = l.getDisplay(nd, false);
                            //d = w2utils.encodeTags(d);
                        }

                        var strDiv = "";
                        if (l && VIS.DisplayType.List == l.displayType) {
                            var lType = l.getLovIconType(val, true);

                            var listIcon = l.getLOVIconElement(val, true);
                            var highlightChar = '';
                            if (!listIcon) {
                                highlightChar = d.substring(0, 1);
                            }
                            // If both , then show text and image
                            if (lType == "B") {
                                strDiv = "<div class='vis-grid-td-icon-grp'>";

                                if (listIcon) {
                                    strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'> " + listIcon + "</div> ";
                                }
                                else {
                                    strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'><span>" + highlightChar + "</span></div>";
                                }
                                strDiv += "<span> " + d + "</span ><div>";
                            }
                            // if Text, then show text only
                            else if (lType == "T") {
                                return d;
                            }
                            //Show icon only
                            else if (lType == "I") {
                                strDiv = "<div class='vis-grid-td-icon-grp' style='Justify-Content:center'>";
                                if (listIcon) {
                                    strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'> " + listIcon + "</div> ";
                                }
                                else {
                                    strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'><span>" + highlightChar + "</span></div>";
                                }
                                strDiv += "<div>";
                            }
                        }

                        else
                            // Based on sequence of image in idenitifer, perform logic and display image with text
                            if (l && l.gethasImageIdentifier()) {
                                var imgIndex = d.indexOf("Images/");

                                if (imgIndex == -1)
                                    return d;

                                //Find Image from Identifier string 
                                var img = d.substring(imgIndex + 7, d.lastIndexOf("^^"));
                                img = VIS.Application.contextUrl + "Images/Thumb32x32/" + img;

                                //Replace Image string with ^^^, so that ^^^ can be used to split Rest of identifer value
                                d = d.replace("^^" + d.substring(imgIndex, d.lastIndexOf("^^") + 2), "^^^")
                                if (d.indexOf("Images/") > -1)
                                    d = d.replace(d.substring(imgIndex, d.lastIndexOf("^^") + 2), "^^^");

                                d = d.split("^^^");

                                //Start HTMl string to be rendered inside Cell
                                strDiv = "<div class='vis-grid-td-icon-grp'>";
                                var highlightChar = '';

                                //Now 'd' may contains identifier values to be displayed before and after image
                                for (var c = 0; c < d.length; c++) {
                                    if (d[c].trim().length > 0) {
                                        //If highlightChar is not found, then get it from first item encounterd.
                                        if (highlightChar.length == 0)
                                            highlightChar = d[c].trim().substring(0, 1).toUpper();
                                        //If image contains nothing.png that means image not found in identfier and 
                                        //we will Display highlightChar
                                        if (c > 0 && img.indexOf("nothing.png") > -1 && highlightChar.length > 0) {
                                            strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'><span>" + highlightChar + "</span></div>";
                                        }
                                        strDiv += "<span>" + d[c] + "</span>";
                                    }
                                    //If image found, then display that image.
                                    if (c == 0 || img.indexOf("nothing.png") > -1) {
                                        if (img.indexOf("nothing.png") == -1) {
                                            strDiv += "<div class='" + oColumns[colIndex]['customClass'] + " vis-grid-row-td-icon'"
                                                + " > <img src='" + img +
                                                "'></div > ";
                                            // "' onerror='this.style.display=\"none\"' ></img></div > ";
                                        }

                                    }
                                }
                                +"</div > ";

                            }


                        if (strDiv == "")
                            return d;



                        return strDiv;
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

                    var customStyle = oColumns[colIndex].gridField.getGridImageStyle();
                    var winNo = oColumns[colIndex].gridField.getWindowNo();
                    var customClass;
                    if (customStyle) {
                        customClass = oColumns[colIndex]['customClass'];
                        if (!customClass) {
                            oColumns[colIndex]['customClass'] = 'vis-grd-custom-' + oColumns[colIndex].gridField.getAD_Column_ID() + winNo;
                            customClass = '.vis-grd-custom-' + oColumns[colIndex].gridField.getAD_Column_ID() + winNo + "{" + customStyle + "}";
                            var styleTag = document.createElement('style');
                            styleTag.type = 'text/css';
                            styleTag.innerHTML = customClass;
                            $($('head')[0]).append(styleTag);
                        }
                    }

                    var val = record["imgurlcolumn" + f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (!val) {
                        val = '<div class="vis-grid-row-td-icon-center">-</div>';
                        return val;
                    }
                    //return VIS.Msg.getElement1('AD_Image_ID') + '-' + val;
                    val = val.toString().replace("Images/", "Images/Thumb32x32/");
                    //var img = $('<img>').attr("src", VIS.Application.contextUrl + val);
                    var img;
                    if (customClass) {
                        img = '<div class="vis-grid-row-td-icon-center"><div class="' + oColumns[colIndex]['customClass'] + ' vis-grid-row-td-icon"><img src="' + VIS.Application.contextUrl + val + '"></div></div>';
                    }
                    else {
                        img = '<div class="vis-grid-row-td-icon-center"><div class="vis-grid-row-td-icon"><img src="' + VIS.Application.contextUrl + val + '"></div></div>';
                    }
                    return img;
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

            if (!oColumn.editable) {
                oColumn.editable = { type: 'custom', ctrl: iControl };
            }


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
                    //if (oColumns[p].gridField.getIsSwitch()) {
                    //    oColumns[p].editable = { type: 'checkbox' };
                    //}

                    if (this.hyperLinkCell == null) {
                        this.hyperLinkCell = oColumns[p].columnName;
                        oColumns[p].style = 'text-decoration:underline; color:rgba(var(--v-c-primary), 1) !important; cursor:pointer';
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
            onRowAdd: this.onRowAdd,
            onCellStyleRender: this.cellStyleRender
            //onResize: function () { alert('resize') }

        });
        var self = this;

        //this.grid.selectType = 'cell';;
        return size;
    };

    /**
     * return current cell-row value as string
     * called form Evaluator class
     * @param {any} field name of field /column 
     */
    VTable.prototype.getValueAsString = function (field) {
        var record = this.grid.records[this.cellRowIndex];
        var data = this.grid.parseField(record, field.toLowerCase());
        if (!data)
            return '';
        return data.toString();
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
        console.log("refresh");
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

    //VTable.prototype.setDefaultFocusField = function (field) {
    //    this.defaultFocusField = field;
    //};

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
            if (action === VIS.VTable.prototype.ROW_ADD && !this.aPanel.curGC.getIsCardRow() && !this.aPanel.curGC.getIsSingleRow()
                && !this.aPanel.curGC.getIsMapRow()) {
                this.setDefaultFocus();
            }
        }

        this.blockSelect = false;
    };

    //Set Default Focus for grid... Not in use Yet.
    VTable.prototype.setDefaultFocus = function (colName) {
       
        if (!this.mTab.defaultFocusField)
            return;
        if (!colName)
            colName = this.mTab.defaultFocusField.getColumnName();
        var selIndices = this.grid.getSelection();  //this.grid.getSelection(true);
        var colIndex = this.grid.getColumn(colName.toLower(), true)
        if (selIndices && selIndices.length > 0) {
            window.setTimeout(function () {
                this.grid.editField(selIndices[0], colIndex);
            }.bind(this), 200);
            //this.grid.dblClick(selIndices[colIndex], { metaKey: true });
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