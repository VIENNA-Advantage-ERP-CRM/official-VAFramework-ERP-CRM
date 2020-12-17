/********************************************************
 * Module Name    : VIS
 * Purpose        : Material Transaction Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     14 May 2015
 ******************************************************/
; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    function VTrxMaterial() {
        this.windowNo = null;
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        var $self = this;
        var $divContainer = null;
        var $divPSelectInfo = null;
        var $divGridPSelect = null;
        var $divProcessPSelect = null;
        var $divLabel = null;
        var $divPaymentAmount = null;
        var $divProcessButtons = null;
        var $cmbOrganization = null;
        var $cmbMoventType = null;
        var $cmbLocator = null;
        var $cmbProduct = null;
        var $dtpFromDate = null;
        var $dtpToDate = null;
        var arrListColumns = [];
        var dGrid = null;
        var $btnRequery = null;
        var $btnZoom = null;
        var $btnOk = null;
        var $btnCancel = null;
        var rows = [];
        var gridData = null;
        var controlsData = null;
        var paymentAmount = 0;
        var $divBusy = null;
        var $locator = null;
        var $product = null;
        var gridController = null;
        var query = null;
        var _mTab = null;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";

        this.initialize = function () {
            busyIndicator();
            isBusy(true);
            window.setTimeout(function () {
                customDesign();
                isBusy(false);
            }, 20);

        };
        //*****************
        //Load BusyDiv
        //*****************
        function busyIndicator() {
            $divBusy = $('<div class="vis-busyindicatorouterwrap" style="visibility: hidden;"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
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
            var height = ($(window).height()) * (96 / 100);


            var lookupOrg = VIS.MLookupFactory.get(VIS.Env.getCtx(), 0, 0, VIS.DisplayType.TableDir, "AD_Org_ID", 0, false, " AD_Org.AD_Org_ID <> 0 AND AD_Org.IsSummary='N' AND AD_Org.IsActive='Y' AND AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N' ");
            $cmbOrganization = new VIS.Controls.VComboBox("AD_Org_ID", false, false, true, lookupOrg, 50);

            // var lookupMoventType = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 3666, VIS.DisplayType.List, "MovementType",189, false, null);
            var lookupMoventType = VIS.MLookupFactory.get(VIS.Env.getCtx(), 0, 0, VIS.DisplayType.List, "MovementType", 189, false, null);
            $cmbMoventType = new VIS.Controls.VComboBox("MovementType", false, false, true, lookupMoventType, 29);

            //*************Lookup for Locator************            
            //var containerdivLocator = $('<div class="d-flex"></div>');
            var $divLocatorInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divLocatorCtrlwrap = $("<div class='vis-control-wrap'>");
            var $divLocatorBtnwrap = $("<div class='input-group-append'>");
            var lookupLocator = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, "M_Locator_ID", 0, false, "");
            $locator = new VIS.Controls.VTextBoxButton("M_Locator_ID", false, false, true, VIS.DisplayType.Search, lookupLocator);
            $divLocatorInputwrap.append($divLocatorCtrlwrap);
            $divLocatorCtrlwrap.append($locator.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            $divLocatorInputwrap.append($divLocatorBtnwrap);
            $divLocatorBtnwrap.append($locator.getBtn(0));
            $divLocatorBtnwrap.append($locator.getBtn(1));
            // $locator.getControl().css("width", "73%");
            //$locator.getControl().addClass("vis-lookupTextboxes");
            $locator.getBtn(0).addClass("vis-lookupButtons");
            $locator.getBtn(1).addClass("vis-lookupButtons");
            // $locator.getBtn(0).css("float", "left");
            //  $locator.getBtn(1).css("float", "right");
            //$locator.getBtn(0).css("height", "30");
            //$locator.getBtn(0).css("width", "30");
            //$locator.getBtn(1).css("width", "30");
            //$locator.getBtn(1).css("height", "30");
            //$locator.getBtn(0).css("padding", "0px");
            //$locator.getBtn(1).css("padding", "0px");
            //*******LookUp Locator Ends**************
            //*************Lookup for Product************            
            //var containerdivProduct = $('<div class="d-flex"></div>');
            var $divProductInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divProductCtrlwrap = $("<div class='vis-control-wrap'>");
            var $divProductBtnwrap = $("<div class='input-group-append'>");
            var lookupProduct = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, "M_Product_ID", 0, false, "");
            $product = new VIS.Controls.VTextBoxButton("M_Product_ID", false, false, true, VIS.DisplayType.Search, lookupProduct);
            $divProductInputwrap.append($divProductCtrlwrap);
            $divProductCtrlwrap.append($product.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            $divProductInputwrap.append($divProductBtnwrap);
            $divProductBtnwrap.append($product.getBtn(0));
            $divProductBtnwrap.append($product.getBtn(1));
            // $product.getControl().css("width", "73%");
            //$product.getControl().addClass("vis-lookupTextboxes");
            $product.getBtn(0).addClass("vis-lookupButtons");
            $product.getBtn(1).addClass("vis-lookupButtons");
            //$product.getBtn(0).css("float", "left");
            //$product.getBtn(1).css("float", "right");
            //$product.getBtn(0).css("height", "30");
            //$product.getBtn(0).css("width", "30");
            //$product.getBtn(0).css("padding", "0px");
            //$product.getBtn(1).css("height", "30");
            //$product.getBtn(1).css("width", "30");
            //$product.getBtn(0).css("padding", "0px");
            //$product.getBtn(1).css("padding", "0px");
            //*******LookUp Product Ends**************
            $divContainer = $("<div class='vis-mainContainer'>");

            var $divInfo = $("<div class='vis-pSelectInfo vis-leftsidebarouterwrap'>");
            var $divInfoinn = $("<div class='vis-pSelectInner'>");

            $divInfo.append($divInfoinn);

            var $divOrganization = $("<div class='vis-paymentselect-field'>");
            var $divOrganizationInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divOrganizationCtrlwrap = $("<div class='vis-control-wrap'>");
            $divOrganization.append($divOrganizationInputwrap);
            $divOrganizationInputwrap.append($divOrganizationCtrlwrap);
            $divOrganizationCtrlwrap.append($cmbOrganization.getControl());
            $divOrganizationCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "AD_Org_ID") + " </label>"));
            $divInfoinn.append($divOrganization);

            var $divMovementType = $("<div class='vis-paymentselect-field'>");
            var $divMovementTypeInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divMovementTypeCtrlwrap = $("<div class='vis-control-wrap'>");
            $divMovementType.append($divMovementTypeInputwrap);
            $divMovementTypeInputwrap.append($divMovementTypeCtrlwrap);
            $divMovementTypeCtrlwrap.append($cmbMoventType.getControl());
            $divMovementTypeCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "MovementType") + " </label>"));
            $divInfoinn.append($divMovementType);

            var $divLocator = $("<div class='vis-paymentselect-field'>");
            $divLocator.append($divLocatorInputwrap);            
            $divLocatorCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "M_Locator_ID") + " </label>"));
            $divInfoinn.append($divLocator);

            var $divProduct = $("<div class='vis-paymentselect-field'>");
            $divProduct.append($divProductInputwrap);  
            $divProductCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "Product") + " </label>"));
            //$divProduct.append(containerdivProduct);
            $divInfoinn.append($divProduct);

            var $divFromDate = $("<div class='vis-paymentselect-field'>");
            var $divFromDateInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divFromDateCtrlwrap = $("<div class='vis-control-wrap'>");
            $divFromDateCtrlwrap.append($("<input  id='VIS_dtpFromDate" + $self.windowNo + "'type='date'>"));
            $divFromDateCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "DateFrom") + " </label>"));
            $divFromDate.append($divFromDateInputwrap);
            $divFromDateInputwrap.append($divFromDateCtrlwrap);
            $divInfoinn.append($divFromDate);

            var $divToDate = $("<div class='vis-paymentselect-field'>");
            var $divToDateInputwrap = $("<div class='input-group vis-input-wrap'>");
            var $divToDateCtrlwrap = $("<div class='vis-control-wrap'>");
            $divToDateCtrlwrap.append($("<input  id='VIS_dtpToDate" + $self.windowNo + "'type='date'>"));
            $divToDateCtrlwrap.append($("<label>" + VIS.Msg.translate(VIS.Env.getCtx(), "DateTo") + " </label>"));
            $divToDate.append($divToDateInputwrap);
            $divToDateInputwrap.append($divToDateCtrlwrap);
            $divInfoinn.append($divToDate);

            var $divRefresh = $("<div class='vis-paymentselect-field'>");
            // $divRefresh.append($("<label style='width:100%;'>" + VIS.Msg.translate(VIS.Env.getCtx(), "DateTo") + " </label>"));
            $divRefresh.append($("<button class='VIS_Pref_btn-2' id='VIS_btnRefresh_" + $self.windowNo + "' style='margin-top: 0px;'><i class='vis vis-refresh'></i></button>"));
            $divInfo.append($divRefresh);
            var $divZoom = $("<div class='vis-paymentselect-field'>");
            // $divRefresh.append($("<label style='width:100%;'>" + VIS.Msg.translate(VIS.Env.getCtx(), "DateTo") + " </label>"));
            $divZoom.append($("<button class='VIS_Pref_btn-2' id='VIS_btnZoom_" + $self.windowNo + "' style='margin-top: 0px;'><i class='vis vis-find'></i></button>"));
            $divInfo.append($divZoom);

            $divGridPSelect = $("<div class='vis-pSelectIionGrid'>");

            var designPSelectProcess = " <div class='vis-pSelectProcess'>"  // div pSelectProcess starts here
                                     + " <div class='vis-paymentselect-field'>"  // div  starts here    
                                     + " <input id='VIS_btnCancel_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("Cancel") + "' ></input>"
                                     + " <input id='VIS_btnOK_" + $self.windowNo + "' class='vis-frm-btn' type='submit' value='" + VIS.Msg.getMsg("OK") + "' ></input>"
                                     + " </div>" // div pSelectButons ends here 
                                     + " </div>" // div pSelectProcess ends here 
            $divContainer.append($divInfo).append($divGridPSelect).append($(designPSelectProcess));
            $root.append($divContainer);
            findControls();
            window.setTimeout(function () {
                loadGrid();
            }, 20);
            eventHandling();
            isBusy(true);

        };
        //******************
        //Find Controls through ID
        //******************
        function findControls() {
            //   $cmbOrganization = $('#VIS_cmbOrg_' + $self.windowNo);
            // $cmbMoventType = $('#VIS_cmbMovementType_' + $self.windowNo);
            //  $cmbLocator = $('#VIS_cmbLocator_' + $self.windowNo);
            // $cmbProduct = $('#VIS_cmbProduct_' + $self.windowNo);
            $dtpFromDate = $('#VIS_dtpFromDate' + $self.windowNo);
            $dtpToDate = $('#VIS_dtpToDate' + $self.windowNo);
            $btnRequery = $('#VIS_btnRefresh_' + $self.windowNo);
            $btnZoom = $('#VIS_btnZoom_' + $self.windowNo);
            $btnOk = $('#VIS_btnOK_' + $self.windowNo);
            $btnCancel = $('#VIS_btnCancel_' + $self.windowNo);

            var now = new Date();
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);
            var today = now.getFullYear() + "-" + (month) + "-" + (day);
            //  $dtpFromDate.val(today);
            // $dtpToDate.val(today);


        };
        //******************
        //EventHandling
        //******************
        function eventHandling() {

            //**On click of Requery Button**//
            $btnRequery.on("click", function () {
                refresh();
            });


            //**On click of Requery Button**//
            $btnZoom.on("click", function () {
                zoom();
            });

            //**On click of OK Button**//
            $btnOk.on("click", function () {
                refresh();

            });

            //**On click of Cancel Button**//
            $btnCancel.on("click", function () {
                $self.dispose();
            });







        };
        //*******************
        //Execute Query
        //*******************
        function loadGrid() {
            var AD_Window_ID = 223;		//	Mavarain Account Combinations 
            VIS.AEnv.getGridWindow($self.windowNo, AD_Window_ID, function (json) {
                if (json.error != null) {
                    VIS.ADialog.error(json.error);    //log error
                    self.dispose();
                    self = null;
                    return;
                }
                var jsonData = $.parseJSON(json.result); // widow json
                VIS.context.setContextOfWindow($.parseJSON(json.wCtx), $self.windowNo);// set window context
                var GridWindow = new VIS.GridWindow(jsonData);
                if (GridWindow == null) {
                    return;
                }
                _mTab = GridWindow.getTabs()[0];
                // var id = windowNo + "_" + C_AcctSchema_ID; //uniqueID
                gridController = new VIS.GridController(false);
                gridController.initGrid(true, $self.windowNo, $self, _mTab);
                gridController.setVisible(true);
                // gridController.sizeChanged(530, 400);
                if (window.innerHeight > 700) {
                    gridController.sizeChanged(window.innerHeight - 230, window.innerWidth);
                }
                else {
                    gridController.sizeChanged(window.innerHeight - 215, window.innerWidth);
                }

                $divGridPSelect.append(gridController.getRoot());
                gridController.activate();
                query = new VIS.Query();
                query.addRestriction("AD_Client_ID", VIS.Query.prototype.EQUAL, VIS.context.getAD_Client_ID());
                //_mTab.setQuery(query);
                //gridController.query(0, 0, false);
            });
        };
        //********************
        //Refresh grid records
        //********************
        function refresh() {

            var localquery = null;
            if (query != null) {
                //localquery = query;//query.DeepCopy();
                localquery = jQuery.extend(true, {}, query);
            }
            else {
                localquery = new VIS.Query();
            }
            //var query = _staticQuery.DeepCopy();
            //  Organization
            var value = $cmbOrganization.getValue();
            //no selected value in the Combo
            if (value == null || value.toString() == "-1" || value.toString().trim().length == 0) {
                value = null;
            }
            if (value != null && value.toString().length > 0) {
                localquery.addRestriction("AD_Org_ID", VIS.Query.prototype.EQUAL, value);
            }
            //  Locator
            value = $locator.getValue();
            if (value != null && value.toString().length > 0) {
                localquery.addRestriction("M_Locator_ID", VIS.Query.prototype.EQUAL, value);
            }
            //  Product
            value = $product.getValue();;
            if (value != null && value.toString().length > 0) {
                localquery.addRestriction("M_Product_ID", VIS.Query.prototype.EQUAL, value);
            }
            //  MovementType
            value = $cmbMoventType.getValue();
            if (value == null || value.toString() == "-1" || value.toString().trim().length == 0) {
                value = null;
            }
            if (value != null && value.toString().length > 0) {
                localquery.addRestriction("MovementType", VIS.Query.prototype.EQUAL, value);
            }
            //  DateFrom
            var ts = $dtpFromDate.val().toString();

            if (ts != null && ts.length > 0) {
                ts = new Date(ts);
                localquery.addRestriction("TRUNC(MovementDate,'DD')", VIS.Query.prototype.GREATER_EQUAL, ts);
            }
            //  DateTO
            ts = $dtpToDate.val().toString();

            if (ts != null && ts.length > 0) {
                ts = new Date(ts);
                localquery.addRestriction("TRUNC(MovementDate,'DD')", VIS.Query.prototype.LESS_EQUAL, ts);
            }
            //  gridController.registerIBusy(this);
            _mTab.setQuery(localquery);
            //_mTab.Query(0);
            gridController.query(0, 0, false);
        };
        //********************
        //Zoom record
        //********************
        function zoom() {

            var AD_Window_ID = 0;
            var ColumnName = null;
            var SQL = null;
            var params = [];
            //
            var lineID = VIS.context.getContextAsInt($self.windowNo, "M_InOutLine_ID");
            if (lineID != 0) {

                if (VIS.context.getContext($self.windowNo, "MovementType").equals("C-")) {
                    AD_Window_ID = 169;     //  Customer Shipment
                }
                else if (VIS.context.getContext($self.windowNo, "MovementType").equals("C+")) {
                    AD_Window_ID = 409;     //  Customer Return
                }
                else if (VIS.context.getContext($self.windowNo, "MovementType").equals("V+")) {
                    AD_Window_ID = 184;     //  Vendor Receipt
                }
                else if (VIS.context.getContext($self.windowNo, "MovementType").equals("V-")) {
                    AD_Window_ID = 411;     //  Vendor Return
                }

                ColumnName = "M_InOut_ID";
                SQL = "VIS_140";
                params[0] = new VIS.DB.SqlParam("@lineID", lineID);
            }
            else {
                lineID = VIS.context.getContextAsInt($self.windowNo, "M_InventoryLine_ID");
                if (lineID != 0) {
                    //  log.Fine("M_InventoryLine_ID=" + lineID);
                    AD_Window_ID = 168;
                    ColumnName = "M_Inventory_ID";
                    SQL = "VIS_141";
                    params[0] = new VIS.DB.SqlParam("@lineID", lineID);
                    ///SQL = "SELECT M_Inventory_ID FROM M_InventoryLine WHERE M_InventoryLine_ID=" + lineID;
                }
                else {
                    lineID = VIS.context.getContextAsInt($self.windowNo, "M_MovementLine_ID");
                    if (lineID != 0) {
                        // log.Fine("M_MovementLine_ID=" + lineID);
                        AD_Window_ID = 170;
                        ColumnName = "M_Movement_ID";
                        SQL = "VIS_142";
                        params[0] = new VIS.DB.SqlParam("@lineID", lineID);
                    }
                    else {
                        lineID = VIS.context.getContextAsInt($self.windowNo, "M_ProductionLine_ID");
                        if (lineID != 0) {
                            // log.Fine("M_ProductionLine_ID=" + lineID);
                            AD_Window_ID = 191;
                            ColumnName = "M_Production_ID";
                            SQL = "VIS_143";
                            params[0] = new VIS.DB.SqlParam("@lineID", lineID);
                        }
                        else {
                            // log.Fine("Not found WindowNo=" + $self.windowNo);
                        }
                    }
                }
            }
            if (AD_Window_ID == 0) {
                return;
            }

            //  Get Parent ID
            var parentID = 0;


            //*********************** When No Record Selected on Grid than display Msg  Done by Vikas And Assigned by Gurvinder***************
            var GridSelectedRowCount = 0;
            GridSelectedRowCount = gridController.getSelectedRows();
            //
            if (GridSelectedRowCount.length > 0) {

                parentID = VIS.Utility.Util.getValueOfInt((executeScalar(SQL, null, null)));
                showWindow(ColumnName, lineID, AD_Window_ID, parentID);
            }
            else {
                return alert(VIS.Msg.getMsg("NoRecordSelected"))

            }
            //************************************************  vikas End   ************************************************ 

            /*   parentID = VIS.Utility.Util.getValueOfInt((VIS.DB.executeScalar(SQL, null, null)));
            // System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => ShowWindow(ColumnName, parentID, AD_Window_ID, SQL, lineID));
              showWindow(ColumnName, lineID, AD_Window_ID,parentID);*/


        };


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











        //******************
        //Show window
        //******************
        function showWindow(columnName, recordID, windowID, parentID) {
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(columnName, VIS.Query.prototype.EQUAL, parentID);
            VIS.viewManager.startWindow(windowID, zoomQuery);
        };
        //*******************
        //Get Root
        //*******************
        this.getRoot = function () {
            return $root;
        };
        //*******************
        //dataStatusChanged
        //*******************
        this.dataStatusChanged = function (e) {
        };
        //*******************
        //set Busy
        //*******************
        this.setBusy = function (show) {
            isBusy(show);
        };
        //********************
        //Dispose Component
        //********************
        this.disposeComponent = function () {
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
    VTrxMaterial.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
        this.initialize();
    };

    //Must implement dispose
    VTrxMaterial.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
    VIS.Apps.AForms.VTrxMaterial = VTrxMaterial;
})(VIS, jQuery);