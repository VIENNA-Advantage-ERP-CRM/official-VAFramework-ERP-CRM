
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};

    function VAttributeGrid() {
        this.frame;
        this.windowNo;

        var $self = this;

        var toggle = false;
        var toggleGen = false;
        var toggleside = false;

        // Setting Grid		
        var _setting = false;
        //	Modes	
        var modes = [];
        var MODE_VIEW = 0;
        var MODE_PO = 0;
        var MODE_PRICE = 0;
        // Price List Version	
        var _M_PriceList_Version_ID = 0;
        // Warehouse		
        var _M_Warehouse_ID = 0;
        var _attributes = [];

        var lblA1 = new VIS.Controls.VLabel(VIS.Msg.getElement(VIS.Env.getCtx(), "M_Attribute_ID"), "M_Attribute_ID", false, true);
        var lblA2 = new VIS.Controls.VLabel(VIS.Msg.getElement(VIS.Env.getCtx(), "M_Attribute_ID"), "M_Attribute_ID", false, true);
        var lblPriceList = new VIS.Controls.VLabel(VIS.Msg.getElement(VIS.Env.getCtx(), "M_PriceList_ID"), "M_PriceList_ID", false, true);
        var lblWareHouse = new VIS.Controls.VLabel(VIS.Msg.getElement(VIS.Env.getCtx(), "M_Warehouse_ID"), "M_Warehouse_ID", false, true);
        var lblMode = new VIS.Controls.VLabel(VIS.Msg.getMsg("Mode", false, false), "Mode", false, true);

        var cmbA1 = new VIS.Controls.VComboBox('', false, false, true);
        var cmbA2 = new VIS.Controls.VComboBox('', false, false, true);
        var cmbPrice = new VIS.Controls.VComboBox('', false, false, true);
        var cmbWH = new VIS.Controls.VComboBox('', false, false, true);
        var cmbMode = new VIS.Controls.VComboBox('', false, false, true);

        this.log = VIS.Logging.VLogger.getVLogger("VAttributeGrid");

        var $root = $("<div class='vis-forms-container' style='width: 100%; height: 100%; background-color: white; '>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var leftsideDiv = null;
        var rightSideGridDiv = null;

        var topLeftDiv = null;
        var paradiv = null;
        var gridDiv = null;
        var bottumDiv = null;

        var btnOk = null;
        var btnRefresh = null;
        var btnToggel = null;

        var sideDivWidth = 260;
        var minSideWidth = 50;
        var selectDivWidth = $(window).width() - (sideDivWidth);
        var selectDivFullWidth = $(window).width() - (minSideWidth);
        var selectDivToggelWidth = selectDivWidth + sideDivWidth + 5;
        var sideDivHeight = $(window).height() - 210;

        var tblpMain = $("<table style='width: 100%;'>");

        function initializeComponent() {
            setBusy(true);

            var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-left.png";
            //left side div
            leftsideDiv = $("<div id='sideDiv_" + $self.windowNo + "' class='vis-archive-left-sidebar'>");//" + sideDivHeight + "

            //topLeftSide div
            topLeftDiv = $("<div id='btnSpaceDiv_" + $self.windowNo + "'  class='vis-archive-l-s-head'>" +
                       "<button id='btnSpace_" + $self.windowNo + "' class='vis-archive-sb-t-button'>" +
                       "<i class='vis vis-arrow-left'></i></button></div>" +
                      "</div>");

            //left side parameter div
            paradiv = $("<div class='vis-archive-l-s-content vis-leftsidebarouterwrap' id='parameterDiv_" + $self.windowNo + "'>");
            leftsideDiv.append(topLeftDiv).append(paradiv);

            //Refresh
            src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
            btnRefresh = $("<button id='btnRefresh_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button'><i class='vis vis-refresh'></i></button>");
            //Cancel button
            btnOk = $("<input id='btnOk_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button' style='min-width: 70px;' type='button' value='" + VIS.Msg.getMsg("OK", false, false) + "'>");


            var tble = $("<table style='width: 100%;'>"); 
            //var tr = $("<tr>");
            //var td = $("<td style='padding: 0px 10px 0px;'>");
            paradiv.append(tble);
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblA1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 10px 10px 0px'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbA1.getControl().css("display", "inline-block"));
            Leftformfieldctrlwrp.append(lblA1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblA2.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbA2.getControl().css("display", "inline-block"));
            Leftformfieldctrlwrp.append(lblA2.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblPriceList.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbPrice.getControl().css("display", "inline-block"));
            Leftformfieldctrlwrp.append(lblPriceList.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblWareHouse.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbWH.getControl().css("display", "inline-block"));
            Leftformfieldctrlwrp.append(lblWareHouse.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            bottumDiv = $("<div class='vis-frm-bot-btn-wrp'>");
            var buttonDiv = $("<div class='vis-frm-bot-btn-wrp'>");
            buttonDiv.append(btnOk);
            bottumDiv.append(btnRefresh);

            //Right side div
            rightSideGridDiv = $("<div id='rightSideGridDiv_" + $self.windowNo + "' class='vis-frm-grid-wrap'>");
            //rightSideGridDiv.css("width", selectDivWidth);

            gridDiv = $("<div id='gridDiv_" + $self.windowNo + "' class='vis-frm-grid-inn'>");
            gridDiv.append(tblpMain);
            rightSideGridDiv.append(gridDiv);
            rightSideGridDiv.append(buttonDiv);

            //Add to root
            leftsideDiv.append(bottumDiv);
            $root.append($busyDiv);
            $root.append(leftsideDiv).append(rightSideGridDiv);
        }

        function dynInit() {
            modes.push({ Key: '0', Name: VIS.Msg.getMsg("ModeView", false, false) });
            vAttributeGrid();
            getAttributeOfClient();
        }

        function getAttributeOfClient() {
            $.ajax({
                url: VIS.Application.contextUrl + "Common/GetDataQueryAttribute",
                type: "GET",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.lst) {
                        _attributes = data.lst;
                        for (var i = 0; i < _attributes.length; i++) {
                            fillDirectComboBox(cmbA1.getControl(), _attributes[i].GetKeyNamePair, i);
                            fillDirectComboBox(cmbA2.getControl(), _attributes[i].GetKeyNamePair, i);
                        }
                    }
                    if (data.priceList) {
                        fillComboBox(cmbPrice.getControl(), data.priceList);
                    }
                    if (data.whList) {
                        fillComboBox(cmbWH.getControl(), data.whList);
                    }
                    setBusy(false);
                }
            });
        }

        function vAttributeGrid() {
            fillComboBox(cmbMode.getControl(), modes);
        }

        function fillComboBox(cntrl, vo) {
            for (var i = 0; i < vo.length; i++) {
                cntrl.append(" <option selected value='" + vo[i].Key + "' >" + vo[i].Name + "</option>");
            }
            cntrl.prop('selectedIndex', 0);
        }

        function fillDirectComboBox(cntrl, vo, count) {
            if (count == 0) {
                cntrl.append(" <option selected value='0' ></option>");
            }
            cntrl.append(" <option selected value='" + vo.Key + "' >" + vo.Name + "</option>");
            cntrl.prop('selectedIndex', 0);
        }

        function createPO() {

        }

        function updatePrices() {

        }

        function createRowsColumn(rows, cols) {
            cols = cols < 2 ? 2 : cols;
            rows += 1;

            for (row = 0; row < rows; row++) {
                var tr = $("<tr id='row_" + row + "' >");
                tblpMain.append(tr);
                for (col = 0; col < cols; col++) {
                    var td = $("<td id='col_" + col + "'  style='padding: 4px 15px 2px;'>");
                    tr.append(td);
                }
            }

            var lbl = new VIS.Controls.VLabel();
            lbl.getControl().text("Mode");
            var rowObj = tblpMain.find("#row_0");
            if (cols == 2) {
                var colObj = rowObj.find("#col_" + (cols - 1));
                colObj.append(cmbMode.getControl().css("display", "inline-block").css("width", "236px").css("height", "30px"));

                colObj = rowObj.find("#col_" + (cols - 2));
                colObj.append(lbl.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
            }
            else {
                var colObj = rowObj.find("#col_2");
                colObj.append(cmbMode.getControl().css("display", "inline-block").css("width", "236px").css("height", "30px"));

                colObj = rowObj.find("#col_1");
                colObj.append(lbl.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
            }
        }

        function getGridElement(xValue, yValue, col, row) {
            var attributeId = 0;
            var attributeValueId = 0;
            var attributeyId = 0;
            var attributeValueyId = 0;

            if (xValue) {
                attributeId = xValue.GetM_Attribute_ID;
                attributeValueId = xValue.GetM_AttributeValue_ID;
            }
            if (yValue) {
                attributeyId = yValue.GetM_Attribute_ID;
                attributeValueyId = yValue.GetM_AttributeValue_ID;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "Common/GetGridElement",
                dataType: "json",
                data: {
                    xM_Attribute_ID: attributeId,
                    xM_AttributeValue_ID: attributeValueId,
                    yM_Attribute_ID: attributeyId,
                    yM_AttributeValue_ID: attributeValueyId,
                    M_PriceList_Version_ID: _M_PriceList_Version_ID,
                    M_Warehouse_ID: _M_Warehouse_ID,
                    windowNo: $self.windowNo
                },
                async: false,
                success: function (data) {
                    //alert(data.stValue);
                    setBusy(false);
                }
            });

        }

        function createGrid() {
            if (cmbA1 == null || _setting)
                return;
            var indexAttr1 = cmbA1.getControl()[0].selectedIndex;
            var indexAttr2 = cmbA2.getControl()[0].selectedIndex;
            if (indexAttr1 == indexAttr2) {
                $self.log.warning("Same Attribute Selected");
                return;
            }
            _setting = true;
            _M_PriceList_Version_ID = cmbPrice.getControl().find('option:selected').val();
            _M_Warehouse_ID = cmbWH.getControl().find('option:selected').val();

            //	x dimension
            var cols = 2;
            var xValues = [];
            var yValues = [];
            var xValue = null;
            var yValue = null;

            if (indexAttr1 > 0) {
                xValues = _attributes[indexAttr1 - 1].GetMAttributeValues;
            }
            if (xValues != null) {
                cols = xValues.length + 1;;
                $self.log.info("X - " + _attributes[indexAttr1 - 1].GetName + " #" + xValues.length);
            }

            //	y dimension
            var rows = 2;
            if (indexAttr2 > 0) {
                yValues = _attributes[indexAttr2 - 1].GetMAttributeValues;
            }
            if (yValues.length > 0) {
                rows = yValues.length + 1;
                $self.log.info("Y - " + _attributes[indexAttr2 - 1].GetName + " #" + yValues.length);
            }

            $self.log.info("Rows=" + rows + " - Cols=" + cols);

            createRowsColumn(rows, cols);

            var rowObj = null;
            var colObj = null;

            for (row = 0; row < rows; row++) {
                for (col = 0; col < cols; col++) {
                    if (xValues != null) {
                        xValue = xValues[col];
                    }
                    if (yValues != null) {
                        yValue = yValues[row];
                    }
                    if (row == 0 && col == 0) {
                        var lbl1 = new VIS.Controls.VLabel();
                        var lbl2 = new VIS.Controls.VLabel();

                        if (xValues != null) {
                            lbl1.getControl().text(_attributes[indexAttr1 - 1].GetName);
                            rowObj = tblpMain.find("#row_1");
                            colObj = rowObj.find("#col_" + col);
                            colObj.append(lbl1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }

                        if (yValues.length > 0) {
                            lbl2.getControl().text(_attributes[indexAttr2 - 1].GetName);
                            rowObj = tblpMain.find("#row_1");
                            colObj = rowObj.find("#col_" + col);

                            colObj.append(lbl2.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }

                        if (xValue != null) {
                            lblA1 = new VIS.Controls.VLabel();
                            lblA1.getControl().text(xValue.Name);
                            rowObj = tblpMain.find("#row_1");
                            colObj = rowObj.find("#col_" + (col + 1));

                            colObj.append(lblA1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }
                    }
                    else if (row == 0)	//	column labels
                    {
                        if (xValue != null) {
                            lblA1 = new VIS.Controls.VLabel();
                            lblA1.getControl().text(xValue.Name);
                            rowObj = tblpMain.find("#row_1");
                            colObj = rowObj.find("#col_" + (col + 1));

                            colObj.append(lblA1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }
                        else {
                            // var lbl = new VIS.Controls.VLabel();
                            // tdp.append(lbl.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }
                    }
                    else if (col == 0)	//	row labels
                    {
                        if (yValue != null) {
                            lblA1 = new VIS.Controls.VLabel();
                            lblA1.getControl().text(yValue.Name);
                            rowObj = tblpMain.find("#row_" + row + 1);
                            colObj = rowObj.find("#col_" + col);

                            colObj.append(lblA1.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }
                        else {
                            //var lbl = new VIS.Controls.VLabel();
                            //tdp.append(lbl.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
                        }
                    }
                    else {
                        getGridElement(xValue, yValue, col, row + 1);
                    }
                }
            }
            _setting = false;
        }

        function gridOK() {
            var mode = cmbMode.getControl().find('option:selected').val();
            //	Create PO
            if (mode == MODE_PO) {
                // CreatePO();
                // cmbMode.getControl().prop("SelectedIndex", MODE_VIEW);
                return;
            }
                //	Update Prices
            else if (mode == MODE_PRICE) {
                // UpdatePrices();
                // cmbMode.getControl().prop("SelectedIndex", MODE_VIEW);
                return;
            }
            else if (mode == MODE_VIEW) {
                    ;
            }
        }

        setBusy = function (isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.Initialize = function () {
            initializeComponent();
            btnToggel = $root.find("#btnSpace_" + $self.windowNo);

            //Events
            if (btnOk != null)
                btnOk.on(VIS.Events.onTouchStartOrClick, function () {
                    gridOK();
                    $self.dispose();
                });

            if (btnRefresh != null)
                btnRefresh.on(VIS.Events.onTouchStartOrClick, function () {
                    setBusy(true);
                    tblpMain.empty();
                    createGrid();
                    setBusy(false);
                });

            if (btnToggel != null)
                var borderspace = 0;
                btnToggel.on(VIS.Events.onTouchStartOrClick, function () {
                    if (toggleside) {
                        btnRefresh.show();
                        lblA1.getControl().show();
                        lblA2.getControl().show();
                        if (VIS.Application.isRTL) {
                            borderspace = 180;
                        }
                        else {
                            borderspace = 0;

                        }
                        btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = false;
                        // btnToggel.animate({ width: sideDivWidth }, "slow");
                        rightSideGridDiv.animate({ width: selectDivWidth }, "slow");
                        paradiv.css("display", "block");
                        topLeftDiv.animate({ width: sideDivWidth }, "slow");
                        leftsideDiv.animate({ width: sideDivWidth }, "slow");
                        //leftsideDiv.animate({ width: sideDivWidth }, "slow", null, function () {
                        //    dGrid.resize();
                        //});
                    }
                    else {
                        if (VIS.Application.isRTL) {
                            borderspace = 0;
                        }
                        else {
                            borderspace = 180;

                        }
                        btnRefresh.hide();
                        lblA1.getControl().hide();
                        lblA2.getControl().hide();
                        btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = true;
                        // btnToggel.animate({ width: minSideWidth }, "slow");
                        rightSideGridDiv.animate({ width: selectDivFullWidth }, "slow");
                        leftsideDiv.animate({ width: minSideWidth }, "slow");
                        topLeftDiv.animate({ width: minSideWidth }, "slow");
                         paradiv.css("display", "none");
                        //rightSideGridDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                        //    dGrid.resize();
                        //});
                    }
                });
        }

        this.display = function () {
            setTimeout(
            dynInit(), 5);
        }

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        this.disposeComponent = function () {

            if (btnOk)
                btnOk.off(VIS.Events.onTouchStartOrClick);
            if (btnRefresh)
                btnRefresh.off(VIS.Events.onTouchStartOrClick);
            if (btnToggel)
                btnToggel.off(VIS.Events.onTouchStartOrClick);

            $self.windowNo = null;

            toggle = null;
            toggleGen = null;
            toggleside = null;

            lblA1 = null;
            lblA2 = null;

            $self.log = null;
            $root = null;
            $busyDiv = null;

            leftsideDiv = null;
            rightSideGridDiv = null;

            topLeftDiv = null;
            paradiv = null;
            gridDiv = null;
            bottumDiv = null;

            btnOk = null;
            btnRefresh = null;
            btnToggel = null;

            sideDivWidth = null;
            minSideWidth = null;
            selectDivWidth = null;
            selectDivFullWidth = null;
            selectDivToggelWidth = null;
            sideDivHeight = null;

            $self = null;
            this.getRoot = null;
            this.disposeComponent = null;
        };
    };

    //Must Implement with same parameter
    VAttributeGrid.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
        this.display();
    };

    //Must implement dispose
    VAttributeGrid.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps.AForms.VAttributeGrid = VAttributeGrid;


})(VIS, jQuery);