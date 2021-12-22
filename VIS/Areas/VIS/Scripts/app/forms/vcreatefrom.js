
; (function (VIS, $) {

    //form declaretion
    function VCreateFrom(mTab) {

        //call parent function on close
        this.onClose = null;
        this.dGrid = null;
        var SELECT_DESELECT_ALL = "SelectDeselectAll";
        //select all button status
        var _checkStatus = true;
        //To button pic on grid selected records
        this.arrListColumns = [];

        // create log
        this.log = VIS.Logging.VLogger.getVLogger("VCreateFrom");
        this.divPaging = null;
        this.ulPaging = null;
        this.liFirstPage = null;
        this.liPrevPage = null;
        this.liCurrPage = null;
        this.liNextPage = null;
        this.liLastPage = null;
        this.cmbPage = null;
        var selectedItems = [];
        this.multiValues = [];
        this.editedItems = [];
        this.NonEditableRecord = [];
        var $self = this;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSet";
        var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";

        this.$root = $("<div style='position:relative'>");
        this.log = VIS.Logging.VLogger.getVLogger("VCreateFrom");
        //this.$busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        this.$busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        this.topDiv = $("<div class='vis-locatormainwrap vis-formouterwrpdiv'>");
        this.middelDiv = $("<div class='vis-crtfrm-datawrp'>");
        this.bottomDiv = $("<div class='vis-ctrfrm-btnwrp'>");
        this.divPaging = $('<div>');
        this.record_ID = 0;
        this.windowNo = mTab.getWindowNo();
        this.mTab = mTab;
        this.initOK = false;
        this.isApplied = false;


        var name = "btnOk_" + this.windowNo;
        this.Okbtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' type='button' value='" + VIS.Msg.getMsg("OK") + "'>");

        name = "btnCancel_" + this.windowNo;
        this.cancelbtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' type='button' value='" + VIS.Msg.getMsg("Cancel") + "'>");

        name = "btnDelete_" + this.windowNo;
        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/uncheck-icon.png";
        this.selectAllButton = $("<button type='button' id='" + name + "' style='margin-bottom:0px;margin-top:0px;display: none;float: left;padding: 0px;border: 0px;' value='Ok' role='button' aria-disabled='false'>" +
            "<img src='" + src + "'></button>");

        name = "btnRefresh_" + this.windowNo;
        src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
        this.btnRefresh = $("<button id='" + name + "' style='margin-bottom: 1px; margin-top: 0px;display: none; float: left; margin-left: 0px;height: 38px; ' class='VIS_Pref_btn-2'>" +
            "<img src='" + src + "'></button>");

        name = "btnApply_" + this.windowNo;
        this.Applybtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' style='display: none;' type='button' value='" + VIS.Msg.getMsg("Apply") + "'>");

        //check Arebic Calture
        //if (VIS.Application.isRTL) {
        //    this.Okbtn.css("margin-right", "-132px");
        //    this.Applybtn.css("margin-right", "-365px");
        //    this.cancelbtn.css("margin-right", "55px");
        //}


        this.lblBankAccount = new VIS.Controls.VLabel();
        this.lblBPartner = new VIS.Controls.VLabel();
        this.lblOrder = new VIS.Controls.VLabel();
        this.lblInvoice = new VIS.Controls.VLabel();
        this.lblShipment = new VIS.Controls.VLabel();
        this.lblLocator = new VIS.Controls.VLabel();
        this.lblDeliveryDate = new VIS.Controls.VLabel();
        this.lblProduct = new VIS.Controls.VLabel();
        // is used to represent document no on Create line form (M_Inout_ID)
        this.lblDocumentNoRef = new VIS.Controls.VLabel();
        // Added by Bharat on 05/May/2017 for Seach control on Bank Statement
        this.lblDocumentNo = new VIS.Controls.VLabel();
        this.lblDate = new VIS.Controls.VLabel();
        this.lblAmount = new VIS.Controls.VLabel();
        this.lblDepositSlip = new VIS.Controls.VLabel();
        this.lblAuthCode = new VIS.Controls.VLabel();
        this.lblCheckNo = new VIS.Controls.VLabel();
        // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
        this.lblContainer = null;
        src = VIS.Application.contextUrl + "Areas/VIS/Images/pallet-icon.png";
        //this.ContainerTree = $("<img id='ContainerTree_" + this.windowNo + "' style='height: 27px;' class='pull-left VIS_buttons-ContainerTree VIS-pallet-icon VIS_Tree-Container-disabled' src='" + src + "'>");
        this.ContainerTree = $("<button class='input-group-text VIS_Tree-Container-disabled'><i id='ContainerTree_" + this.windowNo + "' class='fa fa-cubes'></i></button>");

        // create column as Provisional Invoice on Create line on AP Invoice
        this.lblProvisionalInvoice = new VIS.Controls.VLabel();
        this.cmbProvisionalInvoice = new VIS.Controls.VComboBox('', false, false, true);

        this.DocumentNo = null;
        this.Date = null;
        this.Amount = null;
        this.DepositSlip = null;
        this.AuthCode = null;
        this.CheckNo = null;

        this.fromInvoice = false;
        this.ProvisionalInvoice = false;
        this.deliveryDate = null;
        this.cmbOrder = new VIS.Controls.VComboBox('', false, false, true);
        this.cmbInvoice = new VIS.Controls.VComboBox('', false, false, true);
        this.cmbShipment = new VIS.Controls.VComboBox('', false, false, true);
        // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
        this.cmbContainer = null;
        this.DocumentNoRef = null;
        this.vProduct = null;
        this.cmbBankAccount = null;
        this.vBPartner = null;
        this.locatorField = null;
        this.child = null;
        this.relatedToOrg = new VIS.Controls.VCheckBox('Related To My Organization', false, false, true, 'Related To My Organization', null);
        this.pattrLookup = new VIS.MPAttributeLookup(VIS.context, $self.windowNo);
        this.$productAttribute = new VIS.Controls.VPAttribute("M_AttributeSetInstance_ID", false, false, true, VIS.DisplayType.PAttribute, $self.pattrLookup, $self.windowNo, false, false, false, true);
        this.title = "";
        this.vetoablechange = function (evt) {
            if (evt.propertyName == "M_Product_ID") {
                $self.isApplied = false;
                $self.setBusy(true);
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                //var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                var deliveryDate = $self.deliveryDate.getValue();
                if (C_Order_ID != null) {
                    if ($self.locatorField != null) {
                        //for shipment haveing locator filed
                        $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);
                    }
                    else {
                        //for invoice
                        $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, 1);
                    }
                }
                else if (C_Invoice_ID != null) {
                    VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, 1);
                }
                else if (M_InOut_ID != null) {
                    // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                    if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                        VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                    }
                    else {
                        VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                    }
                } else if (C_ProvisionalInvoice_ID != null) {
                    // VA228:Load provisional invoice data in grid from AP invoice window
                    VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, 1);
                }
                else {
                    $self.setBusy(false);
                }
            }
            else if (evt.propertyName == "C_BPartner_ID") {
                $self.setBusy(true);
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var trxDate = $self.Date.getValue();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            }
            // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
            else if (evt.propertyName == "M_Locator_ID") {
                if ($self.locatorField != null) {
                    $self.isApplied = false;
                    // load Containers
                    var M_locator_ID = parseInt($self.locatorField.getValue());
                    if (M_locator_ID > 0)
                        VIS.VCreateFromShipment.prototype.initContainerDetails(M_locator_ID);
                }
            }
            //End
            else {
                this.locatorField.setBackground("");
            }
        };

        this.createPageSettings = function () {
            this.ulPaging = $('<ul class="vis-statusbar-ul">');

            this.liFirstPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftleft" title="First Page" style="opacity: 0.6;"></i></div></li>');

            this.liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-pageup" title="Page Up" style="opacity: 0.6;"></i></div></li>');

            this.cmbPage = $('<select>');

            this.liCurrPage = $('<li>').append(this.cmbPage);

            this.liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" title="Page Down" style="opacity: 0.6;"></i></div></li>');

            this.liLastPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" title="Last Page" style="opacity: 0.6;"></i></div></li>');


            this.ulPaging.append(this.liFirstPage).append(this.liPrevPage).append(this.liCurrPage).append(this.liNextPage).append(this.liLastPage);
            this.pageEvents();
        };

        this.pageEvents = function () {
            this.liFirstPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    $self.setBusy(true);
                    var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                    var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                    var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                    var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                    var C_BankAccount_ID = null;
                    if ($self.cmbBankAccount != null) {
                        C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    }
                    var M_Product_ID = null, deliveryDate = null;
                    if ($self.vProduct != null) {
                        M_Product_ID = $self.vProduct.getValue();
                    }
                    if ($self.deliveryDate != null) {
                        deliveryDate = $self.deliveryDate.getValue();
                    }
                    if (C_Order_ID != null) {
                        if ($self.locatorField != null) {
                            //for shipment haveing locator filed
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);
                        }
                        else {
                            //for invoice
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, 1);
                        }
                    }
                    else if (C_Invoice_ID != null) {
                        VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, 1);
                    }
                    else if (M_InOut_ID != null) {
                        // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                        if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                            VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                        }
                        else {
                            VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                        }
                    }
                    else if (C_BankAccount_ID != null) {
                        var trxDate = $self.Date.getValue();
                        var C_BPartner_ID = $self.vBPartner.getValue();
                        var DepositSlip = $self.DepositSlip.getValue();
                        var DocumentNo = $self.DocumentNo.getValue();
                        var AuthCode = $self.AuthCode.getValue();
                        var CheckNo = $self.CheckNo.getValue();
                        var Amount = $self.Amount.getValue();
                        VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
                        //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, 1);
                    } else if (C_ProvisionalInvoice_ID != null) {
                        // VA228:Load provisional invoice from AP invoice window
                        VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, 1);
                    }
                    //$self.setBusy(false);
                }
            });
            this.liPrevPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    //displayData(true, parseInt(this.cmbPage.val()) - 1);
                    $self.setBusy(true);
                    var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                    var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                    var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                    var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                    var C_BankAccount_ID = null;
                    if ($self.cmbBankAccount != null) {
                        C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    }
                    var M_Product_ID = null, deliveryDate = null;
                    if ($self.vProduct != null) {
                        M_Product_ID = $self.vProduct.getValue();
                    }
                    if ($self.deliveryDate != null) {
                        deliveryDate = $self.deliveryDate.getValue();
                    }
                    if (C_Order_ID != null) {
                        if ($self.locatorField != null) {
                            //for shipment haveing locator filed
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, parseInt($self.cmbPage.val()) - 1);
                        }
                        else {
                            //for invoice
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, parseInt($self.cmbPage.val()) - 1);
                        }
                    }
                    else if (C_Invoice_ID != null) {
                        VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, parseInt($self.cmbPage.val()) - 1);
                    }
                    else if (M_InOut_ID != null) {
                        // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                        if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                            VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.val()) - 1);
                        }
                        else {
                            VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.val()) - 1);
                        }
                    }
                    else if (C_BankAccount_ID != null) {
                        var trxDate = $self.Date.getValue();
                        var C_BPartner_ID = $self.vBPartner.getValue();
                        var DepositSlip = $self.DepositSlip.getValue();
                        var DocumentNo = $self.DocumentNo.getValue();
                        var AuthCode = $self.AuthCode.getValue();
                        var CheckNo = $self.CheckNo.getValue();
                        var Amount = $self.Amount.getValue();
                        VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, parseInt($self.cmbPage.val()) - 1);
                        //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, parseInt($self.cmbPage.val()) - 1);
                    } else if (C_ProvisionalInvoice_ID != null) {
                        // VA228:Load provisional invoice from AP invoice window
                        VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, parseInt($self.cmbPage.val()) - 1);
                    }
                    //$self.setBusy(false);
                }
            });
            this.liNextPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    //displayData(true, parseInt(this.cmbPage.val()) + 1);
                    $self.setBusy(true);
                    var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                    var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                    var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                    var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                    var C_BankAccount_ID = null;
                    if ($self.cmbBankAccount != null) {
                        C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    }
                    var M_Product_ID = null, deliveryDate = null;
                    if ($self.vProduct != null) {
                        M_Product_ID = $self.vProduct.getValue();
                    }
                    if ($self.deliveryDate != null) {
                        deliveryDate = $self.deliveryDate.getValue();
                    }
                    if (C_Order_ID != null) {
                        if ($self.locatorField != null) {
                            //for shipment haveing locator filed
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, parseInt($self.cmbPage.val()) + 1);
                        }
                        else {
                            //for invoice
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, parseInt($self.cmbPage.val()) + 1);
                        }
                    }
                    else if (C_Invoice_ID != null) {
                        VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, parseInt($self.cmbPage.val()) + 1);
                    }
                    else if (M_InOut_ID != null) {
                        // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                        if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                            VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.val()) + 1);
                        }
                        else {
                            VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.val()) + 1);
                        }
                    }
                    else if (C_BankAccount_ID != null) {
                        var trxDate = $self.Date.getValue();
                        var C_BPartner_ID = $self.vBPartner.getValue();
                        var DepositSlip = $self.DepositSlip.getValue();
                        var DocumentNo = $self.DocumentNo.getValue();
                        var AuthCode = $self.AuthCode.getValue();
                        var CheckNo = $self.CheckNo.getValue();
                        var Amount = $self.Amount.getValue();
                        VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, parseInt($self.cmbPage.val()) + 1);
                        //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, parseInt($self.cmbPage.val()) + 1);
                    } else if (C_ProvisionalInvoice_ID != null) {
                        // VA228:Load provisional invoice from AP invoice window
                        VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, parseInt($self.cmbPage.val()) + 1);
                    }
                    //$self.setBusy(false);
                }
            });
            this.liLastPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    //displayData(true, parseInt(this.cmbPage.find("Option:last").val()));
                    $self.setBusy(true);
                    var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                    var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                    var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                    var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                    var C_BankAccount_ID = null;
                    if ($self.cmbBankAccount != null) {
                        C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    }
                    var M_Product_ID = null, deliveryDate = null;
                    if ($self.vProduct != null) {
                        M_Product_ID = $self.vProduct.getValue();
                    }
                    if ($self.deliveryDate != null) {
                        deliveryDate = $self.deliveryDate.getValue();
                    }
                    if (C_Order_ID != null) {
                        if ($self.locatorField != null) {
                            //for shipment haveing locator filed
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, parseInt($self.cmbPage.find("Option:last").val()));
                        }
                        else {
                            //for invoice
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, parseInt($self.cmbPage.find("Option:last").val()));
                        }
                    }
                    else if (C_Invoice_ID != null) {
                        VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, parseInt($self.cmbPage.find("Option:last").val()));
                    }
                    else if (M_InOut_ID != null) {
                        // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                        if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                            VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.find("Option:last").val()));
                        }
                        else {
                            VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, parseInt($self.cmbPage.find("Option:last").val()));
                        }
                    }
                    else if (C_BankAccount_ID != null) {
                        var trxDate = $self.Date.getValue();
                        var C_BPartner_ID = $self.vBPartner.getValue();
                        var DepositSlip = $self.DepositSlip.getValue();
                        var DocumentNo = $self.DocumentNo.getValue();
                        var AuthCode = $self.AuthCode.getValue();
                        var CheckNo = $self.CheckNo.getValue();
                        var Amount = $self.Amount.getValue();
                        VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, parseInt($self.cmbPage.find("Option:last").val()));
                        //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, parseInt($self.cmbPage.find("Option:last").val()));
                    } else if (C_ProvisionalInvoice_ID != null) {
                        // VA228:Load provisional invoice from AP invoice window
                        VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, parseInt($self.cmbPage.find("Option:last").val()));
                    }
                    //$self.setBusy(false);
                }
            });
            this.cmbPage.on("change", function () {
                //displayData(true, this.cmbPage.val());
                $self.setBusy(true);
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                var C_BankAccount_ID = null;
                if ($self.cmbBankAccount != null) {
                    C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                }
                var M_Product_ID = null, deliveryDate = null;
                if ($self.vProduct != null) {
                    M_Product_ID = $self.vProduct.getValue();
                }
                if ($self.deliveryDate != null) {
                    deliveryDate = $self.deliveryDate.getValue();
                }
                if (C_Order_ID != null) {
                    if ($self.locatorField != null) {
                        //for shipment haveing locator filed
                        $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, $self.cmbPage.val());
                    }
                    else {
                        //for invoice
                        $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, $self.cmbPage.val());
                    }
                }
                else if (C_Invoice_ID != null) {
                    VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, $self.cmbPage.val());
                }
                else if (M_InOut_ID != null) {
                    // call loadshipments from VCreateFormProvisionalInvoice.js for Provisiona Invoice
                    if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                        VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, $self.cmbPage.val());
                    }
                    else {
                        VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, $self.cmbPage.val());
                    }
                }
                else if (C_BankAccount_ID != null) {
                    var trxDate = $self.Date.getValue();
                    var C_BPartner_ID = $self.vBPartner.getValue();
                    var DepositSlip = $self.DepositSlip.getValue();
                    var DocumentNo = $self.DocumentNo.getValue();
                    var AuthCode = $self.AuthCode.getValue();
                    var CheckNo = $self.CheckNo.getValue();
                    var Amount = $self.Amount.getValue();
                    VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, $self.cmbPage.val());
                    //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, $self.cmbPage.val());
                } else if (C_ProvisionalInvoice_ID != null) {
                    // VA228:Load provisional invoice from AP invoice window
                    VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, $self.cmbPage.val());
                }
                //$self.setBusy(false);
            });
        };

        this.resetPageCtrls = function (psetting) {
            this.cmbPage.empty();
            if (psetting.TotalPage > 0) {
                for (var i = 0; i < psetting.TotalPage; i++) {
                    this.cmbPage.append($("<option value=" + (i + 1) + ">" + (i + 1) + "</option>"))
                }
                this.cmbPage.val(psetting.CurrentPage);


                if (psetting.TotalPage > psetting.CurrentPage) {
                    this.liNextPage.css("opacity", "1");
                    this.liLastPage.css("opacity", "1");
                }
                else {
                    this.liNextPage.css("opacity", "0.6");
                    this.liLastPage.css("opacity", "0.6");
                }

                if (psetting.CurrentPage > 1) {
                    this.liFirstPage.css("opacity", "1");
                    this.liPrevPage.css("opacity", "1");
                }
                else {
                    this.liFirstPage.css("opacity", "0.6");
                    this.liPrevPage.css("opacity", "0.6");
                }

                if (psetting.TotalPage == 1) {
                    this.liFirstPage.css("opacity", "0.6");
                    this.liPrevPage.css("opacity", "0.6");
                    this.liNextPage.css("opacity", "0.6");
                    this.liLastPage.css("opacity", "0.6");
                }
            }
            else {
                this.liFirstPage.css("opacity", "0.6");
                this.liPrevPage.css("opacity", "0.6");
                this.liNextPage.css("opacity", "0.6");
                this.liLastPage.css("opacity", "0.6");
            }
        }

        this.setBusy = function (isBusy) {
            this.$busyDiv.css("display", isBusy ? 'block' : 'none');
        };
        // Change By Mohit 30/06/2016
        this.MergeItemsForSave = function () {
            var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
            var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
            var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
            if ($self.editedItems.length > 0) {
                for (var items in $self.editedItems) {
                    var selectprd = $self.editedItems[items]["M_Product_ID_K"];
                    var obj = $.grep($self.multiValues, function (n, i) {

                        if (C_Order_ID != null) {
                            return n.M_Product_ID_K == selectprd && n.C_Order_ID_K == $self.editedItems[items]["C_Order_ID_K"]
                        }
                        else if (C_Invoice_ID != null) {
                            return n.M_Product_ID_K == selectprd && n.C_Invoice_ID_K == $self.editedItems[items]["C_Invoice_ID_K"]
                        }
                        else if (M_InOut_ID != null) {
                            return n.M_Product_ID_K == selectprd && n.M_InOut_ID_K == $self.editedItems[items]["M_InOut_ID_K"]
                        }
                        else {
                            return n.C_Payment_ID_K == $self.editedItems[items]["C_Payment_ID_K"]
                        }
                    });
                    if (obj.length > 0) {

                    }
                    else {
                        $self.multiValues.push($self.editedItems[items]);
                    }
                }
            }
        };
        this.showDialog = function () {
            if (this.locatorField) {
                this.locatorField.setBackground(false);
                this.locatorField.addVetoableChangeListener(this);
            }

            if (this.vProduct) {
                this.vProduct.addVetoableChangeListener(this);
            }

            if (this.vBPartner) {
                this.vBPartner.addVetoableChangeListener(this);
            }
            var obj = this;
            this.$root.append(this.$busyDiv);
            this.setBusy(false);
            this.$root.dialog({
                modal: true,
                title: this.title,
                width: 900,
                height: 600,
                resizable: false,
                position: { at: "center top", of: window },
                close: function () {
                    obj.dispose();
                    if (obj.dGrid != null) {
                        obj.dGrid.destroy();
                    }
                    $self = null;
                    obj.$root.dialog("destroy");
                    obj.$root = null;
                }
            });
        };

        // function to check comma or dot from given value and return new value
        this.checkcommaordot = function (event, val) {
            var foundComma = false;
            event.value_new = VIS.Utility.Util.getValueOfString(val);
            if (event.value_new.contains(".")) {
                foundComma = true;
                var indices = [];
                for (var i = 0; i < event.value_new.length; i++) {
                    if (event.value_new[i] === ".")
                        indices.push(i);
                }
                if (indices.length > 1) {
                    event.value_new = removeAllButLast(event.value_new, '.');
                }
            }
            if (event.value_new.contains(",")) {
                if (foundComma) {
                    event.value_new = removeAllButLast(event.value_new, ',');
                }
                else {
                    var indices = [];
                    for (var i = 0; i < event.value_new.length; i++) {
                        if (event.value_new[i] === ",")
                            indices.push(i);
                    }
                    if (indices.length > 1) {
                        event.value_new = removeAllButLast(event.value_new, ',');
                    }
                    else {
                        event.value_new = event.value_new.replace(",", ".");
                    }
                }
            }
            if (event.value_new == "") {
                event.value_new = "0";
            }
            return event.value_new;
        };

        // Remove all seperator but only bring last seperator
        this.removeAllButLast = function (amt, seprator) {
            var parts = amt.split(seprator);
            amt = parts.slice(0, -1).join('') + '.' + parts.slice(-1);
            if (amt.indexOf('.') == (amt.length - 1)) {
                amt = amt.replace(".", "");
            }
            return amt;
        };

        this.disposeComponent = function () {
            $self = null;
            if (this.Okbtn)
                this.Okbtn.off("click");
            if (this.cancelbtn)
                this.cancelbtn.off("click");
            if (this.selectAllButton)
                this.selectAllButton.off("click");
            if (this.btnRefresh)
                this.btnRefresh.off("click");
            if (this.Applybtn)
                this.Applybtn.off("click");
            this.disposeComponent = null;
        };
    };

    //dispose call
    VCreateFrom.prototype.dispose = function () {
        this.disposeComponent();
    };

    VCreateFrom.prototype.create = function (mTab) {
        //	dynamic init preparation
        var AD_Table_ID = VIS.Env.getCtx().getContextAsInt(mTab.getWindowNo(), "BaseTable_ID");

        var retValue = null;// VCreateFrom form object
        if (AD_Table_ID == 392)             //  C_BankStatement
        {
            retValue = new VIS.VCreateFromStatement(mTab);
        }
        else if (AD_Table_ID == 318)        //  C_Invoice
        {
            retValue = new VIS.VCreateFromInvoice(mTab);
        }
        else if (AD_Table_ID == 319)        //  M_InOut
        {
            retValue = new VIS.VCreateFromShipment(mTab);
        }
        else if (AD_Table_ID == 426)		//	C_PaySelection
        {
            return null;	//	ignore - will call process C_PaySelection_CreateFrom
        }
        else if (mTab.keyColumnName == "C_ProvisionalInvoice_ID")		//	C_ProvisionalInvoice
        {
            retValue = new VIS.VCreateFormProvisionalInvoice(mTab);
        }
        else    //  Not supported CreateFrom
        {
            return null;
        }
        return retValue;
    }

    VCreateFrom.prototype.initBPDetails = function (C_BPartner_ID) {
        return C_BPartner_ID;
    };

    VCreateFrom.prototype.initBPartner = function (forInvoice) {
        //  load BPartner
        var AD_Column_ID = 3499;        //  C_Invoice.C_BPartner_ID
        var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), this.windowNo, AD_Column_ID, VIS.DisplayType.Search);

        this.vBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, lookup);
        var C_BPartner_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_BPartner_ID");
        this.vBPartner.setValue(C_BPartner_ID);
        //  initial loading
        return this.initBPartnerOIS(C_BPartner_ID, forInvoice);
    }

    VCreateFrom.prototype.initBPartnerOIS = function (C_BPartner_ID, forInvoice) {
        var orcmb = this.cmbOrder;
        //$(orcmb).empty()
        $(orcmb).html("");

        var isReturnTrx = false;

        // Handled the case of Return, show only Return orders
        if (forInvoice) {
            var dt = VIS.dataContext.getJSONRecord("MDocType/GetDocType", VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_DocTypeTarget_ID").toString());
            isReturnTrx = VIS.Utility.Util.getValueOfBoolean(dt["IsReturnTrx"]);
            var isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsSOTrx"));
            // Load Provisional Invoice to combo only when creating AP Invoice
            if (!isReturnTrx && !isSoTrx)
                this.getProvisionlInvoices(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx);
        }
        else {
            isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsReturnTrx"));
        }
        //var orders =
        // JID_0350: "When user create MR with refrence to order OR by invoice by using "Create Line From" charge should not shows on grid.

        this.getOrders(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx, forInvoice);


        //for (var i = 0; i < orders.length; i++) {
        //    if (i == 0) {
        //        this.cmbOrder.getControl().append(" <option value=0> </option>");
        //    }
        //    this.cmbOrder.getControl().append(" <option value=" + orders[i].ID + ">" + orders[i].value + "</option>");
        //};

        //this.cmbOrder.getControl().prop('selectedIndex', 0);
        return this.initBPDetails(C_BPartner_ID);
    }

    /**Load Order and BP Deatil for Provisional Invoice */
    VCreateFrom.prototype.initBPartnerForProvisional = function () {
        //  load BPartner
        var AD_Column_ID = 3499;        //  C_Invoice.C_BPartner_ID
        var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), this.windowNo, AD_Column_ID, VIS.DisplayType.Search);

        this.vBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, lookup);
        var C_BPartner_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_BPartner_ID");
        this.vBPartner.setValue(C_BPartner_ID);

        //  initial loading
        var orcmb = this.cmbOrder;
        $(orcmb).html("");

        var isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsReturnTrx"));

        this.getOrdersForProvisional(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx);

        return this.initBPDetails(C_BPartner_ID);
    }

    // Get Orders
    VCreateFrom.prototype.getOrders = function (ctx, C_BPartner_ID, isReturnTrx, forInvoice) {

        var display = "o.DocumentNo||' - ' ||".concat(VIS.DB.to_char("o.DateOrdered", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("||' - '||").concat(
            VIS.DB.to_char("o.GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

        var column = "m.M_InOutLine_ID";
        if (forInvoice) {
            column = "m.C_InvoiceLine_ID";
        }


        var OrgId = VIS.Env.getCtx().getContextAsInt(this.windowNo, "AD_Org_ID")
        // Added by Vivek on 09/10/2017 advised by Pradeep
        var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsDropShip"));
        var _isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsSOTrx"));
        //JID_0976
        var recordId = VIS.context.getWindowContextAsInt(this.windowNo, "C_Invoice_ID", true);
        //var pairs = [];
        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/VCreateGetOrders",
            type: 'POST',
            //async: false,
            data: {
                displays: display,
                columns: column,
                C_BPartner_IDs: C_BPartner_ID,
                isReturnTrxs: isReturnTrx,
                OrgIds: OrgId,
                IsDrop: _isdrop,
                IsSOTrx: _isSoTrx,
                forInvoices: forInvoice,
                recordID: recordId,
            },
            success: function (datas) {
                var ress = datas.result;
                if (ress && ress.length > 0) {
                    try {
                        var key, value;
                        for (var i = 0; i < ress.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(ress[i].key);
                            value = VIS.Utility.encodeText(ress[i].value);
                            //pairs.push({ ID: key, value: value });

                            if (i == 0) {
                                $self.cmbOrder.getControl().append(" <option value=0> </option>");
                            }
                            $self.cmbOrder.getControl().append(" <option value=" + key + ">" + value + "</option>");
                        }
                        $self.cmbOrder.getControl().prop('selectedIndex', 0);
                    }
                    catch (e) {

                    }
                }
            },
            error: function (e) {
                $self.log.info(e);
            },
        });
        //return pairs;
    }

    /**
    * Get Provisional Invoice into provisional invoice combo
    * @param {any} ctx
    * @param {any} C_BPartner_ID
    * @param {any} isReturnTrx
    */
    VCreateFrom.prototype.getProvisionlInvoices = function (ctx, C_BPartner_ID, isReturnTrx) {

        var display = "i.DocumentNo||' - ' ||".concat(VIS.DB.to_char("i.DateInvoiced", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("||' - '||").concat(
            VIS.DB.to_char("i.GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

        var OrgId = VIS.Env.getCtx().getContextAsInt(this.windowNo, "AD_Org_ID")
        var recordId = VIS.context.getWindowContextAsInt(this.windowNo, "C_Invoice_ID", true);
        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/VCreateGetProvisionalInvoices",
            type: 'POST',
            //async: false,
            data: {
                displays: display,
                C_BPartner_IDs: C_BPartner_ID,
                isReturnTrxs: isReturnTrx,
                OrgIds: OrgId,
                recordID: recordId,
            },
            success: function (datas) {
                var ress = datas.result;
                if (ress && ress.length > 0) {
                    try {
                        var key, value;
                        for (var i = 0; i < ress.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(ress[i].key);
                            value = VIS.Utility.encodeText(ress[i].value);

                            if (i == 0) {
                                $self.cmbProvisionalInvoice.getControl().append(" <option value=0> </option>");
                            }
                            $self.cmbProvisionalInvoice.getControl().append(" <option value=" + key + ">" + value + "</option>");
                        }
                        $self.cmbProvisionalInvoice.getControl().prop('selectedIndex', 0);
                    }
                    catch (e) {

                    }
                }
            },
            error: function (e) {
                $self.log.info(e);
            },
        });
    }
    /**
     * Load Order for Provisional Invoice
     * @param {any} ctx
     * @param {any} C_BPartner_ID
     * @param {any} isReturnTrx
     */
    VCreateFrom.prototype.getOrdersForProvisional = function (ctx, C_BPartner_ID, isReturnTrx) {

        var display = "o.DocumentNo||' - ' ||".concat(VIS.DB.to_char("o.DateOrdered", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("||' - '||").concat(
            VIS.DB.to_char("o.GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

        var column = "m.C_InvoiceLine_ID";

        var OrgId = VIS.Env.getCtx().getContextAsInt(this.windowNo, "AD_Org_ID")
        var _isdrop = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsDropShip"));
        var _isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsSOTrx"));
        var recordId = VIS.context.getWindowContextAsInt(this.windowNo, "C_ProvisionalInvoice_ID", true);
        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/VCreateGetOrders",
            type: 'POST',
            //async: false,
            data: {
                displays: display,
                columns: column,
                C_BPartner_IDs: C_BPartner_ID,
                isReturnTrxs: isReturnTrx,
                OrgIds: OrgId,
                IsDrop: _isdrop,
                IsSOTrx: _isSoTrx,
                forInvoices: false,
                recordID: recordId,
            },
            success: function (datas) {
                var ress = datas.result;
                if (ress && ress.length > 0) {
                    try {
                        var key, value;
                        for (var i = 0; i < ress.length; i++) {
                            key = VIS.Utility.Util.getValueOfInt(ress[i].key);
                            value = VIS.Utility.encodeText(ress[i].value);
                            if (i == 0) {
                                $self.cmbOrder.getControl().append(" <option value=0> </option>");
                            }
                            $self.cmbOrder.getControl().append(" <option value=" + key + ">" + value + "</option>");
                        }
                        $self.cmbOrder.getControl().prop('selectedIndex', 0);
                    }
                    catch (e) { }
                }
            },
            error: function (e) {
                $self.log.info(e);
            },
        });
    }

    VCreateFrom.prototype.jbInit = function () {
        this.init();

        this.lblBankAccount.getControl().text(VIS.Msg.getMsg("BankAcctCurrency"));
        this.lblBPartner.getControl().text(VIS.Msg.getMsg("BusinessPartner"));
        this.lblOrder.getControl().text(VIS.Msg.getMsg("Order"));
        this.lblInvoice.getControl().text(VIS.Msg.getMsg("Invoice"));
        this.lblShipment.getControl().text(VIS.Msg.getMsg("Shipment/Receipt"));
        this.lblLocator.getControl().text(VIS.Msg.getMsg("M_Locator_ID"));
        this.lblDeliveryDate.getControl().text(VIS.Msg.getMsg("DeliveryDate"));
        this.lblProduct.getControl().text(VIS.Msg.getMsg("ProductName"));
        // informative lable : document no on inout and invoice

        this.lblDocumentNoRef.getControl().text(VIS.Msg.getMsg("DocumentNo"));
        // Added by Bharat on 05/May/2017 for Seach control on Bank Statement
        this.lblDocumentNo.getControl().text(VIS.Msg.getMsg("DocumentNo"));
        this.lblDate.getControl().text(VIS.Msg.getMsg("DateAcct"));
        this.lblAmount.getControl().text(VIS.Msg.getMsg("Amount"));
        this.lblDepositSlip.getControl().text(VIS.Msg.getMsg("DepositSlipNo"));
        this.lblAuthCode.getControl().text(VIS.Msg.getMsg("AuthCode"));
        this.lblCheckNo.getControl().text(VIS.Msg.getMsg("CheckNo"));

        this.lblProvisionalInvoice.getControl().text(VIS.Msg.getMsg("ProvisionalInvoice"));


        //First Row
        /*******************************/
        var line = $("<div class='VIS_Pref_show'>");

        //First Column
        /*******************************/
        var col = $("<div class='VIS_Pref_dd'>");
        //var label = $("<div class='vis-pref-ctrl-lblwrp'>");

        //if (VIS.Application.isRTL) {
        //    label = $("<div style='float: right; margin-right: 5px; width: 100%; text-align: left'>");
        //}

        //var lableCtrl = $("<h5 style='width: 100%'>");
        //var ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
        var DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
        var DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
        var DivCtrlBtnWrap = $("<div class='input-group-append'>");

        this.topDiv.append(line);
        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);
        DivInputWrap.append(DivCtrlBtnWrap);

        //else {
        // To append Business Partner Control.. 
        if (this.vBPartner != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(DivInputWrap);
            //DivInputWrap.append(DivInputCtrlWrap);
            //DivInputWrap.append(DivCtrlBtnWrap);
            if (this.cmbBankAccount != null) {
                DivInputCtrlWrap.append(this.vBPartner.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
                DivCtrlBtnWrap.append(this.vBPartner.getBtn(0));
                DivCtrlBtnWrap.append(this.vBPartner.getBtn(1));
            }
            else {
                DivInputCtrlWrap.append(this.vBPartner.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
                DivCtrlBtnWrap.append(this.vBPartner.getBtn(0));
            }
            DivInputCtrlWrap.append(this.lblBPartner.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //End Business Partner Control
        //}
        //End First Column
        /*******************************/

        //Second Column
        /*******************************/
        col = $("<div class='VIS_Pref_dd'>");
        //label = $("<div class='vis-pref-ctrl-lblwrp'>");
        //if (VIS.Application.isRTL) {
        //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        //}
        //lableCtrl = $("<h5 style='width: 100%'>");
        //ctrl = $("<div class='VIS_Pref_slide-show vis-pref-ctrlwrap'>");
        DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
        DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
        DivCtrlBtnWrap = $("<div class='input-group-append'>");

        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);
        DivInputWrap.append(DivCtrlBtnWrap);

        //if (VIS.Application.isRTL) {
        //    //reverse controls order            
        //    if (this.vBPartner != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        //col.append(DivInputWrap);
        //        //DivInputWrap.append(DivInputCtrlWrap);
        //        //DivInputWrap.append(DivCtrlBtnWrap);
        //        if (this.cmbBankAccount != null) {
        //            DivInputCtrlWrap.append(this.vBPartner.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
        //            DivCtrlBtnWrap.append(this.vBPartner.getBtn(0));
        //            DivCtrlBtnWrap.append(this.vBPartner.getBtn(1));
        //        }
        //        else {
        //            DivInputCtrlWrap.append(this.vBPartner.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
        //            DivCtrlBtnWrap.append(this.vBPartner.getBtn(0));
        //        }
        //        DivInputCtrlWrap.append(this.lblBPartner.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.cmbBankAccount != null) {
            //col.append(DivInputWrap);
            //DivInputWrap.append(DivInputCtrlWrap);
            DivInputCtrlWrap.append(this.cmbBankAccount.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblBankAccount.getControl().addClass('VIS_Pref_Label_Font'));
        }
        else {
            if (this.DocumentNoRef != null) {
                //ctrl.removeClass("VIS_Pref_slide-show pp");
                //col.append(DivInputWrap);
                //DivInputWrap.append(DivInputCtrlWrap);
                DivInputCtrlWrap.append(this.DocumentNoRef.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                DivInputCtrlWrap.append(this.lblDocumentNoRef.getControl().addClass('VIS_Pref_Label_Font'));
            }
            else if (this.cmbProvisionalInvoice != null
                && $self.mTab.keyColumnName == "C_Invoice_ID"
                && "N".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsSOTrx"))) {
                // show Provisional Invoice column on AP Invoice
                DivInputCtrlWrap.append(this.cmbProvisionalInvoice.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                DivInputCtrlWrap.append(this.lblProvisionalInvoice.getControl().addClass('VIS_Pref_Label_Font'));
            }
        }
        //}
        //End Second Column
        /*******************************/
        //End First Row
        /*******************************/

        //Second Row
        /*******************************/
        line = $("<div class='VIS_Pref_show'>");
        if (this.Amount != null) {
            col = $("<div class='VIS_Pref_dd'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 40%; text-align: left'>");
            //}
        }
        else {
            col = $("<div class='VIS_Pref_dd'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
        }
        //lableCtrl = $("<h5 style='width: 100%'>");

        this.topDiv.append(line);
        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.Date != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        col.append(DivInputWrap);
        //        DivInputCtrlWrap.append(this.Date.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblDate.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.DocumentNo != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.DocumentNo.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblDocumentNo.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.cmbInvoice != null) {
        //        col.append(DivInputWrap);
        //        DivInputCtrlWrap.append(this.cmbInvoice.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //    if (this.cmbShipment != null) {
        //        col.append(DivInputWrap);
        //        DivInputCtrlWrap.append(this.cmbShipment.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        // else {

        if (this.cmbOrder != null) {
            col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.cmbOrder.getControl());
            DivInputCtrlWrap.append(this.lblOrder.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}

        // Amount Control
        if (this.Amount != null) {
            col = $("<div class='VIS_Pref_dd'>");
            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //lableCtrl = $("<h5 style='width: 100%'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            line.append(col);
            col.append(DivInputWrap);
            DivInputWrap.append(DivInputCtrlWrap);

            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.Amount.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblAmount.getControl().addClass('VIS_Pref_Label_Font'));
        }

        // Reset Column size
        if (this.Amount != null) {
            col = $("<div class='VIS_Pref_dd'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 40%; text-align: left'>");
            //}
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
        }
        else {
            col = $("<div class='VIS_Pref_dd'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
        }
        //lableCtrl = $("<h5 style='width: 100%'>");


        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.cmbOrder != null) {
        //        //col.append(DivInputWrap);
        //        DivInputCtrlWrap.append(this.cmbOrder.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblOrder.getControl().addClass('VIS_Pref_Label_Font'));
        //    }

        //} 
        //else {
        if (this.cmbInvoice != null) {
            //col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.cmbInvoice.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
        }
        if (this.cmbShipment != null) {
            //col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.cmbShipment.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
        }
        // }

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.DocumentNo != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        //col.append(DivInputWrap);
        //        DivInputCtrlWrap.append(this.DocumentNo.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblDocumentNo.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.Date != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(DivInputWrap);
            DivInputCtrlWrap.append(this.Date.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblDate.getControl().addClass('VIS_Pref_Label_Font'));
        }
        // }

        //End Second Row
        /*******************************/

        //Third Row
    /*******************************/
        if (this.locatorField != null || this.cmbContainer != null) {
            this.middelDiv.css('height', '48.5%');

            line = $("<div class='VIS_Pref_show'>");
            col = $("<div class='VIS_Pref_dd'>");
            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //lableCtrl = $("<h5 style='width: 100%'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            DivCtrlBtnWrap = $("<div class='input-group-append'>");


            this.topDiv.append(line);
            line.append(col);
            col.append(DivInputWrap);
            DivInputWrap.append(DivInputCtrlWrap);
            DivInputWrap.append(DivCtrlBtnWrap);

            //if (VIS.Application.isRTL) {
            //    // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
            //    if (this.cmbContainer != null) {
            //        //var src = VIS.Application.contextUrl + "Areas/VIS/Images/pallet-icon.png";
            //        col.css('visibility', 'hidden');
            //        //.append(ctrl);
            //        if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"] != undefined) {
            //            if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"].equals("Y", true)) {
            //                col.css('visibility', 'visible');
            //                //.append(ctrl);
            //                this.ContainerTree.removeClass("VIS_Tree-Container-disabled");
            //            }
            //        }
            //        DivInputCtrlWrap.append(this.cmbContainer.getControl().attr("disabled", true).attr('data-placeholder', '').attr('placeholder', ' '));
            //        DivCtrlBtnWrap.append(this.ContainerTree);
            //        DivInputCtrlWrap.append(this.lblContainer.getControl().addClass('VIS_Pref_Label_Font'));
            //    }
            //}
            //else {

            if (this.locatorField != null) {
                //ctrl.removeClass("VIS_Pref_slide-show pp");
                //col.append(ctrl);
                DivInputCtrlWrap.append(this.locatorField.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
                DivCtrlBtnWrap.append(this.locatorField.getBtn(0));
                DivInputCtrlWrap.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
            }
            //}


            col = $("<div class='VIS_Pref_dd'>");
            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //lableCtrl = $("<h5 style='width: 100%'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            DivCtrlBtnWrap = $("<div class='input-group-append'>");

            line.append(col);
            col.append(DivInputWrap);
            DivInputWrap.append(DivInputCtrlWrap);
            DivInputWrap.append(DivCtrlBtnWrap);

            //if (VIS.Application.isRTL) {
            //    if (this.locatorField != null) {
            //        //ctrl.removeClass("VIS_Pref_slide-show pp");
            //        //col.append(ctrl);
            //        DivInputCtrlWrap.append(this.locatorField.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            //        DivCtrlBtnWrap.append(this.locatorField.getBtn(0));
            //        DivInputCtrlWrap.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
            //    }
            //}
            //else {
            // Added by Manjot on 12/9/18 for combobox of Container on Material Receipt
            if (this.cmbContainer != null) {
                //var src = VIS.Application.contextUrl + "Areas/VIS/Images/pallet-icon.png";
                col.css('visibility', 'hidden');//.append(ctrl);
                if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"] != undefined) {
                    if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"].equals("Y", true)) {
                        col.css('visibility', 'visible');//.append(ctrl);
                        this.ContainerTree.removeClass("VIS_Tree-Container-disabled");
                    }
                }
                DivInputCtrlWrap.append(this.cmbContainer.getControl().attr("disabled", true).attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
                DivCtrlBtnWrap.append(this.ContainerTree);
                DivInputCtrlWrap.append(this.lblContainer.getControl().addClass('VIS_Pref_Label_Font'));
            }
            //}
        }
        //End Third Row
        /*******************************/

        //Forth Row
        /*******************************/

        line = $("<div class='VIS_Pref_show'>");

        // Reset Size
        if (this.CheckNo != null) {
            col = $("<div class='VIS_Pref_dd'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 40%; text-align: left'>");
            //}
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            DivCtrlBtnWrap = $("<div class='input-group-append'>");
        }
        else {
            col = $("<div class='VIS_Pref_dd'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            DivCtrlBtnWrap = $("<div class='input-group-append'>");
        }
        //lableCtrl = $("<h5 style='width: 100%'>");

        // VA228: Increase hegight of middle div when open from Provisional Invoice
        if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
            this.middelDiv.css('height', '64%');
        }

        this.topDiv.append(line);
        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);
        DivInputWrap.append(DivCtrlBtnWrap);
        // Added by Bharat on 05/May/2017
        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.AuthCode != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        //col.append(ctrl);
        //        DivInputCtrlWrap.append(this.AuthCode.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblAuthCode.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.DepositSlip != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(ctrl);
            DivInputCtrlWrap.append(this.DepositSlip.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblDepositSlip.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.vProduct != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        //col.append(ctrl);
        //        DivInputCtrlWrap.append(this.vProduct.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
        //        DivCtrlBtnWrap.append(this.vProduct.getBtn(0));
        //        DivCtrlBtnWrap.append(this.vProduct.getBtn(1));
        //        DivInputCtrlWrap.append(this.lblProduct.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.deliveryDate != null) {
            //col.append(ctrl);
            DivInputCtrlWrap.append(this.deliveryDate.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblDeliveryDate.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}

        // Check No Control
        if (this.CheckNo != null) {
            col = $("<div class='VIS_Pref_dd'>");
            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //lableCtrl = $("<h5 style='width: 100%'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");

            line.append(col);
            col.append(DivInputWrap);
            DivInputWrap.append(DivInputCtrlWrap);

            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(ctrl);
            DivInputCtrlWrap.append(this.CheckNo.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblCheckNo.getControl().addClass('VIS_Pref_Label_Font'));
        }

        // Reset Column Size
        if (this.CheckNo != null) {
            col = $("<div class='VIS_Pref_dd'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 40%; text-align: left'>");
            //}
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
        }
        else {
            col = $("<div class='VIS_Pref_dd'>");

            //label = $("<div class='vis-pref-ctrl-lblwrp'>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");
            DivCtrlBtnWrap = $("<div class='input-group-append'>");
        }
        //lableCtrl = $("<h5 style='width: 100%'>");

        line.append(col);
        col.append(DivInputWrap);
        DivInputWrap.append(DivInputCtrlWrap);
        DivInputWrap.append(DivCtrlBtnWrap);

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.deliveryDate != null) {
        //        //col.append(ctrl);
        //        DivInputCtrlWrap.append(this.deliveryDate.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblDeliveryDate.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.vProduct != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(ctrl);
            DivInputCtrlWrap.append(this.vProduct.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            DivCtrlBtnWrap.append(this.vProduct.getBtn(0));
            DivCtrlBtnWrap.append(this.vProduct.getBtn(1));
            DivInputCtrlWrap.append(this.lblProduct.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}

        //if (VIS.Application.isRTL) {
        //    //reverse controls order
        //    if (this.DepositSlip != null) {
        //        //ctrl.removeClass("VIS_Pref_slide-show pp");
        //        //col.append(ctrl);
        //        DivInputCtrlWrap.append(this.DepositSlip.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
        //        DivInputCtrlWrap.append(this.lblDepositSlip.getControl().addClass('VIS_Pref_Label_Font'));
        //    }
        //}
        //else {
        if (this.AuthCode != null) {
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            //col.append(ctrl);
            DivInputCtrlWrap.append(this.AuthCode.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            DivInputCtrlWrap.append(this.lblAuthCode.getControl().addClass('VIS_Pref_Label_Font'));
        }
        //}
        //End Forth Row
        /******************************/

        //Fifth Row
        /******************************/
        // specific to Batir
        //if (window.BTR001) {
        if (this.relatedToOrg != null) {
            line = $("<div class='VIS_Pref_show'>");
            col = $("<div class='VIS_Pref_dd'>");
            //label = $("<div class=''>");
            //if (VIS.Application.isRTL) {
            //    label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
            //}
            //lableCtrl = $("<h5 style='width: 100%'>");
            //ctrl = $("<div class='VIS_Pref_slide-show pp vis-pref-ctrlwrap'>");
            DivInputWrap = $("<div class='input-group vis-input-wrap'></div>");
            DivInputCtrlWrap = $("<div class='vis-control-wrap'></div>");


            this.topDiv.append(line);
            line.append(col);
            col.append(DivInputWrap);
            DivInputWrap.append(DivInputCtrlWrap);

            //if (window.DTD001) {
            DivInputCtrlWrap.append(this.relatedToOrg.getControl());
        }
        //End Fifth Row
        /******************************/
    };

    VCreateFrom.prototype.init = function () {
        this.createPageSettings();
        this.divPaging.append(this.ulPaging);
        this.$root.append(this.topDiv).append(this.middelDiv).append(this.divPaging).append(this.bottomDiv);
        this.bottomDiv.append(this.selectAllButton);
        this.bottomDiv.append(this.btnRefresh);
        this.bottomDiv.append(this.cancelbtn);
        this.bottomDiv.append(this.Applybtn);
        this.bottomDiv.append(this.Okbtn);
        this.bottomDiv.append(this.divPaging);
        //this.dGrid= VIS.Utility.Util.gridLoad(,,);
        this.middelDiv.append(this.dGrid);
        $self = this;
        this.Okbtn.on("click", function () {

            //JID_0225: if there is already line inserted through Apply button, form will be closed on OK button.
            if ($self.isApplied) {
                $self.$root.dialog('close');
                return;
            }
            $self.setBusy(true);
            var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
            var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
            var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
            var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();

            var selection = null;
            if ($self.dGrid != null) {
                selection = $self.dGrid.getSelection();
            }
            if (selection == null || selection.length == 0) {
                VIS.ADialog.info("RecordSelectionRequired");
                $self.setBusy(false);
                return;
            }

            if ($self.locatorField != null) {
                // Change By Mohit 30/06/2016
                //$self.multiValues = [];
                //for shipment haveing locator filed                
                if ($self.dGrid != null && C_Order_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                else if ($self.dGrid != null && C_Invoice_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Invoice_ID_K == $self.dGrid.get(selection[item])["C_Invoice_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                // Change By Mohit 30/06/2016
                $self.MergeItemsForSave();
                VIS.VCreateFromShipment.prototype.saveMInOut(false);
            }
            else if ($self.cmbBankAccount != null) {
                if ($self.dGrid != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        //JID_ 1743 if Account Date is not selected showing message
                        if ($self.dGrid.get(selection[item])["Date"] == "") {
                            VIS.ADialog.error("VIS_PLAccountDate")
                            return false;
                        }

                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.C_Payment_ID_K == $self.dGrid.get(selection[item])["C_Payment_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }

                VIS.VCreateFromStatement.prototype.saveStatment();
                $self.middelDiv.css("height", "61%");
            }
            else {
                if ($self.dGrid != null && C_Order_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                if ($self.dGrid != null && M_InOut_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_InOut_ID_K"] && n.M_InOut_ID_K == $self.dGrid.get(selection[item])["M_InOut_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                // When provisional invoice selected by VA228(Rakesh) 29/July/2021
                if ($self.dGrid != null && C_ProvisionalInvoice_ID != null) {
                    for (item in selection) {
                        // Check duplicate key and add value to dictionary object
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Invoice_ID_K == $self.dGrid.get(selection[item])["C_Invoice_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                        }
                    }
                }
                if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                    VIS.VCreateFormProvisionalInvoice.prototype.saveProvisionalInvoice();
                }
                else {
                    VIS.VCreateFromInvoice.prototype.saveInvoice();
                }
            }
            //$self.setBusy(false);
            //if (output == false) {
            //    $self.setBusy(false);
            //    $self.$root.dialog('close');    
            //$self.editedItems = [];
            //}
        });

        this.Applybtn.on("click", function () {
            var output = false;
            $self.setBusy(true);
            var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
            var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
            var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();

            var selection = null;
            if ($self.dGrid != null) {
                selection = $self.dGrid.getSelection();
            }
            if (selection == null || selection.length == 0) {
                VIS.ADialog.info("RecordSelectionRequired");
                $self.setBusy(false);
                return;
            }
            if ($self.locatorField != null) {
                // Change By Mohit 30/06/2016
                //$self.multiValues = [];
                //for shipment haveing locator filed                
                if ($self.dGrid != null && C_Order_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                else if ($self.dGrid != null && C_Invoice_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Invoice_ID_K == $self.dGrid.get(selection[item])["C_Invoice_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                // Change By Mohit 30/06/2016
                $self.MergeItemsForSave();
                VIS.VCreateFromShipment.prototype.saveMInOut(true);
            }
            else if ($self.cmbBankAccount != null) {
                if ($self.dGrid != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        //JID_ 1743 if Account Date is not selected showing message
                        if ($self.dGrid.get(selection[item])["Date"] == "") {
                            VIS.ADialog.error("VIS_PLAccountDate")
                            return false;
                        }

                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.C_Payment_ID_K == $self.dGrid.get(selection[item])["C_Payment_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                VIS.VCreateFromStatement.prototype.saveStatment();
                $self.middelDiv.css("height", "61%");
            }
            else {
                if ($self.dGrid != null && C_Order_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                if ($self.dGrid != null && M_InOut_ID != null) {
                    //var selection = $self.dGrid.getSelection();
                    for (item in selection) {
                        var obj = $.grep($self.multiValues, function (n, i) {
                            return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_InOut_ID_K"] && n.M_InOut_ID_K == $self.dGrid.get(selection[item])["M_InOut_ID_K"]
                        });
                        if (obj.length > 0) {

                        }
                        else {
                            $self.multiValues.push($self.dGrid.get(selection[item]));
                            //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
                            //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
                        }
                    }
                }
                VIS.VCreateFromInvoice.prototype.saveInvoice();
            }
        });

        VCreateFrom.prototype.callBackSave = function (output) {
            if (output == false) {
                $self.setBusy(false);
            }
            else {
                // Change By Mohit 30/06/2016
                $self.isApplied = true;
                VIS.ADialog.info("VIS_SuccessFullyInserted", null, null, null);
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                var deliveryDate = $self.deliveryDate.getValue();
                if (C_Order_ID > 0 || C_Invoice_ID > 0) {
                    if ($self.locatorField != null) {
                        //for shipment haveing locator filed
                        //$self.loadOrder(C_Order_ID, false);
                        $self.dGrid.selectNone();
                        if (C_Order_ID > 0) {
                            $self.multiValues = [];
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);

                        }
                        if (C_Invoice_ID > 0) {
                            $self.multiValues = [];
                            VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, 1);
                        }
                        $self.dGrid.selectNone();
                        $self.editedItems = [];
                    }
                    else {
                        //for invoice
                        //$self.loadOrder(C_Order_ID, true);
                        $self.dGrid.selectNone();
                        if (C_Order_ID > 0) {
                            $self.multiValues = [];
                            $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, 1);
                        }
                        if (C_Invoice_ID > 0) {
                            $self.multiValues = [];
                            VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, 1);
                        }
                        $self.dGrid.selectNone();
                        $self.editedItems = [];
                    }
                }
            }
        };

        this.cancelbtn.on("click", function () {
            $self.setBusy(false);
            $self.$root.dialog('close');
        });

        var toggal = true;
        this.selectAllButton.on("click", function () {
            //if ($self.dGrid != null) {
            //    if (toggal) {
            //        $(this.children[0]).attr('src', './Areas/VIS/Images/base/check-icon.png');
            //        w2ui.gridCreateForm.selectAll()
            //        toggal = false;
            //    }
            //    else {
            //        $(this.children[0]).attr('src', './Areas/VIS/Images/base/uncheck-icon.png');
            //        w2ui.gridCreateForm.selectNone()
            //        toggal = true;
            //    }
            //}

        });

        if (this.relatedToOrg) {
            this.relatedToOrg.getControl().on("click", function () {
                $self.isApplied = false;
                $self.setBusy(true);
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                var C_ProvisionalInvoice_Id = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                if (C_Order_ID != null) {
                    $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                    $self.cmbShipment.getControl().prop('selectedIndex', -1);
                    //$self.loadOrder(C_Order_ID, false);
                    var M_Product_ID = $self.vProduct.getValue();
                    var deliveryDate = $self.deliveryDate.getValue();
                    $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);
                } else if (C_ProvisionalInvoice_Id != null) {
                    // VA228:Load provisional invoice data into grid
                    $self.cmbOrder.getControl().prop('selectedIndex', -1);
                    $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                    $self.cmbShipment.getControl().prop('selectedIndex', -1);
                    var M_Product_ID = $self.vProduct.getValue();
                    VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_Id, M_Product_ID, 1);
                }
                else {
                    $self.setBusy(false);
                }
            });
        }

        if (this.btnRefresh) {
            this.btnRefresh.on("click", function () {
                if ($self.cmbBankAccount != null) {
                    $self.setBusy(true);
                    var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    if (C_BankAccount_ID) {
                        var trxDate = $self.Date.getValue();
                        var C_BPartner_ID = $self.vBPartner.getValue();
                        var DepositSlip = $self.DepositSlip.getValue();
                        var DocumentNo = $self.DocumentNo.getValue();
                        var AuthCode = $self.AuthCode.getValue();
                        var CheckNo = $self.CheckNo.getValue();
                        var Amount = $self.Amount.getValue();
                        VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, $self.cmbPage.val());
                        //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, $self.cmbPage.val());
                    }
                    //$self.setBusy(false);
                }
            });
        }

        if (this.cmbOrder) {
            this.cmbOrder.getControl().change(function () {
                $self.isApplied = false;
                $self.setBusy(true);
                $self.editedItems = [];
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                //  set Invoice and Shipment to Null
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                $self.cmbProvisionalInvoice.getControl().prop('selectedIndex', -1);
                //$self.lblDate.setVisible = false;
                //$self.Date.setVisible = false;
                //$self.lblDocumentNo.setVisible = false;
                //$self.DocumentNo.setVisible = false;
                //$self.lblDepositSlip.setVisible = false;
                //$self.DepositSlip.setVisible = false;
                //$self.lblAuthCode.setVisible = false;
                //$self.AuthCode.setVisible = false;
                $self.lblDeliveryDate.setVisible(true);
                $self.deliveryDate.setVisible(true);
                var M_Product_ID = $self.vProduct.getValue();
                var deliveryDate = $self.deliveryDate.getValue();
                if ($self.locatorField != null) {
                    //for shipment haveing locator filed
                    //$self.loadOrder(C_Order_ID, false);
                    $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);
                }
                else {
                    //for invoice
                    //$self.loadOrder(C_Order_ID, true);
                    $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, 1);
                }
                //$self.setBusy(false);
            });
        }

        if (this.cmbBankAccount) {
            this.cmbBankAccount.getControl().change(function () {
                $self.editedItems = [];
                $self.setBusy(true);
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                if (C_BankAccount_ID) {
                    VIS.VCreateFromStatement.prototype.initBPartner(C_BankAccount_ID);
                    $self.Date.setValue("");
                    $self.DepositSlip.setValue("");
                    $self.DocumentNo.setValue("");
                    $self.AuthCode.setValue("");
                    var C_BPartner_ID = $self.vBPartner.getValue();
                    var trxDate = $self.Date.getValue();
                    var DepositSlip = $self.DepositSlip.getValue();
                    var DocumentNo = $self.DocumentNo.getValue();
                    var AuthCode = $self.AuthCode.getValue();
                    var CheckNo = $self.CheckNo.getValue();
                    var Amount = $self.Amount.getValue();
                    VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);

                    //VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, 1);
                }
                //$self.setBusy(false);
            });
        }

        if (this.Date) {
            this.Date.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        if (this.DocumentNo) {
            this.DocumentNo.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        if (this.DepositSlip) {
            this.DepositSlip.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        if (this.AuthCode) {
            this.AuthCode.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        // On change of Check No
        if (this.CheckNo) {
            this.CheckNo.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        // On change of Amount
        if (this.Amount) {
            this.Amount.getControl().change(function () {
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var trxDate = $self.Date.getValue();
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                var C_BPartner_ID = $self.vBPartner.getValue();
                var DepositSlip = $self.DepositSlip.getValue();
                var DocumentNo = $self.DocumentNo.getValue();
                var AuthCode = $self.AuthCode.getValue();
                var CheckNo = $self.CheckNo.getValue();
                var Amount = $self.Amount.getValue();
                VIS.VCreateFromStatement.prototype.loadBankAccounts(C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, 1);
            });
        }

        if (this.cmbInvoice) {
            this.cmbInvoice.getControl().change(function () {
                $self.isApplied = false;
                $self.setBusy(true);
                $self.editedItems = [];
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                //  set Order and Shipment to Null
                $self.cmbOrder.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                $self.cmbProvisionalInvoice.getControl().prop('selectedIndex', -1);
                //$self.lblDate.setVisible = false;
                //$self.Date.setVisible = false;
                //$self.lblDocumentNo.setVisible = false;
                //$self.DocumentNo.setVisible = false;
                //$self.lblDepositSlip.setVisible = false;
                //$self.DepositSlip.setVisible = false;
                //$self.lblAuthCode.setVisible = false;
                //$self.AuthCode.setVisible = false;
                $self.lblDeliveryDate.setVisible(false);
                $self.deliveryDate.setVisible(false);
                VIS.VCreateFromShipment.prototype.loadInvoices(C_Invoice_ID, M_Product_ID, 1);
                //$self.setBusy(false);
            });
        }

        if (this.cmbShipment) {
            this.cmbShipment.getControl().change(function () {
                $self.setBusy(true);
                $self.editedItems = [];
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                //  set Order and Shipment to Null
                $self.cmbOrder.getControl().prop('selectedIndex', -1);
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                $self.cmbProvisionalInvoice.getControl().prop('selectedIndex', -1);
                //$self.lblDate.setVisible = false;
                //$self.Date.setVisible = false;
                //$self.lblDocumentNo.setVisible = false;
                //$self.DocumentNo.setVisible = false;
                //$self.lblDepositSlip.setVisible = false;
                //$self.DepositSlip.setVisible = false;
                //$self.lblAuthCode.setVisible = false;
                //$self.AuthCode.setVisible = false;
                $self.lblDeliveryDate.setVisible(false);
                $self.deliveryDate.setVisible(false);
                if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                    VIS.VCreateFormProvisionalInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                }
                else {
                    VIS.VCreateFromInvoice.prototype.loadShipments(M_InOut_ID, M_Product_ID, 1);
                }
                //$self.setBusy(false);
            });
        }

        if (this.deliveryDate) {
            this.deliveryDate.getControl().change(function () {
                $self.isApplied = false;
                $self.setBusy(true);
                $self.multiValues = [];
                $self.editedItems = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                var deliveryDate = $self.deliveryDate.getValue();
                if ($self.locatorField != null) {
                    //for shipment haveing locator filed
                    $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, false, 1);
                }
                else {
                    //for invoice
                    $self.loadOrders(C_Order_ID, M_Product_ID, deliveryDate, true, 1);
                }
                //$self.setBusy(false);
            });
        }

        if (this.cmbProvisionalInvoice) {
            this.cmbProvisionalInvoice.getControl().change(function () {
                //Load data into grid on provisional invoice combo change
                $self.isApplied = false;
                $self.setBusy(true);
                $self.editedItems = [];
                $self.multiValues = [];
                if ($self.dGrid != null) {
                    $self.dGrid.destroy();
                    $self.dGrid = null;
                }
                var C_ProvisionalInvoice_ID = $self.cmbProvisionalInvoice.getControl().find('option:selected').val();
                var M_Product_ID = $self.vProduct.getValue();
                //  set Order/Invoice and Shipment to Null
                $self.cmbOrder.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                $self.lblDeliveryDate.setVisible(false);
                $self.deliveryDate.setVisible(false);
                VIS.VCreateFromInvoice.prototype.loadProvisionalInvoices(C_ProvisionalInvoice_ID, M_Product_ID, 1);
            });
        }

        // This function is used to open Product container in Tree structure
        if (this.ContainerTree) {
            this.ContainerTree.on("click", function () {
                if (VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"] != undefined) {
                    if (!VIS.context.ctx["#PRODUCT_CONTAINER_APPLICABLE"].equals("Y", true)) {
                        return;
                    }
                }
                // get Locator Value - if found then open container else not
                var locator = $self.locatorField.getValue();
                if (locator > 0) {
                    var obj = new VIS.productContainerTree(0, locator, 0);
                    if (obj != null) {
                        obj.showDialog();

                        obj.onClosing = function (containerId, containerText) {
                            // set combo value - 
                            $self.cmbContainer.getControl().empty();
                            $self.cmbContainer.getControl().append(" <option value=" + containerId + ">" + containerText + "</option>");
                        };
                    }
                    obj = null;
                }
                locator = null;
            });
        }
    };

    VCreateFrom.prototype.isInitOK = function () {
        return this.initOK;
    }

    // Changes as per new Fields in search criteria
    VCreateFrom.prototype.loadOrders = function (C_Order_ID, M_Product_ID, DeliveryDate, forInvoice, pNo) {
        //_order = new MOrder(Env.getCtx(), C_Order_ID, null);      //  save

        var data = null;
        data = this.getOrdersData(VIS.Env.getCtx(), C_Order_ID, M_Product_ID, DeliveryDate, forInvoice, pNo);
        //this.loadGrid(data);
    }

    // get order
    VCreateFrom.prototype.getOrdersData = function (ctx, C_Order_ID, M_Product_ID, DeliveryDate, forInvoice, pNo) {
        var data = [];
        var sql = "";
        if ($self.dGrid != null) {
            var selection = $self.dGrid.getSelection();
            for (item in selection) {
                var obj = $.grep($self.multiValues, function (n, i) {
                    return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
                });
                if (obj.length > 0) {

                }
                else {
                    $self.multiValues.push($self.dGrid.get(selection[item]));
                }
            }
        }
        if ($self.mTab.keyColumnName == "M_InOut_ID") {
            $self.record_ID = $self.mTab.getValue("M_InOut_ID");
        }
        else if ($self.mTab.keyColumnName == "C_Invoice_ID") {
            $self.record_ID = $self.mTab.getValue("C_Invoice_ID");
        }
        else if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
            $self.record_ID = $self.mTab.getValue("C_ProvisionalInvoice_ID");
        }

        if ($self.relatedToOrg != null && $self.relatedToOrg.getValue()) {
            var isBaseLanges = "";
            var MProductIDs = "";
            var DelivDate = "";
            var dateCret = "";
            if (!pNo) {
                pNo = 1;
            }

            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                isBaseLanges = " LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)";
            }
            else {
                isBaseLanges = " LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx).concat("')");
            }
            if (M_Product_ID != null) {
                MProductIDs = " AND l.M_Product_ID=" + M_Product_ID;
            }
            if (DeliveryDate != null) {
                var date = VIS.DB.to_date(DeliveryDate, true);
                dateCret = date;
                DelivDate = " AND l.DatePromised <= " + date;
            }
            var adOrgIDS = $self.mTab.getValue("AD_Org_ID")


            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetOrdersDataCommon",
                dataType: "json",
                type: "POST",
                data: {
                    keyColumnName: $self.mTab.keyColumnName,
                    tableName: "C_OrderLine",
                    recordID: $self.record_ID,
                    pageNo: pNo,
                    forInvoicees: forInvoice,
                    C_Ord_IDs: C_Order_ID,
                    isBaseLangess: isBaseLanges,
                    MProductIDss: MProductIDs,
                    DelivDates: DelivDate,
                    adOrgIDSs: adOrgIDS,
                },
                error: function (e) {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    $self.setBusy(false);
                    $self.log.info(e);
                    return;
                },
                success: function (dyndata) {

                    var res = JSON.parse(dyndata);
                    if (res.Error) {
                        VIS.ADialog.error(res.Error);
                        $self.setBusy(false);
                        return;
                    }
                    $self.resetPageCtrls(res.pSetting);
                    $self.loadGrid(res.data);
                    $self.setBusy(false);
                }
            });
        }
        else {
            var isBaseLanges = "";
            var MProductIDs = "";
            var DelivDate = "";
            var dateCret = "";

            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                isBaseLanges = " LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)";
            }
            else {
                // JID_1720
                isBaseLanges = " LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx) + "') INNER JOIN C_UOM uom1 ON uom1.C_UOM_ID = uom.C_UOM_ID ";
            }
            if (M_Product_ID > 0) {
                MProductIDs = " AND l.M_Product_ID=" + M_Product_ID;
            }
            if (DeliveryDate != null) {
                var date = VIS.DB.to_date(DeliveryDate, true);
                DelivDate = " AND l.DatePromised <= " + date;
            }

            if (!pNo) {
                pNo = 1;
            }
            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetOrdersDataCommonOrg",
                dataType: "json",
                type: "POST",
                data: {
                    keyColumnName: $self.mTab.keyColumnName,
                    tableName: "C_OrderLine",
                    recordID: $self.record_ID,
                    pageNo: pNo,

                    forInvoicees: forInvoice,
                    C_Ord_IDs: C_Order_ID,
                    isBaseLangess: isBaseLanges,
                    MProductIDss: MProductIDs,
                    DelivDates: DelivDate,
                },
                error: function (e) {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    $self.setBusy(false);
                    $self.log.info(e);
                    return;
                },
                success: function (dyndata) {

                    var res = JSON.parse(dyndata);
                    if (res.Error) {
                        VIS.ADialog.error(res.Error);
                        $self.setBusy(false);
                        return;
                    }
                    $self.resetPageCtrls(res.pSetting);
                    $self.loadGrid(res.data);
                    $self.setBusy(false);
                }
            });
        }
        return data;
    }

    //VCreateFrom.prototype.getOrdersData = function (ctx, C_Order_ID, M_Product_ID, DeliveryDate, forInvoice, pNo) {
    //    var data = [];
    //    var sql = "";
    //    if ($self.dGrid != null) {
    //        var selection = $self.dGrid.getSelection();
    //        for (item in selection) {
    //            var obj = $.grep($self.multiValues, function (n, i) {
    //                return n.M_Product_ID_K == $self.dGrid.get(selection[item])["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.get(selection[item])["C_Order_ID_K"]
    //            });
    //            if (obj.length > 0) {

    //            }
    //            else {
    //                $self.multiValues.push($self.dGrid.get(selection[item]));
    //                //if ($self.multiValues.indexOf($self.dGrid.get(selection[item])["M_Product_ID"]) == -1) {
    //                //    $self.multiValues.push($self.dGrid.get(selection[item])["M_Product_ID"]);
    //            }
    //        }
    //    }
    //    if ($self.mTab.keyColumnName == "M_InOut_ID") {
    //        $self.record_ID = $self.mTab.getValue("M_InOut_ID");
    //    }
    //    else if ($self.mTab.keyColumnName == "C_Invoice_ID") {
    //        $self.record_ID = $self.mTab.getValue("C_Invoice_ID");
    //    }
    //    // Enable this check
    //    // if (window.DTD001) {
    //    if ($self.relatedToOrg.getValue()) {
    //        sql = ("SELECT "
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
    //          + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
    //          + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
    //          + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
    //          + " ins.description , "
    //          + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
    //          + " FROM C_OrderLine l"
    //           + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

    //        sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
    //        sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

    //        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
    //        }
    //        else {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
    //        }

    //        sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");
    //        sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID);
    //        if (M_Product_ID != null) {
    //            sql = sql.concat(" AND l.M_Product_ID=" + M_Product_ID);
    //        }
    //        if (DeliveryDate != null) {
    //            var date = VIS.DB.to_date(DeliveryDate, true);
    //            sql = sql.concat(" AND l.DatePromised <= " + date);
    //        }
    //        sql = sql.concat(" AND l.DTD001_Org_ID = " + $self.mTab.getValue($self.windowNo, "AD_Org_ID")
    //            + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
    //            + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
    //                + "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description  "
    //            + "ORDER BY l.Line");
    //    }
    //    else {
    //        sql = ("SELECT "
    //           + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
    //           + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
    //           + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
    //           + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
    //           + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
    //          // + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
    //           + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,p.Name as PRODUCT,"	//	5..6
    //           + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
    //           + " ins.description , "
    //           + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
    //           + " FROM C_OrderLine l"
    //            + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

    //        sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
    //        //sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");
    //        sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)");

    //        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
    //        }
    //        else {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
    //        }

    //        sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");
    //        sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID + " AND L.M_Product_ID>0 "); //Edited By Arpit Rai on 25 March,2017
    //        if (M_Product_ID > 0) {
    //            sql = sql.concat(" AND l.M_Product_ID=" + M_Product_ID);
    //        }
    //        if (DeliveryDate != null) {
    //            var date = VIS.DB.to_date(DeliveryDate, true);
    //            sql = sql.concat(" AND l.DatePromised <= " + date);
    //        }
    //        sql = sql.concat(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
    //                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
    //                //+ "l.M_Product_ID,COALESCE(p.Name,c.Name), l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description  ");
    //                + "l.M_Product_ID,p.Name, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description  ");
    //        //+ "ORDER BY COALESCE(p.Name,c.Name),ins.description"); //commented by arpit on 25 March,2017
    //        //Added Arpit
    //        sql = sql.concat("UNION SELECT "
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "					//	1               
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
    //          + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
    //          + " 0 as M_PRODUCT_ID , c.Name as PRODUCT,"	//	5..6
    //          + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
    //          + " ins.description , "
    //          + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
    //          + " FROM C_OrderLine l"
    //           + " LEFT OUTER JOIN C_INVOICELINE M ON(L.C_OrderLine_ID=M.C_OrderLine_ID) AND ");

    //        sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
    //        sql = sql.concat(" IS NOT NULL").concat(" LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

    //        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
    //        }
    //        else {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
    //        }

    //        sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");
    //        sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID + " AND C.C_Charge_ID >0 ");
    //        if (DeliveryDate != null) {
    //            var date = VIS.DB.to_date(DeliveryDate, true);
    //            sql = sql.concat(" AND l.DatePromised <= " + date);
    //        }
    //        sql = sql.concat(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
    //                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
    //                + "l.M_Product_ID,c.Name, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description  ");
    //        //Arpit
    //    }

    //    var sqlNew = "SELECT * FROM (" + sql.toString() + ") WHERE QUANTITY > 0";
    //    if (!pNo) {
    //        pNo = 1;
    //    }
    //    $.ajax({
    //        url: VIS.Application.contextUrl + "Common/GetData",
    //        dataType: "json",
    //        type: "POST",
    //        data: {
    //            sql: sqlNew,
    //            keyColumnName: $self.mTab.keyColumnName,
    //            tableName: "C_OrderLine",
    //            recordID: $self.record_ID,
    //            pageNo: pNo
    //        },
    //        error: function () {
    //            alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
    //            $self.setBusy(false);
    //            return;
    //        },
    //        success: function (dyndata) {

    //            var res = JSON.parse(dyndata);
    //            if (res.Error) {
    //                VIS.ADialog.error(res.Error);
    //                $self.setBusy(false);
    //                return;
    //            }
    //            $self.resetPageCtrls(res.pSetting);
    //            $self.loadGrid(res.data);
    //            $self.setBusy(false);
    //        }
    //    });

    //    //var dr = null;
    //    //try {
    //    //    dr = VIS.DB.executeReader(sqlNew.toString(), null, null);
    //    //    console.log(sqlNew.toString());
    //    //    var count = 1;
    //    //while (dr.read()) {
    //    //    var line = {};
    //    //    var select = false;
    //    //    var rec = 0;
    //    //    if ($self.mTab.keyColumnName == "M_InOut_ID") {
    //    //        if (dr.getInt("c_orderline_id") > 0) {
    //    //            sql = "SELECT Count(*) FROM M_InOutLine WHERE M_InOut_ID = " + $self.mTab.getValue("M_InOut_ID") + " AND C_OrderLine_ID = " + dr.getInt("c_orderline_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        else {
    //    //            sql = "SELECT Count(*) FROM M_InOutLine WHERE M_InOut_ID = " + $self.mTab.getValue("M_InOut_ID") + " AND M_Product_ID = " + dr.getInt("m_product_id") + " AND M_AttributeSetInstance_ID = " + dr.getInt("m_attributesetinstance_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //    }
    //    //    else if ($self.mTab.keyColumnName == "C_Invoice_ID") {
    //    //        if (dr.getInt("c_orderline_id") > 0) {
    //    //            sql = "SELECT Count(*) FROM C_InvoiceLine WHERE C_Invoice_ID = " + $self.mTab.getValue("C_Invoice_ID") + " AND C_OrderLine_ID = " + dr.getInt("c_orderline_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //        else {
    //    //            sql = "SELECT Count(*) FROM C_InvoiceLine WHERE C_Invoice_ID = " + $self.mTab.getValue("C_Invoice_ID") + " AND M_Product_ID = " + dr.getInt("m_product_id") + " AND M_AttributeSetInstance_ID = " + dr.getInt("m_attributesetinstance_id");
    //    //            rec = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql));
    //    //            if (rec > 0) {
    //    //                select = true;
    //    //            }
    //    //        }
    //    //    }
    //    //    line['Select'] = select;
    //    //    line['Quantity'] = dr.getString("quantity");  //  1-Qty
    //    //    line['QuantityEntered'] = dr.getString("qtyenter");  //  2-Qty
    //    //    line['C_UOM_ID'] = dr.getString("uom");    //  3-UOM
    //    //    line['M_Product_ID'] = dr.getString("product");    //  4-Product
    //    //    line['M_AttributeSetInstance_ID'] = dr.getString("description");        //  5-Ship -Key
    //    //    line['C_Order_ID'] = dr.getString("line");      //  4-OrderLine
    //    //    line['M_InOut_ID'] = null;        //  5-Ship
    //    //    line['C_Invoice_ID'] = null;        //  6-Invoice
    //    //    // line['Att'] = dr.getString("description");
    //    //    line['C_UOM_ID_K'] = dr.getString("c_uom_id");    //  2-UOM -Key
    //    //    line['M_Product_ID_K'] = dr.getString("m_product_id");      //  3-Product -Key
    //    //    line['M_AttributeSetInstance_ID_K'] = dr.getString("m_attributesetinstance_id");        //  5-Ship -Key
    //    //    line['C_Order_ID_K'] = dr.getString("c_orderline_id");      //  4-OrderLine -Key
    //    //    line['M_InOut_ID_K'] = null;        //  5-Ship -Key
    //    //    line['C_Invoice_ID_K'] = null;        //  6-Invoice -Key
    //    //    line['recid'] = count;
    //    //    count++;
    //    //    data.push(line);
    //    //}
    //    //dr.close();
    //    //}
    //    //catch (e) {
    //    //    // s_log.log(Level.SEVERE, sql.toString(), e);
    //    //}
    //    return data;
    //}

    // end of new changes by Bharat

    VCreateFrom.prototype.loadOrder = function (C_Order_ID, forInvoice) {
        //_order = new MOrder(Env.getCtx(), C_Order_ID, null);      //  save

        var data = null;
        data = this.getOrderData(VIS.Env.getCtx(), C_Order_ID, forInvoice);
        this.loadGrid(data);
    }

    VCreateFrom.prototype.getOrderData = function (ctx, C_Order_ID, forInvoice) {
        var data = [];
        var sql = "";
        if ($self.relatedToOrg.getValue()) {
            var isBaseLang = "";
            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                isBaseLang = " LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)";
            }
            else {
                isBaseLang = " LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx) + "')";
            }
            var orggetVal = $self.mTab.getValue($self.windowNo, "AD_Org_ID");
            var lang = VIS.Env.getAD_Language(ctx);
            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetOrderDataCommons",
                dataType: "json",
                type: "POST",
                async: false,
                data: {
                    forInvoices: forInvoice,
                    isBaseLangs: isBaseLang,
                    C_OrderID: C_Order_ID,
                    orggetVals: orggetVal,
                    langs: lang
                },
                error: function (e) {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    $self.setBusy(false);
                    $self.log.info(e);
                    return;
                },
                success: function (dyndata) {
                    var res = JSON.parse(dyndata);

                    if (res && res != null && res.length > 0) {
                        var count = 1;
                        for (var i = 0; i < res.length; i++) {
                            var line = {};
                            line['Quantity'] = res[i]["quantity"];
                            line['QuantityEntered'] = res[i]["quantityentered"];
                            line['C_UOM_ID'] = res[i]["c_uom_id"];
                            line['M_Product_ID'] = res[i]["m_product_id"];
                            line['M_AttributeSetInstance_ID'] = res[i]["m_attributesetinstance_id"];
                            line['C_Order_ID'] = res[i]["c_order_id"];
                            line['M_InOut_ID'] = null;
                            line['C_Invoice_ID'] = null;
                            line['C_UOM_ID_K'] = res[i]["c_uom_id_k"];
                            line['M_Product_ID_K'] = res[i]["m_product_id_k"];
                            line['M_AttributeSetInstance_ID_K'] = res[i]["m_attributesetinstance_id_k"];
                            line['C_Order_ID_K'] = res[i]["c_order_id_k"];
                            line['M_InOut_ID_K'] = null;
                            line['C_Invoice_ID_K'] = null;
                            line['C_PaymentTerm_ID'] = res[i]["C_PaymentTerm_ID"];
                            line['PaymentTermName'] = res[i]["PaymentTermName"];
                            line['IsAdvance'] = res[i]["IsAdvance"];
                            line['C_InvoicePaymentTerm_ID'] = res[i]["C_InvoicePaymentTerm_ID"];
                            line['IsInvoicePTAdvance'] = res[i]["IsInvoicePTAdvance"];
                            line['recid'] = count;
                            data.push(line);
                        }
                    }
                    $self.setBusy(false);
                }
            });
            return data;
        }
        else {
            var isBaseLang = "";
            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                isBaseLang = " LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)";
            }
            else {
                isBaseLang = " LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + VIS.Env.getAD_Language(ctx) + "')";
            }
            var lang = VIS.Env.getAD_Language(ctx);

            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/GetOrderDataCommonsNotOrg",
                dataType: "json",
                type: "POST",
                async: false,
                data: {
                    forInvoices: forInvoice,
                    isBaseLangs: isBaseLang,
                    C_OrderID: C_Order_ID,
                    langs: lang
                },
                error: function (e) {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    $self.setBusy(false);
                    $self.log.info(e);
                    return;
                },
                success: function (dyndata) {
                    var res = JSON.parse(dyndata);

                    if (res && res != null && res.length > 0) {
                        var count = 1;
                        for (var i = 0; i < res.length; i++) {
                            var line = {};
                            line['Quantity'] = res[i]["quantity"];
                            line['QuantityEntered'] = res[i]["quantityentered"];
                            line['C_UOM_ID'] = res[i]["c_uom_id"];
                            line['M_Product_ID'] = res[i]["m_product_id"];
                            line['M_AttributeSetInstance_ID'] = res[i]["m_attributesetinstance_id"];
                            line['C_Order_ID'] = res[i]["c_order_id"];
                            line['M_InOut_ID'] = null;
                            line['C_Invoice_ID'] = null;
                            line['C_UOM_ID_K'] = res[i]["c_uom_id_k"];
                            line['M_Product_ID_K'] = res[i]["m_product_id_k"];
                            line['M_AttributeSetInstance_ID_K'] = res[i]["m_attributesetinstance_id_k"];
                            line['C_Order_ID_K'] = res[i]["c_order_id_k"];
                            line['M_InOut_ID_K'] = null;
                            line['C_Invoice_ID_K'] = null;
                            line['C_PaymentTerm_ID'] = res[i]["C_PaymentTerm_ID"];
                            line['PaymentTermName'] = res[i]["PaymentTermName"];
                            line['IsAdvance'] = res[i]["IsAdvance"];
                            line['C_InvoicePaymentTerm_ID'] = res[i]["C_InvoicePaymentTerm_ID"];
                            line['IsInvoicePTAdvance'] = res[i]["IsInvoicePTAdvance"];
                            if ($self.mTab.keyColumnName == "C_ProvisionalInvoice_ID") {
                                line['POPrice'] = res[i]["POPrice"];
                                line['ProvisionalPrice'] = res[i]["ProvisionalPrice"];
                            }
                            line['recid'] = count;
                            data.push(line);
                        }
                    }
                    $self.setBusy(false);
                }
            });

            return data;
        }

    }

    //VCreateFrom.prototype.getOrderData = function (ctx, C_Order_ID, forInvoice) {
    //    /**
    //     *  Selected        - 0
    //     *  Qty             - 1
    //     *  C_UOM_ID        - 2
    //     *  M_Product_ID    - 3
    //     *  OrderLine       - 4
    //     *  ShipmentLine    - 5
    //     *  InvoiceLine     - 6
    //     */

    //    var data = [];
    //    var sql = "";

    //    // Enable this check
    //    // if (window.DTD001) {
    //    if ($self.relatedToOrg.getValue()) {
    //        sql = ("SELECT "
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
    //          + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
    //          + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
    //          + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
    //          + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
    //          + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
    //          + " ins.description , "
    //          + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
    //          + " FROM C_OrderLine l"
    //           + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

    //        sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
    //        sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

    //        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
    //        }
    //        else {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
    //        }

    //        sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");

    //        sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID + " AND l.DTD001_Org_ID = " + $self.mTab.getValue($self.windowNo, "AD_Org_ID")
    //            + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
    //            + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
    //                + "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description  "
    //            + "ORDER BY l.Line");
    //    }
    //    else {
    //        sql = ("SELECT "
    //           + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
    //           + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
    //           + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
    //           + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
    //           + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
    //           + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
    //           + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
    //           + " ins.description , "
    //           + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
    //           + " FROM C_OrderLine l"
    //            + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

    //        sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
    //        sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

    //        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
    //        }
    //        else {
    //            sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
    //        }

    //        sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");

    //        sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID			//	#1
    //            + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
    //            + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
    //                + "l.M_Product_ID,COALESCE(p.Name,c.Name), l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description  "
    //            + "ORDER BY l.Line");
    //    }

    //    var sqlNew = "SELECT * FROM (" + sql.toString() + ") WHERE QUANTITY > 0";

    //    var dr = null;
    //    try {
    //        dr = VIS.DB.executeReader(sqlNew.toString(), null, null);
    //        console.log(sqlNew.toString());
    //        var count = 1;
    //        while (dr.read()) {
    //            var line = {};
    //            //line.push(false);       //  0-Selection

    //            ////var qtyOrdered = dr.getDecimal(0);
    //            ////var multiplier = dr.getDecimal(1);
    //            ////var qtyEntered = qtyOrdered * multiplier;
    //            //line['Quantity'] = qtyEntered;  //  1-Qty
    //            //line['C_UOM_ID'] = dr.getString(3);    //  2-UOM
    //            //line['M_Product_ID'] = dr.getString(5);      //  3-Product
    //            //line['C_Order_ID'] = dr.getString(7);;      //  4-OrderLine
    //            //line['M_InOut_ID'] = null;        //  5-Ship
    //            //line['C_Invoice_ID'] = null;        //  6-Invoice
    //            //line['C_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
    //            //line['M_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
    //            //line['C_Order_ID_K'] = dr.getInt(6);;      //  4-OrderLine -Key
    //            //line['M_InOut_ID_K'] = null;        //  5-Ship -Key
    //            //line['C_Invoice_ID_K'] = null;        //  6-Invoice -Key

    //            line['Quantity'] = dr.getString("quantity");  //  1-Qty
    //            line['QuantityEntered'] = dr.getString("qtyenter");  //  2-Qty
    //            line['C_UOM_ID'] = dr.getString("uom");    //  3-UOM
    //            line['M_Product_ID'] = dr.getString("product");    //  4-Product
    //            line['M_AttributeSetInstance_ID'] = dr.getString("description");        //  5-Ship -Key
    //            line['C_Order_ID'] = dr.getString("line");      //  4-OrderLine
    //            line['M_InOut_ID'] = null;        //  5-Ship
    //            line['C_Invoice_ID'] = null;        //  6-Invoice
    //            // line['Att'] = dr.getString("description");
    //            line['C_UOM_ID_K'] = dr.getString("c_uom_id");    //  2-UOM -Key
    //            line['M_Product_ID_K'] = dr.getString("m_product_id");      //  3-Product -Key
    //            line['M_AttributeSetInstance_ID_K'] = dr.getString("m_attributesetinstance_id");        //  5-Ship -Key
    //            line['C_Order_ID_K'] = dr.getString("c_orderline_id");      //  4-OrderLine -Key
    //            line['M_InOut_ID_K'] = null;        //  5-Ship -Key
    //            line['C_Invoice_ID_K'] = null;        //  6-Invoice -Key

    //            line['recid'] = count;
    //            count++;
    //            data.push(line);
    //        }
    //        dr.close();
    //    }
    //    catch (e) {
    //        // s_log.log(Level.SEVERE, sql.toString(), e);
    //    }
    //    return data;
    //}

    VCreateFrom.prototype.loadGrid = function (data) {
        var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
        var dotFormatter = VIS.Env.isDecimalPoint();
        if (this.dGrid != null) {
            this.dGrid.destroy();
            this.dGrid = null;
        }
        var isSoTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsSOTrx"));
        // VA228:Create new grid header when combo selection changed in case of AP Invoice
        if (this.arrListColumns.length == 0 || ($self.mTab.keyColumnName == "C_Invoice_ID" && !isSoTrx)) {
            this.arrListColumns = [];
            this.arrListColumns.push({
                field: "Select", caption: VIS.Msg.getMsg("Select"), sortable: false, size: '50px', hidden: false,
                //editable: { type: 'checkbox' }
                render: function () {
                    return '<div><input type=checkbox disabled="true"></div>';
                }
            });
            this.arrListColumns.push({
                field: "Quantity", caption: VIS.Msg.getMsg("QtyPending"), sortable: false, size: '150px', hidden: false,
                render: function (record, index, col_index) {
                    var val = record["Quantity"];
                    return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
                }
            });
            this.arrListColumns.push({ field: "QuantityPending", caption: VIS.Msg.getMsg("Quantity"), sortable: false, size: '150px', render: 'number:4', hidden: true });
            
            // VA228: Show AP provisional invoice fields when provisional invice combo selected from AP Invoice window
            if ($self.cmbProvisionalInvoice.getControl().find('option:selected').val() > 0 && !isSoTrx) {
                this.arrListColumns.push({ field: "QuantityEntered", caption: VIS.Msg.getMsg("QtyEntered"), sortable: false, size: '150px', hidden: false });
                this.arrListColumns.push({ field: "POPrice", caption: VIS.Msg.getMsg("POPrice"), sortable: false, size: '150px', hidden: false });
                this.arrListColumns.push({ field: "ProvisionalPrice", caption: VIS.Msg.getMsg("ProvisionalPrice"), sortable: false, size: '150px', hidden: false });
                this.arrListColumns.push({
                    field: "InvoicePrice", caption: VIS.Msg.getMsg("InvoicePrice"), editable: { type: 'number', style: 'background-color: #f8f8f8;' }, sortable: false, size: '150px', hidden: false,

                    render: function (record, index, col_index) {
                        var val = record["InvoicePrice"];
                        val = $self.checkcommaordot(event, val);
                        return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
                    },
                    style: 'background-color: #d1d1d14a'
                });
            } else {
                this.arrListColumns.push({
                    field: "QuantityEntered", caption: VIS.Msg.getMsg("QtyEntered"), editable: { type: 'number', style: 'background-color: #f8f8f8;' }, sortable: false, size: '150px', hidden: false,
                    //Added to get QuantityEntered value acc.to culture
                    render: function (record, index, col_index) {
                        var val = record["QuantityEntered"];
                        val = $self.checkcommaordot(event, val);
                        return parseFloat(val).toLocaleString(undefined, { minimumFractionDigits: 4 });
                    },
                    style: 'background-color: #d1d1d14a'
                });
            }
            this.arrListColumns.push({ field: "C_UOM_ID", caption: VIS.Msg.getMsg("UomName"), sortable: false, size: '150px', hidden: false });
            //Add product search key column in Grid 
            this.arrListColumns.push({ field: "M_Product_SearchKey", caption: VIS.Msg.getMsg("ProductSearchKey"), sortable: false, size: '150px', hidden: false });
            //Set sortable: true for Product Name in Grid // Pratap : 06/08/2016
            this.arrListColumns.push({ field: "M_Product_ID", caption: VIS.Msg.getMsg("ProductName"), sortable: true, size: '150px', hidden: false });
            this.arrListColumns.push({
                //field: "M_AttributeSetInstance_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_AttributeSetInstance_ID"), sortable: false, size: '150px', hidden: false,
                //render: function () {
                //    return '<div><input type=text readonly="readonly" style= "width:80%; border:none" ></input><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/MultiX16.png" alt="Attribute Set Instance" title="Attribute Set Instance" style="opacity:1;float:right;"></div>';
                //}
                field: "M_AttributeSetInstance_ID", caption: VIS.Msg.getMsg("ASI"), sortable: false, size: '150px', editable: { type: 'custom', ctrl: $self.$productAttribute, showAll: true, style: 'background-color: #f8f8f8;' },
                render: function (record, index, col_index) {
                    var l = $self.pattrLookup;
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

            // Issue JID_0564: payment term detail on order 
            this.arrListColumns.push({ field: "C_PaymentTerm_ID", caption: VIS.Msg.getMsg("C_PaymentTerm_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "PaymentTermName", caption: VIS.Msg.getMsg("PaymentTermName"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "IsAdvance", caption: VIS.Msg.getMsg("IsAdvance"), sortable: false, size: '150px', hidden: true });
            //Issue JID_0564: invoice header payment term detail
            this.arrListColumns.push({ field: "C_InvoicePaymentTerm_ID", caption: VIS.Msg.getMsg("C_InvoicePaymentTerm_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "IsInvoicePTAdvance", caption: VIS.Msg.getMsg("IsInvoicePTAdvance"), sortable: false, size: '150px', hidden: true });

            this.arrListColumns.push({ field: "C_Order_ID", caption: VIS.Msg.getMsg("OrderLine"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "M_InOut_ID", caption: VIS.Msg.getMsg("Shipment/Receipt"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "C_Invoice_ID", caption: VIS.Msg.getMsg("Invoice"), sortable: false, size: '150px', hidden: false });
            //this.arrListColumns.push({ field: "Att", caption: "Att", "type": "button", "value": "Att", sortable: true, size: '150px', hidden: false }),
            // Hidden -- > true
            this.arrListColumns.push({ field: "C_UOM_ID_K", caption: VIS.Msg.getMsg("UomName"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "M_Product_ID_K", caption: VIS.Msg.getMsg("ProductName"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "C_Order_ID_K", caption: VIS.Msg.getMsg("Order"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "M_InOut_ID_K", caption: VIS.Msg.getMsg("Shipment/Receipt"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "C_Invoice_ID_K", caption: VIS.Msg.getMsg("Invoice"), sortable: false, size: '150px', hidden: true });

        }

        w2utils.encodeTags(data);
        var grdname = 'gridCreateForm' + (this.windowNo);
        grdname = grdname.replace('.', '');
        this.dGrid = $(this.middelDiv).w2grid({
            name: grdname,
            recordHeight: 30,
            show: { selectColumn: true },
            multiSelect: true,
            columns: this.arrListColumns,
            records: data,
            onSelect: function (event) {
                event.onComplete = function () {
                    // Issue No : JID_0564 : make row non editable
                    CreateNonEditableRecord();
                }
            },
            // Added to get value acc. to culture while editing grid column
            onEditField: function (event) {
                id = event.recid;
                if (event.column == 3) {
                    $self.dGrid.records[event.index]["QuantityEntered"] = $self.checkcommaordot(event, $self.dGrid.records[event.index]["QuantityEntered"]);
                    var _value = format.GetFormatAmount($self.dGrid.records[event.index]["QuantityEntered"], "init", dotFormatter);
                    $self.dGrid.records[event.index]["QuantityEntered"] = format.GetConvertedString(_value, dotFormatter);
                    $("#grid_" + $self.dGrid.name + "_rec_" + id).keydown(function (event) {
                        if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                            return false;
                        }
                        else if (dotFormatter && event.keyCode == 188) { // . separator
                            return false;
                        }
                        if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf('.') > -1) {
                                event.target.value = event.target.value.replace('.', '');
                            }
                        }
                        else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf(',') > -1) {
                                event.target.value = event.target.value.replace(',', '');
                            }
                        }
                        if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                            (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                            && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                            && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                            return false;
                        }
                    });
                }

                // change event of Provisional Price
                if (event.column == 4 && $self.dGrid.columns[event.column].field == "ProvisionalPrice") {
                    $self.dGrid.records[event.index]["ProvisionalPrice"] = $self.checkcommaordot(event, $self.dGrid.records[event.index]["ProvisionalPrice"]);
                    var _value = format.GetFormatAmount($self.dGrid.records[event.index]["ProvisionalPrice"], "init", dotFormatter);
                    $self.dGrid.records[event.index]["ProvisionalPrice"] = format.GetConvertedString(_value, dotFormatter);
                    $self.dGrid.refreshRow(event.index + 1);//refresh a row of the grid
                    $("#grid_" + $self.dGrid.name + "_rec_" + id).keydown(function (event) {
                        if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                            return false;
                        }
                        else if (dotFormatter && event.keyCode == 188) { // . separator
                            return false;
                        }
                        if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf('.') > -1) {
                                event.target.value = event.target.value.replace('.', '');
                            }
                        }
                        else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf(',') > -1) {
                                event.target.value = event.target.value.replace(',', '');
                            }
                        }
                        if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                            (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                            && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                            && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                            return false;
                        }
                    });
                }

                // VA228:change event of Provisional Invoice Price (AP Window)
                if (event.column == 6 && $self.dGrid.columns[event.column].field == "InvoicePrice") {
                    $self.dGrid.records[event.index]["InvoicePrice"] = $self.checkcommaordot(event, $self.dGrid.records[event.index]["InvoicePrice"]);
                    var _value = format.GetFormatAmount($self.dGrid.records[event.index]["InvoicePrice"], "init", dotFormatter);
                    $self.dGrid.records[event.index]["InvoicePrice"] = format.GetConvertedString(_value, dotFormatter);
                    $self.dGrid.refreshRow(event.index + 1);//refresh a row of the grid
                    $("#grid_" + $self.dGrid.name + "_rec_" + id).keydown(function (event) {
                        if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                            return false;
                        }
                        else if (dotFormatter && event.keyCode == 188) { // . separator
                            return false;
                        }
                        if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf('.') > -1) {
                                event.target.value = event.target.value.replace('.', '');
                            }
                        }
                        else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                            if (event.target.value.indexOf(',') > -1) {
                                event.target.value = event.target.value.replace(',', '');
                            }
                        }
                        if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                            (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                            && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                            && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                            return false;
                        }
                    });
                }
            },
            //Implemented for BankStatement window (when click on Create Line From button),
            //change the Converted Amt according to when change the AcctDate on the grid -added by Koteswararao
            onChange: function (event) {
                window.setTimeout(function () {
                    if ($self.dGrid.getChanges(event.recid) != undefined) {
                        if (event.column == 0 && $self.dGrid.columns[event.column].field == "Date") {
                            //AD_Org_ID getting from the window.
                            var _orgId = VIS.context.getContextAsInt($self.windowNo, "AD_Org_ID");
                            //Actual Amount
                            var _amount = $self.dGrid.records[event.index].Amount;
                            //BankStatement C_Currency_ID
                            var _currency_Id = $self.dGrid.records[event.index].C_Currency_ID_K;
                            var _convsionId = $self.dGrid.records[event.index].C_ConversionType_ID;
                            //Account Date
                            var _date = $self.dGrid.records[event.index].Date;
                            //_type differentiate weather it is Payment or Cash Journal Line
                            var _type = $self.dGrid.records[event.index].Type;
                            var _payment_Id = $self.dGrid.records[event.index].C_Payment_ID_K;

                            $.ajax({
                                url: VIS.Application.contextUrl + "VCreateFrom/GetConvertedAmount",
                                dataType: "json",
                                type: "POST",
                                data: {
                                    _paymentId: _payment_Id, amount: _amount, currencyId: _currency_Id, convsion_Id: _convsionId, date: _date, paymentType: _type, _org_id: _orgId
                                },
                                error: function () {
                                    //if through error it will return below message.
                                    VIS.ADialog.info("ErrorWhileGettingData", null, "", "");
                                    self.$super.setBusy(false);
                                    return;
                                },
                                success: function (_list) {
                                    _list = JSON.parse(_list);
                                    if (VIS.Utility.Util.getValueOfString(_list._message) == "Success") {
                                        _convertedAmt = VIS.Utility.Util.getValueOfDecimal(_list.convtAmt);
                                        $self.dGrid.records[event.index].changes.ConvertedAmount = _convertedAmt;
                                        $self.dGrid.records[event.index]["ConvertedAmount"] = _convertedAmt;
                                        $self.dGrid.refreshRow(event.index + 1);//refresh a row of the grid
                                    }
                                    else {
                                        VIS.ADialog.info(_list._message, null, "", "");
                                    }
                                }
                            });
                        }
                    }
                }, 100);
            }
        });
        this.dGrid.selectNone();
        var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
        var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
        var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();

        for (itm in $self.multiValues) {
            for (item in $self.dGrid.records) {
                //if ($self.dGrid.records[item]["M_Product_ID"] == $self.multiValues[itm]) {
                //    $self.dGrid.select(parseInt(item) + 1);
                //}
                var obj = $.grep($self.multiValues, function (n, i) {
                    if (C_Order_ID != null) {
                        return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.records[item]["C_Order_ID_K"]
                    }
                    else if (C_Invoice_ID != null) {
                        return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.C_Invoice_ID_K == $self.dGrid.records[item]["C_Invoice_ID_K"]
                    }
                    else if (M_InOut_ID != null) {
                        return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.M_InOut_ID_K == $self.dGrid.records[item]["M_InOut_ID_K"]
                    }
                    else {
                        return n.C_Payment_ID_K == $self.dGrid.records[item]["C_Payment_ID_K"]
                    }
                });
                if (obj.length > 0) {

                    $self.dGrid.select(parseInt(item) + 1);
                }
            }
        }

        if (this.dGrid.records.length > 0) {
            $self.NonEditableRecord = [];
            for (var i = 0; i < $self.dGrid.records.length; i++) {
                //$("#grid_" + $self.dGrid.name + "_rec_" + (i + 1)).find("input[type=text]").val(data[i].M_AttributeSetInstance_ID);
                $($("#grid_" + $self.dGrid.name + "_rec_" + (i + 1)).find("input[type=checkbox]")[1]).prop("checked", data[i].Select);

                // Issue No : JID_0564 
                if (($self.dGrid.records[i]["C_InvoicePaymentTerm_ID"] == $self.dGrid.records[i]["C_PaymentTerm_ID"]) || $self.dGrid.records[i]["C_InvoicePaymentTerm_ID"] == 0) {
                    // not to disable record when payment term is same or for M_Inout table
                }
                else if (!($self.dGrid.records[i]["C_InvoicePaymentTerm_ID"] != $self.dGrid.records[i]["C_PaymentTerm_ID"]
                    && $self.dGrid.records[i]["IsAdvance"] == false && $self.dGrid.records[i]["IsInvoicePTAdvance"] == false)) {
                    // except payment term not matched and not advance
                    $("#grid_" + $self.dGrid.name + "_rec_" + (i + 1) + ' ' + ' input').attr('disabled', true);
                    $self.NonEditableRecord.push(i + 1);
                }
            }
        }

        function CreateNonEditableRecord() {
            for (var i = 0; i < $self.dGrid.records.length; i++) {
                // Issue JID_0564 : if index exist in non editable record array, then make that record read only and mark selected checkbox as false 
                if ($self.NonEditableRecord.indexOf(i + 1) >= 0) {
                    $self.dGrid.unselect(i + 1);
                    $("#grid_" + $self.dGrid.name + "_rec_" + (i + 1) + ' ' + ' input').attr('disabled', true);
                }
            }
        };

        this.dGrid.on("change", function (e) {
            CreateNonEditableRecord();
            if ($self.dGrid.columns[e.column].field == "QuantityEntered") {
                var _val = format.GetConvertedNumber(e.value_new, dotFormatter);
                if ($self.fromInvoice) {
                    if ($self.dGrid.records[e.index]["QuantityPending"] < _val.toFixed(4)) {
                        VIS.ADialog.error("InvoiceQtyGreater");
                        e.value_new = e.value_original;
                        return;
                    }
                }       // JID_0273: Create line from on material Recipt allow user set qty more than PO qty
                else {
                    if ($self.dGrid.records[e.index]["QuantityPending"] < _val.toFixed(4)) {
                        VIS.ADialog.error("QtyCanNotbeGreater");
                        e.value_new = e.value_original;
                        return;
                    }
                }
                $self.dGrid.records[e.index]["QuantityEntered"] = _val.toFixed(4);
                if (C_Order_ID > 0 || C_Invoice_ID > 0) {
                    AddEditedLine("QuantityEntered", e.index, _val.toFixed(4));
                }
            }
            if ($self.dGrid.columns[e.column].field == "ProvisionalPrice") {
                var _val = format.GetConvertedNumber(e.value_new, dotFormatter);
                if (_val < 0) {
                    VIS.ADialog.error("PriceCantNegative");
                    e.value_new = e.value_original;
                    return;
                }
                $self.dGrid.records[e.index]["ProvisionalPrice"] = _val.toFixed(4);
                AddEditedLine("ProvisionalPrice", e.index, _val.toFixed(4));
            } else if ($self.dGrid.columns[e.column].field == "InvoicePrice") {
                //VA228:Check invoice price and apply amount format
                var _val = format.GetConvertedNumber(e.value_new, dotFormatter);
                if (_val < 0) {
                    VIS.ADialog.error("PriceCantNegative");
                    e.value_new = e.value_original;
                    return;
                }
                $self.dGrid.records[e.index]["InvoicePrice"] = _val.toFixed(4);
                AddEditedLine("InvoicePrice", e.index, _val.toFixed(4));
            }
            else if ($self.dGrid.columns[e.column].field == "M_AttributeSetInstance_ID") {
                $self.dGrid.records[e.index]["M_AttributeSetInstance_ID"] = e.value_new;
                if (C_Order_ID > 0 || C_Invoice_ID > 0) {
                    AddEditedLine("M_AttributeSetInstance_ID", e.index, e.value_new);
                }
            }
            // JID_1743: Need to make the Account date as editable on Create lines from Bank Statement.
            else if ($self.dGrid.columns[e.column].field == "Date") {
                //handled Invalid Date when user click on Account Date and not 
                //selected Date just Double clicked and exist form that edit field - Added by Koteswararao
                if (e.value_new != "") {
                    $self.dGrid.records[e.index]["Date"] = e.value_new;
                }
                else if (e.value_previous != "") {
                    $self.dGrid.records[e.index]["Date"] = e.value_previous;
                }
                else if (e.value_original != "") {
                    $self.dGrid.records[e.index]["Date"] = e.value_original;
                }
                if ($self.dGrid.records[e.index]["Date"] != "") {
                    //AddEditedLine("Date", e.index, e.value_new);
                    AddEditedLine("Date", e.index, $self.dGrid.records[e.index]["Date"]);
                }
            }
        });
        // Change By Mohit 30/06/2016
        function AddEditedLine(column, index, new_value) {
            var selectprd = $self.dGrid.records[index]["M_Product_ID_K"];

            //if ($self.multiValues.indexOf(unselectedVal) > -1) {
            //    $self.multiValues.splice($self.multiValues.indexOf(unselectedVal), 1);
            //}
            var obj = $.grep($self.editedItems, function (n, i) {
                if (C_Order_ID != null) {
                    return n.M_Product_ID_K == selectprd && n.C_Order_ID_K == $self.dGrid.records[index]["C_Order_ID_K"]
                }
                else if (C_Invoice_ID != null) {
                    return n.M_Product_ID_K == selectprd && n.C_Invoice_ID_K == $self.dGrid.records[index]["C_Invoice_ID_K"]
                }
                else if (M_InOut_ID != null) {
                    return n.M_Product_ID_K == selectprd && n.M_InOut_ID_K == $self.dGrid.records[index]["M_InOut_ID_K"]
                }
                else {
                    return n.C_Payment_ID_K == $self.dGrid.records[item]["C_Payment_ID_K"]
                }
            });
            if (obj.length > 0) {
                $self.editedItems.splice($self.editedItems.indexOf(obj[0]), 1);
                $self.editedItems.push($self.dGrid.records[index]);
            }
            else {
                //can't add duplicate records in editedItems, so that implemented below flag condition
                var flag = 0;
                for (var i = 0; i < $self.editedItems.length; i++) {
                    if ($self.editedItems[i].recid == $self.dGrid.records[index].recid) {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0) {
                    $self.editedItems.push($self.dGrid.records[index]);
                }
            }
        };

        //this.dGrid.on("click", function (e) {
        //    e.preventDefault;
        //    if ($self.dGrid.columns[e.column].field == "M_AttributeSetInstance_ID") {
        //        var mattsetid = 0;

        //        var mProductidK = $self.dGrid.records[e.recid - 1]["M_Product_ID_K"]
        //        $.ajax({
        //            url: VIS.Application.contextUrl + "VCreateFrom/GetMattSetIDCommon",
        //            dataType: "json",
        //            type: "POST",
        //            async: false,
        //            data: {
        //                M_Product_ID_Ks: mProductidK,
        //            },
        //            error: function () {
        //                alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
        //                $self.setBusy(false);
        //                return;
        //            },
        //            success: function (dyndata) {
        //                var res = JSON.parse(dyndata);
        //                mattsetid = res;
        //            }
        //        });

        //        if (mattsetid != 0) {
        //            VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1]["M_Product_ID_K"]));
        //        }
        //        else {
        //            return;
        //        }
        //    }
        //});


        this.dGrid.on("click", function (e) {
            e.preventDefault;
            CreateNonEditableRecord();
            // JID_0815 : Removed check to restrict the opening of attribute from in case product is not attribute enabled. Save product in context for every click in grid row.
            //if ($self.dGrid.columns[e.column].field == "M_AttributeSetInstance_ID") {

            // JID_1278: Should not allow to bind Attribute Set Instance to the Product if the Attribute Set is not mapped with the Product.
            VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1]["M_Product_ID_K"]));

            //var AD_Column_ID = 0;
            //var productWindow = AD_Column_ID == 8418;		//	HARDCODED
            //var M_Locator_ID = VIS.context.getContextAsInt($self.windowNo, "M_Locator_ID");
            //var C_BPartner_ID = VIS.context.getContextAsInt($self.windowNo, "C_BPartner_ID");
            //var obj = new VIS.PAttributesForm(VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1].M_AttributeSetInstance_ID_K), VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1].M_Product_ID_K), M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, $self.windowNo);
            //if (obj.hasAttribute) {
            //    obj.showDialog();
            //}
            //obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
            //    $("#grid_" + $self.dGrid.name + "_rec_" + (e.recid)).find("input[type=text]").val(name);
            //    $self.dGrid.records[e.recid - 1].M_AttributeSetInstance_ID_K = mAttributeSetInstanceId;
            //    $self.dGrid.records[e.recid - 1].M_AttributeSetInstance_ID = name;
            //};

            //var mattsetid = VIS.Utility.Util.getValueOfInt(executeScalar("SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + $self.dGrid.records[e.recid - 1]["M_Product_ID_K"]));
            //if (mattsetid != 0) {                    
            //}
            //else {
            //    return;
            //}
            //}
        });


        var executeScalar = function (sql, params, callback) {
            var async = callback ? true : false;
            var dataIn = { sql: sql, page: 1, pageSize: 0 }

            var value = null;

            getDataSetJString(dataIn, async, function (jString) {
                dataSet = new VIS.DB.DataSet().toJson(jString);
                var dataSet = new VIS.DB.DataSet().toJson(jString);
                if (dataSet.getTable(0).getRows().length > 0) {
                    value = dataSet.getTable(0).getRow(0).getCell(0);
                }
                else { value = null; }
                dataSet.dispose();
                dataSet = null;
                if (async) {
                    callback(value);
                }
            });

            return value;
        };

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSet";
        var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";

        function getDataSetJString(data, async, callback) {
            var result = null;
            // data.sql = VIS.secureEngine.encrypt(data.sql);
            $.ajax({
                url: dataSetUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: async,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                if (callback) {
                    callback(json);
                }
                //return result;
            });
            return result;
        };


        //this.dGrid.on("select", function (event) {
        //    if (event.all) {
        //        event.onComplete = function () {
        //            if ($self.dGrid.records.length > 0) {
        //                for (var i = 0; i < $self.dGrid.records.length; i++) {
        //                    $("#grid_" + $self.dGrid.name + "_rec_" + (i + 1)).find("input[type=text]").val(data[i].M_AttributeSetInstance_ID);
        //                    $($("#grid_" + $self.dGrid.name + "_rec_" + (i + 1)).find("input[type=checkbox]")[1]).prop("checked", data[i].Select);
        //                }
        //            }
        //        }
        //    }
        //});

        this.dGrid.on('unselect', function (e) {
            if (e.all) {
                e.onComplete = function () {
                    for (item in $self.dGrid.records) {
                        //if ($self.dGrid.records[item]["M_Product_ID"] == $self.multiValues[itm]) {
                        //    $self.dGrid.select(parseInt(item) + 1);
                        //}
                        var obj = $.grep($self.multiValues, function (n, i) {
                            if (C_Order_ID != null) {
                                return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.C_Order_ID_K == $self.dGrid.records[item]["C_Order_ID_K"]
                            }
                            else if (C_Invoice_ID != null) {
                                return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.C_Invoice_ID_K == $self.dGrid.records[item]["C_Invoice_ID_K"]
                            }
                            else if (M_InOut_ID != null) {
                                return n.M_Product_ID_K == $self.dGrid.records[item]["M_Product_ID_K"] && n.M_InOut_ID_K == $self.dGrid.records[item]["M_InOut_ID_K"]
                            }
                            else {
                                return n.C_Payment_ID_K == $self.dGrid.records[item]["C_Payment_ID_K"]
                            }
                        });
                        if (obj.length > 0) {
                            $self.multiValues.splice($self.multiValues.indexOf(obj[0]), 1);
                        }
                    }
                };
            }
            else {
                var unselectprd = w2ui[e.target].records[e.index]["M_Product_ID_K"];

                //if ($self.multiValues.indexOf(unselectedVal) > -1) {
                //    $self.multiValues.splice($self.multiValues.indexOf(unselectedVal), 1);
                //}
                var obj = $.grep($self.multiValues, function (n, i) {
                    if (C_Order_ID != null) {
                        return n.M_Product_ID_K == unselectprd && n.C_Order_ID_K == w2ui[e.target].records[e.index]["C_Order_ID_K"]
                    }
                    else if (C_Invoice_ID != null) {
                        return n.M_Product_ID_K == unselectprd && n.C_Invoice_ID_K == w2ui[e.target].records[e.index]["C_Invoice_ID_K"]
                    }
                    else if (M_InOut_ID != null) {
                        return n.M_Product_ID_K == unselectprd && n.M_InOut_ID_K == w2ui[e.target].records[e.index]["M_InOut_ID_K"]
                    }
                    else {
                        return n.C_Payment_ID_K == $self.dGrid.records[item]["C_Payment_ID_K"]
                    }
                });
                if (obj.length > 0) {
                    $self.multiValues.splice($self.multiValues.indexOf(obj[0]), 1);
                }
            }
        });
    };

    //Load form into VIS
    VIS.VCreateFrom = VCreateFrom;

})(VIS, jQuery);