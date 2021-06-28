/* Copyright (c) Business Objects 2006. All rights reserved. */




/**
 * Abstract base class. IOAdapters allow the ViewerListener to request data from 
 * a server without knowing the details of how a particular framework requires
 * the request to be made.
 */
bobj.crv.IOAdapterBase = {
    /**
     * Send a viewer request to the Server.
     * 
     * @param pageState   [Object] The composite view state for all viewers on the page
     * @param viewerName  [String] The name of the viewer that should handle the request
     * @param eventArgs   [Object] Event arguements
     * @param allowAsynch [bool]   True if asynchronous requests are allowed
     * 
     * @return [MochiKit.Async.Deferred] Returns a Deferred if an asynchronous
     *       request is pending.
     */
    request: function() {},
    
    /**
     * Add a parameter to server requests.
     * 
     * @param fldName  [String]  Name of the parameter
     * @param fldValue [String]  Value of the parameter
     */
    addRequestField: function(fldName, fldValue) {},
    
     /**
     * Remove a parameter from server requests.
     * 
     * @param fldName  [String]  Name of the parameter
     */
    removeRequestField: function(fldName) {},
    
    /**
     * Save the page state. Persist the state in a manner apropriate
     * for the framework.
     *
     * @param pageState   [Object] The composite view state for all viewers on the page
     * @param viewerName  [String] The name of the viewer that making the request
     */
    saveViewState: function(pageState, viewerName) {},
    
    /**
     * Get the postback data queryString to use for Active X print control
     *
     * @param pageState   [Object] The composite view state for all viewers on the page
     * @param viewerName  [String] The name of the viewer that making the request
     * @return [String] the postback data in a query string format
     */
    getPostDataForPrinting: function(pageState, viewerName) {},

    /**
     * Allows the IOAdapter to manipulate the error before the Viewer processes it for display to the user.
     */
    processError: function(response) {return response;},
    
    canUseAjax: function() {
        try {
            return (MochiKit.Async.getXMLHttpRequest() !== null);
        }
        catch (e) {
            return false;
        }
    },
    
    /**
     * Ensures that there is a hidden iframe to be used for postbacks.
     */
    _getPostbackIframe: function() {
        if (!this._iframe) {
            ifrm = document.createElement("IFRAME");
            ifrm.id = bobj.uniqueId();
            ifrm.name = ifrm.id;
            ifrm.style.width = '0px';
            ifrm.style.height = '0px';
            ifrm.style.position = 'absolute';
            ifrm.style.top = '0px';
            ifrm.style.left = '0px';
            ifrm.style.visibility = 'hidden';
            document.body.appendChild(ifrm);
            // in IE, we need to set the window name again
            if (!ifrm.contentWindow.name) {
                ifrm.contentWindow.name = ifrm.id;
            }
            
            this._iframe = ifrm;
        }
        
        return this._iframe;
    }
};

/*
================================================================================
ServletAdapter. ServletAdapter issues requests to the Java DHTML viewer.
================================================================================
*/

/**
 * Constructor for ServletAdapter. 
 *
 * @param pageUrl [string]  URL of the page 
 * @param servletUrl [string]  URL to which requests to a servlet should be made
 *                             It doubles as the url for all asyncronous requests
 */
bobj.crv.ServletAdapter = function(pageUrl, servletUrl) {
    this._pageUrl = pageUrl;
    this._servletUrl = servletUrl;
    this._form = null;
};

bobj.crv.ServletAdapter._requestParams = {
    STATE: 'CRVCompositeViewState',
    TARGET: 'CRVEventTarget',
    ARGUMENT: 'CRVEventArgument'
};
    

bobj.crv.ServletAdapter.prototype = MochiKit.Base.merge(bobj.crv.IOAdapterBase, {
    
    request: function(pageState, viewerName, eventArgs, allowAsync, useIframe) {
        if (!this._form) {
            this._createForm();
        }
        
        var rp = bobj.crv.ServletAdapter._requestParams;
        var toJSON = MochiKit.Base.serializeJSON;
        
        this._form[rp.STATE].value = encodeURIComponent(toJSON(pageState));  
        this._form[rp.TARGET].value = encodeURIComponent(viewerName);
        this._form[rp.ARGUMENT].value = encodeURIComponent(toJSON(eventArgs));
        
        var deferred = null;
        if (allowAsync && this._servletUrl) {
            var req = MochiKit.Async.getXMLHttpRequest();
            req.open("POST", this._servletUrl, true);
            req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            req.setRequestHeader('Accept','application/json');
            deferred = MochiKit.Async.sendXMLHttpRequest(req, MochiKit.Base.queryString(this._form));
        }
        else {
            if (useIframe) {
                this._form.target = this._getPostbackIframe().id;
            }
            this._form.submit();
        }

        // once the form is submitted it is not intended to be reused.
        
        MochiKit.DOM.removeElement(this._form);
        this._form = null;
        return deferred;
    },

    redirectToServlet: function () {
        if (!this._form) {
            this._createForm();
        }

        this._form.action = this._servletUrl;
    },

    _createForm: function() {
        var d = MochiKit.DOM;
        var rp = bobj.crv.ServletAdapter._requestParams;
        
        this._form = d.FORM({
                name: bobj.uniqueId(), 
                style: 'display:none',
                method: 'POST',
                enctype: 'application/x-www-form-urlencoded;charset=utf-8',
                action: this._pageUrl},
            d.INPUT({type: 'hidden', name: rp.STATE}),
            d.INPUT({type: 'hidden', name: rp.TARGET}),
            d.INPUT({type: 'hidden', name: rp.ARGUMENT}));
            
        document.body.appendChild(this._form);
    },
    
    addRequestField: function(fldName, fldValue) {
        if (fldName && fldValue) {
            if (!this._form) {
                this._createForm();
            }
            
            var existingElem = this._form[fldName];
            if (existingElem) {
                existingElem.value = fldValue;
            }
            else {
                this._form.appendChild(MochiKit.DOM.INPUT({type: 'hidden', name:fldName, value:fldValue}));
            }
        }
    },
    
    removeRequestField: function(fldName) {
        if (fldName) {
            var form = this._form;
            
            if (form) {            
                var existingElem = form[fldName];
                if (existingElem) {
                    MochiKit.DOM.removeElement(existingElem);
                    if(form[fldName]) { // Fix for FF
                        form[fldName] = null;
                    }
                }
                
                existingElem = null;
            }
        }
    },
  
    getPostDataForPrinting: function(pageState, viewerName) {
        var toJSON = MochiKit.Base.serializeJSON;
        var rp = bobj.crv.ServletAdapter._requestParams;
        var state = toJSON(pageState);
        
        var postData = {};
        postData[rp.STATE] = encodeURIComponent(state);
        postData[rp.TARGET] = encodeURIComponent(viewerName);
        postData[rp.ARGUMENT] = encodeURIComponent('"axprint="');
        if(document.getElementById('com.sun.faces.VIEW')) {
            postData['com.sun.faces.VIEW'] = encodeURIComponent(document.getElementById('com.sun.faces.VIEW').value);
        }
        
        return MochiKit.Base.queryString(postData);
    },
    
    processError: function(response) { 
        if (!(typeof(response.number) == 'undefined') && response.number == 404) {
            return L_bobj_crv_ServletMissing;
        }
        
        return response;
    }
});

/*
================================================================================
AspDotNetAdapter. AspDotNetAdapter issues requests to the WebForm DHTML viewer.
================================================================================
*/

/**
 * Constructor for AspDotNetAdapter. 
 *
 * @param postbackEventReference [string]  The full functiona call to the ASP.NET dopostback function
 * @param replacementParameter [string] the string to replace in the dopostback function
 * @param stateID [string] the name of the input field to save the state to 
 */
bobj.crv.AspDotNetAdapter = function(postbackEventReference, replacementParameter, stateID, callbackEventReference, aspnetVersion) {
    this._postbackEventReference = postbackEventReference;
    this._replacementParameter = replacementParameter;
    this._stateID = stateID;
    this._aspnetVersion = aspnetVersion;
    this._form = null;
    this._callbackEventReference = callbackEventReference;
    this._additionalReqFlds = null;
    var tmpState = bobj.getElementByIdOrName(this._stateID);
    if (tmpState) {
        this._form = tmpState.form;
    }
    
    if(this._isAspNetVersionPriorToVersion4()) {
        WebForm_CallbackComplete = this.WebForm_CallbackComplete;
    }
};


bobj.crv.AspDotNetAdapter.prototype = MochiKit.Base.merge(bobj.crv.IOAdapterBase, {
    
    request: function(pageState, viewerName, eventArgs, allowAsync, useIframe, callbackHandler, errbackHandler) {
        var toJSON = MochiKit.Base.serializeJSON;
        if (eventArgs && this._additionalReqFlds) {
            eventArgs = MochiKit.Base.update(eventArgs, this._additionalReqFlds);
        }
        this._additionalReqFlds = null;
        
        var jsonEventArgs = toJSON(eventArgs);
        this.saveViewState(pageState, viewerName);

        if (allowAsync) {
            if(typeof WebForm_InitCallback == "function") {
                __theFormPostData = ""; //Used by WeForm_InitCallback
                __theFormPostCollection = [];  //Used by WeForm_InitCallback
                WebForm_InitCallback(); //Need to call this to work around a problem where ASP.NET2 callback does not collect form data prior to request
            }
            var callback = this._callbackEventReference.replace("'arg'", "jsonEventArgs");
            callback = callback.replace("'cb'", "callbackHandler");
            callback = callback.replace("'errcb'", "errbackHandler");
            callback = callback.replace("'frmID'", "this._form.id");
            return eval(callback);
        }
        else {
            if (useIframe) {
                this._form.target = this._getPostbackIframe().id;
            }
            
            var postbackCall;
            if(this._postbackEventReference.indexOf("'" + this._replacementParameter + "'") >= 0){
                postbackCall = this._postbackEventReference.replace("'" + this._replacementParameter + "'", "jsonEventArgs");
            }
            else {
                postbackCall = this._postbackEventReference.replace('"' + this._replacementParameter + '"', "jsonEventArgs");
            }
            eval(postbackCall);
            this._clearEventFields();
            this._form.target = ""; //Give back the target of the form after using, or the form cannot get focused
        }
    },
    
    /**
     * @return true if aspnet verion is prior to version 4
     */
    _isAspNetVersionPriorToVersion4 : function () {
        if(this._aspnetVersion != null) {
            var sep = this._aspnetVersion.split(".");
            if(eval(sep[0]) < 4) 
                return true;
        }
        
        return false;
    },
    
    saveViewState: function(pageState, viewerName) {
        var toJSON = MochiKit.Base.serializeJSON;
        var viewState = pageState[viewerName];
        var state = bobj.getElementByIdOrName(this._stateID);
        if (state) {
            state.value = toJSON(viewState);
        }
    },
    
    getPostDataForPrinting: function(pageState, viewerName) {
        this.saveViewState(pageState, viewerName);
        var nv = MochiKit.DOM.formContents(this.form);
        var names = nv[0];
        var values = nv[1];
        names.push('crprint');
        values.push(viewerName);
        var queryString = MochiKit.Base.queryString(names, values);
        return queryString;
    },
    
    addRequestField: function(fldName, fldValue) {
        if (!this._additionalReqFlds) {
            this._additionalReqFlds = {};
        }
        this._additionalReqFlds[fldName] = fldValue;
        
        /*if (fldName && fldValue) {
            var existingElem = this._form[fldName];
            if (existingElem) {
                existingElem.value = fldValue;
            }
            else {
                this._form.appendChild(MochiKit.DOM.INPUT({type: 'hidden', name:fldName, value:fldValue}));
            }
        }*/
    },
        
    _clearRequestField: function(fldName) {
        if (fldName) {
            if (this._form) {            
                var existingElem = this._form[fldName];
                if (existingElem) {
                    existingElem.value = "";
                }
            }
        }
    },
    
    _clearEventFields: function() {
        this._clearRequestField("__EVENTTARGET");
        this._clearRequestField("__EVENTARGUMENT");
    },
    
    /*
     * Overriding WebForm_CallbackComplete provided by .Net framework 2(WebResource.asx) as it was incorrect.
     * The code below is taken from .Net 4 copy of this function
     * This code can be removed once the support for .Net 2 is terminated
     */
    WebForm_CallbackComplete : function () {
        for (var i = 0; i < __pendingCallbacks.length; i++) {
            callbackObject = __pendingCallbacks[i];
            if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
                if (!__pendingCallbacks[i].async) {
                    __synchronousCallBackIndex = -1;
                }
                __pendingCallbacks[i] = null;
                var callbackFrameID = "__CALLBACKFRAME" + i;
                var xmlRequestFrame = document.getElementById(callbackFrameID);
                if (xmlRequestFrame) {
                    xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
                }
                WebForm_ExecuteCallback(callbackObject);
            }
        }

    }
});

/*
================================================================================
FacesAdapter. FacesAdapter issues requests to a JSF viewer component. 
================================================================================
*/

/**
 * Constructor for FacesAdapter. 
 *
 * @param formName [string]  Name of the form to submit
 * @param servletUrl [string]  URL to which requests to a servlet should be made
 *                             It doubles as the url for all asyncronous requests
 */
bobj.crv.FacesAdapter = function(formName, servletUrl) {
    this._formName = formName;
    this._servletUrl = servletUrl;
    this._useServlet = false;
    if (!bobj.crv.FacesAdapter._hasInterceptedSubmit) { 
        this._interceptSubmit();
        bobj.crv.FacesAdapter._hasInterceptedSubmit = true;
    }
};

bobj.crv.FacesAdapter._requestParams = {
    STATE: 'CRVCompositeViewState',
    TARGET: 'CRVEventTarget',
    ARGUMENT: 'CRVEventArgument'
};

bobj.crv.FacesAdapter.prototype = MochiKit.Base.merge(bobj.crv.IOAdapterBase, {
    
    request: function(pageState, viewerName, eventArgs, allowAsync, useIframe) {
        
        var rp = bobj.crv.FacesAdapter._requestParams;
        var toJSON = MochiKit.Base.serializeJSON;
        var INPUT =  MochiKit.DOM.INPUT;
        
        var deferred = null;
        
        var form = this._getForm();
        if (!form) {
            return;
        }
        
        if (!form[rp.TARGET]) {
            form.appendChild( INPUT({type: 'hidden', name: rp.TARGET}) );    
        }
        form[rp.TARGET].value = encodeURIComponent(viewerName);
        
        if (!form[rp.ARGUMENT]) {
            form.appendChild( INPUT({type: 'hidden', name: rp.ARGUMENT}) );
        }
        form[rp.ARGUMENT].value = encodeURIComponent(toJSON(eventArgs));
        
        if (!form[rp.STATE]) {
            form.appendChild( INPUT({type: 'hidden', name: rp.STATE}) );
        }
        form[rp.STATE].value = encodeURIComponent(toJSON(pageState));  
        
        if (allowAsync && this._servletUrl) {
            var req = MochiKit.Async.getXMLHttpRequest();
            req.open("POST", this._servletUrl, true);
            req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            req.setRequestHeader('Accept','application/json');
            deferred = MochiKit.Async.sendXMLHttpRequest(req, MochiKit.Base.queryString(form));
        }
        else {
            var pageUrl = form.action;
            if (this._useServlet === true) {
                form.action = this._servletUrl;
            }
            
            var origTarget = form.target;
            if (useIframe) {
                form.target = this._getPostbackIframe().id;
            }
            
            form.submit();
            
            form.action = pageUrl;
            form.target = origTarget;
            this._useServlet = false;
        }
        
        // clear out the request fields so that the form can be reused
        form[rp.TARGET].value = "";
        form[rp.ARGUMENT].value = "";
        form[rp.STATE].value = "";
        
        // TODO Post Titan: we shouldn't need to have explicit knowledge of this parameter
        this.removeRequestField ('ServletTask');
        
        return deferred;
    },
    
    redirectToServlet: function () {
        this._useServlet = true;
    },
    
    addRequestField: function(fldName, fldValue) {
        if (fldName && fldValue) {
            var form = this._getForm();
            
            if (form) {            
                var existingElem = form[fldName];
                if (existingElem) {
                    existingElem.value = fldValue;
                }
                else {
                    form.appendChild(MochiKit.DOM.INPUT({type: 'hidden', name:fldName, value:fldValue}));
                }
            }
        }
    },
    
    removeRequestField: function(fldName) {
        if (fldName) {
            var form = this._getForm();
            
            if (form) {            
                var existingElem = form[fldName];
                if (existingElem) {
                    MochiKit.DOM.removeElement(existingElem);
                    if(form[fldName]) { // Fix for FF
                        form[fldName] = null;
                    }
                }
                
                existingElem = null;
            }
        }
    },
    
    saveViewState: function(pageState, viewerName) {
        if (!bobj.crv.FacesAdapter._isStateSaved) {
            var form = this._getForm();
            if (form) {
                var rp = bobj.crv.FacesAdapter._requestParams;
                var toJSON = MochiKit.Base.serializeJSON;
                var INPUT =  MochiKit.DOM.INPUT;
                
                if (!form[rp.STATE]) {
                    form.appendChild( INPUT({type: 'hidden', name: rp.STATE}) );
                }
                form[rp.STATE].value = encodeURIComponent(toJSON(pageState));  
            }
            bobj.crv.FacesAdapter._isStateSaved = true;
        }
    },
    
    _getForm: function() {
        return document.forms[this._formName];   
    },
    
    _interceptSubmit: function() {
        var form = this._getForm();
        if (form) {
            var oldSubmit = form.submit;
            form.submit = function() {
                bobj.event.publish('saveViewState');
                form.submit = oldSubmit; // IE needs this
                form.submit(); // Can't apply args because IE misbehaves
            };
        }
    },
      
    getPostDataForPrinting: function(pageState, viewerName) {
        var toJSON = MochiKit.Base.serializeJSON;
        var rp = bobj.crv.ServletAdapter._requestParams;
        var state = toJSON(pageState);
        
        var postData = {};
        postData[rp.STATE] = encodeURIComponent(state);
        postData[rp.TARGET] = encodeURIComponent(viewerName);
        postData[rp.ARGUMENT] = encodeURIComponent('"axprint="');
        if(document.getElementById('com.sun.faces.VIEW')) {
            postData['com.sun.faces.VIEW'] = encodeURIComponent(document.getElementById('com.sun.faces.VIEW').value);
        }
        
        return MochiKit.Base.queryString(postData);
    },
    
    processError: function(response) { 
        if (!(typeof(response.number) == 'undefined') && response.number == 404) {
            return L_bobj_crv_ServletMissing;
        }
        
        return response;
    }      
});