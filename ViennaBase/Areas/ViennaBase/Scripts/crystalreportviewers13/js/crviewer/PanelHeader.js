/**
 * The header that appears above toolPanel. It consists of a title and close button.
 */
bobj.crv.PanelHeader = function() {
    this.id = bobj.uniqueId () + "_panelHeader";

    this._closeButton = newIconWidget (this.id + "_close", bobj.crv.allInOne.uri, bobj
            .bindFunctionToObject (this._closeButtonOnClick, this), null, L_bobj_crv_Close, 8, 7, 0, bobj.crv.allInOne.closePanelDy,null,null,true);

    this.normalCssClass = "panelHeaderCloseButton";
    this.highlightedCssClass = "panelHeaderCloseButtonHighlighted";
    
    this._closeButton.setClasses (this.normalCssClass, this.normalCssClass, this.highlightedCssClass, this.highlightedCssClass);
    this._title = "";
};

bobj.crv.PanelHeader.prototype = {
    getHTML : function() {
        var DIV = bobj.html.DIV;
        var style = {height : bobj.isBorderBoxModel() ? '21px' : '20px'};

        return DIV ( {
            'class' : 'panelHeader',
            id : this.id,
            style : style
        }, DIV ( {
            'class' : 'panelHeaderTitle',
            id : this.id + "_title"
        }, this._title), DIV ( {
            'class' : 'panelHeaderButtonCtn'
        }, this._closeButton.getHTML ()));
    },

    init : function() {
        this.layer = getLayer (this.id);
        this.css = this.layer.style;
        this._closeButton.init ();

        var cbLayer = this._closeButton.layer
        if (cbLayer) {
            MochiKit.Signal.connect (cbLayer, "onfocus", this, this._closeButtonOnFocus);
            MochiKit.Signal.connect (cbLayer, "onblur", this, this._closeButtonOnBlur);
        }
    },

    /**
     * 
     * @return [DOM element] of title
     */
    _getTitleLayer : function() {
        return getLayer (this.id + "_title");
    },

    /**
     * Sets title on panel header
     * @param title
     * @return
     */
    setTitle : function(title) {
        this._title = title;
        this._closeButton.changeTooltip(L_bobj_crv_Close + " " + title);
        var l = this._getTitleLayer ();
        if (l)
            l.innerHTML = title;
    },

    _closeButtonOnFocus : function() {
        if (this._closeButton && this._closeButton.layer)
            MochiKit.DOM.addElementClass (this._closeButton.layer, this.highlightedCssClass);
    },

    _closeButtonOnBlur : function() {
        if (this._closeButton && this._closeButton.layer)
            MochiKit.DOM.removeElementClass (this._closeButton.layer, this.highlightedCssClass);
    },

    _closeButtonOnClick : function() {
        MochiKit.Signal.signal (this, 'switchPanel', bobj.crv.ToolPanelType.None);
    },

    resize : function(w, h) {
        if (this.layer)
            bobj.setOuterSize (this.layer, w, h);
        
        var titleLayer = this._getTitleLayer ();
        if(titleLayer)
            bobj.setOuterSize (titleLayer, w - 30);
    },

    hideCloseButton : function() {
        if (this._closeButton)
            this._closeButton.setDisplay(false);
    },

    getWidth : Widget_getWidth,
    getHeight : Widget_getHeight,
    move : Widget_move,
    setDisplay : Widget_setDisplay
};
