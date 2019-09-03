/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * Constructor. StateManager holds state for multiple viewers.  
 */
bobj.crv.StateManager = function() {
    this._state = {};
};

bobj.crv.StateManager.prototype = {
    /**
     * Set the state object for a report view
     *
     * @param viewerName [String]  
     * @param stateName  [String]  The name of the report view 
     * @param viewState  [Object]  The state associated with the report view  
     */
    setViewState: function(viewerName, stateName, viewState) {
        var state = this._state;
        
        if (!state[viewerName]) {
            state[viewerName] = {};
        }
        
        state[viewerName][stateName] = viewState;
    },
    
    /**
     * Get the state object for a report view
     *
     * @param viewerName [String]  
     * @param stateName  [String]  The name of the report view 
     *
     * @return [Object] Returns the state object for the report view or null
     *                  if no object is associated with (viewerName, stateName)
     */
    getViewState: function(viewerName, stateName) {
        var state = this._state;
        
        if (!state[viewerName]) {
            return null;    
        }
        return state[viewerName][stateName];
    },
    
    /**
     * Set the compound state object for a viewer components. This object 
     * should contain a state object for every report view displayed by the 
     * viewer.
     *
     * @param viewerName [String]  
     * @param state      [Object]  All report view states for the viewer  
     */
    setComponentState: function(viewerName, state) {
        this._state[viewerName] = state;
    },
    
    /**
     * Get the compound state object for a viewer component. This object 
     * should contain a state object for every report view displayed by the 
     * viewer.
     *
     * @param viewerName [String]  
     *
     * @return [Object]  Returns all report view states for the viewer  
     */
    getComponentState: function(viewerName) {
        return this._state[viewerName];
    },
    
    /**
     * Get the state for all viewer components on the page.
     *
     * @return [Object] Returns the state of all viewers on the page, mapped
     *                  by the id of the viewer widgets.
     */
    getCompositeState: function() {
        return this._state;
    }
};

// Create a single StateManager for all viewers in the page to share
if (typeof bobj.crv.viewerState == 'undefined') {
    bobj.crv.stateManager = new bobj.crv.StateManager();
}

