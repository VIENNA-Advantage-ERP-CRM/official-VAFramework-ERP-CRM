/********************************************************
 * Module Name    :     Application
 * Purpose        :     Product Container Tree Structure
 * Author         :     Amit Bansal
 * Date           :     22-Oct-2018 
  ******************************************************/

; (function (VIS, $) {
    // this constructor is having 3 parameter - based on this parameter - show Product Container Tree
    function productContainerTree(warehouseId, locatorId, containerRef, validation) {
        var $self = this;
        this.onClose = null;
        this.onClosing = null;

        // configuration for open Popup
        var $ContainerPopUp = null;
        var ContainerDialog = null;
        var $ContainerParent;
        var $ContainerParentChild = null;
        var $btnSearch = null;
        var $txtSearch = null;
        var $btnSave = null;
        var $btnAdd = null;
        var $btnCancel = null;
        var divContainer = null;
        var $_DialogOKCancelProductContainer;
        var $labelInfo = null;

        this.showDialog = function () {
            var param;
            var info = "";
            if (validation != null) {
                param = getLocatorFromValidation(validation)
            } else if (locatorId > 0) {
                param = locatorId;
            };
            if (param != null) {
                info = VIS.dataContext.getJSONRecord("ProductContainer/GetWarehouseAndLocatorName", param);
            }
            // var info = "avc";
            // Create Dialog design
            $ContainerPopUp = $("<div class='vis-formouterwrpdiv' style='float: left; width: 100%;min-height:480px; max-height:480px !important'></div>");
            var $_ContainerPopUp;
            var $_CreateNewProductContainer;
            //var $_CancelProductContainer;

            $_CreateNewProductContainer = $("<div id='AddNewContainer' style='float: left; width: 100%; display:none;'>"

                                            + "<div class='VIS_form-data-row'>"
                                            + "<div class='VIS_form-data-col input-group vis-input-wrap'><div class='vis-control-wrap'>" // style='float: left; width: 50%; margin-bottom:10px;
                                            + "<input id='VIS_SearchValue' type='text' placeholder=' ' data-placeholder=''><label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Value") + "</label>"
                                            + "</div></div>" // style='width: 96%;'

                                            + "<div class='VIS_form-data-col input-group vis-input-wrap'><div class='vis-control-wrap'>" // style='float: left; width: 50%; margin-bottom:10px;'
                                            + "<input id='VIS_Name' type='text' class='vis-ev-col-mandatory' placeholder=' ' data-placeholder=''><label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Name") + "</label>"
                                            + "</div></div>" //style='width: 96%;'
                                            + "</div>"

                                            + "<div class='VIS_form-data-row'>"
                                            + "<div class='VIS_form-data-col input-group vis-input-wrap'><div class='vis-control-wrap'>" // style='float: left; width: 50%; margin-bottom:10px;'
                                            + "<input id='VIS_Height' type='Number' placeholder=' ' data-placeholder=''><label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Height") + "</label>"
                                            + "</div></div>" //style='width: 96%;'

                                            + "<div class='VIS_form-data-col input-group vis-input-wrap'><div class='vis-control-wrap'>" //style='float: left; width: 50%; margin-bottom:10px;'
                                            + "<input id='VIS_Width' type='Number' placeholder=' ' data-placeholder=''><label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Width") + "</label>"
                                            + "</div></div>" // style='width: 96%;'
                                            + "</div>"

                                            + "<div class='VIS_form-data-row' style='display: none;'>"
                                            + "<div class='VIS_form-data-col'>" //style='float: left; width: 50%; margin-bottom:10px;'
                                            + "<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Ref_M_Container_ID") + "</label>"
                                            + "</div>"
                                            + "</div>"

                                            + "</div>");

            $_ContainerPopUp = $("<div class='VIS_form-data-row'>"
                               + "<div class='VIS_form-search-wrap input-group vis-input-wrap'><div class='vis-control-wrap'>"
                               + "<input class='VIS_form-search-wrap input' id='VIS_Search' type='text' placeholder='Search Here...' data-hasbtn=' '></div>"
                               + "<div class='input-group-append'><button class='VIS_form-search-wrap button input-group-text' id=VIS_BtnSearch title=" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_Search") + "><i class='vis vis-search'></i></button>"
                               + "<button class='VIS_form-search-wrap button input-group-text' id=VIS_BtnAdd title=" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_NewContainer") + "><i class='vis vis-plus'></i></button>"
                               + "<button class='VIS_form-search-wrap button input-group-text' style='display:none;' id=VIS_BtnSave title=" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_SaveContainer") + "><i class='vis vis-save'></i></button>"
                               + "<button class='VIS_form-search-wrap button input-group-text' style='display:none;' id=VIS_BtnCancel title=" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_Cancel") + "><i class='vis vis-mark'></i></button>"
                               + "</div></div>");

            $ContainerParentChild = $("<div id='VIS_ContainerParentChild' class='VIS_Tree-Container' ></div>"); //style='height:400px; float: left; width: 100%;'

            //$_CancelProductContainer = $("<div id='VIS_CancelProductContainer' class='VIS_buttons-wrap VIS_pop-btn-wrap'>"
            //                            + " <span class='btn' style='display:none;' id=VIS_BtnCancel1>" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_Cancel") + "</span>"
            //                            + " </div>");

            $_DialogOKCancelProductContainer = $("<div id='VIS_DialogOKCancelProductContainer' class='VIS_buttons-wrap VIS_pop-btn-wrap' style='display:block;'>"
                                        + " <span class='btn' id=VIS_DialogOK>" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_DialogOK") + "</span>"
                                        + " <span class='btn' id=VIS_DialogCancel>" + VIS.Msg.translate(VIS.Env.getCtx(), "VIS_DialogCancel") + "</span>"
                                        + " </div>");

            $labelInfo = $("<div id='VIS_LabelInfo' class='VIS_Tree-Container-labelInfo'>" // style='height:10px; float: left; width: 100%; display:none;'
                           + "<label>" + info + "</label>"
                           + "</div>");

            $_ContainerPopUp.append($ContainerParentChild);
            $ContainerPopUp.append($_CreateNewProductContainer).append($_ContainerPopUp).append($_DialogOKCancelProductContainer).append($labelInfo);
            ContainerDialog = new VIS.ChildDialog();
            ContainerDialog.setContent($ContainerPopUp);
            ContainerDialog.setTitle(VIS.Msg.translate(VIS.Env.getCtx(), "VIS_Container"));
            ContainerDialog.setWidth("450px");
            ContainerDialog.setEnableResize(false);
            ContainerDialog.setModal(true);
            ContainerDialog.show();

            ContainerDialog.hidebuttons();



            //Get Control Search
            $btnSearch = $ContainerPopUp.find("#VIS_BtnSearch");
            $txtSearch = $ContainerPopUp.find("#VIS_Search");


            $searchInput = $ContainerPopUp.find("#VIS_SearchValue");
            $NameInput = $ContainerPopUp.find("#VIS_Name");
            $HeightInput = $ContainerPopUp.find("#VIS_Height");
            $WidthInput = $ContainerPopUp.find("#VIS_Width");
            $btnAdd = $ContainerPopUp.find("#VIS_BtnAdd");
            $btnSave = $ContainerPopUp.find("#VIS_BtnSave");
            $btnCancel = $ContainerPopUp.find("#VIS_BtnCancel");

            $DialogbtnOk = $ContainerPopUp.find("#VIS_DialogOK");
            $DialogbtnCancel = $ContainerPopUp.find("#VIS_DialogCancel");

            // OK Click Event
            //ContainerDialog.onOkClick = function () {
            $DialogbtnOk.click(function (e) {
                var treeStructure = $ContainerParentChild;
                var length = $(treeStructure).find(".current").length;
                if (length > 1) {
                    VIS.ADialog.error("VIS_SelectSingleContainer", true, "", "");
                    return false;
                }
                if (length > 0) {
                    var containerId = parseInt($(treeStructure).find(".current")[0].htmlFor);
                    var containerText = $($(treeStructure).find(".current")[0]).text();

                    // on close passing container ID and its name -- from where this dialog is opened
                    //ContainerDialog.onClose = function () {

                    $self.onClosing(containerId, containerText);
                    ContainerDialog.close();
                    //if ($self.onClose) $self.onClose();
                    //}
                }
                else {
                    ContainerDialog.close();
                }
            });

            // Close dialog box
            $DialogbtnCancel.click(function (e) {
                ContainerDialog.close();
            });


            if (containerRef) {
                $txtSearch.val(containerRef);
            }

            // Load data
            loadgrdContainer(callbackController);

            //Add Button Click event
            $btnAdd.click(function (e) {
                // Show control of -- creation of new product Container
                ShowNewContainerControl();
            });

            //Cancel Button Click event
            $btnCancel.click(function (e) {
                // hide control of -- creation of new product Container
                HideNewContainerControl();

                // clear container fields
                clearContainerfields();
            });

            //Save Button Click event
            $btnSave.click(function (e) {
                // Container Name should be mandatory
                if ($NameInput.val() == "") {
                    VIS.ADialog.error("VIS_ContainerNameNotSelected", true, "", "");
                    return false;
                }
                var abc = validation;
                // Need to define wither waehouse of locator for saving product container
                if (warehouseId == "" && locatorId == "" && validation == "") {
                    VIS.ADialog.error("VIS_WarehouseOrLocatorNotFound", true, "", "");
                    return false;
                }
                else if (validation != null) {
                    var token;
                    validation = validation.toUpperCase();
                    if (validation.contains("M_LOCATOR_ID")) {
                        token = validation.substr(validation.indexOf("M_LOCATOR_ID=") + 13, validation.length);
                    } else if (validation.contains("M_LOCATORTO_ID")) {
                        token = validation.substr(validation.indexOf("M_LOCATORTO_ID=") + 15, validation.length);
                    }
                    if (token != undefined) {
                        if (token.contains("AND")) {
                            locatorId = token.substr(0, token.indexOf("AND")).trim();
                        }
                        else {
                            locatorId = token.trim();
                        }
                        if (parseInt(locatorId) == 0) {
                            VIS.ADialog.error("VIS_WarehouseOrLocatorNotFound", true, "", "");
                        }
                    }
                }

                // find parent container 
                var treeStructure = $ContainerParentChild;
                var length = $(treeStructure).find(".current").length;
                var parentContainerId = 0;
                if (length <= 1) {
                    if ($(treeStructure).find(".current")[0] != undefined) {
                        parentContainerId = parseInt($(treeStructure).find(".current")[0].htmlFor);
                    }
                }
                else {
                    VIS.ADialog.error("VIS_SelectSingleContainer", true, "", "");
                    return false;
                }

                // Save new record of Product Container
                SaveContainer($searchInput.val(),
                              $NameInput.val(),
                              $HeightInput.val() == "" ? 0 : $HeightInput.val(),
                              $WidthInput.val() == "" ? 0 : $WidthInput.val(),
                              parentContainerId);
            });

        };


        function getLocatorFromValidation(validation) {
            var token;
            validation = validation.toUpperCase();
            if (validation.contains("M_LOCATOR_ID")) {
                token = validation.substr(validation.indexOf("M_LOCATOR_ID=") + 13, validation.length);
            } else if (validation.contains("M_LOCATORTO_ID")) {
                token = validation.substr(validation.indexOf("M_LOCATORTO_ID=") + 15, validation.length);
            }
            if (token != undefined) {
                if (token.contains("AND")) {
                    token = token.substr(0, token.indexOf("AND")).trim();
                }
                else {
                    token = token.trim();
                }
            }
            return token;
        };

        // Get From Controller Data 
        function loadgrdContainer(callback) {

            $.ajax({
                url: VIS.Application.contextUrl + "ProductContainer/LoadContainerAsTree",
                type: "GET",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                data: ({ warehouse: warehouseId, locator: locatorId, container: 0, validation: validation }),
                success: function (data) {
                    callback(data);
                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorLoadingFromController");
                }
            });
        };

        // callback function for loading containers in Tree Structure
        function callbackController(data) {
            $ContainerParent = '<div id=containerParent style="width:100%;height:400px">'
                                    + '<div>'
                                    + '<div class="well VIS_tree-container" style="width:100%;float: left;">'
                                    + '<div id=containerParent1 style="overflow-y: auto;height: 383px;">'
                                    + '<ul class="nav nav-list">';
            var result = JSON.parse(data);
            for (var j = 0; j < result.length; j++) {
                // Represent Parent
                if (j == 0) {
                    Ascendent(result[j], result[j].Level);
                    continue;
                }
                    // Represent Child
                else if (result[j - 1].Level < result[j].Level) {
                    $ContainerParent += '<ul class="nav nav-list tree" style="display: none;">';
                    Ascendent(result[j], result[j].Level);

                    if (j + 1 == result.length) {
                        $ContainerParent += '</li> </ul>';
                    }
                    continue;
                }
                    // represent also child
                else if (result[j - 1].Level == result[j].Level && result[j].Level != 1) {
                    $ContainerParent += '</li>';
                    Ascendent(result[j], result[j].Level);

                    if (j + 1 == result.length) {
                        $ContainerParent += '</li>';
                    }
                    continue;
                }
                    // represent also child
                else if (result[j - 1].Level > result[j].Level && result[j].Level != 1) {
                    // closingTag();
                    if (result[j - 1].Level > result[j].Level) {
                        var level = (result[j - 1].Level) - (result[j].Level);
                        for (var l = 0; l < level ; l++) {
                            closingTag();
                        }
                    }
                    Ascendent(result[j], result[j].Level);

                    if (j + 1 == result.length) {
                        $ContainerParent += '</li>';
                    }
                    continue;
                }
                    // Represent Parent
                else if (result[j].Level == 1) {
                    if (result[j - 1].Level > result[j].Level) {
                        var level = (result[j - 1].Level) - 1;
                        for (var l = 0; l < level ; l++) {
                            closingTag();
                        }
                    }
                    $ContainerParent += '</li>';
                    $ContainerParent += '<li class="divider"></li>';
                    Ascendent(result[j], result[j].Level);
                    if (j + 1 == result.length) {
                        $ContainerParent += '</li>';
                    }
                    continue;
                }
            }

            $ContainerParent += '</ul> </div> </div> </div> </div>';
            //$ContainerParentChild.append($ContainerParent);
            $("#VIS_ContainerParentChild").html($ContainerParent);
            $ContainerParentChild = $("#VIS_ContainerParentChild")[0];

            //To add "overflow: hidden" property on the content dialog 
            //$($ContainerParentChild).parents('.ui-dialog-content').css('overflow', 'hidden');

            $('label.tree-toggler').click(function (e) {
                var treeStructure = $($ContainerParentChild);
                // remove class
                treeStructure.find(".nav-header").removeClass("current");
                // Add class
                //$($(this).parent().context).addClass('current')
                // after bootstrap 4
                $($(this)).addClass('current');
                // toggle records
                $(this).parent().children('ul.tree').toggle(300);
            });

            //Search Button Click event
            $btnSearch.click(function (e) {
                if ($txtSearch.val() != "") {
                    searchResults($($ContainerParentChild), $txtSearch.val());
                }
                if ($txtSearch.val() == "") {
                    //remove class
                    $($ContainerParentChild).find(".nav-header").removeClass("current");
                }
            });


            if (containerRef) {
                $btnSearch.trigger("click");
            }

            // is used to search container in Tree
            $(document).keypress(function (event) {

                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) event.preventDefault();
                if (keycode == '13') {
                    if ($txtSearch.val() != "") {
                        searchResults($($ContainerParentChild), $txtSearch.val());
                    }
                    else {
                        // remove class
                        // $($ContainerParentChild).find(".nav-header").removeClass("current");
                    }
                }
            });

            // is used to set update Interface Of Container Name
            $(document).keyup(function (event) {
                if ($NameInput.val() != "") {
                    $NameInput.css('background-color', SetMandatory(false));
                }
                else {
                    $NameInput.css('background-color', SetMandatory(true));
                }
            });
        };

        // Creating (ul - li) tag for Tree
        function Ascendent(record, level) {
            if (level == 1)
                $ContainerParent += '<li data-value=' + record.M_ProductContainer_ID + ' class="First">';
            else
                $ContainerParent += '<li data-value=' + record.M_ProductContainer_ID + '>';

            $ContainerParent += '<label class="tree-toggler nav-header" for=' + record.M_ProductContainer_ID + '>' + record.ContainerName + ''
              + '<span class="vis-treewindow-span"><span class="vis-css-treewindow-arrow-up"></span></span></label>';
        };

        // closing tag 
        function closingTag() {
            $ContainerParent += '</li> </ul>';
        };

        // use for searching container by its name
        function searchResults(e, val) {
            var labelFound = e.find("label:contains('" + val + "')");
            $("label").removeClass("current");
            labelFound.addClass('current');
            //Toggle
            if ($(labelFound).parents(".First").find('ul.tree:last-child').css('display') == 'none') {
                $(labelFound).parents(".First").find('ul.tree:last-child').toggle();
                $(labelFound).parents(".First").find('ul.tree:last-child').css('display', 'block');
            }
            else {
                $(labelFound).parents(".First").find('ul.tree:last-child').css('display', 'block');
            }
            labelFound[0].scrollIntoView();
        };

        // Save new record of Product Container
        function SaveContainer(value, name, height, width, parentContainerId) {
            $.ajax({
                url: VIS.Application.contextUrl + "ProductContainer/SaveContainer",
                datatype: "json",
                async: false,
                contentType: "application/json; charset=utf-8",
                data: ({ warehouseId: warehouseId, locatorId: locatorId, value: value, name: name, height: height, width: width, parentContainerId: parentContainerId }),
                success: function (data) {
                    if (JSON.parse(data) == "") {
                        VIS.ADialog.info("VIS_ContainerInserted", true, "", "");
                        $($ContainerParentChild).empty();
                        $ContainerParentChild = $("<div id='VIS_ContainerParentChild' style='height:382px; float: left; width: 100%;'></div>");
                        // Load data
                        loadgrdContainer(callbackController);

                        // hide control of -- creation of new product Container
                        HideNewContainerControl();

                        // clear container fields
                        clearContainerfields();
                    }
                    else {
                        VIS.ADialog.error("", true, JSON.parse(data), "");
                    }
                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorLoadingFromController");
                }
            });
        };

        // show Inteface as mandatory or not
        function SetMandatory(value) {
            if (value)
                return '#FFB6C1';
            else
                return 'White';
        };

        //hide control of -- creation of new product Container
        function HideNewContainerControl() {
            divContainer = $ContainerPopUp.find("#AddNewContainer");
            divContainer[0].style.display = "none";

            $btnAdd[0].style.display = "flex";
            $btnSave[0].style.display = "none";

            $btnCancel[0].style.display = "none";

            $ContainerPopUp.find("#VIS_DialogOKCancelProductContainer")[0].style.display = "block";

            $ContainerPopUp.find("#containerParent").css('height', '400px');
            $ContainerPopUp.find("#containerParent1").css('height', '383px');
            $ContainerPopUp.find("#VIS_ContainerParentChild").css('height', '382px');

            $labelInfo[0].style.display = "none";
        };

        //show control of -- creation of new product Container
        function ShowNewContainerControl() {
            divContainer = $ContainerPopUp.find("#AddNewContainer");
            divContainer[0].style.display = "block";

            $btnAdd[0].style.display = "none";
            $btnSave[0].style.display = "flex";

            $btnCancel[0].style.display = "flex";

            $ContainerPopUp.find("#VIS_DialogOKCancelProductContainer")[0].style.display = "none";

            $ContainerPopUp.find("#containerParent").css('height', '295px');
            $ContainerPopUp.find("#containerParent1").css('height', '275px');
            $ContainerPopUp.find("#VIS_ContainerParentChild").css('height', '265px');

            $labelInfo[0].style.display = "block";
        }

        // Clear Text fields
        function clearContainerfields() {
            $searchInput.val("");
            $NameInput.val("");
            $HeightInput.val("");
            $WidthInput.val("");
        };

        jQuery.expr[':'].contains = function (a, i, m) {
            return jQuery(a).text().toUpperCase()
                .indexOf(m[3].toUpperCase()) >= 0;
        };

        this.disposeComponent = function () {
            $self = null;
            $ContainerPopUp = null;
            ContainerDialog = null;
            $ContainerParent;
            $ContainerParentChild = null;
            $btnSearch = null;
            $txtSearch = null;

            $labelInfo = null;

            $btnSave = null;
            $btnAdd = null;
            $btnCancel = null;
            divContainer = null;
            $_DialogOKCancelProductContainer;

            this.disposeComponent = null;
        };

    }

    //dispose call
    productContainerTree.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    //Load form into VIS
    VIS.productContainerTree = productContainerTree;

})(VIS, jQuery);