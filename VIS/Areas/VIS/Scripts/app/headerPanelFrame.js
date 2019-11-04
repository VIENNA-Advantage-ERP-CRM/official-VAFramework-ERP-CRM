; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        var $parent = null;
        this.headerItems = null;
        var $self = this;
        this.gTab = null;
        this.controls = [];
        var setHeaderLayout = function (_gTab, $parentRoot) {
            if (_gTab.getIsHeaderPanel()) {
                $parent = $parentRoot;
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;

                if ($self.headerItems) {
                    var tHeight = $self.gTab.getHeaderHeight();

                    $root = $('<div style="display:grid">');

                    var rows = $self.gTab.getHeaderTotalRow();
                    var columns = $self.gTab.getHeaderTotalColumn();
                    var backColor = $self.gTab.getHeaderBackColor();
                    var alignment = $self.gTab.getHeaderBackColor();

                    $root.css(
                        {
                            'grid-template-columns': 'repeat(' + columns + ', auto)',
                            'grid-template-rows': 'repeat(' + rows + ', auto)',
                        });

                    $self.setHeaderItems();
                    $parentRoot.css('background-color', 'rgba(var(--v-c-primary))');
                    $parentRoot.append($root);
                }
            }
        };


        this.setHeaderItems = function () {

            if ($self.controls && $self.controls.length > 0) {


                for (var i = 0; i < $self.controls.length; i++) {
                    var objControl = $self.controls[i];
                    if (objControl) {
                        var controls = objControl["control"];
                        var mField = objControl["field"];
                        var iControl = controls["control"];

                        if (iControl == null && !mField.getIsHeading()) {
                            continue;
                        }

                        var colValue = getFieldValue(mField);
                        iControl.setValue(w2utils.encodeTags(colValue));
                    }
                }
            }
            else {
                var fields = $self.gTab.gridTable.gridFields;
                //$root.empty();
                for (var i = 0; i < fields.length; i++) {
                    var mField = fields[i];
                    if (mField.getIsHeaderPanelitem()) {
                        var controls = {};
                        var headerSeqNo = mField.getHeaderSeqno();
                        var headerItem = $self.headerItems[headerSeqNo];
                        var startCol = headerItem.StartColumn;
                        var colSpan = headerItem.ColumnSpan;
                        var startRow = headerItem.StartRow;
                        var rowSpan = headerItem.RowSpan;
                        var $div = null;

                        var $divIcon = null;

                        var $divLabel = null;

                        var $label = null;
                        var iControl = null;

                        $div = $('<div class="vis-w-p-header-data-f" style="grid-column: ' + startCol + '/ span ' + colSpan + '; grid-row: ' + startRow + '/ span ' + rowSpan + '"/>');

                        $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                        $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');

                        $label = VIS.VControlFactory.getLabel(mField);

                        iControl = VIS.VControlFactory.getReadOnlyControl($self.gTab, mField, false, false, false);

                        controls["control"] = iControl;
                        $self.controls.push({ "control": controls, "field": mField });

                        //var $spanIcon = $('<span><i></i><img></span>');
                        var $spanIcon = $('<span></span>');
                        var icon = VIS.VControlFactory.getIcon(mField);
                        if (icon && mField.getShowIcon()) {
                            $spanIcon.append(icon);
                        }

                        if (iControl == null && !mField.getIsHeading()) {
                            continue;
                        }

                        var $lblControl = $label.getControl();
                        var colValue = getFieldValue(mField);

                        iControl.setValue(w2utils.encodeTags(colValue));
                        $divIcon.append($spanIcon);
                        $divLabel.append($lblControl).append(iControl.getControl());
                        $div.append($divIcon).append($divLabel);
                        $root.append($div);
                    }
                }
            }

        };

        var getFieldValue = function (mField) {
            var colValue = mField.getValue();

            if (!mField.getIsDisplayed())
                return  "";
            if (colValue) {
                var displayType = mField.getDisplayType();

                if (mField.lookup) {
                    colValue = mField.lookup.getDisplay(colValue, true);
                }
                //	Date
                else if (VIS.DisplayType.IsDate(displayType)) {
                    colValue = new Date(colValue).toLocaleString();
                }
                //	YesNo
                else if (displayType == VIS.DisplayType.YesNo) {
                    var str = colValue.toString();
                    if (mField.getIsEncryptedColumn())
                        str = VIS.secureEngine.decrypt(str);
                    colValue = str.equals("Y");	//	Boolean
                }
                //	LOB 
                else
                    colValue = colValue.toString();//string
                //	Encrypted
                if (mField.getIsEncryptedColumn() && displayType != VIS.DisplayType.YesNo)
                    colValue = VIS.secureEngine.decrypt(colValue);
            }
            else {
                colValue = null;
            }

            return colValue;
        }

        this.init = function (gTab, $parentRoot) {
            setHeaderLayout(gTab, $parentRoot);
        };

        this.getRoot = function () {
            return $root;
        };

    };

    HeaderPanel.prototype.navigate = function () {
        this.setHeaderItems();
    };

    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));