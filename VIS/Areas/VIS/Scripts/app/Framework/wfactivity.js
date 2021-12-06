/********************************************************
 * Module Name    :     Application
 * Purpose        :     Show Suspended Workflow And Process further
 * Author         :     Lakhwinder
 * Date           :     20-Oct-2014
  ******************************************************/
; (function (VIS, $) {
    function wfActivity(container, divDetail, workflowActivityData, welcomeScreenFeedsListScroll, $wfSearchShow) {
        var data = null;
        var fulldata = [];
        var dataItemDivs = [];
        var self = this;


        var divSearch = null;
        var divSelectAll = null;
        var $txtSearch = null;
        var $btnSearch = null;
        var $dateFrom = null;
        var $dateTo = null;
        var divScroll = null;
        var $cmbWindows = null;
        var chkSelectAll = null;
        var pageNo = 1, PageSize = 10;
        var p_size = 10, p_no = 1;
        var tabdataLastPage = 0;
        var tabdatacntpage = 0;
        var windowID = 0;
        var winNideID = "0_0";
        var nodeID = 0;
        var search = true;

        //will be true when user click on Select all checkbox. If user uncheck any checkbox then its value become false.
        var selectedAll = false;
        var uncheckall = false;
        // To Add selected workflows in array
        var selectedItems = [];
        var $busyIndicator = $("#divTabMenuDataLoder");
        var $alertTxtTypeCount = $("#hAlrtTxtTypeCount");
        var $divActivity = $("#divfActivity");
        // Right Panel
        var workflowActivity = $('#workflowActivity');

        /**
         *  Create Search design
         */
        function createlayout(refresh) {
            if (!refresh) {
                divSearch = $('<div class="wfactivity-homepage-parentdiv"></div>');

                var divSearchText = $(' <div class="frm-data-col-wrap frm-data-col-2" style="padding-right: 10px;"> <div class="frm-data-search-wrap"><input class="frm-data-col-searchinput" id="homeSearchWorkflow" type="text" placeholder="' + VIS.Msg.getMsg("Search") + '">'
                    + '<button id="btnWorkflowSearch" class="vis-wfSearch-btn"><i class="fa fa-search" aria-hidden="true"></i></button></div></div>');
                $txtSearch = divSearchText.find('#homeSearchWorkflow');
                $btnSearch = divSearchText.find('#btnWorkflowSearch');

                var divWindow = $(' <div class="frm-data-col-wrap frm-data-col-2" style="padding-left: 7px"> <div class="frm-data-search-wrap"><select class="vis-custom-select"></select></div></div>');
                $cmbWindows = divWindow.find('select');

                var divDateFrom = $('<div style="display:none;padding-left: 7px" class="frm-data-col-wrap frm-data-col-2"> <label>' + VIS.Msg.getMsg("FromDate") + '</label><input type="date" placeholder="date"></div>');
                $dateFrom = divDateFrom.find('input');


                var divDateTo = $('<div  style="display:none;padding-right:10px" class="frm-data-col-wrap frm-data-col-2"><label>' + VIS.Msg.getMsg("ToDate") + '</label><input type="date" placeholder="date"></div>');
                $dateTo = divDateTo.find('input');

                divSearch.append(divWindow).append(divSearchText).append(divDateFrom).append(divDateTo);

                divSelectAll = $('<div class="frm-select-all-check"  style="display:none"></div>');

                chkSelectAll = $('<input type="checkbox">');
                divSelectAll.append('<label>' + VIS.Msg.getMsg('SelectAll') + '</label>');
                divSelectAll.append(chkSelectAll);
                $wfSearchShow.addClass('vis-eye-plus');
                $wfSearchShow.removeClass('vis-eye-minus');
                $wfSearchShow.attr('title', VIS.Msg.getMsg("ShowSearch"));

                //loadWindows();
            }

            //load windows everytime , beacuse if user completes all the activites of window, then that window should not be displayed in drop down.
            loadWindows();

            divScroll = $('<div class="wfactivity-homepage-activites" style="padding-right:0px"></div>')

        };

        /**
        *  Load windows to be displayed in dropdown. Shows only that windows for 
        *  which workflow is unprocessed and is in susspeneded state
        */
        function loadWindows(loadData) {
            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetWorkflowWindows",
                dataType: "json",
                type: "POST",
                error: function () {
                    return;
                },
                success: function (result) {
                    if (result) {
                        result = JSON.parse(result);
                        $cmbWindows.empty();
                        $cmbWindows.append('<option value="0_0">' + VIS.Msg.getMsg("SelectWindow") + '</option>');
                        if (result && result.length > 0) {
                            var windowExist = false;
                            for (var i = 0; i < result.length; i++) {
                                $cmbWindows.append('<option value="' + result[i].AD_Window_ID + '_' + result[i].AD_Node_ID + '">' + result[i].WindowName + '</option>');
                                if (result[i].AD_Window_ID + '_' + result[i].AD_Node_ID == winNideID) {
                                    windowExist = true;
                                }
                            }
                            //if user complete some activites of window and then on refresh show that window as selected window.
                            if (winNideID && windowExist) {
                                $cmbWindows.val(winNideID);
                            }
                            else {
                                winNideID = "0_0";
                                $cmbWindows.val("0_0");
                            }
                        }
                        else {
                            winNideID = "0_0";
                            $cmbWindows.val("0_0");
                        }
                    }
                    if (loadData) {
                        searchRecord();
                    }
                }
            });
        }


        /**
             *  Add events to controls
             *
             */
        function eventHandling(refresh) {
            $cmbWindows.off("change");
            $cmbWindows.on("change", windowSelection);
            divScroll.on("scroll", loadOnScroll);
            if (!refresh) {
                $btnSearch.on('click', function () {
                    if (search) {
                        search = false;
                        searchRecord();
                    }
                });

                $txtSearch.on("keydown", function (e) {
                    if (e.keyCode == 13) {
                        if (search) {
                            search = false;
                            searchRecord();
                        }
                    }
                });

                // Validate From date and To Date
                $dateFrom.on('focusout', function () {
                    if ($dateFrom.val() != "" && $dateTo.val() != "" && $dateFrom.val() > $dateTo.val()) {
                        $dateTo.val('');
                    }
                });

                $dateTo.on('focusout', function () {
                    if ($dateFrom.val() != "" && $dateTo.val() != "" && $dateFrom.val() > $dateTo.val()) {
                        $dateTo.val('');
                        VIS.ADialog.info("ToDateMustGreater");
                    }
                });

                /**
                *  Show and hide search and set height activites area .
                *
                */
                $wfSearchShow.on("click", function () {
                    if ($dateTo.is(':visible')) {
                        //divSearch.hide();
                        $($dateFrom.parent()).css('display', 'none');
                        $($dateTo.parent()).css('display', 'none');
                        $dateFrom.val('');
                        $dateTo.val('');
                        $wfSearchShow.attr('title', VIS.Msg.getMsg("ShowSearch"));
                        $wfSearchShow.addClass('vis-eye-plus');
                        $wfSearchShow.removeClass('vis-eye-minus');
                        if (chkSelectAll.is(':visible')) {
                            divScroll.css('top', '65px');
                        }
                        else {
                            divScroll.css('top', '47px');
                        }
                    }
                    else {
                        //divSearch.show();
                        $($dateFrom.parent()).css('display', 'block');
                        $($dateTo.parent()).css('display', 'block');
                        $wfSearchShow.attr('title', VIS.Msg.getMsg("HideSearch"));
                        $wfSearchShow.removeClass('vis-eye-plus');
                        $wfSearchShow.addClass('vis-eye-minus');

                        if (chkSelectAll.is(':visible')) {
                            divScroll.css('top', '118px');
                        }
                        else {
                            divScroll.css('top', '98px');
                        }

                    }
                });

                chkSelectAll.on("click", selectAllActivities);
            }



        };

        /**
        *  Select OR deselect all activites
        *
        */
        function selectAllActivities(e) {
            var selectedChk = divScroll.find('.wfActivity-selectchk');

            //else {
            //    selectedChk.prop('checked', false);
            //}
            if (chkSelectAll.prop('checked') == false) {
                uncheckall = true;
            }

            if (chkSelectAll.prop("checked") == true) {
                selectedAll = true;
            }

            for (var j = 0; j < selectedChk.length; j++) {
                if (chkSelectAll.prop("checked") == true) {
                    if ($(selectedChk[j]).prop('checked') == false) {
                        $(selectedChk[j]).trigger('click');
                    }
                }
                else {
                    if ($(selectedChk[j]).prop('checked') == true) {
                        $(selectedChk[j]).trigger('click');
                    }
                }
            }



            uncheckall = false;
            if (chkSelectAll.prop("checked") == false) {
                $(divScroll.find('.vis-activityContainer')[0]).trigger('click');
            }
        }

        /**
        * on select window, show activies of that window, show select all checkbox
        *
        */
        function windowSelection(e) {
            //chkSelectAll.prop('checked', false);
            workflowActivity.hide();
            divDetail.empty();

            if ($cmbWindows.val() == "0_0") {
                divSelectAll.hide();
                if (chkSelectAll.prop('checked') == true) {
                    chkSelectAll.trigger('click');
                }
                if ($dateTo.is(':visible')) {
                    divScroll.css('top', '98px');
                }
                else {
                    divScroll.css('top', '47px');
                }

            }
            else {
                divSelectAll.hide();
                if ($dateTo.is(':visible')) {
                    divScroll.css('top', '118px');
                }
                else {
                    divScroll.css('top', '67px');
                }
            }
            winNideID = $cmbWindows.val();

            searchRecord();
        };


        function searchRecord() {
            pageNo = 1;
            PageSize = 10;
            workflowActivity.hide();
            divDetail.empty();
            divScroll.empty();
            selectedItems = [];
            selectedItems.length = 0;
            self.AppendRecord(pageNo, PageSize, true);
        };

        this.Load = function (refresh) {
            //  $("#divfeedbsy")[0].style.visibility = "visible";
            if (chkSelectAll && chkSelectAll.prop('checked') == false) {
                workflowActivity.hide();
            }
            container.children().last().remove();
            $busyIndicator.show();
            pageNo = 1;
            PageSize = 10;
            tabdataLastPage = 0;
            tabdatacntpage = 0;
            // selectedAll = false;
            createlayout(refresh);
            selectedItems = [];
            if (refresh) {

                container.find('.wfactivity-homepage-activites').remove();

                container.append(divScroll);
                //selectedItems = [];
                //if (chkSelectAll.prop('checked') == true)
                //    chkSelectAll.trigger('click');
            }
            else {
                container.append(divSearch).append(divSelectAll).append(divScroll);
            }
            eventHandling(refresh);
            windowID = $cmbWindows.val();
            if (windowID && windowID != "0_0") {
                nodeID = windowID.split('_')[1];
                windowID = windowID.split('_')[0];
                if ($dateTo.is(':visible')) {
                    container.find('.wfactivity-homepage-activites').css('top', '118px');
                }
                else {
                    container.find('.wfactivity-homepage-activites').css('top', '68px');
                }

            }
            else {
                nodeID = 0;
                windowID = 0;
                if ($dateTo.is(':visible')) {
                    container.find('.wfactivity-homepage-activites').css('top', '98px');
                }
                else {
                    container.find('.wfactivity-homepage-activites').css('top', '47px');
                }
            }


            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivities",
                data: { pageNo: 1, pageSize: 10, refresh: true, searchText: $txtSearch.val(), "AD_Window_ID": windowID, "dateFrom": $dateFrom.val(), "dateTo": $dateTo.val(), "AD_Node_ID": nodeID },
                dataType: "json",
                type: "POST",

                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    //   $("#divfeedbsy")[0].style.visibility = "hidden";
                    $busyIndicator.hide();
                    VIS.HomeMgr.BindMenuClick();
                    return;
                },
                success: function (dyndata) {
                    $txtSearch.val('');
                    window.setTimeout(function () {
                        VIS.HomeMgr.BindMenuClick();
                        var active = VIS.HomeMgr.getActiveTab();
                        if (active.activeTabType == 3) {
                            if (dyndata.result == null || dyndata.result.LstInfo == null) {
                                $busyIndicator.hide();
                                $divActivity.empty();
                                $divActivity.append(0);
                                $divActivity.show();

                                //$("#sAlrtTxtType").empty();
                                //$("#sAlrtTxtType").append(VIS.Msg.getMsg('WorkflowActivities'));
                                $alertTxtTypeCount.empty();
                                $alertTxtTypeCount.append(0);
                                str = "<p  id='pnorecFound' style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                                container.find('#pnorecFound').remove();
                                divScroll.empty();
                                container.append(str);
                                return;
                            }
                            data = dyndata.result.LstInfo;
                            divScroll.empty();
                            // container.children('.vis-activityContainer').not(':first').remove();

                            if (refresh == true) {
                                //Reset Count Header
                                dataItemDivs = [];
                                fulldata = [];


                            }
                            $divActivity.empty();
                            $divActivity.append(dyndata.result.count);
                            $divActivity.show();
                            $alertTxtTypeCount.empty();
                            $alertTxtTypeCount.append(dyndata.result.count);

                            LoadRecords(0);
                            VIS.HomeMgr.adjustDivSize();
                        }
                        else {
                            return;
                        }
                        // workflowActivityData.refresh();
                    }, 100);
                }
            });
            // LoadRecords();


        };

        var scrollWF = true;
        function loadOnScroll(e) {

            // do something
            if ($(this).scrollTop() + $(this).innerHeight() >= (this.scrollHeight * 0.75) && scrollWF) {//Condition true when 75 scroll is done
                scrollWF = false;
                tabdataLastPage = parseInt($divActivity.html());
                tabdatacntpage = pageNo * PageSize;
                if (tabdatacntpage <= tabdataLastPage) {
                    pageNo += 1;
                    self.AppendRecord(pageNo, PageSize);
                }
                else {
                    scrollWF = true;
                }
            }
        };

        this.AppendRecord = function (pageNo, paeSize, refresh) {
            if (!refresh) {
                refresh = false;
            }

            $busyIndicator.show();

            windowID = $cmbWindows.val();
            nodeID = windowID.split('_')[1];
            windowID = windowID.split('_')[0];

            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivities",
                data: { pageNo: pageNo, pageSize: paeSize, refresh: refresh, searchText: $txtSearch.val(), "AD_Window_ID": windowID, "dateFrom": $dateFrom.val(), "dateTo": $dateTo.val(), "AD_Node_ID": nodeID },
                dataType: "json",
                type: "POST",
                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    $busyIndicator.hide();
                    VIS.HomeMgr.BindMenuClick();
                    search = true;
                    scrollWF = true;
                },
                success: function (dyndata) {
                    $txtSearch.val('');
                    VIS.HomeMgr.BindMenuClick();
                    var active = VIS.HomeMgr.getActiveTab();
                    if (active.activeTabType == 3) {
                        if (dyndata.result) {
                            data = dyndata.result.LstInfo;
                        }
                        else {
                            data = null;
                        }


                        if (refresh == true) {
                            //Reset Count Header
                            dataItemDivs = [];
                            fulldata = [];
                            $divActivity.empty();
                            divScroll.empty();
                            if (dyndata.result) {
                                container.find('#pnorecFound').remove();
                                $divActivity.append(dyndata.result.count);
                            }
                            else {
                                $alertTxtTypeCount.empty();
                                $alertTxtTypeCount.append(0);

                                var str = "<p id='pnorecFound' style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                                container.find('#pnorecFound').remove();
                                container.append(str);
                                $divActivity.append(0);
                            }
                            //$("#hAlrtTxtTypeCount").empty();
                            //$("#hAlrtTxtTypeCount").append(dyndata.result.count);

                        }
                        LoadRecords(pageNo - 1);
                        VIS.HomeMgr.adjustDivSize();

                    }
                    else {
                        search = true;
                    }
                    scrollWF = true;

                    //workflowActivityData.refresh();
                }
            });
        }

        var LoadRecords = function (pageNumber) {

            var divOuter = null;
            //data = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/GetActivities", {})).result;
            if (data == null || data.length == 0) {
                //bsyDiv[0].style.visibility = "hidden";

                $busyIndicator.hide();
                //if ($cmbWindows.val() == "0_0") {

                divSelectAll.hide();
                chkSelectAll.prop('checked', false);

                //}

                search = true;
                return;
            }
            if ($cmbWindows.val() != "0_0") {
                if ($dateTo.is(':visible')) {
                    divScroll.css('top', '118px');
                }
                else {
                    divScroll.css('top', '67px');
                }
                divSelectAll.show();
            }
            else {
                if ($dateTo.is(':visible')) {
                    divScroll.css('top', '98px');
                }
                else {
                    divScroll.css('top', '47px');
                }
                divSelectAll.hide();
                chkSelectAll.prop('checked', false);
            }

            for (var item in data) {
                fulldata.push(data[item]);
                var dataIem = {};
                divOuter = $("<div class='vis-activityContainer' data-id='" + (Number(10 * pageNumber) + Number(item)) + "'>");
                divOuter.on('click', function (e) {
                    $('.vis-activityContainer').removeClass('vis-activeFeed');
                    $(this).addClass('vis-activeFeed');
                    var datatab = $(e.target).data("viswfazoom");
                    if (datatab == 'wfZoom') {
                        $(this).addClass('vis-activeFeed');
                        var id = $(e.target).data("index");
                        zoom(id);
                        search = true;
                        return;
                    }

                    var id = $(this).data("id");
                    var async = true;
                    if (selectedItems.length <= 1 && chkSelectAll.prop('checked') == true) {
                        async = false;
                    }
                    var ids = $(this).find('.wfActivity-selectchk').data('ids');
                    if (selectedItems.length == 1 && ids != selectedItems[0]) {
                        return;
                    }
                    getDetail(dataItemDivs[id].wfActivityID, id, this, async);
                    search = true;
                });
                var divActions = $("<div class='vis-feedTitleBar'>");

                // create checkbox to be added in header on workflow activities
                var chkSelect = $('<input class="wfActivity-selectchk" type="checkbox" style="float:left;margin-right:5px" data-ids="' + data[item].AD_Window_ID + "_" + data[item].AD_Node_ID + "_" + data[item].AD_WF_Activity_ID + '" ></input>');

                var header = $("<h3>");
                header.css('font-weight', 'normal');
                //header.css('color', '#1b95d7');
                header.append(VIS.Utility.encodeText(data[item].NodeName));
                // Add Only if any window is selected in search criteria
                if ($cmbWindows && $cmbWindows.val() != "0_0") {
                    divActions.append(chkSelect);
                }

                divActions.append(header);
                var divBtns = $("<div class='vis-feedTitleBar-buttons'>");
                var ul = $("<ul>");
                var liZoom = $("<li>");
                var aZoom = $("<a href='javascript:void(0)'  data-index='" + (Number(10 * pageNumber) + Number(item)) + "' data-viswfazoom='wfZoom' >").append('<i class="vis vis-find" data-index="' + (Number(10 * pageNumber) + Number(item)) + '" data-viswfazoom="wfZoom"></i>');
                //aZoom.append("View Feed");

                liZoom.append(aZoom);
                ul.append(liZoom);

                divBtns.append(ul);

                divActions.append(divBtns);

                divOuter.append(divActions);



                var divContent = $("<div class='vis-feedDetails'>");


                var para = $("<pre>");
                if (data[item].DocumentNameValue == undefined || data[item].DocumentNameValue == '') {

                    // Get Workflow Activity details from selected Text Template

                    //var summary = VIS.Utility.encodeText(data[item].Summary).split("●");
                    //if (summary != null && summary.length > 0) {
                    //    for (var k = 0; k < summary.length; k++) {
                    //        para.append(summary[k]);
                    //        if (k < summary.length - 1) {
                    //            para.append("<br>");
                    //        }
                    //    }
                    //}
                    para.append(VIS.Utility.encodeText(data[item].Summary));
                }
                else {
                    para.append(VIS.Utility.encodeText(data[item].DocumentNameValue + " - " + data[item].Summary));
                }

                var br = $("<br>");
                para.append(br);
                para.append(VIS.Utility.encodeText(VIS.Msg.getMsg('Priority') + " " + data[item].Priority));

                divContent.append(para);


                //Checkbox Click Event
                chkSelect.on("click", function (e) {
                    var target = $(e.target);
                    onCheckSelectClick(target, e);
                });



                var pdate = $("<div class='vis-feedDateTime'>");
                pdate.append($("<br>")).append(Globalize.format(new Date(data[item].Created), 'd'));
                divContent.append(pdate);

                divOuter.append(divContent);



                divScroll.append(divOuter);
                dataIem.ctrl = divOuter;
                dataIem.index = item;

                dataIem.recordID = data[item].Record_ID;
                dataIem.wfActivityID = data[item].AD_WF_Activity_ID;

                dataItemDivs.push(dataIem);

                if (chkSelectAll.prop('checked') == true) {
                    chkSelect.trigger('click');
                    if (fulldata.length == 1) {
                        //   divOuter.trigger('click');
                    }
                }
            }
            search = true;
            $busyIndicator.hide();


        };


        var onCheckSelectClick = function (target, e) {
            var ite = target.data('ids');

            if (target.prop("checked") == false) {
                chkSelectAll.prop('checked', false);
                selectedAll = false;
            }



            if (target.prop("checked") == true) {
                if (selectedItems.length == 0) {
                    selectedItems.push(target.data('ids'));
                    selectedAll = false;
                    checkSelectAll();
                }
                else {
                    var its = selectedItems[0].split('_');
                    var ids = ite.split('_');

                    //Match if selected item is have same node ID and window ID, only then add this
                    //item to array, otherwise no action.
                    if (its[0] + "-" + its[1] == ids[0] + "-" + ids[1]) {
                        selectedItems.push(ite);
                        if (chkSelectAll.prop('checked') == true && !selectedAll) {
                            e.stopPropagation();
                        }
                        selectedAll = false;
                        checkSelectAll();
                    }
                    else {
                        checkSelectAll();
                        e.preventDefault();
                        return false;
                    }
                }
            }
            else {// If uncheck checkbox, then remove item from array
                selectedItems = jQuery.grep(selectedItems, function (value) {
                    return value != ite;
                });
                if (selectedItems.length == 1 && !uncheckall) {
                    var chks = divScroll.find('.wfActivity-selectchk');
                    //$.each($("input[class='wfActivity-selectchk']:checked"), function () {
                    //    //favorite.push($(this).val());
                    //    $(this).trigger("click");
                    //    return;
                    //});


                    for (var h = 0; h < chks.length; h++) {
                        if ($(chks[h]).prop('checked') == true) {
                            $(chks[h]).parent().trigger('click');
                            e.stopPropagation();
                            return;
                        }

                    }


                }
                if (selectedItems.length >= 0 || chkSelectAll.prop('checked') == true) {
                    e.stopPropagation();
                    return false;
                }
            }


            if (target.prop("checked") == true) {
                //wfActivity-selectchk

            }

        }

        function checkSelectAll() {
            var selectedchecks = divScroll.find('input[class="wfActivity-selectchk"]:checked').length;
            var totalchecks = divScroll.find('.wfActivity-selectchk').length;
            if (totalchecks == selectedchecks) {
                chkSelectAll.prop('checked', true);
            }
        };


        var adjust_size = function () {
            //alert( "adjust_size called." );
            var windowWidth = $(window).width();
            var divCount = $(".row.scrollerHorizontal > div").length
            divCount = divCount - 1;
            $('.scrollerHorizontal').width(windowWidth);
            var sectionsWidth = windowWidth / divCount - 20;
            var sectionsWidthFinal = parseInt(sectionsWidth);

            /* latop and large display screen size */
            if (windowWidth >= 1366) {
                if (workflowActivity.css('display') == 'none') {
                    //alert(divCount);
                    $("#fllupsScreen").css("left", (sectionsWidthFinal + 20));
                    $("#favLinks").css("left", (sectionsWidthFinal * 2 + 40));
                    $('#welcomeScreen,#fllupsScreen,#favLinks').width(sectionsWidthFinal);
                }
                else if (workflowActivity.css('display') == 'block') {
                    var newWidth = windowWidth + sectionsWidthFinal + 20;
                    $('.scrollerHorizontal').width(newWidth);
                    // horizontalScroll.refresh();
                    //alert(newWidth);
                    $("#workflowActivity").css("left", (sectionsWidthFinal + 20));
                    $("#fllupsScreen").css("left", (sectionsWidthFinal * 2 + 40));
                    $("#favLinks").css("left", (sectionsWidthFinal * 3 + 60));
                    $('#welcomeScreen,#fllupsScreen,#favLinks,#workflowActivity').width(sectionsWidthFinal);
                }
            }
        };


        var lstDetailCtrls = [];


        var getDetail = function (wfActivityID, index, ctrl, async) {

            //$("#divfeedbsy")[0].style.visibility = "visible";
            $busyIndicator.show();
            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivityInfo",
                dataType: "json",
                type: "POST",
                async: async,
                data: {
                    activityID: wfActivityID,
                    nodeID: fulldata[index].AD_Node_ID,
                    wfProcessID: fulldata[index].AD_WF_Process_ID
                },
                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    // $("#divfeedbsy")[0].style.visibility = "hidden";
                    $busyIndicator.hide();
                    return;
                },
                success: function (res) {

                    //Load Activity info, only if one workflow is selected. 
                    //otherwise don't show workflow related information
                    if (selectedItems.length <= 1 || chkSelectAll.prop('checked') == true) {
                        workflowActivity.show();
                        VIS.HomeMgr.adjustDivSize();
                        var selectedEffect = "transfer";
                        var options = {};
                        options = { to: divDetail, className: "wsp-ui-effects-transfer" };
                        window.setTimeout(function () {
                            loadDetail(wfActivityID, index, res.result);
                            $(ctrl).effect(selectedEffect, options, 500, function () {
                            });
                        }, 400);
                    }
                    else {
                        if (selectedItems.length <= 2) {
                            loadDetail(wfActivityID, index, res.result);
                        }
                        $busyIndicator.hide();
                    }
                }
            });

        };

        var loadDetail = function (wfActivityID, index, info) {


            var detailCtrl = {};
            lstDetailCtrls = [];
            detailCtrl.Index = index;
            var docnameval;
            // var info = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/GetActivityInfo", { "activityID": wfActivityID, "nodeID": data[index].AD_Node_ID, "wfProcessID": data[index].AD_WF_Process_ID })).result;


            divDetail.empty();

            var divHeader = $("<div class='vis-workflowActivityDetails-Heading'>");
            divDetail.append(divHeader);

            var hHeader = $("<h3>");
            hHeader.append(VIS.Msg.getMsg('Detail'));
            divHeader.append(hHeader);
            // if  any checkbox is checked, then don't show History in middle panel.
            if (selectedItems.length <= 1) {

                //info.ColName == 'VADMS_IsSigned'
                if (info.ColName == 'VADMS_SignStatus') {

                    docnameval = fulldata[index].DocumentNameValue.split('_');

                    var docno = {
                        DocumentNo: parseInt(docnameval[docnameval.length - 1])
                    };

                    var folderofDoc = '';
                    // Get certificate status
                    $.post(VIS.Application.contextUrl + 'VADMS/Document/GetFolderID', docno, function (res) {
                        if (res && res.result != '' && !res.result.contains('ERR-') && !res.result.contains('F')) {
                            folderofDoc = parseInt(res.result);
                        }
                        else {
                            VIS.ADialog.error(VIS.Msg.getMsg("VA055_FolderNotFound"));
                        }
                    }, 'json').fail(function (jqXHR, exception) {
                        VIS.ADialog.error(exception);
                    });

                    var formName = {
                        FromName: 'VADMS_Document'
                    };

                    var formID = '';
                    // Get certificate status
                    $.post(VIS.Application.contextUrl + 'VADMS/Document/GetFormID', formName, function (res) {
                        if (res && res.result != '') {
                            formID = res.result;
                        }
                        else {
                            VIS.ADialog.error(VIS.Msg.getMsg("VA055_FormNotFound"));
                        }
                    }, 'json').fail(function (jqXHR, exception) {
                        VIS.ADialog.error(exception);
                    });

                    // Dms Zoom
                    var aZoomDMS = $("<a href='javascript:void(0)' class='vis-btn-zoom' style='margin-left:10px;' data-id='" +
                        docnameval[docnameval.length - 1] +
                        "'>");
                    aZoomDMS.append($("<span class='vis-btn-ico vis-btn-zoom-bg vis-btn-zoom-border'>"));
                    aZoomDMS.append(VIS.Msg.getMsg('Zoom'));
                    divHeader.append(aZoomDMS);

                    aZoomDMS.on(VIS.Events.onTouchStartOrClick, function (e) {
                        var id = $(this).data("id");

                        var $additionalInfo = {
                            DocNo: id,
                            DocFolderID: folderofDoc
                        };
                        if (formID > 0) {
                            VIS.viewManager.startForm(formID, $additionalInfo);
                        }
                        else {

                        }
                    });
                }
                else {
                    var aZoom = $("<a href='javascript:void(0)' class='vis-btn-zoom' data-id='" + index + "'>");
                    //aZoom.css("data-id", index);
                    aZoom.append($("<span class='vis-btn-ico vis vis-find vis-btn-zoom-border'>"));
                    aZoom.append(VIS.Msg.getMsg('Zoom'));
                    divHeader.append(aZoom);
                    aZoom.on(VIS.Events.onTouchStartOrClick, function (e) {
                        var id = $(this).data("id");
                        zoom(id);
                    });
                }
            }

            divHeader.append($("<div class='clearfix'>"));

            var ul = $("<ul class='vis-IIColumnContent'>");
            divDetail.append(ul);

            var li1 = $("<li>");
            li1.css('width', '100%');
            var p1 = $("<p>");
            p1.append(VIS.Msg.getMsg('Node'));
            p1.append($("<br>"));
            p1.append(VIS.Utility.encodeText(fulldata[index].NodeName));
            li1.append(p1);
            ul.append(li1);


            // var li2 = $("<li>");
            // if  any checkbox is checked, then don't show summary in middle panel.
            if (selectedItems.length <= 1) {
                var p2 = $("<pre>");
                p2.css('margin-top', '10px');
                p2.css('margin-bottom', '0px');
                p2.css('font-size', '14px');
                p2.css('font-family', 'NoirPro-Regular');
                p2.append(VIS.Msg.getMsg('Summary'));
                p2.append($("<br>"));

                // Get Workflow Activity details from selected Text Template

                //var summary = VIS.Utility.encodeText(fulldata[index].Summary).split("●");
                //if (summary != null && summary.length > 0) {
                //    for (var k = 0; k < summary.length; k++) {
                //        p2.append(summary[k]);
                //        if (k < summary.length - 1) {
                //            p2.append("<br>");
                //        }
                //    }
                //}

                p2.append(VIS.Utility.encodeText(fulldata[index].Summary));
                li1.append(p2);
            }
            //ul.append(li2);

            divDetail.append($("<div class='clearfix'>"));

            var hDesc = $("<h4>");
            hDesc.append(VIS.Msg.getMsg('Description'));
            divDetail.append(hDesc);
            var pDesc = $("<p>");
            pDesc.append(VIS.Utility.encodeText(fulldata[index].Description));
            divDetail.append(pDesc);

            divDetail.append($("<div class='clearfix'>"));

            var hHelp = $("<h4>");
            //hHelp.append($("<span class='vis-workflowActivityIcons vis-icon-help'>"))
            hHelp.append(VIS.Msg.getMsg('Help'));
            divDetail.append(hHelp);
            var pHelp = $("<p>");
            pHelp.append(VIS.Utility.encodeText(fulldata[index].Help));
            divDetail.append(pHelp);

            divDetail.append($("<h3>").append(VIS.Msg.getMsg('Action')));
            divDetail.append($("<div class='clearfix'>"));

            var ulA = $("<ul class='vis-IIColumnContent'>");

            var liAInput = $("<li>");
            ulA.append(liAInput);
            liAInput.append($("<p style='margin-bottom: 0'>").append(VIS.Msg.getMsg('Answer')));
            //Get Answer Control

            if (info.NodeAction == 'C') {
                var ctrl = getControl(info, wfActivityID);
                detailCtrl.AnswerCtrl = ctrl;
                if (ctrl != null) {


                    if (ctrl.getBtnCount() > 0) {
                        var divFwd = $("<div class='vis-wforwardwrap'>");
                        divFwd.append(ctrl.getControl());
                        divFwd.append(ctrl.getBtn(0));
                        liAInput.append(divFwd);

                    }
                    else {
                        liAInput.append(ctrl.getControl());
                    }
                    detailCtrl.AnswerCtrl = ctrl;
                }
                detailCtrl.Action = 'C';
            }
            else if (info.NodeAction == 'W') {
                var ansBtn = $('<button style="margin-bottom:10px;margin-top: 0px;width: 100%;" class="VIS_Pref_pass-btn" data-id="' + index + '" data-window="' + info.AD_Window_ID + '" data-col="' + info.KeyCol + '">').append(info.NodeName);
                detailCtrl.AnswerCtrl = ansBtn;
                liAInput.append(ansBtn);
                ansBtn.on('click', function () {

                    ansBtnClick($(this).data("id"), $(this).data("window"), $(this).data("col"));
                });
                detailCtrl.Action = 'W';
            }
            else if (info.NodeAction == 'X') {
                var ansBtn = $('<button style="margin-bottom:10px;margin-top: 0px;width: 100%;" class="VIS_Pref_pass-btn" data-id="' + index + '" data-form="' + info.AD_Form_ID + '" data-col="' + info.KeyCol + '">').append(info.NodeName);
                detailCtrl.AnswerCtrl = ansBtn;
                liAInput.append(ansBtn);
                ansBtn.on('click', function () {
                    VIS.viewManager.startForm($(this).data("form"));
                });
                detailCtrl.Action = 'X';
            }


            liAInput.append($("<p style='margin-bottom: 0'>").append(VIS.Msg.getMsg('Forward')));

            //Get User Lookup
            var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, "AD_User_ID", 0, false, null);
            var txtb = new VIS.Controls.VTextBoxButton("AD_User_ID", false, false, true, VIS.DisplayType.Search, lookup);
            detailCtrl.FwdCtrl = txtb;
            txtb.getBtn();

            if (txtb.getBtnCount() == 2) {
                var divFwd = $("<div class='vis-wforwardwrap'>");
                divFwd.append(txtb.getControl());
                divFwd.append(txtb.getBtn(0));
                liAInput.append(divFwd);

            }

            //Add Vtextbox button for User Selection

            var liDoit = $("<li>");
            var aOk = $("<a href='javascript:void(0)' class='vis-btn vis-btn-done vis-icon-doneButton vis-workflowActivityIcons' data-clicked='N' data-id='" + index + "'>");
            //aOk.css("data-id",index);
            aOk.append($("<span class='vis-btn-ico vis-btn-done-bg vis-btn-done-border'>"));
            aOk.append(VIS.Msg.getMsg('Done'));
            liDoit.append(aOk);
            aOk.on(VIS.Events.onTouchStartOrClick, function (e) {
                if (aOk.data('clicked') == 'Y') {
                    return;
                }
                aOk.data('clicked', 'Y');
                // Digital signature work - Apply default sign at default location with selected status
                if (window.VA055 && window.VADMS && info.ColName == 'VADMS_SignStatus') {

                    var signData = {
                        documentNo: docnameval[docnameval.length - 1],
                        defaultReasonKey: $('[name="VADMS_SignStatus"]').children("option:selected").val(),
                        defaultReason: $('[name="VADMS_SignStatus"]').children("option:selected").text(),
                        //defaultDigitalSignatureID: $('#pdfsignreason').children("option:selected").data('digitalsignatureid')
                    };

                    if (signData.defaultReasonKey == undefined || signData.defaultReasonKey == '' || signData.defaultReason == undefined || signData.defaultReason == '') {
                        aOk.data('clicked', 'N');
                        VIS.ADialog.info('VA055_ChooseStatus');
                        return;
                    }

                    $.post(VIS.Application.contextUrl + 'VADMS/Document/SignatureUsingWorkflow', signData, function (res) {

                        if (res && res != 'null' && res.result == 'success') {

                            $("#divfeedbsy")[0].style.visibility = "hidden";
                            divScroll.empty();
                            adjust_size();
                            lstDetailCtrls = [];
                            selectedItems = [];
                            $busyIndicator.show();

                            window.setTimeout(function () {
                                loadWindows(true);
                            }, 5000);
                        }
                        else {
                            aOk.data('clicked', 'N');
                            VIS.ADialog.error(res.result);
                        }

                    }, 'json').fail(function (jqXHR, exception) {
                        aOk.data('clicked', 'N');
                        VIS.ADialog.error(exception);
                    });
                }
                else {
                    var id = $(this).data("id");
                    approveIt(id, aOk);
                }


            });
            ulA.append(liDoit);

            divDetail.append(ulA);
            divDetail.append($("<div class='clearfix'>"));

            divDetail.append($("<p style='margin-bottom: 0'>").append(VIS.Msg.getMsg('Message')));
            divDetail.append($("<div class='clearfix'>"));

            var divMsg = $("<div class='vis-sendMessage'>");
            var msg = $("<input type='text' placeholder='" + VIS.Msg.getMsg('TypeMessage') + "....'>");
            detailCtrl.MsgCtrl = msg;
            divMsg.append(msg);
            divMsg.append($("<button class='vis vis-sms'></button>"));
            divMsg.append($("<div class='clearfix'>"));

            divDetail.append(divMsg);



            lstDetailCtrls.push(detailCtrl);

            // if  any checkbox is checked, then don't show History in middle panel.
            if (selectedItems.length <= 1) {

                divDetail.append($("<h3>").append(VIS.Msg.getMsg('History')));
                divDetail.append($("<div class='clearfix'>"));

                var divHistory = $("<div class='vis-history-wrap'>");
                divDetail.append(divHistory);

                if (info.Node != null) {
                    var divHistoryNode = $("<div style='margin-top:15px;margin-bottom:15px'>");

                    for (node in info.Node) {

                        if (info.Node[node].History != null) {
                            for (hNode in info.Node[node].History) {

                                if (info.Node[node].History[hNode].State == 'CC'
                                    && node < (info.Node.length - 1)) {
                                    divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/4.jpg'>")));
                                    var divAppBy = $("<div class='vis-approved_wrap'>");
                                    var nodename = '';
                                    nodename = info.Node[node].Name;



                                    var divLeft = $("<div class='vis-left-part'>");
                                    if (info.Node[node].History[hNode].TextMsg.length > 0) {
                                        var btnDetail = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip' style='margin-right:5px'>").append($("<img class='VIS_Pref_img-i'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/i.png"));
                                        var span = $("<span>");
                                        span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                                        span.append($("<label class='VIS_Pref_Label_Font'>").append(VIS.Utility.encodeText(info.Node[node].History[hNode].TextMsg)))
                                        btnDetail.append(span);

                                        divLeft.append(btnDetail);
                                    }
                                    divLeft.append(nodename);
                                    divAppBy.append(divLeft);
                                    var divRight = $("<div class='vis-right-part'>");
                                    divRight.append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy));
                                    //divRight.append(btnDetail);
                                    divAppBy.append(divRight);
                                    divHistoryNode.append(divAppBy);

                                }
                                else if ((node < (info.Node.length - 1)) || info.Node.length == 1) {
                                    var divAppBy = $("<div class='vis-pending_wrap' style='margin-top:-2px'>");
                                    divAppBy.append($("<div class='vis-left-part'>").append(info.Node[node].Name));
                                    divAppBy.append($("<div class='vis-right-part'>").append(VIS.Msg.getMsg('Pending')));
                                    divHistoryNode.append(divAppBy);
                                    //divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='/ViennaAdvantageWeb/Areas/VIS/Images/home/4.jpg'>")));
                                }
                                else {
                                    divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/4.jpg'>")));
                                    var divStart = $("<div class='vis-start_wrap' style='margin-bottom:-8px'>");


                                    var divLeft = $("<div class='vis-left-part'>");
                                    if (info.Node[node].History[hNode].TextMsg.length > 0) {
                                        var btnDetail = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip' style='margin-right:5px'>").append($("<img class='VIS_Pref_img-i'>").attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/i.png"));
                                        var span = $("<span >");
                                        span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                                        span.append($("<label class='VIS_Pref_Label_Font'>").append(VIS.Utility.encodeText(info.Node[node].History[hNode].TextMsg)))
                                        btnDetail.append(span);

                                        divLeft.append(btnDetail);
                                    }
                                    divLeft.append(info.Node[node].Name);

                                    divStart.append(divLeft);
                                    var divRight = $("<div class='vis-right-part'>");
                                    divRight.append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy));
                                    //divRight.append(btnDetail);
                                    divStart.append(divRight);
                                    // divStart.append($("<div class='vis-right-part'>").append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy)));
                                    divHistoryNode.append(divStart);
                                }
                            }
                            divHistory.append(divHistoryNode);
                        }


                    }
                }
            }
            //  $("#divfeedbsy")[0].style.visibility = "hidden";
            $busyIndicator.hide();
        };

        var zoom = function (index) {

            // vinay bhatt window id
            VIS.AEnv.wfzoom(fulldata[index].AD_Table_ID, fulldata[index].Record_ID, fulldata[index].AD_WF_Activity_ID);
            //

            /*
            //alert(index);
            //data[index].Record_ID;
 
            var sql = "SELECT TableName, AD_Window_ID, PO_Window_ID FROM AD_Table WHERE AD_Table_ID=" + fulldata[index].AD_Table_ID;
 
 
            //var sql = "select ad_window_id from ad_window where name='Mail Configuration'";// Upper( name)=Upper('user' )
            var ad_window_Id = -1;
            var tableName = '';
            try {
                var dr = VIS.DB.executeDataReader(sql);
                if (dr.read()) {
                    ad_window_Id = dr.getInt(1);
                    tableName = dr.get(0);
                }
                dr.dispose();
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql, e);
            }
 
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(tableName + "_ID", VIS.Query.prototype.EQUAL, fulldata[index].Record_ID);
            VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
            */
        };

        var approveIt = function (index, aOK) {
            var aOK = aOK;
            $("#divfeedbsy")[0].style.visibility = "visible";
            window.setTimeout(function () {
                for (var item in lstDetailCtrls) {
                    try {
                        if (index === lstDetailCtrls[item].Index) {
                            var fwdTo = lstDetailCtrls[item].FwdCtrl.getValue();
                            var msg = VIS.Utility.encodeText(lstDetailCtrls[item].MsgCtrl.val());
                            var answer = null;
                            if (lstDetailCtrls[item].Action == 'C') {
                                var answer = lstDetailCtrls[item].AnswerCtrl.getValue();

                            }

                            //var info = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/ApproveIt",
                            //    { "activityID": fulldata[index].AD_WF_Activity_ID, "nodeID": fulldata[index].AD_Node_ID, "txtMsg": msg, "fwd": fwdTo, "answer": answer })).result;

                            var activitIDs = "";
                            // if checkbox is selected, then join activity ID using comma splitter.
                            if (selectedItems && selectedItems.length > 0) {
                                for (var k = 0; k < selectedItems.length; k++) {
                                    if (activitIDs.length > 0) {
                                        activitIDs += ",";
                                    }
                                    activitIDs += selectedItems[k].split("_")[2];
                                }
                            }
                            else {
                                activitIDs = fulldata[index].AD_WF_Activity_ID;
                            }

                            // set window ID of activity
                            windowID = fulldata[index].AD_Window_ID;

                            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/ApproveIt",
                                { "activityID": activitIDs, "nodeID": fulldata[index].AD_Node_ID, "txtMsg": msg, "fwd": fwdTo, "answer": answer, "AD_Window_ID": windowID }, function apprvoIt(info) {

                                    if (info.result == '') {
                                        //refresh
                                        //alert("Done");
                                        $("#divfeedbsy")[0].style.visibility = "hidden";
                                        aOK.data('clicked', 'N');
                                        //window.setTimeout(function () {
                                        //    $('#workflowActivity').hide();
                                        //}, 200);
                                        divScroll.empty();
                                        adjust_size();
                                        lstDetailCtrls = [];
                                        selectedItems = [];
                                        // $("#divfeedbsy")[0].style.visibility = "visible";
                                        $busyIndicator.show();
                                        loadWindows(true);

                                    }
                                    else {

                                        alert(VIS.Msg.getMsg(info.result));
                                        aOK.data('clicked', 'N');
                                        $("#divfeedbsy")[0].style.visibility = "hidden";
                                    }
                                });
                            break;
                        }
                    }
                    catch (e) {

                        alert('FillManadatory');
                        aOK.data('clicked', 'N');
                        $("#divfeedbsy")[0].style.visibility = "hidden";
                    }

                }
                aOK.data('clicked', 'N');

            }, 2);

        };

        var getControl = function (info, wfActivityID) {


            var ctrl = null;

            if (info.ColID == 0) {
                return ctrl;
            }


            if (info.ColReference == VIS.DisplayType.YesNo) {

                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.List, info.ColName, 319, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColReference == VIS.DisplayType.List) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.List, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColName.toUpperCase() == "C_GENATTRIBUTESETINSTANCE_ID") {
                // alert('Gen Attribute Not Implement Yet');
                var vAttSetInstance = null;
                var lookupCur = new VIS.MGAttributeLookup(VIS.context, 0);
                $.ajax({
                    url: VIS.Application.contextUrl + "WFActivity/GetRelativeData",
                    async: false,
                    data: { activityID: wfActivityID },
                    dataType: "json",
                    success: function (dyndata) {
                        if (dyndata.result) {
                            vAttSetInstance = new VIS.Controls.VPAttribute('C_GenAttributeSetInstance', true, false, true, VIS.DisplayType.PAttribute, lookupCur, 0, true, false, false, false);
                            vAttSetInstance.SetC_GenAttributeSet_ID(dyndata.result.GenAttributeSetID);
                        }
                    }
                });


                return vAttSetInstance;
            }
            else if (info.ColReference == VIS.DisplayType.TableDir) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.TableDir, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColReference == VIS.DisplayType.Search) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VTextBoxButton(info.ColName, false, false, true, VIS.DisplayType.Search, lookup);
                return ctrl;
            }
            else {
                ctrl = new VIS.Controls.VTextBox(info.ColName, false, false, true, 50, 100, null, null, false);
                return ctrl;
            }
        };

        var ansBtnClick = function (index, AD_Window_ID, columnName) {
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(columnName, VIS.Query.prototype.EQUAL, fulldata[index].Record_ID);
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
        };

        this.clear = function () {

            for (var itm in dataItemDivs) {
                dataItemDivs[itm] = null;
            }

        };

    }; VIS.wfActivity = wfActivity;
})(VIS, jQuery);