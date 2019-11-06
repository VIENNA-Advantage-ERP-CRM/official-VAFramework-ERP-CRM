; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        this.headerItems = null;
        var $self = this;
        this.gTab = null;
        this.controls = [];
        var setHeaderLayout = function (_gTab, $parentRoot) {
            //if Tab is market as Header Panel, only then execute further code.
            if (_gTab.getIsHeaderPanel()) {
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;

                if ($self.headerItems) {
                    var rows = $self.gTab.getHeaderTotalRow();
                    var columns = $self.gTab.getHeaderTotalColumn();
                    var backColor = $self.gTab.getHeaderBackColor();
                    var alignment = $self.gTab.getHeaderAlignment();
                    var height = $self.gTab.getHeaderHeight() || '150px';
                    var width = $self.gTab.getHeaderWidth() || '250px';

                    /*Set Alignment and Height/Width of Header Panel
                    * Default Height is 150 px
                    * Default Width is 250px
                    */
                    if (alignment.equals("H")) {
                        $parentRoot.removeClass("vis-ad-w-p-header-l").addClass("vis-ad-w-p-header-t");
                        $parentRoot.height(height);
                    }
                    else {
                        $parentRoot.width(width);
                    }

                    //Set background Color of Header Panel. If no color found then get color from Theme
                    if (backColor) {
                        $parentRoot.css('background-color', backColor);
                    }
                    else {
                        $parentRoot.css('background-color', 'rgba(var(--v-c-primary))');
                    }

                    // Create Root for header Panel
                    $root = $('<div style="display:grid">');

                    // Add Rows and Columns to Header Panel.
                    $root.css(
                        {
                            'grid-template-columns': 'repeat(' + columns + ', auto)',
                            'grid-template-rows': 'repeat(' + rows + ', auto)',
                        });

                    //Load Header Panel Items and add them to UI.
                    $self.setHeaderItems();

                    // Add Header Panel to Parent Control
                    $parentRoot.append($root);
                }
            }
        };


        this.setHeaderItems = function () {

            /*If controls are already loaded, then do not manipulate DOM.Only fetch there reference from DOM and Change Values.
             *Else create header panel. 
             */
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
                for (var i = 0; i < fields.length; i++) {
                    var mField = fields[i];
                    // Check if field is marked as Header Panel Item or Not.
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

                        //var clsFieldGroup = $("<style type='text / css'> .clsFieldGroup" + headerSeqNo+" {'background-color':'green', 'grid-column': '" + startCol + "/ span " + colSpan + "'; 'grid-row': '" + startRow + "/ span " + rowSpan + "'} </style>");
                        //$('html > head').append(clsFieldGroup);

                        var headerStyle = mField.getHeaderStyle();

                        var style = document.createElement('style');
                        style.type = 'text/css';
                        style.innerHTML = ".clsFieldGroup" + headerSeqNo + " {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + "}";
                        $($('head')[0]).append(style);




                        $div = $('<div class="vis-w-p-header-data-f clsFieldGroup' + headerSeqNo +'">');

                        $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                        $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');

                        // If Referenceof field is Image then added extra class to align image and Label in center.
                        if (mField.getDisplayType() == VIS.DisplayType.Image) {
                            $divLabel.addClass('vis-w-p-header-Label-center-f');
                        }

                        // Get Controls to be displayed in Header Panel
                        $label = VIS.VControlFactory.getLabel(mField);
                        iControl = VIS.VControlFactory.getReadOnlyControl($self.gTab, mField, false, false, false);

                        // Create object of controls and push object and Field in Array
                        // THis array is used when user navigate from one record to another.
                        controls["control"] = iControl;
                        $self.controls.push({ "control": controls, "field": mField });

                        var $spanIcon = $('<span></span>');
                        var icon = VIS.VControlFactory.getIcon(mField);

                        //if (icon && mField.getShowIcon()) {
                        //    $spanIcon.append(icon);
                        //}

                        if (iControl == null) {
                            continue;
                        }

                        var $lblControl = $label.getControl();
                        var colValue = getFieldValue(mField);

                        iControl.setValue(w2utils.encodeTags(colValue));

                        /*Set what do you want to show? Icon OR Label OR Both OR None*/
                        if (!mField.getHeaderIconOnly() && !mField.getHeaderHeadingOnly()) {
                            $div.append($divIcon);
                            $divIcon.append($spanIcon);
                            $divLabel.append($lblControl);
                        }
                        else if (mField.getHeaderIconOnly() && mField.getHeaderHeadingOnly()) {
                            $div.append($divIcon);
                        }
                        else if (mField.getHeaderIconOnly()) {
                            $div.append($divIcon);
                            $divIcon.append($spanIcon);
                        }
                        else if (mField.getHeaderHeadingOnly()) {
                            $divLabel.append($lblControl);
                        }
                        /****END ******  Set what do you want to show? Icon OR Label OR Both OR None*/
                        $divLabel.append(iControl.getControl());
                        $div.append($divLabel);
                        $root.append($div);
                    }
                }
            }

        };

        var getFieldValue = function (mField) {
            var colValue = mField.getValue();

            if (!mField.getIsDisplayed())
                return "";
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