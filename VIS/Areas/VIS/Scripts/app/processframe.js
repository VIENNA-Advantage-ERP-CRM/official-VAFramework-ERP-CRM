﻿; (function (VIS, $) {

    /**
     *	Dialog to Start process.
     *	Displays information about the process
     *		and lets the user decide to start it
     *  	and displays results (optionally print them).
     *  Calls ProcessCtl to execute.
     *
     */
    var baseUrl = VIS.Application.contextUrl;
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var executeQuery = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls.join("/"), param: params };

        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        $.ajax({
            url: nonQueryUrl + 'yWithCode',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(dataIn)
        }).done(function (json) {
            ret = json;
            if (callback) {
                callback(json);
            }
        });

        return ret;
    };

    function AProcess(AD_Process_ID, height, splitUI, extrnalForm) {

        var self = null;
        this.parent;
        this.mPanel;
        this.ctx = VIS.Env.getCtx();
        this.isLocked = false;
        this.log = VIS.Logging.VLogger.getVLogger("VIS.AProcess");
        this.isComplete = false;
        this.jpObj = null; //Jsong Process Info Object
        this.splitUI = splitUI;
        this.extrnalForm = extrnalForm;
        this.partitionContainer = null;
        this.parameterContainer = null;
        this.reportContainer = null;
        this.reportToolbar = null;
        this.isReport = false;
        this.calculateWidth = true;


        this.hideclose = false;
        this.showParameterClose = true;
        this.cssForAll = null;
        this.visRepformatIcons = null;
        this.visSaveCsvIcons = null;
        this.visSavePdfIcons = null;
        this.visArchiveIcons = null;
        this.visRequeryIcons = null;
        this.visCustomizeIcons = null;
        this.visPrintIcons = null;
        this.toolbarColor = null;
        var $divOuterMain = null;
        var $imgToggle = $('<img class="vis-apanel-lb-img" title="Actions" src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/mt24.png">');
        if (this.splitUI) {
            this.partitionContainer = $('<div class="vis-process-outer-wrap">');
            this.parameterContainer = $('<div class="vis-process-left-sidebar">');
            this.reportAreaContainer = $('<div class="vis-process-main-wrap">');
            this.reportToolbar = $('<div class="vis-process-toolbar">');
            this.parameterFixedArea = $('<div class="vis-process-parm-top">');
            this.parameterContainer.append($('<div class="vis-apanel-lb-toggle"></div>').append($imgToggle)).append(this.parameterFixedArea);
            $divOuterMain = $('<div class="vis-process-outer-main-wrap">');

            this.$divDesc = $('<div class="vis-process-description">');
            // <h5></h5>
            //<p></p>
            //</div>
            this.reportContainer = $('<div class="vis-process-result">');
            this.reportAreaContainer.append(this.$divDesc).append(this.reportContainer);

            $divOuterMain.append(this.parameterContainer).append(this.reportAreaContainer);
            this.partitionContainer.append(this.reportToolbar).append($divOuterMain);
            //this.partitionContainer.append(this.parameterContainer).append(this.reportContainer);
            this.showParameterClose = false;

            //this.reportContainer.height(this.reportAreaContainer.height() - (this.$divDesc.height()+25));
        }

        //InitComponenet

        self = this; //self pointer

        //if (IsPosReport == "IsPosReport") {
        //    this.IsPosReport = "IsPosReport";
        //    self.PosAD_Process_ID = PosAD_Process_ID;
        //}

        var $root, $busyDiv, $contentGrid, $table;
        var $btnOK, $btnClose, $text, $cmbType, $lblRptTypeHeader, $chkIsBG, $lblIsBG;
        function initilizedComponent() {

            $root = $("<div style='position:relative;width:100%'>");
            $busyDiv = $("<div class='vis-apanel-busy'>");
            $table = $("<table  class='vis-process-table'>");
            $btnOK = $("<button class='vis-button-ok'>").text(VIS.Msg.getMsg("OK"));
            $btnClose = $("<button class='vis-button-close'>").text(VIS.Msg.getMsg("Close"));
            if (splitUI) {
                $busyDiv.css('top', '0px');
                $btnOK.hide();
                $btnClose.hide();
            }
            $table.height(height);
            $busyDiv.height(height);

            $contentGrid = $("<div>");
            // $table.append($("<tr>").append($contentGrid));
            $root.append($table).append($busyDiv); // append to root

            $divOuterMain.append($busyDiv);

            $cmbType = $('<select style="display:none;width:100%">');
            //$cmbType.append("<option value='S'>" + VIS.Msg.getMsg("Select") + "</option>");
            //$cmbType.append("<option value='P'>" + VIS.Msg.getMsg("OpenPDF") + "</option>");
            //$cmbType.append("<option value='C'>" + VIS.Msg.getMsg("OpenCSV") + "</option>");
            //loadFileTypes();

            $chkIsBG = $('<input type="checkbox" class="vis-process-background">');
            $lblIsBG = $('<label  class="vis-process-background-label">' + VIS.Msg.translate(VIS.context, "IsBackgroundProcess") + '</label>');

            $text = $("<span class='vis-process-description-Span' style='height: 40px;'>").val("asassasasasasaasa");
            //ProcessDialog
            $contentGrid.append($text);
            $lblRptTypeHeader = $('<label style="float: left;margin: 0px 10px 0px 0px;display:none;color:white">' + VIS.Msg.getMsg("ChooseReportType") + '</label>');
            var divrptType = $('<div style="display:inherit"></div>');
            //divrptType.append($lblRptTypeHeader).append($cmbType);
            self.parameterFixedArea.append($lblRptTypeHeader).append($cmbType).append($chkIsBG).append($lblIsBG);

            $contentGrid.append(divrptType);

            var $divBG = $('<div>');
            //$divBG.append($chkIsBG).append($lblIsBG);
            //$contentGrid.append($divBG);

            $contentGrid.append($btnOK).append($btnClose);


            self.$divDesc.append($contentGrid);

            //if (IsPosReport == "IsPosReport") {
            //    $btnOK.css("display", "none");
            //    $btnClose.css("display", "none");
            //}

            //if (self.splitUI) {
            //    self.parent.getContentGrid().append(self.partitionContainer);
            //    //this.reportContainer.append(panel.getRoot());
            //    // this.parameterContainer.append(pctl.parameters.getRoot());

            //}
            ////else {
            //    self.parent.getContentGrid().append(panel.getRoot());
            //}
            //$chkIsBG.on('click', function () {
            //    pi.setIsBackground($chkIsBG.is(':checked'));
            //});


        }

        this.loadFileTypes = function () {
            var sel = this;
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GetReportFileTypes/",
                dataType: "json",
                data: {
                    AD_Process_ID: self.jpObj.AD_Process_ID
                },
                success: function (data) {
                    if (data == null) {
                        return;
                    }
                    var d = jQuery.parseJSON(data);

                    if (d.length == 0) {
                        sel.showReportTypes(false);
                        return;
                    }

                    $cmbType.append("<option value='S'>" + VIS.Msg.getMsg("Select") + "</option>");
                    for (var i = 0; i < d.length; i++) {
                        var opt = $("<option value=" + d[i].Key + ">" + d[i].Name + "</li>");
                        $cmbType.append(opt);
                        opt = null;
                    }


                }

            });

        };
        initilizedComponent();


        this.setSize = function (height, width) {
            if ($root) {
                $root.height(height); //height 
                $busyDiv.height(height);
                //$contentGrid.height(height);
                if (this.setMsg) {
                    this.setMsg("");
                }
            }

        }
        this.setSize(height);

        this.isBackground = function () {
            return $chkIsBG.is(':checked');
        };

        //privilized function
        this.getRoot = function () { return $root; };
        this.getContentGrid = function () { return $contentGrid; };
        this.setBusy = function (busy, focus) {
            isLocked = busy;
            if (busy) {
                //		setStatusLine(processing);
                $busyDiv[0].style.visibility = 'visible';// .show();
                $busyDiv.css('display', '');
            }
            else {
                //$busyDiv.hide();
                $busyDiv[0].style.visibility = 'hidden';
                $busyDiv.css('display', 'none');
                if (focus) {
                    //curGC.requestFocusInWindow();
                }
            }
        };
        this.setMsg = function (msg, width) {

            var $bExist = $text.find('b');

            if ($text.text().length > 0 && msg && msg.length > 0) {
                var $resultSpan = $('<span>' + msg + '</span>');
                this.reportContainer.removeClass('vis-process-result');
                this.reportContainer.addClass('vis-process-detail-result');
                this.reportContainer.append($resultSpan);
                //return;
            }
            else {
                $text.append(msg);
            }
            var $bMore = '';

            if (this.isReport && (!$bExist || $bExist.length <= 0)) {
                $bMore = $('<strong class="vis-process-papapa vis-procss-more"    style="position: fixed;right: 29px;top:96px"><i class="glyphicon glyphicon-chevron-down"></i></strong>');
                $text.append($bMore);
            }

            var $paraG = $($text.find('p')[0]);
            $bMore = $text.find('.vis-process-papapa');
            //if ($paraG && $bMore) {
            //    $paraG.css({ 'width': ($text.width() - $bMore.width() - 10 + 'px'), 'float': 'left' });
            //}
            $paraG.css({ 'float': 'left' });
            if (this.isReport) {
                var paraWidth = $paraG.width();
                if (paraWidth + 45 > $text.width()) {
                    $bMore.show();
                }
                else {
                    $bMore.hide();
                }
            }
            if (this.isReport && (!$bExist || $bExist.length <= 0)) {
                $bMore.on('click', function () {
                    var continerHei = self.reportAreaContainer.height();
                    var $i = $bMore.find('i');
                    if ($i.hasClass('glyphicon-chevron-up')) {
                        $i.addClass('glyphicon-chevron-down');
                        $i.removeClass('glyphicon-chevron-up');
                        $text.css({ 'height': '40px' });
                    }
                    else {
                        $i.addClass('glyphicon-chevron-up');
                        $i.removeClass('glyphicon-chevron-down');
                        $text.css({ 'height': '' });
                    }
                    self.reportContainer.height(continerHei - (self.$divDesc.height() + 20));
                });
            };

            this.setMsgHeight(this.isReport);







            //var resultTable = $contentGrid.find('.vis-process-result-table');
            //var trheight = resultTable.parent().closest('div').height();
            //var spanHeight = resultTable.closest('span').height();

            //resultTable.css('max-height', (trheight - (spanHeight + 26 + 20 + 33)) + 'px');;
            //  resultTable.height(trheight - (spanHeight + 26 + 20 + 33));


            ////}
        };

        this.setMsgHeight = function (isReport) {
            if (isReport) {
                $text.css('height', '40px');
                $text.css('max-height', '');
            }
            else {
                $text.css('height', '');
                if (self.parent) {
                    $text.css('max-height', self.parent.getContentGrid().height() - 100 + 'px');
                }
            }
            this.reportContainer.height(this.reportAreaContainer.height() - (this.$divDesc.height() + 20));
            //if (this.calculateWidth) {
            this.reportAreaContainer.width(this.partitionContainer.width() - (this.parameterContainer.width()));
            //this.calculateWidth = false;
            //}
        };

        this.setParameterHeights = function (height, width) {
            var selff = this;
            var $divOuterWrap = $(selff.partitionContainer.find('.vis-process-outer-main-wrap')[0]);
            if (height) {
                selff.partitionContainer.height(height);
                $divOuterWrap.height(height);
            }

            var wrapperHeight = $divOuterWrap.height();
            var paraRoot = $(selff.partitionContainer.find('.vis-pro-para-root')[0]);// $root.closest('.vis-process-outer-main-wrap').height();
            paraRoot.height((wrapperHeight - 114) + 'px');
            this.reportAreaContainer.width(width - this.parameterContainer.width());

        };

        this.getParametersContainer = function () {
            return this.parameterContainer;
        };

        this.getReportAreaContainer = function () {
            return this.reportContainer;
        };


        this.getContentTable = function () {
            return $table;
        };

        this.getBusyIndicator = function () {
            return $busyDiv;
        };

        this.getToolbar = function () {
            return this.reportToolbar;
        };

        this.showCloseIcon = function (show) {
            if (show) {
                this.hideclose = false;
            }
            else {
                this.hideclose = true;
            }
        };

        this.showOkIcon = function (val) {
            if (val) {
                this.parameterContainer.append($('<div style="overflow:auto;padding:0 10px">').append($btnOK));
                $btnOK.removeClass('');
                $btnOK.addClass('vis-process-ok-btn VIS_Pref_btn-2');
                $btnOK.show();
            }
            else
                $btnOK.hide();
        };


        this.showParameterCloseIcon = function (show) {
            if (show) {
                this.showParameterClose = true;
            }
            else {
                this.showParameterClose = false;
            }
        };

        this.getShowParameterCloseIcon = function () {
            return this.showParameterClose;
        };
        this.setCssForToolbarIcons = function (className) {
            this.cssForAll = className;
        };
        this.setReportFormatIcons = function (className) {
            this.visRepformatIcons = className;
        };
        this.setSaveCsvIcons = function (className) {
            this.visSaveCsvIcons = className;
        };
        this.setSavePdfIcons = function (className) {
            this.visSavePdfIcons = className;
        };
        this.setArchiveIcons = function (className) {
            this.visArchiveIcons = className;
        };
        this.setRequeryIcons = function (className) {
            this.visRequeryIcons = className;
        };
        this.setCustomizeIcons = function (className) {
            this.visCustomizeIcons = className;
        };
        this.setPrintIcons = function (className) {
            this.visPrintIcons = className;
        };
        this.setToolbarColor = function (color) {
            this.toolbarColor = color;
        }

        this.getFileType = function () {
            return $cmbType.val();
        }

        this.reportBytes = null;
        this.reportPath = null;



        this.showReportTypes = function (value) {
            if (value) {
                //if (IsPosReport != "IsPosReport") {
                $cmbType.css('display', 'block');
                $lblRptTypeHeader.css('display', 'block');
                //}
            }
            else {
                $cmbType.css('display', 'none');
                $lblRptTypeHeader.css('display', 'none');
            }
        };

        this.showBackgroundProcess = function (value) {
            if (value) {
                self.parameterFixedArea.addClass('vis-process-pram-checkbox-text');
                $chkIsBG.css('display', 'block');
                $lblIsBG.css('display', 'block');
            }
            else {
                self.parameterFixedArea.removeClass('vis-process-pram-checkbox-text');
                $chkIsBG.css('display', 'none');
                $lblIsBG.css('display', 'none');
            }

        };

        this.enableBackgroundSetting = function (value) {
            $chkIsBG.prop('checked', value);
            if (value == true) {
                $chkIsBG.prop('disabled', 'disabled');
            }
            else {
                $chkIsBG.prop('disabled', '');
            }
        }

        // to Archive Document
        this.archiveDocument = function (panel, pctl) {
            panel.setBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                dataType: "json",
                type: "post",
                data: {
                    AD_Process_ID: pctl.pi.getAD_Process_ID(),
                    Name: pctl.pi.getTitle(),
                    AD_Table_ID: pctl.pi.getTable_ID(),
                    Record_ID: pctl.pi.getRecord_ID(),
                    C_BPartner_ID: 0,
                    isReport: true,
                    binaryData: self.reportBytes,
                    reportPath: self.reportPath
                },
                success: function (data) {
                    VIS.ADialog.info('Archived', true, "", "");
                    panel.setBusy(false);
                }
            });
        };

        $btnOK.on("click", function () { //click event
            self.onclickTrigger(self);
        });

        this.onclickTrigger = function (self) {
            if (self.isComplete) {
                self.dispose();
                return;
            }
            var pi = null;
            //if (self.IsPosReport == "IsPosReport") {//1000553
            //    pi = new VIS.ProcessInfo(self.IsPosReport, self.PosAD_Process_ID, 0, 0);
            //}
            //else {
            pi = new VIS.ProcessInfo(self.jpObj.Name, self.jpObj.AD_Process_ID, 0, 0);
            // }

            // var pi = new VIS.ProcessInfo(self.jpObj.Name, self.jpObj.AD_Process_ID, 0, 0);
            pi.setAD_User_ID(self.ctx.getAD_User_ID());
            pi.setAD_Client_ID(self.ctx.getAD_Client_ID());
            pi.setUseCrystalReportViewer(VIS.context.getIsUseCrystalReportViewer());
            pi.setIsBackground($chkIsBG.is(':checked'));
            pi.setIsReport(self.isReport);

            var ctl = new VIS.ProcessCtl(self, pi, null);

            //if (self.jpObj.defaultFileType) {
            //    pi.setFileType(self.jpObj.defaultFileType);
            //}
            //else {
            pi.setFileType($cmbType.val());
            // }
            //if ($cmbType.val() == "C") {
            //    ctl.setIsPdf(false);
            //    ctl.setIsCsv(true);
            //}
            //else if ($cmbType.val() == pctl.REPORT_TYPE_PDF) {
            //    ctl.setIsPdf(true);
            //    ctl.setIsCsv(false);
            //}

            //           ctl.process(self.windowNo, self.IsPosReport); //call dispose intenally

            if (self.windowNo) {
                ctl.process(self.windowNo); //call dispose intenally
            }
            else {
                ctl.process(VIS.Env.getWindowNo()); //call dispose intenally
            }


            ctl = null;
        };

        $btnClose.on("click", function () {
            self.parent.dispose();
        });

        $imgToggle.on("click", function (e) {
            e.stopPropagation();
            e.preventDefault();
            if (self.parameterContainer.width() > 40) {
                self.parameterContainer.css('flex-basis', '40px');
                self.parameterFixedArea.hide();
                self.getParametersContainer().find('.vis-pro-para-root').hide();
                self.getParametersContainer().find('.vis-process-ok-btn').hide();
            }
            else {
                self.parameterContainer.css('flex-basis', '280px');
                self.parameterFixedArea.show();
                self.getParametersContainer().find('.vis-pro-para-root').show();
                self.getParametersContainer().find('.vis-process-ok-btn').show();
            }
        });

        //this.setReportBytes = function (bytes) {
        //    reportBytes = bytes;
        //};


        this.disposeComponent = function () {
            if ($btnOK)
                $btnOK.off("click");
            if ($btnClose)
                $btnClose.off("click");
            if ($root)
                $root.remove();
            $root = null;
            if ($contentGrid)
                $contentGrid.remove();
            $contentGrid = null;
            //this.partitionContainer.append(this.reportToolbar).append(this.parameterContainer).append(this.reportContainer);
            if (this.reportContainer) {
                this.reportContainer.remove();
            }
            if (this.parameterContainer) {
                this.parameterContainer.remove();
            }
            if (this.reportToolbar) {
                this.reportToolbar.remove();
            }
            if (this.partitionContainer) {
                this.partitionContainer.remove();
            }


            if ($busyDiv)
                $busyDiv.remove();
            $busyDiv = null;
            $btnOK = $btnClose = $text = null;
            this.$parentWindow = null;
            //this.ctx = null;
            this.isComplete = false;
            //this.setSize = null;
            this.jpObj = null;
            self = null;
        }

    };


    AProcess.prototype.init = function (json, $parent, windowNo) {

        //if (json.IsReport) {
        //    VIS.ADialog.info("Process Reoprt is not supported yet");
        //    return fasle;
        //}
        if (json.Name == null || json.Name == "") {
            VIS.ADialog.info("Process Name is empty");
            return fasle;
        }

        this.ctx.setWindowContext(windowNo, "IsSOTrx", json.IsSOTrx);

        $parent.getContentGrid().append(this.getRoot());
        this.isReport = json.IsReport;
        this.setMsg(json.MessageText);
        this.setBusy(false);

        this.parent = $parent; // this parameter
        this.jpObj = json;
        if (json.IsReport) {
            this.showReportTypes(true);
            this.loadFileTypes();
            if (json.IsCrystal && VIS.context.getIsUseCrystalReportViewer()) {
                this.parameterFixedArea.hide();
            }
        }
        else {
            this.showBackgroundProcess(true);
            this.enableBackgroundSetting(json.IsBackground);

        }
        this.setMsgHeight(json.IsReport);




        if (!json.HasPara) {
            this.showOkIcon(true);
        }

        if (this.splitUI && json.HasPara) {
            this.onclickTrigger(this);
        }

        if (this.splitUI) {
            this.parent.getContentGrid().empty();
            this.parent.getContentGrid().append(this.partitionContainer);
        }

        this.windowNo = windowNo;
        return true;
    };

    AProcess.prototype.setTitle = function (title) {
        if (this.parent)
            this.parent.setTitle(VIS.Utility.Util.cleanMnemonic(title));
    };

    /**
	 *  Lock User Interface
	 *  Called from the process before processing
	 *  @param pi process info
	 */



    AProcess.prototype.lockUI = function (pi) {
        this.isLocked = true;
        this.setBusy(true);
        //this.isLocked = true;
        //this.setBusy(true);
    };

    /**
	 *  Unlock User Interface.
	 *  Called from the complete when processing is done
	 *  @param pi process info
	 */
    AProcess.prototype.unlockUI = function (pi) {
        if (pi.logs && pi.logs.length > 0) {
            pi.logs = null;
        }
        VIS.ProcessInfoUtil.setLogFromDB(pi);

        var msg = "";
        if (pi && pi.getSummary()) {
            msg = "<p style='float: left;width: 100%;'><font color=\"" + (pi.getIsError() ? "#FF0000" : "#0000FF") + "\">** " +
                pi.getSummary() + "</font></p>";
        }
        else if (!this.jpObj.IsReport) {
            msg = "<p style='float: left;width: 100%;'><font color=\"" + (pi.getIsError() ? "#FF0000" : "#0000FF") + "\">** " +
             "</font></p>";
        }


        msg += pi.getLogInfo(true);

        this.setMsg(msg);
        //btnOK.IsEnabled = true;
        // this.isComplete = true;
        this.setBusy(false);
        pi.setSummary("");
        this.isLocked = false;
    };

    /**
	 *  Is the UI locked (Internal method)
	 *  @return true, if UI is locked
	 */
    AProcess.prototype.getIsUILocked = function () {
        return this.isLocked;
    };

    /**
	 *  clean up
	 *  @return true, if UI is locked
	 */
    AProcess.prototype.dispose = function () {
        if (this.disposed)
            return;
        this.disposed = true;
        if (this.mPanel) {
            this.mPanel.dispose();
            this.mPanel = null;
        }

        if (this.parent) {
            this.parent.dispose();
            this.parent = null;
        }
        VIS.Env.clearWinContext(this.ctx, this.windowNo);

        this.disposeComponent();
    };

    /* Private functions vars*/
    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var executeReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        var dr = null;
        getDataSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };

    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }

        var value = null;


        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            var dataSet = new VIS.DB.DataSet().toJson(jString);
            if (dataSet.getTable(0).getRows().length > 0) {
                value = dataSet.getTable(0).getRow(0).getCell(0);

            }
            else { value = null; }
            dataSet.dispose();
            dataSet = null;
            if (async) {
                callback(value);
            }
        });

        return value;
    };

    var executeQuery = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls, param: params };

        //   dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        $.ajax({
            url: nonQueryUrl + 'iesWithCode',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            ret = json;
            if (callback) {
                callback(json);
            }
        });

        return ret;
    };

    //DataSet String
    function getDataSetJString(data, async, callback) {
        var result = null;
        //data.sql = VIS.secureEngine.encrypt(data.sql);
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

    //var REPORT_TYPE_CSV = "C";
    //var REPORT_TYPE_PDF = pctl.REPORT_TYPE_PDF;
    //var REPORT_TYPE_RTF = "R";
    /* END Private */

    AProcess.prototype.createUI = function (panel, pctl, repObj) {

        var btnClose = null;
        var actionContainer = null;
        var ulAction = null;
        var btnArchive = null;
        var btnRequery = null;
        var btnCustomize = null;
        var btnPrint = null;
        var btnSaveCsv = null;
        var btnSavePdf = null;
        var btnRF = null;
        var canExport = false;
        var otherPf = [];
        var overlay = null;
        var $cmbPages = null;
        var $menu = null;
        var btnSaveCsvAll = null;
        var btnsavepdfall = null;
        var self = this;
        var toolbar = null;





        function createControls(panel, repObj) {
            var AD_Table_ID = pctl.pi.get_AD_PrintFormat_Table_ID();

            //var partitionContainer = null;
            //var parameterContainer = null;
            //var reportContainer = null;

            canExport = VIS.MRole.getDefault().getIsCanExport(AD_Table_ID);
            var checkName = [];
            var count = -1;
            otherPf = [];
            var dr = null;
            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Form/GetPrintFormatDetails",
                data: { AD_Table_ID: AD_Table_ID },
                success: function (data) {
                    dr = new VIS.DB.DataReader().toJson(data);

                    try {
                        while (dr.read()) {
                            count = parseInt(count) + 1;
                            var name = dr.getString(1);
                            if (checkName.indexOf(name) > -1) {
                                name = name + "_" + (parseInt(count) + 1);
                            }
                            var item = {};
                            item.ID = VIS.Utility.Util.getValueOfInt(dr.get(0));
                            item.Name = name;
                            item.IsDefault = dr.getString(3);
                            checkName.push(dr.getString(1));
                            otherPf.push(item);
                        }
                        dr.close();
                    }
                    catch (e) { dr.close(); }
                    dr = null;
                },
            });

            applyIconCss();
            //if (!self.cssForAll || self.cssForAll.length == 0)
            //{
            //    self.cssForAll = ' vis-report-icon ';
            //}



            //Create Custom Report ToolBar
            overlay = $('<div>');
            $cmbPages = $('<select class="vis-selectcsview-page">');
            $menu = $("<ul class='vis-apanel-rb-ul'>");
            if (self.cssForAll) {
                btnSaveCsvAll = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllRecordCsv") + "'  style='cursor:pointer;'class='" + self.cssForAll + "vis-report-icon vis-savecsvAll-ico'></a></li>");
                btnsavepdfall = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllPagePdf") + "' style='cursor:pointer;' class='" + self.cssForAll + "vis-report-icon vis-savepdfALL-ico'></a></li>");
            }
            else {
                btnSaveCsvAll = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllRecordCsv") + "'  style='cursor:pointer;'class='vis-report-icon vis-savecsvAll-ico'></a></li>");
                btnsavepdfall = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllPagePdf") + "' style='cursor:pointer;' class='vis-report-icon vis-savepdfALL-ico'></a></li>");
            }
            toolbar = $("<div class='vis-report-header'>").append($('<h3 class="vis-report-tittle" style="float:left;padding-top: 10px;">').append(VIS.Msg.getMsg("Report")));

            if (self.toolbarColor) {
                toolbar.css('background-color', self.toolbarColor);
            }



            btnClose = $('<a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose" style="float:right">');
            actionContainer = $('<div class="vis-report-top-icons" style="float:right;">');
            ulAction = $('<ul style="margin-top: 10px;float:left">');
            btnRF = $("<li><a style='cursor:pointer;margin-top:2px' class='" + self.visRepformatIcons + "'></a></li>");

            $menu.append($('<li data-id="-1">').append(VIS.Msg.getMsg('NewReport')));
            overlay.append($menu);
            // panel.getRoot().height(self.parent.getContentGrid().height() - 10);
            //panel.getRoot().height();
            ulAction.append(btnRF);

            if (canExport) {
                btnSaveCsv = $("<li><a title='" + VIS.Msg.getMsg("SaveCSVPage") + "' style='cursor:pointer;margin-top:1px' class='" + self.visSaveCsvIcons + "'></a></li>");
                ulAction.append(btnSaveCsv);
            }
            //if ((isReportFormat) && totalRecords > 0 && canExport) {
            //    ulAction.append(btnSaveCsvAll);

            //    totalPages = parseInt(totalRecords / PAGE_SIZE) + parseFloat((totalRecords % PAGE_SIZE) > 0 ? 1 : 0);
            //    for (var d = 1; d < totalPages + 1; d++) {
            //        $cmbPages.append($('<option value=' + d + '>' + d + '</option>'));
            //    }
            //    actionContainer.append($cmbPages);
            //}
            //else if ((AD_ReportView_ID || IsJasperReport) && totalRecords > 0 && canExport) {
            //    ulAction.append(btnSaveCsvAll);
            //    // ulAction.append(btnSaveCsvAll);
            //    totalPages = totalRecords;
            //    for (var d = 1; d < totalPages + 1; d++) {
            //        $cmbPages.append($('<option value=' + d + '>' + d + '</option>'));
            //    }
            //    actionContainer.append($cmbPages);
            //}
            if (pctl.pi.getSupportPaging() && canExport) {
                ulAction.append(btnSaveCsvAll);
                for (var d = 1; d < pctl.pi.getTotalPages() + 1; d++) {
                    $cmbPages.append($('<option value=' + d + '>' + d + '</option>'));
                }
                actionContainer.append($cmbPages);
            }

            if (canExport) {
                btnSavePdf = $("<li><a title='" + VIS.Msg.getMsg("SavePDFDocumentPage") + "' style='cursor:pointer;margin-top:1px' class='" + self.visSavePdfIcons + "'></a></li>");
                ulAction.append(btnSavePdf);
            }

            if (pctl.pi.getSupportPaging() && canExport) {
                ulAction.append(btnsavepdfall);
            }
            else {
                btnSaveCsv.find('a').attr("title", VIS.Msg.getMsg("SaveCSV"));
                btnSavePdf.find('a').attr("title", VIS.Msg.getMsg("SavePdf"));
            }
            //if ((isReportFormat) && totalRecords > 0 && canExport) {
            //    ulAction.append(btnsavepdfall);
            //}
            //else if ((AD_ReportView_ID) && totalRecords > 0 && canExport) {
            //    ulAction.append(btnSaveCsvAll);
            //    //ulAction.append(btnsavepdfall);
            //}
            //else {
            //    btnSaveCsv.find('a').attr("title", VIS.Msg.getMsg("SaveCSV"));
            //    btnSavePdf.find('a').attr("title", VIS.Msg.getMsg("SavePdf"));
            //}

            if (VIS.context.ctx["#BULK_REPORT_DOWNLOAD"] == 'Y') {
                btnSaveCsvAll.css('display', 'block');
                btnsavepdfall.css('display', 'block');
            }
            else {
                btnSaveCsvAll.css('display', 'none');
                btnsavepdfall.css('display', 'none');
            }

            btnArchive = $("<li><a title='" + VIS.Msg.getMsg("Archive") + "' style='cursor:pointer;' class='" + self.visArchiveIcons + "'></a></li>");
            ulAction.append(btnArchive);
            btnRequery = $("<li><a title='" + VIS.Msg.getMsg("Requery") + "' style='cursor:pointer;' class='" + self.visRequeryIcons + "'></a></li>");
            ulAction.append(btnRequery);

            btnCustomize = $("<li><a style='cursor:pointer;' class='" + self.visCustomizeIcons + "'></a></li>");
            ulAction.append(btnCustomize);
            btnPrint = $("<li><a style='cursor:pointer;' class='" + self.visPrintIcons + "'></a></li>");
            ulAction.append(btnPrint);

            //if (  isPdf) {
            if (!repObj.HTML) {
                btnPrint.css('display', 'none');
                btnCustomize.css('display', 'none');
                btnRF.css('display', 'none');
            }
            actionContainer.append(ulAction);
            if (!self.hideclose) {
                toolbar.append(btnClose);
            }
            toolbar.append(actionContainer);

            if (self.splitUI) {
                self.reportToolbar.css('display', 'flex');
                self.reportToolbar.empty().append(toolbar);
                // self.parent.getContentGrid().append(toolbar).append(this.partitionContainer);
                //self.parent.getContentGrid().prepend(self.reportToolbar)
                self.reportContainer.empty().append(panel.getRoot());
                if (self.extrnalForm) {
                    self.getContentTable().empty().append(self.reportContainer);
                }
                //panel.getRoot().css({ 'width': "100%", 'height': self.getContentTable().height() });
                //self.reportContainer.height(self.reportAreaContainer.height() - (self.$divDesc.height()+25));
                //self.parameterContainer.append(pctl.parameters.getRoot());
            }
            else {
                self.parent.getContentGrid().append(toolbar).append(panel.getRoot());
                panel.getRoot().css('width', $(window).width());
            }

            ////this.parent.getContentGrid().append(toolbar).append(panel.getRoot());
            ////if (selfee.IsPosReport !== "IsPosReport") {

            ////}
            ////else {
            ////    $(toolbar.children()[1]).hide();
            ////    VAPOS.ReportIframe.empty();
            ////    VAPOS.ReportIframe.find("table").remove();
            ////    VAPOS.ReportIframe.append(toolbar.children()).append(panel.getRoot().find("iframe")[0]);
            ////    var tst = VAPOS.ReportIframe.find("h3");
            ////    tst.remove();

            ////    var $lblReportNameRep = $('<h4 class="vis-report-tittle" style="float:left;">' + tst.text() + '</h4>');
            ////    VAPOS.ReportIframe.append($('<div class="VAPOS-ReportHeadRap">').append($lblReportNameRep).append($('<div style="float:right">').append(VAPOS.ReportIframe.find(".vis-report-top-icons"))));
            ////    VAPOS.ReportIframe.append($('<div style="height:100%">').append(VAPOS.ReportIframe.find("iframe")));



            ////    VAPOS.ReportIframe.find(".vis-repformat-ico").removeClass("vis-repformat-ico").addClass("vis-repformat-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-savecsv-ico").removeClass("vis-savecsv-ico").addClass("vis-savecsv-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-savepdf-ico").removeClass("vis-savepdf-ico").addClass("vis-savepdf-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-archive-ico").removeClass("vis-archive-ico").addClass("vis-archive-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-requery-ico").removeClass("vis-requery-ico").addClass("vis-requery-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-customize-ico").removeClass("vis-customize-ico").addClass("vis-customize-ico-pos VAPOS-callfromPOS");
            ////    VAPOS.ReportIframe.find(".vis-print-ico").removeClass("vis-print-ico").addClass("vis-print-ico-pos VAPOS-callfromPOS");
            ////}
            ////try {
            ////    var tables = document.getElementsByClassName('vis-reptabledetect');
            ////    if (tables.length > 0) {
            ////        var tableWidth = 0;
            ////        var tmp = 0;
            ////        for (var i = 0, j = tables.length; i < j; i++) {
            ////            tmp = $(tables[i]).width();
            ////            if (tmp > tableWidth) tableWidth = tmp;
            ////        }
            ////        panel.getRightDiv().width($(window).width() - 10);
            ////        panel.getRightInnerDiv().width(tableWidth + 50);
            ////    }
            ////}
            ////catch (e) { }

            //function callBusyIndicatorPOS(demand, selfContent) {
            //    if (selfContent.find(".VAPOS-callfromPOS").length > 0) {
            //        panel.setBusy(demand, "IsPosReport");
            //    }
            //    else {
            //        panel.setBusy(demand);
            //    }
            //};

        }

        function handleEvents(panel, repObj) {
            var cFrame = self.parent;
            var ismobile = /ipad|iphone|ipod|mobile|android|blackberry|webos|windows phone/i.test(navigator.userAgent.toLowerCase());
            btnClose.off("click");
            btnRequery.off("click");
            btnCustomize.off("click");
            btnPrint.off('click');
            btnRF.off('click');
            $menu.off('click');
            btnArchive.off('click');
            btnClose.on('click', function () {
                if (panel) {
                    panel.dispose();
                    panel = null;
                }
                if (cFrame) {
                    cFrame.dispose();
                    cFrame = null;
                }
                if (pctl.pi)
                    pctl.pi.dispose();
                pctl.pi = null;
                ulAction.empty();
                //toolbar.empty();
                //toolbar.remove();

                btnClose.off("click");
                btnRequery.off("click");
                btnCustomize.off("click");
                btnPrint.off('click');
                btnRF.off('click');
                $menu.off('click');
                //toolbar = null;
                btnClose = null;
                actionContainer = null;
                ulAction = null;
                btnArchive = null;
                btnRequery = null;

                btnCustomize = null;
                btnPrint = null;
                btnSaveCsv = null;
                btnSavePdf = null;
                handleEvents = null;
            });

            function getExeProcessParameter(fileType) {
                pctl.pi.setFileType(fileType);
                var data = { processInfo: pctl.pi.toJson(), parameterList: pctl.paraList }
                return data;
            };

            function getExecuteReportforCsvPara(pageNo, PageSize) {
                var data = {
                    AD_Process_ID: pctl.pi.getAD_Process_ID(),
                    Name: pctl.pi.getTitle(),
                    AD_PInstance_ID: pctl.pi.getAD_PInstance_ID(),
                    AD_Table_ID: pctl.pi.getTable_ID(),
                    Record_ID: pctl.pi.getRecord_ID(),
                    pageNumber: pageNo,
                    page_Size: PageSize,
                    saveAll: false
                }
                return data;
            };

            function getGenerateReportPara(queryInfo, code, isCreateNew, nodeID, treeID, showSummary) {
                var data = {
                    processInfo: pctl.pi.toJson(),
                    queryInfo: JSON.stringify(queryInfo),
                    code: code,
                    isCreateNew: isCreateNew,
                    treeID: treeID,
                    node_ID: nodeID,
                    IsSummary: showSummary
                }
                return data;
            };

            function getExecuteReportComposerData(AD_Process_ID, Name, AD_PInstance_ID, AD_Table_ID, Record_ID, pageNumber, page_Size, printAllPages) {
                var data = {
                    AD_Process_ID: AD_Process_ID,
                    Name: Name,
                    AD_PInstance_ID: AD_PInstance_ID,
                    AD_Table_ID: AD_Table_ID,
                    Record_ID: Record_ID,
                    pageNumber: pageNumber,
                    page_Size: page_Size,
                    printAllPages: printAllPages
                }

                return data;
            };

            function executeProcess(params, callBack) {
                VIS.dataContext.executeProcess(params, function (jsonStr) {
                    if (jsonStr.error != null) {
                        pctl.pi.setSummary(jsonStr.error, true);
                        pctl.unlock();
                        pctl = null;
                        return;
                    }
                    var json = JSON.parse(jsonStr.result);
                    if (json.IsError) {
                        pctl.pi.setSummary(json.Message, true);
                        pctl.unlock();
                        pctl = null;
                        return;
                    }
                    //return json;
                    callBack(json);

                });
            };

            function manageRepTable(json, panel) {
                try {
                    if (json.HTML) {
                        panel.getRightInnerDiv().html(json.HTML);

                        var tables = document.getElementsByClassName('vis-reptabledetect');
                        if (tables.length > 0) {
                            var tableWidth = 0;
                            var tmp = 0;
                            for (var i = 0, j = tables.length; i < j; i++) {
                                tmp = $(tables[i]).width();
                                if (tmp > tableWidth) tableWidth = tmp;
                            }
                            panel.getRightDiv().width($(window).width() - 10);
                            panel.getRightDiv().css('min-width', $(window).width() + 'px');
                            panel.getRightInnerDiv().width(tableWidth + 50);
                        }
                        panel.getRightInnerDiv().width(tableWidth);
                    }
                    else if (json.ReportFilePath) {
                        var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                        $object.attr("src", VIS.Application.contextUrl + json.ReportFilePath);
                        panel.getRightDiv().empty().append($object);
                        self.reportPath = json.ReportFilePath;
                    }
                }
                catch (e) { }
                panel.setBusy(false);
            };

            function loadedReportRendering(d, panel, pNoI, tp, bulkdownload) {
                if (d.HTML && d.HTML.length > 0) { //report
                    manageRepTable(d, panel);
                }
                else {
                    if (tp == 0) {
                        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                        panel.setBusy(false);
                    }
                    else {
                        bulkDownloadUI(tp, d.ReportFilePath, bulkdownload, d);
                    }
                }
            };

            function bulkDownloadUI(totalPages, reportUrl, bulkdownload, d) {
                bulkdownload.setBulkBusy(false);
                $("#forreportdownload").addClass('hide');
                formlinks += '<input type="checkbox" data-num="' + d + '"  data-url="' + VIS.Application.contextUrl + reportUrl + '" checked />';
                pNoI += 1;
                updateCreateBar(((pNoI / totalPages) * 100));
                if (pNoI == totalPages) {
                    setTimeout(function () {
                        updateCreateBar(100);
                        showCreateMessage(VIS.Msg.getMsg("Generated"));
                        $('<form>', {
                            'id': 'download_form',
                            'html': formlinks,
                            'action': '#',
                            'Class': 'hide'
                        }).appendTo(document.body).submit();
                    }, 1000);
                }
            };

            function loadDynamicReport(reportType, pageNo) {
                var data = { processInfo: pctl.pi.toJson() };
                $.ajax({
                    url: VIS.Application.contextUrl + pctl.pi.dynamicAction,
                    type: "Post",
                    data: data,
                    success: function (result) {
                        var data = JSON.parse(result);

                        if (ismobile || reportType != pctl.REPORT_TYPE_PDF) {
                            window.open(VIS.Application.contextUrl + data);
                        }
                        else {
                            var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                            $object.attr("src", VIS.Application.contextUrl + data);
                            panel.getRightDiv().empty().append($object);
                            self.reportPath = data.ReportFilePath;
                        }
                        panel.setBusy(false);
                        return data.ReportFilePath;
                    }
                });
            };


            function loadDynamicReportAll(d, bulkdownload, totalPages) {
                pctl.pi.setPageNo(d);
                var data = { processInfo: pctl.pi.toJson() };
                $.ajax({
                    url: VIS.Application.contextUrl + pctl.pi.dynamicAction,
                    type: "Post",
                    data: data,
                    success: function (result) {
                        result = JSON.parse(result);
                        // reportUrl = "TempDownload/" + reportUrl;
                        bulkDownloadUI(totalPages, result, bulkdownload, d);

                        panel.setBusy(false);
                    }
                });
            };


            btnRequery.on('click', function () {
                panel.setBusy(true);
                panel.getRightInnerDiv().html("");
                panel.getRightInnerDiv().width(0);
                //VIS.dataContext.executeProcess(getExeProcessParameter(pctl, true), function (jsonStr) {
                //    if (jsonStr.error != null) {
                //        pCtl.pi.setSummary(jsonStr.error, true);
                //        pCtl.unlock();
                //        pCtl = null;
                //        return;
                //    }
                //    var json = JSON.parse(jsonStr.result);
                //    if (json.IsError) {
                //        pCtl.pi.setSummary(json.Message, true);
                //        pCtl.unlock();
                //        pCtl = null;
                //        return;
                //    }

                if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                    loadDynamicReport(pctl.REPORT_TYPE_PDF, $cmbPages.val());
                }
                else {

                    //$.when(executeProcess(getExeProcessParameter(pctl.REPORT_TYPE_PDF))).done(function (json) { manageRepTable(json, panel); });
                    //executeProcess(getExeProcessParameter(pctl.REPORT_TYPE_PDF), function (json) {
                    //    manageRepTable(json, panel);
                    //});

                    loadReportData();


                    //////executeProcess(getExeProcessParameter(pctl, true), pCtl, function (json) {
                    //////    //try {
                    //////    //    panel.getRightInnerDiv().html(json.HTML);

                    //////    //    var tables = document.getElementsByClassName('vis-reptabledetect');
                    //////    //    if (tables.length > 0) {
                    //////    //        var tableWidth = 0;
                    //////    //        var tmp = 0;
                    //////    //        for (var i = 0, j = tables.length; i < j; i++) {
                    //////    //            tmp = $(tables[i]).width();
                    //////    //            if (tmp > tableWidth) tableWidth = tmp;
                    //////    //        }
                    //////    //        panel.getRightDiv().width($(window).width() - 10);
                    //////    //        panel.getRightDiv().css('min-width', $(window).width() + 'px');
                    //////    //        panel.getRightInnerDiv().width(tableWidth + 50);
                    //////    //    }
                    //////    //    panel.getRightInnerDiv().width(tableWidth);
                    //////    //}
                    //////    //catch (e) { }
                    //////    //panel.setBusy(false);
                    //////    manageRepTable(json, panel);
                    //////});
                }
                //});

            });
            btnCustomize.on('click', function () {
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_PrintFormat_ID", VIS.Query.prototype.EQUAL, pctl.pi.AD_PrintFormat_ID);
                VIS.viewManager.startWindow(240, zoomQuery);
            });
            btnPrint.on('click', function () {
                if (panel.getIsHtmlReport()) {
                    var mywindow = window.open();
                    mywindow.document.write('<html><head>');
                    mywindow.document.write('</head><body >');
                    mywindow.document.write(panel.getHtml());
                    mywindow.document.write('</body></html>');
                    mywindow.print();
                    mywindow.close();
                }
            });
            if (canExport) {
                btnSaveCsv.on('click', function () {
                    pctl.pi.setFileType("C");
                    if (pctl.pi.getPageNo() == 1) {
                        panel.setBusy(true);
                        executeProcess(getExeProcessParameter(pctl.REPORT_TYPE_CSV), function (json) {
                            window.open(VIS.Application.contextUrl + json.ReportFilePath);
                            panel.setBusy(false);
                        });
                    }
                    else {
                        panel.setBusy(true);

                        window.setTimeout(function () {

                            if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                                loadDynamicReport(pctl.REPORT_TYPE_CSV, pctl.pi.getPageNo());
                            }
                            else {

                                if (pctl.pi.GetAD_ReportView_ID() > 0) {
                                    getCsv($cmbPages.val(), 0, null);
                                }
                                else if (pctl.pi.getIsJasperReport()) {
                                    getJasperReport("C", pctl.pi.getPageNo());
                                }
                                else {
                                    pageNo = $cmbPages.val();
                                    var data = getExecuteReportforCsvPara(pageNo, pctl.pi.getPageSize());
                                    //    {
                                    //    AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                                    //    Name: pCtl.pi.getTitle(),
                                    //    AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
                                    //    AD_Table_ID: pCtl.pi.getTable_ID(),
                                    //    Record_ID: pCtl.pi.getRecord_ID(),
                                    //    pageNumber: pageNo,
                                    //    page_Size: repObj.PAGE_SIZE,
                                    //    saveAll: false
                                    //    //ParameterList: paraList,
                                    //    //csv: false,
                                    //    //pdf: true,
                                    //}

                                    $.ajax({
                                        url: VIS.Application.contextUrl + "ReportCom/ExecuteReportforCsv",
                                        type: "Post",
                                        data: data,
                                        success: function (result) {
                                            var data = JSON.parse(result);
                                            window.open(VIS.Application.contextUrl + "TempDownload/" + data);
                                            panel.setBusy(false);
                                        }
                                    });
                                }
                            }
                        }, 100);
                    }
                });


                btnSavePdf.on('click', function () {
                    ////var getCurrentThis = $(this);
                    pctl.pi.setFileType(pctl.REPORT_TYPE_PDF);
                    if (pctl.pi.getPageNo() == 1) {
                        panel.setBusy(true);

                        ////////executeProcess(getExeProcessParameter(pctl, true), pCtl, function (json) {

                        ////////    if (ismobile) {
                        ////////        window.open(VIS.Application.contextUrl + json.ReportFilePath);
                        ////////    }
                        ////////    else {
                        ////////        var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                        ////////        $object.attr("src", VIS.Application.contextUrl + json.ReportFilePath);
                        ////////        //// getCurrentThis.reportPath = VIS.Application.contextUrl + json.ReportFilePath;
                        ////////        cPanel.getRightDiv().empty().append($object);
                        ////////        self.reportPath = json.ReportFilePath;
                        ////////    }

                        ////////    panel.setBusy(false);
                        ////////});


                        executeProcess(getExeProcessParameter(pctl.REPORT_TYPE_PDF), function (json) {
                            manageRepTable(json, panel);
                        });

                        //});
                    }
                    else {
                        //panel.setBusy(true);

                        //window.setTimeout(function () {


                        //    if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                        //        loadDynamicReport(pctl.REPORT_TYPE_PDF, pctl.pi.getPageNo());
                        //    }
                        //    else {
                        //        if (repObj.AD_ReportView_ID > 0) {
                        //            getPdf($cmbPages.val(), 0, null);
                        //        }
                        //        else if (repObj.IsJasperReport) {
                        //            getJasperReport("PDF", pctl.pi.getPageNo());
                        //        }
                        //        else {
                        //            pageNo = $cmbPages.val();

                        //            var data = getExecuteReportComposerData(pctl.pi.getAD_Process_ID(), pctl.pi.getTitle(), pctl.pi.getAD_PInstance_ID(), pctl.pi.getTable_ID(), pctl.pi.getRecord_ID(), pageNo, pctl.pi.getPageSize(), false);


                        //            $.ajax({
                        //                url: VIS.Application.contextUrl + "ReportCom/ExecuteReport",
                        //                type: "Post",
                        //                data: data,
                        //                success: function (result) {
                        //                    var data = JSON.parse(result);
                        //                    if (ismobile) {
                        //                        window.open(VIS.Application.contextUrl + data);
                        //                    }
                        //                    else {
                        //                        var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                        //                        $object.attr("src", VIS.Application.contextUrl + data);
                        //                        panel.getRightDiv().empty().append($object);
                        //                    }
                        //                    self.reportPath = data;
                        //                    //  window.open(VIS.Application.contextUrl + data);
                        //                    panel.setBusy(false);
                        //                }
                        //            });
                        //        }
                        //    }
                        //}, 200);
                        loadReportData();

                    }
                });

                function loadReportData() {
                    panel.setBusy(true);

                    window.setTimeout(function () {

                        if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                            loadDynamicReport(pctl.REPORT_TYPE_PDF, pctl.pi.getPageNo());
                        }
                        else {
                            if (pctl.pi.GetAD_ReportView_ID() > 0) {
                                getReportData(pctl.pi.getPageNo(), 0, null);
                            }
                            else if (pctl.pi.getIsJasperReport()) {
                                getJasperReport(pctl.REPORT_TYPE_PDF, pctl.pi.getPageNo());
                            }
                            else if (pctl.pi.getIsReportFormat()) {
                                // pageNo = $cmbPages.val();

                                var data = getExecuteReportComposerData(pctl.pi.getAD_Process_ID(), pctl.pi.getTitle(), pctl.pi.getAD_PInstance_ID(), pctl.pi.getTable_ID(), pctl.pi.getRecord_ID(), pctl.pi.getPageNo(), pctl.pi.getPageSize(), false);


                                $.ajax({
                                    url: VIS.Application.contextUrl + "ReportCom/ExecuteReport",
                                    type: "Post",
                                    data: data,
                                    success: function (result) {
                                        var data = JSON.parse(result);
                                        if (ismobile) {
                                            window.open(VIS.Application.contextUrl + data);
                                        }
                                        else {
                                            var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                                            $object.attr("src", VIS.Application.contextUrl + data);
                                            panel.getRightDiv().empty().append($object);
                                        }
                                        self.reportPath = data;
                                        //  window.open(VIS.Application.contextUrl + data);
                                        panel.setBusy(false);
                                    }
                                });
                            }
                            else {
                                executeProcess(getExeProcessParameter(pctl.REPORT_TYPE_PDF), function (json) {
                                    manageRepTable(json, panel);
                                });
                            }
                        }
                    }, 200);
                };

                $cmbPages.on("change", function () {
                    pageNo = $cmbPages.val();

                    if (pctl.pi.GetAD_ReportView_ID() > 0) {
                        panel.setBusy(true);
                        var queryInfo = [];
                        pctl.pi.setPageNo(pageNo);

                        //if (selfee.paralist_ && selfee.paralist_.length > 0) {

                        var rquery = new VIS.Query(pctl.pi.getPrintFormatTableName());

                        if (pctl.pi.getPrintFormatTableName()) {
                            // var query = new VIS.Query(tableName);
                            queryInfo.push(rquery.getTableName());
                            queryInfo.push(rquery.getWhereClause(true));
                        }
                        //}

                        var paramData = getGenerateReportPara(queryInfo, "", false, 0, 0, false);

                        $.ajax({
                            url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                            dataType: "json",
                            type: 'POST',
                            data: paramData,
                            success: function (jsonStr) {

                                var json = jQuery.parseJSON(jsonStr);
                                if (json.IsError) {
                                    pctl.pi.setSummary(json.Message, true);
                                    pctl.unlock();
                                    pctl = null;
                                    return;
                                }


                                //if (json.HTML && json.HTML.length > 0) { //report
                                //    showReport(json.HTML);
                                //}
                                //try {
                                //    panel.getRightInnerDiv().html(json.repInfo.HTML);

                                //    var tables = document.getElementsByClassName('vis-reptabledetect');
                                //    if (tables.length > 0) {
                                //        var tableWidth = 0;
                                //        var tmp = 0;
                                //        for (var i = 0, j = tables.length; i < j; i++) {
                                //            tmp = $(tables[i]).width();
                                //            if (tmp > tableWidth) tableWidth = tmp;
                                //        }
                                //        panel.getRightDiv().width($(window).width() - 10);
                                //        panel.getRightInnerDiv().width(tableWidth + 50);
                                //    }
                                //    panel.getRightInnerDiv().width(tableWidth);

                                //   
                                //}
                                //catch (e) { }
                                //panel.setBusy(false);
                                manageRepTable(json.repInfo, panel);
                                self.reportPath = json.repInfo.ReportFilePath;
                            }
                        });
                    }
                    else {
                        pctl.pi.setPageNo(pageNo);
                    }
                    //else if (repObj.IsJasperReport) {
                    //    panel.setBusy(true);

                    //    window.setTimeout(function () {

                    //        pageNo = $cmbPages.val();

                    //        panel.setBusy(false);

                    //    }, 200);
                    //}

                });
                function getJasperReport(rptExportType, rptPageNo) {
                    pctl.pi.setFileType(rptExportType);
                    var data = { processInfo: pctl.pi.toJson() };

                    $.ajax({
                        url: VIS.Application.contextUrl + "JasperReport/ExecuteReport",
                        type: "Post",
                        data: data,
                        success: function (result) {
                            var data = JSON.parse(result);
                            if (ismobile) {
                                window.open(VIS.Application.contextUrl + data);
                            }
                            else {
                                if (rptExportType == pctl.REPORT_TYPE_PDF) {
                                    var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                                    $object.attr("src", VIS.Application.contextUrl + data);
                                    panel.getRightDiv().empty().append($object);
                                }
                                else {
                                    window.open(VIS.Application.contextUrl + data);

                                }
                                self.reportPath = data;
                            }
                            //  window.open(VIS.Application.contextUrl + data);
                            panel.setBusy(false);
                        }
                    });
                };

                function getJasperReportAll(d, totalPages, bulkdownload) {
                    var reportUrl = "";
                    pctl.pi.setPageNo(d);
                    var data = { processInfo: pctl.pi.toJson() };

                    $.ajax({
                        url: VIS.Application.contextUrl + "JasperReport/ExecuteReport",
                        type: "Post",
                        data: data,
                        success: function (result) {
                            result = JSON.parse(result);
                            // reportUrl = "TempDownload/" + reportUrl;
                            bulkDownloadUI(totalPages, result, bulkdownload, d);

                            panel.setBusy(false);
                        }
                    });
                };





                btnSaveCsvAll.on("click", function () {
                    panel.setBusy(true);
                    pNoI = 0;
                    formlinks = '';
                    pctl.pi.setFileType("C");
                    window.setTimeout(function () {
                        var formlinks = '';
                        var bulkdownload = new VIS.BulkDownload(self.windowNo, 'CSV');
                        bulkdownload.onClose = function () { };
                        bulkdownload.show();
                        if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                            for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                loadDynamicReportAll(d, bulkdownload, pctl.pi.getTotalPages());
                            }
                        }
                        else {
                            if (pctl.pi.GetAD_ReportView_ID() > 0) {
                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    getCsv(d, pctl.pi.getTotalPages(), bulkdownload);
                                }
                            }
                            else if (pctl.pi.getIsJasperReport()) {
                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    getJasperReportAll(d, pctl.pi.getTotalPages(), bulkdownload);
                                }
                            }
                            else {
                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    getAllCSV(d, bulkdownload, pctl.pi.getTotalPages());
                                }
                            }
                        }
                        pctl.pi.setPageNo($cmbPages.val());
                    }, 100);
                });

                function getAllCSV(d, bulkdownload, totalPages) {
                    var reportUrl = "";
                    if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                        reportUrl = loadDynamicReport(pctl.REPORT_TYPE_CSV, d);
                    }
                    else {
                        var data = getExecuteReportforCsvPara(d, pctl.pi.getPageSize());
                        //{
                        //    AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                        //    Name: pCtl.pi.getTitle(),
                        //    AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
                        //    AD_Table_ID: pCtl.pi.getTable_ID(),
                        //    Record_ID: pCtl.pi.getRecord_ID(),
                        //    pageNumber: d,
                        //    page_Size: repObj.PAGE_SIZE,
                        //    saveAll: false
                        //}
                        $.ajax({
                            url: VIS.Application.contextUrl + "ReportCom/ExecuteReportforCsv",
                            type: "Post",
                            data: data,
                            success: function (result) {
                                var reportUrl = JSON.parse(result);
                                //bulkdownload.setBulkBusy(false);
                                //$("#forreportdownload").addClass('hide');
                                //formlinks += '<input type="checkbox" data-num="' + d + '" data-url="' + VIS.Application.contextUrl + "TempDownload/" + data + '" checked />';
                                //pNoI += 1;
                                //updateCreateBar(((pNoI / totalPages) * 100));
                                //if (pNoI == totalPages) {
                                //    setTimeout(function () {
                                //        updateCreateBar(100);
                                //        showCreateMessage(VIS.Msg.getMsg("Generated"));
                                //        $('<form>', {
                                //            'id': 'download_form',
                                //            'html': formlinks,
                                //            'action': '#',
                                //            'Class': 'hide'
                                //        }).appendTo(document.body).submit();
                                //    }, 1000);
                                //}
                                reportUrl = "TempDownload/" + reportUrl;
                                bulkDownloadUI(totalPages, reportUrl, bulkdownload, d);

                                panel.setBusy(false);

                                //selfee.reportPath = "TempDownload/" + data;
                            }
                        });
                    }

                };

                var getReportData = function (pNoI, tP, bulkdownload) {
                    //setBusy(true);
                    //var id = null;
                    //if (processInfo != null) id = processInfo.Key;
                    //else id = Ad_Table_ID;
                    var queryInfo = [];
                    var query = new VIS.Query(pctl.pi.getPrintFormatTableName());
                    queryInfo.push(query.getTableName());
                    queryInfo.push(query.getWhereClause(true));

                    var paramData = getGenerateReportPara(queryInfo, "", false, 0, 0, false);

                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                        dataType: "json",
                        data: paramData,
                        type: 'POST',
                        success: function (data) {
                            if (data == null) {
                                VIS.ADialog.error('ERRORGettingRepData');
                            }
                            var dres = jQuery.parseJSON(data);
                            var d = dres.repInfo;
                            if (d.ErrorText) {
                                VIS.ADialog.error(d.ErrorText);
                                panel.setBusy(true);
                                return;
                            }

                            //if (d.HTML && d.HTML.length > 0) { //report
                            //    showReport(d.HTML);
                            //}
                            //else {
                            //    if (tP == 0) {
                            //        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                            //        panel.setBusy(false);
                            //    }
                            //    else {
                            //        bulkdownload.setBulkBusy(false);
                            //        $("#forreportdownload").addClass('hide');
                            //        formlinks += '<input type="checkbox" data-num="' + pNoI + '"  data-url="' + VIS.Application.contextUrl + d.ReportFilePath + '" checked />';
                            //        pNoI += 1;
                            //        updateCreateBar(((pNoI / tP) * 100));
                            //        if (pNoI == tP) {
                            //            setTimeout(function () {
                            //                updateCreateBar(100);
                            //                showCreateMessage(VIS.Msg.getMsg("Generated"));
                            //                $('<form>', {
                            //                    'id': 'download_form',
                            //                    'html': formlinks,
                            //                    'action': '#',
                            //                    'Class': 'hide'
                            //                }).appendTo(document.body).submit();
                            //            }, 1000);
                            //        }
                            //    }
                            //}

                            loadedReportRendering(d, panel, pNoI, tP, bulkdownload);

                            panel.setBusy(false);
                            self.reportPath = d.ReportFilePath;
                        }
                    });
                };

                var getCsv = function (pNoI, tP, bulkdownload) {
                    var queryInfo = [];
                    var query = new VIS.Query(pctl.pi.getPrintFormatTableName());
                    queryInfo.push(query.getTableName());
                    queryInfo.push(query.getWhereClause(true));

                    var paramData = getGenerateReportPara(queryInfo, "", false, 0, 0, false);


                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                        dataType: "json",
                        data: paramData,
                        type: 'POST',
                        success: function (data) {
                            if (data == null) {
                                VIS.ADialog.error('ERRORGettingRepData');
                            }
                            var dres = jQuery.parseJSON(data);
                            var d = dres.repInfo;
                            if (d.ErrorText) {
                                VIS.ADialog.error(d.ErrorText);
                                setBusy(false);
                                return;
                            }

                            //if (d.HTML && d.HTML.length > 0) { //report
                            //    showReport(d.HTML);
                            //}
                            //else {
                            //    if (tP == 0) {
                            //        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                            //        panel.setBusy(false);
                            //    }
                            //    else {
                            //        bulkdownload.setBulkBusy(false);
                            //        $("#forreportdownload").addClass('hide');
                            //        formlinks += '<input type="checkbox" data-num="' + pNoI + '"  data-url="' + VIS.Application.contextUrl + d.ReportFilePath + '" checked />';
                            //        pNoI += 1;
                            //        updateCreateBar(((pNoI / tP) * 100));
                            //        if (pNoI == tP) {
                            //            setTimeout(function () {
                            //                updateCreateBar(100);
                            //                showCreateMessage(VIS.Msg.getMsg("Generated"));
                            //                $('<form>', {
                            //                    'id': 'download_form',
                            //                    'html': formlinks,
                            //                    'action': '#',
                            //                    'Class': 'hide'
                            //                }).appendTo(document.body).submit();
                            //            }, 1000);
                            //        }
                            //    }
                            //}
                            loadedReportRendering(d, panel, pNoI, tP, bulkdownload);
                            self.reportPath = d.ReportFilePath;
                            panel.setBusy(false);
                        }
                    });
                };
                btnsavepdfall.on("click", function () {
                    panel.setBusy(true);
                    pNoI = 0;
                    formlinks = '';
                    pctl.pi.setFileType(pctl.REPORT_TYPE_PDF);
                    window.setTimeout(function () {

                        var formlinks = '';
                        var bulkdownload = new VIS.BulkDownload(self.windowNo, 'PDF');
                        bulkdownload.onClose = function () { };
                        bulkdownload.show();

                        if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                            for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                loadDynamicReportAll(d, bulkdownload, pctl.pi.getTotalPages());
                            }
                        }
                        else {
                            if (pctl.pi.GetAD_ReportView_ID() > 0) {

                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    getReportData(d, pctl.pi.getTotalPages(), bulkdownload);
                                }

                            }
                            else if (pctl.pi.getIsJasperReport()) {
                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    getJasperReportAll(d, pctl.pi.getTotalPages(), bulkdownload);
                                }
                            }
                            else {
                                for (var d = 1; d <= pctl.pi.getTotalPages() ; d++) {
                                    savePDFAll(d, bulkdownload, pctl.pi.getTotalPages());
                                }
                            }
                        }
                        pctl.pi.setPageNo($cmbPages.val());
                    }, 100);
                });
                var pNoI = 0;
                function savePDFAll(d, bulkdownload, totalPages) {
                    var reportUrl = "";
                    if (pctl.pi.dynamicAction && pctl.pi.dynamicAction.length > 0) {
                        reportUrl = loadDynamicReportAll(pctl.REPORT_TYPE_CSV, d);
                    }
                    else {
                        var data = getExecuteReportComposerData(pctl.pi.getAD_Process_ID(), pctl.pi.getTitle(), pctl.pi.getAD_PInstance_ID(), pctl.pi.getTable_ID(), pctl.pi.getRecord_ID(), d, pctl.pi.getPageSize(), false);


                        $.ajax({
                            url: VIS.Application.contextUrl + "ReportCom/ExecuteReport",
                            type: "Post",
                            data: data,
                            success: function (result) {
                                var reportUrl = JSON.parse(result);
                                //bulkdownload.setBulkBusy(false);
                                //$("#forreportdownload").addClass('hide');
                                //formlinks += '<input type="checkbox" data-num="' + d + '"  data-url="' + VIS.Application.contextUrl + data + '" checked />';
                                //pNoI += 1;
                                //updateCreateBar(((pNoI / totalPages) * 100));
                                //if (pNoI == totalPages) {
                                //    setTimeout(function () {
                                //        updateCreateBar(100);
                                //        showCreateMessage(VIS.Msg.getMsg("Generated"));
                                //        $('<form>', {
                                //            'id': 'download_form',
                                //            'html': formlinks,
                                //            'action': '#',
                                //            'Class': 'hide'
                                //        }).appendTo(document.body).submit();
                                //    }, 1000);
                                //}
                                bulkDownloadUI(totalPages, reportUrl, bulkdownload, d);
                                self.reportPath = reportUrl;
                                panel.setBusy(false);
                            }
                        });
                    }

                };

                var download = function () {
                    for (var i = 0; i < arguments.length; i++) {
                        var iframe = $('<iframe style="visibility: collapse;"></iframe>');
                        $('body').append(iframe);
                        var content = iframe[0].contentDocument;
                        var form = '<form action="' + arguments[i] + '" method="GET"></form>';
                        content.write(form);
                        $('form', content).submit();
                        setTimeout((function (iframe) {
                            return function () {
                                iframe.remove();
                            }
                        })(iframe), 2000);
                    }
                }
                var formlinks = '';
                function updateCreateBar(percent) {
                    var currpercent = $('#reportcreate').find(".progress-bar").width() * 100 / $('#reportcreate').width();
                    if (isNaN(currpercent)) currpercent = 0;
                    if (currpercent <= percent) {
                        $("#reportcreate").removeClass("hide")
                        .find(".progress-bar")
                        .attr("aria-valuenow", percent)
                        .css({
                            width: percent + "%"
                        }).text(percent.toFixed(0) + "%");;
                    }
                }
                function resetCreateMessage() {
                    $("#reportcreateresult")
                    .removeClass()
                    .text("");
                }
                function showCreateMessage(text) {
                    resetCreateMessage();
                    $("#reportcreateresult")
                    .addClass("alert alert-success")
                    .text(text);
                    $("#forreportdownload").removeClass('hide');
                }
                function showCreateError(text) {
                    resetCreateMessage();
                    $("#reportcreateresult")
                    .addClass("alert alert-danger")
                    .text(text);
                }
            }
            btnRF.on('click', function () {
                $menu.empty();
                for (var i = 0; i < otherPf.length; i++) {
                    var className = otherPf[i].IsDefault == "Y" ? "vis-mainfavitem" : "vis-mainnonfavitem";
                    var ulItem = $('<li><a data-isdefbtn="no" data-id="' + otherPf[i].ID + '">' + otherPf[i].Name + '</a></li>');
                    $menu.append(ulItem);
                }
                $menu.append($('<li><a data-id="-1">' + VIS.Msg.getMsg('NewReport') + '</a>'));
                $(this).w2overlay(overlay.clone(true), { css: { height: '300px' } });
            });
            $menu.on('click', "LI", function (e) {

                panel.setBusy(true);

                var btn = $(e.target);
                var id = btn.data("id");
                if (btn.data("isdefbtn") == "yes") {

                    //var sql = "UPDATE AD_PrintFormat SET IsDefault='N' WHERE IsDefault='Y' AND AD_Table_ID=" + Ad_Table_ID + " AND AD_Tab_ID=" + curTab.getAD_Tab_ID();

                    var sqlQry = "VIS_76";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@tableID", pctl.pi.Ad_Table_ID);
                    param[1] = new VIS.DB.SqlParam("@tabID", curTab.getAD_Tab_ID());
                    executeQuery(sql, param);

                    var sqlQry = "VIS_77";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@printForamt", id);
                    executeQuery(sql, param);


                    for (var i = 0; i < otherPf.length; i++) {
                        if (otherPf[i].ID == id) {
                            otherPf[i].IsDefault = "Y";
                        }
                        else {
                            otherPf[i].IsDefault = "N";
                        }
                    }
                    return;
                }
                AD_PrintFormat_ID = id;
                var isCreateNew = false;
                if (id == -1) {
                    id = pctl.pi.get_AD_PrintFormat_Table_ID;
                    isCreateNew = true;
                }
                var queryInfo = [];
                var query = new VIS.Query(pctl.pi.getPrintFormatTableName());
                queryInfo.push(query.getTableName());
                queryInfo.push(query.getWhereClause(true));

                var paramData = getGenerateReportPara(queryInfo, query.getCode(0), false0, 0, false);


                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                    dataType: "json",
                    data: paramData,
                    type: 'POST',
                    success: function (jsonStr) {

                        var json = jQuery.parseJSON(jsonStr);
                        if (json.IsError) {
                            pctl.pi.setSummary(json.Message, true);
                            pctl.unlock();
                            pctl = null;
                            return;
                        }
                        //try {
                        //    panel.getRightInnerDiv().html(json.repInfo.HTML);

                        //    var tables = document.getElementsByClassName('vis-reptabledetect');
                        //    if (tables.length > 0) {
                        //        var tableWidth = 0;
                        //        var tmp = 0;
                        //        for (var i = 0, j = tables.length; i < j; i++) {
                        //            tmp = $(tables[i]).width();
                        //            if (tmp > tableWidth) tableWidth = tmp;
                        //        }
                        //        panel.getRightDiv().width($(window).width() - 10);
                        //        panel.getRightInnerDiv().width(tableWidth + 50);
                        //    }
                        //    panel.getRightInnerDiv().width(tableWidth);
                        //}
                        //catch (e) { }
                        //panel.setBusy(false);

                        manageRepTable(json.repInfo, panel);

                        self.reportPath = json.repInfo.ReportFilePath;
                    }
                });

            });
            btnArchive.on('click', function () {
                self.archiveDocument(panel, pctl);
                //panel.setBusy(true);
                //$.ajax({
                //    url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                //    dataType: "json",
                //    type: "post",
                //    data: {
                //        AD_Process_ID: pctl.pi.getAD_Process_ID(),
                //        Name: pctl.pi.getTitle(),
                //        AD_Table_ID: pctl.pi.getTable_ID(),
                //        Record_ID: pctl.pi.getRecord_ID(),
                //        C_BPartner_ID: 0,
                //        isReport: true,
                //        binaryData: self.reportBytes,
                //        reportPath: self.reportPath
                //    },
                //    success: function (data) {
                //        VIS.ADialog.info('Archived', true, "", "");
                //        panel.setBusy(false);
                //    }
                //});
            });

        };

        function applyIconCss() {

            //this.visRepformatIcons = null;
            //this.visSaveCsvIcons = null;
            //this.visSavePdfIcons = null;
            //this.visArchiveIcons = null;
            //this.visRequeryIcons = null;
            //this.visCustomizeIcons = null;
            //this.visPrintIcons = null;

            if (!self.visRepformatIcons || self.visRepformatIcons.length == 0) {
                if (self.cssForAll) {
                    self.visRepformatIcons = self.cssForAll + ' vis-repformat-ico ';
                }
                else {
                    self.visRepformatIcons = 'vis-repformat-ico vis-report-icon ';
                }

            }


            if (!self.visSaveCsvIcons || self.visSaveCsvIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visSaveCsvIcons = self.cssForAll + ' vis-savecsv-ico ';
                //}
                //else {
                self.visSaveCsvIcons = ' vis-savecsv-ico vis-report-icon ';
                //}

            }
            else {
                self.visSaveCsvIcons = self.visSaveCsvIcons + ' vis-report-icon ';
            }

            if (!self.visSavePdfIcons || self.visSavePdfIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visSavePdfIcons = self.cssForAll + ' vis-savepdf-ico ';
                //}
                //else {
                self.visSavePdfIcons = ' vis-savepdf-ico vis-report-icon ';
                //}

            }
            else {
                self.visSavePdfIcons = self.visSavePdfIcons + ' vis-report-icon ';
            }

            if (!self.visArchiveIcons || self.visArchiveIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visArchiveIcons = self.cssForAll + ' vis-archive-ico ';
                //}
                //else {
                self.visArchiveIcons = ' vis-archive-ico vis-report-icon ';
                //}
            }
            else {
                self.visArchiveIcons = self.visArchiveIcons + ' vis-report-icon ';
            }

            if (!self.visRequeryIcons || self.visRequeryIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visRequeryIcons = self.cssForAll + ' vis-requery-ico ';
                //}
                //else {
                self.visRequeryIcons = ' vis-requery-ico vis-report-icon ';
                //}
            }
            else {
                self.visRequeryIcons = self.visRequeryIcons + ' vis-report-icon ';
            }

            if (!self.visCustomizeIcons || self.visCustomizeIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visCustomizeIcons = self.cssForAll + ' vis-customize-ico ';
                //}
                //else {
                self.visCustomizeIcons = ' vis-customize-ico vis-report-icon ';
                //}
            }
            else {
                self.visCustomizeIcons = self.visCustomizeIcons + ' vis-report-icon ';
            }

            if (!self.visPrintIcons || self.visPrintIcons.length == 0) {
                //if (self.cssForAll) {
                //    self.visPrintIcons = self.cssForAll + ' vis-print-ico ';
                //}
                //else {
                self.visPrintIcons = ' vis-print-ico vis-report-icon ';
                //}

            }
            else {
                self.visPrintIcons = self.visPrintIcons + ' vis-report-icon ';
            }
        }

        createControls(panel, repObj);
        handleEvents(panel, repObj);
    }

    AProcess.prototype.showReport = function (panel, repObj, pCtl, isCrystalReport) {
        /* dispose Content */
        var self = this;
        if (!this.splitUI) {
            if (this.mPanel) {
                this.mPanel.dispose();
                this.mPanel = null;
            }
            this.disposeComponent();
        }
        this.mPanel = panel;
        this.pCtl = pCtl;
        //if (pCtl.pi.getUseCrystalReportViewer() && isCrystalReport) {
        //    var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
        //    $object.attr("src", VIS.Application.contextUrl + 'Areas/VIS/WebPages/CrystalReprotViewer.aspx?title=rpt&aid=' + pCtl.pi.getAD_PInstance_ID() + '&pid=' + pCtl.pi.getAD_Process_ID() + '&tid=' + pCtl.pi.getTable_ID() + '&rid=' + pCtl.pi.getRecord_ID());
        //    //self.reportContainer.empty().append(panel.getRoot());
        //    this.parent.getContentGrid().append(panel.getRoot());
        //    panel.getRightDiv().empty().append($object);
        //}
        //else {
        this.createUI(panel, pCtl, repObj);

        this.parent.hideHeader(true);
        //}
        panel.setBusy(false);

        // Archive Document automatically, if selected on Tenant
        if (VIS.context.ctx["$AutoArchive"] == '1' || VIS.context.ctx["$AutoArchive"] == '2') {
            self.archiveDocument(panel, pCtl);
        }
    };

    AProcess.prototype.showTelerikReport = function (painstanceID, tableID) {
        var tc = new KJS.TelerikContainer(painstanceID, tableID);
        tc.getRoot().height(this.parent.getContentGrid().height());
        this.parent.getContentGrid().empty();
        this.parent.getContentGrid().append(tc.getRoot());
        tc.init();
    };

    AProcess.prototype.setReportBytes = function (bytes) {
        this.reportBytes = bytes;
    };

    AProcess.prototype.setReportPath = function (path) {
        this.reportPath = path;
    };

    AProcess.prototype.sizeChanged = function (height, width) {
        this.setSize(height, width);

        if (this.mPanel && this.mPanel.sizeChanged && !this.splitUI) {
            this.mPanel.sizeChanged(height, width);
        }

        if (this.setParameterHeights) {
            this.setParameterHeights(height, width);
        }
    };

    AProcess.prototype.refresh = function () {
        if (this.setParameterHeights) {
            this.setParameterHeights();
        }

        if (this.mPanel && this.mPanel.refresh) {
            this.mPanel.refresh();
        }
        if (this.setMsg) {
            this.setMsg("");
        }

    };

    //Assignment
    VIS.AProcess = AProcess;

})(VIS, jQuery);