VIS = window.VIS || {};

(function (VIS, $) {

    function calloutColumn() {
        VIS.CalloutEngine.call(this, "calloutColumn"); //must call
    };

    VIS.Utility.inheritPrototype(calloutColumn, VIS.CalloutEngine);

    calloutColumn.prototype.setDBColumnName = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value == 0) {
            return;
        }

        this.setCalloutActive(true);

        var _mTab = mTab;

        $.ajax({
            url: VIS.Application.contextUrl + "VIS/CalloutColumn/GetDBColunName",
            data: { AD_Element_ID: value },
            success: function (result) {
                if (result)
                    _mTab.setValue("ColumnName", result);
            },
            error: function (err) {
                this.log.severe(err);
            }
        });

        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;

        return "";

    };

    VIS.calloutColumn = calloutColumn;

})(VIS, jQuery);
