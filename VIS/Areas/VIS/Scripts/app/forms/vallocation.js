; (function (VIS, $) {

    function VAllocation() {
        this.frame = null;
        this.windowNo = 0;
        var ctx = VIS.Env.getCtx();
        var $self = this;
        var $root = $('<div class="vis-allocate-root vis-forms-container">');
        var $row1 = $('<div class="vis-leftpanel-wrapper vis-pad-0">');
        //set height of filters div and Process Button div
        var $innerRow = $('<div class="vis-leftpanel-inn-wrap">');
        var rowContiner = $('<div class="vis-allocation-rightContainer">');
        var $row2 = $('<div class="vis-allocate-paymentdiv" >');
        var $row3 = $('<div class="vis-allocate-cashdiv" >');
        var $row4 = $('<div class="vis-allocate-invoicediv" >');
        //added GL Grid for GL Allocation
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

        //Declared variables for gl-allocation
        var $divGl = null;
        var $allocationFrom = $('<select class="vis-allocation-currencycmb" ></select>');
        var $allocationTo = $('<select class="vis-allocation-currencycmb" ></select>');
        var $vchkGlVoucher = null;
        var $vchkGlInvoice = null;
        // var $divChkGLInvoice = null;
        var $lblglSum = null;
        var $glSelectAll = null;
        var _noGL = 0;
        var glLineGrid;
        var $DivGllineGrid;
        var readOnlyGL = true;
        var glTotal = 0;
        var colGlCheck = false;
        //End

        var $gridPayment = null;
        var $gridCashline = null;
        var $gridInvoice = null;

        //Added for Select All Functionality by Manjot assigned by Mukesh sir and Savita
        var $invSelectAll = null;
        var $paymentSelctAll = null;
        var $cashSelctAll = null;
        var maxDate = null;

        var $vchkMultiCurrency = null;
        var $vchkAllocation = null;

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

        var _noInvoices = 0;
        var _noPayments = 0;
        var _noCashLines = 0;

        // array for selected invoices
        var selectedInvoices = [];
        var totalselectedinv = 0;
        var $clrbtn = null;

        var colInvCheck = false;
        var colPayCheck = false;
        var colCashCheck = false;

        var $vtxtDifference = null;
        var $vbtnAllocate = null;
        //added new search button for search in left panel
        var $vbtnSearch = null;
        var $vlblAllocCurrency = null;
        //var $vchkAutoWriteOff = null;

        var selection = null;
        var $bsyDiv = null;
        var self = this;

        var readOnlyCash = true;
        var readOnlyPayment = false;

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
        //Culture seperator object
        var culture = new VIS.CultureSeparator();
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
            "C_Cash_ID",
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
            var $divProcess = $('<div style="padding-right: 15px;float: right; margin-top: 10px;">');
            $divProcess.append(' <a class="vis-group-btn vis-group-create vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg('Process') + '</a>');
            $row1.append($divProcess);
            var $divSearch = $('<div style="padding-right: 15px;float: right; margin-top: 10px;">');
            $divSearch.append(' <a class="vis-group-create vis-group-grayBtn" id="vis_srchbtn_' + $self.windowNo + '" style="float: left;">' + VIS.Msg.getMsg('Search') + '</a>');
            $row1.append($divSearch);
            $root.append($row1.append($innerRow).append($divProcess).append($divSearch)).append(rowContiner);
            $vbtnAllocate = $root.find('.vis-group-btn');
            $vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            $vbtnSearch = $root.find('#vis_srchbtn_' + $self.windowNo + '');
            createBusyIndicator();
            eventHandling();
            //added to load all blank grids
            blankAllGrids();
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
                //Commetd code because now we want to search data on search button not on every control's event
                //////loadBPartner();
                //loadCurrencyPrecision(parseInt($cmbCurrency.val()));
            });
            $vchkMultiCurrency.on("change", function (e) {
                vetoableChange("Date", $vchkMultiCurrency.is(':checked'));
                //Commetd code because now we want to search data on search button not on every control's event
                //////loadBPartner();
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

            $vbtnSearch.on("click", function (e) {
                if (VIS.Utility.Util.getValueOfInt($vSearchBPartner.value) > 0) {
                    searchbyParameters();
                }
                else {
                    VIS.ADialog.info("", true, VIS.Msg.getMsg("SelectBusinessPartnerFirst"), "");
                }
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
                //////loadGLDataGrid(e);
            });
            $vchkGlInvoice.on("change", function (e) {
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
            //hide the AutoWriteOff Checkbox 
            //$vchkAutoWriteOff.on("click", function (e) {
            //    autoWriteOff();
            //    calculate();
            //});
            //-----Controls events and load Invoice Grid--Neha
            $txtDocNo.on("change", function (e) {
                //Commetd code because now we want to search data on search button not on every control's event
                //////loadInvoice();
            });
            $cmbDocType.on("change", function (e) {
                //Commetd code because now we want to search data on search button not on every control's event
                //////loadInvoice();
            });
            $fromDate.on("change", function (e) {
                //Commetd code because now we want to search data on search button not on every control's event
                ////////loadInvoice();
            });
            $toDate.on("change", function (e) {
                //Commetd code because now we want to search data on search button not on every control's event
                ////////loadInvoice();
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
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
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
                //Commetd code because now we want to search data on search button not on every control's event
                //////loadBPartner();
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

            //allocation From combo event
            $allocationFrom.on("change", function (e) {

                if ($allocationFrom.val() == 0) {
                    $allocationFrom.css("background-color", SetMandatory(true));
                }
                else
                    $allocationFrom.css("background-color", SetMandatory(false));
                //if allocation from is change then we have to clear the selection of invoices
                clearInvoiceArrays(e);

                if ($allocationFrom.val() == "P") { // In case of Payment Cash Option must be Hide
                    $allocationTo.find("option[value=C]").hide();
                    $allocationTo.find("option[value=P]").show();
                    $allocationTo.find("option[value=G]").show();
                }
                else if ($allocationFrom.val() == "C") { // In case of Cash Payment Option must be Hide
                    $allocationTo.find("option[value=P]").hide();
                    $allocationTo.find("option[value=C]").hide();
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
                //if allocation from is change then we have to clear the selection of invoices
                clearInvoiceArrays(e);

                var allocfrm = $allocationFrom.val();
                var allocto = $allocationTo.val();
                loadGrids($allocationTo.val());
                displayGrids(allocfrm, allocto)
            });
        };

        //to load GL Grid and define events of Gl Grid
        function loadGLGrid() {
            if (glLineGrid != undefined && glLineGrid != null) {
                glLineGrid.destroy();
                glLineGrid = null;
            }
            glLineGrid = $divGl.w2grid({
                name: "VIS_GLLineGrid_" + $self.windowNo,
                show: { toolbar: true },
                multiSelect: true,
                columns: [
                    { field: "SelectRow", caption: 'check', size: '40px', editable: { type: 'checkbox' } },
                    { field: "DATEDOC", caption: VIS.translatedTexts.Date, render: 'date:yyyy-mm-dd', size: '80px', hidden: false },
                    { field: "DOCUMENTNO", caption: VIS.translatedTexts.DocumentNo, size: '120px', hidden: false },
                    { field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false },
                    { field: "ConvertedAmount", caption: VIS.Msg.getMsg("Amount"), size: '150px', hidden: false },
                    { field: "OpenAmount", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false },
                    { field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false },
                    { field: "GL_JOURNALLINE_ID", caption: VIS.translatedTexts.GL_JOURNALLINE_ID, size: '150px', hidden: true },
                    { field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true },
                    { field: "GL_Journal_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true },
                    { field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, render: 'date:yyyy-mm-dd', size: '85px', hidden: false }
                ],
                onClick: function (event) {
                    //paymentCellClicked(event);
                    getMaxDate();
                    // when we select a record, check conversion type is same or not.
                    // if not then not to select this record
                    var getChanges = glLineGrid.getChanges();
                    if (getChanges == undefined || getChanges.length == 0) {
                        return;
                    }
                    var element = $.grep(getChanges, function (ele, index) {
                        return parseInt(ele.recid) == parseInt(event.recid);
                    });
                    var colIndex = glLineGrid.getColumn('AppliedAmt', true);
                    if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                        glLineGrid.refreshCell(0, "AppliedAmt");
                    }
                    else {
                        if (element[0].SelectRow == true) {
                            if (C_ConversionType_ID == 0) {
                                C_ConversionType_ID = glLineGrid.get(event.recid).C_ConversionType_ID;
                            }
                            else if (C_ConversionType_ID != glLineGrid.get(event.recid).C_ConversionType_ID) {

                                var $x = document.getElementsByName(event.target);
                                if ($x.length > 0)
                                    $x = $x[0];

                                var $z = $($x).find("#grid_openformatgrid_rec_" + event.recid + "");

                                if ($z.length > 0)
                                    $z = $z[0];
                                if ($($z).find("input:checked").prop('checked')) {
                                    $($z).find("input:checked").prop('checked', false);
                                }
                                glLineGrid.get(event.recid).changes = false;
                                glLineGrid.unselect(event.recid);
                                glLineGrid.columns[colIndex].editable = false;
                                glLineGrid.get(event.recid).changes.AppliedAmt = "0";
                                glLineGrid.refreshCell(event.recid, "AppliedAmt");
                                VIS.ADialog.warn(("VIS_ConversionNotMatched"));
                            }
                        }
                        else {
                            glLineGrid.unselect(event.recid);
                            glLineGrid.columns[colIndex].editable = false;
                            glLineGrid.get(event.recid).changes.AppliedAmt
                            glLineGrid.refreshCell(event.recid, "AppliedAmt");
                        }
                    }
                    glTableChanged(event.recid, event.column);
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
        //to load Data in GL Grid
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
                //$divChkGLInvoice.css('display', 'none');
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
        //to load all grids 
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
        // to display grids based on selected allocation From and TO
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
                if ($gridPayment) {
                    $gridPayment.clear();
                }
                //////loadUnallocatedPayments();
            }
            else if (allocFrm == "C") {
                readOnlyCash = false;
                $row3.css('display', 'block');// Cash Grid
                if ($gridCashline) {
                    $gridCashline.clear();
                }
                //////loadUnallocatedCashLines();
            }
            else if (allocFrm == "I") {
                $row4.css('display', 'block'); // Invoice Grid
                if ($gridInvoice) {
                    $gridInvoice.clear();
                }
                //////if ($gridInvoice) {
                //////    loadInvoice();
                //////}
            }
            else if (allocFrm == "G") {
                $row5.css('display', 'block');// GL journal grid
                //$bsyDiv[0].style.visibility = "visible";
                readOnlyGL = false;
                if (glLineGrid) {
                    glLineGrid.clear();
                }
                //////loadGLVoucher();
            }

            if (allocTo == "P") {
                readOnlyPayment = false;
                $row2.css('display', 'block'); // Payment Grid
                if ($gridPayment) {
                    $gridPayment.clear();
                }
                //////loadUnallocatedPayments();
            }
            else if (allocTo == "C") {
                readOnlyCash = false;
                $row3.css('display', 'block');// Cash Grid
                if ($gridCashline) {
                    $gridCashline.clear();
                }
                //////loadUnallocatedCashLines();
            }
            else if (allocTo == "I") {
                $row4.css('display', 'block'); // Invoice Grid
                if ($gridInvoice) {
                    $gridInvoice.clear();
                }
                ////////if ($gridInvoice) {
                ////////    loadInvoice();
                ////////}
            }
            else if (allocTo == "G") {
                $row5.css('display', 'block');// GL journal grid
                //$bsyDiv[0].style.visibility = "visible";
                readOnlyGL = false;
                if (glLineGrid) {
                    glLineGrid.clear();
                }
                ////////loadGLVoucher();
            }
        };
        //function to clear the selection of invoices
        function clearInvoiceArrays(e) {
            if ($gridInvoice != null) {
                var chk = $('#grid_' + $gridInvoice.name + '_records td[col="0"]').find('input[type="checkbox"]');
                for (var i = 0; i < chk.length; i++) {
                    $(chk[i]).prop('checked', false);
                    //$(chk[i]).change(e);
                    $gridInvoice.editChange.call($gridInvoice, chk[i], i, 0, e);
                    var eData = { "type": "click", "phase": "before", "target": "grid", "recid": i, "index": i, "isStopped": false, "isCan//celled": false, "onComplete": null };
                    $gridInvoice.trigger(eData);
                }
                selectedInvoices = [];
            }
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
            $allocationFrom.append('<option value="C">' + VIS.translatedTexts.C_Cash_ID + '</option>');
            $allocationTo.append('<option value="C">' + VIS.translatedTexts.C_Cash_ID + '</option>');
            $allocationFrom.append('<option value="I">' + VIS.translatedTexts.C_Invoice_ID + '</option>');
            $allocationTo.append('<option value="I">' + VIS.translatedTexts.C_Invoice_ID + '</option>');
            $allocationFrom.append('<option value="G">' + VIS.translatedTexts.GL_Journal_ID + '</option>');
            $allocationTo.append('<option value="G">' + VIS.translatedTexts.GL_Journal_ID + '</option>');
            $allocationFrom.append('<option value="P">' + VIS.translatedTexts.C_Payment_ID + '</option>');
            $allocationTo.append('<option value="P">' + VIS.translatedTexts.C_Payment_ID + '</option>');
        };

        function createRow1() {
            var $divBp = $('<div class="vis-allocation-leftControls">');
            $divBp.append('<span class="vis-allocation-inputLabels">' + VIS.translatedTexts.C_BPartner_ID + '</span>').append($vSearchBPartner.getControl().addClass("vis-allocation-bpartner")).append($vSearchBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            var $divCu = $('<div class="vis-allocation-leftControls">');
            $divCu.append('<span class="vis-allocation-inputLabels">' + VIS.translatedTexts.C_Currency_ID + '</span>').append($cmbCurrency);
            $innerRow.append($divBp).append($divCu);

            //added for sequence of multicurrency checkbox
            var $multiCurr = $('<div class="vis-allocation-leftControls">'
                + '<input name="vchkMultiCurrency" class="vis-allocation-multicheckbox" type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("MultiCurrency") + '</label>'
                + '</div>'
                + '<div class="vis-allocation-leftControls" id = VIS_cnvrDateDiv_' + $self.windowNo + ' >'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("ConversionDate") + '</span>'
                + '<input  class="vis-allocation-date" style="display:block;" id=VIS_cmbConversionDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');
            $innerRow.append($multiCurr);
            //end

            //added for enhancement of new combo regarding allocation from and to
            var $divallocFrom = $('<div class="vis-allocation-leftControls">');
            $divallocFrom.append('<span class="vis-allocation-inputLabels"> ' + VIS.Msg.getMsg("AllocationFrom") + '</span>').append($allocationFrom);
            var $divallocTo = $('<div class="vis-allocation-leftControls">');
            $divallocTo.append('<span class="vis-allocation-inputLabels"> ' + VIS.Msg.getMsg("AllocationTo") + '</span>').append($allocationTo);
            $innerRow.append($divallocFrom).append($divallocTo);
            //end

            var $rowOne = $('<div class="vis-allocation-leftControls" id="cashMaindiv"  style="display: none !important;">'
                + '<input  class="vis-allocation-cashbox"  type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("Cash") + '</label>'
                + '</div>'
                //Added checkbox for gl-allocation
                + '<div class="vis-allocation-leftControls" id="glMaindiv" style="display: none !important;">'
                + '<input  class="vis-allocation-glvoucher"  type="checkbox">'
                + '<label>' + VIS.Msg.getMsg("GL Voucher") + '</label>'
                + '</div>'

                // + '<div class="vis-allocation-leftControls vis-allocation-glinvoiceDiv" style="display: none !important;">'
                // + '<input  class="vis-allocation-glinvoice"  type="checkbox">'
                // + '<label>' + VIS.translatedTexts.C_Invoice_ID + '</label>'
                // + '</div>'

                //end
                //+ '<div class="vis-allocation-leftControls">'
                //+ '<input  class="vis-allocation-autowriteoff"  type="checkbox">'
                //+ '<label>' + VIS.Msg.getMsg("AutoWriteOff") + '</label>'
                //+ '</div>'
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
                + '<a role="button" data-toggle="collapse" data-parent="#accordion" href="#collapseOne" aria-expanded="true" aria-controls="collapseOne" class="VIS-Accordion-head collapsed"><span>' + VIS.Msg.getMsg("InvoiceFilter")
                + '</span><i class="glyphicon glyphicon-chevron-down pull-right"></i>'
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
            //$divChkGLInvoice = $innerRow.find('.vis-allocation-glinvoiceDiv');
            //end
            //$vchkAutoWriteOff = $innerRow.find('.vis-allocation-autowriteoff');

            //-----get controls values------------------
            $txtDocNo = $innerRow.find('.vis-allocation-docNo');
            $fromDate = $innerRow.find('.vis-allocation-fromDate');
            $toDate = $innerRow.find('.vis-allocation-toDate');
            //------------------------------------------
            var $resultDiv = $('<div class="vis-allocation-resultdiv">');
            $resultDiv.append('<div class="vis-allocation-leftControls">' +
                '<span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("Difference")
                + '</span>');
            $resultDiv.append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-lblCurrnecy"></span>'
                + '<span class="vis-allocation-lbldifferenceAmt" style="float:right;">'
                + '</span>');

            $resultDiv.append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("TransactionDate") + '</span>'
                + '<input  class="vis-allocation-date" disabled  id=VIS_cmbDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');
            $resultDiv.append('<div class="vis-allocation-leftControls">'
                + '<span class="vis-allocation-inputLabels" title="View allocation will be created on this date" type="date" >' + VIS.Msg.getMsg("DateAcct") + '</span>'
                + '<input  class="vis-allocation-date" disabled id=VIS_cmbAcctDate_' + $self.windowNo + ' type="date"></input>'
                + '</div>');
            $resultDiv.append('<div class="vis-allocation-leftControls"> <span class="vis-allocation-inputLabels" title="View allocation will be created in this organization" >' + VIS.translatedTexts.AD_Org_ID + '</span> <select class="vis-allocation-currencycmb" id=VIS_cmbOrg_' + $self.windowNo + '></select>');

            $innerRow.append($resultDiv);
            $date = $innerRow.find('#VIS_cmbDate_' + $self.windowNo);
            $date.val(Globalize.format(new Date(), "yyyy-MM-dd"));
            $dateAcct = $innerRow.find('#VIS_cmbAcctDate_' + $self.windowNo);
            $dateAcct.val(Globalize.format(new Date(), "yyyy-MM-dd"));
            $conversionDate = $innerRow.find('#VIS_cmbConversionDate_' + $self.windowNo);
            $conversionDiv = $innerRow.find('#VIS_cnvrDateDiv_' + $self.windowNo);
            $conversionDiv.css('display', 'none');
            $cmbOrg = $innerRow.find('#VIS_cmbOrg_' + $self.windowNo);
            $vchkBPAllocation = $innerRow.find('#VIS_chkbxBPAllocation_' + $self.windowNo);
            $vtxtDifference = $resultDiv.find('.vis-allocation-lbldifferenceAmt');
            $vlblAllocCurrency = $resultDiv.find('.vis-allocation-lblCurrnecy');
        };

        function createRow2() {
            $row2.append('<div class="d-flex"><p>' + VIS.translatedTexts.C_Payment_ID + '</p>  <input type="checkbox" id="paymentselectall" /><p class="vis-allocate-paymentSum">' + VIS.Msg.getMsg("SelectedPayments") + '0-Sum 0.00</p></div>').append('<div class="vis-allocation-payment-grid"></div>');//.append('<p class="vis-allocate-paymentSum">0-Sum 0.00</p>');
            $divPayment = $row2.find('.vis-allocation-payment-grid');
            $lblPaymentSum = $row2.find('.vis-allocate-paymentSum');
            $paymentSelctAll = $row2.find('#paymentselectall');
        };

        function createRow3() {
            $row3.append('<div class="d-flex"><p>' + VIS.translatedTexts.C_CashLine_ID + '</p> <input type="checkbox" id="cashselectall" /><p class="vis-allocate-cashSum">' + VIS.Msg.getMsg("SelectedCashlines") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-cashLine-grid"></div>');//.append('<p class="vis-allocate-cashSum">0-Sum 0.00</p>');
            $divCashline = $row3.find('.vis-allocation-cashLine-grid');
            $lblCashSum = $row3.find('.vis-allocate-cashSum');
            $cashSelctAll = $row3.find('#cashselectall');
        };

        function createRow4() {
            //
            $row4.append('<div class="d-flex"><p>' + VIS.translatedTexts.C_Invoice_ID + '</p> <input type="checkbox" id="invoiceselectall" /><p style="margin: 0 10px;"> ' + VIS.Msg.getMsg("Reset") + ' </p> <span id="clrbutton_' + $self.windowNo + '" style="cursor: pointer;margin: 2px 0 0;" class="glyphicon glyphicon-refresh"></span><p class="vis-allocate-invoiceSum">' + VIS.Msg.getMsg("SelectedInvoices") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-invoice-grid"></div>');//.append('<p class="vis-allocate-invoiceSum">0-Sum 0.00</p>');
            $divInvoice = $row4.find('.vis-allocation-invoice-grid');
            $lblInvoiceSum = $row4.find('.vis-allocate-invoiceSum');
            $invSelectAll = $row4.find('#invoiceselectall');
            //get control of clear button
            $clrbtn = $row4.find('#clrbutton_' + $self.windowNo);
        };

        //added grid design for gl-allocation
        function createRow5() {
            $row5.append('<div class="d-flex"><p>' + VIS.translatedTexts.GL_Journal_ID + '</p> <input type="checkbox" id="glselectall" /><p class="vis-allocate-glSum">' + VIS.Msg.getMsg("SelectedGL") + ' 0-Sum 0.00</p></div>').append('<div  class="vis-allocation-gl-grid"></div>');
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
            var AD_Role_ID = ctx.getAD_Role_ID();
            var AD_User_ID = ctx.getAD_User_ID();
            var role = VIS.MRole.getDefault();
            //  Need to have both values
            //	Async BPartner Test
            var key = _C_BPartner_ID;
            /********************************
         *  Load unallocated Payments
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            loadUnallocatedPayments();

            /********************************
         *  Load unallocated Cash lines
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            loadUnallocatedCashLines();

            loadInvoice();//Neha
            // refresh label on load
            refreshLabel();
            $allocationFrom.val("P");
            $allocationFrom.trigger("change");
            $allocationTo.trigger("change");
            displayGrids($allocationFrom.val(), $allocationTo.val());
            //$bsyDiv[0].style.visibility = "hidden";
        };

        /********************************
*  Load unallocated Payments
*      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
*      5-ConvAmt, 6-ConvOpen, 7-Allocated
*/
        function loadUnallocatedPayments() {
            if (_C_BPartner_ID == 0 || _C_Currency_ID == 0) {
                return;
            }
            pageNoPayment = 1, gridPgnoPayment = 1, paymentRecord = 0;
            isPaymentGridLoaded = false;
            var chk = $vchkMultiCurrency.is(':checked');
            $bsyDiv[0].style.visibility = "visible";
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
        };

        /********************************
    *  Load unallocated Cash lines
    *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
    *      5-ConvAmt, 6-ConvOpen, 7-Allocated
    */
        function loadUnallocatedCashLines() {
            if (_C_BPartner_ID == 0 || _C_Currency_ID == 0) {
                return;
            }
            pageNoCashJournal = 1, gridPgnoCashJounal = 1, CashRecord = 0;
            isCashGridLoaded = false;
            var chk = $vchkMultiCurrency.is(':checked');
            $bsyDiv[0].style.visibility = "visible";
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
        //render to culture format
        function bindPaymentGrid(data, chk) {
            var columns = [];
            columns.push({ field: 'SelectRow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            columns.push({
                field: "Date1", caption: VIS.translatedTexts.Date, size: '80px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Date1"];
                    return new Date(val).toLocaleString();
                }
            });
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
                //render column into float with culture format
                columns.push({
                    field: "Payment", caption: VIS.translatedTexts.Amount, size: '150px', hidden: false, render: function (record, index, col_index) {
                        var val = record["Payment"];
                        return parseFloat(val).toLocaleString();
                    }
                });
            }
            else {
                columns.push({ field: "Payment", caption: VIS.translatedTexts.Amount, size: '150px', hidden: true });
            }
            //render column into float with culture format
            columns.push({
                field: "ConvertedAmount", caption: VIS.translatedTexts.ConvertedAmount, size: '150px', hidden: false, render: function (record, index, col_index) {
                    var val = record["ConvertedAmount"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "OpenAmt", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false, render: function (record, index, col_index) {
                    var val = record["OpenAmt"];
                    return parseFloat(val).toLocaleString();
                }
            });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, size: '150px', hidden: true });
            //render column into float with culture format
            columns.push({
                field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false, render: function (record, index, col_index) {

                    var val = record["AppliedAmt"];
                    return parseFloat(val).toLocaleString();
                }
            });
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({
                field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false, render: function (record, index, col_index) {
                    var val = record["DATEACCT"];
                    return new Date(val).toLocaleString();
                }
            });
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
                },
                //on edit grid columns need to prevent dot or comma according to culture
                onEditField: function (event) {
                    event.onComplete = function (event) {
                        id = event.recid;

                        $('#grid_openformatgrid_' + $self.windowNo + '_edit_' + id + '_10').keydown(function (event) {
                            var isDotSeparator = culture.isDecimalSeparatorDot(window.navigator.language);

                            if (!isDotSeparator && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (isDotSeparator && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                this.value = this.value.replace('.', '');
                            }
                            if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                this.value = this.value.replace(',', '');
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    };
                }
            });
            $gridPayment.autoLoad = true;
            $($($gridPayment.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($($gridPayment.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
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
        //culture work
        function bindCashline(data, chk) {
            var columns = [];
            columns.push({ field: 'SelectRow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            columns.push({
                field: "Created", caption: VIS.translatedTexts.Date, size: '80px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Created"];
                    return new Date(val).toLocaleString();
                }
            });
            columns.push({ field: "ReceiptNo", caption: VIS.translatedTexts.RECEIPTNO, size: '120px', hidden: false });
            if (chk == true) {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: false });
            }
            else {
                columns.push({ field: "Isocode", caption: VIS.Msg.getMsg("TrxCurrency"), size: '85px', hidden: true });
            }
            columns.push({ field: "ConversionName", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: false });
            if (chk == true) {
                //render column into float with culture format
                columns.push({
                    field: "Amount", caption: VIS.translatedTexts.Amount, size: '150px', hidden: false, render: function (record, index, col_index) {
                        var val = record["Amount"];
                        return parseFloat(val).toLocaleString();
                    }
                });
            }
            else {
                columns.push({ field: "Amount", caption: VIS.translatedTexts.Amount, size: '150px', hidden: true });
            }
            //render column into float with culture format
            columns.push({
                field: "ConvertedAmount", caption: VIS.Msg.getMsg("ConvertedAmount"), size: '150px', hidden: false, render: function (record, index, col_index) {
                    var val = record["ConvertedAmount"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "OpenAmt", caption: VIS.translatedTexts.OpenAmount, size: '150px', hidden: false, render: function (record, index, col_index) {
                    var val = record["OpenAmt"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '150px', hidden: false, render: function (record, index, col_index) {
                    var val = record["AppliedAmt"];
                    return parseFloat(val).toLocaleString();
                }
            });
            columns.push({ field: "CcashlineiID", caption: VIS.translatedTexts.ccashlineid, size: '150px', hidden: true });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, hidden: true });
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({
                field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false, render: function (record, index, col_index) {
                    var val = record["DATEACCT"];
                    return new Date(val).toLocaleString();
                }
            });
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
                },
                //on edit grid columns need to prevent dot or comma according to culture
                onEditField: function (event) {
                    event.onComplete = function (event) {
                        id = event.recid;

                        $('#grid_openformatgridcash_' + $self.windowNo + '_edit_' + id + '_8').keydown(function (event) {
                            var isDotSeparator = culture.isDecimalSeparatorDot(window.navigator.language);

                            if (!isDotSeparator && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (isDotSeparator && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                this.value = this.value.replace('.', '');
                            }
                            if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                this.value = this.value.replace(',', '');
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    };
                }
            });

            $gridCashline.autoLoad = true;
            $($($gridCashline.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($($gridCashline.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
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
            columns.push({
                field: "Date1", caption: VIS.translatedTexts.Date, size: '80px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Date1"];
                    return new Date(val).toLocaleString();
                }
            });
            columns.push({
                field: "InvoiceScheduleDate", caption: VIS.translatedTexts.ScheduleDate, size: '80px', hidden: false, render: function (record, index, col_index) {
                    var val = record["InvoiceScheduleDate"];
                    return new Date(val).toLocaleString();
                }
            });
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
                //render column into float with culture format
                columns.push({
                    field: "Currency", caption: VIS.translatedTexts.Amount, size: '100px', hidden: false, render: function (record, index, col_index) {
                        var val = record["Currency"];
                        return parseFloat(val).toLocaleString();
                    }
                });
            }
            else {
                columns.push({ field: "Currency", caption: VIS.translatedTexts.Amount, size: '100px', hidden: true });
            }
            //render column into float with culture format
            columns.push({
                field: "Converted", caption: VIS.Msg.getMsg("ConvertedAmount"), size: '100px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Converted"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "Amount", caption: VIS.translatedTexts.OpenAmount, size: '100px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Amount"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "Discount", caption: VIS.translatedTexts.DiscountAmt, size: '100px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Discount"];
                    return parseFloat(val).toLocaleString();
                }
            });
            columns.push({ field: "Multiplierap", caption: VIS.translatedTexts.multiplierap, hidden: true });
            columns.push({ field: "DocBaseType", caption: VIS.translatedTexts.DocBaseType, size: '150px', hidden: true });
            //render column into float with culture format
            columns.push({
                field: "Writeoff", caption: VIS.translatedTexts.WriteOffAmount, size: '100px', hidden: false, render: function (record, index, col_index) {
                    var val = record["Writeoff"];
                    return parseFloat(val).toLocaleString();
                }
            });
            //render column into float with culture format
            columns.push({
                field: "AppliedAmt", caption: VIS.translatedTexts.AppliedAmount, size: '100px', hidden: false, render: function (record, index, col_index) {
                    var val = record["AppliedAmt"];
                    return parseFloat(val).toLocaleString();
                }
            });
            if (countVA009 > 0) {
                columns.push({ field: "C_InvoicePaySchedule_ID", caption: VIS.translatedTexts.C_InvoicePaySchedule_ID, size: '100px', hidden: false });
            }
            columns.push({ field: "C_ConversionType_ID", caption: VIS.translatedTexts.C_ConversionType_ID, size: '85px', hidden: true });
            columns.push({
                field: "DATEACCT", caption: VIS.translatedTexts.DateAcct, size: '85px', hidden: false, render: function (record, index, col_index) {
                    var val = record["DATEACCT"];
                    return new Date(val).toLocaleString();
                }
            });
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
                onChange: function (event) {
                    invoiceCellChanged(event);
                    getMaxDate();
                },
                onClick: function (event) {
                    invoiceCellClicked(event);
                    getMaxDate();
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
                },
                //on edit grid columns need to prevent dot or comma according to culture
                onEditField: function (event) {
                    event.onComplete = function (event) {
                        id = event.recid;
                        if (event.column == 14 || event.column == 13 || event.column == 10) {
                            $('#grid_openformatgridinvoice_' + $self.windowNo + '_edit_' + id + '_' + event.column).keydown(function (event) {
                                var isDotSeparator = culture.isDecimalSeparatorDot(window.navigator.language);

                                if (!isDotSeparator && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                    return false;
                                }
                                else if (isDotSeparator && event.keyCode == 188) { // . separator
                                    return false;
                                }
                                if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                    this.value = this.value.replace('.', '');
                                }
                                if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                    this.value = this.value.replace(',', '');
                                }
                                if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                    (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                    && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                    && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                    return false;
                                }
                            });
                        }
                    };
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
                //changed column type from text to number on edit event
                $gridPayment.columns[event.column].editable = { type: 'number' }
                $gridPayment.columns[event.column].render = function (record, index, col_index) {

                    var val = 0;
                    if ($gridPayment.columns[event.column].field == "AppliedAmt") {
                        if (record.changes == undefined || record.changes.AppliedAmt == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.OpenAmt, record.Payment);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.OpenAmt, record.Payment);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                record.changes.AppliedAmt = checkcommaordot(event, record.changes.AppliedAmt, record.AppliedAmt);
                                val = record.changes.AppliedAmt;
                            }
                        }
                    }
                    if ($gridPayment.columns[event.column].field == "Writeoff") {
                        if (record.changes == undefined || record.changes.Writeoff == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                        }
                    }
                    if ($gridPayment.columns[event.column].field == "Discount") {
                        if (record.changes == undefined || record.changes.Discount == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                record.changes.Discount = checkcommaordot(event, record.changes.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                        }
                    }
                    //else if ($gridPayment.columns[event.column].field == "Writeoff") {
                    //    if (record.changes == undefined || record.changes.Writeoff == undefined) {
                    //        val = 0;
                    //    }
                    //    else {
                    //        record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Writeoff);
                    //        val = record.changes.Writeoff;
                    //    }
                    //}
                    //else if ($gridPayment.columns[event.column].field == "Discount") {
                    //    if (record.changes == undefined || record.changes.Discount == undefined) {
                    //        val = 0;
                    //    }
                    //    else {
                    //        record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Discount);
                    //        val = record.changes.Discount;
                    //    }
                    //}

                    if (parseFloat(val) > 0 && parseFloat(val) > parseFloat(record.OpenAmt)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.OpenAmt);
                    }
                    else if (parseFloat(record.OpenAmt) < 0 && parseFloat(val) < parseFloat(record.OpenAmt)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.OpenAmt);
                    }

                    return parseFloat(val).toLocaleString();
                };
            }
            //End
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
                //changed column type from text to number on edit event
                $gridCashline.columns[event.column].editable = { type: 'number' }
                $gridCashline.columns[event.column].render = function (record, index, col_index) {

                    var val = 0;
                    if ($gridCashline.columns[event.column].field == "AppliedAmt") {
                        if (record.changes == undefined || record.changes.AppliedAmt == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.OpenAmt, record.OpenAmt);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.OpenAmt, record.OpenAmt);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                record.changes.AppliedAmt = checkcommaordot(event, record.changes.AppliedAmt, record.OpenAmt);
                                val = record.changes.AppliedAmt;
                            }
                        }
                    }
                    if ($gridCashline.columns[event.column].field == "Writeoff") {
                        if (record.changes == undefined || record.changes.Writeoff == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                        }
                    }
                    if ($gridCashline.columns[event.column].field == "Discount") {
                        if (record.changes == undefined || record.changes.Discount == undefined || record.changes.SelectRow == undefined) {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                record.changes.Discount = checkcommaordot(event, record.changes.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                        }
                    }
                    //else if ($gridCashline.columns[event.column].field == "Writeoff") {
                    //    if (record.changes == undefined || record.changes.Writeoff == undefined) {
                    //        val = 0;
                    //    }
                    //    else {
                    //        record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Writeoff);
                    //        val = record.changes.Writeoff;
                    //    }
                    //}
                    //else if ($gridCashline.columns[event.column].field == "Discount") {
                    //    if (record.changes == undefined || record.changes.Discount == undefined) {
                    //        val = 0;
                    //    }
                    //    else {
                    //        record.changes.Discount = checkcommaordot(event, record.changes.Discount, record.Discount);
                    //        val = record.changes.Discount;
                    //    }
                    //}

                    if (parseFloat(val) > 0 && parseFloat(val) > parseFloat(record.OpenAmt)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.OpenAmt);
                    }
                    else if (parseFloat(record.OpenAmt) < 0 && parseFloat(val) < parseFloat(record.OpenAmt)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.OpenAmt);
                    }
                    return parseFloat(val).toLocaleString();
                }
                //End
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
                //changed column type from text to number on edit event
                $gridInvoice.columns[event.column].editable = { type: 'number' }
                $gridInvoice.columns[event.column].render = function (record, index, col_index) {

                    var val = 0;
                    if ($gridInvoice.columns[event.column].field == "AppliedAmt") {
                        if (record.changes == undefined || record.changes.AppliedAmt == undefined || record.changes.SelectRow == undefined) {

                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.Amount, record.Amount);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.AppliedAmt == undefined)) {
                                record.changes.AppliedAmt = checkcommaordot(event, record.Amount, record.Amount);
                                val = record.changes.AppliedAmt;
                            }
                            else {
                                record.changes.AppliedAmt = checkcommaordot(event, record.changes.AppliedAmt, record.Amount);
                                val = record.changes.AppliedAmt;
                            }
                        }
                    }
                    else if ($gridInvoice.columns[event.column].field == "Writeoff") {
                        if (record.changes == undefined || record.changes.Writeoff == undefined || record.changes.SelectRow == undefined) {

                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Writeoff == undefined)) {
                                record.changes.Writeoff = checkcommaordot(event, record.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                            else {
                                record.changes.Writeoff = checkcommaordot(event, record.changes.Writeoff, record.Writeoff);
                                val = record.changes.Writeoff;
                            }
                        }
                    }
                    else if ($gridInvoice.columns[event.column].field == "Discount") {
                        if (record.changes == undefined || record.changes.Discount == undefined || record.changes.SelectRow == undefined) {

                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                val = 0;
                            }
                        }
                        else {
                            if ((record.changes != undefined) && (record.changes.SelectRow != undefined) && (record.changes.SelectRow == true && record.changes.Discount == undefined)) {
                                record.changes.Discount = checkcommaordot(event, record.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                            else {
                                record.changes.Discount = checkcommaordot(event, record.changes.Discount, record.Discount);
                                val = record.changes.Discount;
                            }
                        }
                    }
                    if (parseFloat(val) > 0 && parseFloat(val) > parseFloat(record.Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.Amount);
                    }
                    else if (parseFloat(record.Amount) < 0 && parseFloat(val) < parseFloat(record.Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        val = parseFloat(record.Amount);
                    }

                    return parseFloat(val).toLocaleString();
                }
            }
            //End
        };

        // function to check comma or dot from given value and return new value
        function checkcommaordot(event, val, amt) {
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
        function removeAllButLast(amt, seprator) {
            var parts = amt.split(seprator);
            amt = parts.slice(0, -1).join('') + '.' + parts.slice(-1);
            if (amt.indexOf('.') == (amt.length - 1)) {
                amt = amt.replace(".", "");
            }
            return amt;
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

                /*** after discussion with ashish we finalized that gl journal applied amount will not be editable because we can not split gl 
                     journal and we can not use gl journal partialy **/

                //if (element[0].SelectRow == true) {
                //    glLineGrid.columns[event.column].editable = { type: 'text' };
                //}
                //else {
                glLineGrid.columns[event.column].editable = false;
                //    return;
                //}
            }
        };
        //end

        function invoiceCellClicked(event) {
            var colIndex = $gridInvoice.getColumn('AppliedAmt', true);
            //get the index of SelectRow Column checkbox from grid
            var selectColIndex = $gridInvoice.getColumn('SelectRow', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridInvoice.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    //Set value to 0 when element is null
                    $gridInvoice.records[event.recid]["AppliedAmt"] = 0;
                    $gridInvoice.records[event.recid]["Writeoff"] = 0;
                    $gridInvoice.records[event.recid]["Discount"] = 0;
                    $gridInvoice.refreshCell(event.recid, "AppliedAmt");
                    $gridInvoice.refreshCell(event.recid, "Writeoff");
                    $gridInvoice.refreshCell(event.recid, "Discount");
                    getMaxDate();
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

                            //changed 0 to event.recid because we need to refresh selected row before it was working only for 1st row.
                            $gridInvoice.get(event.recid).changes = false;
                            $gridInvoice.columns[colIndex].editable = false;
                            $gridInvoice.get(event.recid).changes.AppliedAmt = "0";
                            $gridInvoice.refreshCell(event.recid, "AppliedAmt");
                            $gridInvoice.get(event.recid).changes.Discount = "0";
                            $gridInvoice.refreshCell(event.recid, "Discount");
                            $gridInvoice.get(event.recid).changes.Writeoff = "0";
                            $gridInvoice.refreshCell(event.recid, "Writeoff");
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
                        //changed 0 to event.recid because we need to refresh selected row before it was working only for 1st row.
                        $gridInvoice.get(event.recid).changes.AppliedAmt = "0";
                        $gridInvoice.refreshCell(event.recid, "AppliedAmt");
                        $gridInvoice.get(event.recid).changes.Discount = "0";
                        $gridInvoice.refreshCell(event.recid, "Discount");
                        $gridInvoice.get(event.recid).changes.Writeoff = "0";
                        $gridInvoice.refreshCell(event.recid, "Writeoff");
                    }

                }
                //check weather someone clicked on select column checkbox in grid
                if ((selectColIndex == event.column || event.column == null) && ((element[0] != undefined) && !(element[0].SelectRow == undefined))) {
                    $gridInvoice.records[event.recid]["AppliedAmt"] = VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount);
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
            //get the index of SelectRow Column checkbox from grid
            var selectColIndex = $gridPayment.getColumn('SelectRow', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridPayment.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    //Set value to 0 when element is null
                    $gridPayment.records[event.recid]["AppliedAmt"] = 0;
                    $gridPayment.records[event.recid]["Writeoff"] = 0;
                    $gridPayment.records[event.recid]["Discount"] = 0;
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
                //check weather someone clicked on select column checkbox in grid
                if ((selectColIndex == event.column || event.column == null) && ((element[0] != undefined) && !(element[0].SelectRow == undefined))) {
                    $gridPayment.records[event.recid]["AppliedAmt"] = VIS.Utility.Util.getValueOfDecimal($gridPayment.get(event.recid).OpenAmt);
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
            //get the index of SelectRow Column checkbox from grid
            var selectColIndex = $gridCashline.getColumn('SelectRow', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = $gridCashline.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].SelectRow == undefined) {
                    //Set value to 0 when element is null
                    $gridCashline.records[event.recid]["AppliedAmt"] = 0;
                    $gridCashline.records[event.recid]["Writeoff"] = 0;
                    $gridCashline.records[event.recid]["Discount"] = 0;
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
                            $gridCashline.refreshCell(0, "AppliedAmt");
                            VIS.ADialog.warn(("VIS_ConversionNotMatched"));
                        }

                        if ($vchkMultiCurrency.is(':checked')) {
                            if ($conversionDate.val() == "") {
                                VIS.ADialog.warn("VIS_SlctcnvrsnDate");
                                $gridCashline.get(event.recid).changes = false;
                                $gridCashline.unselect(event.recid);
                                $gridCashline.columns[colIndex].editable = false;
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
                //check weather someone clicked on select column checkbox in grid
                if ((selectColIndex == event.column || event.column == null) && ((element[0] != undefined) && !(element[0].SelectRow == undefined))) {
                    //$gridCashline.records[event.recid]["AppliedAmt"] = VIS.Utility.Util.getValueOfDecimal($gridCashline.get(event.recid).Amount);
                    $gridCashline.records[event.recid]["AppliedAmt"] = VIS.Utility.Util.getValueOfDecimal($gridCashline.get(event.recid).ConvertedAmount);
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
                    $gridCashline.get(event.recid).changes.AppliedAmt = $gridCashline.get(event.recid).OpenAmt;
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfInt($gridCashline.get(event.recid).OpenAmt) < 0 && VIS.Utility.Util.getValueOfInt(event.value_new) < VIS.Utility.Util.getValueOfInt($gridCashline.get(event.recid).OpenAmt)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    $gridCashline.get(event.recid).changes.AppliedAmt = $gridCashline.get(event.recid).OpenAmt;
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

        };

        function invoiceCellChanged(event) {

            var colIndex = $gridInvoice.getColumn('AppliedAmt', true);
            var wcolIndex = $gridInvoice.getColumn('Writeoff', true);
            var dcolIndex = $gridInvoice.getColumn('Discount', true);
            var selectColIndex = $gridInvoice.getColumn('SelectRow', true);
            var discountchng = 0;
            if ($gridInvoice.columns[colIndex].editable == undefined && $gridInvoice.columns[wcolIndex].editable == undefined && $gridInvoice.columns[dcolIndex].editable == undefined) {
                return;
            }
            if ($gridInvoice.getChanges(event.recid) != undefined && $gridInvoice.getChanges(event.recid).length > 0 && $gridInvoice.get(event.recid).changes) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (VIS.Utility.Util.getValueOfInt(event.value_new) > 0 && VIS.Utility.Util.getValueOfInt(event.value_new) > VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    $gridInvoice.get(event.recid).changes.AppliedAmt = $gridInvoice.get(event.recid).Amount;
                    event.preventDefault();
                    return;
                }
                else if (VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount) < 0 && VIS.Utility.Util.getValueOfInt(event.value_new) < VIS.Utility.Util.getValueOfInt($gridInvoice.get(event.recid).Amount)) {
                    VIS.ADialog.warn("AppliedAmtgrtr");
                    $gridInvoice.get(event.recid).changes.AppliedAmt = $gridInvoice.get(event.recid).Amount;
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
                        $gridInvoice.get(event.recid).changes.AppliedAmt = $gridInvoice.get(event.recid).Amount;
                        event.preventDefault();
                        return;
                    }
                    else if (VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount) > 0 && (VIS.Utility.Util.getValueOfDecimal(event.value_new) +
                        VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Writeoff) + VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).changes.Discount))
                        > VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount)) {
                        VIS.ADialog.warn("AppliedAmtgrtr");
                        $gridInvoice.get(event.recid).changes.AppliedAmt = $gridInvoice.get(event.recid).Amount;
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

            //if we click on select column checkbox in grid
            if (selectColIndex == event.column || event.column == null) {
                $gridInvoice.records[event.recid]["AppliedAmt"] = VIS.Utility.Util.getValueOfDecimal($gridInvoice.get(event.recid).Amount);
            }

            if (event.column == $gridInvoice.getColumn('Discount', true)
                || event.column == $gridInvoice.getColumn('Writeoff', true)
                || event.column == $gridInvoice.getColumn('AppliedAmt', true)) {

                tableChanged(event.index, event.column, true, false);

            }
        };

        function tableChanged(rowIndex, colIndex, isInvoice, cash) {
            var row = rowIndex;
            var col = colIndex;
            if (col == null || col == undefined) {
                col = 0;
            }
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

                        $gridPayment.columns[_payment].editable = false;
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

                        $gridCashline.columns[_paymentCash].editable = false;
                    }
                }
            }
            //  Invoice Selection
            else if (col == 0) {
                var columns = $gridInvoice.columns;

                colInvCheck = getBoolValue($gridInvoice.getChanges(), row);
                //  selected - set payment amount
                var changes = $gridInvoice.get(row).changes;
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
                            $gridInvoice.records[row]["AppliedAmt"] = 0;
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

                    $gridInvoice.columns[_applied].editable = false;
                }
            }

            //  Invoice - Try to balance entry
            //if ($vchkAutoWriteOff.is(':checked')) {
            //    autoWriteOff();
            //}
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
            // if cash record is selected means our cash grid is not readonly
            //if ($vchkAllocation.is(':checked')) {
            if (!readOnlyCash) {
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

            if (invoiceTotal > paymentTotal) {
                var difference = invoiceTotal - paymentTotal;
                if ((invoiceTotal * .05) >= difference) {
                    if (lastRow != null) {
                        lastRow.changes.Writeoff = difference;
                        lastRow.changes.AppliedAmt = lastRow.changes.AppliedAmt - difference;
                        $gridInvoice.refreshCell(lastRow.recid, "Writeoff");
                        $gridInvoice.refreshCell(lastRow.recid, "AppliedAmt");
                    }
                }
            }
        };

        function bpValueChanged() {
            vetoableChange($vSearchBPartner.getName(), $vSearchBPartner.value);
            //added for gl-allocation
            //////loadGLDataGrid($vSearchBPartner.event);
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
                        //_dateAcct = [];
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
                        if ($gridPayment.getChanges().length == 0 && $gridCashline.getChanges().length == 0 && glLineGrid.getChanges().length == 0) {
                            var DATEACCT = $gridInvoice.records[$gridInvoice.getChanges()[i].recid].DATEACCT;
                            _dateAcct.push(new Date(DATEACCT));
                            $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        }
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
                        if ($gridPayment.getChanges().length == 0 && $gridInvoice.getChanges().length == 0 && glLineGrid.getChanges().length == 0) {
                            var DATEACCT = $gridCashline.records[$gridCashline.getChanges()[i].recid].DATEACCT;
                            _dateAcct.push(new Date(DATEACCT));
                            $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        }
                    }
                }
            }
            for (var i = 0; i < glLineGrid.getChanges().length; i++) {
                if (glLineGrid.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if (glLineGrid.getChanges()[i].SelectRow == true) {
                        var row = glLineGrid.records[glLineGrid.getChanges()[i].recid].DATEACCT;
                        _allDates.push(new Date(row));
                        if ($gridPayment.getChanges().length == 0 && $gridCashline.getChanges().length == 0 && $gridInvoice.getChanges().length == 0) {
                            var DATEACCT = glLineGrid.records[glLineGrid.getChanges()[i].recid].DATEACCT;
                            _dateAcct.push(new Date(DATEACCT));
                            $dateAcct.val(Globalize.format(new Date(Math.max.apply(null, _dateAcct)), "yyyy-MM-dd"));
                        }
                    }
                }
            }
            maxDate = new Date(Math.max.apply(null, _allDates));
            if (_allDates.length == 0)
                maxDate = new Date();
            //$date.val(Globalize.format(new Date(maxDate), "yyyy-MM-dd"));
            $dateAcct.val(Globalize.format(new Date(maxDate), "yyyy-MM-dd"));
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
                        if (result) {
                            callbackLoadGlLines(result);
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
                var data = JSON.parse(data);
                var rows = [];

                var colkeys = [];


                if (data.length > 0) {
                    colkeys = Object.keys(data[0]);
                }
                // check records exist in grid or not
                // for updating record id 
                var count = 0;
                if (glLineGrid.records.length > 0) {
                    count = glLineGrid.records.length;
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
                //glLineGrid.add(rows);
                glLineGrid.records = rows;
                glLineGrid.refresh();
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
                    }
                }
            }
            //added for gl-allocation
            for (var i = 0; i < glLineGrid.getChanges().length; i++) {
                if (glLineGrid.getChanges()[i].SelectRow === undefined) {
                }
                else {
                    if (glLineGrid.getChanges()[i].SelectRow == true) {
                        var row = glLineGrid.records[glLineGrid.getChanges()[i].recid].DATEACCT;
                        _allDates.push(new Date(row));
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
            if (value == null) {
                return;
            }

            //  BPartner
            if (name == "C_BPartner_ID") {
                if (_C_BPartner_ID != value) {
                    //added to load all blank grids
                    blankAllGrids();
                }
                _C_BPartner_ID = value;
                if (_C_BPartner_ID > 0) {
                    // If BP is selected then  set mandatory false---Neha
                    $vSearchBPartner.getControl().css("background-color", SetMandatory(false));
                }
                //////loadBPartner();
            }
            //	Currency
            else if (name == "C_Currency_ID") {
                _C_Currency_ID = parseInt(value);
                ////////loadBPartner();
            }
            //	Date for Multi-Currency
            else if (name == "Date" && $vchkMultiCurrency) {
                //////loadBPartner();
                refreshLabel();
            }
        }

        function calculate() {
            $bsyDiv[0].style.visibility = "visible";
            var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
            var allocDate = new Date();
            allocDate = null;
            //  Payment******************
            var totalPay = parseFloat(0);
            _noPayments = 0;
            var rowsPayment = null;
            var rowsCash = null;
            var rowsInvoice = null;
            //added for gl-allocation
            var rowsGL = null;
            //end
            if ($gridPayment)
                rowsPayment = $gridPayment.getChanges();
            if ($gridCashline)
                rowsCash = $gridCashline.getChanges();
            if ($gridInvoice)
                rowsInvoice = $gridInvoice.getChanges();
            if (glLineGrid)
                rowsGL = glLineGrid.getChanges();


            if (rowsPayment != null) {
                for (var i = 0; i < rowsPayment.length; i++) {
                    if (rowsPayment[i].SelectRow == true) {
                        var currnetRow = $gridPayment.get(rowsPayment[i].recid);
                        var ts = new Date(currnetRow.Date1);
                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);
                        var keys = Object.keys(currnetRow);
                        var bd = parseFloat(rowsPayment[i][keys[keys.indexOf("AppliedAmt")]]);
                        totalPay = totalPay + (isNaN(bd) ? 0 : bd);  //  Applied Pay
                        _noPayments++;
                    }
                }
            }
            $lblPaymentSum.text(VIS.Msg.getMsg("SelectedPayments") + _noPayments + " - " + VIS.Msg.getMsg("Sum") + "  " + parseFloat(totalPay.toFixed(stdPrecision)).toLocaleString() + " ");

            //  Cash******************
            var totalCash = parseFloat(0);
            _noCashLines = 0;
            if (rowsCash != null) {
                for (var i = 0; i < rowsCash.length; i++) {
                    if (rowsCash[i].SelectRow == true) {
                        var currnetRow = $gridCashline.get(rowsCash[i].recid);
                        var ts = new Date(currnetRow.Created);
                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);
                        //************************************** Changed
                        var keys = Object.keys(currnetRow);
                        var bd = parseFloat(rowsCash[i][keys[keys.indexOf("AppliedAmt")]]);
                        totalCash = totalCash + (isNaN(bd) ? 0 : bd);  //  Applied Pay
                        _noCashLines++;
                    }
                }
            }
            $lblCashSum.text(VIS.Msg.getMsg("SelectedCashlines") + _noCashLines + " - " + VIS.Msg.getMsg("Sum") + "  " + parseFloat((totalCash.toFixed(stdPrecision))).toLocaleString() + " ");


            //  Invoices******************
            var totalInv = parseFloat(0);
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
                        var keys = Object.keys(currnetRow);
                        var bd;
                        if (rowsInvoice[i][keys[keys.indexOf("AppliedAmt")]] != "") {
                            bd = parseFloat(rowsInvoice[i][keys[keys.indexOf("AppliedAmt")]]);
                        }
                        else {
                            bd = 0;
                        }
                        totalInv = totalInv + (isNaN(bd) ? 0 : bd);  //  Applied Inv
                        _noInvoices++;
                    }
                }
            }
            $lblInvoiceSum.text(VIS.Msg.getMsg("SelectedInvoices") + _noInvoices + " - " + VIS.Msg.getMsg("Sum") + "  " + parseFloat(totalInv.toFixed(stdPrecision)).toLocaleString() + " ");

            //added for gl-allocation

            var totalGL = parseFloat(0);
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
                        if (rowsGL[i][keys[keys.indexOf("AppliedAmt")]] != "") {
                            bd = parseFloat(rowsGL[i][keys[keys.indexOf("AppliedAmt")]]);
                        }
                        else {
                            bd = 0;
                        }
                        totalGL = totalGL + (isNaN(bd) ? 0 : bd);  //  Applied Inv
                        _noGL++;
                    }
                }
            }
            $lblglSum.text(VIS.Msg.getMsg("SelectedGL") + _noGL + " - " + VIS.Msg.getMsg("Sum") + "  " + parseFloat(totalGL.toFixed(stdPrecision)).toLocaleString() + " ");
            //end

            //	Set AllocationDate
            // not to update date. now it always shows as current date
            //  Set Allocation Currency
            if ($cmbCurrency.children("option").filter(":selected").text() != null) {
                $vlblAllocCurrency.text($cmbCurrency.children("option").filter(":selected").text());
            }
            //  Difference 
            //  Difference --New Logic for Invoice-(cash+payment)-by raghu 18-jan-2011 //  Cash******************
            var difference = (parseFloat(totalPay.toFixed(stdPrecision)) + parseFloat(totalCash.toFixed(stdPrecision))) - (parseFloat(totalInv.toFixed(stdPrecision)) + parseFloat(totalGL.toFixed(stdPrecision)));
            $vtxtDifference.text(parseFloat(difference.toFixed(stdPrecision)).toLocaleString());
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
            // if cash record is selected means our cash grid is not readonly
            //if ($vchkAllocation.is(':checked')) {
            if (!readOnlyCash) {
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
            if ($allocationFrom.val() == "G" || $allocationTo.val() == "G") {
                //   needs to check in case of Party JV allocation weather you are allocating payment with JV Or you are allocating CashLine with JV.
                glData(rowsPayment, rowsInvoice, rowsCash, rowsGLVoucher, DateTrx, DateAcct);
            }
            else if ($allocationFrom.val() == "C" || $allocationTo.val() == "C") {
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
                            applied = keys[keys.indexOf("AppliedAmt")];
                            discount = keys[keys.indexOf("Discount")];
                            writeOff = keys[keys.indexOf("Writeoff")];
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
                                isCash: true, _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
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
                            applied = keys[keys.indexOf("AppliedAmt")];
                            payment = keys[keys.indexOf("AppliedAmt")];
                            discount = keys[keys.indexOf("Discount")];
                            writeOff = keys[keys.indexOf("Writeoff")];
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
                        // if cash record is selected means our cash grid is not readonly
                        var cashcheck = false;
                        if (!readOnlyCash) {
                            cashcheck = true;
                        }

                        $.ajax({
                            url: VIS.Application.contextUrl + "PaymentAllocation/SavePaymentData",
                            type: 'POST',
                            data: ({
                                paymentData: JSON.stringify(paymentData), cashData: JSON.stringify(cashData), invoiceData: JSON.stringify(invoiceData), currency: $cmbCurrency.val(),
                                isCash: cashcheck, _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
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
                            applied = keys[keys.indexOf("AppliedAmt")];
                            discount = keys[keys.indexOf("Discount")];
                            writeOff = keys[keys.indexOf("Writeoff")];
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
                data: { paymentData: rowsPayment, invoiceData: rowsInvoice, cashData: rowsCash, glData: rowsGLVoucher, DateTrx: DateTrx, _windowNo: self.windowNo, C_Currency_ID: _C_Currency_ID, C_BPartner_ID: _C_BPartner_ID, AD_Org_ID: $cmbOrg.val(), C_CurrencyType_ID: C_CurrencyType_ID, DateAcct: $dateAcct.val() },
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

        /**Work done for searching from left parameters */
        function searchbyParameters() {
            console.info("BPartner Name : " + $vSearchBPartner.getName() + " , Value : " + $vSearchBPartner.value);
            console.info("Currency ID : " + $cmbCurrency.val());
            console.info("Allocation From : " + $allocationFrom.val());
            console.info("Allocation To : " + $allocationTo.val());
            console.info("MultiCurrency : " + $vchkMultiCurrency.is(':checked'));
            console.info("Organization : " + $cmbOrg.val());
            console.info("Document Number : " + $txtDocNo.val());
            console.info("Document Type : " + $cmbDocType.val());
            console.info("From Date : " + $fromDate.val());
            console.info("To Date : " + $toDate.val());
            console.info("Conversion Date : " + $conversionDate.val());
            $bsyDiv[0].style.visibility = "visible";
            // initialize Pageno as 1 when we change BP
            pageNoInvoice = 1, gridPgnoInvoice = 1, invoiceRecord = 0;
            pageNoCashJournal = 1, gridPgnoCashJounal = 1, CashRecord = 0;
            pageNoPayment = 1, gridPgnoPayment = 1, paymentRecord = 0;
            isPaymentGridLoaded = false, isCashGridLoaded = false, isInvoiceGridLoaded = false;
            C_ConversionType_ID = 0;
            /********************************
         *  Load unallocated Payments
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            loadUnallocatedPayments();

            /********************************
         *  Load unallocated Cash lines
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            loadUnallocatedCashLines();

            loadInvoice();

            loadGLVoucher();
            displayGrids($allocationFrom.val(), $allocationTo.val());
        };

        /**Load all grids as blank */
        function blankAllGrids() {
            var isMultiCurrency = $vchkMultiCurrency.is(':checked');
            loadGLGrid();
            bindInvoiceGrid([], isMultiCurrency);
            bindPaymentGrid([], isMultiCurrency);
            bindCashline([], isMultiCurrency);
            $($(glLineGrid.box)[0]).find('.w2ui-search-down').css('margin-top', '4px');
            $($(glLineGrid.box)[0]).find('.w2ui-search-clear').css('margin-top', '4px');
            $('.vis-allocate-glSum').css('text-align', 'right');
            $('.vis-allocate-glSum').css('margin-right', '10px');
        };

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
            if (glLineGrid != undefined && glLineGrid != null) {
                glLineGrid.destroy();
                glLineGrid = null;
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