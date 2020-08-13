; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //**********CalloutGLJournal Start*********

    function CalloutGLJournal() {
        VIS.CalloutEngine.call(this, "VIS.CalloutGLJournal");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutGLJournal, VIS.CalloutEngine); //inherit calloutengine

    /// <summary>
    /// Journal - Period.
    //  Check that selected period is in DateAcct Range or Adjusting Period
    //  Called when C_Period_ID or DateAcct, DateDoc changed
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">fields</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutGLJournal.prototype.Period = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        //  
        var colName = mField.getColumnName();

        this.setCalloutActive(true);

        var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID", false);
        var DateAcct = new Date();
        if (colName == "DateAcct") {
            //DateAcct = Util.getValueOfDateTime(value);
            // DateAcct = Util.getValueOfDateTime(value);
            DateAcct = value;
        }
        else {
            // DateAcct = Util.getValueOfDateTime(mTab.getValue("DateAcct"));
            //DateAcct = Util.getValueOfDateTime(mTab.getValue("DateAcct"));
            DateAcct = mTab.getValue("DateAcct");
        }
        var C_Period_ID = 0;
        if (colName == "C_Period_ID") {
            C_Period_ID = Util.getValueOfInt(value);

        }
        //  When DateDoc is changed, update DateAcct
        if (colName == "DateDoc") {
            mTab.setValue("DateAcct", value);
        }

            //  When DateAcct is changed, set C_Period_ID
        else if (colName == "DateAcct") {
            var sql = "SELECT C_Period_ID "
                + "FROM C_Period "
                + "WHERE C_Year_ID IN "
                + "	(SELECT C_Year_ID FROM C_Year WHERE C_Calendar_ID ="
                + "  (SELECT C_Calendar_ID FROM AD_ClientInfo WHERE AD_Client_ID=@param1))"
                + " AND @param2 BETWEEN StartDate AND EndDate"
                + " AND PeriodType='S'";
            var param = [];
            //SqlParameter[] param = new SqlParameter[2];
            var idr = null;
            try {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, AD_Client_ID);
                //pstmt.setTimestamp(2, DateAcct);
                param[0] = new VIS.DB.SqlParam("@param1", AD_Client_ID);
                param[1] = new VIS.DB.SqlParam("@param2", DateAcct);
                param[1].setIsDate(true);
                idr = VIS.DB.executeReader(sql, param, null);
                if (idr.read()) {
                    C_Period_ID = Util.getValueOfInt(idr.get(0));// rs.getInt(1);
                }
                idr.close();
            }
            catch (err) {
                if (idr != null) {
                    idr.close();
                }
                this.log.log(Level.SEVERE, sql, e);
                this.setCalloutActive(false);
                return err.message;
            }
            if (C_Period_ID != 0) {
                mTab.setValue("C_Period_ID", Util.getValueOfInt(C_Period_ID));
            }
        }
            //  When C_Period_ID is changed, check if in DateAcct range and set to end date if not
        else {
            var sql = "SELECT PeriodType, StartDate, EndDate "
                + "FROM C_Period WHERE C_Period_ID=@param";

            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            var idr = null;
            try {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, C_Period_ID);
                param[0] = new VIS.DB.SqlParam("@param", C_Period_ID);
                idr = VIS.DB.executeReader(sql, param, null);
                if (idr.read()) {
                    var PeriodType = idr.get("periodtype");// rs.getString(1);
                    var StartDate = idr.get("startdate");// rs.getTimestamp(2);
                    var EndDate = idr.get("enddate");// rs.getTimestamp(3);
                    if (PeriodType == "S") //  Standard Periods
                    {
                        //  out of range - set to last day
                        if (DateAcct == null
                            || DateAcct < StartDate || DateAcct > EndDate)
                            mTab.setValue("DateAcct", EndDate);
                    }
                }
                idr.close();
            }
            catch (err) {
                if (idr != null) {
                    idr.close();
                }
                this.log.log(Level.SEVERE, sql, e);
                this.setCalloutActive(false);
                return e.message;
            }
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  	Journal_Period



    /// <summary>
    /// Journal/Line - rate.	Set CurrencyRate from DateAcct, C_ConversionType_ID, C_Currency_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutGLJournal.prototype.Rate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        //  Source info
        try {
            var Currency_ID = Util.getValueOfInt(mTab.getValue("C_Currency_ID"));
            var C_Currency_ID = Util.getValueOfInt(Currency_ID);
            var ConversionType_ID = Util.getValueOfInt(mTab.getValue("C_ConversionType_ID"));
            var C_ConversionType_ID = Util.getValueOfInt(ConversionType_ID);
            var DateAcct = mTab.getValue("DateAcct");
            if (DateAcct == null) {
                var currentDate = new Date();
                //DateAcct = DateTime.Now;//  new Timestamp(System.currentTimeMillis());
                DateAcct = currentDate.toGMTString();
            }
            //
            var C_AcctSchema_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_AcctSchema_ID", false));
            var paramString = C_AcctSchema_ID;
            var aas = VIS.dataContext.getJSONRecord("MAcctSchema/GetAcctSchema", paramString);
            // MAcctSchema aas = MAcctSchema.Get(ctx, C_AcctSchema_ID.Value);
            var AD_Client_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "AD_Client_ID", false));
            var AD_Org_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "AD_Org_ID", false));

            //var CurrencyRate = MConversionRate.GetRate(C_Currency_ID.Value, aas.GetC_Currency_ID(),
            //    DateAcct, C_ConversionType_ID.Value, AD_Client_ID.Value, AD_Org_ID.Value);
            var paramStr = C_Currency_ID + "," + aas["C_Currency_ID"] + "," + DateAcct + "," + C_ConversionType_ID + "," + AD_Client_ID + "," + AD_Org_ID;
            var CurrencyRate = VIS.dataContext.getJSONRecord("MConversionRate/GetRate", paramStr);
            this.log.fine("rate = " + CurrencyRate);
            if (CurrencyRate == null) {
                CurrencyRate = 0;
            }
            mTab.setValue("CurrencyRate", CurrencyRate);
        }
        catch (err) {

            this.log.log(Level.SEVERE, sql, e);
            this.setCalloutActive(false);
            return e.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	rate

    /// <summary>
    /// JournalLine - Amt.  Convert the source amount to accounted amount (AmtAcctDr/Cr)Called when source amount (AmtSourceCr/Dr) or rate changes
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns> null or error message</returns>
    CalloutGLJournal.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        //	String colName = mField.getColumnName();
        if (value == null || value.toString() == "" || this.isCalloutActive()) {
            return "";
        }

        this.setCalloutActive(true);

        //  Get Target Currency & Precision from C_AcctSchema.C_Currency_ID
        var C_AcctSchema_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_AcctSchema_ID"));
        var paramString = C_AcctSchema_ID.toString();
        var aas = VIS.dataContext.getJSONRecord("MAcctSchema/GetAcctSchema", paramString);

        //MAcctSchema aas = MAcctSchema.Get(ctx, C_AcctSchema_ID);
        //var Precision = aas.GetStdPrecision();

        var Precision = aas["StdPrecision"];

        var CurrencyRate = mTab.getValue("CurrencyRate");
        if (CurrencyRate == null) {
            CurrencyRate = 1;
            mTab.setValue("CurrencyRate", CurrencyRate);
        }

        //  AmtAcct = AmtSource * CurrencyRate  ==> Precision
        var AmtSourceDr = mTab.getValue("AmtSourceDr");
        if (AmtSourceDr == null) {
            AmtSourceDr = 0;
        }
        var AmtSourceCr = mTab.getValue("AmtSourceCr");
        if (AmtSourceCr == null) {
            AmtSourceCr = 0;
        }

        // var AmtAcctDr = Decimal.Multiply(AmtSourceDr.Value, CurrencyRate.Value);

        var AmtAcctDr = AmtSourceDr * CurrencyRate;
        //AmtAcctDr = AmtAcctDr.setScale(Precision, BigDecimal.ROUND_HALF_UP);
        //AmtAcctDr = Decimal.Round(AmtAcctDr.Value, Precision);//, MidpointRounding.AwayFromZero);//   BigDecimal.ROUND_HALF_UP);
        AmtAcctDr.toFixed(Precision);
        mTab.setValue("AmtAcctDr", AmtAcctDr);
        //var AmtAcctCr = Decimal.Multiply(AmtSourceCr.Value, CurrencyRate.Value);
        var AmtAcctCr = AmtSourceCr * CurrencyRate;

        // AmtAcctCr = Decimal.Round(AmtAcctCr.Value, Precision);//, MidpointRounding.AwayFromZero);// BigDecimal.ROUND_HALF_UP);
        AmtAcctCr.toFixed(Precision);
        mTab.setValue("AmtAcctCr", AmtAcctCr);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  amt

    //	CalloutGLJournal
    VIS.Model.CalloutGLJournal = CalloutGLJournal;
    //**************CalloutGLJournal End*********


})(VIS, jQuery);