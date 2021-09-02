/********************************************************
 * Module Name    :     Application
 * Purpose        :     Move Product Container.
 * Author         :     Amit Bansal
 * Date           :     26-July-2018
  ******************************************************/
; (function (VIS, $) {
    VIS.AForms = VIS.AForms || {};
    function productContainerMove() {

        this.frame;
        this.windowNo;
        var $self = this;
        var $root = $("<div style='height:100%;background-color:white;' class='vis-forms-container' >");
        var $bsyDiv = $("<div class='vis-busyindicatorouterwrap'><div class='vis-busyindicatorinnerwrap'><i class='vis-busyindicatordiv'></i></div></div>");

        /**Create Main View Variable **/
        var $mainpageContent = null;
        var $formWrap = null;
        var $formDataRow = null;
        var $formData = null;
        var $formDataR = null;
        var $formData3 = null;

        var $lblFromWarehouse = null;
        var $cmbFromWarehouse = null;
        var $lblToWarehouse = null;
        var $cmbToWarehouse = null;

        var $lblFromLocator = null;
        var $cmbFromLocator = null;
        var $lblToLocator = null;
        var $cmbToLocator = null;
        var $btnFromContainerTree = null;
        var $lblFromContainer = null;
        var $cmbFromContainer = null;
        var $btnFromContainerTo = null;

        var $lblToContainer = null;
        var $cmbToContainer = null;

        // configuration for open Popup
        var config = null;
        var $fromContainerPopUp = null;
        var fromContainerDialog = null;
        var $fromContainerParent;
        var $fromContainerParentChild = null;

        var $toContainerPopUp = null;
        var toContainerDialog = null;
        var $toContainerParent;
        var $toContainerParentChild = null;


        var $moveFullContainer = null;
        var $lblMoveFullContainer = null;
        var $withoutContainer = null;
        var $lblWithoutContainer = null;
        var $withContainer = null;
        var $lblWithContainer = null;


        var $formRightWrap = null;
        var $containerGrid = null;
        var CGrid = null;
        this.grdname;

        var $actionPanel = null;
        var $buttons = null;
        var $btnApply = null;
        var $btnOk = null;
        var $btnCancel = null;
        var $labelMessage = null;
        //Search Button
        var $btnSearch = null;
        var $txtSearch = null;
        var $btnSearchTo = null;
        var $txtSearchTo = null;

        /* Variable for Paging*/
        var PAGESIZE = 50;
        var CurrentPage = 1;
        var pageNoContainer = 1, gridPgnoContainer = 1, containerRecord = 0;
        var isContainerGridLoaded = false;
        var ttlPages = 1;
        var fromContainerID = 0;
        //To store multiple select values
        var multiselectData = [];

        this.Initialize = function () {
            busyDiv('visible');
            createMainView();
            bindEvents();
            busyDiv('hidden');
        };

        this.setHeight = function () {
            return 700;
        };

        function createMainView() {
            $mainpageContent = $('<div class="VIS_main-wrap vis-formouterwrpdiv">');

            // Row 1
            $formWrap = $('<div class="VIS_form-wrap">');
            $formDataRow = $('<div class="VIS_form-row">');

            $formData = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataCtrlWrp = $('<div class="vis-control-wrap">');
            $lblFromWarehouse = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_Warehouse_ID") + '</label>');
            $cmbFromWarehouse = $('<select disabled class="vis-ev-col-readonly">');
            $formData.append($formDataCtrlWrp);
            $formDataCtrlWrp.append($cmbFromWarehouse).append($lblFromWarehouse);

            $formDataR = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataRCtrlWrp = $('<div class="vis-control-wrap">');
            $lblToWarehouse = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_WarehouseTo_ID") + '</label>');
            $cmbToWarehouse = $('<select disabled class="vis-ev-col-readonly">');
            $formDataR.append($formDataRCtrlWrp);
            $formDataRCtrlWrp.append($cmbToWarehouse).append($lblToWarehouse);

            $formDataRow.append($formData).append($formDataR);
            $formWrap.append($formDataRow);

            // Row 2
            $formDataRow = $('<div class="VIS_form-row">');

            $formData = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataCtrlWrp = $('<div class="vis-control-wrap">');
            $lblFromLocator = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_Locator_ID") + '</label>');
            $cmbFromLocator = $('<select>');
            $formData.append($formDataCtrlWrp);
            $formDataCtrlWrp.append($cmbFromLocator).append($lblFromLocator);

            $formDataR = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataRCtrlWrp = $('<div class="vis-control-wrap">');
            $lblToLocator = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_LocatorTo_ID") + '</label>');
            $cmbToLocator = $('<select>');
            $formDataR.append($formDataRCtrlWrp);
            $formDataRCtrlWrp.append($cmbToLocator).append($lblToLocator);

            $formDataRow.append($formData).append($formDataR);
            $formWrap.append($formDataRow);

            // Row 3
            $formDataRow = $('<div class="VIS_form-row">');

            $formData = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataCtrlWrp = $('<div class="vis-control-wrap">');
            var $formDataBtnWrp = $('<div class="input-group-append">');
            $lblFromContainer = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_FromContainer") + '</label>');
            $cmbFromContainer = $('<select disabled class="VIS-container-move-combo-width" class="vis-ev-col-readonly" data-hasbtn=" ">');
            $btnFromContainerTree = $('<span class="fa fa-cubes input-group-text VIS_buttons-ContainerTree VIS-pallet-icon VIS_Tree-Container-disabled"></span>');
            $formData.append($formDataCtrlWrp);
            $formDataCtrlWrp.append($cmbFromContainer).append($lblFromContainer);
            $formData.append($formDataBtnWrp);
            $formDataBtnWrp.append($btnFromContainerTree);

            $formDataR = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataRCtrlWrp = $('<div class="vis-control-wrap">');
            var $formDataRBtnWrp = $('<div class="input-group-append">');
            $lblToContainer = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "Ref_M_ProductContainerTo_ID") + '</label>');
            $cmbToContainer = $('<select disabled class="VIS-container-move-combo-width" class="vis-ev-col-readonly" data-hasbtn=" ">');
            $btnToContainerTree = $('<span class="fa fa-cubes input-group-text VIS_buttons-ContainerTree VIS-pallet-icon VIS_Tree-Container-disabled"></span>');
            $formDataR.append($formDataRCtrlWrp);
            $formDataRCtrlWrp.append($cmbToContainer).append($lblToContainer);
            $formDataR.append($formDataRBtnWrp);
            $formDataRBtnWrp.append($btnToContainerTree);

            $formDataRow.append($formData).append($formDataR);
            $formWrap.append($formDataRow);

            // Row 4
            $formDataRow = $('<div class="VIS_form-row">');

            $formData = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formDataCtrlWrp = $('<div class="vis-control-wrap">');
            var $ChkFullContainer = $('<label class="vis-ec-col-lblchkbox" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_MoveFullContainerToolTip") + '"><input type="checkbox" name="">' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_MoveFullContainer") + '</label>');
            $formData.append($formDataCtrlWrp);
            $formDataCtrlWrp.append($ChkFullContainer)/*.append($lblMoveFullContainer)*/;
            $moveFullContainer = $ChkFullContainer.find('input');

            //$formDataR = $('<div class="VIS_form-col-checkbox">');
            //$withContainer = $('<input type="radio" name="WithContainer" disabled>');
            //$lblWithContainer = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(),"VIS_WithContainer") + '</label>');
            //$formDataR.append($withContainer).append($lblWithContainer);

            $formData3 = $('<div class="VIS_form-col input-group vis-input-wrap">');
            var $formData3CtrlWrp = $('<div class="vis-control-wrap">');
            //$withoutContainer = $('<input type="radio" name="WithoutContainer" disabled>');
            var $ChkwithoutContainer = $('<label class="vis-ec-col-lblchkbox" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_WithoutContainerToolTip") + '"><input type="checkbox" name="WithoutContainer">' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_WithoutContainer") + '</label>');
            $formData3.append($formData3CtrlWrp);
            $formData3CtrlWrp.append($ChkwithoutContainer)/*.append($lblWithoutContainer)*/;
            $withoutContainer = $ChkwithoutContainer.find('input');

            $formDataRow.append($formData).append($formData3);
            $formWrap.append($formDataRow);

            // Product Container Grid 
            $formRightWrap = $(' <div class="VIS_outer-table-wrap">');
            $containerGrid = $('<div class="VIS_outer-table-wrap">');
            $formRightWrap.append($containerGrid);

            //Action 
            $actionPanel = $('<div class="VIS_buttons-wrap">');
            $labelMessage = $('<div class="pull-left" style="display: none;"><label>' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_ContainerGridInfo") + '</label></div>');

            $buttons = $('<div class="pull-right">');
            //$btnApply = $('<span class="btn">Apply</span>');
            //$btnCancel = $('<span class="btn">Cancel</span>');
            $btnOk = $('<span class="btn">' + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_OK") + '</span>');
            createPageSettings($buttons);
            $buttons.append($btnOk);
            $actionPanel.append($labelMessage).append($buttons);
            $mainpageContent.append($formWrap).append($formRightWrap).append($actionPanel);
            $root.append($mainpageContent).append($bsyDiv);
        };

        function createPageSettings(buttonsdiv) {
            ulPaging = $('<ul style="float:left;margin-top:4px" class="vis-statusbar-ul">');

            liFirstPage = $('<li style="display: none; "><div><i class="vis vis-shiftleft" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "FirstPage") + '"  style="opacity: 0.6;"></i></div></li>');

            liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-pageup" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "PageUp") + '"  style="opacity: 0.6;"></i></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "PageDown") + '" style="opacity: 0.6;"></i></div></li>');

            liLastPage = $('<li style="display: none;"><div><i class="vis vis-shiftright" title="' + VIS.Msg.translate(VIS.Env.getCtx(), "LastPage") + '" style="opacity: 0.6;"></div></li>');


            ulPaging.append(liFirstPage).append(liPrevPage).append(liCurrPage).append(liNextPage).append(liLastPage);
            buttonsdiv.append(ulPaging);

            //pageEvents();
        }

        function pageEvents() {
            liPrevPage.on("click", function (e) {
                e.preventDefault();
                if ($(this).css("opacity") == "1") {
                    busyDiv('visible');
                    LoadProductContainerGrid(fromContainerID, true, -1);
                }
            });
            liNextPage.on("click", function (e) {
                e.preventDefault();
                if ($(this).css("opacity") == "1") {
                    busyDiv('visible');
                    LoadProductContainerGrid(fromContainerID, true, 1);
                }
            });
            cmbPage.on("change", function (e) {
                e.preventDefault();
                busyDiv('visible');
                LoadProductContainerGrid(fromContainerID, true, 0);
            });
        }

        function resetPageCtrls(CurrentPage, TotalPage) {
            cmbPage.val(CurrentPage);
            if (TotalPage > 0) {
                if (TotalPage > CurrentPage) {
                    liNextPage.css("opacity", "1");
                }
                else {
                    liNextPage.css("opacity", "0.6");
                }

                if (CurrentPage > 1) {
                    liPrevPage.css("opacity", "1");
                }
                else {
                    liPrevPage.css("opacity", "0.6");
                }

                if (TotalPage == 1) {
                    liPrevPage.css("opacity", "0.6");
                    liNextPage.css("opacity", "0.6");
                }
            }
            else {
                liPrevPage.css("opacity", "0.6");
                liNextPage.css("opacity", "0.6");
            }
            busyDiv('hidden');
        }

        function bindEvents() {

            // load From warehouse
            LoadFromWarehouse();

            // Load From Locator
            loadFromLocator();

            // load To warehouse
            loadToWarehouse();

            // load To Locator
            //loadToLocator();

            // on selection of "From Locator" -- load "From Container"
            $cmbFromLocator.on('change', function () {
                busyDiv('visible');
                $cmbFromContainer.empty();
                if ($cmbFromLocator.val() == "0") {
                    $btnFromContainerTree.addClass("VIS_Tree-Container-disabled");
                }
                else {
                    $btnFromContainerTree.removeClass("VIS_Tree-Container-disabled");
                }
                window.setTimeout(function () {
                    LoadProductContainerGrid(0, false, 0);
                }, 20);
                busyDiv('hidden');
            });

            // on selection of "To Warehouse" -- load locator
            $cmbToWarehouse.on('change', function () {
                busyDiv('visible');
                $cmbToContainer.empty();
                loadToLocator();
                busyDiv('hidden');
            });

            // on selection of "To Locator" -- load "To Container"
            $cmbToLocator.on('change', function () {
                busyDiv('visible');
                $cmbToContainer.empty();
                if ($cmbToLocator.val() == "0") {
                    $btnToContainerTree.addClass("VIS_Tree-Container-disabled");
                }
                else {
                    $btnToContainerTree.removeClass("VIS_Tree-Container-disabled");
                }
                busyDiv('hidden');
            });

            // Load Grid
            LoadProductContainerGrid(0, false, 0);

            // Create Line on Movement Line 
            $btnOk.on("click", function () {
                if ($cmbToWarehouse.val() == "0") {
                    VIS.ADialog.error("VIS_ToWarehouseNotSelected", true, "", "");
                    return false;
                };
                if ($cmbFromLocator.val() == "0") {
                    VIS.ADialog.error("VIS_FromLocatorNotSelected", true, "", "");
                    return false;
                };
                if ($cmbToLocator.val() == "0") {
                    VIS.ADialog.error("VIS_ToLocatorNotSelected", true, "", "");
                    return false;
                };
                if ($cmbFromContainer.val() == null && $cmbToContainer.val() == null) {
                    // Need to select either "From Container" or "To Container"
                    // Otherwise no need to use Move container functionality
                    VIS.ADialog.error("VIS_SelectContainer", true, "", "");
                    return false;
                }

                var countRecordChanges = multiselectData.length;
                if (countRecordChanges == 0 && !($withoutContainer.is(":checked")) && !($moveFullContainer.is(":checked"))) {
                    VIS.ADialog.error("VIS_MoveQtyNotSelected", true, "", "");
                    return false;
                };
                if ($moveFullContainer.is(":checked")) {
                    VIS.ADialog.confirm("VIS_DoYouWantToDeleteManualCL", true, "", "Confirm", function (result) {
                        if (result) {
                            busyDiv('visible');
                            SaveMovementLine();
                            busyDiv('hidden');
                        }
                        else
                            return false;
                    });
                }
                else {
                    busyDiv('visible');
                    SaveMovementLine();
                    busyDiv('hidden');
                }
            });

            // Load Tree Structure for "From Container"
            $btnFromContainerTree.on("click", function () {
                //config.gridFromContainer();
                if ($btnFromContainerTree.hasClass("VIS_Tree-Container-disabled")) { return; };
                var warehouse = ($cmbFromWarehouse.val() == "0" || $cmbFromWarehouse.val() == null) ? 0 : parseInt($cmbFromWarehouse.val());
                var locator = ($cmbFromLocator.val() == "0" || $cmbFromLocator.val() == null) ? 0 : parseInt($cmbFromLocator.val());

                var obj = new VIS.productContainerTree(warehouse, locator, null, null);
                if (obj != null) {
                    obj.showDialog();

                    obj.onClosing = function (containerId, containerText) {
                        fromContainerID = containerId;
                        if (containerId > 0) {
                            $labelMessage.css("display", "block");
                        }
                        // set combo value - 
                        $cmbFromContainer.empty();
                        $cmbFromContainer.append(" <option value=" + containerId + "  selected>" + containerText + "</option>");
                        multiselectData = [];// Empty Array
                        LoadProductContainerGrid(containerId, false, 0);
                    };
                }
                obj = null;
            });

            // Load Tree Structure for "To Container"
            $btnToContainerTree.on("click", function () {
                //config.TreeToContainer();
                if ($btnToContainerTree.hasClass("VIS_Tree-Container-disabled")) { return; };
                var warehouse = ($cmbToWarehouse.val() == "0" || $cmbToWarehouse.val() == null) ? 0 : parseInt($cmbToWarehouse.val());
                var locator = ($cmbToLocator.val() == "0" || $cmbToLocator.val() == null) ? 0 : parseInt($cmbToLocator.val());

                var obj = new VIS.productContainerTree(warehouse, locator, null, null);
                if (obj != null) {
                    obj.showDialog();

                    obj.onClosing = function (containerId, containerText) {
                        // set combo value - 
                        $cmbToContainer.empty();
                        $cmbToContainer.append(" <option value=" + containerId + "  selected>" + containerText + "</option>");
                    };
                }
                obj = null;
            });

            // Is used to lock Grid
            $moveFullContainer.change(function () {
                if (this.checked) {
                    w2ui[grdname].lock('');
                    $(this).prop("checked", true);

                    $withoutContainer.prop('checked', false);

                    // disable = false
                    //$withoutContainer.prop("disabled", false);
                    //$withContainer.prop("disabled", false);

                    // set value as false
                    //$withoutContainer.attr('checked', false);
                    //$withContainer.attr('checked', false);
                }
                else {
                    w2ui[grdname].unlock('');
                    $(this).prop("checked", false);

                    //$withoutContainer.prop("disabled", true);
                    //$withContainer.prop("disabled", true);

                    //$withoutContainer.attr('checked', false);
                    //$withContainer.attr('checked', false);
                }
            });

            $withoutContainer.change(function () {
                if (this.checked) {
                    w2ui[grdname].lock('');
                    $(this).prop("checked", true);

                    $moveFullContainer.prop('checked', false);

                    //$withoutContainer.attr('checked', true);
                    //$withContainer.attr('checked', false);
                }
                else {
                    w2ui[grdname].unlock('');
                    $(this).prop("checked", false);
                }
            });

            pageEvents();

        };

        // load From warehouse
        function LoadFromWarehouse() {
            // get from warehouse id from the context
            var fromWarehouse_ID = VIS.context.getWindowContext($self.windowNo, "DTD001_MWarehouseSource_ID", true); // windowNo, context, onlyWindow
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/LoadWarehouse",
                data: { fromWarehouse_ID: fromWarehouse_ID },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        for (var i = 0; i < result.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(result[i].ID);
                            value = VIS.Utility.Util.getValueOfString(result[i].Name);
                            $cmbFromWarehouse.append(" <option value=" + key + ">" + value + "</option>");
                        }
                    }
                    $cmbFromWarehouse.prop('selectedIndex', 0);
                },
                error: function (er) {
                    busyDiv('hidden');
                    console.log(er);
                }
            });
        };

        // load from Locator
        function loadFromLocator() {
            // get from warehouse id from the context
            var fromWarehouse_ID = VIS.context.getWindowContext($self.windowNo, "DTD001_MWarehouseSource_ID", true); // windowNo, context, onlyWindow
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/LoadLocator",
                data: { fromWarehouse_ID: fromWarehouse_ID },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        $cmbFromLocator.empty();
                        $cmbFromLocator.append("<option value= 0 >Select</option>");
                        for (var i = 0; i < result.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(result[i].ID);
                            value = VIS.Utility.Util.getValueOfString(result[i].Name);
                            $cmbFromLocator.append(" <option value=" + key + ">" + value + "</option>");
                        }
                    }
                    else {
                        $cmbFromLocator.append("<option value= 0 >Select</option>");
                    }
                    $cmbFromLocator.prop('selectedIndex', 0);
                },
                error: function (er) {
                    busyDiv('hidden');
                    console.log(er);
                }
            });
        };

        // load To warehouse
        function loadToWarehouse() {
            var fromWarehouse_ID = VIS.context.getWindowContext($self.windowNo, "M_Warehouse_ID", true); // windowNo, context, onlyWindow
            if (fromWarehouse_ID == "")
                fromWarehouse_ID = 0;
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/LoadWarehouse",
                data: { fromWarehouse_ID: fromWarehouse_ID },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        for (var i = 0; i < result.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(result[i].ID);
                            value = VIS.Utility.Util.getValueOfString(result[i].Name);
                            $cmbToWarehouse.append(" <option value=" + key + ">" + value + "</option>");
                        }
                    }
                    $cmbToWarehouse.prop('selectedIndex', 0);

                    // load To Locator
                    loadToLocator();
                },
                error: function (er) {
                    busyDiv('hidden');
                    console.log(er);
                }
            });
        };

        // load To Locator
        function loadToLocator() {
            var fromWarehouse_ID = ($cmbToWarehouse.val() == "0" || $cmbToWarehouse.val() == null) ? 0 : parseInt($cmbToWarehouse.val());
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/LoadLocator",
                data: { fromWarehouse_ID: fromWarehouse_ID },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        $cmbToLocator.empty();
                        $cmbToLocator.append("<option value= 0 >Select</option>");
                        for (var i = 0; i < result.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(result[i].ID);
                            value = VIS.Utility.Util.getValueOfString(result[i].Name);
                            $cmbToLocator.append(" <option value=" + key + ">" + value + "</option>");
                        }
                    }
                    else {
                        $cmbToLocator.append("<option value= 0 >Select</option>");
                    }
                    $cmbToLocator.prop('selectedIndex', 0);
                },
                error: function (er) {
                    busyDiv('hidden');
                    console.log(er);
                }
            });
        };

        function LoadProductContainerGrid(containerId, isReload, prvNextNo) {

            pageNoContainer = 1, gridPgnoContainer = 1, containerRecord = 0;
            isContainerGridLoaded = false;
            //var container = ($cmbFromContainer.val() == "0" || $cmbFromContainer.val() == null) ? 0 : parseInt($cmbFromContainer.val());
            var AD_Org_ID = parseInt(VIS.context.getWindowContext($self.windowNo, "AD_Org_ID", true)); // windowNo, context, onlyWindow
            var date = VIS.context.getWindowContext($self.windowNo, "MovementDate", true); // windowNo, context, onlyWindow
            var locator = ($cmbFromLocator.val() == "0" || $cmbFromLocator.val() == null) ? 0 : parseInt($cmbFromLocator.val());
            //if grid is reloading than store the value of page combo in pageNoContainer
            if (isReload) {
                pageNoContainer = parseInt(cmbPage.val());
                if (prvNextNo != 0) {
                    if (prvNextNo == -1) {
                        pageNoContainer = pageNoContainer - 1;
                        if (pageNoContainer < 1)
                            pageNoContainer = 1;
                    }
                    else if (prvNextNo == 1) {
                        pageNoContainer = pageNoContainer + 1;
                        if (pageNoContainer > ttlPages)
                            pageNoContainer = ttlPages;
                    }
                }
            }
            //if (containerId != 0 || locator != 0) {
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/MoveContainer",
                data: { container: containerId, movementDate: date, AD_Org_ID: AD_Org_ID, locator: locator, page: pageNoContainer, size: PAGESIZE },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        if (pageNoContainer == 1 && result.length > 0) {
                            containerRecord = result[0].ContainerRecord;
                            gridPgnoContainer = Math.ceil(containerRecord / PAGESIZE);
                        }
                        createContainerGrid(result);
                        if (!isReload) {
                            ttlPages = gridPgnoContainer;
                            liCurrPage.find("option").remove()
                            if (gridPgnoContainer > 0) {
                                for (var i = 0; i < gridPgnoContainer; i++) {
                                    cmbPage.append($("<option value=" + (i + 1) + ">" + (i + 1) + "</option>"));
                                }
                            }
                            resetPageCtrls(CurrentPage, gridPgnoContainer);
                            cmbPage.val(pageNoContainer);
                        }
                        else {
                            CurrentPage = pageNoContainer;
                            cmbPage.val(CurrentPage);

                            if (CurrentPage > 1)
                                liPrevPage.css("opacity", "1");
                            else
                                liPrevPage.css("opacity", "0.6");

                            busyDiv('hidden');
                        }
                        //$containerGrid.find('#grid_' + grdname + '_records').on('scroll', cartContainerScroll);
                        isContainerGridLoaded = true;
                    }
                },
                error: function (er) {
                    busyDiv('hidden');
                    console.log(er);
                }
            });
            //}
        };

        /** Create Grids**/
        function createContainerGrid(data) {
            var grdCols = [];
            var grdRows = [];
            //  this.gridRowsData = [];
            grdCols.push({ field: "ProductName", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ProductName"), size: "20%" });
            grdCols.push({ field: "ASI", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ASI"), size: "15%" });
            grdCols.push({ field: "UomName", caption: VIS.Msg.translate(VIS.Env.getCtx(), "UomName"), size: "15%" });
            grdCols.push({ field: "ContainerQty", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ContainerQty"), size: "10%" });
            grdCols.push({
                field: "MoveQty", caption: VIS.Msg.translate(VIS.Env.getCtx(), "MoveQty"),
                editable: { type: 'float' }, render: 'number:4', sortable: false, size: "10%"
            });
            grdCols.push({ field: "M_ProductContainer_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_ProductContainer_ID"), hidden: true });
            grdCols.push({ field: "M_PRODUCT_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_PRODUCT_ID"), hidden: true });
            grdCols.push({ field: "M_AttributeSetInstance_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_AttributeSetInstance_ID"), size: "15%", hidden: true });
            grdCols.push({ field: "C_Uom_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_Uom_ID"), size: "15%", hidden: true });
            grdCols.push({ field: "M_Transaction_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_Transaction_ID"), size: "15%", hidden: true });


            for (var j = 0; j < data.length; j++) {
                var row = {};
                //row['Select'] = false;
                row['ProductName'] = data[j].ProductName;
                row['ASI'] = data[j].ASI;
                row['UomName'] = data[j].UomName;
                row['ContainerQty'] = data[j].ContainerQty;
                row['M_ProductContainer_ID'] = data[j].M_ProductContainer_ID;
                row['M_Product_ID'] = data[j].M_Product_ID
                row['M_AttributeSetInstance_ID'] = data[j].M_AttributeSetInstance_ID;
                row['C_Uom_ID'] = data[j].C_Uom_ID;

                if (multiselectData.length > 0) {
                    var obj = $.map(multiselectData, function (value) {
                        if (value.M_Product_ID == data[j].M_Product_ID && value.M_AttributeSetInstance_ID == data[j].M_AttributeSetInstance_ID) {
                            return value;
                        }
                    });
                    if (obj.length > 0)
                        row['MoveQty'] = obj[0].MoveQty;
                    else
                        row['MoveQty'] = 0;
                }
                else
                    row['MoveQty'] = 0;

                row['recid'] = j + 1;
                grdRows[j] = row;
            }
            //end
            // create global array grid for paging
            //  gridRowsData.push(grdRows);

            grdname = 'MCont_gridContainer' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(grdRows);
            CGrid = $containerGrid.w2grid({
                name: grdname,
                recordHeight: 20,
                show: {
                    toolbar: false,  // indicates if toolbar is visible
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: false,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: false,   // indicates if toolbar delete button is visible
                    toolbarSave: false,   // indicates if toolbar save button is visible
                    selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    recordTitles: false	 // indicates if to define titles for records

                },
                columns: grdCols,
                records: grdRows,
                multiSelect: false,
                onClick: function (event) {
                },
                onChange: function (event) {
                    if (event.column == 4 && event.value_new && event.value_new != 0) {
                        var MoveQty = event.value_new;
                        var ContainerQty = w2ui[grdname].get(event.recid).ContainerQty;
                        if (ContainerQty < 0 && MoveQty > 0) {
                            VIS.ADialog.error('VIS_CantbePositive');
                            event.value_new = 0;
                            return false;
                        }
                        if (Math.abs(MoveQty) > Math.abs(ContainerQty)) {
                            VIS.ADialog.error('VIS_CantGreater');
                            event.value_new = 0;
                            return false;
                        }
                        //Push the data into array for multiple selection
                        pushDataIntoArrayForSave(MoveQty, event.recid);
                    }
                }
            });

            if ($moveFullContainer.is(":checked") || $withoutContainer.is(":checked")) {
                w2ui[grdname].lock('');
            }
        };

        // Save MovementLine
        function SaveMovementLine() {
            // initialize array
            var moveData = [];

            // check Full container 
            var isMoveContainer = $moveFullContainer.is(":checked");
            var isWithoutContainer = false;
            isWithoutContainer = $withoutContainer.is(":checked");

            if (isMoveContainer && $cmbFromContainer.val() == null) {
                // In Full Move Container movement, From Container is mandatory
                VIS.ADialog.error("VIS_FullContainerMove", true, "", "");
                return false;
            }

            var FromContainer = ($cmbFromContainer.val() == "0" || $cmbFromContainer.val() == null) ? 0 : parseInt($cmbFromContainer.val());
            var ToContainer = ($cmbToContainer.val() == "0" || $cmbToContainer.val() == null) ? 0 : parseInt($cmbToContainer.val());
            if (FromContainer == ToContainer) {
                //From and To Container can't be same.
                VIS.ADialog.error("VIS_ContainerCantSame", true, "", "");
                return false;
            }

            // check length of selected record
            var countRecordChanges = multiselectData.length;
            if (countRecordChanges == 0 && !isMoveContainer && !isWithoutContainer) {
                VIS.ADialog.error("VIS_MoveQtyNotSelected", true, "", "");
                return false;
            };
            // get record Id
            var recordId = VIS.context.getWindowContext($self.windowNo, "M_Movement_ID", true);

            if (countRecordChanges > 0 || isMoveContainer || isWithoutContainer) {
                if (!isMoveContainer && !isWithoutContainer && countRecordChanges > 0) {
                    $.each(multiselectData, function () { this.ToLocator = parseInt($cmbToLocator.val()); this.ToContainer = ($cmbToContainer.val() == "0" || $cmbToContainer.val() == null) ? 0 : parseInt($cmbToContainer.val()); });
                    moveData = multiselectData;
                }

                // when move full container (with or without qty)
                if (isMoveContainer || isWithoutContainer) {
                    moveData = [];
                    for (var i = 0; i < w2ui[grdname].records.length; i++) {
                        var row = w2ui[grdname].records[i];
                        moveData.push({
                            M_Product_ID: row.M_Product_ID,
                            M_AttributeSetInstance_ID: row.M_AttributeSetInstance_ID,
                            C_UOM_ID: row.C_Uom_ID,
                            FromLocator: parseInt($cmbFromLocator.val()),
                            ToLocator: parseInt($cmbToLocator.val()),
                            FromContainer: ($cmbFromContainer.val() == "0" || $cmbFromContainer.val() == null) ? 0 : parseInt($cmbFromContainer.val()),
                            ToContainer: ($cmbToContainer.val() == "0" || $cmbToContainer.val() == null) ? 0 : parseInt($cmbToContainer.val()),
                            MoveQty: row.ContainerQty,
                            M_Movement_ID: recordId,
                            IsFullMoveContainer: isMoveContainer ? true : false,
                            IsfullContainerQtyWise: isWithoutContainer ? true : false
                        });
                        break;
                    };
                    // when we move parent container and parent container not having any product, but child have 
                    // then pass this limited inforrmation to server side -- for picking data of child record fo movement
                    if (moveData.length == 0 && isMoveContainer) {
                        moveData.push({
                            M_Product_ID: 0,
                            M_AttributeSetInstance_ID: 0,
                            C_UOM_ID: 0,
                            FromLocator: parseInt($cmbFromLocator.val()),
                            ToLocator: parseInt($cmbToLocator.val()),
                            FromContainer: ($cmbFromContainer.val() == "0" || $cmbFromContainer.val() == null) ? 0 : parseInt($cmbFromContainer.val()),
                            ToContainer: ($cmbToContainer.val() == "0" || $cmbToContainer.val() == null) ? 0 : parseInt($cmbToContainer.val()),
                            MoveQty: 0,
                            M_Movement_ID: recordId,
                            IsFullMoveContainer: isMoveContainer ? true : false,
                            IsfullContainerQtyWise: isWithoutContainer ? true : false
                        });
                    }
                };

                if (moveData.length == 0) {
                    VIS.ADialog.error("VIS_MoveQtyNotSelected", true, "", "");
                    return false;
                }

                // Save Record
                $.ajax({
                    url: VIS.Application.contextUrl + "ProductContainer/SaveMoveData",
                    type: 'POST',
                    data: ({
                        moveData: JSON.stringify(moveData)
                    }),
                    success: function (result) {
                        if (JSON.parse(result) == "VIS_SuccessFullyInserted") {
                            VIS.ADialog.info("VIS_SuccessFullyInserted", true, "", "");
                        }
                        else {
                            VIS.ADialog.error("Error", true, VIS.Msg.translate(VIS.Env.getCtx(), "VIS_NotInserted") + " - " + result, "");
                        }
                        //LoadProductContainerGrid(0 , false, 0);
                        $moveFullContainer.prop("checked", false);
                        $withoutContainer.prop("checked", false);
                        multiselectData = [];// Empty Array
                        $self.frame.close();
                    },
                    error: function (result) {
                        //disposeComponent();
                        //LoadProductContainerGrid(0, false , 0);
                        multiselectData = [];// Empty Array
                        $self.frame.close();
                        busyDiv('hidden');
                    }
                });
            }
        };

        function pushDataIntoArrayForSave(MoveQty, recid) {

            var isMoveContainer = $moveFullContainer.is(":checked");
            var isWithoutContainer = $withoutContainer.is(":checked");
            // get record Id
            var recordId = VIS.context.getWindowContext($self.windowNo, "M_Movement_ID", true);
            var countRecordChanges = w2ui[grdname].getChanges().length;
            // when product move from continer to container
            if (MoveQty != 0) {
                var row = w2ui[grdname].get(recid);
                multiselectData.push({
                    M_Product_ID: row.M_Product_ID,
                    M_AttributeSetInstance_ID: row.M_AttributeSetInstance_ID,
                    C_UOM_ID: row.C_Uom_ID,
                    FromLocator: parseInt($cmbFromLocator.val()),
                    ToLocator: parseInt($cmbToLocator.val()),
                    FromContainer: ($cmbFromContainer.val() == "0" || $cmbFromContainer.val() == null) ? 0 : parseInt($cmbFromContainer.val()),
                    ToContainer: ($cmbToContainer.val() == "0" || $cmbToContainer.val() == null) ? 0 : parseInt($cmbToContainer.val()),
                    MoveQty: MoveQty,
                    M_Movement_ID: recordId,
                    IsFullMoveContainer: isMoveContainer ? true : false,
                    IsfullContainerQtyWise: isWithoutContainer ? true : false
                });
            }
        };

        // Busy Indicator
        function busyDiv(Value) {
            $bsyDiv[0].style.visibility = Value;
        };

        this.getRoot = function () {
            return $root;
        };

        disposeComponent = function () {
            $mainpageContent = null;
            $formWrap = null;
            $formDataRow = null;
            $formData = null;
            $formDataR = null;
            $formData3 = null;

            $lblFromWarehouse = null;
            $cmbFromWarehouse = null;
            $lblToWarehouse = null;
            $cmbToWarehouse = null;

            $lblFromLocator = null;
            $cmbFromLocator = null;
            $lblToLocator = null;
            $cmbToLocator = null;

            $lblFromContainer = null;
            $cmbFromContainer = null;
            $lblToContainer = null;
            $cmbToContainer = null;

            // configuration for open Popup
            config = null;
            $fromContainerPopUp = null;
            fromContainerDialog = null;

            $fromContainerParent;
            $fromContainerParentChild = null;

            $moveFullContainer = null;
            $lblMoveFullContainer = null;
            $withoutContainer = null;
            $lblWithoutContainer = null;
            $withContainer = null;
            $lblWithContainer = null;


            $formRightWrap = null;
            $containerGrid = null;
            CGrid = null;
            this.grdname = null;

            $actionPanel = null;
            $buttons = null;
            $btnApply = null;
            $btnOk = null;
            $btnCancel = null;

            PAGESIZE = null;
            pageNoContainer = null, gridPgnoContainer = null, containerRecord = null;
            isContainerGridLoaded = null;

            $self = null;
            $root = null;
        };
    };

    productContainerMove.prototype.init = function (windowNo, frame) {

        this.frame = frame;
        this.windowNo = windowNo;
        var obj = this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
    };

    //Must implement dispose
    productContainerMove.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        disposeComponent();

        //call frame dispose function
        this.frame = null;
    };


    VIS.AForms.productContainerMove = productContainerMove;
})(VIS, jQuery);