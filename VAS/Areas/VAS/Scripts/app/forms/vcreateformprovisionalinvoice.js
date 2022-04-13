
; (function (VIS, $) {

    //Provisonal Invoice form declaration
    function VCreateFormProvisionalInvoice(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var selfChild = this;
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;
        var windowNo = tab.getWindowNo();

        // create Log
        this.log = VIS.Logging.VLogger.getVLogger("VCreateFormProvisionalInvoice");

        /** Intialize Create Line form for Provisonal Invoice */
        function dynInit() {
            //DynInit
            baseObj.title = VIS.Msg.getMsg("ProvisionalInvoice") + " .. " + VIS.Msg.getMsg("CreateFrom");
            baseObj.relatedToOrg = null;
            baseObj.fromInvoice = false;
            baseObj.ProvisionalInvoice = true;
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
            var C_BPartner_ID = baseObj.initBPartnerForProvisional();
            baseObj.vBPartner.setReadOnly(true);
            initBPDetails(C_BPartner_ID);


            // load grid structure
            selfChild.getTableFieldVOs();

            return true;
        }

        /* Intialize Shipment/Receipt Detail */
        function initBPDetails(C_BPartner_ID) {
            baseObj.cmbShipment.getControl().html("");
            getShipments(VIS.Env.getCtx(), C_BPartner_ID);
        }

        /* Load Shipment/Receipt Combo data */
        function getShipments(ctx, C_BPartner_ID) {
            //var pairs = [];

            var display = ("s.DocumentNo||' - '||")
                .concat(VIS.DB.to_char("s.MovementDate", VIS.DisplayType.Date, VIS.Env.getAD_Language(VIS.Env.getCtx())));

            var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsDropShip"));
            var _isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(selfChild.windowNo, "IsSOTrx"));

            //VA230:Get IsReturnTrx based on document type on provisional invoice
            var dt = VIS.dataContext.getJSONRecord("MDocType/GetDocType", VIS.Env.getCtx().getContextAsInt(selfChild.windowNo, "C_DocType_ID").toString());
            var isReturnTrx = VIS.Utility.Util.getValueOfBoolean(dt["IsReturnTrx"]);

            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetShipmentsData",
                type: 'POST',
                //async: false,
                data: {
                    displays: display, CBPartnerIDs: C_BPartner_ID, IsDrop: _isdrop, IsSOTrx: _isSoTrx, isReturnTrxs: isReturnTrx, isProvisionlInvoices: true
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
        }

        /* Dispose Components */
        this.disposeComponent = function () {
            baseObj = null;
            selfChild = null;
            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFormProvisionalInvoice, VIS.VCreateFrom);//Inherit from VCreateFrom

    /* Create Grid Columsn for Proviosnal Invoice */
    VCreateFormProvisionalInvoice.prototype.getTableFieldVOs = function () {
        var baseObj = this.$super;
        var self = this;
        baseObj.arrListColumns = [];
        baseObj.arrListColumns.push({
            field: "Quantity", caption: VIS.Msg.getMsg("QtyPending"), sortable: false, size: '150px', hidden: false,
            render: function (record, index, col_index) {
                var val = record["Quantity"];
                return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
            }
        });
        baseObj.arrListColumns.push({ field: "QuantityPending", caption: VIS.Msg.getMsg("Quantity"), sortable: false, size: '150px', render: 'number:4', hidden: true });
        baseObj.arrListColumns.push({
            field: "QuantityEntered", caption: VIS.Msg.getMsg("QtyEntered"), sortable: false, size: '150px', hidden: false,
            render: function (record, index, col_index) {
                var val = record["QuantityEntered"];
                val = baseObj.checkcommaordot(event, val);
                return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 }); /* editable: { type: 'number' },*/
            }
        });
        baseObj.arrListColumns.push({
            field: "POPrice", caption: VIS.Msg.getMsg("POPrice"), sortable: false, size: '150px', hidden: false,
            render: function (record, index, col_index) {
                var val = record["POPrice"];
                return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
            }
        });
        baseObj.arrListColumns.push({
            field: "ProvisionalPrice", caption: VIS.Msg.getMsg("ProvisionalPrice"), sortable: false, size: '150px', hidden: false,
            render: function (record, index, col_index) {
                var val = record["ProvisionalPrice"];
                return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
            }
        });

        baseObj.arrListColumns.push({ field: "C_UOM_ID", caption: VIS.Msg.getMsg("UomName"), sortable: false, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "M_Product_SearchKey", caption: VIS.Msg.getMsg("ProductSearchKey"), sortable: false, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "M_Product_ID", caption: VIS.Msg.getMsg("ProductName"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({
            field: "M_AttributeSetInstance_ID", caption: VIS.Msg.getMsg("ASI"), sortable: false, size: '150px', editable: { type: 'custom', ctrl: self.$productAttribute, showAll: true, style: 'background-color: #f8f8f8;' },
            render: function (record, index, col_index) {
                var l = self.pattrLookup;
                var val = record["M_AttributeSetInstance_ID"];
                if (record.changes && typeof record.changes["M_AttributeSetInstance_ID"] != 'undefined') {
                    val = record.changes["M_AttributeSetInstance_ID"];
                }
                var d;
                if (l) {
                    d = l.getDisplay(val, true);
                }
                return d;
            },
            style: 'background-color: #d1d1d14a'
        });
        baseObj.arrListColumns.push({ field: "C_PaymentTerm_ID", caption: VIS.Msg.getMsg("C_PaymentTerm_ID"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "PaymentTermName", caption: VIS.Msg.getMsg("PaymentTermName"), sortable: false, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "IsAdvance", caption: VIS.Msg.getMsg("IsAdvance"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_InvoicePaymentTerm_ID", caption: VIS.Msg.getMsg("C_InvoicePaymentTerm_ID"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "IsInvoicePTAdvance", caption: VIS.Msg.getMsg("IsInvoicePTAdvance"), sortable: false, size: '150px', hidden: true });

        baseObj.arrListColumns.push({ field: "C_Order_ID", caption: VIS.Msg.getMsg("OrderLine"), sortable: false, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "M_InOut_ID", caption: VIS.Msg.getMsg("Shipment/Receipt"), sortable: false, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_Invoice_ID", caption: VIS.Msg.getMsg("Invoice"), sortable: false, size: '150px', hidden: false });
        // Hidden -- > true
        baseObj.arrListColumns.push({ field: "C_UOM_ID_K", caption: VIS.Msg.getMsg("UomName"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "M_Product_ID_K", caption: VIS.Msg.getMsg("ProductName"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_Order_ID_K", caption: VIS.Msg.getMsg("Order"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "M_InOut_ID_K", caption: VIS.Msg.getMsg("Shipment/Receipt"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_Invoice_ID_K", caption: VIS.Msg.getMsg("Invoice"), sortable: false, size: '150px', hidden: true });
        baseObj.arrListColumns.push({
            field: "Select", caption: VIS.Msg.getMsg("Select"), sortable: false, size: '50px', hidden: true,
            render: function () {
                return '<div><input type=checkbox disabled="true"></div>';
            }
        });
    }

    /* Save Provisional Invoice Line */
    VCreateFormProvisionalInvoice.prototype.saveProvisionalInvoice = function () {
        if (this.$super.dGrid == null) {
            return false;
        }

        var model = {};
        var selectedItems = this.$super.multiValues;

        if (selectedItems == null) {
            return false;
        }
        if (selectedItems.length <= 0) {
            return false;
        }
        for (var i = 0; i < selectedItems.length; i++) {
            model[i] = (selectedItems[i]);
        }

        //	Get Shipment
        var C_ProvisionalInvoice_ID = this.$super.mTab.getValue("C_ProvisionalInvoice_ID");
        var C_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();
        var M_InOut_ID = this.$super.cmbShipment.getControl().find('option:selected').val();

        return this.saveData(model, "", C_Order_ID, M_InOut_ID, C_ProvisionalInvoice_ID);
    }

    /* Save Provisional Invoice Line */
    VCreateFormProvisionalInvoice.prototype.saveData = function (model, selectedItems, C_Order_ID, M_inout_id, C_ProvisionalInvoice_ID) {
        var obj = this;
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveProvisionalInvoice",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_Order_ID: C_Order_ID,
                C_ProvisionalInvoice_ID: C_ProvisionalInvoice_ID,
                M_inout_id: M_inout_id
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
                console.log(textStatus);
                obj.$super.setBusy(false);
                alert(errorThrown);
            }
        });
    }

    /* Get Shipment Data for grid*/
    VCreateFormProvisionalInvoice.prototype.loadShipments = function (M_InOut_ID, M_Product_ID, pNo) {
        var data = this.getShipmentsData(VIS.Env.getCtx(), M_InOut_ID, M_Product_ID, pNo);
    }

    /* Get Shipment Data for grid*/
    VCreateFormProvisionalInvoice.prototype.getShipmentsData = function (ctx, M_InOut_ID, M_Product_ID, pNo) {

        var data = [];
        var self = this;
        this.$super.record_ID = $self.mTab.getValue("C_ProvisionalInvoice_ID");
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

    //dispose call
    VCreateFormProvisionalInvoice.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };

    //Load form into VIS
    VIS.VCreateFormProvisionalInvoice = VCreateFormProvisionalInvoice;

})(VIS, jQuery);

