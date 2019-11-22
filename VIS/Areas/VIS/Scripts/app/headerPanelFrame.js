; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        this.headerItems = null;
        var $self = this;
        this.gTab = null;
        this.controls = [];
        var textAlignEnum = { "C": "Center", "R": "Right", "L": "Left" };
        var alignItemEnum = { "C": "Center", "T": "flex-start", "B": "flex-end" };
        this.windowNo = 0;

        /**
         * This function will check if tab is marked as header panel, then start creating header panel
         * and call next method to load items of header panel.
         * @param {any} _gTab
         * @param {any} $parentRoot
         */
        var setHeaderLayout = function (_gTab, $parentRoot) {
            //if Tab is market as Header Panel, only then execute further code.
            if (_gTab.getIsHeaderPanel()) {
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;
                $self.windowNo = $self.gTab.getWindowNo();

                if ($self.headerItems) {
                    var alignmentHorizontal = $self.gTab.getHeaderHorizontal();
                    // Create Root for header Panel
                    $root = $('<div>');
                    var rootCustomStyle = headerUISettings(columns, rows, alignmentHorizontal);
                    $root.addClass(rootCustomStyle);

                    if (alignmentHorizontal) {
                        $parentRoot.removeClass("vis-ad-w-p-header-l").addClass("vis-ad-w-p-header-t");
                    }

                    for (var j = 0; j < $self.headerItems.length; j++) {

                        var currentItem = $self.headerItems[j];

                        var rows = currentItem.HeaderTotalRow;
                        var columns = currentItem.HeaderTotalColumn;
                        var backColor = currentItem.HeaderBackColor;

                        //var height = currentItem.getHeaderHeight();
                        //var width = currentItem.getHeaderWidth();

                        var headerCustom = headerParentCustomUISettings("", backColor, "", alignmentHorizontal, j);
                        $parentRoot.addClass(headerCustom);

                        var $containerDiv = $('<div style="display:grid;grid-template-columns:repeat(' + columns + ', 1fr);grid-template-rows:repeat(' + rows + ', 1fr);background-color:' + backColor + '">');
                        $root.append($containerDiv);

                        //Load Header Panel Items and add them to UI.
                        $self.setHeaderItems(currentItem, $containerDiv);
                    }

                    // Add Header Panel to Parent Control
                    $parentRoot.append($root);
                }
            }
        };


        /**
         * This method create headr panel items when user open header panel first time. After that when user change record, system simply change values of label
         * and icons.
         * */
        this.setHeaderItems = function (currentItem, $containerDiv) {

            /*If controls are already loaded, then do not manipulate DOM.Only fetch there reference from DOM and Change Values.
             *Else create header panel items. 
             */
            if ($self.controls && $self.controls.length > 0 && !currentItem && !$containerDiv) {
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

                fields = $.grep(fields, function (item) {
                    if (item.getIsHeaderPanelitem()) {
                        return item;
                    }
                });

                fields = fields.sort(function (a, b) { return a.getHeaderSeqno() - b.getHeaderSeqno() });

                for (var i = 0; i < fields.length; i++) {
                    var mField = fields[i];
                    // Check if field is marked as Header Panel Item or Not.
                    if (mField.getIsHeaderPanelitem()) {
                        var controls = {};
                        var headerSeqNo = mField.getHeaderSeqno();
                        var headerItem = currentItem.HeaderItems[headerSeqNo];

                        if (!headerItem || headerItem.length <= 0) {
                            continue;
                        }
                        var startCol = headerItem.StartColumn;
                        var colSpan = headerItem.ColumnSpan;
                        var startRow = headerItem.StartRow;
                        var rowSpan = headerItem.RowSpan;
                        var justyFy = headerItem.JustifyItems;
                        var alignItem = headerItem.AlignItems;


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
                        $label = VIS.VControlFactory.getLabel(mField, true);

                        iControl = VIS.VControlFactory.getReadOnlyControl($self.gTab, mField, false, false, false);

                        // Create object of controls and push object and Field in Array
                        // THis array is used when user navigate from one record to another.
                        controls["control"] = iControl;
                        $self.controls.push({ "control": controls, "field": mField });

                        var $spanIcon = $('<span></span>');
                        var icon = VIS.VControlFactory.getIcon(mField);

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
                        $containerDiv.append($div);
                    }
                }
            }

        };

        /**
         * Create class that iclude  settings to create Root grid of header panel.
         * @param {any} columns
         * @param {any} rows
         */
        var headerUISettings = function (columns, rows, alignmentHorizontal) {
            var style = document.createElement('style');
            var dynamicClassName = "vis-ad-w-p-header_root_" + $self.windowNo;
            $(style).attr('id', dynamicClassName);
            style.type = 'text/css';

            //style.innerHTML = "." + dynamicClassName + " {overflow:auto;display:grid;flex:1;grid-template-columns:" + "repeat(" + columns + ", 1fr)" + ";grid-template-rows:" + "repeat(" + rows + ", 1fr)" + "}";
            style.innerHTML = "." + dynamicClassName + " {overflow:auto;display:flex;flex:1;";

            if (alignmentHorizontal) {
                style.innerHTML += "flex-direction:row";
            }
            else {
                style.innerHTML += "flex-direction:column";
            }

            style.innerHTML += "}"
            $($('head')[0]).append(style);
            return dynamicClassName;
        };

        /**
         * Create Class that include settings that would be applied on parent of root classs.
         * @param {any} width
         * @param {any} backColor
         * @param {any} height
         * @param {any} alignment
         */
        var headerParentCustomUISettings = function (width, backColor, height, alignmentHorizontal, seqNo) {
            var style = document.createElement('style');
            var dynamicClassName = "vis-ad-w-p-header_Custom_" + $self.windowNo + "_" + seqNo;

            $(style).attr('id', dynamicClassName);
            style.type = 'text/css';

            style.innerHTML = "." + dynamicClassName + " {flex:1;";

            /*Set Alignment and Height/Width of Header Panel
                   * Default Height is 150 px
                   * Default Width is 250px
                   */
            if (alignmentHorizontal) {
                style.innerHTML += "height: " + height + ";";
            }
            else {
                style.innerHTML += "width: " + width + ";";
            }

            //Set background Color of Header Panel. If no color found then get color from Theme
            if (backColor) {
                style.innerHTML += 'background-color: ' + backColor;
            }
            else {
                style.innerHTML += 'background-color: ' + 'rgba(var(--v-c-primary))';
            }

            style.innerHTML += "}";

            $($('head')[0]).append(style);

            return dynamicClassName;
        };

        /**
         * Created CSS Class that will be applied to Field group( Parent div of ICON, label and value)
         * Create row, rowspan , column, column span, and custom header style defined at field level.
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

            var dynamicClassName = "vis-hp-FieldGroup_" + startRow + "_" + startCol + "_" + $self.windowNo;


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

        /**
         * This method set justfy and alignment of text fields
         * @param {any} headerSeqNo
         * @param {any} justify
         * @param {any} alignItem
         */
        var justifyAlignTextItems = function (headerSeqNo, justify, alignItem) {
            var style = document.createElement('style');
            var dynamicClassName = "vis-w-p-header-label-justify_" + headerSeqNo + "_" + $self.windowNo;
            $(style).attr('id', dynamicClassName);
            style.type = 'text/css';
            style.innerHTML = "." + dynamicClassName + " {text-align:" + textAlignEnum[justify] + ";align-items:" + alignItemEnum[alignItem] + "}";
            $($('head')[0]).append(style);
            return dynamicClassName;
        };

        /**
        * This method set justfy and alignment of Image Field
        * @param {any} headerSeqNo
        * @param {any} justify
        * @param {any} alignItem
        */
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
                    if (displayType == VIS.DisplayType.DateTime) {
                        colValue = new Date(colValue).toLocaleString();
                    }
                    else if (displayType == VIS.DisplayType.Date) {
                        colValue = new Date(colValue).toLocaleDateString();
                    }
                    else {
                        colValue = (new Date(colValue).toLocaleTimeString());
                    }
                }
                //	YesNo
                else if (displayType == VIS.DisplayType.YesNo) {
                    var str = colValue.toString();
                    if (mField.getIsEncryptedColumn())
                        str = VIS.secureEngine.decrypt(str);
                    colValue = str.equals("true");	//	Boolean
                }
                //	LOB 
                else
                    colValue = colValue.toString();//string

                //	Encrypted
                // If field is marked encrypted, then replace all text of field with *.
                if (mField.getIsEncryptedField()) {
                    if (colValue && colValue.length > 0) {
                        colValue = colValue.replace(/[a-zA-Z0-9-. ]/g, '*').replace(/[^a-zA-Z0-9-. ]/g, '*');
                    }
                }
            }
            else {
                colValue = null;
            }

            return colValue;
        }

        this.init = function (gTab, $parentRoot) {
            setHeaderLayout(gTab, $parentRoot);
        };

        /**
         * 
         * Return root div of header panel*/
        this.getRoot = function () {
            return $root;
        };

        /**
         * Dispose component
         * */
        this.disposeComponent = function () {
            var keys = Object.keys(this.headerItems);
            // Find Dynamically added classes from DOM and remove them.
            //for (var i = 1; i <= keys.length; i++) {
            //    var startCol = this.headerItems[i].StartColumn;
            //    var startRow = this.headerItems[i].StartRow;
            //    $('#vis-hp-FieldGroup_' + startRow + "_" + startCol + "_" + this.windowNo).remove();

            //    $("#vis-w-p-header-label-justify_" + keys[i] + "_" + this.windowNo).remove();

            //    $("#vis-w-p-header-label-center-justify_" + keys[i] + "_" + this.windowNo).remove();
            //}
            this.headerItems = null;
            $self = null;
            this.gTab = null;
            this.controls = null;
            $root.remove();
            $root = null;

        };

    };
    /**
     * This method will be invoked on record change in window.
     * */
    HeaderPanel.prototype.navigate = function () {
        this.setHeaderItems();
    };

    /**
     * this method will be invoked on window close.
     * */
    HeaderPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));