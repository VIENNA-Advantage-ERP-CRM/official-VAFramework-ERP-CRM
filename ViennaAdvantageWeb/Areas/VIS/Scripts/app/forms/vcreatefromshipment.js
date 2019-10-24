
; (function (VIS, $) {

    //form declaretion
    function VCreateFromShipment(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var selfChild = this;
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;

        // create log
        this.log = VIS.Logging.VLogger.getVLogger("VCreateFromShipment");


        function dynInit() {
            //DynInit
            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "M_InOut_ID", false) + " .. " + VIS.Msg.translate(VIS.Env.getCtx(), "CreateFrom");

            // baseObj.lblShipment.visible = false;
            // baseObj.cmbShipment.visible = false;

            if (baseObj.lblShipment != null)
                baseObj.lblShipment.getControl().css('display', 'none');
            if (baseObj.cmbShipment != null)
                baseObj.cmbShipment.getControl().css('display', 'none');
            if (baseObj.Applybtn)
                baseObj.Applybtn.css("display", "block");
            var AD_Column_ID = 3537;            //  M_InOut.M_Locator_ID
            var lookup = new VIS.MLocatorLookup(VIS.Env.getCtx(), baseObj.windowNo);
            baseObj.locatorField = new VIS.Controls.VLocator("M_Locator_ID", true, false, true, VIS.DisplayType.Locator, lookup);
            baseObj.deliveryDate = new VIS.Controls.VDate("DeliveryDate", false, false, true, VIS.DisplayType.Date, "DeliveryDate");
            var lookupProd = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), baseObj.windowNo, 2221, VIS.DisplayType.Search);
            baseObj.vProduct = new VIS.Controls.VTextBoxButton("M_Product_ID", true, false, true, VIS.DisplayType.Search, lookupProd);
            baseObj.DocumentNoRef = new VIS.Controls.VTextBox("DocumentNo", false, true, true, 50, 100, null, null, false);
            // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
            baseObj.lblContainer = new VIS.Controls.VLabel();
            baseObj.cmbContainer = new VIS.Controls.VComboBox('', false, false, true);
            baseObj.lblContainer.getControl().text(VIS.Msg.getMsg('VIS_Container'));

            if (baseObj.lblContainer != null)
                baseObj.lblContainer.getControl().css('display', 'block');
            if (baseObj.cmbContainer != null)
                baseObj.cmbContainer.getControl().css('display', 'block');

            //  Set Default
            // JID_1430 : On create line from form system is not loading the Document no. field showing NAN in field only.
            var DocumentNos = VIS.Env.getCtx().getContext(baseObj.windowNo, "DocumentNo");
            baseObj.DocumentNoRef.setValue(DocumentNos);

            var C_BPartner_ID = baseObj.initBPartner(false);
            baseObj.vBPartner.setReadOnly(true);
            initBPDetails(C_BPartner_ID);
        }


        // Get Invoice Data
        function getInvoices(ctx, C_BPartner_ID, isReturnTrx) {
            //var pairs = [];

            var display = ("i.DocumentNo||' - '||").concat(
                    VIS.DB.to_char("DateInvoiced", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("|| ' - ' ||")
                    .concat(VIS.DB.to_char("GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));
            // New column added to fill invoice which drop ship is true
            // Added by Vivek on 09/10/2017 advised by Pradeep
            var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsDropShip"));

            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetInvoicesVCreate",
                type: 'POST',
                //async: false,
                data: {
                    displays: display,
                    cBPartnerId: C_BPartner_ID,
                    isReturnTrxs: isReturnTrx,
                    IsDrops: _isdrop
                },
                success: function (data) {
                    var ress = JSON.parse(data);
                    if (ress && ress.length > 0) {
                        try {
                            var key, value;
                            for (var i = 0; i < ress.length; i++) {
                                key = VIS.Utility.Util.getValueOfInt(ress[i].key);
                                value = VIS.Utility.encodeText(ress[i].value);
                                //pairs.push({ ID: key, value: value });

                                if (i == 0) {
                                    baseObj.cmbInvoice.getControl().append(" <option value=0> </option>");
                                }
                                baseObj.cmbInvoice.getControl().append(" <option value=" + key + ">" + value + "</option>");
                            }
                            baseObj.cmbInvoice.getControl().prop('selectedIndex', 0);
                        }
                        catch (e) {

                        }
                    }
                },
                error: function (e) {
                    selfChild.log.info(e);
                },
            });
            //return pairs;
        }


        //function getInvoices(ctx, C_BPartner_ID, isReturnTrx) {
        //    var pairs = [];

        //    var display = ("i.DocumentNo||' - '||").concat(
        //            VIS.DB.to_char("DateInvoiced", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("|| ' - ' ||")
        //            .concat(VIS.DB.to_char("GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

        //    //var sql = ("SELECT i.C_Invoice_ID,").concat(display).concat(
        //    //        " FROM C_Invoice i INNER JOIN C_DocType d ON (i.C_DocType_ID = d.C_DocType_ID) "
        //    //                + "WHERE i.C_BPartner_ID=" + C_BPartner_ID + " AND i.IsSOTrx='N' "
        //    //                + "AND d.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' "
        //    //                + "AND i.DocStatus IN ('CL','CO')"
        //    //                + " AND i.C_Invoice_ID IN " + "(SELECT il.C_Invoice_ID FROM C_InvoiceLine il"
        //    //                + " LEFT OUTER JOIN M_MatchInv mi ON (il.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
        //    //                + "GROUP BY il.C_Invoice_ID,mi.C_InvoiceLine_ID,il.QtyInvoiced "
        //    //                + "HAVING (il.QtyInvoiced<>SUM(mi.Qty) AND mi.C_InvoiceLine_ID IS NOT NULL)"
        //    //                + " OR mi.C_InvoiceLine_ID IS NULL) " + "ORDER BY i.DateInvoiced");

        //    var sql = ("SELECT i.C_Invoice_ID,").concat(display).concat(
        //        " FROM C_Invoice i INNER JOIN C_DocType d ON (i.C_DocType_ID = d.C_DocType_ID) "
        //        + "WHERE i.C_BPartner_ID=" + C_BPartner_ID + " AND i.IsSOTrx='N' "
        //        + "AND d.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' AND i.DocStatus IN ('CL','CO')"
        //        + " AND i.C_Invoice_ID IN "
        //        + "(SELECT C_Invoice_ID FROM (SELECT il.C_Invoice_ID,il.C_InvoiceLine_ID,il.QtyInvoiced,mi.Qty FROM C_InvoiceLine il "
        //        + "LEFT OUTER JOIN M_MatchInv mi ON (il.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) WHERE (il.QtyInvoiced <> nvl(mi.Qty,0) "
        //        + "AND mi.C_InvoiceLine_ID IS NOT NULL) OR mi.C_InvoiceLine_ID IS NULL ) GROUP BY C_Invoice_ID,C_InvoiceLine_ID,QtyInvoiced "
        //        + "HAVING QtyInvoiced > SUM(nvl(Qty,0))) ORDER BY i.DateInvoiced");

        //    var dr = null;
        //    try {
        //        dr = VIS.DB.executeReader(sql.toString(), null, null);
        //        while (dr.read()) {
        //            key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
        //            value = dr.getString(1);
        //            pairs.push({ ID: key, value: value });
        //        }
        //        dr.close();
        //    }
        //    catch (e) {
        //        dr.close();
        //    }
        //    return pairs;
        //}

        function initBPDetails(C_BPartner_ID) {
            baseObj.cmbInvoice.getControl().html("")
            var isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(baseObj.windowNo, "IsReturnTrx"));
            //var invoices =

            //// JID_0350: "When user create MR with refrence to order OR by invoice by using "Create Line From" charge should not shows on grid.
            getInvoices(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx);

            //for (var i = 0; i < invoices.length; i++) {
            //    if (i == 0) {
            //        baseObj.cmbInvoice.getControl().append(" <option value=0> </option>");
            //    }
            //    baseObj.cmbInvoice.getControl().append(" <option value=" + invoices[i].ID + ">" + invoices[i].value + "</option>");
            //};
            //baseObj.cmbInvoice.getControl().prop('selectedIndex', 0);
        };

        this.disposeComponent = function () {
            baseObj = null;
            selfChild = null;
            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromShipment, VIS.VCreateFrom);//Inherit from VCreateFrom

    VCreateFromShipment.prototype.saveMInOut = function (fromApply) {
        debugger;
        if (this.$super.dGrid == null) {
            this.$super.setBusy(false);
            return false;
        }
        var model = {};//this.$super.dGrid.records;
        var selectedItems = this.$super.multiValues;

        if (selectedItems == null) {
            this.$super.setBusy(false);
            return false;
        }
        if (selectedItems.length <= 0) {
            this.$super.setBusy(false);
            return false;
        }
        //var splitValue = selectedItems.toString().split(',');
        for (var i = 0; i < selectedItems.length; i++) {
            model[i] = (selectedItems[i]);
        }

        var loc = this.$super.locatorField.getValue();

        if (loc == null || loc == 0) {
            //alert("Locator Field is Mandatory");
            VIS.ADialog.info("Error", true, VIS.Msg.translate(VIS.Env.getCtx(), "VIS_locfieldMandatory"), "");
            this.$super.setBusy(false);
            return false;
        }
        if (model.length == 0) {
            this.$super.setBusy(false);
            return false;
        }

        var Container_ID = this.$super.cmbContainer.getValue();
        if (Container_ID === undefined)
            Container_ID = 0;

        var M_Locator_ID = loc;


        //	Get Shipment
        var M_InOut_ID = this.$super.mTab.getValue("M_InOut_ID");
        var C_Invoice_ID = this.$super.cmbInvoice.getControl().find('option:selected').val();
        var C_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();

        return this.saveData(model, "", C_Order_ID, C_Invoice_ID, M_Locator_ID, M_InOut_ID, Container_ID, fromApply);
    }

    // Added by Bharat for new search filters
    VCreateFromShipment.prototype.loadInvoices = function (C_Invoice_ID, M_Product_ID, pNo) {
        var data = this.getInvoicesData(VIS.Env.getCtx(), C_Invoice_ID, M_Product_ID, pNo);
        //this.$super.loadGrid(data);
    }


    VCreateFromShipment.prototype.getInvoicesData = function (ctx, C_Invoice_ID, M_Product_ID, pNo) {
        var data = [];
        var self = this;
        this.$super.record_ID = $self.mTab.getValue("M_InOut_ID");
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

            isBaseLangs = "FROM C_UOM uom INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ";
        }
        else {
            isBaseLangs = "FROM C_UOM_Trl uom Left join C_UOM uom1 on (uom1.C_UOM_ID=uom.C_UOM_ID) INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='"
               + VIS.Env.getAD_Language(ctx) + "') ";
        }
        if (M_Product_ID != null) {
            mProductID = " AND l.M_Product_ID = " + M_Product_ID;
        }

        if (!pNo) {
            pNo = 1;
        }

        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/GetInvoicesDataVCreate",
            dataType: "json",
            type: "POST",
            data: {
                keyColumnName: self.$super.mTab.keyColumnName,
                tableName: "C_InvoiceLine",
                recordID: self.$super.record_ID,
                pageNo: pNo,

                isBaseLangss: isBaseLangs,
                cInvoiceID: C_Invoice_ID,
                mProductIDs: mProductID
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

    VCreateFromShipment.prototype.initContainerDetails = function (M_Locator_ID) {
        this.$super.cmbContainer.getControl().html("");
    }


    //VCreateFromShipment.prototype.getInvoicesData = function (ctx, C_Invoice_ID, M_Product_ID, pNo) {
    //    var data = [];
    //    var self = this;
    //    this.$super.record_ID = $self.mTab.getValue("M_InOut_ID");
    //    if (self.$super.dGrid != null) {
    //        var selection = self.$super.dGrid.getSelection();
    //        for (item in selection) {
    //            var obj = $.grep(self.$super.multiValues, function (n, i) {
    //                return n.M_Product_ID_K == self.$super.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Invoice_ID_K == self.$super.dGrid.get(selection[item])["C_Invoice_ID_K"]
    //            });
    //            if (obj.length > 0) {

    //            }
    //            else {
    //                self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
    //            }
    //        }
    //    }
    //    var sql = ("SELECT "	//	Entered UOM
    //        //+ "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
    //        + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //        + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QUANTITY,"	//	2
    //        + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
    //        + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QTYENTER,"	//	2
    //        + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
    //        + " l.M_Product_ID,p.Name as PRODUCT, l.C_InvoiceLine_ID,l.Line,"      //  5..8
    //        + " l.C_OrderLine_ID,"                   					//  9
    //        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
    //        + " ins.description ");
    //    if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {

    //        sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
    //    }
    //    else {
    //        sql = sql.concat("FROM C_UOM_Trl uom ")
    //           .concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='")
    //           .concat(VIS.Env.getAD_Language(ctx)).concat("') ");
    //    }
    //    sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ")
    //        .concat("LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) ")
    //        .concat("LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ")
    //        .concat("WHERE l.C_Invoice_ID=" + C_Invoice_ID); 									//  #1
    //    if (M_Product_ID != null) {
    //        sql = sql.concat(" AND l.M_Product_ID = " + M_Product_ID);
    //    }
    //    sql = sql.concat(" GROUP BY l.QtyInvoiced,l.QtyEntered,"
    //    + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
    //        + "l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description "
    //    + "ORDER BY l.Line");

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
    //            tableName: "C_InvoiceLine",
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
    //    //var dr = null;
    //    //try {
    //    //    dr = VIS.DB.executeReader(sql.toString(), null, null);
    //    //    var count = 1;
    //    //    while (dr.read()) {
    //    //        var line = {};
    //    //        //line.Add(false);           //  0-Selection
    //    //        var qtyInvoiced = dr.getDecimal(0);
    //    //        //var multiplier = dr.getDecimal(1);
    //    //        var qtyEntered = dr.getDecimal(1);
    //    //        var select = false;
    //    //        var rec = 0;
    //    //        if (dr.getInt("c_orderline_id") > 0) {
    //    //            sql = "SELECT Count(*) FROM M_InOutLine WHERE M_InOut_ID = " + this.$super.mTab.getValue("M_InOut_ID") + " AND C_OrderLine_ID = " + dr.getInt("c_orderline_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        else {
    //    //            sql = "SELECT Count(*) FROM M_InOutLine WHERE M_InOut_ID = " + this.$super.mTab.getValue("M_InOut_ID") + " AND M_Product_ID = " + dr.getInt("m_product_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        line['Select'] = select;
    //    //        line['Quantity'] = qtyInvoiced;        //  1-Qty
    //    //        line['QuantityEntered'] = qtyEntered;  //  2-Qty Entered
    //    //        line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
    //    //        line['M_Product_ID'] = dr.getString(5); //  3-Product
    //    //        line['M_AttributeSetInstance_ID'] = dr.getString("description");
    //    //        line['C_Order_ID'] = ".";      //  4-Order
    //    //        line['M_InOut_ID'] = null;        //  5-Ship
    //    //        line['C_Invoice_ID'] = dr.getString(7);        //  6-Invoice

    //    //        line['C_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
    //    //        line['M_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
    //    //        line['M_AttributeSetInstance_ID_K'] = dr.getString("m_attributesetinstance_id");
    //    //        line['C_Order_ID_K'] = dr.getInt(8);;      //  4-OrderLine -Key
    //    //        line['M_InOut_ID_K'] = null;        //  5-Ship -Key
    //    //        line['C_Invoice_ID_K'] = dr.getInt(6);        //  6-Invoice -Key

    //    //        line['recid'] = count;
    //    //        count++;
    //    //        data.push(line);
    //    //    }
    //    //    dr.close();
    //    //}
    //    //catch (e) {
    //    //    //s_log.log(Level.SEVERE, sql.toString(), e);
    //    //}
    //    return data;
    //}
    // end 

    VCreateFromShipment.prototype.loadInvoice = function (C_Invoice_ID) {
        var data = this.getInvoiceData(VIS.Env.getCtx(), C_Invoice_ID);
        this.$super.loadGrid(data);
    }

    VCreateFromShipment.prototype.getInvoiceData = function (ctx, C_Invoice_ID) {
        var data = [];
        var sql = ("SELECT "	//	Entered UOM
            //+ "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QUANTITY,"	//	2
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QTYENTER,"	//	2
            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"			//  3..4
            + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,"      //  5..8
            + " l.C_OrderLine_ID ");                   					//  9

        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {

            sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
        }
        else {
            sql = sql.concat("FROM C_UOM_Trl uom ")
               .concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='")
               .concat(VIS.Env.getAD_Language(ctx)).concat("') ");
        }
        sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ")
           .concat("LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) ")
           .concat("WHERE l.C_Invoice_ID=" + C_Invoice_ID 									//  #1
            + " GROUP BY l.QtyInvoiced,l.QtyEntered,"
            + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID "
            + "ORDER BY l.Line");

        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql.toString(), null, null);
            var count = 1;
            while (dr.read()) {
                var line = {};

                //line.Add(false);           //  0-Selection
                var qtyInvoiced = dr.getDecimal(0);
                //var multiplier = dr.getDecimal(1);
                var qtyEntered = dr.getDecimal(1);

                line['Quantity'] = qtyInvoiced;        //  1-Qty
                line['QuantityEntered'] = qtyEntered;  //  2-Qty Entered
                line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
                line['M_Product_ID'] = dr.getString(5); //  3-Product
                line['C_Order_ID'] = ".";      //  4-Order
                line['M_InOut_ID'] = null;        //  5-Ship
                line['C_Invoice_ID'] = dr.getString(7);        //  6-Invoice

                line['C_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
                line['M_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
                line['C_Order_ID_K'] = dr.getInt(8);;      //  4-OrderLine -Key
                line['M_InOut_ID_K'] = null;        //  5-Ship -Key
                line['C_Invoice_ID_K'] = dr.getInt(6);        //  6-Invoice -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
            dr.close();
        }
        catch (e) {
            //s_log.log(Level.SEVERE, sql.toString(), e);
        }
        return data;
    }

    VCreateFromShipment.prototype.saveData = function (model, selectedItems, C_Order_ID, C_Invoice_ID, m_locator_id, M_inout_id, Container_ID, fromApply) {
        var obj = this;
        console.log(m_locator_id);
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveShipment",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_Order_ID: C_Order_ID,
                C_Invoice_ID: C_Invoice_ID,
                M_Locator_ID: m_locator_id,
                M_InOut_ID: M_inout_id,
                Container_ID: Container_ID
            },
            success: function (data) {
                returnValue = data.result;
                if (returnValue) {
                    // Change By Mohit 30/06/2016
                    if (!fromApply) {
                        obj.dispose();
                        obj.$super.setBusy(false);
                        //VIS.ADialog.info("VIS_SuccessFullyInserted", null, null, null);
                        obj.$super.$root.dialog('close');
                    }
                    else {
                        obj.$super.callBackSave(returnValue);
                    }
                    return;
                }
                //obj.$super.setBusy(false);
                //alert(returnValue);
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
    VCreateFromShipment.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };



    //Load form into VIS
    VIS.VCreateFromShipment = VCreateFromShipment;

})(VIS, jQuery);

