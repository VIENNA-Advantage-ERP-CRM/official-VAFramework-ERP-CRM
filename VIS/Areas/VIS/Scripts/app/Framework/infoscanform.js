
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Product info Scan Form.
 * Author         :     Bharat
 * Date           :     27-May-2015
  ******************************************************/
; (function (VIS, $) {

    function InfoScanForm(windowNo, showCart) {
        this.onClose = null;
        this.arrListColumns = [];
        var inforoot = $("<div>");
        var isExpanded = true;
        var subroot = $("<div class='vis-info-subroot'>");
        var searchTab = null;
        var searchSec = null;
        var datasec = null;
        var btnsec = null;
        var searchtxt = null;
        var canceltxt = null;
        var Oktxt = null;
        var divbtnRight = null;
        var divbtnsec = null;
        var btnCancel = null;
        var btnOK = null;
        var btnSearch = null;
        var chkDelCart = null;
        var divbtnLeft = null;
        var bsyDiv = null;
        var dGrid = null;
        var self = this;
        var window_ID = 0;
        var grdname = null;
        var lblNametxt = null;
        var lblReftxt = null;
        var lblFromDate = null;
        var lblToDate = null;
        var label = null;
        var ctrl = null;
        var tableSArea = $("<table>");
        var tr = null;
        var refreshUI = false;
        var srchCtrls = [];

        //Paging
        var divPaging, ulPaging, liFirstPage, liPrevPage, liCurrPage, liNextPage, liLastPage, cmbPage;
        var selectedItems = [];

        var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
        //var sql = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "AD_Tab_ID"));
        //window_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
        var AD_tab_ID = VIS.context.getWindowTabContext(windowNo, 0, "AD_Tab_ID");
        window_ID = VIS.dataContext.getJSONRecord("InfoProduct/GetWindowID", AD_tab_ID.toString());

        function initializeComponent() {
            inforoot.css("width", "100%");
            inforoot.css("height", "100%");
            inforoot.css("position", "relative");
            //subroot.css("width", "98%");
            //subroot.css("height", "97%");
            //subroot.css("position", "absolute");
            tableSArea.css("width", "100%");
            //subroot.css("margin-left", "-10px");

            //sSContainer = $("<div style='display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;'>");
            //ssHeader = $('<div style="padding: 7px; background-color: #F1F1F1;margin-bottom: 2px;height:10.5%;">');
            //btnExpander = $('<button style="border: 0px;background-color: transparent; padding: 0px;">').append($('<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/arrow-left.png">'));

            //ssHeader.append(btnExpander);
            //sSContainer.append(ssHeader);

            //if (VIS.Application.isRTL) {
            //    searchTab = $("<div style='background-color: rgb(241, 241, 241);height:88.9%;display: inline-block;width:25%;height:87.8%;overflow:auto;padding-right: 10px;margin-left: 10px;'>");
            //    searchSec = $("<div style='background-color: rgb(241, 241, 241);display: inline-block;height:87.8%;'>");
            //    searchTab.append(searchSec);
            //    datasec = $("<div style='display: inline-block;width:73%;height:87.8%;overflow:auto;'>");
            //    btnsec = $("<div style='display: inline-block;width:99%;height:auto;margin-top: 2px;'>");
            //}
            //else {
                searchTab = $("<div class='vis-info-l-s-wrap vis-pad-0 vis-leftsidebarouterwrap'>");
                searchSec = $("<div class='vis-info-l-s-content'>");
                searchTab.append(searchSec);
                datasec = $("<div class='vis-info-datasec'>");
                btnsec = $("<div class='vis-info-btnsec vis-just-cont-end'>");
            //}

            subroot.append(searchTab);
            subroot.append(datasec);
            subroot.append(btnsec);

            lblNametxt = VIS.Msg.translate(VIS.Env.getCtx(), "Name")
            if (lblNametxt.indexOf('&') > -1) {
                lblNametxt = lblNametxt.replace('&', '');
            }
            lblReftxt = VIS.Msg.translate(VIS.Env.getCtx(), "ReferenceNo");
            if (lblReftxt.indexOf('&') > -1) {
                lblReftxt = lblUPCtxt.replace('&', '');
            }

            lblFromDate = VIS.Msg.translate(VIS.Env.getCtx(), "FromDate");
            if (lblFromDate.indexOf('&') > -1) {
                lblFromDate = lblFromDate.replace('&', '');
            }

            lblToDate = VIS.Msg.translate(VIS.Env.getCtx(), "ToDate");
            if (lblToDate.indexOf('&') > -1) {
                lblToDate = lblToDate.replace('&', '');
            }

            canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }

            searchtxt = VIS.Msg.getMsg("Search");
            if (searchtxt.indexOf('&') > -1) {
                searchtxt = searchtxt.replace('&', '');
            }

            Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            divbtnRight = $("<div class='vis-info-btnswrap'>");
            divbtnsec = $("<div class='vis-info-ls-btnswrap'>");
            //if (VIS.Application.isRTL) {
            //    btnCancel = $("<button class='VIS_Pref_btn-2'>").append(canceltxt);
            //    btnOK = $("<button class='VIS_Pref_btn-2'>").append(Oktxt);
            //    btnSearch = $("<button class='VIS_Pref_btn-2'>").append(searchtxt);
            //}
            //else {

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
                var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
                label = $("<label class='VIS_Pref_Label_Font'>").append(lblNametxt);
                tr.append(td);
                td.append(Leftformfieldwrp);
                Leftformfieldwrp.append(Leftformfieldctrlwrp);

                //tr = $("<tr>");
                //tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("Name", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "Name";
                var tdctrl = $("<td>");
                //tr.append(tdctrl);
                Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                Leftformfieldctrlwrp.append(label);
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
                var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
                label = $("<label class='VIS_Pref_Label_Font'>").append(lblReftxt);
                tr.append(td);
                td.append(Leftformfieldwrp);
                Leftformfieldwrp.append(Leftformfieldctrlwrp);

                //tr = $("<tr>");
                //tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("Reference", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "Reference";
                var tdctrl = $("<td>");
                //tr.append(tdctrl);
                Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                Leftformfieldctrlwrp.append(label);
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
                var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
                label = $("<label class='VIS_Pref_Label_Font'>").append(lblFromDate);
                tr.append(td);
                td.append(Leftformfieldwrp);
                Leftformfieldwrp.append(Leftformfieldctrlwrp);

                //tr = $("<tr>");
                //tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VDate("TrxFromDate", false, false, true, VIS.DisplayType.Date, VIS.Msg.translate("FromDate"));
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "TrxFromDate";
                var tdctrl = $("<td>");
                //tr.append(tdctrl);
                Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                Leftformfieldctrlwrp.append(label);
                //tdctrl.append(ctrl.getControl().addClass("vis-allocation-date"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
                var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
                label = $("<label class='VIS_Pref_Label_Font'>").append(lblToDate);
                tr.append(td);
                td.append(Leftformfieldwrp);
                Leftformfieldwrp.append(Leftformfieldctrlwrp);

                //tr = $("<tr>");
                //tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VDate("TrxToDate", false, false, true, VIS.DisplayType.Date, VIS.Msg.translate("ToDate"));
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "TrxToDate";
                var tdctrl = $("<td>");
                //tr.append(tdctrl);
                Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                Leftformfieldctrlwrp.append(label);
                srchCtrls.push(srchCtrl);

                searchSec.append(tableSArea);

                btnCancel = $("<button class='VIS_Pref_btn-2'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2'>").append(Oktxt);
                btnSearch = $("<button class='VIS_Pref_btn-2'>").append(searchtxt);
            //}

            divbtnRight.append(btnCancel);
            divbtnRight.append(btnOK);
            divbtnRight.append($('<div><input type="checkbox" id="chkDelCart_' + windowNo + '" ><span>' + VIS.Msg.getMsg("DeleteCart") + '</span>'));
            divbtnsec.append(btnSearch);
            searchTab.append(divbtnsec);
            btnsec.append(divbtnRight);

            //PagingCtrls
            divPaging = $('<div class="vis-info-pagingwrp">');
            createPageSettings();
            divPaging.append(ulPaging);

            //if (VIS.Application.isRTL) {
            //    divbtnLeft.append(btnCancel);
            //    divbtnLeft.append(btnOK);
            //    divbtnsec.append(btnSearch);
            //    searchTab.append(divbtnsec);
            //}
            //else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
                divbtnsec.append(btnSearch);
                searchTab.append(divbtnsec);
            //}

            // divbtnLeft.append(btnRequery);
            btnsec.append(divbtnLeft);
            btnsec.append(divPaging);
            btnsec.append(divbtnRight);
            //Busy Indicator
            bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //bsyDiv.css("width", "98%");
            //bsyDiv.css("height", "97%");
            //bsyDiv.css('text-align', 'center');
            //bsyDiv.css("position", "absolute");
            bsyDiv[0].style.visibility = "visible";
            chkDelCart = btnsec.find("#chkDelCart_" + windowNo);

            inforoot.append(subroot);
            inforoot.append(bsyDiv);
        };

        initializeComponent();

        function bindEvent() {
            if (!VIS.Application.isMobile) {
                inforoot.on('keyup', function (e) {
                    if (!(e.keyCode === 13)) {
                        return;
                    }
                    bsyDiv[0].style.visibility = 'visible';
                    selectedItems = [];
                    dGrid.selectNone();
                    displayData(true, cmbPage.val());
                });
            }

            btnCancel.on("click", function () {
                self.dispose();
            });

            btnOK.on("click", function () {
                btnOKClick();
            });

            btnSearch.on("click", function () {
                bsyDiv[0].style.visibility = 'visible';
                selectedItems = [];
                dGrid.selectNone();
                displayData(true, 1);
            });
        };

        bindEvent();

        var displayData = function (requery, pNo) {
            if (dGrid) {
                var selection = dGrid.getSelection();
                for (item in selection) {
                    if (selectedItems.indexOf(dGrid.get(selection[item])["count_ID"]) == -1) {
                        selectedItems.push(dGrid.get(selection[item])["count_ID"]);
                    }
                }
            }
            var query = "";
            var whereClause = "";
            
            var M_Locator_ID = 0;
            var M_LocatorTo_ID = 0;

            var M_Warehouse_ID = 0;
            var M_WarehouseTo_ID = 0;

            if (requery == true) {
                //var name = "";
                //var ref = "";
                //var date = "";
                var srchValue = null;
                //var drInv = null;
                //var data = [];

                for (var i = 0; i < srchCtrls.length; i++) {
                    srchValue = srchCtrls[i].Ctrl.getValue();
                    if (srchValue == null || srchValue.length == 0 || srchValue == 0) {
                        continue;
                    }

                    if (srchCtrls[i].Ctrl.colName == "Name") {
                        query += " AND upper(VAICNT_ScanName) LIKE '%" + srchValue.toUpperCase() + "%' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "Reference") {
                        query += " AND upper(VAICNT_ReferenceNo) LIKE '%" + srchValue.toUpperCase() + "%' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "TrxFromDate") {
                        var date = VIS.DB.to_date(srchValue, true);
                        query += " AND DateTrx >= " + date;
                    }

                    else if (srchCtrls[i].Ctrl.colName == "TrxToDate") {
                        var date = VIS.DB.to_date(srchValue, true);
                        query += " AND DateTrx <= " + date;
                    }
                }
                if (showCart) {
                    query += " AND VAICNT_TransactionType = 'OT' ";
                }
                else {
                    if (window_ID == 184) {   // JID_1026: System is not checking the document status of Order and requisition while loading cart on M_inout and internal use move line respectively
                        query += " AND VAICNT_TransactionType = 'MR' and VAICNT_ReferenceNo in (SELECT DocumentNo from C_Order WHERE C_BPartner_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "C_BPartner_ID")) + " AND DocStatus IN ('CO', 'CL'))";
                    }
                    else if (window_ID == 319 || window_ID == 170) {
                        query += " AND VAICNT_TransactionType = 'IM' ";
                        // extra parameters only for these windows
                        M_Locator_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "M_Locator_ID", true))
                        M_LocatorTo_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "M_LocatorTo_ID", true));
                        M_Warehouse_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "DTD001_MWarehouseSource_ID", true));
                        M_WarehouseTo_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "M_Warehouse_ID", true));
                    }
                    else if (window_ID == 168) {
                        query += " AND VAICNT_TransactionType = 'PI' ";
                        M_Warehouse_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "M_Warehouse_ID", true));
                    }
                    else if (window_ID == 169) {
                        query += " AND VAICNT_TransactionType = 'SH' and VAICNT_ReferenceNo in (SELECT DocumentNo from C_Order WHERE  C_BPartner_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "C_BPartner_ID")) + " AND DocStatus IN ('CO'))";
                    }
                    else if (window_ID == 341) {
                        query += " AND VAICNT_TransactionType = 'IU' AND VAICNT_ReferenceNo IN (SELECT DocumentNo FROM M_Requisition WHERE IsActive = 'Y' AND M_Warehouse_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowContext(windowNo, "M_Warehouse_ID", true)) + " AND DocStatus IN ('CO'))";
                    }
                    else {
                        query += " AND VAICNT_TransactionType = 'OT' ";
                    }
                }
            }
            else {
                query += " AND VAICNT_InventoryCount_ID = -1";
            }

            sql = "SELECT VAICNT_ScanName,VAICNT_ReferenceNo,DateTrx,VAICNT_InventoryCount_ID FROM VAICNT_InventoryCount WHERE IsActive='Y' AND AD_Client_ID = "
                + VIS.Utility.Util.getValueOfInt(VIS.context.getAD_Client_ID()) + query;
            
            var _sql = VIS.secureEngine.encrypt(sql);
            if (!pNo) {
                pNo = 1;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "InfoProduct/GetCart",
                dataType: "json",
                data: {
                    sql: _sql,
                    pageNo: pNo,
                    isCart: showCart,
                    windowID: window_ID,
                    WarehouseID: M_Warehouse_ID,
                    WarehouseToID: M_WarehouseTo_ID,
                    LocatorID: M_Locator_ID,
                    LocatorToID: M_LocatorTo_ID,
                },
                error: function () {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    bsyDiv[0].style.visibility = 'hidden';
                    return;
                },
                success: function (dyndata) {
                    var res = JSON.parse(dyndata);
                    if (res == null) {
                        bsyDiv[0].style.visibility = "hidden";
                        return;
                    }
                    if (res.Error) {
                        VIS.ADialog.error(res.Error);
                        bsyDiv[0].style.visibility = "hidden";
                        return;
                    }
                    resetPageCtrls(res.pSetting);
                    loadGrid(res.data);
                }
            });
        };

        var loadGrid = function (data) {

            if (data == null) {
                bsyDiv[0].style.visibility = "hidden";
                return;
            }
            if (dGrid != null) {
                dGrid.destroy();
                dGrid = null;
            }

            dGrid = null;
            self.arrListColumns = [];

            self.arrListColumns.push({ field: "Name", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Name"), sortable: true, size: '16%', min: 150, hidden: false });
            self.arrListColumns.push({ field: "Reference", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ReferenceNo"), sortable: true, size: '16%', min: 150, hidden: false });
            self.arrListColumns.push({ field: "TrxDate", caption: VIS.Msg.translate(VIS.Env.getCtx(), "DateTrx"), sortable: true, size: '16%', min: 150, hidden: false, render: 'date' });
            self.arrListColumns.push({ field: "count_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ID"), sortable: true, size: '16%', min: 150, hidden: true });
            var grdname = 'gridPrdInfodata' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(data);
            dGrid = $(datasec).w2grid({
                name: grdname,
                recordHeight: 30,
                show: {
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,     // indicates if line numbers column is visible
                    selectColumn: true     // indicates if select column is visible
                },
                columns: self.arrListColumns,
                records: data,
                onClick: function (event) {
                    if (dGrid.records.length > 0) {
                        btnOK.removeAttr("disabled");
                    }
                }
            });

            dGrid.selectNone();
            for (itm in selectedItems) {
                for (item in dGrid.records) {
                    if (dGrid.records[item]["count_ID"] == selectedItems[itm]) {
                        dGrid.select(parseInt(item) + 1);
                    }
                }
            }

            dGrid.on('unselect', function (e) {
                if (e.all) {
                    e.onComplete = function () {
                        for (item in dGrid.records) {
                            var unselectedVal = dGrid.records[item]["count_ID"];
                            if (selectedItems.indexOf(unselectedVal) > -1) {
                                selectedItems.splice(selectedItems.indexOf(unselectedVal), 1);
                            }
                        }
                    };
                }
                else {
                    var unselectcart = dGrid.records[e.index]["count_ID"];
                    if (selectedItems.indexOf(unselectcart) > -1) {
                        selectedItems.splice(selectedItems.indexOf(unselectcart), 1);
                    }
                }
            });
            bsyDiv[0].style.visibility = "hidden";
        };

        var btnOKClick = function () {
            var countID = "";
            var _query = "";
            if (dGrid != null) {
                var selection = dGrid.getSelection();
                if (selection.length > 0) {
                    for (var item in selection) {
                        if (selectedItems.indexOf(dGrid.get(selection[item])["count_ID"]) == -1) {
                            selectedItems.push(dGrid.get(selection[item])["count_ID"]);
                        }
                    }
                }
                if (selectedItems.length > 0) {
                    for (var item in selectedItems) {
                        countID += selectedItems[item] + ",";
                    }
                    countID = countID.substr(0, countID.length - 1);
                    //_query = "SELECT cl.M_Product_ID,prd.Name,prd.Value,cl.VAICNT_Quantity,cl.M_AttributeSetInstance_ID,cl.C_UOM_ID,uom.Name as UOM,ic.VAICNT_ReferenceNo,cl.VAICNT_InventoryCountLine_ID,"
                    //    + " ats.Description FROM VAICNT_InventoryCount ic INNER JOIN VAICNT_InventoryCountLine cl ON ic.VAICNT_InventoryCount_ID = cl.VAICNT_InventoryCount_ID"
                    //    + " INNER JOIN M_Product prd ON cl.M_Product_ID = prd.M_Product_ID INNER JOIN C_UOM uom ON cl.C_UOM_ID = uom.C_UOM_ID LEFT JOIN M_AttributeSetInstance ats"
                    //    + " ON cl.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE ic.VAICNT_InventoryCount_ID IN (" + countID + ") ORDER BY cl.Line";

                    //}
                    //else {
                    //_query = "select cnt.M_Product_ID,p.Value,cnt.vaicnt_quantity,cnt.vaicnt_attributeno,ats.Islot,ats.IsSerNo,ats.IsGuaranteeDate,case when (ats.IsGuaranteeDate = 'Y') then"
                    //+ " sysdate+p.GuaranteeDays end as ExpiryDate,cnt.VAICNT_ReferenceNo,NVL(cnt.M_AttributeSetInstance_ID,0) as M_AttributeSetInstance_ID FROM (SELECT CASE WHEN (cl.upc = mr.upc) THEN mr.M_product_ID"
                    //+ " ELSE CASE WHEN (cl.upc = prd.upc) THEN prd.M_Product_ID ELSE CASE WHEN (cl.upc = patr.upc) THEN patr.M_Product_ID END END END AS M_Product_ID,patr.M_AttributeSetInstance_ID,cl.vaicnt_quantity,"
                    //+ " cl.vaicnt_attributeno,ic.VAICNT_ReferenceNo FROM VAICNT_InventoryCount ic INNER JOIN VAICNT_InventoryCountLine cl ON (ic.VAICNT_InventoryCount_ID = cl.VAICNT_InventoryCount_ID)"
                    //+ " LEFT JOIN M_manufacturer mr ON cl.upc = mr.upc LEFT JOIN M_product prd ON cl.upc = prd.upc LEFT JOIN M_ProductAttributes patr ON cl.upc = patr.upc WHERE ic.IsActive = 'Y'"
                    //+ " AND cl.IsActive = 'Y' AND (cl.upc = mr.upc OR cl.upc = prd.upc OR cl.upc = patr.upc) AND ic.ad_client_id = " + VIS.context.getAD_Client_ID() +
                    //" AND ic.VAICNT_InventoryCount_ID = " + countID + " ) cnt INNER JOIN M_Product p on cnt.M_Product_ID=p.M_Product_ID LEFT JOIN M_AttributeSet ats on p.M_attributeset_id=ats.M_attributeset_id"
                    //+ " WHERE p.ad_client_id = " + VIS.context.getAD_Client_ID();
                    //}

                    //var drProd = VIS.DB.executeReader(_query, null, null);
                    var drProd = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "InfoProduct/GetCartData", { "invCount_ID": countID, "WindowID": window_ID }, null);
                    if (self.onClose)
                        self.onClose(drProd, chkDelCart.prop("checked"), countID);
                    inforoot.dialog('close');
                }
                else {
                    alert("Please Select a Record");
                }
            }
        };

        function createPageSettings() {
            ulPaging = $('<ul class="vis-statusbar-ul">');

            liFirstPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftleft" title="First Page" style="opacity: 0.6;"></i></div></li>');

            liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-pageup" title="Page Up" style="opacity: 0.6;"></i></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" title="Page Down" style="opacity: 0.6;"></i></div></li>');

            liLastPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" title="Last Page" style="opacity: 0.6;"></i></div></li>');


            ulPaging.append(liFirstPage).append(liPrevPage).append(liCurrPage).append(liNextPage).append(liLastPage);
            pageEvents();
        }

        function pageEvents() {
            liFirstPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    bsyDiv[0].style.visibility = 'visible';
                    displayData(true, 1);
                }
            });
            liPrevPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    bsyDiv[0].style.visibility = 'visible';
                    displayData(true, parseInt(cmbPage.val()) - 1);
                }
            });
            liNextPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    bsyDiv[0].style.visibility = 'visible';
                    displayData(true, parseInt(cmbPage.val()) + 1);
                }
            });
            liLastPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    bsyDiv[0].style.visibility = 'visible';
                    displayData(true, parseInt(cmbPage.find("Option:last").val()));
                }
            });
            cmbPage.on("change", function () {
                bsyDiv[0].style.visibility = 'visible';
                displayData(true, cmbPage.val());
            });

        }

        function resetPageCtrls(psetting) {

            //cmbPage.empty();
            cmbPage = null;
            liCurrPage.find("select").remove();
            cmbPage = $("<select>");
            liCurrPage.append(cmbPage);

            cmbPage.on("change", function () {
                bsyDiv[0].style.visibility = 'visible';
                displayData(true, cmbPage.val());
            });

            if (psetting.TotalPage > 0) {
                for (var i = 0; i < psetting.TotalPage; i++) {
                    cmbPage.append($("<option value=" + (i + 1) + ">" + (i + 1) + "</option>"))
                }
                cmbPage.val(psetting.CurrentPage);


                if (psetting.TotalPage > psetting.CurrentPage) {
                    liNextPage.css("opacity", "1");
                    liLastPage.css("opacity", "1");
                }
                else {
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

                if (psetting.CurrentPage > 1) {
                    liFirstPage.css("opacity", "1");
                    liPrevPage.css("opacity", "1");
                }
                else {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                }

                if (psetting.TotalPage == 1) {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

            }
            else {
                liFirstPage.css("opacity", "0.6");
                liPrevPage.css("opacity", "0.6");
                liNextPage.css("opacity", "0.6");
                liLastPage.css("opacity", "0.6");
            }
        }

        this.showDialog = function () {
            inforoot.dialog({
                modal: true,
                title: VIS.Msg.translate(VIS.Env.getCtx(), "Info Scan"),
                width: 800,
                height: 450,
                resizable: false,
                close: function () {
                    self.dispose();
                }
            });
            displayData(false, cmbPage.val());
        };

        this.disposeComponent = function () {

            inforoot.off('keyup');
            btnCancel.off("click");
            btnOK.off("click");
            btnSearch.off("click");
            datasec = null;
            searchSec = null;

            liFirstPage.off("click");
            liPrevPage.off("click");
            liNextPage.off("click");
            liLastPage.off("click");
            cmbPage.off("change");
            liFirstPage = liPrevPage = liNextPage = liLastPage = cmbPage = null;

            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;

            isExpanded = null;
            subroot = null;
            searchTab = null;
            btnsec = null;
            searchtxt = null;
            canceltxt = null;
            Oktxt = null;
            divbtnRight = null;
            btnCancel = null;
            btnOK = null;
            chkDelCart = null;
            divbtnLeft = null;
            bsyDiv = null;
            srchCtrls = null;
            self = null;
            refreshUI = false;
            if (inforoot != null) {
                // inforoot.dialog('close');
                inforoot.dialog('destroy');
                inforoot.remove();
            }
            inforoot = null;

            //this.btnOKClick = null;
            this.displayData = null;
            this.disposeComponent = null;
        };
    };

    //dispose call
    InfoScanForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    VIS.InfoScanForm = InfoScanForm;

})(VIS, jQuery);
