; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //************CalloutProject Start *******
    function CalloutProject() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProject"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProject, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Project Line Planned - Price + Qty.
    //- called from PlannedPrice, PlannedQty, PriceList, Discount
    //- calculates PlannedAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Gridfield</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutProject.prototype.Planned = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);

        var PlannedQty, PlannedPrice, PriceList, Discount;
        var RemainingMargin = 0;
        var StdPrecision = ctx.getStdPrecision();

        //	get values
        var id = Util.getValueOfInt(mTab.getValue("C_ProjectTask_ID"));
        var Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_id IN (select C_ProjectPhase_id from" +
                    " C_ProjectTask WHERE C_ProjectTask_ID=" + id + ")";
        var projID = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));

        if (id == 0) {
            projID = Util.getValueOfInt(mTab.getValue("C_Project_ID"));
        }

        var query = "SELECT PriceLimit FROM M_ProductPrice WHERE M_PriceList_Version_ID = (SELECT c.m_pricelist_version_id FROM  c_project c WHERE c.c_project_Id=" + projID + ")  AND M_Product_id=" + Util.getValueOfInt(mTab.getValue("M_Product_ID"));
        var PriceLimit = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));

        PlannedQty = Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        if (PlannedQty == null) {
            PlannedQty = Envs.ONE;
        }
        else {

            RemainingMargin = (Util.getValueOfDecimal(mTab.getValue("PlannedPrice")) - PriceLimit) * Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        }


        PlannedPrice = Util.getValueOfDecimal(mTab.getValue("PlannedPrice"));
        if (PlannedPrice == null) {
            PlannedPrice = VIS.Env.ZERO;
        }
        PriceList = mTab.getValue("PriceList");
        if (PriceList == null) {
            PriceList = PlannedPrice;
        }
        Discount = mTab.getValue("Discount");
        if (Discount == null) {
            Discount = VIS.Env.ZERO;
        }

        var columnName = mField.getColumnName();
        if (columnName == "PlannedPrice") {
            if (PriceList == 0) {
                Discount = VIS.Env.ZERO;
            }
            else {
                //Decimal multiplier = PlannedPrice.multiply(Env.ONEHUNDRED)
                //.divide(PriceList, StdPrecision, Decimal.ROUND_HALF_UP);
                //var multiplier = Decimal.Round(Decimal.Divide(Decimal.Multiply(PlannedPrice.Value, Utility.Env.ONEHUNDRED),
                //   PriceList.Value), StdPrecision);//, MidpointRounding.AwayFromZero);

                var multiplier = ((PlannedPrice * VIS.Env.ONEHUNDRED) /
                   PriceList).toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
                //Discount = Env.ONEHUNDRED.subtract(multiplier);
                //Discount = Decimal.Subtract(VIS.Env.ONEHUNDRED, multiplier);
                Discount = (VIS.Env.ONEHUNDRED - multiplier);
            }
            mTab.setValue("Discount", Discount);
            this.log.fine("PriceList=" + PriceList + " - Discount=" + Discount
                 + " -> [PlannedPrice=" + PlannedPrice + "] (Precision=" + StdPrecision + ")");
        }
        else if (columnName == "PriceList") {
            if (VIS.Env.signum(PriceList) == 0) {
                Discount = VIS.Env.ZERO;
            }
            else {
                var multiplier = ((PlannedPrice * VIS.Env.ONEHUNDRED) /
                    PriceList).toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
                //Discount = Env.ONEHUNDRED.subtract(multiplier);
                Discount = VIS.Env.ONEHUNDRED - multiplier;
            }
            mTab.setValue("Discount", Discount);
            this.log.fine("[PriceList=" + PriceList + "] - Discount=" + Discount
                 + " -> PlannedPrice=" + PlannedPrice + " (Precision=" + StdPrecision + ")");
        }
        else if (columnName == "Discount") {
            var multiplier = (Discount / VIS.Env.ONEHUNDRED).toFixed(10);//, MidpointRounding.AwayFromZero);

            multiplier = VIS.Env.ONE - multiplier;
            //
            PlannedPrice = PriceList * multiplier;
            if (Util.scale(PlannedPrice) > StdPrecision) {
                PlannedPrice = PlannedPrice.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
            }
            mTab.setValue("PlannedPrice", PlannedPrice);
            this.log.fine("PriceList=" + PriceList + " - [Discount=" + Discount
                 + "] -> PlannedPrice=" + PlannedPrice + " (Precision=" + StdPrecision + ")");
        }

        //	Calculate Amount
        var PlannedAmt = PlannedQty * PlannedPrice;
        if (Util.scale(PlannedAmt) > StdPrecision) {
            //PlannedAmt = PlannedAmt.setScale(StdPrecision,Decimal.ROUND_HALF_UP);
            PlannedAmt = PlannedAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);

        }
        //
        this.log.fine("PlannedQty=" + PlannedQty + " * PlannedPrice=" + PlannedPrice + " -> PlannedAmt=" + PlannedAmt + " (Precision=" + StdPrecision + ")");
        mTab.setValue("PlannedAmt", PlannedAmt);
        mTab.setValue("PlannedMarginAmt", (RemainingMargin));
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    //	planned

    /// <summary>
    /// Project Line Product
    //- called from Product
    //- calculates PlannedPrice, PriceList, Discount
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="mTab">grid tab</param>
    /// <param name="mField">grid field</param>
    /// <param name="value">new valuw</param>
    /// <returns>null or error message</returns>
    CalloutProject.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = Util.getValueOfInt(value);
        var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
        if (M_Product_ID == null || Util.getValueOfInt(M_Product_ID) == 0
            || M_PriceList_Version_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
        var Qty = Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        var IsSOTrx = true;
        //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
        //    Util.getValueOfInt(M_Product_ID), C_BPartner_ID, Qty, IsSOTrx);
        //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
        var date = Util.getValueOfDateTime(mTab.getValue("PlannedDate"));
        //DateTime date = (DateTime)mTab.getValue("PlannedDate");
        if (date == null) {
            date = Util.getValueOfDateTime(mTab.getValue("DateContract"));
            if (date == null) {
                date = Util.getValueOfDateTime(mTab.getValue("DateFinish"));
                if (date == null) {
                    //date = new DateTime(System.currentTimeMillis());
                    date = new Date();
                    //date = new DateTime(CommonFunctions.CurrentTimeMillis());// (DateTime)(Util.getValueOfDateTime(CommonFunctions.CurrentTimeMillis()));
                }
            }
            //pp.SetPriceDate(date);
            ////
            //var PriceList = pp.GetPriceList();
            //mTab.setValue("PriceList", PriceList);
            //var PlannedPrice = pp.GetPriceStd();
            //mTab.setValue("PlannedPrice", PlannedPrice);
            //var Discount = pp.GetDiscount();
            //mTab.setValue("Discount", Discount);
            ////
            //var curPrecision = 2;
            //var PlannedAmt = pp.GetLineAmt(curPrecision);
            //mTab.setValue("PlannedAmt", PlannedAmt);


            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                               Qty.toString(), ",", //3
                                                               isSOTrx, ",", //4 
                                                               M_PriceList_ID.toString(), ",", //5
                                                               M_PriceList_Version_ID.toString(), ",", //6
                                                               date.toString(), ",",//7
                                                               null); //8





            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            var rowDataDB = null;


            // MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, Qty, isSOTrx);


            //		
            mTab.setValue("PriceList", dr["PriceList"]);
            // mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PlannedPrice", dr.PriceActual);
            //mTab.setValue("PriceEntered", dr.PriceEntered);
            //  mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            mTab.setValue("Discount", dr.Discount);
            mTab.setValue("PlannedAmt", dr.LineAmt);


            //	
            //this.log.fine("PlannedQty=" + Qty + " * PlannedPrice=" + PlannedPrice + " -> PlannedAmt=" + PlannedAmt);
            return "";
        }	//	product
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	CalloutProject

    VIS.Model.CalloutProject = CalloutProject;
    //************CalloutProject End ******


    //************CalloutProjectLine Start***************
    function CalloutProjectLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProjectLine");//must call
    };

    VIS.Utility.inheritPrototype(CalloutProjectLine, VIS.CalloutEngine); //inherit prototype

    CalloutProjectLine.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        var paramString = mTab.getValue("C_Charge_ID").toString();
        //X_C_Charge charge = new X_C_Charge(ctx, Cid, null);
        //var amt=charge.GetChargeAmt();
        var amt = VIS.dataContext.getJSONRecord("MCharge/GetCharge", paramString);
        mTab.setValue("PlannedQty", 1);
        mTab.setValue("PlannedPrice", amt);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutProjectLine = CalloutProjectLine;
    //**************CalloutProjectLine End*************



})(VIS, jQuery);