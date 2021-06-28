// Copyright (c) Business Objects 2010. All rights reserved.
// CrystalReports DHTML Viewer SDK
// Version: Internal (Unsupported)
// ###################################################################################
// CRViewer object contains utilities that allow interaction with the CrystalReports DHTML viewer.
// CRViewer has the following properties and methods defined.
//  CRViewer.getViewer()
//      returns an CRViewer.Viewer object that represents the DHTML viewer on the current page. Use this object to interact with the DHTML viewer.
//  CRViewer.getViewer().addEventListener (event type, listener function)
//      Registers a listener function with the given event type. The listener function should return true if the event was handled and normal viewer processing should not occur.
//  CRViewer.getViewer().removeEventListener (event type, listener function)
//      Removes the listener for the given event type.
//  CRViewer.Events
//      returns an Array of event types that can be handled using event listeners
//  CRViewer.Events.HyperlinkClicked
//      An event that will be fired when a hyperlink is clicked. Note this event will only occur if the viewer is configured to render hyperlinks using javascript.
//      Arguments to listeners of these events will be as follows: { url : 'url string', target : 'target for the link'}
// ###################################################################################

if (typeof (CRViewer) == "undefined") {
    CRViewer = {
        
        getViewer : function (/* optional */ viewerName) {
            return this._getViewer(viewerName);
        },
        
        Events : {
    		HyperlinkClicked : "hyperlinkClicked"
        },
        
        _viewersMap : [],
        _getViewer : function (viewerName) {
        	var map = this._viewersMap;
            if (viewerName) {
                for (var i = 0, len = map.length; i < len; i++) {
                    if (map[i].id == viewerName) {
                        return map[i];
                    }
                }
            } else if (map.length > 0) {
                return map[0];
            }
            
            return this._createViewer(viewerName);
        },
        
        _getRealViewer : function (viewerName)
        {
        	if (viewerName) {
                return getWidgetFromID(viewerName);
            } else {
                if (_widgets) {
                    for (var i = 0, len = _widgets.length; i < len; i++) {
                        if (_widgets[i].widgetType == "Viewer") {
                            return _widgets[i];
                        }
                    }
                }
            }
        },
        
        _createViewer : function (viewerName)
        {
            var realViewer = this._getRealViewer(viewerName);
            
            if (realViewer && realViewer.widgetType == "Viewer") {
                var o = {};
                o.id = realViewer.id;
                o.realViewer = realViewer;
                
                o.addEventListener = function (event, listener) {
                    this.realViewer.addViewerEventListener(event, listener);
                }
                
                o.removeEventListener = function (event, listener) {
                    this.realViewer.removeViewerEventListener(event, listener);
                }
                
                this._viewersMap[this._viewersMap.length] = o;
                return o;
            }
        }
    };
}