; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //***********CalloutProduction Start ******
    function CalloutProduction() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProduction"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProduction, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Product modified
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutProduction.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0) {
            return "";
        }
        //	Set Attribute
        // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

        //if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
        //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
        //    mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID")));
        //}
        //else {
        mTab.setValue("M_AttributeSetInstance_ID", null);
        //}
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// when we select BOM on production Plan, if attribute defined on BOM then need to set the same Attributesetinstance
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutProduction.prototype.SetAttributeSetInstance = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "" || this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var paramString = value.toString();;
            var asi = VIS.dataContext.getJSONRecord("MProductionLine/GetAttributeSetInstance", paramString);
            mTab.setValue("M_AttributeSetInstance_ID", asi);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }

    /// <summary>
    /// when we select either Product or AttibutesetInstance on production Plan, pick respective BOM based on respective input
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutProduction.prototype.SetBOM = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "" || this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var paramString;
            if (mField.getColumnName().equals("M_Product_ID")) {
                // on selection of product, make ASI as null
                mTab.setValue("M_AttributeSetInstance_ID", 0);
                paramString = value.toString().concat(",", 0);
            }
            else if (mField.getColumnName().equals("M_AttributeSetInstance_ID")) {
                paramString = mTab.getValue("M_Product_ID").toString().concat(",", value.toString())
            }
            var bom = VIS.dataContext.getJSONRecord("MProductionLine/GetBOM", paramString);
            mTab.setValue("M_BOM_ID", bom);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }

    VIS.Model.CalloutProduction = CalloutProduction;
    //***********CalloutProduction End ******

})(VIS, jQuery);