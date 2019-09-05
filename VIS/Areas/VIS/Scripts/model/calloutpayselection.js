; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //************ CalloutPaySelection Start ***
    function CalloutPaySelection() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaySelection"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPaySelection, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Payment Selection Line - Payment Amount.
    /// - called from C_PaySelectionLine.PayAmt
    /// - update DifferenceAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutPaySelection.prototype.PayAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (this.isCalloutActive() || value == null || value == "") {
            return "";
        }
        //	get invoice info
        var ii = Util.getValueOfInt(mTab.getValue("C_Invoice_ID"));
        if (ii == null) {
            return "";
        }
        var C_Invoice_ID = ii;// ii.intValue();
        if (C_Invoice_ID == 0) {
            return "";
        }
        //
        var OpenAmt = Util.getValueOfDecimal(mTab.getValue("OpenAmt"));
        var PayAmt = Util.getValueOfDecimal(mTab.getValue("PayAmt"));
        var DiscountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt"));
        this.setCalloutActive(true);
        // var DifferenceAmt = Decimal.Subtract(Decimal.Subtract(OpenAmt, PayAmt), DiscountAmt);

        var DifferenceAmt = ((OpenAmt - PayAmt) - DiscountAmt);


        this.log.fine(" - OpenAmt=" + OpenAmt + " - PayAmt=" + PayAmt
            + ", Discount=" + DiscountAmt + ", Difference=" + DifferenceAmt);

        mTab.setValue("DifferenceAmt", DifferenceAmt);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Payment Selection Line - Invoice.
    /// - called from C_PaySelectionLine.C_Invoice_ID
    /// - update PayAmt & DifferenceAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutPaySelection.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value == "") {
            return "";
        }
        //	get value
        var C_Invoice_ID = Util.getValueOfInt(value);
        if (C_Invoice_ID == 0) {
            return "";
        }
        var C_BankAccount_ID = ctx.getContextAsInt(windowNo, "C_BankAccount_ID");
        //var PayDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime("PayDate"));// new DateTime(ctx.getContextAsTime("PayDate"));

        var PayDate = ctx.getContext("PayDate");
        this.setCalloutActive(true);

        var OpenAmt = VIS.Env.ZERO;


        var DiscountAmt = VIS.Env.ZERO;
        var isSOTrx = false;//            Boolean.FALSE;
        var sql = "SELECT currencyConvert(invoiceOpen(i.C_Invoice_ID, 0), i.C_Currency_ID,"
                + "ba.C_Currency_ID, i.DateInvoiced, i.C_ConversionType_ID, i.AD_Client_ID, i.AD_Org_ID) as OpenAmt,"
            + " paymentTermDiscount(i.GrandTotal,i.C_Currency_ID,i.C_PaymentTerm_ID,i.DateInvoiced,'" + PayDate + "') As DiscountAmt, i.IsSOTrx "
            + "FROM C_Invoice_v i, C_BankAccount ba "
            + "WHERE i.C_Invoice_ID=@Param2 AND ba.C_BankAccount_ID=@Param3";	//	#1..2
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[3];
        var idr = null;
        try {
            //Param[0] = new VIS.DB.SqlParam("@Param1", PayDate);  //Temporary Commented By Sarab
            Param[0] = new VIS.DB.SqlParam("@Param2", C_Invoice_ID);
            Param[1] = new VIS.DB.SqlParam("@Param3", C_BankAccount_ID);
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                OpenAmt = idr.get("openamt");
                DiscountAmt = idr.get("discountamt");
                IsSOTrx = "Y" == idr.get("issotrx");
            }
            idr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
        }

        this.log.fine(" - OpenAmt=" + OpenAmt + " (Invoice=" + C_Invoice_ID + ",BankAcct=" + C_BankAccount_ID + ")");
        mTab.setValue("OpenAmt", OpenAmt);
        mTab.setValue("PayAmt", (OpenAmt - DiscountAmt));
        mTab.setValue("DiscountAmt", DiscountAmt);
        mTab.setValue("DifferenceAmt", VIS.Env.ZERO);
        mTab.setValue("IsSOTrx", isSOTrx ? "Y" : "N");

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPaySelection = CalloutPaySelection;
    //************ CalloutPaySelection End ******

})(VIS, jQuery);