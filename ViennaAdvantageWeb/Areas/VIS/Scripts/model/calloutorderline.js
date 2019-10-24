; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;


    //*************CalloutOrderLine Start**************
    function CalloutOrderLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderLine"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOrderLine, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutOrderLine.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return ""; //Must sir but it was not the case
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        //var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency; //By Sarab
        var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var invoice = Util.getValueOfInt(mTab.getValue("NoofCycle"));
        //var End = SDate.addDays(days * invoice);     //By sarab 
        //var End = SDate.setMonth(SDate.getMonth() + (days * invoice));        // By Karan
        var End = SDate.getDate() + "/" + (SDate.getMonth() + (months * invoice)) + "/" + SDate.getFullYear();
        if (End <= 0) {
            End = new Date();
        }
        else {
            End = new Date(End);
        }
        End = End.toISOString();
        mTab.setValue("EndDate", End);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutOrderLine.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        var Cycles = Util.getValueOfDecimal(value);
        var cyclesCount = Util.getValueOfDecimal(mTab.getValue("QtyPerCycle"));
        var qty = Cycles * cyclesCount;
        mTab.setValue("QtyEntered", qty);
        mTab.setValue("QtyOrdered", qty);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutOrderLine.prototype.QtyPerCycle = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var cyclesCount = Util.getValueOfDecimal(value);
        var Cycles = Util.getValueOfDecimal(mTab.getValue("NoofCycle"));
        var qty = Cycles * cyclesCount;
        mTab.setValue("QtyEntered", qty);
        mTab.setValue("QtyOrdered", qty);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOrderLine = CalloutOrderLine;
    //**************CalloutOrderLine End*****************

    //***********CalloutOrderlineRecording Start********
    function CalloutOrderlineRecording() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderlineRecording"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOrderlineRecording, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutOrderlineRecording.prototype.Orderline = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var paramString = Util.getValueOfInt(value);
            var dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);

            //X_C_OrderLine ol = new X_C_OrderLine(Env.GetCtx(), Util.getValueOfInt(value), null);
            mTab.setValue("M_Product_ID", dr["M_Product_ID"]);
            mTab.setValue("Qty", dr["Qty"]);
            mTab.setValue("C_UOM_ID", dr["C_UOM_ID"]);
            mTab.setValue("C_BPartner_ID", dr["C_BPartner_ID"]);
            mTab.setValue("PlannedHours", dr["PlannedHours"]);
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOrderlineRecording = CalloutOrderlineRecording;
    //***********CalloutOrderlineRecording End********




})(VIS, jQuery);