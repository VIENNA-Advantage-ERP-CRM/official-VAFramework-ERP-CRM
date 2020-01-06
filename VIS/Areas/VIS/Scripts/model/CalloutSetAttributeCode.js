; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;


    //********** CalloutSetAttributeCode Start *****

    function CalloutSetAttributeCode() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetAttributeCode"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutSetAttributeCode, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutSetAttributeCode.prototype.FillAttribute = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var AttrCode = ctx.getContext(windowNo, "AttrCode");
        this.setCalloutActive(true);
        if (AttrCode != null && AttrCode != "") {
            var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + Util.getValueOfInt(value);
            var attributeSet_ID = Util.getValueOfInt(VIS.DB.executeScalar(qry));
            if (attributeSet_ID > 0) {
                qry = "SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE UPC = '" + AttrCode + "' AND M_Product_ID = " + Util.getValueOfInt(value);
                var Attribute_ID = Util.getValueOfInt(VIS.DB.executeScalar(qry));
                if (Attribute_ID > 0) {
                    mTab.setValue("M_AttributeSetInstance_ID", Attribute_ID);
                }

                    // Change GSI Barcode
                else if (AttrCode.indexOf("(01)") >= 0) {
                    var paramString = AttrCode + "," + Util.getValueOfInt(value) + "," + windowNo;

                    var ASI_ID = VIS.dataContext.getJSONRecord("MInOut/GetASIID", paramString);
                    if (ASI_ID != 0) {
                        mTab.setValue("M_AttributeSetInstance_ID", ASI_ID);
                    }
                }
            }
        }
        this.setCalloutActive(false);
        return "";
    };

    CalloutSetAttributeCode.prototype.FillUPC = function (ctx, windowNo, mTab, mField, value, oldValue) {
        var sql = "";
        var manu_ID = 0;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var AttrCode = ctx.getContext(windowNo, "AttrCode");
        this.setCalloutActive(true);
        if (Util.getValueOfString(AttrCode) != "") {
            sql = "SELECT Count(*) FROM M_Manufacturer WHERE IsActive = 'Y' AND UPC = '" + AttrCode + "'";
            manu_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            if (manu_ID > 0) {
                this.setCalloutActive(false);
                return VIS.Msg.getMsg("Attribute Code Already Exist");
            }
            else {
                mTab.setValue("UPC", AttrCode);
            }
        }
        this.setCalloutActive(false);
        return "";
    };

    VIS.Model.CalloutSetAttributeCode = CalloutSetAttributeCode;
    //********CalloutSetAttribute End********



})(VIS, jQuery);