; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;


    //*********** CalloutPayment Start ****
    function CalloutPayment() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPayment"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPayment, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Payment_Invoice.
    /// when Invoice selected
    /// - set C_Currency_ID 
    /// - C_BPartner_ID
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - PayAmt = invoiceOpen (ID) - Discount
    /// - WriteOffAmt = 0
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutPayment.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Invoice_ID = Util.getValueOfInt(value.toString());//(int)value;
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Invoice_ID == null
            || C_Invoice_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        mTab.setValue("C_Order_ID", null);
        mTab.setValue("C_Charge_ID", null);
        mTab.setValue("IsPrepayment", false);//Boolean.FALSE);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("IsOverUnderPayment", false);//Boolean.FALSE);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        var C_InvoicePaySchedule_ID = 0;
        var IsReturnTrx = "";
        //Added by Bharat
        var invoiceDiscrepancy = false;
        var payDiscrepancy = mTab.getField("IsInDispute") != null;

        //------------14-11-2014---------
        //Pratap changes 7-8-14/////
        var dueAmount = 0;
        var _chk = 0;

        // Added by Bharat on 02 June 2017 to remove client side queries

        var drAmt = null;
        try {
            drAmt = VIS.dataContext.getJSONRecord("MInvoice/GetInvPaySchedDetail", C_Invoice_ID.toString());
            if (drAmt != null) {
                C_InvoicePaySchedule_ID = Util.getValueOfInt(drAmt["C_InvoicePaySchedule_ID"]);
                dueAmount = Util.getValueOfDecimal(drAmt["dueAmount"]);
                IsReturnTrx = Util.getValueOfString(drAmt["IsReturnTrx"]);
                if ("Y" == IsReturnTrx) {
                    dueAmount = (dueAmount) * (-1);
                }
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
                mTab.setValue("PayAmt", dueAmount);
                _chk = 1;
                var DataPrefix = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", "VA009_");
                if (DataPrefix["VA009_"]) {
                    if ((Util.getValueOfString(drAmt["VA009_PaymentBaseType"]) != "B") && (Util.getValueOfString(drAmt["VA009_PaymentBaseType"]) != "C") && (Util.getValueOfString(drAmt["VA009_PaymentBaseType"]) != "P")) {
                        mTab.setValue("VA009_PaymentMethod_ID", Util.getValueOfInt(drAmt["VA009_PaymentMethod_ID"]));
                    }
                    else
                        mTab.setValue("VA009_PaymentMethod_ID", null);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, "CalloutPayment.Invoice -" + C_Invoice_ID, err.message);
            return err.toString();
        }
        // Bharat

        if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
            && ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID") != 0) {
            C_InvoicePaySchedule_ID = ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID");
        }

        //  Payment Date
        var ts = new Date(mTab.getValue("DateTrx"));
        var tsDate;
        if (ts == null) {
            //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
            ts = new Date();
        }

        // Added by Bharat on 02 June 2017 to remove client side queries

        var paramString = C_Invoice_ID.toString() + "," + C_InvoicePaySchedule_ID.toString() + "," + ts.toString();
        var dr = null;
        try {
            dr = VIS.dataContext.getJSONRecord("MPayment/GetInvoiceData", paramString);
            if (dr != null) {
                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr["C_BPartner_ID"]));//.getInt(1)));
                mTab.setValue("C_Bpartner_Location_Id", Util.getValueOfInt(dr["C_Bpartner_Location_Id"]));
                var C_Currency_ID = Util.getValueOfInt(dr["C_Currency_ID"]);					//	Set Invoice Currency
                mTab.setValue("C_Currency_ID", C_Currency_ID);

                // JID_1208: System should set Currency Type that is defined on Invoice.
                mTab.setValue("C_ConversionType_ID", dr["C_ConversionType_ID"]);
                var invoiceOpen = Util.getValueOfDecimal(dr["invoiceOpen"]);            		//	Set Invoice Open Amount
                if (invoiceOpen == null) {
                    invoiceOpen = VIS.Env.ZERO;
                }
                var discountAmt = Util.getValueOfDecimal(dr["invoiceDiscount"]);        		//	Set Discount Amt
                if (discountAmt == null) {
                    discountAmt = VIS.Env.ZERO;
                }

                if (_chk == 0)//Pratap
                {
                    mTab.setValue("PayAmt", (invoiceOpen - discountAmt));
                }
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);//Pratap
                mTab.setValue("DiscountAmt", discountAmt);
                //  reset as dependent fields get reset
                ctx.setContext(windowNo, "C_Invoice_ID", C_Invoice_ID.toString());

                //Added by Bharat to Set Discrepancy from Invoice
                invoiceDiscrepancy = Util.getValueOfString(dr["IsInDispute"]) == "Y";
                if (payDiscrepancy) {
                    if (invoiceDiscrepancy) {
                        mTab.setValue("IsInDispute", true);
                        VIS.ADialog.info("DiscrepancyInvoice");
                    }
                    else {
                        mTab.setValue("IsInDispute", false);
                    }
                }
            }

        }
        catch (err) {
            //if (dr != null) {
            //    dr.close();
            //    dr = null;
            //}
            //this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err.message;
        }
        this.setCalloutActive(false);
        // ctx = mTab = mField = value = oldValue = null;
        return this.DocType(ctx, windowNo, mTab, mField, value);
    };


    /// <summary>
    /// Payment_Order.
    /// when Waiting Payment Order selected
    /// - set C_Currency_ID
    /// - C_BPartner_ID
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - PayAmt = invoiceOpen (ID) - Discount
    /// - WriteOffAmt = 0
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField"> Grid Field</param>
    /// <param name="value"> New Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Order = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Order_ID = value;
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Order_ID == null
            || C_Order_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        mTab.setValue("C_Invoice_ID", null);
        mTab.setValue("C_Charge_ID", null);
        mTab.setValue("IsPrepayment", true);// Boolean.TRUE);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("IsOverUnderPayment", false);// Boolean.FALSE);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        //  Payment Date
        var ts = mTab.getValue("DateTrx");
        if (ts == null) {
            ts = new Date();
        }

        // Added by Bharat on 18/May/2017 to remove client side queries

        var _CountVA009 = false;
        var paramString = "VA009_";
        try {
            var dr = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", paramString);
            if (dr != null) {
                _CountVA009 = dr["VA009_"];
            }
            paramString = _CountVA009.toString() + "," + C_Order_ID.toString();
            dr = VIS.dataContext.getJSONRecord("MPayment/GetOrderData", paramString);
            if (dr != null) {
                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr["C_BPartner_ID"]));
                mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(dr["C_BPartner_Location_ID"]));
                var C_Currency_ID = Util.getValueOfInt(dr["C_Currency_ID"]);
                mTab.setValue("C_Currency_ID", C_Currency_ID);
                //
                var grandTotal = Util.getValueOfDecimal(dr["GrandTotal"]);
                if (grandTotal == null) {
                    grandTotal = VIS.Env.ZERO;
                }
                if (_CountVA009) {
                    var PaymentMethod = Util.getValueOfInt(dr["VA009_PaymentMethod_ID"]);
                    mTab.setValue("VA009_PaymentMethod_ID", PaymentMethod);
                }
                mTab.setValue("PayAmt", grandTotal);
            }
        }
        catch (err) {
            //if (dr != null) {
            //    dr.close();
            //    dr = null;
            //}
            //this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }

        this.setCalloutActive(false);
        //ctx = mTab = mField = value = oldValue = null;
        return this.DocType(ctx, windowNo, mTab, mField, value);
    };


    /// <summary>
    /// Payment_Charge.
    /// - reset - C_BPartner_ID, Invoice, Order, Project,
    /// Discount, WriteOff
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Charge = function (ctx, windowNo, mTab, mField, value) {
        //  
        if (value == null || value.toString() == "") {
            return "";

            var C_Charge_ID = value;
            if (this.isCalloutActive()		//	assuming it is resetting value
                || C_Charge_ID == null
                || C_Charge_ID == 0) {
                return "";
            }
            this.setCalloutActive(true);
            mTab.setValue("C_Invoice_ID", null);
            mTab.setValue("C_Order_ID", null);
            //	 mTab.setValue("C_Project_ID", null);
            mTab.setValue("IsPrepayment", false);// Boolean.FALSE);
            //
            mTab.setValue("DiscountAmt", VIS.Env.ZERO);
            mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
            mTab.setValue("IsOverUnderPayment", false);// Boolean.FALSE);
            mTab.setValue("OverUnderAmt", VIS.Env.ZERO);
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldValue = null;
            return "";
        }
    };


    /// <summary>
    /// Payment_Document Type.
    /// Verify that Document Type (AP/AR) and Invoice (SO/PO) are in sync
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns
    CalloutPayment.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {


        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        var C_Order_ID = ctx.getContextAsInt(windowNo, "C_Order_ID");
        var C_DocType_ID = ctx.getContextAsInt(windowNo, "C_DocType_ID");
        this.log.fine("Payment_DocType - C_Invoice_ID=" + C_Invoice_ID + ", C_DocType_ID=" + C_DocType_ID);
        var paramString = C_DocType_ID.toString();

        var dt = VIS.dataContext.getJSONRecord("MDocType/GetDocType", paramString);
        var isSOTrx = Util.getValueOfBoolean(dt["IsSOTrx"]);
        if (C_DocType_ID != 0) {
            // dt = MDocType.Get(ctx, C_DocType_ID);

            // ctx.setIsSOTrx(windowNo, dt["IsSOTrx"]);
            // Change by mohit to set the Sales transaction value in window context according to the selection of document type on payment window- asked by Mukesh Sir- 3 April 2019.
            ctx.setWindowContext(windowNo, "IsSOTrx", isSOTrx ? "Y" : "N");
            //ctx.setContext("IsSOTrx", isSOTrx ? "Y" : "N");
        }
        //	Invoice//	Invoice
        if (C_Invoice_ID != 0) {

            paramString = C_Invoice_ID.toString();
            var inv = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);
            //  MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            if (dt != null) {
                if (Util.getValueOfBoolean(inv["IsSOTrx"]) != isSOTrx) {
                    return VIS.Msg.getMsg("DocTypeInvInconsistent");
                }
            }
        }
        //	Order
        if (C_Order_ID != 0) {
            // Edited by Vivek assigned by Mukesh sir on 05/10/2017
            paramString = C_Order_ID.toString();
            var Ord = VIS.dataContext.getJSONRecord("MOrder/GetOrder", paramString);
            //  MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            if (dt != null) {
                if (Util.getValueOfBoolean(Ord["IsSOTrx"]) != isSOTrx) {
                    return VIS.Msg.getMsg("DocTypeOrdInconsistent");
                }
            }
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    // Manish 22/2/2017 Get all amount with currency wise.

    CalloutPayment.prototype.AmountCurrencyType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //if (value == null || value.toString() == "") {
        //    return "";
        //}
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        var bp_BusinessPartner = mTab.getValue("C_BPartner_ID");
        var asOnDate = mTab.getValue("AsOnDate");
        //asOnDate = VIS.DB.to_date(asOnDate, true);
        var countVA027 = false;
        // Added by Bharat on 17/May/2017 to remove client side queries..
        try {
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", "VA027_");
            if (dr != null) {
                countVA027 = dr["VA027_"];
            }

            var paramString = countVA027.toString() + "," + bp_BusinessPartner.toString() + "," + asOnDate.toString();
            dr = VIS.dataContext.getJSONRecord("MPayment/GetOutStandingAmt", paramString);
            if (dr != null) {
                var amounts = Util.getValueOfString(dr);
                mTab.setValue("OutStandingAmount", amounts);
            }
        }
        catch (err) {
            //if (idr != null) {
            //    idr.close();
            //    idr = null;
            //}
            //this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err.message;
        }
        this.setCalloutActive(false);
        return "";
    };
    // End 22/2/2017 Manish


    /// <summary>
    /// Payment_Amounts.
    /// Change of:
    /// - IsOverUnderPayment -> set OverUnderAmt to 0
    /// - C_Currency_ID, C_ConvesionRate_ID -> convert all
    /// - PayAmt, DiscountAmt, WriteOffAmt, OverUnderAmt -> PayAmt
    /// make sure that add up to InvoiceOpenAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <param name="oldValue">Old Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Amounts = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive())		//	assuming it is resetting value
        {
            return "";
        }
        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        //	New Payment
        if (ctx.getContextAsInt(windowNo, "C_Payment_ID") == 0
            && ctx.getContextAsInt(windowNo, "C_BPartner_ID") == 0
            && C_Invoice_ID == 0) {
            return "";
        }
        var cur = mTab.getValue("C_Currency_ID");
        if (cur == null) {
            return "";
        }
        // added by Bharat as discussed with Vivek
        if (C_Invoice_ID != 0) {
            this.setCalloutActive(true);

            //	Changed Column
            var colName = mField.getColumnName();

            var C_InvoicePaySchedule_ID = 0;
            if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
                && mTab.getValue("C_InvoicePaySchedule_ID") != null) {
                C_InvoicePaySchedule_ID = mTab.getValue("C_InvoicePaySchedule_ID");
            }
            //	Get Open Amount & Invoice Currency
            var invoiceOpenAmt = VIS.Env.ZERO;
            var discountAmt = VIS.Env.ZERO;
            var C_Currency_Invoice_ID = 0, conversionType_Invioce_ID = 0;
            var IsReturnTrx = "N";
            var checkPrecision = true;
            if (C_Invoice_ID != 0) {
                var ts = mTab.getValue("DateTrx");
                if (ts == null) {
                    //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
                    ts = new Date();
                }

                // Added by Bharat on 18 May 2017 to remove client side queries
                var paramString = C_Invoice_ID.toString() + "," + C_InvoicePaySchedule_ID.toString() + "," + ts.toString();
                var dr = VIS.dataContext.getJSONRecord("MPayment/GetInvoiceData", paramString);
                if (dr != null) {
                    C_Currency_Invoice_ID = Util.getValueOfInt(dr["C_Currency_ID"]);
                    conversionType_Invioce_ID = Util.getValueOfInt(dr["C_ConversionType_ID"]);
                    invoiceOpenAmt = Util.getValueOfDecimal(dr["invoiceOpen"]);
                    if (invoiceOpenAmt == null) {
                        invoiceOpenAmt = VIS.Env.ZERO;
                    }
                    discountAmt = Util.getValueOfDecimal(dr["invoiceDiscount"]);
                    IsReturnTrx = dr["IsReturnTrx"];
                }
            }	//	get Invoice Info

            this.log.fine("Open=" + invoiceOpenAmt + " Discount= " + discountAmt
                + ", C_Invoice_ID=" + C_Invoice_ID
                + ", C_Currency_ID=" + C_Currency_Invoice_ID);

            //	Get Info from Tab
            var payAmt = Util.getValueOfDecimal(mTab.getValue("PayAmt") == null ? VIS.Env.ZERO : mTab.getValue("PayAmt"));
            var writeOffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt") == null ? VIS.Env.ZERO : mTab.getValue("WriteOffAmt"));
            var overUnderAmt = Util.getValueOfDecimal((mTab.getValue("OverUnderAmt") == null ? VIS.Env.ZERO : mTab.getValue("OverUnderAmt")));
            var enteredDiscountAmt = Util.getValueOfDecimal((mTab.getValue("DiscountAmt") == null ? VIS.Env.ZERO : mTab.getValue("DiscountAmt")));
            if (IsReturnTrx == "Y") {
                if (payAmt > 0) {
                    payAmt = payAmt * -1;
                    mTab.setValue("PayAmt", payAmt);
                }
                if (enteredDiscountAmt > 0) {
                    enteredDiscountAmt = enteredDiscountAmt * -1;
                }
                if (discountAmt > 0) {
                    discountAmt = discountAmt * -1;
                }
                if (writeOffAmt > 0) {
                    writeOffAmt = writeOffAmt * -1;
                    mTab.setValue("WriteOffAmt", writeOffAmt);
                }
                if (overUnderAmt > 0) {
                    overUnderAmt = overUnderAmt * -1;
                    mTab.setValue("OverUnderAmt", overUnderAmt);
                }
                if (invoiceOpenAmt > 0) {
                    invoiceOpenAmt = invoiceOpenAmt * -1;
                }
            }
            else {
                if (payAmt < 0) {
                    payAmt = payAmt * -1;
                    mTab.setValue("PayAmt", payAmt);
                }
                if (enteredDiscountAmt < 0) {
                    enteredDiscountAmt = enteredDiscountAmt * -1;
                }
                if (discountAmt < 0) {
                    discountAmt = discountAmt * -1;
                }
                if (writeOffAmt < 0) {
                    writeOffAmt = writeOffAmt * -1;
                    mTab.setValue("WriteOffAmt", writeOffAmt);
                }
                if (overUnderAmt < 0) {
                    overUnderAmt = overUnderAmt * -1;
                    mTab.setValue("OverUnderAmt", overUnderAmt);
                }
                if (invoiceOpenAmt < 0) {
                    invoiceOpenAmt = invoiceOpenAmt * -1;
                }
            }
            this.log.fine("Pay=" + payAmt + ", Discount=" + enteredDiscountAmt
                + ", WriteOff=" + writeOffAmt + ", OverUnderAmt=" + overUnderAmt);
            //	Get Currency Info
            var C_Currency_ID = Util.getValueOfInt(cur);
            var paramString = C_Currency_ID.toString();
            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
            var precision = currency["StdPrecision"];
            //MCurrency currency = MCurrency.Get(ctx, C_Currency_ID);
            var ConvDate = mTab.getValue("DateAcct");
            var C_ConversionType_ID = 0;
            var ii = Util.getValueOfInt(mTab.getValue("C_ConversionType_ID"));
            if (ii != null) {
                C_ConversionType_ID = ii;
            }
            var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID");
            var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
            //	Get Currency Rate
            var currencyRate = VIS.Env.ONE;
            if ((C_Currency_ID > 0 && C_Currency_Invoice_ID > 0 && C_Currency_ID != C_Currency_Invoice_ID) || colName == "C_Currency_ID" || colName == "C_ConversionType_ID") {
                this.log.fine("InvCurrency=" + C_Currency_Invoice_ID
                    + ", PayCurrency=" + C_Currency_ID
                    + ", Date=" + ConvDate + ", Type=" + C_ConversionType_ID);


                var paramStr = C_Currency_Invoice_ID + "," + C_Currency_ID + "," + ConvDate + "," + C_ConversionType_ID + "," + AD_Client_ID + "," + AD_Org_ID;
                currencyRate = VIS.dataContext.getJSONRecord("MConversionRate/GetRate", paramStr);

                //currencyRate = MConversionRate.GetRate(C_Currency_Invoice_ID, C_Currency_ID,
                //    ConvDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
                if (currencyRate == null || currencyRate.toString() == 0) {
                    //	 mTab.setValue("C_Currency_ID", new int(C_Currency_Invoice_ID));	//	does not work
                    this.setCalloutActive(false);
                    if (C_Currency_Invoice_ID == 0) {
                        return "";		//	no error message when no invoice is selected
                    }
                    mTab.setValue("C_Currency_ID", C_Currency_Invoice_ID);

                    // Set Invoice Currency and Currency Type if Currency Conversion not available
                    if (colName == "C_ConversionType_ID") {
                        if (conversionType_Invioce_ID == 0) {
                            return "";
                        }
                        mTab.setValue("C_ConversionType_ID", conversionType_Invioce_ID);
                    }
                    return "NoCurrencyConversion";
                }
                //
                invoiceOpenAmt = Util.getValueOfDecimal((invoiceOpenAmt * currencyRate).toFixed(currency["StdPrecision"]));//, MidpointRounding.AwayFromZero);
                if (enteredDiscountAmt != 0)
                    discountAmt = enteredDiscountAmt;
                discountAmt = Util.getValueOfDecimal((discountAmt * currencyRate).toFixed(currency["StdPrecision"]));
                //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
                this.log.fine("Rate=" + currencyRate + ", InvoiceOpenAmt=" + invoiceOpenAmt + ", DiscountAmt=" + discountAmt);
            }

            //	Currency Changed - convert all
            if (colName == "C_Currency_ID" || colName == "C_ConversionType_ID") {

                //writeOffAmt = (writeOffAmt * currencyRate).toFixed(currency["StdPrecision"]);
                writeOffAmt = 0;
                //  currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
                mTab.setValue("WriteOffAmt", writeOffAmt);
                //overUnderAmt = (overUnderAmt * currencyRate).toFixed(currency["StdPrecision"]);
                overUnderAmt = 0;
                //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
                mTab.setValue("OverUnderAmt", overUnderAmt);

                // nnayak - Entered Discount amount should be converted to entered currency 
                //enteredDiscountAmt = (enteredDiscountAmt * currencyRate).toFixed(currency["StdPrecision"]);
                enteredDiscountAmt = 0;
                //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
                mTab.setValue("DiscountAmt", enteredDiscountAmt);
                discountAmt = enteredDiscountAmt;
                payAmt = (((invoiceOpenAmt - discountAmt) - writeOffAmt) - overUnderAmt);
                mTab.setValue("PayAmt", Util.getValueOfDecimal(payAmt.toFixed(precision)));
            }

            //	No Invoice - Set Discount, Witeoff, Under/Over to 0
            else if (C_Invoice_ID == 0) {
                if (VIS.Env.ZERO != discountAmt) {
                    mTab.setValue("DiscountAmt", VIS.Env.ZERO);
                }
                if (VIS.Env.ZERO != writeOffAmt) {
                    mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
                }
                if (VIS.Env.ZERO != overUnderAmt) {
                    mTab.setValue("OverUnderAmt", VIS.Env.ZERO);
                }
            }
            //  PayAmt - calculate write off
            else if (colName == "PayAmt") {
                if (enteredDiscountAmt != 0)
                    discountAmt = enteredDiscountAmt;
                overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
                //if (VIS.Env.ZERO.compareTo(overUnderAmt) > 0) {
                //if (Math.abs(writeOffAmt).compareTo(discountAmt) <= 0) {
                //    discountAmt = (discountAmt + overUnderAmt);
                //}
                //else {
                //    discountAmt = VIS.Env.ZERO;
                //}  
                //overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
                //}
                // --------Commented as discussed with Mukesh Sir and Mandeep sir on 14 June 2017---------
                if ((IsReturnTrx == "N" && overUnderAmt > 0) || (IsReturnTrx == "Y" && overUnderAmt < 0)) {
                    VIS.ADialog.info("LessScheduleAmount");
                }
                if ((IsReturnTrx == "Y" && overUnderAmt > 0) || (IsReturnTrx == "N" && overUnderAmt < 0)) {
                    VIS.ADialog.info("MoreScheduleAmount");
                    payAmt = ((invoiceOpenAmt - discountAmt) - writeOffAmt);
                    overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
                    if (checkPrecision) {
                        mTab.setValue("PayAmt", Util.getValueOfDecimal(payAmt.toFixed(precision)));
                    }
                    else {
                        mTab.setValue("PayAmt", payAmt);
                    }
                }
                if (checkPrecision) {
                    mTab.setValue("DiscountAmt", Util.getValueOfDecimal(discountAmt.toFixed(precision)));
                    mTab.setValue("OverUnderAmt", Util.getValueOfDecimal(overUnderAmt.toFixed(precision)));
                }
                else {
                    mTab.setValue("DiscountAmt", discountAmt);
                    mTab.setValue("OverUnderAmt", overUnderAmt);
                }
            }
            else    //  calculate PayAmt
            {
                /* nnayak - Allow reduction in discount, but not an increase. To give a discount that is higher
                   than the calculated discount, users have to enter a write off */
                //Source code modified by Suganthi for Allowing positive and negative discount in order
                //if(EnteredDiscountAmt.compareTo(DiscountAmt)<0)
                discountAmt = enteredDiscountAmt;
                payAmt = (((invoiceOpenAmt - discountAmt) - writeOffAmt) - overUnderAmt);

                if ((IsReturnTrx == "Y" && payAmt > 0) || (IsReturnTrx == "N" && payAmt < 0)) {
                    VIS.ADialog.info("MoreScheduleAmount");
                    if (colName == "OverUnderAmt") {
                        payAmt = ((invoiceOpenAmt - discountAmt) - writeOffAmt);
                        overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
                    }
                    if (colName == "DiscountAmt") {
                        payAmt = ((invoiceOpenAmt - overUnderAmt) - writeOffAmt);
                        discountAmt = (((invoiceOpenAmt - payAmt) - overUnderAmt) - writeOffAmt);
                    }
                    if (colName == "WriteOffAmt") {
                        payAmt = ((invoiceOpenAmt - discountAmt) - overUnderAmt);
                        writeOffAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - overUnderAmt);
                    }
                }
                if (checkPrecision) {
                    mTab.setValue("PayAmt", Util.getValueOfDecimal(payAmt.toFixed(precision)));
                    mTab.setValue("DiscountAmt", Util.getValueOfDecimal(discountAmt.toFixed(precision)));
                    mTab.setValue("WriteOffAmt", Util.getValueOfDecimal(writeOffAmt.toFixed(precision)));
                    mTab.setValue("OverUnderAmt", Util.getValueOfDecimal(overUnderAmt.toFixed(precision)));
                }
                else {
                    mTab.setValue("PayAmt", payAmt);
                    mTab.setValue("DiscountAmt", discountAmt);
                    mTab.setValue("WriteOffAmt", writeOffAmt);
                    mTab.setValue("OverUnderAmt", overUnderAmt);
                }
                if ((IsReturnTrx == "N" && overUnderAmt > 0) || (IsReturnTrx == "Y" && overUnderAmt < 0)) {
                    VIS.ADialog.info("LessScheduleAmount");
                }
            }
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     * Get Bank Account Curremcy
     * @param ctx context
     * @param windowNo current Window No
     * @param mTab Grid Tab
     * @param mField Grid Field
     * @param value New Value
     * @param oldValue Old Value
     * @return Error message or ""
     */
    CalloutPayment.prototype.BankAccount = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }

        var c_bankaccount_ID = value;
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var currency = Util.getValueOfInt(VIS.dataContext.getJSONRecord("MPayment/GetBankAcctCurrency", c_bankaccount_ID.toString()));
            mTab.setValue("C_Currency_ID", currency);
        }
        catch (err) {
            this.setCalloutActive(false);
            return err.message;
        }
        this.setCalloutActive(false);
        ctx = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutPayment = CalloutPayment;
    //*********** CalloutPayment End ******


    //************CalloutPaymentAllocate Start ********
    function CalloutPaymentAllocate() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaymentAllocate"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPaymentAllocate, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Payment_Invoice.
    /// when Invoice selected
    /// - set InvoiceAmt = invoiceOpen
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - Amount = invoiceOpen (ID) - Discount
    /// - WriteOffAmt,OverUnderAmt = 0
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutPaymentAllocate.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        // 
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Invoice_ID = Util.getValueOfInt(value);
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Invoice_ID == null || C_Invoice_ID == 0) {
            return "";
        }

        //	Check Payment
        var C_Payment_ID = ctx.getContextAsInt(windowNo, "C_Payment_ID");

        var paramString = C_Payment_ID.toString();

        //Get product price information
        var payment = null;
        payment = VIS.dataContext.getJSONRecord("MPayment/GetPayment", paramString);



        // MPayment payment = new MPayment(ctx, C_Payment_ID, null);
        if (payment["C_Charge_ID"] != 0 || payment["C_Invoice_ID"] != 0
            || payment["C_Order_ID"] != 0)
            return VIS.Msg.getMsg("PaymentIsAllocated");

        this.setCalloutActive(true);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        var C_InvoicePaySchedule_ID = 0;
        if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
            && ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID") != 0) {
            C_InvoicePaySchedule_ID = ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID");
        }

        var dueAmount = 0;
        var _chk = 0;

        // Added by Bharat on 28 June 2017

        var drAmt = null;
        try {
            drAmt = VIS.dataContext.getJSONRecord("MInvoice/GetInvPaySchedDetail", C_Invoice_ID);
            if (drAmt != null) {
                C_InvoicePaySchedule_ID = Util.getValueOfInt(drAmt["C_InvoicePaySchedule_ID"]);
                dueAmount = Util.getValueOfDecimal(drAmt["dueAmount"]);
                IsReturnTrx = drAmt["IsReturnTrx"];
                if ("Y" == IsReturnTrx) {
                    dueAmount = (dueAmount) * (-1);
                }
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
                mTab.setValue("Amount", dueAmount);
                _chk = 1;
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, "PaymentAllocate.Invoice -" + C_Invoice_ID, err.message);
            return err.toString();
        }
        // Bharat
        //  Payment Date
        var ts = Util.getValueOfDate(ctx.getContextAsTime(windowNo, "DateTrx"));
        if (ts == null) {
            //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
            ts = new Date();
        }

        var paramString = C_Invoice_ID.toString() + "," + C_InvoicePaySchedule_ID.toString() + "," + ts.toString();
        var dr = null;
        try {
            dr = VIS.dataContext.getJSONRecord("MPayment/GetInvoiceData", paramString);
            if (dr != null) {
                var invoiceOpen = Util.getValueOfDecimal(dr["invoiceOpen"]);            		//	Set Invoice OPen Amount
                if (invoiceOpen == null) {
                    invoiceOpen = VIS.Env.ZERO;
                }
                var discountAmt = Util.getValueOfDecimal(dr["invoiceDiscount"]);        		//	Set Discount Amt
                if (discountAmt == null) {
                    discountAmt = VIS.Env.ZERO;
                }
                mTab.setValue("InvoiceAmt", invoiceOpen);
                if (_chk == 0) {
                    mTab.setValue("Amount", (invoiceOpen - discountAmt));
                }
                mTab.setValue("DiscountAmt", discountAmt);
                //  reset as dependent fields get reset
                ctx.setContext(windowNo, "C_Invoice_ID", C_Invoice_ID.toString());
            }

            //    idr.close();
        }
        catch (err) {
            //if (idr != null) {
            //    idr.close();
            //    idr = null;
            //}
            //this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Payment_Amounts.
    /// Change of:
    /// - IsOverUnderPayment -> set OverUnderAmt to 0
    /// - C_Currency_ID, C_ConvesionRate_ID -> convert all
    /// - PayAmt, DiscountAmt, WriteOffAmt, OverUnderAmt -> PayAmt
    /// make sure that add up to InvoiceOpenAmt
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <param name="oldValue"></param>
    /// <returns></returns>
    CalloutPaymentAllocate.prototype.Amounts = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        //if (value == DBNull.Value || value == null || value.toString() == "")
        //{
        //    return "";
        //}
        if (this.isCalloutActive())		//	assuming it is resetting value
        {
            return "";
        }
        //	No Invoice
        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        if (C_Invoice_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        //	Get Info from Tab
        // Added by Bharat on 23 June 2017
        var C_InvoicePaySchedule_ID = 0;
        if (mTab.getValue("C_InvoicePaySchedule_ID") != null) {
            C_InvoicePaySchedule_ID = mTab.getValue("C_InvoicePaySchedule_ID");
        }
        var invoiceOpenAmt = VIS.Env.ZERO;
        var IsReturnTrx = "N";
        var C_Currency_Invoice_ID = 0;
        var checkPrecision = true;
        var amount = Util.getValueOfDecimal(mTab.getValue("Amount"));
        var discountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt"));
        var writeOffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt"));
        var overUnderAmt = Util.getValueOfDecimal(mTab.getValue("OverUnderAmt"));
        var invoiceAmt = Util.getValueOfDecimal(mTab.getValue("InvoiceAmt"));
        this.log.fine("Amt=" + amount + ", Discount=" + discountAmt
            + ", WriteOff=" + writeOffAmt + ", OverUnder=" + overUnderAmt
            + ", Invoice=" + invoiceAmt);

        // Added by Bharat on 23 june 2017 to remove client side queries
        var ts = ctx.getContext(windowNo, "DateTrx");
        if (ts == null) {
            ts = new Date();
        }
        var paramString = C_Invoice_ID.toString() + "," + C_InvoicePaySchedule_ID.toString() + "," + ts.toString();
        var dr = VIS.dataContext.getJSONRecord("MPayment/GetInvoiceData", paramString);
        if (dr != null) {
            C_Currency_Invoice_ID = Util.getValueOfInt(dr["C_Currency_ID"]);
            invoiceOpenAmt = Util.getValueOfDecimal(dr["invoiceOpen"]);
            if (invoiceOpenAmt == null) {
                invoiceOpenAmt = VIS.Env.ZERO;
            }
            if (VIS.Env.ZERO == discountAmt) {
                discountAmt = Util.getValueOfDecimal(dr["invoiceDiscount"]);
            }
            IsReturnTrx = dr["IsReturnTrx"];
        }
        if (IsReturnTrx == "Y") {
            if (amount > 0) {
                amount = amount * -1;
                mTab.setValue("Amount", amount);
            }

            if (discountAmt > 0) {
                discountAmt = discountAmt * -1;
            }

            if (writeOffAmt > 0) {
                writeOffAmt = writeOffAmt * -1;
                mTab.setValue("WriteOffAmt", writeOffAmt);
            }

            if (overUnderAmt > 0) {
                overUnderAmt = overUnderAmt * -1;
                mTab.setValue("OverUnderAmt", overUnderAmt);
            }
            if (invoiceOpenAmt > 0) {
                invoiceOpenAmt = invoiceOpenAmt * -1;
            }
        }
        else {
            if (amount < 0) {
                amount = amount * -1;
                mTab.setValue("Amount", amount);
            }

            if (discountAmt < 0) {
                discountAmt = discountAmt * -1;
            }
            if (writeOffAmt < 0) {
                writeOffAmt = writeOffAmt * -1;
                mTab.setValue("WriteOffAmt", writeOffAmt);
            }
            if (overUnderAmt < 0) {
                overUnderAmt = overUnderAmt * -1;
                mTab.setValue("OverUnderAmt", overUnderAmt);
            }
            if (invoiceOpenAmt < 0) {
                invoiceOpenAmt = invoiceOpenAmt * -1;
            }
        }
        //	Changed Column
        var colName = mField.getColumnName();
        paramString = C_Currency_Invoice_ID.toString();
        var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
        var precision = currency["StdPrecision"];
        //  PayAmt - calculate write off
        if (colName == "Amount") {
            //writeOffAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, amount), discountAmt), overUnderAmt);
            //writeOffAmt = (((invoiceAmt - amount) - discountAmt) - overUnderAmt);
            overUnderAmt = (((invoiceOpenAmt - amount) - discountAmt) - writeOffAmt);
            //mTab.setValue("WriteOffAmt", writeOffAmt);
            if ((IsReturnTrx == "N" && overUnderAmt > 0) || (IsReturnTrx == "Y" && overUnderAmt < 0)) {
                VIS.ADialog.info("LessScheduleAmount");
            }
            if ((IsReturnTrx == "Y" && overUnderAmt > 0) || (IsReturnTrx == "N" && overUnderAmt < 0)) {
                VIS.ADialog.info("MoreScheduleAmount");
                amount = ((invoiceOpenAmt - discountAmt) - writeOffAmt);
                overUnderAmt = (((invoiceOpenAmt - amount) - discountAmt) - writeOffAmt);
                if (checkPrecision) {
                    mTab.setValue("Amount", amount.toFixed(precision));
                }
                else {
                    mTab.setValue("Amount", amount);
                }
            }
            if (checkPrecision) {
                mTab.setValue("DiscountAmt", discountAmt.toFixed(precision));
                mTab.setValue("OverUnderAmt", overUnderAmt.toFixed(precision));
            }
            else {
                mTab.setValue("DiscountAmt", discountAmt);
                mTab.setValue("OverUnderAmt", overUnderAmt);
            }
        }
        else    //  calculate Amount
        {
            //amount = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, discountAmt), writeOffAmt), overUnderAmt);
            amount = (((invoiceOpenAmt - discountAmt) - writeOffAmt) - overUnderAmt);

            if ((IsReturnTrx == "Y" && amount > 0) || (IsReturnTrx == "N" && amount < 0)) {
                VIS.ADialog.info("MoreScheduleAmount");
                if (colName == "OverUnderAmt") {
                    amount = ((invoiceOpenAmt - discountAmt) - writeOffAmt);
                    overUnderAmt = (((invoiceOpenAmt - amount) - discountAmt) - writeOffAmt);
                }
                if (colName == "DiscountAmt") {
                    amount = ((invoiceOpenAmt - overUnderAmt) - writeOffAmt);
                    discountAmt = (((invoiceOpenAmt - amount) - overUnderAmt) - writeOffAmt);
                }
                if (colName == "WriteOffAmt") {
                    amount = ((invoiceOpenAmt - discountAmt) - overUnderAmt);
                    writeOffAmt = (((invoiceOpenAmt - amount) - discountAmt) - overUnderAmt);
                }
            }
            if (checkPrecision) {
                mTab.setValue("Amount", amount.toFixed(precision));
                mTab.setValue("DiscountAmt", discountAmt.toFixed(precision));
                mTab.setValue("WriteOffAmt", writeOffAmt.toFixed(precision));
                mTab.setValue("OverUnderAmt", overUnderAmt.toFixed(precision));
            }
            else {
                mTab.setValue("Amount", amount);
                mTab.setValue("DiscountAmt", discountAmt);
                mTab.setValue("WriteOffAmt", writeOffAmt);
                mTab.setValue("OverUnderAmt", overUnderAmt);
            }
            if ((IsReturnTrx == "N" && overUnderAmt > 0) || (IsReturnTrx == "Y" && overUnderAmt < 0)) {
                VIS.ADialog.info("LessScheduleAmount");
            }
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPaymentAllocate = CalloutPaymentAllocate;
    //************ CalloutPaymentAllocate End *********

    //*************CalloutPaymentTypeSO Starts***********
    function CalloutPaymentTypeSO() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaymentTypeSO");//must call
    };
    VIS.Utility.inheritPrototype(CalloutPaymentTypeSO, VIS.CalloutEngine); //inherit prototype


    CalloutPaymentTypeSO.prototype.SetPaymentType = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (mField.getColumnName() == "PaymentRule") {

                if (mTab.getValue("PaymentRule") == null) {
                    mTab.setValue("PaymentMethod", null);
                }
                else {
                    var _PaymentMethod_ID = mTab.getValue("PaymentRule");
                    mTab.setValue("PaymentMethod", _PaymentMethod_ID);
                }

            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutPaymentTypeSO = CalloutPaymentTypeSO;
    //*************CalloutPaymentTypeSO Ends*************

})(VIS, jQuery);