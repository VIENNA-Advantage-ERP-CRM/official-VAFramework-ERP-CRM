; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        this.headerItems = null;
        var $self = this;
        this.gTab = null;
        this.controls = [];
        var textAlignEnum = { "C": "Center", "R": "Right", "L": "Left" };
        var alignItemEnum = { "C": "Center", "T": "flex-start", "B": "flex-end" };

        var setHeaderLayout = function (_gTab, $parentRoot) {
            //if Tab is market as Header Panel, only then execute further code.
            if (_gTab.getIsHeaderPanel()) {
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;

                if ($self.headerItems) {
                    var rows = $self.gTab.getHeaderTotalRow();
                    var columns = $self.gTab.getHeaderTotalColumn();
                    var backColor = $self.gTab.getHeaderBackColor();
                    var alignment = $self.gTab.getHeaderHorizontal();
                    var height = $self.gTab.getHeaderHeight();
                    var width = $self.gTab.getHeaderWidth();

                    /*Set Alignment and Height/Width of Header Panel
                    * Default Height is 150 px
                    * Default Width is 250px
                    */
                    if (alignment) {
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
        this.windowNo = 0;


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
                        var justyFy = headerItem.JustifyItems;
                        var alignItem = headerItem.AlignItems;

                        $self.windowNo = $self.gTab.getWindowNo();
                        var $div = null;

                        var $divIcon = null;

                        var $divLabel = null;

                        var $label = null;
                        var iControl = null;

                        //Apply HTML Style
                        var dynamicClassName = applyCustomUISettings(mField, headerSeqNo, startCol, colSpan, startRow, rowSpan);


                        $div = $('<div class="vis-w-p-header-data-f ' + dynamicClassName + '">');

                        $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                        $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');

                        // If Referenceof field is Image then added extra class to align image and Label in center.
                        if (mField.getDisplayType() == VIS.DisplayType.Image) {
                            $divLabel.addClass('vis-w-p-header-Label-center-f');
                            var dynamicClassForImageJustyfy = justifyAlignImageItems(headerSeqNo, justyFy, alignItem);
                            $divLabel.addClass(dynamicClassForImageJustyfy);
                        }
                        else {
                            if (justyFy || alignItem) {
                                var dynamicClassForJustyfy = justifyAlignTextItems(headerSeqNo, justyFy, alignItem);
                                $divLabel.addClass(dynamicClassForJustyfy)
                            }
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
                            $divIcon.append($spanIcon.append(icon));
                            $divLabel.append($lblControl);
                        }
                        else if (mField.getHeaderIconOnly() && mField.getHeaderHeadingOnly()) {
                            $div.append($divLabel);
                        }
                        else if (mField.getHeaderIconOnly()) {
                            $div.append($divIcon);
                            $divIcon.append($spanIcon.append(icon));
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

        /**
         * Create CSS Class and Addd to dome and Apply to element.
         * @param {any} mField
         * @param {any} headerSeqNo
         * @param {any} startCol
         * @param {any} colSpan
         * @param {any} startRow
         * @param {any} rowSpan
         */
        var applyCustomUISettings = function (mField, headerSeqNo, startCol, colSpan, startRow, rowSpan) {
            var headerStyle = mField.getHeaderStyle();
            var style = document.createElement('style');

            var dynamicClassName = "clsFieldGroup_" + headerSeqNo + "_" + $self.windowNo;


            $(style).attr('id', dynamicClassName);

            style.type = 'text/css';
            if (headerStyle) {
                style.innerHTML = "." + dynamicClassName + " {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";" + headerStyle + "}";
            }
            else {
                style.innerHTML = "." + dynamicClassName + "  {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + "}";
            }
            $($('head')[0]).append(style);

            return dynamicClassName;
        };

        var justifyAlignTextItems = function (headerSeqNo, justify, alignItem) {
            var style = document.createElement('style');
            var dynamicClassName = "vis-w-p-header-label-justify_" + headerSeqNo + "_" + $self.windowNo;
            $(style).attr('id', dynamicClassName);
            style.type = 'text/css';
            style.innerHTML = "." + dynamicClassName + " {text-align:" + textAlignEnum[justify] + ";align-items:" + alignItemEnum[alignItem] + "}";
            $($('head')[0]).append(style);
            return dynamicClassName;
        };

        var justifyAlignImageItems = function (headerSeqNo, justify, alignItem) {
            var style = document.createElement('style');
            var dynamicClassName = "vis-w-p-header-label-center-justify_" + headerSeqNo + "_" + $self.windowNo;
            $(style).attr('id', dynamicClassName);
            style.type = 'text/css';
            style.innerHTML = "." + dynamicClassName + " {justify-content:" + textAlignEnum[justify] + ";align-items:" + alignItemEnum[alignItem] + "}";
            $($('head')[0]).append(style);
            return dynamicClassName;
        };

        /**
         * Get value for current field for current field
         * @param {any} mField
         */
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

        /**
         * Dispose component
         * */
        this.disposeComponent = function () {
            var keys = Object.keys(this.headerItems);

            // Find Dynamically added classes from DOM and remove them.
            for (var i = 0; i < keys.length; i++) {
                $('#clsFieldGroup_' + keys[i] + "_" + this.windowNo).remove();
            }


            this.headerItems = null;
            $self = null;
            this.gTab = null;
            this.controls = null;
            $root.remove();
            $root = null;

        };

    };

    HeaderPanel.prototype.navigate = function () {
        this.setHeaderItems();
    };

    HeaderPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));