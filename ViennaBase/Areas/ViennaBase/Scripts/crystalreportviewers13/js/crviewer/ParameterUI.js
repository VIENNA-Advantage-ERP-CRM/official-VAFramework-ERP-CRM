
/* Copyright (c) Business Objects 2006. All rights reserved. */

/*
================================================================================
ParameterUI

Widget for displaying and editing parameter values. Contains one or many
ParameterValueRows and, optionally, UI that allows rows to be added.
================================================================================
*/

bobj.crv.params.newParameterUI = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        canChangeOnPanel: false,
        allowCustom: false,
        isPassword : false,
        isReadOnlyParam: true,
        allowRange : false,
        values: [],
        defaultValues: null,
        width: '200px',
        changeValueCB: null,
        enterPressCB: null,
        openAdvDialogCB: null,
        maxNumParameterDefaultValues: 200,
        tooltip : null,
        calendarProperties : {displayValueFormat : '' , isTimeShown : false, hasButton : false, iconUrl : ''},
        maxNumValuesDisplayed : 7,
        canOpenAdvDialog : false
    }, kwArgs);
    
    var o = newWidget(kwArgs.id);
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    o.displayAllValues = false;
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.params.ParameterUI);
    
    o._createMenu();
    o._rows = [];
    o._infoRow = new bobj.crv.params.ParameterInfoRow(o.id);
    return o;
};

bobj.crv.params.ParameterUI = {
    /** 
     *  Creates single menubar for all parameter value rows of current param UI
     */
    _createMenu : function() {
        var dvLength = this.defaultValues.length;
        if (dvLength > 0) {
            var kwArgs = {
                originalValues : this.defaultValues
            };

            if (dvLength == this.maxNumParameterDefaultValues) {
                kwArgs.originalValues[this.maxNumParameterDefaultValues] = L_bobj_crv_ParamsMaxNumDefaultValues;
                MochiKit.Base.update (kwArgs, {
                    openAdvDialogCB : this.openAdvDialogCB,
                    maxNumParameterDefaultValues : this.maxNumParameterDefaultValues
                });
            }

            this._defaultValuesMenu = bobj.crv.params.newScrollMenuWidget (kwArgs);
        } else {
            this._defaultValuesMenu = null;
        }
    },
    
    setFocusOnRow : function(rowIndex) {
        var row = this._rows[rowIndex];
        if (row)
            row.focus ();
    },
    
    /*
     * Disables tabbing if dis is true
     */
    setTabDisabled : function(dis) {
        for(var i = 0, len = this._rows.length; i < len; i++) {
            this._rows[i].setTabDisabled(dis);
        }
        
        this._infoRow.setTabDisabled(dis);
    },
    
    init : function() {
        Widget_init.call (this);

        var rows = this._rows;
        for ( var i = 0, len = rows.length; i < len; ++i) {
            rows[i].init ();
        }
        
        MochiKit.Signal.connect(this._infoRow, "switch", this, '_onSwitchDisplayAllValues');
        this.refreshUI ();
    },
    
    /**
     * Processes actions triggered by clicks on "x more values" or "collapse" button displayed in inforow
     */
    _onSwitchDisplayAllValues: function() {
        this.displayAllValues = !this.displayAllValues;
        var TIME_INTERVAL = 10; /* 10 msec or 100 actions per second */
        var timerIndex = 0;
        
        if(this.displayAllValues) {
            if (this.values.length > this._rows.length) {
                for(var i = this._rows.length, l = this.values.length; i < l; i++) {
                    var addRow = function(paramUI, value) {
                        return function() { return paramUI._addRow(value); };
                    };
                    
                    timerIndex++;
                    setTimeout(addRow(this, this.values[i]), TIME_INTERVAL * timerIndex);
                }
            }
        }
        else {
            if(this._rows.length > this.maxNumValuesDisplayed) {
                for(var i = this._rows.length -1; i >= this.maxNumValuesDisplayed; i--) {
                    var deleteRow = function(paramUI, rowIndex) {
                        return function() { return paramUI.deleteValue(rowIndex); };
                    };
                    
                    timerIndex++;
                    setTimeout(deleteRow(this, i), TIME_INTERVAL * timerIndex);
                }
            }
        }
        
        var signalResize =  function(paramUI) {
            return function() {MochiKit.Signal.signal(paramUI, 'ParameterUIResized'); };
        };
        
        setTimeout(signalResize(this), TIME_INTERVAL * timerIndex);
    },
    
    getHTML : function() {
        var rowsHtml = '';

        var values = this.values;
        var rows = this._rows;

        var rowsCount = Math.min (values.length, this.maxNumValuesDisplayed);
        for ( var i = 0; i < rowsCount; ++i) {
            rows.push (this._getRow (values[i]));
            rowsHtml += rows[i].getHTML ();
        }

        return bobj.html.DIV ( {
            id : this.id,
            style : {
                width : bobj.unitValue (this.width),
                'padding-left' : '20px'
            }
        }, rowsHtml);
    },
    
    _getNewValueRowArgs : function(value) {
        return {
            value : value,
            defaultValues : this.defaultValues,
            width : this.width,
            isReadOnlyParam : this.isReadOnlyParam,
            canChangeOnPanel : this.canChangeOnPanel,
            allowCustom : this.allowCustom,
            isPassword : this.isPassword,
            calendarProperties : this.calendarProperties,
            defaultValuesMenu : this._defaultValuesMenu,
            tooltip : this.tooltip,
            isRangeValue : this.allowRange,
            canOpenAdvDialog : this.canOpenAdvDialog
        };
    },
    
    _getNewValueRowConstructor : function() {
        return bobj.crv.params.newParameterValueRow;
    },
    
    _getRow : function(value) {
        var row = this._getNewValueRowConstructor()(this._getNewValueRowArgs(value));
        var bind = MochiKit.Base.bind;
        
        row.changeCB = bind(this._onChangeValue, this, row); 
        row.enterCB = bind(this._onEnterValue, this, row);
        
        return row;
    },
    
    _addRow : function(value) {
        var row = this._getRow (value);
        this._rows.push (row);
        append (this.layer, row.getHTML ());

        row.init ();
        
        this.refreshUI ();
        return row;
    },

    _onChangeValue : function(row) {
        if (this.changeValueCB) {
            this.changeValueCB (this._getRowIndex (row), row.getValue ());
        }
    },

    _onEnterValue : function(row) {
        if (this.enterPressCB) {
            this.enterPressCB (this._getRowIndex (row));
        }
    },

    _getRowIndex : function(row) {
        if (row) {
            var rows = this._rows;
            for ( var i = 0, len = rows.length; i < len; ++i) {
                if (rows[i] === row) {
                    return i;
                }
            }
        }
        return -1;
    },

    getNumValues : function() {
        return this._rows.length;
    },


    refreshUI : function() {
        if (this.allowRange)
            this.alignRangeRows ();
        
        var displayInfoRow = false;
        var infoRowText = "";

        if (this.values.length > this.maxNumValuesDisplayed) {
            displayInfoRow = true;
            
            if(this.displayAllValues) 
                infoRowText = L_bobj_crv_Collapse;
            else {
                var hiddenValuesCount = this.values.length - this.maxNumValuesDisplayed;
                infoRowText = (hiddenValuesCount == 1) ? L_bobj_crv_ParamsMoreValue : L_bobj_crv_ParamsMoreValues;               
                infoRowText = infoRowText.replace ("%1", hiddenValuesCount);
            }
        } 

        this._infoRow.setText (infoRowText);
        this._infoRow.setVisible (displayInfoRow);
    },
    
    getValueAt : function(index) {
        var row = this._rows[index];
        if (row) {
            return row.getValue ();
        }
        return null;
    },
    
    getValues : function() {
        var values = [];
        for ( var i = 0, len = this._rows.length; i < len; ++i) {
            values.push (this._rows[i].getValue ());
        }
        return values;
    },
    
    setValueAt : function(index, value) {
        var row = this._rows[index];
        if (row) {
            row.setValue (value);
        }

        this.refreshUI ();
    },
    
    resetValues : function(values) {
        if (!values) {
            return;
        }

        this.values = values;
        var valuesLen = values.length;
        var rowsLen = this._rows.length;

        //Resets value
        for ( var i = 0; i < valuesLen && i < rowsLen; ++i) {
            this._rows[i].reset (values[i]);
        }

        //removes newly added values that are not commited
        if (rowsLen > valuesLen) {
            for ( var i = rowsLen - 1; i >= valuesLen; --i) {
                // delete from the end to minimize calls to setBgColor
                this.deleteValue (i);
            }
        }
        //re-adds removed values
        else if (valuesLen > rowsLen) {
            for ( var i = rowsLen; i < valuesLen && (this.displayAllValues || i < this.maxNumValuesDisplayed); ++i) {
                var row = this._addRow (values[i]);

            }
        }
        
        MochiKit.Signal.signal(this, 'ParameterUIResized');
        this.refreshUI ();
    },
    
    alignRangeRows : function() {
        if (!this.allowRange)
            return;

        var lowerBoundWidth = 0;
        for ( var i = 0, l = this._rows.length; i < l; i++) {
            var row = this._rows[i];
            var rangeField = row._valueWidget;
            lowerBoundWidth = Math.max (lowerBoundWidth, rangeField.getLowerBoundValueWidth ());
        }

        for ( var i = 0, l = this._rows.length; i < l; i++) {
            var row = this._rows[i];
            var rangeField = row._valueWidget;
            rangeField.setLowerBoundValueWidth (lowerBoundWidth);
        }
    },
    
    setValues : function(values) {
        if (!values) 
            return;

        this.values = values;
        var valuesLen = values.length;
        var rowsLen = this._rows.length;

        for ( var i = 0; i < valuesLen && i < rowsLen; ++i) {
            this._rows[i].setValue (values[i]);
        }

        if (rowsLen > valuesLen) {
            for ( var i = rowsLen - 1; i >= valuesLen; --i) {
                // delete from the end to minimize calls to setBgColor
                this.deleteValue (i);
            }
        } else if (valuesLen > rowsLen) {
            for ( var i = rowsLen; i < valuesLen && (this.displayAllValues || i < this.maxNumValuesDisplayed); ++i) {
                this._addRow (values[i]);
            }
        }

        MochiKit.Signal.signal(this, 'ParameterUIResized');
        this.refreshUI ();
    },
    
    setCleanValue : function(index, value) {
        var row = this._rows[index];
        if (row)
            row.setCleanValue (value);
    },
    
    deleteValue : function(index) {
        if (index >= 0 && index < this._rows.length) {
            var row = this._rows[index];
            row.layer.parentNode.removeChild (row.layer);
            _widgets[row.widx] = null;

            this._rows.splice (index, 1);
            var rowsLen = this._rows.length;
        }

        this.refreshUI ();
    },
    
    setWarning : function(index, warning) {
        var row = this._rows[index];
        if (row) {
            row.setWarning (warning);
        }
    },
    
    getWarning : function(index) {
        var row = this._rows[index];
        if (row) 
            return row.getWarning ();
        
        return null;
    },
    
    resize : function(w) {
        if (w !== null) {
            this.width = w;
            if (this.layer) {
                bobj.setOuterSize (this.layer, w);
            }
        }
    }
};
