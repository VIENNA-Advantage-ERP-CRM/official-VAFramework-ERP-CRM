//*** AForm *******//

; (function (VIS, $) {

    /**
     *	Form Framework
     *
     */
    function AForm(height) {

        this.$parent; //parent container
        this.mPanel; // content panel
        this.ctx = VIS.Env.getCtx(); //ctx
        this.log = VIS.Logging.VLogger.getVLogger("VIS.AForm"); //Logger

        var $root, $busyDiv, $contentGrid;

        //InitComponenet
        function initComponent() {
            $root = $("<div class='vis-height-full vis-app-aform-root' >");
            $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            $contentGrid = $("<div class='vis-height-full'>");
            $root.append($contentGrid).append($busyDiv);  
        };

        initComponent();

        this.setSize = function (height, width) {
             $root.height(height); //height 
            //$busyDiv.height(height);
             $contentGrid.height(height);
        };
        this.setSize(height);

        //privilized function

        this.getRoot = function () { return $root; };
        this.getContentGrid = function () { return $contentGrid; };
        this.setBusy = function (busy, focus) {
            isLocked = busy;
            if (busy) {
                $busyDiv[0].style.visibility = 'visible';// .show();
            }
            else {
                $busyDiv[0].style.visibility = 'hidden';
                if (focus) {
                }
            }
        };

        this.disposeComponent = function () {
            $root.remove();
            $root = null;
            $contentGrid.remove();
            $contentGrid = null;
            $busyDiv.remove();
            $busyDiv = null;
            this.$parentWindow = null;
            this.ctx = null;

            this.getRoot = null;
            this.getContentGrid = null;
            this.setBusy = null;
            this.setSize = null;
        }
    };

    /**
	 * 	Open Form (directly)
	 *	@param jObject form object
	 *	@param $parent parent control
	 *	@param windowNo window number 
	 *	@return true if started
	 */
    AForm.prototype.openForm = function (json, $parent, windowNo, additionalInfo) {

        if (json.IsReport) {
            VIS.ADialog.info("Form Report is not supported");
            return false;
        }


        this.parent = $parent;


        var className = "";

        //if (json.Prefix.length > 0) // has Mod
        //{
        //    className = json.Prefix + json.ClassName;
        //}
        //else {
        //    //check for VAdv

        //    className = "VIS" + json.ClassName;
        //}

        //className = json.Prefix + json.ClassName;
        className = json.Prefix + json.ClassName;

        console.log(className);


        this.log.info("Form Name= " + json.Name + ", Class=" + className);
        this.ctx.setWindowContext(windowNo, "WindowName", json.DisplayName);

        try {

            //className = "VIS.Apps.TestForm";
            var type = VIS.Utility.getFunctionByName(className, window);
            var o = new type(windowNo);
            o.init(windowNo, this, additionalInfo);
            this.mPanel = o;
            o = null;
        }
        catch (e) {
           
                this.log.log(VIS.Logging.Level.WARNING, "Class=" + className + ", AD_Form Name=" + json.Name, e)
                return false;
           
            
        }

        //this.getRoot().html("Form _ID=>" + json.ClassName);

       
        this.setBusy(false);
        this.windowNo = windowNo;


        this.setTitle(VIS.Env.getHeader(this.ctx, windowNo));
        this.parent.setName(json.DisplayName);

        $parent = null;

        return true;
    };
    /**
    *  set title of form
    *  @param title 
    */
    AForm.prototype.setTitle = function (title) {
        if (this.parent)
            this.parent.setTitle(VIS.Utility.Util.cleanMnemonic(title));
    };

    AForm.prototype.hideHeader = function (hide) {
        if (this.parent)
            this.parent.hideHeader(hide);
    };



    AForm.prototype.hideCloseIcon = function (hide) {
        if (this.parent)
            this.parent.hideCloseIcon(hide);
    };

    AForm.prototype.sizeChanged = function (height, width) {
        this.setSize(height);
        if (this.mPanel && this.mPanel.sizeChanged) {
            this.mPanel.sizeChanged(height, width);
        }
    };

    AForm.prototype.refresh = function () {
        if (this.mPanel && this.mPanel.refresh) {
            this.mPanel.refresh();
        }
    };



    /**
    * 	dispose
    */
    AForm.prototype.dispose = function () {
        if (this.disposed)
            return;
        this.disposed = true;
        if (this.mPanel) {
            if (this.mPanel.disposeComponent)
                this.mPanel.dispose();
            this.mPanel = null;
        }

        if (this.parent) {
            this.parent.dispose();
            this.parent = null;
        }
        VIS.Env.clearWinContext(this.ctx, this.windowNo);

        this.disposeComponent();
    };



    




    // global assignment
    VIS.AForm = AForm;

})(VIS, jQuery);