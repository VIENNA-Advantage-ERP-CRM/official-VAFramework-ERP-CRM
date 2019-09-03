/*
================================================================================
ParameterInfoRow

Internal class for use by ParameterUI. Draws a UI for a single info row. 
This row is always displayed as last child of parameterUI
================================================================================
 */

bobj.crv.params.ParameterInfoRow = function(parentId) {
    this.layer = null;
    this.parentId = parentId;
    this.id = bobj.uniqueId();
};

bobj.crv.params.ParameterInfoRow.prototype = {
    setTabDisabled : function(dis) {
        if (this.layer) {
            bobj.disableTabbingKey(this.layer, dis)
        }
    },

    init : function() {
        var parent = getLayer(this.parentId);
        if (parent) {
            append2(parent, this.getHTML());
            this.layer = getLayer(this.id);
        }
        
        if(this.layer) {
            MochiKit.Signal.connect(this.layer, "onclick", this, '_onClick');
            MochiKit.Signal.connect(this.layer, "onkeydown", this, '_onKeyDown');
        }
        
    },

    getHTML : function() {
        return bobj.html.DIV( {
            'class' :'parameterInfoRow',
            id :this.id,
            "tabIndex" : "0"
        });
    },

    setText : function(text) {
        if (!this.layer)
            this.init();

        this.layer.innerHTML = text;
    },

    setVisible : function(isVisible) {
        if (isVisible) {
            if (!this.layer)
                this.init();

            this.shiftToLastRow();
            this.layer.style.display = "block";
        } else {
            if (this.layer)
                this.layer.style.display = "none";
        }
    },

    /**
     * ParamterInfoRow must always be displayed after all ParmeterValueRows
     * @return
     */
    shiftToLastRow : function() {
        var parentNode = getLayer(this.parentId);
        if (this.layer && parentNode) {
            parentNode.removeChild(this.layer);
            parentNode.appendChild(this.layer);
        }
    },
    
    _onClick : function (ev) {
        MochiKit.Signal.signal(this, "switch");
        ev.stop();
    },
    
    _onKeyDown : function (ev) {
        if(ev && ev.key() && ev.key().code == 13) {
            MochiKit.Signal.signal(this, "switch");
            ev.stop();
        }
    }
};
