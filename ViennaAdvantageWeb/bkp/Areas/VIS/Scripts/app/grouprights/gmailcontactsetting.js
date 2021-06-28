//(function (VIS, $) {

//    VIS.contactSettings = function (self) {

//        var settingsDialog = null;
//        var c = null;
//        var gmailcontactdialog = null;
//        var grdCols = [];
//        var dData = [];
//        var originalData = [];
//        var $mainTable = null;
//        var $tr0 = null;
//        var $td01 = null;
//        var $lblUsername = null;
//        var $td02 = null;
//        var $txtUsername = null;
//        var $tr1 = null;
//        var $td11 = null;
//        var $lblPassword = null;
//        var $td12 = null;
//        var $txtPassword = null;
//        var $tr2 = null;
//        var $td21 = null;
//        var $lblRole = null;
//        var $td22 = null;
//        var $cmbRole = null;
//        var $tr3 = null;
//        var $td31 = null;
//        var $lblchkExisting;
//        var $chkbxUpdateExisting = null;
//        var $lblchkShowLinkedContacts = null;
//        var $chkbxShowLinkedContacts = null;
//        var chkShowLinkedContacts = false;
//        var chkUpdateExistingRecords = false;
//        var $contactsetting = $('<div></div>');
//        var $dGrid = null;
//        var gContactHtml = "";
//        var bpAddress = [];
//        var bpGroupSource = [];
//        var role = "";
//        var bpgroup = "";
//        var org = "";
//        var $btnImport;
//        var $btnVerify;
//        var $btnRequery;
//        var $btnOK;
//        var $isBusy = null;
//        var $isBusyImg = null;
//        var $btnCancel;
//        var windowWidth = $(window).width();
//        var windowHeight = $(window).height();
//        var lblUmandatory = $('<div style="color:red;margin-left:1px;display:inline;">*</div>');
//        var lblPmandatory = $('<div style="color:red;margin-left:1px;display:inline;">*</div>');
//        var bsyDiv = $("<div class='vis-apanel-busy'>");  //Busy Indicator
//        bsyDiv.css("width", "98%");
//        bsyDiv.css("height", "98%");
//        bsyDiv.css('text-align', 'center');
//        bsyDiv.css("position", "absolute");
//        initialize();



//        function initialize() {

//            $mainTable = $('<table style="border-collapse:separate;margin-left:20px;margin-top:15px;"></table>');
//            $tr0 = $('<tr></tr>');
//            $trlbl1 = $('<tr></tr>');
//            $td02 = $('<td></td>');
//            var $divUsername = $('<div style="float:left"></div>');
//            $txtUsername = $("<input type='text' name='txtUsername' id='txtUsername_" + self.windowNo + "' style='margin-bottom:10px;width:292px;height:40px;' placeholder='" + VIS.Msg.getMsg("Username") + "'></input>");
//            $divUsername.append($txtUsername);
//            $td02.append($divUsername);
//            $td02.append(lblUmandatory);
//            $tr0.append($td02);
//            $mainTable.append($tr0);
//            $tr1 = $('<tr></tr>');
//            $trlbl2 = $('<tr></tr>');
//            $td12 = $('<td></td>');
//            var $divPassword = $('<div style="float:left"></div>');
//            $txtPassword = $("<input type='password' name='txtPassword' id='txtPassword_" + self.windowNo + "' style='margin-bottom:10px;width:292px;height:40px;' placeholder='" + VIS.Msg.getMsg("Password") + "'></input>");
//            $divPassword.append($txtPassword);
//            $td12.append($divPassword);
//            $td12.append(lblPmandatory);
//            $tr1.append($td12);
//            $mainTable.append($tr1);
//            $isBusy = $("#busy_1");
//            $isBusyImg = $isBusy.children();
//            $isBusy.hide();
//            load();

//        }

//        //********************
//        //Get Percentage 
//        //param1=value whose percentage to get
//        //param2=how many percentage of param1 you want 
//        //e.g if you want 50% of 500 then call as getpercentage(500,50)
//        //********************
//        function getpercentage(value, percentile) {
//            return (parseInt(value) * parseInt(percentile)) / 100;
//        }




//        //**************************
//        //Load function Open Gmail Setting Dialog Box
//        //**************************
//        function load() {

//            getrole();

//            // Open Settings Dialog Box
//            bsyDiv[0].style.visibility = 'hidden';
//            settingsDialog = new VIS.ChildDialog();
//            settingsDialog.setContent($contactsetting);
//            //settingsDialog.setHeight(365);
//            // settingsDialog.setWidth(352);
//            settingsDialog.setTitle(VIS.Msg.getMsg("GmailContactSettings"));
//            settingsDialog.setEnableResize(false);
//            settingsDialog.setModal(true);
//            settingsDialog.show();
//            settingsDialog.hidebuttons();    //buttons are hidden because custom buttons are used in this dialog
//            getsaveddetail();
//            if ($txtUsername.val().length == 0) {
//                lblUmandatory[0].style.visibility = 'visible';
//            }
//            else {
//                lblUmandatory[0].style.visibility = 'hidden';
//            }
//            if ($txtPassword.val().length == 0) {
//                lblPmandatory[0].style.visibility = 'visible';
//            }
//            else {
//                lblPmandatory[0].style.visibility = 'hidden';
//            }
//            events();
//        };


//        //************************************
//        // This function is used to get saved credientials 
//        //************************************
//        function getsaveddetail() {
//            var url = VIS.Application.contextUrl + "GmailContacts/GetSavedDetail";
//            $.ajax({
//                type: "GET",
//                async: false,
//                url: url,
//                dataType: "json",
//                success: function (data) {

//                    var dd = JSON.parse(data);
//                    //if settings are saved in database(WSP_GmalConfigration)then set those values in setting dialog Box controls
//                    $txtUsername.val(dd["Username"]);
//                    $txtPassword.val(dd["Password"]);
//                    if (dd["Role"] != "") {
//                        $("#ddlRole_" + self.windowNo).val(dd["Role"])
//                    }
//                    var update = dd["IsUpdate"];
//                    if (update.toString().toLowerCase() == "false") {
//                        ($chkbxUpdateExisting).prop("checked", false);
//                        chkUpdateExistingRecords = false;
//                    }
//                    else {
//                        ($chkbxUpdateExisting).prop("checked", true);
//                        chkUpdateExistingRecords = true;
//                    }


//                }
//            });
//        };




//        //***********************************
//        //Events of Gmail Setting Dialog Box
//        //***********************************
//        function events() {
//            settingsDialog.onOkClick = function (e) {

//                // bsyDiv[0].style.visibility = 'visible';
//                var username = $txtUsername.val();
//                var password = $txtPassword.val();
//                $isBusy.show();
//                var h = windowHeight * (40 / 100);
//                var w = windowWidth * (50 / 100);
//                $isBusyImg.css("margin-top", h);
//                $isBusyImg.css("margin-left", w);
//                window.setTimeout(function () {
//                    if (username == null || username.length == 0) {
//                        lblUmandatory[0].style.visibility = 'visible';
//                        // alert(VIS.Msg.getMsg("UsernameNotFound"));
//                        $isBusy.hide();
//                        return false;

//                    }
//                    else if (password == null || password.length == 0) {
//                        lblPmandatory[0].style.visibility = 'visible';
//                        $isBusy.hide();
//                        return false;

//                    }
//                    else {
//                        //Open Gmail Contact form in CFrame
//                        c = new VIS.CFrame();
//                        var gmailContactForm = new WSP.gmailcontacts(username, password, $("#ddlRole_" + self.windowNo).val(), Boolean(chkUpdateExistingRecords), c.windowNo);
//                        c.setName(VIS.Msg.getMsg("MyGmailContacts"));
//                        c.setTitle(VIS.Msg.getMsg("MyGmailContacts"));
//                        c.hideHeader(true);
//                        c.setContent(gmailContactForm);
//                        settingsDialog.close();
//                        c.show();
//                        gmailContactForm.loadSavedData();
//                        $isBusy.hide();
//                    }

//                }, 500);

//            };


//            settingsDialog.onCancelClick = function () {

//                this.disposeComponent();
//            };
//            //************
//            // OK Button Click
//            $btnOK.on("click", function (e) {
//                settingsDialog.onOkClick(e);
//                this.disposeComponent();
//            });
//            //************
//            // Cancel Button Click
//            $btnCancel.on("click", function () {
//                settingsDialog.close();
//                this.disposeComponent();
//            });
//            settingsDialog.onClose = function () {

//                // disposeComponent();
//            };
//            //***********
//            //CheckBox Is Update Existing Records Click
//            $chkbxUpdateExisting.on("click", function (event) {

//                chkUpdateExistingRecords = event.originalEvent.target.checked;;
//            });
//            $txtUsername.on("focus", function (event) {

//                lblUmandatory[0].style.visibility = 'hidden';
//            });
//            $txtUsername.on("blur", function (event) {

//                if ($txtUsername.val().length == 0) {
//                    lblUmandatory[0].style.visibility = 'visible';
//                }
//                else {
//                    lblUmandatory[0].style.visibility = 'hidden';
//                }
//            });
//            $txtPassword.on("focus", function (event) {

//                lblPmandatory[0].style.visibility = 'hidden';
//            });
//            $txtPassword.on("blur", function (event) {

//                if ($txtPassword.val().length == 0) {
//                    lblPmandatory[0].style.visibility = 'visible';
//                }
//                else {
//                    lblPmandatory[0].style.visibility = 'hidden';
//                }
//            });


//        }




//        //**********************
//        //Get Roles in combobox
//        //*********************
//        function getrole() {
//            var url = VIS.Application.contextUrl + "GmailContacts/GetRole";
//            $.ajax({
//                type: "GET",
//                async: false,
//                url: url,
//                dataType: "json",

//                success: function (data) {

//                    var dd = JSON.parse(data);
//                    role += "<option value='-1'  selected='selected'>" + VIS.Msg.getMsg("SelectRole") + "</option>";
//                    for (var i = 0; i < dd.length; i++) {
//                        role += "<option value='" + dd[i]["RoleID"] + "'>" + dd[i]["RoleName"] + "</option>";
//                    }
//                    appendContactSettings();
//                }
//            });
//        };


//        //****************
//        //Append HTML of Form
//        //*****************
//        function appendContactSettings() {
//            $tr2 = $('<tr></tr>');
//            $td22 = $('<td></td>');
//            $cmbRole = $("<select id='ddlRole_" + self.windowNo + "' style='width:292px;height:40px;'>"
//                             + role
//                            + "</select>");
//            $tr2.append($cmbRole);
//            $mainTable.append($tr2);
//            $tr3 = $('<tr></tr>');
//            $lblchkExisting = $("<span>" + VIS.Msg.getMsg("UpdateExisting") + " </span>");
//            $tdchkbox = $('<td></td>');
//            $chkbxUpdateExisting = $("<input style='margin-top:15px;margin-right:5px;' type='checkbox' id='chkUpdateExisting'  style='margin-top:10px;'/>");
//            $tr3.append($chkbxUpdateExisting);
//            $tr3.append($lblchkExisting);
//            $mainTable.append($tr3);
//            var $divbuttons = $('<div style="float:right;display:inline;margin-right:5px;margin-top:15px;"></div>');
//            var $divbtnOK = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            $btnOK = $('<input style="margin-left:2%;background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:left;" type="submit" value="' + VIS.Msg.getMsg("OK") + '" ></input>');
//            $divbtnOK.append($btnOK);
//            var $divbtnCancel = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            $btnCancel = $('<input style="margin-left:2%;background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:left;" type="submit" value="' + VIS.Msg.getMsg("Cancel") + '" ></input>');
//            $divbtnCancel.append($btnCancel);
//            $divbuttons.append($divbtnOK);
//            $divbuttons.append($divbtnCancel);
//            $contactsetting.append(($mainTable));
//            $contactsetting.append($divbuttons);
//        };

//        //********************
//        //Dispose the variables
//        //*********************
//        this.disposeComponent = function () {
//            settingsDialog = null;
//            c = null;
//            gmailcontactdialog = null;
//            grdCols = null;
//            dData = null;
//            originalData = null;
//            $mainTable = null;
//            $tr0 = null;
//            $td01 = null;
//            $lblUsername = null;
//            $td02 = null;
//            $txtUsername = null;
//            $tr1 = null;
//            $td11 = null;
//            $lblPassword = null;
//            $td12 = null;
//            $txtPassword = null;
//            $tr2 = null;
//            $td21 = null;
//            $lblRole = null;
//            $td22 = null;
//            $cmbRole = null;
//            $tr3 = null;
//            $td31 = null;
//            $lblchkExisting = null;
//            $chkbxUpdateExisting = null;
//            $lblchkShowLinkedContacts = null;
//            $chkbxShowLinkedContacts = null;
//            chkShowLinkedContacts = null;
//            chkUpdateExistingRecords = null;
//            $contactsetting = null;
//            $dGrid = null;
//            gContactHtml = null;
//            bpAddress = null;
//            bpGroupSource = null;
//            role = null;
//            bpgroup = null;
//            org = null;
//            $btnImport = null;
//            $btnVerify = null;
//            $btnRequery = null;
//            $btnOK = null;
//            $isBusy = null;
//            $isBusyImg = null;
//            $btnCancel = null;
//            windowWidth = null;
//            windowHeight = null;
//            lblUmandatory = null;
//            lblPmandatory = null;
//            bsyDiv = null;
//        };




//    };
//    VIS.contactSettings.prototype.dispose = function (frame) {
//        /*CleanUp Code */
//        //dispose this component     

//        //call frame dispose function
//        if (frame != null)
//            frame.dispose();
//        frame = null;
//    };

//})(VIS, jQuery);