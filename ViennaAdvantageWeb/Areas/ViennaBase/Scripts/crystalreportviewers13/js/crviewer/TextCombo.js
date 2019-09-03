
/*
================================================================================
TextCombo

Internal class. Editable combo box that renders correctly in a Parameter UI
================================================================================
*/

/**
 * Constructor. TextCombo extends TextComboWidget from the dhtmllib.
 */
bobj.crv.params.newTextCombo = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    var PARAMS = bobj.crv.params;
    kwArgs = UPDATE({
        id: bobj.uniqueId(),
        width: '100%',
        maxChar: null,
        tooltip: null,
        disabled: false,
        editable: false,
        changeCB: null,
        enterCB: null,
        keyUpCB: null,
        isTextItalic: false
    }, kwArgs); 

    var o = newTextComboWidget(
        kwArgs.id,
        kwArgs.maxChar,
        kwArgs.tooltip,
        null, // width
        kwArgs.changeCB,  
        null, // check CB
        null, // beforeShowCB
        null);// formName 
        
    o.widgetType = 'TextCombo';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs); 
    o.width = kwArgs.width;
    
    // Update instance with member functions
    o.init_TextCombo = o.init;
    UPDATE(o, PARAMS.TextCombo);
    
    o._createTextField(); // Override parent's text property    
    o._createArrow(); 
    o.arrow.dy += 2; // Center the arrow
    o.arrow.disDy += 2;
    
    return o;
};

bobj.crv.params.TextCombo = {
    setTextItalic : function(isTextItalic) {
        if (this.text) {
            this.text.setTextItalic (isTextItalic);
        }
    },
    
    setForeColor : function(color) {
        if (this.text)
            this.text.setForeColor (color);
    },
    
    setTooltip : function(tooltip) {
        if (this.text) {
            this.text.setTooltip (tooltip);
        }
    },
    
    setTabDisabled : function(dis) {
        if (this.text)
            this.text.setTabDisabled (dis);

        if (this.arrow)
            bobj.disableTabbingKey (this.arrow.layer, dis);
    },
    
    setMenu : function(menu) {
        this.menu = menu;
    },
    
    init : function() {
        this.init_TextCombo ();
        this.arrowContainer = getLayer (this.id + '_arrowCtn');

        if (this.arrow) {
            this.arrow.layer.onfocus = IconWidget_realOverCB;
            this.arrow.layer.onblur = IconWidget_realOutCB;
        }

        this.text.setValue (this.cleanValue);
    },
    
    toggleMenu : function() {
        var menu = this.menu;
        menu.parIcon = this;
        var toShow = !menu.isShown();
        menu.show (toShow);
        if (toShow)
        	menu.valueSelect (this.text.getValue () + '');
    },
    
    _createArrow : function() {
        var tooltip = _openMenu.replace("{0}", this.tooltip? this.tooltip: '');
        this.arrow = newIconWidget(this.id + "arrow_",
                                    bobj.skinUri("menus.gif"),
                                    bobj.bindFunctionToObject(this.toggleMenu,this),
                                    null,
                                    tooltip,
                                    7, /* w */
                                    12, /* h */
                                    0,  /* dx */
                                    83,  /* dy */
                                    0,  /* disDx */
                                    99);  /* disDy */

        this.arrow.setClasses("iconnocheck", "combobtnhover", "combobtnhover", "combobtnhover");
        this.arrow.par = this;                           
    },
    
    _createTextField : function() {
        this.text = bobj.crv.params.newTextField ( {
            id : this.id + '_text',
            cleanValue : this.cleanValue,
            width : '100%',
            maxChar : null,
            tooltip : this.tooltip,
            disabled : false,
            editable : this.editable,
            password : false,
            focusCB : this.focusCB,
            blurCB : this.blurCB,
            keyUpCB : bobj.bindFunctionToObject (this._onKeyUp, this),
            enterCB : this.enterCB,
            foreColor : this.foreColor,
            isTextItalic : this.isTextItalic
        });
    },
    
    getHTML : function() {
        var h = bobj.html;

        var arrowClassName = 'iactTextComboArrow';
        var arrowStyle = {};
        arrowStyle.right = "0px";

        if (MochiKit.Base.isIE ()) {
            arrowStyle.height = "18px";

        } else {
            arrowStyle.height = "16px";
        }

        var html = h.DIV ( {
            id : this.id,
            style : {
                width : "100%",
                position : "relative"
            }
        }, h.DIV ( {
            style : {
                position : "relative"
            },
            'class' : 'iactTextComboTextField'
        }, this.text.getHTML ()), h.DIV ( {
            'class' : arrowClassName,
            id : this.id + '_arrowCtn',
            style : arrowStyle
        }, this.arrow.getHTML ()));

        return html;
    },
    
    /**
     * Resets TextCombo. sets original value and hides arrow icon
     */
    reset : function(value) {
        this.text.reset (value);
    },
    
    /**
     * Set the content of the text box and update the menu selection
     *
     * @param text [String]
     */
    setValue : function(text) {
        this.text.setValue(text);
    },
    
    setCleanValue : function(text) {
        this.text.setCleanValue (text);
    },
    
    /**
     * Private. Overrides parent.
     */
    selectItem : function(item) {
        if (item) {
            this.val = item.value;
            this.text.setValue (item.value, true); /* keep the previous clean value */
            this.menu.select (item.index);
        }
    },
    
    /**
     * Get the content on the text box
     *
     * @return [String]
     */
    getValue : function() {
        return this.text.getValue ();
    },
    
    _onKeyUp : function(e) {
        var text = this.text.getValue ();

        if (this.keyUpCB)
            this.keyUpCB (e);
    }
};


/**
 * ScrollMenuWidget
 */

bobj.crv.params.newScrollMenuWidget = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id : bobj.uniqueId(),
        originalValues: [],
        hasProperWidth: false,
        hasValueList: false,
        maxVisibleItems: 10,
        openAdvDialogCB: null,
        maxNumParameterDefaultValues: null
    },kwArgs);
    
    var visibleLines = (kwArgs.originalValues.length >= kwArgs.maxVisibleItems) ? kwArgs.maxVisibleItems : kwArgs.originalValues.length;
    if (visibleLines === 1) {
        visibleLines++;
    }

    var o = newScrollMenuWidget(
        "menu_"+ kwArgs.id, //id
        bobj.crv.params.ScrollMenuWidget.onChange, //changeCB
        false, //multi
        null, 
        visibleLines, //lines 
        null, //tooltip 
        null,//dblClickCB 
        null, //keyUpCB
        false, //showLabel
        '', //label 
        '',//convBlanks 
        null, //beforeShowCB
        null); //menuClickCB
      
    o.oldShow = o.show;
    MochiKit.Base.update(o,kwArgs, bobj.crv.params.ScrollMenuWidget);                      

    return o;
};

bobj.crv.params.ScrollMenuWidget = {
    onChange : function() {
        var o = this.parIcon;
        var item = this.getSelection ();

        if (item) {
            if (this.maxNumParameterDefaultValues && item.index == this.maxNumParameterDefaultValues) {
                if (this.openAdvDialogCB) {
                    this.openAdvDialogCB ();
                    this.clearSelection ();
                }
            } else {
                o.val = item.value;
                o.text.setValue (item.value);
            }
        } else {
            o.val = null;
            o.text.setValue ("");
        }

        if (o.changeCB) {
            o.changeCB ();
        }
    },
    
    getPosition : function() {
        if (this.parIcon === null) {
            return;
        }

        var layer = this.parIcon.layer;
        var getDimensions = MochiKit.Style.getElementDimensions;
        var position = getPosScrolled (layer);
        var xPos = position.x + 2;
        var yPos = position.y + getDimensions (layer).h + 3;
        if (MochiKit.Base.isIE ()) {
            xPos -= 1;
            if (bobj.isQuirksMode ()) {
                yPos -= 2;
            }
        }

        return {
            x : xPos,
            y : yPos
        };

    },
    
    setProperWidth : function() {
        if (this.hasProperWidth === false) {
            this.css.display = "block";
            this.orginalWidth = this.layer.offsetWidth;
            this.css.display = "none";
            this.hasProperWidth = true;
        }
    },
    
    setValueList : function() {
        if (this.hasValueList === false) {
            this.hasValueList = true;
            var origValues = this.originalValues;
            for ( var i = 0, len = origValues.length; i < len; i++) {
                this.add (origValues[i], origValues[i], false);
            }
        }
    },
    
    setFocus : function(focus) {
        if (focus) {
            var focusCB = bobj.bindFunctionToObject (this.list.focus, this.list);
            setTimeout (focusCB, 300);
        } else {
            if (this.parIcon.selected === true) {
                this.parIcon.arrow.focus ();
            }
        }
    },
    
    show : function(show) {
        if (this.layer === null)
            this.justInTimeInit ();

        if (this.hasValueList === false)
            this.setValueList ();

        if (this.parIcon === null)
            return;

        if (this.hasProperWidth === false)
            this.setProperWidth ();

        if (this.parIcon && this.parIcon.layer) {
            var layer = this.parIcon.layer;
            if (layer.clientWidth > this.orginalWidth) {
                this.css.width = layer.clientWidth + "px";
                this.list.css.width = layer.clientWidth + "px";
            } else {
                this.css.width = this.orginalWidth + "px";
                this.list.css.width = this.orginalWidth + "px";
            }
        }

        var pos = this.getPosition ();
        this.oldShow (show, pos.x, pos.y);

        this.setFocus (show);
    }
};
