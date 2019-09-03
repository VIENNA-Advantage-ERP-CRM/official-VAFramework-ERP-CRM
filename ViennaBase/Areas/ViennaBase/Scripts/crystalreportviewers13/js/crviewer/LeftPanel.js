/**
 * LeftPanel is a container class managing PanelNavigator and ToolPanel classes
 */

bobj.crv.newLeftPanel = function(kwArgs) {
    kwArgs = MochiKit.Base.update ( {
        id : bobj.uniqueId () + "_leftPanel",
        hasToggleGroupTreeButton : true,
        hasToggleParameterPanelButton : true,
        paramIconImg : null,
        treeIconImg : null
    }, kwArgs);

    return new bobj.crv.LeftPanel (kwArgs.id, kwArgs.hasToggleGroupTreeButton, kwArgs.hasToggleParameterPanelButton, kwArgs.paramIconImg, kwArgs.treeIconImg);
};

bobj.crv.LeftPanel = function(id, hasToggleGroupTreeButton, hasToggleParameterPanelButton, paramIconImg, treeIconImg) {
    this._panelNavigator = null;
    this._panelHeader = null;
    this._toolPanel = null;
    this.id = id;
    this.widgetType = 'LeftPanel';
    this.hasToggleParameterPanelButton = hasToggleParameterPanelButton;
    this.hasToggleGroupTreeButton = hasToggleGroupTreeButton;
    this.paramIconImg = paramIconImg;
    this.treeIconImg = treeIconImg;
    this._lastViewedPanel = null;
};

bobj.crv.LeftPanel.prototype = {
    getHTML : function() {
        var toolPanelHTML = this._toolPanel ? this._toolPanel.getHTML () : '';
        var panelHeaderHTML = this._panelHeader ? this._panelHeader.getHTML () : '';
        var navigatorHTML = this._panelNavigator ? this._panelNavigator.getHTML () : '';

        return bobj.html.DIV ( {
            'class' : 'leftPanel',
            id : this.id
        }, navigatorHTML, panelHeaderHTML, toolPanelHTML);
    },

    /**
     * @return width that would best fit both navigator and toolpanel
     */
    getBestFitWidth : function() {
        var w = 0;
        if (this._panelNavigator)
            w += this._panelNavigator.getWidth ();
        
        if (this._toolPanel && this._toolPanel.isDisplayed())
            w += this._toolPanel.getWidth();
        else
            w += 5; //The padding between navigator and reportAlbum when no toolpanel exist
        
        return w;
    },
    
    getBestFitHeight : function () {
        var toolPanelHeight = 0;
        var panelNavigatorHeight = 0;
        
        if(this._panelHeader)
            toolPanelHeight += this._panelHeader.getHeight()
        if(this._toolPanel)
            toolPanelHeight += this._toolPanel.getBestFitHeight();
        if(this._panelNavigator)
            panelNavigatorHeight = this._panelNavigator.getBestFitHeight();
        
        return Math.max (toolPanelHeight, panelNavigatorHeight);
    },

    /**
     * AJAX update flow
     * @param update
     * @return
     */
    update : function(update) {
        if (!update || update.cons != "bobj.crv.newLeftPanel") {
            return;
        }

        for ( var childNum in update.children) {
            var child = update.children[childNum];
            if (child && child.cons == "bobj.crv.newToolPanel") {
                if (this._toolPanel)
                    this._toolPanel.update (child);
                break;
            }
        }
    },

    /**
     * Initializes widgets and connects signals
     */
    init : function() {
        this.layer = getLayer (this.id);
        this.css = this.layer.style;
        
        if (this._toolPanel)
            this._toolPanel.init ();

        if (this._panelHeader) {
            this._panelHeader.init ();
            if(!this.isToolPanelDisplayed())
                this._panelHeader.setDisplay(false);
        }

        if (this._panelNavigator)
            this._panelNavigator.init ();
    },

    /**
     * Connects signals
     * @return
     */
    _initSignals : function() {
        var partial = MochiKit.Base.partial;
        var signal = MochiKit.Signal.signal;
        var connect = MochiKit.Signal.connect;

        if (this._toolPanel) {
            MochiKit.Iter.forEach ( [ 'grpDrilldown', 'grpNodeRetrieveChildren', 'grpNodeCollapse', 'grpNodeExpand', 'resetParamPanel',
                    'resizeToolPanel' ], function(sigName) {
                connect (this._toolPanel, sigName, partial (signal, this, sigName));
            }, this);
        }

        if (this._panelNavigator) 
            connect (this._panelNavigator, "switchPanel", this, '_switchPanel');
        
        if(this._panelHeader)
            connect (this._panelHeader, "switchPanel", this, '_switchPanel');
    },

    /**
     * @return true if toolpanel is displayed
     */
    isToolPanelDisplayed : function() {
        return this._toolPanel && this._toolPanel.isDisplayed ();
    },
    
    /**
     * Do not Remove, Used by WebElements Public API
     */
    displayLastViewedPanel : function () {
        if(this._toolPanel) {
            switch(this._lastViewedPanel)
            {
                case bobj.crv.ToolPanelType.GroupTree:
                    this._switchPanel (bobj.crv.ToolPanelType.GroupTree);
                    break;
                case bobj.crv.ToolPanelType.ParameterPanel:
                    this._switchPanel (bobj.crv.ToolPanelType.ParameterPanel);
                    break;
                default :
                    this._switchPanel (bobj.crv.ToolPanelType.GroupTree);
            }
        }
    },
    
    /**
     * Do not Remove, Used by WebElements Public API
     */
    hideToolPanel : function () {
        this._switchPanel (bobj.crv.ToolPanelType.None);
    },
    
    /**
     * 
     * @param panelType [bobj.crv.ToolPanelType]
     * @return
     */
    _switchPanel : function(panelType) {
        if (this._toolPanel) {
            this._toolPanel.setView (panelType);
            if(panelType == bobj.crv.ToolPanelType.None) {
                this._toolPanel.setDisplay(false);
                this._panelHeader.setDisplay(false);
            }
            else {
                this._toolPanel.setDisplay(true);
                this._panelHeader.setDisplay(true);
                this._lastViewedPanel = panelType;
            }
        }

        if (this._panelHeader)
            var title = bobj.crv.ToolPanelTypeDetails[panelType].title;
            this._panelHeader.setTitle (title);

        if (this._panelNavigator)
            this._panelNavigator.selectChild (panelType);
        
        MochiKit.Signal.signal (this, 'switchPanel', panelType);
    },
    
    /**
     * Do not Remove, Used by WebElements Public API
     */
    getPanelNavigator : function () {
        return this._panelNavigator;
    },

    getToolPanel : function() {
        return this._toolPanel;
    },

    addChild : function(child) {
        if (child.widgetType == "ToolPanel") {
            this._toolPanel = child;
            this.updateChildren ();
            this._initSignals ();
        }
    },

    /**
     * Update Navigator and Header with toolPanel's children
     * @return
     */
    updateChildren : function() {
        if (this._toolPanel) {

            this._panelNavigator = new bobj.crv.PanelNavigator ();
            this._panelHeader = new bobj.crv.PanelHeader ();
            var newChild = null;

            if (this._toolPanel.hasParameterPanel () && this.hasToggleParameterPanelButton) {
                newChild = {
                    name : bobj.crv.ToolPanelType.ParameterPanel,
                    img : this.paramIconImg ? this.paramIconImg : bobj.crv.ToolPanelTypeDetails.ParameterPanel.img,
                    title : bobj.crv.ToolPanelTypeDetails.ParameterPanel.title
                };
                this._panelNavigator.addChild (newChild);
            }
            if (this._toolPanel.hasGroupTree () && this.hasToggleGroupTreeButton) {
                newChild = {
                    name : bobj.crv.ToolPanelType.GroupTree,
                    img : this.treeIconImg ? this.treeIconImg : bobj.crv.ToolPanelTypeDetails.GroupTree.img,
                    title : bobj.crv.ToolPanelTypeDetails.GroupTree.title
                };
                this._panelNavigator.addChild (newChild);
            }
            
            this._lastViewedPanel = this._toolPanel.initialViewType;

            this._panelNavigator.selectChild (this._toolPanel.initialViewType);
            this._panelHeader.setTitle (bobj.crv.ToolPanelTypeDetails[this._toolPanel.initialViewType].title);
            
            if (!this._panelNavigator.hasChildren())
            {
            	this._panelHeader.hideCloseButton();
            	this._toolPanel.addLeftBorder();
            }
        }
    },
    
    resize : function(w, h) {
        bobj.setOuterSize (this.layer, w, h);
        this._doLayout ();
    },
    
    /**
    * Layouts children where PanelHeader appears on top of toolPanel and panelNavigator to left of both header and toolpanel
    * @return
    */
   _doLayout : function() {
       if(!this._toolPanel || !this._panelNavigator || !this._panelHeader)
           return;
       
       var w = this.getWidth ();
       var h = this.getHeight ();
       var navigatorW = this._panelNavigator.getWidth ();
       var newToolPanelWidth = w - navigatorW;
       var newToolPanelHeight = h - this._panelHeader.getHeight ();
       
       if (this._toolPanel.isDisplayed()) {
           this._toolPanel.resize (newToolPanelWidth, newToolPanelHeight);
           this._toolPanel.move (navigatorW, this._panelHeader.getHeight ());
       }
       
       this._panelHeader.resize (newToolPanelWidth, null);
       this._panelHeader.move (navigatorW, 0);
       this._panelNavigator.resize(navigatorW, h);
   },

    move : Widget_move,
    getWidth : Widget_getWidth,
    getHeight : Widget_getHeight
};
