
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};

    function VCharge() {
        this.frame;
        this.windowNo;

        var $self = this;
        var arrListColumns = [];
        var dGrid = null;
        // Create Log
        this.log = VIS.Logging.VLogger.getVLogger("VCharge");

        var toggle = false;
        var toggleGen = false;
        var toggleside = false;

        var lblSearchKey = new VIS.Controls.VLabel(VIS.Msg.getMsg("SearchKey", false, false), "SearchKey", false, true);
        var lblName = new VIS.Controls.VLabel(VIS.Msg.getMsg("Name", false, false), "Name", false, true);
        var txtSearchKey = new VIS.Controls.VTextBox("SearchKey", true, false, true, 50, 100, null, null, false);
        var txtName = new VIS.Controls.VTextBox("Name", false, false, true, 50, 100, null, null, false);
        var chkExpense = new VIS.Controls.VCheckBox("Expense", false, false, true, VIS.Msg.getMsg("Expense", false, false), null);
        var chkSelectAll = new VIS.Controls.VCheckBox("SelectAll", false, false, true, VIS.Msg.getMsg("SelectAll", false, false), null);
        var btnCreateCharge = null;
        var btnAccount = null;
        var lblStatusInfo = new VIS.Controls.VLabel();
        this.log = VIS.Logging.VLogger.getVLogger("VCharge");

        var $root = $("<div style='width: 100%; height: 100%; background-color: white; '>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var leftsideDiv = null;
        var rightSideGridDiv = null;

        var topLeftDiv = null;
        var paradiv = null;
        var gridDiv = null;
        var bottumDiv = null;

        var btnCancel = null;
        var btnRefresh = null;
        var btnToggel = null;

        var searchValueText = "";
        var searchNameText = "";
        var searchChkProp = true;


        var sideDivWidth = 260;
        var minSideWidth = 50;
        var selectDivWidth = $(window).width() - (sideDivWidth + 20 + 5);
        var selectDivFullWidth = $(window).width() - (20 + minSideWidth);
        var selectDivToggelWidth = selectDivWidth + sideDivWidth + 5;
        var sideDivHeight = $(window).height() - 210;


        /** Account Element     */
        var m_C_Element_ID = 0;
        /** AccountSchema       */
        var m_C_AcctSchema_ID = 0;
        /** Default Charge Tax Category */
        var m_C_TaxCategory_ID = 0;
        var m_AD_Client_ID = 0;
        var m_AD_Org_ID = 0;

        function initializeComponent() {
            setBusy(true);
            chkExpense.setValue(true);
            var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-left.png";
            //left side div
            leftsideDiv = $("<div id='sideDiv_" + $self.windowNo + "' class='vis-archive-left-sidebar vis-leftsidebarouterwrap'>");//" + sideDivHeight + "

            //topLeftSide div
            topLeftDiv = $("<div id='btnSpaceDiv_" + $self.windowNo + "' class='vis-archive-l-s-head'>" +
                "<button id='btnSpace_" + $self.windowNo + "' class='vis-archive-sb-t-button'>" +
                "<i class='vis vis-arrow-left'></i></button></div>" +
                "</div>");

            //left side parameter div
            paradiv = $("<div class='vis-archive-l-s-content' id='parameterDiv_" + $self.windowNo + "'>");
            leftsideDiv.append(topLeftDiv).append(paradiv);

            //Refresh
            src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
            btnRefresh = $("<button id='btnRefresh_" + $self.windowNo + "' style='margin-left: 12px;' class='VIS_Pref_btn-2 vis-frm-button vis-pull-left'><i class='vis vis-refresh'></i></button>");
            //JID_0134: Change the button name as "Create New" available at left section.
            //Create charge
            btnCreateCharge = $("<input id='btnCreateCharge_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button' type='button' value='" + VIS.Msg.getMsg("CreateNew", false, false) + "'>");
            //Cancel button
            btnCancel = $("<input id='btnCancel_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button' type='button' value='" + VIS.Msg.getMsg("Cancel", false, false) + "'>");
            //JID_0134: Change the button name as "Create From Account" at right section.
            //Create charge from Account
            btnAccount = $("<input id='btnAccount_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button' type='button' value='" + VIS.Msg.getMsg("ChargeFromAccount", false, false) + "'>");


            var tble = $("<table style='width: 100%;'>");
            var tr = $("<tr>");
            var td = $("<td style='padding: 0px 10px 0px;'>");
            paradiv.append(tble);
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblSearchKey.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtSearchKey.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            Leftformfieldctrlwrp.append(lblSearchKey.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblName.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtName.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            Leftformfieldctrlwrp.append(lblName.getControl().addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(chkExpense.getControl());

            bottumDiv = $("<div class='vis-frm-bot-btn-wrp'>");
            var buttonDiv = $("<div class='vis-frm-bttsec'>");
            buttonDiv.append(btnAccount).append(btnCancel);

            bottumDiv.append(btnCreateCharge).append(btnRefresh);

            //Right side div
            rightSideGridDiv = $("<div id='rightSideGridDiv_" + $self.windowNo + "' class='vis-frm-grid-wrap'>");
            //rightSideGridDiv.css("width", selectDivWidth);

            gridDiv = $("<div id='gridDiv_" + $self.windowNo + "' class='vis-frm-grid-inn'>");
            rightSideGridDiv.append(gridDiv);
            rightSideGridDiv.append(buttonDiv);
            //Add to root
            leftsideDiv.append(bottumDiv);
            $root.append($busyDiv);
            $root.append(leftsideDiv).append(rightSideGridDiv);
        }

        function loadGrid(data) {

            if (dGrid != null) {
                dGrid.destroy();
                dGrid = null;
            }
            if (arrListColumns.length == 0) {
                arrListColumns.push({
                    field: "C_ElementValue_ID", caption: VIS.Msg.getMsg("C_ELEMENTVALUE_ID"),
                    sortable: true, size: '16%', min: 150, hidden: true
                });
                // arrListColumns.push({ field: "Select1", caption: VIS.Msg.getMsg("Select"), sortable: true, size: '25%', min: 150, hidden: false, editable: { type: 'checkbox' } });
                arrListColumns.push({ field: "Value", caption: VIS.Msg.getMsg("SearchKey"), sortable: true, size: '25%', min: 150, hidden: false });
                arrListColumns.push({ field: "Name", caption: VIS.Msg.getMsg("Name"), sortable: true, size: '25%', min: 150, hidden: false });
                arrListColumns.push({ field: "Expense", caption: VIS.Msg.getMsg("Expense"), sortable: true, size: '25%', min: 150, hidden: false, editable: { type: 'checkbox' } });
            }

            w2utils.encodeTags(data);
            dGrid = $(gridDiv).w2grid({
                name: "gridAccountElement" + $self.windowNo,
                recordHeight: 40,
                show: { selectColumn: true },
                multiSelect: true,
                columns: arrListColumns,
                records: data
            });
        }

        // Get data from c_elementvalue table
        function dynInit() {
            var data = [];
            m_C_AcctSchema_ID = VIS.Env.getCtx().getContextAsInt("$C_AcctSchema_ID");
            m_AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();
            m_AD_Org_ID = VIS.Env.getCtx().getAD_Org_ID();

            $.ajax({
                url: VIS.Application.contextUrl + "VCharge/VChargeLodGrideData",
                dataType: "json",
                //async: false,
                data: {
                    mcAcctSchemaID: m_C_AcctSchema_ID,
                    mADClientId: m_AD_Client_ID,
                    //valueSearch: searchValueText,
                    //nameSearch: searchNameText,
                    //chkExpenses: searchChkProp
                },
                success: function (results) {
                    if (results.result.meID == 0) {
                        return;
                    }
                    if (results.result && results.result.VchargeMCElemTaxCatID) {
                        var MCElemTaxCatID = results.result.VchargeMCElemTaxCatID;
                        m_C_Element_ID = results.result.VchargeMCElemTaxCatID[0]["mCElementID"];
                        if (m_C_Element_ID == 0) {
                            return;
                        }
                        m_C_TaxCategory_ID = results.result.VchargeMCElemTaxCatID[0]["mCTaxCategoryID"];
                    }

                    if (results.result && results.result.VchargeCElementValue) {
                        var vals = results.result.VchargeCElementValue;
                        var count = 1;
                        for (var i = 0; i < vals.length; i++) {
                            var line = {};
                            line['C_ElementValue_ID'] = vals[i]["C_ElementValue_ID"];
                            line['Value'] = vals[i]["Value"];
                            line['Name'] = vals[i]["Name"];
                            line['Expense'] = vals[i]["Expense"] == "false" ? false : true;
                            line['recid'] = count;
                            count++;
                            data.push(line);
                        }
                        loadGrid(data);
                        setBusy(false);
                    }
                },
                error: function (e) {
                    setBusy(false);
                    $self.log.info(e);
                }
            });

            //loadGrid(data);
            //setBusy(false);
            //return data;
        }




        //function dynInit() {
        //    var data = [];
        //    m_C_AcctSchema_ID = VIS.Env.getCtx().getContextAsInt("$C_AcctSchema_ID");
        //    m_AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();
        //    m_AD_Org_ID = VIS.Env.getCtx().getAD_Org_ID();

        //    var sql = "SELECT C_Element_ID FROM C_AcctSchema_Element "
        //                   + "WHERE ElementType='AC' AND C_AcctSchema_ID= " + m_C_AcctSchema_ID;
        //    m_C_Element_ID = VIS.DB.executeScalar(sql, null, null);
        //    if (m_C_Element_ID == 0)
        //        return;
        //    sql = "SELECT C_TaxCategory_ID FROM C_TaxCategory WHERE IsDefault='Y' AND AD_Client_ID=" + m_AD_Client_ID;
        //    m_C_TaxCategory_ID = VIS.DB.executeScalar(sql, null, null);

        //    //  Create SQL
        //    sql = "SELECT 'false' as Select1,C_ElementValue_ID,Value, Name, case when AccountType = 'E' THEN 'true' else 'false' end as Expense"
        //           + " FROM C_ElementValue "
        //           + " WHERE AccountType IN ('R','E')"
        //           + " AND IsSummary='N'"
        //           + " AND C_Element_ID= " + m_C_Element_ID
        //           + " ORDER BY 2 desc";
        //    try {
        //        var dr = VIS.DB.executeReader(sql.toString(), null, null);
        //        var count = 1;
        //        while (dr.read()) {
        //            var line = {};
        //            // line['Select1'] = false;
        //            line['C_ElementValue_ID'] = dr.getString(1);
        //            line['Value'] = dr.getString(2);
        //            line['Name'] = dr.getString(3);
        //            line['Expense'] = dr.getString(4) == "false" ? false : true;
        //            line['recid'] = count;
        //            count++;
        //            data.push(line);
        //        }
        //    }
        //    catch (e) {
        //    }
        //    loadGrid(data);
        //    setBusy(false);
        //    return data;
        //}

        setBusy = function (isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        function createCharge(m_C_AcctSchema_ID, m_C_TaxCategory_ID, name, C_ElementValue_ID, expense) {
            $.ajax({
                url: VIS.Application.contextUrl + "VCharge/CreateCharge",
                dataType: "json",
                //async: false,
                data: {
                    m_C_AcctSchema_ID: m_C_AcctSchema_ID,
                    m_C_TaxCategory_ID: m_C_TaxCategory_ID,
                    name: name,
                    C_ElementValue_ID: C_ElementValue_ID,
                    expense: expense
                },
                success: function (data) {
                    if (data.ID == 0) {
                        VIS.ADialog.info("ChargeNotCreated", true, null);
                        setBusy(false);
                        return;
                    }
                    else {
                        // JID_0134:System should refresh the form once charge is created and clear the value in Value, Name and Expense if charge is created succussfully.
                        txtSearchKey.ctrl.val("");
                        txtName.ctrl.val("");
                        var C_Charge_ID = data.ID;
                        VIS.ADialog.info("ChargeCreated", true, null);
                        dynInit()
                    }
                }
            });
        }

        function createChargeByList(m_C_AcctSchema_ID, m_C_TaxCategory_ID, namelst, C_ElementValue_IDlst, expenselst) {
            $.ajax({
                url: VIS.Application.contextUrl + "VCharge/CreateChargeByList",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                //async: false,
                data: JSON.stringify({
                    'AcctSchemaID': m_C_AcctSchema_ID,
                    'TaxCategoryID': m_C_TaxCategory_ID,
                    'namepara': namelst,
                    'ElementValuID': C_ElementValue_IDlst,
                    'expense': expenselst
                }),
                success: function (data) {
                    if (data.listCreatedP) {
                        VIS.ADialog.info("ChargeCreated", true, null);
                    }
                    if (data.listRejectedP) {
                        VIS.ADialog.info("ChargeNotCreated", true, null);

                    }
                    setBusy(false);
                }
            });
        }

        this.Initialize = function () {
            initializeComponent();
            btnToggel = $root.find("#btnSpace_" + $self.windowNo);

            //Events

            if (btnCancel != null)
                btnCancel.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.dispose();
                });

            if (btnRefresh != null)
                btnRefresh.on(VIS.Events.onTouchStartOrClick, function () {
                    //var getCtrl = $(this).parents().find("#parameterDiv_" + $self.windowNo + "");
                    //searchValueText = getCtrl.find("input").val();
                    //searchNameText = getCtrl.find('input[name="Name"]').val();
                    //searchChkProp = getCtrl.find('input[name="Expense"]').is(":checked");

                    // JID_0134:clear the value in Value, Name and Expense if charge is created succussfully.
                    txtSearchKey.ctrl.val("");
                    txtName.ctrl.val("");
                    setBusy(true);
                    dynInit();
                    //setBusy(false);
                });

            if (btnToggel != null)
                btnToggel.on(VIS.Events.onTouchStartOrClick, function () {
                    var borderspace = 0;
                    if (toggleside) {
                        if (VIS.Application.isRTL) {
                            borderspace = 180;
                        }
                        else {
                            borderspace = 0;

                        }
                        btnCreateCharge.show();
                        btnRefresh.show();
                        lblSearchKey.getControl().show();
                        lblName.getControl().show();
                        txtSearchKey.getControl().show();
                        txtName.getControl().show();
                        chkExpense.getControl().show();

                        btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = false;
                        // btnToggel.animate({ width: sideDivWidth }, "slow");
                        rightSideGridDiv.animate({ width: selectDivWidth }, "slow");
                        paradiv.css("display", "block");
                        //topLeftDiv.animate({ width: sideDivWidth }, "slow");
                        leftsideDiv.animate({ width: sideDivWidth }, "slow", null, function () {
                            dGrid.resize();
                        });
                    }
                    else {
                        if (VIS.Application.isRTL) {
                            borderspace = 0;
                        }
                        else {
                            borderspace = 180;

                        }
                        btnCreateCharge.hide();
                        btnRefresh.hide();
                        lblSearchKey.getControl().hide();
                        lblName.getControl().hide();
                        txtSearchKey.getControl().hide();
                        txtName.getControl().hide();
                        chkExpense.getControl().hide();
                        btnToggel.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = true;
                        // btnToggel.animate({ width: minSideWidth }, "slow");
                        //topLeftDiv.animate({ width: minSideWidth }, "slow");
                        leftsideDiv.animate({ width: minSideWidth }, "slow");
                        paradiv.css("display", "none");
                        rightSideGridDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                            dGrid.resize();
                        });
                    }
                });

            //size chnage 
            this.sizeChanged = function (h, w) {
                selectDivWidth = w - (sideDivWidth + 1);
                selectDivFullWidth = w - (minSideWidth + 1);
                if (toggleside == true) {
                    leftsideDiv.animate({ width: minSideWidth }, "slow");
                    rightSideGridDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                        dGrid.resize();
                    });
                }
                else {
                    rightSideGridDiv.animate({ width: selectDivWidth }, "slow");
                    leftsideDiv.animate({ width: sideDivWidth }, "slow", null, function () {
                        dGrid.resize();
                    });
                }
            }

            if (btnAccount != null)
                btnAccount.on(VIS.Events.onTouchStartOrClick, function () {
                    setBusy(true);
                    //var eleValue_ID = "";
                    //var eleName = "";
                    //var expense = "";
                    var eleValue_ID = [];
                    var eleName = [];
                    var expense = [];
                    if (!dGrid) {
                        return;
                    }
                    var selectedItems = dGrid.getSelection();
                    if (!selectedItems) {
                        setBusy(false);
                        return;
                    }
                    if (selectedItems.length <= 0) {
                        setBusy(false);
                        return;
                    }
                    var splitValue = selectedItems.toString().split(',');
                    //for (var i = 0; i < splitValue.length; i++) {
                    //    eleValue_ID += (dGrid.get(splitValue[i])).C_ElementValue_ID + ",";
                    //    eleName += (dGrid.get(splitValue[i])).Name + ",";
                    //    expense += (dGrid.get(splitValue[i])).Expense + ",";
                    //}

                    //if (eleValue_ID.length == 0) {
                    //    return "";
                    //}
                    //eleValue_ID = eleValue_ID.replace(/,\s*$/, "");
                    //eleName = eleName.replace(/,\s*$/, "");
                    //expense = expense.replace(/,\s*$/, "");

                    for (var i = 0; i < splitValue.length; i++) {
                        eleValue_ID[i] = (dGrid.get(splitValue[i])).C_ElementValue_ID;
                        eleName[i] = (dGrid.get(splitValue[i])).Name;
                        expense[i] = (dGrid.get(splitValue[i])).Expense;
                    }

                    if (eleValue_ID.length == 0) {
                        setBusy(false);
                        return "";
                    }
                    createChargeByList(m_C_AcctSchema_ID, m_C_TaxCategory_ID, eleName, eleValue_ID, expense);
                });

            if (btnCreateCharge != null)
                btnCreateCharge.on(VIS.Events.onTouchStartOrClick, function () {
                    setBusy(true);
                    var expense = chkExpense.getValue();
                    var value = txtSearchKey.getValue().trim();
                    if (value.length == 0) {
                        VIS.ADialog.warn("SearchKeyMandatory", true, null);
                        setBusy(false);
                        return;
                    }
                    var name = txtName.getValue().trim()
                    if (name.length == 0) {
                        VIS.ADialog.warn("NameMandatory", true, null);
                        setBusy(false);
                        return;
                    }

                    //  Create Element
                    $.ajax({
                        url: VIS.Application.contextUrl + "VCharge/CreateElementValue",
                        dataType: "json",
                        //async: false,
                        data: {
                            m_AD_Org_ID: m_AD_Org_ID,
                            value: value,
                            name: name,
                            expense: expense,
                            m_C_Element_ID: m_C_Element_ID
                        },
                        success: function (data) {
                            if (data.ID == 0) {
                                $self.log.saveError("ChargeNotCreated", data.Msg);
                                VIS.ADialog.warn(data.Msg, true, null);
                                setBusy(false);
                                return;
                            }
                            else {
                                createCharge(m_C_AcctSchema_ID, m_C_TaxCategory_ID, name, data.ID, expense);
                            }
                        }
                    });
                });
        }

        this.display = function () {
            setTimeout(
                dynInit(), 5);
        }

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        this.disposeComponent = function () {

            if (btnCancel)
                btnCancel.off(VIS.Events.onTouchStartOrClick);
            if (btnRefresh)
                btnRefresh.off(VIS.Events.onTouchStartOrClick);
            if (btnToggel)
                btnToggel.off(VIS.Events.onTouchStartOrClick);
            if (btnAccount)
                btnAccount.off(VIS.Events.onTouchStartOrClick);

            $self.windowNo = null;
            arrListColumns = null;
            dGrid = null;

            toggle = null;
            toggleGen = null;
            toggleside = null;

            lblSearchKey = null;
            lblName = null;
            txtSearchKey = null;
            txtName = null;
            chkExpense = null;
            chkSelectAll = null;
            btnCreateCharg = null;
            btnAccount = null;
            lblStatusInfo = null;

            $self.log = null;
            $root = null;
            $busyDiv = null;

            leftsideDiv = null;
            rightSideGridDiv = null;

            topLeftDiv = null;
            paradiv = null;
            gridDiv = null;
            bottumDiv = null;

            btnCancel = null;
            btnRefresh = null;
            btnToggel = null;

            sideDivWidth = null;
            minSideWidth = null;
            selectDivWidth = null;
            selectDivFullWidth = null;
            selectDivToggelWidth = null;
            sideDivHeight = null;

            m_C_Element_ID = null;
            m_C_AcctSchema_ID = null;
            m_C_TaxCategory_ID = null;
            m_AD_Client_ID = null;
            m_AD_Org_ID = null;

            $self = null;
            this.getRoot = null;
            this.disposeComponent = null;
        };
    };

    //Must Implement with same parameter
    VCharge.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
        this.display();
    };

    //Must implement dispose
    VCharge.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps.AForms.VCharge = VCharge;


})(VIS, jQuery);