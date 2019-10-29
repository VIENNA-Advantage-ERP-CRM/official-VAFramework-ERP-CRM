; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;




    //***************CalloutDashboard Start********************

    function CalloutDashboard() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDashboard");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDashboard, VIS.CalloutEngine); //inherit prototype

    CalloutDashboard.prototype.UpdateTableInContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
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

    CalloutDashboard.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
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
            mTab.setValue("TableView_ID", -1);
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };

    CalloutDashboard.prototype.UpdateTableViewIDOnTableContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (value > 0) {
            mTab.setValue("TableView_ID", -1);
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };

    CalloutDashboard.prototype.SelectFunctionOnDashboard = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
            return "";
        }
        this.setCalloutActive(true);
        var bl = Util.getValueOfBoolean(value);

        if (mField.getColumnName() == "IsSum") {
            //DisplayType.Integer;
            mTab.setValue("IsAvg", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsNone", false);
        }
        else if (mField.getColumnName() == "IsAvg") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsNone", false);

        }
        else if (mField.getColumnName() == "IsCount") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsAvg", false);
            mTab.setValue("IsNone", false);

        }
        else if (mField.getColumnName() == "IsNone") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsAvg", false);

        }
        this.setCalloutActive(false);


        return "";
    };

    //Manish 4/7/2016   For Maximum value check should not be greater then 60
    CalloutDashboard.prototype.ValueCheck = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);


        var val = mTab.getValue("LastNValue");
        if (val > 60) {
            mTab.setValue("LastNValue", 1);
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };
    //end 


    VIS.Model.CalloutDashboard = CalloutDashboard;

    //**************************CalloutDashboard End******************************\\


    //***************CalloutDashboard Start********************

    function CalloutDashboardView() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDashboardView");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDashboardView, VIS.CalloutEngine); //inherit prototype

    CalloutDashboardView.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
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

    CalloutDashboardView.prototype.GroupByChecked = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);
        var paramstring = mTab.getValue("RC_View_ID").toString() + ',' + mTab.getValue("RC_ViewColumn_ID").toString();
        var dr = VIS.dataContext.getJSONRecord("MDashboard/GroupByChecked", paramString);
        //var sql = "UPDATE RC_ViewColumn SET IsGroupBy='N' WHERE RC_View_ID=" + mTab.getValue("RC_View_ID") + " AND RC_ViewColumn_ID NOT IN(" + mTab.getValue("RC_ViewColumn_ID") + ")";
        //var count = VIS.DB.executeQuery(sql);
        this.setCalloutActive(false);
        return "";

    };

    CalloutDashboardView.prototype.IsViewChecked = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);
        if (value) {
            mTab.setValue("AD_Tab_ID", -1);
            mTab.setValue("AD_Table_ID", -1);
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
            ctx.setContext(windowNo, "AD_Table_ID", -1);
        }
        else {
            mTab.setValue("TableView_ID", -1);
            ctx.setContext(windowNo, "TableView_ID", -1);
        }
        this.setCalloutActive(false);
        return "";

    };

    VIS.Model.CalloutDashboardView = CalloutDashboardView;

    //**************************CalloutDashboard End******************************\\

})(VIS, jQuery);