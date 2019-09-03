/* Copyright (c) Business Objects 2006. All rights reserved. */

bobj.crv.ToolPanelType = { //values must match server's expectations
    None: 'None',
    GroupTree : 'GroupTree',
    ParameterPanel: 'ParameterPanel'
};

bobj.crv.ToolPanelTypeDetails = {
    None : {
        title : null,
        img : null,
        alt : null
    },
    GroupTree : {
        title : L_bobj_crv_GroupTree,
        img : { uri: bobj.crv.allInOne.uri, dx: 0, dy: bobj.crv.allInOne.groupTreeToggleDy },
        alt : L_bobj_crv_GroupTree
    },
    ParameterPanel : {
        title : L_bobj_crv_ParamPanel,
        img : { uri: bobj.crv.allInOne.uri, dx: 0, dy: bobj.crv.allInOne.paramPanelToggleDy },
        alt : L_bobj_crv_ParamPanel
    }
};

/**
 * ToolPanel constructor
 *
 * @param kwArgs.id     [String]  DOM node id
 * @param kwArgs.width  [String]  Width
 * @param kwArgs.height [String]  Height
 */
bobj.crv.newToolPanel = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId() + "_toolPanel",
        width: '300px',
        height: '100%',
        initialViewType: bobj.crv.ToolPanelType.None
    }, kwArgs);
    

    var o = newWidget(kwArgs.id); 
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);   
    
    o.widgetType = 'ToolPanel';
    
    o._children = [];
    o._selectedChild = null;
    o._groupTree = null;  
    o._paramPanel = null;

    // Update instance with member functions
    o.initOld = o.init;
    o.resizeOld = o.resize;
    MochiKit.Base.update(o, bobj.crv.ToolPanel);
    
    o.needLeftBorder = false;
    
    return o;
};

bobj.crv.ToolPanel = {
    hasGroupTree : function() {
        return this._groupTree != null;
    },
    
    /**
     * Adds a child widget as a view. If the child has an isSelected attribute
     * that evaluates as true, it will be the selected (active) view.
     *
     * This function must be called before getHTML() is called. 
     *
     * @param widget [Widget]  Child view widget
     */
    addChild : function(widget) {
        if (!widget) {
            return;
        }

        var connect = MochiKit.Signal.connect;
        var partial = MochiKit.Base.partial;
        var signal = MochiKit.Signal.signal;
        var Type = bobj.crv.ToolPanelType;

        if (widget.widgetType == 'GroupTree') {
            this._groupTree = widget;

            MochiKit.Iter.forEach ( [ 'grpDrilldown', 'grpNodeRetrieveChildren', 'grpNodeCollapse', 'grpNodeExpand' ], function(sigName) {
                connect (this._groupTree, sigName, partial (signal, this, sigName));
            }, this);

            if (this.initialViewType == Type.GroupTree) {
                this._selectedChild = widget;
            }
        } else if (widget.widgetType == 'ParameterPanel') {
            this._paramPanel = widget;
            connect (this._paramPanel, 'resetParamPanel', partial (signal, this, 'resetParamPanel'));
            if (this.initialViewType == Type.ParameterPanel) {
                this._selectedChild = widget;
            }
        }

        this._children.push (widget);
    },
    
    hasParameterPanel : function () {
        return this._paramPanel != null;
    },
    
    getParameterPanel : function() {
        return this._paramPanel;
    },
    
    /**
     * This method must be called after viewer has been initialized. Adds new child to toolpanel.
     */
    delayedAddChild : function(widget) {
        this.addChild (widget);

        var display = widget === this._selectedChild ? '' : 'none';
        append2 (this.layer, bobj.html.DIV ( {
            style : {
                display : display
            }
        }, widget.getHTML ()));
        widget.init ();
    },
    
    /*
     * The caller is responsible for calling doLayout after making a call to setView. calling doLayout is very expensive and want to avoid it if
     * possible.
     */
    setView : function(panelType) {
        var prevSelectedChild = this._selectedChild;
        this.updateSelectedChild (panelType);
        var nextSelectedChild = this._selectedChild;
        
        if(prevSelectedChild != nextSelectedChild) {
            if (prevSelectedChild) {
                var container = bobj.getContainer (prevSelectedChild);
                if (container) {
                    container.style.display = 'none';
                }
            }
            
            if(nextSelectedChild) {
                var container = bobj.getContainer (nextSelectedChild);
                if (container) {
                    bobj.displayElementWithAnimation(container);
                }
            }
        }
    },
    
    updateSelectedChild : function (panelType) {
        var Type = bobj.crv.ToolPanelType;
        switch(panelType) {
            case Type.GroupTree:
                this._selectedChild = this._groupTree;
                break;
            case Type.ParameterPanel:
                this._selectedChild = this._paramPanel;
                break;
            default:
                this._selectedChild = null;
        }
    },
    
    getHTML : function() {
        var h = bobj.html;

        var content = '';
        var children = this._children;
        for ( var i = 0, len = children.length; i < len; ++i) {
            var child = children[i];
            var display = child === this._selectedChild ? '' : 'none';
            content += h.DIV ( {
                style : {
                    display : display
                }
            }, child.getHTML ());
        }

        var isDisplayed = (bobj.crv.ToolPanelType.None !== this.initialViewType);
        var toolPanelClass = 'toolPanel';
        if (this.needLeftBorder)
            toolPanelClass += ' leftBorder';


        var layerAtt = {
            id : this.id,
            'class' : toolPanelClass,
            style : {
                position : 'absolute',
                margin : '0',
                width : this.width,
                height : this.height,
                overflow : 'hidden',
                display : isDisplayed ? '' : 'none'
            }
        };

        var html = h.DIV (layerAtt, content);

        return html;
    },
    
    init : function() {
        this.initOld ();
        if (this._groupTree)
            this._groupTree.init ();

        if (this._paramPanel)
            this._paramPanel.init ();
    },
    
    update : function(update) {
        if (update && update.cons == "bobj.crv.newToolPanel") {
            for ( var childVar in update.children) {
                var child = update.children[childVar];
                if (child) {
                    switch (child.cons) {
                        case "bobj.crv.newGroupTree":
                            /* if there is a group tree, update it */
                            if (this._groupTree) {
                                this._groupTree.update (child);
                            }
                            /* else create and add one */
                            else {
                                this.delayedAddChild (bobj.crv.createWidget (child));
                            }
                            break;
                        case "bobj.crv.params.newParameterPanel":
                            if (this._paramPanel) {
                                this._paramPanel.update (child);
                            }
                            else {
                                this.delayedAddChild (bobj.crv.createWidget (child));
                            }
                            break;
                    }
                }
            }

            this.initialViewType = update.args.initialViewType;
            this.setView (this.initialViewType);
            this.css.width = update.args.width;
        }
    },
    
    getBestFitHeight : function () {
        var height = 0;
        if(this._selectedChild != null)
            height = this._selectedChild.getBestFitHeight(); 
        
        return height;
    },
    
    hasPercentWidth : function () {
        return (this.width != null) && (this.width.length > 0) && (this.width.charAt(this.width.length - 1) == '%');
    },
    
    getPercentWidth : function () {
        return parseInt(this.width) / 100;
    },
            
    _doLayout : function() {
        var innerWidth = this.layer.clientWidth;
        var contentHeight = this.layer.clientHeight;

        if (this._selectedChild) {
            this._selectedChild.setDisplay (true);
            this._selectedChild.resize (innerWidth, contentHeight);
        }
    },
    
    resize : function(w, h) {
        bobj.setOuterSize (this.layer, w, h);
        this._doLayout ();

        var width = _ie && _isQuirksMode ? this.layer.offsetWidth : this.layer.clientWidth;
        var height = _ie && _isQuirksMode ? this.layer.offsetWidth : this.layer.clientWidth;
        MochiKit.Signal.signal (this, 'resizeToolPanel', width, height);
        
        // do not have a percent width if we have resized the tool panel
        this.width = width;
    },

    addLeftBorder : function() {
        this.needLeftBorder = true;
    }
};
