; (function (VIS, $) {
    var windowNo = 0;
    var windowWidth = $(document).width();
    var windowHeight = $(document).height();
    var $contactInfoRoot = null;
    var ad_userID = 0;
    var url = null;
    var NavigateURL = null;
    var $divDots = null;
    //var $mainUserInfo = null;
    var animateTime = null;
    var kbSize = null;
    var maxSize = null;
    var contactInfoDialog = null;
    var windowWidth = $(document).width();
    var windowHeight = $(document).height();
    VIS.ContactInfo = function (userID, winNo) {
        var sql = "SELECT COUNT(*) FROM AD_Table WHERE TableName='C_BPartner'";
        var dr = VIS.DB.executeReader(sql, null);
        while (dr.read()) {
            if (parseInt(dr.getString(0)) == 0) {
                VIS.ADialog.error('VIS_NotSupported');
                self.dispose();
                return;

            }
        }
        
        ad_userID = userID;

        windowNo = winNo;
        $(".vis-apanel-busy").css("visibility", "visible");

        url = VIS.Application.contextUrl + "WSP/Contacts/UserContactInfo?WinNo=" + windowNo + "&userID=" + ad_userID + "&isPartialViewLoad=" + true;

        LoadHtmlDesign(ad_userID, url);

        this.show = function () {

            if (!window.WSP) {
                alert("please download WSP !!!");
                $(".vis-apanel-busy").css("visibility", "hidden");
                return;
            }
            
            window.setTimeout(function () {
                contactInfoDialog = new VIS.ChildDialog();

                contactInfoDialog.setContent($contactInfoRoot);
                contactInfoDialog.setWidth(610);
                //contactInfoDialog.setHeight(700);
                contactInfoDialog.setTitle(VIS.Msg.getMsg("ContactInfo"));
                contactInfoDialog.setEnableResize(false);
                contactInfoDialog.setModal(true);

                contactInfoDialog.show();
                //if (windowWidth <= 1024) {
                //    contactInfoDialog.getRoot().dialog({ position: [200, 0] });
                //}
                //else {
                contactInfoDialog.getRoot().dialog({ position: [400, 0] });
                //}
                contactInfoDialog.onClose = function () {
                    dispose();
                };
                $.ajax({
                    type: "GET",
                    async: false,
                    url: url,
                    dataType: "json",

                    success: function (data) {
                        var result = JSON.parse(data);
                        

                        var category = "";
                        if (result[0].Customer != "") {
                            category += result[0].Customer;
                        }
                        if (result[0].Vendor != "") {
                            if (result[0].Customer != "") {
                                category += ",";
                            }
                            category += result[0].Vendor;
                        }
                        if (result[0].Employee != "") {
                            if (result[0].Customer != "" || result[0].Vendor != "") {
                                category += ",";
                            }
                            category += result[0].Employee;
                        }
                        if (result[0].Prospect != "") {
                            if (result[0].Customer != "" || result[0].Vendor != "" || result[0].Employee != "") {
                                category += ",";
                            }
                            category += result[0].Prospect;
                        }

                        var $divRole = $contactInfoRoot.find("#divRole_" + windowNo)
                        var $divTeam = $contactInfoRoot.find("#divTeam_" + windowNo);
                        var $divContactDetails = $contactInfoRoot.find("#divContactDetails_" + windowNo)
                        var $sliderDotImg = $contactInfoRoot.find("#dotImg_" + windowNo);
                        var $slideruserID = $contactInfoRoot.find("#userID_" + windowNo);
                        var $slideruserTableID = $contactInfoRoot.find("#userTableID_" + windowNo);
                        var $sliderhduserName = $contactInfoRoot.find("#hduserName_" + windowNo);
                        var $slideruserName = $contactInfoRoot.find("#userName_" + windowNo);
                        var $sliderBpID = $contactInfoRoot.find("#BPID_" + windowNo);
                        var $slideruBPGroupID = $contactInfoRoot.find("#BPGroupID_" + windowNo);
                        var $slidercompanyName = $contactInfoRoot.find("#companyName_" + windowNo);
                        var $sliderpContactTypeCategory = $contactInfoRoot.find("#pContactTypeCategory_" + windowNo);
                        var $slidermobileImg = $contactInfoRoot.find("#mobileImg_" + windowNo);
                        var $sliderteam = $contactInfoRoot.find("#team_" + windowNo);
                        var $sliderteamNotFound = $contactInfoRoot.find("#teamNotFound_" + windowNo);
                        var $slideraddress = $contactInfoRoot.find("#address_" + windowNo);
                        var $slidercountry = $contactInfoRoot.find("#country_" + windowNo);
                        var $sliderpostal = $contactInfoRoot.find("#postal_" + windowNo);
                        var $sliderphone = $contactInfoRoot.find("#phone_" + windowNo);
                        var $slideruserEmail = $contactInfoRoot.find("#userEmail_" + windowNo);
                        var $slideremailNotFound = $contactInfoRoot.find("#emailNotFound_" + windowNo);
                        var $slidercompanyDetails = $contactInfoRoot.find("#divcompanyDetails_" + windowNo);
                        var $slidercompanyNotFound = $contactInfoRoot.find("#companyNotFound" + windowNo);
                        var $sliderrole = $contactInfoRoot.find("#role_" + windowNo);
                        var $sliderroleNotFound = $contactInfoRoot.find("#roleNotFound_" + windowNo);
                        var $sliderbigUserImage = $contactInfoRoot.find("#bigUserImage_" + windowNo);
                        var $sliderdivBirthday = $contactInfoRoot.find("#divBirthday_" + windowNo);
                        var $sliderdivbigUserImage = $contactInfoRoot.find("#userBigImg_" + windowNo);
                        var $editContact = $contactInfoRoot.find("#edit_" + windowNo);
                        $divDots = $contactInfoRoot.find("#WSP_dots_" + windowNo);
                        var $sendSms = $contactInfoRoot.find("#sendSMS_" + windowNo);
                        var $email = $contactInfoRoot.find("#email_" + windowNo);
                        var $BPInfo = $contactInfoRoot.find("#BPInfo_" + windowNo);
                        var $createOrder = $contactInfoRoot.find("#createOrder_" + windowNo);
                        var $addBPartner = $contactInfoRoot.find("#addBPartner_" + windowNo);
                        var $history = $contactInfoRoot.find("#History_" + windowNo);
                        var $appointment = $contactInfoRoot.find("#appointment_" + windowNo);
                        var $activity = $contactInfoRoot.find("#activity_" + windowNo);
                        var $importContacts = $contactInfoRoot.find("#ImportContacts_" + windowNo);
                        var $fileUpload = $contactInfoRoot.find("#wsp-file-input_" + windowNo);
                        var $mobilefileUpload = $contactInfoRoot.find("#wsp-fileinput_" + windowNo);
                        var $bigUserImage = $contactInfoRoot.find(".WSP_profile_big_img");
                        var $mobileImg = $contactInfoRoot.find(".WSP_profile_big_img_mobile");
                        var $showAddressOnGoogleMap = null;
                        var $mainUserInfo = $contactInfoRoot.find("#mainUserInfoDiv_" + windowNo);
                        //var $contactInfoRoot = null;

                        var isTeamExist = false;
                        var isContactDetails = false;
                        var isRole = false;
                        var result = JSON.parse(data);
                        var strRole = "";
                        var strTeams = "";
                        $slideruserID.empty();
                        $slideruserID.append(result[0].UserID);
                        $sliderhduserName.empty();
                        $sliderhduserName.append(result[0].FirstName);
                        $sliderhduserName.hide();
                        $slideruserName.empty();
                        $sliderDotImg.empty();
                        if (category == "" || category == null) {
                            $editContact.removeAttr('href');
                            $editContact.css('cursor', 'auto');
                            $editContact.find("p").css('color', 'black');
                            $BPInfo.removeAttr('href');
                            $BPInfo.css('cursor', 'auto');
                            $BPInfo.find("p").css('color', 'black');
                        }
                        else {
                            $editContact.attr('href', 'javascript:void(0)');
                            $editContact.css('cursor', 'pointer');
                            $editContact.removeAttr("style");
                            $BPInfo.attr('href', 'javascript:void(0)');
                            $BPInfo.css('cursor', 'pointer');
                            $BPInfo.removeAttr("style");
                        }
                        if (windowWidth >= 1000 && windowWidth <= 1024) {
                            if (result[0].FirstName.length > 15) {
                                $sliderDotImg.show();
                                $slideruserName.append(result[0].FirstName.substring(0, 15)).append($sliderDotImg.append("..."));
                            }
                            else {
                                $slideruserName.append(result[0].FirstName);
                            }
                        }
                        else {
                            if (result[0].FirstName.length > 20) {
                                $sliderDotImg.show();
                                $slideruserName.append(result[0].FirstName.substring(0, 20)).append($sliderDotImg.append("..."));
                            }
                            else {
                                $slideruserName.append(result[0].FirstName);
                            }
                        }
                        $sliderBpID.empty();
                        
                        $sliderBpID.append(result[0].BPartnerID);
                        $slideruserTableID.append(result[0].TableID);
                        $slideruBPGroupID.empty();
                        $slideruBPGroupID.append(result[0].BPGroupID);
                        $slidercompanyName.empty();
                        $slidercompanyName.append(result[0].CompanyName);
                        $sliderpContactTypeCategory.empty();
                        $sliderpContactTypeCategory.append(category);
                        $slidermobileImg.removeAttr("src").attr("src", result[0].Image);
                        $divTeam.empty();
                        $divTeam.append("<p>" + VIS.Msg.getMsg("Teams") + "</p>");
                        for (var i = 0; i < result.length; i++) {
                            if (result[i].Team != "" && result[i].Team != null) {
                                isTeamExist = true;
                                if (i == 0 || strTeams == "") {
                                    strTeams += result[i].Team;
                                }
                                else { strTeams += ", " + result[i].Team; }

                                //  $divTeam.append(" <p id='team_" + windowNo + "' style='word-break: break-all'>" + result[i].Team + "</p>");
                            }
                        }
                        if (!isTeamExist) {

                            $divTeam.append(" <p  id='teamNotFound_" + windowNo + "' style='word-break: break-all; font-style: italic;'>" + VIS.Msg.getMsg("NotFound") + "</p>");
                        }
                        if (isTeamExist) {
                            $divTeam.append(" <p id='team_" + windowNo + "' >" + strTeams + "</p>");
                        }
                        $slideraddress.empty();
                        $divContactDetails.empty();
                        $divContactDetails.append("<p style='word-break: break-all;'> " + VIS.Msg.getMsg("ContactDetails") + " <a href='javascript:void(0)'> <img style='margin-bottom:3px' id='showAddressOnGoogleMap_" + windowNo + "' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/ToLink20.png'> </a></p> ");
                        for (var i = 0; i < result.length; i++) {
                            if (result[i].FirstName != "") {

                                continue;
                            }
                            if (result[i].Address != "" && result[i].Address != null) {
                                isContactDetails = true;

                                $divContactDetails.append("<p id='address_" + windowNo + "' style='word-break: break-all'>" + result[i].Address + "</p>");
                            }
                        }

                        $slidercountry.empty();
                        if (result[0].CountryName != "" && result[0].CountryName != null) {
                            isContactDetails = true;
                            $divContactDetails.append("<p id='country_" + windowNo + "' style='word-break: break-all'>" + result[0].City + "(" + result[0].CountryName + ")</p> ");
                        }
                        $sliderpostal.empty();
                        if (result[0].Postal != "" && result[0].Postal != null) {
                            isContactDetails = true;
                            $divContactDetails.append("<p id='postal_" + windowNo + "' style='word-break: break-all'>" + VIS.Msg.getMsg("ZipCode") + ": " + result[0].Postal + "</p>");
                        }
                        $sliderphone.empty();
                        for (var i = 0; i < result.length; i++) {
                            if (result[i].FirstName != "") {

                                continue;
                            }
                            if (result[i].PhoneNo != "" && result[i].PhoneNo != null) {
                                isContactDetails = true;
                                if (windowWidth <= 1024) {
                                    $divContactDetails.append("<a href='tel:" + result[i].PhoneNo + "'><p id='phone_" + windowNo + "' style='word-break: break-all'><span class='wsp-caller'><img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/caller.png'></span>" + result[i].PhoneNo + "</p></a>");
                                }

                                else {

                                    $divContactDetails.append("<p id='phone_" + windowNo + "' style='word-break: break-all'><span class='wsp-caller'><img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/caller.png'></span>" + result[i].PhoneNo + "</p>");
                                }
                            }

                        }
                        $slideruserEmail.empty();
                        if (result[0].Email != "" && result[0].Email != null) {
                            isContactDetails = true;
                            $divContactDetails.append("<p id='userEmail_" + windowNo + "' style='word-break: break-all;display: inline-flex;margin-top: 0;'> <span class='wsp-caller'><img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/sms.png'></span>" + result[0].Email + "</p>");
                        }
                        if (!isContactDetails) {

                        }

                        $slidercompanyDetails.empty();
                        $slidercompanyDetails.append("<p>" + VIS.Msg.getMsg("Company") + "</p>");
                        $slidercompanyDetails.append("<p id='companyDetails_" + windowNo + "' style='word-break: break-all'>" + result[0].CompanyName + "</p>");
                        $slidercompanyNotFound.empty();
                        if (result[0].CompanyName != "" && result[0].CompanyName != null) {
                            $slidercompanyNotFound.append(VIS.Msg.getMsg("NotFound"));
                        }

                        $divRole.empty();
                        $divRole.append("<p>" + VIS.Msg.getMsg("RoleAssign") + "</p>");
                        for (var i = 0; i < result.length; i++) {

                            if (result[i].Role != "" && result[i].Role != null) {
                                isRole = true;
                                if (i == 0) {
                                    strRole += result[i].Role;
                                }
                                else {
                                    strRole += ", " + result[i].Role;
                                }
                                //if (i == 0) {
                                //    $divRole.append("<p id='role_" + windowNo + "' style='word-break: break-all'>" + result[i].Role + "</p>");
                                //}
                                //else {
                                //    $divRole.find("p").append();
                                //}
                            }
                        }
                        if (isRole) {
                            $divRole.append("<p id='role_" + windowNo + "'>" + strRole + "</p>");
                        }
                        $sliderroleNotFound.empty();
                        if (!isRole) {
                            $divRole.append("<p id='roleNotFound_" + windowNo + "' style='word-break: break-all; font-style: italic;'>" + VIS.Msg.getMsg("NotFound") + "</p>");
                        }
                        if (result[0].Image.indexOf("User") > 0) {
                            $sliderbigUserImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/home/User.png");
                        }
                        else {
                            $sliderbigUserImage.removeAttr("src").attr("src", result[0].Image);
                        }
                        $sliderdivBirthday.empty();

                        var now = new Date();
                        var userDOB = new Date(result[0].Birthday);
                        var currentDate = now.getMonth() + 1 + "/" + now.getDate();
                        var userD = userDOB.getMonth() + 1 + "/" + userDOB.getDate();
                        if (result[0].Birthday != "") {
                            $sliderdivBirthday.show();


                            $sliderdivBirthday.css("border", "1px solid #1886BF");
                            var $birthdayDiv = $("<div class='wsp-birthdat_text'>");
                            if (userD == currentDate) {
                                $birthdayDiv.append("<p> " + VIS.Msg.getMsg("Today") + " " + VIS.Msg.getMsg("is") + " <span class='wsp-birthday_text_highlight'>" + result[0].FirstName + "</span> " + VIS.Msg.getMsg("Birthday") + "</p>");
                            }
                            $birthdayDiv.append("<p>" + result[0].FirstName + " " + VIS.Msg.getMsg("Birthday") + " " + VIS.Msg.getMsg("on") + "  <span class='wsp-birthday_text_highlight'>" + result[0].Birthday + "</span></p> </div>");
                            $sliderdivBirthday.append($birthdayDiv);
                        }
                        else {
                            $sliderdivBirthday.hide();
                        }


                        $sliderDotImg.on("click", function (e) {


                            $popUp.show();
                            // var $userNameToolTip = $contactInfoRoot.find("<div style='width:0;height:0;border-bottom: 80px solid #37A047;border-left: 50px solid transparent;border-right: 50px solid transparent;'> </div> <DIV id='PopUp' style='position: absolute; border-radius: 5px;left: 100px; top: 50px; border: solid black 1px; padding: 10px; background-color: rgb(255,253,208); text-align: justify; font-size: 12px; width: auto;' ><SPAN id='PopUpText'>" + hdUserName + "</SPAN></DIV>");
                            var $userNameToolTip = $contactInfoRoot.find("<div class='arrow_box'> <p style='margin-left:5px;margin-top:5px;margin-right:5px'>" + $sliderhduserName.text() + "</p></div>");
                            $popUp.append($userNameToolTip);
                            $popUp.delay(2000).fadeOut();
                            e.stopPropagation();
                        });

                        if ($divDots != null) {
                            $divDots.on("click", function (e) {


                                $mainUserInfo.animate({ right: '-1000px' }, animateTime);
                                e.stopPropagation();

                            });
                        }
                        $(".ui-dialog-content").on("click", function (e) {
                            
                            $("#w2ui-overlay").hide();
                            e.stopPropagation();
                        });
                        //$sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                        //$mainUserInfo.animate({ right: '0px' }, animateTime);
                        //$mainUserInfo.css("display", "block");

                        //var img = new Image();
                        //img.onload = function () {


                        //    if (ismobile) {
                        //        // $sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                        //        if (img.width >= 320) {


                        //            $sliderbigUserImage.css("width", "100%");

                        //            //$sliderbigUserImage.css("height", img.height - (img.width - $sliderdivbigUserImage.width()));
                        //            var val = (img.width * 100) / img.height;
                        //            var finalHeight = ($sliderbigUserImage.width() * 100) / val;
                        //            $sliderbigUserImage.css("height", finalHeight);

                        //            // $sliderbigUserImage.css("height", img.height);
                        //        }
                        //        else {

                        //            $sliderbigUserImage.css("width", img.width);
                        //            $sliderbigUserImage.css("height", img.height);

                        //        }
                        //    }

                        //};
                        //img.src = $sliderbigUserImage.attr('src');
                        
                        //for (var i = 0; i < $(".vis-awindow").length; i++) {
                        //    if ($(".vis-awindow").eq(i).css("display") == "table") {
                        //        var $seletectWindow = $(".vis-awindow").eq(i);
                        //        $seletectWindow.find(".vis-apanel-table tr:first").find("#contactInfo_" + windowNo).remove();
                        //        $seletectWindow.find(".vis-apanel-table tr:first").append($contactInfoRoot);
                        //        $mainUserInfo.animate({ right: '0px' }, animateTime);
                        //        $mainUserInfo.css("display", "block");
                        //    }
                        //}
                        //var contactInfoDialog = new VIS.ChildDialog();

                        //contactInfoDialog.setContent($contactInfoRoot);
                        //contactInfoDialog.setWidth(665);
                        ////contactInfoDialog.setHeight(700);
                        //contactInfoDialog.setTitle(VIS.Msg.getMsg("ContactInfo"));
                        //contactInfoDialog.setEnableResize(false);
                        //contactInfoDialog.setModal(true);
                        //contactInfoDialog.show();
                        //contactInfoDialog.onClose = function () {
                        //    dispose();
                        //}

                        var $editOption = $("<ul id='ulEditOption' class='vis-apanel-rb-ul'>");
                        var $menu = $("<ul id='ulCreateOrder' class='vis-apanel-rb-ul'>");
                        $editContact.on("click", function (e) {

                            debugger;
                            if (category == "" || category == null) {
                                e.preventDefault();
                                return false;
                            }
                            $editContact.find("p").css("background-color", "#e0e0e0");
                            e.stopPropagation();
                            var url = "";
                            var userID = $slideruserID.text();
                            var strCategory = category.split(',');
                            if (strCategory.length > 1) {
                                $("#w2ui-overlay").show();
                                if ($editOption != null) {
                                    $editOption.empty();
                                }

                                var $createorderroot = $('<div/>');//, {
                                $createorderroot.css('width', 'auto');

                                $createorderroot.css('height', '100%');

                                $editOption.css('width', 'auto');
                                $editOption.css('height', '100%');
                                var li = "";
                                for (var i = 0; i < strCategory.length; i++) {
                                    li += "<li style='margin-top:15px;margin-left:10px;margin-right:13px;font-size: 15px' id='" + strCategory[i].trim() + "_" + windowNo + "'>" + VIS.Msg.getMsg(strCategory[i].trim()) + "</li>";

                                }

                                $editOption.append(li);

                                $createorderroot.append($editOption);
                                $editContact.w2overlay($createorderroot.clone(true), { css: { height: '300px' } });
                                $createorderroot.css("top", "395px");
                                $editOption.load();
                               



                            }
                            else {

                                if (category == "Employee")
                                    NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Employee Master&userID=" + userID;
                                if (category == "Customer")
                                    NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Customer Master&userID=" + userID;
                                if (category == "Vendor")
                                    NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Vendor Master&userID=" + userID;
                                if (category == "Prospect")
                                    NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Prospects&userID=" + userID;

                                OpenWindow(NavigateURL, true, e);



                            }


                        });


                        $editOption.on("click", "LI", function (e) {
                            $("#w2ui-overlay").hide();
                            
                            var editOption = $(this).text();
                            if (editOption == "Employee")
                                NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Employee Master&userID=" + $slideruserID.text();
                            if (editOption == "Customer")
                                NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Customer Master&userID=" + $slideruserID.text();
                            if (editOption == "Vendor")
                                NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Vendor Master&userID=" + $slideruserID.text();
                            if (editOption == "Prospect")
                                NavigateURL = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Prospects&userID=" + $slideruserID.text();
                            // CreateOrder(editOption, e, true);
                            OpenWindow(NavigateURL, true, e);
                            e.stopPropagation();
                        });
                        $sendSms.on("click", function (e) {
                            $sendSms.find("p").css("background-color", "#e0e0e0");
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");
                            e.stopPropagation();
                            window.setTimeout(function () {
                                var sms = new VIS.Sms(null, null, 0, true);
                                var c = new VIS.CFrame();
                                c.setName(VIS.Msg.getMsg("Sms"));
                                c.setTitle(VIS.Msg.getMsg("Sms"));
                                c.hideHeader(true);
                                c.setContent(sms);
                                c.show();
                                sms.initializeComponent();
                                $(".vis-apanel-busy").css("visibility", "hidden");
                                contactInfoDialog.close();
                            }, 2);
                        });


                        $email.on("click", function (e) {
                            
                            $email.find("p").css("background-color", "#e0e0e0");
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");
                            e.stopPropagation();
                            window.setTimeout(function () {
                                var email = new VIS.Email($("#userEmail_" + windowNo).text(), null, null, $slideruserID.text(), true, true, $slideruserTableID.text());
                                var c = new VIS.CFrame();
                                c.setName(VIS.Msg.getMsg("EMail"));
                                c.setTitle(VIS.Msg.getMsg("EMail"));
                                c.hideHeader(true);
                                c.setContent(email);
                                c.show();
                                email.initializeComponent();
                                $(".vis-apanel-busy").css("visibility", "hidden");
                                contactInfoDialog.close();
                            }, 2);
                        });

                        $BPInfo.on("click", function (e) {
                            if (category == "" || category == null) {
                                e.preventDefault();
                                return false;
                            }
                            $BPInfo.find("p").css("background-color", "#e0e0e0");
                            window.setTimeout(function () {
                                var userID = $slideruserID.text();
                                var url = "";

                                url = VIS.Application.contextUrl + "WSP/Contacts/GetWindowID?windowName=Business Partner Info&userID=" + userID;
                                $.ajax({
                                    type: "GET",
                                    async: false,
                                    url: url,
                                    dataType: "json",

                                    success: function (data) {

                                        var dd = JSON.parse(data);
                                        var zoomQuery = new VIS.Query();

                                        zoomQuery.addRestriction("C_BPartner" + "_ID", VIS.Query.prototype.EQUAL, dd[1]);
                                        VIS.viewManager.startWindow(dd[0], zoomQuery);
                                        $BPInfo.find("p").removeAttr("style");
                                        contactInfoDialog.close();
                                    }
                                });
                            }, 500);
                        });

                        $createOrder.on("click", function (e) {
                            $createOrder.find("p").css("background-color", "#e0e0e0");
                            //events();
                            debugger;

                            $("#w2ui-overlay").show();
                            $("#w2ui-overlay").find("div").empty();
                            if ($menu != null) {
                                $menu.empty();
                            }

                            var $createorderroot = $('<div/>');//, {
                            $createorderroot.css('width', 'auto');

                            $createorderroot.css('height', '100%');

                            $menu.css('width', 'auto');
                            $menu.css('height', '100%');
                            var li = $("<li style='margin-top:15px;margin-left:10px;margin-right:13px;font-size: 15px' id='SaleOrder_" + windowNo + "'>" + VIS.Msg.getMsg("Sales Order") + "</li> <li  style='margin-top:10px;margin-left:10px;margin-right:13px;font-size: 15px' id='PurchaseOrder_" + windowNo + "' >" + VIS.Msg.getMsg("PurchaseOrder") + "</li>");
                            $menu.append(li);
                            $createorderroot.append($menu);
                            $createOrder.w2overlay($createorderroot.clone(true), { css: { height: '300px' } });
                            $createorderroot.css("top", "395px");
                            e.stopPropagation();
                            $menu.load();


                        }

                        );


                        $menu.on("click", "LI", function (e) {
                            $("#w2ui-overlay").hide();
                            var createOrder = $(this).text();
                            //CreateOrder(createOrder, e, false);
                            url = VIS.Application.contextUrl + "WSP/Contacts/GetWindowAndOrderID?windowName=" + createOrder + "&userID=" + $slideruserID.text();
                            OpenWindow(url, false, e);
                            e.stopPropagation();
                        });

                        $addBPartner.on("click", function (e) {
                            $addBPartner.find("p").css("background-color", "#e0e0e0");

                            e.stopPropagation();

                            $saleOrder = null;
                            $purchaseOrder = null;
                            VIS.AddUpdateBPartner(windowNo, 0, category, null, $sliderBpID.text(), $slideruBPGroupID.text());
                            $addBPartner.find("p").removeAttr("style");


                        });

                        $history.on("click", function (e) {
                            // UserTableID, Record_ID, BP_ID, 0, "keyColumnName"
                            var atHistory = new VIS.AttachmentHistory($slideruserTableID.val(), $slideruserID.text(), $sliderBpID.text(), 0, "AD_User_ID");
                            atHistory.show();
                        });

                        $appointment.on("click", function (e) {
                            
                            e.stopPropagation();
                            $appointment.find("p").css("background-color", "#e0e0e0");

                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");
                            window.setTimeout(function () {

                                VIS.AppointmentsForm.init($slideruserTableID.text(), $("#userID_" + windowNo).text(), VIS.context.getAD_User_ID(), $sliderhduserName.text(), false);

                                $appointment.find("p").removeAttr("style");
                                $(".vis-apanel-busy").css("visibility", "hidden");
                                contactInfoDialog.close();
                            }, 500);
                        });

                        $importContacts.on("click", function (e) {
                            e.stopPropagation();
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");
                            window.setTimeout(function () {
                                WSP.importcontacts(self);
                                $(".vis-apanel-busy").css("visibility", "hidden");
                                //$importContacts.find("p").removeAttr("style");
                            }, 50);
                        });
                        $activity.on("click", function (e) {
                            e.stopPropagation();
                            $activity.find("p").css("background-color", "#e0e0e0");
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");

                            window.setTimeout(function () {
                                //   VIS.AppointmentsForm.init($slideruserTableID.text(), $("#userID_" + windowNo).text(), $("#userID_" + windowNo).text(), $sliderhduserName.text(), true);

                                VIS.AppointmentsForm.init(0, $("#userID_" + windowNo).text(), VIS.context.getAD_User_ID(), $sliderhduserName.text(), true);
                                // $isBusy.hide();
                                $activity.find("p").removeAttr("style");
                                $(".vis-apanel-busy").css("visibility", "hidden");
                            }, 500);
                        });
                        $fileUpload.on("change", function () {
                            

                            //$isBusy.show();
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");

                            var userID = $slideruserID.text();
                            var xhr = new XMLHttpRequest();
                            var fd = new FormData();
                            var fileUploadFile = $fileUpload;

                            //fd.append("file", document.getElementById('#wsp-file-input_' + self.windowNo).files[0]);
                            fd.append("file", fileUploadFile[0].files[0]);
                            fd.append("userID", userID);

                            if ((fileUploadFile[0].files[0].size / kbSize) > maxSize) {
                                alert(VIS.Msg.getMsg("UploadedFileSizeLimit"));
                                $fileUpload.replaceWith($fileUpload.val('').clone(true));
                                return;
                            }

                            xhr.open("POST", VIS.Application.contextUrl + "WSP/Contacts/SaveImageAsByte", true);
                            xhr.setRequestHeader("Cache-Control", "no-cache");
                            xhr.setRequestHeader("Pragma", "no-cache");
                            xhr.send(fd);
                            xhr.addEventListener("load", function (event) {
                                
                                var dd = event.target.response;
                                var res = JSON.parse(dd);
                                image = JSON.parse(res);
                                //  $selectedContact.find("#userImg_" + self.windowNo).removeAttr("src").attr("src", "data:image/jpg;base64," + image[0]);
                                $sliderbigUserImage.removeAttr("src").attr("src", "data:image/jpg;base64," + image[1]);
                                //$mobileImg.find("#mobileImg_" + windowNo).removeAttr("src").attr("src", "data:image/jpg;base64," + image[1]);
                                //  $sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                                //    var img = new Image();
                                //    img.onload = function () {


                                //        if (ismobile) {
                                //            $sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                                //            if (img.width >= 320) {

                                //                $sliderbigUserImage.css("width", "100%");
                                //                //$sliderbigUserImage.css("height", img.height - (img.width - $sliderdivbigUserImage.width()));
                                //                //alert(img.height);
                                //                //$sliderbigUserImage.css("height", (img.height - (img.width- $sliderbigUserImage.width()))+10);
                                //                var val = (img.width * 100) / img.height;
                                //                var finalHeight = ($sliderbigUserImage.width() * 100) / val;
                                //                $sliderbigUserImage.css("height", finalHeight);
                                //            }
                                //            else {
                                //                //alert("userName" + result[0].FirstName + "imgwidth:" + $sliderbigUserImage.width() + "  imfheight:" + $sliderbigUserImage.height());

                                //                $sliderbigUserImage.css("width", img.width);
                                //                $sliderbigUserImage.css("height", img.height);

                                //            }
                                //        }

                                //    };
                                //    img.src = $bigUserImage.find("#bigUserImage_" + self.windowNo).attr("src");
                                $(".vis-apanel-busy").css("visibility", "hidden");


                                //    //  $bigUserImage.find("img:eq(1)").attr("src", imageUrl.replace("\Thumb32x32", ""));
                                //    // $("#imgUsrImage").attr('src', "data:image/jpg;base64," + a);
                            }, false);
                            $fileUpload.val("");
                        });


                        $mobilefileUpload.on("change", function () {
                            $(".vis-apanel-busy").css("visibility", "visible");
                            $(".vis-apanel-busy").css("z-index", "999999999999");

                            var userID = $slideruserID.text();
                            var xhr = new XMLHttpRequest();
                            var fd = new FormData();
                            var fileUploadFile = $fileUpload;

                            //fd.append("file", document.getElementById('#wsp-file-input_' + self.windowNo).files[0]);
                            fd.append("file", fileUploadFile[0].files[0]);
                            fd.append("userID", userID);
                            if ((fileUploadFile[0].files[0].size / kbSize) > maxSize) {
                                alert(VIS.Msg.getMsg("UploadedFileSizeLimit"));
                                return;
                            }
                            xhr.open("POST", VIS.Application.contextUrl + "WSP/Contacts/SaveImageAsByte", true);
                            xhr.setRequestHeader("Cache-Control", "no-cache");
                            xhr.setRequestHeader("Pragma", "no-cache");
                            xhr.send(fd);
                            xhr.addEventListener("load", function (event) {

                                var dd = event.target.response;
                                var res = JSON.parse(dd);
                                image = JSON.parse(res);
                                //$selectedContact.find("#userImg_" + self.windowNo).removeAttr("src").attr("src", "data:image/jpg;base64," + image[0]);
                                $sliderbigUserImage.removeAttr("src").attr("src", "data:image/jpg;base64," + image[1]);
                                $mobileImg.removeAttr("src").attr("src", "data:image/jpg;base64," + image[1]);
                                //  $sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                                //var img = new Image();
                                //img.onload = function () {


                                //    if (ismobile) {
                                //        $sliderdivbigUserImage.css("width", $userInfoRightPanelDiv.width());
                                //        if (img.width >= 320) {

                                //            $sliderbigUserImage.css("width", "100%");
                                //            // $sliderbigUserImage.css("height", img.height - (img.width - $sliderdivbigUserImage.width()));
                                //            //$sliderbigUserImage.css("height", (img.height - (img.width - $sliderbigUserImage.width())) + 10);
                                //            var val = (img.width * 100) / img.height;
                                //            var finalHeight = ($sliderbigUserImage.width() * 100) / val;
                                //            $sliderbigUserImage.css("height", finalHeight);
                                //        }
                                //        else {
                                //            //alert("userName" + result[0].FirstName + "imgwidth:" + $sliderbigUserImage.width() + "  imfheight:" + $sliderbigUserImage.height());

                                //            $sliderbigUserImage.css("width", img.width);
                                //            $sliderbigUserImage.css("height", img.height);

                                //        }
                                //    }

                                //};
                                //img.src = $mobileImg.find("#mobileImg_" + self.windowNo).attr("src");
                                $(".vis-apanel-busy").css("visibility", "hidden");



                                //  $bigUserImage.find("img:eq(1)").attr("src", imageUrl.replace("\Thumb32x32", ""));
                                // $("#imgUsrImage").attr('src', "data:image/jpg;base64," + a);
                            }, false);

                        });
                        $showAddressOnGoogleMap = $divContactDetails.find("#showAddressOnGoogleMap_" + windowNo);
                        $showAddressOnGoogleMap.on("click",
                          function () {
                              
                              var urlAddress = $divContactDetails.find("#address_" + windowNo).text() + $divContactDetails.find("#country_" + windowNo).text();
                              var url = "http://local.google.com/maps?q=" + urlAddress;
                              window.open(url);
                          });

                        function OpenWindow(url, isEdit, e) {
                            e.stopPropagation();
                            $.ajax({
                                type: "GET",
                                async: false,
                                url: url,
                                dataType: "json",
                                success: function (data) {
                                    
                                    var dd = JSON.parse(data);
                                    var zoomQuery = new VIS.Query();
                                    if (isEdit) {
                                        zoomQuery.addRestriction("C_BPartner" + "_ID", VIS.Query.prototype.EQUAL, dd[1]);
                                        VIS.viewManager.startWindow(dd[0], zoomQuery);
                                        $editContact.find("p").removeAttr("style");
                                        contactInfoDialog.close();
                                    }
                                    else {
                                        //zoomQuery.addRestriction("C_Order" + "_ID", VIS.Query.prototype.EQUAL, dd[1]);
                                        zoomQuery.addRestriction("C_Order" + "_ID", VIS.Query.prototype.EQUAL, dd[1]);
                                        win = VIS.viewManager.startWindow(dd[0], zoomQuery);
                                        win.onLoad = function () {
                                            var gc = win.cPanel.curGC;
                                            gc.onRowInserting = function () {
                                                win.cPanel.cmd_new(false);
                                            };

                                            gc.onRowInserted = function () {
                                                tab = win.cPanel.curTab;
                                                tab.setValue("C_BPartner_ID", $sliderBpID.text());

                                            };
                                        };
                                        $createOrder.find("p").removeAttr("style");
                                        contactInfoDialog.close();
                                    }

                                    // VIS.viewManager.startWindow(dd[0], null);


                                }
                            });
                        };
                        function dispose() {
                            
                            $addBPartner.off("click");
                            $activity.off("click");
                            $appointment.off("click");
                            $BPInfo.off("click");
                            $createOrder.off("click");
                            $editContact.off("click");
                            $sendSms.off("click");
                            $email.off("click");
                            $mobilefileUpload.off("change");
                            $fileUpload.off("change");
                            $menu.off("click");
                            $menu.off("click");

                            contactInfoDialog.dispose();
                            contactInfoDialog = null;
                            $contactInfoRoot.remove();
                            $contactInfoRoot.empty();
                        };
                        $(".vis-apanel-busy").css("visibility", "hidden");
                    }
                });

            }, 2);


        };


    };
    function LoadHtmlDesign(ad_userID, url) {
        $contactInfoRoot = null;
        NavigateURL = null;
        $divDots = null;
        $mainUserInfo = null;
        animateTime = 800;
        kbSize = 1048576;
        maxSize = 10;
        //$contactInfoRoot = $("<td id='contactInfo_" + windowNo + "' style='position:absolute;z-index:9999999'> <div id='mainUserInfoDiv_" + windowNo + "' class='wsp-view_profile_container sliderdiv' style='margin-top: -2px; width: 601.6px; display: block; right: 0px;'>" +
        $contactInfoRoot = $(" <div id='mainUserInfoDiv_" + windowNo + "'>" +
//"<div id='WSP_dots_" + windowNo + "' class='wsp-dots' ></div>" +
//" <div id='divUserInfo_" + windowNo + "' style='overflow: hidden;'><div class='wsp-view_left_part'>" +
"<div class='wsp-view_left_part' style='float:left;width: 45%;padding-left: 10px;padding-right: 10px'>" +
"<input type='hidden' value='0' id='userID_" + windowNo + "'></input>" +
" <p id='hduserName_" + windowNo + "' style='display: none;'></p>" +
"  <p id='userName_" + windowNo + "' class='wsp-view_profile_name'></p> <a id='dotImg_" + windowNo + "' href='javascript:void(0)' style='display: none;'></a>" +
"<div id='popUpDiv_" + windowNo + "' style='display: none;'></div>" +



"<input type='hidden' value='0' id='BPID_" + windowNo + "'></input>" +
"<input type='hidden' value='0' id='BPGroupID_" + windowNo + "'></input>" +
"<input type='hidden' value='' id='userTableID_" + windowNo + "'>" +
"<p id='companyName_" + windowNo + "' class='wsp-view_profile_possession border'></p>" +
"<p class='wsp-view_profile_possession border' style='border-bottom: 1px solid #DFDFDF; padding-bottom: 20px;' id='pContactTypeCategory_" + windowNo + "'></p>" +

"<div class='wsp-profile_big_img_mobile'>" +

"<span id='editMobileImage_" + windowNo + "' class='mobilespan' style='width:24px;float:right;'>" +
"<input type='file' id='wsp-fileinput_" + windowNo + "' class='wsp-file-input' value='Add Pic' style='opacity: 0; width: 7%; margin-top: -6px; float: left; z-index: 999; position: absolute; cursor: pointer;'>" +
"<img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/Edit-icon.png' style='z-index: 100; position: absolute;'>" +
"</span>" +
"<img id='mobileImg_" + windowNo + "' width='100%' alt='Image is not available' src='Areas/VIS/Images/home/User.png'>" +


"</div>" +

"<div id='divTeam_" + windowNo + "' class='wsp-view_team'></div>" +

"<div id='divContactDetails_" + windowNo + "' class='wsp-view_team'></div>" +

"<div id='divcompanyDetails_" + windowNo + "' class='wsp-view_team'></div>" +

"<div id='divRole_" + windowNo + "' class='wsp-view_team'></div>" +


"</div>" +

"<div class='wsp-view_right_part'  style='float:left;width: 55%;' id='WSP_view_right_part_" + windowNo + "'>" +

"<div id='userBigImg_" + windowNo + "' class='wsp-profile_big_img' style='width: 322px;'>" +

"<div class='image-upload' style='float: right; position: absolute; z-index: 999; width: 100%'>" +
" <label for='file-input' class='wsp-file-label'>" +
"<span class='wsp-change-picture-ico'></span>" +
"</label>" +

"<input type='file' id='wsp-file-input_" + windowNo + "' class='wsp-file-input' value='Add Pic' style='opacity: 0; width: 30%; margin-top: 0px; float: right; z-index: 999; position: absolute; right: 0px; cursor: pointer;'>" +

"</div>" +

"<div class='wsp-bigimg'>" +

" <img id='bigUserImage_" + windowNo + "' alt='Image is not available' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/User.png'>" +
"</div>" +
"</div>" +

"<div class='wsp-action'>" +
"<table id='actionTable_" + windowNo + "' class='wsp-tableRightContent'>" +
"<tbody><tr style='height:30px'>" +
" <td class='tdactionlist'><a id='edit_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
" <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/edit_2.png'></span>" +
" <p>Edit </p>" +
" </a>" +
" </td>" +

" <td class='tdactionlist'>" +

" <a id='sendSMS_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
" <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/send_sms.png'></span>" +
" <p>" +
" Send Sms" +
" </p>" +
" </a>" +
" </td>" +
" </tr>" +

"  <tr style='height:30px'>" +
"       <td class='tdactionlist'><a id='email_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon email_icon'>" +
"           <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/message.png'></span>" +
"           <p>Email </p>" +
"       </a>" +


"       </td>" +

"       <td class='tdactionlist'><a id='appointment_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
"           <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/appointment.png'></span>" +

"           <p>Appointment </p>" +
"       </a>" +

"       </td>" +

"   </tr>" +

"   <tr style='height:30px'>" +

"       <td class='tdactionlist'><a id='activity_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
"            <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/activity.png'></span>" +
"           <p>Activity</p>" +
"       </a>" +
"       </td>" +

"        <td class='tdactionlist'><a id='createOrder_" + windowNo + "' href='javascript:void(0)' class='wsp-tableaction'><span class='wsp-left_icon'>" +
"           <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/create_order.png'></span>" +
"           <p>Create Order</p>" +
"       </a>" +

"        </td>" +

"    </tr>" +

"   <tr style='height:30px'>" +
"       <td class='tdactionlist'><a id='BPInfo_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
"           <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/bp_info.png'></span>" +
"           <p>BP Info</p>" +
"       </a>" +

"        </td>" +

"        <td class='tdactionlist'><a id='addBPartner_" + windowNo + "' href='javascript:void(0)' class='wsp-tableaction'><span class='wsp-left_icon'>" +
"           <img src='" + VIS.Application.contextUrl + "Areas/WSP/Images/addBPartner.png'></span>" +
"           <p style=''>Add BPartner </p>" +
"       </a>" +

"       </td>" +
"   </tr>" +

" <tr style='height:30px'>" +
                "<td class='tdactionlist'><a id='History_" + windowNo + "' class='wsp-tableaction' href='javascript:void(0)'><span class='wsp-left_icon'>" +
                    "<img src='/Areas/WSP/Images/History24.png'></span>" +
                    "<p>"+VIS.Msg.getMsg('History')+"</p>" +
                "</a>" +

                "</td>" +

            "</tr>" +


"   <tr>" +

"   </tr>" +
"</tbody></table>" +
"</div>" +


"<div id='divBirthday_" + windowNo + "' class='wsp-birthday_container' style='display: none;'></div></div></div>");



    };






})(VIS, jQuery);