; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //************CalloutInOut Start**********************
    function CalloutInOut() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInOut");//must call
    };
    VIS.Utility.inheritPrototype(CalloutInOut, VIS.CalloutEngine); //inherit prototype

    /// <summary>
    /// C_Order - Order Defaults.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Order = function (ctx, windowNo, mTab, mField, value, oldValue) {
        // 

        if (value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_Order_ID = Util.getValueOfInt(value.toString());
            if (C_Order_ID == null || C_Order_ID == 0) {
                return "";
            }
            //	No Callout Active to fire dependent values
            if (this.isCalloutActive())	//	prevent recursive
            {
                return "";
            }
            var paramString = C_Order_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MOrder/GetOrder", paramString);


            if (Util.getValueOfInt(dr["ID"]) != 0) {
                mTab.setValue("DateOrdered", dr["DateOrdered"]);
                mTab.setValue("POReference", dr["POReference"]);
                if (mTab.getValue("AD_Org_ID") != Util.getValueOfInt(dr["AD_Org_ID"])) {    // If Org ID is same then no need to update the Org ID, else Document type gets refreshed
                    mTab.setValue("AD_Org_ID", Util.getValueOfInt(dr["AD_Org_ID"]));
                }
                //
                mTab.setValue("DeliveryRule", dr["DeliveryRule"]);
                mTab.setValue("DeliveryViaRule", dr["DeliveryViaRule"]);
                mTab.setValue("M_Shipper_ID", Util.getValueOfInt(dr["M_Shipper_ID"]));
                mTab.setValue("FreightCostRule", dr["FreightCostRule"]);
                mTab.setValue("FreightAmt", dr["FreightAmt"]);

                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr["C_BPartner_ID"]));
                //sraval: source forge bug # 1503219 - added to default ship to location
                mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(dr["C_BPartner_Location_ID"]));

                mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(dr["AD_OrgTrx_ID"]));
                mTab.setValue("C_Activity_ID", Util.getValueOfInt(dr["C_Activity_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(dr["C_Campaign_ID"]));
                mTab.setValue("C_Project_ID", Util.getValueOfInt(dr["C_Project_ID"]));
                mTab.setValue("User1_ID", Util.getValueOfInt(dr["User1_ID"]));
                mTab.setValue("User2_ID", Util.getValueOfInt(dr["User2_ID"]));

                // Added by Bharat on 30 Jan 2018 to set Inco Term from Order
                if (mTab.getField("C_IncoTerm_ID") != null) {
                    mTab.setValue("C_IncoTerm_ID", Util.getValueOfInt(dr["C_IncoTerm_ID"]));
                }

                var isReturnTrx = mTab.getValue("IsReturnTrx");
                if (isReturnTrx) {
                    mTab.setValue("Orig_Order_ID", dr["Orig_Order_ID"]);
                    mTab.setValue("Orig_InOut_ID", dr["Orig_InOut_ID"]);
                    // added by vivek on 09/10/2017 advised by pradeep to set drop ship checkbox value
                    if (Util.getValueOfString(dr["IsDropShip"]) == "Y") {
                        mTab.setValue("IsDropShip", true);
                    }
                    else {
                        mTab.setValue("IsDropShip", false);
                    }
                }
                mTab.setValue("M_Warehouse_ID", Util.getValueOfInt(dr["M_Warehouse_ID"]));
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //MessageBox.Show("CalloutInOut--Order Defaults");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// InOut - DocType.
    /// - sets MovementType
    /// - gets DocNo
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_DocType_ID = Util.getValueOfInt(value.toString());// (int)value;
        if (C_DocType_ID == null || C_DocType_ID == 0) {
            return "";
        }
        var sql = "SELECT d.docBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
            + "FROM C_DocType d, AD_Sequence s "
            + "WHERE C_DocType_ID=" + C_DocType_ID		//	1
            + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
        var dr = null;
        try {
            ctx.setContext(windowNo, "C_DocTypeTarget_ID", C_DocType_ID);
            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                //	Set Movement Type
                var docBaseType = dr.get("docbasetype");
                var isReturnTrx = dr.get("isreturntrx") == "Y";
                if (docBaseType.equals("MMS") && !isReturnTrx)					//	Material Shipments
                {
                    mTab.setValue("MovementType", "C-");				//	Customer Shipments
                }
                else if (docBaseType.equals("MMS") && isReturnTrx)				//	Material Shipments
                {
                    mTab.setValue("MovementType", "C+");				//	Customer Returns
                }
                else if (docBaseType.equals("MMR") && !isReturnTrx)				//	Material Receipts
                {
                    mTab.setValue("MovementType", "V+");				//	Vendor Receipts
                }
                else if (docBaseType.equals("MMR") && isReturnTrx)				//	Material Receipts
                {
                    mTab.setValue("MovementType", "V-");					//	Return to Vendor
                }

                //	DocumentNo
                if (dr.get("isdocnocontrolled") == "Y") {
                    mTab.setValue("DocumentNo", "<" + dr.get("currentnext") + ">");
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            //MessageBox.Show("CalloutInOut--DocType");
            this.log.log(Level.SEVERE, sql, err);
            return err.message;
            //return e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOut - Defaults for BPartner.
    /// - Location
    /// - Contact
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var sql = "";
        var idr = null;
        var drl = null;
        if (value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = Util.getValueOfInt(value.toString());// (int)value;
            if (C_BPartner_ID == null || C_BPartner_ID == 0) {
                return "";
            }

            var isReturnTrx = mTab.getValue("IsReturnTrx");

            //	sraval: source forge bug # 1503219
            var order = mTab.getValue("C_Order_ID");

            //sql = "SELECT p.AD_Language, p.POReference,"
            //    + "SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            //    + "l.C_BPartner_Location_ID, c.AD_User_ID "
            //    + "FROM C_BPartner p"
            //    + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID)"
            //    + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            //    + "WHERE p.C_BPartner_ID=" + C_BPartner_ID;		//	1
            sql = "SELECT p.AD_Language, p.POReference,"
              + "p.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable,"
              + "l.C_BPartner_Location_ID, c.AD_User_ID , p.SOCreditStatus "
              + "FROM C_BPartner p"
              + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID)"
              + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
              + "WHERE p.C_BPartner_ID=" + C_BPartner_ID;		//	1 // SD


            idr = VIS.DB.executeReader(sql, null, null);
            if (idr.read()) {
                //	Location
                var ii = Util.getValueOfInt(idr.get("c_bpartner_location_id"));
                // sraval: source forge bug # 1503219 - default location for material receipt
                if (order == null) {
                    //if (dr.wasNull())
                    if (ii == 0) {
                        mTab.setValue("C_BPartner_Location_ID", null);
                    }
                    else {
                        mTab.setValue("C_BPartner_Location_ID", ii);
                    }
                }
                //	Contact
                ii = Util.getValueOfInt(idr.get("ad_user_id"));
                //if (dr.wasNull())
                if (ii == 0) {
                    mTab.setValue("AD_User_ID", null);
                }
                else {
                    mTab.setValue("AD_User_ID", ii);
                }

                // Skip credit check for returns
                if (!isReturnTrx) {
                    //	CreditAvailable
                    var CreditStatus = idr.getString("CreditStatusSettingOn");
                    var CreditAvailable = Util.getValueOfDouble(idr.get("creditavailable"));
                    if (CreditStatus == "CH") {
                        if (idr.get("SOCreditStatus") != null) {
                            if (!idr.get("SOCreditStatus").equals("X")) {// SD
                                if (CreditAvailable <= 0) {
                                    VIS.ADialog.info("CreditLimitOver");
                                }
                            }
                        }
                    }
                    else {
                        var locId = Util.getValueOfInt(mTab.getValue("C_BPartner_Location_ID"));
                        sql = "SELECT bp.CreditStatusSettingOn,p.SOCreditStatus,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable" +
                            " FROM C_BPartner_Location p INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = p.C_BPartner_ID) WHERE p.C_BPartner_Location_ID = " + locId;
                        drl = VIS.DB.executeReader(sql);
                        if (drl.read()) {
                            CreditStatus = drl.getString("CreditStatusSettingOn");
                            if (CreditStatus == "CL") {
                                var CreditLimit = Util.getValueOfDouble(drl.get("so_creditlimit"));
                                var CreditAvailable = Util.getValueOfDouble(drl.get("creditavailable"));
                                var SOCreditStatus = dr.getString("SOCreditStatus");
                                //if (CreditLimit != 0) {                                    
                                //    if (dr != null && CreditAvailable <= 0) {
                                //        VIS.ADialog.info("CreditOver", null, "", "");
                                //    }
                                //}
                                if (SOCreditStatus != null) {
                                    if (!SOCreditStatus.equals("X")) {// SD
                                        if (CreditAvailable <= 0) {
                                            VIS.ADialog.info("CreditOver");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            idr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
                idr = null;
            }
            if (drl != null) {
                drl.close();
                drl = null;
            }
            //MessageBox.Show("CalloutInOut--BPartner");
            this.log.log(Level.SEVERE, sql, e);
            //return e.getLocalizedMessage();
            return e.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_Warehouse.
    /// Set Organization and Default Locator
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Warehouse = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        var M_Warehouse_ID = Util.getValueOfInt(value);// (int)value;
        if (M_Warehouse_ID == null || M_Warehouse_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var sql = "SELECT w.AD_Org_ID, l.M_Locator_ID "
            + "FROM M_Warehouse w"
            + " LEFT OUTER JOIN M_Locator l ON (l.M_Warehouse_ID=w.M_Warehouse_ID AND l.IsDefault='Y') "
            + "WHERE w.M_Warehouse_ID=" + M_Warehouse_ID;		//	1
        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                //	Org
                var ii = Util.getValueOfInt(dr.get("ad_org_id"));//.getInt(1));
                var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
                if (AD_Org_ID != ii) {
                    mTab.setValue("AD_Org_ID", ii);
                }
                //	Locator
                ii = Util.getValueOfInt(dr.get("m_locator_id"));// new int(dr.getInt(2));
                //if (dr.wasNull())
                if (ii == 0) {
                    ctx.setContext(windowNo, 0, "M_Locator_ID", null);
                }
                else {
                    this.log.config("M_Locator_ID=" + ii);
                    ctx.setContext(windowNo, "M_Locator_ID", ii);
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            //MessageBox.Show("CalloutInOut--Warehouse");
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// OrderLine Callout
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.OrderLine = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var C_OrderLine_ID = Util.getValueOfInt(value.toString());// (int)value;
        if (C_OrderLine_ID == null || C_OrderLine_ID == 0) {
            this.setCalloutActive(false);
            return "";
        }
        try {
            //	Get Details
            var paramString = C_OrderLine_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);
            // MOrderLine ol = new MOrderLine(ctx, C_OrderLine_ID, null);

            //	Get Details
            if (Util.getValueOfInt(dr["GetID"]) != 0) {
                mTab.setValue("M_Product_ID", Util.getValueOfInt(dr["M_Product_ID"]));
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(dr["M_AttributeSetInstance_ID"]));
                mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr["C_UOM_ID"]));
                //var movementQty = Decimal.Subtract(ol.GetQtyOrdered(), ol.GetQtyDelivered());
                var movementQty = (Util.getValueOfDecimal(dr["QtyOrdered"]) - Util.getValueOfDecimal(dr["QtyDelivered"]));
                mTab.setValue("MovementQty", movementQty);
                var qtyEntered = movementQty;
                if ((Util.getValueOfDecimal(dr["QtyEntered"])).toString().compareTo(Util.getValueOfDecimal(dr["QtyOrdered"])) != Util.getValueOfDecimal(dr["QtyOrdered"])) {
                    //qtyEntered = qtyEntered.multiply(ol.getQtyEntered()).divide(ol.getQtyOrdered(), 12, Decimal.ROUND_HALF_UP);
                    qtyEntered = ((qtyEntered * (Util.getValueOfDecimal(dr["QtyEntered"]))) / (Util.getValueOfDecimal(dr["QtyOrdered"]).toFixed(12)));//, MidpointRounding.AwayFromZero));
                }
                mTab.setValue("QtyEntered", qtyEntered);
                //
                mTab.setValue("C_Activity_ID", Util.getValueOfInt(dr["C_Activity_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(dr["C_Campaign_ID"]));
                mTab.setValue("C_Project_ID", Util.getValueOfInt(dr["C_Project_ID"]));
                mTab.setValue("C_ProjectPhase_ID", Util.getValueOfInt(dr["C_ProjectPhase_ID"]));
                mTab.setValue("C_ProjectTask_ID", Util.getValueOfInt(dr["C_ProjectTask_ID"]));
                mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(dr["AD_OrgTrx_ID"]));
                mTab.setValue("User1_ID", Util.getValueOfInt(dr["User1_ID"]));
                mTab.setValue("User2_ID", Util.getValueOfInt(dr["User2_ID"]));
                //if (dr["IsReturnTrx"]=="true")
                if (Util.getValueOfBoolean(dr["IsReturnTrx"])) {
                    mTab.setValue("Orig_OrderLine_ID", Util.getValueOfInt(dr["Orig_OrderLine_ID"]));
                    var paramString = dr["Orig_InOutLine_ID"];
                    var line = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramString);

                    mTab.setValue("M_Locator_ID", line["M_Locator_ID"]);
                }
                if (Util.getValueOfString(dr["IsDropShip"]) == "Y") {
                    mTab.setValue("IsDropShip", true);
                }
                else {
                    mTab.setValue("IsDropShip", false);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOutLine - Default UOM/Locator for Product.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = Util.getValueOfInt(value);// (int)value;
        if (M_Product_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            //	Set Attribute & Locator
            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

            var M_Locator_ID = 0;
            //if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
            //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
            //    mTab.setValue("M_AttributeSetInstance_ID",
            //        Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID").toString()));
            //	Locator from Info Window - ASI
            //M_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
            //if (M_Locator_ID != 0) {
            //    mTab.setValue("M_Locator_ID", Util.getValueOfInt(M_Locator_ID.toString()));
            //}
            //}
            //else {
            mTab.setValue("M_AttributeSetInstance_ID", null);
            //}

            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            // var isSOTrx = ctx.getContext("IsSOTrx");
            if (isSOTrx) {
                this.setCalloutActive(false);
                return "";
            }

            //	PO - Set UOM/Locator/Qty

            //MProduct product = MProduct.Get(ctx, M_Product_ID);
            //mTab.setValue("C_UOM_ID", Util.getValueOfInt(product.GetC_UOM_ID().toString()));
            var paramString = M_Product_ID.toString();
            var C_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramString);
            mTab.setValue("C_UOM_ID", C_UOM_ID);
            var qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            mTab.setValue("MovementQty", qtyEntered);
            var qryBP = "SELECT C_BPartner_ID FROM M_InOut WHERE M_InOut_ID = " + Util.getValueOfInt(mTab.getValue("M_InOut_ID"));
            var bpartner = Util.getValueOfInt(VIS.DB.executeScalar(qryBP));
            var qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + M_Product_ID + " AND vdr.C_BPartner_ID = " + bpartner;
            var uom = Util.getValueOfInt(VIS.DB.executeScalar(qryUom));
            if (C_UOM_ID != 0) {
                if (C_UOM_ID != uom && uom != 0) {
                    var Res = Util.getValueOfDecimal(VIS.DB.executeScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + M_Product_ID + " AND IsActive='Y'"));
                    if (Res > 0) {
                        mTab.setValue("QtyEntered", Util.getValueOfInt(mTab.getValue("QtyEntered")) * Res);
                        //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                    }
                    else {
                        var res = Util.getValueOfDecimal(VIS.DB.executeScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                        if (res > 0) {
                            mTab.setValue("QtyEntered", Util.getValueOfInt(mTab.getValue("QtyEntered")) * Res);
                            //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                        }
                    }
                    mTab.setValue("C_UOM_ID", uom);
                }
                else {
                    mTab.setValue("C_UOM_ID", C_UOM_ID);
                }
            }

            // Commented as not in use now
            //if (window.BTR002) {
            //    if (isSOTrx) {
            //        var prdWarehouse = 0, prdLocator = 0;
            //        var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
            //        var qryPrd = "SELECT loc.M_Warehouse_ID FROM M_Product p INNER JOIN M_Locator loc ON p.M_Locator_ID= loc.M_Locator_ID WHERE p.M_Product_ID=" + M_Product_ID;
            //        var idr = VIS.DB.executeReader(qryPrd);
            //        while (idr.read()) {
            //            prdWarehouse = Util.getValueOfInt(idr.get("m_warehouse_id"));
            //            prdLocator = Util.getValueOfInt(idr.get("m_locator_id"));
            //        }
            //        idr.close();
            //        if (M_Warehouse_ID == prdWarehouse) {
            //            mTab.setValue("M_Locator_ID", prdLocator);
            //        }
            //    }
            //}
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// InOut Line - Quantity.
    /// - called from C_UOM_ID, qtyEntered, movementQty
    /// - enforces qty UOM relationship
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            this.log.log(Level.WARNING, "qty - init - M_Product_ID=" + M_Product_ID);
            var movementQty, qtyEntered;
            var paramString = "";
            var precision = 0;

            //	No Product
            if (M_Product_ID == 0) {
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                mTab.setValue("MovementQty", qtyEntered);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName().toString().equals("C_UOM_ID")) {
                var C_UOM_To_ID = value;
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                paramString = C_UOM_To_ID.toString();
                precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramString);
                var QtyEntered1 = qtyEntered.toFixed(precision);//, MidpointRounding.AwayFromZero);
                if (qtyEntered.toString().compareTo(QtyEntered1) != 0) {
                    this.log.fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                      + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", qtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", qtyEntered.toString());
                movementQty = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);
                //movementQty = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, qtyEntered.Value);
                if (movementQty == null) {
                    movementQty = qtyEntered;
                }
                var conversion = qtyEntered.compareTo(movementQty) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " movementQty=" + movementQty);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("MovementQty", movementQty);
            }
                //	No UOM defined
            else if (ctx.getContextAsInt(windowNo, "C_UOM_ID") == 0) {
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                mTab.setValue("MovementQty", qtyEntered);
            }
                //	qtyEntered changed - calculate movementQty
            else if (mField.getColumnName().toString().equals("QtyEntered")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                qtyEntered = Util.getValueOfDecimal(value);
                paramString = M_Product_ID.toString();
                precision = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramString);

                // JID_0681: If we copy the MR lines using copy from button system is only copy the Qty only before decimal.
                var QtyEntered1 = qtyEntered.toFixed(precision);
                if (qtyEntered.compareTo(QtyEntered1) != 0) {
                    this.log.fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", qtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", qtyEntered.toString());
                movementQty = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);
                // movementQty = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //     C_UOM_To_ID, qtyEntered.Value);
                if (movementQty == null) {
                    movementQty = qtyEntered;
                }
                var conversion = qtyEntered.compareTo(movementQty) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " movementQty=" + movementQty);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("MovementQty", movementQty);
            }
                //	movementQty changed - calculate qtyEntered (should not happen)
            else if (mField.getColumnName().toString().equals("MovementQty")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                movementQty = Util.getValueOfDecimal(value);

                paramString = M_Product_ID.toString();
                precision = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramString);

                // JID_0681: If we copy the MR lines using copy from button system is only copy the Qty only before decimal.
                var MovementQty1 = movementQty.toFixed(precision);
                if (movementQty.compareTo(MovementQty1) != 0) {
                    this.log.fine("Corrected movementQty "
                        + movementQty + "->" + MovementQty1);
                    movementQty = MovementQty1;
                    mTab.setValue("MovementQty", movementQty);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", movementQty.toString());
                qtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);

                //qtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, movementQty);
                if (qtyEntered == null) {
                    qtyEntered = movementQty;
                }
                var conversion = movementQty.compareTo(qtyEntered) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", movementQty=" + movementQty
                    + " -> " + conversion
                    + " qtyEntered=" + qtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", qtyEntered);
            }

            // Check for RMA
            var isReturnTrx = "Y".equals(ctx.getContext("IsReturnTrx"));
            if (M_Product_ID != 0 && isReturnTrx) {
                var oLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                paramString = oLine_ID.toString();
                var oLine = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);
                //  MOrderLine oLine = new MOrderLine(ctx, oLine_ID, null);
                if (oLine.Get_ID() != 0) {
                    var orig_IOLine_ID = oLine["tOrig_InOutLine_ID"];
                    if (orig_IOLine_ID != 0) {
                        var paramString = orig_IOLine_ID.toString();
                        var orig_IOLine = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramString);

                        // MInOutLine orig_IOLine = new MInOutLine(ctx, orig_IOLine_ID, null);
                        var shippedQty = orig_IOLine["MovementQty"];
                        movementQty = Util.getValueOfDecimal(mTab.getValue("MovementQty"));
                        if (shippedQty.toString().compareTo(movementQty) < 0) {
                            if (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") {
                                // ShowMessage.Info("QtyShippedAndReturned", true, "", "");
                                VIS.ADialog.info("QtyShippedAndReturned");
                            }
                            else {
                                // ShowMessage.Info("QtyRecievedAndReturnd", true, "", "");
                                VIS.ADialog.info("QtyRecievedAndReturnd");
                            }
                            mTab.setValue("MovementQty", shippedQty);
                            movementQty = shippedQty;

                            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                            paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", movementQty.toString());
                            qtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);
                            //qtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                            //        C_UOM_To_ID, movementQty);
                            if (qtyEntered == null) {
                                qtyEntered = movementQty;
                            }
                            mTab.setValue("QtyEntered", qtyEntered);
                            mTab.setValue("MovementQty", movementQty);
                            this.log.fine("qtyEntered : " + qtyEntered.toString() +
                                      "movementQty : " + movementQty.toString());
                        }
                    }
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOutLine - ASI.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Asi = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_ASI_ID = Util.getValueOfInt(value);// (int)value;
        if (M_ASI_ID == null || M_ASI_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
            //var M_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
            //var M_Locator_ID = VIS.DB.executeScalar("SELECT MIN(M_Locator_ID) FROM M_Locator WHERE IsActive = 'Y' AND M_Warehouse_ID = " + M_Warehouse_ID);
            var M_Locator_ID = VIS.dataContext.getJSONRecord("MinOut/GetWHLocator", M_Warehouse_ID.toString());
            var selectedM_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");

            // JID_0805: On selection of attribute set instance callout is setting the default locator and replace the previous selected locator.
            if (selectedM_Locator_ID == 0) {               
                mTab.setValue("M_Locator_ID", M_Locator_ID);
                ctx.setContext(windowNo, "M_Locator_ID", M_Locator_ID);
            }
            
            this.log.fine("M_Product_ID=" + M_Product_ID
                + ", M_ASI_ID=" + M_ASI_ID
                + " - M_Warehouse_ID=" + M_Warehouse_ID
                + ", M_Locator_ID=" + M_Locator_ID);

            //	Check Selection
            //var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
            //if (M_ASI_ID == M_AttributeSetInstance_ID) {
            //    var selectedM_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
            //    if (selectedM_Locator_ID != 0) {
            //        this.log.fine("Selected M_Locator_ID=" + selectedM_Locator_ID);
            //        mTab.setValue("M_Locator_ID", selectedM_Locator_ID);
            //    }
            //}
        }
        catch (errx) {
            this.setCalloutActive(false);
            this.log.severe(ex.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutInOut = CalloutInOut;
    //**************CalloutInOut End********************

})(VIS, jQuery);