
; (function (VIS, $) {
    VIS.Framework = VIS.Framework || {};
    //form declaretion
    function UserPreference(isGmailSettings) {

        this.frame;
        this.windowNo;
        var $autocommit = null;
        var $storepws = null;
        var $autologin = null;
        var $transtab = null;
        var $acctab = null;
        var $autonewrecord = null;
        var $advanstab = null;
        var $cachewindows = null;
        var $tracecmb = null;
        var $tracefile = null;
        // var $printer = null;
        //var $printpreview = null;
        var $prefdate = null;
        var $dictionary = null;
        var $show = null;
        var $showmingrid = null;
        var $pagesize = null;
        var $printnative = null;
        var $windowpagesize = null;
        var $variablename = null;
        var $variablevalue = null;
        var $erroronly = null;
        var $reset = null;
        // var $sentmail = null;
        var $savetofile = null;
        var $btnlogDate = null;
        var $btnDownLog = null;
        var $error = null;

        var $Okbtn = null;
        var $cancelbtn = null;
        var $role = null;
        var $contextlist = null;
        //var $connDetail = null;
        //current object of the class
        var $self = this;
        var $tab = null;

        var $btnChangePws = null;
        var $currentPws = null;
        var $newPws = null;
        var $conformPws = null;
        var $chkEmail = null;
        var $chkNotice = null;
        var $chkSMS = null;
        var $chkFax = null;
        var $txtUserName = null;
        var $txtPws = null;
        var $btnEmailConfig = null;
        var $btnSubstitute = null;
        var $btnSaveChanges = null;
        var $btnSaveChangePassword = null;
        var lblRecordSave = null;
        var lblChangePasswordMessage = null;
        var btnSave = null;
        var lblPrefSave = null;
        var _arrOfarr = null;

        //*******Gmail Task Settings********
        var $btnGetGmailAuthCode = null;
        var $btnSyncGmailTasks = null;
        var $txtGmailAuthCode = null;
        var $chkIsSyncBackground = null;
        var $lblTaskSettingMessage = null;
        var chkIsSyncBackground = null;
        var isRemoveTaskLink = false;
        //***********************************

        //*******Gmail Calendar Settings********
        var $btnGetGmailCalAuthCode = null;
        var $btnSyncGmailCalendar = null;
        var $txtGmailCalAuthCode = null;
        var $chkCalIsSyncBackground = null;
        var $lblCalendarSettingMessage = null;
        var chkCalIsSyncBackground = null;
        var isRemoveCalendarLink = false;
        //***********************************

        //*******Gmail Contact Settings********

        var $btnGetGmailContactAuthCode = null;
        var $btnSyncGmailContact = null;
        var $txtGmailContactAuthCode = null;
        var $chkContactIsSyncBackground = null;
        var $lblContactSettingMessage = null;
        var chkContactIsSyncBackground = null;
        var isRemoveContactLink = false;

        var $txtGmailUsername = null;
        var $txtGmailPassword = null;
        var $cmbGmailRole = null;
        var $chkIsUpdateExisting = null;
        var chkIsUpdateExisting = null;
        var $lblContactSettingMessage = null;
        var $btnSyncGmailContacts = null;
        //***********************************


        //********Login Settings***********
        var $cmdRole = null;
        var $cmdClient = null;
        var $cmdOrg = null;
        var $cmdWareHouse = null;
        var defaultLogin = null;
        //*********************************
        var drpTheme = null;
        var ulTheme = null;

        var btnTheme = null;

        var $root = $("<div class='vis-forms-container'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>')
        var windowNo = VIS.Env.getWindowNo();
        this.log = VIS.Logging.VLogger.getVLogger("UserPreference");

        this.load = function () {

            //this.log.log(VIS.Logging.Level.SEVERE, "cretical error");
            //this.log.log(VIS.Logging.Level.INFO, "INFO");
            //this.log.log(VIS.Logging.Level.WARNING, "WARNING");
            //this.log.log(VIS.Logging.Level.OFF, "OFF");
            $busyDiv[0].style.visibility = 'visible';

            $root.load(VIS.Application.contextUrl + 'UserPreference/Index/?windowno=' + windowNo + '&adUserId=' + VIS.context.getAD_User_ID(), function (event) {

              

                $self.init($root);
                var divget = $root.find("#content_" + windowNo);
                if (divget != null) {
                    // divget.tabs();
                    //divget.tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
                    //divget.find("li").removeClass("ui-corner-top").addClass("ui-corner-left");
                }
                //remove image
                $busyDiv[0].style.visibility = 'hidden';
            });

        };

        this.init = function (root) {

            //Initialize or get controls
            // $connDetail = root.find("#txtConnectionDetail_" + windowNo);

            $autocommit = root.find("#chkAutoCommit_" + windowNo);
            $storepws = root.find("#chkStorePassword_" + windowNo);
            $autologin = root.find("#chkAutoLogin_" + windowNo);   //NotFound
            $transtab = root.find("#chkShowTranslationTab_" + windowNo);
            $acctab = root.find("#chkShowAccountingTab_" + windowNo);
            $autonewrecord = root.find("#chkAutoNewRecord_" + windowNo);
            $advanstab = root.find("#chkShowAdvancedTab_" + windowNo);
            $cachewindows = root.find("#chkCacheWindows_" + windowNo);
            $tracecmb = root.find("#cmbTraceLevel_" + windowNo);
            $tracefile = root.find("#chkTraceFile_" + windowNo);
            // $printer = root.find("#cmbPrinter_" + windowNo);
            //$printpreview = root.find("#chkPreviewPrint_" + windowNo);

            $prefdate = root.find("#dt_Date_" + windowNo);
            $dictionary = root.find("#chkDictionaryMaintain_" + windowNo); //NotFound
            $show = root.find("#cmbClientOrg_" + windowNo);
            $showmingrid = root.find("#chkShowMiniGrid_" + windowNo); //NotFound
            $pagesize = root.find("#cmbPageSize_" + windowNo);
            $printnative = root.find("#chkNativeDigit_" + windowNo);
            $windowpagesize = root.find("#cmbWindowPageSize_" + windowNo);

            $variablename = root.find("#lblVariableName_" + windowNo);
            $variablevalue = root.find("#lblVariableValue_" + windowNo);


            $Okbtn = root.find("#btnOk_" + windowNo);
            $cancelbtn = root.find("#btnCancel_" + windowNo);
            $role = root.find("#btnRole_" + windowNo); //NotFound
            $contextlist = root.find("#contextlst_" + windowNo);
            $error = root.find("#prefErrortbl_" + windowNo);
            $Okbtn.hide();
            $cancelbtn.hide();
            //User Setting control find
            $btnChangePws = root.find("#btnChangePws_" + windowNo);
            $currentPws = root.find("#txtCurrentPws_" + windowNo);
            $newPws = root.find("#txtNewPws_" + windowNo);
            $conformPws = root.find("#txtConformPws_" + windowNo);
            $chkEmail = root.find("#chkIsEmail_" + windowNo);
            $chkNotice = root.find("#chkIsNotice_" + windowNo);
            $chkSMS = root.find("#chkIsSMS_" + windowNo);
            $chkFax = root.find("#chkIsFax_" + windowNo);
            $txtUserName = root.find("#txtUserName_" + windowNo);
            $txtPws = root.find("#txtPws_" + windowNo);
            $btnEmailConfig = root.find("#btnEmailConfig_" + windowNo);
            $btnSubstitute = root.find("#btnSubstitute_" + windowNo);
            $btnSaveChanges = root.find("#btnSaveChanges_" + windowNo);
            $btnSaveChangePassword = root.find("#btnSaveChangePassword_" + windowNo);
            lblPrefSave = root.find("#lblPrefSave_" + windowNo);
            btnSave = root.find("#btnSave_" + windowNo);

            //*******Gmail Task Settings********
            $btnGetGmailAuthCode = root.find("#vbtnGmailAuth_" + windowNo);
            $btnSyncGmailTasks = root.find("#vbtnSyncGmailTask_" + windowNo);
            $txtGmailAuthCode = root.find("#txtAuthCode_" + windowNo);
            $chkIsSyncBackground = root.find("#chkIsSyncBackground_" + windowNo);
            $lblTaskSettingMessage = root.find("#lblTaskSettingMessage_" + windowNo);
            $lblTaskSettingMessage.hide();
            //getSavedTaskDetail();
            //**********************************

            //*******Gmail Calendar Settings********
            $btnGetGmailCalAuthCode = root.find("#vbtnGmailCalAuth_" + windowNo);
            $btnSyncGmailCalendar = root.find("#vbtnSyncGmailCalendar_" + windowNo);
            $txtGmailCalAuthCode = root.find("#txtCalAuthCode_" + windowNo);
            $chkCalIsSyncBackground = root.find("#chkCalIsSyncBackground_" + windowNo);
            $lblCalendarSettingMessage = root.find("#lblCalendarSettingMessage_" + windowNo);
            $lblCalendarSettingMessage.hide();
            //getSavedTaskDetail();
            // getSavedCalendarDetail();
            //**********************************

            //*******Gmail Contact Settings********
            $btnGetGmailContactAuthCode = root.find("#vbtnGmailContactAuth_" + windowNo);
            $btnSyncGmailContact = root.find("#vbtnSyncGmailContact_" + windowNo);
            $txtGmailContactAuthCode = root.find("#txtContactAuthCode_" + windowNo);
            // $chkContactIsSyncBackground = root.find("#chkContactIsSyncBackground_" + windowNo);
            $lblContactSettingMessage = root.find("#lblContactSettingMessage_" + windowNo);
            $lblContactSettingMessage.hide();
            getSavedTaskDetail();



            //*******Gmail Contact Settings********
            $txtGmailUsername = root.find("#txtGmailUserName_" + windowNo);
            $txtGmailPassword = root.find("#txtGmailPassword_" + windowNo);
            $cmbGmailRole = root.find("#cmbSettingRole_" + windowNo);
            $chkIsUpdateExisting = root.find("#chkbxUpdateExisting_" + windowNo);
            $btnSyncGmailContacts = root.find("#vbtnSyncGmailContact_" + windowNo);
            $lblContactSettingMessage = root.find("#lblContactSettingMessage_" + windowNo);
            $lblContactSettingMessage.hide();
            loadGmailContactSettings();
            //**********************************

            //********Login Settings***********
            $cmdRole = root.find("#cmbRole_" + windowNo);
            $cmdRole.on("change", function () { loadClient() });
            $cmdClient = root.find("#cmbClient_" + windowNo);
            $cmdClient.on("change", function () { loadOrg() });
            $cmdOrg = root.find("#cmbOrg_" + windowNo);
            $cmdOrg.on("change", function () { loadWH() });
            $cmdWareHouse = root.find("#cmbWareHouse_" + windowNo);

            $btnTheme = root.find("#btnTheme_"+windowNo)

            defaultLogin = {};
            loadDefault();
            //loadRoles();
            getThemes();
            //*********************************


            //Display message from MSG class on Menu
            var lblUserSetMenu = root.find("#lblUserSetMenu_" + windowNo);
            var lblPreferenceMenu = root.find("#lblPreferenceMenu_" + windowNo);
            var lblContextMenu = root.find("#lblContextMenu_" + windowNo);
            var lblErrorMenu = root.find("#lblErrorMenu_" + windowNo);

            lblUserSetMenu.text(VIS.Msg.getMsg("UserSetting"));
            lblPreferenceMenu.text(VIS.Msg.getMsg("Preference"));
            lblContextMenu.text(VIS.Msg.getMsg("Context"));
            lblErrorMenu.text(VIS.Msg.getMsg("PreferenceError"));

            var lblchangePwsHeader = root.find("#lblchangePwsHeader_" + windowNo);
            lblchangePwsHeader.text(VIS.Msg.getMsg("ChangePassword"));



            var lblAutoMatic = root.find("#lblAutoMatic_" + windowNo);
            lblAutoMatic.text(VIS.Msg.getMsg("AutoCommit"));

            var imgAutoMatic = root.find("#imgAutoMatic_" + windowNo);
            imgAutoMatic.text(VIS.Msg.getMsg("AutoCommit", true));

            var lblTranslation = root.find("#lblTranslation_" + windowNo);
            lblTranslation.text(VIS.Msg.getMsg("ShowTranslationTab"));

            var imgTranslation = root.find("#imgTranslation_" + windowNo);
            imgTranslation.text(VIS.Msg.getMsg("ShowTranslationTab"));


            var lblShowAccounting = root.find("#lblShowAccounting_" + windowNo);
            lblShowAccounting.text(VIS.Msg.getMsg("ShowAcctTab"));

            var imgShowAccounting = root.find("#imgShowAccounting_" + windowNo);
            imgShowAccounting.text(VIS.Msg.getMsg("ShowAcctTab"));


            var lblAutoNewRecord = root.find("#lblAutoNewRecord_" + windowNo);
            lblAutoNewRecord.text(VIS.Msg.getMsg("AutoNew"));

            var imgAutoNewRecord = root.find("#imgAutoNewRecord_" + windowNo);
            imgAutoNewRecord.text(VIS.Msg.getMsg("AutoNew"));


            var lblShowAdvanced = root.find("#lblShowAdvanced_" + windowNo);
            lblShowAdvanced.text(VIS.Msg.getMsg("ShowAdvancedTab"));

            var imgShowAdvanced = root.find("#imgShowAdvanced_" + windowNo);
            imgShowAdvanced.text(VIS.Msg.getMsg("ShowAdvancedTab"));


            var lblCacheWindows = root.find("#lblCacheWindows_" + windowNo);
            lblCacheWindows.text(VIS.Msg.getMsg("CacheWindow"));

            var imgCacheWindows = root.find("#imgCacheWindows_" + windowNo);
            imgCacheWindows.text(VIS.Msg.getMsg("CacheWindow"));


            var vlblClientOrg = root.find("#vlblClientOrg_" + windowNo);
            vlblClientOrg.text(VIS.Msg.getMsg("Show"));

            var imgClientOrg = root.find("#imgClientOrg_" + windowNo);
            imgClientOrg.text(VIS.Msg.getMsg("ClientOrg"));


            var lblWindowPageSize = root.find("#lblWindowPageSize_" + windowNo);
            lblWindowPageSize.text(VIS.Msg.getMsg("WindowPageSize"));

            var imgWindowPageSize = root.find("#imgWindowPageSize_" + windowNo);
            imgWindowPageSize.text(VIS.Msg.getMsg("WindowPageSize"));

            var lblAutoLogin = root.find("#lblAutoLogin_" + windowNo);
            lblAutoLogin.text(VIS.Msg.getMsg("AutoLogin"));

            var imgAutoLogin = root.find("#imgAutoLogin_" + windowNo);
            imgAutoLogin.text(VIS.Msg.getMsg("AutoLogin"));

            var lblStorePassword = root.find("#lblStorePassword_" + windowNo);
            lblStorePassword.text(VIS.Msg.getMsg("StorePassword"));

            var imgStorePassword = root.find("#imgStorePassword_" + windowNo);
            imgStorePassword.text(VIS.Msg.getMsg("StorePassword"));

            var lblTraceLevel = root.find("#lblTraceLevel_" + windowNo);
            lblTraceLevel.text(VIS.Msg.getMsg("TraceLevel"));

            var imgTraceLevel = root.find("#imgTraceLevel_" + windowNo);
            imgTraceLevel.text(VIS.Msg.getMsg("TraceLevel"));


            var lblTraceFile = root.find("#lblTraceFile_" + windowNo);
            lblTraceFile.text(VIS.Msg.getMsg("TraceFile"));

            var imgTraceFile = root.find("#imgTraceFile_" + windowNo);
            imgTraceFile.text(VIS.Msg.getMsg("TraceFile"));


            var lblNativeDigit = root.find("#lblNativeDigit_" + windowNo);
            lblNativeDigit.text(VIS.Msg.getMsg("PrintNativeDigits"));

            var imgNativeDigit = root.find("#imgNativeDigit_" + windowNo);
            imgNativeDigit.text(VIS.Msg.getMsg("PrintNativeDigits"));

            var vlblDateText = root.find("#vlblDateText_" + windowNo);
            vlblDateText.text(VIS.Msg.getMsg("Date"));

            var imgDateText = root.find("#imgDateText_" + windowNo);
            imgDateText.text(VIS.Msg.getMsg("Date"));

            var vlblTheme = root.find("#vlblthemeText_" + windowNo);
            vlblTheme.text(VIS.Msg.getMsg("SelectTheme"));

            var imgThemetext = root.find("#imgThemetext" + windowNo);
            imgThemetext.text(VIS.Msg.getMsg("SelectTheme"));

            drpTheme = root.find("#vis_pref_theme" + windowNo);
            ulTheme = drpTheme.find('ul');
                
            var vlblPageSize = root.find("#vlblPageSize_" + windowNo);
            vlblPageSize.text(VIS.Msg.getMsg("Pagesize"));

            var imgPageSize = root.find("#imgPageSize_" + windowNo);
            imgPageSize.text(VIS.Msg.getMsg("Pagesize"));


            var vlblPageSize = root.find("#vlblPageSize_" + windowNo);
            vlblPageSize.text(VIS.Msg.getMsg("Pagesize"));

            var lblContext = root.find("#lblContext_" + windowNo);
            lblContext.text(VIS.Msg.getMsg("Context"));


            //records Save
            lblRecordSave = root.find("#lblRecordSave_" + windowNo);
            lblChangePasswordMessage = root.find("#lblChangePasswordMessage_" + windowNo);
            lblRecordSave.hide();
            lblPrefSave.hide();
            lblChangePasswordMessage.hide();
            //paragraph
            var pchangePws = root.find("#pchangePws_" + windowNo);
            pchangePws.text(VIS.Msg.getMsg("PPasswordInfo"));

            var pNotification = root.find("#pNotification_" + windowNo);
            pNotification.text(VIS.Msg.getMsg("PNotificationInfo"));

            var pEmailConfig = root.find("#pEmailConfig_" + windowNo);
            pEmailConfig.text(VIS.Msg.getMsg("PEmailConfigInfo"));

            var pSubstitute = root.find("#pSubstitute_" + windowNo);
            pSubstitute.text(VIS.Msg.getMsg("PSubstituteInfo"));

            var pContext = root.find("#pContext_" + windowNo);
            pContext.text(VIS.Msg.getMsg("PContextInfo"));

            var pError = root.find("#pError_" + windowNo);
            pError.text(VIS.Msg.getMsg("PErrorInfo"));


            /* This is the function that will get executed after the DOM is fully loaded */
            //$prefdate.datepicker({
            //    changeMonth: true,//this option for allowing user to select month
            //    changeYear: true //this option for allowing user to select from year range
            //});

            //Load Trace level.
            if ($tracecmb != null) {
                var levelsValue = [VIS.Logging.Level.OFF.getIntValue(), VIS.Logging.Level.SEVERE.getIntValue(), VIS.Logging.Level.WARNING.getIntValue(),
                    VIS.Logging.Level.INFO.getIntValue(), VIS.Logging.Level.CONFIG.getIntValue(), VIS.Logging.Level.FINE.getIntValue(),
                    VIS.Logging.Level.FINER.getIntValue(), VIS.Logging.Level.FINEST.getIntValue(), VIS.Logging.Level.ALL.getIntValue()];

                var levelsName = [VIS.Logging.Level.OFF.getName(), VIS.Logging.Level.SEVERE.getName(), VIS.Logging.Level.WARNING.getName(),
                    VIS.Logging.Level.INFO.getName(), VIS.Logging.Level.CONFIG.getName(), VIS.Logging.Level.FINE.getName(),
                    VIS.Logging.Level.FINER.getName(), VIS.Logging.Level.FINEST.getName(), VIS.Logging.Level.ALL.getName()];

                var selectedIndex = 0;
                for (var i = 0; i < levelsValue.length; i++) {
                    $tracecmb.append(" <option value=" + levelsValue[i] + ">" + levelsName[i] + "</option>");
                    if (VIS.Ini.getLocalStorage(VIS.IniConstants.P_TRACELEVEL) == levelsValue[i]) {
                        selectedIndex = i;
                    }
                };
                $tracecmb.prop('selectedIndex', selectedIndex);
            };

            //show org.
            if ($show != null) {

                var msgvalues = [VIS.Msg.getMsg("Client+Org"), VIS.Msg.getMsg("Client"), VIS.Msg.getMsg("Org"), VIS.Msg.getMsg("None")];
                var selectedIndex = 0;
                for (var i = 0; i < msgvalues.length; i++) {
                    $show.append("<option>" + msgvalues[i] + "</option>");
                    if (VIS.Ini.getLocalStorage(VIS.IniConstants.CO_SHOWLEVEL) == i) {
                        selectedIndex = i;
                    }
                }
                $show.prop('selectedIndex', selectedIndex);
            };

            //load Page Size
            if ($pagesize != null) {
                var pagesizeArr = [5, 6, 7, 8, 9, 10];
                var selectedIndex = 0;
                for (var i = 0; i < pagesizeArr.length; i++) {
                    $pagesize.append("<option>" + pagesizeArr[i] + "</option>");
                    if (VIS.Ini.getLocalStorage(VIS.IniConstants.W_PAGESIZE) == pagesizeArr[i]) {
                        selectedIndex = i;
                    }
                }
                $pagesize.prop('selectedIndex', selectedIndex);
            }

            //load Window Page Size
            if ($windowpagesize != null) {

                var pagesizeArr = [50, 100, 500];
                var selectedIndex = 0;
                for (var i = 0; i < pagesizeArr.length; i++) {
                    $windowpagesize.append("<option>" + pagesizeArr[i] + "</option>");
                    if (VIS.Ini.getLocalStorage(VIS.IniConstants.WIN_PAGESIZE) == pagesizeArr[i]) {
                        selectedIndex = i;
                    }
                }
                $windowpagesize.prop('selectedIndex', selectedIndex);
            }


            //load context settting
            if ($contextlist != null) {
                var arr = VIS.context.getEntireCtx();//VIS.context.ctx;
                var textToInsert = [];
                var i = 0;
                var length = Object.keys(arr).length;
                for (var a = 0; a < length; a += 1) {
                    textToInsert[i++] = ' <li class="VIS_Pref_li-err" ><span >';
                    textToInsert[i++] = Object.keys(arr)[a] + " == " + arr[Object.keys(arr)[a]];
                    textToInsert[i++] = '</span></li>';
                }
                $contextlist.append(textToInsert.join(''));
                arr = null;
                textToInsert = null;
            };



            $erroronly = root.find("#vchkErrorOnly_" + windowNo);
            $reset = root.find("#vbtnReset_" + windowNo);
            // $sentmail = root.find("#vbtnSentMail_" + windowNo);
            $savetofile = root.find("#vbtnSaveToFile_" + windowNo);

            // check if did not logged in as System Administrator Role, 
            // then hide download server log and Date section from user preference
            if (VIS.context && !VIS.context.getAD_Role_ID() == 0)
                root.find(".VIS_Pref_err_btnLeft").hide();

            $btnlogDate = root.find("#vbtnLogDate_" + windowNo);
            $btnDownLog = root.find("#vbtnDownLog_" + windowNo);

            setTodaysDate($btnlogDate);

            $btnDownLog.on('click', function (e) {
                var dateLog = $btnlogDate.val();
                if (dateLog)
                    dateLog = new Date(dateLog).toISOString()
                else
                    dateLog = new Date().toISOString();
                setBusy(true);
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "UserPreference/DownloadServerLog",
                    dataType: "json",
                    data: {
                        logDate: dateLog,
                    },
                    success: function (zipName) {
                        if (zipName) {
                            var url = "TempDownload/" + zipName;
                            window.open(VIS.Application.contextUrl + url);
                            setTodaysDate($btnlogDate);
                        }
                        else {
                            VIS.ADialog.info("VIS_NoLogFound", null, null);
                        }
                        setBusy(false);
                    },
                    error: function (data) {
                        setBusy(false);
                    }
                });
            });

            // var hgt = $(".VIS_Pref_content-right-4").height() - 90;

            // $("#divError_" + windowNo).css("height", hgt + "px");

            //check error check box
            $erroronly.prop("checked", true);


            //Load error log settings
            if ($error != null) {
                var arrOfarr = VIS.Logging.VLogErrorBuffer.get(true).getLogData($erroronly.prop("checked"));
                _arrOfarr = arrOfarr;
                //var arrOfarr = VIS.Logging.VLogErrorBuffer.get(true).getLogData(true);
                displayErrors(arrOfarr);
            }



            //load errors into anchar tag
            //var test = 'data:csv;base64,' + window.btoa(arrOfarr);
            ////  var csvData = 'data:application/txt;charset=utf-8,' + encodeURIComponent(arrOfarr);
            //$savetofile.attr({
            //    'href': test,
            //    'target': '_blank'b,sdf
            //});

            //Theme changed Event

           // drpTheme.on()
            drpTheme.on("click", "div.vis-theme-rec", function (e) {
                var $tgt = $(e.currentTarget);
                var clr = $tgt.data("color");
                var id = Number($tgt.data("id"));

                                if (VIS.themeMgr)
                    VIS.themeMgr.applyTheme(clr);
                //save theme 
                VIS.dataContext.postJSONData(VIS.Application.contextUrl + 'Theme/SaveForUser', { id: id, uid: VIS.context.getAD_User_ID() }, function (e) {

                });
                
            });

            $btnTheme.on("click", function () {
                var thme = new VIS.ThemeCnfgtor();
                thme.show();

            });


            //Error get error list on click
            $savetofile.on('click', function () {
                arrOfarr = _arrOfarr;//VIS.Logging.VLogErrorBuffer.get(true).getLogData($erroronly.prop("checked"));
                if (arrOfarr.length > 0) {
                    var str = "";
                    for (var i = 0; i < arrOfarr.length; i++) {
                        str += arrOfarr[i] + "\n";
                    }
                    var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                    var fileData = "data:text/csv;base64," + window.btoa(str);
                    $(this).attr("href", fileData).attr("download", "file-" + d + ".csv");
                }
            });


            //Load values from database(context)
            setValuePref();

            $autocommit.prop("checked", VIS.Env.getCtx().isAutoCommit());
            $autonewrecord.prop("checked", VIS.Env.getCtx().isAutoNew());
            $autologin.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_A_LOGIN) == "N" ? false : true);
            $storepws.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_STORE_PWD) == "N" ? false : true);
            $showmingrid.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_Show_Mini_Grid) == "N" ? false : true);
            $printnative.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_Print_Native_Digits) == "Y" ? true : false);
            $cachewindows.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_CACHE_WINDOW) == "Y" ? true : false);
            // $printpreview.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_PRINTPREVIEW) == null ? false : VIS.Ini.getLocalStorage(VIS.IniConstants.P_PRINTPREVIEW));
            $tracefile.prop("checked", VIS.Ini.getLocalStorage(VIS.IniConstants.P_TRACEFILE) == "Y" ? true : false);


            //$prefdate.SetValue(null);
            var dt = new Date(Number(VIS.Env.getCtx().ctx["#Date"]));
            if (dt != null) {
                $prefdate.val(Globalize.format(dt, "yyyy-MM-dd"));
            }

            //Events
            //Context list click
            $contextlist.on("click", "li", function () {
                var ctxText = $(this).text().split("==")[0];
                var ctxValue = $(this).text().split("==")[1];

                $variablename.text(ctxText.toString());
                $variablevalue.text(ctxValue.toString());
            });

            //Menu click element
            $('.VIS_Pref_meni li').on('click', function () {
                $('.VIS_Pref_menu li').removeClass('active');
                $(this).addClass('active');
            });

            /**********************click events***************************************/
            $reset.on("click", function () {
                var arrOfarr = VIS.Logging.VLogErrorBuffer.get(true).resetBuffer($erroronly.prop("checked"));
                _arrOfarr = arrOfarr;
                if (arrOfarr != null) {
                    displayErrors(arrOfarr);
                }
                else {
                    $error.find("tr:gt(0)").remove();
                }
            });

            //Tab 1
            //Show Password
            $(".VIS_Pref_btn-pass-click").on("click", function () {
                $(".VIS_Pref_new-password").css("display", "block");
                $(".VIS_Pref_Save").css("display", "block");
            });

            $(".VIS_Pref_close-btn").click(function () {
                $(".VIS_Pref_new-password").css("display", "none");
                $(".VIS_Pref_Save").css("display", "none");
            });

            // menu Link click
            $(".VIS_Pref_link-1").click(function () {
                $(".VIS_Pref_content-right").css("display", "block");
                $(".VIS_Pref_content-right-2").css("display", "none");
                $(".VIS_Pref_content-right-3").css("display", "none");
                $(".VIS_Pref_content-right-4").css("display", "none");
                $(".VIS_Pref_content-right-5").css("display", "none");
            });
            $(".VIS_Pref_link-2").click(function () {
                $(".VIS_Pref_content-right").css("display", "none");
                $(".VIS_Pref_content-right-2").css("display", "block");
                $(".VIS_Pref_content-right-3").css("display", "none");
                $(".VIS_Pref_content-right-4").css("display", "none");
                $(".VIS_Pref_content-right-5").css("display", "none");
            });
            $(".VIS_Pref_link-3").click(function () {
                $(".VIS_Pref_content-right").css("display", "none");
                $(".VIS_Pref_content-right-2").css("display", "none");
                $(".VIS_Pref_content-right-3").css("display", "block");
                $(".VIS_Pref_content-right-4").css("display", "none");
                $(".VIS_Pref_content-right-5").css("display", "none");
            });
            $(".VIS_Pref_link-4").click(function () {
                $(".VIS_Pref_content-right").css("display", "none");
                $(".VIS_Pref_content-right-2").css("display", "none");
                $(".VIS_Pref_content-right-3").css("display", "none");
                $(".VIS_Pref_content-right-4").css("display", "flex");
                $(".VIS_Pref_content-right-5").css("display", "none");
            });
            //**************************************
            //Added By Sarab for the purpose of Gmail Settings to Import Gmail Tasks
            $(".VIS_Pref_link-5").click(function () {
                if (window.WSP) {
                    $(".VIS_Pref_content-right").css("display", "none");
                    $(".VIS_Pref_content-right-2").css("display", "none");
                    $(".VIS_Pref_content-right-3").css("display", "none");
                    $(".VIS_Pref_content-right-4").css("display", "none");
                    $(".VIS_Pref_content-right-5").css("display", "block");
                }
                else {
                    alert("please download WSP !!!");
                }
            });
            if (isGmailSettings) {
                $(".VIS_Pref_link-5").trigger("click");
            }
            //************************************
            btnSave.on("click", function () {
                save_Preference();
            });

            $Okbtn.on("click", function () {
                save_Preference();
            });

            $cancelbtn.on("click", function () {
                // $self.disposeComponent();
                $root.dialog('close');

            });

            $erroronly.on("click", function () {
                var arrOfarr = null;
                $error.find('tr').slice(1, -1).remove()
                arrOfarr = VIS.Logging.VLogErrorBuffer.get(true).getLogData($erroronly.prop("checked"));
                _arrOfarr = arrOfarr;
                if (arrOfarr != null) {
                    displayErrors(arrOfarr);
                }
            });

            $role.on("click", function () {
                var text = VIS.MRole.toStringX(VIS.Env.getCtx());
            });

            $btnSaveChangePassword.on("click", function () {

                var currentPws = $currentPws.val();
                var newPws = $newPws.val();
                var conformPws = $conformPws.val();
                var emailUserName = $txtUserName.val();
                var emailPws = $txtPws.val();

                //if (currentPws.length <= 0 || newPws.length <= 0 || conformPws.length <= 0 || emailUserName.length <= 0 || emailPws.length <= 0) {
                //    VIS.ADialog.warn("Please fill all mandatory Fields!", null, null);
                //    return;
                //}

                // Updated by vinay bhatt to solve password notchanging fro user preferences 18 oct 2018
                //if ((currentPws.length <= 0 && newPws.length <= 0 && conformPws.length <= 0) || (currentPws.length > 0 && newPws.length > 0 && conformPws.length > 0)) {
                //    //then  countinue with changes
                //}
                if (currentPws.length <= 0 || newPws.length <= 0 || conformPws.length <= 0) {
                    lblChangePasswordMessage.css('color', 'red');
                    lblChangePasswordMessage.text(VIS.Msg.getMsg("FillMandatory"));
                    lblChangePasswordMessage.show();
                    return;
                }
                //match confirm password
                else if (newPws != conformPws) {
                    lblChangePasswordMessage.css('color', 'red');
                    lblChangePasswordMessage.text(VIS.Msg.getMsg("samePassword"));
                    lblChangePasswordMessage.show();
                    return;
                }


                                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/SaveChangePassword",
                    dataType: "json",
                    data: {

                        AD_User_ID: VIS.context.getAD_User_ID(),
                        currentPws: currentPws,
                        newPws: newPws,

                    },
                    success: function (data) {
                        returnValue = data.result;
                        lblChangePasswordMessage.show();
                        if (returnValue == "OK") {
                            lblChangePasswordMessage.css('color', 'green');
                            lblChangePasswordMessage.text(VIS.Msg.getMsg("Passwordchanged"));
                        }
                        else {
                            lblChangePasswordMessage.css('color', 'red');
                            lblChangePasswordMessage.text(returnValue);
                        }

                        //lblChangePasswordMessage.text(returnValue);
                        //if (!returnvalue.issaved) {
                        //    lblchangepasswordmessage.css('color', 'red');
                        //VIS.ADialog.warn(returnValue.Msg, null, null);
                        //}
                    }
                });


            });


            $btnSaveChanges.on("click", function () {

                var currentPws = $currentPws.val();
                var newPws = $newPws.val();
                var conformPws = $conformPws.val();
                var emailUserName = $txtUserName.val();
                var emailPws = $txtPws.val();

                //if (currentPws.length <= 0 || newPws.length <= 0 || conformPws.length <= 0 || emailUserName.length <= 0 || emailPws.length <= 0) {
                //    VIS.ADialog.warn("Please fill all mandatory Fields!", null, null);
                //    return;
                //}

                if ((currentPws.length <= 0 && newPws.length <= 0 && conformPws.length <= 0) || (currentPws.length > 0 && newPws.length > 0 && conformPws.length > 0)) {
                    //then  countinue with changes
                }
                else {
                    lblRecordSave.text(VIS.Msg.getMsg("mandatoryFields"));
                    return;
                }

                //match confirm password
                if ($newPws.text().localeCompare($conformPws.text()) == -1) {
                    lblRecordSave.text(VIS.Msg.getMsg("samePassword"));
                    return;
                }

                var AD_Role_ID = $cmdRole.val();
                var AD_Client_ID = $cmdClient.val();
                var AD_Org_ID = $cmdOrg.val();
                var AD_WH_ID = $cmdWareHouse.val();
                if (AD_Role_ID == undefined || AD_Client_ID == undefined || AD_Org_ID == undefined) {
                    lblRecordSave.text(VIS.Msg.getMsg("VIS_FillLogin"));
                    return;
                }
                if (AD_WH_ID == undefined) {
                    AD_WH_ID = 0;
                }


                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/SaveUserSettings",
                    dataType: "json",
                    data: {

                        AD_User_ID: VIS.context.getAD_User_ID(),
                        currentPws: currentPws,
                        newPws: newPws,
                        conformPws: conformPws,
                        chkEmail: $chkEmail.prop("checked"),
                        chkNotice: $chkNotice.prop("checked"),
                        chkSMS: $chkSMS.prop("checked"),
                        chkFax: $chkFax.prop("checked"),
                        emailUserName: emailUserName,
                        emailPws: emailPws,
                        AD_Role_ID: AD_Role_ID,
                        AD_Client_ID: AD_Client_ID,
                        AD_Org_ID: AD_Org_ID,
                        M_Warehouse_ID: AD_WH_ID
                    },
                    success: function (data) {
                        returnValue = data.result;
                        lblRecordSave.show();
                        lblRecordSave.css('color', 'green');
                        lblRecordSave.text(returnValue.Msg);
                        if (!returnValue.IsSaved) {
                            lblRecordSave.css('color', 'red');
                            //VIS.ADialog.warn(returnValue.Msg, null, null);
                        }
                    }
                });
            });

            $btnEmailConfig.on("click", function () {
                //get windowId for User Window
                //var sql = "select ad_window_id from ad_window where name='Mail Configuration'";// Upper( name)=Upper('user' )
                var ad_window_Id = 0;
                try {
                    //var dr = VIS.DB.executeDataReader(sql);
                    //if (dr.read()) {
                    //    ad_window_Id = dr.getInt(0);
                    //}
                    //dr.dispose();
                    // Added by Bharat on 12 June 2017 to remove client side queries
                    ad_window_Id = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "UserPreference/GetWindowID", { "WindowName": "Mail Configuration" }, null); // spelling corrected by vinay bhatt on 18 oct 2018
                    if (ad_window_Id > 0) {
                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction("AD_User_ID", VIS.Query.prototype.EQUAL, VIS.context.getAD_User_ID());
                        VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                    }
                }
                catch (e) {
                    this.log.log(VIS.Logging.Level.SEVERE, sql, e);
                }
                $root.dialog('close');
            });

            $btnSubstitute.on("click", function () {
                //get windowId for User Window
                //var sql = "select ad_window_id from ad_window where name='User Substitute'";// Upper( name)=Upper('user' )
                var ad_window_Id = 0;
                try {
                    //var dr = VIS.DB.executeDataReader(sql);
                    //if (dr.read()) {
                    //    ad_window_Id = dr.getInt(0);
                    //}
                    //dr.dispose();
                    // Added by Bharat on 12 June 2017 to remove client side queries
                    ad_window_Id = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "UserPreference/GetWindowID", { "WindowName": "User Substitute" }, null); // spelling corrected by vinay bhatt on 18 oct 2018
                    if (ad_window_Id > 0) {
                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction("AD_User_ID", VIS.Query.prototype.EQUAL, VIS.context.getAD_User_ID());
                        VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                    }
                }
                catch (e) {
                    this.log.log(VIS.Logging.Level.SEVERE, sql, e);
                }
                $root.dialog('close');
            });

            //over riding the local object event
            //$root.dialog({
            //    beforeClose: function (event, ui) {
            //        $self.disposeComponent();
            //     }
            //});

            //$sentmail.on("click", function () {
            //    alert("$vbtnSentMail click");
            //});


            //*****************Added By Sarab*************

            $btnGetGmailAuthCode.on("click", function () {

                getGmailAuthorizationCode(true, false);

            });
            $btnSyncGmailTasks.on("click", function () {

                $lblTaskSettingMessage.css("color", "gray");
                $lblTaskSettingMessage.text(VIS.Msg.getMsg("PleaseWait"));
                $lblTaskSettingMessage.show();
                window.setTimeout(function () {
                    saveGmailAuthSettings(true, chkIsSyncBackground, $txtGmailAuthCode.val(), isRemoveTaskLink, false);
                }, 500);

            });

            $chkIsSyncBackground.on("click", function (event) {

                chkIsSyncBackground = event.originalEvent.target.checked;;
            });

            $btnGetGmailContactAuthCode.on("click", function () {
                getGmailAuthorizationCode(false, true);
            });
            $btnSyncGmailContact.on("click", function () {
                $lblContactSettingMessage.css("color", "gray");
                $lblContactSettingMessage.text(VIS.Msg.getMsg("PleaseWait"));
                $lblContactSettingMessage.show();
                window.setTimeout(function () {
                    saveGmailAuthSettings(false, false, $txtGmailContactAuthCode.val(), isRemoveContactLink, true);
                }, 500);
            });

            $btnGetGmailCalAuthCode.on("click", function () {

                getGmailAuthorizationCode(false, false);

            });
            $btnSyncGmailCalendar.on("click", function () {

                $lblCalendarSettingMessage.css("color", "gray");
                $lblCalendarSettingMessage.text(VIS.Msg.getMsg("PleaseWait"));
                $lblCalendarSettingMessage.show();
                window.setTimeout(function () {
                    saveGmailAuthSettings(false, chkCalIsSyncBackground, $txtGmailCalAuthCode.val(), isRemoveCalendarLink, false);
                }, 500);

            });

            $chkCalIsSyncBackground.on("click", function (event) {

                chkCalIsSyncBackground = event.originalEvent.target.checked;;
            });

            $chkIsUpdateExisting.on("click", function (event) {

                chkIsUpdateExisting = event.originalEvent.target.checked;;
            });

            $btnSyncGmailContacts.on("click", function () {

                if ($txtGmailUsername.val() == "" || $txtGmailUsername.val().length == 0 || $txtGmailPassword.val() == "" || $txtGmailPassword.val().length == 0) {
                    return;
                }
                $lblContactSettingMessage.css("color", "gray");
                $lblContactSettingMessage.text(VIS.Msg.getMsg("PleaseWait"));
                $lblContactSettingMessage.show();
                window.setTimeout(function () {
                    SaveGmailContactSettings($txtGmailUsername.val(), $txtGmailPassword.val(), $cmbGmailRole.val(), chkIsUpdateExisting);
                }, 500);
            });

            function setBusy(isBusy) {
                if (isBusy) 
                    $busyDiv[0].style.visibility = 'visible';
                else
                    $busyDiv[0].style.visibility = 'hidden';
            };

            function setTodaysDate(ctrl) {
                var now = new Date();
                var day = ("0" + now.getDate()).slice(-2);
                var month = ("0" + (now.getMonth() + 1)).slice(-2);
                var today = now.getFullYear() + "-" + (month) + "-" + (day);
                ctrl.val(today);
            };

            function getSavedTaskDetail() {

                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/GetSavedDetail",
                    dataType: "json",
                    data: {
                        isTask: true,
                    },
                    success: function (data) {
                        isRemoveCalendarLink = false;
                        isRemoveTaskLink = false;
                        isRemoveContactLink = false;
                        var dic = JSON.parse(data);
                        if (dic["IsSyncInBackground"] == "false") {
                            $chkIsSyncBackground.prop("checked", false);
                            chkIsSyncBackground = false;
                        }
                        else {
                            $chkIsSyncBackground.prop("checked", true);
                            chkIsSyncBackground = true;
                        }
                        if (dic["IsCalSyncInBackground"] == "false") {
                            $chkCalIsSyncBackground.prop("checked", false);
                            chkCalIsSyncBackground = false;
                        }
                        else {
                            $chkCalIsSyncBackground.prop("checked", true);
                            chkCalIsSyncBackground = true;
                        }
                        $txtGmailAuthCode.val(dic["TaskAuthCode"]);
                        if ($txtGmailAuthCode.val().length > 0) {
                            $btnSyncGmailTasks.html(VIS.Msg.getMsg('RemoveLink'));
                            isRemoveTaskLink = true;
                        }
                        $txtGmailCalAuthCode.val(dic["CalendarAuthCode"]);
                        if ($txtGmailCalAuthCode.val().length > 0) {
                            $btnSyncGmailCalendar.html(VIS.Msg.getMsg('RemoveLink'));
                            isRemoveCalendarLink = true;
                        }
                        $txtGmailContactAuthCode.val(dic["ContactAuthCode"]);
                        if ($txtGmailContactAuthCode.val().length > 0) {
                            $btnSyncGmailContact.html(VIS.Msg.getMsg('RemoveLink'));
                            isRemoveContactLink = true;
                        }

                    }
                });

            };

            function getThemes() {

                var url = VIS.Application.contextUrl + "JsonData/GetThemes";
                VIS.dataContext.getJSONData(url, null, getThemeCompleted);
            };

            function getThemeCompleted(lst) {
                var html = [];
                if (lst) {
                    for (var i = 0, j = lst.length; i < j; i++) {
                        var data = lst[i];
                        html.push('<li>');
                        html.push('<div class="vis-theme-rec" data-color="')
                        html.push(data.PColor);
                        html.push('|');
                        html.push(data.OnPColor);
                        html.push('|');
                        html.push(data.SColor);
                        html.push('|');
                        html.push(data.OnSColor);
                        html.push('" data-id="' + data.Id+'" >');
                        html.push('<span class="vis-theme-color" style="background-color:rgba(' + data.PColor + ',1)"></span>');
                        html.push('<span class="vis-theme-color" style="background-color:rgba(' + data.OnPColor + ',1)"></span>');
                        html.push('<span class="vis-theme-color" style="background-color:rgba(' + data.SColor + ',1)"></span>');
                        html.push('<span class="vis-theme-color" style="background-color:rgba(' + data.OnSColor + ',1)"></span>');
                        html.push('</div>');
                        html.push('</li>');
                    }
                    ulTheme.append(html.join(''));
                }
            };

            function getSavedCalendarDetail() {

                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/GetSavedDetail",
                    dataType: "json",
                    data: {
                        isTask: false,
                    },
                    success: function (data) {

                        var dic = JSON.parse(data);
                        if (dic["IsSyncInBackground"] == "false") {
                            $chkCalIsSyncBackground.prop("checked", false);
                            chkCalIsSyncBackground = false;
                        }
                        else {
                            $chkCalIsSyncBackground.prop("checked", true);
                            chkCalIsSyncBackground = true;
                        }
                        $txtGmailCalAuthCode.val(dic["CalendarAuthCode"]);
                    }
                });

            };

            function getGmailAuthorizationCode(istask, isContact) {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/GetGmailAuthorizationCode",
                    dataType: "json",
                    data: {

                        isTask: istask, isContact: isContact

                    },
                    success: function (data) {

                        var url = JSON.parse(data)
                        if (url != null && url.length > 0) {
                            window.open(url);
                        }
                    }
                });
            };

            function saveGmailAuthSettings(isTask, chkSyncBackground, authCode, isRLink, isContact) {

                $.ajax({
                    type: "POST",
                    async: false,
                    url: VIS.Application.contextUrl + "UserPreference/SaveGmailAccountSettings",
                    dataType: "json",
                    data: {

                        authCodes: authCode,
                        isTask: isTask,
                        isSyncInBackground: chkSyncBackground,
                        isRemoveLink: isRLink,
                        isContact: isContact

                    },
                    success: function (data) {

                        var msg = JSON.parse(data)
                        if (isTask) {
                            if (msg.toString() == "false") {
                                $lblTaskSettingMessage.css("color", "red");
                                $lblTaskSettingMessage.text(VIS.Msg.getMsg("RecordNotSaved"));
                                $lblTaskSettingMessage.show();
                            }
                            else if (msg.toString() == "true") {
                                $lblTaskSettingMessage.css("color", "green");
                                $lblTaskSettingMessage.text(VIS.Msg.getMsg("RecordSaved"));
                                if (isRLink) {
                                    $txtGmailAuthCode.val("");
                                    $btnSyncGmailTasks.html(VIS.Msg.getMsg('Save'));
                                    $chkIsSyncBackground.prop("checked", false);
                                    isRemoveTaskLink = false;
                                    $txtGmailAuthCode.removeAttr("disabled");
                                    $lblTaskSettingMessage.text(VIS.Msg.getMsg("LinkRemoved"));

                                }
                                else {
                                    $btnSyncGmailTasks.html(VIS.Msg.getMsg('RemoveLink'));
                                    $txtGmailAuthCode.attr("disabled", true);
                                    isRemoveTaskLink = true;
                                }
                                $lblTaskSettingMessage.show();
                            }
                            else {
                                window.open(msg);
                            }
                        }
                        else if (isContact) {
                            if (msg.toString() == "false") {
                                $lblContactSettingMessage.css("color", "red");
                                $lblContactSettingMessage.text(VIS.Msg.getMsg("RecordNotSaved"));
                                $lblContactSettingMessage.show();
                            }
                            else if (msg.toString() == "true") {
                                $lblContactSettingMessage.css("color", "green");
                                $lblContactSettingMessage.text(VIS.Msg.getMsg("RecordSaved"));
                                if (isRLink) {
                                    $txtGmailContactAuthCode.val("");
                                    $btnSyncGmailContact.html(VIS.Msg.getMsg('Save'));
                                    //$chkIsSyncBackground.prop("checked", false);
                                    isRemoveContactLink = false;
                                    $txtGmailContactAuthCode.removeAttr("disabled");
                                    $lblContactSettingMessage.text(VIS.Msg.getMsg("LinkRemoved"));

                                }
                                else {
                                    $btnSyncGmailContact.html(VIS.Msg.getMsg('RemoveLink'));
                                    $txtGmailContactAuthCode.attr("disabled", true);
                                    isRemoveContactLink = true;
                                }
                                $lblContactSettingMessage.show();
                            }
                            else {
                                window.open(msg);
                            }
                        }
                        else {
                            if (msg.toString() == "false") {
                                $lblCalendarSettingMessage.css("color", "red");
                                $lblCalendarSettingMessage.text(VIS.Msg.getMsg("RecordNotSaved"));
                                $lblCalendarSettingMessage.show();
                            }
                            else if (msg.toString() == "true") {
                                $lblCalendarSettingMessage.css("color", "green");
                                $lblCalendarSettingMessage.text(VIS.Msg.getMsg("RecordSaved"));
                                if (isRLink) {
                                    $txtGmailCalAuthCode.val("");
                                    $btnSyncGmailCalendar.html(VIS.Msg.getMsg('Save'));
                                    $chkCalIsSyncBackground.prop("checked", false);
                                    isRemoveCalendarLink = false;
                                    $txtGmailCalAuthCode.removeAttr("disabled");
                                    $lblCalendarSettingMessage.text(VIS.Msg.getMsg("LinkRemoved"));
                                }
                                else {
                                    $btnSyncGmailCalendar.html(VIS.Msg.getMsg('RemoveLink'));
                                    $txtGmailCalAuthCode.attr("disabled", true);
                                    isRemoveCalendarLink = true;
                                }
                                $lblCalendarSettingMessage.show();

                            }
                            else {
                                window.open(msg);
                            }
                        }
                        //if (msg != null) {
                        //    window.open(msg);
                        //}
                        //else {                            
                        //    $txtGmailAuthCode.val(" ");
                        //}
                    }
                });
            };

            function loadGmailContactSettings() {
                getrole();
                if ($cmbGmailRole != null) {
                    $cmbGmailRole.append(role);
                };
                getsaveddetail();
            };

            //************************************
            // This function is used to get saved credientials 
            //************************************
            function getsaveddetail() {
                if (!window.WSP)
                    return;
                var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetSavedDetail";
                $.ajax({
                    type: "GET",
                    async: true,
                    url: url,
                    dataType: "json",
                    success: function (data) {

                        var dd = JSON.parse(data);
                        //if settings are saved in database(WSP_GmalConfigration)then set those values in setting dialog Box controls
                        $txtGmailUsername.val(dd["Username"]);
                        $txtGmailPassword.val(dd["Password"]);
                        if (dd["Role"] != "") {
                            $cmbGmailRole.val(dd["Role"])
                        }
                        var update = dd["IsUpdate"];
                        if (update.toString().toLowerCase() == "false") {
                            ($chkIsUpdateExisting).prop("checked", false);
                            chkIsUpdateExisting = false;
                        }
                        else {
                            ($chkIsUpdateExisting).prop("checked", true);
                            chkIsUpdateExisting = true;
                        }


                    }
                });
            };

            //**********************
            //Get Roles in combobox
            //*********************
            var role = null;
            function getrole() {
                if (!window.WSP)
                    return;
                var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetRole";
                $.ajax({
                    type: "GET",
                    async: true,
                    url: url,
                    dataType: "json",

                    success: function (data) {

                        var dd = JSON.parse(data);
                        role += "<option value='-1'  selected='selected'>" + VIS.Msg.getMsg("SelectRole") + "</option>";
                        for (var i = 0; i < dd.length; i++) {
                            role += "<option value='" + dd[i]["RoleID"] + "'>" + dd[i]["RoleName"] + "</option>";
                        }

                    }
                });
            };


            function SaveGmailContactSettings(uName, pWord, roleID, updateExisting) {

                var url = VIS.Application.contextUrl + "WSP/ContactSettings/SaveGmailContactSettings";
                $.ajax({
                    type: "POST",
                    async: false,
                    url: url,
                    dataType: "json",
                    data: {

                        username: uName,
                        password: pWord,
                        role_ID: roleID,
                        isUpdateExisting: updateExisting,

                    },
                    success: function (dynData) {

                        var msg = JSON.parse(dynData);
                        if (msg.toString() == "false") {
                            $lblContactSettingMessage.css("color", "red");
                            $lblContactSettingMessage.text(VIS.Msg.getMsg("RecordNotSaved"));
                            $lblContactSettingMessage.show();
                        }
                        else {
                            $lblContactSettingMessage.css("color", "green");
                            $lblContactSettingMessage.text(VIS.Msg.getMsg("RecordSaved"));
                            $lblContactSettingMessage.show();
                        }

                    }
                });
            }
            //********************************************


            function save_Preference() {

                VIS.Env.getCtx().setContext("AutoCommit", $autocommit.prop("checked") ? "Y" : "N");

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_A_NEW, $autonewrecord.prop("checked") ? "Y" : "N");
                VIS.Env.getCtx().setContext("AutoNew", $autonewrecord.prop("checked") ? "Y" : "N");

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_TRACEFILE, $tracefile.prop("checked") ? "Y" : "N");
                // set cmbTraceLevel.SelectedItem;

                var levelInt = $tracecmb.find('option:selected').val();

                //	TraceLevel/File
                //var level = log.Level.getLevel(levelInt);
                var level = VIS.Logging.Level.prototype.getLevel(levelInt);
                //VIS.VLogMgt.setLevel(level);
                VIS.Logging.VLogMgt.setLevel(level);

                //VIS.Ini.setProperty(Ini.P_TRACELEVEL, level.getName());

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_TRACELEVEL, $tracecmb.find('option:selected').val());
                var index = $show[0].selectedIndex;

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.CO_SHOWLEVEL, index < 0 ? 0 : index);

                VIS.Env.getCtx().setContext("#ClientOrgLevel", index < 0 ? 0 : index);

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.W_PAGESIZE, $pagesize.find('option:selected').val());
                VIS.Env.getCtx().setContext("#ShowWorkSpacePageSize", $pagesize.find('option:selected').val());

                var winPageSize = $windowpagesize.find('option:selected').val();
                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.WIN_PAGESIZE, winPageSize);
                VIS.Env.setWINDOW_PAGE_SIZE(winPageSize);

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_Show_Mini_Grid, $showmingrid.prop("checked") ? "Y" : "N");
                VIS.Env.getCtx().setContext("#ShowMiniGrid", $showmingrid.prop("checked") ? "Y" : "N");

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_Print_Native_Digits, $printnative.prop("checked") ? "Y" : "N");
                VIS.Env.getCtx().setContext("#PrintNativeDigits", $printnative.prop("checked") ? "Y" : "N");

                //	AutoLogin
                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_A_LOGIN, $autologin.prop("checked") ? "Y" : "N");
                //	Save Password
                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_STORE_PWD, $storepws.prop("checked") ? "Y" : "N");
                //	Show Acct Tab
                VIS.Env.getCtx().setContext("#ShowAcct", $acctab.prop("checked") ? "Y" : "N");
                //	Show Trl Tab
                VIS.Env.getCtx().setContext("#ShowTrl", $transtab.prop("checked") ? "Y" : "N");

                //	Show Advanced Tab
                VIS.Env.getCtx().setContext("#ShowAdvanced", $advanstab.prop("checked") ? "Y" : "N");

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_CACHE_WINDOW, $cachewindows.prop("checked") ? "Y" : "N");

                //  Print Preview
                // VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_PRINTPREVIEW, $printpreview.prop("checked") ? "Y" : "N");

                VIS.Ini.setValueToLocalStorage(VIS.IniConstants.P_TRACEFILE, $tracefile.prop("checked") ? "Y" : "N");

                //save context value on server side
                //m_preference.Save();


                var callbackValue = savePreference({
                    IsAutoCommit: $autocommit.prop("checked") ? "Y" : "N",
                    IsShowAcct: $acctab.prop("checked") ? "Y" : "N",
                    IsShowTrl: $transtab.prop("checked") ? "Y" : "N",
                    IsShowAdvanced: $advanstab.prop("checked") ? "Y" : "N"
                });
            };

            function displayErrors(arrOfarr) {
                var length = arrOfarr.length;
                var textToInsert = "";
                for (var a = 0; a < length; a += 1) {
                    textToInsert += "<tr class='VIS_Pref-tbl-Row'>";
                    for (var i = 0; i < arrOfarr[a].length; i += 1) {
                        var obj = arrOfarr[a][i];
                        if (i == 0) {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {

                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                        else if (i == arrOfarr[a].length - 1) {
                            if (a % 2) {

                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                        else {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                    }
                    textToInsert += " </tr>";
                }

                //Append last row in the table
                //var error = "vchkErrorOnly_" + windowNo;
                //var reset = "vbtnReset_" + windowNo;
                //var errSave = "vbtnSaveToFile_" + windowNo;
                //textToInsert += '<tr>' +
                //    '<td style="border-left: 1px solid #ECECEC; border-bottom: 1px solid #ECECEC; border-bottom-left-radius: 5px;"></td>' +
                //    '<td style="border-bottom: 1px solid #ECECEC;"></td>' +
                //    '<td style="border-bottom: 1px solid #ECECEC;">' +
                //        '<input id=' + error + ' type="checkbox" class="VIS_Pref_check"><span>Errors only </span>' +
                //        '<button id=' + reset + ' class="VIS_Pref_pass-btn" style="margin-left: 30px;">Reset</button>' +
                //    '</td>' +
                //    '<td style="border-left: 0px; border-bottom: 1px solid #ECECEC; border-bottom-right-radius: 5px;" class="VIS_Pref_table-row1">' +
                //        '<button id=' + errSave + ' class="VIS_Pref_pass-btn">Export data into Excel</button>' +
                //    '</td>' +
                //'</tr>';


                $error.find('tbody > tr').eq(0).after($.parseHTML(textToInsert));
                arr = null;
                textToInsert = null;
            };

            function setValuePref() {
                //if (VIS.MRole.IsShowAcct())
                //{
                $acctab.prop("checked", VIS.Env.getCtx().ctx["#ShowAcct"] == "Y" ? true : false);
                //}
                //else {
                //    $acctab.prop("checked", false);
                //    $acctab.prop("enabled", false);
                //}

                //	Show Trl/Advanced Tab
                $transtab.prop("checked", VIS.Env.getCtx().ctx["#ShowTrl"] == "Y" ? true : false);
                $advanstab.prop("checked", VIS.Env.getCtx().ctx["#ShowAdvanced"] == "Y" ? true : false);
            }

            function savePreference(data, callback) {
                var result = null;
                $.ajax({
                    url: VIS.Application.contextUrl + "UserPreference/SavePrefrence",
                    type: "POST",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    data: JSON.stringify({ pref: data })
                }).done(function (json) {

                    result = json;
                    $root.dialog('close');
                })
            };



        };

        var loadDefault = function () {
            $.ajax({
                url: VIS.Application.contextUrl + "UserPreference/GetDefaultLogin",
                dataType: "json",
                //async: false,
                data: { AD_User_ID: VIS.context.getAD_User_ID() },
                success: function (data) {
                    var di = JSON.parse(data);
                    defaultLogin.AD_Role_ID = di.AD_Role_ID;
                    defaultLogin.AD_Client_ID = di.AD_Client_ID;
                    defaultLogin.AD_Org_ID = di.AD_Org_ID;
                    defaultLogin.M_Warehouse_ID = di.M_Warehouse_ID;
                    loadRoles();
                }
            });
        };
        var loadRoles = function () {
            var sql = "SELECT  r.Name,r.AD_Role_ID" +
               //" u.ConnectionProfile, u.Password "+	//	4,5
               " FROM AD_User u" +
               " INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive='Y')" +
               " INNER JOIN AD_Role r ON (ur.AD_Role_ID=r.AD_Role_ID AND r.IsActive='Y') " +
                //.Append("WHERE COALESCE(u.LDAPUser,u.Name)=@username")		//	#1
               " WHERE " +//(COALESCE(u.LDAPUser,u.Name)=@username OR COALESCE(u.LDAPUser,u.Value)=@username)"+
               " u.AD_User_ID=" + VIS.context.getAD_User_ID() + " AND u.IsActive='Y' " +
               " AND u.IsLoginUser='Y' " +
               " AND EXISTS (SELECT * FROM AD_Client c WHERE u.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')" +
               " AND EXISTS (SELECT * FROM AD_Client c WHERE r.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')" +
               " ORDER BY r.Name";


            $.ajax({
                url: VIS.Application.contextUrl + "UserPreference/GetLoginData",
                dataType: "json",
                data: { sql: sql },
                success: function (data) {
                    var dic = JSON.parse(data);
                    var cmbRoleContent = "";
                    for (var itm in dic) {
                        cmbRoleContent += "<option value=" + dic[itm].RecKey + ">" + dic[itm].Name + "</option>";
                    }
                    $cmdRole.append(cmbRoleContent);
                    $cmdRole.val(defaultLogin.AD_Role_ID);
                    cmbRoleContent = null;
                    dic = null;
                    loadClient();
                }
            });

        };
        var loadClient = function () {
            var roleID = $cmdRole.val();
            $cmdClient.empty();
            $cmdOrg.empty();
            $cmdWareHouse.empty();
            if (roleID == undefined) {
                return;

            }
            var sql = "SELECT c.Name,r.AD_Client_ID FROM AD_Role r INNER JOIN AD_Client c ON (c.AD_Client_ID=r.AD_Client_ID) WHERE r.AD_Role_ID=" + roleID;
            roleID = null;

            $.ajax({
                url: VIS.Application.contextUrl + "UserPreference/GetLoginData",
                dataType: "json",
                data: { sql: sql },
                success: function (data) {
                    var dic = JSON.parse(data);
                    var cmbClientContent = "";
                    for (var itm in dic) {
                        cmbClientContent += "<option value=" + dic[itm].RecKey + ">" + dic[itm].Name + "</option>";
                    }
                    $cmdClient.append(cmbClientContent);
                    $cmdClient.val(defaultLogin.AD_Client_ID);
                    cmbClientContent = null;
                    dic = null;
                    sql = null;
                    loadOrg();
                }
            });


        };
        var loadOrg = function () {
            var AD_Role_ID = $cmdRole.val();
            var AD_Client_ID = $cmdClient.val();
            $cmdOrg.empty();
            $cmdWareHouse.empty();
            if (AD_Role_ID == undefined || AD_Client_ID == undefined) {
                return;

            }
            var sql = "SELECT o.Name,o.AD_Org_ID "	//	1..3
                + "FROM AD_Role r, AD_Client c"
                + " INNER JOIN AD_Org o ON (c.AD_Client_ID=o.AD_Client_ID OR o.AD_Org_ID=0) "
                + "WHERE r.AD_Role_ID='" + AD_Role_ID + "'" 	//	#1
                + " AND c.AD_Client_ID='" + AD_Client_ID + "'"	//	#2
                + " AND o.IsActive='Y' AND o.IsSummary='N'  AND o.IsCostCenter='N' AND o.IsProfitCenter='N' "
                + " AND (r.IsAccessAllOrgs='Y' "
                    + "OR (r.IsUseUserOrgAccess='N' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_Role_OrgAccess ra "
                        + "WHERE ra.AD_Role_ID=r.AD_Role_ID AND ra.IsActive='Y')) "
                    + "OR (r.IsUseUserOrgAccess='Y' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_User_OrgAccess ua "
                        + "WHERE ua.AD_User_ID='" + VIS.context.getAD_User_ID() + "' AND ua.IsActive='Y'))"		//	#3
                    + ") "
                + "ORDER BY o.Name";

            AD_Role_ID = null;
            AD_Client_ID = null;
            $.ajax({
                url: VIS.Application.contextUrl + "UserPreference/GetLoginData",
                dataType: "json",
                data: { sql: sql },
                success: function (data) {
                    var dic = JSON.parse(data);
                    var cmbOrgContent = "";
                    for (var itm in dic) {
                        cmbOrgContent += "<option value=" + dic[itm].RecKey + ">" + dic[itm].Name + "</option>";
                    }
                    $cmdOrg.append(cmbOrgContent);
                    $cmdOrg.val(defaultLogin.AD_Org_ID);
                    cmbOrgContent = null;
                    dic = null;
                    sql = null;
                    loadWH();
                }
            });


        };
        var loadWH = function () {
            var AD_Org_ID = $cmdOrg.val();

            $cmdWareHouse.empty();
            if (AD_Org_ID == undefined) {
                return;

            }
            var sql = "SELECT Name,M_Warehouse_ID  FROM M_Warehouse "
                + "WHERE AD_Org_ID=" + AD_Org_ID + " AND IsActive='Y' "
                + "ORDER BY Name";

            AD_Org_ID = null;

            $.ajax({
                url: VIS.Application.contextUrl + "UserPreference/GetLoginData",
                dataType: "json",
                data: { sql: sql },
                success: function (data) {
                    var dic = JSON.parse(data);
                    var cmbWHContent = "";
                    for (var itm in dic) {
                        cmbWHContent += "<option value=" + dic[itm].RecKey + ">" + dic[itm].Name + "</option>";
                    }
                    $cmdWareHouse.append(cmbWHContent);
                    $cmdWareHouse.val(defaultLogin.M_Warehouse_ID);
                    cmbWHContent = null;
                    dic = null;
                    sql = null;
                }
            });


        };

        this.showDialog = function () {
            var w = $(window).width() - 150;
            var h = $(window).height() - 40;
            $busyDiv.height(h);
            $busyDiv.width(w);
            $root.append($busyDiv);
            $root.dialog({
                modal: false,
                title: VIS.Msg.getMsg("Preference"),
                width: w,
                height: h,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();

                    $root.dialog("destroy");

                    $("#ui-datepicker-div").remove()
                    $root = null;
                    $self = null;
                }
            });
        };

        this.show = function () {
            this.load();
            this.showDialog();
        }

        this.disposeComponent = function () {
            $self = null;
            if (drpTheme) {
                drpTheme.off('click');
            }

            if ($Okbtn)
                $Okbtn.off("click");
            if ($cancelbtn)
                $cancelbtn.off("click");
            if ($reset)
                $reset.off("click");
            //if ($sentmail)
            //    $sentmail.off("click");
            if ($savetofile)
                $savetofile.off("click");

            if ($btnDownLog)
                $btnDownLog.off("click");

            $autologin = null;
            $transtab = null;
            $acctab = null;
            $autonewrecord = null;
            $advanstab = null;
            $tracefile = null;
            $tracecmb = null;
            // $printer = null;
            // $printpreview = null
            _arrOfarr = null;
            $dictionary = null;
            $show = null;
            $showmingrid = null;
            $pagesize = null;
            $printnative = null;
            $windowpagesize = null;
            $variablename = null
            $variablevalue = null;
            $transtab = null;
            $erroronly = null;
            $reset = null;
            // $sentmail = null;
            $savetofile = null;
            $error = null;
            $role = null;
            $Okbtn = null;
            $cancelbtn = null;

            $btnChangePws = null;
            $currentPws = null;
            $newPws = null;
            $conformPws = null;
            $chkEmail = null;
            $chkNotice = null;
            $chkSMS = null;
            $chkFax = null;
            $txtUserName = null;
            $txtPws = null;
            $btnEmailConfig = null;
            $btnSubstitute = null;
            $btnSaveChanges = null;

            lblUserSetMenu = null;
            lblPreferenceMenu = null;
            lblContextMenu = null;
            lblErrorMenu = null;
            lblchangePwsHeader = null;
            lblAutoMatic = null;
            imgAutoMatic = null;
            lblTranslation = null;
            imgTranslation = null;
            lblShowAccounting = null;
            imgShowAccounting = null;
            lblAutoNewRecord = null;
            imgAutoNewRecord = null;
            lblShowAdvanced = null;
            imgShowAdvanced = null;
            lblCacheWindows = null;
            imgCacheWindows = null;
            vlblClientOrg = null;
            imgClientOrg = null;
            lblWindowPageSize = null;
            imgWindowPageSize = null;
            lblAutoLogin = null;
            imgAutoLogin = null;
            lblStorePassword = null;
            imgStorePassword = null;
            lblTraceLevel = null;
            imgTraceLevel = null;
            lblTraceFile = null;
            imgTraceFile = null;
            lblNativeDigit = null;
            imgNativeDigit = null;
            vlblDateText = null;
            imgDateText = null;
            vlblPageSize = null;
            imgPageSize = null;
            vlblPageSize = null;
            lblContext = null;
            $prefdate.datepicker("destroy");
            $prefdate = null;

            $cmdRole.off("change");
            $cmdClient.off("change");
            $cmdOrg.off("change");
            $cmdRole = null;
            $cmdClient = null;
            $cmdOrg = null;
            $cmdWareHouse = null;
            loadRoles = null;
            loadOrg = null;
            loadClient = null;
            loadWH = null;



            this.disposeComponent = null;
        };

    };

    //dispose call
    UserPreference.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;

    };

    VIS.Framework.UserPreference = UserPreference;

})(VIS, jQuery);