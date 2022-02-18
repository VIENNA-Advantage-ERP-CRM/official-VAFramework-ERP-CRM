; (function (VIS, $) {

    var AWINDOW_HEADER_HEIGHT = 43;
    var APANEL_HEADER_HEIGHT = 50; //margin adjust of first tr
    var APANEL_FOOTER_HEIGHT = 40
    var GC_HEADER_HEIGHT = 0;

    var Level = VIS.Logging.Level;


    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var dSetUrl = baseUrl + "Form/JDataSet";

    var executeReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        var dr = null;
        getDataSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };

    //executeDataSet
    var executeDataSet = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }

        var dataSet = null;

        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            if (callback) {
                callback(dataSet);
            }
        });

        return dataSet;
    };

    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        if (params) {
            dataIn.param = params;
        }

        var value = null;


        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            var dataSet = new VIS.DB.DataSet().toJson(jString);
            if (dataSet.getTable(0).getRows().length > 0) {
                value = dataSet.getTable(0).getRow(0).getCell(0);

            }
            else { value = null; }
            dataSet.dispose();
            dataSet = null;
            if (async) {
                callback(value);
            }
        });

        return value;
    };

    var executeQueries = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls.join("/"), param: params };

        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        $.ajax({
            url: nonQueryUrl + 'ies',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(dataIn)
        }).done(function (json) {
            ret = json;
            if (callback) {
                callback(json);
            }
        });

        return ret;
    };

    //DataSet String
    function getDataSetJString(data, async, callback) {
        var result = null;
        // data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dataSetUrl,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            result = json;
            if (callback) {
                callback(json);
            }
            //return result;
        });
        return result;
    };


    var executeDReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        var dr = null;
        getDSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };

    function getDSetJString(data, async, callback) {
        var result = null;
        data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dSetUrl,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            result = json;
            if (callback) {
                callback(json);
            }
            //return result;
        });
        return result;
    };




    //****************************************************//
    //**             AWindow                           **//
    //**************************************************//



    /**
     *  Main Application Window.
     *  - Constructs, initializes and positions window framecnd_re
     *  - Gets content, menu, from APanel
     *
     */

    function AWindow() {
        this.name;
        this.windowNo;
        this.id;
        this.cPanel; // common pointer , contain Window Panel OR Form Panel OR Process Panel
        this.isHeaderVisible = true;
        this.onClosed; // event
        this.title = "window";

        var $header = null;

        var $table, $contentGrid, $lblTitle, $btnClose;
        var $toolDiv = null;
        this.onLoad = null;

        this.DocActionVariables = {};
        this.DocActionVariables.STATUS_COMPLETED = "CO";
        this.DocActionVariables.STATUS_CLOSED = "CL";
        this.DocActionVariables.STATUS_VOIDED = "VO";
        this.DocActionVariables.STATUS_REVERSED = "RE";

        function initComponent() {
            // $contentGrid = $("<div class='vis-awindow-body'>");
            $table = $("<table class='vis-awindow'>");
            var td11 = $("<td style='max-height:42px;'>");
            $contentGrid = $("<td class='vis-height-full'>");
            $lblTitle = $("<h1>");//.addClass("vis-awindow-title-label");

            // Mohit - Shortcut as title.
            $btnClose = $('<a href="javascript:void(0)" title="' + VIS.Msg.getMsg("Close") + " " + VIS.Msg.getMsg("Shct_Close") + '" class="vis-mainMenuIcons vis-icon-menuclose"></a>');

            $toolDiv = $("<div class='vis-awindow-toolbar' >");
            $header = $("<div class='vis-awindow-header vis-menuTitle' >").append($btnClose).append($lblTitle).append($toolDiv);
            td11.append($header);
            var tr1 = $("<tr>").append(td11);
            var tr2 = $("<tr>").append($contentGrid);
            $table.append(tr1).append(tr2);
        }
        initComponent();

        /* Privilized Functions */

        this.setTitle = function (titl) {
            this.title = titl;
            $lblTitle.text(this.title);
        }

        /**
          set name odf window
          */

        this.setSize = function (height, width) {
            $table.height(height);
        };


        this.hideHeader = function (hide) {

            this.isHeaderVisible = !hide;
            if (hide) {
                $header.hide();
            }
            else {
                $header.show();
            }
            this.sizeChanged();
        };

        this.hideCloseIcon = function (hide) {
            if (hide) {
                $btnClose.hide();
            }
            else {
                $btnClose.show();
            }
        };

        /** encode
         * get root container
         */
        this.getRootLayout = function () {
            return $table;
        };

        /** 
         * get content grid
         */
        this.getContentGrid = function () {
            return $contentGrid;
        };

        this.setToolbar = function (bar) {
            $toolDiv.append(bar);
        };

        var self = this; /* self pointer */

        $btnClose.on(VIS.Events.onTouchStartOrClick, function (e) {
            e.preventDefault();
            e.stopPropagation();
            self.dispose(); //dispose

        });

        /**
         * clean up 
        */
        this.disposeComponent = function () {
            self = null;
            if ($btnClose)
                $btnClose.off(VIS.Events.onTouchStartOrClick);
            $btnClose = null;

            if ($table)
                $table.remove();
            if ($contentGrid)
                $contentGrid.remove();

            $table = $contentGrid = $lblTitle = $btnClose = null;

            this.setTitle = null;

            this.name = null;
            this.windowNo = null;
            this.id = null;
            this.setTitle = null;
            this.setName = null;
            this.getRootLayout = null;
            this.getContentGrid = null;
            this.onLoad = null;
        }
    };

    AWindow.prototype.setName = function (name) {
        this.name = name;
    };

    AWindow.prototype.getName = function () {
        return this.name;
    };

    AWindow.prototype.sizeChanged = function (height, width) {
        if (!height)
            height = VIS.Env.getScreenHeight();
        if (!width)
            width = window.innerWidth;
        //if (height == VIS.Env.getScreenHeight())
        //    return;
        // console.log("resize");
       
        this.setSize(height);
        var hHeight = this.isHeaderVisible ? AWINDOW_HEADER_HEIGHT : 0;
        this.cPanel.sizeChanged(height - hHeight, width);
    };

    AWindow.prototype.refresh = function () {
        //console.log("refresh");
        this.cPanel.refresh();
        return this;
    };


    AWindow.prototype.keyDown = function (evt) {
        //console.log("refresh");
        if (this.cPanel.keyDown)
            this.cPanel.keyDown(evt);
        return this;
    };

    AWindow.prototype.navigateThroghtShortcut = function (forward) {
        //console.log("refresh");
        var fEle = $(document.activeElement);
        if (fEle && fEle.length > 0) {
            fEle.trigger("change");
            var tis = this;
            window.setTimeout(function () {
                if (tis.cPanel.navigateThroghtShortcut)
                    tis.cPanel.navigateThroghtShortcut(forward);
            }, 200);
        }
        else {
            if (this.cPanel.navigateThroghtShortcut)
                this.cPanel.navigateThroghtShortcut(forward);
        }
        return this;
    };


    /**
	 *	Dynamic Initialization Single Window
	 *  @param AD_Window_ID window
	 *  @param query selection criteria
     *  @param callback to add menu item for window
	 *  @return true if loaded OK
	 */
    AWindow.prototype.initWindow = function (AD_Window_ID, query, callback, action, sel) {

        this.cPanel = new VIS.APanel(); //initlize Apanel

        //set variable
        var windowNo = VIS.Env.getWindowNo();
        this.id = windowNo + "_" + AD_Window_ID;
        this.hid = action + "=" + AD_Window_ID;

        // $(this.getRootLayout()).attr('tabindex', windowNo);

        var self = this;


        VIS.AEnv.getGridWindow(windowNo, AD_Window_ID, function (json) {
            if (json.error != null) {
                VIS.ADialog.error(json.error);    //log error
                self.dispose();
                self = null;
                return;
            }

            var jsonData = $.parseJSON(json.result); // widow json

            VIS.context.setContextOfWindow($.parseJSON(json.wCtx), windowNo);// set window context
            //console.log(jsonData);

            self.cPanel.initPanel(jsonData, query, self, false, sel); //initPanel
            self.sizeChanged();// set size and window
            self.cPanel.createSearchAutoComplete();
            //self.cPanel.selectFirstTab();

            //Updated by raghu 
            //date:19-01-2016
            //Change/Update for:Zoom from workflow on home page
            self.cPanel.selectFirstTab(query != null);
            VIS.MLookupCache.initWindowLookup(windowNo);

            if (callback) {
                callback(self.id, null, self.name, self.hid); //add shortcut
            }

            if (self.onLoad)
                self.onLoad();
            jsonData = null;
            self = null;
        });

        this.AD_Window_ID = AD_Window_ID;
        this.windowNo = windowNo;

        this.getRootLayout().id = this.id;
        this.getRootLayout().attr("id", "AS_" + this.id);
        this.getContentGrid().append(this.cPanel.getRoot());
        return true;
    };

    AWindow.prototype.setCFrameContent = function (root, windowNo) {

        this.cPanel = root; //initlize Apanel
        //set variable
        this.windowNo = windowNo;
        this.id = this.windowNo + "_CFrame";
        //this.frame.hid = action + "=";
        this.getRootLayout().id = this.id;
        this.getRootLayout().attr("id", "AS_" + this.id);
        this.getContentGrid().append(this.cPanel.getRoot());
    };

    /**
	 *	Dynamic Initialization form
	 *  @param AD_Form_ID form
     *  @param callback to add menu item for form
	 *  @return true if loaded OK
	 */
    AWindow.prototype.initForm = function (AD_Form_ID, callback, action, additionalInfo) {

        this.cPanel = new VIS.AForm(VIS.Env.getScreenHeight() - AWINDOW_HEADER_HEIGHT); //initlize AForm

        //set variable
        var windowNo = VIS.Env.getWindowNo();
        this.id = windowNo + "_" + AD_Form_ID;
        this.hid = action + "=" + AD_Form_ID;

        var self = this;
        VIS.dataContext.getFormDataString({ AD_Form_ID: AD_Form_ID }, function (json) {
            if (json.error != null) {
                VIS.ADialog.error(json.error);    //log error
                self.dispose();
                self = null;
                return;
            }

            var jsonData = $.parseJSON(json.result); // widow json
            //console.log(jsonData);

            if (jsonData.IsError) {
                VIS.ADialog.error(jsonData.Message);    //log error
                self.dispose();
                self = null;
                return;
            }


            self.setTitle(jsonData.DisplayName);
            self.setName(jsonData.DisplayName);


            if (!self.cPanel.openForm(jsonData, self, windowNo, additionalInfo)) {
                self.dispose();
                self = null;
                return;
            }

            if (callback) {
                callback(self.id, null, self.name, self.hid); //add shortcut
            }

            jsonData = null;
            self = null;

        });

        this.windowNo = windowNo;
        this.getRootLayout().id = this.id;
        this.getRootLayout().attr("id", "AS_" + this.id);
        this.getContentGrid().append(this.cPanel.getRoot());
        return true;
    };

    /**
	 *	Dynamic Initialization process
	 *  @param AD_Process_ID process
     *  @param callback to add menu item for form
	 *  @return true if loaded OK
	 */
    AWindow.prototype.initProcess = function (AD_Process_ID, callback, action, splitUI, extrnalForm) {

        //this.cPanel = new VIS.AProcess(AD_Process_ID, VIS.Env.getScreenHeight() - AWINDOW_HEADER_HEIGHT, splitUI, extrnalForm); //initlize AForm
        this.cPanel = new VIS.AProcess(AD_Process_ID, VIS.Env.getScreenHeight() - 0, splitUI, extrnalForm); //initlize AForm
        //set variable
        var windowNo = VIS.Env.getWindowNo();
        this.id = windowNo + "_" + AD_Process_ID;
        this.hid = action + "=" + AD_Process_ID;
        this.hideHeader(true);
        var self = this;
        VIS.dataContext.getProcessDataString({ AD_Process_ID: AD_Process_ID }, function (json) {
            if (json.error != null) {
                VIS.ADialog.error(json.error);    //log error
                self.dispose();
                self = null;
                return;
            }

            var jsonData = $.parseJSON(json.result); // widow json


            if (jsonData.IsError) {
                VIS.ADialog.error(jsonData.Message);    //log error
                self.dispose();
                self = null;
                return;
            }


            self.setTitle("");
            self.setName(jsonData.Name);

            jsonData.AD_Process_ID = AD_Process_ID;
            //console.log(jsonData);

            if (!self.cPanel.init(jsonData, self, windowNo)) {
                self.dispose();
                self = null;
                return;
            }

            if (callback) {
                callback(self.id, null, self.name, self.hid); //add shortcut
            }

            jsonData = null;
            self = null;

        });

        this.windowNo = windowNo;
        this.getRootLayout().id = this.id;
        this.getRootLayout().attr("id", "AS_" + this.id);
        this.getContentGrid().append(this.cPanel.getRoot());
        return true;
    };


    AWindow.prototype.refreshProcess = function (AD_Process_ID, callback, action, splitUI, externalForm) {
        if (this.cPanel) {
            this.cPanel.disposeComponent();
            this.cPanel = null;
        }
        splitUI = true;
        this.initProcess(AD_Process_ID, callback, action, splitUI, externalForm);
        if (externalForm.disposeComponent) {
            externalForm.getParameterContainer().empty().append(this.cPanel.getParametersContainer());
            this.cPanel.getContentTable().css('height', externalForm.getContentContainer().height());
            externalForm.getContentContainer().empty().append(this.cPanel.getContentTable()).append(this.cPanel.getBusyIndicator()).css('position', 'relative');;
            externalForm.getToolbarContainer().empty().append(this.cPanel.getToolbar());
            //externalForm.getBusyIndicatorContainer().empty().append(this.cPanel.getBusyIndicator());
        }
        return this.cPanel;
    };

    AWindow.prototype.getProcessPanel = function () {
        return this.cPanel;
    }

    /**
     *  get title of window
     *
     * @return title of window
     */
    AWindow.prototype.getTitile = function () {
        return this.title;
    };

    AWindow.prototype.getAD_Window_ID = function () {
        return this.AD_Window_ID;
    };

    /**
     Show window frame on ui
     */
    AWindow.prototype.show = function ($parent, callback) {
        $parent.append(this.getRootLayout());
        if (callback) {
            callback(this.id, null, this.name, this.hid); //add shortcut
        }
        return true;
    };

    AWindow.prototype.refreshData = function () {
        this.cPanel.refreshData();
    };

    /** 
     * get id of window
     * @return unique id
     */
    AWindow.prototype.getId = function () {
        return this.id;
    };

    /** 
     *  no of window
     * @return window number
     */
    AWindow.prototype.getWindowNo = function () {
        return this.windowNo;
    };

    /** 
     * dispose 
     */
    AWindow.prototype.dispose = function () {

        //if (VIS.context.getContext("#DisableMenu") == 'Y') {
        //    return;
        //}

        if (this.onClosed) {
            if (!this.onClosed(this.id, this.$layout, this.hid, this.AD_Window_ID))
                return;
        }
        this.onClosed = null;
        if (this.cPanel)
            this.cPanel.dispose();
        this.cPanel = null;
        //if (this.AForm)
        //    this.AForm.dispose();
        //this.AForm = null;


        this.children = null;


        this.disposeComponent();
    };


    //************* AWindow End ************************//



    //****************************************************//
    //**             APanel                            **//
    //**************************************************//

    /**
     *	Main Application Panel.
     *  Structure:
     *	ToolBar
     *  tabPanel
     *  StatusBar
     *
     */
    function APanel() {

        //This variable public to Instance
        this.$parentWindow;
        this.ctx = VIS.Env.getCtx();
        //this.tabPages = {};
        this.curGC;
        this.curST;
        this.curTab;
        this.vTabbedPane = new VIS.VTabbedPane(false);
        this.statusBar = new StatusBar();
        /* current Tab panel */
        this.curWinTab = null;
        /* Tab Index */
        this.curTabIndex;
        /* Sort Tab */
        this.firstTabId = null;

        var actionItemCount_Left = 0;;
        var actionItemCount_Right = 0;


        this.toolbarCreated = false;
        this.errorDisplayed = false;

        this.isPersonalLock = VIS.MRole.getIsPersonalLock();

        this.log = VIS.Logging.VLogger.getVLogger("APanel");
        var isLocked = false;
        this.isSummaryVisible = false;
        //private 
        var $table, $divNav, $tdContentArea, $ulNav, $divSearch, $divToolbar, $ulToobar, $divStatus, $ulTabControl, $divTabControl, $divTabNav;
        var $txtSearch, $arrowSearch, $imgSearch;
        var $root, $busyDiv;
        var $rightBarLPart, $rightBarRPart, $rightBar, $ulRightBar1, $ulRightBar2, $ulRightActionbar //right bar
        var $td0leftbar, $btnlbToggle, $ulLefttoolbar, $divlbMain, $divlbNav; //left bar
        var $td1parentDetail = "", $td3IncludedEmpty, $lb, $divHeaderNav, $rigthBarAction;

        var $tr3, $tr2, $tr;
        var $td2_tr3, $td2_tr1, $td5_tr1;
        var $td4;

        /***Tab panel**/
        var $divTabPanelOuterWrap, $divTabPanels, $divTabPanelsHead, $divTabPanelsContent, $divTabPanelIconBar, $divTabPanelsIcon, $headerTabPanel, $infoIconspan, $ulIconList, $spanPin;
        var panelMaxWidth = $(document).width() / 2;
        /***END Tab panel**/





        var tabItems = [], tabLIObj = {};
        this.defaultSearch = true;
        this.isAutoCompleteOpen = false;
        this.panelWidth = 0;
        this.windowWidth = 0;

        function initComponenet() {

            $root = $("<div style='position:relative;'>"); //main div
            $busyDiv = $("<div class='vis-apanel-busy'>"); // busy indicator




            //tolbar and search 
            $ulToobar = $("<ul class='vis-appsaction-ul'>"); //toolbar item list
            $divSearch = $("<div class='vis-apanel-search'>");
            $divToolbar = $("<div class='vis-apanel-toolbar'>").append($ulToobar); //toolbar

            //navigation and tab control
            $ulNav = $("<ul class='vis-appsaction-ul vis-apanel-nav-ul'>"); //navigation list
            $divNav = $("<div class='vis-apanel-nav'>").append($ulNav); //navigation container
            $ulTabControl = $("<ul class='vis-appsaction-ul vis-apanel-tabcontrol-ul'>");//tab control
            $divTabControl = $("<div class='vis-apanel-tabcontrol'>").append($ulTabControl);
            $divTabNav = $("<div class='vis-apanel-tab-oflow'>").hide();
            $divTabNav.html("<a href='javascript:void(0)'><img data-dir='bf' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/thumb-" + (VIS.Application.isRTL ? "last" : "first") + ".png'></a><a href='javascript:void(0)'><img data-dir='b' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/thumb-" + (VIS.Application.isRTL ? "forward" : "back") + ".png'></a><a href='javascript:void(0)' ><img data-dir='r' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/thumb-" + (VIS.Application.isRTL ? "back" : "forward") + ".png' /></a><a href='javascript:void(0)' ><img data-dir='rl' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/thumb-" + (VIS.Application.isRTL ? "first" : "last") + ".png' /></a>");
            $divHeaderNav = $("<div class='vis-apanel-header-nav'>").hide();


            //left bar 
            $td0leftbar = $("<td style='width:40px;' style='height:100%' rowspan='3'>");
            $lb = $("<div class='vis-apanel-lb'>");
            $btnlbToggle = $("<div class='vis-apanel-lb-toggle' ><img class='vis-apanel-lb-img' title='" + VIS.Msg.getMsg("Actions") + "' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/mt24.png'></div>");
            $ulLefttoolbar = $("<ul class='vis-apanel-lb-ul'>");
            $divlbMain = $('<div class="vis-apanel-lb-main">');
            $divlbNav = $("<div class='vis-apanel-lb-oflow'>").hide();
            $divlbNav.html("<a data-dir='u' href='javascript:void(0)'><img style='margin-left:10px' data-dir='u' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-top.png' ></a><a data-dir='d' href='javascript:void(0)' ><img style='margin-left:10px' data-dir='d' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-bottom.png' /></a>");


            $lb.append($btnlbToggle);
            $lb.append($divlbMain);
            $lb.append($divlbNav);

            $td0leftbar.append($lb);


            //parentDetail
            $td1parentDetail = $("<td style='width:auto;height:100%;' rowspan='3'>");

            //Emptyt td
            $td3IncludedEmpty = $("<td class='vis-apanel-table-td3' style='height:100%;' rowspan='3'>");

            //right pane
            /* Right Pane */
            $rigthBarAction = $("<a href='javascript:void(0)' class='vis-flex-prev vis-apanel-sliderBackground'>").html("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/arrow-slider.png'>");
            $rightBarLPart = $("<div class='vis-apanel-bar-fixpart' style='position:absolute'>").append($rigthBarAction);

            $rightBarRPart = $("<div class='vis-apanel-bar-varpart' style='overflow-y:auto'>");
            $rightBar = $("<div class='vis-apanel-bar' style='position:relative'>").append($rightBarLPart).append($rightBarRPart);
            $td4 = $("<td rowspan='3' style='height:100%;vertical-align:top;max-width:8px;display:none'>").append($rightBar);
            $ulRightActionbar = $("<ul class='vis-apanel-rb-ul'>");
            $ulRightBar1 = $("<ul class='vis-apanel-rb-ul'>");
            $ulRightBar2 = $("<ul class='vis-apanel-rb-ul'>");


            if (VIS.Application.isRTL) {//vis-window-tab-td-rtl
                $td5_tr1 = $("<td class='vis-window-tab-td-rtl' style='display:none; max-width:" + panelMaxWidth + "px' rowspan='3' >");
            }
            else {
                $td5_tr1 = $("<td class='vis-window-tab-td' style='display:none;max-width:" + panelMaxWidth + "px' rowspan='3'>");
            }

            //center content 
            $td2_tr1 = $("<td style='height:" + APANEL_HEADER_HEIGHT + "px;position:relative'setwi>").append($divHeaderNav); //row 1 col2 //Tab Control
            // var $tdNav = $("<td style='width:250px'>").append($divNav); //row 1 col2 Navigation
            $tdContentArea = $("<td style='height:100%;'>"); //w remianing full area 

            //StatusBar
            $divStatus = $("<div class='vis-apanel-statusbar'>").hide();
            $td2_tr3 = $("<td style='height:" + APANEL_FOOTER_HEIGHT + "px'>").append($divStatus);

            $tr3 = $("<tr>").append($td2_tr3);
            $tr2 = $("<tr>").append($tdContentArea); //row 2
            $tr = $("<tr>").append($td0leftbar).append($td1parentDetail).append($td2_tr1).append($td3IncludedEmpty).append($td5_tr1).append($td4); //row 1

            /********* Tab Panels **************/
            $divTabPanelOuterWrap = $('<div class="vis-window-tab-panel-outerwrap">');
            $divTabPanels = $('<div class="vis-window-tab-panels vis-pull-left">');
            $divTabPanelsHead = $('<div class="vis-window-tabpanel-head vis-pull-left">');
            $divTabPanelsContent = $('<div class="vis-window-tab-panelContent">');
            $divTabPanelIconBar = $(' <div class="vis-window-tab-panels-iconsbar vis-pull-right">');
            $divTabPanelsIcon = $('<div class="vis-window-tab-icons">');
            $headerTabPanel = $('<h3 class="vis-pull-left">');
            $infoIconspan = $('<span class="vis-window-tab-infoicon">');
            $ulIconList = $('<ul>');
            $spanPin = $('<span class="vis-pull-right">').append('<i class="glyphicon glyphicon-remove"></i>');


            $divTabPanelsIcon.append($ulIconList);
            $divTabPanelIconBar.append($infoIconspan).append($divTabPanelsIcon);
            $divTabPanelsHead.append($headerTabPanel).append($spanPin);
            $divTabPanels.append($divTabPanelsHead).append($divTabPanelsContent);
            $divTabPanelOuterWrap.append($divTabPanels).append($divTabPanelIconBar);
            $td5_tr1.append($divTabPanelOuterWrap);


            /********* END Tab Panels **************/

            $table = $("<table class='vis-apanel-table' >"); //main root

            $root.append($table);
            $root.append($busyDiv);

            //Search 
            //$arrowSearch = $("<span style='float:right'>").text(">");
            $txtSearch = $("<input type='text' class='vis-apanel-search' placeholder='" + VIS.Msg.getMsg("Search") + "'><span style='right:58px;display:none' class='glyphicon glyphicon glyphicon-remove VIS-winSearch-autocom'></span><span style='display:none' class='glyphicon glyphicon-chevron-down VIS-winSearch-autocom'></span>");
            //$txtSearch = $("<input type='text' class='vis-apanel-search' placeholder='" + VIS.Msg.getMsg("Search") + "'>");
            // Mohit - Shortcut as title.
            $imgSearch = $("<img  title='" + VIS.Msg.getMsg("Search") + " " + VIS.Msg.getMsg("Shct_Search") + "' src='" + AppsAction.prototype.getPath() + "Sear.png'>");

            $divSearch.append($txtSearch).append($imgSearch);//.append($arrowSearch).
        };

        this.createSearchAutoComplete = function (text) {
            if ($txtSearch) {


                var $selfpanel = this;
                $($txtSearch[0]).autocomplete({
                    source: function (request, response) {
                        if (request.term.trim().length == 0) {
                            return;
                        }

                        $($txtSearch[2]).css("transform", "rotate(180deg)");

                        //                        var sqlUserSearch = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
                        //+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=" + self.curTab.getAD_Tab_ID() + " AND AD_DefaultUserQuery.AD_User_ID=" + self.ctx.getAD_User_ID() + "  )  "
                        //+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=" + self.curTab.getAD_Tab_ID() + " AND AD_DefaultUserQuery.AD_User_ID=" + self.ctx.getAD_User_ID() + "  )   ELSE null End as AD_DefaultUserQuery_ID"
                        //       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =" + self.ctx.getAD_Client_ID() + " AND AD_UserQuery.IsActive             ='Y' "
                        //       + " AND (AD_UserQuery.AD_Tab_ID           =" + self.curTab.getAD_Tab_ID() + " OR AD_UserQuery.AD_Table_ID           =" + self.curTab.getAD_Table_ID() + ")"
                        //       + " ORDER BY Upper(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";

                        var sqlUserSearch = "VIS_114";


                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@AD_Tab_ID", self.curTab.getAD_Tab_ID());
                        param[1] = new VIS.DB.SqlParam("@AD_User_ID", self.ctx.getAD_User_ID());
                        param[2] = new VIS.DB.SqlParam("@AD_Tab_ID1", self.curTab.getAD_Tab_ID());
                        param[3] = new VIS.DB.SqlParam("@AD_User_ID1", self.ctx.getAD_User_ID());
                        param[4] = new VIS.DB.SqlParam("@AD_Client_ID", self.ctx.getAD_Client_ID());
                        param[5] = new VIS.DB.SqlParam("@AD_Tab_ID2", self.curTab.getAD_Tab_ID());
                        param[6] = new VIS.DB.SqlParam("@AD_Table_ID", self.curTab.getAD_Table_ID());
                        param[7] = new VIS.DB.SqlParam("@queryData", request.term);

                        executeDataSet(sqlUserSearch, param, function (data) {
                            var userQueries = [];

                            if (data && data.tables[0].rows && data.tables[0].rows.length > 0) {
                                userQueries.push({ 'title': VIS.Msg.getMsg("All"), 'name': VIS.Msg.getMsg("All"), 'code': VIS.Msg.getMsg("All") });
                                $($txtSearch[1]).css('display', 'block');
                                $($txtSearch[2]).css('display', 'block');
                                var hasDefaultSearch = false;
                                for (var i = 0; i < data.tables[0].rows.length; i++) {
                                    if (data.tables[0].rows[i].cells["ad_defaultuserquery_id"] > 0) {
                                        userQueries.push({ 'title': data.tables[0].rows[i].cells["title"], 'name': data.tables[0].rows[i].cells["name"], 'code': data.tables[0].rows[i].cells["code"], 'id': data.tables[0].rows[i].cells["ad_userquery_id"], 'defaultids': data.tables[0].rows[i].cells["ad_defaultuserquery_id"], 'userid': data.tables[0].rows[i].cells["ad_defaultuserquery_id"] });
                                        hasDefaultSearch = true;
                                    }
                                    else {
                                        userQueries.push({ 'title': data.tables[0].rows[i].cells["title"], 'name': data.tables[0].rows[i].cells["name"], 'code': data.tables[0].rows[i].cells["code"], 'id': data.tables[0].rows[i].cells["ad_userquery_id"] });
                                    }
                                }
                                $selfpanel.toggleASearchIcons(true, hasDefaultSearch);
                            }
                            else {

                                //          var sqlUserSearch = "SELECT count(*) "
                                //+ " FROM AD_UserQuery AD_UserQuery LEFT OUTER JOIN AD_DefaultUserQuery AD_DefaultUserQuery ON AD_DefaultUserQuery.AD_UserQuery_ID=AD_UserQuery.AD_UserQuery_ID WHERE"
                                //                   + " AD_UserQuery.AD_Client_ID=" + self.ctx.getAD_Client_ID() + " AND AD_UserQuery.IsActive='Y'"
                                //                   + " AND (AD_UserQuery.AD_Tab_ID=" + self.curTab.getAD_Tab_ID() + " OR AD_UserQuery.AD_Table_ID=" + self.curTab.getAD_Table_ID() + ")";
                                //          sqlUserSearch += " ORDER BY AD_UserQuery.AD_UserQuery_ID";


                                var sqlUserSearch = "VIS_115";

                                var param = [];
                                param[0] = new VIS.DB.SqlParam("@AD_Client_ID", self.ctx.getAD_Client_ID());
                                param[1] = new VIS.DB.SqlParam("@AD_Tab_ID", self.curTab.getAD_Tab_ID());
                                param[2] = new VIS.DB.SqlParam("@AD_Table_ID", self.curTab.getAD_Table_ID());

                                executeDataSet(sqlUserSearch, param, function (data) {
                                    var userQueries = [];
                                    if (data && data.tables[0].rows && data.tables[0].rows.length > 0) {
                                        if (data.tables[0].rows[0].cells[0] > 0) {
                                            $selfpanel.toggleASearchIcons(true, false);
                                        }
                                        else {
                                            $selfpanel.toggleASearchIcons(false, false);
                                        }
                                    }

                                });

                            }

                            response($.map(userQueries, function (item) {
                                return {
                                    label: item.name,
                                    value: item.name,
                                    code: item.code,
                                    title: item.title,
                                    id: item.id,
                                    defid: item.defaultids,
                                    userid: item.userid,
                                }
                            }));
                        });
                        //$(self.div).autocomplete("search", "");
                        //$(self.div).trigger("focus");

                    },
                    select: function (ev, ui) {
                        //self.cmd_find(ui.item.code);

                        //if (ev.originalEvent.keyCode == 13) {
                        //    self.defaultSearch = false;
                        //}

                        self.defaultSearch = false;
                        if (self.isSummaryVisible) {
                            self.curTab.setShowSummaryNodes(true);
                        }
                        else {
                            self.curTab.setShowSummaryNodes(false);
                        }
                        var query = new VIS.Query(self.curTab.getTableName(), false);

                        self.curTab.searchText = ui.item.value;
                        self.curTab.userQueryID = ui.item.id;

                        if (ui.item.code != VIS.Msg.getMsg("All")) {
                            query.addRestriction(ui.item.code);
                        }
                        //else {
                        //    query.addRestriction(" 1 = 1 ");
                        //}
                        //	History

                        //Set Page value to 1
                        self.curTab.getTableModel().setCurrentPage(1);
                        //	Confirmed query
                        self.curTab.setQuery(query);
                        self.curGC.query(0, 0, false);   //  autoSize
                        $($txtSearch[1]).css("display", "inherit");
                        $($txtSearch[2]).css("display", "inherit");
                        $($txtSearch[2]).css("transform", "rotate(360deg)");
                        ev.stopPropagation();
                    },
                    minLength: 0,
                    open: function (ev, ui) {
                        $selfpanel.isAutoCompleteOpen = true;
                    },
                    close: function (event, ui) {
                        $($txtSearch[2]).css("transform", "rotate(360deg)");
                        window.setTimeout(function () {

                            $selfpanel.isAutoCompleteOpen = false;

                        }, 400);
                    }
                })

                window.setTimeout(function () {

                    $($txtSearch[0]).autocomplete().data('ui-autocomplete')._renderItem = function (ul, item) {


                        var span = null;
                        if ($selfpanel.curTab.getTabLevel() == 0) {
                            if (item.defaultids && item.userid > 0) {
                                span = $("<span title='" + VIS.Msg.getMsg("DefaultSearch") + "'  data-id='" + item.id + "' class='VIS-winSearch-defaultIcon'></span>");
                            }
                            else {
                                span = $("<span title='" + VIS.Msg.getMsg("MakeDefaultSearch") + "' data-id='" + item.id + "' class='VIS-winSearch-NonDefaultIcon'></span>");
                            }
                        }
                        else {
                            span = $("<span title='" + VIS.Msg.getMsg("DefaultSearch") + "'  data-id='" + item.id + "'></span>");
                        }


                        var li = $("<li>")
                            .append($("<a style='display:block' title='" + item.title + "'>" + item.label + "</a>").append(span))
                            .appendTo(ul);


                        span.on("click", function (e) {

                            var uQueryID = $(this).data('id');

                            $.ajax({
                                url: VIS.Application.contextUrl + "JsonData/InsertUpdateDefaultSearch",
                                dataType: "json",
                                data: { AD_Tab_ID: self.curTab.getAD_Tab_ID(), AD_Table_ID: self.curTab.getAD_Table_ID(), AD_User_ID: self.ctx.getAD_User_ID(), AD_UserQuery_ID: uQueryID },
                                success: function (data) {

                                },
                                error: function (er) {
                                    console.log(er);
                                }

                            });
                        });

                        return li;
                    };
                    // $($txtSearch[0]).autocomplete().data('ui-autocomplete').find('.VIS-winSearch-defaultIcon').off("click");
                    // $($txtSearch[0]).autocomplete().data('ui-autocomplete').find('.VIS-winSearch-defaultIcon').on("click", function (e) {

                    //  console(this);

                    //});

                }, 200);




            }
        };

        function finishLayout() {
            $divHeaderNav.append($divTabControl).append($divTabNav).append($divNav);
            $rightBarRPart.append("<h1 class='vis-apnel-rb-header'>" + VIS.Msg.getMsg("Action") + "</h1>").append($ulRightActionbar);
            //$rightBarRPart.append("<h1 class='vis-apnel-rb-header'>" + VIS.Msg.getMsg("Setting") + "</h1>").append($ulRightBar1);
            $rightBarRPart.append("<h1 class='vis-apnel-rb-header'>" + VIS.Msg.getMsg("Related") + "</h1>").append($ulRightBar2);

            $divlbMain.append($ulLefttoolbar);
            $divHeaderNav.show();
            $divStatus.show();
            $table.append($tr).append($tr2).append($tr3);
            $tr3 = $tr2 = $tr = null;
        }
        /* Tool bar */
        var self = this;
        initComponenet();

        $divStatus.append(this.statusBar.getRoot()); //Status bar

        var h = VIS.Env.getScreenHeight() - AWINDOW_HEADER_HEIGHT;
        $table.height(h);
        $busyDiv.height(h);

        this.setSize = function (height, width) {

            $table.height(height);
            $lb.height(height);
            $rightBar.height(height);
            $divStatus.height(APANEL_FOOTER_HEIGHT);
            $divlbMain.height(height - 43); //left bar overflow

            if ($ulLefttoolbar.height() > $divlbMain.height()) {
                if (!VIS.Application.isMobile) {
                    $divlbMain.height(height - (43 + 40));
                    $divlbNav.show();
                }
            }
            if (this.aParentDetail) {
                this.aParentDetail.setSize(height);
            }
            $busyDiv.height(height);
            height = null;

        }


        //Action Perormed
        var onAction = function (action) {
            self.actionPerformed(action);
        };

        //tabAction

        this.onTabChange = function (action) {
            self.tabActionPerformed(action);

        };

        this.statusBar.onComboChange = function (index) {
            self.setBusy(true);
            //console.log(index);
            setTimeout(function () {
                self.curGC.navigatePageExact(index + 1);
                if (!self.curGC.onDemandTree) {
                    self.setBusy(false);
                }
            }, 100);
        };

        this.createToolBar = function () {

            //1. toolbar action
            this.aRefresh = this.addActions(this.ACTION_NAME_REFRESH, null, true, true, false, onAction, null, "Shct_Refresh");
            this.aDelete = this.addActions(this.ACTION_NAME_DELETE, null, true, true, false, onAction, null, "Shct_Delete");
            this.aNew = this.addActions(this.ACTION_NAME_NEW, null, true, true, false, onAction, null, "Shct_New");
            this.aIgnore = this.addActions("Ignore", null, true, true, false, onAction, null, "Shct_Ignore");
            this.aSave = this.addActions("Save", null, true, true, false, onAction, null, "Shct_Save");
            this.aFind = this.addActions("Find", null, true, true, false, onAction, null, "Shct_Find");
            this.aInfo = this.addActions("Info", null, true, true, false, onAction, null, "Shct_Info");
            this.aReport = this.addActions("Report", null, true, true, false, onAction, null, "Shct_Report");
            this.aPrint = this.addActions("Print", null, true, true, false, onAction, null, "Shct_Print");


            $ulToobar.append(this.aIgnore.getListItm());
            $ulToobar.append(this.aNew.getListItm());
            $ulToobar.append(this.aDelete.getListItm());
            $ulToobar.append(this.aSave.getListItm());
            $ulToobar.append(this.aRefresh.getListItm());
            $ulToobar.append(this.aReport.getListItm());
            $ulToobar.append(this.aPrint.getListItm());



            //lakhwinder
            //$ulToobar.append(this.aInfo.getListItm());

            $ulToobar.append(new AppsAction().getSeprator(false, true));
            $ulToobar.append(this.aFind.getListItm());

            // Mohit - Shortcut as title.
            ////2.Navigation sub-tollbar
            this.aPrevious = this.addActions(this.ACTION_NAME_PREV, null, true, true, true, onAction, null, "Shct_PrevRec");
            this.aFirst = this.addActions(this.ACTION_NAME_FIRST, null, true, true, true, onAction, null, "Shct_FirstRec");
            this.aLast = this.addActions(this.ACTION_NAME_LAST, null, true, true, true, onAction, null, "Shct_LastRec");
            this.aNext = this.addActions(this.ACTION_NAME_NEXT, null, true, true, true, onAction, null, "Shct_NextRec");
            this.aMulti = this.addActions("Multi", null, false, true, true, onAction, true, "Shct_MultiRow");
            this.aCard = this.addActions("Card", null, false, true, true, onAction, null, "Shct_CardView");

            this.aMap = this.addActions("Map", null, false, true, true, onAction);

            //this.aBack = this.addActions("Back", null, false, true, true, onAction);
            //Create Navigation
            //$ulNav.append(this.aPrevious.getListItm()).append(this.aFirst.getListItm()).append(this.aLast.getListItm()).append(this.aNext.getListItm());
            $ulNav.append(this.aFirst.getListItm()).append(this.aPrevious.getListItm()).append(this.aNext.getListItm()).append(this.aLast.getListItm());
            $ulNav.append(this.aMulti.getListItm());
            $ulNav.append(this.aCard.getListItm());
            $ulNav.append(this.aMap.getListItm().hide());

            // Mohit - Shortcut as title.
            ///3. bottom toolbar 
            this.aPageUp = this.addActions("PageUp", null, true, true, true, onAction, null, "Shct_PageUp");
            this.aPageFirst = this.addActions("PageFirst", null, true, true, true, onAction, null, "Shct_PageFirst");
            this.aPageLast = this.addActions("PageLast", null, true, true, true, onAction, null, "Shct_PageLast");
            this.aPageDown = this.addActions("PageDown", null, true, true, true, onAction, null, "Shct_PageDown");

            //Action Bar[Left] 


            var mWindow = this.gridWindow;
            actionItemCount_Right = 0;
            if (mWindow.getIsAppointment()) {
                this.aAppointment = this.addActions("Appointment", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aAppointment.getListItmIT());
            }
            if (mWindow.getIsTask()) {
                this.aTask = this.addActions("Task", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aTask.getListItmIT());
            }
            if (mWindow.getIsEmail()) {
                this.aEmail = this.addActions("EMail", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aEmail.getListItmIT());
            }
            if (mWindow.getIsLetter()) {
                this.aLetter = this.addActions("Letter", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aLetter.getListItmIT());
            }
            if (mWindow.getIsSms()) {
                this.aSms = this.addActions("Sms", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aSms.getListItmIT());
            }
            if (mWindow.getIsFaxEmail()) {
                this.aFaxEmail = this.addActions("FaxEmail", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aFaxEmail.getListItmIT());
            }






            //add
            if (mWindow.getIsChat()) {
                this.aChat = this.addActions(this.ACTION_NAME_CHAT, null, false, false, false, onAction, true);  //1
                $ulLefttoolbar.append(this.aChat.getListItmIT());
            }
            if (mWindow.getIsAttachment()) {
                this.aAttachment = this.addActions("Attachment", null, false, false, false, onAction, true); //1
                $ulLefttoolbar.append(this.aAttachment.getListItmIT());
            }
            if (mWindow.getIsHistory()) {
                this.aHistory = this.addActions("History", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aHistory.getListItmIT());
            }
            if (mWindow.getIsCheckRequest()) {
                this.aRequest = this.addActions("Request", null, true, false, false, onAction);
                $ulLefttoolbar.append(this.aRequest.getListItmIT());
            }


            if (VIS.AEnv.getIsWorkflowProcess()) {
                this.aWorkflow = this.addActions("Workflow", null, true, false, false, onAction);
                $ulLefttoolbar.append(this.aWorkflow.getListItmIT());
            }

            if (mWindow.getIsCopyReocrd()) {
                this.aCopy = this.addActions("Copy", null, false, false, false, onAction);
                $ulLefttoolbar.append(this.aCopy.getListItmIT());
            }
            if (mWindow.getIsSubscribedRecord()) {
                this.aSubscribe = this.addActions("Subscribe", null, true, false, false, onAction, true);
                $ulLefttoolbar.append(this.aSubscribe.getListItmIT());
            }
            if (mWindow.getIsZoomAcross()) {
                this.aZoomAcross = this.addActions("ZoomAcross", null, true, false, false, onAction);
                $ulLefttoolbar.append(this.aZoomAcross.getListItmIT());
            }
            if (mWindow.getIsCreatedDocument()) {
                this.aCreateDocument = this.addActions("CreateDocument", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aCreateDocument.getListItmIT());
            }
            if (mWindow.getIsUploadedDocument()) {
                this.aUploadDocument = this.addActions("UploadDocument", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aUploadDocument.getListItmIT());
            }
            if (mWindow.getIsViewDocument()) {
                this.aViewDocument = this.addActions("ViewDocument", null, false, false, false, onAction, true); //1
                $ulLefttoolbar.append(this.aViewDocument.getListItmIT());
            }
            if (mWindow.getIsAttachDocumentFrom()) {
                this.aAttachFrom = this.addActions("AttachDocumentFrom", null, false, false, false, onAction, true); //1
                $ulLefttoolbar.append(this.aAttachFrom.getListItmIT());
            }
            if (mWindow.getIsMarkToExport()) {
                this.aMarkToExport = this.addActions("Mark", null, false, false, false, onAction, true); //1
                $ulLefttoolbar.append(this.aMarkToExport.getListItmIT());
            }

            if (mWindow.getIsImportMap()) {
                this.aImportMap = this.addActions("Import", null, false, false, false, onAction); //1
                $ulLefttoolbar.append(this.aImportMap.getListItmIT());
            }



            if (mWindow.getIsArchive()) {
                //this.aArchive = this.addActions("Archive", null, false, false, false, onAction); //1
                //$ulLefttoolbar.append(this.aArchive.getListItmIT());
            }
            if (mWindow.getIsAttachmail()) {
                //this.aEmailAttach = this.addActions("EmailAttach", null, false, false, false, onAction); //1
                //$ulLefttoolbar.append(this.aEmailAttach.getListItmIT());
            }
            if (mWindow.getIsRoleCenterView()) {
                //this.aRoleCenterView = this.addActions("RoleCenterView", null, false, false, false, onAction); //1
                //$ulLefttoolbar.append(this.aRoleCenterView.getListItmIT());
            }

            if (this.isPersonalLock) {
                this.aLock = this.addActions("Lock", null, true, false, false, onAction, true);
                $ulLefttoolbar.append(this.aLock.getListItmIT());
                this.aRecAccess = this.addActions("RecordAccess", null, true, false, false, onAction, true);
                $ulLefttoolbar.append(this.aRecAccess.getListItmIT());
            }


            //if (mWindow.getIsCall()) {
            //    this.aCall = this.addActions("Call", null, false, false, false, onAction, true);  //1
            //    $ulLefttoolbar.append(this.aCall.getListItmIT());
            //}

            //$ulLefttoolbar.append(this.aImportMap.getListItmIT());
            // $ulLefttoolbar.append(this.aMarkToExport.getListItmIT());
            // $ulLefttoolbar.append(this.aArchive.getListItmIT());
            // $ulLefttoolbar.append(this.aEmailAttach.getListItmIT());
            //$ulLefttoolbar.append(this.aRoleCenterView.getListItmIT());












            this.aPreference = this.addActions("Preference", null, false, false, true, onAction); //2
            /////5 Right bar
            if (VIS.MRole.getDefault().getIsShowPreference()) {

                $ulRightBar2.append(this.aPreference.getListItmIT());
            }

            this.aHelp = this.addActions("Help", null, true, false, true, onAction);
            $ulRightBar2.append(this.aHelp.getListItmIT());

            this.aCardDialog = this.addActions("CardDialog", null, true, false, true, onAction);
            $ulRightBar2.append(this.aCardDialog.getListItmIT());

            this.aShowSummaryLevel = this.addActions("ShowSummaryNodes", null, true, false, true, onAction, true);
            $ulRightBar2.append(this.aShowSummaryLevel.getListItmIT());


            mWindow = null;




            //Up
            //$ulRightBar1.append(this.aSubscribe.getListItmIT());
            //$ulRightBar1.append(this.aZoomAcross.getListItmIT());
            //2


            this.statusBar.setPageItem(this.aPageFirst.getListItm());
            this.statusBar.setPageItem(this.aPageUp.getListItm());
            this.statusBar.setComboPage();
            this.statusBar.setPageItem(this.aPageDown.getListItm());
            this.statusBar.setPageItem(this.aPageLast.getListItm());
            this.statusBar.render();
            this.toolbarCreated = true;

            /* Set Tool Bar */
            this.$parentWindow.setToolbar($divToolbar);
            this.$parentWindow.setToolbar($divSearch);
            finishLayout();


        };

        this.setDynamicActions = function () {

            if (this.curGC == null)
                return;

            ///* Clear Previous */
            //var rightActionCount = $ulRightActionbar.children().length;

            //if (rightActionCount > actionItemCount_Right) {

            //}

            var index = 0;
            var actions = [];
            if (this.curGC.leftPaneLinkItems.length > 0) {
                actions = this.curGC.leftPaneLinkItems;
                for (index = 0; index < actions.length; index++) {
                    $ulRightActionbar.append(actions[index].getControl());

                }
            }
            index = 0;
            if (this.curGC.rightPaneLinkItems.length > 0) {
                actions = this.curGC.rightPaneLinkItems;
                for (index = 0; index < actions.length; index++) {
                    $ulRightActionbar.append(actions[index].getControl());

                }
            }
            actions = null;
        };

        //privilized function
        this.getRoot = function () { return $root; };

        this.getLayout = function () { return $tdContentArea; };

        this.getIncludedEmptyArea = function () {
            return $td3IncludedEmpty;
        }

        /*left bar */

        this.getParentDetailPane = function () {
            return $td1parentDetail;
        };


        /* Set Tab Panel Icons */

        $infoIconspan.on("click", function () {
            $rightBarLPart.trigger("click");
        });

        /*
        *   Set Icons of tab panels for tab
        */



        this.setTabPanelIcons = function () {
            if (this.curGC) {
                $ulIconList.empty();
                $ulIconList.append(this.curGC.createTabPanel(this.curTab.getTabPanels()));
                if (!this.curGC.getCurrentPanel()) {// if no panel opened then check for default panel.
                    var defaultPanel = $ulIconList.find("[default='true']").first();
                    if (defaultPanel && defaultPanel.length > 0) {
                        setPanelPositionRelative();
                        defaultPanel.trigger("click");
                        this.showPanelContainer(true);
                    }
                    else {
                        this.showPanelContainer(false);
                    }
                }
                else { // if panel opened then check for that panel.
                    var panelID = this.curGC.getCurrentPanelID();
                    var defaultPanel = $ulIconList.find("[data-panelid='" + panelID + "']").first();
                    if (defaultPanel && defaultPanel.length > 0) {
                        setPanelPositionRelative();
                        defaultPanel.trigger("click");
                        this.showPanelContainer(true);
                    }
                    else {
                        this.showPanelContainer(false);
                    }
                }
            }
        };

        /*
        *   Set current Panel for tab
        */
        this.setCurrentTabPanel = function () {
            if (this.curGC) {
                // Get Current panel for tab
                var panel = this.curGC.getCurrentPanel();
                if (panel) {
                    $divTabPanelsContent.empty();
                    // if panel found, open it in container
                    $divTabPanelsContent.append(panel.getRoot());
                    this.showPanelContainer(true);
                }
            }
        };

        /*
        *   Show OR hide tab panel depending on, if linked tab panel or not
        */
        this.showTabPanel = function (show) {
            if (show) {
                $td5_tr1.css({ "display": "" });
                if (this.curTab.getIncluded_Tab_ID() > 0) {
                    $td3IncludedEmpty.css('width', '');
                    var style = $td3IncludedEmpty.attr('style', style);
                }
                else {
                    if ($td5_tr1.width() <= 40) {
                        $td3IncludedEmpty.css({ 'width': '', 'padding-left': '', 'padding-right': '' });
                    }
                    else {
                        $td3IncludedEmpty.width(0);
                    }
                }
            }
            else {
                $td3IncludedEmpty.css('width', '');
                $td5_tr1.css({ "display": "" });
                $td5_tr1.css("width", "40px");
                $divTabPanels.css("display", "none");
            }
        };

        /*
            *   Change width of included tab, if no included tab linked
            */
        this.setIncludedTabWidth = function (show) {
            if (show) {
                $td3IncludedEmpty.css('width', '');
            }
            else {
                $td3IncludedEmpty.css('width', '0px');
            }
        }

        /*
          *   Show or hide panel container depending on panel linked to tab or not
          */
        this.showPanelContainer = function (show) {
            if (show) {
                $td5_tr1.css("width", this.panelWidth + 'px');
                $divTabPanels.css({ "display": "", "z-index": "" });
            }
            else {
                $td5_tr1.css({ "display": "" });
                $td5_tr1.css("width", "40px");
                $divTabPanels.css("display", "none");
                $divTabControl.css('width', '');
                //$table.find('.vis-apanel-tabcontrol').css('max-width', '')

                if ($td5_tr1.is('.ui-resizable')) {
                    $td5_tr1.resizable('destroy');
                }
            }
            this.setTabNavigation();
        };

        /*
          *   Set width
          */
        this.setWidth = function (width, openPanel) {
            var self = this;
            //window.setTimeout(function () {
            if (width) {
                if (width > 0) {
                    //$td5_tr1.css({ "width": width + "%" });
                    //$td5_tr1.attr("width", width + "%");
                    // self.panelWidth = $table.find('.vis-window-tab-td').width();
                    width = 100 - width;
                    width = ($(document).width() * width) / 100;
                    if (width > panelMaxWidth) {
                        width = panelMaxWidth;
                    }
                    $td5_tr1.css({ "width": width + "px" });
                    self.panelWidth = width;

                }
                // $divTabControl.css('display', 'none');
                // var extraWidth = 44;

                // if (this.curTab.getIncluded_Tab_ID() > 0 && $divHeaderNav.width() <= 752) {
                //if (openPanel && self.curTab.getHasPanel()) {
                // extraWidth += 96;// Tab Navigation width
                // extraWidth += 188;// Record Navigation width
                //   extraWidth += $divTabControl.width(); // Tabs Width

                //   if (extraWidth >= $divHeaderNav.width()) {
                //       extraWidth = $divHeaderNav.width() - ($divTabControl.width() + $table.find('.vis-apanel-tab-oflow').width())
                //   }
                //   else {
                //       extraWidth = 44;
                //  }

                // }

                ////if ($divHeaderNav.width() <= 400) {
                ////    $ulNav.removeClass('vis-apanel-nav-ul li');
                ////    $ulNav.find('li').css('margin', '0px 1px 0px 1px');
                ////}
                ////else {
                ////    $ulNav.addClass('vis-apanel-nav-ul li');
                ////}

                // panel width vala code dekhna hai
                //if ($divHeaderNav.width() <= 800) {
                //        $divTabControl.css('width', ($divHeaderNav.width() - (96 + extraWidth + 188)) + 'px');
                //}
                //else {
                //    $divTabControl.css('width', '');
                //}
                // $divTabControl.width($divHeaderNav.width() - 305);

                // $divTabControl.css('display', '');
                this.refresh();
            }
            // }, 100);
        };

        /*
          *   Close tab panel.
          */
        $spanPin.on("click", function () {
            self.showPanelContainer(false);
            self.setIncludedTabWidth(true);
            self.setWidth(-1, false);
            self.curGC.setCurrentPanel(null);
            self.setTabNavigation();
            var selected = $ulIconList.find('.vis-selected-list')[0];
            $(selected).removeClass('vis-selected-list');



        });

        //function setPanelPosition() {
        //    if ($spanPin.hasClass('vis-window-unpin-icon')) {
        //        setPanelPositionAbsolute();
        //    }
        //    else {
        //        setPanelPositionRelative();
        //    }
        //};

        function setPanelPositionRelative() {
            //$spanPin.addClass('vis-window-unpin-icon');
            $td5_tr1.css({ 'position': 'relative', "z-index": "" });
            var dragType = 'w';
            if (VIS.Application.isRTL) {
                dragType = 'e';
            }

            if (!$td5_tr1.is('.ui-resizable')) {
                $td5_tr1.resizable({
                    handles: dragType,
                    minWidth: 102,
                    maxWidth: panelMaxWidth,
                    resize: function (event, ui) {
                        self.panelWidth = ui.size.width;
                        $td5_tr1.css({ 'position': 'absolute', "left": "", "z-index": "99" });
                    },
                    start: function (event, ui) {
                        $td5_tr1.css({ 'position': 'absolute', "z-index": "99" });
                        //windowWidth=
                    },
                    stop: function (event, ui) {

                        //var style = $td3IncludedEmpty.attr('style');
                        //if (self.curTab.getIncluded_Tab_ID() == 0) {
                        //    style += 'width:0px;padding-left: 1px !important; padding-right: 1px !important';
                        //}
                        //else {
                        //    style += 'padding-left: 1px !important; padding-right: 1px !important';
                        //}
                        //$td3IncludedEmpty.attr('style', style);
                        if (self.curTab.getIncluded_Tab_ID() == 0) {
                            $td3IncludedEmpty.css('width', '0px');
                        }
                        else {
                            $td3IncludedEmpty.css('width', '');
                        }
                        var curPanel = self.curGC.getCurrentPanel();
                        if (curPanel && curPanel.sizeChanged) {
                            curPanel.sizeChanged(ui.originalSize.width);
                        }


                        if (VIS.Application.isRTL) {
                            $td5_tr1.css({ 'position': 'relative', "right": "", "z-index": "" });
                        }
                        else {
                            $td5_tr1.css({ 'position': 'relative', "left": "", "z-index": "" });
                        }
                        self.setWidth(-1, true);
                        if (self.curTab.getIncluded_Tab_ID() > 0) {
                            self.curGC.vIncludedGC.vTable.resize();
                        }
                        self.setTabNavigation();
                    }
                });
            }
        };

        function setPanelPositionAbsolute() {
            // $spanPin.removeClass('vis-window-unpin-icon');
            self.setWidth(-1, false);
            $td5_tr1.css({ 'position': 'relative', 'left': '', 'z-index': '' });
            //if ($td5_tr1.is('.ui-resizable')) {
            //    $td5_tr1.resizable('destroy');
            //}
        };

        $ulIconList.on("click", function (e) {
            var $target = $(e.target);
            if ($target.is('li')) {

                var sibling = $target.siblings('.vis-selected-list');
                if (sibling) {
                    sibling.removeClass('vis-selected-list');
                }
                sibling.removeClass('vis-selected-list');
                $target.addClass('vis-selected-list');
                $target = $target.find('img');

            }
            else if ($target.is('img')) {
                var $parent = $($target.parent());
                var sibling = $parent.siblings('.vis-selected-list');
                if (sibling) {
                    sibling.removeClass('vis-selected-list');
                }
                $parent.addClass('vis-selected-list');
            }
            $headerTabPanel.text($target.data('name'));

            $divTabPanelsContent.empty();
            //var type = VIS.Utility.getFunctionByName($target.data('cname'), window);
            //panel = new type();
            //panel.startPanel(self.$parentWindow.getWindowNo(), self.curTab);
            //panel.refreshPanelData(self.curTab.getRecord_ID(), self.curGC.getSelectedRows());
            //self.curGC.setCurrentPanel(panel);
            //$divTabPanelsContent.append(panel.getRoot());
            self.curGC.setCurrentPanelID($target.data('panelid'));
            self.curGC.setCurrentPanel($target.data('cname'), self.$parentWindow.getWindowNo());
            $divTabPanelsContent.append(self.curGC.getCurrentPanel().getRoot());
            setPanelPositionRelative();
            if ($td5_tr1.width() <= 40) {
                self.showPanelContainer(true);

            }
            if (self.curTab.getIncluded_Tab_ID() == 0) {
                self.setIncludedTabWidth(false);

            }
            else {
                self.setIncludedTabWidth(true);
            }

            self.showTabPanel(true);
            self.setWidth(-1, true);
            self.setTabNavigation();



        });

        /* END Set Tab Panel Icons */

        /*tabcontrol */
        this.setTabControl = function (tabs) {
            tabItems = tabs;
            for (var i = 0; i < tabs.length; i++) {
                // tabItems[i] = tabs[i].getTabBtnListItm();
                var li = tabs[i].getListItm();
                //if (tabItems[i].action === id) {

                //  this.selectedTab = li;
                //  this.selTabIndex = i;
                //  this.seletedTab.addClass("vis-apanel-tab-selected");
                // }
                tabLIObj[tabItems[i].action] = li;
                $ulTabControl.append(li);
            }

            //this.vTabbedPane.setTabObject(tabLIObj);
            if ($ulTabControl.width() > $divTabControl.width()) {
                if (!VIS.Application.isMobile)
                    $divTabNav.show();
            }
        };

        this.setTabNavigation = function () {
            if ($ulTabControl.width() > $divTabControl.width()) {
                if (!VIS.Application.isMobile)
                    $divTabNav.show();
            }
            else {
                $divTabNav.hide();
            }
        };

        this.setSelectedTab = function (id) {
            if (this.selectedTab)
                this.selectedTab.removeClass("vis-apanel-tab-selected");
            this.selectedTab = tabLIObj[id];
            this.selectedTab.addClass("vis-apanel-tab-selected");
        };


        this.navigateThroghtShortcut = function (forward) {




            var next = null;
            if (forward) {
                next = $ulTabControl.find('.vis-apanel-tab-selected').next();
                if (!next || next.length <= 0) {
                    next = $ulTabControl.children().first();
                }
            }
            else {
                next = $ulTabControl.find('.vis-apanel-tab-selected').prev();
                if (!next || next.length <= 0) {
                    next = $ulTabControl.children().last();
                }
            }
            //$ulTabControl.find('.vis-apanel-tab-selected').removeClass('vis-apanel-tab-selected');
            //next.addClass('vis-current-nav-window').focus();
            next.trigger('click');
        };

        this.setBusy = function (busy, focus) {
            isLocked = busy;
            if (busy) {
                //		setStatusLine(processing);
                $busyDiv[0].style.visibility = 'visible';// .show();
            }
            else {
                //$busyDiv.hide();
                $busyDiv[0].style.visibility = 'hidden';
                if (focus) {
                    //curGC.requestFocusInWindow();
                }
            }
        };

        $divTabNav.on("click", function (e) {
            e.stopPropagation();
            var dir = $(e.target).data('dir');
            if (!dir) return;

            var dWidth = $divTabControl.width();
            var ulWidth = $ulTabControl.width();
            var cPos = $divTabControl.scrollLeft();
            var offSet = Math.ceil(dWidth / 2);
            //console.log(dWidth + "--" + ulWidth + '---' + cPos);
            var s = 0;
            if (dir == 'r') {
                if ((cPos + offSet) >= ulWidth - offSet)
                    return;
                var ms = ulWidth - dWidth;
                s = cPos + offSet;
                $divTabControl.animate({ scrollLeft: s > ms ? ms : s }, 1000);
            }
            else if (dir == 'b') {
                if (cPos == 0)
                    return;
                s = (cPos - offSet);
                $divTabControl.animate({ scrollLeft: s < 0 ? 0 : s }, 1000);
                //$divTabControl.scrollLeft(cPos - offSet);
            }
            if (dir == 'rl') {
                if ((cPos + offSet) >= ulWidth - offSet)
                    return;
                var ms = ulWidth - dWidth;
                //s = cPos + offSet;
                $divTabControl.animate({ scrollLeft: ms }, 500);
            }
            else if (dir == 'bf') {
                if (cPos == 0)
                    return;
                s = (cPos - offSet);
                $divTabControl.animate({ scrollLeft: 0 }, 500);
                //$divTabControl.scrollLeft(cPos - offSet);
            }

        });

        $divlbNav.on("click", function (e) {
            e.stopPropagation();
            var dir = $(e.target).data('dir');
            if (!dir) return;

            var dHeight = $divlbMain.height();
            var ulheight = $ulLefttoolbar.height();
            var cPos = $divlbMain.scrollTop();
            var offSet = Math.ceil(dHeight / 2);
            var s = 0;
            if (dir == 'd') {
                if ((cPos + offSet) >= ulheight - offSet)
                    return;
                var ms = ulheight - dHeight;
                s = cPos + offSet;
                $divlbMain.animate({ scrollTop: s > ms ? ms : s }, 1000, "easeOutBounce");
            }
            else if (dir == 'u') {
                if (cPos == 0)
                    return;
                s = (cPos - offSet);
                $divlbMain.animate({ scrollTop: s < 0 ? 0 : s }, 1000, "easeOutBounce");
                //$divTabControl.scrollLeft(cPos - offSet);
            }
        });



        //Search
        $imgSearch.on(VIS.Events.onTouchStartOrClick, function (e) {
            //  e.stopPropagation();
            //   e.preventDefault();

            if ($($txtSearch[1]).is(':visible') == true) {
                $($txtSearch[1]).css("display", "none");
                $($txtSearch[0]).val("");
                self.curTab.searchText = "";
                var query = new VIS.Query();
                //query.addRestriction(" 1 = 1 ");
                self.findRecords(query);
            }
            else {
                self.cmd_find($txtSearch.val());
                $($txtSearch[1]).css("display", "none");
                self.curTab.searchText = "";
                $txtSearch.val("");
            }
            e.stopPropagation();

        });

        if (!VIS.Application.isMobile) {
            $($txtSearch[0]).on("keyup", function (e) {
                var code = e.charCode || e.keyCode;
                if (code == 13) {
                    if (!self.defaultSearch) {
                        //self.defaultSearch = true;
                        return;
                    }
                    $($txtSearch[1]).css("display", "none");
                    self.cmd_find($($txtSearch[0]).val());
                    $($txtSearch[0]).val("");
                }
                else if (code == 8) {
                    if ($($txtSearch[1]).is(':visible') == true) {
                        e.preventDefault();
                        self.defaultSearch = true;
                        $($txtSearch[1]).css("display", "none");
                        $($txtSearch[0]).val("");
                        var query = new VIS.Query();
                        query.addRestriction(" 1 = 1 ");
                        self.findRecords(query);
                    }
                }
            });
        }

        $($txtSearch[2]).on("click", function () {

            //if($($txtSearch[0]).autocomplete('widget')[0].style.display === 'none') {

            if (!self.isAutoCompleteOpen) {
                //$(this).autocomplete('search','');
                $($txtSearch[2]).css("transform", "rotate(180deg)");
                self.refreshSavedASearchList(true);
            }
            else {
                $($txtSearch[2]).css("transform", "rotate(360deg)");
            }
            //self.refreshSavedASearchList(true);
        });

        $($txtSearch[1]).on("click", function () {
            $($txtSearch[1]).css("display", "none");
            self.defaultSearch = true;
            self.curTab.searchText = "";
            $($txtSearch[0]).val("");
            var query = new VIS.Query();
            //query.addRestriction(" 1 = 1 ");
            self.findRecords(query);
            $($txtSearch[2]).css("transform", "rotate(360deg)");
        });

        this.setAdvancedSerachText = function (hideicon, text) {
            if (hideicon) {
                $($txtSearch[1]).css("display", "none");
            }
            else {
                $($txtSearch[1]).css("display", "inherit");
                $($txtSearch[2]).css("display", "inherit");
            }
            $($txtSearch[0]).val(text);

        };

        this.toggleASearchIcons = function (show, hasDefault) {
            if (show && hasDefault) {
                $($txtSearch[1]).css('display', 'inherit');
                $($txtSearch[2]).css('display', 'inherit');
            }
            else if (show && !hasDefault) {
                $($txtSearch[1]).css('display', 'none');
                $($txtSearch[2]).css('display', 'inherit');
            }
            else {
                $($txtSearch[1]).css('display', 'none');
                $($txtSearch[2]).css('display', 'none');
            }
        };

        this.setSearchFocus = function (focus) {
            if (focus) {
                $($txtSearch[0]).focus();
            }
            else {
                $($txtSearch[0]).trigger('focusout');
            }
        };

        this.refreshSavedASearchList = function (showData, text) {
            //            var sqlUserSearch = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END  AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
            //+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=" + self.curTab.getAD_Tab_ID() + " AND AD_DefaultUserQuery.AD_User_ID=" + self.ctx.getAD_User_ID() + "  )  "
            //+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=" + self.curTab.getAD_Tab_ID() + " AND AD_DefaultUserQuery.AD_User_ID=" + self.ctx.getAD_User_ID() + "   )   ELSE null End as AD_DefaultUserQuery_ID"
            //       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =" + self.ctx.getAD_Client_ID() + " AND AD_UserQuery.IsActive             ='Y' "
            //       + " AND (AD_UserQuery.AD_Tab_ID           =" + self.curTab.getAD_Tab_ID() + " OR AD_UserQuery.AD_Table_ID           =" + self.curTab.getAD_Table_ID() + ")"
            //                   + " ORDER BY Upper(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";

            var sqlUserSearch = "VIS_116";

            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Tab_ID", self.curTab.getAD_Tab_ID());
            param[1] = new VIS.DB.SqlParam("@AD_User_ID", self.ctx.getAD_User_ID());
            param[2] = new VIS.DB.SqlParam("@AD_Tab_ID1", self.curTab.getAD_Tab_ID());
            param[3] = new VIS.DB.SqlParam("@AD_User_ID1", self.ctx.getAD_User_ID());
            param[4] = new VIS.DB.SqlParam("@AD_Client_ID", self.ctx.getAD_Client_ID());
            param[5] = new VIS.DB.SqlParam("@AD_Tab_ID2", self.curTab.getAD_Tab_ID());
            param[6] = new VIS.DB.SqlParam("@AD_Table_ID", self.curTab.getAD_Table_ID());

            var $selfpanel = this;
            executeDataSet(sqlUserSearch, param, function (data) {
                var userQueries = [];


                if (data && data.tables[0].rows && data.tables[0].rows.length > 0) {

                    //$($txtSearch[1]).css("display", "inherit");
                    $($txtSearch[2]).css('display', 'inherit');
                    userQueries.push({ 'label': VIS.Msg.getMsg("All"), 'value': VIS.Msg.getMsg("All"), 'code': VIS.Msg.getMsg("All") });
                    var hasDefaultSearch = false;
                    for (var i = 0; i < data.tables[0].rows.length; i++) {

                        if (data.tables[0].rows[i].cells["ad_defaultuserquery_id"] > 0) {
                            userQueries.push({ 'title': data.tables[0].rows[i].cells["title"], 'label': data.tables[0].rows[i].cells["name"], 'value': data.tables[0].rows[i].cells["name"], 'code': data.tables[0].rows[i].cells["code"], 'id': data.tables[0].rows[i].cells["ad_userquery_id"], 'defaultids': data.tables[0].rows[i].cells["ad_defaultuserquery_id"], 'userid': data.tables[0].rows[i].cells["ad_defaultuserquery_id"] });
                            hasDefaultSearch = true;
                        }
                        else {
                            userQueries.push({ 'title': data.tables[0].rows[i].cells["title"], 'label': data.tables[0].rows[i].cells["name"], 'value': data.tables[0].rows[i].cells["name"], 'code': data.tables[0].rows[i].cells["code"], 'id': data.tables[0].rows[i].cells["ad_userquery_id"] });
                        }
                    }
                    //$selfpanel.toggleASearchIcons(true, hasDefaultSearch);
                }
                else {
                    $selfpanel.toggleASearchIcons(false, false);
                }

                if (!text) {
                    text = $($txtSearch[0]).val();
                }

                if (text && text.length > 0) {

                    if (text.length > 25) {
                        $($txtSearch[0]).val(text.substr(0, 25) + '...');
                    }
                    else {
                        $($txtSearch[0]).val(text);
                    }
                    $($txtSearch[1]).css('display', 'inherit');
                    $selfpanel.defaultSearch = false;
                }
                else {
                    $($txtSearch[1]).css('display', 'none');
                }

                $($txtSearch[0]).autocomplete('option', 'source', userQueries);
                if (showData) {
                    $($txtSearch[0]).autocomplete("search", "");
                    $($txtSearch[0]).trigger("focus");
                }
            })
        };

        this.findRecords = function (query) {

            var onlyCurrentDays = 0;
            var created = false;

            this.curTab.getTableModel().setCurrentPage(1);

            if (this.isSummaryVisible) {
                this.curTab.setShowSummaryNodes(true);
            }
            else {
                this.curTab.setShowSummaryNodes(false);
            }

            this.curGC.skipRowInserting(true); // do - not insert row 
            if (query != null && query.getIsActive()) {
                //log.config(query.toString());
                this.curTab.setQuery(query);
                this.curGC.query(0, 0, created);   //  autoSize
                //, this.curGC.treeNodeID, this.treeID
            }
            else {
                //var maxRows = VIS.MRole.getMaxQueryRecords();
                var maxRows = 0;
                //log.config("OnlyCurrentDays=" + onlyCurrentDays + ", MaxRows=" + maxRows);
                this.curTab.setQuery(null);	//	reset previous queries
                this.curGC.query(onlyCurrentDays, maxRows, created);   //  autoSize
            }
        };


        //End

        /* left bar toggle */
        // $btnlbToggle.on(VIS.Events.onTouchStartOrClick, function (e) {
        $btnlbToggle.on(VIS.Events.onTouchStartOrClick, function (e) {
            e.stopPropagation();
            e.preventDefault();
            var w = $td0leftbar.width();

            if (w > 50) {
                $ulLefttoolbar.find('span').hide();
            }

            $td0leftbar.animate({
                "width": w > 50 ? 40 : 200
            }, 300, 'swing', function () {

                if (w < 50) {
                    $ulLefttoolbar.find('span').show();
                }
                if (self.curGC) {
                    self.curGC.multiRowResize();
                }
                self.setWidth(-1, true);
                self.setTabNavigation();
            });
        });

        /*Right Bar */
        $rightBarLPart.on("click", function (e) {
            e.stopPropagation();
            var rtl = VIS.Application.isRTL;
            if ($rightBar.width() > 100) {
                $rightBar.animate({
                    "left": rtl ? 0 : $root.width() - 8 + "px",
                    "width": "8px"
                }, 500, "swing", function () {
                    //$rightBar.css("position", "static");
                    $rightBarRPart.css({ "width": "0px" });
                });
                $rightBarRPart.animate({
                    "width": "0px"
                }, 500, "swing");
                if (self.curTab.getHasPanel()) {
                    $($rigthBarAction.find('img')).css({ 'transform': 'rotate(360deg)', 'display': 'none' });
                    $td4.css('display', 'none');
                }
                else {
                    $($rigthBarAction.find('img')).css({ 'transform': 'rotate(360deg)' });
                    $td4.css('display', 'none');
                }
            }
            else {
                $rightBar.css({
                    "position": "absolute",
                    // "height": $rightBar.height(),
                    "width": "8px",
                    "left": rtl ? 0 : $root.width() - 8
                });

                $rightBar.animate({
                    "left": rtl ? 0 : $root.width() - 220 + "px",
                    "width": "220px"
                }, 500, "swing");

                $rightBarRPart.animate({
                    "width": "212px"
                }, 500, "swing");
                if (self.curTab.getHasPanel()) {
                    $($rigthBarAction.find('img')).css({ 'transform': 'rotate(180deg)', 'display': '' });
                    $td4.css('display', '');
                }
                else {
                    $($rigthBarAction.find('img')).css({ 'transform': 'rotate(180deg)' });
                    $td4.css('display', '');
                }
            }
        });

        this.disposeComponent = function () {

            //Search
            $imgSearch.off(VIS.Events.onTouchStartOrClick);
            $txtSearch.remove();
            $txtSearch = null;

            //left bar
            $btnlbToggle.off(VIS.onTouchStartOrClick);
            $divlbNav.off("click");
            $divTabNav.off("click");

            $root.remove();
            $busyDiv.remove();
            $table.remove();
            $root = $busyDiv = $table = $divNav = $tdContentArea = $ulNav = $divSearch = $divToolbar = $ulToobar = $divStatus = null;
            self = null;
            onAction = null;
            //
            if (this.toolbarCreated) {
                this.aRefresh.dispose();
                this.aDelete.dispose();
                this.aNew.dispose();
                this.aSave.dispose();
                this.aPrevious.dispose();
                this.aFirst.dispose();
                this.aLast.dispose();
                this.aNext.dispose();

                this.aPageUp.dispose();
                this.aPageFirst.dispose();
                this.aPageLast.dispose();
                this.aPageDown.dispose();
                this.aCard.dispose();
                this.aCardDialog.dispose();

                if (this.aShowSummaryLevel) {
                    this.aShowSummaryLevel.dispose();
                }


                if (this.aChat) {
                    this.aChat.dispose();
                }
                if (this.aAppointment) {
                    this.aAppointment.dispose();
                }

                this.aHelp.dispose();
                if (this.aSubscribe) {
                    this.aSubscribe.dispose();
                }
                if (this.aAttachment) {
                    this.aAttachment.dispose();
                }
                if (this.aHistory) {
                    this.aHistory.dispose();
                }
                if (this.aZoomAcross) {
                    this.aZoomAcross.dispose();
                }
                if (this.aRequest) {
                    this.aRequest.dispose();
                }
                if (this.aMarkToExport) {
                    this.aMarkToExport.dispose();
                }
                if (this.aWorkflow) {
                    this.aWorkflow.dispose();
                }
                if (this.aRecAccess) {
                    this.aRecAccess.dispose();
                }
                if (this.aImportMap) {
                    this.aImportMap.dispose();
                }

                this.aRefresh = this.aDelete = this.aNew = this.aPrevious = this.aFirst = this.aLast = this.aNext = null;
                this.aChat = this.aPageUp = this.aPageFirst = this.aPageLast = this.aPageDown = null;
                this.aHelp = this.aSubscribe = this.aAttachment = null, this.toolbarCreated = null;
                this.aZoomAcross = this.aRequest = this.aMark = this.aWorkflow = this.aHistory = null;
                this.aAppointment = null; this.aRecAccess = this.aImportMap = this.aCard = this.aCardDialog = this.aShowSummaryLevel = null;
            }

            this.statusBar.dispose();
            this.statusBar.onComboChange = null;
            this.statusBar = null;

            this.getRoot = null;
            this.getLayout = null;
            this.setBusy = null;
            this.createToolBar = null;

            this.$parentWindow = null;
            this.ctx = null;
            //this.tabPages = {};
            this.curGC = null;
            this.curST = null;
            this.curTab = null;
            this.vTabbedPane = null;
            /* current Tab panel */
            this.curWinTab = null;
            /* Tab Index */
            this.curTabIndex = null;
            /* Sort Tab */
            this.firstTabId = null;

            //Right bar
            $rightBarLPart.off("click");
            $rightBarLPart = null;
            $rightBarRPart = null;

            $td1parentDetail.remove();
            $td1parentDetail = null;
            this.getParentDetailPane = null;
            if (tabItems) {
                for (var i = 0; i < tabItems.length; i++) {
                    tabItems[i].dispose();
                }
            }
            tabItems = null;
            tabLIObj = null;
            $ulTabControl.remove();
        };
    };
    /** Shared action names*/
    APanel.prototype.ACTION_NAME_FIRST = "First";
    APanel.prototype.ACTION_NAME_LAST = "Last";
    APanel.prototype.ACTION_NAME_PREV = "Previous";
    APanel.prototype.ACTION_NAME_NEXT = "Next";

    APanel.prototype.ACTION_NAME_NEW = "New";
    APanel.prototype.ACTION_NAME_DELETE = "Delete";
    APanel.prototype.ACTION_NAME_REFRESH = "Refresh";
    APanel.prototype.ACTION_NAME_FIND = "Find";
    APanel.prototype.ACTION_NAME_CHAT = "Chat";
    APanel.prototype.ACTION_NAME_APPOINTMENT = "Appointment";
    APanel.prototype.ACTION_NAME_ARCHIVE = "Archive";

    APanel.prototype.keyDown = function (evt) {
        if (evt.altKey && this.curGC) {
            var en = this.aNew.getIsEnabled();
            switch (evt.keyCode) {
                case 78:      //N for ADD
                    if (en)
                        this.actionPerformed(this.aNew.getAction());
                    break;
                case 83:      //S for save
                    //if (!en) {
                    //this.setSearchFocus(true);
                    //var selfPanel = this;
                    //window.setTimeout(function () {
                    //selfPanel.actionPerformed(selfPanel.aSave.getAction());
                    this.ShortcutNavigation(this.aSave.getAction());
                    //selfPanel.setSearchFocus(false);
                    //}, 100);
                    //}
                    break;
                case 68:      // D for Delete
                    if (en)
                        this.actionPerformed(this.aDelete.getAction());
                    break;
                case 90:      // Z for undo
                    if (!en)
                        this.actionPerformed(this.aIgnore.getAction());
                    break;
                case 80:      // P for print
                    if (en)
                        this.actionPerformed(this.aPrint.getAction());
                    break;
                case 82:      // R for GridReport
                    if (en)
                        this.actionPerformed(this.aReport.getAction());
                    break;
                case 81:      // Q for Refresh
                    if (en)
                        this.actionPerformed(this.aRefresh.getAction());
                    break;
                case 65:      // A for Advanced Search
                    this.actionPerformed(this.aFind.getAction());
                    break;
                case 70:      // F for Find
                    this.setSearchFocus(true);
                    break;
                case 37:      // Left Arrow for First Record
                    // this.actionPerformed(this.aFirst.getAction());
                    this.ShortcutNavigation(this.aFirst.getAction());
                    break;
                case 39:      // Right Arrow for Last record
                    //this.actionPerformed(this.aLast.getAction());
                    this.ShortcutNavigation(this.aLast.getAction());
                    break;
                case 38:      // ArrowUP for preivious Record
                    //var fEle = $(document.activeElement);
                    //if (fEle && fEle.length > 0)
                    //    fEle.trigger("change");
                    //this.actionPerformed(tis.aPrevious.getAction());
                    this.ShortcutNavigation(this.aPrevious.getAction());
                    break;
                case 40:      // Arrow Down for next record
                    //this.actionPerformed(this.aNext.getAction());
                    this.ShortcutNavigation(this.aNext.getAction());
                    break;
                case 84:      // Arrow Down for next record
                    this.actionPerformed(this.aMulti.getAction());
                    break;
                case 86:      // Arrow Down for next record
                    this.actionPerformed(this.aCard.getAction());
                    break;
                case 88:      // X for close
                    this.$parentWindow.dispose();
                    break;
                case 33:
                    if (evt.ctrlKey) {
                        if (this.aPageFirst.isEnabled) {
                            //this.actionPerformed(this.aPageFirst.getAction());
                            this.ShortcutNavigation(this.aPageFirst.getAction());
                        }
                        break;
                    }
                    else {
                        if (this.aPageUp.isEnabled) // move to previous Page
                            //this.actionPerformed(this.aPageUp.getAction());
                            this.ShortcutNavigation(this.aPageUp.getAction());
                        break;
                    }
                case 34:
                    if (evt.ctrlKey) {
                        if (this.aPageLast.isEnabled) { // move to Last Page
                            //this.actionPerformed(this.aPageLast.getAction());
                            this.ShortcutNavigation(this.aPageLast.getAction());
                        }
                        break;
                    }
                    else {
                        if (this.aPageDown.isEnabled) { // move to Next Page
                            //this.actionPerformed(this.aPageDown.getAction());
                            this.ShortcutNavigation(this.aPageDown.getAction());
                        }
                        break;
                    }
            }
            evt.preventDefault();
            evt.stopPropagation();
        }
        else if (evt.keyCode == 112) {
            this.actionPerformed(this.aHelp.getAction());
            evt.preventDefault();
            evt.stopPropagation();
        }
    }

    APanel.prototype.ShortcutNavigation = function (action) {
        var tis = this;
        var fEle = $(document.activeElement);
        if (fEle && fEle.length > 0)
            fEle.trigger("change");
        window.setTimeout(function () {
            tis.actionPerformed(action);
        }, 200);

    };

    APanel.prototype.sizeChanged = function (height, width) {
        this.setSize(height, width);
        if (this.vTabbedPane)
            this.vTabbedPane.sizeChanged(height - (APANEL_HEADER_HEIGHT + APANEL_FOOTER_HEIGHT), width);
    };

    APanel.prototype.refresh = function () {
        if (this.curGC) {
            this.curGC.vTable.resize();
            if (this.curGC.vIncludedGC) {
                this.curGC.vIncludedGC.vTable.refresh();
            }
        }

    };

    APanel.prototype.refreshData = function () {
        var ssel = this;
        window.setTimeout(function () {
            if (ssel.curTab.getAD_Tab_ID() == ssel.firstTabId.split('_')[1]) {
                ssel.curGC.dataRefreshAll();
            }
            else {

                ssel.selectFirstTab(false, function () {
                    ssel.curGC.dataRefreshAll();
                });
            }
            ssel.setBusy(false);
        }, 100);
        this.setBusy(true);
    };

    APanel.prototype.addActions = function (action, parent, disableIcon, imageOnly, isSmall, onAction, toggle, toolTipText) {
        var action = new VIS.AppsAction({ action: action, parent: parent, enableDisable: disableIcon, toggle: toggle, imageOnly: imageOnly, isSmall: isSmall, onAction: onAction, toolTipText: toolTipText }); //Create Apps Action
        return action;
    };

    /**************************************************************************
     *	Dynamic Panel Initialization - either single window or workbench.
     *  <pre>
     *  - Workbench tabPanel    (VTabbedPane)
     *      - Tab               (GridController)
       - Tab           (GridController)
     *  </pre>
     *  tabPanel
     *  @param jsonData  window properties
     *  @param query			if not a Workbench, Zoom Query - additional SQL where clause
     *  @return true if Panel is initialized successfully
     */

    APanel.prototype.initPanel = function (jsonData, query, $parent, goSingleRow, sel) {

        this.$parentWindow = $parent;
        var gridWindow = new VIS.GridWindow(jsonData);
        this.gridWindow = gridWindow; //ref to call dispose
        //this.setWidth(gridWindow.getWindowWidth());
        this.createToolBar(); // Init ToolBar

        var curWindowNo = $parent.getWindowNo();
        var autoNew = this.ctx.isAutoNew();

        var tabs = gridWindow.getTabs(); //Tabs VO

        this.ctx.setAutoCommit(curWindowNo, this.ctx.isAutoCommit());
        this.ctx.setAutoNew(curWindowNo, autoNew);

        //	Set SO/AutoNew for Window
        this.ctx.setIsSOTrx(curWindowNo, gridWindow.getIsSOTrx());
        if (!autoNew && gridWindow.getIsTransaction())
            this.ctx.setAutoNew(curWindowNo, true);

        this.ctx.setContext(curWindowNo, "WindowName", gridWindow.getName());


        /* Select Record */
        if (!query && sel) {
            query = new VIS.Query();
            //	Current row
            var link = tabs[0].getKeyColumnName();
            //	Link for detail records
            if (link.length == 0)
                link = tabs[0].getLinkColumnName();
            if (link.length != 0) {
                if (link.endsWith('_ID'))
                    query.addRestriction(link, VIS.Query.prototype.EQUAL, sel);
                else
                    query.addRestriction(link, VIS.Query.prototype.EQUAL, sel);
            }
        }

        var gTab;
        var tabActions = []; //Tabs Apps Action
        //var firstTabId = null;

        var includedMap = {};

        if (gridWindow.getHasPanel()) {
            var panelwidth = gridWindow.getWindowWidth();
            if (panelwidth && panelwidth > 0 && panelwidth < 75) {
                this.setWidth(panelwidth, true);
            }
            else {
                this.setWidth(75, true);
            }
        }


        for (var i = 0; i < tabs.length; i++) {

            var id = curWindowNo + "_" + tabs[i].getAD_Tab_ID(); //uniqueID
            tabActions[i] = new VIS.AppsAction({ action: id, text: tabs[i].getName(), toolTipText: tabs[i].getDescription, textOnly: true }); //Create Apps Action

            gTab = tabs[i];
            if (i === 0) {
                this.curTab = gTab;
                if (query != null) {

                    gTab.setQuery(query);
                }
            }	//	query on first tab

            var tabElement = null;
            //        //  GridController
            if (gTab.getIsSortTab())//     .IsSortTab())
            {
                //var st = new VIS.VSortTab(curWindowNo, id);
                var st = new VIS.VSortTab(curWindowNo, gTab.getAD_Table_ID(),
                    gTab.getAD_ColumnSortOrder_ID(), gTab.getAD_ColumnSortYesNo_ID(), gTab.getIsReadOnly(), id);
                //st.setTabLevel(gTab.getTabLevel());
                tabElement = st;
                if (i == 0) {
                    firstTabId = id;
                }
            }
            else	//	normal tab
            {
                var gc = new VIS.GridController(true, true, id);
                gc.initGrid(false, curWindowNo, this, gTab);
                //            gc.addDataStatusListener(this);

                //Set Title of Tab
                if (i === 0) {
                    this.curGC = gc;
                    this.firstTabId = id;
                }

                tabElement = gc;
                //	If we have a zoom query, switch to single row
                if (i === 0 && goSingleRow)
                    gc.switchSingleRow();

                // For first tab, if panel avilable, then set width for window

                //if (gTab.getIncluded_Tab_ID() == 0) {

                //}
                //else {
                //    this.setIncludedTabWidth(true);
                //}
                //var panelwidth = gridWindow.getWindowWidth();
                //if (panelwidth && panelwidth > 0 && panelwidth < 75) {
                //    this.setWidth(panelwidth, true);
                //}
                //else {
                //    this.setWidth(75, true);
                //}

                // END Tab Panel

                //	Store GC if it has a included Tab
                if (gTab.getIncluded_Tab_ID() != 0) {

                    includedMap[gTab.getIncluded_Tab_ID()] = gc;
                    if (i == 0)
                        this.aParentDetail = new VIS.AParentDetail(gc, this.getParentDetailPane());
                }

                //	Is this tab included?
                if (!$.isEmptyObject(includedMap)) {
                    var parent = includedMap[gTab.getAD_Tab_ID()];
                    if (parent != null) {
                        var included = parent.includeTab(gc);
                        //if (!included)
                        //  log.log(Level.SEVERE, "Not Included = " + gc);
                    }
                }
            }	//	normal tab

            this.vTabbedPane.addTab(id, gTab, tabElement, tabActions[i]);
            if (tabElement) {
                this.getLayout().append(tabElement.getRoot());
            }
            //TabChange Action Callback
            tabActions[i].onAction = this.onTabChange; //Perform tab Change
        }

        // for (var item = 0 ; item < this.vTabbedPane.Items.length ; item++) {
        // this.vTabbedPane.Items[item].setTabControl(tabActions); //Set TabPage 
        // }
        this.setTabControl(tabActions);

        tabActions = null;



        this.ctx.setWindowContext(curWindowNo, "WindowName", jsonData._vo.DisplayName);
        $parent.setTitle(VIS.Env.getHeader(this.ctx, curWindowNo));
        $parent.setName(jsonData._vo.DisplayName);
        this.curWindowNo = curWindowNo;
        jsonData = null;
        $parent = null;
        // this.curGC.setVisible(true);
    };

    /**
    *  Activate first tab 
    */
    //APanel.prototype.selectFirstTab = function () {
    //    setTimeout(function (that) {
    //        that.tabActionPerformed(that.firstTabId);
    //        that.setTabNavigation();
    //        //that.setBusy(false, false);
    //        that = null;
    //    }, 10, this);

    //};

    //Updated by raghu 
    //date:19-01-2016
    //Change/Update for:Zoom from workflow on home page
    APanel.prototype.selectFirstTab = function (isSelect) {
        this.curGC.isZoomAction = isSelect;
        this.curTab.setIsZoomAction(isSelect);
        setTimeout(function (that) {
            that.curGC.isZoomAction = isSelect;
            that.tabActionPerformed(that.firstTabId);
            that.setTabNavigation();
            that = null;
        }, 10, this);
    };

    /**
     *  Is the UI locked (Internal method)
     *  @return true, if UI is locked
     */
    APanel.prototype.getIsUILocked = function () {
        return this.isLocked;
    }   //  isLoacked

    /**
     *  Lock User Interface.
     *  Called from the Worker before processing
     *  @param pi process info
     */
    APanel.prototype.lockUI = function (pi) {
        //	log.fine("" + pi);
        this.setBusy(true, false);
    };  //  lockUI

    /**
     *  Unlock User Interface.
     *  Called from the Worker when processing is done
     *  @param pi of execute ASync call
     */
    APanel.prototype.unlockUI = function (pi) {
        //	log.fine("" + pi);
        var notPrint = pi != null
            && pi.getAD_Process_ID() != this.curTab.getAD_Process_ID();
        //  Process Result
        if (notPrint)		//	refresh if not print
        {
            //	Refresh data
            this.curTab.dataRefresh();
            //	Timeout
            if (pi.getIsTimeout())		//	set temporarily to R/O
                VIS.context.setWindowContext(this.curWindowNo, "Processed", "Y");
            this.curGC.dynamicDisplay(-1);
            //	Update Status Line
            this.setStatusLine(pi.getSummary(), pi.getIsError());
            // Change Lokesh Chauhan
            if (pi.customHTML && pi.customHTML != "") {
                this.displayDialog($(pi.customHTML));
            }
            else {
                //	Get Log Info
                VIS.ProcessInfoUtil.setLogFromDB(pi);
                var logInfo = pi.getLogInfo();
                if (logInfo.length > 0) {
                    VIS.ADialog.info(pi.getTitle(), true, logInfo, "");
                    this.setStatusLine(pi.getSummary(), pi.getIsError());
                }
                //ADialog.info(m_curWindowNo, this, Env.getHeader(m_ctx, m_curWindowNo),
                //      pi.getTitle(), logInfo);	//	 clear text
            }
        }
        this.setBusy(false, notPrint);
    };  //  unlockUI

    // Change Lokesh Chauhan
    APanel.prototype.displayDialog = function (message) {
        var chDia = new VIS.ChildDialog();
        chDia.setTitle("");
        var wdth = window.innerWidth - 150;
        var hgt = window.innerHeight - 250;
        var diaCtr = $('<div style="max-height: ' + hgt + 'px; max-width: ' + wdth + 'px; min-width: 150px; min-height: 60px;"></div>');
        diaCtr.append(message);
        chDia.setContent(diaCtr);
        chDia.close = function () {
            chDia.dispose();
        }
        chDia.show();
        chDia.hidebuttons();
    };

    /**
     *	Action Listener
     *  @param action string or object
     */
    APanel.prototype.actionPerformed = function (action) {

        if (this.getIsUILocked())
            return;
        //	Do Screenrt w/o busy

        var selfPan = this;
        setTimeout(function () {
            //  Command Buttons

            if (action.source instanceof VIS.Controls.VButton) {
                if (!selfPan.actionButton(action.source)) {
                    selfPan.setBusy(false, true);
                }
                return;
            }

            selfPan.actionPerformedCallback(selfPan, action);

        });

        this.setBusy(true);


    };

    APanel.prototype.actionPerformedCallback = function (tis, action) {

        /*Naviagtion */
        if (tis.aFirst.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigate(0)
        } else if (tis.aPrevious.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigateRelative(-1);
        } else if (tis.aLast.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigate(tis.curTab.getRowCount() - 1);
        } else if (tis.aNext.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigateRelative(+1);
        } else if (tis.aMulti.getAction() === action) {
            tis.aMulti.setPressed(!tis.curGC.getIsSingleRow());
            tis.curGC.switchRowPresentation();
        } else if (tis.aCard.getAction() === action) {
            tis.aMulti.setPressed(false);
            tis.curGC.switchCardRow();
        } else if (tis.aMap.getAction() === action) {
            tis.aMulti.setPressed(true);
            tis.curGC.switchMapRow();
        } else if (tis.aPageUp.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigatePage(-1);
        } else if (tis.aPageFirst.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigatePage(0);
        } else if (tis.aPageDown.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigatePage(1);
        } else if (tis.aPageLast.getAction() === action) {
            tis.isDefaultFocusSet = false;
            tis.curGC.navigatePage('last');
        }
        /*MainToolBar */
        else if (tis.aRefresh.getAction() === action) {
            tis.cmd_refresh();
        }
        else if (tis.aIgnore.getAction() === action) {
            tis.cmd_ignore();
        }
        else if (tis.aSave.getAction() === action) {
            tis.cmd_save(true);
        }
        else if (tis.aNew.getAction() === action) {
            tis.cmd_new(false);
        }
        else if (tis.aCopy && tis.aCopy.getAction() === action) {
            tis.cmd_new(true);
        }
        else if (tis.aDelete.getAction() === action) {
            tis.cmd_delete();
        }
        else if (tis.aFind.getAction() === action) {
            tis.cmd_finddialog();
        }
        else if (tis.aChat && tis.aChat.getAction() === action) {
            tis.cmd_chat();
        }
        else if (tis.aAttachment && tis.aAttachment.getAction() === action) {
            tis.cmd_attachment();
        }
        else if (tis.aHistory && tis.aHistory.getAction() == action) {
            tis.cmd_history();
        }
        else if (tis.aPreference.getAction() === action) {
            tis.cmd_preference();
        }
        else if (tis.aHelp.getAction() === action) {
            tis.cmd_help();
        }

        else if (tis.aCardDialog.getAction() === action) {
            tis.cmd_cardDialog();
        }

        else if (tis.aAppointment && tis.aAppointment.getAction() === action) {
            tis.cmd_appointment();
        }
        else if (tis.aTask && tis.aTask.getAction() === action) {
            tis.cmd_task();
        }

        else if (tis.aSubscribe && tis.aSubscribe.getAction() === action) {
            tis.cmd_subscribe();
        }
        else if (tis.aImportMap && tis.aImportMap.getAction() === action) {
            tis.cmd_ImportMap();
        }
        else if (tis.aEmail && tis.aEmail.getAction() === action) {
            tis.cmd_email();
        }
        else if (tis.aLetter && tis.aLetter.getAction() === action) {
            tis.cmd_letter();
        }
        else if (tis.aSms && tis.aSms.getAction() === action) {
            tis.cmd_sms();
        }
        //lakhwinder
        else if (tis.aInfo.getAction() === action) {
            tis.cmd_infoWindow();

        }
        else if (tis.aZoomAcross && tis.aZoomAcross.getAction() === action) {
            tis.cmd_zoomAcross();
        }
        else if (tis.aRequest && tis.aRequest.getAction() === action) {
            tis.cmd_request();
        }
        else if (tis.aReport.getAction() === action) {
            tis.cmd_report();
        }

        else if (tis.isPersonalLock && tis.aLock.getAction() === action)
            tis.cmd_lock();

        else if (tis.isPersonalLock && tis.aRecAccess.getAction() === action) {
            tis.cmd_recAccess();
        }

        //	Tools
        else if (tis.aWorkflow != null && action === (tis.aWorkflow.getAction())) {

            if (tis.curTab.getRecord_ID() > 0) {
                VIS.AEnv.startWorkflowProcess(tis.curTab.getAD_Table_ID(), tis.curTab.getRecord_ID());
            }


        }

        else if (tis.aPrint.getAction() === action) {
            tis.cmd_print();
        }
        else if (tis.aCreateDocument && tis.aCreateDocument.getAction() === action) {
            if (window.VADMS) {
                var frame = new VIS.CFrame();
                var editDoc = new window.VADMS.editDocument(0, "", 0, "", 0, null, "", tis.curTab.getAD_Window_ID(), tis.curTab.getAD_Table_ID(), tis.curTab.getRecord_ID());
                frame.setName(VIS.Msg.getMsg("VADMS_CreateDocument"));
                frame.setTitle(VIS.Msg.getMsg("VADMS_CreateDocument"));
                frame.hideHeader(true);
                frame.setContent(editDoc);
                editDoc.initialize();
                frame.show();
            }
            else {
                VIS.ADialog.error('PleaseInstallDMSModule', true, "");
            }

        }
        else if (tis.aUploadDocument && tis.aUploadDocument.getAction() === action) {

            if (window.VADMS) {
                // New parameter add for upload document dialoag to sent current window name and tab name
                window.VADMS.uploaddocument(0, tis.curTab.getAD_Window_ID(), tis.curTab.getAD_Table_ID(), tis.curTab.getRecord_ID(), tis.$parentWindow.name, tis.curTab.getName());

            }
            else {
                VIS.ADialog.error('PleaseInstallDMSModule', true, "");
            }

        }
        else if (tis.aViewDocument && tis.aViewDocument.getAction() === action) {
            if (window.VADMS) {
                var frame = new VIS.CFrame();
                var doc = new window.VADMS.DocumentManagementSystem();
                frame.setName(VIS.Msg.getMsg("VADMS_Document"));
                frame.setTitle(VIS.Msg.getMsg("VADMS_Document"));
                frame.hideHeader(true);
                doc.setWindowNo(VIS.Env.getWindowNo());
                doc.setWindowID(tis.curTab.getAD_Window_ID());
                doc.setTableID(tis.curTab.getAD_Table_ID());
                doc.setRecordID(tis.curTab.getRecord_ID());
                doc.setWindowName(tis.gridWindow.getName());
                frame.setContent(doc);
                doc.initialize();
                frame.show();

            }
            else {
                VIS.ADialog.error('PleaseInstallDMSModule', true, "");
            }
        }
        else if (tis.aAttachFrom && tis.aAttachFrom.getAction() === action) {
            if (window.VADMS) {
                var self = tis;
                var documentID = VIS.context.getContext("VADMS_Document_ID");
                if (documentID.length > 0) {
                    var dataIn = {
                        docID: documentID,
                        winID: tis.curTab.getAD_Window_ID(),
                        tableID: tis.curTab.getAD_Table_ID(),
                        recID: tis.curTab.getRecord_ID()
                    };
                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/AttachFrom",
                        dataType: "json",
                        data: dataIn,
                        success: function (data) {
                            if (JSON.parse(data) == "OK") {
                                self.curTab.loadDocuments();
                                self.aViewDocument.setPressed(self.curTab.hasDocument());
                                //if (!VIS.ADialog.ask("AttachWithOther")) {
                                //    VIS.context.setContext("VADMS_Document_ID", 0);
                                //}


                                VIS.ADialog.confirm("AttachWithOther", true, "", "Confirm", function (result) {
                                    if (!result) {
                                        VIS.context.setContext("VADMS_Document_ID", 0);
                                    }
                                });


                                self = null;
                            }
                            else {
                                VIS.ADialog.error('NotAttached', true, "");
                            }
                        }
                    });
                }
            }
            else {
                VIS.ADialog.error('PleaseInstallDMSModule', true, "");
            }
        }
        else if (tis.aMarkToExport && tis.aMarkToExport.getAction() === action) {

            tis.cmd_markToExport();
        }
        else if (tis.aShowSummaryLevel.getAction() == action) {
            tis.ShowSummaryNodes();
        }

        //else if (tis.aCall && tis.aCall.getAction() === action) {
        //    tis.cmd_call();
        //}

        tis.setBusy(false);
        tis = null;
    };

    /**
     *	Start Button Process
     *  @param vButton button
     *  @retrun true to hide busy indicator
     */
    APanel.prototype.actionButton = function (vButton) {
        var startWOasking = false;
        var batch = false;
        var dateScheduledStart = null;
        var columnName = vButton.getColumnName();
        var ctx = VIS.context;
        var self = this;


        //  Zoom Button
        if (columnName.equals("Record_ID")) {
            var AD_Table_ID = ctx.getContextAsInt(this.curWindowNo, "AD_Table_ID");
            var Record_ID = ctx.getContextAsInt(this.curWindowNo, "Record_ID");
            VIS.AEnv.zoom(AD_Table_ID, Record_ID);
            return;
        }   //  Zoom

        //  save first	---------------

        var needExecute = true;

        if (this.curTab.needSave(true, false)) {
            needExecute = false;
            this.cmd_save(true, function (result) {
                if (result) {
                    self.actionButtonCallBack(vButton, startWOasking, batch, dateScheduledStart, columnName, ctx, self);
                }
            })
        }


        /**
         *  Start Process ----
         */
        if (needExecute) {
            return self.actionButtonCallBack(vButton, startWOasking, batch, dateScheduledStart, columnName, ctx, self);
        }


    };


    APanel.prototype.actionButtonCallBack = function (vButton, startWOasking, batch, dateScheduledStart, columnName, ctx, self) {
        var table_ID = this.curTab.getAD_Table_ID();
        //	Record_ID
        var record_ID = this.curTab.getRecord_ID();
        //	Record_ID - Language Handling
        if (record_ID == -1 && this.curTab.getKeyColumnName().equals("AD_Language"))
            record_ID = ctx.getContextAsInt(this.curWindowNo, "AD_Language_ID");
        //	Record_ID - Change Log ID
        if (record_ID == -1
            && (vButton.getProcess_ID() == 306 || vButton.getProcess_ID() == 307)) {
            var id = this.curTab.getValue("AD_ChangeLog_ID");
            record_ID = id;
        }
        //	Record_ID - EntityType
        if (record_ID == -1 && this.curTab.getKeyColumnName().equals("EntityType")) {
            record_ID = this.curTab.getValue("AD_EntityType_ID");
        }
        //	Ensure it's saved
        if (record_ID == -1 && this.curTab.getKeyColumnName().toUpperCase().endsWith("_ID")) {
            VIS.ADialog.error("SaveErrorRowNotFound", true, "");
            return;
        }

        //	Pop up Payment Rules
        if (columnName.equals("PaymentRule")) {
            var vp = new VIS.VPayment(this.curWindowNo, this.curTab, vButton);
            vp.show();
            vp.init();
            vp.onClose = function () {

                if (vp.isInitOK()) {
                    self.curGC.dynamicDisplay(vButton.getName());
                    self.cmd_save(false);
                    //if (vp.btnTextChange) {
                    //    SetRowState(true, false);
                    //    SetButtons(true, true);
                    //}
                    //if (vp.NeedSave()) {

                    //}
                    checkAndCallProcess(vButton, table_ID, record_ID, ctx, self, startWOasking, batch);
                }
            };
            return;

            //if (vp.isInitOK())		//	may not be allowed
            //    vp.setVisible(true);
            //vp.dispose();
            //if (vp.needSave())
            //{
            //    cmd_save(false);
            //    cmd_refresh();
            //}
        }	//	PaymentRule

        //	Pop up Document Action (Workflow)
        else if (columnName.equals("DocAction")) {
            var vda = new VIS.VDocAction(this.curWindowNo, this.curTab, record_ID);
            vda.show();
            vda.onClose = function () {

                //	Something to select from?
                if (vda.getNumberOfOptions() == 0) {
                    vda.dispose();
                    self.log.info("DocAction - No Options");
                    return;
                }
                else {
                    // vda.setVisible(true);
                    if (!vda.isStartProcess()) {
                        vda.dispose();
                        return;
                    }
                    batch = vda.isBatch();
                    //  dateScheduledStart = vda.getDateScheduledStart();
                    startWOasking = true;

                    checkAndCallProcess(vButton, table_ID, record_ID, ctx, self, startWOasking, batch);
                    vda.dispose();
                    self = null;
                }


            };
            return;

        }	//	DocAction

        //  Pop up Create From
        else if (columnName.equals("CreateFrom")) {
            //  m_curWindowNo
            // Change by Lokesh Chauhan 18/05/2015
            var chkModule = false;
            if (this.curTab.getAD_Window_ID() == 341 || this.curTab.getAD_Window_ID() == 170) {
                if (window.MMPM) {
                    var vvcf = MMPM.Requisition.prototype.create(this.curTab.getAD_Window_ID(), this.curTab.getRecord_ID());
                    chkModule = true;
                }
                else if (window.DTD001) {
                    var vvcf = DTD001.Requisition.prototype.create(this.curTab.getAD_Window_ID(), this.curTab.getRecord_ID());
                    chkModule = true;
                }
            }
            if (chkModule) {
                return;
            }
            var vcf = VIS.VCreateFrom.prototype.create(this.curTab);
            if (vcf != null) {
                if (vcf.isInitOK()) {
                    vcf.showDialog();
                    vcf.onClose = function (value) {
                        vcf.dispose();
                        this.curTab.dataRefresh();//DataRefreshRow
                    };
                    vcf = null;
                }
                else {
                    vcf.dispose();
                }
                return;
            }

            //	else may start process
        }	//	CreateFrom

        else if (columnName.equals("GenerateSticker")) {

            if (window.DTD001) {

                var vvcf = DTD001.StickerProduct.prototype.create(this.curTab.getAD_Window_ID(), this.curTab.getRecord_ID());
            }
            return;
        }
        else if (columnName.equals("DTD001_GenerateSticker")) {

            if (window.DTD001) {

                var vvcf = DTD001.MRProductSticker.create(this.curTab.getAD_Window_ID(), this.curTab.getRecord_ID(), this.curTab.getTabLevel());
            }
            return;
        }
        //Lakhwinder
        //requested by Mohit ,Mukesh Arora
        else if (columnName.equals("BGT01_CreateLinePo")) {
            if (window.BGT01) {
                BGT01.CreateLineMovement(this.curTab.getAD_Window_ID(), this.curTab.getAD_Tab_ID(), this.curTab.getRecord_ID());
            }
            return;
        }

        //  Posting -----

        else if (columnName == "Posted" && VIS.MRole.getDefault().getIsShowAcct()) {
            //  Check Doc Status
            var processed = VIS.context.getWindowContext(this.curWindowNo, "Processed");//
            if (processed != "Y") {
                var docStatus = VIS.context.getWindowContext(this.curWindowNo, "DocStatus");
                if (this.DocActionVariables.STATUS_COMPLETED == docStatus
                    || this.DocActionVariables.STATUS_CLOSED == docStatus
                    || this.DocActionVariables.STATUS_REVERSED == docStatus
                    || this.DocActionVariables.STATUS_VOIDED == docStatus)
                    ;
                else {
                    //ADialog.error(m_curWindowNo, this, "PostDocNotComplete");
                    VIS.ADialog.info(VIS.Msg.getMsg("PostDocNotComplete"));
                    return;
                }
            }

            //  Check Post Status
            var ps = this.curTab.getValue("Posted");
            if (ps != null && ps == "Y") {
                //get Current record orgID by window no
                var obj = new VIS.AcctViewer(VIS.context.getAD_Client_ID(), this.curTab.getAD_Table_ID(), this.curTab.getRecord_ID(), this.curWindowNo, this.curTab.getAD_Window_ID());
                if (obj != null) {
                    this.setBusy(false);
                    obj.showDialog();
                }
                obj = null;
            }
            else {
                //  if (VIS.ADialog.ask("PostImmediate?")) {
                VIS.ADialog.confirm("PostImmediate?", true, "", "Confirm", function (results) {
                    var selfLocal = self;
                    if (results) {

                        selfLocal.setBusy(true, true);

                        var force = ps != null && ps != "N";//	force when problems
                        //check for old and new posting logic
                        checkPostingByNewLogic(function (result) {
                            var postingByNewLogic = false;
                            if (result == "Yes") {
                                postingByNewLogic = true;
                            }
                            if (window.FRPT && postingByNewLogic) {
                                var orgID = Number(VIS.context.getWindowTabContext(selfLocal.curWindowNo, 0, "AD_Org_ID"));
                                var winID = selfLocal.curTab.getAD_Window_ID();
                                var docTypeID = Number(VIS.context.getWindowTabContext(selfLocal.curWindowNo, 0, "C_DocType_ID"));
                                var postObj = FRPT.PostingLogic(selfLocal.curWindowNo, selfLocal.curTab.getAD_Table_ID(), selfLocal.curTab.getRecord_ID(), force, orgID, winID, docTypeID, function () {
                                    selfLocal.curGC.dataRefresh();
                                    selfLocal.setBusy(false, true);
                                    return;
                                });
                            }
                            else {
                                $.ajax({
                                    url: VIS.Application.contextUrl + "Posting/PostImmediate",
                                    dataType: "json",
                                    data: {
                                        AD_Client_ID: VIS.context.getAD_Client_ID(),
                                        AD_Table_ID: selfLocal.curTab.getAD_Table_ID(),
                                        Record_ID: selfLocal.curTab.getRecord_ID(),
                                        force: force
                                    },
                                    error: function (e) {
                                        selfLocal.setBusy(false, true);
                                        VIS.ADialog.info('ERRORGettingPostingServer');
                                        //bsyDiv[0].style.visibility = "hidden";
                                    },
                                    success: function (data) {

                                        if (data.result != "OK") {
                                            selfLocal.setBusy(false, true);
                                            VIS.ADialog.info(data.result);
                                        }
                                        else {
                                            selfLocal.setBusy(false, true);
                                            selfLocal.curGC.dataRefresh();
                                            //refresh Row
                                        }

                                    }
                                });
                            }
                        });
                    }
                    else {
                        selfLocal.setBusy(false, true);
                        return false;
                    }
                });
                //}
                // else return false;
            }
            return false;
        }   //  Posted

        //	Send Email -----
        else if (columnName.equals("SendNewEMail")) {
            // AD_Process_ID = vButton.getProcess_ID();
            //if (AD_Process_ID != 0)
            //{
            //}
            ////	Mail Defaults
            //String title = getTitle();
            //String to = null;
            //Object oo = m_curTab.getValue("AD_User_ID");
            //if (oo instanceof Integer)
            //{
            //    MUser user = new MUser(Env.getCtx (), ((Integer)oo).intValue (), null);
            //    to = user.getEMail();
            //}
            //if (to == null)
            //    to = (String)m_curTab.getValue("EMail");
            //String subject = (String)m_curTab.getValue("Name");;
            //String message = "";
            //new EMailDialog (Env.getFrame(this), title,
            //		MUser.get(Env.getCtx()),
            //		to,	subject, message,
            //		null);
            return;
        }

        if (vButton.AD_Process_ID > 0) {

            var ret = checkAndCallProcess(vButton, table_ID, record_ID, ctx, self);
            self = null;
            return ret;
        }
        else if (vButton.AD_Form_ID > 0) {

            if (VIS.MRole.getFormAccess(vButton.AD_Form_ID)) {
                var wForm = new VIS.WForm(VIS.Env.getScreenHeight(), vButton.AD_Form_ID, this.curTab, this.curWindowNo);
            }
            else {
                VIS.ADialog.warn("AccessTableNoView");
            }
        }

    };

    function checkPostingByNewLogic(callback) {
        $.ajax({
            url: VIS.Application.contextUrl + "Posting/PostByNewLogic",
            dataType: "json",
            async: true,
            data: {
                AD_Client_ID: VIS.context.getAD_Client_ID()
            },
            error: function (e) {
                VIS.ADialog.info(VIS.Msg.getMsg('ERRORGettingPostingServer'));
            },
            success: function (data) {
                if (callback) {
                    callback(data.result);
                }
            }
        });
    }

    function checkAndCallProcess(vButton, table_ID, record_ID, ctx, aPanel, startWOasking, batch) {
        if (vButton.getProcess_ID() == 0)
            return;
        //	Save item changed

        var canExecute = true;

        if (aPanel.curTab.needSave(true, false)) {
            canExecute = false;
            aPanel.cmd_save(true, function (result) {
                if (!result)
                    return;
                else {
                    return btnClickAfterSave2(vButton, table_ID, record_ID, ctx, aPanel, startWOasking, batch);
                }
            });
        }
        if (canExecute) {
            return btnClickAfterSave2(vButton, table_ID, record_ID, ctx, aPanel, startWOasking, batch);
        }
    };

    function btnClickAfterSave2(vButton, table_ID, record_ID, ctx, aPanel, startWOasking, batch) {
        var columnName = vButton.getName();
        var ret = false;

        var needExecute = true;

        try {

            // If admin wants user to set background option
            // If background checkbox is checked, then user can see the setting throguh dialog but cannot change
            // if this checkbox is unchecked, then user will not be asked about setting, but process execute according to settings in DB.
            if (vButton.getAskUserBGProcess() == true || vButton.getIsBackgroundProcess() == true) {
                // Create Custom UI and pass root div as parameter to confirmCustomUI.
                var $customDIv = $('<div class="vis-confirm-popup-check"><label>' + VIS.Msg.translate(VIS.context, 'IsBackgroundProcess') + '</label></div>');
                var $chkBG = $('<input type="checkbox">');
                var isChecked = vButton.getIsBackgroundProcess();
                // Set (Disable or enable) and (checked or unchecked) based on DB setting
                $chkBG.prop('checked', isChecked);
                $chkBG.prop('disabled', isChecked);
                $customDIv.prepend($chkBG);
                VIS.ADialog.confirmCustomUI("StartProcess?", true, vButton.getDescription() + "\n" + vButton.getHelp(), "Confirm", $customDIv, function (result) {
                    if (result) {
                        isBg = $chkBG.is(':checked');
                        return btnClickAfterSave2New(vButton, table_ID, record_ID, ctx, batch, aPanel, ret, columnName, isBg);
                    }
                });


            }
            else {
                var isbg = vButton.getIsBackgroundProcess();
                //	Ask user to start process, if Description and Help is not empty
                if (!startWOasking && !(vButton.getDescription().equals("") && vButton.getHelp().equals(""))) {
                    needExecute = false;
                    VIS.ADialog.confirm("StartProcess?", true, vButton.getDescription() + "\n" + vButton.getHelp(), "Confirm", function (result) {
                        if (result) {
                            return btnClickAfterSave2New(vButton, table_ID, record_ID, ctx, batch, aPanel, ret, columnName, isbg);
                        }
                    });
                }

                if (needExecute) {
                    return btnClickAfterSave2New(vButton, table_ID, record_ID, ctx, batch, aPanel, ret, columnName, isbg);
                }
            }

            // }



            //if (needExecute) {
            //    return btnClickAfterSave2New(vButton, table_ID, record_ID, ctx, batch, aPanel, ret, columnName);
            //}


        }
        catch (ex) {
            VIS.ADialog.error("Error?", true, "error in process : " + ex.message);
            ret = false;
        }
        return false;
    };




    function btnClickAfterSave2New(vButton, table_ID, record_ID, ctx, batch, aPanel, ret, columnName, isbackground) {
        var title = vButton.getDescription();
        if (title == null || title.length == 0)
            title = columnName;
        var pi = new VIS.ProcessInfo(title, vButton.getProcess_ID(), table_ID, record_ID);
        pi.setAD_User_ID(ctx.getAD_User_ID());
        pi.setAD_Client_ID(ctx.getAD_Client_ID());
        pi.setAD_Window_ID((aPanel.$parentWindow === undefined ? 0 : aPanel.$parentWindow.AD_Window_ID));// vinay bhatt window id
        pi.setIsBatch(batch);
        pi.setIsBackground(isbackground);
        //start process

        var pCtl = new VIS.ProcessCtl(aPanel, pi, null);
        //pCtl.setIsPdf(vButton.isPdf);
        //pCtl.setIsCsv(vButton.isCsv);
        if (vButton.isPdf) {
            pi.setFileType(VIS.ProcessCtl.prototype.REPORT_TYPE_PDF);
        }
        else if (vButton.isCsv) {
            pi.setFileType(VIS.ProcessCtl.prototype.REPORT_TYPE_CSV);
        }

        pCtl.process(aPanel.curWindowNo); //  calls lockUI, unlockUI
        ret = true;
        aPanel = null;
        vButton = null;
        batch = false;
        startWOasking = false;
        actionProcessAfterSave = null;
        return ret;
    };


    /**
     *	tab change
     *  @param action tab item's id
     */
    APanel.prototype.tabActionPerformed = function (action) {

        if (!this.vTabbedPane.getIsTabChanged(action)) {
            console.log("tabNotChange");
            return false;
        }


        var back = false;
        var isAPanelTab = false;
        var tabEle = this.vTabbedPane.getTabElement(action);
        var curEle = this.curST || this.curGC;
        var oldGC = null;

        //// To Clear SearchText Box on Tab Change
        this.toggleASearchIcons(false, false);
        this.setAdvancedSerachText(true, "");
        //// END


        var selfPanel = this;
        //  Workbench Tab Change
        if (this.vTabbedPane.getIsWorkbench()) {
            //
        }
        else {
            //  Just a Tab Change
            //log.Info("Tab=" + tp);
            this.curWinTab = this.vTabbedPane;
            var tpIndex = this.curWinTab.getSelectedIndex();
            back = tpIndex < this.curTabIndex;
            var gc = null, st = null;
            if (tabEle instanceof VIS.VSortTab) {
                st = tabEle;
                isAPanelTab = true;
            } else {
                gc = tabEle;
            }

            var canExecute = true;

            if (this.curGC != null) {


                //  has anything changed?
                if (this.curTab.needSave(true, false)) {   //  do we have real change
                    if (this.curTab.needSave(true, true)) {
                        //	Automatic Save
                        if (this.ctx.isAutoCommit(this.curWindowNo)) {
                            if (!this.curTab.dataSave(true)) {	//  there is a problem, so we go back	
                                this.vTabbedPane.restoreTabChange();//m_curWinTab.setSelectedIndex(m_curTabIndex);
                                this.setBusy(false, true);
                                return false;
                            }
                        }
                        //    //  explicitly ask when changing tabs
                        //else if (VIS.ADialog.ask("SaveChanges?", true, this.curTab.getCommitWarning(), '')) {//  yes we want to save
                        //    if (!this.curTab.dataSave(true)) {   //  there is a problem, so we go back
                        //        //m_curWinTab.setSelectedIndex(m_curTabIndex);
                        //        this.vTabbedPane.restoreTabChange();
                        //        this.setBusy(false, true);
                        //        return false;
                        //    }
                        //}
                        //else    //  Don't save
                        //    this.curTab.dataIgnore();

                        else {
                            canExecute = false;
                            VIS.ADialog.confirm("SaveChanges?", true, this.curTab.getCommitWarning(), 'Confirm', function (results) {
                                if (results) {
                                    if (!selfPanel.curTab.dataSave(true)) {   //  there is a problem, so we go back
                                        //m_curWinTab.setSelectedIndex(m_curTabIndex);
                                        selfPanel.vTabbedPane.restoreTabChange();
                                        selfPanel.setBusy(false, true);
                                        return false;
                                    }
                                }
                                else {
                                    selfPanel.curTab.dataIgnore();
                                }

                                curEle = selfPanel.curGC;
                                oldGC = selfPanel.curGC;
                                selfPanel.curGC = null;
                                //selfPanel.tabActionPerformedCallback3(curEle, isAPanelTab, gc, tpIndex);

                                if (selfPanel.curST != null) {
                                    selfPanel.curST.saveData();
                                    selfPanel.curST.unRegisterAPanel();
                                    curEle = selfPanel.curST;
                                    selfPanel.curST = null;
                                }

                                selfPanel.curTabIndex = tpIndex;
                                if (!isAPanelTab)
                                    selfPanel.curGC = gc;

                                selfPanel.tabActionPerformedCallback(action, back, isAPanelTab, tabEle, curEle, oldGC, gc, st);
                            });
                        }
                    }
                    else    //  new record, but nothing changed
                        selfPanel.curTab.dataIgnore();
                }
                if (canExecute) {
                    curEle = this.curGC;
                    oldGC = this.curGC;
                    this.curGC = null;
                }
            }

            if (canExecute) {
                if (this.curST != null) {
                    this.curST.saveData();
                    this.curST.unRegisterAPanel();
                    curEle = this.curST;
                    this.curST = null;
                }

                this.curTabIndex = tpIndex;
                if (!isAPanelTab)
                    this.curGC = gc;
            }

        }
        if (canExecute) {
            selfPanel.tabActionPerformedCallback(action, back, isAPanelTab, tabEle, curEle, oldGC, gc, st);
        }

        return true;
    };

    //APanel.prototype.tabActionPerformedCallback2 = function (curEle, oldGC) {
    //    curEle = this.curGC;
    //    oldGC = this.curGC;
    //    this.curGC = null;
    //}


    //APanel.prototype.tabActionPerformedCallback3 = function (curEle, isAPanelTab, gc, tpIndex) {
    //    if (this.curST != null) {
    //        this.curST.saveData();
    //        this.curST.unRegisterAPanel();
    //        curEle = this.curST;
    //        this.curST = null;
    //    }

    //    this.curTabIndex = tpIndex;
    //    if (!isAPanelTab)
    //        this.curGC = gc;
    //}

    APanel.prototype.tabActionPerformedCallback = function (action, back, isAPanelTab, tabEle, curEle, oldGC, gc, st) {
        this.setSelectedTab(action); //set Seleted tab

        if (isAPanelTab) {
            this.curST = st;
            st.registerAPanel(this);
            st.loadData();
        }
        else {
            gc.activate(oldGC);
            if (oldGC)
                oldGC.detachDynamicAction();
            this.curTab = gc.getMTab();
            this.setDynamicActions();
            //PopulateSerachCombo(false);
            /*	Refresh only current row when tab is current(parent)*/
            if (!this.curTab.getIsZoomAction() && this.curTab.getTabLevel() > 0) {
            //if (!gc.isZoomAction && this.curTab.getTabLevel() > 0) {
                var queryy = new VIS.Query();
                this.curTab.query = queryy;
            }

            if (back && this.curTab.getIsCurrent()) {
                if (this.curTab.getTabLevel() == 0) {
                    if (this.curTab.searchText) {
                        this.setAdvancedSerachText(false, this.curTab.searchText);
                    }
                    else if (this.curTab.hasSavedAdvancedSearch) {
                        this.toggleASearchIcons(true, false);
                    }
                }
                else {
                    if (this.curTab.searchText) {
                        this.setAdvancedSerachText(false, this.curTab.searchText);
                    }
                    else if (this.curTab.hasSavedAdvancedSearch) {
                        this.toggleASearchIcons(true, false);
                    }
                }
                gc.dataRefresh();
            }
            else	//	Requery and bind
            {
                this.curTab.getTableModel().setCurrentPage(1);
                if (!this.curGC.onDemandTree || gc.isZoomAction) {

                    //var query = new VIS.Query(this.curTab.getTableName(), true);
                    //query.addRestriction(" 1 = 1 ");
                    //this.curTab.setQuery(query);

                    this.curTab.searchText = "";
                    //if (this.curTab.getTabLevel() > 0) {
                    //    this.curTab.linkValue = "-1";
                    //}
                    this.setDefaultSearch(gc);
                    //}
                    gc.query(this.curTab.getOnlyCurrentDays(), 0, false);	//	updated
                }
            }


            if (this.curGC.onDemandTree) {
                this.aShowSummaryLevel.show();
            }
            else {
                this.aShowSummaryLevel.hide();
            }
        }

        //	Order Tab
        if (isAPanelTab) {
            this.aMulti.setPressed(false);
            this.aMulti.setEnabled(false);
            this.aCard.setEnabled(false);
            this.aCardDialog.setEnabled(false);
            this.aNew.setEnabled(false);
            this.aDelete.setEnabled(false);
            this.aFind.setEnabled(false);
            this.aRefresh.setEnabled(false);
            this.aNext.setEnabled(false);
            this.aLast.setEnabled(false);
            this.aFirst.setEnabled(false);
            this.aPrevious.setEnabled(false);

            this.aPageFirst.setEnabled(false);
            this.aPageUp.setEnabled(false);
            this.aPageLast.setEnabled(false);
            this.aPageDown.setEnabled(false);
            //aAttachment.setEnabled(false);
            //aChat.setEnabled(false);
        }
        else	//	Grid Tab
        {
            this.aMulti.setEnabled(true);
            this.aMulti.setPressed(this.curGC.getIsSingleRow() || this.curGC.getIsMapRow());
            this.aCard.setEnabled(true);
            this.aCardDialog.setEnabled(true);
            this.aFind.setEnabled(true);
            this.aRefresh.setEnabled(true);
            //aAttachment.setEnabled(true);
            //aChat.setEnabled(true);
        }

        curEle.setVisible(false);
        tabEle.setVisible(true);

        /*******     Tab Panels      ******/
        if (this.curTab.getHasPanel()) {
            this.setCurrentTabPanel();
            this.setTabPanelIcons();
            this.showTabPanel(true);
        }
        else {
            this.setTabPanelIcons();
            this.showTabPanel(false);
        }

        this.refresh();

        this.setTabNavigation();

        /*******    END Tab Panels     ******/

        if (this.aParentDetail)
            this.aParentDetail.evaluate(tabEle);

        curEle = tabEle = null;

        if (this.curTab.getAD_Process_ID() == 0) {
            this.aPrint.setEnabled(false);
        }
        else this.aPrint.setEnabled(true);

        if (this.curTab.getIsMapView()) {
            this.aMap.show();
        }
        else {
            this.aMap.hide();
        }
    };



    APanel.prototype.setDefaultSearch = function (gc) {

        var $selfpanel = this;

        //       var sqlUserSearch = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END  AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
        //+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=" + this.curTab.getAD_Tab_ID() + "  AND AD_DefaultUserQuery.AD_User_ID=" + this.ctx.getAD_User_ID() + "  )  "
        //+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=" + this.curTab.getAD_Tab_ID() + "  AND AD_DefaultUserQuery.AD_User_ID=" + this.ctx.getAD_User_ID() + "  )   ELSE null End as AD_DefaultUserQuery_ID"
        //       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =" + this.ctx.getAD_Client_ID() + " AND AD_UserQuery.IsActive             ='Y' "
        //       + " AND (AD_UserQuery.AD_Tab_ID           =" + this.curTab.getAD_Tab_ID() + " OR AD_UserQuery.AD_Table_ID           =" + this.curTab.getAD_Table_ID() + ")"
        //       + " ORDER BY UPPER(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";



        var sqlUserSearch = "VIS_117";

        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Tab_ID", this.curTab.getAD_Tab_ID());
        param[1] = new VIS.DB.SqlParam("@AD_User_ID", this.ctx.getAD_User_ID());
        param[2] = new VIS.DB.SqlParam("@AD_Tab_ID1", this.curTab.getAD_Tab_ID());
        param[3] = new VIS.DB.SqlParam("@AD_User_ID1", this.ctx.getAD_User_ID());
        param[4] = new VIS.DB.SqlParam("@AD_Client_ID", this.ctx.getAD_Client_ID());
        param[5] = new VIS.DB.SqlParam("@AD_Tab_ID2", this.curTab.getAD_Tab_ID());
        param[6] = new VIS.DB.SqlParam("@AD_Table_ID", this.curTab.getAD_Table_ID());

        var data = executeDataSet(sqlUserSearch, param);

        if (data && data.tables[0].rows && data.tables[0].rows.length > 0) {
            $selfpanel.curTab.hasSavedAdvancedSearch = true;
            if ($selfpanel.curTab.getTabLevel() == 0 && !gc.isZoomAction) {
                var hasDefaultSearch = false;
                for (var i = 0; i < data.tables[0].rows.length; i++) {
                    if (data.tables[0].rows[i].cells["ad_defaultuserquery_id"] > 0) {
                        hasDefaultSearch = true;
                        $selfpanel.setAdvancedSerachText(false, data.tables[0].rows[i].cells["name"]);
                        var query = new VIS.Query($selfpanel.curTab.getTableName(), false);
                        query.addRestriction(data.tables[0].rows[i].cells["code"]);
                        $selfpanel.curTab.setQuery(query);
                        $selfpanel.defaultSearch = false;
                        $selfpanel.curTab.searchText = data.tables[0].rows[i].cells["name"];
                    }
                }
                if (!$selfpanel.curTab.hasSavedAdvancedSearch) {
                    //var query = new VIS.Query($selfpanel.curTab.getTableName(), true);
                    //$selfpanel.curTab.setQuery(query);
                }

                $selfpanel.toggleASearchIcons(true, hasDefaultSearch);
            }
            else {
                //var query = new VIS.Query($selfpanel.curTab.getTableName(), true);
                //$selfpanel.curTab.setQuery(query);
                $selfpanel.toggleASearchIcons(true, false);
            }
        }
        else {
            //var query = new VIS.Query($selfpanel.curTab.getTableName(), true);
            //$selfpanel.curTab.setQuery(query);
            $selfpanel.toggleASearchIcons(false, false);
            $selfpanel.setAdvancedSerachText(true, "");
        }

        ///});



    };

    /**
     *	Data Status Listener (row change)			^ | v
     *  @param e event 
     */
    APanel.prototype.dataStatusChanged = function (e) {

        var dbInfo = e.getMessage();
        var findPressed = this.curTab.getIsQueryActive() || this.curTab.getOnlyCurrentDays() > 0;
        if (findPressed)
            dbInfo = "[ " + dbInfo + " ]";
        this.statusBar.setStatusDB(dbInfo, e);

        //	Set Message / Info
        if (e.getAD_Message() != null || e.getInfo() != null) {
            var sb = new StringBuilder();
            var msg = e.getMessage();
            if (msg != null && msg.length > 0)
                sb.append(VIS.Msg.getMsg(e.getAD_Message()));
            var info = e.getInfo();
            if (info != null && info.length > 0) {
                if (sb.length() > 0 && !sb.endsWith(":"))
                    sb.append(": ");
                sb.append(info);
            }
            if (sb.length() > 0) {
                var pos = sb.indexOf("\n");
                if (pos != -1)  // replace 
                    sb.replace("\n", " - ");
                this.setStatusLine(sb.toString(), e.getIsError());
            }
        }

        //  Confirm Error
        if (e.getIsError() && !e.getIsConfirmed()) {
            VIS.ADialog.error(e.getAD_Message(), true, e.getInfo());
            e.setConfirmed(true);   //  show just once - if MTable.setCurrentRow is involved the status event is re-issued
            this.errorDisplayed = true;
        }
        //  Confirm Warning
        else if (e.getIsWarning() && !e.getIsConfirmed()) {
            VIS.ADialog.warn(e.getAD_Message(), true, e.getInfo());
            e.setConfirmed(true);   //  show just once - if MTable.setCurrentRow is involved the status event is re-issued
        }




        //	update Navigation
        var firstRow = e.getIsFirstRow();
        this.aFirst.setEnabled(!firstRow);
        this.aPrevious.setEnabled(!firstRow);
        var lastRow = e.getIsLastRow();
        this.aNext.setEnabled(!lastRow);
        this.aLast.setEnabled(!lastRow);

        var firstPage = e.getIsFirstPage();
        this.aPageFirst.setEnabled(!firstPage);
        this.aPageUp.setEnabled(!firstPage);
        var lastPage = e.getIsLastPage();
        this.aPageLast.setEnabled(!lastPage);
        this.aPageDown.setEnabled(!lastPage);

        //	update Change
        var changed = e.getIsChanged() || e.getIsInserting();
        var readOnly = this.curTab.getIsReadOnly();
        var insertRecord = !readOnly;
        if (insertRecord)
            insertRecord = this.curTab.getIsInsertRecord();
        this.aNew.setEnabled(!changed && insertRecord);
        if (this.aCopy) {
            this.aCopy.setEnabled(!changed && insertRecord);
        }
        this.aRefresh.setEnabled(!changed);
        this.aDelete.setEnabled(!changed && !readOnly && e.getCurrentRow() > -1);
        //
        if (readOnly && this.curTab.getIsAlwaysUpdateField())
            readOnly = false;
        this.aIgnore.setEnabled(changed && !readOnly);
        this.aSave.setEnabled(changed && !readOnly);
        this.aCardDialog.setEnabled(!changed);

        //
        //	No Rows
        if (e.getTotalRows() == 0 && insertRecord) {
            this.aNew.setEnabled(true);
            this.aDelete.setEnabled(false);
        }

        //	Single-Multi
        this.aMulti.setPressed(this.curGC.getIsSingleRow() || this.curGC.getIsMapRow());
        if (this.aChat) {
            this.aChat.setPressed(this.curTab.hasChat());
        }
        if (this.aAttachment) {
            this.aAttachment.setPressed(this.curTab.hasAttachment());
        }
        if (this.aMarkToExport) {
            this.aMarkToExport.setPressed(this.curTab.hasMarked());
        }

        if (this.aSubscribe) {
            this.aSubscribe.setPressed(this.curTab.HasSubscribed());
        }

        //  this.aChat.setEnabled(true);

        if (this.isPersonalLock) {
            this.aLock.setEnabled(true);
            this.aLock.setPressed(this.curTab.getIsLocked());
            this.aRecAccess.setEnabled(true);
        }


        if (this.curTab.getRecord_ID() == -1) {
            //this.aMulti.setEnabled(false);
            if (this.aChat) {
                this.aChat.setEnabled(false);
            }
            if (this.aAttachment) {
                this.aAttachment.setEnabled(false);
            }
            if (this.aSubscribe) {
                this.aSubscribe.setEnabled(false);
            }
            //if (this.aImportMap) {
            //    this.aImportMap.setEnabled(false);
            //}
            if (this.aHistory) {
                this.aHistory.setEnabled(false);
            }
            if (this.aEmail) {
                this.aEmail.setEnabled(false);
            }
            if (this.aLetter) {
                this.aLetter.setEnabled(false);
            }
            if (this.aSms) {
                this.aSms.setEnabled(false);
            }
            if (this.aFaxEmail) {
                this.aFaxEmail.setEnabled(false);
            }

            if (this.aCreateDocument) {
                this.aCreateDocument.setEnabled(false);
            }
            if (this.aUploadDocument) {
                this.aUploadDocument.setEnabled(false);
            }
            if (this.aViewDocument) {
                this.aViewDocument.setEnabled(false);
            }
            if (this.aAttachFrom) {
                this.aAttachFrom.setEnabled(false);
            }
            if (this.aZoomAcross) {
                this.aZoomAcross.setEnabled(false);
            }
            if (this.aMarkToExport) {
                this.aMarkToExport.setEnabled(false);
            }
            if (this.aArchive) {
                this.aArchive.setEnabled(false);
            }
            if (this.aEmailAttach) {
                this.aEmailAttach.setEnabled(false);
            }
            if (this.aAppointment) {
                this.aAppointment.setEnabled(false);
            }
            if (this.aTask) {
                this.aTask.setEnabled(false);
            }
            if (this.aRequest) {
                this.aRequest.setEnabled(false);
            }
            if (this.aWorkflow) {
                this.aWorkflow.setEnabled(false);
            }
            if (this.aCopy) {
                this.aCopy.setEnabled(false);
            }
            if (this.aLock) {
                this.aLock.setEnabled(false);
            }
            if (this.aRecAccess) {
                this.aRecAccess.setEnabled(false);
            }

            //if (this.aCall) {
            //    this.aCall.setEnabled(false);
            //}
        }
        else {

            if (this.aChat) {
                this.aChat.setEnabled(true);
            }
            if (this.aAttachment) {
                this.aAttachment.setEnabled(true);
            }
            if (this.aSubscribe) {
                this.aSubscribe.setEnabled(true);
            }
            if (this.aHistory) {
                this.aHistory.setEnabled(true);
            }
            if (this.aEmail) {
                this.aEmail.setEnabled(true);
            }
            if (this.aLetter) {
                this.aLetter.setEnabled(true);
            }
            if (this.aSms) {
                this.aSms.setEnabled(true);
            }
            if (this.aFaxEmail) {
                this.aFaxEmail.setEnabled(true);
            }
            if (this.aImportMap) {
                this.aImportMap.setEnabled(true);
            }
            if (this.aCreateDocument) {
                this.aCreateDocument.setEnabled(true);
            }
            if (this.aUploadDocument) {
                this.aUploadDocument.setEnabled(true);
            }
            if (this.aViewDocument) {
                this.aViewDocument.setEnabled(true);
            }
            if (this.aAttachFrom) {
                this.aAttachFrom.setEnabled(true);
            }
            if (this.aZoomAcross) {
                this.aZoomAcross.setEnabled(true);
            }
            if (this.aMarkToExport) {
                this.aMarkToExport.setEnabled(true);
            }
            if (this.aArchive) {
                this.aArchive.setEnabled(true);
            }
            if (this.aEmailAttach) {
                this.aEmailAttach.setEnabled(true);
            }
            if (this.aAppointment) {
                this.aAppointment.setEnabled(true);
            }
            if (this.aTask) {
                this.aTask.setEnabled(true);
            }
            if (this.aRequest) {
                this.aRequest.setEnabled(true);
            }
            if (this.aWorkflow) {
                this.aWorkflow.setEnabled(true);
            }
            if (this.aCopy) {
                this.aCopy.setEnabled(true);
            }
            if (this.aLock) {
                this.aLock.setEnabled(true);
            }
            //if (this.aCall) {
            //    this.aCall.setEnabled(true);
            //}


            //this.aMulti.setEnabled(true);
            //this.aChat.setEnabled(true);
            //this.aAttachment.setEnabled(true);
            //this.aSubscribe.setEnabled(true);
            //this.aHistory.setEnabled(true);
            //this.aEmail.setEnabled(true);
            //this.aLetter.setEnabled(true);
            //this.aSms.setEnabled(true);
            //this.aFaxEmail.setEnabled(true);
            //this.aSubscribe.setEnabled(true);
            //this.aCreateDocument.setEnabled(true);
            //this.aUploadDocument.setEnabled(true);
            //this.aViewDocument.setEnabled(true);
            //this.aAttachFrom.setEnabled(true);
            //this.aZoomAcross.setEnabled(true);
            //this.aMarkToExport.setEnabled(true);
            //this.aArchive.setEnabled(true);
            //this.aEmailAttach.setEnabled(true);
            //this.aAppointment.setEnabled(true);
            //this.aTask.setEnabled(true);
            //this.aRequest.setEnabled(true);
            //this.aWorkflow.setEnabled(true);
        }

        //	Transaction info

        if (!e.getIsInserting()) {
            //var trxInfo = VIS.GridTab.prototype.getTrxInfo(this.curTab.getTableName(), VIS.context, this.curTab.getWindowNo(), this.curTab.getTabNo());
            var tht = this;
            VIS.GridTab.prototype.getFooterInfo(this.curTab.getTableName(), VIS.context, this.curTab.getWindowNo(),
                this.curTab.getTabNo(), e.getRecord_ID()).then(function (info) {
                    if (tht && tht.statusBar)
                        tht.statusBar.setInfo(info);
                }, function (err) {
                    if (tht && tht.statusBar)
                        tht.statusBar.setInfo(err);
                });


        }
        else {
               this.statusBar.setInfo(null);
        }

        if (this.curWinTab == this.vTabbedPane) {
            this.curWinTab.evaluate(null);
        }




    };   //

    /**
     *	Set Status Line to text
     *  @param text clear text
     *  @param error error flag
     */
    APanel.prototype.setStatusLine = function (text, error) {
        this.statusBar.setStatusLine(text, error);
    };




    //Cmd_Actions

    APanel.prototype.cmd_refresh = function () {
        this.cmd_save(false);
        this.curGC.dataRefreshAll();
    };//Refresh

    APanel.prototype.cmd_ignore = function () {
        //m_curGC.stopEditor(false);
        this.curGC.dataIgnore();

    };//Undo

    APanel.prototype.cmd_help = function ()//sarab
    {
        var help = new VIS.Apps.help(this.gridWindow);
    };

    APanel.prototype.cmd_cardDialog = function () {

        var card = new VIS.CVDialog(this);
        card.show();
    };

    APanel.prototype.cmd_save = function (manual, callback) {
        //cmd_save(false);
        //this.curGC.dataRefreshAll();
        if (this.curST != null)
            manual = false;
        this.errorDisplayed = false;
        //this.curGC.stopEditor(true);

        if (this.curST != null) {
            this.curST.saveData();
            this.aSave.setEnabled(false);	//	set explicitly
            return;
        }

        var needExecute = true;

        if (this.curTab.getCommitWarning().length > 0 && this.curTab.needSave(true, false)) {
            needExecute = false;

            var selfPanel = this;

            VIS.ADialog.confirm("SaveChanges?", true, this.curTab.getCommitWarning(), "Confirm", function (result) {

                if (!result) {
                    return;
                }
                var retValue = selfPanel.curGC.dataSave(manual);

                if (manual && !retValue && !selfPanel.errorDisplayed) {

                }
                selfPanel.curGC.refreshTabPanelData(this.curTab.getRecord_ID());
                if (manual)
                    selfPanel.curGC.dynamicDisplay(-1);

            });

            if (callback) {
                callback(retValue);
            }
        }

        if (needExecute) {

            var retValue = this.curGC.dataSave(manual);

            if (manual && !retValue && !this.errorDisplayed) {

            }

            if (manual)
                this.curGC.dynamicDisplay(-1);

            if (callback) {
                callback(retValue);
            }
            this.curGC.refreshTabPanelData(this.curTab.getRecord_ID());
            return retValue;
        }

    };//Save

    APanel.prototype.cmd_new = function (copy) { //Create New Record
        if (!this.curTab.getIsInsertRecord()) {
            //log.warning("Insert Record disabled for Tab");
            return;
        }
        //cmd_save(false);
        this.curGC.dataNew(copy);
    };// New

    APanel.prototype.cmd_delete = function () {
        if (this.curTab.getIsReadOnly())
            return;
        //var keyID = this.curTab.getRecord_ID();
        //prevent deletion if client access for Read Write does not exist for this Role.

        var ids = this.curGC.canDeleteRecords()


        // if (!VIS.MRole.getDefault().getIsClientAccess(this.curTab.getAD_Client_ID(), true))
        if (ids.length > 0) {
            VIS.ADialog.error("CannotDelete", true, " [ " + ids.join(",") + "]");
            return;
        }

        //if (VIS.ADialog.ask("DeleteRecord?")) {
        //    this.curGC.dataDelete();
        //}

        var thisPanel = this;

        VIS.ADialog.confirm("DeleteRecord?", true, "", "Confirm", function (result) {
            if (result) {
                thisPanel.curGC.dataDeleteAsync();
            }
        });


    };

    /* 
     -Quick Search 
     @param val text to search
     */

    APanel.prototype.cmd_find = function (val) {

        if (!this.curTab)
            return;

        if (!this.defaultSearch) {
            this.defaultSearch = true;
            return;
        }

        this.setBusy(true);

        var query = null;

        if (val && val.trim() !== "") {
            val = "%" + val + "%";
            query = this.curTab.getSearchQuery(val);
        }

        var onlyCurrentDays = 0;
        var created = false;

        this.curTab.getTableModel().setCurrentPage(1);

        if (this.isSummaryVisible) {
            this.curTab.setShowSummaryNodes(true);
        }
        else {
            this.curTab.setShowSummaryNodes(false);
        }

        this.curGC.skipRowInserting(true); // do - not insert row 
        if (query != null && query.getIsActive()) {
            //log.config(query.toString());
            this.curTab.setQuery(query);
            this.curGC.query(0, 0, created);   //  autoSize
            //, this.curGC.treeNodeID, this.treeID
        }
        else {
            //var maxRows = VIS.MRole.getMaxQueryRecords();
            var maxRows = 0;
            //log.config("OnlyCurrentDays=" + onlyCurrentDays + ", MaxRows=" + maxRows);
            this.curTab.setQuery(null);	//	reset previous queries
            this.curGC.query(onlyCurrentDays, maxRows, created);   //  autoSize
        }
        // this.curGC.dynamicDisplay(-1);
        //this.setBusy(false);
    };

    APanel.prototype.cmd_chat = function () {
        var record_ID = this.curTab.getRecord_ID();
        if (record_ID == -1)	//	No Key
        {
            this.aChat.setEnabled(false);
            return;
        }

        //	Find display
        var infoName = null;
        var infoDisplay = null;
        for (var i = 0; i < this.curTab.getFieldCount(); i++) {
            var field = this.curTab.getField(i);
            if (field.getIsKey())
                infoName = field.getHeader();
            if ((field.getColumnName().toString() == "Name" || field.getColumnName().toString() == "DocumentNo")
                && (field.getValue() != null && field.getValue() != ""))
                infoDisplay = field.getValue();
            if (infoName != null && infoDisplay != null)
                break;
        }

        var self = this;
        //var onchatClose = function () { self.curTab.loadChats(); self.aChat.setPressed(self.curTab.hasChat()); };

        var chat = new VIS.Chat(record_ID, this.curTab.getCM_ChatID(), this.curTab.getAD_Table_ID(), infoName + ": " + infoDisplay, this.curWindowNo);

        chat.onClose = function () {
            self.curTab.loadChats();
            self.aChat.setPressed(self.curTab.hasChat());
            self = null;
        }
        chat.show();
    };

    APanel.prototype.cmd_appointment = function () {
        var record_ID = this.curTab.getRecord_ID();
        ///Check table has Email column

        var AD_Table_ID = this.curTab.getAD_Table_ID();
        //log.Info("Record_ID=" + record_ID);
        if (record_ID == -1)	//	No Key
        {
            return;
        }
        VIS.AppointmentsForm.init(AD_Table_ID, record_ID, 0, 0, false, true);
    };

    APanel.prototype.cmd_task = function () {
        var record_ID = this.curTab.getRecord_ID();
        ///Check table has Email column

        var AD_Table_ID = this.curTab.getAD_Table_ID();
        //log.Info("Record_ID=" + record_ID);
        if (record_ID == -1)	//	No Key
        {
            return;
        }
        VIS.AppointmentsForm.init(AD_Table_ID, record_ID, 0, 0, true);
    };

    APanel.prototype.cmd_letter = function () {
        var record_ID = this.curTab.getRecord_ID();
        if (record_ID == -1)	//	No Key
        {
            this.aLetter.setEnabled(false);
            return;
        }

        var email = new VIS.Email("", this.curTab, this.curGC, record_ID, false);
        var c = new VIS.CFrame();
        c.setName(VIS.Msg.getMsg("Letter"));
        c.setTitle(VIS.Msg.getMsg("Letter"));
        c.hideHeader(true);
        c.setContent(email);
        c.show();
        email.initializeComponent();

    };

    APanel.prototype.cmd_email = function () {
        var record_ID = this.curTab.getRecord_ID();
        //List<DataGridRow> Rows = new List<DataGridRow>();
        //IList rowsource = _curGC.GetSelectedRows();
        // log.Info("Record_ID=" + record_ID);
        if (record_ID == -1)	//	No Key
        {
            this.aEmail.setEnabled(false);
            return;
        }

        //string to = "";
        //if (((DataUtil.DataObject)rowsource[0]).GetFieldValue("EMAIL") != null)
        //{
        //    if (_curGC.IsSingleRow())
        //    {
        //        object a = _curTab.GetValue("Email");
        //        if (a != null)
        //        {
        //            to = a.ToString();
        //        }
        //    }

        //    else//multi row selected
        //    {
        //        //_curGC.ge
        //        int count = rowsource.Count;
        //        if (count == 1)
        //        {
        //            object a = _curTab.GetValue("Email");
        //            if (a != null)
        //            {
        //                to = a.ToString();
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < rowsource.Count; i++)
        //            {
        //                if (((DataUtil.DataObject)rowsource[i]).GetFieldValue("EMAIL") != null)
        //                {
        //                    to += ((DataUtil.DataObject)rowsource[i]).GetFieldValue("EMAIL").ToString().Trim() + ",";
        //                }
        //            }
        //            while (to.EndsWith(","))
        //            {
        //                to = to.Substring(0, to.Length - 1);
        //            }
        //        }
        //    }
        //}
        //else
        //{

        //}

      
        var email = new VIS.Email("", this.curTab, this.curGC, record_ID, true);

        var c = new VIS.CFrame();
        c.setName(VIS.Msg.getMsg("EMail"));
        c.setTitle(VIS.Msg.getMsg("EMail"));
        c.hideHeader(true);
        c.setContent(email);
        c.show();
        email.initializeComponent();

        //email.show();
    };

    APanel.prototype.cmd_report = function () {

        if (!VIS.MRole.getDefault().getIsCanReport(this.curTab.getAD_Table_ID())) {
            VIS.ADialog.warn("AccessCannotReport");
            return;
        }
        if (this.curTab.needSave(true, false)) {
            this.cmd_save(true);
            return;
        }
        //var rquery = this.curTab.query; //new VIS.Query(this.curTab.getTableName());
        var rquery = null;
        if (this.curTab.query && this.curTab.query.list.length > 0)
            rquery = this.curTab.query;
        else
            rquery = new VIS.Query(this.curTab.getTableName());


        rquery.tableName = this.curTab.getTableName();
        var queryColumn = this.curTab.getLinkColumnName();
        if (queryColumn.length == 0)
            queryColumn = this.curTab.getKeyColumnName();
        var infoName = null;
        var infoDisplay = null;
        for (var i = 0, j = this.curTab.getFieldCount(); i < j; i++) {
            var field = this.curTab.getField(i);
            if (field.getIsKey())
                infoName = field.getHeader();
            if ((field.getColumnName() == "Name" || field.getColumnName() == "DocumentNo")
                && field.getValue() != null)
                infoDisplay = field.getValue();
            if (infoName != null && infoDisplay != null)
                break;
        }

        var isParent = this.curTab.getParentColumnNames().length == 0;
        if (queryColumn.length != 0) {
            if (!isParent || (this.curTab.getLinkColumnName() != null && this.curTab.getLinkColumnName() != ""))    //only selected record to be printed
            {
                if (queryColumn.endsWith("_ID")) {
                    if (infoName == null && infoDisplay == null) {
                        rquery.addRestriction(queryColumn, VIS.Query.prototype.EQUAL,
                            VIS.context.getContextAsInt(this.curWindowNo, queryColumn));
                    }
                    else {
                        rquery.addRestriction(queryColumn, VIS.Query.prototype.EQUAL,
                            VIS.context.getContextAsInt(this.curWindowNo, queryColumn),
                            infoName, infoDisplay);
                    }
                }
                else {
                    if (infoName == null && infoDisplay == null) {
                        rquery.addRestriction(queryColumn, VIS.Query.prototype.EQUAL,
                            VIS.context.getContext(this.curWindowNo, queryColumn));
                    }
                    else {
                        rquery.addRestriction(queryColumn, VIS.Query.prototype.EQUAL,
                            VIS.context.getContext(this.curWindowNo, queryColumn),
                            infoName, infoDisplay);
                    }
                }
            }
        }

        if (this.curGC.onDemandTree) {
            if (!this.isSummaryVisible) {
                var report = new VIS.AReport(this.curTab.getAD_Table_ID(), rquery, this.curTab.getAD_Tab_ID(), this.curWindowNo, this.curTab, this.curGC.treeID, this.curGC.treeNodeID, false);
            }
            else {
                var report = new VIS.AReport(this.curTab.getAD_Table_ID(), rquery, this.curTab.getAD_Tab_ID(), this.curWindowNo, this.curTab, this.curGC.treeID, this.curGC.treeNodeID, true);
            }
        }
        else {
            var report = new VIS.AReport(this.curTab.getAD_Table_ID(), rquery, this.curTab.getAD_Tab_ID(), this.curWindowNo, this.curTab, 0, 0, false);
        }
    };

    APanel.prototype.cmd_print = function () {
        var rowsSource = this.curGC.getSelectedRows();
        if (rowsSource.length == 0) {
            VIS.ADialog.info('SelectRecord');
            return;
        }

        var AD_Process_ID = this.curTab.getAD_Process_ID();
        if (AD_Process_ID == 0) {
            return;
        }


        var sql = "VIS_118";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Process_ID", AD_Process_ID);
        var AD_ReportFormat_ID = executeScalar(sql, param);


        sql = "VIS_119";
        var InstalledVersion = executeScalar(sql);



        if (rowsSource.length > 1 && AD_ReportFormat_ID > 0 && InstalledVersion && (InstalledVersion.toString() > ('1.0.0.3'))) {

            if (this.curTab.needSave(true, false)) {
                this.cmd_save(true);
                return;
            }

            var recIds = '';

            for (var i = 0; i < rowsSource.length; i++) {
                if (i == 0) {
                    recIds = rowsSource[i][this.curTab.getKeyColumnName().toLower()];
                }
                else {
                    recIds += ',' + rowsSource[i][this.curTab.getKeyColumnName().toLower()];
                }
            }
            var print = new VIS.APrint(AD_Process_ID, this.curTab.getAD_Table_ID(), 0, this.curWindowNo, recIds, this.curTab, true);
            print.start(this.aPrint.getListItmIT());
        }
        else {
            var recID = this.curTab.getRecord_ID();
            if (recID == -1) {
                VIS.ADialog.info(VIS.Msg.getMsg('SelectRecord'));
                return;
            }

            if (this.curTab.needSave(true, false)) {
                this.cmd_save(true);
                return;
            }

            var print = new VIS.APrint(AD_Process_ID, this.curTab.getAD_Table_ID(), recID, this.curWindowNo, null, this.curTab);
            print.start(this.aPrint.getListItmIT());
        }
        //var table_ID = this.curTab.getAD_Table_ID();
        //var record_ID = this.curTab.getRecord_ID();
        //var pi = new VIS.ProcessInfo('Print', AD_Process_ID, table_ID, record_ID);        
        //pi.setAD_User_ID(VIS.context.getAD_User_ID());
        //pi.setAD_Client_ID(VIS.context.getAD_Client_ID());

        //pctrl.process(this.curWindowNo);
    };

    APanel.prototype.cmd_sms = function () {
        var record_ID = this.curTab.getRecord_ID();
        //List<DataGridRow> Rows = new List<DataGridRow>();
        //IList rowsource = _curGC.GetSelectedRows();
        // log.Info("Record_ID=" + record_ID);
        if (record_ID == -1)	//	No Key
        {
            this.aSms.setEnabled(false);
            return;
        }

        //string to = "";
        //if (((DataUtil.DataObject)rowsource[0]).GetFieldValue("EMAIL") != null)
        //{
        //    if (_curGC.IsSingleRow())
        //    {
        //        object a = _curTab.GetValue("Email");
        //        if (a != null)
        //        {
        //            to = a.ToString();
        //        }
        //    }

        //    else//multi row selected
        //    {
        //        //_curGC.ge
        //        int count = rowsource.Count;
        //        if (count == 1)
        //        {
        //            object a = _curTab.GetValue("Email");
        //            if (a != null)
        //            {
        //                to = a.ToString();
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < rowsource.Count; i++)
        //            {
        //                if (((DataUtil.DataObject)rowsource[i]).GetFieldValue("EMAIL") != null)
        //                {
        //                    to += ((DataUtil.DataObject)rowsource[i]).GetFieldValue("EMAIL").ToString().Trim() + ",";
        //                }
        //            }
        //            while (to.EndsWith(","))
        //            {
        //                to = to.Substring(0, to.Length - 1);
        //            }
        //        }
        //    }
        //}
        //else
        //{

        //}
        var sms = new VIS.Sms(this.curTab, this.curGC, record_ID, false);
        var c = new VIS.CFrame();
        c.setName(VIS.Msg.getMsg("Sms"));
        c.setTitle(VIS.Msg.getMsg("Sms"));
        c.hideHeader(true);
        c.setContent(sms);
        c.show();
        sms.initializeComponent();
    };

    APanel.prototype.cmd_subscribe = function () {
        var record_ID = this.curTab.getRecord_ID();
        if (record_ID == -1)	//	No Key
        {
            this.aSubscribe.setEnabled(false);
            return;
        }
        var self = this;
        var reloadSubscribe = function () {
            self.curTab.loadSubscribe();
            self.aSubscribe.setPressed(self.curTab.HasSubscribed());
        };
        VIS.dataContext.subscribeUnsubscribeRecords(this.curTab.getCM_SubScribedID(), this.curTab.getAD_Window_ID(), record_ID, this.curTab.getAD_Table_ID(), reloadSubscribe);
    };

    APanel.prototype.cmd_ImportMap = function () {
        if (window.VDIU) {

            if (this.curTab.getIsReadOnly()) {
                VIS.ADialog.warn("ReadOnly");
                return;
            }

            var excel = new VDIU.ImportExcel(this.curTab.getAD_Window_ID(), this.gridWindow.getName());
            var c = new VIS.CFrame();
            c.setName(VIS.Msg.getMsg("Import"));
            c.setTitle(VIS.Msg.getMsg("Import"));
            //c.hideHeader(true);
            c.setContent(excel);
            c.show();
            excel.initialize();
        }
        else {
            VIS.ADialog.error("PleaseInstallExcelImportModule");

        }
    };

    APanel.prototype.cmd_attachment = function () {
        //alert("attachment");
        if (this.curTab.getRecord_ID() < 1) {
            this.aAttachment.setEnabled(false);
            return;
        }
        var self = this;
        var att = new VIS.attachmentForm(0, 0, this.curTab.getAD_Table_ID(), this.curTab.getRecord_ID(), '');
        att.show();
        att.onClose = function () {
            self.curTab.loadAttachments();
            self.aAttachment.setPressed(self.curTab.hasAttachment());
            self = null;
        };

        //att.on('close', function () {
        //    self.curTab.loadAttachments();
        //    self.aChat.setPressed(self.curTab.hasAttachment());
        //    self = null;
        //    //this.aAttachment.setPressed(this.curTab.hasAttachment());
        //});

    };

    APanel.prototype.cmd_history = function () {
        var atHistory = null;
        var c_Bpartner_ID = 0;
        var AD_User_ID = 0;
        if (Object.keys(this.curGC.getColumnNames()).indexOf("C_BPartner_ID") > 0 || (this.curTab.getField("C_BPartner_ID") != null && this.curTab.getField("C_BPartner_ID").getValue() > 0)) {
            c_Bpartner_ID = this.curTab.getField("C_BPartner_ID").getValue();
            //atHistory = new VIS.AttachmentHistory(this.curTab.getAD_Table_ID(), this.curTab.getRecord_ID(), this.curTab.getField("C_BPartner_ID").getValue());
        }

        if (Object.keys(this.curGC.getColumnNames()).indexOf("AD_User_ID") > 0 || (this.curTab.getField("AD_User_ID") != null && this.curTab.getField("AD_User_ID").getValue() > 0)) {
            AD_User_ID = this.curTab.getField("AD_User_ID").getValue();
        }

        atHistory = new VIS.AttachmentHistory(this.curTab.getAD_Table_ID(), this.curTab.getRecord_ID(), c_Bpartner_ID, AD_User_ID, this.curTab.getKeyColumnName());

        atHistory.show();
    };

    APanel.prototype.cmd_finddialog = function () {

        var find = new VIS.Find(this.curWindowNo, this.curTab, 0);
        var self = this;
        var savedSearchName = "";
        find.onClose = function () {

            if (find.getIsOKPressed() || find.needRefresh()) {
                //if (find.getIsOKPressed()) {

                self.setAdvancedSerachText(true, "");
                self.curTab.searchText = "";

                if (self.isSummaryVisible) {
                    self.curTab.setShowSummaryNodes(true);
                }
                else {
                    self.curTab.setShowSummaryNodes(false);
                }
                var query = find.getQuery();
                //	History
                var onlyCurrentDays = find.getCurrentDays();
                var created = find.getIsCreated();
                savedSearchName = find.getSavedQueryName();
                self.curTab.userQueryID = find.getSavedID() //find.getID();
                find.dispose();
                find = null;

                //Set Page value to 1
                self.curTab.getTableModel().setCurrentPage(1);
                //	Confirmed query
                if (query != null && query.getIsActive()) {
                    //log.config(query.toString());
                    self.curTab.setQuery(query);
                    self.curGC.query(0, 0, created);   //  autoSize
                }
                else if (query != null) {
                    var maxRows = VIS.MRole.getDefault().getMaxQueryRecords();
                    //self.log.config("OnlyCurrentDays=" + onlyCurrentDays
                    //        + ", MaxRows=" + maxRows);
                    self.curTab.setQuery(null);	//	reset previous queries
                    self.curGC.query(onlyCurrentDays, maxRows, created);   //  autoSize
                }
                var findPressed = self.curTab.getIsQueryActive() || self.curTab.getOnlyCurrentDays() > 0;
                self.aFind.setPressed(findPressed);


            }
            ////Refresh everytime bcoz smtimes user create an ASearch and save it, 
            ////but this search has no data for now. but search will be saved, so we have to refresh list everytime
            self.refreshSavedASearchList(false, savedSearchName);

            self = null;
        };
        find.show();

    };

    APanel.prototype.cmd_preference = function () {

        var uf = new VIS.Framework.UserPreference();
        uf.load();
        uf.showDialog();
        uf = null;

    };

    //lakhwinder
    APanel.prototype.cmd_infoWindow = function () {

        VIS.InfoMenu.show(this.aInfo.getItem());
    };

    APanel.prototype.cmd_zoomAcross = function () {
        var Record_ID = this.curTab.getRecord_ID();

        if (Record_ID > 0) {
            //alert('ZoomAcross');
            //	Query
            var query = new VIS.Query();
            //	Current row
            var link = this.curTab.getKeyColumnName();
            //	Link for detail records
            if (link.length == 0)
                link = this.curTab.getLinkColumnName();
            if (link.length != 0) {
                if (link.endsWith('_ID'))
                    query.addRestriction(link, VIS.Query.prototype.EQUAL, VIS.context.getContextAsInt(this.curWindowNo, link));
                else
                    query.addRestriction(link, VIS.Query.prototype.EQUAL, VIS.context.getContext(this.curWindowNo, link));
            }
            //AZoomAcross zoom = new AZoomAcross(aZoomAcross.GetDropDownButton(), _curTab.GetTableName(), query, _curTab.GetAD_Window_ID());
            var zoom = new VIS.AZoomAcross(this.aZoomAcross, this.curTab.getTableName(), query, this.curTab.getAD_Window_ID(), this, this.aZoomAcross.getListItmIT(), link, Record_ID);
            zoom.init();

            //zoom.ShowPopup(bnavZoomAcross, _curTab.GetTableName(), query, windowV0.AD_Window_ID);

            //zoom = null;
        }

        // VIS.InfoMenu.show(this.aInfo.getItem());

    };

    /**
     * 	Open/View Request
     */
    APanel.prototype.cmd_request = function () {
        var record_ID = this.curTab.getRecord_ID();
        //log.Info("ID=" + record_ID);
        if (record_ID > 0) {
            var AD_Table_ID = this.curTab.getAD_Table_ID();
            //var C_BPartner_ID = 0;
            var BPartner_ID = this.curTab.getValue("C_BPartner_ID");
            //if (BPartner_ID != null)
            //    C_BPartner_ID = parseInt(BPartner_ID);

            var req = new VIS.ARequest(this.aRequest, AD_Table_ID, record_ID, BPartner_ID, null, this.aRequest.getListItmIT());
            req.getRequests();
            req = null;
        }
    };

    APanel.prototype.cmd_markToExport = function () {
        var recID = this.curTab.getRecord_ID();
        var rowsource = this.curGC.getSelectedRows();
        var recStr = null;
        var table_ID = this.curTab.getAD_Table_ID();
        var tableName = this.curTab.getTableName();
        if (recID == -1)	//	No Key
        {
            var data = {
                AD_Table_ID: table_ID
            };
            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "JsonData/GetKeyColumns", data);
            if (res == null) {
                return;
            }

            //$.ajax({
            //    url: VIS.Application.contextUrl + "JsonData/GetKeyColumns",
            //    dataType: "json",
            //    type: "POST",                
            //    data: {
            //        AD_Table_ID: table_ID
            //    },
            //    error: function () {
            //        return;
            //    },
            //    success: function (dyndata) {

            //        var _recordID = "";
            //        var _recordID2 = "";

            //        if (this.curGC.singleRow)
            //        {
            //            var a = this.curGC.getValue(keyNames[0]);
            //            var b = this.curGC.getValue(keyNames[1]);
            //            if (a != null && b != null)
            //            {
            //                _recordID = a.toString();
            //                _recordID2 = b.toString();
            //            }
            //        }
            //    }
            //});
        }
        else {
            var _primaryKey = tableName + "_ID";
            var _recordID = "";

            //Single Row
            if (this.curGC.singleRow) {
                recStr = recID;
                var markM = new VIS.MarkModule();

                markM.init(recStr, table_ID, tableName);
                markM.show();
                var self = this;
                markM.onClose = function () {
                    self.curTab.loadMarking();
                    self.aMarkToExport.setPressed(self.curTab.hasMarked());
                    self = null;
                };
                return;
            }
            //MultiRow 
            else {
                var count = rowsource.length;
                if (count == 1) {
                    _recordID = recID;
                }
                else {
                    for (var i = 0; i < rowsource.length; i++) {
                        _recordID += rowsource[i][_primaryKey.toLowerCase()] + ',';

                    }
                    if (_recordID.endsWith(",")) {
                        _recordID = _recordID.substring(0, _recordID.length - 1);
                    }
                }

                var markM = new VIS.MarkModule();
                markM.init(_recordID, table_ID, tableName);
                markM.show();
                var self = this;
                markM.onClose = function () {
                    self.curTab.loadMarking();
                    self.aMarkToExport.setPressed(self.curTab.hasMarked());
                    self = null;
                };
                return;
            }
        }


        //var markM = new VIS.MarkModule();
        //markM.show();
    };


    APanel.prototype.cmd_lock = function () {
        var locked = false;
        if (!this.isPersonalLock) {
            return;
        }
        var record_ID = this.curTab.getRecord_ID();
        if (record_ID == -1 || record_ID < 0)	//	No Key
        {
            return;
        }
        this.curTab.locks(VIS.context, record_ID, this.aLock.getIsPressed());
        this.curTab.loadAttachments();			//	reload
        locked = this.curTab.getIsLocked();
        this.aLock.setPressed(locked);
    };
    APanel.prototype.cmd_recAccess = function () {
        var recAccessDialog = new VIS.RecordAccessDialog();
        recAccessDialog.Load(this.curTab.getAD_Table_ID(), this.curTab.getRecord_ID());

    };

    APanel.prototype.ShowSummaryNodes = function () {
        if (this.isSummaryVisible) {
            this.aShowSummaryLevel.setPressed(false);
            this.isSummaryVisible = false;
        }
        else {
            this.aShowSummaryLevel.setPressed(true);
            this.isSummaryVisible = true;
        }
    };

    APanel.prototype.cmd_call = function () {

        var record_ID = this.curTab.getRecord_ID();
        if (record_ID == -1)	//	No Key
        {
            this.aCall.setEnabled(false);
            return;
        }
        var table_ID = this.curTab.getAD_Table_ID();

        if (VA048 && VA048.Apps) {
            var call = new VA048.Apps.CallForm();
            call.initializeComponent(table_ID, record_ID);

            var c = new VIS.CFrame();
            c.setName(VIS.Msg.getMsg("Call"));
            c.setTitle(VIS.Msg.getMsg("Call"));
            c.hideHeader(true);
            c.setContent(call);
            c.show();
        }
        else {
            alert(VIS.Msg.getMsg("Please install Communication module first"));
        }
    };

    /* END */


    /** 
     *  dispose
     */

    APanel.prototype.dispose = function () {

        if (this.aParentDetail)
            this.aParentDetail.dispose();

        if (this.curST != null) {
            this.curST.unRegisterAPanel();
            this.curST = null;
        }
        this.vTabbedPane.dispose();
        this.vTabbedPane = null;
        if (this.gridWindow) {
            this.gridWindow.dispose();
            this.gridWindow = null;
            this.ctx.setAutoCommit(this.$parentWindow.getWindowNo(), false);
            this.ctx.removeWindow(this.$parentWindow.getWindowNo());
            VIS.MLookupCache.cacheReset(this.$parentWindow.getWindowNo());
        }

        this.ctx = null;
        this.$parentWindow = null;
        this.tabPages = null;
        this.curGC = null;
        this.curST = null;
        this.aParentDetail = null;
        this.curTab = null;
        this.disposeComponent();

    };


    //****************** APanel END ***********************//



    //****************************************************//
    //**             VTabbedPane                       **//
    //**************************************************//

    /**
     *  Tabbed Pane - Window Tab
     *  
     */

    VIS.VTabbedPane = function (isWorkBench) {
        /** Workbench 				*/
        var _workbenchTab = false;
        /** List of dependent Variables		*/
        this.oldTabIndex = -1;

        this.Items = [];
        this.ItemsIds = [];
        this.count = 0;
        this.dependents = [];
        this.TabItems = [];

        this.getIsWorkbench = function () {
            return _workbenchTab;
        }
    };

    VIS.VTabbedPane.prototype.setTabObject = function (obj) {
        this.tabObj = obj
    }

    /**
     * 	Add Tab
     *	@param id tab id
     *	@param gTab grid tab model
     *	@param tabElement GridController or VSortTab
     */
    VIS.VTabbedPane.prototype.addTab = function (id, gTab, tabElement, tabItem) {

        this.ItemsIds[this.count] = id;
        this.Items[this.count] = tabElement;
        this.TabItems.push(tabItem);

        var tabDependents = gTab.getDependentOn();

        for (var i = 0; i < tabDependents.length; i++) {
            var name = tabDependents[i];
            if (this.dependents.indexOf(name) < 0) { // this.dependents.contains(name)) {
                this.dependents.push(name);
            }
        }
        this.count++;
    };

    /**
    * 	is tab really change
    *	@param action tab id
    */
    VIS.VTabbedPane.prototype.getIsTabChanged = function (action) {

        var index = this.ItemsIds.indexOf(action);
        var oldIndex = this.oldTabIndex;

        if (index === oldIndex) {  //Same Tab 
            console.log("same tab");
            return false;
        }



        var oldGC = this.Items[index];
        var newGC = null;

        if (oldGC instanceof VIS.GridController) {
            newGC = oldGC;
            var display = newGC.getIsDisplayed(); // if tab is not displayed
            if (!display) {
                //VLogger.Get().Info("Not displayed - " + newGC.ToString());
                return false;
            }
        }

        if (newGC != null && oldIndex >= 0 && index != oldIndex) {
            var oldGC = this.Items[oldIndex];//.Controls[0];
            if (oldGC != null && (oldGC instanceof VIS.GridController)) {

                /* check for tab Level of tab */
                if (newGC.getTabLevel() > oldGC.getTabLevel() + 1) {
                    //	Search for right tab
                    for (var i = index - 1; i >= 0; i--) {
                        var rightC = this.Items[i];// .Controls[0];// getComponentAt(i);
                        var rightGC = null;
                        if (rightC instanceof VIS.GridController) {
                            rightGC = rightC;
                            if (rightGC.getTabLevel() == oldGC.getTabLevel() + 1) {
                                VIS.ADialog.warn("TabSwitchJumpGo", true, "", rightGC.getTitle());
                                return false;;
                            }
                        }
                    }
                    VIS.ADialog.warn("TabSwitchJump");
                    return false;
                }
                oldGC.setMnemonics(false);
            }
        }
        //	Switch
        if (newGC != null) {
            newGC.setMnemonics(true);
        }
        this.oldTabIndex = index;

        return true;
    };

    VIS.VTabbedPane.prototype.restoreTabChange = function () {
        this.oldTabIndex = -1;
    }

    /**
     *  current selected tab element either GridController or VSortTab
     */
    VIS.VTabbedPane.prototype.getTabElement = function (action) {
        return this.Items[this.oldTabIndex];
    };
    /**
     *  current selected tab index
     */
    VIS.VTabbedPane.prototype.getSelectedIndex = function () {
        return this.oldTabIndex;
    };

    VIS.VTabbedPane.prototype.sizeChanged = function (height, width) {
        for (var prop in this.Items) {
            this.Items[prop].sizeChanged(height, width);
        }
    }

    VIS.VTabbedPane.prototype.evaluate = function (e) {
        var process = e == null;
        var columnName = null;
        if (!process) {
            columnName = e;
            if (columnName != null)
                process = this.dependents.indexOf(columnName) > -1;//  contains(columnName);
            else
                process = true;
        }

        if (process) {
            //VLogger.Get().Config(columnName == null ? "" : columnName);
            for (var i = 0; i < this.TabItems.length; i++) {
                var c = this.Items[i];
                if (c instanceof VIS.GridController) {
                    var gc = c;
                    var display = gc.getIsDisplayed();
                    this.TabItems[i].setEnabled(display);
                }
            }
        }
    };


    /**
     *  Dispose all contained VTabbedPanes and GridControllers
     */
    VIS.VTabbedPane.prototype.dispose = function () {
        for (var prop in this.Items) {
            this.Items[prop].dispose();
            this.Items[prop] = null;
        }

        this.TabItems.length = 0;
        this.TabItems = null;

        this.Items.length = 0;
        this.Items = null;
        this.ItemsIds.length = 0;
        this.ItemsIds = null;

        this.dependents.length = 0;
        this.dependents = null;
    };
    //****************APanel END************************//



    //****************************************************//
    //**             Apps Action                       **//
    //**************************************************//
    /*
    Optinons = {
       action:,
       text:,
       imageName:
       toggle:,
       toolTipText:,
       imageOnly:  // show only Image
       textOnly:
       isSmall:
       this.onAction=null;
    */
    function AppsAction(options) {
        if (options) {
            this.action;
            this.toggle;
            this.enableDisable;
            this.pressed;
            this.isEnabled = true;
            this.text;
            this.toolTipText;
            this.imageOnly;
            this.textOnly;
            this.onAction = null;
            this.isSmall;
            $.extend(true, this, options);
            this.items = {};

            this.$li;

            if (!this.text) {
                this.text = VIS.I18N.getLabel(this.action); //action;
                if (this.text.contains("&")) {
                    this.text = this.text.replace('&', '');
                }
                //	Data
                if (!this.toolTipText)
                    this.toolTipText = VIS.Msg.getMsg(this.action);
                else {
                    this.toolTipText = VIS.Msg.getMsg(this.toolTipText);
                    this.text = this.text + " " + this.toolTipText;
                }
                if (this.toolTipText.contains("&")) {
                    this.toolTipText = this.toolTipText.replace('&', '');
                }
            }
            var imgUrl = this.getPath();
            var imgUrlX = this.getPath();

            if (this.isSmall) {
                imgUrl += this.action + "16.png";
                if (this.toggle || this.enableDisable) {
                    imgUrlX += this.action + "X16.png";
                }
            }
            else {
                imgUrl += this.action + "24.png";
                if (this.toggle || this.enableDisable) {
                    imgUrlX += this.action + "X24.png";
                }
            }
            this.imgUrl = imgUrl;
            this.imgUrlX = imgUrlX;
        }

        var that = this;

        this.getListItm = function (listId) {
            var li = $("<li>");
            var d = $("<div></div>");
            var fired = true;
            li.on(VIS.Events.onClick, function (e) {
                e.stopPropagation();
                if (fired && that.onAction && that.isEnabled) {
                    if (that.toggle) {
                        that.setPressed(!that.pressed);
                    }


                    fired = false;
                    d.css('background-color', 'red')
                    setTimeout(function () {
                        d.css('background-color', 'transparent');
                        that.onAction(that.action);
                        fired = true;
                    }, 10);

                }
            });


            if (this.textOnly) {
                // li.text(this.text);
                li.append($('<h2>').text(this.text));
            }
            else if (this.imageOnly) {
                this.img = $('<img />').attr({ 'src': this.imgUrl, 'alt': this.text, 'title': this.text });

                li.append(d);
                d.append(this.img);
            }
            else {
                li.append('<ul class="vis-appsaction-ul-inner"><li><img src="' + this.imgUrl + '" title="' + this.text + '" /></li><li><span>' + this.text + '</span></li></ul>');
                this.img = li.find("img");
            }



            this.$li = li;

            if (listId) {
                this.items[listId] = li;
            }

            return this.$li;
        };


        this.getListItmIT = function (listId) {
            if (this.$li)
                return this.$li;

            var li = $("<li>");
            var d = $("<div></div>");
            var fired = true;
            li.on(VIS.Events.onClick, function (e) {
                e.stopPropagation();
                if (fired && that.onAction && that.isEnabled) {
                    if (that.toggle) {
                        that.setPressed(!that.pressed);
                    }
                    fired = false;
                    d.css('background-color', 'red')
                    setTimeout(function () {
                        d.css('background-color', 'transparent');
                        that.onAction(that.action);
                        fired = true;
                    }, 10);

                }
            });


            if (this.textOnly) {
                li.text(this.text);
            }
            else if (this.imageOnly) {
                this.img = $('<img />').attr({ 'src': this.imgUrl, 'alt': this.text, 'title': this.text });

                li.append(d);
                d.append(this.img);
            }
            else {
                li.append('<img src="' + this.imgUrl + '"  title="' + this.text + '"   /><span> ' + this.text + '</span>');
                this.img = li.find("img");
            }



            this.$li = li;

            if (listId) {
                this.items[listId] = li;
            }

            return this.$li;
        }

        this.disposeComponent = function () {
            that = null;
            this.getListItm = null;
            this.getListItmIT = null;
        }
    };

    AppsAction.prototype.setPressed = function (pressed) {
        if (!this.toggle)
            return;
        this.pressed = pressed;
        if (this.img) {
            if (this.toggle)
                this.img.prop("src", !pressed ? this.imgUrl : this.imgUrlX);
        }
    };

    AppsAction.prototype.getIsPressed = function () {
        return this.pressed;
    };

    AppsAction.prototype.getAction = function () {
        return this.action;
    };

    AppsAction.prototype.getPath = function () {
        return VIS.Application.contextUrl + "Areas/VIS/Images/base/";
    };

    AppsAction.prototype.getSeprator = function (isSamll, pipe) {
        var src = this.getPath();
        if (isSamll) {
            src += "seprator16.png";
        }
        else {
            src += "seprator24.png";
        }
        if (pipe)
            return "<li>|</li>";

        return '<li><img src="' + src + '"></li>';
    };

    AppsAction.prototype.setEnabled = function (enable) {
        this.isEnabled = enable;
        if (this.$li) {
            this.$li.css("opacity", enable ? 1 : .6);
        }

        if (this.img) {
            this.img.prop("disabled", !enable).css("opacity", enable ? 1 : .6);
            //if (this.enableDisable)
            // this.img.prop("src", enable ? this.imgUrl : this.imgUrlX);
        }
    };

    AppsAction.prototype.show = function () {
        //this.isEnabled = enable;
        if (this.$li) {
            this.$li.show();
        }

        if (this.img) {
            this.img.show();
            //if (this.enableDisable)
            // this.img.prop("src", enable ? this.imgUrl : this.imgUrlX);
        }
    };

    AppsAction.prototype.hide = function () {
        //this.isEnabled = enable;
        if (this.$li) {
            this.$li.hide();
        }

        if (this.img) {
            this.img.hide();
            //if (this.enableDisable)
            // this.img.prop("src", enable ? this.imgUrl : this.imgUrlX);
        }
    };

    /*  Get Item
    * --
    */
    AppsAction.prototype.getItem = function () {
        return this.$li;
    };

    AppsAction.prototype.getIsEnabled = function () {
        return this.isEnabled;
    };

    AppsAction.prototype.dispose = function (id) {
        this.action = this.toggle = this.pressed = this.isEnabled = this.text = this.toolTipText = this.imageOnly = null;
        this.textOnly = this.onAction = this.isSmall = null;
        if (this.$li) {
            this.$li.off(VIS.Events.onClick);
            this.$li.remove();
            this.$li = null;
        }

        if (this.img) {
            this.img = null;
        }

        if (id) {
            if (id in this.items) {
                //console.log(id);
                var val = this.items[id];
                if (val) {
                    val.off(VIS.Events.onClick);
                    val.remove();
                    val = null;
                }
                delete this.items[id];
            }
        }
        this.disposeComponent();
    };

    AppsAction.prototype.setIsRo;




    //****************** END ********************//



    //****************************************************//
    //**            StatusBar                    **//
    //**************************************************//

    function StatusBar(withInfo) {

        this.$statusLine = $("<span>");
        this.$statusDB = $("<span class='vis-statusbar-statusDB'>").text("0/0");
        this.$infoLine = $("<span class='vis-statusbar-infoLine'>").text("info");

        var $spanPageResult = $("<span class='vis-statusbar-pageMsg'>");
        var $seprator = $("<img class='vis-statusbar-img' src= '" + VIS.Application.contextUrl + "Areas/VIS/Images/base/seprator16.png" + "'>");
        var $comboPage = $("<select class='vis-statusbar-combo'>");

        this.text = "";
        this.dse;
        this.merror;
        this.mtext;
        //

        var showText = VIS.Msg.getMsg("ShowingResult");
        var ofText = VIS.Msg.getMsg("of");

        var $root, r1Col1, r2Col1, $ulPages, r2Col2, r2Col3;
        this.onComboChange;

        function initilizeComponent() {
            $root = $("<table class='vis-statusbar-table'>");
            r1Col1 = $("<td style='text-align:center' colspan='3' >")
            r2Col1 = $("<td style='width:auto;white-space:nowrap;'>");
            r2Col2 = $("<td  style='width:100%;'>");
            r2Col3 = $("<td style='width:auto;white-space:nowrap;'>");
            $ulPages = $("<ul class='vis-statusbar-ul'>");
        };

        initilizeComponent();
        //page Numbers
        $ulPages.append($("<li>").append(this.$statusDB));
        r2Col1.append($spanPageResult).append($seprator);
        r2Col2.append($("<div class='vis-statusbar-statusLine'>").append(this.$statusLine));
        r2Col3.append($ulPages);
        r1Col1.append(this.$infoLine);

        if (!withInfo) {
            this.$infoLine.hide();

        }


        /* Previlige function */
        this.getRoot = function () {
            return $root;
        };

        this.render = function () {
            $root.empty();
            $root.append($("<tr class='vis-height-auto'>").append(r1Col1)).append($("<tr class='vis-height-full'>").append(r2Col1).append(r2Col2).append(r2Col3));
        };

        this.setPageItem = function (item) {
            $ulPages.append(item);
        };

        this.setComboPage = function () {
            $ulPages.append($("<li>").append($comboPage));
        };

        this.setPageLine = function (dse) {
            if (dse != null) {
                var cp = dse.getCurrentPage();
                var tp = dse.getTotalPage();
                var ps = dse.getPageSize();
                var tr = dse.getTotalRecords();

                var s = (cp - 1) * ps;
                var e = s + ps;
                if (e > tr) e = tr;

                var text = showText + " " + (s + 1) + "-" + e + " " + ofText + " " + dse.getTotalRecords();

                $spanPageResult.text(text);

                if (tp != $comboPage[0].options.length) {
                    var output = [];
                    var selIndex = -1;
                    for (var i = 0; i < tp; i++) {
                        output[i] = '<option>' + (i + 1) + '</option>';
                    }
                    $comboPage.empty();
                    $comboPage.html(output.join(''));
                }
                if ($comboPage[0].selectedIndex != (cp - 1)) {
                    $comboPage[0].selectedIndex = (cp - 1);
                }
            }
        };

        this.setComboCallback = function (callback) {
            selectionCallback = callback;
        };

        var self = this;
        $comboPage.on("change", function (e) {
            e.stopPropagation();
            if (self.onComboChange) {
                self.onComboChange(this.selectedIndex);
            }
        });

        this.$statusDB.on("click", function () {
            var title = VIS.Msg.getMsg("Who") + self.text;
            var r = new VIS.RecordInfo(title, self.dse);
            r.show();

        });

        this.disposeComponent = function () {
            self = null;
            $root.remove();
            r1Col1 = null;
            r2Col1 = null;
            $root = null;
            this.getRoot = null;
            this.onComboChange = null;
            $ulPages.remove();
            $ulPages = null;

            $spanPageResult = null;
            this.$statusLine = null;
            this.$statusDB = null;
            $seprator = null;

            this.disposeComponent = null;

            this.setPageItem = null;
            this.getRoot = null;
            this.render = null;
            this.setPageLine = null;

            this.$infoLine = null;

            this.text = null;
            this.dse = null;
            this.merror = null;
            this.mtext = null;
            $comboPage.empty();
            $comboPage.remove();
            $comboPage = null;
        };
    };

    StatusBar.prototype.setStatusDB = function (text, dse) {

        if (text == null || text.length == 0) {
            this.$statusDB.text("");
        }
        else {
            this.$statusDB.text(text);
            //if (!statusDB.isVisible())
            //    statusDB.setVisible(true);
        }

        //  Save
        this.text = text;
        this.dse = dse;
        this.setPageLine(dse);
    };	//	setStatusDB

    StatusBar.prototype.setStatusLine = function (text, error) {
        this.merror = error;
        this.mtext = text;
        //if (error)
        //    statusLine.setForeground(red);
        //else
        //    statusLine.setForeground(black);
        this.$statusLine.text(text);
    };//




    StatusBar.prototype.setInfo = function (text) {
        if (!this.$infoLine.is(':visible')) {
            this.$infoLine.show(); //infoLine.setVisible(true);
        }
        this.$infoLine.text(text);
    }	//	set

    StatusBar.prototype.dispose = function () {
        this.disposeComponent();
    };

    //****************************************************//
    //**            End                    **//
    //**************************************************//



    //****************************************************//
    //**            Grid Controller                    **//
    //**************************************************//
    VIS.GridController = function (showRowNo, doPaging, id) {

        this.id = id;
        this.vGridPanel = new VIS.VGridPanel();
        this.vTable = new VIS.VTable();
        this.vCardView = new VIS.VCardView();
        this.vMapView = new VIS.VMapView();
        this.windowNo = 0;
        this.aPanel = null;
        this.singleRow = false;
        this.isCardRow = false;
        this.doPaging = doPaging;
        this.vIncludedGC = null;
        this.m_tree = null;

        this.onRowInserted = null;
        this.onRowInserting = null;
        this.curTabPanel = null;
        this.ul_tabPanels = null;




        this.rightPaneLinkItems = [];
        this.leftPaneLinkItems = [];

        this.showClient = false;
        this.showOrg = false;

        var level = VIS.Env.getCtx().getShowClientOrg();

        if (level == VIS.Env.SHOW_CLIENT_ONLY) {
            this.showClient = true;

        }
        else if (level == VIS.Env.SHOW_ORG_ONLY) {
            this.showOrg = true;
        }
        else if (level == VIS.Env.SHOW_CLIENT_ORG) {
            this.showOrg = true;
            this.showClient = true;
        }

        this.isParentDetailVisible = false; //gc has parent detail panel used in swutch row presentation

        this.isIncludedGCVisible = false; // Is Include Grid  Visible or Not

        this.displayAsIncludedGC = false; // is this GC act as IncludedGrid in other GC

        var $divPanel, $divCard, $divMap, $tabControl, $tableMain, $divHeader, tabItems = [];  //layout
        var td1_tr1, td1_tr2, td1_tr3, $divGrid, $divTree, $divContent, $divMain, $td0_tr3;
        var $layout = null;

        var aAdd, aEdit = null; //toolbar action

        function initlizeComponent() {

            //            $tableMain = $("<div class='vis-height-full'>").hide();

            td1_tr1 = $("<td colspan='2' class='vis-height-auto'>");
            td1_tr2 = $("<td colspan='2' class='vis-height-auto'>");
            td1_tr3 = $("<td style='width:100%'>");

            /* Tree Div */
            $divTree = $("<div>"); //tree div

            $td0_tr3 = $("<td>").append($divTree).hide();

            $tableMain = $("<table class='vis-gc-table'>").append($("<tr>").append(td1_tr1))
                .append($("<tr>").append(td1_tr2))
                .append($("<tr  class='vis-height-full'>").append($td0_tr3).append(td1_tr3)).hide();

            /* Tab Control */
            $tabControl = $("<ul  class='vis-appsaction-ul vis-gc-tabcontrol'>").hide();
            /* End */

            /*divHeader*/
            $divHeader = $("<div class='vis-gc-header'>").hide();
            /*end*/

            /* Multi,card and single view */
            $divGrid = $("<div class='vis-gc-vtable'>");
            $divPanel = $("<div class='vis-gc-vpanel' id='AS_" + id + "'>");
            $divCard = $("<div class='vis-gc-vcard'>");
            $divMap = $("<div class='vis-gc-vmap'>");
            /* End */

            td1_tr1.append($divHeader); //first Row
            td1_tr2.append($tabControl); //Second Row

            $divContent = $("<div class='vis-height-full' style='overflow:hidden'>"); //Main Contant
            $divMain = $("<div class='vis-height-full'>");
            $divContent.append($divGrid).append($divPanel).append($divCard).append($divMap);
            td1_tr3.append($divContent);

        }

        initlizeComponent();

        var self = this;

        var onsubToolBarClick = function (action) {
            //console.log(action);

            if (action == "Edit_sub") {
                if (self.displayAsIncludedGC) {
                    //fire Tab changed and open in edit mode
                    if (self.aPanel.tabActionPerformed(self.id)) {
                        self.switchSingleRow();
                        $tabControl.find('.vis-apanel-tab-selected')[0].scrollIntoView();
                    }
                    return;
                }
            }
            else {
                if (self.displayAsIncludedGC) {
                    //fire Tab changed and open in edit mode
                    if (!self.aPanel.tabActionPerformed(self.id))
                        return;
                    self.switchSingleRow();
                    //self.aPanel.cmd_new();
                    // return;
                    setTimeout(function (t) {
                        t.aPanel.cmd_new()
                    }, 500, self);
                }
            }
        };

        function createToolbar() {

            aAdd = new VIS.AppsAction({ action: "Add_sub", parent: null, enableDisable: true, toggle: false, imageOnly: true, isSmall: true, onAction: onsubToolBarClick }); //Create Apps Action
            aEdit = new VIS.AppsAction({ action: "Edit_sub", parent: null, enableDisable: true, toggle: false, imageOnly: true, isSmall: true, onAction: onsubToolBarClick }); //Create Apps Action
            $tabControl.append(aEdit.getListItm()).append(aAdd.getListItm());
        };
        createToolbar();

        this.initLayout = function () {
            //console.log(this.id);
            //var pstyle = 'border: 1px solid #dfdfdf; padding: 0px;';
            //var pstyle = 'padding: 0px;background-color:transparent;';

            //var panels = [];
            //if (this.m_tree != null) {
            //    panels.push({ type: 'left', size: 250, style: pstyle, resizable: true, content: this.m_tree.getRoot() });
            //}
            //panels.push({ type: 'main', style: pstyle, content: $divContent });

            //$layout = $divMain.w2layout({
            //    name: 'layout_' + id,
            //    panels: panels,
            //    resizer: 3,
            //});

            //$divMain.w2render($layout['name']);
            this.layoutLoaded = true;
        };

        this.sizeChanged = function (height, width) {
            /* SetHeight */
            if (!height) {
                height = VIS.Env.getScreenHeight() - (AWINDOW_HEADER_HEIGHT + APANEL_HEADER_HEIGHT + APANEL_FOOTER_HEIGHT);
            }
            $tableMain.height(height);
            $divGrid.height(height - 2);
            $divPanel.height(height);
            $divTree.height(height);
            $divCard.height(height);
            $divMap.height(height);
            if (this.m_tree) {
                this.m_tree.setSize(height, width);
            }
            if (this.vCardView)
                this.vCardView.sizeChanged(height, $divCard.width());
            if (this.vMapView)
                this.vMapView.sizeChanged(height, $divMap.width());

        };

        this.sizeChanged();
        $divPanel.append(this.vGridPanel.getRoot()); //apaend Single Layout

        this.getRoot = function () {
            return $tableMain;
        };




        this.getTreeArea = function () {
            return $divTree;
        };

        this.setTreePanelWidth = function (width) {
            $td0_tr3.show();
        };


        this.getId = function () {
            return id;
        };

        this.getReocrdDiv = function () {
            return $divHeader;
        };

        this.getTabControl = function () {
            return $tabControl;
        };

        this.setRecord = function (record) {

            // $divRecords.empty();
            // $divRecords.html(record + " " + VIS.Msg.getMsg("Results"));
        };

        this.getVTablePanel = function () {
            return $divGrid;
        };

        this.getVPanel = function () {
            return $divPanel;
        };

        this.getVCardPanel = function () {
            return $divCard;
        };

        this.getVMapPanel = function () {
            return $divMap;
        };

        //  this.setRecord(0);

        this.setUI = function (isIncluded) {
            if (isIncluded) {
                $divHeader.html(this.gTab.getName());
                $divHeader.css('white-space', 'nowrap');
                $divHeader.show();
                $tabControl.show();
                aEdit.setEnabled(false);
                this.vTable.grid.show.selectColumn = false;
            }
            else {
                $divHeader.hide();
                $tabControl.hide();
                this.vTable.grid.show.selectColumn = true;
            }
        };

        this.enableDisableToolbarItems = function (isEnable) {
            aEdit.setEnabled(isEnable);
        };

        //Bind Table Event
        this.vTable.onSelect = function (event) {

            if (self.aPanel && self.aPanel.setBusy) {
                self.aPanel.setBusy(true);
            }
            self.cancelSel = false;
            //var cRow = -1, nRow;
            //if (self.gTab.needSave(true, false))
            //    cRow = self.gTab.getCurrentRow();

            self.onTableRowSelect(event);

            if (self.cancelSel)
                event.isCancelled = true;
            else {
                //    nRow = self.gTab.getCurrentRow();
                //  if (cRow != -1 && cRow != nRow)
                ///    setTimeout(function (t, r) {
                //     t.refreshRow(r); //refresh old row
                // }, 10, this, cRow);
            }
            if (self.aPanel && self.aPanel.setBusy) {
                self.aPanel.setBusy(false);
            }
        };


        this.vTable.onCellValueChanged = function (event, invokeReq) {
            if (invokeReq)
                window.setTimeout(function () {
                    self.vetoablechange(event);
                    self.vTable.refreshCells();
                }, 10);
            else {
                self.vetoablechange(event);
                self.vTable.refreshCells();
            }
        };

        this.vCardView.onCardEdit = function (event, onlySelect) {
            self.onTableRowSelect(event);
            //switch self.singleRow = false; //force single view
            if (!onlySelect)
                self.switchSingleRow();
        };

        //On Sort event
        this.vTable.onSort = function (event) {


            //this.vTable.getGrid().records
            window.setTimeout(function () {
                self.navigate(self.gTab.getCurrentRow(), true);
            }, 10);
            // console.log(self.vTable.getGrid().records);
            // console.log(self.gTab.getRecords());
            // console.log(self.gTab.getTableModel().getSortModel());
        };

        //show single layout
        //this.vTable.onEdit = function (recid) {
        //    // if (self.singleRow)
        //    //  return true;
        //    // if (self.vTable.getSelection().length < 1)
        //    // return;

        //    if (self.displayAsIncludedGC) {
        //        //fire Tab changed and open in edit mode
        //        self.aPanel.tabActionPerformed(tabItems[self.selTabIndex].action);
        //        self.switchSingleRow();
        //        return;
        //    }

        //    self.switchRowPresentation();
        //};


        //this.vTable.onAdd = function (recid) {
        //    // if (self.singleRow)
        //    //  return true;
        //    // if (self.vTable.getSelection().length < 1)
        //    // return;

        //    if (self.displayAsIncludedGC) {
        //        //fire Tab changed and open in edit mode
        //        self.aPanel.tabActionPerformed(tabItems[self.selTabIndex].action);
        //        self.switchSingleRow();
        //        //self.aPanel.cmd_new();
        //        // return;
        //    }
        //    setTimeout(function (t) {
        //        t.aPanel.cmd_new()
        //    }, 500, self);
        //};


        ////Called by editor controls
        //this.vetoablechangeListner = function (event) {
        //    self.vetoablechangeHandler(event);
        //}

        this.disposeComponent = function () {

            //$divRecords.off("tap click");

            this.rightPaneLinkItems.length = 0;
            this.rightPaneLinkItems = null;
            this.leftPaneLinkItems.length = 0;
            this.leftPaneLinkItems = null;


            $divGrid = null;
            $divRecords = null;
            //tabItems.length = 0;

            for (var i = 0; i < tabItems.length; i++) {
                tabItems[i].dispose("ul_" + this.id);
            }

            tabItems = null;
            this.seletedTab = null;
            td1_tr1 = null;
            td1_tr2 = null;
            //td1_tr3 = null;
            this.vGridPanel.dispose();
            this.vGridPanel = null;

            this.vCardView.dispose();
            this.vCardView.onSelect = null;

            this.vMapView.dispose();


            this.vTable.dispose();
            this.vTable.onSelect = null;
            this.vTable.onSort = null;
            this.vTable = null;

            $divGrid = null;
            $divPanel = null;
            $divCard = null;
            $divMap = null;
            self = null;
            this.getId = null;
            this.getReocrdDiv = null;
            this.getRoot = null;
            this.getVTablePanel = null;
            this.getVPanel = null;
            this.getVCardPanel = null;
            $tableMain.remove();
            //console.log($tableMain);
            $tableMain = null;
            if ($layout)
                $layout.destroy();
            $layout = null;
            this.onRowInserted = null;
        };
    };


    VIS.GridController.prototype.createTabPanel = function (panels) {
        if (!this.ul_tabPanels) {
            if (panels && panels.length > 0) {
                var str = '';
                for (var i = 0; i < panels.length; i++) {
                    var iconPath = '';
                    if (panels[i].getIconPath()) {
                        iconPath = panels[i].getIconPath();
                    }
                    else {
                        iconPath = 'VIS/Images/base/defPanel.ico';
                    }

                    str += '<li ><img alt="' + panels[i].getName() + '" title="' + panels[i].getName() + '" default="' + panels[i].getIsDefault() + '" data-panelID="' + panels[i].getAD_TabPanel_ID() + '" data-cName="' + panels[i].getClassName()
                        + '" data-Name="' + panels[i].getName() + '" data-extrainfo="' + panels[i].getExtraInfo() + '" src="' + VIS.Application.contextUrl + 'Areas/' + iconPath + '"></img></li>';
                }
                this.ul_tabPanels = str;
            }
        }
        else {
            this.ul_tabPanels = this.ul_tabPanels.replace('default="true"', '');
        }
        return this.ul_tabPanels;
    };

    VIS.GridController.prototype.setCurrentPanelID = function (ID) {
        this.panelID = ID;
    };

    VIS.GridController.prototype.getCurrentPanelID = function () {
        return this.panelID;
    };

    VIS.GridController.prototype.setCurrentPanel = function (className, windowNo) {

        if (this.curTabPanel) {
            if (this.curTabPanel.dispose) {
                this.curTabPanel.dispose();
            }
            this.curTabPanel = null;
        }

        if (className) {
            var type = VIS.Utility.getFunctionByName(className, window);
            if (type) {
                var panel = new type();
                panel.startPanel(windowNo, this.gTab);
                if (this.gTab.getRecord_ID() > -1) {
                    panel.refreshPanelData(this.gTab.getRecord_ID(), this.gTab.getTableModel().getRow(this.gTab.getCurrentRow()));
                }
                this.curTabPanel = panel;
            }
        }
    };

    VIS.GridController.prototype.getCurrentPanel = function (panel) {
        return this.curTabPanel;
    };

    VIS.GridController.prototype.refreshTabPanelData = function (record_ID) {
        if (this.curTabPanel && $(this.curTabPanel.getRoot()).is(':visible')) {
            this.curTabPanel.refreshPanelData(record_ID, this.gTab.getTableModel().getRow(this.gTab.getCurrentRow()));
        }
    };


    /** initGrid
      * - Map table to model
        - Update (multi-row) table info with renderers/editors
        - build single-row panel
        - initialize display
    @param onlyMultirow
    @param curwindowNumber
    @name  aPanel
    @name mTab
    <returns></returns>*/
    VIS.GridController.prototype.initGrid = function (onlyMultiRow, curWindowNo, aPanel, mTab) {

        var fields = mTab.gridTable.gridFields;
        var mField = null;
        var vGridId = curWindowNo + "_" + mTab.vo.AD_Tab_ID;
        var vCardId = curWindowNo + "_c" + mTab.vo.AD_Tab_ID;
        var vMapId = curWindowNo + "_m" + mTab.vo.AD_Tab_ID;

        mTab.getTableModel().setDoPaging(this.doPaging);

        //bindingSource =  tab.GetDataTable();
        //var bindingSource = null;
        var role = VIS.MRole;
        if (!role.getIsDisplayClient() || !this.showClient)
            mTab.getField("AD_Client_ID").setDisplayed(false);
        if (!role.getIsDisplayOrg() || !this.showOrg)
            mTab.getField("AD_Org_ID").setDisplayed(false);



        var size = this.vTable.setupGridTable(aPanel, fields, this.getVTablePanel(), vGridId, mTab, this);


        mTab.addDataStatusListener(this);

        if (!onlyMultiRow) {

            this.vCardView.setupCardView(aPanel, mTab, this.getVCardPanel(), vCardId);

            if (mTab.getIsMapView())
                this.vMapView.setupMapView(aPanel, this, mTab, this.getVMapPanel(), vMapId);



            for (var i = 0; i < size; i++) {
                mField = fields[i];

                if (mField.getIsDisplayed()) {
                    var iControl = VIS.VControlFactory.getControl(mTab, mField, false, false, false);
                    if (iControl == null && !mField.getIsHeading()) {
                        //log.warning("Editor not created for " + mField.getColumnName());
                        continue;
                    }
                    if (iControl != null) {
                        //  MField => VEditor - New Field value to be updated to editor
                        iControl.setReadOnly(true);
                        mField.setPropertyChangeListener(iControl);
                        //  VEditor => this - New Editor value to be updated here (MTable)
                        iControl.addVetoableChangeListener(this);
                    }
                    this.vGridPanel.addField(iControl, mField);

                    if (iControl instanceof VIS.Controls.VButton) {

                        if (mField.getIsLink()) {
                            if (mField.getIsRightPaneLink()) {
                                this.rightPaneLinkItems.push(iControl);
                            }
                            else {
                                this.leftPaneLinkItems.push(iControl);
                            }
                        }
                        iControl.addActionListner(aPanel);
                    }

                    iControl = null;
                }
            }
        }


        //  Tree Graphics Layout
        var AD_Tree_ID = 0;

        if (mTab.getIsTreeTab()) {
            //, Name
            var sql = "VIS_120";

            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Client_ID", VIS.Env.getCtx().getAD_Client_ID());
            param[1] = new VIS.DB.SqlParam("@AD_Table_ID", mTab.getAD_Table_ID());
            AD_Tree_ID = executeScalar(sql, param);

            //if (AD_Tree_ID > 0) {
            //    this.m_tree = new VIS.TreePanel(curWindowNo, false, true);
            //    //Set Style
            //    if (mTab.getTabNo() == 0)	//	initialize other tabs later
            //    {
            //        this.m_tree.initTree(AD_Tree_ID);
            //    }

            //    this.getTreeArea().append(this.m_tree.getRoot());
            //    this.m_tree.addSelectionChangeListner(this);
            //    this.setTreePanelWidth("300px");
            //    this.getTreeArea().width("300px");
            //    this.m_tree.setSize(this.getTreeArea().height());

            //}
            if (AD_Tree_ID > 0) {
                this.treeID = AD_Tree_ID;
                if (mTab.getShowSummaryLevel()) {
                    this.onDemandTree = true;
                    this.m_tree = new VIS.TreePanel(curWindowNo, false, true, true, this);
                    aPanel.aShowSummaryLevel.show();

                }
                else {
                    this.onDemandTree = false;
                    this.m_tree = new VIS.TreePanel(curWindowNo, false, true, false, this);
                    aPanel.aShowSummaryLevel.hide();
                }
                this.m_tree.setTabID(mTab.getAD_Tab_ID());
                //Set Style
                if (mTab.getTabNo() == 0)	//	initialize other tabs later
                {
                    this.m_tree.initTree(AD_Tree_ID);
                }

                this.getTreeArea().append(this.m_tree.getRoot());
                this.m_tree.addSelectionChangeListner(this);



                this.setTreePanelWidth("300px");
                this.getTreeArea().width("300px");
                this.m_tree.setSize(this.getTreeArea().height());

            }
            else    //  No Graphics - hide
            {

            }
        }

        //ADD Table Model Event Listner
        mTab.getTableModel().addTableModelListener(this.vTable);
        mTab.getTableModel().addCardModelListener(this.vCardView);


        //ADD Table Model Event Listner
        mTab.getTableModel().addRowChangedListener(this);

        //AddQueryCompleteListner
        mTab.getTableModel().addQueryCompleteListner(this);

        mTab.getTableModel().setDoPaging(this.doPaging);
        mTab.getTableModel().setCurrentPage(1);

        this.gTab = mTab;
        this.windowNo = curWindowNo
        this.onlyMultiRow = onlyMultiRow;
        this.aPanel = aPanel;

        //  Set initial presentation
        if (onlyMultiRow || !mTab.getIsSingleRow()) {
            // this.switchMultiRow();
            this.singleRow = false;
            //this.switchMultiRow();
        }
        else
            this.switchSingleRow(true);
    };

    VIS.GridController.prototype.detachDynamicAction = function () {
        var i = 0;
        var j = 0;

        for (var i = 0, j = this.leftPaneLinkItems.length; i < j; i++) {
            this.leftPaneLinkItems[i].getControl().detach();
        }
        for (i = 0, j = this.rightPaneLinkItems.length; i < j; i++) {
            this.rightPaneLinkItems[i].getControl().detach();
        }
        i = null;
        j = null;
    };

    VIS.GridController.prototype.switchRowPresentation = function () {

        if (this.singleRow)
            this.switchMultiRow();
        else
            this.switchSingleRow();
    };

    VIS.GridController.prototype.getIsSingleRow = function () {
        return this.singleRow;
    };

    VIS.GridController.prototype.getIsMapRow = function () {
        return this.isMapRow;
    };

    VIS.GridController.prototype.onTableRowSelect = function (event) {

        if (this.rowSetting) { return };
        //  no rows
        if (this.gTab.getRowCount() == 0)
            return;

        //	vTable.stopEditor(graphPanel);
        var rowTable = this.vTable.get(event.recid, true);
        var rowCurrent = this.gTab.getCurrentRow();

        if (rowTable == -1)  //  nothing selected
        {
            if (rowCurrent >= 0) {
                this.vTable.select(event.recid);
                this.vCardView.navigate(event.recid, !this.isCardRow);
                return;
            }
        }
        if (rowTable != rowCurrent) {
            this.rowSetting = true;
            this.navigate(rowTable);
            this.rowSetting = false;
        }
        else if (!this.settingGridSelecton) {
            return;
        }
        this.dynamicDisplay(-1);


        //	TreeNavigation - Synchronize 	-- select node in tree
        //if (m_tree != null)
        //    m_tree.setSelectedNode (m_mTab.getRecord_ID());	//	ignores new (-1)


        var selfThis = this;

        //	Query Included Tab
        if (!this.getIsSingleRow()) {
            window.setTimeout(function () {
                if (selfThis.vIncludedGC != null) {
                    selfThis.switchIncludedGC();
                    //vIncludedGC.getMTab().query(0, 0, false);
                }
            }, 50);
        }
        else {
            if (selfThis.vIncludedGC != null) {
                selfThis.switchIncludedGC();
                //vIncludedGC.getMTab().query(0, 0, false);
            }
        }
        //if (this.currentRowIndex === event.index) {
        //    return;
        //}
        //this.currentRowIndex = event.index;

        //this.gTab.navigate(this.currentRowIndex, true);
        //this.dynamicDisplay(-1);
    };

    VIS.GridController.prototype.switchIncludedGC = function () {
        if (this.vIncludedGC != null) {
            this.switchIncludedGC();
            //vIncludedGC.getMTab().query(0, 0, false);
        }
    };

    VIS.GridController.prototype.dynamicDisplay = function (col) {
        //if (!this.getIsSingleRow() || this.onlyMultiRow)
        //    return;
        if (this.gTab == null)
            return;

        if (this.isCardRow || this.isMapRow) {
            return;
        }


        var recID = this.gTab.getRecord_ID();

        if (!this.getIsSingleRow()) {
            this.dynamicDisplayLinks(col);
            //if (recID == -1) {
            //    this.setDefaultFocus();
            //}
            return;
        }


        if (!this.gTab.getIsOpen())
            return;
        if (col >= 0) {
            var changedField = this.gTab.getField(col);
            var columnName = changedField.getColumnName();
            var dependants = this.gTab.getDependantFields(columnName);
            //log.config("(" + m_mTab.toString() + ") "
            //	+ columnName + " - Dependents=" + dependants.size());
            //	No Dependents and no Callout - Set just Background
            if (dependants.length == 0 && changedField.getCallout().length > 0) {
                // List<Control> comp = currentTab.GetControls();
                //for (var i = 0; i < vPanel.allControls.Count; i++)
                // {
                //if (columnName.equals(vPanel.allControls[i].Name))
                //{
                // vPanel.allControls[i].BackColor = Color.LightSkyBlue;
                //ve.setBackground( changedField.isError() );
                //   break;
                //}
                // }
                return;
            }
        }

        //  complete single row re-display
        var noData = this.gTab.getRowCount() == 0;
        //log.config(m_mTab.toString() + " - Rows=" + m_mTab.getRowCount());
        //  All Components in vPanel (Single Row)
        var compos = this.vGridPanel.getComponents();
        var size = compos.length;




        for (var i = 0; i < size; i++) {
            var comp = compos[i];
            //IControl compI = (IControl)comp;
            var columnName = comp.getName();

            if (columnName != null) {

                if (columnName.startsWith("lbl")) {
                    columnName = columnName.substring(3);
                }
                var mField = this.gTab.getField(columnName);
                if (mField != null) {
                    if (mField.getIsDisplayed(true)) {		//  check context
                        var vis = comp.tag;
                        if (!comp.getIsVisible() && ((vis == null || vis == "undefined") || vis))
                            comp.setVisible(true);		//  visibility
                        if (comp instanceof VIS.Controls.IControl) {
                            var ve = comp;
                            if (noData)
                                ve.setReadOnly(true);
                            else {
                                //   mField.vo.tabReadOnly = this.gTab.getIsReadOnly();
                                var rw = mField.getIsEditable(true) && !this.gTab.getIsReadOnly();	//  r/w - check Context



                                ve.setReadOnly(!rw);
                                //	log.log(Level.FINEST, "RW=" + rw + " " + mField);
                                ve.setMandatory(mField.getIsMandatory(true));
                                //mField.validateValue();
                                ve.setBackground(mField.getIsError());

                                // Check if new record and current field marked as default field and focus is not set yet, 
                                // then set focus.
                                if (mField.getIsDefaultFocus() && !this.isDefaultFocusSet && !comp.getName().startsWith("lbl")) {
                                    ve.setDefaultFocus(true);
                                    this.isDefaultFocusSet = true;
                                }

                            }
                        }
                    }
                    else if (comp.getIsVisible())
                        comp.setVisible(false);

                    // reset error status for nondisplayed fields if they are not mandatory
                    if (!mField.getIsDisplayed(true) && !mField.getIsMandatory(true)) {
                        mField.setError(false);
                    }
                }
            }
        }
    };

    VIS.GridController.prototype.dynamicDisplayLinks = function (col) {
        if (this.displayAsIncludedGC) return;
        //  complete single row re-display
        var noData = this.gTab.getRowCount() == 0;
        var linkArr = this.vGridPanel.getLinkComponents();

        for (var i = 0; i < linkArr.length; i++) {
            var comp = linkArr[i];
            //IControl compI = (IControl)comp;
            var columnName = comp.getName();

            if (columnName != null) {

                if (columnName.startsWith("lbl")) {
                    columnName = columnName.substring(3);
                }
                var mField = this.gTab.getField(columnName);
                if (mField != null) {
                    if (mField.getIsDisplayed(true)) {//  check context
                        var vis = comp.tag;
                        if (!comp.getIsVisible() && ((vis == null || vis == "undefined") || vis))
                            comp.setVisible(true);		//  visibility
                        if (comp instanceof VIS.Controls.IControl) {
                            var ve = comp;
                            if (noData)
                                ve.setReadOnly(true);
                            else {
                                var rw = mField.getIsEditable(true) && !this.gTab.getIsReadOnly();	//  r/w - check Context
                                ve.setReadOnly(!rw);

                                //if (mField.getIsDefaultFocus()) {
                                //    ve.setDefaultFocus(true);
                                //}


                                //	log.log(Level.FINEST, "RW=" + rw + " " + mField);
                                ve.setMandatory(mField.getIsMandatory(true));
                                //mField.validateValue();
                                ve.setBackground(mField.getIsError());
                            }
                        }
                    }
                    else if (comp.getIsVisible())
                        comp.setVisible(false);
                    // reset error status for nondisplayed fields if they are not mandatory
                    if (!mField.getIsDisplayed(true) && !mField.getIsMandatory(true)) {
                        mField.setError(false);
                    }
                }
            }
        }
    };


    //Set Default Focus for grid... Not in use Yet.
    VIS.GridController.prototype.setDefaultFocus = function () {

        var noData = this.gTab.getRowCount() == 0;
        var compos = this.vGridPanel.getComponents();
        var size = compos.length;

        for (var i = 0; i < size; i++) {
            var comp = compos[i];
            //IControl compI = (IControl)comp;
            var columnName = comp.getName();

            if (columnName != null) {
                var mField = this.gTab.getField(columnName);
                if (mField != null) {
                    if (mField.getIsDefaultFocus()) {
                        this.vTable.setDefaultFocus(columnName);
                        break;
                    }
                }
            }
        }


    };

    VIS.GridController.prototype.setVisible = function (visible) {

        if (!this.layoutLoaded) {
            this.initLayout();

        }


        if (visible) {
            this.getRoot().show();
            this.vTable.resize();
        }
        else {
            this.getRoot().hide();
        }
    };

    VIS.GridController.prototype.getMTab = function () {
        return this.gTab;
    };

    VIS.GridController.prototype.getIsDisplayed = function () {
        return this.gTab.getIsDisplayed(false);
    };

    VIS.GridController.prototype.getTabLevel = function () {
        return this.gTab.getTabLevel();
    };

    VIS.GridController.prototype.getTitle = function () {
        return this.gTab.getName();
    };

    VIS.GridController.prototype.getSelection = function (retIndex) {
        return this.vTable.getSelection(retIndex);
    };

    /* 
     * Selected grid Rows 
     *  
     */
    VIS.GridController.prototype.getSelectedRows = function () {
        return this.vTable.getSelectedRows();
    };

    VIS.GridController.prototype.getColumnNames = function () {

        if (this.colNames)
            return this.colNames;

        var fields = this.gTab.getTableModel().getFields();

        var colObj = {};
        for (var i = 0; i < fields.length; i++) {
            colObj[fields[i].getColumnName()] = fields[i].getHeader();
        }

        this.colNames = colObj;
        return this.colNames;
    };

    VIS.GridController.prototype.setMnemonics = function (setNum) {

    };

    VIS.GridController.prototype.activate = function (oldGC) {

        this.vTable.activate();
        oldGC = oldGC || {};

        //if (oldGC.isIncludedGCVisible) { //Hide Included of Old GC
        //    oldGC.vIncludedGC.setVisible(false);
        //}
        this.vTable.setReadOnly(false);

        if (this.displayAsIncludedGC) {
            var tdArea = this.aPanel.getLayout();
            this.setUI(false);
            this.getRoot().detach();
            tdArea.append(this.getRoot());
            this.displayAsIncludedGC = false;
            this.aPanel.getIncludedEmptyArea().css({ 'width': '', "padding": '' });
        }
        else if (this.gTab.getIncluded_Tab_ID() == 0) {
            var olcIncludedTab = oldGC.vIncludedGC;
            if (olcIncludedTab) {
                var tdArea = olcIncludedTab.aPanel.getLayout();
                olcIncludedTab.setUI(false);
                olcIncludedTab.getRoot().detach();
            }
            //tdArea.append(oldGC.getRoot());
            //oldGC.displayAsIncludedGC = false;
            //oldGC.aPanel.getIncludedEmptyArea().css({ 'width': '', "padding": '' });

            //old.switchIncludedGC();
        }
        //vIncludedGC
        this.isIncludedGCVisible = false;
        // || this.isIncludedGCVisible || oldGC.vIncludedGC) // this is Shown as Included GC then reset parameter
        //{
        //    if (!this.isIncludedGCVisible) {
        //        this.getRoot().width("100%");
        //    }

        //    //oldGC.isIncludedGCVisible = false;
        //    this.displayAsIncludedGC = false;
        //    this.isIncludedGCVisible = false;
        //    this.toggleTabItems();
        //}

        if (this.vIncludedGC) { // has included GC
            //  this.vIncludedGC.vTable.activate();
            this.vIncludedGC.displayAsIncludedGC = false;
            this.vIncludedGC.isIncludedGCVisible = false;

        }

        this.activateTree();


        //this.switchMultiRow();
        //oldGC = null;
    };

    VIS.GridController.prototype.multiRowResize = function () {
        if (!this.singleRow && !this.isCardRow)
            this.vTable.resize();
    };

    /* TREE */
    /*
     * 	Activate Grid Controller.
     * 	Called by APanel when GridController is displayed (foreground)
     */
    VIS.GridController.prototype.activateTree = function () {
        //	Tree to be initiated on second/.. tab
        if (this.gTab.getIsTreeTab() && (this.gTab.getTabNo() > 0)) {
            var AD_Tree_ID = 0;
            if (this.gTab.getTabLevel() > 0)	//	check previous tab for AD_Tree_ID
            {
                var keyColumnName = this.gTab.getKeyColumnName();
                var treeName = "AD_Tree_ID";
                if (keyColumnName.startsWith("CM")) {
                    if (keyColumnName.equals("CM_Container_ID"))
                        treeName = "AD_TreeCMC_ID";
                    else if (keyColumnName.equals("CM_CStage_ID"))
                        treeName = "AD_TreeCMS_ID";
                    else if (keyColumnName.equals("CM_Template_ID"))
                        treeName = "AD_TreeCMT_ID";
                    else if (keyColumnName.equals("CM_Media_ID"))
                        treeName = "AD_TreeCMM_ID";
                }
                AD_Tree_ID = VIS.Env.getCtx().getWindowContextAsInt(this.windowNo, treeName);
                //log.config(keyColumnName + " -> " + treeName + " = " + AD_Tree_ID);
            }
            if (AD_Tree_ID == 0) {

                var AD_Table_ID = this.gTab.getAD_Table_ID();
                var AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();

                if (AD_Table_ID == 0)
                    return 0;

                //var dr = executeReader("SELECT AD_Tree_ID, Name FROM AD_Tree "
                //    + "WHERE AD_Client_ID=" + AD_Client_ID + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                //    + "ORDER BY IsDefault DESC, AD_Tree_ID");

                var sql = "VIS_121";
                var param = [];
                param[0] = new VIS.DB.SqlParam("@AD_Client_ID", AD_Client_ID);
                param[1] = new VIS.DB.SqlParam("@AD_Table_ID", AD_Table_ID);
                var dr = executeReader(sql, param);

                if (dr.read()) {
                    AD_Tree_ID = dr.getInt(0);
                }
                dr = null;
                //AD_Tree_ID = 101;
                //MTree.getDefaultAD_Tree_ID(
                //		Env.getCtx().getAD_Client_ID(), m_mTab.getAD_Table_ID());
            }
            if (this.m_tree != null && AD_Tree_ID > 0)
                this.m_tree.initTree(AD_Tree_ID);
        }
    };	//	activate

    VIS.GridController.prototype.nodeSelectionChanged = function (e) {
        //	System.out.println("propertyChange");
        //	System.out.println(e);
        if (e == null)
            return;
        var value = e.newValue;
        if (value == null)
            return;
        //log.config(e.propertyName() + "=" + value

        //  We Have a TreeNode
        var nodeID = value;
        this.treeNodeID = nodeID;
        if (this.onDemandTree) {

            if (this.aPanel && this.aPanel.setBusy) {
                this.aPanel.setBusy(true);
            }

            this.gTab.setTreeNodeID(nodeID);

            this.gTab.gridTable.setCurrentPage(1);
            this.gTab.setQuery(null);
            this.navigate(0);

            this.query(0, 0, false, nodeID, this.treeID, this.gTab.getAD_Table_ID());   //  autoSize
            return;
        }

        //  Search all rows for mode id
        var size = this.gTab.getRowCount();
        var row = -1;
        for (var i = 0; i < size; i++) {
            if (this.gTab.getKeyID(i) == nodeID) {
                row = i;
                break;
            }
        }
        if (row == -1) {
            //this.log.log(Level.WARNING, "Tab does not have ID with Node_ID=" + nodeID);
            return;
        }

        //  Navigate to node row
        this.navigate(row);
    };   //  pro

    /** END*/

    /*
      skip row inserting (used by window qiuck serch )  
      -if Automatic new row record set to true
      @param ignore true or false
     */
    VIS.GridController.prototype.skipRowInserting = function (ignore) {
        this.skipInserting = ignore;
    };

    VIS.GridController.prototype.query = function (onlyCurrentDays, maxRows, created, nodeID, treeID, tableID) {
        if (this.aPanel && this.aPanel.setBusy) {
            this.aPanel.setBusy(true);
        }

        if (this.onDemandTree) {
            if (nodeID) {
                this.gTab.setTreeNodeID(nodeID);
            }
            if (treeID) {
                this.gTab.setTreeID(treeID);
            }
            if (tableID) {
                this.gTab.setTreeTable(tableID);
            }
            if (this.aPanel.isSummaryVisible) {
                this.gTab.setShowSummaryNodes(true);
            }
            else {
                this.gTab.setShowSummaryNodes(false);
            }
        }

        var result = this.gTab.prepareQuery(onlyCurrentDays, maxRows, created, false);
    };

    VIS.GridController.prototype.queryCompleted = function (result) {
        this.vTable.clear();
        this.gTab.clearSelectedRow();
        //
        this.vTable.add(this.gTab.getTableModel().getDataTable());
        this.gTab.getTableModel().setSortModel(this.vTable.getGrid().records);


        if (!this.displayAsIncludedGC) {
            if (this.onRowInserting)
                this.onRowInserting();
            else
                this.checkInsertNewRow();
        }

        //Updated by raghu 
        //date:19-01-2016
        //Change/Update for:Zoom from workflow on home page

        //if (!this.vIncludedGC || this.isIncludedGCVisible || this.isZoomAction || 1==1) {
        this.navigate(this.gTab.getCurrentRow(), !this.gTab.getTableModel().getIsInserting());
        //}
        //else {
        //    this.gTab.currentRow = -1;
        //    this.gTab.fireDataStatusEventOnly();
        //}

        //refresh card view
        if (this.isCardRow)
            this.vCardView.refreshUI(this.getVCardPanel().width());

        //refresh card view
        if (this.isMapRow)
            this.vMapView.refreshUI(this.getVMapPanel().width());

        if (this.aPanel) {
            this.aPanel.setBusy(false);
        }

        this.skipInserting = false; // reset 
        this.dynamicDisplay(-1);
        //if (this.aPanel.$parentWindow.onLoad) {  //Change/Update for:Zoom from workflow on home page
        //    this.aPanel.$parentWindow.onLoad();
        //    this.aPanel.$parentWindow.onLoad = null;
        //}
    };

    VIS.GridController.prototype.checkInsertNewRow = function () {
        if (this.aPanel == null)
            return false;

        var parentValid = true;
        var lc = this.gTab.getLinkColumnName();
        var lcValue = VIS.context.getWindowContext(this.windowNo, lc);
        if (lc.Length > 0 && lcValue.length == 0) {
            parentValid = false;
        }
        //Set Initial record
        //  Set initial record
        if (this.gTab.getTableModel().getTotalRowCount() == 0) {
            //	Automatically create New Record, if none & tab not RO
            if (!this.gTab.getIsReadOnly() &&
                (VIS.context.getIsAutoNew(this.windowNo)
                    || this.gTab.getIsQueryNewRecord()) && parentValid) {
                if (this.gTab.getIsInsertRecord() && !this.skipInserting) {
                    this.dataNew(false);
                    return true;
                }
                else {
                    //aPanel.SetButtons(false, false);
                    //aPanel.SetNavigateButtons();
                }
            }
        }
        //reset
        return false;
    };
    /*
      - Handle Control's Change value Event
    */
    VIS.GridController.prototype.vetoablechange = function (e) {

        ////  Get Row/Col Info
        var mTable = this.gTab.getTableModel();
        var row = this.gTab.getCurrentRow();
        var col = mTable.findColumn(e.propertyName);
        ////
        //// gwu: modified to enforce validation even when the new value      null
        mTable.setValueAt(e.newValue, row, col);	//	-> dataStatusChanged -> dynamicDisplay
        ////	Force Callout
        //if (e.getPropertyName().equals("S_ResourceAssignment_ID")) {
        //    //GridField mField = m_mTab.getField(col);
        //    // if ((mField != null) && (mField.getCallout().length() > 0))
        //    //   m_mTab.processFieldChange(mField);     //  Dependencies & Callout
        //}
    };

    /*
     - handle UI refresh Request 
    */
    VIS.GridController.prototype.refreshUI = function (e) {
        this.dataRefreshAll();
    };

    VIS.GridController.prototype.navigate = function (tRow, force) {

        //  nothing to do
        //console.log(!force);
        if (!force && tRow == this.gTab.getCurrentRow())
            return this.gTab.getCurrentRow();

        //  new position
        var recid = this.gTab.navigate(tRow, true, force);
        if (recid > -1 && !this.rowSetting) {
            this.settingGridSelecton = true;
            this.vTable.select(recid) //select row for Grid
            this.settingGridSelecton = false;
            this.vCardView.navigate(recid, !this.isCardRow);
        }
        else if (recid > -1) {
            this.vCardView.navigate(recid, true);
        }

        if (recid == -1) {
            this.cancelSel = true;
            return;
        }
        if (recid > -1) {
            this.refreshTabPanelData(this.gTab.getRecord_ID());
        }
        //treeselectin
        //	TreeNavigation - Synchronize 	-- select node in tree
        if (this.m_tree != null)
            this.m_tree.setSelectedNode(this.gTab.getRecord_ID());	//	ignores new (-1)

        this.vTable.scrollInView(tRow);

        if (this.onDemandTree) {

            var recID = this.gTab.getRecord_ID();
            var treID = this.treeID;
            var selfApnel = this.aPanel;
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GetTreeNodePath",
                data: { nodeID: recID, treeID: treID },
                success: function (result) {



                    var data = JSON.parse(result);
                    selfApnel.setStatusLine(data);
                },
                error: function (e) {
                    console.log(e);
                }

            });
        }


        return this.gTab.getCurrentRow();
    };

    VIS.GridController.prototype.navigatePageExact = function (newPage) {

        this.gTab.getTableModel().setCurrentPage(newPage);
        //MRole role = MRole.GetDefault();
        this.query(this.gTab.getOnlyCurrentDays(),
            //role.GetMaxQueryRecords(), false);	//	updated
            0, false, this.treeNodeID, this.treeID, this.gTab.getAD_Table_ID());	//	updated
    };

    VIS.GridController.prototype.navigatePage = function (newPage) {

        this.gTab.getTableModel().setCurrentPageRelative(newPage);
        //MRole role = MRole.GetDefault();
        this.query(this.gTab.getOnlyCurrentDays(),
            //role.GetMaxQueryRecords(), false);	//	updated
            0, false, this.treeNodeID, this.treeID, this.gTab.getAD_Table_ID());	//	updated
    };

    VIS.GridController.prototype.navigateRelative = function (rowChange) {
        return this.navigate(this.gTab.getCurrentRow() + rowChange);
    };

    VIS.GridController.prototype.dataRefresh = function () {
        var record = this.gTab.dataRefresh();
        this.dynamicDisplay(-1);
        window.setTimeout(function (t) { t.switchIncludedGC(); t = null }, 500, this);
    };

    VIS.GridController.prototype.dataRefreshAll = function () {

        this.gTab.getTableModel().dataRefreshAll();

        //this.vTable.clear();
        //this.vTable.add(this.gTab.getTableModel().getDataTable());
        //this.gTab.getTableModel().setSortModel(this.vTable.getGrid().records);

        //this.setRecord(this.gTab.getTableModel().getTotalRowCount());

        //this.navigate(this.gTab.getCurrentRow(), true);

    };

    VIS.GridController.prototype.dataSave = function (manualCmd) {

        if (this.m_tree != null) {
            this.gTab.SetSelectedNode(this.m_tree.currentNode);
        }


        var retVal = this.gTab.dataSave(manualCmd);
        if (retVal) {
            //refresh Grid Row
            // this.vTable.refreshRow();
        }
        return retVal;
    };

    VIS.GridController.prototype.dataNew = function (copy) {
        //this.rowSetting = true;
        //this.switchSingleRow();

        // Default Focus should be set only for first time, not for every datastatus Changed. 
        //So to achieve this, a flag is set on every new click.
        this.isDefaultFocusSet = false;

        this.gTab.dataNew(copy);
        this.dynamicDisplay(-1);
        this.switchIncludedGC();
        if (this.onRowInserted) {
            this.onRowInserted();
        }

        this.isDefaultFocusSet = true;
    };

    VIS.GridController.prototype.canDeleteRecords = function () {
        var selIndices = this.vTable.getSelection(true);
        var records = this.vTable.getGrid().records;
        var retIndices = [];

        for (var i = 0; i < selIndices.length; i++) {
            var record = records[selIndices[i]];
            if ("ad_client_id" in record) {
                if (!VIS.MRole.getIsClientAccess(record.ad_client_id, true))
                    retIndices.push(selIndices[i]);
            }
        }
        return retIndices;
    };

    VIS.GridController.prototype.dataDelete = function () {
        var retValue = this.gTab.dataDelete(this.vTable.getSelection(true));
        this.refreshTabPanelData(this.gTab.getRecord_ID());
        this.dynamicDisplay(-1);
        return retValue;
    };

    VIS.GridController.prototype.dataDeleteAsync = function () {
        //var retValue = this.gTab.dataDeleteAsync(this.vTable.getSelection(true));
        //this.refreshTabPanelData(this.gTab.getRecord_ID());
        //this.dynamicDisplay(-1);
        //return retValue;
        this.aPanel.setBusy(true);
        var that = this;
        that.gTab.getTableModel().dataDeleteAsync(that.vTable.getSelection(true), that.gTab.currentRow).then(function (info) {
            that.gTab.setCurrentRow(that.gTab.currentRow, true);
            that.refreshTabPanelData(that.gTab.getRecord_ID());
            that.dynamicDisplay(-1);
            that.aPanel.setBusy(false);
        });

    };

    /**
     *  Row Changed - synchronize with Tree
     *
     *  @param  save    true the row was saved (changed/added), false if the row was deleted
     *  @param  keyID   the ID of the row changed
     */
    VIS.GridController.prototype.rowChanged = function (save, keyID) {
        if (this.m_tree == null)
            return;
        if ($.isArray(keyID) && !save) {
            for (var i = 0; i < keyID.length; i++)
                this.m_tree.nodeChanged(save, keyID[i], "", "",
                    "", "");
            return;
        }

        if (keyID <= 0)
            return;

        var name = this.gTab.getValue("Name");
        var description = this.gTab.getValue("Description");
        var IsSummary = this.gTab.getValue("IsSummary");
        var summary = IsSummary == true || IsSummary == "Y";
        var imageIndicator = this.gTab.getValue("Action");  //  Menu - Action
        //
        this.m_tree.nodeChanged(save, keyID, name, description,
            summary, imageIndicator);
    };  //  rowChanged

    VIS.GridController.prototype.dataIgnore = function () {
        this.gTab.dataIgnore();
        this.dynamicDisplay(-1);
        this.switchIncludedGC();
        this.vTable.refreshUndo();
    };

    VIS.GridController.prototype.dataStatusChanged = function (e) {

        if (this.displayAsIncludedGC) {
            this.enableDisableToolbarItems(true);
            return;
        }
        this.aPanel.dataStatusChanged(e);
        var col = e.getChangedColumn();
        if (!e.getIsChanged() || col < 0)
            return;

        //  Process Callout
        var mField = this.gTab.getField(col);
        if (mField != null) {
            //mField.validateValue();
            if (mField.getCallout().length > 0) {
                var msg = this.gTab.processFieldChange(mField);     //  Dependencies & Callout
                if (msg.length > 0) {
                    VIS.ADialog.error(msg);
                }
            }
            else	//	no callout to set dependent fields
            {
                var columnName = mField.getColumnName();
                dependants = this.gTab.getDependantFields(columnName);
                for (var i = 0; i < dependants.length; i++) {
                    var dep = dependants[i];
                    if (dep == null)
                        continue;
                    var lookup = dep.getLookup();
                    if (lookup == null)
                        continue;
                    //
                    var val = lookup.getValidation();
                    if (val.indexOf(columnName) != -1)	//	dep is dependent
                    {
                        //Object oldValue = lookup.getSelectedItem();
                        //boolean mandatory = dep.isMandatory(false);
                        // lookup.fillComboBox (mandatory, true, true, false);
                        // lookup.setSelectedItemAlways(oldValue);	//	set old value with new rules
                    }
                }	//	for all dependent fields
            }
        }
        this.dynamicDisplay(col);	//	 -1 = all
    }; //  dataStatusChanged

    VIS.GridController.prototype.includeTab = function (gc) {
        var imcludedMTab = gc.getMTab();
        if (this.gTab.getIncluded_Tab_ID() != imcludedMTab.getAD_Tab_ID())
            return false;
        this.vIncludedGC = gc;
        this.vIncludedGC.switchMultiRow();
        return true;
    };	//	IncludeTab

    VIS.GridController.prototype.switchSingleRow = function (skip) {
        if (this.onlyMultiRow || this.singleRow)
            return;
        this.singleRow = true;
        this.isCardRow = false;
        this.isMapRow = false;

        var p1 = this.getVTablePanel();
        var p = this.getVPanel();

        if (this.isIncludedGCVisible || true) {
            //p1.width("0%");//  css('width:50%');;
            //p.width("99%");//  css('width:50%');;
            p1.hide();
            p.css("display", "block");// .show();
            this.getVCardPanel().hide();
            this.getVMapPanel().hide();

        }
        this.dynamicDisplay(-1);
    };

    VIS.GridController.prototype.switchMultiRow = function () {
        if (this.singleRow) {

            this.singleRow = false;
            this.isCardRow = false;
            this.isMapRow = false;

            var p1 = this.getVTablePanel();
            this.getVPanel().hide();

            this.getVCardPanel().hide();
            this.getVMapPanel().hide();

            // p1.width(this.displayAsIncludedGC ? '98%' : '97%');
            if (this.isIncludedGCVisible)
                p1.css({ "float": 'right' });
            else p1.css({ "float": '' });

            p1.show();
            p1 = null;
            this.vTable.resize();
            this.vTable.refreshRow();
        }
        //this.switchIncludedGC();
    };

    VIS.GridController.prototype.switchCardRow = function () {
        if (!this.isCardRow) {

            this.singleRow = false;
            this.isCardRow = true;
            this.isMapRow = false;


            this.getVTablePanel().hide();
            this.getVPanel().hide();

            this.getVMapPanel().hide();

            var p1 = this.getVCardPanel();

            //p1.width(this.displayAsIncludedGC ? '98%' : '97%');
            if (this.isIncludedGCVisible)
                p1.css({ "float": 'right' });
            else p1.css({ "float": '' });

            p1.show();
            this.vCardView.refreshUI(this.getVCardPanel().width());
            p1 = null;
            //this.vTable.resize();
        }
        //this.switchIncludedGC();
    };

    VIS.GridController.prototype.switchMapRow = function () {
        if (!this.isMapRow) {

            this.singleRow = true;
            this.isCardRow = false;
            this.isMapRow = true;


            this.getVTablePanel().hide();
            this.getVPanel().hide();

            this.getVCardPanel().hide();

            var p1 = this.getVMapPanel();

            //p1.width(this.displayAsIncludedGC ? '98%' : '97%');
            if (this.isIncludedGCVisible)
                p1.css({ "float": 'right' });
            else p1.css({ "float": '' });

            p1.show();
            this.vMapView.refreshUI(this.getVMapPanel().width());
            p1 = null;
            //this.vTable.resize();
        }
        //this.switchIncludedGC();
    };




    //show hide Included grid
    VIS.GridController.prototype.switchIncludedGC = function () {
        if (!this.vIncludedGC || this.displayAsIncludedGC) //has included grid
            return;

        var visible = this.isIncludedGCVisible; //`
        //return;
        //if (this.singleRow) { ///hide if parent gridcontroller in edit mode

        //if (visible) {
        //    this.vIncludedGC.getRoot().hide();
        //    this.vIncludedGC.getRoot().width("100%");
        //    this.getRoot().width("100%");
        //    this.isIncludedGCVisible = false;
        //    // this.displayAsIncludedGC = false;
        //    this.vIncludedGC.displayAsIncludedGC = false;
        //    //this.vIncludedGC.isIncludedGCVisible = false;

        //    //this.toggleTabItems();
        //    //this.vIncludedGC.toggleTabItems();
        //     return;
        //}

        //}

        if (!visible) {
            var tdArea = this.aPanel.getIncludedEmptyArea();
            tdArea.empty();
            var inGc = this.vIncludedGC.getRoot();
            inGc.detach();
            this.vIncludedGC.setUI(true);

            //this.getRoot().css({
            //    'float': 'left',
            //    width: '49%'
            //});

            //inGc.css({
            //    'position': 'absolute',
            //    'left': '100%',
            //    width: '100%'
            //});
            tdArea.append(inGc);

            if (this.gTab.getHasPanel()) {
                tdArea.css("padding", "0px 1px 0px 1px");
            }
            else {
                tdArea.css("padding", "0px 5px 0px 7px");
            }
            tdArea.width(VIS.Application.isMobile ? 250 : 350);

            inGc.show();
            this.vIncludedGC.vTable.activate();
            this.vIncludedGC.vTable.setReadOnly(true);
            //this.vIncludedGC.vTable.resize()
            //setTimeout(function (child) {
            //  child.getRoot().show().animate({

            //width: '49%'
            //    }, 400, function () {
            //        $(this).css('position', 'static');
            //        child.vTable.resize()
            //        child = null;
            //    });
            //}, 500, this.vIncludedGC);

            //this.getRoot().show();
            this.isIncludedGCVisible = true;

            if (!this.singleRow) {
                this.singleRow = true;
                this.switchMultiRow();
            }

            //this.displayAsIncludedGC = false;
            // this.vIncludedGC.isIncludedGCVisible = false;
            this.vIncludedGC.displayAsIncludedGC = true;
            this.vIncludedGC.singleRow = true;
            this.vIncludedGC.switchMultiRow();
            //this.toggleTabItems();
            //this.vIncludedGC.toggleTabItems();

            this.vTable.resize();
            if (this.gTab.getHasPanel()) {
                this.aPanel.setWidth(-1, true);
            }

            //setTimeout(function (that) {
            //    // that.vTable.refresh();

            //    that.vTable.resize();
            //    //that.vIncludedGC.getRoot().slideDown(5000);
            //    //this.vIncludedGC.getRoot().slideDown(1000).show();
            //    that = null;
            //}, 10, this);
        }

        window.setTimeout(function (s) { s.vIncludedGC.query(0, 0, false); s = null; }, 1, this);
    };


    VIS.GridController.prototype.removeRecord = function (e) {
        var selectedRecs = this.vTable.getSelection(true);
        if (selectedRecs && selectedRecs.length > 0) {

            for (var i = 0; i < selectedRecs.length; i++) {
                this.vTable.grid.unselect(selectedRecs[i] + 1);
                this.vTable.grid.remove(selectedRecs[i] + 1);
            }
        }
        this.navigate(0);
        this.aPanel.setBusy(false);
    };

    VIS.GridController.prototype.dispose = function () {
        //unwind events
        //this.vTable.getGrid().off('select', this.onTableRowSelect);

        this.gTab.removeDataStatusListener(this.aPanel);
        this.gTab.removeDataStatusListener();
        this.gTab.getTableModel().removeTableModelListener(this.vTable);
        this.gTab.getTableModel().removeCardModelListener(this.vCardView);
        this.gTab.getTableModel().removeRowChangedListener();
        this.gTab.getTableModel().removeQueryCompleteListner();
        this.disposeComponent();
        this.gTab = null;
        this.windowNo = null;
        this.onlyMultiRow = null;
        this.aPanel = null;
        if (this.m_tree)
            this.m_tree.dispose();
        if (this.curTabPanel) {
            this.curTabPanel.dispose();
            this.curTabPanel = null;
        }
        this.m_tree = null;
    };

    VIS.GridController.prototype.HEADER_HEIGHT = 55;

    //****************** END *****************************//

    VIS.AParentDetail = function (gc, $root) {
        this.gc = gc;
        this.$root = $root;
        this.visible = false;
        this.expaned = false;
        this.curGC = null;


        var fields = gc.getMTab().getTableModel().getFields(); //all fields
        var len = fields.length;


        var $rPart, $lPart, $main = null;
        var $divlbMain, $divlbNav, $lPartContent;

        function initComponent() {
            $main = $("<div class='vis-apanel-bar'>");
            $rPart = $("<div class='vis-apanel-bar-fixpart' style='background-color:white'>");
            $lPart = $("<div class='vis-apanel-bar-varpart vis-apanel-bar-pdetail'>");

            $divlbMain = $('<div class="vis-apanel-lb-main">');
            $divlbNav = $("<div class='vis-apanel-lb-oflow' style='border-left: 1px solid white;'>").hide();
            $divlbNav.html("<a data-dir='u' href='javascript:void(0)'><img style='margin-left:10px' data-dir='u' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-top.png' ></a><a data-dir='d' href='javascript:void(0)' ><img style='margin-left:10px' data-dir='d' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-bottom.png' /></a>");

            $lPart.append($divlbMain).append($divlbNav);

            $lPartContent = $("<div style='width:192px;overflow-y:auto;padding-left: 4px;padding-right:3px;'>");
            $divlbMain.append($lPartContent);

            $main.append($lPart).append($rPart);
            $root.append($main);
            $main.hide();
        }
        initComponent();

        var self = this;
        $rPart.on(VIS.Events.onTouchStartOrClick, function (e) {
            if (self.$root.width() > 20) {
                self.hide();
            }
            else {
                self.show();
            }
            e.stopPropagation();
        });


        $divlbNav.on("click", function (e) {
            e.stopPropagation();
            var dir = $(e.target).data('dir');
            if (!dir) return;

            var dHeight = $divlbMain.height();
            var ulheight = $lPartContent.height();
            var cPos = $divlbMain.scrollTop();
            var offSet = Math.ceil(dHeight / 2);
            var s = 0;
            if (dir == 'd') {
                if ((cPos + offSet) >= ulheight - offSet)
                    return;
                var ms = ulheight - dHeight;
                s = cPos + offSet;
                $divlbMain.animate({ scrollTop: s > ms ? ms : s }, 1000, "easeOutBounce");
            }
            else if (dir == 'u') {
                if (cPos == 0)
                    return;
                s = (cPos - offSet);
                $divlbMain.animate({ scrollTop: s < 0 ? 0 : s }, 1000, "easeOutBounce");
                //$divTabControl.scrollLeft(cPos - offSet);
            }
        });


        function refresh() {

            var field = null;
            var html = "";
            var colValue = ""

            for (var i = 0; i < len; i++) {
                field = fields[i];
                colValue = null;

                colValue = field.getValue();

                if (!colValue || colValue == "" || !field.getIsDisplayed())
                    continue;

                html += "<h3>" + w2utils.encodeTags(field.getHeader()) + "</h3>";

                var displayType = field.getDisplayType();

                if (field.lookup) {
                    colValue = field.lookup.getDisplay(colValue, true);
                }

                //	Date
                else if (VIS.DisplayType.IsDate(displayType)) {
                    colValue = new Date(colValue).toLocaleString();
                }
                //	RowID or Key (and Selection)

                //	YesNo
                else if (displayType == VIS.DisplayType.YesNo) {
                    var str = colValue.toString();
                    if (field.getIsEncryptedColumn())
                        str = VIS.secureEngine.decrypt(str);
                    colValue = str.equals("Y");	//	Boolean
                }
                //	LOB 

                else
                    colValue = colValue.toString();//string
                //	Encrypted
                if (field.getIsEncryptedColumn() && displayType != VIS.DisplayType.YesNo)
                    colValue = VIS.secureEngine.decrypt(colValue);

                html += "<h5>" + w2utils.encodeTags(colValue) + "</h5>";
            }

            $lPartContent.empty();
            $lPartContent.html("<h1>" + gc.getMTab().getName() + "</h1>" + html);

            self.setSize($main.height());
        };

        this.show = function () {

            if (self.expaned) {
                refresh();
                return;
            }
            this.expaned = true;

            if (self.visible) {
                $main.show();
            }
            //$main.height($root.height());
            $root.animate({
                //"left": $root.width() - 250 + "px",
                "width": "200px",
            }, 300, "swing");

            $lPart.show().animate({
                "width": "192px"

            }, 300, "swing", showComplete);
        };

        this.hide = function () {

            if (!this.expaned && this.visible) //hiddden
                return;

            this.expaned = false;


            $root.animate({
                "width": self.visible ? "8px" : "0px"
            }, 300, "swing");

            if (!self.visible) {
                $main.hide();
            }
            $lPart.hide().animate({
                "width": "0px"
            }, 300, "swing", hideComplete);
        };

        function hideComplete() {
            if (self.curGC) {
                self.curGC.multiRowResize();
                self.curGC.aPanel.setWidth(-1, true);
                self.curGC.aPanel.setTabNavigation();
            }
        };

        function showComplete() {
            refresh();
            if (self.curGC) {
                self.curGC.multiRowResize();
                self.curGC.aPanel.setWidth(-1, true);
                self.curGC.aPanel.setTabNavigation();
            }
        };

        this.setSize = function (height) {
            $main.height(height);
            $divlbMain.height(height); //left bar overflow

            if ($lPartContent.height() > $divlbMain.height()) {
                if (!VIS.Application.isMobile) {
                    $divlbMain.height(height - 40);
                    $divlbNav.show();
                }
            }
        };

        this.disposeComponenet = function () {
            this.gc = null;
            this.$root = null;
            this.visible = null;
            this.expaned = null;
            $divlbNav.off("click");
            $rPart.off(VIS.Events.onTouchStartOrClick);
            $rPart = $lPart = null;
            $main.remove();
            $main = null;
            self = null;

            this.curGC = null;
            fields.length = 0;
            fields = null;

            this.show = null;
            this.hide = null;
        };

    };

    VIS.AParentDetail.prototype.evaluate = function (curGc) {
        this.curGC = null;
        this.curGC = curGc;

        if (this.gc.getId() === curGc.getId()) {
            this.visible = false;
            this.hide();
        }
        else {
            this.curGC.isParentDetailVisible = true;
            if (this.visible) //already visible
                return;
            this.visible = true;
            this.show();
        }
    };

    VIS.AParentDetail.prototype.dispose = function () {
        this.disposeComponenet();
    };




    //****************************************************//
    //**             VSortTab                          **//
    //**************************************************//
    VIS.VSortTab = function (windowNo, AD_Table_ID, AD_ColumnSortOrder_ID, AD_ColumnSortYesNo_ID,
        isReadOnly, id) {

        this.winNumber = windowNo;
        this.tableName = null;
        this.columnSortName = null;
        this.columnYesNoName = null;
        this.keyColumnName = null;
        this.identifierColumnName = null;
        this.identifierTranslated = false;

        this.parentColumnName = null;

        this.aPanel = null;


        this.log = VIS.Logging.VLogger.getVLogger("VSortTab");

        var $tblRoot, $tabControl;
        var $lblNo, $lblYes, $lstNo, $lstYes;
        var $btnRight, $btnLeft, $btnUp, $btnDown;
        var path = VIS.Application.contextUrl + "Areas/VIS/Images/base/";
        var oldValues = null;

        function initializeComponent() {

            $lblNo = $("<span>").text(VIS.Msg.getMsg("N"));
            $lblYes = $("<span>").text(VIS.Msg.getMsg("Y"));
            //$lstNo = $("<select multiple " + (isReadOnly ? " disabled" : "") + ">");
            //$lstYes = $("<select multiple" + (isReadOnly ? " disabled" : "") + " >");

            $lstNo = $("<ul class='vis-sortTab-select' " + (isReadOnly ? " disabled" : "") + ">");
            $lstYes = $("<ul class='vis-sortTab-select' " + (isReadOnly ? " disabled" : "") + " >");

            //$noDiv = $("<div class='vis-apanel-sorttab-div' " + (isReadOnly ? " disabled" : "") + ">");
            //$yesDiv = $("<div class='vis-apanel-sorttab-div' " + (isReadOnly ? " disabled" : "") + " >");

            $btnLeft = $("<button" + (isReadOnly ? " disabled" : "") + " ><img src='" + path + "Left32.png" + "' /></button");
            $btnRight = $("<button" + (isReadOnly ? " disabled" : "") + " ><img src='" + path + "Right32.png" + "' /></button");
            $btnUp = $("<button" + (isReadOnly ? " disabled" : "") + " ><img src='" + path + "Up32.png" + "' /></button");
            $btnDown = $("<button" + (isReadOnly ? " disabled" : "") + " ><img src='" + path + "Down32.png" + "' /></button");

            $tblRoot = $("<table class='vis-apanel-sorttab' style='display:none;'>");
            //$tabControl = $("<ul class='vis-gc-tabs-ul'>");

            //$tblRoot.append($("<tr>").append($("<td>").append($tabControl)));

            var $td = $("<td class='vis-apanel-sorttab-td'>");
            var $divLeftContainer = $('<div>');

            $divLeftContainer.append($lstNo).append($("<div class='vis-apanel-sorttab-td-right'>").append($btnRight).append($btnLeft)));
                
            $td.append($("<div class='vis-apanel-sorttab-td-inner'>").append($lblNo)).append($divLeftContainer);

            var $divRightContainer = $('<div>');
            $divRightContainer.append($lstYes).append($("<div class='vis-apanel-sorttab-td-right'>").append($btnUp).append($btnDown));

            $td.append($("<div class='vis-apanel-sorttab-td-inner'>").append($lblYes).append($divRightContainer));

            $tblRoot.append($("<tr>").append($td));
        }

        initializeComponent();

        this.dynInit(AD_Table_ID, AD_ColumnSortOrder_ID, AD_ColumnSortYesNo_ID); //Dynamic Initilize

        this.getRoot = function () {
            return $tblRoot;
        }

        this.getId = function () {
            return id;
        }

        this.getlstModel = function (isYes) {
            if (isYes)
                return $lstYes.find("li");// [0].options;
            return $lstNo.find("li");// [0].options;
        }

        this.setLabelName = function (no, yes) {
            $lblNo.text(no);
            $lblYes.text(yes);
        };

        this.setListOptions = function (no, yes) {
            $lstNo.empty();
            $lstNo.append(no);
            $lstYes.empty();
            $lstYes.append(yes);

        };

        this.setOldValues = function (list) {
            oldValues = list;
        };

        this.getOldValues = function () {
            return oldValues;
        };

        var self = this;

        $btnLeft.on(VIS.Events.onTouchStartOrClick, function (e) {
            self.btn_Click("Left");
            e.stopPropagation();
        });

        $btnRight.on(VIS.Events.onTouchStartOrClick, function (e) {
            self.btn_Click("Right");
            e.stopPropagation();
        });

        $btnUp.on(VIS.Events.onTouchStartOrClick, function (e) {
            self.btn_Click("Up");
            e.stopPropagation();
        });

        $btnDown.on(VIS.Events.onTouchStartOrClick, function (e) {
            self.btn_Click("Down");
            e.stopPropagation();
        });

        $lstNo.on(VIS.Events.onClick, "LI", function () {
            $(this).toggleClass("vis-apanel-sorttab-selected");
        });

        $lstYes.on(VIS.Events.onClick, "LI", function () {
            $(this).toggleClass("vis-apanel-sorttab-selected");
        });

        this.btn_Click = function (action) {

            var change = false;
            var selObjects = null;

            if (action == "Right") {

                selObjects = $lstNo.find("li.vis-apanel-sorttab-selected");
                $lstYes.children().removeClass('vis-apanel-sorttab-selected');
                //$lstYes[0].selectedIndex = -1;
                for (var i = 0; i < selObjects.length; i++) {
                    selObjects[i].remove();
                    $(selObjects[i]).toggleClass("vis-apanel-sorttab-selected");
                    //$lstYes[0].add(selObjects[i]);
                    $lstYes.append(selObjects[i]);
                    change = true;
                }
                selObjects.length = 0;
                selObjects = 0;
            }
            else if (action == "Left") {
                selObjects = $lstYes.find("li.vis-apanel-sorttab-selected");
                //$lstNo[0].selectedIndex = -1;
                $lstNo.children().removeClass('vis-apanel-sorttab-selected');
                for (var j = 0; j < selObjects.length; j++) {
                    selObjects[j].remove();
                    $(selObjects[j]).toggleClass("vis-apanel-sorttab-selected");
                    $lstNo.append(selObjects[j]);
                    change = true;
                }
                selObjects.length = 0;
                selObjects = 0;
            }
            else {
                var selObjects = $lstYes.find('li.vis-apanel-sorttab-selected');
                if (selObjects == null) {
                    return;
                }
                var length = selObjects.length;
                if (length == 0)
                    return;
                var selObject = $(selObjects[0]);
                if (selObject == null)
                    return;
                $lstYes.children().removeClass('vis-apanel-sorttab-selected');
                selObject.addClass('vis-apanel-sorttab-selected');
                //IList indices = ;
                var index = selObject.index();// $lstYes[0].selectedIndex;
                //if (index == -1)
                //    return;
                //selObject = $lstYes[0].options[index];
                //if (selObject == null)
                //  return;

                if (action == "Up") {
                    if (index == 0)
                        return;
                    //Object newObject = lstYes.Items[index - 1];
                    //lstYes.Items.Insert( index,newObject);
                    //$lstYes[0].remove(index);
                    selObject.remove();
                    selObject.insertBefore($lstYes.find("li").eq(index - 1));
                    //$lstYes[0].add(selObject, index - 1);
                    // $lstYes[0].selectedIndex = index - 1;
                    change = true;
                }

                else if (action == "Down") {
                    if (index >= $lstYes.children().length - 1)
                        return;
                    selObject.remove();
                    //$lstYes[0].remove(index);
                    //$lstYes[0].add(selObject, index + 1);
                    selObject.insertAfter($lstYes.find("li").eq(index));
                    // $lstYes[0].selectedIndex = index + 1;
                    change = true;
                }
            }
            if (change && this.aPanel != null) {
                this.aPanel.aSave.setEnabled(true);
            }
        };

        this.disposeComponent = function () {

            $btnLeft.off(VIS.Events.onTouchStartOrClick);

            $btnRight.off(VIS.Events.onTouchStartOrClick);

            $btnUp.off(VIS.Events.onTouchStartOrClick);

            $btnDown.off(VIS.Events.onTouchStartOrClick);

            this.seletedTab = null;
            $tblRoot.remove();
            $tblRoot = null;
            //$tabControl.remove();
            //$tabControl = null;

            self = null;

            this.winNumber = null;
            this.tableName = null;
            this.columnSortName = null;
            this.columnYesNoName = null;
            this.keyColumnName = null;
            this.identifierColumnName = null;
            this.identifierTranslated = false;
            this.parentColumnName = null;
            this.aPanel = null;
            this.log = null;
            $lstNo = null;
            $lstYes = null;
            $btnUp = $btnDown = $btnLeft = $btnRight = null;

            //functions
            this.btn_Click = this.setListOptions = this.getlstModel = this.setLabelName = this.setListOptions = this.getRoot = this.getId = null;
            console.log("dispose vSortTab");
        }
    };

    VIS.VSortTab.prototype.dynInit = function (AD_Table_ID, AD_ColumnSortOrder_ID, AD_ColumnSortYesNo_ID) {

        var trl = !VIS.Env.isBaseLanguage(VIS.Env.getCtx(), "");



        //var sql = "SELECT t.TableName, c.AD_Column_ID, c.ColumnName, e.Name,"	//	1..4
        //    + "c.IsParent, c.IsKey, c.IsIdentifier, c.IsTranslated "				//	4..8
        //    + "FROM AD_Table t, AD_Column c, AD_Element e "
        //    + "WHERE t.AD_Table_ID=" + AD_Table_ID						//	#1
        //    + " AND t.AD_Table_ID=c.AD_Table_ID"
        //    + " AND (c.AD_Column_ID=" + AD_ColumnSortOrder_ID + " OR AD_Column_ID=" + AD_ColumnSortYesNo_ID 	//	#2..3
        //    + " OR c.IsParent='Y' OR c.IsKey='Y' OR c.IsIdentifier='Y')"
        //    + " AND c.AD_Element_ID=e.AD_Element_ID";


        var sql = "VIS_122";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Table_ID", AD_Table_ID);
        param[1] = new VIS.DB.SqlParam("@AD_ColumnSortOrder_ID", AD_ColumnSortOrder_ID);
        param[2] = new VIS.DB.SqlParam("@AD_ColumnSortYesNo_ID", AD_ColumnSortYesNo_ID);

        if (trl) {
            //sql = "SELECT t.TableName, c.AD_Column_ID, c.ColumnName, et.Name,"	//	1..4
            //    + "c.IsParent, c.IsKey, c.IsIdentifier, c.IsTranslated "		//	4..8
            //    + "FROM AD_Table t, AD_Column c, AD_Element_Trl et "
            //    + "WHERE t.AD_Table_ID=" + AD_Table_ID						//	#1
            //    + " AND t.AD_Table_ID=c.AD_Table_ID"
            //    + " AND (c.AD_Column_ID=" + AD_ColumnSortOrder_ID + " OR AD_Column_ID=" + AD_ColumnSortYesNo_ID	//	#2..3
            //    + "	OR c.IsParent='Y' OR c.IsKey='Y' OR c.IsIdentifier='Y')"
            //    + " AND c.AD_Element_ID=et.AD_Element_ID"
            //    + " AND et.AD_Language='" + VIS.Env.getAD_Language(VIS.Env.getCtx()) + "'";                   //	#4

            sql = "VIS_123";
            param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Table_ID", AD_Table_ID);
            param[1] = new VIS.DB.SqlParam("@AD_ColumnSortOrder_ID", AD_ColumnSortOrder_ID);
            param[2] = new VIS.DB.SqlParam("@AD_ColumnSortYesNo_ID", AD_ColumnSortYesNo_ID);
            param[3] = new VIS.DB.SqlParam("@AD_Language", VIS.Env.getAD_Language(VIS.Env.getCtx()));

        }

        var self = this;
        var tableName = "";
        executeReader(sql, param, function (dr) {
            if (dr != null) {

                var lblYesName = "";
                while (dr.read()) {
                    self.tableName = dr.getString(0);
                    //	Sort Column
                    if (AD_ColumnSortOrder_ID == dr.get(1)) {
                        //log.Fine("Sort=" + dr.GetString(0) + "." + dr.GetString(2));
                        self.columnSortName = dr.getString(2);
                        lblYesName = dr.getString(3);
                    }
                    //	Optional YesNo
                    else if (AD_ColumnSortYesNo_ID == dr.get(1)) {
                        //log.Fine("YesNo=" + dr.GetString(0) + "." + dr.GetString(2));
                        self.columnYesNoName = dr.getString(2);
                    }
                    //	Parent2
                    else if (dr.getString(4) == "Y") {
                        //log.Fine("Parent=" + dr.GetString(0) + "." + dr.GetString(2));
                        self.parentColumnName = dr.getString(2);
                    }
                    //	KeyColumn
                    else if (dr.getString(5) == "Y") {
                        //log.Fine("Key=" + dr.GetString(0) + "." + dr.GetString(2));
                        self.keyColumnName = dr.getString(2);
                    }
                    //	Identifier
                    else if (dr.getString(6) == "Y") {
                        //log.Fine("Identifier=" + dr.GetString(0) + "." + dr.GetString(2));
                        self.identifierColumnName = dr.getString(2);
                        if (trl)
                            self.identifierTranslated = "Y" == dr.getString(7);
                    }
                    else {
                        //log.Fine("??NotUsed??=" + dr.GetString(0) + "." + dr.GetString(2));
                    }

                }
                dr.close();
                self.setLabelName(VIS.Msg.getMsg("Available"), lblYesName);
            }
            self = null;
        });

    };

    VIS.VSortTab.prototype.setVisible = function (visible) {
        if (visible) {
            this.getRoot().show();
        }
        else {
            this.getRoot().hide();
        }
    };

    VIS.VSortTab.prototype.loadData = function () {
        //lstNo.Items.Clear();
        //lstYes.Items.Clear();

        var sql = "";

        sql += "SELECT t." + this.keyColumnName;				//	1
        if (this.identifierTranslated) {
            sql += ",tt.";
        }
        else {
            sql += ",t."
        }
        sql += this.identifierColumnName						//	2
            + ",t." + this.columnSortName;				//	3
        if (this.columnYesNoName != null)
            sql += ",t." + this.columnYesNoName;			//	4
        //	Tables
        sql += " FROM " + this.tableName + " t";
        if (this.identifierTranslated)
            sql += ", " + this.tableName + "_Trl tt";
        //	Where
        sql += " WHERE t." + this.parentColumnName + "=@ID";
        if (this.identifierTranslated)
            sql += " AND t." + this.keyColumnName + "=tt." + this.keyColumnName
                + " AND tt.AD_Language='" + VIS.context.getAD_Language() + "'";
        //	Order
        sql += " ORDER BY ";
        if (this.columnYesNoName != null)
            sql += "4 DESC,";		//	t.IsDisplayed DESC
        sql += "3,2";				//	t.SeqNo, tt.Name 
        var ID = VIS.Env.getCtx().getWindowContext(this.winNumber, this.parentColumnName);

        //log.Config(sql.ToString() + " - ID=" + ID);

        //BackgroundWorker bgw = new BackgroundWorker();



        var dr = executeDReader(sql, [new VIS.DB.SqlParam("@ID", ID)]);
        var yesHtml = "";
        var noHtml = "";
        var listOldValues = [];

        try {
            while (dr.read()) {
                var key = dr.get(0);
                var name = dr.getString(1);
                var seq = dr.get(2);
                var isYes = seq != 0;
                if (this.columnYesNoName != null)
                    isYes = dr.getString(3) == "Y";

                var pp = "<li data-value='" + key + "'>" + name + "</li>";

                if (isYes) {
                    yesHtml += pp;
                    listOldValues.push({ "key": key, "value": seq });
                }
                else {
                    noHtml += pp;
                    listOldValues.push({ "key": key, "value": seq });
                }
            }
            dr.close();
            dr.dispose();
            this.setOldValues(listOldValues);
            this.setListOptions(noHtml, yesHtml);

        }
        catch (e) {
            // if (!dr.IsClosed)
            this.log.Log(VIS.Logging.Level.SEVERE, sql, e);
        }
        this.aPanel.aSave.setEnabled(false);
    };

    VIS.VSortTab.prototype.saveData = function () {

        if (!this.aPanel.aSave.getIsEnabled())
            return;

        var sql = null;
        //	noList - Set SortColumn to null and optional YesNo Column to 'N'
        var noModel = this.getlstModel();

        var self = this;
        //var queries = [];
        var i = 0;
        var pp;
        //	noList - Set SortColumn to null and optional YesNo Column to 'N'

        //for (i = 0; i < noModel.length; i++) {

        var tableName = this.tableName;
        var columnSortName = this.columnSortName;
        var columnYesNoName = this.columnYesNoName;
        var keyColumnName = this.keyColumnName;

        var values = [];
        var noYes = [];
        noModel.each(function (i, li) {

            values.push($(li).data("value"));
            noYes.push("N");
            //var value = $(li).data("value");
            //sql = "";
            //sql += "UPDATE " + tableName
            //    + " SET " + columnSortName + "=0";
            //if (columnYesNoName != null)
            //    sql += "," + columnYesNoName + "='N'";

            //sql += ", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP)";

            //sql += " WHERE " + keyColumnName + "=" + value;

            //queries.push(sql);




        });

        var yesModel = this.getlstModel(true);
        // for (i = 0; i < yesModel.length; i++) {
        yesModel.each(function (i, li) {
            values.push($(li).data("value"));
            noYes.push("Y");

            //var value = $(li).data("value");
            //sql = "";
            //sql += "UPDATE " + tableName
            //    + " SET " + columnSortName + "=" + (i + 1) + "0";	//	10 steps
            //if (columnYesNoName != null)
            //    sql += "," + columnYesNoName + "='Y'";

            //sql += ", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP)";

            //sql += " WHERE " + keyColumnName + "=" + value;



            //queries.push(sql);
        });


        $.ajax({
            url: VIS.Application.contextUrl + 'Form/SetFieldsSorting',
            async: false,
            type: 'POST',
            data: {
                values: JSON.stringify(values),
                noYes: JSON.stringify(noYes),
                tableName: tableName,
                keyColumnName: keyColumnName,
                columnSortName: columnSortName,
                columnYesNoName: columnYesNoName,
                oldValues: JSON.stringify(self.getOldValues())
            },
            success: function (data) { },
            error: function (er) {

            }
        })


        //var ret = executeQueries(queries, null);

    };

    VIS.VSortTab.prototype.unRegisterAPanel = function () {
        this.saveData();
        this.aPanel = null;
    };

    VIS.VSortTab.prototype.registerAPanel = function (pnl) {
        this.aPanel = pnl;
    };

    VIS.VSortTab.prototype.sizeChanged = function (height, width) {
        this.getRoot().height(height);
    };

    VIS.VSortTab.prototype.dispose = function () {
        this.disposeComponent();
    };

    //********************** END *********************//




    //****************************************************//
    //**             VPanel                            **//
    //**************************************************//
    VIS.VGridPanel = function () {

        var oldFieldGroup = null, columnIndex = -2, allControlCount = -1;;
        var allControls = [];
        var allLinkControls = [];

        var $table;
        var $row = null;
        var $td0, $td1, $td11, $td12, $td2, $td3, $td31, $td32, $td4;

        /** Map of group name to list of components in group. */
        //control = field array
        var compToFieldMap = {}

        /** Map of group name to list of components in group. */
        var groupToCompsMap = {};


        function initComponent() {
            $table = $("<table class='vis-gc-vpanel-table'>");
            //<tr><td class='vis-gc-vpanel-table-td0'><td class='vis-gc-vpanel-table-td1'>" +
            //"<td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'><td class='vis-gc-vpanel-table-td2'>" +
            //"<td  class='vis-gc-vpanel-table-td3'><td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'>" +
            //"<td class='vis-gc-vpanel-table-td4'></tr></table>");
        };

        initComponent();

        function addRow() {
            $td0 = $td1 = $td11 = $td12 = $td2 = $td3 = $td31 = $td32 = $td4 = $row = null;
            $td0 = $("<td  class='vis-gc-vpanel-table-td0'>");
            $td1 = $("<td colspan = '3' class='vis-gc-vpanel-table-td1'>");
            $td2 = $("<td class='vis-gc-vpanel-table-td2'>");
            $td3 = $("<td colspan='3' class='vis-gc-vpanel-table-td3'>");
            $td4 = $("<td class='vis-gc-vpanel-table-td4'>");
            $row = $("<tr>");
            $table.append($row);
            $row.append($td0).append($td1).append($td2).append($td3).append($td4);
        }

        function onGroupClick(e) {
            e.stopPropagation();
            var o = $(this);
            var name = o.data("name");
            var dis = o.data("display");

            //console.log(name);
            //console.log(dis);
            var show = false;
            if (dis === "show") {
                o.data("display", "hide");
                $(o.children()[0]).addClass("vis-gc-vpanel-fieldgroup-img-rotate");
            } else {
                o.data("display", "show");
                show = true;
                $(o.children()[0]).removeClass("vis-gc-vpanel-fieldgroup-img-rotate");
            }

            var list = groupToCompsMap[name];
            for (var i = 0; i < list.length; i++) {
                var field = compToFieldMap[list[i].getName()];
                list[i].tag = show;
                list[i].setVisible(show && field.getIsDisplayed(true));
            }
        };

        function addGroup(fieldGroup) {
            if (oldFieldGroup == null) {
                //addTop();
                oldFieldGroup = "";
            }
            if (fieldGroup == null || fieldGroup.length == 0 || fieldGroup.equals(oldFieldGroup))
                return false;
            oldFieldGroup = fieldGroup;

            addRow();
            var gSpan = $("<span class='vis-gc-vpanel-fieldgroup-span'>" + fieldGroup + "</span>");
            var gImg = $("<img class='vis-gc-vpanel-fieldgroup-img' src= '" + VIS.Application.contextUrl + "Areas/VIS/Images/base/fieldgrpdown.png' >");
            var gDiv = $("<div class='vis-gc-vpanel-fieldgroup' data-name='" + fieldGroup + "' data-display='show' >").append(gImg).append(gSpan);
            //$td1.attr("colspan", 3);
            $td3.remove();
            $td2.remove();
            $td1.prop('colspan', 8);
            $td1.append(gDiv);

            //VLine fp = new VLine(fieldGroup);
            gDiv.on("click", onGroupClick);

            return true;
        };

        function addToCompList(comp) {

            if (oldFieldGroup != null && !oldFieldGroup.equals("")) {
                var compList = null;

                if (groupToCompsMap[oldFieldGroup]) {
                    compList = groupToCompsMap[oldFieldGroup];
                }

                if (compList == null) {
                    compList = [];
                    groupToCompsMap[oldFieldGroup] = compList;
                }
                compList.push(comp);
            }
        };

        this.addField = function (editor, mField) {

            /* Dont Add in control panel */
            if (mField.getIsLink()) {
                allControls[++allControlCount] = editor;
                //allControls.push(editor);
                allLinkControls.push(editor);
                return;
            }

            var label = VIS.VControlFactory.getLabel(mField);
            if (label == null && editor == null)
                return;
            var sameLine = mField.getIsSameLine();
            if (addGroup(mField.getFieldGroup())) {
                sameLine = false;
            }

            if (sameLine) {
                ++columnIndex;
                if (columnIndex > 2) {
                    sameLine = false;
                    addRow();
                    columnIndex = 0;
                }
                else if (columnIndex < 0) {
                    addRow();
                    columnIndex = 0;
                }

            }
            else {
                columnIndex = 0;
                addRow();
            }

            if (label != null) {
                if (sameLine) {
                    $td3.append(label.getControl());
                } else {
                    $td1.append(label.getControl());
                }

                if (mField.getDescription().length > 0) {
                    label.getControl().prop('title', mField.getDescription());
                }


                addToCompList(label);
                compToFieldMap[label.getName()] = mField;
                allControls[++allControlCount] = label;
            }
            if (editor != null) {
                if (sameLine) {
                    $td3.append(editor.getControl());
                } else {
                    $td1.append(editor.getControl());
                }
                var fieldVFormat = mField.getVFormat();
                switch (fieldVFormat) {
                    case '': {
                        break;
                    }
                    default: {
                        editor.getControl().on("focusout", function (e) {
                            var patt = new RegExp(fieldVFormat);
                            if (VIS.DisplayType.IsString(mField.getDisplayType())) {
                                if ($(e.target).val() != null) {
                                    if ($(e.target).val().toString().trim().length > 0) {
                                        if (!patt.test($(e.target).val())) {
                                            //Work DOne to set focus in field whose value does not match with regular expression.
                                            VIS.ADialogUI.warn(VIS.Msg.getMsg('RegexFailed') + ":" + mField.getHeader(), "", function () {
                                                $(e.target).focus();
                                            });

                                        }
                                    }
                                }
                            }
                        });
                    }
                }

                var count = editor.getBtnCount();

                if (count > 0) {

                    while (count > 0) {
                        var ctrl = editor.getBtn(count - 1);

                        if (ctrl != null) {
                            if (sameLine) {
                                $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td3);
                            }
                            else {
                                $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td1);
                            }
                        }
                        --count;
                    }
                    count = -1;
                    ctrl = null;
                }
                else {

                }

                count = editor.getBtnCount();


                if (!sameLine && mField.getIsLongField()) {
                    $td2.remove();
                    $td3.remove();
                    $td1.prop('colspan', 8 - count);
                    $td1.css('width', '100%');
                    columnIndex = 2;
                }
                else {
                    if (sameLine) {
                        $td3.prop('colspan', 3 - count);
                        columnIndex = 2;
                    } else {
                        $td1.prop('colspan', 3 - count);
                        //$td3.prop('colspan', 3);
                    }
                }
                count = 0;

                addToCompList(editor);
                compToFieldMap[editor.getName()] = mField;
                allControls[++allControlCount] = editor;
            }
        };

        this.getRoot = function () {
            return $table;
        };

        this.getComponents = function () {
            return allControls;
        }

        this.getLinkComponents = function () {
            return allLinkControls;
        }

        this.dispose = function () {
            allLinkControls.length = 0;
            allLinkControls = null;

            while (allControls.length > 0) {
                allControls.pop().dispose();
            };



            // console.log(compToFieldMap);
            for (var p in compToFieldMap) {
                compToFieldMap[p] = null;
                delete compToFieldMap[p];
            }
            compToFieldMap = null;

            // console.log(groupToCompsMap);
            for (var p1 in groupToCompsMap) {
                groupToCompsMap[p1].length = 0;
                groupToCompsMap[p1] = null;
                delete groupToCompsMap[p];
            }
            groupToCompsMap = null;

            allControlCount = null;
            allControls = null;
            $table.remove();
            $table = null;
            this.addField = null;
            addRow = null;
            addToCompList = null;
        };
    };




    //****************************************************//
    //**             VTable                            **//
    //**************************************************//
    function VTable() {

        this.grid = null;
        this.id = null;
        this.$container = null;
        this.rendered = false;

        this.onSelect = null;
        this.onCellEditing = null;
        this.onCellValueChanged = null;


        this.onSort = null;
        this.onEdit = null;
        this.onAdd = null;

        this.editColumnIndex = -1;


        var self = this;
        var editColumn = {
            caption: "",
            sortable: false,
            render: function (record) {
                return '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/pencil.png"  />';
            },
            size: '25px'
        };

        this.getEditColumn = function () {
            return editColumn;
        }

        this.onClick = function (evt) {
            // console.log(evt);
            if (this.readOnly)
                return;

            self.skipEditing = false;
            if (this.records.length > 0) {
                if (this.records[self.mTab.getCurrentRow()].recid != evt.recid) {
                    self.skipEditing = true;
                }
            };

            //if (isNaN(evt)) {
            //    if (evt.column === self.editColumnIndex) {
            //        var recids = self.grid.getSelection();
            //        if (recids.indexOf(parseInt(evt.recid)) > -1) {
            //            if (self.onEdit) {
            //                self.onEdit(evt.recid);
            //            }
            //        }
            //        //evt.isCancelled = true;

            //    }
            // }
            //else alert(evt);
            //console.log(" double click");
        };

        this.onSingleClick = function (evt) {
            //this.cRecid = evt.recid;
            //console.log("click");
        };

        this.onSelectLocal = function (evt) {

            if (self.blockSelect) {
                self.blockSelect = false;
                return;
            }
            if (self.onSelect) {
                self.onSelect(evt);
            }
        };

        this.checkCellEditable = true;

        function isCellEditable(column) {

            if (self.readOnly)
                return false;

            var col = self.grid.columns[column];

            if (!self.mTab.getTableModel().getIsInserting() && col.readOnly) {
                return false;
            }
            else {
                var field = col.gridField; // .Column.  CurrentCell.OwningColumn.Name);
                if (field.getIsEditable(true, true))//|| _headerClicked)
                {
                    return true;
                }
            }
            return false;
        };

        this.onEditField = function (evt) {

            if (self.skipEditing) {
                self.skipEditing = false;
                evt.isCancelled = true;
                this.select({ recid: evt.recid });
                return;
            }

            evt.isCancelled = !isCellEditable(evt.column) || (evt.originalEvent && evt.originalEvent.altKey);
            //self.checkCellEditable = false; // if cell is checked for edit then mark flag to false to skip repeateable check
            self.blockSelect = !evt.isCancelled
        };

        this.onChange = function (evt) {

            if (self.grid.columns[evt.column].editable.type == 'checkbox') { // check box field on fire on edit field event
                if (this.records.length > 0) {
                    if (this.records[self.mTab.getCurrentRow()].recid != evt.recid) {
                        evt.isCancelled = true;
                        //window.setTimeout(function(t){
                        //    t.onChange(evt);
                        //},1,this);
                        return;
                    }
                };
                evt.isCancelled = !isCellEditable(evt.column);
            }
            //self.checkCellEditable = true;

            evt.onComplete = function (event) {

                // if (event.value_original != event.value_new) {
                var evta = { newValue: event.value_new, propertyName: self.grid.columns[event.column].field };

                if (self.onCellValueChanged) {
                    self.onCellValueChanged(evta, self.grid.columns[event.column].editable.type == 'checkbox');
                }
                //  }
            }
        };

        this.onUnSelect = function (evt) {
            //var recids = self.grid.getSelection();
            //if (recids.length == 1 && this.cRecid === recids[0]) {
            //    evt.isCancelled = true;
            //}
        };

        this.onRowAdd = function (evt) {
            self.paintRow(evt.index);
        };

        this.paintRow = function (index) {

            var rec = this.grid.records[index];
            if (!checkRowEditable(index, 0)) {

                rec.style = "background-color:whitesmoke";

                return;
            }

            if (rec && rec.style)
                delete rec['style'];
        };

        function checkRowEditable(index, col) {
            if (self.readOnly)
                return false;

            //  IsActive Column always editable if no processed exists
            if (col == self.indexActiveColumn && self.indexProcessedColumn == -1)
                return true;
            //	Row
            if (!isRowEditable(index))
                return false;

            return true;
        };

        function isRowEditable(row) {
            if (self.readOnly || row < 0)
                return false;
            //	If not Active - not editable
            if (self.indexActiveColumn && self.indexActiveColumn > 0)		//	&& m_TabNo != Find.s_TabNo)
            {
                //Object value = GetCellValue(this.Rows[row], _indexActiveColumn);
                var value = self.grid.getCellValue(row, self.indexActiveColumn);

                if (typeof value == "boolean") {
                    if (!value)
                        return value;
                }

                else if (!"True".equals(value))// instanceof Boolean)
                {
                    //if (!((Boolean)value).booleanValue())
                    return false;
                }
                else if ("N".equals(value))
                    return false;
            }
            //	If Processed - not editable (Find always editable)
            if (self.indexProcessedColumn && self.indexProcessedColumn > 0)		//	&& m_TabNo != Find.s_TabNo)
            {
                //Object processed = GetCellValue(this.Rows[row], _indexProcessedColumn);
                var processed = self.grid.getCellValue(row, self.indexProcessedColumn);
                if (typeof processed == "boolean") {
                    if (processed)
                        return false;
                }

                else if ("True".equals(processed))// instanceof Boolean)
                {
                    return false;
                }
                else if ("Y".equals(processed))
                    return false;
            }
            //
            //int[] co = GetClientOrgRecordID(this.Rows[row]);
            var co = getClientOrgRecordID(row);
            var AD_Client_ID = co[0];
            var AD_Org_ID = co[1];
            var Record_ID = co[2];

            return VIS.MRole.canUpdate
                (AD_Client_ID, AD_Org_ID, self.AD_Table_ID, Record_ID, false);

        };

        function getClientOrgRecordID(row) {
            var AD_Client_ID = -1;
            if (typeof self.indexClientColumn != "undefined" && self.indexClientColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexClientColumn);//].Value;
                if (ii != null && ii !== "")
                    AD_Client_ID = VIS.Utility.Util.getValueOfInt(ii);
            }
            var AD_Org_ID = 0;
            if (typeof self.indexOrgColumn != "undefined" && self.indexOrgColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexOrgColumn);
                if (ii != null && ii !== "")
                    AD_Org_ID = VIS.Utility.Util.getValueOfInt(ii);
            }
            var Record_ID = 0;
            if (typeof self.indexKeyColumn != "undefined" && self.indexKeyColumn != -1) {
                var ii = self.grid.getCellValue(row, self.indexKeyColumn);
                if (ii != null && ii !== "")
                    Record_ID = VIS.Utility.Util.getValueOfInt(ii);
            }

            return [AD_Client_ID, AD_Org_ID, Record_ID];
        };

        //this.onToolBarClick = function (target, data) {
        //    //self.$editBtn.img = "icon-reload";
        //    if (target.startsWith("Edit")) {
        //        if (self.onEdit) {
        //            self.onEdit();
        //        }
        //    }
        //    else if (target.startsWith("Add")) {
        //        if (self.onAdd) {
        //            self.onAdd();
        //        }
        //    }
        //};

        this.disposeComponenet = function () {
            self = null;
            editColumn = null;
            this.getEditColumn = null;
            this.onClick = null;
        };
    };

    VTable.prototype.ROW_ADD = 'A';
    VTable.prototype.ROW_DELETE = 'D';
    VTable.prototype.ROW_REFRESH = 'F';
    VTable.prototype.ROW_UNDO = 'U';

    VTable.prototype.setupGridTable = function (aPanel, grdFields, $container, name, mTab, gc) {

        if (!mTab.getIsDisplayed(true))
            return 0;

        this.id = name;
        this.$container = $container;
        this.mTab = mTab;
        this.AD_Table_ID = this.mTab.getAD_Table_ID();


        var oColumns = [];
        var mField = null;
        var size = grdFields.length;
        var visibleFields = 0;

        var mFields = grdFields.slice(0);

        mFields.sort(function (a, b) { return a.getMRSeqNo() - b.getMRSeqNo() });

        var j = -1;
        for (var i = 0; i < mFields.length; i++) {
            mField = mFields[i];
            if (mField == null)
                continue;
            var columnName = mField.getColumnName();
            var displayType = mField.getDisplayType();

            //if (VIS.DisplayType.ID == displayType || columnName == "Created" || columnName == "CreatedBy"
            //                                    || columnName == "Updated" || columnName == "UpdatedBy") {
            //    if (!mField.getIsDisplayed()) {
            //        continue;
            //    }
            //}

            ++j;
            if (mField.getIsKey())
                this.indexKeyColumn = j;
            else if (columnName.equals("IsActive"))
                this.indexActiveColumn = j;
            else if (columnName.equals("Processed"))
                this.indexProcessedColumn = j;
            else if (columnName.equals("AD_Client_ID"))
                this.indexClientColumn = j;
            else if (columnName.equals("AD_Org_ID"))
                this.indexOrgColumn = j;

            var isDisplayed = mField.getIsDisplayedMR ? mField.getIsDisplayedMR() : mField.getIsDisplayed();

            var mandatory = mField.getIsMandatory(false);      //  no context check
            var readOnly = mField.getIsReadOnly();
            var updateable = mField.getIsEditable(false);      //  no context check
            //int WindowNo = mField.getWindowNo();

            //  Not a Field
            if (mField.getIsHeading())
                continue;

            var oColumn = {

                resizable: true
            }

            oColumn.gridField = mField;

            if (readOnly || !updateable) {
                oColumn.readOnly = true;   //
            }

            oColumn.caption = mField.getHeader();
            if (mandatory) {
                oColumn.caption += '<sup style="color:red;font-size: 11px;top: 0;">*</sup>';
            }
            oColumn.field = columnName.toLowerCase();
            oColumn.hidden = !isDisplayed;
            var columnWidth = oColumn.gridField.getColumnWidth();

            if (columnWidth) {
                oColumn.size = columnWidth + 'px';
            }
            else {
                oColumn.size = '100px';
            }

            if (displayType == VIS.DisplayType.Amount) {
                oColumn.sortable = true;
                oColumn.customFormat = VIS.DisplayType.GetNumberFormat(displayType);
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    //if (record.changes && typeof record.changes[f] != 'undefined') val = record.changes[f];
                    return parseFloat(oColumns[colIndex].customFormat.GetFormatedValue(val)).toLocaleString();
                };
            }
            else if (VIS.DisplayType.IsNumeric(displayType)) {
                oColumn.sortable = true;
                oColumn.customFormat = VIS.DisplayType.GetNumberFormat(displayType);
                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    //if (record.changes && typeof record.changes[f] != 'undefined') val = record.changes[f];
                    //return oColumns[colIndex].customFormat.GetFormatedValue(val);
                    return Globalize.format(Number(oColumns[colIndex].customFormat.GetFormatedValue(val)));
                };
            }
            //	YesNo
            else if (displayType == VIS.DisplayType.YesNo) {

                oColumn.sortable = true;
                var lCol = columnName.toLowerCase();
                //oColumn.render = function (record, index, colIndex) {
                //    var chk = (record[oColumns[colIndex].field]) ? "checked" : "";
                //    //console.log(chk);
                //    return '<input type="checkbox" ' + chk + ' disabled="disabled" >';
                //}
                oColumn.editable = { type: 'checkbox' };
            }
            //	String (clear/password)
            else if (displayType == VIS.DisplayType.String
                || displayType == VIS.DisplayType.Text || displayType == VIS.DisplayType.TextLong
                || displayType == VIS.DisplayType.Memo) {


                oColumn.sortable = true;

                if (mField.getIsEncryptedField()) {
                    oColumn.render = function (record, index, colIndex) {
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }

                        var val = record[oColumns[colIndex].field];
                        if (val || (val === 0))
                            return val.replace(/\w|\W/g, "*");
                        return "";
                    }
                }
                else {
                    oColumn.render = function (record, index, colIndex) {
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }



                        if (val || val == 0) {
                            //if (d.toString().indexOf('<') > -1)
                            //    return "";
                            val = w2utils.encodeTags(val);
                            return val;
                        }
                        return "";
                    }
                }

            }

            else if (VIS.DisplayType.IsLookup(displayType) || displayType == VIS.DisplayType.ID) {

                oColumn.sortable = true;


                oColumn.lookup = mField.getLookup();

                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;

                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            //val = record.changes[f];
                        }
                        var d;
                        if (l) {
                            d = l.getDisplay(val, true);
                            //if (d.startsWith("<"))
                            //  d = l.getDisplay(nd, false);
                            //d = w2utils.encodeTags(d);
                        }

                        return d;
                        //return '<span>' + d + '</span>';
                    }
                }
            }
            //Date /////////
            else if (VIS.DisplayType.IsDate(displayType)) {

                oColumn.sortable = true;
                oColumn.displayType = displayType;

                //oColumn.render = 'date';
                oColumn.render = function (record, index, colIndex) {
                    var col = oColumns[colIndex];
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }



                    if (val)
                        if (col.displayType == VIS.DisplayType.Date) {
                            var d = new Date(val);
                            d.setMinutes(d.getTimezoneOffset() + d.getMinutes());
                            //val = Globalize.format(d, 'd');
                            val = d.toLocaleDateString();
                        }
                        else if (col.displayType == VIS.DisplayType.DateTime)
                            val = new Date(val).toLocaleString();
                        //val = Globalize.format(new Date(val), 'f');
                        else
                            val = new Date(val).toTimeString();
                    //val = Globalize.format(new Date(val), 't');
                    else val = "";
                    return val;
                }
            }

            else if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator) {

                oColumn.sortable = true;

                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.AmtDimension) {

                oColumn.sortable = true;
                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.ProductContainer) {

                oColumn.sortable = true;
                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }


                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }

                        return val;
                    }
                }
            }


            else if (displayType == VIS.DisplayType.Account || displayType == VIS.DisplayType.PAttribute) {

                oColumn.sortable = true;

                oColumn.lookup = mField.getLookup();
                if (isDisplayed) {
                    oColumn.render = function (record, index, colIndex) {
                        var l = oColumns[colIndex].lookup;
                        var f = oColumns[colIndex].field;
                        var val = record[f];
                        if (record.changes && typeof record.changes[f] != 'undefined') {
                            val = record.changes[f];
                        }



                        if (l) {
                            val = l.getDisplay(val, true);
                            val = w2utils.encodeTags(val);
                        }
                        return val;
                    }
                }
            }



            else if (displayType == VIS.DisplayType.PAttribute) {

                oColumn.sortable = true;

                oColumn.render = 'int';
            }

            else if (displayType == VIS.DisplayType.Button) {

                oColumn.sortable = true;

                //oColumn.render = function (record) {
                //    return '<div>button</div>';
                //}
            }

            else if (displayType == VIS.DisplayType.Image) {

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (!val) {
                        val = "-";
                        return val;
                    }
                    return VIS.Msg.getElement1('AD_Image_ID') + '-' + val;
                }
            }

            else if (VIS.DisplayType.IsLOB(displayType)) {

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (!val) {
                        val = "";
                    }
                    return "#" + val.toString().length;
                }
            }

            else { //all text Type Columns

                oColumn.sortable = true;

                oColumn.render = function (record, index, colIndex) {
                    var f = oColumns[colIndex].field;
                    var val = record[f];
                    if (record.changes && typeof record.changes[f] != 'undefined') {
                        val = record.changes[f];
                    }

                    if (val || val == 0) {
                        //if (d.toString().indexOf('<') > -1)
                        //    return "";
                        val = w2utils.encodeTags(val);
                        return val;
                    }
                    return "";
                }
            }
            if (!oColumn.hidden) {
                visibleFields++;
            }
            oColumns.push(oColumn);
            oColumn.columnName = columnName;


            var iControl = VIS.VControlFactory.getControl(mTab, mField, false, false, false);
            iControl.setReadOnly(false);

            if (!oColumn.editable)
                oColumn.editable = { type: 'custom', ctrl: iControl };

            iControl.addVetoableChangeListener(gc);

            if (iControl instanceof VIS.Controls.VButton) {
                iControl.addActionListner(aPanel);
            }
        }

        if (visibleFields > 0) {

            var w = Math.floor(100 / visibleFields);
            for (var p in oColumns) {
                if (oColumns[p].hidden) {
                }
                else {
                    if (!oColumns[p].size < 0) {

                        oColumns[p].size = w + '%';
                        //oColumns[p].size = 150+'px';
                        //oColumns[p].size = w + '%';
                        oColumns[p].min = 100;
                    }

                }
            }
        }


        //oColumns[oColumns.length - 1].size = "100%";
        this.grid = $().w2grid({
            name: name,
            autoLoad: false,
            columns: oColumns,
            records: [],
            show: {

                toolbar: false,  // indicates if toolbar is v isible
                columnHeaders: true,   // indicates if columns is visible
                lineNumbers: false,  // indicates if line numbers column is visible
                selectColumn: true,  // indicates if select column is visible
                toolbarReload: false,   // indicates if toolbar reload button is visible
                toolbarColumns: true,   // indicates if toolbar columns button is visible
                toolbarSearch: false,   // indicates if toolbar search controls are visible
                toolbarAdd: false,   // indicates if toolbar add new button is visible
                toolbarDelete: false,   // indicates if toolbar delete button is visible
                toolbarSave: false,   // indicates if toolbar save button is visible
                selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                recordTitles: false	 // indicates if to define titles for records
            },
            //toolbar: {
            //    items: [
            //        //{ type: 'spacer' },
            //        { type: 'break' },
            //        //{ type: 'button', id: 'Add_' + name,  img: 'icon-Add' },
            //        { type: 'button', id: 'Edit_' + name, img: 'icon-edit' },
            //        { type: 'break' },
            //        { type: 'button', id: 'Add_' + name, img: 'icon-add' }
            //    ],
            //    onClick: this.onToolBarClick
            //    //{

            //    //    console.log(data);
            //    //}
            //},
            recordHeight: 36,
            onSelect: this.onSelectLocal,
            onUnselect: this.onUnSelect,
            onSort: this.onSort,
            onClick: this.onSingleClick,
            onDblClick: this.onClick,
            onEditField: this.onEditField,
            onChange: this.onChange,
            onRowAdd: this.onRowAdd
            //onResize: function () { alert('resize') }

        });
        //this.grid.selectType = 'cell';;
        return size;
    };

    VTable.prototype.get = function (recid, isIndex) {
        return this.grid.get(recid, isIndex);
    };

    VTable.prototype.activate = function () {
        if (this.grid && !this.rendred) {
            this.$container.w2render(this.grid['name']);
            this.rendred = true;
        }
        else {
            //this.grid.refresh();
            //this.grid.resize();

        }
    };

    VTable.prototype.setReadOnly = function (ro) {
        this.readOnly = ro;
    };

    VTable.prototype.getGrid = function () {
        return this.grid;
    };

    VTable.prototype.select = function (recid) {

        var selIds = this.grid.getSelection();
        if (selIds.indexOf(recid) != -1)
            return;
        if (selIds.length == 1) {
            this.grid.unselect(selIds[0]);
        }
        else if (selIds.length > 1) {
            while (selIds.length > 0) {
                this.grid.unselect(selIds.pop());
            }
        }
        return this.grid.select(recid);
    };

    VTable.prototype.add = function (records) {
        this.grid.add(records);
    };

    VTable.prototype.clear = function () {
        this.grid.clear(true);
        this.grid.reset();
    };

    VTable.prototype.refresh = function () {
        this.grid.refresh();
    };

    VTable.prototype.resize = function () {
        this.grid.resize();//
    };

    VTable.prototype.setRow = function (record) {
        if (isNaN(record)) {
            this.grid.set(record.recid, record);
        }
        else {
            this.grid.set(record);
        }
    };

    VTable.prototype.refreshRow = function (row, isRecid) {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        if (sel < 0) {
            return;
        }

        if (typeof row != 'undefined')
            sel = row;

        this.paintRow(isRecid ? this.grid.get(row, true) : sel);

        this.grid.clearRowChanges(isRecid ? row : this.grid.records[sel].recid);

    };

    VTable.prototype.refreshCells = function () {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        this.paintRow(sel);

        this.grid.refreshRow(this.grid.records[sel].recid);
    };

    VTable.prototype.refreshUndo = function () {

        if (this.grid.records.length < 1)
            return;
        var sel = this.mTab.getCurrentRow();

        this.grid.clearRowChanges(this.grid.records[sel].recid);

        this.paintRow(sel);
        this.grid.refreshRow(this.grid.records[sel].recid);
    };

    VTable.prototype.getSelection = function (retIndex) {
        return this.grid.getSelection(retIndex);
    };

    VTable.prototype.getSelectedRows = function () {
        var indexs = this.grid.getSelection(true);
        var rows = [];
        for (var i = 0, j = indexs.length; i < j; i++) {
            rows.push(this.grid.records[indexs[i]]);
        }
        return rows;
    };

    VTable.prototype.getColumnNames = function () {
        var cols = this.grid.columns;
        var colObj = {};
        for (var i = 0, j = cols.length; i < j; i++) {
            colObj[cols[i].columnName] = cols[i].caption;
        }
        return colObj;
    };

    VTable.prototype.scrollInView = function (index) {
        this.grid.scrollIntoView(index);
    };

    VTable.prototype.tableModelChanged = function (action, args, actionIndexOrId) {

        this.blockSelect = true;

        if (action === VIS.VTable.prototype.ROW_REFRESH) {
            this.setRow(args); //record 
            this.refreshRow(args.recid, true);
        }

        else {

            var id = null;
            if (action === VIS.VTable.prototype.ROW_UNDO) {
                this.grid.unselect(args);
                this.grid.remove(args);
                if (actionIndexOrId >= (this.grid.records.length - 1) && this.grid.records.length > 0) {
                    this.blockSelect = false;  //fire select event to update single layout
                    id = this.grid.records[this.grid.records.length - 1].recid;
                }
                else if (actionIndexOrId < this.grid.records.length) {
                    id = this.grid.records[actionIndexOrId].recid; // just select grid row
                }
            }
            else if (action === VIS.VTable.prototype.ROW_ADD) {
                this.grid.records.splice(actionIndexOrId, 0, args);// add at index
                id = args.recid; // row to select
                this.grid.refresh(); //refresh Grid
                this.blockSelect = true; // forcefully block select changed event
            }

            else if (action === VIS.VTable.prototype.ROW_DELETE) {
                //var size = args.length;
                var argsL = args.slice();
                while (argsL.length > 0) {
                    var recid = argsL.pop();
                    this.grid.unselect(recid);
                    this.grid.remove(recid);
                }
                if (isNaN(actionIndexOrId)) //recid array In this case
                {
                    id = actionIndexOrId[0];
                }
                else {
                    if (this.grid.records.length > 0)
                        id = this.grid.records[(this.grid.records.length - 1) < actionIndexOrId ? (this.grid.records.length - 1) : actionIndexOrId].recid;
                }
            }



            if (id) {
                this.select(id); //Select Row
            }
        }

        this.blockSelect = false;
    };

    //Set Default Focus for grid... Not in use Yet.
    VTable.prototype.setDefaultFocus = function (colName) {
        var selIndices = this.grid.getSelection();  //this.grid.getSelection(true);
        var colIndex = this.grid.getColumn(colName.toLower(), true)
        if (selIndices && selIndices.length > 0) {
            ///this.grid.editField(selIndices[0], colIndex);
            this.grid.dblClick(selIndices[0], { metaKey: true });
        }
    };

    VTable.prototype.dispose = function () {
        this.grid.off("select", this.onSelect);
        this.grid.off("sort", this.onSort);
        this.grid.off("click", this.onClick);
        this.onSelect = null;
        this.onSort = null;

        var cols = this.grid.columns;

        for (var col in cols) {
            if (cols[col].editable.ctrl)
                cols[col].editable.ctrl.dispose();
        }

        // console.log(this.grid);
        this.grid.destroy();
        // console.log(this.grid);
        this.grid = null;
        this.id = null;
        this.$container = null;
        this.rendered = null;
        this.disposeComponenet();
    };

    //*************** END VTABLE   *********************//

    /**
     *	Card view Container
     * 
     */

    function VCardView() {

        this.cGroupInfo = {}; //group control
        this.cCols = [];  //Include Column
        this.cConditions = []; //Conditions
        this.cGroup = null;//Group Column
        this.mTab;
        this.AD_Window_ID;
        this.AD_Tab_ID;
        this.groupCtrls = [];
        this.fields = [];// card view fields
        this.grpCount = 0;
        this.grpColName = '';
        this.hasIncludedCols = false;
        // this.aPanel;
        this.onCardEdit = null;

        var root;
        var body = null;
        var headerdiv;
        //  var cardList;
        function init() {
            root = $("<div class='vis-cv-body vis-noselect'>");
            body = $("<div class='vis-cv-main'>");
            headerdiv = $("<div class='vis-cv-header'>");
            //   cardList = $("<img style='margin-left:10px;margin-top:4px;float:right' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/defaultCView.png' >");
            root.append(body);
        }

        //function eventHandle() {
        //    var selfff = this;
        //    var $menu = $('<ul class="vis-apanel-rb-ul-card"></ul>');

        //    cardList.on("click", function () {

        //        $menu.empty();

        //        var selfee = this;
        //        var url = VIS.Application.contextUrl + "CardView/GetCardView";
        //        $.ajax({
        //            type: "GET",
        //            async: false,
        //            url: url,
        //            dataType: "json",
        //            contentType: 'application/json; charset=utf-8',
        //            data: { ad_Window_ID: self.mTab.getAD_Window_ID(), ad_Tab_ID: self.mTab.getAD_Tab_ID() },
        //            success: function (data) {
        //                data = JSON.parse(data);

        //                if (data && data.length > 0 && data[0].lstCardViewData && data[0].lstCardViewData.length > 0) {

        //                    for (var i = 0; i < data[0].lstCardViewData.length; i++) {
        //                        if (data[0].lstCardViewData[i].DefaultID) {
        //                            $menu.append($('<li style="background-color:white;overflow:auto;border:white;border-bottom:2px #EBEBEB solid;"  data-id="' + data[0].lstCardViewData[i].CardViewID + '" ><p style="margin:5px 5px 5px 0px;font-size:12px;float:left" data-id="' + data[0].lstCardViewData[i].CardViewID + '"  >' + data[0].lstCardViewData[i].CardViewName + '</p><span data-id="' + data[0].lstCardViewData[i].CardViewID + '" title="' + VIS.Msg.getMsg("DefaultSearch") + '" class="VIS-winSearch-defaultIcon"></span> </div></li>'));
        //                        }
        //                        else {
        //                            $menu.append($('<li style="background-color:white;overflow:auto;border:white;border-bottom:2px #EBEBEB solid;"  data-id="' + data[0].lstCardViewData[i].CardViewID + '" ><p style="margin:5px 5px 5px 0px;font-size:12px;float:left" data-id="' + data[0].lstCardViewData[i].CardViewID + '"  >' + data[0].lstCardViewData[i].CardViewName + '</p><span data-id="' + data[0].lstCardViewData[i].CardViewID + '" title="' + VIS.Msg.getMsg("MakeDefaultSearch") + '" class="VIS-winSearch-NonDefaultIcon"></span> </div></li>'));
        //                        }
        //                    }
        //                }

        //                $(selfee).w2overlay($menu.clone(true),
        //                    {
        //                        options: {
        //                            css: { width: 'auto' },
        //                            onShow: function (e) {
        //                                console.log(e);
        //                            }
        //                        }
        //                    }
        //               );
        //                //var item = [];
        //                //if (data && data.length > 0 && data[0].lstCardViewData && data[0].lstCardViewData.length > 0) {
        //                //    for (var i = 0; i < data[0].lstCardViewData.length; i++) {
        //                //        if (data[0].lstCardViewData[i].DefaultID) {
        //                //            item.push({ id: i, text: data[0].lstCardViewData[i].CardViewName, img: 'VIS-winSearch-defaultIcon' });
        //                //        }
        //                //        else {
        //                //            item.push({ id: i, text: data[0].lstCardViewData[i].CardViewName, img: 'VIS-winSearch-NonDefaultIcon' });
        //                //        }
        //                //    }
        //                //}

        //                //$(selfee).w2menu({
        //                //    items: item,
        //                //    onSelect: function (event) {
        //                //        console.log(event);
        //                //        var target = $(event.target);
        //                //        AD_CardView_ID = target.data("id");

        //                //    }
        //                //});


        //            }
        //        });
        //    });


        //    $menu.on("click", function (e) {
        //        var target = $(e.target);
        //        AD_CardView_ID = target.data("id");
        //        if (!AD_CardView_ID)
        //        {
        //            return;
        //        }
        //        if (target.is('span')) {
        //            $.ajax({
        //                type: "GET",
        //                url: VIS.Application.contextUrl + "CardView/SetDefaultView",
        //                dataType: "json",
        //                data: { AD_Tab_ID: self.mTab.getAD_Tab_ID(), cardView: AD_CardView_ID },
        //                success: function (data) {

        //                },
        //                error: function (err) {
        //                    console.log(err);
        //                }
        //            });
        //        }


        //        var url = VIS.Application.contextUrl + "CardView/GetCardViewColumns";
        //        $.ajax({
        //            type: "GET",
        //            async: false,
        //            url: url,
        //            dataType: "json",
        //            contentType: 'application/json; charset=utf-8',
        //            data: { ad_CardView_ID: AD_CardView_ID },
        //            success: function (data) {
        //                var dbResult = JSON.parse(data);
        //                var CVColumns = dbResult[0].lstCardViewData;
        //                var LstCardViewCondition = dbResult[0].lstCardViewConditonData;

        //                if (CVColumns != null && CVColumns.length > 0) {
        //                    var cardViewUserID = CVColumns[0].UserID;

        //                    var incColumns = [];
        //                    for (var i = 0; i < CVColumns.length; i++) {
        //                        if (CVColumns[i].AD_Field_ID == 0) {
        //                            continue;
        //                        }
        //                        incColumns.push(CVColumns[i].AD_Field_ID);
        //                    }


        //                    var retVal = {};
        //                    retVal.FieldGroupID = CVColumns[0].AD_GroupField_ID;
        //                    retVal.IncludedCols = incColumns;
        //                    retVal.Conditions = [];
        //                    retVal.AD_CardView_ID = AD_CardView_ID;
        //                    retVal.Conditions = dbResult[0].lstCardViewConditonData;
        //                    self.setCardViewData(retVal);


        //                    self.refreshUI(self.aPanel.curGC.getVCardPanel().width());

        //                }
        //            }, error: function (errorThrown) {
        //                VIS.ADialog.error(errorThrown.statusText);
        //            }
        //        })


        //    });


        //};
        var self = this;

        init();
        //eventHandle();

        this.getRoot = function () {
            return root;
        };
        this.getBody = function () {
            return body;
        };

        this.setHeader = function (txt) {
            headerdiv.text(txt);
        }
        this.getHeader = function () {
            return headerdiv;
        }

        //this.getCardList = function () {
        //    return cardList;
        //};


        this.sizeChanged = function (h, w) {
            root.height((h - 12) + 'px');
            this.calculateWidth(w);
        }

        this.calculateWidth = function (width) {
            //set width
            var grpCtrlC = this.groupCtrls.length;
            if (grpCtrlC < 1)
                return;
            if (width) {

                var tGrpW = 262 * grpCtrlC;
                if (tGrpW > width) {
                    body.width(262 * grpCtrlC);
                }
                else {
                    body.width(width);
                    var newW = Math.ceil(width / grpCtrlC) - 24;
                    while (grpCtrlC > 0) {
                        --grpCtrlC;

                        this.groupCtrls[grpCtrlC].setWidth(newW);
                    }
                }
            }
            else
                body.width(262 * (grpCtrlC));
            this.navigate();
        };


        //body.on('click', 'span.vis-cv-card-edit', function (e) {
        //    if (self.onCardEdit) {
        //        var s = $(e.target).data('recid');
        //        if (s || s === 0) {
        //            self.onCardEdit({ 'recid': s })
        //        }
        //    }
        //    e.stopPropagation();
        //});

        body.on('click', 'div.vis-cv-card', function (e) {

            if (self.onCardEdit) {
                var d = $(e.target);
                var s;
                if (d[0].nodeName == 'SPAN' && d.hasClass('vis-cv-card-edit')) {
                    s = d.data('recid');
                    if (s || s === 0) {
                        self.onCardEdit({ 'recid': s })
                    }
                }
                else {
                    var i = 0;
                    while (!d.hasClass('vis-cv-card')) {
                        if (i > 5)
                            break;
                        d = d.parent();
                        i++
                    }
                    s = d.data('recid');
                    if (s || s === 0) {
                        self.onCardEdit({ 'recid': s }, true)
                        self.navigate(s, false, true);
                    }
                }
            }
            e.stopPropagation();
        });

        this.getAD_CardView_ID = function () {
            return this.AD_CardView_ID;
        };

        this.getField_Group_ID = function () {
            if (this.cGroup)
                return this.cGroup.getAD_Field_ID();
            return 0;
        };

        var curCard = null;
        var crid = null;
        this.navigate = function (rid, oset, skipScroll) {
            if (rid)
                crid = rid;
            if (oset)
                return;

            if (curCard && curCard.length > 0)
                curCard.toggleClass("vis-cv-card-selected");     //.find('span.vis-cv-card-selected').css({ 'display': 'none' });

            curCard = body.find('div.vis-cv-card[name~=vc_' + crid + ']');
            if (curCard.length != 0) {
                //curCard.find('span.vis-cv-card-selected').css({ 'display': 'block' });
                curCard.toggleClass("vis-cv-card-selected");
                if (!skipScroll)
                    curCard[0].scrollIntoView();
            }
        };

        this.dC = function () {

            body.off('click');
            this.onCardEdit = null;
            self = null;

            root.remove();
            body.remove();
            headerdiv.remove();

            root = body = headerdiv = null;

            this.getRoot = this.getBody = this.setHeader = this.getHeader = this.dC = null;
            curCard = null;
            this.cGroupInfo = {}; //group control
            this.cCols.length = 0;
            this.cConditions.length = 0;
            this.cGroup = null;//Group Column
            this.mTab = null;
            this.fields.length = 0;
            this.grpCount = null;

            while (this.groupCtrls.length > 0) {
                this.groupCtrls.pop().dispose();
            }
        };
    };

    VCardView.prototype.tableModelChanged = function (action, args, actionIndexOrId) {

        // this.blockSelect = true;
        var id = null;
        if (action === VIS.VTable.prototype.ROW_REFRESH) {
            if (args.recid)
                id = args.recid;
            else
                id = args;
        }

        else {

            if (action === VIS.VTable.prototype.ROW_UNDO) {
                //this.grid.unselect(args);
                this.getBody().find('div.vis-cv-card[name~=vc_' + args + ']').remove();
            }

            else if (action === VIS.VTable.prototype.ROW_DELETE) {
                var argsL = args.slice();
                while (argsL.length > 0) {
                    var recid = argsL.pop();
                    this.getBody().find('div.vis-cv-card[name~=vc_' + recid + ']').remove();
                }
                if (isNaN(actionIndexOrId)) //recid array In this case
                {
                    id = actionIndexOrId[0];
                }
                else {
                    //if (this.grid.records.length > 0)
                    //   id = this.grid.records[(this.grid.records.length - 1) < actionIndexOrId ? (this.grid.records.length - 1) : actionIndexOrId].recid;
                }
            }
            else if (action === VIS.VTable.prototype.ROW_ADD) {
                id = args.recid; // ro
            }
        }
        if (id) {
            this.navigate(id);
        }
    };

    VCardView.prototype.setupCardView = function (aPanel, mTab, cContainer, vCardId) {
        this.mTab = mTab;
        // this.aPanel = aPanel;
        var self = this;
        VIS.dataContext.getCardViewInfo(mTab.getAD_Window_ID(), mTab.getAD_Tab_ID(), function (retData) {
            //init 
            //var retData = {};
            // retData.Group = "AccessLevel" //C_UOM_ID";
            // retData.InCol = ['Name', 'Description', 'Help', 'C_UOM_ID']
            // retData.Conditions = [];
            self.setCardViewData(retData);

        });

        //this.createGroups();
        //  cContainer.append(this.getCardList()).append(this.getHeader());
        cContainer.append(this.getHeader());
        cContainer.append(this.getRoot());
    };

    VCardView.prototype.setCardViewData = function (retData) {
        this.hasIncludedCols = false;
        this.fields = [];
        this.cGroup = null;
        this.cConditions = [];
        if (retData) {

            this.AD_CardView_ID = retData.AD_CardView_ID;

            var f = this.mTab.getFieldById(retData.FieldGroupID)
            if (f) {
                this.cGroup = f;
            }
            // self.cCols = retData.IncludedCols;
            this.cConditions = retData.Conditions;

            for (var i = 0; i < retData.IncludedCols.length; i++) {

                var f = this.mTab.getFieldById(retData.IncludedCols[i]);
                if (f)
                    this.fields.push(f);

                this.hasIncludedCols = true;
            }
        }

        if (this.fields.length < 1) {

            //Check for Included column
            // var aFields = this.mTab.gridTable.gridFields.slice(); //copy of array
            //var f = null;
            //while (aFields.length > 0) {
            //    f = aFields.pop();
            //    if (f.getIsIncludedColumn()) {
            //        this.fields.unshift(f);
            //    }
            //}

            if (this.fields.length < 1) {
                f = this.mTab.getField('Name');
                if (f) {
                    this.fields.push(f);
                }
                var f = this.mTab.getField('Description');
                if (f) {
                    this.fields.push(f);
                }
                var f = this.mTab.getField('Help');
                if (f) {
                    this.fields.push(f);
                }
            }
        }
        for (var p in this.cGroupInfo) {
            this.cGroupInfo[p].records.length = 0;
        }
        this.cGroupInfo = {};
        this.grpCount = 0;

        this.isProcessed = false;
    };

    VCardView.prototype.createGroups = function () {
        //1 get Grup field
        // var groupName = [];
        //  var groupValue = [];

        if (this.isProcessed) {
            for (var p in this.cGroupInfo) {
                this.cGroupInfo[p].records = [];
            }
            return;
        }

        if (this.cGroup) {

            var field = this.cGroup;
            if (field) {
                if (field.getDisplayType() == VIS.DisplayType.YesNo) {
                    this.cGroupInfo['true'] = { 'name': 'Yes', 'records': [] };
                    this.cGroupInfo['false'] = { 'name': 'No', 'records': [] };
                    this.grpCount = 2;
                }
                else if (VIS.DisplayType.IsLookup(field.getDisplayType()) && field.getLookup()) { //TODO: check validated also

                    //getlookup
                    var lookup = field.getLookup();
                    lookup.fillCombobox(true, true, true, false);

                    var data = lookup.data;

                    for (var i = 0; i < data.length; i++) {
                        this.cGroupInfo[data[i].Key] = { 'name': data[i].Name, 'records': [] };
                        this.grpCount += 1;
                        //if (i >= 4)
                        //    break;
                    }
                }
                this.setHeader(field.getHeader());
            }
            else {
                this.setHeader('');
            }
        }
        if (this.grpCount < 1) {//add one group by de
            this.cGroupInfo['All'] = { 'name': VIS.Msg.getMsg('All'), 'records': [] };
            this.grpCount = 1;
        }
        this.isProcessed = true;
    };

    VCardView.prototype.refreshUI = function (width) {

        this.createGroups();

        var records = this.mTab.getTableModel().mSortList;


        var root = this.getBody();

        while (this.groupCtrls.length > 0) {
            this.groupCtrls.pop().dispose();
        }

        this.groupCtrls.length = 0;

        root.empty();

        var cardGroup = null;
        if (this.grpCount == 1) {

            var n = '';
            for (var p in this.cGroupInfo) {
                n = this.cGroupInfo[p].name;
                break;
            }

            cardGroup = new VCardGroup(true, records, n, this.fields, this.cConditions);
            this.groupCtrls.push(cardGroup);
            root.append(cardGroup.getRoot())
        }
        else {
            this.filterRecord(records);
            for (var p in this.cGroupInfo) {
                cardGroup = new VCardGroup(this.grpCount === 1, this.cGroupInfo[p].records, this.cGroupInfo[p].name, this.fields, this.cConditions);
                this.groupCtrls.push(cardGroup);
                root.append(cardGroup.getRoot())
            }
        }
        this.calculateWidth(width);
    };

    VCardView.prototype.filterRecord = function (records) {
        var len = records.length;

        var grpCol = this.cGroup.getColumnName().toLowerCase();
        var record = null;
        var colValue = null;

        for (var i = 0; i < len; i++) {
            record = records[i];

            colValue = record[grpCol];
            if (this.cGroupInfo[colValue])
                this.cGroupInfo[colValue].records.push(record);
            else {
                if (!this.cGroupInfo['Other__1']) {
                    this.cGroupInfo['Other__1'] = { 'name': 'Others', 'records': [] };
                    this.grpCount += 1;
                }
                this.cGroupInfo['Other__1'].records.push(record);
            }
        }

        var eCols = [];
        for (var p in this.cGroupInfo) {
            if (this.cGroupInfo[p].records.length < 1) {
                eCols.push(p);
            }
        }
        this.grpCount -= eCols.length;

        while (eCols.length > 0) {
            delete this.cGroupInfo[eCols.pop()];
        }
    };

    VCardView.prototype.dispose = function () {
        this.dC();
    };

    /* Group Control */
    function VCardGroup(onlyOne, records, grpName, fields, conditions) {

        //conditions = [{ 'bgColor': '#80ff80', 'cValue': '@AD_User_ID@=1005324 & @C_DocTypeTarget_ID@=132' }];

        var root = null;
        var body;
        var cards = [];

        function init() {
            var str = "<div class='vis-cv-cg vis-pull-left'> <div class='vis-cv-head' >" + grpName
                + "</div><div class='vis-cv-grpbody'></div></div>";
            root = $(str);
            body = root.find('.vis-cv-grpbody');
            if (onlyOne) {
                root.css({ 'margin-right': '0px', 'width': '100%' });
                //root.width('100%');
            }
        };
        init();


        function createCards() {
            var card = null;
            for (var i = 0; i < records.length; i++) {
                card = new VCard(fields, records[i]);
                if (onlyOne) {
                    card.getRoot().width("240px").css({ 'margin': '5px 12px 12px 5px', 'float': (VIS.Application.isRTL ? 'right' : 'left') });
                }
                cards.push(card);
                body.append(card.getRoot());
                card.evaluate(conditions)
            }
        };
        createCards();

        this.getRoot = function () {
            return root;
        };

        this.getBody = function () {
            return body;
        };

        this.setWidth = function (w) {
            root.width(w);
        };

        this.dC = function () {
            while (cards.length > 0) {
                cards.pop().dispose();
            }
            root.remove();
            root = null;
            body = null;
            this.getBody = null;
            this.getRoot = null;
        };

    };

    VCardGroup.prototype.dispose = function () {
        this.dC();
    };

    /* Card View Control */
    function VCard(fields, record) {
        this.record = record;

        var root = $('<div class="vis-cv-card" data-recid=' + record.recid + ' name = vc_' + record.recid + ' ></div>');

        //root.append($("<i class='pin'></i>"));
        // root.append($('<span class="glyphicon glyphicon-hand-down vis-cv-card-selected"></span>'));
        var pencil = $('<span class="glyphicon glyphicon-pencil vis-cv-card-edit vis-pull-right" data-recid=' + record.recid + '></span>');
        root.append(pencil);

        var field = null;
        var dt;
        //createview
        for (var i = 0; i < fields.length; i++) {
            field = fields[i];
            var value = record[field.getColumnName().toLowerCase()];
            dt = field.getDisplayType();

            if (VIS.DisplayType.IsLookup(dt)) {
                value = field.getLookup().getDisplay(value);
            }

            else if (VIS.DisplayType.YesNo == dt) {
                if (value || value == 'Y')
                    value = VIS.Msg.getMsg('Yes');
                else
                    value = value = VIS.Msg.getMsg('No');
            }

            else if (VIS.DisplayType.IsDate(dt)) {
                if (value) {
                    if (dt == VIS.DisplayType.Date)
                        value = Globalize.format(new Date(value), 'd');
                    else if (dt == VIS.DisplayType.DateTime)
                        value = Globalize.format(new Date(value), 'f');
                    else
                        value = Globalize.format(new Date(value), 't');
                }
                else value = null;
            }

            if (!value && value != 0)
                value = ' -- ';
            value = w2utils.encodeTags(value);

            var span = "";
            //if (i != 0)
            //    span = "<p>" + field.getHeader() + " : <strong>" + value + "<strong></p>";
            //else
            if (VIS.Application.isRTL)
                span = "<p><strong title='" + value + "'>" + value + "</strong> :" + field.getHeader() + "</p>";
            else
                span = "<p>" + field.getHeader() + ": <strong title='" + value + "'>" + value + "</strong></p>";

            root.append($(span));
        };

        this.setColor = function (bc, fc) {
            if (bc)
                root.css('background-color', bc);
            if (fc)
                root.css('color', bc);
        };





        pencil.on('touchstart', function () {
            $(this).css({ 'color': 'gray' });
        });

        this.getRoot = function () {
            return root;
        };

        this.setWidth = function (w) {
            root.width(w)
        };

        this.dC = function () {
            pencil.off('touchstart mouseover');
            root.remove();
            root = null;
            this.getRoot = null;
            this.dc = null;
        };
    };

    VCard.prototype.evaluate = function (cnds) {
        //if (!cnds)
        //{
        //    return;
        //}
        var c = null;
        for (var i = 0; i < cnds.length; i++) {
            c = cnds[i];
            if (!c.ConditionValue)
                continue;
            if (VIS.Evaluator.evaluateLogic(this, c.ConditionValue)) {
                this.setColor(c.Color, c.FColor);
            }
        }
    };

    VCard.prototype.getValueAsString = function (vName) {
        var val = this.record[vName.toLowerCase()];
        if (val) {
            if (val === true) {
                val = 'Y';
            }
            else if (val && val.toString().endsWith('.000Z')) {

                val = val.replace('.000Z', 'Z');
            }
            val = val.toString();
        }
        else if (val === false)
            val = 'N';
        else if (val === 0) {
            val = '0';
        }

        return val;
    };

    VCard.prototype.dispose = function () {
        this.dC();
    };



    /* Map View */

    function VMapView(lstCols) {

        var map = null;
        var root = $('<div class="vis-mv-main">');
        var mapDiv = "";
        var busyDiv = "";
        var markers = [];
        var mapProp;
        var bounds = null;
        var cmbLoc = null;
        var cmbDiv = null;
        var self = this;

        var isMapAvail = window.google && google.maps ? true : false;




        function addMarker(location, msg) {
            var marker = new google.maps.Marker({
                position: location,
                animation: google.maps.Animation.DROP,
                map: map,
                title: msg
            });
            marker.info = new google.maps.InfoWindow({
                content: msg
            });

            marker.info.open(map, marker);//(map, this);  

            google.maps.event.addListener(marker, 'click', function (point) {
                //this = marker  
                this.info.open(map, this);//(map, this);  
            });
            markers.push(marker);
        };

        function addMarkerWithTimeout(location, msg, timeout) {
            window.setTimeout(function () {
                addMarker(location, msg);
            }
                , timeout);
        };


        // Sets the map on all markers in the array.
        function setAllMap(map) {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(map);
                google.maps.event.clearListeners(markers[i], 'click');
                markers[i].info = null;
            }
        };
        // Removes the markers from the map, but keeps them in the array.
        function clearMarkers() {
            setAllMap(null);
        };

        // Shows any markers currently in the array.
        function showMarkers() {
            setAllMap(map);
        };

        // Deletes all markers in the array by removing references to them.
        function deleteMarkers() {
            clearMarkers();
            markers = [];
        };

        function removeMap() {
            deleteMarkers();
            //mapDiv.remove();
            map = null;
        };

        function renderMap() {
            if (!map) {
                map = new google.maps.Map(mapDiv[0], mapProp);
                fillCombo();


            }
        };

        function fillCombo() {
            var html = '';
            var f = self.mapFields;
            if (f.length > 1) {

                for (var i = 0; i < f.length; i++) {
                    html += '<option value=' + f[i].getColumnName() + ' >' + f[i].getHeader() + '</option>';
                }
                cmbLoc.html(html);
                cmbLoc[0].selectedIndex = 0;
                bindEvent();
            }
            else {
                cmbDiv.hide();
                mapDiv.css('top', '0');
            }
        };

        function initialize() {

            mapDiv = $('<div class="vis-mv-map">');
            busyDiv = $('<div class="vis-apanel-busy vis-full-height">').hide();
            cmbDiv = $('<div class="vis-mv-header"> <select class="vis-mv-select vis-pull-right" /> </div>');
            cmbLoc = cmbDiv.find(".vis-mv-select");
            root.append(cmbDiv).append(mapDiv).append(busyDiv);
            if (isMapAvail) {
                mapProp = {
                    center: new google.maps.LatLng(26, 76),
                    zoom: 4,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
            }
        };

        initialize();

        function bindEvent() {
            cmbLoc.on("change", function (e) {
                self.setBusy(true);
                self.curIndex = this.selectedIndex;
                self.setMapData(self.mapcols[self.curIndex]);
            });
        };

        this.setBusy = function (busy) {
            if (busy)
                busyDiv.show();
            else
                busyDiv.hide();
        };

        this.getRoot = function () {
            return root;
        };

        this.sizeChanged = function (h, w) {

        };

        this.setMapData = function (lstLatLng) {
            if (!isMapAvail)
                return;

            renderMap();
            deleteMarkers();
            bounds = null;
            bounds = new google.maps.LatLngBounds();


            google.maps.event.trigger(map, 'resize');
            // window.setTimeout(function () {
            var len = lstLatLng.length;
            for (var i = 0; i < len; i++) {
                if (!lstLatLng[i].Latitude || !lstLatLng[i].Longitude)
                    continue;
                var ll = null;
                try {

                    ll = new google.maps.LatLng(Number(lstLatLng[i].Latitude), Number(lstLatLng[i].Longitude));
                    addMarkerWithTimeout(ll, lstLatLng[i].msg, 1 * 100);

                    //addMarker(ll, lstLatLng[i].msg);
                    bounds.extend(ll);
                    map.fitBounds(bounds);
                }
                catch (e) {
                    console.log(e);
                }
            }
            map.fitBounds(bounds);
            self.setBusy(false);
            //}, 10);
        };

        this.dc = function () {
            removeMap();
            root.remove();

            this.cols = this.gc = this.aPanel = this.mapcols = null;
            this.mapFields = null;
            this.curIndex = 0;

            this.getRoot = null;
            this.dc = null;
        };

    };

    VMapView.prototype.setupMapView = function (aPanel, GC, mTab, mapContainer, vMapId) {

        this.mapFields = [];


        var cols = mTab.getMapColumns();

        for (var i = 0; i < cols.length; i++) {
            var f = mTab.getField(cols[i]);
            if (f)
                this.mapFields.push(f);
        }
        this.cols = cols;

        this.gc = GC;
        this.aPanel = aPanel;
        this.mapcols = {};
        this.curIndex = 0;

        mapContainer.append(this.getRoot());
    };

    VMapView.prototype.refreshUI = function (width) {

        var records = this.gc.getSelectedRows();
        var len = records.length;
        if (records.length < 1 || this.cols.length < 1)
            return;
        var mapcols = [[]];

        for (var i = 0; i < this.cols.length; i++) {
            var colName = this.cols[i];
            var l = this.mapFields[i].getLookup();
            var locIds = [];
            for (var j = 0; j < len; j++) {
                var lid = records[j][colName.toLowerCase()];
                if (lid) {
                    var ll = l.getLatLng(lid);
                    if (ll) {
                        ll.msg = l.getDisplay(lid);
                        locIds.push(ll);
                    }
                }
            }
            this.mapcols[i] = locIds;
        }
        this.setMapData(this.mapcols[this.curIndex]);
    };

    VMapView.prototype.dispose = function () {
        this.dc();
    };


    /**
     *	Record Information 
     * - show record id and logged changed values
     */

    VIS.RecordInfo = function (_title, _dse) {

        /** The Data		*/
        var m_data = null;
        var title = _title;
        /**	Logger			*/
        var log = VIS.Logging.VLogger.getVLogger("VIS.RecordInfo");

        /** Info			*/
        var info = "";

        var $root = $("<div style='min-width:550px;max-width:600px'>");
        var ch = null;

        //  data Information

        var dataIn = {
            "CreatedBy": _dse.CreatedBy,
            "Created": _dse.Created,
            "Updated": _dse.Updated,
            "UpdatedBy": _dse.UpdatedBy,
            "Info": _dse.Info,
            "AD_Table_ID": _dse.AD_Table_ID,
            "Record_ID": _dse.Record_ID
        }



        VIS.dataContext.getJSONData(VIS.Application.contextUrl + "JsonData/GetRecordInfo", { dse: dataIn }, function (data) {
            $root.html("<span>" + data.Info + "</span>");
            if (data.ShowGrid) {

                var tbl = $("<table class='vis-advancedSearchTable'>");
                var tRoot = $("<div class='vis-advancedSearchTableWrap vis-table-responsive' style='max-height:200px;overflow-y:auto'>").append(tbl);

                $root.append(tRoot);

                var html = "";
                var htm = "", obj = null;

                for (var i = 0, j = data.Rows.length; i < j; i++) {
                    if (i == 0) {
                        html += '<thead><tr class="vis-advancedSearchTableHead">';

                        for (var k = 0; k < data.Headers.length; k++) {

                            html += '<th>' + data.Headers[k] + '</th>';
                        }
                        html += '</tr></thead><tbody>';
                    }
                    htm = '<tr class="vis-advancedSearchTableRow">';
                    obj = data.Rows[i];
                    htm += '<td>' + obj["AD_Column_ID"] + '</td><td>' + obj["NewValue"] + '</td>' +
                        '<td>' + obj["OldValue"] + '</td><td>' + obj["UpdatedBy"] + '</td><td>' + Globalize.format(new Date(obj["Updated"]), 'f') + '</td>';
                    htm += '</tr>';
                    html += htm;
                }
                html += '</tbody>';
                tbl.html(html);
            }
        });

        function dispose() {
            m_data = null;
            log = null;
            $root.remove();
            $root = null;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setTitle(title);
            ch.setContent($root);
            ch.close = function () {
                dispose();
            }
            ch.show();
        };
    };

    //function ProcessRunDialog(content, isBackground) {
    //    var $mainDiv = $('<div class="vis-PopupContent-alert vis-PopupContent-alert-info vis-PopupContent-alert-Confirm">');
    //    var $divMain = $('<div class="vis-PopupInput-alert">');
    //    var $lblContent = $('<label style="width: 90%;padding-left: 10px; word-break: break-word;">');
    //    var images = $('<img class="vis-alert-img" style="float:left" src="'+VIS.Application.contextUrl + 'Areas/VIS/Images/base/confirm-icon.png'+'">');
    //    var $chkbackground = $('<input type="checkbox"></input>');
    //    var ch = null;
    //    if (content && content.length > 0) {
    //        $lblContent.text(content);
    //    }
    //    $chkbackground.prop('checked', isBackground);
    //    if (isBackground == true) {
    //        $chkbackground.prop('disabled', true);
    //    }
    //    else {
    //        $chkbackground.prop('disabled', '');
    //    }

    //    var dResult = false;
    //    this.onClose=null;
    //    $mainDiv.append($divMain.append(images).append($lblContent).append($chkbackground));

    //    this.show = function () {
    //        ch = new VIS.ChildDialog();
    //        ch.setWidth(300);
    //        ch.setTitle(VIS.Msg.getMsg("Confirm"));
    //        ch.setModal(true);
    //        ch.setContent($mainDiv);
    //        ch.show();
    //        ch.onOkClick = ok;
    //        ch.onCancelClick = cancel;
    //        ch.onClose = close;
    //    };

    //    this.result = function () {
    //        return dResult;
    //    }

    //    this.isBackground = function () {
    //        return $chkbackground.is('checked');
    //    };

    //    function ok() {
    //        dResult = true;
    //        ch.close();
    //    };


    //    function close()
    //    {
    //        this.onClose();
    //    };

    //    function cancel() {
    //        dResult = false;
    //        ch.close();
    //    };

    //};





    //Assignment Gobal Namespace
    VIS.AWindow = AWindow;
    VIS.AppsAction = AppsAction;
    VIS.APanel = APanel;
    VIS.VTable = VTable;
    VIS.VCardView = VCardView;
    VIS.VMapView = VMapView;
    //VIS.ProcessRunDialog = ProcessRunDialog;

}(VIS, jQuery));