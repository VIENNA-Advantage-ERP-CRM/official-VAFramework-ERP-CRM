
; (function (VIS) { // scope

    /* main UI manager 
       - manage window desktop like look
       - manage home and other container 
       - manage menu and taskbar 
       - bind and handle events of menu and taskbar etc
       BG:234
    */

    function desktopMgr() {

        var searchSelectItem = null; // menu's search current item
        var currentActiveView = null; //active view
        var dynamicViewCache = {}; // all opened views
        var viewsZIndexCache = []; // Z Index of view
        var isDataListSupported = false;// Modernizr.input.list; // Browser support Datalist or Not
        //New
        var $home = $("#vis_home"); // Home Div
        var $section = $('#vis_mainSection'); //Outer main Area
        var $mainConatiner = $("#vis_mainConatiner"); //central-right main container has  [home page] and all opened views...
        var $shortcutUL = $("#vis_taskbar"); // [dynamic shortcut]

        var $vis_appMenu = $("#vis_appMenu"); // app menu
        var $vis_mainMenu = $("#vis_mainMenu"); // 
        var $vis_closeMenu = $("#vis_closeMenu"); // menu close button
        var $vis_CloudDaysLeft = $('#vis_cloud_daysleft');

        var $vis_menuSearch = $('#vis_menuSearch'); //Search Box
        var $menuTree = $('#vis_divTree');  //tree div
        var $menuOverlay = $("#vis_menuOverlay"); // Menu Overlay div

        var $mainNavigationDiv = null;

        var curSelTaskBarItem = null; //Current active TaskBar item

        // button for opening calling interface  
        //var $vis_appCall = $("#vis_appCall");

        /* main viewable area 
        */
        function getMainConatiner() {
            return $mainConatiner;
        };

        /* task bar  list
        */
        function getShortcutUL() {
            return $shortcutUL;
        };

        /*
        * layout and events binding
        */
        function bindEvents() {

            $menuOverlay.on(VIS.Events.onTouchStartOrClick, function (event) {
                event.preventDefault();
                hideMenu(); //Hide start menu
            });

            // menu tree click
            var time = Date.now();
            $menuTree.on("click", function (event) {
                menuItemClick(event);
            });

            $menuTree.on("keyDown", function (event) {
                if (event.which == 13) {
                    menuItemClick(event);
                }
            });

            function menuItemClick(event) {
                if (event.target.nodeName === "LABEL") {
                    if (Date.now() - time < 300) { //skip double click
                        event.preventDefault();
                        event.stopPropagation();
                        time = Date.now();
                        return;
                    }
                    time = Date.now();
                }

                if (event.target.nodeName === "A") {
                    var $target = $(event.target);
                    if ($target.data('isfavbtn') == 'yes') {
                        VIS.FavouriteHelper.showOverlay($target); // show menu item's options
                        return;
                    }
                    startMenuAction($target.data('action'), $target.data('actionid')); //start action
                    return;
                }
            };

            // task bar click event
            $shortcutUL.on(VIS.Events.onClick, function (event) {
                event.preventDefault();
                //if (VIS.context.getContext("#DisableMenu") == 'Y') {
                //    return;
                //}
                var $c = null;
                if (event.target.nodeName === "LI") {
                    $c = $(event.target);
                }
                else if (event.target.parentNode.nodeName === "LI") {
                    $c = $(event.target.parentNode);
                }
                if ($c) {

                    toggleContainer($c[0].id);
                    activateTaskBarItem($c);
                }
                $c = null;
            });


            $vis_appMenu.on("click", function (event) {

                //var disableMenu = VIS.context.getContext("#DisableMenu");
                if (VIS.MRole.getIsDisableMenu()) {
                    return;
                }
                event.preventDefault();
                window.setTimeout(function () {
                    $vis_menuSearch.focus();

                    $('#vis_divTree').on('focus', 'li', function (e) {
                        var $target = $(e.target);
                        $($target).off("keydown");
                        $($target).on("keydown", function (event) {
                            if (event.key == "Enter") {
                                if ($target.siblings('label').length > 0) {
                                    $($target.siblings('label')).trigger('click');
                                }
                                if ($target.siblings('a').length > 0) {
                                    $($target.siblings('label')).trigger('a')
                                }
                            }
                        });

                        $($target.siblings('label')).css('border', '1px solid #1aa0ed');
                    });

                    $('#vis_divTree').on('focusout', 'li', function (e) {
                        var $target = $(e.target);
                        $($target.siblings('label')).css('border', '1px solid white');
                    });


                }, 2);
                showMenu(); // show the menu
            });

            // call/show calling module 
            //$vis_appCall.on("click", function (event) {
            //    event.preventDefault();
            //    VA048.Apps.GetCallingInstance(true);
            //    return false;
            //});

            /* Main Menu Close */
            $vis_closeMenu.on("click", function (event) {
                //alert( "Handler for .click() called." );
                event.preventDefault();
                hideMenu();
            });

            if (isDataListSupported) {
                $vis_menuSearch.bind('input', function () {
                    searchAndStartAction($vis_menuSearch.val(), this.list);
                });
            }

            $(window).resize(adjustHeight);

            //this event work for IE,FireFox and Chrome
            //on other browsers not tested till now
            window.onbeforeunload = function (e) {

                $.ajax({
                    url: VIS.Application.contextUrl + "Account/LogOffHandler",
                    type: "POST"
                });
            };
            refreshSession(1);
            //window.setInterval(refreshSession, 1000 * 60 * 3); // every 1.5 minutes
        };

        /*
        Refresh session
        */

        //function refreshSession() {
        //    $.ajax({
        //        url: VIS.Application.contextUrl + "Account/Refresh",
        //        type: "POST"
        //    });
        //};
        function refreshSession(time) {
            window.setTimeout(function () {
                $.ajax({
                    url: VIS.Application.contextUrl + "Account/Refresh",
                    complete: function () {
                        //window.setTimeout(function () {
                        refreshSession(60 * 3);
                        //}, );
                    }
                });
            }, 1000 * time);
        };

        function getDaysLeft() {
            //console.log("start");
            //console.log(VIS.Application.contextFullUrl);
            if (VIS.Application.contextFullUrl.toLower().indexOf("softwareonthecloud.com") > -1) {
                $.ajax({
                    url: VIS.Application.contextUrl + "home/GetSubscriptionDaysLeft/",
                    success: function (result) {
                        //console.log("success");
                        var data = JSON.parse(result);
                        if (data) {
                            if (data == "True" || data == "") {
                                // console.log("none");
                                $vis_CloudDaysLeft.css("display", "none");
                            }
                            else {
                                // console.log("show");
                                if (data > 1) {
                                    $vis_CloudDaysLeft.text(VIS.Msg.getMsg("VIS_TrialPeriod") + ": " + data + " " + VIS.Msg.getMsg("VIS_TrialDaysLeft"));
                                }
                                else if (data == 1) {
                                    $vis_CloudDaysLeft.text(VIS.Msg.getMsg("VIS_TrialPeriod") + ": " + data + " " + VIS.Msg.getMsg("VIS_TrialDayLeft"));
                                }
                            }
                        }
                    },
                    error: function (e) {
                        // console.log("error");
                        // console.log(e);
                        $vis_CloudDaysLeft.css("display", "none");
                    }
                });
            }
            else {
                //  console.log("else");
                $vis_CloudDaysLeft.css("display", "none");
            }
        };

        /* 
          called when auto complete control fire  closed event
          * check the current search item
           - if found then start menu action 
        */
        function onAutoCompleteClose() {

            if (searchSelectItem && searchSelectItem.id && searchSelectItem.action) {
                startMenuAction(searchSelectItem.action, searchSelectItem.id);
                $vis_menuSearch.val("");
            }
            searchSelectItem = null;

        };

        /* Hide app menu
         - clear menu serach text
        */
        function hideMenu() {
            menuFilterMgr.closePopup();
            $vis_mainMenu.hide('fast');
            $menuOverlay.fadeOut();
            $vis_menuSearch.val("");
            $vis_menuSearch.blur();
        };

        /* show app menu */
        function showMenu() {
            $menuOverlay.fadeIn();
            $vis_mainMenu.show('fast');
        };

        function toggleMenu() {
            $menudiv.fadeToggle();
            $menuOverlay.fadeToggle();
        };

        /*set height of section (main container) to window size */
        function adjustHeight() {
            $section.css('height', window.innerHeight - 22);
            VIS.Env.setScreenHeight(window.innerHeight - 42 - 22);

            if (VIS.viewManager)
                VIS.viewManager.sizeChanged();

            // Resize event for calling interface
            //if (window.VA048 && VA048.Apps.GetCallingInstance(false))
            //    VA048.Apps.GetCallingInstance(false).resize();
        };

        /*
           start menu action
           - call view manger start action 
           - open corresponding view
           - hide the  menu
        */
        function startMenuAction(action, id) {
            VIS.viewManager.startAction(action, id);
            hideMenu();
        };

        /*
         *  change background color of active Task bar item
         *
         *@param itm
         */
        function activateTaskBarItem(itm) {
            //select unselect taskbar items
            if (curSelTaskBarItem) {
                curSelTaskBarItem.css('background-color', '');
            }
            curSelTaskBarItem = itm.css('background-color', '#D7E3E7');
            itm = null;
        };


        function activateTaskBarItemUsingID(item) {
            var nId = item.data('wid');
            activateTaskBarItem($shortcutUL.find("LI#" + nId));
        };

        /* 
           hide current active view 
           and get container by passed name or id to set as current active view
         *
         * @param  name name or id of view
         */

        function toggleContainer(name) {
            if (currentActiveView) {
                currentActiveView.hide();;
            }
            else {
                $mainConatiner.children('div').hide();
            }

            if (dynamicViewCache[name]) { //get from view list
                currentActiveView = dynamicViewCache[name];

                currentActiveView.css({
                    "opacity": 0
                });

                currentActiveView.show().animate({
                    opacity: 1
                }, 500, "linear", function () {

                    VIS.viewManager.resize(name); //resize view
                });
            }

            if (viewsZIndexCache.indexOf(name) != -1) { // maintain visible order of view
                viewsZIndexCache.splice(viewsZIndexCache.indexOf(name), 1);
            }
            viewsZIndexCache.push(name);
            hideMenu(); //hide menu
        };

        /* 
           get item from list by matching passed txt
          -get values from mating item and call start action function
        * 
        *@param txt matching text
        *@param list options list
        */
        function searchAndStartAction(txt, list) {
            if (txt === "" || txt.length < 1)
                return;
            for (var i = 0; i < list.options.length ; i++) {
                if (list.options[i].value === txt) { //matched
                    var $option = $(list.options[i]);
                    startMenuAction($option.data("action"), $option.data("actionid"));
                    $vis_menuSearch.val("")
                    break;
                }
            }
        };


        /* initialize action */
        function start() {
            polyFills();//implement fallback for Datalist and other newer controls 
            menuFilterMgr.init($menuTree, $('#vis_filterMenu')); //init Menu filter popup Manager
            authDialog.init($('#vis_home_ca'), $('#vis_userName')); //Init Authorization Dialog
            setAndLoadHomePage(); //Home Page
            historyMgr.restoreHistory(); //Restore History of App if any
            navigationInit();
        }

        /*
           add home div in view cache 
           - init home Manager js 
           - set active view to home page
        */
        function setAndLoadHomePage() {
            if (!VIS.MRole.getIsDisableMenu() ||( VIS.MRole.getIsDisableMenu() && VIS.MRole.getHomePage() == 0))
            {
                renderHomePage();
            }
            if (VIS.MRole.getIsDisableMenu())
            {
                $('.vis-topMenu').hide();
                $('.vis-menuHeaderLogo').show();
            }
            var homePageID = VIS.MRole.getHomePage();
            if (homePageID && parseInt(homePageID) > 0) {
                startMenuAction("X", homePageID);
            }
        };

        function renderHomePage()
        {
            $('#vis_lhome').show();
            dynamicViewCache['vis_lhome'] = $home;
            currentActiveView = $home.show();
            VIS.HomeMgr.initHome($home);
        }

        /*
         * add new dynamic view to list
         * set set as current active view
         *
         * @param id   unique key if view
         * @param view UI Element  
         * @return view 
         */
        function registerView(id, view) {
            dynamicViewCache[id] = view;
            viewsZIndexCache.push(id);

            toggleContainer(id);//show view
            return view;
            //return dynamicViewCache;
        };

        /*
         * remove view from list
         * remove from Task bar div
         * 
         * @param id   unique key if view
         */
        function unRegisterView(id) {

            $shortcutUL.find("LI#" + id).remove(); //Remove Taskbar Item

            var view = dynamicViewCache[id];
            dynamicViewCache[id] = null;
            delete dynamicViewCache[id];

            viewsZIndexCache.pop();

            view = null;
            var nId = "";

            if (viewsZIndexCache.length > 0) {
                nId = viewsZIndexCache[viewsZIndexCache.length - 1];
            }
            else {
                nId = 'vis_lhome';

            }

            toggleContainer(nId);//show view
            activateTaskBarItem($shortcutUL.find("LI#" + nId));

            return true;
        };

        /* add item in Taskbar 
        * @parm id id of item
        *@param imgpath path of image
        *a@param name name to diplay
        */
        function addTaskBarItem(id, imgPath, name) {
            var $li = $("<li id=" + id + "><img src= " + imgPath + " /> <a >" + name + "</a></li>");
            $shortcutUL.append($li);
            activateTaskBarItem($li);
            $li = null;
        };

        /* fallback for datalist */
        function polyFills() {

            var datalist, listAttribute, options = [];
            // If the browser doesn't support the list attribute...
            if (!isDataListSupported) {
                // For each text input with a list attribute...
                // $('input[type="text"][list]').each(function () {
                // Find the id of the datalist on the input
                // Using this, find the datalist that corresponds to the input.
                listAttribute = $vis_menuSearch.attr('list');
                datalist = $('#' + listAttribute);
                // If the input has a corresponding datalist element...
                if (datalist.length > 0) {
                    options = [];
                    // Build up the list of options to pass to the autocomplete widget.
                    datalist.find('option').each(function () {
                        options.push({ label: this.value, value: this.value, action: $(this).data("action"), id: $(this).data("actionid") });
                        //<option data-action="W" data-actionid="262" title="360" value="Expense Invoice (Alpha)"></option>
                    });
                    // Transform the input into a jQuery UI autocomplete widget.
                    $vis_menuSearch.autocomplete({
                        source: options,
                        open: function () {
                            window.setTimeout(function () {
                                $('.ui-autocomplete').css('z-index', 1000);

                                //if (!VIS.Application.isMobile)
                                //{
                                var menuHeight = (window.innerHeight * 60) / 100;
                                $('.ui-autocomplete').css('max-height', menuHeight);
                                //}
                            });
                        },
                        select: function (event, ui) {
                            searchSelectItem = ui.item;
                        },
                        close: function (event, ui) {
                            onAutoCompleteClose();
                        }
                    });
                }
                // });
                // Remove all datalists from the DOM so they do not display.
                datalist.remove();
            }
        };

        //Call function 
        adjustHeight(); //set default size 

        bindEvents(); // bind events

        getDaysLeft();

        function openMenu() {
            $vis_appMenu.trigger('click');
        };


        function closeMenu() {
            $vis_closeMenu.trigger('click');
        };


        function navigationInit() {
            $mainNavigationDiv = $('<div class="vis-nav-windowContainer"></div>');
            $section.prepend($mainNavigationDiv);
        };

        function getNavigationSection() {
            return $mainNavigationDiv;
        };


        function getMenuDiv() {
            return $vis_appMenu;
        }

        /* public object */
        var desktopMgr = {
            startMenuAction: startMenuAction,
            //getMenu: getMenu,
            getMainConatiner: getMainConatiner,
            toggleContainer: toggleContainer,
            registerView: registerView,
            unRegisterView: unRegisterView,
            addTaskBarItem: addTaskBarItem,
            start: start,
            activateTaskBarItemUsingID: activateTaskBarItemUsingID,
            openMenu: openMenu,
            closeMenu: closeMenu,
            getNavigationSection: getNavigationSection,
            getMenuDiv: getMenuDiv
        };
        return desktopMgr;
    };

    VIS.desktopMgr = desktopMgr(); // assigned to VIS

    /************************************************************************/

    /* auhtorization Up Dialog
       - show auth dialog containg loginned used auhtorization detail
       - provide functionality, user can change authorized credential
    */
    var authDialog = function () {

        var form, cmbRole, hidRoleName, hidClientName, cmbClient, cmbOrg, hidOrgName, cmbWare, hidWareName, liUserName;
        var root, btnClose, btnChange;
        var orgRoleVal, orgClientHtml, orgOrgHtml, orgWareHtml, wareHouseId;
        var changed = false, setting = false;
        var contextUrl = "";

        var lblRole, lblClient, lblOrg, lblWare;
        var main = $('<div class="vis-wakeup-main" >').hide();

        /* 
         inilialize dialog
         *
         *@param $root
         @param $liuserName
         */
        function init($root, $liUserName) {
            $root.remove(); // remove root from DOM
            liUserName = $liUserName; // event invoker li
            root = $root;

            form = $root.find('#vis_changeAuthForm');
            cmbRole = $root.find('#vis_home_role'); //role
            //hidRoleName = $root.find('#vis_home_rolename');
            cmbClient = $root.find('#vis_home_client'); //client
            //hidClientName = $root.find('#vis_home_clientName');
            cmbOrg = $root.find('#vis_home_org');
            //hidOrgName = $root.find('#vis_home_orgName');
            cmbWare = $root.find('#vis_home_warehouse');
            //hidWareName = $root.find('#vis_home_warehouseName');
            wareHouseId = $root.find('#vis_home_warehouseId');


            btnClose = $root.find('#vis-auth-close'); //cancel
            btnChange = $root.find('#vis-auth-change'); //change

            lblRole = $root.find('label[for="Login2Model_Role"]');
            lblClient = $root.find('label[for="Login2Model_Client"]');
            lblOrg = $root.find('label[for="Login2Model_Org"]');
            lblWare = $root.find('label[for="Login2Model_Warehouse"]');

            orgRoleVal = cmbRole.val();

            orgClientHtml = cmbClient.html();
            orgOrgHtml = cmbOrg.html();
            orgWareHtml = cmbWare.html();

            events();

            $('body').append(main);
            $('body').append(root);
            contextUrl = VIS.Application.contextUrl;
            setText();
            if (wareHouseId.val() == "-1") {
                cmbWare.val(null);
            }
        };

        /*
          set text according to culture
        */
        function setText() {
            lblRole.text(Globalize.localize("Role"));
            lblClient.text(Globalize.localize("Client"));
            lblOrg.text(Globalize.localize("Organization"));
            lblWare.text(Globalize.localize("Warehouse"));
            btnChange.val(VIS.Msg.getMsg("Change"));
            btnClose.val(VIS.Msg.getMsg("Close"));
        };

        /*
        bind events 
        */
        function events() {
            liUserName.on('click', show);
            main.on("click", hide);
            btnClose.on("click", hide);

            cmbRole.on("change", comboChange);
            cmbClient.on("change", comboChange);
            cmbOrg.on("change", comboChange);
            cmbWare.on("change", comboChange);

            form.submit(formSubmitHandler);
        };

        /*
           restore to original value
           - user can change value of combobox but later close the form 
        */
        function restore() {
            cmbRole.val(orgRoleVal);
            cmbClient.html(orgClientHtml);
            cmbOrg.html(orgOrgHtml);
            cmbWare.html(orgWareHtml);
            if (wareHouseId.val() == "-1") {
                cmbWare.val(null);
            }
            btnChange.prop("disabled", true);
            displayErrors(form, "");
            changed = false;
        };

        /* show dialog
        */
        function show() {
            main.show(); // disable background
            root.show('slideDown'); //popup
            btnClose.prop('disabled', false);
            btnChange.prop('disabled', true);
        };
        /* hide dialog
        */
        function hide(e) {
            if (changed)
                restore();
            main.hide();
            root.hide('slideUp');
            e.preventDefault();
            return false;
            //root.detach();
        };

        /* get data and fill combobox
        *@param combo combobox to fill
        *@param url url of controller
        *@param data input data
        */
        function getdata(combo, url, data) {
            //$imgbusy1.show();
            $.ajax(url, {
                data: data
            }).success(function (result) {
                fillCombo(combo, result.data);
            })
            .fail(function (result) {
                alert(result);
            });
        };

        /* handle combobox change event
        */
        function comboChange() {
            if (setting)
                return;

            if (!changed) {
                changed = true;
                btnChange.prop("disabled", false);
            }


            combo = this;
            //$cmbRole.attr('id');
            if (combo.id === cmbRole.attr('id')) {
                //Clear other combo
                cmbClient.empty();
                cmbOrg.empty();
                cmbWare.empty();

                getdata(cmbClient, contextUrl + "Account/GetClients", { 'Id': combo.value });

            }
            else if (combo.id === cmbClient.attr('id')) {
                cmbOrg.empty();
                cmbWare.empty();

                getdata(cmbOrg, contextUrl + "Account/GetOrgs", { 'role': cmbRole.val(), 'user': VIS.context.getAD_User_ID(), 'client': combo.value });
            }
            else if (combo.id === cmbOrg.attr('id')) {
                cmbWare.empty();
                getdata(cmbWare, contextUrl + "Account/GetWarehouse", { 'id': cmbOrg.val() });
            }
            else if (combo.id === cmbWare.attr('id')) {

            }

            var $hidden = $('#' + combo.id + 'Name');
            var text = this.options[this.selectedIndex].innerHTML;

            $hidden.val(text);


        };

        /* function to fill combo
       */
        function fillCombo(combo, data) {
            setting = true;
            combo.empty();

            var text = Globalize.localize('SelectWarehouse');
            if (combo === cmbRole) {
                text = Globalize.localize('SelectRole');
            }
            else if (combo === cmbClient) {
                text = Globalize.localize('SelectClient');
            }
            else if (combo === cmbOrg) {
                text = Globalize.localize('SelectOrg');
            }

            $("<option />", {
                val: "-1",
                text: text
            }).appendTo(combo);


            $(data).each(function () {
                $("<option />", {
                    val: this.Key,
                    text: this.Name
                }).appendTo(combo);
            });
            setting = false;
        };

        /*
           handle form sumbit handler
           - true then reload page 
           - false them show error message
        */
        function formSubmitHandler(e) {
            var $form = $(this);
            displayErrors($form, "");
            btnClose.prop('disabled', true);
            btnChange.prop('disabled', true);
            $form.find('#vis_home_langugage').val(localStorage.getItem("vis_login_langCode"));
            //$imgbusy1.show();
            // We check if jQuery.validator exists on the form
            if (!$form.valid || $form.valid()) {
                $.post($form.attr('action'), $form.serializeArray())
                    .done(function (json) {
                        json = json || {};

                        // In case of success, we redirect to the provided URL or the same page.

                        if (json.success) {
                            window.location = json.redirect || location.href;
                            if (window.location.href.indexOf('#') != -1) {
                                location.reload();
                            }
                            // btnClose.prop('disabled', false);
                            // btnChange.prop('disabled', true);
                        } else if (json.errors) {
                            displayErrors($form, json.errors);
                            btnClose.prop('disabled', false);
                            btnChange.prop('disabled', false);
                            //$imgbusy1.hide();
                        }

                    })
                    .error(function () {
                        displayErrors($form, ['An unknown error happened.']);
                        btnClose.prop('disabled', false);
                        btnChange.prop('disabled', false);
                        //$imgbusy2.hide();
                        //$imgbusy1.hide();
                    });
            }
            // Prevent the normal behavior since we opened the dialog
            e.preventDefault();
        };

        var getValidationSummaryErrors = function ($form) {
            var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
            return errorSummary;
        };

        var displayErrors = function (form, errors) {
            var errorSummary = getValidationSummaryErrors(form)
                .removeClass('validation-summary-valid')
                .addClass('validation-summary-errors');

            var items = $.map(errors, function (error) {
                return '<li>' + Globalize.localize(error) + '</li>';
            }).join('');

            errorSummary.find('ul').empty().append(items);
        };

        return {
            init: init
        };

    }();

    /*********************************************************************/
    /* Menu filter popup
       - show menu filter option and filter menu tree according to selection
    */
    var menuFilterMgr = function () {
        var menuUL = null;
        var menuHtml = "";
        var filteredMenuHtml = "";
        var filterA = null;
        var isVisible = false;

        //var main = $('<div class="vis-wakeup-main" >').hide(); 
        var root = $('<div class="vis-dialog vis-filter-dialog">');

        /* initialize
        @param menuTree  menu tree UI element
        @param filterMenuA filter UI element
        */
        function init(menuTree, filterMenuA) {

            // create UI
            root.html('<input type="radio" name="filter" id="vis_filter_radio_1" value="A" checked style="margin-bottom:5px;margin:1px;" /><label for="vis_filter_radio_1">' + VIS.Msg.getMsg("All") + '</label><br>' +
                      '<input type="radio" name="filter" id="vis_filter_radio_2" value="W" style="margin-bottom:5px;margin:1px;" /><label for="vis_filter_radio_2">' + VIS.Msg.getMsg("Window") + '</label><br>' +
                      '<input type="radio" name="filter" id="vis_filter_radio_3" value="X" style="margin-bottom:5px;margin:1px;"/><label for="vis_filter_radio_3">' + VIS.Msg.getMsg("Form") + '</label><br>' +
                      '<input type="radio" name="filter" id="vis_filter_radio_4" value="P" style="margin-bottom:5px;margin:1px;"/><label for="vis_filter_radio_4">' + VIS.Msg.getMsg("Process") + '</label><br>' +
                      '<input type="radio" name="filter" id="vis_filter_radio_5" value="R" style="margin-bottom:20px;margin:1px;"/><label for="vis_filter_radio_5">' + VIS.Msg.getMsg("Report") + '</label><br/>' +
                      '<input type="button" name="filter" style="background-color: #616364;color:white" value=' + VIS.Msg.getMsg("Filter") + '></input>');

            menuUL = menuTree.find(">ul"); //menu UL element
            var options = [], itm = null;
            menuUL.find('li').each(function () {
                itm = $(this);
                if (itm.data("summary") == "N") {
                    options.push(itm[0].outerHTML);
                }
            });

            menuHtml = menuUL.html(); //all html tree string
            filteredMenuHtml = options.join(''); // all leaf nodes
            options.length = 0;
            options = null;
            filterA = filterMenuA; // event invoker
            $('body').append(root); // append div to body
            events(); // bind events
        };

        /*
        bind events to ui elements
        */
        function events() {
            filterA.on("click", toggle);
            root.on("click", "input[type=button]", function (e) {
                e.stopPropagation();
                var rd = root.find("input[type=radio]:checked").val();
                filterMenu(rd); // menu filter
                closePopup(); // close popup
            });
        };

        /* filter the menu accroding to action
        @param action type of menu item
        */
        function filterMenu(action) {
            if (action === "A") { // all tree
                menuUL.empty();
                menuUL.html(menuHtml);
            }
            else {
                menuUL.empty();
                menuUL.html(filteredMenuHtml); // all leaf nodes
                menuUL.find('li').hide(); // hide all

                menuUL.find('li > a[data-action="' + action + '"]').parent().show(); // show only match action
            }
        };

        /* show hide popup
        */
        function toggle() {
            isVisible = !isVisible;
            isVisible ? root.show('slide-down') : root.hide('slide-up');
        };

        /* close popup
        */
        function closePopup() {
            if (isVisible) {
                toggle();
            }
        };

        // return object public function
        return {
            init: init,
            closePopup: closePopup
        }
    }();


    /***************************************************************/
    /*
        wake Up Dialog
       - show dialog when ajax request session or authorize time out 
       - refresh url to re build tokens
    */
    var wakeupDialog = function () {
        // ui root
        var root = $('<div class="vis-wakeup-main"></div>'
                    + '<div class="vis-wakeup"></div>'
                    + '<div class="vis-wakeup-content">'
                    + '<div style="width: 40%; float: left; height: 100%">'
                    + '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/smiley-sleep.gif" style="padding: 10px;padding-top:15px;" class="vis-wakeup-sleep" />'
                    + '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/smiley-awake.gif" style="padding: 10px;display:none;padding-top:15px;" class="vis-wakeup-awake" />'
                    + '</div><div style="width: 60%; height: 100%; float: right;text-align:right">'
                    + '<div style="height: 70%;margin: 10px; overflow: auto;" class="vis-wakeup-text">'
                    //+ VIS.Msg.getMsg('WakeupText')
                    + '</div><button style="margin-right:5px;">WakeUp </button></div></div></div>');

        var btn = root.find('button');
        var imgS = root.find('.vis-wakeup-sleep');
        var imgA = root.find('.vis-wakeup-awake');
        var txt = root.find(".vis-wakeup-text");

        /* initilize 
         @param tx text to display  
        */
        function init(tx) {
            txt.text(tx);
            $('body').append(root);
        };

        btn.on('click', function () {
            btn.prop("disabled", true);
            imgS.hide();
            imgA.show();
            window.location.reload(); //refresh url
        });

        var d = {
            init: init
        };
        return d;
    }();

    /*
       global ajax error handler 
      - hanlde authorize and session time error
      - show wakeup dialog  
      */

    var redirect = false;
    $(document).ajaxError(function (xhr, props) {
        if (props.status === 401) {
            if (!redirect) {
                wakeupDialog.init(VIS.Msg.getMsg('UnAuthorized')); // show wakeup dialog
                redirect = true;
            }
        }

        else if (props.status === 999) {
            if (!redirect) {
                wakeupDialog.init(VIS.Msg.getMsg('WakeupText')); // show wakeup dialog
                redirect = true;
            }
        }
        else {
            console.log(props.responseText);
        }
    });




})(VIS);







