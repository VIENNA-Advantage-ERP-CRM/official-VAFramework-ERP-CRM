
//** ButtonList Widget *************************************************************

/**
 * ButtonList Constructor
 */
bobj.crv.newButtonList = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        numLines: null, // null allows the height to grow (will fit within the viewport)
        buttonWidth: 24,
        buttonTooltip: L_bobj_crv_TabList,
        changeCB: null,
        label: null,
        tabIndex: 0,
        multiSelect: false,
        menuWidth: null,
        menuTooltip: null 
    }, kwArgs);
    
    var o = newButtonWidget(
        kwArgs.id,
        kwArgs.label,
        bobj.crv.ButtonList._onClick, 
        kwArgs.buttonWidth,
        null,
        kwArgs.buttonTooltip,
        kwArgs.tabIndex,
        0, _skin+"menus.gif", 7, 16, 0, 81, true, 0, 97);
    
    o._menu = newListWidget(
        kwArgs.id + "_menu",
        MochiKit.Base.bind(bobj.crv.ButtonList._onChange, o),
        kwArgs.multiSelect,
        kwArgs.menuWidth,
        kwArgs.numLines || 2, 
        kwArgs.menuTooltip,
        null,  //dblClickCB
        null); //keyUpCB
        
    o._listItems = [];
    o._blOldInit = o.init;
    o._blOldGetHTML = o.getHTML;
    o._menuDiv = null;
    
    o._captureClicks = MenuWidget_captureClicks;
    o._releaseClicks = MenuWidget_releaseClicks;
    
    bobj.fillIn(o, kwArgs);  
    o.widgetType = 'ButtonList';
    MochiKit.Base.update(o, bobj.crv.ButtonList);
    
    return o;
};

bobj.crv.ButtonList = {
    /**
     * @return [Widget] Menu/list widget associated with this button.
     */
    getMenu : function() {
        return this._menu;
    },
    
    /**
     * Add an item to the menu
     *
     * @param label [String]           The text to display in the menu
     * @param value [any - opt.]       The value associated with the new menu item
     * @param isSelected [bool - opt.] True if item should be selected after being added
     * @param id    [String - opt.]    DHTML id associated with menu item;  
     */
    add : function(label, value, isSelected, id) {
        if (this._menu && this._menu.layer)
            this._menu.add (label, value, isSelected, id);

        else
            this._listItems.push ( {
                lbl : label,
                val : value,
                sel : isSelected,
                id : id
            });
    },
    
    init : function() {
        var menu = this._menu;
        this._blOldInit ();
        menu.init ();

        this._menuDiv = getLayer (this.id + '_menuDiv');

        var listItems = this._listItems;
        for ( var i = 0, len = listItems.length; i < len; ++i) {
            var it = listItems[i];
            menu.add (it.lbl, it.val, it.sel, it.id);
        }
        this._listItems = [];
    },
    
    getHTML : function() {
        var h = bobj.html;

        var menuDivAtts = {
            id : this.id + '_menuDiv',
            onmousedown : 'event.cancelBubble=true',
            'class' : 'menuFrame',
            style : {
                visibility : 'hidden',
                position : 'absolute',
                'z-index' : 5000
            }
        };
        return this._blOldGetHTML () + h.DIV (menuDivAtts, this._menu.getHTML ());
    },
    
    /**
     * @return [bool] True if and only if the menu is visible
     */
    isMenuShowing : function() {
        return this._menuDiv && this._menuDiv.style.visibility != 'hidden';
    },

    /**
     * Hide the menu
     */
    hideMenu : function() {
        if (this._menuDiv) {
            this._menuDiv.style.visibility = 'hidden';
        }
    },
    
    /**
     * Position and show the menu
     */
    showMenu : function() {
        if (this._menuDiv) {
            this._captureClicks ();

            var body = document.body;

            if (this._menuDiv.parentNode !== body) {
                body.appendChild (this._menuDiv);
            }

            var divStyle = this._menuDiv.style;
            divStyle.left = '-1000px';
            divStyle.top = '-1000px';
            divStyle.visibility = 'visible';

            var winDim = MochiKit.Style.getViewportDimensions ();

            var w = this._menu.layer.offsetWidth;
            var h = this._menu.getHeight ();

            /*
             * If numLines wasn't specified, use as much space as necessary while remaining within the viewport
             */
            if (!this.numLines) {
                h = Math.min (this._menu.layer.scrollHeight + 10, winDim.h - 10);
                this._menu.resize (null, h);
            }

            /* Place the menu below the button and aligned with the left edge */
            var btnPos = getPosScrolled (this.layer);
            var x = btnPos.x;
            var y = btnPos.y + this.getHeight ();

            /* Change coordinates so the whole menu is on the screen */
            var xRight = x + w + 4;
            var yBottom = y + h + 4;

            var xMax = winDim.w + body.scrollLeft - Math.max (0, (winDim.w - body.offsetWidth));
            if (xRight > xMax) {
                x = Math.max (0, x - (xRight - xMax));
            }

            var yMax = winDim.h + body.scrollTop;
            if (yBottom > yMax) {
                y = Math.max (0, y - (yBottom - yMax));
            }

            divStyle.left = x + 'px';
            divStyle.top = y + 'px';
        }
    },
    
    /**
     * Private. Capture clicks in the current document so that the menu can be hidden automatically.
     */
    _captureClicks : function() {
        var BIND = MochiKit.Base.bind;
        try {
            this.layer.onmousedown = BIND (this._onCaptureClick, this, true);
            this._oldMousedown = document.onmousedown;
            document.onmousedown = BIND (this._onCaptureClick, this, false);
        } catch (ex) {
            if (bobj.crv.config.isDebug) {
                throw ex;
            }
        }
    },
    
    /**
     * Private. Stop capturing clicks.
     */
    _releaseClicks : function() {
        if (this.layer.onmousedown) { /* non-null if clicks are being captured */
            this.layer.onmousedown = null;
            document.onmousedown = this._oldMousedown;
        }
    },
    

    /**
     * Private. Button click callback.
     */
    _onClick : function() {
        if (!this._cancelNextClick) 
            this.showMenu ();
        
        this._cancelNextClick = false;
    },
    
    /**
     * Private. Menu change callback.
     */
    _onChange : function() {
        this._releaseClicks ();
        this.hideMenu ();

        if (this.changeCB) 
            this.changeCB ();
    },
    
    /**
     * Private. Called when a click is captured (after _captureClicks has been called)
     */
    _onCaptureClick : function(cancelNext, e) {
        this._cancelNextClick = cancelNext;
        eventCancelBubble (e);
        this.hideMenu ();
        this._releaseClicks ();
    }
};
