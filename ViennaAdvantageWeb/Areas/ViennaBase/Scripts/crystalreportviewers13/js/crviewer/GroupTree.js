/*
 ================================================================================
 GroupTree
 ================================================================================
 */

/**
 * GroupTree constructor. 
 *
 * @param kwArgs.id       [String]  DOM node id
 * @param kwArgs.icns     [String]  URL to magnifying glass icon
 * @param kwArgs.minIcon  [String]  URL to min.gif
 * @param kwArgs.plusIcon [String]  URL to plus.gif
 */
bobj.crv.newGroupTree = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    kwArgs = UPDATE ( {
        id : bobj.uniqueId (),
        visualStyle : {
            className : null,
            backgroundColor : null,
            borderWidth : null,
            borderStyle : null,
            borderColor : null,
            fontFamily : null,
            fontWeight : null,
            textDecoration : null,
            color : null,
            width : null,
            height : null,
            fontStyle : null,
            fontSize : null
        },
        icns : bobj.crvUri ('images/magnify.gif'),
        minIcon : bobj.crvUri ('images/min.gif'),
        plusIcon : bobj.crvUri ('images/plus.gif')
    }, kwArgs);

    var o = newTreeWidget (kwArgs.id + '_tree', '100%', '100%', kwArgs.icns, null, null, 'groupTree',
            bobj.crv.GroupTree._expand, bobj.crv.GroupTree._collapse, null, kwArgs.minIcon, kwArgs.plusIcon);

    o._children = [];
    o._modalChildren = [];
    o._lastNodeIdInitialized = -1;
    o._lastNodeInitialized = null;
    o._curSigs = [];
    bobj.fillIn (o, kwArgs);
    o.widgetType = 'GroupTree';
    o.initOld = o.init;

    UPDATE (o, bobj.crv.GroupTree);
   
    return o;
};

bobj.crv.GroupTree = {
    /**
     * Disposes group tree
     */
    dispose : function() {
        /* removes all the signals */
        while (this._curSigs.length > 0) {
            bobj.crv.SignalDisposer.dispose (this._curSigs.pop ());
        }
         
        /* disposes all the children*/
        while (this._children.length > 0) {
            var child = this._children.pop ();
            child.dispose ();
            bobj.deleteWidget (child);
            delete child;
        }

        this._lastNodeIdInitialized = -1;
        this._lastNodeInitialized = null;
        this.sub = [];
        bobj.removeAllChildElements(this.treeLyr);
    },
    
    getModalChildren : function () {
        return this._modalChildren;
    },
    
    /**
     * Adds a child widget as a group tree node.
     * 
     * @param widget
     *            [Widget] Child tree node widget
     */
    addChild : function(widget) {
        var Base = MochiKit.Base;
        var Signal = MochiKit.Signal;
        var connect = Signal.connect;

        widget.expandPath = this._children.length + '';
        this._children.push (widget);

        widget._updateProperty (this.enableDrilldown, this.enableNavigation);
        this.add (widget);
        widget.delayedAddChild (this.enableDrilldown, this.enableNavigation);

        this._curSigs.push (connect (widget, 'grpDrilldown', Base.partial (Signal.signal, this, 'grpDrilldown')));
        this._curSigs.push (connect (widget, 'grpNodeRetrieveChildren', Base.partial (Signal.signal, this, 'grpNodeRetrieveChildren')));
    },
    
    /**
     * Since GroupTree nodes are delay loaded, we would have to store the timeout ids to cancel them in case user drill down to another views
     */
    delayedBatchAdd : function(children) {
        if (!children || children.length == 0)
            return;
        
        this._modalChildren = children;
        var childrenHTML = "";
        
        var numChildrenToRender = children.length > 100 ? 100 : children.length;
        if(numChildrenToRender > 0) {
            for(var i = 0 ; i < numChildrenToRender ; i++) {
                var child = bobj.crv.createWidget (this._modalChildren[i]);
                this.addChild(child);
                if(this.initialized())
                    childrenHTML += child.getHTML(0);
            }
        }
        
        if(this.initialized()) {
            this.appendChildrenHTML (childrenHTML);
            this.initChildren ();
        }
    },
    
    appendChildrenHTML : function (childrenHTML) {
        append (this.treeLyr, childrenHTML);
    },
    
    init : function() {
        this.initOld ();
        bobj.setVisualStyle (this.layer, this.visualStyle);
        this.css.verticalAlign = "top";

        this.initChildren ();
        
        this._groupTreeListener = new bobj.crv.GroupTreeListener(this);
    },
    
    update : function(update) {
        if (update.cons == "bobj.crv.newGroupTree") {
            var args = update.args;
            var path = args.lastExpandedPath;
            /* if path specified, then update specific path, otherwise recreate grouptree */
            // if user has expands a node after session timeout -> the whole gt must be rerendered
            if (path.length > 0 && this._children.length > 0) { 
                this.updateNode (path, update);
            } else {
                this.refreshChildNodes (update)
            }
        }
    },
        
    delayedAddChild : function(widget) {
        this.addChild (widget);
        append (this.treeLyr, widget.getHTML (this.initialIndent));
    },
    
    initChildren : function () {
        while(this._lastNodeIdInitialized < this._children.length - 1)
            this.initNextChild ();
    },
    
    initNextChild : function () {
        var nextNode = null;
        var nextNodeId = -1;
        if(this._lastNodeIdInitialized == -1) {
            var treeSpanLayer = getLayer("treeCont_" + this.id);
            nextNode = treeSpanLayer.firstChild;
            nextNodeId = 0;
        }
        else {
            nextNode = this._lastNodeInitialized.nextSibling;
            while(!(nextNode.id && nextNode.id.indexOf("TWe_") > -1))
                nextNode = nextNode.nextSibling;
            
            nextNodeId = this._lastNodeIdInitialized + 1;
        }
        
        if(nextNodeId < this._children.length && nextNode != null) {
            this._children[nextNodeId].init(nextNode);
            this._lastNodeInitialized = nextNode;
            this._lastNodeIdInitialized = nextNodeId; 
        }
    },
    
    getBestFitHeight : function () {
        return bobj.getHiddenElementDimensions (this.layer).h;
        /**
         * Since container of tree could be invisible, getHiddenElementDimensions has to be called
         * instead of element.getHeight()
         */
    },

    /**
     * refreshes group tree by removing all nodes and adding new ones
     */
    refreshChildNodes : function(update) {
        this.dispose ();
        this.delayedBatchAdd (update.children);
        MochiKit.Signal.signal(this, "refreshed");
    },
   
    /**
     * @param path
     *            path to the node that should be updated eg) 0-1-2
     * @param newTree
     *            the new tree sent in update
     * 
     * Updates children of node specified by path
     */
    updateNode : function(path, newTree) {
        if (path && path.length > 0) {
            var pathArray = path.split ('-');
            var node = this;
            var newNode = newTree;

            /* Navigating to the node that requires update */
            for ( var i = 0, len = pathArray.length; i < len; i++) {
                if (node && newNode) {
                    var childIndex = parseInt (pathArray[i]);
                    newNode = newNode.children[childIndex];
                    node = node._children[childIndex];
                } else {
                    break;
                }
            }

            /* if we found the node that requires update, then update its children */
            if (node && newNode && newNode.args.groupPath == node.groupPath && node._children.length == 0) {
                for ( var nodeNum in newNode.children) {
                    var newChildnode = bobj.crv.createWidget (newNode.children[nodeNum]);
                    node.addChild (newChildnode);
                }

                node.delayedAddChild (this.enableDrilldown, this.enableNavigation);
                node.expand ();
            }
        }
    },
    
    getChildrenCount : function () {
        return this.sub.length;
    },

    /**
     * Private. Callback function when a (complete) group tree node is collapsed.
     */
    _collapse : function(expandPath) {
        MochiKit.Signal.signal (this, 'grpNodeCollapse', expandPath);
    },
    
    /**
     * Private. Callback function when a (complete) group tree node is expanded.
     */
    _expand : function(expandPath) {
        MochiKit.Signal.signal (this, 'grpNodeExpand', expandPath);
    },
    
    resize : function(width, height) {
        bobj.setOuterSize (this.layer, width, height);
        MochiKit.Signal.signal(this, "resized");
    }
};
