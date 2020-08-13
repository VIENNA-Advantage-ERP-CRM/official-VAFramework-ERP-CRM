; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //*************** CalloutContract ************  
    function CalloutContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContract");//must call
    };
    VIS.Utility.inheritPrototype(CalloutContract, VIS.CalloutEngine); //inherit prototype
    CalloutContract.prototype.InvoiceCount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var SDate = Util.getValueOfDateTime(mTab.getValue("StartDate"));
        var Edate = mTab.getValue("EndDate");
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));

        var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totaldays = (Edate - SDate).Days;
        var count = totaldays / days;
        mTab.setValue("TotalInvoice", count);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };


    CalloutContract.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var SDate = new Date(mTab.getValue("StartDate"));
        var startDate = new Date(mTab.getValue("StartDate"));
        //var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var months = VIS.dataContext.getJSONRecord("MOrderLine/GetNoOfMonths", Util.getValueOfString(mTab.getValue("C_Frequency_ID")));
        //var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        //var months = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var invoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        //var End = SDate.AddMonths(months * invoice).AddDays(-1);
        //   var End = SDate.setDate(SDate.getMonth() + (months * invoice));  by Karan
        //var End = SDate.getDate() + "/" + (SDate.getMonth() + (months * invoice)) + "/" + SDate.getFullYear();
        var End = SDate.setMonth(SDate.getMonth() + (months * invoice));
        End = new Date(SDate.setDate(SDate.getDate() - 1));
        mTab.setValue("EndDate", End);
        End = new Date(End);
        End = End.toISOString();
        SDate = startDate.toISOString();
        if (End < SDate) {
            mTab.setValue("EndDate", startDate);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutContract.prototype.BillStartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var billStartDate = new Date(mTab.getValue("BillStartDate"));
        if (billStartDate.toISOString() < SDate.toISOString()) {
            // ShowMessage.Info("StartDateShouldBeLessThanBillStartDate", true, null, null);
            VIS.ADialog.info("StartDateShouldBeLessThanBillStartDate");
            mTab.setValue("BillStartDate", mTab.getValue("StartDate"));
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutContract.prototype.StartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totalInvoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        var cycles = totalInvoice * days;
        //var endDate =SDate.addDays(Util.getValueOfDouble(cycles));
        var endDate = new Date(SDate.setDate(SDate.getDay() + Util.getValueOfDouble(cycles)));
        endDate = endDate.toISOString();
        mTab.setValue("EndDate", endDate);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    CalloutContract.prototype.StartDateChange = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        var months = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totalInvoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        var SDate = new Date(value);
        var startDate = new Date(value);
        //var endDate = SDate.addMonths(months * totalInvoice).addDays(-1);
        //var endDate = SDate.setDate(SDate.getMonth() + (months * totalInvoice));
        var endDate = SDate.setMonth(SDate.getMonth() + (months * totalInvoice));
        endDate = new Date(SDate.setDate(SDate.getDate() - 1));
        mTab.setValue("EndDate", endDate.toISOString());
        endDate = endDate.toISOString();
        SDate = startDate.toISOString();
        if (endDate < SDate) {
            mTab.setValue("EndDate", SDate);
        }
        mTab.setValue("BillStartDate", mTab.getValue("StartDate"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutContract = CalloutContract;
    //***************CalloutContract End ************

    //*************CalloutContractProduct Start ******
    function CalloutContractProduct() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContractProduct");//must call
    };
    VIS.Utility.inheritPrototype(CalloutContractProduct, VIS.CalloutEngine); //inherit prototype
    CalloutContractProduct.prototype.ContractProduct = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Proudct_ID"));
        if (M_Product_ID != 0) {
            if (Util.getValueOfBoolean(value)) {
                mTab.setValue("PriceList", 0);
                mTab.setValue("PriceLimit", 0);
                mTab.setValue("PriceActual", 0);
                mTab.setValue("PriceEntered", 0);
            }
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutContractProduct = CalloutContractProduct;
    //************* CalloutContractProduct End ******

    //****************CalloutContractQty Start ******
    function CalloutContractQty() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContractQty"); //must call
    };

    VIS.Utility.inheritPrototype(CalloutContractQty, VIS.CalloutEngine);//inherit CalloutEngine
    //	Debug Steps		
    /// <summary>
    ///
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutContractQty.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        var price = Util.getValueOfDecimal(mTab.getValue("PriceActual"));
        var val = Util.getValueOfDecimal(value);
        //
        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var LineAmount = "";
        var TotalRate = VIS.Env.ZERO;
        mTab.setValue("LineNetAmt", price * val);

        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(Rate)) / 100);
        TotalRate = TotalRate.toFixed(2);
        mTab.setValue("taxamt", TotalRate);
        mTab.setValue("GrandTotal", ((price * val) + Util.getValueOfDecimal(mTab.getValue("TaxAmt"))));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutContractQty = CalloutContractQty;
    //****************CalloutContractQty End ******

    //***************CalloutOrderContract Start********************

    function CalloutOrderContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderContract");//must call
    };

    VIS.Utility.inheritPrototype(CalloutOrderContract, VIS.CalloutEngine); //inherit prototype

    //	Debug Steps		


    /// <summary>
    /// Order Header Change - DocType.
    /// - InvoiceRuld/DeliveryRule/PaymentRule
    /// - temporary Document
    ///   Context:
    ///   - DocSubTypeSO
    ///   - HasCharges\
    ///   - (re-sets Business Partner info of required)
    ///            *  @param ctx      
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrderContract.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** InvoiceRule AD_Reference_ID=150 */
        var INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** DeliveryRule AD_Reference_ID=151 */
        var DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var DELIVERYRULE_Availability = "A";
        /** Force = F */
        var DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var DELIVERYRULE_AfterReceipt = "R";

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

        var sql = "";
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var C_DocType_ID = value;		//	Actually C_DocTypeTarget_ID
            if (C_DocType_ID == null || C_DocType_ID == 0) {
                return "";
            }
            var oldDocNo;
            //if (mTab.getValue("DocumentNo") == DBNull.Value)
            //{
            //    oldDocNo = DBNull.Value.toString();
            //}
            //else
            //{
            oldDocNo = mTab.getValue("DocumentNo");
            // }
            //	Re-Create new DocNo, if there is a doc number already
            //	and the existing source used a different Sequence number
            //var oldDocNo = (String)mTab.getValue("DocumentNo");
            //var newDocNo = (oldDocNo == null);
            var newDocNo = (oldDocNo == null);
            if (!newDocNo && oldDocNo.toString().startsWith("<") && oldDocNo.toString().endsWith(">"))
                newDocNo = true;
            var oldC_DocType_ID = mTab.getValue("C_DocType_ID");

            sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
                 + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
                 + "s.AD_Sequence_ID,d.IsSOTrx, d.IsReturnTrx "              //	7..9
                /*//jz right outer join
                + "FROM C_DocType d, AD_Sequence s "
                + "WHERE C_DocType_ID=?"	//	#1
                + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)"; */
                 + "FROM C_DocType d "
                 + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
                 + "WHERE C_DocType_ID=";	//	#1

            var AD_Sequence_ID = 0;
            //DataSet ds = new DataSet();
            var ds = null;
            //	Get old AD_SeqNo for comparison
            if (!newDocNo && oldC_DocType_ID != 0) {
                sql = sql + oldC_DocType_ID;
                ds = VIS.DB.executeDataset(sql, null);
                //ds.setInt(1, oldC_DocType_ID.intValue());
                //ResultSet dr = ds.executeQuery();
                for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                    // DataRow dr = ds.Tables[0].Rows[i];
                    AD_Sequence_ID = Util.getValueOfInt(ds.getRows()[i].getCell("AD_Sequence_ID"));
                }
            }
            else {
                sql = sql + C_DocType_ID;
                ds = VIS.DB.executeDataset(sql, null);
            }
            var DocSubTypeSO = "";
            var isSOTrx = true;
            var isReturnTrx = false;
            //	we found document type
            for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                // DataRow dr = ds.Tables[0].Rows[i];
                //	Set Context:	Document Sub Type for Sales Orders
                DocSubTypeSO = Util.getValueOfString(ds.getRows()[i].getCell("DocSubTypeSO"));
                if (DocSubTypeSO == null)
                    DocSubTypeSO = "--";
                ctx.setContext(windowNo, "OrderType", DocSubTypeSO);
                //	No Drop Ship other than Standard
                if (!DocSubTypeSO.toString().equals(DocSubTypeSO_Standard))
                    mTab.setValue("IsDropShip", "N");

                //	IsSOTrx
                if ("N".toString().equals(Util.getValueOfString(ds.getRows()[i].getCell("IsSOTrx"))))
                    isSOTrx = false;

                //IsReturnTrx
                isReturnTrx = "Y".toString().equals(Util.getValueOfString(ds.getRows()[i].getCell("IsReturnTrx")));

                //	Skip these steps for RMA. These are copied from the Original Order
                if (!isReturnTrx) {
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Force);
                    else if (DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_AfterReceipt);
                    else
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Availability);

                    //	Invoice Rule
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS)
                        || DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay)
                        || DocSubTypeSO.toString().equals(DocSubTypeSO_OnCredit))
                        mTab.setValue("InvoiceRule", INVOICERULE_Immediate);
                    else
                        mTab.setValue("InvoiceRule", INVOICERULE_AfterDelivery);

                    //	Payment Rule - POS Order
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("PaymentRule", PAYMENTRULE_Cash);
                    else
                        mTab.setValue("PaymentRule", PAYMENTRULE_OnCredit);

                    //	Set Context:
                    ctx.setContext(windowNo, "HasCharges", Util.getValueOfString(ds.getRows()[i].getCell("HasCharges")));
                }
                else // Returns
                {
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Force);
                    else
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Manual);
                }

                //	DocumentNo
                if (dr[3].toString().equals("Y"))			//	IsDocNoControlled
                {
                    if (!newDocNo && AD_Sequence_ID != Util.getValueOfInt(ds.getRows()[i].getCell("AD_Sequence_ID")))
                        newDocNo = true;
                    if (newDocNo)
                        try {
                        //if (/*Ini.IsPropertyBool(Ini._COMPIERESYS) &&*/ GetCtx().GetAD_Client_ID() < 1000000)
                        //{
                        //    mTab.setValue("DocumentNo", "<" + dr[5].toString() + ">");
                        //}
                        //else
                        //{
                            mTab.setValue("DocumentNo", "<" + Util.getValueOfString(ds.getRows()[i].getCell("CurrentNext")) + ">");
                        // }
                        }
                        catch (err) {
                            this.setCalloutActive(false);
                        }
                }
            }
            // Skip remaining steps for RMA
            if (isReturnTrx)
                return "";
            //  When BPartner is changed, the Rules are not set if
            //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
            //  This re-reads the Rules and applies them.
            if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS) || DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay))    //  not for POS/PrePay
            {

            }
            else {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
                    + "InvoiceRule,DeliveryRule,"                       //  3..4
                    + "FreightCostRule,DeliveryViaRule, "               //  5..6
                    + "PaymentRulePO,PO_PaymentTerm_ID "
                    + "FROM C_BPartner "
                    + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
                ds = VIS.DB.executeDataset(sql, null);
                for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                    var dr = ds.getTables()[0].rows[i];
                    //	PaymentRule

                    //var s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].toString();
                    var s = Util.getValueOfString(dr.get(isSOTrx ? "paymentrule" : "paymentrulepo"));
                    if (s != null && s.toString().length != 0) {
                        if (isSOTrx && (s.toString().equals("B") || s.toString().equals("S") || s.toString().equals("U")))           	//	No Cash/Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        if (!isSOTrx && (s.toString().equals("B")))					//	No Cash for PO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    //var ii = Util.getValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].toString());
                    var ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    //if (!dr.wasNull())
                    if (dr != null) {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	InvoiceRule
                    s = dr[2].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = dr[3].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = dr[4].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = dr[5].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }   //  re-read customer rules
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, sql, err);
            return e.message;

        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
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
    CalloutOrderContract.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** InvoiceRule AD_Reference_ID=150 */
        var INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** DeliveryRule AD_Reference_ID=151 */
        var DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var DELIVERYRULE_Availability = "A";
        /** Force = F */
        var DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var DELIVERYRULE_AfterReceipt = "R";

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

        var sql = "";
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var C_BPartner_ID = 0;
            if (value != null)
                C_BPartner_ID = Util.getValueOfInt(value.toString());
            if (C_BPartner_ID == 0)
                return "";

            // Skip rest of steps for RMA. These fields are copied over from the orignal order instead.
            var isReturnTrx = mTab.getValue("IsReturnTrx");
            if (isReturnTrx)
                return "";

            this.setCalloutActive(true);

            sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
                + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

            var ds = VIS.DB.executeDataset(sql, null);
            for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                var dr = ds.getTables()[0].rows[i];
                //	PriceList (indirect: IsTaxIncluded & Currency)
                var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                if (dr != null && ii != 0) {
                    mTab.setValue("M_PriceList_ID", ii);
                }

                // JID_0364: If price list not available at BP, user need to select it manually

                //else {	//	get default PriceList
                //    var i1 = ctx.getContextAsInt("#M_PriceList_ID");
                //    if (i1 != 0)
                //        mTab.setValue("M_PriceList_ID", i1);
                //}
                //	Bill-To BPartner
                mTab.setValue("Bill_BPartner_ID", C_BPartner_ID);
                var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
                if (bill_Location_ID == 0)
                    mTab.setValue("Bill_Location_ID", null);
                else
                    mTab.setValue("Bill_Location_ID", bill_Location_ID);

                // Ship-To Location
                var shipTo_ID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.toString().length > 0)
                        shipTo_ID = int.Parse(loc);
                }
                if (shipTo_ID == 0)
                    mTab.setValue("C_BPartner_Location_ID", null);
                else {
                    mTab.setValue("C_BPartner_Location_ID", shipTo_ID);
                    if ("Y".toString().equals(Util.getValueOfString(dr.get("isshipto"))))	//	set the same
                        mTab.setValue("Bill_Location_ID", shipTo_ID);
                }
                //	Contact - overwritten by InfoBP selection
                var contID = Util.getValueOfInt(dr.get("ad_user_id").toString());
                if (C_BPartner_ID.toString().toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.toString().length > 0)
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
                    var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                    //	var SOCreditStatus = dr.getString("SOCreditStatus");
                    if (CreditLimit != 0) {
                        var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                        if (dr != null && CreditAvailable < 0) {
                            //mTab.fireDataStatusEEvent("CreditLimitOver",
                            //    DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),
                            //    false);
                            // MessageBox.Show("Create fireDataStatusEEvent");
                        }
                    }
                }

                //	VAdvantage.Model.PO Reference
                var s = dr.get("poreference").toString();
                if (s != null && s.toString().length != 0)
                    mTab.setValue("POReference", s);
                // should not be reset to null if we entered already value! VHARCQ, accepted YS makes sense that way
                // TODO: should get checked and removed if no longer needed!
                /*else
                    mTab.setValue("POReference", null);*/

                //	SO Description
                s = Util.getValueOfString(dr.get("so_description"));
                if (s != null && s.toString().trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                s = Util.getValueOfString(dr.get("isdiscountprinted"));
                if (s != null && s.toString().length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", INVOICERULE_AfterDelivery);
                mTab.setValue("DeliveryRule", DELIVERYRULE_Availability);
                mTab.setValue("PaymentRule", PAYMENTRULE_OnCredit);
                if (OrderType.toString().equals(DocSubTypeSO_Prepay)) {
                    mTab.setValue("InvoiceRule", INVOICERULE_Immediate);
                    mTab.setValue("DeliveryRule", DELIVERYRULE_AfterReceipt);
                }
                else if (OrderType.toString().equals(MOrder.DocSubTypeSO_POS))	//  for POS
                    mTab.setValue("PaymentRule", PAYMENTRULE_Cash);
                else {
                    //	PaymentRule
                    s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].toString();
                    if (s != null && s.length != 0) {
                        if (s.toString().equals("B"))				//	No Cache in Non POS
                            s = "P";
                        if (isSOTrx && (s.toString().equals("S") || s.toString().equals("U")))	//	No Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    if (dr != null && ii != 0)//ii=0 when dr return ""
                    {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	InvoiceRule
                    s = Util.getValueOfString(dr.get("invoicerule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = Util.getValueOfString(dr.get("deliveryrule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = Util.getValueOfString(dr.get("freightcostrule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = Util.getValueOfString(dr.get("deliveryviarule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }
        }
        catch (err) {
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return e.message;
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
    CalloutOrderContract.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        try {  ///
            // Boolean flag1 = System.Convert.ToBoolean(value);
            //// Int32 M_Product_ID = 0;
            // var ProductType = "";
            var M_Product_ID = value;
            if (M_Product_ID == null || M_Product_ID == 0)
                return "";

            var isReturnTrx = "Y".toString().equals(ctx.getContext("IsReturnTrx"));
            if (isReturnTrx)
                return "";

            this.setCalloutActive(true);
            if (steps) {
                this.log.warning("init");
            }
            //
            mTab.setValue("C_Charge_ID", null);
            //	Set Attribute
            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.

            //if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
            //    && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
            //    mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID")));
            //}
            //else {
            mTab.setValue("M_AttributeSetInstance_ID", null);
            //}

            /*****	Price Calculation see also qty	****/
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
            var Qty = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //        M_Product_ID, C_BPartner_ID, Qty, isSOTrx);
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            // pp.SetM_PriceList_ID(M_PriceList_ID);

            //Get PriceListversion based on Pricelist
            var M_PriceList_Version_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", M_PriceList_ID.toString());

            /** PLV is only accurate if PL selected in header */
            if (M_PriceList_Version_ID == 0) {
                M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            }
            //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            var orderDate = mTab.getValue("DateOrdered");
            //pp.SetPriceDate(orderDate);
            //	
            //if (orderDate == null) {
            //    orderDate = Date.now();
            //}

            /***** Bharat ****/
            var sql = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'";
            countEd011 = Util.getValueOfInt(VIS.DB.executeScalar(sql));
            var purchasingUom = 0;
            if (countEd011 > 0) {
                sql = "SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND  C_BPartner_ID = " + Util.getValueOfInt(mTab.getValue("C_BPartner_ID")) +
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
            var C_UOM_ID = Util.getValueOfInt(mTab.getValue("C_UOM_ID"))


            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                          Qty.toString(), ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID.toString(), ",", //5
                                                          M_PriceList_Version_ID.toString(), ",", //6
                                                          null, ",",         //7
                                                          null, ",", null, ",",  //8 , 9
                                                          C_UOM_ID, ",", countEd011); // 10 , 11

            //var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
            //                                           Qty.toString(), ",", //3
            //                                           isSOTrx, ",", //4 
            //                                           M_PriceList_ID.toString(), ",", //5
            //                                           M_PriceList_Version_ID.toString(), ",", //6
            //                                           null, ",", null, ",", null); //7
            /*** Bharat Done ***/
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            //mTab.setValue("PriceList", pp.GetPriceList());
            //mTab.setValue("PriceLimit", pp.GetPriceLimit());
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            //mTab.setValue("PriceEntered", pp.GetPriceStd());
            //mTab.setValue("C_Currency_ID", Util.getValueOfInt(pp.GetC_Currency_ID()));
            //mTab.setValue("Discount", pp.GetDiscount());
            //mTab.setValue("C_UOM_ID", Util.getValueOfInt(pp.GetC_UOM_ID()));
            //mTab.setValue("QtyEntered", mTab.getValue("QtyEntered"));
            //ctx.setContext(windowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
            //ctx.setContext(windowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");


            mTab.setValue("PriceList", dr["PriceList"]);
            mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PriceActual", dr.PriceActual);
            mTab.setValue("PriceEntered", dr.PriceEntered);
            if (Util.getValueOfInt(dr.C_Currency_ID) > 0) {
                mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            }
            mTab.setValue("Discount", dr.Discount);
            mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
            mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
            ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
            ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");



            //	Check/Update Warehouse Setting
            //	var M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
            //	var wh = (int)mTab.getValue("M_Warehouse_ID");
            //	if (wh.intValue() != M_Warehouse_ID)
            //	{
            //		mTab.setValue("M_Warehouse_ID", new int(M_Warehouse_ID));
            //		ADiathis.log.warn(,windowNo, "WarehouseChanged");
            //	}

            //if (ctx.getContext("IsSOTrx"))
            if (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") {
                //  MProduct product = MProduct.Get(ctx, M_Product_ID);
                var paramString = M_Product_ID.toString() + ",0";
                var isStocked = VIS.dataContext.getJSONRecord("MProduct/GetProduct", paramString);
                if (isStocked) {
                    var QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                    var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                    var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
                    var paramString = M_Warehouse_ID + "," + M_Product_ID + "," + M_AttributeSetInstance_ID;
                    var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);
                    //var available = MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                        available = VIS.Env.ZERO;
                    if (available == 0) {
                        //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                        //  ShowMessage.Info("NoQtyAvailable", true, "0", "");
                    }
                    else if (available.toString().compareTo(QtyEntered) < 0) {
                        //mTab.fireDataStatusEEvent("InsufficientQtyAvailable", available.toString(), false);
                        //  ShowMessage.Info("InsufficientQtyAvailable", true, available.toString(), "");
                    }
                    else {
                        var C_OrderLine_ID = 0;
                        if (mTab.getValue("C_OrderLine_ID") != null) {
                            C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                        }

                        if (C_OrderLine_ID == null)
                            C_OrderLine_ID = 0;
                        paramString = M_Warehouse_ID.toString() + "," + M_Product_ID.toString() + "," + M_AttributeSetInstance_ID.toString() + "," + C_OrderLine_ID.toString();
                        var notReserved = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/GetNotReserved", paramString));
                        //var notReserved = MOrderLine.GetNotReserved(ctx,
                        //    M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                        //    C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = VIS.Env.ZERO;
                        //var total = available.subtract(notReserved);
                        var total = (available - notReserved);
                        if (total.toString().compareTo(QtyEntered) < 0) {
                            //var info = Msg.ParseTranslation(ctx, "@QtyAvailable@=" + available  //Temporary commented
                            //  + " - @QtyNotReserved@=" + notReserved + " = " + total);
                            //mTab.fireDataStatusEEvent("InsufficientQtyAvailable",info, false);
                            //ShowMessage.Info("InsufficientQtyAvailable", true, info, ""); //Temporary commented BY sarab
                        }
                    }
                }
            }
            this.setCalloutActive(false);

            if (steps) {
                this.log.warning("fini");
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            // MessageBox.Show("error in Product");
        }
        // ctx = windowNo = mTab = mField = value = oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };

    CalloutOrderContract.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var TotalRate = VIS.Env.ZERO;
        var LineAmount = "";
        //var GrandTotal =VIS.Env.ZERO;
        //var Discount=Env.ZERO;
        //var taxamt =VIS.Env.ZERO;
        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));
        LineAmount = this.Amt(ctx, windowNo, mTab, mField, value);
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(Rate)) / 100);
        mTab.setValue("taxamt", TotalRate);
        // ctx = windowNo = mTab = mField = value = oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    CalloutOrderContract.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {

            if (steps) {
                this.log.warning("init");
            }

            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            var aparam = M_PriceList_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", aparam);
            var StdPrecision = Util.getValueOfInt(dr["StdPrecision"]);//Neha--GetPriceList Method's Dictionary returns string value--06 Sep,2018
            //var StdPrecision = MPriceList.GetPricePrecision(ctx, M_PriceList_ID);
            var QtyEntered, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            this.log.fine("QtyEntered=" + QtyEntered + ", Ordered=" + QtyEntered + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

            Discount = Util.getValueOfDecimal(mTab.getValue("Discount"));
            PriceLimit = Util.getValueOfDecimal(mTab.getValue("PriceLimit"));
            PriceList = Util.getValueOfDecimal(mTab.getValue("PriceList"));
            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((mField.getColumnName().toString().equals("QtyEntered")
                || mField.getColumnName().toString().equals("QtyEntered")
                || mField.getColumnName().toString().equals("M_Product_ID"))
                && !"N".toString().equals(ctx.getContext("DiscountSchema"))) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName().toString().equals("QtyEntered")) {
                    var paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + QtyEntered.toString();
                    //QtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                    //    C_UOM_To_ID, QtyEntered);
                    QtyEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString));
                }
                //if (QtyEntered == null) {//Neha---Do not have any logic in if statement--06 Sep,2018

                //}
                //  QtyEntered = QtyEntered;
                var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

                var M_PriceList_Version_ID = VIS.dataContext.getJSONRecord("MPriceListVersion/GetM_PriceList_Version_ID", M_PriceList_ID.toString());
                if (M_PriceList_Version_ID == 0) {
                    M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                }
                //var date = mTab.getValue("DateOrdered");
                //if (date == null) {
                //    date = Date.now();
                //}

                /***** Bharat ****/
                var sql = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'";
                countEd011 = Util.getValueOfInt(VIS.DB.executeScalar(sql));
                var purchasingUom = 0;
                if (countEd011 > 0) {
                    sql = "SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND  C_BPartner_ID = " + Util.getValueOfInt(mTab.getValue("C_BPartner_ID")) +
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
                var C_UOM_ID = Util.getValueOfInt(mTab.getValue("C_UOM_ID"))


                var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                              QtyEntered.toString(), ",", //3
                                                              isSOTrx, ",", //4 
                                                              M_PriceList_ID.toString(), ",", //5
                                                              M_PriceList_Version_ID.toString(), ",", //6
                                                              null, ",",         //7
                                                              null, ",", null, ",",  //8 , 9
                                                              C_UOM_ID, ",", countEd011); // 10 , 11

                //var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                //                                            QtyEntered.toString(), ",", //3
                //                                            isSOTrx, ",", //4 
                //                                            M_PriceList_ID.toString(), ",", //5
                //                                            M_PriceList_Version_ID.toString(), ",", //6
                //                                            null, ",", null, ",", null); //7

                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);



                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //        M_Product_ID, C_BPartner_ID, QtyEntered, isSOTrx);
                //pp.SetM_PriceList_ID(M_PriceList_ID);
                //var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                //var date = Util.getValueOfDateTime(mTab.getValue("DateOrdered"));
                //pp.SetPriceDate(date);
                ////

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + dr.PriceStd.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, pp.GetPriceStd());
                if (PriceEntered == null || PriceEntered == 0)//Neha--PriceEntered is decimal so It can't be null.--06 Sep,2018
                    PriceEntered = dr.PriceStd;
                //
                this.log.fine("QtyChanged -> PriceActual=" + dr.PriceStd
                    + ", PriceEntered=" + PriceEntered + ", Discount=" + dr.Discount);
                PriceActual = dr.PriceStd;
                mTab.setValue("PriceActual", dr.PriceActual);
                mTab.setValue("Discount", dr.Discount);
                mTab.setValue("PriceEntered", dr.PriceEntered);
                ctx.setContext(windowNo, "DiscountSchema", dr.DiscountSchema ? "Y" : "N");
            }
            else if (mField.getColumnName().toString().equals("PriceActual")) {
                PriceActual = Util.getValueOfDecimal(value);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceActual.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));


                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null || PriceEntered == 0)//Neha--PriceEntered is decimal so It can't be null.--06 Sep,2018
                    PriceEntered = PriceActual;
                //
                this.log.fine("PriceActual=" + PriceActual
                    + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName().toString().equals("PriceEntered")) {
                PriceEntered = Util.getValueOfDecimal(value);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceEntered.toString();

                PriceActual = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString));

                //PriceActual = (Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceEntered);
                if (PriceActual == null || PriceEntered == 0)//Neha--PriceEntered is decimal so It can't be null.--06 Sep,2018
                    PriceActual = PriceEntered;
                //
                this.log.fine("PriceEntered=" + PriceEntered
                    + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (mField.getColumnName().toString().equals("Discount")) {

                PriceActual = Util.getValueOfDecimal(((100.0 - Util.getValueOfDouble(Discount))
                    / 100.0 * Util.getValueOfDouble(PriceList)));
                if (Util.scale(PriceActual) > StdPrecision)
                    PriceActual = PriceActual.toFixed(StdPrecision);//);//, MidpointRounding.AwayFromZero);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceActual.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null || PriceEntered == 0)//Neha--PriceEntered is decimal so It can't be null.--06 Sep,2018
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
                    Discount = Util.getValueOfDecimal((((PriceList) - (PriceActual)) / (PriceList) * 100.0));
                }
                if (Util.scale(Discount) > 2)
                    // Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                    Discount = Discount.toFixed(2);//);//, MidpointRounding.AwayFromZero);


                mTab.setValue("Discount", Discount);
            }
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") && epl != null && epl.toString().equals("Y");
            var isReturnTrx = "Y".toString().equals(ctx.getContext("IsReturnTrx"));
            if (enforce && (VIS.MRole.getDefault().IsOverwritePriceLimit() || isReturnTrx))
                enforce = false; //Temporary Commented
            //	Check Price Limit?
            if (enforce && Util.getValueOfDouble(PriceLimit) != 0.0
              && PriceActual.toString().compare(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceLimit.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceLimit.Value);
                if (PriceEntered == null || PriceEntered == 0)//Neha--PriceEntered is decimal so It can't be null.--06 Sep,2018
                    PriceEntered = PriceLimit;
                this.log.fine("(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                mTab.setValue("PriceActual", PriceLimit);
                mTab.setValue("PriceEntered", PriceEntered);
                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                //ShowMessage.Info("UnderLimitPrice", true, "", ""); //Temporary commented by sarab
                VIS.ADialog.info("UnderLimitPrice");
                //	Repeat Discount calc
                if (PriceList != 0) {
                    Discount = Util.getValueOfDecimal((((PriceList) - (PriceActual)) / (PriceList) * 100.0));
                    if (Util.scale(Discount) > 2) {
                        // Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                        Discount = Discount.toFixed(2);//);//, MidpointRounding.AwayFromZero);
                    }
                    mTab.setValue("Discount", Discount);
                }
            }

            //	Line Net Amt
            var LineNetAmt = (QtyEntered * PriceActual);
            if (Util.scale(LineNetAmt) > StdPrecision)
                // LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);//);//, MidpointRounding.AwayFromZero);
            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);


            mTab.setValue("GrandTotal", (Util.getValueOfDecimal(mTab.getValue("TaxAmt")) + LineNetAmt));
            //Decimal? GrandTotal = Decimal.Add(Util.getValueOfDecimal(mTab.getValue("TaxAmt")), LineNetAmt);


        }
        catch (err) {

            // MessageBox.Show("error in Amt" + ex.Message.toString());
            //return "";
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutOrderContract = CalloutOrderContract;
    //*********************************

    //***************CalloutSetContract Starts***********
    function CalloutSetContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetContract");//must call
    };

    VIS.Utility.inheritPrototype(CalloutSetContract, VIS.CalloutEngine); //inherit prototype


    CalloutSetContract.prototype.SetContract = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var paramStr = M_Product_ID.toString().concat(","); //1
            var productType = VIS.dataContext.getJSONRecord("MProduct/GetProductType", paramStr);

            // var precision = gp;//MProduct.get(ctx, M_Product_ID).getUOMPrecision();

            // MProduct prod = new MProduct(ctx, M_Product_ID, null);
            if (productType == "S") {
                mTab.getField("IsContract").setReadOnly(false);
            }
            else {
                mTab.getValue("IsContract", false);
                mTab.getField("IsContract").setReadOnly(true);
            }
        }
        catch (ex) {
            this.log.severe(ex.toString());
        }
        this.setCalloutActive(false);
        return "";
    };

    VIS.Model.CalloutSetContract = CalloutSetContract;
    //*************** CalloutSetContract Ends*****

})(VIS, jQuery);