/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof(bobj.crv.SharedWidgetHolder) == 'undefined') {
    bobj.crv.SharedWidgetHolder = {};
    bobj.crv.SharedWidgetHolder._registry = {};
}

/**
 * Constructor. SharedWidgetHolder is a placeholder for a widget. 
 * SharedWidgetHolder instances belong to groups, in which one managed 
 * widget will be shown in one placeholder at a time. When show(true) is called  
 * for any member of the group, the managed widget will be displayed in that 
 * member.
 *
 * @param id           [String]       DHTML id
 * @param group        [String]       Group the new instance will belong to
 * @param width        [int | String] Width of the placeholder
 * @param height       [int | String] Height of the placeholder
 * @param resizeWidget [bool]         Resize to the placeholder's dimensions
 */
bobj.crv.newSharedWidgetHolder = function(kwArgs) {
    var mb = MochiKit.Base;
    var ms = MochiKit.Signal;
    
    kwArgs = mb.update({
        id: bobj.uniqueId(),
        group: 'SharedWidgetHolder', 
        width: null,
        height: null,
        resizeWidget: true
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);
    o.widgetType = 'SharedWidgetHolder';
    bobj.fillIn(o, kwArgs);  
    
    o._setVisible = o.show;
    o._resizeHolder = o.resize;
    o._initHolder = o.init;
    mb.update(o, bobj.crv.SharedWidgetHolder);
    
    o._register();
    
    return o;    
};

bobj.crv.SharedWidgetHolder.init = function() {
    this._initHolder();
    var regInfo = this._regInfo;
    if (regInfo.managedWidget && this === regInfo.visibleHolder) {
        regInfo.managedWidget.init(); 
        this.resize();
    }
};

bobj.crv.SharedWidgetHolder.getHTML = function() {
    var ISNUMBER = bobj.isNumber;
    var ISSTRING = bobj.isString;
    
    var vis = this.isHoldingWidget() ? 'visible' : 'hidden';
    var innerHTML = this.isHoldingWidget() ?  this._regInfo.managedWidget.getHTML() : '';
    
    var style = {visibility: vis};
    
    var width = this.width;
    if (ISNUMBER(width)) {
        width = width + 'px';
    }
    if (ISSTRING(width)) {
        style.width = width;
    }
    
    var height = this.height;
    if (ISNUMBER(height)) {
        height = height + 'px';
    }
    if (ISSTRING(height)) {
        style.height = height;    
    }
    
    return bobj.html.DIV({id: this.id, style: style}, innerHTML);
};

/**
 * Private. Register this instance in a group.
 */
bobj.crv.SharedWidgetHolder._register = function() {
    var registry = bobj.crv.SharedWidgetHolder._registry;
    var holderInfo = registry[this.group];
    
    if (!holderInfo) {
        holderInfo = {
            managedWidget: null,
            visibleHolder: this
        };
        registry[this.group] = holderInfo;
    }
    
    this._regInfo = holderInfo;
};

/**
 * @return [bool] True if and only if this instance is currently holding a 
 *                non-null managed widget. Only one member of a group can return
 *                true at any given time.
 */
bobj.crv.SharedWidgetHolder.isHoldingWidget = function() {
    var regInfo = this._regInfo; 
    return ((regInfo.visibleHolder === this) && regInfo.managedWidget);    
};

/**
 * @return [Widget] The widget that is managed by the holder group that this
 *                  instance belongs to.
 */
bobj.crv.SharedWidgetHolder.getManagedWidget = function() {
    return this._regInfo.managedWidget;
};

/**
 * Set the widget that is managed by the group that this isntance belongs to.
 * The widget will be displayed in the currently visible member of the group.
 *
 * @param widget [Widget]  The widget that should be managed. 
 */
bobj.crv.SharedWidgetHolder.setManagedWidget = function(widget) {
    var regInfo = this._regInfo;
    var oldWidget = regInfo.managedWidget;
    regInfo.managedWidget = widget;
    
    if (oldWidget && oldWidget.layer) {
        MochiKit.DOM.removeElement(oldWidget.layer);
    }
    
    if (!regInfo.visibleHolder) {
        regInfo.visibleHolder = this;
    }
    
    var holder = regInfo.visibleHolder;
    
    if (holder.layer && widget && widget.layer) {
        holder.layer.appendChild(widget.layer);
        holder.resize();
    }
};

/**
 * Alias for setManagedWidget. Allows instantiation using bobj.crv.createWidget(); 
 */
bobj.crv.SharedWidgetHolder.addChild = bobj.crv.SharedWidgetHolder.setManagedWidget;

/**
 * Show or hide the managed widget in this holder instance.
 *
 * @param show [bool]  If true, the managed widget will be displayed in this
 *                     instance. If false and this holder is currently showing
 *                     the managed widget, the widget will be hidden.  
 */
bobj.crv.SharedWidgetHolder.show = function(show) {
    var regInfo = this._regInfo;
    if (show && (regInfo.visibleHolder !== this) && regInfo.managedWidget) {
        this.layer.appendChild(regInfo.managedWidget.layer);
        regInfo.visibleHolder._setVisible(false);
        regInfo.visibleHolder = this;
        this.resize();
    }
    this._setVisible(show);
};

/**
 * Resize the holder instance. If resizeWidget property is true, the managed 
 * widget will be resized to the dimensions of the placeholder. 
 *
 * @param w [width, optional]   Width in pixels
 * @param h [height, optional]  Height in pixels 
 */
bobj.crv.SharedWidgetHolder.resize = function(w, h) {
    this._resizeHolder(w, h);
    
    if (this.resizeWidget && this.isHoldingWidget()) {
        this._regInfo.managedWidget.resize(this.getWidth(), this.getHeight());    
    }
};