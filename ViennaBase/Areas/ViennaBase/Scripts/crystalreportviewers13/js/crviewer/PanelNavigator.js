/**
 * Navigator on left of viewer, controlling which panel is shown
 */
bobj.crv.PanelNavigator = function() {
    this._children = [];
    this.widgetType = "PanelNavigator";
    this.id = bobj.uniqueId () + "_panelNav";
};

bobj.crv.PanelNavigator.prototype = {
    getHTML : function() {
        var childrenHTML = "";
        for ( var i = 0; i < this._children.length; i++)
            childrenHTML += this._children[i].getHTML ();

        var DIV = bobj.html.DIV;
        var style = { width : bobj.isBorderBoxModel() ? '37px' : '35px'};


        return DIV ( {
            'class' : 'panelNavigator',
            id : this.id,
            style : style
        }, DIV ( { id : this.id + "_innerBorder",
            'class' : 'panelNavigatorInnerBorder'
        }, childrenHTML));
    },

    /**
     * Initializes its layer and calls init on children
     * @return
     */
    init : function() {
        this.layer = getLayer (this.id);
        this._innerBorder = getLayer(this.id + "_innerBorder");
        this.css = this.layer.style;
        
        if(this._children.length == 0) {
            this.css.display = "none";
        }
        else {
            for ( var i = 0; i < this._children.length; i++)
                this._children[i].init ();   
        }
    },

    /**
     * Sets selection on specified child and removes selection from any other child
     * @return
     */
    selectChild : function(childName) {
        for ( var i = 0; i < this._children.length; i++) {
            var child = this._children[i];
            child.setSelected (child.getName () == childName);
        }
    },
    
    getChild : function (childName) {
        for ( var i = 0; i < this._children.length; i++) {
            var child = this._children[i];
            if(child.getName () == childName)
                return child;
        }
        return null;
    },
    
    hasChildren : function () {
       return (this._children.length > 0);
    },
    
    /**
     * Do not Remove, Used by WebElements Public API
     */
    getGroupTreeButton : function () {
        return this.getChild (bobj.crv.ToolPanelType.GroupTree);
    },
    
    /**
     * Do not Remove, Used by WebElements Public API
     */
    getParamPanelButton : function () {
        return this.getChild (bobj.crv.ToolPanelType.ParameterPanel);
    },
    
    
    /**
     * 
     * @param kwArgs [JSON] creates PanelNavigatorItem with properties specified in kwArgs and connects signals
     * @return
     */
    addChild : function(kwArgs) {
        kwArgs = MochiKit.Base.update ( {
            name : '',
            title : '',
            img : { uri: '', dx: 0, dy: 0 }
        }, kwArgs);

        var partial = MochiKit.Base.partial;
        var signal = MochiKit.Signal.signal;
        var connect = MochiKit.Signal.connect;

        var navItem = new bobj.crv.PanelNavigatorItem (kwArgs.name, kwArgs.img, kwArgs.title, 35 * this._children.length);

        connect (navItem, "switchPanel", partial (signal, this, "switchPanel"));
        this._children.push (navItem);
    },

    resize : function(w, h) {
        bobj.setOuterSize (this.layer, w, h);
        bobj.setOuterSize (this._innerBorder, w -2, h -2); //Since border size is 1px for all edges
    },

    getBestFitHeight : function () {
        var height = 0;
        for ( var i = 0; i < this._children.length; i++)
            height += this._children[i].getHeight();   
        
        return height;
    },
    
    move : Widget_move,
    getWidth : Widget_getWidth

};
