
; (function (VIS, $) {

    //form declaretion
    function VCreateFromInvoice(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var selfChild = this;
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;
        var windowNo = tab.getWindowNo();

        // create Log
        this.log = VIS.Logging.VLogger.getVLogger("VCreateFromInvoice");

        function dynInit() {
            //DynInit
            baseObj.title = VIS.Msg.getMsg("Invoice") + " .. " + VIS.Msg.getMsg("CreateFrom");

            //baseObj.lblInvoice.visible = false;
            //baseObj.cmbInvoice.visible = false;
            //baseObj.lblLocator.visible = false;
            //baseObj.locatorField.visible = false;

            if (baseObj.lblInvoice != null)
                baseObj.lblInvoice.getControl().css('display', 'none');
            if (baseObj.cmbInvoice != null)
                baseObj.cmbInvoice.getControl().css('display', 'none');
            if (baseObj.lblLocator != null)
                baseObj.lblLocator.getControl().css('display', 'none');
            if (baseObj.locatorField != null)
                baseObj.locatorField.getControl().css('display', 'none');
            baseObj.deliveryDate = new VIS.Controls.VDate("DeliveryDate", false, false, true, VIS.DisplayType.Date, "DeliveryDate");
            var lookupProd = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), baseObj.windowNo, 2221, VIS.DisplayType.Search);
            baseObj.vProduct = new VIS.Controls.VTextBoxButton("M_Product_ID", true, false, true, VIS.DisplayType.Search, lookupProd);
            var C_BPartner_ID = baseObj.initBPartner(true);
            baseObj.vBPartner.setReadOnly(true);
            initBPDetails(C_BPartner_ID);
            baseObj.fromInvoice = true;
            return true;
        }


        // Checked
        function getShipments(ctx, C_BPartner_ID) {
            //var pairs = [];

            var display = ("s.DocumentNo||' - '||")
                .concat(VIS.DB.to_char("s.MovementDate", VIS.DisplayType.Date, VIS.Env.getAD_Language(VIS.Env.getCtx())));
            // New column added to fill invoice which drop ship is true
            // Added by Vivek on 09/10/2017 advised by Pradeep
            var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsDropShip"));

            var _isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsSOTrx"));
            //VA230:Get IsReturnTrx based on document type on invoice
            var dt = VIS.dataContext.getJSONRecord("MDocType/GetDocType", VIS.Env.getCtx().getContextAsInt(selfChild.windowNo, "C_DocTypeTarget_ID").toString());
            var isReturnTrx = VIS.Utility.Util.getValueOfBoolean(dt["IsReturnTrx"]);

            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetShipmentsData",
                type: 'POST',
                //async: false,
                data: {
                    displays: display, CBPartnerIDs: C_BPartner_ID, IsDrop: _isdrop, IsSOTrx: _isSoTrx, isReturnTrxs: isReturnTrx, isProvisionlInvoices: false
                },
                success: function (data) {
                    var ress = JSON.parse(data);
                    if (ress && ress.length > 0) {
                        var key, value;
                        for (var i = 0; i < ress.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(ress[i]["key"]);
                            value = VIS.Utility.encodeText(ress[i]["value"]);
                            //pairs.push({ ID: key, value: value });

                            if (i == 0) {
                                baseObj.cmbShipment.getControl().append(" <option value=0> </option>");
                            }
                            baseObj.cmbShipment.getControl().append(" <option value=" + key + ">" + value + "</option>");
                        }
                        baseObj.cmbShipment.getControl().prop('selectedIndex', 0);
                    }
                },
                error: function (e) {
                    selfChild.log.info(e);
                },
            });
            //return pairs;
        }







        //function getShipments(ctx, C_BPartner_ID) {
        //    var pairs = [];

        //    //	Display
        //    var display = ("s.DocumentNo||' - '||")
        //        .concat(VIS.DB.to_char("s.MovementDate", VIS.DisplayType.Date, VIS.Env.getAD_Language(VIS.Env.getCtx())));

        //    //var sql = ("SELECT s.M_InOut_ID,").concat(display)
        //    //    .concat(" FROM M_InOut s "
        //    //    + "WHERE s.C_BPartner_ID=" + C_BPartner_ID + " AND s.IsSOTrx='N' AND s.DocStatus IN ('CL','CO')"
        //    //    + " AND s.M_InOut_ID IN "
        //    //        + "(SELECT sl.M_InOut_ID FROM M_InOutLine sl"
        //    //        + " LEFT OUTER JOIN M_MatchInv mi ON (sl.M_InOutLine_ID=mi.M_InOutLine_ID) "
        //    //        + "GROUP BY sl.M_InOut_ID,mi.M_InOutLine_ID,sl.MovementQty "
        //    //        + "HAVING (sl.MovementQty<>SUM(mi.Qty) AND mi.M_InOutLine_ID IS NOT NULL)"
        //    //        + " OR mi.M_InOutLine_ID IS NULL) "
        //    //    + "ORDER BY s.MovementDate");
        //    //Added by Arpit Rai on 30th March,2017
        //    //Purpose -changed in query to pic only those shipment whose invoice not in completed /closed / drafted mode
        //    //code commented
        //    //var sql = ("SELECT s.M_InOut_ID,").concat(display).concat(" FROM M_InOut s "
        //    //    + "WHERE s.C_BPartner_ID=" + C_BPartner_ID + " AND s.IsSOTrx='N' AND s.DocStatus IN ('CL','CO')"
        //    //    + " AND s.M_InOut_ID IN "
        //    //    + "(SELECT M_InOut_ID FROM (SELECT sl.M_InOut_ID,sl.M_InOutLine_ID,sl.MovementQty,mi.Qty FROM M_InOutLine sl "
        //    //    + "LEFT OUTER JOIN M_MatchInv mi ON (sl.M_InOutLine_ID=mi.M_InOutLine_ID) WHERE (sl.MovementQty <> nvl(mi.Qty,0) "
        //    //    + "AND mi.M_InOutLine_ID IS NOT NULL) OR mi.M_InOutLine_ID IS NULL ) GROUP BY M_InOut_ID,M_InOutLine_ID,MovementQty "
        //    //    + "HAVING MovementQty > SUM(nvl(Qty,0))) ORDER BY s.MovementDate");
        //    //code added here
        //    var sql = ("SELECT s.M_InOut_ID,").concat(display).concat(" FROM M_InOut s "
        //       + "WHERE s.C_BPartner_ID=" + C_BPartner_ID + " AND s.IsSOTrx='N' AND s.DocStatus IN ('CL','CO')"
        //       + " AND s.M_InOut_ID IN "
        //       + "(SELECT M_InOut_ID FROM (SELECT sl.M_InOut_ID,sl.M_InOutLine_ID,sl.MovementQty,mi.Qty,IL.QtyInvoiced FROM M_InOutLine sl "
        //       + "LEFT OUTER JOIN M_MatchInv mi ON (sl.M_InOutLine_ID=mi.M_InOutLine_ID) "
        //       + " LEFT OUTER JOIN C_INVOICELINE IL    ON (sl.C_ORDERLINE_ID =IL.C_ORDERLINE_ID)"
        //       + " LEFT OUTER JOIN C_Invoice I   ON I.C_INVOICE_ID      =IL.C_INVOICE_ID "
        //       + " AND I.DOCSTATUS NOT   IN ('VO','RE') "
        //       + " WHERE (sl.MovementQty <> nvl(mi.Qty,0) OR SL.MovementQty     <> NVL(IL.QtyInvoiced,0)"
        //       + "AND mi.M_InOutLine_ID IS NOT NULL) OR mi.M_InOutLine_ID IS NULL ) GROUP BY M_InOut_ID,M_InOutLine_ID,MovementQty "
        //       + "HAVING MovementQty > SUM(nvl(Qty,0)) OR MovementQty    > SUM(NVL(QtyInvoiced,0)) ) ORDER BY s.MovementDate");
        //    //End here
        //    try {
        //        var dr = VIS.DB.executeReader(sql.toString(), null);
        //        var key, value;
        //        while (dr.read()) {
        //            key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
        //            value = dr.getString(1);
        //            pairs.push({ ID: key, value: value });
        //        }
        //        dr.close();
        //    }
        //    catch (e) {
        //        //s_log.log(Level.SEVERE, sql.toString(), e);
        //    }
        //    return pairs;
        //}

        function initBPDetails(C_BPartner_ID) {

            baseObj.cmbShipment.getControl().html("");
            //var shipments = ;
            getShipments(VIS.Env.getCtx(), C_BPartner_ID);

            //for (var i = 0; i < shipments.length; i++) {
            //    if (i == 0) {
            //        baseObj.cmbShipment.getControl().append(" <option value=0> </option>");
            //    }
            //    baseObj.cmbShipment.getControl().append(" <option value=" + shipments[i].ID + ">" + shipments[i].value + "</option>");
            //};
            //baseObj.cmbShipment.getControl().prop('selectedIndex', 0);
        }

        this.disposeComponent = function () {
            baseObj = null;
            selfChild = null;
            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromInvoice, VIS.VCreateFrom);//Inherit from VCreateFrom

    VCreateFromInvoice.prototype.saveInvoice = function () {
        if (this.$super.dGrid == null) {
            return false;
        }

        var model = {};//this.$super.dGrid.records;
        var selectedItems = this.$super.multiValues;

        if (selectedItems == null) {
            return false;
        }
        if (selectedItems.length <= 0) {
            return false;
        }
        //var splitValue = selectedItems.toString().split(',');
        for (var i = 0; i < selectedItems.length; i++) {
            model[i] = (selectedItems[i]);
        }



        //	Get Shipment
        var C_Invoice_ID = this.$super.mTab.getValue("C_Invoice_ID");
        var C_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();
        var M_InOut_ID = this.$super.cmbShipment.getControl().find('option:selected').val();
        var C_ProvisionalInvoice_ID = this.$super.cmbProvisionalInvoice.getControl().find('option:selected').val();
        if (C_ProvisionalInvoice_ID == undefined || C_ProvisionalInvoice_ID == "")
            C_ProvisionalInvoice_ID = 0;

        return this.saveData(model, "", C_Order_ID, M_InOut_ID, C_Invoice_ID, C_ProvisionalInvoice_ID);
    }

    //Added by Bharat for new search filters

    VCreateFromInvoice.prototype.loadShipments = function (M_InOut_ID, M_Product_ID, pNo) {
        var data = this.getShipmentsData(VIS.Env.getCtx(), M_InOut_ID, M_Product_ID, pNo);
        //this.$super.loadGrid(data);
    }

    // Create 
    VCreateFromInvoice.prototype.getShipmentsData = function (ctx, M_InOut_ID, M_Product_ID, pNo) {

        var data = [];
        var self = this;
        this.$super.record_ID = $self.mTab.getValue("C_Invoice_ID");
        if (self.$super.dGrid != null) {
            var selection = self.$super.dGrid.getSelection();
            for (item in selection) {
                var obj = $.grep(self.$super.multiValues, function (n, i) {
                    return n.M_Product_ID_K == self.$super.dGrid.get(selection[item])["M_Product_ID_K"] && n.M_InOut_ID_K == self.$super.dGrid.get(selection[item])["M_InOut_ID_K"]
                });
                if (obj.length > 0) {

                }
                else {
                    self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
                }
            }
        }

        var isBaseLangage = "";
        var mProductIDs = "";
        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
            isBaseLangage = "FROM C_UOM uom INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ";
        }
        else {
            isBaseLangage = "FROM C_UOM_Trl uom Left join C_UOM uom1 on (uom1.C_UOM_ID=uom.C_UOM_ID)  INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx) + "') ";
        }
        if (M_Product_ID != null) {
            mProductIDs = " AND l.M_Product_ID=" + M_Product_ID;
        }
        if (!pNo) {
            pNo = 1;
        }

        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/GetDataVCreateFrom",
            dataType: "json",
            type: "POST",
            data: {
                keyColumnName: self.$super.mTab.keyColumnName,
                tableName: "M_InOutLine",
                recordID: self.$super.record_ID,
                pageNo: pNo,

                mInOutId: M_InOut_ID,
                isBaseLanguages: isBaseLangage,
                mProductIDD: mProductIDs,
            },
            error: function (e) {
                alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                self.$super.setBusy(false);
                selfChild.log.info(e);
                return;
            },
            success: function (dyndata) {

                var res = JSON.parse(dyndata);
                if (res.Error) {
                    VIS.ADialog.error(res.Error);
                    self.$super.setBusy(false);
                    return;
                }
                self.$super.resetPageCtrls(res.pSetting);
                self.$super.loadGrid(res.data);
                self.$super.setBusy(false);
            }
        });
        return data;
    }

    //VCreateFromInvoice.prototype.getShipmentsData = function (ctx, M_InOut_ID, M_Product_ID, pNo) {

    //    var data = [];
    //    var self = this;
    //    this.$super.record_ID = $self.mTab.getValue("C_Invoice_ID");
    //    if (self.$super.dGrid != null) {
    //        var selection = self.$super.dGrid.getSelection();
    //        for (item in selection) {
    //            var obj = $.grep(self.$super.multiValues, function (n, i) {
    //                return n.M_Product_ID_K == self.$super.dGrid.get(selection[item])["M_Product_ID_K"] && n.M_InOut_ID_K == self.$super.dGrid.get(selection[item])["M_InOut_ID_K"]
    //            });
    //            if (obj.length > 0) {

    //            }
    //            else {
    //                self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
    //            }
    //        }
    //    }
    //    var sql = ("SELECT " // QtyEntered                
    //            //+ "l.MovementQty-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.MovementQty,6),"
    //            + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //            + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QUANTITY,"	//	2
    //            + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //            + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QTYENTER,"	//	2
    //            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
    //            + " l.M_Product_ID,p.Name as Product, l.M_InOutLine_ID,l.Line," // 5..8
    //            + " l.C_OrderLine_ID, " // 9
    //            + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
    //            + " ins.description ");
    //    if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //        sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
    //    }
    //    else {
    //        sql = sql.concat("FROM C_UOM_Trl uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("') ");
    //    }
    //    sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ")
    //        .concat("LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) ")
    //        .concat("LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ")
    //        .concat("WHERE l.M_InOut_ID=" + M_InOut_ID); // #1
    //    if (M_Product_ID != null) {
    //        sql = sql.concat(" AND l.M_Product_ID=" + M_Product_ID);
    //    }
    //    sql = sql.concat(" GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
    //        + "l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description ").concat("ORDER BY l.Line");
    //    if (!pNo) {
    //        pNo = 1;
    //    }

    //    $.ajax({
    //        url: VIS.Application.contextUrl + "Common/GetData",
    //        dataType: "json",
    //        type: "POST",
    //        data: {
    //            sql: sql,
    //            keyColumnName: self.$super.mTab.keyColumnName,
    //            tableName: "M_InOutLine",
    //            recordID: self.$super.record_ID,
    //            pageNo: pNo
    //        },
    //        error: function () {
    //            alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
    //            self.$super.setBusy(false);
    //            return;
    //        },
    //        success: function (dyndata) {

    //            var res = JSON.parse(dyndata);
    //            if (res.Error) {
    //                VIS.ADialog.error(res.Error);
    //                self.$super.setBusy(false);
    //                return;
    //            }
    //            self.$super.resetPageCtrls(res.pSetting);
    //            self.$super.loadGrid(res.data);
    //            self.$super.setBusy(false);
    //        }
    //    });

    //    //try {

    //    //    dr = VIS.DB.executeReader(sql.toString(), null, null);
    //    //    var count = 1;
    //    //    while (dr.read()) {
    //    //        var line = {};

    //    //        //line.Add(false);           //  0-Selection
    //    //        var qtyMovement = dr.getDecimal(0);
    //    //        //var multiplier = dr.getDecimal(1);
    //    //        //var qtyEntered = qtyMovement * multiplier;
    //    //        var qtyEntered = dr.getDecimal(1);
    //    //        var select = false;
    //    //        var rec = 0;
    //    //        if (dr.getInt("m_inoutline_id") > 0) {
    //    //            sql = "SELECT Count(*) FROM C_InvoiceLine WHERE C_Invoice_ID = " + this.$super.mTab.getValue("C_Invoice_ID") + " AND M_InOutLine_ID = " + dr.getInt("m_inoutline_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        else {
    //    //            sql = "SELECT Count(*) FROM C_InvoiceLine WHERE C_Invoice_ID = " + this.$super.mTab.getValue("C_Invoice_ID") + " AND M_Product_ID = " + dr.getInt("m_product_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        line['Select'] = select;
    //    //        line['Quantity'] = qtyMovement;        //  1-Qty
    //    //        line['QuantityEntered'] = qtyEntered;  //  2-Qty Entered
    //    //        line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
    //    //        line['M_Product_ID'] = dr.getString(5); //  3-Product
    //    //        line['M_AttributeSetInstance_ID'] = dr.getString("description");
    //    //        line['C_Order_ID'] = ".";               //  4-Order
    //    //        line['M_InOut_ID'] = dr.getString(7);   //  5-Ship
    //    //        line['C_Invoice_ID'] = null;            //  6-Invoice

    //    //        line['C_UOM_ID_K'] = dr.getString(2);   //  2-UOM -Key
    //    //        line['M_Product_ID_K'] = dr.getInt(4);  //  3-Product -Key
    //    //        line['M_AttributeSetInstance_ID_K'] = dr.getString("m_attributesetinstance_id");
    //    //        line['C_Order_ID_K'] = dr.getInt(8);;   //  4-OrderLine -Key
    //    //        line['M_InOut_ID_K'] = dr.getInt(6);    //  5-Ship -Key
    //    //        line['C_Invoice_ID_K'] = null;          //  6-Invoice -Key

    //    //        line['recid'] = count;
    //    //        count++;
    //    //        data.push(line);
    //    //    }
    //    //    dr.close();
    //    //}
    //    //catch (e) {
    //    //    //s_log.log( Level.SEVERE, sql.toString(), e );
    //    //}
    //    return data;
    //}

    //End

    VCreateFromInvoice.prototype.loadShipment = function (M_InOut_ID) {
        var data = this.getShipmentData(VIS.Env.getCtx(), M_InOut_ID);
        this.$super.loadGrid(data);
    }

    /// Unused Function
    VCreateFromInvoice.prototype.getShipmentData = function (ctx, M_InOut_ID) {

        var data = [];


        var isBaseLanguageUmo = "";
        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
            isBaseLanguageUmo = "FROM C_UOM uom INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ";
        }
        else {
            isBaseLanguageUmo = "FROM C_UOM_Trl uom INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx) + "') ";
        }


        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/GetShipmentDatas",
            type: 'POST',
            async: false,
            data: { MInOutIDs: M_InOut_ID, isBaseLanguageUmos: isBaseLanguageUmo },
            success: function (data) {
                var ress = JSON.parse(data);
                if (ress && ress.length > 0) {
                    var count = 1;
                    for (var i = 0; i < ress.length; i++) {
                        var line = {};
                        var qtyMovement = dr.getDecimal(0);
                        var qtyEntered = dr.getDecimal(1);
                        line['Quantity'] = qtyMovement;
                        line['QuantityEntered'] = qtyEntered;
                        line['C_UOM_ID'] = dr.getString(3);
                        line['M_Product_ID'] = dr.getString(5);
                        line['C_Order_ID'] = ".";
                        line['M_InOut_ID'] = dr.getString(7);
                        line['C_Invoice_ID'] = null;

                        line['C_UOM_ID_K'] = dr.getString(2);
                        line['M_Product_ID_K'] = dr.getInt(4);
                        line['C_Order_ID_K'] = dr.getInt(8);;
                        line['M_InOut_ID_K'] = dr.getInt(6);
                        line['C_Invoice_ID_K'] = null;

                        line['recid'] = count;
                        count++;
                        data.push(line);
                    }
                }
            },
            error: function (e) {
            },
        });
        return data;
    }

    // Start Provisional Invoice
    VCreateFromInvoice.prototype.loadProvisionalInvoices = function (C_Invoice_ID, M_Product_ID, pNo) {
        var data = this.getProvisionalInvoicesData(VIS.Env.getCtx(), C_Invoice_ID, M_Product_ID, pNo);
    }

    VCreateFromInvoice.prototype.getProvisionalInvoicesData = function (ctx, C_Invoice_ID, M_Product_ID, pNo) {
        var data = [];
        var self = this;

        this.$super.record_ID = $self.mTab.getValue("C_Invoice_ID");
        if (self.$super.dGrid != null) {
            var selection = self.$super.dGrid.getSelection();
            for (item in selection) {
                var obj = $.grep(self.$super.multiValues, function (n, i) {
                    return n.M_Product_ID_K == self.$super.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Invoice_ID_K == self.$super.dGrid.get(selection[item])["C_Invoice_ID_K"]
                });
                if (obj.length > 0) {

                }
                else {
                    self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
                }
            }
        }

        var isBaseLangs = "";
        var mProductID = "";
        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {

            isBaseLangs = "FROM C_UOM uom INNER JOIN C_ProvisionalInvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ";
        }
        else {
            isBaseLangs = "FROM C_UOM_Trl uom Left join C_UOM uom1 on (uom1.C_UOM_ID=uom.C_UOM_ID) INNER JOIN C_ProvisionalInvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='"
                + VIS.Env.getAD_Language(ctx) + "') ";
        }
        if (M_Product_ID != null) {
            mProductID = " AND l.M_Product_ID = " + M_Product_ID;
        }

        if (!pNo) {
            pNo = 1;
        }
        var orgId = null
        if (self.$super.relatedToOrg != null && self.$super.relatedToOrg.getValue()) {
            orgId = self.$super.mTab.getValue("AD_Org_ID")
        }
        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/GetProvisionalInvoicesDataVCreate",
            dataType: "json",
            type: "POST",
            data: {
                keyColumnName: self.$super.mTab.keyColumnName,
                tableName: "C_ProvisionalInvoiceLine",
                recordID: self.$super.record_ID,
                pageNo: pNo,

                isBaseLangss: isBaseLangs,
                cInvoiceID: C_Invoice_ID,
                mProductIDs: mProductID,
                orgId: orgId
            },
            error: function (e) {
                alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                selfChild.log.info(e);
                self.$super.setBusy(false);
                return;
            },
            success: function (dyndata) {
                var res = JSON.parse(dyndata);
                if (res.Error) {
                    VIS.ADialog.error(res.Error);
                    self.$super.setBusy(false);
                    return;
                }
                self.$super.resetPageCtrls(res.pSetting);
                self.$super.loadGrid(res.data);
                self.$super.setBusy(false);
            }
        });
        return data;
    }



    //VCreateFromInvoice.prototype.getShipmentData = function (ctx, M_InOut_ID) {

    //    var data = [];
    //    var sql = ("SELECT " // QtyEntered                
    //            //+ "l.MovementQty-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.MovementQty,6),"
    //            + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //            + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QUANTITY,"	//	2
    //            + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //            + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QTYENTER,"	//	2
    //            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," // 3..4
    //            + " l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line," // 5..8
    //            + " l.C_OrderLine_ID "); // 9

    //    if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //        sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
    //    }
    //    else {
    //        sql = sql.concat("FROM C_UOM_Trl uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("') ");
    //    }
    //    sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ").concat(
    //            "LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) ").concat("WHERE l.M_InOut_ID=" + M_InOut_ID) // #1
    //            .concat(
    //                    " GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
    //                            + "l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID ").concat("ORDER BY l.Line");
    //    try {

    //        dr = VIS.DB.executeReader(sql.toString(), null, null);
    //        var count = 1;
    //        while (dr.read()) {
    //            var line = {};

    //            //line.Add(false);           //  0-Selection
    //            var qtyMovement = dr.getDecimal(0);
    //            //var multiplier = dr.getDecimal(1);
    //            //var qtyEntered = qtyMovement * multiplier;
    //            var qtyEntered = dr.getDecimal(1);
    //            line['Quantity'] = qtyMovement;        //  1-Qty
    //            line['QuantityEntered'] = qtyEntered;  //  2-Qty Entered
    //            line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
    //            line['M_Product_ID'] = dr.getString(5); //  3-Product
    //            line['C_Order_ID'] = ".";               //  4-Order
    //            line['M_InOut_ID'] = dr.getString(7);   //  5-Ship
    //            line['C_Invoice_ID'] = null;            //  6-Invoice

    //            line['C_UOM_ID_K'] = dr.getString(2);   //  2-UOM -Key
    //            line['M_Product_ID_K'] = dr.getInt(4);  //  3-Product -Key
    //            line['C_Order_ID_K'] = dr.getInt(8);;   //  4-OrderLine -Key
    //            line['M_InOut_ID_K'] = dr.getInt(6);    //  5-Ship -Key
    //            line['C_Invoice_ID_K'] = null;          //  6-Invoice -Key

    //            line['recid'] = count;
    //            count++;
    //            data.push(line);
    //        }
    //        dr.close();
    //    }
    //    catch (e) {
    //        //s_log.log( Level.SEVERE, sql.toString(), e );
    //    }
    //    return data;
    //}

    VCreateFromInvoice.prototype.saveData = function (model, selectedItems, C_Order_ID, M_inout_id, C_Invoice_ID, C_ProvisionalInvoice_ID) {
        var obj = this;
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveInvoice",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_Order_ID: C_Order_ID,
                C_Invoice_ID: C_Invoice_ID,
                M_inout_id: M_inout_id,
                C_ProvisionalInvoice_ID: C_ProvisionalInvoice_ID
            },
            success: function (data) {
                returnValue = data.result;
                if (returnValue) {
                    obj.dispose();
                    obj.$super.$root.dialog('close');
                    obj.$super.setBusy(false);
                    return;
                }
                obj.$super.setBusy(false);
                alert(returnValue);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                debugger;
                console.log(textStatus);
                obj.$super.setBusy(false);
                alert(errorThrown);
            }
        });

    }

    //dispose call
    VCreateFromInvoice.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };

    //Load form into VIS
    VIS.VCreateFromInvoice = VCreateFromInvoice;

})(VIS, jQuery);

