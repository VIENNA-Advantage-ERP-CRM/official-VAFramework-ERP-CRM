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

        this.init = function () {
            $root = $('<div style="height:100%;width:100%;padding:15px"></div>');
        };

        this.update = function (record_ID) {
            loadSession(record_ID);
        };

        this.getRoot = function () {
            return $root;
        };

        var loadSession = function (record_ID) {
            $.ajax({
                url: VIS.Application.contextUrl + 'BiPanel/GetUserBILogin',
                success: function (data) {
                    if (data) {
                        data = JSON.parse(data);
                    }
                    if (data.length > 0) {
                        if (data[0] == "1") {
                            VIS.ADialog.error("VA037_BIToolMembership");
                        }
                        else if (data[0] == "2") {
                            VIS.ADialog.error("VA037_BIUrlNotFound");
                        }
                        else if (data[0] == "3") {
                            VIS.ADialog.error("VA037_NotBIUser");
                        }
                        else if (data[0] == "3") {
                            VIS.ADialog.error("VA037_BICallingError");
                        }
                        else {

                            var _src = data[1] + "JsAPI?token=" + data[0] + "&reportUUID=" + extraInfo + "=" + record_ID;
                            $root.html('<iframe src=' + _src + ' ; height=100%; width="100%"; frameborder="0" ></iframe>');
                        }
                    }
                },
                error: function (er) {

                }
            });
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

});