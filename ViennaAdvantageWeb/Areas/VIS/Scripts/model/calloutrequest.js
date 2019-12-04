; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //***********CalloutRequest Start ******

    function CalloutRequest() {
        VIS.CalloutEngine.call(this, "VIS.CalloutRequest"); //must call
    }
    VIS.Utility.inheritPrototype(CalloutRequest, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Request - Copy Mail Text - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutRequest.prototype.CopyMail = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        if (value == null || value.toString() == "") {
            return "";
        }
        var R_MailText_ID = Util.getValueOfInt(value);
        var sql = "SELECT MailHeader, MailText FROM R_MailText "
            + "WHERE R_MailText_ID=@Param1";
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);

            //pstmt.setInt(1, R_MailText_ID.intValue());
            Param[0] = new VIS.DB.SqlParam("@Param1", Util.getValueOfInt(R_MailText_ID));
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                //String txt = rs.getString(2);
                var txt = idr.get("mailtext");
                txt = VIS.Env.parseContext(ctx, windowNo, txt, false, true);
                mTab.setValue("Result", txt);
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
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  copyText


    /// <summary>
    /// Request - Copy Response Text - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>message or ""</returns>
    CalloutRequest.prototype.CopyResponse = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        if (value == null || value.toString() == "") {
            return "";
        }
        var R_StandardResponse_ID = Util.getValueOfInt(value);
        var sql = "SELECT Name, ResponseText FROM R_StandardResponse "
            + "WHERE R_StandardResponse_ID=@Param1";
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
            Param[0] = new VIS.DB.SqlParam("@Param1", Util.getValueOfInt(R_StandardResponse_ID));
            //pstmt.setInt(1, R_StandardResponse_ID.intValue());
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                //tring txt = rs.getString(2);
                //var txt = Util.getValueOfString(idr[1]);
                var txt = idr.get("responsetext");
                txt = VIS.Env.parseContext(ctx, windowNo, txt, false, true);
                mTab.setValue("Result", txt);
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
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  copyResponse

    /// <summary>
    /// Request - Chane of Request Type - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value"> The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutRequest.prototype.Type = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        mTab.setValue("R_Status_ID", null);
        if (value == null || value.toString() == "") {
            return "";
        }
        //var R_RequestType_ID = ((Integer)value).intValue();
        var R_RequestType_ID = Util.getValueOfInt(value);
        if (R_RequestType_ID == 0) {
            return "";
        }
        var paramString = R_RequestType_ID.toString();


        //Get BankAccount information

        var R_Status_ID = VIS.dataContext.getJSONRecord("MRequestType/GetDefaultR_Status_ID", paramString);
        //MRequestType rt = MRequestType.Get(ctx, R_RequestType_ID);
        // var R_Status_ID = rt.GetDefaultR_Status_ID();
        if (R_Status_ID != 0) {
            //mTab.setValue("R_Status_ID", new Integer(R_Status_ID));
            mTab.setValue("R_Status_ID", Util.getValueOfInt(R_Status_ID));

        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };//	type


    //CalloutRequest.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
    //    if (value == null || value.toString() == "" || this.isCalloutActive()) {
    //        return "";
    //    }

    //    this.setCalloutActive(true);

    //    var sql = "Select AD_User_ID FROM AD_User WHERE IsActive='Y' AND C_BPartner_ID=" + value;
    //    var AD_User_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql));

    //    if (AD_User_ID) {
    //        mTab.setValue("AD_User_ID", 0);
    //        mTab.setValue("AD_User_ID", AD_User_ID);
    //    }

    //    this.setCalloutActive(false);
    //    return "";
    //};

    VIS.Model.CalloutRequest = CalloutRequest;
    //***********CalloutRequest End ******

})(VIS, jQuery);