; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //************Callout Order*************//
    //****CalloutOrder Start
    function CalloutOrder() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrder");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutOrder, VIS.CalloutEngine); //inherit calloutengine

    CalloutOrder.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** Blanket Sales Order Sub Type - BO	*/
        var DocSubTypeSO_Blanket = "BO";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";

        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";



        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";
        //var Util=VIS.Util;

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        //var C_DocType_ID = Util.getValueOfInt(value);		//	Actually C_DocTypeTarget_ID
        var C_DocType_ID = Util.getValueOfInt(value);

        if (C_DocType_ID == null || C_DocType_ID == 0) {
            return "";
        }

        this.setCalloutActive(true);
        //	Re-Create new DocNo, if there is a doc number already
        //	and the existing source used a different Sequence number

        var oldDocNo = Util.getValueOfString(mTab.getValue("DocumentNo"));

        var newDocNo = (oldDocNo == null);

        if (!newDocNo && oldDocNo.startsWith("<") && oldDocNo.endsWith(">"))
            newDocNo = true;

        var oldC_DocType_ID = Util.getValueOfInt(mTab.getValue("C_DocType_ID"));

        var sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
            + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
            + "s.AD_Sequence_ID,d.IsSOTrx, d.IsReturnTrx, d.value, d.IsBlanketTrx "              //	7..9
            + "FROM C_DocType d "
            + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
            + "WHERE C_DocType_ID=@param1";	//	#1
        var idr = null;
        //var param = new SqlParameter[1];
        var p = [];

        try {
            var AD_Sequence_ID = 0;
            //	Get old AD_SeqNo for comparison
            if (!newDocNo && Util.getValueOfInt(oldC_DocType_ID) != 0) {
                p[0] = new VIS.DB.SqlParam("@param1", oldC_DocType_ID);

                //param[0] = new SqlParameter("@param1", oldC_DocType_ID);
                idr = VIS.DB.executeReader(sql, p);
                if (idr.read()) {
                    //AD_Sequence_ID = Util.getValueOfInt(idr.tables[0].rows[0].cells["currentnextsys"]);
                    AD_Sequence_ID = Util.getValueOfInt(idr.get("currentnextsys"));
                }
                idr.close();
            }
            p[0] = new VIS.DB.SqlParam("@param1", C_DocType_ID);
            idr = VIS.DB.executeReader(sql, p);

            p.length = 0;
            p = null;

            var DocSubTypeSO = "";
            var IsSOTrx = true;
            var isReturnTrx = false;
            var DocTypeValue = "";
            if (idr.read())		//	we found document type
            {

                //	Set Context:	Document Sub Type for Sales Orders
                DocSubTypeSO = idr.get("docsubtypeso");
                DocTypeValue = idr.get("value");

                if (DocSubTypeSO == null)
                    DocSubTypeSO = "--";
                ctx.setContext(windowNo, "OrderType", DocSubTypeSO);
                ctx.setContext(windowNo, "DocTypeValue", DocTypeValue);

                if (DocSubTypeSO == 'BO') {
                    ctx.setContext(windowNo, "BlanketOrderType", DocSubTypeSO);
                    mTab.setValue("BlanketOrderType", DocSubTypeSO);
                }
                else {
                    ctx.setContext(windowNo, "BlanketOrderType", "OO");
                    mTab.setValue("BlanketOrderType", "OO");
                }

                //	No Drop Ship other than Standard
                if (!DocSubTypeSO == DocSubTypeSO_Standard)
                    mTab.setValue("IsDropShip", "N");

                //	IsSOTrx
                if ("N" == idr.get("issotrx"))
                    IsSOTrx = false;

                //IsReturnTrx
                isReturnTrx = idr.get("isreturntrx") == "Y" ? true : false;

                //	Skip these steps for RMA. These are copied from the Original Order
                if (!isReturnTrx) {
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Force);
                    else if (DocSubTypeSO == DocSubTypeSO_Prepay)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_AfterReceipt);
                    else
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Availability);

                    //	Invoice Rule
                    if ((DocSubTypeSO == DocSubTypeSO_POS)
                        || (DocSubTypeSO == DocSubTypeSO_Prepay)
                        || (DocSubTypeSO == DocSubTypeSO_OnCredit))
                        mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                    else
                        mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);

                    //	Payment Rule - POS Order
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                    else
                        mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);


                    //	Set Context:
                    ctx.setContext(windowNo, "HasCharges", Util.getValueOfString(idr.get("hascharges")));
                }
                else // Returns
                {
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Force);
                    else
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Manual);
                }

                //	DocumentNo
                if (idr.get("isdocnocontrolled") == "Y")			//	IsDocNoControlled
                {
                    if (!newDocNo && AD_Sequence_ID != Util.getValueOfInt(idr.get("ad_sequence_id")))
                        newDocNo = true;
                    if (newDocNo)  //Temporaly Commented By Sarab
                        //if (Ini.isPropertyBool(Ini.P_VIENNASYS) && Env.getCtx().getAD_Client_ID() < 1000000) 
                        if (VIS.Ini.getLocalStorage(VIS.IniConstants.P_VIENNASYS) && ctx.getAD_Client_ID() < 1000000)
                            mTab.setValue("DocumentNo", "<" + idr.tables[0].rows[0].cells["currentnextsys"] + ">");
                        else
                            mTab.setValue("DocumentNo", "<" + idr.tables[0].rows[0].cells["currentnext"] + ">");
                }
            }
            idr.close();

            // Skip remaining steps for RMA
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }
            //  When BPartner is changed, the Rules are not set if
            //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
            //  This re-reads the Rules and applies them.
            if ((DocSubTypeSO == DocSubTypeSO_POS)
                || (DocSubTypeSO == DocSubTypeSO_Prepay))    //  not for POS/PrePay
                ;
            else {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
                    + "InvoiceRule,DeliveryRule,"                       //  3..4
                    + "FreightCostRule,DeliveryViaRule, "               //  5..6
                    + "PaymentRulePO,PO_PaymentTerm_ID "
                    + "FROM C_BPartner "
                    + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
                if (C_BPartner_ID != 0) {
                    idr = VIS.DB.executeReader(sql, null);
                }

                if (idr.read()) {
                    //	PaymentRule
                    //var s = Util.getValueOfString(idr[IsSOTrx ? "PaymentRule" : "PaymentRulePO"]);
                    var s = idr.get(IsSOTrx ? "paymentrule" : "paymentrulepo");
                    if (s != null && s.length != 0) {
                        if (IsSOTrx && (s == "B") || (s == "S") || (s == "U"))	//	No Cash/Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        if (!IsSOTrx && (s == "B"))					//	No Cash for PO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    //var ii = Util.getValueOfInt(idr[IsSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"]);
                    var ii = idr.get(IsSOTrx ? "c_paymentterm_id" : "po_paymentterm_id");

                    //if (!idr.wasNull())
                    if (idr != null)
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    //	InvoiceRule
                    s = idr.get("invoicerule");
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = idr.get("deliveryrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = idr.get("freightcostrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = idr.get("deliveryviarule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
                idr.close();
            }   //  re-read customer rules
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };



    // On selecting Blanket Order line relative fields should be auto populated, on Order line tab.
    // Added on 27 July 2017. SUkhwinder.    
    CalloutOrder.prototype.BlanketOrderLine = function (ctx, windowNo, mTab, mField, value, oldValue) {
        var dr = null;
        var sql = "";
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_BlanketOrderLine = 0;
            if (value != null)
                C_BlanketOrderLine = Util.getValueOfInt(value.toString());
            if (C_BlanketOrderLine == 0)
                return "";

            this.setCalloutActive(true);

            var param = C_BlanketOrderLine.toString();
            dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", param);
            if (dr != null) {
                var M_Product_ID = Util.getValueOfInt(dr["M_Product_ID"]);
                var Qty = Util.getValueOfInt(dr["Qty"]);
                var QtyOrdered = Util.getValueOfInt(dr["QtyOrdered"]);
                var QtyReleased = Util.getValueOfInt(dr["QtyReleased"]);
                var PriceList = Util.getValueOfDouble(dr["PriceList"]);
                var PriceActual = Util.getValueOfDouble(dr["PriceActual"]);
                var C_UOM_ID = Util.getValueOfDouble(dr["C_UOM_ID"]);
                var Discount = Util.getValueOfDouble(dr["Discount"]);
                var PriceEntered = Util.getValueOfDouble(dr["PriceEntered"]);
                var M_AttributeSetInstance_ID = Util.getValueOfInt(dr["M_AttributeSetInstance_ID"]);
                var C_Tax_ID = Util.getValueOfDouble(dr["C_Tax_ID"]);



                QtyEntered = Util.getValueOfDecimal(Qty);
                //Get precision from server
                paramStr = C_UOM_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                //var QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.getPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
                var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);

                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                }

                paramStr = M_Product_ID.toString().concat(",", C_UOM_ID.toString(), ",", //2

                   QtyEntered.toString()); //3
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                QtyOrdered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, QtyEntered.Value);
                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;

                //var conversion = QtyEntered.Value.compareTo(QtyOrdered.Value) != 0;
                var conversion = QtyEntered != QtyOrdered;


                if (conversion) {
                    paramStr = M_Product_ID.toString().concat(",", C_UOM_ID.toString(), ",", QtyReleased.toString());
                    var qtyRel = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);
                    if (qtyRel != null) {
                        QtyReleased = qtyRel;
                    }
                }


                if (M_Product_ID != 0 && M_Product_ID != null) {
                    mTab.setValue("M_Product_ID", M_Product_ID);
                }

                if (Qty != 0 && Qty != null) {
                    mTab.setValue("QtyEntered", Qty);
                }

                if (PriceList != null) {
                    mTab.setValue("PriceList", PriceList);
                }

                if (PriceActual != null) {
                    mTab.setValue("PriceActual", PriceActual);
                    mTab.setValue("LineNetAmt", PriceActual * Qty);
                }

                if (PriceEntered != null) {
                    mTab.setValue("PriceEntered", PriceEntered);
                    mTab.setValue("PriceEntered", PriceEntered);
                }

                if (Discount != null) {
                    mTab.setValue("Discount", Discount);
                }

                if (C_UOM_ID != 0 && C_UOM_ID != null) {
                    mTab.setValue("C_UOM_ID", C_UOM_ID);
                }

                // JID_1368: Blanket Order Quantity on order line should be in base UOM. Right now system is setting value in transaction UOM that is leading error: Order Quantity More Than Pending Blanket Quanity
                if (QtyOrdered != 0 && QtyOrdered != null && QtyReleased != null) {
                    mTab.setValue("QtyBlanket", ((QtyOrdered + QtyReleased) - QtyReleased));
                }

                if (M_AttributeSetInstance_ID != 0 && M_AttributeSetInstance_ID != null) {
                    mTab.setValue("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
                }

                if (C_Tax_ID != 0 && C_Tax_ID != null) {
                    mTab.setValue("C_Tax_ID", C_Tax_ID);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Qty(ctx, windowNo, mTab, mField, mTab.getValue("QtyEntered"));
    };




    // On selecting Blanket Order relative fields should be auto populated.
    // Added on 26 July 2017. SUkhwinder.
    CalloutOrder.prototype.BlanketOrder = function (ctx, windowNo, mTab, mField, value, oldValue) {
        var dr = null;
        var sql = "";
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_Order_Blanket = 0;
            if (value != null)
                C_Order_Blanket = Util.getValueOfInt(value.toString());
            if (C_Order_Blanket == 0)
                return "";


            this.setCalloutActive(true);
            var isSOTrx = ctx.isSOTrx(windowNo);
            //if (isSOTrx) {
            var param = C_Order_Blanket.toString();
            dr = VIS.dataContext.getJSONRecord("MOrder/GetOrder", param);
            if (dr != null) {
                var C_BPartner_ID = Util.getValueOfDouble(dr["C_BPartner_ID"]);
                var M_PriceList_ID = Util.getValueOfDouble(dr["M_PriceList_ID"]);
                var M_Warehouse_ID = Util.getValueOfDouble(dr["M_Warehouse_ID"]);
                var C_PaymentTerm_ID = Util.getValueOfDouble(dr["C_PaymentTerm_ID"]);
                var PaymentRule = Util.getValueOfDouble(dr["PaymentRule"]);
                var C_Payment_ID = Util.getValueOfDouble(dr["C_Payment_ID"]);
                var VA009_PaymentMethod_ID = Util.getValueOfDouble(dr["VA009_PaymentMethod_ID"]);
                var C_Currency_ID = Util.getValueOfDouble(dr["C_Currency_ID"]);

                var C_Project_ID = Util.getValueOfDouble(dr["C_Project_ID"]);
                var C_Campaign_ID = Util.getValueOfDouble(dr["C_Campaign_ID"]); C_ProjectRef_ID
                var C_ProjectRef_ID = Util.getValueOfDouble(dr["C_ProjectRef_ID"]);
                var SalesRep_ID = Util.getValueOfDouble(dr["SalesRep_ID"]);
                var PriorityRule = Util.getValueOfDouble(dr["PriorityRule"]);

                if (C_BPartner_ID != 0 && C_BPartner_ID != null) {
                    mTab.setValue("C_BPartner_ID", C_BPartner_ID);
                }

                if (M_PriceList_ID != 0 && M_PriceList_ID != null) {
                    mTab.setValue("M_PriceList_ID", M_PriceList_ID);
                }

                if (M_Warehouse_ID != 0 && M_Warehouse_ID != null) {
                    mTab.setValue("M_Warehouse_ID", M_Warehouse_ID);
                }

                if (C_PaymentTerm_ID != 0 && C_PaymentTerm_ID != null) {
                    mTab.setValue("C_PaymentTerm_ID", C_PaymentTerm_ID);
                }
                if (PaymentRule != 0 && PaymentRule != null) {
                    mTab.setValue("PaymentRule", PaymentRule);
                }
                if (C_Payment_ID != 0 && C_Payment_ID != null) {
                    mTab.setValue("C_Payment_ID", C_Payment_ID);
                }

                if (VA009_PaymentMethod_ID != 0 && VA009_PaymentMethod_ID != null) {
                    mTab.setValue("VA009_PaymentMethod_ID", VA009_PaymentMethod_ID);
                }

                if (C_Currency_ID != 0 && C_Currency_ID != null) {
                    mTab.setValue("C_Currency_ID", C_Currency_ID);
                }

                if (C_Project_ID != 0 && C_Project_ID != null) {
                    mTab.setValue("C_Project_ID", C_Project_ID);
                }

                if (C_Campaign_ID != 0 && C_Campaign_ID != null) {
                    mTab.setValue("C_Campaign_ID", C_Campaign_ID);
                }

                if (C_ProjectRef_ID != 0 && C_ProjectRef_ID != null) {
                    mTab.setValue("C_ProjectRef_ID", C_ProjectRef_ID);
                }

                if (SalesRep_ID != 0 && SalesRep_ID != null) {
                    mTab.setValue("SalesRep_ID", SalesRep_ID);
                }

                if (PriorityRule != 0 && PriorityRule != null) {
                    mTab.setValue("PriorityRule", PriorityRule);
                }

                // Added by Bharat on 07 Feb 2018 to set Inco Term from Order
                if (mTab.getField("C_IncoTerm_ID") != null) {
                    mTab.setValue("C_IncoTerm_ID", Util.getValueOfInt(dr["C_IncoTerm_ID"]));
                }

            }
            //}
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);

        oldValue = null;
        return this.BPartner(ctx, windowNo, mTab, mField, mTab.getValue("C_BPartner_ID"));
    };

    /// <summary>
    /// Order Header - BPartner.
    /// - M_PriceList_ID (+ Context)
    /// - C_BPartner_Location_ID
    /// - Bill_BPartner_ID/Bill_Location_ID
    /// 	- AD_User_ID
    /// 	- POReference
    /// 	- SO_Description
    /// 	- IsDiscountPrinted
    /// 	- InvoiceRule/DeliveryRule/PaymentRule/FreightCost/DeliveryViaRule
    /// 	- C_PaymentTerm_ID
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrder.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";

        /** InvoiceRule AD_Reference_ID=150 */
        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";
        var PaymentBasetype = null;
        //var Util=VIS.Util;
        //var sql = "";
        var dr = null;
        //var drl = null;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = 0;
            var isvendor = 'N';
            var isCustomer = 'N';
            if (value != null)
                C_BPartner_ID = Util.getValueOfInt(value.toString());
            if (C_BPartner_ID == 0)
                return "";

            // Skip rest of steps for RMA. These fields are copied over from the orignal order instead.
            var isReturnTrx = Util.getValueOfBoolean(mTab.getValue("IsReturnTrx"));
            if (isReturnTrx)
                return "";

            this.setCalloutActive(true);

            // Added by Bharat on 13/May/2017 to remove client side queries

            var _CountVA009 = false;
            var paramString = "VA009_";
            var isSOTrx = ctx.isSOTrx(windowNo);
            var dr = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", paramString);
            if (dr != null) {
                _CountVA009 = dr["VA009_"];
            }

            paramString = _CountVA009.toString() + "," + C_BPartner_ID;
            dr = VIS.dataContext.getJSONRecord("MBPartner/GetBPartnerOrderData", paramString);
            if (dr != null) {
                // Price List

                var PriceListPresent = Util.getValueOfInt(mTab.getValue("M_PriceList_ID")); // from BSO/BPO window
                var C_Order_Blanket = Util.getValueOfDecimal(mTab.getValue("C_Order_Blanket"));
                if (PriceListPresent > 0 && C_Order_Blanket > 0) {
                }
                else {
                    var ii = Util.getValueOfInt(isSOTrx ? dr["M_PriceList_ID"] : dr["PO_PriceList_ID"]);
                    if (ii != 0) {
                        mTab.setValue("M_PriceList_ID", ii);
                    }
                    // JID_0364: If price list not available at BP, user need to select it manually

                    //else {	//	get default PriceList
                    //    var i1 = ctx.getContextAsInt(windowNo, "#M_PriceList_ID", false);
                    //    if (i1 != 0)
                    //        mTab.setValue("M_PriceList_ID", i1);
                    //}
                }

                //	Bill-To BPartner
                mTab.setValue("Bill_BPartner_ID", C_BPartner_ID);
                var bill_Location_ID = Util.getValueOfInt(dr["Bill_Location_ID"]);
                if (bill_Location_ID == 0) {
                    var bill_Partner_ID = Util.getValueOfInt(dr["Bill_BPartner_ID"]);
                    if (bill_Partner_ID > 0)
                        mTab.setValue("Bill_BPartner_ID", bill_Partner_ID);
                }
                else
                    mTab.setValue("Bill_Location_ID", bill_Location_ID);

                // Ship-To Location
                var shipTo_ID = Util.getValueOfInt(dr["C_BPartner_Location_ID"]);
                if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.length > 0)
                        shipTo_ID = Util.getValueOfInt(loc);
                }
                if (shipTo_ID == 0)
                    mTab.setValue("C_BPartner_Location_ID", null);
                else {
                    mTab.setValue("C_BPartner_Location_ID", shipTo_ID);
                    if ("Y" == Util.getValueOfString(dr["IsShipTo"]))	//	set the same
                        mTab.setValue("Bill_Location_ID", shipTo_ID);
                }

                var _salerep_ID = Util.getValueOfInt(dr["SalesRep_ID"])
                if (_salerep_ID > 0)
                    mTab.setValue("SalesRep_ID", _salerep_ID);

                if (_CountVA009) {
                    var _PaymentMethod_ID = Util.getValueOfInt(dr["VA009_PaymentMethod_ID"]);
                    //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
                    var _PO_PaymentMethod_ID = 0;
                    var _PO_PAYMENTBASETYPE = "T";
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


                    if (_PaymentMethod_ID == 0) {
                        var C_Order_Blanket = Util.getValueOfInt(mTab.getValue("C_Order_Blanket"))
                        if (C_Order_Blanket > 0) {
                            var paramString = _CountVA009.toString() + "," + C_Order_Blanket.toString();
                            dr = VIS.dataContext.getJSONRecord("MOrder/GetPaymentMethod", paramString);
                            if (dr != null) {
                                var paymthd_id = Util.getValueOfInt(dr["VA009_PaymentMethod_ID"]);
                                if (paymthd_id > 0) {
                                    mTab.setValue("VA009_PaymentMethod_ID", paymthd_id);
                                    var PaymentBasetype = Util.getValueOfString(dr["VA009_PaymentBaseType"]);
                                    if (PaymentBasetype != "W" && PaymentBasetype != null) {
                                        // if (PaymentBasetype != null) {
                                        mTab.setValue("PaymentMethod", PaymentBasetype);
                                        if (isvendor == 'N')
                                            mTab.setValue("PaymentRule", PaymentBasetype);
                                        else
                                            mTab.setValue("PaymentRulePO", PaymentBasetype);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if (_PaymentMethod_ID == 0)
                            mTab.setValue("VA009_PaymentMethod_ID", null);
                        else {
                            mTab.setValue("VA009_PaymentMethod_ID", _PaymentMethod_ID);

                            //PaymentBasetype = Util.getValueOfString(dr["VA009_PaymentBaseType"]);
                            if (PaymentBasetype != "W" && PaymentBasetype != null) {
                                //if (PaymentBasetype != null) {
                                mTab.setValue("PaymentMethod", PaymentBasetype);
                                if (isvendor == 'N')
                                    mTab.setValue("PaymentRule", PaymentBasetype);
                                else
                                    mTab.setValue("PaymentRulePO", PaymentBasetype);
                            }
                            else {
                                mTab.setValue("PaymentMethod", "T");
                                if (isvendor == 'N')
                                    mTab.setValue("PaymentRule", "T");
                                else
                                    mTab.setValue("PaymentRulePO", "T");
                            }
                        }
                    }
                }

                var contID = Util.getValueOfInt(dr["AD_User_ID"]);
                if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.length > 0)
                        contID = Util.getValueOfInt(cont);
                }
                if (contID == 0)
                    mTab.setValue("AD_User_ID", null);
                else {
                    mTab.setValue("AD_User_ID", contID);
                    mTab.setValue("Bill_User_ID", contID);
                }

                //	CreditAvailable 
                if (isSOTrx) {
                    var CreditStatus = Util.getValueOfString(dr["CreditStatusSettingOn"]);
                    if (CreditStatus == "CH") {
                        var CreditLimit = Util.getValueOfDouble(dr["SO_CreditLimit"]);
                        if (CreditLimit != 0) {
                            var CreditAvailable = Util.getValueOfDouble(dr["CreditAvailable"]);
                            if (CreditAvailable <= 0) {
                                VIS.ADialog.info("CreditOver");
                            }
                        }
                    }
                    else {
                        var locId = Util.getValueOfInt(mTab.getValue("C_BPartner_Location_ID"));
                        var dr1 = VIS.dataContext.getJSONRecord("MBPartner/GetLocationData", locId.toString());
                        if (dr1 != null) {
                            CreditStatus = Util.getValueOfString(dr1["CreditStatusSettingOn"]);
                            if (CreditStatus == "CL") {
                                var CreditLimit = Util.getValueOfDouble(dr1["SO_CreditLimit"]);
                                if (CreditLimit != 0) {
                                    var CreditAvailable = Util.getValueOfDouble(dr1["CreditAvailable"]);
                                    if (CreditAvailable <= 0) {
                                        VIS.ADialog.info("CreditOver");
                                    }
                                }
                            }
                        }
                    }
                }

                //	PO Reference
                var s = Util.getValueOfString(dr["POReference"]);
                if (s != null && s.length != 0)
                    mTab.setValue("POReference", s);
                s = Util.getValueOfString(dr["SO_Description"]);
                if (s != null && s.trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                s = Util.getValueOfString(dr["IsDiscountPrinted"]);
                if (s != null && s.length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);
                mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Availability);
                if (_CountVA009) {
                    if (PaymentBasetype != "W" && PaymentBasetype != null) {
                        //if (PaymentBasetype != null) {
                        if (isvendor == 'N')
                            mTab.setValue("PaymentRule", PaymentBasetype);
                        else
                            mTab.setValue("PaymentRulePO", PaymentBasetype);
                    }
                    else {
                        if (isvendor == 'N')
                            mTab.setValue("PaymentRule", "T");
                        else
                            mTab.setValue("PaymentRulePO", "T");
                        mTab.setValue("PaymentMethod", "T");
                    }
                }
                else
                    mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);

                if (OrderType == DocSubTypeSO_Prepay) {
                    mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                    mTab.setValue("DeliveryRule", XC_DELIVERYRULE_AfterReceipt);
                }
                else if (OrderType == DocSubTypeSO_POS)	//  for POS
                {
                    if (_CountVA009) {
                        if (PaymentBasetype != "W" && PaymentBasetype != null) {
                            //if (PaymentBasetype != null) {
                            if (isvendor == 'N')
                                mTab.setValue("PaymentRule", PaymentBasetype);
                            else
                                mTab.setValue("PaymentRulePO", PaymentBasetype);
                        }
                        else {
                            if (isvendor == 'N')
                                mTab.setValue("PaymentRule", "T");
                            else
                                mTab.setValue("PaymentRulePO", "T");
                            mTab.setValue("PaymentMethod", "T");
                        }
                    }
                    else
                        mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                }
                else {
                    if (_CountVA009) {
                        if (PaymentBasetype != "W" && PaymentBasetype != null) {
                            //if (PaymentBasetype != null) {
                            if (isvendor == 'N')
                                mTab.setValue("PaymentRule", PaymentBasetype);
                            else
                                mTab.setValue("PaymentRulePO", PaymentBasetype);
                        }
                        else {
                            if (isvendor == 'N')
                                mTab.setValue("PaymentRule", "T");
                            else
                                mTab.setValue("PaymentRulePO", "T");
                            mTab.setValue("PaymentMethod", "T");
                        }
                    }
                    else {
                        //	PaymentRule
                        s = Util.getValueOfString(isSOTrx ? dr["PaymentRule"] : dr["PaymentRulePO"]);
                        if (s != null && s.length != 0) {
                            if (s == "B")				//	No Cache in Non PO
                                s = "P";
                            if (isSOTrx && (s == "S") || (s == "U"))	//	No Check/Transfer for SO_Trx
                                s = "P";										//  Payment Term
                            mTab.setValue("PaymentRule", s);
                        }
                    }
                    //	Payment Term

                    var PaymentTermPresent = Util.getValueOfInt(mTab.getValue("C_PaymentTerm_ID")); // from BSO/BPO window
                    var C_Order_Blanket = Util.getValueOfDecimal(mTab.getValue("C_Order_Blanket"));
                    if (PaymentTermPresent > 0 && C_Order_Blanket > 0) {
                    }
                    else {
                        ii = Util.getValueOfInt(isSOTrx ? dr["C_PaymentTerm_ID"] : dr["PO_PaymentTerm_ID"]);
                        //when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- not to update
                        // false means - not to update
                        var isPaymentTermUpdate = this.checkAdvancePaymentTerm(Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")), ii);
                        if (isPaymentTermUpdate) {
                            if (ii != 0)//ii=0 when dr return ""
                            {
                                mTab.setValue("C_PaymentTerm_ID", ii);
                            }
                        }
                        else {
                            mTab.setValue("C_PaymentTerm_ID", null);
                        }
                    }
                    //	InvoiceRule
                    s = Util.getValueOfString(dr["InvoiceRule"]);
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = Util.getValueOfString(dr["DeliveryRule"]);
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = Util.getValueOfString(dr["FreightCostRule"]);
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = Util.getValueOfString(dr["DeliveryViaRule"]);
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }

            //sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID, "
            //    + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
            //    + " p.SO_Description, p.salesrep_id, ";
            //var _CountVA009 = Util.getValueOfInt(CalloutDB.executeCalloutScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            //if (_CountVA009 > 0) {
            //    sql += " p.VA009_PaymentMethod_ID, ";
            //}

            //sql += " p.IsDiscountPrinted,"
            //+ " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
            //+ " p.CreditStatusSettingOn,p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            //+ " lship.C_BPartner_Location_ID,c.AD_User_ID,"
            //+ " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
            //+ " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
            //+ " FROM C_BPartner p"
            //+ " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
            //+ " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
            //+ " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
            //+ " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            //+ "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            //var isSOTrx = ctx.isSOTrx(windowNo);

            //dr = CalloutDB.executeCalloutReader(sql);
            //if (dr.read()) {
            //// Price List
            //var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
            //if (dr != null && ii != 0) {
            //    mTab.setValue("M_PriceList_ID", ii);
            //}
            //else {	//	get default PriceList                    
            //    var i1 = ctx.getContextAsInt(windowNo, "#M_PriceList_ID", false);
            //    if (i1 != 0)
            //        mTab.setValue("M_PriceList_ID", i1);
            //}

            ////	Bill-To BPartner               
            //mTab.setValue("Bill_BPartner_ID", C_BPartner_ID);                
            //var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
            //if (bill_Location_ID == 0) {
            //    sql = "SELECT C_BPartnerRelation_ID FROM C_BP_Relation WHERE C_BPartner_ID = " + C_BPartner_ID;
            //    var bill_Partner_ID = Util.getValueOfInt(CalloutDB.executeCalloutScalar(sql));
            //    if (bill_Partner_ID > 0)
            //        mTab.setValue("Bill_BPartner_ID", bill_Partner_ID);
            //}
            //else
            //    mTab.setValue("Bill_Location_ID", bill_Location_ID);

            //// Ship-To Location               
            //var shipTo_ID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));                
            //if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
            //    var loc = ctx.getContext("C_BPartner_Location_ID");
            //    if (loc.length > 0)
            //        shipTo_ID = Util.getValueOfInt(loc);
            //}
            //if (shipTo_ID == 0)
            //    mTab.setValue("C_BPartner_Location_ID", null);
            //else {
            //    mTab.setValue("C_BPartner_Location_ID", shipTo_ID);
            //    if ("Y" == dr.get("isshipto"))	//	set the same
            //        mTab.setValue("Bill_Location_ID", shipTo_ID);
            //}

            //// Code Added by Anuj------29-12-2015

            //var _salerep_ID = Util.getValueOfInt(dr.get("SalesRep_ID"))
            ////Modified by Arpit on 31st March,2017 
            ////Description - To set Sales Rep ID if found on Business Partner otherwise not to set null for login user (By defualt login user ID)
            //if (_salerep_ID > 0)
            //    mTab.setValue("SalesRep_ID", _salerep_ID);
            ////Arpit

            ////payment method ID set to Column----Anuj- 04-09-2015-------------------------------------------------------------------------------------------

            //var _CountVA009 = Util.getValueOfInt(CalloutDB.executeCalloutScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            //if (_CountVA009 > 0) {
            //    var _PaymentMethod_ID = Util.getValueOfInt(dr.get("VA009_PaymentMethod_ID"))
            //    if (_PaymentMethod_ID == 0)
            //        mTab.setValue("VA009_PaymentMethod_ID", null);
            //    else {
            //        mTab.setValue("VA009_PaymentMethod_ID", _PaymentMethod_ID);
            //        PaymentBasetype = CalloutDB.executeCalloutScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID=" + _PaymentMethod_ID);
            //        if (PaymentBasetype != "W" && PaymentBasetype != null) {
            //            mTab.setValue("PaymentMethod", PaymentBasetype);
            //            mTab.setValue("PaymentRule", PaymentBasetype);
            //        }
            //        else {
            //            mTab.setValue("PaymentMethod", "T");
            //            mTab.setValue("PaymentRule", "T");
            //        }
            //    }
            //}

            ////payment method ID set to Column----Anuj- 04-09-2015-----------------------------------------

            //var contID = Util.getValueOfInt(dr.get("ad_user_id"));

            //if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
            //    var cont = ctx.getContext("AD_User_ID");
            //    if (cont.length > 0)
            //        contID = Util.getValueOfInt(cont);
            //}
            //if (contID == 0)
            //    mTab.setValue("AD_User_ID", null);
            //else {
            //    mTab.setValue("AD_User_ID", contID);
            //    mTab.setValue("Bill_User_ID", contID);
            //}

            ////	CreditAvailable 
            //if (isSOTrx) {
            //    var CreditStatus = dr.getString("CreditStatusSettingOn");
            //    if (CreditStatus == "CH") {
            //        var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));                        
            //        if (CreditLimit != 0) {
            //            var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
            //            if (dr != null && CreditAvailable <= 0) {                                
            //                VIS.ADialog.info("CreditOver", null, "", "");
            //            }
            //        }
            //    }
            //    else {
            //        var locId = Util.getValueOfInt(mTab.getValue("C_BPartner_Location_ID"));
            //        sql = "SELECT p.CreditStatusSettingOn,p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable" +
            //            " FROM C_BPartner_Location p WHERE C_BPartner_Location_ID = " + locId;
            //        drl = CalloutDB.executeCalloutReader(sql);
            //        if (drl.read()) {
            //            CreditStatus = drl.getString("CreditStatusSettingOn");
            //            if (CreditStatus == "CL") {
            //                var CreditLimit = Util.getValueOfDouble(drl.get("so_creditlimit"));                               
            //                if (CreditLimit != 0) {
            //                    var CreditAvailable = Util.getValueOfDouble(drl.get("creditavailable"));
            //                    if (dr != null && CreditAvailable <= 0) {                                       
            //                        VIS.ADialog.info("CreditOver", null, "", "");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            ////	PO Reference                
            //var s = dr.get("poreference");
            //if (s != null && s.length != 0)
            //    mTab.setValue("POReference", s);                
            //s = dr.get("so_description");
            //if (s != null && s.trim().length != 0)
            //    mTab.setValue("Description", s);
            ////	IsDiscountPrinted               
            //s = dr.get("isdiscountprinted")
            //if (s != null && s.length != 0)
            //    mTab.setValue("IsDiscountPrinted", s);
            //else
            //    mTab.setValue("IsDiscountPrinted", "N");

            ////	Defaults, if not Walkin Receipt or Walkin Invoice
            //var OrderType = ctx.getContext("OrderType");
            //mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);
            //mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Availability);
            //var _CountVA009 = Util.getValueOfInt(CalloutDB.executeCalloutScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            //if (_CountVA009 > 0) {                    
            //    if (PaymentBasetype != "W" && PaymentBasetype != null) {
            //        mTab.setValue("PaymentRule", PaymentBasetype);
            //    }
            //    else {
            //        mTab.setValue("PaymentRule", "T");
            //        mTab.setValue("PaymentMethod", "T");
            //    }
            //}
            //else
            //    mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);

            //if (OrderType == DocSubTypeSO_Prepay) {
            //    mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
            //    mTab.setValue("DeliveryRule", XC_DELIVERYRULE_AfterReceipt);
            //}
            //else if (OrderType == DocSubTypeSO_POS)	//  for POS
            //{
            //    var _CountVA009 = Util.getValueOfInt(CalloutDB.executeCalloutScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            //    if (_CountVA009 > 0) {                        
            //        if (PaymentBasetype != "W" && PaymentBasetype != null) {
            //            mTab.setValue("PaymentRule", PaymentBasetype);
            //        }
            //        else {
            //            mTab.setValue("PaymentRule", "T");
            //            mTab.setValue("PaymentMethod", "T");
            //        }
            //    }
            //    else
            //        mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
            //}
            //else {
            //    var _CountVA009 = Util.getValueOfInt(CalloutDB.executeCalloutScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            //    if (_CountVA009 > 0) {                        
            //        if (PaymentBasetype != "W" && PaymentBasetype != null) {
            //            mTab.setValue("PaymentRule", PaymentBasetype);
            //        }
            //        else {
            //            mTab.setValue("PaymentRule", "T");
            //            mTab.setValue("PaymentMethod", "T");
            //        }
            //    }
            //    else {
            //        //	PaymentRule                       
            //        s = dr.get(isSOTrx ? "paymentrule" : "paymentrulepo");
            //        if (s != null && s.length != 0) {
            //            if (s == "B")				//	No Cache in Non PO
            //                s = "P";
            //            if (isSOTrx && (s == "S") || (s == "U"))	//	No Check/Transfer for SO_Trx
            //                s = "P";										//  Payment Term
            //            mTab.setValue("PaymentRule", s);
            //        }
            //    }
            //    //	Payment Term                   
            //    ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
            //    if (dr != null && ii != 0)//ii=0 when dr return ""
            //    {
            //        mTab.setValue("C_PaymentTerm_ID", ii);
            //    }
            //    //	InvoiceRule                    
            //    s = dr.get("invoicerule");
            //    if (s != null && s.length != 0)
            //        mTab.setValue("InvoiceRule", s);
            //    //	DeliveryRule                    
            //    s = dr.get("deliveryrule");
            //    if (s != null && s.length != 0)
            //        mTab.setValue("DeliveryRule", s);
            //    //	FreightCostRule                   
            //    s = dr.get("freightcostrule");
            //    if (s != null && s.length != 0)
            //        mTab.setValue("FreightCostRule", s);
            //    //	DeliveryViaRule                    
            //    s = dr.get("deliveryviarule");
            //    if (s != null && s.length != 0)
            //        mTab.setValue("DeliveryViaRule", s);
            //}
            //}
            //dr.close();
        }
        catch (err) {
            //if (dr != null) {
            //    dr.close();
            //    dr = null;
            //}
            //if (drl != null) {
            //    drl.close();
            //    drl = null;
            //}
            //this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        //ctx = windowNo = mTab = mField = value =
        oldValue = null;
        return this.BPartnerBill(ctx, windowNo, mTab, mField, mTab.getValue("Bill_BPartner_ID"));
    };

    /// <summary>
    /// Order Header - C_BPartner_Location.
    /// - C_BPartner_Location_ID
    /// - Bill_BPartner_ID/Bill_Location_ID
    /// 	- AD_User_ID
    /// 	- POReference
    /// 	- SO_Description
    /// 	- IsDiscountPrinted
    /// 	- InvoiceRule/DeliveryRule/PaymentRule/FreightCost/DeliveryViaRule
    /// 	- C_PaymentTerm_ID
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrder.prototype.BPartnerLoc = function (ctx, windowNo, mTab, mField, value, oldValue) {
        var dr = null;
        var sql = "";
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_BPartner_Location_ID = 0;
            if (value != null)
                C_BPartner_Location_ID = Util.getValueOfInt(value.toString());
            if (C_BPartner_Location_ID == 0)
                return "";

            // Skip rest of steps for RMA. These fields are copied over from the orignal order instead.
            var isReturnTrx = Util.getValueOfBoolean(mTab.getValue("IsReturnTrx"));
            if (isReturnTrx)
                return "";
            this.setCalloutActive(true);
            var isSOTrx = ctx.isSOTrx(windowNo);
            if (isSOTrx) {
                // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019 
                sql = "SELECT bp.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable" +
                            " FROM C_BPartner_Location p INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = p.C_BPartner_ID) WHERE p.C_BPartner_Location_ID = " + C_BPartner_Location_ID;
                dr = VIS.DB.executeReader(sql);
                if (dr.read()) {
                    var CreditStatus = dr.getString("CreditStatusSettingOn");
                    if (CreditStatus == "CL") {
                        var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                        //	var SOCreditStatus = dr.getString("SOCreditStatus");
                        if (CreditLimit != 0) {
                            var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                            if (dr != null && CreditAvailable <= 0) {
                                //VIS.ADialog.info("CreditLimitOver", null, "", "");
                                VIS.ADialog.info("CreditOver", null, "", "");
                            }
                        }
                    }
                }
            }
        }
        catch (err) {
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        //ctx = windowNo = mTab = mField = value =
        oldValue = null;
    };

    /// <summary>
    /// Order Header - Invoice BPartner.
    /// - M_PriceList_ID (+ Context)
    /// - Bill_Location_ID
    /// - Bill_User_ID
    /// - POReference
    /// - SO_Description
    /// - IsDiscountPrinted
    /// - InvoiceRule/PaymentRule
    /// - C_PaymentTerm_ID
    ///   *  @param ctx      
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrder.prototype.BPartnerBill = function (ctx, windowNo, mTab, mField, value, oldValue) {


        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";


        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";



        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";
        if (this.isCalloutActive())
            return "";
        if (value == null || value.toString() == "") {
            return "";
        }
        var dr = null;
        try {
            var bill_BPartner_ID = Util.getValueOfInt(value.toString());
            if (bill_BPartner_ID == null || bill_BPartner_ID == 0)
                return "";
            this.setCalloutActive(true);
            // Skip rest of steps for RMA
            var isReturnTrx = Util.getValueOfBoolean(mTab.getValue("IsReturnTrx"));
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }
            var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + "p.M_PriceList_ID,p.PaymentRule,p.POReference,"
                + "p.SO_Description,p.IsDiscountPrinted,"
                + "p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + "p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "c.AD_User_ID,"
                + "p.PO_PriceList_ID, p.PaymentRulePO, p.PO_PaymentTerm_ID,"
                + "lbill.C_BPartner_Location_ID AS Bill_Location_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + bill_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            var isSOTrx = "Y" == (ctx.getWindowContext(windowNo, "IsSOTrx", true));
            //var isSOTrx = false;
            //if("Y"==(ctx.getContext("IsSOTrx")))
            //{
            //    isSOTrx=true;
            //}


            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	PriceList (indirect: IsTaxIncluded & Currency)
                ////var PriceListPresent = Util.getValueOfInt(mTab.getValue("M_PriceList_ID")); //Price List from BSO/BPO window
                ////if (PriceListPresent > 0) {
                ////}
                ////else {
                ////    var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                ////    if (ii != 0)

                ////        mTab.setValue("M_PriceList_ID", ii);
                ////}
                //Code commented by Arpit Rai on 31st March,2017 ----Not to set defualt price list in context
                //asked by Mandeep Sir
                /*   else {	//	get default PriceList
                   var iCont = ctx.getContextAsInt(windowNo, "#M_PriceList_ID", false);
                   //var iCont = ctx.getContextAsInt("#M_PriceList_ID");//Sarab
                   if (iCont != 0)
                       mTab.setValue("M_PriceList_ID", Util.getValueOfInt(iCont));
               }*/ //Arpit
                var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                if (bill_BPartner_ID.toString() == (ctx.getContext("C_BPartner_ID"))) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.length > 0)
                        bill_Location_ID = Util.getValueOfInt(loc);
                }
                if (bill_Location_ID == 0)
                    mTab.setValue("Bill_Location_ID", null);
                else
                    mTab.setValue("Bill_Location_ID", Util.getValueOfInt(bill_Location_ID));

                //	Contact - overwritten by InfoBP selection
                var contID = Util.getValueOfInt(dr.get("ad_user_id"));
                if (bill_BPartner_ID.toString() == (ctx.getContext("C_BPartner_ID"))) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.length > 0)
                        contID = Util.getValueOfInt(cont);
                }
                if (contID == 0)
                    mTab.setValue("Bill_User_ID", null);
                else
                    mTab.setValue("Bill_User_ID", Util.getValueOfInt(contID.toString()));
                //	CreditAvailable 
                if (isSOTrx) {
                    var CreditLimit = 0;
                    if (CreditLimit != 0) {
                        var CreditAvailable = 0;
                        if (dr != null && CreditAvailable < 0) {
                            //mTab.fireDataStatusEEvent("CreditLimitOver",
                            //    DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),
                            //    false);
                            VIS.ADialog.info("CreditLimitOver", null, "", "");
                        }
                    }
                }

                //	PO Reference
                var s = dr.get("poreference");

                // Order Reference should not be set by Bill To BPartner; only by BPartner.
                /* if (s != null && s.length() != 0)
                    mTab.setValue("POReference", s);
                else
                    mTab.setValue("POReference", null);*/
                //	SO Description
                s = dr.get("so_description");
                if (s != null && s.toString().trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                s = dr.get("isdiscountprinted");
                if (s != null && s.toString().length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);
                mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);
                if (OrderType == DocSubTypeSO_Prepay) {

                    mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                }
                else if (OrderType == DocSubTypeSO_POS)	//  for POS 
                {
                    mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                }

                else {
                    //	PaymentRule
                    s = dr.get(isSOTrx ? "paymentrule" : "paymentrulepo");
                    if (s != null && s.toString().length != 0) {
                        if (s == "B")				//	No Cache in Non POS
                            s = "P";
                        if (isSOTrx && ((s == "S") || s == "U"))	//	No Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term

                    var PaymentTermPresent = Util.getValueOfInt(mTab.getValue("C_PaymentTerm_ID")); // from BSO/BPO window
                    var C_Order_Blanket = Util.getValueOfDecimal(mTab.getValue("C_Order_Blanket"));
                    if (PaymentTermPresent > 0 && C_Order_Blanket > 0) {

                    }
                    else {
                        ii = dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id");
                        //when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- not to update
                        // false means - not to update
                        var isPaymentTermUpdate = this.checkAdvancePaymentTerm(Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")), ii);
                        if (isPaymentTermUpdate) {
                            if (dr != null)
                                mTab.setValue("C_PaymentTerm_ID", ii);
                        }
                        else {
                            mTab.setValue("C_PaymentTerm_ID", null);
                        }
                    }
                    //	InvoiceRule
                    s = dr.get("invoicerule");
                    if (s != null && s.toString().length != 0)
                        mTab.setValue("InvoiceRule", s);
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
            this.log(Level.SEVERE, "bPartnerBill", err);
            return err;
        }
        this.setCalloutActive(false);
        return this.PriceList(ctx, windowNo, mTab, mField, mTab.getValue("M_PriceList_ID"));
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
    CalloutOrder.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var sql = "";
        var dr = null;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {

            var M_PriceList_ID = Util.getValueOfInt(value.toString());
            if (M_PriceList_ID == null || M_PriceList_ID == 0)
                return "";
            this.setCalloutActive(true);
            if (steps) {
                this.log.warning("init");
            }

            sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
            //	Use net price list - may not be future

            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	Tax Included
                mTab.setValue("IsTaxIncluded", "Y" == dr.get("istaxincluded").toString());
                //	Price Limit Enforce
                ctx.setContext(windowNo, "EnforcePriceLimit", dr.get("enforcepricelimit").toString());
                //	Currency
                var ii = Util.getValueOfInt(dr.get("c_currency_id"));

                var CurrencyPresent = Util.getValueOfInt(mTab.getValue("C_Currency_ID")); //Price List from BSO/BPO window
                var C_Order_Blanket = Util.getValueOfDecimal(mTab.getValue("C_Order_Blanket"))
                if (CurrencyPresent > 0 && C_Order_Blanket > 0) {
                }
                else {
                    mTab.setValue("C_Currency_ID", ii);
                }
                var prislst = Util.getValueOfInt(dr.get("m_pricelist_version_id"));
                //	PriceList Version
                ctx.setContext(windowNo, "M_PriceList_Version_ID", prislst);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log(Level.SEVERE, sql, err);
            return err;
        }
        if (steps) {
            this.log.warning("finish");
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Line - Product.
    /// - reset C_Charge_ID / M_AttributeSetInstance_ID
    /// - PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
    /// - UOM
    /// Calls Tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var sql = "";
        var _priceListVersion_ID = 0;
        var M_Product_ID = Util.getValueOfInt(value);
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";

        var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
        if (isReturnTrx)
            return "";

        this.setCalloutActive(true);
        try {
            if (steps) {
                this.log.warning("init");
            }
            //
            mTab.setValue("C_Charge_ID", null);
            //	Set Attribute
            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

            //if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID
            //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false) != 0) {
            //    mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false)));
            //}
            //else {
            mTab.setValue("M_AttributeSetInstance_ID", null);
            //}

            // Amit 26-05-2015
            var params = mTab.getValue("M_Product_ID").toString().concat(",", (mTab.getValue("C_BPartner_ID")).toString());
            var productInfo = VIS.dataContext.getJSONRecord("MOrderLine/GetProductInfo", params);

            //
            //var sql = "SELECT producttype FROM m_product where isactive = 'Y' AND M_Product_ID = " + M_Product_ID;
            //var productType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            var productType = productInfo["productType"].toString();
            if (productType == "S") {
                mTab.setValue("IsContract", true);
            }
            else {
                mTab.setValue("IsContract", false);
                mTab.setValue("NoofCycle", null);
                mTab.setValue("QtyPerCycle", null);
                mTab.setValue("StartDate", null);
                mTab.setValue("C_Contract_ID", 0); // Contract
                mTab.setValue("EndDate", null);
                mTab.setValue("C_Frequency_ID", 0);  // Billing frequncy
            }
            //
            //CalloutOrder.prototype.BPartner

            // JID_0165 -  Not updating current cost price on product selection, it will update on Before Save 
            //var params = mTab.getValue("M_Product_ID").toString();
            //var productCost = VIS.dataContext.getJSONRecord("MOrderLine/GetProductCost", params);
            //var cost = productCost["Cost"].toString();
            //mTab.setValue("CurrentCostPrice", cost);


            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            var isReturnTrx = ctx.getWindowContext(windowNo, "IsReturnTrx", true) == "Y";

            if (productInfo["IsDropShip"].toString() == "Y") {
                mTab.setValue("IsDropShip", true);
            }
            else {
                mTab.setValue("IsDropShip", false);
            }

            /***** Amit ****/

            countEd011 = Util.getValueOfInt(productInfo["countEd011"]);

            var purchasingUom = 0;
            if (countEd011 > 0) {
                purchasingUom = Util.getValueOfInt(productInfo["purchasingUom"]);

                if (purchasingUom > 0 && isSOTrx == false && isReturnTrx == false) {
                    mTab.setValue("C_UOM_ID", purchasingUom);
                }
                else {
                    mTab.setValue("C_UOM_ID", Util.getValueOfInt(productInfo["headerUom"]));
                }
            }

            /*****Amit done *******/

            /*****	Price Calculation see also qty	****/
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var Qty = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID", false);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID", false);
            var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false);

            var orderDate = mTab.getValue("DateOrdered");

            /*** Amit ***/
            var C_UOM_ID = Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
            /*** Amit Done ***/

            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                          Qty.toString(), ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID.toString(), ",", //5
                                                          M_PriceList_Version_ID.toString(), ",", //6
                                                          orderDate.toString(), ",",         //7
                                                          null, ",", M_AttributeSetInstance_ID.toString(), ",",  //8 , 9
                                                          C_UOM_ID, ",", countEd011); // 10 , 11
            try {
                //Get product price information
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);
                if (dr != null) {
                    mTab.setValue("PriceList", dr["PriceList"]);
                    mTab.setValue("PriceLimit", dr.PriceLimit);
                    mTab.setValue("PriceActual", dr.PriceActual);
                    mTab.setValue("PriceEntered", dr.PriceEntered);
                    mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
                    mTab.setValue("Discount", dr.Discount);
                    if (countEd011 <= 0) {
                        mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
                    }
                    mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));

                    //Start UOM
                    if (countEd011 > 0 && purchasingUom > 0 && isSOTrx == false && isReturnTrx == false) {

                        var params = mTab.getValue("M_Product_ID").toString().concat("," + ctx.getAD_Client_ID().toString() + "," + (mTab.getValue("C_Order_ID")).toString() +
                                     "," + (C_BPartner_ID).toString() + "," + (mTab.getValue("QtyEntered")).toString() + "," + Util.getValueOfString(purchasingUom));
                        var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPricesOnProductChange", params);

                        PriceList = Util.getValueOfDecimal(prices["PriceList"]);
                        mTab.setValue("PriceList", PriceList);
                        PriceEntered = Util.getValueOfDecimal(prices["PriceEntered"]);
                        mTab.setValue("PriceEntered", PriceEntered);
                        PriceActual = Util.getValueOfDecimal(prices["PriceEntered"]);
                        mTab.setValue("PriceActual", PriceActual);
                        mTab.setValue("QtyOrdered", Util.getValueOfDecimal(prices["QtyOrdered"]));
                    }
                    //End UOM

                    ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
                    ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");

                    //	Check/Update Warehouse Setting
                    //	var M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
                    //	var wh = (int)mTab.getValue("M_Warehouse_ID");
                    //	if (wh.intValue() != M_Warehouse_ID)
                    //	{
                    //		mTab.setValue("M_Warehouse_ID", new int(M_Warehouse_ID));
                    //		ADialog.warn(,windowNo, "WarehouseChanged");
                    //	}

                    if (ctx.isSOTrx()) {

                        //MProduct product = MProduct.get(ctx, M_Product_ID);
                        //Check product Stock
                        //var dr = null;
                        // dr = VIS.dataContext.getJSONRecord("CalloutOrder/GetProductStock", M_Product_ID.toString());


                        //if (dr.IsStocked) {
                        var QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                        var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                        var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
                        var C_OrderLine_ID = 0;

                        if (mTab.getValue("C_OrderLine_ID") != null) {
                            C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                        }

                        if (C_OrderLine_ID == null)
                            C_OrderLine_ID = 0;

                        //Get Qty information from server side
                        var paramString = M_Product_ID.toString().concat(",", M_Warehouse_ID.toString(), ",", //2
                                                               M_AttributeSetInstance_ID.toString(), ",", //3
                                                               C_OrderLine_ID.toString()); //4

                        var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);

                        if (available == null)
                            available = VIS.Env.ZERO;
                        if (available == 0) {
                            //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                            // VIS.ADialog.info("NoQtyAvailable", true, "0", "");
                        }
                        else if (available.toString().compareTo(QtyOrdered) < 0) {
                            //mTab.fireDataStatusEEvent("InsufficientQtyAvailable", available.toString(), false);
                            // VIS.ADialog.info("InsufficientQtyAvailable", true, available.toString(), "");
                        }
                        else {

                            var paramString = M_Warehouse_ID.toString() + "," + M_Product_ID.toString() + "," + M_AttributeSetInstance_ID.toString() + "," + C_OrderLine_ID.toString();
                            var notReserved = VIS.dataContext.getJSONRecord("MOrderLine/GetNotReserved", paramString);

                            if (notReserved == null)
                                notReserved = VIS.Env.ZERO;

                            var total = available - notReserved;
                            if (total.compareTo(QtyOrdered) < 0) {
                                var info = VIS.Msg.parseTranslation(ctx, "@QtyAvailable@=" + available
                                    + " - @QtyNotReserved@=" + notReserved + " = " + total);
                                VIS.ADialog.info("InsufficientQtyAvailable", true, info, "");
                            }
                        }
                    }
                }

                if (steps) {
                    this.log.warning("fini");
                }
            }
            catch (err) {
                this.log.saveError("calloutorder", err.toString());
                this.setCalloutActive(false);
                return err.message;
            }
            this.setCalloutActive(false);
            //return Tax(ctx, windowNo, mTab, mField, value);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError("calloutorder", err.toString());
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);

    };




    /// <summary>
    /// Order Line - Charge.
    /// - updates PriceActual from Charge
    /// - sets PriceLimit, PriceList to zero
    /// 	Calles tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value"> New Value</param>
    /// <returns>null or error message</returns>


    CalloutOrder.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {

        try {
            if (this.isCalloutActive() || value == null || value.toString() == "") {
                return "";
            }
            this.setCalloutActive(true);
            var C_Charge_ID = Util.getValueOfInt(value);
            if (C_Charge_ID == null || C_Charge_ID == 0) {
                this.setCalloutActive(false);
                return "";
            }

            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }

            //	No Product defined
            if (mTab.getValue("M_Product_ID") != null) {

                mTab.setValue("M_Product_ID", null);
                //mTab.setValue("C_Charge_ID", null);
                //this.setCalloutActive(false);
                //return "ChargeExclusively";
            }
            mTab.setValue("M_AttributeSetInstance_ID", null);
            mTab.setValue("S_ResourceAssignment_ID", null);

            //JID_0054: Currently System setting the Each as UOM after selecting the cahrge. Need to set default UOM for charge.
            var c_uom_id = ctx.getContextAsInt("#C_UOM_ID");
            if (c_uom_id > 0) {
                mTab.setValue("C_UOM_ID", c_uom_id);	//	Default charge from context
            }
            else {
                mTab.setValue("C_UOM_ID", 100);	//	EA
            }
            ctx.setContext(windowNo, "DiscountSchema", "N");
            var sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;
            var dr = null;

            dr = VIS.DB.executeReader(sql);
            var PriceEntered;
            if (dr != null) {
                if (dr.read()) {
                    // DataRow dr = ds.Tables[0].Rows[i];
                    PriceEntered = Util.getValueOfDecimal(dr.get("chargeamt"));
                    //mTab.setValue("PriceEntered", Util.getValueOfDecimal(dr[0]));
                    //mTab.setValue("PriceActual", Util.getValueOfDecimal(dr[0]));
                    //mTab.setValue("PriceLimit", VIS.Env.ZERO);
                    //mTab.setValue("PriceList", VIS.Env.ZERO);
                    //mTab.setValue("Discount", VIS.Env.ZERO);
                }
            }
            mTab.setValue("PriceEntered", PriceEntered);
            //mTab.SetValue("PriceEntered", Utility.Util.GetValueOfDecimal(dr[0]));
            //mTab.SetValue("PriceActual", Utility.Util.GetValueOfDecimal(dr[0]));
            mTab.setValue("PriceActual", PriceEntered);
            mTab.setValue("PriceLimit", VIS.Env.ZERO);
            mTab.setValue("PriceList", VIS.Env.ZERO);
            mTab.setValue("Discount", VIS.Env.ZERO);
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            return err
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);

    };

    /// <summary>
    /// Order Line - Tax.
    /// - basis: Product, Charge, BPartner Location
    /// 	- sets C_Tax_ID
    /// 	Calles Amount
    /// 	@param ctx 
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>ull or error message</returns>
    CalloutOrder.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        ///*Created By Sunil 10/11/2016*/
        //if (Util.getValueOfInt(mTab.getValue("C_Charge_ID")) > 0 && Util.getValueOfInt(mTab.getValue("M_Product_ID")) == 0) {
        //    return "";
        //}
        console.log("Before Charge Or Product");
        if (Util.getValueOfInt(mTab.getValue("C_Charge_ID")) == 0 && Util.getValueOfInt(mTab.getValue("M_Product_ID")) == 0) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            /**** Start Amit For Tax Type Module ****/
            var taxRule = "";
            var sql = "";
            var paramString = "";

            //change by amit 7-june-2016
            var params = Util.getValueOfString(mTab.getValue("M_Product_ID")).concat(",", (mTab.getValue("C_Order_ID")).toString() +
                "," + Util.getValueOfString(mTab.getValue("C_Charge_ID")));
            var recDic = VIS.dataContext.getJSONRecord("MOrderLine/GetTaxId", params);
            //end 

            //var _CountVATAX = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX IN ('VATAX_' )"));
            //var _CountVATAX = Util.getValueOfInt(recDic["_CountVATAX"]);

            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            var isReturnTrx = ctx.getWindowContext(windowNo, "IsReturnTrx", true) == "Y";

            //paramString = mTab.getValue("C_Order_ID").toString();
            //var order = VIS.dataContext.getJSONRecord("MOrder/GetOrder", paramString);

            //sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + Util.getValueOfInt(order["AD_Org_ID"]) + " AND IsActive ='Y' AND AD_Client_ID =" + ctx.getAD_Client_ID();
            if (recDic["_CountVATAX"]) {
                //taxRule = Util.getValueOfString(VIS.DB.executeScalar(sql));
                taxRule = Util.getValueOfString(recDic["taxRule"]);
            }
            if (taxRule == "T" && ((isSOTrx == true && isReturnTrx == false)
                                   || (isSOTrx == false && isReturnTrx == false))) {
                var taxId = Util.getValueOfInt(recDic["taxId"]);

                //sql = "SELECT Count(*) FROM AD_Column WHERE ColumnName = 'C_Tax_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'C_TaxCategory')";
                //if (Util.getValueOfInt(VIS.DB.executeScalar(sql)) > 0) {
                //    paramString = Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString() + "," + Util.getValueOfInt(mTab.getValue("M_Product_ID")).toString() +
                //        "," + Util.getValueOfInt(mTab.getValue("C_Charge_ID")).toString();
                //    taxId = VIS.dataContext.getJSONRecord("MOrderLine/GetTax", paramString);
                //}
                //else {
                //    sql = "SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + Util.getValueOfInt(order["C_BPartner_ID"]) +
                //                      " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + Util.getValueOfInt(order["Bill_Location_ID"]);
                //    var taxType = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //    if (taxType == 0) {
                //        sql = "SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + Util.getValueOfInt(order["C_BPartner_ID"]) + " AND IsActive = 'Y'";
                //        taxType = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //    }
                //    var prodtaxCategory = VIS.dataContext.getJSONRecord("MProduct/GetTaxCategory", value.toString());
                //    sql = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = " + prodtaxCategory + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                //    taxId = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                //}

                //change by amit 7-june-2016
                //var params = mTab.getValue("M_Product_ID").toString().concat(",", (mTab.getValue("C_Order_ID")).toString() + "," + Util.getValueOfString(mTab.getValue("C_Charge_ID")) +
                //    "," + Util.getValueOfString(order["C_BPartner_ID"]) + "," + Util.getValueOfString(order["Bill_Location_ID"]));
                //taxId = VIS.dataContext.getJSONRecord("MOrderLine/GetTaxId", params);
                //end 

                if (Util.getValueOfInt(taxId) > 0) {
                    mTab.setValue("C_Tax_ID", taxId);
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
                var column = mField.getColumnName();
                if (value == null)
                    return "";
                if (steps) {
                    this.log.warning("init");
                }
                //	Check Product
                var M_Product_ID = 0;
                if (column == "M_Product_ID") {
                    M_Product_ID = Util.getValueOfInt(value);
                }
                else {
                    M_Product_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_Product_ID"));
                }
                var C_Charge_ID = 0;
                if (column == "C_Charge_ID") {
                    C_Charge_ID = Util.getValueOfInt(value);
                }
                else {
                    C_Charge_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_Charge_ID"));
                }
                this.log.fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
                if (M_Product_ID == 0 && C_Charge_ID == 0) {
                    this.setCalloutActive(false);
                    return this.Amt(ctx, windowNo, mTab, mField, value);		//
                }
                //	Check Partner Location
                var shipC_BPartner_Location_ID = 0;
                if (column == "C_BPartner_Location_ID") {
                    shipC_BPartner_Location_ID = Util.getValueOfInt(value);
                }
                else {
                    shipC_BPartner_Location_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_BPartner_Location_ID"));
                }
                if (shipC_BPartner_Location_ID == 0) {
                    this.setCalloutActive(false);
                    return this.Amt(ctx, windowNo, mTab, mField, value);		// 
                }
                this.log.fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
                //DateTime billDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateOrdered"));
                var billDate = ctx.getContext("DateOrdered");
                this.log.fine("Bill Date=" + billDate);
                //DateTime shipDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DatePromised"));
                var shipDate = ctx.getContext("DatePromised");
                this.log.fine("Ship Date=" + shipDate);
                var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
                this.log.fine("Org=" + AD_Org_ID);
                var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                this.log.fine("Warehouse=" + M_Warehouse_ID);
                var billC_BPartner_Location_ID = ctx.getContextAsInt(windowNo, "Bill_Location_ID");
                if (billC_BPartner_Location_ID == 0)
                    billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
                this.log.fine("Bill BP_Location=" + billC_BPartner_Location_ID);
                //var C_Tax_ID = VAdvantage.Model.Tax.get(ctx, M_Product_ID, C_Charge_ID, billDate, shipDate,
                //    AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
                //    "Y" == (ctx.getContext("IsSOTrx")));
                var isSotTrx = "Y" == ctx.getWindowContext(windowNo, "IsSOTrx", true);
                var paramString = M_Product_ID.toString() + "," + C_Charge_ID.toString() + "," + billDate.toString() + "," +
                                   shipDate.toString() + "," + AD_Org_ID.toString() + "," + M_Warehouse_ID.toString() + "," + billC_BPartner_Location_ID.toString()
                                   + "," + shipC_BPartner_Location_ID.toString() + ","
                                   + isSotTrx.toString();
                var C_Tax_ID = VIS.dataContext.getJSONRecord("MTax/Get_Tax_ID", paramString);
                this.log.info("Tax ID=" + C_Tax_ID);
                //
                if (C_Tax_ID == 0) {
                    //mTab.fireDataStatusEEvent(CLogger.retrieveError());
                    // VIS.ADialog.info(VLogger.RetrieveError().toString(), true, "", "");
                }
                else {
                    mTab.setValue("C_Tax_ID", Util.getValueOfInt(C_Tax_ID));
                }
                //
                if (steps) {
                    this.log.warning("fini");
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    /// <summary>
    /// Order Line - Amount.
    /// - called from QtyOrdered, Discount and PriceActual
    /// - calculates Discount or Actual Amount
    /// - calculates LineNetAmt
    /// - enforces PriceLimit
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //////////;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        console.log("Before Charge Or Product");
        if (Util.getValueOfInt(mTab.getValue("C_Charge_ID")) == 0 && Util.getValueOfInt(mTab.getValue("M_Product_ID")) == 0) {
            return "";
        }
        console.log("After Charge Or Product");
        this.setCalloutActive(true);
        try {

            if (steps) {
                this.log.Warning("init");
            }

            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");

            var isBlanketOrderLine = Util.getValueOfInt(mTab.getValue("C_OrderLine_Blanket_ID")) > 0 ? true : false;
            var isQuotationOrderLine = Util.getValueOfInt(mTab.getValue("C_Quotation_Line_ID")) > 0 ? true : false;
            // if order line having sales quotation ID then set isBlanketOrderLine AS TRUE
            if (isQuotationOrderLine) { isBlanketOrderLine = isQuotationOrderLine }

            // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
            var isReactivation = Util.getValueOfDecimal(mTab.getValue("QtyDelivered")) > 0 ? true : false;
            if (!isReactivation) {
                isReactivation = Util.getValueOfDecimal(mTab.getValue("QtyInvoiced")) > 0 ? true : false;
            }

            //Get Qty information from server side
            var pStr = M_PriceList_ID.toString(); //1

            //Get product price information
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", M_PriceList_ID.toString());


            var StdPrecision = Util.getValueOfInt(dr["StdPrecision"]);
            var QtyEntered, QtyOrdered, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            this.log.fine("QtyEntered=" + QtyEntered + ", Ordered=" + QtyOrdered + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

            Discount = Util.getValueOfDecimal(mTab.getValue("Discount"));
            PriceLimit = Util.getValueOfDecimal(mTab.getValue("PriceLimit"));
            PriceList = Util.getValueOfDecimal(mTab.getValue("PriceList"));
            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //countEd011 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));

            var C_BPartner_ID1 = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var params = M_Product_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() +
                  "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                  "," + Util.getValueOfString(mTab.getValue("C_UOM_ID")) + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID1.toString() +
                  "," + (mTab.getValue("QtyEntered")).toString());
            var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPrices", params);

            countEd011 = Util.getValueOfInt(prices["countEd011"]);
            var countVAPRC = Util.getValueOfInt(prices["countVAPRC"]);

            // start vikas dec/8/2014
            //if (Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO  WHERE PREFIX IN('VAPRC_','ED011_')")) >= 1) {
            if (countEd011 >= 1 || countVAPRC >= 1) {
                PriceActual = PriceEntered;
            }
            // end vikas dec/8/2014

            //Start Amit UOM
            if (mField.getColumnName() == "QtyEntered" && countEd011 > 0) {

                // Added by Bharat on 29 August 2017 to handle the case of Return
                if (Util.getValueOfInt(mTab.getValue("Orig_OrderLine_ID")) > 0) {
                }
                else {
                    // SI_0605: not to update price when blanket order line exist
                    // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                    if (!isBlanketOrderLine && !isReactivation) {
                        PriceList = Util.getValueOfDecimal(prices["PriceList"]);
                        mTab.setValue("PriceList", Util.getValueOfDecimal(prices["PriceList"]));
                        PriceEntered = Util.getValueOfDecimal(prices["PriceEntered"]);
                        mTab.setValue("PriceEntered", Util.getValueOfDecimal(prices["PriceEntered"]));
                        PriceActual = Util.getValueOfDecimal(prices["PriceEntered"]);
                        mTab.setValue("PriceActual", PriceActual);
                    }
                }
            }
            //end Amit UOM

            //	Qty changed - recalc price
            if ((mField.getColumnName() == "QtyOrdered"
                || mField.getColumnName() == "QtyEntered"
                || mField.getColumnName() == "M_Product_ID")
                && !("N" == (ctx.getContext("DiscountSchema")))) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName() == "QtyEntered") {
                    var paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString() + "," + QtyEntered.toString());

                    var dr = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);

                    QtyOrdered = dr;
                }

                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;
                // Added by Bharat on 29 August 2017 to handle the case of Return
                if (Util.getValueOfInt(mTab.getValue("Orig_OrderLine_ID")) > 0) {
                }
                else {
                    var isSOTrx = (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y");
                    var orderDate = mTab.getValue("DateOrdered");
                    var paramStr = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                                      QtyOrdered.toString(), ",", //3
                                                                      isSOTrx, ",", //4 
                                                                      M_PriceList_ID.toString(), ",", //5
                                                                      "0,", //6
                                                                      orderDate.toString(), ",", null, ",", null, ",",  //7
                                                                       C_UOM_To_ID, ",", countEd011);

                    //Get product price information
                    var pp = null;
                    pp = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramStr);
                    var stdPrice = pp.PriceStd;

                    if (countEd011 <= 0 && countVAPRC <= 0) {
                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                            stdPrice.toString()); //3

                        var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                        PriceEntered = drPC;
                    }
                    else {
                        //Start Amit Uom Conversion                        
                        var C_BPartner_ID1 = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);

                        // SI_0605: not to update price when blanket order line exist
                        // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                        if (!isBlanketOrderLine && !isReactivation) {
                            var params = M_Product_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() +
                                "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                                "," + Util.getValueOfString(mTab.getValue("C_UOM_ID")) + "," + ctx.getAD_Client_ID().toString() +
                                "," + C_BPartner_ID1.toString() +
                                "," + (mTab.getValue("QtyEntered")).toString() + "," + countEd011.toString() + "," + countVAPRC.toString());
                            var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPricesOnChange", params);

                            PriceList = Util.getValueOfDecimal(prices["PriceList"]);
                            mTab.setValue("PriceList", Util.getValueOfDecimal(prices["PriceList"]));
                            PriceEntered = Util.getValueOfDecimal(prices["PriceEntered"]);
                        }
                    }
                    if (PriceEntered == null)
                        PriceEntered = stdPrice;
                    //
                    this.log.fine("QtyChanged -> PriceActual=" + stdPrice
                        + ", PriceEntered=" + PriceEntered + ", Discount=" + pp.Discount);
                    PriceActual = pp.PriceStd;
                    //mTab.setValue("PriceActual", PriceActual);
                    // SI_0605: not to update price when blanket order line exist
                    // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                    if (!isBlanketOrderLine && !isReactivation) {
                        mTab.setValue("PriceActual", PriceEntered);
                    }
                    if (countEd011 <= 0 && countVAPRC <= 0) {
                        // SI_0605: not to update price when blanket order line exist
                        // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                        if (!isBlanketOrderLine && !isReactivation) {
                            mTab.setValue("Discount", pp.Discount);
                        }
                    }
                    else {
                        // SI_0605: not to update price when blanket order line exist
                        // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                        if (!isBlanketOrderLine && !isReactivation) {
                            PriceActual = PriceEntered;
                            mTab.setValue("Discount", ((Util.getValueOfDecimal(mTab.getValue("PriceList")) - PriceEntered) / Util.getValueOfDecimal(mTab.getValue("PriceList"))) * 100);
                        }
                    }
                    mTab.setValue("PriceEntered", PriceEntered);
                    ctx.setContext(windowNo, "DiscountSchema", pp.DiscountSchema ? "Y" : "N");
                }
            }
            else if (mField.getColumnName() == "PriceActual") {
                PriceActual = Util.getValueOfDecimal(value);

                // commneted by Amit on behalf of Ravikant 15-10-2015
                //make parameter string
                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //      PriceActual.toString()); //3

                //var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                //PriceEntered = drPC;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);



                //if (PriceEntered == null)
                PriceEntered = PriceActual;
                //end

                //
                this.log.fine("PriceActual=" + PriceActual
                    + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName() == "PriceEntered") {
                PriceEntered = Util.getValueOfDecimal(value);

                //make parameter string
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                  PriceEntered.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                PriceActual = drPC;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceEntered);

                // vikas dec/8/2014
                if (Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO  WHERE PREFIX IN('VAPRC_','ED011_')")) > 1) {
                    PriceActual = PriceEntered;
                }
                //

                if (PriceActual == null)
                    PriceActual = PriceEntered;
                //
                this.log.fine("PriceEntered=" + PriceEntered
                    + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (mField.getColumnName() == "Discount") {
                //PriceActual = Util.getValueOfDecimal(((100.0 - Decimal.ToDouble(Discount.Value))
                //    / 100.0 * Decimal.ToDouble(PriceList.Value)));
                PriceActual = Util.getValueOfDecimal((100.0 - Discount)
                    / 100.0 * PriceList);

                //if (Env.Scale(PriceActual.Value) > StdPrecision)
                if (Util.scale(PriceActual) > StdPrecision)
                    //PriceActual = Decimal.Round(PriceActual.Value, StdPrecision);//, MidpointRounding.AwayFromZero);
                    PriceActual = PriceActual.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);

                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //     PriceActual.toString()); //3

                //var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                //PriceEntered = drPC;// (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);
                PriceEntered = PriceActual;
                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                mTab.setValue("PriceActual", PriceActual);
                mTab.setValue("PriceEntered", PriceEntered);
            }
                //	calculate Discount
            else {
                if (PriceList == 0) {
                    Discount = VIS.Env.ZERO;
                }
                else {
                    //Discount = new Bigvar ((PriceList.doubleValue() - PriceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    //Discount = Util.getValueOfDecimal(((Decimal.ToDouble(PriceList.Value) - Decimal.ToDouble(PriceActual.Value)) / Decimal.ToDouble(PriceList.Value) * 100.0));
                    Discount = Util.getValueOfDecimal(((PriceList - PriceActual) / PriceList * 100.0));
                    if (isNaN(Discount)) {
                        this.setCalloutActive(false);
                        return "PriceListNotSelected";
                    }
                }
                //if (Discount.Scale() > 2)
                //   Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                Discount = Discount.toFixed(2);

                // SI_0605: not to update price when blanket order line exist
                // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                if (!isBlanketOrderLine && !isReactivation) {
                    mTab.setValue("Discount", Discount);
                }
            }
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            var OverwritePriceLimit = false;
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = (ctx.isSOTrx() && epl != null && epl == "Y");
            // change by Amit 15-10-2015
            if (epl == "") {
                var paramString = Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString();
                var C_Order = VIS.dataContext.getJSONRecord("MOrder/GetOrder", paramString);
                var sql = "SELECT EnforcePriceLimit FROM M_PriceList WHERE IsActive = 'Y' AND M_PriceList_ID = " + C_Order["M_PriceList_ID"];
                epl = VIS.DB.executeScalar(sql);
                enforce = (C_Order["IsSOTrx"] && epl != null && epl == "Y");
                sql = "SELECT OverwritePriceLimit FROM AD_Role WHERE IsActive = 'Y' AND AD_Role_ID = " + ctx.getContext("#AD_Role_ID");
                OverwritePriceLimit = Util.getValueOfBoolean(VIS.DB.executeScalar(sql));
            }
            //end
            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
            // if (enforce && (VIS.MRole.getDefault().IsOverwritePriceLimit() || isReturnTrx))
            if (enforce && (OverwritePriceLimit || isReturnTrx))
                enforce = false;

            //	Check Price Limit?
            if (enforce && PriceLimit != 0.0
              && PriceActual.compareTo(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                //paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                //    PriceLimit.toString()); //3

                //var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                //PriceEntered = drPC; //(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceLimit.Value);
                PriceEntered = PriceActual;
                if (PriceEntered == null)
                    PriceEntered = PriceLimit;
                this.log.fine("(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                // SI_0605: not to update price when blanket order line exist
                // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                if (!isBlanketOrderLine && !isReactivation) {
                    mTab.setValue("PriceActual", PriceLimit);
                    mTab.setValue("PriceEntered", PriceEntered);
                }
                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                VIS.ADialog.info("UnderLimitPrice", true, "", "");
                //	Repeat Discount calc
                if (PriceList != 0) {
                    //Discount = Util.getValueOfDecimal(((Decimal.ToDouble(PriceList.Value) - Decimal.ToDouble(PriceActual.Value)) / Decimal.ToDouble(PriceList.Value) * 100.0));
                    Discount = Util.getValueOfDecimal(((PriceList - PriceActual) / PriceList * 100.0));
                    //if (Env.scale(Discount.Value) > 2)
                    //{

                    //    Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                    //}
                    Discount = Discount.toFixed(2);
                    // SI_0605: not to update price when blanket order line exist
                    // JID_1362: when qty delivered / invoiced > 0, then priace acual and entererd not change
                    if (!isBlanketOrderLine && !isReactivation) {
                        mTab.setValue("Discount", Discount);
                    }
                }
            }

            //	Line Net Amt
            // var LineNetAmt = QtyOrdered * PriceActual;
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            var LineNetAmt = QtyEntered * PriceEntered;

            if (QtyEntered > 0 && LineNetAmt == 0) {
                //Check if it is RSO/RPO and prices available which are entered through blanket sales order.
                var BlanketOrderLineID = Util.getValueOfDecimal(mTab.getValue("C_OrderLine_Blanket_ID"));

                dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", BlanketOrderLineID.toString());
                if (dr != null) {

                    var PriceList = Util.getValueOfDouble(dr["PriceList"]);
                    var PriceActual = Util.getValueOfDouble(dr["PriceActual"]);
                    var PriceEntered = Util.getValueOfDouble(dr["PriceEntered"]);
                    var Discount = Util.getValueOfDouble(dr["Discount"]);

                    if (PriceEntered != null) {
                        mTab.setValue("PriceEntered", PriceEntered);
                        mTab.setValue("LineNetAmt", PriceEntered * QtyEntered);
                        LineNetAmt = PriceEntered * QtyEntered;
                    }

                    if (PriceList != null) {
                        mTab.setValue("PriceList", PriceList);
                    }

                    if (Discount != null) {
                        mTab.setValue("Discount", Discount);
                    }

                    if (PriceActual != null) {
                        mTab.setValue("PriceActual", PriceActual);
                    }
                }
            }

            if (Util.scale(LineNetAmt) > StdPrecision) {//LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);
            }

            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);

            //	Calculate Tax Amount for PO / SO (JID_1073)

            var taxAmt = VIS.Env.ZERO;
            if (mField.getColumnName() == "TaxAmt") {
                taxAmt = mTab.getValue("TaxAmt");
            }
            else {
                var taxID = mTab.getValue("C_Tax_ID");
                if (taxID != null) {
                    var C_Tax_ID = taxID;//.intValue();
                    var IsTaxIncluded = this.IsTaxIncluded(windowNo, ctx);
                    var paramString = C_Tax_ID.toString().concat(",", LineNetAmt.toString(), ",", //2
                                                     IsTaxIncluded, ",", //3
                                                     StdPrecision.toString() //4 
                                                    ); //7          
                    var dr = null;
                    taxAmt = VIS.dataContext.getJSONRecord("MTax/CalculateTax", paramString);
                    mTab.setValue("TaxAmt", taxAmt);
                }
            }

            if (IsTaxIncluded) {
                mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(LineNetAmt)));
            }
            else {
                mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(LineNetAmt) + taxAmt));
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
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
    CalloutOrder.prototype.IsTaxIncluded = function (windowNo, ctx) {
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

    /// <summary>
    /// Order Line - Quantity.
    /// - called from C_UOM_ID, QtyEntered, QtyOrdered
    /// - enforces qty UOM relationship
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var paramStr = ""; //user for send and parameter value to controller Action
        //var U=Util;
        if (this.isCalloutActive() || value == null || value.toString() == "")
            return "";
        console.log("Before Charge Or Product");
        if (Util.getValueOfInt(mTab.getValue("C_Charge_ID")) == 0 && Util.getValueOfInt(mTab.getValue("M_Product_ID")) == 0) {
            return "";
        }
        console.log("After Charge Or Product");
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            if (steps) {
                this.log.Warning("init - M_Product_ID=" + M_Product_ID + " - ");
            }
            var QtyOrdered = VIS.Env.ZERO;
            var QtyEntered = VIS.Env.ZERO;
            var QtyEstimation = VIS.Env.ZERO;
            var PriceActual, PriceEntered;

            // Check for RMA
            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));

            var isBlanketOrderLine = Util.getValueOfInt(mTab.getValue("C_OrderLine_Blanket_ID")) > 0 ? true : false;
            var isQuotationOrderLine = Util.getValueOfInt(mTab.getValue("C_Quotation_Line_ID")) > 0 ? true : false;
            // if order line having sales quotation ID then set isBlanketOrderLine AS TRUE
            if (isQuotationOrderLine) { isBlanketOrderLine = isQuotationOrderLine }
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var bpartner = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID.toString());
            var prodC_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", M_Product_ID.toString());
            //	No Product
            if (M_Product_ID == 0) {
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                QtyOrdered = QtyEntered;
                mTab.setValue("QtyOrdered", QtyOrdered);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName() == "C_UOM_ID") {
                var C_UOM_To_ID = Util.getValueOfInt(value);
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));

                /*** Start Amit ***/

                var params = C_BPartner_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() +
                                  "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                                  "," + Util.getValueOfString(C_UOM_To_ID) + "," + ctx.getAD_Client_ID().toString() +
                                  "," + (mTab.getValue("M_Product_ID")).toString() + "," + (mTab.getValue("QtyEntered")).toString());
                var productPrices = VIS.dataContext.getJSONRecord("MOrderLine/GetProductPriceOnUomChange", params);

                countEd011 = Util.getValueOfInt(productPrices["countEd011"]);

                if (countEd011 > 0 && Util.getValueOfInt(mTab.getValue("C_Order_ID"))) {

                    var isAttributeValue = Util.getValueOfInt(productPrices["isAttributeValue"]);

                    if (isAttributeValue == 1) {

                        var actualPrice1 = Util.getValueOfDecimal(productPrices["PriceEntered"]);
                        // SI_0605: not to update price when blanket order line exist
                        if (!isBlanketOrderLine) {
                            mTab.setValue("PriceList", Util.getValueOfDecimal(productPrices["PriceList"]));
                            mTab.setValue("PriceLimit", Util.getValueOfDecimal(productPrices["PriceLimit"]));
                            mTab.setValue("PriceActual", Util.getValueOfDecimal(productPrices["PriceEntered"]));
                            mTab.setValue("PriceEntered", Util.getValueOfDecimal(productPrices["PriceEntered"]));
                        }

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
                            mTab.setValue("QtyOrdered", QtyOrdered);
                        }
                        else {
                            //mTab.setValue("QtyOrdered", Decimal.Multiply(QtyOrdered, QtyEntered1));
                            mTab.setValue("QtyOrdered", (QtyOrdered * QtyEntered1));
                        }

                        // SI_0605: not to update price when blanket order line exist
                        if (!isBlanketOrderLine) {
                            mTab.setValue("PriceActual", actualPrice1);
                            //mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                            mTab.setValue("PriceList", Util.getValueOfDecimal(productPrices["PriceList"]));
                        }
                        // End Pick Price based on attribute and seleceted UOM
                    }
                        //else if (isAttributeValue == false) {
                    else if (isAttributeValue == 2 || isAttributeValue == 0) {
                        if (isAttributeValue == 2) {
                            var actualPrice2 = Util.getValueOfDecimal(productPrices["PriceEntered"]);
                            // SI_0605: not to update price when blanket order line exist
                            if (!isBlanketOrderLine) {
                                mTab.setValue("PriceList", Util.getValueOfDecimal(productPrices["PriceList"]));
                                mTab.setValue("PriceLimit", Util.getValueOfDecimal(productPrices["PriceLimit"]));
                                mTab.setValue("PriceActual", Util.getValueOfDecimal(productPrices["PriceEntered"]));
                                mTab.setValue("PriceEntered", Util.getValueOfDecimal(productPrices["PriceEntered"]));
                            }

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
                                mTab.setValue("QtyOrdered", QtyOrdered);
                            }
                            else {
                                mTab.setValue("QtyOrdered", (QtyOrdered * QtyEntered1));
                            }

                            // SI_0605: not to update price when blanket order line exist
                            if (!isBlanketOrderLine) {
                                mTab.setValue("PriceActual", actualPrice2);
                                //mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                                mTab.setValue("PriceList", Util.getValueOfDecimal(productPrices["PriceList"]));
                            }
                            // End Pick Price of selected UOM
                        }
                        else {
                            //start Conversion based on product header UOM
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
                            QtyOrdered = pc;
                            if (QtyOrdered == null)
                                QtyOrdered = QtyEntered;
                            var conversion = QtyEntered != QtyOrdered;

                            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
                            //Conversion of Price Entered
                            paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                            PriceActual.toString()); //3
                            var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                            PriceEntered = pc;
                            if (PriceEntered == null)
                                PriceEntered = PriceActual;
                            this.log.fine("UOM=" + C_UOM_To_ID
                                + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                                + " -> " + conversion
                                + " QtyOrdered/PriceEntered=" + QtyOrdered + "/" + PriceEntered);
                            ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                            mTab.setValue("QtyOrdered", QtyOrdered);
                            mTab.setValue("PriceEntered", PriceEntered);

                            var pricelist = 0;
                            var pricestd = 0;

                            // SI_0605: not to update price when blanket order line exist
                            if (!isBlanketOrderLine) {
                                //change by amit 6-june-2016
                                var params = M_Product_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() + "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                                    "," + Util.getValueOfString(C_UOM_To_ID) + "," + ctx.getAD_Client_ID().toString() + "," + bpartner["M_DiscountSchema_ID"].toString() +
                                    "," + (bpartner["FlatDiscount"]).toString() + "," + (mTab.getValue("QtyEntered")).toString() + "," + Util.getValueOfString(prodC_UOM_ID));
                                var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPricesOnUomChange", params);

                                PriceList = Util.getValueOfDecimal(prices["PriceList"]);
                                mTab.setValue("PriceList", Util.getValueOfDecimal(prices["PriceList"]));
                                PriceEntered = Util.getValueOfDecimal(prices["PriceEntered"]);
                                mTab.setValue("PriceEntered", Util.getValueOfDecimal(prices["PriceEntered"]));
                                PriceActual = Util.getValueOfDecimal(prices["PriceEntered"]);
                                mTab.setValue("PriceActual", PriceActual);
                            }
                            //end Conversion based on product header UOM
                        }
                        //}
                        //}
                    }
                }
                    // End
                else {
                    //Get precision from server side
                    paramStr = C_UOM_To_ID.toString().concat(","); //1

                    var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);



                    //var QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.getPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
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

                    QtyOrdered = pc;// (Decimal?) MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                    //C_UOM_To_ID, QtyEntered.Value);


                    if (QtyOrdered == null)
                        QtyOrdered = QtyEntered;
                    var conversion = QtyEntered != QtyOrdered;
                    PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

                    paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                    PriceActual.toString()); //3
                    var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                    PriceEntered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                    //C_UOM_To_ID, PriceActual.Value);
                    if (PriceEntered == null)
                        PriceEntered = PriceActual;
                    this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                        + " -> " + conversion
                        + " QtyOrdered/PriceEntered=" + QtyOrdered + "/" + PriceEntered);
                    ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                    mTab.setValue("QtyOrdered", QtyOrdered);
                    mTab.setValue("PriceEntered", PriceEntered);
                }
            }
                //	QtyEntered changed - calculate QtyOrdered
            else if (mField.getColumnName() == "QtyEntered") {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyEntered = Util.getValueOfDecimal(value);

                //Get precision from server
                paramStr = C_UOM_To_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);


                //var QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.getPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
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
                //C_UOM_To_ID, QtyEntered.Value);
                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;

                //var conversion = QtyEntered.Value.compareTo(QtyOrdered.Value) != 0;
                var conversion = QtyEntered != QtyOrdered;



                this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered=" + QtyEntered
                        + " -> " + conversion
                        + " QtyOrdered=" + QtyOrdered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                mTab.setValue("QtyOrdered", QtyOrdered);
            }
                //	QtyOrdered changed - calculate QtyEntered (should not happen)
            else if (mField.getColumnName() == "QtyOrdered") {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyOrdered = Util.getValueOfDecimal(value);

                paramStr = M_Product_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramStr);

                var precision = gp;//MProduct.get(ctx, M_Product_ID).getUOMPrecision();

                var QtyOrdered1 = QtyOrdered.toFixed(precision);

                if (QtyOrdered != QtyOrdered1) {
                    this.log.fine("Corrected QtyOrdered Scale "
                        + QtyOrdered + "->" + QtyOrdered1);
                    QtyOrdered = QtyOrdered1;
                    mTab.setValue("QtyOrdered", QtyOrdered);
                }

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                      QtyOrdered.toString()); //3

                var pt = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                QtyEntered = pt//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //C_UOM_To_ID, QtyOrdered);
                if (QtyEntered == null)
                    QtyEntered = QtyOrdered;
                var conversion = QtyOrdered != QtyEntered;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", QtyOrdered=" + QtyOrdered
                    + " -> " + conversion
                    + " QtyEntered=" + QtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", QtyEntered);
            }
            else {
                //	QtyEntered = (Decimal)mTab.getValue("QtyEntered");
                QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            }

            if (M_Product_ID != 0
               && isReturnTrx) {
                var inOutLine_ID = Util.getValueOfInt(mTab.getValue("Orig_InOutLine_ID"));
                if (inOutLine_ID != 0) {
                    paramStr = inOutLine_ID.toString();
                    var dr = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramStr);
                    var mq = dr["MovementQty"];
                    var shippedQty = Util.getValueOfDecimal(mq);

                    QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                    if (shippedQty < QtyOrdered) {
                        if (ctx.isSOTrx()) {
                            //mTab.fireDataStatusEEvent("QtyShippedLessThanQtyReturned", shippedQty.toString(), false);
                            VIS.ADialog.info("QtyShippedAndReturned", null, shippedQty.toString(), "");
                        }
                        else {
                            // mTab.fireDataStatusEEvent("QtyReceivedLessThanQtyReturned", shippedQty.toString(), false);
                            VIS.ADialog.info("QtyRecievedAndReturnd", null, shippedQty.toString(), "");
                        }
                        mTab.setValue("QtyOrdered", shippedQty);
                        QtyOrdered = shippedQty;

                        var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                        QtyOrdered.toString()); //3

                        QtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);
                        // QtyEntered = pt.retValue;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                        //C_UOM_To_ID, QtyOrdered);
                        if (QtyEntered == null)
                            QtyEntered = QtyOrdered;
                        mTab.setValue("QtyEntered", QtyEntered);
                        this.log.fine("QtyEntered : " + QtyEntered.toString() +
                                    "QtyOrdered : " + QtyOrdered.toString());
                    }
                }
            }

            //Start Amit (Attribute selection then get price according to attribute n uom)  (Purchase side only)
            if (M_Product_ID != 0
                //&& ctx.isSOTrx()
             && QtyOrdered > 0
             && !isReturnTrx		//	no negative (returns)
             && (mField.getColumnName() == "M_AttributeSetInstance_ID")) {
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                var pricelist = 0;
                var params = M_Product_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() +
                     "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                     "," + Util.getValueOfString(mTab.getValue("C_UOM_ID")) + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID.toString() +
                     "," + QtyEntered.toString());
                var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPrices", params);
                if (prices != null) {

                    countEd011 = Util.getValueOfInt(prices["countEd011"]);
                    var _countVAPRC = Util.getValueOfInt(prices["countVAPRC"]);
                    PriceEntered = prices["PriceEntered"];
                    pricelist = prices["PriceList"];

                    if (!isBlanketOrderLine) {
                        mTab.setValue("PriceList", pricelist);
                        mTab.setValue("PriceActual", PriceEntered);
                        mTab.setValue("PriceEntered", PriceEntered);
                    }

                    if (countEd011 <= 0 && _countVAPRC <= 0) {

                        //start Conversion of Price and qty Ordered
                        params = mTab.getValue("C_UOM_ID").toString().concat(",");
                        var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", params);

                        var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                        if (QtyEntered != QtyEntered1) {
                            this.log.fine("Corrected QtyEntered Scale UOM=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                            QtyEntered = QtyEntered1;
                            mTab.setValue("QtyEntered", QtyEntered);
                        }

                        //Conversion of Qty Ordered
                        params = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ","
                                                                     , QtyEntered.toString());
                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", params);
                        QtyOrdered = pc;
                        if (QtyOrdered == null)
                            QtyOrdered = QtyEntered;
                        var conversion = QtyEntered != QtyOrdered;

                        PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
                        //Conversion of Price Entered
                        paramStr = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ",", //2
                        PriceActual.toString()); //3
                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                        PriceEntered = pc;
                        if (PriceEntered == null)
                            PriceEntered = PriceActual;
                        this.log.fine("UOM=" + C_UOM_To_ID
                            + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                            + " -> " + conversion
                            + " QtyOrdered/PriceEntered=" + QtyOrdered + "/" + PriceEntered);
                        ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                        mTab.setValue("QtyOrdered", QtyOrdered);
                        mTab.setValue("PriceEntered", PriceEntered);
                    }
                    else {
                        if (!isBlanketOrderLine) {
                            params = M_Product_ID.toString().concat(",", (mTab.getValue("C_Order_ID")).toString() + "," + Util.getValueOfString(mTab.getValue("M_AttributeSetInstance_ID")) +
                                "," + Util.getValueOfString(mTab.getValue("C_UOM_ID")) + "," + ctx.getAD_Client_ID().toString() + "," + C_BPartner_ID.toString() +
                                "," + QtyEntered.toString() + "," + countEd011.toString() + "," + _countVAPRC.toString());
                            var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPricesOnChange", params);

                            pricelist = Util.getValueOfDecimal(prices["PriceList"]);
                            mTab.setValue("PriceList", pricelist);
                            PriceEntered = Util.getValueOfDecimal(prices["PriceEntered"]);
                            mTab.setValue("PriceEntered", PriceEntered);
                            PriceActual = Util.getValueOfDecimal(prices["PriceEntered"]);
                            mTab.setValue("PriceActual", PriceActual);
                        }

                        paramStr = mTab.getValue("C_UOM_ID").toString().concat(",");
                        var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                        var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                        if (QtyEntered != QtyEntered1) {
                            this.log.fine("Corrected QtyEntered Scale UOM=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                            QtyEntered = QtyEntered1;
                            mTab.setValue("QtyEntered", QtyEntered);
                        }

                        //Conversion of Qty Ordered
                        paramStr = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ","
                                                                     , QtyEntered.toString());
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
                            mTab.setValue("QtyOrdered", QtyOrdered);
                        }
                        else {
                            mTab.setValue("QtyOrdered", (QtyOrdered * QtyEntered1));
                        }
                    }
                }
                //countEd011 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));
                //var _countVAPRC = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VAPRC_'"));

                //paramString = (mTab.getValue("C_Order_ID")).toString();
                //var M_PriceList_ID = VIS.dataContext.getJSONRecord("MOrder/GetM_PriceList", paramString);

                ////Get PriceListversion based on Pricelist
                //var _priceListVersion_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", M_PriceList_ID.toString());

                //var C_BPartner_ID1 = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
                //var bpartner1 = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", C_BPartner_ID1.toString());

                //if (countEd011 > 0 && _countVAPRC > 0) {
                //    sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE IsActive='Y' AND M_Product_ID = " + M_Product_ID +
                //                      " AND C_UOM_ID =" + Util.getValueOfInt(mTab.getValue("C_UOM_ID")) +
                //                      " AND M_AttributeSetInstance_ID = " + value +
                //                      " AND M_PriceList_Version_ID = " + _priceListVersion_ID;
                //    var dsPrice = VIS.DB.executeDataSet(sql);
                //    if (dsPrice != null) {
                //        if (dsPrice.getTables().length > 0) {
                //            // Start Based On Attribute and UOM(On current Record)
                //            if (dsPrice.getTables()[0].getRows().length > 0) {
                //                // SI_0605: not to update price when blanket order line exist
                //                if (!isBlanketOrderLine) {
                //                    //Flat Discount

                //                    //var actualPrice4 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(dsPrice.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                    //                        bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                    // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                    paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", dsPrice.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                    bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                    var actualPrice4 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));

                //                    mTab.setValue("PriceList", Util.getValueOfDecimal(dsPrice.getTables()[0].getRows()[0].getCell("PriceList")));
                //                    mTab.setValue("PriceLimit", Util.getValueOfDecimal(dsPrice.getTables()[0].getRows()[0].getCell("PriceLimit")));
                //                    mTab.setValue("PriceActual", actualPrice4);
                //                    mTab.setValue("PriceEntered", actualPrice4);
                //                }
                //            }
                //                //End Based On Attribute and UOM(On current Record)
                //            else {
                //                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                //                sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.getValueOfInt(mTab.getValue("M_Product_ID"))
                //                                + " AND M_PriceList_Version_ID = " + _priceListVersion_ID
                //                                + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"));
                //                var ds = VIS.DB.executeDataSet(sql);
                //                if (ds.getTables().length > 0) {
                //                    // Start Set Price based on Only UOM, consider Attribute as 0 and conversion of qty Ordered
                //                    if (ds.getTables()[0].getRows().length > 0) {
                //                        var actualPrice5 = 0;
                //                        // SI_0605: not to update price when blanket order line exist
                //                        if (!isBlanketOrderLine) {
                //                            //flat Discount

                //                            //actualPrice5 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                            //              bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                            paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", ds.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                            bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                            actualPrice5 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));

                //                            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //                            mTab.setValue("PriceLimit", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceLimit")));
                //                            mTab.setValue("PriceActual", actualPrice5);
                //                            mTab.setValue("PriceEntered", actualPrice5);
                //                        }

                //                        paramStr = mTab.getValue("C_UOM_ID").toString().concat(",");
                //                        var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                //                        var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                //                        if (QtyEntered != QtyEntered1) {
                //                            this.log.fine("Corrected QtyEntered Scale UOM=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
                //                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                //                            QtyEntered = QtyEntered1;
                //                            mTab.setValue("QtyEntered", QtyEntered);
                //                        }

                //                        //Conversion of Qty Ordered
                //                        paramStr = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ","
                //                                                                     , QtyEntered.toString());
                //                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                //                        QtyOrdered = pc;

                //                        var conversion = false
                //                        if (QtyOrdered != null) {
                //                            conversion = QtyEntered != QtyOrdered;
                //                        }
                //                        if (QtyOrdered == null) {
                //                            conversion = false;
                //                            QtyOrdered = 1;
                //                        }
                //                        if (conversion) {
                //                            mTab.setValue("QtyOrdered", QtyOrdered);
                //                        }
                //                        else {
                //                            mTab.setValue("QtyOrdered", (QtyOrdered * QtyEntered1));
                //                        }
                //                        // SI_0605: not to update price when blanket order line exist
                //                        if (!isBlanketOrderLine) {
                //                            mTab.setValue("PriceActual", actualPrice5);
                //                            mTab.setValue("PriceList", Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("PriceList")));
                //                        }
                //                        //end Set Price based on Only UOM, consider Attribute as 0 and conversion of qty Ordered
                //                    }
                //                    else {
                //                        //start Conversion of Price and qty Ordered
                //                        paramStr = mTab.getValue("C_UOM_ID").toString().concat(",");
                //                        var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                //                        var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));

                //                        if (QtyEntered != QtyEntered1) {
                //                            this.log.fine("Corrected QtyEntered Scale UOM=" + Util.getValueOfInt(mTab.getValue("C_UOM_ID"))
                //                                + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                //                            QtyEntered = QtyEntered1;
                //                            mTab.setValue("QtyEntered", QtyEntered);
                //                        }

                //                        //Conversion of Qty Ordered
                //                        paramStr = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ","
                //                                                                     , QtyEntered.toString());
                //                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                //                        QtyOrdered = pc;
                //                        if (QtyOrdered == null)
                //                            QtyOrdered = QtyEntered;
                //                        var conversion = QtyEntered != QtyOrdered;

                //                        PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
                //                        //Conversion of Price Entered
                //                        paramStr = M_Product_ID.toString().concat(",", mTab.getValue("C_UOM_ID").toString(), ",", //2
                //                        PriceActual.toString()); //3
                //                        var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                //                        PriceEntered = pc;
                //                        if (PriceEntered == null)
                //                            PriceEntered = PriceActual;
                //                        this.log.fine("UOM=" + C_UOM_To_ID
                //                            + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                //                            + " -> " + conversion
                //                            + " QtyOrdered/PriceEntered=" + QtyOrdered + "/" + PriceEntered);
                //                        ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                //                        mTab.setValue("QtyOrdered", QtyOrdered);
                //                        mTab.setValue("PriceEntered", PriceEntered);

                //                        paramStr = M_Product_ID.toString();
                //                        var prodC_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramStr);                                       

                //                        // SI_0605: not to update price when blanket order line exist
                //                        if (!isBlanketOrderLine) {
                //                            var params = (mTab.getValue("M_Product_ID")).toString().concat(",", (mTab.getValue("C_Order_ID")).toString() + "," + Util.getValueOfString(0) +
                //                           "," + Util.getValueOfString(mTab.getValue("C_UOM_ID")) + "," + ctx.getAD_Client_ID().toString() + "," + bpartner1["M_DiscountSchema_ID"].toString() +
                //                           "," + (bpartner1["FlatDiscount"]).toString() + "," + (mTab.getValue("QtyEntered")).toString() + "," + Util.getValueOfString(prodC_UOM_ID));
                //                            var prices = VIS.dataContext.getJSONRecord("MOrderLine/GetPricesOnUomChange", params);

                //                            PriceList = Util.getValueOfDecimal(prices["PriceList"]);
                //                            mTab.setValue("PriceList", Util.getValueOfDecimal(prices["PriceList"]));
                //                            mTab.setValue("PriceEntered", Util.getValueOfDecimal(prices["PriceEntered"]));
                //                            mTab.setValue("PriceActual", Util.getValueOfDecimal(prices["PriceEntered"]));
                //                        }

                //                        //end Conversion of Price and qty Ordered
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                // start when not UOM but  Vienna Advance Pricing
                //if (countEd011 <= 0 && _countVAPRC > 0) {
                //    sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE IsActive='Y' AND M_Product_ID = " + M_Product_ID +
                //                      " AND M_AttributeSetInstance_ID =  " + value +
                //                      " AND M_PriceList_Version_ID = " + _priceListVersion_ID;
                //    var dsPrice1 = VIS.DB.executeDataSet(sql);
                //    if (dsPrice1.getTables().length > 0) {
                //        // Start Based On Attribute and UOM(On current Record)
                //        if (dsPrice1.getTables()[0].getRows().length > 0) {
                //            // SI_0605: not to update price when blanket order line exist
                //            if (!isBlanketOrderLine) {
                //                //flat Discount

                //                //var actualPrice7 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(dsPrice1.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                //                         bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", dsPrice1.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                var actualPrice7 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));

                //                mTab.setValue("PriceList", Util.getValueOfDecimal(dsPrice1.getTables()[0].getRows()[0].getCell("PriceList")));
                //                mTab.setValue("PriceLimit", Util.getValueOfDecimal(dsPrice1.getTables()[0].getRows()[0].getCell("PriceLimit")));
                //                mTab.setValue("PriceActual", actualPrice7);
                //                mTab.setValue("PriceEntered", actualPrice7);
                //            }
                //        }
                //        else {
                //            sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE IsActive='Y' AND M_Product_ID = " + M_Product_ID +
                //                              " AND M_AttributeSetInstance_ID = 0 " +
                //                              " AND M_PriceList_Version_ID = " + _priceListVersion_ID;
                //            var dsPrice = VIS.DB.executeDataSet(sql);
                //            if (dsPrice != null) {
                //                if (dsPrice.getTables().length > 0) {
                //                    if (dsPrice.getTables()[0].getRows().length > 0) {
                //                        // SI_0605: not to update price when blanket order line exist
                //                        if (!isBlanketOrderLine) {
                //                            //flat discount

                //                            //var actualPrice8 = this.FlatDiscount(M_Product_ID, ctx.getAD_Client_ID(), Util.getValueOfDecimal(dsPrice1.getTables()[0].getRows()[0].getCell("PriceStd")),
                //                            //           bpartner1["M_DiscountSchema_ID"], Util.getValueOfDecimal(bpartner1["FlatDiscount"]), Util.getValueOfInt(mTab.getValue("QtyEntered")));

                //                            // JID_0487: Calculate Discount based on Discount Schema selected on Business Partner
                //                            paramStr = M_Product_ID.toString().concat(",", ctx.getAD_Client_ID().toString(), ",", dsPrice1.getTables()[0].getRows()[0].getCell("PriceStd").toString(), ",",
                //                            bpartner1["M_DiscountSchema_ID"].toString(), ",", bpartner1["FlatDiscount"].toString(), ",", mTab.getValue("QtyEntered").toString());
                //                            var actualPrice8 = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/FlatDiscount", paramStr));

                //                            mTab.setValue("PriceList", Util.getValueOfDecimal(dsPrice.getTables()[0].getRows()[0].getCell("PriceList")));
                //                            mTab.setValue("PriceLimit", Util.getValueOfDecimal(dsPrice.getTables()[0].getRows()[0].getCell("PriceLimit")));
                //                            mTab.setValue("PriceActual", actualPrice8);
                //                            mTab.setValue("PriceEntered", actualPrice8);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //End when not UOM but  Vienna Advance Pricing
            }
            //End Amit (Attribute selection then get price according to attribute n uom)  (Purchase side only)

            //	Storage
            if (M_Product_ID != 0
                && ctx.isSOTrx()
                && QtyOrdered > 0
                && !isReturnTrx)		//	no negative (returns)
            {

                var pi = VIS.dataContext.getJSONRecord("MProduct/GetProduct", M_Product_ID.toString());
                //MProduct product = MProduct.get(ctx, M_Product_ID);
                var C_OrderLine_ID = 0;

                if (Util.getValueOfBoolean(pi.IsStocked)) {
                    var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                    var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");


                    //Decimal? available = MStorage.getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    //Get Qty information from server side
                    var paramString = M_Product_ID.toString().concat(",", M_Warehouse_ID.toString(), ",", //2
                                                           M_AttributeSetInstance_ID.toString(), ",", //3
                                                           C_OrderLine_ID.toString()); //4

                    //Get product price information
                    var dr = null;
                    var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);

                    // var available = dr.available;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI

                    if (available == null)
                        available = VIS.Env.ZERO;
                    if (available == 0) {
                        //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                        //VIS.ADiathis.log.info("NoQtyAvailable", null, "0", "");
                    }
                    else if (available.toString().compareTo(QtyOrdered) < 0) {
                        // VIS.ADialog.info("InsufficientQtyAvailable", null, available.toString(), "");
                    }
                    else {

                        if (mTab.getValue("C_OrderLine_ID") == "") {
                            C_OrderLine_ID = 0;
                        }
                        else {
                            C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                        }
                        if (C_OrderLine_ID == null)
                            C_OrderLine_ID = 0;

                        var notReserved = dr.notReserved;//MOrderLine.getNotReserved(ctx,
                        //M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                        //C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = VIS.Env.ZERO;
                        //var total = Decimal.Subtract(available.Value, notReserved);
                        var total = available - notReserved;
                        if (total < QtyOrdered) {
                            var info = VIS.Msg.parseTranslation(ctx, "@QtyAvailable@=" + available
                                + "  -  @QtyNotReserved@=" + notReserved + "  =  " + total);
                            VIS.ADialog.info("InsufficientQtyAvailable", null, info, "");
                        }
                    }
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_Order - Orig Order Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_Order = function (ctx, windowNo, mTab, mField, value, oldValue) {


        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var C_Order_ID = Util.getValueOfInt(value);
        if (C_Order_ID == null || C_Order_ID == 0)
            return "";
        this.setCalloutActive(true);
        try {
            // var U=Util;
            //	Get Details from Original Order
            //dr = VIS.dataContext.getJSONRecord("CalloutOrder/GetQtyInfo", paramString);
            var order = VIS.dataContext.getJSONRecord("MOrder/GetOrder", C_Order_ID.toString());
            //MOrder order = new MOrder(ctx, C_Order_ID, null);
            var bpartner = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", order["C_BPartner_ID"].toString());
            //MBPartner bpartner = new MBPartner(ctx, order.C_BPartner_ID, null);

            // Reset Orig Shipment
            mTab.setValue("Orig_InOut_ID", null);

            mTab.setValue("C_BPartner_ID", Util.getValueOfInt(order["C_BPartner_ID"]));
            mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(order["C_BPartner_Location_ID"]));
            mTab.setValue("Bill_BPartner_ID", Util.getValueOfInt(order["Bill_BPartner_ID"]));
            mTab.setValue("Bill_Location_ID", Util.getValueOfInt(order["Bill_Location_ID"]));

            if (order["AD_User_ID"] != 0)
                mTab.setValue("AD_User_ID", Util.getValueOfInt(order["AD_User_ID"]));

            if (order["Bill_User_ID"] != 0)
                mTab.setValue("Bill_User_ID", Util.getValueOfInt(order["Bill_User_ID"]));

            if (ctx.isSOTrx())
                mTab.setValue("M_ReturnPolicy_ID", Util.getValueOfInt(bpartner["M_ReturnPolicy_ID"]));
            else
                mTab.setValue("M_ReturnPolicy_ID", Util.getValueOfInt(bpartner["PO_ReturnPolicy_ID"]));

            //mTab.setValue("DateOrdered", order.getDateOrdered());
            //mTab.setValue("M_PriceList_ID", Util.getValueOfInt(order["M_PriceList_ID"]));

            //------------- Neha 13-01-2016 Mentis Issue no.  0000399
            if ("M_PriceList_ID" != 0) {
                this.setCalloutActive(false);
                mTab.setValue("M_PriceList_ID", Util.getValueOfInt(order["M_PriceList_ID"]));
                this.setCalloutActive(true);
            };

            //--------------End------------------
            mTab.setValue("PaymentRule", order["PaymentRule"]);
            //when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- not to update
            // false means - not to update
            var isPaymentTermUpdate = this.checkAdvancePaymentTerm(Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")), Util.getValueOfInt(order["C_PaymentTerm_ID"]));
            if (isPaymentTermUpdate) {
                mTab.setValue("C_PaymentTerm_ID", Util.getValueOfInt(order["C_PaymentTerm_ID"]));
            }
            else {
                mTab.setValue("C_PaymentTerm_ID", null);
            }
            //mTab.setValue ("DeliveryRule", X_C_Order.DELIVERYRULE_Manual);

            mTab.setValue("Bill_Location_ID", Util.getValueOfInt(order["Bill_Location_ID"]));
            mTab.setValue("InvoiceRule", order["InvoiceRule"]);
            mTab.setValue("PaymentRule", order["PaymentRule"]);
            mTab.setValue("DeliveryViaRule", order["DeliveryViaRule"]);
            mTab.setValue("FreightCostRule", order["FreightCostRule"]);

            //-------------Anuj 04-09-2015-------------------------
            var _CountVA009 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (_CountVA009 > 0) {
                var paymthd_id = VIS.DB.executeScalar("SELECT VA009_PaymentMethod_ID FROM C_Order WHERE C_Order_ID=" + C_Order_ID);
                if (paymthd_id > 0) {
                    mTab.setValue("VA009_PaymentMethod_ID", paymthd_id);
                    var PaymentBasetype = VIS.DB.executeScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID=" + paymthd_id);
                    if (PaymentBasetype != "W" && PaymentBasetype != null) {
                        //if (PaymentBasetype != null) {
                        mTab.setValue("PaymentMethod", PaymentBasetype);
                        mTab.setValue("PaymentRule", PaymentBasetype);
                    }
                }
            }

            //------------------Anuj 04-09-2015------------------------------------
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_InOut - Shipment Line Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_InOut = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            // var U=Util;
            var Orig_InOut_ID = Util.getValueOfInt(value);
            if (Orig_InOut_ID == null || Orig_InOut_ID == 0)
                return "";
            this.setCalloutActive(true);
            //	Get Details from Original Shipment
            //MInOut io = new MInOut(ctx, Orig_InOut_ID, null);
            var io = VIS.dataContext.getJSONRecord("MInOut/GetInOut", Orig_InOut_ID.toString());
            //mTab.setValue("ShipDate", Util.getValueOfDate(io.MovementDate));
            mTab.setValue("C_Project_ID", Util.getValueOfInt(io.C_Project_ID));
            mTab.setValue("C_Campaign_ID", Util.getValueOfInt(io.C_Campaign_ID));
            mTab.setValue("C_Activity_ID", Util.getValueOfInt(io.C_Activity_ID));
            mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(io.AD_OrgTrx_ID));
            mTab.setValue("User1_ID", Util.getValueOfInt(io.User1_ID));
            mTab.setValue("User2_ID", Util.getValueOfInt(io.User2_ID));
            // added by vivek on 09/10/2017 advised by pradeep to set drop ship checkbox value and warehouse_ID
            if (Util.getValueOfString(io.IsDropShip) == "Y") {
                mTab.setValue("M_Warehouse_ID", Util.getValueOfInt(io.M_Warehouse_ID));
                mTab.setValue("IsDropShip", true);
            }
            else {
                mTab.setValue("IsDropShip", false);
                mTab.setValue("M_Warehouse_ID", null);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_Order - Orig Order Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_OrderLine = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try { //var U=Util;
            var Orig_OrderLine_ID = Util.getValueOfInt(value);
            if (Orig_OrderLine_ID == null || Orig_OrderLine_ID == 0)
                return "";
            this.setCalloutActive(true);

            //MOrderLine orderline = new MOrderLine(ctx, Orig_OrderLine_ID, null);
            var orderline = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", Orig_OrderLine_ID.toString());
            mTab.setValue("Orig_InOutLine_ID", null);
            mTab.setValue("C_Tax_ID", Util.getValueOfInt(orderline["C_Tax_ID"]));
            mTab.setValue("PriceList", Util.getValueOfDecimal(orderline["PriceList"]));
            mTab.setValue("PriceLimit", Util.getValueOfDecimal(orderline["PriceLimit"]));
            mTab.setValue("PriceActual", Util.getValueOfDecimal(orderline["PriceActual"]));
            mTab.setValue("PriceEntered", Util.getValueOfDecimal(orderline["PriceEntered"]));
            mTab.setValue("C_Currency_ID", Util.getValueOfInt(orderline["C_Currency_ID"]));
            mTab.setValue("Discount", Util.getValueOfDecimal(orderline["Discount"]));
            // mTab.setValue("Discount", Util.getValueOfDecimal(orderline.Discount));
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", M_PriceList_ID.toString());


            var StdPrecision = Util.getValueOfInt(dr["StdPrecision"]);
            var QtyOrdered;
            QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));



            //	Line Net Amt
            var LineNetAmt = QtyOrdered * Util.getValueOfDecimal(orderline["PriceActual"]);

            if (Util.scale(LineNetAmt) > StdPrecision) {//LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);
            }
            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);
        }
        catch (err) {
            //MessageBox.Show("error in Orig_OrderLine");
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };

    /// <summary>
    /// Orig_InOutLine - Shipment Line Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_InOutLine = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            //var U=Util;
            var Orig_InOutLine_ID = Util.getValueOfInt(value);
            if (Orig_InOutLine_ID == null || Orig_InOutLine_ID == 0)
                return "";

            this.setCalloutActive(true);
            //	Get Details
            var Orig_InOutLine = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", Orig_InOutLine_ID.toString())
            //MInOutLine Orig_InOutLine = new MInOutLine(ctx, Orig_InOutLine_ID, null);

            if (Orig_InOutLine != null) {
                mTab.setValue("C_Project_ID", Util.getValueOfInt(Orig_InOutLine["C_Project_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(Orig_InOutLine["C_Campaign_ID"]));
                mTab.setValue("M_Product_ID", Util.getValueOfInt(Orig_InOutLine["M_Product_ID"]));
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(Orig_InOutLine["M_AttributeSetInstance_ID"]));
                mTab.setValue("C_UOM_ID", Util.getValueOfInt(Orig_InOutLine["C_UOM_ID"]));

                // JID_1310: On Selection of Shipment line on Customer/Vendor RMA. System should check Total Delivred - Total Return Qty From Sales PO line and Balance  show in qty field
                mTab.setValue("QtyEntered", Util.getValueOfDecimal(Orig_InOutLine["QtyEntered"]));
                if (Util.getValueOfString(Orig_InOutLine["IsDropShip"]) == "Y") {
                    mTab.setValue("IsDropShip", true);
                    ctx.setContext(windowNo, "IsDropShip", Util.getValueOfString(Orig_InOutLine["IsDropShip"]));
                }
                else {
                    mTab.setValue("IsDropShip", false);
                }


            }
        }
        catch (err) {
            //MessageBox.Show("error in Orig_InOutLine");
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// <param name="documnetType_Id">C_DocumentType_ID</param>
    /// <param name="PaymentTerm_Id">C_PaymentTerm_ID</param>
    /// This function will return True, when we want to show Advance payment term
    /// if doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- then system return false
    CalloutOrder.prototype.checkAdvancePaymentTerm = function (documnetType_Id, PaymentTerm_Id) {
        var isAdvancePayTerm = true;
        var dr = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", "VA009_");
        if (dr != null && !dr["VA009_"]) {
            return isAdvancePayTerm;
        }
        if (Util.getValueOfInt(documnetType_Id) != 0 && Util.getValueOfInt(PaymentTerm_Id) != 0) {
            var paramString = documnetType_Id.toString() + "," + PaymentTerm_Id.toString();
            isAdvancePayTerm = VIS.dataContext.getJSONRecord("MOrder/checkAdvancePaymentTerm", paramString);
        }
        return isAdvancePayTerm;
    };


    VIS.Model.CalloutOrder = CalloutOrder;

})(VIS, jQuery);