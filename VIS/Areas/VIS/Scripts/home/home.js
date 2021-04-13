; (function (VIS, $) {
    function HomeMgr() {
        /*Start initialize  globale variable for home page*/
        var $home = null;
        var horizontalScroll = null;
        var FllUpsMain = null;
        var workflowActivityData = null;
        var userLinksScroll = null;
        var userFavsScroll = null;
        var WelcomeTabDatacontainers = null;
        var $hlnkRefTabData = null;
        var favLinks = null;
        //to get click type of tab menu
        var activeTabType = null;
        var $workflowActivity = null;
        var isWsp = null;
        var $divTabContainerLoder = null;
        var $workflowActivityDetails = null;
        var $WelcomeScreenHdr = null;
        var activity = null;
        var isTabAjaxBusy = null;
        var isTabDataRef = null;
        //menu tab object
        var $ulHomeTabMenu = null;
        //loder of menu tab
        var $divTabMenuDataLoder = null;
        /* count div object of menu tab  start*/
        var $divNoticeCount = null;
        var $divRequestCount = null;
        var $divNotesCount = null;
        var $divAptCount = null;
        var $divKPICount = null;
        //welcomeScreenHeader
        var $sAlrtTxtType = null, $hAlrtTxtTypeCount = null;
        /* count div object of menu tab  end*/
        var $spanWelcomeTabtopHdr = null;

        /*Search Data*/
        var $searchData = null;
        //set Follups Enter Key Code
        var EnterKeyCode = 13;
        //1=Profile,2=Request,3=Workflow,4=Appointments,5=MyTask,6=Task Assign By Me,7=Document receave,8=Notice,9=Notes,10=KPI
        var ProfileType = 1, RequestType = 2, WorkflowType = 3, AppointmentsType = 4, MyTaskType = 5, TaskAssignByme = 6, DocumentReceaveType = 7, NoticeType = 8, NotesType = 9, kPIType = 10;
        //PageNo start from 1 for scroll event
        //Page Size start from 10 for Scroll event
        var p_size = 10, p_no = 1;
        //Pagging varibale declaration
        var isTabscroll = null, tabdatapcount = null, tabdataPageSize = null, tabdataPage = null, tabdataLastPage = null, tabdatacntpage = null;
        //declare Vis newrecord  variable
        var $welcomeNewRecord = null;
        var $wfSearchShow = null;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
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



        /*end initialize  globale variable for home page*/
        function initHome(home) {
            $home = home;
            loadHome();
        }

        //to adjust div of home page
        function adjustDivSize() {
            //alert("call adjust");   
            var windowWidth = $(window).width();
            var divCount = $(".row.scrollerHorizontal > div").length
            divCount = divCount - 1;
            $('.scrollerHorizontal').width(windowWidth);
            var sectionsWidth = windowWidth / divCount - 20;
            var sectionsWidthFinal = parseInt(sectionsWidth);
            var align = {};
            if (VIS.Application.isRTL) {
                $('#welcomeScreen').css({ "right": "0", "left": "auto" });
                //$("#fllupsScreen").css("padding-left", "0px");
                //$("#favLinks").css("padding-left", "0px");
                //$('#welcomeScreen').css("padding-left", "0px");
                //$('.row').css("margin-left", "-10px");
                $('.vis-welcomeScreenFeeds > .row').css('margin-right', '0px');
                //$('.scrollerHorizontal').css({ 'margin-left': '0px', 'margin-right': '' });
            }
            /* latop and large display screen size */
            if (windowWidth >= 1366) {
                if ($('#workflowActivity').css('display') == 'none') {
                    //alert(divCount);
                    $('#welcomeScreen').width(sectionsWidthFinal);


                    if (VIS.Application.isRTL) {
                        align = {
                            width: sectionsWidthFinal,
                            right: sectionsWidthFinal + 20,
                        };
                    }
                    else {
                        align = {
                            width: sectionsWidthFinal,
                            left: sectionsWidthFinal + 20,
                        };
                    }

                    $("#fllupsScreen").animate(align, 200, function () {
                        // Animation complete.
                    });

                    if (VIS.Application.isRTL) {
                        align = {
                            width: sectionsWidthFinal,
                            right: sectionsWidthFinal * 2 + 40,
                        };
                    }
                    else {
                        align = {
                            width: sectionsWidthFinal,
                            left: sectionsWidthFinal * 2 + 40,
                        };
                    }


                    $("#favLinks").animate(align, 200, function () {
                        // Animation complete.
                    });
                    //$("#feedsScreen").css("left" , (sectionsWidthFinal + 20));
                    //$("#favLinks").css("left" , (sectionsWidthFinal * 2 + 40));
                }
                else if ($('#workflowActivity').css('display') == 'block') {
                    var newWidth = windowWidth + sectionsWidthFinal + 20;
                    $('.scrollerHorizontal').width(newWidth);
                    horizontalScroll.scroll();
                    //alert(newWidth);
                    $('#welcomeScreen').width(sectionsWidthFinal);



                    if (VIS.Application.isRTL) {
                        align = {
                            width: sectionsWidthFinal,
                            right: sectionsWidthFinal + 20,
                        };
                    }
                    else {
                        align = {
                            width: sectionsWidthFinal,
                            left: sectionsWidthFinal + 20,
                        };
                    }


                    $("#workflowActivity").animate(align, 200, function () {
                        workflowActivityData.scroll();
                    });

                    if (VIS.Application.isRTL) {
                        align = {
                            width: sectionsWidthFinal,
                            right: sectionsWidthFinal * 2 + 40,
                        };
                    }
                    else {
                        align = {
                            width: sectionsWidthFinal,
                            left: sectionsWidthFinal * 2 + 40,
                        };
                    }

                    $("#fllupsScreen").animate(align, 200, function () {
                        // Animation complete.
                    });

                    if (VIS.Application.isRTL) {
                        align = {
                            width: sectionsWidthFinal,
                            right: sectionsWidthFinal * 3 + 60,
                        };
                    }
                    else {
                        align = {
                            width: sectionsWidthFinal,
                            left: sectionsWidthFinal * 3 + 60,
                        };
                    }


                    $("#favLinks").animate(align, 200, function () {
                        // Animation complete.
                    });

                }
                //$('#vis_userTime').show();
                $('#vis_userDate').show();
            }


            if (windowWidth > 1024 && windowWidth < 1366) {
                if ($('#workflowActivity').css('display') == 'none') {
                    //alert(divCount);
                    if (VIS.Application.isRTL) {
                        $("#fllupsScreen").css("right", (sectionsWidthFinal + 20));
                        $("#favLinks").css("right", (sectionsWidthFinal * 2 + 40));
                    }
                    else {
                        $("#fllupsScreen").css("left", (sectionsWidthFinal + 20));
                        $("#favLinks").css("left", (sectionsWidthFinal * 2 + 40));
                    }
                    $('#welcomeScreen,#fllupsScreen,#favLinks').width(sectionsWidthFinal);
                }
                else if ($('#workflowActivity').css('display') == 'block') {
                    var newWidth = windowWidth + sectionsWidthFinal + 20;
                    $('.scrollerHorizontal').width(newWidth);
                    //alert(newWidth);
                    horizontalScroll.scroll();
                    $('#welcomeScreen,#fllupsScreen,#favLinks,#workflowActivity').width(sectionsWidthFinal);
                    if (VIS.Application.isRTL) {
                        $("#workflowActivity").css("right", (sectionsWidthFinal + 20));
                        $("#fllupsScreen").css("right", (sectionsWidthFinal * 2 + 40));
                        $("#favLinks").css("right", (sectionsWidthFinal * 3 + 60));
                    }
                    else {
                        $("#workflowActivity").css("left", (sectionsWidthFinal + 20));
                        $("#fllupsScreen").css("left", (sectionsWidthFinal * 2 + 40));
                        $("#favLinks").css("left", (sectionsWidthFinal * 3 + 60));
                    }
                }
                //$('#vis_userTime').show();
                $('#vis_userDate').show();
            }

            /* ipad landscape and small display screen size */
            else if (windowWidth >= 1000 && windowWidth <= 1024) {
                if ($('#workflowActivity').css('display') == 'none') {
                    var screenResolution = windowWidth / 2;
                    var screenResolutionInteger = parseInt(screenResolution);
                    var screenResolutionFinal = screenResolutionInteger * divCount;
                    $('.scrollerHorizontal').width(screenResolutionFinal);
                    if (VIS.Application.isRTL) {
                        $("#fllupsScreen").css("right", (screenResolutionInteger));
                        $("#favLinks").css("right", (screenResolutionInteger * 2));
                    }
                    else {
                        $("#fllupsScreen").css("left", (screenResolutionInteger));
                        $("#favLinks").css("left", (screenResolutionInteger * 2));
                    }
                    $('#welcomeScreen,#fllupsScreen,#favLinks').width(screenResolutionInteger - 20);
                }
                else if ($('#workflowActivity').css('display') == 'block') {
                    divCount = divCount + 1;
                    var screenResolution = windowWidth / 2;
                    var screenResolutionInteger = parseInt(screenResolution);
                    var screenResolutionFinal = screenResolutionInteger * divCount;
                    $('.scrollerHorizontal').width(screenResolutionFinal);
                    horizontalScroll.scroll();
                    $('#welcomeScreen,#fllupsScreen,#favLinks,#workflowActivity').width(screenResolutionInteger - 20);
                    if (VIS.Application.isRTL) {
                        $("#workflowActivity").css("right", (screenResolutionInteger));
                        $("#fllupsScreen").css("right", (screenResolutionInteger * 2));
                        $("#favLinks").css("right", (screenResolutionInteger * 3));
                    }
                    else {
                        $("#workflowActivity").css("left", (screenResolutionInteger));
                        $("#fllupsScreen").css("left", (screenResolutionInteger * 2));
                        $("#favLinks").css("left", (screenResolutionInteger * 3));
                    }
                }
                //$('#vis_userTime').show();
                $('#vis_userDate').show();
            }

            /* ipad portrait, other tablets, fablets and mobile display screen size */
            else if (windowWidth < 1000) {
                if ($('#workflowActivity').css('display') == 'none') {
                    var screenResolution = windowWidth * divCount;
                    var screenResolutionInteger = parseInt(screenResolution);
                    var screenElements = windowWidth;
                    $('.scrollerHorizontal').width(screenResolutionInteger);
                    if (VIS.Application.isRTL) {
                        $("#fllupsScreen").css("right", (screenElements));
                        $("#favLinks").css("right", (screenElements * 2));
                    }
                    else {
                        $("#fllupsScreen").css("left", (screenElements));
                        $("#favLinks").css("left", (screenElements * 2));
                    }
                    $('#welcomeScreen,#fllupsScreen,#favLinks').width(screenElements - 20);
                }
                else if ($('#workflowActivity').css('display') == 'block') {
                    divCount = divCount + 1;
                    var screenResolution = windowWidth * divCount;
                    var screenResolutionInteger = parseInt(screenResolution);
                    var screenElements = windowWidth;
                    $('.scrollerHorizontal').width(screenResolutionInteger);
                    horizontalScroll.scroll();
                    $('#welcomeScreen,#fllupsScreen,#favLinks,#workflowActivity').width(screenElements - 20);
                    if (VIS.Application.isRTL) {
                        $("#workflowActivity").css("right", (screenElements));
                        $("#fllupsScreen").css("right", (screenElements * 2));
                        $("#favLinks").css("right", (screenElements * 3));
                    }
                    else {
                        $("#workflowActivity").css("left", (screenElements));
                        $("#fllupsScreen").css("left", (screenElements * 2));
                        $("#favLinks").css("left", (screenElements * 3));
                    }
                }
                //$('#vis_userTime').hide();
                $('#vis_userDate').hide();
            }
        }

        //Get User Greetings(Good Morning,Good Afternoon etc)............Vivek
        function getUserGreetings() {
            var greeting = "";
            var date = new Date();
            var hours = date.getHours();

            if (hours < 12) {
                return VIS.Msg.getMsg("GoodMorning");
            }
            else if (hours < 17) {
                return VIS.Msg.getMsg("GoodAfternoon");
            }
            else {
                return VIS.Msg.getMsg("GoodEvening");

            }
        }

        //Load Home Page like partial view Home
        function loadHome() {
            $home.load(VIS.Application.contextUrl + 'Home/Home', function () {
                isTabAjaxBusy = false;
                isWsp = false;
                isTabscroll = false;
                isTabDataRef = false;
                //Welcome Screan hder
                $sAlrtTxtType = $("#sAlrtTxtType")
                $hAlrtTxtTypeCount = $("#hAlrtTxtTypeCount");
                $ulHomeTabMenu = $("#ulHomeTabMenu");
                $divTabMenuDataLoder = $("#divTabMenuDataLoder");
                $spanWelcomeTabtopHdr = $("#spanWelcomeTabtopHdr");
                $searchData = $('#searchData');
                /*Start initialize div  object*/
                horizontalScroll = $("#dataContainer");
                FllUpsMain = $("#fllupsList");
                workflowActivityData = $("#workflowActivityData");
                userLinksScroll = $("#userLinks-List");
                userFavsScroll = $("#userFavs-List");
                $hlnkRefTabData = $("#hlnkTabDataRef");
                WelcomeTabDatacontainers = $("#welcomeScreenFeedsList");
                favLinks = $("#favLinks");
                activeTabType = 1;
                $workflowActivity = $('#workflowActivity');
                $divTabContainerLoder = $("#divTabMenuDataLoder");
                $workflowActivityDetails = $("#workflowActivityDetails");
                $WelcomeScreenHdr = $("#hWelcomeScreenSwapHdr");
                $welcomeNewRecord = $("#sNewNts");
                $wfSearchShow = $('#WFSearchshow');
                /*end  initialize div object */





                //Count
                $divNoticeCount = $("#divNoticeCount");
                $divRequestCount = $("#divRequestCount");
                $divNotesCount = $("#divNotesCount");
                $divAptCount = $("#divAptCount");
                $divKPICount = $('#divKPI');
                /*Start Pagging initialize */
                tabdataPageSize = 10, tabdataPage = 0, tabdataLastPage = 0, tabdatacntpage = 0;
                /*End Pagging initialize */

                /* App Layout Adjustment Script START */
                adjustDivSize();
                $(window).resize(adjustDivSize);
                /* App Layout Adjustment Script End */

                /* Workflow Script start */
                $('div.vis-workflowActivityTitleBar-buttons ul li .vis-icon-check').on(VIS.Events.onTouchStartOrClick, function (evnt) {
                    $('#workflowActivity').hide();
                    $('.vis-activityContainer').removeClass('vis-activeFeed');
                    adjustDivSize();
                    horizontalScroll.scroll();
                });

                $('#btnWFAClose').on(VIS.Events.onTouchStartOrClick, function (evnt) {
                    $('#workflowActivity').hide();
                    $('.vis-activityContainer').removeClass('vis-activeFeed');
                    adjustDivSize();
                    horizontalScroll.scroll();
                });
                /* Workflow Script end */

                /* start change Profile picture  */
                // Change User Image   
                VIS.changeUserImage();
                /* End change Profile picture  */

                /* Toggle App Fav and links Script START */

                $('#userFav-Tab').click(function () {
                    $('#userLinks div').removeClass('row userLinks_activeTab');
                    $('#userLinks div').addClass('row userLinks_inactiveTab');

                    $('#userFavs div').removeClass('row userFavs_inactiveTab');
                    $('#userFavs div').addClass('row userFavs_activeTab');

                    $('#userLinks-List').hide('fast');
                    $('#userFavs-List').show('fast', function () {
                        setTimeout(function () {
                            //  userFavsScroll.scroll();
                            //favLinks.scroll();
                        }, 0);
                    });
                });
                $('#userLinks-Tab').click(function () {
                    $('#userFavs div').removeClass('row userFavs_activeTab');
                    $('#userFavs div').addClass('row userFavs_inactiveTab');

                    $('#userLinks div').removeClass('row userLinks_inactiveTab');
                    $('#userLinks div').addClass('row userLinks_activeTab');

                    $('#userFavs-List').hide('fast');
                    $('#userLinks-List').show('fast', function () {
                        setTimeout(function () {
                            // userLinksScroll.scroll();
                            //favLinks.scroll();
                        }, 0);
                    });
                });

                VIS.shortcutMgr.init($('#userLinks-List'), $('#userFavs-List'));


                /* Toggle App Fav and links Script END */

                /* Toggle Welcome Screen Tabs */
                $('.vis-welcomeScreenFeeds').hide();
                $('.vis-icon-userfeeds').off("click");
                $('.vis-icon-userfeeds').on("click", function () {
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').show('fast');
                    //$('.vis-welcomeScreenFeeds').show('fast', function () {
                    //    setTimeout(function () {
                    //        WelcomeTabDatacontainers.scroll();
                    //    }, 0);
                    //});
                });



                $('.vis-welcomeScreenTab-notificationBubbles').on("click", function (e) {

                    return false;

                });

                //Show Greetings on Home Screen...........Vivek
                var greeting = getUserGreetings();
                $('#vis-userGreeting').text(greeting);
                /* start Referesh New tab Data*/
                $hlnkRefTabData.on("click", function () {

                    isTabDataRef = true;
                    if (isTabAjaxBusy == false) {
                        if (activeTabType == ProfileType) {
                        }
                        else if (activeTabType == RequestType) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            emptyWelcomeTabDatacontainers();;
                            LoadHomeRequest();
                        }
                        else if (activeTabType == WorkflowType) {

                            //   $workflowActivity.hide();
                            adjustDivSize();
                            $welcomeNewRecord.hide();
                            $wfSearchShow.show();
                            $sAlrtTxtType.empty();
                            $sAlrtTxtType.append(VIS.Msg.getMsg('WorkflowActivities'));
                            $hAlrtTxtTypeCount.empty();
                            $hAlrtTxtTypeCount.append($("#divfActivity").html());
                            $WelcomeScreenHdr.empty();
                            $WelcomeScreenHdr.append(VIS.Msg.getMsg('Activity'));
                            //var divActivityContainer = $("#welcomeScreenFeedsList");
                            //var divActivityDetailContainer = $("#workflowActivityDetails");
                            // emptyWelcomeTabDatacontainers();;
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;

                            if (activity == null) {
                                activity = new VIS.wfActivity(WelcomeTabDatacontainers, $workflowActivityDetails, workflowActivityData, WelcomeTabDatacontainers, $wfSearchShow);
                            }
                            activity.Load(true);

                        }
                        else if (activeTabType == NoticeType) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            emptyWelcomeTabDatacontainers();;
                            LoadHomeNotice();
                        }
                        else if (activeTabType == MyTaskType) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            WSP.wspHomeMgr.viewWSPHome(MyTaskType, isTabDataRef);
                        }
                        else if (activeTabType == TaskAssignByme) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            WSP.wspHomeMgr.viewWSPHome(TaskAssignByme, isTabDataRef);
                        }
                        else if (activeTabType == DocumentReceaveType) {
                            return;
                        }
                        else if (activeTabType == AppointmentsType) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            emptyWelcomeTabDatacontainers();;
                            WSP.wspHomeMgr.viewWSPHome(AppointmentsType, isTabDataRef);
                        }
                        else if (activeTabType == NotesType) {
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            emptyWelcomeTabDatacontainers();;
                            WSP.wspHomeMgr.viewWSPHome(NotesType, isTabDataRef);
                        }
                        else if (activeTabType == kPIType) {
                            $workflowActivity.hide();
                            adjustDivSize();
                            tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                            emptyWelcomeTabDatacontainers();;
                            $wfSearchShow.hide();
                            VADB.Apps.KpiHomeScreen(WelcomeTabDatacontainers, $divTabContainerLoder, $welcomeNewRecord, $workflowActivity, $workflowActivityDetails, $WelcomeScreenHdr, $divKPICount);
                        }
                    }
                });
                /* Start Referesh New tab data */


                /* start  bind click event of Main data container of Tab Menu */
                //for notice and request
                WelcomeTabDatacontainers.on("click", function (evnt) {
                    var datarcrd = $(evnt.target).data("vishomercrd");
                    if (activeTabType == NoticeType) {

                        if (evnt.target.tagName === "SPAN" && datarcrd === "more") {
                            //more-details
                            if ($(evnt.target.parentNode.parentNode).data("vishomercrd") == "more-details") {
                                var divid = evnt.target.parentNode.parentNode.id;

                                var $divntitleid = $("#snoticetitle_" + divid);
                                var $divndescid = $("#snoticedesc_" + divid);
                                var $divnmorecid = $("#snoticemore_" + divid);
                                $divnmorecid.hide();
                                $divntitleid.hide();
                                $divndescid.show();
                                $("#snoticeless_" + divid).show();
                            }
                        }
                        else {

                            if ($(evnt.target.parentNode.parentNode).data("vishomercrd") == "more-details") {
                                var divid = evnt.target.parentNode.parentNode.id;
                                var $divntitleid = $("#snoticetitle_" + divid);
                                var $divndescid = $("#snoticedesc_" + divid);
                                var $divnmorecid = $("#snoticemore_" + divid);
                                $divnmorecid.show();
                                $divntitleid.show();
                                $divndescid.hide();
                                $("#snoticeless_" + divid).hide();
                            }
                        }
                        //for notice view/zoom
                        if (datarcrd === "view") {

                            var vid = evnt.target.id;
                            var arrn = vid.toString().split('|');

                            var n_id = arrn[0];
                            var n_table = arrn[1];
                            var n_win = arrn[2];
                            var n_rcrd = arrn[3];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(n_table + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(n_id));
                            VIS.viewManager.startWindow(n_win, zoomQuery);

                        }
                        //for notice view/zoom
                        else if (datarcrd === "liview") {
                            var vid = evnt.target.firstChild.id;
                            var arrn = vid.toString().split('|');


                            var n_id = arrn[0];
                            var n_table = arrn[1];
                            var n_win = arrn[2];
                            var n_rcrd = arrn[3];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(n_table + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(n_id));
                            VIS.viewManager.startWindow(n_win, zoomQuery);

                        }
                        //for notice approve
                        else if (datarcrd === "approve") {
                            var vid = evnt.target.id;
                            ApproveNotice(vid, true);
                        }
                        //for notice approve
                        else if (datarcrd === "liapprove") {
                            var vid = evnt.target.firstChild.id;
                            ApproveNotice(vid, true);
                        }
                        else if (datarcrd === "lispecial") {
                            var vid = evnt.target.firstChild.id;
                            var arrn = vid.toString().split('|');


                            var recID = arrn[0];
                            var tableName = arrn[1];
                            var winID = arrn[2];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(tableName + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(recID));
                            VIS.viewManager.startWindow(winID, zoomQuery);
                        }//
                        else if (datarcrd === "lispecial1") {
                            var vid = evnt.target.id;
                            var arrn = vid.toString().split('|');


                            var recID = arrn[0];
                            var tableName = arrn[1];
                            var winID = arrn[2];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(tableName + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(recID));
                            VIS.viewManager.startWindow(winID, zoomQuery);
                        }

                    }
                    else if (activeTabType == RequestType) {

                        //for request view/zoom
                        if (datarcrd === "view") {

                            var vid = evnt.target.id;
                            var arrn = vid.toString().split('|');

                            var r_id = arrn[0];
                            var r_table = arrn[1];
                            var r_win = arrn[2];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(r_table + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(r_id));
                            VIS.viewManager.startWindow(r_win, zoomQuery);


                        }
                        //for request view/zoom
                        else if (datarcrd === "liview") {
                            var vid = evnt.target.firstChild.id;
                            var arrn = vid.toString().split('|');

                            var r_id = arrn[0];
                            var r_table = arrn[1];
                            var r_win = arrn[2];

                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction(r_table + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(r_id));
                            VIS.viewManager.startWindow(r_win, zoomQuery);

                        }
                    }
                });/* End  bind click event of Main data container of Tab Menu */
                
                var welcomeTabScroll = true;
                
                /* Start Bind Scroll of main data contianer of tab menu */
                WelcomeTabDatacontainers.bind('scroll', function () {
                    //var thisscroll = this;
                    //clearTimeout($.data(this, 'scrollTimer'));//Clear scroll timer to wait next scroll event happens after 250 ms
                    //$.data(this, 'scrollTimer', setTimeout(function () {
                    if ($(this).scrollTop() + $(this).innerHeight() >= (this.scrollHeight * .75) && welcomeTabScroll) {//Condition true when 75 scroll is done
                        welcomeTabScroll = false;
                        setTimeout(function () {
                            welcomeTabScroll = true;
                        }, 100);
                        isTabscroll = true;
                            isTabDataRef = false;
                            if (activeTabType == WorkflowType) {
                                tabdataLastPage = parseInt($("#divfActivity").html());
                                tabdatacntpage = tabdataPage * tabdataPageSize;
                                if (tabdatacntpage <= tabdataLastPage && activity != null) {
                                    tabdataPage += 1;
                                    activity.AppendRecord(tabdataPage, tabdataPageSize);
                                }
                                else {
                                    return;
                                }
                            }
                            else if (activeTabType == NoticeType) {
                                tabdataLastPage = parseInt($divNoticeCount.text());
                                tabdatacntpage = tabdataPage * tabdataPageSize;
                                tabdataPage += 1;
                                if (tabdatacntpage <= tabdataLastPage) {
                                    LoadHomeNotice();
                                }
                                else {
                                    return;
                                }
                            }
                            else if (activeTabType == RequestType) {
                                tabdataLastPage = parseInt($divRequestCount.text());
                                tabdatacntpage = tabdataPage * tabdataPageSize;
                                tabdataPage += 1;
                                if (tabdatacntpage <= tabdataLastPage) {
                                    LoadHomeRequest();
                                }
                                else {
                                    return;
                                }
                            }
                            else if (activeTabType == AppointmentsType && window.WSP) {
                                tabdataLastPage = parseInt($divAptCount.text());
                                tabdatacntpage = tabdataPage * tabdataPageSize;
                                tabdataPage += 1;
                                if (tabdatacntpage <= tabdataLastPage) {
                                    WSP.wspHomeMgr.doScrollWSPHome(AppointmentsType, tabdataPage, tabdataPageSize);
                                }
                                else {
                                    return;
                                }
                            }
                            else if (activeTabType == NotesType && window.WSP) {
                                tabdataLastPage = parseInt($divNotesCount.text());
                                tabdatacntpage = tabdataPage * tabdataPageSize;
                                if (tabdatacntpage <= tabdataLastPage) {
                                    tabdataPage += 1;
                                    WSP.wspHomeMgr.doScrollWSPHome(NotesType, tabdataPage, tabdataPageSize);
                                }
                                else {
                                    return;
                                }
                            }

                        }
                    //}, 200));
                });
                /* End Bind Scroll of main data contianer of tab menu    */


                /*########### Start FolloUps ###########*/
                var $FllUpsRefresh = $("#hlnkFllupsRef"), $FllupsCnt = $("#follupsCount"), $FllMainLoder = $("#divFllMainLoder");
                var FllCmntTxt = "", FllCmntBtn = "", FllupsID = "";
                var fllpcount = 0, fllPageSize = 10, fllPage = 1, fllLastPage = 0, fllcntpage = 0;
                var fllCmntPageSize = 10, fllCmntPage = 0, fllCmntLastPage = 0, fllCmntcntpage = 0;
                var fllChatID = 0, fllSubscriberID = 0, isFllScroll = false, isRef = false, isfllBusy = false;
                //if (VIS.Application.isRTL) {
                //    $FllUpsRefresh.css("margin-right", "10px");
                //}
                //resize change event
                //$("#fllupsScreen").on("orientationchange", function (evnt) {
                //    alert("call fllups");
                //    $("#fllupsScreen").height(50);
                //});

                //Click events for follups
                FllUpsMain.on("click", function (evnt) {
                    var datafll = $(evnt.target).data("fll");
                    if (datafll === "viewmorefllupscmnt") {

                        FllupsID = evnt.target.parentNode.id;
                        fllChatID = 0, fllSubscriberID = 0;
                        fllChatID = evnt.target.parentNode.children[3].id;
                        getFllUpsCmnt(FllupsID, fllCmntPageSize, fllCmntPage);

                        //View Less comment
                        var targetID = evnt.target.id;
                        var arrtarget = targetID.split('_');
                        $("#divfllvl_" + arrtarget[1]).show();
                        $("#" + targetID).hide();
                    }
                    else if (datafll === "viewlessfllupscmnt") {
                        FllupsID = evnt.target.parentNode.id;
                        fllChatID = 0, fllSubscriberID = 0;
                        fllChatID = evnt.target.parentNode.children[3].id;

                        //View Less comment
                        var targetID = evnt.target.id;
                        var arrtarget = targetID.split('_');
                        $("#divfllvm_" + arrtarget[1]).show();
                        // $("#divfllcmntdata" + arrtarget[1]).empty();
                        var $divFllupsCmndData = $("#divfllcmntdata" + arrtarget[1]);
                        var divLastChild = $divFllupsCmndData.find("div:last-child");
                        $divFllupsCmndData.empty();
                        $divFllupsCmndData.html(divLastChild);
                        $("#" + targetID).hide();
                    }
                    else if (datafll === "azoomfllups") {
                        FllupsID = evnt.target.parentNode.parentNode.parentNode.parentNode.parentNode.id;
                        var arr = FllupsID.toString().split('-');
                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction(arr[5] + "_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(arr[1]));
                        VIS.viewManager.startWindow(arr[4], zoomQuery);
                    }

                    else if (datafll === "asubscribefllups") {
                        FllupsID = evnt.target.parentNode.parentNode.parentNode.parentNode.parentNode.id;
                        var arr = FllupsID.toString().split('-');
                        $.ajax({
                            url: VIS.Application.contextUrl + 'Subscribe/UnSubscribe',
                            type: 'GET',
                            dataType: 'Json',
                            data: { AD_Window_ID: arr[4], Record_ID: arr[1], AD_Table_ID: arr[3] },
                            success: function (result) {
                                if (result == false) {
                                    alert(VIS.Msg.getMsg("UnSubscription Failed"));
                                }
                                else {
                                    $("#" + FllupsID).animate({ "width": "100%", "height": "132px", "margin-top": "-132px" }, 700, function () {
                                        $("#" + FllupsID).remove();
                                    });
                                    var fcnt = $FllupsCnt.text();
                                    $FllupsCnt.text(fcnt - 1);
                                }
                            }
                        });
                    }
                    else if (datafll === "btncmntfll") {
                        fllChatID = evnt.target.parentNode.id;
                        FllupsData = $("#" + evnt.target.parentNode.parentNode.children[1].id);
                        FllCmntTxt = $("#" + evnt.target.previousElementSibling.id);
                        FllCmntBtn = $("#" + evnt.target.id);
                        var cmntTxt = $(FllCmntTxt).val();
                        if (cmntTxt !== "") {
                            var cd = new Date();
                            var Cdate = Globalize.format(cd, "F", Globalize.cultureSelector);
                            SaveFllCmnt(cmntTxt, FllupsData);
                            $(FllCmntTxt).val("");
                        }
                    }
                    else if (datafll == "UID") {
                        var UID = $(evnt.target).data("uid");
                        var windoNo = VIS.Env.getWindowNo();
                        var contactInfo = new VIS.ContactInfo(UID, windoNo);
                        contactInfo.show();
                    }
                });
                //key down events for follups
                FllUpsMain.on("keydown", function (evnt) {
                    var datafll = $(evnt.target).data("fll");
                    if (datafll === "txtcmntfll") {
                        FllupsID = evnt.target.parentNode.parentNode.id;
                        fllChatID = evnt.target.parentNode.id;
                        FllupsData = $("#" + evnt.target.parentNode.parentNode.children[1].id);
                        FllCmntTxt = $("#" + evnt.target.id);
                        var cmntTxt = $(FllCmntTxt).val();
                        if (cmntTxt !== "" && cmntTxt !== null) {
                            var code = evnt.charCode || evnt.keyCode;
                            if (code === EnterKeyCode) {
                                //  BindFllCmnt(uname, uimage, Cdate, cmntTxt, FllupsData);
                                SaveFllCmnt(cmntTxt, FllupsData)
                                $(FllCmntTxt).val("");
                            }
                        }
                    }
                });
                var followUpScroll = true;
                //Bind Scroll evnt on Follups
                FllUpsMain.bind('scroll', function () {
                    //var thisscroll = this;
                    //clearTimeout($.data(this, 'scrollTimer'));//Clear scroll timer to wait next scroll event happens after 250 ms
                    //$.data(this, 'scrollTimer', setTimeout(function () {
                    if ($(this).scrollTop() + $(this).innerHeight() >= (this.scrollHeight * .75) && followUpScroll) {//Condition true when 75 scroll is done
                        isRef = false;
                        followUpScroll = false;
                        fllLastPage = $FllupsCnt.text();

                        if (fllLastPage > fllPageSize) {
                            fllcntpage = fllPage * fllPageSize;
                            fllPage += 1;
                            if (fllcntpage <= fllLastPage) {
                                getFllUps(fllPageSize, fllPage, isRef);
                            }
                            else {
                                followUpScroll = true;
                                return;
                            }
                        }
                        followUpScroll = true;
                    }
                    //}, 200));
                });
                //To refresh fllups
                $FllUpsRefresh.on("click", function () {
                    if (isfllBusy == false) {
                        fllPageSize = 10, fllPage = 1;
                        fllpcount = $FllupsCnt.text();
                        isRef = true;
                        FllUpsMain.empty();
                        getFllUps(fllPageSize, fllPage, isRef);
                    }

                });
                //To get Follups form Controller
                function getFllUps(fllPageSize, fllPage, isRef) {
                    $FllMainLoder.show();
                    isfllBusy = true;
                    var url = VIS.Application.contextUrl + 'Home/GetJSONFllups';
                    $.ajax({
                        url: url,
                        data: { "fllPageSize": fllPageSize, "fllPage": fllPage, "isRef": isRef },
                        type: 'GET',
                        cache: false,
                        datatype: 'json',
                        success: function (result) {
                            var data = JSON.parse(result);
                            var cnt = 0;
                            var str = "";
                            var uimg = "";
                            var dbdate = "";
                            if (data.lstFollowups.length > 0) {
                                if (isRef) {
                                    $FllupsCnt.text(data.FllCnt)
                                }
                                for (var s in data.lstFollowups) {
                                    var divfllupsID = data.lstFollowups[cnt].ChatID + "-" + data.lstFollowups[cnt].RecordID + "-" + data.lstFollowups[cnt].SubscriberID + "-" + data.lstFollowups[cnt].TableID + "-" + data.lstFollowups[cnt].WinID + "-" + data.lstFollowups[cnt].TableName;
                                    if (data.lstFollowups[cnt].Cdate != null && data.lstFollowups[cnt].Cdate != "") {
                                        var cd = new Date(data.lstFollowups[cnt].Cdate);
                                        dbdate = Globalize.format(cd, "F", Globalize.cultureSelector);
                                    }
                                    if (data.lstFollowups[cnt].AD_Image_ID == 0) {
                                        uimg = "<i data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "' class='fa fa-user'></i>";
                                    }
                                    else {
                                        for (var a in data.lstUserImg) {
                                            if (data.lstUserImg[a].AD_Image_ID == data.lstFollowups[cnt].AD_Image_ID) {
                                                if (data.lstUserImg[a].UserImg != "NoRecordFound" && data.lstUserImg[a].UserImg != "FileDoesn'tExist" && data.lstUserImg[a].UserImg != null) {
                                                    uimg = "<div class='vis-feedimgwrap'  data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'><img data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "' alt='" + VIS.Msg.getMsg("UserImage") + "' title='" + VIS.Msg.getMsg("UserImage") + "' class='userAvatar-Feeds' src='" + VIS.Application.contextUrl + data.lstUserImg[a].UserImg + "?" + new Date($.now()).getSeconds() + "'/></div>";
                                                }
                                                else {
                                                    uimg = "<i data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "' class='fa fa-user'></i>";
                                                }
                                                break;
                                            }
                                        }
                                    }

                                    str += "<div id=" + divfllupsID + " data-fll='divfllups' class='vis-feedContainer'>"
                                        + "<div class='vis-feedTitleBar'>"
                                        + "<h3> " + data.lstFollowups[cnt].WinName + ' : ' + data.lstFollowups[cnt].Identifier + " </h3>"
                                        + "<div class='vis-feedTitleBar-buttons'>"
                                        + "<ul><li> <a href='javascript:void(0)'  data-fll='azoomfllups'  title='" + VIS.Msg.getMsg("ViewFollowups") + "'  class='vis vis-find'></a></li>"
                                        + "<li> <a href='javascript:void(0)'  data-fll='asubscribefllups'  title='" + VIS.Msg.getMsg("UnsubscribeFollowups") + "' class='fa fa-rss'></a></li></ul>"
                                        + " </div></div>"

                                        + "<div id='divfllcmntdata" + data.lstFollowups[cnt].ChatID + "' data-fll='fll-cmnt' class='vis-feedDetails'>"
                                        + "<div class='vis-feedDetails-cmnt' data-fll='fll-cmnt'>"
                                        + uimg
                                        + "<p>"
                                        + " <strong data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'>";
                                    if (data.lstFollowups[cnt].AD_User_ID == VIS.Env.getCtx().getAD_User_ID()) {
                                        str += VIS.Msg.getMsg("Me") + " </strong> <br />"
                                    }
                                    else {
                                        str += data.lstFollowups[cnt].Name + "</strong> <br />"
                                    }

                                    str += VIS.Utility.encodeText(data.lstFollowups[cnt].ChatData)
                                        + "</p>"
                                        + "<p class='vis-feedDateTime'>" + dbdate + "</p>"
                                        + "</div></div>"

                                        + "<a id='divfllvm_" + data.lstFollowups[cnt].ChatID + "'  data-fll='viewmorefllupscmnt' href='javascript:void(0)' class='vis-viewMoreComments'><span class='vis-feedIcons vis-icon-viewMoreComments'></span>" + VIS.Msg.getMsg("ViewMoreComments") + "...</a>"
                                        + "<a id='divfllvl_" + data.lstFollowups[cnt].ChatID + "' style='display:none;'  data-fll='viewlessfllupscmnt' href='javascript:void(0)' class='vis-viewMoreComments'><span class='vis-feedIcons vis-icon-viewMoreComments'></span>" + VIS.Msg.getMsg("ViewLessComments") + "...</a>"
                                        + " <div class='clearfix'></div> "

                                        + "<div id=" + data.lstFollowups[cnt].ChatID + " class='vis-feedMessage'>"
                                        + " <input id='txtFllCmnt" + data.lstFollowups[cnt].ChatID + "' data-fll='txtcmntfll' placeholder='" + VIS.Msg.getMsg('TypeMessage') + "' type='text' value='' />"
                                        + " <span  id='btnFllCmnt" + data.lstFollowups[cnt].ChatID + "' data-fll='btncmntfll' title='" + VIS.Msg.getMsg('PostMessage') + "'  class='vis vis-sms' ></span>"
                                        + " <div class='clearfix'></div> "
                                        + "</div></div> ";
                                    cnt++;
                                }
                                FllUpsMain.append(str);
                            }
                            else {
                                if (isFllScroll == false) {
                                    if (VIS.Application.isRTL) {
                                        FllUpsMain.append("<p style='margin-top: 200px;text-align: center '>" + VIS.Msg.getMsg("NoRecordFound") + "!!!</p>");
                                    }
                                    else {
                                        FllUpsMain.append("<p style='margin-top: 200px;text-align: center '>" + VIS.Msg.getMsg("NoRecordFound") + "!!!</p>");
                                    }
                                }
                            }
                            isfllBusy = false;
                            followUpScroll = true;
                            $FllMainLoder.hide();
                        },
                        error: function () {
                            followUpScroll = true;
                        }
                    });
                }
                //get fllups comment from Controller
                function getFllUpsCmnt(FllupsID, fllCmntPageSize, fllCmntPage) {
                    var url = VIS.Application.contextUrl + 'Home/GetJSONFllupsCmnt/';
                    $.ajax({
                        url: url,
                        data: { FllupsID: FllupsID, fllCmntPageSize: fllCmntPageSize, fllCmntPage: fllCmntPage },
                        type: 'GET',
                        cache: false,
                        datatype: 'json',
                        success: function (result) {
                            var data = JSON.parse(result);
                            var cnt = 0;
                            var str = "";
                            var dbdate = "";
                            var ChatID = data.lstFollowups[cnt].ChatID;
                            var uimg = "";
                            for (var r in data.lstFollowups) {

                                if (data.lstFollowups[r].Cdate != null && data.lstFollowups[r].Cdate != "") {
                                    var cd = new Date(data.lstFollowups[r].Cdate);
                                    dbdate = Globalize.format(cd, "F", Globalize.cultureSelector);
                                }
                                if (data.lstFollowups[cnt].AD_Image_ID == 0) {
                                    uimg = "<i data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "' class='fa fa-user'></i>"
                                }
                                else {
                                    for (var b in data.lstUserImg) {
                                        if (data.lstUserImg[b].AD_Image_ID == data.lstFollowups[cnt].AD_Image_ID) {
                                            if (data.lstUserImg[b].UserImg != "NoRecordFound" && data.lstUserImg[b].UserImg != "FileDoesn'tExist" && data.lstUserImg[b].UserImg != null) {
                                                uimg = "<div class='vis-feedimgwrap' data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'><img  alt='" + VIS.Msg.getMsg("UserImage") + "'  title='" + VIS.Msg.getMsg("UserImage") + "'  class='userAvatar-Feeds' src='" + data.lstUserImg[b].UserImg + "?" + new Date($.now()).getSeconds() + "'/></div>"
                                            }
                                            else {
                                                //uimg = "<img  style='cursor:pointer;'  data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'  alt='" + VIS.Msg.getMsg("UserImage") + "'  title='" + VIS.Msg.getMsg("UserImage") + "'  class='userAvatar-Feeds' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/defaultUser46X46.png'/>"

                                                uimg = "<i  data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'    class='fa fa-user'></i>"

                                            }
                                            break;
                                        }
                                    }
                                }
                                str += "<div class='vis-feedDetails-cmnt'  data-fll='fll-cmnt'>"
                                    + uimg
                                    + "<p>"
                                    + " <strong  data-fll='UID' data-UID='" + data.lstFollowups[cnt].AD_User_ID + "'>";

                                if (data.lstFollowups[cnt].AD_User_ID == VIS.Env.getCtx().getAD_User_ID()) {
                                    str += VIS.Msg.getMsg("Me") + " </strong> <br />"
                                }
                                else {
                                    str += data.lstFollowups[cnt].Name + "</strong> <br />"
                                }

                                str += VIS.Utility.encodeText(data.lstFollowups[cnt].ChatData)
                                    + "</p>"
                                    + "<p class='vis-feedDateTime'>" + dbdate + "</p></div>";
                                cnt++;
                            }

                            var FllupsData = $("#divfllcmntdata" + ChatID)
                            // $("#divfllvm" + ChatID).hide();
                            FllupsData.html("");
                            FllupsData.append(str);
                            //FllUpsMain.animate({ scrollTop: 1000 }, '50');                
                        },
                        error: function () {
                            console.log("Error");
                        }
                    });
                }

                //Save follups Comment in DB
                function SaveFllCmnt(cmntTxt, FllupsData) {
                    var url = VIS.Application.contextUrl + 'Home/PostFllupsCmnt/';
                    $.ajax({
                        url: url,
                        data: { fllChatID: fllChatID, fllSubscriberID: fllSubscriberID, cmntTxt: cmntTxt },
                        type: 'POST',
                        cache: false,
                        datatype: 'json',
                        success: function (data) {
                            var cd = new Date();
                            var cdate = Globalize.format(cd, "F", Globalize.cultureSelector);
                            var user_image = data;
                            var name = VIS.context.getContext("##AD_User_Name");
                            var uimg = "";
                            if (user_image !== null) {

                                if (user_image != "NoRecordFound" && user_image != "FileDoesn'tExist") {
                                    uimg = "<div class='vis-feedimgwrap'  data-fll='UID' data-UID='" + VIS.Env.getCtx().getAD_User_ID() + "' ><img data-fll='UID' data-UID='" + VIS.Env.getCtx().getAD_User_ID() + "'  alt='" + VIS.Msg.getMsg("UserImage") + "'  class='userAvatar-Feeds' src='" + user_image + "?" + new Date($.now()).getSeconds() + "' /></div>"
                                }
                                else {
                                    uimg = "<i data-fll='UID' data-UID='" + VIS.Env.getCtx().getAD_User_ID() + "' class='fa fa-user'></i>"
                                }
                            }
                            else {
                                uimg = "<i data-fll='UID' data-UID='" + VIS.Env.getCtx().getAD_User_ID() + "' class='fa fa-user'></i>"
                            }
                            var str = "<div class='vis-feedDetails-cmnt' data-fll='fll-cmnt'>"
                                + uimg
                                + "<p>"
                                + " <strong  data-fll='UID' data-UID='" + VIS.Env.getCtx().getAD_User_ID() + "'>" + VIS.Msg.getMsg("Me") + "</strong><br />"
                                + VIS.Utility.encodeText(cmntTxt)
                                + "</p>"
                                + "<p class='vis-feedDateTime'>" + cdate + "</p></div>";
                            FllupsData.append(str);

                        },
                        error: function () {
                            console.log("Error");
                        }
                    });
                }

                /*########### End Folloups ########### */


                /*start Switch between different sections of the Welcome Screen using bottom Tabs menu  */
                $ulHomeTabMenu.off("click");
                BindMenuClick();
                /*End Switch between different sections of the Welcome Screen using bottom Tabs menu  */


                /*Start Wsp Module */
                if (window.WSP) {
                    isWsp = true;
                    $wfSearchShow.hide();
                    WSP.wspHomeMgr.initWSPHome(WelcomeTabDatacontainers, $ulHomeTabMenu, $workflowActivityDetails, workflowActivityData, $workflowActivity, $divTabContainerLoder, $WelcomeScreenHdr, $welcomeNewRecord, $sAlrtTxtType, $hAlrtTxtTypeCount, $divAptCount, $divNotesCount);
                }
                else {
                    console.log("PleaseInstallWSPModule");

                }
                /*End Wsp Module */
            });
        }

        function getActiveTab() {
            return { activeTabType: activeTabType }
        }


        /* Start Request */
        function LoadHomeRequest() {
            $divTabMenuDataLoder.show();
            isTabAjaxBusy = true;
            //$ulHomeTabMenu.off("click");
            $.ajax({
                url: VIS.Application.contextUrl + 'Home/GetJSONHomeRequest',
                data: { "pagesize": tabdataPageSize, "page": tabdataPage, "isTabDataRef": isTabDataRef },
                type: 'GET',
                datatype: 'json',
                cache: false,
                success: function (result) {
                    var data = JSON.parse(result.data);
                    var str = "";
                    if (data.length > 0) {
                        if (activeTabType == RequestType) {
                            if (isTabDataRef) {
                                tabdataLastPage = parseInt(result.count);
                                $divRequestCount.empty();
                                $divRequestCount.show();
                                $hAlrtTxtTypeCount.empty();
                                $divRequestCount.text(result.count);
                                $hAlrtTxtTypeCount.text(result.count);
                            }
                        }
                        for (var s in data) {
                            var StartDate = "";
                            if (data[s].StartDate != null || data[s].StartDate != "") {
                                var cd = new Date(data[s].StartDate);
                                StartDate = Globalize.format(cd, "d", Globalize.cultureSelector);
                            }
                            var NextActionDate = "";
                            if (data[s].NextActionDate != null) {
                                var cd = new Date(data[s].NextActionDate);
                                NextActionDate = Globalize.format(cd, "d", Globalize.cultureSelector);
                            }
                            else {
                                NextActionDate = "&nbsp;-----------";
                            }
                            var CreatedDate = "";
                            if (data[s].CreatedDate != null || data[s].CreatedDate != "") {
                                var cd = new Date(data[s].CreatedDate);
                                CreatedDate = Globalize.format(cd, "F", Globalize.cultureSelector);
                            }

                            var summary = data[s].Summary;
                            if (summary.length > 80) {
                                summary = summary.substr(0, 80) + "..."
                            }
                            var casetype = data[s].CaseType;
                            if (casetype.length > 30) {
                                casetype = casetype.substr(0, 30) + "..."
                            }

                            str += "<div class='vis-activityContainer'>"
                                + "<div class='vis-feedTitleBar'>"
                                + "<h3>#" + data[s].DocumentNo + "</h3>"
                                + "<div class='vis-feedTitleBar-buttons'>"
                                + "<ul>";

                            if (data[s].Name && data[s].Name.length > 0) {
                                str += "<li class='vis-home-request-BP'>" + data[s].Name + "</li>"
                            }

                            + "<li class='vis-home-request-BP'>" + data[s].Name + "</li>"
                            str += "<li data-vishomercrd='liview'><a href='javascript:void(0)' data-vishomercrd='view' id=" + data[s].R_Request_ID + "|" + data[s].TableName + "|" + data[s].AD_Window_ID + "  title='" + VIS.Msg.getMsg("View") + "'  class='vis vis-find'></a></li>"
                                + "</ul>"
                                + "</div>"
                                + "</div>"

                                + "<div  class='vis-feedDetails vis-pt-0 vis-pl-0'>"
                                + "<div class='vis-table-request'>"
                                + "<ul>"
                                + "<li><span>" + VIS.Msg.getMsg('Priority') + ":</span><br>" + data[s].Priority + "</li>"
                                + "<li><span>" + VIS.Msg.getMsg('Status') + ":</span><br>" + data[s].Status + "</li>"
                                + "<li><span>" + VIS.Msg.getMsg('NextActionDate') + ":</span><br>" + NextActionDate + "</li>"
                                + "</ul>"
                                + "</div>"
                                + "<p class='vis-maintain-customer-p'>"
                                + "<strong>" + VIS.Utility.encodeText(casetype) + " </strong><br />"
                                + "<span>" + VIS.Msg.getMsg('Message') + ":</span><br>" + VIS.Utility.encodeText(summary) + "</p>"
                                + "<p class='vis-feedDateTime'  style=' width: 69%; margin-right: 10px;'>" + CreatedDate + "</p>"
                                + "</div>"
                                + "</div>"
                        }

                    }
                    else {
                        if (WelcomeTabDatacontainers.find(".vis-table-request").length == 0) {

                            if (isTabscroll == false) {
                                if (VIS.Application.isRTL) {
                                    str = "<p style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                                }
                                else {
                                    str = "<p style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                                }
                                $divRequestCount.empty();
                                $hAlrtTxtTypeCount.empty();
                                $divRequestCount.text(0);
                                $divRequestCount.hide();
                                $hAlrtTxtTypeCount.text(0);
                            }
                        }
                    }
                    isTabAjaxBusy = false;
                    // $ulHomeTabMenu.on("click");
                    BindMenuClick();
                    if (activeTabType == RequestType) {
                        WelcomeTabDatacontainers.append(str);
                    }
                    $divTabMenuDataLoder.hide();
                }
            });
        }
        /* End Request */

        /* Start  Notice */
        function LoadHomeNotice() {
            isTabAjaxBusy = true;
            //    $ulHomeTabMenu.off("click");
            $divTabMenuDataLoder.show();
            $.ajax({
                url: VIS.Application.contextUrl + 'Home/GetJSONHomeNotice',
                data: { "pagesize": tabdataPageSize, "page": tabdataPage, "isTabDataRef": isTabDataRef },
                type: 'GET',
                datatype: 'json',
                success: function (result) {
                    var data = JSON.parse(result.data);
                    var str = "";
                    var strActn = "";
                    var dbdate = null;
                    if (data.length > 0) {
                        if (activeTabType == NoticeType) {
                            if (isTabDataRef) {
                                tabdataLastPage = parseInt(result.count);
                                $divNoticeCount.empty();
                                $divNoticeCount.show();
                                $hAlrtTxtTypeCount.empty();
                                $divNoticeCount.text(result.count);
                                $hAlrtTxtTypeCount.text(result.count);
                            }
                        }
                        for (var s in data) {
                            if (data[s].CDate != null && data[s].CDate != "") {
                                var cd_ = new Date(data[s].CDate);
                                dbdate = Globalize.format(cd_, "F", Globalize.cultureSelector);
                            }

                            var divtitle_ = "";
                            var title_ = data[s].Description;
                            if (title_.length <= 100) {
                                divtitle_ = "<pre><strong style='color:#666666' data-vishomercrd='title' id='" + data[s].AD_Note_ID + "'>" + VIS.Utility.encodeText(data[s].Title) + "</strong></pre>";
                            }
                            else {
                                divtitle_ = "<pre>"
                                    + "<strong  id='snoticetitle_" + data[s].AD_Note_ID + "'  style='color:#666666;' >" + VIS.Utility.encodeText(data[s].Title) + "...</strong>"
                                    + "<strong id='snoticedesc_" + data[s].AD_Note_ID + "' style='display:none; color:#666666;'>" + VIS.Utility.encodeText(data[s].Description) + "...</strong> "
                                    + "<span id='snoticemore_" + data[s].AD_Note_ID + "' data-vishomercrd='more' style='color:rgba(var(--v-c-primary), 1); float:right;height:20px'>" + VIS.Msg.getMsg("more") + "</span>"
                                    + "<span id='snoticeless_" + data[s].AD_Note_ID + "' data-vishomercrd='less' style='display:none; color:rgba(var(--v-c-primary), 1); float:right;height:20pxvis-feedTitleBar'>" + VIS.Msg.getMsg("less") + "</span>"
                                    + "</pre>";
                            }

                            str += "<div data-vishomercrd='view-recrd-cntainer' id='divrecdcntnr_" + data[s].AD_Note_ID + "' class='vis-activityContainer'>"
                                + " <div class='vis-feedTitleBar'>";

                            if (data[s].SpecialTable) {
                                str += "<h3>" + VIS.Utility.encodeText(data[s].MsgType) + "</h3>";
                            }
                            else {
                                str += "<h3>" + VIS.Utility.encodeText(data[s].MsgType) + "</h3>";
                            }


                            str += " <div class='vis-feedTitleBar-buttons'>"
                                + "  <ul>";
                            //if (data[s].SpecialTable)
                            //{
                            //    str += "<li data-vishomercrd='lispecial'><a data-vishomercrd='lispecial1' href='javascript:void(0)' id='" + data[s].Record_ID + "|" + data[s].ProcessTableName + "|" + data[s].ProcessWindowID + "' data-vishomercrd='SpecialTable' title='" + VIS.Msg.getMsg("ShowNotice") + "' class='vis-processZoomIcon vis-icon-check'> title='" + VIS.Msg.getMsg("ShowNotice") + "'</a></li>"
                            //}

                            // Renaming of Approve highlight to Acknowledge under notification
                            str += "<li data-vishomercrd='liapprove'><a href='javascript:void(0)' data-vishomercrd='approve'  id=" + data[s].AD_Note_ID + "  title='" + VIS.Msg.getMsg("Acknowledge") + "' class='vis vis-markx'></a></li>"
                                + "<li data-vishomercrd='liview'><a href='javascript:void(0)' data-vishomercrd='view' id=" + data[s].AD_Note_ID + "|" + data[s].TableName + "|" + data[s].AD_Window_ID + "|" + data[s].Record_ID + " title='" + VIS.Msg.getMsg("View") + "' class='vis vis-find'></a></li>"
                                + "</ul>"
                                + "  </div>"
                                + "</div>"
                                + "<div data-vishomercrd='more-details' id=" + data[s].AD_Note_ID + " class='vis-feedDetails'>"
                                + divtitle_
                                + " <p class='vis-feedDateTime'>" + VIS.Utility.encodeText(dbdate) + "</p>"
                                + " </div>"
                                + " </div>"

                        }
                    }
                    else {

                        if (WelcomeTabDatacontainers.find(".vis-feedTitleBar").length == 0) {

                            if (VIS.Application.isRTL) {
                                str = "<p style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                            }
                            else {
                                str = "<p style=' margin-top:200px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "</p>";
                            }
                            if (activeTabType == NoticeType) {
                                $divNoticeCount.empty();
                                $hAlrtTxtTypeCount.empty();
                                $divNoticeCount.hide();
                                $divNoticeCount.text(result.count);
                                $hAlrtTxtTypeCount.text(result.count);
                            }
                        }
                    }
                    isTabAjaxBusy = false;
                    //$ulHomeTabMenu.on("click");
                    BindMenuClick();
                    $divTabMenuDataLoder.hide();
                    if (activeTabType == NoticeType) {
                        WelcomeTabDatacontainers.append(str);
                    }

                }
            });
        }
        function ApproveNotice(Ad_Note_ID, isAcknowldge) {
            $.ajax({
                url: VIS.Application.contextUrl + 'Home/ApproveNotice',
                data: { "Ad_Note_ID": Ad_Note_ID, "isAcknowldge": isAcknowldge },
                type: 'POST',
                datatype: 'json',
                success: function (result) {
                    var data = JSON.parse(result);
                    $("#divrecdcntnr_" + Ad_Note_ID).animate({ "width": "0px", "height": "132px", "margin-left": "800px" }, 700, "", function () {
                        $("#divrecdcntnr_" + Ad_Note_ID).remove();
                    });

                }
            });
        }
        /* End Notice */

        //Bind Menu Click event
        function BindMenuClick() {
            $ulHomeTabMenu.off("click");
            $ulHomeTabMenu.on("click", function (evnt) {
                $hlnkRefTabData.show();
                WelcomeTabDatacontainers.css({ "margin-left": "auto", "margin-top": "auto" });
                WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });

                $(".vis-welcomeScreenTabMenu li").removeClass('vis-activeTab');
                if (evnt.target.nodeName == "LI") {
                    $(evnt.target).addClass('vis-activeTab');
                }
                else if (evnt.target.nodeName == "A") {
                    $(evnt.target.parentNode).addClass('vis-activeTab');
                }
                var datatab = $(evnt.target).data("vishome");
                if (datatab === "userprofile") {
                    $spanWelcomeTabtopHdr.show();
                    //$spanWelcomeTabtopHdr.css("background-position", "0px 0px");
                    activeTabType = ProfileType;
                    isTabscroll = false;
                    isTabDataRef = true;
                    $('li .vis-welcomeScreenTab-notificationBubbles').removeClass('blank').addClass('vis-feedsAlert');
                    $('li:first-child .vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');
                    $('li:last-child .vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');

                    $('.vis-welcomeScreenContainer > div').fadeOut('fast');
                    $('.vis-welcomeScreen-Data').fadeIn('fast');
                    $welcomeNewRecord.hide();
                    $wfSearchShow.hide();
                    $workflowActivity.hide();
                    adjustDivSize();
                }
                else if (datatab === "KPI") {




                    $('.vis-welcomeScreen-Data').hide('fast');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    $sAlrtTxtType.html("");
                    $spanWelcomeTabtopHdr.show();
                    $sAlrtTxtType.html(VIS.Msg.getMsg("KPI"));
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-kpi");// .css("background-position", "0px -537px");
                    WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });
                    activeTabType = kPIType;
                    if (window.VADB && window.VADB != null && VADB.Apps.KpiHomeScreen != undefined) {

                        WelcomeTabDatacontainers.css({ "text-align": "auto" });

                        activeTabType = kPIType;
                        isTabscroll = false;
                        isTabDataRef = true;
                        $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');

                        $hlnkRefTabData.css("margin-right", "17px");


                        $ulHomeTabMenu.off("click");
                        VADB.Apps.KpiHomeScreen(WelcomeTabDatacontainers, $divTabContainerLoder, $welcomeNewRecord, $workflowActivity, $workflowActivityDetails, $WelcomeScreenHdr, $divKPICount);
                        $workflowActivity.hide();
                        adjustDivSize();
                        $welcomeNewRecord.show();
                        $wfSearchShow.hide();

                        //emptyWelcomeTabDatacontainers();;
                        //var str = "<p style='margin-left:135px; margin-top:150px;'>" + VIS.Msg.getMsg('Comming Soon ') + "!!!</p>";
                        //WelcomeTabDatacontainers.html(str);
                    }
                    else {
                        $hlnkRefTabData.hide();

                        //$spanWelcomeTabtopHdr.hide();
                        WelcomeTabDatacontainers.html(VIS.Msg.getMsg("PleaseInstallVADashboard"));

                        if (VIS.Application.isRTL) {
                            //WelcomeTabDatacontainers.css({ "margin-left": "-125px", "margin-top": "200px" });
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                        else {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                            //WelcomeTabDatacontainers.css({ "margin-left": "125px", "margin-top": "200px" });
                        }
                    }


                }
                else if (datatab === "workflow") {
                    $spanWelcomeTabtopHdr.show();
                    $ulHomeTabMenu.off("click");
                    WelcomeTabDatacontainers.css("overflow-y", "auto");
                    WelcomeTabDatacontainers.css({ "text-align": "auto" });
                    activeTabType = WorkflowType;
                    isTabscroll = false;
                    isTabDataRef = true;

                    $hlnkRefTabData.css("margin-right", "17px");
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-userfeed");//  .css("background-position", "0px -76px");
                    $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    $workflowActivity.hide();
                    adjustDivSize();
                    $welcomeNewRecord.hide();
                    $wfSearchShow.show();
                    $sAlrtTxtType.empty();
                    $sAlrtTxtType.append(VIS.Msg.getMsg('WorkflowActivities'));
                    $hAlrtTxtTypeCount.empty();
                    $hAlrtTxtTypeCount.append($("#divfActivity").html());
                    $WelcomeScreenHdr.empty();
                    $WelcomeScreenHdr.append(VIS.Msg.getMsg('Activity'));
                    //var divActivityContainer = $("#welcomeScreenFeedsList");
                    //var divActivityDetailContainer = $("#workflowActivityDetails");
                    emptyWelcomeTabDatacontainers();;
                    tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                    activity = new VIS.wfActivity(WelcomeTabDatacontainers, $workflowActivityDetails, workflowActivityData, WelcomeTabDatacontainers, $wfSearchShow);
                    activity.Load(false);

                }
                else if (datatab === "notice") {
                    $spanWelcomeTabtopHdr.show();
                    $ulHomeTabMenu.off("click");
                    activeTabType = NoticeType;
                    isTabscroll = false;
                    isTabDataRef = true;

                    $hlnkRefTabData.css("margin-right", "17px");
                    WelcomeTabDatacontainers.css("overflow-y", "auto");
                    WelcomeTabDatacontainers.css({ "text-align": "auto" });
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-notice");//$spanWelcomeTabtopHdr.css("background-position", "0px -143px");
                    $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                   
                    $sAlrtTxtType.html(VIS.Msg.getMsg("Notice"));
                    $welcomeNewRecord.hide();
                    $wfSearchShow.hide();
                    tabdataLastPage = parseInt($divNoticeCount.text());
                    $divNoticeCount.text(tabdataLastPage);
                    $hAlrtTxtTypeCount.text(tabdataLastPage);
                    if (isTabAjaxBusy == false) {
                        tabdatapcount = 0, tabdataPageSize = 10, tabdataPage = 1, tabdatacntpage = 0;
                        emptyWelcomeTabDatacontainers();;
                        LoadHomeNotice();
                    }
                    $workflowActivity.hide();
                    adjustDivSize();

                }
                else if (datatab === "mytask") {
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    $sAlrtTxtType.html("");
                    $sAlrtTxtType.html(VIS.Msg.getMsg("MyTask"));
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-mytasks");//$spanWelcomeTabtopHdr.css("background-position", "0px -406px");
                    WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });
                    activeTabType = MyTaskType;
                    if (isWsp) {
                        WelcomeTabDatacontainers.css({ "text-align": "auto" });
                        $ulHomeTabMenu.off("click");
                        $spanWelcomeTabtopHdr.show();
                        activeTabType = MyTaskType;
                        isTabscroll = false;
                        isTabDataRef = false;
                        WelcomeTabDatacontainers.css("overflow-y", "hidden");
                        $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');

                        $welcomeNewRecord.show();
                        $wfSearchShow.hide();
                        $hAlrtTxtTypeCount.html($("#divMyTaskCount").html());

                        $hlnkRefTabData.css("margin-right", "17px");
                        // emptyWelcomeTabDatacontainers();;
                        //Call My Task
                        WSP.wspHomeMgr.viewWSPHome(MyTaskType, isTabDataRef);
                        $workflowActivity.hide();
                        adjustDivSize();
                    }
                    else {
                        $hlnkRefTabData.hide();

                        //$spanWelcomeTabtopHdr.hide();
                        WelcomeTabDatacontainers.html(VIS.Msg.getMsg("PleaseInstallWSPModule"));


                        if (VIS.Application.isRTL) {
                            //WelcomeTabDatacontainers.css({ "margin-left": "-125px", "margin-top": "200px" });
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                        else {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                            //WelcomeTabDatacontainers.css({ "margin-left": "125px", "margin-top": "200px" });
                        }

                    }

                }
                else if (datatab === "taskassignbyme") {
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');


                    $sAlrtTxtType.html("");
                    $sAlrtTxtType.html(VIS.Msg.getMsg("TaskAssignByMe"));
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-taskassigned");//$spanWelcomeTabtopHdr.css("background-position", "0px -340px");
                    WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });
                     activeTabType = TaskAssignByme;
                    if (isWsp) {
                        WelcomeTabDatacontainers.css({ "text-align": "auto" });
                        $spanWelcomeTabtopHdr.show();
                        $ulHomeTabMenu.off("click");
                        isTabscroll = false;
                        isTabDataRef = false;
                        activeTabType = TaskAssignByme;
                        WelcomeTabDatacontainers.css("overflow-y", "hidden");

                        $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');

                        $welcomeNewRecord.show();
                        $wfSearchShow.hide();
                        $hAlrtTxtTypeCount.html(parseInt($("#divTaskAssignBymeCount").html()));

                        $hlnkRefTabData.css("margin-right", "17px");
                        //Call  Task Assign by me
                        //   emptyWelcomeTabDatacontainers();;
                        WSP.wspHomeMgr.viewWSPHome(TaskAssignByme, isTabDataRef);
                        $workflowActivity.hide();
                        adjustDivSize();
                    }
                    else {
                        $hlnkRefTabData.hide();
                        //$spanWelcomeTabtopHdr.hide();
                        WelcomeTabDatacontainers.html(VIS.Msg.getMsg("PleaseInstallWSPModule"));

                        if (VIS.Application.isRTL) {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                        else {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                    }

                }
                else if (datatab === "documentrecive") {
                    //$ulHomeTabMenu.off("click");
                    $spanWelcomeTabtopHdr.show();

                    $hlnkRefTabData.css("margin-right", "17px");
                    $hlnkRefTabData.hide();
                    activeTabType = DocumentReceaveType;
                    isTabscroll = false;
                    isTabDataRef = true;
                    $workflowActivity.hide();
                    adjustDivSize();
                    WelcomeTabDatacontainers.css("overflow-y", "auto");
                    WelcomeTabDatacontainers.css({ "text-align": "auto" });
                    $sAlrtTxtType.html(VIS.Msg.getMsg("Documents"));
                    tabdataLastPage = 0;
                    $hAlrtTxtTypeCount.html(tabdataLastPage);

                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-document");//$spanWelcomeTabtopHdr.css("background-position-y", "-603px");
                    $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    emptyWelcomeTabDatacontainers();;
                    var str = "<p style=' margin-top:200px;text-align:center'>" + VIS.Msg.getMsg('PleaseInstallDMSModule') + "</p>";
                    WelcomeTabDatacontainers.html(str);
                    $welcomeNewRecord.hide();
                    $wfSearchShow.hide();
                }
                else if (datatab === "request") {
                    $spanWelcomeTabtopHdr.show();
                    $ulHomeTabMenu.off("click");
                    activeTabType = RequestType;
                    isTabscroll = false;
                    isTabDataRef = true;
                    WelcomeTabDatacontainers.css("overflow-y", "auto");
                    WelcomeTabDatacontainers.css({ "text-align": "auto" });
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon fa fa-bell-o");//$spanWelcomeTabtopHdr.css("background-position", "0px -209px");
                    $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    //$welcomeNewRecord.hide();
                    $welcomeNewRecord.show();
                    $wfSearchShow.hide();

                    $hlnkRefTabData.css("margin-right", "17px");
                    $sAlrtTxtType.html(VIS.Msg.getMsg("Requests"));
                    tabdataLastPage = parseInt($divRequestCount.text());
                    $hAlrtTxtTypeCount.html(tabdataLastPage);
                    if (isTabAjaxBusy == false) {
                        tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                        emptyWelcomeTabDatacontainers();;
                        LoadHomeRequest();
                    }
                    $workflowActivity.hide();
                    adjustDivSize();
                    $welcomeNewRecord.off("click");
                    $welcomeNewRecord.on("click", function () {

                        var sql = "VIS_129";
                        var n_win = executeScalar(sql);

                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction("R_Request_ID", VIS.Query.prototype.EQUAL, 0);
                        VIS.viewManager.startWindow(n_win, zoomQuery);

                    });
                }
                else if (datatab === "appointments") {
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    $sAlrtTxtType.html("");
                    $sAlrtTxtType.html(VIS.Msg.getMsg("Appointment"));
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-appointment");//$spanWelcomeTabtopHdr.css("background-position", "0px -275px");
                    WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });
                    activeTabType = AppointmentsType;
                    if (isWsp) {
                        WelcomeTabDatacontainers.css({ "text-align": "auto" });
                        $ulHomeTabMenu.off("click");
                        $spanWelcomeTabtopHdr.show();
                        activeTabType = AppointmentsType;
                        isTabscroll = false;
                        isTabDataRef = true;

                        $hlnkRefTabData.css("margin-right", "17px");
                        WelcomeTabDatacontainers.css("overflow-y", "auto");
                        $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');


                        $welcomeNewRecord.hide();
                        $wfSearchShow.hide();
                        tabdataLastPage = parseInt($divAptCount.text());
                        $hAlrtTxtTypeCount.html(tabdataLastPage);
                        // emptyWelcomeTabDatacontainers();;
                        tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                        WSP.wspHomeMgr.viewWSPHome(AppointmentsType, isTabDataRef);
                        $workflowActivity.hide();
                        adjustDivSize();

                    }
                    else {
                        $hlnkRefTabData.hide();
                        //$spanWelcomeTabtopHdr.hide();
                        WelcomeTabDatacontainers.html(VIS.Msg.getMsg("PleaseInstallWSPModule"));

                        if (VIS.Application.isRTL) {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                        else {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                    }
                }
                else if (datatab === "notes") {
                    $('.vis-welcomeScreen-Data').hide('slow');
                    $('.vis-welcomeScreenFeeds').fadeIn('fast');
                    $sAlrtTxtType.html("");
                    $sAlrtTxtType.html(VIS.Msg.getMsg("MyNotes"));
                    $spanWelcomeTabtopHdr.removeClass().addClass("vis-welcomeScreenContentTittle-icon vis vis-contacts");//  .css("background-position", "0px -473px");
                    WelcomeTabDatacontainers.css({ "text-align": "", "margin-top": "" });
                    activeTabType = NotesType;
                    if (isWsp) {
                        WelcomeTabDatacontainers.css({ "text-align": "auto" });
                        $ulHomeTabMenu.off("click");
                        activeTabType = NotesType;
                        $spanWelcomeTabtopHdr.show();
                        isTabscroll = false;
                        isTabDataRef = true;

                        $hlnkRefTabData.css("margin-right", "17px");
                        WelcomeTabDatacontainers.css("overflow-y", "auto");
                        $('.vis-welcomeScreenTab-notificationBubbles').removeClass('vis-feedsAlert').addClass('blank');


                        $welcomeNewRecord.show();
                        $wfSearchShow.hide();
                        tabdataLastPage = parseInt($divNotesCount.text());
                        $hAlrtTxtTypeCount.html(tabdataLastPage);
                        tabdatapcount = 0, tabdataPageSize = p_size, tabdataPage = p_no, tabdatacntpage = 0;
                        emptyWelcomeTabDatacontainers();;
                        WSP.wspHomeMgr.viewWSPHome(NotesType, isTabDataRef);
                        $workflowActivity.hide();
                        adjustDivSize();
                    }
                    else {
                        $hlnkRefTabData.hide();
                        //$spanWelcomeTabtopHdr.hide();
                        WelcomeTabDatacontainers.html(VIS.Msg.getMsg("PleaseInstallWSPModule"));

                        if (VIS.Application.isRTL) {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                        else {
                            WelcomeTabDatacontainers.css({ "text-align": "center", "margin-top": "200px" });
                        }
                    }
                }
                // evnt.stopPropagation();
            });
        }

        $('#vis_appInfoWindow').click(function () {
            VIS.InfoMenu.show($(this));
        });

        function emptyWelcomeTabDatacontainers() {
            if (WelcomeTabDatacontainers.children().length > 0)
                WelcomeTabDatacontainers.children().remove();
            WelcomeTabDatacontainers.empty();
        }

        return {
            initHome: initHome,
            adjustDivSize: adjustDivSize,
            BindMenuClick: BindMenuClick,
            getActiveTab: getActiveTab
        }

    };
    VIS.HomeMgr = HomeMgr();

})(VIS, jQuery);




