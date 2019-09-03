/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * ReportView Constructor
 */
bobj.crv.newReportView = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        viewStateId: null,
        isMainReport: false
    }, kwArgs);
    var o = newWidget(kwArgs.id);

    bobj.fillIn(o, kwArgs);  
    o.widgetType = 'ReportView';
    
    o.reportPage = null;
    
    o._lastPanelWidth = null;
    
    // Attach member functions 
    o.initOld = o.init;
    o.isMainReportFlag = o.isMainReport;
    MochiKit.Base.update(o, bobj.crv.ReportView);
    
    return o;    
};

bobj.crv.ReportView = {
    init : function() {
        this.initOld ();
        if (this.reportPage)
            this.reportPage.init ();
    },
    
    addChild : function(widget) {
        if (widget.widgetType == 'ReportPage')
            this.reportPage = widget;
    },
    
    /**
     * This method should be called after viewer has initialized. Adds a child to view by first appending its html
     * to view and then intializing it.
     */
    delayedAddChild : function(widget) {
        this.addChild (widget);
        append2 (this.layer, widget.getHTML ());
        widget.init ();

    },
    
    scrollToHighlighted : function (scrollWindow) {
        if(this.reportPage) {
            this.reportPage.scrollToHighlighted(scrollWindow);
        }
    },
    
    update : function(update) {
        if (update && update.cons == "bobj.crv.newReportView") {
            if(update.args)
                this.viewStateId = update.args.viewStateId;
            
            for ( var childVar in update.children) {
                var child = update.children[childVar];
                if (child && child.cons == "bobj.crv.newReportPage") {
                    if (!this.reportPage) {
                        /* adds reportPage if not existing */
                        this.delayedAddChild (bobj.crv.createWidget (child));
                    } else {
                        /* updates reportpage */
                        this.reportPage.update (child);
                    }

                    break; /* There is only one child */
                }
            }
        }
    },
    
    getHTML : function() {
        var h = bobj.html;

        var layerStyle = {
            width : '100%',
            height : '100%',
            overflow : 'hidden',
            position : 'relative'
        };

        var html = h.DIV ( {
            id : this.id,
            style : layerStyle
        }, this.reportPage ? this.reportPage.getHTML () : '');

        return html;
    },
    
    _doLayout : function() {
        if (this.reportPage)
            this.reportPage.resize (this.getWidth (), this.getHeight ());
    },
    
    isMainReport : function() {
        return this.isMainReportFlag;
    },
    
    /**
     * ReportView will always fill its container but it should be told when to 
     * resize so that the layout of its contents will be updated.
     */
    resize : function() {
        this._doLayout ();
    },
    
    dispose : function() {
        if (this.reportPage) {
            this.reportPage.dispose ();
            bobj.deleteWidget (this.reportPage);
            delete this.reportPage;
        }

        bobj.removeAllChildElements (this.layer);
    },
    
    /**
     * @return Returns a suggested size for the widget as an object with width and height integer properties that specify the dimensions in
     *         pixels.
     */
    getBestFitSize : function() {
        var w = 0;
        var h = 0;

        var pageSize = this.reportPage ? this.reportPage.getBestFitSize () : null;
        if (pageSize) {
            w += pageSize.width;
            h += pageSize.height;
        }

        return {
            width : w,
            height : h
        };
    },

    /**
     * @return True if the view has report content. False if the view is empty.
     */
    hasContent : function() {
        return this.reportPage != null;
    },
    
    hideFrame : function() {
        if (this.reportPage)
            this.reportPage.hideFrame();
    }
};
