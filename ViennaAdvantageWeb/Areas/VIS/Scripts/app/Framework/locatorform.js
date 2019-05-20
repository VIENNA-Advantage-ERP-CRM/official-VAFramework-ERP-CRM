; (function (VIS, $) {
    //form declaretion
    function LocatorForm(columnName, lookup, locatorId, mandatory, warehouseId, windowNum) {

        this.onClose = null;
        this.change = false;

        var $root = $("<div style='position:relative;'>");
        var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");


        var Okbtn = null;
        var cancelbtn = null;
        var $self = this;
        var sep = null;

        var columnName = columnName;
        var locatorId = locatorId;
        var mandatory = mandatory;
        var onlyWarehouseId = warehouseId;
        var warehouseId = 0;
        var windowNo = windowNum;
        var mLocator = lookup;

        //controlls
        var cmbLocator = null;
        var chkCreateNew = null;
        var divCreateNew = null;
        var divWarehouseInfo = null;
        var divWarehouse = null;
        var txtWarehouseInfo = null;
        var cmbWarehouse = null;
        var txtX = null;
        var txtY = null;
        var txtZ = null;
        var txtValue = null;

        var lblX = null;
        var lblY = null;
        var lblZ = null;
        var lblValue = null;
        var lblWarehouse = null;

        setBusy = function (isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.load = function () {
            $root.load(VIS.Application.contextUrl + 'Locator/Index/?windowno=' + windowNo + '&locatorId=' + locatorId, function (event) {
                setBusy(true);
                init()
                setBusy(false);
            });
        };

        this.showDialog = function () {
            $root.append($busyDiv);
            $root.dialog({
                modal: true,
                resizable: false,
                title: VIS.Msg.translate(VIS.Env.getCtx(), "Locator"),
                width: 510,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });
        };

        this.disposeComponent = function () {

            if (Okbtn)
                Okbtn.off("click");
            if (cancelbtn)
                cancelbtn.off("click");

            Okbtn = null;
            cancelbtn = null;
            $self = null;

            columnName = null;
            locatorId = null;
            mandatory = null;
            onlyWarehouseId = null;
            warehouseId = null;
            windowNo = null;
            mLocator = null;

            divCreateNew = null;
            divWarehouseInfo = null;
            divWarehouse = null;

            //controlls
            cmbLocator = null;
            chkCreateNew = null;
            txtWarehouseInfo = null;
            cmbWarehouse = null;

            txtX = null;
            txtY = null;
            txtZ = null;
            txtValue = null;

            lblX = null;
            lblY = null;
            lblZ = null;
            lblValue = null;
            lblWarehouse = null;

            this.disposeComponent = null;
            this.load = null;
            sep = null;
        };

        function init() {
            Okbtn = $root.find("#btnOk_" + windowNo);
            cancelbtn = $root.find("#btnCancel_" + windowNo);
            cmbLocator = $root.find("#cmbLocator_" + windowNo);
            chkCreateNew = $root.find("#chkCreateNew_" + windowNo);
            txtWarehouseInfo = $root.find("#txtWarehouseInfo_" + windowNo);
            cmbWarehouse = $root.find("#cmbWarehouse_" + windowNo);
            txtX = $root.find("#txtX_" + windowNo);
            txtY = $root.find("#txtY_" + windowNo);
            txtZ = $root.find("#txtZ_" + windowNo);
            txtValue = $root.find("#txtValue_" + windowNo);
            divCreateNew = $root.find("#divchkCreateNew_" + windowNo);
            divWarehouseInfo = $root.find("#divWarehouseInfo_" + windowNo);
            divWarehouse = $root.find("#divWarehouse_" + windowNo);

            lblX = $root.find("#lblX_" + windowNo);
            lblY = $root.find("#lblY_" + windowNo);
            lblZ = $root.find("#lblZ_" + windowNo);
            lblValue = $root.find("#lblValue_" + windowNo);
            lblWarehouse = $root.find("#lblWarehouse_" + windowNo);

            //check Arebic Calture
            if (VIS.Application.isRTL) {
                Okbtn.css("margin-right", "-132px");
                cancelbtn.css("margin-right", "55px");
                divCreateNew.children(0).css("float", "left");
            }

            //fill locator in the control
            mLocator.refresh();
            mLocator.fillCombobox(mandatory, true, true, false);
            var selectedIndex = 0;
            //load locator and set selected locator into the combo
            for (var i = 0; i < mLocator.data.length ; i++) {
                cmbLocator.append(" <option value=" + mLocator.data[i].Key + ">" + mLocator.data[i].Name + "</option>");
                if (locatorId == mLocator.data[i].Key) {
                    selectedIndex = i;
                }
            };
            cmbLocator.prop('selectedIndex', selectedIndex);

            //load warehouse combo
            //var sql = "SELECT M_Warehouse_ID, Name FROM M_Warehouse";
            //if (onlyWarehouseId != 0) {
            //    sql += " WHERE M_Warehouse_ID=" + onlyWarehouseId;
            //}

            //var finalSql = VIS.MRole.addAccessSQL(sql, "M_Warehouse", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO) + " ORDER BY 2";
            //var ds = VIS.DB.executeDataSet(finalSql);

            var ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Locator/GetWarehouse", { "Warehouse_ID": onlyWarehouseId }, null);

            //for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //    cmbWarehouse.append(" <option value=" + ds.tables[0].getRow(i).getCell(0) + ">" + ds.tables[0].getRow(i).getCell(1) + "</option>");
            //}
            if (ds != null) {
                for (var i in ds) {
                    cmbWarehouse.append(" <option value=" + ds[i]["M_Warehouse_ID"] + ">" + ds[i]["Name"] + "</option>");
                }
            }
            cmbWarehouse.prop('selectedIndex', 0);

            //fill all textboxes on bases of selected locatorId
            displayLocator(ds);

            //show and hide controls
            if (mLocator.getIsOnlyOutgoing()) {
                divCreateNew.hide();
            }

            //var sel = chkCreateNew.prop("checked");
            //if (sel) {
            //    divWarehouseInfo.hide();
            //}
            //else {
            //    divWarehouse.hide();
            //}

            //txtX.attr("readonly", !sel);
            //txtY.attr("readonly", !sel);
            //txtZ.attr("readonly", !sel);
            //txtValue.attr("readonly", !sel);

            enableNew();
            function enableNew() {
                var sel = chkCreateNew.prop("checked");
                if (sel) {
                    divWarehouseInfo.hide();
                    divWarehouse.show();
                    txtX.removeClass("vis-gc-vpanel-table-readOnly");
                    txtY.removeClass("vis-gc-vpanel-table-readOnly");
                    txtZ.removeClass("vis-gc-vpanel-table-readOnly");
                    txtValue.removeClass("vis-gc-vpanel-table-readOnly");
                    cmbWarehouse.removeClass("vis-gc-vpanel-table-readOnly");
                    cmbWarehouse.removeAttr("disabled");
                }
                else {
                    divWarehouse.hide();
                    divWarehouseInfo.show();
                    txtX.addClass("vis-gc-vpanel-table-readOnly");
                    txtY.addClass("vis-gc-vpanel-table-readOnly");
                    txtZ.addClass("vis-gc-vpanel-table-readOnly");
                    txtValue.addClass("vis-gc-vpanel-table-readOnly");
                    cmbWarehouse.addClass("vis-gc-vpanel-table-readOnly");
                    cmbWarehouse.attr("disabled", true);
                }

                txtX.attr("readonly", !sel);
                txtY.attr("readonly", !sel);
                txtZ.attr("readonly", !sel);
                txtValue.attr("readonly", !sel);
            }

            function valueChange() {
                var buf = cmbWarehouse.find('option:selected').text() + " " + sep + " " + txtX.val()
           + " " + sep + " " + txtY.val()
           + " " + sep + " " + txtZ.val();
                txtValue.val(buf);
            }

            events();
            function events() {
                Okbtn.on("click", function () {
                    saveLocator();
                    if ($self.onClose)
                        $self.onClose(locatorId);
                    $root.dialog('close');
                });

                cancelbtn.on("click", function () {
                    $root.dialog('close');
                });

                chkCreateNew.change(function () {
                    enableNew();
                });

                txtWarehouseInfo.change(function () { });
                txtX.change(function () { valueChange(); });
                txtY.change(function () { valueChange(); });
                txtZ.change(function () { valueChange(); });

                cmbWarehouse.change(function () {
                    valueChange();
                });

                cmbLocator.change(function () {
                    displayLocator(ds);
                });
            };
        };

        function saveLocator() {
            if (chkCreateNew.prop("checked")) {
                var mandatoryFields = "";
                if (txtValue.val().trim().length == 0) {
                    mandatoryFields += lblValue.val() + " - ";
                }
                if (txtX.val().trim().length == 0) {
                    mandatoryFields += lblX.val() + " - ";
                }
                if (txtY.val().trim().length == 0) {
                    mandatoryFields += lblY.val() + " - ";
                }
                if (txtZ.val().trim().length == 0) {
                    mandatoryFields += lblZ.val() + " - ";
                }

                var lblWText = lblWarehouse.val();
                var tValue = txtValue.val(), tX = txtX.val(), tY = txtY.val(), tZ = txtZ.val();
                var warehId = cmbWarehouse.find('option:selected').val();                

                if (warehId != 0) {
                    if (warehouseId != warehId) {
                        warehouseId = warehId;
                        //no need to get info from database
                    }
                }

                if (warehouseId == 0) {
                    mandatoryFields += lblWText + " - ";
                }

                if (mandatoryFields.length != 0) {
                    VIS.ADialog.error("FillMandatory", true, mandatoryFields.substring(0, mandatoryFields.length - 3));
                }
                else {
                    $.ajax({
                        url: VIS.Application.contextUrl + "Locator/Save",
                        dataType: "json",
                        async: false,
                        data: {
                            warehouseId: warehouseId,
                            tValue: tValue,
                            tX: tX,
                            tY: tY,
                            tZ: tZ
                        },
                        success: function (data) {
                            returnValue = data.locatorId;
                            locatorId = returnValue;
                            mLocator.refresh();
                        }
                    });
                }
            }
            else {
                $self.change = true;
            }
        };

        function displayLocator(ds) {
            locatorId = cmbLocator.find('option:selected').val();
            var separator = "";
            var warehouseValue = "";
            //var sql = "SELECT  w.name,l.x,l.y,l.z,l.value, w.M_Warehouse_ID,w.Value wValue,w.separator from m_warehouse w" +
            //      " inner join M_Locator l on w.m_warehouse_id=l.M_Warehouse_ID and l.m_locator_id=" + locatorId;

            //var dsw = VIS.DB.executeDataSet(sql);
            //if (dsw.tables[0].rows.length > 0) {
            //    txtWarehouseInfo.val(dsw.tables[0].getRow(0).getCell("name"));
            //    txtX.val(dsw.tables[0].getRow(0).getCell("x"));
            //    txtY.val(dsw.tables[0].getRow(0).getCell("y"));
            //    txtZ.val(dsw.tables[0].getRow(0).getCell("z"));
            //    txtValue.val(dsw.tables[0].getRow(0).getCell("value"));
            //    warehouseId = dsw.tables[0].getRow(0).getCell("M_Warehouse_ID");
            //    warehouseValue = dsw.tables[0].getRow(0).getCell("wValue");
            //    separator = dsw.tables[0].getRow(0).getCell("separator");
            //}
            var dsw = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Locator/GetWarehouseData", { "Locator_ID": locatorId }, null);
            // Done by Bharat on 24 Jan 2019
            // JID_1084: When there is no locator in warehouse system is not allowing to create the locator from the locator control
            if (ds != null && dsw != null) {
                txtWarehouseInfo.val(dsw["Name"]);
                txtX.val(dsw["x"]);
                txtY.val(dsw["y"]);
                txtZ.val(dsw["z"]);
                txtValue.val(dsw["Value"]);
                warehouseId = dsw["M_Warehouse_ID"];
                warehouseValue = dsw["wValue"];
                separator = dsw["Separator"];
            }

            sep = separator;
            //show values in the value text box
            var buf = warehouseValue + " " + separator + " " + txtX.val()
            + " " + separator + " " + txtY.val()
            + " " + separator + " " + txtZ.val();

            txtValue.val(buf);

            //	Set Warehouse
            if (ds != null) {
                for (var i in ds) {
                    if (warehouseId == ds[i]["M_Warehouse_ID"]) {
                        cmbWarehouse.prop('selectedIndex', i);
                        break;
                    }
                }
            }
        }
    };

    LocatorForm.prototype.dispose = function () {
        this.disposeComponent();
    };



    VIS.LocatorForm = LocatorForm;

})(VIS, jQuery);