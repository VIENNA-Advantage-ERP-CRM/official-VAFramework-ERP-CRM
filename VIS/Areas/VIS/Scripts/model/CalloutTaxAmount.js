; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;

    //**************CalloutTaxAmount Start*********************
    //Made By Mohit 24.09/2015

    function CalloutTaxAmount() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTaxAmount");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTaxAmount, VIS.CalloutEngine);//inherit prototype
    // Method on payment window
    CalloutTaxAmount.prototype.PaymentTaxAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            // get precision from currency
            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", mTab.getValue("C_Currency_ID").toString());
            var StdPrecision = currency["StdPrecision"];

            if (mField.getColumnName() == "C_Tax_ID") {
                if (Util.getValueOfDecimal(mTab.getValue("PayAmt")) > 0) {
                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", value.toString() + "," + mTab.getValue("PayAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmount", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", value.toString());
                        var Rate = Util.getValueOfDecimal(dr);

                        if (Rate > 0) {

                            //Formula for caluculating Tax amount ==>  Amount - Amount / ((Rate / 100) + 1)
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("PayAmt")) - (Util.getValueOfDecimal(mTab.getValue("PayAmt")) / ((Rate / 100) + 1)));
                            // Round the amount according to currency precision
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));
                            mTab.setValue("TaxAmount", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmount", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }
            }
            else {
                if (Util.getValueOfInt(mTab.getValue("C_Tax_ID")) > 0) {
                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", mTab.getValue("C_Tax_ID").toString() + "," + mTab.getValue("PayAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmount", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", mTab.getValue("C_Tax_ID").toString());
                        var Rate = Util.getValueOfDecimal(dr);
                        if (Rate > 0) {

                            //Formula for caluculating Tax amount ==>  Amount - Amount / ((Rate / 100) + 1)
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("PayAmt")) - (Util.getValueOfDecimal(mTab.getValue("PayAmt")) / ((Rate / 100) + 1)));

                            // Round the amount according to currency precision                        
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));

                            mTab.setValue("TaxAmount", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmount", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }


            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    // Method on Cash Journal
    CalloutTaxAmount.prototype.CashLineTaxAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            // get precision from currency
            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", ctx.getContext(windowNo, "C_Currency_ID").toString());
            var StdPrecision = currency["StdPrecision"];

            if (mField.getColumnName() == "C_Tax_ID") {
                if (Util.getValueOfDecimal(mTab.getValue("Amount")) != 0) {

                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", value.toString() + "," + mTab.getValue("Amount").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmt", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", value.toString());
                        var Rate = Util.getValueOfDecimal(dr);

                        if (Rate > 0) {

                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("Amount")) - (Util.getValueOfDecimal(mTab.getValue("Amount")) / ((Rate / 100) + 1)));

                            // JID_1037: On cash line tax amount field was not working accordingling to currency precision
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));

                            mTab.setValue("TaxAmt", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmt", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }
            }
            else {
                if (Util.getValueOfInt(mTab.getValue("C_Tax_ID")) > 0) {

                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", mTab.getValue("C_Tax_ID").toString() + "," + mTab.getValue("Amount").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmt", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", mTab.getValue("C_Tax_ID").toString());
                        var Rate = Util.getValueOfDecimal(dr);


                        if (Rate > 0) {

                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("Amount")) - (Util.getValueOfDecimal(mTab.getValue("Amount")) / ((Rate / 100) + 1)));

                            // JID_1037: On cash line tax amount field was not working accordingling to currency precision
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));

                            mTab.setValue("TaxAmt", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmt", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }


            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    // Method on Bank Statement Line
    CalloutTaxAmount.prototype.BankStatementLineTaxAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var paramString = "";
        try {

            // Need to round Tax Amount according to Currency Precision
            paramString = mTab.getValue("C_Currency_ID").toString();
            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
            var StdPrecision = currency["StdPrecision"];

            if (mField.getColumnName() == "C_Tax_ID") {
                if (Util.getValueOfDecimal(mTab.getValue("ChargeAmt")) != 0) {

                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", value.toString() + "," + mTab.getValue("ChargeAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmt", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", value.toString());
                        var Rate = Util.getValueOfDecimal(dr);
                        if (Rate > 0) {
                            //var TaxAmt = Util.getValueOfDecimal((Rate * Util.getValueOfDecimal(mTab.getValue("ChargeAmt"))) / 100);

                            // Change by amit 6-11-2015
                            //Formula for caluculating Tax amount ==>  Amount - Amount / ((Rate / 100) + 1)
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("ChargeAmt")) - (Util.getValueOfDecimal(mTab.getValue("ChargeAmt")) / ((Rate / 100) + 1)));

                            // JID_0333: Need to round Tax Amount according to Currency Precision
                            paramString = mTab.getValue("C_Currency_ID").toString();
                            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
                            var StdPrecision = currency["StdPrecision"];
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));
                            mTab.setValue("TaxAmt", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmt", 0);
                            this.setCalloutActive(false);
                            ctx = windowNo = mTab = mField = value = oldValue = null;
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    ctx = windowNo = mTab = mField = value = oldValue = null;
                    return "";
                }
            }
            else {
                if (Util.getValueOfInt(mTab.getValue("C_Tax_ID")) > 0) {
                    var dr = null;
                    if (mTab.getField("SurchargeAmt") != null) {
                        // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", mTab.getValue("C_Tax_ID").toString() + "," + mTab.getValue("ChargeAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmt", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        dr = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", mTab.getValue("C_Tax_ID").toString());
                        var Rate = Util.getValueOfDecimal(dr);

                        if (Rate > 0) {
                            //var TaxAmt = Util.getValueOfDecimal((Rate * Util.getValueOfDecimal(mTab.getValue("ChargeAmt"))) / 100);

                            // Change by amit 6-11-2015
                            //Formula for caluculating Tax amount ==>  Amount - Amount / ((Rate / 100) + 1)
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("ChargeAmt")) - (Util.getValueOfDecimal(mTab.getValue("ChargeAmt")) / ((Rate / 100) + 1)));

                            // JID_0333: Need to round Tax Amount according to Currency Precision
                            paramString = mTab.getValue("C_Currency_ID").toString();
                            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
                            var StdPrecision = currency["StdPrecision"];
                            TaxAmt = Util.getValueOfDecimal(TaxAmt.toFixed(StdPrecision));
                            mTab.setValue("TaxAmt", TaxAmt);
                        }
                        else {
                            mTab.setValue("TaxAmt", 0);
                            this.setCalloutActive(false);
                            ctx = windowNo = mTab = mField = value = oldValue = null;
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    ctx = windowNo = mTab = mField = value = oldValue = null;
                    return "";
                }


            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutTaxAmount = CalloutTaxAmount;

    //**************CalloutTaxAmount End*********************

})(VIS, jQuery);