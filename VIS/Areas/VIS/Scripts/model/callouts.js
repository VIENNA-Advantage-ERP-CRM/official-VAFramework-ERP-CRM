/** 
  *    Sample Class for Callout
       -  must call base class (CalloutEngine)
       -- must inheirt Base class
  */


; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;
    //1
    /* Sample Start */


    /**
    *  Callout Class
      -   must call this function
             VIS.CalloutEngine.call(this, [className]);
    */
    function TestClass() {
        VIS.CalloutEngine.call(this, "VIS.TestClass"); //must call
    };
    /**
     * Inherit CallourEngile Class 
     */
    VIS.Utility.inheritPrototype(TestClass, VIS.CalloutEngine);//inherit CalloutEngine


    /**
     *  Callout function
     */
    TestClass.prototype.set = function (ctx, windowNo, mTab, mField, value, oldValue) {
        mTab.setValue("Description", value);
        return "";
    };

    VIS.Model.TestClass = TestClass; //assign object in Model NameSpace

    /* Sample END */





    //*************CalloutObjectData Start******
    function CalloutObjectData() {
        VIS.CalloutEngine.call(this, "VIS.CalloutObjectData"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutObjectData, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutObjectData.prototype.Objectchk = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (value == null) {
            return "";
        }

        var FO_OBJ_DATA_ID = Util.getValueOfInt(mTab.getValue("FO_OBJECT_DATA_ID"));//Object Data ID
        //Integer acc_ID=null; //Accomodation ID
        var oldstartdate;//From Date stored in Database
        var oldtilldate;//Till Date stored in Database
        var todaydate = Util.getValueOfDateTime(mTab.getValue("TODAYDATE"));//Date 
        if (FO_OBJ_DATA_ID == null) {
            return "";
        }
        else {
            var sql = "select FO_RES_ACCOMODATION_ID,DATE_FROM,TILL_DATE from FO_RES_ACCOMODATION where FO_OBJECT_DATA_ID=@FO_OBJ_DATA_ID";
            var dr = null;
            try {
                //SqlParameter[] param = new SqlParameter[1];
                var param = [];
                param[0] = new VIS.DB.SqlParam("@FO_OBJ_DATA_ID", FO_OBJ_DATA_ID);
                dr = VIS.DB.executeReader(sql, param, null);
                //PreparedStatement pst = DataBase.prepareStatement(sql, null);
                //pst.setInt(1, FO_OBJ_DATA_ID);
                //ResultSet rs = pst.executeQuery();
                while (dr.read()) {
                    //	acc_ID= rs.getInt(1);
                    oldstartdate = Util.getValueOfDateTime(dr[1]);
                    oldtilldate = Util.getValueOfDateTime(dr[2]);
                    // curr_date is the current date holding the cursor
                    var curr_date = oldstartdate;
                    //if (todaydate.after(oldtilldate) == true)
                    if (todaydate.compareTo(oldtilldate) > 0) {
                        mTab.setValue("RES_STATUS", false);
                        return "";
                    }
                    //else if (todaydate.before(oldstartdate) == true)
                    else if (todaydate.compareTo(oldstartdate) < 0) {
                        mTab.setValue("RES_STATUS", false);
                        return "";
                    }
                    //else if (todaydate.after(oldstartdate) && todaydate.before(oldtilldate))
                    else if ((todaydate.compareTo(oldstartdate) > 0) && (todaydate.compareTo(oldtilldate) < 0)) {
                        while (!(curr_date.compareTo(oldtilldate) == 0)) {
                            if (todaydate.compareTo(curr_date) == 0) {
                                mTab.setValue("RES_STATUS", true);
                            }
                            //curr_date = TimeUtil.addDays(curr_date, 1);
                            curr_date = curr_date.AddDays(1);
                        }
                    }
                }
                dr.close();
                //pst.close();
            }
            catch (err) {
                this.setCalloutActive(false);
                if (dr != null) {
                    dr.close();
                }
                this.log.severe(e.toString());
            }
            finally {
                if (dr != null) {
                    dr.close();
                }
            }
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutObjectData = CalloutObjectData;
    //*************CalloutObjectData End*********

    //************CalloutOffer Start********
    function CalloutOffer() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOffer"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOffer, VIS.CalloutEngine);//inherit CalloutEngine
    /**
    * 
    * @param ctx context
    * @param windowNo window no
    * @param mTab tab
    * @param mField field
    * @param value value
    * @return null or error message
    */
    CalloutOffer.prototype.Datechk = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var a = Util.getValueOfDateTime(value);
        if (a == null || a == 0) {
            return "";
        }
        var org_id = mTab.getValue("AD_ORG_ID");
        var days_Payable = 0;//Payable Days stored in settings window
        //String sql = "select  DAYSPAYABLE2 from fo_deposits where FO_DEPOSITS_ID=1000000 ";

        // Query changed because of the change of the table (Sandeep 6-10-2009)
        var sql = "select  DAYSPAYABLE2 from FO_SETTINGS where AD_ORG_ID=" + org_id;
        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql, null, null);
            while (dr.read()) {
                days_Payable = Util.getValueOfInt(dr[0].toString());//.getInt(1);
                // Console.WriteLine("Days Payable"+days_Payable);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }

        var dt = Util.getValueOfDateTime(mTab.getValue("DATE1"));
        //Console.WriteLine("dt"+dt);
        //java.sql.Timestamp incdays;
        var incdays;
        incdays = dt.AddDays(days_Payable);//incdays = TimeUtil.AddDays(dt, days_Payable);
        //Console.WriteLine("Inc Days"+incdays);
        mTab.setValue("DATE2", incdays);

        var Offer;
        var Offer_num = 100;
        var sql1 = "select max(OFFERNO)from FO_OFFER";
        try {
            dr = VIS.DB.executeReader(sql1, null, null);
            while (dr.read()) {
                Offer_num = Util.getValueOfInt(dr[0].toString());//.getInt(1);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        if (Offer_num == null | Offer_num == 0) {
            Offer = 100;

        }
        else {
            Offer = Offer_num + 1;

        }
        mTab.setValue("OFFERNO", Offer);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /**
    * 
    * @param ctx context
    * @param windowNo window no
    * @param mTab tab
    * @param mField field
    * @param value value
    * @return null or error message
    */
    CalloutOffer.prototype.guestpricelist = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null) {
            return "";
        }
        //int add_ID = (int)value;
        var add_ID = Util.getValueOfInt(value);
        if (add_ID == null || add_ID == 0)
            return "";
        //int add_ID = (Integer)mTab.getValue("FO_ADDRESS_ID");
        var sql = "select FO_PRICE_LIST_ID from FO_ADDRESS_PRICE where FO_ADDRESS_ID=" + add_ID;
        var pricelist_ID = 0;
        var dr = null;
        try {
            //PreparedStatement pst = DataBase.prepareStatement(sql, null);

            //ResultSet rs = pst.executeQuery();
            dr = VIS.DB.executeReader(sql, null, null);
            while (dr.read()) {
                pricelist_ID = Util.getValueOfInt(dr[0]);//rs.getInt(1);
            }
            dr.close();
            // pst.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        mTab.setValue("FO_PRICE_LIST_ID", pricelist_ID);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOffer = CalloutOffer;
    //************ CalloutOffer End *********


    //****************CalloutCurrency Start ******
    function CalloutCurrency() {
        VIS.CalloutEngine.call(this, "VIS.CalloutCurrency"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutCurrency, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutCurrency.prototype.Currency = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var sql = "";
        try {
            sql = "select C_Currency_id from m_pricelist where m_pricelist_ID = " + Util.getValueOfInt(value);
            var C_Currency_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            mTab.setValue("C_Currency_ID", C_Currency_ID);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };
    VIS.Model.CalloutCurrency = CalloutCurrency;
    //****************CalloutCurrency End ******


    //*************copyname Starts****************
    function copyname() {
        VIS.CalloutEngine.call(this, "VIS.copyname");//must call
    };
    VIS.Utility.inheritPrototype(copyname, VIS.CalloutEngine); //inherit prototype
    copyname.prototype.product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        mTab.setValue("Help", mTab.getValue("Name"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    copyname.prototype.product2 = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        mTab.setValue("Description", mTab.getValue("Help"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.copyname = copyname;
    //****************copyname Ends*************


    //*************CalloutUserTimeRec Start******************

    function CalloutUserTimeRec() {
        VIS.CalloutEngine.call(this, "VIS.CalloutUserTimeRec");//must call
    };
    VIS.Utility.inheritPrototype(CalloutUserTimeRec, VIS.CalloutEngine); //inherit prototype

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutUserTimeRec.prototype.IsInternal = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            // Util.getValueOfInt(value);
            var sql = "select ProfileType from S_Resource where AD_User_ID = " + Util.getValueOfInt(value);
            var pType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            if (pType != "") {
                if (pType.toUpper() == "I") {
                    mTab.setValue("IsInternal", true);
                }
                else if (pType.toUpper() == "E") {
                    mTab.setValue("IsInternal", false);
                }
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutUserTimeRec = CalloutUserTimeRec;
    //**************** CalloutUserTimeRec End ***************

    //*************CalloutTimeExpense Start*******************
    function CalloutTimeExpense() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTimeExpense");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTimeExpense, VIS.CalloutEngine); //inherit prototype



    /// <summary>
    /// Expense Report Line
    //- called from M_Product_ID, S_ResourceAssignment_ID
    //set ExpenseAmt

    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="mTab">grid tab</param>
    /// <param name="mField">grid field</param>
    /// <param name="value">new value</param>
    /// <returns>null or error message</returns>
    CalloutTimeExpense.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var M_Product_ID = value;
        if (M_Product_ID == null || Util.getValueOfInt(M_Product_ID) == 0) {
            return "";
        }
        this.setCalloutActive(true);
        var priceActual = null;

        //	get expense date - or default to today's date
        //var DateExpense = VIS.Env.ctx.getContext("DateExpense");
        var DateExpense = ctx.getContext(windowNo, "DateExpense");
        var sql = null;
        var idr = null;
        try {
            var noPrice = true;

            //	Search Pricelist for current version
            sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList_Version pv "
                + "WHERE p.M_Product_ID=pp.M_Product_ID"
                + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                + " AND pv.M_PriceList_ID=pl.M_PriceList_ID"
                + " AND pv.IsActive='Y'"
                + " AND p.M_Product_ID=@param1"		//	1
                + " AND pl.M_PriceList_ID=@param2"	//	2
                + " ORDER BY pv.ValidFrom DESC";
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
            var param = [];
            //pstmt.setInt(1, M_Product_ID.intValue());
            param[0] = new VIS.DB.SqlParam("@param1", Util.getValueOfInt(M_Product_ID));
            //pstmt.setInt(2, ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
            param[1] = new VIS.DB.SqlParam("@param2", ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, param, null);
            while (idr.read() && noPrice) {
                // DateTime plDate = rs.GetDateTime("ValidFrom");
                var plDate = idr.get("validfrom");//.GetDateTime("ValidFrom");
                //	we have the price list
                //	if order date is after or equal PriceList validFrom
                // if (plDate == null || !DateExpense.before(plDate))
                if (plDate == null || !(DateExpense < plDate)) {
                    noPrice = false;
                    //	Price
                    //priceActual =Util.getValueOfDecimal(idr["PriceStd"]);//.GetDecimal("PriceStd");
                    priceActual = Util.getValueOfDecimal(idr.get("pricestd"));//.GetDecimal("PriceStd");

                    if (priceActual == null) {
                        priceActual = Util.getValueOfDecimal(idr.get("pricelist"));//.GetDecimal("PriceList");
                    }
                    if (priceActual == null) {
                        priceActual = Util.getValueOfDecimal(idr.get("pricelimit"));//.GetDecimal("PriceLimit");
                    }
                    //	Currency
                    var ii = Util.getValueOfInt(idr.get("c_currency_id"));
                    if (!(idr == null)) {
                        mTab.setValue("C_Currency_ID", ii);
                    }
                }
            }
            idr.close();
            //	no prices yet - look base pricelist
            if (noPrice) {
                //	Find if via Base Pricelist
                sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                    + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                    + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                    + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                    + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList bpl, M_PriceList_Version pv "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID"
                    + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                    + " AND pv.M_PriceList_ID=bpl.M_PriceList_ID"
                    + " AND pv.IsActive='Y'"
                    + " AND bpl.M_PriceList_ID=pl.BasePriceList_ID"	//	Base
                    + " AND p.M_Product_ID=@param1"		//  1
                    + " AND pl.M_PriceList_ID=@param2"	//	2
                    + " ORDER BY pv.ValidFrom DESC";
                var param1 = [];
                //pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, M_Product_ID.intValue());
                param1[0] = new VIS.DB.SqlParam("@param1", Util.getValueOfInt(M_Product_ID));

                //pstmt.setInt(2, ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
                param1[1] = new VIS.DB.SqlParam("@param2", ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
                //rs = pstmt.executeQuery();
                idr = VIS.DB.executeReader(sql, param1, null);
                while (idr.read() && noPrice) {
                    var plDate = idr.get("validfrom");//.GetDateTime("ValidFrom");
                    //	we have the price list
                    //	if order date is after or equal PriceList validFrom
                    if (plDate == null || !(DateExpense < plDate)) {
                        noPrice = false;
                        //	Price
                        priceActual = Util.getValueOfDecimal(idr.get("pricestd"));//.GetDecimal("PriceStd");
                        if (priceActual == null) {
                            priceActual = Util.getValueOfDecimal(idr.get("pricelist"));//.GetDecimal("PriceList");
                        }
                        if (priceActual == null) {
                            priceActual = Util.getValueOfDecimal(idr.get("pricelimit"));//.GetDecimal("PriceLimit");
                        }
                        //	Currency
                        var ii = Util.getValueOfInt(idr.get("c_currency_id"));
                        if (!(idr == null)) {
                            mTab.setValue("C_Currency_ID", ii);
                        }
                    }
                }
                idr.close();
            }
        }
        catch (err) {
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return e.message;//.getLocalizedMessage();
        }

        //	finish
        this.setCalloutActive(false);	//	calculate amount
        if (priceActual == null)
            priceActual = VIS.Env.ZERO;
        mTab.setValue("ExpenseAmt", priceActual);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	Expense_Product

    /// <summary>
    ///  Expense - Amount.- called from ExpenseAmt, C_Currency_ID,- calculates ConvertedAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutTimeExpense.prototype.Amount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);

        //	get values
        var ExpenseAmt = mTab.getValue("ExpenseAmt");
        var C_Currency_From_ID = mTab.getValue("C_Currency_ID");
        var C_Currency_To_ID = ctx.getContextAsInt(windowNo, "$C_Currency_ID");
        //DateTime DateExpense = new DateTime(ctx.getContextAsTime(windowNo, "DateExpense"));
        var DateExpense = ctx.getContext(windowNo, "DateExpense");
        //
        this.log.fine("Amt=" + ExpenseAmt + ", C_Currency_ID=" + C_Currency_From_ID);
        //	Converted Amount = Unit price
        var ConvertedAmt = ExpenseAmt.toString();
        //	convert if required
        if (!ConvertedAmt.equals(VIS.Env.ZERO) && C_Currency_To_ID != Util.getValueOfInt(C_Currency_From_ID)) {
            var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID");
            var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
            var paramString = ConvertedAmt.toString() + "," + C_Currency_From_ID.toString() + "," + C_Currency_To_ID.toString() + "," +
                AD_Client_ID.toString() + "," + AD_Org_ID.toString();

            //ConvertedAmt = VAdvantage.Model.MConversionRate.Convert(ctx,
            //    ConvertedAmt, Util.getValueOfInt(C_Currency_From_ID), C_Currency_To_ID,
            //    DateExpense, 0, AD_Client_ID, AD_Org_ID);
            ConvertedAmt = VIS.dataContext.getJSONRecord("MConversionRate/Convert", paramString);
        }
        mTab.setValue("ConvertedAmt", ConvertedAmt);
        this.log.fine("= ConvertedAmt=" + ConvertedAmt);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	Expense_Amount
    //	CalloutTimeExpense

    VIS.Model.CalloutTimeExpense = CalloutTimeExpense;

    //***************CalloutTimeExpense End*****************

    //*************CalloutTeamForcast Start***************
    function CalloutTeamForcast() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTeamForcast");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTeamForcast, VIS.CalloutEngine); //inherit prototype
    CalloutTeamForcast.prototype.ProductInfo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";
        var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
        if (M_PriceList_ID != 0) {
            var query = "Select M_PriceList_Version_ID from M_ProductPrice where M_Product_id=" + Util.getValueOfInt(value) +
                " and M_PriceList_Version_ID in (select m_pricelist_version_id from m_pricelist_version" +
                " where m_pricelist_id = " + M_PriceList_ID + " and isactive='Y')";
            var M_PriceList_Version_ID = Util.getValueOfInt(VIS.DB.executeScalar(query, null, null));
            if (M_PriceList_Version_ID != 0) {
                query = "Select PriceStd from M_ProductPrice where M_PriceList_Version_ID=" + M_PriceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
                var PriceStd = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));
                //ForcastLine.SetPriceStd(PriceStd);
                mTab.setValue("PriceStd", PriceStd);
                mTab.setValue("UnitPrice", PriceStd);
                mTab.setValue("PriceStd", (PriceStd * Util.getValueOfDecimal(mTab.getValue("QtyEntered"))));
            }
        }
        else {
            // ShowMessage.info("PriceLisetNotFound", true, null, null);
            VIS.ADialog.info("PriceLisetNotFound");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutTeamForcast.prototype.CalculatePrice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) <= 0) {
            return "";
        }

        var price = Util.getValueOfDecimal(mTab.getValue("UnitPrice")) * Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
        // ForcastLine.SetQtyEntered(price);
        mTab.setValue("PriceStd", price);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTeamForcast = CalloutTeamForcast;
    //************CalloutTeamForcast End****************


    //************CalloutTaxAmt Start***************
    function CalloutTaxAmt() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTaxAmt");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTaxAmt, VIS.CalloutEngine); //inherit prototype
    CalloutTaxAmt.prototype.TaxID = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var sql = "select Rate from C_Tax where C_Tax_ID = " + Util.getValueOfInt(value);
            var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql, null, null));
            if (rate != 0) {
                var taxAmt = (Util.getValueOfDecimal(mTab.getValue("ApprovedExpenseAmt")) * rate) / 100;
                taxAmt = taxAmt.toFixed(2);
                mTab.setValue("TaxAmt", taxAmt);
            }
            else {
                mTab.setValue("TaxAmt", VIS.Env.ZERO);
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutTaxAmt.prototype.ExpenseAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var sql = "select Rate from C_Tax where C_Tax_ID = " + Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
            var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql, null, null));
            if (rate != 0) {
                var taxAmt = (Util.getValueOfDecimal(value) * rate) / 100;
                taxAmt = taxAmt.toFixed(2);
                mTab.setValue("TaxAmt", taxAmt);
            }
            else {
                mTab.setValue("TaxAmt", VIS.Env.ZERO);
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTaxAmt = CalloutTaxAmt;
    //*************CalloutTaxAmt End**************

    //**************CalloutTax Start**********
    function CalloutTax() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTax");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTax, VIS.CalloutEngine); //inherit prototype
    CalloutTax.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var LineAmount = "";
        var TotalRate = VIS.Env.ZERO;
        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        //var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        //Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));

        // JID_0872: Grand Total is not calculating right
        Rate = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", C_Tax_ID.toString());
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((LineNetAmt * Rate) / 100);

        TotalRate = Util.getValueOfDecimal(TotalRate.toFixed(2));

        mTab.setValue("GrandTotal", (TotalRate + LineNetAmt));
        mTab.setValue("taxamt", TotalRate);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTax = CalloutTax;
    //***********CalloutTax End *************

    //**************CalloutSetBP Start******************
    function CalloutSetBP() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetBP");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetBP, VIS.CalloutEngine); //inherit prototype
    CalloutSetBP.prototype.SetBP = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            mTab.setValue("C_BPartner_ID", Util.getValueOfInt(value));
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutSetBP = CalloutSetBP;
    //**************CalloutSetBP End******************

    //*************CalloutProductToOpportunity Start***********
    function CalloutProductToOpportunity() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProductToOpportunity");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProductToOpportunity, VIS.CalloutEngine); //inherit prototype
    CalloutProductToOpportunity.prototype.ProductInfo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }

        // Added by Bharat on 23 May 2017 to remove client side queries
        var taskID = Util.getValueOfInt(mTab.getValue("C_ProjectTask_ID"));
        var phaseID = Util.getValueOfInt(mTab.getValue("C_ProjectPhase_ID"));
        var projID = Util.getValueOfInt(mTab.getValue("C_Project_ID"));
        var productID = Util.getValueOfInt(mTab.getValue("M_Product_ID"));

        var paramString = taskID.toString() + "," + phaseID.toString() + "," + projID.toString() + "," + productID.toString();

        //var id = Util.getValueOfInt(mTab.getValue("C_ProjectTask_ID"));

        //var Sql = "select c_project_id from C_ProjectPhase where C_ProjectPhase_id in(select C_ProjectPhase_id from" +
        //            " C_ProjectTask where C_ProjectTask_id=" + id + ")";

        //var projID = Util.getValueOfInt(CalloutDB.executeCalloutScalar(Sql, null, null));

        //if (id == 0) {

        //    Sql = "select c_project_id from C_ProjectPhase where C_ProjectPhase_id in(" + mTab.getValue("C_ProjectPhase_ID") + " )";
        //    projID = Util.getValueOfInt(CalloutDB.executeCalloutScalar(Sql, null, null));

        //    if (projID == 0) {
        //        projID = Util.getValueOfInt(mTab.getValue("C_Project_ID"));
        //    }

        //}

        //var paramString = projID.toString();
        var dr = VIS.dataContext.getJSONRecord("MProject/GetProjectDetail", paramString);
        if (dr != null) {
            var PriceList = Util.getValueOfDecimal(dr["PriceList"]);
            mTab.setValue("PriceList", PriceList);

            var PriceStd = Util.getValueOfDecimal(dr["PriceStd"]);
            mTab.setValue("PlannedPrice", PriceStd);
            mTab.setValue("PlannedQty", 1);
            var PriceLimit = Util.getValueOfDecimal(dr["PriceLimit"]);


            //var priceList_Version_ID = Util.getValueOfInt(dr["M_PriceList_Version_ID"]);
            // X_C_Project proj = new X_C_Project(ctx, projID, null);
            //var query = "Select M_ProductPrice_id from M_ProductPrice where M_Product=" + Util.getValueOfInt(value);
            //var priceID = Util.getValueOfInt(CalloutDB.executeCalloutScalar(query,null,null)); 
            //var query = "Select PriceList from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
            //ViennaAdvantage.Model.X_M_ProductPrice ProPrice = new ViennaAdvantage.Model.X_M_ProductPrice(ctx, proj.GetM_PriceList_Version_ID(), null);
            // var PriceList = Util.getValueOfDecimal(CalloutDB.executeCalloutScalar(query, null, null));
            //mTab.setValue("PriceList", PriceList);
            // oppLine.SetPriceList(Util.getValueOfDecimal(PriceList));

            //query = "Select PriceStd from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
            //var PriceStd = Util.getValueOfDecimal(CalloutDB.executeCalloutScalar(query, null, null));
            //mTab.setValue("PlannedPrice", PriceStd);
            // oppLine.SetPlannedPrice(PriceStd);

            //mTab.setValue("PlannedQty", 1);
            // oppLine.SetPlannedQty(1);

            // query = "Select PriceLimit from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
            //var PriceLimit = Util.getValueOfDecimal(CalloutDB.executeCalloutScalar(query, null, null));


            var discount;
            try {

                discount = ((PriceList - PriceStd) * 100) / PriceList;
                if (isNaN(discount)) {
                    this.setCalloutActive(false);
                    return VIS.Msg.getMsg("PriceNotDefined");
                }
            }
            catch (err) {
                this.setCalloutActive(false);
                return "PriceListNotSelected";
            }

            mTab.setValue("Discount", discount.toFixed(2));
            // oppLine.SetDiscount(Decimal.Subtract(PriceList ,PriceStd));

            mTab.setValue("PlannedMarginAmt", (PriceStd - PriceLimit));
            // oppLine.SetPlannedMarginAmt( Decimal.Subtract(PriceStd, PriceLimit));
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProductToOpportunity = CalloutProductToOpportunity;
    //**********CalloutProductToOpportunity End**************

    //*************CalloutPriceListOpp Start**************
    function CalloutPriceListOpp() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPriceListOpp");//must call
    };
    VIS.Utility.inheritPrototype(CalloutPriceListOpp, VIS.CalloutEngine); //inherit prototype
    CalloutPriceListOpp.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var sql = "";
        try {
            sql = "select M_PriceList_ID from M_PriceList_Version where M_PriceList_Version_ID = " + Util.getValueOfInt(value);
            var M_PriceList_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            if (M_PriceList_ID != 0) {
                mTab.setValue("M_PriceList_ID", M_PriceList_ID);

                sql = "select C_Currency_id from m_pricelist where m_pricelist_ID = " + M_PriceList_ID;
                var C_Currency_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
                if (C_Currency_ID != 0) {
                    mTab.setValue("C_Currency_ID", C_Currency_ID);
                }
                else {
                    //  ShowMessage.Info("CurrencyNotDefinedForThePriceList", true, null, null);
                    VIS.ADialog.info("CurrencyNotDefinedForThePriceList");
                }
            }

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError(sql, sql);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPriceListOpp = CalloutPriceListOpp;
    //**************CalloutPriceListOpp End*************


    //****************CalloutService Start***********
    function CalloutService() {
        VIS.CalloutEngine.call(this, "VIS.CalloutService");//must call
    };
    VIS.Utility.inheritPrototype(CalloutService, VIS.CalloutEngine); //inherit prototype

    /**
     *  @param ctx      Context
     *  @param WindowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutService.prototype.StatisticGroup = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        //When we change the Statistic Group the Statistic subgroup 
        //value will be cleared
        mTab.setValue("FO_STATISTICSUBGROUP_ID", 0);
        return "";
    }
    //CalloutService.prototype.SelectFunctionOnDashboard = function (ctx, windowNo, mTab, mField, value, oldValue) {

    //    if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
    //        return "";
    //    }
    //    this.setCalloutActive(true);
    //    var bl = Util.getValueOfBoolean(value);

    //    if (mField.getColumnName() == "IsSum") {
    //        //DisplayType.Integer;
    //        mTab.setValue("IsAvg", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsNone", false);
    //    }
    //    else if (mField.getColumnName() == "IsAvg") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsNone", false);

    //    }
    //    else if (mField.getColumnName() == "IsCount") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsAvg", false);
    //        mTab.setValue("IsNone", false);

    //    }
    //    else if (mField.getColumnName() == "IsNone") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsAvg", false);

    //    }
    //    this.setCalloutActive(false);


    //    return "";
    //}

    VIS.Model.CalloutService = CalloutService;
    //*****************CalloutService Ends*******************************

    //**************CalloutWorkflow Starts********
    function CallOutWorkflow() {
        VIS.CalloutEngine.call(this, "VIS.CallOutWorkflow ");//must call
    };
    VIS.Utility.inheritPrototype(CallOutWorkflow, VIS.CalloutEngine); //inherit prototype


    CallOutWorkflow.prototype.WorkflowType = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var wfType = Util.getValueOfString(value.toString());
        if (wfType == "R") {
            var ID = Util.getValueOfInt(VIS.DB.executeScalar("SELECT AD_Table_ID FROM AD_Table WHERE IsActive='Y' AND TableName= 'VADMS_MetaData'"));
            if (ID == 0) {
                VIS.ADialog.info("No_VADMS", null, "", "");
                //ShowMessage.Error("No_VADMS", true);
                return VIS.Msg.getMsg("No_VADMS");
            }
            mTab.setValue("AD_Table_ID", ID);
        }
        this.setCalloutActive(false);
        return "";
    };
    CallOutWorkflow.prototype.SetSelectedColumn = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        if ((Util.getValueOfString(VIS.DB.executeScalar("SELECT ColumnName FROM AD_Column WHERE AD_Column_ID=" + value))).toLower() == "C_GenAttributeSetInstance_ID".toLower()) {
            ctx.setContext(windowNo, "IsGenericAttribute", "Y");
        }
        else {
            ctx.setContext(windowNo, "IsGenericAttribute", "N");
        }
        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CallOutWorkflow = CallOutWorkflow;
    //**************CalloutWorkflow Ends**********

    //**************CalloutSalesQuotation Starts**************
    function CalloutSalesQuotation() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSalesQuotation ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSalesQuotation, VIS.CalloutEngine); //inherit prototype


    CalloutSalesQuotation.prototype.GetPaymentNote = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "" || value == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        var Note = Util.getValueOfString(VIS.DB.executeScalar("select documentnote from c_paymentterm where c_paymentterm_id=" + value));
        if (Note != null) {
            mTab.setValue("description", Note);
        }
        this.setCalloutActive(false);
        return "";

    }
    VIS.Model.CalloutSalesQuotation = CalloutSalesQuotation;
    //**************CalloutSalesQuotation Ends**************

    //*************CalloutSetDate Starts*****************
    function CalloutSetDate() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetDate ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetDate, VIS.CalloutEngine); //inherit prototype


    CalloutSetDate.prototype.SetDate = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        //if (this.isCalloutActive())
        //{
        //    return "";
        //}
        if (mTab.getValue("StartDate") == null || mTab.getValue("NoofCycle") == null) {
            this.setCalloutActive(false);
            return "";
        }
        else {
            try {
                this.setCalloutActive(true);

                //Neha----When we select Billing Frequency 'Year' Callout not change the Year 05 Sep,2018

                ////mTab.SetValue("EndDate", mTab.GetValue("StartDate"));
                //var enddate = new Date(mTab.getValue("StartDate"));
                //// int month = Util.GetValueOfInt(mTab.GetValue("NoofCycle")) + Util.GetValueOfInt(startdate.Value.Month);
                ////DateTime? endate = new DateTime(startdate.Value.Year, month, startdate.Value.Day);
                //var finalenddate = enddate.setMonth(enddate.getMonth() + Util.getValueOfInt(mTab.getValue("NoofCycle")));
                //if (finalenddate <= 0) {
                //    finalenddate = new Date();
                //}
                //else {
                //    finalenddate = new Date(finalenddate);
                //}
                //finalenddate = finalenddate.toISOString();
                //mTab.setValue("EndDate", finalenddate);
                //this.setCalloutActive(false);

                //Neha-- Update End Date on the basis of No of Cycle on Sales Order Line and Start Date and Billing Frequency (No of Months)” 05 Sep,2018
                // var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
                //var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
                //var months = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
                var months = "";
                months = VIS.dataContext.getJSONRecord("MOrderLine/GetNoOfMonths", Util.getValueOfString(mTab.getValue("C_Frequency_ID")));
                var SDate = new Date(mTab.getValue("StartDate"));
                var endDate = SDate.setMonth(SDate.getMonth() + (months * Util.getValueOfInt(mTab.getValue("NoofCycle"))));
                endDate = new Date(SDate.setDate(SDate.getDate() - 1));
                mTab.setValue("EndDate", endDate.toISOString());
                ctx = windowNo = mTab = mField = value = oldValue = null;
                this.setCalloutActive(false);
                //---------------------End---------------------------------------
            }
            catch (err) {

            }
        }
        return "";
    }
    VIS.Model.CalloutSetDate = CalloutSetDate;
    //*************CalloutSetDate Ends*******************

    //*************CalloutDisplayButton Starts***********
    function CalloutDisplayButton() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDisplayButton ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDisplayButton, VIS.CalloutEngine); //inherit prototype

    CalloutDisplayButton.prototype.DisplayButton = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";

        }
        if (Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")) != 0) {
            var sql = "select dc.DocSubTypeSO from c_doctype dc inner join c_docbasetype db on(dc.DocBaseType=db.DocBaseType)"
                + "where c_doctype_id=" + Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")) + " and db.DocBaseType='SOO' and dc.DocSubTypeSO in ('WR','WI')";
            var _DocBaseType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            if (_DocBaseType == "WR" || _DocBaseType == "WI") {
                mTab.setValue("InvoicePrint", "Y");
            }
            else {
                mTab.setValue("InvoicePrint", "N");

            }

        }
        else {
            this.setCalloutActive(false);
            return "";
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutDisplayButton = CalloutDisplayButton;
    //*************CalloutDisplayButton Ends*************
    //*************CalloutMoveConfirmLineSetQty**********//
    function CalloutMoveConfirmLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMoveConfirmLine");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMoveConfirmLine, VIS.CalloutEngine); //inherit prototype

    CalloutMoveConfirmLine.prototype.SetQty = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")))) {
            mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
            mTab.setValue("DifferenceQty", 0);
            mTab.setValue("ScrappedQty", 0);
            this.setCalloutActive(false);
            return "";
        }
        if (mField.getColumnName() == "ConfirmedQty") {
            if (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("ScrappedQty", 0);
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));

        }
        else if (mField.getColumnName() == "DifferenceQty") {
            if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty")))) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("ScrappedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty"))));

        }
        else if (mField.getColumnName() == "ScrappedQty") {
            if (Util.getValueOfDecimal(mTab.getValue("ScrappedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty")))) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));

        }

        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CalloutMoveConfirmLine = CalloutMoveConfirmLine;
    //*****************MoveConfirmLineSetQty********************//
    //*************CalloutDocumentType Starts***********
    function CalloutDocumentType() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDocumentType");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDocumentType, VIS.CalloutEngine); //inherit prototype


    CalloutDocumentType.prototype.SetSalesQuotation = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (Util.getValueOfString(mTab.getValue("DocSubTypeSO")) == 'OB' || Util.getValueOfString(mTab.getValue("DocSubTypeSO")) == 'ON') {
                mTab.setValue("IsSalesQuotation", true);
            }
            else {
                mTab.setValue("IsSalesQuotation", false);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }

    // JID_0811 
    // Added by Bharat on 24 August 2018 on change of Organization set Warehouse from Organization Info
    CalloutDocumentType.prototype.Organization = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var colName = "";
            if (mTab.getField("M_Warehouse_ID") != null) {
                var warehouse_ID = VIS.dataContext.getJSONRecord("MDocType/GetWarehouse", value.toString());
                if (warehouse_ID > 0) {
                    if (mTab.getTableName() == "M_Movement" && mTab.getField("DTD001_MWarehouseSource_ID") != null) {
                        mTab.setValue("DTD001_MWarehouseSource_ID", warehouse_ID);
                    }
                    else {
                        mTab.setValue("M_Warehouse_ID", warehouse_ID);
                    }
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
    VIS.Model.CalloutDocumentType = CalloutDocumentType;
    //*************CalloutDocumentType Ends*************
    //*************CalloutProjectCBParner Starts***********
    function CalloutProjectCBPartner() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProjectCBPartner");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProjectCBPartner, VIS.CalloutEngine); //inherit prototype


    CalloutProjectCBPartner.prototype.SetAddress = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (mField.getColumnName() == "C_BPartner_ID") {
                if (mTab.getField("Ref_BPartner_ID") != null) {
                    mTab.setValue("Ref_BPartner_ID", value)
                }
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MBPartner/GetBPData", Util.getValueOfString(mTab.getValue("C_BPartner_ID")));
                if (dr != null) {
                    var _Location_ID = VIS.Utility.Util.getValueOfInt(dr["C_BPartner_Location_ID"]);
                    if (_Location_ID == 0)
                        mTab.setValue("C_BPartner_Location_ID", null);
                    else
                        mTab.setValue("C_BPartner_Location_ID", _Location_ID);
                    var _User_ID = VIS.Utility.Util.getValueOfInt(dr["AD_User_ID"]);
                    if (_User_ID == 0)
                        mTab.setValue("AD_User_ID", null);
                    else
                        mTab.setValue("AD_User_ID", _User_ID);
                }
            }
            else if (mField.getColumnName() == "C_BPartnerSR_ID") {
                if (mTab.getField("Ref_BPartner_ID") != null) {
                    mTab.setValue("Ref_BPartner_ID", value)
                }
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MBPartner/GetBPData", Util.getValueOfString(mTab.getValue("C_BPartnerSR_ID")));
                if (dr != null) {
                    var _Location_ID = VIS.Utility.Util.getValueOfInt(dr["C_BPartner_Location_ID"]);
                    if (_Location_ID == 0)
                        mTab.setValue("C_BPartner_Location_ID", null);
                    else
                        mTab.setValue("C_BPartner_Location_ID", _Location_ID);
                    var _User_ID = VIS.Utility.Util.getValueOfInt(dr["AD_User_ID"]);
                    if (_User_ID == 0)
                        mTab.setValue("AD_User_ID", null);
                    else
                        mTab.setValue("AD_User_ID", _User_ID);
                }
            }
            //var sql = "SELECT au.ad_user_id,  cl.c_bpartner_location_id FROM c_bpartner cp  INNER JOIN c_bpartner_location cl ON cl.c_bpartner_id=cp.c_bpartner_id INNER JOIN Ad_User au ON au.c_bpartner_id   =cp.c_bpartner_id WHERE cp.c_bpartner_id= " + VIS.Utility.Util.getValueOfString(mTab.getValue("C_BPartner_ID")) + " AND cp.isactive       ='Y'  ORDER BY cp.created";

            //var ds = CalloutDB.executeCalloutDataSet(sql, null);
            //for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //    var dr = ds.tables[0].rows[i];
            //    if (dr != null) {
            //        var _Location_ID = VIS.Utility.Util.getValueOfInt(dr.getCell("C_BPartner_Location_ID"));
            //        if (_Location_ID == 0)
            //            mTab.setValue("C_BPartner_Location_ID", null);
            //        else
            //            mTab.setValue("C_BPartner_Location_ID", _Location_ID);

            //        var _User_ID = VIS.Utility.Util.getValueOfInt(dr.getCell("AD_User_ID"));
            //        if (_User_ID == 0)
            //            mTab.setValue("AD_User_ID", null);
            //        else
            //            mTab.setValue("AD_User_ID", _User_ID);
            //    }
            //}


            //var _locatio_id = Util.getValueOfString(CalloutDB.executeCalloutScalar(sql, null, null));
            //if (parseInt(_locatio_id)>0) {
            //     mTab.setValue("C_BPartner_Location_ID", parseInt(_locatio_id));
            //}

        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutProjectCBPartner = CalloutProjectCBPartner;
    //*************CalloutProjectCBParner Ends*************


    //*************CalloutMCampaign Starts***********
    function CalloutMCampaign() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMCampaign");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMCampaign, VIS.CalloutEngine); //inherit prototype


    CalloutMCampaign.prototype.DateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
            DateDoc = new Date(mTab.getValue("DateContract"));
            DateReq = new Date(mTab.getValue("DateFinish"));
            if (DateReq.toISOString() < DateDoc.toISOString()) {
                mTab.setValue("DateFinish", "");
                this.setCalloutActive(false);
                VIS.ADialog.info("DateInvalid", null, "", "");
            }
            this.log.fine("DateFinish=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("DateError" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutMCampaign = CalloutMCampaign;
    //*************CalloutMCampaign Ends*************

    //*************CalloutMRequest Starts***********
    function CalloutMRequest() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMRequest");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMRequest, VIS.CalloutEngine); //inherit prototype


    CalloutMRequest.prototype.DateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
            DateDoc = new Date(mTab.getValue("StartDate"));
            DateReq = new Date(mTab.getValue("CloseDate"));
            if (DateReq.toISOString() < DateDoc.toISOString()) {
                mTab.setValue("CloseDate", "");
                this.setCalloutActive(false);
                VIS.ADialog.info("CloseDateInvalid", null, "", "");
            }
            this.log.fine("CloseDate=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("error in Date" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutMRequest.prototype.PlanDateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
            DateDoc = new Date(mTab.getValue("DateStartPlan"));
            DateReq = new Date(mTab.getValue("DateCompletePlan"));

            if (DateReq.toISOString() < DateDoc.toISOString()) {
                mTab.setValue("DateCompletePlan", "");
                this.setCalloutActive(false);
                VIS.ADialog.info("CmpDateInvalid", null, "", "");
            }
            this.log.fine("DateCompletePlan=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("error in Date" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutMRequest = CalloutMRequest;
    //*************CalloutMRequest Ends*************
    //****CalloutOrder Start
    function CalloutModuleMgmt() {
        VIS.CalloutEngine.call(this, "VIS.CalloutModuleMgmt");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutModuleMgmt, VIS.CalloutEngine); //inherit calloutengine

    CalloutModuleMgmt.prototype.GenerateMenuItemExportID = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var modulePrefix = ctx.getContext(windowNo, "Prefix");


        if ((mTab.getValue("MenuItemExport_ID") == null && mTab.getValue("IsMenuItem").toString() == "true")) {
            var date = new Date();
            var ExportID = modulePrefix + date.getTime();
            mTab.setValue("MenuItemExport_ID", ExportID);
        }

        this.setCalloutActive(false);
        return "";
    }

    CalloutModuleMgmt.prototype.GenerateVersionID = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var versionno = ctx.getContext(windowNo, "VersionNo");
        var versionid = versionno.replace(".", "");
        mTab.setValue("VersionID", versionid);
        this.setCalloutActive(false);
        return "";
    }

    CalloutModuleMgmt.prototype.CheckBoxValidation = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var CheckBoxValue = Util.getValueOfBoolean(value);
        if (CheckBoxValue == true) {
            var columnName = mField.getColumnName();
            if (columnName == "IsProfessionalFree") {
                mTab.setValue("IsProfessionalPaid", false);
                mTab.setValue("IsFree", false);
                mTab.setValue("IsPaid", false);
                // mTab.SetValue("IsProfessionalPaid", false);
            }
            else if (columnName == "IsProfessionalPaid") {
                mTab.setValue("IsProfessionalFree", false);
                mTab.setValue("IsFree", false);
                mTab.setValue("IsPaid", false);
            }
            else if (columnName == "IsFree") {
                mTab.setValue("IsProfessionalPaid", false);
                mTab.setValue("IsProfessionalFree", false);
                mTab.setValue("IsPaid", false);
            }
            else if (columnName == "IsPaid") {
                mTab.setValue("IsProfessionalPaid", false);
                mTab.setValue("IsFree", false);
                mTab.setValue("IsProfessionalFree", false);
            }
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutModuleMgmt = CalloutModuleMgmt;
    //*************CalloutDisplayButton Ends*************

    //**************CalloutPriclistUOM Start********************
    function CalloutPriclistUOM() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPriclistUOM");//must call
    };
    VIS.Utility.inheritPrototype(CalloutPriclistUOM, VIS.CalloutEngine); //inherit prototype

    CalloutPriclistUOM.prototype.SetUOM = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.
        if (mTab.getField("M_AttributeSetInstance_ID") != null) {
            mTab.setValue("M_AttributeSetInstance_ID", null);
        }
        try {
            if (Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_' ")) > 0) {
                var _M_Product_ID = Util.getValueOfInt(value);
                var _C_UOM_ID = Util.getValueOfInt(VIS.DB.executeScalar("SELECT C_UOM_ID FROM M_Product WHERE IsActive = 'Y' AND M_PRoduct_ID = " + _M_Product_ID));
                mTab.setValue("C_UOM_ID", _C_UOM_ID);
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

    VIS.Model.CalloutPriclistUOM = CalloutPriclistUOM;
    //**************CalloutPriclistUOM End********************


    //Added By Mohit as Changes given by Surya Sir.
    function CalloutProdCatVal() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProdCatVal");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProdCatVal, VIS.CalloutEngine);//inherit prototype
    CalloutProdCatVal.prototype.SetProdCatValues = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var Sql = "SELECT IsActive "
            // Change By Mohit Amortization process 02/11/2016
            var countVA038 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
            //var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            // End Change Amortization
            var countVA005 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' "));
            if (countVA005 > 0) {
                Sql += " , producttype ";
            }
            if (countVA005 > 0) {
                Sql += " , m_attributeset_id, c_taxcategory_id ";
            }
            Sql += " FROM M_Product_Category WHERE m_product_category_id=" + Util.getValueOfInt(value);
            var result = VIS.DB.executeDataSet(Sql);
            if (result != null) {
                if (result.tables[0].rows.length > 0) {

                    if (Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' ")) > 0) {
                        if (result.tables[0].rows[0].cells.producttype != null || Util.getValueOfString(result.tables[0].rows[0].cells.producttype) != "") {
                            mTab.setValue("PRODUCTTYPE", result.tables[0].rows[0].cells.producttype);
                        }
                        else {
                            mTab.setValue("PRODUCTTYPE", "");
                        }
                    }
                    if (countVA005 > 0) {
                        if (result.tables[0].rows[0].cells.m_attributeset_id != null || Util.getValueOfString(result.tables[0].rows[0].cells.m_attributeset_id) != "") {
                            mTab.setValue("M_AttributeSet_ID", Util.getValueOfInt(result.tables[0].rows[0].cells.m_attributeset_id));
                        }
                        else {
                            mTab.setValue("M_AttributeSet_ID", 0);
                        }
                        if (result.tables[0].rows[0].cells.c_taxcategory_id != null || Util.getValueOfString(result.tables[0].rows[0].cells.c_taxcategory_id) != "") {
                            mTab.setValue("C_TaxCategory_ID", Util.getValueOfInt(result.tables[0].rows[0].cells.c_taxcategory_id));
                        }
                        else {
                            mTab.setValue("C_TaxCategory_ID", 0);
                        }
                    }
                }
            }
            // Change Done By Mohit Aortization Process 02/11/2016, Checkin By Sukhwinder on 06 March, 2017
            if (countVA038 > 0) {
                if (Util.getValueOfString(result.tables[0].rows[0].cells.producttype) == "E" || Util.getValueOfString(result.tables[0].rows[0].cells.producttype) == "S") {
                    var AssetGroup_ID = Util.getValueOfInt(VIS.DB.executeScalar("SELECT A_Asset_Group_ID FROM M_Product_Category WHERE M_Product_Category_ID=" + Util.getValueOfInt(value)));
                    if (AssetGroup_ID > 0) {
                        var AmortizTemp_ID = Util.getValueOfInt(VIS.DB.executeScalar("SELECT VA038_AmortizationTemplate_ID FROM A_Asset_Group WHERE A_Asset_Group_ID=" + AssetGroup_ID));
                        if (AmortizTemp_ID > 0) {
                            mTab.setValue("VA038_AmortizationTemplate_ID", AmortizTemp_ID);
                        }
                        else {
                            mTab.setValue("VA038_AmortizationTemplate_ID", 0);
                        }
                    }
                    else {
                        mTab.setValue("VA038_AmortizationTemplate_ID", 0);
                    }
                }
                else {
                    mTab.setValue("VA038_AmortizationTemplate_ID", 0);
                }
            }
            // End Change Amortization

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }
    VIS.Model.CalloutProdCatVal = CalloutProdCatVal;


    //Changes by Sukhwinder on 6th March for Pardeep S D.
    function CalloutProdTypeVal() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProdTypeVal");
    }

    VIS.Utility.inheritPrototype(CalloutProdTypeVal, VIS.CalloutEngine);

    CalloutProdTypeVal.prototype.SetProdTypeValues = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {

            // If "Product Type" is Servies, Resource and Expense Type "Stocked" checkbox value will be "False". 
            if (value != 'I') {
                mTab.setValue("IsStocked", false);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }

    VIS.Model.CalloutProdTypeVal = CalloutProdTypeVal;
    //


    //  ****Callout Profit Tax Start*******************Added by Vikas  29-dec*********************************88
    function CalloutProfitTax() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProfitTax"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProfitTax, VIS.CalloutEngine);//inherit CalloutEngine

    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Gridfield</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutProfitTax.prototype.SetProfitBeforeTax = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null) {
            return "";
        }

        this.setCalloutActive(true);
        try {

            var Ptax = 0;
            var yr = 0, pDim = 0;
            var ds = null;
            ds = VIS.DB.executeDataSet("SELECT ProfitBeforeTax,C_Year_ID,C_ProfitAndLoss_ID FROM C_ProfitLoss WHERE C_ProfitLoss_ID=" + value, null, null);
            if (ds != null) {

                if (ds.tables[0].rows.length > 0) {

                    Ptax = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("ProfitBeforeTax"));
                    yr = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("C_Year_ID"));
                    pDim = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("C_ProfitAndLoss_ID"));
                    mTab.setValue("ProfitBeforeTax", Ptax);
                    mTab.setValue("C_Year_ID", yr);
                    mTab.setValue("C_ProfitAndLoss_ID", pDim);
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
    }
    VIS.Model.CalloutProfitTax = CalloutProfitTax;
    //********************* Callout Profit Tax Start End ***************************************************************

    //---- Code Added by Anuj --- 30-12-2015
    function CalloutBPartner() {
        VIS.CalloutEngine.call(this, "VIS.CalloutBPartner");
    };
    VIS.Utility.inheritPrototype(CalloutBPartner, VIS.CalloutEngine);//inherit prototype
    CalloutBPartner.prototype.BPGroup = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var _sql = "select M_ReturnPolicy_ID, M_DiscountSchema_ID, M_PRICELIST_ID, PO_PriceList_ID,PO_DiscountSchema_ID,PO_ReturnPolicy_ID from C_BP_Group where C_BP_Group_ID=" + Util.getValueOfInt(value);
            var ds = VIS.DB.executeDataSet(_sql);
            var IsCustomer = mTab.getValue("IsCustomer");
            var IsVendor = mTab.getValue("IsVendor");
            if (IsCustomer) {
                if (ds != null && ds.tables[0].rows.length > 0) {
                    var _returnPolicy = Util.getValueOfInt(ds.tables[0].rows[0].cells["m_returnpolicy_id"]);
                    var _discountSchema = Util.getValueOfInt(ds.tables[0].rows[0].cells["m_discountschema_id"]);
                    var _pricelist = Util.getValueOfInt(ds.tables[0].rows[0].cells["m_pricelist_id"]);
                    mTab.setValue("M_ReturnPolicy_ID", _returnPolicy);
                    mTab.setValue("M_DiscountSchema_ID", _discountSchema);
                    mTab.setValue("M_PRICELIST_ID", _pricelist);
                }
            }
            if (IsVendor) {
                if (ds != null && ds.tables[0].rows.length > 0) {
                    _returnPolicy = Util.getValueOfInt(ds.tables[0].rows[0].cells["po_returnpolicy_id"]);
                    _discountSchema = Util.getValueOfInt(ds.tables[0].rows[0].cells["po_discountschema_id"]);
                    _pricelist = Util.getValueOfInt(ds.tables[0].rows[0].cells["po_pricelist_id"]);
                    mTab.setValue("PO_ReturnPolicy_ID", _returnPolicy);
                    mTab.setValue("PO_DiscountSchema_ID", _discountSchema);
                    mTab.setValue("PO_PriceList_ID", _pricelist);

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
    }
    VIS.Model.CalloutBPartner = CalloutBPartner;


    //-----------Shivani Saka------ Sept.15,2016

    function CalloutshipLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutshipLine");
    };
    VIS.Utility.inheritPrototype(CalloutshipLine, VIS.CalloutEngine); //inherit calloutengine


    CalloutshipLine.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        this.setCalloutActive(true);
        try {
            sql = "SELECT M_Product_ID , movementqty , M_AttributeSetInstance_ID FROM M_InOutLine WHERE M_InOutLine_ID=" + Util.getValueOfInt(value);
            // var M_Product_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));

            var ds = VIS.DB.executeDataSet(sql);

            var movementqty = 0;
            if (ds.getTables().length > 0) {
                if (ds.getTables()[0].getRows().length > 0) {
                    var product = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("M_Product_ID"));
                    movementqty = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("movementqty"));
                    var attribute = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("M_AttributeSetInstance_ID"))


                }
            }

            sql = "SELECT  SUM(ConfirmedQty + scrappedqty) FROM M_PackageLine WHERE  M_Package_ID=" + mTab.getValue("M_Package_ID") + " and m_inoutline_id =  " + mTab.getValue("M_InOutLine_ID");
            var totalConfirmedAndScrapped = Util.getValueOfDecimal(VIS.DB.executeScalar(sql))

            mTab.setValue("M_Product_ID", product);
            mTab.setValue("Qty", movementqty - totalConfirmedAndScrapped);
            mTab.setValue("M_AttributeSetInstance_ID", attribute);
            mTab.setValue("DTD001_IsConfirm", true);

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }
    VIS.Model.CalloutshipLine = CalloutshipLine;
    //***Callout CalloutshipLine End

    /*  Callout Recurring*******************Added by Arpit Rai on 2nd Jan,2016  */
    function CalloutRecurring() {
        VIS.CalloutEngine.call(this, "VIS.CalloutRecurring"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutRecurring, VIS.CalloutEngine);//inherit CalloutEngine

    //Callout To Set Remaning Runs Zero while editing the Maximum Runs
    CalloutRecurring.prototype.SetMaxRunZero = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var maxRun = mTab.getValue("RunsMax");
        if (maxRun != "" && maxRun > 0) {
            mTab.setValue("RunsRemaining", 0);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }
    VIS.Model.CalloutRecurring = CalloutRecurring;


    /*  Callout ProductionLine**********Added on 25 july,2017  */
    function CalloutProductionLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProductionLine"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProductionLine, VIS.CalloutEngine);//inherit CalloutEngine

    //Callout To Set Charge Amount
    CalloutProductionLine.prototype.SetChargeAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var paramString = value.toString();;
            var dr = VIS.dataContext.getJSONRecord("MProductionLine/GetChargeAmt", paramString);
            if (dr != null) {
                mTab.setValue("Amt", dr["ChargeAmt"]);
                mTab.setValue("M_Product_ID", null);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }
    VIS.Model.CalloutProductionLine = CalloutProductionLine;


    /*  Callout CalloutProfitLoss**********Added on 05 December, 2017 By SUkhwinder */
    function CalloutProfitLoss() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProfitLoss"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProfitLoss, VIS.CalloutEngine);//inherit CalloutEngine

    //Callout To Set Datefrom and DateTo from selected year.(Sukhwinder on 7th Dec)
    CalloutProfitLoss.prototype.SetDateFromAndDateTo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var paramString = value.toString() + "," + ctx.getAD_Client_ID().toString();
            //var adClientID = ctx.getAD_Client_ID();
            //var yearID = mTab.getValue("C_Year_ID");

            var dr = VIS.dataContext.getJSONRecord("Common/GetPeriodFromYear", paramString);
            if (dr != null) {
                var fromDate = Util.getValueOfDate(dr["STARTDATE"]);
                var toDate = Util.getValueOfDate(dr["ENDDATE"]);

                mTab.setValue("DateFrom", fromDate);
                mTab.setValue("DateTo", toDate);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }


    //Callout To Set Currencyfrom selected acconting schema.(Sukhwinder on 12th Dec)
    CalloutProfitLoss.prototype.SetCurrency = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var paramString = value.toString() + "," + ctx.getAD_Client_ID().toString();
            //var adClientID = ctx.getAD_Client_ID();
            //var yearID = mTab.getValue("C_Year_ID");

            var dr = VIS.dataContext.getJSONRecord("Common/GetCurrencyFromAccountingSchema", paramString);
            if (dr != null) {
                var C_Currency_ID = Util.getValueOfInt(dr["CurrencyID"]);

                mTab.setValue("C_Currency_ID", C_Currency_ID);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }

    VIS.Model.CalloutProfitLoss = CalloutProfitLoss;

    //Arpit -to restrcit user from entering negative quantities in Ship/Receipt Confirm Line
    // JID_0236: On Ship/Receipt Confirm line Sum of [Confirmed Qty+Difference Qty+Scrap Qty] should not be greater than Target Qty. Same is working on Move confrimation window
    function CalloutShipReceiptConfirmLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutShipReceiptConfirmLine");//must call
    };
    VIS.Utility.inheritPrototype(CalloutShipReceiptConfirmLine, VIS.CalloutEngine); //inherit prototype

    CalloutShipReceiptConfirmLine.prototype.SetQty = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")))) {
            mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
            mTab.setValue("DifferenceQty", 0);
            mTab.setValue("ScrappedQty", 0);
            this.setCalloutActive(false);
            return "";
        }
        if (mField.getColumnName() == "ConfirmedQty") {
            if (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("ScrappedQty", 0);
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));

        }
        //else if (mField.getColumnName() == "DifferenceQty") {
        //    if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty")))) {
        //        mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
        //        mTab.setValue("DifferenceQty", 0);
        //        mTab.setValue("ScrappedQty", 0);
        //        this.setCalloutActive(false);
        //        return "";
        //    }
        //    mTab.setValue("ScrappedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty"))));

        //}
        else if (mField.getColumnName() == "ScrappedQty") {
            if (Util.getValueOfDecimal(mTab.getValue("ScrappedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty")))) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));
        }

        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CalloutShipReceiptConfirmLine = CalloutShipReceiptConfirmLine;


    //Arpit -to set multiply rate 1 in case of same UOM in both the columns..asked by sachin sir
    function CalloutUOMConversion() {
        VIS.CalloutEngine.call(this, "VIS.CalloutUOMConversion");//must call
    };
    VIS.Utility.inheritPrototype(CalloutUOMConversion, VIS.CalloutEngine); //inherit prototype
    CalloutUOMConversion.prototype.SetMultiplyRate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            mTab.setValue("MultiplyRate", 0);
            return "";
        }
        this.setCalloutActive(true);
        var fromUOM = mTab.getValue("C_UOM_ID");
        var toUOM = mTab.getValue("C_UOM_To_ID");
        if (fromUOM == toUOM) {
            mTab.setValue("DivideRate", 1);
            mTab.setValue("MultiplyRate", 1);
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutUOMConversion = CalloutUOMConversion;
    //Arpit

    //Added by Bharat on 30 Jan 2018 - to check inco term reference changed

    function CalloutIncoTerm() {
        VIS.CalloutEngine.call(this, "VIS.CalloutIncoTerm");//must call
    };

    VIS.Utility.inheritPrototype(CalloutIncoTerm, VIS.CalloutEngine); //inherit prototype

    CalloutIncoTerm.prototype.CheckIncoTerm = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var isSOTrx = ctx.isSOTrx(windowNo);
        var recordID = 0;
        var incoTerm = 0;
        var paramString = "";
        try {
            if (mTab.getTableName().startsWith("C_Order")) {
                if (isSOTrx && Util.getValueOfInt(mTab.getValue("C_Order_Quotation")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_Quotation," + Util.getValueOfInt(mTab.getValue("C_Order_Quotation")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (!isSOTrx && Util.getValueOfInt(mTab.getValue("Ref_Order_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",Ref_Order_ID," + Util.getValueOfInt(mTab.getValue("Ref_Order_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (Util.getValueOfInt(mTab.getValue("C_Order_Blanket")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_Blanket," + Util.getValueOfInt(mTab.getValue("C_Order_Blanket")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
            }
            else if (mTab.getTableName().startsWith("C_Invoice")) {
                if (isSOTrx && Util.getValueOfInt(mTab.getValue("C_Order_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_ID," + Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (!isSOTrx && Util.getValueOfInt(mTab.getValue("C_Order_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_ID," + Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (!isSOTrx && Util.getValueOfInt(mTab.getValue("M_InOut_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",M_InOut_ID," + Util.getValueOfInt(mTab.getValue("M_InOut_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
            }
            else if (mTab.getTableName().startsWith("M_InOut")) {
                if (isSOTrx && Util.getValueOfInt(mTab.getValue("C_Order_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_ID," + Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (!isSOTrx && Util.getValueOfInt(mTab.getValue("C_Order_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Order_ID," + Util.getValueOfInt(mTab.getValue("C_Order_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
                else if (!isSOTrx && Util.getValueOfInt(mTab.getValue("C_Invoice_ID")) > 0) {
                    paramString = isSOTrx + "," + mTab.getTableName() + ",C_Invoice_ID," + Util.getValueOfInt(mTab.getValue("C_Invoice_ID")).toString();
                    incoTerm = VIS.dataContext.getJSONRecord("MOrder/GetIncoTerm", paramString);
                }
            }
            if (incoTerm > 0 & incoTerm != value) {
                this.setCalloutActive(false);
                VIS.ADialog.warn("IncoTermChanged");
                return "";
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        return "";
    }

    VIS.Model.CalloutIncoTerm = CalloutIncoTerm;

    //Neha Thakur -Copy all fields on Blanket Purchase order and Blanket Sales Order header as per selected Blanket Order Reference..asked by Vineet Anand
    function CalloutBlanketOrderRef() {
        VIS.CalloutEngine.call(this, "VIS.CalloutBlanketOrderRef");//must call
    };
    VIS.Utility.inheritPrototype(CalloutBlanketOrderRef, VIS.CalloutEngine); //inherit prototype
    CalloutBlanketOrderRef.prototype.BlanketOrderRef = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        if (mField.getColumnName() == "Blanket_Ref_Order_ID") {
            var DataPrefix = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", "VA009_");
            if (DataPrefix["VA009_"]) {
                var paramString = value.toString() + "," + DataPrefix["VA009_"];
            }
            else {
                var paramString = value.toString() + "," + false;
            }

            var result = VIS.dataContext.getJSONRecord("MOrder/GetOrderHeader", paramString);
            if (result != null) {
                mTab.setValue("AD_Client_ID", result["AD_Client_ID"]);
                mTab.setValue("AD_Org_ID", result["AD_Org_ID"]);
                mTab.setValue("Description", result["Description"]);
                mTab.setValue("C_DocTypeTarget_ID", result["C_DocTypeTarget_ID"]);
                mTab.setValue("DateOrdered", Dateoffset(result["DateOrdered"]));
                mTab.setValue("DatePromised", Dateoffset(result["DatePromised"]));
                mTab.setValue("OrderValidFrom", Dateoffset(result["OrderValidFrom"]));
                mTab.setValue("OrderValidTo", Dateoffset(result["OrderValidTo"]));
                mTab.setValue("C_BPartner_ID", result["C_BPartner_ID"]);
                mTab.setValue("Bill_BPartner_ID", result["Bill_BPartner_ID"]);
                mTab.setValue("C_BPartner_Location_ID", result["C_BPartner_Location_ID"]);
                mTab.setValue("Bill_Location_ID", result["Bill_Location_ID"]);
                mTab.setValue("AD_User_ID", result["AD_User_ID"]);
                mTab.setValue("Bill_User_ID", result["Bill_User_ID"]);
                mTab.setValue("M_Warehouse_ID", result["M_Warehouse_ID"]);
                mTab.setValue("PriorityRule", result["PriorityRule"]);
                mTab.setValue("M_PriceList_ID", result["M_PriceList_ID"]);
                mTab.setValue("C_IncoTerm_ID", result["C_IncoTerm_ID"]);
                mTab.setValue("C_Currency_ID", result["C_Currency_ID"]);
                mTab.setValue("C_ConversionType_ID", result["C_ConversionType_ID"]);
                mTab.setValue("SalesRep_ID", result["SalesRep_ID"]);
                if (DataPrefix["VA009_"]) {
                    mTab.setValue("VA009_PaymentMethod_ID", result["VA009_PaymentMethod_ID"]);
                }
                mTab.setValue("C_PaymentTerm_ID", result["C_PaymentTerm_ID"]);
                mTab.setValue("C_Campaign_ID", result["C_Campaign_ID"]);
                mTab.setValue("C_Activity_ID", result["C_Activity_ID"]);
                mTab.setValue("AD_OrgTrx_ID", result["AD_OrgTrx_ID"]);
                mTab.setValue("User1_ID", result["User1_ID"]);
                mTab.setValue("User2_ID", result["User2_ID"]);
                mTab.setValue("TotalLines", result["TotalLines"]);
                mTab.setValue("GrandTotal", result["GrandTotal"]);
            }
        }
        this.setCalloutActive(false);
        return "";
    }
    function Dateoffset(datepara) {
        var date = new Date(datepara);
        date.setMinutes(-date.getTimezoneOffset() + date.getMinutes());
        return date.toISOString();

    };
    VIS.Model.CalloutBlanketOrderRef = CalloutBlanketOrderRef;

    //Neha - On change of Warehouse or Locator, make parent container and Locator as null.
    function CalloutLocatorPC() {
        VIS.CalloutEngine.call(this, "VIS.CalloutLocatorPC");//must call
    };
    VIS.Utility.inheritPrototype(CalloutLocatorPC, VIS.CalloutEngine); //inherit prototype
    CalloutLocatorPC.prototype.SetLocatorPC = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (mField.getColumnName() == "M_Warehouse_ID") {
            if (value != oldValue) {
                mTab.setValue("M_Locator_ID", "");
                mTab.setValue("Ref_M_Container_ID", "");
            }
        }
        else
            if (mField.getColumnName() == "M_Locator_ID") {
                if (value != oldValue) {
                    mTab.setValue("Ref_M_Container_ID", "");
                }
            }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }

    VIS.Model.CalloutLocatorPC = CalloutLocatorPC;

    //Neha - On change of Billing Frequency on Sales Order Line Or Service Contract, make Start Date and End Date  as null.
    function CalloutSetDates() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetDates");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetDates, VIS.CalloutEngine); //inherit prototype
    CalloutSetDates.prototype.SetDates = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (mField.getColumnName() == "C_Frequency_ID") {
            if (value != oldValue) {
                mTab.setValue("StartDate", "");
                mTab.setValue("EndDate", "");
            }
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutSetDates = CalloutSetDates;

    // Callout on selection of Product to Remove Attribute Set Instance if selected.
    function CalloutLandedCost() {
        VIS.CalloutEngine.call(this, "VIS.CalloutLandedCost");//must call
    };
    VIS.Utility.inheritPrototype(CalloutLandedCost, VIS.CalloutEngine); //inherit prototype
    CalloutLandedCost.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        if (mTab.getField("M_AttributeSetInstance_ID") != null) {
            mTab.setValue("M_AttributeSetInstance_ID", null);
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutLandedCost = CalloutLandedCost;

})(VIS, jQuery);
