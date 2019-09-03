/*
 * ================================================================================
 * TextField
 * 
 * Internal class. Text box that renders correctly in a Parameter UI
 * ================================================================================
 */

/**
 * Constructor. TextField extends TextFieldWidget from the dhtmllib.
 */
bobj.crv.params.newTextField = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        cleanValue: '',
        width: '100%',
        maxChar: null,
        tooltip: null,
        disabled: false,
        editable: true, 
        password: false,
        focusCB: null,
        blurCB: null,
        changeCB: null,
        keyUpCB: null,
        enterCB: null,
        foreColor : 'black',
        isTextItalic : false,
        canOpenAdvDialog : false
    }, kwArgs);
    
    var o = newTextFieldWidget(
        kwArgs.id,
        kwArgs.changeCB, 
        kwArgs.maxChar,
        kwArgs.keyUpCB, 
        kwArgs.enterCB, 
        true, //nomargin
        kwArgs.tooltip,
        null, //width
        kwArgs.focusCB, 
        kwArgs.blurCB);
        
    o.widgetType = 'TextField';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    o.disabled = kwArgs.disabled;
    o.width = kwArgs.width;
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.params.TextField);
    
    if (kwArgs.cleanValue) {
        o.setValue(kwArgs.cleanValue);    
    }
    
    return o;
};

bobj.crv.params.TextField = {
    setForeColor : function(color) {
        this.foreColor = color;
        if (this.css)
            this.css.color = color;
    },
    
    setTextItalic : function(isTextItalic) {
        this.isTextItalic = isTextItalic;
        if (this.css)
            this.css.fontStyle = isTextItalic ? 'italic' : '';
    },
    
    setTabDisabled : function(dis) {
        bobj.disableTabbingKey (this.layer, dis);
    },
    
    eraseHelpTxt: MochiKit.Base.noop,
    
    getHTML : function() {
        var style = {
            width : bobj.unitValue (this.width)
        };

        var isIE = MochiKit.Base.isIE ();
        var className = 'iactTextField';

        var attributes = {
            type : this.password ? 'password' : 'text',
            name : this.id,
            id : this.id,
            maxLength : this.maxChar,
            style : style,
            'class' : className,
            oncontextmenu : "event.cancelBubble=true;return true",
            onfocus : "TextFieldWidget_focus(this)",
            onblur : "TextFieldWidget_blur(this)",
            onchange : "TextFieldWidget_changeCB(event, this)",
            onkeydown : "return TextFieldWidget_keyDownCB(event, this);",
            onkeyup : "return TextFieldWidget_keyUpCB(event, this);",
            onkeypress : "return TextFieldWidget_keyPressCB(event, this);",
            ondragstart : "event.cancelBubble=true; return true;",
            onselectstart : "event.cancelBubble=true; return true;"
        };

        if (this.disabled) {
            attributes.disabled = "disabled";
        }

        if (this.isTextItalic) {
            style["font-style"] = "italic";
        }

        style.color = this.foreColor;

        if (!this.editable) {
            attributes.readonly = "readonly";
            
            if (this.canOpenAdvDialog) {
                style.cursor = "pointer";
            }
            else {
                style.cursor = "default";
            }
        }

        if (this.tooltip) {
            attributes.title = this.tooltip.replace(/"/g, "&quot;");
        }

        return bobj.html.INPUT (attributes);
    },
    
    /**
     * Resets textField object. Sets clean value, current value
     */
    reset : function(value) {
        this.value = value;
        this.cleanValue = value;
        this.setValue (value);
    },
    
    setValue : function(value) {
        TextFieldWidget_setValue.call (this, value);
    },
    
    setCleanValue : function(value) {
        this.cleanValue = value;
    }
};

