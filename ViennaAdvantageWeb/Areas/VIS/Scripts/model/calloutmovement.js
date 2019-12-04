; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //***********CalloutMovement Start*********
    /// <summary>
    /// Inventory Movement Callouts
    /// </summary>
    function CalloutMovement() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMovement"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutMovement, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Product modified
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>Error message or ""</returns>
    CalloutMovement.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            if (Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(*) FROM AD_column WHERE ad_table_id = " +
                   " (SELECT ad_table_id   FROM ad_table   WHERE upper(tablename) = upper('M_MovementLine'))" +
                   " and upper(columnname) = 'C_UOM_ID'", null, null)) > 0) {
                mTab.setValue("MovementQty", 1);
                mTab.setValue("QtyEntered", 1);
            }
            return "";
        }
        this.setCalloutActive(true);
        try {
            var M_Product_ID = value;
            if (M_Product_ID == null || M_Product_ID == 0)
                return "";
            //	Set Attribute
            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

            //if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
            //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0)
            //    mTab.setValue("M_AttributeSetInstance_ID", ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID"));
            //else
            mTab.setValue("M_AttributeSetInstance_ID", null);

            // change by Shivani 
            var sql = "SELECT COUNT(*) FROM AD_column WHERE ad_table_id = " +
                   " (SELECT ad_table_id   FROM ad_table   WHERE upper(tablename) = upper('M_MovementLine'))" +
                   " and upper(columnname) = 'C_UOM_ID'";
            if (Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null)) > 0) {
                sql = "select c_uom_id from m_product where m_product_id=" + M_Product_ID;
                var c_uom_id = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
                mTab.setValue("C_UOM_ID", c_uom_id);
                if (value != oldValue) {

                    mTab.setValue("MovementQty", 1);
                    mTab.setValue("QtyEntered", 1);
                }

                // var c_uom_id = Util.getValueOfInt(value);

                c_uom_id = Util.getValueOfDecimal(mTab.getValue("C_UOM_ID"));
                var QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Product_ID"));

                var paramStr = M_Product_ID.toString().concat(',').concat(c_uom_id.toString()).concat(',').concat(QtyEntered.toString());
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                QtyOrdered = pc;

                var conversion = false
                if (QtyOrdered != null) {
                    conversion = QtyEntered != QtyOrdered;
                }
                if (QtyOrdered == null) {
                    conversion = false;
                    QtyOrdered = 1;
                }
                if (conversion) {
                    mTab.setValue("MovementQty", QtyOrdered);
                }
                else {
                    mTab.setValue("MovementQty", (QtyOrdered * QtyEntered));
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.severe, sql, err);
            return err.message;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };

    VIS.Model.CalloutMovement = CalloutMovement;
    //***********CalloutMovement End**********   

})(VIS, jQuery);