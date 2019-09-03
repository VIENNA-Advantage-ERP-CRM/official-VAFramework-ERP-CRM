
/*
================================================================================
StackedTab
================================================================================
*/
bobj.crv.newStackedTab = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
   
    kwArgs = UPDATE({
        id: bobj.uniqueId(),
        label: '',
        width: 300,
        height: null,  // null height means grow as big as necessary
        openAdvCB: null,
        name : '',
        isDataFetching: false
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);
    o.widgetType = 'StackedTab';
    bobj.fillIn(o, kwArgs);  
    
    o._content = null;
    
    if(o.openAdvCB) {
        o._advanceButton = newIconWidget( 
            o.id + '_advBtn',        
            bobj.crv.allInOne.uri,    
            o.openAdvCB, 
            null,   //text
            L_bobj_crv_paramsOpenAdvance.replace("%1", o.name),//tooltip,   
            10, 10, 3, bobj.crv.allInOne.openParameterArrowDy + (_ie ? 2 : 3));   //width, height, dx, dy, disDx, disDy, cancelEventPropagation
        bobj.crv.setAllClasses(o._advanceButton, 'arrow_button');
        o._advanceButton.margin = 0;
    }
    
    o._initWidget = o.init;
    o._resizeWidget = o.resize;
    UPDATE(o, bobj.crv.StackedTab);
    
    return o;    
};

bobj.crv.StackedTab = {
    setTabDisabled : function(dis) {
        if (this._content)
            this._content.setTabDisabled (dis);

        if (this._advanceButton && this._advanceButton.layer)
            bobj.disableTabbingKey (this._advanceButton.layer, dis);

        if (this._textCtn)
            bobj.disableTabbingKey (this._textCtn, dis);
        
        if(this._dataFetchLayer)
            bobj.disableTabbingKey (this._dataFetchLayer, dis);
    },
    
    init : function() {
        var connect = MochiKit.Signal.connect;
        var signal = MochiKit.Signal.signal;
        var partial = MochiKit.Base.partial;
        
        this._initWidget ();

        if (this._content) {
            this._content.init ();
        }

        if (this._advanceButton) {
            this._advanceButton.init ();
            this._onAdvanceButtonClickOld = MochiKit.Base.bind(this._advanceButton.layer.onclick, this._advanceButton.layer); 
            this._advanceButton.layer.onclick = MochiKit.Base.bind(this.advButtonOnClick, this);
            this._advanceButton.css.width = "14px";
        }
        
        this._dataFetchLayer = getLayer(this.id + "_df");
        this._labelCtn = getLayer (this.id + '_labelCtn');
        this._textCtn = getLayer (this.id + '_textCtn');
        this._contentCtn = getLayer (this.id + '_contentCtn');

        if (this._advanceButton) {
            var advButtonLayer = this._advanceButton.layer;
            var bind = bobj.bindFunctionToObject;
            connect (this.layer, 'onclick', bind(IconWidget_upCB, advButtonLayer));
            connect (this.layer, 'onmouseover', bind(IconWidget_realOverCB, advButtonLayer));
            connect (this.layer, 'onmouseout', bind(IconWidget_realOutCB, advButtonLayer));
            connect (this.layer, 'onmousedown', bind(IconWidget_downCB, advButtonLayer));
            
        }
        
        connect(this._content, 'ParameterUIResized',  partial (signal, this, "StackedTabResized"));
    },
        
    getHTML : function() {
        var h = bobj.html;
        var DIV = h.DIV;
        var IMG = h.IMG;
        
        var stackedTabAtt = {
            'class' : 'stackedTab',
            cellpadding : "0",
            id : this.id,
            style : {
                cursor : this._advanceButton ? _hand : 'default'
            }
        };
        
        var labelCtnAtt = {
            id: this.id + '_labelCtn',
            cellpadding : "0",
            'class': 'crvnoselect stackedTabTitle'
        };
                
        var contentHTML = this._content ? this._content.getHTML() : '';
        var advButtonHTML = this._advanceButton ? h.TD ({width : '17px'}, this._advanceButton.getHTML()) : ''; /* 14 for arrow height + 3px right margin */
        var dataFetchHTML = "";
        
        if (this.isDataFetching) {
            var URL_TAG = "url(%1);"
            dataFetchHTML = h.TD ({width : '20px'}, IMG ( {
                src : _skin + '../transp.gif',
                title : L_bobj_crv_ParamsDataTip,
                tabindex: 0,
                id: this.id + "_df",
                style : {
                    width : "16px",
                    height : "16px",
                    "background-image" : URL_TAG.replace("%1", bobj.crv.allInOne.uri),
                    "background-position" : "0px " + (-bobj.crv.allInOne.paramDataFetchingDy) + "px",
                    "margin-right" : '4px',
                    'vertical-align' : 'middle'
                }
            }));
        }

        var html = DIV (stackedTabAtt, DIV (labelCtnAtt, h.TABLE ( {
            cellpadding : "0",
            width : '100%',
            height : '20px',
            style : {
                'table-layout' : 'fixed'
            }
        }, h.TD ({style : {"vertical-align" : "top", "overflow" : "hidden"}}, DIV ( {
            'class' :'stackedTabText',
            id :this.id + '_textCtn',
            title :this.label,
            'tabIndex' :0,
            style : {
                'font-weight' : 'bold',
                'color' : '#4F5C72'
            }
        }, convStr (this.label))
        ), dataFetchHTML, advButtonHTML)
        ), DIV ( {
            id :this.id + '_contentCtn',
            'class' :'stackedTabContentCtn'
        }, contentHTML));
            
        return html;
    },
    
    setDirty : function(isDirty) {
        if(this._textCtn) {
            this._textCtn.style.fontStyle = isDirty ? "italic" : "";
            this._textCtn.title = isDirty ? this.label + " " + L_bobj_crv_ParamsDirtyTip : this.label;
        }
        
        if(this._labelCtn) {
            var titleClassName = isDirty ? "stackedTabTitleDirty" : "stackedTabTitle";
            
            // IE 7 uses "className"
            this._labelCtn.setAttribute("className", titleClassName);
            
            // IE 8, FireFox, and Safari use "class"
            this._labelCtn.setAttribute("class", titleClassName);
        }
    },
    
    resize : function(w) {
        w = w - 4; // TODO exclude margins properly
        if(this._labelCtn) {
            // Exclude margins for safari as it miscalculates left/top margins
            var excludeMargins = !_saf; 
            bobj.setOuterSize(this._labelCtn, w  , null, excludeMargins);   
        }
        if (this._content) { 
            this._content.resize(w -2);
        }    
        bobj.setOuterSize(this.layer, w);
    },
    
    /**
     * Set the widget that is displayed below the tab. Must be called before getHTML.
     *
     * @param widget [Widget]  Widget that appears below the tab when the tab is expanded
     */
    setContent : function(widget) {
        this._content = widget;   
    },
    
    /**
     * Get the widget that is displayed below the tab.
     *
     * @return [Widget]  Widget that appears below the tab
     */
    getContent : function() {
        return this._content;   
    },
    
    /**
     * Focus the advanced button if available
     */
    focusAdvButton : function() {
    	if (this._advanceButton && this._advanceButton.focus) 
    		this._advanceButton.focus();
    },
    
    /**
     * Override default onclick for the advanced button to cancel propagating the event.
     */
    advButtonOnClick : function(e) {
    	if (this._onAdvanceButtonClickOld)
    	{
    		this._onAdvanceButtonClickOld(e);
    		eventCancelBubble(e);
    	}
    }
};