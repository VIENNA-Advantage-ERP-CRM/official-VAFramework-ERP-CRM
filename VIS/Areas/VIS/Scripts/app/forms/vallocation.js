; (function (VIS, $) {

    function VAllocation() {
        this.frame = null;
        this.windowNo = 0;
        var ctx = VIS.Env.getCtx();
        var $self = this;
        var $root = $('<div class="vis-allocate-root vis-forms-container">');
        var $row1 = $('<div class="vis-leftpanel-wrapper" style="padding-right:0px">');
        //set height of filters div and Process Button div
        var $innerRow = $('<div style="height: calc(100% - 42px);padding-right:15px; overflow: auto;">');
        var rowContiner = $('<div class="vis-allocation-rightContainer">');
        var $row2 = $('<div class="vis-allocate-paymentdiv" >');
        var $row3 = $('<div class="vis-allocate-cashdiv" >');
        var $row4 = $('<div class="vis-allocate-invoicediv" >');
        var $row5 = $('<div class="vis-allocate-gldiv" style="height:50%" >');
        var $row6 = $('<div >');

        var $vSearchBPartner = null;
        var $cmbCurrency = $('<select class="vis-allocation-currencycmb" ></select>');
        var $cmbOrg = null;
        //Declare variables
        var $cmbDocType = $('<select class="vis-allocation-docTypecmb" ></select>');
        var $txtDocNo, $fromDate, $toDate, fromDate, toDate, docNo, c_docType_ID = null;
        var docType, c_DocType_ID = null;
        //---------------------
        var $divPayment = null;
        var $divCashline = null;
        var $divInvoice = null;
        //added for gl-allocation
        var $divGl = null;
        var $allocationFrom = $('<select class="vis-allocation-currencycmb" ></select>');;
        var $allocationTo = $('<select class="vis-allocation-currencycmb" ></select>');;

        var $gridPayment = null;
        var $gridCashline = null;
        var $gridInvoice = null;

        //Added for Select All Functionality by Manjot assigned by Mukesh sir and Savita
        var $invSelectAll = null;
        var $paymentSelctAll = null;
        var $cashSelctAll = null;
        //added for gl-allocation
        var $glSelectAll = null;
        var maxDate = null;

        var $vchkMultiCurrency = null;
        var $vchkAllocation = null;
        //added for gl-allocation
        var $vchkGlVoucher = null;
        //added for gl-allocation
        var $vchkGlInvoice = null;
        var $divChkGLInvoice = null;

        //Added for inter-business partner work assigned by puneet and mukesh sir
        var $vchkBPAllocation = null;
        var _isInterBPartner = false;
        var $dateAcct = null;
        var $date = null;
        var conversionDate = "";
        var $conversionDate = null;
        var $conversionDiv = null;

        var _C_BPartner_ID = 0;
        var _C_Currency_ID = 0;
        var _payment = 10;
        var _paymentCash = 8;
        var date = null;
        var _open = 7;
        var _openPay = 8;
        var _openInv = 9;
        var _discount = 8;
        var _writeOff = 11;
        var _applied = 12;

        var $lblPaymentSum = null;
        var $lblCashSum = null;
        var $lblInvoiceSum = null;
        //added for gl-allocation
        var $lblglSum = null;

        var _noInvoices = 0;
        var _noPayments = 0;
        var _noCashLines = 0;

        //added for gl-allocation
        var _noGL = 0;
        var glLineGrid;
        var $DivGllineGrid;
        //end

        // array for selected invoices
        var selectedInvoices = [];
        var totalselectedinv = 0;
        var $clrbtn = null;


        var colInvCheck = false;
        var colPayCheck = false;
        var colCashCheck = false;
        //added for gl-allocation
        var colGlCheck = false;

        var $vtxtDifference = null;
        var $vbtnAllocate = null;
        var $vlblAllocCurrency = null;
        var $vchkAutoWriteOff = null;

        var selection = null;
        var $bsyDiv = null;

        var self = this;

        var readOnlyCash = true;
        var readOnlyPayment = false;
        //added for gl-allocation
        var readOnlyGL = true;
        var glTotal = 0;
        //end 

        var paymentTotal = 0;
        var invoiceTotal = 0;

        var countVA009 = 0;
        var stdPrecision = 0;

        /* Variable for Paging*/
        var PAGESIZE = 50;
        var pageNoInvoice = 1, gridPgnoInvoice = 1, invoiceRecord = 0;
        var pageNoCashJournal = 1, gridPgnoCashJounal = 1, CashRecord = 0;
        var pageNoPayment = 1, gridPgnoPayment = 1, paymentRecord = 0;
        var isPaymentGridLoaded = false, isCashGridLoaded = false, isInvoiceGridLoaded = false;
        var C_ConversionType_ID = 0;

        //array to get all the date of selected grid records
        var _allDates = [];

        // is used to compare org of payment / invoice / cash journal with org of selecetd dropdown
        var isOrgMatched = true;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
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

        var elements = [
            "AD_Org_ID",
            "C_BPartner_ID",
            "C_Currency_ID",
            "C_Payment_ID ",
            "C_CashLine_ID",
            "C_Invoice_ID ",
            "Date",
            "DocumentNo",
            "TrxCurrency",
            "C_ConversionType_ID",
            "Amount",
            "ConvertedAmount",
            "OpenAmount",
            "multiplierap",
            "AppliedAmount",
            "C_ConversionType_ID",
            "RECEIPTNO",
            "ccashlineid",
            "ScheduleDate",
            "cinvoiceid",
            "DiscountAmt",
            "DocBaseType",
            "WriteOffAmount",
            "C_InvoicePaySchedule_ID",
            "DateAcct",
            "GL_JOURNALLINE_ID",
            "GL_Journal_ID"
        ];

        VIS.translatedTexts = VIS.Msg.translate(VIS.Env.getCtx(), elements, true);


        //DataSet String
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

        initialize();

        function initialize() {
            _C_Currency_ID = ctx.getContextAsInt("$C_Currency_ID");   //  default
            countVA009 = executeScalar("VIS_131");
            fillLookups();
            loadDocType();
            loadCurrencyPrecision();
            loadOrganization();
            //load allocation combo
            loadAllocationCmb();
            //end
            createRow1();
            createRow2();
            createRow3();
            createRow4();
            //added for gl-allocation
            createRow5();
            rowContiner.append($row2).append($row3).append($row4).append($row5).append($row6);
            //---Set Business Partner Mandatory---Neha
            $vSearchBPartner.getControl().css("background-color", SetMandatory(true));
            //---Resize the Parameter div and Display and hide Payment and Cash div----Neha-----
            $row1.resizable({
                handles: 'e',
                minWidth: 226,
                maxWidth: 600,
                resize: function (event, ui) {
                    var width = ui.size.width;
                    if (width > 197) {
                        rowContiner.width($(document).width() - (width + 40));
                    }
                }
            });
            $row2.css('display', '');
            $row3.css('display', 'none');
            //------------------------------
            $root.append($row1.append($innerRow)).append(rowContiner);
            // to set process button static in design it will not scroll with other filters
            var $divProcess = $('<div style="padding-right: 15px;">');
            $divProcess.append(' <a class="vis-group-btn vis-group-create vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg('Process') + '</a>');
            $row1.append($divProcess);
            $root.append($row1.append($innerRow).append($divProcess)).append(rowContiner);
            $vbtnAllocate = $root.find('.vis-group-btn');
            $vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            createBusyIndicator();
            eventHandling();
            //added for gl-allocation
            $row5.css('display', 'none');
            //end
            $root.find(".vis-allocation-resultdiv").css({ "width": "100%", "margin": "0", "bottom": "0", "position": "inherit" });
            $bsyDiv[0].style.visibility = "hidden";
        };

        function eventHandling() {
            $vSearchBPartner.fireValueChanged = bpValueChanged;
            $cmbCurrency.on("change", function (e) {
                vetoableChange("C_Currency_ID", $cmbCurrency.val());
                loadBPartner();
                //loadCurrencyPrecision(parseInt($cmbCurrency.val()));
                //added for gl-allocation
                loadGLDataGrid(e);
            });
            $vchkMultiCurrency.on("change", function (e) {
                vetoableChange("Date", $vchkMultiCurrency.is(':checked'));
                loadBPartner();
                //added for gl-allocation
                loadGLDataGrid(e);
            });
            $vchkAllocation.on("change", function (e) {
                if ($vchkGlVoucher.is(':checked')) {
                    $(".vis-allocate-invoicediv").insertBefore(".vis-allocate-gldiv");
                    $row2.css('display', 'none'); // Payment Grid
                    $row3.css('display', 'none');// Cash Grid
                    $row5.css('display', 'block');// GL journal grid
                    $row4.css('display', 'block'); // Invoice Grid
                    if ($gridInvoice)
                        $gridInvoice.refresh();
                    if (glLineGrid)
                        glLineGrid.refresh();
                }
                vchkapplocationChange();
            });

            //added for gl-allocation
            $vchkGlVoucher.on("change", function (e) {
                $vchkAllocation.prop('checked', false);
                if ($vchkGlVoucher.is(':checked')) {
                    $(".vis-allocate-invoicediv").insertBefore(".vis-allocate-gldiv");
                    $row4.css('display', 'block');// Invoice Grid
                    $row5.css('display', 'block');// GL journal grid
                    $row2.css('display', 'none'); // Payment Grid
                    $row3.css('display', 'none');// Cash Grid
                    if ($gridInvoice)
                        $gridInvoice.refresh();
                    if (glLineGrid)
                        glLineGrid.refresh();
                }
                else {
                    $(".vis-allocate-invoicediv").insertBefore(".vis-allocate-paymentdiv");
                    $row4.css('display', 'block');// Invoice Grid
                    $row5.css('display', 'none');// GL journal grid
                    $row2.css('display', 'block'); // Payment Grid
                    $row3.css('display', 'none');// Cash Grid
                    if ($gridInvoice)
                        $gridInvoice.refresh();
                    if ($gridPayment)
                        $gridPayment.refresh();
                }
                loadGLDataGrid(e);
            });
            $vchkGlInvoice.on("change", function (e) {
                //$vchkAllocation.prop('checked', false);
                //if ($vchkGlInvoice.is(':checked')) {
                //    $(".vis-allocate-invoicediv").insertBefore(".vis-allocate-paymentdiv");
                //    $row2.css('display', 'none'); // Payment Grid
                //    $row3.css('display', 'none');// Cash Grid
                //    $row4.css('display', 'block');// Invoice Grid
                //    if ($gridInvoice)
                //        $gridInvoice.refresh();
                //}
                //else {
                //    $(".vis-allocate-invoicediv").insertBefore(".vis-allocate-gldiv");
                //    $row2.css('display', 'none');// Payment Grid
                //    $row5.css('display', 'block');// GL journal grid
                //    $row3.css('display', 'none'); // Cash Grid
                //    $row4.css('display', 'block'); // Invoice Grid
                //    if ($gridInvoice)
                //        $gridInvoice.refresh();
                //    if (glLineGrid)
                //        glLineGrid.refresh();
                //}
                //$glInvoiceMaindiv
            });
            //end

            $date.on("change", function (e) {
                /** Commented because we need to stop the load business partner on selection of date
                //vetoableChange("Date", $date.val());
                //loadBPartner();
                */
                if (getSelectedRecordsCount()) {
                    if (new Date($date.val()) < maxDate) {
                        VIS.ADialog.info("", true, VIS.Msg.getMsg("SelectMaxDate") + Globalize.format(new Date(maxDate), "yyyy-MM-dd"), "");
                        $date.val(Globalize.format(new Date(maxDate), "yyyy-MM-dd"));
                        return false;
                    }
                }
                else {
                    VIS.ADialog.info("", true, VIS.Msg.getMsg("PleaseSelectRecord"), "");
                }
            });
            $vbtnAllocate.on("click", function (e) {
                if (getSelectedRecordsCount()) {
                    if ($date.val().trim() == "") {
                        VIS.ADialog.info("", true, VIS.Msg.getMsg("InvalidDateFormat"), "");
                        return false;
                    }
                    if (new Date($date.val()) < maxDate) {
                        VIS.ADialog.info("", true, VIS.Msg.getMsg("SelectMaxDate") + Globalize.format(new Date(maxDate), "yyyy-MM-dd"), "");
                        $date.val(Globalize.format(new Date(maxDate), "yyyy-MM-dd"));
                        return false;
                    }

                    if (isOrgMatched) {
                        //Org Manadatory because system will create view allocation in the selected org.
                        if (parseInt($cmbOrg.val()) > 0)
                            allocate();
                        else
                            VIS.ADialog.info("", true, VIS.Msg.getMsg("ValidateOrg"), "");
                    }
                    else {
                        /*Selected records are having a different organization than parameter organization. Do you want to proceed?*/
                        VIS.ADialog.confirm("ConfirmForDiffOrg", true, "", "Confirm", function (result) {
                            if (result) {
                                //Org Manadatory because system will create view allocation in the selected org.
                                if (parseInt($cmbOrg.val()) > 0)
                                    allocate();
                                else
                                    VIS.ADialog.info("", true, VIS.Msg.getMsg("ValidateOrg"), "");
                            }
                            else {
                                return;
                            }
                        });
                    }
                }
                else {
                    VIS.ADialog.info("", true, VIS.Msg.getMsg("PleaseSelectRecord"), "");
                }
            });
            $vchkAutoWriteOff.on("click", function (e) {
                autoWriteOff();
                calculate();
            });
            //-----Controls events and load Invoice Grid--Neha
            $txtDocNo.on("change", function (e) {
                loadInvoice();
            });
            $cmbDocType.on("change", function (e) {
                loadInvoice();
            });
            $fromDate.on("change", function (e) {
                loadInvoice();
            });
            $toDate.on("change", function (e) {
                loadInvoice();
            });
            //-----For Select All buttons on grid by Manjot assigned by Mukesh sir and Savita
            $invSelectAll.on("change", function (e) {
                var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', $invSelectAll.prop("checked"));
                    //$(chk[i]).change(e);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }

                // Clear Selected invoices array when we de-select the select all checkbox. work done for to hold all the selected invoices
                if ($invSelectAll.prop("checked") == false)
                    selectedInvoices = [];
            });
            $paymentSelctAll.on("change", function (e) {
                var chk = $('#grid_' + $gridPayment.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', $paymentSelctAll.prop("checked"));
                    //$(chk[i]).change(e);
                    $gridPayment.editChange.call($gridPayment, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridPayment.trigger(eData);
                }

            });
            $cashSelctAll.on("change", function (e) {
                var chk = $('#grid_' + $gridCashline.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', $cashSelctAll.prop("checked"));
                    //$(chk[i]).change(e);
                    $gridCashline.editChange.call($gridCashline, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridCashline.trigger(eData);
                }

            });

            //added for gl-allocation
            $glSelectAll.on("change", function (e) {
                var chk = $('#grid_' + glLineGrid.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', $glSelectAll.prop("checked"));
                    //$(chk[i]).change(e);
                    glLineGrid.editChange.call(glLineGrid, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i + 1, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    glLineGrid.trigger(eData);
                }
            });
            //end 

            $cmbOrg.on("change", function (e) {
                if (parseInt($cmbOrg.val()) > 0)
                    $cmbOrg.css("background-color", SetMandatory(false));
                else
                    $cmbOrg.css("background-color", SetMandatory(true));

            });
            $conversionDate.on("change", function (e) {
                conversionDate = $conversionDate.val();
                loadBPartner();
            });
            //---For inter-business Partner data load in all grids
            $vchkBPAllocation.on("change", function (e) {
                vetoableChange("Date", $vchkBPAllocation.is(':checked'));
                _isInterBPartner = $vchkBPAllocation.is(':checked');
                loadBPartner();
            });
            //clear selected invoices invoices 
            $clrbtn.on("click", function (e) {

                var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', false);
                    //$(chk[i]).change(e);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }
                selectedInvoices = [];
                loadInvoice();
            });

            //allocation combo event
            $allocationFrom.on("change", function (e) {

                if ($allocationFrom.val() == 0) {
                    $allocationFrom.css("background-color", SetMandatory(true));
                }
                else
                    $allocationFrom.css("background-color", SetMandatory(false));

                if ($allocationFrom.val() == "P") { // In case of Payment Cash Option must be Hide
                    $allocationTo.find("option[value=C]").hide();
                    $allocationTo.find("option[value=P]").show();
                    $allocationTo.find("option[value=G]").show();
                }
                else if ($allocationFrom.val() == "C") { // In case of Cash Payment Option must be Hide
                    $allocationTo.find("option[value=P]").hide();
                    $allocationTo.find("option[value=C]").show();
                    $allocationTo.find("option[value=G]").show();
                }
                else if ($allocationFrom.val() == "G") { // In case of GL Journal GL Journal Option must be Hide
                    $allocationTo.find("option[value=P]").show();
                    $allocationTo.find("option[value=C]").show();
                    $allocationTo.find("option[value=G]").hide();
                }
                else { //we need to show all option
                    $allocationTo.find("option[value=C]").show();
                    $allocationTo.find("option[value=P]").show();
                    $allocationTo.find("option[value=G]").show();
                }
                $allocationTo.val(0);
                $allocationTo.css("background-color", SetMandatory(true));
                loadGrids($allocationFrom.val());
                displayGrids($allocationFrom.val(), $allocationTo.val())
            });

            $allocationTo.on("change", function (e) {
                if ($allocationTo.val() == 0) {
                    $allocationTo.css("background-color", SetMandatory(true));
                }
                else
                    $allocationTo.css("background-color", SetMandatory(false));

                var allocfrm = $allocationFrom.val();
                var allocto = $allocationTo.val();
                loadGrids($allocationTo.val());
                displayGrids(allocfrm, allocto)
            });
        };

        //added for gl-allocation
        function loadGLGrid() {
            glLineGrid = null;
            glLineGrid = $divGl.w2grid({
                name: "INT15_GLLineGrid" + $self.windowNo,
                show: { toolbar: true },
                multiSelect: true,
                columns: [
                    { field: "SelectRow", caption: 'check', size: '40px', editable: { type: 'checkbox' } },
                    { field: "DATEDOC", caption: VIS.translatedTexts.Date, size: '80px', hidden: false },
                    { field: "DOCUMENTNO", caption: VIS.translatedTexts.RECEIPTNO, size: '120px', hidden: false },
                    { field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false },
                    { field: "ConvertedAmount", caption: VIS.Msg.getMsg("ConvertedAmount"), size: '150px', hidden: false },
                    { field: "OpenAmount", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false },
                    { field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false },
                    { field: "GL_JOURNALLINE_ID", caption: VIS.translatedTexts.GL_JOURNALLINE_ID, size: '150px', hidden: true },
                    { field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true },
                    { field: "GL_Journal_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true },
                    { field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false }
                ],
                onClick: function (event) {
                    //paymentCellClicked(event);
                    //getMaxDate();
                    glTableChanged(event.recid, event.column);
                    //alert("onClick");
                },
                onChange: function (event) {
                    // glCellChanged(event);
                    glCellChanged(event);
                    //alert("onChange");
                },
                onDblClick: function (event) {
                    glDoubleClicked(event);
                },
                onSelect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            //alert("eventall");
                            //$divPayment.find('#grid_openformatgrid_records').on('scroll', cartPaymentScroll);
                        }
                    }
                    //else
                    //  alert("eventSingle");
                }
            });
        };
        function loadGLDataGrid(e) {

            if (!glLineGrid) {
                loadGLGrid();
                $($(glLineGrid.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
                $($(glLineGrid.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
                $('.vis-allocate-glSum').css('text-align', 'right');
                $('.vis-allocate-glSum').css('margin-right', '10px');
            }
            if (!$gridInvoice) {
                loadInvoice();
            }

            if ($vchkGlVoucher.is(':checked')) {
                //$row4.css('display', 'none');
                //$row5.css('display', 'block');
                //$divChkGLInvoice.css('display', 'block');
                $bsyDiv[0].style.visibility = "visible";
                readOnlyGL = false;
                //Set False to select all 
                $glSelectAll.prop('checked', false);
                $invSelectAll.prop('checked', false);
                glLineGrid.clear();
                loadGLVoucher();
            }
            else {
                $vchkAllocation.prop('checked', false);
                $vchkGlInvoice.prop('checked', false);
                glLineGrid.clear();
                //$row4.css('display', 'block');
                //$row5.css('display', 'none');
                $divChkGLInvoice.css('display', 'none');
                readOnlyGL = true;
                $glSelectAll.prop('checked', false);
                $invSelectAll.prop('checked', false);
                vchkapplocationChange();
                if ($gridInvoice) {
                    $gridInvoice.refresh();
                }
            }

            if ($gridInvoice) {
                var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', $invSelectAll.prop("checked"));
                    //$(chk[i]).change(e);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }
            }
            displayGrids($allocationFrom.val(), $allocationTo.val());
        };
        function loadGrids(value) {
            if (value == "P") {
                if ($gridPayment) {
                    $gridPayment.refresh();
                }
            }
            else if (value == "C") {
                if ($gridCashline) {
                    $gridCashline.refresh();
                }
            }
            else if (value == "I") {
                if ($gridInvoice) {
                    $gridInvoice.refresh();
                }
            }
            else if (value == "G") {
                if (glLineGrid) {
                    glLineGrid.refresh();
                }
            }
        };
        function displayGrids(allocFrm, allocTo) {
            $row2.css('display', 'none'); // Payment Grid
            $row3.css('display', 'none');// Cash Grid
            $row4.css('display', 'none'); // Invoice Grid
            $row5.css('display', 'none');// GL journal grid
            readOnlyCash = true;
            readOnlyPayment = true;
            readOnlyGL = true;

            if (allocFrm == "P") {
                readOnlyPayment = false;
                $row2.css('display', 'block'); // Payment Grid
            }
            else if (allocFrm == "C") {
                readOnlyCash = false;
                $row3.css('display', 'block');// Cash Grid
            }
            else if (allocFrm == "I") {
                $row4.css('display', 'block'); // Invoice Grid
            }
            else if (allocFrm == "G") {
                $row5.css('display', 'block');// GL journal grid
                $bsyDiv[0].style.visibility = "visible";
                readOnlyGL = false;
                glLineGrid.clear();
                loadGLVoucher();
            }

            if (allocTo == "P") {
                readOnlyPayment = false;
                $row2.css('display', 'block'); // Payment Grid
            }
            else if (allocTo == "C") {
                readOnlyCash = false;
                $row3.css('display', 'block');// Cash Grid
            }
            else if (allocTo == "I") {
                $row4.css('display', 'block'); // Invoice Grid
            }
            else if (allocTo == "G") {
                $row5.css('display', 'block');// GL journal grid
                $bsyDiv[0].style.visibility = "visible";
                readOnlyGL = false;
                glLineGrid.clear();
                loadGLVoucher();
            }

            //$(".vis-allocate-invoicediv").insertBefore(".vis-allocate-gldiv");
            //$(".vis-allocate-invoicediv").insertBefore(".vis-allocate-paymentdiv");
        };
        //end

        function fillLookups() {

            var data = null;
            var value = VIS.MLookupFactory.getMLookUp(ctx, self.windowNo, 3505, VIS.DisplayType.TableDir);
            data = value.getData(true, true, false, false);
            if (data != null && data != undefined && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $cmbCurrency.append('<option value=' + data[i].Key + '>' + data[i].Name + '</option>');
                    if (data[i].Key == _C_Currency_ID) {
                        $cmbCurrency.val(_C_Currency_ID);
                    }
                }
            }

            value = VIS.MLookupFactory.getMLookUp(ctx, self.windowNo, 3499, VIS.DisplayType.Search);
            $vSearchBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, value);
        };

        // Set Mandatory and non mandatory---Neha
        function SetMandatory(Value) {

            if (Value)
                return '#FFB6C1';
            else
                return 'White';
        };

        //-------Load Document Type Grid-----------Neha------------
        function loadDocType() {
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "PaymentAllocation/GetDocType",
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        $cmbDocType.empty();
                        $cmbDocType.append("<option value= 0 >Select</option>");
                        for (var i = 0; i < result.length; i++) {
                            docType = VIS.Utility.Util.getValueOfString(result[i].DocType);
                            c_DocType_ID = VIS.Utility.Util.getValueOfInt(result[i].C_DocType_ID);
                            $cmbDocType.append(" <option value=" + c_DocType_ID + ">" + docType + "</option>");
                        }
                    }
                    else {
                        $cmbDocType.append("<option value= 0 >Select</option>");
                    }
                    $cmbDocType.prop('selectedIndex', 0);
                },
                error: function (er) {
                    console.log(er);
                }
            });
        };

        //-----Load Invoice Grid----Neha
        function loadInvoice() {
            if (_C_BPartner_ID > 0) {
                fromDate = $fromDate.val();
                toDate = $toDate.val();
                //---If ToDate is less than From Date--Display error message-----Neha
                if (toDate != "") {
                    if (fromDate > toDate) {
                        $toDate.val('');
                        return VIS.ADialog.info("Message", true, VIS.Msg.getMsg("VIS_FromDateGreater"), "");
                    }
                }
                $bsyDiv[0].style.visibility = "visible";
                pageNoInvoice = 1, gridPgnoInvoice = 1, invoiceRecord = 0;
                isInvoiceGridLoaded = false;
                C_ConversionType_ID = 0;
                var chk = $vchkMultiCurrency.is(':checked');
                var getDate = null;
                date = $date.val();
                if (date != null && date != undefined && date != "") {
                    getDate = VIS.DB.to_date(date);
                }
                else
                    getDate = VIS.DB.to_date(new Date());
                //---Get values and pass values to the controller----Neha
                docNo = $txtDocNo.val();
                c_docType_ID = $cmbDocType.val();
                if (fromDate != null && fromDate != undefined && fromDate != "") {
                    VIS.DB.to_date(fromDate);
                }
                if (toDate != null && toDate != undefined && toDate != "") {
                    VIS.DB.to_date(toDate);
                }

                $.ajax({
                    url: VIS.Application.contextUrl + "PaymentAllocation/GetInvoice",
                    data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, date: getDate, page: pageNoInvoice, size: PAGESIZE, docNo: docNo, c_docType_ID: c_docType_ID, fromDate: fromDate, toDate: toDate, conversionDate: conversionDate },
                    async: true,
                    success: function (result) {
                        var data = JSON.parse(result);
                        if (data) {
                            if (pageNoInvoice == 1 && data.length > 0) {
                                invoiceRecord = data[0].InvoiceRecord;
                                gridPgnoInvoice = Math.ceil(invoiceRecord / PAGESIZE);
                            }

                            //if any invoice is selected and we load another invoice from db than we have to check weather that invoice is already selected or not. if already selected tha skip that.
                            if (selectedInvoices.length > 0) {
                                totalselectedinv = selectedInvoices.length;
                                for (var i = 0; i < data.length; i++) {

                                    var filterObj = selectedInvoices.filter(function (e) {
                                        return e.C_InvoicePaySchedule_ID == data[i]["C_InvoicePaySchedule_ID"];
                                    });

                                    if (filterObj.length == 0) {
                                        data[i]["recid"] = selectedInvoices.length + i;
                                        selectedInvoices.push(data[i]);
                                    }
                                }
                                data = selectedInvoices;
                                // Clear Selected invoices array when we de-select the select all checkbox. work done for to hold all the selected invoices
                                selectedInvoices = [];
                            }
                            //end

                            bindInvoiceGrid(data, chk);
                            //Amount
                            _openInv = 9;
                            // _discount = chk ? 7 : 5;
                            _discount = 10;
                            _writeOff = 13;
                            _applied = 14;
                            //added window no to find grid to implement scroll  issue : scroll was not working properly
                            $divInvoice.find('#grid_openformatgridinvoice_' + $self.windowNo + '_records').on('scroll', cartInvoiceScroll);
                            isInvoiceGridLoaded = true;
                            isBusyIndicatorHidden();
                        }
                    },
                    error: function (err) {
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });
            }
        };

        //----Laod Comobo Allocation From and Allocation To
        function loadAllocationCmb() {
            $allocationFrom.empty();
            $allocationTo.empty();
            $allocationFrom.append("<option value=0></option>");
            $allocationTo.append("<option value=0></option>");
            $allocationFrom.append("<option value='P'>Payment</option>");
            $allocationTo.append("<option value='P'>Payment</option>");
            $allocationFrom.append("<option value='C'>Cash</option>");
            $allocationTo.append("<option value='C'>Cash</option>");
            $allocationFrom.append("<option value='I'>Invoice</option>");
            $allocationTo.append("<option value='I'>Invoice</option>");
            $allocationFrom.append("<option value='G'>GL Journal</option>");
            $allocationTo.append("<option value='G'>GL Journal</option>");
        };

        function createRow1() {
            var $divBp = $('<div class="vis-allocation-leftControls">');
            $divBp.append('<span class="vis-allocation-inputLabels">' + VIS.translatedTexts.C_BPartner_ID + '</span>').append($vSearchBPartner.getControl().addClass("vis-allocation-bpartner")).append($vSearchBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            var $divCu = $('<div class="vis-allocation-leftControls">');
            $divCu.append('<span class="vis-allocation-inputLabels">' + VIS.translatedTexts.C_Currency_ID + '</span>').append($cmbCurrency);
            $innerRow.append($divBp).append($divCu);
            //added for enhancement of new combo regarding allocation from and to
            var $divallocFrom = $('<div class="vis-allocation-leftControls">');
            $divallocFrom.append('<span class="vis-allocation-inputLabels"> ' + "Allocation From" /* ' + VIS.translatedTexts.AllocationFrom + ' */ + '</span>').append($allocationFrom);
            var $divallocTo = $('<div class="vis-allocation-leftControls">');
            $divallocTo.append('<span class="vis-allocation-inputLabels"> ' + "Allocation To"  /* ' + VIS.translatedTexts.AllocationTo + */ + '</span>').append($allocationTo);
            $innerRow.append($divallocFrom).append($divallocTo);
            //end

            var $rowOne = $('<div class="vis-allocation-leftControls">'
                + '<input name="vchkMultiCurrency" class="vis-allocation-multicheckbox" type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("MultiCurrency") + '</label>'
                + '</div>'
                + '<div class="vis-allocation-leftControls" id="cashMaindiv"  style="display: none !important;">'
                + '<input  class="vis-allocation-cashbox"  type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("Cash") + '</label>'
                + '</div>'
                //Added for gl-allocation
                + '<div class="vis-allocation-leftControls" id="glMaindiv" style="display: none !important;">'
                + '<input  class="vis-allocation-glvoucher"  type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("GL Voucher") + '</label>'
                + '</div>'

                + '<div class="vis-allocation-leftControls vis-allocation-glinvoiceDiv" style="display: none !important;">'
                + '<input  class="vis-allocation-glinvoice"  type="checkbox">'
                + '<label>' + VIS.translatedTexts.C_Invoice_ID + '</label>'
                + '</div>'

                //end
                + '<div class="vis-allocation-leftControls">'
                + '<input  class="vis-allocation-autowriteoff"  type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("AutoWriteOff") + '</label>'
                + '</div>'
                + '<div class="vis-allocation-leftControls">'
                + '<input id=VIS_chkbxBPAllocation_' + $self.windowNo + ' class="vis-allocation-interBP" style="display:none !important;" type="checkbox">'
                + '<label style="display:none !important;">' + VIS.Msg.getMsg("BPAllocation") + '</label>'
                + '</div>'
                //---------------------------Added new parameters----Neha----3 August 2018---Asked by Amit
                + '<div class="vis-allocation-leftControls">'
                + '<div class="panel-group" style="margin: 0px;" id="accordion" role="tablist" aria-multiselectable="true">'
                + '<div class="panel panel-default">'
                + '<div class="panel-heading" role="tab" id="headingOne">'
                + '<h4 class="panel-title">'
                + '<a role="button" data-toggle="collapse" data-parent="#accordion" href="#collapseOne" aria-expanded="true" aria-controls="collapseOne" class="VIS-Accordion-head collapsed">' + VIS.Msg.getMsg("InvoiceFilter")
                + '<i class="glyphicon glyphicon-chevron-down pull-right"></i>'
                + '</a>'
                + '</h4>'
                + '</div>'
                + '<div id="collapseOne" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingOne" style="height: 0px;">'
                + '<div class="panel-body" style=" overflow: auto; height: 140px !important; ">'
                + '<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels">' + VIS.Utility.encodeText(VIS.Msg.getMsg("Document_No")) + '</span>'
                + ' <input class="vis-allocation-docNo" type="textbox"></input>'
                + '</div>'
                + '<div class="vis-allocation-leftControls vis-allocation-cmbdoctype">'
                + '<span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("DocType") + '</span>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>');
            $innerRow.append($rowOne);
            $rowOne.find(".vis-allocation-cmbdoctype").append($cmbDocType);
            $rowOne.find(".panel-body").append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("VIS_FromDate") + '</span>'
                + '<input  class="vis-allocation-fromDate"  type="date"></input>'
                + '</div>'
                + '<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("VIS_ToDate") + '</span>'
                + '<input  class="vis-allocation-toDate"  type="date"></input>'
                + '</div>');
            $divBp.find(".vis-allocation-bpartner").css('width', 'calc(100% - 30px)');
            //-----------Neha-----------------
            $vchkMultiCurrency = $innerRow.find('.vis-allocation-multicheckbox');
            $vchkAllocation = $innerRow.find('.vis-allocation-cashbox');
            //Added for gl-allocation
            $vchkGlVoucher = $innerRow.find('.vis-allocation-glvoucher');
            $vchkGlInvoice = $innerRow.find('.vis-allocation-glinvoice');
            $divChkGLInvoice = $innerRow.find('.vis-allocation-glinvoiceDiv');
            //end
            $vchkAutoWriteOff = $innerRow.find('.vis-allocation-autowriteoff');

            //-----get controls values------------------
            $txtDocNo = $innerRow.find('.vis-allocation-docNo');
            $fromDate = $innerRow.find('.vis-allocation-fromDate');
            $toDate = $innerRow.find('.vis-allocation-toDate');
            //------------------------------------------
            var $resultDiv = $('<div class="vis-allocation-resultdiv">');
            $resultDiv.append('<div class="vis-allocation-leftControls">' +
                '<span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("Difference")
                + '</span>');
            $resultDiv.append('<div style="width:20% !important" class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-lblCurrnecy"></span>'
                + '<span class="vis-allocation-lbldifferenceAmt" style="float:right;">'
                + '</span>');
            //$resultDiv.append('<div style="width:80% !important"  class="vis-allocation-leftControls">'
            //    + '<span class="vis-allocation-lbldifferenceAmt">'
            //    + '</span>');
            $resultDiv.append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("TransactionDate") + '</span>'
                + '<input  class="vis-allocation-date" disabled  id=VIS_cmbDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');
            $resultDiv.append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("DateAcct") + '</span>'
                + '<input  class="vis-allocation-date" disabled id=VIS_cmbAcctDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');
            $resultDiv.append('<div class="vis-allocation-leftControls"> <span class="vis-allocation-inputLabels" title="View allocation will be created in this organization" >' + VIS.translatedTexts.AD_Org_ID + '</span> <select class="vis-allocation-currencycmb" id=VIS_cmbOrg_' + $self.windowNo + '></select>');

            $resultDiv.append('<div class="vis-allocation-leftControls" id=VIS_cnvrDateDiv_' + $self.windowNo + '>'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("ConversionDate") + '</span>'
                + '<input  class="vis-allocation-date" style="display:block;" id=VIS_cmbConversionDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');

            //$resultDiv.append('<div class="vis-allocation-leftControls"><input  class="vis-allocation-autowriteoff"  type="checkbox"><label>' + VIS.Msg.getMsg("AutoWriteOff") + '</label></div>');
            //Hide the privious Process Button
            // $resultDiv.append(' <a class="vis-group-btn vis-group-create vis-group-grayBtn" style="float: right; display: none; ">' + VIS.Msg.getMsg('Process') + '</a>');

            $innerRow.append($resultDiv);
            $date = $innerRow.find('#VIS_cmbDate_' + $self.windowNo);
            $date.val(Globalize.format(new Date(), "yyyy-MM-dd"));
            $dateAcct = $innerRow.find('#VIS_cmbAcctDate_' + $self.windowNo);
            $dateAcct.val(Globalize.format(new Date(), "yyyy-MM-dd"));
            $conversionDate = $innerRow.find('#VIS_cmbConversionDate_' + $self.windowNo);
            $conversionDiv = $innerRow.find('#VIS_cnvrDateDiv_' + $self.windowNo);
            $conversionDiv.css('display', 'none');
            //$conversionDate.val(Globalize.format(new Date(), "yyyy-MM-dd"));
            $cmbOrg = $innerRow.find('#VIS_cmbOrg_' + $self.windowNo);
            $vchkBPAllocation = $innerRow.find('#VIS_chkbxBPAllocation_' + $self.windowNo);
            $vtxtDifference = $resultDiv.find('.vis-allocation-lbldifferenceAmt');
            //$vbtnAllocate = $resultDiv.find('.vis-group-btn');
            //$vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            $vlblAllocCurrency = $resultDiv.find('.vis-allocation-lblCurrnecy');
        };

        function createRow2() {
            $row2.append('<div style=" width: 100%; float: left; "><p style="float:left;">' + VIS.translatedTexts.C_Payment_ID + '</p>  <input style="float:left;" type="checkbox" id="paymentselectall" /><p class="vis-allocate-paymentSum">' + VIS.Msg.getMsg("SelectedPayments") + '0-Sum 0.00</p></div>').append('<div class="vis-allocation-payment-grid"></div>');//.append('<p class="vis-allocate-paymentSum">0-Sum 0.00</p>');
            $divPayment = $row2.find('.vis-allocation-payment-grid');
            $lblPaymentSum = $row2.find('.vis-allocate-paymentSum');
            $paymentSelctAll = $row2.find('#paymentselectall');
        };

        function createRow3() {
            $row3.append('<div style=" width: 100%; float: left; "><p style="float:left;">' + VIS.translatedTexts.C_CashLine_ID + '</p> <input style="float:left;" type="checkbox" id="cashselectall" /><p class="vis-allocate-cashSum">' + VIS.Msg.getMsg("SelectedCashlines") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-cashLine-grid"></div>');//.append('<p class="vis-allocate-cashSum">0-Sum 0.00</p>');
            $divCashline = $row3.find('.vis-allocation-cashLine-grid');
            $lblCashSum = $row3.find('.vis-allocate-cashSum');
            $cashSelctAll = $row3.find('#cashselectall');
        };

        function createRow4() {
            //
            $row4.append('<div style=" width: 100%; float: left; "><p style="float:left;">' + VIS.translatedTexts.C_Invoice_ID + '</p> <input style="float:left;" type="checkbox" id="invoiceselectall" /><p style="float:left;margin-left: 10px;"> ' + VIS.Msg.getMsg("Reset") + ' </p> <span id="clrbutton_' + $self.windowNo + '" style="float:left;cursor: pointer !important;margin-left: 5px;margin-top: 2px;" class="glyphicon glyphicon-refresh"></span><p class="vis-allocate-invoiceSum">' + VIS.Msg.getMsg("SelectedInvoices") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-invoice-grid"></div>');//.append('<p class="vis-allocate-invoiceSum">0-Sum 0.00</p>');
            $divInvoice = $row4.find('.vis-allocation-invoice-grid');
            $lblInvoiceSum = $row4.find('.vis-allocate-invoiceSum');
            $invSelectAll = $row4.find('#invoiceselectall');
            //get control of clear button
            $clrbtn = $row4.find('#clrbutton_' + $self.windowNo);
        };

        //added for gl-allocation
        function createRow5() {
            $row5.append('<div style=" width: 100%; float: left; "><p style="float:left;">' + VIS.translatedTexts.GL_Journal_ID + '</p> <input style="float:left;" type="checkbox" id="glselectall" /><p class="vis-allocate-glSum">' + VIS.Msg.getMsg("SelectedGL") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-gl-grid" style="height:400px;"></div>');
            $divGl = $row5.find('.vis-allocation-gl-grid');
            $lblglSum = $row5.find('.vis-allocate-glSum');
            $glSelectAll = $row5.find('#glselectall');
        };
        //end

        /**
        *To get all the  Organization which are accessable by login user
        *@alias    loadCurrency
        *@return {list} list of Organization Name and ID.
        */
        function loadOrganization() {
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VIS/PaymentAllocation/GetOrganization",
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        $cmbOrg.empty();
                        $cmbOrg.css("background-color", SetMandatory(true));
                        $cmbOrg.append("<option value=0></option>");
                        for (var i = 0; i < result.length; i++) {
                            OrgName = VIS.Utility.Util.getValueOfString(result[i].Name);
                            AD_Org_ID = VIS.Utility.Util.getValueOfInt(result[i].Value);
                            $cmbOrg.append(" <option value=" + AD_Org_ID + ">" + OrgName + "</option>");
                        }
                    }
                    else {
                        $cmbOrg.append("<option value=0></option>");
                    }
                    if (VIS.context.getContextAsInt("#AD_Org_ID") > 0) {
                        $cmbOrg.val(VIS.context.getContextAsInt("#AD_Org_ID"));
                        $cmbOrg.css("background-color", SetMandatory(false));
                    }
                    else
                        $cmbOrg.prop('selectedIndex', 0);
                },
                error: function (er) {
                    console.log(er);
                }
            });
        };

        /*
        Create busyIndicator
        */
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        };

        /// <summary>
        ///  Load Business Partner Info
        ///  - Payments
        ///  - Invoices
        /// </summary>
        function loadBPartner() {
            if (_C_BPartner_ID == 0 || _C_Currency_ID == 0) {
                //  SetBusy(false);
                return;
            }
            // If BP is selected then  set mandatory false---Neha
            $vSearchBPartner.getControl().css("background-color", SetMandatory(false));
            $bsyDiv[0].style.visibility = "visible";
            // initialize Pageno as 1 when we change BP
            pageNoInvoice = 1, gridPgnoInvoice = 1, invoiceRecord = 0;
            pageNoCashJournal = 1, gridPgnoCashJounal = 1, CashRecord = 0;
            pageNoPayment = 1, gridPgnoPayment = 1, paymentRecord = 0;
            isPaymentGridLoaded = false, isCashGridLoaded = false, isInvoiceGridLoaded = false;
            C_ConversionType_ID = 0;
            var getDate = null;
            date = $date.val();
            if (date != null && date != undefined && date != "") {
                getDate = VIS.DB.to_date(date);
            }
            else
                getDate = VIS.DB.to_date(new Date());

            var chk = $vchkMultiCurrency.is(':checked');
            //vdgvPayment.ItemsSource = null;
            //vdgvCashLine.ItemsSource = null;
            //vdgvInvoice.ItemsSource = null;
            //SetBusy(true);
            //txtBusy.Text = Msg.GetMsg("Processing");

            var AD_Role_ID = ctx.getAD_Role_ID();
            var AD_User_ID = ctx.getAD_User_ID();
            var role = VIS.MRole.getDefault();

            //  log.Config("BPartner=" + _C_BPartner_ID + ", Cur=" + _C_Currency_ID);
            //  Need to have both values


            //	Async BPartner Test
            var key = _C_BPartner_ID;
            /********************************
         *  Load unallocated Payments
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            //            var sql = "SELECT 'false' as SELECTROW, p.DateTrx as DATE1,p.DocumentNo As DOCUMENTNO,p.C_Payment_ID As CPAYMENTID,"  //  1..3
            //               + "c.ISO_Code as ISOCODE,p.PayAmt AS PAYMENT,"                            //  4..5
            //               + "currencyConvert(p.PayAmt ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1
            //               + "currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) as OPENAMT,"  //  7   #2
            //               + "p.MultiplierAP as MULTIPLIERAP, "
            //               + "0 as APPLIEDAMT "
            //               + "FROM C_Payment_v p"		//	Corrected for AP/AR
            //               + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
            //               + "WHERE "
            //          + "  ((p.IsAllocated ='N' and p.c_charge_id is null) "
            //+ " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
            //            + " AND p.Processed='Y'"
            //               + " AND p.C_BPartner_ID=" + _C_BPartner_ID;
            //            if (!chk) {
            //                sql += " AND p.C_Currency_ID=" + _C_Currency_ID;				//      #4
            //            }
            //            sql += " ORDER BY p.DateTrx,p.DocumentNo";

            //            sql = role.addAccessSQL(sql, "p", true, false);
            //            var dsPayment = VIS.DB.executeDataSet(sql);
            //            _payment = 9;
            //            bindPaymentGrid(dsPayment, chk);


            //_payment = 9;

            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetPayments",
                data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, page: pageNoPayment, size: PAGESIZE },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {
                        if (pageNoPayment == 1 && data.length > 0) {
                            paymentRecord = data[0].PaymentRecord;
                            gridPgnoPayment = Math.ceil(paymentRecord / PAGESIZE);
                        }
                        bindPaymentGrid(data, chk);
                        //added window no to find grid to implement scroll  issue : scroll was not working properly
                        $divPayment.find('#grid_openformatgrid_' + $self.windowNo + '_records').on('scroll', cartPaymentScroll);
                        isPaymentGridLoaded = true;
                        isBusyIndicatorHidden();
                    }
                },
                error: function (err) {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });

            //$divPayment.find('#grid_openformatgrid_records').on('scroll', cartPaymentScroll);

            /********************************
         *  Load unallocated Cash lines
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            //          var sqlCash = "SELECT 'false' as SELECTROW, cn.created as CREATED, cn.receiptno AS RECEIPTNO, cn.c_cashline_id AS CCASHLINEID,"
            //          + "c.ISO_Code AS ISO_CODE,cn.amount AS AMOUNT, "
            //           + "currencyConvert(cn.Amount ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.Created ,114 ,cn.AD_Client_ID ,cn.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1cn.amount as OPENAMT,"
            //          + " cn.amount as OPENAMT,"
            //+ "cn.MultiplierAP AS MULTIPLIERAP,"
            //+ "0 as APPLIEDAMT "
            // + " from c_cashline_new cn"
            //              //+ " from C_CASHLINE_VW cn"
            //          + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID)"
            //          + " WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
            //          + " and cn.cashtype = 'B' and cn.docstatus in ('CO','CL') "
            //              // Commented because Against Business Partner there is no charge
            //              // + " AND cn.C_Charge_ID  IS Not NULL"
            //          + " AND cn.C_BPartner_ID=" + _C_BPartner_ID;
            //          if (!chk) {
            //              sqlCash += " AND cn.C_Currency_ID=" + _C_Currency_ID;
            //          }
            //          sqlCash += " ORDER BY cn.created,cn.receiptno";

            //          sqlCash = role.addAccessSQL(sqlCash, "cn", true, false);

            //          var dsCash = VIS.DB.executeDataSet(sqlCash);

            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetCashJounral",
                data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, page: pageNoCashJournal, size: PAGESIZE },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {
                        if (pageNoCashJournal == 1 && data.length > 0) {
                            CashRecord = data[0].CashRecord;
                            gridPgnoCashJounal = Math.ceil(CashRecord / PAGESIZE);
                        }
                        bindCashline(data, chk);
                        //added window no to find grid to implement scroll  issue : scroll was not working properly
                        $divCashline.find('#grid_openformatgridcash_' + $self.windowNo + '_records').on('scroll', cartCashScroll);
                        isCashGridLoaded = true;
                        isBusyIndicatorHidden();
                        //_payment = 7;
                    }
                },
                error: function (err) {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });

            //$divCashline.find('#grid_openformatgridcash_records').on('scroll', cartCashScroll);



            //            var sqlInvoice = "SELECT 'false' as SELECTROW , i.DateInvoiced  as DATE1 ,"
            //+ "  i.DocumentNo    AS DOCUMENTNO  ,"
            //+ "  i.C_Invoice_ID AS CINVOICEID,"
            //+ "  c.ISO_Code AS ISO_CODE    ,"
            //                //+ "  (i.GrandTotal  *i.MultiplierAP) AS CURRENCY    ,"
            //+ "  (invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP) AS CURRENCY    ,"
            //                //+ "currencyConvert(i.GrandTotal  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ) AS CONVERTED  ,"
            //+ "currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ) AS CONVERTED  ,"
            //+ " currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + ",i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)                                         *i.MultiplierAP AS AMOUNT,"
            //                //+ "(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID)) AS AMOUNT ,"
            //+ "  (currencyConvert(invoiceDiscount(i.C_Invoice_ID ," + getDate + ",C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID )*i.Multiplier*i.MultiplierAP) AS DISCOUNT ,"
            //+ "  i.MultiplierAP ,i.docbasetype  ,"
            //+ "0 as WRITEOFF ,"
            //+ "0 as APPLIEDAMT , i.C_InvoicePaySchedule_ID "
            //                 + " FROM C_Invoice_v i"		//  corrected for CM/Split
            //                 + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) "
            //                 + "WHERE i.IsPaid='N' AND i.Processed='Y'"
            //                 + " AND i.C_BPartner_ID=" + _C_BPartner_ID;                                            //  #5
            //            if (!chk) {
            //                sqlInvoice += " AND i.C_Currency_ID=" + _C_Currency_ID;                                   //  #6
            //            }
            //            sqlInvoice += " ORDER BY i.DateInvoiced, i.DocumentNo";

            //            sqlInvoice = role.addAccessSQL(sqlInvoice, "i", true, false);

            //var dsInvoice = VIS.DB.executeDataSet(sqlInvoice);
            //-----------------------------------Neha------------------
            //$.ajax({
            //    url: VIS.Application.contextUrl + "PaymentAllocation/GetInvoice",
            //    data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, chk: chk, date: getDate, page: pageNoInvoice, size: PAGESIZE },
            //    async: true,
            //    success: function (result) {
            //        var data = JSON.parse(result);
            //        if (data) {
            //            if (pageNoInvoice == 1 && data.length > 0) {
            //                invoiceRecord = data[0].InvoiceRecord;
            //                gridPgnoInvoice = Math.ceil(invoiceRecord / PAGESIZE);
            //            }
            //            bindInvoiceGrid(data, chk);
            //            _openInv = 9;
            //            // _discount = chk ? 7 : 5;
            //            _discount = 10;
            //            _writeOff = 13;
            //            _applied = 14;
            //            $divInvoice.find('#grid_openformatgridinvoice_records').on('scroll', cartInvoiceScroll);
            //            isInvoiceGridLoaded = true;
            //            isBusyIndicatorHidden();
            //        }
            //    },
            //    error: function (err) {
            //        $bsyDiv[0].style.visibility = "hidden";
            //    }
            //});
            loadInvoice();//Neha
            //$divInvoice.find('#grid_openformatgridinvoice_records').on('scroll', cartInvoiceScroll);


            //vdgvInvoice.AutoSize = true;
            //setTimeout(function () {
            //    calculate();
            //}, 60);

            // refresh label on load
            refreshLabel();
            $allocationFrom.val("P");
            $allocationFrom.trigger("change");
            $allocationTo.trigger("change");
            displayGrids($allocationFrom.val(), $allocationTo.val());
            //$bsyDiv[0].style.visibility = "hidden";
        };

        function loadCurrencyPrecision() {
            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetCurrencyPrecision",
                data: { _C_Currency_ID: _C_Currency_ID },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {
                        stdPrecision = data;
                    }
                },
                error: function (err) {
                }
            });

        };

        // is used to shown busy indicator when we change BP and other parameters
        function isBusyIndicatorHidden() {
            if (isPaymentGridLoaded == true && isCashGridLoaded == true && isInvoiceGridLoaded == true) {
                $bsyDiv[0].style.visibility = "hidden";
            }
            //else if (isPaymentGridLoaded == false && isCashGridLoaded == true && isInvoiceGridLoaded == true) {
            //    $bsyDiv[0].style.visibility = "hidden";
            //}
            else {
                $bsyDiv[0].style.visibility = "visible";
            }
        }

        // scroll implementation for payment grid
        function cartPaymentScroll() {
            if ($(this).scrollTop() > 0) {
                $bsyDiv[0].style.visibility = "visible";
                if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                    if (pageNoPayment < gridPgnoPayment) {
                        pageNoPayment++;
                        getPaymentDataOnScroll(pageNoPayment, PAGESIZE);
                    }
                }
                $bsyDiv[0].style.visibility = "hidden";
            }
        };
        function getPaymentDataOnScroll(pageNoPayment, PAGESIZE) {
            var chk = $vchkMultiCurrency.is(':checked');
            var getDate = null;
            date = $date.val();
            if (date != null && date != undefined && date != "") {
                getDate = VIS.DB.to_date(date);
            }
            else
                getDate = VIS.DB.to_date(new Date());

            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetPayments",
                data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, page: pageNoPayment, size: PAGESIZE },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {
                        bindPaymentGridOnScroll(data);
                    }
                },
                error: function (err) {

                }
            });
        };

        // scroll implementation for cash grid
        function cartCashScroll() {
            if ($(this).scrollTop() > 0) {
                $bsyDiv[0].style.visibility = "visible";
                if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                    if (pageNoCashJournal < gridPgnoCashJounal) {
                        pageNoCashJournal++;
                        getCashDataOnScroll(pageNoCashJournal, PAGESIZE);
                    }
                }
                $bsyDiv[0].style.visibility = "hidden";
            }
        };
        function getCashDataOnScroll(pageNoCashJournal, PAGESIZE) {
            var chk = $vchkMultiCurrency.is(':checked');
            var getDate = null;
            date = $date.val();
            if (date != null && date != undefined && date != "") {
                getDate = VIS.DB.to_date(date);
            }
            else
                getDate = VIS.DB.to_date(new Date());

            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetCashJounral",
                data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, page: pageNoCashJournal, size: PAGESIZE },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {
                        bindCashlineGridOnScroll(data);
                    }
                },
                error: function (err) {

                }
            });
        };

        // scroll implementation for invoice grid
        function cartInvoiceScroll() {
            if ($(this).scrollTop() > 0) {
                $bsyDiv[0].style.visibility = "visible";
                if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                    if (pageNoInvoice < gridPgnoInvoice) {
                        pageNoInvoice++;
                        getInvoiceDataOnScroll(pageNoInvoice, PAGESIZE);
                    }
                }
                $bsyDiv[0].style.visibility = "hidden";
            }
        };
        function getInvoiceDataOnScroll(pageNoInvoice, PAGESIZE) {
            var chk = $vchkMultiCurrency.is(':checked');
            var getDate = null;
            date = $date.val();
            if (date != null && date != undefined && date != "") {
                getDate = VIS.DB.to_date(date);
            }
            else
                getDate = VIS.DB.to_date(new Date());

            //---Get values and pass values to the controller----Neha
            docNo = $txtDocNo.val();
            c_docType_ID = $cmbDocType.val();
            if (fromDate != null && fromDate != undefined && fromDate != "") {
                VIS.DB.to_date(fromDate);
            }
            if (toDate != null && toDate != undefined && toDate != "") {
                VIS.DB.to_date(toDate);
            }

            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/GetInvoice",
                data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, isInterBPartner: _isInterBPartner, chk: chk, date: getDate, page: pageNoInvoice, size: PAGESIZE, docNo: docNo, c_docType_ID: c_docType_ID, fromDate: fromDate, toDate: toDate, conversionDate: conversionDate },
                async: true,
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data) {

                        //if any invoice is selected and we load another invoice from db than we have to check weather that invoice is already selected or not. if already selected tha skip that.
                        if (selectedInvoices.length > 0) {
                            totalselectedinv = selectedInvoices.length;
                            for (var i = 0; i < data.length; i++) {
                                var filterObj = selectedInvoices.filter(function (e) {
                                    return e.C_InvoicePaySchedule_ID == data[i]["C_InvoicePaySchedule_ID"];
                                });

                                if (filterObj.length == 0) {
                                    data[i]["recid"] = selectedInvoices.length + i;
                                    selectedInvoices.push(data[i]);
                                }
                            }
                            data = selectedInvoices;
                            // Clear Selected invoices array when we de-select the select all checkbox. work done for to hold all the selected invoices
                            selectedInvoices = [];
                        }
                        //end

                        bindInvoiceGridOnScroll(data);
                        //Amount
                        _openInv = 9;
                        _discount = 10;
                        _writeOff = 13;
                        _applied = 14;
                    }
                },
                error: function (err) {

                }
            });
        };

        //payment grid bind
        function bindPaymentGrid(data, chk) {
            var columns = [];
            columns.push({ field: 'SelectRow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            columns.push({ field: "Date1", caption: VIS.translatedTexts.Date, size: '80px', hidden: false });
            columns.push({ field: "Documentno", caption: VIS.translatedTexts.DocumentNo, size: '120px', hidden: false });
            columns.push({ field: "CpaymentID", caption: VIS.translatedTexts.c_payment_id, size: '150px', hidden: true });
            if (chk) {
                columns.push({ field: "Isocode", caption: VIS.translatedTexts.TrxCurrency, size: '85px', hidden: false });
            }
            else {
                columns.push({ field: "Isocode", caption: VIS.translatedTexts.TrxCurrency, size: '85px', hidden: true });
            }
            columns.push({ field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false });
            if (chk) {
                columns.push({ field: "Payment", caption: VIS.translatedTexts.Amount, size: '150px', hidden: false });
            }
            else {
                columns.push({ field: "Payment", caption: VIS.translatedTexts.Amount, size: '150px', hidden: true });
            }
            columns.push({ field: "ConvertedAmount", caption: VIS.translatedTexts.ConvertedAmount, size: '150px', hidden: false });
            columns.push({ field: "OpenAmt", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, size: '150px', hidden: true });
            columns.push({ field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false });
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({ field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false });
            columns.push({ field: "AD_Org_ID", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: true });
            columns.push({ field: "OrgName", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: false });


            var rows = [];

            var colkeys = [];

            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna == "SelectRow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridPayment != undefined && $gridPayment != null) {
                $gridPayment.destroy();
                $gridPayment = null;
            }

            $gridPayment = $divPayment.w2grid({
                name: 'openformatgrid_' + $self.windowNo,
                show: { toolbar: true },
                multiSelect: true,
                columns: columns,
                records: rows,
                onClick: function (event) {
                    paymentCellClicked(event);
                    getMaxDate();
                },
                onChange: function (event) {
                    paymentCellChanged(event);
                },
                onDblClick: function (event) {
                    paymentDoubleClicked(event);
                },
                onSelect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            //added window no to find grid to implement scroll  issue : scroll was not working properly
                            $divPayment.find('#grid_openformatgrid_' + $self.windowNo + '_records').on('scroll', cartPaymentScroll);
                        }
                    }
                }
            });

            $gridPayment.autoLoad = true;
            $($($gridPayment.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($($gridPayment.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
            //$('.w2ui-search-down').css('margin-top', '4px');
            //$('.w2ui-search-clear').css('margin-top', '4px');

        };
        function bindPaymentGridOnScroll(data) {
            var rows = [];
            var colkeys = [];

            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            // check records exist in grid or not
            // for updating record id 
            var count = 0;
            if ($gridPayment.records.length > 0) {
                count = $gridPayment.records.length;
            };

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = count + r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna == "SelectRow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            // bind rows with grid
            w2utils.encodeTags(rows);
            $gridPayment.add(rows);

            rows = [];
            colkeys = [];
            //added window no to find grid to implement scroll  issue : scroll was not working properly
            $divPayment.find('#grid_openformatgrid_' + $self.windowNo + '_records').on('scroll', cartPaymentScroll);
        };

        //Cash line grid bind
        function bindCashline(data, chk) {
            var columns = [];
            columns.push({ field: 'SelectRow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            columns.push({ field: "Created", caption: VIS.translatedTexts.Date, size: '80px', hidden: false });
            columns.push({ field: "ReceiptNo", caption: VIS.translatedTexts.RECEIPTNO, size: '120px', hidden: false });
            if (chk == true) {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: false });
            }
            else {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: true });
            }
            columns.push({ field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false });
            if (chk == true) {
                columns.push({ field: "Amount", caption: VIS.translatedTexts.Amount, size: '150px', hidden: false });
            }
            else {
                columns.push({ field: "Amount", caption: VIS.translatedTexts.Amount, size: '150px', hidden: true });
            }

            columns.push({ field: "ConvertedAmount", caption: VIS.Msg.getMsg("ConvertedAmount"), size: '150px', hidden: false });
            columns.push({ field: "OpenAmt", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false });
            columns.push({ field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false });
            columns.push({ field: "CcashlineiID", caption: VIS.translatedTexts.ccashlineid, size: '150px', hidden: true });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, hidden: true });
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({ field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false });
            columns.push({ field: "AD_Org_ID", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: true });
            columns.push({ field: "OrgName", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: false });

            var rows = [];

            var colkeys = [];


            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna.toLower() == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridCashline != undefined && $gridCashline != null) {
                $gridCashline.destroy();
                $gridCashline = null;
            }

            $gridCashline = $divCashline.w2grid({
                name: 'openformatgridcash_' + $self.windowNo,
                multiSelect: true,
                show: { toolbar: true },
                columns: columns,
                records: rows,
                onChange: function (event) {
                    cashCellChanged(event);
                },
                onClick: function (event) {
                    cashCellClicked(event);
                    getMaxDate();
                },
                onDblClick: function (event) {
                    cashDoubleClicked(event);
                },
                onSelect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            //added window no to find grid to implement scroll  issue : scroll was not working properly
                            $divCashline.find('#grid_openformatgridcash_' + $self.windowNo + '_records').on('scroll', cartCashScroll);
                        }
                    }
                }
            });

            $gridCashline.autoLoad = true;
            $($($gridCashline.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($($gridCashline.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
            //$('.w2ui-search-down').css('margin-top', '4px');
            //$('.w2ui-search-clear').css('margin-top', '4px');
        };
        function bindCashlineGridOnScroll(data) {
            var rows = [];
            var colkeys = [];

            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            // check records exist in grid or not
            // for updating record id 
            var count = 0;
            if ($gridCashline.records.length > 0) {
                count = $gridCashline.records.length;
            };

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = count + r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna.toLower() == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            // bind rows with grid
            w2utils.encodeTags(rows);
            $gridCashline.add(rows);

            rows = [];
            colkeys = [];
            //added window no to find grid to implement scroll  issue : scroll was not working properly
            $divCashline.find('#grid_openformatgridcash_' + $self.windowNo + '_records').on('scroll', cartCashScroll);
        };

        //Invoice grid bind
        function bindInvoiceGrid(data, chk) {
            var columns = [];
            columns.push({ field: 'SelectRow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            columns.push({ field: "Date1", caption: VIS.translatedTexts.Date, render: 'date:yyyy-mm-dd', size: '80px', hidden: false });
            columns.push({ field: "InvoiceScheduleDate", caption: VIS.translatedTexts.ScheduleDate, size: '80px', hidden: false });
            columns.push({ field: "Documentno", caption: VIS.translatedTexts.DocumentNo, size: '120px', hidden: false });
            columns.push({ field: "CinvoiceID", caption: VIS.translatedTexts.cinvoiceid, size: '150px', hidden: true });
            if (chk) {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: false });
            }
            else {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: true });
            }
            columns.push({ field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false });
            if (chk) {
                columns.push({ field: "Currency", caption: VIS.translatedTexts.Amount, size: '100px', hidden: false });
            }
            else {
                columns.push({ field: "Currency", caption: VIS.translatedTexts.Amount, size: '100px', hidden: true });
            }
            columns.push({ field: "Converted", caption: VIS.Msg.getMsg("ConvertedAmount"), size: '100px', hidden: false });
            columns.push({ field: "Amount", caption: VIS.translatedTexts.OpenAmount, size: '100px', hidden: false });
            columns.push({ field: "Discount", caption: VIS.translatedTexts.DiscountAmt, size: '100px', hidden: false });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, hidden: true });
            columns.push({ field: "DocBaseType", caption: VIS.translatedTexts.DocBaseType, size: '150px', hidden: true });
            columns.push({ field: "Writeoff", caption: VIS.translatedTexts.WriteOffAmount, size: '100px', hidden: false });
            columns.push({ field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '100px', hidden: false });
            if (countVA009 > 0) {
                columns.push({ field: "C_InvoicePaySchedule_ID", caption: VIS.translatedTexts.C_InvoicePaySchedule_ID, size: '100px', hidden: false });
            }
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({ field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false });
            columns.push({ field: "AD_Org_ID", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: true });
            columns.push({ field: "OrgName", caption: VIS.translatedTexts.AD_Org_ID, size: '85px', hidden: false });
            //}
            //end
            //}


            var rows = [];

            var colkeys = [];


            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna.toLower() == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridInvoice != undefined && $gridInvoice != null) {
                $gridInvoice.destroy();
                $gridInvoice = null;
            }

            $gridInvoice = $divInvoice.w2grid({
                name: 'openformatgridinvoice_' + $self.windowNo,
                show: { toolbar: true },
                multiSelect: true,
                columns: columns,
                records: rows,
                //    searches: [
                //{ field: 'Date1', caption: 'Date', type: 'date', render: 'date:yyyy-mm-dd' },
                //{ field: 'Documentno', caption: 'Document no', type: 'text' },
                //{ field: 'DocBaseType1', caption: 'DocBaseType', type: 'list', options: { items: DocBaseTypeData } },
                //{ field: 'DocBaseType', caption: 'Document Type', type: 'text' }

                //    ],
                onChange: function (event) {
                    invoiceCellChanged(event);
                },
                onClick: function (event) {
                    invoiceCellClicked(event);
                    //getMaxDate();
                },
                onDblClick: function (event) {
                    invoiceDoubleClicked(event);
                },
                onSelect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            //added window no to find grid to implement scroll  issue : scroll was not working properly
                            $divInvoice.find('#grid_openformatgridinvoice_' + $self.windowNo + '_records').on('scroll', cartInvoiceScroll);
                        }
                    }
                }
            });
            // when we load new invoice schedules either after filter or directly and if any invoice schedule is selected already than we need to trigger it's click event.
            if (totalselectedinv > 0) {
                for (var i = 0; i < totalselectedinv; i++) {
                    var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                    $(chk[i]).prop('checked', true);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, event);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }
            }
            totalselectedinv = 0;
            //end

            $gridInvoice.autoLoad = true;
            $($($gridInvoice.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($($gridInvoice.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
            //$('.w2ui-search-down').css('margin-top', '4px');
            //$('.w2ui-search-clear').css('margin-top', '4px');
        };
        function bindInvoiceGridOnScroll(data) {
            var rows = [];
            var colkeys = [];

            if (data.length > 0) {
                colkeys = Object.keys(data[0]);
            }

            // check records exist in grid or not
            // for updating record id 
            var count = 0;
            if ($gridInvoice.records.length > 0) {
                count = $gridInvoice.records.length;
            };

            for (var r = 0; r < data.length; r++) {
                var singleRow = {};
                singleRow['recid'] = count + r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna.toLower() == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(data[r][colna]);
                    }
                }
                rows.push(singleRow);
            }

            // bind rows with grid
            w2utils.encodeTags(rows);
            $gridInvoice.add(rows);

            // when we load new invoice schedules either after filter or directly and if any invoice schedule is selected already than we need to trigger it's click event.
            if (totalselectedinv > 0) {
                for (var i = 0; i < totalselectedinv; i++) {
                    var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                    $(chk[i]).prop('checked', true);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, event);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }
            }
            totalselectedinv = 0;
            //end

            rows = [];
            colkeys = [];
            //added window no to find grid to implement scroll  issue : scroll was not working properly
            $divInvoice.find('#grid_openformatgridinvoice_' + $self.windowNo + '_records').on('scroll', cartInvoiceScroll);
        };

        function paymentDoubleClicked(event) {
            if ($gridPayment.columns[event.column].field == "AppliedAmt") {
                var getChanges = $gridPayment.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].SelectRow == undefined) {
                    $gridPayment.columns[event.column].editable = false;
                    return;
                }
                $gridPayment.columns[event.column].editable = { type: 'text' }
            }
        };

        function cashDoubleClicked(event) {
            if ($gridCashline.columns[event.column].field == "AppliedAmt") {
                var getChanges = $gridCashline.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].SelectRow == undefined) {
                    $gridCashline.columns[event.column].editable = false;
                    return;
                }
                $gridCashline.columns[event.column].editable = { type: 'text' }
            }
        };

        function invoiceDoubleClicked(event) {
            if ($gridInvoice.columns[event.column].field == "AppliedAmt" ||
                $gridInvoice.columns[event.column].field == "Writeoff" ||
                $gridInvoice.columns[event.column].field == "Discount") {
                var getChanges = $gridInvoice.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].SelectRow == undefined) {
                    $gridInvoice.columns[event.column].editable = false;
                    return;
                }
                $gridInvoice.columns[event.column].editable = { type: 'text' }
            }
        };

        //added for gl-allocation
        function glDoubleClicked(event) {
            if (glLineGrid.columns[event.column].field == "AppliedAmt") {
                var getChanges = glLineGrid.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].SelectRow == undefined) {
                    glLineGrid.columns[event.column].editable = false;
                    return;
                }
                if (element[0].SelectRow == true) {
                    glLineGrid.columns[event.column].editable = { type: 'text' };
                }
                else {
                    glLineGrid.columns[event.column].editable = false;
                    return;
                }
            }
        };
        //end

        function invoiceCellClicked(event) {
            var colIndex = $gridInvoice.getColumn('AppliedAmt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridInvoice.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    $gridInvoice.refreshCell(0, "AppliedAmt");
                    $gridInvoice.refreshCell(0, "Writeoff");
                    $gridInvoice.refreshCell(0, "Discount");
                }
                else {
                    if (element[0].SelectRow == true) {
                        // when we select a record, check conversion type is same or not.
                        // if not then not to select this record
                        if (C_ConversionType_ID == 0) {
                            C_ConversionType_ID = $gridInvoice.get(event.recid).C_ConversionType_ID;
                        }
                        else if (C_ConversionType_ID != $gridInvoice.get(event.recid).C_ConversionType_ID) {

                            var $x = document.getElementsByName(event.target);
                            if ($x.length > 0)
                                $x = $x[0];

                            var $z = $($x).find("#grid_openformatgridinvoice_rec_" + event.recid + "");

                            if ($z.length > 0)
                                $z = $z[0];
                            if ($($z).find("input:checked").prop('checked')) {
                                $($z).find("input:checked").prop('checked', false);
                            }

                            $gridInvoice.get(event.recid).changes = false;
                            $gridInvoice.columns[colIndex].editable = false;
                            $gridInvoice.get(event.recid).changes.AppliedAmt = "0";
                            $gridInvoice.refreshCell(0, "AppliedAmt");
                            $gridInvoice.get(event.recid).changes.Discount = "0";
                            $gridInvoice.refreshCell(0, "Discount");
                            $gridInvoice.get(event.recid).changes.Writeoff = "0";
                            $gridInvoice.refreshCell(0, "Writeoff");
                            //alert("Conversion Type not matched");
                            VIS.ADialog.warn(("VIS_ConversionNotMatched"));
                        }
                        if ($vchkMultiCurrency.is(':checked')) {
                            if ($conversionDate.val() == "") {
                                VIS.ADialog.warn("VIS_SlctcnvrsnDate");
                                $gridInvoice.get(event.recid).changes = false;
                                $gridInvoice.unselect(event.recid);
                                $gridInvoice.columns[colIndex].editable = false;
                                $gridInvoice.get(event.recid).changes.AppliedAmt = "0";
                                $gridInvoice.refreshCell(event.recid, "AppliedAmt");
                                var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                                $(chk[event.recid]).prop('checked', false);
                                $gridInvoice.editChange.call($gridInvoice, chk[event.recid], event.recid, 0, event);
                                var eData = { "type": "click", "phase": "before", "target": "grid", "recid": event.recid, "index": event.recid, "isStopped": false, "isCan//celled": false, "onComplete": null };
                                $gridInvoice.trigger(eData);
                                return false;
                            }
                            var ConversnDate = Globalize.format(new Date($conversionDate.val()), "yyyy-MM-dd");
                            $dateAcct.val(Globalize.format(new Date(ConversnDate), "yyyy-MM-dd"));
                        }
                        else {
                            getMaxDate();
                        }
                    }
                    else {
                        $gridInvoice.unselect(event.recid);
                        $gridInvoice.columns[colIndex].editable = false;
                        $gridInvoice.get(0).changes.AppliedAmt = "0";
                        $gridInvoice.refreshCell(0, "AppliedAmt");
                        $gridInvoice.get(0).changes.Discount = "0";
                        $gridInvoice.refreshCell(0, "Discount");
                        $gridInvoice.get(0).changes.Writeoff = "0";
                        $gridInvoice.refreshCell(0, "Writeoff");
                    }

                }
                tableChanged(event.recid, event.column, true, false);
            }
        };

        function paymentCellClicked(event) {
            if (readOnlyPayment) {
                event.preventDefault();
                return;
            }
            var colIndex = $gridPayment.getColumn('AppliedAmt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridPayment.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    $gridPayment.refreshCell(event.recid, "AppliedAmt");
                }
                else {
                    if (element[0].SelectRow == true) {
                        // when we select a record, check conversion type is same or not.
                        // if not then not to select this record
                        if (C_ConversionType_ID == 0) {
                            C_ConversionType_ID = $gridPayment.get(event.recid).C_ConversionType_ID;
                        }
                        else if (C_ConversionType_ID != $gridPayment.get(event.recid).C_ConversionType_ID) {

                            var $x = document.getElementsByName(event.target);
                            if ($x.length > 0)
                                $x = $x[0];

                            var $z = $($x).find("#grid_openformatgrid_rec_" + event.recid + "");

                            if ($z.length > 0)
                                $z = $z[0];
                            if ($($z).find("input:checked").prop('checked')) {
                                $($z).find("input:checked").prop('checked', false);
                            }

                            $gridPayment.get(event.recid).changes = false;
                            $gridPayment.unselect(event.recid);
                            $gridPayment.columns[colIndex].editable = false;
                            $gridPayment.get(event.recid).changes.AppliedAmt = "0";
                            $gridPayment.refreshCell(event.recid, "AppliedAmt");
                            VIS.ADialog.warn(("VIS_ConversionNotMatched"));
                        }
                        if ($vchkMultiCurrency.is(':checked')) {
                            if ($conversionDate.val() == "") {
                                VIS.ADialog.warn("VIS_SlctcnvrsnDate");
                                $gridPayment.get(event.recid).changes = false;
                                $gridPayment.unselect(event.recid);
                                $gridPayment.columns[colIndex].editable = false;
                                $gridPayment.get(event.recid).changes.AppliedAmt = "0";
                                $gridPayment.refreshCell(event.recid, "AppliedAmt");
                                var chk = $('#grid_' + $gridPayment.name + '_records td[col="0"]').find('input[type="checkbox"]');
                                $(chk[event.recid]).prop('checked', false);
                                $gridPayment.editChange.call($gridPayment, chk[event.recid], event.recid, 0, event);
                                var eData = { "type": "click", "phase": "before", "target": "grid", "recid": event.recid, "index": event.recid, "isStopped": false, "isCan//celled": false, "onComplete": null };
                                $gridPayment.trigger(eData);
                                return false;
                            }
                            var DATEACCT = $gridPayment.records[event.recid].DATEACCT;
                            var ConversnDate = Globalize.format(new Date($conversionDate.val()), "yyyy-MM-dd");
                            DATEACCT = Globalize.format(new Date(DATEACCT), "yyyy-MM-dd");
                            if (ConversnDate != DATEACCT) {
                                VIS.ADialog.warn("VIS_SelectSameDate");
                                var colIndex = $gridPayment.getColumn('DATEACCT', true);
                                $gridPayment.get(event.recid).changes = false;
                                $gridPayment.unselect(event.recid);
                                $gridPayment.columns[colIndex].editable = false;
                                $gridPayment.get(event.recid).changes.AppliedAmt = "0";
                                $gridPayment.refreshCell(event.recid, "AppliedAmt");
                                var chk = $('#grid_' + $gridPayment.name + '_records td[col="0"]').find('input[type="checkbox"]');
                                $(chk[event.recid]).prop('checked', false);
                                $gridPayment.editChange.call($gridPayment, chk[event.recid], event.recid, 0, event);
                                var eData = { "type": "click", "phase": "before", "target": "grid", "recid": event.recid, "index": event.recid, "isStopped": false, "isCan//celled": false, "onComplete": null };
                                $gridPayment.trigger(eData);
                                return false;
                            }
                        }
                    }
                    else {
                        $gridPayment.unselect(event.recid);
                        $gridPayment.columns[colIndex].editable = false;
                        $gridPayment.get(event.recid).changes.AppliedAmt = "0";
                        $gridPayment.refreshCell(event.recid, "AppliedAmt");
                    }

                }
                tableChanged(event.recid, event.column, false, false);
            }
        };

        function cashCellClicked(event) {
            if (readOnlyCash) {
                event.preventDefault();
                return;
            }
            var colIndex = $gridCashline.getColumn('AppliedAmt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridCashline.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    $gridCashline.refreshCell(0, "AppliedAmt");
                }
                else {
                    if (element[0].SelectRow == true) {
                        // when we select a record, check conversion type is same or not.
                        // if not then not to select this record
                        if (C_ConversionType_ID == 0) {
                            C_ConversionType_ID = $gridCashline.get(event.recid).C_ConversionType_ID;
                        }
                        else if (C_ConversionType_ID != $gridCashline.get(event.recid).C_ConversionType_ID) {

                            var $x = document.getElementsByName(event.target);
                            if ($x.length > 0)
                                $x = $x[0];

                            var $z = $($x).find("#grid_openformatgridcash_rec_" + event.recid + "");

                            if ($z.length > 0)
                                $z = $z[0];
                            if ($($z).find("input:checked").prop('checked')) {
                                $($z).find("input:checked").prop('checked', false);
                            }

                            $gridCashline.get(event.recid).changes = false;
                            $gridCashline.unselect(event.recid);
                            $gridCashline.columns[colIndex].editable = false;
                            //$gridCashline.get(0).changes.AppliedAmt = "0";
                            $gridCashline.refreshCell(0, "AppliedAmt");
                            VIS.ADialog.warn(("VIS_ConversionNotMatched"));
                        }

                        if ($vchkMultiCurrency.is(':checked')) {
                            if ($conversionDate.val() == "") {
                                VIS.ADialog.warn("VIS_SlctcnvrsnDate");
                                $gridCashline.get(event.recid).changes = false;
                                $gridCashline.unselect(event.recid);
                                $gridCashline.columns[colIndex].editable = false;
                                // $gridCashline.get(event.recid).changes.AppliedAmt = "0";
                                $gridCashline.refreshCell(event.recid, "AppliedAmt");
                                var chk = $('#grid_' + $gridCashline.name + '_records td[col="0"]').find('input[type="checkbox"]');
                                $(chk[event.recid]).prop('checked', false);
                                $gridCashline.editChange.call($gridCashline, chk[event.recid], event.recid, 0, event);
                                var eData = { "type": "click", "phase": "before", "target": "grid", "recid": event.recid, "index": event.recid, "isStopped": false, "isCan//celled": false, "onComplete": null };
                                $gridCashline.trigger(eData);
                                return false;
                            }
                            var DATEACCT = $gridCashline.records[event.recid].DATEACCT;
                            var ConversnDate = Globalize.format(new Date($conversionDate.val()), "yyyy-MM-dd");
                            DATEACCT = Globalize.format(new Date(DATEACCT), "yyyy-MM-dd");
                            if (ConversnDate != DATEACCT) {
                                VIS.ADialog.warn("VIS_SelectSameDate");
                                var colIndex = $gridCashline.getColumn('DATEACCT', true);
                                $gridCashline.get(event.recid).changes = false;
                                $gridCashline.unselect(event.recid);
                                $gridCashline.columns[colIndex].editable = false;
                                //$gridCashline.get(event.recid).changes.AppliedAmt = "0";
                                $gridCashline.refreshCell(event.recid, "AppliedAmt");
                                var chk = $('#grid_' + $gridCashline.name + '_records td[col="0"]').find('input[type="checkbox"]');
                                $(chk[event.recid]).prop('checked', false);
                                $gridCashline.editChange.call($gridCashline, chk[event.recid], event.recid, 0, event);
                                var eData = { "type": "click", "phase": "before", "target": "grid", "recid": event.recid, "index": event.recid, "isStopped": false, "isCan//celled": false, "onComplete": null };
                                $gridCashline.trigger(eData);
                                return false;
                            }
                        }
                    }
                    else {
                        $gridCashline.unselect(event.recid);
                        $gridCashline.columns[colIndex].editable = false;
                        $gridCashline.get(0).changes.AppliedAmt = "0";
                        $gridCashline.refreshCell(0, "AppliedAmt");
                    }
                }
                tableChanged(event.recid, event.column, true, true);
            }
        };

        function paymentCellChanged(event) {

            if (readOnlyPayment) {
                event.preventDefault();
                return;
            }
            var colIndex = $gridPayment.getColumn('AppliedAmt', true);

            if ($gridPayment.columns[colIndex].editable == undefined) {
                return;
            }
            if ($gridPayment.getChanges(event.recid) != undefined && $gridPayment.getChanges(event.recid).length > 0 && $gridPayment.get(event.recid).changes) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (VIS.Utility.Util.getValueOfDecimal(event.value_new) > 0 && VIS.Utility.Util.getValueOfDecimal(event.value_new) > VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt) < 0 && VIS.Utility.Util.getValueOfDecimal(event.value_new) < VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                //added for gl-allocation
                else if (VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt) > 0 && VIS.Utility.Util.getValueOfDecimal(event.value_new) < 0) {
                    $gridPayment.set(0, { "AppliedAmt": VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt) });
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }

                $gridPayment.get(event.recid).changes.AppliedAmt = event.value_new;
                $gridPayment.refreshCell(event.recid, "AppliedAmt");
            }
            else {
                if (event.column > 0) {
                    $gridPayment.set(event.recid, { AppliedAmt: event.value_new });
                }
            }
            if (event.column == colIndex) {
                //logic to not set greater appliedAmount then open amount
                if (parseFloat($gridPayment.get(event.index).OpenAmt) > parseFloat($gridPayment.get(event.index).AppliedAmt)) {

                }
                else {
                    $gridPayment.set(0, { "AppliedAmt": $gridPayment.get(event.index).OpenAmt });
                }
                tableChanged(event.index, event.column, false, false);
            }
        };

        //added for gl-allocation
        function glCellChanged(event) {
            if (readOnlyGL) {
                event.preventDefault();
                return;
            }
            var colIndex = glLineGrid.getColumn('AppliedAmt', true);

            if (glLineGrid.columns[colIndex].editable == undefined) {
                return;
            }

            if (glLineGrid.getChanges(event.recid) != undefined && glLineGrid.getChanges(event.recid).length > 0 && glLineGrid.get(event.recid).changes) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (VIS.Utility.Util.getValueOfDecimal(event.value_new) > 0 && VIS.Utility.Util.getValueOfDecimal(event.value_new) > VIS.Utility.Util.getValueOfDecimal(glLineGrid.get(event.recid).OpenAmount)) {
                    glLineGrid.set(0, { "AppliedAmt": VIS.Utility.Util.getValueOfDecimal(glLineGrid.get(event.recid).OpenAmount) });
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfDecimal(glLineGrid.get(event.recid).OpenAmount) < 0 && VIS.Utility.Util.getValueOfDecimal(event.value_new) < VIS.Utility.Util.getValueOfDecimal(glLineGrid.get(event.recid).OpenAmount)) {
                    glLineGrid.set(0, { "AppliedAmt": VIS.Utility.Util.getValueOfDecimal(glLineGrid.get(event.recid).OpenAmount) });
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }

                if (colIndex == event.column)
                    glLineGrid.get(event.recid).changes.AppliedAmt = event.value_new;

                glLineGrid.refreshCell(event.recid, "AppliedAmt");
            }
            else {
                if (event.column > 0) {
                    glLineGrid.set(event.recid, { AppliedAmt: event.value_new });
                }
            }

            if (event.column == colIndex) {
                //logic to not set greater appliedAmount then open amount
                if (parseFloat(glLineGrid.get(event.recid).OpenAmount) > parseFloat(glLineGrid.get(event.recid).AppliedAmt)) {

                }
                else {
                    glLineGrid.set(0, { "AppliedAmt": glLineGrid.get(event.recid).OpenAmount });
                }
                // tableChanged(event.index, event.column, true, true);
            }
            calculate();
        };
        //end

        function cashCellChanged(event) {
            if (readOnlyCash) {
                event.preventDefault();
                return;
            }
            var colIndex = $gridCashline.getColumn('AppliedAmt', true);

            if ($gridCashline.columns[colIndex].editable == undefined) {
                return;
            }
            if ($gridCashline.getChanges(event.recid) != undefined && $gridCashline.getChanges(event.recid).length > 0 && $gridCashline.get(event.recid).changes) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (VIS.Utility.Util.getValueOfInt(event.value_new) > 0 && VIS.Utility.Util.getValueOfInt(event.value_new) > VIS.Utility.Util.getValueOfInt($gridCashline.get(event.recid).OpenAmt)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfInt($gridCashline.get(event.recid).OpenAmt) < 0 && VIS.Utility.Util.getValueOfInt(event.value_new) < VIS.Utility.Util.getValueOfInt($gridCashline.get(event.recid).OpenAmt)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                $gridCashline.get(event.recid).changes.AppliedAmt = event.value_new;
                $gridCashline.refreshCell(event.recid, "AppliedAmt");
            }
            else {
                if (event.column > 0) {
                    $gridCashline.set(event.recid, { AppliedAmt: event.value_new });
                }
            }
            if (event.column == colIndex) {
                //logic to not set greater appliedAmount then open amount
                if (parseFloat($gridCashline.get(event.index).OpenAmt) > parseFloat($gridCashline.get(event.index).AppliedAmt)) {

                }
                else {
                    $gridCashline.set(0, { "AppliedAmt": $gridCashline.get(event.index).paidamount });
                }
                tableChanged(event.index, event.column, true, true);
            }




            //var colIndex = $gridCashline.getColumn('AppliedAmt', true);

            //if ($gridCashline.columns[colIndex].editable == undefined) {
            //    return;
            //}

            //if (event.column == colIndex) {
            //    //logic to not set greater appliedAmount then open amount
            //    if (parseFloat($gridCashline.get(event.index).openamt) > parseFloat($gridCashline.get(event.index).AppliedAmt)) {

            //    }
            //    else {
            //        $gridCashline.set(0, { "AppliedAmt": $gridCashline.get(event.index).paidamount });
            //    }
            //    tableChanged(event.index, event.column, true, true);
            //}
        };

        function invoiceCellChanged(event) {

            var colIndex = $gridInvoice.getColumn('AppliedAmt', true);
            var wcolIndex = $gridInvoice.getColumn('Writeoff', true);
            var dcolIndex = $gridInvoice.getColumn('Discount', true);
            var discountchng = 0;
            if ($gridInvoice.columns[colIndex].editable == undefined && $gridInvoice.columns[wcolIndex].editable == undefined && $gridInvoice.columns[dcolIndex].editable == undefined) {
                return;
            }
            if ($gridInvoice.getChanges(event.recid) != undefined && $gridInvoice.getChanges(event.recid).length > 0 && $gridInvoice.get(event.recid).changes) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (VIS.Utility.Util.getValueOfInt(event.value_new) > 0 && VIS.Utility.Util.getValueOfInt(event.value_new) > VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount) < 0 && VIS.Utility.Util.getValueOfInt(event.value_new) < VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    event.preventDefault();
                    return;
                }
                if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount) > 0) {
                    discountchng = VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount);
                }
                else
                    discountchng = VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Discount);
                // check if applied amt is greater than open amount added by vivek on 06/01/2018                
                if (colIndex == event.column) {
                    // check if discount+writeoff + applied is gretaer than open amount added by vivek on 06/01/2018
                    if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) < 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Writeoff) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount))
                        < VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }
                    else if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) > 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Writeoff) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount))
                        > VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }

                    $gridInvoice.get(event.recid).changes.AppliedAmt = event.value_new;
                    $gridInvoice.refreshCell(event.recid, "AppliedAmt");
                }

                else if (wcolIndex == event.column) {
                    if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) < 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.AppliedAmt) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount))
                        < VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }
                    else if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) > 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.AppliedAmt) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount))
                        > VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }
                    $gridInvoice.get(event.recid).changes.Writeoff = event.value_new;
                    $gridInvoice.refreshCell(event.recid, "Writeoff");
                }
                else if (dcolIndex == event.column) {
                    if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) < 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.AppliedAmt) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Writeoff))
                        < VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }
                    else if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) > 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.AppliedAmt) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Writeoff))
                        > VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        event.preventDefault();
                        return;
                    }
                    $gridInvoice.get(event.recid).changes.Discount = event.value_new;
                    $gridInvoice.refreshCell(event.recid, "Discount");
                }
            }
            else {
                if (event.column > 0) {
                    if (colIndex == event.column) {
                        $gridInvoice.set(event.recid, { AppliedAmt: event.value_new });
                    }
                    else if (wcolIndex == event.column) {
                        $gridInvoice.set(event.recid, { Writeoff: event.value_new });
                    }
                    else if (dcolIndex == event.column) {
                        $gridInvoice.set(event.recid, { Discount: event.value_new });
                    }
                }
            }

            if (event.column == $gridInvoice.getColumn('Discount', true)
                || event.column == $gridInvoice.getColumn('Writeoff', true)
                || event.column == $gridInvoice.getColumn('AppliedAmt', true)) {

                tableChanged(event.index, event.column, true, false);

            }
        };

        function tableChanged(rowIndex, colIndex, isInvoice, cash) {

            //var rowInvoice = $gridInvoice.getSelection();
            //var rowPayment = $gridPayment.getSelection();
            //var rowCash = $gridCashline.getSelection();
            //  Not a table update
            //if (!isUpdate)
            //{
            //    Calculate();
            //    return;
            //}

            //Setting defaults
            //if (_calculating)  //  Avoid recursive calls
            //    return;
            //_calculating = true;
            var row = rowIndex;
            var col = colIndex;
            if (col == null || col == undefined) {
                col = 0;
            }
            //   log.Config("Row=" + row + ", Col=" + col + ", InvoiceTable=" + isInvoice);

            var AllocationDate = null;
            //  Payments
            if (!isInvoice) {
                //TableModel payment = vdgvInvoice.getModel();
                if (col == 0) {
                    var columns = $gridPayment.columns;
                    colPayCheck = getBoolValue($gridPayment.getChanges(), row);

                    //AppliedAmt---//get column index from grid
                    _payment = getIndexFromArray(columns, "AppliedAmt");
                    var payemntCol = columns[_payment].field;


                    //  selected - set payment amount
                    var changes = $gridPayment.get(row).changes;

                    if (colPayCheck) {

                        //OpenAmt--//get column index from grid
                        _openPay = getIndexFromArray(columns, "OpenAmt");

                        var amount = parseFloat($gridPayment.get(row)[columns[_openPay].field]);

                        //Date1--//get column index from grid
                        var _date1 = getIndexFromArray(columns, "Date1");
                        AllocationDate = new Date($gridPayment.get(row)[columns[_date1].field]);

                        if (payemntCol == "AppliedAmt") {
                            if (changes != null && changes != undefined) {
                                changes.AppliedAmt = amount;
                                $gridPayment.refreshCell(row, "AppliedAmt");
                            }
                            else {
                                $gridPayment.set(row, { "AppliedAmt": amount });
                            }
                        }
                    }
                    else    //  de-selected
                    {
                        if (payemntCol == "AppliedAmt") {
                            if (changes != null && changes != undefined) {
                                changes.AppliedAmt = 0;
                                $gridPayment.refreshCell(row, "AppliedAmt");
                            }
                            else {
                                $gridPayment.set(row, { "AppliedAmt": 0 });
                            }
                        }
                        var isAllUncheckedInvoice = false;
                        for (var i = 0; i < $gridInvoice.getChanges().length; i++) {
                            if (!($gridInvoice.getChanges()[i].SelectRow == undefined || $gridInvoice.getChanges()[i].SelectRow == false))
                                isAllUncheckedInvoice = true;
                        }

                        var isAllUncheckedCash = false;
                        for (var i = 0; i < $gridCashline.getChanges().length; i++) {
                            if (!($gridCashline.getChanges()[i].SelectRow == undefined || $gridCashline.getChanges()[i].SelectRow == false))
                                isAllUncheckedCash = true;
                        }

                        var isAllUncheckedPayment = false;
                        for (var i = 0; i < $gridPayment.getChanges().length; i++) {
                            if (!($gridPayment.getChanges()[i].SelectRow == undefined || $gridPayment.getChanges()[i].SelectRow == false))
                                isAllUncheckedPayment = true;
                        }

                        if (!isAllUncheckedInvoice && !isAllUncheckedCash && !isAllUncheckedPayment)
                            C_ConversionType_ID = 0;
                    }
                }
            }
            //Cash Lines
            else if (cash) {

                if (col == 0) {
                    var columns = $gridCashline.columns;
                    colCashCheck = getBoolValue($gridCashline.getChanges(), row);

                    //AppliedAmt--- //get column index from grid
                    _paymentCash = getIndexFromArray(columns, "AppliedAmt");
                    var payemntCol = columns[_paymentCash].field;
                    //  selected - set payment amount
                    var changes = $gridCashline.get(row).changes;
                    //  selected - set payment amount
                    if (colCashCheck) {

                        //OpenAmt--//get column index from grid
                        _open = getIndexFromArray(columns, "OpenAmt");
                        var amount = parseFloat($gridCashline.get(row)[columns[_open].field]);
                        if (payemntCol == "AppliedAmt") {
                            if (changes != null && changes != undefined) {
                                changes.AppliedAmt = amount;
                                $gridCashline.refreshCell(row, "AppliedAmt");
                            }
                            else {
                                $gridCashline.set(row, { "AppliedAmt": amount });
                            }
                        }
                        //************************************** Changed
                    }
                    else    //  de-selected
                    {
                        //************************************** Changed
                        if (payemntCol == "AppliedAmt") {
                            if (changes != null && changes != undefined) {
                                changes.AppliedAmt = 0;
                                $gridCashline.refreshCell(row, "AppliedAmt");
                            }
                            else {
                                $gridCashline.set(row, { "AppliedAmt": 0 });
                            }
                        }
                        var isAllUncheckedInvoice = false;
                        for (var i = 0; i < $gridInvoice.getChanges().length; i++) {
                            if (!($gridInvoice.getChanges()[i].SelectRow == undefined || $gridInvoice.getChanges()[i].SelectRow == false))
                                isAllUncheckedInvoice = true;
                        }

                        var isAllUncheckedCash = false;
                        for (var i = 0; i < $gridCashline.getChanges().length; i++) {
                            if (!($gridCashline.getChanges()[i].SelectRow == undefined || $gridCashline.getChanges()[i].SelectRow == false))
                                isAllUncheckedCash = true;
                        }

                        var isAllUncheckedPayment = false;
                        for (var i = 0; i < $gridPayment.getChanges().length; i++) {
                            if (!($gridPayment.getChanges()[i].SelectRow == undefined || $gridPayment.getChanges()[i].SelectRow == false))
                                isAllUncheckedPayment = true;
                        }

                        if (!isAllUncheckedInvoice && !isAllUncheckedCash && !isAllUncheckedPayment)
                            C_ConversionType_ID = 0;
                    }
                }
            }
            //  Invoice Selection
            else if (col == 0) {
                var columns = $gridInvoice.columns;

                colInvCheck = getBoolValue($gridInvoice.getChanges(), row);

                //var payemntCol = columns[_payment].field;
                //  selected - set payment amount
                var changes = $gridInvoice.get(row).changes;
                //  selected - set payment amount

                //Writeoff--//get column index from grid
                _writeOff = getIndexFromArray(columns, "Writeoff");
                var writeOff = columns[_writeOff].field;

                //AppliedAmt-- //get column index from grid
                _applied = getIndexFromArray(columns, "AppliedAmt");
                var applied = columns[_applied].field;
                //  selected - set applied amount
                if (colInvCheck) {

                    // prepared same array for grid when we select any invoice from invoice grid and push that object into selected invoice array.
                    var record = $gridInvoice.records[row];
                    var rcdRow = {
                        SelectRow: record.SelectRow,
                        InvoiceRecord: record.InvoiceRecord,
                        Date1: record.Date1,
                        Documentno: record.Documentno,
                        CinvoiceID: record.CinvoiceID,
                        Isocode: record.Isocode,
                        Currency: record.Currency,
                        Converted: record.Converted,
                        Amount: record.Amount,
                        Discount: record.Discount,
                        Multiplierap: record.Multiplierap,
                        DocBaseType: record.DocBaseType,
                        Writeoff: record.Writeoff,
                        AppliedAmt: record.AppliedAmt,
                        C_InvoicePaySchedule_ID: record.C_InvoicePaySchedule_ID,
                        InvoiceScheduleDate: record.InvoiceScheduleDate,
                        C_ConversionType_ID: record.C_ConversionType_ID,
                        ConversionName: record.ConversionName,
                        DATEACCT: record.DATEACCT,
                        AD_Org_ID: record.AD_Org_ID,
                        OrgName: record.OrgName
                    };
                    selectedInvoices.push(rcdRow);

                    //Amount-- //get column index from grid
                    _openInv = getIndexFromArray(columns, "Amount");
                    var amount = parseFloat($gridInvoice.get(row)[columns[_openInv].field]);
                    //Discount  //get column index from grid
                    _discount = getIndexFromArray(columns, "Discount");
                    amount = amount - parseFloat($gridInvoice.get(row)[columns[_discount].field]);
                    if (applied == "AppliedAmt") {
                        if (changes != null && changes != undefined) {
                            changes.Writeoff = 0;
                            changes.AppliedAmt = amount;
                            //get column index from grid
                            _discount = getIndexFromArray(columns, "Discount");
                            changes.Discount = parseFloat($gridInvoice.get(row)[columns[_discount].field]);
                            $gridInvoice.refreshCell(row, "Writeoff");
                            $gridInvoice.refreshCell(row, "AppliedAmt");
                            $gridInvoice.refreshCell(row, "Discount");
                        }
                        else {
                            $gridInvoice.set(row, { "Writeoff": 0 });
                            $gridInvoice.set(row, { "AppliedAmt": amount });

                        }
                    }
                    //************************************** Changed
                }
                else    //  de-selected
                {
                    // remove invoice schedule from array when we de-select any schedule from invoice grid.
                    for (var x = 0; x < selectedInvoices.length; x++) {
                        if (selectedInvoices[x].C_InvoicePaySchedule_ID == $gridInvoice.records[row].C_InvoicePaySchedule_ID) {
                            selectedInvoices.splice(x, 1);
                        }
                    }

                    if (applied == "AppliedAmt") {
                        if (changes != null && changes != undefined) {
                            changes.Writeoff = 0;
                            changes.AppliedAmt = amount;
                            //Discount---//get column index from grid
                            _discount = getIndexFromArray(columns, "Discount");
                            changes.Discount = parseFloat($gridInvoice.get(row)[columns[_discount].field]);
                            $gridInvoice.refreshCell(row, "Writeoff");
                            $gridInvoice.refreshCell(row, "AppliedAmt");
                            $gridInvoice.refreshCell(row, "Discount");
                        }
                        else {
                            //$gridInvoice.set(row, { "Writeoff": 0 });
                            //$gridInvoice.set(row, { "AppliedAmt": amount });
                        }
                    }
                    var isAllUncheckedInvoice = false;
                    for (var i = 0; i < $gridInvoice.getChanges().length; i++) {
                        if (!($gridInvoice.getChanges()[i].SelectRow == undefined || $gridInvoice.getChanges()[i].SelectRow == false))
                            isAllUncheckedInvoice = true;
                    }

                    var isAllUncheckedCash = false;
                    for (var i = 0; i < $gridCashline.getChanges().length; i++) {
                        if (!($gridCashline.getChanges()[i].SelectRow == undefined || $gridCashline.getChanges()[i].SelectRow == false))
                            isAllUncheckedCash = true;
                    }

                    var isAllUncheckedPayment = false;
                    for (var i = 0; i < $gridPayment.getChanges().length; i++) {
                        if (!($gridPayment.getChanges()[i].SelectRow == undefined || $gridPayment.getChanges()[i].SelectRow == false))
                            isAllUncheckedPayment = true;
                    }

                    if (!isAllUncheckedInvoice && !isAllUncheckedCash && !isAllUncheckedPayment)
                        C_ConversionType_ID = 0;
                }
                //if (colInvCheck) {
                //    var amount = parseFloat($gridInvoice.get(row)[columns[_open].field]);
                //    amount = amount - parseFloat($gridInvoice.get(row)[columns[_discount].field]);


                //    $gridInvoice.set(row, { writeOff: 0 });
                //    $gridInvoice.set(row, { applied: amount });
                //}

            }

            //  Invoice - Try to balance entry
            if ($vchkAutoWriteOff.is(':checked')) {



                autoWriteOff();




                //var columns = $gridInvoice.columns;
                ////  if applied entered, adjust writeOff
                //var writeOff = columns[_writeOff].field
                //var applied = columns[_applied].field;
                //if (col == _applied) {
                //    var openAmount = parseFloat($gridInvoice.get(row)[columns[_open].field]);
                //    var amount = openAmount - parseFloat($gridInvoice.get(row)[columns[_discount].field]);

                //    amount = amount - parseFloat($gridInvoice.get(row)[columns[_applied].field]);

                //    $gridInvoice.set(row, { writeOff: amount });
                //    if ((amount / openAmount) > .30) {
                //        VIS.ADialog.error("AllocationWriteOffWarn");
                //    }
                //}
                //else    //  adjust applied
                //{
                //    var amount = parseFloat($gridInvoice.get(row)[columns[_open].field]); //  OpenAmount
                //    amount = amount - parseFloat($gridInvoice.get(row)[columns[_discount].field]);
                //    amount = amount - parseFloat($gridInvoice.get(row)[columns[_writeOff].field]);
                //    $gridInvoice.set(row, { AppliedAmt: amount });

                //}
            }
            calculate();
        };

        //to get index of column from array
        function getIndexFromArray(columns, name) {
            var index = 0;
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].field == name) {
                    index = i;
                    break;
                }
            }
            return index;
        };

        //added for gl-allocation
        function glTableChanged(rowIndex, colIndex) {

            var row = rowIndex;
            var col = colIndex;
            if (col == null || col == undefined) {
                col = 0;
            }

            var AllocationDate = null;
            //  GL Allocation
            if (col == 0) {
                var columns = glLineGrid.columns;
                colGlCheck = getBoolValue(glLineGrid.getChanges(), row);
                var payemntCol = columns[6].field;
                //  selected - set payment amount
                var changes;
                if (!glLineGrid.get(row)) { }
                else
                    changes = glLineGrid.get(row).changes;

                if (colGlCheck) {
                    var amount = parseFloat(glLineGrid.get(row)[columns[5].field]);
                    AllocationDate = new Date(glLineGrid.get(row)[columns[1].field]);
                    if (payemntCol == "AppliedAmt") {
                        if (changes != null && changes != undefined) {
                            changes.AppliedAmt = amount;
                            glLineGrid.refreshCell(row, "AppliedAmt");
                        }
                        else {
                            glLineGrid.set(row, { "AppliedAmt": amount });
                        }
                    }
                }
                else    //  de-selected
                {
                    if (payemntCol == "AppliedAmt") {
                        if (changes != null && changes != undefined) {
                            changes.AppliedAmt = 0;
                            glLineGrid.refreshCell(row, "AppliedAmt");
                        }
                        else {
                            glLineGrid.set(row, { "AppliedAmt": 0 });
                        }
                    }
                }
            }
            calculate();
        };
        //end

        function getBoolValue(changes, row) {
            if (changes == null || changes.length == 0) {
                return false;
            }
            var element = $.grep(changes, function (ele, index) {
                return parseInt(ele.recid) == row;
            });
            if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                return false;
            }
            else {
                return element[0].SelectRow;;
            }

        };

        function autoWriteOff() {

            invoiceTotal = 0;
            paymentTotal = 0;

            if ($vchkAllocation.is(':checked')) {
                var cashChanges = $gridCashline.getChanges();
                for (var i = 0; i < cashChanges.length; i++) {
                    if (cashChanges[i].SelectRow == true) {
                        paymentTotal += parseFloat(cashChanges[i].AppliedAmt);
                    }
                }
            }
            else {
                var paymentChanges = $gridPayment.getChanges();
                for (var i = 0; i < paymentChanges.length; i++) {
                    if (paymentChanges[i].SelectRow == true) {
                        paymentTotal += parseFloat(paymentChanges[i].AppliedAmt);
                    }
                }
            }






            var invoiceChanges = $gridInvoice.getChanges();

            var lastRow = null;
            var columns = $gridInvoice.columns;
            for (var i = invoiceChanges.length - 1; i >= 0; i--) {
                if (invoiceChanges[i].SelectRow == true) {
                    invoiceTotal += parseFloat(invoiceChanges[i].AppliedAmt);

                    if (lastRow == null) {
                        lastRow = $gridInvoice.get(invoiceChanges[i].recid);
                        //Amount
                        if (lastRow[columns[_openInv].field] <= 0) {
                            lastRow = null;
                        }
                    }

                }
            }


            //for (var i = 0; i < invoiceChanges.length; i++) {
            //    if (invoiceChanges[i].SelectRow == true) {
            //        paymentTotal += parseFloat(invoiceChanges[i].AppliedAmt);

            //        if (i == invoiceChanges.length - 1) {
            //            lastRow = $gridInvoice.get(invoiceChanges[i].recid);
            //        }

            //    }
            //}

            if (invoiceTotal > paymentTotal) {
                var difference = invoiceTotal - paymentTotal;
                if ((invoiceTotal * .05) >= difference) {
                    //    VIS.ADialog.error("AllocationWriteOffWarn");
                    //}
                    if (lastRow != null) {
                        lastRow.changes.Writeoff = difference;
                        lastRow.changes.AppliedAmt = lastRow.changes.AppliedAmt - difference;
                        $gridInvoice.refreshCell(lastRow.recid, "Writeoff");
                        $gridInvoice.refreshCell(lastRow.recid, "AppliedAmt");
                    }
                }
            }
            //else if (paymentTotal > invoiceTotal) {
            //    var difference = paymentTotal - invoiceTotal;
            //    //if ((paymentTotal * .05) >= difference) {
            //    //    VIS.ADialog.error("AllocationWriteOffWarn");
            //    //}
            //    if (lastRow != null) {
            //        if (lastRow.changes.AppliedAmt % 1 === 0) {

            //        }
            //        else {
            //            lastRow.changes.Writeoff = difference;
            //            lastRow.changes.AppliedAmt = lastRow.changes.AppliedAmt - difference;
            //            $gridInvoice.refreshCell(lastRow.recid, "Writeoff");
            //            $gridInvoice.refreshCell(lastRow.recid, "AppliedAmt");
            //        }
            //    }
            //}


        };

        function bpValueChanged() {
            vetoableChange($vSearchBPartner.getName(), $vSearchBPartner.value);
            //added for gl-allocation
            loadGLDataGrid($vSearchBPartner.event);
        };

        function getMaxDate() {
            _allDates = [];
            _dateAcct = [];
            for (var i = 0; i < $gridPayment.getChanges().length; i++) {
                if ($gridPayment.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridPayment.getChanges()[i].SelectRow == true) {
                        var row = $gridPayment.records[$gridPayment.getChanges()[i].recid].Date1;
                        _allDates.push(new Date(row));
                        var DATEACCT = $gridPayment.records[$gridPayment.getChanges()[i].recid].DATEACCT;
                        _dateAcct.push(new Date(DATEACCT));
                        $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        _dateAcct = [];
                        //console.log(_allDates);
                    }
                }
            }
            for (var i = 0; i < $gridInvoice.getChanges().length; i++) {
                if ($gridInvoice.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridInvoice.getChanges()[i].SelectRow == true) {
                        var row = $gridInvoice.records[$gridInvoice.getChanges()[i].recid].Date1; //changed schedule date to invoice date suggested by Mukesh sir, ravi and amit.
                        _allDates.push(new Date(row));
                        if ($gridPayment.getChanges().length == 0 && $gridCashline.getChanges().length == 0) {
                            var DATEACCT = $gridInvoice.records[$gridInvoice.getChanges()[i].recid].DATEACCT;
                            _dateAcct.push(new Date(DATEACCT));
                            $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        }
                        //console.log(_allDates);
                    }
                }
            }
            for (var i = 0; i < $gridCashline.getChanges().length; i++) {
                if ($gridCashline.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridCashline.getChanges()[i].SelectRow == true) {
                        var row = $gridCashline.records[$gridCashline.getChanges()[i].recid].Created;
                        _allDates.push(new Date(row));
                        // console.log(_allDates);
                        if ($gridPayment.getChanges().length == 0) {
                            var DATEACCT = $gridCashline.records[$gridCashline.getChanges()[i].recid].DATEACCT;
                            _dateAcct.push(new Date(DATEACCT));
                            $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        }
                    }
                }
            }
            maxDate = new Date(Math.max.apply(null, _allDates));
            if (_allDates.length == 0)
                maxDate = new Date();
            $date.val(Globalize.format(new Date(maxDate), "yyyy-MM-dd"));
            console.log("Max Date:- " + maxDate);
        };

        //added for gl-allocation
        function loadGLVoucher() {
            if (_C_BPartner_ID > 0) {
                $.ajax({
                    url: VIS.Application.contextUrl + "VIS/PaymentAllocation/GetGLData",
                    dataType: "json",
                    data: { _C_Currency_ID: _C_Currency_ID, _C_BPartner_ID: _C_BPartner_ID, page: pageNoInvoice, size: PAGESIZE },
                    async: true,
                    success: function (result) {
                        // var data = JSON.parse(result);
                        if (result) {
                            callbackLoadGlLines(result);
                            //isBusyIndicatorHidden();
                        }
                    },
                    error: function (err) {
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });
            }
        };

        function callbackLoadGlLines(data) {
            if (data != null) {
                var result = JSON.parse(data);
                glLineGrid.add(result);
            }
            $bsyDiv[0].style.visibility = "hidden";
        };
        //end

        function getSelectedRecordsCount() {
            _allDates = [];
            isOrgMatched = true;
            for (var i = 0; i < $gridPayment.getChanges().length; i++) {
                if ($gridPayment.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridPayment.getChanges()[i].SelectRow == true) {
                        var row = $gridPayment.records[$gridPayment.getChanges()[i].recid].Date1;
                        // check org matched or not 
                        if (isOrgMatched && parseInt($cmbOrg.val()) != parseInt($gridPayment.records[$gridPayment.getChanges()[i].recid].AD_Org_ID)) {
                            isOrgMatched = false;
                        }
                        _allDates.push(new Date(row));
                        //console.log(_allDates);
                    }
                }
            }
            for (var i = 0; i < $gridInvoice.getChanges().length; i++) {
                if ($gridInvoice.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridInvoice.getChanges()[i].SelectRow == true) {
                        var row = $gridInvoice.records[$gridInvoice.getChanges()[i].recid].Date1; //changed schedule date to invoice date suggested by Mukesh sir, ravi and amit.
                        // check org matched or not 
                        if (isOrgMatched && parseInt($cmbOrg.val()) != parseInt($gridInvoice.records[$gridInvoice.getChanges()[i].recid].AD_Org_ID)) {
                            isOrgMatched = false;
                        }
                        _allDates.push(new Date(row));
                        //console.log(_allDates);
                    }
                }
            }
            for (var i = 0; i < $gridCashline.getChanges().length; i++) {
                if ($gridCashline.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if ($gridCashline.getChanges()[i].SelectRow == true) {
                        var row = $gridCashline.records[$gridCashline.getChanges()[i].recid].Created;
                        // check org matched or not 
                        if (isOrgMatched && parseInt($cmbOrg.val()) != parseInt($gridCashline.records[$gridCashline.getChanges()[i].recid].AD_Org_ID)) {
                            isOrgMatched = false;
                        }
                        _allDates.push(new Date(row));
                        // console.log(_allDates);
                    }
                }
            }
            //added for gl-allocation
            // glLineGrid
            for (var i = 0; i < glLineGrid.getChanges().length; i++) {
                if (glLineGrid.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if (glLineGrid.getChanges()[i].SelectRow == true) {
                        var row = glLineGrid.records[glLineGrid.getChanges()[i].recid - 1].DATEACCT;
                        _allDates.push(new Date(row));
                        // console.log(_allDates);
                    }
                }
            }

            if (_allDates.length == 0)
                return false;
            return true;
        };

        /**
        *  Vetoable Change Listener.
        *  - Business Partner
        *  - Currency
        * 	- Date
        *  @param e event
        */

        function vetoableChange(name, value) {
            //log.Config(name + "=" + value);
            if (value == null) {
                //SetBusy(false);
                return;
            }

            //  BPartner
            if (name == "C_BPartner_ID") {
                _C_BPartner_ID = value;
                loadBPartner();
            }
            //	Currency
            else if (name == "C_Currency_ID") {
                _C_Currency_ID = parseInt(value);
                loadBPartner();
            }
            //	Date for Multi-Currency
            else if (name == "Date" && $vchkMultiCurrency) {
                loadBPartner();
            }
        }

        function calculate() {
            //log.Config("");
            //
            //DecimalFormat format = DisplayType.GetNumberFormat(DisplayType.Amount);

            $bsyDiv[0].style.visibility = "visible";

            var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
            var allocDate = new Date();
            allocDate = null;

            //  Payment******************
            var totalPay = parseFloat(0);
            //int rows = vdgvPayment.ItemsSource.OfType<object>().Count();
            _noPayments = 0;
            var rowsPayment = null;
            var rowsCash = null;
            var rowsInvoice = null;
            //added for gl-allocation
            var rowsGL = null;
            //end
            //Dispatcher.BeginInvoke(delegate
            //{
            //  rowsPayment = $gridPayment.getSelection();
            if ($gridPayment)
                rowsPayment = $gridPayment.getChanges();
            if ($gridCashline)
                rowsCash = $gridCashline.getChanges();
            if ($gridInvoice)
                rowsInvoice = $gridInvoice.getChanges();
            if (glLineGrid)
                rowsGL = glLineGrid.getChanges();


            //});
            if (rowsPayment != null) {
                for (var i = 0; i < rowsPayment.length; i++) {
                    if (rowsPayment[i].SelectRow == true) {
                        var currnetRow = $gridPayment.get(rowsPayment[i].recid);
                        var ts = new Date(currnetRow.Date1);
                        //allocDate = VIS.TimeUtil.max(allocDate, ts);

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);

                        var keys = Object.keys(currnetRow);
                        //var bd = parseFloat(rowsPayment[i][keys[_payment + 1]]);
                        var bd = parseFloat(rowsPayment[i][keys[keys.indexOf("AppliedAmt")]]);
                        totalPay = totalPay + (isNaN(bd) ? 0 : bd);  //  Applied Pay
                        _noPayments++;
                    }
                    //  log.Fine("Payment_" + i + " = " + bd + " - Total=" + totalPay);
                }
            }
            $lblPaymentSum.text(VIS.Msg.getMsg("SelectedPayments") + _noPayments + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalPay.toFixed(stdPrecision)) + " ");

            //  Cash******************
            var totalCash = parseFloat(0);
            //rows = vdgvCashLine.ItemsSource.OfType<object>().Count();
            _noCashLines = 0;
            if (rowsCash != null) {
                for (var i = 0; i < rowsCash.length; i++) {
                    if (rowsCash[i].SelectRow == true) {
                        var currnetRow = $gridCashline.get(rowsCash[i].recid);
                        var ts = new Date(currnetRow.Created);
                        //allocDate = VIS.TimeUtil.max(allocDate, ts);

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);
                        //allocDate = ts;
                        //************************************** Changed

                        var keys = Object.keys(currnetRow);

                        //var bd = parseFloat(rowsCash[i][keys[_paymentCash + 3]]);// Util.GetValueOfDecimal(((BindableObject)rowsCash[i]).GetValue(_payment));
                        var bd = parseFloat(rowsCash[i][keys[keys.indexOf("AppliedAmt")]]);
                        totalCash = totalCash + (isNaN(bd) ? 0 : bd);  //  Applied Pay
                        _noCashLines++;
                        //log.Fine("Payment_" + i + " = " + bd + " - Total=" + totalCash);
                    }
                }
            }
            $lblCashSum.text(VIS.Msg.getMsg("SelectedCashlines") + _noCashLines + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalCash.toFixed(stdPrecision)) + " ");


            //  Invoices******************
            var totalInv = parseFloat(0);
            //rows = vdgvInvoice.ItemsSource.OfType<object>().Count();
            _noInvoices = 0;
            if (rowsInvoice != null) {
                for (var i = 0; i < rowsInvoice.length; i++) {
                    if (rowsInvoice[i].SelectRow == true) {
                        var currnetRow = $gridInvoice.get(rowsInvoice[i].recid);
                        var ts;
                        if (countVA009 > 0) {
                            ts = new Date(currnetRow.InvoiceScheduleDate);
                        }
                        else {
                            ts = new Date(currnetRow.Date1);
                        }

                        //when Invoice Schedule Date is null then ts become Invalid Date
                        if (ts == "Invalid Date") {
                            ts = new Date(currnetRow.Date1);
                        }

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);

                        //allocDate = ts;

                        var keys = Object.keys(currnetRow);
                        var bd;
                        //if (rowsInvoice[i][keys[_applied + 1]] != "") {
                        if (rowsInvoice[i][keys[keys.indexOf("AppliedAmt")]] != "") {
                            //bd = parseFloat(rowsInvoice[i][keys[_applied]]);
                            bd = parseFloat(rowsInvoice[i][keys[keys.indexOf("AppliedAmt")]]);
                        }
                        else {
                            bd = 0;
                        }
                        totalInv = totalInv + (isNaN(bd) ? 0 : bd);  //  Applied Inv
                        _noInvoices++;
                        //log.Fine("Invoice_" + i + " = " + bd + " - Total=" + totalPay);
                    }
                }
            }
            $lblInvoiceSum.text(VIS.Msg.getMsg("SelectedInvoices") + _noInvoices + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalInv.toFixed(stdPrecision)) + " ");

            //added for gl-allocation

            var totalGL = parseFloat(0);
            //rows = vdgvInvoice.ItemsSource.OfType<object>().Count();
            _noGL = 0;
            if (rowsGL != null) {
                for (var i = 0; i < rowsGL.length; i++) {
                    if (rowsGL[i].SelectRow == true) {
                        var currnetRow = glLineGrid.get(rowsGL[i].recid);
                        ts = new Date(currnetRow.Date1);
                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);
                        var keys = Object.keys(currnetRow);
                        var bd;
                        //if (rowsGL[i][keys[_applied + 1]] != "") {
                        if (rowsGL[i][keys[keys.indexOf("AppliedAmt")]] != "") {
                            //bd = parseFloat(rowsGL[i][keys[_applied]]);
                            bd = parseFloat(rowsGL[i][keys[keys.indexOf("AppliedAmt")]]);
                        }
                        else {
                            bd = 0;
                        }
                        totalGL = totalGL + (isNaN(bd) ? 0 : bd);  //  Applied Inv
                        _noGL++;
                        //log.Fine("Invoice_" + i + " = " + bd + " - Total=" + totalPay);
                    }
                }
            }
            $lblglSum.text(VIS.Msg.getMsg("SelectedGL") + _noGL + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalGL.toFixed(stdPrecision)) + " ");

            //end

            //	Set AllocationDate
            // not to update date. now it always shows as current date
            //if (allocDate != null && allocDate != "Invalid Date")
            //    $date.val(Globalize.format(allocDate, "yyyy-MM-dd"));
            //  Set Allocation Currency
            if ($cmbCurrency.children("option").filter(":selected").text() != null) {
                //vlblAllocCurrency.Content = cmbCurrencyPick.GetText(); //.getDisplay());
                $vlblAllocCurrency.text($cmbCurrency.children("option").filter(":selected").text());
            }
            // }
            //  Difference 
            //  Difference --New Logic for Invoice-(cash+payment)-by raghu 18-jan-2011 //  Cash******************
            //var difference = (totalPay + totalCash) - totalInv;
            //var difference = (parseFloat(totalPay.toFixed(stdPrecision)) + parseFloat(totalCash.toFixed(stdPrecision))) - parseFloat(totalInv.toFixed(stdPrecision));
            var difference = (parseFloat(totalPay.toFixed(stdPrecision)) + parseFloat(totalCash.toFixed(stdPrecision))) - (parseFloat(totalInv.toFixed(stdPrecision)) + parseFloat(totalGL.toFixed(stdPrecision)));

            $vtxtDifference.text(format.GetFormatedValue(difference.toFixed(stdPrecision)));
            if (difference == parseFloat(0)) {
                $vbtnAllocate.prop("readonly", false);
                $vbtnAllocate.css({ "pointer-events": "auto", "opacity": "1" });

            }
            else {
                $vbtnAllocate.prop("readonly", true);
                $vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            }


            $bsyDiv[0].style.visibility = "hidden";
        };

        // when we change or load business partner, refresh label
        function refreshLabel() {
            var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
            var totalPay = parseFloat(0);
            $lblPaymentSum.text(VIS.Msg.getMsg("SelectedPayments") + 0 + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalPay) + " ");
            $lblCashSum.text(VIS.Msg.getMsg("SelectedCashlines") + 0 + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalPay) + " ");
            $lblInvoiceSum.text(VIS.Msg.getMsg("SelectedInvoices") + 0 + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalPay) + " ");
            $vtxtDifference.text(format.GetFormatedValue(0));
            //Set False to select all 
            $invSelectAll.prop('checked', false);
            $paymentSelctAll.prop('checked', false);
            $cashSelctAll.prop('checked', false);
            //to expand the height of all grids
            $('.vis-allocation-payment-grid, .vis-allocation-cashLine-grid, .vis-allocation-invoice-grid, .vis-allocation-gl-grid').css('height', '85%');
            $('.vis-allocation-leftControls').prop('style', ' margin-bottom: 5px');
            $('#cashMaindiv').css('display', 'none');
            $('#glMaindiv').css('display', 'none');
            if ($vchkMultiCurrency.is(':checked'))
                $conversionDiv.css('display', 'block');
            else
                $conversionDiv.css('display', 'none');
        };

        function vchkapplocationChange() {
            var rowsPayment;
            var rowsCash;
            if ($gridPayment != undefined)
                rowsPayment = $gridPayment.getChanges();
            if ($gridCashline != undefined)
                rowsCash = $gridCashline.getChanges();
            //Set False to select all 
            $paymentSelctAll.prop('checked', false);
            $cashSelctAll.prop('checked', false);
            //added for gl-allocation
            $vchkGlInvoice.prop('checked', false);
            //end

            if ($vchkAllocation.is(':checked')) {


                readOnlyCash = false;
                readOnlyPayment = true;
                if (rowsPayment != null) {
                    for (var i = 0; i < rowsPayment.length; i++) {
                        var boolValue = getBoolValue($gridPayment.getChanges(), rowsPayment[i].recid);
                        if (boolValue) {
                            var changes = $gridPayment.get(rowsPayment[i].recid).changes;
                            if (changes != null && changes != undefined) {
                                changes.SelectRow = false;
                                changes.AppliedAmt = 0;
                                $gridPayment.refreshCell(rowsPayment[i].recid, "SelectRow");
                                $gridPayment.refreshCell(rowsPayment[i].recid, "AppliedAmt");
                            }
                        }
                    }
                    if ($gridPayment)
                        $gridPayment.refresh();
                }
                //---Display Cash Grid and hide Payment Grid and refresh cash grid-----Neha
                $row2.css('display', 'none');
                $row3.css('display', '');
                if ($gridCashline)
                    $gridCashline.refresh();
                //------------------------------
            }
            else {
                readOnlyCash = true;
                readOnlyPayment = false;
                if (rowsCash != null) {
                    for (var i = 0; i < rowsCash.length; i++) {
                        var boolValue = getBoolValue($gridCashline.getChanges(), rowsCash[i].recid);
                        if (boolValue) {
                            var changes = $gridCashline.get(rowsCash[i].recid).changes;
                            if (changes != null && changes != undefined) {
                                changes.SelectRow = false;
                                changes.AppliedAmt = 0;
                                $gridCashline.refreshCell(rowsCash[i].recid, "SelectRow");
                                $gridCashline.refreshCell(rowsCash[i].recid, "AppliedAmt");
                            }
                        }
                    }
                    if ($gridCashline)
                        $gridCashline.refresh();
                }
                //---Display Payment Grid and hide Cash Grid and refresh Payment grid-----Neha
                $row3.css('display', 'none');
                $row2.css('display', '');
                if ($gridPayment)
                    $gridPayment.refresh();
            }
            calculate();

        };

        function allocate() {
            var canContinue = true;
            if (invoiceTotal > paymentTotal) {
                var difference = invoiceTotal - paymentTotal;
                if ((invoiceTotal * .05) <= difference) {
                    canContinue = false;
                    VIS.ADialog.confirm("AllocationWriteOffWarn", true, "", "Confirm", function (result) {
                        if (result) {
                            allocateNow();
                        }
                    });
                }
            }
            else if (paymentTotal > invoiceTotal) {
                var difference = paymentTotal - invoiceTotal;
                if ((paymentTotal * .05) >= difference) {
                    canContinue = VIS.ADialog.error("AllocationWriteOffWarn");
                }
            }
            if (canContinue) {
                allocateNow();
            }

        };

        function allocateNow() {
            $bsyDiv[0].style.visibility = "visible";
            var rowsPayment = $gridPayment.getChanges();
            var rowsCash = $gridCashline.getChanges();
            var rowsInvoice = $gridInvoice.getChanges();
            //added for gl-allocation
            var rowsGLVoucher = glLineGrid.getChanges();
            var DateTrx = $date.val();
            var DateAcct = $dateAcct.val();
            //added for gl-allocation
            //    if ($vchkGlVoucher.is(':checked')) {
            if ($allocationFrom.val() == "G" || $allocationTo.val() == "G") {
                //   needs to check in case of Party JV allocation weather you are allocating payment with JV Or you are allocating CashLine with JV.
                glData(rowsPayment, rowsInvoice, rowsCash, rowsGLVoucher, DateTrx, DateAcct);
            }
            else if ($vchkAllocation.is(':checked')) {
                saveCashData(rowsPayment, rowsCash, rowsInvoice, DateTrx, DateAcct);
            }
            else {
                savePaymentData(rowsPayment, rowsCash, rowsInvoice, DateTrx, DateAcct);
            }
        };

        function saveCashData(rowsPayment, rowsCash, rowsInvoice, DateTrx, DateAcct) {

            if (_noInvoices + _noCashLines == 0)
                return "";

            // check period is open or not
            // also check is Non Business Day?
            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/CheckPeriodState",
                data: { DateTrx: $date.val() },
                async: false,
                success: function (result) {
                    if (result != "") {
                        //alert(result);
                        VIS.ADialog.info("", true, result, "");
                        loadBPartner();
                        $bsyDiv[0].style.visibility = "hidden";
                        return "";
                    }
                    else {
                        var payment = "";
                        var applied = "";
                        var discount = "";
                        var writeOff = "";
                        var open = "";
                        var C_CurrencyType_ID = 0;

                        var paymentData = [];
                        var cashData = [];
                        var invoiceData = [];
                        for (var i = 0; i < rowsPayment.length; i++) {
                            var row = $gridPayment.get(rowsPayment[i].recid);
                            C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                            paymentData.push({
                                AppliedAmt: rowsPayment[i].AppliedAmt, Date: row.Date1, Converted: row.ConvertedAmount, CpaymentID: row.CpaymentID, Documentno: row.Documentno, Isocode: row.Isocode,
                                Multiplierap: row.Multiplierap, OpenAmt: row.OpenAmt, Payment: row.Payment, Org: parseInt($cmbOrg.val())
                            });
                        }
                        if (rowsCash.length > 0) {
                            var keys = Object.keys($gridCashline.get(0));
                            // is uesd to pick applied amount
                            //payment = keys[_paymentCash + 1];
                            payment = keys[keys.indexOf("AppliedAmt")];
                            for (var i = 0; i < rowsCash.length; i++) {
                                var row = $gridCashline.get(rowsCash[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                cashData.push({
                                    AppliedAmt: rowsCash[i].AppliedAmt, Date: row.Created, Amount: row.Amount, ccashlineid: row.CcashlineiID, Converted: row.ConvertedAmount, Isocode: row.Isocode,
                                    Multiplierap: row.Multiplierap, OpenAmt: row.OpenAmt, ReceiptNo: row.ReceiptNo, Org: parseInt($cmbOrg.val())
                                });
                            }
                        }

                        if (rowsInvoice.length > 0) {
                            var keys = Object.keys($gridInvoice.get(0));
                            //applied = keys[_applied];
                            applied = keys[keys.indexOf("AppliedAmt")];
                            //discount = keys[_discount];
                            discount = keys[keys.indexOf("Discount")];
                            //writeOff = keys[_writeOff];
                            writeOff = keys[keys.indexOf("Writeoff")];
                            //open = keys[_openInv];
                            open = keys[keys.indexOf("Amount")];

                            for (var i = 0; i < rowsInvoice.length; i++) {
                                var row = $gridInvoice.get(rowsInvoice[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                var discounts = rowsInvoice[i].Discount;
                                if (discounts == undefined) {
                                    discounts = row.Discount;
                                }

                                var appliedamts = rowsInvoice[i].AppliedAmt;
                                if (appliedamts == undefined) {
                                    appliedamts = row.AppliedAmt;
                                }
                                var writeoffs = rowsInvoice[i].Writeoff;
                                if (writeoffs == undefined) {
                                    writeoffs = row.Writeoff;
                                }
                                if (countVA009 <= 0) {
                                    invoiceData.push({
                                        AppliedAmt: appliedamts, Discount: discounts, Writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, Converted: row.Converted, Currency: row.Currency,
                                        Date: row.Date1, Docbasetype: row.DocBaseType,
                                        documentno: row.Documentno, Isocode: row.Isocode, Multiplierap: row.Multiplierap, Amount: row.Amount, Org: parseInt($cmbOrg.val())
                                    });
                                }
                                else {
                                    invoiceData.push({
                                        AppliedAmt: appliedamts, Discount: discounts, Writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, Converted: row.Converted, Currency: row.Currency,
                                        //Date: row.Date1, Docbasetype: row.DocBaseType,
                                        // send invoice schedule date if va009 module is updated
                                        Date: row.InvoiceScheduleDate, Docbasetype: row.DocBaseType,
                                        documentno: row.Documentno, Isocode: row.Isocode, Multiplierap: row.Multiplierap, Amount: row.Amount,
                                        c_invoicepayschedule_id: row.C_InvoicePaySchedule_ID, Org: parseInt($cmbOrg.val())
                                    });
                                }
                            }
                        }


                        $.ajax({
                            url: VIS.Application.contextUrl + "PaymentAllocation/SaveCashData",
                            type: 'POST',
                            data: ({
                                paymentData: JSON.stringify(paymentData), cashData: JSON.stringify(cashData), invoiceData: JSON.stringify(invoiceData), currency: $cmbCurrency.val(),
                                isCash: $vchkAllocation.is(':checked'), _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
                                , discount: discount, writeOff: writeOff, open: open, DateAcct: DateAcct, _CurrencyType_ID: C_CurrencyType_ID, isInterBPartner: false
                            }),
                            success: function (result) {
                                // Clear Selected invoices array when we de-select the select all checkbox. work done for to hold all the selected invoices
                                selectedInvoices = [];
                                VIS.ADialog.info("", true, result, "");
                                loadBPartner();
                                $bsyDiv[0].style.visibility = "hidden";
                            },
                            error: function (result) {
                                VIS.ADialog.info("", true, result, "");
                                $bsyDiv[0].style.visibility = "hidden";
                            }
                        });

                    }
                },
                error: function (result) {
                    VIS.ADialog.info("", true, result, "");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        function savePaymentData(rowsPayment, rowsCash, rowsInvoice, DateTrx, DateAcct) {

            if (_noInvoices + _payment == 0)
                return "";

            // check period is open or not
            // also check is Non Business Day?
            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/CheckPeriodState",
                data: { DateTrx: $date.val() },
                async: false,
                success: function (result) {
                    if (result != "") {
                        VIS.ADialog.info("", true, result, "");
                        //alert(result);
                        loadBPartner();
                        $bsyDiv[0].style.visibility = "hidden";
                        return "";
                    }
                    else {
                        var payment = "";
                        var applied = "";
                        var discount = "";
                        var writeOff = "";
                        var open = "";
                        var C_CurrencyType_ID = 0;

                        var paymentData = [];
                        var cashData = [];
                        var invoiceData = [];
                        for (var i = 0; i < rowsPayment.length; i++) {
                            var row = $gridPayment.get(rowsPayment[i].recid);
                            var keys = Object.keys($gridPayment.get(0));
                            C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                            payment = keys[10];
                            paymentData.push({
                                appliedamt: rowsPayment[i].AppliedAmt, date: row.Date1, converted: row.ConvertedAmount, cpaymentid: row.CpaymentID, documentno: row.Documentno, isocode: row.Isocode,
                                multiplierap: row.Multiplierap, openamt: row.OpenAmt, payment: row.Payment, Org: parseInt($cmbOrg.val())
                            });
                        }
                        if (rowsCash.length > 0) {
                            var keys = Object.keys($gridCashline.get(0));
                            for (var i = 0; i < rowsCash.length; i++) {
                                var row = $gridCashline.get(rowsCash[i].recid);

                                cashData.push({
                                    appliedamt: rowsCash[i].AppliedAmt, date: row.Created, amount: row.Amount, ccashlineid: row.CcashlineiID, converted: row.ConvertedAmount, isocode: row.Isocode,
                                    multiplierap: row.Multiplierap, openamt: row.OpenAmt, receiptno: row.ReceiptNo, Org: parseInt($cmbOrg.val())
                                });
                            }
                        }

                        if (rowsInvoice.length > 0) {
                            var keys = Object.keys($gridInvoice.get(0));
                            //applied = keys[_applied];
                            applied = keys[keys.indexOf("AppliedAmt")];
                            //payment = keys[_applied];
                            payment = keys[keys.indexOf("AppliedAmt")];

                            //discount = keys[_discount];
                            discount = keys[keys.indexOf("Discount")];
                            //writeOff = keys[_writeOff];
                            writeOff = keys[keys.indexOf("Writeoff")];
                            //open = keys[_openInv];
                            open = keys[keys.indexOf("Amount")];

                            for (var i = 0; i < rowsInvoice.length; i++) {
                                var row = $gridInvoice.get(rowsInvoice[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                var discounts = rowsInvoice[i].Discount;
                                if (discounts == undefined) {
                                    discounts = row.Discount;
                                }

                                var appliedamts = rowsInvoice[i].AppliedAmt;
                                if (appliedamts == undefined) {
                                    appliedamts = row.AppliedAmt;
                                }
                                var writeoffs = rowsInvoice[i].Writeoff;
                                if (writeoffs == undefined) {
                                    writeoffs = row.Writeoff;
                                }

                                if (countVA009 <= 0) {
                                    invoiceData.push({
                                        appliedamt: appliedamts, discount: discounts, writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, converted: row.Converted, currency: row.Currency,
                                        date: row.Date1, docbasetype: row.DocBaseType,
                                        documentno: row.Documentno, isocode: row.Isocode, multiplierap: row.Multiplierap, amount: row.Amount, Org: parseInt($cmbOrg.val())
                                    });
                                }
                                else {
                                    invoiceData.push({
                                        appliedamt: appliedamts, discount: discounts, writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, converted: row.Converted, currency: row.Currency,
                                        //date: row.Date1, docbasetype: row.DocBaseType,
                                        // send invoice schedule date if va009 module is updated
                                        date: row.InvoiceScheduleDate, docbasetype: row.DocBaseType,
                                        DocumentNo: row.Documentno, Isocode: row.Isocode, Multiplierap: row.Multiplierap, Amount: row.Amount,
                                        c_invoicepayschedule_id: row.C_InvoicePaySchedule_ID, Org: parseInt($cmbOrg.val())
                                    });
                                }
                            }
                        }


                        $.ajax({
                            url: VIS.Application.contextUrl + "PaymentAllocation/SavePaymentData",
                            type: 'POST',
                            data: ({
                                paymentData: JSON.stringify(paymentData), cashData: JSON.stringify(cashData), invoiceData: JSON.stringify(invoiceData), currency: $cmbCurrency.val(),
                                isCash: $vchkAllocation.is(':checked'), _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
                                , discount: discount, writeOff: writeOff, open: open, DateAcct: DateAcct, _CurrencyType_ID: C_CurrencyType_ID, isInterBPartner: false
                            }),
                            success: function (result) {
                                // Clear Selected invoices array when we de-select the select all checkbox. work done for to hold all the selected invoices
                                selectedInvoices = [];
                                loadBPartner();
                                VIS.ADialog.info("", true, result, "");
                                $bsyDiv[0].style.visibility = "hidden";
                            },
                            error: function () {
                                VIS.ADialog.info("", true, result, "");
                                $bsyDiv[0].style.visibility = "hidden";
                            }
                        });
                    }
                },
                error: function (result) {
                    VIS.ADialog.info("", true, result, "");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        //added for gl-allocation
        function glData(rowsPayment, rowsInvoice, rowsCash, rowsGLVoucher, DateTrx, DateAcct) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/PaymentAllocation/CheckPeriodState",
                data: { DateTrx: $date.val() },
                async: false,
                success: function (result) {
                    if (result != "") {
                        var e;
                        VIS.ADialog.info("", true, result, "");
                        loadBPartner();
                        loadGLDataGrid(e);
                        $bsyDiv[0].style.visibility = "hidden";
                        return "";
                    }
                    else {
                        $bsyDiv[0].style.visibility = "visible";
                        var payment = "";
                        var applied = "";
                        var discount = "";
                        var writeOff = "";
                        var open = "";
                        var C_CurrencyType_ID = 0;
                        var paymentData = [];
                        var cashData = [];
                        var glData = [];
                        var invoiceData = [];

                        //Payment Data
                        for (var i = 0; i < rowsPayment.length; i++) {
                            var row = $gridPayment.get(rowsPayment[i].recid);
                            C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                            paymentData.push({
                                AppliedAmt: rowsPayment[i].AppliedAmt, Date: row.Date1, Converted: row.ConvertedAmount, CpaymentID: row.CpaymentID, Documentno: row.Documentno, Isocode: row.Isocode,
                                Multiplierap: row.Multiplierap, OpenAmt: row.OpenAmt, Payment: row.Payment, Org: parseInt($cmbOrg.val()), IsPaid: false, paidAmt: 0
                            });
                        }

                        //Cash Data
                        if (rowsCash.length > 0) {
                            var keys = Object.keys($gridCashline.get(0));
                            payment = keys[keys.indexOf("AppliedAmt")];
                            for (var i = 0; i < rowsCash.length; i++) {
                                var row = $gridCashline.get(rowsCash[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                cashData.push({
                                    AppliedAmt: rowsCash[i].AppliedAmt, Date: row.Created, Amount: row.Amount, ccashlineid: row.CcashlineiID, Converted: row.ConvertedAmount, Isocode: row.Isocode,
                                    Multiplierap: row.Multiplierap, OpenAmt: row.OpenAmt, ReceiptNo: row.ReceiptNo, Org: parseInt($cmbOrg.val()), IsPaid: false, paidAmt: 0
                                });
                            }
                        }

                        if (rowsInvoice.length > 0) {
                            var keys = Object.keys($gridInvoice.get(0));
                            //applied = keys[_applied];
                            applied = keys[keys.indexOf("AppliedAmt")];
                            //discount = keys[_discount];
                            discount = keys[keys.indexOf("Discount")];
                            //writeOff = keys[_writeOff];
                            writeOff = keys[keys.indexOf("Writeoff")];
                            //open = keys[_openInv];
                            open = keys[keys.indexOf("Amount")];

                            for (var i = 0; i < rowsInvoice.length; i++) {
                                var row = $gridInvoice.get(rowsInvoice[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                var discounts = rowsInvoice[i].Discount;
                                if (discounts == undefined) {
                                    discounts = row.Discount;
                                }

                                var appliedamts = rowsInvoice[i].AppliedAmt;
                                if (appliedamts == undefined) {
                                    appliedamts = row.AppliedAmt;
                                }
                                var writeoffs = rowsInvoice[i].Writeoff;
                                if (writeoffs == undefined) {
                                    writeoffs = row.Writeoff;
                                }
                                if (countVA009 <= 0) {
                                    invoiceData.push({
                                        AppliedAmt: appliedamts, Discount: discounts, Writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, Converted: row.Converted, Currency: row.Currency,
                                        Date: row.Date1, Docbasetype: row.DocBaseType,
                                        documentno: row.Documentno, Isocode: row.Isocode, Multiplierap: row.Multiplierap, Amount: row.Amount, Org: parseInt($cmbOrg.val()), IsPaid: false, paidAmt: 0
                                    });
                                }
                                else {
                                    invoiceData.push({
                                        AppliedAmt: appliedamts, Discount: discounts, Writeoff: writeoffs,
                                        cinvoiceid: row.CinvoiceID, Converted: row.Converted, Currency: row.Currency,
                                        //Date: row.Date1, Docbasetype: row.DocBaseType,
                                        // send invoice schedule date if va009 module is updated
                                        Date: row.InvoiceScheduleDate, Docbasetype: row.DocBaseType,
                                        documentno: row.Documentno, Isocode: row.Isocode, Multiplierap: row.Multiplierap, Amount: row.Amount,
                                        c_invoicepayschedule_id: row.C_InvoicePaySchedule_ID, Org: parseInt($cmbOrg.val()), IsPaid: false, paidAmt: 0
                                    });
                                }
                            }
                        }

                        if (rowsGLVoucher.length > 0) {

                            for (var i = 0; i < rowsGLVoucher.length; i++) {
                                var row = glLineGrid.get(rowsGLVoucher[i].recid);
                                C_CurrencyType_ID = parseInt(row.C_ConversionType_ID);
                                glData.push({
                                    AppliedAmt: rowsGLVoucher[i].AppliedAmt, Date: row.DATEACCT, DateDoc: row.DATEDOC, DocumentNo: row.DOCUMENTNO,
                                    OpenAmount: row.OpenAmount, GL_JournalLine_ID: row.GL_JOURNALLINE_ID, GL_Journal_ID: row.GL_Journal_ID,
                                    ConvertedAmount: row.ConvertedAmount, Org: parseInt($cmbOrg.val()), IsPaid: false, paidAmt: 0
                                });
                            }
                        }
                        saveGLData(JSON.stringify(paymentData), JSON.stringify(invoiceData), JSON.stringify(cashData), JSON.stringify(glData), $date.val(), C_CurrencyType_ID);
                    }
                },
                error: function (result) {
                    VIS.ADialog.info("", true, result, "");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        function saveGLData(rowsPayment, rowsInvoice, rowsCash, rowsGLVoucher, DateTrx, C_CurrencyType_ID) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/PaymentAllocation/saveGLJData",
                type: 'POST',
                data: { paymentData: rowsPayment, invoiceData: rowsInvoice, cashData: rowsCash, glData: rowsGLVoucher, DateTrx: DateTrx, _windowNo: self.windowNo, C_Currency_ID: _C_Currency_ID, C_BPartner_ID: _C_BPartner_ID, AD_Org_ID: $cmbOrg.val(), C_CurrencyType_ID: C_CurrencyType_ID },
                async: false,
                success: function (result) {
                    var e;
                    VIS.ADialog.info("", true, result, "");
                    loadBPartner();
                    loadGLDataGrid(e);
                    if ($gridCashline)
                        $gridCashline.refresh();
                    if (glLineGrid)
                        glLineGrid.refresh();
                    if ($gridInvoice)
                        $gridInvoice.refresh();
                    if ($gridPayment)
                        $gridPayment.refresh();
                    var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
                    $lblglSum.text(VIS.Msg.getMsg("SelectedGL") + 0 + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(0) + " ");
                    $vtxtDifference.text(format.GetFormatedValue(0));
                    $vchkGlVoucher.trigger("click");
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (result) {
                    VIS.ADialog.info("", true, result, "");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };
        //end

        this.disposeComponent = function () {
            if ($gridPayment != undefined && $gridPayment != null) {
                $gridPayment.destroy();
                $gridPayment = null;
            }

            if ($gridCashline != undefined && $gridCashline != null) {
                $gridCashline.destroy();
                $gridCashline = null;
            }

            if ($gridInvoice != undefined && $gridInvoice != null) {
                $gridInvoice.destroy();
                $gridInvoice = null;
            }
            $invSelectAll = null;
            $paymentSelctAll = null;
            $cashSelctAll = null;
            _allDates = null;
        };

        this.getRoot = function () {
            return $root;
        };
    };

    VAllocation.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
    };

    VAllocation.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps = VIS.Apps || {};
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    VIS.Apps.AForms.VAllocation = VAllocation;

})(VIS, jQuery)