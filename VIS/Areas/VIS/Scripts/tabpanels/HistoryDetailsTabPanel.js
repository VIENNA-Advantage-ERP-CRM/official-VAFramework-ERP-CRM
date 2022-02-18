VIS = window.VIS || {};
(function () {

    function HistoryDetailsTabPanel() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.table_ID = 0;
        this.frame;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;

        var $root = $('<div class="VIS-root-div"></div>');
        var $html, $rootcontent, $htmlcontent, $printhtml, $footerhtml, $paginghtml, $commentshtml;
        var PAGESIZE = 10;
        var currentPage = 0;
        var prevPage = 0;
        var nextPage = 0;
        var totalPages = 0;
        var totalRecords = 0;
        var selectedPage = 0;
        var _selectedId = 0;
        var _selectedRecId = 0;
        var historyRecords;
        var _curPageRecords = 0;
        var tableID = 0;
        var window_No = 0;
        var $lblTaskMsg = null;
        var taskClosed;

        function setContentHeight(fromRoot) {
            var $outerwrap = $root.closest(".vis-ad-w-p-ap-tp-outerwrap");
            var $divHead = $outerwrap.find('.vis-ad-w-p-ap-tp-o-b-head');
            var $divContent = $outerwrap.find(".vis-ad-w-p-ap-tp-o-b-content");
            var outerwrapheight = $outerwrap.height();
            var divHeadheight = $divHead.height() + 40;
            var divContentheight = $divContent.height();
            $divContent.css("height", (outerwrapheight - divHeadheight));
            var pagingHtmlheight = $('#VIS_pagingHtml' + window_No).height() + 10;

            if ($('#VIS_recordDetail' + window_No).is(':visible')) {
                $('#VIS_recordDetail' + window_No).css("height", ((outerwrapheight - divHeadheight) / 2) + 50);
                $('#VIS_HistoryGrd' + window_No).css("height", (((outerwrapheight - divHeadheight) / 2) - pagingHtmlheight) - 50);
            }
            else {
                $('#VIS_HistoryGrd' + window_No).css("height", (outerwrapheight - (divHeadheight + pagingHtmlheight)));
            }
        }
        /**   Intialize UI Elements  */
        this.init = function () {
            $html = $('<div class="VIS-testPanel VIS-HistGrd-data" id="VIS_HistoryGrd' + this.windowNo + '" ></div>');
            $rootcontent = $('<div id="VIS_recordDetail' + this.windowNo + '" style="display:none;" class="VIS-tp-detailsPanel"></div>');
            $root.prepend($html);

            $paginghtml = $('<div id="VIS_pagingHtml' + this.windowNo + '" class="vis-ad-w-p-s-pages VIS-root-pagingHtml"><ul class="vis-ad-w-p-s-plst w-100">' +
                '<li class="flex-fill"><div id="VIS_pagingText' + this.windowNo + '" class="vis-ad-w-p-s-result"><span class="vis-ad-w-p-s-statusdb" id="VIS_pageIndx' + window_No + '">0/0</span><span>Showing Result 0-0 of 0</span></div></li>' +
                '<li id="VIS_prevPage' + this.windowNo + '" style="opacity: 1;"><div ><i class="vis vis-pageup" title="Page Up(Alt+ Pg Up)" })="" style="opacity: 1;"></i></div></li>' +
                '<li><select id="VIS_ddlPages' + this.windowNo + '" class="vis-statusbar-combo"><option></option></select></li>' +
                '<li id="VIS_nextPage' + this.windowNo + '" style="opacity: 1;"><div ><i class="vis vis-pagedown" title="Page Down(Alt+ Pg Dn)" })="" style="opacity: 1;"></i></div></li></div>' +
                '</ul></div>');
            $commentshtml = $('<div id="VIS_viewMoreComments' + this.windowNo + '" style="display:none;" class="VIS-tp-commentsPanel"></div>');
            $root.append($paginghtml);
            $root.append($rootcontent);
            window_No = this.windowNo;
            setContentHeight();
        };

        /** Return container of panel's Design  */
        this.getRoot = function () { return $root; };

        /** Update UI elements with latest record's values.   */
        this.update = function (record_ID) {
            _selectedId = record_ID;
            tableID = this.table_ID;
            getGridDataRecordCount(record_ID, tableID);

            //var Email = VIS.context.getWindowContext(this.windowNo, "EMail");
            //var Mobile = VIS.context.getWindowContext(this.windowNo, "Mobile");
            //var RecordId = VIS.context.getWindowContext(this.windowNo, "C_Lead_ID");
            //var Phone = VIS.context.getWindowContext(this.windowNo, "Phone");

            loadGridData(record_ID, 0, this.windowNo, tableID);
        };

        function setPages(RecordId, selPage) {
            if (totalRecords > 0) {
                if (totalRecords % PAGESIZE == 0)
                    totalPages = totalRecords / PAGESIZE;
                else
                    totalPages = Math.floor(totalRecords / PAGESIZE) + 1;

                if (VIS.Utility.Util.getValueOfInt($('#VIS_ddlPages' + window_No).find(":selected").val()) > 0)
                    currentPage = VIS.Utility.Util.getValueOfInt($('#VIS_ddlPages' + window_No).find(":selected").val());
                else
                    currentPage = 1;

                if (currentPage > 0)
                    prevPage = currentPage - 1;
                if (totalPages >= (currentPage + 1))
                    nextPage = currentPage + 1;

                if (VIS.Utility.Util.getValueOfInt($('#VIS_ddlPages' + window_No).find(":selected").val()) <= 0 || $('#VIS_ddlPages' + window_No).children('option').length <= 0) {
                    $('#VIS_ddlPages' + window_No).empty();

                    for (var i = 1; i <= totalPages; i++) {
                        if (i == 1)
                            $('#VIS_ddlPages' + window_No).append(new Option(i, i, true, true));
                        else
                            $('#VIS_ddlPages' + window_No).append(new Option(i, i, false, false));
                    }
                }
                if (totalPages > 1 && totalPages > VIS.Utility.Util.getValueOfInt($('#VIS_ddlPages' + window_No).find(":selected").val()))
                    $('#VIS_pagingText' + window_No).html('<span class="vis-ad-w-p-s-statusdb" id="VIS_pageIndx' + window_No + '">1/' + _curPageRecords + '</span><span>Showing Result ' + (((currentPage - 1) * PAGESIZE) + 1) + '-' + (currentPage * PAGESIZE) + ' of ' + totalRecords + '</span>');
                else if (totalPages > 1 && totalPages == VIS.Utility.Util.getValueOfInt($('#VIS_ddlPages' + window_No).find(":selected").val()))
                    $('#VIS_pagingText' + window_No).html('<span class="vis-ad-w-p-s-statusdb" id="VIS_pageIndx' + window_No + '">1/' + _curPageRecords + '</span><span>Showing Result ' + (((currentPage - 1) * PAGESIZE) + 1) + '-' + totalRecords + ' of ' + totalRecords + '</span>');
                else if (totalPages == 1)
                    $('#VIS_pagingText' + window_No).html('<span class="vis-ad-w-p-s-statusdb" id="VIS_pageIndx' + window_No + '">1/' + _curPageRecords + '</span><span>Showing Result 1-' + totalRecords + ' of ' + totalRecords + '</span>');
            }
            else {
                $('#VIS_pagingText' + window_No).html('<span class="vis-ad-w-p-s-statusdb" id="VIS_pageIndx' + window_No + '">0/0</span><span>Showing Result 0-0 of 0</span>');
                $('#VIS_ddlPages' + window_No).empty();
            }
        }

        /* Getting the record count*/
        function getGridDataRecordCount(RecordId, TableId) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetHistoryRecordsCount",
                //async: false,
                data: { RecordId: RecordId, AD_Table_ID: TableId },
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null) {
                        totalRecords = res;
                        $('#VIS_ddlPages' + window_No).empty();
                        setPages(RecordId, 0);
                    };
                },
                error: function (e) {
                }
            });
        };

        function ddlpagesOnChange(e) {
            selectedPage = $('#VIS_ddlPages' + window_No).find(":selected").val();
            loadGridData(_selectedId, VIS.Utility.Util.getValueOfInt(selectedPage), window_No, tableID);
            currentPage = selectedPage;
        };

        /* Loading the data in to Grid*/
        function loadGridData(RecordId, selPage, window_No, TableId) {
            $('#VIS_tabPanelDataLoader' + window_No).show();
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetHistoryRecordDetails",
                //async: false,
                data: { RecordId: RecordId, AD_Table_ID: TableId, CurrentPage: selPage },
                success: function (data) {
                    var res = [];
                    var tp_con
                    res = JSON.parse(data);
                    if (res != null) {
                        historyRecords = res;
                        renderHistoryData(res, window_No, false);
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                        //setTimeout($('#tabPanelDataLoader').hide(), 10000);
                        if (VIS.Utility.Util.getValueOfInt(selPage) > 0)
                            setPages(RecordId, VIS.Utility.Util.getValueOfInt(selPage));
                        $('#VIS_tabPanelDataLoader' + window_No).hide();

                        $('#VIS_ddlPages' + window_No).one("change", ddlpagesOnChange);
                        $('#VIS_prevPage' + window_No).one("click");
                        $('#VIS_nextPage' + window_No).one("click");

                        $('#VIS_prevPage' + window_No).one("click", function (e) {
                            selectedPage = $('#VIS_ddlPages' + window_No).find(":selected").val();

                            if (VIS.Utility.Util.getValueOfInt(selectedPage) > 1) {
                                $('#VIS_prevPage' + window_No).off("click");
                                $('#VIS_ddlPages' + window_No).off("change");

                                loadGridData(_selectedId, (VIS.Utility.Util.getValueOfInt(selectedPage) - 1), window_No, TableId);
                                currentPage = selectedPage;

                                $('#VIS_ddlPages' + window_No).val((VIS.Utility.Util.getValueOfInt(selectedPage) - 1));
                                e.stopPropagation();
                                e.stopImmediatePropagation();
                            }
                        });

                        $('#VIS_nextPage' + window_No).one("click", function (e) {
                            selectedPage = $('#VIS_ddlPages' + window_No).find(":selected").val();

                            if (VIS.Utility.Util.getValueOfInt(selectedPage) < $('#VIS_ddlPages' + window_No).children('option').length) {
                                $('#VIS_nextPage' + window_No).off("click");
                                $('#VIS_ddlPages' + window_No).off("change");

                                loadGridData(_selectedId, (VIS.Utility.Util.getValueOfInt(selectedPage) + 1), window_No, TableId);
                                currentPage = selectedPage;

                                $('#VIS_ddlPages' + window_No).val((VIS.Utility.Util.getValueOfInt(selectedPage) + 1));
                                e.stopPropagation();
                                e.stopImmediatePropagation();
                            }
                        });
                    };
                },
                error: function (e) {
                }
            });
        };

        function renderHistoryData(res, window_No, isSearch) {
            var $tpdataloader = $('<div class="VIS-tabPanelDataLoader" id="VIS_tabPanelDataLoader' + window_No + '"><i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i></div>');

            var $maindatadivhtml = $('<div class="VIS-tp-mainContainer"></div>');
            var $searchhtml = $('<div class="VIS-tp-topSearchWrp">' +
                '<button id="VIS_btnRefresh' + window_No + '" class="VIS-tp-refreshIcon" >' +
                '<i class="vis vis-refresh"></i>' +
                '</button>' +
                '<div class="VIS-tp-searchControl">' +
                '<input id="VIS_txtSearch' + window_No + '" type="text" placeholder="Search">' +
                '<i id="VIS_faSearch' + window_No + '" class="fa fa-search"></i>' +
                '</div>' +
                '</div>');
            if (res.length > 0) {
                var $recshtml = $('<div class="VIS-tp-recordswrap"></div>');
                var $rechtml;
                _curPageRecords = res.length;
                $('#VIS_pageIndx' + window_No).text('1/' + _curPageRecords);

                for (var i = 0; i < res.length; i++) {
                    if (res[i].Type.toLower() == 'email')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="email" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" >' +
                            '<i data-recid="' + i + '" class="vis vis-email"></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<i data-recid="' + i + '" class="fa fa-arrow-up"></i>' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + ((VIS.Utility.Util.getValueOfString(res[i].HasAttachment) == 'true') ? '" class="vis vis-attachment1"></i>' : '"></i>') +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'inbox')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="email" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" >' +
                            '<i data-recid="' + i + '" class="vis vis-email"></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<i data-recid="' + i + '" class="fa fa-arrow-down"></i>' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + ((VIS.Utility.Util.getValueOfString(res[i].HasAttachment) == 'true') ? '" class="vis vis-attachment1"></i>' : '"></i>') +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'call')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="call" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap  ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" >' +
                            '<i data-recid="' + i + '" class="fa fa-phone" aria-hidden="true" title="Call" ></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + ((VIS.Utility.Util.getValueOfString(res[i].HasAttachment) == 'true') ? '" class="vis vis-attachment1">&nbsp;</i>' : '">&nbsp;</i>') +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'chat')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="chat" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap  ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" >' +
                            '<i data-recid="' + i + '" class="vis vis-chat" title="chat" ></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + '">&nbsp;</i>' +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'letter')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="letter" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" >' +
                            '<i data-recid="' + i + '" class="vis vis-letter"></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + ((VIS.Utility.Util.getValueOfString(res[i].HasAttachment) == 'true') ? '" class="vis vis-attachment1">&nbsp;</i>' : '">&nbsp;</i>') +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'appointment')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="appointment" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordIcon">' +
                            '<i data-recid="' + i + '" class="vis vis-appointment" title="appointment"></i>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + '">&nbsp;</i>' +
                            '<small data-recid="' + i + '">By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'task')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="task" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordIcon">' +
                            '<i data-recid="' + i + '" class="vis vis-task" title="task"></i>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            (res[i].IsTaskClosed ? '<span data-recid="' + i + '" class="VIS-tp-taskTag">' + VIS.Msg.getMsg("Closed") + '</span>' : '') +
                            '<i data-recid="' + i + '">&nbsp;</i>' +
                            '<small data-recid="' + i + '" >By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    else if (res[i].Type.toLower() == 'attachment')
                        $rechtml = $('<div data-rid="' + res[i].ID + '" data-username="' + res[i].UserName + '" data-winno="' + window_No + '" data-atype="attachment" data-recid="' + i + '" id="rowId' + i + '" class="VIS-tp-recordWrap ' +
                            '">' +
                            '<div data-recid="' + i + '" class= "VIS-tp-recordIcon" title="attachment">' +
                            '<i data-recid="' + i + '" class="vis vis-attachmentx"></i>' +
                            '</div >' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfo">' +
                            '<h6 data-recid="' + i + '">' + new Date(res[i].Created).toLocaleString() + '</h6>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordSubject">' +
                            '<p data-recid="' + i + '">' + res[i].Subject + '</p>' +
                            '</div>' +
                            '</div>' +
                            '<div data-recid="' + i + '" class="VIS-tp-recordInfoRight">' +
                            '<i data-recid="' + i + '" >&nbsp;</i>' +
                            '<small data-recid="' + i + '" >By: ' + res[i].UserName + '</small>' +
                            '</div>' +
                            '</div>');
                    $recshtml.append($rechtml);
                }

                if (isSearch) {
                    $root.find('.VIS-tp-recordswrap').empty();
                    $root.find('.VIS-tp-recordswrap').append($recshtml.html());
                }
                else {
                    $maindatadivhtml.append($tpdataloader).append($searchhtml).append($recshtml);
                    $html.empty();
                    $html.append($maindatadivhtml);
                }

                $root.find(".VIS-tp-recordWrap").click(function (e) {
                    var target = e.target;
                    var recId = '';

                    recId = VIS.Utility.Util.getValueOfInt($(target).data('recid'));
                    $root.find('#rowId' + recId).addClass('VIS-tp-selectedRecord');
                    if (VIS.Utility.Util.getValueOfInt(_selectedRecId) >= 0 && (recId != VIS.Utility.Util.getValueOfInt(_selectedRecId)) && $root.find('#rowId' + _selectedRecId).hasClass('VIS-tp-selectedRecord'))
                        $root.find('#rowId' + _selectedRecId).removeClass('VIS-tp-selectedRecord');
                    _selectedRecId = recId;

                    $('#VIS_pageIndx' + window_No).text((VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1) + '/' + _curPageRecords);
                    var record_Type = $root.find('#rowId' + recId).data('atype').toString().toUpper();
                    var rID = $root.find('#rowId' + recId).data('rid');
                    var userName = $root.find('#rowId' + recId).data('username');

                    $('#VIS_recordDetail' + window_No).show();

                    if (record_Type == "EMAIL" || record_Type == "INBOX")
                        showMail(rID, userName, window_No);
                    else if (record_Type == "CALL")
                        showCallInfo(rID, userName, window_No);
                    else if (record_Type == "APPOINTMENT")
                        showAppointmentInfo(rID, userName, window_No);
                    else if (record_Type == "LETTER")
                        showLetter(rID, userName, window_No);
                    else if (record_Type == "TASK")
                        showTask(rID, userName, window_No);
                    else if (record_Type == "ATTACHMENT")
                        showAttachment(rID, userName, window_No);
                    else if (record_Type == "CHAT")
                        showChat(rID, userName, window_No);
                    setContentHeight();
                });

                $('#VIS_txtSearch' + window_No).keyup(function (e) {

                    if (e.keyCode != undefined && e.keyCode != 13 && e.keyCode != 8) {
                        return;
                    }

                    var searchField = $(this).val();
                    if (searchField === '' && e.keyCode != 13) {
                        return;
                    }
                    var searchRecords = [];
                    var regex = new RegExp(searchField, "i");
                    var reccount = 0;
                    $.each(historyRecords, function (key, record) {
                        if ((record.Subject.search(regex) != -1) || (record.UserName.search(regex) != -1)) {
                            searchRecords[reccount] = record;
                            reccount++;
                        }
                    });
                    renderHistoryData(searchRecords, window_No, true);
                    $('#VIS_recordDetail' + window_No).fadeOut(100);
                    $('#VIS_recordDetail' + window_No).hide();

                    if (searchField === '' && e.keyCode == 13)
                        $('#VIS_pagingHtml' + window_No).show();
                    else
                        $('#VIS_pagingHtml' + window_No).hide();
                });

                $('#VIS_btnRefresh' + window_No).click(function () {
                    if (VIS.Utility.Util.getValueOfInt(_selectedId) > 0) {
                        getGridDataRecordCount(_selectedId, tableID);
                        loadGridData(_selectedId, 0, window_No, tableID);
                    }
                    $('#VIS_recordDetail' + window_No).hide();
                    $('#VIS_pagingHtml' + window_No).show();
                });

                $('#VIS_faSearch' + window_No).click(function () {
                    var searchField = $('#VIS_txtSearch' + window_No).val();
                    if (searchField === '') {
                        if (VIS.Utility.Util.getValueOfInt(_selectedId) > 0) {
                            getGridDataRecordCount(_selectedId, tableID);
                            loadGridData(_selectedId, 0, window_No, tableID);
                        }
                        $('#VIS_recordDetail' + window_No).hide();
                        $('#VIS_pagingHtml' + window_No).show();
                    }
                    else {
                        var searchRecords = [];
                        var regex = new RegExp(searchField, "i");
                        var reccount = 0;
                        $.each(historyRecords, function (key, record) {
                            if ((record.Subject.search(regex) != -1) || (record.UserName.search(regex) != -1)) {
                                searchRecords[reccount] = record;
                                reccount++;
                            }
                        });
                        renderHistoryData(searchRecords, window_No, true);
                        $('#VIS_recordDetail' + window_No).fadeOut(100);
                        $('#VIS_recordDetail' + window_No).hide();
                        $('#VIS_pagingHtml' + window_No).hide();
                    }
                });
            }
            else {
                $maindatadivhtml.append($searchhtml);
                $html.empty();
                $html.append($maindatadivhtml);
            }
        }

        function NavigateRecord(recId, window_No) {
            var historyrecord, historyrID, historyuserName, historyrecord_Type;
            var currentRecord = -1;

            if (historyRecords != null && (VIS.Utility.Util.getValueOfInt(recId) >= 0 && VIS.Utility.Util.getValueOfInt(recId) < historyRecords.length)) {
                currentRecord = (VIS.Utility.Util.getValueOfInt(recId));
                historyrecord = historyRecords[currentRecord];
                historyrID = historyrecord.ID;
                historyuserName = historyrecord.UserName;
                historyrecord_Type = VIS.Utility.Util.getValueOfString(historyrecord.Type).toUpper();

                $('#rowId' + currentRecord).addClass('VIS-tp-selectedRecord');
                if (VIS.Utility.Util.getValueOfInt(_selectedRecId) >= 0 && (currentRecord != VIS.Utility.Util.getValueOfInt(_selectedRecId)) && $('#rowId' + _selectedRecId).hasClass('VIS-tp-selectedRecord'))
                    $('#rowId' + _selectedRecId).removeClass('VIS-tp-selectedRecord');
                _selectedRecId = currentRecord;
                $('#VIS_pageIndx' + window_No).text((VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1) + '/' + _curPageRecords);
            }

            if (historyrecord_Type == "EMAIL" || historyrecord_Type == "INBOX")
                showMail(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "CALL")
                showCallInfo(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "APPOINTMENT")
                showAppointmentInfo(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "LETTER")
                showLetter(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "TASK")
                showTask(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "ATTACHMENT")
                showAttachment(historyrID, historyuserName, window_No);
            else if (historyrecord_Type == "CHAT")
                showChat(historyrID, historyuserName, window_No);
        }

        function showMail(ID, UserName, window_No) {

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedMailDetails",
                datatype: "json",
                type: "get",
                cache: false,
                data: { ID: ID },
                success: function (data) {
                    var result = JSON.parse(data);
                    var _mattachID;
                    var noOfAttchs = 0;
                    var _Record_ID = result.Record_ID;
                    var _AD_Table_ID = result.AD_Table_ID;
                    var attachID = 0;
                    _mattachID = VIS.Utility.Util.getValueOfString(result.ID);
                    var mailaddresscc, mailaddressbcc;
                    var userInitials = '';
                    var uname = VIS.Utility.Util.getValueOfString(UserName).split(' ');
                    var mailtoddlhtml, $mailbodyhtml;

                    if (uname.length > 1) {
                        userInitials = VIS.Utility.Util.getValueOfString(uname[0]).substring(0, 1).toUpper() + VIS.Utility.Util.getValueOfString(uname[1]).substring(0, 1).toUpper();
                    }
                    else
                        userInitials = VIS.Utility.Util.getValueOfString(UserName).substring(0, 2).toUpper();

                    if (VIS.Utility.Util.getValueOfString(result.Cc) != '' || VIS.Utility.Util.getValueOfString(result.Bcc) != '') {
                        mailtoddlhtml = '<div class="dropdown show VIS-tp-dropdownWrap">' + result.To +
                            '<a class="btn dropdown-toggle" style="font-size:.75rem;" href = "#" role = "button" id="dropdownMenuLink" ' +
                            'data-toggle="dropdown" aria-haspopup="true" aria-expanded="true" >' +
                            '&nbsp;<i class="fa fa-angle-down VIS-tp-recordLabels"></i></a>' +
                            '<div class="dropdown-menu" aria-labelledby="dropdownMenuLink">';

                        if (VIS.Utility.Util.getValueOfString(result.Cc) != '') {
                            mailaddresscc = VIS.Utility.Util.getValueOfString(result.Cc).split(',');
                            for (var i = 0; i < mailaddresscc.length; i++) {
                                if (mailaddresscc[i] != 'undefined' && mailaddresscc[i] != '') {
                                    if (i == 0)
                                        mailtoddlhtml += '<span class="VIS-tp-recordLabels VIS-mail-to">' + VIS.Msg.getMsg("Cc") + ':&nbsp; </span>' +
                                            '<a class="dropdown-item VIS-mail-to" href = "#">' + mailaddresscc[i] + '</a>';
                                    else
                                        mailtoddlhtml += '<a class="dropdown-item VIS-mail-to" href="#">' + mailaddresscc[i] + '</a>';
                                }
                            }
                        }
                        if (VIS.Utility.Util.getValueOfString(result.Bcc) != '') {
                            mailaddressbcc = VIS.Utility.Util.getValueOfString(result.Bcc).split(',');
                            for (var j = 0; j < mailaddressbcc.length; j++) {
                                if (mailaddressbcc[j] != 'undefined' && mailaddressbcc[j] != '') {
                                    if (j == 0)
                                        mailtoddlhtml += '<span class="VIS-tp-recordLabels VIS-mail-to">' + VIS.Msg.getMsg("Bcc") + ':&nbsp; </span><a class="dropdown-item VIS-mail-to" href="#">' + mailaddressbcc[j] + '</a>';
                                    else
                                        mailtoddlhtml += '<a class="dropdown-item VIS-mail-to" href="#">' + mailaddressbcc[j] + '</a>';
                                }
                            }
                        }
                        mailtoddlhtml += '</div></div>';
                    }
                    else
                        mailtoddlhtml = result.To;

                    if (result.Attach != null && result.Attach.length > 0) {
                        noOfAttchs = result.Attach.length;
                        attachID = result.ID;
                    }

                    if ($rootcontent.length < 1) {
                        $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailPanel"></div>');
                    }

                    $rootcontent.empty();
                    $htmlcontent = $('<div class="VIS-testPanel VIS-tp-borderBott" ><div class="d-flex align-items-center VIS-tp-leftIcons"><span><i class="fa fa-reply" data-mailto="' + result.To + '" data-mailcc="' + result.Cc + '" data-mailbcc="' + result.Bcc + '" id="VIS_imgReply' + window_No + '"></i></span><span><i class="fa fa-reply-all" data-mailto="' + result.To + '" data-mailcc="' + result.Cc + '" data-mailbcc="' + result.Bcc + '" id="VIS_imgReplyAll' + window_No + '"></i></span><span><i class="fa fa-share" id="VIS_imgForward' + window_No + '"></i></span></div>'
                        + '<div class= "align-items-center d-flex VIS-tp-rightIcons" ><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span><span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div ></div > ');
                    $mailbodyhtml = $('<div class="VIS-tp-contentdiv" >'
                        + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input"><div class="vis-tp-emailDetailWrap"><div class="VIS-mail-user-div"><span class="VIS-mail-user-span">' + userInitials + '</span></div>'
                        + '<div class="VIS-contentTitile"><span class="VIS-mail-username">' + UserName + '</span><span id="VIS_mailSubject' + window_No + '" class="VIS-mail-subject VIS-tp-recordLabels">' + result.Title
                        + '</span><div class="VIS-mail-content"><div class="VIS-mail-from"><span class="VIS-tp-recordLabels">' + VIS.Msg.getMsg("From") + ':&nbsp;</span><span style="word-break: break-word;">'
                        + result.From + '</span></div><small>' + new Date(result.Date).toLocaleString() + '</small></div><span class="VIS-mail-to"><span class="VIS-tp-recordLabels">' + VIS.Msg.getMsg("To") + ':&nbsp; </span>' + mailtoddlhtml + '</span></div></div>'
                        + '<div id="VIS_mailBody' + window_No + '" class="VIS-mail-body" >' + result.Detail + '</div></div >'
                        + '<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div>'
                        + '<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments')
                        + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments'
                        + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div></div>');

                    $printhtml = $('<div class="VIS-tp-contentdiv" >'
                        + '<div class="VIS-tp-comments-input' + window_No + '" ><div class="vis-tp-emailDetailWrap"<div class="VIS-mail-user-div"><span class="VIS-mail-user-span">' + userInitials + '</span></div>'
                        + '<div class="VIS-contentTitile"><span class="VIS-mail-username">' + UserName + '</span><span id="mailSubject" class="VIS-mail-subject VIS-tp-recordLabels">' + result.Title
                        + '</span><div class="VIS-mail-content"><span class="VIS-mail-from"><span class="VIS-tp-recordLabels">' + VIS.Msg.getMsg("From") + ':&nbsp;</span><span style="word-break: break-word;">'
                        + result.From + '</span></div><small>' + new Date(result.Date).toLocaleString() + '</small></div><span class="VIS-mail-to"><span class="VIS-tp-recordLabels">' + VIS.Msg.getMsg("To") + ':&nbsp; </span>' + mailtoddlhtml + '</span></div></div>'
                        + '<div class="VIS-mail-body" >' + result.Detail + '</div></div>');


                    $footerhtml = $('<div class="VIS-tp-downloadAttachment"><div class="VIS-tp-attachments"><i class="vis vis-attachment1" ></i><span> ' + noOfAttchs + ' ' + VIS.Msg.getMsg("Attachments") + '</span></div><div class="VIS-tp-attchDownload" id="dwnldAllAttach"><i class="vis vis-import" title="Download All" style="opacity: 1;"></i><span> ' + VIS.Msg.getMsg("VIS_DownloadAll") + '</span></div></div>'
                        + '</div>');
                    var $contenthtml = $('<div class="VIS-tp-emailDetailOuterPanel VIS-tp-recordDetail"></div>');
                    $contenthtml.append($mailbodyhtml).append($footerhtml);
                    $rootcontent.append($htmlcontent).append($contenthtml);

                    if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                        $root.append($rootcontent);
                    if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                        $root.append($paginghtml);
                    lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);


                    $('#VIS_recordDetail' + window_No).show();
                    $('#dwnldAllAttach').click(function () {
                        downLoadAllAttach(ID);
                    });
                    $('#VIS_btnComments' + window_No).click(function (e) {
                        saveComments(false, false, e);
                    });
                    $('#VIS_txtComments' + window_No).keyup(function (e) {
                        saveComments(false, false, e);
                    });
                    $('#VIS_viewAllComments' + window_No).click(function (e) {
                        if ($('#VIS_viewAllComments' + window_No).text() == VIS.Msg.getMsg('HideComments')) {
                            $('#VIS_viewMoreComments' + window_No).empty();
                            $('#VIS_viewMoreComments' + window_No).hide();
                            $('#VIS-tp-comments-input' + window_No).show();
                            $('#VIS_commentsMsg' + window_No).show();
                            $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('ViewMoreComments'));
                            lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);

                        }
                        else
                            viewAll(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);
                    });
                    $("#VIS_imgReply" + window_No).click(function (e) {
                        var action = "R";
                        var hline = '<br><br><hr>';
                        panelAction(_Record_ID, _AD_Table_ID, hline + $('#VIS_mailBody' + window_No).html(), $('#VIS_mailSubject' + window_No).text(), attachID, action, e);
                    });
                    $("#VIS_imgReplyAll" + window_No).click(function (e) {
                        var action = "RA";
                        var hline = '<br><br><hr>';
                        panelAction(_Record_ID, _AD_Table_ID, hline + $('#VIS_mailBody' + window_No).html(), $('#VIS_mailSubject' + window_No).text(), attachID, action, e);
                    });
                    $("#VIS_imgForward" + window_No).click(function (e) {
                        var action = "F";
                        var hline = '<br><br><hr>';
                        panelAction(_Record_ID, _AD_Table_ID, hline + $('#VIS_mailBody' + window_No).html(), $('#VIS_mailSubject' + window_No).text(), attachID, action, e);
                    });
                    $('#VIS_prtHistory' + window_No).find('i').click(function () {
                        finalPrint($printhtml.html());
                    });
                    $('#VIS_prevRecord' + window_No).click(function () {
                        if (_selectedRecId > 0)
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                    });
                    $('#VIS_nextRecord' + window_No).click(function () {
                        if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                    });
                    $('#VIS_btnClose' + window_No).click(function () {
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                    });
                }
            });
        };

        function showLetter(ID, UserName, window_No) {

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedLetterDetails",
                datatype: "json",
                type: "get",
                cache: false,
                data: { ID: ID },
                success: function (data) {
                    var result = JSON.parse(data);
                    var _mattachID;

                    //if (result != null && result != '') {
                    _mattachID = VIS.Utility.Util.getValueOfString(result.ID);
                    var attchFile = '';
                    var userInitials = '';
                    var uname = VIS.Utility.Util.getValueOfString(UserName).split(' ');
                    if (uname.length > 1) {
                        userInitials = VIS.Utility.Util.getValueOfString(uname[0]).substring(0, 1).toUpper() + VIS.Utility.Util.getValueOfString(uname[1]).substring(0, 1).toUpper();
                    }
                    else
                        userInitials = VIS.Utility.Util.getValueOfString(UserName).substring(0, 2).toUpper();

                    if (result.Attach != null && result.Attach.length > 0)
                        attchFile = result.Attach[0].Name;

                    if ($rootcontent.length < 1) {
                        $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                    }
                    $rootcontent.empty();

                    $htmlcontent = $('<div class="VIS-contentHeadOuter VIS-tp-borderBott" ><div class="VIS-tp-recordIcon" ><i class="vis vis-letter"></i></div><div class="VIS-contentHead"><span class="VIS-letter-header" >' + VIS.Msg.getMsg("Letter") + '</span></div><div class="align-items-center d-flex VIS-tp-rightIcons" ><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span><span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div>');


                    $mailbodyhtml = $('<div class="VIS-tp-contentdiv" >'
                        + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input"><div style="width: 100%"><table height="50px" width="100%"><tr height="50px"><td style="width:60px"><div class="VIS-mail-user-div"><span class="VIS-mail-user-span">' + userInitials + '</span></div></td>'
                        + '<td height="50px"><span class="VIS-mail-username">' + UserName + '</span><span id="mailSubject" class="VIS-mail-subject VIS-tp-recordLabels">' + result.Title + '</span><span class="VIS-mail-to">&nbsp;</span><span class="VIS-mail-from">&nbsp;</span></td><td height="50px" class="VIS-mail-date">' + new Date(result.Date).toLocaleString() + '</td></tr></table></div>'
                        + '<div id="mailBody" class="VIS-mail-body" >' + result.Detail + '</div></div>'
                        + '<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div>'
                        + '<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div></div> ');

                    $printhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                        + '<div class="" ><div style="width:100%" ><table height="50px" width="100%"><tr height="50px"><td><div class="VIS-mail-user-div"><span class="VIS-mail-user-span">' + userInitials + '</span></div></td>'
                        + '<td height="50px"><span class="VIS-mail-username">' + UserName + '</span><span id="mailSubject" class="VIS-mail-subject VIS-tp-recordLabels">' + result.Title + '</span><span class="VIS-mail-to">&nbsp;</span><span class="VIS-mail-from">&nbsp;</span></td><td height="50px" class="VIS-mail-date">' + result.Date + '</td></tr></table></div>'
                        + '<div id="mailBody" class="VIS-mail-body" >' + result.Detail + '</div></div>');


                    $footerhtml = $('<div class="VIS-tp-downloadAttachment"><div class="VIS-tp-attachments"><i class="vis vis-attachment1" ></i><span> ' + attchFile + '</span></div><div class="VIS-tp-attchDownload" id="dwnldLetterAttach"><i class="vis vis-import" title="Download" style="opacity: 1;"></i><span> ' + VIS.Msg.getMsg("VIS_Download") + '</span></div></div>'
                        + '</div>');
                    var $contenthtml = $('<div class="VIS-tp-emailDetailOuterPanel VIS-tp-recordDetail"></div>');

                    $contenthtml.append($mailbodyhtml).append($footerhtml);
                    $rootcontent.append($htmlcontent).append($contenthtml);

                    if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                        $root.append($rootcontent);
                    if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                        $root.append($paginghtml);
                    //}
                    lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);

                    $('#VIS_recordDetail' + window_No).show();
                    $('#VIS_btnComments' + window_No).click(function (e) {
                        saveComments(false, false, e);
                    });
                    $('#VIS_txtComments' + window_No).keyup(function (e) {
                        saveComments(false, false, e);
                    });
                    $('#VIS_viewAllComments' + window_No).click(function (e) {
                        if ($('#VIS_viewAllComments' + window_No).text() == VIS.Msg.getMsg('HideComments')) {
                            $('#VIS_viewMoreComments' + window_No).empty();
                            $('#VIS_viewMoreComments' + window_No).hide();
                            $('#VIS-tp-comments-input' + window_No).show();
                            $('#VIS_commentsMsg' + window_No).show();
                            $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('ViewMoreComments'));
                            lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);
                        }
                        else
                            viewAll(VIS.Utility.Util.getValueOfInt(_mattachID), false, false);
                    });
                    $('#dwnldLetterAttach').click(function () {
                        downLoadAttach(ID, attchFile);
                    });
                    $('#VIS_prtHistory' + window_No).find('i').click(function () {
                        finalPrint($printhtml.html());
                    });
                    $('#VIS_prevRecord' + window_No).click(function () {
                        if (_selectedRecId > 0)
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                    });
                    $('#VIS_nextRecord' + window_No).click(function () {
                        if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                    });
                    $('#VIS_btnClose' + window_No).click(function () {
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                    });
                }
            });
        };

        function showCallInfo(ID, UserName, window_No) {
            var CallDetails_ID = VIS.Utility.Util.getValueOfInt(ID);
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedCallDetails",
                datatype: "json",
                type: "get",
                cache: false,
                data: {
                    'ID': ID
                },
                success: function (data) {
                    var result = JSON.parse(data);
                    var _mattachID = CallDetails_ID;
                    var attchFile = ''; attach_ID = 0; attachLine_ID = 0;
                    var $callhtml, statusColor;
                    //Emp code:187
                    //To display the call duration
                    var duration = result.VA048_Duration >= 60 ? DurationCalculator(result.VA048_Duration) : result.VA048_Duration + ' ' + VIS.Msg.getMsg('VA048_Seconds');

                    if (result.Attach != null && result.Attach.length > 0) {
                        attchFile = result.Attach[0].Name;
                        attach_ID = result.Attach[0].AttID;
                        attachLine_ID = result.Attach[0].ID;
                    }

                    if (VIS.Utility.Util.getValueOfString(result.VA048_Status).toLower() == 'completed')
                        statusColor = 'style = "color: #42d819;"';

                    if ($rootcontent.length < 1) {
                        $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                    }
                    $rootcontent.empty();

                    $htmlcontent = $('<div class="VIS-tp-borderBott"><div class="VIS-contentHeadOuter">' +
                        '<div class= "VIS-tp-recordIcon" ><i class="fa fa-phone" aria-hidden="true" title="Call"></i></div >' +
                        '<div class="VIS-contentHead"><div class="VIS-contentTitile"><h6 class="mb-0">' + UserName + '</h6><small>' + result.VA048_From + '</small></div>' +
                        '<div class="align-items-center d-flex VIS-tp-rightIcons"><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span>' +
                        '<span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div></div></div>');

                    $callhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                        + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                        + '<div class="" >'
                        + '<table height="50px" width="100%">'
                        + '<tr class="VIS-call-col-header" ><td>' + VIS.Msg.getMsg("VA048_To") + '</td><td>' + VIS.Msg.getMsg("VA048_Duration") + '</td></tr>'
                        + '<tr class="VIS-call-col-data " ><td>' + result.VA048_To + '</td><td>' + duration + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Created") + '</td><td>' + VIS.Msg.getMsg("VA048_Status") + '</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + new Date(result.Created).toLocaleString() + '</td><td ' + statusColor + ' >' + result.VA048_Status + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("VA048_Price") + '</td><td>' + VIS.Msg.getMsg("VA048_PriceUnit") + '</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + result.VA048_Price + '</td><td>' + result.VA048_Price_Unit + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Category") + '</td><td>' + VIS.Msg.getMsg("Attachments") + '</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + VIS.Msg.getMsg("VA048_CallType") + '</td><td class="VIS-tp-attchDownload" id="dwnldCallAttach">' + attchFile + '</td></tr>'
                        + '</table>'
                        + '</div>'
                        + '</div>'
                        + '<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel" ></div>'
                        + '<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                        + '</div>');

                    $printhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                        + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                        + '<div class="" >'
                        + '<table height="50px" width="100%">'
                        + '<tr class="VIS-call-col-header" ><td>' + VIS.Msg.getMsg("VA048_To") + '</td><td>' + VIS.Msg.getMsg("VA048_Duration") + '</td></tr>'
                        + '<tr class="VIS-call-col-data " ><td>' + result.VA048_To + '</td><td>' + result.VA048_Duration + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Created") + '</td><td>' + VIS.Msg.getMsg("VA048_Status") + '</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + result.Created + '</td><td style="color: #42d819;">' + result.VA048_Status + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("VA048_Price") + 'Price</td><td>' + VIS.Msg.getMsg("VA048_PriceUnit") + 'Price Unit</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + result.VA048_Price + '</td><td>' + result.VA048_Price_Unit + '</td></tr>'
                        + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Category") + '</td><td>' + VIS.Msg.getMsg("Attachments") + '</td></tr>'
                        + '<tr class="VIS-call-col-data"><td>' + VIS.Msg.getMsg("VA048_CallType") + '</td><td class="VIS-tp-attchDownload" id="dwnldCallAttach">' + attchFile + '</td></tr>'
                        + '</table>'
                        + '</div>'
                        + '</div>');

                    $footerhtml = $('<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                        + '</div>');

                    var $contenthtml = $('<div class="VIS-mail-header VIS-tp-recordDetail"></div>');
                    $contenthtml.append($callhtml); //.append($footerhtml);
                    $rootcontent.append($htmlcontent).append($contenthtml);

                    if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                        $root.append($rootcontent);
                    if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                        $root.append($paginghtml);
                    lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, true);

                    $('#VIS_recordDetail' + window_No).show();
                    $('#dwnldCallAttach').click(function () {
                        downLoadAttachCall(attachLine_ID, attach_ID, attchFile);
                    });
                    $('#VIS_btnComments' + window_No).click(function (e) {
                        saveComments(false, true, e);
                    });
                    $('#VIS_txtComments' + window_No).keyup(function (e) {
                        saveComments(false, true, e);
                    });
                    $('#VIS_viewAllComments' + window_No).click(function (e) {
                        if ($('#VIS_viewAllComments' + window_No).text() == VIS.Msg.getMsg('HideComments')) {
                            $('#VIS_viewMoreComments' + window_No).empty();
                            $('#VIS_viewMoreComments' + window_No).hide();
                            $('#VIS-tp-comments-input' + window_No).show();
                            $('#VIS_commentsMsg' + window_No).show();
                            $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('ViewMoreComments'));
                            lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), false, true);
                        }
                        else
                            viewAll(VIS.Utility.Util.getValueOfInt(_mattachID), false, true);
                    });
                    $('#VIS_prtHistory' + window_No).find('i').click(function () {
                        finalPrint($printhtml.html());
                    });
                    $('#VIS_prevRecord' + window_No).click(function () {
                        if (_selectedRecId > 0)
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                    });
                    $('#VIS_nextRecord' + window_No).click(function () {
                        if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                    });
                    $('#VIS_btnClose' + window_No).click(function () {
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                    });
                }
            });
        };

        function showAppointmentInfo(ID, UserName, window_No) {
            var attdInfo = "";
            var $appthtml;
            var ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedAppointmentDetails", { "record_ID": ID }, null);
            var _mattachID;
            if (ds != null) {
                strApp = "";
                _mattachID = VIS.Utility.Util.getValueOfString(ds["AppointmentsInfo_ID"]);
                var attInfo = ds["AttendeeInfo"];
                if (attInfo != null && attInfo != "") {
                    names = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "AttachmentHistory/GetUser", { "UserQry": attInfo }, null);
                    if (names != null && names.length > 0) {
                        strApp = "";
                        for (var k = 0; k < names.length; k++) {
                            strApp += names[k] + ", ";
                        }
                        strApp = strApp.substring(0, strApp.length - 2);
                        attdInfo = strApp;
                    }
                }

                if ($rootcontent.length < 1) {
                    $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                }
                $rootcontent.empty();

                $htmlcontent = $('<div class="VIS-tp-borderBott"><div class="VIS-contentHeadOuter">' +
                    '<div class= "VIS-tp-recordIcon" ><i class="vis vis-task" title="task"></i></div >' +
                    '<div class="VIS-contentHead"><div class="VIS-contentTitile"><h6 class="mb-0">' + VIS.Msg.getMsg("Appointment") + '</h6><small>' + ds["Subject"] + '</small></div>' +
                    '<div class="align-items-center d-flex VIS-tp-rightIcons"><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span>' +
                    '<span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div></div></div>');

                $appthtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                    + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                    + '<div >'
                    + '<table height="50px" width="100%">'
                    + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Location") + '</td><td>' + VIS.Msg.getMsg("AllDay") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td>' + ds["Location"] + '</td><td>' + ds["Allday"] + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("StartDate") + '</td><td>' + VIS.Msg.getMsg("EndDate") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td>' + new Date(ds["StartDate"]).toLocaleString() + '</td><td>' + new Date(ds["EndDate"]).toLocaleString() + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Contacts") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td colspan="2">' + attdInfo + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Description") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td colspan="2">' + ds["Description"] + '</td></tr>'
                    + '</table>'
                    + '</div>'
                    + '</div>'
                    + '<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div>'
                    + '<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                    + '</div>');

                $printhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                    + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                    + '<div >'
                    + '<table height="50px" width="100%">'
                    + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Location") + '</td><td>' + VIS.Msg.getMsg("AllDay") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td>' + ds["Location"] + '</td><td>' + ds["Allday"] + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("StartDate") + '</td><td>' + VIS.Msg.getMsg("EndDate") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td>' + ds["StartDate"] + '</td><td>' + ds["EndDate"] + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Contacts") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td colspan="2">' + attdInfo + '</td></tr>'
                    + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Description") + '</td></tr>'
                    + '<tr class="VIS-call-col-data"><td colspan="2">' + ds["Description"] + '</td></tr>'
                    + '</table>'
                    + '</div>'
                    + '</div>');

                $footerhtml = $('<div id="ViewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div><div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                    + '</div>');

                var $contenthtml = $('<div class="VIS-mail-header VIS-tp-recordDetail"></div>');
                $contenthtml.append($appthtml); //.append($footerhtml);
                $rootcontent.append($htmlcontent).append($contenthtml);

                if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                    $root.append($rootcontent);
                if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                    $root.append($paginghtml);
                lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);

                attdInfo = "";
                $('#VIS_recordDetail' + window_No).show();
                $('#VIS_btnComments' + window_No).click(function (e) {
                    saveComments(true, false, e);
                });
                $('#VIS_txtComments' + window_No).keyup(function (e) {
                    saveComments(true, false, e);
                });
                $('#VIS_viewAllComments' + window_No).click(function (e) {
                    if ($('#VIS_viewAllComments' + window_No).text() == VIS.Msg.getMsg('HideComments')) {
                        $('#VIS_viewMoreComments' + window_No).empty();
                        $('#VIS_viewMoreComments' + window_No).hide();
                        $('#VIS-tp-comments-input' + window_No).show();
                        $('#VIS_commentsMsg' + window_No).show();
                        $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('ViewMoreComments'));
                        lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);
                    }
                    else
                        viewAll(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);
                });
                $('#VIS_prtHistory' + window_No).find('i').click(function () {
                    finalPrint($printhtml.html());
                });
                $('#VIS_prevRecord' + window_No).click(function () {
                    if (_selectedRecId > 0)
                        NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                });
                $('#VIS_nextRecord' + window_No).click(function () {
                    if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                        NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                });
                $('#VIS_btnClose' + window_No).click(function () {
                    $('#VIS_recordDetail' + window_No).hide();
                    setContentHeight();
                });
            }
        };

        function showTask(rID, UserName, window_No) {

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedTaskDetails",
                datatype: "json",
                type: "get",
                cache: false,
                //async: false,
                data: { record_ID: rID },
                success: function (data) {
                    var result = JSON.parse(data);
                    var attdInfo = "";
                    var _mattachID, $taskhtml;
                    if (result != null) {
                        var strApp = "";
                        var attInfo = result.AttendeeInfo;
                        taskClosed = result.IsTaskClosed;
                        _mattachID = VIS.Utility.Util.getValueOfString(result.AppointmentsInfo_ID);
                        if (attInfo != null && attInfo != "") {
                            names = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "AttachmentHistory/GetUser", { "UserQry": attInfo }, null);
                            if (names != null && names.length > 0) {
                                strApp = "";
                                for (var k = 0; k < names.length; k++) {
                                    strApp += names[k] + ", ";
                                }
                                strApp = strApp.substring(0, strApp.length - 2);
                                attdInfo = strApp;
                            }
                        }
                        var prtyText, prtyTextColor;

                        if (VIS.Utility.Util.getValueOfString(result.PriorityKey) != '') {
                            if (result.PriorityKey == "1") {
                                prtyTextColor = "color: #f60000;";
                                prtyText = "Urgent";
                            }
                            else if (result.PriorityKey == "3") {
                                prtyTextColor = "color: #fb9300;";
                                prtyText = "High";
                            }
                            else if (result.PriorityKey == "5") {
                                prtyTextColor = "color: #6f04e8;";
                                prtyText = "Medium";
                            }
                            else if (result.PriorityKey == "7") {
                                prtyTextColor = "color: #03e5f3;";
                                prtyText = "Low";
                            }
                            else if (result.PriorityKey == "9") {
                                prtyTextColor = "color: #42d819;";
                                prtyText = "Minor";
                            }
                        }

                        if ($rootcontent.length < 1) {
                            $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                        }
                        $rootcontent.empty();

                        $htmlcontent = $('<div class="VIS-tp-borderBott"><div class="VIS-contentHeadOuter">' +
                            '<div class= "VIS-tp-recordIcon" ><i class="vis vis-task" title="task"></i></div >' +
                            '<div class="VIS-contentHead"><div class="VIS-contentTitile"><h6 class="mb-0">' + VIS.Msg.getMsg("Task") + '</h6><small>' + result.Subject + '</small></div>' +
                            '<div class="align-items-center d-flex VIS-tp-rightIcons"><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span>' +
                            '<span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div></div></div>');

                        $taskhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                            + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                            + '<div class="VIS-tp-taskcontTag" >'
                            + (result.IsTaskClosed ? '<span class="VIS-tp-taskTag">' + VIS.Msg.getMsg("Closed") + '</span>' : '')
                            + '<table height="50px" width="100%">'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Priority") + '</td><td>' + VIS.Msg.getMsg("Status") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td style="' + prtyTextColor + '">' + prtyText + '</td><td>' + (VIS.Utility.Util.getValueOfInt(result.TaskStatus) * 10) + '%</td></tr>'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("StartDate") + '</td><td>' + VIS.Msg.getMsg("EndDate") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td>' + new Date(result.StartDate).toLocaleString() + '</td><td>' + new Date(result.EndDate).toLocaleString() + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("ASSIGNEDTO") + '</td><td>' + VIS.Msg.getMsg("Category") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td>' + result.AssignedTo + '</td><td>' + result.CategoryName + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Result") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td colspan="2">' + result.Result + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Description") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td colspan="2">' + result.Description + '</td></tr>'
                            + '<tr><td><div class="vis-float-left vis-frm-ls-top"><input id="VIS_chkTaskComplete' + window_No
                            + '" value="1" type="checkbox" class="vis-float-left"><label id="VIS_lblTaskComplete' + window_No
                            + '" for="chkTaskComplete" class="wsp-task-from-inputLabel vis-float-left" style="margin:0 0 0 5px;">' + VIS.Msg.getMsg("Closed")
                            + '</label></div></td><td><div class="vis-float-right"><a href="javascript:void(0)" id="VIS_hlnktaskdone' + window_No
                            + '" class="vis-btn vis-btn-done vis-icon-doneButton vis-float-right vis-btnOk"> <span class="vis-btn-ico vis-btn-done-bg vis-btn-done-border"></span>'
                            + VIS.Msg.getMsg("Done") + '</a></div></td></tr>'
                            + '</table>'
                            + '</div>'
                            + '</div>'
                            + '<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div>'
                            + '<div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                            + '</div>');

                        $printhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'
                            + '<div id="VIS-tp-comments-input' + window_No + '" class="VIS-tp-comments-input">'
                            + '<div class="VIS-tp-taskcontTag" >'
                            + (result.IsTaskClosed ? '<span class="VIS-tp-taskTag">' + VIS.Msg.getMsg("Closed") + '</span>' : '')
                            + '<table height="50px" width="100%">'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("Priority") + '</td><td>' + VIS.Msg.getMsg("Status") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td style="' + prtyTextColor + '">' + result.Priority + '</td><td>' + (VIS.Utility.Util.getValueOfInt(result.TaskStatus) * 10) + '%</td></tr>'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("StartDate") + '</td><td>' + VIS.Msg.getMsg("EndDate") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td>' + result.StartDate + '</td><td>' + result.EndDate + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td>' + VIS.Msg.getMsg("ASSIGNEDTO") + '</td><td>' + VIS.Msg.getMsg("Category") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td>' + result.AssignedTo + '</td><td>' + result.CategoryName + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Result") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td colspan="2">' + result.Result + '</td></tr>'
                            + '<tr class="VIS-call-col-header"><td colspan="2">' + VIS.Msg.getMsg("Description") + '</td></tr>'
                            + '<tr class="VIS-call-col-data"><td colspan="2">' + result.Description + '</td></tr>'
                            + '</table>'
                            + '</div>'
                            + '</div>');

                        $footerhtml = $('<div id="VIS_viewMoreComments' + window_No + '" style="display:none;" class="VIS-tp-commentsPanel"></div><div id="VIS_commentsdata' + window_No + '"><div class="pr-0 m-0 VIS-tp-commentsField d-flex flex-column w-100"><p id="VIS_viewAllComments' + window_No + '" class="vis-attachhistory-view-comments" > ' + VIS.Msg.getMsg('ViewMoreComments') + '</p><div class="vis-attachhistory-comments vis-feedMessage m-0"><input id="VIS_txtComments' + window_No + '" type="text" placeholder="' + VIS.Msg.getMsg('TypeComment') + '"></input><span id="VIS_btnComments' + window_No + '" class="vis-attachhistory-comment-icon vis vis-sms"></span></div></div>'
                            + '</div>');

                        var $contenthtml = $('<div class="VIS-mail-header VIS-tp-recordDetail"></div>');
                        $contenthtml.append($taskhtml); //.append($footerhtml);
                        $rootcontent.append($htmlcontent).append($contenthtml);

                        if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                            $root.append($rootcontent);
                        if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                            $root.append($paginghtml);
                        lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);

                        attdInfo = "";
                        $('#VIS_recordDetail' + window_No).show();
                        $('#VIS_btnComments' + window_No).click(function (e) {
                            saveComments(true, false, e);
                        });
                        $('#VIS_txtComments' + window_No).keyup(function (e) {
                            saveComments(true, false, e);
                        });
                        $('#VIS_viewAllComments' + window_No).click(function (e) {
                            if ($('#VIS_viewAllComments' + window_No).text() == VIS.Msg.getMsg('HideComments')) {
                                $('#VIS_viewMoreComments' + window_No).empty();
                                $('#VIS_viewMoreComments' + window_No).hide();
                                $('#VIS-tp-comments-input' + window_No).show();
                                $('#VIS_commentsMsg' + window_No).show();
                                $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('ViewMoreComments'));
                                lastHistoryComment(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);
                            }
                            else
                                viewAll(VIS.Utility.Util.getValueOfInt(_mattachID), true, false);
                        });
                        $('#VIS_prtHistory' + window_No).find('i').click(function () {
                            finalPrint($printhtml.html());
                        });
                        $('#VIS_prevRecord' + window_No).click(function () {
                            if (_selectedRecId > 0)
                                NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                        });
                        $('#VIS_nextRecord' + window_No).click(function () {
                            if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                                NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                        });
                        $('#VIS_btnClose' + window_No).click(function () {
                            $('#VIS_recordDetail' + window_No).hide();
                            setContentHeight();
                        });

                        //changes done by Emp Id:187
                        //After clicking on div if the task is closed it need to hide closed checkbox and Done 
                        if (taskClosed) {
                            $taskhtml.find("#VIS_chkTaskComplete" + window_No).hide();
                            $taskhtml.find("#VIS_hlnktaskdone" + window_No).hide();
                            $taskhtml.find("#VIS_lblTaskComplete" + window_No).hide();

                        }
                        // Task is updated when task is closed from tab panel
                        $taskhtml.find("#VIS_hlnktaskdone" + window_No).on("click", function (e) {

                            if ($lblTaskMsg != null)
                                $lblTaskMsg.empty();
                            var t_title = result.Subject;
                            var t_ctgry = result.CategoryName;
                            var t_sdatetime = result.StartDate;
                            var t_edatetime = result.EndDate;
                            if (t_edatetime < t_sdatetime) {
                                $lblTaskMsg.append(VIS.Msg.getMsg("WSP_EndDateShouldBeGreaterThanStartDate"));
                                return;
                            }
                            var t_desc = result.Description;
                            var t_result = result.Result;
                            var t_prtykey = result.PriorityKey;
                            var t_prty = result.Priority;
                            var t_sts = parseInt(result.TaskStatus);
                            var t_ststext = result.TaskStatus;
                            var t_isClosed = "";
                            var $chkTaskCmplte = $taskhtml.find("#VIS_chkTaskComplete" + window_No).prop("checked", true);;
                            if ($chkTaskCmplte.is(":checked")) {
                                t_isClosed = true;
                            }
                            else {
                                t_isClosed = false;
                            }
                            var models = [];
                            var task_ID = result.AppointmentsInfo_ID;
                            var t_cntact_id = [];
                            WspSharedUser = [];
                            var Ad_UserID = 0;
                            var taskusrID = result.UserID;
                            if (taskusrID != null && taskusrID != 0) {
                                Ad_UserID = taskusrID;
                            }
                            else {
                                Ad_UserID = VIS.context.getAD_User_ID();
                            }
                            models.push({ AppointmentsInfo_ID: task_ID, Title: t_title, Description: t_desc, Start: t_sdatetime, End: t_edatetime, Categories: t_ctgry, TaskStatus: t_sts, PriorityKey: t_prtykey, Ad_User_ID: Ad_UserID, Contacts: t_cntact_id, ContactsInfo: WspSharedUser, isClosed: t_isClosed, Result: t_result });
                            if (t_title != "" && t_title.trim().length > 0) {
                                $.ajax({
                                    url: VIS.Application.contextUrl + 'WSP/Home/UpdateJson_Task',
                                    type: "POST",
                                    datatype: "JSON",
                                    contentType: "application/json; charset=utf-8",
                                    async: true,
                                    //data: { models: models },
                                    data: JSON.stringify({ models: models }),
                                    success: function (result) {
                                        var data = JSON.parse(result);
                                        VIS.ADialog.info("WSP_TaskUpdated", true, null);
                                        $taskhtml.find("#VIS_chkTaskComplete" + window_No).hide();
                                        $taskhtml.find("#VIS_hlnktaskdone" + window_No).hide();
                                        $taskhtml.find("#VIS_lblTaskComplete" + window_No).hide();
                                    }
                                });
                            }
                        });
                    }
                },
                error: function (e) {
                }
            });
        };

        function showAttachment(ID, UserName, window_No) {

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedAttachmentDetails",
                datatype: "json",
                type: "get",
                cache: false,
                data: { record_ID: ID },
                success: function (data) {
                    var result = JSON.parse(data);
                    var attchFile = '', attachments = '';

                    if (result != null && result.length > 0) {

                        for (var i = 0; i < result.length; i++) {
                            var imgTag = '';
                            if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.doc' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.docx') {
                                imgTag = '<i class="vis vis-doc-word"></i>';//'<img src="./Areas/VIS/Images/word.png" >';
                            }
                            else if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.xls' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.xlsx') {
                                imgTag = '<i class="vis vis-doc-excel"></i>';//'<img src="./Areas/VIS/Images/excel.png" >';
                            }
                            else if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.ppt' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.pptx') {
                                imgTag = '<i class="vis vis-doc-pp"></i>';//'<img src="./Areas/VIS/Images/ppt.png" >';
                            }
                            else if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.pdf') {
                                imgTag = '<i class="vis vis-doc-pdf"></i>';//'<img src="./Areas/VIS/Images/pdf.png" >';
                            }
                            else if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.txt') {
                                imgTag = '<i class="vis vis-doc-text"></i>';//'<img src="./Areas/VIS/Images/text.png" >';
                            }
                            else if (VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.png' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.jpg' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.jpeg' || VIS.Utility.Util.getValueOfString(result[i].FileType).toString().toLower() == '.gif') {
                                imgTag = '<i class="vis vis-doc-img"></i>';//'<img src="./Areas/VIS/Images/image.png" >';
                            }
                            else {
                                imgTag = '<i class="vis vis-doc-img"></i>';//'<img src="./Areas/VIS/Images/text.png" >';
                            }

                            attachments += '<div class="VIS-tp-recordWrap"><div class="VIS-tp-attachIcon">' + imgTag + '</div><div data-filename="' + VIS.Utility.Util.getValueOfString(result[i].FileName) + '" data-attid="' + VIS.Utility.Util.getValueOfString(result[i].AD_Attachment_ID) + '" data-id="' + VIS.Utility.Util.getValueOfString(result[i].ID) + '" class="VIS-tp-attachInfo" ><h6>' + VIS.Utility.Util.getValueOfString(result[i].FileName) + '</h6><small>' + VIS.Utility.Util.getValueOfString(result[i].FileSize) + ' kb</small></div>'
                                + '<div class="VIS-tp-recordInfoRight"><span class="VIS-tp-dateTime">' + new Date(result[i].CreatedOn).toLocaleString() + '</span><small>By: ' + VIS.Utility.Util.getValueOfString(result[i].CreatedBy) + '</small></div></div>';
                        }
                    }

                    if ($rootcontent.length < 1) {
                        $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                    }
                    $rootcontent.empty();

                    $htmlcontent = $('<div class="VIS-contentHeadOuter VIS-tp-borderBott"><div class= "VIS-tp-recordIcon" ><i class="vis vis-attachmentx"></i></div><div class="VIS-contentHead"><span class="VIS-letter-header">' + VIS.Msg.getMsg('Attachment') + '</span></div><div class="align-items-center d-flex VIS-tp-rightIcons" ><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span><span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div>');
                    $printhtml = $('<div class="VIS-tp-contentdiv">'
                        + '<div class="VIS-tp-contentWrap VIS-tp-attachmentContent">'
                        + attachments
                        + '</div>'
                        + '</div>');

                    var $contenthtml = $('<div class="VIS-mail-header VIS-tp-recordDetail"></div>');
                    $contenthtml.append($printhtml);
                    $rootcontent.append($htmlcontent).append($contenthtml);

                    if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                        $root.append($rootcontent);
                    if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                        $root.append($paginghtml);

                    attdInfo = "";
                    $('#VIS_recordDetail' + window_No).show();
                    $(".VIS-tp-attachInfo").click(function (e) {
                        $('.VIS-tp-contentWrap').css({ "cursor": "wait" });
                        downLoadHistoryAttach(e);
                    });
                    $('#VIS_prtHistory' + window_No).find('i').click(function () {
                        finalPrint($printhtml.html());
                    });
                    $('#VIS_prevRecord' + window_No).click(function () {
                        if (_selectedRecId > 0)
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                    });
                    $('#VIS_nextRecord' + window_No).click(function () {
                        if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                    });
                    $('#VIS_btnClose' + window_No).click(function () {
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                    });
                }
            });
        };

        function showChat(ID, UserName, window_No) {

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/GetSelectedChatDetails",
                datatype: "json",
                type: "get",
                cache: false,
                data: { record_ID: ID },
                success: function (data) {
                    var result = JSON.parse(data);
                    var senderImg = '', receptorImg = '', chatData = '';
                    var isDisplayChatbox = false;
                    var rec_ID = 0;
                    var _createdDt, createdDt;
                    var loginUserName = VIS.context.getAD_User_Name();

                    if (result != null && result.length > 0) {
                        var isSender = '';
                        for (var i = 0; i < result.length; i++) {
                            if (result[i].Created != null && result[i].Created != '') {
                                var currentDt = new Date();
                                _createdDt = new Date(result[i].Created.toString());
                                var hours = _createdDt.getHours();
                                var mins = _createdDt.getMinutes();
                                var mid = ' AM';
                                if (hours == 0) {
                                    hours = 12;
                                }
                                else if (hours < 12) {
                                    hours = hours;
                                }
                                else if (hours == 12) {
                                    hours = hours;
                                    mid = ' PM';
                                }
                                else if (hours > 12) {
                                    hours = hours - 12;
                                    mid = ' PM';
                                }

                                createdDt = _createdDt.getMonth() + '/' + _createdDt.getDate() + '/' + _createdDt.getFullYear() + ' | ' + ((hours >= 10) ? hours : ('0' + hours)) + ':' + ((mins >= 10) ? mins : ('0' + mins)) + mid;
                            }
                            if (i == 0) {
                                chatData = '<div class="VIS-tp-contentWrap VIS-tp-attachmentContent">';
                            }

                            if (i == 0 || i == (VIS.Utility.Util.getValueOfInt(result.length) - 1)) {
                                if (_createdDt.getDate() == currentDt.getDate() && _createdDt.getMonth() == currentDt.getMonth() && _createdDt.getFullYear() == currentDt.getFullYear())
                                    isDisplayChatbox = true;
                            }

                            rec_ID = VIS.Utility.Util.getValueOfInt(result[i].Record_ID);

                            if (loginUserName.toString().toLower() == result[i].UserName.toString().toLower()) //|| (i > 0 && result[i].UserName.toString() != result[i - 1].UserName.toString())
                            {
                                chatData += '<div class="VIS-tp-recordWrap VIS-tp-msgSend">' +
                                    '<div class="VIS-tp-userImg" >' + result[i].UserImage + '</div>' +
                                    '<div class="VIS-tp-recordInfo pr-0">' +
                                    '<div class="VIS-tp-chatInfo"><h6>' + result[i].UserName + '</h6><span>' + _createdDt.toLocaleString() + '</span></div>' +
                                    '<p class="VIS-tp-message">' + result[i].CharacterData + '</p></div></div >'

                            }
                            else {
                                chatData += '<div class="VIS-tp-recordWrap VIS-tp-msgRecevied">' +
                                    '<div class="VIS-tp-userImg" >' + result[i].UserImage + '</div>' +
                                    '<div class="VIS-tp-recordInfo pr-0">' +
                                    '<div class="VIS-tp-chatInfo"><h6>' + result[i].UserName + '</h6><span>' + _createdDt.toLocaleString() + '</span></div>' +
                                    '<p class="VIS-tp-message">' + result[i].CharacterData + '</p></div></div>'
                            }

                            if (i == (result.length - 1))
                                chatData += '</div>';
                        }
                    }

                    if ($rootcontent.length < 1) {
                        $rootcontent = $('<div id="VIS_recordDetail' + window_No + '" class="VIS-tp-detailsPanel"></div>');
                    }
                    $rootcontent.empty();
                    var chatboxhtml = '';
                    if (isDisplayChatbox) {
                        chatboxhtml = '<div class="VIS-chatBoxWrap"><div class="VIS-chat-box"><textarea id="VIS_chatBox' + window_No + '" class="VIS-chat-msgbox" style="height:46px;" maxlength="500"></textarea></div>'
                            + '<div class="VIS-chat-send-div"><div style="float:left;"></div><div class="VIS-chat-send"><i class="fa fa-paper-plane VIS-tp-chatBtn" data-chid="' + VIS.Utility.Util.getValueOfInt(ID).toString() + '" data-recid="' + rec_ID.toString() + '" id="VIS_imgSend' + window_No + '" ></i><i id="VIS_imgCancel' + window_No + '" class="fa fa-times-circle VIS-tp-chatBtn"></i></div></div></div>';
                    };
                    $htmlcontent = $('<div class="VIS-contentHeadOuter VIS-tp-borderBott" ><div class= "VIS-tp-recordIcon" ><i class="vis vis-chat"></i></div><div class="VIS-contentHead"><span class="VIS-letter-header">' + VIS.Msg.getMsg('Chat') + '</span></div><div class="align-items-center d-flex VIS-tp-rightIcons" ><span id="VIS_prtHistory' + window_No + '"><i class="vis vis-print" title="Print"></i></span><span><i id="VIS_prevRecord' + window_No + '" class="fa fa-arrow-left"></i></span><span><i id="VIS_nextRecord' + window_No + '" class="fa fa-arrow-right"></i></span><span class="VIS-close-btn" id="VIS_btnClose' + window_No + '"><i class="vis vis-cross"></i></span></div></div>');

                    $printhtml = $('<div class="VIS-tp-contentdiv VIS-tp-contentPanel">'

                        + '<div class="' + (isDisplayChatbox ? 'VIS-chatContentWrap' : '') + '" >'
                        + chatData
                        + '</div>');

                    $footerhtml = $(chatboxhtml
                        + '</div>');

                    var $contenthtml = $('<div class="VIS-mail-header VIS-tp-recordDetail"></div>');
                    $contenthtml.append($printhtml).append($footerhtml);
                    $rootcontent.append($htmlcontent).append($contenthtml);

                    if (!$root.html().toString().contains('VIS_recordDetail' + window_No))
                        $root.append($rootcontent);
                    if (!$root.html().toString().contains('VIS_pagingHtml' + window_No))
                        $root.append($paginghtml);

                    attdInfo = "";
                    $('#VIS_recordDetail' + window_No).show();
                    $('#VIS_imgSend' + window_No).click(function (e) {
                        saveChat(e);
                    });
                    $('#VIS_imgCancel' + window_No).click(function (e) {
                        $('#VIS_chatBox' + window_No).val('');
                    });
                    $('#VIS_prtHistory' + window_No).find('i').click(function () {
                        finalPrint($printhtml.html());
                    });
                    $('#VIS_prevRecord' + window_No).click(function () {
                        if (_selectedRecId > 0)
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) - 1, window_No);
                    });
                    $('#VIS_nextRecord' + window_No).click(function () {
                        if (_selectedRecId < (VIS.Utility.Util.getValueOfInt(totalRecords) - 1))
                            NavigateRecord(VIS.Utility.Util.getValueOfInt(_selectedRecId) + 1, window_No);
                    });
                    $('#VIS_btnClose' + window_No).click(function () {
                        $('#VIS_recordDetail' + window_No).hide();
                        setContentHeight();
                    });
                }
            });
        };

        function DurationCalculator(time) {
            var hr = ~~(time / 3600);
            var min = ~~((time % 3600) / 60);
            var sec = time % 60;
            var sec_min = "";
            if (hr > 0) {
                sec_min += "" + hrs + ":" + (min < 10 ? "0" : "");
            }
            sec_min += "" + min + ":" + (sec < 10 ? "0" : "");
            sec_min += "" + sec;
            return sec_min + ' ' + VIS.Msg.getMsg('VA048_Minutes');
        }

        function finalPrint(html) {
            var mywindow = window.open();
            var link = '<link rel="stylesheet" href="' + VIS.Application.contextUrl + 'Areas/VIS/Content/VIS.all.min.css" />';

            mywindow.document.write(link + html);

            setTimeout(function () {
                mywindow.print();
                mywindow.close();
            }, 300);

        };

        function downLoadAllAttach(ID) {
            if (ID == null || ID == 0) {
                return;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/DownloadAllAttachments",
                datatype: "json",
                type: "get",
                cache: false,
                data: { ID: ID, Name: name },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result != null && result != "") {
                        var url = VIS.Application.contextUrl + "TempDownload/" + result;
                        window.open(url);
                    }
                    else {
                        VIS.ADialog.error("AttachmentNotFound");
                    }
                }
            });
        };

        function downLoadHistoryAttach(e) {
            var target = e.target;
            var name = '';
            var ID = '';
            var AttID = '';

            if ($(target).is('div')) {
                name = $(target).data('filename');
                AttID = $(target).data('attid');
                ID = $(target).data('id');
            }
            else if ($(target).is('h6')) {
                name = $(target).parent().data('filename');
                AttID = $(target).parent().data('attid');
                ID = $(target).parent().data('id');
            }

            if (ID == null || ID == 0) {
                return;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/DownloadHistoryAttachment",
                datatype: "json",
                type: "get",
                cache: false,
                data: { AttID: AttID, ID: ID, Name: name },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result != null && result != "") {
                        var url = VIS.Application.contextUrl + "TempDownload/" + result;
                        $('#tblAttach').css({ "cursor": "default" });
                        window.open(url);
                    }
                    else {
                        VIS.ADialog.error("AttachmentNotFound");
                    }
                }
            });
        };

        function downLoadAttach(ID, name) {

            if (ID == null || ID == 0) {
                return;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "VIS/HistoryDetailsData/DownloadAttachment",
                datatype: "json",
                type: "get",
                cache: false,
                data: { ID: ID, Name: name },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result != null && result != "") {
                        var url = VIS.Application.contextUrl + "TempDownload/" + result;
                        window.open(url);
                    }
                    else {
                        VIS.ADialog.error("AttachmentNotFound");
                    }
                }
            });
        };

        function downLoadAttachCall(ID, AttID, name) {

            if (ID == null || ID == 0) {
                return;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "AttachmentHistory/DownloadAttachmentCall",
                datatype: "json",
                type: "get",
                cache: false,
                data: { AttID: AttID, ID: ID, Name: name },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result != null && result != "") {
                        var url = VIS.Application.contextUrl + "TempDownload/" + result;
                        window.open(url);
                    }
                    else {
                        VIS.ADialog.error("AttachmentNotFound");
                    }
                }
            });
        };

        function panelAction(_Record_ID, _AD_Table_ID, detailHtml, titleText, attachID, data, e) {
            var email = null;
            var target = e.target;
            var mailto, mailcc, mailbcc;

            if ($(target).is('i') && (data == "R" || data == "RA")) {
                mailto = $(target).data('mailto');
                mailcc = $(target).data('mailcc');
                mailbcc = $(target).data('mailbcc');
            }

            if (data == "R" || data == "RA") {
                email = new VIS.Email(mailto, null, null, _Record_ID, true, true, _AD_Table_ID, detailHtml, 'RE: ' + VIS.Utility.Util.getValueOfString(titleText), attachID);
            }
            else if (data == "F") {
                email = new VIS.Email('', null, null, _Record_ID, true, true, _AD_Table_ID, detailHtml, 'FW: ' + VIS.Utility.Util.getValueOfString(titleText), attachID);
            }
            var c = new VIS.CFrame();
            c.setName(VIS.Msg.getMsg("EMail"));
            c.setTitle(VIS.Msg.getMsg("EMail"));
            c.hideHeader(true);
            c.setContent(email);
            c.show();

            if (data == "RA") {
                email.showCcBccMails(mailcc.replace(/,/g, ''), mailbcc.replace(/,/g, ''));
            }
            email.initializeComponent();
        };

        function saveChat(e) {
            var target = e.target;
            var rec_ID = 0, ch_id = 0; _AD_Table_ID = 876;
            if ($(target).is('i')) {
                rec_ID = $(target).data('recid');
                ch_id = $(target).data('chid');
            }

            var text = $('#VIS_chatBox' + window_No).val();
            if ($.trim(text) == "" || text == "" || text == null) {
                VIS.ADialog.info("EnterData");
                if (e != undefined) {
                    e.preventDefault();
                }
                return false;
            }
            var infoName = 'info';
            var infoDisplay = 'test';
            var chat = new VIS.Chat(rec_ID, ch_id, _AD_Table_ID, infoName + ": " + infoDisplay, this.windowNo);
            chat.prop.ChatText = text;
            chat.onClose = function () {
            }

            VIS.dataContext.saveChat(chat.prop);

            showChat(ch_id, '', window_No);
        };

        function viewAll(record_ID, isAppointment, isCall) {

            $.ajax({
                url: VIS.Application.contextUrl + "AttachmentHistory/ViewChatonHistory",
                datatype: "json",
                type: "POST",
                cache: false,
                data: { record_ID: record_ID, isAppointment: isAppointment, isCall: isCall },
                success: function (data) {
                    var result = JSON.parse(data);
                    createCommentsSection(result, isAppointment, isCall);
                },
                error: function (data) {
                    VIS.ADialog.error("RecordNotSaved");
                }
            });
        };

        function saveComments(isAppointment, isCall, e) {
            var target = e.target;
            if (e.keyCode != undefined) {
                if (e.keyCode != 13) {
                    return;
                }
            }
            if (e.keyCode == undefined && !$(target).hasClass("vis-attachhistory-comment-icon")) {
                return;
            }
            var ID = 0;
            ID = $('#rowId' + _selectedRecId).data("rid");
            if (ID == 0) {
                return;
            }
            var comme = '';
            comme = $('#VIS_txtComments' + window_No).val();

            if (comme.length == 0) {
                return;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "AttachmentHistory/SaveComment",
                datatype: "json",
                type: "POST",
                cache: false,
                data: { ID: ID, Text: comme, isAppointment: isAppointment, isCall: isCall },
                success: function (data) {
                    // var rID = $('#rowId' + recId).data('rid');
                    var result = JSON.parse(data);
                    var updateComment = true;
                    var $commenthtml;
                    var userimg;
                    var value;
                    if (result != null) {
                        value = result.length - 1;
                    }

                    if (result[value].UserImage != "VIS_NoRecordFound" && result[value].UserImage != "VIS_FileDoesn'tExist" && result[value].UserImage != null) {
                        userimg = '<img  class="userAvatar-Feeds"  src="' + result[value].UserImage + '?' + new Date() + '">';
                    }
                    else {
                        userimg = '<i class="fa fa-user userAvatar-Feeds"></i>';
                    }

                    $('#VIS_txtComments' + window_No).val("");

                    if ($('#VIS_commentsdata' + window_No).html().contains("VIS_commentsMsg")) {
                        $commenthtml = $('<div class= "vis-attachhistory-comment-detail VIS-feedDetails-cmnt d-flex" >' +
                            userimg +
                            '<div class="vis-attachhistory-comment-text ml-0">' +
                            '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">Me </h6><div class="vis-attachhistory-comment-dateTime"><p>' + new Date().toLocaleString().replace(",", "").replace(/:.. /, " ") + '</p></div></div>' +
                            '<p class= "VIS-attachhistory-comment-text" >' + comme + '</p></div>' +
                            '</div>');
                        $('#VIS_commentsMsg' + window_No).empty();
                        $('#VIS_commentsMsg' + window_No).append($commenthtml);
                        if ($('#VIS_viewMoreComments' + window_No).is(':visible')) {
                            $('#VIS_viewMoreComments' + window_No).append($commenthtml);
                        }
                        $('#VIS_commentsdata' + window_No).show();
                        $('#VIS_commentsMsg' + window_No).show();


                    }
                    else {
                        $commenthtml = $('<div id="VIS_commentsMsg' + window_No + '"  class="vis-attachhistory-comment-data mb-0 VIS-tp-commentData">' +
                            '<div class= "vis-attachhistory-comment-detail VIS-feedDetails-cmnt d-flex" >' +
                            // '<i class="fa fa-user userAvatar-Feeds"></i>' +
                            userimg +
                            '<div class="vis-attachhistory-comment-text ml-0">' +
                            '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">Me </h6><div class="vis-attachhistory-comment-dateTime"><p>' + new Date().toLocaleString().replace(",", "").replace(/:.. /, " ") + '</p></div></div>' +
                            '<p class= "VIS-attachhistory-comment-text" >' + comme + '</p></div>' +
                            '</div></div>');
                        $('#VIS_commentsdata' + window_No).prepend($commenthtml);
                        if ($('#VIS_viewMoreComments' + window_No).is(':visible')) {
                            $('#VIS_viewMoreComments' + window_No).append($commenthtml);
                        }
                    }

                },
                error: function (data) {
                    VIS.ADialog.error("RecordNotSaved");
                }
            });
        };

        function updateComments(str, result) {

            str += ' <div class="vis-attachhistory-comment-data  mb-0 VIS-tp-commentData"><div class="vis-attachhistory-comment-detail VIS-feedDetails-cmnt d-flex">';

            if (result.UserImage != "VIS_NoRecordFound" && result.UserImage != "VIS_FileDoesn'tExist" && result.UserImage != null) {
                str += '<img  class="userAvatar-Feeds"  src="' + result.UserImage + '?' + new Date() + '">';
            }
            else {
                str += '<i class="fa fa-user userAvatar-Feeds"></i>';
            }
            str += '<div class="vis-attachhistory-comment-text ml-0">';

            if (result.CreatedBy == VIS.Env.getCtx().getAD_User_ID()) {
                str += '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">' + VIS.Msg.getMsg("Me") + " </h6>";
            }
            else {
                str += '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">' + result.UserName + "</h6>"
            }

            str += '<div class="vis-attachhistory-comment-dateTime"><p>' + new Date(result.Created).toLocaleString() + '</p>' + '</div></div>' +
                ' <p class="VIS-attachhistory-comment-text">' + result.CharacterData + '</p>' +
                '</div></div></div>';

            return str;
        };

        function createCommentsSection(result, isappointment, iscall) {
            if (result.length > 0) {
                var str = '<div class="vis-attachhistory-comments-container">';

                for (var i = 0; i < result.length; i++) {
                    str = updateComments(str, result[i]);
                }
                str += '</div>';
                $('#VIS_viewMoreComments' + window_No).show();
                $('#VIS-tp-comments-input' + window_No).hide();
                $('#VIS_viewMoreComments' + window_No).empty();
                $('#VIS_commentsMsg' + window_No).hide();
                $('#VIS_viewAllComments' + window_No).text(VIS.Msg.getMsg('HideComments'));
                $('#VIS_viewMoreComments' + window_No).height($root.find('.VIS-tp-contentdiv').height() - $('#VIS_commentsdata' + window_No).height());
                $('#VIS_viewMoreComments' + window_No).append(str);
            }
            //else
            //    VIS.ADialog.error("NoRecordFound");
        };

        function updateRecentComments(str, result) {

            str += '<div class="vis-attachhistory-comment-detail VIS-feedDetails-cmnt d-flex">';

            if (result.UserImage != "VIS_NoRecordFound" && result.UserImage != "VIS_FileDoesn'tExist" && result.UserImage != null) {
                str += '<img  class="userAvatar-Feeds"  src="' + result.UserImage + '?' + new Date() + '">';
            }
            else {
                str += '<i class="fa fa-user userAvatar-Feeds"></i>';
            }
            str += '<div class="vis-attachhistory-comment-text ml-0">';

            if (result.CreatedBy == VIS.Env.getCtx().getAD_User_ID()) {
                str += '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">' + VIS.Msg.getMsg("Me") + " </h6>";
            }
            else {
                str += '<div class="d-flex justify-content-between VIS-tp-commentHead"><h6 class="VIS-attachhistory-comment-text">' + result.UserName + "</h6>"
            }

            str += '<div class="vis-attachhistory-comment-dateTime"><p>' + new Date(result.Created).toLocaleString() + '</p>' + '</div></div>' +
                ' <p class="VIS-attachhistory-comment-text">' + result.CharacterData + '</p>' +
                '</div></div > ';
            if (!$('#VIS_commentsdata' + window_No).html().contains("VIS_commentsMsg")) {

                str = '<div id="VIS_commentsMsg' + window_No + '" class="vis-attachhistory-comment-data  mb-0 VIS-tp-commentData">' + str + "</div >";
            }
            return str;
        };

        function lastHistoryComment(recid, isAppointment, isCall) {
            $.ajax({
                url: VIS.Application.contextUrl + "AttachmentHistory/ViewChatonLastHistory",
                datatype: "json",
                type: "POST",
                cache: false,
                data: { record_ID: recid, isAppointment: isAppointment, isCall: isCall },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result.length > 0) {
                        var cmt = '';
                        cmt = updateRecentComments(cmt, result[0]);
                        $('#VIS_commentsMsg' + window_No).empty();
                        if ($('#VIS_commentsdata' + window_No).html().contains("VIS_commentsMsg")) {
                            $('#VIS_commentsMsg' + window_No).prepend(cmt);
                        }
                        else
                            $('#VIS_commentsdata' + window_No).prepend(cmt);

                    }
                },
            });
        };

        this.disposeComponent = function () {
            this.record_ID = 0;
            this.windowNo = 0;
            this.curTab = null;
            this.rowSource = null;
            this.panelWidth = null;
            _gridCtrl = null;
            $root.remove();
            $root = null;
            window_No = 0;
        };
    };

    /**      * Invoked when user click on panel icon      */
    HistoryDetailsTabPanel.prototype.startPanel = function (_window_No, curTab) {
        this.windowNo = _window_No; this.curTab = curTab; this.init(); window_No = _window_No; this.table_ID = curTab.getAD_Table_ID();
    };

    /**          * This function will execute when user navigate  or refresh a record          */
    HistoryDetailsTabPanel.prototype.refreshPanelData = function (recordID, selectedRow) {
        this.record_ID = recordID; this.selectedRow = selectedRow; this.update(recordID);
    };

    /**      * Fired When Size of panel Changed      */
    HistoryDetailsTabPanel.prototype.sizeChanged = function (width) {
        this.panelWidth = width;
    };

    /**      * Dispose Component      */
    HistoryDetailsTabPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    /* * Fully qualified class name     */
    VIS.HistoryDetailsTabPanel = HistoryDetailsTabPanel;
})();