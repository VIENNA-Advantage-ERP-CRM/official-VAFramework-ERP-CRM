/* Copyright (c) Business Objects 2006. All rights reserved. */

/* TODO Dave
 * - Use delete button to set value to empty string if it can't be removed
 */

/**
 * ParameterController creates and controls the widgets that show and edit
 * parameter values in the interactive parameter panel.
 *
 * @param panel [ParameterPanel] Panel that will contain the parameter widgets
 * @param viewerCtrl [ViewerListener] Controller for the entire viewer 
 * @param paramOpts [Object] Parameter options, such as dateTimeFormat 
 */
bobj.crv.params.ParameterController = function(panel, viewerCtrl, paramOpts) {
    this._panel = panel;
    this._viewerCtrl = viewerCtrl;
    this._paramOpts = paramOpts;
    this._paramList = null;
    this._unusedParamList = null;

    var applyClickCB = bobj.bindFunctionToObject(this._onClickTbApplyButton,this);
    var resetClickCB = bobj.bindFunctionToObject(this._onClickTbResetButton,this);
    this._panel.setToolbarCallBacks(applyClickCB,resetClickCB);
};

bobj.crv.params.ParameterController.prototype = {
    
    /**
     * Set the list of parameters to display in the parameter panel.
     *
     * @param paramList [Array] List of bobj.crv.params.Parameter instances
     */
    setParameters: function(paramList, finishCB) {
        var Type = bobj.crv.params.DataTypes;
        var map = MochiKit.Base.map;
        var bind = MochiKit.Base.bind;
        
        this._deleteWidgets();
        var TIMEER_INTERVAL = 50;
        
        this._paramList = paramList;
        
        for (var i = 0; i < paramList.length; ++i) {
            var param = paramList[i];
            
            var getDisplayText = bind(this._getDisplayText, this, param);
            var getDefaultValue = bind(this._getDefaultValue, this, param.valueDataType,param.defaultDisplayType);
            var panelCanChangeValues = this._canPanelChangeValues(param);
            var minMaxText = this._getMinMaxText(param.valueDataType, param.minValue, param.maxValue);
            var openAdvDialogCB = bind(this._onClickTbAdvButton, this, param);
            var clearValuesCB = bind(this.clearParameterValues, this, param);
            var isReadOnly = !param.isEditable;
            var paramValue = param.getValue();
            var isValueOptional = param.isOptionalPrompt || param.allowNullValue;
            var calendarProperties = {
                hasButton : param.allowCustomValue && panelCanChangeValues && (param.valueDataType === Type.DATE || param.valueDataType === Type.DATE_TIME),
                isTimeShown : param.valueDataType === Type.DATE_TIME,
                displayValueFormat : this._getDateTimeFormat(param.valueDataType),
                iconUrl : bobj.crvUri('images/calendar.gif')
            };
            
            if(isValueOptional)
                paramValue = this.convertOptionalParameterValue(paramValue);
                                    
            var paramWidgetConstructor = isValueOptional ? bobj.crv.params.newOptionalParameterUI :  bobj.crv.params.newParameterUI;
            var canOpenAdvDialog = param.isEditable && (param.allowRangeValue || param.allowMultiValue || param.isDCP() || param.editMask);
            
            var newParamWidgetArgs = {
                values :map(getDisplayText, paramValue),
                canChangeOnPanel :panelCanChangeValues,
                allowCustom :param.allowCustomValue,
                allowRange :param.allowRangeValue,
                canAddValues :param.allowMultiValue && panelCanChangeValues,
                isReadOnlyParam :isReadOnly,
                isPassword :param.isPassword(),
                defaultValues :map(getDefaultValue, param.defaultValues || []),
                openAdvDialogCB : openAdvDialogCB,
                maxNumParameterDefaultValues :this._paramOpts.maxNumParameterDefaultValues,
                tooltip :param.getTitle(),
                calendarProperties :calendarProperties,
                clearValuesCB : clearValuesCB,
                canOpenAdvDialog : canOpenAdvDialog
            };
            
            if(isValueOptional)  {
                newParamWidgetArgs.noValueDisplayText = this._getNoValueDisplayText(param.valueDataType, panelCanChangeValues)
                newParamWidgetArgs.isEmptyStringNoValue = param.valueDataType != Type.STRING;
            }
            
            var paramWidget = paramWidgetConstructor(newParamWidgetArgs);    
            this._observeParamWidget(paramWidget);
            
            var addParameter = function(panel, param, paramWidget, openAdvDialogCB, isValueOptional, panelCanChangeValues, clearValuesCB, isLast, finishCB) {
                return function() {
                    panel.addParameter({paramUI: paramWidget,
                        label : param.getTitle(),
                        name : param.getName(),
                        isDataFetching: param.isDataFetching,
                        openAdvCB: openAdvDialogCB
                    });
                    
                    if(isLast && finishCB)
                        finishCB();
                };
            };
            
            setTimeout(addParameter(this._panel, param, paramWidget, canOpenAdvDialog ? openAdvDialogCB : null, isValueOptional, panelCanChangeValues, clearValuesCB, i == paramList.length -1, finishCB), TIMEER_INTERVAL * i);
        }
        
        if(paramList) {
            var updatePanel = function (panel) { 
                return function() { 
                    //Parameter panel has to be resized in case scroll bars appear
                    panel.resize();
            
                    // Force the panel to be redrawn to fix a problem
                    // in IE if the prompt text is too long to be displayed.
                    if (MochiKit.Base.isIE ()){
                        var curDisplay = panel.isDisplayed();
                        panel.setDisplay(!curDisplay);
                        panel.setDisplay(curDisplay);
                    }
                };
            };
            setTimeout(updatePanel(this._panel), TIMEER_INTERVAL * paramList.length );
        }
        
        //Panel should be disabled when current view is not main report view
        this._panel.setDisabled(!this._isCurrentViewMainReport());  
        //Disables all buttons on param toolbar -> necessary when refreshing report via ajax request
        this._panel.setApplyButtonEnabled(false);
        this._panel.setResetButtonEnabled(false); 
    },
    
    setUnusedParameters: function(unusedParamList) {
        this._unusedParamList = unusedParamList;
    },
    
    convertOptionalParameterValue: function(values) {
        if(values === undefined || values.length === undefined)
            return [];

        var newValues = bobj.cloneArray (values);
        if(newValues.length == 0 || newValues[0] == null)
            newValues[0] = undefined;
        
        return newValues;
    },
    
    /**
     * Get the list of params that are displayed in the panel
     *
     * @return [Array] List of bobj.crv.params.Parameter instances or null
     */
    getParameters: function() {
        return this._paramList;    
    },
    
    /**
     * When view changes, ParameterController must disable panel if current view is not main report view
     * @return
     */
    onChangeView: function() {
        this._panel.setDisabled(!this._isCurrentViewMainReport());        
    },
    
    /**
     * 
     * @return true if panel is edittable
     */
    _isPanelEditable: function() {
        return this._isCurrentViewMainReport();
    },
    
    /**
     * @return true if current view is the main report view
     */
    _isCurrentViewMainReport: function() {
    	    var currentView = this._viewerCtrl.getCurrentView ();
    	    return currentView && currentView.isMainReport();
    },
    
    findParamIndexByName : function( paramName) {
        for (var i = 0; i < this._paramList.length; ++i) {
            if (this._paramList[i].paramName === paramName) {
                return i;
            }
        }
        
        return -1;
    },
    
    findUnusedParamIndexByName : function(paramName) {
        for (var i = 0; i < this._unusedParamList.length; ++i) {
            if (this._unusedParamList[i].paramName === paramName) {
                return i;
            }
        }
        
        return -1;
    },
    
    findParamByName : function(paramName) {
        var index = this.findParamIndexByName(paramName);
        if(index == -1)
            return null;
        else
            return this._paramList[index];
    },
    
    /**
     * Sets the current values of the param with the same name as the one passed
     * in and updates the UI.
     *
     * @param [Parameter] 
     */
    updateParameter: function(paramName, paramValue) {
        if (paramName) {
            var index = this.findParamIndexByName(paramName);
            
            if (index != -1) {
                var param = this._paramList[index];
                param.setValue(paramValue);
                
                var paramUI = this._panel.getParameter(index);
                var getDisplayText = MochiKit.Base.bind(this._getDisplayText, this, this._paramList[index]);
                
                if (param.isOptionalPrompt || param.allowNullValue) 
                    paramValue = this.convertOptionalParameterValue(paramValue);
                
                 paramUI.setValues( MochiKit.Base.map(getDisplayText, paramValue) );
                 
                 var paramTab = this._panel.getParameterTab(index);
                 if(paramTab)
                     paramTab.setDirty(true);
            }
            else {
                index = this.findUnusedParamIndexByName(paramName);
                
                if (index != -1) {
                    var param = this._unusedParamList[index];
                    param.setValue(paramValue);
                }
            }
        }
    },
    
    getFocusAdvButtonCB: function (paramName) {
        return MochiKit.Base.bind(this._focusAdvButton, this, paramName);
    },
    
    _focusAdvButton: function(paramName) {
        if (paramName) {
            var index = this.findParamIndexByName(paramName);
            
            if (index != -1) {
                 var paramTab = this._panel.getParameterTab(index);
                 
                 if(paramTab)
                     paramTab.focusAdvButton();
            }
        }
    },
        
    /** 
     * Checks whether a parameter can be edited directly in the panel. Params
     * that can't be edited in the panel are either read-only or must be
     * edited in the Advanced Dialog.
     *
     * @param param [Parameter]
     *
     * @return [bool] True if ParameterPanel is capable of changing the 
     *                parameter's values. False otherwise.
     */
    _canPanelChangeValues: function(param) {
        return param && param.isEditable && !param.allowRangeValue && !param.editMask && !param.isDCP() && !param.allowMultiValue;
    },
    
    /**
     * Private. Deletes the parameter widgets in the panel.
     */
    _deleteWidgets: function() {
        var paramUI = this._panel.getParameter(0);
        while (paramUI) { 
            delete _widgets[paramUI.widx];
            this._panel.removeParameter(0);
            paramUI = this._panel.getParameter(0);
        }
    },
        
    /**
     * Restores original state of parameter panel
     */
    resetParamPanel: function() {
        
    	// Resets value of all parameters in panel to values stored in view state
        for(var i = 0, iLen = this._paramList.length; i < iLen; i++) {
            var param = this._paramList[i];
            param.reset();
            
            var paramUI = this._findWidget(param);
            var getDisplayText = MochiKit.Base.bind(this._getDisplayText, this, param);
            
            var paramValue = param.getValue();
            if (param.isOptionalPrompt || param.allowNullValue)
                paramValue = this.convertOptionalParameterValue(paramValue);
            
            var values = MochiKit.Base.map(getDisplayText, paramValue); 
            paramUI.resetValues(values);
        }
    	
    	for(var i = 0, l = this._panel.getParameterCount(); i < l; i++) {
    	    var paramTab = this._panel.getParameterTab(i);
    	    if(paramTab)
    	        paramTab.setDirty(false);
    	}
        
        this._viewerCtrl.clearAdvancedPromptData ();
        
        //Disables all buttons on param toolbar
        this._panel.setApplyButtonEnabled(false);
        this._panel.setResetButtonEnabled(false); 
    },
    
    /**
     * Private. Compares 2 objects that have date like members (y, m, d, h, min, s, ms)
     * returns true if all members match, false otherwise.
     */
    _compareCustomDateObject: function(valA, valB) {
        if (valA.y != valB.y) {
            return false;
        }
        
        if (valA.m != valB.m) {
            return false;
        }

        if (valA.d != valB.d) {
            return false;
        }

        if (valA.h != valB.h) {
            return false;
        }

        if (valA.min != valB.min) {
            return false;
        }

        if (valA.s != valB.s) {
            return false;
        }

        if (valA.ms != valB.ms) {
            return false;
        }
        
        return true;
    },
    
    /**
     * Private. Erom doesn't give us NULL in all cases so we need to compare against the boundary values for each type.
     * type - the value type
     * minValue - the value to check
     * returns true if the minValue is NOT equal NULL or the MIN value for the given datatype.
     */
    _hasMinBound: function(type,minValue) {
        if (minValue == null) {
            return false;
        }
        
        var Type = bobj.crv.params.DataTypes;
        switch(type) {
            case Type.STRING:
                if (minValue == 0) {
                    return false;
                }
                return true;
            case Type.DATE:
            case Type.DATE_TIME:
                absoluteMin = {y:1753, m:0, d:1, h:0, min:0, s:0, ms:0};
                return !this._compareCustomDateObject (absoluteMin, minValue);
            case Type.TIME:
                absoluteMin = {y:1899, m:11, d:30, h:0, min:0, s:0, ms:0};
                return !this._compareCustomDateObject (absoluteMin, minValue);
            case Type.NUMBER:
            case Type.CURRENCY:
                if (minValue == -3.40282346638529e+38) {
                    return false;
                }
                return true;
       }
       
       // should not get here!
       return false;
    },
    
    /**
     * Private. Erom doesn't give us NULL in all cases so we need to compare against the boundary values for each type.
     * type - the value type
     * minValue - the value to check
     * returns true if the maxValue is NOT equal NULL or the MAX value for the given datatype.
     */
    _hasMaxBound: function(type,maxValue) {
        if (maxValue == null) {
            return false;
        }
        
        var Type = bobj.crv.params.DataTypes;
        switch(type) {
            case Type.STRING:
                if (maxValue == 65534) {
                    return false;
                }
                return true;
            case Type.DATE:
                absoluteMax = {y:9999, m:11, d:12, h:0, min:0, s:0, ms:0};
                return !this._compareCustomDateObject (absoluteMax, maxValue);
            case Type.TIME:
                absoluteMax = {y:1899, m:11, d:30, h:23, min:59, s:59, ms:0};
                return !this._compareCustomDateObject (absoluteMax, maxValue);
            case Type.DATE_TIME:
                absoluteMax = {y:9999, m:11, d:12, h:23, min:59, s:59, ms:0};
                return !this._compareCustomDateObject (absoluteMax, maxValue);
            case Type.NUMBER:
            case Type.CURRENCY:
                if (maxValue == 3.40282346638529e+38) {
                    return false;
                }
                return true;
       }
       
       // should not get here!
       return false;
    },
    
    _getNoValueDisplayText : function(valueType, isEditableOnPanel) {
        if(!isEditableOnPanel) {
            return L_bobj_crv_ParamsNoneSelected;
        }
        else {
            var DataTypes =  bobj.crv.params.DataTypes;
            var valueTypeText = '';
            switch(valueType) {
            case DataTypes.DATE :
                valueTypeText = L_bobj_crv_Date;
                break;
            case DataTypes.DATE_TIME:
                valueTypeText = L_bobj_crv_DateTime;
                break;
            case DataTypes.TIME:
                valueTypeText = L_bobj_crv_Time;
                break;
            case DataTypes.STRING :
                valueTypeText = L_bobj_crv_Text;
                break;
            case DataTypes.NUMBER :
                valueTypeText = L_bobj_crv_Number;
                break;
            case DataTypes.CURRENCY :
                valueTypeText = L_bobj_crv_Number;
                break;
            case DataTypes.BOOLEAN :
                valueTypeText = L_bobj_crv_Boolean;
                break;
            }
            
            return L_bobj_crv_ParamsEnterOptional.replace("%1", valueTypeText);
        }
    },
    
    _getMinMaxText: function(type,minValue,maxValue) {
   
        var Type = bobj.crv.params.DataTypes;
        var maxValueDisplay,minValueDisplay;
        
        if(type == Type.STRING) {
            /* because min/max of string are in number format */
            minValueDisplay = this._getValueText(Type.NUMBER , minValue);
            maxValueDisplay = this._getValueText(Type.NUMBER, maxValue);
        }
        else {
            minValueDisplay = this._getValueText(type, minValue);
            maxValueDisplay = this._getValueText(type, maxValue);        
        }
        
        if(type == Type.BOOLEAN || (minValue == null && maxValue == null)) {
            return null;
        }
        
        var displayType,returnString;
        
        switch(type) {
            case Type.DATE:
                displayType = L_bobj_crv_Date;
                break;
            case Type.TIME:
                displayType = L_bobj_crv_Time;
                break;
            case Type.DATE_TIME:
                displayType = L_bobj_crv_DateTime;
                break;
            case Type.NUMBER:
                displayType = L_bobj_crv_Number;
                break;
            case Type.CURRENCY:
                displayType = L_bobj_crv_Number;
                break;
       }
       
       var hasMinBound = this._hasMinBound (type, minValue);
       var hasMaxBound = this._hasMaxBound (type, maxValue);
       switch(type) {
            case Type.STRING:
                if(hasMinBound && hasMaxBound) {
                    returnString = L_bobj_crv_ParamsStringMinAndMaxTooltip.replace("%1", minValueDisplay);
                    returnString = returnString.replace("%2",maxValueDisplay);
                }
                else if(hasMinBound) {
                    returnString = L_bobj_crv_ParamsStringMinOrMaxTooltip.replace("%1", L_bobj_crv_Minimum);
                    returnString = returnString.replace("%2", minValueDisplay);
                }
                else if(hasMaxBound) {
                    returnString = L_bobj_crv_ParamsStringMinOrMaxTooltip.replace("%1", L_bobj_crv_Maximum);
                    returnString = returnString.replace("%2", maxValueDisplay);
                }
                break;
            default: 
                if(hasMinBound && hasMaxBound) {
                    returnString = L_bobj_crv_ParamsMinAndMaxTooltip.replace("%1", displayType);
                    returnString = returnString.replace("%2",minValueDisplay);
                    returnString = returnString.replace("%3",maxValueDisplay);
                }
                else if(hasMinBound) {
                    returnString = L_bobj_crv_ParamsMinTooltip.replace("%1", displayType);
                    returnString = returnString.replace("%2", minValueDisplay);
                }
                else if(hasMaxBound) {
                    returnString = L_bobj_crv_ParamsMaxTooltip.replace("%1", displayType);
                    returnString = returnString.replace("%2", maxValueDisplay);
                }    
       }
       
       return returnString;   
    },
    /**
     * Constructs description and display value for default values
     * @param type [bobj.crv.params.DataTypes]
     * @param displayType [bobj.crv.params.DefaultValueDisplayTypes]
     * @param value   [Object] Parameter value to convert to text
     *
     * @return {value: convertedValue, desc: description}
     */
    _getDefaultValue: function(type, displayType, valueObj) {
        var displayTypes = bobj.crv.params.DefaultDisplayTypes;
        var valueText = this._getValueText(type, valueObj.value);
        var valueDesc;
        
        switch(displayType) {
            case displayTypes.Description:
                if(valueObj.desc != null && valueObj.desc.length > 0) {
                    valueDesc = valueObj.desc;
                }
                else {
                    valueDesc = valueText;
                }           
                break;
            case displayTypes.DescriptionAndValue: // Value And Description
                valueDesc = valueText;
                if(valueObj.desc != null && valueObj.desc.length > 0) {
                    valueDesc += ' - ' + valueObj.desc;
                }             
                break;

        }
                
        return valueDesc;
    
    },

    /*
     * Private. Convert value text which could contains description to value text based on default values of param
     * @param param   [bobj.crv.params.Parameter]    Value's data type (bobj.crv.params.DataTypes)
     * @param desc    [string] 
     */
    _getValueTextFromDefValueDesc: function(param, desc) {
        if(param.defaultValues && bobj.isArray(param.defaultValues)) {
            for(var i = 0 ; i < param.defaultValues.length;i++) {
                var defValueDesc = this._getDefaultValue(param.valueDataType,param.defaultDisplayType, param.defaultValues[i]);
                if(defValueDesc == desc) {
                    // if desc is equal to one of LOV's row desc, then get value text of that row
                    return this._getValueText(param.valueDataType, param.defaultValues[i].value);
                }
            }
        }
        return null;
    },
    

    /**
     * Private. Converts a parameter value to display text.
     *
     * @param type    [int]    Value's data type (bobj.crv.params.DataTypes)
     * @param isRange [bool] 
     * @param value   [Object] Parameter value to convert to text
     *
     * @return [String] Returns display text representing the value
     */
    _getValueText: function(type, value) {
        if (value === undefined) {
            return undefined;    
        }

        // Get the actual value to be used. When setting values from the param
        // panel advanced UI it's possible the value will be an object such as
        // {value:'the value'} or {value:'the value', desc:'description'}
        value = bobj.crv.params.getValue (value);
    
        var Type = bobj.crv.params.DataTypes;
        switch(type) {
            case Type.DATE:
                return this._getDateTimeText(value, this._paramOpts.dateFormat);
            case Type.TIME:
                return this._getDateTimeText(value, this._paramOpts.timeFormat);
            case Type.DATE_TIME:
                return this._getDateTimeText(value, this._paramOpts.dateTimeFormat);
            case Type.NUMBER:
            case Type.CURRENCY:
                return this._getNumberText(value,this._paramOpts.numberFormat);    
            case Type.BOOLEAN:    
                return this._getBooleanText(value,this._paramOpts.booleanFormat);
            case Type.STRING:
            default:
                return '' + value;
        }
    },
    
    _getBooleanText: function(value, booleanFormat) {

        return booleanFormat[''+value];
    },
    
     /**
     * Private. Converts a Number value into display text.
     *
     * @param value  [int]  Parameter value to convert to text
     * @param format [{decimalSeperator,groupSeperator}]  Number format  
     *
     * @return [String] Returns display text for the value
     */   
    _getNumberText: function(value,format) {
        var dcSeperator = format.decimalSeperator;
        var gpSeperator = format.groupSeperator;
        var valueSplitted = ('' + value).split(".");
        var leftVal,rightVal,formattedValue;
        var numberSign = null;
        
        leftVal = valueSplitted[0];
        if(leftVal.length > 0 && leftVal.slice(0,1) == '-' || leftVal.slice(0,1) == '+') {
            numberSign = leftVal.slice(0,1);
            leftVal = leftVal.slice(1,leftVal.length);            
        }
        
        rightVal = (valueSplitted.length == 2) ? valueSplitted[1] : null;     
        formattedLeftVal = null;
        
        if(leftVal.length <= 3) {
            formattedLeftVal = leftVal;
        }
        else {
            var gp = null;
            var sliceIndex = null;
            while(leftVal.length > 0) {
                sliceIndex = (leftVal.length > 3) ? leftVal.length - 3 : 0;
                gp = leftVal.slice(sliceIndex,leftVal.length);
                leftVal = leftVal.slice(0, sliceIndex);
                formattedLeftVal = (formattedLeftVal == null) ? gp : gp + gpSeperator + formattedLeftVal;
            }
        }
        
        formattedValue = (rightVal != null) ? formattedLeftVal + dcSeperator + rightVal : formattedLeftVal;
        formattedValue = (numberSign != null) ? numberSign + formattedValue : formattedValue;
        return formattedValue;   
    },
    
    /**
     * Private. Converts a DateTime value into display text.
     *
     * @param value  [Object]  Parameter value to convert to text
     * @param format [String]  DateTime format  
     *
     * @return [String] Returns display text for the value
     */
    _getDateTimeText: function(value, format) {
        var date = bobj.crv.params.jsonToDate(value);
        if (date) {
            return bobj.external.date.formatDate(date, format);
        }
        return '';
    },
    
    /*
     * Looks up value in the param's defualt values list, and if it's existing, get value/desc based on valueDisplayType
     * @param value  [Object] Parameter value to convert to text
     */
    _getValueTextFromDefaultValue: function(param, value) {
        var desc = bobj.crv.params.getDescription (value);
        if (desc !== null) {
            return this._getDefaultValue(param.valueDataType, param.defaultDisplayType, value);
        }

        // Get the actual value to be used, then try to match the description in the LOV
        value = bobj.crv.params.getValue (value);
        
         if(bobj.isArray(param.defaultValues)) {
             var hashValue = bobj.getValueHashCode(param.valueDataType,value);
             for(var i = 0 ; i < param.defaultValues.length; i++) {
                if(hashValue == bobj.getValueHashCode(param.valueDataType,param.defaultValues[i].value)) {
                    return this._getDefaultValue(param.valueDataType, param.defaultDisplayType, param.defaultValues[i]);
                }
            }
        }
        
        return null;       
    },
    
    /*
     * Converts value object to value text in order to be displayed on ParameterUI. takes into account valueDisplayType
     * IMPORTANT : use _getValueText if you want to convert param value Obj to param value text 
     */
    _getDisplayText: function(param,value) {
        if (value === undefined) {
            return undefined;    
        }
        
        if (value.lowerBoundType !== undefined || value.upperBoundType !== undefined) {
            return this._getRangeDisplayText(param, value);    
        }        
        
        var valueText = this._getValueTextFromDefaultValue(param,value);
        
        if(valueText == null) {
            valueText = this._getValueText(param.valueDataType, value);
        }
        
        return valueText;
    },
    
    /**
     * Private. Converts a range value into display text.
     *
     * @param value  [Object] Parameter value to convert to text
     * @param type   [int]    Value's data type (bobj.crv.params.DataTypes)   
     *
     * @return [String] Returns display text for the value
     */
    _getRangeDisplayText: function(param, value) {  
        var displayText = new Object;
        displayText.lowerBound = {type : value.lowerBoundType, value : this._getDisplayText(param, value.beginValue)};
        displayText.upperBound = {type : value.upperBoundType, value : this._getDisplayText(param, value.endValue)};

        return displayText;
    },

    /**
     * Private. Converts text into a parameter value.
     *
     * @param type [int]    Value's data type (bobj.crv.params.DataTypes)
     * @param text [String] Value's display text
     *
     * @return [Object] Parameter value
     */
    _getParamValue: function(type, text) {
        if (type === undefined) {
            return undefined;    
        }
        
        var Type = bobj.crv.params.DataTypes;
        switch(type) {
            case Type.DATE:
                return this._getDateTimeParamValue(text, this._paramOpts.dateFormat);
            case Type.TIME:
                return this._getDateTimeParamValue(text, this._paramOpts.timeFormat);
            case Type.DATE_TIME:
                return this._getDateTimeParamValue(text, this._paramOpts.dateTimeFormat);
            case Type.NUMBER:
            case Type.CURRENCY:
                return this._getNumberParamValue(text,this._paramOpts.numberFormat);
            case Type.BOOLEAN:
                return this._getBooleanParamValue(text,this._paramOpts.booleanFormat);
            case Type.STRING:    
            default:
                return text;
        }
    },
  
      /**
     * Private. Converts boolean text to a paramter value
     * @param   text    [String] Value's display text
     * @param   booleanFormat    [boolean format] {true: text : false : text}
     * @return [String] 
     */  
    _getBooleanParamValue: function(text,booleanFormat) {
        if(text != null && text.length != 0) {
            return  booleanFormat["true"] == text;
        }
        else {
            return null;
        }
    },
    
    clearParameterValues : function(param) {
        if(param.allowNullValue)
            this.updateParameter(param.paramName, [null]);
        else
            this.updateParameter(param.paramName, []);
        
        this._updateToolbar();
    },
    
    /**
     * Private. Converts Number text to a paramter value
     * @param   text    [String] Value's display text
     * @param   format  [{decimalSeperator,groupSeperator}] number format
     *
     * @return [String] 
     */
    _getNumberParamValue: function(text,format) {
        if(text == null) {
            return null;
        }
        
        var value = '';
        
        if (/[ \f\n\r\t\v\u00A0\u2028\u2029]/.test(format.groupSeperator)) {
            value = text.replace(/[ \f\n\r\t\v\u00A0\u2028\u2029]/g, '');
        }
        else {
            var gpRE = new RegExp( "\\" + format.groupSeperator, "g" );
            value = text.replace(gpRE,"")
        }
        
        return value.replace(format.decimalSeperator, ".");
    },
    
    /**
     * Private. Converts DateTime text into a parameter value.
     *
     * @param text   [String] Value's display text
     * @param format [String] DateTime format
     *
     * @return [Object] DateTime parameter value or the original value if getDateFromFormat fails
     */
    _getDateTimeParamValue: function(text, format) {
        var date = bobj.external.date.getDateFromFormat(text, format);
        if (date) {
            return bobj.crv.params.dateToJson(date);
        }
        return text;
    },
    
    /**
     * Private. Attach callbacks to a parameter widget
     *
     * @param widget [ParameterUI]
     */
    _observeParamWidget: function(widget) {
        if (widget) {
            var bind = MochiKit.Base.bind;
            widget.changeValueCB = bind(this._onChangeValue, this, widget);
            widget.enterPressCB = bind(this._onEnterPress, this, widget);
        }
    },
 
    /**
     * Private ParameterUI callback. Updates the UI when a param value is edited.
     *
     * @param widget    [ParameterUI] The source of the event
     * @param valueIdx  [int]         The index of the value that changed
     * @param new Value [String]      The new value (display text) at valueIdx 
     */
    _onChangeValue: function(widget, valueIdx, newValue) { 
        if (!this._isPanelEditable()) {
            return;
        }
        
        var parameterTab = this._panel.getParameterTabByWidget(widget);
        if(parameterTab)
            parameterTab.setDirty(true);
        
        this._checkAndSetValue(widget, valueIdx);
        this._updateToolbar();
    },
    
    /**
     * Private ParameterUI callback. Attempts to apply change param values to 
     * the report when the Enter key is pressed while editing a value. 
     *
     * @param widget    [ParameterUI] The source of the event
     * @param valueIdx  [int]         The index of the value that has focus  
     */
    _onEnterPress: function(widget, valueIdx) {
        if(this._panel.isApplyButtonEnabled())
            this._applyValues();
    },
        
    /**
     * Private ParameterPanel callback. Applies changed parameter values to the
     * report when the run button is clicked on the panel's toolbar.
     */
    _onClickTbApplyButton: function() {
        this._applyValues();
    },
    
    _onClickTbResetButton: function() {
        this.resetParamPanel();
    },    
    
    /**
     * Private ParameterPanel callback. Shows the advanced dialog for the 
     * selected parameter when the advanced dialog button is clicked on the 
     * panel's toolbar.
     */
    _onClickTbAdvButton: function(param) {
        if (this._isPanelEditable()) {
            this._viewerCtrl.showAdvancedParamDialog(param);
        }
    },
            
    /**
     * Private. Attempts to apply changed parameter values to the report.
     * Changed display text will be converted to parameter values and validated.
     * Validation failure will cause a warning to appear instead of updating the
     * report.
     */
    _applyValues: function() {      
       
        var numParams = this._paramList.length;
        var badParamIdx = -1;
        var badValueIdx = -1;
        var warning = null;
        var paramUI = null;
        
        for (var i = 0; (i < numParams) && !warning; ++i) {
            paramUI = this._panel.getParameter(i);
            var numValues = paramUI.getNumValues();
            for (var j = 0; (j < numValues) && !warning; ++j) {
                warning = paramUI.getWarning(j);
                if (warning) {
                    badParamIdx = i;
                    badValueIdx = j;
                }
            }
        }
        
        if (warning) {
            paramUI = this._panel.getParameter(badParamIdx);
            paramUI.setFocusOnRow(badValueIdx);
        }
        else {
            // make all value changes permanent
            for (var i = 0, c = this._paramList.length; i < c; i++) {
                this._paramList[i].commitValue();
            }
            for (var i = 0, c = this._unusedParamList.length; i < c; i++) {
                this._unusedParamList[i].commitValue();
            }
            this._viewerCtrl.applyParams(this._paramList.concat(this._unusedParamList));
            this._panel.setApplyButtonEnabled(false);
        }
    },
    
    /**
     * Private. Finds the Parameter associated with a given ParameterUI.
     * 
     * @param widget [ParameterUI] 
     *
     * @return [Parameter] 
     */
    _findParam: function(widget) {
        return this._paramList[this._panel.getIndex(widget)];   
    },
    
    _findWidget: function(param) {
        for(var i = 0 ; i < this._paramList.length; i++) {
            if(this._paramList[i].paramName == param.paramName) {
                return this._panel.getParameter(i)
            }
        }
        return null;
    },
    
    /**
     * Private. Updates the state of the parameter panel's toolbar buttons. 
     */
    _updateToolbar: function() {
        if (!this._isPanelEditable()) {
            return;
        }
                        
        this._panel.setApplyButtonEnabled(true); //FIXME: should only enable when panel is dirty
        this._panel.setResetButtonEnabled(true); //FIXME: should only enable when panel is dirty
    },
    
    /**
     * Private. Selects a Date/Time format string from paramOpts based on the 
     * give parameter value type.
     *
     * @param dataType [bobj.crv.params.DataTypes]
     *
     * @return [String] Returns a DateTime format string.
     */
    _getDateTimeFormat: function(dataType) {
        var Type = bobj.crv.params.DataTypes;
        switch(dataType) {
            case Type.DATE: 
                return this._paramOpts.dateFormat;
            case Type.TIME: 
                return this._paramOpts.timeFormat;
            case Type.DATE_TIME: 
                return this._paramOpts.dateTimeFormat;
            default: return null;
        }
    },
    
    /**
     * Private. Convert display text to a param value, validate it, and update
     * the value of the associated Parameter instance when validation succeeds.
     * Sets the warning text of the value when validation fails.
     *
     * @param widget   [ParameterUI]  
     * @param valueIdx [int] Index of value to validate and update
     */
    _checkAndSetValue: function(widget, valueIdx) {
        /*
         * Return if parameter value is modified in advanced dialog as the parameter value is already set by updateParameter
         */
        if(!widget.canChangeOnPanel) {
            return;
        }
        
        var parameter = this._findParam(widget);
        var valueText = widget.getValueAt(valueIdx); 
        
        // Checking if value exists in defaultValues and if it does, get the value from defaultValue
        var defValue = this._getValueTextFromDefValueDesc(parameter,valueText);
        if(defValue != null) {
                valueText = defValue;
        }

        var paramValue = this._getParamValue(parameter.valueDataType, valueText);
        
        
        // if the following condition is true, the user is providing 'no value' to an optional parameter
        if (valueIdx == 0 && paramValue == undefined) {
            if(parameter.allowNullValue && parameter.getValue().length == 0) {
                parameter.setValue(0, null);
            }
            
            if(parameter.isOptionalPrompt) {
                parameter.clearValue();
            }
            
            widget.setWarning(valueIdx,null); // Removes warning if there is any
            
            return;
        }
        
        var Status = bobj.crv.params.Validator.ValueStatus;
        var code = Status.OK;
        var warningMsg = null;
        
        code = bobj.crv.params.Validator.getInstance().validateValue(parameter, paramValue);
        
        if (Status.OK === code) {
            var cText = this._getValueTextFromDefaultValue(parameter,paramValue);
            if(cText != null) {
                widget.setValueAt(valueIdx,cText);
            }           
            widget.setWarning(valueIdx,null); // Removes warning
            parameter.setValue(valueIdx, paramValue); //checks for duplicates when setting a value

            
        }
        else {
            warningMsg = this._getWarningText(parameter, code);
            widget.setWarning(valueIdx, {code : code, message : warningMsg });
        }
        
    },
    
    /**
     * Private. Convert a validation error code into a readable warning message.
     *
     * @param param [Parameter]   Parameter that has the erroroneous value
     * @param code  [ValueStatus] Validation error code
     *
     * @return [String] Returns a warning message suitable for display in the UI
     */
    _getWarningText: function(param, code) { 
        var Type = bobj.crv.params.DataTypes;

            switch(param.valueDataType) {
                case Type.DATE:
                case Type.TIME:
                case Type.DATE_TIME:
                    return this._getDateTimeWarning(param,code);
                case Type.STRING:
                    return this._getStringWarning(param, code);
                case Type.NUMBER:
                case Type.CURRENCY:
                    return this._getNumberWarning(param,code);
                default:
                    return null;
            }   
    },

    /**
     * Private. Create warning message for invalid datetime value
     * @param param [Parameter]   Parameter that has the erroroneous value
     * @param code  [ValueStatus] Validation error code
     *
     * @return [String] Returns a warning message suitable for display in the UI
     */ 
    _getDateTimeWarning: function(param,code) {
        var dataTypes = bobj.crv.params.DataTypes;
        var ValueStatus = bobj.crv.params.Validator.ValueStatus;
        var dateFormat = this._paramOpts.dateFormat;
        
        dateFormat = dateFormat.replace('yyyy', '%1');
        dateFormat = dateFormat.replace(/M+/, '%2');
        dateFormat = dateFormat.replace(/d+/, '%3');   
        dateFormat = dateFormat.replace('%1', L_bobj_crv_ParamsYearToken);
        dateFormat = dateFormat.replace('%2', L_bobj_crv_ParamsMonthToken);
        dateFormat = dateFormat.replace('%3', L_bobj_crv_ParamsDayToken);        
        
        if(code == ValueStatus.ERROR || code == ValueStatus.VALUE_INVALID_TYPE) {
            switch(param.valueDataType) {
                case dataTypes.DATE:
                    return L_bobj_crv_ParamsBadDate.replace("%1",dateFormat);
                case dataTypes.DATE_TIME:
                    return L_bobj_crv_ParamsBadDateTime.replace("%1",dateFormat);
                    break;
                case dataTypes.TIME:
                    return L_bobj_crv_ParamsBadTime
                    break;     
            }
        }
        else if(code == ValueStatus.VALUE_TOO_BIG || code == ValueStatus.VALUE_TOO_SMALL){
            return this._getMinMaxText(param.valueDataType,param.minValue,param.maxValue);
        }
        
        return null;       
    },
    
    /**
     * Private. Create a warning message for String parameter 
     *
     * @param param [Parameter]   Parameter that has the erroroneous value
     * @param code  [ValueStatus] Validation error code
     *
     * @return [String] Returns a warning message suitable for display in the UI
     */ 
    _getStringWarning: function(param, code) {
        var Status = bobj.crv.params.Validator.ValueStatus;
        if (Status.VALUE_TOO_LONG === code) {
            return L_bobj_crv_ParamsTooLong.replace('%1', param.maxValue);
        }
        else if(Status.VALUE_TOO_SHORT === code){ 
            return L_bobj_crv_ParamsTooShort.replace('%1', param.minValue);
        }
        return null;
    },
    
    /**
     * Private. Create warning message for invalid datetime value
     * @param param [Parameter]   Parameter that has the erroroneous value
     * @param code  [ValueStatus] Validation error code
     *
     * @return [String] Returns a warning message suitable for display in the UI
     */   
     _getNumberWarning : function(param,code) {
        var ValueStatus = bobj.crv.params.Validator.ValueStatus;
        var dataTypes = bobj.crv.params.DataTypes;
        switch(code) {
            case ValueStatus.ERROR:
            case ValueStatus.VALUE_INVALID_TYPE:
                if(param.valueDataType == dataTypes.NUMBER) {
                    return L_bobj_crv_ParamsBadNumber;
                }
                else if(param.valueDataType == dataTypes.CURRENCY) {
                    return L_bobj_crv_ParamsBadCurrency;
                }    
            case ValueStatus.VALUE_TOO_BIG:
            case ValueStatus.VALUE_TOO_SMALL:
                return this._getMinMaxText(param.valueDataType,param.minValue,param.maxValue);
            default:
                return null;
        
        }
     }  
};
