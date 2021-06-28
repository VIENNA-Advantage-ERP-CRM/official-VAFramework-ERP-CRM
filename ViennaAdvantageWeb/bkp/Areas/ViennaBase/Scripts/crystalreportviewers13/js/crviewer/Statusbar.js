/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * Statusbar widget constructor ({id: String})
 */
bobj.crv.newStatusbar = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    kwArgs = UPDATE({
        id: bobj.uniqueId(),
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
            width           : null,
            height          : null,
            fontStyle       : null,
            fontSize        : null         
       }
   }, kwArgs);
       
    var o = newPaletteContainerWidget(kwArgs.id);

    o.margin = 0;
    bobj.fillIn(o, kwArgs);  
    o._rightZoneWgts = [];
    o.widgetType = 'Statusbar';
    
    // Attach member functions (since we can't use prototypes)
    o.initOld = o.init;
    UPDATE(o, bobj.crv.Statusbar);
    
    o.palette = newPaletteWidget(o.id + "_palette");
    o.palette.isLeftTableFixed = true;
    o.add(o.palette);
    
    return o;    
};

bobj.crv.Statusbar = {
    /**
     * Overrides parent
     */
    init : function() {
        this.initOld ();
        bobj.setVisualStyle (this.layer, this.visualStyle);
        this.palette.init ();
    },
    
    /**
     * Overrides parent
     */
    beginHTML : function() {
        return bobj.html.openTag ('div', {
            id : this.id,
            'class' : 'dialogzone',
            style : {
                width : '100%',
                overflow : 'hidden',
                margin : this.margin + 'px',
                padding : '2px 0px',
                position : 'absolute'
            }
        });
    },
    
    /**
     * Overrides parent
     */
    getHTML : function() {
        this._addRightZone ();
        return (this.beginHTML () + this.palette.getHTML () + this.endHTML ());
    },
    
    /**
     * Private. Adds right-aligned widgets to the right zone of the palette 
     */
    _addRightZone : function() {
        this.palette.beginRightZone ();

        var w = null;
        while (w = this._rightZoneWgts.pop ()) {
            this.palette.add (w);
        }

        delete this._rightZoneWgts;
    },
    
    /**
     * Overrides parent
     */
    write : function() {
        this._addRightZone ();
        this.begin ();
        this.palette.write ();
        this.end ();
        document.write (bobj.crv.getInitHTML (this.widx));
    },
    
    /**
     * Add child widgets to the statusbar - left or right
     */
    addChild : function(widget) {
        switch (widget.widgetType) {
            case 'StatusbarBreadcrumb':
                this.breadcrumb = widget;
                break;
            case 'StatusbarVersionIndicator':
                this.versionIndicator = widget;
                break;
        }

        /* Delay adding right-aligned widgets due to the semantics of the palette */
        if (widget.layoutAlign == 'right') {
            this._rightZoneWgts.push (widget);
        } else {
            this.palette.add (widget);
        }
    },
    
    /**
     * Update child widgets - if they are exists in the statusbar
     */
    update : function(update) {
        if (update) {
            for ( var childNum in update.children) {
                var child = update.children[childNum];
                if (child) {
                    switch (child.cons) {
                        case "bobj.crv.newStatusbarBreadcrumb":
                            if (this.breadcrumb) {
                                this.breadcrumb.update (child.args);
                            }
                            break;
                        case "bobj.crv.newStatusbarVersionIndicator":
                            if (this.versionIndicator) {
                                this.versionIndicator.update (child.args);
                            }
                            break;
                    }
                }
            }
        }
    },
    
    doLayout : function() {
        if (this.breadcrumb) {
            this.breadcrumb._doLayout();
        }
    }
};

/**
 * StatusbarBreadcrumb widget constructor ({values: String[]})  
 */
bobj.crv.newStatusbarBreadcrumb = function(kwArgs) {
	var o = newWidget(bobj.uniqueId());

    o.widgetType = 'StatusbarBreadcrumb'; 
    o.values = kwArgs.values;
    o.layoutAlign = 'left';
    
    o._separatorImage = img(bobj.crvUri('images/breadcrumbSep.gif'), 14, 9); 

    MochiKit.Base.update(o, bobj.crv.StatusbarBreadcrumb);
    return o;
};

bobj.crv.StatusbarBreadcrumb = {
   /**
     * Update the breadcrumbs ({values: String[]})
     */
    update : function(kwArgs) {
        this.values = kwArgs.values;
        this.layer.innerHTML = this._render ();
    },
    
    /**
     * Overrides parent
     */
    getHTML : function() {
        return bobj.html.DIV ( {
            'class' : 'statusbar_breadcrumb'
        }, bobj.html.DIV ( {
            id : this.id
        }, this._render ()));
    },
    
    /**
     * Render the breadcrumbs as table cells separated by an image - called by getHTML and update
     */
    _render : function() {
        var html = '';
        if (this.values && this.values.length > 0) {
            var cells = '';
            for (i = 0; i < this.values.length; i++) {
                if (i > 0) { /* insert a separate image before values - except first one */
                    cells += bobj.html.TD (null, this._separatorImage);
                }
                cells += bobj.html.TD ( {
                    style : {
                        'white-space' : 'nowrap'
                    }
                }, this.values[i]);
            }

            html = bobj.html.TABLE ( {
                'class' : 'iconText',
                cellspacing : '0',
                cellpadding : '0'
            }, bobj.html.TR (null, cells));
        }
        return html;
    },
    
    _doLayout : function() {
        var needsRightAlign = (this.layer.parentNode.scrollWidth > this.layer.parentNode.offsetWidth) || (this.layer.offsetLeft < 0);
        if (needsRightAlign) {
            this.layer.style.position = "absolute";
            this.layer.style.top = "0px";
            this.layer.style.right = "0px";
        }
        else {
            this.layer.style.position = "";
        }
    }
};


/**
 * StatusbarVersionIndicator widget constructor ({value: String})
 */
bobj.crv.newStatusbarVersionIndicator = function(kwArgs) {
    var text = (kwArgs && kwArgs.value) ? L_bobj_crv_LastRefreshed + ": " + kwArgs.value : ' ';
    var o = NewLabelWidget(bobj.uniqueId(), text, true);    
    
    o.widgetType = 'StatusbarVersionIndicator';
    o.layoutAlign = 'right';    
    
    MochiKit.Base.update(o, bobj.crv.StatusbarVersionIndicator);    
    return o; 
};

bobj.crv.StatusbarVersionIndicator = {
    /**
     * Update the last refreshed value
     */
    update : function(kwArgs) {
        var text = (kwArgs && kwArgs.value) ? L_bobj_crv_LastRefreshed + ":&nbsp;" + kwArgs.value : '&nbsp;';
        this.layer.innerHTML = text;
    }
};