; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    var countEd011 = 0;

    //***********CalloutRequisition Start ******
    function CalloutRequisition() {
        VIS.CalloutEngine.call(this, "VIS.CalloutRequisition"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutRequisition, VIS.CalloutEngine);//inherit CalloutEngine

    /** Logger					*/


    /**
     *	Requisition Line - Product.
     *		- PriceStd
     *  @param ctx context
     *  @param WindowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutRequisition.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var M_Product_ID = value;
            if (M_Product_ID == null || M_Product_ID == 0)
                return "";
            //	setCalloutActive(true);
            //
            /**	Set Attribute
            if (ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID.intValue()
                && ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
                mTab.setValue("M_AttributeSetInstance_ID", Integer.valueOf(ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID")));
            else
                mTab.setValue("M_AttributeSetInstance_ID", null);
            **/

            // JID_0910: On change of product on line system is not removing the ASI. if product is changed then also update the ASI field.
            mTab.setValue("M_AttributeSetInstance_ID", null);

            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
            var qty = mTab.getValue("Qty");
            var isSOTrx = false;
            //  MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, qty, isSOTrx);
            //
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            // pp.SetM_PriceList_ID(M_PriceList_ID);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            //DateTime orderDate = (DateTime)mTab.getValue("DateRequired");
            var orderDate = mTab.getValue("DateRequired");
            // pp.SetPriceDate(orderDate);

            //Added by Amit 24-09-2015
            var sql = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'";
            var existEd011 = Util.getValueOfInt(VIS.DB.executeScalar(sql));
            var paramString;
            var C_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", M_Product_ID);
            if (existEd011 > 0) {
                paramString = M_Product_ID.toString().concat(",", C_BPartner_ID, ",", //2
                                                            qty, ",", //3
                                                            isSOTrx, ",", //4 
                                                            M_PriceList_ID, ",", //5
                                                            M_PriceList_Version_ID, ",", //6
                                                            orderDate, ",", null, ",", C_UOM_ID, ",", existEd011); //7
            }
            else {
                paramString = M_Product_ID.toString().concat(",", C_BPartner_ID, ",", //2
                                                            qty, ",", //3
                                                            isSOTrx, ",", //4 
                                                            M_PriceList_ID, ",", //5
                                                            M_PriceList_Version_ID, ",", //6
                                                            orderDate, ",", null); //7
            }


            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);
            mTab.setValue("PriceActual", dr["PriceActual"]);

            //		
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            ctx.setContext(windowNo, "EnforcePriceLimit", dr["EnforcePriceLimit"] ? "Y" : "N");	//	not used
            ctx.setContext(windowNo, "DiscountSchema", dr["DiscountSchema"] ? "Y" : "N");

            // Set Product UOM 
            mTab.setValue("C_UOM_ID", C_UOM_ID);

            var sql = "SELECT C_OrderLine_ID FROM C_OrderLine"
                                           + " WHERE C_Order_ID ="
                                           + " (SELECT C_Order_ID "
                                           + " FROM C_Order "
                                           + " WHERE DocumentNo="
                                           + " (SELECT DocumentNo FROM M_Requisition WHERE M_Requisition.M_Requisition_id = " + mTab.getValue("M_Requisition_id") + ")"
                                           + " AND AD_Client_ID =" + ctx.getAD_Client_ID() + ")"
                                           + " AND M_Product_ID=" + value;
            var OrderLine = Util.getValueOfInt(VIS.DB.executeScalar(sql));
            if (OrderLine > 0) {
                mTab.setValue("C_OrderLine_ID", OrderLine);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err.message;
            //MessageBox.Show("CalloutRequisition- Product");
        }
        //	setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *	Order Line - Amount.
     *		- called from Qty, PriceActual
     *		- calculates LineNetAmt
     *  @param ctx context
     *  @param WindowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutRequisition.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        try {
            this.setCalloutActive(true);

            //	Qty changed - recalc price
            if (mField.getColumnName() == "Qty"
                && "Y" == ctx.getContext("DiscountSchema")) {
                var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                var qty = value;
                var isSOTrx = false;
                var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
                //pp.SetM_PriceList_ID(M_PriceList_ID);
                var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                ///DateTime orderDate = (DateTime)mTab.getValue("DateInvoiced");
                var orderDate = mTab.getValue("DateInvoiced");
                // pp.SetPriceDate(orderDate);
                //

                //*******************
                var paramString = M_Product_ID.concat(",", C_BPartner_ID, ",", //2
                                                          qty, ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID, ",", //5
                                                          M_PriceList_Version_ID, ",", //6
                                                          orderDate, ",", null); //7


                //Get product price information
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


                //var rowDataDB = null;
                ////	only one row
                ////if (dr.read())
                //{
                //    //rowDataDB = this.readData(dr);

                //    mTab.setValue("PriceList", dr["PriceList"]);
                //    mTab.setValue("PriceLimit", dr.PriceLimit);
                //    mTab.setValue("PriceActual", dr.PriceActual);
                //    mTab.setValue("PriceEntered", dr.PriceEntered);
                //    mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
                //    mTab.setValue("Discount", dr.Discount);
                //    mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
                //    mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
                //    ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
                //    ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
                //}
                //*******************








                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //    M_Product_ID, C_BPartner_ID, qty, isSOTrx);
                //

                mTab.setValue("PriceActual", dr["PriceActual"]);
            }
            var StdPrecision = ctx.getStdPrecision();
            var Qty = mTab.getValue("QtyEntered");
            //Decimal PriceActual = (Decimal)mTab.getValue("PriceActual");
            var PriceActual = mTab.getValue("PriceActual");

            //	get values
            this.log.fine("amt - Qty=" + Qty + ", Price=" + PriceActual + ", Precision=" + StdPrecision);

            //	Multiply
            var LineNetAmt = Qty * PriceActual;
            if (Util.scale(LineNetAmt) > StdPrecision)
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
            mTab.setValue("LineNetAmt", LineNetAmt);
            this.log.info("amt - LineNetAmt=" + LineNetAmt);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError("CalloutRequisation", err);
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// Requisition Line - Quantity.
    /// - called from C_UOM_ID, QtyEntered
    /// - enforces qty UOM relationship
    /// </summary> JID_0996 - On entering the Entered qty callout will update the qty field in base UOM.
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutRequisition.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var paramStr = "";
        if (this.isCalloutActive() || value == null || value.toString() == "")
            return "";
        console.log("Before Charge Or Product");
        if (Util.getValueOfInt(mTab.getValue("M_Product_ID")) == 0) {
            return "";
        }
        console.log("After Charge Or Product");
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            if (steps) {
                this.log.Warning("init - M_Product_ID=" + M_Product_ID + " - ");
            }
            var QtyEntered = VIS.Env.ZERO;
            var QtyRequired = VIS.Env.ZERO;
            var PriceActual, PriceEntered;

            //	No Product
            if (M_Product_ID == 0) {
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                QtyRequired = QtyEntered;
                mTab.setValue("Qty", QtyRequired);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName() == "C_UOM_ID") {
                var C_UOM_To_ID = Util.getValueOfInt(value);
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                var dr = VIS.dataContext.getJSONRecord("ModulePrefix/GetModulePrefix", "VA009_");
                var countEd011 = 0;
                if (dr != null) {
                    countEd011 = dr["VA009_"] ? 1 : 0;
                }
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
                var isSOTrx = false;
                var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
                var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");

                var orderDate = ctx.getContext(windowNo, "DateDoc");
                paramStr = M_Product_ID.toString().concat(",", C_BPartner_ID, ",", //2
                                                          QtyEntered, ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID, ",", //5
                                                          M_PriceList_Version_ID, ",", //6
                                                          orderDate, ",", null, ",", M_AttributeSetInstance_ID.toString(), ",",  //7
                                                          C_UOM_To_ID, ",", countEd011);
                //Get product price information
                dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramStr);
                if (dr != null) {
                    PriceActual = dr["PriceActual"];
                }
                //Get precision from server side
                paramStr = C_UOM_To_ID.toString().concat(",");
                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);

                var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));
                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                //Conversion of Qty Entered
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ","
                                                             , QtyEntered.toString());
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                QtyRequired = pc;

                if (QtyRequired == null) {
                    QtyRequired = QtyEntered;
                }

                var conversion = QtyEntered != QtyRequired;

                if (PriceActual == 0) {
                    //Conversion of Price Entered
                    PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));
                    paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                    PriceActual.toString()); //3
                    pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                    PriceEntered = pc;
                    if (PriceEntered == null)
                        PriceEntered = PriceActual;
                    this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                        + " -> " + conversion
                        + " QtyRequired/PriceEntered=" + QtyRequired + "/" + PriceEntered);
                    ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                    mTab.setValue("PriceActual", PriceEntered);
                }
                else {
                    mTab.setValue("PriceActual", PriceActual);
                }
                mTab.setValue("Qty", QtyRequired);
            }
                //	QtyEntered changed - calculate QtyRequired
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

                QtyRequired = pc;
                if (QtyRequired == null)
                    QtyRequired = QtyEntered;

                var conversion = QtyEntered != QtyRequired;

                this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered=" + QtyEntered
                        + " -> " + conversion
                        + " Qty=" + QtyRequired);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                mTab.setValue("Qty", QtyRequired);
            }

            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));
            var StdPrecision = ctx.getStdPrecision();
            var LineNetAmt = QtyEntered * PriceActual;
            if (Util.scale(LineNetAmt) > StdPrecision)
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
            mTab.setValue("LineNetAmt", LineNetAmt);
            this.log.info("amt - LineNetAmt=" + LineNetAmt);
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutRequisition = CalloutRequisition;
    //*********** CalloutRequisition End *******

})(VIS, jQuery);