/* Copyright (c) Business Objects 2006. All rights reserved. */

/*
================================================================================
ParameterPanel
================================================================================
*/

/**
 * Constructor
 */
bobj.crv.params.newParameterPanel = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId() + '_IPPanel'
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);
    o.widgetType = 'ParameterPanel';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.params.ParameterPanel);
    
    o._tabPanel = bobj.crv.newStackedPanel({
        id: o.id + '_ParamtersStack'
    });

    //Layer that appears on top of panel when it is disabled; necessary for preventing user to select parameters/widgets
    o._overlayLayer = new bobj.crv.params.ParameterPanel.OverlayLayer(o.id); 
    o._toolbar = bobj.crv.params.newParameterPanelToolbar({
        id: o.id + '_IPToolbar'
    });
    
    return o;
};

bobj.crv.params.ParameterPanel = {
    setToolbarCallBacks : function(applyClickCB, resetClickCB) {
        if(this._toolbar) {
            this._toolbar.applyClickCB = applyClickCB;
            this._toolbar.resetClickCB = resetClickCB;
        }
    },
    
    /**
     * Disables panel by showing modal layer on top of panel, reducing opacity of panel, and disabling tabbing between widgets
     */
    setDisabled : function(dis) {
        this._overlayLayer.setVisible (dis);
        this.setTabDisabled (dis);
    },
    
    /**
     * Disables tabbing for toolbar and all parameters within panel
     */
    setTabDisabled : function(dis) {
        this._toolbar.setTabDisabled (dis);
        this._tabPanel.setTabDisabled (dis);
    },
    
    init : function() {
        Widget_init.call (this);
        this._toolbar.init ();
        if (this._tabPanel) {
            this._tabPanel.init ();
        }
        
        MochiKit.Signal.signal (this, "resetParamPanel");
    },
    
    update : function (update) {
        if (update && update.cons == "bobj.crv.params.newParameterPanel") {
            if(update.args && update.args.isResetParamPanel) 
                MochiKit.Signal.signal (this, "resetParamPanel");
        }
    },
    
    getHTML : function() {
        var DIV = bobj.html.DIV;
        var layerStyle = {
            overflow : 'hidden',
            width : this.width ? bobj.unitValue(this.width) : 'auto',
            height : this.height ? bobj.unitValue(this.height) : 'auto'
        }

        var innerHTML = this._toolbar.getHTML ();
        if (this._tabPanel) {
            innerHTML += this._tabPanel.getHTML ();
        }

        return DIV ( {
            id : this.id,
            style : layerStyle
        }, innerHTML);
    },
    
    getBestFitHeight : function () {
        var height = 0;
        if(this._tabPanel) {
            /**
             * Since container of parampanel could be invisible, getHiddenElementDimensions has to be called
             * instead of element.getHeight()
             */
            height += bobj.getHiddenElementDimensions(this._tabPanel.layer).h;
        }
        
        if(this._toolbar)
            height += this._toolbar.getHeight ();
        
        return height;
    },
    
    /**
     * Resize the panel
     * 
     * @param w [int - optional] Width in pixels
     * @param h [int - optional] Height in pixels
     */
    resize : function(w, h) {
        Widget_resize.call (this, w, h);

        if (this._toolbar) {
            w = this.layer.clientWidth;
            this._toolbar.resize (w);
            if (this._tabPanel) {
                h = this.layer.clientHeight - this._toolbar.getHeight ();
                this._tabPanel.resize (w, h);
            }
        }
    },
    
    /**
     * Add a ParameterUI instance to the panel
     *
     * @param paramUI [ParameterUI] 
     * @param label [String]  Parameter title 
     * @param isDataFetching [bool]  Shows the data fetching icon when true
     */
    addParameter : function(kwArgs) {
        kwArgs = MochiKit.Base.update ( {
            paramUI : null,
            label : null,
            isDataFetching : false,
            openAdvCB : null,
            clearValuesCB : null,
            id : this._tabPanel.id + '_P' + (this._tabPanel.getNumTabs () + 1)
        }, kwArgs);

        if (kwArgs.paramUI) {
            var paramTab = bobj.crv.newStackedTab (kwArgs);
            paramTab.setContent (kwArgs.paramUI);
            this._tabPanel.addTab (paramTab);
        }
    },
    

    /**
     * Remove a ParameterUI instance from the panel
     *
     * @param index [int] Index of the widget
     */
    removeParameter : function(index) {
        this._tabPanel.removeTab(index);
    },
    
    getWidth : function() {
        if (this.layer) {
            return this.layer.offsetWidth;
        }
        return this.width;
    },
    
    setResetButtonEnabled : function(isEnabled) {
        this._toolbar.resetButton.setDisabled (!isEnabled);
        var tooltip = isEnabled ? L_bobj_crv_ResetTip : L_bobj_crv_ResetDisabledTip;
        this._toolbar.resetButton.changeTooltip (tooltip, true);
    },
    
    setApplyButtonEnabled : function(isEnabled) {
        this._toolbar.applyButton.setDisabled (!isEnabled);
        var tooltip = isEnabled ? L_bobj_crv_ParamsApplyTip : L_bobj_crv_ParamsApplyDisabledTip;
        this._toolbar.applyButton.changeTooltip (tooltip, true);
    },
    
    isApplyButtonEnabled : function () {
        return this._toolbar != null && this._toolbar.applyButton != null && !this._toolbar.applyButton.isDisabled();
    },
    
    getIndex : function(paramUI) {
        var numTabs = this._tabPanel.getNumTabs ();
        for ( var idx = 0; idx < numTabs; ++idx) {
            var tab = this._tabPanel.getTab (idx);
            if (tab.getContent () === paramUI) {
                return idx;
            }
        }
        return -1;
    },
    
    getParameterTabByWidget : function(paramUI) {
        var index = this.getIndex (paramUI);
        if (index >= 0)
            return this._tabPanel.getTab (index);

        return null;
    },
    
    getParameter : function(index) {
        var tab = this._tabPanel.getTab (index);
        if (tab) {
            return tab.getContent ();
        }
        return null;
    },
    
    /**
     * @return ParameterTab, tab specified by index
     */
    getParameterTab : function(index) {
        return this._tabPanel.getTab (index);
    },
    
    /**
     * return Number of tabs/parameters in panel
     */
    getParameterCount : function() {
        return this._tabPanel.getNumTabs ();
    }
};

/**
 * A layer that appears on top of panel when it is disabled. Prevents user from selecting widgets
 * inside panel
 */
bobj.crv.params.ParameterPanel.OverlayLayer = function(paramPanelID) {
	this.paramPanelId = paramPanelID;
	this.layer = null;
	this.id = bobj.uniqueId();
	this.widx = _widgets.length
	_widgets[this.widx] = this;

	return this;
};

bobj.crv.params.ParameterPanel.OverlayLayer.prototype = {
    setVisible: function(visible) {
        if(!this.layer) {
            this.init();
        }
        if(this.css) {
            this.css.visibility = visible ? "visible" : "hidden";
        }
    },
    
    isVisible: function() {
        if(!this.layer) {
            this.init();
        }
    	return this.css.visibility == "visible";
    },
    
    getHTML: function() {
        return '<div id = ' + this.id + ' onselectstart="return false" ondragstart="return false" onmousedown="'+_codeWinName+'.eventCancelBubble(event)" border="0" hspace="0" vspace="0" src="'+_skin+'../transp.gif" class="paramPanelOverLay">'+(_ie?img(_skin+'../transp.gif','100%','100%',null,'ISMAP'):'')+'</div>'
    },
    
    init: function() {
        var paramPanelLayer = getLayer(this.paramPanelId);

        if(paramPanelLayer) {
            append2(paramPanelLayer, this.getHTML()); //adds overlay layer to parameter panel layer
        }
        
        Widget_init.call(this);
    }
}
