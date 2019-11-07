; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;

    //CalloutAssigment Start
    function CalloutAssignment() {
        VIS.CalloutEngine.call(this, "VIS.CalloutAssignment"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutAssignment, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Assignment_Product.
    /// - called from S_ResourceAssignment_ID
    /// - sets M_Product_ID, Description
    /// - Qty.. 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>

    CalloutAssignment.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {


        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        //	get value
        var S_ResourceAssignment_ID = Util.getValueOfInt(value);
        if (S_ResourceAssignment_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var M_Product_ID = 0;
        var name = null;
        var description = null;
        var Qty = null;
        var sql = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty "
            + "FROM S_ResourceAssignment ra"
            + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) "
            + "WHERE ra.S_ResourceAssignment_ID=" + S_ResourceAssignment_ID;
        var idr = null;
        try {
            idr = VIS.DB.executeReader(sql, null, null);
            if (idr.read()) {
                M_Product_ID = Util.getValueOfInt(idr.get(0));//.getInt (1);
                name = Util.getValueOfString(idr.get(1));//.getString(2);
                description = Util.getValueOfString(idr.get(2));//.getString(3);
                Qty = Util.getValueOfDecimal(idr.get(3));//.getBigDecimal(4);
            }
            idr.close();
        }
        catch (err) {
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, "product", err);
        }

        this.log.fine("S_ResourceAssignment_ID=" + S_ResourceAssignment_ID + " - M_Product_ID=" + M_Product_ID);
        if (M_Product_ID != 0) {
            mTab.setValue("M_Product_ID", Util.getValueOfInt(M_Product_ID));
            if (description != null) {
                name += " (" + description + ")";
            }
            if ("." != name)//(!".".equals(name))
            {
                mTab.setValue("Description", name);
            }
            //
            var variable = "Qty";	//	TimeExpenseLine
            if (mTab.getTableName().startsWith("C_Order")) {
                variable = "QtyOrdered";
            }
            else if (mTab.getTableName().startsWith("C_Invoice")) {
                variable = "QtyInvoiced";
            }
            if (Qty != null) {
                mTab.setValue(variable, Qty);
            }
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutAssignment = CalloutAssignment;

})(VIS, jQuery);