/**
 * 
 * @param name [String] Name of itme
 * @param img [String] uri location of icon image
 * @param title [String] tooltip 
 */
bobj.crv.PanelNavigatorItem = function(name, img, title, topOffset) {
    this._name = name;
    this._img = img;
    this._isSelected = false;
    this._title = title;
    this.topOffset = topOffset;
    this.widgetType = 'PanelNavigatorItem';
    
    this.id = bobj.uniqueId () + "_navItem_" + name;
};

bobj.crv.PanelNavigatorItem.prototype = {
    getHTML : function() {
        var h = bobj.html;
        var img = this._img;

        return h.DIV ( {
            id : this.id,
            'class' : 'panelNavigatorItem',
            'tabindex' : '0',
            style : {top : this.topOffset + "px"},
            title : this._title,
            role : 'button'
        }, imgOffset(img.uri, 22, 22, img.dx, img.dy, null, 'class="panelNavigatorItemImage" title="' + this._title + '"')
        );
    },

    getName : function() {
        return this._name;
    },

    init : function() {
        this.layer = getLayer (this.id);
        this.css = this.layer.style;

        var connect = MochiKit.Signal.connect;
        connect (this.layer, "onclick", this, this._onClick);
        connect (this.layer, "onkeydown", this, this._onKeyDown);
        connect (this.layer, "onmouseover", this, this._onMouseOver);
        connect (this.layer, "onmouseout", this, this._onMouseOut);
        connect (this.layer, "onfocus", this, this._onFocus);
        connect (this.layer, "onblur", this, this._onBlur);
        
        this.setSelected (this._isSelected);
    },
    
    _onFocus : function () {
        MochiKit.DOM.addElementClass (this.layer, "highlighted");
    },
    
    _onBlur : function () {
        MochiKit.DOM.removeElementClass (this.layer, "highlighted");
    },
    
    _onMouseOver : function () {
        MochiKit.DOM.addElementClass (this.layer, "highlighted");
    },
    
    _onMouseOut : function () {
        MochiKit.DOM.removeElementClass (this.layer, "highlighted");
    },

    _onKeyDown : function(ev) {
        if (ev && ev.key () && (ev.key ().code == 13 || ev.key ().code == 32)) // On Enter or spacebar, signal swith panel
            this._signalSwitchPanel();
    },

    _onClick : function() {
        this._signalSwitchPanel ();
    },
    
    /**
     * Signals switch panel if it's not currently selected
     * @return
     */
    _signalSwitchPanel : function () {
        if(!this._isSelected)
            MochiKit.Signal.signal (this, "switchPanel", this._name);
    },

    /**
     * @param isSelected [Boolean] is item selected or not selected
     */
    setSelected : function(isSelected) {
        this._isSelected = isSelected;
        var DOM = MochiKit.DOM;
        if (this.layer) {
            if (isSelected)
                DOM.addElementClass (this.layer, "selected");
            else
                DOM.removeElementClass (this.layer, "selected");
        }
    },

    getWidth : Widget_getWidth,
    getHeight: Widget_getHeight,
    setDisplay : Widget_setDisplay,
    isDisplayed : Widget_isDisplayed
};
