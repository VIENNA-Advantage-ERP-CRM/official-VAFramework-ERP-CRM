/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof(bobj) == 'undefined') {
    bobj = {};    
}

if (typeof(bobj.crv) == 'undefined') {
    bobj.crv = {};    
    
    // Constants used in the viewer
    bobj.crv.ActxPrintControl_CLSID = 'B7DA1CA9-1EF8-4831-868A-A767093EA685';
    bobj.crv.ActxPrintControl_Version = '13,0,9,1312';
    
    // Configuration for ALL viewers in a page
    // TODO The dhtmllib doesn't currently suport widgets from different locales
    //   on the same page. We will need that support for the viewer.
    bobj.crv.config = {
        isDebug : false, 
        scriptUri: null,  // The uri for the viewer script dir (that holds crv.js)
        skin: "skin_standard",
        needFallback: true, 
        lang: "en",
        useCompressedScripts: true,
        useAsync : true,
        indicatorOnly: false,
        resources : { 
          'HTMLPromptingSDK' : {isLoaded: false, path : '../../promptengine-compressed.js'}, 
          'ParameterControllerAndDeps' : {isLoaded: false, path: '../../parameterUIController-compressed.js'}  
        },
        logging: { enabled: false, id: 0}
    };
    
    bobj.crv.logger = {
    	info: function(){ /* Do Nothing */}
    };
    
    // Udate configuration with properties from crv_config, if it exists
    if (typeof(crv_config) == 'object') {
        for (var i in crv_config) {
            bobj.crv.config[i] = crv_config[i];        
        }
    }
    
    // If the uri for the bobj.crv script dir wasn't set, try to figure it out 
    if (!bobj.crv.config.scriptUri) {
        var scripts = document.getElementsByTagName("script");
        var reCrvJs = /(.*)crv\.js$/i;
        
        for(var i = 0; i < scripts.length; i++) {
            var src = scripts[i].getAttribute("src");
            if(!src) { continue; }
            
            var matches = src.match(reCrvJs);
            
            if(matches && matches.length) {
                bobj.crv.config.scriptUri = matches[1];
                break;
            }
        }
        
        if (!bobj.crv.config.scriptUri ) {
            bobj.crv.config.scriptUri = '';
            if (bobj.crv.config.isDebug) {
                throw  "bobj.crv.config.scriptUri is undefined";
            }
        }
    }
    
    /**
     * Parse a URI into its constitutents as defined in RFC2396
     *
     * @param uri [String]  Uri to parse
     *
     * @return Returns object has string values for the uri's components
     */
    bobj.parseUri = function(uri) {
        var regExp = "^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?$";
        var result = uri.match(new RegExp(regExp));
        return {
            scheme    : result[2],
            authority : result[4],
            path      : result[5],
            query     : result[7],
            fragment  : result[9]
        };
    };


    bobj.crvUri = function(uri) {
        return  bobj.crv.config.scriptUri + uri;   
    };
    
    /**
     * A helper method for evaluating whether parent window is JSUnit test runner
     */
    bobj.isParentWindowTestRunner = function() {
        try {
            var tempTop = top || window;
            while(tempTop.top && tempTop.top != tempTop) {
                tempTop = tempTop.top;
            }

            return tempTop.jsUnitTestSuite !== undefined;
        }
        
        catch(err){}
        
        return false;
    };
    
    bobj.skinUri = function(uri) {
        return bobj.crvUri("../dhtmllib/images/") + bobj.crv.config.skin + "/" + uri;
    };
    
    /**
     * Create a widget using json 
     * eg.
     * createWidget({
     *     cons: bobj.crv.myWidget,
     *     args: {foo: 'bar'},
     *     children: [ {cons: ...}, ...]
     * });
     *
     * @param json [Object]  Object representing the widget
     * @return Returns the widget or null if creation failed
     */
    bobj.crv.createWidget = function(json) { 
        if (!bobj.isObject(json)){ 
            return null;    
        }
        
        var constructor = json.cons;
        if (bobj.isString(constructor)) {  
            try {
                constructor = eval(constructor);           
            }
            catch (err) {
                if (bobj.crv.config.isDebug) {
                    throw  "bobj.crv.createWidget: invalid constructor";
                }
            }
        }
        
        if (!bobj.isFunction(constructor)) {
            if (bobj.crv.config.isDebug) {
                throw  "bobj.crv.createWidget: invalid constructor";
            }
            return null;    
        }
        
        var widget = constructor(json.args);
        
        if (bobj.isArray(json.children)) {
            if (widget.delayedBatchAdd) {
                widget.delayedBatchAdd(json.children);
            }
            else if (widget.addChild){
                for (var i = 0, len = json.children.length; i < len; ++i) {
                    var child = bobj.crv.createWidget(json.children[i]);
                    widget.addChild(child);
                }
            }
        }
        
        return widget;
    };
    
    /**
     * Create a widget using bobj.crv.createWidget and write it to the document.
     * If optional element is given, the innerHTML is written instead and scripts
     * are executed.
     *
     * @param json [Object]  Object representing the widget
     * @return Returns the widget or null if creation failed
     */
    bobj.crv.writeWidget = function(json, element) {
       
        var widget = bobj.crv.createWidget(json);
        if (widget) {
            if(element){
                var html = widget.getHTML();
                var ext = bobj.html.extractHtml(html);
                var links = ext.links;
                element.innerHTML = html;
                for(var iLinks = 0, linksLen = links.length; iLinks < linksLen; ++iLinks){
                    bobj.includeLink(links[iLinks]);
                }
                
                var scripts = ext.scripts;
                for (var iScripts = 0, scriptsLen = scripts.length; iScripts < scriptsLen; ++iScripts) {
                    var script = scripts[iScripts];
                    if (!script) {continue;}
                    
                    if (script.text) {
                        try {
                            bobj.evalInWindow(script.text);
                        }
                        catch(err) {}
                    }
                }
                
                var styleText = "";
                for (var i = 0, len = ext.styles.length; i < len ; i++) {
                    styleText += ext.styles[i].text + '\n';
                }       
                
                if(styleText.length > 0) {
                    bobj.addStyleSheet(styleText);
                }          
            }
            else {
                widget.write();
            }
        }
        return widget;
    };
    
    /**
     * Initialize a widget.  
     *
     * @param widx [Int]  Widget index
     * @param initElt [DOM Node, opt]  The element that called the init event 
     */
    bobj.crv._initWidget = function(widx, initElt) { 
        try {
            var widget = _widgets[widx];
            widget.init();
            
            if (initElt) {
                MochiKit.DOM.removeElement(initElt);
            }
        }
        catch (err) {
            if (bobj.crv.config.isDebug) {
                var msg = "bobj.crv._initWidget: Couldn't initialize widget: ";
                msg += 'widx=' + widx;
                msg += ', type=';
                if (!_widgets[widx]) {
                    msg += 'null';
                }
                else if (bobj.isString(_widgets[widx].widgetType)) {
                    msg +=  _widgets[widx].widgetType;
                }
                else
                {
                    msg += 'unknown';
                }
                msg += ' because: ' + err;
                throw msg; 
            }
        }
    };
    
    /**
     * Creates an hidden image tag that initializes a widget using its onload
     * event. The tag should be appended to the html of the widget.
     *
     * Note: doesn't work in Opera 9, but we don't support that yet
     *
     * TODO nested widgets fail in Firefox because parent's init method has to
     * be called after child's init method and the timing of the img onload
     * events looks to be unpredictable. onerror might be more predictable if
     * no src is specified... is it supported in Safari?
     *
     * @param widx [Int]  Widget index
     *
     * @return Returns html that initializes the widget with index == widx
     
    bobj.crv.getInitHTML = function(widx) {
        return bobj.html.IMG({
            style: {display:'none'},
            //onload: 'bobj.crv._initWidget(' + widx + ', this);',
            onload: "alert('loading " + widx + "')",
            src: bobj.crvUri("../dhtmllib/images/transp.gif")
        });
    }*/
    
    /**
     * Returns HTML that auto-intializes a widget.
     */
    bobj.crv.getInitHTML = function(widx) {
        var scriptId = 'bobj_crv_getInitHTML_' + widx;
        return '<script id="' + scriptId + '" language="javascript">' +
            'bobj.crv._initWidget(' + widx + ',"' + scriptId + '");' +
            '</script>';
    };
	
    bobj.crv._preloadProcessingIndicatorImages = function() { 
    	var relativeImgPath = bobj.crvUri("../dhtmllib/images/" + bobj.crv.config.skin + "/");
        var images = [];
        var imgNames = [
                "wait01.gif", 
                "dialogtitle.gif", 
                "dialogelements.gif", 
                "../transp.gif"];

        for(var i = 0, len = imgNames.length; i < len; i++){
            images[i] = new Image();
            images[i].src = relativeImgPath + imgNames[i];
        }        
    };
    
	bobj.crv._loadJavaScript = function(scriptFile) {
		if (scriptFile) {
			document.write('<script language="javascript" src="' 
	                + bobj.crvUri(scriptFile)
	                +'"></script>');
		}
	};
	
    /**
     * Include the viewer's scripts in the document. 
     */
    bobj.crv._includeAll = function() {
    
        bobj.crv._includeLocaleStrings();
        
        if (bobj.crv.config.useCompressedScripts) {
            if (bobj.crv.config.indicatorOnly) {
                bobj.crv._loadJavaScript('../../processindicator.js');
            }
            else {
                bobj.crv._loadJavaScript('../../allInOne.js');
                if(!bobj.crv.config.useAsync) {
                    for(var name in bobj.crv.config.resources) {
                        var resource = bobj.crv.config.resources[name];
                        resource.isLoaded = true;
                        bobj.crv._loadJavaScript(resource.path);
                    }
                }
                if (bobj.crv.config.logging.enabled) {
                    bobj.crv._loadJavaScript('../log4javascript/log4javascript.js');
                }
            }
            return;
        }
        else {
            // list of script uris relative to crv.js
            var scripts = [];
            if (bobj.crv.config.indicatorOnly) {
                scripts = [
                    '../MochiKit/Base.js',
                    '../dhtmllib/dom.js',
                    'initDhtmlLib.js',
                    '../dhtmllib/dialog.js',
                    '../dhtmllib/waitdialog.js',
                    'common.js',
                    'Dialogs.js'
                ];
            }
            else {
                scripts = [
                    '../MochiKit/Base.js',
                    '../MochiKit/Async.js', 
                    '../MochiKit/DOM.js',  
                    '../MochiKit/Style.js',  
                    '../MochiKit/Signal.js', 
                    '../MochiKit/New.js', 
                    '../MochiKit/Color.js',  
                    '../MochiKit/Iter.js',  
                    '../MochiKit/Visual.js',
                    '../log4javascript/log4javascript_uncompressed.js',
                    '../external/date.js',
                    '../dhtmllib/dom.js',
                    'initDhtmlLib.js',
                    '../dhtmllib/palette.js',
                    '../dhtmllib/menu.js',
                    '../dhtmllib/psheet.js',
                    '../dhtmllib/treeview.js',
                    '../dhtmllib/dialog.js',
                    '../dhtmllib/waitdialog.js',
                    '../../prompting/js/promptengine_prompts2.js',
                    '../../prompting/js/promptengine_calendar2.js',
                    '../swfobject/swfobject.js',
                    'common.js',
                    'encoding.js',
                    'html.js',
                    'ImageSprites.js',
                    'Toolbar.js',
                    'Statusbar.js',
                    'PanelNavigator.js',
                    'PanelNavigatorItem.js',
                    'PanelHeader.js',
                    'LeftPanel.js',
                    'GroupTreeNode.js',
                    'GroupTree.js',
                    'GroupTreeListener.js',
                    'ToolPanel.js',
                    'ReportPage.js',
                    'ReportView.js',
                    'ButtonList.js',
                    'ReportAlbum.js',
                    'Separator.js',
                    'Viewer.js',
                    'ViewerListener.js',
                    'StateManager.js',
                    'IOAdapters.js',
                    'ArgumentNormalizer.js',
                    'event.js',
                    'PromptPage.js',
                    'Dialogs.js',
                    'StackedTab.js',
                    'StackedPanel.js',
                    'Parameter.js',
                    'ParameterController.js',
                    'Colors.js',
                    'TextField.js',
                    'TextCombo.js',
                    'RangeField.js',
                    'ParameterValueRow.js',
                    'ParameterInfoRow.js',
                    'OptionalParameterValueRow.js',
                    'ParameterUI.js',
                    'OptionalParameterUI.js',
                    'ParameterDialog.js',
                    'ParameterPanelToolbar.js',
                    'ParameterPanel.js',
                    'bobjcallback.js',
                    'Calendar.js',
                    'WarningPopup.js',
                    '../FlexParameterBridge.js',
                    'ViewerFlexParameterAdapter.js',
                    'SignalDisposer.js'
                ];
            }
            
            if(bobj.isParentWindowTestRunner()) {
                scripts.push('../jsunit/tests/crViewerTestSuite.js');
            }
               
            for (var i = 0, len = scripts.length; i < len; i++) {
                document.write('<script language="javascript" src="' 
                    + bobj.crvUri(scripts[i]) 
                    +'"></script>');
            }
            
            for(var i in bobj.crv.config.resources){
                //all Resources are automatically loaded at initialization when useCompressedScripts is false (in debug mode)
                bobj.crv.config.resources[i].isLoaded = true;
            }
        }
    };
    
    bobj.crv.getLangCode = function () {
    	var splitChar = '_';  // default to java's locale split character	
        if (bobj.crv.config.lang.indexOf ('-') > 0) {
            splitChar = '-'; // must be .Net's locale split
        }
        var lang = bobj.crv.config.lang.split(splitChar);
        
        // TODO (Julian): Java server side now handles the chinese variants due to language pack fix
        // We'll do the same for WebForm and then the following code can be removed.
        // For Chinese locales "zh" we only support "TW" & "CN"
        // zh_HK / zh_MO / zh_MY, are all mapped to zh_TW, everything else is zh_CN
        if(lang[0].toLowerCase() == "zh") {
            if(lang.length > 1 ) {
                var region = lang[1].toUpperCase();
                if(region == "TW" || region == "HK" || region == "MO" || region == "MY") {
                    lang[1] = "TW";
                } else {
                    lang[1] = "CN";
                }
            } else {
                lang[1] = "CN";
            }
        }
	    
        // .NET webform viewers don't support variants from server side - until we fix this chop-off variant code for all other than "zh"
        if (lang.length > 1 && (!bobj.crv.config.needFallback || lang[0].toLowerCase() == "zh")) {

            lang = lang[0] + "_" + lang[1];
        }
        else {
            lang = lang[0];
        }

	return lang;
    };
    
    bobj.crv._includeLocaleStrings = function() {
        var lang = bobj.crv.getLangCode ();

        if (bobj.crv.config.needFallback) {
            bobj.crv._loadJavaScript('../../allStrings_en.js');
            if (lang == 'en') return; // DO NOT load 'en' strings below again!
        }

        bobj.crv._loadJavaScript('../../allStrings_' + lang + '.js');
    };
    
    
    if (typeof MochiKit == 'undefined') {
        MochiKit = {};
    }
    if (typeof MochiKit.__export__ == 'undefined') {
        MochiKit.__export__ = false; // don't export symbols into the global namespace
    }
    
    if (!bobj.crv.config.useAsync) bobj.crv._preloadProcessingIndicatorImages();
    bobj.crv._includeAll();
	
    bobj.crv.initLog = function(logLevel, handlerUri) {
        bobj.crv.logger = log4javascript.getLogger();        
        log4javascript.setEnabled(bobj.crv.config.logging.enabled);
        
        if (bobj.crv.config.logging.enabled) {			
            bobj.crv.logger.setLevel(logLevel);    
            var uri = handlerUri + '?ServletTask=Log';
            var ajaxAppender = new log4javascript.AjaxAppender(uri);
            bobj.crv.logger.addAppender(ajaxAppender);
			
            var oldLogFunc = bobj.crv.logger.log;
            bobj.crv.logger.log = function(level, message, exception) {
                oldLogFunc(level, bobj.crv.config.logging.id + ' ' + message, exception);
            };
			            
            bobj.crv.logger.info('Logging Initialized');
        }
    };
    
    bobj.crv.invokeActionAdapter = function (args) {
        var n = "invokeAction";
        var f = window[n];
        if (!f && window.parent){
            f = window.parent[n];
        }
        
        if (f){
            f(args.url, args.actionId, args.objectId, args.containerId, args.actionType, null);
        }
    };
}

if(typeof(Sys)!=='undefined' && typeof(Sys.Application)!== 'undefined' && typeof(Sys.Application.notifyScriptLoaded)!== 'undefined') {
    Sys.Application.notifyScriptLoaded();
}
