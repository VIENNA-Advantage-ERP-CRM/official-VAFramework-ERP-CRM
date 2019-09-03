/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof(bobj.crv.StackedPanel) == 'undefined') {
    bobj.crv.StackedPanel = {};
}

/**
 * Constructor. 
 *
 * @param id      [String]  DHTML id
 * @param width   [int]     Width of the panel in pixels
 * @param height  [int]     Height of the panel in pixels
 */
bobj.crv.newStackedPanel = function(kwArgs) {
    var mb = MochiKit.Base;
    var UPDATE = mb.update;
    var BIND = mb.bind;
    
    kwArgs = UPDATE({
        id: bobj.uniqueId(),
        width: null,
        height: null
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);
    o.widgetType = 'StackedPanel';
    bobj.fillIn(o, kwArgs);  
    
    o._tabs = [];
    
    o._initWidget = o.init;
    o._resizeWidget = o.resize;
    UPDATE(o, bobj.crv.StackedPanel);

    
    return o;    
};

bobj.crv.StackedPanel = {
    init : function() {
        this._initWidget ();

        var tabs = this._tabs;
        /* Tabs that were added after getHTML must be written now */
        var index = this._numTabsWritten;
        while (index < tabs.length) {
            append (this.layer, tabs[index].getHTML (), document);
            index++;
        }

        for ( var i = 0, len = tabs.length; i < len; ++i) {
            tabs[i].init ();
        }
    },

    setTabDisabled : function(dis) {
        for ( var i = 0, len = this._tabs.length; i < len; i++) {
            this._tabs[i].setTabDisabled (dis);
        }
    },
    
    getHTML : function() {
        var DIV = bobj.html.DIV;

        var layerStyle = {};

        if (this.height) {
            layerStyle.height = bobj.unitValue (this.height);
        }

        if (this.width) {
            layerStyle.width = bobj.unitValue (this.width);
        }

        return DIV ( {
            id : this.id,
            style : layerStyle,
            'class' : 'stackedPanel',
            tabIndex : "-1"
        }, this._getTabsHTML ());
    },
    
    _getTabsHTML : function() {
        var tabsHTML = '';
        var tabs = this._tabs;
        var tabsLen = tabs.length;
        for ( var i = 0; i < tabsLen; ++i) {
            tabsHTML += tabs[i].getHTML ();
        }
        this._numTabsWritten = tabsLen;
        return tabsHTML;
    },
    
    /**
     * Add a tab to the panel. Must be called before getHTML is called.
     * 
     * @param tab
     *            [StackedTab]
     */
    addTab : function(tab) {
        if (tab) {
            this._tabs.push (tab);
            if (this.layer) {
                append (this.layer, tab.getHTML ());
                tab.init ();
            }
            
            if (this.layer) 
                tab.resize(this.layer.clientWidth);
            
            MochiKit.Signal.connect(tab, "StackedTabResized", this, '_onStackedTabResize');
        }
    },
    
    getNumTabs : function() {
        return this._tabs.length;
    },
    
    getTab : function(index) {
        return this._tabs[index];
    },
    
    removeTab : function(index) {
        if (index >= 0 && index < this._tabs.length) {
            var tab = this._tabs[index];
            this._tabs.splice (index, 1);
            delete _widgets[this._tabs.widx];
            if (tab.layer) {
                tab.layer.parentNode.removeChild (tab.layer);
            }
        }
    },
    
    _onStackedTabResize: function () {
        this.resize (this.getWidth());
    },
    
    resize : function(w, h) {
        /* Exclude margins for safari as it miscalculates left/top margins */
        var excludeMargins = !_saf; 
        bobj.setOuterSize(this.layer, w, h, excludeMargins);
        var tabs = this._tabs;
        var tabsLen = tabs.length;
        if (tabsLen) {
            /* Ensure that the vertical scrollbar never covers the content*/
            var tabWidth = this.layer.clientWidth;
            
            /* IE changes the value of clientWidth after resizing the first child... */
            tabs[0].resize(tabWidth);
            if (tabWidth != this.layer.clientWidth) {
                tabWidth = this.layer.clientWidth; 
                tabs[0].resize(tabWidth);
            }
            
            for (var i = 1; i < tabsLen; ++i) {
                tabs[i].resize(tabWidth);    
            }
        }
    }
};
