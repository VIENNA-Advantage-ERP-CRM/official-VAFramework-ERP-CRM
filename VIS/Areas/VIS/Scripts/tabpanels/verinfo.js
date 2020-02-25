VIS = window.VIS || {};

(function () {
    function verinfo() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;
        var $root = $('<div class="vis-mas-verinf-wrap"></div>');
        var cntnrDiv = null;

        this.init = function () {
            var html = ('<div class="vis-mas-verinf-hdrCtr">'
                + '<div class="vis-mas-verinf-hdrsLft">'
                + VIS.Msg.getMsg("FieldName")
                + '</div>'
                + '<div class="vis-mas-verinf-hdrsRgt">'
                + VIS.Msg.getMsg("OldValue")
                + '</div>'
                + '</div>'
                + '<div class="vis-mas-verinf-CngCtr">'
                + '</div>'
            );
            $root.append(html);
            cntnrDiv = $root.find('.vis-mas-verinf-CngCtr');
        };

        /*
       * Retrun container of panel's Design
       */
        this.getRoot = function () {
            return $root;
        };

        /*
        * Update UI elements with latest record's values.
        */
        this.update = function (record_ID) {
            // Get Value from Context
            try {
                if (this.selectedRow.recordversion > 1) {
                    this.selectedRow.RID = this.record_ID;
                    this.selectedRow.TName = this.curTab.vo.TableName;
                    this.selectedRow.TabID = this.curTab.vo.AD_Tab_ID;
                    this.selectedRow.TblID = this.curTab.vo.AD_Table_ID;
                    var self = this;
                    $.ajax({
                        type: 'POST',
                        async: true,
                        url: VIS.Application.contextUrl + "JsonData/GetVerInfo",
                        contentType: "application/json; charset=utf-8",
                        data: "{ 'RowData': '" + JSON.stringify(this.selectedRow) + "' }",
                        success: function (data) {
                            var cT = self.curTab;
                            cntnrDiv.empty();
                            if (data) {
                                var retRes = JSON.parse(data);
                                if (retRes.ColumnNames && retRes.ColumnNames.length > 0) {
                                    var htmlUI = new Array();
                                    for (var i = 0; i < retRes.ColumnNames.length; i++) {
                                        var displayType = 10;
                                        try {
                                            displayType = $.grep(self.curTab.gridTable.gTable.m_fields, function (e) { return e._vo.ColumnName == retRes.DBColNames[i] })[0]._vo.displayType;
                                        }
                                        catch (e) {
                                            displayType = 10;
                                        }
                                        var colVal = retRes.OldVals[i];
                                        if (displayType == 15)
                                            colVal = Globalize.format(new Date(colVal), 'd');
                                        else if (displayType == 16)
                                            colVal = Globalize.format(new Date(colVal + "Z"), 'f');
                                        else if (displayType == 24)
                                            colVal = Globalize.format(new Date(colVal + "Z"), 't');
                                        if (retRes.OldVals[i] == "")
                                            colVal = '&nbsp';
                                        htmlUI.push('<div class="vis-mas-verinf-cntCtr">'
                                            + '<div class="vis-mas-verinf-cnt">'
                                            + retRes.ColumnNames[i]
                                            + '</div>'
                                            + '<div class="vis-mas-verinf-cnt">'
                                            + colVal
                                            + '</div>'
                                            + '</div>');
                                    }
                                    cntnrDiv.append(htmlUI.join(' '));
                                }
                            }
                        },
                        error: function (e) {
                            var y = "";
                        }
                    });
                }
                else if (this.selectedRow.recordversion == 1) {
                    cntnrDiv.empty();
                    cntnrDiv.append('<div class="vis-mas-verinf-newrecord">' + VIS.Msg.getMsg("FirstVersion")+'</div>');
                }
                else {
                    cntnrDiv.empty();
                    cntnrDiv.append('<div class="vis-mas-verinf-newrecord">' + VIS.Msg.getMsg("NoVersionsFound") + '</div>');
                }
            }
            catch (ex) {
            }
        };

        this.disposeComponent = function () {
            this.record_ID = 0;
            this.windowNo = 0;
            this.curTab = null;
            this.rowSource = null;
            this.panelWidth = null;
            $root.remove();
            $root = null;
        };
    };

    /**
     *	Invoked when user click on panel icon
     */
    verinfo.prototype.startPanel = function (windowNo, curTab) {
        this.windowNo = windowNo;
        this.curTab = curTab;
        this.init();
    };

    /**
         *	This function will execute when user navigate  or refresh a record
         */
    verinfo.prototype.refreshPanelData = function (recordID, selectedRow) {
        this.record_ID = recordID;
        this.selectedRow = selectedRow;
        this.update(recordID);
    };

    /**
     *	Fired When Size of panel Changed
     */
    verinfo.prototype.sizeChanged = function (height, width) {
        this.panelWidth = width;
    };

    /**
     *	Dispose Component
     */
    verinfo.prototype.dispose = function () {
        this.disposeComponent();
    };

    /*
    * Fully qualified class name
    */
    VIS.verinfo = verinfo;
})();