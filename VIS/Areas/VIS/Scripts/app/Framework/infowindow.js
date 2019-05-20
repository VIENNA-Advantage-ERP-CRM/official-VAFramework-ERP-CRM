/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate info dialog
 * Author         :     Lakhwinder
 * Date           :     2-Aug-2014
  ******************************************************/
; (function (VIS, $) {
    function InfoWindow(_AD_InfoWindow_ID, ctrlValue, windowNo, validationCode, multiSelection) {

        this.onClose = null;


        var inforoot = $("<div>");
        var subroot = $("<div>");
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
            subroot.css("width", "98%");
            subroot.css("height", "97%");
            subroot.css("position", "absolute");
            // subroot.css("margin-left", "-10px");
            subroot.css('overflow', 'auto');


            // sSContainer = $("<div style='display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;'>");
            // ssHeader = $('<div style="padding: 7px; background-color: #F1F1F1;margin-bottom: 2px;height:10.5%;">');

            // btnExpander = $('<button style="border: 0px;background-color: transparent; padding: 0px;">').append($('<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/arrow-left.png">'));

            // ssHeader.append(btnExpander);
            // sSContainer.append(ssHeader);

            //searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;overflow:auto;'>");
            if (VIS.Application.isRTL) {
                searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;display: inline-block;width:23%;height:87.8%;overflow:auto;margin-left:10px;padding-right: 10px;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);display: inline-block;height:87.8%;'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block;width:75%;height:87.8%;overflow:auto;'>");
                btnsec = $("<div style='display: inline-block;width:99%;height:auto;margin-top: 2px;'>");
            }
            else {
                searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block; float: left;width:75%;height:87.8%;margin-left:10px;'>");
                btnsec = $("<div style='display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;'>");
            }
            //if (true) {//IPAD
            //    ssHeader.css('display', 'none');
            //    searchTab.css('height', '100%');
            //    searchSec.css('height', '100%');
            //}
            //  searchTab.append(searchSec);
            // sSContainer.append(searchTab);


            //btnsec = $("<div style='display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;'>");
            //subroot.append(sSContainer);
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
            divbtnRight = $("<div style='float:right;'>");
            if (VIS.Application.isRTL) {
                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(Oktxt);
            }
            else {
                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);
            }

            var divbtnLeft = $("<div style='float:left;'>");
            btnRequery = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png'>"))//.append(refreshtxt);

            //PagingCtrls
            divPaging = $('<div>');
            createPageSettings();
            divPaging.append(ulPaging);

            if (VIS.Application.isRTL) {
                divbtnLeft.append(btnCancel);
                divbtnLeft.append(btnOK);
                divbtnRight.append(btnRequery);
            }
            else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
                divbtnLeft.append(btnRequery);
            }

            btnsec.append(divbtnRight);
            btnsec.append(divPaging);
            btnsec.append(divbtnLeft);

            //Busy Indicator
            bsyDiv = $("<div class='vis-apanel-busy'>");
            bsyDiv.css("width", "98%");
            bsyDiv.css("height", "97%");
            bsyDiv.css('text-align', 'center');
            bsyDiv.css("position", "absolute");

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
            //btnExpander.on('click', function () {
            //    //sSContainer.css("width", '4%');
            //    //datasec.css("width", '95%');
            //    //searchSec.hide();
            //    //$(document.getElementsByName('gridInfodata')).css("width", '95%');
            //    window.setTimeout(function () {
            //        if (isExpanded) {
            //            btnExpander.animate({ borderSpacing: 180 }, {
            //                step: function (now, fx) {
            //                    $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
            //                    $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
            //                    $(this).css('transform', 'rotate(' + now + 'deg)');
            //                },
            //                duration: 'slow'
            //            }, 'linear');

            //            sSContainer.animate({ width: '4%' }, "slow");
            //            searchSec.hide();
            //            //datasec.animate({ width: "94%" }, "slow");
            //            //datasec.css("width", '94.8%');
            //            //dGrid.animate({ width: "94%" }, "fast");               

            //            // $(window).trigger('resize');
            //            // $(document.getElementsByName('gridInfodata')).css('width','100%')
            //            datasec.animate({ width: "94%" }, "slow", null, function () {
            //                dGrid.resize();
            //                dGrid.refresh();
            //            });

            //        }
            //        else {
            //            btnExpander.animate({ borderSpacing: 0 }, {
            //                step: function (now, fx) {
            //                    $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
            //                    $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
            //                    $(this).css('transform', 'rotate(' + now + 'deg)');
            //                },
            //                duration: 'slow'
            //            }, 'linear');

            //            // datasec.css("width", '76%');
            //            searchSec.show();
            //            datasec.animate({ width: "75%" }, "slow");
            //            sSContainer.animate({ width: '23%' }, "slow");
            //        }
            //        isExpanded = !isExpanded;
            //    }, 2);

            //});  /* remove events */

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
                    tr = $("<tr>");
                    tableSArea.append(tr);

                    var td = $("<td class='vis-gc-vpanel-table-td1'>");
                    //	No Label for FieldOnly, CheckBox, Button
                    if (!(schema[item].AD_Reference_ID == VIS.DisplayType.YesNo
                      || schema[item].AD_Reference_ID == VIS.DisplayType.Button
                      || schema[item].AD_Reference_ID == VIS.DisplayType.Label)) {
                        //Set label
                        label = new VIS.Controls.VLabel(schema[item].Name, schema[item].ColumnName);
                        var lblctrl = label.getControl().css("margin-bottom", "0px").css("margin-top", "5px");
                        lblctrl.css("font-weight", "inherit");
                        td.append(lblctrl);

                    }
                    else {
                        appendTopMargin = true;
                    }

                    tr.append(td);

                    tr = $("<tr>");
                    tableSArea.append(tr);

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

                    // code commented by mohit
                    //if (schema[item].Condition != null && schema[item].SetValue != null) {
                    //    //condition = schema[item].Condition.substring(schema[item].Condition.lastIndexOf("="));
                    //    //condition = condition.substring(0 + 1, condition.indexOf("@"));
                    //    // subCondition = schema[item].Condition.substring(0 + 1, schema[item].Condition.lastIndexOf("@"));
                    //    //if (schema[item].Condition.contains("@")) {

                    //    conditionColumn = schema[item].Condition.substring(0 + 1, schema[item].Condition.lastIndexOf("@"));
                    //    conditionVal = schema[item].Condition.substring(schema[item].Condition.lastIndexOf("=") + 1, schema[item].Condition.length);
                    //    conditionVal = conditionVal.replace(/\'/g, "");
                    //    conditionVal = conditionVal.replace(/\"/g, "");
                    //    appVal = VIS.context.getWindowContext(windowNo, conditionColumn, false);
                    //    if (conditionVal == appVal) {
                    //        if (displayType == VIS.DisplayType.YesNo) {
                    //            if (schema[item].SetValue.toUpperCase() == "Y") {
                    //                ctrl.getControl().find("input").prop("checked", true);
                    //            }
                    //            else if (schema[item].SetValue.toUpperCase() == "N") {
                    //                ctrl.getControl().find("input").prop("checked", false);
                    //            }
                    //        }
                    //        else {
                    //            ctrl.setValue(schema[item].SetValue);
                    //        }
                    //    }
                    //    //}
                    //    //else {
                    //    //   conditionColumn = schema[item].SetValue.substring(0 + 1, schema[item].SetValue.lastIndexOf("@"));
                    //    //    appVal = VIS.context.getWindowContext(windowNo, conditionColumn, false);
                    //    //   ctrl.setValue(appVal);
                    //    // }

                    //    //condition = schema[item].Condition;
                    //    //if (VIS.context.isSOTrx(windowNo)) {
                    //    //    ctxValue = "@IsSOTrx@='Y'";
                    //    //}
                    //    //else {
                    //    //    ctxValue = "@IsSOTrx@='N'";
                    //    //}
                    //    ////key = windowNo + "|" + subCondition.trim();
                    //    ////value = VIS.context.m_map[windowNo][key];
                    //    ////ctxValue = "@" + subCondition.trim() + "@" + "=" + "'" + value + "'";
                    //    //if (condition.toUpperCase() == ctxValue.toUpperCase()) {
                    //    //    if (schema[item].SetValue.toUpperCase() == "Y") {
                    //    //        ctrl.getControl().find("input").prop("checked", true);
                    //    //    }
                    //    //    else {
                    //    //        ctrl.getControl().find("input").prop("checked", false);
                    //    //    }
                    //    //}

                    //}
                    // }
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
                    tr.append(tdctrl);
                    var count = ctrl.getBtnCount();
                    if (count == 2) {
                        var div = $("<div>");
                        if (appendTopMargin) {
                            div.css('margin-top', '5px');
                        }

                        if (VIS.Application.isRTL) {
                            div.css("width", "97%");


                            ctrlBtn = ctrl.getBtn(1);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px').css('float', 'left'));
                            }
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px').css('float', 'left'));
                            }
                            div.append(ctrl.getControl().css("width", "72%").css("float", "left").css('height', '30px'));

                            count = -1;
                            ctrl = null;
                        }
                        else {
                            div.css("width", "97%");
                            div.append(ctrl.getControl().css("width", "72%").css("float", "left").css('height', '30px'));
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px'));
                            }
                            ctrlBtn = ctrl.getBtn(1);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px'));
                            }
                            count = -1;
                            ctrl = null;
                        }
                        tdctrl.append(div);
                    }
                    else if (count == 1) {
                        var div = $("<div>");
                        if (appendTopMargin) {
                            div.css('margin-top', '5px');
                        }
                        if (VIS.Application.isRTL) {
                            div.css("width", "97%");
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%")
                                    .css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px').css('float', 'left'));
                            }
                            div.append(ctrl.getControl().css("width", "86%").css("float", "left").css('height', '30px'));
                            count = -1;
                            ctrl = null;
                        }
                        else {
                            div.css("width", "97%");
                            div.append(ctrl.getControl().css("width", "86%").css("float", "left").css('height', '30px'));
                            var ctrlBtn = ctrl.getBtn(0);
                            if (ctrlBtn != null) {
                                div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%")
                                    .css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px'));
                            }
                            count = -1;
                            ctrl = null;
                        }
                        tdctrl.append(div);
                    }
                    else {
                        if (appendTopMargin) {
                            ctrl.getControl().css('margin-top', '5px');
                        }
                        if (schema[item].AD_Reference_ID == VIS.DisplayType.YesNo) {
                            ctrl.getControl().css('margin-top', '12px').css('height', '17px');
                        }
                        tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit").css('height', '30px'));
                    }

                    if (schema[item].IsRange) {
                        srchCtrl.IsRange = true;
                        tr = $("<tr>");
                        tableSArea.append(tr);

                        tr.append((new VIS.Controls.VLabel(VIS.Msg.getMsg("To"), schema[item].ColumnName)).getControl().css("margin-bottom", "0px").css("margin-top", "5px").css("font-weight", "inherit"));
                        tr = $("<tr>");
                        tableSArea.append(tr);

                        ctrl = getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name);
                        srchCtrl.CtrlTo = ctrl;
                        var tdctrlTo = $("<td>");
                        var count = ctrl.getBtnCount();
                        if (count == 2) {
                            var div = $("<div>");
                            div.css("width", "97%");
                            if (appendTopMargin) {
                                ctrl.getControl().css('margin-top', '5px');
                            }
                            if (VIS.Application.isRTL) {
                                div.css("width", "97%");


                                ctrlBtn = ctrl.getBtn(1);
                                if (ctrlBtn != null) {
                                    div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px').css('float', 'left'));
                                }
                                var ctrlBtn = ctrl.getBtn(0);
                                if (ctrlBtn != null) {
                                    div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px').css('float', 'left'));
                                }
                                div.append(ctrl.getControl().css("width", "72%").css("float", "left").css('height', '30px'));

                                count = -1;
                                ctrl = null;
                            }
                            else {
                                div.css("width", "97%");
                                div.append(ctrl.getControl().css("width", "72%").css("float", "left").css('height', '30px'));
                                var ctrlBtn = ctrl.getBtn(0);
                                if (ctrlBtn != null) {
                                    div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px'));
                                }
                                ctrlBtn = ctrl.getBtn(1);
                                if (ctrlBtn != null) {
                                    div.append(ctrlBtn.css("class", "vis-controls-txtbtn-table-td2").css("width", "14%").css('padding', '0px').css('border-color', '#BBBBBB').css('height', '30px'));
                                }
                                count = -1;
                                ctrl = null;
                            }
                            tdctrlTo.append(div);
                        }
                        else {
                            if (appendTopMargin) {
                                ctrl.getControl().css('margin-top', '5px');
                            }
                            if (schema[item].AD_Reference_ID == VIS.DisplayType.YesNo) {
                                ctrl.getControl().css('margin-top', '12px').css('height', '17px');
                            }
                            else {
                                ctrl.getControl().css('height', '30px');
                            }
                            tdctrlTo.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                        }


                        tr.append(tdctrlTo);
                    }
                    else {
                        srchCtrl.IsRange = false;
                    }

                    rowctrl = rowctrl + 1;
                    srchCtrls.push(srchCtrl);

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
                modal: true

                //,
                //buttons: [
                //     {
                //         text: refreshtxt,
                //         click: function () {

                //             bsyDiv[0].style.visibility = 'visible';
                //             displayData(true);

                //         },
                //         style: "float:left;margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     },
                //     {
                //         text: Oktxt,
                //         click: function () {
                //             btnOKClick();
                //         },
                //         style: "margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     },
                //     {
                //         text: canceltxt,
                //         click: function () {
                //             disposeComponent();
                //             inforoot.dialog("close");
                //             inforoot = null;
                //         },
                //         style: "margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     }
                //]
                ,
                closeText: VIS.Msg.getMsg("close"),
                close: onClosing
            });

            displayData(false, cmbPage.val());
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
                    if (srchValue == null || srchValue.length == 0 || (srchValue == 0 && srchCtrls[i].AD_Reference_ID != VIS.DisplayType.YesNo) || srchValue == -1) {
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
                            whereClause += "  UPPER(" + srchCtrls[i].SearchColumnName + ") LIKE '" + srchValue.toUpperCase() + "' ";
                        }
                        else if (srchCtrls[i].AD_Reference_ID == VIS.DisplayType.Date) {
                            var fromValue = null;
                            var toValue = null;
                            var date = new Date(srchCtrls[i].Ctrl.getValue());
                            fromValue = "TO_DATE( '" + (Number(date.getMonth()) + 1) + "-" + date.getDate() + "-" + date.getFullYear() + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);

                            if (srchCtrls[i].IsRange) {
                                date = new Date(srchCtrls[i].CtrlTo.getValue());
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
                                toValue = srchCtrls[i].CtrlTo.getValue();
                                whereClause += " ( " + srchCtrls[i].SearchColumnName + " BETWEEN '" + fromValue + "' AND '" + toValue + "')";
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
                    else if (displayType == VIS.DisplayType.Amount) {
                        oColumn.render = 'number:2';
                    }
                    else {
                        oColumn.render = 'number:1';
                    }
                }
                    //	YesNo
                    //else if (displayType == VIS.DisplayType.YesNo) {

                    //    oColumn.render = function (record, index, colIndex) {

                    //        var chk = (record[grdCols[colIndex].field]) == 'True' ? "checked" : "";

                    //        return '<input type="checkbox" ' + chk + ' disabled="disabled" >';
                    //    }
                    //}

                    //Date /////////
                else if (VIS.DisplayType.IsDate(displayType)) {
                    oColumn.render = function (record, index, colIndex) {

                        var d = record[grdCols[colIndex].field];
                        if (d) {
                            d = Globalize.format(new Date(d), 'd');
                        }
                        else d = "";
                        return d;

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

                    toolbar: true,  // indicates if toolbar is v isible
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
            ulPaging = $('<ul style="float:right;margin-top:10px" class="vis-statusbar-ul">');

            liFirstPage = $('<li style="opacity: 1;"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/PageFirst16.png" alt="First Page" title="' + VIS.Msg.getMsg("FirstPage") + '" style="opacity: 0.6;"></div></li>');

            liPrevPage = $('<li style="opacity: 1;"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/PageUp16.png" alt="Page Up" title="' + VIS.Msg.getMsg("PageUp") + '" style="opacity: 0.6;"></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/PageDown16.png" alt="Page Down" "' + VIS.Msg.getMsg("PageDown") + '" style="opacity: 0.6;"></div></li>');

            liLastPage = $('<li style="opacity: 1;"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/PageLast16.png" alt="Last Page" title="' + VIS.Msg.getMsg("LastPage") + '" style="opacity: 0.6;"></div></li>');


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