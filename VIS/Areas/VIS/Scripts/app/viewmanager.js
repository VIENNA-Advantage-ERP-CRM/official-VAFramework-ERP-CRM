; (function (VIS) { //scope
    var dm = VIS.desktopMgr; // shorthand for desktopMge object
    var $mainConatiner = dm.getMainConatiner(); // main conatiner 

    function viewManager() {

        var ACTION_WORKBENCH = "B";
        /** WorkFlow = F */
        var ACTION_WORKFLOW = "F";
        /** Process = P */
        var ACTION_PROCESS = "P";
        /** Report = R */
        var ACTION_REPORT = "R";
        /** Task = T */
        var ACTION_TASK = "T";
        /** Window = W */
        var ACTION_WINDOW = "W";
        /** Form = X */
        var ACTION_FORM = "X";
        var SEPRATOR = "!"; // history action seprator
        var historyActions = []; // conatins history string array
        var windowObjects = {}; // store all window(javascrpt)  references
        var s_hiddenWindows = []; /** list of hidden Windows				*/
        var navigatingInWindows = false; // set its value true when navigaing through windows , otherwise false
       // var $mainNavigationDiv = null; //Navigation Div Container

        var $navRoot = null;
        var $innerDiv = null;
        var $menu = null;
        var restoreAction = false;

        var restoredWindowCount = 0;

        var currentFrame = null;

        init();



        var mainPage = {
            startAction: startAction,
            restoreActions: restoreActions,
            resize: refresh,
            sizeChanged: sizeChanged,
            startWindow: startWindow,
            startForm: startForm,
            startCFrame: startCFrame,
            startActionInNewTab: startActionInNewTab,
            closeFrame:closeFrame
        };

        return mainPage; // return mainpage object and public functions

    /*
     * close the window
     * @param id id of item
     */
 
        function closeFrame(id) {
            return windowObjects[id].dispose(); 
        }


        /*
         * start menu Item
         * @param action type of item
         * @param id id of item
         */
        function startAction(action, id) {

            if (ACTION_WINDOW === action) {
                startWindow(id);
            }
            else if (ACTION_FORM === action) {
                startForm(id);
                //
            }
            else if (ACTION_PROCESS === action || ACTION_REPORT === action) {
                startProcess(id, action);
            }
        };

        function addShortcut(id, imgName, name, hid) {
            if (restoreAction) {
                addRestoredShortcut();
            }
            else {
                var imgPath = "fa fa-window-maximize";
                if (imgName) {
                    imgPath = imgName;
                }

                dm.addTaskBarItem(id, imgPath, name);

                // In case of default Home page, Do not add home page in URL. 
                if (hid && id.slice(id.indexOf('_') + 1) != VIS.MRole.getHomePage()) {
                    historyActions.push(hid);
                    historyMgr.updateHistory(encode(historyActions.join(SEPRATOR)));
                }
            }

        };

        function addRestoredShortcut() {
            restoredWindowCount++;
            if (restoredWindowCount == Object.keys(windowObjects).length) {
                restoreAction = false;
                for (var item in windowObjects) {
                    addShortcut(windowObjects[item].id, windowObjects[item].img, windowObjects[item].name, windowObjects[item].hid);
                }

            }
        };

        function encode(action) {
            action = VIS.Utility.encode(action)
            return action;
        }


        /* Remove ShortCut icon form list
         * @param id parameter id of LI element to remove
         * @param $panel jquery window object to remove
         */

        function removeShortcut(id, $panel, hid, AD_Window_ID) {

            dm.unRegisterView(id); //remove from desktop
            if (hid) {
                var index = historyActions.indexOf(hid);
                if (index > -1) {
                    historyActions.splice(index, 1);
                }
                historyMgr.updateHistory(encode(historyActions.join(SEPRATOR)));
            }
            var windw = windowObjects[id];
            windowObjects[id] = null;
            delete windowObjects[id];
            currentFrame = null;
            if (AD_Window_ID && hideWindow(windw))
                return false;
            return true;
        };

        /* Start Window
         *@param id id of window
         *@param qry Query Object
         */
        function startWindow(id, qry) {

            var sel = null;
            if (id.toString().indexOf("&") != -1) {
                sel = id.split("&")[1];
                id = id.split("&")[0];
            }

            var windw = showWindow(id); //form cache
            if (windw) {
                windw.refreshData();
                windw.show($mainConatiner, addShortcut);
                registerView(windw);
                setGridFont();
                return windw;
            }

            windw = new VIS.AWindow();

            if (windw.initWindow(id, qry, addShortcut, ACTION_WINDOW, sel)) {
                windw.onClosed = removeShortcut;
                windw.show($mainConatiner);
                registerView(windw);
                setGridFont();
            }

            return windw;
        };

        /* Check browser zoom level and based on that set W2UI grid font for window */
        function setGridFont() {
           
            var dynamicClassName = ".w2ui-reset table{font-family: var(--v-c-font-family);";
            if (Math.round(window.devicePixelRatio * 100) >= 100) {

                dynamicClassName += "font-size: 0.85rem !important;}";
            }
            else {
                dynamicClassName += "font-size: 1rem !important;}";
            }
            //
            if (this.styleTag) {
                this.styleTag.remove();
                $('.vis-gc-vtable').removeClass('w2ui-reset');
                $('.vis-gc-vtable').addClass('w2ui-reset');
            }
            else {
                $('.vis-gc-vtable').addClass('w2ui-reset');
            }
            this.styleTag = document.createElement('style');
            this.styleTag.type = 'text/css'
            this.styleTag.innerHTML = dynamicClassName;
            $($('head')[0]).append(this.styleTag);
        };

        /* Start form
        *@param id id of form
        */
        function startForm(id, additionalInfo) {           
            if (id.toString().indexOf("&") != -1) {
                additionalInfo = id.split("&")[1];
                id = id.split("&")[0];
            }
            var windw = new VIS.AWindow();
            if (VIS.MRole.getIsDisableMenu())
            {
                windw.hideCloseIcon(true);
            }
            if (windw.initForm(id, addShortcut, ACTION_FORM, additionalInfo)) {
                windw.onClosed = removeShortcut;
                windw.show($mainConatiner);
                registerView(windw);
            }
        };

        /* Start process
        *@param id id of process
        */
        function startProcess(id, action) {
            var windw = new VIS.AWindow();
            //Uncomment this code to run report in split mode.
              if (windw.initProcess(id, addShortcut, action,true)) {
          //  if (windw.initProcess(id, addShortcut, action)) {
                windw.onClosed = removeShortcut;
                windw.show($mainConatiner);
                registerView(windw);
            }
        };

        function startCFrame(windw) {
            windw.onClosed = removeShortcut;
            windw.show($mainConatiner);
            addShortcut(windw.getId(), "fa fa-list-alt", windw.getName(), null);
            registerView(windw);
        };

        /*
          cache window object 
          and send window Ui root element to desktopmanager
          @param window object
        */
        function registerView(windw) {
            dm.registerView(windw.getId(), windw.getRootLayout());
            windowObjects[windw.getId()] = windw;
            //restoredWindowObjects[windw.getId()] = null;
        };

        /*
           restore history actions 
           @param actionstr url string
        */
        function restoreActions(actionStr) {



            try {
                historyMgr.updateHistory("#");
                var sel = [];
                if (actionStr.indexOf("&") != -1) {
                    actionStr = actionStr.split("&");
                    sel = actionStr[1].split(',');
                    actionStr = actionStr[0];
                }

                actionStr = VIS.Utility.decode(actionStr); // decrypt url
                if (actionStr.length > 0) {
                    restoreAction = true;
                    var actions = actionStr.split(SEPRATOR); // seprate actions
                    for (var i = 0, len = actions.length; i < len; i++) {
                        if (actions[i].length > 0) {
                            var actionId = actions[i];
                            var action = actionId.substring(0, 1); // either W  or P or X etc...
                            var id = actionId.substring(2); // id of action
                            if (ACTION_FORM === action) {
                                if (!VIS.MRole.getFormAccess(id)) {
                                    console.log("No Form Access");
                                    if (sel && sel.length > 0) {
                                        sel.shift();
                                    }
                                    continue;
                                }
                                if (sel && sel.length > 0) {
                                    id += "&" + sel.shift();
                                }
                            }
                            else if (ACTION_PROCESS === action || ACTION_REPORT === action) {
                                if (!VIS.MRole.getProcessAccess(id)) {
                                    console.log("No Process Access");
                                    continue;
                                }
                            }
                            else if (ACTION_WINDOW === action) {
                                if (!VIS.MRole.getWindowAccess(id)) {
                                    console.log("No Window Access");
                                    if (sel && sel.length > 0) {
                                        sel.shift();
                                    }
                                    continue;
                                }
                                if (sel && sel.length > 0) {
                                    id += "&" + sel.shift();
                                }
                            }
                            startAction(action, id); //start menu action
                        }
                    }
                }
            }
            catch (e) {
                historyMgr.updateHistory("#"); //update url
                console.log(e);
            }
        };

        /* 
        - grid need to be resize when window get focus
        @param id unique name of window object
        */
        function refresh(id) {
            if (windowObjects[id])
                currentFrame = windowObjects[id].refresh();
        };

        /* resize all open windows when screen size is changed
        */
        function sizeChanged(h, w) {
            setGridFont();
            for (var id in windowObjects) {
                windowObjects[id].sizeChanged(h,w);
            }
            //alert("resize");
        };

        /*
          open action(W,P,X) in new tab of browser
          @param action type of menu item
          @param id of action
        */
        function startActionInNewTab(action, id) {
            if (action && id) {
                var qs = action + "=" + id;
                qs = encode(qs);
                window.open(VIS.Application.contextUrl + "#" + qs);
                return;
            }
            alert("improper data");
        };

        /*
          cache window object 
          if cache window option set to true in preference setting
          @param window object
        */
        function hideWindow(windw) {
            if (!VIS.Ini.getIsCacheWindow())
                return false;
            for (var i = 0; i < s_hiddenWindows.length; i++) {
                var hidden = s_hiddenWindows[i];
                //_log.Info(i + ": " + hidden);
                if (hidden.getAD_Window_ID() == windw.getAD_Window_ID())
                    return true;	//	already there
            }
            if (windw.getAD_Window_ID() != 0) {
                try {
                    s_hiddenWindows.push(windw);
                    if (s_hiddenWindows.length > 10)
                        s_hiddenWindows.splice(0, 1);		//	sort of lru
                    return true;
                }
                catch (e) {
                    return false;
                }
            }
            return false;
        };

        /* show window from cache 
        @param AD_window_ID if of window
        */
        function showWindow(AD_Window_ID) {
            for (var i = 0; i < s_hiddenWindows.length; i++) {
                var hidden = s_hiddenWindows[i];
                if (hidden.getAD_Window_ID() == AD_Window_ID) {
                    s_hiddenWindows.splice(i, 1);// RemoveAt(i);
                    return hidden;
                }
            }
            return;
        };

        function init() {
            $menu = dm.getMenuDiv();
            $(document).on('keydown', keydownDoc).on('keyup', keydownup);

            $navRoot = dm.getNavSection();
            $innerDiv = $navRoot.find(".vis-app-action-nav-inner");

            $innerDiv.on("click", navClick);
        }
        
        /* Handle Keydown event to add shortcuts to window
        @param event : event argument provided by JQuery
        */
        function keydownDoc(event) {

            // If Dialog is opened, then window's shortcut should not work
            if ($('.ui-widget-overlay.ui-front').length > 0 && ( (event.altKey && event.ctrlKey) || event.altKey)) {
                return false;
        }
        // Open Menu (Alt + M)
        if (event.altKey && event.keyCode == 88)
        {
            currentFrame.dispose();
        }
        else if (event.altKey && event.keyCode == 77) {
            dm.openMenu();
        }
        else if ($menu.css('display') != 'none' && event.keyCode == 27) { // Close Menu (Escape)
            dm.closeMenu();
        }
        else if (event.altKey && event.keyCode == 76) { // LogOut (Alt + L)
            VIS.ADialog.confirm("LogOut?", true, "", "Confirm", function (result) {
                if (result) {
                    $('#logoutForm').trigger('submit');
                }
            });
        }
        else if (!event.altKey && (event.ctrlKey || (event.ctrlKey && event.shiftKey)) && event.keyCode == 32) { // CTRL + Space

            // If no window is opened, then return
            if (!windowObjects || Object.keys(windowObjects).length <= 0)
            {
                return;
            }

            // Curser move backword if shift and ctrl key pressed
            if (event.ctrlKey && event.shiftKey) {
                navigationShortcuts(true, false);
            }
            else {
                navigationShortcuts(true, true);
            }
            event.preventDefault();
            event.stopPropagation();
        }
            // Tab Navigation
        else if (event.altKey && event.ctrlKey && (event.keyCode == 37 || event.keyCode == 39)) {
            currentFrame.navigateThroghtShortcut(event.keyCode == 39);
            event.preventDefault();
            event.stopPropagation();
        }
            // Window's Common Actions
        else if (currentFrame && typeof currentFrame.keyDown === 'function') {
            currentFrame.keyDown.call(currentFrame, event);
        }
    };

    function keydownup(event) {
        if (!event.ctrlKey) {
            navigationShortcuts(false);
            navigatingInWindows = false;
        }
    };

    /*
    Create Navigation Shortcuts and display
    @param show: Show OR Hide navigation items
    @param forward: Move forward or backwards
    */
    function navigationShortcuts(show, forward) {
        if (show) {
            $navRoot.css('display', 'flex');
        }
        else {
            if (navigatingInWindows) {
                var item = $innerDiv.find('.vis-current-nav-window');
                showSelectedFrame(item);
            }
            return;
        }

        // navigatingInWindows false means , not navigating through open objects, So create navigation shortcuts
        if (!navigatingInWindows) {

            var htm = [];
            //Parse each opened object(window, form, report OR process) and create navigation Shortcut
            $.each(windowObjects, function (i, obj) {
               
                //if (obj.hid) {
                //    if (obj.hid.startsWith("W")) {         //if (obj.cPanel.constructor.name == 'APanel') {
                //        imgSrc = "fa fa-window-maximize";
                //        if (obj.img) {
                //            imgSrc = obj.img;
                //        }
                //    }
                //    else if (obj.hid.startsWith("P")) {    //(obj.cPanel.constructor.name == 'AProcess') {
                //        imgSrc = "fa fa-cog";
                //    }
                //    else if (obj.hid.startsWith("X")) {   //if (obj.cPanel.constructor.name == 'AForm') {
                //        imgSrc = "fa fa-list-alt";
                //    }
                //    else if (obj.hid.startsWith("R")) {    //if (obj.cPanel.constructor.name == 'AForm') {
                //        imgSrc = "vis vis-report";
                //    }
                //}
                if (!obj.img || obj.img == '') {
                    obj.img  = "fa fa-list-alt";
                }

                var imgSrc = obj.img;

                htm.push('<div tabindex=' + i + ' data-wid="' + obj.id + '" class="');

                if (currentFrame.id == obj.id) {
                    htm.push('vis-current-nav-window '); 
                }
                htm.push('vis-nav-window">');
                htm.push('<span>' + obj.name + '</span><div class="vis-nav-content-wrap">');
                if (imgSrc.indexOf('.') > -1) {
                    imgSrc = imgSrc.replace("Thumb16x16", "Thumb140x120");
                    htm.push('<img data-wid="' + obj.id + '" class="vis-nav-window-img" src="' + imgSrc + '" alt="' + obj.name +'"> </img>');
                }
                else
                    htm.push('<i data-wid="' + obj.id + '" class="'+imgSrc+'"></i>');
                htm.push('</div></div>');
            });
            $innerDiv.append(htm.join(' '));
            navigatingInWindows = true;
        }

           
        if (forward) {
            var item = $innerDiv.find('.vis-current-nav-window').next();
            if (!item || item.length <= 0) {// if control moved to last item of navigation items, then move it to first
                item = $innerDiv.children().first();
            }
        }
        else {
            var item = $innerDiv.find('.vis-current-nav-window').prev(); // if control moved to First item of navigation items while backword movement, then move it to Last
            if (!item || item.length <= 0) {
                item = $innerDiv.children().last();
            }
        }
        //}
        $innerDiv.find('.vis-current-nav-window').removeClass('vis-current-nav-window');
        item.addClass('vis-current-nav-window').focus();
    };

    function showSelectedFrame(item) {
        if (currentFrame.id != item.data('wid')) {
            dm.toggleContainer(item.data('wid'));
            dm.activateTaskBarItemUsingID(item);// Open selected Shortcut item
        }
        $navRoot.css('display', 'none');
        $innerDiv.empty();
        navigatingInWindows = false;
    };

    /*
    IF User click on Navigation Item, then open it
    */
    function navClick(event) {
        var target = $(event.target);
        if (target.hasClass('vis-nav-window') || target.hasClass('vis-nav-window-img')) {
            showSelectedFrame(target);
        }
       
    };

};



/*FRame for a Form 
 -set task bar ikon form
 -remove task bar item form task bar on close
*/
function CFrame(windowNo) {
    this.frame = new VIS.AWindow();
    this.frame.setName("Form");
    this.onOpened = null;
    this.onClose = null;
    var content = null;
    this.contentDispose = null;
    this.error = false;;
    this.windowNo = windowNo || VIS.Env.getWindowNo();
    this.hideH = false;

    var self = this;
    this.setContent = function (root) {

        if (!root.getRoot || !root.getRoot()) {
            VIS.Dialog.error("Error", true, "class must define root element  and implement getRoot function");
            root.dispose();
            this.error = true;
        }

        if (!root.sizeChanged) root.sizeChanged = function () { };

        if (!root.refresh) root.refresh = function () { };

        if (root.dispose) {
            this.contentDispose = root.dispose;
            root.dispose = function ()
            { self.dispose(); }
        }
        content = root;
        this.frame.setCFrameContent(root, this.windowNo);
        if (this.hideH)
            this.frame.hideHeader(this.hideH);
    };

    this.dispose = function () {
        if (this.disposing)
            return;
        this.disposing = true;
        this.contentDispose.call(content);
        //if (this.frame && this.frame.onClosed) {
        //    this.frame.onClosed(this.frame.id, this.frame.$layout, this.frame.hid);
        //    this.frame.onClosed = null;
        //}

        if (this.frame)
            this.frame.dispose();
        this.frame = null;
        this.onOpened = null;
        this.onClose = null;
        content = null;
        this.contentDispose = null;
        self = null;
    };
};

CFrame.prototype.setTitle = function (title) {
    this.frame.setTitle(title);
};

CFrame.prototype.getTitle = function (title) {
    return this.frame.getTitle();
};

CFrame.prototype.setName = function (name) {
    this.frame.setName(name);
};

CFrame.prototype.show = function () {
    if (this.error) {
        this.dispose();
        return;
    }


    VIS.viewManager.startCFrame(this.frame);
    if (this.onOpened)
        this.onOpened();
};

CFrame.prototype.hideHeader = function (hide) {
    this.hideH = hide;
    if (this.frame.cPanel)
        this.frame.hideHeader(hide);
};

VIS.viewManager = viewManager();

VIS.CFrame = CFrame;

})(VIS);

