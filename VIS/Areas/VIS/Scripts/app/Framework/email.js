//; vis = window.VACOM || {};
; (function (VIS, $) {

    function Email(to, _curtab, _curGC, Record_ID, isEmail, isWindowForm, tableID, body, subject, attchID) {

        //local variables          
        this.frame = null;
        this.windowNo = VIS.Env.getWindowNo();
        var ctx = VIS.Env.getCtx();
        var ch = null;
        var bpColumnName = "C_BPARTNER_ID";
        var rowsSource = null;
        var rowsSingleView = null;
        var selectedTemplateID = 0;
        var formatName;
        var saveForAllWindows;
        var lstLatestFiles = [];
        var oldFiles = [];
        var dLAContent = null;
        var dOldAtt = null;
        var dOAContent = null;
        var attachmentID = 0;
        if (attchID) {
            attachmentID = attchID;
        }

        //Is used to hide buttons of select file and upload , if it is set true then attachment design wil be changed
        var newdesign = true;
        // used to check email entered in inputs are valid or not
        var hsaValidinputmails = false;
        //  var dOAContent = null;

        var currentTable_ID = 0;
        var selectedValues = [];
        var callingFromOutsideofWindow = false;
        var filesforAttachmentforOpenFormat = [];
        var filesforAttachmentforNewAttachment = [];
        //var attachmentContainerOpen = false;

        // Design variables        //contains Field names of tab on left side
        var $ulFieldNames = null;
        var $root = null;
        var $leftDiv = null;
        var $middleDiv = null;
        var $rightDiv = null;
        var $attachDmsDoc = null;
        var $imgAction = null;
        //contain list of bcc emails
        var $bccChkList = null;
        var $dynamicDisplay = null;
        var $to = null;
        var $bcc = null;
        var $cc = null;
        var $refresh = null;
        var $attachment = null;
        var $subject = null;
        // Text Area of editor
        var $txtArea = null;
        // JQuery object of Text Area of editor after inserting kendo ui
        var $textAreakeno = null;
        //contains dynmic display checkbox and rest
        var $leftfootArea = null;
        // preview button
        var $btnHdrPreview = null;
        //send Button
        var $btnHdrSend = null;
        //open saved format button
        var $btnHdrOpen = null;
        //save new Format button
        var $btnHdrSaveAs = null;
        //save exsisting format if there, else save new format
        var $btnHdrSave = null;
        var $savePdf = null;
        var $btnHdrCancel = null;
        var $bsyDiv;
        var $toolbarDiv = $('<div class="vis-awindow-header vis-menuTitle">');
        var self = this;
        var $uploaddiv;
        var $btnupload;
        var $Attachdiv;
        var $insertAttachdiv;
        var $btnSavePdf;

        var $btnNewLetter;

        var $btnInsert;
        var formHeight = 0;
        var formWidth = 0;
        var winNo = self.windowNo;
        var arrWindowNo = [];
        $root = $('<div class="vis-email-rootDiv vis-forms-container"></div>');
        var isBusy = $('<div class="vis-email-rootDivBusy"></div>');
        var $AttacmentOption = $("<ul id='ulAttachmentOption' class='vis-apanel-rb-ul'>");
        // Added By Param On date 18-sep-2015
        //Start//
        var doc = null;
        var $fileSystemOption = null;
        var $dmsOption = null;
        //End//
        var isViewLoaded = false;
        var $lineImgDiv;

        var $toggleAction;
        var $scrollIcon;
        var flagImg = false;



        var divReadOnly = $("<div style='width: 100%;height: 100%;background:black;opacity: .1;display:none'>");//table-cell
        var divProgress = null;
        if (VIS.Application.isRTL) {
            divProgress = $("<div id='progress_bar' class='vis-ui-progress-bar ui-container' style='top:41%;width: 80%;right: 10%;'>");

        }
        else {
            divProgress = $("<div id='progress_bar' class='vis-ui-progress-bar ui-container' style='top:41%;width: 80%;left: 9%;'>");
        }
        divReadOnly.append(divProgress);
        var divGreen = $("<div class='vis-ui-progress' style='width: 0%; display: block;position:absolute;margin-top:2px'>");
        divProgress.append(divGreen);
        var lblpercentage = $("<span class='ui-label' >").append("0%");
        divProgress.append(lblpercentage);



        //Atachment
        var currentchunk = 0;
        var currentFile = 0;
        var folder = Date.now().toString();
        var chunkSize = 1 * 1024 * 1024;
        var totalChunks = 0;
        var currentFileChunkNo = 0;
        var filesInfo = [];
        var htt = '';
        var fileBrowser = null;
        var file;
        var $dmsheader = null;
        var $divlbNav;
        // var $divlbNav;

        var $SubjectTextChange = 0;


        //Lakhwinder
        //send printformats as attachment Dynamically
        var $chkBSendPFasAtt = null;
        var $cmbPfFiletype = null;

        /** Tab panel-adding cc & bcc emails ** Dt: 28/06/2021 ** Modified By: Kumar **/
        var ccmail = '';
        var bccmail = '';

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
            else {
                if (tableID != null && tableID != undefined) {
                    currentTable_ID = tableID;
                }
            }
            $bccChkList = $('<ul class="vis-email-ul" ></ul>');


            $bsyDiv = $("<div>");
            $bsyDiv.css("position", "absolute");
            $bsyDiv.css("bottom", "0");
            $bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/LoadingCircle.gif') no-repeat");
            $bsyDiv.css("background-position", "center center");
            $bsyDiv.css("width", "98%");
            $bsyDiv.css("height", "98%");
            $bsyDiv.css('text-align', 'center');
            $bsyDiv.css('opacity', '.1');
            $bsyDiv[0].style.visibility = "hidden";

        };

        this.initializeComponent = function () {

            var $btncloseChart = null;
            var pheader;
            if (isEmail == true) {
                $btncloseChart = $('<a href="javascript:void(0)"  class="vis-icon-menuclose"><i class="vis vis-cross"></i></a>');
                if (callingFromOutsideofWindow) {
                    pheader = $('<p>' + VIS.Msg.getMsg("EMail") + ' (' + VIS.Msg.getMsg("Contacts") + ')' + ' </p>');
                }
                else {
                    pheader = $('<p>' + VIS.Msg.getMsg("EMail") + ' (' + _curGC.aPanel.$parentWindow.getName() + ')' + ' </p>');
                }
                //$btncloseChart.append(pheader);

            }
            else {
                $btncloseChart = $('<a href="javascript:void(0)"  class="vis-icon-menuclose"><i class="vis vis-cross"></i></a>');
                if (callingFromOutsideofWindow) {
                    pheader = $('<p>' + VIS.Msg.getMsg("Letter") + ' (' + VIS.Msg.getMsg("Contacts") + ')' + ' </p>');
                }
                else {
                    pheader = $('<p>' + VIS.Msg.getMsg("Letter") + ' (' + _curGC.aPanel.$parentWindow.getName() + ')' + ' </p>');
                }
                //$btncloseChart.append(pheader);
            }

            $root.append($toolbarDiv.append($btncloseChart).append(pheader));

            $btncloseChart.click(function (e) {
                self.dispose();
                self = null;
                e.stopPropagation();
            });

            //if (checktextArea() == false) {
            //    return;
            //}


            loadView();

            if (loadTextArea() == false) {
                return;
            }

            $root.append($bsyDiv);
            Addbuttons();
            if (!callingFromOutsideofWindow) {
                if (isEmail) {
                    loadEmails(false);
                }
            }

            if (!callingFromOutsideofWindow) {
                loadFields(false);
                addPrintOption();
            }
            //  loadTextArea();
            //createDesign();

            eventhandlers();
        };

        this.getRoot = function () {
            return $root;
        };

        function Addbuttons() {
            if (isEmail) {
                $btnHdrSend = $('<i class="vis-email-sendbtn vis vis-paper-plane"  title="' + VIS.Msg.getMsg("SendMail").replace('&', '') + '"></i>');
                $toolbarDiv.append($btnHdrSend);
            }
            else {
                $btnHdrSend = $('<i class="vis-email-saveAttachbtn vis vis-save-attach"  title="' + VIS.Msg.getMsg("SaveAttachment").replace('&', '') + '" ></i>');
                $toolbarDiv.append($btnHdrSend);
            }

            //$btnHdrdDivider = $('<img  style="margin-top:10px;margin-right:20px;cursor: pointer;float:right"  src="' + VIS.Application.contextUrl + 'Areas/vis/Images/divider.png"> </img>');
            //$toolbarDiv.append($btnHdrdDivider);

            //$btnHdrCancel = $('<img  style="margin-top:10px;margin-right:20px;cursor: pointer;float:right" title="' + VIS.Msg.getMsg("Cancel").replace('&', '') + '" src="' + VIS.Application.contextUrl + 'Areas/vis/Images/cancel.png"> </img>');
            //$toolbarDiv.append($btnHdrCancel);
            if (!callingFromOutsideofWindow) {
                $btnHdrPreview = $('<i class="vis-email-btns vis vis-viewdocument"  title="' + VIS.Msg.getMsg("Preview").replace('&', '') + '" ></i>');
                $toolbarDiv.append($btnHdrPreview);

                $btnHdrSaveAs = $('<i class="vis-email-btns vis vis-save-as" title="' + VIS.Msg.getMsg("SaveAs").replace('&', '') + '" ></i>');
                $toolbarDiv.append($btnHdrSaveAs);

                $btnHdrSave = $('<i class="vis-email-btns vis vis-save" title="' + VIS.Msg.getMsg("Save").replace('&', '') + '"></i>');
                $toolbarDiv.append($btnHdrSave);

                $btnHdrOpen = $('<i class="vis-email-btns vis vis-open-file" title="' + VIS.Msg.getMsg("Open").replace('&', '') + '" ></i>');
                $toolbarDiv.append($btnHdrOpen);
            }
            if (!isEmail) {
                $btnSavePdf = $('<i class="vis-email-btns vis vis-save-pdf" title="' + VIS.Msg.getMsg("SavePdf").replace('&', '') + '"></i>');
                ///Manish 31/5/2016                                                                                                                                    
                $btnNewLetter = $('<i class="vis-email-btns vis vis-new" title="' + VIS.Msg.getMsg("NewLetter").replace('&', '') + '" ></i>');
                ///End 31/5/2016
                $toolbarDiv.append($btnSavePdf).append($btnNewLetter);
            }
        }

        function loadView() {
            var datainit = {
                windowNo: self.windowNo,
                language: VIS.Env.getAD_Language(VIS.Env.getCtx())
            };

            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Email/Init",
                data: datainit,
                success: function (data) {
                    setDesign(data);
                },
            });
        };

        function setDesign(data) {

            $root.append($(data));
            var middleDivWidth = 0;
            $dynamicDisplay = $root.find('#' + self.windowNo + "_dyndis");

            $chkBSendPFasAtt = $root.find('#' + self.windowNo + "_dynPF");
            $cmbPfFiletype = $root.find('#' + self.windowNo + "_dynPFType");

            $root.find('.vis-email-attachmentContainer').hide();

            $leftDiv = $root.find('.vis-email-leftDiv');
            $rightDiv = $root.find('.vis-Email-BccList');
            $subject = $root.find('#' + self.windowNo + "_emailSubject");
            //Add By Param on Date 22-sep-2015//
            $attachDmsDoc = $root.find('.vis-DMSAttachDocList');
            $dmsheader = $root.find('.vis-dmsdocumentheader');

            if (!window.VADMS) {
                $attachDmsDoc.css('display', 'none');
                $dmsheader.css('display', 'none');
            }
            //End//
            if (subject != undefined && subject != null) {
                $subject.val(subject);
            }

            $attachment = $root.find('#' + self.windowNo + "_vis-email-attacImg");
            $txtArea = $root.find('#' + self.windowNo + "_vis-Email-textarea");
            $leftfootArea = $root.find(".vis-Email-leftFooter");


            //$root.find('.vis-form-data-sub').css('margin-top', '10px');
            $root.find('.contentArea').css('height', $root.height() - 44);
            $root.find(".vis-Email-textarea-div").css({ 'margin-top': '10px' });

            var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase())
            if (ismobile == true) {
                $root.find(".vis-Email-textarea-div").css({ 'overflow': 'auto' });
            }


            $root.find('.vis-email-leftDiv').height($root.find('.contentArea').height());
            if (callingFromOutsideofWindow) {
                $root.find('.vis-form-data-sub').css('margin-top', '0px');
                $root.find('.vis-email-leftDiv').hide();
                $root.find('.vis-Email-rytWrap').hide();
                $to = $root.find('#' + self.windowNo + "_emailToTop");
                $bcc = $root.find('#' + self.windowNo + "_emailBccTop");
                $cc = $root.find('#' + self.windowNo + "_eamilCcTop");

                /** Tab panel-adding cc & bcc emails ** Dt: 28/06/2021 ** Modified By: Kumar **/
                if (ccmail != undefined && ccmail != null && ccmail != '') {
                    $cc.val(ccmail);
                }
                else
                    $cc.hide();

                /** Tab panel-adding cc & bcc emails ** Dt: 28/06/2021 ** Modified By: Kumar **/
                if (bccmail != undefined && bccmail != null && bccmail != '') {
                    $bcc.val(bccmail);
                }
                else
                    $bcc.hide();

                $root.find(".vis-Email-textarea-div").height($root.height() - (180));

                $root.find('.vis-Email-leftWrap').css("width", '100%');
                $root.find('.vis-Email-leftFooter').hide();
                middleDivWidth = $root.width() - 40;
                $dynamicDisplay.prop('checked', false);
                $root.find('.vis-Email-topinputWrap').css('overflow', 'auto');
                $dynamicDisplay.hide();


                $chkBSendPFasAtt.prop('checked', false);
                $chkBSendPFasAtt.parent().hide();
                $cmbPfFiletype.prop('checked', false);
                $cmbPfFiletype.parent().hide();

                if (to != undefined && to != null) {
                    $to.val(to);
                }

                //$root.find('.vis-form-data-sub').css({ 'width': '100%', 'padding-right': '42px' });
                //$root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 60);
                $root.find('.vis-form-data-sub').width($root.width() - 85);

            }
            else {
                if (isEmail) {
                    $root.find('.vis-Email-topinputWrap').hide();
                    $to = $root.find('#' + self.windowNo + "_emailTo");
                    $bcc = $root.find('#' + self.windowNo + "_emailBcc");
                    $cc = $root.find('#' + self.windowNo + "_eamilCc");
                    $root.find(".vis-Email-textarea-div").height($root.height() - (115 + 30));
                    $refresh = $root.find('#' + self.windowNo + "_emailRefresh");

                    //if ($textAreakeno != null && $textAreakeno != undefined) {
                    //    $textAreakeno.body.style.height = ($root.find(".vis-Email-textarea-div").height() - 80) + "px";
                    //}
                    middleDivWidth = $root.width() - 90;
                    $root.find('.vis-Email-leftWrap').width(middleDivWidth - 350);
                    $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 60);
                }
                else {
                    $root.find('.vis-Email-rytWrap').hide();
                    $root.find('.vis-Email-topinputWrap').hide();
                    $attachment.hide();
                    $root.find(".vis-Email-textarea-div").height($root.height() - (125 + 17));
                    $root.find('.vis-Email-leftWrap').css("margin-right", '20px');
                    middleDivWidth = $root.width() - 240;
                    $root.find('.vis-Email-leftWrap').width(middleDivWidth - 25);
                    //$root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width());
                    $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 60);

                }

            }



            if (window.VADMS) {
                $root.find('.vis-Email-BccList').height(($root.find('.vis-Email-rytContent').height() - ($root.find('.vis-Email-rytForm').height() + 70)) - 520);
            }
            else {
                $root.find('.vis-Email-BccList').height(($root.find('.vis-Email-rytContent').height() - ($root.find('.vis-Email-rytForm').height() + 70)) - 280);
            }

            $middleDiv = $root.find('.vis-Email-ContentWrap');
            $middleDiv.css('width', middleDivWidth);

            if ($textAreakeno != null && $textAreakeno != undefined) {
                $textAreakeno.body.style.width = (middleDivWidth - 370) + "px";
            }

            isViewLoaded = true;
        };



        function loadTextArea() {
            try {
                $txtArea.kendoEditor({
                    tools: [
                        "bold",
                        "italic",
                        "underline",
                        "strikethrough",
                        "justifyLeft",
                        "justifyCenter",
                        "justifyRight",
                        "justifyFull",
                        "insertUnorderedList",
                        "insertOrderedList",
                        "indent",
                        "outdent",
                        "createLink",
                        "unlink",
                        "insertImage",
                        "insertFile",
                        "subscript",
                        "superscript",
                        "createTable",
                        "addRowAbove",
                        "addRowBelow",
                        "addColumnLeft",
                        "addColumnRight",
                        "deleteRow",
                        "deleteColumn",
                        "viewHtml",
                        "formatting",
                        "cleanFormatting",
                        "fontName",
                        {
                            name: "fontSize",
                            items: [].concat(
                                [{ text: "8px", value: "8px" }],
                                [{ text: "12px", value: "12px" }],
                                [{ text: "16px", value: "16px" }],
                                [{ text: "20px", value: "20px" }],
                                [{ text: "24px", value: "24px" }]
                            )
                        },
                        "foreColor",
                        "backColor"
                    ],
                    keyup: getTextChange,
                    encoded: false
                });

                $textAreakeno = $txtArea.data("kendoEditor");
                $textAreakeno.value("");

                if (body != undefined && body != null) {
                    $textAreakeno.value(body);
                }
            }
            catch (ex) {
                console.log(ex);
                VIS.ADialog.error("PleaseInstallKendoUIModule");
                self.dispose();
                self = null;
                return false;
            }



            return true;

            //$textAreakeno.body.style.height = ($root.find(".vis-Email-textarea-div").height() - 80) + "px";
            //if ($textAreakeno != null && $textAreakeno != undefined) {
            //    $textAreakeno.body.style.width = ($middleDiv.width() - 370) + "px";
            //}
        };

        function getTextChange() {
            //if ($textAreakeno.value() != "") {
            //    if ($subject.val() != "") {
            //        $SubjectTextChange = 1;
            //    }
            //    else {
            //        $SubjectTextChange = 0;
            //    }
            //}
            //else {
            //    $SubjectTextChange = 0;
            //}
            if ($textAreakeno.value() != "" || $subject.val() != "") {
                $SubjectTextChange = 1;
            }

        };


        function loadEmails(refersh) {

            if (refersh == true) {
                //$bccChkList.empty();
                // this is data source for multiView Records
                rowsSource = _curGC.getSelectedRows();

                // this is data source for single View Records
                rowsSingleView = _curtab.getRecords()[_curtab.getCurrentRow()];
            }
            if (_curGC.singleRow && rowsSource.length == 0)//show records for single window
            {
                loademailforSingleView();
            }
            else {
                if (rowsSource.length > 0) {
                    var htm = '';
                    for (var i = 0; i < rowsSource.length; i++) {

                        // else {
                        if (isEmail) {
                            if (rowsSource[i]["email"] != null) {
                                isEmail = true;
                                var ID = rowsSource[i][_curtab.getKeyColumnName().toLower()];

                                htm += '<li class="vis-list-li-bcc"  data-email="' + rowsSource[i]["email"] + '"><input data-currentrec="Y" data-recID="' + ID + '" id="' + self.windowNo + '_' + rowsSource[i]["email"] + '_CheckBoxList1" type="checkbox" value="' + rowsSource[i]["email"] + '" checked/><label class="vis-chcklist-label"  for="' + self.windowNo + '_' + rowsSource[i]["email"] + '_CheckBoxList1">' + rowsSource[i]["email"] + '(' + ID + ')</label></li>';
                            }

                        }
                    }
                    $bccChkList.append(htm);
                    htm = null;
                }
                else {
                    loademailforSingleView();
                }
            }
            if (!refersh) {
                $rightDiv.append($bccChkList);
            }
        };

        function loademailforSingleView() {
            if (isEmail) {
                if (rowsSingleView["email"] != null) {
                    isEmail = true;
                    var ID = rowsSingleView[_curtab.getKeyColumnName().toLower()];

                    $bccChkList.append('<li class="vis-list-li-bcc" data-email="' + rowsSingleView["email"] + '"><input id="' + self.windowNo + '_' + rowsSingleView["email"] + '_CheckBoxList1" type="checkbox" value="' + rowsSingleView["email"]
                        + '" checked /><label class="vis-chcklist-label" for="' + self.windowNo + '_' + rowsSingleView["email"] + '_CheckBoxList1">' + rowsSingleView["email"] + '(' + ID + ')</label></li>');
                }
            }
        };

        function loadFields(isRefresh) {


            if (!isRefresh) {
                $lineImgDiv = $('<div class="vis-apanel-lb-toggle">');

                $imgAction = $('<i class="fa fa-bars"></i>');
                //$toggleAction = $('<div class="vis-apanel-lb-toggle" style="height:100%;overflow-x:hidden;overflow-y: hidden;"></div>');
                $toggleAction = $('<div class="vis-apanel-lb-toggle" style="border:none;height:100%;overflow-x:hidden;overflow-y: hidden;"></div>');


                ///$divlbNav = $("<div class='vis-apanel-lb-oflow'>").hide();
                $divlbNav = $("<div class='vis-apanel-lb-oflow' style='border-top:0;display:none' >");
                $divlbNav.html("<a data-dir='u' href='javascript:void(0)'><i class='vis vis-arrow-up' data-dir='u'></i></a><a data-dir='d' href='javascript:void(0)' ><i class='vis vis-arrow-down' data-dir='d'></i></a>");

                $scrollIcon = $('<div class="vis-apanel-lb-toggle" style="padding: 5px 10px;overflow-x:hidden;overflow-y: hidden;"></div>');
                $scrollIcon.append($divlbNav);


                //$toggleAction.append($imgAction);
                $lineImgDiv.append($imgAction);
                $leftDiv.append($lineImgDiv);
                $leftDiv.append($toggleAction);
                $leftDiv.append($scrollIcon);

                //$divlbNav.hide();                
                //$toggleAction.css("border-bottom", "none");

                $ulFieldNames = $('<ul class="vis-apanel-lb-ul"></ul>');

                //$ulFieldNames.append($divlbNav);
                $ulFieldNames.css({ "overflow": "auto", "overflow-x": "hidden", "white-space": "nowrap", "width": "100%" });

            }
            else {
                $ulFieldNames.empty();
            }


            var htmlmail = '';

            for (var i = 0; i < _curtab.gridTable.getFields().length; i++) {

                var iskeyColumn = (_curtab.gridTable.getFields(0)[i].getColumnName().toUpper() == _curtab.getKeyColumnName().toUpper());// in case of custmer Master , C_BPartner is not displayed but it is required to show its user's email which have isEmail=Y

                if (!_curtab.gridTable.getFields(0)[i].getIsDisplayed() && !iskeyColumn) {
                    continue;
                }

                if (!iskeyColumn) {
                    htmlmail += '<li>' + _curtab.gridTable.getFields(0)[i].getHeader() + '</li>';
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
            if (!iskeyColumn) {
                $ulFieldNames.append(htmlmail);
            }


            if (!isRefresh) {
                $toggleAction.append($ulFieldNames);
            }

            $ulFieldNames.children('li').on("click", insertSelectedField)
        };



        this.leftULsize = function () {
            //debugger;

            if (!callingFromOutsideofWindow) {
                var ulHeight = ($scrollIcon.height() + $lineImgDiv.height()) + 53;
                $($ulFieldNames.parent()).height($leftDiv.height() - ulHeight);


                //$ulFieldNames.height($leftDiv.height() - ulHeight);
                //$toggleAction.height($leftDiv.height() - ulHeight);
                if (flagImg == true) {
                    leftULdynamicheight();
                }
            }

        };

        function leftULdynamicheight() {
            debugger;

            if (!callingFromOutsideofWindow) {
                if ($ulFieldNames.height() > $($ulFieldNames.parent()).height()) {
                    $toggleAction.css("border-bottom", "1px solid rgba(var(--v-c-secondary), 1)");
                    $divlbNav.css("display", "block");

                    var ulHeight = ($scrollIcon.height() + $lineImgDiv.height()) + 37;
                    $toggleAction.height($leftDiv.height() - ulHeight);

                    $leftDiv.css("overflow", "hidden");
                    $toggleAction.css({ "overflow-x": "hidden", "width": "100%", "overflow-y": "visible" });


                }
                else {
                    $toggleAction.css("border-bottom", "none");
                    $divlbNav.css("display", "none");

                    //$divlbNav.hide();
                }
            }
        };


        function fillIDsOFUsers(bpID, prID) {
            //Dictionary<int, string> dicExsit = vchkListBoxTop.GetKeyValues();
            //List<int> exsisitnIDS = vchkListBoxTop.GetKeyValues().Keys.ToList();




            var pvID = 0;
            for (var i = 0; i < bpID.length; i++) {
                //var sql = "Select AD_User_ID,email from ad_user where isEmail='Y' AND c_bpartner_ID=" + bpID[i];
                //var ds = VIS.DB.executeDataSet(sql);
                var ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Email/GetUser", { "BPartner_ID": bpID[i] }, null);
                var isBroken = false;
                var htm = '';
                //if (ds != null && ds.getTables()[0].getRows().length > 0) {
                //    for (var j = 0; j < ds.getTables()[0].getRows().length; j++) {
                //        if (ds.getTables()[0].getRows()[j].getCell("email") != null) {
                //            //if (ds.getTables()[0].getRows()[j].getCell("ISEMAIL").toString().equals("Y")) {
                //            if (ds.getTables()[0].getRows()[j].getCell("EMAIL") == null || ds.getTables()[0].getRows()[j].getCell("EMAIL") == "") {
                //                continue;
                //            }

                //            if ($($bccChkList.children('li').data(ds.getTables()[0].getRows()[j].getCell("EMAIL") + prID[i])).length > 0) {
                //                continue;
                //            }

                //            htm += '<li  class="vis-list-li-bcc"><input id="' + self.windowNo + '_' + ds.getTables()[0].getRows()[j].getCell("EMAIL") + '_CheckBoxList1" type="checkbox"  value="' + ds.getTables()[0].getRows()[j].getCell("EMAIL")
                //                 + '" checked/><label class="vis-chcklist-label"  for="' + self.windowNo + '_' + ds.getTables()[0].getRows()[j].getCell("EMAIL") + '_CheckBoxList1">' + ds.getTables()[0].getRows()[j].getCell("EMAIL") + '(' + prID[i] + ')</label></li>';
                //            // }
                //        }
                //    }
                //}

                if (ds != null) {
                    for (var j in ds) {
                        if (ds[j]["Email"] != null) {
                            if (ds[j]["Email"] == null || ds[j]["Email"] == "") {
                                continue;
                            }

                            if ($($bccChkList.children('li').data(ds[j]["Email"] + prID[i])).length > 0) {
                                continue;
                            }

                            htm += '<li  class="vis-list-li-bcc"><input data-currentrec="N" data-recid="' + prID[i] + '" id="' + self.windowNo + '_' + ds[j]["Email"] + '_CheckBoxList1" type="checkbox"  value="' + ds[j]["Email"]
                                + '" checked/><label class="vis-chcklist-label"  for="' + self.windowNo + '_' + ds[j]["Email"] + '_CheckBoxList1">' + ds[j]["Email"] + '(' + prID[i] + ')</label></li>';
                            // }
                        }
                    }
                }
                $bccChkList.append(htm);
                htm = null;
            }
        };

        function eventhandlers() {


            //$attachment.on("click", attachContainer);

            //if ($dynamicDisplay.is(":checked"))
            //{
            //    $divlbNav.show();
            //    $toggleAction.css("border-bottom", "1px solid white");
            //}
            //else
            //{
            //    $divlbNav.hide();
            //    $toggleAction.css("border-bottom", "none");
            //};
            $subject.on("change", getTextChange);


            //$divlbNav.off("click");
            if (!callingFromOutsideofWindow) {
                $divlbNav.on("click", function (e) {



                    e.stopPropagation();
                    var dir = $(e.target).data('dir');
                    if (!dir) return;

                    var dHeight = ($ulFieldNames.parent()).height();
                    var ulheight = $ulFieldNames.height();
                    var cPos = ($ulFieldNames.parent()).scrollTop();
                    var offSet = Math.ceil(dHeight / 2);
                    var s = 0;
                    if (dir == 'd') {
                        if ((cPos + offSet) >= ulheight - offSet)
                            return;
                        var ms = ulheight - dHeight;
                        s = cPos + offSet;
                        ($ulFieldNames.parent()).animate({ scrollTop: s > ms ? ms : s }, 1000);
                    }
                    else if (dir == 'u') {
                        if (cPos == 0)
                            return;
                        s = (cPos - offSet);
                        ($ulFieldNames.parent()).animate({ scrollTop: s < 0 ? 0 : s }, 1000);
                        //$divTabControl.scrollLeft(cPos - offSet);
                    }
























                    //e.stopPropagation();
                    //var dir = $(e.target).data('dir');
                    //if (!dir) return;

                    //var dHeight = $toggleAction.height();
                    //var ulheight = $ulFieldNames.height();                
                    //var cPos = $toggleAction.scrollTop();
                    //var offSet = Math.ceil(dHeight / 2);
                    //var s = 0;
                    //if (dir == 'd') {
                    //    if ((cPos + offSet) >= (ulheight - offSet))
                    //        return;
                    //    var ms = ulheight - dHeight;
                    //    s = cPos + offSet;
                    //    $toggleAction.animate({ scrollTop: s > ms ? ms : s }, 1000, "easeOutBounce");
                    //}
                    //else if (dir == 'u')
                    //{
                    //    if (cPos == 0) {
                    //        return;
                    //    }
                    //    else {
                    //        s = (cPos - offSet);
                    //        $toggleAction.animate({ scrollTop: s < 0 ? 0 : s }, 1000, "easeOutBounce");
                    //    }
                    //    //$divTabControl.scrollLeft(cPos - offSet);
                    //}


                    //e.stopPropagation();
                    //var dir = $(e.target).data('dir');
                    //if (!dir) return;

                    //var dHeight = $leftDiv.height();
                    //var ulheight = $ulFieldNames.height();
                    //var cPos = $ulFieldNames.scrollTop();
                    //var offSet = Math.ceil(dHeight / 2);
                    //var s = 0;
                    //if (dir == 'd')
                    //{
                    //    if ((cPos + offSet) >= ulheight - offSet)
                    //        //return;
                    //    var ms = ulheight - dHeight;
                    //    s = cPos + offSet;
                    //    $ulFieldNames.animate({ scrollTop: s > ms ? ms : s }, 1000, "easeOutBounce");
                    //}
                    //else if (dir == 'u') {
                    //    if (cPos == 0)
                    //        return;
                    //    s = (cPos - offSet);
                    //    $ulFieldNames.animate({ scrollTop: s < 0 ? 0 : s }, 1000, "easeOutBounce");
                    //    //$divTabControl.scrollLeft(cPos - offSet);
                    //}
                });
            }




            $attachment.on("click", function () {
                AttachOptionOverLay();

            });
            $AttacmentOption.on("click", "li", function (evt) {
                evt.stopPropagation();
                var value = $(this).attr("value");
                if (parseInt(value) == 0) {
                    attachContainer(evt);
                }
                else if (parseInt(value) == 1) {
                    $("#w2ui-overlay").hide();
                    if (doc != null && doc.GetDmsCloseOrNot()) {
                        VIS.ADialog.info('DMSAlreadyOpen');
                        return;
                    }
                    var frame = new VIS.CFrame();
                    doc = new VADMS.DocumentManagementSystem();// window.VADMS.DocumentManagementSystem.prototype.init(VIS.Env.getWindowNo(), frame);
                    frame.setName(VIS.Msg.getMsg("VADMS_Document"));
                    frame.setTitle(VIS.Msg.getMsg("VADMS_Document"));
                    frame.hideHeader(true);
                    doc.setWindowNo(self.windowNo);
                    doc.OpenFromEmail(true);
                    frame.setContent(doc);
                    doc.initialize();
                    frame.show();
                    // arrWindowNo.push(winNo);
                    // winNo++;
                }

            });
            if (isEmail) {
                $btnHdrSend.on("click", send);
                if ($refresh != null && $refresh != undefined) {
                    $refresh.on("click", refresh);
                }
            }
            else {
                $btnHdrSend.on("click", saveAttachment);
            }
            if (!isEmail) {
                $btnSavePdf.on("click", pdf);
                ///Manish 31/5/2016
                $btnNewLetter.on("click", createNewLetter)
                /////End 31/5/2016
            }
            //ch.onClose = dispose;
            if (!callingFromOutsideofWindow) {

                $imgAction.on("click", imgActionClick);
                $dynamicDisplay.on("click", dynamicDisplaychange);
                ///  $ulFieldNames.children('li').on("click", insertSelectedField);
                $btnHdrPreview.on("click", preview);
                $btnHdrOpen.on("click", open);
                $btnHdrSaveAs.on("click", saveAs);
                $btnHdrSave.on("click", save);
                //$btnHdrCancel.on("click", cancel);
                $imgAction.trigger('click');

                $chkBSendPFasAtt.on("click", attachDynamicPF);

            }
            else {
                $root.find('.vis-Email-CcBcc').on("click", showBccpanel);
            }

            //$dynamicDisplay.trigger('click');
            setDesignOnSizeChange();
            //if (!isEmail) {
            //    $subject.on("keypress", function ()
            //    {
            //        if ($textAreakeno.value() != "" && $subject.val() != "") {
            //            $SubjectTextChange = 1;
            //        }
            //        else
            //        {
            //            $SubjectTextChange = 0;
            //        }
            //    });
            //}
            if (!isEmail) {
                $subject.keyup(function (e) {
                    if ($subject.val() != "") {
                        if ($textAreakeno.value() != "") {
                            $SubjectTextChange = 1;
                        }
                        else {
                            $SubjectTextChange = 0;
                        }
                    }
                    else {
                        $SubjectTextChange = 0;
                    }
                });
            }

        };

        function AttachOptionOverLay() {
            if ($AttacmentOption != null) {
                $AttacmentOption.empty();
            }

            var $root = $('<div/>');//, {
            $root.css('width', 'auto');

            $root.css('height', '100%');

            $AttacmentOption.css('width', 'auto');
            $AttacmentOption.css('height', '100%');
            var li = "<li class='vis-filesystem' value='0'>" + VIS.Msg.getMsg("FileSystem") + "</li> ";
            if (window.VADMS) {
                li += "<li class='vis-dms' value='1'>" + VIS.Msg.getMsg("DMS") + "</li>"
            }
            $AttacmentOption.append(li);
            $root.append($AttacmentOption);
            $attachment.w2overlay($root.clone(true), { css: { height: '300px' } });
            $root.css("top", "395px");
            $AttacmentOption.load();


        };


        function imgActionClick(e) {

            if ($dynamicDisplay.prop("checked") == true) {
                if ($leftDiv.width() == 40) {

                    //   $middleDiv.width($middleDiv.width() - 160);
                    // $root.find('.vis-Email-leftWrap').width($root.find('.vis-Email-leftWrap').width() - 160);
                    // $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 20);

                    if (!isEmail) {
                        middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 200);
                        $root.find('.vis-Email-leftWrap').width(middleDivWidth - 15);
                        $root.find('.vis-form-data-sub').width(middleDivWidth - 40);
                        $middleDiv.width(middleDivWidth);

                    }
                    else {
                        middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 200);
                        $root.find('.vis-Email-leftWrap').width(middleDivWidth - 355);
                        $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 45);
                        $middleDiv.width(middleDivWidth);
                    }
                    $leftDiv.css('width', '200px');
                    $leftDiv.find('ul').show();

                    leftULdynamicheight();

                    //$divlbNav.css("display", "inherit");
                    //$toggleAction.css("border-bottom", "1px solid white");

                    if ($textAreakeno != null && $textAreakeno != undefined) {
                        $textAreakeno.body.style.width = $textAreakeno.body.style.width - 160;
                    }
                }
                else {
                    shrinkLeftDiv();
                    //leftULdynamicheight();

                    $toggleAction.css("border-bottom", "none");
                    if (!callingFromOutsideofWindow) {
                        $divlbNav.css("display", "none");
                    }
                }
            }
            else {
                if ($leftDiv.width() != 40) {
                    shrinkLeftDiv();
                }
            }
            e.stopPropagation();
        };

        function shrinkLeftDiv() {
            $leftDiv.css({ 'width': '40px' });
            $leftDiv.find('ul').hide();
            //$middleDiv.width($middleDiv.width() + 160);
            //$root.find('.vis-Email-leftWrap').width($root.find('.vis-Email-leftWrap').width() + 160);
            //$root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 20);

            if (!isEmail) {
                middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 40);
                $root.find('.vis-Email-leftWrap').width(middleDivWidth - 15);
                $root.find('.vis-form-data-sub').width(middleDivWidth - 40);
                $middleDiv.width(middleDivWidth);
            }
            else {
                middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 40);
                $root.find('.vis-Email-leftWrap').width(middleDivWidth - 355);
                $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 45);
                $middleDiv.width(middleDivWidth);
            }

            if ($textAreakeno != null && $textAreakeno != undefined) {
                $textAreakeno.body.style.width = $textAreakeno.body.style.width + 160;
            }
        };

        function attachContainer(e) {
            if (newdesign == false) {
                if (!$root.find('.vis-email-attachmentContainer').is(":visible")) {
                    var conheight = $root.find('.vis-Email-ContentArea').height();

                    $root.find('.vis-email-attachmentContainer').show();

                    $root.find(".vis-Email-textarea-div").height($root.find(".vis-Email-textarea-div").height() - ($root.find(".vis-email-attachmentContainer").height() + 13));
                    if (fileBrowser == null || fileBrowser == undefined) {

                        createattachmentcontainer();
                        fileBrowser = $root.find("<input type='file' multiple='true' style='display:none;'>");
                        $uploaddiv.append(fileBrowser);

                        $btnupload.on("click", function () {
                            fileBrowser.trigger('click');

                        });

                        //Add file to Latest File Content
                        fileBrowser.change(function () {
                            AppendFile(this);
                        });
                    }
                }
                else {
                    $root.find('.vis-email-attachmentContainer').hide();
                    $root.find(".vis-Email-textarea-div").height($root.find(".vis-Email-textarea-div").height() + ($root.find(".vis-email-attachmentContainer").height() + 13));
                }
            }
            else {

                if (!$root.find('.vis-email-attachmentContainer').is(":visible")) {
                    var conheight = $root.find('.vis-Email-ContentArea').height();


                    // if (attachmentContainerOpen == false) {
                    if (fileBrowser == null || fileBrowser == undefined) {

                        createattachmentcontainer();
                        fileBrowser = $("<input type='file' multiple='true' style='display:none;'>");
                        //Add file to Latest File Content
                        fileBrowser.change(function () {
                            if (this.files.length < 1) {
                                return;
                            }
                            $root.find('.vis-email-attachmentContainer').show();
                            AppendFile(this);
                            //if ($root.find('.vis-email-attachmentContainer').is(':visible')) {
                            //    $root.find(".vis-Email-textarea-div").height($root.find(".vis-Email-textarea-div").height() - ($root.find(".vis-email-attachmentContainer").height() + 13));
                            //}
                            if (callingFromOutsideofWindow) {
                                $root.find(".vis-Email-textarea-div").height($root.find('.vis-Email-ContentArea').height() - ($root.find('.vis-form-horizontal').height() + 30));
                            }
                            else {
                                $root.find(".vis-Email-textarea-div").height($root.find('.vis-Email-ContentArea').height() - ($root.find('.vis-form-horizontal').height() + 50));
                            }





                            UploadFiles();
                        });
                    }

                }
                fileBrowser.trigger('click');
            }
            //  attachmentContainerOpen = true;

        };

        function createattachmentcontainer() {
            $uploaddiv = $('<div></div>');


            var divup = $('<div></div>');

            $root.find('.vis-email-attachmentContainer').append(divup);

            if (newdesign == false) {
                $btnupload = $('<a class="vis-attach-ico browse-ico" style="margin-right:8px;float:right;height:30px;width:30px;margin-top:6px;"></a>');
                $uploaddiv.append($btnupload);
                divup.append($btnupload);
            }

            $Attachdiv = $('<div></div>');

            divup.append($Attachdiv);

            dLAContent = $("<div class='vis-attach-content-wrap' style='height:83px;border-bottom:2px solid rgba(var(--v-c-secondary), 1);padding-bottom:0px'>");
            dLAContent.append($('<div style="height:auto;overflow:auto" ></div>'));
            $Attachdiv.append(dLAContent);
            $root.find('.vis-email-attachmentContainer').append(divReadOnly);
            if (newdesign == false) {
                $insertAttachdiv = $('<div></div>');

                divup.append($insertAttachdiv);
                $btnInsert = $('<a href="javascript:void(0)" class="vis-attach-ico uploadall-ico" style="height:30px;width:30px;margin-right: 7px;float:right"></a>');
                $insertAttachdiv.append($btnInsert);


                $btnInsert.on("click", function () {


                    UploadFiles();
                });
            }
        };

        var showProgress = function (show) {
            if (show) {
                divReadOnly.show();
                divProgress.show();
            }
            else {
                divReadOnly.hide();
                divProgress.hide();
            }
        };

        var AppendFile = function (sender) {

            for (var i = 0; i < sender.files.length; i++) {
                file = sender.files[i];
                if (file == undefined) {
                    return;
                }
                if (file.size == 0) {
                    continue;
                }

                for (var itm in lstLatestFiles) {
                    if (file.name == lstLatestFiles[itm].name) {
                        window.setTimeout(function () {
                            VIS.ADialog.info('fileAlreadyAttached');
                        }, 2);
                        return;

                    }
                }
                if (filesforAttachmentforOpenFormat != null) {
                    for (var o = 0; o < filesforAttachmentforOpenFormat.length; o++) {
                        if (file.name == filesforAttachmentforOpenFormat[o]) {
                            window.setTimeout(function () {
                                VIS.ADialog.info('fileAlreadyAttached');
                            }, 2);
                            return;

                        }
                    }
                }


                totalChunks = 0;
                for (var itm in lstLatestFiles) {
                    totalChunks += lstLatestFiles[itm].size;
                }


                if ((totalChunks + file.size) > (chunkSize * 24)) {
                    window.setTimeout(function () {
                        VIS.ADialog.info("MaxFileSize25MB");
                    }, 2);
                    return;

                }

                if (file.size > (chunkSize * 24)) {
                    window.setTimeout(function () {
                        VIS.ADialog.info("MaxFileSize25MB");
                    }, 2);
                    return;

                }



                //var windowWidth = $(window).width();
                ///* latop and large display screen size */
                //if (windowWidth >= 1366) {
                //}
                //if (windowWidth > 1024 && windowWidth < 1366) {
                //}
                //else if (windowWidth >= 1000 && windowWidth <= 1024) {
                //}
                //else if (windowWidth < 1000) {
                //}



                var fileInfo = {};
                var dAWrap = $("<div style='margin-bottom:3px' class='vis-attach-file-wrapla'>");
                dLAContent.children(0).append(dAWrap);

                var dTop = $("<div class='vis-attach-file-top'>");
                var btnRemove = $("<a class='vis-file-close-ico'><i class='vis vis-mark'></i></a>");
                dTop.append(btnRemove);
                dAWrap.append(dTop);
                dLAContent.show();

                btnRemove.on("click", function () {

                    var c = null;
                    var divFInfo = $($(this).parent()).parent();
                    // divFInfo.css("display", "none");
                    var html = divFInfo.html();
                    divFInfo.remove();
                    divFInfo = null;
                    for (var itm in lstLatestFiles) {
                        if ((String(html).indexOf(lstLatestFiles[itm].name)) > -1) {
                            totalChunks = totalChunks - itm.size;
                            lstLatestFiles.splice(itm, 1);
                            break;
                        }
                    }

                    filesforAttachmentforNewAttachment.splice(0, filesforAttachmentforNewAttachment.length);

                    for (var itm in lstLatestFiles) {
                        filesforAttachmentforNewAttachment.push(lstLatestFiles[itm].name);
                    }

                    if (!hasScrollBar()) {
                        $root.find('.vis-attach-content-wrap').height(83);

                        $root.find('.vis-email-attachmentContainer').height(90);
                        if (callingFromOutsideofWindow) {
                            $root.find(".vis-Email-textarea-div").height($('.vis-Email-ContentArea').height() - ($('.vis-form-horizontal').height() + 30));
                        }
                        else {
                            $root.find(".vis-Email-textarea-div").height($('.vis-Email-ContentArea').height() - ($('.vis-form-horizontal').height() + 50));
                        }
                    }

                    if (lstLatestFiles.length == 0) {
                        $root.find('.vis-email-attachmentContainer').hide();
                        $root.find('.vis-email-attachmentContainer').height(90);
                        $root.find('.vis-attach-content-wrap').height(83);
                        if (callingFromOutsideofWindow) {
                            $root.find(".vis-Email-textarea-div").height($('.vis-Email-ContentArea').height() - ($('.vis-form-horizontal').height() + 30));
                        }
                        else {
                            $root.find(".vis-Email-textarea-div").height($('.vis-Email-ContentArea').height() - ($('.vis-form-horizontal').height() + 50));
                        }
                    }
                    currentFile--;
                });

                var dAContent = $("<div class='vis-attach-file-content'>");
                dAWrap.append(dAContent);

                var dIcon = $("<div class='vis-attach-file-icon'>");
                var imgsrc = getImageUrl(file.name);
                dIcon.append($("<img src='" + imgsrc + "'>"));
                dAContent.append(dIcon);

                var dfInfo = $("<div class='vis-attach-file-text' style='margin-bottom:10px'>");
                var shortName = '';
                var lblFName = $("<p>");
                if (file.name.length > 17) {
                    shortName = file.name.toString().substr(0, 17);
                    lblFName.append(shortName);
                    var aFName = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip'>").append('...');
                    var span = $("<span style='width: inherit;'>");
                    span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/email-ccc.png").append("ToolTip Text"));
                    span.append($("<label class='VIS_Pref_Label_Font'>").append(file.name));
                    aFName.append(span);
                    lblFName.append(aFName);
                }
                else {
                    lblFName.append(file.name);
                }
                // dfInfo.append($("<p>").append(shortName));
                dfInfo.append(lblFName);
                dfInfo.append($("<p>").append((Number(file.size) / 1024).toFixed(2) + "KB"));
                dAContent.append(dfInfo);
                fileInfo.Name = file.name;
                fileInfo.Size = file.size;
                lstLatestFiles.push(file);

                if (hasScrollBar()) {
                    if ($root.find('.vis-email-attachmentContainer').height() == 90) {
                        $root.find('.vis-email-attachmentContainer').height(155);
                        $root.find('.vis-attach-content-wrap').height(145);
                    }
                }

            }
        };

        function hasScrollBar() {


            if (dLAContent.children(0).height() < 75) {

                return false;
            }
            else {
                return true;
            }


            ////if ($root.find('.attach-content-wrap')[0].scrollHeight > $root.find('.attach-content-wrap').height()) {
            //if ($root.find('.attach-content-wrap')[0].scrollHeight > dLAContent.children(0).height()) {

            //    return true;
            //}
            //else {
            //    return false;
            //}
        };

        var getImageUrl = function (fileName) {

            if (fileName == null || fileName.length == 0) {
                return '';
            }
            var index = fileName.lastIndexOf('.');
            var imgUrl = VIS.Application.contextUrl + "Areas/VIS/Images/";
            if (index < 1) {
                //return Default Image
                imgUrl += 'defult.png';
                return imgUrl;
            }
            var ext = fileName.substr(index, fileName.length - index);
            if (ext == '.xlsx' || ext == '.xls') {
                imgUrl += 'excel.png';
            }
            else if (ext == '.html' || ext == '.htm') {
                imgUrl += 'html.png';
            }
            else if (ext == '.pdf') {
                imgUrl += 'pdf.png';
            }
            else if (ext == '.ppt') {
                imgUrl += 'ppt.png';
            }
            else if (ext == '.txt') {
                imgUrl += 'text.png';
            }
            else if (ext == '.docx' || ext == '.doc') {
                imgUrl += 'word.png';
            }
            else if (ext == '.xml') {
                imgUrl += 'xml.png';
            }
            else {
                imgUrl += 'defult.png';
            }
            return imgUrl;
        };

        var AppendFileOldRegion = function (file, size, index) {
            //var dAWrap = $("<div class='attach-file-wrapla'>");
            //dLAContent.append(dAWrap);

            //var dTop = $("<div class='attach-file-top'>");
            //var btnRemove = $("<a class='file-close-ico'>");
            //dTop.append(btnRemove);
            //dAWrap.append(dTop);

            //btnRemove.on("click", function () {
            //    
            //    var c = null;
            //    var divFInfo = $($(this).parent()).parent();
            //    divFInfo.css("display", "none");
            //    var html = divFInfo.html();
            //    divFInfo.empty();
            //    divFInfo = null;
            //    for (var itm in filesforAttachmentforOpenFormat) {
            //        if ((String(html).indexOf(filesforAttachmentforOpenFormat[itm])) > -1) {
            //            filesforAttachmentforOpenFormat.splice(itm, 1);
            //            break;
            //        }
            //    }
            //});

            //filesforAttachmentforOpenFormat.push(file);

            var dAWrap = $("<div class='vis-attach-file-wrapla'>");
            dLAContent.append(dAWrap);
            var dTop = $("<div class='vis-attach-file-top'>");
            var btnRemove = $("<a class='vis-file-close-ico'>");
            dTop.append(btnRemove);
            dAWrap.append(dTop);

            btnRemove.on("click", function () {

                var c = null;
                var divFInfo = $($(this).parent()).parent();
                divFInfo.css("display", "none");
                var html = divFInfo.html();
                divFInfo.empty();
                divFInfo = null;
                for (var itm in filesforAttachmentforOpenFormat) {
                    if ((String(html).indexOf(filesforAttachmentforOpenFormat[itm])) > -1) {
                        filesforAttachmentforOpenFormat.splice(itm, 1);
                        break;
                    }
                }
            });


            var oldFile = {};
            var dAContent = $("<div class='vis-attach-file-content'>");
            dAWrap.append(dAContent);

            var dIcon = $("<div class='vis-attach-file-icon'>");
            dIcon.append($("<img src='" + getImageUrl(file) + "'>"));
            dAContent.append(dIcon);

            var dfInfo = $("<div class='vis-attach-file-text'  style='margin-bottom:10px'>");
            var shortName = '';
            var lblFName = $("<p>");
            if (file.length > 17) {
                shortName = file.toString().substr(0, 17);
                lblFName.append(shortName);
                var aFName = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip'>").append('...');
                var span = $("<span style='width: inherit;'>");
                span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/email-ccc.png").append("ToolTip Text"));
                span.append($("<label class='VIS_Pref_Label_Font'>").append(file));
                aFName.append(span);
                lblFName.append(aFName);
            }
            else {
                lblFName.append(file);
            }
            dfInfo.append(lblFName);


            dfInfo.append($("<p>").append((Number(size) / 1024).toFixed(2) + "KB"));
            dAContent.append(dfInfo);

            oldFile.Name = file;
            // oldFile.Size = file.Size;
            oldFile.Line_ID = 0;
            oldFile.HaveSource = false;
            oldFile.Source = null;
            oldFile.IsDeleted = false;
            oldFiles.push(oldFile);

        };

        var UploadFiles = function () {


            var fileInfo = null;
            //folder = Date.now().toString();
            //$bsyDiv[0].style.visibility = "visible";


            var tcSingleFile = 0;
            var currentChunk = 0;


            // filesforAttachment.splice(0, filesforAttachment.length);
            var totalSize = 0;
            totalChunks = 0;
            for (var itm in lstLatestFiles) {

                tcSingleFile = parseInt(lstLatestFiles[itm].size / chunkSize);
                if (file.size % chunkSize > 0) {
                    tcSingleFile++;
                }
                totalChunks += tcSingleFile;
                totalSize += lstLatestFiles[itm].size;
                fileInfo = {};
                filesforAttachmentforNewAttachment.push(lstLatestFiles[itm].name);
                fileInfo.Name = lstLatestFiles[itm].name;
                fileInfo.Size = lstLatestFiles[itm].size;
                filesInfo.push(fileInfo);
            }


            if (totalSize > (chunkSize * 24)) {
                return;
            }

            showProgress(true);

            TransferFile();

        };

        var TransferFile = function () {
            if (lstLatestFiles == null || lstLatestFiles.length == 0) {
                showProgress(false);
                $root.find('.vis-email-attachmentContainer').hide();
                $root.find(".vis-Email-textarea-div").height($root.find(".vis-Email-textarea-div").height() + ($root.find(".vis-email-attachmentContainer").height() + 13));
                window.setTimeout(function () {
                    VIS.ADialog.info("NothingToUpload");
                    return;
                }, 2);
            }
            if (currentFile >= lstLatestFiles.length) {
                showProgress(false);
                return;
            }

            var xhr = new XMLHttpRequest();
            var fd = new FormData();
            fd.append("file", lstLatestFiles[currentFile].slice(currentFileChunkNo * chunkSize, currentFileChunkNo * chunkSize + Number(chunkSize)));
            xhr.open("POST", VIS.Application.contextUrl + "Email/SaveAttachmentinTemp?filename=" + lstLatestFiles[currentFile].name + "&folderKey=" + folder, false);
            xhr.send(fd);
            currentchunk++;
            currentFileChunkNo++;
            var totalFileChunk = parseInt(lstLatestFiles[currentFile].size / chunkSize);
            if (lstLatestFiles[currentFile].size % chunkSize > 0) {
                totalFileChunk++;
            }

            if (currentFileChunkNo == totalFileChunk) {
                currentFile++;
                currentFileChunkNo = 0;
            }

            if (currentchunk <= totalChunks) {
                setProgressValue(parseInt((currentchunk / totalChunks) * 100));
            }
            window.setTimeout(function () {
                TransferFile();
            }, 2);

        };

        var setProgressValue = function (value) {
            divGreen.css('width', value + '%');
            lblpercentage.empty();
            lblpercentage.append(value + '%');
        };

        function dynamicDisplaychange(e) {
            if (this.checked) {
                if ($leftDiv.width() != 40) {
                    $leftDiv.find('ul').show();
                }
                //in ase of dynmic mail, to, bccc and cc will be enabled only for single record.
                //otherwise disable them.
                if (isEmail == true && _curGC.singleRow == false && rowsSource.length > 1) {
                    $to.val('');
                    $to.prop('disabled', true);
                    $cc.val('');
                    $cc.prop('disabled', true);
                    $bcc.val('');
                    $bcc.prop('disabled', true);
                }
                flagImg = true;
            }
            else {
                if ($leftDiv.width() != 40) {
                    shrinkLeftDiv();
                }
                if (isEmail == true && _curGC.singleRow == false && rowsSource.length > 1 && $chkBSendPFasAtt.prop("checked") == false) {
                    $to.prop('disabled', false);
                    $cc.prop('disabled', false);
                    $bcc.prop('disabled', false);
                }
                flagImg = false;

                $toggleAction.css("border-bottom", "none");
                if (!callingFromOutsideofWindow) {
                    $divlbNav.css("display", "none");
                }
            }

        };

        function attachDynamicPF(e) {

            if (this.checked) {
                $cmbPfFiletype.parent().show();
                //in ase of dynmic mail, to, bccc and cc will be enabled only for single record.
                //otherwise disable them.

                if (isEmail == true && _curGC.singleRow == false && rowsSource.length > 1) {
                    $to.val('');
                    $to.prop('disabled', true);
                    $cc.val('');
                    $cc.prop('disabled', true);
                    $bcc.val('');
                    $bcc.prop('disabled', true);
                }
            }
            else {
                $cmbPfFiletype.parent().hide();
                if (isEmail == true && _curGC.singleRow == false && rowsSource.length > 1 && $dynamicDisplay.prop("checked") == false) {
                    $to.prop('disabled', false);
                    $cc.prop('disabled', false);
                    $bcc.prop('disabled', false);
                }
            }


        };

        function addPrintOption() {


            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GetReportFileTypes",
                datatype: "json",
                type: "post",
                cache: false,
                data: {
                    AD_Process_ID: _curtab.getAD_Process_ID()
                },
                success: function (data) {
                    if (data == null) {
                        return;
                    }
                    var d = jQuery.parseJSON(data);

                    if (d.length == 0) {

                        return;
                    }
                    for (var i = 0; i < d.length; i++) {
                        $cmbPfFiletype.append($("<option value=" + d[i].Key + ">" + d[i].Name + "</option>"));
                    }
                }
            });
        };

        function insertSelectedField(e) {
            $textAreakeno.paste('@@' + $(this).text() + '@@');
        };

        function preview(e) {
            var html = $textAreakeno.value();
            var finalhtmls = '';
            if (_curGC.singleRow == false || rowsSource.length > 0) {
                finalhtmls = parseHtml1(html, 0);
            }
            else {
                finalhtmls = parseHtml2(html)
            }
            var $preDiv = $('<div></div>');
            $preDiv.append(finalhtmls);

            $($preDiv.find('table')).css({ "width": '100%', "border": "1px solid Black" });
            $($preDiv.find('td')).css({ "border": "1px solid Black" });

            var chp = new VIS.ChildDialog();
            chp.setHeight(420);
            chp.setWidth(850);
            chp.setTitle(VIS.Msg.getMsg("Preview"));
            chp.setModal(true);
            chp.setContent($preDiv);
            chp.show();
            chp.hidebuttons();
            //var $preDiv = $('<div></div>');
            //$preDiv.append(finalhtmls);
            //$preDiv.dialog();

        };

        function send(e) {
            var subj = $subject.val();
            var body = $textAreakeno.value();

            var canExecute = true;

            if ((subj == null || subj == "" || subj.trim().length == 0) && (body == null || body == "" || body.trim().length == 0)) {
                canExecute = false;
                VIS.ADialog.confirm("SendWithoutSubjectandBody", true, "", "Confirm", function (result) {
                    if (result) {
                        sendCallback(e, subj, body);
                    }
                });

                return;
                subj = " ";    //done bcoz mail is not being sent if subject length is 0....
            }
            else {
                if (subj == null || subj == "" || subj.trim().length == 0) {
                    canExecute = false;
                    VIS.ADialog.confirm("SendWithoutSubject", true, "", "Confirm", function (result) {
                        if (result) {
                            sendCallback(e, subj, body);
                        }
                    });
                    return;
                    subj = " ";     //done bcoz mail is not being sent if subject length is 0....
                }
                if (body == null || body == "" || body.trim().length == 0) {
                    canExecute = false;
                    //if (!VIS.ADialog.ask(VIS.Msg.getMsg("SendWithoutBody"))) {
                    //    return;
                    //}
                    VIS.ADialog.confirm("SendWithoutBody", true, "", "Confirm", function (result) {
                        if (result) {
                            sendCallback(e, subj, body);
                        }
                    });
                    return;

                }
            }

            if (canExecute) {
                sendCallback(e, subj, body);
            }
        };

        function sendCallback(e, subj, body) {
            hsaValidinputmails = false;

            var toval = $to.val();
            var bccVal = $bcc.val();
            var ccval = $cc.val();


            if (toval.trim().length > 0) {
                if (toval.contains(';')) {
                    var arraybcc = toval.split(';');
                    for (var i = 0; i < arraybcc.length; i++) {
                        if (arraybcc[i] != null && arraybcc[i] != "") {
                            if (!validateEmail(arraybcc[i].trim())) {
                                window.setTimeout(function () {
                                    VIS.ADialog.info("NotValidEmail", true, ":" + arraybcc[i]);

                                }, 2);
                                return;
                            }
                            else {
                                hsaValidinputmails = true;
                            }
                        }
                    }
                }
                else {
                    if (!validateEmail(toval.trim())) {
                        window.setTimeout(function () {
                            VIS.ADialog.info("NotValidEmail", true, ":" + toval);

                        }, 2);
                        return;
                    }
                    else {
                        hsaValidinputmails = true;
                    }
                }
            }

            if (ccval.trim().length > 0) {
                if (ccval.contains(';')) {
                    var arraybcc = ccval.split(';');
                    for (var i = 0; i < arraybcc.length; i++) {
                        if (arraybcc[i] != null && arraybcc[i] != "") {
                            if (!validateEmail(arraybcc[i].trim())) {
                                window.setTimeout(function () {
                                    VIS.ADialog.info("NotValidEmail", true, ":" + arraybcc[i]);

                                }, 2);
                                return;
                            }
                            else {
                                hsaValidinputmails = true;
                            }
                        }
                    }
                }
                else {
                    if (!validateEmail(ccval.trim())) {
                        window.setTimeout(function () {
                            VIS.ADialog.info("NotValidEmail", true, ":" + ccval);
                        }, 2);
                        return;
                    }
                    else {
                        hsaValidinputmails = true;
                    }
                }
            }


            if (bccVal.trim().length > 0) {
                if (bccVal.contains(';')) {
                    var arraybcc = bccVal.split(';');
                    for (var i = 0; i < arraybcc.length; i++) {
                        if (arraybcc[i] != null && arraybcc[i] != "") {
                            if (!validateEmail(arraybcc[i].trim())) {
                                window.setTimeout(function () {
                                    VIS.ADialog.info("NotValidEmail", true, ":" + arraybcc[i]);

                                }, 2);
                                return;
                            }
                            else {
                                hsaValidinputmails = true;
                            }
                        }
                    }
                }
                else {
                    if (!validateEmail(bccVal.trim())) {
                        window.setTimeout(function () {
                            VIS.ADialog.info("NotValidEmail", true, ":" + bccVal);

                        }, 2);
                        return;
                    }
                    else {
                        hsaValidinputmails = true;
                    }
                }
            }


            //for static mails
            var mail = [];

            if ($dynamicDisplay.prop("checked") == false && $chkBSendPFasAtt.prop("checked") == false) {
                var hasMailID = false;
                var mailInfo = {};



                mailInfo.To = toval;
                mailInfo.Cc = ccval;

                mailInfo.Bcc = [];

                if (bccVal.contains(';')) {
                    var arraybcc = bccVal.split(';');
                    for (var i = 0; i < arraybcc.length; i++) {
                        if (arraybcc[i] != null && arraybcc[i] != "") {
                            mailInfo.Bcc.push(arraybcc[i]);
                        }
                    }
                }
                else {
                    if (bccVal != null && bccVal != "") {
                        mailInfo.Bcc.push(bccVal);
                    }
                }


                if ($bccChkList != null && $bccChkList != undefined) {

                    selectedValues = [];
                    $("[id*=CheckBoxList1]:checked").each(function () {
                        selectedValues.push($(this).val());
                    });

                    if (selectedValues.length > 0) {
                        for (var l = 0; l < selectedValues.length; l++) {

                            if (!validateEmail(selectedValues[l])) {
                                window.setTimeout(function () {
                                    VIS.ADialog.info("NotValidEmail", true, ":" + selectedValues[l]);

                                }, 2);
                                return;
                            }
                            hasMailID = true;
                            mailInfo.Bcc.push(selectedValues[l]);
                        }
                    }
                }



                mailInfo.Subject = subj;

                // mailInfo.Body = $textAreakeno.value();
                mailInfo.TableID = currentTable_ID;
                mailInfo.Recordids = '';

                if (rowsSingleView != null) {
                    if (_curGC.singleRow == false || rowsSource.length > 0) {
                        var recsIDs = '';
                        for (var r = 0; r < rowsSource.length; r++) {
                            recsIDs += rowsSource[r][_curtab.keyColumnName.toLower()] + ",";
                        }
                        mailInfo.Recordids = recsIDs;
                    }
                    else {
                        if (Record_ID > 0) {
                            mailInfo.Recordids = Record_ID;
                        }
                        else {
                            if (_curGC.singleRow == true) {
                                mailInfo.Recordids = _curtab.getField(_curtab.keyColumnName.toLower()).value;
                            }
                            else {
                                mailInfo.Recordids = rowsSingleView[_curtab.keyColumnName.toLower()];
                            }
                        }
                    }
                }
                else {
                    if (Record_ID > 0) {
                        mailInfo.Recordids = Record_ID;
                    }
                }
                mailInfo.AttachmentFolder = folder;

                if (!hasMailID) {
                    if (!hsaValidinputmails) {
                        window.setTimeout(function () {
                            VIS.ADialog.info("NoEmailAddressFound");

                        }, 2);
                        return;
                    }
                }

                mail.push(mailInfo);
            }
            //for dynamic mails
            else {
                selectedValues = [];
                var selectedRecs = [];
                var currentRec = [];
                $("[id*=CheckBoxList1]:checked").each(function () {
                    selectedValues.push($(this).val());
                    selectedRecs.push($(this).data('recid'));
                    currentRec.push($(this).data('currentrec'));
                });

                if (selectedValues.length > 0) {
                    for (var l = 0; l < selectedValues.length; l++) {
                        if (!validateEmail(selectedValues[l])) {
                            window.setTimeout(function () {
                                VIS.ADialog.info("NotValidEmail", true, ":" + selectedValues[l]);

                            }, 2);
                            return;
                        }
                    }

                    for (var z = 0; z < selectedValues.length; z++) {

                        if (_curGC.singleRow == true && rowsSource.length == 0) {
                            var mailRes = createDynmicmails(rowsSingleView, 0);
                            if (mailRes == null || mailRes == undefined) {
                                return;
                            }
                            mail.push(mailRes);
                        }
                        else {
                            for (var r = 0; r < rowsSource.length; r++) {
                                if (selectedRecs[z] == rowsSource[r][_curtab.getKeyColumnName().toLower()]) {
                                    var mailRes = null;
                                    if (currentRec[z] == "N") {
                                        mailRes = createDynmicmails(rowsSource[r], r, selectedValues[z])
                                    }
                                    else {
                                        mailRes = createDynmicmails(rowsSource[r], r)
                                    }

                                    if (mailRes == null || mailRes == undefined) {
                                        continue;
                                    }
                                    mail.push(mailRes);
                                }
                            }
                        }
                    }
                }
                else if (hsaValidinputmails) {
                    if (_curGC.singleRow == true && rowsSource.length == 0) {
                        var mailRes = createDynmicmails(rowsSingleView, 0);
                        if (mailRes == null || mailRes == undefined) {
                            return;
                        }
                        mail.push(mailRes);
                    }
                    else {
                        for (var r = 0; r < rowsSource.length; r++) {
                            var mailRes = createDynmicmails(rowsSource[r], r)
                            if (mailRes == null || mailRes == undefined) {
                                continue;
                            }
                            mail.push(mailRes);
                        }
                    }
                }

                //if (_curGC.singleRow == true && rowsSource.length == 0) {
                //    var mailRes = createDynmicmails(rowsSingleView, 0);
                //    if (mailRes == null || mailRes == undefined) {
                //        return;
                //    }
                //    mail.push(mailRes);
                //}
                //else {
                //    for (var r = 0; r < rowsSource.length; r++) {

                //        var mailRes = createDynmicmails(rowsSource[r], r)
                //        if (mailRes == null || mailRes == undefined) {
                //            continue;
                //        }
                //        mail.push(mailRes);
                //    }
                //}
            }
            if (mail.length == 0) {
                window.setTimeout(function () {
                    VIS.ADialog.info("NoEmailAddressFound");

                }, 2);
                return;
            }

            var mails = JSON.stringify(mail);
            var wantNotification = false;

            var pfFiletype = 'X';// no Action
            if ($chkBSendPFasAtt.prop("checked")) {
                pfFiletype = $cmbPfFiletype.val();
            }


            VIS.ADialog.confirm("WantAlertMessage", true, "", "Confirm", function (result) {
                if (result) {
                    wantNotification = true;
                }


                $bsyDiv[0].style.visibility = "visible";
                var datainit = {
                    mails: VIS.Utility.encodeText(mails), AD_User_ID: ctx.getAD_User_ID(), AD_Client_ID: ctx.getAD_Client_ID(),
                    AD_Org_ID: ctx.getAD_Org_ID(), attachment_ID: attachmentID, fileNamesFornNewAttach: JSON.stringify(filesforAttachmentforNewAttachment),
                    fileNamesForopenFormat: JSON.stringify(filesforAttachmentforOpenFormat), mailFormat: VIS.Utility.encodeText($textAreakeno.value()),
                    notify: wantNotification, strDocAttach: VIS.context.getContext("DocumentAttachViaEmail_" + self.windowNo),
                    /** tab panel- null check ** Dt: 05/07/2021 ** Modified By: Kumar **/
                    AD_Process_ID: ((_curtab != null) ? _curtab.getAD_Process_ID() : 0), printformatfileType: pfFiletype
                };
                $.ajax({
                    url: VIS.Application.contextUrl + "Email/SendMail",
                    data: datainit,
                    datatype: "json",
                    type: "post",
                    cache: false,
                    async: true,
                    success: function (data) {
                        $bsyDiv[0].style.visibility = "hidden";
                        var res = JSON.parse(data);
                        if (res == "") {
                            self.dispose();
                            self = null;
                            if (doc != null) {
                                doc.dispose();
                            }
                            doc = null;
                        }
                        else {
                            window.setTimeout(function () {

                                VIS.ADialog.info(res);
                                if (res.contains("MailSent")) {
                                    VIS.context.setContext("DocumentAttachViaEmail_" + self.windowNo, "");
                                }
                            }, 2);
                        }
                    }
                });


            });




        };

        function validateEmail(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }

        function saveAttachment(e) {
            debugger;
            var subj = $subject.val();

            if (subj == null || subj == "" || subj.trim().length == 0) {
                VIS.ADialog.info("PleaseAddSubject");
                return;
            }

            var html = $textAreakeno.value();
            if (html == null || html == "" || html == undefined || html.trim().length < 1) {
                return;
            }
            var finalhtmls = '';
            var values = {};

            rowsSource = _curGC.getSelectedRows();

            // this is data source for single View Records
            rowsSingleView = _curtab.getRecords()[_curtab.getCurrentRow()];


            if (_curGC.singleRow == false || rowsSource.length > 0) {
                for (var r = 0; r < rowsSource.length; r++) {
                    values[rowsSource[r][_curtab.keyColumnName.toLower()]] = listofSelectedItems1(html, r);
                }
            }
            else {
                if (Record_ID > 0) {
                    values[Record_ID] = listofSelectedItems2(html);
                }
                else {
                    values[rowsSingleView[_curtab.keyColumnName.toLower()]] = listofSelectedItems2(html);
                }
            }

            $bsyDiv[0].style.visibility = "visible";

            $.ajax({
                url: VIS.Application.contextUrl + "Email/SaveAttachment",
                datatype: "json",
                type: "post",
                cache: false,
                data: { subject: subj, AD_Table_ID: currentTable_ID, html: VIS.Utility.encodeText(html), values: JSON.stringify(values) },
                success: function (data) {
                    $bsyDiv[0].style.visibility = "hidden";
                    VIS.ADialog.info("Saved");
                    //var result = JSON.parse(data);
                    //htt = result;
                    //var datauri = 'data:application/pdf;base64,' + htt;

                    //window.open(datauri);
                    //return;
                },
                error: function (err) {
                    //A potentially dangerous Request.Form value was detected from the client
                    //VIS.ADialog.error("PotentiallyDangerous");
                    console.log(err);
                }
            });

        };

        function pdf(e) {
            var html = $textAreakeno.value();
            if (html == null || html == "" || html == undefined || html.trim().length < 1) {
                return;
            }
            var finalhtmls = '';
            var values = [];

            rowsSource = _curGC.getSelectedRows();

            // this is data source for single View Records
            rowsSingleView = _curtab.getRecords()[_curtab.getCurrentRow()];


            if (_curGC.singleRow == false || rowsSource.length > 0) {

                for (var r = 0; r < rowsSource.length; r++) {
                    //  finalhtmls += parseHtml1(html, r) + '~';

                    values.push(listofSelectedItems1(html, r));
                    //doc.fromHTML(finalhtmls, 15, 15, {
                    //    'elementHandlers': {}
                    //});
                    //doc.addPage();
                }


            }
            else {

                values.push(listofSelectedItems2(html));
                //finalhtmls = parseHtml2(html);
                //doc.fromHTML(finalhtmls, 15, 15, {
                //    'elementHandlers': {}
                //});
            }



            //newLetters = [];
            //newLetters.push({ Body: finalhtmls, Subject: "asdadsaDSA" });



            $bsyDiv[0].style.visibility = "visible";

            //var mywindow = window.open();
            //mywindow.document.write('<html><head>');
            //mywindow.document.write('</head><body >');

            //mywindow.document.write(html);

            //mywindow.document.write('</body></html>');
            //mywindow.print();

            $.ajax({
                url: VIS.Application.contextUrl + "Email/InsertAttachmentText",
                datatype: "json",
                type: "post",
                cache: false,
                data: { html: VIS.Utility.encodeText(html), values: JSON.stringify(values) },
                success: function (data) {
                    $bsyDiv[0].style.visibility = "hidden";
                    var result = JSON.parse(data);
                    htt = result;
                    // var datauri = 'data:application/pdf;charset=utf-8;base64,' + htt;
                    var datauri = htt;


                    //var iframe = "<iframe width='100%' height='100%' style='direction:rtl'  src='" + datauri + "'></iframe>"

                    //var x = window.open();
                    //x.document.open();
                    //x.document.write(iframe);
                    //x.document.close();

                    window.open(VIS.Application.contextUrl + datauri);


                    //var mywindow = window.open();
                    //mywindow.document.write('<html dir="rtl"><head>');
                    //mywindow.document.write('</head><body >');
                    //mywindow.document.write(htt);

                    //mywindow.document.write('</body></html>');
                    return;
                }
            });
        };
        var isCallNew = 0;
        function createNewLetter(e) {
            onOkOpentn = false;
            isCallNew = 0;
            if ($SubjectTextChange == 1) {
                if ($textAreakeno.value() != "" || $subject.val() != "") {
                    VIS.ADialog.confirm("DoYouWantToSave", true, "", "Confirm", function (result) {
                        if (!result) {
                            $subject.val("");
                            $textAreakeno.value('');
                            selectedTemplateID = 0;
                            $btnHdrSave.css("opacity", "1");
                            $SubjectTextChange = 0;
                        }
                        else {
                            save(e);
                            $SubjectTextChange = 0;
                            isCallNew = 1;
                        }
                    });
                }
            }
            else {
                $subject.val("");
                $textAreakeno.value('');
                selectedTemplateID = 0;
                $btnHdrSave.css("opacity", "1");
                $SubjectTextChange = 0;
            }
        };



        var openFormatClientID = ctx.getAD_Client_ID();
        var onOkOpentn = false;
        var getMailFormat = null;
        var openCallAgain = 0;
        function open(e) {
            var openDailogshow = 0;
            openCallAgain = 0;
            var mailformat = new VIS.Openmailformat(_curGC.gTab.getAD_Window_ID(), isEmail);
            getMailFormat = mailformat;

            if (!isEmail) {
                isCallNew = 0;
                if ($textAreakeno.value() != "" && $subject.val() != "" && $SubjectTextChange == 1) {
                    VIS.ADialog.confirm("DoYouWantToSave", true, "", "Confirm", function (result) {
                        if (!result) {
                            //$subject.val("");
                            //$textAreakeno.value('');
                            selectedTemplateID = 0;
                            $btnHdrSave.css("opacity", "1");
                            $SubjectTextChange = 0;
                            mailformat.show();
                        }
                        else {
                            save(e);
                            openCallAgain = 1;
                            onOkOpentn = true;
                        }
                    });
                    openDailogshow = 1;
                }
            }

            if (openDailogshow == 0) {
                mailformat.show();
            }
            mailformat.onClose = function () {
                $SubjectTextChange = 0;
                var selectedRow = mailformat.getSelectedRow();
                if (selectedRow != null && selectedRow != undefined) {
                    // changes done by Bharat on 27 Sep 2018 to habdle issue of open text format
                    selectedTemplateID = selectedRow.AD_TEXTTEMPLATE_ID;
                    openFormatClientID = selectedRow.AD_CLIENT_ID;
                    $subject.val(selectedRow.SUBJECT);

                    if (openFormatClientID != ctx.getAD_Client_ID()) {

                        $btnHdrSave.css("opacity", "0.4");
                    }
                    else {

                        $btnHdrSave.css("opacity", "1");
                    }


                    if (selectedRow.ISDYNAMICCONTENT == "Y") {
                        $dynamicDisplay.prop("checked", false).trigger("click");

                    }
                    else {
                        $dynamicDisplay.prop("checked", true).trigger("click");

                    }

                    if (selectedRow.AD_Window_ID != null && selectedRow.AD_Window_ID != undefined && selectedRow.AD_Window_ID > 0) {
                        saveForAllWindows = false;
                    }
                    else {
                        saveForAllWindows = true;
                    }

                    formatName = selectedRow.NAME;
                    $textAreakeno.value(selectedRow.MAILTEXT);
                }
                onOkOpentn = false;
            };
        };

        function saveAs(e) {
            var text = $textAreakeno.value();
            if (text != null && text.trim() != "" && text != undefined) {
                var newformat = new VIS.Newmailformat(isEmail);
                newformat.show();
                newformat.onClose = function () {


                    formatName = newformat.getName();
                    saveForAllWindows = newformat.saveForAll();

                    saveFormat(0);
                };
            }
        };

        function save(e) {

            if (openFormatClientID != ctx.getAD_Client_ID()) {
                return;
            }

            var text = $textAreakeno.value();
            if (text != null && text != "" && text != undefined) {
                if (selectedTemplateID > 0) {
                    saveFormat(selectedTemplateID);
                }
                else {
                    saveAs();
                }
            }
        };


        //Refresh Email IDs..
        function refresh(e) {
            $bccChkList.empty();        //Create already added mail address.

            loadEmails(true); // Load emails

            loadFields(true); // Load Feilds and their bpartner's email IDS....

        };

        function cancel(e) {
            self.dispose();
            self = null;
        };

        function saveFormat(id) {
            var inputData = {
                id: id,
                AD_Client_ID: ctx.getAD_Client_ID(),
                AD_Org_ID: ctx.getAD_Org_ID(),
                name: VIS.Utility.encodeText(formatName),
                isDynamic: $dynamicDisplay.prop("checked"),
                subject: VIS.Utility.encodeText($subject.val()),
                text: VIS.Utility.encodeText($textAreakeno.value()),
                saveforAll: saveForAllWindows,
                AD_Window_ID: _curtab.getAD_Window_ID(),
                folderName: folder,
                attachmentID: attachmentID
            };

            $.ajax({
                type: 'POST',
                url: VIS.Application.contextUrl + "Email/SaveFormats",
                data: inputData,
                success: function (data) {
                    selectedTemplateID = parseInt(data);
                    openFormatClientID = ctx.getAD_Client_ID();
                    $btnHdrSave.css("opacity", "1");
                    $SubjectTextChange = 0;

                    if (!isEmail) {
                        if (isCallNew == 1) {
                            $subject.val("");
                            $textAreakeno.value("");

                        }
                        if (onOkOpentn == true) {
                            window.setTimeout(function () {
                                if (openCallAgain == 1) {
                                    open();
                                    onOkOpentn = false;
                                }
                            }, 200);
                        }
                    }
                }
            });

        };


        /* Will be visible when user click on link of CC & Bcc*/
        function showBccpanel(e) {
            if ($bcc.is(':visible')) {
                $bcc.hide();
                $cc.hide();
                //   $root.find(".vis-Email-textarea-div").height($root.find(".vis-Email-textarea-div").height() + 85);


                if (!$root.find('.vis-email-attachmentContainer').is(":visible")) {

                    $root.find(".vis-Email-textarea-div").height($root.height() - 180);
                }
                else {
                    $root.find(".vis-Email-textarea-div").height($root.height() - 365);
                }




            }
            else {
                $bcc.show();
                $cc.show();
                // $root.find(".vis-Email-textarea-div").height($root.height() - 265);


                if (!$root.find('.vis-email-attachmentContainer').is(":visible")) {

                    $root.find(".vis-Email-textarea-div").height($root.height() - 255);
                }
                else {
                    $root.find(".vis-Email-textarea-div").height($root.height() - 440);
                }
            }
        };

        function createDynmicmails(source, i, email) {
            var hasMail = false;

            var mailInfo = {};
            mailInfo.To = $to.val();
            mailInfo.Cc = $cc.val();
            mailInfo.Bcc = [];
            if ($bcc.val().contains(';')) {
                var arraybcc = $bcc.val().split(';');
                for (var i = 0; i < arraybcc.length; i++) {
                    if (arraybcc[i] != null && arraybcc[i] != "") {
                        mailInfo.Bcc.push(arraybcc[i]);
                    }
                }
            }
            else {
                if ($bcc.val() != null && $bcc.val() != "") {
                    mailInfo.Bcc.push($bcc.val());
                }
            }

            if (email) {
                if (!validateEmail(email)) {
                    window.setTimeout(function () {
                        VIS.ADialog.info("NotValidEmail", true, ":" + email);
                        return;
                    }, 2);
                }
                mailInfo.Bcc.push(email);
                hasMail = true;
            }
            else if ($bccChkList != null && $bccChkList != undefined) {
                if (Object.keys(source).indexOf("email") > -1 && selectedValues.indexOf(source['email']) > -1) {
                    //var lis = $bccChkList.children('li');
                    //if (lis.length > 0) {
                    //    for (var l = 0; l < lis.length; l++) {
                    //        mailInfo.bcc.push($(lis[l]).data('email'));
                    //    }
                    //}
                    if (!validateEmail(source['email'])) {
                        window.setTimeout(function () {
                            VIS.ADialog.info("NotValidEmail", true, ":" + source['email']);
                            return;
                        }, 2);
                    }
                    mailInfo.Bcc.push(source['email']);
                    hasMail = true;
                }
                else if (!_curGC.singleRow == false && rowsSource.length == 1) {

                }
                else {
                    if (!_curGC.singleRow == false || rowsSource.length > 1) {
                        return;
                    }
                }
            }

            var sssub = $subject.val();
            if (sssub.trim().length == 0) {
                sssub = "  ";
            }
            mailInfo.Subject = sssub;  //done bcoz mail is not being sent if subject length is 0....
            mailInfo.TableID = currentTable_ID;
            if (_curGC.singleRow == true && rowsSource.length == 0) {
                mailInfo.Recordids = Record_ID;
            }
            else {
                mailInfo.Recordids = source[_curtab.keyColumnName.toLower()];
            }
            if (_curGC.singleRow == false) {
                mailInfo.Body = listofSelectedItems1($textAreakeno.value(), i);
            }
            else {
                mailInfo.Body = listofSelectedItems2($textAreakeno.value());
            }

            mailInfo.AttachmentFolder = folder;

            if (_curGC.singleRow == true || rowsSource.length == 1) {
                if (!hasMail) {
                    if (!hsaValidinputmails) {
                        return;
                    }
                }
            }
            return mailInfo;
        };

        ///For Multi Row Selection
        function listofSelectedItems1(html, i) {
            var copyhtml = html;
            var retValues = {};
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (columnName != undefined && columnName != null) {
                    if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                        if (rowsSource[i][columnName.toLower()] != null && rowsSource[i][columnName.toLower()] != undefined) {
                            fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSource[i][columnName.toLower()]);
                        }
                    }
                    else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                        var displayType = _curtab.getField(columnName).getDisplayType()
                        fieldValue = _curtab.getField(columnName).value;

                        if (VIS.DisplayType.Date == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleDateString();
                        }
                        else if (VIS.DisplayType.Time == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleTimeString();
                        }
                        else {
                            fieldValue = new Date(fieldValue).toLocaleString();
                        }

                        //toLocaleDateString
                        //toLocaleTimeString
                    }
                    else {
                        fieldValue = rowsSource[i][columnName.toLower()];
                    }
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                retValues['@@' + fieldname + '@@'] = fieldValue;
            }
            return retValues;
        };

        ///For Single View Selection
        function listofSelectedItems2(html) {
            var copyhtml = html;
            var retValues = {};
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (columnName != undefined && columnName != null) {
                    if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                        if (rowsSource.length > 0 && rowsSource[0][columnName.toLower()] != null && rowsSource[0][columnName.toLower()] != undefined) {
                            fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSource[0][columnName.toLower()]);
                        }
                        else if (rowsSingleView[columnName.toLower()] != null && rowsSingleView[columnName.toLower()] != undefined) {
                            fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSingleView[columnName.toLower()]);
                        }
                    }
                    else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {
                        var displayType = _curtab.getField(columnName).getDisplayType();
                        fieldValue = _curtab.getField(columnName).value;

                        if (VIS.DisplayType.Date == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleDateString();
                        }
                        else if (VIS.DisplayType.Time == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleTimeString();
                        }
                        else {
                            fieldValue = new Date(fieldValue).toLocaleString();
                        }

                    }
                    else {
                        if (_curGC.singleRow == true && rowsSource.length == 0) {
                            fieldValue = _curtab.getField(columnName).value;
                        }
                        else {
                            if (rowsSource && rowsSource.length > 0) {
                                fieldValue = rowsSource[0][columnName.toLower()];
                            }
                            else
                                fieldValue = rowsSingleView[columnName.toLower()];
                        }
                    }
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                retValues['@@' + fieldname + '@@'] = fieldValue;
            }
            return retValues;
        };

        ///For Multi Row Selection
        function parseHtml1(html, i) {
            var finalhtmlperRecord = html;
            var copyhtml = html;
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (columnName != undefined && columnName != null) {
                    if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                        if (rowsSource[i][columnName.toLower()] != null && rowsSource[i][columnName.toLower()] != undefined) {
                            //fieldValue = _curtab.getField(columnName.toLower()).lookup.lookup[" " + rowsSource[i][columnName.toLower()]].Name;
                            fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSource[i][columnName.toLower()]);
                        }
                    }
                    else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {

                        var displayType = _curtab.getField(columnName).getDisplayType();
                        fieldValue = _curtab.getField(columnName).value;

                        if (VIS.DisplayType.Date == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleDateString();
                        }
                        else if (VIS.DisplayType.Time == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleTimeString();
                        }
                        else {
                            fieldValue = new Date(fieldValue).toLocaleString();
                        }

                    }
                    else {
                        fieldValue = rowsSource[i][columnName.toLower()];
                    }
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                if (fieldValue != null && fieldValue != undefined) {
                    finalhtmlperRecord = finalhtmlperRecord.replace('@@' + fieldname + '@@', fieldValue);
                }
                else {
                    finalhtmlperRecord = finalhtmlperRecord.replace('@@' + fieldname + '@@', "");
                }
            }

            return finalhtmlperRecord;
        };

        ///For Single View Selection
        function parseHtml2(html) {

            var copyhtml = html;
            while (copyhtml.indexOf("@@") > -1) {
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                var fieldname = copyhtml.substring(0, copyhtml.indexOf("@@"));
                var fieldValue = null;
                var columnName = Object.keys(_curGC.getColumnNames()).filter(function (key) { return _curGC.getColumnNames()[key] === fieldname })[0];
                if (columnName != undefined && columnName != null) {
                    if (VIS.DisplayType.IsLookup(_curtab.getField(columnName).getDisplayType()) || VIS.DisplayType.Location == _curtab.getField(columnName).getDisplayType()) {
                        if (rowsSingleView[columnName.toLower()] != null && rowsSingleView[columnName.toLower()] != undefined) {
                            fieldValue = _curtab.getField(columnName.toLower()).lookup.getDisplay(rowsSingleView[columnName.toLower()]);
                        }
                        else {
                            fieldValue = "";
                        }
                    }
                    else if (VIS.DisplayType.IsDate(_curtab.getField(columnName).getDisplayType())) {

                        var displayType = _curtab.getField(columnName).getDisplayType()
                        fieldValue = _curtab.getField(columnName).value;

                        if (VIS.DisplayType.Date == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleDateString();
                        }
                        else if (VIS.DisplayType.Time == displayType) {
                            fieldValue = new Date(fieldValue).toLocaleTimeString();
                        }
                        else {
                            fieldValue = new Date(fieldValue).toLocaleString();
                        }
                    }
                    else {
                        if (_curGC.singleRow == true && rowsSource.length == 0) {
                            fieldValue = _curtab.getField(columnName).value;
                            if (fieldValue == null) {
                                fieldValue = "";
                                //$($textAreakeno.selectionRestorePoint.range.commonAncestorContainer.parentNode).remove();
                            }
                        }
                        else {
                            fieldValue = rowsSingleView[columnName.toLower()];
                        }
                    }
                }
                copyhtml = copyhtml.substring(copyhtml.indexOf("@@") + 2);
                html = html.replace('@@' + fieldname + '@@', fieldValue);
            }
            return html;


        };

        function setDesignOnSizeChange() {

            var middleDivWidth = 0;
            $root.height(formHeight);
            $root.width(formWidth);
            $root.find('.contentArea').css('height', $root.height() - 42);


            $root.find('.vis-Email-BccList').height(1);

            $root.find('.vis-email-leftDiv').height($root.find('.contentArea').height());
            if (callingFromOutsideofWindow) {
                if (!$root.find('.vis-email-attachmentContainer').is(":visible")) {

                    $root.find(".vis-Email-textarea-div").height($root.height() - 180);
                }
                else {
                    $root.find(".vis-Email-textarea-div").height($root.height() - 345);
                }

                $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 85);
                middleDivWidth = $root.width() - 40;
            }
            else {
                if (isEmail) {


                    if ($root.find('.vis-email-attachmentContainer').is(":visible")) {
                        $root.find(".vis-Email-textarea-div").height($root.height() - ($root.find('.vis-email-attachmentContainer').height() + 150));

                        //if ($textAreakeno != null && $textAreakeno != undefined) {
                        //    $textAreakeno.body.style.height = ($root.find(".vis-Email-textarea-div").height() - 80) + "px";
                        //}
                    }
                    else {
                        //$root.find(".vis-Email-textarea-div").height($root.height() - ($root.find('.vis-email-attachmentContainer').height() + 50));

                        $root.find(".vis-Email-textarea-div").height($root.height() - ($root.find('.vis-email-attachmentContainer').height() + 70));



                        //if ($textAreakeno != null && $textAreakeno != undefined) {
                        //    $textAreakeno.body.style.height = ($root.find(".vis-Email-textarea-div").height() - 80) + "px";
                        //}
                    }
                    //middleDivWidth = $root.width() - 232;
                    middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 30);
                    $root.find('.vis-Email-leftWrap').width(middleDivWidth - 355);
                    $root.find('.vis-form-data-sub').width($root.find('.vis-Email-leftWrap').width() - 45);
                }
                else {

                    $root.find(".vis-Email-textarea-div").height($root.height() - (125 + 30));

                    if ($textAreakeno != null && $textAreakeno != undefined) {
                        //if ($textAreakeno != null && $textAreakeno != undefined) {
                        //    $textAreakeno.body.style.height = ($root.find(".vis-Email-textarea-div").height() - 80) + "px";
                        //}
                        //$textAreakeno.height($root.height() - (125 + 30 + 10));
                    }
                    // $('.vis-Email-leftWrap').css("width", '100%');
                    //middleDivWidth = $root.width() - 232;
                    middleDivWidth = formWidth - ($root.find('.vis-email-leftDiv').width() + 30);
                    $root.find('.vis-Email-leftWrap').width(middleDivWidth - 15);
                    $root.find('.vis-form-data-sub').width(middleDivWidth - 40);

                }

            }


            if ($middleDiv != null) {
                $middleDiv.css('width', middleDivWidth - 10);
                //window.setInterval(function () {
                //    $middleDiv.css('width', middleDivWidth);
                //}, 200);
            }

            if ($textAreakeno != null && $textAreakeno != undefined) {
                $textAreakeno.body.style.width = (middleDivWidth - 370) + "px";
            }

            if (!callingFromOutsideofWindow) {
                if (window.VADMS) {
                    $root.find('.vis-Email-BccList').height($root.find('.vis-Email-rytContent').height() - ($root.find('.vis-Email-rytForm').height() + 310));
                }
                else {
                    $root.find('.vis-Email-BccList').height($root.find('.vis-Email-rytContent').height() - ($root.find('.vis-Email-rytForm').height() + 70));
                }
            }

        };


        this.showCcBccMails = function (mailcc, mailbcc) {
            if (mailcc != undefined && mailcc != null && mailcc != '')
                ccmail = mailcc;
            if (mailbcc != undefined && mailbcc != null && mailbcc != '')
                bccmail = mailbcc;
        }

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setHeight(800);
            ch.setWidth(1600);
            ch.setTitle(VIS.Msg.getMsg("EMail"));
            ch.setModal(true);
            ch.show();

            ch.setContent($root);
            initializeComponent();
        };

        this.disposeComponent = function () {
            //this.frame = null;
            //this.windowNo = 0;
            ctx = null;
            ch = null;
            bpColumnName = "";
            //rowsSource = null;
            //rowsSingleView = null;
            currentTable_ID = 0;
            //selectedValues.clear();
            selectedValues = null;

            //if ($btnHdrCancel != null) {
            //    $btnHdrCancel.off('click');
            //}
            if (!callingFromOutsideofWindow) {
                if ($btnHdrOpen != null)
                    $btnHdrOpen.off('click');
                if ($btnHdrPreview != null)
                    $btnHdrPreview.off('click');
                if ($btnHdrSave != null)
                    $btnHdrSave.off('click');
                if ($btnHdrSaveAs != null)
                    $btnHdrSaveAs.off('click');
                if ($imgAction != null)
                    $imgAction.off('click');
                if ($ulFieldNames != null)
                    $ulFieldNames.remove();
                if ($imgAction != null)
                    $imgAction.remove();
                if ($bccChkList != null)
                    $bccChkList.remove();
                if ($dynamicDisplay != null)
                    $dynamicDisplay.remove();
                if ($txtArea != null)
                    $txtArea.remove();
            }
            if ($btnHdrSend != null) {
                $btnHdrSend.off('click');
            }
            if (fileBrowser != null && fileBrowser != undefined) {
                fileBrowser.off("change");
                fileBrowser.off("click");
            }



            $ulFieldNames = null;
            if ($root != null)
                $root.remove();
            $root = null;
            if ($leftDiv != null) {
                $leftDiv.remove();
            }
            $leftDiv = null;
            if ($middleDiv != null)
                $middleDiv.remove();
            $middleDiv = null;
            if ($rightDiv != null)
                $rightDiv.remove();
            $rightDiv = null;
            //$imgAction.remove();
            $imgAction = null;
            // $bccChkList.remove();
            $bccChkList = null;
            //$dynamicDisplay.remove();
            $dynamicDisplay = null;
            if (isEmail) {
                if ($to != null)
                    $to.remove();
                $to = null;
                if ($bcc != null)
                    $bcc.remove();
                $bcc = null;
                if ($cc != null)
                    $cc.remove();
                $cc = null;
            }
            if ($subject != null)
                $subject.remove();
            $subject = null;
            //  $txtArea.remove();
            $txtArea = null;
            if ($textAreakeno != null && $textAreakeno != undefined) {
                $textAreakeno.destroy();
            }
            $textAreakeno = null;
            if ($leftfootArea != null)
                $leftfootArea.remove();
            $leftfootArea = null;

            $btnHdrPreview = null;
            $btnHdrSend = null;
            $btnHdrCancel = null;
            $btnHdrOpen = null;
            $btnHdrSave = null;
            $btnHdrSaveAs = null;

            if ($chkBSendPFasAtt != null) {
                $chkBSendPFasAtt.off("click");
                $chkBSendPFasAtt = null;
            }
            if ($cmbPfFiletype != null) {
                $cmbPfFiletype = null;
            }
            //self = null;
        };

        this.sizeChanged = function (height, width) {

            formHeight = height;
            formWidth = width;
            if (isViewLoaded == true) {
                setDesignOnSizeChange();
                this.leftULsize();
            }
        };


        this.ShowDMSDocumentAttchment = function () {

            var docLst = null;
            var a = null;
            $attachDmsDoc.children().remove();
            var attachDocUl = $("<ul></ul>");
            if (VIS.context.getContext("DocumentAttachViaEmail_" + self.windowNo) != "") {
                $dmsheader.empty();
                $dmsheader.append(VIS.Msg.getMsg("Documents"));
                docLst = VIS.context.getContext("DocumentAttachViaEmail_" + self.windowNo).split(",");
                for (var i = 0; i < docLst.length; i++) {
                    if (docLst[i].trim() == "") {
                        continue;
                    }
                    a = docLst[i].split("-");
                    attachDocUl.append("<Li><input type='checkbox' checked value='" + a[0] + "'> " + a[1] + "</Li>");
                }
                $attachDmsDoc.append(attachDocUl);
            }
            else {
                $dmsheader.empty();
            }

            attachDocUl.on("click", "input", function () {
                var strAttachDocs = "";
                var id = $(this).attr("value");
                if (!$(this).is(":checked")) {
                    var l = VIS.context.getContext("DocumentAttachViaEmail_" + self.windowNo).split(",");
                    var r = jQuery.grep(l, function (value) {
                        if (!value.trim().contains(id.toString().trim()))
                            return value;
                    });
                    for (var i = 0; i < r.length; i++) {
                        if (i == 0) {
                            strAttachDocs += r[i].trim();
                        }
                        else {
                            strAttachDocs += "," + r[i].trim();
                        }
                    }
                }
                else if ($(this).is(":checked")) {
                    var f = id + "-" + $(this).text();
                    strAttachDocs = VIS.context.getContext("DocumentAttachViaEmail_" + self.windowNo);
                    if (strAttachDocs != "") {
                        strAttachDocs += "," + f;
                    }
                    else { strAttachDocs += f; }
                }
                VIS.context.setContext("DocumentAttachViaEmail_" + self.windowNo, strAttachDocs);

            });
            // self.IsBusy(false);
        };

        this.IsBusy = function (isShow) {

            if (isShow) {
                isBusy.css("display", "block");
            }
            else { isBusy.css("display", "none"); }
            isBusy.appendTo($root);
        };

    };

    //Must Implement with same parameter
    Email.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        frame.hideHeader(true);
        this.initializeComponent();

        this.frame.getContentGrid().append(this.getRoot());

    };
    Email.prototype.refresh = function () {
        //this.IsBusy(true);
        if (window.VADMS) {
            this.ShowDMSDocumentAttchment();
        }
        this.leftULsize();
    };
    //Must implement dispose
    Email.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Email = Email;

})(VIS, jQuery);





//;(function($) {
//    $.fn.hasScrollBar = function() {
//        return this.get(0).scrollHeight > this.height();
//    }
//})(jQuery);