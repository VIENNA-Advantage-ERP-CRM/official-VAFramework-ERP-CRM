/**
 * GroupTreeNode constructor. 
 *
 * @param kwArgs.id     [String]  DOM node id
 * @param kwArgs.groupName  [String]  Name of the group
 * @param kwArgs.groupPath  [String]  Path of the group
 */
bobj.crv.newGroupTreeNode = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    kwArgs = UPDATE ( {
        id : bobj.uniqueId ()
    }, kwArgs);
    var iconAlt = null;
    var iconId = -1; // by default, no icon
    if (!kwArgs.isVisible) {
        iconId = 0;
        iconAlt = L_bobj_crv_Tree_Drilldown_Node.replace ('%1', kwArgs.groupName);
    }

    var o = newTreeWidgetElem (iconId, kwArgs.groupName, kwArgs.groupPath, null, null, null, iconAlt, null, null, false);

    o._children = [];
    o._curSigs = [];
    bobj.fillIn (o, kwArgs);

    o.widgetType = 'GroupTreeNode';
    o.initOld = o.init;
    o.selectOld = o.select;
    o.select = bobj.crv.GroupTreeNode._drilldown;

    if (!kwArgs.isVisible) {
        o.setCursorClass('drill_cursor');
    }    

    UPDATE (o, bobj.crv.GroupTreeNode);

    return o;
};

bobj.crv.GroupTreeNode = {
    /**
     * Diposes grouptree node. Deletes all signals and children
     */        
    dispose : function() {        
        while (this._curSigs.length > 0) {
            bobj.crv.SignalDisposer.dispose (this._curSigs.pop ());
        }
        
        while (this._children.length > 0) {
            var child = this._children.pop ();
            child.dispose ();

            bobj.deleteWidget (child);
            delete child;
        }

        this.sub = [];
        
        //layers of all GroupTreeNodes are disposed in GroupTree.dispose in one batch for performance reason
    },
    
    init : function(layer) {
        this.initOld (layer);
        this._setVisualStyle ();

        if (this.isStatic) {
            /*"treeNormal" is the default css class for tree node text */
            var spans = MochiKit.DOM.getElementsByTagAndClassName ("span", "treeNormal", this.layer);
            if (spans && spans.length > 0) 
                spans[0].style.cursor = 'text';
            
        }
    },
    
    /**
     * @return boolean true if node is expanded
     */
    isExpanded : function() {
        var elemId = TreeIdToIdx (this.layer);
        return _TreeWidgetElemInstances[elemId].expanded;
    },
    
    /**
     * expands node and shows its children
     */
     
     expand : function() {
        var elemId = TreeIdToIdx (this.layer);
        _TreeWidgetElemInstances[elemId].expanded = false
        TreeWidget_toggleCB (elemId);
    },
    
    collapse : function() {
        var elemId = TreeIdToIdx (this.layer);
        _TreeWidgetElemInstances[elemId].expanded = true
        TreeWidget_toggleCB (elemId);
    },
    
    _setVisualStyle : function() {
        try {
            var textNode = this.layer.lastChild;
            var parentNode = this.treeView;
        } catch (err) {
            return;
        }

        var pvStyle = parentNode.visualStyle;
        var tStyle = textNode.style;

        if (pvStyle.fontFamily)
            tStyle.fontFamily = pvStyle.fontFamily;

        if (pvStyle.fontWeight)
            tStyle.fontWeight = pvStyle.fontWeight;

        if (pvStyle.textDecoration)
            tStyle.textDecoration = pvStyle.textDecoration;

        if (pvStyle.color)
            tStyle.color = pvStyle.color;

        if (pvStyle.fontStyle)
            tStyle.fontStyle = pvStyle.fontStyle;

        if (pvStyle.fontSize)
            tStyle.fontSize = pvStyle.fontSize;
    },
    
    /**
     * Delay add the child nodes to a node recursively. 
     * The addition of nodes has to happen in a top-down fashion because each node has a reference to the tree 
     * and this reference is retrieved from the parent node.
     */
    
     delayedAddChild : function(enableDrilldown, enableNavigation) {
        var CONNECT = MochiKit.Signal.connect;
        var SIGNAL = MochiKit.Signal.signal;
        var PARTIAL = MochiKit.Base.partial;
        var childCount = this._children.length;

        if (childCount > 0) {
            this.expanded = true;
        } else {
            this.expanded = false;
            if (!this.leaf) {
                this.setIncomplete (bobj.crv.GroupTreeNode._getChildren);
            }
        }

        var children = this._children;
        for ( var i = 0; i < childCount; i++) {
            var childNode = children[i];
            childNode.expandPath = this.expandPath + '-' + i;
            childNode._updateProperty (enableDrilldown, enableNavigation);
            this.add (childNode);
            this._curSigs.push (CONNECT (childNode, 'grpDrilldown', PARTIAL (SIGNAL, this, 'grpDrilldown')));
            this._curSigs.push (CONNECT (childNode, 'grpNodeRetrieveChildren', PARTIAL (SIGNAL, this, 'grpNodeRetrieveChildren')));
            childNode.delayedAddChild (enableDrilldown, enableNavigation);
        }
    },
    
    addChild : function(widget) {
        this._children.push (widget);
    },
    
    getLevel : function() {
    	return this.expandPath.split('-').length;
    },
    
    /**
     * Private. Callback function when a group tree node is clicked, which is a group drilldown.
     */
     _drilldown : function() {
        this.selectOld ();
        MochiKit.Signal.signal (this, 'grpDrilldown', this.groupName, this.groupPath, this.isVisible, this.groupNamePath);
    },
    
    /**
     * Private. Callback function when an incomplete group tree node is expanded.
     */
    _getChildren : function() {
        this.plusLyr.src = _skin + '../loading.gif';
        MochiKit.Signal.signal (this, 'grpNodeRetrieveChildren', this.expandPath);
    },
    
    /**
     * Private. Change the select event handler and text style class based on the two given flags.
     */
    _updateProperty : function(enableDrilldown, enableNavigation) {
        var isStatic = false;
        if (this.isVisible && !enableNavigation) {
            isStatic = true;
        } else if (!this.isVisible && !enableDrilldown) {
            isStatic = true;
        }

        if (isStatic) {
            this.select = MochiKit.Base.noop;
        }

        this.isStatic = isStatic;
    }
};