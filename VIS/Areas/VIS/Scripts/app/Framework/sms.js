; (function (VIS, $) {

    function Sms(_curtab, _curGC, Record_ID, isWindowForm) {

        //local variables
        this.frame = null;
        this.windowNo = VIS.Env.getWindowNo();
        var ctx = VIS.Env.getCtx();
        var ch = null;
        var callingFromOutsideofWindow = false;
        var bpColumnName = "C_BPARTNER_ID";
        var rowsSource = null;
        var rowsSingleView = null;
        var currentTable_ID = 0;
        var csvData = [];
        var self = this;
        var formHeight = 0;
        var formWidth = 0;

        // Design variables        //contains Field names of tab on left side
        var $ulFieldNames = null;
        var $root = $('<div class="vis-sms-rootDiv vis-forms-container"></div>');
        var $toolbarDiv = $('<div class="vis-awindow-header vis-menuTitle">');
        var $leftDiv = null;
        var $middleDiv = null;
        var $rightDiv = null;
        var $imgAction = null;
        var $txtArea = null;
        //contain list of bcc Mobile Numbers
        var $bccChkList = null;
        var $mobileContainer = null;
        var $ulFieldNames = null;
        var $actualtextPara;
        var $operator;
        var $maxtextPara;
        var $btnHdrSend;
        var $btnHdrPreview;
        var isViewLoaded = false;
        var $bsyDiv;
        var $refresh = null;
        var $dynamicDisplay = null;
        initEmail();

        function initEmail() {
            callingFromOutsideofWindow = isWindowForm;
            if (!callingFromOutsideofWindow) {
                currentTable_ID = _curtab.gridTable.AD_Table_ID;
                // this is data source for multiView Records
                rowsSource = _curGC.getSelectedRows();

                // this is data source for single View Records
                rowsSingleView = _curtab.getRecords()[_curtab.getCurrentRow()];
            }

            $bccChkList = $('<ul class="vis-sms-ul" ></ul>');

            $bsyDiv = $("<div>");
            $bsyDiv.css("position", "absolute");
            $bsyDiv.css("bottom", "0");
            $bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/LoadingCircle.gif') no-repeat");
            $bsyDiv.css("background-position", "center center");
            $bsyDiv.css("width", "100%");
            $bsyDiv.css("height", "100%");
            $bsyDiv.css('text-align', 'center');
            $bsyDiv.css('opacity', '.1');
            $bsyDiv[0].style.visibility = "hidden";
        };

        this.initializeComponent = function () {
            var $btncloseChart = $('<a href="javascript:void(0)"  class="vis-icon-menuclose"><i class="vis vis-cross"></i></a>');
            var pheader = '';
            if (callingFromOutsideofWindow) {
                pheader = $('<p>' + VIS.Msg.getMsg("Sms") + ' (' + VIS.Msg.getMsg("Contacts") + ')' + ' </p>');
            }
            else {
                pheader = $('<p>' + VIS.Msg.getMsg("Sms") + ' (' + _curGC.aPanel.$parentWindow.getName() + ')' + ' </p>');
            }


            $root.append($toolbarDiv.append($btncloseChart).append(pheader));
            $root.append($bsyDiv);
            $btncloseChart.click(function (e) {
                self.dispose();
                self = null;
                e.stopPropagation();
            });

            loadDesign();
            Addbuttons();
            if (!callingFromOutsideofWindow) {
                loadFields();
                loadMobileNumbers(false);
            }
            eventhandlers();
        };

        function loadDesign() {

            var html = '         <div class="contentArea" style="width: 100%; float: left;">';
            html += ' <div class="vis-sms-leftDiv"></div>';
            html += '   <div class="vis-sms-ContentWrap"><div class="vis-sms-ContentArea"><div class="vis-sms-leftWrap"><div class="vacom-form-horizontal">';
            html += '   <div class="vis-sms-inputWrap"><div class="vis-sms-form-data-sub">';
            html += '         <input type="text"  id="' + self.windowNo + '_emailMoblie"  placeholder="' + VIS.Msg.getMsg("EnterMobileNumber") + '">';
            html += '</div>';
            html += '  </div>';
            html += '  </div><div class="vis-sms-textarea-div" style="width: 100%; float: left;">';
            html += ' <textarea  ondrop="return false;" id="' + self.windowNo + '_vis-sms-textarea" style="width: 100%; height:100%;resize:none" placeholder="' + VIS.Msg.getMsg("WriteMsgHere") + '"></textarea></div>';
            html += ' <div class="vis-sms-leftFooter">';
            html += '<div ><p class="vis-sms-counter" id=' + self.windowNo + '_acCount></p> <p  class="vis-sms-counter"   id=' + self.windowNo + '_oper></p> <p  class="vis-sms-counter"   id=' + self.windowNo + '_maxCount >160</p> </div>';
            html += ' <input id="' + self.windowNo + '_dyndis" type="checkbox"><label  id="' + self.windowNo + '_dyndislbl"  for="' + self.windowNo + '_dyndis" >' + VIS.Msg.getMsg("DynamicSms") + '</label></div></div>';
            html += '  <div class="vis-sms-divider"></div>';
            html += ' <div class="vis-sms-rytWrap"><div class="vis-sms-rytContent">';
            html += ' <div class="vis-sms-rytBcc"><div class="vis-email-rytBcc-header"><h4>' + VIS.Msg.getMsg("Mobile") + '</h4>';
            html += ' <i id="' + self.windowNo + '_SmsRefresh" title="' + VIS.Msg.getMsg("Refresh") + '" class="vis vis-refresh"></i>';
            html += '   </div><div class="vis-sms-BccList">';
            html += ' </div></div></div></div>  </div></div>';

            $root.append($(html));
            var middleDivWidth = 0;
            $dynamicDisplay = $root.find('#' + self.windowNo + "_dyndis");

            $root.find('.contentArea').css('height', $root.height() - 42);
            $root.find('.vis-sms-leftDiv').height($root.height() - 42);
            $leftDiv = $root.find('.vis-sms-leftDiv');
            $rightDiv = $root.find('.vis-sms-BccList');
            $mobileContainer = $root.find('#' + self.windowNo + "_emailMoblie");
            $refresh = $root.find('#' + self.windowNo + '_SmsRefresh');

            //if (to != undefined && to != null && to.trim().length > 0)
            //{
            //    $mobileContainer.val(to);
            //}

            $mobileContainer.css('margin-top', '10px');
            $txtArea = $root.find('#' + self.windowNo + '_vis-sms-textarea');
            $leftfootArea = $root.find(".vis-sms-leftFooter");
            $actualtextPara = $root.find('#' + self.windowNo + "_acCount");
            $operator = $root.find('#' + self.windowNo + "_oper");
            $maxtextPara = $root.find('#' + self.windowNo + "_maxCount");
            $root.find(".vis-sms-textarea-div").css('margin-top', '10px');
            $root.find('.vis-sms-rytBcc').height($root.find('.vis-sms-rytContent').height() - 10);
            if (callingFromOutsideofWindow) {
                $root.find('.vis-sms-leftDiv').hide();
                $root.find('.vis-sms-rytWrap').hide();
                $root.find(".vis-sms-textarea-div").height($root.height() - 150);
                $root.find('.vis-sms-leftWrap').css("width", '100%');
                $root.find('#' + self.windowNo + '_dyndislbl').hide();
                middleDivWidth = $root.width() - 40;
                $dynamicDisplay.prop('checked', false);
                $dynamicDisplay.hide();
            }
            else {
                $root.find(".vis-sms-textarea-div").height($root.height() - 150);
                middleDivWidth = $root.width() - 240;
                $root.find('.vis-sms-leftWrap').width(middleDivWidth - 345);
            }

            $middleDiv = $root.find('.vis-sms-ContentWrap');
            $middleDiv.css('width', middleDivWidth);
            isViewLoaded = true;


            $root.find('.vis-sms-BccList').height($root.find('.vis-sms-rytContent').height() - (70));

        };

        function loadFields() {

            $imgAction = $('<i class="fa fa-bars"></i>');
            var $toggleAction = $('<div class="vis-apanel-lb-toggle" style="overflow-x:hidden;overflow-y:hidden"></div>');
            $toggleAction.append($imgAction);
            $leftDiv.append($toggleAction);

            $ulFieldNames = $('<ul class="vis-apanel-lb-ul"></ul>');
            $ulFieldNames.css({ "height": "calc(100% - 54px)", "overflow": "auto", "white-space": "nowrap", "width": "100%" });


            for (var i = 0; i < _curtab.gridTable.getFields().length; i++) {
                var iskeyColumn = (_curtab.gridTable.getFields(0)[i].getColumnName().toUpper() == _curtab.getKeyColumnName().toUpper());// in case of custmer Master , C_BPartner is not displayed but it is required to show its user's mobile which have isSms=Y
                if (!_curtab.gridTable.getFields(0)[i].getIsDisplayed() && !iskeyColumn) {
                    continue;
                }
                if (!iskeyColumn) {
                    $ulFieldNames.append($('<li>' + _curtab.gridTable.getFields(0)[i].getHeader() + '</li>'));
                }


                var bpList = [];
                var pkList = [];

                if (_curtab.gridTable.getFields(0)[i].getColumnName().toUpper().equals(bpColumnName)) {
                    for (var q = 0; q < rowsSource.length; q++) {
                        if (VIS.DisplayType.IsLookup(_curtab.getField(bpColumnName).getDisplayType())) {
                            if (rowsSource[q][bpColumnName.toLower()] != null) {
                                bpList.push(rowsSource[q][bpColumnName.toLower()]);
                                pkList.push(rowsSource[q][_curtab.getKeyColumnName().toLower()]);
                            }
                        }
                        else if (rowsSource[q][bpColumnName.toLower()] != null) {
                            bpList.push(rowsSource[q][bpColumnName.toLower()]);
                            pkList.push(rowsSource[q][_curtab.getKeyColumnName().toLower()]);
                        }
                    }
                }
                fillIDsOFUsers(bpList, pkList);
            }
            $leftDiv.append($ulFieldNames);
        };

        function loadMobileNumbers(refrsh) {
            if (refrsh) {
                $bccChkList.empty();
                // this is data source for multiView Records
                rowsSource = _curGC.getSelectedRows();

                // this is data source for single View Records
                rowsSingleView = _curtab.getRecords()[_curtab.getCurrentRow()];
            }

            if (_curGC.singleRow)//show records for single window
            {
                loadMobileNumberForSingleView();
            }
            else {
                if (rowsSource.length > 0) {
                    for (var i = 0; i < rowsSource.length; i++) {

                        //else {
                        if (rowsSource[i]["mobile"] != null) {
                            var ID = rowsSource[i][_curtab.getKeyColumnName().toLower()];
                            $bccChkList.append('<li class="vis-sms-list-li-bcc"  data-mobile="' + rowsSource[i]["mobile"] + '"><input id="' + self.windowNo + '_' + rowsSource[i]["mobile"] + '_CheckBoxList1" type="checkbox" value="' + rowsSource[i]["mobile"] + '" checked/><label class="vis-sms-chcklist-label"  for="' + self.windowNo + '_' + rowsSource[i]["mobile"] + '_CheckBoxList1">' + rowsSource[i]["mobile"] + '(' + ID + ')</label></li>');
                        }
                        //}
                    }
                }
                else {
                    loadMobileNumberForSingleView();
                }
            }
            if (!refrsh) {
                $rightDiv.append($bccChkList);
            }

            if ($bccChkList.children().length > 1) {
                if ($dynamicDisplay.prop("checked") == true) {
                    $mobileContainer.val('');
                    $mobileContainer.prop('disabled', true);
                }
            }
        };

        function loadMobileNumberForSingleView() {
            if (rowsSingleView["mobile"] != null) {
                var ID = rowsSingleView[_curtab.getKeyColumnName().toLower()];
                //  vchkListBoxTop.AddKeyAsItem = new KeyValuePair<string, int>(((VAdvantage.DataUtil.DataObject)rowsSource[i]).GetFieldValue("EMAIL").ToString() + "(" + ID + ")", ID);

                $bccChkList.append('<li class="vis-sms-list-li-bcc" data-mobile="' + rowsSingleView["mobile"] + '"><input id="' + self.windowNo + '_' + rowsSingleView["mobile"] + '_CheckBoxList1" type="checkbox" value="' + rowsSingleView["mobile"]
                       + '" checked /><label class="vis-sms-chcklist-label"  for="' + self.windowNo + '_' + rowsSingleView["mobile"] + '_CheckBoxList1">' + rowsSingleView["mobile"] + '(' + ID + ')</label></li>');
            }
        };

        function Addbuttons() {

            $btnHdrSend = $('<i class="vis-email-sendbtn vis vis-paper-plane vis-Sms-sendButton" title="' + VIS.Msg.getMsg("SendSms").replace('&', '') + '" ></i>');
            $toolbarDiv.append($btnHdrSend);
            if (!callingFromOutsideofWindow) {
                $btnHdrPreview = $('<i class="vis-email-btns vis vis-viewdocument vis-sms-preview-btn" title="' + VIS.Msg.getMsg("Preview").replace('&', '') + '"></i>');
                $toolbarDiv.append($btnHdrPreview);
            }

        }

        function fillIDsOFUsers(bpID, prID) {

            var pvID = 0;
            for (var i = 0; i < bpID.length; i++) {
                //var sql = "Select AD_User_ID,issms,mobile from ad_user where c_bpartner_ID=" + bpID[i];
                //var ds = VIS.DB.executeDataSet(sql);
                //var isBroken = false;
                //if (ds != null && ds.getTables()[0].getRows().length > 0) {
                //    for (var j = 0; j < ds.getTables()[0].getRows().length; j++) {
                //        if (ds.getTables()[0].getRows()[j].getCell("ISSMS") != null) {
                //            if (ds.getTables()[0].getRows()[j].getCell("ISSMS").toString().equals("Y")) {
                //                if (ds.getTables()[0].getRows()[j].getCell("MOBILE") == null || ds.getTables()[0].getRows()[j].getCell("MOBILE") == "") {
                //                    continue;
                //                }

                //                if ($($bccChkList.children('li').data(ds.getTables()[0].getRows()[j].getCell("MOBILE") + prID[i])).length > 0) {
                //                    continue;
                //                }

                //                $bccChkList.append('<li  class="vis-sms-list-li-bcc"><input id="' + self.windowNo + '_' + ds.getTables()[0].getRows()[j].getCell("MOBILE") + '_CheckBoxList1" type="checkbox"  value="' + ds.getTables()[0].getRows()[j].getCell("MOBILE") + prID[i]
                //                    + '" checked/><label class="vis-sms-chcklist-label" for="' + self.windowNo + '_' + ds.getTables()[0].getRows()[j].getCell("MOBILE") + '_CheckBoxList1">' + ds.getTables()[0].getRows()[j].getCell("MOBILE") + '(' + prID[i] + ')</label></li>');
                //            }
                //        }
                //    }
                //}

                var ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Sms/GetUser", { "BPartner_ID": bpID[i] }, null);
                var isBroken = false;
                if (ds != null) {
                    for (var j in ds) {
                        if (ds[j]["ISSMS"] != null) {
                            if (ds[j]["ISSMS"].toString().equals("Y")) {
                                if (ds[j]["MOBILE"] == null || ds[j]["MOBILE"] == "") {
                                    continue;
                                }

                                if ($($bccChkList.children('li').data(ds[j]["MOBILE"] + prID[i])).length > 0) {
                                    continue;
                                }

                                $bccChkList.append('<li  class="vis-sms-list-li-bcc"><input id="' + self.windowNo + '_' + ds[j]["MOBILE"] + '_CheckBoxList1" type="checkbox"  value="' + ds[j]["MOBILE"] + prID[i]
                                    + '" checked/><label class="vis-sms-chcklist-label" for="' + self.windowNo + '_' + ds[j]["MOBILE"] + '_CheckBoxList1">' + ds[j]["MOBILE"] + '(' + prID[i] + ')</label></li>');
                            }
                        }
                    }
                }
            }
        };

        function eventhandlers() {
            $btnHdrSend.on("click", send);
            $txtArea.on("keyup", countText);


            if (!callingFromOutsideofWindow) {
                $imgAction.on("click", imgActionClick);
                $dynamicDisplay.on("click", dynamicDisplaychange);
                $ulFieldNames.children('li').on("click", insertSelectedField);
                $btnHdrPreview.on("click", preview);
                $imgAction.trigger('click');
                $refresh.on("click", refresh);

                //}
                //else {
                //$root.find('.vis-sms-CcBcc').on("click", showBccpanel);
            }
        };

        function imgActionClick(e) {
            if ($dynamicDisplay.prop("checked") == true) {
                if ($leftDiv.width() == 40) {
                    $leftDiv.css('width', '200px');
                    $leftDiv.find('ul').show();
                    $middleDiv.width($middleDiv.width() - 160);
                    $root.find('.vis-sms-leftWrap').width($root.find('.vis-sms-leftWrap').width() - 160);
                    // $('.vis-sms-form-data-sub').width($('.vis-sms-leftWrap').width());

                }
                else {
                    shrinkLeftDiv();
                }
            }
            else {
                if ($leftDiv.width() != 40) {
                    shrinkLeftDiv();
                }
            }

        };

        function shrinkLeftDiv() {
            $leftDiv.css({ 'width': '40px' });
            $leftDiv.find('ul').hide();
            $middleDiv.width($middleDiv.width() + 160);
            $root.find('.vis-sms-leftWrap').width($root.find('.vis-sms-leftWrap').width() + 160);
        };

        function dynamicDisplaychange(e) {

            if (this.checked) {
                if ($leftDiv.width() != 40) {
                    $leftDiv.find('ul').show();
                }
                //in ase of dynmic mail, to, bccc and cc will be enabled only for single record.
                //otherwise disable them.
                if (_curGC.singleRow == false && rowsSource.length > 1) {
                    $mobileContainer.val('');
                    $mobileContainer.prop('disabled', true);
                    //$cc.val('');
                    //$cc.prop('disabled', true);
                    //$bcc.val('');
                    //$bcc.prop('disabled', true);
                }
            }
            else {
                if ($leftDiv.width() != 40) {
                    shrinkLeftDiv();
                }
                $mobileContainer.prop('disabled', false);
                //$cc.prop('disabled', false);
                //$bcc.prop('disabled', false);
            }
        };

        function insertSelectedField(e) {
            //  $textAreakeno.paste('@@' + $(this).text() + '@@');
            var $textAreaLocal = $txtArea[0];
            var textToInsert = "@@" + $(this).text() + "@@";

            var scrollPos = $textAreaLocal.scrollTop;
            var strPos = 0;
            var br = (($textAreaLocal.selectionStart || $textAreaLocal.selectionStart == '0') ?
            "ff" : (document.selection ? "ie" : false));
            if (br == "ie") {
                $txtArea.focus();
                var range = document.selection.createRange();
                range.moveStart('character', -$txtArea.val().length);
                strPos = range.text.length;
            }
            else if (br == "ff") strPos = $textAreaLocal.selectionStart;

            var front = ($txtArea.val()).substring(0, strPos);
            var back = ($txtArea.val()).substring(strPos, $txtArea.val().length);
            $txtArea.val(front + textToInsert + back);
            strPos = strPos + textToInsert.length;
            if (br == "ie") {
                $txtArea.focus();
                var range = document.selection.createRange();
                range.moveStart('character', -$txtArea.val().length);
                range.moveStart('character', strPos);
                range.moveEnd('character', 0);
                range.select();
            }
            else if (br == "ff") {
                $textAreaLocal.selectionStart = strPos;
                $textAreaLocal.selectionEnd = strPos;
                $txtArea.focus();
            }
            $textAreaLocal.scrollTop = scrollPos;
            $textAreaLocal = null;
            countText();

        };

        function preview(e) {
            var html = $txtArea.val();
            var finalhtmls = '';
            if (_curGC.singleRow == false && rowsSource.length > 0) {
                finalhtmls = parseHtml1(html, 0);
            }
            else {
                finalhtmls = parseHtml2(html)
            }
            var $preDiv = $('<div></div>');
            $preDiv.append(VIS.Utility.encodeText(finalhtmls));
            var chp = new VIS.ChildDialog();
            chp.setHeight(420);
            chp.setWidth(850);
            chp.setTitle(VIS.Msg.getMsg("Preview"));
            chp.setModal(true);
            chp.setContent($preDiv);
            chp.show();
            chp.hidebuttons();
        };

        function refresh(e) {
            loadMobileNumbers(true);
        };

        function countText(e) {

            $actualtextPara.text($txtArea.val().length);
            if (parseInt($actualtextPara.text()) > parseInt($maxtextPara.text())) {
                $operator.text(">");
                //if text length grator then the set length turn text into red
                setColor("#ED0000");
            }
            else if (parseInt($actualtextPara.text()) == parseInt($maxtextPara.text())) {
                $operator.text("=");
                //else black
                setColor("#000000");
            }
            else {
                $operator.text("<");
                //else black
                setColor("#000000");
            }
        };

        function setColor(color) {
            $operator.css('color', color);//forecolor of text
            $actualtextPara.css('color', color);//label control color
            $maxtextPara.css('color', color);//fore color
        };

        function listofSelectedItems1(html, i) {

            var finalhtmlperRecord = html;
            var retValues = {};

            var copyhtml = html;
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                    if (rowsSource[i][columnName.toLower()] != null && rowsSource[i][columnName.toLower()] != undefined) {
                        //fieldValue = _curtab.getField(columnName.toLower()).lookup.lookup[" " + rowsSource[i][columnName.toLower()]].Name;
                        fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSource[i][columnName.toLower()]);
                    }
                }
                else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                    fieldValue = _curtab.getField(columnName).value;
                    fieldValue = new Date(fieldValue);
                }
                else {
                    fieldValue = rowsSource[i][columnName.toLower()];
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                retValues['@@' + fieldname + '@@'] = fieldValue;
            }

            return retValues;
        };

        function listofSelectedItems2(html) {

            var copyhtml = html;
            var retValues = {};
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                    if (rowsSingleView[columnName.toLower()] != null && rowsSingleView[columnName.toLower()] != undefined) {
                        //   fieldValue = _curtab.getField(columnName.toLower()).lookup.lookup[" " + rowsSingleView[columnName.toLower()]].Name;

                        fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSingleView[columnName.toLower()]);
                    }
                }
                else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                    fieldValue = _curtab.getField(columnName).value;
                    fieldValue = new Date(fieldValue);
                }
                else {
                    fieldValue = rowsSingleView[columnName.toLower()];
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                retValues['@@' + fieldname + '@@'] = fieldValue;
            }
            return retValues;
        };

        function parseHtml1(html, i) {
            var finalhtmlperRecord = html;
            var copyhtml = html;
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                    if (rowsSource[i][columnName.toLower()] != null && rowsSource[i][columnName.toLower()] != undefined) {
                        //fieldValue = _curtab.getField(columnName.toLower()).lookup.lookup[" " + rowsSource[i][columnName.toLower()]].Name;
                        fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSource[i][columnName.toLower()]);
                    }
                }
                else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                    fieldValue = _curtab.getField(columnName).value;
                    fieldValue = new Date(fieldValue);
                }
                else {
                    fieldValue = rowsSource[i][columnName.toLower()];
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                finalhtmlperRecord = finalhtmlperRecord.replace('@@' + fieldname + '@@', fieldValue);
            }

            return finalhtmlperRecord;
        };

        function parseHtml2(html) {

            var copyhtml = html;
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                    if (rowsSingleView[columnName.toLower()] != null && rowsSingleView[columnName.toLower()] != undefined) {
                        //   fieldValue = _curtab.getField(columnName.toLower()).lookup.lookup[" " + rowsSingleView[columnName.toLower()]].Name;

                        fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSingleView[columnName.toLower()]);
                    }
                }
                else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                    fieldValue = _curtab.getField(columnName).value;
                    fieldValue = new Date(fieldValue);
                }
                else {
                    if (_curGC.singleRow == true) {
                        fieldValue = _curtab.getField(columnName).value;
                    }
                    else {
                        fieldValue = rowsSingleView[columnName.toLower()];
                    }
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                html = html.replace('@@' + fieldname + '@@', fieldValue);
            }
            return html;


        };

        function send(e) {
            var body = $txtArea.val();
            var canExecute = true;
            if ((body == null || body == "" || body.trim().length == 0)) {
                canExecute = false;
                VIS.ADialog.confirm("SendWithoutBody", true, "", "Confirm", function (result) {
                    if (result) {
                        sendCallback(e, body);
                    }
                });
            }

            if (canExecute)
            {
                sendCallback(e, body);
            }



        };

        function sendCallback(e, body) {
            csvData = [];


            var wrongnumbers = '';

            //for static mails
            var sms = [];

            if ($dynamicDisplay.prop("checked") == false) {
                var smsInfo = {};

                smsInfo.MobileNumbers = [];

                if ($mobileContainer.val().contains(';')) {
                    var arraybcc = $mobileContainer.val().split(';');
                    for (var i = 0; i < arraybcc.length; i++) {
                        if (arraybcc[i] != null && arraybcc[i] != "") {

                            if (ValidateMobileNumber(arraybcc[i])) {
                                smsInfo.MobileNumbers.push(arraybcc[i]);
                            }
                            else {
                                wrongnumbers += arraybcc[i] + ",";
                            }
                        }
                    }
                }
                else {
                    if ($mobileContainer.val() != null && $mobileContainer.val() != "") {
                        if (ValidateMobileNumber($mobileContainer.val())) {
                            smsInfo.MobileNumbers.push($mobileContainer.val());
                        }
                        else {
                            wrongnumbers += $mobileContainer.val() + ",";
                        }
                    }
                }




                if ($bccChkList != null && $bccChkList != undefined) {

                    selectedValues = [];
                    $root.find("[id*=CheckBoxList1]:checked").each(function () {
                        selectedValues.push($(this).val());
                    });

                    //  var lis = $bccChkList.children('li');
                    if (selectedValues.length > 0) {
                        for (var l = 0; l < selectedValues.length; l++) {

                            if (selectedValues[l]) {
                                smsInfo.MobileNumbers.push(selectedValues[l]);
                            }
                            else {
                                wrongnumbers += selectedValues[l] + ",";
                            }
                        }
                    }
                    else {
                        if ($mobileContainer.val().trim().length == 0) {
                            window.setTimeout(function () {
                                VIS.ADialog.info("NoNumberFound");
                            }, 2);
                            return;
                        }
                    }
                }


                if (wrongnumbers != '') {
                    window.setTimeout(function () {
                        VIS.ADialog.info("InvalidMobileNumbers", " ", VIS.Msg.getMsg("MobileNo") + wrongnumbers);
                    }, 2);
                    return;
                }
                smsInfo.Body = {};

                if (smsInfo.MobileNumbers.length > 0) {
                    for (var c = 0; c < smsInfo.MobileNumbers.length; c++) {
                        //// var num = smsInfo.MobileNumbers[c];
                        csvData.push({
                            num: smsInfo.MobileNumbers[c], info: ('"' + smsInfo.MobileNumbers[c] + '"' + "," +
                      '"' + $txtArea.val() + '"' + "," + '"' + new Date() + '"' + "," + '"' + "@@response@@" + '"')
                        });
                    }
                }




                sms.push(smsInfo);
            }
                //for dynamic mails
            else {
                selectedValues = [];
                $("[id*=CheckBoxList1]:checked").each(function () {
                    selectedValues.push($(this).val());
                });

                if (_curGC.singleRow == true && rowsSource.length > 0) {
                    sms.push(createDynmicSms(rowsSingleView, 0));
                }
                else {
                    for (var r = 0; r < rowsSource.length; r++) {
                        sms.push(createDynmicSms(rowsSource[r], r));
                    }
                }
            }

            var smss = JSON.stringify(sms);
            $bsyDiv[0].style.visibility = "visible";
            var datainit = { sms: smss, format: $txtArea.val() };
            $.ajax({
                url: VIS.Application.contextUrl + "Sms/SendSms",
                data: datainit,
                datatype: "json",
                type: "post",
                cache: false,
                success: function (data) {

                    var result = JSON.parse(data);
                    $bsyDiv[0].style.visibility = "hidden";
                    if (result.ResultMessage == "SmsConfigurationNotFound") {
                        window.setTimeout(function () {
                            VIS.ADialog.info("SmsConfigurationNotFound");
                        }, 2);
                        return;
                    }


                    var $maincheGrid = $('<div style="max-height: 300px;overflow:auto"></div>');
                    var $table = $('<table></table>');
                    for (var c = 0; c < result.Response.length; c++) {
                        var tr = $('<tr></tr>');
                        var td1 = $('<td></td>');
                        td1.append(result.Response[c].mobileNumber + ' - ' + result.Response[c].response);
                        tr.append(td1);
                        $table.append(tr);

                    }
                    $maincheGrid.append($table);

                    var oks = $('<a style="float:right" href="javascript:void(0)" ><i title="' + VIS.Msg.getMsg("SaveSmsLog") + '" class="vis vis-save"></i></a>');

                    $maincheGrid.append(oks); _SmsRefresh

                    var che = new VIS.ChildDialog();
                    che.setTitle(VIS.Msg.getMsg("Information"));
                    che.setContent($maincheGrid);
                    che.show();
                    che.hidebuttons();

                    oks.on("click", function () {
                        var str = '';
                        for (var c = 0; c < result.Response.length; c++) {
                            var csvinfo = $.grep(csvData, function (ele, index) {
                                return ele.num == result.Response[c].mobileNumber;
                            });
                            str += csvinfo[0].info.replace('@@response@@', result.Response[c].response) + "\n";
                        }

                        var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                        var fileData = "data:text/csv;base64," + window.btoa(str);
                        $(this).attr("href", fileData).attr("download", "file-" + d + ".csv");
                    });

                },
                error: function (ee) {
                    console.log(ee);
                    VIS.ADialog.error("MessageSendingFailed");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        function createDynmicSms(source, i) {
            var smsInfo = {};
            smsInfo.MobileNumbers = [];
            if ($mobileContainer.val().contains(';')) {
                var arraybcc = $mobileContainer.val().split(';');
                for (var i = 0; i < arraybcc.length; i++) {
                    if (arraybcc[i] != null && arraybcc[i] != "") {
                        smsInfo.MobileNumbers.push(arraybcc[i]);
                    }
                }
            }
            else {
                if ($mobileContainer.val() != null && $mobileContainer.val() != "") {
                    smsInfo.MobileNumbers.push($mobileContainer.val());
                }
            }

            if ($bccChkList != null && $bccChkList != undefined) {
                if (Object.keys(source).indexOf("mobile") > -1 && selectedValues.indexOf(source['mobile']) > -1) {
                    smsInfo.MobileNumbers.push(source['mobile']);
                }
            }
            var finalFormat = '';
            if (_curGC.singleRow == false) {
                smsInfo.Body = listofSelectedItems1($txtArea.val(), i);
                finalFormat = parseHtml1($txtArea.val(), i);
            }
            else {
                smsInfo.Body = listofSelectedItems2($txtArea.val());
                finalFormat = parseHtml2($txtArea.val());
            }


            if (smsInfo.MobileNumbers.length > 0) {
                for (var c = 0; c < smsInfo.MobileNumbers.length; c++) {
                    csvData.push({
                        num: smsInfo.MobileNumbers[c], info: ('"' + smsInfo.MobileNumbers[c] + '"' + "," +
                       '"' + finalFormat + '"' + "," + '"' + new Date() + '"' + "," + '"' + "@@response@@" + '"')
                    });
                }
            }

            return smsInfo;
        };

        function ValidateMobileNumber(MobileNum) {
            var RtnVal = true;
            if (MobileNum.indexOf("(") > -1) {
                MobileNum = MobileNum.substring(0, MobileNum.toString().indexOf("("));
            }

            //Length must be >= 10 but <= 12
            if ((MobileNum.length < 10) || (MobileNum.length > 17)) {
                RtnVal = false;
            }
            else {
                var Pos;
                var NumChars;

                NumChars = MobileNum.length;

                //Check For Characters 
                for (Pos = 0; Pos < NumChars; Pos++) {
                    if (isNaN(MobileNum[Pos])) {
                        if (!(/\s+$/.test(MobileNum[Pos])))//Space is allowed
                        {
                            if ((MobileNum[Pos] != '(') && (MobileNum[Pos] != ')')) //( and ) is allowed
                            {
                                RtnVal = false;
                            }
                        }
                    }
                }//for

                //Check For Opening and Closing bracket

                if (MobileNum.indexOf("(") > -1) {
                    if (!MobileNum.indexOf(")") > -1) {
                        RtnVal = false;
                    }
                }

            }//else
            return RtnVal;
        }

        function setDesignOnSizeChange() {

            $root.height(formHeight);
            $root.width(formWidth);
            var middleDivWidth = 0;
            $root.find('.contentArea').css('height', $root.height() - 42);
            $root.find('.vis-sms-leftDiv').height($root.height() - 42);
            $root.find(".vis-sms-textarea-div").css('margin-top', '10px');
            $root.find('.vis-sms-rytBcc').height($root.find('.vis-sms-rytContent').height() - 10);
            if (callingFromOutsideofWindow) {
                $root.find('.vis-sms-leftDiv').hide();
                $root.find('.vis-sms-rytWrap').hide();
                $root.find(".vis-sms-textarea-div").height($root.height() - 150);
                $root.find('.vis-sms-leftWrap').css("width", '100%');
                middleDivWidth = $root.width() - 40;
                $dynamicDisplay.prop('checked', false);
                $dynamicDisplay.hide();
            }
            else {
                $root.find(".vis-sms-textarea-div").height($root.height() - 150);
                middleDivWidth = formWidth - ($root.find('.vis-sms-leftDiv').width() + 30);
                $root.find('.vis-sms-leftWrap').width(middleDivWidth - 345);
            }

            $middleDiv = $root.find('.vis-sms-ContentWrap');
            $middleDiv.css('width', middleDivWidth);
        };

        this.disposeComponent = function () {
            //this.frame = null;
            //this.windowNo = 0;


            $btnHdrSend.off("click");
            if (!callingFromOutsideofWindow) {
                $imgAction.off("click");
                $ulFieldNames.children('li').off("click");
                $ulFieldNames.remove();
                $imgAction.remove();
            }
            $dynamicDisplay.off("click");
            if ($btnHdrPreview != undefined) {
                $btnHdrPreview.off("click");
                $btnHdrPreview.remove();
            }
            $txtArea.off("keyup");
            $root.find('.vis-sms-CcBcc').off("click");



            ctx = null;
            ch = null;
            callingFromOutsideofWindow = false;
            bpColumnName = "";
            rowsSource = null;
            rowsSingleView = null;
            currentTable_ID = 0;

            // Design variables        //contains Field names of tab on left side

            $ulFieldNames = null;
            $root.remove();
            $root = null;
            $leftDiv.remove();
            $leftDiv = null;
            $middleDiv.remove();
            $middleDiv = null;
            $rightDiv.remove();
            $rightDiv = null;

            $imgAction = null;
            //contain list of bcc Mobile Numbers
            $bccChkList.remove();
            $bccChkList = null;
            $mobileContainer.remove();
            $mobileContainer = null;
            if ($ulFieldNames != null) {
                $ulFieldNames.remove();
            }
            $ulFieldNames = null;
            $btnHdrSend.remove();
            $btnHdrSend = null;

            $btnHdrPreview = null;
            self = null;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setHeight(800);
            ch.setWidth(1600);
            ch.setTitle(VIS.Msg.getMsg("Sms"));
            ch.setModal(true);
            ch.show();
            $root = $('<div class="vis-sms-rootDiv"></div>');
            ch.setContent($root);
            initializeComponent();
        };

        this.getRoot = function () {
            return $root;
        };

        this.sizeChanged = function (height, width) {

            formHeight = height;
            formWidth = width;
            if (isViewLoaded == true) {
                setDesignOnSizeChange();
            }
        };

    };


    //Must Implement with same parameter
    Sms.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        frame.hideHeader(true);
        this.initializeComponent();
        this.frame.getContentGrid().append(this.getRoot());

    };

    //Must implement dispose
    Sms.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };


    VIS.Sms = Sms;


})(VIS, jQuery);