; (function (VIS, $) {

    /**
     *	Process Interface Controller.
     *
     */
    function ProcessCtl(parent, pi, trx) {
        this.parent = parent;
        this.pi = pi;
        this.windowNo;
        this.paraList = null;
        this.splitUI = parent.splitUI;
        this.parameters = null;
        //this.isPdf = false;
        //this.isCsv = false;
        //this.PAGE_SIZE = VIS.context.ctx["#REPORT_PAGE_SIZE"];
        // this.pageNo = 1;
        //this.fileType = "P";

        this.cansplitUI = function (split) {
            splitUI = split;
        };

        this.getFileType = function () {
            return parent.getFileType();
        }

        this.setBusy = function (busy) {
            parent.setBusy(busy);
        };

        this.isBackground = function () {
            return parent.isBackground();
        };

    };

    ProcessCtl.prototype.REPORT_TYPE_CSV = "C";
    ProcessCtl.prototype.REPORT_TYPE_PDF = "P";
    ProcessCtl.prototype.REPORT_TYPE_RTF = "R";
    ProcessCtl.prototype.REPORT_TYPE_BIHTML = "B";

    ProcessCtl.prototype.ORIGIN_WINDOW = "W";
    ProcessCtl.prototype.ORIGIN_FORM = "F";
    ProcessCtl.prototype.ORIGIN_MENU = "P";

    //ProcessCtl.prototype.setIsPdf = function (ispdf) {
    //    this.isPdf = ispdf;
    //};

    //ProcessCtl.prototype.setIsCsv = function (iscsv) {
    //    this.isCsv = iscsv;
    //};
    //ProcessCtl.prototype.setFileType = function (filetype) {
    //    this.fileType = filetype;
    //};

    /**
	 *	Async Process - Do it all.
	 *  <code>
	 *	- Get Instance ID
	 *	- Get Parameters
	 *	- execute (lock - start process - unlock)
	 *  </code>
	 *  Creates a ProcessCtl instance, which calls
	 *  lockUI and unlockUI if parent is a ASyncProcess
	 *  <br>
	 *	Called from ProcessCtl.startProcess, ProcessDialog.actionPerformed,
	 *  APanel.cmd_print, APanel.actionButton, VPaySelect.cmd_generate
	 *
	 *  @param WindowNo window no
	 */
    ProcessCtl.prototype.process = function (windowNo) {

        this.lock();
        this.windowNo = windowNo;
        this.pi.setWindowNo(windowNo);

        var self = this; //self pointer
        this.jObjectFromServer = null;

        var data = { processInfo: this.pi.toJson() };
        //{
        //    AD_Process_ID: this.pi.getAD_Process_ID(),
        //    Name: this.pi.getTitle(),
        //    AD_Table_ID: this.pi.getTable_ID(),
        //    Record_ID: this.pi.getRecord_ID(),
        //    WindowNo: windowNo,
        //    fileType: this.fileType,
        //    pageSize: this.PAGE_SIZE,
        //    pageNo: this.pageNo,
        //    AD_Window_ID: (this.parent.$parentWindow === undefined ? 0 : this.parent.$parentWindow.AD_Window_ID) // vinay bhatt window id
        //}

        VIS.dataContext.process(data, function (jsonStr) {

            if (jsonStr.error != null) {
                self.pi.setSummary(jsonStr.error, true);
                self.unlock();
                self = null;
                return;
            }
            var json = JSON.parse(jsonStr.result);
            if (json.IsError) {
                self.pi.setSummary(json.Message, true);
                self.unlock();
                self = null;
                return;
            }

            self.complete(json);
            self = null;

        });
    }; //process

    /**
	 *	Execute Process complete.
	 *  Calls unlockUI if parent is a ASyncProcess
     *  @param jObject process data contract from server
	 */
    ProcessCtl.prototype.complete = function (jObject) {
        this.jObjectFromServer = jObject;
        this.pi.setAD_PInstance_ID(jObject.AD_PInstance_ID);
        this.pi.setIsBiHtml(jObject.IsBiHTMlReport)
        // Change Lokesh Chauhan
        this.pi.setCustomHTML(jObject.CustomHTML);
        this.pi.set_AD_PrintFormat_ID(jObject.AD_PrintFormat_ID);
        if (jObject && jObject.ReportProcessInfo) {
            this.pi.setIsReport(jObject.ReportProcessInfo.IsReport);
            this.pi.setTotalPages(jObject.ReportProcessInfo.TotalPage);
            this.pi.setSupportPaging(jObject.ReportProcessInfo.SupportPaging);
            this.pi.setPageSize(jObject.ReportProcessInfo.PageSize);
        }

        if (jObject.ShowParameter) { //Open Paramter Dialog
            this.pi.setAD_PInstance_ID(jObject.AD_PInstance_ID);
            try {
                var pp = new VIS.ProcessParameter(this.pi, this, this.windowNo);
                if (this.splitUI) {
                    this.parent.parameterContainer.append(pp.getRoot());
                    this.parent.setParameterHeights();
                }
                pp.initDialog(jObject.ProcessFields);

                if (this.splitUI) {
                    pp.showCloseIcon(this.parent.getShowParameterCloseIcon());
                }
                else {
                    pp.showDialog();
                    pp = null;
                }
            }
            catch (e) {

                this.pi.setSummary(e.message, true);
            }
            this.unlock();
        }
        else {
            ////this.pi.dispose(); // dispose current pi
            ////if (IsPosReport != "IsPosReport") {
            //  this.pi.dispose(); // dispose current pi
            ////}
            // this.pi = this.pi.fromJson(jObject.ReportProcessInfo);

            ////if (jObject.ReportFilePath && jObject.ReportFilePath.length > 10) { //report
            ////    if (this.parent) {
            ////        this.parent.showReport(new  VIS.PdfViewer());
            ////    }
            ////}

            if (jObject.IsTelerikReport) {
                if (window.KJS) {
                    if (this.parent) {
                        this.parent.showTelerikReport(this.pi.AD_PInstance_ID, this.pi.getTable_ID());
                    }
                }
            }
            else {
                ////if (IsPosReport != "IsPosReport") {
                this.unlock();
                ////}
                ////else {
                ////    this.unlock("IsPosReport");
                ////}

                if (this.parent) {
                    if (jObject.AskForNewTab && jObject.HTML && jObject.HTML.length > 0) {
                        //if (VIS.ADialog.ask("VIS_OpenNewTab")) {
                        //    window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        //}
                        VIS.ADialog.confirm("VIS_OpenNewTab", true, "", "Confirm", function (result) {
                            if (result) {
                                window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                            }
                        });


                    }
                    else if (jObject.ReportFilePath) {

                        var ismobile = /ipad|iphone|ipod|mobile|android|blackberry|webos|windows phone/i.test(navigator.userAgent.toLowerCase());
                        if (ismobile) {
                            // window.setTimeout(function () {
                            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                            // }, 200);                      
                        }
                        else if (!this.parent.setReportBytes) {
                            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        }
                        else {
                            this.parent.setReportBytes(jObject.Report);
                            this.parent.setReportPath(jObject.ReportFilePath);

                            var pdfViewer = null;
                            var ispdf = false;
                            if (jObject.HTML && jObject.HTML.length > 0) {
                                pdfViewer = new VIS.PdfViewer(jObject.HTML, null, true);
                                this.parent.showReport(pdfViewer, jObject, this)
                            }
                            else if (jObject.ReportFilePath && jObject.ReportFilePath.length > 0) {
                                pdfViewer = new VIS.PdfViewer(jObject.ReportFilePath, this.pi)
                                ispdf = true;
                                this.parent.showReport(pdfViewer, jObject, this)
                            }

                            //this.parent.showReport(pdfViewer, jObject, this);// this.windowNo, this.paraList, jObject.AD_Table_ID, ispdf, jObject.TotalRecords, jObject.IsReportFormat, this.PAGE_SIZE, this.pageNo, jObject.AD_ReportView_ID, jObject.IsJasperReport);
                        }
                    }
                    else if (this.pi.getUseCrystalReportViewer() && this.pi.getIsReport()) {
                        //var pdfViewer = new VIS.PdfViewer(null, null, true);
                        //if (this.parent && this.parent.showReport) {
                        //    this.parent.showReport(pdfViewer, jObject, this, true);
                        //}
                        //else {
                        var repV = new VIS.ReportViewerContainer(this);
                        repV.show();
                        //}

                    }
                }
            }
            this.setBusy(false);
            this.dispose();

            /*if (jObject.HTML && jObject.HTML.length > 0) {
                if (this.parent) {
                    if (jObject.AskForNewTab) {
                        //if (VIS.ADialog.ask("VIS_OpenNewTab")) {
                        //    window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        //}
                        VIS.ADialog.confirm("VIS_OpenNewTab", true, "", "Confirm", function (result) {
                            if (result) {
                                window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                            }
                        });


                    }
                    else {

                        var ismobile = /ipad|iphone|ipod|mobile|android|blackberry|webos|windows phone/i.test(navigator.userAgent.toLowerCase());
                        if (ismobile) {
                            // window.setTimeout(function () {
                            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                            // }, 200);                      
                        }
                        else if (!this.parent.setReportBytes) {
                            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        }
                        else {
                            this.parent.setReportBytes(jObject.Report);
                            this.parent.setReportPath(jObject.ReportFilePath);
                            this.parent.showReport(new VIS.PdfViewer(jObject.HTML, null, true), jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID, false, jObject.TotalRecords, jObject.IsReportFormat, this.PAGE_SIZE, this.pageNo, jObject.AD_ReportView_ID, jObject.IsJasperReport);
                        }
                    }
                }
            }
            else {
                if (jObject.ReportFilePath && jObject.ReportFilePath.length > 0) {
                    //window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                    var ismobile = /ipad|iphone|ipod|mobile|android|blackberry|webos|windows phone/i.test(navigator.userAgent.toLowerCase());
                    if (ismobile) {
                        // window.setTimeout(function () {
                        window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        // }, 200);                      
                    }
                    else if (!this.parent.setReportBytes) {
                        window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        //this.parent.setReportBytes(jObject.Report);
                        //this.parent.showReport(new VIS.PdfViewer(jObject.ReportFilePath), jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID, true, jObject.TotalRecords, jObject.IsReportFormat, this.PAGE_SIZE, this.pageNo);
                    }
                    else {
                        this.parent.setReportBytes(jObject.Report);
                        this.parent.setReportPath(jObject.ReportFilePath);
                        this.parent.showReport(new VIS.PdfViewer(jObject.ReportFilePath), jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID, true, jObject.TotalRecords, jObject.IsReportFormat, this.PAGE_SIZE, this.pageNo, jObject.AD_ReportView_ID, jObject.IsJasperReport);

                    }
                }
            }
        }
        //this.dispose();
        ////if (IsPosReport != "IsPosReport") {
            this.dispose();
        ////}*/
            //this.pi.setAD_PInstance_ID(0);
        }
    };

    /**
    *	handle process parameter closed event
    *   - Calls lock and unlock
    *   - call async process excute function
    *   
    *  @param isOk userclick Ok button 
    *  @param gridfield array  for each process parameter  if any
    */
    ProcessCtl.prototype.onProcessDialogClosed = function (isOK, paraList) {
        var getPInstanceIDss = 0;

        if (isOK) { //Ok clicked
            var self = this;
            this.paraList = paraList;
            this.lock();

            ////if (IsPosReport == "IsPosReport") {
            ////    getPInstanceIDss = getPinstanceIdOnOkClick(this.pi.getAD_Process_ID(), this.pi.getTitle(), this.pi.getTable_ID(), this.pi.getRecord_ID(), VIS.Env.getWindowNo(), this.fileType);
            ////}
            ////else {
            ////getPInstanceIDss = this.pi.getAD_PInstance_ID();
            //// }

            var data = { processInfo: self.pi.toJson(), parameterList: paraList }


            //    {
            //    AD_Process_ID: self.pi.getAD_Process_ID(),
            //    AD_PInstance_ID: self.pi.getAD_PInstance_ID(),
            //    Name: self.pi.getTitle(),
            //    AD_Table_ID: self.pi.getTable_ID(),
            //    Record_ID: self.pi.getRecord_ID(),
            //    ParameterList: paraList,
            //    fileType: self.fileType
            //}

            VIS.dataContext.executeProcess(data, function (jsonStr) {
                if (jsonStr.error != null) {
                    self.pi.setSummary(jsonStr.error, true);
                    self.unlock();
                    self = null;
                    return;
                }
                var json = JSON.parse(jsonStr.result);
                if (json.IsError) {
                    self.pi.setSummary(json.Message, true);
                    self.unlock();
                    self = null;
                    return;
                }

                ////self.complete(json); // call process complete
                ////if (IsPosReport == "IsPosReport") {
                ////    self.complete(json, IsPosReport); // call process complete
                ////}
                ////else {
                self.complete(json); // call process complete
                ////}
                if (!this.splitUI) {
                    self = null;
                }

            });
        }
        else {

        }
    };
    /*////
    function getPinstanceIdOnOkClick(AD_Process_IDs, Names, AD_Table_IDs, Record_IDs, pageSizes) {
        var json = 0;
        var data = {
            AD_Process_ID: AD_Process_IDs,
            Name: Names,
            AD_Table_ID: AD_Table_IDs,
            Record_ID: Record_IDs,
            WindowNo: VIS.Env.getWindowNo(),
            fileType: "P",
            pageSize: pageSizes,
            pageNo: 1
        }

        VIS.dataContext.processAsyncFalse(data, function (jsonStr) {
            json = JSON.parse(jsonStr.result);            
        });
        if (json) {
            return getPInstanceIDss = json.AD_PInstance_ID;
        }
        else {
            return json;
        }
    };////*/


    /**
	 *  Unlock UI & dispose 
	 */
    ProcessCtl.prototype.unlock = function () {
        var summary = this.jObjectFromServer ? this.jObjectFromServer.Result : null;
        if (summary != null && summary.indexOf("@") != -1) {
            this.pi.setSummary(VIS.Msg.parseTranslation(VIS.Env.getCtx(), summary));
        }
        else if (summary) {
            this.pi.setSummary(summary);
        }
        if (this.jObjectFromServer)
            this.pi.setError(this.jObjectFromServer.IsError);

        if (this.parent) {
            ////if (IsPosReport == "IsPosReport") {
            ////    this.parent.unlockUI(this.pi, "IsPosReport");
            ////}
            ////else {
            ////    this.parent.unlockUI(this.pi);
            ////}
            this.parent.unlockUI(this.pi);
        }
    };//unlock

    /**
	 *  Lock UI & show Waiting
	 */
    ProcessCtl.prototype.lock = function () {
        if (this.parent)
            this.parent.lockUI(this.pi);
    };//lock

    /**
    *  dispose
    */
    ProcessCtl.prototype.dispose = function () {
        if (!this.splitUI) {
            this.parent = null;
        }
        //if (this.pi)
        //    this.pi.dispose();
        //this.pi = null;
    };



    function PdfViewer(rptName, pi, isHtml) {
        this.bytes = null;
        this.RptFileName = rptName;
        this.Pi = pi;

        var $root = null;
        var $leftDiv = null, $rightDiv = null; $innerRightDiv = null;
        var $object = null;
        var $btn = "";
        var bsyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        function initializedComponenet() {

            $root = $("<div class='vis-height-full'>");
            $rightDiv = $("<div class='vis-height-full'>");
            $rightDiv.css('background-color', 'rgba(var(--v-c-primary), .3)');
            $innerRightDiv = $("<div class='vis-height-full' style='padding:5px;'>");
            $root.append($rightDiv);
            $root.append(bsyDiv);
            if (isHtml) {
                $rightDiv.append($innerRightDiv);

                $innerRightDiv.html(rptName);
            }
            else if (pi.getIsBiHtml())// if report is Bi html report, then send request to respective view
            {
                $object = $("<iframe style = 'width:100%;height:99.4%;'>");
                $object.attr("src", VIS.Application.contextUrl + "BiPanel/GetHTMLReport?info=" + window.encodeURIComponent(rptName));
                $rightDiv.css('height', '95%');
                $rightDiv.css('width', '100%');
                $rightDiv.append($object);
            }
            else {
                $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                $object.attr("src", VIS.Application.contextUrl + rptName);
                $rightDiv.css('height', '95%');
                $rightDiv.css('width', '100%');
                $rightDiv.append($object);
            }
        };

        initializedComponenet();

        //$leftDiv.on("click", function () {
        //    setTimeout(function () {
        //        $object.get(0).contentWindow.focus();
        //        $object.get(0).contentWindow.print();
        //    }, 100);
        //});

        this.getRoot = function () {
            return $root;
        };
        this.getRightDiv = function () {
            return $rightDiv;
        };
        this.getRightInnerDiv = function () {
            return $innerRightDiv;
        };
        this.getIsHtmlReport = function () {
            return isHtml;
        };
        this.getHtml = function () {
            return rptName;
        };

        this.disposeComponent = function () {
            if ($root != null) {
                $root.remove();
                $root = null;
            }
        };
        this.setBusy = function (isBusy) {
            //////if (IsPosReport == "IsPosReport") {
            ////    if (isBusy) {
            ////        VAPOS.SetBusyIndicator[0].style.visibility = 'visible';
            ////    }
            ////    else {
            ////        VAPOS.SetBusyIndicator[0].style.visibility = 'hidden';
            ////    }
            ////}
            ////else {
            bsyDiv.css("display", isBusy ? 'block' : 'none');
            ////}
        };

        this.sizeChanged = function (height, width) {
            // $root.height(height - 40);
            //$root.width(width);
        };

        this.refresh = function () {
            // $root.css("width", "auto");

            //var child = $root.find('.vis-height-full');
            //if (child && child.length > 1) {
            //    $(child[1]).css("width", "auto");
            //}
            if ($object) {
                window.setTimeout(function () {
                    $object.css('height', '99.4%');
                }, 50);
                $object.css('height', $object.height() - 1);
            }
        };


    };



    PdfViewer.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.PdfViewer = PdfViewer;
    //global assignment
    VIS.ProcessCtl = ProcessCtl;

})(VIS, jQuery);