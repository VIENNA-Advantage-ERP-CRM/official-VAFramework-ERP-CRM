(function (VIS, $) {
    function attachmentForm(windowNo, AD_Attachment_ID, AD_Table_ID, Record_ID, trxName, newRecord_ID, restrictUpload, isDMSAttachment) {
        var isFALoaded = false;
        var isIMALoaded = false;
        var isSMALoaded = false;
        var isLTRLoaded = false;
        var isAPPLoaded = false;
        this.isWindowAction = false;
        var dLAContent = null;
        var dOldAtt = null;
        var dOAContent = null;
        var cmbFileLocation = null;
        this.windowNo = windowNo;
        var AD_Attachment_ID = 0;
        if (!newRecord_ID) {
            newRecord_ID = 0;
        }

        if (!isDMSAttachment) {
            isDMSAttachment = false;
        }

        if (!restrictUpload) {
            restrictUpload = false;
        }

        function disposeComponent() {
            dLAContent.empty();
            dLAContent = null;
            dOldAtt.empty();
            dOldAtt = null;
            dOAContent.empty();
            dOAContent = null;
            fileAtt = null;
            inboxMailAtt = null;
            sentMailAtt = null;
            letter = null;
            appointment = null;
            tabs = null;
            divAppointment = null;
            divContainer = null;
            divFA = null;
            divIMA = null;
            divSMA = null;
            divLetter = null;
            bsyDiv = null;
            tabs = null;

            lstLatestFiles = null;
            progresBar = null;
            oldFiles = null;

            if (root != null) {
                root.dialog('destroy');
                root.remove();
            }
            root = null;

        }

        this.onClose = null;
        var selfi = this;
        var root = null;

        if (VIS.Application.isRTL) {
            root = $("<div style='overflow:hidden;width:100%;height:100%;'>");
        }
        else {
            root = $("<div style='overflow:hidden;width:100%;height:100%;'>");
        }
        //root.css("width", "100%");
        //root.css("height", "100%");

        var tabs = $("<ul style='height:30px;display:none;list-style:none;width:97%;margin-left:-41px'>");
        //tabs.css("list-style", "none");
        //tabs.css("width", "97%");
        //tabs.css("margin-left", "-41px");

        var fileAtt = $("<li>");
        fileAtt.css("float", "left");
        fileAtt.css("margin", "1px");
        fileAtt.css("padding", "5px");
        fileAtt.css("display", "block");
        fileAtt.css("position", "relative");
        fileAtt.css("font-weight", "700");
        // fileAtt.css("background", "#eee");
        //fileAtt.css("border", "1px solid #ddc");
        fileAtt.css("border", "0px");
        fileAtt.css("border-bottom", "none");
        fileAtt.css("border-width", "1px");
        fileAtt.css("color", "#999");
        fileAtt.css("cursor", "default");
        fileAtt.css("height", "35px");
        fileAtt.css("margin-bottom", "-1px");
        fileAtt.css("margin-right", "5px");
        fileAtt.css("border-top-left-radius", "3px");
        fileAtt.css("border-top-right-radius", "3px");

        fileAtt.append(VIS.Msg.getMsg("FileAttachment"));
        tabs.append(fileAtt);

        var inboxMailAtt = $("<li>");
        inboxMailAtt.css("float", "left");
        inboxMailAtt.css("margin", "1px");
        inboxMailAtt.css("padding", "5px");
        inboxMailAtt.css("display", "block");
        inboxMailAtt.css("position", "relative");
        inboxMailAtt.css("font-weight", "700");
        // fileAtt.css("background", "#eee");
        //fileAtt.css("border", "1px solid #ddc");
        fileAtt.css("border", "0px");
        inboxMailAtt.css("border-bottom", "none");
        inboxMailAtt.css("border-width", "1px");
        inboxMailAtt.css("color", "#999");
        inboxMailAtt.css("cursor", "default");
        inboxMailAtt.css("height", "35px");
        inboxMailAtt.css("margin-bottom", "-1px");
        inboxMailAtt.css("margin-right", "5px");
        inboxMailAtt.css("border-top-left-radius", "3px");
        inboxMailAtt.css("border-top-right-radius", "3px");

        inboxMailAtt.append(VIS.Msg.getMsg("InboxMailAttachment"));
        tabs.append(inboxMailAtt);

        var sentMailAtt = $("<li>");
        sentMailAtt.css("float", "left");
        sentMailAtt.css("margin", "1px");
        sentMailAtt.css("padding", "5px");
        sentMailAtt.css("display", "block");
        sentMailAtt.css("position", "relative");
        sentMailAtt.css("font-weight", "700");
        // fileAtt.css("background", "#eee");
        //fileAtt.css("border", "1px solid #ddc");
        fileAtt.css("border", "0px");
        sentMailAtt.css("border-bottom", "none");
        sentMailAtt.css("border-width", "1px");
        sentMailAtt.css("color", "#999");
        sentMailAtt.css("cursor", "default");
        sentMailAtt.css("height", "35px");
        sentMailAtt.css("margin-bottom", "-1px");
        sentMailAtt.css("margin-right", "5px");
        sentMailAtt.css("border-top-left-radius", "3px");
        sentMailAtt.css("border-top-right-radius", "3px");

        sentMailAtt.append(VIS.Msg.getMsg("SentMailAttachment"));
        tabs.append(sentMailAtt);

        var letter = $("<li>");
        letter.css("float", "left");
        letter.css("margin", "1px");
        letter.css("padding", "5px");
        letter.css("display", "block");
        letter.css("position", "relative");
        letter.css("font-weight", "700");
        // fileAtt.css("background", "#eee");
        //fileAtt.css("border", "1px solid #ddc");
        fileAtt.css("border", "0px");
        letter.css("border-bottom", "none");
        letter.css("border-width", "1px");
        letter.css("color", "#999");
        letter.css("cursor", "default");
        letter.css("height", "35px");
        letter.css("margin-bottom", "-1px");
        letter.css("margin-right", "5px");
        letter.css("border-top-left-radius", "3px");
        letter.css("border-top-right-radius", "3px");

        letter.append(VIS.Msg.getMsg("Letter"));
        tabs.append(letter);

        var appointment = $("<li>");
        appointment.css("float", "left");
        appointment.css("margin", "1px");
        appointment.css("padding", "5px");
        appointment.css("display", "block");
        appointment.css("position", "relative");
        appointment.css("font-weight", "700");
        // fileAtt.css("background", "#eee");
        //fileAtt.css("border", "1px solid #ddc");
        fileAtt.css("border", "0px");
        appointment.css("border-bottom", "none");
        appointment.css("border-width", "1px");
        appointment.css("color", "#999");
        appointment.css("cursor", "default");
        appointment.css("height", "35px");
        appointment.css("margin-bottom", "-1px");
        appointment.css("margin-right", "5px");
        appointment.css("border-top-left-radius", "3px");
        appointment.css("border-top-right-radius", "3px");

        appointment.append(VIS.Msg.getMsg("Appointments"));
        tabs.append(appointment);

        root.append(tabs);

        var divContainer = $("<div class='vis-pu-outerwrap'>");
        //divContainer.css("width", "100%");
        //divContainer.css("height", "100%");
        //divContainer.css("position", "absolute");
        //divContainer.css("border", "1px solid #ddc");        
        //divContainer.css("border", "0px");
        //divContainer.css("margin-top", "2px");

        var divFA = $("<div class='vis-pu-innerwrap'>");
        //divFA.append("File Att");
        //divFA.css("width", "100%");
        //divFA.css("height", "100%");
        //divFA.css("position", "absolute");
        divContainer.append(divFA);

        var divIMA = $("<div>");
        divIMA.append("Inbox Mail Att");
        divIMA.css("width", "100%");
        divIMA.css("height", "100%");
        divIMA.css("position", "absolute");
        divContainer.append(divIMA);

        var divSMA = $("<div>");
        divSMA.append("SENT MAIL Att");
        divSMA.css("width", "100%");
        divSMA.css("height", "100%");
        divSMA.css("position", "absolute");
        divContainer.append(divSMA);

        var divLetter = $("<div>");
        divLetter.append("Leter Att");
        divLetter.css("width", "100%");
        divLetter.css("height", "100%");
        divLetter.css("position", "absolute");
        divContainer.append(divLetter);

        var divAppointment = $("<div>");
        divAppointment.append("Appointment Att");
        divAppointment.css("width", "100%");
        divAppointment.css("height", "100%");
        divAppointment.css("position", "absolute");
        divContainer.append(divAppointment);

        //var viewer = $("<div style='visibility:collapse;-webkit-overflow-scrolling:touch;'>");
        //viewer.css("width", "100%");
        //viewer.css("height", "100%");
        //viewer.css("position", "absolute");
        //divContainer.append(viewer);

        //var viTop = $("<div class='attach-file-top'>");
        //var vibtnRemove = $("<a class='file-close-ico'>");
        //vibtnRemove.on('click', function () {
        //    iFrame.css('visibility', 'collapse');
        //    viewer.css('visibility', 'collapse');
        //    divFA.css('visibility', 'visible');
        //    iFrame.attr('src', null);
        //});
        //viTop.append(vibtnRemove);
        //viewer.append(viTop);
        //var viContent = $("<div style='height:100%;width:100%;'>");
        //viewer.append(viContent);
        //var iFrame = $("<iframe style='visibility:collapse;height:98%;width:100%;'>");        
        //viContent.append(iFrame);


        //var bsyDiv = $("<div class='vis-apanel-busy'>");
        var bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        //bsyDiv.css("width", "100%");
        //bsyDiv.css("height", "100%");
        //bsyDiv.css('text-align', 'center');
        //bsyDiv.css("position", "absolute");
        divContainer.append(bsyDiv);


        //ProgressBar
        var divReadOnly = $("<div style='width: 100%;height: 100%;background:black;opacity: .1;display:none; position:absolute;top:0;left:0'>");//table-cell
        divContainer.append(divReadOnly);

        var divpContainer = $("<div style='width: 100%;height: 100%;background:transparent;text-align: center;display:none ;vertical-align: middle;position:absolute;top:0;left:0'>");//table-cell
        divContainer.append(divpContainer);

        var divProgress = null;
        if (VIS.Application.isRTL) {
            divProgress = $("<div id='progress_bar' class='vis-ui-progress-bar vis-ui-container' style='top:41%;width: 80%;right: 9%;'>");
        }
        else {
            divProgress = $("<div id='progress_bar' class='vis-ui-progress-bar vis-ui-container' style='top:41%;width: 80%;left: 9%;'>");
        }
        var divGreen = $("<div class='vis-ui-progress' style='width: 0%; display: block;position:absolute'>");
        var lblpercentage = $("<span class='vis-ui-label'  style='position:absolute'>").append("0%");
        divGreen.append(lblpercentage);
        divProgress.append(divGreen);
        divpContainer.append(divProgress);

        root.append(divContainer);

        divFA[0].style.visibility = 'visible';
        divIMA[0].style.visibility = 'hidden';
        divSMA[0].style.visibility = 'hidden';
        divLetter[0].style.visibility = 'hidden';
        divAppointment[0].style.visibility = 'hidden';
        bsyDiv[0].style.visibility = 'hidden';


        fileAtt.css("background", "#fff");
        fileAtt.css("color", "#0088CC");
        var setActive = function (ctrl, isActive) {
            if (isActive) {
                ctrl.css("background", "#fff");
                ctrl.css("color", "#0088CC");

            }
            else {

                ctrl.css("background", "#eee");
                ctrl.css("color", "#999");

            }

        };



        fileAtt.on("click", function () {
            divFA[0].style.visibility = 'visible';
            divIMA[0].style.visibility = 'hidden';
            divSMA[0].style.visibility = 'hidden';
            divLetter[0].style.visibility = 'hidden';
            divAppointment[0].style.visibility = 'hidden';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, true);
            setActive(inboxMailAtt, false);
            setActive(sentMailAtt, false);
            setActive(letter, false);
            setActive(appointment, false);
            LoadFileAttachment();
        });

        inboxMailAtt.on("click", function () {
            divFA[0].style.visibility = 'hidden';
            divIMA[0].style.visibility = 'visible';
            divSMA[0].style.visibility = 'hidden';
            divLetter[0].style.visibility = 'hidden';
            divAppointment[0].style.visibility = 'hidden';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, false);
            setActive(inboxMailAtt, true);
            setActive(sentMailAtt, false);
            setActive(letter, false);
            setActive(appointment, false);
        });

        sentMailAtt.on("click", function () {
            divFA[0].style.visibility = 'hidden';
            divIMA[0].style.visibility = 'hidden';
            divSMA[0].style.visibility = 'visible';
            divLetter[0].style.visibility = 'hidden';
            divAppointment[0].style.visibility = 'hidden';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, false);
            setActive(inboxMailAtt, false);
            setActive(sentMailAtt, true);
            setActive(letter, false);
            setActive(appointment, false);
        });

        letter.on("click", function () {
            divFA[0].style.visibility = 'hidden';
            divIMA[0].style.visibility = 'hidden';
            divSMA[0].style.visibility = 'hidden';
            divLetter[0].style.visibility = 'visible';
            divAppointment[0].style.visibility = 'hidden';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, false);
            setActive(inboxMailAtt, false);
            setActive(sentMailAtt, false);
            setActive(letter, true);
            setActive(appointment, false);
        });

        appointment.on("click", function () {
            divFA[0].style.visibility = 'hidden';
            divIMA[0].style.visibility = 'hidden';
            divSMA[0].style.visibility = 'hidden';
            divLetter[0].style.visibility = 'hidden';
            divAppointment[0].style.visibility = 'visible';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, false);
            setActive(inboxMailAtt, false);
            setActive(sentMailAtt, false);
            setActive(letter, false);
            setActive(appointment, true);
        });

        this.show = function () {


            divFA[0].style.visibility = 'visible';
            divIMA[0].style.visibility = 'hidden';
            divSMA[0].style.visibility = 'hidden';
            divLetter[0].style.visibility = 'hidden';
            divAppointment[0].style.visibility = 'hidden';
            bsyDiv[0].style.visibility = 'hidden';
            setActive(fileAtt, true);
            setActive(inboxMailAtt, false);
            setActive(sentMailAtt, false);
            setActive(letter, false);
            setActive(appointment, false);
            LoadFileAttachment();

            root.dialog({
                width: 841,
                height: 502,
                title: VIS.Msg.getMsg("Attachment"),
                resizable: false,
                modal: true,
                close: onClosing
            });
        };

        function onClosing() {
            if (selfi.onClose) {
                selfi.onClose();
            }
            disposeComponent();


        };








        var ddLABtnsFull = $("<div class='vis-pu-uploadsbtnswrap'>");;
        var lstLatestFiles = [];
        //var progresBar = null;
        var btnInsert = null;
        function LoadFileAttachment() {

            if (isFALoaded) {
                return;
            }
            bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "Attachment/GetAttachment",
                dataType: "json",
                data: {
                    AD_Table_ID: AD_Table_ID,
                    Record_ID: Record_ID
                },
                error: function () {
                    VIS.ADialog.info('ERRORGettingAttachment');
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {
                    // debugger;
                    var locations = data.result.FLocation;
                    var attachment = data.result.Attachment;
                    if (locations == null) {
                        VIS.ADialog.info('ERRORGettingAttachment');
                        bsyDiv[0].style.visibility = "hidden";
                        return;
                    }



                    var dlatestAtt = $("<div class='vis-latest-attachments'>");
                    divFA.append(dlatestAtt);

                    var dlaHeader = $("<div class='vis-attach-top'>");
                    dlatestAtt.append(dlaHeader);

                    //var dlbl = $("<div class='attach-heading pull-left'>").append($("<h4>").append(VIS.Msg.getMsg('New')));
                    // dlaHeader.append(dlbl);
                    var dfRight = $("<div class='vis-attach-file-location'>");
                    var dflInputWrap = $("<div class='input-group vis-input-wrap' style='margin-bottom: 0;'>");
                    var dflInputCtrlWrap = $("<div class='vis-control-wrap'>");
                    dlaHeader.append(dfRight);
                    dfRight.append(dflInputWrap);
                    dflInputWrap.append(dflInputCtrlWrap);

                    cmbFileLocation = $('<select class="vis-custom-select">');
                    cmbFileLocation.attr("disabled", true);
                    //add options to combo
                    for (var itm in locations.values) {
                        var opt = $("<option value=" + locations.values[itm].Key + ">");
                        opt.append(locations.values[itm].Name);
                        cmbFileLocation.append(opt);
                        if (locations.selectedvalue == locations.values[itm].Key) {
                            cmbFileLocation.val(locations.selectedvalue);
                        }

                    }

                    dflInputCtrlWrap.append(cmbFileLocation);

                    var lblF = $("<label>").append(VIS.Msg.getMsg('FileLocation'));
                    dflInputCtrlWrap.append(lblF);


                    var dHeaderRight = $("<div class='vis-attach-top-btns'>");
                    dlaHeader.append(dHeaderRight);


                    //var divFUpload = $("<div class='image-upload'>")
                    //dHeaderRight.append(divFUpload);

                    //divFUpload.append($("<lable class='vis-file-label'>").append(VIS.Msg.getMsg('Upload')));
                    //divFUpload.append($("<input type='file'>"));


                    var fileBrowser = $("<input type='file' multiple='true' style='display:none;'>");
                    dHeaderRight.append(fileBrowser);

                    //Add file to Latest File Content
                    fileBrowser.change(function () {
                        //VIS.ADialog.info('file Uploaded ');
                        AppendFile(this);
                        
                    });
                    fileBrowser.click(function () {                        
                        this.value = null;
                    });

                   
                    var btnUpload = $("<a title='" + VIS.Msg.getMsg('SelectFile') + "' class='vis-attach-ico'><i class='fa fa-paperclip' aria-hidden='true'></i></a>");
                    // btnUpload.append($("<span class='plus-ico'>"));
                    // btnUpload.append(VIS.Msg.getMsg('Upload'));

                    dHeaderRight.append(btnUpload);

                    //open file dialog
                    btnUpload.on("click", function () {                       
                        fileBrowser.trigger('click');
                    });

                    if (restrictUpload) {
                        btnUpload.hide();
                    }

                    var dArrange = $("<div class='vis-dropdown pull-left'>");
                    dHeaderRight.append(dArrange);

                    var btnArrange = $("<a class='vis-btn vis-btn-arrange vis-dropdown-toggle' type='button' data-toggle='dropdown' style='color: #fff;display:none;'>");
                    btnArrange.append(VIS.Msg.getMsg('Arrange'));
                    btnArrange.append($("<span class='vis-caret'>"));
                    dArrange.append(btnArrange);

                    var ulArrange = $("<ul class='dropdown-menu' role='menu' aria-labelledby='dropdownMenu1' id='dropdownMenu' style='margin-left: -33px;'>");
                    dArrange.append(ulArrange);

                    var liType = $("<li role='presentation'>");
                    var aType = $("<a role='menuitem' tabindex='-1' href='#'>");
                    aType.append(VIS.Msg.getMsg('Type'));
                    liType.append(aType);
                    liType.on("click", function () {

                    });
                    ulArrange.append(liType);

                    var liSize = $("<li role='presentation'>");
                    var aSize = $("<a role='menuitem' tabindex='-1' href='#'>");
                    aSize.append(VIS.Msg.getMsg('Size'));
                    liSize.append(aSize);
                    liSize.on("click", function () {

                    });
                    ulArrange.append(liSize);

                    var liName = $("<li role='presentation'>");
                    var aName = $("<a role='menuitem' tabindex='-1' href='#'>");
                    aName.append(VIS.Msg.getMsg('Name'));
                    liName.append(aName);
                    liName.on("click", function () {

                    });
                    ulArrange.append(liName);


                    dLAContent = $("<div class='vis-attach-content-wrap' style='display:none;'>");
                    dlatestAtt.append(dLAContent);


                    //btns in Latest file Region
                    // ddLABtnsFull = $("<div style='width:100%;overflow: auto;margin-top: 5px;border-bottom: 2px solid #eee'>");
                    dlatestAtt.append(ddLABtnsFull);
                    var dLABtns = $("<div class='vis-pull-right'>");
                    ddLABtnsFull.append(dLABtns);
                    var btnuploadFiles = $("<a title='" + VIS.Msg.getMsg('Upload') + "' href='javascript:void(0)'  class='vis-attach-ico'><i class='fa fa-upload' aria-hidden='true'></i></a>");
                    dLABtns.append(btnuploadFiles);
                    btnuploadFiles.on('click', function () {
                        UploadFiles();
                    });

                    var btnCancelAll = $("<a title='" + VIS.Msg.getMsg('Cancel') + "' href='javascript:void(0)'  class='vis-attach-ico'><i class='fa fa-times-circle-o' aria-hidden='true'></i></a>");
                    dLABtns.append(btnCancelAll);
                    btnCancelAll.on('click', function () {
                        dLAContent.empty();
                        filesInfo = [];
                        lstLatestFiles = [];
                        dLAContent.hide();
                        ddLABtnsFull.hide();
                        dOAContent.css('height', '353px');
                    });


                    //Old Attachments
                    dOldAtt = $("<div class='vis-old-attachments'>");
                    divFA.append(dOldAtt);

                    //var dOldhdr = $("<div class='attach-top'>");
                    //dOldAtt.append(dOldhdr);

                    //var dOldhdrRight = $("<div class='attach-heading pull-left'>");
                    //dOldhdr.append(dOldhdrRight);

                    //var hOld = $("<h4>").append(VIS.Msg.getMsg('Old'));
                    //dOldhdrRight.append(hOld);

                    dOAContent = $("<div class='vis-attach-content-wrap' style='height: 353px;'>");
                    dOldAtt.append(dOAContent);

                    //AppendFiles in Old Files Region
                    if (attachment != null) {
                        AD_Attachment_ID = attachment.AD_Attachment_ID;
                    }
                    if (attachment != null && attachment._lines != null) {
                        AD_Attachment_ID = attachment.AD_Attachment_ID;
                        for (var item in attachment._lines) {
                            AppendFileOldRegion(attachment._lines[item], item);
                        }
                    }


                    //footer 
                    var dfooter = $("<div class='vis-attach-footer'>");
                    divFA.append(dfooter);

                    //var dfRight = $("<div class='attach-file-location pull-left' style='margin-top: 5px;'>");
                    //dfooter.append(dfRight);

                    //var lblF = $("<label>").append(VIS.Msg.getMsg('FileLocation'));
                    //dfRight.append(lblF);

                    //cmbFileLocation = $('<select>');
                    ////add options to combo
                    //for (var itm in locations.values) {
                    //    var opt = $("<option value=" + locations.values[itm].Key + ">");
                    //    opt.append(locations.values[itm].Name);
                    //    cmbFileLocation.append(opt);
                    //    if (locations.selectedvalue == locations.values[itm].Key) {
                    //        cmbFileLocation.val(locations.selectedvalue);
                    //    }

                    //}

                    //dfRight.append(cmbFileLocation);

                    //progresBar = $("<progress value='0' max='100' style='margin-left:5px;'>");
                    //dfRight.append(progresBar);

                    var dfLeft = $("<div class='vis-attach-btns pull-right' style='margin-top: 5px;'>");
                    dfooter.append(dfLeft);

                    //btnInsert = $('<a class="btn" style="margin-left: 5px;">').append(VIS.Msg.getMsg('Insert'));
                    //btnInsert.on("click", function () {
                    //    UploadFiles();
                    //});
                    //dfLeft.append(btnInsert);

                    var btnDeleteAll = $('<a  title="' + VIS.Msg.getMsg('DeleteAll') + '" class="vis-btn">').append(VIS.Msg.getMsg('DeleteAll'));
                    dfLeft.append(btnDeleteAll);
                    btnDeleteAll.on('click', function () {

                        if (oldFiles.length == 0 || restrictUpload) {
                            return;
                        }
                        // if (VIS.ADialog.ask("DeleteRecord?")) {

                        VIS.ADialog.confirm("DeleteRecord?", true, "", "Confirm", function (result) {
                            if (result) {
                                //DELETE ALL FILES
                                var whereQry = "";

                                for (var itm in oldFiles) {
                                    whereQry += oldFiles[itm].Line_ID + ',';
                                }
                                bsyDiv[0].style.visibility = "visible";
                                $.ajax({
                                    url: VIS.Application.contextUrl + "Attachment/DeleteAttachment",
                                    dataType: "json",
                                    data: {
                                        AttachmentLines: whereQry
                                    },
                                    error: function () {
                                        VIS.ADialog.info('ERRORDeletingFile');
                                        bsyDiv[0].style.visibility = "hidden";
                                    },
                                    success: function (data) {
                                        if (data.result == -1) {
                                            VIS.ADialog.info("ErrorDeletingFile");
                                        }
                                        else {
                                            dOAContent.empty();
                                            oldFiles = [];

                                        }
                                        bsyDiv[0].style.visibility = "hidden";
                                    }
                                });
                            }

                        });
                    });


                    var btnCancel = $('<a title="' + VIS.Msg.getMsg('Cancel') + '" class="vis-btn">').append(VIS.Msg.getMsg('Cancel'));
                    btnCancel.on('click', function () {
                        if (selfi.onClose) {
                            selfi.onClose();
                        }
                        disposeComponent();

                    })
                    dfLeft.append(btnCancel);
                    isFALoaded = true;
                    bsyDiv[0].style.visibility = "hidden";



                }
            });

        }


        var LoadOldFile = function () {

            //bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "Attachment/GetAttachment",
                dataType: "json",
                data: {
                    AD_Table_ID: AD_Table_ID,
                    Record_ID: Record_ID
                },
                error: function () {
                    VIS.ADialog.info('ERRORGettingAttachment');
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {

                    var attachment = data.result.Attachment;
                    //AppendFiles in Old Files Region
                    if (attachment != null) {
                        AD_Attachment_ID = attachment.AD_Attachment_ID;
                    }
                    if (attachment != null && attachment._lines != null) {
                        AD_Attachment_ID = attachment.AD_Attachment_ID;
                        for (var item in attachment._lines) {
                            AppendFileOldRegion(attachment._lines[item], item);
                        }
                    }
                    bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        var AppendFile = function (sender) {
            var invalidFiles = [];
            for (var i = 0; i < sender.files.length; i++) {
                file = sender.files[i];
                if (file == undefined) {
                    return;
                }
                if (file.name.indexOf('&') > -1 || file.name.indexOf('?') > -1 || file.name.indexOf('#') > -1 || file.name.indexOf('/') > -1 || file.name.indexOf('\\') > -1) {

                    invalidFiles.push(file.name);
                    continue;
                }

                for (var itm in lstLatestFiles) {
                    if (file.name == lstLatestFiles[itm].name) {
                        window.setTimeout(function () {
                            VIS.ADialog.info('AlreadyExists');

                        }, 20);
                        return;
                    }
                }
                if (oldFiles != null) {
                    for (var itm in oldFiles) {
                        if (file.name == oldFiles[itm].Name && !oldFiles[itm].IsDeleted) {
                            window.setTimeout(function () {
                                VIS.ADialog.info('AlreadyExists');
                            }, 20);
                            return;
                        }
                    }
                }

                var fileInfo = {};
                var dAWrap = $("<div class='vis-attach-file-wrapla'>");
                dLAContent.append(dAWrap);

                var dTop = $("<div class='vis-attach-file-top'>");
                var btnRemove = $("<a class='vis-file-close-ico'><i class='fa fa-times' aria-hidden='true'></i></a>");
                dTop.append(btnRemove);
                dAWrap.append(dTop);
                dLAContent.show();
                ddLABtnsFull.show();

                btnRemove.on("click", function () {
                    //debugger;
                    var c = null;
                    var divFInfo = $($(this).parent()).parent();
                    divFInfo.css("display", "none");
                    var html = divFInfo.html();
                    divFInfo.empty();
                    divFInfo = null;
                    for (var itm in lstLatestFiles) {
                        if ((String(html).indexOf(lstLatestFiles[itm].name)) > -1) {
                            lstLatestFiles.splice(itm, 1);
                            break;
                        }
                    }
                    if (lstLatestFiles.length > 4) {
                        dLAContent.css('height', '153px');
                        dOAContent.css('height', '153px');
                    }
                    else {
                        dLAContent.css('height', '97px');
                        dOAContent.css('height', '209px');

                    }
                    if (lstLatestFiles.length == 0) {
                        dLAContent.hide();
                        ddLABtnsFull.hide();
                        dLAContent.css('height', '0px');
                        dOAContent.css('height', '353px');
                    }
                    //dLAContent.remove(divFInfo);
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
                    var aFName = $("<a href='#' class='VIS_Pref_tooltip'>").append('...');
                    var span = $("<span style='width: inherit;'>");
                    span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
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
                if (lstLatestFiles.length > 4) {
                    dLAContent.css('height', '153px');
                    dOAContent.css('height', '153px');
                }
                else {
                    dLAContent.css('height', '97px');
                    dOAContent.css('height', '209px');

                }
            }



            if (invalidFiles.length > 0) {
                var resString = '';
                for (var item in invalidFiles) {
                    resString += invalidFiles[item].toString() + '\n';
                }
                if (invalidFiles.length == 1) {
                    resString += 'File has';
                }
                else {
                    resString += 'Files have';
                }
                resString += ' invalid name. Please change the file name and try again.';
                VIS.ADialog.info(resString);
            }
        };

        var oldFiles = [];
        var AppendFileOldRegion = function (file, index) {
            // style='margin-top:5px;margin-bottom:5px;'
            var dAWrap = $("<div class='vis-attach-file-wrap'>");
            dOAContent.append(dAWrap);



            var oldFile = {};
            var dAContent = $("<div class='vis-attach-file-content'>");
            dAWrap.append(dAContent);

            var dIcon = $("<div class='vis-attach-file-icon'>");
            dIcon.append($("<img src='" + getImageUrl(file.FileName) + "'>"));
            dAContent.append(dIcon);

            var dfInfo = $("<div class='vis-attach-file-text'>");
            var shortName = '';
            var lblFName = $("<p>");
            if (file.FileName.length > 17) {
                shortName = file.FileName.toString().substr(0, 17);
                lblFName.append(shortName);
                var aFName = $("<a href='#' class='VIS_Pref_tooltip'>").append('...');
                var span = $("<span style='width: inherit;'>");
                span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                span.append($("<label class='VIS_Pref_Label_Font'>").append(file.FileName));
                aFName.append(span);
                lblFName.append(aFName);
            }
            else {
                lblFName.append(file.FileName);
            }
            // dfInfo.append($("<p>").append(shortName));
            dfInfo.append(lblFName);


            //dfInfo.append($("<p>").append(file.FileName));
            dfInfo.append($("<p>").append((Number(file.Size) / 1024).toFixed(2) + "KB"));
            dAContent.append(dfInfo);

            var dbtns = $("<div class='vis-attach-BottomRight'>");

            //var iframe = $("<iframe style='display:none;'>");
            //dbtns.append(iframe);

            var btnDownload = $("<a href='javascript:void(0)'  class='vis-attach-ico' data-id='" + index + "'><i class='fa fa-long-arrow-down' aria-hidden='true'></i></a>");
            dbtns.append(btnDownload);
            btnDownload.on('click', function (event) {
                // debugger;
                //event.preventDefault();
                var id = $(this).data("id");
                if (!oldFiles[id].HaveSource) {

                    DownloadFile(id, this);
                }
                else {

                    //iFrame.attr('src', oldFiles[index].Source);
                    //LoadIframe();
                    window.open(oldFiles[index].Source, '_blank');
                }

            });

            var btnDelete = $("<a class='vis-attach-ico' data-id='" + index + "'><i class='vis vis-delete' aria-hidden='true'></i></a>");
            dbtns.append(btnDelete);
            btnDelete.on('click', function () {

                if (restrictUpload) {
                    return false;
                }
                var self = $(this);

                VIS.ADialog.confirm("DeleteRecord?", true, "", "Confirm", function (result) {
                    if (result) {
                        var id = self.data("id");
                        bsyDiv[0].style.visibility = "visible";
                        $.ajax({
                            url: VIS.Application.contextUrl + "Attachment/DeleteAttachment",
                            dataType: "json",
                            data: {
                                AttachmentLines: oldFiles[id].Line_ID
                            },
                            error: function () {
                                VIS.ADialog.info('ERRORDeletingFile');
                                bsyDiv[0].style.visibility = "hidden";
                            },
                            success: function (data) {
                                if (data.result == -1) {
                                    VIS.ADialog.info("ErrorDeletingFile");
                                }
                                else {
                                    // debugger;
                                    var divFInfo = self.parent().parent();
                                    divFInfo.css("display", "none");

                                    divFInfo.empty();
                                    divFInfo = null;
                                    //oldFiles.splice(id, 1);
                                    oldFiles[id].IsDeleted = true;

                                }
                                bsyDiv[0].style.visibility = "hidden";
                            }
                        });
                    }
                });
            });

            dAWrap.append(dbtns);

            oldFile.Name = file.FileName;
            oldFile.Size = file.Size;
            oldFile.Line_ID = file.Line_ID;
            oldFile.HaveSource = false;
            oldFile.Source = null;
            oldFile.IsDeleted = false;
            oldFiles.push(oldFile);

        };



        var currentchunk = 0;
        var currentFile = 0;
        var folder = Date.now().toString();
        var chunkSize = 1 * 1024 * 1024;
        var totalChunks = 0;
        var currentFileChunkNo = 0;
        var filesInfo = [];
        var UploadFiles = function () {
            // debugger;

            var fileInfo = null;
            //folder = Date.now().toString();
            //bsyDiv[0].style.visibility = "visible";
            showProgress(true);

            var tcSingleFile = 0;
            var currentChunk = 0;
            for (var itm in lstLatestFiles) {
                tcSingleFile = parseInt(lstLatestFiles[itm].size / chunkSize);
                if (file.size % chunkSize > 0) {
                    tcSingleFile++;
                }
                totalChunks += tcSingleFile;

                fileInfo = {};
                fileInfo.Name = lstLatestFiles[itm].name;
                fileInfo.Size = lstLatestFiles[itm].size;
                filesInfo.push(fileInfo);
            }
            //for (var itm in lstLatestFiles) {
            //    fileInfo = {};
            //    fileInfo.Name = lstLatestFiles[itm].name;
            //    fileInfo.Size = lstLatestFiles[itm].size;
            //    filesInfo.push(fileInfo);
            //UploadSingleFile(lstLatestFiles[itm], folder, chunkSize, currentChunk,totalChunks);
            TransferFile();
            //}


            //$.ajax({
            //    url: VIS.Application.contextUrl + "Attachment/SaveAttachmentEntries",
            //    dataType: "json",
            //    type: "POST",
            //    data: {
            //        files: JSON.stringify(filesInfo),
            //        AD_Attachment_ID: AD_Attachment_ID,
            //        folderKey: folder,
            //        AD_Table_ID: AD_Table_ID,
            //        Record_ID: Record_ID,
            //        fileLocation: cmbFileLocation.val()
            //    },
            //    error: function () {
            //        //VIS.ADialog.info(VIS.Msg.getMsg('ErrorWhileGettingData'));
            //        //bsyDiv[0].style.visibility = "hidden";
            //        // $("#divfeedbsy")[0].style.visibility = "hidden";
            //        //$("#divTabMenuDataLoder").hide();
            //        //return;
            //        bsyDiv[0].style.visibility = "hidden";
            //    },
            //    success: function (res) {
            //        //VIS.ADialog.info('done');
            //        //  workflowActivityData.refresh();
            //        if (res.result == "") {

            //            dLAContent.empty();
            //            oldFiles = [];
            //            dOAContent.empty();
            //            lstLatestFiles = [];
            //            LoadOldFile();
            //            dLAContent.hide();
            //            dLAContent.css('height', '0px');
            //            dOAContent.css('height', '356px');
            //        }
            //        else {
            //            VIS.ADialog.info(res.result);
            //        }
            //        bsyDiv[0].style.visibility = "hidden";
            //        setProgressValue(100);
            //        showProgress(false);
            //    }
            //});

        };

        var TransferFile = function () {
            if (lstLatestFiles == null || lstLatestFiles.length == 0) {
                showProgress(false);
                VIS.ADialog.info("NothingToUpload");
                return;
            }
            if (currentFile >= lstLatestFiles.length) {

                //Call Server Process


                $.ajax({
                    url: VIS.Application.contextUrl + "Attachment/SaveAttachmentEntries",
                    dataType: "json",
                    type: "POST",
                    data: {
                        files: JSON.stringify(filesInfo),
                        AD_Attachment_ID: AD_Attachment_ID,
                        folderKey: folder,
                        AD_Table_ID: AD_Table_ID,
                        Record_ID: Record_ID,
                        fileLocation: cmbFileLocation.val(),
                        NewRecord_ID: newRecord_ID,
                        IsDMSAttachment: isDMSAttachment
                    },
                    error: function () {
                        //VIS.ADialog.info(VIS.Msg.getMsg('ErrorWhileGettingData'));
                        //bsyDiv[0].style.visibility = "hidden";
                        // $("#divfeedbsy")[0].style.visibility = "hidden";
                        //$("#divTabMenuDataLoder").hide();
                        //return;
                        showProgress(false);
                        filesInfo = [];
                        bsyDiv[0].style.visibility = "hidden";
                    },
                    success: function (data) {
                        //VIS.ADialog.info('done');
                        //  workflowActivityData.refresh();
                        // debugger;

                        var res = data.result.Error;

                        if (res == "") {

                            /* ******THIS IS DONE BCOZ IN DMS ONLY ONE ATTACHMENT IS ALLOWED WITH ONE METADATA*********  */

                            if (isDMSAttachment) {
                                var oldAttachments = $('.vis-old-attachments').find('.vis-attach-file-wrap');

                                if (oldAttachments.length > 1) {
                                    $(oldAttachments[0]).remove();
                                }
                            }

                            /*********************************************************************************************/


                            setProgressValue(100);
                            window.setTimeout(function () {
                                showProgress(false);
                            }, 500);
                            filesInfo = [];
                            var selectedEffect = "transfer";
                            var options = {};
                            options = { to: dOAContent, className: "vis-Att-ui-effects-transfer" };

                            dLAContent.effect(selectedEffect, options, 500, function () {

                                // VIS.ADialog.info("call");
                                window.setTimeout(function () {
                                    oldFiles = [];
                                    dOAContent.empty();
                                    LoadOldFile();
                                    lstLatestFiles = [];
                                    dLAContent.empty();
                                    dLAContent.hide();
                                    ddLABtnsFull.hide();
                                    dLAContent.css('height', '0px');
                                    dOAContent.css('height', '353px');
                                    divGreen.css('width', 'O%');
                                }, 500);


                            });

                        }
                        else {
                            filesInfo = [];
                            showProgress(false);
                            VIS.ADialog.info(res);
                        }
                        currentchunk = 0;
                        currentFile = 0;
                        //folder = Date.now().toString();
                        // chunkSize = 1 * 1024 * 1024;
                        totalChunks = 0;
                        currentFileChunkNo = 0;
                        bsyDiv[0].style.visibility = "hidden";




                    }
                });

                return;
            }

            var xhr = new XMLHttpRequest();
            var fd = new FormData();
            fd.append("file", lstLatestFiles[currentFile].slice(currentFileChunkNo * chunkSize, currentFileChunkNo * chunkSize + Number(chunkSize)));
            xhr.open("POST", VIS.Application.contextUrl + "Attachment/SaveFileinTemp/?filename=" + lstLatestFiles[currentFile].name + "&folderKey=" + folder, false);
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
                setProgressValue(parseInt((currentchunk / totalChunks) * 80));
            }
            window.setTimeout(function () {
                TransferFile();
            }, 2);


            //xhr.onreadystatechange = function () {
            //    debugger;
            //    if (xhr.readyState == 4 && xhr.status == 200) {
            //        //document.getElementById("myDiv").innerHTML = xmlhttp.responseText;
            //        currentchunk++;
            //        setProgressValue(parseInt((currentchunk / totalChunks) * 80));
            //        TransferFile(chunkSize, totalChunks);

            //    }
            //}


        };

        var showProgress = function (show) {
            if (show) {
                divReadOnly.show();
                divProgress.show();
                divpContainer.show();
            }
            else {
                divReadOnly.hide();
                divProgress.hide();
                divpContainer.hide();
            }

        };





        var UploadSingleFile = function (file, folder, chunkSize, currentChunk, totalChunks) {

            // var folderkey = null;
            progresBar.attr("value", 0);

            var totalChunksSF = parseInt(file.size / chunkSize);

            if (file.size % chunkSize > 0) {
                totalChunksSF++;
            }

            var isRunning = false;
            for (var i = 0; i < totalChunksSF ; i++) {
                if (i == totalChunksSF - 1) {
                    if (file.size % chunkSize > 0) {
                        chunkSize = file.size % chunkSize
                    }
                }

                isRunning = true;

                //$.ajax({
                //    url: VIS.Application.contextUrl + "Attachment/SaveFileinTemp",
                //    dataType: "json",
                //    data: {
                //        file: file.slice(i * chunkSize, i * chunkSize + Number(chunkSize)),
                //        filename: file.name,
                //        folderKey:folder
                //    },
                //    error: function () {
                //        VIS.ADialog.info(VIS.Msg.getMsg('ERRORUploadingFile'));
                //        bsyDiv[0].style.visibility = "hidden";
                //        isRunning=false;
                //    },
                //    success: function (data) {
                //        debugger;
                //        if (data.result.indexOf('ERROR')) {
                //            VIS.ADialog.info(data.result);
                //        }
                //        else {

                //            currentChunk++;
                //            setProgressValue(parseInt((currentChunk / totalChunks) * 80));

                //        }
                //        isRunning=false;
                //    }
                //});




                var xhr = new XMLHttpRequest();
                var fd = new FormData();
                fd.append("file", file.slice(i * chunkSize, i * chunkSize + Number(chunkSize)));
                xhr.open("POST", VIS.Application.contextUrl + "Attachment/SaveFileinTemp/?filename=" + file.name + "&folderKey=" + folder, false);
                xhr.send(fd);

                currentChunk++;
                setProgressValue(parseInt((currentChunk / totalChunks) * 80));
                ////xhr.onreadystatechange = function () {
                ////    debugger;
                ////    if (xhr.readyState == 4 && xhr.status == 200) {
                ////        //document.getElementById("myDiv").innerHTML = xmlhttp.responseText;
                ////        currentChunk++;
                ////        setProgressValue(parseInt((currentChunk / totalChunks) * 80));
                ////        isRunning = false;
                ////    }
                ////}
                ////currentChunk++;
                ////progresBar.attr("value", parseInt((currentChunk / totalChunks) * 80));
                ////animateProgress(parseInt((currentChunk / totalChunks) * 80));
                ////while (isRunning) {
                ////}
            }



        };


        var DownloadFile = function (index, sender) {
            var actionOrigin = VIS.ProcessCtl.prototype.ORIGIN_WINDOW;
            if (!selfi.isWindowAction) {
                actionOrigin = VIS.ProcessCtl.prototype.ORIGIN_FORM;
            }
            bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "Attachment/DownloadAttachment",
                dataType: "json",
                data: {
                    fileName: oldFiles[index].Name,
                    AD_Attachment_ID: AD_Attachment_ID,
                    AD_AttachmentLine_ID: oldFiles[index].Line_ID,
                    actionOrigin: actionOrigin,
                    originName: VIS.context.getWindowContext(selfi.windowNo, "WindowName"),
                    AD_Table_ID: AD_Table_ID,
                    recordID: Record_ID

                },
                error: function () {
                    VIS.ADialog.info('ERRORGettingFile');
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {
                    //debugger;
                    var d = new Date();
                    var filePath = data.result;
                    var url = VIS.Application.contextUrl + "TempDownload/" + filePath + "/" + oldFiles[index].Name + "?" + d.getTime();
                    var fileName = oldFiles[index].Name
                    //$(sender).attr("href", url);
                    //$(sender).attr('download', fileName);
                    //$(sender).attr('target', '_blank');
                    oldFiles[index].HaveSource = true;
                    bsyDiv[0].style.visibility = "hidden";
                    //sender.click();//.trigger('click');
                    window.open(url, '_blank');

                    oldFiles[index].Source = url;
                    //iFrame.attr('src', url);
                    //LoadIframe();

                    //var ev = document.createEvent("MouseEvents");
                    //ev.initMouseEvent("click", true, false, self, 0, 0, 0, 0, 0,
                    // false, false, false, false, 0, null);
                    //sender.dispatchEvent(ev);

                }
            });


        };

        var LoadIframe = function () {
            if (iFrame == null) {
                return;
            }
            //var dTop = $("<div class='attach-file-top'>");
            //var btnRemove = $("<a class='file-close-ico'>");
            //btnRemove.on('click', function () {
            //    iFrame.css('visibility', 'collapse');
            //    viewer.css('visibility', 'collapse');
            //    divFA.css('visibility', 'visible');
            //    iFrame.attr('src', null);
            //});
            //dTop.append(btnRemove);
            //viewer.append(dTop);

            //var dAContent = $("<div class='attach-file-content'>");
            //viewer.append(dAContent);

            //dAContent.append(iFrame);
            iFrame.css('visibility', 'visible');
            viewer.css('visibility', 'visible');
            divFA.css('visibility', 'collapse');
        };

        var setProgressValue = function (value) {
            divGreen.css('width', value + '%');
            lblpercentage.empty();
            lblpercentage.append(value + '%');
        };

        var animateProgress = function (progress) {
            return divProgress.each(function () {
                $(this).animate({
                    width: progress + '%'
                }, {
                    duration: 2000,

                    // swing or linear
                    easing: 'swing',

                    // this gets called every step of the animation, and updates the label
                    step: function (progress) {
                        var labelEl = $('.ui-label', this),
                            valueEl = $('.value', labelEl);

                        if (Math.ceil(progress) < 20 && $('.ui-label', this).is(":visible")) {
                            labelEl.hide();
                        } else {
                            if (labelEl.is(":hidden")) {
                                labelEl.fadeIn();
                            };
                        }

                        if (Math.ceil(progress) == 100) {
                            labelEl.text('Done');
                            setTimeout(function () {
                                labelEl.fadeOut();
                            }, 1000);
                        } else {
                            valueEl.text(Math.ceil(progress) + '%');
                        }
                    },
                    //complete: function (scope, i, elem) {
                    //    if (callback) {
                    //        callback.call(this, i, elem);
                    //    };
                    //}
                });
            });
        };

        var getImageUrl = function (fileName) {
            // debugger;
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
            else if (ext == '.jpg' || ext == '.gif' || ext == '.png') {
                imgUrl += 'image.png';
            }
            else if (ext == '.mp3') {
                imgUrl += 'audio.png';
            }
            else if (ext == '.zip' || ext == '.rar') {
                imgUrl += 'compresed.png';
            }
            else {
                imgUrl += 'defult.png';
            }
            return imgUrl;
        };
    };

    attachmentForm.prototype.setIsWindowAction = function (iswindowAction) {
        this.isWindowAction = iswindowAction;
    };

    VIS.attachmentForm = attachmentForm;
})(VIS, jQuery);