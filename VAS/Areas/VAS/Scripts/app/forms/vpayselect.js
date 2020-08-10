/********************************************************
 * Module Name    : VIS
 * Purpose        : Create payment Selection(Manual) Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     14 May 2015
 ******************************************************/
; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    function VPaySelect() {
        this.windowNo;
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        var $self = this;        
        var $divContainer = null;
        var $divPSelectInfo = null;
        var $divBankAccount = null;
        var $divCurrentBalance = null;
        var $divBusinessPartner = null;
        var $divOnlyDueInvoice = null;
        var $divPaymentDate = null;
        var $divPaymentMethod = null;
        var $divGridPSelect = null;
        var $divProcessPSelect = null;
        var $divLabel = null;
        var $divPaymentAmount = null;
        var $divProcessButtons = null;
        var $cmbBankAccount = null;
        var $cmbBusinessPartner = null;
        var $cmbPaymentMethod = null;
        var $cmbPaymentDate = null;
        var $lblCurrentBal = null;
        var $chkOnlyDueInvoice = null;
        var $txtCurrentBal = null;
        var $lblMsg = null;
        var $txtPaymentAmount = null;
        var arrListColumns = [];
        var dGrid = null;
        var $btnRequery = null;
        var $btnOk = null;
        var $btnCancel = null;
        var rows = [];
        var gridData = null;
        var controlsData = null;
        var paymentAmount = 0;
        var $divBusy = null;
        this.initialize = function () {
            busyIndicator();
            isBusy(true);
            window.setTimeout(function () {
                customDesign();
            }, 20);
            
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
            var height = ($(window).height())*(94/100);
            $divContainer = $("<div class='vis-mainContainer'>");
            var designPSelectInfo = " <div class='vis-pSelectInfo vis-leftsidebarouterwrap'>"  // div pSelectInfo starts here
                             +"<div class='vis-pSelectInner'>"
                             + " <div class='vis-paymentselect-field'>"  // div bankAccount starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbBankAccount_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_BankAccount_ID") + " </label>"
                             + "</div></div> </div>" // div bankAccount ends here 
                             + " <div class='vis-paymentselect-field'>"  // div currentBalance starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtCurrentBal_" + $self.windowNo + "' MaxLength='50' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.getMsg("CurrentBalance") + " </label>"
                             + "</div></div>  </div>" // div currentBalance ends here 
                             + " <div class='vis-paymentselect-field'>"  // div businessPartner starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbbusinessPartner_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID") + " </label>"
                             + "</div></div>  </div>" // div businessPartner ends here                             
                             + " <div class='vis-paymentselect-field'>"  // div paymentDate starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input  id='VIS_cmbPayDate_" + $self.windowNo + "'type='date' data-placeholder='' placeholder=' '>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "PayDate") + " </label>"
                             + "</div></div>  </div>" // div paymentDate ends here 
                             + " <div class='vis-paymentselect-field'>"  // div paymentMethod starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbPaymentMethod_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "PaymentRule") + " </label>"
                             + "</div></div>  </div>" // div paymentMethod ends here 
                             + " <div class='vis-paymentselect-field'>"  // div paymentAmount starts here
                             + '<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                + " <input type='number' id='VIS_txtPaymentAmount_" + $self.windowNo + "' disabled MaxLength='50' data-placeholder='' placeholder=' '>"
                + " <label>" + VIS.Msg.getMsg("PaymentAmount") + " </label>"
                             + "</div></div>  </div>" // div paymentAmount ends here 
                             + " <div class='vis-paymentselect-field'>"  // div onlyDueInvoice starts here
                + " <input type='checkbox' id='VIS_chkOnlyDue_" + $self.windowNo + "' data-placeholder='' placeholder=' '>"
                             + " <label for='VIS_chkOnlyDue_" + $self.windowNo + "'>" + VIS.Msg.getMsg("OnlyDue") + " </label>"
                             + "  </div>"  // div onlyDueInvoice starts here
                             + " <div class='vis-paymentselect-field'>"  // div lblSHowDetail starts here                          
                             + " <label for='VIS_lblShowDetail_" + $self.windowNo + "'></label>"
                             + " </div>" // div lblSHowDetail ends here 
                             + " </div>" // div vis-pSelectInner ends here 
                             + " <div class='vis-paymentselect-field'>"  // div lblSHowDetail starts here
                             + " <button class='VIS_Pref_btn-2' disabled id='VIS_btnRefresh_" + $self.windowNo + "' style='margin-top: 0px;'><i class='vis vis-refresh'></i></button>"
                             + " </div>" // btn ends here 
                             + " </div>" // div pSelectInfo ends here 

            $divGridPSelect = $("<div class='vis-pSelectIionGrid'>");

            var designPSelectProcess = " <div class='vis-pSelectProcess'>"  // div pSelectProcess starts here
                                     + " <div class='vis-paymentselect-field'>"  // div lblMsg starts here    
                                      + " <input id='VIS_btnCancel_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("Cancel") + "' ></input>"
                                     + " <input id='VIS_btnOK_" + $self.windowNo+"' disabled class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("OK") + "' ></input>"                                    
                                     + " </div>" // div pSelectButtons ends here 
                                     + " </div>" // div pSelectProcess ends here 
            $divContainer.append($(designPSelectInfo)).append($divGridPSelect).append($(designPSelectProcess));
            $root.append($divContainer);
            findControls();
           
                            
        };
        //******************
        //Find Controls through ID
        //******************
        function findControls() {
            $cmbBankAccount = $('#VIS_cmbBankAccount_' + $self.windowNo);
            $cmbBusinessPartner = $('#VIS_cmbbusinessPartner_' + $self.windowNo);
            $cmbPaymentMethod = $('#VIS_cmbPaymentMethod_' + $self.windowNo);
            $txtCurrentBal = $('#VIS_txtCurrentBal_' + $self.windowNo);
            $chkOnlyDueInvoice = $('#VIS_chkOnlyDue_' + $self.windowNo);
            $txtPaymentAmount = $('#VIS_txtPaymentAmount_' + $self.windowNo);
            $lblMsg = $('#VIS_lblShowDetail_' + $self.windowNo);
            $btnRequery = $('#VIS_btnRefresh_' + $self.windowNo);
            $cmbPaymentDate = $('#VIS_cmbPayDate_' + $self.windowNo);
            $btnOk = $('#VIS_btnOK_' + $self.windowNo);
            $btnCancel = $('#VIS_btnCancel_' + $self.windowNo);

            var now = new Date();
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);
            var today = now.getFullYear() + "-" + (month) + "-" + (day);
            $cmbPaymentDate.val(today);          
            loadGrid();
            eventHandling();
            loadFormData();
            if (controlsData.BankAccount.length > 0) {
                setCurrentBalance();
            }
            
        };
        //******************
        //EventHandling
        //******************
        function eventHandling() {

            //**On click of Requery Button**//
            $btnRequery.on("click", function () {
                isBusy(true);

                if (validateInput()) {
                    window.setTimeout(function () {
                        fetchGridRecords();
                        isBusy(false);
                    }, 20);
                }
            });

            //**On click of OK Button**//
            $btnOk.on("click", function () {               
                
                //Open Apyment Print form in CFrame
                if(dGrid.getSelection().length<=0)
                {
                    VIS.ADialog.info("PleaseSelectRecord");
                    
                    return false;
                }
              
                var selectedIndex = dGrid.getSelection();
                if (selectedIndex.length > 0) {
                    for (var i = 0; i < selectedIndex.length; i++) {
                        gridData[selectedIndex[i]].SELECT = true;
                    }
                    isBusy(true);
                    window.setTimeout(function () {
                        generatePaymentSelection();
                    }, 20);
                }
            });

            //**On click of Cancel Button**//
            $btnCancel.on("click", function () {
                $self.dispose();
            });

            //**On change of Bank Account($cmbBankAccount) **//
            $cmbBankAccount.on("change", function () {
                isBusy(true);
                window.setTimeout(function () {
                    getPaymentMethod();
                    setCurrentBalance();
                    isBusy(false);
                }, 20);
            });

            //**On checked change of Only Due Invoice checkbox **//
            $chkOnlyDueInvoice.on("click", function () {
                $btnRequery.trigger("click");
            });

           
        };        
        //******************
        //Load Grid 
        //******************
        function loadGrid()
        {
            try
            {
                if (dGrid != null) {
                    dGrid.destroy();
                    dGrid = null;
                }
            }
            catch(err)
            {
            }
            if (arrListColumns.length == 0) {
                //debugger;
                arrListColumns.push({ field: "SELECT", caption: VIS.Msg.getMsg("SELECT"), sortable: true, size: '100px',hidden:true });
                arrListColumns.push({ field: "C_INVOICE_ID", caption: VIS.Msg.getMsg("Invoice"), sortable: true, size: '100px' });
                arrListColumns.push({ field: "DUEDATE", caption: VIS.Msg.getMsg("DueDate"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val;
                        if (record.changes == undefined || record.changes.DUEDATE == undefined) {
                            val = record["DUEDATE"];
                        }
                        else {
                            val = record.changes.DUEDATE;
                        }
                        return new Date(val).toLocaleDateString();
                    }
                });
                arrListColumns.push({ field: "BUSINESSPARTNER", caption: VIS.Msg.getMsg("BusinessPartner"), sortable: true, size: '100px' });
                arrListColumns.push({ field: "DOCUMENTNO", caption: VIS.Msg.getMsg("Document_No"), sortable: true, size: '100px' });
                arrListColumns.push({ field: "CURRENCY", caption: VIS.Msg.getMsg("Currency"), sortable: true, size: '100px' });
                arrListColumns.push({ field: "GRANDTOTAL", caption: VIS.Msg.getMsg("GrandTotal"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val = record["GRANDTOTAL"];
                        return parseFloat(val).toLocaleString();
                    }
                });
                arrListColumns.push({ field: "DISCOUNTAMOUNT", caption: VIS.Msg.getMsg("DiscountAmt"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val = record["DISCOUNTAMOUNT"];
                        return parseFloat(val).toLocaleString();
                    }
                });
                arrListColumns.push({ field: "DISCOUNTDATE", caption: VIS.Msg.getMsg("DiscountDate"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val;
                        if (record.changes == undefined || record.changes.DISCOUNTDATE == undefined) {
                            val = record["DISCOUNTDATE"];
                        }
                        else {
                            val = record.changes.DISCOUNTDATE;
                        }
                        return new Date(val).toLocaleDateString();
                    }
                });
                arrListColumns.push({ field: "AMOUNTDUE", caption: VIS.Msg.getMsg("AmountDue"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val = record["AMOUNTDUE"];
                        return parseFloat(val).toLocaleString();
                    }
                });
                arrListColumns.push({ field: "PAYMENTAMOUNT", caption: VIS.Msg.getMsg("PaymentAmount"), sortable: true, size: '100px',
                    render: function (record, index, col_index) {
                        var val = record["PAYMENTAMOUNT"];
                        return parseFloat(val).toLocaleString();
                    }
                });
            }
          
            dGrid=$divGridPSelect.w2grid({
                name: "gridPaymentSelection_" + $self.windowNo,
                recordHeight: 40,
                show: { selectColumn: true },
                multiSelect: true,
                columns: arrListColumns,
                records: rows
            });

            //**On GridViewrow selected  **//
            dGrid.on('select', function (event) {
                paymentAmount += VIS.Utility.Util.getValueOfDecimal(rows[event.index].PAYMENTAMOUNT);
                $txtPaymentAmount.val(paymentAmount);
                var selectedIndex = dGrid.getSelection();
                //if (selectedIndex.length > 0) {
                    $btnOk.removeAttr("disabled");
               // }

                
            });
            //**On GridViewrow selected  **//
            dGrid.on('unselect', function (event) {
                paymentAmount -= VIS.Utility.Util.getValueOfDecimal(rows[event.index].PAYMENTAMOUNT);
                $txtPaymentAmount.val(paymentAmount);
                var selectedIndex = dGrid.getSelection();
                //if (selectedIndex.length > 0) {
                //    $btnOk.removeAttr("disabled");
                //}
                //else {
                //    $btnOk.attr("disabled");
                //}
            });
        };
        //********************
        //Fetch Grid Records
        //*********************
        function fetchGridRecords() {
            rows = [];
            var count = 0;
            var param = [];
            param.push($cmbBankAccount.val());
            if (controlsData != null) {
                var selectedBackAccount = jQuery.grep(controlsData.BankAccount, function (value) {
                    return value.ID == VIS.Utility.Util.getValueOfInt($cmbBankAccount.val())
                });
                if (selectedBackAccount.length > 0) {
                    param.push(selectedBackAccount[0].Currency_ID);
                }
                else {
                    param.push(0);
                }
            }
            else {
                param.push(0);
            }           
            param.push($cmbBusinessPartner.val());
            param.push($cmbPaymentDate.val());
            param.push($cmbPaymentMethod.val());
            param.push($txtPaymentAmount.val());
            param.push($chkOnlyDueInvoice.prop("checked"));
            $.ajax({
                url: VIS.Application.contextUrl + "VPaySelect/GetGridRecords",
                async: false,
                datatype: "Json",
                type: "POST",
                cache: false,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    rows = [];
                    if (data != null || data != undefined) {
                        gridData = data;
                        for (i = 0; i < data.length; i++) {
                            var line = {};
                            line['SELECT'] = data[i].SELECT;
                            line['C_INVOICE_ID'] = data[i].C_INVOICE_ID;
                            line['DUEDATE'] = data[i].DUEDATE;
                            line['BUSINESSPARTNER'] = data[i].BUSINESSPARTNER;
                            line['DOCUMENTNO'] = data[i].DOCUMENTNO;
                            line['CURRENCY'] = data[i].CURRENCY;
                            line['GRANDTOTAL'] = data[i].GRANDTOTAL;
                            line['DISCOUNTAMOUNT'] = data[i].DISCOUNTAMOUNT;
                            line['DISCOUNTDATE'] = data[i].DISCOUNTDATE;
                            line['AMOUNTDUE'] = data[i].AMOUNTDUE;
                            line['PAYMENTAMOUNT'] = data[i].PAYMENTAMOUNT;
                            line['recid'] = count;
                            count++;
                            rows.push(line)
                        }
                    }
                    else {
                       
                        
                    }


                },
                error: function (e) {
                    console.log(e);
                }

            });
            //if (gridData.length > 0) {
                loadGrid();
           // }
        };
        //******************
        //Load Data on Form Load
        //******************
        function loadFormData() {
            $.ajax({
                url: VIS.Application.contextUrl + "VPaySelect/GetDetail",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,               
                success: function (jsonResult) {                   
                    var data = JSON.parse(jsonResult);
                    controlsData = data;                   
                    if (data != null || data != undefined) {
                        if (data.BankAccount.length > 0) {
                            $btnRequery.removeAttr("disabled");
                            fillData(data);
                        }
                        }
                    isBusy(false);
                    
                },
                error: function (e) {
                    console.log(e);
                    isBusy(false);
                }
            });
        };
        //******************
        //Fill Data in Controls
        //******************
        function fillData(data) {
            //* Load BankAccount
            if (data.BankAccount != null || data.BankAccount.length > 0) {
                $cmbBankAccount.empty();
                for (var i in data.BankAccount) {
                    $cmbBankAccount.append($('<Option value="' + data.BankAccount[i].ID + '">' + data.BankAccount[i].Value + '</option>'));
                }
            }
            //* Load BusinessPartner
            if (data.BusinessPartner != null || data.BusinessPartner.length > 0) {
                $cmbBusinessPartner.empty();
                $cmbBusinessPartner.append($('<Option value="-1"></option>'));
                for (var i in data.BusinessPartner) {
                    $cmbBusinessPartner.append($('<Option value="' + data.BusinessPartner[i].ID + '">' + data.BusinessPartner[i].Value + '</option>'));
                }
            }
            //* Load PaymentMethod
            if (data.PaymentMethod != null || data.PaymentMethod.length > 0) {
                $cmbPaymentMethod.empty();
                for (var i in data.PaymentMethod) {
                    $cmbPaymentMethod.append($('<Option value="' + data.PaymentMethod[i].ID + '">' + data.PaymentMethod[i].Value + '</option>'));
                }
            }
            isBusy(false);
        };
        //******************
        //Get Payment Method
        //******************
        function getPaymentMethod() {
            $.ajax({
                url: VIS.Application.contextUrl + "VPaySelect/GetPaymentMethod",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { C_BankAccount_ID: parseInt($cmbBankAccount.val()) },
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        //* Load PaymentMethod
                        if (data != null || data.length > 0) {
                            $cmbPaymentMethod.empty();
                            for (var i in data) {
                                $cmbPaymentMethod.append($('<Option value="' + data[i].ID + '">' + data[i].Value + '</option>'));
                            }
                        }
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //Set Current Balance
        //******************
        function setCurrentBalance() {
            var selectedBackAccount = jQuery.grep(controlsData.BankAccount, function (value) {
                return value.ID == VIS.Utility.Util.getValueOfInt($cmbBankAccount.val())
            });
            if (selectedBackAccount.length > 0) {
                $txtCurrentBal.val(selectedBackAccount[0].CurrentBalance + " " + selectedBackAccount[0].ISOCode);
            }
        };
        //*******************
        //Validate Input
        //*******************
        function validateInput() {
            if ($cmbBusinessPartner != null && $cmbBusinessPartner.val() <= 0) {
                VIS.ADialog.info("SelectBusinessPartnerFirst");
                isBusy(false);
                return false;
            }
            return true;
        };
        //*******************
        //Generate Payment Selection
        //*******************
        function generatePaymentSelection() {
            var param = [];
            param.push($cmbBankAccount.val());
            if(controlsData!=null)
            {
                var selectedBackAccount = jQuery.grep(controlsData.BankAccount, function (value) {
                    return value.ID == VIS.Utility.Util.getValueOfInt($cmbBankAccount.val())
                });
                param.push(selectedBackAccount[0].Currency_ID);
            }
            else
            {
                param.push(0);
            }
           
            param.push($cmbBusinessPartner.val());
            param.push($cmbPaymentDate.val());
            param.push($cmbPaymentMethod.val());
            param.push($txtPaymentAmount.val());
            param.push($chkOnlyDueInvoice.prop("checked"));
            param.push(JSON.stringify(gridData));
            $.ajax({
                url: VIS.Application.contextUrl + "VPaySelect/GeneratePaymentSelection",
                async: false,
                datatype: "Json",
                type: "POST",
                cache: false,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                   var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        //Open Apyment Print form in CFrame
                        $self.dispose();
                         var c = new VIS.CFrame();
                        var paymentPrint = new VIS.Apps.AForms.VPayPrint();
                        c.setName(VIS.Msg.getMsg("Payment Print/Export"));
                        c.setTitle(VIS.Msg.getMsg("Payment Print/Export"));
                        c.hideHeader(false);
                        c.setContent(paymentPrint);                       
                        c.show();
                        paymentPrint.initialize();                       
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
        //********************
        //Dispose Component
        //********************
        this.disposeComponent=function() {
            windowNo = null;
            $root = null;
            $self = this;
            $divContainer = null;
            $divPSelectInfo = null;
            $divBankAccount = null;
            $divCurrentBalance = null;
            $divBusinessPartner = null;
            $divOnlyDueInvoice = null;
            $divPaymentDate = null;
            $divPaymentMethod = null;
            $divGridPSelect = null;
            $divProcessPSelect = null;
            $divLabel = null;
            $divPaymentAmount = null;
            $divProcessButtons = null;
            $cmbBankAccount = null;
            $cmbBusinessPartner = null;
            $cmbPaymentMethod = null;
            $cmbPaymentDate = null;
            $lblCurrentBal = null;
            $chkOnlyDueInvoice = null;
            $txtCurrentBal = null;
            $lblMsg = null;
            $txtPaymentAmount = null;
            arrListColumns = null;
            dGrid = null;
            $btnRequery = null;
            $btnOk = null;
            $btnCancel = null;
            rows = null;
        };
    };

    //Must Implement with same parameter
    VPaySelect.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
        this.initialize();
    };

    //Must implement dispose
    VPaySelect.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
    VIS.Apps.AForms.VPaySelect = VPaySelect;
})(VIS, jQuery);