VIS = window.VIS || {};

(function (VIS, $) {

    var Util = VIS.Utility.Util;

    function calloutInvRevaluation() {
        VIS.CalloutEngine.call(this, "calloutInvRevaluation"); //must call
    };

    VIS.Utility.inheritPrototype(calloutInvRevaluation, VIS.CalloutEngine);

    /**
     * This functionis used to update Account Date as Document Date
     * @param {any} ctx
     * @param {any} windowNo
     * @param {any} mTab
     * @param {any} mField
     * @param {any} value
     * @param {any} oldValue
     */
    calloutInvRevaluation.prototype.setAcctDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value == 0) {
            return;
        }
        this.setCalloutActive(true);
        if (mField.getColumnName() == "DateDoc") {
            mTab.setValue("DateAcct", mTab.getValue("DateDoc"));
        }
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     * This Function is used to update Currency selected on Accounting Schema
     * @param {any} ctx
     * @param {any} windowNo
     * @param {any} mTab
     * @param {any} mField
     * @param {any} value
     * @param {any} oldValue
     */
    calloutInvRevaluation.prototype.AccountingSchema = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value == 0) {
            return;
        }
        this.setCalloutActive(true);
        try {
            var C_Currency_ID = VIS.dataContext.getJSONRecord("MInvRevaluation/GetAccountingSchemaCurrency", mTab.getValue("C_AcctSchema_ID").toString());
            mTab.setValue("C_Currency_ID", C_Currency_ID)
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     * This function is used to clear fields values
     * @param {any} ctx
     * @param {any} windowNo
     * @param {any} mTab
     * @param {any} mField
     * @param {any} value
     * @param {any} oldValue
     */
    calloutInvRevaluation.prototype.clearFields = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value == 0) {
            return;
        }
        this.setCalloutActive(true);
        try {
            // clear Warehouse when costing level is not "Warehouse" or "Warehouse+ Batch"
            if (mField.getColumnName() == "CostingLevel" &&
                !(mTab.getValue("CostingLevel") == "D" || mTab.getValue("CostingLevel") == "W")) {
                mTab.setValue("M_Warehouse_ID", null);
            }

            // clear Revaluation Period when Revaluation Type is "On Available Quantity"
            if (mField.getColumnName() == "RevaluationType" && mTab.getValue("RevaluationType") == "A") {
                mTab.setValue("C_Period_ID", null);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     * This function is used to calculate cost and update the respective fields (on Inventory Revaluation Line)
     * @param {any} ctx
     * @param {any} windowNo
     * @param {any} mTab
     * @param {any} mField
     * @param {any} value
     * @param {any} oldValue
     */
    calloutInvRevaluation.prototype.updateCostAndDiffrences = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value == 0) {
            return;
        }
        this.setCalloutActive(true);
        try {
            // Get Inventory Revaluation Details
            var dr = VIS.dataContext.getJSONRecord("MInvRevaluation/GetInvRevaluationDetails", mTab.getValue("M_InventoryRevaluation_ID").toString());
            if (dr != null) {

                // get Costing Precision
                var CostingPrecision = Util.getValueOfInt(dr["CostingPrecision"]);

                if (mField.getColumnName() == "NewCostPrice") {
                    mTab.setValue("DifferenceCostPrice", (mTab.getValue("NewCostPrice") - mTab.getValue("CurrentCostPrice")).toFixed(CostingPrecision));
                    mTab.setValue("TotalDifference", (mTab.getValue("DifferenceCostPrice") * mTab.getValue("QtyOnHand")).toFixed(CostingPrecision));

                    // When REVALUATIONTYPE_OnSoldConsumedQuantity
                    if (Util.getValueOfString(dr["RevaluationType"]) == "S") {
                        mTab.setValue("NewValue", (mTab.getValue("SoldQty") * mTab.getValue("NewCostPrice")).toFixed(CostingPrecision));
                        mTab.setValue("DifferenceValue", (mTab.getValue("NewValue") - mTab.getValue("SoldValue")).toFixed(CostingPrecision));
                    }
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.calloutInvRevaluation = calloutInvRevaluation;

})(VIS, jQuery);
