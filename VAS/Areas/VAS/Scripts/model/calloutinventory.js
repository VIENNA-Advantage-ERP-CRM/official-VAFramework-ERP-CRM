; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //************CalloutInventory Start********
    /// <summary>
    /// Physical Inventory Callouts
    /// </summary>

    function CalloutInventory() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInventory"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutInventory, VIS.CalloutEngine); //inherit calloutengine
    /// <summary>
    /// Product/Locator/asi modified.
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>error message or ""</returns>
    CalloutInventory.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        // when method call from poduct container and set value of container as null
        if (!this.isCalloutActive() && mField.getColumnName() == "M_ProductContainer_ID" && (value == null || value.toString() == "")) {
            value = 0;
        }
        //JID_1181: On change of product on physical inventory line nad internal use system is giving error [TypeError: Cannot read property 'toString' of null]
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        //	overkill - see new implementation
        var Attribute_ID = 0;
        var AttrCode = ctx.getContext(windowNo, "AttrCode");
        // not call this function when calling happen from ProductContainer reference
        if (AttrCode != null && AttrCode != "" && mField.getColumnName() != "M_ProductContainer_ID") {
            //if (AttrCode != null && AttrCode != "") {
            //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + Util.getValueOfInt(value);
            //var attributeSet_ID = Util.getValueOfInt(VIS.DB.executeScalar(qry));
            //if (attributeSet_ID > 0) {
            //  qry = "SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE UPC = '" + AttrCode + "' AND M_Product_ID = " + Util.getValueOfInt(value);
            //Removed client side query
            // Mohit  21 May 2019
            var paramString = value.toString() + ',' + AttrCode.toString();
            Attribute_ID = VIS.dataContext.getJSONRecord("MInventoryLine/GetProductAttribute", paramString);

        }
        var inventoryLine = Util.getValueOfInt(mTab.getValue("M_InventoryLine_ID"));
        var M_Inventory_ID = Util.getValueOfInt(mTab.getValue("M_Inventory_ID"));
        var dr1 = null;
        var paramInventory = M_Inventory_ID.toString();
        dr1 = VIS.dataContext.getJSONRecord("MInventory/GetMInventory", paramInventory);
        var MovementDate = dr1["MovementDate"];
        var AD_Org_ID = dr1["AD_Org_ID"];


        // added by mohit to get/set the product uom on line-12 June 2018   
        var Uom;
        // JID_0855 :On change of locator and Attribute set instance sytem is removing the UOM of product.-
        // Mohit - 21 May 2019.
        if (mField.getColumnName() == "M_Product_ID") {
            Uom = VIS.dataContext.getJSONRecord("MInventoryLine/GetProductUOM", value.toString());
            mTab.setValue("C_UOM_ID", Util.getValueOfInt(Uom));
        }


        var bd = null;
        var paramString = inventoryLine.toString();
        /**
         * Modified for update Book Qty on existing records.
         * Also checks the old asi and removes it if product has been change.
         */
        if (inventoryLine != null && inventoryLine != 0) {
            //Get MInventoryLine Information
            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MInventoryLine/GetMInventoryLine", paramString);

            var M_Product_ID = dr["M_Product_ID"];//dr.M_Product_ID;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI
            var M_Locator_ID = dr["M_Locator_ID"];//dr.M_Locator_ID;



            // MInventoryLine iLine = new MInventoryLine(ctx, inventoryLine.Value, null);  
            var M_Product_ID1 = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
            var M_Locator_ID1 = Util.getValueOfInt(mTab.getValue("M_Locator_ID"));
            // get product Container
            var M_ProductContainer_ID = Util.getValueOfInt(mTab.getValue("M_ProductContainer_ID"));
            var M_AttributeSetInstance_ID1 = 0;
            // if product or locator has changed recalculate Book Qty
            //if (M_Product_ID1 != M_Product_ID || M_Locator_ID1 != M_Locator_ID) {
            if (M_Product_ID1 != M_Product_ID || M_Locator_ID1 != M_Locator_ID || (mField.getColumnName() == "M_ProductContainer_ID")) {
                this.setCalloutActive(true);
                // Check asi - if product has been changed remove old asi
                if (Attribute_ID > 0) {
                    mTab.setValue("M_AttributeSetInstance_ID", Attribute_ID);
                }
                if (M_Product_ID1 == M_Product_ID) {
                    M_AttributeSetInstance_ID1 = Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"));
                }
                else {
                    mTab.setValue("M_AttributeSetInstance_ID", null);
                }
                try {
                    bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID1, M_Product_ID1, M_Locator_ID1, Util.getValueOfDate(MovementDate), M_ProductContainer_ID);
                    //bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID1, M_Product_ID1, M_Locator_ID1, Util.getValueOfDate(MovementDate));
                    mTab.setValue("QtyBook", bd);
                }
                catch (err) {
                    this.setCalloutActive(false);
                    this.log.severe(err.toString());
                    return mTab.setValue("QtyBook", bd);
                }
            }
            // added by mohit to get/set the product uom on line-12 June 2018
            //mTab.setValue("C_UOM_ID", Util.getValueOfInt(C_Uom_ID));
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldValue = null;
            return "";
        }

        //	New Line - Get Book Value
        var M_Product_ID = 0;
        var product = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
        if (product != null)
            M_Product_ID = product;
        if (M_Product_ID == 0)
            return "";
        var M_Locator_ID = 0;
        var locator = Util.getValueOfInt(mTab.getValue("M_Locator_ID"));
        if (locator != null)
            M_Locator_ID = locator;
        if (M_Locator_ID == 0)
            return "";

        this.setCalloutActive(true);
        //	Set Attribute
        if (Attribute_ID > 0) {
            mTab.setValue("M_AttributeSetInstance_ID", Attribute_ID);
        }
        var M_AttributeSetInstance_ID = 0;
        var asi = Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"));
        if (asi != null)
            M_AttributeSetInstance_ID = asi;
        //	Product Selection
        // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

        //if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID) {
        //    M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false);
        //    if (M_AttributeSetInstance_ID != 0)
        //        mTab.setValue("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        //    else
        mTab.setValue("M_AttributeSetInstance_ID", null);
        //}

        var M_ProductContainer_ID = Util.getValueOfInt(mTab.getValue("M_ProductContainer_ID"));

        //Call's now the extracted function
        try {
            bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, Util.getValueOfDate(MovementDate), M_ProductContainer_ID);
            //bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, Util.getValueOfDate(MovementDate));
            mTab.setValue("QtyBook", bd);

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return mTab.setValue("QtyBook", bd);
        }
        //
        this.log.info("M_Product_ID=" + M_Product_ID
             + ", M_Locator_ID=" + M_Locator_ID
             + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
             + " - QtyBook=" + bd);
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Product/Locator/asi modified.
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>error message or ""</returns>
    CalloutInventory.prototype.AttributeSetInstance = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive())
            return "";

        //	overkill - see new implementation
        var inventoryLine = Util.getValueOfInt(mTab.getValue("M_InventoryLine_ID"));
        var M_Inventory_ID = Util.getValueOfInt(mTab.getValue("M_Inventory_ID"));
        var dr1 = null;
        var paramInventory = M_Inventory_ID.toString();
        dr1 = VIS.dataContext.getJSONRecord("MInventory/GetMInventory", paramInventory);
        var MovementDate = dr1["MovementDate"];
        var AD_Org_ID = dr1["AD_Org_ID"];

        var bd = null;
        var M_Product_ID = 0;
        var product = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
        if (product != null)
            M_Product_ID = product;
        if (M_Product_ID == 0)
            return "";
        var M_Locator_ID = 0;
        var locator = Util.getValueOfInt(mTab.getValue("M_Locator_ID"));
        if (locator != null)
            M_Locator_ID = locator;
        if (M_Locator_ID == 0)
            return "";

        this.setCalloutActive(true);
        //	Set Attribute
        var M_AttributeSetInstance_ID = 0;
        var asi = Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"));
        if (asi != null)
            M_AttributeSetInstance_ID = asi;

        var M_ProductContainer_ID = Util.getValueOfInt(mTab.getValue("M_ProductContainer_ID"));

        // Call's now the extracted function
        try {
            bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, Util.getValueOfDate(MovementDate), M_ProductContainer_ID);
            //bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, Util.getValueOfDate(MovementDate));
            mTab.setValue("QtyBook", bd);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return mTab.setValue("QtyBook", bd);
        }

        //
        this.log.info("M_Product_ID=" + M_Product_ID
             + ", M_Locator_ID=" + M_Locator_ID
             + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
             + " - QtyBook=" + bd);
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// FifferenceQty/AsOnDateCount modified.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>error message or ""</returns>
    CalloutInventory.prototype.SetDiff = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var asOnDateQty = 0, openingStock = 0, diffQty = 0;
        this.setCalloutActive(true);
        try {
            if (mField.getColumnName().equals("DifferenceQty")) {
                openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                diffQty = Util.getValueOfDecimal(mTab.getValue("DifferenceQty"));
                asOnDateQty = openingStock - diffQty;
                mTab.setValue("AsOnDateCount", asOnDateQty);
                /* SI_0631, SI_0632 : set qty as 0, System is not allowing to convert -ve qty to +ve. 'Validation Error : Value of Quantity count must be greater than 0"*/
                mTab.setValue("QtyCount", 0)
            }
            else if (mField.getColumnName().equals("AsOnDateCount")) {
                openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                asOnDateQty = Util.getValueOfDecimal(mTab.getValue("AsOnDateCount"));
                diffQty = openingStock - asOnDateQty;
                mTab.setValue("DifferenceQty", diffQty);
                /* SI_0631, SI_0632 : set qty as 0, System is not allowing to convert -ve qty to +ve. 'Validation Error : Value of Quantity count must be greater than 0"*/
                mTab.setValue("QtyCount", 0)
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return err.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// Returns the current Book Qty for given parameters or 0
    /// </summary>
    /// <param name="M_AttributeSetInstance_ID">attribute set instance id</param>
    /// <param name="M_Product_ID">product id</param>
    /// <param name="M_Locator_ID">locator id</param>
    /// <returns></returns>
    //CalloutInventory.prototype.SetQtyBook = function (AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, MovementDate) {
    //    //  
    //    // Set QtyBook from first storage location
    //    var bd = null;
    //    var query = "", qry = "";
    //    var result = 0;
    //    //var isContainerApplicable = false;
    //    //if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"] != undefined) {
    //    //    isContainerApplicable = VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"].equals("Y", true);
    //    //}
    //    var tsDate = "TO_DATE( '" + (Number(MovementDate.getMonth()) + 1) + "-" + MovementDate.getDate() + "-" + MovementDate.getFullYear() + "', 'MM-DD-YYYY')";     // GlobalVariable.TO_DATE(MovementDate, true);
    //    //if (isContainerApplicable) {
    //    //    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + tsDate +
    //    //        " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
    //    //}
    //    //else {
    //    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + tsDate +
    //   " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
    //    //}
    //    result = Util.getValueOfInt(VIS.DB.executeScalar(query));
    //    if (result > 0) {
    //        //if (isContainerApplicable) {
    //        //    qry = "SELECT  NVL(ContainerCurrentQty, 0) AS currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
    //        //        "(SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + tsDate + "  AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //        //        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
    //        //        ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //        //        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
    //        //        ")  AND  M_Product_ID = " + M_Product_ID +
    //        //        " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
    //        //}
    //        //else {
    //        qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
    //       "(SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + tsDate + "  AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //       " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //       " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
    //       " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
    //        //}
    //        bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
    //    }
    //    else {
    //        //if (isContainerApplicable) {
    //        //    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID +
    //        //        " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
    //        //}
    //        //else {
    //        query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID +
    //       " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
    //        //}
    //        result = Util.getValueOfInt(VIS.DB.executeScalar(query));
    //        if (result > 0) {
    //            //if (isContainerApplicable) {
    //            //    qry = "SELECT NVL(ContainerCurrentQty, 0) AS currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
    //            //        " (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //            //        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
    //            //        ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //            //        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
    //            //        ")  AND  M_Product_ID = " + M_Product_ID +
    //            //        " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
    //            //}
    //            //else {
    //            qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
    //            " (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //            " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
    //            " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
    //            " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
    //            // }
    //            bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
    //        }
    //    }
    //    if (bd != null) {
    //        return bd;
    //    }
    //    return 0;
    //};

    CalloutInventory.prototype.SetQtyBook = function (AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, MovementDate, M_ProductContainer_ID) {
        //  
        // Set QtyBook from first storage location
        var bd = null;
        var query = "", qry = "";
        var result = 0;
        var isContainerApplicable = false;
        if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"] != undefined) {
            isContainerApplicable = VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"].equals("Y", true);
        }
        var tsDate = "TO_DATE( '" + (Number(MovementDate.getMonth()) + 1) + "-" + MovementDate.getDate() + "-" + MovementDate.getFullYear() + "', 'MM-DD-YYYY')";     // GlobalVariable.TO_DATE(MovementDate, true);
        if (isContainerApplicable) {
            query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + tsDate +
                " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
        }
        else {
            query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + tsDate +
           " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
        }
        result = Util.getValueOfInt(VIS.DB.executeScalar(query));
        if (result > 0) {
            if (isContainerApplicable) {
                qry = "SELECT  NVL(ContainerCurrentQty, 0) AS currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
                    "(SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + tsDate + "  AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
                    ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
                    ")  AND  M_Product_ID = " + M_Product_ID +
                    " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
            }
            else {
                qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
               "(SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + tsDate + "  AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
               " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
               " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
               " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            }
            bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
        }
        else {
            if (isContainerApplicable) {
                query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID +
                    " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
            }
            else {
                query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID +
               " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            }
            result = Util.getValueOfInt(VIS.DB.executeScalar(query));
            if (result > 0) {
                if (isContainerApplicable) {
                    qry = "SELECT NVL(ContainerCurrentQty, 0) AS currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
                        " (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
                        ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                        " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID +
                        ")  AND  M_Product_ID = " + M_Product_ID +
                        " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND NVL(M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
                }
                else {
                    qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
                    " (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
                    " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                }
                bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
            }
        }
        if (bd != null) {
            return bd;
        }
        return 0;
    };

    // Callout added by mohit to get UOM conversion on Physical inventory line and internal use inventory line against the selected UOM.- 12 June 20018
    /// <summary>
    /// convert the qty according to selected UOM
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>Set qty or error message</returns>
    CalloutInventory.prototype.SetUOMQty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive())
            return "";
        var asOnDateQty = 0, openingStock = 0, diffQty = 0, QtyOrdered = 0;
        this.setCalloutActive(true);
        try {
            var M_Product_ID = Util.getValueOfDecimal(mTab.getValue("M_Product_ID"));
            if (M_Product_ID == 0) {

                this.setCalloutActive(false);
                return "";
            }



            // Check the source window
            //physical inventory call
            if (mTab.getValue("IsInternalUse") == false || mTab.getValue("IsInternalUse") == null) {
                if (mField.getColumnName() == "C_UOM_ID") {

                    var C_UOM_To_ID = Util.getValueOfInt(value);
                    QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                    M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
                    paramStr = M_Product_ID.toString().concat(',').concat(C_UOM_To_ID.toString()).concat(',').concat(QtyEntered.toString());
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
                        QtyOrdered;
                    }
                    else {

                        QtyOrdered = QtyOrdered * QtyEntered;
                    }


                }

                else if (mField.getColumnName() == "QtyEntered") {
                    var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                    QtyEntered = Util.getValueOfDecimal(value);

                    //Get precision from server
                    paramStr = C_UOM_To_ID.toString().concat(","); //1
                    var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);
                    var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);
                    if (QtyEntered != QtyEntered1) {
                        this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                        QtyEntered = QtyEntered1;
                        mTab.setValue("QtyEntered", QtyEntered);
                    }

                    paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2

                       QtyEntered.toString()); //3
                    var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                    QtyOrdered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,

                    if (QtyOrdered == null)
                        QtyOrdered = QtyEntered;


                    var conversion = QtyEntered != QtyOrdered;



                    this.log.fine("UOM=" + C_UOM_To_ID
                            + ", QtyEntered=" + QtyEntered
                            + " -> " + conversion
                            + " QtyOrdered=" + QtyOrdered);
                    ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                    QtyOrdered;
                }

                if (mTab.getValue("AdjustmentType").toString() == "D") {
                    openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                    diffQty = QtyOrdered;
                    asOnDateQty = openingStock - diffQty;
                    mTab.setValue("DifferenceQty", QtyOrdered);
                    mTab.setValue("AsOnDateCount", asOnDateQty);
                }
                else if (mTab.getValue("AdjustmentType").toString() == "A") {
                    openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                    asOnDateQty = QtyOrdered;
                    diffQty = openingStock - asOnDateQty;
                    mTab.setValue("AsOnDateCount", QtyOrdered);
                    mTab.setValue("DifferenceQty", diffQty);
                }

            }
                // Internal use inventory call
            else if (mTab.getValue("IsInternalUse") == true) {
                if (mField.getColumnName() == "C_UOM_ID") {

                    var C_UOM_To_ID = Util.getValueOfInt(value);
                    QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                    M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
                    paramStr = M_Product_ID.toString().concat(',').concat(C_UOM_To_ID.toString()).concat(',').concat(QtyEntered.toString());
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
                        mTab.setValue("QtyInternalUse", QtyOrdered);
                    }
                    else {

                        mTab.setValue("QtyInternalUse", (QtyOrdered * QtyEntered));
                    }


                }

                else if (mField.getColumnName() == "QtyEntered") {
                    var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                    QtyEntered = Util.getValueOfDecimal(value);

                    //Get precision from server
                    paramStr = C_UOM_To_ID.toString().concat(","); //1
                    var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);
                    var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);
                    if (QtyEntered != QtyEntered1) {
                        this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                        QtyEntered = QtyEntered1;
                        mTab.setValue("QtyEntered", QtyEntered);
                    }

                    paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2

                       QtyEntered.toString()); //3
                    var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                    QtyOrdered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,

                    if (QtyOrdered == null)
                        QtyOrdered = QtyEntered;


                    var conversion = QtyEntered != QtyOrdered;



                    this.log.fine("UOM=" + C_UOM_To_ID
                            + ", QtyEntered=" + QtyEntered
                            + " -> " + conversion
                            + " QtyOrdered=" + QtyOrdered);
                    ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                    mTab.setValue("QtyInternalUse", QtyOrdered);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return err.message;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };
    VIS.Model.CalloutInventory = CalloutInventory;
    //************CalloutInventory End*********

})(VIS, jQuery);