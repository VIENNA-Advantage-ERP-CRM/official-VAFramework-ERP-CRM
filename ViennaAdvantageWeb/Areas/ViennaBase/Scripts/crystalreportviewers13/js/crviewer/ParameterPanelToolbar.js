
/*
================================================================================
ParameterPanelToolbar

Contains the Delete and Run buttons
================================================================================
*/

/**
 * Constructor
 */
bobj.crv.params.newParameterPanelToolbar = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId()
    }, kwArgs);
    var o = newPaletteContainerWidget(kwArgs.id);

    bobj.fillIn(o, kwArgs);  
    o.widgetType = 'ParameterPanelToolbar';
    
    // Attach member functions
    o._paletteContainerInit = o.init;
    MochiKit.Base.update(o, bobj.crv.params.ParameterPanelToolbar);
    
    o._palette = newPaletteWidget(o.id + "_palette");
    o.add(o._palette);
    
    var bind = MochiKit.Base.bind;
    
    o.applyButton = newIconWidget( 
        o.id + '_applyBtn',        
        bobj.crv.allInOne.uri,    
        bind(o._onApplyClick, o), //clickCB,   
        L_bobj_crv_ParamsApply,   //text
        L_bobj_crv_ParamsApplyDisabledTip,//tooltip,   
        16, 16, 3, 3 + bobj.crv.allInOne.paramRunDy, 25, 3 + bobj.crv.allInOne.paramRunDy, false);   //width, height, dx, dy, disDx, disDy     
       
    o.applyButton.setClasses("", "", "", ""); //FIXME : saeed, Assign css class
    o.resetButton = newIconWidget( 
        o.id + '_resetBtn',        
        bobj.crv.allInOne.uri,    
        bind(o._onResetClick, o), //clickCB,   
        L_bobj_crv_Reset,   //text
        L_bobj_crv_ResetDisabledTip,//tooltip,   
        16, 16, 0, bobj.crv.allInOne.undoDy, 16, bobj.crv.allInOne.undoDy, false);   //width, height, dx, dy, disDx, disDy 
    
    o.resetButton.setClasses("", "", "", ""); //FIXME : saeed, Assign css class
    o._palette.add(o.applyButton);
    o._palette.add(); // separator        
    o._palette.add(o.resetButton);

    return o;    
};

bobj.crv.params.ParameterPanelToolbar = {
    init : function() {
        this._paletteContainerInit ();
        this._palette.init ();
        this.applyButton.setDisabled (true);
        this.resetButton.setDisabled (true);
    },
    
    /**
     * Disables tabbing for all buttons in toolbar
     */
    setTabDisabled : function(dis) {
        var items = [ this.applyButton, this.resetButton ];

        for ( var i = 0, len = items.length; i < len; i++) {
            var item = items[i];
            if (item) {
                bobj.disableTabbingKey (item.layer, dis);
            }
        }
    },
    
    /**
     * Overrides parent. Opens the toolbar's tags.
     */
    beginHTML : function() {
        return bobj.html.openTag ('div', {
            id : this.id,
            'class' : 'parameterPanelToolbar'
        });
    },
    
    getHTML : function() {
        return (this.beginHTML () + this._palette.getHTML () + this.endHTML ());
    },
    
    _onApplyClick : function() {
        if (this.applyClickCB) {
            bobj.crv.logger.info ('UIAction ParameterPanel.Apply');
            this.applyClickCB ();
        }
    },
    
    _onResetClick : function() {
        if (this.resetClickCB) {
            this.resetClickCB ();
        }
    }
};
