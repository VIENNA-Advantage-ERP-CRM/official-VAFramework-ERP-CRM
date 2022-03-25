/********************************************************
 * Module Name    : VIS
 * Purpose        : Print payment Selection
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     18 May 2015
 ******************************************************/
; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    function VPayPrint() {
        this.windowNo = VIS.Env.getWindowNo();
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        var $self = this;
        var $divContainer = null;
        var $cmbPaymentSelect = null;
        var $txtBankAccount = null;
        var $txtCurrentBalance = null;
        var $cmbPaymentMethod = null;
        var $txtCurreny = null;
        var $txtCheckNo = null;
        var $txtNoOfPayments = null;
        var $chkShowPrintedPayments = null;
        var $btnPrint = null;
        var $btnExport = null;
        var $btnCancel = null;
        var C_BankAccount_ID = null;
        var $divBusy = null;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";

        this.initialize = function () {
            customDesign();

        };
        //*****************
        //Load BusyDiv
        //*****************
        function busyIndicator() {
            $divBusy = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$divBusy.css({
            //    "position": "absolute", "bottom": "0", "background": "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat", "background-position": "center center",
            //    "width": "98%", "height": "98%", 'text-align': 'center', 'opacity': '.1', 'z-index': '9999999'
            //});
            $divBusy[0].style.visibility = "hidden";
            $root.append($divBusy);
        };
        //*****************
        //Show/Hide Busy Indicator
        //*****************
        function isBusy(show) {
            if (show) {
                $divBusy[0].style.visibility = "visible";
            }
            else {
                $divBusy[0].style.visibility = "hidden";
            }
        };
        //******************
        //Custom Design of Paymnet Selection Form
        //******************
        function customDesign() {
            $divContainer = $("<div class='vis-mainContainer vis-formouterwrpdiv'>");
            var designPSelectInfo = " <div class='vis-pPrintInfo'>"  // div pSelectInfo starts here                            
                + " <div class='vis-paymentselect-field' style='margin-top: 8px;'>"  // div PaySelection starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <div class='vis-paymentPrintChk-field'>"  // div ShowPrintedPayments starts here
                             + " <input type='checkbox' id='VIS_chkShowPrintPayment_" + $self.windowNo + "' data-placeholder='' placeholder=' '>"
                             + " <label for='VIS_chkOnlyDue_" + $self.windowNo + "'>" + VIS.Msg.getMsg("ShowPrintedPayments") + " </label>"
                             + " </div>" // div ShowPrintedPayments ends here 
                             + " <select id='VIS_PaySelection_" + $self.windowNo + "'></select>"
                             + " <label style='top: 11px;'>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_PaySelection_ID") + " </label>"
                             + "</div></div> </div>" // div PaySelection ends here 
                             + " <div class='vis-paymentPrint-field'>"  // div bankAccount starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='text' name='vis-name vis-fieldreadonly' class='vis-fieldreadonly' disabled id='VIS_txtBankAccount_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_BankAccount_ID") + " </label>"
                             + " </div></div></div>" // div bankAccount ends here 
                             + " <div class='vis-paymentPrint-field'>"  // div currentBalance starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtCurrentBal_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.getMsg("CurrentBalance") + " </label>"
                             + " </div></div></div>" // div currentBalance ends here                             
                             + " <div class='vis-paymentPrint-field'>"  // div paymentMethod starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select name='vis-name' id='VIS_cmbPaymentMethod_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "PaymentRule") + " </label>"
                             + " </div></div></div>" // div paymentMethod ends here 
                             + " <div class='vis-paymentPrint-field'>"  // div Currency starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtCurrency_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_Currency_ID") + " </label>"
                             + " </div></div></div>" // div Currency ends here                            
                             + " <div class='vis-paymentPrint-field'>"  // div CheckNo starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='number' name='vis-name'  id='VIS_txtCheckNo_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.getMsg("CheckNo") + " </label>"
                             + " </div></div></div>" // div CheckNo ends here    
                             + " <div class='vis-paymentPrint-field'>"  // div NoOfPayments starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtNoOfPayments_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.getMsg("NoOfPayments") + " </label>"
                             + " </div></div></div>" // div NoOfPayments ends here  
                             + " </div>" // div pSelectInfo ends here      

            var designPSelectProcess = " <div class='vis-pPrintProcess'>"  // div pSelectProcess starts here
                                     + " <div class='vis-paymentPrint-field'>"  // div starts here  
                                      + " <input id='VIS_btnCancel_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("Cancel") + "' >"
                                      //+ " <input id='VIS_btnExport_" + $self.windowNo + "' style='background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:right;width:100px;margin-top:10px;margin-left:10px;height:40px;' type='submit' value='" + VIS.Msg.getMsg("Export") + "' ></input>"
                                      + " <input id='VIS_btnPrint_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("Print") + "' ></input>"
                                     + " </div>" // div ends here 
                                     + " </div>" // div pSelectProcess ends here 
            $divContainer.append($(designPSelectInfo)).append($(designPSelectProcess));
            $root.append($divContainer);
            busyIndicator();
            findControls();

        };
        //******************
        //Find Controls through ID
        //******************
        function findControls() {
            $cmbPaymentSelect = $('#VIS_PaySelection_' + $self.windowNo);
            $txtBankAccount = $('#VIS_txtBankAccount_' + $self.windowNo);
            $txtCurrentBalance = $('#VIS_txtCurrentBal_' + $self.windowNo);
            $cmbPaymentMethod = $('#VIS_cmbPaymentMethod_' + $self.windowNo);
            $txtCurreny = $('#VIS_txtCurrency_' + $self.windowNo);
            $txtCheckNo = $('#VIS_txtCheckNo_' + $self.windowNo);
            $txtNoOfPayments = $('#VIS_txtNoOfPayments_' + $self.windowNo);
            $chkShowPrintedPayments = $('#VIS_chkShowPrintPayment_' + $self.windowNo);
            $chkShowPrintedPayments.attr("disabled", true)
            $btnPrint = $('#VIS_btnPrint_' + $self.windowNo);
            $btnExport = $('#VIS_btnExport_' + $self.windowNo);
            $btnCancel = $('#VIS_btnCancel_' + $self.windowNo);
            loadFormData(true);
            eventHandling();

        };
        //******************
        //Load Data on Form Load
        //******************
        function loadFormData(isFirstTime) {
            var pSelectID = null;
            if (isFirstTime) {
                pSelectID = 0;
            }
            else {
                pSelectID = $cmbPaymentSelect.val();
            }
            isBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/GetDetail",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { showPrintedPayment: $chkShowPrintedPayments.prop("checked"), C_PaymentSelect_ID: pSelectID, isFirstTime: isFirstTime },
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    controlsData = data;
                    if (data != null || data != undefined) {
                        fillData(data);
                        LoadPaymentRuleInfo();
                        isBusy(false);
                    }
                },
                error: function (e) {
                    isBusy(false);
                    console.log(e);
                }
            });
        };
        //******************
        //Load Data on Form Load
        //******************
        function LoadPaymentRuleInfo() {
            //debugger;
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/LoadPaymentRuleInfo",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text() },
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    isBusy(false);
                    if (data != null || data != undefined) {
                        $txtCheckNo.val(data.CheckNo);
                        $txtNoOfPayments.val(data.NoOfPayments);
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //Fill Data in Controls
        //******************
        function fillData(data) {
            try {
                //* Load PaymentSelection
                var pSelectID = $cmbPaymentSelect.val();
                var payMethodID = 0;
                if (data.PaymentSelection != null || data.PaymentSelection.length > 0) {
                    $cmbPaymentSelect.empty();
                    for (var i in data.PaymentSelection) {
                        $cmbPaymentSelect.append($('<Option value="' + data.PaymentSelection[i].ID + '">' + data.PaymentSelection[i].Value + '</option>'));
                    }
                }
                if (pSelectID > 0) {
                    $cmbPaymentSelect.val(pSelectID);
                    var sql = "VIS_135";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@pSelectID", pSelectID);

                    payMethodID = VIS.Utility.Util.getValueOfInt(executeScalar(sql, param));
                }
                //* Load PaymentMethod
                if (data.PaymentMethod != null || data.PaymentMethod.length > 0) {
                    $cmbPaymentMethod.empty();
                    for (var i in data.PaymentMethod) {
                        $cmbPaymentMethod.append($('<Option value="' + data.PaymentMethod[i].ID + '">' + data.PaymentMethod[i].Value + '</option>'));
                    }
                }
                if (payMethodID > 0) {
                    $cmbPaymentMethod.val(payMethodID);
                }
                //* Load Another Values
                if (data.PSelectInfo != null || data.PSelectInfo.length > 0) {
                    $txtBankAccount.val(data.PSelectInfo[0].BankAccount);
                    $txtCheckNo.val(data.PSelectInfo[0].CheckNo);
                    $txtCurrentBalance.val(parseFloat(data.PSelectInfo[0].CurrentBalance).toLocaleString());
                    $txtCurreny.val(data.PSelectInfo[0].Currency);
                    $txtNoOfPayments.val(data.PSelectInfo[0].NoOfPayments);
                    C_BankAccount_ID = data.PSelectInfo[0].BankAccount_ID;
                }
            }
            catch (err) {
                VIS.ADialog.info("VPayPrintNoRecords");
                isBusy(false);
                return false;

            }
            if ($cmbPaymentSelect.val() == null) {
                VIS.ADialog.info("VPayPrintNoRecords");
                return false;
            }
        };
        //******************
        //EventHandling
        //******************
        function eventHandling() {
            //**On click of Print Button**//
            $btnPrint.on("click", function () {
                if (validateInputOnPrint()) {
                    cmd_Print();
                }
            });
            $btnExport.on("click", function () {
                if (validateInputOnPrint()) {
                    cmd_Export();
                }
            });
            //**On click of Cancel Button**//
            $btnCancel.on("click", function () {
                $self.dispose();
            });

            //**On change of PaymentSelection($cmbPaymentSelect) **//
            $cmbPaymentSelect.on("change", function () {
                isBusy(true);
                window.setTimeout(function () {
                    loadFormData(false);
                    isBusy(false);
                }, 20);
            });

            //**On change of PaymentMethod($cmbPaymentMethod) **//
            $cmbPaymentMethod.on("change", function () {
                isBusy(true);
                window.setTimeout(function () {
                    LoadPaymentRuleInfo();
                    isBusy(false);
                }, 20);
            });

            //**On checked change of Show only Printed Payments checkbox **//
            $chkShowPrintedPayments.on("click", function () {
                isBusy(true);
                window.setTimeout(function () {
                    loadFormData(false);
                    isBusy(false);
                }, 20);
            });


        };
        //******************
        //validateInputOnPrint
        //******************
        function validateInputOnPrint() {
            try {
                if ($cmbPaymentSelect == null || $cmbPaymentSelect.val() <= -1 || C_BankAccount_ID <= -1 || $cmbPaymentMethod.val() <= -1 || $txtCheckNo.val().length <= 0) {
                    VIS.ADialog.info("VPayPrintNoRecords");
                    return false;
                }
            }
            catch (err) {
                VIS.ADialog.info("VPayPrintNoRecords");
                return false;
            }
            return true;
        };
        //******************
        //Cmd Print
        //******************
        function cmd_Print() {
            isBusy(true);

            // var printParent = new VIS.AProcess(300); //initlize AForm

            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/Cmd_Print",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text(), checkNo: $txtCheckNo.val() },
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {

                        VIS.ADialog.confirm("ContinueCheckPrint?", true, "", "Confirm", function (result) {
                            if (result) {
                                isBusy(true);
                                window.setTimeout(function () {
                                    continueCheckPrint(data);
                                }, 20);
                            }
                            else {
                                $self.dispose();
                            }
                        });
                    }
                    else {
                        VIS.ADialog.info("VPayPrintNoRecords");
                        return false;
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };

        //******************
        //ContinueCheckPrint
        //******************
        function continueCheckPrint(data) {
            var check_ID = data.check_ID[0];
            var param = [];
            param.push($cmbPaymentMethod.val());
            param.push($cmbPaymentSelect.val());
            param.push(C_BankAccount_ID);
            param.push($cmbPaymentMethod.text());
            param.push($txtCheckNo.val());
            param.push(JSON.stringify(data.check_ID));
            param.push(JSON.stringify(data.m_batch));
            param.push(JSON.stringify(data.m_checks));
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/ContinueCheckPrint",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                    // Change by mohit to get check prints.
                    var sql = "VIS_152";
                    var params = [];
                    params[0] = new VIS.DB.SqlParam("@BankAcct_ID", C_BankAccount_ID);
                    var ad_process_id = executeScalar(sql, params);
                    
                    sql = "VIS_150";
                    var ad_table_id = executeScalar(sql);

                    var prin = new VIS.APrint(ad_process_id, ad_table_id, parseInt($cmbPaymentSelect.val()), $self.windowNo);
                    prin.startPdf(null);
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        //get documenNo's
                        var paymentId = data.join();
                        var _docNo = getDocumentNo(paymentId);
                        //used get Message to get Value
                        //updated value appended to _docNo variable
                        _docNo = _docNo != null ? VIS.Msg.getMsg("VIS_PaymentCreated") + _docNo : _docNo;
                        VIS.ADialog.confirm("VPayPrintPrintRemittance?", true, _docNo, "Confirm", function (result) {
                            if (result) {
                                isBusy(true);
                                window.setTimeout(function () {
                                    vPayPrintPrintRemittance(jsonResult);
                                }, 20);
                            }
                            else {
                                $self.dispose();
                            }
                        });
                    }
                },
                error: function (e) {
                    isBusy(false);
                    console.log(e);
                }
            });
        };

        //Get the DocumentNo's for Payment
        function getDocumentNo(_paymentId) {
            var _docId = null;
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/GetDocumentNo",
                async: false,
                datatype: "Json",
                type: "GET",
                contentType: 'application/json; charset=utf-8',
                data: { paymentId: _paymentId },
                success: function (data) {
                    _docId = JSON.parse(data);
                    _docId = _docId != null ? _docId.join() : _docId;
                },
                error: function (e) {
                    console.log(e);
                }
            });
            return _docId;
        }
        //******************
        //VPayPrintPrintRemittance
        //******************$self.windowNo
        function vPayPrintPrintRemittance(data) {
            var paymentID = JSON.parse(data);
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/VPayPrintRemittance",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: data,
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (paymentID != null || paymentID != undefined) {
                        //var ad_process_id = VIS.DB.executeScalar("select ad_process_id from ad_process where value = 'remittanceprintformat'");
                        // Change by mohit to print remittance slip.
                        var sql = "VIS_151";

                        var ad_process_id = executeScalar(sql);


                        sql = "VIS_150";
                        var ad_table_id = executeScalar(sql);
                        // for (var j = 0; j < data.check_id; j++) {
                        //var pi = new VIS.ProcessInfo(null, ad_process_id, ad_table_id, paymentID[0]);
                        //pi.setAD_User_ID(VIS.context.getAD_User_ID());
                        //var ctl = new VIS.ProcessCtl($self, pi, null);
                        //ctl.setIsPdf(true);
                        //ctl.process($self.windowNo); //call dispose intenally
                        //ctl = null;
                        var prin = new VIS.APrint(ad_process_id, ad_table_id, parseInt($cmbPaymentSelect.val()), $self.windowNo);
                        prin.startPdf(null);
                        $self.dispose();
                        // }
                    }
                    else {
                        VIS.ADialog.info("PaymentNotProcessed");
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //Cmd Export
        //******************
        function cmd_Export() {
            isBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/Cmd_Export",
                async: false,
                datatype: "Json",
                type: "POST",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text(), checkNo: $txtCheckNo.val() },
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        VIS.ADialog.confirm("VPayPrintSuccess?", true, "", "Confirm", function (result) {
                            if (result) {
                                isBusy(true);
                                window.setTimeout(function () {
                                    vPayPrintSuccess(data);
                                }, 20);
                                var url = VIS.Application.contextUrl + "TempDownload/" + data.filepath;
                                window.open(url, '_blank');

                            }


                        });

                    }
                    else {
                        VIS.ADialog.info("VPayPrintNoRecords");
                        return false;
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //VPayPrintSuccess
        //******************
        function vPayPrintSuccess(data) {
            var param = [];
            param.push(JSON.stringify(data.check_ID));
            param.push(JSON.stringify(data.m_batch));
            param.push(JSON.stringify(data.m_checks));
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/VPayPrintSuccess",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {

                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //*******************
        //Get Root
        //*******************
        this.getRoot = function () {
            return $root;
        };




        var executeScalar = function (sql, params, callback) {
            var async = callback ? true : false;
            var dataIn = { sql: sql, page: 1, pageSize: 0 }
            var value = null;
            if (params) {
                dataIn.param = params;
            }
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

        function getDataSetJString(data, async, callback) {
            var result = null;
            //data.sql = VIS.secureEngine.encrypt(data.sql);
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
            });
            return result;
        };






        this.lockUI = function (abc) {

        };

        this.unlockUI = function (abc) {

        };


        //********************
        //Dispose Component
        //********************
        this.disposeComponent = function () {
            windowNo = null;
            $root = null;
            $self = this;

        };
    };

    //Must Implement with same parameter
    VPayPrint.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
        this.initialize();
    };

    //Must implement dispose
    VPayPrint.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
    VIS.Apps.AForms.VPayPrint = VPayPrint;
})(VIS, jQuery);