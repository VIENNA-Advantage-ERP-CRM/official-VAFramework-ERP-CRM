/* Copyright (c) Business Objects 2006. All rights reserved. */

/*
 ================================================================================
 ViewerFlexParameterAdapter.js

 Viewer's implementaion for flex prompting UI
 ================================================================================
 */

bobj.crv.params.ViewerFlexParameterAdapter = {
    _viewerLayoutType : [],
    _promptData : [],
    _paramCtrl : [],
    _iParam : [],
    _iPromptUnitData : [],
    _iParamData : [],
    _moveArea : null,

    ///////////////////////////
    // Member setters
    //////////////////////////
    setViewerLayoutType : function(id, l) {
        this._viewerLayoutType[id] = l;
    },
    
    setPromptData : function(id, d, forIParams) {
        if (!forIParams){
            /* Full prompt UI data */
            this._promptData[id] = d;
            this.clearIParamPromptUnitData(id);
        }else{
            /* Interactive prompt UI data */
            for(var i = 0, l = d.length; i < l; i++) {
                var unit = d[i];
                this._addIParamPromptUnitData(id, unit.id, unit.names, unit.data);
            }
        }
    },

    setCurrentIParamInfo : function(id, c, p) {
        this._paramCtrl[id] = c;
        this._iParam[id] = p;
    },
    
    getShowMinUI : function (id) {
        return this.hasIParamPromptUnitData(id);
    },
    
    getWidth : function (id) {
        if(this.hasIParamPromptUnitData(id))
            return 300;
        else
            return this.getSWFWidth(id);
    },
    
    getHeight : function (id) {
        if(this.hasIParamPromptUnitData(id))
            return 315;
        else
            return this.getSWFHeight(id);
    },
    
    getScreenHeight : function (id) {
        var lDim = MochiKit.Style.getElementDimensions(getLayer(id));
        return lDim.h - 2;
    },
    
    getScreenWidth : function (id) {
        var lDim = MochiKit.Style.getElementDimensions(getLayer(id));
        return lDim.w - 2;
    },

    /*
     * Returns the ideal height of the prompting window
     */
    getSWFHeight : function(id) {
        var lTypes = bobj.crv.Viewer.LayoutTypes;
        var layout = lTypes.CLIENT;
        if (this._viewerLayoutType[id]) {
            layout = this._viewerLayoutType[id];
        }
        
        var min = layout === lTypes.FIXED ? 0 : 480;
        var sH = this.getScreenHeight(id);
        return Math.max(min, sH - 200);
    },
    
    getSWFWidth : function(id) {
        var sW = this.getScreenWidth(id);
        return Math.min(600, sW - 20);
    },
    
    getAllowFullScreen : function(id)
    {
        return !this.hasIParamPromptUnitData(id);
    },
    
    hasIParamPromptUnitData : function (id) {
        return (this._iPromptUnitData[id] != null) && (this._iParamData[id] != null) && (this._iParam[id] != null);
    },
    
    _addIParamPromptUnitData : function(id, unitID, names, data) {
        if (!this.hasIParamPromptUnitData(id)) {
            this._iPromptUnitData[id] = [];
            this._iParamData[id] = [];
        }

        this._iPromptUnitData[id][unitID] = data;
        for ( var i = 0, len = names.length; i < len; i++) {
            this._iParamData[id][names[i]] = unitID;
        }
    },

    clearIParamPromptUnitData : function (id) {
        if (!this.hasIParamPromptUnitData(id)) {
            return;
        }
        
        delete this._iPromptUnitData[id];
        delete this._iParamData[id];
        delete this._iParam[id];
    },
    
    ///////////////////////////
    // Callbacks
    //////////////////////////

    getPromptData : function (id) {
        if (this.hasIParamPromptUnitData(id)) {
            var promptUUID = this._iParamData[id][this._iParam[id].paramName];
            if (promptUUID) {
                return this._iPromptUnitData[id][promptUUID];
            }
        }
        
        return this._promptData[id];
    },
    
    /**
     * Flex callback to start moving the dialog
     */
    startDrag : function(id) {
        var swf = bobj.crv.params.FlexParameterBridge.getSWF(id);
        if (swf) {
            if (this._moveArea) {
                return;
            }
        
            this._moveArea = document.createElement('div');
            this._moveArea.id = bobj.uniqueId();
        
            MOVE_STYLE = this._moveArea.style;
        
            var STYLE = swf.style;
            var P_STYLE = swf.parentNode.style;
        
            MOVE_STYLE.top = P_STYLE.top;
            MOVE_STYLE.left = P_STYLE.left;
            MOVE_STYLE.width = STYLE.width ? STYLE.width : swf.width + 'px';
            MOVE_STYLE.height = STYLE.height ? STYLE.height : swf.height + 'px';;
            MOVE_STYLE.border = '1px';
            MOVE_STYLE.borderStyle = 'solid';
            MOVE_STYLE.borderColor = '#000000';
            MOVE_STYLE.backgroundColor = '#FFFFFF';
            MOVE_STYLE.position = 'absolute';
            MOVE_STYLE.opacity = 0.50;
            MOVE_STYLE.filter = 'alpha(opacity=50)';
            MOVE_STYLE.zIndex = bobj.constants.modalLayerIndex - 1;
        
            document.body.appendChild(this._moveArea);
            document.body.style.cursor = 'move';
        }
    },

    /**
     * Flex callback when finished moving the dialog
     */
    stopDrag : function(id) {
        if (this._moveArea) {
            var p = MochiKit.Style.getElementPosition(this._moveArea);
            this.move(id, p.x, p.y);
        
            document.body.removeChild(this._moveArea);
            delete this._moveArea;
        
            document.body.style.cursor = 'default';
        }
    },

    /**
     * Flex callback for moving the dialog.
     * x is the amount to move on the x axis, -:left +:right
     * y is the amount to move on the y axis, -:up +:down
     */
    drag : function(id, x, y) {
        var LOG = bobj.crv.logger;
        LOG.info('doMove Called viewer:' + id + ' x:' + x + ' y:' + y);
    
        var l = getLayer(id);
        if (!l) {
            LOG.error('Shifting SWF could not find the viewer:' + id);
            return;
        }
    
        var m = this._moveArea;
        if (!m) {
            LOG.error('Unable to move SWF, no move area available');
            return;
        }
    
        var mX = m.offsetLeft;
        var mY = m.offsetTop;
        var mH = m.offsetHeight;
        var mW = m.offsetWidth;
        var vX = l.offsetLeft;
        var vY = l.offsetTop;
        var vH = l.offsetHeight;
        var vW = l.offsetWidth;
    
        var newX = mX + x;
        var newY = mY + y;
    
        if (newY < vY) {
            newY = vY;
        } else if (newY + mH > vY + vH) {
            newY = vH - mH;
        }
    
        if (newX < vX) {
            newX = vX;
        } else if (newX + mW > vX + vW) {
            newX = vW - mW;
        }
    
        m.style.top = newY + 'px';
        m.style.left = newX + 'px';
    
        LOG.info('Moved the SWF to x:' + newX + ' y:' + newY);
    },


    /**
     * Flex callback when finished moving the dialog
     */
    move : function(id, x, y) {
        var swf = bobj.crv.params.FlexParameterBridge.getSWF(id);
        if (swf) {
            var p = new MochiKit.Style.Coordinates(x,y);
            MochiKit.Style.setElementPosition(swf.parentNode, p);
        }
    },
    
    setParamValues : function(id, valueData) {
        bobj.crv.logger.info('setting parameter values');
        if (this.hasIParamPromptUnitData(id)){
            this._setIParamValues (id, valueData);
        } else {
            this._setFullParamValues (id, valueData);
            this.closeDialog(id);
        }
    },

    _setFullParamValues : function (id, valueData) {
        bobj.event.publish('crprompt_flexparam', id, valueData);
    },
    
    _setIParamValues : function (id, valueData) {
        var param = this._iParam[id];
        var ctrl = this._paramCtrl[id];
        var data = this._iParamData[id];
        var unitData = this._iPromptUnitData[id];

        if (!param || !ctrl || !data || !unitData || valueData.length != 1) {
            return;
        }
        
        var vPromptUnit = valueData[0];
        var vPrompts = vPromptUnit.prompts;
        for (var i = 0, len = vPrompts.length; i < len; i++) {
            var vPrompt = vPrompts[i];
            
            if (!vPrompt || !vPrompt.name || !vPrompt.values) {
                continue;
            }

            ctrl.updateParameter(decodeURI(vPrompt.name), this._convertFlexValues(vPrompt, param.valueDataType));
        }
        
        ctrl._updateToolbar();
    
        this._updatePromptData(id, vPromptUnit, param.valueDataType);
    
        this.closeDialog(id);
    },

    _updatePromptData: function (id, newUnitData, type){
        var newPrompts = newUnitData.prompts;
        
        var data = this._iPromptUnitData[id][newUnitData.id];
        var unitData = data.promptUnits[0];
        var prompts = unitData.prompts;
        for (var i = 0, pLen = prompts.length; i < pLen; i++) {
            var prompt = prompts[i];
            for (var j = 0, npLen = newPrompts.length; j < npLen; j++) {
                var newPrompt = newPrompts[j];
                if (prompt.id == newPrompt.id){
                    prompt.values = this._unescapeFlexValues(newPrompt.values, type);
                    break;
                }
            }
        }
    },
    
    _unescapeFlexValues: function (fValues, type){
        if (type != bobj.crv.params.DataTypes.STRING) {
            return fValues;
        }
        
        for (var i = 0, len = fValues.length; i < len; i++) {
            this._unescapeFlexValue(fValues[i], type);
        }
        
        return fValues;
    }, 
    
    _unescapeFlexValue: function (fValue, type) {
        if (type != bobj.crv.params.DataTypes.STRING) {
            return;
        }
       
        if ((fValue.value !== undefined && fValue.value !== null)){
            fValue.value = decodeURI(fValue.value);
            
            if (fValue.labels !== undefined && fValue.labels !== null){
               for (var i = 0, len = fValue.labels.length; i < len; i++) {
                   fValue.labels[i] = decodeURI(fValue.labels[i]);
               }
            }
            
        } else {
            // Range
            if (fValue.start){
                this._unescapeFlexValue(fValue.start, type);
            }

            if (fValue.end){
                this._unescapeFlexValue(fValue.end, type);
            }
        }
    },
    
    _getDescriptionIndex: function (prompt)
    {
       var vIndex = prompt.lovValueIndex;
       var types = prompt.lovFieldTypes;
       if (vIndex !== undefined && types !== undefined)
       {
       	    for (var i=0, len=types.length; i<len; i++) {
                if (i != vIndex && types[i] == "s"){
                    return i;
                }
            }
       }
    
       return -1;
    },
    
    _convertFlexValues: function (prompt, type){
        var dIndex = this._getDescriptionIndex(prompt);
        var fValues = prompt.values;
        var jsValues = [];
        for (var i = 0, len = fValues.length; i < len; i++) {
            jsValues.push(this._convertFlexValue(fValues[i], type, dIndex));
        }
        return jsValues;
    },

    _convertFlexValue: function (fValue, type, dIndex) {
       var jsValue = {};
        if ((fValue.value !== undefined && fValue.value !== null)){
            // Discrete
            if (dIndex > -1 && fValue.labels && fValue.labels.length > dIndex){
               jsValue.desc = decodeURI(fValue.labels[dIndex]);
            }
        	
            var Type = bobj.crv.params.DataTypes;
            switch (type) {
                case Type.DATE:
                case Type.TIME:
                case Type.DATE_TIME:
                    jsValue.value = this._convertDateTimeFlexValue(fValue.value, type);
                    break;
                default:
                    jsValue.value = decodeURI(fValue.value);
                    break;
            }   
            
        } else {
            // Range
            if (fValue.start){
                jsValue.lowerBoundType = fValue.start.inc == true ? 2 : 1;
                jsValue.beginValue = this._convertFlexValue(fValue.start, type);
            } else {
                jsValue.lowerBoundType = 0;
            }

            if (fValue.end){
                jsValue.upperBoundType = fValue.end.inc == true ? 2 : 1;
                jsValue.endValue = this._convertFlexValue(fValue.end, type);
            } else {
                jsValue.upperBoundType = 0;
            }
        }

        return jsValue;
    },

    _convertDateTimeFlexValue : function (fValue, type) {
        var Type = bobj.crv.params.DataTypes;
        var dValue = {};
        var parts = fValue.split(',');
        switch(type) {
            case Type.DATE:
                dValue.y = parseInt(parts[0].substring(5), 10);
                dValue.m = parseInt(parts[1], 10) - 1;
                dValue.d = parseInt(parts[2].substring(parts[2].length - 1, 0), 10);
                break;
                
            case Type.TIME:
                dValue.h = parseInt(parts[0].substring(5), 10);
                dValue.min = parseInt(parts[1], 10);
                dValue.s = parseInt(parts[2].substring(parts[2].length - 1, 0), 10);
                dValue.ms = 0;
                break;
                
            case Type.DATE_TIME:
                dValue.y = parseInt(parts[0].substring(9), 10);
                dValue.m = parseInt(parts[1], 10) - 1;
                dValue.d = parseInt(parts[2], 10);
                dValue.h = parseInt(parts[3], 10);
                dValue.min = parseInt(parts[4]);
                dValue.s = parseInt(parts[5].substring(parts[5].length - 1, 0), 10);
                dValue.ms = 0;
                break;
        }
        return dValue;
    },

        
    logon : function(id, logonData) {
        bobj.crv.logger.info('logging on');
        this.closeDialog(id);
        bobj.event.publish('crprompt_flexlogon', id, logonData);
    },
    
    processingCancel : function(id) {
        var v = getWidgetFromID(id);
        if (v && v._reportProcessing) {
            v._reportProcessing.cancelShow();
        }
    },
    
    processingDelayedShow : function(id) {
        var v = getWidgetFromID(id);
        if (v && v._reportProcessing) {
            v._reportProcessing.delayedShow();
        }
    },
    
    logger : function(text) {
        bobj.crv.logger.info(text);
    },
    
    getSWFBaseURL : function() {
        return bobj.crvUri("../../swf/");
    },
    
    getSWFID : function() {
        return bobj.uniqueId();
    },
    
    getZIndex : function() {
        return bobj.constants.modalLayerIndex;
    },
    
    getUseSavedData : function(id) {
        return this.hasIParamPromptUnitData(id);
    },
    
    closeDialog : function(id) {
        var v = getWidgetFromID(id);
        if (v) {
            v.hideFlexPromptDialog();
        }
    },
    
    getUseOKCancelButtons : function(id) {
        return this.hasIParamPromptUnitData(id);
    },
    
    getIsDialog : function(id) {
        return true;
    },
    
    getShouldAutoResize : function(id) {
        return true;
    },
    
    setVisibility : function(id) {
        var swf = bobj.crv.params.FlexParameterBridge.getSWF(id);
        if (swf) {
            var P_STYLE = swf.parentNode.style;
            
            P_STYLE.position = 'absolute';
            P_STYLE.visibility = 'visible';
            P_STYLE.borderStyle = 'none';
            P_STYLE.opacity = 1;

            if (swf.focus !== undefined) {
                swf.focus();
            }
        }
    },
    
    getReportStateInfo : function(id) {
        var s = bobj.crv.stateManager.getComponentState(id);
        if (s && s.common && s.common.reqCtx) {
            return MochiKit.Base.serializeJSON(s.common.reqCtx);
        }
    },
    
    setReportStateInfo : function(id, rsInfo) {
        var s = bobj.crv.stateManager.getComponentState(id);
        if (s && s.common && s.common.reqCtx) {
            s.common.reqCtx = MochiKit.Base.evalJSON(unescape(rsInfo));
        }
    },

    sendAsyncRequest : function(id, args) {        
        bobj.event.publish('crprompt_asyncrequest', id, args);    
    },
    
    readyToShow: function(id) {
    	this.processingCancel(id);
    }    
};
