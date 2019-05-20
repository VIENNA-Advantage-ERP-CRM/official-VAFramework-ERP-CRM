; (function (VIS, $) {

    function cvd(aPanel) {
        var gc = aPanel.curGC;
        var mTab = gc.getMTab();
        var cardView = gc.vCardView;
        //var gridWindow = aPanel.gridWindow;
        var AD_Window_ID = mTab.getAD_Window_ID();
        var AD_Tab_ID = mTab.getAD_Tab_ID();
        var tabName = mTab.getName();
        var WindowName = aPanel.curGC.aPanel.$parentWindow.getName();
        var AD_CardView_ID = cardView.getAD_CardView_ID();
        var AD_GroupField_ID = cardView.getField_Group_ID();
        var cardViewInfo = 0;
        var roleInfo = 0;
        var btnRight = null;
        var btnLeft = null;
        var btnUp = null;
        var btnDown = null;
        var ulWindowField = null;
        var ulCardViewColumnField = null;
        var cardViewColumns = [];
        var tabField = mTab.getFields();
        var totalTabFileds = [];
        var lastSelectWindowFieldItem = null;
        var lastSelectCardViewColumnFieldItem = null;
        var windowFieldindex = 0;
        var cardViewColumnFieldindex = -1;
        var WindowAD_Field_ID = 0;
        var cardViewColumnAD_Field_ID = 0;
        var cardViewColumnAD_Field_ID = 0;
        var AD_CardViewColumn_ID = 0;
        var columnFieldArray = [];
        var cardViewColArray = [];
        var cardViewArray = [];
        var seqNo = 0;
        var FieldName = "";
        var count = 0;
        var AD_User_ID = 0;
        var isNewRecord = false;
        var CardViewName = "";




        var fields = null;
        var dbResult = null;
        var rootGroupFieldsCombo = null;
        var divLeftGroupFieldsCombo = null;
        var divRightGroupFieldsCombo = null;
        var centerBtn = null;
        var centerBtnUpAndDown = null;
        var divChildLeftLable = null;
        var divChildRightLable = null;
        var divChildLeftColumns = null;
        var divChildRightColumns = null;
        var ulLeftColumns = null;
        var ulRightColumns = null;
        var rootCardViewUI = null;
        var cmbCardView = null;
        var cmbUser = null;
        var txtWindowName = null;
        var txtTabName = null;
        var cmbGroupField = null;
        var cardViewUserID = 0;
        var txtCardViewName = null;
        var FieldName = "";
        var orginalAD_CardView_ID = 0;
        var orginalcardViewUserID = 0;
        var orginalIncludedCols = [];
        var orginalAD_GroupField_ID = 0;
        var loginUserName = VIS.context.getAD_User_Name();
        var lableCardViewName = null;
        var ddlCardView = null;
        var btnNew = null;
        var btnCancle = null;
        var ulRole = null;
        var LstRoleID = [];
        var LstCardViewRole = null;
        var cardViewName = "";
        var isEdit = false;
        var defaultMsg = VIS.Msg.getMsg("IsDefault");
        var cmbColumn = null;
        var control1 = null;
        var control2 = null;
        var divValue1 = null;
        var drpOp = null;
        var findFields = mTab.getFields().slice();
        var self = this;
        var cvTable = null;
        var cardviewCondition = [];
        var cvConditionArray = null;
        var ctrColor = null;
        var strConditionArray = [];
        var LstCardViewCondition = null;
        var isFirstLoad = false;
        var LastCVCondition = [];
        var isBusyRoot = null;
        // var 
        var root, ch;
        function init() {
            root = $('<div style="width:100%;height:100%"></div>');
            ArrayTotalTabFields();
            CardViewUI();
            FillCardViewCombo();
            // FillTextControl();
            // FillGroupFields();
            //FillColumnInclude(false);
            isFirstLoad = true;
            cmbVardChange();
            FillCVConditionCmbColumn(cmbColumn);

        };
        var ArrayTotalTabFields = function () {
            for (var i = 0; i < mTab.getFields().length; i++) {
                totalTabFileds.push(mTab.getFields()[i]);
            }

            for (var i = 0; i < cardView.fields.length; i++) {
                orginalIncludedCols.push(cardView.fields[i].getAD_Field_ID());
            }
        };

        var CardViewUI = function () {

            // Card View Dropdown
            isBusyRoot = $("<div class='vis-apanel-busy vis-cardviewmainbusy'></div> ");
            rootCardViewUI = $("<div class='vis-cardviewmain'></div>");
            var divCardViewMainFirstChild = $("<div class='vis-cardviewmainfirstchild vis-pull-left'></div>");
            var divCardViewMainSecondChild = $("<div class='vis-cardviewmainsecondchild'></div>");
            var divCardViewCondition = $("<div class='vis-cardviewCondition'></div>").append("<div class='vis-cardviewConditionControls'> </div>  <div class='vis-cardviewConditionGrid'> </div> ");
            var divCardViewbtn = $("<div class='vis-cardviewbtn'><button class='vis-btnDelete'> </button> <div class='vis-cdv-customokcancle'><button class='vis-btnOk'>  " + VIS.Msg.getMsg("Ok") + "  </button><button class='vis-btnCardViewCancle'>  " + VIS.Msg.getMsg("Cancel") + "  </button></div> </div>");
            rootCardViewUI.append(divCardViewMainFirstChild);
            divCardViewMainFirstChild.append("<div class='vis-firstdiv vis-pull-left' ></div>");
            divCardViewMainFirstChild.append("<div class='vis-seconddiv vis-pull-right'></div>");
            divCardViewMainFirstChild.append("<div class='vis-thirddiv vis-pull-left' ></div>");
            rootCardViewUI.find(".vis-firstdiv").append("<div class='vis-first-divHeader'><label class='vis-ddlcardview'>" + VIS.Msg.getMsg("SelectCardView") + " </label><label style='display:none' class='vis-lbcardviewname'>" + VIS.Msg.getMsg("CardViewName") + " </label></div>  ")
            var divCardView = $("<div class='vis-CardView vis-pull-left'> <div  class='vis-cardviewchild vis-pull-left'></div>  </div>");
            var divCardViewName = $("<input style='width:100%;display:none' class='vis-txtcardviewname' type='text'>");
            var divUser = $("<div class='vis-User vis-pull-left'></div>");
            var btnNewAndCancle = $("<div class='vis-pull-left'><button  class='vis-btnnew vis-cvd-btn'>  </button><button style='display:none' class='vis-btncancle vis-cvd-canclebtn'>  </button> <button class='vis-btnedit vis-cvd-editbtn'>  </button></div>");
            divCardView.find(".vis-cardviewchild").append("<select class='vis-cmbcardview'> </select>");


            divUser.append("<Select type='text' class='vis-cmbuser'> </Select>");

            if (VIS.MRole.isAdministrator) {

                rootCardViewUI.append(divCardViewMainSecondChild);
                rootCardViewUI.find(".vis-firstdiv").append(" <div style='display:none' class='vis-first-divHeader vis-pull-right'><label >" + VIS.Msg.getMsg("SelectUser") + "</label></div>");
                rootCardViewUI.find(".vis-CardView").append("");
                rootCardViewUI.find(".vis-firstdiv").append(divCardView);

                rootCardViewUI.find(".vis-cardviewchild").append(divCardViewName);
                rootCardViewUI.find(".vis-cardviewchild").css({ "width": "67%" });
                rootCardViewUI.find(".vis-CardView").append(btnNewAndCancle);
                cmbUser = rootCardViewUI.find(".vis-cmbuser");
                AddCVConditionControl(divCardViewCondition.find(".vis-cardviewConditionControls"));
                rootCardViewUI.append(divCardViewCondition);
                rootCardViewUI.append(divCardViewbtn);
                rootCardViewUI.find(".vis-btnDelete").css("display", "block");

            }
            else {

                divCardViewCondition.css('width', '100%');
                rootCardViewUI.find(".vis-firstdiv").append(divCardView);
                rootCardViewUI.find(".vis-cardviewchild").append(divCardViewName);
                rootCardViewUI.find(".vis-cardviewchild").css({ "width": "92%" });
                divCardViewMainFirstChild.css({ "width": "100%", "float": "left" });
                rootCardViewUI.find(".k ").css("display", "none");
                AddCVConditionControl(divCardViewCondition.find(".vis-cardviewConditionControls"));
                rootCardViewUI.append(divCardViewCondition);
                rootCardViewUI.append(divCardViewbtn);
                // divCardViewbtn.css({ "float": "right" });
                rootCardViewUI.find(".vis-btnDelete").css("display", "none");
            }
            CreateCVGrid(rootCardViewUI.find(".vis-cardviewConditionGrid"));
            divCardViewMainSecondChild.append("<label class='vis-pull-left'> " + VIS.Msg.getMsg("SelectRole") + "</label>");

            divCardViewMainSecondChild.append("<div style='border: 1px solid #ccc;height: 171px;margin-top: 25px;overflow:auto'><ul class='vis-ulrole'> </ul></div>");
            ulRole = rootCardViewUI.find(".vis-ulrole");
            // controls 
            ddlCardView = rootCardViewUI.find(".vis-ddlcardview");
            cmbCardView = rootCardViewUI.find(".vis-cmbcardview");
            lableCardViewName = rootCardViewUI.find(".vis-lbcardviewname");
            txtCardViewName = rootCardViewUI.find(".vis-txtcardviewname");
            btnNew = rootCardViewUI.find(".vis-btnnew");
            btnCancle = rootCardViewUI.find(".vis-btncancle");
            btnEdit = rootCardViewUI.find(".vis-btnedit");
            // Textbox Control
            var rootTextControl = $("<div class='vis-divtextcontrol' ></div>");

            rootTextControl.append("<div style='display:none' class='vis-second-divHeader vis-pull-left' ><div style='width:100%'> <label>" + VIS.Msg.getMsg("Window") + "</label></div><div style='width:100%'><input type='text'  class='vis-txtwindowname' style='width:100%' > </input></div></div>");
            rootTextControl.append("<div style='display:none' class='vis-second-divHeader vis-pull-left' ><div style='width:100%'><label>" + VIS.Msg.getMsg("Tab") + "</label></div><div style='width:100%'><input type='text'  class='vis-txttabname' style='width:100%' > </input></div></div>");
            rootTextControl.append("<div class='vis-second-divHeader vis-pull-left' ><div style='width:100%'><label>" + VIS.Msg.getMsg("GroupByField") + "</label></div><div style='width:92%'><select   class='vis-cmbgroupfield' style='width:100%;height:31px'> </select></div></div>");
            rootCardViewUI.find(".vis-seconddiv").append(rootTextControl);
            // TextBox Control
            txtWindowName = rootTextControl.find(".vis-txtwindowname");
            txtTabName = rootTextControl.find(".vis-txttabname");
            cmbGroupField = rootTextControl.find(".vis-cmbgroupfield");
            // Column Fields
            if (VIS.MRole.isAdministrator) {
                rootGroupFieldsCombo = $("<div class='vis-third-header vis-pull-left'></div>");
            }
            else {
                rootGroupFieldsCombo = $("<div style='width:101%' class='vis-third-header vis-pull-left'></div>");
            }
            divLeftGroupFieldsCombo = $("<div class='vis-left-fields vis-pull-left'></div>");
            divRightGroupFieldsCombo = $("<div class='vis-left-fields vis-pull-left' ></div>");
            centerBtn = $("<div class='vis-cardviewbutton vis-pull-left' ><button class='vis-btnup'  style='margin-top:10px'><img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/up-cardview.png'></button><button  class='vis-btnright'  style='margin-top:10px'><img  src='" + VIS.Application.contextUrl + "Areas/VIS/Images/" + (VIS.Application.isRTL ? "left" : "right") + "-shift.png' ></button></div>");
            centerBtn.append("<button   class='vis-btnleft' style='margin-top:10px'><img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/" + (VIS.Application.isRTL ? "right" : "left") + "-shift.png' ></button><button  class='vis-btndown' style='margin-top:10px'><img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/down arrow.png' ></button>");
            divChildLeftLable = $("<div style='width:100%'><label>" + VIS.Msg.getMsg("SelectField") + "</label></div>");
            divChildRightLable = $("<div style='width:100%'><label>" + VIS.Msg.getMsg("IncludedField") + "</label></div>");
            divChildLeftColumns = $("<div style='width:99%;border: 1px solid #ccc;height:89%;overflow:auto'></div>");
            divChildRightColumns = $("<div style='width:100%;border: 1px solid #ccc;height:89%;overflow:auto'></div>");
            ulLeftColumns = $("<ul  class='vis-windowfield'></ul>");
            ulRightColumns = $("<ul  class='vis-cardviewcolumnfield'></ul>");
            divChildLeftColumns.append(ulLeftColumns);
            divChildRightColumns.append(ulRightColumns);
            divLeftGroupFieldsCombo.append(divChildLeftLable);
            divLeftGroupFieldsCombo.append(divChildLeftColumns);
            divRightGroupFieldsCombo.append(divChildRightLable);
            divRightGroupFieldsCombo.append(divChildRightColumns);
            rootGroupFieldsCombo.append(divLeftGroupFieldsCombo);
            rootGroupFieldsCombo.append(centerBtn);
            rootGroupFieldsCombo.append(divRightGroupFieldsCombo);
            rootCardViewUI.find(".vis-thirddiv").append(rootGroupFieldsCombo);
            root.append(rootCardViewUI);
            rootCardViewUI.before(isBusyRoot);

            // UL Controls
            ulWindowField = rootCardViewUI.find(".vis-windowfield");
            ulCardViewColumnField = rootCardViewUI.find(".vis-cardviewcolumnfield");

            //Controls
            btnRight = rootCardViewUI.find(".vis-btnright");
            btnLeft = rootCardViewUI.find(".vis-btnleft");
            btnUp = rootCardViewUI.find(".vis-btnup");
            btnDown = rootCardViewUI.find(".vis-btndown");
            btnOk = rootCardViewUI.find(".vis-btnOk");
            btnCardViewCancle = rootCardViewUI.find(".vis-btnCardViewCancle");
            btnDelete = rootCardViewUI.find(".vis-btnDelete");
            cmbColumn = rootCardViewUI.find(".vis-cmbcolumn");
            divValue1 = rootCardViewUI.find(".vis-cvd-valcontainer");
            drpOp = rootCardViewUI.find(".vis-cmboperator");
            btnSave = rootCardViewUI.find(".vis-btnsave");
            cvTable = rootCardViewUI.find(".vis-cv-rowtable");
            ctrColor = rootCardViewUI.find(".vis-cmbcolor");



            isBusyRoot.css({
                "display": "none"
            });
            Events();

        };

        function IsBusy(isBusy) {
            if (isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "block" });
            }
            if (!isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "none" });
            }
        };

        var AddCVConditionControl = function (root) {

            var pDiv = $('<div style="float:left;Width:calc( 100% - 41px)">');
            var divCVConditionCmbColor = $("<div class='vis-divcvc-cmbcolor'><div style='width:100%'><lable>" + VIS.Msg.getMsg("BGColor") + "</lable></div> <div style='width:100%'> <input class='vis-cmbcolor' type='color' /></div></div>");
            var divCVConditionCmbColumnColor = $("<div class='vis-divcvc-cmbcolumn'><div style='width:100%'><lable>" + VIS.Msg.getMsg("Column") + "</lable></div> <div><select class='vis-cmbcolumn'></select></div></div>");
            var divCVConditionCmbOperator = $("<div class='vis-divcvc-cmboperator'><div style='width:100%'><lable>" + VIS.Msg.getMsg("Operator") + "</lable></div>  <div><select class='vis-cmboperator'></select> </div></div>");
            var divCVConditionQueryValue = $("<div class='vis-divcvc-queryvalue'><div style='width:100%'><lable>" + VIS.Msg.getMsg("QueryValue") + "</lable></div> <div class='vis-cvd-valcontainer' style='width:100%'></div></div>");
            var divCVConditionBtnSave = $("<div class='vis-divcvc-btnsave'><button class='vis-btnsave'></button></div>");
            pDiv.append(divCVConditionCmbColor);
            pDiv.append(divCVConditionCmbColumnColor);
            pDiv.append(divCVConditionCmbOperator);
            pDiv.append(divCVConditionQueryValue);
            root.append(pDiv);
            root.append(divCVConditionBtnSave);
        };

        var FillCardViewCombo = function (isDelete) {
            cmbCardView.children().remove();
            var url = VIS.Application.contextUrl + "CardView/GetCardView";
            $.ajax({
                type: "GET",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: { ad_Window_ID: AD_Window_ID, ad_Tab_ID: AD_Tab_ID },
                success: function (data) {
                    dbResult = JSON.parse(data);
                    cardViewInfo = dbResult[0].lstCardViewData;
                    roleInfo = dbResult[0].lstRoleData;
                    LstCardViewRole = dbResult[0].lstCardViewRoleData;
                    LstCardViewCondition = dbResult[0].lstCardViewConditonData;
                    if (cardViewInfo != null && cardViewInfo.length > 0) {

                        for (var i = 0; i < cardViewInfo.length; i++) {
                            // AD_CardView_ID = cardViewInfo[0].CardViewID;
                            cmbCardView.append("<Option ad_user_id=" + cardViewInfo[i].UserID + " cardviewid=" + cardViewInfo[i].CardViewID + " ad_field_id=" + cardViewInfo[i].AD_GroupField_ID + "> " + w2utils.encodeTags(cardViewInfo[i].CardViewName) + "</Option>");
                        }
                    }
                    else {
                        btnNew.trigger("click");
                        btnCancle.prop("disabled", true);
                        btnEdit.trigger("click");
                        btnDelete.prop("disabled", true);
                    }
                    cmbCardView.find("[cardviewid='" + AD_CardView_ID + "']").attr("selected", "selected");

                    orginalAD_CardView_ID = AD_CardView_ID;

                    //cardViewUserID = cmbCardView.find(":selected").attr("ad_user_id");
                    try {
                        if (!isDelete) {
                            cardViewUserID = parseInt(cmbCardView.find(":selected").attr("ad_user_id"));
                            AD_User_ID = cardViewUserID;
                            orginalcardViewUserID = cardViewUserID;
                        }
                        else {
                            orginalAD_CardView_ID = cmbCardView.find(":selected").attr("cardviewid");
                            AD_GroupField_ID = cmbCardView.find(":selected").attr("ad_field_id");
                            cardViewUserID = parseInt(cmbCardView.find(":selected").attr("ad_user_id"));
                            AD_CardView_ID = orginalAD_CardView_ID;
                        }
                    }
                    catch (e) {
                    }

                    // FillCVConditonTable(LstCardViewCondition);
                    //  txtCardViewName.val(cmbCardView.find(":selected").data('name'));
                    if (VIS.MRole.isAdministrator) {
                        // FillUserList(cmbUser);
                        FillRoleList(ulRole);
                    }
                    if (AD_User_ID > 0) {
                        ulRole.attr('disabled', 'disabled');
                    }
                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                }
            });

            if (cmbCardView != null) {
                cmbCardView.on("change", function () {
                    isFirstLoad = false;
                    cmbVardChange();
                });
            }
        };


        var cmbVardChange = function () {

            var sel = cmbCardView.find(":selected");
            if (sel.length > 0) {
                AD_CardView_ID = parseInt(sel.attr("cardviewid"));
                cardViewUserID = parseInt(sel.attr("ad_user_id"));
            }

            //  txtCardViewName.val(sel.data('name'));
            changeHeader();
            FillTextControl();
            if (AD_CardView_ID > 0) {
                FillColumnInclude(true, false);

            } else {
                FillColumnInclude(false, false);
            }
            FillGroupFields();
            FillRoleList(ulRole);
        }

        var FillTextControl = function () {
            txtWindowName.val(WindowName);
            txtTabName.val(tabName);
            txtWindowName.prop("disabled", "disabled");
            txtTabName.prop("disabled", "disabled");
        };

        var FillGroupFields = function () {
            if (cmbGroupField != null)
            { cmbGroupField.children().remove(); }
            var fields = null;
            var dbResult = null;
            if (mTab != null && mTab.getFields().length > 0) {
                cmbGroupField.append("<Option FieldID=" + -1 + "></Option>");
                tabField = mTab.getFields();
                for (var i = 0; i < tabField.length; i++) {
                    var c = tabField[i].getColumnName().toLower();
                    if (c == "created" || c == "createdby" || c == "updated" || c == "updatedby") {
                        continue;
                    }
                    if ((VIS.DisplayType.IsLookup(tabField[i].getDisplayType()) && tabField[i].getLookup() && tabField[i].getLookup().getIsValidated()) || (tabField[i].getDisplayType() == VIS.DisplayType.YesNo)) {
                        cmbGroupField.append("<Option FieldID=" + tabField[i].getAD_Field_ID() + "> " + tabField[i].getHeader() + "</Option>");
                    }
                }
            }
            if (AD_GroupField_ID != null && AD_GroupField_ID > 0) {
                var result = jQuery.grep(tabField, function (value) {
                    return value.getAD_Field_ID() == AD_GroupField_ID;
                });
                cmbGroupField.find("[FieldID='" + AD_GroupField_ID + "']").attr("selected", "selected");
                orginalAD_GroupField_ID = cmbGroupField.find(":selected").attr("fieldid");
            }

            if (cmbGroupField != null) {
                cmbGroupField.on("change", function () {
                    AD_GroupField_ID = parseInt($(this).find(":selected").attr("fieldid"));
                });
            }
        };


        var FillColumnInclude = function (isReload, isShowAllColumn) {
            //if (!isReload) {
            if (ulLeftColumns != null) {
                ulLeftColumns.children().remove();
            }
            fields = null; FillColumnInclude
            dbResult = null;

            tabField = mTab.getFields();
            FillCardViewColumns(ulRightColumns, isReload);
            if (mTab != null && mTab.getFields().length > 0) {
                for (var i = 0; i < tabField.length; i++) {
                    var c = tabField[i].getColumnName().toLower();
                    if (c == "created" || c == "createdby" || c == "updated" || c == "updatedby") {
                        continue;
                    }

                    if (VIS.DisplayType.IsLOB(tabField[i].getDisplayType())) {
                        continue;
                    }

                    if (cardView.hasIncludedCols && !isShowAllColumn) {
                        var result = jQuery.grep(columnFieldArray, function (value) {
                            return value == tabField[i].getAD_Field_ID();
                        });
                        if (result.length > 0) {

                            continue;
                        }
                    }
                    ulLeftColumns.append("<li index=" + i + " FieldID=" + tabField[i].getAD_Field_ID() + "> " + tabField[i].getHeader() + "</li>");
                }
            }

        };



        var Events = function () {


            if (btnNew != null) {
                btnNew.on("click", function (e) {
                    e.stopPropagation();
                    isEdit = false;
                    lableCardViewName.css({ "display": "block" });
                    txtCardViewName.css({ "display": "block" });
                    ddlCardView.css({ "display": "none" });
                    cmbCardView.css({ "display": "none" });
                    btnNew.css({ "display": "none" });
                    btnCancle.css({ "display": "block" });
                    btnEdit.css({ "display": "none" });
                    rootCardViewUI.find(".vis-cardviewchild").css({ "width": "81%" });
                    isNewRecord = true;
                    LstRoleID = [];
                    UnSelectRoleUl();
                    cmbGroupField.find("[FieldID='" + -1 + "']").attr("selected", "selected");
                    FillColumnInclude(false, true);
                    ulRightColumns.children().remove();
                    ulRole.find('input[type=checkbox]').attr('disabled', false);
                    LastCVCondition = cardviewCondition;
                    cardviewCondition = [];
                    AddRow(cardviewCondition);
                });
            }
            if (btnCancle != null) {
                btnCancle.on("click", function (e) {
                    e.stopPropagation();
                    isEdit = false;
                    ddlCardView.css({ "display": "block" });
                    cmbCardView.css({ "display": "block" });
                    lableCardViewName.css({ "display": "none" });
                    txtCardViewName.css({ "display": "none" });
                    btnNew.css({ "display": "block" });
                    btnCancle.css({ "display": "none" });
                    txtCardViewName.val("");
                    btnEdit.css({ "display": "block" });
                    rootCardViewUI.find(".vis-cardviewchild").css({ "width": "67%" });
                    isNewRecord = false;
                    FillColumnInclude(true, false);
                    FillGroupFields();
                    FillRoleList(ulRole);
                    AddRow(LastCVCondition);
                });
            }

            if (btnEdit != null) {
                btnEdit.on("click", function (e) {
                    e.stopPropagation();
                    isEdit = true;
                    lableCardViewName.css({ "display": "block" });
                    txtCardViewName.css({ "display": "block" });
                    ddlCardView.css({ "display": "none" });
                    cmbCardView.css({ "display": "none" });
                    btnNew.css({ "display": "none" });
                    btnCancle.css({ "display": "block" });
                    btnEdit.css({ "display": "none" });
                    rootCardViewUI.find(".vis-cardviewchild").css({ "width": "81%" });
                    txtCardViewName.val(cmbCardView.find(":selected").text().trim());

                });
            }
            if (cmbUser != null) {
                cmbUser.on("change", function () {
                    //if (cardViewUserID != VIS.context.getAD_User_ID() && AD_CardView_ID == 0) {
                    //    isNewRecord = true;
                    //}
                    //if (VIS.MRole.isAdministrator) {
                    //    AD_User_ID = parseInt($(this).find(":selected").attr("ad_user_id"));
                    //    if (cardViewUserID != AD_User_ID) {
                    //        isNewRecord = true;
                    //        txtCardViewName.val(txtCardViewName.val() + $(this).find(":selected").text());
                    //    }
                    //}
                    //else {
                    //    AD_User_ID = VIS.context.getAD_User_ID();
                    //}

                });
            }
            if (ulWindowField != null) {
                ulWindowField.on("click", "li", function () {
                    if (lastSelectWindowFieldItem != null) {
                        lastSelectWindowFieldItem.css("background-color", "white");
                    }
                    $(this).css("background-color", "#1aa0ed");
                    lastSelectWindowFieldItem = $(this);
                    //windowFieldindex = $(this).attr("index");
                    windowFieldindex = $(this).index();
                    WindowAD_Field_ID = $(this).attr("FieldID");
                    FieldName = $(this).text();
                });
            }
            if (ulCardViewColumnField != null) {
                ulCardViewColumnField.on("click", "li", function () {
                    if (lastSelectCardViewColumnFieldItem != null) {
                        lastSelectCardViewColumnFieldItem.css("background-color", "white");
                    }
                    $(this).css("background-color", "#1aa0ed");
                    lastSelectCardViewColumnFieldItem = $(this);
                    //cardViewColumnFieldindex = $(this).attr("index");
                    cardViewColumnFieldindex = $(this).index();
                    cardViewColumnAD_Field_ID = $(this).attr("FieldID");
                    AD_CardViewColumn_ID = $(this).attr("cardviewcolumnid");
                    FieldName = $(this).text();
                    seqNo = $(this).attr("seqno");
                });
            }
            if (btnRight != null) {
                btnRight.on("click", function (e) {
                    if (parseInt(WindowAD_Field_ID) <= 0) {
                        return;
                    }
                    ulCardViewColumnField.append("<li seqno=" + 0 + " index=" + count + "  FieldID=" + WindowAD_Field_ID + "> " + FieldName + "</li>");
                    WindowAD_Field_ID = 0;
                    ulWindowField.children().eq(windowFieldindex).remove();
                    e.stopPropagation();
                    e.preventDefault();

                });
            }
            if (btnLeft != null) {
                btnLeft.on("click", function (e) {
                    if (parseInt(cardViewColumnAD_Field_ID) <= 0) {
                        return;
                    }
                    ulCardViewColumnField.children().eq(cardViewColumnFieldindex).remove();
                    var lastIndex = ulWindowField.children().length + 1;
                    ulWindowField.append("<li seqno=" + 0 + " index=" + lastIndex + "  FieldID=" + cardViewColumnAD_Field_ID + "> " + FieldName + "</li>");
                    cardViewColumnAD_Field_ID = 0;
                    cardViewColumnFieldindex = -1;
                    e.stopPropagation();
                    e.preventDefault();
                });
            }
            if (btnUp != null) {
                btnUp.on("click", function (e) {
                    if (parseInt(cardViewColumnFieldindex) == 0 || parseInt(cardViewColumnFieldindex) == -1) {
                        return;
                    }
                    // var currentColArray = cardViewColArray[parseInt(cardViewColumnFieldindex)];
                    //cardViewColArray.splice(parseInt(cardViewColumnFieldindex), 1);
                    var currenIndex = parseInt(cardViewColumnFieldindex) - 1;
                    var currentLiElement = ulCardViewColumnField.children().eq(cardViewColumnFieldindex);
                    var preElement = ulCardViewColumnField.children().eq(currenIndex);
                    ulCardViewColumnField.children().eq(cardViewColumnFieldindex).remove();
                    ulCardViewColumnField.children().eq(currenIndex).before(currentLiElement);
                    // currentLiElement.attr("index", currenIndex);
                    //  preElement.attr("index", parseInt(cardViewColumnFieldindex));
                    cardViewColumnFieldindex = currenIndex;
                    //  cardViewColArray.splice(currenIndex, 0, currentColArray);
                    //  ReloadCardViewColumns($("#CardViewColumnField"), currentColArray.AD_Field_ID);
                    e.stopPropagation();
                    e.preventDefault();
                });
            }
            if (btnDown != null) {
                btnDown.on("click", function (e) {
                    var len = ulCardViewColumnField.children().length - 1;
                    if (len == cardViewColumnFieldindex || parseInt(cardViewColumnFieldindex) == -1) {
                        return;
                    }
                    var currenIndex = parseInt(cardViewColumnFieldindex) + 1;
                    var currentLiElement = ulCardViewColumnField.children().eq(cardViewColumnFieldindex);
                    var preElement = ulCardViewColumnField.children().eq(currenIndex);
                    ulCardViewColumnField.children().eq(cardViewColumnFieldindex).remove();
                    preElement.after(currentLiElement);
                    cardViewColumnFieldindex = currenIndex;
                    e.stopPropagation();
                    e.preventDefault();
                });
            }


            if (ulRole != null) {
                ulRole.on("click", "li", function () {
                    var roleID = $(this).attr("ad_role_id");
                    if (!$(this).find("input").is(":checked")) {
                        $(this).find("input").prop("checked", false);
                        for (var i = 0; i < LstRoleID.length; i++) {
                            if (LstRoleID[i].AD_Role_ID == roleID) {
                                LstRoleID.splice(i, 1);
                            }

                        }
                    }
                    else {
                        $(this).find("input").prop("checked", true);
                        LstRoleID.push({ AD_Role_ID: parseInt(roleID) });
                    }

                });
            }

            if (btnOk != null) {
                btnOk.on("click", function (e) {
                    IsBusy(true);
                    window.setTimeout(function () {
                        var cvConditionValue = "";
                        var cvConditionText = "";
                        strConditionArray = [];
                        var queryValue = "";
                        for (i = 0; i < cardviewCondition.length; i++) {
                            cvConditionValue = "";
                            cvConditionText = "";
                            for (j = 0; j < cardviewCondition[i].Condition.length; j++) {
                                if (j == 0) {
                                    cvConditionValue += "@" + cardviewCondition[i].Condition[j].ColName + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryValue;
                                    cvConditionText += "@" + cardviewCondition[i].Condition[j].ColHeader + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryText;
                                }
                                else {
                                    cvConditionValue += " & " + "@" + cardviewCondition[i].Condition[j].ColName + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryValue;
                                    cvConditionText += " & " + "@" + cardviewCondition[i].Condition[j].ColHeader + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryText
                                }
                                //  cvConditionValue += "@" + cardviewCondition[i].Condition[j].ColName + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryValue + " & ";
                                //cvConditionText += "@" + cardviewCondition[i].Condition[j].ColHeader + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryText + " & ";
                            }
                            strConditionArray.push({ "Color": cardviewCondition[i].Color.toString(), "ConditionValue": cvConditionValue, "ConditionText": cvConditionText })
                        }


                        if (isNewRecord) {
                            if (txtCardViewName.val() == "") {
                                VIS.ADialog.error("FillMandatory", true, "Name");
                                IsBusy(false);
                                return false;
                            }

                        }
                        else if (!isNewRecord && (AD_CardView_ID < 1 && VIS.MRole.isAdministrator)) {
                            VIS.ADialog.error("ClickNew", true, "");
                            IsBusy(false);
                            return false;
                        }




                        var retVal = {};
                        retVal.FieldGroupID = AD_GroupField_ID;
                        retVal.IncludedCols = SaveCardViewColumn(cardViewColArray);
                        if (!retVal.IncludedCols) {
                            return false;
                        }
                        retVal.Conditions = strConditionArray;
                        retVal.AD_CardView_ID = AD_CardView_ID;
                        if (isNewRecord && parseInt(AD_User_ID) < 0) {
                            retVal.FieldGroupID = orginalAD_GroupField_ID;
                            retVal.IncludedCols = orginalIncludedCols;
                            retVal.Conditions = strConditionArray;
                            retVal.AD_CardView_ID = orginalAD_CardView_ID;
                        }
                        cardView.setCardViewData(retVal);
                        if (gc.isCardRow)
                            cardView.refreshUI(gc.getVCardPanel().width());
                        ch.close();
                        e.stopPropagation();
                        e.preventDefault();
                    }, 50);
                });
            }
            if (btnCardViewCancle != null) {
                btnCardViewCancle.on("click", function (e) {
                    ch.close();
                });
            }
            if (btnDelete != null) {
                btnDelete.on("click", function (e) {
                    var diaDel = confirm(VIS.Msg.getMsg("SureWantToDelete"));
                    if (diaDel) {
                        isFirstLoad = false;
                        DeleteCardView();

                        FillCardViewCombo(true);
                        FillTextControl();
                        FillGroupFields();
                        FillColumnInclude(true, false);
                        if (cardViewUserID == VIS.context.getAD_User_ID()) {
                            AD_User_ID = 0;
                            cardViewUserID = 0;
                        }
                    }
                    e.stopPropagation();
                    e.preventDefault();
                });
            }

            if (cmbColumn != null) {

                // setBusy(false);

                cmbColumn.on("change", function (evt) {

                    evt.stopPropagation();
                    //SetQueryValueControl();

                    var columnName = cmbColumn.val();
                    setControlNullValue(true);
                    if (columnName && columnName != "-1") {
                        var dsOp = null;
                        var dsOpDynamic = null;
                        // if column name is of ant ID
                        if (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")) {
                            // fill dataset with operators of type ID
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.CVOPERATORS_ID);
                        }
                        else if (columnName.startsWith("Is")) {
                            // fill dataset with operators of type Yes No
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_YN);
                        }
                        else {
                            // fill dataset with all operators available
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.CVOPERATORS);
                        }

                        var f = mTab.getField(columnName);

                        //if (f != null && VIS.DisplayType.IsDate(f.getDisplayType())) {
                        //   // drpDynamicOp.html(self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DATE_DYNAMIC, true));
                        //   // divDynamic.show();
                        //    //chkDynamic.prop("disabled", false);
                        //   // setDynamicQryControls();
                        //}
                        //else if (self.getIsUserColumn(columnName)) {
                        //   // drpDynamicOp.html(self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DYNAMIC_ID, true));
                        //   // divDynamic.show();
                        //    //chkDynamic.prop("disabled", false);
                        //    //setDynamicQryControls(true);
                        //}

                        drpOp.html(dsOp);
                        drpOp[0].SelectedIndex = 0;
                        // get field
                        var field = getTargetMField(columnName);
                        // set control at value1 position
                        setControl(true, field);
                        // enable the save row button
                        // setEnableButton(btnSave, true);//silverlight comment
                        drpOp.prop("disabled", false);
                    }
                    // enable control at value1 position
                    setValueEnabled(true);
                    // disable control at value2 position
                    // setValue2Enabled(false);
                });
            }



            if (drpOp != null) {

                drpOp.on("change", function () {
                    var selOp = drpOp.val();

                    // set control at value2 position according to the operator selected
                    if (!selOp) {
                        selOp = "";
                    }

                    var columnName = "";
                    var field = "";

                    if (selOp && selOp != "0") {
                        //if user selects between operator
                        if (VIS.Query.prototype.BETWEEN.equals(selOp)) {
                            columnName = cmbColumn.val();
                            // get field
                            field = getTargetMField(columnName);
                            // set control at value2 position
                            setControl(false, field);
                            // enable the control at value2 position
                            // setValue2Enabled(true);
                        }
                        else {
                            //setValue2Enabled(false);
                        }
                    }
                    else {
                        setEnableButton(btnSave, false);//
                        // setValue2Enabled(false);
                        setControlNullValue(true);
                    }
                });
            }
            if (btnSave != null) {
                btnSave.on("click", function () {
                    if (cmbColumn.find(":selected").val() == -1) {
                        return;
                    }
                    var condition = {};
                    cvConditionArray = {};

                    var colorValue = "";

                    if (!Modernizr.inputtypes.color) {
                        colorValue = ctrColor.spectrum('get');
                    }
                    else {
                        colorValue = ctrColor.val();
                    }



                    var index = $.map(cardviewCondition, function (value, i) {
                        if (value.Color == colorValue) {
                            return i;
                        }
                    });


                    if (index.length <= 0) {
                        cvConditionArray["Color"] = colorValue.toString();
                        cvConditionArray["Condition"] = [];
                        condition["ColHeader"] = cmbColumn.find(":selected").text();
                        condition["ColName"] = cmbColumn.find(":selected").val();
                        condition["Operator"] = drpOp.val();
                        condition["OperatorText"] = drpOp.find(":selected").text();;
                        condition["QueryValue"] = getControlValue(true);
                        condition["QueryText"] = getControlText(true);
                        cvConditionArray["Condition"].push(condition);
                        cardviewCondition.push(cvConditionArray);
                    }
                    else {
                        condition["ColHeader"] = cmbColumn.find(":selected").text();
                        condition["ColName"] = cmbColumn.find(":selected").val();
                        condition["Operator"] = drpOp.val();
                        condition["OperatorText"] = drpOp.find(":selected").text();;
                        condition["QueryValue"] = getControlValue(true);
                        condition["QueryText"] = getControlText(true);
                        cardviewCondition[index[0]].Condition.push(condition);
                    }
                    AddRow(cardviewCondition);
                    cmbColumn.find("[value='" + -1 + "']").attr("selected", "selected");
                    drpOp[0].SelectedIndex = -1;
                    SetControlValue(true);
                    SetControlText(true);
                });
            }

            if (cvTable != null) {
                cvTable.on("click", "tr td img", function () {
                    var rowIndex = $(this).parent().parent().index();
                    var selectRowColor = $(this).parent().parent().children().eq(0).attr("value");
                    var colName = $(this).parent().parent().children().eq(1).attr("value");
                    cvTable.find("tr").eq(rowIndex).remove();
                    var idx = $.map(cardviewCondition, function (value, i) {
                        if (value.Color == selectRowColor) {
                            return i;
                        }
                    });
                    for (i = 0; i < cardviewCondition[idx].Condition.length; i++) {

                        if (colName == cardviewCondition[idx].Condition[i].ColName) {
                            cardviewCondition[idx].Condition.splice(i, 1);
                        }
                        if (cardviewCondition[idx].Condition.length <= 0) {
                            cardviewCondition.splice(idx, 1);
                            break;
                        }
                    }
                });
            }
            if (cvTable != null) {
                cvTable.on("click", "tr td", function () {
                    var value = $(this).attr("value");
                    if (typeof (value) != "undefined" && value.startsWith("#")) {

                        if (!Modernizr.inputtypes.color) {
                            ctrColor.spectrum("set", value);
                        }
                        else {
                            ctrColor.val(value);
                        }
                    }
                });
            }
        };


        function UnSelectRoleUl() {
            if (ulRole != null) {
                ulRole.children().find("input").prop('checked', false)
            }

        };
        var ReloadCardViewColumns = function (ulRoot, ad_Field_ID) {
            if (ulRoot != null) {
                ulRoot.children().remove();
            }
            for (var i = 0; i < cardViewColArray.length; i++) {
                ulRoot.append("<li seqno=" + cardViewColArray[i].SeqNo + " index=" + i + " FieldID=" + cardViewColArray[i].AD_Field_ID + "> " + cardViewColArray[i].FieldName + "</li>");
            }
            if (ad_Field_ID > 0) {
                lastSelectCardViewColumnFieldItem = ulRoot.find("[fieldid='" + ad_Field_ID + "']");
                ulRoot.find("[fieldid='" + ad_Field_ID + "']").css("background-color", "#1aa0ed");
            }
        };
        var ReloadWindowFieldsColumn = function (ulRoot, result) {
            if (ulRoot != null) {
                ulRoot.children().remove();
            }
            for (var i = 0; i < result.length; i++) {
                var item = jQuery.grep(cardViewColArray, function (value) {
                    return value.AD_Field_ID == result[i].getAD_Field_ID();
                });
                if (item.length > 0) {
                    continue;
                }
                ulRoot.append("<li index=" + i + " FieldID=" + result[i].getAD_Field_ID() + "> " + result[i].getHeader() + "</li>");
            }
        };
        var FillCardViewColumns = function (ulRoot, isReload) {
            if (ulRoot != null) {
                ulRoot.children().remove();
            }
            cardViewColArray = [];
            cardViewColumns = [];
            columnFieldArray = [];
            if (isReload && (AD_CardView_ID > 0 || typeof (AD_CardView_ID) == "undefined")) {
                if (typeof (AD_CardView_ID) == "undefined") {
                    AD_CardView_ID = 0;
                }
                var url = VIS.Application.contextUrl + "CardView/GetCardViewColumns";
                $.ajax({
                    type: "GET",
                    async: false,
                    url: url,
                    dataType: "json",
                    contentType: 'application/json; charset=utf-8',
                    data: { ad_CardView_ID: AD_CardView_ID },
                    success: function (data) {
                        dbResult = JSON.parse(data);
                        var CVColumns = dbResult[0].lstCardViewData;
                        LstCardViewCondition = dbResult[0].lstCardViewConditonData;
                        if (CVColumns != null && CVColumns.length > 0) {
                            AD_GroupField_ID = CVColumns[0].AD_GroupField_ID;
                            cardViewUserID = CVColumns[0].UserID;
                            for (var i = 0; i < CVColumns.length; i++) {
                                if (CVColumns[i].AD_Field_ID == 0) {
                                    continue;
                                }
                                var fieldItem = jQuery.grep(totalTabFileds, function (value) {
                                    return value.getAD_Field_ID() == CVColumns[i].AD_Field_ID
                                });
                                if (fieldItem.length > 0) {
                                    columnFieldArray.push(fieldItem[0].getAD_Field_ID());
                                }
                                ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + CVColumns[i].AD_Field_ID + "> " + CVColumns[i].FieldName + "</li>");
                            }
                        }
                        if (LstCardViewCondition != null && LstCardViewCondition.length > 0) {
                            cardviewCondition = [];
                            FillCVConditonTable(LstCardViewCondition);
                        }
                        else {
                            cardviewCondition = [];
                            AddRow(cardviewCondition);
                        }
                    }, error: function (errorThrown) {
                        alert(errorThrown.statusText);
                    }
                });
            }
            else if (cardView.hasIncludedCols) {
                var fieldItem = null;
                columnFieldArray = [];
                var includedFields = cardView.fields;
                //cardViewColumns = cardView.fields;
                if (includedFields != null && includedFields.length > 0) {
                    for (var i = 0; i < includedFields.length; i++) {
                        fieldItem = jQuery.grep(totalTabFileds, function (value) {
                            return value.getAD_Field_ID() == includedFields[i].getAD_Field_ID()
                        });
                        if (fieldItem.length > 0) {
                            columnFieldArray.push(fieldItem[0].getAD_Field_ID());
                        }

                        cardViewColArray.push({ AD_Field_ID: includedFields[i].getAD_Field_ID(), CardViewID: AD_CardView_ID, SeqNo: 0, FieldName: includedFields[i].getHeader() });
                        //ulRoot.append("<li seqno=" + cardViewColumns[i].SeqNo + " index=" + i + " CardViewColumnID=" + cardViewColumns[i].AD_CardViewColumn_ID + " FieldID=" + cardViewColumns[i].AD_Field_ID + "> " + dbResult[i].FieldName + "</li>");
                        ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + includedFields[i].getAD_Field_ID() + "> " + includedFields[i].getHeader() + "</li>");
                    }
                }
            }
        };

        var SaveCardViewColumn = function (lstCardView) {

            var isShow
            if (VIS.MRole.isAdministrator) {
                if (isNewRecord) {
                    cardViewName = txtCardViewName.val();
                    if (LstRoleID == null || LstRoleID.length <= 0) {
                        if (AD_User_ID != VIS.context.getAD_User_ID()) {
                            if (!confirm(VIS.Msg.getMsg("CardViewWarning"))) {
                                AD_User_ID = 0;
                                return false;
                            }
                            AD_User_ID = VIS.context.getAD_User_ID();
                        }
                        else
                            AD_User_ID = 0;
                    }
                    else AD_User_ID = 0;
                } else {
                    cardViewName = txtCardViewName.val();
                    if (AD_User_ID == cardViewUserID) {
                        // if (orginalAD_CardView_ID > 0 && orginalcardViewUserID > 0){
                        //     AD_CardView_ID = orginalAD_CardView_ID;
                        //     cardViewUserID = orginalcardViewUserID;
                        //}
                    }
                    else {
                        AD_User_ID = 0;
                    }
                }
            }
            else {

                if (!cardViewUserID || cardViewUserID < 1) {
                    isNewRecord = true;
                    AD_User_ID = VIS.context.getAD_User_ID();
                }

                if (isNewRecord) {
                    if (orginalAD_CardView_ID > 0 && orginalcardViewUserID > 0) {
                        isNewRecord = false;
                        AD_CardView_ID = orginalAD_CardView_ID;
                        cardViewUserID = orginalcardViewUserID;
                        cardViewName = cmbCardView.find(":selected").text() + " (" + defaultMsg + ")";
                    }
                    else {
                        cardViewName = cmbCardView.find(":selected").text() + " " + defaultMsg;
                    }
                }
            }
            if (cardViewName.length < 1 && !isEdit) {
                cardViewName = cmbCardView.find(":selected").text();
            }
            else if (isEdit) {
                cardViewName = txtCardViewName.val();
            }
            var len = ulCardViewColumnField.children().length;
            cardViewColArray = [];
            var includeCols = [];
            for (var i = 0; i < len; i++) {
                var f = {};
                f.AD_Field_ID = ulCardViewColumnField.children().eq(i).attr("fieldid");
                f.CardViewID = AD_CardView_ID;
                cardViewColArray.push(f);
                includeCols.push(parseInt(f.AD_Field_ID));
            }

            cardViewArray.push({ AD_Window_ID: AD_Window_ID, AD_Tab_ID: AD_Tab_ID, UserID: AD_User_ID, AD_GroupField_ID: AD_GroupField_ID, isNewRecord: isNewRecord, CardViewName: cardViewName, CardViewID: AD_CardView_ID });
            var url = VIS.Application.contextUrl + "CardView/SaveCardViewColumns";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ 'lstCardView': cardViewArray, 'lstCardViewColumns': cardViewColArray, 'LstRoleID': LstRoleID, 'lstCardViewCondition': strConditionArray }),
                success: function (data) {
                    var result = JSON.parse(data);
                    AD_CardView_ID = result;
                    IsBusy(false);
                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }
            });
            return includeCols;


        };

        var DeleteCardView = function () {
            var url = VIS.Application.contextUrl + "CardView/DeleteCardViewRecord";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ 'ad_CardView_ID': AD_CardView_ID }),
                success: function (data) {
                    var result = JSON.parse(data);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                }
            });
        };

        var FillUserList = function (root) {
            if (root != null)
            { root.children().remove(); }
            root.append("<Option ad_user_id=" + -1 + " ></Option>");
            for (var i = 0; i < userInfo.length; i++) {
                root.append("<Option ad_user_id=" + userInfo[i].AD_User_ID + " > " + userInfo[i].UserName + "</Option>");
            }
            if (cardViewUserID > 0)
                cmbUser.find("[ad_user_id='" + cardViewUserID + "']").attr("selected", "selected");
        };


        var FillRoleList = function (root) {
            LstRoleID = [];
            var d = '';
            if (cardViewUserID > 0) {
                d = 'disabled';
            }

            if (root != null)
            { root.children().remove(); }
            for (var i = 0; i < roleInfo.length; i++) {
                root.append("<li ad_role_id=" + roleInfo[i].AD_Role_ID + " > <input type='checkbox' " + d + "> " + w2utils.encodeTags(roleInfo[i].RoleName) + "</li>");
            }
            for (var i = 0; i < LstCardViewRole.length; i++) {
                if (LstCardViewRole[i] == null) {
                    continue;
                }
                for (var j = 0; j < LstCardViewRole[i].length; j++) {
                    if (LstCardViewRole[i][j].AD_CardView_ID == AD_CardView_ID) {
                        root.find("[ad_role_id='" + LstCardViewRole[i][j].AD_Role_ID + "']").find("input").prop('checked', true);
                        LstRoleID.push({ AD_Role_ID: LstCardViewRole[i][j].AD_Role_ID });
                    }
                }
            }
            //if (cardViewUserID > 0)
            //    root.find("[ad_role_id='" + cardViewUserID + "']").attr("selected", "selected");
        };
        var changeHeader = function () {
            if (cardViewUserID > 0) {
                if (ch != null)
                    ch.changeTitle(VIS.Msg.getMsg("Card") + "(" + VIS.Msg.getMsg("IsPrivate") + ")");
            }
            else {
                if (ch != null)
                    ch.changeTitle(VIS.Msg.getMsg("Card"));
            }
        };

        var CreateCVGrid = function (root) {
            var cvHeaderTable = $("<Table class='vis-cv-headertable'><tr class='vis-cv-TableHead'>" +
                                "<th >" + VIS.Msg.getMsg("BGColor") + "</th>" +
                                "<th>" + VIS.Msg.getMsg("Column") + "</th>" +
                                "<th>" + VIS.Msg.getMsg("Operator") + "</th>" +
                                "<th>" + VIS.Msg.getMsg("QueryValue") + "</th>" +
                                "<th>" + VIS.Msg.getMsg("Action") + "</th>" +
                            "<tr></Table>");
            var cvRowTable = $("<div class='vis-cv-divrowtable'><Table class='vis-cv-rowtable'></Table></div>");
            root.append(cvHeaderTable);
            root.append(cvRowTable);
        };

        var AddRow = function (data) {
            if (cvTable != null) {
                cvTable.children().remove();
            }
            if (data != null) {
                var row = null;
                var col1Width = rootCardViewUI.find(".vis-cv-TableHead").children().eq(0).innerWidth();
                var col2Width = rootCardViewUI.find(".vis-cv-TableHead").children().eq(1).innerWidth();
                var col3Width = rootCardViewUI.find(".vis-cv-TableHead").children().eq(2).innerWidth();
                var col4Width = rootCardViewUI.find(".vis-cv-TableHead").children().eq(3).innerWidth();
                var col5Width = rootCardViewUI.find(".vis-cv-TableHead").children().eq(4).innerWidth();
                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < data[i].Condition.length; j++) {
                        row = $("<tr class='vis-cv-TableRow'></tr");
                        row.append("<td value=" + data[i].Color + " style='width:" + col1Width + "px'><div class='vis-cvd-bgcolor' style='background-color:" + data[i].Color + ";cursor:pointer'>" + data[i].Color + "</div></td>");
                        row.append("<td style='width:" + col2Width + "px' value=" + data[i].Condition[j].ColName + ">" + data[i].Condition[j].ColHeader + "</td>");
                        row.append("<td style='width:" + col3Width + "px' value=" + data[i].Condition[j].Operator + ">" + data[i].Condition[j].OperatorText + "</td>");
                        row.append("<td style='width:" + col4Width + "px' value=" + data[i].Condition[j].QueryValue + ">" + data[i].Condition[j].QueryText + "</td>");
                        row.append("<td style='width:" + col5Width + "px'><img  style='cursor:pointer;margin-right:5px;width:20px;height:20px'  src='" + VIS.Application.contextUrl + "Areas/VIS/Images/delete-ico.png' > </td>");
                        cvTable.append(row);
                    }
                }
            }
        };
        var FillCVConditionCmbColumn = function () {
            var html = '<option value="-1"> </option>';
            for (var c = 0; c < findFields.length; c++) {
                // get field
                var field = findFields[c];
                if (field.getIsEncrypted())
                    continue;
                // get field's column name
                var columnName = field.getColumnName();
                if (field.getDisplayType() == VIS.DisplayType.Button) {
                    if (field.getAD_Reference_Value_ID() == 0)
                        continue;
                    if (columnName.endsWith("_ID"))
                        field.setDisplayType(VIS.DisplayType.Table);
                    else
                        field.setDisplayType(VIS.DisplayType.List);
                    //field.loadLookUp();
                }
                // get text to be displayed
                var header = field.getHeader();
                if (header == null || header.length == 0) {
                    // get text according to the language selected
                    header = VIS.Msg.getElement(VIS.context, columnName);
                    if (header == null || header.Length == 0)
                        continue;
                }
                // if given field is any key, then add "(ID)" to it
                if (field.getIsKey())
                    header += (" (ID)");

                // add a new row in datatable and set values
                //dr = dt.NewRow();
                //dr[0] = header; // Name
                //dr[1] = columnName; // DB_ColName
                //dt.Rows.Add(dr);
                html += '<option value="' + columnName + '">' + header + '</option>';
            }
            cmbColumn.html(html);
        };

        var SetQueryValueControl = function () {
            var columnName = cmbColumn.val();
            setControlNullValue(true);
            if (columnName && columnName != "-1") {
                // get field
                var field = getTargetMField(columnName);
                // set control at value1 position
                setControl(true, field);
                // enable the save row button
                //setEnableButton(btnSave, true);//silverlight comment
            }
            // enable control at value1 position
            setValueEnabled(true);
        };
        function getTargetMField(columnName) {
            // if no column name, then return null
            if (columnName == null || columnName.length == 0)
                return null;
            // else find field for the given column
            for (var c = 0; c < mTab.getFields().length; c++) {
                var field = mTab.getFields()[c];
                if (columnName.equals(field.getColumnName()))
                    return field;
            }
            return null;
        };

        function getControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                if (VIS.DisplayType.IsDate(crtlObj.getDisplayType())) {

                    var val = crtlObj.getValue();
                    if (val && val.endsWith('.000Z'))
                        val = val.replace('.000Z', 'Z');
                    return val;
                }
                // return control's value
                return crtlObj.getValue();
            }
            return "";
        };
        function SetControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                // return control's value
                crtlObj.setValue("");
            }

        };

        /* <param name="isValue1">true if get control's text at value1 position else false</param>
         */
        function getControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplay();
            }
            return "";
        };
        function SetControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplayType("");
            }
            return "";
        };
        function setControlNullValue(isValue2) {
            var crtlObj = null;
            if (isValue2) {
                crtlObj = control1;
            }

            // if control exists
            if (crtlObj != null) {
                crtlObj.setValue(null);
            }
        };

        function setValueEnabled(isEnabled) {
            // get control
            var ctrl = divValue1.children()[1];
            var btn = null;
            if (divValue1.children().length > 2)
                btn = divValue1.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setEnableButton(btn, isEnable) {
            btn.prop("disabled", !isEnable);
        };

        function setControl(isValue1, field) {

            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[0];
                if (divValue1.children().length > 1)
                    ctrl2 = divValue1.children()[1];
            }

            //Remove any elements in the list
            if (ctrl != null) {
                $(ctrl).remove();
                if (ctrl2 != null)
                    $(ctrl2).remove();
                ctrl = null;
            }
            /**********************************/
            var crt = null;
            // if any filed is given
            if (field != null) {
                // if field id any key, then show number textbox 
                if (field.getIsKey()) {
                    crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                                     field.getColumnName());
                }
                else {
                    crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                }
            }
            else {
                // if no field is given show an empty disabled textbox
                crt = new VIS.Controls.VTextBox("columnName", false, true, false, 20, 20, "format",
                          "GetObscureType", false);// VAdvantage.Controls.VTextBox.TextType.Text, DisplayType.String);
            }
            if (crt != null) {
                //crt.SetIsMandatory(false);
                crt.setReadOnly(false);

                if (VIS.DisplayType.Text == field.getDisplayType() || VIS.DisplayType.TextLong == field.getDisplayType()) {
                    crt.getControl().attr("rows", "1");
                    crt.getControl().css("width", "100%");
                }
                else if (VIS.DisplayType.YesNo == field.getDisplayType()) {
                    crt.getControl().css("clear", "both");
                }
                else if (VIS.DisplayType.IsDate(field.getDisplayType())) {
                    crt.getControl().css("line-height", "1");
                }

                var btn = null;
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {

                    divValue1.append(crt.getControl().css("width", "95%"));
                    control1 = crt;
                    if (btn) {
                        divValue1.append(btn);
                        crt.getControl().css("width", "60%");
                        btn.css("max-width", "35px");
                    }
                }
            }
        };

        function FillCVConditonTable(data) {
            var condition = {};
            cvConditionArray = {};
            var strConditionValue = "";
            var strConditionText = "";

            var s = null;
            var st = null;
            var colHeader = null;
            var colVaue = null;
            var queryValue = null;
            var queryText = null;
            var operator = null;
            var operatorText = null;
            for (var i = 0; i < data.length; i++) {
                cvConditionArray = {};
                cvConditionArray["Color"] = data[i].Color;
                cvConditionArray["Condition"] = [];
                strConditionValue = data[i].ConditionValue.split("&");
                strConditionText = data[i].ConditionText.split("&");

                for (var j = 0; j < strConditionValue.length; j++) {
                    condition = {}
                    if (strConditionValue[j].contains("!")) {
                        s = strConditionValue[j].trim().split("!");
                        st = strConditionText[j].trim().split("!");
                        operator = "!";
                        operatorText = "!=";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains("=")) {
                        s = strConditionValue[j].split("=");
                        st = strConditionText[j].split("=");
                        operator = "="
                        operatorText = "=";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains("<")) {
                        s = strConditionValue[j].split("<");
                        st = strConditionText[j].split("<");
                        operator = "<";
                        operatorText = "<";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains(">")) {
                        s = strConditionValue[j].split(">");
                        st = strConditionText[j].split(">");
                        operator = ">";
                        operatorText = ">";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }

                    condition["ColHeader"] = colHeader;
                    condition["ColName"] = colVaue;
                    condition["Operator"] = operator;
                    condition["OperatorText"] = operatorText;
                    condition["QueryValue"] = queryValue;
                    condition["QueryText"] = queryText;
                    cvConditionArray["Condition"].push(condition);

                }
                cardviewCondition.push(cvConditionArray);
            }
            if (!isFirstLoad) {
                AddRow(cardviewCondition);
            }
        };

        init();

        this.getRoot = function () {
            return root;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("Card"));
            ch.setContent(root);
            //ch.close = function () {

            //}
            ch.onOkClick = function (e) {
                if (isNewRecord) {
                    if (txtCardViewName.val().trim() == "") {
                        VIS.ADialog.error("FillMandatory", true, "Name");
                        return false;
                    }

                }
                else if (!isNewRecord && ((AD_CardView_ID < 1 && (cmbCardView.val() == "" || cmbCardView.val() == null)) && VIS.MRole.isAdministrator)) {
                    VIS.ADialog.error("ClickNew", true, "");
                    return false;
                }

                //if (AD_GroupField_ID <= 0) {
                //    VIS.ADialog.error("FillMandatory", true, "GroupField");
                //    return false;
                //}
                var retVal = {};
                retVal.FieldGroupID = AD_GroupField_ID;
                retVal.IncludedCols = SaveCardViewColumn(cardViewColArray);
                if (!retVal.IncludedCols) {
                    return false;
                }
                retVal.Conditions = [];
                retVal.AD_CardView_ID = AD_CardView_ID;
                if (VIS.MRole.isAdministrator && AD_User_ID < 1 && orginalAD_CardView_ID > 0) {
                    //retVal.FieldGroupID = orginalAD_GroupField_ID;
                    //retVal.IncludedCols = orginalIncludedCols;
                    retVal.Conditions = [];
                    retVal.AD_CardView_ID = orginalAD_CardView_ID;
                }
                cardView.setCardViewData(retVal);
                if (gc.isCardRow)
                    cardView.refreshUI(gc.getVCardPanel().width());

                //}
            };
            ch.show();
            ch.hideButtons();
            changeHeader();
            AddRow(cardviewCondition);
            if (!Modernizr.inputtypes.color) {
                ctrColor.spectrum({
                    showButtons: false,
                    preferredFormat: "hex"
                });
            };
        };

    }


    cvd.prototype.getOperatorsQuery = function (vnpObj, translate) {
        var html = "";
        var val = "";
        for (var p in vnpObj) {
            val = vnpObj[p];
            if (translate)
                val = VIS.Msg.getMsg(val);
            html += '<option value="' + p + '">' + val + '</option>';
        }
        return html;
    };



    VIS.CVDialog = cvd;
    //alert(VIS.CVDialog);

}(VIS, jQuery));