bobj.crv.GroupTreeListener = function(groupTree) {
    this._groupTree = groupTree;
    this._groupTreePrevState = this.getTreeState ();
    this._lastNodeRendererd = this.getNumberOfNodesRendered () - 1;
    this._nodeHeight = -1;
    this._futreNodesPlaceHolder = null;
    this.actionIDs = [];
    this.addFutureNodesPlaceHolder ();
    
    MochiKit.Signal.connect(groupTree.layer, "onscroll", bobj.bindFunctionToObject(this.detectTreeChanges, this));
    MochiKit.Signal.connect(groupTree, "refreshed", this, this.reset);
    MochiKit.Signal.connect(groupTree, "resized", this, this.detectTreeChanges);
};

bobj.crv.GroupTreeListener.prototype = {
    /**
     * 
     * @return number of nodes rendered by group tree
     */
    getNumberOfNodesRendered : function() {
        return this._groupTree.getChildrenCount ();
    },

    /**
     * 
     * @return number of nodes that have not been rendered yet
     */
    getNumberOfNodesMissing : function() {
        return this._groupTree.getModalChildren ().length - this._lastNodeRendererd - 1;
    },

    getTreeState : function() {
        var gt = this._groupTree;
        return {
            height :gt.getHeight (),
            scrollTop :gt.layer.scrollTop
        };
    },

    /**
     * 
     * @return number of nodes required to be rendered to fill up the group tree height
     */
    getNumberOfNodesToRender : function() {
        if (this.getNumberOfNodesMissing() == 0)
            return 0;

        var lastNodeRendered = this._groupTree.sub[this._lastNodeRendererd];
        var treeNewState = this.getTreeState ();

        if (lastNodeRendered != null) {
            var offsetY = lastNodeRendered.layer.offsetTop;
            if (offsetY < treeNewState.scrollTop + treeNewState.height) {
                var freeSpace = treeNewState.scrollTop + treeNewState.height - offsetY;
                return Math.floor (freeSpace / this.getNodeHeight ());
            }
        }

        return 0;
    },

    /**
     * 
     * @return a single node height
     */
    getNodeHeight : function() {
        if (this._nodeHeight > -1)
            return this._nodeHeight;
        else if (this.getNumberOfNodesRendered () > 0) {
            var node = this._groupTree.sub[0];
            this._nodeHeight =  bobj.getHiddenElementDimensions (node.layer).h;
            return this._nodeHeight;
        }

        return 0;
    },

    /**
     * 
     *  Adds as many nodes as it can to fill up the grouptree's height 
     */
    updateTreeChildren : function() {
        var numNodesToRender = this.getNumberOfNodesToRender ();
        var groupTreeModalChildren = this._groupTree.getModalChildren ();

        if (numNodesToRender > 0) {
            var childrenHTML = "";
            for ( var i = this._lastNodeRendererd + 1, lastNode = this._lastNodeRendererd + numNodesToRender; i <= lastNode; i++) {
                var modalChild = groupTreeModalChildren[i];
                if (modalChild != null) {
                    var treeNode = bobj.crv.createWidget (modalChild);
                    this._groupTree.addChild (treeNode);
                    childrenHTML += treeNode.getHTML(0);
                    this._lastNodeRendererd  = i;
                }
            }
            
            this._groupTree.appendChildrenHTML(childrenHTML);
            this._groupTree.initChildren();
        }
        
        this.updateFutureNodesPlaceHolderHeight ();
    },
    
    /**
     * Detects if the tree UI has changed and calls update function
     * 
     */
    detectTreeChanges : function() {
        if (this.isTreeStateChanged ())
            this.actionIDs.push(setTimeout(bobj.bindFunctionToObject(this.updateTreeChildren, this), 200));

        this._groupTreePrevState = this.getTreeState ();
    },

    isTreeStateChanged : function() {
        var currentState = this.getTreeState ();
        if (currentState.height != this._groupTreePrevState.height)
            return true;
        if (currentState.scrollTop != this._groupTreePrevState.scrollTop)
            return true;

        return false;
    },

    /**
     * 
     * Results listener's internal state when grouptree is refreshed
     */
    reset : function() {
        this._groupTreePrevState = this.getTreeState ();
        this._lastNodeRendererd = this.getNumberOfNodesRendered () - 1;
        this.clearActions ();
        this.updateFutureNodesPlaceHolderHeight ();
    },
    
    clearActions : function () {
        while(this.actionIDs.length > 0) {
            clearTimeout(this.actionIDs.pop());
        }
    },

    updateFutureNodesPlaceHolderHeight : function() {
        var futurePlaceHolderLayer = this.getFutureNodesPlaceHolderLayer ();
        if (futurePlaceHolderLayer != null) {
            futurePlaceHolderLayer.style.height = (this.getNumberOfNodesMissing () * this.getNodeHeight ()) + "px";
        }
    },

    getFutureNodesPlaceHolderLayer : function() {
        return this._futreNodesPlaceHolder;
    },

    addFutureNodesPlaceHolder : function() {
        this._futreNodesPlaceHolder = MochiKit.DOM.DIV ( {
            id :bobj.uniqueId () + "_futureHolder",
            style : {
                width :'0px',
                height :(this.getNumberOfNodesMissing () * this.getNodeHeight ()) + "px"
            }
        });
        this._groupTree.layer.appendChild (this._futreNodesPlaceHolder);

    }
}
