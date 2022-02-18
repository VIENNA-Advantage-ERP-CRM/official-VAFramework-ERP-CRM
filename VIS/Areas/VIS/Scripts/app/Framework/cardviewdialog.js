; (function (VIS, $) {

    function cvd(aPanel) {
        var gc = aPanel.curGC;
        var mTab = gc.getMTab();
        var cardView = gc.vCardView;
        //var gridWindow = aPanel.gridWindow;
        var AD_Window_ID = mTab.getAD_Window_ID();
        var AD_Tab_ID = mTab.getAD_Tab_ID();
        var tabName = mTab.getName();
        var WindowNo = mTab.getWindowNo();
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
        var lovcardList = {};



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
        var btnEdit = null;
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
        var $vSearchHeaderLayout = null;
        var cmbOrderClause = null;
        var btnCardAsc = null;
        var btnCardDesc = null;
        var isdefault = null;
        var chkPublic = null;
        var lblDefault = null;
        var backgroundType = null;
        var isGradient = null;
        var rootGroupFieldsSeq = null;
        var ulGroupSeqColumns = null;
        var GroupShifBtn = null;
        var btnGrpUp = null;
        var btnGrpDown = null;
        var orderByClause = null;
        // var chkdisabledWindowPage = null;
        var sortDDL = null;
        var btnApply;
        var closeDialog = true;
        var sortOrderArray = [];
        var lastSortOrderArray = [];
        var isAsc = "ASC";
        var sortList;
        // var 
        var root, ch;
        function init() {
            root = $('<div class="vis-forms-container" style="width:100%;height:100%"></div>');
            ArrayTotalTabFields();
            CardViewUI();
            FillCardViewCombo();
            // FillTextControl();
            // FillGroupFields();
            //FillColumnInclude(false);
            isFirstLoad = true;
            cmbVardChange();
            FillCVConditionCmbColumn(cmbColumn);
            if (!isNewRecord || !isEdit) {
                ulGroupSeqColumns.find('input').prop("disabled", true);
                ulCardViewColumnField.find('select').prop("disabled", true);
            }

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

            sortDDL = '<select>'
                + '<option value=""></option>'
                + '<option value="1">↑</option>'
                + '<option value="-1">↓</option></select>';

            // Card View Dropdown


            var $divHeadderLay = $('<div class="input-group vis-input-wrap vis-card-input-wrap">');



            isBusyRoot = $("<div class='vis-apanel-busy vis-cardviewmainbusy'></div> ");
            rootCardViewUI = $("<div class='vis-cardviewmain'></div>");
            var divCardViewMainFirstChild = $("<div class='vis-cardviewmainfirstchild vis-pull-left'></div>");
            var CardViewTopFiledsWrap = $("<div class='vis-cardviewtopfieldswrap'></div>");
            var divCardViewMainSecondChild = $("<div class='vis-cardviewmainsecondchild'></div>");
            var divCardViewCondition = $("<div class='vis-cardviewCondition'></div>").append("<div class='vis-cardviewConditionControls'> </div>  <div class='vis-cardviewConditionGrid'> </div> ");


            var cardviewCondition = $("<div class='vis-cardviewbtn'>");
            //if (VIS.MRole.isAdministrator) {
            //    cardviewCondition.append(lblDefault).append(lblIsPublic);
            //} else {
            //    cardviewCondition.append(lblDefault);
            //}

            var divLayout = $("<div class='d-flex vis-cardviewsortContainer'>");

            //orderByClause = $('<input type="text" name="OrderByClause" >');
            //var lbldisabledWindowPage = $('<label class="vis-ec-col-lblchkbox" style="opacity: 1;width: 50%;">' + VIS.Msg.getMsg("disabledWindowPage") + '</label>');
            //chkdisabledWindowPage = $('<input type="checkbox"  name="disabledWindowPage" value="disabledWindowPage">');
            //lbldisabledWindowPage.prepend(chkdisabledWindowPage);


            var divinputctrl = $("<div class='input-group vis-input-wrap'></div>").append($("<div class='vis-control-wrap'></div>"));//.append(orderByClause).append('<label for="Name">' + VIS.Msg.getMsg("orderByClause") + '</label>'));
            //cardviewCondition.append(lbldisabledWindowPage);

            var divCardViewbtn = $("<div class='vis-cardviewbtn' style='margin-top:0;'> </div>");


            rootCardViewUI.append(divCardViewMainFirstChild);
            divCardViewMainFirstChild.append(CardViewTopFiledsWrap);
            CardViewTopFiledsWrap.append("<div class='vis-firstdiv vis-pull-left' ></div>");
            CardViewTopFiledsWrap.append("<div class='vis-cv-blankdiv' ></div>");
            CardViewTopFiledsWrap.append("<div class='vis-seconddiv vis-pull-right'></div>");
            divCardViewMainFirstChild.append("<div class='vis-thirddiv vis-pull-left' ></div>");
            rootCardViewUI.find(".vis-firstdiv").append("<div class='vis-first-divHeader'><label class='vis-ddlcardview'>" + VIS.Msg.getMsg("SelectCardView") + " </label><label style='display:none' class='vis-lbcardviewname'>" + VIS.Msg.getMsg("CardViewName") + " </label></div>  ")
            var divCardView = $("<div class='vis-CardView vis-pull-left'> <div  class='vis-cardviewchild vis-pull-left'></div>  </div>");
            var divCardViewName = $("<input style='display:none' class='vis-txtcardviewname' type='text'>");
            var divUser = $("<div class='vis-User vis-pull-left'></div>");
            var btnNewAndCancle = $("<div class='vis-pull-left'><button  class='vis-btnnew vis-cvd-btn'><i title=" + VIS.Msg.getMsg("AddNew") + " class='vis vis-plus'></i></button><button style='display:none' title=" + VIS.Msg.getMsg("VIS_DialogCancel") + " class='vis-btncancle vis-cvd-canclebtn'><i class='vis vis-mark'></i></button> <button class='vis-btnedit vis-cvd-editbtn' title=" + VIS.Msg.getMsg("EditRecord") + "  ><i class='vis vis-pencil'></i></button></div>");
            divCardView.find(".vis-cardviewchild").append("<select class='vis-cmbcardview'> </select>");


            divUser.append("<Select type='text' class='vis-cmbuser'> </Select>");



            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "CardView/GetColumnIDWindowID", { tableName: 'AD_CardView', columnName: 'AD_HeaderLayout_ID' });
            if (res) {
                res = res.split(",");
                //var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), WindowNo, res, VIS.DisplayType.Search);
                var value = VIS.MLookupFactory.get(VIS.Env.getCtx(), WindowNo, res[0], VIS.DisplayType.Search, "AD_HeaderLayout_ID", 0, false, "IsHeaderView='N'");
                $vSearchHeaderLayout = new VIS.Controls.VTextBoxButton("AD_HeaderLayout_ID", true, false, true, VIS.DisplayType.Search, value, Number(res[1]));

                var $divCtrl = $('<div class="vis-control-wrap">');
                var $divCtrlBtn = $('<div class="input-group-append">');
                $divCtrl.append($vSearchHeaderLayout.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label for="Name">' + VIS.Msg.getMsg("cardLayout") + '</label>');

                $divHeadderLay.append($divCtrl).append($divCtrlBtn);
                $divCtrlBtn.append($vSearchHeaderLayout.getBtn(0)).append($vSearchHeaderLayout.getBtn(1));
                //divLayout.append();
            }

            divCardViewbtn.append("<button class='vis-btnDelete'><i title=" + VIS.Msg.getMsg("DeleteRecord") + " class='vis vis-delete'></i></button>");

            divCardViewbtn.append($divHeadderLay);

            lblDefault = $('<label class="vis-ec-col-lblchkbox" style="opacity: 1;display:none">' + defaultMsg + '</label>');
            isdefault = $('<input type="checkbox" name="IsDefault" value="Default">');
            lblDefault.prepend(isdefault);
            // divCardViewbtn.append(lblDefault);

            lblIsPublic = $('<label class="vis-ec-col-lblchkbox" style="opacity: 1;">' + VIS.Msg.getMsg("Shared") + '</label>');
            chkPublic = $('<input type="checkbox" checked="true" name="IsPublic" value="Public">');
            lblIsPublic.prepend(chkPublic);
            //divCardViewbtn.append(lblIsPublic);


            if (VIS.MRole.isAdministrator) {
                divCardViewbtn.append(lblDefault).append(lblIsPublic);
            } else {
                divCardViewbtn.append(lblDefault);
            }

            divCardViewbtn.append("<div class='vis-cdv-customokcancle'><button class='vis-btnCardApply'>  " + VIS.Msg.getMsg("SaveAndApply") + "  </button><button class='vis-btnOk'>  " + VIS.Msg.getMsg("SaveIt") + "  </button><button class='vis-btnCardViewCancle'>  " + VIS.Msg.getMsg("close") + "  </button></div>");



            //cmbOrderClause = $('<select class="vis-cardorderclause"></select>');

            //for (var j = 0; j < totalTabFileds.length; j++) {
            //    cmbOrderClause.append('<option value="' + totalTabFileds[j].getColumnName() + '">' + totalTabFileds[j].getHeader() + '</option>')
            //}

            //$divHeadderLay.append(cmbOrderClause);


            //var cmbOrderDir = $('<select>'
            //    + '<option value=""></option>'
            //    + '<option value="1">↑</option>'
            //    + '<option value="-1">↓</option></select>');

            //$divHeadderLay.append(cmbOrderDir);



            var sortHTML = $('<div class="vis-sortOrderContainer">'
                + '<div class="vis-sortOrderData">'
                + '<div class="input-group vis-input-wrap">'
                + '<div class="vis-control-wrap">'
                + '<select name="Rating" class="vis-cardOrderClause" placeholder=" " data-placeholder="">'

                + '</select>'
                + '<label for="Rating">' + VIS.Msg.getMsg("SortOrderField") + '</label>'
                + '</div>'
                + '</div>'

                + '<div class="vis-sortButtons">'
                + '<span class="vis-sortBtn vis-sortCardAsc" title="' + VIS.Msg.getMsg("Ascending") + '" >'
                + '<i class="fa fa-sort-amount-asc"></i>'
                + '</span>'
                + '<span class="vis-sortBtn vis-sortCardDesc" title="' + VIS.Msg.getMsg("Descending") + '" >'
                + '<i class="fa fa-sort-amount-asc"></i>'
                + '</span>'
                + '</div>'

                + '<button class="btn VIS_Pref_btn-2">' + VIS.Msg.getMsg("Add") + '</button>'
                + '</div>'
                + '<div class="vis-sortingList">'

                + '</div>');

            btnCardAsc = sortHTML.find('.vis-sortCardAsc');
            btnCardDesc = sortHTML.find('.vis-sortCardDesc');
            var btnAddOrder = sortHTML.find('.VIS_Pref_btn-2');
            sortList = sortHTML.find('.vis-sortingList');

            btnCardAsc.on('click', function () {

                if (cmbOrderClause.val() == -1 || cmbOrderClause.val() == null)
                    return;

                btnCardAsc.css('color', 'rgba(var(--v-c-primary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                isAsc = "ASC";
            });

            btnCardDesc.on('click', function () {

                if (cmbOrderClause.val() == -1 || cmbOrderClause.val() == null)
                    return;

                btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-primary), 1)');
                isAsc = "DESC";
            });

            cmbOrderClause = sortHTML.find('.vis-cardOrderClause');
            cmbOrderClause.append('<option value="-1"> </option>)');

            totalTabFileds.sort(function (a, b) {
                var n1 = a.getHeader().toUpperCase();
                if (n1 == null || n1.length == 0) {
                    n1 = VIS.Msg.getElement(VIS.context, a.getColumnName());
                }
                var n2 = b.getHeader().toUpperCase();
                if (n2 == null || n2.length == 0) {
                    n2 = VIS.Msg.getElement(VIS.context, b.getColumnName());
                }
                if (n1 > n2) return 1;
                if (n1 < n2) return -1;
                return 0;
            });

            for (var j = 0; j < totalTabFileds.length; j++) {
                var header = totalTabFileds[j].getHeader();
                if (header == null || header.length == 0) {
                    header = VIS.Msg.getElement(VIS.context, totalTabFileds[j].getColumnName());
                    if (header == null || header.Length == 0)
                        continue;
                }


                cmbOrderClause.append('<option value="' + totalTabFileds[j].getColumnName() + '">' + header + '</option>')
            }

            divLayout.append(sortHTML);


            //var btnSaveOrder = $("<button class='vis-btnSaveSort'>" + VIS.Msg.getMsg("SaveIt") + "</button>")
            //$divHeadderLay.append(btnSaveOrder);

            btnAddOrder.on("click", function (e) {
                var selectedVal = cmbOrderClause.val();

                if (selectedVal == -1) {
                    return;
                }

                btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');

                if (sortOrderArray && sortOrderArray.length < 3) {
                    var selectedColtext = cmbOrderClause.find(':selected').text();


                    if (sortOrderArray.indexOf(selectedVal + ' ASC') > -1 || sortOrderArray.indexOf(selectedVal + ' DESC') > -1) {
                        VIS.ADialog.warn("CardSortColAdded");
                        return;
                    }

                    addOrderByClauseItems(selectedColtext, selectedVal, isAsc);
                    sortOrderArray.push(selectedVal + ' ' + isAsc);
                    cmbOrderClause.val(-1);
                }
                else {
                    VIS.ADialog.warn("MaxSortColumn");
                }
            });

            sortList.on("click", function (e) {
                if (isEdit || isNewRecord) {
                    var $target = $(e.target);
                    if ($target.hasClass('fa-close'))
                        $target = $target.parent();

                    if ($target.hasClass('vis-sortListItemClose')) {
                        const itemid = sortOrderArray.indexOf($target.data('text'));
                        sortOrderArray.splice(itemid, 1);
                        $target.closest('.vis-sortListItem').remove();
                    }
                }
            });

            cmbOrderClause.on("change", function () {
                if (cmbOrderClause.val() == -1 || cmbOrderClause.val() == null) {
                    btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                    btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                }
                isAsc = "ASC";
            });



            if (VIS.MRole.isAdministrator) {

                //rootCardViewUI.append(divCardViewMainSecondChild);
                rootCardViewUI.find(".vis-firstdiv").append(" <div style='display:none' class='vis-first-divHeader vis-pull-right'><label >" + VIS.Msg.getMsg("SelectUser") + "</label></div>");
                rootCardViewUI.find(".vis-CardView").append("");
                rootCardViewUI.find(".vis-firstdiv").append(divCardView);

                rootCardViewUI.find(".vis-cardviewchild").append(divCardViewName);
                rootCardViewUI.find(".vis-cardviewchild");
                rootCardViewUI.find(".vis-CardView").append(btnNewAndCancle);
                cmbUser = rootCardViewUI.find(".vis-cmbuser");
                AddCVConditionControl(divCardViewCondition.find(".vis-cardviewConditionControls"));
                rootCardViewUI.append(divCardViewCondition);
                rootCardViewUI.append(cardviewCondition);
                rootCardViewUI.append(divLayout).append(divCardViewbtn);
                rootCardViewUI.find(".vis-btnDelete").css("display", "block");

            }
            else {

                divCardViewCondition;
                rootCardViewUI.find(".vis-firstdiv").append(divCardView);
                rootCardViewUI.find(".vis-cardviewchild").append(divCardViewName);
                rootCardViewUI.find(".vis-cardviewchild");
                rootCardViewUI.find(".vis-CardView").append(btnNewAndCancle);
                //divCardViewMainFirstChild.css({ "width": "100%", "float": "left" });
                rootCardViewUI.find(".k ").css("display", "none");
                AddCVConditionControl(divCardViewCondition.find(".vis-cardviewConditionControls"));
                rootCardViewUI.append(divCardViewCondition);
                rootCardViewUI.append(divLayout).append(cardviewCondition);
                rootCardViewUI.append(divCardViewbtn);
                // divCardViewbtn.css({ "float": "right" });
                rootCardViewUI.find(".vis-btnDelete").css("display", "none");
            }
            CreateCVGrid(rootCardViewUI.find(".vis-cardviewConditionGrid"));
            //divCardViewMainSecondChild.append("<label class='vis-pull-left'> " + VIS.Msg.getMsg("SelectRole") + "</label>");

            //divCardViewMainSecondChild.append("<div class='vis-cardviewmainsecondchildinner'><ul class='vis-ulrole'> </ul></div>");
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

            rootTextControl.append("<div style='display:none' class='vis-second-divHeader vis-pull-left' ><div> <label>" + VIS.Msg.getMsg("Window") + "</label></div><div><input type='text'  class='vis-txtwindowname'></div></div>");
            rootTextControl.append("<div style='display:none' class='vis-second-divHeader vis-pull-left' ><div><label>" + VIS.Msg.getMsg("Tab") + "</label></div><div><input type='text'  class='vis-txttabname'></div></div>");
            rootTextControl.append("<div class='vis-second-divHeader vis-pull-left' ><div><label>" + VIS.Msg.getMsg("GroupByField") + "</label></div><div class='vis-CardView'><select class='vis-cmbgroupfield'> </select></div></div>");
            rootCardViewUI.find(".vis-seconddiv").append(rootTextControl);
            // TextBox Control
            txtWindowName = rootTextControl.find(".vis-txtwindowname");
            txtTabName = rootTextControl.find(".vis-txttabname");
            cmbGroupField = rootTextControl.find(".vis-cmbgroupfield");
            // Column Fields
            rootGroupFieldsCombo = $("<div class='vis-third-header vis-pull-left' style='width:68%'></div>");
            rootGroupFieldsSeq = $("<div class='vis-third-header vis-pull-left' style='width:28%;margin: 0 15px 0 15px;'></div>");

            divLeftGroupFieldsCombo = $("<div class='vis-left-fields vis-pull-left'></div>");
            divRightGroupFieldsCombo = $("<div class='vis-left-fields vis-pull-left' ></div>");
            var divGroupFieldsSeqCombo = $("<div class='vis-left-fields vis-pull-left' ></div>");
            centerBtn = $("<div class='vis-cardviewbutton vis-pull-left' ><button class='vis-btnup'><i class='vis vis-arrow-up'></i></button><button  class='vis-btnright'><i class='vis vis-shiftright'></i></button></div>");
            centerBtn.append("<button   class='vis-btnleft'><i class='vis vis-shiftleft'></i></button><button  class='vis-btndown'><i class='vis vis-arrow-down'></i></button>");
            divChildLeftLable = $("<div style='width:100%'><label>" + VIS.Msg.getMsg("SelectField") + "</label></div>");
            divChildRightLable = $("<div style='width:100%'><label>" + VIS.Msg.getMsg("IncludedField") + "</label></div>");
            var divChildGroupSeqLable = $("<div style='width:100%'><label>" + VIS.Msg.getMsg("GroupSeq") + "</label></div>");
            divChildLeftColumns = $("<div class='vis-cv-filedslist-wrap'></div>");
            var divChildSeqColumns = $("<div class='vis-cv-filedslist-wrap'></div>");
            divChildRightColumns = $("<div class='vis-cv-filedslist-wrap'></div>");
            ulLeftColumns = $("<ul  class='vis-windowfield'></ul>");
            ulRightColumns = $("<ul  class='vis-cardviewcolumnfield'></ul>");
            ulGroupSeqColumns = $("<ul  class='vis-cardviewcolumnfield1'></ul>");
            divChildLeftColumns.append(ulLeftColumns);
            divChildRightColumns.append(ulRightColumns);
            divLeftGroupFieldsCombo.append(divChildLeftLable);
            divLeftGroupFieldsCombo.append(divChildLeftColumns);
            divRightGroupFieldsCombo.append(divChildRightLable);
            divRightGroupFieldsCombo.append(divChildRightColumns);
            rootGroupFieldsCombo.append(divLeftGroupFieldsCombo);
            rootGroupFieldsCombo.append(centerBtn);
            rootGroupFieldsCombo.append(divRightGroupFieldsCombo);
            divChildSeqColumns.append(ulGroupSeqColumns);
            rootGroupFieldsSeq.append(divGroupFieldsSeqCombo);
            divGroupFieldsSeqCombo.append(divChildGroupSeqLable);
            divGroupFieldsSeqCombo.append(divChildSeqColumns);

            GroupShifBtn = $('<div class="vis-cardviewbutton vis-pull-left" style="margin-top:25%;"><button class="vis-Grpbtnup"><i class="vis vis-arrow-up"></i></button><button class="vis-btnGrpdown"><i class="vis vis-arrow-down"></i></button></div>');
            rootGroupFieldsSeq.append(GroupShifBtn);
            rootCardViewUI.find(".vis-thirddiv").append(rootGroupFieldsCombo).append(rootGroupFieldsSeq);
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
            btnApply = rootCardViewUI.find(".vis-btnCardApply");

            btnGrpDown = rootCardViewUI.find(".vis-btnGrpdown");
            btnGrpUp = rootCardViewUI.find(".vis-Grpbtnup");

            btnCardViewCancle = rootCardViewUI.find(".vis-btnCardViewCancle");
            btnDelete = rootCardViewUI.find(".vis-btnDelete");
            cmbColumn = rootCardViewUI.find(".vis-cmbcolumn");
            divValue1 = rootCardViewUI.find(".vis-cvd-valcontainer");
            drpOp = rootCardViewUI.find(".vis-cmboperator");
            btnSave = rootCardViewUI.find(".vis-btnsave");
            cvTable = rootCardViewUI.find(".vis-cv-rowtable");
            ctrColor = rootCardViewUI.find(".vis-cmbcolor");
            isGradient = rootCardViewUI.find(".vis-gradient");


            isBusyRoot.css({
                "display": "none"
            });

            disableView();
            Events();

        };

        function disableView() {
            rootCardViewUI.find("*").attr("disabled", "disabled");
            rootCardViewUI.find(".vis-firstdiv *").removeAttr("disabled");
            rootCardViewUI.find(".vis-btnDelete").removeAttr("disabled");
            rootCardViewUI.find(".vis-cardviewbtn .vis-cdv-customokcancle *").removeAttr("disabled");
        };

        function IsBusy(isBusy) {
            if (isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "block" });
            }
            if (!isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "none" });
            }
        };


        function addOrderByClauseFromDB(OrderByClause) {
            clearOrderByClause();
            if (OrderByClause && OrderByClause.length > 0) {
                sortOrderArray = OrderByClause.split(",");
                for (var m = 0; m < sortOrderArray.length; m++) {
                    var val = sortOrderArray[m].split(' ');
                    var f = mTab.getField(val[0]);
                    addOrderByClauseItems(f.getHeader(), val[0], val[1]);
                }
            }
        };

        function clearOrderByClause() {
            sortList.empty();
            sortOrderArray = [];
            lastSortOrderArray = [];
        };


        function addOrderByClauseItems(selectedColtext, val, isAsc) {
            var item = '<div class="vis-sortListItem">'
                + '<p>' + selectedColtext + '</p>'
                + '<div class="vis-sortListIcons">'
                + '<span class="vis-sortAsc">';
            if (isAsc == "ASC")
                item += '<i class="fa fa-sort-amount-asc"></i>';
            else
                item += '<i class="fa fa-sort-amount-asc" style="transform: rotate(180deg);padding-top:1px"></i>';
            item += '</span>'
                + '<span class="vis-sortIcon vis-sortListItemClose" data-text="' + val + ' ' + isAsc + '">'
                + '<i class="fa fa-close"></i>'
                + '</span>'
                + '</div>'
                + '</div>';
            // $divHeadderLay.append('<label>' + selectedColtext + '(' + isAsc + ')</label>');
            sortList.append(item);
        };

        //function EnableDisableSortList() {
        //    if (sortOrderArray && sortOrderArray.length < 3) {
        //        cmbOrderClause
        //    }
        //};

        var AddCVConditionControl = function (root) {

            backgroundType = $('<div style="width:100%"> <input class="vis-firstcolor vis-pull-left" type="color" style="width: 20%;"><input class="vis-firstPer vis-pull-left" min="0" max="100" value="0" type="number" style="width: 20%;"><input class="vis-Lastcolor vis-pull-left" type="color" style="width: 20%;"><input class="vis-LastPer vis-pull-left" type="number" min="0" max="100" value="100" style="width: 20%;">'
                + '<select class="vis-GrdType" style = "width: 20%;">'
                + '<option value="to bottom">↓</option>'
                + '<option value="to top">↑</option>'
                + '<option value="to right">→</option>'
                + '<option value="to left">←</option>'
                + '<option value="to top left">↖</option>'
                + '<option value="to top right">↗</option>'
                + '<option value="to bottom left">↙</option>'
                + '<option value="to bottom right">↘</option>'
                + '</select></div>');

            var pDiv = $('<div class="vis-cv-condctrl-inner">');
            var divCVConditionCmbColor = $("<div class='vis-divcvc-cmbcolor'><div style='width:100%'><lable><input type='checkbox' class='vis-gradient' />" + VIS.Msg.getMsg("BGColor") + " </lable></div> <div class='vis-backgroundType' style='width:100%'> <input class='vis-cmbcolor' type='color' /></div></div>");
            var divCVConditionCmbColumnColor = $("<div class='vis-divcvc-cmbcolumn'><div style='width:100%'><lable>" + VIS.Msg.getMsg("Column") + "</lable></div> <div><select class='vis-cmbcolumn'></select></div></div>");
            var divCVConditionCmbOperator = $("<div class='vis-divcvc-cmboperator'><div style='width:100%'><lable>" + VIS.Msg.getMsg("Operator") + "</lable></div>  <div><select class='vis-cmboperator'></select> </div></div>");
            var divCVConditionQueryValue = $("<div class='vis-divcvc-queryvalue'><div style='width:100%'><lable>" + VIS.Msg.getMsg("QueryValue") + "</lable></div> <div class='vis-cvd-valcontainer' style='width:100%'></div></div>");
            var divCVConditionBtnSave = $("<div class='vis-divcvc-btnsave'><button class='vis-btnsave'><i class='vis vis-plus'></i></button></div>");
            pDiv.append(divCVConditionCmbColor);
            pDiv.append(divCVConditionCmbColumnColor);
            pDiv.append(divCVConditionCmbOperator);
            pDiv.append(divCVConditionQueryValue);
            root.append(pDiv);
            root.append(divCVConditionBtnSave);
        };

        var FillCardViewCombo = function (isDelete) {
            cmbCardView.children().remove();
            if (isDelete) {
                AD_CardView_ID = 0;
                AD_GroupField_ID = 0;
            }
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
                            if (isDelete && i == 0) {
                                AD_CardView_ID = cardViewInfo[0].CardViewID;
                                AD_GroupField_ID = cardViewInfo[0].AD_GroupField_ID;
                            }
                            cmbCardView.append("<Option idx=" + i + " is_shared=" + cardViewInfo[i].UserID + " ad_user_id=" + cardViewInfo[i].CreatedBy + " cardviewid=" + cardViewInfo[i].CardViewID + " groupSequence='" + cardViewInfo[i].groupSequence + "' excludedGroup='" + cardViewInfo[i].excludedGroup + "'  ad_field_id=" + cardViewInfo[i].AD_GroupField_ID + " isdefault=" + cardViewInfo[i].DefaultID + " ad_headerLayout_id=" + cardViewInfo[i].AD_HeaderLayout_ID + "> " + w2utils.encodeTags(cardViewInfo[i].CardViewName) + "</Option>");
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
                    var idx = cmbCardView.find(":selected").attr("idx");
                    //cardViewUserID = cmbCardView.find(":selected").attr("ad_user_id");
                    if (cardViewInfo != null && cardViewInfo.length > 0) {
                        chkPublic.attr("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                        isdefault.attr("checked", cardViewInfo[idx].DefaultID ? true : false);
                    }

                    //if (cardViewInfo && cardViewInfo[idx].disableWindowPageSize) {
                    //    chkdisabledWindowPage.prop("checked", true);
                    //} else {
                    //    chkdisabledWindowPage.prop("checked", false);
                    //}
                    if (cardViewInfo) {
                        if (cardViewInfo[idx].OrderByClause && cardViewInfo[idx].OrderByClause.length > 0) {
                            addOrderByClauseFromDB(cardViewInfo[idx].OrderByClause);
                        }
                        else {
                            clearOrderByClause();
                        }
                    }
                    if (cardViewInfo != null && cardViewInfo.length > 0) {
                        $vSearchHeaderLayout.setValue(cardViewInfo[idx].AD_HeaderLayout_ID);
                        cardViewUserID = cardViewInfo[idx].CreatedBy;
                    }
                    if (cardViewUserID == VIS.context.getAD_User_ID()) {
                        rootCardViewUI.find(".vis-btnDelete").css("display", "block");
                        rootCardViewUI.find(".vis-cvd-editbtn i").removeClass('vis-copy').addClass('vis-pencil');
                        rootCardViewUI.find(".vis-cvd-editbtn").attr("title", VIS.Msg.getMsg("EditRecord"));
                    } else {
                        rootCardViewUI.find(".vis-btnDelete").css("display", "none");
                        rootCardViewUI.find(".vis-cvd-editbtn i").removeClass('vis-pencil').addClass('vis-copy');
                        rootCardViewUI.find(".vis-cvd-editbtn").attr("title", VIS.Msg.getMsg("CopyCard"));
                    }

                    try {
                        if (!isDelete) {
                            //cardViewUserID = parseInt(cmbCardView.find(":selected").attr("ad_user_id"));
                            AD_User_ID = cardViewUserID;
                            orginalcardViewUserID = cardViewUserID;
                        }
                        else {
                            if (cardViewInfo != null && cardViewInfo.length > 0) {
                                orginalAD_CardView_ID = cardViewInfo[idx].CardViewID;
                                AD_GroupField_ID = cardViewInfo[idx].AD_GroupField_ID;
                            }
                            //cardViewUserID = parseInt(cmbCardView.find(":selected").attr("ad_user_id"));
                            AD_CardView_ID = orginalAD_CardView_ID;
                            if (gc.isCardRow)
                                cardView.getCardViewData(mTab, AD_CardView_ID);
                        }

                    }
                    catch (e) {
                    }

                    // FillCVConditonTable(LstCardViewCondition);
                    //  txtCardViewName.val(cmbCardView.find(":selected").data('name'));
                    if (VIS.MRole.isAdministrator) {
                        // FillUserList(cmbUser);
                        //FillRoleList(ulRole);
                    }
                    //if (AD_User_ID > 0) {
                    //    ulRole.attr('disabled', 'disabled');
                    //}

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
                cmbOrderClause.val(-1);
                sortOrderArray = [];
                lastSortOrderArray = [];
                LastCVCondition = [];
                //AD_CardView_ID = parseInt(sel.attr("cardviewid"));
                //cardViewUserID = parseInt(sel.attr("ad_user_id"));
                //$vSearchHeaderLayout.setValue(sel.attr("ad_headerLayout_id"));
                //isdefault.attr("checked", sel.attr("isdefault") == 'true' ? true : false);
                //chkPublic.attr("checked", parseInt(sel.attr("is_shared")) > 0 ? false : true);
                var idx = sel.attr("idx");
                if (cardViewInfo) {
                    AD_CardView_ID = cardViewInfo[idx].CardViewID;
                    cardViewUserID = cardViewInfo[idx].CreatedBy;
                    $vSearchHeaderLayout.setValue(cardViewInfo[idx].AD_HeaderLayout_ID);
                    isdefault.prop("checked", cardViewInfo[idx].DefaultID ? true : false);
                    chkPublic.prop("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                }

                //if (cardViewInfo && cardViewInfo[idx].disableWindowPageSize) {
                //    chkdisabledWindowPage.prop("checked", true);
                //} else {
                //    chkdisabledWindowPage.prop("checked", false);
                //}
                if (cardViewInfo && cardViewInfo[idx].OrderByClause && cardViewInfo[idx].OrderByClause.length) {
                    addOrderByClauseFromDB(cardViewInfo[idx].OrderByClause);
                }
                else {
                    clearOrderByClause();
                }

                if (cardViewUserID == VIS.context.getAD_User_ID()) {
                    rootCardViewUI.find(".vis-btnDelete").css("display", "block");
                    rootCardViewUI.find(".vis-cvd-editbtn i").removeClass('vis-copy').addClass('vis-pencil');
                    rootCardViewUI.find(".vis-cvd-editbtn").attr("title", VIS.Msg.getMsg("EditRecord"));
                } else {
                    rootCardViewUI.find(".vis-btnDelete").css("display", "none");
                    rootCardViewUI.find(".vis-cvd-editbtn i").removeClass('vis-pencil').addClass('vis-copy');
                    rootCardViewUI.find(".vis-cvd-editbtn").attr("title", VIS.Msg.getMsg("CopyCard"));
                }
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

            if (!isNewRecord || !isEdit) {
                ulGroupSeqColumns.find('input').prop("disabled", true);
                ulCardViewColumnField.find('select').prop("disabled", true);
            }
            //FillRoleList(ulRole);
        }

        var FillTextControl = function () {
            txtWindowName.val(WindowName);
            txtTabName.val(tabName);
            txtWindowName.prop("disabled", "disabled");
            txtTabName.prop("disabled", "disabled");
        };

        var FillGroupFields = function () {
            if (cmbGroupField != null) { cmbGroupField.children().remove(); }
            var fields = null;
            var dbResult = null;
            lovcardList = {};
            if (mTab != null && mTab.getFields().length > 0) {
                cmbGroupField.append("<Option FieldID=" + -1 + "></Option>");
                tabField = mTab.getFields();
                for (var i = 0; i < tabField.length; i++) {
                    var c = tabField[i].getColumnName().toLower();
                    if (c == "created" || c == "createdby" || c == "updated" || c == "updatedby") {
                        continue;
                    }

                    if ((VIS.DisplayType.IsLookup(tabField[i].getDisplayType()) && tabField[i].getLookup() && tabField[i].getLookup().getIsValidated() && tabField[i].getIsDisplayed()) || tabField[i].getDisplayType() == VIS.DisplayType.YesNo) {
                        cmbGroupField.append("<Option FieldID=" + tabField[i].getAD_Field_ID() + "> " + tabField[i].getHeader() + "</Option>");
                        if (tabField[i].getDisplayType() == VIS.DisplayType.List || tabField[i].getDisplayType() == VIS.DisplayType.TableDir || tabField[i].getDisplayType() == VIS.DisplayType.Table || tabField[i].getDisplayType() == VIS.DisplayType.Search) {
                            if (tabField[i].lookup && tabField[i].lookup.getData()) {
                                lovcardList[tabField[i].getAD_Field_ID()] = tabField[i].lookup.getData();
                            }
                        }
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
            FillforGroupSeq(AD_GroupField_ID);

            if (cmbGroupField != null) {
                cmbGroupField.on("change", function () {
                    AD_GroupField_ID = parseInt($(this).find(":selected").attr("fieldid"));
                    FillforGroupSeq(AD_GroupField_ID);
                });
            }

        };


        var FillColumnInclude = function (isReload, isShowAllColumn) {
            //if (!isReload) {
            if (ulLeftColumns != null) {
                ulLeftColumns.children().remove();
            }
            fields = null;
            dbResult = null;

            tabField = mTab.getFields();
            tabField.sort(function (a, b) {
                var a1 = a.getHeader().toLower(), b1 = b.getHeader().toLower();
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });
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

                    if (!tabField[i].getIsDisplayed()) {
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
                    ulLeftColumns.append("<li index=" + i + " FieldID=" + tabField[i].getAD_Field_ID() + "> <span>" + tabField[i].getHeader() + "</span></li>");
                }
            }

        };

        var FillforGroupSeq = function (fieldID) {
            ulGroupSeqColumns.html('');
            ulGroupSeqColumns.parent().removeAttr('style');
            if (lovcardList[fieldID]) {
                for (var i = 0, ln = lovcardList[fieldID]; i < ln.length; i++) {
                    if (ln[i].Key.toString().length > 0 && ln[i].Name.toString().length > 0) {
                        ulGroupSeqColumns.append('<li key="' + ln[i].Key + '"><input type="checkbox"/>' + ln[i].Name + '</li>');
                    }
                };
                ulGroupSeqColumns.find('input').prop('checked', true)
                var idx = cmbCardView.find(":selected").attr('idx');
                var seq = cardViewInfo[idx].groupSequence;
                var excGrp = cardViewInfo[idx].excludedGroup;
                if (seq) {
                    seq = seq.split(",");
                    excGrp = excGrp.split(",");
                    for (var j = 0; j < seq.length; j++) {
                        var item = ulGroupSeqColumns.find("[key='" + seq[j] + "']");
                        if (excGrp.lastIndexOf(seq[j]) != -1) {
                            item.find('input').prop('checked', false);
                        }

                        var before = ulGroupSeqColumns.find("li").eq(j);
                        item.insertBefore(before);
                        //if (item) {
                        //    var temp = item;
                        //    item.remove();
                        //    ulGroupSeqColumns.find('li').eq(j).before(temp);
                        //}
                    }
                }
            } else {
                ulGroupSeqColumns.parent().css("background-color", "rgba(var(--v-c-on-secondary), 0.04)");
                ulGroupSeqColumns.append('<li style="padding-top:40%;text-align:center" key="">' + VIS.Msg.getMsg("OnlyForLOV") + '</li>');
            }

        }


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
                    lblDefault.css({ "display": "block" }).attr('checked', false);
                    $vSearchHeaderLayout.setValue(null);
                    rootCardViewUI.find(".vis-cardviewchild");
                    //  chkdisabledWindowPage.prop("checked", false);
                    chkPublic.prop("checked", false);
                    isNewRecord = true;
                    LstRoleID = [];
                    //UnSelectRoleUl();
                    cmbGroupField.find(":selected").removeAttr("selected");
                    cmbGroupField.find("[FieldID='" + -1 + "']").attr("selected", "selected");
                    FillColumnInclude(false, true);
                    ulRightColumns.children().remove();
                    //ulRole.find('input[type=checkbox]').attr('disabled', false);
                    LastCVCondition = cardviewCondition;
                    cardviewCondition = [];
                    AddRow(cardviewCondition);
                    btnDelete.hide();
                    lastSortOrderArray = sortOrderArray;
                    sortOrderArray = [];
                    clearOrderByClause();

                    ulGroupSeqColumns.html('');
                    ulGroupSeqColumns.parent().removeAttr('style');
                    ulGroupSeqColumns.parent().css("background-color", "rgba(--v-c-on-secondary), 0.04)");
                    ulGroupSeqColumns.append('<li style="padding-top:40%;text-align:center" key="">' + VIS.Msg.getMsg("OnlyForLOV") + '</li>');

                    rootCardViewUI.find("*").removeAttr("disabled");
                });
            }
            if (btnCancle != null) {
                btnCancle.on("click", function (e) {
                    e.stopPropagation();
                    isEdit = false;
                    cmbOrderClause.val(-1);
                    clearOrderByClause();
                    btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                    btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                    toggleNewRecord();
                    FillColumnInclude(true, false);
                    FillGroupFields();
                    //FillRoleList(ulRole);
                    if (!isNewRecord)
                        AddRow(LastCVCondition);
                    isNewRecord = false;
                    var idx = cmbCardView.find(":selected").attr('idx');
                    //  chkdisabledWindowPage.prop('checked', cardViewInfo[idx].disableWindowPageSize);
                    chkPublic.prop("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                    isdefault.prop("checked", cardViewInfo[idx].DefaultID ? true : false);
                    $vSearchHeaderLayout.setValue(cardViewInfo[idx].AD_HeaderLayout_ID);
                    rootCardViewUI.find("*").attr("disabled", "disabled");
                    rootCardViewUI.find(".vis-firstdiv *").removeAttr("disabled");
                    rootCardViewUI.find(".vis-cardviewbtn .vis-cdv-customokcancle *").removeAttr("disabled");
                    rootCardViewUI.find(".vis-btnDelete").removeAttr("disabled");
                    if (cardViewInfo[idx].CreatedBy == VIS.context.getAD_User_ID()) {
                        rootCardViewUI.find(".vis-btnDelete").css("display", "block");
                    } else {
                        rootCardViewUI.find(".vis-btnDelete").css("display", "none");
                    }
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
                    rootCardViewUI.find(".vis-cardviewchild");
                    LastCVCondition = cardviewCondition;
                    txtCardViewName.val(cmbCardView.find(":selected").text().trim());
                    if (cardViewInfo != null && cardViewInfo.length > 0) {
                        lblDefault.css({ "display": "none" }).attr('checked', false);
                    } else {
                        lblDefault.css({ "display": "block" }).attr('checked', false);
                    }
                    cvTable.find('.vis-delete').css({ "cursor": "pointer" });
                    rootCardViewUI.find("*").removeAttr("disabled");
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
                    if (isNewRecord || isEdit) {
                        if (lastSelectWindowFieldItem != null) {
                            //lastSelectWindowFieldItem.css(vis-btnsave"background-color", "white");
                            lastSelectWindowFieldItem.removeClass('vis-cv-liselected');
                        }
                        //$(this).css("background-color", "#1aa0ed");
                        $(this).addClass('vis-cv-liselected');
                        lastSelectWindowFieldItem = $(this);
                        //windowFieldindex = $(this).attr("index");
                        windowFieldindex = $(this).index();
                        WindowAD_Field_ID = $(this).attr("FieldID");
                        FieldName = $(this).find('span').text();
                    }
                });
            }
            if (ulCardViewColumnField != null) {
                ulCardViewColumnField.on("click", "li", function () {
                    if (isNewRecord || isEdit) {
                        if (lastSelectCardViewColumnFieldItem != null) {
                            //lastSelectCardViewColumnFieldItem.css("background-color", "white");
                            lastSelectCardViewColumnFieldItem.removeClass('vis-cv-liselected');

                        }
                        //$(this).css("background-color", "#1aa0ed");
                        $(this).addClass('vis-cv-liselected');
                        lastSelectCardViewColumnFieldItem = $(this);
                        //cardViewColumnFieldindex = $(this).attr("index");
                        cardViewColumnFieldindex = $(this).index();
                        cardViewColumnAD_Field_ID = $(this).attr("FieldID");
                        AD_CardViewColumn_ID = $(this).attr("cardviewcolumnid");
                        FieldName = $(this).find('span').text();
                        seqNo = $(this).attr("seqno");
                    }
                });
            }
            if (ulGroupSeqColumns != null) {
                ulGroupSeqColumns.on("click", "li", function () {
                    if (isNewRecord || isEdit) {
                        ulGroupSeqColumns.find('.vis-cv-liselected').removeClass('vis-cv-liselected');
                        $(this).addClass('vis-cv-liselected');
                    }
                });
            }
            if (btnRight != null) {
                btnRight.on("click", function (e) {
                    if (parseInt(WindowAD_Field_ID) <= 0) {
                        return;
                    }
                    ulCardViewColumnField.append("<li seqno=" + 0 + " index=" + count + "  FieldID=" + WindowAD_Field_ID + "><span> " + FieldName + "</span></li>");
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
                    ulWindowField.append("<li seqno=" + 0 + " index=" + lastIndex + "  FieldID=" + cardViewColumnAD_Field_ID + "> <span>" + FieldName + "<span></li>");
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
            if (btnGrpUp != null) {
                btnGrpUp.on("click", function (e) {
                    var item = ulGroupSeqColumns.find('.vis-cv-liselected');
                    var before = item.prev();
                    item.insertBefore(before);
                    e.stopPropagation();
                    e.preventDefault();
                });
            }
            if (btnGrpDown != null) {
                btnGrpDown.on("click", function (e) {
                    var item = ulGroupSeqColumns.find('.vis-cv-liselected');
                    var after = item.next();
                    item.insertAfter(after);
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
                    if (isNewRecord || isEdit) {
                        closeDialog = false;
                        SaveChanges(e);
                    }
                });
            }

            if (btnApply != null) {
                btnApply.on("click", function (e) {
                    closeDialog = true;
                    SaveChanges(e);
                });
            }

            if (btnCardViewCancle != null) {
                btnCardViewCancle.on("click", function (e) {
                    ch.close();
                });
            }

            if (btnDelete != null) {
                btnDelete.on("click", function (e) {
                    VIS.ADialog.confirm("SureWantToDelete", true, "", VIS.Msg.getMsg("Confirm"), function (result) {
                        if (result) {
                            isFirstLoad = false;
                            DeleteCardView();

                            FillCardViewCombo(true);
                            if (cardViewInfo != null && cardViewInfo.length > 0) {
                                FillTextControl();
                                FillGroupFields();
                                FillColumnInclude(true, false);
                                toggleNewRecord();
                                if (cardViewUserID == VIS.context.getAD_User_ID()) {
                                    AD_User_ID = 0;
                                    cardViewUserID = 0;
                                    btnEdit.find("i").removeClass('vis-copy').addClass('vis-pencil');
                                }
                                else {
                                    btnEdit.find("i").removeClass('vis-pencil').addClass('vis-copy');
                                }
                                disableView();
                            }

                        }
                    });
                    //var diaDel = confirm(VIS.Msg.getMsg("SureWantToDelete"));

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
                    if ($(isGradient).is(':checked')) {
                        var firstColor = rootCardViewUI.find('.vis-firstcolor');
                        var LirstColor = rootCardViewUI.find('.vis-Lastcolor');
                        if (!Modernizr.inputtypes.color) {
                            firstColor = firstColor.spectrum('get');
                            LirstColor = LirstColor.spectrum('get');
                        }
                        else {
                            firstColor = firstColor.val();
                            LirstColor = LirstColor.val();
                        }
                        var firstPer = rootCardViewUI.find('.vis-firstPer').val() || 0;
                        var LastPer = rootCardViewUI.find('.vis-LastPer').val() || 0;
                        var gredType = rootCardViewUI.find('.vis-GrdType option:selected').val();
                        colorValue = 'linear-gradient(' + gredType + ', ' + firstColor + ' ' + firstPer + '%, ' + LirstColor + ' ' + LastPer + '%)';
                    } else {

                        if (!Modernizr.inputtypes.color) {
                            colorValue = ctrColor.spectrum('get');
                        }
                        else {
                            colorValue = ctrColor.val();
                        }
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
                cvTable.on("click", "tr td i", function () {
                    if (isEdit || isNewRecord) {
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

            if (isGradient != null) {
                isGradient.on("click", function () {
                    if ($(this).is(':checked')) {
                        rootCardViewUI.find('.vis-backgroundType').html('').append(backgroundType);
                        rootCardViewUI.find('.vis-divcvc-cmboperator').css({ 'width': '10%' });
                        rootCardViewUI.find('.vis-divcvc-cmbcolor').css({ 'width': '40%' });

                    } else {
                        rootCardViewUI.find('.vis-divcvc-cmboperator').css({ 'width': '25%' });
                        rootCardViewUI.find('.vis-divcvc-cmbcolor').css({ 'width': '25%' });
                        ctrColor = $("<input class='vis-cmbcolor' type='color' />");
                        rootCardViewUI.find('.vis-backgroundType').html("").append(ctrColor);
                    }
                })
            }
        };

        function toggleNewRecord() {
            ddlCardView.css({ "display": "block" });
            cmbCardView.css({ "display": "block" });
            lableCardViewName.css({ "display": "none" });
            txtCardViewName.css({ "display": "none" });
            btnNew.css({ "display": "block" });
            btnCancle.css({ "display": "none" });
            txtCardViewName.val("");
            btnEdit.css({ "display": "block" });
            lblDefault.css({ "display": "none" });
            rootCardViewUI.find(".vis-cardviewchild");

        }

        function SaveChanges(e) {
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

                var len = ulCardViewColumnField.children().length;
                if (len.length <= 0)
                    return false;

                SaveCardViewColumn(cardViewColArray);
                e.stopPropagation();
                e.preventDefault();
            }, 50);
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
                //ulRoot.find("[fieldid='" + ad_Field_ID + "']").css("background-color", "#1aa0ed");
                ulRoot.find("[fieldid='" + ad_Field_ID + "']").addClass('vis-cv-liselected');
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
                            cardViewUserID = CVColumns[0].CreatedBy;
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
                                // var ddl = $(sortDDL).val(CVColumns[i].sort);
                                ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + CVColumns[i].AD_Field_ID + "> <span>" + CVColumns[i].FieldName + "</span></li>");
                                ulRoot.find('li:eq(' + i + ') select').val(CVColumns[i].sort);
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
                        //if (lastSortOrderArray != null && lastSortOrderArray.length > 0) {
                        //    addOrderByClauseFromDB(lastSortOrderArray.join(','));
                        //}
                        //else {
                        addOrderByClauseFromDB(CVColumns[0].OrderByClause);
                        //}
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
            var idx = cmbCardView.find(":selected").attr('idx');
            if (cardViewInfo != null) {
                cardViewUserID = parseInt(cardViewInfo[idx].CreatedBy);
            }
            if (VIS.context.getAD_User_ID() == cardViewUserID && !isNewRecord) {
                AD_User_ID = VIS.context.getAD_User_ID();
            } else if (VIS.context.getAD_User_ID() != cardViewUserID && !isNewRecord && isEdit) {
                isNewRecord = true;
                AD_User_ID = VIS.context.getAD_User_ID();
                if (!VIS.MRole.isAdministrator) {
                    chkPublic.prop("checked", false);
                }
            } else {
                //  isNewRecord = true;
                AD_User_ID = VIS.context.getAD_User_ID();
            }

            if (isNewRecord && cardViewInfo != null) {
                for (var a = 0; a < cardViewInfo.length; a++) {
                    if (cardViewInfo[a].CardViewName.trim() == txtCardViewName.val().trim()) {
                        VIS.ADialog.error("cardAlreadyExist", true, "");
                        IsBusy(false);
                        return false;
                    }
                }
            }

            //if (cardViewName.length < 1 && !isEdit) {
            //    cardViewName = cmbCardView.find(":selected").text();
            //}
            //else if (isEdit) {
            cardViewName = txtCardViewName.val().trim();
            //}

            if (isEdit || isNewRecord) {
                SaveCardInfoFinal();
            }
            else {
                IsBusy(false);
                if (closeDialog) {
                    ch.close();
                    if (gc.isCardRow)
                        //cardView.requeryData();
                        cardView.getCardViewData(mTab, AD_CardView_ID);
                }
                else {
                    btnCancle.trigger("click");
                }
            }
        };

        var SaveCardInfoFinal = function () {
            var len = ulCardViewColumnField.children().length;
            cardViewColArray = [];
            var includeCols = [];
            for (var i = 0; i < len; i++) {
                var f = {};
                f.AD_Field_ID = ulCardViewColumnField.children().eq(i).attr("fieldid");
                f.CardViewID = AD_CardView_ID;
                f.sort = ulCardViewColumnField.children().eq(i).find('option:selected').val()
                cardViewColArray.push(f);
                includeCols.push(parseInt(f.AD_Field_ID));
            }

            var grpSeq = "";
            var skipGrp = "";
            $.each(ulGroupSeqColumns.find('li'), function () {
                grpSeq += $(this).attr('key') + ",";
                if (!$(this).find('input').is(':checked')) {
                    skipGrp += $(this).attr('key') + ",";
                }
            });
            var selIdx = cmbCardView.find(":selected").attr('idx');
            grpSeq = grpSeq.replace(/,\s*$/, "");
            skipGrp = skipGrp.replace(/,\s*$/, "");

            var sortOrder = sortOrderArray.join(',');
            cardViewArray = [];
            var cardID = AD_CardView_ID;
            if (isNewRecord)
                cardID = 0;
            cardViewArray.push({ AD_Window_ID: AD_Window_ID, AD_Tab_ID: AD_Tab_ID, UserID: AD_User_ID, AD_GroupField_ID: cmbGroupField.find(":selected").attr("fieldid"), isNewRecord: isNewRecord, CardViewName: cardViewName, CardViewID: cardID, IsDefault: isdefault.is(":checked"), AD_HeaderLayout_ID: $vSearchHeaderLayout.getValue(), isPublic: chkPublic.is(":checked"), groupSequence: grpSeq });
            var url = VIS.Application.contextUrl + "CardView/SaveCardViewColumns";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ 'lstCardView': cardViewArray, 'lstCardViewColumns': cardViewColArray, /*'LstRoleID': LstRoleID,*/ 'lstCardViewCondition': strConditionArray, 'excludeGrp': skipGrp, 'orderByClause': sortOrder }),
                success: function (data) {
                    var result = JSON.parse(data);
                    AD_CardView_ID = result;
                    orginalAD_CardView_ID = AD_CardView_ID;
                    if (closeDialog) {
                        if (gc.isCardRow)
                            cardView.getCardViewData(mTab, AD_CardView_ID);
                        ch.close();
                    }
                    else {
                        if (isNewRecord) {
                            var idx = 0;
                            if (cmbCardView.find('option').length > 0)
                                idx = cmbCardView.find('option').length;
                            else
                                idx = 0;
                            if (!cardViewInfo) {
                                cardViewInfo = [];
                            }
                            //<Option idx="+i+" is_shared=" + cardViewInfo[i].UserID + " ad_user_id=" + cardViewInfo[i].CreatedBy + " cardviewid=" + cardViewInfo[i].CardViewID + " groupSequence='" + cardViewInfo[i].groupSequence + "' excludedGroup='" + cardViewInfo[i].excludedGroup +"' ad_field_id=" + cardViewInfo[i].AD_GroupField_ID + " isdefault=" + cardViewInfo[i].DefaultID + " ad_headerLayout_id=" + cardViewInfo[i].AD_HeaderLayout_ID + "> " + w2utils.encodeTags(cardViewInfo[i].CardViewName) + "</Option>");
                            cardViewInfo.push({
                                'CardViewName': cardViewName, 'UserID': AD_User_ID, 'CreatedBy': VIS.context.getAD_User_ID(), 'CardViewID': AD_CardView_ID, 'groupSequence': grpSeq, 'excludedGroup': skipGrp, 'AD_GroupField_ID': cmbGroupField.find(":selected").attr("fieldid"), 'DefaultID': isdefault.is(":checked"), 'AD_HeaderLayout_ID': $vSearchHeaderLayout.getValue(), 'CardViewName': cardViewName, 'OrderByClause': sortOrder
                            });
                            cmbCardView.append("<Option idx=" + idx + " is_shared=" + AD_User_ID + " ad_user_id=" + VIS.context.getAD_User_ID() + " cardviewid=" + AD_CardView_ID + " groupSequence='" + grpSeq + "' excludedGroup='" + skipGrp + "'  ad_field_id=" + cmbGroupField.find(":selected").attr("fieldid") + " isdefault=" + isdefault.is(":checked") + " ad_headerLayout_id=" + $vSearchHeaderLayout.getValue() + "> " + w2utils.encodeTags(cardViewName) + "</Option>");
                            cmbCardView.find('[cardviewid="' + AD_CardView_ID + '"]').prop("selected", true).trigger("change");
                        }
                        else {
                            cardViewInfo[selIdx].groupSequence = grpSeq;
                            cardViewInfo[selIdx].excludedGroup = skipGrp;
                            cardViewInfo[selIdx].AD_GroupField_ID = cmbGroupField.find(":selected").attr("fieldid");
                            cardViewInfo[selIdx].AD_HeaderLayout_ID = $vSearchHeaderLayout.getValue();
                            cardViewInfo[selIdx].CardViewName = cardViewName;
                            cardViewInfo[selIdx].UserID = AD_User_ID;
                            cardViewInfo[selIdx].OrderByClause = sortOrder;
                            cmbCardView.find("[cardviewid='" + AD_CardView_ID + "']").text(cardViewName);
                        }
                        btnCancle.trigger("click");


                    }
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
            if (root != null) { root.children().remove(); }
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

            if (root != null) { root.children().remove(); }
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
            if (ch != null)
                ch.changeTitle(VIS.Msg.getMsg("Card"));
            //if (cardViewUserID > 0) {
            //    if (ch != null)
            //        ch.changeTitle(VIS.Msg.getMsg("Card") + "(" + VIS.Msg.getMsg("IsPrivate") + ")");
            //}
            //else {
            //    if (ch != null)
            //        ch.changeTitle(VIS.Msg.getMsg("Card"));
            //}
        };

        var CreateCVGrid = function (root) {
            var cvHeaderTable = $("<Table class='vis-cv-headertable'><tr class='vis-cv-TableHead'>" +
                "<th >" + VIS.Msg.getMsg("BGColorGrid") + "</th>" +
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
                        row.append("<td value='" + data[i].Color + "' style='width:" + col1Width + "px'><div class='vis-cvd-bgcolor' style='background:" + data[i].Color + ";cursor:pointer;color:transparent'>-</div></td>");
                        row.append("<td style='width:" + col2Width + "px' value=" + data[i].Condition[j].ColName + ">" + data[i].Condition[j].ColHeader + "</td>");
                        row.append("<td style='width:" + col3Width + "px' value=" + data[i].Condition[j].Operator + ">" + data[i].Condition[j].OperatorText + "</td>");
                        row.append("<td style='width:" + col4Width + "px' value=" + data[i].Condition[j].QueryValue + ">" + data[i].Condition[j].QueryText + "</td>");
                        row.append("<td style='width:" + col5Width + "px'><i class='vis vis-delete'></i></td>");
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

                if (VIS.DisplayType.IsNumeric(crtlObj.getDisplayType())) {
                    return 0;
                }
                // return control's value
                if (crtlObj.getValue() == '') {
                    return null;
                }
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
                crtlObj.setValue(null);
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
                //var retVal = {};
                //retVal.FieldGroupID = AD_GroupField_ID;

                var len = ulCardViewColumnField.children().length;
                if (len.length <= 0)
                    return false;

                SaveCardViewColumn(cardViewColArray);
                //if (!retVal.IncludedCols) {
                //    return false;
                ////}
                //retVal.Conditions = [];
                //retVal.AD_CardView_ID = AD_CardView_ID;
                //if (VIS.MRole.isAdministrator && AD_User_ID < 1 && orginalAD_CardView_ID > 0) {
                //    //retVal.FieldGroupID = orginalAD_GroupField_ID;
                //    //retVal.IncludedCols = orginalIncludedCols;
                //    retVal.Conditions = [];
                //    retVal.AD_CardView_ID = orginalAD_CardView_ID;
                //}
                //cardView.setCardViewData(retVal);
                //if (gc.isCardRow)
                //    cardView.getCardViewData(null, AD_CardView_ID);

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

    function cardCopyDialog() {
        var $root = $('<div style="padding-bottom:0px">');
        var txtDescription = $('<span style="display:block;margin-bottom:5px;">' + VIS.Msg.getMsg('NewCardInfo') + '</span>');
        var $txtName = $('<input style="margin-left:5px">');
        var $lblName = $('<label>' + VIS.Msg.getMsg('EnterName') + '</label>');
        $root.append(txtDescription).append($lblName).append($txtName);
        var self = this;
        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("CardName"));
            ch.setModal(true);
            ch.setContent($root);
            ch.show();
            ch.onOkClick = ok;
            ch.onCancelClick = cancel;
            ch.onClose = cancel;
        };

        this.getName = function () {
            return $txtName.val();
        };

        function ok() {
            if ($txtName.val() == null || $txtName.val() == "") {
                VIS.ADialog.info("FileNameMendatory");
                return false;
            }
            self.onClose();
            return true;
        };

        function cancel() {
            self.onClose();
            return true;
        };


        function dispose() {
            $txtName.remove();
            $txtName = null;
            txtDescription.remove();
            txtDescription = null;
            $lblName.remove();
            $lblName = null;
            $root.remove();
            $root = null;
            ch = null;
        };
    };

    VIS.CardCopyDialog = cardCopyDialog;

    //alert(VIS.CVDialog);

}(VIS, jQuery));