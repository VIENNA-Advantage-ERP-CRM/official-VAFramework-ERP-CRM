; (function (VIS, $) {
    //SerNo Parameter Added by Manjot To implement Search Functionality on Grid 10 May 2018 google Sheet ID SI_0607 
    function PAttributeInstance(title, M_Warehouse_ID, M_Locator_ID, M_Product_ID, C_BPartner_ID, SerNo, lotNo, garunteeDate, isSOTrx) {

        var $self = this;
        this.onClose = null;
        var $root = $("<div>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var mWarehouseID = M_Warehouse_ID;
        var mLocatorID = M_Locator_ID;
        var mProductID = M_Product_ID;
        var mCBPartnerID = C_BPartner_ID;
        var mtitle = title;
        var issotrx = isSOTrx;
        this.arrListColumns = [];
        this.dGrid = null;

        var mAttributeSetInstanceID = -1;
        var mAttributeSetInstanceName = null;
        var attrCode = "";
        var msql = "";
        var msqlNonZero = "";
        //	From Clause						
        var msqlFrom = "M_ProductAttributes patr LEFT JOIN M_Storage s ON (patr.M_AttributeSetInstance_ID = s.M_AttributeSetInstance_ID AND patr.M_Product_ID = s.M_Product_ID)"
            + " LEFT JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
            + " LEFT JOIN M_Warehouse w ON (l.M_Warehouse_ID=w.M_Warehouse_ID)"
            + " INNER JOIN M_Product p ON (patr.M_Product_ID=p.M_Product_ID)"
            + " INNER JOIN M_AttributeSetInstance asi ON (patr.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID)";

        var msqlWhere = " patr.M_Product_ID=@M_Product_ID AND patr.M_AttributeSetInstance_ID != 0";
        msqlNonZero = " AND s.QtyOnHand>0";            // (s.QtyOnHand<>0 OR s.QtyReserved<>0 OR s.QtyOrdered<>0)";
        var msqlMinLife = "";

        this.log = VIS.Logging.VLogger.getVLogger("PAttributeInstance");
        var windowNo = VIS.Env.getWindowNo();

        var chkShowAll = $("<input id='" + "chkShowAll_" + windowNo + "' type='checkbox' >" +
            "<span><label id='" + "lblShowAll_" + windowNo + "' class='VIS_Pref_Label_Font'>" + VIS.Msg.getMsg("VIS_ShowAll") + "</label></span>");

        var btnOk = $("<input id='" + "btnOk_" + windowNo + "' class='VIS_Pref_btn-2' style='float: right; margin-right: 10px;' type='button' value='" + VIS.Msg.getMsg("OK") + "'>");

        var btnCancel = $("<input id='" + "btnCancel_" + windowNo + "' class='VIS_Pref_btn-2' style='float: right;margin-right: 0px;' type='button' value='" + VIS.Msg.getMsg("Cancel") + "'>");

        var topdiv = $("<div id='" + "topdiv_" + windowNo + "' style='float: left; width: 100%; height: 7%; text-align: right;'>");
        var middeldiv = $("<div id='" + "middeldiv_" + windowNo + "' style='float: left; width: 100%; height: 79%;'>");
        var bottomdiv = $("<div id='" + "bottomdiv_" + windowNo + "' style='float: left; width: 100%; height: 14%;'>");

        var disableSearch = false;
        if (VIS.Application.isRTL) {
            topdiv.css("text-align", "left");
            btnOk.css("margin-right", "0px");
            btnOk.css("margin-left", "10px");
            btnOk.css("float", "left");
            btnCancel.css("margin-right", "0px");
            btnCancel.css("float", "left");
        }


        function dynInit(C_BPartner_ID) {
            $self.log.config("C_BPartner_ID=" + C_BPartner_ID);
            if (C_BPartner_ID != 0) {
                var shelfLifeMinPct = 0;
                var shelfLifeMinDays = 0;

                //var sql = "SELECT bp.ShelfLifeMinPct, bpp.ShelfLifeMinPct, bpp.ShelfLifeMinDays "
                //    + "FROM C_BPartner bp "
                //    + " LEFT OUTER JOIN C_BPartner_Product bpp"
                //    + " ON (bp.C_BPartner_ID=bpp.C_BPartner_ID AND bpp.M_Product_ID=" + mProductID + ") "
                //    + "WHERE bp.C_BPartner_ID=" + C_BPartner_ID;

                var dr = null;
                try {
                    dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "PAttributes/GetBPData", { "Product_ID": mProductID, "BPartner_ID": C_BPartner_ID }, null);
                    if (dr != null) {
                        shelfLifeMinPct = dr["ShelfLifeMinPct"];
                        var pct = dr["PCT"];
                        if (pct > 0) {
                            shelfLifeMinDays = pct;
                        }
                        shelfLifeMinDays = dr["ShelfLifeMinDays"];
                    }
                    //dr = VIS.DB.executeReader(sql, null);
                    //if (dr.read()) {
                    //    shelfLifeMinPct = dr.getInt(0);		//	BP
                    //    var pct = dr.getInt(1);				//	BP_P
                    //    if (pct > 0)	//	overwrite
                    //    {
                    //        shelfLifeMinDays = pct;
                    //    }
                    //    shelfLifeMinDays = dr.getInt(2);
                    //}
                    //dr.close();
                    //dr = null;
                }
                catch (e) {
                    //if (dr != null) {
                    //    dr.close();
                    //}
                    //this.log.Log(Level.SEVERE, sql, e);
                    console.log(e);
                }

                if (shelfLifeMinPct > 0) {
                    msqlMinLife = " AND COALESCE(TRUNC((daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD'))/p.GuaranteeDays)*100),0)>=" + shelfLifeMinPct;
                    $self.log.config("PAttributeInstance.dynInit - ShelfLifeMinPct=" + shelfLifeMinPct);
                }
                if (shelfLifeMinDays > 0) {
                    msqlMinLife += " AND COALESCE(daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD')),0)>=" + shelfLifeMinDays;
                    $self.log.config("PAttributeInstance.dynInit - ShelfLifeMinDays=" + shelfLifeMinDays);
                }
            }	//	BPartner != 0

            msql = prepareTable(msqlFrom, msqlWhere, false, "patr") + " ORDER BY asi.GuaranteeDate, QtyOnHand DESC";	//	oldest, smallest first
            //refresh();
            topdiv.append(chkShowAll);
            bottomdiv.append(btnCancel).append(btnOk);
            $root.append($busyDiv).append(topdiv).append(middeldiv).append(bottomdiv);

            if (issotrx) {
                chkShowAll.prop("checked", false);
            } else {
                chkShowAll.prop("checked", true);
            }
            //topdiv.hide();
            events();
        }

        function prepareTable(from, where, multiSelection, tableName) {
            var sql = "SELECT DISTINCT patr.M_AttributeSetInstance_ID, asi.Description, asi.Lot, asi.SerNo, asi.GuaranteeDate, asi.Value AS AttrCode, l.Value, NVL(s.M_Locator_ID,0) AS M_Locator_ID," +
                " NVL(s.QtyOnHand,0) AS QtyOnHand, NVL(s.QtyReserved,0) AS QtyReserved, NVL(s.QtyOrdered,0) AS QtyOrdered," +
                " (daysBetween(TRUNC(asi.GuaranteeDate,'DD'), TRUNC(SysDate,'DD'))-p.GuaranteeDaysMin) as ShelfLifeDays," +
                " daysBetween(TRUNC(asi.GuaranteeDate,'DD'), TRUNC(SysDate,'DD')) as GoodForDays, CASE WHEN p.GuaranteeDays > 0 THEN " +
                " ROUND(daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD'))/p.GuaranteeDays*100,12) ELSE 0 END as ShelfLifeRemainingPct";

            sql = sql.concat(" FROM ").concat(from);
            sql = sql.concat(" WHERE ").concat(where);

            //if (mLocatorID != 0) {
            //    sql = sql.concat(" AND s.M_Locator_ID = " + mLocatorID);
            //}
            if (mWarehouseID != 0) {
                sql = sql.concat(" AND NVL(l.M_Warehouse_ID,0) IN (0," + mWarehouseID + ")");
            }

            if (from.length == 0) {
                return sql.toString();
            }
            //
            $self.log.finest(finalSQL);
            var finalSQL = VIS.MRole.getDefault().addAccessSQL(sql, tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO);
            return finalSQL
        }

        function refresh() {
            if (msql == null) {
                msql = "";
            }
            var data = [];

            var sql = msql;
            var pos = msql.lastIndexOf(" ORDER BY ");
            if (!chkShowAll.prop("checked")) {
                sql = msql.substring(0, pos) + msqlNonZero;
                if (msqlMinLife.length > 0) {
                    sql += msqlMinLife;
                }
                sql += msql.substring(pos);
            }
            //
            $self.log.finest(sql);

            try {
                var _sql = VIS.secureEngine.encrypt(sql);
                dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "PAttributes/GetAttributeData", { "Sq1Atribute": _sql, "Product_ID": mProductID }, null);
                if (dr != null && dr.length > 0) {
                    var count = 1;
                    for (var i in dr) {
                        var line = {};
                        line['M_AttributeSetInstance_ID'] = dr[i]["M_AttributeSetInstance_ID"];
                        line['Description'] = dr[i]["Description"];
                        line['Lot'] = dr[i]["Lot"];
                        line['SerNo'] = dr[i]["SerNo"];
                        line['GuaranteeDate'] = dr[i]["GuaranteeDate"];
                        line['Value'] = dr[i]["Value"];
                        line['QtyReserved'] = dr[i]["QtyReserved"];
                        line['QtyOrdered'] = dr[i]["QtyOrdered"];
                        line['QtyOnHand'] = dr[i]["QtyOnHand"];
                        line['GoodForDays'] = dr[i]["GoodForDays"];
                        line['ShelfLifeDays'] = dr[i]["ShelfLifeDays"];
                        line['ShelfLifeRemainingPct'] = dr[i]["ShelfLifeRemainingPct"];
                        line['M_Locator_ID'] = dr[i]["M_Locator_ID"];
                        line['AttrCode'] = dr[i]["AttrCode"];
                        line['recid'] = count;
                        count++;
                        data.push(line);
                    }
                }

                //var param = [];
                //param[0] = new VIS.DB.SqlParam("@M_Product_ID", mProductID);
                //var dr = VIS.DB.executeReader(sql, param);
                //var count = 1;
                //while (dr.read()) {
                //    var line = {};
                //    line['M_AttributeSetInstance_ID'] = dr.getInt("M_AttributeSetInstance_ID");
                //    line['Description'] = dr.getString("Description");
                //    line['Lot'] = dr.getString("Lot");
                //    line['SerNo'] = dr.getString("SerNo");
                //    line['GuaranteeDate'] = dr.getString("GuaranteeDate");
                //    line['Value'] = dr.getString("Value");
                //    line['QtyReserved'] = dr.getString("QtyReserved");
                //    line['QtyOrdered'] = dr.getString("QtyOrdered");
                //    line['QtyOnHand'] = dr.getString("QtyOnHand");
                //    line['GoodForDays'] = dr.getString("GoodForDays");
                //    line['ShelfLifeDays'] = dr.getString("ShelfLifeDays");
                //    line['ShelfLifeRemainingPct'] = dr.getString("ShelfLifeRemainingPct");
                //    line['M_Locator_ID'] = dr.getString("M_Locator_ID");
                //    line['recid'] = count;
                //    count++;
                //    data.push(line);
                //}
                enableButtons();
            }
            catch (e) {
                //$self.log.Log(Level.SEVERE, sql, e);
                console.log(e);
            }

            loadGrid(data);
        }

        function loadGrid(data) {

            if ($self.dGrid != null) {
                $self.dGrid.destroy();
                $self.dGrid = null;
            }
            if ($self.arrListColumns.length == 0) {

                $self.arrListColumns.push({ field: "Description", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Description"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "Lot", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Lot"), sortable: true, size: '10%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "SerNo", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "SerNo"), sortable: true, size: '10%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "GuaranteeDate", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "GuaranteeDate"), sortable: true, size: '10%', min: 100, hidden: false, render: 'date', options: { format: 'yyyy-mm-dd' } });
                $self.arrListColumns.push({ field: "Value", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_Locator_ID"), sortable: true, size: '10%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "QtyReserved", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyReserved"), sortable: true, size: '15%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "QtyOrdered", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyOrdered"), sortable: true, size: '15%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "QtyOnHand", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyOnHand"), sortable: true, size: '15%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "GoodForDays", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "GoodForDays"), sortable: true, size: '15%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "ShelfLifeDays", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "ShelfLifeDays"), sortable: true, size: '15%', min: 100, hidden: false });
                $self.arrListColumns.push({ field: "ShelfLifeRemainingPct", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "ShelfLifeRemainingPct"), sortable: true, size: '20%', min: 100, hidden: false });

                $self.arrListColumns.push({ field: "M_Locator_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_Locator_ID"), sortable: true, size: '16%', min: 150, hidden: true });
                $self.arrListColumns.push({ field: "M_AttributeSetInstance_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ID"), sortable: true, size: '16%', min: 150, hidden: true });
                $self.arrListColumns.push({ field: "AttrCode", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "AttrCode"), sortable: true, size: '16%', min: 150, hidden: true });

            }
            setTimeout(10);
            w2utils.encodeTags(data);
            $self.dGrid = $(middeldiv).w2grid({
                name: "gridPinstance" + windowNo,
                show: {
                    toolbar: true,
                    footer: true
                },
                recordHeight: 30,
                columns: $self.arrListColumns,
                records: data,
                //Added by Manjot To implement Search Functionality on Grid 10 May 2018 google Sheet ID SI_0607
                multiSearch: true,
                searches: [
                    { field: 'Description', caption: 'Description', type: 'text' },
                    { field: 'Lot', caption: 'Lot', type: 'text' },
                    { field: 'SerNo', caption: 'SerNo', type: 'text' },
                    { field: 'GuaranteeDate', caption: 'GuaranteeDate', type: 'date', options: { format: 'yyyy-mm-dd' } }
                ],
                //end
                onClick: function (event) {
                    if ($self.dGrid.records.length > 0) {
                        btnOk.removeAttr("disabled");
                    }
                },
            });
            //Added by Manjot To implement Search Functionality on Grid 10 May 2018 google Sheet ID SI_0607
            // 
            if (!disableSearch) {
                if (SerNo)
                    $self.dGrid.search('all', SerNo);
                else if (lotNo)
                    $self.dGrid.search('all', lotNo);
            }
            //$self.dGrid.hideColumn('M_Locator_ID');
            $busyDiv.css("display", 'none');
        }

        function enableButtons() {
            var recid = -1;
            var enabled = recid != -1;
            if ($self.dGrid != null) {
                recid = Number($self.dGrid.getSelection().toString());
                mAttributeSetInstanceID = -1;
                mAttributeSetInstanceName = null;
                mLocatorID = 0;
                enabled = recid != -1;
                if (enabled && recid > 0) {
                    var id = $self.dGrid.get(recid).M_AttributeSetInstance_ID;
                    if (id > 0) {
                        mAttributeSetInstanceID = id;
                        mAttributeSetInstanceName = $self.dGrid.get(recid).Description;
                        mLocatorID = $self.dGrid.get(recid).M_Locator_ID;
                        attrCode = $self.dGrid.get(recid).AttrCode;
                    }
                }
            }

            btnOk.attr('disabled', true);
            if (enabled) {
                btnOk.removeAttr("disabled");
            }

            $self.log.fine("M_AttributeSetInstance_ID=" + mAttributeSetInstanceID + " - " + mAttributeSetInstanceName + "; M_Locator_ID=" + mLocatorID);
        }

        function events() {

            if (btnOk != null)
                btnOk.on(VIS.Events.onTouchStartOrClick, function () {
                    enableButtons();
                    if ($self.onClose)
                        $self.onClose(mAttributeSetInstanceID, mAttributeSetInstanceName, mLocatorID, attrCode);
                    $root.dialog('close');
                });

            if (btnCancel != null)
                btnCancel.on(VIS.Events.onTouchStartOrClick, function () {
                    $root.dialog('close');
                });

            if (chkShowAll != null) {
                chkShowAll.change(function () {
                    disableSearch = true;
                    refresh();
                });
            }
            //10 May 2018
            //if (btnSearch != null) {
            //    btnSearch.on(VIS.Events.onTouchStartOrClick, function () {
            //        alert(txtSearch.val());
            //    });
            //}
        }

        this.showDialog = function () {
            $root.dialog({
                modal: true,
                title: mtitle, // VIS.Msg.translate(VIS.Env.getCtx(), mtitle),
                width: 600,
                height: 450,
                resizable: false,
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });

            refresh();
        };

        this.disposeComponent = function () {

            if (btnOk)
                btnOk.off("click");
            if (btnCancel)
                btnCancel.off("click");

            btnOk = null;
            btnCancel = null;
            //10 May 2018
            //btnSearch = null;
            //txtSearch = null;

            windowNo = null;
            this.onClose = null;
            $busyDiv = null;

            mWarehouseID = null;
            mLocatorID = null;
            mProductID = null;
            mCBPartnerID = null;
            mtitle = null;
            issotrx = null;
            this.arrListColumns = null;
            this.dGrid = null;
            msqlFrom = null;
            msqlWhere = null;
            msqlNonZero = null;
            msqlMinLife = null;
            this.log = null;
            chkShowAll = null;
            topdiv = null;
            middeldiv = null;
            bottomdiv = null;

            mAttributeSetInstanceID = null;
            mAttributeSetInstanceName = null;
            msql = null;
            this.disposeComponent = null;
        };

        dynInit(mCBPartnerID);

    };

    //dispose call
    PAttributeInstance.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    VIS.PAttributeInstance = PAttributeInstance;

})(VIS, jQuery);
