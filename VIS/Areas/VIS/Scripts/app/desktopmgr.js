
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

            $($menuTree.find('.vis-navLeftWrap a')[0]).trigger("click");

            function menuItemClick(event) {
                var $target = $(event.target);
                if ($target.hasClass('fa fa-arrow-left')) {
                    $target = $target.parent();
                }
                var $par = $target.parent();


                // If modile menu is opened and user clicks on back button
                if ($target.hasClass('vis-menuitm-backbtn')) {
                    $target.closest('ul').css('display', 'none');;
                    $vis_mainMenu.find('.vismenu-parent').show();
                    $menuTree.find('.vismenu-selectedMbTab').removeClass('vismenu-selectedMbTab');

                    //Show All items of main menu
                    $menuTree.find('[data-val="' + $par.data('ulid') + '"]').siblings().show()
                    $menuTree.find('[data-val="' + $par.data('ulid') + '"]').show();

                    //hide all submenus
                    $menuTree.find('ul:not(.vismenu-parent)').hide();
                    $menuTree.find('li:not(.vis-hasSubMenu)').hide();
                    if (menuFilterMgr)
                        menuFilterMgr.setIsMenuHeaderClicked(false);
                }
                // If user clicks on any item in mobile menu
                else if (event.target.nodeName === "LABEL" && $par.data("con") == "Y") {
                    var pID = $par.data("val");
                    var currentFilter;
                    if (!menuFilterMgr)
                        return;
                    // if any filter is applied, then on navigation on main menu, that filter must be applied.
                    currentFilter = menuFilterMgr.getFilterValue();
                    menuFilterMgr.setIsMenuHeaderClicked(false);
                    var selectedTab = $menuTree.find('[data-ulid="' + pID + '"]');
                    selectedTab.css('display', 'flex').addClass('vismenu-selectedMbTab');
                    selectedTab.find('ul').css('display', 'block');
                    selectedTab.find('li').hide();
                    selectedTab.find('.vis-subNavFirstElement').show();
                    selectedTab.find('.vis-menu-innerFolders').show();

                    selectedTab.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
                    if (currentFilter != "A")
                        selectedTab.find('li > a[data-action="' + currentFilter + '"]').parent().css('display', 'flex');
                    else
                        selectedTab.find('li:not(.vis-menusubItem)').css('display', 'flex');

                    $vis_mainMenu.find('.vismenu-parent').hide();
                    menuFilterMgr.hideMobileEmptyFolder();

                    if ($par.data("summary") == 'Y') {
                        $par.siblings().hide()
                        $par.hide();
                    }
                    else {
                        $par.parent().siblings().hide();
                        $par.hide();
                    }
                }
                //For Web Menu
                else {

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
                        //If user clicks summary Item, thenmark it as selected item in main menu and show its child along with
                        if ($target.data('summary') == "Y") {
                            var currentFilter;
                            // if any filter is applied, then on navigation on main menu, that filter must be applied.

                            if (!menuFilterMgr) {
                                currentFilter = null;
                            }
                            else {
                                currentFilter = menuFilterMgr.getFilterValue();
                            }

                            $vis_mainMenu.find('.vis-navmenuItems-Container').hide();
                            $target.parent().siblings().removeClass("vis-navSelected");
                            $target.parent().addClass("vis-navSelected");
                            var items = $vis_mainMenu.find('#Menu' + $target.data('value'));
                            items.removeAttr('style');
                            items.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');

                            $menuTree.find('.vismenu-hidden-header').hide();
                            $menuTree.find('.vis-nav-AllItems').hide();
                            $menuTree.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
                            if (currentFilter != null) {
                                if (currentFilter != "A") {
                                    $menuTree.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');
                                }
                                else {
                                    $menuTree.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
                                }
                            }
                            items.find('.vismenu-subSummaryNode').css('display', 'none');

                            if (menuFilterMgr)
                                menuFilterMgr.hideEmptyFolders();

                            return;
                        }
                        startMenuAction($target.data('action'), $target.data('actionid')); //start action
                        return;
                    }
                    // For Sub menus in Web.
                    if (event.target.nodeName === "I") {
                        var $target = $(event.target);
                        if ($target.hasClass('vis-navAccordian')) {
                            //If user clicks on plus sign, then show all items of submenu
                            if ($target.hasClass('fa-plus')) {
                                $target.removeClass('fa-plus').addClass('fa-minus');
                                var id = $target.data('pid');
                                $menuTree.find('[data-ulid="' + id + '"]').show();
                            }
                            //If user clicks on plus sign, then hide all items of submenu
                            else {
                                $target.removeClass('fa-minus').addClass('fa-plus');
                                var id = $target.data('pid');
                                $menuTree.find('[data-ulid="' + id + '"]').hide();
                            }
                            return;
                        }


                        var $target = $target.parent();
                        if ($target.data('isfavbtn') == 'yes') {
                            VIS.FavouriteHelper.showOverlay($target); // show menu item's options
                            return;
                        }
                    }
                }
            };

            // task bar click event
            $shortcutUL.on(VIS.Events.onClick, "LI", function (event) {
                event.preventDefault();
                //if (VIS.context.getContext("#DisableMenu") == 'Y') {
                //    return;
                //}

                if (event.target.nodeName === "I" || event.target.nodeName === "SPAN") {
                    //close
                    closeFrame($(event.currentTarget));
                    return;
                }


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

                        $($target.siblings('label')).css('border', '1px solid rgba(var(--v-c-primary), 1)');
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
            $vis_mainMenu.attr('style', 'display: none !important');
            $menuOverlay.fadeOut();
            $vis_menuSearch.val("");
            $vis_menuSearch.blur();
        };

        /* show app menu */
        function showMenu() {
            $menuOverlay.fadeIn();
            //if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
            $vis_mainMenu.attr('style', 'display: block !important')
            //}
            //else {
            //    $vis_mainMenu.attr('style', 'display: flex !important')
            //}
        };

        function toggleMenu() {
            $menudiv.fadeToggle();
            $menuOverlay.fadeToggle();
        };

        /*set height of section (main container) to window size */
        function adjustHeight() {
            var height = 0;
            if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
                height = document.documentElement.clientHeight;
            }
            else {
                height = window.innerHeight;
            }
            ////$section.css('height', height - 22);
            height = height - 23;
            if (VIS.Utility.Util.getValueOfInt(VIS.context.ctx["#FRAMEWORK_VERSION"]) < 2)
                height = height - 43;

            VIS.Env.setScreenHeight(height);
            document.documentElement.style.setProperty('--vis-screen-height', (height * 0.01) + 'px');
            if (VIS.viewManager)
                VIS.viewManager.sizeChanged(height, window.innerwidth);
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
            if (itm.length > 0) {
                if (itm[0].id == "vis_lhome")
                    return;
                //select unselect taskbar items
                if (curSelTaskBarItem) {
                    //curSelTaskBarItem.css('background-color', '');
                    curSelTaskBarItem.removeClass('vis-app-f-selected');
                }
                //curSelTaskBarItem = itm.css('background-color', '#D7E3E7');
                curSelTaskBarItem = itm.addClass('vis-app-f-selected');
                itm = null;
            }
        };

        function activateTaskBarItemUsingID(item) {
            var nId = item.data('wid');
            activateTaskBarItem($shortcutUL.find("LI#" + nId));
        };

        /*
           close active view
           and get container by passed name or id to set as current active view
         *
         * @param  name name or id of view
         */
        function closeFrame(ele) {
            if (viewsZIndexCache[viewsZIndexCache.length - 1] == ele[0].id)
                VIS.viewManager.closeFrame(ele[0].id);
            else {
                toggleContainer(ele[0].id);
                activateTaskBarItem(ele);
            }
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
            for (var i = 0; i < list.options.length; i++) {
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
            menuFilterMgr.init($menuTree, $('#vis_filterMenu'), $vis_mainMenu); //init Menu filter popup Manager
            authDialog.init($('#vis_home_ca'), $('#vis_userName')); //Init Authorization Dialog
            setAndLoadHomePage(); //Home Page
            historyMgr.restoreHistory(); //Restore History of App if any
            navigationInit();
            window.addEventListener("DOMContentLoaded", VIS.sseManager.start, false);
        }

        /*
           add home div in view cache 
           - init home Manager js 
           - set active view to home page
        */
        function setAndLoadHomePage() {
           // if (!VIS.MRole.getIsDisableMenu() || (VIS.MRole.getIsDisableMenu() && VIS.MRole.getHomePage() == 0)) {
            if (!VIS.MRole.getIsDisableMenu()) {
                renderHomePage();
            }
            if (VIS.MRole.getIsDisableMenu()) {
                $('.vis-topMenu').hide();
                $('.vis-menuHeaderLogo').show();
            }
            var homePageID = VIS.MRole.getHomePage();
            if (homePageID && parseInt(homePageID) > 0) {
                startMenuAction("X", homePageID);
            }
        };

        function renderHomePage() {
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
            var img = ['<li id=' + id + '>'];
            if (imgPath) {
                if (imgPath.indexOf(".") > -1)
                    img.push('<img src= "' + imgPath + '" />');
                else
                    img.push('<i class= "' + imgPath + '" />');
            }

            img.push('<a>' + name + '</a><span style="padding:0 7px;"><i class="fa fa-times-circle-o" /></span></li>');
            var $li = $(img.join(' '));
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
        };

        function getNavigationSection() {
            if (!$mainNavigationDiv)
                $mainNavigationDiv = $(".vis-app-action-nav");
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
            getNavSection: getNavigationSection,
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
            displayErrors(form, []);
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
            displayErrors($form, []);
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
        var selectedMenu = '';
        var _menuTree = null;
        var newContainer = null;
        var isheaderClicked = false;
        var _mainMenu = null;

        //var main = $('<div class="vis-wakeup-main" >').hide(); 
        var root = $('<div class="vis-dialog vis-filter-dialog">');

        /* initialize
        @param menuTree  menu tree UI element
        @param filterMenuA filter UI element
        */
        function init(menuTree, filterMenuA, mainMenu) {

            // create UI
            root.html('<input type="radio" name="filter" id="vis_filter_radio_1" value="A" checked style="margin-bottom:5px;margin:1px;" /><label for="vis_filter_radio_1">' + VIS.Msg.getMsg("All") + '</label><br>' +
                '<input type="radio" name="filter" id="vis_filter_radio_2" value="W" style="margin-bottom:5px;margin:1px;" /><label for="vis_filter_radio_2">' + VIS.Msg.getMsg("Window") + '</label><br>' +
                '<input type="radio" name="filter" id="vis_filter_radio_3" value="X" style="margin-bottom:5px;margin:1px;"/><label for="vis_filter_radio_3">' + VIS.Msg.getMsg("Form") + '</label><br>' +
                '<input type="radio" name="filter" id="vis_filter_radio_4" value="P" style="margin-bottom:5px;margin:1px;"/><label for="vis_filter_radio_4">' + VIS.Msg.getMsg("Process") + '</label><br>' +
                '<input type="radio" name="filter" id="vis_filter_radio_5" value="R" style="margin-bottom:20px;margin:1px;"/><label for="vis_filter_radio_5">' + VIS.Msg.getMsg("Report") + '</label><br/>' +
                '<input type="button" name="filter" value=' + VIS.Msg.getMsg("Filter") + '></input>');

            _mainMenu = mainMenu;
            _menuTree = menuTree;
            var options = [], itm = null;
            options = getMenuList();
            menuHtml = menuUL.html(); //all html tree string
            filteredMenuHtml = options; // all leaf nodes
            filterA = filterMenuA; // event invoker
            $('body').append(root); // append div to body
            createFilterAllMenu();
            events(); // bind events
        };


        /**
         * Create a list of all items to be displayed in Show all contents
         * @param {any} action
         */
        function getMenuList(action) {
            var options = [];
            menuUL = _menuTree.find(".vis-navMainContent"); //menu UL element
            var menuUL1 = _menuTree.find(".vismenu-hidden-header"); //menu UL element
            for (var i = 0; i < menuUL1.length; i++) {
                itm = $(menuUL1[i]);
                options.push(itm[0].outerHTML);
            };
            return options;
        };

        /*
        bind events to ui elements
        */
        function events() {
            filterA.on("click", toggle);
            root.on("click", "input[type=button]", function (e) {
                e.stopPropagation();
                filterClicked();
            });
            _mainMenu.find('.vis-removeFilter').on("click", function () {
                root.find('#vis_filter_radio_1').prop("checked", true);
                filterClicked();
            });

            /**
             *  When user clicks on menu name, show all items in menu
             * 
             **/
            _mainMenu.find('.vis-menu-headerName').click(function () {
                setIsMenuHeaderClicked(true);
                showAllItems();
            });

            function filterClicked() {
                var rd = root.find("input[type=radio]:checked").val();
                filterSelectedMenu(rd);
                if (rd == "A") {
                    _mainMenu.find('.vis-removeFilter').hide();
                    _menuTree.find('.vismenu-subSummaryNode').css('display', 'none');
                    if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
                        if (!isheaderClicked) {
                            _menuTree.find('.vis-menusubItem').css('display', 'none');
                            _menuTree.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
                        }
                        else {
                            _menuTree.find('.vis-menusubItem').css('display', '');
                            _menuTree.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');
                        }

                    }
                    else {
                        _menuTree.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
                    }
                }
                else {
                    _mainMenu.find('.vis-removeFilter').show();
                    _menuTree.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');

                }
                closePopup(); // close popup
            };
        };

        function filterSelectedMenu(action) {
            if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
                if (_menuTree.find('.vismenu-parent').is(':visible'))
                    return;

                //if using mobile device, then filter is All Items, show all otherwise based on filter
                _menuTree.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');

                if (action === "A") { // all tree
                    _menuTree.find('li').show();
                }
                else {
                    _menuTree.find('li').hide();
                    _menuTree.find('.vis-menu-innerFolders').show();
                    _menuTree.find('.vis-menuitm-backbtn').parent().show();
                    _menuTree.find('li > a[data-action="' + action + '"]').parent().show();
                }

                hideMobileEmptyFolder();

            }
            else {
                _menuTree.find('.vis-navSubMenu').removeClass('vis-menuSum-hide');

                // If all items are visible
                //if (newContainer.is(':visible')) {
                //    newContainer.find('.vis-navMainContent').removeAttr('style');
                //    if (action === "A") { // all tree
                //        _menuTree.find('.vis-navMainContent li').show();///Show All
                //    }
                //    else {
                //        _menuTree.find('.vis-navMainContent li').hide(); // hide all
                //        _menuTree.find('.vis-navMainContent li > a[data-action="' + action + '"]').parent().show(); // show only match action
                //    }
                //}
                //else {
                //    if (action === "A") { // all tree
                //        _menuTree.find('.vis-navMainContent li').show();///Show All
                //    }
                //    else {
                //        _menuTree.find('.vis-navMainContent li').hide(); // hide all
                //        _menuTree.find('.vis-navMainContent li > a[data-action="' + action + '"]').parent().show(); // show only match action
                //    }
                //}

                //Show or hide items based on filters applied
                if (newContainer.is(':visible')) {
                    newContainer.find('.vis-navMainContent').removeAttr('style');
                }
                if (action === "A") { // all tree
                    _menuTree.find('.vis-navMainContent li').show();///Show All
                }
                else {
                    _menuTree.find('.vis-navMainContent li').hide(); // hide all
                    _menuTree.find('.vis-navMainContent li > a[data-action="' + action + '"]').parent().show(); // show only match action
                }


                hideEmptyFolders();
            }
        }

        /**
         * In case of mobiles, when user apply filter then hide all folders with no visible child
         * @param {any} allItems
         */
        function hideMobileEmptyFolder(allItems) {
            if (root.find("input[type=radio]:checked").val() != "A") {
                if (_menuTree.find('.vismenu-parent').is(':visible'))
                    return;
                var selectedTab = _menuTree.find('.vismenu-selectedMbTab');
                var allUls = [];
                if (selectedTab && selectedTab.length > 0) {
                    allUls = selectedTab.find('ul');
                    selectedTab.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');
                }

                else {
                    allUls = _menuTree.find('ul');
                    _menuTree.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');
                }

                //Hide all Emplty Subfolders
                for (var j = 0; j < allUls.length; j++) {
                    var thisUL = $(allUls[j]);
                    if (thisUL.hasClass('vismenu-parent'))
                        continue;

                    var liList = thisUL.find('li');
                    var canhide = false;
                    for (var k = 0; k < liList.length; k++) {
                        if ($(liList[k]).is(':visible') && !$(liList[k]).hasClass('vis-menu-innerFolders')) {
                            canhide = false;
                            break;
                        }
                        canhide = true;
                    }
                    if (canhide) {
                        thisUL.addClass('vis-menuSum-hide');
                        thisUL.css('display', '');
                        thisUL.parent().css('display', '');
                    }
                }

                if (selectedTab && selectedTab.length > 0 && !isheaderClicked) {
                    allUls = selectedTab.find('.vis-menuitm-backbtn');
                }
                else {
                    allUls = _menuTree.find('.vis-menuitm-backbtn');
                    selectedTab = null;
                }
                //Hide all Main folder having no visible child after appling filter
                for (var j = 0; j < allUls.length; j++) {
                    var thisUL = $(allUls[j]).parent();;

                    var liList = thisUL.siblings();
                    var canhide = false;
                    for (var k = 0; k < liList.length; k++) {
                        if ($(liList[k]).is(':visible')) {
                            canhide = false;
                            break;
                        }
                        canhide = true;
                    }

                    if (canhide) {
                        if (selectedTab == null || selectedTab.length == 0 || isheaderClicked) {
                            thisUL.css('display', '');
                            thisUL.addClass('vis-menuSum-hide');
                        }
                    }

                }

            }

        }

        /**
         * In case of  Web Menu, when user apply filter then hide all folders with no visible child
         */
        function hideEmptyFolders() {
            if (root.find("input[type=radio]:checked").val() != "A") {
                var allUls = menuUL.find('.vis-navmenuItems-Container').find('ul');
                menuUL.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');
                menuUL.find('.vismenu-subSummaryNode').css('display', '');
                //Hide all empty subfolders
                for (var j = 0; j < allUls.length; j++) {
                    var thisUL = $(allUls[j]);
                    var liList = thisUL.find('li');
                    var canhide = false;
                    for (var k = 0; k < liList.length; k++) {
                        if ($(liList[k]).is(':visible')) {
                            canhide = false;
                            break;
                        }
                        canhide = true;
                    }
                    if (canhide)
                        thisUL.closest('.vis-navSubMenu').addClass('vis-menuSum-hide');
                }

                //Hide all empty parent Folders
                allUls = menuUL.find('.vis-navmenuItems-Container-allItems');
                for (var j = 0; j < allUls.length; j++) {
                    var canhide = false;
                    var liList = $(allUls[j]).find('.vis-navSubMenu');
                    if (liList.length > 0) {
                        for (var k = 0; k < liList.length; k++) {
                            if (!$(liList[k]).hasClass('vis-menuSum-hide')) {
                                canhide = false;
                                break;
                            }
                            canhide = true;
                        }
                        //if (canhide)
                        //    $(allUls[j]).addClass('vis-menuSum-hide');
                    }
                    //else {
                    var liList = $(allUls[j]).find('li');
                    for (var k = 0; k < liList.length; k++) {
                        if ($(liList[k]).is(':visible')) {
                            canhide = false;
                            break;
                        }
                        canhide = true;
                    }
                    if (canhide)
                        $(allUls[j]).addClass('vis-menuSum-hide');
                    //}
                }
            }
            else {
                menuUL.find('.vis-menuSum-hide').removeClass('vis-menuSum-hide');
            }
        }

        /**
         * Create UI to display all items together in case of web Menu
         * */
        function createFilterAllMenu() {
            if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
                return;
            }
            else {
                // In case of web menu,when showing all items, create copy of all item containers and add them in new parent.
                //This is done because in case of displaying all items, design is not same as with Single Item.
                var container = _menuTree.find('.vis-navmenuItems-Container');
                newContainer = $("<div class='vis-nav-AllItems'></div>");
                for (var i = 0; i < container.length; i++) {
                    newContainer.append($(container[i]).clone().removeAttr('style').addClass('vis-navmenuItems-Container-allItems'));
                }
                newContainer.hide();
                _menuTree.find('.vis-navMainContent').append(newContainer);
            }
        }

        /**
         * When User clicks on menu name, all items of menu will be visible.
         * */
        function showAllItems() {
            if ((VIS.Application.isMobile || VIS.Application.isIOS) && document.documentElement) {
                var filter = getFilterValue();
                _menuTree.find('li').hide();
                _menuTree.find('.vis-subNavFirstElement').show();
                _menuTree.find('.vis-menu-innerFolders').show();
                _menuTree.find('ul').show();
                if (filter == "A")
                    _menuTree.find('li').show();
                else
                    _menuTree.find('li > a[data-action="' + filter + '"]').parent().show();
                _menuTree.find('.vis-subNavFirstElement').css({ "position": "relative", "top": "0px" });
                _menuTree.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');
                _menuTree.find('.vismenu-parent').hide();
                hideMobileEmptyFolder(true);
            }
            else {
                _menuTree.find('.vis-navmenuItems-Container').hide();
                newContainer.find('.vis-navmenuItems-Container').removeAttr('style');
                newContainer.find('.vismenu-hidden-header').show();

                _menuTree.find(".vis-navSelected").removeClass('vis-navSelected');
                newContainer.show();
                _menuTree.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');
                newContainer.find('.vismenu-subSummaryNode').css('display', '');
                hideEmptyFolders();

            }
        };

        /* show hide popup
        */
        function toggle() {
            isVisible = !isVisible;
            var left = Math.round(_mainMenu.find('.vis-navFilterIco').offset().left) - 84;

            if (VIS.Application.isRTL) {
                root.css('right', 'auto');
            }
            root.css('left', left);
            isVisible ? root.show('slide-down') : root.hide('slide-up');
        };

        /**
         * return Applied filter(window Or Form or process or reports or all)
         * */
        function getFilterValue() {
            return root.find("input[type=radio]:checked").val();
        }

        function setIsMenuHeaderClicked(isheader) {
            isheaderClicked = isheader;
        }

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
            closePopup: closePopup,
            hideEmptyFolders: hideEmptyFolders,
            getFilterValue: getFilterValue,
            hideMobileEmptyFolder: hideMobileEmptyFolder,
            setIsMenuHeaderClicked: setIsMenuHeaderClicked
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
            + '<div class="vis-wakeup-outerwrap">'
            + '<div class="vis-wakeup-content">'
            + '<div class="vis-wakeup-headsec">'
            + '</div>'
            + '<div class="vis-wakeup-datawrp">'
            + '<strong>Oh no! Something went wrong</strong>'
            + '<div class="vis-wakeup-text">'
            //+ VIS.Msg.getMsg('WakeupText')
            + '</div><button>Refresh</button></div></div></div></div>');

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
            //imgS.hide();
            //imgA.show();
            window.location.reload(); //refresh url
        });

        var d = {
            init: init
        };
        return d;
    }();


    /**
    * Start Server sent events
    */
    //var startToastr = function () {
    //    var source = new EventSource('JsonData/MsgForToastr?varificationToken=' + $("#vis_antiForgeryToken").val());
    //    source.onmessage = function (e) {
    //        var returnedItem = JSON.parse(e.data);
    //        if (returnedItem.message && returnedItem.message.length > 0) {
    //            toastr.success(returnedItem.message, '', { timeOut: 4000, "positionClass": "toast-top-center", "closeButton": true, });
    //        }
    //    };
    //};



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







