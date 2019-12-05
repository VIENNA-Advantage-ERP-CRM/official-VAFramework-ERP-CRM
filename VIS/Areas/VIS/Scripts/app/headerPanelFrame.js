; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        this.headerItems = null;
        var $self = this;
        this.gTab = null;
        this.controls = [];
        this.textAlignEnum = { "C": "Center", "R": "flex-end", "L": "flex-start" };
        this.alignItemEnum = { "C": "Center", "T": "flex-start", "B": "flex-end" };
        this.windowNo = 0;
        this.dynamicStyle = [];
        this.styleTag = document.createElement('style');


        /**
         * This function will check if tab is marked as header panel, then start creating header panel
         * and call next method to load items of header panel.
         * @param {any} _gTab
         * @param {any} $parentRoot
         */
        this.setHeaderLayout = function (_gTab, $parentRoot) {
            //if Tab is market as Header Panel, only then execute further code.
            if (_gTab.getIsHeaderPanel()) {
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;
                $self.windowNo = $self.gTab.getWindowNo();

                if ($self.headerItems) {
                    // Create Root for header Panel
                    $root = $('<div class="vis-ad-w-p-header_root_common">');
                    var headerCustom = this.headerParentCustomUISettings("");
                    $parentRoot.addClass(headerCustom);
                  
                   
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

                        if (colValue || mField.getDisplayType() == VIS.DisplayType.Image)
                            iControl.setValue(w2utils.encodeTags(colValue));
                        else {
                            iControl.setValue("&nbsp;", true);
                        }

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
                        var backgroundColor = headerItem.BackgroundColor;
                        if (!backgroundColor) {
                            backgroundColor = '';
                        }
                        var FontColor = headerItem.FontColor;
                        if (!FontColor) {
                            FontColor = '';
                        }
                        var fontSize = headerItem.FontSize;
                        if (!fontSize) {
                            fontSize = '';
                        }


                        var $div = null;
                        var $divIcon = null;
                        var $divLabel = null;
                        var $label = null;
                        var iControl = null;

                        //Apply HTML Style
                        var dynamicClassName = this.applyCustomUISettings(headerSeqNo, startCol, colSpan, startRow, rowSpan, justyFy, alignItem,
                            backgroundColor, FontColor, fontSize);

                        $div = $('<div class="vis-w-p-header-data-f ' + dynamicClassName + '">');

                        $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                        $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');

                        // If Referenceof field is Image then added extra class to align image and Label in center.
                        if (mField.getDisplayType() == VIS.DisplayType.Image) {
                            $divLabel.addClass('vis-w-p-header-Label-center-f');
                            var dynamicClassForImageJustyfy = this.justifyAlignImageItems(headerSeqNo, justyFy, alignItem);
                            $divLabel.addClass(dynamicClassForImageJustyfy);
                        }
                        else {
                        }

                        // Get Controls to be displayed in Header Panel
                        $label = VIS.VControlFactory.getLabel(mField, true);

                        iControl = VIS.VControlFactory.getReadOnlyControl($self.gTab, mField, false, false, false);

                        var dynamicFieldValue = this.applyCustomUIForFieldValue(headerSeqNo, startCol, startRow, mField);

                        iControl.getControl().addClass(dynamicFieldValue);

                        // Create object of controls and push object and Field in Array
                        // THis array is used when user navigate from one record to another.
                        controls["control"] = iControl;
                        $self.controls.push({ "control": controls, "field": mField });

                        var $spanIcon = $('<span></span>');
                        var icon = VIS.VControlFactory.getIcon(mField);

                        if (iControl == null) {
                            continue;
                        }
                        var $lblControl = $label.getControl().addClass('vis-w-p-header-data-label');
                        var colValue = getFieldValue(mField);
                        if (colValue || mField.getDisplayType() == VIS.DisplayType.Image) {
                            iControl.setValue(w2utils.encodeTags(colValue));
                        }
                        else {
                            iControl.setValue("&nbsp;", true);
                        }

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
         * Get value for current field for current field
         * @param {any} mField
         */
        var getFieldValue = function (mField) {
            var colValue = mField.getValue();

            //if (!mField.getIsDisplayed())
            //    return "";
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
            this.styleTag.remove();
            this.styleTag = null;
            this.headerItems = null;
            $self = null;
            this.gTab = null;
            this.controls = null;
            $root.remove();
            $root = null;

        };

    };

    HeaderPanel.prototype.init = function (gTab, $parentRoot, callBack) {
        this.setHeaderLayout(gTab, $parentRoot);
        var root = this.getRoot();
        var rootClass = "vis-w-p-Header-Root-v";//Fixed Class for vertical Alignment
        var alignmentHorizontal = this.gTab.getHeaderHorizontal();
        var height = this.gTab.getHeaderHeight();
        var width = this.gTab.getHeaderWidth();
        var backColor = this.gTab.getHeaderBackColor();
        var padding = this.gTab.getHeaderPadding();
        var $slider = $parentRoot.find('.fa-angle-double-left');

        var rootCustomStyle = this.headerUISettings(alignmentHorizontal, height, width, backColor, padding);
        root.addClass(rootCustomStyle);
        $parentRoot.css("flex-direction", "column");
        $slider.parent().css('display', 'flex');
        if (alignmentHorizontal) {
            $parentRoot.removeClass("vis-ad-w-p-header-l").addClass("vis-ad-w-p-header-t");
            rootClass = 'vis-w-p-Header-Root-h';//Fixed Class for Horizontal Alignment
            $slider.removeClass('fa-angle-double-left').addClass('fa-angle-double-up');
            $slider.parent().css('background-color', 'transparent');
            $parentRoot.css('flex-direction', 'row');
        }

        for (var j = 0; j < this.headerItems.length; j++) {

            var currentItem = this.headerItems[j];

            var rows = currentItem.HeaderTotalRow;
            var columns = currentItem.HeaderTotalColumn;
            var backColor = currentItem.HeaderBackColor;
            var padding = currentItem.HeaderPadding;

            if (!backColor) {
                backColor = '';
            }

            if (!padding) {
                padding = '';
            }

            var dymcClass = this.fieldGroupContainerUISettings(columns, rows, backColor, padding, j);

            var $containerDiv = $('<div class="' + rootClass + ' ' + dymcClass + '">');
            root.append($containerDiv);

            //Load Header Panel Items and add them to UI.
            this.setHeaderItems(currentItem, $containerDiv);
        }
        this.addStyleToDom();

        // Add Header Panel to Parent Control
        $parentRoot.append(root);

        function eventHandling() {
            $slider.on("click", function () {
                if (alignmentHorizontal) {
                    if ($parentRoot.height() == 0) {
                        $parentRoot.height(height);
                        root.show();
                        $slider.removeClass('fa-angle-double-down').addClass('fa-angle-double-up').removeClass('vis-ad-w-p-header-v');
                    }
                    else {
                        $parentRoot.height(0);
                        root.hide();
                        $slider.removeClass('fa-angle-double-up').addClass('fa-angle-double-down').addClass('vis-ad-w-p-header-v');
                    }
                }
                else {
                    if ($parentRoot.width() == 0) {
                        $parentRoot.width(width);
                        root.show();
                        $slider.removeClass('fa-angle-double-right').addClass('fa-angle-double-left').removeClass('vis-ad-w-p-header-h');
                    }
                    else {
                        $parentRoot.width(0);
                        root.hide();
                        $slider.removeClass('fa-angle-double-left').addClass('fa-angle-double-right').addClass('vis-ad-w-p-header-h');
                    }
                }
            });
        };

        eventHandling();

    };



    /**
         * Create class that include  settings to be applied on Field Group Container
         * @param {any} columns
         * @param {any} rows
         * @param {any} backcolor
         * @param {any} padding
         * @param {any} itemNo
         */
    HeaderPanel.prototype.fieldGroupContainerUISettings = function (columns, rows, backcolor, padding, itemNo) {
        var dynamicClassName = "vis-ad-w-p-fg_container_" + rows + "_" + columns + "_" + this.windowNo + "_" + itemNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {");
        this.dynamicStyle.push('grid-template-columns:repeat(' + columns + ', 1fr);grid-template-rows:repeat(' + rows + ', auto);background-color:' + backcolor + ';padding:' + padding);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };



    /**
         * Added dynamic style to DOM
         * */
    HeaderPanel.prototype.addStyleToDom = function () {
        this.styleTag.type = 'text/css';
        this.styleTag.innerHTML = this.dynamicStyle.join(" ");
        $($('head')[0]).append(this.styleTag);
    };


    /**
         * Create class that iclude  settings to create Root grid of header panel.
         * @param {any} columns
         * @param {any} rows
         */
    HeaderPanel.prototype.headerUISettings = function (alignmentHorizontal, height, width, backcolor, padding) {
        var dynamicClassName = "vis-ad-w-p-header_root_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {display:flex;flex:1;");
        if (alignmentHorizontal) {
            this.dynamicStyle.push("flex-direction:row;height: " + height + "; ");
        }
        else {
            this.dynamicStyle.push("flex-direction:column;width: " + width + "; ");
        }
        this.dynamicStyle.push("background-color:" + backcolor + ";padding:" + padding);

        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    /**
        * Create Class that include settings that would be applied on parent of root classs.
        * @param {any} width
        * @param {any} backColor
        * @param {any} height
        * @param {any} alignment
        */
    HeaderPanel.prototype.headerParentCustomUISettings = function (backColor) {
        var dynamicClassName = "vis-ad-w-p-header_Custom_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {flex:1;");
        //Set background Color of Header Panel. If no color found then get color from Theme
        if (backColor) {
            this.dynamicStyle.push('background-color: ' + backColor);
        }
        else {
            this.dynamicStyle.push('background-color: ' + 'rgba(var(--v-c-primary))');
        }

        this.dynamicStyle.push("} ");
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
    HeaderPanel.prototype.applyCustomUISettings = function (headerSeqNo, startCol, colSpan, startRow, rowSpan, justify, alignment, backColor, fontColor, fontSize) {
        //var headerStyle = mField.getHeaderStyle();
        var dynamicClassName = "vis-hp-FieldGroup_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        //if (headerStyle) {
        //    this.dynamicStyle.push(" ." + dynamicClassName + " {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";" + headerStyle + ";");
        //}
        //else {
        this.dynamicStyle.push("." + dynamicClassName + "  {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";");
        //}

        this.dynamicStyle.push("justify-content:" + this.textAlignEnum[justify] + ";align-items:" + this.alignItemEnum[alignment]);
        this.dynamicStyle.push(";background-color:" + backColor + ";font-size:" + fontSize + ";color:" + fontColor);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    HeaderPanel.prototype.applyCustomUIForFieldValue = function (headerSeqNo, startCol, startRow, mField) {
        //var headerStyle = mField.getHeaderStyle();
        var dynamicClassName = "vis-hp-FieldValue_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        //if (headerStyle) {
        //    this.dynamicStyle.push(" ." + dynamicClassName + " {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";" + headerStyle + ";");
        //}
        //else {
        this.dynamicStyle.push("." + dynamicClassName + "  {" + mField.getHeaderStyle());
        //}

        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    /**
     * This method set justfy and alignment of Image Field
     * @param {any} headerSeqNo
     * @param {any} justify
     * @param {any} alignItem
     */
    HeaderPanel.prototype.justifyAlignImageItems = function (headerSeqNo, justify, alignItem) {
        var dynamicClassName = "vis-w-p-header-label-center-justify_" + headerSeqNo + "_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {justify-content:" + this.textAlignEnum[justify] + ";align-items:" + this.alignItemEnum[alignItem] + "}");
        return dynamicClassName;
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