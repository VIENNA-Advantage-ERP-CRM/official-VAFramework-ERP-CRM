; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //********** CalloutProduct Start *****
    /// <summary>
    /// Product Callouts
    /// </summary>
    function CalloutProduct() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProduct"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProduct, VIS.CalloutEngine);//inherit CalloutEngine
    /**
     *	Product Category
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.ProductCategory = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_Category_ID = Util.getValueOfInt(value);
        if (M_Product_Category_ID == null || Util.getValueOfInt(M_Product_Category_ID) == 0
            || M_Product_Category_ID == 0)
            return "";
        var paramString = Util.getValueOfInt(value);
        /**
         * Modified for update Book Qty on existing records.
         * Also checks the old asi and removes it if product has been change.
         */

        //Get MInventoryLine Information
        //Get product price information
        var dr = null;
        dr = VIS.dataContext.getJSONRecord("MProductCategory/GetProductCategory", paramString);

        //var M_Product_ID = dr.M_Product_ID;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI
        //var M_Locator_ID=dr.M_Locator_ID;     
        var IsPurchasedToOrder = dr.IsPurchasedToOrder;
        //  MProductCategory pc = new MProductCategory(ctx, M_Product_Category_ID, null);
        mTab.setValue("IsPurchasedToOrder", IsPurchasedToOrder);
        pc = null;
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    CalloutProduct.prototype.UOM = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        if (mTab.getValue("M_Product_id") == null) {
            return "";
        }
        this.setCalloutActive(true);

        // 26-09-2011 Lokesh Chauhan

        // If values are in The Transaction Tab then restrict user so that UOM can't be changed or set to what it was previously.
        var sql = "select count(*) from m_transaction where m_product_id = " + Util.getValueOfInt(mTab.getValue("M_Product_id"));
        var result = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));

        if (result > 0) {
            //ShowMessage.Info("Can't change UOM due to Transactions happens based on existing UOM", true, null, null);
            VIS.ADialog.info("Can't change UOM due to Transactions happens based on existing UOM");
            sql = "select c_uom_id from m_product where m_product_id =  " + Util.getValueOfInt(mTab.getValue("M_Product_id"));
            var uom_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            mTab.setValue("C_UOM_ID", uom_ID);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *	Resource Group
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.ResourceGroup = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var resgrp = Util.getValueOfString(value);
        if (resgrp == null || resgrp.length == 0)
            return "";

        if ("O" == resgrp)
            mTab.setValue("BasisType", null);
        else
            mTab.setValue("BasisType", "I");
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  Organization
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.Organization = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var AD_Org_ID = Util.getValueOfInt(value);

        if (AD_Org_ID == null) {
            return "";
        }
        var dr = null;
        dr = VIS.dataContext.getJSONRecord("MLocator/GetLocator", paramString);

        var Default_Locator_ID = dr["Default_Locator_ID"];//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI


        // MLocator defaultLocator = MLocator.GetDefaultLocatorOfOrg(ctx, AD_Org_ID);
        // if (defaultLocator != null)
        // {
        mTab.setValue("M_Locator_ID", Default_Locator_ID);
        //}
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProduct = CalloutProduct;
    //**********CalloutProduct End ******



})(VIS, jQuery);