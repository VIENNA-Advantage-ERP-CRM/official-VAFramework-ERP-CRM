/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * PromptPage constructor
 *
 * @param kwArgs.id        [String]  DOM node id
 * @param kwArgs.contentId [String]  DOM node id of report page content container
 * @param kwArgs.bgColor   [String]  Background color of the page
 * @param kwArgs.width        [Int] Page content's width in pixels
 * @param kwArgs.height       [Int] Page content's height in pixels
 * @param kwArgs.topMargin    [Int] Top margin of report page in pixels
 * @param kwArgs.rightMargin  [Int] Right margin of report page in pixels
 * @param kwArgs.bottomMargin [Int] Bottom margin of report page in pixels
 * @param kwArgs.leftMargin   [Int] Left margin of report page in pixels
 */
bobj.crv.newPromptPage = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        layoutType: 'fixed',
        content : null,
        width: 800,
        height: 600,
        padding: 5,
        top: 0,
        left: 0
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);    
    o.widgetType = 'PromptPage';
    o._reportProcessing = null;
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    o.initOld = o.init;
    MochiKit.Base.update(o, bobj.crv.PromptPage);
    
    window[o.id] = o;
    
    return o;
};

bobj.crv.PromptPage = {
    /**
     * Overrides parent. Sets the content of the report page.
     * 
     * @param content
     *            [String|DOM Node] Html or Node to use as report page content
     */   
    setHTML : function(content) {
        var pageNode = this._pageNode;
        if (bobj.isString (content)) {
            var ext = bobj.html.extractHtml(content);
            pageNode.innerHTML = ext.html;
            
            var links = ext.links;
            for(var iLinks = 0, linksLen = links.length; iLinks < linksLen; ++iLinks) {
                bobj.includeLink(links[iLinks]);
            }
                    
            var scripts = ext.scripts;
            for (var iScripts = 0, scriptsLen = scripts.length; iScripts < scriptsLen; ++iScripts) {
                var script = scripts[iScripts];
                if (!script) {continue;}
                
                if (script.text) {
                    bobj.evalInWindow(script.text);
                }
            }
        } else if (bobj.isObject (content)) {
            pageNode.innerHTML = '';
            pageNode.appendChild (content);
            var contentStyle = content.style;
            contentStyle.display = 'block';
            contentStyle.visibility = 'visible';
        }
    },
    
    getHTML : function() {
        var h = bobj.html;
        var isBorderBoxModel = bobj.isBorderBoxModel ();

        var pageOuterHeight = this.height + this.topMargin + this.bottomMargin;
        var pageOuterWidth = this.width + this.leftMargin + this.rightMargin;

        var contentHeight = isBorderBoxModel ? pageOuterHeight : this.height;
        var contentWidth = isBorderBoxModel ? pageOuterWidth : this.width;

        var layerStyle = {
            position : 'relative',
            width : contentWidth + 'px',
            height : contentHeight + 'px',
            top : this.top + "px",
            left : this.left + "px",
            border : 'none',
            'z-index' : 1,
            'background-color' : this.bgColor
        };

        /* To support the width and height */
        if (this.layoutType == 'fixed') {
            layerStyle.overflow = 'auto';
        }

        var pageStyle = {
            'padding' : this.padding + 'px'
        };

        var html = h.DIV ( {
            id : this.id,
            style : layerStyle
        }, h.DIV ( {
            id : this.id + '_page',
            style : pageStyle
        }));

        return html;
    },
    
    init : function() {
        this._pageNode = document.getElementById (this.id + '_page');

        this.initOld ();

        if (this.contentId) {
            var content = document.getElementById (this.contentId);
            if (content) {
                this.setHTML (content);
            }
        }
        else if(this.content) {
            this.setHTML(this.content);
            delete this.content;
        }

        var connect = MochiKit.Signal.connect;

        if (this.layoutType.toLowerCase () == 'client') {
            connect (window, 'onresize', this, '_doLayout');
        }

        this._doLayout ();
    },
    
    /* TODO: fix the layout (fitreport) to behave like XIR2 */
    _doLayout : function() {
        var layout = this.layoutType.toLowerCase ();

        if ('client' == layout) {
            this.css.width = '100%';
            this.css.height = '100%';
        } else if ('fitreport' == layout) {
            this.css.width = '100%';
            this.css.height = '100%';
        } else { /* fixed layout  */
            if(this.width != null && this.width.length > 0) {
                if(this.width.indexOf("px") > 0 || this.width.indexOf("%") > 0)
                    this.css.width = this.width;
                else
                    this.css.width = this.width + 'px';
            }
            if(this.height != null && this.height.length > 0) {
                if(this.height.indexOf("px") > 0 || this.height.indexOf("%") > 0)
                    this.css.height = this.height;
                else
                    this.css.height = this.height + 'px';
            }
        }

        var rptProcessing = this._reportProcessing;
        if (rptProcessing && rptProcessing.layer) {
            rptProcessing.center ();
        }
    },
    
    addChild : function(widget) {
        if (widget.widgetType == 'ReportProcessingUI') {
            this._reportProcessing = widget;
        }
    }
};


/**
 * FlexPromptPage constructor
 *
 * @param kwArgs.id        [String]  DOM node id
 * @param kwArgs.contentId [String]  DOM node id of report page content container
 * @param kwArgs.bgColor   [String]  Background color of the page
 * @param kwArgs.width        [Int] Page content's width in pixels
 * @param kwArgs.height       [Int] Page content's height in pixels
 * @param kwArgs.topMargin    [Int] Top margin of report page in pixels
 * @param kwArgs.rightMargin  [Int] Right margin of report page in pixels
 * @param kwArgs.bottomMargin [Int] Bottom margin of report page in pixels
 * @param kwArgs.leftMargin   [Int] Left margin of report page in pixels
 */
bobj.crv.newFlexPromptPage = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        layoutType: 'fixed',
        width: 800,
        height: 600,
        padding: 5,
        top: 0,
        left: 0
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);    
    o.widgetType = 'FlexPromptPage';
    o._reportProcessing = null;
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    o.initOld = o.init;
    MochiKit.Base.update(o, bobj.crv.FlexPromptPage);
    window[o.id] = o;
    
    return o;
};

bobj.crv.FlexPromptPage = {
    /**
     * Overrides parent. Does nothing because the swf content will replace the DIV later
     *
     * @param content [String|DOM Node]  Html or Node to use as report page content 
     */
    setHTML : MochiKit.Base.noop,
    
    getHTML : function() {
        var isBorderBoxModel = bobj.isBorderBoxModel ();

        var pageOuterHeight = this.height + this.topMargin + this.bottomMargin;
        var pageOuterWidth = this.width + this.leftMargin + this.rightMargin;

        var contentHeight = isBorderBoxModel ? pageOuterHeight : this.height;
        var contentWidth = isBorderBoxModel ? pageOuterWidth : this.width;

        var useSize = this.layoutType.toLowerCase () == bobj.crv.Viewer.LayoutTypes.FIXED;

        var layerStyle = {
            position : 'relative',
            width : useSize ? contentWidth + 'px' : '100%',
            height : useSize ? contentHeight + 'px' : '100%',
            top : this.top + "px",
            left : this.left + "px",
            border : 'none',
            'z-index' : 1,
            'background-color' : this.bgColor
        };

        var pageStyle = {
            'padding': this.padding + 'px',
            position: 'absolute' // Must be absolute to avoid double initialization in firefox ADAPT01260507
        };

        bobj.crv.params.ViewerFlexParameterAdapter.setViewerLayoutType (this.id, this.layoutType);

        var h = bobj.html;
        return h.DIV ( {
            id : this.id,
            style : layerStyle
        }, h.DIV ( {
            id : this.id + '_page',
            style : pageStyle
        }, h.DIV ( {
            id : this.contentId
        })));
    },
    
    init : function() {
        var connect = MochiKit.Signal.connect;
        if (this.layoutType.toLowerCase () == 'client') {
            connect (window, 'onresize', this, '_doLayout');
        }

        this._doLayout ();
    },
    
    _doLayout : function() {
        var rptProcessing = this._reportProcessing;
        if (rptProcessing && rptProcessing.layer) {
            rptProcessing.center ();
        }
    },
    
    addChild : function(widget) {
        if (widget.widgetType == 'ReportProcessingUI') {
            this._reportProcessing = widget;
        }
    }
        
};
