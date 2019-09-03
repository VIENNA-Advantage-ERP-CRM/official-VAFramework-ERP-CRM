/*
================================================================================
ParameterDialog

Advanced Dialog for editing parameters using the prompt engine
================================================================================
*/

bobj.crv.params.newParameterDialog = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        prompt: null,
        showCB : null,
        hideCB : null
    }, kwArgs);  
    
    var o = newDialogBoxWidget(
        kwArgs.id, 
        L_bobj_crv_ParamsDlgTitle, 
        kwArgs.width, 
        kwArgs.height /*,defaultCB,cancelCB,noCloseButton*/);

    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    o._showDialogBox = o.show;
    o._initDialogBox = o.init;
    o._resizeSignal = null;
    MochiKit.Base.update(o, bobj.crv.params.ParameterDialog);
    
    return o;
};

bobj.crv.params.ParameterDialog = {
    init : function() {
        this._initDialogBox ();
        this._form = document.getElementById (this.id + '_form');
    },
    
    _checkInitialization : function() {
        if (!this.layer) {
            targetApp (this.getHTML ());
            this.init ();
        }
    },
    
    show : function(show) {
        if (show) {
            this._checkInitialization ();
            this.doLayout ();
            this.setResize (MochiKit.Base.noop);
            this._showDialogBox (true);
            o._resizeSignal = MochiKit.Signal.connect(window, 'onresize', this, '_onWindowResize');
        } else {
            if (this.layer)
                this._showDialogBox (false);
            bobj.crv.SignalDisposer.dispose(o._resizeSignal, true);
        }

        if (show && this.showCB) {
            this.showCB ();
        } else if (!show && this.hideCB) {
            this.hideCB ();
        }
    },
    
    isVisible : function() {
        return (this.initialized () && this.isDisplayed ());
    },
    
    /* if dialog's height is less than window's height, it would return dialog's height
       if dialog's height is greater than window's height, it would return window's height - 100
       if dialog's height is less than 100, it would return 100
     */
    getPreferredHeight : function () {
        return Math.min(Math.max (100, winHeight() - 100), this.getFormHeight()); 
    },
    
    getFormHeight : function () {
        var form = this._form.cloneNode(true);
        form.style.display = "none";
        form.style.height ="";     
        document.body.appendChild(form);
        var dimension = MochiKit.Style.getElementDimensions(form);
        form.innerHTML = ""; // http://support.microsoft.com/kb/925014
        document.body.removeChild(form);
                
        return dimension.h;
    },
    
    _onWindowResize : function () {
        this.doLayout ();
        this.center();
    },
    
    doLayout: function () {
        if(this._form) {
            this._form.style.height = this.getPreferredHeight() + "px";
        }
    },
    
    updateHtmlAndDisplay : function(html) {
        if (html) {
            this._checkInitialization ();
            
            if (this.isDisplayed ()) {
                this.show (false);
            }

            var ext = bobj.html.extractHtml (html);
            
            var styleText = "";
            for ( var i = 0, len = ext.styles.length; i < len; i++) {
                styleText += ext.styles[i].text + '\n';
            }
            
            var styleLayerID = this.id + '_stylesheet';
            
            var styleLayer = getLayer(styleLayerID);
            if(styleLayer) {
                MochiKit.DOM.removeElement (styleLayer);
            }
            
            if(styleText.length > 0) {
                bobj.addStyleSheet (styleText, styleLayerID);
            }

            if (this._form) {
                this._form.innerHTML = '<div style="overflow:auto">' + ext.html + '</div>';
            }
           
            var callback = function (parameterDialog, scripts) {
                return function () {                  
                    
                    //Must show dialog before executing scripts as scripts could change scroll position
                    parameterDialog.show(true);

                    for ( var iScripts = 0, scriptsLen = scripts.length; iScripts < scriptsLen; ++iScripts) {
                        var script = scripts[iScripts];
                        if (!script) {
                            continue;
                        }

                        if (script.text) {
                            bobj.evalInWindow (script.text);
                        }
                    }
                }
            }(this, ext.scripts);
            
            //CSS links in extracted html are first loaded, then dialog is shown to correctly calcualte
            //the width and height of dialog
            
            bobj.includeCSSLinksAndExecuteCallback ( ext.links, callback);         

        }
    },
        
    getHTML : function(html) {
        var FORM = bobj.html.FORM;  
        var DIV = bobj.html.DIV;
        var onsubmit = 'eventCancelBubble(event);return false;';
        
        return this.beginHTML() +
        DIV({'class' : 'dlgFrame naviBarFrame', style : { padding : '20px 15px 5px 20px'}}, FORM({id: this.id + '_form', style : {overflow : 'auto'}, onsubmit: onsubmit})) +
            this.endHTML();    
    }
};
