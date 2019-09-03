/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof bobj.crv.params == 'undefined') {
    bobj.crv.params = {};
}
    
    bobj.crv.params.DataTypes = {
        DATE: "d",
        DATE_TIME: "dt",
        TIME: "t",
        STRING: "s",
        NUMBER: "n",
        CURRENCY: "c",
        BOOLEAN: "b"
    };
    
    bobj.crv.params.RangeBoundTypes = {
        UNBOUNDED: 0,
        EXCLUSIVE: 1,
        INCLUSIVE: 2
    };
    
    bobj.crv.params.DefaultDisplayTypes = {
        Description: 0,
        DescriptionAndValue: 1
    };
    
    bobj.crv.params.CompareResults = {
        TOO_BIG: 1,
        TOO_SMALL: -1,
        EQUAL: 0
    };

/*
================================================================================
Parameter
================================================================================
*/

/**
 * Constructor.
 */
bobj.crv.params.Parameter = function(paramInfo) {
    var PARAMS = bobj.crv.params;
    var displayTypes = PARAMS.DefaultDisplayTypes;
    MochiKit.Base.update(this, {
        paramName: null,
        reportName: null,
        description: null,
        valueDataType: null,
        value: null,
        modifiedValue: null,
        defaultValues: null,
        defaultDisplayType : displayTypes.DescriptionAndValue,
        maxValue: null, 
        minValue: null,
        allowCustomValue: true,
        allowDiscreteValue: true,
        allowMultiValue: false,
        allowNullValue: false,
        allowRangeValue: false,
        editMask: null,
        isOptionalPrompt: false,
        isEditable: true,
        isHidden: false,
        isDataFetching: false,
        attributes: null
    }, paramInfo);
    
};

/**
 *  IMPORTANT! Use getValue and setValue to access the value of the parameter, instead of the property itself.
 */
bobj.crv.params.Parameter.prototype = {
    getTitle: function() {
        return (this.description || this.paramName); 
    },
    
    getName: function () {
        return this.paramName;
    },
    
    hasLOV: function() {
        return (this.defaultValues != null && this.defaultValues.length > 0);    
    },
    
    hasDescription: function() {
    	return this.description != null;
    },
    
    isPassword: function() {
        return (this.editMask !== null && this.editMask.toLowerCase() == "password");
    },
    
    getValue: function() {
        this._initModifiedValue();
        return this.modifiedValue;
    },
    
    /*
     * Undos any modifications done on parameter that have not been commited
     */
    reset: function() {
    	delete this.modifiedValue;
    },
    
    removeValueAt: function(index) {
        this._initModifiedValue();
        var value = this.modifiedValue[index];
        this.modifiedValue.splice(index, 1);
    },
    
    // setValue accepts either an array, or an int + an object.
    setValue: function (i, newValue) {
        this._initModifiedValue();
        
        if (arguments.length == 1 && bobj.isArray(arguments[0])) {
            var value= arguments[0];
            this.modifiedValue = value;
        }
        else if (arguments.length == 2) {
            var oldValue = this.modifiedValue[i];
            this.modifiedValue[i] = newValue;
        }
    },
    
    clearValue: function() {
        this._initModifiedValue();
        this.modifiedValue = [];
    },
    
    // commitValue will actually apply the changes made so far
    commitValue: function() {
        this._initModifiedValue();
        this.value = this.modifiedValue.slice(0);
    },
    
    _initModifiedValue: function() {
        if (!this.modifiedValue) {
            if (bobj.isArray(this.value)) {
                this.modifiedValue = this.value.slice(0); // make a deep copy of "value"
            }
            else {
                this.modifiedValue = [];
            }
        }
    },
        
    isDCP: function () {
        if (this.attributes != null) {
            if (this.attributes['IsDCP'] === true) {
                return true;
            }
        }
        
        return false;
    }
};

/*
================================================================================
Validator for Parameters
================================================================================
*/
bobj.crv.params.Validator = function(){};

bobj.crv.params.Validator.ValueStatus = {
    OK: 0,               
    ERROR: 1,
    VALUE_MISSING: 2,      // Required value is missing
    VALUE_INVALID_TYPE: 3, // Value has the wrong data type
    VALUE_TOO_LONG: 4,     // Value's length is less than the minimum
    VALUE_TOO_SHORT: 5,    // Value's length is greater than the maximum
    VALUE_TOO_BIG: 6,      // Value is greater than the maximum
    VALUE_TOO_SMALL: 7,     // Value is less than the minimum
    VALUE_DUPLICATE: 8
};

bobj.crv.params.Validator.getInstance = function(){
    if (!bobj.crv.params.Validator.__instance) {
        bobj.crv.params.Validator.__instance = new bobj.crv.params.Validator();
    }
    return bobj.crv.params.Validator.__instance;
};
    
bobj.crv.params.Validator.prototype = {
    /**
     * Validate a Parameter instance and return a status object
     */
    validateParameter: function(param) {
        var PARAMS = bobj.crv.params;
        if (!param) {return null;}
        
        var Status = PARAMS.Validator.ValueStatus;
        
        if (!bobj.isArray(param.value) || !param.value.length) {
            return {
                isValid: false,
                reason: Status.VALUE_MISSING
            };
        }
        
        var isValid = true;
        var statusList = [];
        
        for (var i = 0, len = param.values.length; i < len; ++i) {
            var status = PARAMS.validateValue(param, i);
            statusList.push(status);
            isValid = isValid && (status === ValueStatus.OK);
        }
        
        return {
            isValid: isValid,
            statusList: statusList
        };
        
    },
    
    /**
     * Validate a parameter value and return a status code
     */
    validateValue: function(param, value) {
        var Status = bobj.crv.params.Validator.ValueStatus;
        
        if (!param || !bobj.isArray(param.value) || (value === undefined)) {
            return Status.ERROR;
        }
        
        var validatorFunc = this._getTypeValidatorFunc(param.valueDataType);
        if (!validatorFunc) {
            return Status.ERROR;
        }
        
        var actualValue = bobj.crv.params.getValue(value);
        return validatorFunc(param, actualValue);
    },
    
    _getTypeValidatorFunc: function(type) {
        var Type = bobj.crv.params.DataTypes;
        
        switch (type) {
            case Type.STRING:
                return this._validateString;
            case Type.NUMBER:
            case Type.CURRENCY:
                return this._validateNumber;
            case Type.DATE:
            case Type.TIME:
            case Type.DATE_TIME:
                return this._validateDateTime;
            case Type.BOOLEAN:
                return this._validateBoolean;
            default:
                return null; 
        }
    },
    
    _validateString: function(param, value) { 
        var Status = bobj.crv.params.Validator.ValueStatus;
        
        if (!bobj.isString(value)) { 
            return Status.VALUE_INVALID_TYPE; 
        }
        
        var maxValue = param.maxValue;
        var minValue = param.minValue;
        if (bobj.isNumber(maxValue) && value.length > maxValue) {
            return Status.VALUE_TOO_LONG;
        }
            
        if (bobj.isNumber(minValue) && value.length < minValue) {
            return Status.VALUE_TOO_SHORT;
        }
        
        return Status.OK;
    },
    
    _validateNumber: function(param, value) {
        //the "value" passed in here is a number that has at most one decimal separator and no group separator
        var Status = bobj.crv.params.Validator.ValueStatus;
        var regNumber = /^(\+|-)?(\d+((\.\d+)|(\.))?|\.\d+)$/;
        
        if(bobj.isString(value) && regNumber.test(value)) {
            value = parseFloat(value);
        }
        else if(!bobj.isNumber(value)) {
            return Status.VALUE_INVALID_TYPE; 
        }
        
        var maxValue = param.maxValue;
        var minValue = param.minValue;
        if(maxValue !== null && value > maxValue) {
            return Status.VALUE_TOO_BIG; 
        }
        else if(minValue !== null && value < minValue) {
            return Status.VALUE_TOO_SMALL;
        }
        else {
            return Status.OK;
        }        
    },
    
    _validateDateTime: function(param, value) {
        var Result = bobj.crv.params.CompareResults;
        var Status = bobj.crv.params.Validator.ValueStatus;

        if (bobj.isObject(value)) {
            var isNumber = function(sel) {return bobj.isNumber(value[sel]);}; 
            if (MochiKit.Iter.every(['d','m', 'y', 'h', 'min', 's', 'ms'], isNumber)) {
            
                var compareFunc = bobj.crv.params.getDateCompareFunc(param.valueDataType);
                if(param.minValue && compareFunc(param.minValue,value) == Result.TOO_BIG) {
                    return Status.VALUE_TOO_SMALL;
                }
                else if(param.maxValue && compareFunc(param.maxValue,value) == Result.TOO_SMALL) {
                    return Status.VALUE_TOO_BIG;
                }
                else {
                    return Status.OK;
                }
            }
        }
        
        return Status.VALUE_INVALID_TYPE;
    },
    
    _validateBoolean: function(param, value) {
        return bobj.crv.params.Validator.ValueStatus.OK; 
    }
};

/*
================================================================================
Utility Functions
================================================================================
*/

/**
 * Get an object that represents a Date and can be serialized to a json string 
 *
 * @param date [Date]  The Date instance that should be represented as json
 * 
 * @return [Object] Object representing the date with (key, value) pairs
 */
bobj.crv.params.dateToJson = function(date) {
    return {
        d: date.getDate(),
        m: date.getMonth(),
        y: date.getFullYear(),
        h: date.getHours(),
        min: date.getMinutes(),
        s: date.getSeconds(),
        ms: date.getMilliseconds()
    };
};

bobj.crv.params.getDateCompareFunc = function(type) {
    var PARAMS = bobj.crv.params;
    var Type = PARAMS.DataTypes;
        
    switch (type) {
            case Type.DATE:
                return PARAMS.compareDate;
            case Type.TIME:
                return PARAMS.compareTime;
            case Type.DATE_TIME:
                return PARAMS.compareDateTime;
            default:
                return null; 
        }
};

bobj.crv.params.compareDateTime = function(dateTimeA, dateTimeB) {
    var PARAMS = bobj.crv.params;
    var Result = PARAMS.CompareResults;
    
    var dateResult = PARAMS.compareDate(dateTimeA,dateTimeB);
    var timeResult = PARAMS.compareTime(dateTimeA,dateTimeB);
    
    if(dateResult == Result.EQUAL && timeResult == Result.EQUAL) {
        return Result.EQUAL;
    }
    
    if(dateResult != Result.EQUAL) {
        return dateResult;
    }
    else {
        return timeResult;
    }
};

/*
 * Compares two dates
 * @param dateA    [JSON DateTime {y,m,d,h,m,s,ms}] first date value
 * @param dateB    [JSON DateTime {y,m,d,h,m,s,ms}] second date value
 *
 * @return 
 *  0:  dateA = dateB
 *  1:  dateA > dateB
 * -1:  dateA < dateB
 */
bobj.crv.params.compareDate = function(dateTimeA, dateTimeB) {
    var Result = bobj.crv.params.CompareResults;
    
    if( dateTimeA.d == dateTimeB.d && dateTimeA.m == dateTimeB.m && dateTimeA.y == dateTimeB.y){
        return Result.EQUAL;
    }
        
    if( dateTimeA.y > dateTimeB.y) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.y < dateTimeB.y) {
        return Result.TOO_SMALL;
    }  
    
    if(dateTimeA.m > dateTimeB.m) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.m < dateTimeB.m) {
        return Result.TOO_SMALL;
    }
    
    if(dateTimeA.d > dateTimeB.d) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.d < dateTimeB.d) {
        return Result.TOO_SMALL;
    }
};

/*
 * Compares two times
 * @param timeA    [JSON DateTime {y,m,d,h,m,s,ms}] first time value
 * @param timeB    [JSON DateTime {y,m,d,h,m,s,ms}] second time value
 *
 * @return 
 *  0:  dateA = dateB
 *  1:  dateA > dateB
 * -1:  dateA < dateB
 */
bobj.crv.params.compareTime = function(dateTimeA,dateTimeB) {
    var Result = bobj.crv.params.CompareResults;
    
    if(dateTimeA.h == dateTimeB.h && dateTimeA.min == dateTimeB.min && dateTimeA.s == dateTimeB.s && dateTimeA.ms == dateTimeB.ms) {
        return Result.EQUAL;
    }
    
    if(dateTimeA.h > dateTimeB.h) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.h < dateTimeB.h){
        return Result.TOO_SMALL;
    }
    
    if(dateTimeA.min > dateTimeB.min) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.min < dateTimeB.min) {
        return Result.TOO_SMALL;
    }
    
    if(dateTimeA.s > dateTimeB.s) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.s < dateTimeB.s) {
        return Result.TOO_SMALL;
    }
    
    if(dateTimeA.ms > dateTimeB.ms) {
        return Result.TOO_BIG;
    }
    else if(dateTimeA.ms < dateTimeB.ms){
        return Result.TOO_SMALL;
    }
};

/**
 * Get a Date instance from an object containing (key, value) pairs
 *
 * @param json [Object]  Object with keys that match Date properties
 *
 * @return [Date] Returns a Date instance
 */
bobj.crv.params.jsonToDate = function(json) {
    var date = new Date();
    
    if (json) {
        date.setFullYear(json.y || 0, json.m || 0, json.d || 1);
        date.setHours(json.h || 0);
        date.setMinutes(json.min || 0);
        date.setSeconds(json.s || 0);
        date.setMilliseconds(json.ms || 0);
    }
    
    return date;
};

bobj.crv.params.getValue = function (pair) {
    if (pair === undefined || pair === null || pair.value === undefined)
        return pair;

    return pair.value;
};

bobj.crv.params.getDescription = function (pair) {
    if (pair === undefined || pair === null || pair.desc === undefined)
        return null;

    return pair.desc;
};
