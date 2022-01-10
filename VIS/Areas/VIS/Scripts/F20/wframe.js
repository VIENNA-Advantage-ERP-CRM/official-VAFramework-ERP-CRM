; (function (VIS, $) {

    var AWINDOW_HEADER_HEIGHT = 0;// 43;
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


    var tmpWindow = document.querySelector('#vis-ad-windowtmp').content;// $("#vis-ad-windowtmp");


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
        this.img = null;
        this.cPanel; // common pointer , contain Window Panel OR Form Panel OR Process Panel
        this.isHeaderVisible = true;
        this.onClosed; // event
        this.title = "window";
        this.closeable = true;
        var $header = null;

        var $table, $contentGrid, $lblTitle, $btnClose;
        var $toolDiv = null;
        this.onLoad = null;

        function initComponent() {
            // $contentGrid = $("<div class='vis-awindow-body'>");

            var clone = document.importNode(tmpWindow, true);

            $table = $(clone.querySelector(".vis-ad-w"));
            //var td11 = $("<td style='max-height:42px;'>");
            $contentGrid = $table.find(".vis-ad-w-body");//  ("<td class='vis-height-full'>");
            $lblTitle = $table.find("h5"); //$("<h1>");//.addClass("vis-awindow-title-label");
            // Mohit - Shortcut as title.
            $btnClose = $table.find(".vis-ad-w-close");//$('<a href="javascript:void(0)" title="' + VIS.Msg.getMsg("Close") + " " + VIS.Msg.getMsg("Shct_Close") + '" class="vis-mainMenuIcons vis-icon-menuclose"></a>');

            $toolDiv = $table.find(".vis-ad-w-toolbar"); //$("<div class='vis-awindow-toolbar' >");
            $header = $table.find(".vis-ad-w-header");

            //     $header = $table.find $("<div class='vis-awindow-header vis-menuTitle' >").append($btnClose).append($lblTitle).append($toolDiv);
            //  td11.append($header);
            //var tr1 = $("<tr>").append(td11);
            //var tr2 = $("<tr>").append($contentGrid);
            //$table.append(tr1).append(tr2);
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
            return;
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
                this.closeable = false;
            }
            else {
                $btnClose.show();
                this.closeable = true;
            }
        };

        this.isCloseable = function (hide) {
            return this.closeable
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
        // this.setSize(height);
        var hHeight = this.isHeaderVisible ? 85 : 43;
        try {
            this.cPanel.sizeChanged(height - hHeight, width);
        }
        catch (ex) { console.log("size changed error"); }
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
        this.getContentGrid().css('display', 'flex'); // to support older design

        //set variable
        var windowNo = VIS.Env.getWindowNo();
        this.id = windowNo + "_" + AD_Window_ID;
        this.hid = action + "=" + AD_Window_ID;

        var self = this;

        this.hideHeader(true);


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

            //Image 
            var wObj = self.cPanel.gridWindow;
            var img = null;
            if (wObj.getFontName() != '')
                img = wObj.getFontName();
            else if (wObj.getImageUrl() != '')
                img = VIS.Application.contextUrl + "Images/Thumb16x16/" + wObj.getImageUrl(); //fixed
            else
                img = "fa fa-window-maximize";

            self.img = img;
            if (callback) {
                callback(self.id, img, self.name, self.hid); //add shortcut
            }

            if (self.onLoad)
                self.onLoad();

            // register popoverlay event for control's description
            //self.cPanel.getRoot().find('.vis-ev-ctrlinfowrap').popover({
            //    trigger: 'focus'
            //});

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


        this.cPanel = new VIS.AForm(VIS.Env.getScreenHeight() - 85); //initlize AForm

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

            self.sizeChanged();// set size and window

            var img = "fa fa-list-alt";
            self.img = img;
            if (callback) {
                callback(self.id, img, self.name, self.hid); //add shortcut
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

        this.cPanel = new VIS.AProcess(AD_Process_ID, VIS.Env.getScreenHeight() - AWINDOW_HEADER_HEIGHT, splitUI, extrnalForm); //initlize AForm

        //set variable
        var windowNo = VIS.Env.getWindowNo();
        this.id = windowNo + "_" + AD_Process_ID;
        this.hid = action + "=" + AD_Process_ID;

        var self = this;
        this.hideHeader(true);
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
            self.cPanel.setActionOrigin(VIS.ProcessCtl.prototype.ORIGIN_MENU);
            if (!self.cPanel.init(jsonData, self, windowNo)) {
                self.dispose();
                self = null;
                return;
            }


            self.sizeChanged();// set size and window

            var img = null;
            if (jsonData.FontName != '')
                img = jsonData.FontName;
            else if (jsonData.ImageUrl != '')
                img = VIS.Application.contextUrl + "Images/Thumb16x16/" + jsonData.ImageUrl; //fixed
            else if (action == "P")
                img = "fa fa-cog";
            else
                img = "vis vis-report";

            self.img = img;

            if (callback) {
                callback(self.id, img, self.name, self.hid); //add shortcut
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

        if (!this.closeable)
            return;
        //if (VIS.context.getContext("#DisableMenu") == 'Y') {
        //    return;
        //}
        //dispose all popover
        // this.cPanel.getRoot().find('.vis-ev-ctrlinfowrap').popover('dispose');
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
        return true;
    };


    //************* AWindow End ************************//





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
       isPopoverText:
       direction:

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
            this.iconName = '';
            this.direction = "right";
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
                    this.toolTipText = this.text;// VIS.Msg.getMsg(this.action);
                else {
                    this.toolTipText = this.text + VIS.Msg.getMsg(this.toolTipText);
                    //this.text = this.text + " " + this.toolTipText;
                }
                if (this.toolTipText.contains("&")) {
                    this.toolTipText = this.text + this.toolTipText.replace('&', '');
                }
            }
            var imgUrl = ''; // this.getPath();
            var imgUrlX = ''; //this.getPath();

            //var cls = '';
            //var clsX = '';

            if (this.isSmall) {
                //imgUrl += this.action + "16.png";
                imgUrl = this.iconName != '' ? this.iconName.toLowerCase() : this.action.toLowerCase();
                if (this.toggle || this.enableDisable) {
                    // imgUrlX += this.action + "X16.png";
                    imgUrlX = imgUrl + 'x';
                }
            }
            else {
                //imgUrl += this.action + "24.png";
                //if (this.toggle || this.enableDisable) {
                //    imgUrlX += this.action + "X24.png";
                //}
                imgUrl = this.iconName != '' ? this.iconName.toLowerCase() : this.action.toLowerCase();
                if (this.toggle || this.enableDisable) {
                    // imgUrlX += this.action + "X16.png";
                    imgUrlX = imgUrl + 'x';
                }
            }
            this.imgUrl = "vis-" + imgUrl;
            this.imgUrlX = "vis-" + imgUrlX;
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
                li.append($('<h5>').text(this.text));
            }
            else if (this.imageOnly) {
                //this.img = $('<img />').attr({ 'src': this.imgUrl, 'alt': this.text, 'title': this.text });
                this.img = $('<i class="vis ' + this.imgUrl + '" title = "' + this.toolTipText + '"})>');
                li.append(d);
                d.append(this.img);
            }
            else {
                //li.append('<ul class="vis-appsaction-ul-inner"><li><img src="' + this.imgUrl + '" title="' + this.text + '" /></li><li><span>' + this.text + '</span></li></ul>');
                if (this.direction == "r")
                    li.append('<span>' + this.text + '</span> <i class="vis ' + this.imgUrl + '" title="' + this.toolTipText + '" ></i>');
                else
                    li.append('<i class="vis ' + this.imgUrl + '" title="' + this.toolTipText + '"></i><span>' + this.text + '</span>');
                this.img = li.find("i");
            }
            this.$li = li;
            li.popover
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
                //this.img = $('<img />').attr({ 'src': this.imgUrl, 'alt': this.text, 'title': this.text });
                //li.append(d);
                //d.append(this.img);
                this.img = $('<i class="vis ' + this.imgUrl + '" title = "' + this.toolTipText + '"})>');
                li.append(d);
                d.append(this.img);
            }
            else {
                if (this.direction == "r")
                    li.append('<span>' + this.text + '</span> <i class="vis ' + this.imgUrl + '" title="' + this.toolTipText + '" ></i>');
                else
                    li.append('<i class="vis ' + this.imgUrl + '" title="' + this.toolTipText + '" ></i><span>' + this.text + '</span>');
                this.img = li.find("i");
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
        if (!this.toggle || this.pressed == pressed)
            return;
        this.pressed = pressed;
        if (this.img) {
            if (this.toggle) {
                //this.img.toggleClass(!pressed ? this.imgUrl : this.imgUrlX);
                if (pressed) {
                    this.img.removeClass(this.imgUrl);
                    this.img.addClass(this.imgUrlX);
                }
                else {
                    this.img.removeClass(this.imgUrlX);
                    this.img.addClass(this.imgUrl);
                }
            }
        }
    };

    AppsAction.prototype.setTextDirection = function (dir) {
        this.direction = dir;
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

    
    AppsAction.prototype.highlightNewButton = function (highlight) {
        if (highlight) {
            if (this.$li.instructionPopRemoved)
                return;

            if (!this.windowContainer) {
                this.windowContainer = this.$li.closest('.vis-ad-w-p');
            }

            if (this.windowContainer.find('.vis-window-instruc-overlay-new').length <= 0) {
                this.windowContainer.prepend('<div class="vis-window-instruc-overlay-new"><div class="vis-window-instruc-overlay-new-inn">'
                    + '<p>' + VIS.Msg.getMsg('CreateNewRec') + '</p></div></div>');

                this.$li.addClass('vis-window-instruc-overlay-new-li');
            }
        }
        else {
            if (!this.windowContainer) {
                this.windowContainer = this.$li.closest('.vis-ad-w-p');
            }
            this.windowContainer.find('.vis-window-instruc-overlay-new').remove();
            this.windowContainer.find('.vis-window-instruc-overlay-new-inn').remove();
            this.$li.removeClass('vis-window-instruc-overlay-new-li');
            this.$li.instructionPopRemoved = true;
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

    var statusTmp = document.querySelector("#vis-ad-statusbartmp").content;

    function StatusBar(withInfo) {

        var clone = document.importNode(statusTmp, true);

        var $root = $(clone.querySelector(".vis-ad-w-p-s-main"));

        this.$statusLine = $root.find(".vis-ad-w-p-s-msg").find("span");// $("<span>");
        this.$statusDB = $root.find(".vis-ad-w-p-s-statusdb");// $("<span class='vis-statusbar-statusDB'>").text("0/0");
        this.$infoLine = $root.find(".vis-ad-w-p-s-infoline");// $("<span class='vis-statusbar-infoLine'>").text("info");

        var $spanPageResult = $root.find(".vis-ad-w-p-s-result").find("span");// $("<span class='vis-statusbar-pageMsg'>");
        var $comboPage = $("<select class='vis-statusbar-combo'>");
        var $ulPages = $root.find(".vis-ad-w-p-s-plst");

        this.text = "";
        this.dse;
        this.merror;
        this.mtext;
        //
        var showText = VIS.Msg.getMsg("ShowingResult");
        var ofText = VIS.Msg.getMsg("of");

        this.onComboChange;

        function initilizeComponent() {
            $root = $("<table class='vis-statusbar-table'>");
            r1Col1 = $("<td style='text-align:center' colspan='3' >")
            r2Col1 = $("<td style='width:auto;white-space:nowrap;'>");
            r2Col2 = $("<td  style='width:100%;'>");
            r2Col3 = $("<td style='width:auto;white-space:nowrap;'>");
            $ulPages = $("<ul class='vis-statusbar-ul'>");
        };

        //initilizeComponent();
        //page Numbers
        //$ulPages.append($("<li>").append(this.$statusDB));
        // r2Col1.append($spanPageResult).append($seprator);
        //r2Col2.append($("<div class='vis-statusbar-statusLine'>").append(this.$statusLine));
        //r2Col3.append($ulPages);
        // r1Col1.append(this.$infoLine);

        if (!withInfo) {
            this.$infoLine.hide();
        }

        /* Previlige function */
        this.getRoot = function () {
            return $root;
        };

        this.render = function () {
            // $root.empty();
            // $root.append($("<tr class='vis-height-auto'>").append(r1Col1)).append($("<tr class='vis-height-full'>").append(r2Col1).append(r2Col2).append(r2Col3));
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
                if (tr == 0) {
                    s -= 1;
                }
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

            $btnLeft = $("<button" + (isReadOnly ? " disabled" : "") + " ><i class='fa fa-arrow-left'></i></button");
            $btnRight = $("<button" + (isReadOnly ? " disabled" : "") + " ><i class='fa fa-arrow-right'></i></button");
            $btnUp = $("<button" + (isReadOnly ? " disabled" : "") + " ><i class='fa fa-arrow-up'></i></button");
            $btnDown = $("<button" + (isReadOnly ? " disabled" : "") + " ><i class='fa fa-arrow-down'></i></button");

            $tblRoot = $("<table class='vis-apanel-sorttab' style='display:none;'>");
            //$tabControl = $("<ul class='vis-gc-tabs-ul'>");

            //$tblRoot.append($("<tr>").append($("<td>").append($tabControl)));

            var $td = $("<td class='vis-apanel-sorttab-td'>");
            var $divLeftContainer = $('<div class="vis-apanel-sorttab-cont">');

            $divLeftContainer.append($lstNo).append($("<div class='vis-apanel-sorttab-td-right'>").append($btnRight).append($btnLeft));

            $td.append($("<div class='vis-apanel-sorttab-td-inner'>").append($lblNo).append($divLeftContainer));

            var $divRightContainer = $('<div class="vis-apanel-sorttab-cont">');
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
            if (change) {
                this.notifyFireChanged(true);
                //this.aPanel.aSave.setEnabled(true);
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

    VIS.VSortTab.prototype.notifyFireChanged = function (enable) {
        if (this.aContentPane)
            this.aContentPane.aSave.setEnabled(enable);
        else if (this.aPanel)
            this.aPanel.aSave.setEnabled(enable);
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
            this.log.log(VIS.Logging.Level.SEVERE, sql, e);
        }
        this.notifyFireChanged(false);
        //this.aPanel.aSave.setEnabled(false);
    };

    VIS.VSortTab.prototype.getIsEnabled = function () {
        var enable = false;
        if (this.aContentPane)
            enable = this.aContentPane.aSave.getIsEnabled();
        else if (this.aPanel)
            enable = this.aPanel.aSave.getIsEnabled();
        return enable;
    };

    VIS.VSortTab.prototype.saveData = function () {

        if (!this.getIsEnabled())
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

    /**
     * add sub tab view datastatus listner 
     * --contentpane
     * @param {any} lsnr
     */
    VIS.VSortTab.prototype.addSubTabDataStatusListner = function (lstner) {
        this.aContentPane = lstner;
    };

    /**
     * Remove subtab view data status listnerlistner
     * */
    VIS.VSortTab.prototype.removeSubTabDataStatusListner = function () {
        this.aContentPane = null;
    };

    VIS.VSortTab.prototype.sizeChanged = function (height, width) {
        this.getRoot().height(height);
    };

    VIS.VSortTab.prototype.dispose = function () {
        this.disposeComponent();
    };

    //********************** END *********************//













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
            ch.setPosition({
                my: "center bottom-3",
            });
            ch.close = function () {
                dispose();
            }
            ch.show();
        };
    };







    //Assignment Gobal Namespace
    VIS.AWindow = AWindow;
    VIS.AppsAction = AppsAction;
    VIS.StatusBar = StatusBar




}(VIS, jQuery));