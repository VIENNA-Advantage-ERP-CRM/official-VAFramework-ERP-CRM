/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Report
 * Author         :     Lakhwinder
 * Date           :     2-Nov-2014
  ******************************************************/
; (function (VIS, $) {

    var $root = $('<div class="vis-reportrootdiv">');//, {

    var $menu = $("<ul class='vis-apanel-rb-ul'  style='width:100%;height:100%'>");
    $root.append($menu);
    //var liCSV = $("<li>").append(VIS.Msg.getMsg('OpenCSV'));
    //$menu.append(liCSV);
    //var liPDF = $("<li>").append(VIS.Msg.getMsg('OpenPDF'));
    //$menu.append(liPDF);
    //var liRTF = $("<li>").append(VIS.Msg.getMsg('OpenRTF'));
    //$menu.append(liRTF);


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

    var executeQuery = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls, param: params };
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


    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        if (params) {
            dataIn.param = params;
        }
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


    //DataSet String
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


    function AReport(AD_Table_ID, query, AD_Tab_ID, windowNo, curTab, treeID, treeNodeID, IsSummary) {


        var AD_Client_ID = null;
        var sql = null;
        var dr = null;
        var list = null;
        var tree_ID = treeID;
        var treeNode_ID = treeNodeID;
        var windowID = curTab.getAD_Window_ID();
        var AD_Table_ID = AD_Table_ID;
        var rv = new VIS.ReportViewer(windowNo, curTab);

        if (treeNodeID > 0) {
            rv.setNodeID(treeNodeID);
        }
        if (treeID > 0) {
            rv.setTreeID(treeID);
        }
        if (!IsSummary) {
            rv.showSummary(IsSummary);
        }
        if (AD_Table_ID) {
            rv.setAD_Table_ID(AD_Table_ID);
        }




        var getPrintFormats = function (AD_Table_ID) {
            AD_Client_ID = VIS.context.getAD_Client_ID();
            //sql = "SELECT AD_PrintFormat_ID, Name, AD_Client_ID "
            //       + "FROM AD_PrintFormat "
            //       + "WHERE AD_Table_ID='" + AD_Table_ID + "' AND IsTableBased='Y' ";
            //if (AD_Tab_ID > 0) {
            //    sql = sql + " AND AD_Tab_ID='" + AD_Tab_ID + "' ";
            //}
            //sql = sql + "ORDER BY AD_Client_ID DESC, IsDefault DESC, Name";	//	Own First
            //sql = VIS.MRole.getDefault().addAccessSQL(sql,		//	Own First
            //       "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);



            dr = null;
            var pp = null;
            var list = [];
            try {


                var dr = null;
                $.ajax({
                    type: 'Get',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetPrintFormats",
                    data: { AD_Table_ID: AD_Table_ID, AD_Tab_ID: AD_Tab_ID },
                    success: function (data) {
                        dr = new VIS.DB.DataReader().toJson(data);
                    },
                });

                //dr = executeReader(sql, null, null);
                while (dr.read()) {
                    if (VIS.Utility.Util.getValueOfInt(dr.get(2).toString()) == AD_Client_ID) {
                        pp = {};
                        pp.Key = VIS.Utility.Util.getValueOfInt(dr.get(0));
                        pp.Name = dr.getString(1);
                        list.push(pp);
                    }
                }
                dr.close();

                if (list.length == 0) {
                    if (pp == null) launchReport(null, AD_Table_ID, AD_Table_ID);		//	calls launch
                    else launchReport(pp, 0, AD_Table_ID);
                }
                else launchReport(list[0], 0, AD_Table_ID);
            }
            catch (e) { dr.close(); }
        };



        var launchReport = function (pp, Ad_Table_ID, TableID) {
            var queryInfo = [];
            queryInfo.push(query.getTableName());
            queryInfo.push(query.getWhereClause(true));
            if (AD_Tab_ID) {
                queryInfo.push(AD_Tab_ID.toString());
            }
            var id = null;
            if (pp != null) id = pp.Key;
            else id = Ad_Table_ID;

            var data = rv.getGenerateReportPara(queryInfo, query.getCode(0), (Ad_Table_ID > 0 && AD_Tab_ID > 0), treeNode_ID, tree_ID, IsSummary, 0, 1, "", id, windowID);


            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: data,
                type: 'POST',
                //data: {
                //    id: id,
                //    queryInfo: JSON.stringify(queryInfo),
                //    code: query.getCode(0),
                //    isCreateNew: (Ad_Table_ID > 0 && AD_Tab_ID > 0),
                //    nProcessInfo: JSON.stringify(""),
                //    pdf: false,
                //    csv: false,
                //    nodeID: treeNode_ID,
                //    treeID: tree_ID,
                //    pageNo: 1,
                //    showSummary: IsSummary,
                //    AD_PInstance_ID: 0
                //},
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var dres = jQuery.parseJSON(data);
                    var d = dres.repInfo;
                    if (d.IsError) {
                        VIS.ADialog.error(d.ErrorText);
                        rv.close();
                        return;
                    }
                    if (d.HTML && d.HTML.length > 0) { //report

                        //rv.show(d.HTML, id, pp, TableID, queryInfo, query.getCode(0));
                        rv.show(d.HTML, d.AD_PrintFormat_ID, pp, TableID, queryInfo, query.getCode(0), dres.pSetting);
                        rv.setReportBytes(d.Report);
                        rv.setIteration(dres.pSetting.TotalPage);

                        // Archive Document automatically, if selected on Tenant
                        if (VIS.context.ctx["$AutoArchive"] == '1' || VIS.context.ctx["$AutoArchive"] == '2') {
                            rv.archiveDocument(queryInfo, TableID, d.Report);                           
                        }
                    }
                }
            });
        };
        getPrintFormats(AD_Table_ID);
    };


    // isPrint,showPaging,totalRecords , PAGE_SIZE
    //these variable will contains value only if grid report bound with printformat insteasd of a reasonable print format
    function ReportViewer(windowNo, curTab, isPrint, showPaging, totalRecords, PAGE_SIZE) {
        var $root = $("<div class='vis-reportrootdiv'>");
        var toolbar = null;
        var btnClose = null;
        var actionContainer = null;
        var ulAction = null;
        var btnArchive = null;
        var btnRequery = null;
        var btnSearch = null;
        var btnCustomize = null;
        var btnPrint = null;
        var btnSavePdf = null;
        var btnSaveCsv = null;

        var btnSaveAllPdf = null;
        var btnSaveAllCsv = null;

        var btnSaveCsvAll = null;
        var btnSavePdfAll = null;
        var $cmbPages = null;
        var cFrame = null;
        var html = null;
        var AD_PrintFormat_ID = null;
        var processInfo = null;
        var Ad_Table_ID = null;
        var queryInfo = null;
        var code = null;
        var height = VIS.Env.getScreenHeight() - 50;
        var width = $(window).width();
        var self = this;
        cFrame = new VIS.CFrame();
        cFrame.setName(VIS.Msg.getMsg("Report"));
        cFrame.setTitle(VIS.Msg.getMsg("Report"));
        var btnRF = null;
        var overlay = $('<div>');
        var $menu = $("<ul class='vis-apanel-rb-ul'>");
        overlay.append($menu);
        var canExport = false;
        var otherPf = [];
        var reportBytes = null;
        var node_ID = 0;
        var tree_ID = 0;
        var divPaging, ulPaging, liFirstPage, liPrevPage, liCurrPage, liNextPage, liLastPage, cmbPage;
        var IsSummary = true;
        var pi = null;
        var windowID = curTab.getAD_Window_ID();

        this.getGenerateReportPara = function (queryInfo, code, isCreateNew, nodeID, treeID, showSummary, ad_PInstance_ID, pageNO, fileType, ad_PrintFormat_ID,AD_Window_ID) {

            if (!pi) {
                pi = new VIS.ProcessInfo("", 0, Ad_Table_ID, 0);
                pi.setAD_User_ID(VIS.context.getAD_User_ID());
                pi.setAD_Client_ID(VIS.context.getAD_Client_ID());
            }
            pi.setTable_ID(Ad_Table_ID);
            pi.setAD_PInstance_ID(ad_PInstance_ID);
            pi.setPageNo(pageNO);
            pi.setFileType(fileType);
            pi.set_AD_PrintFormat_ID(ad_PrintFormat_ID);
            pi.setAD_Window_ID(AD_Window_ID);
            if (AD_Window_ID > 0) {
                pi.setActionOrigin(VIS.ProcessCtl.prototype.ORIGIN_WINDOW);
            }
            else {
                pi.setActionOrigin(VIS.ProcessCtl.prototype.ORIGIN_FORM);
            }
            pi.setOriginName(VIS.context.getWindowContext(windowNo, "WindowName"));

            var data = {
                processInfo: pi.toJson(),
                queryInfo: JSON.stringify(queryInfo),
                code: code,
                isCreateNew: isCreateNew,
                treeID: treeID,
                node_ID: nodeID,
                IsSummary: showSummary
            }
            return data;
        };


        this.setNodeID = function (nodeID) {
            node_ID = nodeID;
        }

        this.setTreeID = function (treeID) {
            tree_ID = treeID;
        }

        this.setAD_Table_ID = function (AD_Table_ID) {
            Ad_Table_ID = AD_Table_ID;
        };

        this.showSummary = function (isSummary) {
            IsSummary = isSummary;
        };

        this.setAD_Window_ID = function (windowID) {
            windowID = windowID;
        }

        this.setReportBytes = function (bytes) {
            reportBytes = bytes;
        };

        var Iteration = 0;
        this.setIteration = function (iteration) {
            Iteration = iteration;
        };

        // to Archive Document
        this.archiveDocument = function (queryInfo, Ad_Table_ID, reportBytes) {
            setBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                dataType: "json",
                type: "post",
                data: {
                    AD_Process_ID: 0,
                    Name: queryInfo[0],
                    AD_Table_ID: Ad_Table_ID,
                    Record_ID: 0,
                    C_BPartner_ID: 0,
                    isReport: true,
                    binaryData: reportBytes
                },
                success: function (data) {
                    VIS.ADialog.info('Archived', true, "", "");
                    setBusy(false);
                }
            });
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

        // bulk download upate progress bar percentage wise
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
                }).text(percent.toFixed(0) + "%");
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

        // bulk download page no variable
        var pNoI = 0;

        this.getRoot = function () {
            return $root;
        };
        this.dispose = function () {
        };
        this.show = function (content, _AD_PrintFormat_ID, _processInfo, _Ad_Table_ID, _queryInfo, _code, pagesetting) {

            processInfo = _processInfo;
            Ad_Table_ID = _Ad_Table_ID;
            queryInfo = _queryInfo;
            AD_PrintFormat_ID = _AD_PrintFormat_ID;
            code = _code;
            canExport = VIS.MRole.getDefault().getIsCanExport(Ad_Table_ID);
            if (isPrint != true) {
                if (canExport) {
                    btnSaveCsv.css('display', 'inline-flex');
                    btnSavePdf.css('display', 'inline-flex');

                    if (VIS.context.ctx["#BULK_REPORT_DOWNLOAD"] == 'Y') {
                        btnSaveAllCsv.css('display', 'inline-flex');
                        btnSaveAllPdf.css('display', 'inline-flex');
                    }
                    else {
                        btnSaveAllCsv.css('display', 'none');
                        btnSaveAllPdf.css('display', 'none');
                    }
                }
                else {
                    btnSaveCsv.css('display', 'none');
                    btnSavePdf.css('display', 'none');

                    btnSaveAllCsv.css('display', 'none');
                    btnSaveAllPdf.css('display', 'none');
                }
            }
            showReport(content);
            if (pagesetting) {
                resetPageCtrls(pagesetting);
            }
        }

        var disposeComponant = function () {

            if (isPrint) {
                var iFrame = $root.children()[1];
                //iFrame.empty();
                iFrame = null;
            }
            liFirstPage.off("click");
            liPrevPage.off("click");
            liNextPage.off("click");
            liLastPage.off("click");
            cmbPage.off("change");

            $root.empty();
            $root.remove();
            if (ulAction) {
                ulAction.empty();
            }
            toolbar.empty();
            toolbar.remove();
            if (cFrame) {
                cFrame.dispose();
                cFrame = null;
            }

            liFirstPage = liPrevPage = liNextPage = liLastPage = cmbPage = null;

            $root = null;
            toolbar = null;
            btnClose = null;
            actionContainer = null;
            ulAction = null;
            btnArchive = null;
            btnRequery = null;
            btnSearch = null;
            btnCustomize = null;
            btnPrint = null;
        };
        this.close = function () {
            disposeComponant();
        };
        var createHeader = function () {


            //if (VIS.Application.isRTL) {
            //    toolbar = $("<div class='vis-report-header'>").append($('<h3 class="vis-report-tittle">').append(VIS.Msg.getMsg("Report")));
            //    btnClose = $('<a href="javascript:void(0)" class="vis-icon-menuclose vis vis-cross">');
            //    actionContainer = $('<div class="vis-report-top-icons">');
            //    ulAction = $('<ul class="vis-reporticonsul">');
            //    actionContainer.append(ulAction);
                //if (showPaging == true) {


                //    $cmbPages = $('<select class="vis-selectcsview-page">');
                //    var totalPages = parseInt(totalRecords / PAGE_SIZE) + parseFloat((totalRecords % PAGE_SIZE) > 0 ? 1 : 0);

                //    for (var d = 1; d < totalPages + 1; d++) {
                //        $cmbPages.append($('<option value=' + d + '>' + d + '</option>'));
                //    }
                //    ulAction.append($('<li>').append($cmbPages));
                //    btnsavepdfall = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllPagePdf") + "' style='cursor:pointer;' class='vis-report-icon vis-savepdf-ico'></a></li>");
                //    ulAction.append(btnsavepdfall);
                //    btnSaveCsvAll = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllRecordCsv") + "'  style='cursor:pointer;'class='vis-report-icon vis-savecsvAll-ico'></a></li>");
                //    ulAction.append(btnSaveCsvAll);



                //}
                //if (isPrint != true) {

            //    btnPrint = $('<li><a title="' + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Print")) + '" class="vis vis-print"></a></li>');
            //    ulAction.append(btnPrint);
            //    btnCustomize = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("PrintCustomize")) + "' class='vis vis-customize'></a></li>");
            //    ulAction.append(btnCustomize);
            //    btnSearch = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Search")) + "' class='vis vis-find'></a></li>");
            //    ulAction.append(btnSearch);
            //    btnRequery = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Requery")) + "' class='vis vis-refresh'></a></li>");
            //    ulAction.append(btnRequery);
            //    btnArchive = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Archive")) + "' class='vis vis-archive'></a></li>");
            //    ulAction.append(btnArchive);
            //    // bulk download icon pdf
            //    btnSaveAllPdf = $("<li style='display:none'><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveAllPagePdf")) + "' class='vis vis-pdf-all'></a></li>");
            //    ulAction.append(btnSaveAllPdf);

            //    btnSavePdf = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SavePdf")) + "' class='vis vis-save-pdf'></a></li>");
            //    ulAction.append(btnSavePdf);
            //    // bulk download icon csv
            //    btnSaveAllCsv = $("<li style='display:none'><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveAllRecordCsv")) + "' class='vis vis-csv-all'></a></li>");
            //    ulAction.append(btnSaveAllCsv);

            //    btnSaveCsv = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveCSV")) + "' class='vis vis-save-csv'></a></li>");
            //    ulAction.append(btnSaveCsv);
            //    btnRF = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("ReportFormat")) + "' class='vis vis-format'></a></li>");
            //    ulAction.append(btnRF);

            //    // }

            //    divPaging = $("<div class='vis-report-top-icons'>");
            //    createPageSettings();
            //    divPaging.append(ulPaging);


            //    toolbar.append(divPaging);
            //    toolbar.append(actionContainer);
            //    toolbar.append(btnClose);
            //    $root.append(toolbar);
            //}
            //else {
                toolbar = $("<div class='vis-report-header' style='padding: 0;'>").append($('<h3 class="vis-report-tittle">').append(VIS.Msg.getMsg("Report")));
                btnClose = $('<a href="javascript:void(0)" class="vis-icon-menuclose vis vis-cross">');
                actionContainer = $('<div class="vis-report-top-icons">');
                ulAction = $('<ul class="vis-reporticonsul">');
                actionContainer.append(ulAction);

                btnRF = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("ReportFormat")) + "'  class='vis vis-format'></a></li>");
                ulAction.append(btnRF);
                btnSaveCsv = $("<li><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveCSV")) + "' class='vis vis-save-csv'></a></li>");
                ulAction.append(btnSaveCsv);

                // bulk download icon csv
                btnSaveAllCsv = $("<li style='display:none'><a  title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveAllRecordCsv")) + "' class='vis vis-csv-all'></a></li>");
                ulAction.append(btnSaveAllCsv);

                btnSavePdf = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SavePdf")) + "' class='vis vis-save-pdf'></a></li>");
                ulAction.append(btnSavePdf);

                // bulk download icon pdf
                btnSaveAllPdf = $("<li style='display:none'><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("SaveAllPagePdf")) + "' class='vis vis-pdf-all'></a></li>");
                ulAction.append(btnSaveAllPdf);

                btnArchive = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Archive")) + "' class='vis vis-archive'></a></li>");
                ulAction.append(btnArchive);
                btnRequery = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Requery")) + "' class='vis vis-refresh'></a></li>");
                ulAction.append(btnRequery);
                btnSearch = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Search")) + "' class='vis vis-find'></a></li>");
                ulAction.append(btnSearch);
                btnCustomize = $("<li><a title='" + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("PrintCustomize")) + "' class='vis vis-customize'></a></li>");
                ulAction.append(btnCustomize);
                btnPrint = $('<li><a title="' + VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Print")) + '" class="vis vis-print"></a></li>');
                ulAction.append(btnPrint);
                //}
                //if (showPaging == true) {
                //    btnSaveCsvAll = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllRecordCsv") + "'  style='cursor:pointer;'class='vis-report-icon vis-savecsvAll-ico'></a></li>");
                //    ulAction.append(btnSaveCsvAll);

                //    btnsavepdfall = $("<li><a  title='" + VIS.Msg.getMsg("SaveAllPagePdf") + "' style='cursor:pointer;' class='vis-report-icon vis-savepdf-ico'></a></li>");
                //    ulAction.append(btnsavepdfall);

                //    $cmbPages = $('<select class="vis-selectcsview-page">');
                //    var totalPages = parseInt(totalRecords / PAGE_SIZE) + parseFloat((totalRecords % PAGE_SIZE) > 0 ? 1 : 0);

                //    for (var d = 1; d < totalPages + 1; d++) {
                //        $cmbPages.append($('<option value=' + d + '>' + d + '</option>'));
                //    }
                //    ulAction.append($('<li>').append($cmbPages))
                //}

                divPaging = $("<div class='vis-report-top-icons'>");
                createPageSettings();
                divPaging.append(ulPaging);


                toolbar.append(divPaging);
                toolbar.append(actionContainer);
                toolbar.append(btnClose);
                $root.append(toolbar);
            //}
            bindEvents();
        };

        var bindEvents = function () {
            btnClose.on('click', function () {
                disposeComponant();
            });
            //if (isPrint != true) {
            btnPrint.on('click', function () {
                printReport();
            });
            btnCustomize.on('click', function () {
                //var AD_Window_ID = 240;		// hardcoded               
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_PrintFormat_ID", VIS.Query.prototype.EQUAL, AD_PrintFormat_ID);
                VIS.viewManager.startWindow(240, zoomQuery);
            });
            btnRequery.on('click', function () {
                subContentPane.empty();
                subContentPane.css('width', '0px');
                launchReport(cmbPage.val());
            });

            btnSavePdf.on('click', function () {
                getPdf(cmbPage.val(), 0), null;
            });
            btnSaveCsv.on('click', function () {
                getCsv(cmbPage.val(), 0, null);
            });

            // for bulk download pdf
            btnSaveAllPdf.on('click', function () {
                pNoI = 0;
                formlinks = '';
                var bulkdownload = new VIS.BulkDownload(windowNo, 'PDF');
                //bulkdownload.onClose = function () { };
                bulkdownload.show();
                for (var i = 1; i <= Iteration; i++) {
                    getPdf(i, Iteration, bulkdownload);
                }
            });

            // for bulk download csv
            btnSaveAllCsv.on('click', function () {
                pNoI = 0;
                formlinks = '';
                var bulkdownload = new VIS.BulkDownload(windowNo, 'CSV');
                //bulkdownload.onClose = function () { };
                bulkdownload.show();
                for (var i = 1; i <= Iteration; i++) {
                    getCsv(i, Iteration, bulkdownload);
                }
            });

            btnRF.on('click', function () {
                $menu.empty();
                for (var i = 0; i < otherPf.length; i++) {
                    if (otherPf[i].ID) {
                        var className = otherPf[i].IsDefault == "Y" ? "vis-favitmchecked" : "vis-favitmunchecked";
                        var ulItem = $('<li data-isdefbtn="no" data-id="' + otherPf[i].ID + '"><a data-isdefbtn="no" data-id="' + otherPf[i].ID + '">' + otherPf[i].Name + '</a><a data-isdefbtn="yes" data-id="' + otherPf[i].ID + '" style="min-height: 16px;display: inline-block;margin-left: 5px;min-width: 16px;margin-top: 0px" class="vis-menufavitm ' + className + '" > </a></li>');
                        $menu.append(ulItem);
                    }
                }
                $menu.append($('<li data-isdefbtn="no" data-id="-1"><a data-id="-1">' + VIS.Msg.getMsg('NewReport') + '</a>'));
                $(this).w2overlay(overlay.clone(true), { css: { height: '300px' } });
            });
            $menu.on('click', "LI", function (e) {

                var btn = $(e.target);
                var id = btn.data("id");
                if (!id) {
                    return;
                }
                if (btn.data("isdefbtn") == "yes") {
                    var sql = "VIS_102";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@Ad_Table_ID", Ad_Table_ID);
                    param[1] = new VIS.DB.SqlParam("@AD_Tab_ID", curTab.getAD_Tab_ID());
                    executeQuery(sql, param);

                    var sql = "VIS_103";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@id", id);
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
                subContentPane.empty();
                subContentPane.css('width', '0px');
                setBusy(true);
                var isCreateNew = false;
                if (id == -1) {
                    id = Ad_Table_ID;
                    isCreateNew = true;
                }

                var data = self.getGenerateReportPara(queryInfo, code, isCreateNew, node_ID, tree_ID, IsSummary, 0, cmbPage.val(), "", id, windowID);

                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                    dataType: "json",
                    data: data,
                    type: 'POST',
                    //data: {
                    //    id: id,
                    //    queryInfo: JSON.stringify(queryInfo),
                    //    code: code,
                    //    isCreateNew: isCreateNew,
                    //    nProcessInfo: JSON.stringify(""),
                    //    pdf: false,
                    //    csv: false,
                    //    nodeID: node_ID,
                    //    treeID: tree_ID,
                    //    pageNo: cmbPage.val(),
                    //    showSummary: IsSummary,
                    //    AD_PInstance_ID: 0
                    //},
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

                        showReport(d.HTML);
                        reportBytes = d.Report;
                        resetPageCtrls(dres.pSetting);

                        AD_PrintFormat_ID = d.AD_PrintFormat_ID;
                        setBusy(false);
                    }
                });
            });
            btnSearch.on('click', function () {
                var find = new VIS.Find(windowNo, curTab, 0);
                find.onClose = function () {
                    if (find.getIsOKPressed()) {
                        var query = find.getQuery();
                        //	History
                        var onlyCurrentDays = find.getCurrentDays();
                        var created = find.getIsCreated();
                        find.dispose();
                        find = null;
                        queryInfo[1] = (query.getWhereClause(true));
                        launchReport(1);
                    }
                };
                find.show();
            });

            btnArchive.on('click', function () {
                self.archiveDocument(queryInfo, Ad_Table_ID, reportBytes);
                //setBusy(true);
                //$.ajax({
                //    url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                //    dataType: "json",
                //    type: "post",
                //    data: {
                //        AD_Process_ID: 0,
                //        Name: queryInfo[0],
                //        AD_Table_ID: Ad_Table_ID,
                //        Record_ID: 0,
                //        C_BPartner_ID: 0,
                //        isReport: true,
                //        binaryData: reportBytes
                //    },
                //    success: function (data) {
                //        VIS.ADialog.info('Archived', true, "", "");
                //        setBusy(false);
                //    }
                //});
            });

            //}
            //if (showPaging == true) {
            //    btnSaveCsvAll.on("click", function () {
            //        panel.setBusy(true);

            //        pageNo = $cmbPages.val();
            //        var data = {
            //            AD_Process_ID: pCtl.pi.getAD_Process_ID(),
            //            Name: pCtl.pi.getTitle(),
            //            AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
            //            AD_Table_ID: pCtl.pi.getTable_ID(),
            //            Record_ID: pCtl.pi.getRecord_ID(),
            //            pageNumber: pageNo,
            //            page_Size: PAGE_SIZE,
            //            saveAll: true
            //            //ParameterList: paraList,
            //            //csv: false,
            //            //pdf: true,
            //        }

            //        $.ajax({
            //            url: VIS.Application.contextUrl + "ReportCom/ExecuteReportforCsv",
            //            type: "Post",
            //            data: data,
            //            success: function (result) {
            //                var data = JSON.parse(result);
            //                window.open(VIS.Application.contextUrl + "TempDownload/" + data);
            //                panel.setBusy(false);
            //            }
            //        });
            //    });

            //    btnsavepdfall.on("click", function () {
            //        panel.setBusy(true);

            //        pageNo = $cmbPages.val();

            //        var data = {
            //            AD_Process_ID: pCtl.pi.getAD_Process_ID(),
            //            Name: pCtl.pi.getTitle(),
            //            AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
            //            AD_Table_ID: pCtl.pi.getTable_ID(),
            //            Record_ID: pCtl.pi.getRecord_ID(),
            //            pageNumber: pageNo,
            //            page_Size: PAGE_SIZE,
            //            printAllPages: true
            //        }

            //        $.ajax({
            //            url: VIS.Application.contextUrl + "ReportCom/ExecuteReport",
            //            type: "Post",
            //            data: data,
            //            success: function (result) {
            //                //var data = JSON.parse(result);
            //                //window.open(VIS.Application.contextUrl + data);
            //                //panel.setBusy(false);

            //                var data = JSON.parse(result);
            //                if (ismobile) {
            //                    window.open(VIS.Application.contextUrl + data);
            //                }
            //                else {
            //                    var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
            //                    $object.attr("src", VIS.Application.contextUrl + data);
            //                    cPanel.getRightDiv().empty().append($object);
            //                }

            //                //  window.open(VIS.Application.contextUrl + data);
            //                panel.setBusy(false);
            //            }
            //        });
            //    });
            //}
        };

        function printReport() {
            var mywindow = window.open();
            mywindow.document.write('<html ><head>');
            mywindow.document.write('</head><body >');

            var $htm = $(html);
            $($htm.children()).css({ "overflow": "hidden" });

            mywindow.document.write($htm.html());
            mywindow.document.write('</body></html>');
            mywindow.print();
            mywindow.close();
        };

        var launchReport = function (pNo) {

            //var id = null;
            //if (AD_PrintFormat_ID > 0) {
            //    id = AD_PrintFormat_ID;
            //}
            //else if (processInfo != null) {
            //    id = processInfo.Key;
            //}
            //else {
            //    id = Ad_Table_ID;
            //}
            setBusy(true);

            var data = self.getGenerateReportPara(queryInfo, code, false, node_ID, tree_ID, IsSummary, 0, pNo, "", AD_PrintFormat_ID, windowID);

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: data,
                type: 'POST',
                //data: {
                //    id: AD_PrintFormat_ID,
                //    queryInfo: JSON.stringify(queryInfo),
                //    code: code,
                //    isCreateNew: false,
                //    nProcessInfo: JSON.stringify(""),
                //    pdf: false,
                //    csv: false,
                //    nodeID: node_ID,
                //    treeID: tree_ID,
                //    pageNo: pNo,
                //    showSummary: IsSummary,
                //    AD_PInstance_ID: 0
                //},
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
                    showReport(d.HTML);
                    reportBytes = d.Report;
                    resetPageCtrls(dres.pSetting);
                    setBusy(false);
                }
            });
        };

        // This function works for both simple download and bulk download
        var getPdf = function (pNo, tP, bulkdownload) {
            setBusy(true);
            //var id = null;
            //if (processInfo != null) id = processInfo.Key;
            //else id = Ad_Table_ID;
            var data = self.getGenerateReportPara(queryInfo, code, false, node_ID, tree_ID, IsSummary, 0, pNo, VIS.ProcessCtl.prototype.REPORT_TYPE_PDF, AD_PrintFormat_ID,windowID);
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: data,
                type: 'POST',
                //data: {
                //    id: AD_PrintFormat_ID,
                //    queryInfo: JSON.stringify(queryInfo),
                //    code: code,
                //    isCreateNew: false,
                //    nProcessInfo: JSON.stringify(""),
                //    pdf: true,
                //    csv: false,
                //    nodeID: node_ID,
                //    treeID: tree_ID,
                //    pageNo: pNo,
                //    showSummary: IsSummary,
                //    AD_PInstance_ID: 0
                //},
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

                    if (d.HTML && d.HTML.length > 0) { //report
                        showReport(d.HTML);
                    }
                    else {
                        //download(d.ReportFilePath);
                        if (tP == 0)
                            window.open(VIS.Application.contextUrl + d.ReportFilePath);
                        else {

                            // downlaod and set its successfull percentage and create rar file on 100% complete
                            bulkdownload.setBulkBusy(false);
                            $("#forreportdownload").addClass('hide');
                            formlinks += '<input type="checkbox" data-num="' + pNo + '"  data-url="' + VIS.Application.contextUrl + d.ReportFilePath + '" checked />';
                            pNoI += 1;
                            updateCreateBar(((pNoI / tP) * 100));
                            if (pNoI == tP) {
                                setTimeout(function () {
                                    updateCreateBar(100);
                                    showCreateMessage(VIS.Msg.getMsg("Generated"));
                                    $('<form>', {
                                        'id': 'download_form',
                                        'html': formlinks,
                                        'action': '#',
                                        'Class': 'hide'
                                    }).appendTo(document.body).submit();

                                    //setTimeout(function () {
                                    //    bulkdownload.onClose();
                                    //}, 1000);

                                }, 1000);
                            }
                        }
                    }
                    setBusy(false);
                }
            });
        };

        // This function works for both simple download and bulk download
        var getCsv = function (pNo, tP, bulkdownload) {
            setBusy(true);
            //var id = null;
            //if (processInfo != null) id = processInfo.Key;
            //else id = Ad_Table_ID;
            var data = self.getGenerateReportPara(queryInfo, code, false, node_ID, tree_ID, IsSummary, 0, pNo, VIS.ProcessCtl.prototype.REPORT_TYPE_CSV, AD_PrintFormat_ID, windowID);
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: data,
                type: 'POST',
                //data: {
                //    id: AD_PrintFormat_ID,
                //    queryInfo: JSON.stringify(queryInfo),
                //    code: code,
                //    isCreateNew: false,
                //    nProcessInfo: JSON.stringify(""),
                //    pdf: false,
                //    csv: true,
                //    nodeID: node_ID,
                //    treeID: tree_ID,
                //    pageNo: pNo,
                //    showSummary: IsSummary,
                //    AD_PInstance_ID: 0
                //},
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
                    if (d.HTML && d.HTML.length > 0) { //report
                        showReport(d.HTML);
                    }
                    else {
                        //download(d.ReportFilePath);
                        if (tP == 0)
                            window.open(VIS.Application.contextUrl + d.ReportFilePath);
                        else {
                            // downlaod and set its successfull percentage and create rar file on 100% complete
                            bulkdownload.setBulkBusy(false);
                            $("#forreportdownload").addClass('hide');
                            formlinks += '<input type="checkbox" data-num="' + pNo + '" data-url="' + VIS.Application.contextUrl + d.ReportFilePath + '" checked />';
                            pNoI += 1;
                            updateCreateBar(((pNoI / tP) * 100));
                            if (pNoI == tP) {
                                setTimeout(function () {
                                    updateCreateBar(100);
                                    showCreateMessage(VIS.Msg.getMsg("Generated"));
                                    $('<form>', {
                                        'id': 'download_form',
                                        'html': formlinks,
                                        'action': '#',
                                        'Class': 'hide'
                                    }).appendTo(document.body).submit();

                                    //setTimeout(function () {
                                    //    bulkdownload.onClose();
                                    //}, 1000);

                                }, 1000);
                            }
                        }
                    }
                    setBusy(false);
                }
            });
        };

        var showReport = function (content) {
            if (isPrint != true) {
                $menu.empty();

                //var sql = "SELECT AD_PrintFormat_ID, Name, Description,IsDefault "
                //            + "FROM AD_PrintFormat "
                //            + "WHERE AD_Table_ID=" + Ad_Table_ID;
                //if (curTab.getAD_Tab_ID() > 0) {
                //    sql = sql + " AND AD_Tab_ID=" + curTab.getAD_Tab_ID();
                //}
                //sql = sql + " ORDER BY Name";
                //sql = VIS.MRole.getDefault().addAccessSQL(sql, "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

                var dr = null;
                var checkName = [];
                var count = -1;
                try {
                    // dr = executeReader(sql, null, null);



                    $.ajax({
                        type: 'Get',
                        async: false,
                        url: VIS.Application.contextUrl + "Form/GetShowReportDetails",
                        data: { AD_Table_ID: Ad_Table_ID, AD_Tab_ID: curTab.getAD_Tab_ID() },
                        success: function (data) {
                            dr = new VIS.DB.DataReader().toJson(data);
                        },
                    });


                    otherPf = [];
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
                        //var ulItem = $('<li><a data-isdefbtn="no" data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '">' + name + '</a><a data-isdefbtn="yes" data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '" style="min-height: 16px;display: inline-block;margin-left: 5px;min-width: 16px;" class="vis-mainnonfavitem" > </a></li>');
                        //// $menu.append($('<li data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '">').append(name));
                        //$menu.append(ulItem);
                        checkName.push(dr.getString(1));
                        otherPf.push(item);
                        //pp = {};
                        //pp.Key = VIS.Utility.Util.getValueOfInt(dr.get(0));
                        //pp.Name = dr.getString(1);
                        //list.push(pp);                    
                    }
                    dr.close();
                }
                catch (e) { console.log(e) }
                dr = null;

                subContentPane.html(content);
                subContentPane.css('min-width', width + 'px');
                html = content;
                var tables = document.getElementsByClassName('vis-reptabledetect');
                var tableWidth = 0;
                var tmp = 0;
                for (var i = 0, j = tables.length; i < j; i++) {
                    tmp = $(tables[i]).width();
                    if (tmp > tableWidth) tableWidth = tmp;
                }
                subContentPane.css('width', tableWidth + 50);
            }
            setBusy(false);
        };
        var setBusy = function (isBusy) {
            bsyDiv.css("display", isBusy ? 'block' : 'none');
        };
        createHeader();

        function createPageSettings() {
            ulPaging = $('<ul class="vis-statusbar-ul vis-toppaging">');

            liFirstPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftleft" aria-hidden="true" title="'+ VIS.Msg.getMsg("FirstPage")+'"></i ></div ></li > ');

            liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-arrow-left" aria-hidden="true" title="' + VIS.Msg.getMsg("PageUp") +'"></i></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-arrow-right" aria-hidden="true" title="' + VIS.Msg.getMsg("PageDown") +'"></i></div></li>');

            liLastPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" aria-hidden="true" title="' + VIS.Msg.getMsg("LastPage") +'"></i></div></li>');


            ulPaging.append(liFirstPage).append(liPrevPage).append(liCurrPage).append(liNextPage).append(liLastPage);
            pageEvents();
        }
        function pageEvents() {
            liFirstPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    launchReport(1);
                }
            });
            liPrevPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    launchReport(parseInt(cmbPage.val()) - 1);
                }
            });
            liNextPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    launchReport(parseInt(cmbPage.val()) + 1);
                }
            });
            liLastPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    launchReport(parseInt(cmbPage.find("Option:last").val()));
                }
            });
            cmbPage.on("change", function () {
                launchReport(cmbPage.val());
            });

        }
        function resetPageCtrls(psetting) {

            cmbPage.empty();
            if (psetting.TotalPage > 0) {
                for (var i = 0; i < psetting.TotalPage; i++) {
                    cmbPage.append($("<option value=" + (i + 1) + ">" + (i + 1) + "</option>"))
                }
                cmbPage.val(psetting.CurrentPage);

                self.setIteration(psetting.TotalPage);

                if (psetting.TotalPage > psetting.CurrentPage) {
                    liNextPage.css("opacity", "1");
                    liLastPage.css("opacity", "1");
                }
                else {
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

                if (psetting.CurrentPage > 1) {
                    liFirstPage.css("opacity", "1");
                    liPrevPage.css("opacity", "1");
                }
                else {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                }

                if (psetting.TotalPage == 1) {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

            }
            else {
                liFirstPage.css("opacity", "0.6");
                liPrevPage.css("opacity", "0.6");
                liNextPage.css("opacity", "0.6");
                liLastPage.css("opacity", "0.6");
            }
        }

        var contentPane = $("<div Style='width:" + width + "px; max-height: " + (height - 50) + "px;' class='vis-report-a-r-container'>");
        var subContentPane = $("<div Style='width:" + width + "px;'>");
        contentPane.append(subContentPane);
        var bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        setBusy(true);
        contentPane.append(bsyDiv);
        if (!(isPrint)) {
            $root.append(contentPane);
        }
        cFrame.hideHeader(true);
        cFrame.setContent(self);
        cFrame.show();
    };

    function APrint(AD_Process_ID, table_ID, record_ID, WindowNo, recIds, curTab, isShowRTF) {
        //var overla = null;
        var windowID = curTab == null ? 0 : curTab.getAD_Window_ID();
        $menu.off("click");
        $menu.on("click", "LI", function (e) {
            var filetype = $(e.target).data("val");
            process(false, null, filetype);
        });
        function loadFileTypes($btnInfo) {

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GetReportFileTypes/",
                dataType: "json",
                data: {
                    AD_Process_ID: AD_Process_ID
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
                        var li = '';
                        if (i == 0) {
                            li = $("<li tabindex=" + i + " data-val=" + d[i].Key + " class='vis-selected-li-print'>" + d[i].Name + "</li>");
                        }
                        else {
                            li = $("<li tabindex=" + i + " data-val=" + d[i].Key + ">" + d[i].Name + "</li>");
                        }

                        $menu.append(li);
                        li = null;
                    }

                    $btnInfo.w2overlay($root.clone(true), { css: { height: '300px' } });
                }
            });
        };


        // var rv = new VIS.ReportViewer();
        //    this.PAGE_SIZE = 10;
        this.start = function ($btnInfo) {
            //liCSV.off('click');
            //liPDF.off('click');
            //liRTF.off('click');
            //if (isShowRTF==true) {
            //    liRTF.css('display','block');
            //}
            //else {
            //    liRTF.css('display', 'none');
            //}

            //liCSV.on('click', function () {
            //    process(true);
            //});
            //liPDF.on('click', function () {
            //    process(false);
            //});
            //liRTF.on('click', function () {
            //    process(false);
            //});
            var sqlQry = "VIS_149";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Process_ID", AD_Process_ID);
            var isCrystalReport = executeScalar(sqlQry, param);

            if (isCrystalReport == "Y" && VIS.context.getIsUseCrystalReportViewer()) {
                process(false, null, VIS.ProcessCtl.prototype.REPORT_TYPE_PDF);
                return;
            }

            if ($menu.find("LI").length > 0) {
                $btnInfo.w2overlay($root.clone(true), { css: { height: '300px' } });
                window.setTimeout(function () {
                    scrollList();
                }, 100);
            }
            else {
                loadFileTypes($btnInfo);
                window.setTimeout(function () {
                    scrollList();
                }, 100);
            }


            function scrollList() {
                // var list = document.getElementById('list'); // targets the <ul>
                var li = $($($(document).find('.w2ui-overlay').find('li'))[0]); //list.firstChild; // targets the first <li>
                var liSelected = li.eq(0);
                $('#w2ui-overlay-main').attr('tabindex', 1);
                $('#w2ui-overlay-main').focus();
                // When user user down Arrow OR Up Arrow, select the item in oevrlay.
                $('#w2ui-overlay-main').keydown(function (e) {
                    if (e.which === 40) {  // If Arrow Down, then move to next item of overlay
                        if (liSelected) {
                            if (liSelected.hasClass('vis-selected-li-print')) {
                                liSelected.removeClass('vis-selected-li-print');
                            }
                            next = liSelected.next();
                            if (next.length > 0) {
                                liSelected = next.addClass('vis-selected-li-print');
                            } else {
                                liSelected = li.eq(0).addClass('vis-selected-li-print');
                            }
                        } else {
                            liSelected = li.eq(0).addClass('vis-selected-li-print');
                        }
                    } else if (e.which === 38) { // If Arrow Up, then move to Previous item of overlay
                        if (liSelected) {
                            if (liSelected.hasClass('vis-selected-li-print')) {
                                liSelected.removeClass('vis-selected-li-print');
                            }
                            next = liSelected.prev();
                            if (next.length > 0) {
                                liSelected = next.addClass('vis-selected-li-print');
                            } else {
                                liSelected = li.last().addClass('vis-selected-li-print');
                            }
                        } else {
                            liSelected = li.last().addClass('vis-selected-li-print');
                        }
                    } else if (e.which === 13) {// On Press Enter, open selected item
                        if (liSelected) {
                            var filetype = liSelected.data("val");
                            process(false, null, filetype);
                            $('#w2ui-overlay-main').off('keydown');
                            $('#w2ui-overlay-main').remove();
                        }
                    }
                    else { // If any other key pressed, then hide overlay
                        $('#w2ui-overlay-main').off('keydown');
                        $('#w2ui-overlay-main').remove();
                    }
                    e.stopPropagation();
                    return false;
                })
            };



        };

        function process(csv, callback, filetype) {
            var actionOrigin = VIS.ProcessCtl.prototype.ORIGIN_FORM;
            if (windowID > 0) {
                actionOrigin = VIS.ProcessCtl.prototype.ORIGIN_WINDOW;
            }
            if (!recIds || recIds.length == 0) {
                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GeneratePrint/",
                    dataType: "json",
                    data: {
                        AD_Process_ID: AD_Process_ID,
                        Name: 'Print',
                        AD_Table_ID: table_ID,
                        Record_ID: record_ID,
                        WindowNo: WindowNo,
                        filetype: filetype,
                        actionOrigin: actionOrigin,
                        originName: VIS.context.getWindowContext(WindowNo, "WindowName")

                    },
                    success: function (data) {
                        if (data == null) {
                            alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                        }
                        var d = jQuery.parseJSON(data);

                        if (callback) {
                            callback();
                        }

                        if (d.IsError) {
                            // Show proper message in case user do not have role for Print
                            VIS.ADialog.error("", "", d.Message, null);
                            // rv.close();
                            return;
                        }

                        //this.pi = {};
                        ////this.pi.getUseCrystalReportViewer = function () {
                        ////    if (VIS.context.ctx["#USE_CRYSTAL_REPORT_VIEWER"] == 'Y') {
                        ////        return true;
                        ////    }
                        ////    else {
                        ////        return false;
                        ////    }
                        ////};

                        ////this.pi.getAD_PInstance_ID = function () {
                        ////    return d.AD_PInstance_ID;
                        ////};

                        ////this.pi.getAD_Process_ID = function () {
                        ////    return d.AD_Process_ID;
                        ////};


                        ////this.pi.getTable_ID = function () {
                        ////    return d.AD_Table_ID;
                        ////};


                        ////this.pi.getRecord_ID = function () {
                        ////    return d.RecordID;
                        ////};


                        if (d.ReportFilePath) {
                            window.open(VIS.Application.contextUrl + d.ReportFilePath);
                        }
                        else if (d.IsTelerikReport) {
                            if (window.KJS) {
                                var tc = new KJS.TelerikContainer(d.AD_PInstance_ID, table_ID);
                                tc.show();
                                this.unlock();
                            }
                        }
                        else if (VIS.context.getIsUseCrystalReportViewer()) {
                            //var pdfViewer = new VIS.PdfViewer(null, null, true);
                            //var apro = new VIS.AProcess(d.AD_Process_ID);
                            var repV = new VIS.ReportViewerContainer(d);
                            repV.show();

                            //var frame = new VIS.CFrame();
                            //frame.setName(VIS.Msg.getMsg("Report"));
                            //frame.setTitle(VIS.Msg.getMsg("Report"));
                            //frame.setContent(repV);
                            //frame.show();
                            //repV.addReport();
                            //var rpv=new ReportViewer(

                        }
                    }
                });
            }
            else {
                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateMultiPrint/",
                    dataType: "json",
                    data: {
                        AD_Process_ID: AD_Process_ID,
                        Name: 'Print',
                        AD_Table_ID: table_ID,
                        RecIDs: recIds,
                        WindowNo: WindowNo,
                        filetype: filetype,
                        actionOrigin: actionOrigin,
                        originName: VIS.context.getWindowContext(WindowNo, "WindowName")

                    },
                    success: function (data) {
                        if (data == null) {
                            alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                        }
                        var d = jQuery.parseJSON(data);

                        if (callback) {
                            callback();
                        }

                        if (d.IsError) {
                            VIS.ADialog.error(d.ErrorText);
                            // rv.close();
                            return;
                        }

                        if (d.ReportFilePath) {
                            window.open(VIS.Application.contextUrl + d.ReportFilePath);
                            //var showPaging = d.IsReportFormat && d.TotalRecords > 0 && VIS.MRole.getDefault().getIsCanExport(table_ID);
                            //var rv = new VIS.ReportViewer(WindowNo, curTab, true,showPaging,d.TotalRecords, this.PAGE_SIZE);

                            //rv.show(d.HTML, d.AD_PrintFormat_ID, null, table_ID, null, null);
                            //var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                            //$object.attr("src", VIS.Application.contextUrl + d.ReportFilePath);

                            //rv.getRoot().append($object);
                            //rv.setReportBytes(d.Report);
                        }
                    }
                });
            }
        };

        this.startPdf = function (callback) {
            process(false, callback, VIS.ProcessCtl.prototype.REPORT_TYPE_PDF);
        };

        this.startCsv = function (callback) {
            process(true, callback, VIS.ProcessCtl.prototype.REPORT_TYPE_CSV);
        };

    };

    function ReportViewerContainer(reportInfo) {
        var $root = $('<div class="vis-reportrootdiv"> ');
        var bsyDiv = $('<div class="vis-busyindicatorouterwrap bsyCrsyVwr"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        this.getRoot = function () {
            return $root;
        };

        this.addReport = function () {
            $root.append(bsyDiv);
            setBusy(true);
            var $object = $("<iframe style = 'width:100%;height:99.4%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
            if (reportInfo.pi && reportInfo.pi.getAD_PInstance_ID()) {
                $object.attr("src", VIS.Application.contextUrl + 'Areas/VIS/WebPages/CrystalReprotViewer.aspx?title=rpt&aid=' + reportInfo.pi.getAD_PInstance_ID() + '&pid=' + reportInfo.pi.getAD_Process_ID() + '&tid=' + reportInfo.pi.getTable_ID() + '&rid=' + reportInfo.pi.getRecord_ID());
            }
            else {
                $object.attr("src", VIS.Application.contextUrl + 'Areas/VIS/WebPages/CrystalReprotViewer.aspx?title=rpt&aid=' + reportInfo.AD_PInstance_ID + '&pid=' + reportInfo.AD_Process_ID + '&tid=' + reportInfo.AD_Table_ID + '&rid=' + reportInfo.RecordID);
            }
            $root.append($object);
        };

        this.dispose = function () {
            $root.remove();
            $root = null;
        };

        var setBusy = function (isBusy) {
            bsyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.show = function () {
            var frame = new VIS.CFrame();
            frame.setName(VIS.Msg.getMsg("Report"));
            frame.setTitle(VIS.Msg.getMsg("Report"));
            frame.setContent(this);
            this.addReport();
            frame.show();

        };
    };

    //function PrintViewer()
    //{

    //};

    VIS.ReportViewer = ReportViewer;
    VIS.AReport = AReport;
    VIS.APrint = APrint;
    VIS.ReportViewerContainer = ReportViewerContainer;
})(VIS, jQuery);