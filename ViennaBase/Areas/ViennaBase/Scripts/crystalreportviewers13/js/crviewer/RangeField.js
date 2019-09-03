
/*
 * ================================================================================
 * Range Field
 * 
 * ================================================================================
 */

bobj.crv.params.newRangeField = function(kwArgs) {
    return new bobj.crv.params.RangeField(kwArgs);
};

bobj.crv.params.RangeField = function(kwArgs) {
    kwArgs = MochiKit.Base.update( {
        id : bobj.uniqueId(),
        cleanValue : {}, 
        foreColor : 'black',
        isTextItalic: false,
        tooltip : ''
    }, kwArgs);

    this.widgetType = 'RangeField';
    this.value = kwArgs.cleanValue;

    // Update instance with constructor arguments
    bobj.fillIn(this, kwArgs);
};

bobj.crv.params.RangeField.prototype = {
    setTabDisabled : function(dis) {
        if(this.layer) {
            var tds = this.layer.getElementsByTagName("TD");
            for(var i = 0, len = tds.length; i < len ; i++) {
                bobj.disableTabbingKey(tds[i], dis);
            }
        }
    },
    
    setTooltip  : function(text) {
        if(this.layer) {
            this.layer.title = text;
        }
    },
    
    setForeColor : function(color) {
        if(this.layer) {
            this.layer.style.color = color;
        }
    },
    
    setTextItalic : function(isItalic) {
        if(this.layer) {
            this.layer.style.fontStyle = isItalic ? 'italic' : '';
        }
    },

    /**
     * Generates HTML for either lower or upper bound value
     * @param value - the value of upper/lower bound
     * @param isRightAligned - a flag for indicating whether left align or right align the generated HTML
     * @return
     */
    getValueHTML : function(value, isRightAligned) {
        value = value ? value : '&nbsp;';
        var style = {
            'text-align' : isRightAligned ? 'right' : 'left'
        };

        return bobj.html.TD( {
            style : style,
            tabIndex : 0
        }, bobj.html.SPAN(null, value));

    },
    
    isValueValidRange: function() {
        return this.value != null && this.value.lowerBound != null && this.value.upperBound != null;
    },

    getHTML : function() {
        var TR = "";
        var h =  bobj.html;
        
        if(this.isValueValidRange()) {
            TR = h.TR(null, this.getValueHTML(this.getValue().lowerBound.value, true), this
                    .getMiddleImageHTML(), this.getValueHTML(this.getValue().upperBound.value, false))
        }
        else {
            TR = h.TR(null, this.getValueHTML(this.getValue(), false))
        }
        
        return h.TABLE( {
            id : this.id,
            'class' : 'iactRangeFieldTable',
            title: this.tooltip,
            style : {color : this.foreColor, "font-style" : this.isTextItalic ? 'italic' :''}
        }, TR);
    },
    
    /**
     * 
     * @return HTML for Range type (INCLUSIVE / EXCLUSIVE / UNBOUND)
     */
    getMiddleImageHTML : function() {
        var lowerImageSrc = "";
        var upperImageSrc = "";
        var lineSrc = "images/line.gif";

        switch (this.getValue().lowerBound.type) {
        case bobj.crv.params.RangeBoundTypes.EXCLUSIVE:
            lowerImageSrc = "images/hollowCircle.gif";
            break;
        case bobj.crv.params.RangeBoundTypes.INCLUSIVE:
            lowerImageSrc = "images/filledCircle.gif";
            break;
        case bobj.crv.params.RangeBoundTypes.UNBOUNDED:
            lowerImageSrc = "images/leftTriangle.gif";
            break;
        }

        switch (this.getValue().upperBound.type) {
        case bobj.crv.params.RangeBoundTypes.EXCLUSIVE:
            upperImageSrc = "images/hollowCircle.gif";
            break;
        case bobj.crv.params.RangeBoundTypes.INCLUSIVE:
            upperImageSrc = "images/filledCircle.gif";
            break;
        case bobj.crv.params.RangeBoundTypes.UNBOUNDED:
            upperImageSrc = "images/rightTriangle.gif";
            break;
        }

        var TDStyle = {
            'vertical-align' : 'middle'
        };

        var IMG = bobj.html.IMG;
        return bobj.html.TD( {
            style : TDStyle
        }, IMG( {
            src : bobj.crvUri(lowerImageSrc)
        }), IMG( {
            src : bobj.crvUri(lineSrc)
        }), IMG( {
            src : bobj.crvUri(upperImageSrc)
        }));
    },

    /**
     * Initializes the widget
     */
    init : function() {
        this.layer = getLayer(this.id);
    },

    /**
     * 
     * @return the evaluated width of lower bound TD
     */
    getLowerBoundValueWidth : function() {
        if(!this.isValueValidRange())
            return 0;
        
        var value = this.getValue().lowerBound.value;

        var fontFamily = MochiKit.Style.computedStyle(this.layer, 'fontFamily');
        var fontSize = MochiKit.Style.computedStyle(this.layer, 'fontSize');

        if (!fontFamily)
            fontFamily = "arial , sans-serif";

        if (!fontSize)
            fontSize = "12px";

        return bobj.getStringWidth(value, fontFamily, fontSize);
    },

    getLowerBoundTD : function() {
        if (this.layer)
            return this.layer.getElementsByTagName("TD")[0];

        return null;
    },

    setLowerBoundValueWidth : function(w) {
        if (this.getLowerBoundTD())
            this.getLowerBoundTD().style.width = w + "px";
    },

    reset : function(value) {
        this.value = value;
        this.cleanValue = value;
        this.updateUI();
    },

    updateUI : function() {
        var parent = this.layer.parentNode;
        MochiKit.DOM.removeElement(this.layer);
        append2(parent, this.getHTML());
        this.init();
    },

    setValue : function(rangeValue) {
        this.value = rangeValue;
        this.updateUI();
    },

    setCleanValue : function(rangeValue) {
        this.cleanValue = rangeValue;
    },

    getValue : function() {
        return this.value;
    }
};