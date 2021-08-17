
; (function (VIS, $) {
    var addBPartnerDialog = null;
    var $addBPartnerRoot = null;
    var self = null;
    var txtLoc = null;
    var $selectBPRelation = null;
    var $selectBPLocation = null;
    var $selectBPGroup = null;
    var isload = false;
    VIS.BPartner = function BPartner(selfobject, userid, BPartnerID, bpgroupID, category, $isBusyImg, $isBusy) {


        if ($addBPartnerRoot != null) {
            $addBPartnerRoot.empty();
        }
        self = selfobject;
        Load(self, userid, BPartnerID, bpgroupID, category, $isBusyImg, $isBusy);

    };
    //WSP.BPartner = function BPartner(selfobject) {
    //    
    //    self = selfobject;
    //    Load(self, userid, BPartnerID, bpgroupID, category, $isBusyImg, $isBusy);

    //};

    function OnRelationChangeEvent(relationID, self) {

        var result = null;
        var bplocation = "";
        url = VIS.Application.contextUrl + "WSP/Contacts/GetBPLocation?WinNo=" + self.windowNo + "&bpartnerID=" + relationID;
        $.ajax({
            type: "GET",
            async: false,
            url: url,
            dataType: "json",

            success: function (data) {
                result = JSON.parse(data);
                for (var i = 0; i < result.lstBPLocation.length; i++) {
                    bplocation += "<option value='" + result.lstBPLocation[i].ID + "'>" + result.lstBPLocation[i].Name + "</option>";

                }
                $selectBPLocation.empty();
                $selectBPLocation.append(bplocation);
            }
        });

    };


    function OpenDialog(root, category, $selectBPRelation, self, $isBusyImg, $isBusy) {

        addBPartnerDialog = new VIS.ChildDialog();

        addBPartnerDialog.setContent(root);
        //addBPartnerDialog.setWidth(w);
        //addBPartnerDialog.setHeight(h);
        if (category == "Customer") {
            addBPartnerDialog.setTitle(VIS.Msg.getMsg("Customer"));
        }
        else if (category == "Employee") {
            addBPartnerDialog.setTitle(VIS.Msg.getMsg("Employee"));
        }
        else if (category == "Vendor") {
            addBPartnerDialog.setTitle(VIS.Msg.getMsg("Vendor"));
        }
        else if (category == "Prospect") {
            addBPartnerDialog.setTitle(VIS.Msg.getMsg("Prospect"));
        }
        addBPartnerDialog.setEnableResize(false);
        addBPartnerDialog.setModal(true);
        addBPartnerDialog.show();
        SaveBPartnerEvents($isBusyImg, category, $isBusy);
        $selectBPRelation.on("change", function () {

            OnRelationChangeEvent($selectBPRelation.val(), self);
        });
    };

    function SaveBPartnerEvents($isBusyImg, category, $isBusy) {
        addBPartnerDialog.onOkClick = function (e) {

            // addBPartnerDialog.dialog("open");
            var $IsBPartnerBusy = $("#divBPartner_" + self.windowNo).find("img");
            var h = $(window).height() * (50 / 100);
            var w = $(window).width() * (50 / 100);
            $isBusyImg.css("margin-top", h);
            $isBusyImg.css("margin-left", w);
            $isBusy.show();
            $IsBPartnerBusy.css("visibility", "visible");
            var txtName = $("#txtName_" + self.windowNo).val();
            var txtSearchKey = $("#txtSearchKey_" + self.windowNo).val();
            var ddlgreeting1 = $("#ddlGreeting1_" + self.windowNo).val();
            var txtName2 = $("#txtName2_" + self.windowNo).val();
            var ddlBPGroup = $("#ddlBPGroup_" + self.windowNo).val();
            var ddlBPRelation = $("#ddlBPRelation_" + self.windowNo).val();
            var ddlBPLocation = $("#ddlBPLoaction_" + self.windowNo).val();
            var txtContact = $("#txtContact_" + self.windowNo).val();
            var ddlgreeting = $("#ddlGreeting_" + self.windowNo).val();
            var ddlTitle = $("#txtTitle_" + self.windowNo).val();
            var ddlEmail = $("#txtEmail_" + self.windowNo).val();
            var ddlAddress = txtLoc.getValue();
            var txtPhone = $("#txtPhone_" + self.windowNo).val();
            var txtPhone2 = $("#txtPhone2_" + self.windowNo).val();
            var txtFax = $("#txtFax_" + self.windowNo).val();
            if (txtName.trim() == "" || txtSearchKey.trim() == "") {
                //alert(VIS.Msg.getMsg("NameMandatory"));
                VIS.ADialog.info("NameMandatory");
                $isBusy.hide();
                $IsBPartnerBusy.css("visibility", "collapse");

                return;
            }


            var BPtype = null;
            if (category == "Customer") {
                BPtype = VIS.Msg.getMsg("Customer");
            }
            else if (category == "Employee") {
                BPtype = VIS.Msg.getMsg("Employee");
            }
            else if (category == "Vendor") {
                BPtype = VIS.Msg.getMsg("Vendor");
            }
            else if (category == "Prospect") {
                BPtype = VIS.Msg.getMsg("Prospect");
            }
            url = VIS.Application.contextUrl + "WSP/Contacts/AddBPartnerInfo";
            if (ddlBPGroup == "0") {
                ddlBPGroup = undefined;
            }
            if (ddlBPRelation == "0") {
                ddlBPRelation = undefined;
            }
            if (ddlBPLocation == "0") {
                ddlBPLocation = undefined;
            }
            if (ddlgreeting == "0") {
                ddlgreeting = undefined;
            }
            if (ddlgreeting1 == "0") {
                ddlgreeting1 = undefined;
            }
            if (ddlAddress == null) {
                ddlAddress = undefined;
            }
            if (txtPhone == "") {
                txtPhone = undefined;
            }
            if (txtPhone2 == "") {
                txtPhone2 = undefined;
            }
            if (txtContact == "")
            { txtContact = undefined; }
            if (txtName == null)
            { txtName = undefined; }
            if (txtFax == "")
            { txtFax = undefined; }
            if (txtSearchKey == "")
            { txtSearchKey = undefined; }
            if (ddlTitle == "")
            { ddlTitle = undefined; }
            if (ddlEmail == "")
            { ddlEmail = undefined; }
            if (BPtype == "")
            { BPtype = undefined; }
            $.ajax({
                type: "Post",
                async: false,
                url: url,
                dataType: "json",
                data: {
                    searchKey: txtSearchKey,
                    name: txtName,
                    name2: txtName2,
                    greeting: ddlgreeting,
                    bpGroup: ddlBPGroup,
                    bpRelation: ddlBPRelation,
                    bpLocation: ddlBPLocation,
                    contact: txtContact,
                    greeting1: ddlgreeting1,
                    title: ddlTitle,
                    email: ddlEmail,
                    address: ddlAddress,
                    phoneNo: txtPhone,
                    phoneNo2: txtPhone2,
                    fax: txtFax,
                    windowNo: self.windowNo, BPtype: BPtype

                },
                success: function (data) {

                    $isBusy.hide();
                    $IsBPartnerBusy.css("visibility", "collapse");
                    // addBPartnerDialog.dialog("close");
                    alert(data);
                    $addBPartnerRoot.empty();

                }
            });
        }
    };

    function Load(self, userid, BPartnerID, bpgroupID, category, $isBusyImg, $isBusy) {

        var $bpartnerroot = null;

        // userID = $("#userID").val();
        var greeting = "";
        var bpgroup = "";
        var bprelation = "";
        var bplocation = "";
        var lookup = new VIS.MLocationLookup(VIS.context.ctx, self.windowNo);
        txtLoc = new VIS.Controls.VLocation("C_Location_ID", true, false, true, VIS.DisplayType.Location, lookup);
        var count = txtLoc.getBtnCount();
        $addBPartnerRoot = $("<div id='divBPartner_" + self.windowNo + "' style='width:auto;height:auto;margin-right:5px'> <img src='/ViennaAdvantageWeb1/Areas/VIS/Images/busy.gif' style=' margin-top: 120px; margin-left: 327px; z-index:9999;position:absolute;visibility:collapse'>");

        var url = VIS.Application.contextUrl + "WSP/Contacts/AddBusinessPartner?WinNo=" + self.windowNo + "&UserID=" + userid;
        $.ajax({
            type: "GET",
            async: false,
            url: url,
            dataType: "json",

            success: function (data) {
                var dd = JSON.parse(data);
                for (var i = 0; i < dd.lstGreeting.length; i++) {
                    greeting += "<option value='" + dd.lstGreeting[i].ID + "'>" + dd.lstGreeting[i].Name + "</option>";
                }
                for (var i = 0; i < dd.lstBPGroup.length; i++) {
                    bpgroup += "<option value='" + dd.lstBPGroup[i].ID + "'>" + dd.lstBPGroup[i].Name + "</option>";
                }
                for (var i = 0; i < dd.lstBPRelation.length; i++) {
                    bprelation += "<option value='" + dd.lstBPRelation[i].ID + "'>" + dd.lstBPRelation[i].Name + "</option>";
                }
                for (var i = 0; i < dd.lstBPLocation.length; i++) {
                    bplocation += "<option value='" + dd.lstBPLocation[i].ID + "'>" + dd.lstBPLocation[i].Name + "</option>";
                }
                $bpartnerroot = AddControls(greeting, bpgroup, bplocation, bprelation, BPartnerID, bpgroupID);
            }
        });
        $addBPartnerRoot.append($bpartnerroot);
        $addBPartnerRoot.load();
        OpenDialog($addBPartnerRoot, category, $selectBPRelation, self, $isBusyImg, $isBusy);

    };

    function AddControls(greeting, bpgroup, bplocation, bprelation, BPartnerID, bpgroupID) {
        var $bpartnerroot = $("<table id='tableBPartner_" + self.windowNo + "'><tr><td><div style='margin-top: 0px; float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='margin-top: 0px;width:100%' class='wsp-BPartner'><input id='txtSearchKey_" + self.windowNo + "' style='height: 25px;width:100%' type='text'></div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: right;margin-right: 5px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-top: 0px;margin-right:5px' class='wsp-BPartner'><select id='ddlGreeting1_" + self.windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:100%' class='wsp-BPartner'><input id='txtName_" + self.windowNo + "' style='height: 25px;width:100%' type='text'></div></td></tr><tr><td><div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name2") + "  </div></td><td style='width:300px'><div style='width:100%' class='wsp-BPartner'><input id='txtName2_" + self.windowNo + "' style='height: 25px;width:100%' type='text'></div></td><td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px' class='wsp-BPartner'><select id='ddlBPGroup_" + self.windowNo + "' style='width: 100%'>" + bpgroup + "</select>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "<tr>"
  + "<td>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPRelation") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div style='width:100%' class='wsp-BPartner'>"
      + "<select style='width:100%' id='ddlBPRelation_" + self.windowNo + "' >"

          + bprelation
      + "</select>"
  + "</div>"
  + "</td> <td style='width:50px'></td>"
  + "<td style='margin-left:10px'>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPLoaction") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div  class='wsp-BPartner' style='margin-right: 5px'>"
      + "<select id='ddlBPLoaction_" + self.windowNo + "' style='width:100%'>"
      + "</select>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "<tr>"
  + "<td>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Contact") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div style='width:100%' class='wsp-BPartner'>"
      + "<input id='txtContact_" + self.windowNo + "' style='height: 25px;width:100%' type='text'>"
  + "</div>"
  + "</td> <td style='width:50px'></td>"
  + "<td style='margin-left:10px'>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div  class='wsp-BPartner' style='margin-right: 5px'>"
      + "<select id='ddlGreeting_" + self.windowNo + "' style='width: 100%'>"
        + greeting
      + "</select>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "<tr>"
  + "<td>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Title") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div style='width:100%' class='wsp-BPartner'>"
      + "<input id='txtTitle_" + self.windowNo + "' style='height: 25px;width:100%' type='text'>"
  + "</div>"
  + "</td> <td style='width:50px'></td>"
  + "<td style='margin-left:10px'>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("EmailAddress") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div  class='wsp-BPartner' style='margin-right: 5px'>"
      + "<input id='txtEmail_" + self.windowNo + "' style='height: 25px;width:100%' type='text'>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "<tr>"
  + "<td>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Address") + "  </div>"
  + "</td>"
  + "<td style='width:300px'><div style='width:100%' id='divAddress_" + self.windowNo + "' class='wsp-BPartner' ></div> </td> <td style='width:50px'></td>"
  + "<td style='margin-left:10px'>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Phone") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div  class='wsp-BPartner' style='margin-right: 5px'>"
      + "<input id='txtPhone_" + self.windowNo + "' style='height: 25px;width:100%' type='text'>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "<tr>"
  + "<td>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("2ndPhone") + "  </div>"
  + "</td>"
  + "<td style='width:300px'>"
  + "<div class='wsp-BPartner' style='width:100%;'>"
      + "<input id='txtPhone2_" + self.windowNo + "' style='height: 25px;width:100%' type='text'>"
  + "</div>"
  + "</td> <td style='width:50px'></td>"
  + "<td style='margin-left:10px'>"
  + "<div style='float: right' class='wsp-BPartner'>" + VIS.Msg.getMsg("Fax") + "  </div>"
  + "</td>"
  + "<td style='width:300px' >"
  + "<div style='margin-bottom:7px;margin-right: 5px' class='wsp-BPartner' >"
      + "<input id='txtFax_" + self.windowNo + "' style='height: 25px;width:100%;' type='text'>"
  + "</div>"
  + "</td>"
  + "</tr>"
  + "</table>");
        var $div = $bpartnerroot.find("#divAddress_" + self.windowNo);
        $bpartnerroot.find("#ddlBPRelation_" + self.windowNo).val(BPartnerID);
        $selectBPRelation = $bpartnerroot.find("#ddlBPRelation_" + self.windowNo);
        $selectBPLocation = $bpartnerroot.find("#ddlBPLoaction_" + self.windowNo);
        $selectBPGroup = $bpartnerroot.find("#ddlBPGroup_" + self.windowNo).val(bpgroupID);

        OnRelationChangeEvent(BPartnerID, self);
        txtLoc.getBtn(0).css("width", "10%");
        txtLoc.getBtn(0).css("height", "10%");
        txtLoc.getBtn(0).css("padding", "0px");
        //txtLoc.getBtn(0).css("border-radius", "none");
        txtLoc.getBtn(1).css("width", "10%");
        txtLoc.getBtn(1).css("height", "10%");
        txtLoc.getBtn(1).css("padding", "0px");
        //txtLoc.getBtn(1).css("border-radius", "none");
        $div.append(txtLoc.getControl().css("width", "80%")).append(txtLoc.getBtn(0)).append(txtLoc.getBtn(1));

        return $bpartnerroot;
    };




    //----------------------------------------------------------------------------------------------------//
    // Add or Update business partner
    //----------------------------------------------------------------------------------------------------//


    VIS.AddUpdateBPartner = function BPartner(windowNo, BPartnerID, BPtype, $busy, bpartnerID, bpGroupID) {
        var $txtName = null;
        var $txtSearchKey = null;
        var $txtGreeting = null;
        var $txtName2 = null;

        var $txtContact = null;
        var $txtGreeting1 = null;
        var $txtTitle = null;
        var $txtEmail = null;
        //txtLoc.getV
        var $txtPhone = null;
        var $txtPhone2 = null;
        var $txtFax = null;
        var $txtTitle = null;
        var $txtAddress = null;
        var $btnAddress = null;
        var $isBusy = null;
        var BPRelationID = 0;
        var BPGroupID = 0;
        var ddlAddress = null;
        var $chkCustomer = null;
        var $chkVendor = null;
        var $chkProspect = null;
        var $chkEmployee = null;
        var btnUpload = null;
        var fileDialog = null;
        var imageName = null;
        var fileKey = "";
        var imgBPartner = null;
        var txtMobile = null;
        var txtWebUrl = null;
        if ($busy == null) {
            $(".vis-apanel-busy").css("visibility", "visible");
            $(".vis-apanel-busy").css("z-index", "9999999");
        }
        $isBusy = $busy;
        if ($addBPartnerRoot != null) {
            $addBPartnerRoot.empty();
        }

        BPRelationID = bpartnerID;
        BPGroupID = bpGroupID;



        function InitBP(windowNo, BPartnerID, BPtype) {

            LoadControls(windowNo, BPartnerID, BPtype);
        };

        function LoadControls(windowNo, BPartnerID, BPtype) {

            var $bpartnerroot = null;

            // userID = $("#userID").val();
            var greeting = "";
            var bpgroup = "";
            var bprelation = "";
            var bplocation = "";
            var lookup = new VIS.MLocationLookup(VIS.context.ctx, windowNo);
            txtLoc = new VIS.Controls.VLocation("C_Location_ID", true, false, true, VIS.DisplayType.Location, lookup);
            var count = txtLoc.getBtnCount();
            $addBPartnerRoot = $("<div id='divBPartner_" + windowNo + "' style='width:auto;height:auto;margin-right:5px'> <img src='/ViennaAdvantageWeb1/Areas/VIS/Images/busy.gif' style=' margin-top: 120px; margin-left: 327px; z-index:9999;position:absolute;visibility:collapse'>");

            OpenDialogPopup($addBPartnerRoot, windowNo, BPartnerID, BPtype);
            var url = VIS.Application.contextUrl + "BPartner/InitBP?WinNo=" + windowNo + "&bPartnerID=" + BPartnerID + "&bpType=" + BPtype;
            $.ajax({
                type: "GET",
                async: false,
                url: url,
                dataType: "json",

                success: function (data) {
                    var dd = JSON.parse(data);
                    for (var i = 0; i < dd.lstGreeting.length; i++) {
                        greeting += "<option value='" + dd.lstGreeting[i].ID + "'>" + VIS.Utility.encodeText(dd.lstGreeting[i].Name) + "</option>";
                    }
                    for (var i = 0; i < dd.lstBPGroup.length; i++) {
                        bpgroup += "<option value='" + dd.lstBPGroup[i].ID + "'>" + VIS.Utility.encodeText(dd.lstBPGroup[i].Name) + "</option>";
                    }
                    // VIS0060
                    //for (var i = 0; i < dd.lstBPRelation.length; i++) {
                    //    bprelation += "<option value='" + dd.lstBPRelation[i].ID + "'>" + VIS.Utility.encodeText(dd.lstBPRelation[i].Name) + "</option>";
                    //}
                    //for (var i = 0; i < dd.lstBPLocation.length; i++) {
                    //    bplocation += "<option value='" + dd.lstBPLocation[i].ID + "'>" + dd.lstBPLocation[i].Name + "</option>";
                    //}
                    $bpartnerroot = HtmlTable(windowNo, BPartnerID, BPtype, dd, greeting, bpgroup, bplocation, bprelation);
                }
            });
            ////$addBPartnerRoot.append($bpartnerroot);
            ////$addBPartnerRoot.load();
            window.setTimeout(function () {
                if (addBPartnerDialog != null) {
                    //addBPartnerDialog.setContent($bpartnerroot);
                    $addBPartnerRoot.append($bpartnerroot);
                    $addBPartnerRoot.load();
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    $(".vis-apanel-busy").css("height", "460px");
                    $(".vis-apanel-busy").css("z-index", "99");
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                }
            }, 500);
            //OpenDialogPopup($addBPartnerRoot, windowNo, BPartnerID, BPtype);
        };


        //function HtmlTable(windowNo, BPartnerID, BPtype, dd, greeting, bpgroup, bplocation, bprelation) {

        //    //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'><tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name2") + "  </div></td><td style='width:300px'><div style='width:95%' class='wsp-BPartner'><input id='txtName2_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></td><td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select>"
        //    //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td style='border: 1px solid #bdbdbd;border-right: 1px white;'><div style='float:left;margin-right: 15px;margin-bottom: 2px;'>  <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "  </div> <div style='float:right;margin-right: 15px;margin-bottom: 2px;'> <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + "  </div></td>  <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;width:50px'></td> <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'></div></td><td ><div style='float:right;margin-right: 15px;/* margin-bottom: 2px; */border: 1px solid #bdbdbd;      border-left: 1px white;width: 288px;height: 25px;'>  </div></td></tr>" +
        //    //        var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td  colspan='4' ><div style='float:left;margin-right: 15px;margin-bottom: 2px;border: 1px solid #bdbdbd;width:98%;height: 35px;margin-left: 5px;'> <div style='float:left;margin-left:100px;margin-top: 5px;'> <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "</div> <div style='float:left;margin-left:150px;margin-top: 5px;'> <input id='chkProspect_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Prospect") + "</div> <div style='float:left;margin-left: 200px;margin-top: 5px;'>  <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + " </div>  </div></td>  </tr>" +
        //    //            "<tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory'  id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory' id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td>  <td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select class='vis-gc-vpanel-table-mandatory' id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select></div> <div id='divBPGroup' style='color:red;visibility:collapse'>*</div></td></tr>"
        //    //+ "<tr>"
        //    //+ "<td>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPRelation") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div style='width:95%' class='wsp-BPartner'>"
        //    //  + "<select style='width:100%' id='ddlBPRelation_" + windowNo + "' >"

        //    //      + bprelation
        //    //  + "</select>"
        //    //+ "</div>"
        //    //+ "</td> <td style='width:50px'></td>"
        //    //+ "<td style='margin-left:10px'>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPLocation") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
        //    //  + "<select id='ddlBPLoaction_" + windowNo + "' style='width:100%'>"
        //    //  + "</select>"
        //    //+ "</div>"
        //    //+ "</td>"
        //    //+ "</tr>"
        //    //+ "<tr>"
        //    //+ "<td>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Contact") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div style='width:95%;float:left' class='wsp-BPartner'>"
        //    //  + "<input class='vis-gc-vpanel-table-mandatory' id='txtContact_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
        //    //+ "</div><div id='divContact_" + windowNo + "' style='color:red;visibility:collapse'>*</div>"
        //    //+ "</td> <td style='width:50px'></td>"
        //    //+ "<td style='margin-left:10px'>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
        //    //  + "<select id='ddlGreeting_" + windowNo + "' style='width: 100%'>"
        //    //    + greeting
        //    //  + "</select>"
        //    //+ "</div>"
        //    //+ "</td>"
        //    //+ "</tr>"
        //    //+ "<tr>"
        //    //+ "<td>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Title") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div style='width:95%' class='wsp-BPartner'>"
        //    //  + "<input id='txtTitle_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
        //    //+ "</div>"
        //    //+ "</td> <td style='width:50px'></td>"
        //    //+ "<td style='margin-left:10px'>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("EmailAddress") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
        //    //  + "<input id='txtEmail_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
        //    //+ "</div>"
        //    //+ "</td>"
        //    //+ "</tr>"
        //    //+ "<tr>"
        //    //+ "<td>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Address") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'><div style='width:95%;float:left' id='divAddress_" + windowNo + "' class='wsp-BPartner' ></div> <div id='divAddressValidate_" + windowNo + "' style='color:red;visibility:collapse'>*</div></td> <td style='width:50px'></td>"
        //    //+ "<td style='margin-left:10px'>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Phone") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
        //    //  + "<input id='txtPhone_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
        //    //+ "</div>"
        //    //+ "</td>"
        //    //+ "</tr>"
        //    //+ "<tr>"
        //    //+ "<td>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("2ndPhone") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px'>"
        //    //+ "<div class='wsp-BPartner' style='width:95%;'>"
        //    //  + "<input id='txtPhone2_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
        //    //+ "</div>"
        //    //+ "</td> <td style='width:50px'></td>"
        //    //+ "<td style='margin-left:10px'>"
        //    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Fax") + "  </div>"
        //    //+ "</td>"
        //    //+ "<td style='width:300px' >"
        //    //+ "<div style='margin-bottom:7px;margin-right: 5px;width:95%' class='wsp-BPartner' >"
        //    //  + "<input id='txtFax_" + windowNo + "' style='height: 25px;width:100%;' type='text'>"
        //    //+ "</div>"
        //    //+ "</td>"
        //    //+ "</tr>"
        //    //+ "</table>");



        //    var $bpartnerroot = $("<DIV class='vis-MainDivBPartnerPopUp'>" +
        //        "<DIV class='vis-DivBPartnerLeft' >" +
        //        "<DIV class='vis-FirstDivBPartnerPopUp' > </DIV>" +
        //        "" + Splliter("50%", "Phone Number", 5) + Splliter("50%", "Internet", 100) +
        //        "<DIV class='vis-SecondDivBPartnerPopUp'> </DIV>" +
        //          "" + Splliter("100%", "Address", 5) +
        //        "<DIV class='vis-ThirdDivBPartnerPopUp'> </DIV> </DIV>" +

        //          //"<DIV class='vis-DivBPartnerRight' > </DIV>" +
        //          "<DIV class='vis-FourthDivBPartnerPopUp'> </DIV> " +
        //        "</DIV>");
        //    $bpartnerroot.find(".vis-FirstDivBPartnerPopUp").append("<DIV class='vis-FirstDivCotrol' ></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivBPartnerPopUp").append("<DIV class='vis-DivBPartnerRight' > </DIV>");
        //    $bpartnerroot.find(".vis-SecondDivBPartnerPopUp").append("<DIV class='vis-SecondDivCotrol' ></DIV> ");
        //    $bpartnerroot.find(".vis-SecondDivBPartnerPopUp").append("<DIV class='vis-SecondDivImgCotrol'></DIV> ");
        //    $bpartnerroot.find(".vis-ThirdDivBPartnerPopUp").append("<DIV class='vis-ThirdDivCotrol' ></DIV>");
        //    $bpartnerroot.find(".vis-FourthDivBPartnerPopUp").append("<DIV class='vis-FourthDivHeader' > " + VIS.Msg.getMsg("BPType") + " </DIV> <DIV class='vis-FourthDivControl'></DIV>");
        //    $bpartnerroot.find(".vis-DivBPartnerRight").append("<div class='image-upload' style='float: right; position: absolute; z-index: 999; top:12px;overflow:hidden;right:0'><label for='file-input' class='wsp-file-label'><span class='wsp-change-picture-ico'></span></label><input type='file' id='VISFileDialog_" + windowNo + "' accept='image/*' class='wsp-file-input' value='Add Pic' style='opacity: 0; width: 100%; margin-top: 0px; float: right; z-index: 999; position: absolute; right: 0px; cursor: pointer;'></div>");
        //    $bpartnerroot.find(".vis-DivBPartnerRight").append("<DIV class='vis-FirstDivImgCotrol'></DIV>");
        //    // $bpartnerroot.find(".vis-DivBPartnerRight").append("<DIV class='vis-SecondDivImgCotrol'></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("SearchKeyValue") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory'  id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Greeting") + "  </div> <div class='vis-all-control'><select id='ddlGreeting_" + windowNo + "' style='width: 100%'>" + greeting + "</select></div></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Contact") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory' id='txtContact_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Name") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory' id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
        //    $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("BPGroup") + "  </div> <div class='vis-all-control'><select class='vis-gc-vpanel-table-mandatory' id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select> </div></DIV>");
        //    //$bpartnerroot.find(".vis-FirstDivImgCotrol").append("<DIV class='vis-FirstDivCotrolChild' >  <div style='float: left;width:320px;height:150px;line-height: 236px;background: none repeat scroll 0% 0% #EEE;padding:0px;text-align:center' ><img id='imgBPartner_" + windowNo + "' /></div>  <div style='float: left;width:100%;height:20%;padding:2px' >   <button  id='VISBPartenrImg_" + windowNo + "' class='VIS-open-icon'></button>" +
        //    //                "<input type='file' id='VISFileDialog_" + windowNo + "' accept='image/*' style='display:none'></div></DIV>");

        //    $bpartnerroot.find(".vis-FirstDivImgCotrol").append("<DIV class='vis-FirstDivCotrolChild' >  <div style='float: left;width:320px;height:186px;line-height: 184px;background: none repeat scroll 0% 0% #EEE;padding:0px;text-align:center' ><img id='imgBPartner_" + windowNo + "' /></div> ");

        //    $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Phone") + "  </div> <div class='vis-all-control'><input id='txtPhone_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
        //    $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Mobile") + "  </div> <div class='vis-all-control'><input id='txtMobile_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
        //    $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Fax") + "  </div> <div class='vis-all-control'><input id='txtFax_" + windowNo + "' style='height: 25px;width:100%;' type='text'> </div></DIV>");
        //    $bpartnerroot.find(".vis-SecondDivImgCotrol").append("<DIV class='vis-SecondDivCotrolChild' style='padding:0px'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("EmailAddress") + "  </div> <div class='vis-all-control'><input id='txtEmail_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
        //    $bpartnerroot.find(".vis-SecondDivImgCotrol").append("<DIV class='vis-SecondDivCotrolChild' style='padding:0px'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("WebUrl") + "  </div> <div class='vis-all-control'><input id='txtWebUrl_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
        //    $bpartnerroot.find(".vis-ThirdDivCotrol").append("<DIV class='vis-ThirdDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Address") + "  </div> <div id='divAddress_" + windowNo + "' class='vis-all-control'></div></DIV>");

        //    $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:33%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "</div></DIV>");
        //    $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:33%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + " </div></DIV>");
        //    $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:33%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkProspect_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Prospect") + "</div></DIV>");

        //    var $div = $bpartnerroot.find("#divAddress_" + windowNo);
        //    $selectBPRelation = $bpartnerroot.find("#ddlBPRelation_" + windowNo);
        //    $selectBPLocation = $bpartnerroot.find("#ddlBPLoaction_" + windowNo);
        //    $selectBPGroup = $bpartnerroot.find("#ddlBPGroup_" + windowNo);
        //    $txtName = $bpartnerroot.find("#txtName_" + windowNo);
        //    $txtSearchKey = $bpartnerroot.find("#txtSearchKey_" + windowNo);
        //    $txtGreeting1 = $bpartnerroot.find("#ddlGreeting1_" + windowNo);
        //    $txtName2 = $bpartnerroot.find("#txtName2_" + windowNo);

        //    $txtContact = $bpartnerroot.find("#txtContact_" + windowNo);
        //    $txtGreeting = $bpartnerroot.find("#ddlGreeting_" + windowNo);
        //    $txtTitle = $bpartnerroot.find("#txtTitle_" + windowNo);
        //    $txtEmail = $bpartnerroot.find("#txtEmail_" + windowNo);

        //    $txtPhone = $bpartnerroot.find("#txtPhone_" + windowNo);
        //    $txtPhone2 = $bpartnerroot.find("#txtPhone2_" + windowNo);
        //    $txtFax = $bpartnerroot.find("#txtFax_" + windowNo);
        //    $chkCustomer = $bpartnerroot.find("#chkCustomer_" + windowNo);
        //    $chkVendor = $bpartnerroot.find("#chkVendor_" + windowNo);
        //    $chkProspect = $bpartnerroot.find("#chkProspect_" + windowNo);
        //    txtLoc.setValue(dd.location);
        //    //btnUpload = $bpartnerroot.find("#VISBPartenrImg_" + windowNo);
        //    fileDialog = $bpartnerroot.find("#VISFileDialog_" + windowNo);
        //    imgBPartner = $bpartnerroot.find("#imgBPartner_" + windowNo);
        //    txtMobile = $bpartnerroot.find("#txtMobile_" + windowNo);
        //    txtWebUrl = $bpartnerroot.find("#txtWebUrl_" + windowNo);
        //    Events();
        //    if (BPartnerID > 0) {
        //        //$bpartnerroot.find("#ddlBPRelation_" + windowNo).val(dd.bpRelationID);
        //        //$bpartnerroot.find("#ddlBPGroup_" + windowNo).val(dd.bpGroupID);
        //        //$bpartnerroot.find("#ddlBPLoaction_" + windowNo).val(dd.bpLocationID);


        //        //$bpartnerroot.find("#txtName_" + windowNo).val(dd.name);
        //        //$bpartnerroot.find("#txtSearchKey_" + windowNo).val(dd.searchKey);
        //        //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val();
        //        //$bpartnerroot.find("#txtName2_" + windowNo).val(dd.name2);

        //        //$bpartnerroot.find("#txtContact_" + windowNo).val(dd.contact);
        //        //$bpartnerroot.find("#ddlGreeting_" + windowNo).val();
        //        //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);
        //        //$bpartnerroot.find("#txtEmail_" + windowNo).val(dd.email);
        //        ////txtLoc.getValue();
        //        //$bpartnerroot.find("#txtPhone_" + windowNo).val(dd.phoneNo);
        //        //$bpartnerroot.find("#txtPhone2_" + windowNo).val(dd.phoneNo2);
        //        //$bpartnerroot.find("#txtFax_" + windowNo).val(dd.fax);
        //        //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val(dd.greeting);
        //        //$bpartnerroot.find("#ddlGreeting_" + windowNo).val(dd.greeting1);
        //        //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);




        //        $selectBPRelation.val(dd.bpRelationID);
        //        $selectBPGroup.val(dd.bpGroupID);
        //        $selectBPLocation.val(dd.bpLocationID);


        //        $txtName.val(VIS.Utility.decodeText(dd.name));
        //        $txtSearchKey.val(VIS.Utility.decodeText(dd.searchKey));
        //        $txtGreeting1.val();
        //        $txtName2.val(VIS.Utility.decodeText(dd.name2));
        //        imgBPartner.removeAttr("src").attr("src", dd.userImage);
        //        $txtContact.val(VIS.Utility.decodeText(dd.contact));
        //        $txtGreeting.val();
        //        $txtTitle.val(VIS.Utility.decodeText(dd.title));
        //        $txtEmail.val(VIS.Utility.decodeText(dd.email));
        //        //txtLoc.getValue();
        //        $txtPhone.val(VIS.Utility.decodeText(dd.phoneNo));
        //        $txtPhone2.val(VIS.Utility.decodeText(dd.phoneNo2));
        //        $txtFax.val(VIS.Utility.decodeText(dd.fax));
        //        $txtGreeting.val(dd.greeting);
        //        $txtGreeting1.val(dd.greeting1);
        //        txtLoc.setValue(dd.location);
        //        txtMobile.val(dd.mobile);
        //        txtWebUrl.val(dd.WebUrl);
        //        if (dd.isCustomer) {
        //            $chkCustomer.attr("checked", "checked");
        //        }
        //        if (dd.isVendor) {
        //            $chkVendor.attr("checked", "checked");
        //        }
        //        OnSelectionChangeEvent($selectBPRelation.val(), windowNo, dd.bpLocationID);
        //    }
        //    else if (BPRelationID > 0 && BPGroupID > 0) {
        //        $bpartnerroot.find("#ddlBPRelation_" + windowNo).val(BPRelationID);
        //        $bpartnerroot.find("#ddlBPGroup_" + windowNo).val(BPGroupID);
        //        OnSelectionChangeEvent($selectBPRelation.val(), windowNo, dd.bpLocationID);
        //    }


        //    txtLoc.getBtn(0).css("width", "10%");
        //    txtLoc.getBtn(0).css("height", "30px");
        //    txtLoc.getBtn(0).css("padding", "0px");
        //    //txtLoc.getBtn(0).css("border-radius", "none");
        //    txtLoc.getBtn(1).css("width", "10%");
        //    txtLoc.getBtn(1).css("height", "30px");
        //    txtLoc.getBtn(1).css("padding", "0px");
        //    //txtLoc.getBtn(1).css("border-radius", "none");
        //    $div.append(txtLoc.getControl().css("width", "80%")).append(txtLoc.getBtn(0)).append(txtLoc.getBtn(1));
        //    $txtAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Input");
        //    $btnAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Button").eq(1);
        //    SaveBPartner(windowNo, BPtype, BPartnerID);
        //    $selectBPRelation.on("change", function () {

        //        OnSelectionChangeEvent($selectBPRelation.val(), windowNo);
        //    });
        //    $txtSearchKey.on("focusout", function () {

        //        if ($txtSearchKey.val() == "") {
        //            $("#divSearchKey").css("visibility", "visible");
        //        }
        //        else {
        //            $("#divSearchKey").css("visibility", "collapse ");
        //        }
        //    });
        //    $txtName.on("focusout", function () {

        //        if ($txtName.val() == "") {
        //            $("#divName").css("visibility", "visible");
        //        }
        //        else {
        //            $("#divName").css("visibility", "collapse");
        //        }
        //    });
        //    $selectBPGroup.on("focusout", function () {

        //        if ($selectBPGroup.val() == "0" || $selectBPGroup.val() == "") {
        //            $("#divBPGroup").css("visibility", "visible");
        //        }
        //        else {
        //            $("#divBPGroup").css("visibility", "collapse ");
        //        }
        //    });

        //    $txtAddress.on("focusout", function () {

        //        if (txtLoc.getValue() <= 0) {
        //            $("#divAddressValidate_" + windowNo).css("visibility", "visible");
        //        }
        //        else {
        //            $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
        //        }
        //    });
        //    $btnAddress.on("focusout", function () {

        //        if (txtLoc.getValue() <= 0) {
        //            $("#divAddressValidate_" + windowNo).css("visibility", "visible");
        //        }
        //        else {
        //            $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
        //        }
        //    });
        //    $txtContact.on("focusout", function () {

        //        if ($txtContact.val() == "") {
        //            $("#divContact_" + windowNo).css("visibility", "visible");
        //        }
        //        else {
        //            $("#divContact_" + windowNo).css("visibility", "collapse");
        //        }
        //    });
        //    return $bpartnerroot;
        //};







        function HtmlTable(windowNo, BPartnerID, BPtype, dd, greeting, bpgroup, bplocation, bprelation) {

            //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'><tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name2") + "  </div></td><td style='width:300px'><div style='width:95%' class='wsp-BPartner'><input id='txtName2_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></td><td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select>"
            //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td style='border: 1px solid #bdbdbd;border-right: 1px white;'><div style='float:left;margin-right: 15px;margin-bottom: 2px;'>  <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "  </div> <div style='float:right;margin-right: 15px;margin-bottom: 2px;'> <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + "  </div></td>  <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;width:50px'></td> <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'></div></td><td ><div style='float:right;margin-right: 15px;/* margin-bottom: 2px; */border: 1px solid #bdbdbd;      border-left: 1px white;width: 288px;height: 25px;'>  </div></td></tr>" +
            //        var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td  colspan='4' ><div style='float:left;margin-right: 15px;margin-bottom: 2px;border: 1px solid #bdbdbd;width:98%;height: 35px;margin-left: 5px;'> <div style='float:left;margin-left:100px;margin-top: 5px;'> <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "</div> <div style='float:left;margin-left:150px;margin-top: 5px;'> <input id='chkProspect_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Prospect") + "</div> <div style='float:left;margin-left: 200px;margin-top: 5px;'>  <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + " </div>  </div></td>  </tr>" +
            //            "<tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory'  id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory' id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td>  <td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select class='vis-gc-vpanel-table-mandatory' id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select></div> <div id='divBPGroup' style='color:red;visibility:collapse'>*</div></td></tr>"
            //+ "<tr>"
            //+ "<td>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPRelation") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div style='width:95%' class='wsp-BPartner'>"
            //  + "<select style='width:100%' id='ddlBPRelation_" + windowNo + "' >"

            //      + bprelation
            //  + "</select>"
            //+ "</div>"
            //+ "</td> <td style='width:50px'></td>"
            //+ "<td style='margin-left:10px'>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPLocation") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
            //  + "<select id='ddlBPLoaction_" + windowNo + "' style='width:100%'>"
            //  + "</select>"
            //+ "</div>"
            //+ "</td>"
            //+ "</tr>"
            //+ "<tr>"
            //+ "<td>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Contact") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div style='width:95%;float:left' class='wsp-BPartner'>"
            //  + "<input class='vis-gc-vpanel-table-mandatory' id='txtContact_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
            //+ "</div><div id='divContact_" + windowNo + "' style='color:red;visibility:collapse'>*</div>"
            //+ "</td> <td style='width:50px'></td>"
            //+ "<td style='margin-left:10px'>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
            //  + "<select id='ddlGreeting_" + windowNo + "' style='width: 100%'>"
            //    + greeting
            //  + "</select>"
            //+ "</div>"
            //+ "</td>"
            //+ "</tr>"
            //+ "<tr>"
            //+ "<td>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Title") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div style='width:95%' class='wsp-BPartner'>"
            //  + "<input id='txtTitle_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
            //+ "</div>"
            //+ "</td> <td style='width:50px'></td>"
            //+ "<td style='margin-left:10px'>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("EmailAddress") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
            //  + "<input id='txtEmail_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
            //+ "</div>"
            //+ "</td>"
            //+ "</tr>"
            //+ "<tr>"
            //+ "<td>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Address") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'><div style='width:95%;float:left' id='divAddress_" + windowNo + "' class='wsp-BPartner' ></div> <div id='divAddressValidate_" + windowNo + "' style='color:red;visibility:collapse'>*</div></td> <td style='width:50px'></td>"
            //+ "<td style='margin-left:10px'>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Phone") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
            //  + "<input id='txtPhone_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
            //+ "</div>"
            //+ "</td>"
            //+ "</tr>"
            //+ "<tr>"
            //+ "<td>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("2ndPhone") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px'>"
            //+ "<div class='wsp-BPartner' style='width:95%;'>"
            //  + "<input id='txtPhone2_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
            //+ "</div>"
            //+ "</td> <td style='width:50px'></td>"
            //+ "<td style='margin-left:10px'>"
            //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Fax") + "  </div>"
            //+ "</td>"
            //+ "<td style='width:300px' >"
            //+ "<div style='margin-bottom:7px;margin-right: 5px;width:95%' class='wsp-BPartner' >"
            //  + "<input id='txtFax_" + windowNo + "' style='height: 25px;width:100%;' type='text'>"
            //+ "</div>"
            //+ "</td>"
            //+ "</tr>"
            //+ "</table>");



            var $bpartnerroot = $("<DIV class='vis-MainDivBPartnerPopUp'>" +
                "<DIV class='vis-DivBPartnerLeft' >" +
                "<DIV class='vis-FirstDivBPartnerPopUp' > </DIV>" +
                "" + Splliter("50%", "Phone Number", 5) + Splliter("50%", "Internet", 100) +
                "<DIV class='vis-SecondDivBPartnerPopUp'> </DIV>" +
                  "" + Splliter("100%", "Address", 5) +
                "<DIV class='vis-ThirdDivBPartnerPopUp'> </DIV> </DIV>" +

                  //"<DIV class='vis-DivBPartnerRight' > </DIV>" +
                  "<DIV class='vis-FourthDivBPartnerPopUp'> </DIV> " +
                "</DIV>");
            $bpartnerroot.find(".vis-FirstDivBPartnerPopUp").append("<DIV class='vis-FirstDivCotrol' ></DIV>");
            $bpartnerroot.find(".vis-FirstDivBPartnerPopUp").append("<DIV class='vis-DivBPartnerRight' > </DIV>");
            $bpartnerroot.find(".vis-SecondDivBPartnerPopUp").append("<DIV class='vis-SecondDivCotrol' ></DIV> ");
            $bpartnerroot.find(".vis-SecondDivBPartnerPopUp").append("<DIV class='vis-SecondDivImgCotrol'></DIV> ");
            $bpartnerroot.find(".vis-ThirdDivBPartnerPopUp").append("<DIV class='vis-ThirdDivCotrol' ></DIV>");
            $bpartnerroot.find(".vis-FourthDivBPartnerPopUp").append("<DIV class='vis-FourthDivHeader' > " + VIS.Msg.getMsg("BPType") + " </DIV> <DIV class='vis-FourthDivControl'></DIV>");
            $bpartnerroot.find(".vis-DivBPartnerRight").append("<div class='image-upload' style='float: right; position: absolute; z-index: 999; top:40px;overflow:hidden;right:20px'><label for='file-input' class='vis-file-label-bp'><span style='margin-right:8px' class='vis-change-picture-ico vis vis-pencil'></span></label><input type='file' id='VISFileDialog_" + windowNo + "' accept='image/*' class='wsp-file-input' value='Add Pic' style='opacity: 0; width: 100%; margin-top: 0px; float: right; z-index: 999; position: absolute; right: 0px; cursor: pointer;'></div>");
            $bpartnerroot.find(".vis-DivBPartnerRight").append("<DIV class='vis-FirstDivImgCotrol'></DIV>");
            // $bpartnerroot.find(".vis-DivBPartnerRight").append("<DIV class='vis-SecondDivImgCotrol'></DIV>");
            $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("SearchKeyValue") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory'  id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
            $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Greeting") + "  </div> <div class='vis-all-control'><select id='ddlGreeting_" + windowNo + "' style='width: 100%'>" + greeting + "</select></div></DIV>");
            $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("ContactName") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory' id='txtContact_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
            $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("CompanyName") + "  </div> <div class='vis-all-control'><input class='vis-gc-vpanel-table-mandatory' id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
            $bpartnerroot.find(".vis-FirstDivCotrol").append("<DIV class='vis-FirstDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("BusinessPartnerGroup") + "  </div> <div class='vis-all-control'><select class='vis-gc-vpanel-table-mandatory' id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select> </div></DIV>");
            //$bpartnerroot.find(".vis-FirstDivImgCotrol").append("<DIV class='vis-FirstDivCotrolChild' >  <div style='float: left;width:320px;height:150px;line-height: 236px;background: none repeat scroll 0% 0% #EEE;padding:0px;text-align:center' ><img id='imgBPartner_" + windowNo + "' /></div>  <div style='float: left;width:100%;height:20%;padding:2px' >   <button  id='VISBPartenrImg_" + windowNo + "' class='VIS-open-icon'></button>" +
            //                "<input type='file' id='VISFileDialog_" + windowNo + "' accept='image/*' style='display:none'></div></DIV>");

            $bpartnerroot.find(".vis-FirstDivImgCotrol").append("<DIV class='vis-FirstDivCotrolChild' >  <div style='float: left;width:100%;height:242px;line-height:236px;background: none repeat scroll 0% 0% #EEE;padding:0px;text-align:center;margin-top:20px' ><img id='imgBPartner_" + windowNo + "' /></div> ");

            $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Phone") + "  </div> <div class='vis-all-control'><input id='txtPhone_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
            $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Mobile") + "  </div> <div class='vis-all-control'><input id='txtMobile_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></DIV>");
            $bpartnerroot.find(".vis-SecondDivCotrol").append("<DIV class='vis-SecondDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Fax") + "  </div> <div class='vis-all-control'><input id='txtFax_" + windowNo + "' style='height: 25px;width:100%;' type='text'> </div></DIV>");
            $bpartnerroot.find(".vis-SecondDivImgCotrol").append("<DIV class='vis-SecondDivCotrolChild' > <div class='vis-all-lable'>" + VIS.Msg.getMsg("EmailAddress") + "  </div> <div class='vis-all-control'><input id='txtEmail_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
            $bpartnerroot.find(".vis-SecondDivImgCotrol").append("<DIV class='vis-SecondDivCotrolChild' > <div class='vis-all-lable'>" + VIS.Msg.getMsg("WebUrl") + "  </div> <div class='vis-all-control'><input id='txtWebUrl_" + windowNo + "' style='height: 25px;width:100%' type='text'> </div></DIV>");
            $bpartnerroot.find(".vis-ThirdDivCotrol").append("<DIV class='vis-ThirdDivCotrolChild'> <div class='vis-all-lable'>" + VIS.Msg.getMsg("Address") + "  </div> <div id='divAddress_" + windowNo + "' class='vis-all-control'></div></DIV>");

            $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:25%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "</div></DIV>");
            $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:25%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + " </div></DIV>");
            $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:25%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkProspect_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Prospect") + "</div></DIV>");
            $bpartnerroot.find(".vis-FourthDivControl").append("<DIV style='float: left;width:25%'> <div style='float: left;width:100%' class='wsp-BPartner'><input id='chkEmployee_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Employee") + "</div></DIV>");

            var $div = $bpartnerroot.find("#divAddress_" + windowNo);
            $selectBPRelation = $bpartnerroot.find("#ddlBPRelation_" + windowNo);
            $selectBPLocation = $bpartnerroot.find("#ddlBPLoaction_" + windowNo);
            $selectBPGroup = $bpartnerroot.find("#ddlBPGroup_" + windowNo);
            $txtName = $bpartnerroot.find("#txtName_" + windowNo);
            $txtSearchKey = $bpartnerroot.find("#txtSearchKey_" + windowNo);
            $txtGreeting1 = $bpartnerroot.find("#ddlGreeting1_" + windowNo);
            $txtName2 = $bpartnerroot.find("#txtName2_" + windowNo);

            $txtContact = $bpartnerroot.find("#txtContact_" + windowNo);
            $txtGreeting = $bpartnerroot.find("#ddlGreeting_" + windowNo);
            $txtTitle = $bpartnerroot.find("#txtTitle_" + windowNo);
            $txtEmail = $bpartnerroot.find("#txtEmail_" + windowNo);

            $txtPhone = $bpartnerroot.find("#txtPhone_" + windowNo);
            $txtPhone2 = $bpartnerroot.find("#txtPhone2_" + windowNo);
            $txtFax = $bpartnerroot.find("#txtFax_" + windowNo);
            $chkCustomer = $bpartnerroot.find("#chkCustomer_" + windowNo);
            $chkVendor = $bpartnerroot.find("#chkVendor_" + windowNo);
            $chkProspect = $bpartnerroot.find("#chkProspect_" + windowNo);
            $chkEmployee = $bpartnerroot.find("#chkEmployee_" + windowNo);
            txtLoc.setValue(dd.location);
            //btnUpload = $bpartnerroot.find("#VISBPartenrImg_" + windowNo);
            fileDialog = $bpartnerroot.find("#VISFileDialog_" + windowNo);
            imgBPartner = $bpartnerroot.find("#imgBPartner_" + windowNo);
            txtMobile = $bpartnerroot.find("#txtMobile_" + windowNo);
            txtWebUrl = $bpartnerroot.find("#txtWebUrl_" + windowNo);
            Events();
            if (BPartnerID > 0) {
                //$bpartnerroot.find("#ddlBPRelation_" + windowNo).val(dd.bpRelationID);
                //$bpartnerroot.find("#ddlBPGroup_" + windowNo).val(dd.bpGroupID);
                //$bpartnerroot.find("#ddlBPLoaction_" + windowNo).val(dd.bpLocationID);


                //$bpartnerroot.find("#txtName_" + windowNo).val(dd.name);
                //$bpartnerroot.find("#txtSearchKey_" + windowNo).val(dd.searchKey);
                //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val();
                //$bpartnerroot.find("#txtName2_" + windowNo).val(dd.name2);

                //$bpartnerroot.find("#txtContact_" + windowNo).val(dd.contact);
                //$bpartnerroot.find("#ddlGreeting_" + windowNo).val();
                //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);
                //$bpartnerroot.find("#txtEmail_" + windowNo).val(dd.email);
                ////txtLoc.getValue();
                //$bpartnerroot.find("#txtPhone_" + windowNo).val(dd.phoneNo);
                //$bpartnerroot.find("#txtPhone2_" + windowNo).val(dd.phoneNo2);
                //$bpartnerroot.find("#txtFax_" + windowNo).val(dd.fax);
                //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val(dd.greeting);
                //$bpartnerroot.find("#ddlGreeting_" + windowNo).val(dd.greeting1);
                //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);




                $selectBPRelation.val(dd.bpRelationID);
                $selectBPGroup.val(dd.bpGroupID);
                $selectBPLocation.val(dd.bpLocationID);


                $txtName.val(VIS.Utility.decodeText(dd.name));
                $txtSearchKey.val(VIS.Utility.decodeText(dd.searchKey));
                $txtGreeting1.val();
                $txtName2.val(VIS.Utility.decodeText(dd.name2));
                imgBPartner.removeAttr("src").attr("src", dd.userImage);
                $txtContact.val(VIS.Utility.decodeText(dd.contact));
                $txtGreeting.val();
                $txtTitle.val(VIS.Utility.decodeText(dd.title));
                $txtEmail.val(VIS.Utility.decodeText(dd.email));
                //txtLoc.getValue();
                $txtPhone.val(VIS.Utility.decodeText(dd.phoneNo));
                $txtPhone2.val(VIS.Utility.decodeText(dd.phoneNo2));
                $txtFax.val(VIS.Utility.decodeText(dd.fax));
                $txtGreeting.val(dd.greeting);
                $txtGreeting1.val(dd.greeting1);
                txtLoc.setValue(dd.location);
                txtMobile.val(dd.mobile);
                txtWebUrl.val(dd.WebUrl);
                if (dd.isCustomer) {
                    $chkCustomer.attr("checked", "checked");
                }
                if (dd.isVendor) {
                    $chkVendor.attr("checked", "checked");
                }
                if (dd.isEmployee)
                {
                    $chkEmployee.attr("checked", "checked");
                }
                // VIS0060
                OnSelectionChangeEvent(VIS.Utility.Util.getValueOfInt($selectBPRelation.val()), windowNo, dd.bpLocationID);
            }
            else if (BPRelationID > 0 && BPGroupID > 0) {
                $bpartnerroot.find("#ddlBPRelation_" + windowNo).val(BPRelationID);
                $bpartnerroot.find("#ddlBPGroup_" + windowNo).val(BPGroupID);
                // VIS0060
                OnSelectionChangeEvent(VIS.Utility.Util.getValueOfInt($selectBPRelation.val()), windowNo, dd.bpLocationID);
            }


            txtLoc.getBtn(0).css("width", "10%");
            txtLoc.getBtn(0).css("height", "30px");
            txtLoc.getBtn(0).css("padding", "0px");
            //txtLoc.getBtn(0).css("border-radius", "none");
            txtLoc.getBtn(1).css("width", "10%");
            txtLoc.getBtn(1).css("height", "30px");
            txtLoc.getBtn(1).css("padding", "0px");
            //txtLoc.getBtn(1).css("border-radius", "none");
            $div.append(txtLoc.getControl().css("width", "80%")).append(txtLoc.getBtn(0)).append(txtLoc.getBtn(1));
            $txtAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Input");
            $btnAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Button").eq(1);
            SaveBPartner(windowNo, BPtype, BPartnerID);
            $selectBPRelation.on("change", function () {
                // VIS0060
                OnSelectionChangeEvent(VIS.Utility.Util.getValueOfInt($selectBPRelation.val()), windowNo);
            });
            $txtSearchKey.on("focusout", function () {

                if ($txtSearchKey.val() == "") {
                    $("#divSearchKey").css("visibility", "visible");
                }
                else {
                    $("#divSearchKey").css("visibility", "collapse ");
                }
            });
            $txtName.on("focusout", function () {

                if ($txtName.val() == "") {
                    $("#divName").css("visibility", "visible");
                }
                else {
                    $("#divName").css("visibility", "collapse");
                }
            });
            $selectBPGroup.on("focusout", function () {

                if ($selectBPGroup.val() == "0" || $selectBPGroup.val() == "") {
                    $("#divBPGroup").css("visibility", "visible");
                }
                else {
                    $("#divBPGroup").css("visibility", "collapse ");
                }
            });

            $txtAddress.on("focusout", function () {

                if (txtLoc.getValue() <= 0) {
                    $("#divAddressValidate_" + windowNo).css("visibility", "visible");
                }
                else {
                    $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
                }
            });
            $btnAddress.on("focusout", function () {

                if (txtLoc.getValue() <= 0) {
                    $("#divAddressValidate_" + windowNo).css("visibility", "visible");
                }
                else {
                    $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
                }
            });
            $txtContact.on("focusout", function () {

                if ($txtContact.val() == "") {
                    $("#divContact_" + windowNo).css("visibility", "visible");
                }
                else {
                    $("#divContact_" + windowNo).css("visibility", "collapse");
                }
            });
            return $bpartnerroot;
        };

        function Splliter(width, text, marginleft) {
            var splliter = "<DIV class='vis-BPsplitter' style='width:" + width + "'><span style='background-color: white; position: relative;top: -0.8em;margin-left: " + marginleft + "px;'> " + text + " </span></DIV>";
            return splliter;
        };

        function Events() {
            //if (btnUpload != null) {
            //    btnUpload.on("click", function () {
            //fileDialog.off('click');
            //fileDialog.trigger('click');
            fileDialog.on('change', function (evt) {
                fileKey = Date.now().toString();
                var xhr = new XMLHttpRequest();
                var fd = new FormData();
                // var fileUploadFile = $("#wsp-file-input_" + self.windowNo);

                fd.append("file", fileDialog[0].files[0]);
                fd.append("fileKey", fileKey);
                //if ((fileDialog[0].files[0].size / kbSize) > maxSize) {
                //    alert(VIS.Msg.getMsg("UploadedFileSizeLimit"));
                //    $fileUpload.replaceWith($fileUpload.val('').clone(true));
                //    $isBusy.hide();
                //    return;
                //}

                xhr.open("POST", VIS.Application.contextUrl + "BPartner/SaveImageAsByte", true);
                xhr.setRequestHeader("Cache-Control", "no-cache");
                xhr.setRequestHeader("Pragma", "no-cache");
                xhr.send(fd);
                xhr.addEventListener("load", function (event) {

                    var dd = event.target.response;
                    var res = JSON.parse(dd);
                    var image = JSON.parse(res);
                    imgBPartner.removeAttr("src").attr("src", "data:image/jpg;base64," + image[0]);
                    imageName = image[1];

                }, false);
                //  fileDialog.val("");
            });
            //});
            // }

        };

        function dispose() {

            $txtSearchKey.off("focusout");
            $txtName.off("focusout");
            $selectBPGroup.off("focusout");
            $txtAddress.off("focusout");
            $btnAddress.off("focusout");
            $txtContact.off("focusout");
            txtLoc.dispose();
            txtLoc = null;
            $txtName = null;
            $txtName2 = null;
            $txtAddress = null;
            $txtContact = null;
            $txtEmail = null;
            $txtFax = null;
            $txtGreeting = null;
            $txtGreeting1 = null;
            $txtPhone = null;
            $txtPhone2 = null;
            $txtSearchKey = null;
            $txtTitle = null;
            BPtype = null;

            $addBPartnerRoot.remove();
            $addBPartnerRoot.empty();
        };

        function OpenDialogPopup(root, windowNo, BPartnerID, BPtype) {

            addBPartnerDialog = new VIS.ChildDialog();

            addBPartnerDialog.setContent(root);
            addBPartnerDialog.onClose = function () {
                dispose();
            }
            var windowWidth = $(window).width();
           
            if (windowWidth >= 1366) {
                addBPartnerDialog.setHeight(620);
                addBPartnerDialog.setWidth(870);
            }
            else {
                addBPartnerDialog.setHeight(520);
                addBPartnerDialog.setWidth(670);
            }
     
            addBPartnerDialog.setTitle(VIS.Msg.getMsg("BusinessPartner"));
           
            addBPartnerDialog.setEnableResize(false);
            addBPartnerDialog.setModal(true);

            addBPartnerDialog.show();
            addBPartnerDialog.getRoot().dialog({ position: [200, 130] });
        };

        function SaveBPartner(windowNo, category, BPartnerID) {
            addBPartnerDialog.onOkClick = function (e) {

                // addBPartnerDialog.dialog("open");

                if ($isBusy != null) {
                    $isBusy.show();
                    var h = $(window).height() * (50 / 100);
                    var w = $(window).width() * (50 / 100);
                }
                else {
                    $(".vis-apanel-busy").css("visibility", "visible");
                }
                ddlAddress = txtLoc.getValue();

                if ($txtSearchKey.val().trim() == "") {
                    //alert(VIS.Msg.getMsg("SearchkeyMandatory"));
                    VIS.ADialog.info("SearchKeyMandatory");                                     
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }
                if ($txtName.val().trim() == "") {
                    //alert(VIS.Msg.getMsg("NameMandatory"));
                    VIS.ADialog.info("NameMandatory");
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }

                //if (ddlBPGroup.trim() == "0") {
                if ($selectBPGroup.val().trim() == "0") {
                    //alert(VIS.Msg.getMsg("BPGroupMandatory"));
                    VIS.ADialog.info("BPGroupMandatory");
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }
                //if (txtContact.trim() == "") {
                if ($txtContact.val().trim() == "") {
                    //alert(VIS.Msg.getMsg("ContactMandatory"));
                    VIS.ADialog.info("ContactMandatory");
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }
                if (ddlAddress <= 0 || ddlAddress == null) {
                    //alert(VIS.Msg.getMsg("AddressMandatory"));
                    VIS.ADialog.info("AddressMandatory");
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }
                if ($chkCustomer.is(":checked") == false && $chkVendor.is(":checked") == false && $chkProspect.is(":checked") == false && $chkEmployee.is(":checked") == false) {
                    alert(VIS.Msg.getMsg("PleaseSelectBusinessType"));
                    if ($isBusy != null) {
                        $isBusy.hide();
                    }
                    $(".vis-apanel-busy").css("visibility", "hidden");
                    return false;
                }
                //if ($selectBPRelation.val() > 0 && $selectBPLocation.val() <= 0) {
                //    alert(VIS.Msg.getMsg("LocationMandatory"));
                //    if ($isBusy != null) {
                //        $isBusy.hide();
                //    }
                //    $(".vis-apanel-busy").css("visibility", "hidden");
                //    return false;
                //}
                var BPtype = null;

                //if (category == "Customer") {
                //    BPtype = VIS.Msg.getMsg("Customer");
                //}
                //else if (category == "Employee") {
                //    BPtype = VIS.Msg.getMsg("Employee");
                //}
                //else if (category == "Vendor") {
                //    BPtype = VIS.Msg.getMsg("Vendor");
                //}
                //else if (category == "Prospect") {
                //    BPtype = VIS.Msg.getMsg("Prospect");
                //}
                BPtype = category;
                url = VIS.Application.contextUrl + "BPartner/AddBPartnerInfo";
                var searchKey = $txtSearchKey.val();
                var name = $txtName.val();
                var name2 = $txtName.val();
                var greeting = $txtGreeting.val();
                var bpgroup = $selectBPGroup.val();
                var bprelation = $selectBPRelation.val();
                var bploaction = $selectBPLocation.val();
                var contact = $txtContact.val();
                var greeting1 = $txtGreeting1.val();
                var title = $txtTitle.val();
                var email = $txtEmail.val();
                var phone = $txtPhone.val();
                var phone2 = $txtPhone.val();
                var fax = $txtFax.val();
                var isCustomer = $chkCustomer.is(":checked");
                var isVendor = $chkVendor.is(":checked");
                var isProspect = $chkProspect.is(":checked");
                var isEmployee = $chkEmployee.is(":checked");
                var uMobile = txtMobile.val();
                var weburl = txtWebUrl.val();
                window.setTimeout(function () {
                    $.ajax({
                        type: "Post",
                        async: false,
                        url: url,
                        dataType: "json",
                        data: {
                            C_BPartner_ID: BPartnerID,
                            searchKey: searchKey,
                            name: name,
                            name2: name2,
                            greeting: greeting,
                            bpGroup: bpgroup,
                            bpRelation: bprelation,
                            bpLocation: bploaction,
                            contact: contact,
                            greeting1: greeting,
                            title: title,
                            email: email,
                            address: ddlAddress,
                            phoneNo: phone,
                            phoneNo2: phone2,
                            fax: fax,
                            windowNo: windowNo, BPtype: BPtype, isCustomer: isCustomer, isVendor: isVendor, isProspect: isProspect, fileName: imageName, mobile: uMobile, webUrl: weburl, isEmployee: isEmployee

                        },
                        success: function (data) {
                            //alert(data);                            
                            //VIS.ADialog.info(JSON.parse(data));

                            VIS.ADialog.info("","",JSON.parse(data));

                            if ($isBusy != null) {
                                $isBusy.hide();
                            }

                            $(".vis-apanel-busy").css("visibility", "hidden");

                        }
                    });
                }, 2);


            }

        };

        function OnSelectionChangeEvent(relationID, windowNo, bpLocationID) {

            var result = null;
            var bplocation = "";
            url = VIS.Application.contextUrl + "BPartner/GetBPLocation?WinNo=" + windowNo + "&bpartnerID=" + relationID;
            $.ajax({
                type: "GET",
                async: false,
                url: url,
                dataType: "json",

                success: function (data) {
                    result = JSON.parse(data);
                    for (var i = 0; i < result.lstBPLocation.length; i++) {
                        bplocation += "<option value='" + result.lstBPLocation[i].ID + "'>" + VIS.Utility.encodeText(result.lstBPLocation[i].Name) + "</option>";

                    }
                    $selectBPLocation.empty();
                    $selectBPLocation.append(bplocation);

                    $selectBPLocation.val(bpLocationID);
                }
            });

        };

        InitBP(windowNo, BPartnerID, BPtype);
    };




    //    function HtmlTable(windowNo, BPartnerID, BPtype, dd, greeting, bpgroup, bplocation, bprelation) {

    //        //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'><tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name2") + "  </div></td><td style='width:300px'><div style='width:95%' class='wsp-BPartner'><input id='txtName2_" + windowNo + "' style='height: 25px;width:100%' type='text'></div></td><td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select>"
    //        //var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td style='border: 1px solid #bdbdbd;border-right: 1px white;'><div style='float:left;margin-right: 15px;margin-bottom: 2px;'>  <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "  </div> <div style='float:right;margin-right: 15px;margin-bottom: 2px;'> <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + "  </div></td>  <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;width:50px'></td> <td style='border: 1px solid #bdbdbd;border-left: 1px white;border-right: 1px white;margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'></div></td><td ><div style='float:right;margin-right: 15px;/* margin-bottom: 2px; */border: 1px solid #bdbdbd;      border-left: 1px white;width: 288px;height: 25px;'>  </div></td></tr>" +
    //        var $bpartnerroot = $("<table id='tableBPartner_" + windowNo + "'> <tr> <td ><div style='margin-top: 0px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPType") + "</div></td><td  colspan='4' ><div style='float:left;margin-right: 15px;margin-bottom: 2px;border: 1px solid #bdbdbd;width:98%;height: 35px;margin-left: 5px;'> <div style='float:left;margin-left:100px;margin-top: 5px;'> <input id='chkCustomer_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Customer") + "</div> <div style='float:left;margin-left:150px;margin-top: 5px;'> <input id='chkProspect_" + windowNo + "' type='checkbox'> " + VIS.Msg.getMsg("Prospect") + "</div> <div style='float:left;margin-left: 200px;margin-top: 5px;'>  <input id='chkVendor_" + windowNo + "' type='checkbox'>  " + VIS.Msg.getMsg("Vendor") + " </div>  </div></td>  </tr>" +
    //            "<tr><td><div style='margin-top: 0px; float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("SearchKey") + " </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory'  id='txtSearchKey_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divSearchKey' style='color:red;visibility:collapse'>*</div></td><td style='width:50px'></td> <td style='margin-left:10px'><div style='margin-top: 0px; float: left;margin-right: 5px;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div></td><td style='width:300px'><div style='margin-right:5px;width:95%' class='wsp-BPartner'><select id='ddlGreeting1_" + windowNo + "' style='width:100%'>" + greeting + "</select></div></td></tr><tr><td><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Name") + "  </div></td><td style='width:300px'><div style='width:95%;float:left' class='wsp-BPartner'><input class='vis-gc-vpanel-table-mandatory' id='txtName_" + windowNo + "' style='height: 25px;width:100%' type='text'></div> <div id='divName' style='color:red;visibility:collapse'>*</div></td>  <td style='width:50px'> </td> <td style='margin-left:10px'><div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPGroup") + "  </div></td><td style='width:300px'><div style='margin-right: 5px;width: 95%' class='wsp-BPartner'><select class='vis-gc-vpanel-table-mandatory' id='ddlBPGroup_" + windowNo + "' style='width: 100%;float:left'>" + bpgroup + "</select></div> <div id='divBPGroup' style='color:red;visibility:collapse'>*</div></td></tr>"
    //+ "<tr>"
    //+ "<td>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPRelation") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div style='width:95%' class='wsp-BPartner'>"
    //  + "<select style='width:100%' id='ddlBPRelation_" + windowNo + "' >"

    //      + bprelation
    //  + "</select>"
    //+ "</div>"
    //+ "</td> <td style='width:50px'></td>"
    //+ "<td style='margin-left:10px'>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("BPLocation") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
    //  + "<select id='ddlBPLoaction_" + windowNo + "' style='width:100%'>"
    //  + "</select>"
    //+ "</div>"
    //+ "</td>"
    //+ "</tr>"
    //+ "<tr>"
    //+ "<td>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Contact") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div style='width:95%;float:left' class='wsp-BPartner'>"
    //  + "<input class='vis-gc-vpanel-table-mandatory' id='txtContact_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
    //+ "</div><div id='divContact_" + windowNo + "' style='color:red;visibility:collapse'>*</div>"
    //+ "</td> <td style='width:50px'></td>"
    //+ "<td style='margin-left:10px'>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Greeting") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
    //  + "<select id='ddlGreeting_" + windowNo + "' style='width: 100%'>"
    //    + greeting
    //  + "</select>"
    //+ "</div>"
    //+ "</td>"
    //+ "</tr>"
    //+ "<tr>"
    //+ "<td>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Title") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div style='width:95%' class='wsp-BPartner'>"
    //  + "<input id='txtTitle_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
    //+ "</div>"
    //+ "</td> <td style='width:50px'></td>"
    //+ "<td style='margin-left:10px'>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("EmailAddress") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
    //  + "<input id='txtEmail_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
    //+ "</div>"
    //+ "</td>"
    //+ "</tr>"
    //+ "<tr>"
    //+ "<td>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Address") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'><div style='width:95%;float:left' id='divAddress_" + windowNo + "' class='wsp-BPartner' ></div> <div id='divAddressValidate_" + windowNo + "' style='color:red;visibility:collapse'>*</div></td> <td style='width:50px'></td>"
    //+ "<td style='margin-left:10px'>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Phone") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div  class='wsp-BPartner' style='margin-right: 5px;width:95%'>"
    //  + "<input id='txtPhone_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
    //+ "</div>"
    //+ "</td>"
    //+ "</tr>"
    //+ "<tr>"
    //+ "<td>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("2ndPhone") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px'>"
    //+ "<div class='wsp-BPartner' style='width:95%;'>"
    //  + "<input id='txtPhone2_" + windowNo + "' style='height: 25px;width:100%' type='text'>"
    //+ "</div>"
    //+ "</td> <td style='width:50px'></td>"
    //+ "<td style='margin-left:10px'>"
    //+ "<div style='float: left;width:125px' class='wsp-BPartner'>" + VIS.Msg.getMsg("Fax") + "  </div>"
    //+ "</td>"
    //+ "<td style='width:300px' >"
    //+ "<div style='margin-bottom:7px;margin-right: 5px;width:95%' class='wsp-BPartner' >"
    //  + "<input id='txtFax_" + windowNo + "' style='height: 25px;width:100%;' type='text'>"
    //+ "</div>"
    //+ "</td>"
    //+ "</tr>"
    //+ "</table>");
    //        var $div = $bpartnerroot.find("#divAddress_" + windowNo);
    //        $selectBPRelation = $bpartnerroot.find("#ddlBPRelation_" + windowNo);
    //        $selectBPLocation = $bpartnerroot.find("#ddlBPLoaction_" + windowNo);
    //        $selectBPGroup = $bpartnerroot.find("#ddlBPGroup_" + windowNo);
    //        $txtName = $bpartnerroot.find("#txtName_" + windowNo);
    //        $txtSearchKey = $bpartnerroot.find("#txtSearchKey_" + windowNo);
    //        $txtGreeting1 = $bpartnerroot.find("#ddlGreeting1_" + windowNo);
    //        $txtName2 = $bpartnerroot.find("#txtName2_" + windowNo);

    //        $txtContact = $bpartnerroot.find("#txtContact_" + windowNo);
    //        $txtGreeting = $bpartnerroot.find("#ddlGreeting_" + windowNo);
    //        $txtTitle = $bpartnerroot.find("#txtTitle_" + windowNo);
    //        $txtEmail = $bpartnerroot.find("#txtEmail_" + windowNo);

    //        $txtPhone = $bpartnerroot.find("#txtPhone_" + windowNo);
    //        $txtPhone2 = $bpartnerroot.find("#txtPhone2_" + windowNo);
    //        $txtFax = $bpartnerroot.find("#txtFax_" + windowNo);
    //        $chkCustomer = $bpartnerroot.find("#chkCustomer_" + windowNo);
    //        $chkVendor = $bpartnerroot.find("#chkVendor_" + windowNo);
    //        $chkProspect = $bpartnerroot.find("#chkProspect_" + windowNo);
    //        txtLoc.setValue(dd.location);
    //        if (BPartnerID > 0) {
    //            //$bpartnerroot.find("#ddlBPRelation_" + windowNo).val(dd.bpRelationID);
    //            //$bpartnerroot.find("#ddlBPGroup_" + windowNo).val(dd.bpGroupID);
    //            //$bpartnerroot.find("#ddlBPLoaction_" + windowNo).val(dd.bpLocationID);


    //            //$bpartnerroot.find("#txtName_" + windowNo).val(dd.name);
    //            //$bpartnerroot.find("#txtSearchKey_" + windowNo).val(dd.searchKey);
    //            //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val();
    //            //$bpartnerroot.find("#txtName2_" + windowNo).val(dd.name2);

    //            //$bpartnerroot.find("#txtContact_" + windowNo).val(dd.contact);
    //            //$bpartnerroot.find("#ddlGreeting_" + windowNo).val();
    //            //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);
    //            //$bpartnerroot.find("#txtEmail_" + windowNo).val(dd.email);
    //            ////txtLoc.getValue();
    //            //$bpartnerroot.find("#txtPhone_" + windowNo).val(dd.phoneNo);
    //            //$bpartnerroot.find("#txtPhone2_" + windowNo).val(dd.phoneNo2);
    //            //$bpartnerroot.find("#txtFax_" + windowNo).val(dd.fax);
    //            //$bpartnerroot.find("#ddlGreeting1_" + windowNo).val(dd.greeting);
    //            //$bpartnerroot.find("#ddlGreeting_" + windowNo).val(dd.greeting1);
    //            //$bpartnerroot.find("#txtTitle_" + windowNo).val(dd.title);




    //            $selectBPRelation.val(dd.bpRelationID);
    //            $selectBPGroup.val(dd.bpGroupID);
    //            $selectBPLocation.val(dd.bpLocationID);


    //            $txtName.val(VIS.Utility.decodeText(dd.name));
    //            $txtSearchKey.val(VIS.Utility.decodeText(dd.searchKey));
    //            $txtGreeting1.val();
    //            $txtName2.val(VIS.Utility.decodeText(dd.name2));

    //            $txtContact.val(VIS.Utility.decodeText(dd.contact));
    //            $txtGreeting.val();
    //            $txtTitle.val(VIS.Utility.decodeText(dd.title));
    //            $txtEmail.val(VIS.Utility.decodeText(dd.email));
    //            //txtLoc.getValue();
    //            $txtPhone.val(VIS.Utility.decodeText(dd.phoneNo));
    //            $txtPhone2.val(VIS.Utility.decodeText(dd.phoneNo2));
    //            $txtFax.val(VIS.Utility.decodeText(dd.fax));
    //            $txtGreeting.val(dd.greeting);
    //            $txtGreeting1.val(dd.greeting1);
    //            txtLoc.setValue(dd.location);
    //            if (dd.isCustomer) {
    //                $chkCustomer.attr("checked", "checked");
    //            }
    //            if (dd.isVendor) {
    //                $chkVendor.attr("checked", "checked");
    //            }
    //            OnSelectionChangeEvent($selectBPRelation.val(), windowNo, dd.bpLocationID);
    //        }
    //        else if (BPRelationID > 0 && BPGroupID > 0) {
    //            $bpartnerroot.find("#ddlBPRelation_" + windowNo).val(BPRelationID);
    //            $bpartnerroot.find("#ddlBPGroup_" + windowNo).val(BPGroupID);
    //            OnSelectionChangeEvent($selectBPRelation.val(), windowNo, dd.bpLocationID);
    //        }


    //        txtLoc.getBtn(0).css("width", "10%");
    //        txtLoc.getBtn(0).css("height", "30px");
    //        txtLoc.getBtn(0).css("padding", "0px");
    //        //txtLoc.getBtn(0).css("border-radius", "none");
    //        txtLoc.getBtn(1).css("width", "10%");
    //        txtLoc.getBtn(1).css("height", "30px");
    //        txtLoc.getBtn(1).css("padding", "0px");
    //        //txtLoc.getBtn(1).css("border-radius", "none");
    //        $div.append(txtLoc.getControl().css("width", "80%")).append(txtLoc.getBtn(0)).append(txtLoc.getBtn(1));
    //        $txtAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Input");
    //        $btnAddress = $bpartnerroot.find("#divAddress_" + windowNo).find("Button").eq(1);
    //        SaveBPartner(windowNo, BPtype, BPartnerID);
    //        $selectBPRelation.on("change", function () {

    //            OnSelectionChangeEvent($selectBPRelation.val(), windowNo);
    //        });
    //        $txtSearchKey.on("focusout", function () {

    //            if ($txtSearchKey.val() == "") {
    //                $("#divSearchKey").css("visibility", "visible");
    //            }
    //            else {
    //                $("#divSearchKey").css("visibility", "collapse ");
    //            }
    //        });
    //        $txtName.on("focusout", function () {

    //            if ($txtName.val() == "") {
    //                $("#divName").css("visibility", "visible");
    //            }
    //            else {
    //                $("#divName").css("visibility", "collapse");
    //            }
    //        });
    //        $selectBPGroup.on("focusout", function () {

    //            if ($selectBPGroup.val() == "0" || $selectBPGroup.val() == "") {
    //                $("#divBPGroup").css("visibility", "visible");
    //            }
    //            else {
    //                $("#divBPGroup").css("visibility", "collapse ");
    //            }
    //        });

    //        $txtAddress.on("focusout", function () {

    //            if (txtLoc.getValue() <= 0) {
    //                $("#divAddressValidate_" + windowNo).css("visibility", "visible");
    //            }
    //            else {
    //                $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
    //            }
    //        });
    //        $btnAddress.on("focusout", function () {

    //            if (txtLoc.getValue() <= 0) {
    //                $("#divAddressValidate_" + windowNo).css("visibility", "visible");
    //            }
    //            else {
    //                $("#divAddressValidate_" + windowNo).css("visibility", "collapse ");
    //            }
    //        });
    //        $txtContact.on("focusout", function () {

    //            if ($txtContact.val() == "") {
    //                $("#divContact_" + windowNo).css("visibility", "visible");
    //            }
    //            else {
    //                $("#divContact_" + windowNo).css("visibility", "collapse");
    //            }
    //        });
    //        return $bpartnerroot;
    //    };




})(VIS, jQuery);