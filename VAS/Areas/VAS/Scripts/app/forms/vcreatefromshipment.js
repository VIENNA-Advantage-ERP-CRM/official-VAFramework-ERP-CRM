
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
            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "VAM_Inv_InOut_ID", false) + " .. " + VIS.Msg.translate(VIS.Env.getCtx(), "CreateFrom");

            // baseObj.lblShipment.visible = false;
            // baseObj.cmbShipment.visible = false;

            if (baseObj.lblShipment != null)
                baseObj.lblShipment.getControl().css('display', 'none');
            if (baseObj.cmbShipment != null)
                baseObj.cmbShipment.getControl().css('display', 'none');
            if (baseObj.Applybtn)
                baseObj.Applybtn.css("display", "block");
            var VAF_Column_ID = 3537;            //  VAM_Inv_InOut.VAM_Locator_ID
            var lookup = new VIS.MLocatorLookup(VIS.Env.getCtx(), baseObj.windowNo);
            baseObj.locatorField = new VIS.Controls.VLocator("VAM_Locator_ID", true, false, true, VIS.DisplayType.Locator, lookup);
            baseObj.deliveryDate = new VIS.Controls.VDate("DeliveryDate", false, false, true, VIS.DisplayType.Date, "DeliveryDate");
            var lookupProd = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), baseObj.windowNo, 2221, VIS.DisplayType.Search);
            baseObj.vProduct = new VIS.Controls.VTextBoxButton("VAM_Product_ID", true, false, true, VIS.DisplayType.Search, lookupProd);
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

            var VAB_BusinessPartner_ID = baseObj.initBPartner(false);
            baseObj.vBPartner.setReadOnly(true);
            initBPDetails(VAB_BusinessPartner_ID);
        }


        // Get Invoice Data
        function getInvoices(ctx, VAB_BusinessPartner_ID, isReturnTrx) {
            //var pairs = [];

            var display = ("i.DocumentNo||' - '||").concat(
                    VIS.DB.to_char("DateInvoiced", VIS.DisplayType.Date, VIS.Env.getVAF_Language(ctx))).concat("|| ' - ' ||")
                    .concat(VIS.DB.to_char("GrandTotal", VIS.DisplayType.Amount, VIS.Env.getVAF_Language(ctx)));
            // New column added to fill invoice which drop ship is true
            // Added by Vivek on 09/10/2017 advised by Pradeep
            var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsDropShip"));

            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetInvoicesVCreate",
                type: 'POST',
                //async: false,
                data: {
                    displays: display,
                    cBPartnerId: VAB_BusinessPartner_ID,
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


        //function getInvoices(ctx, VAB_BusinessPartner_ID, isReturnTrx) {
        //    var pairs = [];

        //    var display = ("i.DocumentNo||' - '||").concat(
        //            VIS.DB.to_char("DateInvoiced", VIS.DisplayType.Date, VIS.Env.getVAF_Language(ctx))).concat("|| ' - ' ||")
        //            .concat(VIS.DB.to_char("GrandTotal", VIS.DisplayType.Amount, VIS.Env.getVAF_Language(ctx)));

        //    //var sql = ("SELECT i.VAB_Invoice_ID,").concat(display).concat(
        //    //        " FROM VAB_Invoice i INNER JOIN VAB_DocTypes d ON (i.VAB_DocTypes_ID = d.VAB_DocTypes_ID) "
        //    //                + "WHERE i.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND i.IsSOTrx='N' "
        //    //                + "AND d.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' "
        //    //                + "AND i.DocStatus IN ('CL','CO')"
        //    //                + " AND i.VAB_Invoice_ID IN " + "(SELECT il.VAB_Invoice_ID FROM VAB_InvoiceLine il"
        //    //                + " LEFT OUTER JOIN VAM_MatchInvoice mi ON (il.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) "
        //    //                + "GROUP BY il.VAB_Invoice_ID,mi.VAB_InvoiceLine_ID,il.QtyInvoiced "
        //    //                + "HAVING (il.QtyInvoiced<>SUM(mi.Qty) AND mi.VAB_InvoiceLine_ID IS NOT NULL)"
        //    //                + " OR mi.VAB_InvoiceLine_ID IS NULL) " + "ORDER BY i.DateInvoiced");

        //    var sql = ("SELECT i.VAB_Invoice_ID,").concat(display).concat(
        //        " FROM VAB_Invoice i INNER JOIN VAB_DocTypes d ON (i.VAB_DocTypes_ID = d.VAB_DocTypes_ID) "
        //        + "WHERE i.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND i.IsSOTrx='N' "
        //        + "AND d.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' AND i.DocStatus IN ('CL','CO')"
        //        + " AND i.VAB_Invoice_ID IN "
        //        + "(SELECT VAB_Invoice_ID FROM (SELECT il.VAB_Invoice_ID,il.VAB_InvoiceLine_ID,il.QtyInvoiced,mi.Qty FROM VAB_InvoiceLine il "
        //        + "LEFT OUTER JOIN VAM_MatchInvoice mi ON (il.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) WHERE (il.QtyInvoiced <> nvl(mi.Qty,0) "
        //        + "AND mi.VAB_InvoiceLine_ID IS NOT NULL) OR mi.VAB_InvoiceLine_ID IS NULL ) GROUP BY VAB_Invoice_ID,VAB_InvoiceLine_ID,QtyInvoiced "
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

        function initBPDetails(VAB_BusinessPartner_ID) {
            baseObj.cmbInvoice.getControl().html("")
            var isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(baseObj.windowNo, "IsReturnTrx"));
            //var invoices =

            //// JID_0350: "When user create MR with refrence to order OR by invoice by using "Create Line From" charge should not shows on grid.
            getInvoices(VIS.Env.getCtx(), VAB_BusinessPartner_ID, isReturnTrx);

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

        var VAM_Locator_ID = loc;


        //	Get Shipment
        var VAM_Inv_InOut_ID = this.$super.mTab.getValue("VAM_Inv_InOut_ID");
        var VAB_Invoice_ID = this.$super.cmbInvoice.getControl().find('option:selected').val();
        var VAB_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();

        return this.saveData(model, "", VAB_Order_ID, VAB_Invoice_ID, VAM_Locator_ID, VAM_Inv_InOut_ID, Container_ID, fromApply);
    }

    // Added by Bharat for new search filters
    VCreateFromShipment.prototype.loadInvoices = function (VAB_Invoice_ID, VAM_Product_ID, pNo) {
        var data = this.getInvoicesData(VIS.Env.getCtx(), VAB_Invoice_ID, VAM_Product_ID, pNo);
        //this.$super.loadGrid(data);
    }


    VCreateFromShipment.prototype.getInvoicesData = function (ctx, VAB_Invoice_ID, VAM_Product_ID, pNo) {
        var data = [];
        var self = this;
        this.$super.record_ID = $self.mTab.getValue("VAM_Inv_InOut_ID");
        if (self.$super.dGrid != null) {
            var selection = self.$super.dGrid.getSelection();
            for (item in selection) {
                var obj = $.grep(self.$super.multiValues, function (n, i) {
                    return n.VAM_Product_ID_K == self.$super.dGrid.get(selection[item])["VAM_Product_ID_K"] && n.VAB_Invoice_ID_K == self.$super.dGrid.get(selection[item])["VAB_Invoice_ID_K"]
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
        if (VIS.Env.isBaseLanguage(ctx, "VAB_UOM")) {

            isBaseLangs = "FROM VAB_UOM uom INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID) ";
        }
        else {
            isBaseLangs = "FROM VAB_UOM_TL uom Left join VAB_UOM uom1 on (uom1.VAB_UOM_ID=uom.VAB_UOM_ID) INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID AND uom.VAF_Language='"
               + VIS.Env.getVAF_Language(ctx) + "') ";
        }
        if (VAM_Product_ID != null) {
            mProductID = " AND l.VAM_Product_ID = " + VAM_Product_ID;
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
                tableName: "VAB_InvoiceLine",
                recordID: self.$super.record_ID,
                pageNo: pNo,

                isBaseLangss: isBaseLangs,
                cInvoiceID: VAB_Invoice_ID,
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

    VCreateFromShipment.prototype.initContainerDetails = function (VAM_Locator_ID) {
        this.$super.cmbContainer.getControl().html("");
    }


    //VCreateFromShipment.prototype.getInvoicesData = function (ctx, VAB_Invoice_ID, VAM_Product_ID, pNo) {
    //    var data = [];
    //    var self = this;
    //    this.$super.record_ID = $self.mTab.getValue("VAM_Inv_InOut_ID");
    //    if (self.$super.dGrid != null) {
    //        var selection = self.$super.dGrid.getSelection();
    //        for (item in selection) {
    //            var obj = $.grep(self.$super.multiValues, function (n, i) {
    //                return n.VAM_Product_ID_K == self.$super.dGrid.get(selection[item])["VAM_Product_ID_K"] && n.VAB_Invoice_ID_K == self.$super.dGrid.get(selection[item])["VAB_Invoice_ID_K"]
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
    //        + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
    //        + " l.VAM_Product_ID,p.Name as PRODUCT, l.VAB_InvoiceLine_ID,l.Line,"      //  5..8
    //        + " l.VAB_OrderLine_ID,"                   					//  9
    //        + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
    //        + " ins.description ");
    //    if (VIS.Env.isBaseLanguage(ctx, "VAB_UOM")) {

    //        sql = sql.concat("FROM VAB_UOM uom ").concat("INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID) ");
    //    }
    //    else {
    //        sql = sql.concat("FROM VAB_UOM_TL uom ")
    //           .concat("INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID AND uom.VAF_Language='")
    //           .concat(VIS.Env.getVAF_Language(ctx)).concat("') ");
    //    }
    //    sql = sql.concat("INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID) ")
    //        .concat("LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) ")
    //        .concat("LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) ")
    //        .concat("WHERE l.VAB_Invoice_ID=" + VAB_Invoice_ID); 									//  #1
    //    if (VAM_Product_ID != null) {
    //        sql = sql.concat(" AND l.VAM_Product_ID = " + VAM_Product_ID);
    //    }
    //    sql = sql.concat(" GROUP BY l.QtyInvoiced,l.QtyEntered,"
    //    + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
    //        + "l.VAM_Product_ID,p.Name, l.VAB_InvoiceLine_ID,l.Line,l.VAB_OrderLine_ID,l.VAM_PFeature_SetInstance_ID,ins.description "
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
    //            tableName: "VAB_InvoiceLine",
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
    //    //        if (dr.getInt("VAB_Orderline_id") > 0) {
    //    //            sql = "SELECT Count(*) FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + this.$super.mTab.getValue("VAM_Inv_InOut_ID") + " AND VAB_OrderLine_ID = " + dr.getInt("VAB_Orderline_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        else {
    //    //            sql = "SELECT Count(*) FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + this.$super.mTab.getValue("VAM_Inv_InOut_ID") + " AND VAM_Product_ID = " + dr.getInt("VAM_Product_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        line['Select'] = select;
    //    //        line['Quantity'] = qtyInvoiced;        //  1-Qty
    //    //        line['QuantityEntered'] = qtyEntered;  //  2-Qty Entered
    //    //        line['VAB_UOM_ID'] = dr.getString(3);     //  2-UOM
    //    //        line['VAM_Product_ID'] = dr.getString(5); //  3-Product
    //    //        line['VAM_PFeature_SetInstance_ID'] = dr.getString("description");
    //    //        line['VAB_Order_ID'] = ".";      //  4-Order
    //    //        line['VAM_Inv_InOut_ID'] = null;        //  5-Ship
    //    //        line['VAB_Invoice_ID'] = dr.getString(7);        //  6-Invoice

    //    //        line['VAB_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
    //    //        line['VAM_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
    //    //        line['VAM_PFeature_SetInstance_ID_K'] = dr.getString("VAM_PFeature_SetInstance_id");
    //    //        line['VAB_Order_ID_K'] = dr.getInt(8);;      //  4-OrderLine -Key
    //    //        line['VAM_Inv_InOut_ID_K'] = null;        //  5-Ship -Key
    //    //        line['VAB_Invoice_ID_K'] = dr.getInt(6);        //  6-Invoice -Key

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

    VCreateFromShipment.prototype.loadInvoice = function (VAB_Invoice_ID) {
        var data = this.getInvoiceData(VIS.Env.getCtx(), VAB_Invoice_ID);
        this.$super.loadGrid(data);
    }

    VCreateFromShipment.prototype.getInvoiceData = function (ctx, VAB_Invoice_ID) {
        var data = [];
        var sql = ("SELECT "	//	Entered UOM
            //+ "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QUANTITY,"	//	2
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QTYENTER,"	//	2
            + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"			//  3..4
            + " l.VAM_Product_ID,p.Name, l.VAB_InvoiceLine_ID,l.Line,"      //  5..8
            + " l.VAB_OrderLine_ID ");                   					//  9

        if (VIS.Env.isBaseLanguage(ctx, "VAB_UOM")) {

            sql = sql.concat("FROM VAB_UOM uom ").concat("INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID) ");
        }
        else {
            sql = sql.concat("FROM VAB_UOM_TL uom ")
               .concat("INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID AND uom.VAF_Language='")
               .concat(VIS.Env.getVAF_Language(ctx)).concat("') ");
        }
        sql = sql.concat("INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID) ")
           .concat("LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) ")
           .concat("WHERE l.VAB_Invoice_ID=" + VAB_Invoice_ID 									//  #1
            + " GROUP BY l.QtyInvoiced,l.QtyEntered,"
            + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.VAM_Product_ID,p.Name, l.VAB_InvoiceLine_ID,l.Line,l.VAB_OrderLine_ID "
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
                line['VAB_UOM_ID'] = dr.getString(3);     //  2-UOM
                line['VAM_Product_ID'] = dr.getString(5); //  3-Product
                line['VAB_Order_ID'] = ".";      //  4-Order
                line['VAM_Inv_InOut_ID'] = null;        //  5-Ship
                line['VAB_Invoice_ID'] = dr.getString(7);        //  6-Invoice

                line['VAB_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
                line['VAM_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
                line['VAB_Order_ID_K'] = dr.getInt(8);;      //  4-OrderLine -Key
                line['VAM_Inv_InOut_ID_K'] = null;        //  5-Ship -Key
                line['VAB_Invoice_ID_K'] = dr.getInt(6);        //  6-Invoice -Key

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

    VCreateFromShipment.prototype.saveData = function (model, selectedItems, VAB_Order_ID, VAB_Invoice_ID, VAM_Locator_id, VAM_Inv_InOut_id, Container_ID, fromApply) {
        var obj = this;
        console.log(VAM_Locator_id);
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveShipment",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                VAB_Order_ID: VAB_Order_ID,
                VAB_Invoice_ID: VAB_Invoice_ID,
                VAM_Locator_ID: VAM_Locator_id,
                VAM_Inv_InOut_ID: VAM_Inv_InOut_id,
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

