; VIS = window.VIS || {};
(function (VIS, $) {
    VIS.UploadAmountDimension = function (control, _mapping) {

        //**************************Global Variables**********************************************       
        var _result = null;
        var lstLatestFiles = [];
        var oldFiles = [];
        var currentchunk = 0;
        var currentFile = 0;
        var folder = Date.now().toString();
        var chunkSize = 1 * 1024 * 1024;
        var totalChunks = 0;
        var currentFileChunkNo = 0;
        var filesInfo = [];
        var arrayOfCharts = [];
        var file = null;
        var $uploadAttachment = control;
        var windowWidth = $(window).width();
        var heightWidth = $(window).height();
        var folder = Date.now().toString();
        var ExcelMapping = _mapping;
        savedFileName = "";
        var excelResult = null;
        var divProgress = $("<div id='progress_bar' class='vis-ui-progress-bar vis-ui-container' style='top:40%;width: 40%;left: 32%;position:absolute;z-index:99999;'>");
        var divGreen = null;
        var lblpercentage = null;
        this.isWait = false;
        self = this;
        //*************************Global variables************************************************
        //***********************
        //Upload Files
        //***********************
        var UploadFiles = function () {


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
            TransferFile();


        };
        //***********************
        //Transfer Files to server
        //***********************
        var TransferFile = function (e) {

            if (lstLatestFiles == null || lstLatestFiles.length == 0) {
                showProgress(false);
                return;
            }
            if (currentFile >= lstLatestFiles.length) {
                return;
            }


            //Transfer files in chuncks
            var xhr = new XMLHttpRequest();
            var fd = new FormData();
            var fileName = lstLatestFiles[currentFile].name.substring(0, lstLatestFiles[currentFile].name.lastIndexOf(".")) + folder + lstLatestFiles[currentFile].name.substring(lstLatestFiles[currentFile].name.lastIndexOf("."))
            savedFileName = fileName;
            fd.append("file", lstLatestFiles[currentFile].slice(currentFileChunkNo * chunkSize, currentFileChunkNo * chunkSize + Number(chunkSize)));
            xhr.open("POST", VIS.Application.contextUrl + "AmountDivision/SaveFileinTemp?fileName=" + fileName + "&folderKey=" + folder + "&orgFileName=" + lstLatestFiles[currentFile].name, false);
            xhr.send(fd);
            _result = xhr.responseText;
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

        };

        //***********************
        //Append Files
        //***********************
        var AppendFile = function (sender) {

            var invalidFiles = [];
            for (var i = 0; i < sender.files.length; i++) {
                file = sender.files[i];
                if (file == undefined) {
                    return;
                }
               
                if (file.size <= 0) {
                    lstLatestFiles = [];
                    oldFiles = [];
                    currentchunk = 0;
                    currentFile = 0;
                    folder = null;
                    chunkSize = null;
                    totalChunks = 0;
                    currentFileChunkNo = 0;
                    filesInfo = [];
                    file = null;
                    VIS.ADialog.info('VADMS_FileSizeShouldGreaterThan0KB');
                    showProgress(false);
                    return false;
                }
                if (file.name.indexOf('&') > -1 || file.name.indexOf('?') > -1 || file.name.indexOf('#') > -1 || file.name.indexOf('/') > -1 || file.name.indexOf('\\') > -1) {

                    invalidFiles.push(file.name);
                    continue;
                }

                for (var itm in lstLatestFiles) {
                    if (file.name == lstLatestFiles[itm].name) {
                        window.setTimeout(function () {
                            showProgress(false);
                            VIS.ADialog.info('fileAlreadyAttached');

                        }, 20);
                        return;
                    }
                }
                if (oldFiles != null) {
                    for (var itm in oldFiles) {
                        if (file.name == oldFiles[itm].Name && !oldFiles[itm].IsDeleted) {
                            window.setTimeout(function () {
                                showProgress(false);
                                VIS.ADialog.info('fileAlreadyAttached');
                            }, 20);
                            return;
                        }
                    }
                }
                var fileInfo = {};
                fileInfo.Name = file.name;
                fileInfo.Size = file.size;
                lstLatestFiles.push(file);
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
                showProgress(false);
                VIS.ADialog.info("", true, resString);
            }

        };
        //***********************
        //Show progress bar while file is being uplaoded
        //***********************
        var showProgress = function (show) {

            $('body').find("#progressbar").remove();
            if (show) {
                divGreen = $("<div class='vis-ui-progress' style='width: 0%; display: block;position:absolute;height: 35px;'>");
                lblpercentage = $("<span class='vis-ui-label'  style='position:absolute;left: 60%;top: 4px;'>").append("0%");
                divGreen.append(lblpercentage);
                divProgress.append(divGreen);
                var $isBusy = $("<div id='progressbar' style='z-index: 999999;' class='VIS-AMTD-Busy-Progress'></div>");
                $isBusy.append(divProgress);
                $isBusy.appendTo('<body>');
            }
            else {
                $('body').find("progressbar").remove();
            }
        };
        //***********************
        //Set value of progress bar in percentage
        //***********************
        var setProgressValue = function (value) {
            divGreen.css('width', value + '%');
            lblpercentage.empty();
            lblpercentage.append(value + '%');
        };
        //************************
        //Dispose Variables
        //************************
        this.getFolderKey = folder;
        this.getSavedFileName = savedFileName;
        this.GetUploadedExcelColumns = function () {

            if (currentFile >= lstLatestFiles.length) {
               
                var param = [];
                var url = VIS.Application.contextUrl + "contextUrl/SaveAttachmentEntries";
                param.push({ Files: JSON.stringify(filesInfo) });
                param.push({ FolderKey: folder });
                param.push({ FileName: savedFileName });
                param.push({ Mapping: _mapping });
                var result = VIS.ControllerRequestAjaxParameter1(url, param, false);
                _mapping.ExcelCol = result.ExcelCol;
                _mapping.FileName = result.FileName;
                
                return result;
            }
        };
        
        function dispose() {
            lstLatestFiles = [];
            oldFiles = [];
            currentchunk = 0;
            currentFile = 0;
            folder = null;
            totalChunks = 0;
            currentFileChunkNo = 0;
            filesInfo = [];
            file = null;
            $uploadAttachment.off("change");
            if (docObj != null) {
                docObj.Reload();
            }
            filesInfo = [];
            var selectedEffect = "transfer";
            var options = {};
            currentchunk = 0;
            currentFile = 0;
            totalChunks = 0;
            currentFileChunkNo = 0;
        };
        showProgress(true);
        AppendFile(control);
        UploadFiles();
        return _result;
    };
})(VIS, jQuery);