//*** AForm *******//

; (function (VIS, $) {

    function WForm(height, AD_Form_ID, curTab, pWwindowNo) {
        var $root, $busyDiv, $contentGrid, ch;
        var ctx = VIS.Env.getCtx(); //ctx
        var log = VIS.Logging.VLogger.getVLogger("VIS.AForm"); //Logger
        var windowNo = VIS.Env.getWindowNo();
        var self = this;
        //InitComponenet
        function initComponent() {
            $root = $("<div class='vis-height-full'>");
            $busyDiv = $("<div class='vis-apanel-busy'>");
            $contentGrid = $("<div class='vis-height-full'>");
            $root.append($contentGrid).append($busyDiv);
            ch = new VIS.ChildDialog();
            getFormDetails();
        };

        initComponent();

        this.getWindowNo = function () {
            return windowNo;
        };

        this.getAD_Table_ID = function () {
            return curTab.getAD_Table_ID();
        }

        this.getAD_Window_ID = function () {
            return curTab.getAD_Window_ID();
        }

        this.getRecord_ID = function () {
            return curTab.getRecord_ID();
        }

        this.getContentGrid = function () { return $contentGrid; };

        this.close = function () {
            if (ch) {
                ch.close();
            }
        };

        this.disposeComponent = function () {
            $root.remove();
            $root = null;
            $contentGrid.remove();
            $contentGrid = null;
            this.getContentGrid = null;
            self = null;
        }

        function getFormDetails() {
            VIS.dataContext.getFormDataString({ AD_Form_ID: AD_Form_ID }, function (json) {
                if (json.error != null) {
                    VIS.ADialog.error(json.error);    //log error
                    self.dispose();
                    self = null;
                    return;
                }

                var jsonData = $.parseJSON(json.result); // widow json
                if (jsonData.IsError) {
                    VIS.ADialog.error(jsonData.Message);    //log error
                    self.dispose();
                    self = null;
                    return;
                }
                ch.setTitle(jsonData.DisplayName);
                openForm(jsonData, self, pWwindowNo);
                jsonData = null;
            })
        };

        openForm = function (json, $parent, pWwindowNo) {

            if (json.IsReport) {
                VIS.ADialog.info("Form Report is not supported");
                return false;
            }

            var className = "";

            className = json.Prefix + json.ClassName;

            console.log(className);


            log.info("Form Name= " + json.Name + ", Class=" + className);
            ctx.setWindowContext(windowNo, "WindowName", json.DisplayName);

            try {

                var type = VIS.Utility.getFunctionByName(className, window);
                var o = new type(windowNo);
                o.init(pWwindowNo, self);
                if (o.setRecordID) {
                    o.setRecordID(curTab.getRecord_ID());
                }
                showForm(o);
            }
            catch (e) {
                log.log(VIS.Logging.Level.WARNING, "Class=" + className + ", AD_Form Name=" + json.Name, e)
                return false;
            }
            return true;
        };

        function showForm(type) {

            ch.setContent($contentGrid);
            if (type.setHeight) {
                ch.setHeight(type.setHeight());
            }
            else {
                ch.setHeight(height);
            }

            if (type.setWidth) {
                ch.setWidth(type.setWidth());
            }
            else {
                ch.setWidth(window.innerWidth - 100);
            }

            ch.setModal(true);
            ch.onClose = function () {

                // invoke dispose event of form if exist.


                if (type.dispose) {
                    type.dispose();
                }

                if (ch.dispose) {
                    ch.dispose();
                }
                // Refresh current record of window..
                curTab.dataRefresh();

                // dispose self..
                if (self.disposeComponent) {
                    self.disposeComponent();
                }
            };
            ch.show();
            ch.hidebuttons();
        };
    };

    VIS.WForm = WForm;

})(VIS, jQuery);