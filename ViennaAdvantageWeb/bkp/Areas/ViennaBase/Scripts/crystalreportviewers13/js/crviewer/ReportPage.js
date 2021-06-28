/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * ReportPage constructor
 *
 * @param kwArgs.id        [String]  DOM node id
 * @param kwArgs.bgColor   [String]  Background color of the page
 * @param kwArgs.width        [Int] Page content's width in pixels
 * @param kwArgs.height       [Int] Page content's height in pixels
 * @param kwArgs.topMargin    [Int] Top margin of report page in pixels
 * @param kwArgs.rightMargin  [Int] Right margin of report page in pixels
 * @param kwArgs.bottomMargin [Int] Bottom margin of report page in pixels
 * @param kwArgs.leftMargin   [Int] Left margin of report page in pixels
 */
 
bobj.crv.newReportPage = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        bgColor: '#FFFFFF',
        width: 720,
        height: 984,
        documentView: bobj.crv.ReportPage.DocumentView.PRINT_LAYOUT
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);    
    o.widgetType = 'ReportPage';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    o.initOld = o.init;
    o.resizeOld = o.resize;
    MochiKit.Base.update(o, bobj.crv.ReportPage);
    
    return o;
};

bobj.crv.ReportPage = {
    DocumentView : {
        WEB_LAYOUT : 'weblayout',
        PRINT_LAYOUT : 'printlayout'
    },
       
    /**
     * Disposes report page by removing its layer and stylesheet from DOM
     */
    dispose : function() {
        MochiKit.DOM.removeElement (this.layer);
    },
    
    /**
     * DO NOT REMOVE. USED BY WEB ELEMENTS
     */
    displayScrollBars : function (isDisplay) {
        this.layer.style.overflow = isDisplay ? "auto" : "hidden";
    },
    
    /**
     * DO NOT REMOVE. USED BY WEB ELEMENTS
     */
    isDisplayScrollBars : function () {
        this.layer.style.overflow == "auto";
    },
    
    
    update : function(update) {
        if (update && update.cons == "bobj.crv.newReportPage") {
            this.updateSize ( {
                width : update.args.width,
                height : update.args.height
            });
            
            this.layer.scrollLeft = 0;
            this.layer.scrollTop = 0;
            this.updateHTML (update.args.content, false);
        }
    },
    
    scrollToHighlighted : function (scrollWindow) {
        if(this._iframe) {
            var iframeDoc = _ie ? this._iframe.contentWindow.document  : this._iframe.contentDocument;
            var e = iframeDoc.getElementById("CrystalHighLighted");
            if(e) {
                var ePosition = MochiKit.Style.getElementPosition (e, null, iframeDoc);
                if(scrollWindow) {
                    var reportPagePos = MochiKit.Style.getElementPosition (this.layer);
                    window.scrollTo (reportPagePos.x + ePosition.x , reportPagePos.y + ePosition.y);
                }
                else {
                    this.layer.scrollLeft = ePosition.x;
                    this.layer.scrollTop = ePosition.y;
                }
            }
        }
    },
        
    updateHTML : function(content, useAnimation) {
        if (content) {
            if(!this._iframe) {
                this._iframe = MochiKit.DOM.createDOM('IFRAME', {id : this.id + '_iframe', width : '100%', height : '100%', frameBorder : '0',  margin : '0'})   
                this._pageNode.appendChild(this._iframe);
            }
            
            if(useAnimation)
                this._iframe.style.display = "none";
            
            var iframeDoc = _ie ? this._iframe.contentWindow.document  : this._iframe.contentDocument;
            iframeDoc.open();
            iframeDoc.write(this.getIFrameHTML (content));
            iframeDoc.close();
            
            if(useAnimation)
                bobj.displayElementWithAnimation(this._iframe);
        }
    },
    
    getIFrameHTML : function (content) {
        return  "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
        "<html>\r\n" +
            "<head>\r\n" +
                "<style> body { overflow :hidden; margin : 0px;}</style>\r\n" +
            "</head>\r\n" +
            "<body>\r\n" +
                content +
            "</body>\r\n" +
        "</html>";
    },
    
    /*
     * Updates size of report page based on update object
     * @param update [{width,height,marginLeft,marginRight,marginTop}] dimension and margins
     * of report p
     */
    updateSize : function(sizeObject) {
        
        if (sizeObject) {
            this.width = (sizeObject.width != undefined) ? sizeObject.width : this.width;
            this.height = (sizeObject.height != undefined) ? sizeObject.height : this.height;
        }

        if (this._pageNode) {
            var isBBM = bobj.isBorderBoxModel ();
            this._pageNode.style.width = (isBBM ? this.width + 2: this.width) + 'px';
            this._pageNode.style.height = (isBBM ? this.height + 2: this.height) + 'px';
        }

        if (this._shadowNode) {
            this._shadowNode.style.width = (isBBM ? this.width + 2: this.width)  + 'px';
            this._shadowNode.style.height = (isBBM ? this.height + 2: this.height) + 'px';
        }
    },
    
    getHTML : function() {
        var h = bobj.html;
        var isBBM = bobj.isBorderBoxModel ();

        var layerStyle = {
            width : '100%',
            height : '100%',
            overflow : 'auto',
            position : 'absolute'
        };

        var pageStyle = {
            position : 'relative',
            width : (isBBM ? this.width + 2: this.width)  + 'px',
            height : (isBBM ? this.height + 2: this.height) + 'px',
            'z-index' : 1,
            'border-width' : '1px',
            'border-style' : 'solid',
            'background-color' : this.bgColor,
            overflow : 'hidden',
            'text-align' : 'left'
        };
        
        var shadowStyle = {
            position : 'absolute',
            'z-index' : 0,
            display : 'none',
            width : (isBBM ? this.width + 2: this.width) + 'px',
            height : (isBBM ? this.height + 2: this.height) + 'px',
            top : '0px',
            left : '0px'
        };
        
        
        var shadowHTML = '';
        if (this.documentView.toLowerCase () == bobj.crv.ReportPage.DocumentView.PRINT_LAYOUT) {
            layerStyle['background-color'] = '#8E8E8E';
            pageStyle['border-color'] = '#000000';
            shadowStyle['background-color'] = '#737373';
            shadowHTML = h.DIV ( {
                id : this.id + '_shadow',
                'class' : 'menuShadow',
                style : shadowStyle
            })
            /* page should appear in the center for print layouts */
            layerStyle['text-align'] = 'center'; /* For page centering in IE quirks mode */
            pageStyle['margin'] = '0 auto'; /* center the page horizontally - CSS2 */
            pageStyle['top'] = "6px";
        } else {
            /* Web Layout*/
            layerStyle['background-color'] = '#FFFFFF';
            pageStyle['border-color'] = '#FFFFFF';
            /* page should appear in left for web layouts */
            pageStyle['margin'] = '0';
        }

        var html = h.DIV ( {
            id : this.id,
            style : layerStyle,
            'class' : 'insetBorder'
        }, h.DIV ( {
            id : this.id + '_page',
            style : pageStyle
        }), shadowHTML);

        return html;
    },
    
    init : function() {
        this._pageNode = getLayer (this.id + '_page');
        this._shadowNode = getLayer (this.id + '_shadow');

        this.initOld ();
        
        this.updateHTML (this.content, true);
    },
    
    updateShadowLocation : function () {
        var updateFunc = function () {
            if(this._shadowNode && this._pageNode) {
                this._shadowNode.style.display = "none"; //Must hide dropshadow as it can cause scrollbars to appear
                var pageNodPos = {x : this._pageNode.offsetLeft, y : this._pageNode.offsetTop};
                this._shadowNode.style.display = "block";
                this._shadowNode.style.top = pageNodPos.y + (bobj.isBorderBoxModel() ? 4 : 6) + "px";
                this._shadowNode.style.left = pageNodPos.x +  (bobj.isBorderBoxModel()  ? 4 : 6) + "px";            
            }
        }
        
        setTimeout(bobj.bindFunctionToObject(updateFunc,this), 0); // Must be executed after viewer has finished doLayout
    },
    
    /**
     * Resizes the outer dimensions of the widget.
     */
    resize : function (w, h) {
        bobj.setOuterSize(this.layer, w, h);
        if(_moz)
            this.css.clip = bobj.getRect(0,w,h,0);
        
        this.updateShadowLocation ();
    },
    
    /**
     * @return Returns an object with width and height properties such that there 
     * would be no scroll bars around the page if they were applied to the widget. 
     */
    getBestFitSize : function() {
        var page = this._pageNode;
        return {
            width: page.offsetWidth + 30, 
            height: page.offsetHeight + 30 
        };
    },
    
    hideFrame : function() {
        this.css.borderStyle = 'none';
        this._pageNode.style.border = '';
    }
};
