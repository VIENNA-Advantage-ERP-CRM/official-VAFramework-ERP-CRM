
VIS = window.VIS || {};
(function () {
    function TestPanel() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;
        var $lblName = null;
        var $lblC_Location = null;
        var $lblBPartner = null;
        var $lblDateTime = null;
        var $lblAmount = null;
        var $root = $('<div style="height:100%;width:100%;padding:15px"></div>');

        /*
        *   Intialize UI Elements
        */
        this.init = function () {

            var html = '<div class="vis-testPanel-Div"><span class="vis-testPanel-Span">Name: </span>'
                     + '<label class="vis-testPanel-Label" id="' + this.windowNo + 'lblName"></label> </div>'
             + '<div class="vis-testPanel-Div"><span class="vis-testPanel-Span">Location: </span>'
            + '<label class="vis-testPanel-Label" id="' + this.windowNo + 'lblLocation"></label> </div>'
             + '<div class="vis-testPanel-Div"><span class="vis-testPanel-Span">Business Partner: </span>'
           + '<label class="vis-testPanel-Label" id="' + this.windowNo + 'lblBPartner"></label> </div>'
            + '<div class="vis-testPanel-Div"><span class="vis-testPanel-Span">Date Time: </span>'
           + '<label class="vis-testPanel-Label" id="' + this.windowNo + 'lblDateTime"></label> </div>'
            + '<div class="vis-testPanel-Div"><span class="vis-testPanel-Span">Amount: </span>'
           + '<label class="vis-testPanel-Label" id="' + this.windowNo + 'lblAmount"></label> </div>';

            $root.append(html);
            $lblName = $root.find('#' + this.windowNo + 'lblName');
            $lblC_Location = $root.find('#' + this.windowNo + 'lblLocation');
            $lblBPartner = $root.find('#' + this.windowNo + 'lblBPartner');
            $lblDateTime = $root.find('#' + this.windowNo + 'lblDateTime');
            $lblAmount = $root.find('#' + this.windowNo + 'lblAmount');
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
            $lblName.text(VIS.context.getWindowContext(this.windowNo, "Name"));
            try{
                // Get Value for lookup Fields
                var columnName = "C_Location_ID";
                var fieldValue = this.selectedRow[columnName.toLower()];
                // Check if field is of lookup Type
                if (VIS.DisplayType.IsLookup(this.curTab.getField(columnName).getDisplayType()) ||
                    VIS.DisplayType.Location == this.curTab.getField(columnName).getDisplayType()) {
                    if (fieldValue) {
                        fieldValue = this.curTab.getField(columnName.toLower()).
                            lookup.getDisplay(fieldValue);
                    }
                    $lblC_Location.text(fieldValue);
                }

                fieldValue = null;
                var columnName = "C_BPartner_ID";
                fieldValue = this.selectedRow[columnName.toLower()];
                // Check if field is of lookup Type
                if (VIS.DisplayType.IsLookup(this.curTab.getField(columnName).getDisplayType())
                    || VIS.DisplayType.Location == this.curTab.getField(columnName).getDisplayType()) {
                    if (fieldValue) {
                        fieldValue = this.curTab.getField(columnName.toLower())
                            .lookup.getDisplay(fieldValue);
                    }
                    $lblBPartner.text(fieldValue);
                }

                // Check if field is of Date Type
                fieldValue = null;
                columnName = 'T_DateTime';
                if (VIS.DisplayType.IsDate(this.curTab.getField(columnName).getDisplayType())) {
                    fieldValue = this.curTab.getField(columnName).value;
                    fieldValue = new Date(fieldValue).toLocaleString();
                    $lblDateTime.text(fieldValue);
                }
                // String, integer Value
                fieldValue = null;
                columnName = 'T_Amount';
                fieldValue = this.selectedRow[columnName.toLower()];
                $lblAmount.text(fieldValue);
            }
            catch(ex)
            {
            }
        };

        this.disposeComponent = function () {
            this.record_ID = 0;
            this.windowNo = 0;
            this.curTab = null;
            this.rowSource = null;
            this.panelWidth = null;
            $lblName.remove();
            $lblName = null;
            $lblC_Location.remove();
            $lblC_Location = null;
            $lblBPartner.remove();
            $lblBPartner = null;
            $lblDateTime.remove();
            $lblDateTime = null;
            $lblAmount.remove();
            $lblAmount = null;
            $root.remove();
            $root = null;
        };
    };

    /**
     *	Invoked when user click on panel icon
     */
    TestPanel.prototype.startPanel = function (windowNo, curTab) {
        this.windowNo = windowNo;
        this.curTab = curTab;
        this.init();
    };

    /**
         *	This function will execute when user navigate  or refresh a record
         */
    TestPanel.prototype.refreshPanelData = function (recordID, selectedRow) {
        this.record_ID = recordID;
        this.selectedRow = selectedRow;
        this.update(recordID);
    };

    /**
     *	Fired When Size of panel Changed
     */
    TestPanel.prototype.sizeChanged = function (width) {
        this.panelWidth = width;
    };

    /**
     *	Dispose Component
     */
    TestPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    /*
    * Fully qualified class name
    */
    VIS.TestPanel = TestPanel;
})();