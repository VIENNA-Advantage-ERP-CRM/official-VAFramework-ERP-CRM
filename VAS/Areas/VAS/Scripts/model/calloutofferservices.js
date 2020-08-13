; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //********* CalloutOfferServices Start *******
    function CalloutOfferServices() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOfferServices"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOfferServices, VIS.CalloutEngine);//inherit CalloutEngine
    CalloutOfferServices.prototype.CalculatePrice = function (number, price, rebate) {
        return Decimal.Subtract(Decimal.Multiply(number, price), rebate);
    };
    CalloutOfferServices.prototype.CalculateRebate = function (number, price, rebate) {
        return Decimal.Multiply(Decimal.Multiply(number, price), Decimal.Multiply(new Decimal(0.01), rebate));
        //return (new Decimal(number),price,arebate);
    };
    /************************************************************************
    *  Service Selected
    *
    *  @param ctx      Context
    *  @param windowNo current Window No
    *  @param mTab     Model Tab
    *  @param mField   Model Field
    *  @param value    The new value
    *  @return Error message or ""
    ************************************************************************/
    /*****************************************************************************
     *   if service is selected then UOM will be displayed
     ****************************************************************************/
    CalloutOfferServices.prototype.OfferServices = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var FO_SERVICE_ID = Util.getValueOfInt(value);
        if (FO_SERVICE_ID == null || FO_SERVICE_ID == 0)
            return "";
        var sql = "select uom.FO_HOTEL_UOM_ID,s.DESCRIPTION " +
                   "from FO_HOTEL_UOM uom " +
                   "inner join FO_SERVICE s ON(uom.FO_HOTEL_UOM_ID= s.FO_HOTEL_UOM_ID) " +
                   "where s.FO_SERVICE_ID=@FO_SERVICE_ID ";
        var FO_HOTEL_UOM_ID = 0;
        var APRICE = new Decimal(0);
        var CG1PRICE = new Decimal(0);
        var CG2PRICE = new Decimal(0);
        var cal, cal1, cal2, grand_total;
        var calrebate, cg1calrebate, cg2calrebate;
        var UOM1, UOM2;
        var dr = null;
        try {
            //PreparedStatement pst = DataBase.prepareStatement(sql,null);
            //pst.setInt(1,FO_SERVICE_ID);
            //ResultSet rs = pst.executeQuery();
            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            param[0] = new VIS.DB.SqlParam("@FO_SERVICE_ID", FO_SERVICE_ID);

            dr = VIS.DB.executeReader(sql, param, null);
            while (dr.read()) {
                FO_HOTEL_UOM_ID = Util.getValueOfInt(dr[0]);
                mTab.setValue("AUOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG1UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG2UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("DESCRIPTION", Util.getValueOfString(dr[1]));//rs.getString(2));
            }
            dr.close();
            //pst.close();
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
        var sqlprice = "select PRICE,CHILDGROUP1,CHILDGROUP2,UOM1,UOM2 from FO_SERVICE_PRICE_PRICELINE " +
                        "where CREATED=(select max(CREATED) from FO_SERVICE_PRICE_PRICELINE " +
                        "where FO_SERVICE_ID=@FO_SERVICE_ID)";
        try {
            //PreparedStatement pst1 = DataBase.prepareStatement(sqlprice,null);
            //pst1.setInt(1,FO_SERVICE_ID);
            //ResultSet rs1 = pst1.executeQuery();
            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@FO_SERVICE_ID", FO_SERVICE_ID);
            dr = VIS.DB.executeReader(sqlprice, param, null);
            while (dr.read()) {
                APRICE = Util.getValueOfDecimal(dr[0]);
                CG1PRICE = Util.getValueOfDecimal(dr[1]);
                CG2PRICE = Util.getValueOfDecimal(dr[2]);

                //code changed by sandeep for is%
                UOM1 = dr[3].toString();
                UOM2 = dr[4].toString();
                if (UOM1 == "Y") {
                    //CG1PRICE=APRICE.multiply(CG1PRICE).multiply(new BigDecimal(0.01));
                    CG1PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG1PRICE), new Decimal(0.01));
                }
                if (UOM2 == "Y") {
                    CG2PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG2PRICE), new Decimal(0.01));
                    //CG2PRICE=APRICE.multiply(CG2PRICE).multiply(new BigDecimal(0.01));
                }

                //changes end
                mTab.setValue("APRICE", APRICE);
                mTab.setValue("CG1PRICE", CG1PRICE);
                mTab.setValue("CG2PRICE", CG2PRICE);
            }
            //rs1.close();
            //pst1.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));



        //Get Days wise difference
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var atoDate=Util.getValueOfDateTime(mTab.getValue("ADATETO");
        //var miltodate=atoDate.GetTime();

        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var afromDate=Util.getValueOfDateTime(mTab.getValue("ADATEFROM");
        //var milfromdate=afromDate.GetTime();

        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var t = afromDate.Subtract(atoDate);
        //var daydiff = t.Days;


        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }

        //var mildiff=miltodate-milfromdate;
        var secdiff = t.Seconds;
        var mindiff = t.Minutes;
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *    Check todate with fromdate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferServices.prototype.tochkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var a = Util.getValueOfDateTime(value);// Timestamp a = (Timestamp)value;
        if (a == null || a.equals(0))
            return "";
        var atoDate = Util.getValueOfDateTime(value);
        atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));

        //var afromDate=Util.getValueOfDateTime(mTab.getValue("ADATEFROM");
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));

        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        //var milfromdate=afromDate.GetTime();
        try {
            atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
            //var miltodate=atoDate.GetTime();
            //TimeSpan t = afromDate.Subtract(atoDate);//commented be sandeep
            // var numdays = t.Days;//commented be sandeep
            var numdays;
            numdays = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);

            //numdays=TimeUtil.getDaysBetween(afromDate, atoDate);
            if (numdays <= 0) {
                //Object[] ob={"ok"};
                //JOptionPane.showOptionDialog(new JFrame(),"ToDate cannot appear Before After Date","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                // ShowMessage.Error("'ToDate' cannot appear Before 'After Date'", true);
                ////////////////////////////////////////////////			
                var incdays;// = afromDate.AddDays(1);
                incdays = Utility.TimeUtil.AddDays(afromDate, 1);

                mTab.setValue("ADATETO", incdays);

                //Date date2 = new Date();
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t1 = atoDate.Subtract(afromDate);
                // var daydiff = t1.Days;//TimeUtil.getDaysBetween(afromDate,atoDate) ;

                var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t1.Milliseconds;//miltodate-milfromdate;
                var secdiff = t1.Seconds;//mildiff/1000;
                var mindiff = t1.Minutes;//secdiff/60;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var SetafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", SetafromDate);
                mTab.setValue("CG2DATEFROM", SetafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.Trim().Length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = System.Convert.ToInt16(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/

    /*****************************************************************************
     *    Check fromdate with todate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferServices.prototype.fromchkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var b = Util.getValueOfDateTime(value);
        if (b == null || b.equals(0))
            return "";
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));

        //var miltodate=atoDate.GetTime();
        try {
            afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
            //var milfromdate=afromDate.getTime();
            //if(afromDate.after(atoDate))
            if (afromDate.compareTo(atoDate) > 0) {
                mTab.setValue("CG1DATEFROM", afromDate);
                mTab.setValue("CG2DATEFROM", afromDate);
                ////////////////////////////////////////////////			
                var incdays;

                //incdays=TimeUtil.addDays(afromDate, 1);
                incdays = afromDate.addDays(1);// TimeUtil.addDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                // TimeSpan t2 = atoDate.Subtract(afromDate);//commented by sandeep
                var t2 = afromDate.subtract(atoDate);//Added by Sandeep
                //var daydiff = t2.Days;
                var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t2.Milliseconds;
                var secdiff = t2.Seconds;
                var mindiff = t2.Minutes;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var SetafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", SetafromDate);
                mTab.setValue("CG2DATEFROM", SetafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();

                if (uomsel == "" || uomsel.Trim().Length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if Adult UOM is selected the same will be displayed in child records
     ****************************************************************************/
    CalloutOfferServices.prototype.UOMSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var UOMsel = Util.getValueOfInt(value);
        UOMsel = Util.getValueOfInt(mTab.getValue("AUOM_ID"));
        if (UOMsel == null)
            return "";
        mTab.setValue("CG1UOM_ID", UOMsel);
        mTab.setValue("CG2UOM_ID", UOMsel);
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var miltodate = atoDate.GetTime();

        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate = afromDate.GetTime();
        var t3;
        if (afromDate.compareTo(atoDate) > 0) {
            t3 = afromDate.Subtract(atoDate);
        }
        else {
            t3 = atoDate.Subtract(afromDate);
        }


        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        //var daydiff = t3.Days;

        //var mildiff = miltodate - milfromdate;
        //var secdiff = mildiff / 1000;
        //var mindiff = secdiff / 60;

        var mildiff = t3.Milliseconds;
        var secdiff = t3.Seconds;
        var mindiff = t3.Minutes;
        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }

        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if Adult Number is selected then the calculation is done
     ****************************************************************************/
    CalloutOfferServices.prototype.AVALSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var uomsel = null;
        try {
            uomsel = mTab.getValue("AUOM_ID").toString();
        }
        catch (err) {
            this.setCalloutActive(false);
            return "";
        }
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));

        //var miltodate = atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate = afromDate.GetTime();
        var t4 = afromDate.Subtract(atoDate);

        var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
        //var daydiff = t4.Days;

        //var mildiff = miltodate - milfromdate;
        //var secdiff = mildiff / 1000;
        //var mindiff = secdiff / 60;

        var mildiff = t4.Milliseconds;
        var secdiff = t4.Seconds;
        var mindiff = t4.Minutes;

        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //						setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOfferServices = CalloutOfferServices;
    //*********CalloutOfferServices End *******

})(VIS, jQuery);