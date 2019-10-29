; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;

    //***************CalloutKPI Start********************

    function CalloutKPI() {
        VIS.CalloutEngine.call(this, "VIS.CalloutKPI");//must call
    };
    VIS.Utility.inheritPrototype(CalloutKPI, VIS.CalloutEngine); //inherit prototype

    CalloutKPI.prototype.CalculationSelection = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
            return "";
        }
        this.setCalloutActive(true);

        if (mField.getColumnName() == "IsSum") {
            //DisplayType.Integer;
            mTab.setValue("IsMaximum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsMinimum", false);
        }
        else if (mField.getColumnName() == "IsMaximum") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsMinimum", false);
            mTab.setValue("IsCount", false);

        }
        else if (mField.getColumnName() == "IsCount") {
            mTab.setValue("IsMaximum", false);
            mTab.setValue("IsSum", false);
            mTab.setValue("IsMinimum", false);

        }
        else if (mField.getColumnName() == "IsMinimum") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsMaximum", false);

        }

        this.setCalloutActive(false);
        return "";
    };



    CalloutKPI.prototype.UpdateKPITableInContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        if (value == true) {
            mTab.setValue("AD_Table_ID", -1);
            mTab.setValue("AD_Tab_ID", -1);
            ctx.setContext(windowNo, "AD_Table_ID", -1);
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
        }
        else {
            mTab.setValue("TableView_ID", -1);
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };



    CalloutKPI.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        if (value < 1 || value == null || value.toString() == "") {
            mTab.setValue("AD_Tab_ID", -1);
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "AD_Tab_ID", value);
        }

        this.setCalloutActive(false);
        return "";
    };



    VIS.Model.CalloutKPI = CalloutKPI;

    //**************************CalloutKPI End******************************\\


})(VIS, jQuery);
