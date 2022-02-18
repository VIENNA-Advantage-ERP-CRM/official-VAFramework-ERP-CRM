VIS = window.VIS || {};
(function () {

    function BiPanel() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;
        this.extraInfo = null;
        var $root;
        var iFrame;

        this.init = function () {
            $root = $('<div style="height:100%;width:100%"></div>');
            if (window.VA037) {
                iFrame = $('<iframe height=100%; width="100%"; frameborder="0" ></iframe>');
                $root.html(iFrame);
            }
            else {
                var $span = $('<span class="vis-bi-tab-panel">' + VIS.Msg.getMsg("PleaseInstallBIReportTool") + '</span>');
                $root.append($span);
            }
        };

        this.update = function (record_ID) {
            loadSession(record_ID);
        };

        this.getRoot = function () {
            return $root;
        };
        var that = this;




        var loadSession = function (record_ID) {
            if (!iFrame)
                return;
            //$root.empty();VIS.Utility.encodeText(mails)
            var extraInfo = that.extraInfo;
            var parm = extraInfo.match(/\@.+?\@/g);
            if (parm) {
                for (var i = 0; i < parm.length; i++) {
                    var key = parm[i].replaceAll("@", "")
                    if (VIS.Env.getCtx().getContext(key).length > 0) {
                        key = VIS.Env.getCtx().getContext(key);
                    }
                    else if (VIS.Env.getCtx().getContext(that.windowNo, key).length > 0) {
                        key = VIS.Env.getCtx().getContext(that.windowNo, key);
                    } else if (VIS.Env.getCtx().getContext(that.windowNo, that.curTab().getTabNo(), key).length > 0) {
                        key = VIS.Env.getCtx().getContext(that.windowNo, that.curTab().getTabNo(), key);
                    }
                    extraInfo = extraInfo.replaceAll(parm[i], key);
                }
            }

            var _src = VIS.Application.contextUrl + "BiPanel/GetUserBILogin?recID=" + record_ID + "&extraInfo=" + window.encodeURIComponent(extraInfo);
            //Frame = $('<iframe height=100%; width="100%"; frameborder="0" ></iframe>');
            //$root.html('<iframe src="'+_src+'"; height=100%; width="100%"; frameborder="0" ></iframe>');
            iFrame.prop('src', _src);
        };

        this.disposeComponent = function () {
            if (iFrame)
                iFrame.remove();
            $root.remove();
            iFrame = null;
            $root = null;
        };

    };

    /**
     *	Invoked when user click on panel icon
     */
    BiPanel.prototype.startPanel = function (windowNo, curTab, extraInfo) {
        this.windowNo = windowNo;
        this.curTab = curTab;
        this.extraInfo = extraInfo;
        this.init();
    };

    /**
         *	This function will execute when user navigate  or refresh a record
         */
    BiPanel.prototype.refreshPanelData = function (recordID, selectedRow) {
        this.record_ID = recordID;
        this.selectedRow = selectedRow;

        this.update(recordID);
    };

    /**
     *	Fired When Size of panel Changed
     */
    BiPanel.prototype.sizeChanged = function (width) {
        this.panelWidth = width;
    };

    /**
     *	Dispose Component
     */
    BiPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    /*
    * Fully qualified class name
    */
    VIS.BiPanel = BiPanel;

})();