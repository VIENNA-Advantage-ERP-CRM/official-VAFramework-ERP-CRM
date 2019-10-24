; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //**************CalloutCheckInOut Start*******
    function CalloutCheckInOut() {
        VIS.CalloutEngine.call(this, "VIS.CalloutCheckInOut"); //must call
    };
    /**
    *  @param ctx      Context
    *  @param windowNo current Window No
    *  @param mTab     Model Tab
    *  @param mField   Model Field
    *  @param value    The new value
    *  @return Error message or ""
    */
    CalloutCheckInOut.prototype.TypeSelection = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //Boolean flag1=Boolean.parseBoolean(value.toString());
        var flag1 = Util.getValueOfBoolean(value);
        if (flag1 == null) {
            return "";
        }
        if (flag1 == true) {
            mTab.setValue("flag1", true);
        }
        else {
            mTab.setValue("flag1", false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */

    CalloutCheckInOut.prototype.RecordSort = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }

        //Boolean flag2=Boolean.parseBoolean(value.toString());
        var flag2 = Util.getValueOfBoolean(value);
        if (flag2 == null)
            return "";
        if (flag2 == true) {
            mTab.setValue("flag2", true);
        }
        else {
            mTab.setValue("flag2", false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutCheckInOut.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //Timestamp a=(Timestamp)value;
        var a = Util.getValueOfDateTime(value);
        if (a == null || a.equals(0))
            return "";
        var enddate = Util.getValueOfDateTime(value);
        var startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
        try {
            enddate = Util.getValueOfDateTime(mTab.getValue("TILL_DATE"));
            if (enddate.compareTo(startdate) < 0)
                //if(enddate.before(startdate))
            {
                //Object ob[]={"ok"};
                //JOptionPane.showOptionDialog(new JFrame(),"'End Date' cannot appear Before 'Start Date'","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                // ShowMessage.Error("EndAndStartDate", true);
                VIS.ADialog.info("EndAndStartDate");
                mTab.setValue("TILL_DATE", startdate);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            //e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutCheckInOut.prototype.StartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        // Timestamp b=(Timestamp)value;
        var b = Util.getValueOfDateTime(value);
        if (b == null || b.equals(0))
            return "";
        var startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
        var enddate = Util.getValueOfDateTime(mTab.getValue("TILL_DATE"));
        try {
            startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
            if (startdate.compareTo(enddate) > 0)
                //if(startdate.after(enddate))
            {
                mTab.setValue("TILL_DATE", startdate);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            //	e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutCheckInOut = CalloutCheckInOut;
    //**************CalloutCheckInOut End************ 


})(VIS, jQuery);