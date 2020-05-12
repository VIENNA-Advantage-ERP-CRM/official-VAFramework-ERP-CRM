
; $(function () {
    var user = 11;

    //var contextUrl = '/';

    var getValidationSummaryErrors = function ($form) {
        var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
        return errorSummary;
    };

    var displayErrors = function (form, errors) {
        var errorSummary = getValidationSummaryErrors(form)
            .removeClass('validation-summary-valid')
            .addClass('validation-summary-errors');

        var items = $.map(errors, function (error) {

            var error1 = Globalize.localize(error);
            var ret = "";
            if (error1) {
                ret = '<li>' + error1 + '</li>';
            }
            else {
                ret = '<li>' + error + '</li>';
            }
            return ret;
        }).join('');

        errorSummary.find('ul').empty().append(items);
    };

    var formSubmitHandler = function (e) {
        var $form = $(this);
        if ($('#login-form-2').is(':visible')) {
            var newPwd = $newPwd.val();
            var newCPwd = $newCPwd.val();

            if (newPwd != newCPwd) {
                e.preventDefault();
                displayErrors($form, ["BothPwdNotMatch"]);
                return false;
            }

            //strong password regular expression
            if (!validatePassword(newPwd)) {
                e.preventDefault();
                displayErrors($form, ["mustMatchCriteria"]);
                return false;
            }


        }
        displayErrors($form, "");
        $btnLogin1.prop('disabled', true);
        $btnLogin2.prop('disabled', true);
        $backButton.prop('disabled', true);



        // $imgbusy1.show();
        $imgbusy1.css('display', 'block');
        // We check if jQuery.validator exists on the form
        if (!$form.valid || $form.valid()) {
            $.post($form.attr('action'), $form.serializeArray())
                .done(function (json) {
                    json = json || {};

                    // In case of success, we redirect to the provided URL or the same page.

                    if (json.step2) {
                        //TODO init 
                        showLogin2();
                        user = json.ctx.AD_User_ID;

                        /*Set context form local Ini */

                        $('#login1Data').val(JSON.stringify(json.ctx));
                        localStorage.setItem("vis_login_langCode", $cmbLang.val());
                        fillCombo($cmbRole, json.role);
                        $btnLogin1.prop('disabled', false);
                        $btnLogin2.prop('disabled', false);
                        $backButton.prop('disabled', false);
                        $imgbusy1.css('display', 'none');
                    }
                    else if (json.ctx && json.ctx.ResetPwd) {
                        $imgbusy1.css('display', 'none');
                        showLoginResetPwd();
                        $('#ResetPwd').val(json.ctx.ResetPwd);
                    }
                    else if (json.ctx && json.ctx.Is2FAEnabled) {
                        showLogin2FA();
                        $("#QRCdeimg").attr('src', json.ctx.QRCodeURL);
                        if (json.ctx.QRFirstTime)
                            $(".vis-loginQRSec").css("display", "block");
                        else {
                            var loginQRLbl = $(".vis-loginQRLabel");
                            loginQRLbl.css("margin-top", "15px");
                            loginQRLbl.css("margin-bottom", "40px");
                        }
                        $otpTwoFA.val("");
                        $('#login3Data').val(JSON.stringify(json.ctx));
                        $otpTwoFA.focus();
                        $imgbusy1.css('display', 'none');
                    }
                    else if (json.success) {
                        window.location = json.redirect || location.href;
                        localStorage.setItem("vis_login_langCode", $cmbLang.val());
                        if (window.location.href.indexOf('#') != -1) {
                            location.reload();
                        }
                    } else if (json.errors) {
                        displayErrors($form, json.errors);
                        $btnLogin1.prop('disabled', false);
                        $btnLogin2.prop('disabled', false);
                        $backButton.prop('disabled', false);
                        // $imgbusy2.hide();
                        //$imgbusy1.hide();
                        $imgbusy1.css('display', 'none');
                    }

                })
                .error(function () {
                    displayErrors($form, ['An unknown error happened.']);
                    $btnLogin1.prop('disabled', false);
                    $btnLogin2.prop('disabled', false);
                    //$imgbusy2.hide();
                    //$imgbusy1.hide();
                    $imgbusy1.css('display', 'none');
                });
        }

        // Prevent the normal behavior since we opened the dialog
        e.preventDefault();
    };

    var validatePassword = function (password) {
        var regex = /^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*[@$!%*?&])[a-zA-Z][A-Za-z\d@$!%*?& ]{4,}$/;// Start with Alphabet, minimum 4 length
       //@$!%*#?& allowed only

        var passed = 0;

       if(!regex.test(password)) {

           return false;
        }
        return true;
    };

    function showLogin2() {
        $("#loginPanel").hide();//"slide", function () {
        $("#login2Panel").show();//"slide", function () {
        $("#roleName").focus();
        $cmbClient.empty();
        $cmbOrg.empty();
        $cmbWarehouse.empty();
        $("#login2Panel").find("ul").empty();
    };

    var showLoginResetPwd = function () {
        $('#login-form-1').hide();
        $('#login-form-3').hide();
        $('#login-form-2').show(); 
        $btnLogin1.prop('disabled', false);
        $newPwd.focus();
    };

    var showLogin2FA = function () {
        $('#login-form-1').hide();
        $('#login-form-2').hide();
        $('#login-form-3').show();
        $btnLogin1.prop('disabled', false);
    };

    var showLogin = function (e) {
        $("#login2Panel").hide();//  "slide", function () {
        $("#loginPanel").show();//"slide", function () {
        $("#loginName").focus();
        e.preventDefault();
    };

    var getdata = function (combo, url, data) {
        //$imgbusy1.show();
        $imgbusy1.css('display', 'block');
        $.ajax(url, {
            data: data
        }).success(function (result) {
            fillCombo(combo, result.data);
            //$imgbusy1.hide();
            $imgbusy1.css('display', 'none');
        })
            .fail(function (result) {
                alert(result);
                //  $imgbusy1.show();
                $imgbusy1.css('display', 'block');
            });
    }

    var comboChange = function () {
        if (setting)
            return;

        $combo = this;
        $cmbRole.attr('id');
        if ($combo.id === $cmbRole.attr('id')) {
            //Clear other combo
            $cmbClient.empty();
            $cmbOrg.empty();
            $cmbWarehouse.empty();

            getdata($cmbClient, contextUrl + "Account/GetClients", { 'Id': $combo.value });

        }
        else if ($combo.id === $cmbClient.attr('id')) {
            $cmbOrg.empty();
            $cmbWarehouse.empty();

            getdata($cmbOrg, contextUrl + "Account/GetOrgs", { 'role': $cmbRole.val(), 'user': user, 'client': $combo.value });
        }
        else if ($combo.id === $cmbOrg.attr('id')) {
            $cmbWarehouse.empty();
            getdata($cmbWarehouse, contextUrl + "Account/GetWarehouse", { 'id': $cmbOrg.val() });
        }
        else if ($combo.id === $cmbWarehouse.attr('id')) {

        }

        $hidden = $('#' + $combo.id + 'Name');
        var text = this.options[this.selectedIndex].innerHTML;

        $hidden.val(text);

        //console.log($hidden.val());
    }

    var fillCombo = function (combo, data) {
        setting = true;
        combo.empty();

        var text = Globalize.localize('SelectWarehouse');
        if (combo === $cmbRole) {
            text = Globalize.localize('SelectRole');
        }
        else if (combo === $cmbClient) {
            text = Globalize.localize('SelectClient');
        }
        else if (combo === $cmbOrg) {
            text = Globalize.localize('SelectOrg');
        }


        $("<option />", {
            val: "-1",
            text: text
        }).appendTo(combo);

        $(data).each(function () {
            $("<option />", {
                val: this.Key,
                text: this.Name
            }).appendTo(combo);
        });
        setting = false;
    }
    var setFoucs = false;

    var langchange = function () {

        //$imgbusy1.show();
        $imgbusy1.css('display', 'block');
        var code = this.options[this.selectedIndex].value.replace("_", "-");
        var referencepath = contextUrl + "Areas/ViennaBase/Scripts/globalize/cultures/globalize.culture." + code + ".js";
        $cmbLang.hide();
        $.getScript(referencepath, function () {
            Globalize.culture(code);
            setText();
            if (code.substring(0, 2) == "ar") { //right to left
                $('html').attr("dir", "rtl");
            }
            else {
                $('html').attr("dir", "ltr");
            }
            //$('html').css("background-color", "lightblue");
            $cmbLang.show();
            if (setFoucs)
                $cmbLang.focus();
            setFoucs = true;
            //$imgbusy1.hide();
            $imgbusy1.css('display', 'none');
        });
    }

    var setText = function () {

        // $lblUser.text(Globalize.localize("User"));
        // $lblPwd.text(Globalize.localize("Password"));
        // $lblLang.text(Globalize.localize("Language"));
        $txtUser.attr("placeholder", Globalize.localize("User"));
        $txtPwd.attr("placeholder", Globalize.localize("Password"));
        $newPwd.attr("placeholder", Globalize.localize("NewPassword"));
        $newCPwd.attr("placeholder", Globalize.localize("NewCPassword"));


        $btnLogin1.val(Globalize.localize("Login"));
        $lblRemember.text(Globalize.localize("RememberMe"));

        $otpTwoFA.attr("placeholder", Globalize.localize("EnterOTP"));
        $lblScanQRCode.text(Globalize.localize("ScanQRCode"));
        $lblEnterVerCode.text(Globalize.localize("EnterVerCode"));

        $lblRole.text(Globalize.localize("Role"));
        $lblClient.text(Globalize.localize("Client"));
        $lblOrg.text(Globalize.localize("Organization"));
        $lblWare.text(Globalize.localize("Warehouse"));
        $lblDate.text(Globalize.localize("Date"));
        $btnLogin2.val(Globalize.localize("Login"));
        $backButton.val(Globalize.localize("Back"));
    }

    $("#loginForm").submit(formSubmitHandler);
    $("#login2Form").submit(formSubmitHandler);
    $("#showLogin").click(showLogin);
    $("#login2Form select").change(comboChange);

    var $cmbRole = $("#role");
    var $cmbClient = $("#client");
    var $cmbOrg = $("#org");
    var $cmbWarehouse = $("#warehouse");
    var $cmbLang = $("#cmbLang");

    var $txtUser = $("#loginName");
    var $txtPwd = $("#txtPwd");
    var $newPwd = $('#txtNewPwd');
    var $newCPwd = $('#txtCNewPwd');
    var $otpTwoFA = $('#txt2FAOTP');
    var $lblScanQRCode = $('#lblScanQRCode');
    var $lblEnterVerCode = $('#lblEnterVerCode');
    //var $lblUser = $('label[for="Login1Model_UserName"]');
    //var $lblPwd = $('label[for="Login1Model_Password"]');
    //var $lblLang = $('label[for="Login1Model_LoginLanguage"]');
    var $lblRemember = $('#lblRem');


    var $lblRole = $('label[for="Login2Model_Role"]');
    var $lblClient = $('label[for="Login2Model_Client"]');
    var $lblOrg = $('label[for="Login2Model_Org"]');
    var $lblWare = $('label[for="Login2Model_Warehouse"]');
    var $lblDate = $('label[for="Login2Model_Date"]');
    var $imgbusy1 = $(".img-login");

    var $btnLogin1 = $("#btnLogin1");
    var $btnLogin2 = $("#btnLogin2");
    var $backButton = $("#showLogin");

    $cmbLang.change(langchange);

    var setting = false;

    function setLanguage() {
        var langCode = localStorage.getItem("vis_login_langCode");
        if (langCode == null || langCode == '') {
            var lang = navigator.language || navigator.userLanguage;
            if (lang) {
                if (lang.indexOf("-") > -1)
                    langCode = lang.replace("-", "_");
                else {
                    langCode = lang;
                    var langs = navigator.languages;
                    if (langs && langs.length > 0) {
                        for (var l = 0; l < langs.length; l++) {
                            if (langs[l].length > 2 && lang == langs[l].substring(0, 2)) {
                                langCode = langs[l].replace("-", "_");
                                break;
                            }
                        }
                        if (langCode.length < 3)
                            langCode = langCode + "_" + langCode.toUpperCase();
                    }
                }
            }
            else {
                langCode = 'en_US';
            }
        }
        // if (langCode != null && langCode != '') {
        $cmbLang.val(langCode);
        if ($cmbLang[0].selectedIndex < 0) {
            $cmbLang.val('en_US');
        }
        $cmbLang.trigger("change");
        // }
    };

    setLanguage();
    setText();

});

(function () {
    $("#login2Panel").hide();
    $('#login-form-2').hide();
    $('#login-form-3').hide();
})();