; (function (VIS, $) {

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

    function AProcess(AD_Process_ID, height) {

        var self = null;
        this.parent;
        this.mPanel;
        this.ctx = VIS.Env.getCtx();
        this.isLocked = false;
        this.log = VIS.Logging.VLogger.getVLogger("VIS.AProcess");
        this.isComplete = false;
        this.jpObj = null; //Jsong Process Info Object

        //InitComponenet

        self = this; //self pointer

        //if (IsPosReport == "IsPosReport") {
        //    this.IsPosReport = "IsPosReport";
        //    self.PosAD_Process_ID = PosAD_Process_ID;
        //}

        var $root, $busyDiv, $contentGrid, $table;
        var $btnOK, $btnClose, $text, $cmbType, $lblRptTypeHeader;
        function initilizedComponent() {

            $root = $("<div style='position:relative;width:99%'>");
            $busyDiv = $("<div class='vis-apanel-busy'>");
            $table = $("<table  class='vis-process-table'>");

            $table.height(height);
            $busyDiv.height(height);

            $contentGrid = $("<td>");
            $table.append($("<tr>").append($contentGrid));
            $root.append($table).append($busyDiv); // append to root

            $cmbType = $('<select style="float: left;display:none;width:150px">');
            //$cmbType.append("<option value='S'>" + VIS.Msg.getMsg("Select") + "</option>");
            //$cmbType.append("<option value='P'>" + VIS.Msg.getMsg("OpenPDF") + "</option>");
            //$cmbType.append("<option value='C'>" + VIS.Msg.getMsg("OpenCSV") + "</option>");
            //loadFileTypes();
            $btnOK = $("<button class='vis-button-ok'>").text(VIS.Msg.getMsg("OK"));
            $btnClose = $("<button class='vis-button-close'>").text(VIS.Msg.getMsg("Close"));
            $text = $("<span>").val("asassasasasasaasa");
            //ProcessDialog
            $contentGrid.append($text);
            $lblRptTypeHeader = $('<label style="float: left;margin: 0px 10px 0px 0px;display:none">' + VIS.Msg.getMsg("ChooseReportType") + '</label>');
            var divrptType = $('<div style="display:inherit"></div>');
            divrptType.append($lblRptTypeHeader).append($cmbType);
            $contentGrid.append(divrptType);
            $contentGrid.append($btnOK).append($btnClose);

            //if (IsPosReport == "IsPosReport") {
            //    $btnOK.css("display", "none");
            //    $btnClose.css("display", "none");
            //}

        }
        this.loadFileTypes = function () {

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
                $contentGrid.height(height);
            }
        }
        this.setSize(height);

        //privilized function
        this.getRoot = function () { return $root; };
        this.getContentGrid = function () { return $contentGrid; };
        this.setBusy = function (busy, focus) {
            isLocked = busy;
            if (busy) {
                //		setStatusLine(processing);
                $busyDiv[0].style.visibility = 'visible';// .show();
            }
            else {
                //$busyDiv.hide();
                $busyDiv[0].style.visibility = 'hidden';
                if (focus) {
                    //curGC.requestFocusInWindow();
                }
            }
        };
        this.setMsg = function (msg) {
            ////if ($text == null) {
            ////    $text = $("<span>").val("asassasasasasaasa");
            ////}
            ////if (isnewDiv) {
            ////    // $contentGrid.prepend($('<div style="overflow:auto">').append(msg));

            ////    if ($contentGrid.find('.vis-process-result').length > 0) {
            ////        $($contentGrid.find('.vis-process-result')).eq(0).append(msg);
            ////    }
            ////    else {

            ////        var maxHeight = $contentGrid.height() - ($($contentGrid.find('.vis-button-ok').eq(0)).height() + $($contentGrid.find('span').eq(0)).height());

            ////        $($('<div class="vis-process-result" style="overflow:auto;max-height:' + maxHeight + 'px">').append(msg)).insertBefore('.vis-button-ok');
            ////    }
            ////}
            ////else {
            $text.append(msg);
            var resultTable = $contentGrid.find('.vis-process-result-table');
            var trheight = resultTable.parent().closest('div').height();
            var spanHeight = resultTable.closest('span').height();

            resultTable.css('max-height', (trheight - (spanHeight + 26 + 20 + 33)) + 'px');;
            ////}
        };


        this.reportBytes = null;
        this.reportPath = null;



        this.showReportTypes = function (value) {
            if (value) {
                //if (IsPosReport != "IsPosReport") {
                $cmbType.css('display', 'block');
                $lblRptTypeHeader.css('display', 'block');
                //}
            }
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

    /**
	 *	Dynamic Init
	 *  @return true, if there is something to process (start from menu)
	 */
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
        this.setMsg(json.MessageText);
        this.setBusy(false);

        this.parent = $parent; // this parameter
        this.jpObj = json;
        if (json.IsReport) {
            this.showReportTypes(true);
            //if (this.IsPosReport != "IsPosReport") {
            this.loadFileTypes();
            // }
            // if (this.IsPosReport == "IsPosReport") {
            //     this.onclickTrigger(this, this.IsPosReport);
            // }
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
        VIS.ProcessInfoUtil.setLogFromDB(pi);

        var msg = "";
        msg = "<p><font color=\"" + (pi.getIsError() ? "#FF0000" : "#0000FF") + "\">** " +
            pi.getSummary() + "</font></p>";

        msg += pi.getLogInfo(true);

        this.setMsg(msg);
        console.log(msg);
        //btnOK.IsEnabled = true;
        // this.isComplete = true;
        this.setBusy(false);
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


        function createControls(panel, repObj) {
            var AD_Table_ID = pctl.pi.get_AD_PrintFormat_Table_ID();
            var toolbar = null;

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

            //Create Custom Report ToolBar
            overlay = $('<div>');
            $cmbPages = $('<select class="vis-selectcsview-page">');
            $menu = $("<ul class='vis-apanel-rb-ul'>");
            btnSaveCsvAll = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllRecordCsv") + "'  style='cursor:pointer;'class='vis-report-icon vis-savecsvAll-ico'></a></li>");
            btnsavepdfall = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllPagePdf") + "' style='cursor:pointer;' class='vis-report-icon vis-savepdfALL-ico'></a></li>");
            toolbar = $("<div class='vis-report-header'>").append($('<h3 class="vis-report-tittle" style="float:left;padding-top: 10px;">').append(VIS.Msg.getMsg("Report")));
            btnClose = $('<a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose" style="float:right">');
            actionContainer = $('<div class="vis-report-top-icons" style="float:right;">');
            ulAction = $('<ul style="margin-top: 10px;float:left">');
            btnRF = $("<li><a style='cursor:pointer;margin-top:2px' class='vis-report-icon vis-repformat-ico'></a></li>");

            $menu.append($('<li data-id="-1">').append(VIS.Msg.getMsg('NewReport')));
            overlay.append($menu);
            panel.getRoot().height(self.parent.getContentGrid().height() - 10);
            ulAction.append(btnRF);

            if (canExport) {
                btnSaveCsv = $("<li><a title='" + VIS.Msg.getMsg("SaveCSVPage") + "' style='cursor:pointer;margin-top:1px' class='vis-report-icon vis-savecsv-ico'></a></li>");
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
                btnSavePdf = $("<li><a title='" + VIS.Msg.getMsg("SavePDFDocumentPage") + "' style='cursor:pointer;margin-top:1px' class='vis-report-icon vis-savepdf-ico'></a></li>");
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

            btnArchive = $("<li><a title='" + VIS.Msg.getMsg("Archive") + "' style='cursor:pointer;' class='vis-report-icon vis-archive-ico'></a></li>");
            ulAction.append(btnArchive);
            btnRequery = $("<li><a title='" + VIS.Msg.getMsg("Requery") + "' style='cursor:pointer;' class='vis-report-icon vis-requery-ico'></a></li>");
            ulAction.append(btnRequery);

            btnCustomize = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-customize-ico'></a></li>");
            ulAction.append(btnCustomize);
            btnPrint = $('<li><a style="cursor:pointer;" class="vis-report-icon vis-print-ico"></a></li>');
            ulAction.append(btnPrint);

            //if (  isPdf) {
            if (!repObj.HTML) {
                btnPrint.css('display', 'none');
                btnCustomize.css('display', 'none');
                btnRF.css('display', 'none');
            }
            actionContainer.append(ulAction);
            toolbar.append(btnClose);
            toolbar.append(actionContainer);
            panel.getRoot().css('width', $(window).width());
            self.parent.getContentGrid().append(toolbar).append(panel.getRoot());
            //////this.parent.getContentGrid().append(toolbar).append(panel.getRoot());
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
                        bulkDownloadUI(tp, d.ReportFilePath, bulkdownload,d);
                    }
                }
            };

            function bulkDownloadUI(totalPages, reportUrl, bulkdownload,d) {
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
                            window.open(VIS.Application.contextUrl +  data);
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


            function loadDynamicReportAll(d, bulkdownload,  totalPages) {
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
                            else 
                            {
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
                       
                        var paramData = getGenerateReportPara(queryInfo, "", false,0,0,false);
                       
                        $.ajax({
                            url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                            dataType: "json",
                            type:'POST',
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
                                bulkDownloadUI(totalPages, reportUrl, bulkdownload,d);

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

                    var paramData = getGenerateReportPara( queryInfo, "", false,0,0,false);

                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                        dataType: "json",
                        data: paramData,
                        type:'POST',
                        success: function (data) {
                            if (data == null) {
                                alert(VIS.Msg.getMsg('ERRORGettingRepData'));
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

                    var paramData = getGenerateReportPara(queryInfo, "", false,0,0,false);


                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                        dataType: "json",
                        data: paramData,
                        type:'POST',
                        success: function (data) {
                            if (data == null) {
                                alert(VIS.Msg.getMsg('ERRORGettingRepData'));
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
                                loadDynamicReportAll(d, bulkdownload,  pctl.pi.getTotalPages());
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
                                    getJasperReportAll( d, pctl.pi.getTotalPages(), bulkdownload);
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
                                bulkDownloadUI(totalPages, reportUrl, bulkdownload,d);
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

                var paramData = getGenerateReportPara(queryInfo, query.getCode(0), false0,0,false);


                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                    dataType: "json",
                    data: paramData,
                    type:'POST',
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
            });

        };

        createControls(panel, repObj);
        handleEvents(panel, repObj);
    }

    AProcess.prototype.showReport = function (panel, repObj, pCtl) {
        /* dispose Content */
        if (this.mPanel) {
            this.mPanel.dispose();
            this.mPanel = null;
        }
        this.disposeComponent();

        this.mPanel = panel;
        this.pCtl = pCtl;

        this.createUI(panel, pCtl, repObj);


        //AProcess.prototype.showReport = function (panel, AD_PrintFormat_ID, _pCtl, _winNo, _paraList, _AD_Table_ID, isPdf, totalRecords, isReportFormat, PAGE_SIZE, pageNo, AD_ReportView_ID, IsJasperReport) {


        //handleEvents(this, panel, pctl, repObj);
        panel.setBusy(false);
        this.parent.hideHeader(true);

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
        if (this.mPanel && this.mPanel.sizeChanged) {
            this.mPanel.sizeChanged(height, width);
        }
    };

    AProcess.prototype.refresh = function () {
        if (this.mPanel && this.mPanel.refresh) {
            this.mPanel.refresh();
        }
    };

    //Assignment
    VIS.AProcess = AProcess;

})(VIS, jQuery);