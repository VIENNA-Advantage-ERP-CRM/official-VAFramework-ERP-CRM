; (function (VIS, $) {

    function HeaderPanel($parentRoot) {
        var $root = null;
        var alignmentHorizontal = false;

        this.headerItems = null;
        var $self = this;
        this.curTab = null;
        this.controls = [];
        this.textAlignEnum = { "C": "Center", "R": "flex-end", "L": "flex-start" };
        this.alignItemEnum = { "C": "Center", "T": "flex-start", "B": "flex-end" };
        this.windowNo = 0;
        this.dynamicStyle = [];
        this.styleTag = document.createElement('style');

        var $slider = $parentRoot.find('.fa-angle-double-left');



        $parentRoot.css("flex-direction", "column");
        $slider.parent().css('display', 'flex');
        /**
         * This function will check if tab is marked as header panel, then start creating header panel
         * and call next method to load items of header panel.
         * @param {any} _gTab
         * @param {any} $parentRoot
         */
        this.setHeaderLayout = function (_gTab, backColor) {
            //if Tab is market as Header Panel, only then execute further code.
            if (_gTab.getIsHeaderPanel()) {
                $self.headerItems = _gTab.getHeaderPanelItems();
                $self.gTab = _gTab;
                $self.windowNo = $self.gTab.getWindowNo();

                if ($self.headerItems) {
                    // Create Root for header Panel
                    $root = $('<div class="vis-ad-w-p-header_root_common">');
                    var headerCustom = this.headerParentCustomUISettings(backColor);
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
            if (this.controls && this.controls.length > 0 && !currentItem && !$containerDiv) {
                for (var i = 0; i < this.controls.length; i++) {
                    var objControl = this.controls[i];
                    if (objControl) {
                        var controls = objControl["control"];
                        var mField = objControl["field"];
                        var iControl = controls["control"];

                        if (mField == null && iControl != null) { //dynamic

                            iControl.setValue(null);
                            continue;
                        }

                        if (iControl == null && !mField.getIsHeading()) {
                            continue;
                        }

                        var colValue = getFieldValue(mField, iControl);

                        if (iControl instanceof VIS.Controls.VButton) {
                            colValue = mField.getValue();
                            setValue(colValue, iControl, mField);
                        }

                        else if (mField.lookup && mField.lookup.gethasImageIdentifier()) {
                            colValue = VIS.Utility.Util.getIdentifierDisplayVal(colValue);

                            var $imgCtrl = objControl["img"];
                            var $imgSpanCtrl = objControl["imgspan"];

                            var img = null;
                            var imgStyle = null;

                            if (mField.lookup.displayType == VIS.DisplayType.List) {
                                img = mField.lookup.getLOVIconElement(mField.getValue(), true, true);
                                //Fetch style of icon for list from image window and apply style on that icon
                                imgStyle = mField.lookup.getLOVIconStyle(mField.getValue());
                                var imgSpan = "";
                                if (!img && colValue) {
                                    imgSpan = colValue.substring(0, 1);
                                }
                                if (img) {
                                    if (img.contains("Images/")) {
                                        $imgCtrl.attr('src', img).attr('style', imgStyle);
                                        $imgSpanCtrl.hide();
                                        $imgCtrl.show();
                                    }
                                    else {
                                        imgSpan = img;
                                        $imgSpanCtrl.empty();
                                        $imgSpanCtrl.append("<i class='" + imgSpan + "' style='" + imgStyle + "'></i>");
                                        $imgSpanCtrl.show();
                                        $imgCtrl.hide();
                                    }
                                }
                                else {
                                    if (!imgSpan)
                                        imgSpan = "";
                                    $imgSpanCtrl.empty();
                                    $imgSpanCtrl.text(imgSpan).attr('style', imgStyle);
                                    $imgSpanCtrl.show();
                                    $imgCtrl.hide();
                                }
                            }
                            else {
                                img = getIdentifierImage(mField);
                                var imgSpan;
                                if (img && !img.contains("Images/")) {
                                    imgSpan = img;//img contains First charater of Name or Identifier text
                                    $imgSpanCtrl.text(imgSpan);
                                    $imgSpanCtrl.show();
                                    $imgCtrl.hide();
                                }
                                else {
                                    $imgCtrl.attr('src', img);
                                    $imgSpanCtrl.hide();
                                    $imgCtrl.show();
                                }
                            }


                            setValue(colValue, iControl, mField);
                        }
                        else {
                            setValue(colValue, iControl, mField);
                        }
                    }
                }
            }
            else {

                if (!currentItem)
                    return;
                var fields = this.curTab.gridTable.gridFields;

                fields = $.grep(fields, function (item) {
                    if (item.getIsHeaderPanelitem()) {
                        return item;
                    }
                });

                fields = fields.sort(function (a, b) { return a.getHeaderSeqno() - b.getHeaderSeqno() });

                //loop through header item
                var headergFields = null;
                for (var headerSeqNo in currentItem.HeaderItems) {

                    var headerItem = currentItem.HeaderItems[headerSeqNo];

                    var startCol = headerItem.StartColumn;
                    var colSpan = headerItem.ColumnSpan;
                    var startRow = headerItem.StartRow;
                    var rowSpan = headerItem.RowSpan;
                    var justyFy = headerItem.JustifyItems;
                    var alignItem = headerItem.AlignItems;
                    var fieldPadding = headerItem.Padding;
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
                    //$divIconSpan = $('<span>');
                    //$divIconImg = $('<img>');
                    var $divLabel = null;
                    var $label = null;
                    var iControl = null;

                    //Apply HTML Style
                    var dynamicClassName = this.applyCustomUISettings(headerSeqNo, startCol, colSpan, startRow, rowSpan, justyFy, alignItem,
                        backgroundColor, FontColor, fontSize, fieldPadding);

                    // Find the div with dynamic class from container. Class will only be available in DOm if two fields are having same item seq. No.
                    $div = $containerDiv.find('.' + dynamicClassName);

                    //If div not found, then create new one.
                    if ($div.length <= 0)
                        $div = $('<div class="vis-w-p-header-data-f ' + dynamicClassName + '">');




                    // is dynamic 
                    if (headerItem.ColSql.length > 0) {
                        var controls = {};
                        $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');
                        iControl = new VIS.Controls.VKeyText(headerItem.ColSql, $self.gTab.getWindowNo(),
                            $self.gTab.getWindowNo() + "_" + headerSeqNo);

                        if (iControl == null) {
                            continue;
                        }

                        controls["control"] = iControl;
                        var objctrls = { "control": controls, "field": null };

                        $divLabel.append(iControl.getControl());
                        $div.append($divLabel);
                        // $div.append($divLabel);
                        $containerDiv.append($div);
                        $self.controls.push(objctrls);
                    }
                    else {
                        if (!headergFields) {
                            headergFields = {};
                            fields = fields.sort(function (a, b) { return a.getHeaderSeqno() - b.getHeaderSeqno() });
                            for (var i = 0; i < fields.length; i++) {
                                var field = fields[i];


                                // Check if field is marked as Header Panel Item or Not.
                                if (field.getIsHeaderPanelitem()) {
                                    if (field.getHeaderSeqno() in headergFields) {
                                        headergFields[field.getHeaderSeqno()].push(field);
                                    }
                                    else {
                                        headergFields[field.getHeaderSeqno()] = [field];
                                    }
                                }
                            }
                        }

                        var mFields = headergFields[headerSeqNo];
                        if (!mFields)
                            continue;
                        for (var x = 0; x < mFields.length; x++) {
                            var mField = mFields[x];
                            if (!mField)
                                continue;

                            var controls = {};
                            $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                            $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');
                            // If Referenceof field is Image then added extra class to align image and Label in center.
                            if (mField.getDisplayType() == VIS.DisplayType.Image) {
                                $divLabel.addClass('vis-w-p-header-Label-center-f');
                                var dynamicClassForImageJustyfy = this.justifyAlignImageItems(headerSeqNo, justyFy, alignItem);
                                $divLabel.addClass(dynamicClassForImageJustyfy);
                            }

                            // Get Controls to be displayed in Header Panel
                            $label = VIS.VControlFactory.getHeaderLabel(mField, true);
                            iControl = VIS.VControlFactory.getReadOnlyControl(this.curTab, mField, false, false, false);

                            if (mField.getDisplayType() == VIS.DisplayType.Button) {
                                if (iControl != null)
                                    iControl.addActionListner(this);
                            }

                            var dynamicFieldValue = this.applyCustomUIForFieldValue(headerSeqNo, startCol, startRow, mField);

                            iControl.getControl().addClass(dynamicFieldValue);

                            // Create object of controls and push object and Field in Array
                            // THis array is used when user navigate from one record to another.
                            controls["control"] = iControl;

                            var objctrls = { "control": controls, "field": mField };

                            var $spanIcon = $('<span></span>');
                            var icon = VIS.VControlFactory.getIcon(mField);
                            if (iControl == null) {
                                continue;
                            }

                            var $lblControl = null;
                            if ($label) {
                                $lblControl = $label.getControl().addClass('vis-w-p-header-data-label');
                            }

                            var colValue = getFieldValue(mField);

                            styleArr = mField.getHeaderStyle();
                            if (styleArr && styleArr.length > 0)
                                styleArr = styleArr.split("|");

                            if (styleArr && styleArr.length > 0) {
                                for (var j = 0; j < styleArr.length; j++) {
                                    if (styleArr[j].indexOf("@img::") > -1 || styleArr[j].indexOf("@span::") > -1) {
                                        $div.append($divIcon);
                                        var css = "";
                                        if (styleArr[j].indexOf("@img::") > -1) {
                                            css = styleArr[j].replace("@img::", "");
                                        }
                                        else if (styleArr[j].indexOf("@span::")) {
                                            css = styleArr[j].replace("@span::", "");
                                        }
                                        $divIcon.attr('style', css);
                                    }
                                    else if (styleArr[j].indexOf("@value::") > -1) {
                                        //var css = "";

                                        //css = styleArr[j].replace("@value::", "");
                                        //$divLabel.attr('style', css);
                                        $div.append($divLabel);
                                    }
                                    else if (styleArr[j].indexOf("<br>") > -1) {
                                        $div.css("flex-direction", "column");
                                    }
                                    else {
                                        $div.append($divIcon);
                                        $div.append($divLabel);
                                    }
                                }
                            }
                            else {
                                $div.append($divIcon);
                                $div.append($divLabel);
                            }

                            var $image = $('<img>');
                            var $imageSpan = $('<span>');
                            objctrls["img"] = $image;


                            if (mField.lookup && mField.lookup.gethasImageIdentifier()) {


                                objctrls["imgspan"] = $imageSpan;

                                var img = null;
                                var imgSpan = null;
                                var imgStyle = null;
                                var styleArr = null;
                                if (VIS.DisplayType.List == mField.lookup.displayType) {

                                    img = mField.lookup.getLOVIconElement(mField.getValue(), true);
                                    //Fetch style of icon for list from image window and apply style on that icon
                                    imgStyle = mField.lookup.getLOVIconStyle(mField.getValue());
                                    if (!img && colValue) {
                                        imgSpan = colValue.substring(0, 1);
                                    }
                                    if (img && !img.contains("Images/")) {
                                        imgSpan = img;//img contains First charater of Name or Identifier text
                                        $imageSpan.text(imgSpan);//.attr('style', imgStyle);
                                    }
                                    else {
                                        $image.attr('src', img);//.attr('style', imgStyle);
                                    }
                                }
                                else {
                                    colValue = VIS.Utility.Util.getIdentifierDisplayVal(colValue);
                                    img = getIdentifierImage(mField);
                                    if (img && !img.contains("Images/")) {
                                        imgSpan = img;//img contains First charater of Name or Identifier text
                                        $imageSpan.text(imgSpan);
                                    }
                                    else {
                                        $image.attr('src', img);
                                    }
                                }
                                

                                $divIcon.append($imageSpan);
                                $divIcon.append($image);

                                /*Set what do you want to show? Icon OR Label OR Both OR None*/
                                if (!mField.getHeaderIconOnly() && !mField.getHeaderHeadingOnly()) {
                                    if (imgSpan != null)
                                        $image.hide();
                                    else {
                                        $imageSpan.hide();
                                    }

                                    if ($lblControl && $lblControl.length > 0)
                                        $divLabel.append($lblControl);
                                }
                                else if (mField.getHeaderIconOnly() && mField.getHeaderHeadingOnly()) {
                                    //$divIcon.hide();
                                    $divIcon.remove();
                                }
                                else if (mField.getHeaderIconOnly()) {
                                    if (imgSpan != null)
                                        $image.hide();
                                    else
                                        $imageSpan.hide();

                                    if ($lblControl && $lblControl.length > 0)
                                        $lblControl.remove();
                                }
                                else if (mField.getHeaderHeadingOnly()) {
                                    if ($lblControl && $lblControl.length > 0) {
                                        $divLabel.append($lblControl);
                                    }
                                    $divIcon.remove();
                                }

                                $divLabel.append(iControl.getControl());

                                $containerDiv.append($div);
                                setValue(colValue, iControl, mField);
                            }
                            else {
                                $spanIcon.addClass('vis-w-p-header-icon-fixed');
                                objctrls["imgspan"] = $spanIcon;
                                /*Set what do you want to show? Icon OR Label OR Both OR None*/
                                if (mField.getDisplayType() == VIS.DisplayType.Button) {
                                    $divIcon.remove(); // button has image with field
                                }
                                else if (!mField.getHeaderIconOnly() && !mField.getHeaderHeadingOnly()) {
                                    $divIcon.append($spanIcon.append(icon));
                                    if ($lblControl && $lblControl.length > 0)
                                        $divLabel.append($lblControl);
                                }
                                else if (mField.getHeaderIconOnly() && mField.getHeaderHeadingOnly()) {
                                    $divIcon.remove();
                                }
                                else if (mField.getHeaderIconOnly()) {
                                    $divIcon.append($spanIcon.append(icon));
                                    if ($lblControl && $lblControl.length > 0)
                                        $lblControl.hide();
                                }
                                else if (mField.getHeaderHeadingOnly()) {
                                    if ($lblControl && $lblControl.length > 0) {
                                        $divLabel.append($lblControl);
                                    }
                                    $divIcon.remove();
                                }

                                setValue(colValue, iControl, mField);
                                /****END ******  Set what do you want to show? Icon OR Label OR Both OR None*/
                            }
                            $divLabel.append(iControl.getControl());
                            $containerDiv.append($div);
                            $self.controls.push(objctrls);
                        }
                    }
                }
            }

        };

        var setValue = function (colValue, iControl, mField) {
            if (colValue) {
                if (colValue.startsWith && colValue.startsWith("<") && colValue.endsWith(">")) {
                    colValue = colValue.replace("<", "").replace(">", "");
                }

                if (mField.getDisplayType() == VIS.DisplayType.Image) {
                    var oldValue = iControl.getValue();
                    iControl.getControl().show();
                    if (oldValue == colValue) {
                        iControl.refreshImage(colValue);
                    }
                }

                else if (iControl.format) {
                    colValue = iControl.format.GetFormatAmount(iControl.format.GetFormatedValue(colValue), "init", VIS.Env.isDecimalPoint());
                }


                iControl.setValue(VIS.Utility.decodeText(colValue), false);


            }
            else {
                if (mField.getDisplayType() == VIS.DisplayType.Image) {
                    iControl.getControl().hide();

                    iControl.setValue(null, false);
                }
                else if (mField.getDisplayType() == VIS.DisplayType.Button && mField.getAD_Reference_Value_ID() > 0) {
                    iControl.setText("- -");
                }
                else
                    iControl.setValue("- -", true);
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
                    colValue = mField.lookup.getDisplay(colValue, true, true);
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

                if (mField.getObscureType()) {
                    if (colValue && colValue.length > 0) {
                        colValue = VIS.Env.getObscureValue(mField.getObscureType(), colValue);
                    }
                }

            }
            else {
                colValue = null;
            }

            return colValue;
        }

        var getIdentifierImage = function (mField) {
            var value = mField.getValue();
            value = mField.lookup.getDisplay(value, true, true);

            if (value != null && value && value.indexOf("Images/") > -1) {// Based on sequence of image in idenitifer, perform logic and display image with text

                var img = value.substring(value.indexOf("Images/") + 7, value.lastIndexOf("^^"));
                img = VIS.Application.contextUrl + "Images/Thumb140x120/" + img;

                if (c == 0 || img.indexOf("nothing.png") > -1) {

                    value = value.replace("^^" + value.substring(value.indexOf("Images/"), value.lastIndexOf("^^") + 2), "^^^")
                    if (value.indexOf("Images/") > -1)
                        value = value.replace(value.substring(value.indexOf("Images/"), value.lastIndexOf("^^") + 2), "^^^");

                    value = value.split("^^^");
                    var highlightChar = '';
                    for (var c = 0; c < value.length; c++) {
                        if (value[c].trim().length > 0) {
                            if (highlightChar.length == 0)
                                highlightChar = value[c].trim().substring(0, 1).toUpper();
                            return highlightChar;
                        }

                    }
                }
                else
                    return img;
            }

        };


        /**
         * 
         * Return root div of header panel*/
        this.getRoot = function () {
            return $root;
        };

        this.getParent = function () {
            return $parentRoot;
        }
        this.hidePanel = function () {
            return $parentRoot.hide();
        }

        this.showPanel = function () {
            return $parentRoot.show();
        }



        this.alignHorzontal = function () {
            alignmentHorizontal = true;
            $parentRoot.removeClass("vis-ad-w-p-header-l").addClass("vis-ad-w-p-header-t");
            $slider.removeClass('fa-angle-double-left').addClass('fa-angle-double-up');
            $slider.parent().css('background-color', 'transparent');
            $parentRoot.css('flex-direction', 'row');
        }

        function eventHandling() {
            $slider.on("click", function () {
                if (alignmentHorizontal) {
                    if ($parentRoot.height() == 0) {
                        $parentRoot.height($self.gTab.getHeaderHeight());
                        $root.show();
                        $parentRoot.find('.vis-ad-w-p-header-arrow-l').css('padding', '');
                        $slider.removeClass('fa-angle-double-down').addClass('fa-angle-double-up').removeClass('vis-ad-w-p-header-v');
                    }
                    else {
                        $parentRoot.height(0);
                        $root.hide();
                        $parentRoot.find('.vis-ad-w-p-header-arrow-l').css('padding', '0px');
                        $slider.removeClass('fa-angle-double-up').addClass('fa-angle-double-down').addClass('vis-ad-w-p-header-v');
                    }
                }
                else {
                    if ($parentRoot.width() == 0) {
                        $slider.removeClass('fa-angle-double-right').addClass('fa-angle-double-left').removeClass('vis-ad-w-p-header-h');
                        $parentRoot.width($self.gTab.getHeaderWidth());
                        $parentRoot.find('.vis-ad-w-p-header-arrow-l').css('padding', '');
                        window.setTimeout(function () {
                            $root.show();
                        }, 50);

                    }
                    else {
                        $parentRoot.width(0);
                        $root.hide();
                        $parentRoot.find('.vis-ad-w-p-header-arrow-l').css('padding', '0px');
                        $slider.removeClass('fa-angle-double-left').addClass('fa-angle-double-right').addClass('vis-ad-w-p-header-h');
                    }
                }
                if ($self.sizeChangedListner && $self.sizeChangedListner.onSizeChanged)
                    $self.sizeChangedListner.onSizeChanged();
            });
        };

        eventHandling();


        /**
         * Dispose component
         * */
        this.disposeComponent = function () {
            $slider.remove();

            $slider = null;
            this.styleTag.remove();
            this.styleTag = null;
            this.headerItems = null;
            $self = null;
            this.curTab = null;
            this.controls = null;
            $root.remove();
            $root = null;
            $parentRoot.remove();
            $parentRoot = null;

        };

    };

    HeaderPanel.prototype.init = function (gc) {
        //Action evt listnder
        this.aPanel = gc.getAPanel();
        this.curTab = gc.getMTab();
        this.curGC = gc;
        var backColor = this.curTab.getHeaderBackColor();
        this.setHeaderLayout(this.curTab, backColor);
        var root = this.getRoot();

        var $parentRoot = this.getParent();
        var rootClass = "vis-w-p-Header-Root-v";//Fixed Class for vertical Alignment
        var alignmentHorizontal = this.curTab.getHeaderHorizontal();
        var height = this.curTab.getHeaderHeight();
        var width = this.curTab.getHeaderWidth();
        var padding = this.curTab.getHeaderPadding();

        var rootCustomStyle = this.headerUISettings(alignmentHorizontal, height, width, "", padding);
        root.addClass(rootCustomStyle);

        if (alignmentHorizontal) {
            this.alignHorzontal();
            rootClass = 'vis-w-p-Header-Root-h';//Fixed Class for Horizontal Alignment
        }

        if (!this.headerItems || this.headerItems.length <= 0) {
            return;
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
            if (!currentItem || !currentItem.HeaderItems || currentItem.HeaderItems.length <= 0) {
                continue;
            }
            this.setHeaderItems(currentItem, $containerDiv);

        }
        this.addStyleToDom();

        // Add Header Panel to Parent Control
        $parentRoot.append(root);

    };

    HeaderPanel.prototype.addSizeChangeListner = function (lstnr) {
        this.sizeChangedListner = lstnr;
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
        this.dynamicStyle.push('grid-template-columns:repeat(' + columns + ', 1fr);grid-template-rows:repeat(' + rows + ', auto);padding:' + padding + ';' + backcolor);
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
        this.dynamicStyle.push(" ." + dynamicClassName + " {display:flex;overflow:auto;");
        if (alignmentHorizontal) {
            this.dynamicStyle.push("flex:1;flex-direction:row;height: " + height + "; ");
        }
        else {
            this.dynamicStyle.push("flex-direction:column;width: " + width + ";height:calc(100vh - 94px); ");
        }
        this.dynamicStyle.push("padding:" + padding + ";" + backcolor);

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
        //if (backColor) {
        //    this.dynamicStyle.push('background: ' + backColor);
        //}
        //else {
        this.dynamicStyle.push('background: ' + 'rgba(var(--v-c-primary));');
        //}
        this.dynamicStyle.push(backColor);

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
    HeaderPanel.prototype.applyCustomUISettings = function (headerSeqNo, startCol, colSpan, startRow, rowSpan, justify, alignment, backColor, fontColor, fontSize, padding) {
        var dynamicClassName = "vis-hp-FieldGroup_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        this.dynamicStyle.push("." + dynamicClassName + "  {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";");

        this.dynamicStyle.push("justify-content:" + this.textAlignEnum[justify] + ";align-items:" + this.alignItemEnum[alignment]);
        this.dynamicStyle.push(";font-size:" + fontSize + ";color:" + fontColor + ";padding:" + padding + ";");
        this.dynamicStyle.push(backColor);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    HeaderPanel.prototype.applyCustomUIForFieldValue = function (headerSeqNo, startCol, startRow, mField) {
        var style = mField.getHeaderStyle();
        var dynamicClassName = "vis-hp-FieldValue_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo + "_" + mField.getAD_Column_ID();
        if (style && style.toLower().indexOf("@value::") > -1) {
            style = getStylefromCompositeValue(style, "@value::");
        }

        this.dynamicStyle.push("." + dynamicClassName + "  {" + style + "} ");
        return dynamicClassName;
    };

    HeaderPanel.prototype.applyCustomUIForFieldImg = function (headerSeqNo, startCol, startRow, mField) {
        var style = mField.getHeaderStyle();
        var dynamicClassName = "vis-hp-FieldImgValue_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        if (style && style.toLower().indexOf("@img::") > -1) {
            style = getStylefromCompositeValue(style, "@img::");
        }
        this.dynamicStyle.push("." + dynamicClassName + "  {" + style + "} ");
        return dynamicClassName;
    };

    HeaderPanel.prototype.applyCustomUIForFieldSpan = function (headerSeqNo, startCol, startRow, mField) {
        var style = mField.getHeaderStyle();
        var dynamicClassName = "vis-hp-FieldImgValue_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        if (style && style.toLower().indexOf("@span::") > -1) {
            style = getStylefromCompositeValue(style, "@span::");
        }
        this.dynamicStyle.push("." + dynamicClassName + "  {" + style + "} ");
        return dynamicClassName;
    };

    var getStylefromCompositeValue = function (style, requiredtype) {
        if (style && style.toLower().indexOf(requiredtype) > -1) {
            var styleArr = style.split("|");
            for (var i = 0; i < styleArr.length; i++) {
                if (styleArr[i].toLower().indexOf(requiredtype) > -1) {
                    return styleArr[i].toLower().replace(requiredtype, "").trim();
                }
            }
        }
    }

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
        this.aPanel = null;
        this.curTab = null;
        this.curGC = null;
        this.sizeChangedListner = null;
        this.disposeComponent();
    };

    HeaderPanel.prototype.actionPerformed = function (action) {
        //selfPan.actionButton(action.source);



        if (this.aPanel.curTab.needSave(true, false)) {
            this.aPanel.cmd_save(true);
            return;
        }


        this.aPanel.actionPerformed(action, this);
    };

    HeaderPanel.prototype.cmd_save = function (manual, callback) {
        return this.aPanel.cmd_save2(manual, this.curTab, this.curGC, this.aPanel, callback);
    }
    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));