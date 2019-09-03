/*
================================================================================
ParameterValueRow

Internal class for use by ParameterUI. Darws a UI for a single parameter value. 
================================================================================
*/

bobj.crv.params.newParameterValueRow = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        value: '',
        defaultValues: null,
        isReadOnlyParam: true,
        canChangeOnPanel: false,
        isRangeValue : false,
        allowCustom: false,
        isPassword: false,
        calendarProperties : {displayValueFormat : '' , isTimeShown : false, hasButton : false, iconUrl : '', clickCB : null}, 
        changeCB: null,
        enterCB: null,
        defaultValuesMenu: null,
        tooltip : null,
        canOpenAdvDialog : false
    }, kwArgs);

    var o = newWidget(kwArgs.id);   
    o.widgetType = 'ParameterValueRow';
    o._prevValueString = kwArgs.value;
    o._warning = null;
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs); 
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.params.ParameterValueRow);
    
    return o;
};

bobj.crv.params.ParameterValueRow = {
    setTabDisabled : function(dis) {
        if (this._valueWidget)
            this._valueWidget.setTabDisabled (dis);

        if (this._calendarButton)
            bobj.disableTabbingKey (this._calendarButton.layer, dis);
    },
    
    /**
     * Resets widget, Restores old value, hides warning and adv dialog icons
     */
    reset : function(value) {
        this.value = value;
        this._prevValueString = value;
        this.setWarning(null);
        /* Removing Red border color if previous value was errorneous */
        MochiKit.DOM.removeElementClass(this.layer, "hasError"); 
        
        if(this._valueWidget) {
            this._valueWidget.reset(value);
        }
    },
    
    init : function() {
        Widget_init.call (this);
        this._valueWidget.init ();
        if (this._calendarButton) {
            this._calendarButton.init ();
        }

        this._valueCtn = getLayer (this.id + '_vc');
        this._btnCtn = getLayer (this.id + '_bc');
        this._valBtnCtn = getLayer (this.id + '_vab');

        if (MochiKit.Base.isIE ()) {
            /* IE's 100% is different than other browsers,
               even in standards compliant mode.... */
            var marg = parseInt (MochiKit.Style.computedStyle (this._valueCtn, 'margin-right'), 10);
            if (bobj.isNumber (marg)) {
                this._valueWidget.layer.style.marginRight = (-1 * marg) + 'px';
            }
        }
    },
    
    calendarSetValue : function(value) {
        this.setValue (value);
        this.changeCB ();
    },
    
    getHTML : function() {
        if (!this._valueWidget) {
            this._valueWidget = this._getValueWidget ();
        }
        if (this.calendarProperties.hasButton && !this._calendarButton) {
            var getValueCB = bobj.bindFunctionToObject (this.getValue, this);
            var setValueCB = bobj.bindFunctionToObject (this.calendarSetValue, this);
            this._calendarButton = bobj.crv.params.newCalendarButton ( {
                calendarProperties : this.calendarProperties,
                getValueCB : getValueCB,
                setValueCB : setValueCB
            });
        }

        var DIV = bobj.html.DIV;
        var IMG = bobj.html.IMG;

        var cssClass = ' iactParamRow';

        if (this.canChangeOnPanel)
            cssClass += ' editable';
        else
            cssClass += ' readOnly';

        if (MochiKit.Base.isIE () && bobj.isQuirksMode ()) {
            cssClass += ' ' + 'iactParamRowIE';
        }

        return DIV ( {
            id : this.id,
            'class' : cssClass
        }, this.calendarProperties.hasButton ? this._getValueAndCalendarHTML () : this._getValueHTML ());
    },
    

    /**
     * @return [String] Returns HTML for the the value
     */
    _getValueHTML : function() {
        var DIV = bobj.html.DIV;
        var style = {};

        if (MochiKit.Base.isIE () && bobj.isQuirksMode ()) {
            style.position = "absolute";
            style.top = "0px";
            style.left = "0px";
        }

        return DIV ( {
            id : this.id + '_vc',
            'class' : 'iactParamValue',
            style : style
        }, this._valueWidget.getHTML ());
    },
    
    /**
     * @return [String] Returns HTML for the value and a button beside it
     */
    _getValueAndCalendarHTML : function() {
        var style = {};
        if (MochiKit.Base.isIE () && bobj.isQuirksMode ()) {
            style.width = '100%';
        }

        var DIV = bobj.html.DIV;

        var buttonRightOffset = (this._valueWidget.widgetType == "TextCombo") ? 16 : 0;
        buttonRightOffset += "px";

        var html = DIV ( {
            id : this.id + "_vab",
            style : style,
            'class' : "iactParamValueAndButton"
        }, this._getValueHTML (), DIV ( {
            id : this.id + "_bc",
            'class' : "iactValueIcon",
            style : {
                position : "absolute",
                right : buttonRightOffset,
                top : "0",
                cursor : _hand
            }
        }, this._calendarButton.getHTML ()));

        return html;
    },
    
    getNewValueWidgetConstructor : function() {
        var showDefVals = this.defaultValuesMenu !== null && !this.isReadOnlyParam && this.canChangeOnPanel;

        if (this.isRangeValue)
            return bobj.crv.params.newRangeField;
        else if (showDefVals)
            return bobj.crv.params.newTextCombo;
        else
            return bobj.crv.params.newTextField;
    },

    getNewValueWidgetArgs : function() {
        var isEditable = this.allowCustom && this.canChangeOnPanel;
        return {
            password : this.isPassword,
            cleanValue : this.value,
            editable : isEditable,
            enterCB : bobj.bindFunctionToObject (this._onEnterPress, this),
            keyUpCB : isEditable ? bobj.bindFunctionToObject (this._onKeyUp, this) : null,
            tooltip : this.tooltip,
            foreColor : this.isReadOnlyParam ? bobj.Colors.GRAY : bobj.Colors.BLACK,
            focusCB : bobj.bindFunctionToObject (this.onFocus, this),
            blurCB : bobj.bindFunctionToObject (this.onBlur, this),
            canOpenAdvDialog : this.canOpenAdvDialog
        };
    },
    
    /**
    * Creates a widget to display/edit the value.
    * 
    * @return [Widget]
    */
    _getValueWidget : function() {
        var cons = this.getNewValueWidgetConstructor ();
        var widget = cons (this.getNewValueWidgetArgs ());
        var showDefVals = this.defaultValuesMenu !== null && !this.isReadOnlyParam && this.canChangeOnPanel;

        if (showDefVals) {
            widget.setMenu (this.defaultValuesMenu);
            widget.changeCB = bobj.bindFunctionToObject (this._onChange, this);
        }

        return widget;
    },
    
    onFocus : function() {
        this.refreshWarningPopup ();
        MochiKit.DOM.removeElementClass (this.layer, "hasError");
    },
    
    refreshWarningPopup : function() {
        if (this._warning) {
            var pos = getPosScrolled (this.layer);
            var fontFamily = MochiKit.Style.computedStyle (this.layer, 'fontFamily');
            var fontSize = MochiKit.Style.computedStyle (this.layer, 'fontSize');
            var valueWidth = bobj.getStringWidth (this.getValue (), fontFamily, fontSize);
            var leftOffset = this.layer.offsetWidth < valueWidth ? this.layer.offsetWidth : valueWidth;
            var topOffset = 33;
            bobj.crv.WarningPopup.getInstance ().show (this._warning.message, pos.x + leftOffset, pos.y + topOffset);
        } else {
            bobj.crv.WarningPopup.getInstance ().hide ();
            MochiKit.DOM.removeElementClass (this.layer, "hasError");
        }

        var tooltipText = this._warning ? this.tooltip + this._warning.message : this.tooltip;
        if (this._valueWidget)
            this._valueWidget.setTooltip (tooltipText);
    },
    
    onBlur : function() {
        if (this._warning) {
            bobj.crv.WarningPopup.getInstance ().hide ();
            MochiKit.DOM.addElementClass (this.layer, "hasError");
        }
    },
    
    getValue : function() {
        return this.value;
    },
    
    setValue : function(value) {
        this.value = value;
        if (this._valueWidget)
            this._valueWidget.setValue (value);
    },
    
    setCleanValue : function(value) {
        if (this._valueWidget) 
            this._valueWidget.setCleanValue(value);    
            
    },
    
    /**
     * Set keyboard focus to the widget and mark it as selected
     */
    focus : function() {
        if (this._valueWidget.widgetType == "TextCombo")
            this._valueWidget.text.focus ();
        else
            this._valueWidget.focus ();
    },
    
    /**
     * Display a warning icon with a tooltip message
     * 
     * @param warning
     *        [String] Tooltip message. If null, warning is hidden.
     */
    setWarning : function(warning) {
        this._warning = warning;
        this.refreshWarningPopup ();
    },

    getWarning : function() {
        return this._warning;
    },
    
    /**
     * Change the outer dimensions of the widget
     * 
     * @param w
     *        [int, optional] Width in pixels
     * @param h
     *        [int, optional] Height in pixels
     */
    resize : function(w, h) {
        bobj.setOuterSize (this.layer, w, h);
    },
    
    deleteValue : function() {
        this._valueWidget.setValue ('', true);
    },
    
    /**
     * Handle keyUp events when editing values or using keyboard navigation.
     * 
     * @param e
     *        [keyup event]
     */
    _onKeyUp : function(e) {
        var event = new MochiKit.Signal.Event (src, e);
        var key = event.key ().string;
        var newValueString = this._valueWidget.getValue ();

        switch (key) {
            case "KEY_ESCAPE":
                this._valueWidget.setValue (this._valueWidget.cleanValue);
                this._onChange ();
                break;

            case "KEY_ARROW_LEFT":
            case "KEY_ARROW_RIGHT":
            case "KEY_HOME":
            case "KEY_END":
            case "KEY_TAB":
                break;

            default:
                if (newValueString !== this._prevValueString) {
                    this._onChange ()
                    this._prevValueString = newValueString;
                }
                break;
        }
    },

    /**
     * Delete the current value
     */
    deleteValue : function() {
        this._valueWidget.setValue ('', true);
    },
    
    _onChange : function() {
        this.value = this._valueWidget.getValue();
        if (this.changeCB) 
            this.changeCB();    
    },
    
    /**
     * Handles Enter key press events.
     */
    _onEnterPress : function() {  
        if(this.enterCB)
            this.enterCB();
    }
};



/*
================================================================================
CalendarButton

Internal class. Button that fits inline with parameter values
================================================================================
*/
bobj.crv.params.newCalendarButton = function(kwArgs) {
    kwArgs = MochiKit.Base.update( {
        id : bobj.uniqueId(),
        calendarProperties : null,
        getValueCB : null,
        setValueCB : null,
        calendarSignals : {
            okSignal : null,
            hideSignal : null,
            cancelSignal : null
        }
    }, kwArgs);

    if (kwArgs.getValueCB == null || kwArgs.setValueCB == null || kwArgs.calendarProperties == null)
        throw "InvalidArgumentException";

    var o = newIconWidget(
            kwArgs.id,
            kwArgs.calendarProperties.iconUrl,
            null, 
            null,
            L_bobj_crv_ParamsCalBtn,
            14,14,0,0,0,0); 
    
    o.setClasses("", "", "", "iconcheckhover");
    
    bobj.fillIn(o, kwArgs);

    // Update instance with member functions
    o.margin = 0;
    o.oldInit = o.init;

    MochiKit.Base.update(o, bobj.crv.params.CalendarButton);
    o.clickCB = bobj.bindFunctionToObject(o.onClick, o);

    return o;
};

bobj.crv.params.CalendarButton = {
    onClick : function() {
        var calendar = bobj.crv.Calendar.getInstance ();
        var date = bobj.external.date.getDateFromFormat (this.getValueCB (), this.calendarProperties.displayValueFormat);
        var connect = MochiKit.Signal.connect;
        if (date)
            calendar.setDate (date);

        this.calendarSignals = {
            okSignal : connect (calendar, calendar.Signals.OK_CLICK, this, 'onClickCalendarOKButton'),
            cancelSignal : connect (calendar, calendar.Signals.CANCEL_CLICK, this, 'onClickCalendarCancelButton'),
            hideSignal : connect (calendar, calendar.Signals.ON_HIDE, this, 'onHideCalendar')
        };

        var absPos = getPosScrolled (this.layer);
        var x = absPos.x + this.getWidth ();
        var y = absPos.y + this.getHeight () + 1;

        calendar.setShowTime (this.calendarProperties.isTimeShown);
        calendar.show (true, x, y);
    },
    
    onClickCalendarOKButton : function(date) {
        var strValue = bobj.external.date.formatDate (date, this.calendarProperties.displayValueFormat);
        this.setValueCB (strValue);
    },
    
    onClickCalendarCancelButton : function() {
        this.clearCalendarSignals ();
    },
    
    clearCalendarSignals : function() {
        for ( var identName in this.calendarSignals) {
            bobj.crv.SignalDisposer.dispose(this.calendarSignals[identName], true);
            this.calendarSignals[identName] = null;
        }
    },
    
    onHideCalendar : function() {
        this.clearCalendarSignals ();
        if (this.layer.focus)
            this.layer.focus ();
    },
    
    init : function() {
        this.oldInit ();
        this.layer.onfocus = IconWidget_realOverCB;
        this.layer.onblur = IconWidget_realOutCB;
    },
    
    getHTML : function() {
        var imgCode;
        var h = bobj.html;
        if (this.src) {
            imgCode = h.DIV( {
                style : {
                    overflow : 'hidden',
                    height : '14px',
                    position : 'relative',
                    top : '2px',
                    width : this.w + 'px'
                }
            }, simpleImgOffset(this.src, this.w, this.h, this.dx, this.dy, 'IconImg_' + this.id, null, this.alt,
                    'cursor:' + _hand), this.extraHTML);
        } else {
            imgCode = h.DIV( {
                'class' : 'iconText',
                'style' : {
                    width : '1px',
                    height : (this.h + this.border) + 'px'
                }
            });
        }

        var divStyle = {
            margin : this.margin + 'px',
            padding : '1px'
        };

        if (this.width) {
            divStyle.width = this.width + 'px';
        }
        if (!this.disp) {
            divStyle.display = 'none';
        }

        return h.DIV( {
            style : divStyle,
            id : this.id,
            'class' : this.nocheckClass
        }, (this.clickCB && _ie) ? lnk(imgCode, null, null, null, ' tabIndex="-1"') : imgCode);
    }
};
