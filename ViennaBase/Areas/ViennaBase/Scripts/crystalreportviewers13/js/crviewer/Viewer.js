/* Copyright (c) Business Objects 2006. All rights reserved. */


/**
 * Viewer Constructor
 *
 * kwArgs.layoutType [String]  Tells the viewer how to size itself. Can be 
 *                             "client" (fill window), "fitReport", or "fixed"
 * kwArgs.width      [Int]          Width in pixels when layoutType=fixed
 * kwArgs.height     [Int]          Height in pixels when layoutType=fixed
 */
bobj.crv.newViewer = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        isDisplayModalBG : false,
        isLoadContentOnInit : false,
        layoutType  : bobj.crv.Viewer.LayoutTypes.FIXED, 
        visualStyle : {
            className       : null,
            backgroundColor : null,
            borderWidth     : null,
            borderStyle     : null,
            borderColor     : null,
            fontFamily      : null,
            fontWeight      : null,
            textDecoration  : null,
            color           : null,
            width           : '800px',
            height          : '600px',
            fontStyle       : null,
            fontSize        : null,
            top             : "0px", /* passed by Java DHTML viewer */
            left            : "0px"  /* passed by Java DHTML viewer */
       }        
    }, kwArgs);
    var o = newWidget(kwArgs.id);

    bobj.fillIn(o, kwArgs);  
    o.widgetType = 'Viewer';
    
    o._topToolbar = null;
    o._reportAlbum = null;
    o._leftPanel = null;
    o._separator = null;
    o._print = null;
    o._export = null;
    o._promptDlg = null;
    o._reportProcessing = null;
    o._eventListeners = [];
    o._statusbar = null;
    o._leftPanelResizeGrabber = newGrabberWidget(
        o.id + '_leftPanelResizeGrabber', 
        bobj.bindFunctionToObject(bobj.crv.Viewer.onGrabberMove, o),
        0, // intial left
        0, // intial top
        4, // width
        1, // intial height (has to be pixels so we'll figure it out later)
        true); // Moves on the horizontal axis        
    
    // Attach member functions 
    o.initOld = o.init;
    o._boundaryControl = new bobj.crv.BoundaryControl(kwArgs.id + "_bc");
    o._modalBackground = new bobj.crv.ModalBackground(
            kwArgs.id + "_mb",
            bobj.bindFunctionToObject(bobj.crv.Viewer.keepFocus, o));
    MochiKit.Base.update(o, bobj.crv.Viewer);
    window[o.id] = o
    
    return o;    
};

bobj.crv.Viewer = {
    LayoutTypes : {
        FIXED : 'fixed',
        CLIENT : 'client',
        FITREPORT : 'fitreport'
    },

    PromptingTypes : {
        HTML : 'html',
        FLEX : 'flex'
    },
    
    onGrabberMove : function(x) {
        if (this._leftPanel) {
            this._leftPanel.resize (x, null);
            this._doLayout ();
        }
    },
    
    keepFocus : function () {
        var swf = bobj.crv.params.FlexParameterBridge.getSWF(this.id);
        if (swf)
            swf.focus();
    },
    
    addChild : function(widget) {
        if (widget.widgetType == 'ReportAlbum') {
            this._reportAlbum = widget;
        } else if (widget.widgetType == 'Toolbar') {
            this._topToolbar = widget;
            this._separator = bobj.crv.newSeparator ();
        } else if (widget.widgetType == 'Statusbar') {
            this._statusbar = widget;
        } else if (widget.widgetType == 'PrintUI') {
            this._print = widget;
        } else if (widget.widgetType == 'ExportUI') {
            this._export = widget;
        } else if (widget.widgetType == 'ReportProcessingUI') {
            this._reportProcessing = widget;
        } else if (widget.widgetType == 'LeftPanel') {
            this._leftPanel = widget;
        }
    },

    getHTML : function() {
        var h = bobj.html;
        
        var layerStyle = {
            overflow: 'hidden',
            position: 'relative',
            left : this.visualStyle.left,
            top  : this.visualStyle.top
        };
        
        var html = h.DIV({dir: 'ltr', id:this.id, style:layerStyle,  'class':'dialogzone'},
            this._topToolbar ? this._topToolbar.getHTML() : '',
            this._separator ? this._separator.getHTML() : '',
            this._leftPanel ? this._leftPanel.getHTML() : '',
            this._reportAlbum ? this._reportAlbum.getHTML() : '',
            this._leftPanelResizeGrabber ? this._leftPanelResizeGrabber.getHTML() : '',
            this._statusbar ? this._statusbar.getHTML(): '');

        return html;
    },
    
    _onWindowResize : function() {
        if (this._currWinSize.w != winWidth () || this._currWinSize.h != winHeight ()) {
            this._doLayout ();
            this._currWinSize.w = winWidth ();
            this._currWinSize.h = winHeight ();
        }
    },
    
    init : function() {
        this.initOld ();
        this._initSignals ();

        if (this._reportAlbum)
            this._reportAlbum.init ();

        if (this._topToolbar)
            this._topToolbar.init ();

        if (this._leftPanel)
            this._leftPanel.init ();

        if (this._statusbar)
            this._statusbar.init ();

        if (this._leftPanelResizeGrabber) {
            this._leftPanelResizeGrabber.init ();

            if (!this._leftPanel || !this._leftPanel.isToolPanelDisplayed ())
                this._leftPanelResizeGrabber.setDisplay (false);
        }
        
        this.setDisplayModalBackground(this.isDisplayModalBG);

        bobj.setVisualStyle (this.layer, this.visualStyle);

        this._currWinSize = {
            w : winWidth (),
            h : winHeight ()
        };
        var connect = MochiKit.Signal.connect;
        var signal = MochiKit.Signal.signal;

        if (this.layoutType.toLowerCase () == bobj.crv.Viewer.LayoutTypes.CLIENT) {
            connect (window, 'onresize', this, '_onWindowResize');
        }

        if (!this._topToolbar && !this._statusbar && this._reportAlbum && !this._reportAlbum.isDisplayDrilldownTab()) {
            this.layer.className += ' hideFrame';
            this._reportAlbum.setHideFrame(true);
        }

        if (this.layer && _ie && bobj.checkParent (this.layer, "TABLE")) {
            /*
             * delays the call to doLayout to ensure all dom elemment's width and
             * height are set beforehand.
             */
            connect (window, 'onload', this, '_doLayoutOnLoad');
            this._oldCssVisibility = this.css.visibility;
            this.css.visibility = "hidden";
        } else {
            this._doLayout ();
        }

        this.scrollToHighlighted ();
        signal (this, 'initialized', this.isLoadContentOnInit);
    },
    
    /**
     * Connects all the signals during initialization
     */
    _initSignals : function() {
        var partial = MochiKit.Base.partial;
        var signal = MochiKit.Signal.signal;
        var connect = MochiKit.Signal.connect;
        var fe = MochiKit.Iter.forEach;

        if (this._topToolbar) {
            fe ( [ 'zoom', 'drillUp', 'firstPage', 'prevPage', 'nextPage', 'lastPage', 'selectPage', 'refresh', 'search', 'export', 'print' ], function(sigName) {
                connect (this._topToolbar, sigName, partial (signal, this, sigName));
            }, this);
        }

        this._initLeftPanelSignals ();

        if (this._reportAlbum) {
            fe ( [ 'selectView', 'removeView', 'viewChanged' ], function(sigName) {
                connect (this._reportAlbum, sigName, partial (signal, this, sigName));
            }, this);
        }

        if (this._print) {
            connect (this._print, 'printSubmitted', partial (signal, this, 'printSubmitted'));
        }

        if (this._export) {
            connect (this._export, 'exportSubmitted', partial (signal, this, 'exportSubmitted'));
        }
    },
    
    /**
     * DO NOT REMOVE. USED BY WEB ELEMENTS
     */
    getLeftPanel : function () {
        return this._leftPanel;
    },
    
    _initLeftPanelSignals : function () {
        var partial = MochiKit.Base.partial;
        var signal = MochiKit.Signal.signal;
        var connect = MochiKit.Signal.connect;
        var fe = MochiKit.Iter.forEach;
        
        if (this._leftPanel) {
            fe ( [ 'grpDrilldown', 'grpNodeRetrieveChildren', 'grpNodeCollapse', 'grpNodeExpand', 'resetParamPanel', 'resizeToolPanel' ], function(sigName) {
                connect (this._leftPanel, sigName, partial (signal, this, sigName));
            }, this);

            connect (this._leftPanel, 'switchPanel', this, '_onSwitchPanel')
        }
    },
    
    /**
     * returns true when main report view is selected in report album
     */
    _isMainReportViewSelected : function() {
        var currentView = this._reportAlbum.getSelectedView();
        return currentView && currentView.isMainReport();
    },
    
    _doLayoutOnLoad : function() {
        this.css.visibility = this._oldCssVisibility;
        this._doLayout();
    },
    
    _doLayout : function() {
        var topToolbarH = this._topToolbar ? this._topToolbar.getHeight() : 0;
        var topToolbarW = this._topToolbar ? this._topToolbar.getWidth() : 0;
        var separatorH = this._separator ? this._separator.getHeight() : 0;
        var statusbarH = this._statusbar ? this._statusbar.getHeight() : 0;
        var leftPanelW = this._leftPanel  ? this._leftPanel.getBestFitWidth() : 0;
        
        var leftPanelGrabberW = this._leftPanelResizeGrabber && this._leftPanelResizeGrabber.isDisplayed() ? 
                this._leftPanelResizeGrabber.getWidth() : 0;
        
        var layout = this.layoutType.toLowerCase();
        
        var toolPanel = this._leftPanel ? this._leftPanel.getToolPanel() : null;
        var hasPercentWidth = (toolPanel && toolPanel.isDisplayed() && toolPanel.hasPercentWidth());
        
        if (bobj.crv.Viewer.LayoutTypes.CLIENT == layout) {
            this.css.width = '100%';
            this.css.height = '100%';
            
            if (hasPercentWidth)
                leftPanelW = Math.max(leftPanelW, (this.getWidth() * toolPanel.getPercentWidth()) - leftPanelGrabberW);
        }
        else if (bobj.crv.Viewer.LayoutTypes.FITREPORT == layout) {
            var viewerWidth = 0;
            var viewerHeight = 0;
            
            if (hasPercentWidth)
                leftPanelW += 200;
            
            if(this._reportAlbum) {
                var albumSize = this._reportAlbum.getBestFitSize();
                viewerWidth = (albumSize.width + leftPanelW + leftPanelGrabberW < topToolbarW) ? topToolbarW  : albumSize.width + leftPanelW + leftPanelGrabberW;
                viewerHeight = (albumSize.height + topToolbarH + separatorH + statusbarH); 
            }
            else if (this._leftPanel) { /* If DisplayPage = false in webformviewer */
                viewerWidth = leftPanelW;
                viewerHeight = (this._leftPanel.getBestFitHeight() + topToolbarH + separatorH + statusbarH); ;
            }
            
            this.css.height = viewerHeight + 'px';
            this.css.width  = viewerWidth + 'px';
        }
        else { /* fixed layout */
            this.css.width = this.visualStyle.width;
            this.css.height = this.visualStyle.height;
            
            if (hasPercentWidth)
                leftPanelW = Math.max(leftPanelW, (this.getWidth() * toolPanel.getPercentWidth()) - leftPanelGrabberW);
        }
        
        var albumW = this.getWidth() - leftPanelW - leftPanelGrabberW;
        var albumH = Math.max(0, this.getHeight() - topToolbarH - separatorH - statusbarH);
        
        if (this._reportAlbum) {
            this._reportAlbum.resizeOuter(albumW, albumH);
            this._reportAlbum.move(leftPanelW + leftPanelGrabberW, topToolbarH + separatorH);
        }
        
        if(this._leftPanel) {
            this._leftPanel.resize(leftPanelW, albumH);
            this._leftPanel.move(0, topToolbarH + separatorH);
        }
        
        if(this._leftPanelResizeGrabber && this._leftPanelResizeGrabber.isDisplayed()) {
            this._leftPanelResizeGrabber.resize(null, albumH);
            this._leftPanelResizeGrabber.move(leftPanelW, topToolbarH + separatorH)
        }
        
        if(this._statusbar) {
            this._statusbar.doLayout();
            this._statusbar.move(0, topToolbarH + separatorH + albumH)
        }

        if (this._print && this._print.layer) {
            this._print.center();
        }
        
        if (this._export && this._export.layer) {
            this._export.center();
        }

        if (this._reportProcessing && this._reportProcessing.layer) {
            this._reportProcessing.center();
        }
        
        
        var viewerP = MochiKit.Style.getElementPosition(this.layer);
        var viewerD = MochiKit.Style.getElementDimensions(this.layer);
        
        if (this._modalBackground)
            this._modalBackground.updateBoundary(viewerD.w, viewerD.h, viewerP.x, viewerP.y);
        
        var bodyD = bobj.getBodyScrollDimension();

        var isViewerCutOff = ((viewerP.x + viewerD.w) >= bodyD.w) || ((viewerP.y + viewerD.h) >= bodyD.h);

        if(isViewerCutOff && (layout !=  bobj.crv.Viewer.LayoutTypes.CLIENT)) {

            /* BoundaryControl adds a hidden div with the same dimension and position as current viewer to body
               to fix the problem of IE regarding scrollbar that are hidden when left + viewer's width > body's width
            */

            this._boundaryControl.updateBoundary(viewerD.w, viewerD.h, viewerP.x, viewerP.y);
        }
        else {
            this._boundaryControl.updateBoundary(0, 0, 0, 0);
        }

    	var FLEXUI = bobj.crv.params.FlexParameterBridge;
    	var swf = FLEXUI.getSWF(this.id);
    	if(swf) {
    		if (this._promptDlg && this._promptDlg.style.visibility != 'hidden') {
        		if(swf._isMaximized) {
        			FLEXUI.fitScreen(this.id);
        		}
        		else {
		        	FLEXUI.resize(this.id, swf.offsetHeight, swf.offsetWidth, true);
        		}
        	}
        }
        
        this._adjustWindowScrollBars();
    },
    
    _onSwitchPanel : function(panelType) {
        var Type = bobj.crv.ToolPanelType;

        if (Type.GroupTree == panelType) {
            MochiKit.Signal.signal (this, 'showGroupTree');
        } else if (Type.ParameterPanel == panelType) {
            MochiKit.Signal.signal (this, 'showParamPanel');
        } else if (Type.None == panelType) {
            MochiKit.Signal.signal (this, 'hideToolPanel');
        }
        this._leftPanelResizeGrabber.setDisplay (!(Type.None == panelType));
        this._doLayout ();
    },
    
    resize : function(w, h) {
        if (bobj.isNumber (w)) {
            w = w + 'px';
        }

        if (bobj.isNumber (h)) {
            h = h + 'px';
        }

        this.visualStyle.width = w;
        this.visualStyle.height = h;
        this._doLayout ();
    },
    
    /** 
     * Set the page number. Updates toolbars with current page and number of pages
     * info.
     *
     * @param curPageNum [String]  
     * @param numPages   [String] (eg. "1" or "1+");
     */
    setPageNumber : function(curPageNum, numPages) {
        if (this._topToolbar) {
            this._topToolbar.setPageNumber (curPageNum, numPages);
        }
    },
    
    /**
     * Display the prompt dialog.
     *
     * @param html [string] HTML fragment to display inside the dialog's form.
     */
    showPromptDialog : function(html, closeCB) {
        if (!this._promptDlg) {
            var promptDialog_ShowCB = MochiKit.Base.bind (this._onShowPromptDialog, this);
            var promptDialog_HideCB = MochiKit.Base.bind (this._onHidePromptDialog, this);
            this._promptDlg = bobj.crv.params.newParameterDialog ( {
                id : this.id + '_promptDlg',
                showCB : promptDialog_ShowCB,
                hideCB : promptDialog_HideCB
            });
        }
        
        this._promptDlg.setCloseCB (closeCB);
        this._promptDlg.setNoCloseButton(!closeCB);

        /* The reason for saving document.onkeypress is that prompt dialog steals the document.onkeypress and never sets it back */
        this._originalDocumentOnKeyPress = document.onkeypress; /*
                                                                 * Must be set before .updateHtmlAndDisplay(html) as this function call modifies
                                                                 * document.onkeypress;
                                                                 */
        this.updatePromptDialog(html);
    },
    
    /**
     * Update the prompt dialog - with CR2010 DCP prompting this allow the dialog to be kept open during dependent prompt submits
     * 
     * @param html [string] HTML fragment to display inside the dialog's form
     */
    updatePromptDialog : function(html) {
        html = html || '';

        var callback = function(prompt, prompthtml) {
            return function() {
                prompt.updateHtmlAndDisplay (prompthtml);
            }
        };

        /**
         * AllInOne.js lacks prompting javascript files (to reduce amount of data transmitted) therefore, prompting data must be loaded on demand
         */

        bobj.loadJSResourceAndExecCallBack(bobj.crv.config.resources.HTMLPromptingSDK, callback(this._promptDlg, html));

        if (bobj.isParentWindowTestRunner ()) {
            /* for testing purposes only*/
            setTimeout (MochiKit.Base.partial (MochiKit.Signal.signal, this, "promptDialogIsVisible"), 5);
        }
    },
        
    showFlexPromptDialog : function(servletURL, closeCB) {
        var FLEXUI = bobj.crv.params.FlexParameterBridge;
        var VIEWERFLEX = bobj.crv.params.ViewerFlexParameterAdapter;

        if (!FLEXUI.checkFlashPlayer ()) {
            var msg = L_bobj_crv_FlashRequired;
            this.showError (msg.substr (0, msg.indexOf ('{0}')), FLEXUI.getInstallHTML ());
            return;
        }

        VIEWERFLEX.setViewerLayoutType (this.id, this.layoutType);
        
        if (!this._promptDlg) {
            this._promptDlg = document.createElement ('div');
            this._promptDlg.id = this.id + '_promptDlg';
            this._promptDlg.closeCB = closeCB;

            var PROMPT_STYLE = this._promptDlg.style;
            PROMPT_STYLE.border = '1px';
            PROMPT_STYLE.borderStyle = 'solid';
            PROMPT_STYLE.borderColor = '#000000';
            PROMPT_STYLE.position = 'absolute';
            PROMPT_STYLE.zIndex = bobj.constants.modalLayerIndex;

            var divID = bobj.uniqueId ();
            this._promptDlg.innerHTML = "<div id=\"" + divID + "\" name=\"" + divID + "\"></div>";
            
            // generate hidden buttons to prevent tabbing into the viewer
            var onfocusCB = bobj.bindFunctionToObject(bobj.crv.Viewer.keepFocus, this);
            
            var firstLink = MochiKit.DOM.createDOM('BUTTON', {
                id : this._promptDlg.id + '_firstLink',
                onfocus : onfocusCB,
                style : {
                    width : '0px',
                    height : '0px',
                    position : 'absolute',
                    left : '-30px',
                    top : '-30px'
                }
            });

            var lastLink = MochiKit.DOM.createDOM('BUTTON', {
                id : this._promptDlg.id + '_lastLink',
                onfocus : onfocusCB,
                style : {
                    width : '0px',
                    height : '0px',
                    position : 'absolute',
                    left : '-30px',
                    top : '-30px'
                }
            });
            
            document.body.appendChild (firstLink);
            document.body.appendChild (this._promptDlg);
            document.body.appendChild (lastLink);

            var state = bobj.crv.stateManager.getComponentState (this.id);
            var sessionID = state.common.reportSourceSessionID;
            var lang = bobj.crv.getLangCode ();

            FLEXUI.setMasterCallBack (this.id, VIEWERFLEX);
            FLEXUI.createSWF (this.id, divID, servletURL, true, lang, sessionID);
        } else {
            this._promptDlg.closeCB = closeCB;
            this._promptDlg.style.display = '';
            FLEXUI.init (this.id);
        }
        
        this.setDisplayModalBackground (true);
    },
    
    sendPromptingAsyncRequest : function (evArgs){
        MochiKit.Signal.signal(this, 'crprompt_asyncrequest', evArgs);
    },
    setDisplayModalBackground : function (isDisplay) {
        isDisplay = this.isDisplayModalBG || isDisplay; //viewer.isDisplayModalBG has higher priority        
        if(this._modalBackground)
            this._modalBackground.show(isDisplay);
    },
    
    _onShowPromptDialog : function() {
        this._adjustWindowScrollBars ();
        this.setDisplayModalBackground (true);
    },
    
    _onHidePromptDialog : function() {
        this._adjustWindowScrollBars ();
        document.onkeypress = this._originalDocumentOnKeyPress;
        this.setDisplayModalBackground (false);
    },
    
    isPromptDialogVisible: function () {
        return this._promptDlg && this._promptDlg.isVisible && this._promptDlg.isVisible (); 
    },
    
    hidePromptDialog : function() {
        if (this.isPromptDialogVisible()) {
            this._promptDlg.show (false);
        }
    },
    
    /**
     * Hide the flex prompt dialog
     */ 
    hideFlexPromptDialog : function() {
        if (this._promptDlg) {
            if (_ie)
            {
                /* IE has an issue where if a user calls back from a swf
                 and closes the containing div then when the div is shown
                 again the swf will lose any external interface calls. To get around
                 this we must set the focus to something other than the swf first
                 before hiding the window. */
                this._promptDlg.focus();
            } 

            this._promptDlg.style.visibility = 'hidden';
            this._promptDlg.style.display = 'none';
            this.setDisplayModalBackground (false);
            
            if (this._promptDlg.closeCB)
                this._promptDlg.closeCB();
        }
    },
    
    _adjustWindowScrollBars : function() {
        if (_ie && this.layoutType == bobj.crv.Viewer.LayoutTypes.CLIENT && this._promptDlg && this._promptDlg.layer && MochiKit.DOM.currentDocument ().body) {
            var bodyOverFlow, pageOverFlow;
            var body = MochiKit.DOM.currentDocument ().body;
            var promptDlgLayer = this._promptDlg.layer;

            if (this.getReportPage () && this.getReportPage ().layer) {
                var reportPageLayer = this.getReportPage ().layer;
            }

            if (!window["bodyOverFlow"]) {
                window["bodyOverFlow"] = MochiKit.DOM.getStyle (body, 'overflow');
            }

            if (body.offsetHeight < (promptDlgLayer.offsetTop + promptDlgLayer.offsetHeight)) {
                if (window["bodyOverFlow"] == "hidden") {
                    bodyOverFlow = "scroll";
                }
                pageOverFlow = "hidden";
            } else {
                bodyOverFlow = window["bodyOverFlow"];
                pageOverFlow = "auto";
            }

            body.style.overflow = bodyOverFlow;
            if (reportPageLayer) {
                reportPageLayer.style.overflow = pageOverFlow;
            }
        }
    },
    
    /**
     * Display an error message dialog.
     *
     * @param text [String]    Short, user-friendly error message
     * @param details [String] Technical info that's hidden unless the user chooses to see it  
     */
    showError : function(text, details) {
        var dlg = bobj.crv.ErrorDialog.getInstance ();
        dlg.setText (text, details);
        dlg.setTitle (L_bobj_crv_Error);
        dlg.show (true);
    },
    
    /**
     * Update the UI using the given properties
     *
     * @param update [Object] Component properties 
     */
    update : function(update) {
        if (!update || update.cons != "bobj.crv.newViewer")
            return;
        
        if(update.args)
            this.isDisplayModalBG = update.args.isDisplayModalBG;
        
        /*
         * With CR2010 DCP prompting we want to keep open the prompt dialog until all parameters 
         * are resolved (ADAPT01346079). Unfortunately, as soon as the parameters resolved, server
         * calls the getPage and returns the page content, so we don't know when to close the dialog. 
         */
        this.hidePromptDialog();

        for ( var childNum in update.children) {
            var child = update.children[childNum];
            if (child) {
                switch (child.cons) {
                    case "bobj.crv.newReportAlbum":
                        if (this._reportAlbum) {
                            this._reportAlbum.update (child);
                        }
                        break;
                    case "bobj.crv.newToolbar":
                        if (this._topToolbar) {
                            this._topToolbar.update (child);
                        }
                        break;
                    case "bobj.crv.newStatusbar":
                        if (this._statusbar) {
                            this._statusbar.update (child);
                        }
                        break;
                    case "bobj.crv.newLeftPanel":
                        if (this._leftPanel)
                            this._leftPanel.update (child);
                        else {
                            this._leftPanel = bobj.crv.createWidget(child);
                            if(this.layer) {
                                append(this.layer, this._leftPanel.getHTML())
                                this._initLeftPanelSignals ();
                                this._leftPanel.init();
                            }

                            if(this._leftPanel && this._leftPanel.isToolPanelDisplayed())
                                this._leftPanelResizeGrabber.setDisplay(true);
                        }
                        break;
                    case "bobj.crv.newExportUI":
                        if (this._export) {
                            this._export.update (child);
                        }
                        break;
                }
            }
        }

        this._doLayout ();
        this.scrollToHighlighted ();
        this.setDisplayModalBackground (this.isDisplayModalBG);
    },
    
    getToolPanel : function() {
        if(this._leftPanel)
            return this._leftPanel.getToolPanel();
        
        return null;
    },
    
    getParameterPanel : function() {
        var toolPanel = this.getToolPanel ();
        if (toolPanel)
            return toolPanel.getParameterPanel ();

        return null;
    },
    
    getReportPage : function() {
        if (this._reportAlbum) {
            var view = this._reportAlbum.getSelectedView();
            if (view) { 
                return view.reportPage;
            }
        }  
        
        return null;
    },
    
    scrollToHighlighted : function() {
        if(!this._reportAlbum)
            return;
        
        var currentView = this._reportAlbum.getSelectedView ();

        if (currentView) {
            currentView.scrollToHighlighted(this.layoutType.toLowerCase () == bobj.crv.Viewer.LayoutTypes.FITREPORT);
        }
    },

    addViewerEventListener : function (e, l) {
        var ls = this._eventListeners[e];
        if (!ls) {
            this._eventListeners[e] = [l];
            return;
        }
    
        ls[ls.length] = l;
    },
    
    removeViewerEventListener : function (e, l) {
        var ls = this._eventListeners[e];
        if (ls) {
            for (var i = 0, lsLen = ls.length; i < lsLen; i++) {
                if (ls[i] == l){
                    ls.splice(i, 1);
                    return;
                }
            }
        }
    },
    
    getEventListeners : function (e) {
        return this._eventListeners[e];
    }
};



bobj.crv.BoundaryControl = function(id) {    
    this.id = id;  
};

bobj.crv.BoundaryControl.prototype = {
    updateBoundary : function(width,height,left,top) {
        if(!this.layer) {
            this._init();
        }
        if(this.layer) {
            this.layer.style.width = width + "px";
            this.layer.style.height = height + "px";
            this.layer.style.left = left + "px";
            this.layer.style.top = top + "px";
        }
        
    },
    
    _getStyle : function () {
        return {
            display:'block',
            visibility:'hidden',
            position:'absolute'
        }; 
    },
    
    _getHTML : function () {
        return bobj.html.DIV({id : this.id, style : this._getStyle()});
    },
    
    _init: function() {
        if(!this.layer){
            append2(_curDoc.body,this._getHTML ());
            this.layer = getLayer(this.id);
            this.layer.onselectstart = function () {return false;};
            this.layer.onmousedown = eventCancelBubble;
            if (this.mouseupCB)
                this.layer.onmouseup = this.mouseupCB;
        }
    }
};

bobj.crv.ModalBackground = function (id, mouseupCB) {
    this.id = id;
    this.mouseupCB = mouseupCB;
};

bobj.crv.ModalBackground.prototype = new bobj.crv.BoundaryControl();
MochiKit.Base.update(bobj.crv.ModalBackground.prototype, {
    _getStyle : function () {
        return {
            'background-color' : '#888888',
            position : 'absolute',
            opacity : 0.30,
            display : 'block',
            filter : 'alpha(opacity=30);',
            'z-index' : bobj.constants.modalLayerIndex - 2,
            visibility : 'hidden'
        }; 
    },
    
    show : function (show) {
        if(!this.layer) {
            this._init();
        }
        
        this.layer.style.visibility = show ? "visible" : "hidden";
    }

});
