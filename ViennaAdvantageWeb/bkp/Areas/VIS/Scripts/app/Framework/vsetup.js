/********************************************************
 * Module Name    :     Application
 * Purpose        :     Create New Tenant
 * Author         :     Lakhwinder
 * Date           :     10-Oct-2014
  ******************************************************/
; (function (VIS, $) {
    VIS.Apps = VIS.Apps || {};
    VIS.Apps.AForms = VIS.Apps.AForms || {}



    function VSetup() {


        var url = VIS.Application.contextFullUrl;
        if (!url)
            url = VIS.Application.contextUrl;
        if (url) {
            if (url.toString().toLowerCase().indexOf("softwareonthecloud.com") > -1) {
                VIS.ADialog.info("VIS_GotoControlPanel", true, "", "");
                return;
            }
        }



        var $root = $("<div class='vis-forms-container' style='height:100%'>");
        // $root.height($(window).height() - 70);
        var container = $("<div class='vis-its-main-wrap vis-pull-left'>");

        var subRoot = $("<div style='height:100%'>");
        $root.append(subRoot);
        subRoot.append(container);
        var divRight = $("<div class='vis-its-ryt-panel vis-pull-right' >");
        var divRightInner = $("<div class='vis-its-ryt-panel-inner vis-pull-left' >");

        subRoot.append(divRight);
        divRight.append(divRightInner);
        divRightInner.append($('<h3>').append(VIS.Msg.getMsg('Result')));
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var windowNo = VIS.Env.getWindowNo();

        var txtTenant = null;
        var txtOrg = null;
        var txtUTenant = null;
        var txtUOrg = null;
        var cmbCurr = null;
        var cmbCou = null;
        var txtCity = null;
        var cmbReg = null;
        var chkOpprotunity = null;
        var chkSR = null;
        var chkProduct = null;
        var chkCam = null;
        var chkBP = null;
        var btnLoadFile = null;
        var btnCancel = null;
        var btnOK = null;
        var fileBrowser = null;
        var folderKey = null;
        var fileName = null
        var chkDA = null;
        var lblDA = null;
        this.load = function () {

            

            var dContent = $("<div class='vis-graywrap'>");
            container.append(dContent);
            var dTForm = $("<div class='initial-client-form'>");
            dContent.append(dTForm);

            //dTForm.append($("<div Style='margin-left:15px;margin-top:15px'>").append($("<label>").append(VIS.Msg.getMsg('TenantHeaderComment'))));
            dTForm.append($("<h3 class='VIS_Pref_change-pass'>").append($("<label class='VIS_Pref_Label_Font'>").append(VIS.Msg.getMsg("TenantHeaderComment"))));

            var dTenant = $("<div class='input-group vis-input-wrap vis-intial-form-data'>");
            var dTenantInner = $("<div class='vis-control-wrap'></div>");
            txtTenant = $('<input type="text" name="tenant" placeholder="' + VIS.Msg.getMsg('Tenant') + '">');
            dTenant.append(dTenantInner);
            dTenantInner.append(txtTenant);
            dTenantInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "AD_Client_ID")));         
            dTForm.append(dTenant);

            var dOrg = $("<div class='input-group vis-input-wrap vis-intial-form-data'>");
            var dOrgInner = $("<div class='vis-control-wrap'></div>");
            txtOrg = $('<input type="text" name="tenant" placeholder="' + VIS.Msg.getMsg('Org') + '">');
            dOrg.append(dOrgInner);
            dOrgInner.append(txtOrg);
            dOrgInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "AD_Org_ID")));          
            dTForm.append(dOrg);

            var dUTenant = $("<div class='input-group vis-input-wrap vis-intial-form-data'>");
            var dUTenantInner = $("<div class='vis-control-wrap'></div>");
            txtUTenant = $('<input type="text" name="tenant" placeholder="' + VIS.Msg.getMsg('TenantAdmin') + '">');
            dUTenant.append(dUTenantInner);
            dUTenantInner.append(txtUTenant);
            dUTenantInner.append($('<label>').append(VIS.Msg.parseTranslation(VIS.context, "@AD_User_ID@ @AD_Client_ID@")));           
            dTForm.append(dUTenant);

            var dUOrg = $("<div class='input-group vis-input-wrap vis-intial-form-data'>");
            var dUOrgInner = $("<div class='vis-control-wrap'></div>");
            txtUOrg = $('<input type="text" name="tenant" placeholder="' + VIS.Msg.getMsg('TenantUser') + '">');
            dUOrg.append(dUOrgInner);
            dUOrgInner.append(txtUOrg);
            dUOrgInner.append($('<label>').append(VIS.Msg.parseTranslation(VIS.context, "@AD_User_ID@ @AD_Org_ID@")));          
            dTForm.append(dUOrg);


            var dCurr = $("<div class='input-group vis-input-wrap vis-intial-form-dataCombo'>");
            var dCurrInner = $("<div class='vis-control-wrap'></div>");            
            cmbCurr = $('<select placeholder="Currency">');
            dCurr.append(dCurrInner);
            dCurrInner.append(cmbCurr);
            dCurrInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Currency_ID")));
            dTForm.append(dCurr);

            var dCou = $("<div class='input-group vis-input-wrap vis-intial-form-dataCombo'>");
            var dCouInner = $("<div class='vis-control-wrap'></div>"); 
            cmbCou = $('<select placeholder="Country">');
            dCou.append(dCouInner);
            dCouInner.append(cmbCou);
            dCouInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Country_ID")));
            dTForm.append(dCou);

            var dCity = $("<div class='input-group vis-input-wrap vis-intial-form-dataCombo'>");
            var dCityInner = $("<div class='vis-control-wrap'></div>"); 
            txtCity = $('<input type="text" placeholder="' + VIS.Msg.getMsg('City') + '" style="width:100%;">');
            dCity.append(dCityInner);
            dCityInner.append(txtCity);
            dCityInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "City")));
            dTForm.append(dCity);

            var dReg = $("<div class='input-group vis-input-wrap vis-intial-form-dataCombo' >");
            var dRegInner = $("<div class='vis-control-wrap'></div>"); 
            cmbReg = $('<select placeholder="Region">');
            dRegInner.append(cmbReg);
            dRegInner.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Region_ID")));
            dReg.append(dRegInner);
            dTForm.append(dReg);

            //var ulCase = $('<ul  class="initial-client-form-list">');
            //dTForm.append(ulCase);

            //var liCurr = $('<li>');
            //ulCase.append(liCurr);
            //var dCurr = $('<div class="intial-form-dataCombo">');
            //liCurr.append(dCurr);
            //dCurr.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Currency_ID")));
            //cmbCurr = $('<select placeholder="Currency">');
            //dCurr.append(cmbCurr);

            //var liCou = $('<li>');
            //ulCase.append(liCou);
            //var dCou = $('<div class="intial-form-dataCombo">');
            //liCou.append(dCou);
            //dCou.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Country_ID")));
            //cmbCou = $('<select placeholder="Country">');
            //dCou.append(cmbCou);

            //var liCity = $('<li>');
            //ulCase.append(liCity);
            //var dCity = $('<div class="intial-form-dataCombo">');
            //liCity.append(dCity);
            //dCity.append($('<label>').append(VIS.Msg.translate(VIS.context, "City")));
            //txtCity = $('<input type="text" placeholder="City" style="width:100%;">');
            //dCity.append(txtCity);

            //var liReg = $('<li>');
            //ulCase.append(liReg);
            //var dReg = $('<div class="intial-form-dataCombo">');
            //liReg.append(dReg);
            //dReg.append($('<label>').append(VIS.Msg.translate(VIS.context, "C_Region_ID")));
            //cmbReg = $('<select placeholder="Region">');
            //dReg.append(cmbReg);

            //var liChks = $("<li>");
            //ulCase.append(liChks);

            dTForm.append($('<div style="width:100%;float:left;">').append($("<label  class='vis-cs-op-lbl' class='vis-pull-left'>").append(VIS.Msg.getMsg("Optional"))));
            var dChkContainer = $('<div class="vis-fieldset-wrap">');
            var dChkBox = $("<div class='vis-intial-form-data vis-fieldset-inn'>");
            dChkContainer.append(dChkBox);
            dTForm.append(dChkContainer);
            //liChks.append(dChkBox);
           

            var dOpprotunity = $('<div class="vis-initial-form-checkbox" >');
            chkOpprotunity = $('<input type="checkbox">');
            dOpprotunity.append(chkOpprotunity);
            dOpprotunity.append($("<label>").append(VIS.Msg.translate(VIS.context, "C_Project_ID")));
            dChkBox.append(dOpprotunity);

            var dSR = $('<div class="vis-initial-form-checkbox">');
            chkSR = $('<input type="checkbox">');
            dSR.append(chkSR);
            dSR.append($("<label>").append(VIS.Msg.translate(VIS.context, "C_SalesRegion_ID")));
            dChkBox.append(dSR);

            var dProduct = $('<div class="vis-initial-form-checkbox">');
            chkProduct = $('<input type="checkbox" checked disabled>');
            dProduct.append(chkProduct);
            dProduct.append($("<label>").append(VIS.Msg.translate(VIS.context, "M_Product_ID")));
            dChkBox.append(dProduct);

            var dCam = $('<div class="vis-initial-form-checkbox">');
            chkCam = $('<input type="checkbox">');
            dCam.append(chkCam);
            dCam.append($("<label>").append(VIS.Msg.translate(VIS.context, "C_Campaign_ID")));
            dChkBox.append(dCam);

            var dBP = $('<div class="vis-initial-form-checkbox">');
            chkBP = $('<input type="checkbox" checked disabled>');
            dBP.append(chkBP);
            dBP.append($("<label>").append(VIS.Msg.translate(VIS.context, "C_BPartner_ID")));
            dChkBox.append(dBP);

            //var liBtns= $("<li>");
            //ulCase.append(liBtns);
            //var dBtns = $("<div class='initial-form-buttons' >");
            //dTForm.append(dBtns);
            //liBtns.append(dBtns);

            // var dRbtns = $("<div class='initial-btn-right'>");
            var dLbtns = $("<div class='vis-initial-btn-left vis-pull-right'>");
            dContent.append(dLbtns);

            var dDefaulAcct = $('<div style="display: inline;">');
            dLbtns.append(dDefaulAcct);
            chkDA = $('<input type="checkbox" style="margin-right: 5px;margin-left: 10px;margin-top: 5px;display:none">');
            chkDA.prop('checked', true);
            chkDA.on('click', function () {
                if (chkDA.prop('checked')) {
                    btnLoadFile.css('display', 'none');
                }
                else {
                    btnLoadFile.css('display', 'initial');
                }
            });
            dDefaulAcct.append(chkDA);
            lblDA = $("<label style='margin-right: 10px;font-size: 14px;color: #666;font-weight: normal;display:none'>").append(VIS.Msg.getMsg("UseDefault"));
            dDefaulAcct.append(lblDA);
            btnLoadFile = $("<a class='vis-initial-btn vis-initial-btn-blue' style='margin-right:10px;display:none;'>").append(VIS.Msg.getMsg("LoadAccountingValues"));
            btnLoadFile.on('click', function () {
                fileBrowser.trigger('click');
            });
            // dRbtns.append(btnLoadFile);
            // dBtns.append(dRbtns);
            dLbtns.append(btnLoadFile);
            fileBrowser = $("<input type='file' style='display:none;' accept='.csv*'>");
            fileBrowser.change(function () {

                uploadFile(this);
            });
            //dRbtns.append(fileBrowser);
            dLbtns.append(fileBrowser);

          
            btnOK = $("<a class='vis-initial-btn' style='margin-right: 15px;'>").append(VIS.Msg.getMsg("Done"));
            btnOK.on('click', function () {
                createTenant();
            });
            dLbtns.append(btnOK);
            //btnCancel = $("<a class='initial-btn'>").append(VIS.Msg.getMsg("Cancel"));
            //dLbtns.append(btnCancel);
            //dBtns.append(dLbtns);

            $root.append($busyDiv);

            $busyDiv[0].style.visibility = "visible";

            $.ajax({
                url: VIS.Application.contextUrl + "VSetup/GetInitialData",
                dataType: "json",
                error: function () {
                    VIS.ADialog.error(VIS.Msg.getMsg('ERRORGettingInitialData'));
                    $busyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {

                    var result = data.result;
                    if (result == null) {
                        VIS.ADialog.error(VIS.Msg.getMsg('ERRORGettingInitialData'));
                        $busyDiv[0].style.visibility = "hidden";
                        return;
                    }
                    loadCurrency(result.currency);
                    loadCountry(result.country);
                    loadRegion(result.region);
                    $busyDiv[0].style.visibility = "hidden";
                }
            });




        };

        var loadCurrency = function (currency) {
           
            if (currency == null) {
                return;
            }
            for (var itm in currency) {

                cmbCurr.append($('<option value="' + currency[itm].ID + '">').append(currency[itm].Name));
            }
        };
        var loadCountry = function (country) {
            
            if (country == null) {
                return;
            }
            for (var itm in country) {

                cmbCou.append($('<option value="' + country[itm].ID + '">').append(country[itm].Name));
            }
        };
        var loadRegion = function (region) {
           
            if (region == null) {
                return;
            }
            for (var itm in region) {

                cmbReg.append($('<option value="' + region[itm].ID + '">').append(region[itm].Name));
            }
        };

        var uploadFile = function (file) {
            fileName = file.files[0].name;
            $busyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {
                folderKey = Date.now().toString();
                var xhr = new XMLHttpRequest();
                var fd = new FormData();
                fd.append("file", file.files[0]);
                xhr.open("POST", VIS.Application.contextUrl + "Attachment/SaveFileinTemp/?filename=" + fileName + "&folderKey=" + folderKey, false);
                xhr.send(fd);
                $busyDiv[0].style.visibility = "hidden";
            }, 2);


        };


        var createTenant = function () {
            //showLog(null);
            //return;
            divRightInner.empty();
            var clientName = txtTenant.val();
            if (clientName == null || clientName.length == 0 || clientName.trim().length == 0) {
                VIS.ADialog.error('FillTenantName');
                return;
            }
            var orgName = txtOrg.val();
            if (orgName == null || orgName.length == 0 || orgName.trim().length == 0) {
                VIS.ADialog.error('FillOrgName');
                return;
            }
            var userClient = txtUTenant.val();
            if (userClient == null || userClient.length == 0 || userClient.trim().length == 0) {
                VIS.ADialog.error('FillUserClient');
                return;
            }
            var userOrg = txtUOrg.val();
            if (userOrg == null || userOrg.length == 0 || userOrg.trim().length == 0) {
                VIS.ADialog.error('FillUserOrg');
                return;
            }
            var city = txtCity.val();
            if (city == null || city.length == 0 || city.trim().length == 0) {
                VIS.ADialog.error('FillCity');
                return;
            }
            var currencyID = cmbCurr.val();
            var currencyName = cmbCurr.find('option:selected').text();
            if (currencyName == null || currencyName.length == 0 || currencyName.trim().length == 0) {
                VIS.ADialog.error('FillCurrency');
                return;
            }
            //  var countryID = cmbCurr.val();
            var countryID = cmbCou.val();
            var countryName = cmbCou.find('option:selected').text();
            if (countryName == null || countryName.length == 0 || countryName.trim().length == 0) {
                VIS.ADialog.error('FillCountry');
                return;
            }
            var regionID = cmbReg.val();
            var regionName = cmbReg.find('option:selected').text();
            if (regionName == null || regionName.length == 0 || regionName.trim().length == 0) {
                VIS.ADialog.error('FillRegion');
                return;
            }
            if (chkDA.prop('checked')) {
                fileName = null;
            }
            else {
                if (fileName == null || fileName.length == 0 || fileName.trim().length == 0) {
                    VIS.ADialog.error('SelectFile');
                    return;
                }
            }

            if (userClient == userOrg)
            {
                VIS.ADialog.error("UsernameMustBeDifferent");
                return;
            }



            var cfProduct = chkProduct.prop('checked');
            var cfBPartner = chkBP.prop('checked');
            var cfProject = chkOpprotunity.prop('checked');
            var cfMCampaign = chkCam.prop('checked');
            var cfSRegion = chkSR.prop('checked');
            $busyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "VSetup/InitailizeClientSetup",
                type: "POST",
                dataType: "json",
                data: {
                    clientName: clientName,
                    orgName: orgName,
                    userClient: userClient,
                    userOrg: userOrg,
                    city: city,
                    currencyID: currencyID,
                    currencyName: currencyName,
                    countryID: countryID,
                    countryName: countryName,
                    regionID: regionID,
                    regionName: regionName,
                    cfProduct: cfProduct,
                    cfBPartner: cfBPartner,
                    cfProject: cfProject,
                    cfMCampaign: cfMCampaign,
                    cfSRegion: cfSRegion,
                    fileName: fileName,
                    folderKey: folderKey
                },

                error: function () {
                    VIS.ADialog.error("ErrorCreatingTenant");
                    $busyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {
                    var tInfo = data.result;
                    //if (tInfo.Log != null && tInfo.Log.trim().length > 0) {
                    //    $busyDiv[0].style.visibility = "hidden";
                    //    VIS.ADialog.error(tInfo.Log);
                    //    return;
                    //}
                    $busyDiv[0].style.visibility = "hidden";
                    showLog(tInfo);
                }
            });

        };

        var showLog = function (tInfo) {
            //var divLog=$('<div>');

            

            if(tInfo.Log!=null && tInfo.Log.trim().length > 0){
                divRightInner.append($('<div style="margin-bottom:10px">').append(" Error - " + tInfo.Log));
                divRightInner.append($('<div style="margin-bottom:10px">').append(VIS.Msg.getMsg('VIS_TenantErrorMsg')));
            }

            divRightInner.append($('<div style="margin-bottom:10px">').append(" Tenant Name - " + tInfo.TenantName));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Organization Name - " + tInfo.OrgName));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Admin Role - " + tInfo.AdminRole));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" User Role - " + tInfo.UserRole));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Admin Username - " + tInfo.AdminUser));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Admin User Password - " + tInfo.AdminUserPwd));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Org Username - " + tInfo.OrgUser));
            divRightInner.append($('<div style="margin-bottom:10px">').append(" Org User Password - " + tInfo.OrgUserPwd));


            //divRight.append($('<div style="margin-bottom:10px">').append(" Tenant Name - A"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Organization Name - A"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Admin Role - AAdmin"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" User Role - AUser"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Admin Username - AA"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Admin User Password - AA"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Org Username - BB"));
            //divRight.append($('<div style="margin-bottom:10px">').append(" Org User Password - BB"));

            var btnSave = null;
            //if (VIS.Application.isRTL) {
            //    btnSave = $('<a href="javascript:void(o)" style="float:left;" class="vis-initial-btn vis-initial-btn-blue">').append($("<i class='vis vis-save'></i>"));
            //}
            //else {
                btnSave = $('<a href="javascript:void(o)" class="vis-initial-btn vis-initial-btn-blue">').append($("<i class='vis vis-save'></i>"));
            //}
            btnSave.on('click', function () {

                var text = '';
                text+=" Tenant Name - " + tInfo.TenantName + "\t\n";
                text+=" Organization Name - " + tInfo.OrgName + "\t\n";
                text+=" Admin Role - " + tInfo.AdminRole + "\t\n";
                text+=" User Role - " + tInfo.UserRole + "\t\n";
                text+=" Admin Username - " + tInfo.AdminUser + "\t\n";
                text+=" Admin User Password - " + tInfo.AdminUserPwd + "\t\n";
                text+=" Org Username - " + tInfo.OrgUser + "\t\n";
                text+=" Org User Password - " + tInfo.OrgUserPwd + "\t\n";

                //text += " Tenant Name - AA" +"\t\n";
                //text += " Organization Name - AA" +"\t\n";
                //text += " Admin Role - AA" + "\t\n";
                //text += " User Role - AA" + "\t\n";
                //text += " Admin Username - AA" + "\t\n";
                //text += " Admin User Password - AA" + "\t\n";
                //text += " Org Username - AA"+ "\t\n";
                //text += " Org User Password - AA" +"\t\n";

                var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                var fileData = "data:text/csv;base64," + window.btoa(text);
                $(this).attr("href", fileData).attr("download", "file-" + d + ".txt");

                divLog.dialog('close');
                divLog = null;
            });
            divRight.append(btnSave);

            //divLog.dialog({
            //    width: 520,
            //    height: 284,
            //    resizable: false,
            //    modal: true
            //    //close: function () { };
            //});

        };
        this.disposeComponent = function () {
            txtTenant = null;
            txtOrg = null;
            txtUTenant = null;
            txtUOrg = null;
            cmbCurr = null;
            cmbCou = null;
            txtCity = null;
            cmbReg = null;
            chkOpprotunity = null;
            chkSR = null;
            chkProduct = null;
            chkCam = null;
            chkBP = null;
            btnLoadFile = null;
            btnCancel = null;
            btnOK = null;
            $busyDiv = null;
            //$root.dialog("close");
            $root = null;
        };

     
        this.getRoot = function () {
            return $root;
        };

      
    };


    //Must Implement with same parameter
    VSetup.prototype.sizeChanged = function (height, width) {
        //this.sizeChanged(height, width);
    };


    //Must Implement with same parameter
    VSetup.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        //frame.hideHeader(true);
        this.load();
        this.frame.getContentGrid().append(this.getRoot());

    };

    //Must implement dispose
    VSetup.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };




    VIS.Apps.AForms.VSetup = VSetup;
})(VIS, jQuery);