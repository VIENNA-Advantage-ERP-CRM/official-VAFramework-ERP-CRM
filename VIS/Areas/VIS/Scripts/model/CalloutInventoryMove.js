; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //***CalloutInventoryMove Start
    // new callout Add By Shivani Saka------------31-08-2016-------------

    function CalloutInventoryMove() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInventoryMove");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutInventoryMove, VIS.CalloutEngine); //inherit calloutengine

    CalloutInventoryMove.prototype.UOM = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";

        }
        if (this.isCalloutActive() || value == null)
            return "";
        this.setCalloutActive(true);
        try {
            var M_Product_ID = Util.getValueOfDecimal(mTab.getValue("M_Product_ID"));
            if (M_Product_ID == 0) {
                QtyEntered = 1;
                QtyOrdered = QtyEntered;
                mTab.setValue("QtyOrdered", QtyOrdered);
                mTab.setValue("QtyEntered", QtyEntered);
            }

            if (mField.getColumnName() == "C_UOM_ID") {

                var C_UOM_To_ID = Util.getValueOfInt(value);
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
                paramStr = M_Product_ID.toString().concat(',').concat(C_UOM_To_ID.toString()).concat(',').concat(QtyEntered.toString());
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                QtyOrdered = pc;

                var conversion = false
                if (QtyOrdered != null) {
                    conversion = QtyEntered != QtyOrdered;
                }
                if (QtyOrdered == null) {
                    conversion = false;
                    QtyOrdered = 1;
                }
                if (conversion) {
                    mTab.setValue("MovementQty", QtyOrdered);
                }
                else {

                    mTab.setValue("MovementQty", (QtyOrdered * QtyEntered));
                }


            }

            else if (mField.getColumnName() == "QtyEntered") {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyEntered = Util.getValueOfDecimal(value);

                //Get precision from server
                paramStr = C_UOM_To_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);
                var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);
                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2

                   QtyEntered.toString()); //3
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                QtyOrdered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,

                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;


                var conversion = QtyEntered != QtyOrdered;



                this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered=" + QtyEntered
                        + " -> " + conversion
                        + " QtyOrdered=" + QtyOrdered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                mTab.setValue("MovementQty", QtyOrdered);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    }
    VIS.Model.CalloutInventoryMove = CalloutInventoryMove;
    //***CalloutInventoryMove End

})(VIS, jQuery);