; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //**************CalloutInvoice Start*********
    function CalloutInvoice() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInvoice"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutInvoice, VIS.CalloutEngine);//inherit CalloutEngine

    /**
     *	Invoice Header - DocType.
     *		- PaymentRule
     *		- temporary Document
     *  Context:
     *  	- DocSubTypeSO
     *		- HasCharges
     *	- (re-sets Business Partner info of required)
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        var C_DocType_ID = value;//(int)value;
        if (C_DocType_ID == null || C_DocType_ID == 0)
            return "";

        //var sql = "SELECT d.HasCharges,'N',d.IsDocNoControlled,"
        //    + "s.CurrentNext, d.DocBaseType "
        //    /*//jz outer join
        //    + "FROM C_DocType d, AD_Sequence s "
        //    + "WHERE C_DocType_ID=?"		//	1
        //    + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
        //    */
        //    + "FROM C_DocType d "
        //    + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
        //    + "WHERE C_DocType_ID=" + C_DocType_ID;		//	1

        var paramString = C_DocType_ID.toString();
        var dr = VIS.dataContext.getJSONRecord("MDocType/GetDocTypeData", paramString);

        try {
            //	Charges - Set Context
            ctx.setContext(windowNo, "HasCharges", Util.getValueOfString(dr["HasCharges"]));

            //	DocumentNo
            if (dr["IsDocNoControlled"] == "Y") {
                mTab.setValue("DocumentNo", "<" + Util.getValueOfString(dr["CurrentNext"]) + ">");
            }

            //  DocBaseType - Set Context
            var s = Util.getValueOfString(dr["DocBaseType"]);
            ctx.setContext(windowNo, "DocBaseType", s);

            //  AP Check & AR Credit Memo
            if (s.startsWith("AP")) {
                mTab.setValue("PaymentRule", "S");    //  Check
            }
            else if (s.endsWith("C")) {
                mTab.setValue("PaymentRule", "P");    //  OnCredit
            }

            // set Value Of Treat As Discount - in case of AP Credit memo only
            if (mTab.getField("TreatAsDiscount") != null) {
                if (s.equals("APC")) {
                    mTab.setValue("TreatAsDiscount", Util.getValueOfBoolean(dr["TreatAsDiscount"]));
                }
                else {
                    mTab.setValue("TreatAsDiscount", false);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
            return err.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /**
     *	Invoice Header- BPartner.
     *		- M_PriceList_ID (+ Context)
     *		- C_BPartner_Location_ID
     *		- AD_User_ID
     *		- POReference
     *		- SO_Description
     *		- IsDiscountPrinted
     *		- PaymentRule
     *		- C_PaymentTerm_ID
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = Util.getValueOfInt(value);//(int)value;
            if (C_BPartner_ID == null || C_BPartner_ID == 0) {
                return "";
            }
            var isvendor = 'N';
            var isCustomer = 'N';
            //var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
            //    + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
            //    + " p.SO_Description,p.IsDiscountPrinted,"
            //    + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            //    + " l.C_BPartner_Location_ID,c.AD_User_ID,"
            //    + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
            //    + "FROM C_BPartner p"
            //    + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
            //    + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
            //    + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            //    + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            //-----------------ANuj----Code----------
            var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,";

            var _CountVA009 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (_CountVA009 > 0) {
                //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
                sql += " p.VA009_PaymentMethod_ID, p.VA009_PO_PaymentMethod_ID, ";
                //p.VA009_PO_PaymentMethod_ID, ";
            }

            sql += "p.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID, "
                + " p.SalesRep_ID,p.IsSalesRep "
                + " FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            var isSOTrx = ctx.isSOTrx(windowNo);
            var dr = null;
            var drl = null;
            try {

                dr = VIS.DB.executeReader(sql, null, null);

                if (dr.read()) {
                    //	PriceList & IsTaxIncluded & Currency
                    var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                    if (ii > 0) {
                        mTab.setValue("M_PriceList_ID", ii);
                    }
                    // JID_0364: If price list not available at BP, user need to select it manually

                    //else {	//	get default PriceList
                    //    var i = ctx.getContextAsInt("#M_PriceList_ID");
                    //    if (i != 0) {
                    //        mTab.setValue("M_PriceList_ID", i);
                    //    }
                    //}

                    //	PaymentRule
                    var s = Util.getValueOfString(dr.get(isSOTrx ? "paymentrule" : "paymentrulepo"));
                    if (s != null && s.length != 0) {
                        if (ctx.getContext("DocBaseType").toString().endsWith("C"))	//	Credits are Payment Term
                        {
                            s = "P";
                        }
                        else if (isSOTrx && (s.toString().equals("S") || s.toString().equals("U")))	//	No Check/Transfer for SO_Trx
                        {
                            s = "P";											//  Payment Term
                        }
                        mTab.setValue("PaymentRule", s);
                    }
                    //payment method ID set to Column----Anuj- 04-09-2015-------------------------------------------------------------------------------------------

                    var _CountVA009 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
                    if (_CountVA009 > 0) {
                        var _PaymentMethod_ID = Util.getValueOfInt(dr.get("VA009_PaymentMethod_ID"));
                        ////VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
                        var isvendor = 'N';
                        var bpdtl = VIS.dataContext.getJSONRecord("MBPartner/GetBPDetails", C_BPartner_ID);
                        if (bpdtl != null) {
                            isvendor = Util.getValueOfString(bpdtl["IsVendor"]);
                            isCustomer = Util.getValueOfString(bpdtl["IsCustomer"]);
                            if (!isSOTrx) { //In case of Purchase Order
                                if (isvendor == "Y") {
                                    _PaymentMethod_ID = Util.getValueOfInt(bpdtl["VA009_PO_PaymentMethod_ID"]);
                                    PaymentBasetype = Util.getValueOfString(bpdtl["VA009_PAYMENTBASETYPEPO"]);
                                }
                                else {
                                    _PaymentMethod_ID = 0;
                                    PaymentBasetype = null;
                                }
                            }
                            else {
                                if (isvendor == "Y") {
                                    _PaymentMethod_ID = 0;
                                    PaymentBasetype = null;
                                    if (isCustomer == "Y") {
                                        _PaymentMethod_ID = Util.getValueOfInt(bpdtl["VA009_PaymentMethod_ID"]);
                                        PaymentBasetype = Util.getValueOfString(bpdtl["VA009_PAYMENTBASETYPE"]);
                                    }
                                }
                                else {
                                    if (isCustomer == "Y") {
                                        _PaymentMethod_ID = Util.getValueOfInt(bpdtl["VA009_PaymentMethod_ID"]);
                                        PaymentBasetype = Util.getValueOfString(bpdtl["VA009_PAYMENTBASETYPE"]);
                                    }
                                }

                            }
                        }

                        if (_PaymentMethod_ID == 0)
                            mTab.setValue("VA009_PaymentMethod_ID", null);
                        else {
                            mTab.setValue("VA009_PaymentMethod_ID", _PaymentMethod_ID);
                            var PaymentBasetype = VIS.DB.executeScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID=" + _PaymentMethod_ID);
                            if (PaymentBasetype != null) {
                                //if (PaymentBasetype != null) {
                                mTab.setValue("PaymentMethod", PaymentBasetype);
                                if (isvendor == 'N')
                                    mTab.setValue("PaymentRule", PaymentBasetype);
                                else
                                    mTab.setValue("PaymentRule", PaymentBasetype);
                            }
                            else {
                                mTab.setValue("PaymentMethod", "T");
                                if (isvendor == 'N')
                                    mTab.setValue("PaymentRule", "T");
                                else
                                    mTab.setValue("PaymentRule", "T");
                            }
                        }
                    }

                    //payment method ID set to Column----Anuj- 04-09-2015-----------------------------------------

                    //  Payment Term

                    var PaymentTermPresent = Util.getValueOfInt(mTab.getValue("C_PaymentTerm_ID")); // from BSO/BPO window
                    var C_Order_Blanket = Util.getValueOfDecimal(mTab.getValue("C_Order_Blanket"))
                    if (PaymentTermPresent > 0 && C_Order_Blanket > 0) {
                    }
                    else {
                        ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                        //if (!dr.wasNull())
                        if (ii > 0) {
                            mTab.setValue("C_PaymentTerm_ID", ii);
                        }
                    }
                    //	Location
                    var locID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may bevar  to differnt BP)
                    if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                        var loc = ctx.getContext("C_BPartner_Location_ID");
                        if (loc && loc.toString().length > 0) {
                            locID = parseInt(loc);
                        }
                    }
                    if (locID == 0) {
                        mTab.setValue("C_BPartner_Location_ID", null);
                    }
                    else {
                        mTab.setValue("C_BPartner_Location_ID", locID);
                    }

                    //	Contact - overwritten by InfoBP selection
                    var contID = Util.getValueOfInt(dr.get("ad_user_id"));
                    if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                        var cont = ctx.getContext("AD_User_ID");
                        if (cont && cont.toString().length > 0) {
                            contID = parseInt(cont);
                        }
                    }
                    if (contID == 0) {
                        mTab.setValue("AD_User_ID", null);
                    }
                    else {
                        mTab.setValue("AD_User_ID", contID);
                    }
                    //Arpit 15th Apr,2017 -To set SalesRepID
                    //var isSalesRep = Util.getValueOfBoolean(dr.get("issalesrep"))
                    //if (isSalesRep) {
                    //    var SalesRepID = Util.getValueOfInt(dr.get("salesrep_id"));
                    //    if (SalesRepID > 0) {
                    //        mTab.setValue("SalesRep_ID", contID);
                    //    }
                    //}
                    //Code End here
                    //	CreditAvailable
                    if (isSOTrx) {
                        //var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                        //if (CreditLimit != 0) {
                        //    var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                        //    if (CreditAvailable < 0) {
                        //        VIS.ADialog.info("CreditLimitOver");
                        //    }
                        //}
                        var CreditStatus = dr.getString("CreditStatusSettingOn");
                        if (CreditStatus == "CH") {
                            var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                            //	var SOCreditStatus = dr.getString("SOCreditStatus");
                            if (CreditLimit != 0) {
                                var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                                if (dr != null && CreditAvailable <= 0) {
                                    //VIS.ADialog.info("CreditLimitOver", null, "", "");
                                    VIS.ADialog.info("CreditOver");
                                }
                            }
                        }
                        else {
                            var locId = Util.getValueOfInt(mTab.getValue("C_BPartner_Location_ID"));
                            // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019 
                            sql = "SELECT bp.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable" +
                                " FROM C_BPartner_Location p INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = p.C_BPartner_ID) WHERE p.C_BPartner_Location_ID = " + locId;
                            drl = VIS.DB.executeReader(sql);
                            if (drl.read()) {
                                CreditStatus = drl.getString("CreditStatusSettingOn");
                                if (CreditStatus == "CL") {
                                    var CreditLimit = Util.getValueOfDouble(drl.get("so_creditlimit"));
                                    //	var SOCreditStatus = dr.getString("SOCreditStatus");
                                    if (CreditLimit != 0) {
                                        var CreditAvailable = Util.getValueOfDouble(drl.get("creditavailable"));
                                        if (dr != null && CreditAvailable <= 0) {
                                            //VIS.ADialog.info("CreditLimitOver", null, "", "");
                                            VIS.ADialog.info("CreditOver");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //	PO Reference
                    s = Util.getValueOfString(dr.get("poreference"));
                    if (s != null && s.length != 0) {
                        mTab.setValue("POReference", s);
                    }
                    else {
                        mTab.setValue("POReference", null);
                    }
                    //	SO Description
                    s = Util.getValueOfString(dr.get("so_description"));
                    if (s != null && s.toString().trim().length != 0) {
                        mTab.setValue("Description", s);
                    }
                    //	IsDiscountPrinted
                    s = Util.getValueOfString(dr.get("isdiscountprinted"));
                    if (s != null && s.toString().trim().length != 0) {
                        mTab.setValue("IsDiscountPrinted", s);
                    }
                    else {
                        mTab.setValue("IsDiscountPrinted", "N");
                    }
                }
                dr.close();
            }
            catch (err) {
                this.setCalloutActive(false);
                if (dr != null) {
                    dr.close();
                }
                if (drl != null) {
                    drl.close();
                }
                this.log.log(Level.SEVERE, "bPartner", err);
                return err.message;
            }
            finally {
                if (dr != null) {
                    dr.close();
                }
                if (drl != null) {
                    drl.close();
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

    /**
     *	Set Payment Term.
     *	Payment Term has changed 
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.PaymentTerm = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_PaymentTerm_ID = value;
            var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID", false);
            var get_ID;
            var apply;
            if (C_PaymentTerm_ID == null || C_PaymentTerm_ID == 0
                || C_Invoice_ID == 0)	//	not saved yet
            {
                return "";
            }
            //
            var paramString = C_PaymentTerm_ID.toString().concat(",", C_Invoice_ID.toString());

            var dr = VIS.dataContext.getJSONRecord("MPaymentTerm/GetPaymentTerm", paramString);

            // Added by Bharat on 18 July 2017 as issue given by Ravikant on Payment Term selection giving Internal server error..
            if (dr != null) {
                get_ID = dr["Get_ID"];
                var valid = dr["Apply"];
                if (get_ID == 0) {
                    return "PaymentTerm not found";
                }

                // JID_0576: "Getting error ""Could not save changes - data was changed after query. IsPayScheduleValid"" while changing payment term after adding invoice line"                
                mTab.setValue("IsPayScheduleValid", valid ? true : false);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /***
     *	Invoice Line - Product.
     *		- reset C_Charge_ID / M_AttributeSetInstance_ID
     *		- PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
     *		- UOM
     *	Calls Tax
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            mTab.setValue("VA038_AmortizationTemplate_ID", 0);
            return "";
        }
        debugger;

        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";
        this.setCalloutActive(true);
        try {

            mTab.setValue("C_Charge_ID", null);

            //	Set Attribute
            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

            //if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID
            //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false) != 0) {
            //    mTab.setValue("M_AttributeSetInstance_ID", ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false));
            //}
            //else {
            mTab.setValue("M_AttributeSetInstance_ID", null);
            //}

            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

            /***** Amit ****/
            sql = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'";
            countEd011 = Util.getValueOfInt(VIS.DB.executeScalar(sql));
            var invoiceRecord = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", mTab.getValue("C_Invoice_ID").toString());

            var purchasingUom = 0;
            if (countEd011 > 0) {
                sql = "SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND  C_BPartner_ID = " + Util.getValueOfInt(invoiceRecord["C_BPartner_ID"]) +
                    " AND M_Product_ID = " + M_Product_ID;
                purchasingUom = Util.getValueOfInt(VIS.DB.executeScalar(sql));

                if (purchasingUom > 0 && isSOTrx == false) {
                    mTab.setValue("C_UOM_ID", purchasingUom);
                }
                else {
                    sql = "SELECT C_UOM_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + M_Product_ID;
                    mTab.setValue("C_UOM_ID", Util.getValueOfInt(VIS.DB.executeScalar(sql)));
                }
            }

            /*****Amit done *******/

            //try
            //{
            //    object[] pqtyAll = VAdvantage.Classes.InfoLines.PQ.ToArray();
            //    for (int x = 0; x < pqtyAll.Length; x++)
            //    {
            //        object f = pqtyAll.GetValue(x);

            //        int AD_Session_ID = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._AD_Session_ID);
            //        int winNo = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._windowNo);
            //        Dictionary<int, Decimal> ProductQty = ((VAdvantage.Classes.InfoLines)f)._prodQty;

            //        List<int> key = ProductQty.Keys.ToList();
            //        if (AD_Session_ID == Env.GetCtx().GetAD_Session_ID() && winNo == windowNo && Util.getValueOfInt(value) == Util.getValueOfInt(key[0]))
            //        {
            //            Decimal qty = Util.getValueOfDecimal(ProductQty[Util.getValueOfInt(value)]);
            //            mTab.setValue("QtyEntered", qty);
            //            mTab.setValue("QtyInvoiced", qty);
            //            VAdvantage.Classes.InfoLines.PQ.RemoveAt(x);
            //            break;
            //        }

            //    }
            //}
            //catch
            //{
            //    for (int k = 0; k < VAdvantage.Classes.InfoLines.PQ.Count; k++)
            //    {
            //        VAdvantage.Classes.InfoLines.PQ.RemoveAt(k);
            //    }
            //}

            /*****	Price Calculation see also qty	****/

            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var Qty = mTab.getValue("QtyInvoiced");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID", false);
            // pp.SetM_PriceList_ID(M_PriceList_ID);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
            // pp.setM_PriceList_Version_ID(M_PriceList_Version_ID);
            //var  time = ctx.getContextAsTime(windowNo, "DateInvoiced");
            //pp.SetPriceDate(time);
            var time = ctx.getContext("DateInvoiced");
            //pp.setPriceDate1(time);

            /*** Amit ***/
            var C_UOM_ID = Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
            /*** Amit Done ***/

            //var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
            //                                               Qty.toString(), ",", //3
            //                                               isSOTrx, ",", //4 
            //                                               M_PriceList_ID.toString(), ",", //5
            //                                               M_PriceList_Version_ID.toString(), ",", //6
            //                                               null, ",",//7
            //                                               time.toString(), ",",
            //                                               M_AttributeSetInstance_ID.toString()); //8

            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                          Qty.toString(), ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID.toString(), ",", //5
                                                          M_PriceList_Version_ID.toString(), ",", //6
                                                          null, ",",//7
                                                          time.toString(), ",", //8
                                                          M_AttributeSetInstance_ID.toString(), ",", //9
                                                           C_UOM_ID, ",", countEd011); // 10 , 11




            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            var rowDataDB = null;


            // MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, Qty, isSOTrx);


            //		
            mTab.setValue("PriceList", dr["PriceList"]);
            mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PriceActual", dr.PriceActual);
            mTab.setValue("PriceEntered", dr.PriceEntered);
            mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            // mTab.setValue("Discount", dr.Discount);
            if (countEd011 <= 0) {
                mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
            }
            // mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));

            //Start UOM
            if (countEd011 > 0 && purchasingUom > 0 && isSOTrx == false) {
                //Get UOM from Product
                var paramString = M_Product_ID.toString();
                var C_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramString);

                //Get priceList From SO/PO
                var M_PriceList_ID = Util.getValueOfInt(invoiceRecord["M_PriceList_ID"]);

                //Get PriceListversion based on Pricelist
                var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", M_PriceList_ID.toString());

                //Get StandardPrecision From UOM
                var standardPrecision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", C_UOM_ID.toString());

                var bpartner1 = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID.toString());

                sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                                    + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                                    + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID=" + purchasingUom;
                var ds = VIS.DB.executeDataSet(sql);
                if (ds.getTables().length > 0) {
                    if (ds.getTables()[0].getRows().length > 0) {
                        // start flat discount

                        //var actualPrice1 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                        //      bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                        // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                        paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                        bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                        var actualPrice1 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                        mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                        mTab.setValue("PriceLimit", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceLimit")));
                        mTab.setValue("PriceActual", actualPrice1);
                        mTab.setValue("PriceEntered", actualPrice1);
                        //set Conversion Ordered Qty

                        sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' AND con.M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID")) +
                          " AND con.C_UOM_ID = " + C_UOM_ID + " AND con.C_UOM_To_ID = " + purchasingUom;
                        var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        if (rate == 0) {
                            sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                               " AND con.C_UOM_ID = " + C_UOM_ID + " AND con.C_UOM_To_ID = " + purchasingUom;
                            rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        }
                        if ((C_UOM_ID == purchasingUom) && (rate == 0)) {
                            rate = 1;
                        }
                        mTab.setValue("QtyInvoiced", (((Util.getValueOfDecimal(mTab.getValue("QtyEntered"))).toFixed(standardPrecision)) * rate));
                    }
                    else {
                        sql = "SELECT PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                                + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID=" + C_UOM_ID;
                        var pricelist = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));

                        sql = "SELECT PriceStd FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                             + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                             + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID=" + C_UOM_ID;
                        var pricestd = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));

                        //start flat discount

                        //pricestd = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), pricestd,
                        //   bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                        // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                        paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", pricestd.toString(), ",",
                        bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                        pricestd = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                        sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' AND con.M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID")) +
                               " AND con.C_UOM_ID = " + C_UOM_ID + " AND con.C_UOM_To_ID = " + purchasingUom;
                        var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        if (rate == 0) {
                            sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                              " AND con.C_UOM_ID = " + C_UOM_ID + " AND con.C_UOM_To_ID = " + purchasingUom;
                            rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        }
                        mTab.setValue("PriceList", (pricelist * rate));
                        mTab.setValue("PriceActual", (pricestd * rate));
                        mTab.setValue("PriceEntered", (pricestd * rate));
                        // If rate = zero then change it to 1 By Vaibhav
                        if (rate == 0) {
                            rate = 1;
                        }
                        mTab.setValue("QtyInvoiced", ((Util.getValueOfDecimal(mTab.getValue("QtyEntered")).toFixed(standardPrecision)) * rate));
                    }
                }
                // ds.close();
            }
            //End UOM

            // Change Done By Mohit Aortization Process 02/11/2016
            //var countVA038 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
            //if (countVA038 > 0 && !isSOTrx) {
            //    var StartDate = new Date();
            //    var EndDate = new Date();
            //    var ds1 = VIS.DB.executeDataSet("SELECT VA038_AmortizationTemplate_ID FROM M_Product where IsActive='Y' AND M_Product_ID=" + Util.getValueOfInt(value));
            //    if (ds1.getTables().length > 0) {
            //        if (ds1.getTables()[0].getRows().length > 0) {
            //            if (Util.getValueOfInt(ds1.getTables()[0].getRows()[0].getCell("VA038_AmortizationTemplate_ID")) > 0) {
            //                mTab.setValue("VA038_AmortizationTemplate_ID", Util.getValueOfInt(ds1.getTables()[0].getRows()[0].getCell("VA038_AmortizationTemplate_ID")));
            //                var Sql = "SELECT VA038_PeriodType,VA038_AMORTIZATIONPERIOD,VA038_TERMSOURCE from VA038_Amortizationtemplate WHERE VA038_Amortizationtemplate_ID=" + VIS.Utility.Util.getValueOfInt(mTab.getValue("VA038_AmortizationTemplate_ID"));
            //                var ds2 = VIS.DB.executeDataSet(Sql);
            //                Sql = "";
            //                if (ds2.getTables()[0].getRows().length > 0) {
            //                    if (VIS.Utility.Util.getValueOfString(ds2.getTables()[0].getRows()[0].getCell("VA038_TERMSOURCE")) == "A") {
            //                        Sql = "SELECT DATEINVOICED FROM C_Invoice WHERE C_Invoice_ID=" + mTab.getValue("C_Invoice_ID");
            //                        StartDate = Util.getValueOfDate(VIS.DB.executeScalar(Sql));
            //                        Sql = "";
            //                    }
            //                    if (VIS.Utility.Util.getValueOfString(ds2.getTables()[0].getRows()[0].getCell("VA038_TERMSOURCE")) == "T") {
            //                        Sql = "SELECT DATEINVOICED FROM C_Invoice WHERE C_Invoice_ID=" + mTab.getValue("C_Invoice_ID");
            //                        StartDate = Util.getValueOfDate(VIS.DB.executeScalar(Sql));
            //                        Sql = "";
            //                    }
            //                    EndDate = StartDate;
            //                    if (Util.getValueOfString(ds2.getTables()[0].getRows()[0].getCell("VA038_PeriodType")) == "M") {
            //                        EndDate.setMonth(StartDate.getMonth() + VIS.Utility.Util.getValueOfInt(ds2.getTables()[0].getRows()[0].getCell("VA038_AMORTIZATIONPERIOD")));
            //                        mTab.setValue("FROMDATE", StartDate);
            //                        mTab.setValue("EndDate", EndDate);
            //                    }
            //                    if (Util.getValueOfString(ds2.getTables()[0].getRows()[0].getCell("VA038_PeriodType")) == "Y") {
            //                        EndDate.setYear(StartDate.getYear() + VIS.Utility.Util.getValueOfInt(ds2.getTables()[0].getRows()[0].getCell("VA038_AMORTIZATIONPERIOD")));
            //                        mTab.setValue("FROMDATE", StartDate);
            //                        mTab.setValue("EndDate", EndDate);
            //                    }
            //                }
            //            }
            //            else {
            //                mTab.setValue("VA038_AmortizationTemplate_ID", 0);
            //            }
            //        }
            //    }
            //}
            // End Change Amortization

            ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
            ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");



            //mTab.setValue("PriceList", pp.GetPriceList());
            //mTab.setValue("PriceLimit", pp.GetPriceLimit());
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            //mTab.setValue("PriceEntered", pp.GetPriceStd());
            //mTab.setValue("C_Currency_ID", pp.GetC_Currency_ID());
            //mTab.setValue("C_UOM_ID", pp.GetC_UOM_ID());
            //ctx.setContext(windowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
            //ctx.setContext(windowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };

    /**
     * 	Calculate Discout based on Discount Schema selected on Business Partner
     *	@param ProductId Product ID
     *	@param ClientId Tenant
     *	@param amount Amount on which discount is to be calculated
     *	@param DiscountSchemaId Discount Schema of Business Partner
     *	@param FlatDiscount Flat Discount % on Business Partner
     *	@param QtyEntered Quantity on which discount is to be calculated
     *	@return Discount value
     */
    //CalloutInvoice.prototype.FlatDiscount = function (ProductId, ClientId, amount, DiscountSchemaId, FlatDiscount, QtyEntered) {
    //    var amountAfterBreak = amount;
    //    var sql = "SELECT UNIQUE M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId;
    //    var productCategoryId = Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    var isCalulate = false;
    //    var dsDiscountBreak = null;
    //    var discountType = "";
    //    // Is flat Discount
    //    // JID_0487: Not considering Business Partner Flat Discout Checkbox value while calculating the Discount
    //    sql = "SELECT  DiscountType, IsBPartnerFlatDiscount, FlatDiscount FROM M_DiscountSchema WHERE "
    //              + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId;
    //    dsDiscountBreak = VIS.DB.executeDataSet(sql);

    //    if (dsDiscountBreak != null && dsDiscountBreak.getTables().length > 0) {
    //        discountType = Util.getValueOfString(dsDiscountBreak.getTables()[0].getRows()[0].getCell("DiscountType"));
    //        if (discountType == "F") {
    //            var discountBreakValue = 0;
    //            isCalulate = true;
    //            if (Util.getValueOfString(dsDiscountBreak.getTables()[0].getRows()[0].getCell("IsBPartnerFlatDiscount")) == "N") {
    //                discountBreakValue = (amount - ((amount * Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[0].getCell("FlatDiscount"))) / 100));
    //            }
    //            else {
    //                discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
    //            }
    //            amountAfterBreak = discountBreakValue;
    //            return amountAfterBreak;
    //        }

    //        else if (discountType == "B") {

    //            // Product Based
    //            sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
    //                       + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
    //                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
    //            dsDiscountBreak = VIS.DB.executeDataSet(sql);
    //            if (dsDiscountBreak.getTables().length > 0) {
    //                var m = 0;
    //                var discountBreakValue = 0;

    //                for (m = 0; m < dsDiscountBreak.getTables()[0].getRows().length; m++) {
    //                    if (QtyEntered < Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakValue"))) {
    //                        continue;
    //                    }
    //                    if (Util.getValueOfString(dsDiscountBreak.getTables()[0].getRows()[m].getCell("IsBPartnerFlatDiscount")) == "N") {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - (amount * Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakDiscount")) / 100));
    //                        break;
    //                    }
    //                    else {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
    //                        break;
    //                    }
    //                }
    //                if (isCalulate) {
    //                    amountAfterBreak = discountBreakValue;
    //                    return amountAfterBreak;
    //                }
    //            }
    //            //

    //            // Product Category Based
    //            sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
    //                       + " M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
    //                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
    //            dsDiscountBreak = VIS.DB.executeDataSet(sql);
    //            if (dsDiscountBreak.getTables().length > 0) {
    //                var m = 0;
    //                var discountBreakValue = 0;

    //                for (m = 0; m < dsDiscountBreak.getTables()[0].getRows().length; m++) {
    //                    if (QtyEntered < Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakValue"))) {
    //                        continue;
    //                    }
    //                    if (Util.getValueOfString(dsDiscountBreak.getTables()[0].getRows()[m].getCell("IsBPartnerFlatDiscount")) == "N") {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - (amount * Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakDiscount")) / 100));
    //                        break;
    //                    }
    //                    else {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
    //                        break;
    //                    }
    //                }
    //                if (isCalulate) {
    //                    amountAfterBreak = discountBreakValue;
    //                    return amountAfterBreak;
    //                }
    //            }
    //            //

    //            // Otherwise
    //            sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
    //                       + " M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
    //                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
    //            dsDiscountBreak = VIS.DB.executeDataSet(sql);
    //            if (dsDiscountBreak.getTables().length > 0) {
    //                var m = 0;
    //                var discountBreakValue = 0;

    //                for (m = 0; m < dsDiscountBreak.getTables()[0].getRows().length; m++) {
    //                    if (QtyEntered < Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakValue"))) {
    //                        continue;
    //                    }
    //                    if (Util.getValueOfString(dsDiscountBreak.getTables()[0].getRows()[m].getCell("IsBPartnerFlatDiscount")) == "N") {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - (amount * Util.getValueOfDecimal(dsDiscountBreak.getTables()[0].getRows()[m].getCell("BreakDiscount")) / 100));
    //                        break;
    //                    }
    //                    else {
    //                        isCalulate = true;
    //                        discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
    //                        break;
    //                    }
    //                }
    //                if (isCalulate) {
    //                    amountAfterBreak = discountBreakValue;
    //                    return amountAfterBreak;
    //                }
    //            }
    //        }
    //    }

    //    return amountAfterBreak;
    //};

    /**
     *	Invoice Line - Charge.
     * 		- updates PriceActual from Charge
     * 		- sets PriceLimit, PriceList to zero
     * 	Calles tax
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            mTab.setValue("VA038_AmortizationTemplate_ID", 0);
            return "";
        }
        var dr = null;
        try {
            var C_Charge_ID = value;
            if (C_Charge_ID == null || C_Charge_ID == 0)
                return "";

            //	No Product defined
            if (mTab.getValue("M_Product_ID") != null) {
                mTab.setValue("C_Charge_ID", null);
                return "ChargeExclusively";
            }
            mTab.setValue("M_AttributeSetInstance_ID", null);
            mTab.setValue("S_ResourceAssignment_ID", null);

            //JID_0054: Currently System setting the Each as UOM after selecting the cahrge. Need to set default UOM for charge.
            var c_uom_id = ctx.getContextAsInt("#C_UOM_ID");
            if (c_uom_id > 0) {
                mTab.setValue("C_UOM_ID", c_uom_id);	//	Default UOM from context.
            }
            else {
                mTab.setValue("C_UOM_ID", 100);	//	EA
            }
            ctx.setContext(windowNo, "DiscountSchema", "N");
            var sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;

            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                mTab.setValue("PriceEntered", Util.getValueOfDecimal(dr.get("chargeamt")));//.getBigDecimal(1));
                mTab.setValue("PriceActual", Util.getValueOfDecimal(dr.get("chargeamt")));//dr.getBigDecimal(1));
                mTab.setValue("PriceLimit", 0);
                mTab.setValue("PriceList", 0);
                mTab.setValue("Discount", 0);
            }
            dr.close();

            // Change Done By Mohit Aortization Process 02/11/2016
            //var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            //var countVA038 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
            //if (countVA038 > 0 && !isSOTrx) {
            //    var ds1 = VIS.DB.executeDataSet("SELECT VA038_AmortizationTemplate_ID FROM C_Charge where IsActive='Y' AND C_Charge_ID=" + Util.getValueOfInt(value));
            //    if (ds1.getTables().length > 0) {
            //        if (ds1.getTables()[0].getRows().length > 0) {
            //            if (Util.getValueOfInt(ds1.getTables()[0].getRows()[0].getCell("VA038_AmortizationTemplate_ID")) > 0) {
            //                mTab.setValue("VA038_AmortizationTemplate_ID", Util.getValueOfInt(ds1.getTables()[0].getRows()[0].getCell("VA038_AmortizationTemplate_ID")));
            //            }
            //            else {
            //                mTab.setValue("VA038_AmortizationTemplate_ID", 0);
            //            }
            //        }
            //    }
            //}
            // End Change Amortization
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            return err.message;
        }
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };

    /**
     *	Invoice Line - Tax.
     *		- basis: Product, Charge, BPartner Location
     *		- sets C_Tax_ID
     *  Calles Amount
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var column = mField.getColumnName();
        try {
            /**** Start Amit For Tax Type Module ****/
            var taxRule = "";
            var sql = "";
            //var paramString = "";
            var params = Util.getValueOfString(mTab.getValue("C_Invoice_ID")).concat(",", (mTab.getValue("M_Product_ID")).toString() +
                "," + Util.getValueOfString(mTab.getValue("C_Charge_ID")));
            var recDic = VIS.dataContext.getJSONRecord("MInvoice/GetTax", params);

            //var _CountVATAX = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX IN ('VATAX_' )"));
            var _CountVATAX = Util.getValueOfInt(recDic["_CountVATAX"]);

            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

            //paramString = mTab.getValue("C_Invoice_ID").toString();
            //var invoice = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);

            //sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + Util.getValueOfInt(invoice["AD_Org_ID"]) + " AND IsActive ='Y' AND AD_Client_ID =" + ctx.getAD_Client_ID();

            if (_CountVATAX > 0) {
                //taxRule = Util.getValueOfString(VIS.DB.executeScalar(sql));
                taxRule = Util.getValueOfString(recDic["taxRule"]);
            }
            if (taxRule == "T") {
                var taxid = Util.getValueOfInt(recDic["taxId"]);

                //var taxid = 0;
                //sql = "SELECT Count(*) FROM AD_Column WHERE ColumnName = 'C_Tax_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'C_TaxCategory')";
                //if (Util.getValueOfInt(VIS.DB.executeScalar(sql)) > 0) {
                //    paramString = Util.getValueOfInt(mTab.getValue("C_Invoice_ID")).toString() + "," + Util.getValueOfInt(mTab.getValue("M_Product_ID")).toString() +
                //        "," + Util.getValueOfInt(mTab.getValue("C_Charge_ID")).toString();
                //    taxid = VIS.dataContext.getJSONRecord("MInvoice/GetTax", paramString);
                //}
                //else {
                //    sql = "select vatax_taxtype_id from c_bpartner_location where c_bpartner_id =" + util.getvalueofint(invoice["c_bpartner_id"]) +
                //                      " and isactive = 'Y'  and c_bpartner_location_id = " + util.getvalueofint(invoice["c_bpartner_location_id"]);
                //    var taxtype = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //    if (taxtype == 0) {
                //        sql = "select vatax_taxtype_id from c_bpartner where c_bpartner_id =" + util.getvalueofint(invoice["c_bpartner_id"]) + " and isactive = 'Y'";
                //        taxtype = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //    }
                //    var prodtaxcategory = vis.datacontext.getjsonrecord("MProduct/GetTaxCategory", value.tostring());
                //    sql = "select c_tax_id from vatax_taxcatrate where c_taxcategory_id = " + prodtaxcategory + " and isactive ='Y' and vatax_taxtype_id =" + taxtype;
                //    taxid = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //}
                if (taxid > 0) {
                    mTab.setValue("C_Tax_ID", taxid);
                }
                else {
                    if (Util.getValueOfInt(mTab.getValue("M_Product_ID")) > 0) {
                        mTab.setValue("M_Product_ID", "");
                        this.setCalloutActive(false);
                        if (recDic["TaxExempt"] == "Y") {
                            return VIS.ADialog.info("TaxNoExemptFound");
                        }
                        else {
                            return VIS.ADialog.info("VATAX_DefineTax");
                        }
                    }
                    else if (Util.getValueOfInt(mTab.getValue("C_Charge_ID")) > 0) {
                        mTab.setValue("C_Charge_ID", "");
                        this.setCalloutActive(false);
                        if (recDic["TaxExempt"] == "Y") {
                            return VIS.ADialog.info("TaxNoExemptFound");
                        }
                        else {
                            return VIS.ADialog.info("VATAX_DefineChargeTax");
                        }
                    }
                }
            }
                /**** end Amit For Tax Type Module ****/
            else {
                //	Check Product
                var M_Product_ID = 0;
                if (column.toString() == "M_Product_ID") {
                    M_Product_ID = value;
                }
                else {
                    M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
                }
                var C_Charge_ID = 0;
                if (column.toString() == "C_Charge_ID") {
                    C_Charge_ID = value;
                }
                else {
                    C_Charge_ID = ctx.getContextAsInt(windowNo, "C_Charge_ID");
                }
                this.log.fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
                if (M_Product_ID == 0 && C_Charge_ID == 0) {
                    return this.Amt(ctx, windowNo, mTab, mField, value);
                }

                //	Check Partner Location
                var shipC_BPartner_Location_ID = ctx.getContextAsInt(windowNo, "C_BPartner_Location_ID");
                if (shipC_BPartner_Location_ID == 0) {
                    return this.Amt(ctx, windowNo, mTab, mField, value);
                }
                this.log.fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
                var billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
                this.log.fine("Bill BP_Location=" + billC_BPartner_Location_ID);

                //	Dates 
                //DateTime billDate = new DateTime(ctx.getContextAsTime(windowNo, "DateInvoiced"));
                var billDate = (ctx.getContext("DateInvoiced"));
                this.log.fine("Bill Date=" + billDate);
                var shipDate = billDate;
                this.log.fine("Ship Date=" + shipDate);

                var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
                this.log.fine("Org=" + AD_Org_ID);

                var M_Warehouse_ID = ctx.getContextAsInt("#M_Warehouse_ID");
                this.log.fine("Warehouse=" + M_Warehouse_ID);



                var paramString = C_Charge_ID.toString().concat(",", billDate.toString(),
                                                               shipDate.toString(), ",",
                                                                AD_Org_ID.toString(), ",",
                                                                M_Warehouse_ID.toString(), ",",
                                                                billC_BPartner_Location_ID.toString(), ",",
                                                                shipC_BPartner_Location_ID.toString(), ",",
                                                                ctx.getWindowContext(windowNo, "IsSOTrx", true).equals("Y"));


                var C_Tax_ID = Util.getValueOfInt(VIS.dataContext.getJSONRecord("MTax/Get", paramString));


                //var C_Tax_ID = VAdvantage.Model.Tax.Get(ctx, M_Product_ID, C_Charge_ID, billDate, shipDate,
                //    AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
                //    ctx.getContext("IsSOTrx").equals("Y"));
                this.log.info("Tax ID=" + C_Tax_ID);
                //
                if (C_Tax_ID == 0) {
                    //mTab.fireDataStatusEEvent(CLogger.retrieveError());
                    // VIS.ADialog.info("");
                    //  ShowMessage.Info(VLogger.RetrieveError().Key.toString(), null, VLogger.RetrieveError().Name, "");
                }
                else {
                    mTab.setValue("C_Tax_ID", C_Tax_ID);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    /**
     *	Invoice - Amount.
     *		- called from QtyInvoiced, PriceActual
     *		- calculates LineNetAmt
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        try {
            this.log.log(Level.WARNING, "amt - init");
            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");


            var paramStr = M_PriceList_ID.toString(); //1

            //Get product price information
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", paramStr);


            var StdPrecision = dr["StdPrecision"];;
            var QtyEntered, QtyInvoiced, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            //added by bharat Mantis id : 1230
            var orderline_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
            //end bharat
            QtyEntered = mTab.getValue("QtyEntered");
            QtyInvoiced = mTab.getValue("QtyInvoiced");
            this.log.fine("QtyEntered=" + QtyEntered + ", Invoiced=" + QtyInvoiced + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = mTab.getValue("PriceEntered");
            PriceActual = mTab.getValue("PriceActual");
            PriceLimit = mTab.getValue("PriceLimit");
            PriceList = mTab.getValue("PriceList");

            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual);// + ", Discount=" + Discount);

            //Start Amit UOM
            if (mField.getColumnName() == "QtyEntered" && countEd011 > 0) {

                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);

                if (orderline_ID == 0) {
                    paramStr = M_Product_ID.toString().concat(",", (mTab.getValue("C_Invoice_ID")).toString() +
                      "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                      "," + C_UOM_To_ID.toString() + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID.toString() +
                      "," + QtyEntered.toString());
                    var prices = VIS.dataContext.getJSONRecord("MInvoice/GetPrices", paramStr);

                    if (prices != null) {
                        PriceList = prices["PriceList"];
                        mTab.setValue("PriceList", PriceList);
                        PriceEntered = prices["PriceEntered"];
                        mTab.setValue("PriceLimit", prices["PriceLimit"]);
                        PriceActual = PriceEntered;
                        mTab.setValue("PriceActual", PriceActual);
                        mTab.setValue("PriceEntered", PriceEntered);
                    }
                }

                // Get Price List From SO/PO Header
                //paramString = (mTab.getValue("C_Invoice_ID")).toString();
                //var invoiceRecord = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);

                ////Get PriceListversion based on Pricelist
                //var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", invoiceRecord["M_PriceList_ID"].toString());

                //var C_BPartner_ID1 = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
                //var bpartner1 = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID1.toString());
                //if (orderline_ID == 0) {
                //    if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                //        sql = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                         + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                         + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                //                         + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                //    }
                //    else {
                //        sql = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                       + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                       + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                //                       + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                //    }
                //    var countPrice = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //    if (countPrice > 0) {
                //        // Selected UOM Price Exist
                //        if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                //            sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                        + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                        + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                //                        + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                //        }
                //        else {
                //            sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                      + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                //                      + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                //        }
                //        var ds = VIS.DB.executeDataSet(sql);
                //        if (ds.getTables().length > 0) {
                //            if (ds.getTables()[0].getRows().length > 0) {
                //                //Flat Discount

                //                //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                //bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                //                //PriceEntered = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd"));
                //                PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                //                mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //                mTab.setValue("PriceEntered", PriceEntered);
                //                PriceActual = PriceEntered;
                //                mTab.setValue("PriceActual", PriceActual);
                //            }
                //        }
                //    }
                //        // Added by bharat - Selected Price without attribute   
                //    else if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0 && countPrice == 0) {
                //        sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                  + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                //                  + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));

                //        var ds = VIS.DB.executeDataSet(sql);
                //        if (ds.getTables().length > 0) {
                //            if (ds.getTables()[0].getRows().length > 0) {
                //                //Flat Discount
                //                //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                //bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                //                PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                //                mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //                mTab.setValue("PriceEntered", PriceEntered);
                //                PriceActual = PriceEntered;
                //                mTab.setValue("PriceActual", PriceActual);
                //            }
                //        }
                //    }
                //        //end bharat
                //    else {
                //        // get uom from product
                //        var paramStr = M_Product_ID.toString();
                //        var prodC_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramStr);

                //        if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                //            sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                        + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                        + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                //                        + "  AND C_UOM_ID=" + prodC_UOM_ID;
                //        }
                //        else {
                //            sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                      + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                //                      + "  AND C_UOM_ID=" + prodC_UOM_ID;
                //        }
                //        var ds = VIS.DB.executeDataSet(sql);
                //        if (ds.getTables().length > 0) {
                //            if (ds.getTables()[0].getRows().length > 0) {
                //                //Flat Discount

                //                //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                //                 bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                //                //PriceEntered = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd"));
                //                PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                //            }
                //        }

                //        sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                //                                   " AND con.M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID")) +
                //                                   " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                //        var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                //        if (rate == 0) {
                //            sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                //                  " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                //            rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                //        }
                //        PriceEntered = PriceEntered * rate;
                //        PriceList = PriceList * rate;
                //        mTab.setValue("PriceList", PriceList);
                //        mTab.setValue("PriceEntered", PriceEntered);
                //        PriceActual = PriceEntered;
                //        mTab.setValue("PriceActual", PriceActual);
                //    }
                //}
            }
            //end Amit UOM


            //	Qty changed - recalc price
            if ((mField.getColumnName() == "QtyInvoiced"
                || mField.getColumnName() == "QtyEntered"
                || mField.getColumnName() == "M_Product_ID")
                && !"N" == ctx.getContext("DiscountSchema")) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName() == "QtyEntered") {
                    var paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                     QtyEntered.toString()); //3

                    QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                    //QtyInvoiced = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                    //    C_UOM_To_ID, QtyEntered);
                }
                if (QtyInvoiced == null)
                    QtyInvoiced = QtyEntered;
                var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

                //Added By Amit
                var date = mTab.getValue("DateInvoiced");

                countEd011 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));

                var invoiceRecord = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", (mTab.getValue("C_Invoice_ID")).toString());

                //Get PriceListversion based on Pricelist
                var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", invoiceRecord["M_PriceList_ID"].toString());

                if (orderline_ID == 0) {

                    var paramString;
                    if (date == null) {
                        paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                                      QtyInvoiced.toString(), ",", //3
                                                                      isSOTrx, ",", //4 
                                                                      M_PriceList_ID.toString(), ",", //5
                                                                      _priceListVersion_ID.toString(), ",", //6
                                                                      date, ",", null, ",",  //7
                                                                       C_UOM_To_ID.toString(), ",", countEd011);
                    }
                    else {
                        paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                                      QtyInvoiced.toString(), ",", //3
                                                                      isSOTrx, ",", //4 
                                                                      M_PriceList_ID.toString(), ",", //5
                                                                      _priceListVersion_ID.toString(), ",", //6
                                                                      date.toString(), ",", null, ",",  //7
                                                                       C_UOM_To_ID.toString(), ",", countEd011);
                    }

                    //Get product price information
                    var dr = null;
                    dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);

                    if (countEd011 <= 0) {
                        //make parameter string
                        var paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                               PriceActual.toString()); //3                        

                        PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                    }
                    else {
                        paramString = M_Product_ID.toString().concat(",", (mTab.getValue("C_Invoice_ID")).toString() +
                             "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                             "," + C_UOM_To_ID.toString() + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID.toString() +
                             "," + QtyEntered.toString());
                        var prices = VIS.dataContext.getJSONRecord("MInvoice/GetPrices", paramString);

                        if (prices != null) {
                            PriceList = prices["PriceList"];
                            mTab.setValue("PriceList", PriceList);
                            PriceEntered = prices["PriceEntered"];
                            mTab.setValue("PriceLimit", prices["PriceLimit"]);
                        }

                        //Start Amit Uom Conversion
                        //paramString = Util.getValueOfInt(mTab.getValue("C_Invoice_ID")).toString();
                        //var invoiceRecord = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);

                        ////Get PriceListversion based on Pricelist
                        //var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", invoiceRecord["M_PriceList_ID"].toString());

                        //var C_BPartner_ID1 = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
                        //var bpartner1 = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID1.toString());

                        //if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                        //    sql = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                     + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                     + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                        //                     + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                        //}
                        //else {
                        //    sql = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                   + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                   + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                        //                   + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                        //}
                        //var countPrice = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                        //if (countPrice > 0) {
                        //    // Selected UOM Price Exist
                        //    if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                        //        sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                    + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                    + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                        //                    + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                        //    }
                        //    else {
                        //        sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                  + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                        //                  + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                        //    }
                        //    var ds = VIS.DB.executeDataSet(sql);
                        //    if (ds.getTables().length > 0) {
                        //        if (ds.getTables()[0].getRows().length > 0) {
                        //            // flat Discount

                        //            //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                        //            //            bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                        //            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                        //            paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                        //            bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                        //            PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                        //            //PriceEntered = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd"));
                        //            PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                        //            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                        //        }
                        //    }
                        //}
                        //    // Added by bharat - Selected Price without attribute   
                        //else if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0 && countPrice == 0) {
                        //    sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //              + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //              + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                        //              + "  AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));

                        //    var ds = VIS.DB.executeDataSet(sql);
                        //    if (ds.getTables().length > 0) {
                        //        if (ds.getTables()[0].getRows().length > 0) {
                        //            //Flat Discount

                        //            //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                        //            //bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                        //            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                        //            paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                        //            bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                        //            PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                        //            PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                        //            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                        //            mTab.setValue("PriceEntered", PriceEntered);
                        //            PriceActual = PriceEntered;
                        //            mTab.setValue("PriceActual", PriceActual);
                        //        }
                        //    }
                        //}
                        //    //end bharat
                        //else {
                        //    // get uom from product
                        //    var paramStr = M_Product_ID.toString();
                        //    var prodC_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramStr);

                        //    if (Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID")) > 0) {
                        //        sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                    + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                    + " AND  M_AttributeSetInstance_ID = " + Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"))
                        //                    + "  AND C_UOM_ID=" + prodC_UOM_ID;
                        //    }
                        //    else {
                        //        sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                        //                  + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                        //                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                        //                  + "  AND C_UOM_ID=" + prodC_UOM_ID;
                        //    }
                        //    var ds = VIS.DB.executeDataSet(sql);
                        //    if (ds.getTables().length > 0) {
                        //        if (ds.getTables()[0].getRows().length > 0) {
                        //            // Flat discount
                        //            //PriceEntered = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                        //            //          bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                        //            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                        //            paramString = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                        //            bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                        //            PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramString));

                        //            //PriceEntered = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd"));
                        //            PriceList = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList"));
                        //        }
                        //    }

                        //    sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                        //                               " AND con.M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID")) +
                        //                               " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                        //    var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        //    if (rate == 0) {
                        //        sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                        //              " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                        //        rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                        //    }
                        //    PriceEntered = PriceEntered * rate;
                        //    PriceActual = PriceEntered;
                        //    PriceList = PriceList * rate;
                        //    mTab.setValue("PriceList", PriceList);
                        //}
                        //End Amit Uom Conversion
                    }

                    if (PriceEntered == null) {
                        PriceEntered = dr.PriceStd;
                    }
                    this.log.fine("amt - QtyChanged -> PriceActual=" + dr.PriceStd
                         + ", PriceEntered=" + PriceEntered + ", Discount=" + dr.Discount);
                    if (countEd011 <= 0) {
                        PriceActual = dr.PriceStd;
                    }
                    else {
                        PriceActual = PriceEntered;
                    }
                    mTab.setValue("PriceActual", PriceActual);
                    //	mTab.setValue("Discount", pp.getDiscount());
                    mTab.setValue("PriceEntered", PriceEntered);
                }
                ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
            }
            else if (mField.getColumnName() == "PriceActual") {
                PriceActual = value;

                //commneted by Amit On behlf of ravikant 15-10-2015
                //make parameter string
                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //      PriceActual.toString()); //3

                //var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                //PriceEntered = drPC;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);


                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                //if (PriceEntered == null) {
                //    PriceEntered = PriceActual;
                //}
                //End
                PriceEntered = PriceActual;
                //
                this.log.fine("amt - PriceActual=" + PriceActual
                     + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName() == "PriceEntered") {
                PriceEntered = value;


                //make parameter string
                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //  PriceEntered.toString()); //3

                //PriceActual = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                //PriceActual = dr.retValue;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //PriceActual = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceEntered);
                PriceActual = PriceEntered;
                if (PriceActual == null) {
                    PriceActual = PriceEntered;
                }
                //
                this.log.fine("amt - PriceEntered=" + PriceEntered
                   + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //	Check PriceLimit
            var OverwritePriceLimit = false;
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = ctx.isSOTrx() && epl != null && epl == "Y";
            // change by amit 15-10-2015
            if (epl == "") {
                var paramString = Util.getValueOfInt(mTab.getValue("C_Invoice_ID")).toString();
                var C_Invoice = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);
                var sql = "SELECT EnforcePriceLimit FROM M_PriceList WHERE IsActive = 'Y' AND M_PriceList_ID = " + C_Invoice["M_PriceList_ID"];
                epl = VIS.DB.executeScalar(sql);
                enforce = (C_Invoice["IsSOTrx"] && epl != null && epl == "Y");
                sql = "SELECT OverwritePriceLimit FROM AD_Role WHERE IsActive = 'Y' AND AD_Role_ID = " + ctx.getContext("#AD_Role_ID");
                OverwritePriceLimit = Util.getValueOfBoolean(VIS.DB.executeScalar(sql));
            }
            //end
            //if (enforce && VIS.MRole.getDefault().IsOverwritePriceLimit()) {
            if (enforce && OverwritePriceLimit) {
                enforce = false;
            }
            //	Check Price Limit?
            if (enforce && Util.getValueOfDouble(PriceLimit) != 0.0
              && PriceActual.compareTo(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //   PriceActual.toString()); //3

                //PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                // PriceEntered = drPC.retValue;// (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);

                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceLimit.Value);
                PriceEntered = PriceActual;
                if (PriceEntered == null) {
                    PriceEntered = PriceLimit;
                }
                this.log.fine("amt =(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                mTab.setValue("PriceActual", PriceLimit);
                mTab.setValue("PriceEntered", PriceEntered);

                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                // ShowMessage.Info("UnderLimitPrice", null, "", ""); //Temporary
                VIS.ADialog.info("UnderLimitPrice");
                //	Repeat Discount calc
                if (Util.getValueOfInt(PriceList) != 0) {
                    Discount = (PriceList - PriceActual) / PriceList * 100.0;
                    if (Util.scale(Discount) > 2) {
                        Discount = Discount.toFixed(2);// MidpointRounding.AwayFromZero);
                    }
                }
            }

            //	Line Net Amt
            //Amit
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            // var lineNetAmt = QtyInvoiced * PriceActual;
            var lineNetAmt = QtyEntered * PriceEntered;
            //End

            if (Util.scale(lineNetAmt) > StdPrecision) {
                lineNetAmt = lineNetAmt.toFixed(StdPrecision);// MidpointRounding.AwayFromZero);
            }
            this.log.info("amt = LineNetAmt=" + lineNetAmt);
            mTab.setValue("LineNetAmt", lineNetAmt);

            //	Calculate Tax Amount for PO / SO (SI_0339)
            var isSOTrx1 = "Y" == ctx.getWindowContext(windowNo, "IsSOTrx", true);
            //if (!isSOTrx1) {
            var taxAmt = VIS.Env.ZERO;
            if (mField.getColumnName() == "TaxAmt") {
                taxAmt = mTab.getValue("TaxAmt");
            }
            else {
                var taxID = mTab.getValue("C_Tax_ID");
                if (taxID != null) {
                    var C_Tax_ID = taxID;//.intValue();
                    var IsTaxIncluded = this.IsTaxIncluded(windowNo, ctx);
                    var paramString = C_Tax_ID.toString().concat(",", lineNetAmt.toString(), ",", //2
                                                     IsTaxIncluded, ",", //3
                                                     StdPrecision.toString() //4 
                                                    ); //7          
                    var dr = null;
                    taxAmt = VIS.dataContext.getJSONRecord("MTax/CalculateTax", paramString);
                    //


                    //MTax tax = new MTax(ctx, C_Tax_ID, null);
                    // taxAmt = dr[0];



                    //MTax tax = new MTax(ctx, C_Tax_ID, null);
                    //taxAmt = tax.CalculateTax(lineNetAmt, IsTaxIncluded(windowNo), StdPrecision);
                    mTab.setValue("TaxAmt", taxAmt);
                }
            }
            //	Added by Vivek Kumar 14/12/2015
            //mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(lineNetAmt) + taxAmt));
            if (IsTaxIncluded) {
                mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(lineNetAmt)));
            }
            else {
                mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(lineNetAmt) + taxAmt));
            }
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


    /**
     * 	Is Tax Included
     *	@param windowNo window no
     *	@return tax included (default: false)
     */
    CalloutInvoice.prototype.IsTaxIncluded = function (windowNo, ctx) {
        //  

        //var ctx = Env.getContext();
        var ss = ctx.getContext("IsTaxIncluded");
        try {
            //	Not Set Yet
            if (ss.toString().length == 0) {
                var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
                if (M_PriceList_ID == 0) {
                    return false;
                }
                ss = VIS.DB.executeScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID=" + M_PriceList_ID, null, null).toString();
                if (ss == null) {
                    ss = "N";
                }
                ctx.setContext(windowNo, "IsTaxIncluded", ss);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "Y" == ss;
    };

    /**
     *	Invoice Line - Quantity.
     *		- called from C_UOM_ID, QtyEntered, QtyInvoiced
     *		- enforces qty UOM relationship
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        this.setCalloutActive(true);
        try {
            var paramStr = "";
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            this.log.log(Level.WARNING, "qty - init - M_Product_ID=" + M_Product_ID);
            var QtyInvoiced, QtyEntered, PriceActual, PriceEntered;

            //	No Product
            if (M_Product_ID == 0) {
                QtyEntered = mTab.getValue("QtyEntered");
                mTab.setValue("QtyInvoiced", QtyEntered);
            }
                //	UOM Changed - convert from Entered -> Product
                // JID_0540: System should check the Price in price list for selected Attribute set instance and UOM,If price is not there is will multiple the base UOM price with UOM conversion multipy value and set that price.
            else if (mField.getColumnName().toString().equals("C_UOM_ID") || mField.getColumnName().toString().equals("M_AttributeSetInstance_ID")) {
                var C_UOM_To_ID = Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                QtyEntered = mTab.getValue("QtyEntered");

                /*** Start Amit ***/

                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
                var bpartner = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID.toString());

                paramStr = M_Product_ID.toString().concat(",", (mTab.getValue("C_Invoice_ID")).toString() +
                  "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                  "," + C_UOM_To_ID.toString() + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID.toString() +
                  "," + QtyEntered.toString());
                var prices = VIS.dataContext.getJSONRecord("MInvoice/GetPrices", paramStr);

                if (prices != null) {
                    countEd011 = prices["countEd011"];
                    var countVAPRC = prices["countVAPRC"];

                    // No UOM and Advance Pricing Modules
                    if (countEd011 <= 0 && countVAPRC <= 0) {
                        var QtyEntered1 = null;

                        if (QtyEntered != null) {
                            paramStr = C_UOM_To_ID.toString();
                            var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);
                            QtyEntered1 = QtyEntered.toFixed(precision);
                        }

                        if (QtyEntered != QtyEntered1) {
                            this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                            QtyEntered = QtyEntered1;
                            mTab.setValue("QtyEntered", QtyEntered);
                        }

                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", QtyEntered.toString());
                        QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                        if (QtyInvoiced == null) {
                            QtyInvoiced = QtyEntered;
                        }

                        var conversion = QtyEntered != QtyInvoiced;
                        PriceActual = mTab.getValue("PriceActual");

                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", PriceActual.toString());
                        PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                        //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                        //   C_UOM_To_ID, PriceActual);
                        if (PriceEntered == null) {
                            PriceEntered = PriceActual;
                        }
                        this.log.fine("qty - UOM=" + C_UOM_To_ID
                             + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                             + " -> " + conversion
                             + " QtyInvoiced/PriceEntered=" + QtyInvoiced + "/" + PriceEntered);
                        ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                        mTab.setValue("QtyInvoiced", QtyInvoiced);
                        mTab.setValue("PriceEntered", PriceEntered);
                    }
                    else {
                        var PriceList = prices["PriceList"];
                        mTab.setValue("PriceList", PriceList);
                        PriceEntered = prices["PriceEntered"];
                        mTab.setValue("PriceLimit", prices["PriceLimit"]);
                        mTab.setValue("PriceActual", PriceEntered);
                        mTab.setValue("PriceEntered", PriceEntered);

                        //Get precision from server side
                        paramStr = C_UOM_To_ID.toString().concat(",");
                        var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                        var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                        if (QtyEntered != QtyEntered1) {
                            this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                            QtyEntered = QtyEntered1;
                            mTab.setValue("QtyEntered", QtyEntered);
                        }

                        //Conversion of Qty Ordered
                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ","
                                                                     , QtyEntered.toString());
                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                        QtyInvoiced = pc;

                        var conversion = false
                        if (QtyInvoiced != null) {
                            conversion = QtyEntered != QtyInvoiced;
                        }
                        if (QtyInvoiced == null) {
                            conversion = false;
                            QtyInvoiced = 1;
                        }
                        if (conversion) {
                            mTab.setValue("QtyInvoiced", QtyInvoiced);
                        }
                        else {
                            mTab.setValue("QtyInvoiced", (QtyInvoiced * QtyEntered1));
                        }
                    }
                }

                //countEd011 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));
                //if (countEd011 > 0) {
                //    //Get priceList From SO/PO
                //    paramString = Util.getValueOfInt(mTab.getValue("C_Invoice_ID")).toString();
                //    var invoiceRecord = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);

                //    //Get PriceListversion based on Pricelist
                //    var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", invoiceRecord["M_PriceList_ID"].toString());

                //    //standard Precision
                //    // var standardPrecision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", C_UOM_To_ID.toString());

                //    sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                        + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                        + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                //                        + "  AND C_UOM_ID=" + C_UOM_To_ID;
                //    var ds = VIS.DB.executeDataSet(sql);
                //    if (ds.getTables().length > 0) {
                //        if (ds.getTables()[0].getRows().length > 0) {
                //            // start Pick Price based on attribute and seleceted UOM
                //            //Flat Discount
                //            //var actualPrice1 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                //            //bpartner["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //            paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //            bpartner["M_DiscountSchema_ID"].toString(), ",", bpartner["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //            var actualPrice1 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));
                //            //end
                //            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //            mTab.setValue("PriceLimit", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceLimit")));
                //            mTab.setValue("PriceActual", actualPrice1);
                //            mTab.setValue("PriceEntered", actualPrice1);

                //            //Get precision from server side
                //            paramStr = C_UOM_To_ID.toString().concat(",");
                //            var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                //            var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                //            if (QtyEntered != QtyEntered1) {
                //                this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                //                    + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                //                QtyEntered = QtyEntered1;
                //                mTab.setValue("QtyEntered", QtyEntered);
                //            }

                //            //Conversion of Qty Ordered
                //            paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ","
                //                                                         , QtyEntered.toString());
                //            var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                //            QtyInvoiced = pc;

                //            var conversion = false
                //            if (QtyInvoiced != null) {
                //                conversion = QtyEntered != QtyInvoiced;
                //            }
                //            if (QtyInvoiced == null) {
                //                conversion = false;
                //                QtyInvoiced = 1;
                //            }
                //            if (conversion) {
                //                mTab.setValue("QtyInvoiced", QtyInvoiced);
                //            }
                //            else {
                //                //mTab.setValue("QtyOrdered", Decimal.Multiply(QtyOrdered, QtyEntered1));
                //                mTab.setValue("QtyInvoiced", (QtyInvoiced * QtyEntered1));
                //            }
                //            mTab.setValue("PriceActual", actualPrice1);
                //            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //        }
                //        else {
                //            paramStr = C_UOM_To_ID.toString().concat(",");
                //            var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                //            var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                //            if (QtyEntered != QtyEntered1) {
                //                this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                //                    + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                //                QtyEntered = QtyEntered1;
                //                mTab.setValue("QtyEntered", QtyEntered);
                //            }

                //            //Conversion of Qty Ordered
                //            paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ","
                //                                                         , QtyEntered.toString());
                //            var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                //            QtyInvoiced = pc;
                //            if (QtyInvoiced == null)
                //                QtyInvoiced = QtyEntered;
                //            var conversion = QtyEntered != QtyInvoiced;

                //            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
                //            //Conversion of Price Entered
                //            paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //            PriceActual.toString()); //3
                //            var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                //            PriceEntered = pc;
                //            if (PriceEntered == null)
                //                PriceEntered = PriceActual;
                //            this.log.fine("UOM=" + C_UOM_To_ID
                //                + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                //                + " -> " + conversion
                //                + " QtyInvoiced/PriceEntered=" + QtyInvoiced + "/" + PriceEntered);
                //            ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                //            mTab.setValue("QtyInvoiced", QtyInvoiced);
                //            mTab.setValue("PriceEntered", PriceEntered);

                //            paramStr = M_Product_ID.toString();
                //            var prodC_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramStr);

                //            sql = "SELECT PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                       + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                       + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID=" + prodC_UOM_ID;
                //            var pricelist = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));

                //            sql = "SELECT PriceStd FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                     + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                     + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID=" + prodC_UOM_ID;
                //            var pricestd = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));

                //            //var actualPrice3 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), pricestd,
                //            //                            bpartner["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //            paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", pricestd.toString(), ",",
                //            bpartner["M_DiscountSchema_ID"].toString(), ",", bpartner["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //            var actualPrice3 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));

                //            sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                //                       " AND con.M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID")) +
                //                       " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                //            var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                //            if (rate == 0) {
                //                sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                //                      " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + C_UOM_To_ID;
                //                rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
                //            }
                //            mTab.setValue("PriceList", pricelist * rate);
                //            mTab.setValue("PriceActual", actualPrice3 * rate);
                //            //end Conversion based on product header UOM
                //        }
                //    }

                //}
                //else {
                //    var QtyEntered1 = null;

                //    if (QtyEntered != null) {
                //        var paramString = C_UOM_To_ID.toString();
                //        var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramString);

                //        //QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.GetPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
                //        QtyEntered1 = QtyEntered.toFixed(precision);
                //    }

                //    //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                //    if (QtyEntered != QtyEntered1) {
                //        this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                //            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                //        QtyEntered = QtyEntered1;
                //        mTab.setValue("QtyEntered", QtyEntered);
                //    }

                //    paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                //                                                      ",", QtyEntered.toString());
                //    QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                //    // QtyInvoiced = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    //  C_UOM_To_ID, QtyEntered);
                //    if (QtyInvoiced == null) {
                //        QtyInvoiced = QtyEntered;
                //    }
                //    // bool conversion = QtyEntered.compareTo(QtyInvoiced) != 0;
                //    var conversion = QtyEntered != QtyInvoiced;

                //    PriceActual = mTab.getValue("PriceActual");

                //    paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                //                                                 ",", PriceActual.toString());
                //    PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                //    //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    //   C_UOM_To_ID, PriceActual);
                //    if (PriceEntered == null) {
                //        PriceEntered = PriceActual;
                //    }
                //    this.log.fine("qty - UOM=" + C_UOM_To_ID
                //         + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                //         + " -> " + conversion
                //         + " QtyInvoiced/PriceEntered=" + QtyInvoiced + "/" + PriceEntered);
                //    ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                //    mTab.setValue("QtyInvoiced", QtyInvoiced);
                //    mTab.setValue("PriceEntered", PriceEntered);
                //}
            }
                //	QtyEntered changed - calculate QtyInvoiced
            else if (mField.getColumnName().equals("QtyEntered")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyEntered = value;

                var QtyEntered1 = null;

                if (QtyEntered != null) {
                    var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", C_UOM_To_ID.toString());
                    QtyEntered1 = QtyEntered.toFixed(precision);//, MidpointRounding.AwayFromZero);
                }

                //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                         + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", QtyEntered.toString());
                QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                //QtyInvoiced = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, QtyEntered);
                if (QtyInvoiced == null) {
                    QtyInvoiced = QtyEntered;
                }

                //bool conversion = QtyEntered.compareTo(QtyInvoiced) != 0;
                var conversion = QtyEntered != QtyInvoiced;

                this.log.fine("qty - UOM=" + C_UOM_To_ID
                     + ", QtyEntered=" + QtyEntered
                     + " -> " + conversion
                     + " QtyInvoiced=" + QtyInvoiced);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyInvoiced", QtyInvoiced);
            }
                //	QtyInvoiced changed - calculate QtyEntered (should not happen)
            else if (mField.getColumnName().equals("QtyInvoiced")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                QtyInvoiced = value;

                //  var precision = MProduct.Get(ctx, M_Product_ID).GetUOMPrecision();
                var paramString = M_Product_ID.toString();
                var precision = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramString);

                var QtyInvoiced1 = null;

                if (QtyInvoiced != null) {
                    QtyInvoiced1 = QtyInvoiced.toFixed(precision);//, MidpointRounding.AwayFromZero);
                }

                //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                if (QtyInvoiced != QtyInvoiced1) {
                    this.log.fine("Corrected QtyInvoiced Scale "
                         + QtyInvoiced + "->" + QtyInvoiced1);
                    QtyInvoiced = QtyInvoiced1;
                    mTab.setValue("QtyInvoiced", QtyInvoiced);
                }
                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                              ",", QtyInvoiced.toString());
                QtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);
                //  QtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //  C_UOM_To_ID, QtyInvoiced);
                if (QtyEntered == null) {
                    QtyEntered = QtyInvoiced;
                }

                //bool conversion = QtyInvoiced.compareTo(QtyEntered) != 0;
                var conversion = QtyInvoiced != QtyEntered;


                this.log.fine("qty - UOM=" + C_UOM_To_ID
                     + ", QtyInvoiced=" + QtyInvoiced
                     + " -> " + conversion
                     + " QtyEntered=" + QtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", QtyEntered);
            }
            //

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //MessageBox.Show("CalloutInvoice--Qty");
        }
        finally {

            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Header - PriceList.
    /// (used also in Invoice)
    /// - C_Currency_ID
    /// 	- IsTaxIncluded
    /// 	Window Context:
    /// 	- EnforcePriceLimit
    /// 	- StdPrecision
    /// 	- M_PriceList_Version_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutInvoice.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var sql = "";
        var dr = null;
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var M_PriceList_ID = Util.getValueOfInt(value.toString());
            if (M_PriceList_ID == null || M_PriceList_ID == 0)
                return "";

            sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
            //	Use newest price list - may not be future

            //DataSet ds = VIS.DB..executeDataset(sql, null);
            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)

            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	Tax Included
                mTab.setValue("IsTaxIncluded", (Boolean)("Y".equals(dr.get("istaxincluded"))));
                //	Price Limit Enforce
                ctx.setContext(windowNo, "EnforcePriceLimit", dr.get("enforcepricelimit"));
                //	Currency
                var ii = Util.getValueOfInt(dr.get("c_currency_id"));
                mTab.setValue("C_Currency_ID", ii);
                var prislst = Util.getValueOfInt(dr.get("m_pricelist_version_id"));
                //	PriceList Version
                ctx.setContext(windowNo, "M_PriceList_Version_ID", prislst);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null)
                dr.close();
            this.log.log(Level.severe, sql, err);
            return err.message;
            //MessageBox.Show("Callout--PriceList");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutInvoice = CalloutInvoice;
    //**************CalloutInvoice End*********

})(VIS, jQuery);