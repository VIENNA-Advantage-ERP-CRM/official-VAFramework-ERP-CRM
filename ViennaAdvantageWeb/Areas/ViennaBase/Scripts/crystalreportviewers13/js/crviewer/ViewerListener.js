/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * ViewerListener Constructor. Handles Viewer UI events.
 */
 
if(typeof(bobj.crv.Async) == 'undefined') {
    bobj.crv.Async = {};
}

bobj.crv.ViewerListener = function(viewerName, ioHandler) {
    this._name = viewerName;
    this._viewer = null;
    this._promptPage = null; 
    this._paramCtrl = null;
    this._ioHandler = ioHandler;
    this._reportProcessing = null;
    
    var connect = MochiKit.Signal.connect;
    var subscribe = bobj.event.subscribe;
    var bind = MochiKit.Base.bind;
    
    var widget = window[viewerName];
    if (widget) {
        if (widget.widgetType == 'Viewer') {
            this._viewer = widget;
            this._reportProcessing = this._viewer._reportProcessing;
        }
        else if (widget.widgetType == 'PromptPage') {
            this._promptPage = widget;    
            this._reportProcessing = this._promptPage._reportProcessing;
        }
    }
    
    if (this._viewer) {
        // ReportAlbum events
        connect(this._viewer, 'selectView', this, '_onSelectView');
        connect(this._viewer, 'removeView', this, '_onRemoveView');
        
        // Toolbar events
        connect(this._viewer, 'firstPage', this, '_onFirstPage');
        connect(this._viewer, 'prevPage', this, '_onPrevPage');
        connect(this._viewer, 'nextPage', this, '_onNextPage');
        connect(this._viewer, 'lastPage', this, '_onLastPage');
        connect(this._viewer, 'selectPage', this, '_onSelectPage');
        connect(this._viewer, 'zoom', this, '_onZoom');
        connect(this._viewer, 'drillUp', this, '_onDrillUp');
        connect(this._viewer, 'refresh', this, '_onRefresh');  
        connect(this._viewer, 'search', this, '_onSearch');
        connect(this._viewer, 'export', this, '_onExport');
        connect(this._viewer, 'print', this, '_onPrint');
        
        // Tool Panel events
        connect(this._viewer, 'resizeToolPanel', this, '_onResizeToolPanel');
        connect(this._viewer, 'hideToolPanel', this, '_onHideToolPanel');
        connect(this._viewer, 'grpDrilldown', this, '_onDrilldownGroupTree');
        connect(this._viewer, 'grpNodeRetrieveChildren', this, '_onRetrieveGroupTreeNodeChildren');
        connect(this._viewer, 'grpNodeCollapse', this, '_onCollapseGroupTreeNode');
        connect(this._viewer, 'grpNodeExpand', this, '_onExpandGroupTreeNode');
        connect(this._viewer, 'showParamPanel', this, '_onShowParamPanel');
        connect(this._viewer, 'showGroupTree', this, '_onShowGroupTree');
        connect(this._viewer, 'viewChanged',  this, '_onChangeView');
        connect(this._viewer, 'resetParamPanel',  this, '_onResetParamPanel');
        connect(this._viewer, 'printSubmitted', this, '_onSubmitPrintPdf');
        connect(this._viewer, 'exportSubmitted', this, '_onSubmitExport');
        connect(this._viewer, 'initialized', this, '_onViewerInitialization');

    }
    
    // Report Page Events
    subscribe('drilldown', this._forwardTo('_onDrilldown')); 
    subscribe('drilldownGraph', this._forwardTo('_onDrilldownGraph'));
    subscribe('drilldownSubreport', this._forwardTo('_onDrilldownSubreport'));
    subscribe('sort', this._forwardTo('_onSort'));
    subscribe('hyperlinkClicked', this._forwardTo('_onHyperlinkClicked'));
    subscribe('displayError', this._forwardTo('_displayError'));
    
    // Prompt events
    subscribe('crprompt_param', this._forwardTo('_onSubmitStaticPrompts'));
    subscribe('crprompt_pmtEngine', this._forwardTo('_onSubmitPromptEnginePrompts'));
    subscribe('crprompt_logon', this._forwardTo('_onSubmitDBLogon'));
    subscribe('crprompt_cancel',this._forwardTo('_onCancelParamDlg'));
    subscribe('crprompt_flexparam', this._forwardTo('_onFlexParam'));
    subscribe('crprompt_flexlogon', this._forwardTo('_onFlexLogon'));
    subscribe('crprompt_asyncrequest', this._forwardTo('_onPromptingAsyncRequest'));

    
    // Report Part Events
    subscribe('pnav', this._forwardTo('_onNavigateReportPart'));
    subscribe ('navbookmark', this._forwardTo('_onNavigateBookmark'));
    subscribe ('updatePromptDlg', this._forwardTo('_onPromptDialogUpdate'));

    // Universal Events, No Target Checks
    subscribe('saveViewState', bind(this._onSaveViewState, this)); 
    
    if(widget) {
        widget.init(); //must initialize widget after connecting event listeners as signals might be thrown during initialization
    }
        
    if(bobj.isObject(window.jsUnit) && !window.jsUnit.testSuite) {
        window.jsUnit.testSuite = new jsUnit.crViewerTestSuite(this);
    } 
};

bobj.crv.ViewerListener.prototype = {

    /**
     * Public
     *
     * @return The current report view 
     */
     getCurrentView: function() {
         if(this._viewer && this._viewer._reportAlbum) {
             return this._viewer._reportAlbum.getSelectedView();
         }
         
         return null;
     },
     
     getPromptingType : function () {
         return this._getCommonProperty('promptingType');
     },
     
     _displayError : function(args) {
         args = MochiKit.Base.parseQueryString(args);
         
         if (args && this._viewer ) { 
             var errorMessage = args.errorMessage || L_bobj_crv_RequestError;
             var debug = args.debug || "";
             this.showError(errorMessage, debug);
         }  
     },
     
    /**
     * Private. Wraps an event handler function with an event target check.
     *
     * @param handlerName [String]  Name of the event handler method
     *
     * @return Function that can be used as a callback for subscriptions
     */
    _forwardTo: function(handlerName) {
        return MochiKit.Base.bind(function(target) {
            if (target == this._name) {
                var args = bobj.slice(arguments, 1);
                this[handlerName].apply(this, args);
            }
        }, this);    
    },
    
    _onViewerInitialization: function (isLoadContentOnInit) {
        if(isLoadContentOnInit) {
            this._initialLoadViewerContent ();
        }
    },

    _onSaveViewState: function() {
        this._saveViewState();
    },

    _onSelectView: function(view) {
        if (view) {
            bobj.crv.logger.info('UIAction View.Select');
            
            // Since "restore state" happens before events, we need to 
            // change the curViewId before making the server request
            var state = bobj.crv.stateManager.getComponentState(this._name);
            if (state) {
                state.curViewId = view.viewStateId;
                this._request({selectView: view.viewStateId}, bobj.crv.config.useAsync, true);
            }
        }
    },
    
    _onRemoveView: function(view) {
        if (view) {
            bobj.crv.logger.info('UIAction View.Remove');
            var viewerState = bobj.crv.stateManager.getComponentState(this._name);
            if (viewerState) {
                // Remove view from viewer state
                delete viewerState[view.viewStateId];
            }
            
            var commonState = this._getCommonState();
            if (commonState) {
                // Remove view from taborder
                var idx = MochiKit.Base.findValue(commonState.rptAlbumOrder, view.viewStateId);
                if (idx != -1) {
                    arrayRemove(commonState, 'rptAlbumOrder', idx);   
                }
            }
        }
    },
    
    _initialLoadViewerContent: function() {
        bobj.crv.logger.info('UIAction InitLoad');
        this._request({ajaxInitLoad : true}, bobj.crv.config.useAsync, true);
    },
    
    _onFirstPage: function() {
        bobj.crv.logger.info('UIAction Toolbar.FirstPage');
        this._request({tb:'first'}, bobj.crv.config.useAsync, true);
    },
    
    _onPrevPage: function() {
        bobj.crv.logger.info('UIAction Toolbar.PrevPage');
        this._request({tb:'prev'}, bobj.crv.config.useAsync, true);
    },
    
    _onNextPage: function() {
        bobj.crv.logger.info('UIAction Toolbar.NextPage');
        this._request({tb:'next'}, bobj.crv.config.useAsync, true);
    },
    
    _onLastPage: function() {
        bobj.crv.logger.info('UIAction Toolbar.LastPage');
        this._request({tb:'last'}, bobj.crv.config.useAsync, true);
    },

    _onDrillUp: function() {
        bobj.crv.logger.info('UIAction Toolbar.DrillUp');
        this._request({tb:'drillUp'}, bobj.crv.config.useAsync, true);
    },
    
    _onChangeView: function() {
        if(this._paramCtrl) {
            this._paramCtrl.onChangeView();
        }
    },
    
    _onResetParamPanel: function() {
        if(this._isResettingParamPanel) {
            return;
        }
        
        this._isResettingParamPanel = true;
        
        if(this._paramCtrl) {
            this._paramCtrl.setParameters([]);
            delete this._paramCtrl;
        }  
        
        this.clearAdvancedPromptData();
        
        var onFinishCB = bobj.bindFunctionToObject( function () { 
            this._isResettingParamPanel = false; 
        }, this);
        
        this._setInteractiveParams(null, onFinishCB);
    },
    
    _onSelectPage: function(pgTxt) {
        bobj.crv.logger.info('UIAction Toolbar.SelectPage ' + pgTxt);
        this._request({tb:'gototext', text:pgTxt}, bobj.crv.config.useAsync, true);    
    },
    
    _onZoom: function(zoomTxt) {
        bobj.crv.logger.info('UIAction Toolbar.Zoom ' + zoomTxt);
        this._request({tb:'zoom', value:zoomTxt}, bobj.crv.config.useAsync, true);    
    },
    
    _onExport: function(closeCB) {
        var exportComponent = this._viewer._export;
        if (exportComponent) {
            if (closeCB) exportComponent.setCloseCB (closeCB);
            
            bobj.crv.logger.info('UIAction Toolbar.Export');
            exportComponent.show(true);
        }
    },
        
    _onPrint: function(closeCB) {
        var printComponent = this._viewer._print;
        if (printComponent) {
            if (closeCB) printComponent.setCloseCB (closeCB);
            
            if (printComponent.isActxPrinting) {
                bobj.crv.logger.info('UIAction Toolbar.Print ActiveX');
                var pageState = bobj.crv.stateManager.getCompositeState();
                var postBackData = this._ioHandler.getPostDataForPrinting(pageState, this._name);
                this._viewer._print.show(true, postBackData);
            }
            else {
                var isOneClickPrint = this._getCommonProperty('pdfOCP') && bobj.hasPDFReaderWithJSFunctionality(); 
                if (isOneClickPrint) {
                    this._onSubmitPrintPdf(0, 0, isOneClickPrint);
                }
                else {
                    bobj.crv.logger.info('UIAction Toolbar.Print PDF');
                    this._viewer._print.show(true);
                }
            }
        }
    },

    _onResizeToolPanel: function(width) {
        this._setCommonProperty('toolPanelWidth', width);
        this._setCommonProperty('toolPanelWidthUnit', 'px');
    },
    
    _onHideToolPanel: function() {
        bobj.crv.logger.info('UIAction Toolbar.HideToolPanel');
        this._setCommonProperty('toolPanelType', bobj.crv.ToolPanelType.None);
    },
    
    _onShowParamPanel: function() {
        bobj.crv.logger.info('UIAction Toolbar.ShowParamPanel');
        this._setCommonProperty('toolPanelType', bobj.crv.ToolPanelType.ParameterPanel);
    },
    
    _onShowGroupTree: function() {
        bobj.crv.logger.info('UIAction Toolbar.ShowGroupTree');
        this._setCommonProperty('toolPanelType', bobj.crv.ToolPanelType.GroupTree);
    },
    
    _onDrilldown: function(drillargs) {
        bobj.crv.logger.info('UIAction Report.Drilldown');
        this._request(drillargs, bobj.crv.config.useAsync, true);
    },
    
    _onDrilldownSubreport: function(drillargs) {
        bobj.crv.logger.info('UIAction Report.DrilldownSubreport');
        this._request(drillargs, bobj.crv.config.useAsync, true);
    },
    
    _onDrilldownGraph: function(event, graphName, branch, offsetX, offsetY, pageNumber, nextpart, twipsPerPixel) {
        if (event) {
            bobj.crv.logger.info('UIAction Report.DrilldownGraph');
            var mouseX, mouseY;
            
            if(_ie || _saf || _ie11Up) {
                mouseX = event.offsetX;
                mouseY = event.offsetY;				
            }
            else {
                mouseX = event.layerX;
                mouseY = event.layerY;				
            }
            
            var zoomFactor = parseInt(this._getCommonProperty('zoom'), 10);
            zoomFactor = (isNaN(zoomFactor) ? 1 : zoomFactor/100);
            
            this._request({ name:encodeURIComponent(graphName),
                            brch:branch,
                            coord:(mouseX*twipsPerPixel/zoomFactor + parseInt(offsetX, 10)) + '-' + (mouseY*twipsPerPixel/zoomFactor +parseInt(offsetY, 10)),
                            pagenumber:pageNumber,
                            nextpart:encodeURIComponent(nextpart)}, 
                            bobj.crv.config.useAsync, 
                            true);
        }
    },
    
    _onDrilldownGroupTree: function(groupName, groupPath, isVisible, groupNamePath) {
        bobj.crv.logger.info('UIAction GroupTree.Drilldown');
        var encodedGroupName = encodeURIComponent(groupName);
        var evtArgs = {drillname: encodedGroupName, gnpath: groupNamePath};
        if(isVisible) {
            evtArgs.grp = groupPath;
        }
        else {
            evtArgs.brch = groupPath;
        }
         
        this._request(evtArgs, bobj.crv.config.useAsync, true);
    },
    
    _onRetrieveGroupTreeNodeChildren: function(groupPath) {
        this._request({grow:groupPath}, bobj.crv.config.useAsync, true);
    },
    
    /*
     * Removes groupPath from currentExpandedPaths in current view's state
     */
    _onCollapseGroupTreeNode: function(groupPath) {
        bobj.crv.logger.info('UIAction GroupTree.CollapseNode');
        var expPathPointer = this.getCurrentExpandedPaths(); // expPathPointer is assigned to expanded paths in memory
        var groupPathArray = groupPath.split('-');

        for(var i = 0 , end = groupPathArray.length - 1; i <= end ; i++) {
            var nodeID = groupPathArray[i];
            if(expPathPointer[nodeID]) {
                if(i == end) {
                    delete expPathPointer[nodeID];
                    return;
                }
                expPathPointer = expPathPointer[nodeID];
            }
            else {
                return;
            }
        }         

    },
    
    showError: function(message,detailText) {
        if(this._viewer) {
            this._viewer.showError(message, detailText);
        }
    },
    
    _onExpandGroupTreeNode: function(groupPath) {
        bobj.crv.logger.info('UIAction GroupTree.ExpandNode');
        var expPathPointer = this.getCurrentExpandedPaths();
        var groupPathArray = groupPath.split('-');
        // Iterates through groupPath, and adds indxes to currentExpandedPaths if not existing
        for(var i = 0 , end = groupPathArray.length; i < end; i++) {
            var nodeID = groupPathArray[i];
            if(!expPathPointer[nodeID]) {
                expPathPointer[nodeID] = {};
            }
            expPathPointer = expPathPointer[nodeID];
        }   
        
    },
    
    _onRefresh: function() {
        bobj.crv.logger.info('UIAction Toolbar.Refresh');

        var commonState = this._getCommonState();
        var useAsyncForRefresh = true;
        if (commonState && commonState.useAsyncForRefresh !== undefined) {
            useAsyncForRefresh = commonState.useAsyncForRefresh;
        }
        
        this._request({tb:'refresh'}, bobj.crv.config.useAsync && useAsyncForRefresh, true);
    },
    
    _onSearch: function(searchText) {
        bobj.crv.logger.info('UIAction Toolbar.Search');
        this._request({tb:'search', text:encodeURIComponent(searchText)}, bobj.crv.config.useAsync, true);
    },
    
    /**
     * Full Prompt page should use post back request even if ajax is enabled as it is unable to process the ajax response
     */
    _canUseAsync : function () {
        return this._viewer != null && bobj.crv.config.useAsync;
    },
    
    _onFlexParam: function(paramData){
        this._request({'crprompt':'flexPromptingSetValues', 'paramList':paramData, 'isFullPrompt': true}, this._canUseAsync());
    },
    
    _onFlexLogon: function(logonData) {
    	for (var i = 0, len = logonData.length; i < len; i++) {
            this._addRequestField(logonData[i].field, logonData[i].value);
        }

        this._request({'crprompt':'logon'}, this._canUseAsync());
    },
    
    /**
     * 
     * @param isFullPrompt [String] a flag indicating whether the current prompt is full prompt which refreshes report content
     * @return
     */
    _onSubmitPromptEnginePrompts: function(isFullPrompt) {

        isFullPrompt = eval(isFullPrompt); //converting it to boolean type
        var useAjax = this._viewer && this._viewer.isPromptDialogVisible(); //use ajax request when prompt dialog is visible
        
        /*
         * this._name is used to get a unique id for prompt variables
         * For dhtml and JSF viewer it will be the name of the viewer
         * For webform viewer it will be the 'unique UI id'
         */
        var valueIDKey = 'ValueID' + this._name;
        var contextIDKey = 'ContextID' + this._name;
        var contextHandleIDKey = 'ContextHandleID' + this._name;

        var valueID = document.getElementById(valueIDKey);
        if (valueID) {
            this._addRequestField(valueIDKey, valueID.value);
        }
        
        var contextID = document.getElementById(contextIDKey);
        if (contextID) {
            this._addRequestField(contextIDKey, contextID.value);
        }
        
        var contextHandleID = document.getElementById(contextHandleIDKey);
        if (contextHandleID) {
            this._addRequestField(contextHandleIDKey, contextHandleID.value);
        }
        
        this._request({'crprompt':'pmtEngine', 'isFullPrompt': isFullPrompt}, useAjax);
        
        // These elements are dynamically created - we should delete them as soon after their job is done.
        this._removeRequestField(valueIDKey);
        this._removeRequestField(contextIDKey);
        this._removeRequestField(contextHandleIDKey);
        
    },
      
    _onSubmitStaticPrompts: function(formName) {
        this._addRequestFields(formName);
        this._request({'crprompt':'param'}, false);
    },
    
    _onSubmitDBLogon: function(formName) {
        var useAjax = this._viewer && this._viewer.isPromptDialogVisible(); //use ajax request when prompt dialog is visible
        
        if(this._viewer)
            this._viewer.hidePromptDialog(); 
        this._addRequestFieldsFromContent(formName);
        /**
         * use async when viewer is loaded (instead of promptpage) and it's allowed to make async requests
         */
        
        this._request({'crprompt':'logon'}, useAjax);
    },
    
    _onSubmitPrintPdf: function (start, end, isOneClickPrint) {
        this._handlePrintOrExport (start, end, 'PDF', isOneClickPrint);
    },
    
    _onSubmitExport: function (start, end, format) {
        this._handlePrintOrExport (start, end, format);
    },
    
    _handlePrintOrExport: function (start, end, format, isOneClickPrint) {
        var isRange = true;
        var useIframe = false;
        if (!start && !end) {
            isRange = false;
        }
        
        if (!format) {
            format = 'PDF';
        }
        
        var isOneClickPDFPrinting = (format == 'PDF' && isOneClickPrint);
        var reqObj = {text:format, range:isRange+''};
        reqObj.tb = isOneClickPDFPrinting ? 'crpdfprint' : 'crexport';
        if (isRange) {
            reqObj['from'] = start + '';
            reqObj['to'] = end + '';
        }

         bobj.crv.logger.info('UIAction Export.Submit ' + format);
        // we want to redirect export requests to a Servlet (ASP should do nothing different)
        if (this._ioHandler instanceof bobj.crv.ServletAdapter || this._ioHandler instanceof bobj.crv.FacesAdapter) {
            useIframe = true; //always use iframe for exporting/printing in Java DHTML Viewer
            this._ioHandler.redirectToServlet ();
            this._ioHandler.addRequestField ('ServletTask', 'Export');
            this._ioHandler.addRequestField ('LoadInIFrame', useIframe);
        }
        else {
            useIframe = isOneClickPDFPrinting;
        }
        this._request(reqObj, false, false, useIframe);
    },
    
    _onCancelParamDlg: function() {
        bobj.crv.logger.info('UIAction PromptDialog.Cancel');
        this._viewer.hidePromptDialog();
    },
    
    _onReceiveParamDlg: function(html) {
        this._viewer.showPromptDialog(html);
    },
    
    _onSort: function(sortArgs) {
        bobj.crv.logger.info('UIAction Report.Sort');
        this._request(sortArgs, bobj.crv.config.useAsync, true);  
    },
    
    _onNavigateReportPart: function(navArgs) {
        bobj.crv.logger.info('UIAction ReportPart.Navigate');
        this._request(navArgs, false);
    },
    
    _onNavigateBookmark: function(navArgs) {
        bobj.crv.logger.info('UIAction Report.Navigate');
        this._request(navArgs, bobj.crv.config.useAsync, true);        
    },
    
    getCurrentExpandedPaths: function() {
        var viewState = this._getViewState();
        if(viewState) {
            return viewState.gpTreeCurrentExpandedPaths;
        }

        //This return shouldn't get exectued
        return {};
    },
    
    applyParams: function(params) {
        // TODO Dave can we just set the parsm into state since they've 
        // gone through client side validation?
        if (params) {
            bobj.crv.logger.info('UIAction ParameterPanel.Apply');
            var clonedParams = [];
            var cloneFunc = MochiKit.Base.clone;
            for(var i = 0, len = params.length; i < len; i++) {
                var clonedParam = cloneFunc(params[i]);
                clonedParam.modifiedValue = null;
                clonedParam.value = cloneFunc(params[i].value); /* _encodeParameter modifes deep copy of value so I have to make a copy of it before encoding it */
                if (this._ioHandler instanceof bobj.crv.ServletAdapter || this._ioHandler instanceof bobj.crv.FacesAdapter) {
                    this._encodeParameter(clonedParam);
                }
                clonedParams.push(clonedParam);
            }            

            this._request({crprompt: 'paramPanel', paramList: clonedParams}, bobj.crv.config.useAsync, true);            
        }
    },
    
    getServletURI : function () {
        var servletURL = "";
        if (this._ioHandler instanceof bobj.crv.ServletAdapter
                || this._ioHandler instanceof bobj.crv.FacesAdapter) {
            servletURL = this._ioHandler._servletUrl;
        }
        
        return servletURL;
    },
    
    /**
    * The loading time of HTML prompting resources is changed from initialization to on demand to reduce the initial loading time.
    * When a request for advance dialog is triggered, resources are loaded prior to sending the request to server
    * @param callBack
    * @return
    */
    showAdvancedParamDialog : function(param) {
        var paramOpts = this._getCommonProperty('paramOpts');
        if(!paramOpts.canOpenAdvancedDialog) {
            this.showError(L_bobj_crv_AdvancedDialog_NoAjax, L_bobj_crv_EnableAjax);
        }
        else {
        	this._focusedParamName = param.paramName;
            
            if (this._isPromptingTypeFlex()) {    
                var flexAdapter = bobj.crv.params.ViewerFlexParameterAdapter;
                flexAdapter.setCurrentIParamInfo (this._name, this._paramCtrl, param);
                    
                if (!flexAdapter.hasIParamPromptUnitData(this._name)) {
                    /* Need to fetch the interactive prompt data*/
                    this._request({promptDlg: this._cloneParameter(param)}, true); 
                } else {
                    /* Already have enough data to prompt*/
                    /* Creating 5+ range controls can take a long time */
                    if (param.allowMultiValue && param.allowRangeValue &&
                        param.modifiedValue.length > 5) {
                        if (this._reportProcessing) {
                            this._reportProcessing.Show();
                        }
                    }
                    var closeCB = this._getPromptDialogCloseCB();
                    this._viewer.showFlexPromptDialog(this.getServletURI(), closeCB);
                }
            } else {
                 /* Need to fetch the interactive parameter HTML*/
                this._request({promptDlg: this._cloneParameter(param)}, true);    
            }
        }
    },
    
    _cloneParameter : function(param) {
        var clonedParam = MochiKit.Base.clone(param);
        clonedParam.defaultValues = null; // ADAPT00776482
        clonedParam.modifiedValue = null;
        if (this._ioHandler instanceof bobj.crv.ServletAdapter || this._ioHandler instanceof bobj.crv.FacesAdapter) {
            // we need to clone the value again
            clonedParam.value = MochiKit.Base.clone(param.value);
            clonedParam = this._encodeParameter(clonedParam);
        }
        return clonedParam;
    },
    
    _encodeParameter: function(p) {
        // we only worry about the "%" sign in the "string" values, param name and the report name.
        // ignore the other properties of the parameter because they are not used on the server side
        if (p) {
            if (p.value && p.valueDataType == bobj.crv.params.DataTypes.STRING) {
                for (var i = 0, valuesLen = p.value.length; i < valuesLen; i++) {
                    if (bobj.isString(p.value[i])) { //discrete value
                        p.value[i] = encodeURIComponent(p.value[i]);
                    }
                    else if (bobj.isObject(p.value[i])) { //range value
                        var actualValue = null;
                        if (p.value[i].beginValue) {
                            actualValue = bobj.crv.params.getValue(p.value[i].beginValue);
                            p.value[i].beginValue = encodeURIComponent(actualValue);
                        }
                        if (p.value[i].endValue) {
                            actualValue = bobj.crv.params.getValue(p.value[i].endValue);
                            p.value[i].endValue = encodeURIComponent(actualValue);
                        }
                    }
                }
            }
            
            if (p.paramName) {
                p.paramName = encodeURIComponent(p.paramName);
            }
            
            if (p.reportName) {
                p.reportName = encodeURIComponent(p.reportName);
            }
        }
        
        return p;
    },
    
    /**
     * Set a property in the state associated with the current report view
     *
     * @param propName [String]  The name of the property to set
     * @param propValue [Any]    The value of the property to set
     */
    _setViewProperty: function(propName, propValue) {
        var viewState = this._getViewState();
        if (viewState) {
            viewState[propName] = propValue;    
        }    
    },
    
    /**
     * Get a property in the state associated with the current report view
     *
     * @param propName [String]  The name of the property to retrieve
     */
    _getViewProperty: function(propName) {
        var viewState = this._getViewState();
        if (viewState) {
            return viewState[propName];
        }
        return null;
    },
    
    /**
     * Set a property that's shared by all report views from the state 
     *
     * @param propName [String]  The name of the property to set
     * @param propValue [String]  The value to set
     */
    _setCommonProperty: function(propName, propValue) {
        var state = this._getCommonState();
        if (state) {
            state[propName] = propValue;
        }
    },
    
    /**
     * Get a property that's shared by all report views from the state 
     *
     * @param propName [String]  The name of the property to retrieve
     */
    _getCommonProperty: function(propName) {
        var state = this._getCommonState();
        if (state) {
            return state[propName];
        }
        return null;
    },
    
    /**
     * Set the UI properties to match the state associated with viewId
     *
     * @param viewId [String - optional]  
     */ 
    _updateUIState: function(viewId) {
        
    },
    
    /**
     * Get the state associated with the current report view
     *
     * @return State object or null 
     */
    _getViewState: function() {
        var compState = bobj.crv.stateManager.getComponentState(this._name);
        if (compState && compState.curViewId !== undefined) {
            return compState[compState.curViewId];
        }
        return null;
    },
    
    /**
     * Get the state that's common to all report views
     *
     * @return State object or null
     */ 
    _getCommonState: function() {
        var compState = bobj.crv.stateManager.getComponentState(this._name);
        if (compState) {
            return compState.common;
        }
        return null;
    },
        
    /**
     * Create CRPrompt instances from interactive parameters in state and pass 
     * them to the Viewer widget so it can display them in the parameter panel.
     */
    _setInteractiveParams: function(paramList, onFinishCB) {
        if (!this._ioHandler.canUseAjax()) {
            var paramPanel = this._viewer.getParameterPanel();
            if (paramPanel) {
                paramPanel.showError(L_bobj_crv_InteractiveParam_NoAjax);
            }
            onFinishCB();
            return;
        }
        
        if (!paramList) {
            var stateParamList = this._getCommonProperty('iactParams');
        
            var unusedParamList = [];
            if (stateParamList) {
                var Parameter = bobj.crv.params.Parameter;
                var paramList = [];
            
                for (var i = 0; i < stateParamList.length; ++i) {
                    if (stateParamList[i].isInUse != null && !stateParamList[i].isInUse)
                        unusedParamList.push(new Parameter(stateParamList[i]));
                    else
                        paramList.push(new Parameter(stateParamList[i]));
                }
            }
        }
        
        if (paramList && paramList.length) {
            var callback = function (viewerListener, paramList) {
                return function () {
                    var paramPanel = viewerListener._viewer.getParameterPanel();
                    if (paramPanel) {
                        var paramOpts = viewerListener._getCommonProperty('paramOpts');
                        viewerListener._paramCtrl = new bobj.crv.params.ParameterController(paramPanel, viewerListener, paramOpts);
                        viewerListener._paramCtrl.setParameters(paramList, onFinishCB);
                        viewerListener._paramCtrl.setUnusedParameters(unusedParamList);
                    }
                }
            }
            
            bobj.loadJSResourceAndExecCallBack(bobj.crv.config.resources.ParameterControllerAndDeps, callback(this, paramList));
        }
        else {
            onFinishCB();
        }
    },
    
    _isPromptingTypeFlex: function() {
    	var type = this.getPromptingType();
        return (type && type.toLowerCase() == bobj.crv.Viewer.PromptingTypes.FLEX);
    },
    
    clearAdvancedPromptData: function() {
    	if (this._isPromptingTypeFlex()){
            bobj.crv.params.ViewerFlexParameterAdapter.clearIParamPromptUnitData(this._name);
        }
    },
    
    _onPromptDialogUpdate: function(update) {
        if (update.resolvedFields) {
            this._viewer.hidePromptDialog(); 

            if(this._paramCtrl) {
                for (var i = 0; i < update.resolvedFields.length; i++) {
                    var param = new bobj.crv.params.Parameter(update.resolvedFields[i]);
                    this._paramCtrl.updateParameter(param.paramName, param.getValue());
                }
                this._paramCtrl._updateToolbar();
            }
        }
        else {
            if (this._isPromptingTypeFlex()) {
                if(update.script) {
                    /* update.script contains JavaScript code for updating ViewerFlexParameterAdapter class*/
                    bobj.evalInWindow(update.script); 
                    var closeCB = this._getPromptDialogCloseCB();
                    this._viewer.showFlexPromptDialog(this.getServletURI(), closeCB);
                }
            }
            else 
            {
                if(update.html) {
                    if (this._viewer.isPromptDialogVisible()) {
                        this._viewer.updatePromptDialog(update.html);
                    }
                    else {
                        var closeCB = this._getPromptDialogCloseCB();
                        this._viewer.showPromptDialog(update.html, closeCB);
                    }
                }
            }
            
        }    
    },

    _getPromptDialogCloseCB: function () {
        var closeCB = null;
        if (this._paramCtrl && this._focusedParamName) {
            closeCB = this._paramCtrl.getFocusAdvButtonCB(this._focusedParamName);
            
            /* Clear the name so that if the dialog is used for a full prompt */
            /* it won't have the focus callback */
            this._focusedParamName = null;
        }
        return closeCB;
    },
    
    _onPromptingAsyncRequest: function(evArgs) {
        this._request(evArgs, true, false, false);
    },
    
    _request: function(evArgs, allowAsynch, showIndicator, useIframe, callback) {
        var pageState = bobj.crv.stateManager.getCompositeState();
        var bind = MochiKit.Base.bind;
        var defaultCallback = callback ? callback : bind(this._onResponse, this, evArgs);
        var defaultErrCallback = bind(this._onIOError, this);
        if (!bobj.isBoolean (showIndicator)) {
            showIndicator = true;
        }

        if (this._reportProcessing && showIndicator) {
            this._reportProcessing.delayedShow ();
        }
        
        var deferred = this._ioHandler.request(pageState, this._name, evArgs, allowAsynch, useIframe, defaultCallback, defaultErrCallback);

        if (deferred) {            
            if (this._reportProcessing && showIndicator) {
                this._reportProcessing.setDeferred (deferred);
            }
        
            deferred.addCallback(defaultCallback);

            deferred.addErrback(defaultErrCallback);
        }
    },
    
    _onResponse: function(evArgs, response) {
        var json = null;
        if (bobj.isString(response)) {
            json = MochiKit.Base.evalJSON(response);
        } else {
            json = MochiKit.Async.evalJSONRequest(response);
        }

        if (json) {
            if (json.needsReload) {
                this._request(evArgs, false, true);
                return;
            }
            
            if(json.redirect) {
                window.location = json.redirect;
                return;
            }

            if (json.status && this._viewer && (json.status.errorMessage || json.status.debug) ) { 
                var errorMessage = json.status.errorMessage || L_bobj_crv_RequestError;
                this.showError(errorMessage, json.status.debug);
            }
            
            if (json.state) {
                var jsonState = json.state;
                if (bobj.isString(jsonState)) {
                    jsonState = MochiKit.Base.evalJSON(jsonState);
                }
                bobj.crv.stateManager.setComponentState(this._name, jsonState);
            }
            
            if (json.update) {
                if (json.update.promptDlg) {
                    this._onPromptDialogUpdate(json.update.promptDlg);
                    bobj.crv.logger.info('Update InteractiveParams');    
                }
                else if (this._viewer) {
                    this._viewer.update(json.update);
                    bobj.crv.logger.info('Update Viewer');
                }
            }

            if (json.script && json.script.length > 0) {				
                bobj.evalInWindow(json.script);
                bobj.crv.logger.info('Execute Script');
            }

        }
        
        if (this._reportProcessing) {
            this._reportProcessing.cancelShow ();
        }
        
        if(bobj.isParentWindowTestRunner())
            MochiKit.Signal.signal(this._viewer, "updated");
    },    
    _onIOError: function(response) { 
    
        if (this._reportProcessing.wasCancelled () == true) {
            return;
        }
        
        if (this._viewer) {
            var error = this._ioHandler.processError (response);
            var detailText = '';
            if (bobj.isString(error)) {
              detailText = error;
            }
            else {
                for (var i in error) {
                    if (bobj.isString(error[i]) || bobj.isNumber(error[i])) {
                        detailText += i + ': ' + error[i] + '\n';     
                    }
                }
            }

            this.showError(L_bobj_crv_RequestError, detailText);    
        }

        if (this._reportProcessing) {
            this._reportProcessing.cancelShow ();
        }
    },
    
    _saveViewState: function() {
        var pageState = bobj.crv.stateManager.getCompositeState();
        this._ioHandler.saveViewState(pageState, this._name);
    },
    
    /**
     * Private. Retrieve all children of the given form and add them to the request.
     *
     * @param formName [String]  Name of the form
     */
    _addRequestFields: function(formName) {
        var frm = document.getElementById(formName);
        if (frm) {
            for (var i in frm) {
                var frmElem = frm[i];
                if (frmElem && frmElem.name && frmElem.value) {
                    this._addRequestField(frmElem.name, frmElem.value);
                }
            }
        }
    },

    /**
     * Private. Retrieve all input fields inside the content div element and add them to the request.
     *
     * @param contentId [String]  Id of the containing div element
     */
    _addRequestFieldsFromContent: function(contentId) {
        var parent = document.getElementById(contentId);
        if (!parent)
            return;

        var elements = MochiKit.DOM.getElementsByTagAndClassName("input", null, parent);
            
        for (var i in elements) {
            var inputElement = elements[i];
            if(inputElement.type && inputElement.type.toLowerCase() == "checkbox" && inputElement.name) {
                if(inputElement.checked)
                    this._addRequestField(inputElement.name, inputElement.value );
            }
            else if (inputElement && inputElement.name && inputElement.value) {
                this._addRequestField(inputElement.name, inputElement.value);
            }
        }
    },
    
    /**
     * Private. Add the given name and value as a request variable.
     *
     * @param fldName [String]  Name of the field
     * @param fldValue [String] Value of the field
     */
    _addRequestField: function(fldName, fldValue) {
        this._ioHandler.addRequestField(fldName, fldValue);
    },
    
    /**
     * Private. Add the given name and value as a request variable.
     *
     * @param fldName [String]  Name of the field
     */
    _removeRequestField: function(fldName) {
        this._ioHandler.removeRequestField(fldName);
    },
    
    _onHyperlinkClicked: function(args) {
        args = MochiKit.Base.parseQueryString(args);
        var ls = this._viewer.getEventListeners('hyperlinkClicked');
        var handled = false;
        if (ls) {
            for (var i = 0, lsLen = ls.length; i < lsLen; i++) {
                if (ls[i](args) == true){
                    handled = true;
                }
            }
        }
        
        if (handled) {return;}

        var w = window;
        if (args.target && args.target != '_self'){
            w.open(args.url, args.target);
        } else {
            w.location = args.url;
        }
    	
    }
};


