
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};

    function VInOutGen() {
        this.frame;
        this.windowNo;
        var $self = this;
        this.arrListColumns = [];
        this.dGrid = null;
        var whereClause = null;
        var M_Warehouse_ID = null;
        var C_BPartner_ID = null;

        var toggle = false;
        var toggleGen = false;
        var toggleside = false;


        var imgOrder, txtOrderNo;
        var C_Order_IDSearch = null;

        this.lblWarehouse = new VIS.Controls.VLabel();
        this.lblBPartner = new VIS.Controls.VLabel();
        this.lblStatusInfo = new VIS.Controls.VLabel();
        this.tabSelect = new VIS.Controls.VLabel();
        this.tabGenrate = new VIS.Controls.VLabel();
        this.cmbWarehouse = new VIS.Controls.VComboBox('', false, false, true);
        this.vSearchBPartner = null;
        this.vSearchOrder = null;

        this.$root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        this.$busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        this.topDiv = null;
        this.sideDiv = null;
        this.gridSelectDiv = null;
        this.gridGenerateDiv = null;
        this.bottumDiv = null;


        this.div = null;


        this.okBtn = null;
        this.cancelBtn = null;
        this.btnRefresh = null;
        this.btnToggel = null;
        this.spnSelect = null;
        this.spnGenerate = null;
        this.btnSpaceDiv = null;
        this.lblGenrate = null;
        this.lblSelect = null;

        var btnClearBP = null;
        var btnClearOrd = null;

        var sideDivWidth = 260;
        var minSideWidth = 50;
        //window with-(sidediv with_margin from left+ space)
        var selectDivWidth = $(window).width() - (sideDivWidth + 20);
        var selectDivFullWidth = $(window).width() - (20 + minSideWidth);
        var selectDivToggelWidth = selectDivWidth + sideDivWidth + 5;
        var sideDivHeight = $(window).height() - 210;


        function initializeComponent() {

            var topDivId = "topDiv_" + $self.windowNo;
            var btnSpaceDivId = "btnSpaceDiv_" + $self.windowNo;
            var btnSpaceId = "btnSpace_" + $self.windowNo;
            var spnSelectid = "spnSelect_" + $self.windowNo;
            var spnGenerateid = "spnGenerate_" + $self.windowNo;

            var lblGenerateid = "lblGenerate_" + $self.windowNo;
            var lblSelectid = "lblSelect_" + $self.windowNo;

            // JID_1110: Clear option required on Business partner and Docuemnt No. parameter.
            //var imgInfo = VIS.Application.contextUrl + "Areas/VIS/Images/clear16.png";
            //var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-left.png";
            btnClearBP = $('<div class="input-group-append"><button id = "btnClearBP_' + $self.windowNo + '" tabindex="-1" class="vis-controls-txtbtn-table-td2 input-group-text"><i class="fa fa-arrow-left" tabindex="-1" title="' + VIS.Msg.getMsg("Clear", false, false) + '"></i></button></div>');
            btnClearOrd = $('<div class="input-group-append"><button id = "btnClearOrd_' + $self.windowNo + '" tabindex="-1" class="vis-controls-txtbtn-table-td2 input-group-text"><i class="fa fa-arrow-left" tabindex="-1" title="' + VIS.Msg.getMsg("Clear", false, false) + '"></i></button></div>');

            $self.topDiv = $("<div id='" + topDivId + "' class='vis-archive-l-s-head vis-frm-ls-top' style='padding: 0;'>" +
                "<div id='" + btnSpaceDivId + "' class='vis-l-s-headwrp'>" +
                       "<button id='" + btnSpaceId + "'  class='vis-archive-sb-t-button'>" +
                       "<i class='vis vis-arrow-left'></i></button></div>" +
                       "<div id='" + spnSelectid + "'style='display: inline-block;  padding-left: 15px;margin-right: 15px;' >" +
                       "<label id='" + lblSelectid + "' class='VIS_Pref_Label_Font' style='vertical-align: middle;font-size: 28px;color: rgba(var(--v-c-primary), 1);'>" + VIS.Msg.translate(VIS.Env.getCtx(), "Select") + "</label></div>" +
                       "<div id='" + spnGenerateid + "' style='display: inline-block; width: 160px;'>" +
                       "<label id='" + lblGenerateid + "' class='VIS_Pref_Label_Font' style='vertical-align: middle;font-size: 17px;'>" + VIS.Msg.translate(VIS.Env.getCtx(), "Generate") + "</label></div></div>");

            var sideDivId = "sideDiv_" + $self.windowNo;
            var parameterDivId = "parameterDiv_" + $self.windowNo;
            $self.btnSpaceDiv = $self.topDiv.find("#" + btnSpaceDivId);
            this.lblGenrate = $self.topDiv.find("#" + lblGenerateid);
            this.lblSelect = $self.topDiv.find("#" + lblSelectid);

            $self.sideDiv = $("<div id='" + sideDivId + "' class='vis-archive-l-s-content vis-leftsidebarouterwrap'>");
            $self.sideDiv.css("width", sideDivWidth);
            //$self.sideDiv.css("height", sideDivHeight);
            $self.sideDiv.css("height", "calc(100% - 103px)");



            $self.div = $("<div class='vis-archive-l-s-content-inner' id='" + parameterDivId + "'>");
            $self.sideDiv.append($self.div);

            var tble = $("<table style='width: 100%;'>");

            var tr = $("<tr>");
            var td = $("<td style='padding: 10px 10px 0px;'>");
            $self.div.append(tble);
            //tble.append(tr);
            //tr.append(td);
            //td.append($self.lblWarehouse.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));


            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');

            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append($self.cmbWarehouse.getControl());
            Leftformfieldctrlwrp.append($self.lblWarehouse.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);

            //td.append($self.lblBPartner.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            var leftformbtnwrap = $('<div class="input-group-append">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);

            // var ctrl = $("<div style='float: left; width: 100%;' class='VIS_Pref_slide-show pp'>");
            //ctrl.removeClass("VIS_Pref_slide-show pp");
            Leftformfieldctrlwrp.append($self.vSearchBPartner.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            Leftformfieldctrlwrp.append($self.lblBPartner.getControl().addClass("VIS_Pref_Label_Font"));
            Leftformfieldwrp.append(leftformbtnwrap);
            leftformbtnwrap.append($self.vSearchBPartner.getBtn(0));
            Leftformfieldwrp.append(btnClearBP);


            //============Manish 27/1/16
            //var OrderNo = VIS.Msg.translate(VIS.Env.getCtx(), "DocumentNo")

            // JID_1138: Document no. rename as Sales order no.
            var OrderNo = VIS.Msg.translate(VIS.Env.getCtx(), "SalesOrder_No")
            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(OrderNo);

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            var leftformbtnwrap = $('<div class="input-group-append">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append($self.vSearchOrder.getControl(0).attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' '));
            Leftformfieldctrlwrp.append('<label>' + OrderNo + '</label>');
            Leftformfieldwrp.append(leftformbtnwrap);
            leftformbtnwrap.append($self.vSearchOrder.getBtn(0));
            Leftformfieldwrp.append(btnClearOrd);


            //$self.vSearchBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB')
            //imgOrder = "<span><img style='cursor:pointer;height:20px;width:20px;margin:-2px 0 0 4px' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/Search24.PNG' id='ordNoSearch'" + $self.windowNo + "></img></span>";
            //imgOrder = $self.vSearchBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB');
            //txtOrderNo = $('<input type="text" name="orderSearch" style="display: inline-block; width: 206px;">');
            //td.append(txtOrderNo).append(imgOrder);
            ////////////////////////////

            var gridSelectDivId = "gridSelectDiv_" + $self.windowNo;
            var gridGenerateDivId = "gridGenerateDiv_" + $self.windowNo;

            $self.gridSelectDiv = $("<div id='" + gridSelectDivId + "' class='vis-frm-grid-outerwrp'>");
            //$self.gridSelectDiv.css("width", selectDivWidth);
            //$self.gridSelectDiv.css("height", sideDivHeight);

            $self.gridGenerateDiv = $("<div id='" + gridGenerateDivId + "' style='height: 85%; float: right; display: none; background-color: rgba(var(--v-c-secondary), .8); margin-left: 15px;margin-right: 15px;'>");
            $self.gridGenerateDiv.css("width", $(window).width() - 30);

            var name = "btnOk_" + $self.windowNo;
            $self.okBtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' style='margin-top: 0px;width: 70px;height: 38px;' type='button' value='Ok'>");

            //name = "btnCancel_" + $self.windowNo;
            //$self.cancelBtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' style='margin-bottom: 0px; margin-top: 6px; float: right;margin-right: 4px;' type='button' value='Cancel'>");
            debugger;
            name = "btnRefresh_" + $self.windowNo;
            //var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
            $self.btnRefresh = $("<button id='" + name + "' style='margin-top: 0px;' class='VIS_Pref_btn-2'><i class='vis vis-refresh'></i></button>");

            var discriptionDivId = "discriptionDiv_" + $self.windowNo;
            $self.bottumDiv = $("<div class='vis-info-btmcnt-wrap vis-p-t-10'>");
            var mdg = $("<div id='" + discriptionDivId + "' style='flex: 1;padding: 0 10px;'>");
            mdg.append($self.lblStatusInfo.getControl().addClass("VIS_Pref_Label_Font"));
            var buttonDiv = $("<div>");
            //buttonDiv.append($self.cancelBtn);
            buttonDiv.append($self.okBtn);

            $self.bottumDiv.append($self.btnRefresh).append(mdg).append(buttonDiv);

            //Add to root
            $self.$root.append($self.$busyDiv);
            $self.$busyDiv[0].style.visibility = "hidden";
            $self.$root.append($self.topDiv).append($self.sideDiv).append($self.gridSelectDiv).append($self.gridGenerateDiv).append($self.bottumDiv);
        }

        function jbInit() {
            $self.lblWarehouse.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "M_Warehouse_ID"));
            $self.lblBPartner.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
            $self.tabSelect.getControl().text(VIS.Msg.getMsg("Select", false, false));
            $self.tabGenrate.getControl().text(VIS.Msg.getMsg("Generate", false, false));
            //Changed Msg By Pratap - 4/12/15 
            //$self.lblStatusInfo.getControl().text(VIS.Msg.getMsg("InvGenerateSel", false, false));
            $self.lblStatusInfo.getControl().text(VIS.Msg.getMsg("VIS_OrderGenShip", false, false));
        }
        //debugger;
        function fillPicks() {
            //debugger;
            //var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2223, VIS.DisplayType.TableDir);

            // JID_0782: InActive warehouse should not be available to select at Generate Shipment Manual form
            var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 2223, VIS.DisplayType.TableDir, "M_Warehouse_ID", 0, false, "M_Warehouse.IsActive='Y'");
            $self.cmbWarehouse = new VIS.Controls.VComboBox("M_Warehouse_ID", true, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);

            // Handled issue when there is no default Document type, no data was coming in Info.
            var value = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 2762, VIS.DisplayType.Search, "C_BPartner_ID", 0, false, "C_BPartner.IsActive ='Y' And C_Bpartner.Issummary ='N'");
            $self.vSearchBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, value);


            //var Ordervalue = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2161, VIS.DisplayType.Search);
            var Ordervalue = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 2161, VIS.DisplayType.Search, "C_Order_ID", 0, false, "DocStatus='CO'");
            $self.vSearchOrder = new VIS.Controls.VTextBoxButton("C_Order_ID", true, false, true, VIS.DisplayType.Search, Ordervalue);
        }

        function dynInit(data) {

            if ($self.dGrid != null) {
                $self.dGrid.destroy();
                $self.dGrid = null;
            }
            if ($self.arrListColumns.length == 0) {
                // this.arrListColumns.push({ field: "Select", caption: VIS.Msg.getMsg("Select"), sortable: true, size: '50px', hidden: false });
                $self.arrListColumns.push({ field: "AD_Org_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "AD_Org_ID"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "C_DocType_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_DocType_ID"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "DocumentNo", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "DocumentNo"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "C_BPartner_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "C_BPartner_ID"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({
                    field: "DateOrdered", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "DateOrdered"), sortable: true, size: '16%', min: 150, hidden: false, render: function (record, index, col_index) {
                        var val;
                        if (record.changes == undefined || record.changes.DateOrdered == undefined) {
                            val = record["DateOrdered"];
                        }
                        else {
                            val = record.changes.DateOrdered;
                        }
                        return new Date(val).toLocaleDateString();
                    }
                });
                //$self.arrListColumns.push({ field: "TotalLines", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "TotalLines"), sortable: true, size: '20%', min: 150, hidden: false });

                $self.arrListColumns.push({ field: "C_Order_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_UOM_ID"), sortable: true, size: '150px', hidden: true });
            }
            setTimeout(10);
            w2utils.encodeTags(data);
            $self.dGrid = $($self.gridSelectDiv).w2grid({
                name: "gridGenShipForm" + $self.windowNo,
                recordHeight: 40,
                show: { selectColumn: true },
                multiSelect: true,
                columns: $self.arrListColumns,
                records: data
            });

        }



        // Seacrh data
        function executeQuery() {
            var data = [];
            var AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();


            var mWarehouseID = "";
            var cBPartnerID = "";
            var cOrderIDSearch = "";

            if (M_Warehouse_ID != null) {
                if (M_Warehouse_ID.toString() != "-1") {
                    mWarehouseID = " AND ic.M_Warehouse_ID=" + M_Warehouse_ID;
                }
            }
            if (C_BPartner_ID != null) {
                cBPartnerID = " AND ic.C_BPartner_ID=" + C_BPartner_ID;
            }
            if (C_Order_IDSearch != null) {
                cOrderIDSearch = " AND C_Order_ID=" + C_Order_IDSearch;
            }

            // set busy indecator
            $($self.$root[0]).addClass("vis-apanel-busyVInOutGenRoot");
            $($self.$busyDiv[0]).addClass("vis-apanel-busyVInOutGenBusyDiv");
            $self.$busyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "VCreateFrom/ExecuteQueryVinoutgen",
                type: 'POST',
                //async: false,
                data: {
                    adClientID: AD_Client_ID,
                    mWarehouseIDs: mWarehouseID,
                    cBPartnerIDs: cBPartnerID,
                    cOrderIDSearchs: cOrderIDSearch
                },
                success: function (res) {
                    var ress = JSON.parse(res);
                    if (ress && ress.length > 0) {
                        try {
                            var count = 1;
                            for (var i = 0; i < ress.length; i++) {
                                var line = {};
                                line['C_Order_ID'] = ress[i].c_order_id,
                                line['AD_Org_ID'] = ress[i].ord,
                                line['C_DocType_ID'] = ress[i].doctype,
                                line['DocumentNo'] = VIS.Utility.encodeText(ress[i].documentno),
                                line['C_BPartner_ID'] = ress[i].bpname,
                                line['DateOrdered'] = ress[i].dateordered,
                                line['recid'] = count;
                                count++;
                                data.push(line);
                            }
                        }
                        catch (e) {

                        }
                    }
                        // JID_1110: done by Bharat on 27 Feb 2019 to disable Ok button when there in no data in the grid and On selection of order which is not shown in grid message "No Document found" should show.
                    else {
                        VIS.ADialog.info("NoDataFound");
                        $self.okBtn.attr('disabled', 'disabled');
                    }
                    dynInit(data);
                    // set busy indecator
                    $($self.$root[0]).removeClass("vis-apanel-busyVInOutGenRoot");
                    $($self.$busyDiv[0]).removeClass("vis-apanel-busyVInOutGenBusyDiv");
                    $self.$busyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    //$self.log.info(e);
                    // set busy indecator
                    $($self.$root[0]).removeClass("vis-apanel-busyVInOutGenRoot");
                    $($self.$busyDiv[0]).removeClass("vis-apanel-busyVInOutGenBusyDiv");
                    $self.$busyDiv[0].style.visibility = "hidden";
                },
            });
        }





        //function executeQuery() {
        //    var data = [];
        //    //C_Order_IDSearch = $self.vSearchOrder.getValue();
        //    var AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();
        //    //  Create SQL
        //    var sql = (
        //        "SELECT C_Order_ID, o.Name as ord, dt.Name as docType, DocumentNo, bp.Name as bpName, DateOrdered, TotalLines "
        //        + "FROM M_InOut_Candidate_v ic, AD_Org o, C_BPartner bp, C_DocType dt "
        //        + "WHERE ic.AD_Org_ID=o.AD_Org_ID"
        //        + " AND ic.C_BPartner_ID=bp.C_BPartner_ID"
        //        + " AND ic.C_DocType_ID=dt.C_DocType_ID"
        //        + " AND ic.AD_Client_ID=" + AD_Client_ID);

        //    if (M_Warehouse_ID != null) {
        //        if (M_Warehouse_ID.toString() != "-1") {
        //            sql = sql.concat(" AND ic.M_Warehouse_ID=").concat(M_Warehouse_ID);
        //        }
        //    }
        //    if (C_BPartner_ID != null) {
        //        sql = sql.concat(" AND ic.C_BPartner_ID=").concat(C_BPartner_ID);
        //    }
        //    if (C_Order_IDSearch != null) {
        //        sql = sql.concat(" AND C_Order_ID=").concat(C_Order_IDSearch);
        //    }
        //    sql = sql.concat(" ORDER BY o.Name,bp.Name,DateOrdered");

        //    try {
        //        var dr = VIS.DB.executeReader(sql.toString(), null, null);
        //        var count = 1;
        //        while (dr.read()) {
        //            var line = {};
        //            line['C_Order_ID'] = dr.getInt(0);
        //            line['AD_Org_ID'] = dr.getString(1);
        //            line['C_DocType_ID'] = dr.getString(2);
        //            line['DocumentNo'] = dr.getString(3);
        //            line['C_BPartner_ID'] = dr.getString(4);
        //            line['DateOrdered'] = dr.getString(5);
        //            //line['TotalLines'] = dr.getString(6);
        //            line['recid'] = count;
        //            count++;
        //            data.push(line);
        //        }
        //    }
        //    catch (e) {
        //    }
        //    dynInit(data);

        //    return data;
        //}

        function saveSelection() {
            if ($self.dGrid == null) {
                return false;
            }
            var results = [];
            var selectedItems = $self.dGrid.getSelection();

            if (selectedItems == null) {
                return;
            }
            if (selectedItems.length <= 0) {
                return;
            }

            //if ($self.cmbWarehouse.getControl().find('option:selected').val() == undefined) {
            //    VIS.ADialog.error("SelectWarehouseFirst");
            //    return;
            //}

            var splitValue = selectedItems.toString().split(',');
            for (var i = 0; i < splitValue.length; i++) {
                results[i] = ($self.dGrid.get(splitValue[i])).C_Order_ID;//($self.dGrid.get(splitValue[i]));// //  ID in column 0
            }

            if (results.length == 0) {
                return "";
            }

            //	Query String
            var keyColumn = "C_Order_ID";
            var sb = keyColumn;
            if (results.length > 1) {
                sb = sb.concat(" IN (");
            }
            else {
                sb = sb.concat("=");
            }
            //	Add elements
            for (var i = 0; i < results.length; i++) {
                if (i > 0) {
                    sb = sb.concat(",");
                }
                if (keyColumn.endsWith("_ID")) {
                    sb = sb.concat(results[i].toString());
                }
                else {
                    sb = sb.concat("'").concat(results[i].toString());
                }
            }

            if (results.length > 1) {
                sb = sb.concat(")");
            }

            return sb.toString();
        }

        function generateShipments(whereClause) {
            var obj = $self;
            $self.$busyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "Common/GenerateShipments",
                dataType: "json",
                data: {
                    whereClause: whereClause,
                    M_Warehouse_ID: M_Warehouse_ID
                },
                success: function (data) {
                    if (data.DocumentText != null) {
                        //show Generate table with message
                        generatetab(true);
                        obj.gridGenerateDiv.html(data.DocumentText);
                        obj.lblStatusInfo.getControl().text(data.lblStatusInfo);
                    }
                    if (data.ErrorMsg != null) {
                        //show Message in pop up
                        VIS.ADialog.info("", null, data.ErrorMsg, null);
                    }
                    obj.$busyDiv[0].style.visibility = "hidden";
                    // set busy indecator
                    $($self.$root[0]).removeClass("vis-apanel-busyVInOutGenRoot");
                    $($self.$busyDiv[0]).removeClass("vis-apanel-busyVInOutGenBusyDiv");

                },
                error: function (e) {
                    obj.$busyDiv[0].style.visibility = "hidden";
                    // set busy indecator
                    $($self.$root[0]).removeClass("vis-apanel-busyVInOutGenRoot");
                    $($self.$busyDiv[0]).removeClass("vis-apanel-busyVInOutGenBusyDiv");
                }
            });
        }

        function generatetab(toggleside) {
            if (toggleside) {
                $self.btnRefresh.hide();
                $self.okBtn.hide();
                $self.btnToggel.animate({ borderSpacing: 0 }, {
                    step: function (now, fx) {
                        $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                        $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                        $(this).css('transform', 'rotate(' + now + 'deg)');
                    },
                    duration: 'slow'
                }, 'linear');

                toggleside = false;

                $self.gridSelectDiv.animate({ width: selectDivWidth }, "fast");
                $self.sideDiv.animate({ width: 'toggle', height: 'toggle' }, "slow");
            }
            $self.gridSelectDiv.css("display", "none");
            $self.sideDiv.css("display", "none");
            $self.gridGenerateDiv.css("display", "block");
            //$self.btnToggel.attr('disabled', 'disabled');
            $self.btnToggel.hide();

            lblSelect.css("font-size", "17px").css("color", "rgba(var(--v-c-on-secondary), 1)");
            lblGenrate.css("font-size", "28px").css("color", "rgba(var(--v-c-primary), 1)");
        }

        this.vetoablechange = function (evt) {
            C_BPartner_ID = $self.vSearchBPartner.getValue();
            C_Order_IDSearch = $self.vSearchOrder.getValue();
            $self.okBtn.removeAttr('disabled');
            executeQuery();
        };
        //size chnage 
        this.sizeChanged = function (h, w) {
            selectDivWidth = w - (sideDivWidth + 20);
            selectDivFullWidth = w - (20 + minSideWidth);
            if (toggleside == true) {
                $self.btnSpaceDiv.animate({ width: minSideWidth }, "slow");
                $self.sideDiv.animate({ width: minSideWidth }, "slow");
                $self.div.css("display", "none");
                $self.gridSelectDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                    $self.dGrid.resize();
                });
            }
            else {
                $self.btnSpaceDiv.animate({ width: sideDivWidth }, "slow");
                $self.gridSelectDiv.animate({ width: selectDivWidth }, "slow");
                $self.div.css("display", "block");
                $self.sideDiv.animate({ width: sideDivWidth }, "slow", null, function () {
                    $self.dGrid.resize();
                });
            }
        }

        this.Initialize = function () {
            fillPicks();
            jbInit();
            initializeComponent();
            this.btnToggel = this.$root.find("#btnSpace_" + $self.windowNo);
            this.spnSelect = this.$root.find("#spnSelect_" + $self.windowNo);
            this.spnGenerate = this.$root.find("#spnGenerate_" + $self.windowNo)
            //display grid

            if (this.okBtn != null)
                this.okBtn.on(VIS.Events.onTouchStartOrClick, function () {
                    var getSelfOut = $self;

                    // JID_1289: Need to show message if warehouse is not selected. Message: "Warehouse is mandatory"
                    if ($self.cmbWarehouse.getControl().find('option:selected').val() == undefined) {
                        VIS.ADialog.error("SelectWarehouseFirst");
                        return;
                    }

                    $self.$busyDiv[0].style.visibility = 'visible';
                    // set busy indecator
                    $($self.$root[0]).addClass("vis-apanel-busyVInOutGenRoot");
                    $($self.$busyDiv[0]).addClass("vis-apanel-busyVInOutGenBusyDiv");

                    $self.okBtn.attr('disabled', 'disabled');
                    whereClause = saveSelection();
                    if (whereClause != null && whereClause.length > 0) {
                        generateShipments(whereClause);
                    }
                    else { // 16 May 2018 issue in google sheet having id SI_0611 assigned by puneet resolved by Manjot to stop busy indicator
                        VIS.ADialog.error("PlzSelectatLeast1Inv");
                        $self.$busyDiv[0].style.visibility = 'hidden';
                    }
                    $self.okBtn.removeAttr('disabled');
                    //  $self.$busyDiv[0].style.visibility = "hidden";
                });

            if (this.cancelBtn != null)
                this.cancelBtn.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.$root.dialog('close');
                });

            if (this.btnRefresh != null)
                this.btnRefresh.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.okBtn.removeAttr('disabled');
                    M_Warehouse_ID = $self.cmbWarehouse.getControl().find('option:selected').val();
                    C_BPartner_ID = $self.vSearchBPartner.getValue();
                    executeQuery();
                });

            if (this.btnToggel != null)
                this.btnToggel.on(VIS.Events.onTouchStartOrClick, function () {
                    var borderspace=0
                    
                    if (toggleside) {
                        if (VIS.Application.isRTL) {
                            borderspace = 180;
                        }
                        else {
                            borderspace = 0;

                        }
                        //$self.btnRefresh.hide();
                        $self.btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = false;
                        $self.btnSpaceDiv.animate({ width: sideDivWidth }, "slow");
                        $self.gridSelectDiv.animate({ width: selectDivWidth }, "slow");
                        $self.div.css("display", "block");
                        $self.sideDiv.animate({ width: sideDivWidth }, "slow", null, function () {
                            $self.dGrid.resize();
                        });
                    }
                    else {
                        if (VIS.Application.isRTL) {
                            borderspace = 0;
                        }
                        else {
                            borderspace = 180;

                        }
                        $self.btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = true;
                        $self.btnSpaceDiv.animate({ width: minSideWidth }, "slow");
                        $self.sideDiv.animate({ width: minSideWidth }, "slow");
                        $self.div.css("display", "none");
                        $self.gridSelectDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                            $self.dGrid.resize();
                        });
                    }
                });

            if (this.spnGenerate != null)
                this.spnGenerate.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.btnRefresh.hide();
                    $self.okBtn.hide();
                    if (toggleside) {
                        $self.btnToggel.animate({ borderSpacing: 0 }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = false;

                        $self.gridSelectDiv.animate({ width: selectDivWidth }, "fast");
                        $self.sideDiv.animate({ width: sideDivWidth }, "fast");
                        $self.btnSpaceDiv.animate({ width: sideDivWidth }, "fast");
                    }

                    $self.gridSelectDiv.css("display", "none");
                    $self.sideDiv.css("display", "none");
                    $self.gridGenerateDiv.css("display", "block");
                    //$self.btnToggel.attr('disabled', 'disabled');
                    $self.btnToggel.hide();

                    lblSelect.css("font-size", "17px").css("color", "rgba(var(--v-c-on-secondary), 1)");
                    lblGenrate.css("font-size", "28px").css("color", "rgba(var(--v-c-primary), 1)");
                });

            if (this.spnSelect != null)
                this.spnSelect.on(VIS.Events.onTouchStartOrClick, function () {
                    if (!toggleside) {
                        $self.btnRefresh.show();
                        $self.okBtn.show();
                        $self.gridGenerateDiv.css("display", "none");
                        $self.gridSelectDiv.css("display", "block");
                        $self.sideDiv.css("display", "block");
                        $self.div.css("display", "block");

                        $self.gridSelectDiv.animate({ width: selectDivWidth }, "fast");

                        //$self.btnToggel.removeAttr('disabled');
                        $self.btnToggel.show();

                        lblSelect.css("font-size", "28px").css("color", "rgba(var(--v-c-primary), 1)");
                        lblGenrate.css("font-size", "17px").css("color", "rgba(var(--v-c-on-secondary), 1)");
                    }
                });

            if (this.cmbWarehouse != null)
                this.cmbWarehouse.getControl().change(function () {
                    $self.okBtn.removeAttr('disabled');
                    M_Warehouse_ID = $self.cmbWarehouse.getControl().find('option:selected').val();
                    executeQuery();
                });

            if (this.vSearchBPartner != null)
                this.vSearchBPartner.addVetoableChangeListener(this);
            if (this.vSearchOrder != null)
                this.vSearchOrder.addVetoableChangeListener(this);

            if (btnClearBP != null) {
                btnClearBP.on(VIS.Events.onTouchStartOrClick, function () {
                    if ($self.vSearchBPartner.isReadOnly)
                        return;
                    $self.vSearchBPartner.setValue(null, false, true);
                });
            }

            if (btnClearOrd != null) {
                btnClearOrd.on(VIS.Events.onTouchStartOrClick, function () {
                    if ($self.vSearchOrder.isReadOnly)
                        return;
                    $self.vSearchOrder.setValue(null, false, true);
                });
            }
        }

        this.display = function () {
            dynInit(null);
        }

        //Privilized function
        this.getRoot = function () {
            return this.$root;
        };

        this.disposeComponent = function () {

            if (this.okBtn)
                this.okBtn.off(VIS.Events.onTouchStartOrClick);
            if (this.cancelBtn)
                this.cancelBtn.off(VIS.Events.onTouchStartOrClick);
            if (btnClearBP)
                btnClearBP.off(VIS.Events.onTouchStartOrClick);
            if (btnClearOrd)
                btnClearOrd.off(VIS.Events.onTouchStartOrClick);
            $self = null;
            this.frame = null;
            this.windowNo = null;
            this.arrListColumns = null;
            this.dGrid = null;
            whereClause = null;
            M_Warehouse_ID = null;
            C_BPartner_ID = null;

            toggle = null;
            toggleGen = null;
            toggleside = null;

            this.lblWarehouse = null;
            this.lblBPartner = null;
            this.lblStatusInfo = null;
            this.tabSelect = null;
            this.tabGenrate = null;
            this.cmbWarehouse = null;
            this.vSearchBPartner = null;

            this.$root = null;
            this.$busyDiv = null;

            this.topDiv = null;
            this.sideDiv = null;
            this.gridSelectDiv = null;
            this.gridGenerateDiv = null;
            this.bottumDiv = null;
            this.okBtn = null;
            this.cancelBtn = null;
            this.btnRefresh = null;
            this.btnToggel = null;
            this.spnSelect = null;
            this.spnGenerate = null;
            sideDivWidth = null;
            selectDivWidth = null;
            selectDivFullWidth = null;
            selectDivToggelWidth = null;
            sideDivHeight = null;
            this.getRoot = null;
            this.disposeComponent = null;
        };
    };

    //Must Implement with same parameter
    VInOutGen.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        VIS.Env.getCtx().setContext(this.windowNo, "IsSOTrx", "Y");

        try {
            var obj = this.Initialize();
        }
        catch (ex) {
            //log.Log(Level.SEVERE, "init", ex);
        }

        this.frame.getContentGrid().append(this.getRoot());
        this.display();
        this.cmbWarehouse.getControl().focus();
    };

    //Must implement dispose
    VInOutGen.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps.AForms.VInOutGen = VInOutGen;

})(VIS, jQuery);