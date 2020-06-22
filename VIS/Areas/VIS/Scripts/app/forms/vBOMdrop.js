/********************************************************
 * Module Name    : VIS
 * Purpose        : Create BOM Drop Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     14 May 2015
 ******************************************************/
; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    function VBOMDrop() {
        this.windowNo = null;
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        var $self = this;
        var $divContainer = null;
        var $divPInfo = null;
        var $divBOMLines = null;
        var $divProduct = null;
        var $divInvoice = null;
        var $divOrder = null;
        var $divOpportunity = null;
        var $divQuantity = null;       
        var $divProcessBOM = null;      
        var $cmbProduct = null;
        var $cmbInvoice = null;
        var $cmbOrder = null;
        var $cmbOpportunity= null;     
        var $txtQuantity = null;
        var $btnOK = null;
        var $btnCancel = null;
        var $divBusy = null;
        var BOMLinesData = [];
        var $btnRequery = null;
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
        //Custom Design of BOM Drop Form
        //******************
        function customDesign() {
            var height = ($(window).height()) * (92 / 100);
            $divContainer = $("<div class='vis-mainContainer'>");
            var designPInfo = " <div class='vis-pSelectInfo vis-leftsidebarouterwrap'>"  // div pSelectInfo starts here
                             +"<div class='vis-pSelectInner'>"
                             + " <div class='vis-paymentselect-field'>"  // div Product starts here
                             +'<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbProduct_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "M_Product_ID") + " </label>"
                             +"</div></div>"
                             + " </div>" // div Product ends here 
                             + " <div class='vis-paymentselect-field'>"  // div Invoice starts here
                             +'<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbinvoice_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_Invoice_ID") + " </label>"
                             +"</div></div>"
                             + " </div>" // div Invoice ends here 
                             + " <div class='vis-paymentselect-field'>"  // div Order starts here
                             +'<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbOrder_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_Order_ID") + " </label>"
                             +"</div></div>"
                             + " </div>" // div Order ends here 
                             + " <div class='vis-paymentselect-field'>"  // div Project/Opportunity starts here
                             +'<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <select id='VIS_cmbOpportunity_" + $self.windowNo + "'></select>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_Project_ID") + " </label>"
                             +"</div></div>"
                             + " </div>" // div Project/Opportunity ends here                             
                             + " <div class='vis-paymentselect-field'>"  // div Quantity starts here
                             +'<div class="input-group vis-input-wrap"><div class="vis-control-wrap">'
                             + " <input data-placeholder='' placeholder=' ' type='number' id='VIS_txtQuantity_" + $self.windowNo + "' min='0' MaxLength='50'>"
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Qty") + "</label>"
                             +"</div></div>"
                             + " </div>" // div Quantity ends here 
                             + " </div>"// div vis-pSelectInner ends here
                             + " <div class='vis-paymentselect-field'>"  // div btnRefresh starts here
                             + " <button class='VIS_Pref_btn-2'  id='VIS_btnRefresh_" + $self.windowNo + "' style='margin-top: 0px;'><i class='vis vis-refresh'></i></button>"
                             + " </div>" // div btnRefresh ends here 
                             + " </div>" // div pSelectInfo ends here 
            $divBOMLines = $("<div class='vis-pSelectIionGrid'>");

            var designPProcess = " <div class='vis-pSelectProcess'>"  // div pSelectProcess starts here
                                     + " <div class='vis-paymentselect-field'>"  // div starts here    
                + " <input id='VIS_btnCancel_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("Cancel") + "' ></input>"
                + " <input id='VIS_btnOK_" + $self.windowNo + "' disabled class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("OK") + "' ></input>"
                                     + " </div>" // div pSelectButtons ends here 
                                     + " </div>" // div pSelectProcess ends here 
            $divContainer.append($(designPInfo)).append($divBOMLines).append($(designPProcess));
            $root.append($divContainer);
            findControls();


        };
        //******************
        //Find Controls through ID
        //******************
        function findControls() {
            $cmbProduct = $('#VIS_cmbProduct_' + $self.windowNo);
            $cmbInvoice = $('#VIS_cmbinvoice_' + $self.windowNo);
            $cmbOrder = $('#VIS_cmbOrder_' + $self.windowNo);
            $cmbOpportunity = $('#VIS_cmbOpportunity_' + $self.windowNo);
            $txtQuantity = $('#VIS_txtQuantity_' + $self.windowNo);
            $btnCancel = $('#VIS_btnCancel_' + $self.windowNo);
            $btnOK = $('#VIS_btnOK_' + $self.windowNo);
            $btnRequery = $('#VIS_btnRefresh_' + $self.windowNo);
            loadFormData();
            eventHandling();

        };
        //******************
        //EventHandling
        //******************
        function eventHandling() {

           
            //**On click of Requery Button**//
            $btnRequery.on("click", function () {
                isBusy(true);
                $divBOMLines.empty();
                window.setTimeout(function () {
                    loadFormData();
                    isBusy(false);
                }, 20);
            });


            //**On click of OK Button**//
            $btnOK.on("click", function () {
                //isBusy(true);
                //window.setTimeout(function () {
                    saveBOMLines();
                //    isBusy(false);
                //}, 20);
            });

            //**On click of Cancel Button**//
            $btnCancel.on("click", function () {
                $self.dispose();
            });

            //**On change of Product($cmbProduct) **//
            $cmbProduct.on("change", function () {
                if ($cmbProduct.val() > 0) {
                    $btnOK.removeAttr("disabled");
                }
                else {
                    $btnOK.attr("disabled", true);
                }
                isBusy(true);
                window.setTimeout(function () {
                    getBOMLines();
                }, 20);
            });

            //**On change of Invoice($cmbProduct) **//
            $cmbInvoice.on("change", function () {
               
                if ($cmbInvoice.val() > 0) {
                    if ($cmbProduct.val() > 0) {
                        $btnOK.removeAttr("disabled");
                    }
                    $cmbOrder.attr("disabled", true);
                    $cmbOpportunity.attr("disabled", true);
                }
                else
                {
                    $btnOK.attr("disabled", true);
                    $cmbOrder.removeAttr("disabled");
                    $cmbOpportunity.removeAttr("disabled");
                }
            });

            //**On change of Order($cmbOrder) **//
            $cmbOrder.on("change", function () {
                
                if ($cmbOrder.val() > 0) {
                    if ($cmbProduct.val() > 0) {
                        $btnOK.removeAttr("disabled");
                    }
                    $cmbInvoice.attr("disabled", true);
                    $cmbOpportunity.attr("disabled", true);
                }
                else {
                    $btnOK.attr("disabled", true);
                    $cmbInvoice.removeAttr("disabled");
                    $cmbOpportunity.removeAttr("disabled");
                }
            });

            //**On change of Opportunity($cmbOpportunity) **//
            $cmbOpportunity.on("change", function () {
                
                if ($cmbOpportunity.val() > 0) {
                    if ($cmbProduct.val() > 0) {
                        $btnOK.removeAttr("disabled");
                    }
                    $cmbOrder.attr("disabled", true);
                    $cmbInvoice.attr("disabled", true);
                }
                else {
                    $btnOK.attr("disabled",true);
                    $cmbOrder.removeAttr("disabled");
                    $cmbInvoice.removeAttr("disabled");
                }
            });

            //** On text changed of Quantity
            $txtQuantity.on("change", function () {
                for(var i=0;i<$($divBOMLines[0]).children().length;i++)
                {
                    var prevValue = BOMLinesData[i].BOMQty;
                    $($($($($divBOMLines[0]).children()[i]).children()[0]).children()[1]).attr("Value",prevValue*$txtQuantity.val());
                }
            });


        }; 
        //******************
        //Load Data on Form Load
        //******************
        function loadFormData() {
            $.ajax({
                url: VIS.Application.contextUrl + "VBOMDrop/GetDetail",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    controlsData = data;
                    if (data != null || data != undefined) {
                                         
                            fillData(data);
                       
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
            //* Load Product
            if (data.Product != null || data.Product.length > 0) {
                $cmbProduct.empty();
                $cmbProduct.append($('<Option value="-1"></option>'));
                for (var i in data.Product) {
                    $cmbProduct.append($('<Option value="' + data.Product[i].ID + '">' + data.Product[i].Value + '</option>'));
                }
            }

            //* Load Invoice
            if (data.Invoice != null || data.Invoice.length > 0) {
                $cmbInvoice.empty();
                $cmbInvoice.append($('<Option value="-1"></option>'));
                for (var i in data.Invoice) {
                    $cmbInvoice.append($('<Option value="' + data.Invoice[i].ID + '">' + data.Invoice[i].Value + '</option>'));
                }
            }

            //* Load Order
            if (data.Order != null || data.Order.length > 0) {
                $cmbOrder.empty();
                $cmbOrder.append($('<Option value="-1"></option>'));
                for (var i in data.Order) {
                    $cmbOrder.append($('<Option value="' + data.Order[i].ID + '">' + data.Order[i].Value + '</option>'));
                }
            }

            //* Load Opportunity
            if (data.Opportunity != null || data.Opportunity.length > 0) {
                $cmbOpportunity.empty();
                $cmbOpportunity.append($('<Option value="-1"></option>'));
                for (var i in data.Opportunity) {
                    $cmbOpportunity.append($('<Option value="' + data.Opportunity[i].ID + '">' + data.Opportunity[i].Value + '</option>'));
                }
            }            
            isBusy(false);
        };
        //******************
        //Get BOM Lines
        //******************
        function getBOMLines() {
            $txtQuantity.val("");
            $.ajax({
                url: VIS.Application.contextUrl + "VBOMDrop/GetBOMLines",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data:{productID:parseInt($cmbProduct.val())},
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    controlsData = data;
                    if (data != null || data != undefined) {
                        debugger;
                        createBOMLines(data);
                    }
                    else {
                        $divBOMLines.empty();
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
        //Create BOM Lines
        //******************
        function createBOMLines(data) {
            var BOMlines = "";
            $divBOMLines.empty();
            var chkbox = "";
            var txtqty = "";
            var isAlternate = false;
            BOMLinesData = data;
            for (var i = 0; i < data.length; i++) {
                BOMlines = "";
                if (data[i].BOMType == 'P') {
                    chkbox = "<input type='checkbox' checked disabled id='VIS_chkBOMLine_" + i + "' style='height: 15px;width: auto;'></input>"
                    txtqty = "<input type='number' id='VIS_qtyBOMLine_" + i + "' style='background: pink;width:200%;' min='0' value='" + data[i].BOMQty + "'></input>";
                }
                else if (data[i].BOMType == 'O') {
                    chkbox = "<input type='checkbox' id='VIS_chkBOMLine_" + i + "' style='height: 15px;width: auto;'></input>";
                    txtqty = "<input type='number' disabled id='VIS_qtyBOMLine_" + i + "' style='background: #FFF5FF;width:200%;'  min='0' value='" + data[i].BOMQty + "'></input>";

                }
                else {
                    if (!isAlternate) {
                        chkbox = " <input type='radio' name='vis-radiolist' checked id='VIS_chkBOMLine_" + i + "' style='height: 15px;width: auto;'></input>"
                        txtqty = "<input type='number' id='VIS_qtyBOMLine_" + i + "' style='background: pink;width:200%;' min='0' value='" + data[i].BOMQty + "'></input>";
                    }
                    else {
                        chkbox = " <input type='radio' name='vis-radiolist' id='VIS_chkBOMLine_" + i + "' style='height: 15px;width: auto;'></input>";
                        txtqty = "<input type='number' disabled id='VIS_qtyBOMLine_" + i + "' style='background: #FFF5FF;width:200%;' min='0' value='" + data[i].BOMQty + "'></input>";
                    }
                }
                BOMlines += "<div class='vis-bomlinesdata'>"
                          + " <div style='float:left;width: 50%;'>"
                          + " <label style='margin: 5px;width:100%;'>" + data[i].ProductName + "</label>"
                          + txtqty
                         + " </div>"
                          + " <div style='float:right;'>"
                          + chkbox
                          + " <label style='margin-right:5px;'>" + data[i].BOMTypeName + "</label>"
                          + " </div>"                         
                          + " </div>";
                $divBOMLines.append($(BOMlines));
                $("#VIS_chkBOMLine_" + i).on("click", function (e) {
                    var $txtbxQty=$($($($(this).parent()).parent().children()[0]).children()[1]);
                    if ($(this).prop("checked")) {
                        $txtbxQty.css("background", "pink");
                        $txtbxQty.removeAttr("disabled");

                    }
                    else {
                        $txtbxQty.css("background", "#FFF5FF");
                        $txtbxQty.attr("disabled", true);
                    }
                });
            }
            
        };
        //*******************
        //Save BOM Lines in DB
        //*******************
        function saveBOMLines() {
            var param = [];
            param.push($cmbProduct.val());
            param.push($cmbInvoice.val());
            param.push($cmbOrder.val());
            param.push($cmbOpportunity.val());
            for (var i = 0; i < $($divBOMLines[0]).children().length; i++) {                
                var isselected = $($($($($divBOMLines[0]).children()[i]).children()[1]).children()[0]).prop("checked");
                if (isselected) {
                    BOMLinesData[i].IsSelected = true;
                    BOMLinesData[i].BOMQty = $($($($($divBOMLines[0]).children()[i]).children()[0]).children()[1]).val();
                }
                else
                {
                    BOMLinesData[i].IsSelected = false;
                }
            }
            param.push(JSON.stringify(BOMLinesData));
            $.ajax({
                url: VIS.Application.contextUrl + "VBOMDrop/SaveRecord",
                async: false,
                datatype: "Json",
                type: "POST",
                cache: false,
                contentType: 'application/json; charset=utf-8',
                data:JSON.stringify(param),
                success: function (jsonResult) {
                    if (JSON.parse(jsonResult)) {
                        $self.dispose();
                    }
                    else {
                        VIS.ADialog.info("RecordNotSaved");
                    }
                },
                error:function(e)
                {
                    console.log(e);
                    isBusy(false);
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
        this.disposeComponent = function () {
            $root = null;
            $self = null;
            $divContainer = null;
            $divPInfo = null;
            $divBOMLines = null;
            $divProduct = null;
            $divInvoice = null;
            $divOrder = null;
            $divOpportunity = null;
            $divQuantity = null;
            $divProcessBOM = null;
            $cmbProduct = null;
            $cmbInvoice = null;
            $cmbOrder = null;
            $cmbOpportunity = null;
            $txtQuantity = null;
            $btnOK = null;
            $btnCancel = null;
            $divBusy = null;
            BOMLinesData = null;
        };
    };

    //Must Implement with same parameter
    VBOMDrop.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
        this.initialize();
    };

    //Must implement dispose
    VBOMDrop.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
    VIS.Apps.AForms.VBOMDrop = VBOMDrop;
})(VIS, jQuery);