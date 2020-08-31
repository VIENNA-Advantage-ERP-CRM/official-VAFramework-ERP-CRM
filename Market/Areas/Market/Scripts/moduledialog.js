; (function (VIS, $) {

    function ModuleDialog(mInfo) {

        if (!mInfo.AD_Module_ID)
            mInfo.AD_Module_ID = mInfo.ModuleID;


        /* variable */
        this.tokenKey, this.isCloudMarket;
        var self = this, ch = null; sblog = '';


        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSet";

        var root = null;//.html("<h1>need to be coded before loaded :) </h1>") ;

        var btnInstall, btnCancel, lstClients, lstLang, lstLogs, busy, progress, progressIn;

        var executeDataSet = function (sql, param, callback) {
            var async = callback ? true : false;
            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }
            var dataSet = null;
            getDataSetJString(dataIn, async, function (jString) {
                dataSet = new VIS.DB.DataSet().toJson(jString);
                if (callback) {
                    callback(dataSet);
                }
            });
            return dataSet;
        };

        function getDataSetJString(data, async, callback) {
            var result = null;
            // data.sql = VIS.secureEngine.encrypt(data.sql);
            $.ajax({
                url: dataSetUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: async,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                if (callback) {
                    callback(json);
                }
                //return result;
            });
            return result;
        };

        function init() {

            var imgSrc = VIS.Application.contextUrl + 'Areas/Market/Images/big_app_image.png';
            if (mInfo.Image)
                imgSrc = 'data:image/png;base64,' + mInfo.Image;



            root = $('<div class="market-popup" >'

	+ '<div class="market-popup-content">'
    	+ '<div style="position:relative">'
          + '  <div class="market-about-app vis-pull-left">'
            + '    <img src=' + imgSrc + ' style="max-width:120px">'
              + '  <div class="market-app-detail-inner">'
                + '    <p class="market-title">' + mInfo.Name + '</p>'
                + '  <!--<img src="img/progressbar.jpg">-->'
                    + '<div class="market-popup-buttons">'
               + ' 	<a href="javascript:void(0)" class="market-popup-btn market-install vis-pull-left">' + VIS.Msg.getMsg("Install") + '</a>'
                 + '   <a href="javascript:vois(0)" class="market-popup-btn market-cancel vis-pull-left">' + VIS.Msg.getMsg("Cancel") + '</a>'
                + '</div>'
                + '</div><!-- end of app_detail_inner -->'
            + '</div><!-- end of popup-content-top -->'

            + '<div class="market-popup-viennaLogo vis-pull-right">'
              + '  <img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/logo.jpg">'
            + '</div><!-- end of popup-viennaLogo -->'
        + '</div><!-- end of app_detail -->'

        + '<div class="market-app-description" style="float:left;padding-top:25px;">'
        	+ '<div class="market-popup-description-inner">'
            + '	<div class="market-popup-install-text">'
              + '      <p>' + VIS.Msg.getMsg("MigrationText") + ';</p>'
      + '          </div><!-- end of popup-install-text -->'

        + '        <div class="market-popup-client">'
          + '      	<h6>' + VIS.Msg.getMsg("SelectClient") + '</h6>'
            + '        <div class="market-popup-client-options market-clients">'

  + '                  </div><!-- end of popup-client-options -->'
    + '            </div><!-- end of popup-client -->'

      + '          <div class="market-popup-client" style="margin-right: 0; float: right;">'
        + '        	<h6>' + VIS.Msg.getMsg("SelectLanguage") + '</h6>'
          + '          <div class="market-popup-client-options market-lang">'

                  + '  </div><!-- end of popup-client-options -->'
               + ' </div><!-- end of popup-client -->'

            + '</div><!-- end of popup-description-inner -->'
            + '<div class="market-pop-divider"></div>'

           + ' <div class="market-popup-description-inner" style="margin-top: 0;">'
            //+ '	<div class="popup-buttons" style="margin-top: 0;">'
            //  + '  	<a href="#" class="popup-btn">Install</a>'
            //    + '    <a href="#" class="popup-btn">Cancel</a>'
            //   + ' </div>'
            + '<progress class="market-progress-in"></progress>'
            + '<progress max="100" value="0" class="market-progress"></progress>'
              + '  <div class="market-popup-logs">'
                + '</div><!-- end of popup-client-options -->'

           + ' </div><!-- end of popup-description-inner -->'
       + ' </div><!-- end of popup-description -->'

+ '	</div><!-- end of popup-content -->'
+ '	<div class="vis-apanel-busy" style="height:90%"></div><!-- end of popup-content -->'
+ '  </div><!-- end of market-popup --> ');



            btnInstall = root.find(".market-install");
            btnCancel = root.find(".market-cancel");
            lstClients = root.find(".market-clients");
            lstLang = root.find(".market-lang");
            lstLogs = root.find(".market-popup-logs");
            busy = root.find(".vis-apanel-busy");
            progress = root.find(".market-progress");
            progressIn = root.find(".market-progress-in").hide();
            fillData();

            bindEvents();
        };

        init();

        function setIsBusy(isbusy) {

            if (isbusy)
                busy.show();
            else busy.hide();
        };

        function setProgreesBar(isbusy) {

            if (isbusy) {
                progress.show();
                btnCancel.hide();
                btnInstall.hide();
            }
            else {
                progress.hide();
                btnCancel.show();
                btnInstall.show();
            }
        };


        function fillData() {

            lstClients.empty();
            lstLang.empty();

            var sb = "SELECT Name as Name,AD_Client_ID as ID FROM AD_Client WHERE ";

            if (mInfo.AccessLevel == "4") //System only
            {
                sb += " AD_Client_ID = 0  ";
            }
            else if (mInfo.AccessLevel == "6") //System+Client
            {
                sb += " AD_Client_ID >= 0 ORDER BY AD_Client_ID  ";
            }
            else {
                sb += " AD_Client_ID > 0 "; //Client Only
            }

            if (mInfo.Translations && mInfo.Translations.length > 0) {
                sb += "~ SELECT Name as Name,AD_Language as ID FROM AD_Language WHERE AD_Language IN (" + mInfo.Translations + ")";
            }

            var ds = executeDataSet(sb);
            var dt = null, strHtml = "";
            if (ds != null && ds.getTables().length > 0) {
                dt = ds.getTables()[0];
                for (var i = 0; i < dt.getRows().length; i++) {
                    strHtml += '<div class="market-client-data vis-pull-left"><input type="checkbox" data-val=' + dt.getRows()[i].getCell("ID") + ' >'
                                + '<label>' + dt.getRows()[i].getCell("Name") + '</label></div>';
                }
                lstClients.html(strHtml);

                if (ds.getTables().length > 1) {
                    dt = ds.getTables()[1];
                    strHtml = "";

                    for (var i = 0; i < dt.getRows().length; i++) {
                        strHtml += '<div class="market-client-data vis-pull-left"><input type="checkbox" data-val=' + dt.getRows()[i].getCell("ID") + ' >'
                                    + '<label>' + dt.getRows()[i].getCell("Name") + '</label></div>';
                    }
                    lstLang.html(strHtml);
                }
                dt = null;
                ds.dispose();
            }
            setIsBusy(false);
        };


       


        function bindEvents() {
            btnInstall.on("click", function () { //Install click
                //check clients

                var cIds = [], langIds = [];
                lstClients.find('input:checked').each(function () {
                    cIds.push($(this).data('val'));
                });
                if (cIds.length < 1) {
                    VIS.ADialog.error("SelectClient", true);
                    return;
                }

                setProgreesBar(true); //show Progress bar
                lstLogs.empty(); //clear log

                //language
                lstLang.find('input:checked').each(function () {
                    langIds.push('"' + $(this).data('val') + '"');
                });

                addLog("[step 1] Initializing Channel");

                //setProgressValue("2", "N"); //show progress bar
                /* init Request */
                sendRequest(VIS.Application.contextUrl + 'Market/Module/DownloadAndExtractModuleFiles', { 'AD_ModuleInfo_ID': mInfo.AD_Module_ID }, function (e) {

                    if (e.startsWith("Error=>")) {
                        setMeassageAndEnableUI(e); //show error messgae
                    }
                    else {

                        var index = e.indexOf("$ModulePath$");

                        if (index > -1) {
                            var module = e.substring(index);

                            addLog(e.substring(0, index) + "<br/>" + "[step 2] Importing module...");

                            var moduleName = module.replace("$ModulePath$", "");
                            setProgressValue("11", "N");
                            window.setTimeout(
                            startMigration(cIds, langIds, moduleName), 100);
                        }
                        else {
                            setMeassageAndEnableUI("Module Path Not Found");
                            setProgreesBar(false);
                        }
                    }
                });
                addLog("Downloading and extracting files");
                addLog("Please wait...");
                setProgressValue("1", "Y");

            });

            btnCancel.on("click", function () {
                ch.close();
            });

        };

        var timeout=null;
        function startMigration(iIds, langCode, moduleName) {
            sendRequest(VIS.Application.contextUrl + "Market/Module/ImportModuleFromLocalFile", { "moduleName": moduleName, "clientIds": iIds, "langCode": langCode, "onlyTranslation": false, "ctxDic": VIS.context.getWindowCtx() }, function (cbd) {

                if (cbd && cbd.Action == "Error") {

                    VIS.ADialog.error("Error?", true, cbd.ActionMsg);

                    setProgressValue("0", "N");
                    setProgreesBar(false);
                    clearTimeout(timeout);
                    return;
                }
                if (cbd && cbd.Action == "Redirect") {

                    var idVersion = cbd.Status;
                    var Path = cbd.ActionMsg;

                    setProgressValue("99", "N");

                    var path = VIS.Application.contextUrl + 'Market/Module/ModuleLog';

                    path += "?id=" + idVersion;

                    if (Path && Path.length > 0) {
                        path += "&path=" + Path;
                        if (self.isCloudMarket || VIS.Application.isSSL) {
                            path += "$Y$";
                        }
                    }

                    path += "&value=" + self.tokenKey;

                    sendRequest(VIS.Application.contextUrl + 'Market/Module/UpdateSubscrption',
                            { 'AD_Module_ID': mInfo.AD_Module_ID, 'Product_ID': mInfo.Product_ID, 'token': self.tokenKey, 'isFree': true, 'version': mInfo.VersionNo },
                            function () {

                            });

                    setProgressValue("100", "N");
                    ch.close();
                    setTimeout(function () {
                        window.location = path
                    });
                    return;
                }

            });

            timeout = window.setTimeout(pullMessage, 5000);
            //client.ImportModuleFromMarketAsync(AD_ModuleInfo_ID, iIds, langCode, false, (Dictionary<string, string>)_ctx.GetMap());
        };

        function pullMessage() {

            sendRequest(VIS.Application.contextUrl + "Market/Module/PullMessage", {}, function (json) {

                var j = JSON.parse(json.result);
                if (json.status == "Finish") {

                    // alert("finish");
                    // return;

                }
                else if (j) {
                    var cbd = null;
                    for (var i = 0; i < j.length; i++) {
                        cbd = j[i];
                        if (cbd.Action == "Error") {

                            VIS.ADialog.error("Error?", true, cbd.ActionMsg);

                            setProgressValue("0", "N");
                            setProgreesBar(false);
                            clearTimeout(timeout);
                            return;
                        }

                        if (cbd.Action == "Redirect") {

                            var idVersion = cbd.Status;
                            var Path = cbd.ActionMsg;

                            setProgressValue("99", "N");

                            var path = VIS.Application.contextUrl + 'Market/Module/ModuleLog';

                            path += "?id=" + idVersion;

                            if (Path && Path.length > 0) {
                                path += "&path=" + Path;
                                if (self.isCloudMarket || VIS.Application.isSSL) {
                                    path += "$Y$";
                                }
                            }

                            path += "&value=" + self.tokenKey;

                            sendRequest(VIS.Application.contextUrl + 'Market/Module/UpdateSubscrption',
                                    { 'AD_Module_ID': mInfo.AD_Module_ID, 'Product_ID': mInfo.Product_ID, 'token': self.tokenKey, 'isFree': true, 'version': mInfo.VersionNo },
                                    function () {

                                    });

                            setProgressValue("100", "N");
                            ch.close();
                            setTimeout(function () {
                                window.location = path
                            });
                            return;
                        }
                        if (cbd.Action == "Done") {
                            setProgressValue("0", "N");
                            setProgreesBar(false);
                            return;
                        }
                        //Progressbar

                        if (cbd.Action == "PValue") {
                            setProgressValue(cbd.Status, cbd.ActionMsg);
                        }
                        else {
                            //sbLog.Append(cq.cbd.Status).Append("\n");
                            addLog(cbd.Status);
                            //if (lstMessage.Items.Count > 0)
                            //{
                            //    lstMessage.SelectedIndex = lstMessage.Items.Count - 1;
                            //    lstMessage.ScrollIntoView(lstMessage.SelectedItem);
                            //    lstMessage.UpdateLayout();

                            //}
                        }
                    }

                    window.setTimeout(pullMessage, 5000, true);
                }
            }, true);
        };

        function sendRequest(url, data, callback, sync) {

            var async = true;
            if (sync)
                async = false;

            $.ajax({
                url: url,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: async,
                data: JSON.stringify(data)
            }).done(function (json) {
                if (callback)
                    callback(json);
            })
            .fail(function (xhr, e) {
                if (callback)
                    callback("Error=>" + e);
            });
        };

        function setMeassageAndEnableUI(msg) {
            addLog(msg);
            setProgressValue("0", "N");
            setIsBusy(false);
        };

        function setProgressValue(value, show) {
            if (!value || value == "")
                return;
            var val = value;
            progress.val(value);
            // progress.text(value+'%');
            //pBar.Value = val;
            //pText.Text = (val/100.0).ToString("P0");
            if (show == "Y") {
                progressIn.show();
            }
            else progressIn.hide();
            //ShowHidePBar2(show);
        };

        function addLog(msg) {
            //sblog += '<br />' + msg;
            var p = $('<p class="market-log">' + msg + '</p>');
            lstLogs.append(p);
            p[0].scrollIntoView();
            p = null;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            //ch.setHeight(800);
            //ch.setWidth(1600);
            ch.setTitle(VIS.Msg.getMsg("Market_ModuleDialogHeader"));
            ch.setModal(true);
            ch.show();
            ch.setContent(root);
            ch.hideButtons();
            ch.onClose = function () {
                self.dispose();
            }
            ch.getRoot().dialog({ position: { at: "center center", of: window } });
        };

        this.dispose = function () {
            root.remove();

            this.show = null;
            ch = null;
            self = null;
        };
    };

    Market.ModuleDialog = ModuleDialog;

})(VIS, jQuery);