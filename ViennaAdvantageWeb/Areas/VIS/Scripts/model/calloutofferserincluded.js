; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //*******CalloutOfferSerIncluded Start ****

    function CalloutOfferSerIncluded() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOfferSerIncluded"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOfferSerIncluded, VIS.CalloutEngine);//inherit CalloutEngine
    CalloutOfferSerIncluded.prototype.CalculatePrice = function (number, price, rebate) {
        return Decimal.Subtract(Decimal.Multiply(number, price), rebate);
    };
    CalloutOfferSerIncluded.prototype.CalculateRebate = function (number, price, rebate) {
        return Decimal.Multiply(Decimal.Multiply(number, price), Decimal.Multiply(new Decimal(0.01), rebate));

        //return (new Decimal(number),price,arebate);
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
     *   if service is selected then UOM will be displayed
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.OfferServices = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
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
            //rs.close();
            //pst.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        var sqlprice = "select PRICE,CHILDGROUP1,CHILDGROUP2,UOM1,UOM2 from FO_SERVICE_PRICE_PRICELINE " +
                        "where CREATED=(select max(CREATED) from FO_SERVICE_PRICE_PRICELINE " +
                        "where FO_SERVICE_ID=@FO_SERVICE_ID)";
        try {
            //PreparedStatement pst1 = DataBase.prepareStatement(sqlprice,null);
            //pst1.setInt(1,FO_SERVICE_ID);
            //ResultSet rs1 = pst1.executeQuery();
            var param = [];
            // SqlParameter[] param = new VIS.DB.SqlParam[1];
            param[0] = new VIS.DB.SqlParam("@FO_SERVICE_ID", FO_SERVICE_ID);
            dr = VIS.DB.executeReader(sqlprice, param, null);

            while (dr.read()) {
                APRICE = Util.getValueOfDecimal(dr[0]);
                CG1PRICE = Util.getValueOfDecimal(dr[1]);
                CG2PRICE = Util.getValueOfDecimal(dr[2]);
                //code changed by sandeep for is%
                UOM1 = dr[3].toString();
                UOM2 = dr[4].toString();
                if (UOM1.equals("Y")) {
                    //CG1PRICE=APRICE.multiply(CG1PRICE).multiply(new BigDecimal(0.01));
                    CG1PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG1PRICE), new Decimal(0.01));
                }
                if (UOM2.equals("Y")) {
                    CG2PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG2PRICE), new Decimal(0.01));
                    //CG2PRICE=APRICE.multiply(CG2PRICE).multiply(new BigDecimal(0.01));
                }

                //code changes end
                mTab.setValue("APRICE", APRICE);
                mTab.setValue("CG1PRICE", CG1PRICE);
                mTab.setValue("CG2PRICE", CG2PRICE);
            }
            dr.close();
            //rs1.close();
            //pst1.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var aprice = Util.getValueOfDecimal(mTab.getValue("AP0RICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1P0RICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var t = afromDate.Subtract(atoDate);
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

        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;

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
                    // grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
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
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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
     *    Check todate with fromdate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.tochkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var a = Util.getValueOfDateTime(value);
        if (a == null || a.equals(0))
            return "";
        var atoDate = Util.getValueOfDateTime(value);
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        // var milfromdate = afromDate.GetTime();
        try {
            atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
            //   var miltodate = atoDate.GetTime();
            var numdays;
            numdays = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
            if (numdays <= 0) {

                ////////////////////////////////////////////////
                var incdays;
                incdays = Utility.TimeUtil.AddDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t = atoDate.Subtract(afromDate);
                var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
                var secdiff = t.TotalMinutes;// mildiff / 1000;
                var mindiff = t.TotalMinutes;// secdiff / 60;
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
                var setafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", setafromDate);
                mTab.setValue("CG2DATEFROM", setafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.toString().trim().length == 0) {
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
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
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
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
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
                            calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
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
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
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
     *    Check fromdate with todate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.fromchkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
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
            //  var milfromdate=afromDate.GetTime();
            if (afromDate.compareTo(atoDate) > 0) {
                //				Object ob[]={"ok"};
                //				JOptionPane.showOptionDialog(new JFrame(),"FromDate cannot appear after ToDate","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                //				mTab.setValue("ADATEFROM",null);
                //				mTab.setValue("CG1DATEFROM",null);
                //				mTab.setValue("CG2DATEFROM",null);
                mTab.setValue("CG1DATEFROM", afromDate);
                mTab.setValue("CG2DATEFROM", afromDate);
                ////////////////////////////////////////////////			
                var incdays;
                incdays = Utility.timeUtil.addDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t = atoDate.Subtract(afromDate);
                var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
                var secdiff = t.TotalSeconds;// mildiff / 1000;
                var mindiff = t.TotalMinutes;// secdiff / 60;
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
                var setafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", setafromDate);
                mTab.setValue("CG2DATEFROM", setafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.toString().trim().length == 0) {
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
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
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
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
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
                            calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
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
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
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
    CalloutOfferSerIncluded.prototype.UOMSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;

        //log.severe("value"+value);
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
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var t = atoDate.Subtract(afromDate);
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
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
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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
    CalloutOfferSerIncluded.prototype.AVALSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //			if (isCalloutActive())
        //				return"";
        //			setCalloutActive(true);
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
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var t = atoDate.Subtract(afromDate);
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);

        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
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
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
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
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
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

    VIS.Model.CalloutOfferSerIncluded = CalloutOfferSerIncluded;
    //******* CalloutOfferSerIncluded End***** 

})(VIS, jQuery);