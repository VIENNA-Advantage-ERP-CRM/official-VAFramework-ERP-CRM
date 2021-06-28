
/**
 * OptionalParameterValueRow extends ParamterValueRow. This class displays a info message for 'undefined' value and
 * clears the info message when its control get focus
 */
bobj.crv.params.newOptionalParameterValueRow = function(kwArgs) {
    kwArgs = MochiKit.Base.update( {
        noValueDisplayText :'',
        isEmptyStringNoValue : true,
        clearValuesCB : null
    }, kwArgs);

    var o = bobj.crv.params.newParameterValueRow(kwArgs);
    bobj.extendClass(o, bobj.crv.params.OptionalParameterValueRow, bobj.crv.params.ParameterValueRow);
    
    if (o.canChangeOnPanel) {
        o._clearValueButton = newIconWidget( 
                o.id + '_clearBtn',        
                bobj.crvUri('images/clear_x.gif'),    
                o.clearValuesCB, 
                null,   //text
                L_bobj_crv_ParamsClearValues,//tooltip,   
                10, 10, 2, 2);   //width, height, dx, dy, disDx, disDy, cancelEventPropagation
        
        o._clearValueButton.setClasses("", "iconcheck", "", "iconcheckhover");
        o._clearValueButton.margin = 2;
    }
    
    return o;
};

bobj.crv.params.OptionalParameterValueRow = {
    /**
     * Returns arguments required for creating new ValueWidget(TextField, TextCombo, RangeField) 
     */
    getNewValueWidgetArgs : function() {
        var args = this.superClass.getNewValueWidgetArgs();

        if (this.value == undefined) { 
            //If value is undefined, the valueWidget should display noValueDisplayText
            args.cleanValue = this.noValueDisplayText;
            args.foreColor = bobj.Colors.GRAY;
        }

        return args;
    },
    
    init : function () {
        this.superClass.init();
        if(this._clearValueButton) {
            this._clearValueButton.init();
        }

        this.updateUI ();
    },
        
    getHTML : function () {
        var rowHTML = this.superClass.getHTML();
        if (this.canChangeOnPanel) {
            var h = bobj.html;
            rowHTML = h.DIV({style : {position : 'relative'}}, rowHTML , h.DIV({'class' : 'clearValueBtnCtn'},this._clearValueButton.getHTML()));
        }
        
        return rowHTML;
    },

    /**
     * When receiving focus if noValueDisplayText is being displayed (happens when current value is undefined and param is editable on panel), 
     * it must be cleared so user can type something
     */
    onFocus : function() {
        this.superClass.onFocus();
        if (this.canChangeOnPanel) {
            if(this.value == undefined) 
                this._valueWidget.setValue('');
        }
    },

    onBlur : function() {
        this.superClass.onBlur();
        
        if (this.canChangeOnPanel) {
            if (this.value == undefined)
                this._valueWidget.setValue(this.noValueDisplayText);
        }
    },
            
    /**
     * When user changes value, value stored in OptionalParmeterUI gets updated and 
     */
    _onChange : function() {
        this.value = this._valueWidget.getValue();
        if(this.isEmptyStringNoValue && this.value != null && this.value.length == 0)
            this.value = undefined;
        
        this.updateUI();
        if (this.changeCB)
            this.changeCB();    
        
    },
    
    updateUI: function() {
        if (this._valueWidget) {
            if(this.isReadOnlyParam) {
                this._valueWidget.setForeColor(bobj.Colors.GRAY);
                this._valueWidget.setTextItalic(false);
            }
            else {
                if(this.value == undefined) {
                    this._valueWidget.setForeColor(bobj.Colors.GRAY); 
                    if(this._clearValueButton)
                        this._clearValueButton.setDisplay(false);
                }
                else {
                    this._valueWidget.setForeColor(bobj.Colors.BLACK);
                    if(this._clearValueButton)
                        this._clearValueButton.setDisplay(true);
                }
            }
        }
    },
    
    getValueWidgetDisplayValue: function(value) {
        return (value == undefined) ? this.noValueDisplayText : value;
    },
    
    setValue : function(value) {
        this.value = value;
        this._valueWidget.setValue(this.getValueWidgetDisplayValue(value));
        this.updateUI();
        
        this.setWarning(null); //Since value is set through parameterController it cannot contain any errors
    },
    
    reset : function(value) {
        this.superClass.reset(value);
        if(this._valueWidget) {
            this._valueWidget.reset(this.getValueWidgetDisplayValue(value));
        }
        
        this.updateUI();
    }
};
