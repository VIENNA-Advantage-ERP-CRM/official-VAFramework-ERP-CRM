/********************************************************
 * Module Name    :     Application
 * Purpose        :     open dialog to add files
 * Author         :     Raghu
 * Date           :     23-july-2015
  ******************************************************/
; (function (VIS, $) {
    function VisImportFiles(allowMutifiles) {
        this.onClose = null;
        var inforoot, $bsyDiv, $divContent, $divTopCon, $divBottomCon, $divBottonRight, btnCancel, btnOK;
        var lstLatestFiles, $fileBrowser, $btnSelectFile, $divFileArea, $divBottomLeft;
        //var $btnUpload;
        var $self = this;
        var isOk = false;
        //file transfer supportive variables
        var currentchunk = 0, currentFile = 0, folder = Date.now().toString() + "_FileCtrl", chunkSize = 1 * 1024 * 1024, totalChunks = 0, currentFileChunkNo = 0, filesInfo = [];

        function initializeComponent(allowMutifiles) {
            inforoot = $("<div style='overflow: hidden;'>");
            $bsyDiv = $('<div class="vis-apanel-busy" style="width: 96%; height: 97%; text-align: center; position: absolute; visibility: hidden;"></div>');

            $divContent = $('<div style="display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;">');
            $divTopCon = $('<div style="padding-left: 7px;height:88.9%;display: inline-block; float: left;width:100%;height:87.8%;">');
            $btnSelectFile = $("<button class='vis-initial-btn vis-initial-btn-blue' style='border: none;'>" + VIS.Msg.getMsg("SelectFile") + "</button>");
            $divFileArea = $("<div class='VIS-form-data' style='width: 100%;height:200px;border: 1px solid #ccc;margin-top: 11px;padding: 10px;overflow:auto;'>");
            $divBottomCon = $('<div style="display: inline-block; float: left;width:100%;height:auto;margin-top: 2px;">');
            $divBottomLeft = $("<div style='float: left;'>");
            // $btnUpload = $("<button class='vis-initial-btn vis-initial-btn-blue' style='margin-top: 5px;margin-left: 7px;border: none; display:none'>" + VIS.Msg.getMsg("Upload") + "</button>");
            $divBottonRight = $('<div style="float:right;">');

            var canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }
            var Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
            btnOK = $("<button class='VIS_Pref_btn-2' disabled='true' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);
            if (allowMutifiles) {
                $fileBrowser = $("<input type='file' multiple='true'>");
            }
            else {
                $fileBrowser = $("<input type='file'>");
            }

            $divTopCon.append($btnSelectFile);
            $divTopCon.append($divFileArea);
            $divContent.append($divTopCon);
            //$divBottomLeft.append($btnUpload);
            $divBottomCon.append($divBottomLeft);
            $divBottonRight.append(btnCancel);
            $divBottonRight.append(btnOK);
            $divBottomCon.append($divBottonRight);
            $divContent.append($divBottomCon);
            inforoot.append($divContent);
            inforoot.append($bsyDiv);

            lstLatestFiles = [];
        };
        initializeComponent(allowMutifiles);

        function bindEvent() {
            btnCancel.on('click', function () {
                isOk = false;
                disposeComponent();
            });

            btnOK.on('click', function () {
                UploadFiles();
            });

            $btnSelectFile.on("click", function () {
                //control retain the last uploaded file path if file path 
                //same then no selection if diffrent only then fire 
                //if (lstLatestFiles.length == 0) {
                //    $fileBrowser.val("");
                //}
                $fileBrowser.val("");
                $fileBrowser.trigger('click');
            });

            $fileBrowser.on("change", function () {
                AppendFile(this);
            });

            //$btnUpload.on('click', function () {
            //    UploadFiles();
            //});
        };

        bindEvent();

        this.show = function () {
            inforoot.dialog({
                width: 500,
                height: 377,
                resizable: false,
                modal: true,
                title: VIS.Msg.getMsg("Upload"),
                close: onClosing
            });
        };

        function onClosing() {
            if (!isOk) {
                folder = "";
            }
            $self.onClose(folder);
            disposeComponent();
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
                            VIS.ADialog.info("fileAlreadyAttached");
                        }, 20);
                        return;
                    }
                }

                var fileInfo = {};
                fileInfo.Name = file.name;
                fileInfo.Size = file.size;
                if (fileInfo.Size > 17000000) {
                    //VIS.ADialog.info(fileInfo.Name + " Sizegreaterthan17MB");
                    continue;
                }

                lstLatestFiles.push(file);
                var divFlewrap = $("<div class='VIS-file-wrap'>");
                var p = $("<p>" + fileInfo.Name + "</p>");
                var index = (lstLatestFiles.length) - 1;
                var closeico = $("<span class='vis vis-mark VIS-close-ico mychoice_" + index + "'  data-index='" + index + "'>");

                closeico.on('click', function () {
                    var parentValue = $(this).parent();
                    if (parentValue) {
                        var para = parentValue.children()[0];
                        if (para) {
                            var index = lstLatestFiles.map(function (item) { return item.name == $(para).text(); }).indexOf(true);
                            lstLatestFiles.splice(index, 1);
                            parentValue.remove();
                        }
                    }

                    //var index = $(this).data("index");
                    //var name = ".mychoice_" + index;
                    //var indexlocal = $divFileArea.find(".VIS-close-ico").index($(name));
                    // lstLatestFiles.splice(indexlocal, 1);
                    //var div = $(this).parent();
                    //$(this).off('click');
                    // div.css("display", "none");
                    // div.empty();
                    // div = null;

                    if (lstLatestFiles.length == 0) {
                        //$btnUpload.css("display", "none");
                        btnOK.attr("disabled", "true");
                    }
                    else {
                        //$btnUpload.css("display", "block");
                        btnOK.removeAttr("disabled");
                    }
                });

                divFlewrap.append(p);
                divFlewrap.append(closeico);
                $divFileArea.append(divFlewrap);

                if (lstLatestFiles.length == 0) {
                    //$btnUpload.css("display", "none");
                    btnOK.attr("disabled", "true");
                }
                else {
                    //$btnUpload.css("display", "block");
                    btnOK.removeAttr("disabled");
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
                resString += ' ' + VIS.Msg.getMsg("VIS_InvalidName") + ' ' + VIS.Msg.getMsg("VIS_ChangeFileName");
                VIS.ADialog.info(resString);
            }
        };

        var UploadFiles = function () {
            $bsyDiv[0].style.visibility = "visible";
            var fileInfo = null;
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
            var isupload = TransferFile();

            //if (isupload == undefined) {
            //    btnOK.removeAttr("disabled");
            //}
            //else {
            //    VIS.ADialog.info("NotUploadedTryAgain");
            //}
        };

        var TransferFile = function () {
            if (lstLatestFiles == null || lstLatestFiles.length == 0) {
                VIS.ADialog.info("NothingToUpload");
                $bsyDiv[0].style.visibility = "hidden";
                return false;
            }
            //start import from server
            if (currentFile >= lstLatestFiles.length) {
                //VIS.ADialog.info("Upload Done");
                $bsyDiv[0].style.visibility = "hidden";
                isOk = true;
                inforoot.dialog('close');
                return true;
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
            //if (currentchunk <= totalChunks) {
            //    setProgressValue(parseInt((currentchunk / totalChunks) * 80));
            //}
            window.setTimeout(function () {
                TransferFile();
            }, 2);
        };

        var disposeComponent = function () {
            btnCancel.off('click');
            btnOK.off('click');
            $bsyDiv = $divContent = $divTopCon = $divBottomCon = $divBottonRight = btnCancel = btnOK = null;
            lstLatestFiles = $fileBrowser = $btnSelectFile = $divFileArea = $divBottomLeft = null;
            currentchunk = currentFile = folder = chunkSize = totalChunks = currentFileChunkNo = filesInfo = null;
            AppendFile = UploadFiles = TransferFile = loadTables = loadLanguage = loadEDL = loadExportScope = null;
            //$btnUpload = null;
            if (inforoot != null) {
                inforoot.dialog('destroy');
                inforoot.remove();
            }
            inforoot = null;
        };

    };
    VIS.VisImportFiles = VisImportFiles;
})(VIS, jQuery);