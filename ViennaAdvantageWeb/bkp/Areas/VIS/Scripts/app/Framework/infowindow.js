/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate info dialog
 * Author         :     Lakhwinder
 * Date           :     2-Aug-2014
  ******************************************************/
; (function (VIS, $) {
    function InfoWindow(_AD_InfoWindow_ID, ctrlValue, windowNo, validationCode, multiSelection) {

        this.onClose = null;


        var inforoot = $("<div class='vis-forms-container'>");
        var subroot = $("<div class='vis-info-subroot'>");
        var isExpanded = true;
        var sSContainer, ssHeader, btnExpander, searchTab, searchSec, datasec, btnsec;   /* decalare Variable */
        var bsyDiv, divbtnRight, btnCancel, btnOK, btnRequery;
        var dGrid = null;
        var info = null;
        var schema = null;
        var considerChkBox = false;
        var srchCtrls = [];
        this.keyCol = '';
        this.windowNo = windowNo;
        var self = this;
        var multiValues = [];
        var grdname = null;
        var divPaging, ulPaging, liFirstPage, liPrevPage, liCurrPage, liNextPage, liLastPage, cmbPage;
        var selectedItems = [];
        function initializeComponent() {

            inforoot.css("width", "100%");
            inforoot.css("height", "100%");
            inforoot.css("position", "relative");
            
            searchTab = $("<div class='vis-info-l-s-wrap vis-leftsidebarouterwrap'>");
                searchSec = $("<div>");
                searchTab.append(searchSec);
                datasec = $("<div class='vis-info-datasec'>");
                btnsec = $("<div class='vis-info-btnsec'>");
            
            subroot.append(searchTab);
            subroot.append(datasec);
            subroot.append(btnsec);



            var refreshtxt = VIS.Msg.getMsg("Refresh");
            if (refreshtxt.indexOf('&') > -1) {
                refreshtxt = refreshtxt.replace('&', '');
            }
            var canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }
            var Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            divbtnRight = $("<div class='vis-info-btnswrap'>");
            //if (VIS.Application.isRTL) {
            //    btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(canceltxt);
            //    btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(Oktxt);
            //}
            //else {
                btnCancel = $("<button class='VIS_Pref_btn-2'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2'>").append(Oktxt);
            //}

            var divbtnLeft = $("<div class='vis-info-btnleft'>");
            btnRequery = $("<button class='VIS_Pref_btn-2' style='margin-top: 10px;'>").append($("<i class='vis vis-refresh'></i>"))//.append(refreshtxt);

            //PagingCtrls
            divPaging = $('<div class="vis-info-pagingwrp">');
            createPageSettings();
            divPaging.append(ulPaging);

            //if (VIS.Application.isRTL) {
            //    divbtnLeft.append(btnCancel);
            //    divbtnLeft.append(btnOK);
            //    divbtnRight.append(btnRequery);
            //}
            //else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
                divbtnLeft.append(btnRequery);
            //}

            btnsec.append(divbtnLeft);
            btnsec.append(divPaging);
            btnsec.append(divbtnRight);

            //Busy Indicator
            bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //bsyDiv.css("width", "98%");
            //bsyDiv.css("height", "97%");
            //bsyDiv.css('text-align', 'center');
            //bsyDiv.css("position", "absolute");

            inforoot.append(subroot);
            inforoot.append(bsyDiv);
        };

        initializeComponent();

        function bindEvents() {
            btnOK.on("click", function () {
                btnOKClick();
            });


            btnRequery.on("click", function () {

                bsyDiv[0].style.visibility = 'visible';
                //if (ctrl != null) {
                if (validationCode != null && validationCode != undefined && validationCode.length > 0) {
                    validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), windowNo, validationCode, false, false);
                }
                // }
                displayData(true, cmbPage.val());
            });

            btnCancel.on("click", function () {

                if (self.onClose != null) {
                    self.onClose();
                }

                disposeComponent();
                //if (inforoot != null) {
                //    inforoot.dialog("close");
                //    inforoot = null;
                //}

                //var amtd = new VIS.AmountDivision();
                //amtd.show();
            });
            if (!VIS.Application.isMobile) {
                inforoot.on('keyup', function (e) {
                    if (!(e.keyCode === 13)) {
                        return;
                    }
                    bsyDiv[0].style.visibility = 'visible';
                    //if (ctrl != null) {
                    if (validationCode != null && validationCode != undefined && validationCode.length > 0) {
                        validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), windowNo, validationCode, false, false);
                    }
                    // }
                    displayData(true, cmbPage.val());
                });
            };
            

        }

        bindEvents();



        function onClosing() {

            if (self.onClose != null) {
                self.onClose();
            }

            disposeComponent();
            //if (inforoot != null) {
            //    //inforoot.dialog("close");
            //    inforoot = null;
            //}
        };

        this.show = function () {
            var InfoObj = self;// cached self object into variable
            $.ajax({
                url: VIS.Application.contextUrl + "InfoWindow/GetSearchColumn/?Ad_InfoWindow_ID=" + _AD_InfoWindow_ID,
                dataType: "json",
                error: function () {
                    alert(VIS.Msg.getMsg('ERRORGettingSchema'));
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {

                    if (data.result == null || data.result.Schema == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingSchema'));
                        return;
                    }
                    info = data.result;
                    schema = info.Schema;
                    if (schema == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingSchema'));
                        return;
                    }
                    
                    displaySchema(InfoObj);
                }
            });

        };

        var displaySchema = function (infoObject) {
            // debugger;
            var label = null;
            var ctrl = null;
            var tableSArea = $("<table>");
            tableSArea.css("width", "100%");
            var tr = null;


            var rowctrl = 0;
            var appendTopMargin = false;
            var displayType = null;
            var condition = null;
            var ctxValue = null;
            var suCondition = null;
            var key = null;
            var value = null;
            var conditionColumn = null;
            var conditionVal = null;
            var appVal = null;
            for (var item in schema) {

                if (schema[item].IsQueryCriteria) {
                    appendTopMargin = false;
                    //tr = $("<tr>");
                    //tableSArea.append(tr);

                    var td = $("<td class='vis-gc-vpanel-table-td1'>");
                    var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
                    var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
                    var Leftformfieldbtnwrap = $('<div class="input-group-append">');                    

                    tr = $("<tr>");
                    tableSArea.append(tr);
                    tr.append(td);
                    td.append(Leftformfieldwrp);
                    Leftformfieldwrp.append(Leftformfieldctrlwrp);

                    var srchCtrl = {
                    };
                    displayType = schema[item].AD_Reference_ID;
                    ctrl = getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                    srchCtrl.Ctrl = ctrl;
                    
                    // check if there is some value needed to be set on this control.- Added by Mohit- 13 Feb 2019
                    if (schema[item].SetValue != null) {
                        var retValue = false;
                          // If there is some condition given, then validate the condition                      
                        if (schema[item].Condition != null) {
                            var checkAll = [];
                            var logical = [];
                            var bracket = new VIS.StringTokenizer(schema[item].Condition, "[]");
                            while (bracket.hasMoreTokens()) {
                                var currentToken = bracket.nextToken();
                                if (currentToken.trim() != " & ".trim()) {
                                    retValue = VIS.Evaluator.evaluateLogic(infoObject, currentToken);
                                    checkAll.push(retValue);
                                }
                                else {
                                    logical.push(currentToken);
                                }
                            }

                            if (checkAll.length > 1) {
                                if (logical[0].trim().equals("&")) {
                                    if (checkAll.indexOf(false) > -1) //conatins
                                        retValue = false;
                                    else
                                        retValue = true;
                                }
                                else if (logical[0].trim().equals("|")) {
                                    if (checkAll.indexOf(true) > -1) // contains()
                                        retValue = true;
                                }
                            }
                            else if (checkAll.length == 1) {
                                if (checkAll[0] == true) {
                                    retValue = true;
                                }
                            }
                        }
                            // if no condition, by default true.
                        else {
                            retValue = true;
                        }
                        // if condition validated, then set value.
                        if (retValue) {
                            if (displayType == VIS.DisplayType.YesNo) {
                                if (schema[item].SetValue.toUpperCase() == "Y") {
                                    ctrl.getControl().find("input").prop("checked", true);
                                }
                                else if (schema[item].SetValue.toUpperCase() == "N") {
                                    ctrl.getControl().find("input").prop("checked", false);
                                }
                            }
                            else {
                                if (schema[item].SetValue.contains("@")) {
                                    conditionColumn = schema[item].SetValue.substring(0 + 1, schema[item].SetValue.lastIndexOf("@"));
                                    appVal = VIS.context.getWindowContext(windowNo, conditionColumn, false);
                                    if (!appVal.equals("")) {
                                        ctrl.setValue(appVal);
                                    }
                                    else {
                                        ctrl.setValue(null);
                                    }
                                }
                                else {

                                    ctrl.setValue(schema[item].SetValue);
                                }
                            }
                        }


                    }

                    
                    srchCtrl.AD_Reference_ID = schema[item].AD_Reference_ID;
                    srchCtrl.ColumnName = schema[item].SelectClause;
                    if (srchCtrl.ColumnName.toUpperCase().indexOf(" AS ") > -1) {
                        srchCtrl.SearchColumnName = srchCtrl.ColumnName.substring(0, srchCtrl.ColumnName.toUpperCase().indexOf(" AS "));
                    }
                    else {
                        srchCtrl.SearchColumnName = srchCtrl.ColumnName;
                    }

                    if (schema[item].ISIDENTIFIER) {
                        if (ctrlValue != null) {
                            ctrl.setValue(ctrlValue);
                        }
                    }

                    var tdctrl = $("<td>");
                    

                    

                    //tr.append(tdctrl);
                    var count = ctrl.getBtnCount();
                    if (count == 2) {
                        //var div = $("<div class='d-flex vis-info-ctrlwrap'>");
                        if (appendTopMargin) {
                            //div.css('margin-top', '5px');
                        }

                            Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                Leftformfieldbtnwrap.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2"));
                                ctrl.getControl().attr('data-hasbtn', ' ');
                            }
                            ctrlBtn = ctrl.getBtn(1);
                            if (ctrlBtn != null) {
                                Leftformfieldbtnwrap.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2"));
                                ctrl.getControl().attr('data-hasbtn', ' ');
                            }
                            count = -1;
                            ctrl = null;
                        //}
                        //tdctrl.append(div);
                        //td.append(div);
                        //Leftformfieldwrp.append(Leftformfieldctrlwrp);
                        Leftformfieldwrp.append(Leftformfieldbtnwrap);
                    }
                    else if (count == 1) {
                        //var div = $("<div class='d-flex vis-info-ctrlwrap'>");
                        if (appendTopMargin) {
                            //div.css('margin-top', '5px');
                        }
                       
                            Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                Leftformfieldbtnwrap.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2"));
                                ctrl.getControl().attr('data-hasbtn', ' ');
                            }
                            count = -1;
                            ctrl = null;
                        //}
                        //td.append(div);
                        //Leftformfieldwrp.append(Leftformfieldctrlwrp);
                        Leftformfieldwrp.append(Leftformfieldbtnwrap);
                    }
                    else {
                        if (appendTopMargin) {
                            ctrl.getControl();
                        }
                        if (schema[item].AD_Reference_ID == VIS.DisplayType.YesNo) {
                            ctrl.getControl();
                            //tr.addClass('vis-check-mb-dec');
                        }
                        Leftformfieldctrlwrp.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                    }

                    if (schema[item].IsRange) {
                        srchCtrl.IsRange = true;
                        //tr = $("<tr>");
                        var Leftformfieldwrpto = $('<div class="input-group vis-input-wrap">');
                        var Leftformfieldctrlwrpto = $('<div class="vis-control-wrap">');
                        var Leftformfieldbtnwrapto = $('<div class="input-group-append">');
                        //tableSArea.append(tr);

                        
                        tr = $("<tr>");
                        tableSArea.append(tr);

                        ctrl = getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name);
                        srchCtrl.CtrlTo = ctrl;
                        var tdctrlTo = $("<td>");
                        tr.append(tdctrlTo);
                        tdctrlTo.append(Leftformfieldwrpto);
                        Leftformfieldwrpto.append(Leftformfieldctrlwrpto);
                        var count = ctrl.getBtnCount();
                        if (count == 2) {
                            //var div = $("<div class='d-flex vis-info-ctrlwrap'>");
                            //div.css("width", "97%");
                            if (appendTopMargin) {
                                ctrl.getControl();
                            }
                            
                            Leftformfieldctrlwrpto.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
                                var ctrlBtn = ctrl.getBtn(0);
                                if (ctrlBtn != null) {
                                    Leftformfieldbtnwrapto.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2"));
                                    ctrl.getControl().attr('data-hasbtn', ' ');
                                }
                                ctrlBtn = ctrl.getBtn(1);
                                if (ctrlBtn != null) {
                                    Leftformfieldbtnwrapto.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2"));
                                    ctrl.getControl().attr('data-hasbtn', ' ');
                                }
                                count = -1;
                                ctrl = null;
                            //}
                            //td.append(div);
                            //Leftformfieldwrp.append(Leftformfieldctrlwrp);
                            Leftformfieldwrpto.append(Leftformfieldbtnwrapto);

                        }
                        else {
                            if (appendTopMargin) {
                                ctrl.getControl();
                            }
                            if (schema[item].AD_Reference_ID == VIS.DisplayType.YesNo) {
                                ctrl.getControl();
                                //tr.addClass('vis-check-mb-dec');
                            }
                            else {
                                ctrl.getControl();
                            }
                            Leftformfieldctrlwrpto.append(ctrl.getControl().attr('data-placeholder', '').attr('placeholder', ' ')); 

                        }
                        Leftformfieldctrlwrpto.append((new VIS.Controls.VLabel(VIS.Msg.getMsg("To"), schema[item].ColumnName)).getControl());


                        //tr.append(tdctrlTo);
                    }
                    else {
                        srchCtrl.IsRange = false;
                    }

                    rowctrl = rowctrl + 1;
                    srchCtrls.push(srchCtrl);

                    //	No Label for FieldOnly, CheckBox, Button
                    if (!(schema[item].AD_Reference_ID == VIS.DisplayType.YesNo
                        || schema[item].AD_Reference_ID == VIS.DisplayType.Button
                        || schema[item].AD_Reference_ID == VIS.DisplayType.Label)) {
                        //Set label
                        label = new VIS.Controls.VLabel(schema[item].Name, schema[item].ColumnName);
                        var lblctrl = label.getControl();
                        //lblctrl.css("font-weight", "inherit");
                        //td.append(lblctrl);
                        Leftformfieldctrlwrp.append(lblctrl);

                    }
                    else {
                        appendTopMargin = true;
                    }

                    
                }


            }
            //displayData();


            searchSec.append(tableSArea);







            var refreshtxt = VIS.Msg.getMsg("Refresh");
            if (refreshtxt.indexOf('&') > -1) {
                refreshtxt = refreshtxt.replace('&', '');
            }
            var canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }
            var Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            inforoot.dialog({
                width: 1020,
                height: 487,
                resizable: false,
                title: info.WindowName,
                modal: true,
                closeText: VIS.Msg.getMsg("close"),
                close: onClosing
            });
            if (validationCode != null && validationCode != undefined && validationCode.length > 0) {
                validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), windowNo, validationCode, false, false);
            }
            displayData(true, cmbPage.val());
        };

        var getControl = function (displayType, columnName, header, refvalueID, lookup) {
            var ctrl = null;
            if (displayType == VIS.DisplayType.Button) {
                var btn = new VIS.Controls.VButton(columnName, false, false, true, header, null, null, 0);
                ctrl = btn;
            }
            else if (displayType == VIS.DisplayType.String) {
                var txt = new VIS.Controls.VTextBox(columnName, false, false, true, 50, 100, null, null, false);
                ctrl = txt;
                txt.click = function () {
                    considerChkBox = true;
                };
            }
            else if (displayType == VIS.DisplayType.YesNo) {
                var chk = new VIS.Controls.VCheckBox(columnName, false, false, true, header, null);
                ctrl = chk;
            }
            else if (VIS.DisplayType.IsDate(displayType)) {
                if (displayType == VIS.DisplayType.DateTime)
                    readOnly = true;
                var vd = new VIS.Controls.VDate(columnName, false, false, true, displayType, header);
                vd.setName(columnName);
                ctrl = vd;
            }
            else if (VIS.DisplayType.IsLookup(displayType) || VIS.DisplayType.ID == displayType) {
                var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, displayType, columnName, refvalueID, false, null);
                if (displayType != VIS.DisplayType.Search && displayType != VIS.DisplayType.MultiKey) {
                    var cmb = new VIS.Controls.VComboBox(columnName, false, false, true, lookup, 50);
                    ctrl = cmb;
                }
                else {
                    displayType = VIS.DisplayType.Search;
                    var txtb = new VIS.Controls.VTextBoxButton(columnName, false, false, true, displayType, lookup);
                    txtb.getBtn();
                    ctrl = txtb;
                }
            }
            else if (displayType == VIS.DisplayType.Location) {
                var txtLoc = new VIS.Controls.VLocation(columnName, false, false, true, displayType, null);
                ctrl = txtLoc;
            }
            else if (displayType == VIS.DisplayType.Locator) {
                var txtLocator = new VIS.Controls.VLocator(columnName, false, false, true, displayType, lookup);
                ctrl = txtLocator;
            }
            else if (VIS.DisplayType.Text == displayType || VIS.DisplayType.TextLong == displayType) {
                var tl = new VIS.Controls.VTextArea(columnName, false, false, true, 50, 100, displayType);
                ctrl = tl;
            }
            else if (VIS.DisplayType.IsNumeric(displayType)) {
                if (VIS.DisplayType.Amount == displayType) {
                    var amt = new VIS.Controls.VAmountTextBox(columnName, false, false, true, 50, 100, displayType, header);
                    ctrl = amt;
                }
                else if (VIS.DisplayType.Integer == displayType) {
                    var int = new VIS.Controls.VNumTextBox(columnName, false, false, true, 50, 100, header);
                    ctrl = int;
                }
                else if (VIS.DisplayType.Number == displayType) {
                    var num = new VIS.Controls.VAmountTextBox(columnName, false, false, true, 50, 100, displayType, header);
                    ctrl = num;
                }
            }
            else if (displayType == VIS.DisplayType.PAttribute) {
                var txtP = new VIS.Controls.VPAttribute(columnName, false, false, true, displayType, lookup, windowNo, false, false, false, true);
                txtP.getBtn(0);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.GAttribute) {
                var txtP = new VIS.Controls.VPAttribute(columnName, false, false, true, displayType, lookup, windowNo, false, false, false, false);
                txtP.getBtn(0);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.Account) {
                var txtA = new VIS.Controls.VAccount(columnName, false, false, true, displayType, lookup, windowNo, header);
                txtA.getBtn(0);
                ctrl = txtA;
            }
            else if (displayType == VIS.DisplayType.Binary) {
                var bin = new VIS.Controls.VBinary(columnName, false, false, true, windowNo);
                ctrl = bin;
            }
            else if (displayType == VIS.DisplayType.Image) {
                var image = new VIS.Controls.VImage(columnName, false, false, windowNo);
                ctrl = image;
            }
            else if (displayType == VIS.DisplayType.URL) {
                var vs = new VIS.Controls.VURL(columnName, false, false, true, 50, 100);
                ctrl = vs;
            }
            else if (displayType == VIS.DisplayType.FileName || displayType == VIS.DisplayType.FilePath) {
                var vs = new VIS.Controls.VFile(columnName, false, false, true, windowNo, displayType);
                ctrl = vs;
            }
            return ctrl;
        };

        var displayData = function (requery, pNo) {

            if (multiSelection && w2ui[grdname]) {
                var selection = w2ui[grdname].getSelection();
                for (item in selection) {
                    if (multiValues.indexOf(w2ui[grdname].get(selection[item])[keyCol]) == -1) {
                        multiValues.push(w2ui[grdname].get(selection[item])[keyCol]);
                    }
                }
            }
            disposeDataSec();
            var sql = "SELECT ";
            var colName = null;
            var cName = null;
            var displayType = 0;
            var count = $.makeArray(schema).length;
            //get Qry from InfoColumns
            for (var item in schema) {
                colName = schema[item].SelectClause;

                if (String(colName).indexOf('.') > -1) {
                    cName = (String(colName).substring(String(colName).lastIndexOf('.') + 1, String(colName).length));
                }
                else {
                    cName = colName;
                }

                if (schema[item].IsKey) {
                    keyCol = cName.toUpperCase();
                }
                displayType = schema[item].AD_Reference_ID;
                if (displayType == VIS.DisplayType.YesNo) {
                    sql += " ( CASE " + colName + " WHEN 'Y' THEN  'True' ELSE 'False'  END ) AS " + (cName);
                }
                else if (displayType == VIS.DisplayType.List) {
                    //    ValueNamePair[] values = MRefList.GetList(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_REFERENCE_VALUE_ID"]), false);
                    //    sql.Append(" CASE ");
                    //    for (int j = 0; j < values.Length; j++)
                    //    {
                    //        sql.Append(" WHEN " + colName + "='" + values[j].Key.ToString() + "' THEN '" + values[j].Name.ToString() + "'");
                    //    }
                    //    sql.Append(" END AS " + cName);
                    var refList = schema[item].RefList;
                    sql += (" CASE ");
                    for (var refListItem in refList) {
                        sql += " WHEN " + colName + "='" + refList[refListItem].Key + "' THEN '" + refList[refListItem].Value + "'";
                    }
                    sql += " END AS " + cName;
                    //sql += colName + " ";
                }
                else {
                    sql += colName + " ";
                }
                if (!((count - 1) == item)) {
                    sql += ', ';
                }

            }
            // sql=sql.substring(0,sql.length-2)
            sql += " FROM " + info.FromClause;

            if (requery == true) {
                //Get Where Clause From SearchControls
                //  debugger;
                var whereClause = " ";
                var srchValue = null;
                var appendAND = false;
                for (var i = 0; i < srchCtrls.length; i++) {
                    srchValue = srchCtrls[i].Ctrl.getValue();

                    //JID_0905:  In Case of Date Range, if From Date is not selected then check if To Date is selected
                    if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.Date && srchCtrls[i].IsRange) {
                        if (srchValue == null) {
                            srchValue = srchCtrls[i].CtrlTo.getValue();
                        }
                    }

                    // Consider checkbox value only in case of true value
                    if (srchValue == null || srchValue.length == 0 || (srchValue == 0 && srchCtrls[i].AD_Reference_ID != VIS.DisplayType.YesNo) || srchValue == -1 || !srchValue) {
                        continue;
                    }

                    {
                        if (appendAND == true) {
                            whereClause += " AND ";
                        }
                        if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.String
                            || srchCtrls[i].AD_Reference_ID == VIS.DisplayType.Text
                            || srchCtrls[i].AD_Reference_ID == VIS.DisplayType.TextLong) {


                            if (!(String(srchValue).indexOf("%") == 0)) {
                                srchValue = "●" + srchValue;
                            }
                            else {
                                srchValue = String(srchValue).replace("%", "●");
                            }
                            if (!((String(srchValue).lastIndexOf("●")) == (String(srchValue).length))) {
                                srchValue = srchValue + "●";
                            }
                            srchValue = VIS.DB.to_string(srchValue);
                            whereClause += "  UPPER(" + srchCtrls[i].SearchColumnName + ") LIKE " + srchValue.toUpperCase();
                        }
                        else if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.Date) {
                            var fromValue = null;
                            var toValue = null;
                            var date = new Date(srchCtrls[i].Ctrl.getValue());
                            fromValue = "TO_DATE( '" + (Number(date.getMonth()) + 1) + "-" + date.getDate() + "-" + date.getFullYear() + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);

                            if (srchCtrls[i].IsRange) {
                                // JID_0905: If To Date is empty, then select system date
                                if (srchCtrls[i].CtrlTo.getValue() == null) {
                                    date = new Date();
                                }
                                else {
                                    date = new Date(srchCtrls[i].CtrlTo.getValue());
                                }
                                toValue = "TO_DATE( '" + (Number(date.getMonth()) + 1) + "-" + date.getDate() + "-" + date.getFullYear() + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);
                                whereClause += " ( " + srchCtrls[i].SearchColumnName + " BETWEEN " + fromValue + " AND " + toValue + ")";
                            }
                            else {
                                whereClause += srchCtrls[i].SearchColumnName + " =" + fromValue;
                            }
                        }
                        else if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.DateTime) {
                            var fromValue = null;
                            var toValue = null;
                            var date = new Date(srchCtrls[i].Ctrl.getValue());
                            fromValue = "TO_DATE( '" + (Number(date.getMonth()) + 1) + "-" + date.getDate() + "-" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + "', 'MM-DD-YYYY HH24:MI:SS')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);

                            if (srchCtrls[i].IsRange) {
                                date = new Date(srchCtrls[i].CtrlTo.getValue());
                                toValue = "TO_DATE( '" + (Number(date.getMonth()) + 1) + "-" + date.getDate() + "-" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + "', 'MM-DD-YYYY HH24:MI:SS')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);
                                whereClause += " ( " + srchCtrls[i].SearchColumnName + " BETWEEN " + fromValue + " AND " + toValue + ")";
                            }
                            else {
                                whereClause += srchCtrls[i].SearchColumnName + " =" + fromValue;
                            }
                        }

                        else if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.YesNo) {


                            srchValue = srchCtrls[i].Ctrl.getValue() == true ? "Y" : "N";
                            whereClause += srchCtrls[i].SearchColumnName + " = '" + srchValue + "'";

                        }
                        else {
                            var fromValue = null;
                            var toValue = null;
                            fromValue = srchCtrls[i].Ctrl.getValue();
                            if (srchCtrls[i].IsRange) {
                                if (srchCtrls[i].Ctrl.getValue()) {     // Consider checkbox value only in case of true value
                                    srchValue = srchCtrls[i].Ctrl.getValue() == true ? "Y" : "";
                                    whereClause += srchCtrls[i].SearchColumnName + " = '" + srchValue + "'";
                                }
                            }
                            else {
                                whereClause += " " + srchCtrls[i].SearchColumnName + " ='" + fromValue + "'";
                            }


                        }




                        appendAND = true;



                    }

                }

                if (whereClause.length > 1) {
                    if (info.FromClause.toUpperCase().indexOf("WHERE") > -1) {
                        sql += " AND " + whereClause;
                    }
                    else {
                        sql += " WHERE " + whereClause;
                    }
                    if (validationCode != null && validationCode.length > 0) {
                        sql += " AND " + validationCode;
                    }
                }
                else if (validationCode != null && validationCode.length > 0) {
                    if (info.FromClause.toUpperCase().indexOf("WHERE") > -1) {
                        sql += " AND " + validationCode;
                    }
                    else {
                        sql += " WHERE " + validationCode;
                    }
                }

                if (info.OTHERCLAUSE != null) {
                    //if (String(info.OTHERCLAUSE).toUpperCase().indexOf("WHERE") == 0 && (whereClause.length > 1 || (validationCode != null && validationCode.length > 1))) {
                    //    info.OTHERCLAUSE = String(info.OTHERCLAUSE).replace("WHERE", "AND");
                    //}
                    //else if (sql.toUpperCase().indexOf("WHERE") == -1 && info.OTHERCLAUSE.toUpperCase().indexOf("AND") > -1) {
                    //    //info.OTHERCLAUSE = String(info.OTHERCLAUSE).replace("AND", "WHERE");
                    //}
                    //sql += " " + info.OTHERCLAUSE;

                    if (String(info.OTHERCLAUSE).toUpperCase().indexOf("WHERE") > -1) {
                        if (info.FromClause.toUpperCase().indexOf("WHERE") > -1
                            || whereClause.length > 1
                            || (validationCode != null && validationCode.length > 1)) {
                            info.OTHERCLAUSE = String(info.OTHERCLAUSE).replace("WHERE", "AND");
                        }

                    }
                    sql += " " + info.OTHERCLAUSE;

                }
            }
            else {
                if (info.FromClause.toUpperCase().indexOf("WHERE") > -1) {
                    sql += " AND rownum=-1";
                }
                else {
                    sql += " WHERE rownum=-1";
                }
            }


            if (!pNo) {
                pNo = 1;
            }
            $.ajax({
                url: VIS.Application.contextUrl + "InfoWindow/GetData",
                dataType: "json",
                type: "POST",
                data: {
                    sql: sql,
                    tableName: info.TableName,
                    pageNo: pNo
                },
                error: function () {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    bsyDiv[0].style.visibility = "hidden";
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
                    loadData(res.data);
                }
            });


        };

        var loadData = function (dynData) {

            if (dynData == null) {
                alert(VIS.Msg.getMsg('NoDataFound'));
                bsyDiv[0].style.visibility = "hidden";
                return;
            }
            var grdCols = [];
            var grdRows = [];
            var rander = null;
            var displayType = null;

            for (var item in dynData) {

                var oColumn = {

                    resizable: true
                }
                displayType = schema[item].AD_Reference_ID;

                oColumn.caption = schema[item].Name;
                oColumn.field = dynData[item].ColumnName.toUpperCase();
                oColumn.columnName = dynData[item].ColumnName.toUpperCase();
                oColumn.sortable = true;
                oColumn.hidden = !schema[item].IsDisplayed;
                oColumn.size = '100px';

                if (VIS.DisplayType.IsNumeric(displayType)) {

                    if (displayType == VIS.DisplayType.Integer) {
                        oColumn.render = 'int';
                    }
                    //else if (displayType == VIS.DisplayType.Amount) {
                    //    oColumn.render = 'number:2';
                    //}
                    // JID_1809  Amount showing as per the browser culture
                    else {          //if (displayType == VIS.DisplayType.Amount) {
                        oColumn.render = function (record, index, colIndex) {
                            var val = VIS.Utility.Util.getValueOfDecimal(record[grdCols[colIndex].field]);
                            return (val).toLocaleString();
                        };
                    }
                    //else {
                    //    oColumn.render = 'number:1';
                    //}
                }
                    //	YesNo
                    //else if (displayType == VIS.DisplayType.YesNo) {

                    //    oColumn.render = function (record, index, colIndex) {

                    //        var chk = (record[grdCols[colIndex].field]) == 'True' ? "checked" : "";

                    //        return '<input type="checkbox" ' + chk + ' disabled="disabled" >';
                    //    }
                    //}

                //Date /////////
             // JID_1809 Date is showing as per browser culture
                else if (VIS.DisplayType.IsDate(displayType)) {
                    if (displayType == VIS.DisplayType.Date) {
                        oColumn.render = function (record, index, colIndex) {
                            var d = record[grdCols[colIndex].field];
                            if (d) {
                                var d = new Date(d);
                                d = d.toLocaleDateString();
                            }
                            else d = "";
                            return d;
                        }
                    }
                    else if (displayType == VIS.DisplayType.DateTime) {
                        oColumn.render = function (record, index, colIndex) {
                            var d = record[grdCols[colIndex].field];
                            if (d) {
                                var d = new Date(d);
                                d = d.toDateString();
                            }
                            else d = "";
                            return d;
                        }
                    }
                    else {
                        oColumn.render = function (record, index, colIndex) {
                            var d = record[grdCols[colIndex].field];
                            if (d) {
                                var d = new Date(d);
                                d = d.toLocaleTimeString();
                            }
                            else d = "";
                            return d;
                        }
                    }                    
                }

                else if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.Account) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.PAttribute) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.Button) {
                    oColumn.render = function (record) {
                        return '<div>button</div>';
                    }
                }

                else if (displayType == VIS.DisplayType.Image) {

                    oColumn.render = function (record) {
                        return '<div>Image</div>';
                    }
                }

                else if (VIS.DisplayType.IsLOB(displayType)) {

                    oColumn.render = function (record) {
                        return '<div>[Lob-blob]</div>';
                    }
                }



                grdCols[item] = oColumn;

                //grdCols[item] = { field: dynData[item].ColumnName, caption: schema[item].Name, hidden: !schema[item].IsDisplayed, sortable: true, size: '100px' };

            }


            for (var j = 0; j < dynData[0].RowCount; j++) {
                var row = {};
                for (var item in dynData) {
                    //grdCol[item] = { field: dynData[item].ColumnName, sortable: true, attr: 'align=center' };
                    row[dynData[item].ColumnName.toUpperCase()] = dynData[item].Values[j];
                }
                row['recid'] = j + 1;
                grdRows[j] = row;
            }
            grdname = 'gridInfodata' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(grdRows);
            dGrid = $(datasec).w2grid({
                name: grdname,
                recordHeight: 40,
                show: {

                    // do not show toolbar on Info Window.
                    toolbar: false,  // indicates if toolbar is v isible
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: false,   // indicates if toolbar delete button is visible
                    toolbarSave: false,   // indicates if toolbar save button is visible
                    selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    recordTitles: false	 // indicates if to define titles for records

                },
                columns: grdCols,
                records: grdRows,
                multiSelect: multiSelection,
                onUnselect: onUnSelectRow
                //resize: function ()
                //{
                //    alert("hi");
                //}
            });

            if (multiSelection) {

                for (itm in multiValues) {
                    for (item in w2ui[grdname].records) {
                        if (w2ui[grdname].records[item][keyCol] == multiValues[itm]) {
                            w2ui[grdname].select(parseInt(item) + 1);
                        }
                    }
                }
            }



            // $("#w2ui-even").css("height", "40px");
            bsyDiv[0].style.visibility = "hidden";
        };

        function onUnSelectRow(e) {
            if (!multiSelection) {
                return;
            }

            if (e.index) {
                var unselectedVal = w2ui[e.target].records[e.index][keyCol];
                if (multiValues.indexOf(unselectedVal) > -1) {
                    multiValues.splice(multiValues.indexOf(unselectedVal), 1);
                }
            }
            else {
                for (var l = 0; l < w2ui[e.target].records.length; l++) {
                    var unselectedVal = w2ui[e.target].records[l][keyCol];
                    if (multiValues.indexOf(unselectedVal) > -1) {
                        multiValues.splice(multiValues.indexOf(unselectedVal), 1);
                    }
                }
            }
        };

        var disposeDataSec = function () {
            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;

        };

        var btnOKClick = function () {
            //for (item in selectedItems) {
            //    multiValues.push(selectedItems[item]);
            //}


            var selection = w2ui[grdname].getSelection();
            for (item in selection) {
                if (multiValues.indexOf(w2ui[grdname].get(selection[item])[keyCol]) == -1) {
                    multiValues.push(w2ui[grdname].get(selection[item])[keyCol]);
                }
            }


            //var selection = w2ui[grdname].getSelection();
            //for (item in selection) {
            //    multiValues.push(w2ui[grdname].get(selection[item])[keyCol]);
            //}

            if (self.onClose != null && multiValues.length > 0) {
                self.onClose();
            }
            disposeComponent();
            //if (inforoot != null) {
            //    inforoot.dialog("close");
            //    inforoot = null;
            //}

            //if (ctrl != null) {
            //    var value = w2ui['gridInfodata'].get(w2ui['gridInfodata'].getSelection())[keyCol];

            //    disposeComponent();
            //    inforoot.dialog("close");
            //    inforoot = null;

            //    if (value !== ctrl.oldValue) {
            //        if (value == -1 || value == "") {
            //            value = null;
            //        }
            //        var evt = { newValue: value, propertyName: ctrl.getName() };
            //        ctrl.fireValueChanged(evt);
            //        evt = null;
            //        ctrl.setValue(value);
            //    }
            //}
            //else {
            //    disposeComponent();
            //    inforoot.dialog("close");
            //    inforoot = null;
            //}
        };

        this.getSelectedValues = function () {
            return multiValues;
        };

        var disposeComponent = function () {


            btnOK.off("click");
            btnRequery.off("click");
            btnCancel.off("click");
            // btnExpander.off('click');
            inforoot.off('keyup');

            liFirstPage.off("click");
            liPrevPage.off("click");
            liNextPage.off("click");
            liLastPage.off("click");
            cmbPage.off("change");


            datasec = null;
            searchSec = null;
            //inforoot = null;
            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;
            subroot = null;
            isExpanded = null;
            sSContainer = ssHeader = btnExpander = searchTab = searchSec = datasec = btnsec = null;
            bsyDiv = divbtnRight = btnCancel = btnOK = btnRequery = null;
            liFirstPage = liPrevPage = liNextPage = liLastPage = cmbPage = null;

            dGrid = null;
            info = null;
            schema = null;
            considerChkBox = null;
            srchCtrls = null;
            keyCol = null;
            self = null;
            multiValues = null;

            if (inforoot != null) {
                inforoot.dialog('destroy');
                inforoot.remove();
            }
            inforoot = null;

            this.disposeComponent = null;
            this.displaySchema = null;
            this.getControl = null;
            this.displayData = null;
            this.loadData = null;
            this.disposeDataSec = null;
            this.btnOKClick = null;
            this.getSelectedValues = null;
            this.disposeComponent = null;
        };

        function createPageSettings() {
            ulPaging = $('<ul class="vis-statusbar-ul">');

            liFirstPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftleft" title="' + VIS.Msg.getMsg("FirstPage") + '" style="opacity: 0.6;"></i></div></li>');

            liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-pageup" title="' + VIS.Msg.getMsg("PageUp") + '" style="opacity: 0.6;"></i></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" title="' + VIS.Msg.getMsg("PageDown") + '" style="opacity: 0.6;"></i></div></li>');

            liLastPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" title="' + VIS.Msg.getMsg("LastPage") + '" style="opacity: 0.6;"></i></div></li>');


            ulPaging.append(liFirstPage).append(liPrevPage).append(liCurrPage).append(liNextPage).append(liLastPage);
            pageEvents();
        }

        function pageEvents() {
            liFirstPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    displayData(true, 1);
                }
            });
            liPrevPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    displayData(true, parseInt(cmbPage.val()) - 1);
                }
            });
            liNextPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    displayData(true, parseInt(cmbPage.val()) + 1);
                }
            });
            liLastPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    displayData(true, parseInt(cmbPage.find("Option:last").val()));
                }
            });
            cmbPage.on("change", function () {
                displayData(true, cmbPage.val());
            });

        }

        function resetPageCtrls(psetting) {

            cmbPage.empty();
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
    };

    // Added interface to get value from context for passed variablename.- added by mohit- 13 Feb 2019
    InfoWindow.prototype.getValueAsString = function (variableName) {
        return VIS.context.getWindowContext(this.windowNo, variableName, true);
    };


    VIS.InfoWindow = InfoWindow;
})(VIS, jQuery);