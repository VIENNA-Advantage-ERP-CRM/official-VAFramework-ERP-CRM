; (function (VIS, $) {


    function VTreeMaintenance() {
        this.frame;
        this.windowNo;
        var $self = this;
        var $root = $('<div class="vis-forms-container" style="padding:3px 0 7px;background-color:white;height:100%"/>');

        var $leftTreeKeno;
        //var $isSummary = false;
        var $isSummary = true;
        var selectedNode;
        var $cmbSelectedType;
        //  var $cmbSelectedID;
        var $treeID;
        var $cmbIsallnodes;
        var TemplateTree = "";
        var $dragMenuNodeID = 0;
        var $treeDataObjectForMatch;
        var pageLength = 50;
        var pageNo = 1;
        //var rightMenuSelectedID = 0;
        var getIDFromContainer;
        var dragMenuNodeIDArray = [];
        var ExistItem = [];
        var $checkMorRdragable = true;
        var $downImg = null;
        var $dragTreeDataNodeID;
        var getParentID = 0;
        var $bsyDiv = null;
        var $bsyDivtreechnage = null;
        var $bsyDivforbottom = null;
        var $bsyDivMenu = null;
        var $bsyDivTree = null;
        var $scrollBottom = true;
        var $dragtrue = true;
        var $recodeCount = null;
        var $getLifromTree;
        var $getDataForTree;
        var movingNode = null;
        var tbname = null;
        var searchRightext = "";
        var $deleteResult = null;
        var $midChildDrag = true;
        var searchChildNode = null;
        var pageNoForChild = 1;
        var pageSizeForChild = 50;
        var childlevel = 0;
        var delNodId = 0;
        var getMovingdiv = null;
        var $chkValueFromDailog = false;
        var $dropableItem = null;
        var $getchildCount = false;
        var $demandsMenu = "All";
        var $rightmenuScroll = false;
        var $setorderflagss = true;


        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSet";
        var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";


        var $ldiv = $("<div  class='VIS-TM-left-tree'>");
        var $mdiv = $("<div  class='VIS-TM-middle'>");
        var $rdiv = $("<div class='VIS-TM-right-slide'>");
        var mouseEnter = false;


        //executeDataSet
        var executeDataSet = function (sql, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }

            var dataSet = null;

            getDataSetJString(dataIn, async, function (jString) {
                dataSet = new VIS.DB.DataSet().toJson(jString);
                if (callback) {
                    callback(dataSet);
                }
            });

            return dataSet;
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

           // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
            $.ajax({
                url: nonQueryUrl + 'y',
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
        
        var $lTopLeftDiv = $('<div class="VIS-TM-data-head" style="padding-top: 8px;">')
        var $lTopRightDiv = $('<div class="VIS-TM-data">');
        var $chkSummaryLevel = $('<input disabled type="checkbox" checked="true" />');
        var $lblSummaryLevel = $('<span>' + VIS.Msg.getMsg("OnlySummaryLevel") + '<span/>');
        var $lblSelectTree = $('<h4>' + VIS.Msg.getMsg("SelectTree") + '</h4>');

        var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());

        $lTopLeftDiv.append($lblSelectTree).append($lTopRightDiv.append($chkSummaryLevel).append($lblSummaryLevel));

        //$lTopLeftDiv.append($lblSelectTree).append($lTopRightDiv);

        var $lTopMidDiv = $('<div class="VIS-TM-treeCombo">');
        var $cmbSelectTree = $('<select></select>');

        //var $cmbRefresh = $('<input class="ass-btns search-icon" type="button" title=' + VIS.Msg.getMsg("VIS_RefreshSelectTree") + ' >');
        //var $treeRefresh = $('<input class="ass-btns search-icon" type="button" style="margin-left: 5px;" title=' + VIS.Msg.getMsg("VIS_RefreshTree") + ' >')
        var $treeRefresh = $('<span style="display:none" class="VIS-Tm-btnicon vis vis-refresh"  title=' + VIS.Msg.getMsg("RefreshSelectTree") + '></span>');
        var $cmbRefresh = $('<span class="VIS-Tm-btnicon vis vis-refresh" style="float:left" title="' + VIS.Msg.getMsg("tmRefresh") + '"></span>');
        //var $cmbRefresh = $('<span class="VIS-Tm-search-icontree glyphicon glyphicon-refresh" style="float:left"></span>');

        var $treeExpandColapse = $('<span class="VIS-Tm-btnicon fa fa-minus-square-o"  style="display:none;" title="' + VIS.Msg.getMsg("TreeCollapse") + '" ></span>')

        var $treeCollapseColapse = $('<span class="VIS-Tm-btnicon fa fa-plus-square-o" title="' + VIS.Msg.getMsg("TreeExpand") + '" ></span>')


        ////var ZoomTreeWindow = $('<span class="VIS-Tm-search-icontree glyphicon glyphicon-plus" style="float:left" title="' + VIS.Msg.getMsg("CreateNewTree") + '"></span>');


        ////var ZoomTreeWindow = $("<img class='VIS-Tm-zoombtn'  src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png'></img>");

        var ZoomTreeWindow = $("<i class='vis vis-find VIS-Tm-btnicon' title='" + VIS.Msg.getMsg("CreateNewTree") + "'></i>");

        var treeCmbDisable = $('<div class="vis-tm-treeCmbDisable "></div>');

        $lTopMidDiv.append(treeCmbDisable).append($cmbSelectTree).append(ZoomTreeWindow).append($cmbRefresh).append($treeExpandColapse).append($treeCollapseColapse);

        //$lTopMidDiv.append($cmbSelectTree).append($cmbRefresh);

        var $treeBackDiv = $('<div class="VIS-TM-tree-wrap">');
        var $leftTreeDiv = $('<div class="VIS-TM-root-node" style="overflow:visible">');

        //        var topTreeDiv = $('<div style="width: 100%;height: 262px; overflow: auto;">');


        var topTreeDiv = $('<div class="vis-tm-topTreeDiv">');
        //////var topTreeDiv = $('<div style="width: 100%; overflow: auto;">');

        //$treeBackDiv.append($treeRefresh).append($leftTreeDiv);


        $treeBackDiv.resizable({
            handles: 'n,s,se'
        });


        var $deleteArea = $('<div style="z-index:9;width:28px;display:none;text-align:center;float: right; height: 95%; position: absolute; right: 5px;"></div>');
        //var $deleteImage = $('<span class="glyphicon glyphicon-trash" style="display:none;position: relative;top: 50%;"></span>');

        var $deleteImage = $('<span class="VIS-Tm-deletetreenode" style="margin:auto;display:none;position: relative;top: 50%;"></span>');

        $deleteArea.append($deleteImage);

        topTreeDiv.append($treeRefresh).append($deleteArea).append($leftTreeDiv);

        $treeBackDiv.append(topTreeDiv);

        //$treeBackDiv.append(topTreeDiv.append($treeRefresh).append($leftTreeDiv));

        var $searchDiv = $("<div class='VIS-TM-tree-search'>");
        var $cmbSearch = $("<input disabled placeholder='" + VIS.Msg.getMsg("Search") + "' type='search'>");
        //var $btnSearch = $("<input class='ass-btns search-icon' type='button'>");


        //var $btnSearch = $("<input class='ass-btns search-icon glyphicon glyphicon-search' style='top:0px' type='button'>");



        var $btnSearch = $('<span class="VIS-Tm-btnicon vis vis-search"></span>');

        var $treeNodeSearch = $('<div class="vis-tm-b-s-ryt">');
        var $chktreeNode = $('<input type="checkbox" disabled>');
        //        var $lblNodetext = $('<label style="font-weight: normal;margin-bottom:0px">Tree Node</label>');
        var $lblNodetext = $('<label style="font-weight: normal;margin-bottom:0px">' + VIS.Msg.getMsg("NodeItem") + '</label>');

        $treeNodeSearch.append($chktreeNode).append($lblNodetext);

        var crossImages = $('<span class="vis-tmlinkdel-icon"></span>');

        $searchDiv.append($cmbSearch).append(crossImages).append($treeNodeSearch).append($btnSearch);

        //var $mTopHeader = $("<div class='VIS-TM-data-head'>");

        var $mTopHeader = $("<div class='VIS-TM-data-headMID'>");

        //var $deleteChild = $('<div class="glyphicon glyphicon-trash" style="float:right" ></div>');

        //var $deleteChild = $("<span style='cursor:pointer;font-size:18px;float:right;margin:9px 0'  class='glyphicon glyphicon-trash vis-tm-delete' title='" + VIS.Msg.getMsg("VA005_DeleteChild") + "' ></span>");
        //var $deleteChild = $("<span style='margin-top:10px;margin-bottom:4px' class='VIS-Tm-search-icontree glyphicon glyphicon-trash vis-tm-delete' title='" + VIS.Msg.getMsg("DeleteChild") + "' ></span>");
        /////var $deleteChild = $("<span style='margin-top:10px;margin-bottom:4px' class='VIS-Tm-search-icontree VIS-Tm-unlinkforbottmo vis vis-link ' title='" + VIS.Msg.getMsg("DeleteChild") + "' ></span>");

        var $deleteChild = $("<span style='display:inherit;' class='VIS-Tm-unlinkforbottmo vis vis-link ' title='" + VIS.Msg.getMsg("UnlinkNode") + "' ></span>");

        //"<span style='cursor:pointer;font-size:18px;'  class='glyphicon glyphicon-trash' title='" + VIS.Msg.getMsg("VA005_DeleteChild") + "' ></span>"

        var $lblMh4 = $('<h4 style="float:left">' + VIS.Msg.getMsg("SelectTree") + '</h4>');
        var $mData = $("<div class='VIS-TM-dataWrap' style='padding-top:0px' >");
        var $recordeNotFound = $('<span class="VIS-TM-recordeNotFound">' + VIS.Msg.getMsg("TMRecordNotFound") + '</span>');

        var $ulMid = $('<ul class="vis-tm-sortable" style="list-style: none;padding:0">');

        var $chkAllCheckOrNot = $('<input type="checkbox" disabled />');

        var $checkSearchOrNot = $('<span class="vis vis-search" style="display:none;cursor:pointer;float:left;margin: 14px 0 0 6px;font-size: 10px;" ></span>')



        $mData.append($ulMid).append($recordeNotFound);
        //  $mTopHeader.append($chkAllCheckOrNot).append($lblMh4.append($checkSearchOrNot)).append($deleteChild);

        //var $squenceDailog = $("<span style='display:inherit;margin-top:10px;margin-bottom:4px' class='VIS-Tm-unlinkforbottmo ' title='" + VIS.Msg.getMsg("UnlinkNode") + "' ></span>");

        /////var $squenceDailog = $("<span style='display:inherit;margin-top:10px;margin-bottom:4px' class='vis-tm-childnodeinidailog glyphicon glyphicon-list-alt' title='" + VIS.Msg.getMsg("SetOrder") + "' ></span>");

        var $squenceDailog = $('<span style="display:inherit;" class="vis-tm-childnodeinidailog" title="' + VIS.Msg.getMsg("SetOrder") + '">Set Order</span>');

        $squenceDailog.addClass("vis-tm-delete");


        var mMouseRestrict = $('<div style="display:none;position:absolute;height: 40.2px;width: 100%; z-index: 5;"></div>'); 


        $mTopHeader.append(mMouseRestrict).append($chkAllCheckOrNot).append($lblMh4).append($checkSearchOrNot).append($squenceDailog).append($deleteChild);
        ////$mTopHeader.append($chkAllCheckOrNot).append($lblMh4).append($checkSearchOrNot).append($squenceDailog).append($deleteChild);


        //        $mTopHeader.append($lblMh4).append($deleteChild);

        var $rTopHeard = $("<div class='VIS-TM-data-head'>");
        var $lblRh4 = $('<h4>' + VIS.Msg.getMsg("AllItems") + '</h4>');
        var $rightDataDiv = $("<div class='VIS-TM-dataWrap'>");
        var $ulBackDiv = $("<div class='VIS-TM-treeList'>");
        var $ulRight = $("<ul style='list-style: none' class='scrolldrag' >");

        var $cmbRightSearch = $("<input disabled placeholder='" + VIS.Msg.getMsg("Search") + "' type='search'>");
        //var $btnRightSearch = $("<input class='ass-btns search-icon' type='button'>");


        var $btnRightSearch = $('<span class="VIS-Tm-btnicon vis vis-search" style="top:0px;float:none"></span>');


        var $pathRightlist = $("<div class='VIS-TM-path'>");
        var $pathInfo = $("<i style='font-weight:600'>");

        var $rightMenuDemand = $('<select disabled style="float: right; min-width: 100px"></select>');

        var $lblGetMenu = $('<span style="margin: 0 7px;">' + VIS.Msg.getMsg("Show") + '<span/>');

        //$rTopHeard.append($lblRh4).append($lblGetMenu).append($rightMenuDemand);


        // var $chkTrace = $('<input type="checkbox" disabled style="margin-right:25px;float:left" />');

        var $chkTrace = $('<input type="checkbox"  style="float:left;margin-right:5px" disabled  />');
        //        var $lblTrace = $('<span style="float:left;margin-right:2px">' + VIS.Msg.getMsg("TraceItem") + '<span/>');

        var $lblTrace = $('<span>' + VIS.Msg.getMsg("TraceItem") + '<span/>');


        var $chkAllRightPannel = $('<input type="checkbox" disabled />');


        var unlinkeAllNode = $('<span class="vis-unlinkallNodes vis-tm-opacity" title="' + VIS.Msg.getMsg("UnlinkItem") + '"  ><b></b><b></b><b></b></span>');


        //$rTopHeard.append($chkAllRightPannel).append($lblRh4).append($('<div style="float: right;">').append($lblTrace).append($chkTrace).append($lblGetMenu).append(unlinkeAllNode).append($rightMenuDemand));

        $rTopHeard.append($chkAllRightPannel).append($lblRh4).append($('<div class="vis-tm-r-list-w">').append($lblGetMenu).append($rightMenuDemand).append(unlinkeAllNode));




        //$rightDataDiv.append($ulBackDiv.append($ulRight)).append($pathRightlist.append($pathInfo));
        //$rightDataDiv.append($cmbRightSearch).append($btnRightSearch).append($ulBackDiv.append($ulRight)).append($pathRightlist.append($pathInfo));
        //var $secoundDiv = $('<div style="height: 50%;float: left;width: 100%;padding-top: 15px;">');

        //var $secoundDiv = $('<div style="height: 25%;float:left;width:100%;overflow:auto">');
        //        var $secoundDiv = $('<div style="float:left;width:100%;overflow:auto;">');

        //var nodeItemDelwithdrag = $('<div style="width: 28px; text-align: center; float: right; height: 50%; position: absolute; right: 5px; display: block; background-color: rgb(255, 192, 203);" class=""><span class="VIS-Tm-deletetreenode" style="margin: auto; position: relative; top: 50%; display: block;"></span></div>');

        var nodeItemDelwithdrag = $('<div class="vis-tm-nodeItemDeleteliketopdiv"><span class="VIS-Tm-deletetreenode vis-tm-nodeItemDeletespans" ></span></div>');

        var $secoundDiv = $('<div style="float:left;width:100%;">');

        var bottomdivdisable = $('<div style="z-index:999;height:100%;display:none;position:absolute; left:0; right:0; top:0; bottom:0;"></div>');



        $secoundDiv.append(bottomdivdisable).append($mTopHeader).append($mData).append(nodeItemDelwithdrag);


        var leftMianDataDiv = $('<div class="VIS-TM-leftMianDataDiv">');
        //var leftMianDataDiv = $('<div style="height:calc(100% - 118px);height:-moz-calc(100%-118px);height:-webkit-calc(100%- 118px);position:relative;z-index:2;float:left;width:100%;">');

        leftMianDataDiv.append($treeBackDiv).append($secoundDiv);


        var leftstopdiv = $('<div style="display:none;height: 100%; width: 100%; position: absolute;z-index: 9999;"></div>');

        $ldiv.append(leftstopdiv).append($('<div style="height:100%;" >').append($lTopLeftDiv).append($lTopMidDiv).append(leftMianDataDiv).append($searchDiv));
        //$ldiv.append($('<div style="height:100%" >').append($lTopLeftDiv).append($lTopMidDiv).append($treeBackDiv).append($searchDiv));

        //   $mdiv.append($('<div style="height:100%">').append($mTopHeader).append($mData));


        var $SearchRightTopWrap = $('<div class="VIS-TM-Righttree-search" >');

        //var $SearchRightTopWrap = $('<div class="VIS-TM-tree-search" >');


        var rightCrossImage = $('<span class="vis-tmlinkdel-icon"></span>');

        $SearchRightTopWrap.append($cmbRightSearch).append(rightCrossImage).append($btnRightSearch);

        //$rightDataDiv.append($('<div>').append($SearchRightTopWrap)).append($ulBackDiv.append($ulRight)).append($pathRightlist.append($pathInfo));

        var $recordeNotFoundRight = $('<span class="VIS-TM-recordeNotFound" style="margin-top:50%" >' + VIS.Msg.getMsg("TMRecordNotFound") + '</span>');

        var onDisableDiv = $('<div style="z-index:999;height:100%;display:none;position:absolute; left:0; right:0; top:0; bottom:0;"></div>');
        //$rightDataDiv.append($SearchRightTopWrap).append($ulBackDiv.append(onDisableDiv).append($ulRight).append($recordeNotFoundRight)).append($pathRightlist.append($pathInfo));
        $rightDataDiv.append(onDisableDiv).append($SearchRightTopWrap).append($ulBackDiv.append($ulRight).append($recordeNotFoundRight)).append($pathRightlist.append($pathInfo));





        ///$rightDataDiv.append($SearchRightTopWrap).append($ulBackDiv.append($ulRight).append($recordeNotFoundRight)).append($pathRightlist.append($pathInfo));

        //$rightDataDiv.append($SearchRightTopWrap).append($ulBackDiv.append($ulRight)).append($pathRightlist.append($pathInfo));

        $rdiv.append($('<div  style="height:100%">').append($rTopHeard).append($rightDataDiv));

        //$rdiv.append($('<div  style="height:100%">').append($rTopHeard).append($rightDataDiv));
        $root.append($ldiv).append($rdiv);
        //$root.append($ldiv).append($mdiv).append($rdiv);


        function GetTreeNameForCombo() {
            // $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/TreeDataForCombo",
                success: function (data) {
                    var res = JSON.parse(data);
                    if (res) {
                        $cmbSelectTree.append($("<Option value=''> </option>"));
                        for (var i = 0; i < res.length; i++) {
                            $cmbSelectTree.append($("<Option value=" + res[i].ID + " data-type='" + res[i].TreeType + "' data-isallnodes='" + res[i].IsAllNodes + "'  >" + VIS.Utility.encodeText(res[i].Name) + "</option>"));
                        }
                    }
                    if (selectedNode != null) {
                        $cmbSelectTree.val(selectedNode.attr("value"));
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "visible";
                },
            });
        };

        function cmbTreeOnRefresh() {
            treeCmbDisable.css("display", "none");
            $treeID = 0;
            $ulRight.empty();
            $ulMid.empty();
            //$leftTreeKeno.data("kendoTreeView").destroy();
            //$leftTreeKeno.empty();
            $pathInfo.empty();
            searchRightext = "";
            $cmbRightSearch.val("");
            selectedNode = null;
            $deleteChild.addClass("vis-tm-delete");
            $cmbSearch.val("");
            window.setTimeout(function () {
                $ulRight.empty();
                $leftTreeKeno.data("kendoTreeView").destroy();
                $leftTreeKeno.empty();
            }, 200);

            $lblRh4.text(VIS.Msg.getMsg("SelectTree"));
            $lblMh4.text(VIS.Msg.getMsg("SelectTree"));
            //$recordeNotFoundRight.css("display", "inherit");
            $recordeNotFound.css("display", "none");

            $squenceDailog.addClass("vis-tm-delete");

            $chktreeNode.prop("disabled", true);
            $chkTrace.prop("disabled", true);
            $chktreeNode.css("cursor", "not-allowed");
            $chkTrace.css("cursor", "not-allowed");

            $cmbSearch.prop("disabled", true);
            $cmbRightSearch.prop("disabled", true);
            $rightMenuDemand.prop("disabled", true);

            $chkAllRightPannel.prop("disabled", true);
            $chkAllRightPannel.css("cursor", "not-allowed");

            rightCrossImage.css("display", "none");

            crossImages.css("display", "none");

            $chkTrace.prop("checked", false);
            $chkAllRightPannel.prop("checked", false);
            $chktreeNode.prop("checked", false);
            $chkAllCheckOrNot.prop("checked", false);
        };

        function GetCountOnTreeChanges() {
            var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            table_id = executeDataSet(table_id, null, null);
            if (table_id.tables[0].rows.length > 0) {
                table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            }

            var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
            tablename = executeDataSet(tablename, null, null);
            if (tablename.tables[0].rows.length > 0) {
                tablename = tablename.tables[0].rows[0].cells["tablename"];
            }


            TreeTableName();
            //  var sqlQry = "SELECT Count(*) as Count FROM " + tableTreeName + " WHERE isactive ='Y' AND AD_Tree_ID=" + $treeID;

            var sqlQry = "SELECT Count(*) as Count FROM " + tablename + " WHERE IsActive='Y' AND " + tablename + "_ID  IN (SELECT Node_ID FROM " + tableTreeName + " where AD_Tree_ID=" + $treeID + ")";

            sqlQry = VIS.MRole.addAccessSQL(sqlQry, tablename, true, false);

            sqlQry = executeDataSet(sqlQry, null, null);
            sqlQry = sqlQry.tables[0].rows[0].cells["count"];
            if (sqlQry > 0) {
                chkCountForrestriction = true;
            }
            else {
                chkCountForrestriction = false;
            }

            if (bindornot == "false") {
                if (sqlQry == convertmenuArray.length) {
                    var sqlQryss = "SELECT node_id  FROM " + tableTreeName + " WHERE isactive ='Y' AND AD_Tree_ID=" + $treeID;
                    sqlQryss = executeDataSet(sqlQryss, null, null);
                    if (sqlQryss != null) {
                        var node_Idss = null;
                        for (var t = 0; t < sqlQryss.tables[0].rows.length; t++) {
                            node_Idss = sqlQryss.tables[0].rows[t].cells["node_id"];

                            if ($.inArray(node_Idss, convertmenuArray) < 0) {
                                chkCountForrestriction = true;
                                break;
                            }
                            else {
                                chkCountForrestriction = false
                            }

                        }
                    }
                }
            }
        };


        var chkCountForrestriction = false;
        function TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary) {
            // $bsyDiv[0].style.visibility = "visible";
            $bsyDivTree[0].style.visibility = "visible";


            if (!$chkSummaryLevel.is(":checked")) {
                GetCountForRestriction();

                if ($chkSummaryLevel.is(":checked")) {
                    $isSummary = true;
                }
                else {
                    $isSummary = false;
                }

            }




            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/BindTree",
                type: 'Post',
                data: { treeType: $cmbSelectedType, AD_Tree_ID: $treeID, isAllNodes: $cmbIsallnodes, isSummary: $isSummary },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        LoadTreeData(result);





                        window.setTimeout(function () {
                            window.setTimeout(function () {
                                if ($cmbSelectTree.val() == "") {
                                    cmbTreeOnRefresh();
                                }

                                else if ($cmbSelectTree.val() == null) {
                                    cmbTreeOnRefresh();
                                }
                                GetCountOnTreeChanges();
                            }, 400);
                        }, 500);


                        $getDataForTree = result;
                        $treeDataObjectForMatch = result.settree;
                    }
                    // SetTreeHeight();
                    //  $bsyDiv[0].style.visibility = "hidden";
                    $bsyDivTree[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    //   $bsyDiv[0].style.visibility = "hidden";
                    $bsyDivTree[0].style.visibility = "hidden";
                },
            });
        };

        //function DailogForTreeMoreRecorde() {
        //    $bsyDiv[0].style.visibility = "hidden";           
        //    var createTab = new VIS.ChildDialog();
        //    //  createTab.setHeight(150);
        //    // createTab.setWidth(350);
        //    createTab.setEnableResize(false);
        //    createTab.setTitle(VIS.Msg.getMsg('VIS_TM_WantToUnlinked'));
        //    createTab.setModal(true);
        //    createTab.setContent(dailogContainer);
        //    createTab.show();
        //    createTab.onClose = function () {

        //    };
        //    createTab.onOkClick = function (e) {

        //    };
        //    createTab.onCancelClick = function () {

        //    };
        //};


        function AskForMoreRecorde() {
            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            //tree = VIS.DB.executeDataSet(tree, null, null);
            //treeType = tree.tables[0].rows[0].cells["treetype"];
            //var tbname = "";
            //var tablename = "";
            //if (treeType == "PR") {
            //    tbname = "ad_treenodepr"
            //}
            //else if (treeType == "BP") {
            //    tbname = "ad_treenodebp"
            //}
            //else if (treeType == "MM") {
            //    tbname = "ad_treenodemm"
            //}
            //else {
            //    tbname = "ad_treenode"
            //}

            //var sqlQry = "SELECT Count(*) as Count FROM " + tbname + " WHERE isactive ='Y' AND AD_Tree_ID=" + $treeID;
            //sqlQry = VIS.DB.executeDataSet(sqlQry, null, null);
            //sqlQry = sqlQry.tables[0].rows[0].cells["count"];

            //if (sqlQry > 5000)
            //{
            //    VIS.ADialog.confirm("itwilltaketime.Wanttocontinue.", true, "", "Confirm", function (result)
            //    {
            //        if (!result) {
            //            $bsyDivTree[0].style.visibility = "hidden";
            //            $chkSummaryLevel.prop("checked", "true");
            //            return;
            //        }
            //        else {
            //            $treeNodeSearch.css("display", "none");
            //            $cmbSearch.css("border-right", "none");
            //            $isSummary = false;
            //            $secoundDiv.css("display", "none");
            //            $treeBackDiv.css("border-bottom", "0px");
            //            $treeBackDiv.css("height", "100%");
            //            topTreeDiv.css("height", "100%");
            //            $treeBackDiv.resizable('destroy')
            //            topTreeDiv.removeClass("VIS-TM-resizediv");
            //            if (selectedNode != null) {
            //                TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
            //            }

            //        }
            //    });
            //}
            //  else
            //  {
            $treeNodeSearch.css("display", "none");
            //$cmbSearch.css("border-right", "none");
            $isSummary = false;
            $secoundDiv.css("display", "none");
            //$treeBackDiv.css("border-bottom", "0px");
            $treeBackDiv.css("height", "100%");
            topTreeDiv.css("height", "100%");
            $treeBackDiv.resizable('destroy')
            topTreeDiv.removeClass("VIS-TM-resizediv");
            if (selectedNode != null) {
                TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
            }
            // }
        };



        var searchflag = true;
        var clickontreenode = true;

        function LoadTreeData(result) {
            //$bsyDiv[0].style.visibility = "visible";
            $bsyDivTree[0].style.visibility = "visible";

            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            //tree = VIS.DB.executeDataSet(tree, null, null);
            //treeType = tree.tables[0].rows[0].cells["treetype"];
            //var tbname = "";
            //var tablename = "";
            //if (treeType == "PR") {
            //    tbname = "ad_treenodepr"
            //}
            //else if (treeType == "BP") {
            //    tbname = "ad_treenodebp"
            //}
            //else if (treeType == "MM") {
            //    tbname = "ad_treenodemm"
            //}
            //else {
            //    tbname = "ad_treenode"
            //}

            //var sqlQry = "SELECT Count(*) as Count FROM " + tbname + " WHERE AD_Tree_ID=" + $treeID;
            //sqlQry = VIS.DB.executeDataSet(sqlQry, null, null);
            //sqlQry = sqlQry.tables[0].rows[0].cells["count"];

            //if (sqlQry > 20000) {
            //    // $recodeCount.css("display", "inherit");
            //}
            //else {
            //    // $recodeCount.css("display", "none");
            //}

            treeCmbDisable.css("display", "inherit");
            if ($leftTreeKeno && $leftTreeKeno.data("kendoTreeView") != undefined) {
                $leftTreeKeno.data("kendoTreeView").destroy();
                $leftTreeKeno.empty();
            }

            //var ds = new kendo.data.HierarchicalDataSource({
            //    data: result.settree,
            //    schema: {
            //        model: {
            //            hasChildren: function (e) {
            //                if (e.IsSummary && e.items != null) {

            //                    return true;
            //                }
            //                else {

            //                    return false;
            //                }
            //            },
            //            children: "items"
            //        }
            //    }
            //});

            $leftTreeKeno = $leftTreeDiv.kendoTreeView({
                autoScroll: true,
                dragAndDrop: true,
                drag: onDrag,
                drop: onDrop,
                dataSource: result.settree,
                select: function (e) {

                    $bsyDivforbottom[0].style.visibility = "visible";
                    if ($chkSummaryLevel.is(":checked")) {
                        leftstopdiv.css("display", "inherit");
                    }


                    $setorderflagss == true;


                    $ulMid.empty();
                    $chkAllCheckOrNot.prop("checked", false);

                    $leftTreeDiv.find(".k-state-selected").removeClass("k-state-selected");

                    $(e.node).find(".k-state-hover").addClass("k-state-selected").removeClass("k-state-hover");
                    var selectNodeText = $($($(e.node).find(".treechild")[0]).find("p")).text();
                    $lblMh4.text(selectNodeText);
                    getParentID = $($(e.node).find(".treechild")[0]).attr("data-nodeid");
                    $scrollBottom = true;

                    //$squenceDailog.removeClass("vis-tm-delete");

                    existItss = -1;
                    $deleteChild.addClass("vis-tm-delete");
                    $recordeNotFound.css("display", "none");
                    searchflag = false;

                    $leftTreeDiv.find(".vis-tm-selectedkendoNode").removeClass("vis-tm-selectedkendoNode");
                    $leftTreeDiv.find(".k-state-selected").find("p").addClass("vis-tm-selectedkendoNode");
                    window.setTimeout(function () {
                        $leftTreeDiv.find(".vis-tm-selectedkendoNode").removeClass("vis-tm-selectedkendoNode");
                        $leftTreeDiv.find(".k-state-selected").find("p").addClass("vis-tm-selectedkendoNode");

                        pageNoForChild = 1;
                        pageSizeForChild = 50;

                        if ($chktreeNode.is(":checked")) {
                            searchChildNode = $cmbSearch.val();
                        }
                        else {
                            searchChildNode = "";
                        }

                        //  searchChildNode = $cmbSearch.val();
                        if ($chktreeNode.is(":checked")) {
                            getTreeNodeChkValue = "true";
                        }
                        else {
                            getTreeNodeChkValue = "false";
                        }


                        window.setTimeout(function () {
                            getParentID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
                            if ($chkSummaryLevel.is(":checked")) {
                                $leftTreeDiv.find(".k-state-hover").removeClass("k-state-hover");
                                GetDataTreeNodeSelect(getParentID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                            }
                        }, 300);
                    }, 300);
                },
                template: "<div class='treechild'  data-getparentnode='#= item.getparentnode #'       data-IsSummary='#= item.IsSummary #' data-TableName='#= item.TableName #' data-NodeID='#= item.NodeID #'   data-TreeParentID='#= item.TreeParentID #' data-ParentID='#= item.ParentID #'  >" +

                    //"<div class='imgdivTree' style='float:left;margin-top:3px'>" +
                    //"<img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='height:20px' >" +
                    //"</div>" +

                                "<div class='imgdivTree'>" +
                                    "<i class='fa fa-folder-o' data-imgsrc='#= item.ImageSource #'></i>" +
                                 "</div>" +

                                //"<div class='textdivTree VIS-TM-textdivTreedata' style='float:left;margin:3px 5px 0 10px'>" +

                                "<div class='textdivTree'>" +

                                "<p>" + VIS.Utility.encodeText("#= item.text #") + "</p>" +
                                     //"<p>#= item.text #</p>" +
                                "</div>" +

                            "</div>"
            });

            // $leftTreeKeno.data("kendoTreeView").expand(".k-item");
            DropMenu();
            //OnHoverTree();
            //$bsyDiv[0].style.visibility = "visible";
            //$bsyDivTree[0].style.visibility = "hidden";

            $leftTreeKeno.find(".k-plus").on("click", function () {
                if ($(this).hasClass("k-minus")) {
                    $treeCollapseColapse.css("display", "inherit");
                    $treeExpandColapse.css("display", "none");
                    //                    alert("minus");
                }

                if ($(this).hasClass("k-plus")) {
                    $treeExpandColapse.css("display", "inherit");
                    $treeCollapseColapse.css("display", "none");
                    //alert("plus");
                }

            });


            // $leftTreeDiv.items().each(function (i, el)
            // {
            $leftTreeKeno.find(".k-in").on("dblclick", function (event) {

                if ($($leftTreeDiv.find(".k-state-selected").parent().find("span")[0]).hasClass("k-minus")) {
                    $treeCollapseColapse.css("display", "inherit");
                    $treeExpandColapse.css("display", "none");
                }

                if ($($leftTreeDiv.find(".k-state-selected").parent().find("span")[0]).hasClass("k-plus")) {
                    $treeExpandColapse.css("display", "inherit");
                    $treeCollapseColapse.css("display", "none");
                }

            });
            //});

            //window.setTimeout(function () {
            //    $leftTreeKeno.find(".k-in").css("min-width", "120px");
            //}, 100);
            window.setTimeout(function () {
                treeCmbDisable.css("display", "none");
            }, 200);


            if (changeSeqFlag == true) {
                $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                //$leftTreeDiv.find("div").find("div[data-nodeid='" + onseqSelectID + "']").parent().addClass("k-state-selected")[0].scrollIntoView();

                $leftTreeDiv.find("div").find("div[data-nodeid='" + onseqSelectID + "']").parent().addClass("k-state-selected");

                if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {
                    $leftTreeDiv.find("div").find("div[data-nodeid='" + onseqSelectID + "']").parent().addClass("k-state-selected")[0].scrollIntoView();
                }

            }




            if ($cmbSearch.val() != "") {
                SearchNodeInTree(chksearchvalues);
            }

            SelectedNodeColor();

            window.setTimeout(function () {
                if ($cmbSearch.val() != "") {
                    SearchNodeInTree(getEvalueforsearch);
                }
            }, 200);
            CreateRestrictionforDelete();
        };






        var gatDenideImg = null;

        var draggingItemsss = null;
        var appenddivforImg = $('<div class="vis-tm-unlinkimgss">');

        var treeTop = 0;

        function onDrag(e) {
            chksearchvalues = e;
            //var eleTop = $(e.sourceNode).offset().top;

            //var eleTop = e.pageY - (topTreeDiv.offset().top);
            //var treeScrollTop = $leftTreeDiv.scrollTop();

            ////////if (!treeTop) {
            ////////    treeTop = $leftTreeDiv.offset().top;
            ////////}
            ////////else {
            ////////    treeTop = treeTop - 50;
            ////////}
            //////////$leftTreeDiv.animate({
            //////////    scrollTop: (treeScrollTop + eleTop) - treeTop
            //////////});

            ////////var $ulllTree = $leftTreeDiv.find('[role="tree"]');
            ////////$leftTreeDiv.css('top', treeTop + 'px');

            // topTreeDiv.scrollTop((treeScrollTop + eleTop) - treeTop);



            //topTreeDiv.scrollTop(treeTop);


            //var y = e.pageY - (topTreeDiv.offset().top + d);
            //topTreeDiv.scrollTop(y);



            //var targetDataItem = $leftTreeDiv.dataItem(e.dropTarget);
            //if (targetDataItem != null || targetDataItem != undefined) {
            //    if (targetDataItem.text == "Root" && e.statusClass == "insert-top") {
            //        e.setStatusClass("k-denied");
            //    }
            //    //  $("#status").html(e.statusClass + "  "+targetDataItem.text );
            //}
            //var y = e.pageY - $leftTreeDiv.offset().top;
            //$leftTreeDiv.scrollTop(y);


            // $leftTreeDiv.css("overflow", "visible");


            //var eleTop = $(e.sourceNode).offset().top;
            //var treeScrollTop = topTreeDiv.scrollTop();
            //var treeTop = topTreeDiv.offset().top;
            //topTreeDiv.animate({
            //    scrollTop: (treeScrollTop + eleTop) - treeTop
            //});




            //$mData.css("overflow", "hidden");
            //$rightDataDiv.css("overflow", "hidden");
            //$ulBackDiv.css("overflow", "hidden");




            //vis-tm-bottomdivdisable 


            $secoundDiv.addClass("vis-tm-bottomdivdisable");



            bottomdivdisable.css("display", "inherit");

            onDisableDiv.css("display", "inherit");

            $dragTreeDataNodeID = $(e.sourceNode).find(".treechild").attr("data-nodeid");
            movingNode = $(e.sourceNode).find(".treechild").parent().parent().parent();
            draggingItemsss = $(e.sourceNode);

            gatDenideImg = this.dragging._draggable.hint.find(".k-denied");

            // $treeRefresh.css("display", "none");

            $($leftTreeDiv.find("ul")[0]).css("width", "95%");
            $deleteArea.addClass("VIS-TM-ondragdrop");

            if ($dragTreeDataNodeID != 0) {
                $deleteImage.css("display", "inherit");
                $deleteArea.css("display", "inherit");
            }

            if ($(e.sourceNode).find(".treechild").attr("data-nodeid") == 0) {
                e.setStatusClass("k-denied");
            }

            if ($(e.dropTarget).parent().parent().attr("data-issummary") == "false") {
                e.setStatusClass("k-denied");
            }

            if ($(e.dropTarget).parent().parent().parent().parent().parent().find(".treechild").attr("data-issummary") == "false") {
                e.setStatusClass("k-denied");
            }

            //if ($(e.dropTarget).find(".treechild").attr("data-issummary") == "false") {
            //    e.setStatusClass("k-denied");
            //}



            if ($(e.dropTarget).hasClass("k-top")) {
                if ($(e.dropTarget).find(".treechild").attr("data-issummary") == "false") {
                    e.setStatusClass("k-denied");
                }
            }

            if ($(e.dropTarget).hasClass("k-item")) {
                e.setStatusClass("k-denied");
            }


            if (this.dragging._draggable.hint.find(".k-add").length == 1) {
                if ($(e.dropTarget).parent().attr("data-issummary") == "false") {
                    e.setStatusClass("k-denied");
                }
            }

            if ($(e.dropTarget).parent().find(".textdivTree").length == 1 && this.dragging._draggable.hint.find(".k-add").length == 1) {
                e.setStatusClass("k-denied");
            }


            //if ($(e.dropTarget).parent().attr("data-issummary") == "false") {
            //    e.setStatusClass("k-denied");
            //}

            if ($(e.dropTarget).attr("aria-expanded") == "true" || $(e.dropTarget).attr("aria-expanded") == "false") {
                e.setStatusClass("k-denied");
            }



            if ($(e.dropTarget).hasClass("selectchangecolor")) {
                gatDenideImg.css("display", "none");
                //                gatDenideImg.parent().addClass("glyphicon glyphicon-trash");

                gatDenideImg.parent().prepend(appenddivforImg);
                gatDenideImg.parent().find(".vis-tm-unlinkimgss").css("display", "inherit")


            }
            else {
                gatDenideImg.css("display", "inherit");
                //gatDenideImg.parent().removeClass("glyphicon glyphicon-trash");

                gatDenideImg.parent().find(".vis-tm-unlinkimgss").css("display", "none")



            }


            if ($(e.dropTarget).hasClass("glyphicon glyphicon-trash")) {
                gatDenideImg.css("display", "none");
                //                gatDenideImg.parent().addClass("glyphicon glyphicon-trash");

                gatDenideImg.parent().prepend(appenddivforImg);
            }



            $downImg = this.dragging._draggable.hint.find("span").hasClass("k-insert-bottom");
            if ($downImg) {
                e.setStatusClass("k-denied");
            }

            if ($(e.dropTarget).hasClass("k-state-hover")) {
                if ($(e.dropTarget).find(".treechild").attr("data-issummary") == "false") {
                    e.setStatusClass("k-denied");
                }
            }

            if ($(e.dropTarget).hasClass("VIS-TM-root-node")) {
                e.setStatusClass("k-denied");
            }


            //$deleteImage.mouseover(function (e) {
            //    gatDenideImg.css("display", "none");
            //    gatDenideImg.parent().addClass("glyphicon glyphicon-trash");
            //});
            //$deleteImage.mouseleave(function () {
            //    gatDenideImg.css("display", "inherit");
            //    gatDenideImg.parent().removeClass("glyphicon glyphicon-trash");
            //});

            //if ($($leftTreeDiv).find(".k-state-focused").find(".treechild").attr("data-issummary") == "false") {
            //    e.setStatusClass("k-denied");
            //}


            //var y = e.pageY - topTreeDiv.offset().top;


            //var y = e.pageY - (topTreeDiv.offset().top + d);
            //topTreeDiv.scrollTop(y);

        };


        var trenodeselectedId = 0;
        function onDrop(e) {


            //if (!e.valid) {
            //    e.preventDefault();
            //    return;
            //}
            trenodeselectedId = 0;
            trenodeselectedId = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");

            chksearchvalues = e;
            $mData.css("overflow", "auto");
            $rightDataDiv.css("overflow", "auto");
            $ulBackDiv.css("overflow", "auto");

            $secoundDiv.removeClass("vis-tm-bottomdivdisable");
            bottomdivdisable.css("display", "none");
            onDisableDiv.css("display", "none");

            if (e.sourceNode.previousSibling == e.destinationNode && e.dropPosition == "after") {
                e.preventDefault();
                $deleteArea.removeClass("VIS-TM-ondragdrop");
                $deleteArea.css("display", "none");
                $deleteImage.css("display", "none");
                return;
            }

            if (e.sourceNode.nextSibling == e.destinationNode && e.dropPosition == "before") {
                e.preventDefault();
                $deleteArea.removeClass("VIS-TM-ondragdrop");
                $deleteArea.css("display", "none");
                $deleteImage.css("display", "none");
                return;
            }

            if (e.dropPosition == "after") {

                if ($(e.dropTarget).parent().attr("data-nodeid") == 0) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    // $treeRefresh.css("display", "inherit");
                    return;
                }

                if ($(e.dropTarget).find(".treechild").attr("data-nodeid") == 0) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    // $treeRefresh.css("display", "inherit");
                    return;
                }

                if ($(e.dropTarget).hasClass("k-minus")) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    //$treeRefresh.css("display", "inherit");
                    return;
                }
                //if ($(e.dropTarget).hasClass("textdivTree")) {
                //    e.preventDefault();
                //    $deleteArea.removeClass("VIS-TM-ondragdrop");
                //    $deleteArea.css("display", "none");
                //    $deleteImage.css("display", "none");
                //    // $treeRefresh.css("display", "inherit");
                //    return;
                //}

            }
            if (e.dropPosition == "before") {

                if ($(e.dropTarget).find(".treechild").attr("data-nodeid") == 0) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    //$treeRefresh.css("display", "inherit");
                    return;
                }

                if ($(e.dropTarget).hasClass("k-minus")) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    //  $treeRefresh.css("display", "inherit");
                    return;
                }

                if ($(e.dropTarget).parent().hasClass("imgdivTree")) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    //  $treeRefresh.css("display", "inherit");
                    return;
                }

                if ($(e.dropTarget).hasClass("VIS-TM-tree-wrap")) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    // $treeRefresh.css("display", "inherit");
                    return;
                }//\\\\\\\\\\\\\
                if ($(e.dropTarget).parent().hasClass("k-bot")) {

                    if ($($($(e.dropTarget).parent()).parent()).parent().attr('role') != 'group') {
                        e.preventDefault();
                        $deleteArea.removeClass("VIS-TM-ondragdrop");
                        $deleteArea.css("display", "none");
                        $deleteImage.css("display", "none");
                        //$treeRefresh.css("display", "inherit");
                        return;
                    }

                }
                if ($(e.dropTarget).parent().hasClass("textdivTree")) {
                    if ($(e.dropTarget).parent().parent().attr("data-nodeid") == 0) {
                        e.preventDefault();
                        $deleteArea.removeClass("VIS-TM-ondragdrop");
                        $deleteArea.css("display", "none");
                        $deleteImage.css("display", "none");
                        //  $treeRefresh.css("display", "inherit");
                        return;
                    }
                }
            }

            if (e.dropPosition == "over") {
                if ($(e.dropTarget).parent().attr("data-issummary") == "false") {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    // $treeRefresh.css("display", "inherit");
                    return;
                }
                if ($(e.dropTarget).find(".treechild").attr("data-nodeid") == 0) {
                    e.preventDefault();
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                    // $treeRefresh.css("display", "inherit");
                    return;
                }
            }


            //  $bsyDiv[0].style.visibility = "visible";

            if ($deleteArea.hasClass("selectchangecolor")) {
                $($(this.dragging._draggable.hint)).animate({
                    left: '250px',
                    opacity: '0.5',
                    height: '2px',
                    width: '2px'
                }, 1000)
            }



            $deleteArea.removeClass("VIS-TM-ondragdrop");
            $deleteArea.css("display", "none");
            $deleteImage.css("display", "none");
            // $treeRefresh.css("display", "inherit");


            if (gatDenideImg.hasClass("k-denied")) {
                if (!$deleteArea.hasClass("selectchangecolor")) {
                    return;
                }
            }


            var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            tree = executeDataSet(tree, null, null);
            treeType = tree.tables[0].rows[0].cells["treetype"];


            if (treeType == "PR") {
                tbname = "AD_TreeNodePR"
            }
            else if (treeType == "BP") {
                tbname = "AD_TreeNodeBP"
            }
            else if (treeType == "MM") {
                tbname = "AD_TreeNodeMM"
            }
            else {
                tbname = "AD_TreeNode"
            }

            var $dropTreeDataNodeIDasParentID = $leftTreeKeno.find(".k-state-hover").find(".treechild").attr("data-nodeid");
            var isSummary = $leftTreeKeno.find(".k-state-hover").find(".treechild").attr("data-issummary");

           

            if (isSummary == "true") {
                SaveTreeDragDrop($treeID, $dragTreeDataNodeID, $dropTreeDataNodeIDasParentID);
                var cNode = $(e.destinationNode);
                $bsyDiv[0].style.visibility = "visible";

                //---------------------------------

                window.setTimeout(function () {
                    $bsyDiv[0].style.visibility = "visible";
                    moveDraggedItemToFirstPalce(cNode);

                    if ($cmbSearch.val() != "") {
                        SearchNodeInTree(e);
                    }
                    SelectedNodeColor();
                    dblSelectedNod();
                    $bsyDiv[0].style.visibility = "hidden";

                }, 100);

                //--------------------


                return;
            }
            else {
                var isSummary = $(e.destinationNode).parent().parent().find(".treechild").attr("data-issummary");

                if (e.sourceNode === e.destinationNode) {
                    e.setValid(false);
                    $bsyDiv[0].style.visibility = "hidden";
                    return;
                };

                dblSelectedNod();
            }

            if (trenodeselectedId > 0) {
                window.setTimeout(function () {

                    $leftTreeDiv.find(".k-state-selected").removeClass("k-state-selected");
                    $($leftTreeDiv.find("div").find("div[data-nodeid='" + trenodeselectedId + "']").parent()).addClass("k-state-selected");
                    SelectedNodeColor();

                }, 200);
            }


            if ($(e.destinationNode).parent().parent().parent().attr("role") == "tree") {

                var cNode = $(e.destinationNode);
                $bsyDiv[0].style.visibility = "visible";
                var droppossitions = e.dropPosition;
                var soursenodes = e.sourceNode;
                var getparentornot = $($(e.sourceNode.parentElement).parent().find(".k-in")[0]).find(".treechild").attr("data-nodeid");

                window.setTimeout(function () {
                    $bsyDiv[0].style.visibility = "visible";
                    var ulss = cNode.parent().children();
                    if (ulss.length > 0) {
                        var sequence = 0;

                        var getseqqry = "SELECT seqno FROM " + tbname + "  WHERE AD_Tree_ID=" + $treeID + " AND node_id=" + cNode.find(".treechild").attr("data-nodeid");
                        var dsss = executeDataSet(getseqqry, null, null);

                        if (dsss.tables[0].rows.length > 0) {
                            sequence = dsss.tables[0].rows[0].cells["seqno"];
                        }


                        if (droppossitions == "after") {
                            sequence = sequence + 1;
                        }





                        var increaseSqe = "update " + tbname + " set seqno=seqno+1,Updated=Sysdate,parent_ID=0 where seqno >=" + sequence +
                                         " AND (parent_id=0 or parent_id is null)  AND AD_Tree_ID=" + $treeID;
                        executeQuery(increaseSqe, null, null);








                        var setmoveSeq = $(soursenodes).find(".treechild").attr("data-nodeid");
                        if (getparentornot > 0) {
                            var movedSetSeq = "update " + tbname + " set seqno=" + sequence + ",parent_id=0  where node_id=" + setmoveSeq +
                                           "   AND AD_Tree_ID=" + $treeID;
                            executeQuery(movedSetSeq, null, null);
                        }
                        else {
                            var movedSetSeq = "update " + tbname + " set seqno=" + sequence + " where node_id=" + setmoveSeq +
                                           " AND (parent_id=0 or parent_id is null) AND AD_Tree_ID=" + $treeID;
                            executeQuery(movedSetSeq, null, null);
                        }





                        //var nodeidfromcnode = cNode.find(".treechild").attr("data-nodeid");
                        //var movednodeid = $(soursenodes).find(".treechild").attr("data-nodeid");
                        //var updatesqn = "UPDATE " + tbname + "  SET Updated=Sysdate, seqNo=" + seqnoafterdrop + " WHERE node_id=" + movednodeid + " AND AD_Tree_ID=" + $treeID;

                        //VIS.DB.executeQuery(updatesqn, null, null);


                        //for (var l = 0; l < ulss.length; l++)
                        //{
                        //    var sql = "UPDATE ";
                        //    sql += tbname + " SET Parent_ID=0, SeqNo=" + l + ", Updated=SysDate" +
                        //                    " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + $(ulss[l]).find(".treechild").attr("data-nodeid");
                        //    VIS.DB.executeQuery(sql, null, null);
                        //}
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                }, 600);
            }
            else {

                // var childs = $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li");

                var $destibation = $(e.destinationNode);

                if ($destibation.length == 0) {

                    delNodId = $(e.sourceNode).find(".treechild").attr("data-nodeid");
                    getMovingdiv = $(e.sourceNode).find(".treechild").parent().parent().parent();

                    if ($deleteArea.hasClass("selectchangecolor")) {
                        DailogForDelete();
                    }
                    else {
                        $deleteArea.removeClass("VIS-TM-ondragdrop");
                        $deleteArea.css("display", "none");
                        $deleteImage.css("display", "none");
                        $bsyDiv[0].style.visibility = "hidden";
                        return;
                    }



                    //var delNodId     = $(e.sourceNode).find(".treechild").attr("data-nodeid");
                    //var getMovingdiv = $(e.sourceNode).find(".treechild").parent().parent().parent();
                    // DeleteNodeFromTree($treeID, delNodId, getMovingdiv);
                    //window.setTimeout(function ()
                    //{
                    //    //if (flagDelete == false)
                    //    //{
                    //    //    if ($deleteResult == "") {
                    //    //        getMovingdiv.remove();
                    //    //        pageNo = 1;
                    //    //        $ulRight.empty();
                    //    //        LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                    //    //    }
                    //    //}
                    //}, 200);
                    return;
                }

                //var pid = $(movingNode.parent().parent().find(".treechild")[0]).attr("data-nodeid");


                var destElement = e.destinationNode.parentElement;
                var sourceElement = e.sourceNode.parentElement;

                var destinationNode = e.destinationNode;
                var SourceNode = e.sourceNode;

                $bsyDiv[0].style.visibility = "visible";
                window.setTimeout(function () {
                    $bsyDiv[0].style.visibility = "visible";

                    var onDropID = $($(SourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li").find(".k-state-hover").find("div").attr("data-nodeid");
                    var childs = $destibation.parent().children();

                    if (destElement == sourceElement) {
                        //var pid = $(movingNode.parent().parent().find(".treechild")[0]).attr("data-nodeid");
                        var pid = $(sourceElement.parentElement).find(".treechild").attr("data-nodeid");

                        if (pid != 0) {
                            for (var i = 0; i < childs.length; i++) {
                                // var nd = $leftTreeKeno.find("div").find("div[data-nodeid='" + pid + "']").parent().parent().parent().find("li")[i];

                                var nd = $(childs[i]).find(".treechild").attr("data-nodeid");

                                var sql = "UPDATE ";
                                sql += tbname + " SET Parent_ID=" + pid + ", SeqNo=" + i + ", Updated=SysDate" +
                                                " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + nd;


                                executeQuery(sql, null, null);
                            }

                        }
                    }
                    else {
                        var pid = $(destinationNode.parentElement).parent().find(".treechild").attr("data-nodeid");
                        if (pid != 0) {
                            for (var i = 0; i < childs.length; i++) {
                                // var nd = $leftTreeKeno.find("div").find("div[data-nodeid='" + pid + "']").parent().parent().parent().find("li")[i];

                                var nd = $(childs[i]).find(".treechild").attr("data-nodeid");

                                var sql = "UPDATE ";
                                sql += tbname + " SET Parent_ID=" + pid + ", SeqNo=" + i + ", Updated=SysDate" +
                                                " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + nd;


                                executeQuery(sql, null, null);
                            }

                        }
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                }, 600);
            }

            window.setTimeout(function () {
                if ($cmbSearch.val() != "") {
                    SearchNodeInTree(e);
                }              
            }, 300);


            //else
            //{
            //    window.setTimeout(function ()
            //    {
            //        for (var i = 0; i < childs.length; i++)
            //        {
            //            var nd = $leftTreeKeno.find("div").find("div[data-nodeid='" + pid + "']").parent().parent().parent().find("li")[i];

            //            var sql = "UPDATE ";
            //            sql += tbname + " SET Parent_ID=" + pid + ", SeqNo=" + i + ", Updated=SysDate" +
            //                            " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + $(nd).find(".treechild").attr("data-nodeid");

            //            VIS.DB.executeQuery(sql, null, null);
            //        }
            //    }, 600);
            //}





            //var childID = $(e.sourceNode).find(".treechild").attr("data-nodeid");
            //var parentID = $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li").find(".k-state-hover").find("div").attr("data-nodeid");
            //var childs = $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li");
            //var getParentIDisZero = $(movingNode.parent().parent().find(".treechild")[0]).attr("data-nodeid");
            //var pid = $(movingNode.parent().parent().find(".treechild")[0]).attr("data-nodeid");
            //var getIsSummaryr = $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li").find(".k-state-hover").find("div").attr("data-issummary");

            //window.setTimeout(function () {
            //    if (getIsSummaryr == "true") { }
            //    {
            //        var nd = $leftTreeKeno.find("div").find("div[data-nodeid='" + pid + "']").parent().parent().parent().find("li")[i];

            //        var sql = "UPDATE ";
            //        sql += tbname + " SET Parent_ID=" + pid + ", SeqNo=" + i + ", Updated=SysDate" +
            //                        " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + $(nd).find(".treechild").attr("data-nodeid");

            //        VIS.DB.executeQuery(sql, null, null);
            //    }


            //    for (var i = 0; i < childs.length; i++) {
            //        var nd = $leftTreeKeno.find("div").find("div[data-nodeid='" + pid + "']").parent().parent().parent().find("li")[i];

            //        var sql = "UPDATE ";
            //        sql += tbname + " SET Parent_ID=" + parentID + ", SeqNo=" + i + ", Updated=SysDate" +
            //                        " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + $(nd).find(".treechild").attr("data-nodeid");

            //        VIS.DB.executeQuery(sql, null, null);
            //    }
            //}, 600);

            ////var oldParent = movingNode.parent().parent().find(".treechild")[0];


            // var movingElementID = $(e.sourceNode).find(".treechild").attr("data-nodeid");
            // var onDropID= $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li").find(".k-state-hover").find("div").attr("data-nodeid");
            // var ndValue = $($(e.sourceNode).find(".treechild").parent().parent().parent().parent().parent()).find("li");




            // $bsyDiv[0].style.visibility = "hidden";

           

        };

        function dblSelectedNod() {
            if ($leftTreeDiv.find(".k-state-selected").length > 1) {
                $leftTreeDiv.find(".k-state-selected").removeClass("k-state-selected");
                $ulMid.empty();
            }
        };


        function moveDraggedItemToFirstPalce(cNode) {
            var currentparent = cNode.parent();

            if (currentparent.attr("role") == "tree") {
                var ulss = cNode.parent().children().children('ul').children()[cNode.parent().children().children('ul').children().length - 1]

                if (ulss && currentparent) {
                    ulss.remove();
                    $($(currentparent.children()[0]).children('ul')).prepend(ulss);
                }

            }
            else {
                var ulss;
                if (cNode.children('ul')) {
                    ulss = cNode.children('ul').children();
                }

                if (ulss) {
                    ulss = $(ulss[ulss.length - 1]);
                }
                if (ulss && ulss.siblings().length > 0) {
                    var firstSibling = $(ulss.siblings()[0]).parent();
                    $leftTreeDiv.data("kendoTreeView").detach(ulss[0]);
                    firstSibling.prepend(ulss[0]);
                }
            }
        };

        function SaveTreeDragDrop($treeID, $dragTreeDataNodeID, $dropTreeDataNodeIDasParentID) {
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/SaveTreeDragDrop",
                type: 'Post',
                data: { treeID: $treeID, NodeID: $dragTreeDataNodeID, ParentID: $dropTreeDataNodeIDasParentID },
                success: function (data) {
                    var res = JSON.parse(data);
                    if (res == VIS.Msg.getMsg("VIS_DataSave")) {

                    }
                    else {
                        // VIS.ADialog.info("VIS_DataNotSaved");
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
        };

        var isulMidData = false;
        function GetDataTreeNodeSelect(getParentID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e) {


            $bsyDivforbottom[0].style.visibility = "visible";

            if (searchChildNode == null) {
                searchChildNode = "";
            }




            //if ($cmbSearch.val() != "") {
            //    $checkSearchOrNot.css("display", "inline-block");
            //} else {
            //    $checkSearchOrNot.css("display", "none");
            //}

            DropMenu();
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/GetDataTreeNodeSelect",
                type: 'Post',
                async: false,
                data: { nodeID: getParentID, treeID: $treeID, pageNo: pageNoForChild, pageLength: pageSizeForChild, searchChildNode: searchChildNode, getTreeNodeChkValue: getTreeNodeChkValue },
                success: function (data) {
                    // $ulMid.sortable();
                    //  sorttableLi();
                    if ($setorderflagss == false) {
                        return;
                    }
                    var res = JSON.parse(data);
                    var summImage = null;
                    var nonSummImage = null;
                    if (res.length > 0) {
                        $bsyDivforbottom[0].style.visibility = "visible";


                        if ($scrollBottom == true) {
                            $ulMid.empty();
                        }
                        isulMidData = true;
                        for (var i = 0; i < res.length; i++) {
                            if ($scrollBottom == false) {
                                if ($ulMid.find("li li").length > 0) {
                                    // var lisItem = $ulMid.find('[id="' + res[i]["parentID"] + '"]');

                                    var lisItem = $ulMid.find('[data-id="' + res[i]["parentID"] + '"]');
                                    if (lisItem && lisItem.length > 0) {
                                        continue;
                                    }

                                }
                            }

                            if (res[i]["parentID"] == 0) {
                                continue;
                            }


                            var checkBox = $('<input class="VIS-tm-checkbox" type="checkbox" />');
                            if (res[i]["issummary"] == "Y") {
                                continue;
                                summImage = "<i class='fa fa-folder-o summNonsumImage' data='Images/folder.png'></i>";
                                //$ulMid.append($('<li class="VIS-tm-topMLi" style="cursor:default;padding:5px 0px">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));
                                //$ulMid.append($('<li class="VIS-tm-topMLi" style="cursor:default;padding:5px 0px">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-NodePID='" + res[i]["NodeParentID"] + "' data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));

                                $ulMid.append($('<li class="VIS-tm-topMLi">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));
                            }
                            else {
                                nonSummImage = "<i class='vis vis-m-window summNonsumImage' data='Images/mWindow.png'></i>";
                                //$ulMid.append($('<li class="VIS-tm-topMLi" style="cursor:default;padding:5px 0px">').append(checkBox).append(nonSummImage).append($("<li class='VIS-tm-MLi' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));
                                //$ulMid.append($('<li class="VIS-tm-topMLi" style="cursor:default;padding:5px 0px">').append(checkBox).append(nonSummImage).append($("<li class='VIS-tm-MLi'  data-NodePID='" + res[i]["NodeParentID"] + "'  data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));

                                $ulMid.append($('<li class="VIS-tm-topMLi">').append(checkBox).append(nonSummImage).append($("<li class='VIS-tm-MLi'    data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));
                            }

                            //var checkBox = $('<input class="VIS-tm-checkbox" style="float:left; margin-right: 10px;margin-left:0" type="checkbox" />');
                            //$ulMid.append($('<li class="VIS-tm-topMLi" style="padding:5px 0px">').append(checkBox).append($("<li class='VIS-tm-MLi' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "' style='cursor:pointer'  >" + res[i]["name"] + "</li>")));
                        }
                        $recordeNotFound.css("display", "none");
                        //$deleteChild.addClass("vis-tm-delete");
                        mListDrag();
                        DropInMidDiv();
                        DropMenu();
                    }
                    else {
                        if ($ulMid.find("li").length > 0) {
                            isulMidData = true;
                            $recordeNotFound.css("display", "none");
                        }
                        else {
                            isulMidData = false;
                            $recordeNotFound.css("display", "inherit");
                        }
                    }
                    SearchNodeInTree(e);

                    // $ulMid.sortable().bind('sortupdate', function () {
                    //UpdateBottomItemSeqe(getParentID, $treeID);
                    // });
                    window.setTimeout(function () {
                        if ($ulMid.find("input:checked").length > 0) {
                            $deleteChild.removeClass("vis-tm-delete");
                        }
                        else {
                            $deleteChild.addClass("vis-tm-delete");
                        }

                    }, 200);
                    AllSelectChkValue();


                    if ($chkAllCheckOrNot.is(":checked")) {
                        $ulMid.find("input").prop("checked", true);
                        $ulMid.find("input").parent().addClass("vis-tm-menuselectedcolor");
                    }

                    if ($ulMid.find("li").length > 0) {
                        isulMidData = true;
                        $recordeNotFound.css("display", "none");
                    }
                    else {
                        isulMidData = false;
                        $recordeNotFound.css("display", "inherit");
                    }

                    $cmbSearch.prop("disabled", false);
                    $btnSearch.prop("disabled", false);
                    $cmbSearch.focus();

                    animateSearchFlag = true;

                    clickontreenode = true;
                    leftstopdiv.css("display", "none");
                    $bsyDiv[0].style.visibility = "hidden";
                    GetCountOnTreeChanges();
                    $bsyDivforbottom[0].style.visibility = "hidden";




                },
                error: function (e) {
                    leftstopdiv.css("display", "none");
                    isulMidData = false;
                    console.log(e);
                    $ulMid.empty();
                    $cmbSearch.prop("disabled", false);
                    $btnSearch.prop("disabled", false);
                    $cmbSearch.focus();
                    clickontreenode = true;
                    $bsyDiv[0].style.visibility = "hidden";
                    $bsyDivforbottom[0].style.visibility = "hidden";
                },
            });
            window.setTimeout(function () {
                if ($ulMid.find("li").length > 0) {
                    isulMidData = true;
                    $recordeNotFound.css("display", "none");
                    $squenceDailog.removeClass("vis-tm-delete");
                    $bsyDivforbottom[0].style.visibility = "hidden";
                }
                else {
                    isulMidData = false;
                    // $recordeNotFound.css("display", "inherit");
                    $squenceDailog.addClass("vis-tm-delete");
                }
            }, 500);
            $mData.height(leftMianDataDiv.height() - ($treeBackDiv.height() + 20 + $mTopHeader.height()));
        };


        function GetIsLIOrNot() {
            if ($ulMid.find("li").length > 0) {
                isulMidData = true;
                //$recordeNotFound.css("display", "none");
            }
            else {
                isulMidData = false;
                //$recordeNotFound.css("display", "inherit");
            }
        };

        var GetPIDforItems = null;
        var sorttingItemsArr = [];
        var inputhelper = null;
        function sorttableLi(e) {
            var helper = null;
            $ulMid.sortable({
                cursorAt: { left: -10, top: -10 },
                helper: function (e, item) {
                    helper = $('<li />');
                    if (!item.hasClass("vis-tm-menuselectedcolor")) {
                        item.addClass('vis-tm-menuselectedcolor').siblings().removeClass('vis-tm-menuselectedcolor');
                    }
                    var elements = item.parent().children('.vis-tm-menuselectedcolor').clone();
                    item.data('multidrag', elements).siblings('.vis-tm-menuselectedcolor').remove();
                    return helper.append(elements);
                },
                start: function (event, ui) {
                    $dragtrue = false;

                    onDisableDiv.css("display", "inherit");
                    $deleteChild.css("display", "none");
                    $ulMid.find("input").prop("checked", false);

                    nodeItemDelwithdrag.height($secoundDiv.height());
                    nodeItemDelwithdrag.css("margin-top", "5px");
                    GetPIDforItems = ui.item.find("li").attr("id");

                    //nodeItemDelwithdrag.addClass("VIS-TM-ondragdrop");                  
                    nodeItemDelwithdrag.css("background-color", "#FFC0CB");
                    nodeItemDelwithdrag.css("display", "inherit");

                    nodeItemDelwithdrag.mouseover(function (e) {
                        nodeItemDelwithdrag.addClass("selectchangecolor");
                        nodeItemDelwithdrag.css("background-color", "#ff0000");
                        //nodeItemDelwithdrag.animate({
                        //    backgroundColor: "#ff0000",
                        //}, 200);
                    });
                    nodeItemDelwithdrag.mouseleave(function () {
                        nodeItemDelwithdrag.removeClass("selectchangecolor");
                        nodeItemDelwithdrag.css("background-color", "#FFC0CB");
                    });

                },
                change: function (event, ui) {
                },
                update: function (event, ui) {
                    nodeItemDelwithdrag.height($secoundDiv.height());
                    //UpdateBottomItemSeqe(getParentID, $treeID);
                },
                stop: function (event, ui) {
                    //$bsyDiv[0].style.visibility = "visible";
                    ui.item.after(ui.item.data('multidrag')).remove();
                    $deleteChild.css("display", "inherit");
                    onDisableDiv.css("display", "none");

                    if ($ulMid.find("li").hasClass("vis-tm-menuselectedcolor")) {
                        $ulMid.find("input").prop("checked", false);
                        $ulMid.find(".vis-tm-menuselectedcolor").find("input").prop("checked", true);
                        $deleteChild.removeClass("vis-tm-delete");
                    }

                    if (nodeItemDelwithdrag.hasClass("selectchangecolor")) {
                        $bsyDiv[0].style.visibility = "visible";
                        TreeTableName();
                        var findchilds = "Select node_id from " + tableTreeName + " where parent_id=" + GetPIDforItems + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
                        var child = executeDataSet(findchilds, null, null);

                        if (child != null && child.tables[0].rows.length == 0) {
                            var selectedItemArray = [];
                            if ($ulMid.find("input:checked").length == 0) {
                                selectedItemArray.push(GetPIDforItems);
                            }

                            for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                                //var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                                var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("data-id");
                                selectedItemArray.push(getID);
                            }

                            bottomchildselectedID = selectedItemArray;
                            selectedItemArray = selectedItemArray.toString();
                            DeleteNodeFromBottom($treeID, selectedItemArray);
                            nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                            nodeItemDelwithdrag.css("display", "none");
                        }
                        else {
                            DailogForItemDelete();
                        }
                        //  $bsyDiv[0].style.visibility = "hidden";
                    }
                    else {
                        UpdateBottomItemSeqe(getParentID, $treeID);
                        nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                        nodeItemDelwithdrag.css("display", "none");
                        $bsyDiv[0].style.visibility = "hidden";
                    }


                },

            });
        };


        function DailogForItemDelete() {
            $bsyDiv[0].style.visibility = "hidden";
            DailogDeleteDesign();
            var createTab = new VIS.ChildDialog();
            //  createTab.setHeight(150);
            // createTab.setWidth(350);
            createTab.setEnableResize(false);
            createTab.setTitle(VIS.Msg.getMsg('UnlinkRecord'));
            createTab.setModal(true);
            createTab.setContent(dailogContainer);
            createTab.show();
            createTab.onClose = function () {

            };
            createTab.onOkClick = function (e) {
                var selectedItemArray = [];
                for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                    // var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                    var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("data-id");
                    selectedItemArray.push(getID);
                }

                if ($ulMid.find("input:checked").length == 0) {
                    selectedItemArray.push(GetPIDforItems);
                }


                bottomchildselectedID = selectedItemArray;
                selectedItemArray = selectedItemArray.toString();
                DeleteNodeFromBottom($treeID, selectedItemArray);

                nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                nodeItemDelwithdrag.css("display", "none");
            };
            createTab.onCancelClick = function () {
                nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                nodeItemDelwithdrag.css("display", "none");
            };
        };


        function UpdateBottomItemSeqe(getParentID, $treeID) {

            $bsyDivforbottom[0].style.visibility = "visible";
            // getTreeTableName();

            var ItemsID = [];
            var ItemsIDTostring = "";
            for (var l = 0; l < $ulMid.find("li li").length; l++) {
                //                var id = $($ulMid.find("li li")[l]).attr("id");
                var id = $($ulMid.find("li li")[l]).attr("data-id");
                ItemsID.push(id);

                //var sql = "UPDATE ";
                //sql += tbnameofTree + " SET SeqNo=" + l + ", Updated=SysDate" +
                //                " WHERE AD_Tree_ID=" + $treeID + " AND Parent_ID=" + getParentID + "   AND Node_ID=" + $($ulMid.find("li li")[l]).attr("id");

                //VIS.DB.executeQuery(sql, null, null);
            }
            ItemsIDTostring = ItemsID.toString();
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/UpdateItemSeqNo",
                type: 'Post',
                data: { treeID: $treeID, itemsid: ItemsIDTostring, ParentID: getParentID },
                success: function (data) {
                    var res = JSON.parse(data);
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "visible";
                },
            });

            $bsyDivforbottom[0].style.visibility = "hidden";
        };

        var tbnameofTree = null;
        function getTreeTableName() {
            var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            table_id = executeDataSet(table_id, null, null);
            if (table_id.tables[0].rows.length > 0) {
                table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            }

            var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
            tablename = executeDataSet(tablename, null, null);
            if (tablename.tables[0].rows.length > 0) {
                tablename = tablename.tables[0].rows[0].cells["tablename"];
            }

            var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            tree = executeDataSet(tree, null, null);
            treeType = tree.tables[0].rows[0].cells["treetype"];


            if (treeType == "PR") {
                tbnameofTree = "AD_TreeNodePR";
            }
            else if (treeType == "BP") {
                tbnameofTree = "AD_TreeNodeBP";
            }
            else if (treeType == "MM") {
                tbnameofTree = "AD_TreeNodeMM";
            }
            else {
                tbnameofTree = "AD_TreeNode"
            }
        };

        var tableTreeName = null;
        function TreeTableName() {
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/TreeTableName",
                async: false,
                data: { treeID: $treeID },
                success: function (data) {
                    var res = JSON.parse(data);
                    tableTreeName = res;
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });

        };

        var mliDragArray = [];
        function mListDrag() {
            $($ulMid.find("li")).draggable({
                //connectToSortable: '.vis-tm-sortable',
                cursorAt: { left: -10, top: -10 },
                helper: function () {
                    var selected = $($ulMid.find("li")).find("input:checked").parent();
                    if (selected.length === 0) {
                        selected = $(this);
                    }
                    var container = $('<div/>').attr('id', 'draggingContainer');

                    if ($($ulMid.find("li")).find("input:checked").length == 0) {
                        if ($(this).hasClass("VIS-tm-topMLi")) {
                            return;
                        }
                        selected = $(this).parent();
                        container = $('<div style="background-color:lightblue;"/>').attr('id', 'draggingContainer');
                    }

                    container.append(selected.clone());
                    getIDFromContainer = container;
                    getIDFromContainer.css("width", "250px");
                    getIDFromContainer.css({ "z-index": 2 });
                    getIDFromContainer.css({ "max-height": "250px", "overflow": "auto" });
                    getIDFromContainer.css({ "overflow": "hidden" });
                    return container;
                },
                start: function (event, ui) {

                    $dragMenuNodeID = $(this).attr("id");
                    $checkMorRdragable = true;
                    dragMenuNodeIDArray = [];
                    mliDragArray = [];
                    for (var j = 0; j < $($(getIDFromContainer.find("li li"))).length; j++) {
                        var IDPush = $($(getIDFromContainer.find("li li"))[j]).attr("id");
                        //if (bindornot == "false") {
                        //    if ($.inArray(parseInt(IDPush), convertmenuArray) >= 0) {
                        //        continue;
                        //    }
                        //}
                        mliDragArray.push(parseInt(IDPush));
                        dragMenuNodeIDArray.push(IDPush);
                    }
                    DropMenu();
                    $midChildDrag = false;
                    isdrag = true;
                    middivDragFlag = false;
                    OnHoverTree();

                    onDisableDiv.css("display", "inherit");


                    $deleteChild.css("display", "none");
                    nodeItemDelwithdrag.height($secoundDiv.height());
                    nodeItemDelwithdrag.css("margin-top", "5px");
                    nodeItemDelwithdrag.css("background-color", "#FFC0CB");
                    nodeItemDelwithdrag.css("display", "inherit");
                    nodeItemDelwithdrag.mouseover(function (e) {
                        nodeItemDelwithdrag.addClass("selectchangecolor");
                        nodeItemDelwithdrag.css("background-color", "#ff0000");
                    });
                    nodeItemDelwithdrag.mouseleave(function () {
                        nodeItemDelwithdrag.removeClass("selectchangecolor");
                        nodeItemDelwithdrag.css("background-color", "#FFC0CB");
                    });

                    //$secoundDiv.css("width", "94%");

                    var setwidth = leftMianDataDiv.width() - 35;
                    $secoundDiv.width(setwidth);

                    $squenceDailog.css("display", "none");
                    //$rightDataDiv.css("overflow", "hidden");
                    $mData.css("overflow", "hidden");

                },
                drag: function (event, ui) {
                },
                stop: function () {
                    isdrag = false;
                    //$secoundDiv.css("width", "100%");
                    //$mData.css("overflow", "auto");
                    ////$rightDataDiv.css("overflow", "auto");

                    //onDisableDiv.css("display", "none");
                    //$squenceDailog.css("display", "inherit");

                    if (nodeItemDelwithdrag.hasClass("selectchangecolor")) {
                        $bsyDiv[0].style.visibility = "visible";
                        TreeTableName();
                        var findchilds = "Select node_id from " + tableTreeName + " where parent_id=" + GetPIDforItems + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
                        var child = executeDataSet(findchilds, null, null);

                        if (child != null && child.tables[0].rows.length == 0) {
                            var selectedItemArray = [];
                            //if ($ulMid.find("input:checked").length == 0) {
                            //    selectedItemArray.push(GetPIDforItems);
                            //}

                            //for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                            //    //var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                            //    var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("data-id");
                            //    selectedItemArray.push(getID);
                            //}

                            //bottomchildselectedID = selectedItemArray;
                            //selectedItemArray = selectedItemArray.toString();
                            if (dragMenuNodeIDArray.length > 0) {
                                if (bindornot == "false") {
                                    var ides = [];
                                    for (var q = 0; q < mliDragArray.length; q++) {
                                        if ($.inArray(mliDragArray[q], convertmenuArray) >= 0) {
                                            continue;
                                        }
                                        ides.push(mliDragArray[q]);
                                    }

                                    if (ides.length > 0) {
                                        DeleteNodeFromBottom($treeID, ides.toString());
                                    }
                                    else {
                                        // VIS.ADialog.info("NeverDelete");
                                        VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);
                                        $bsyDiv[0].style.visibility = "hidden";

                                    }


                                }
                                else {
                                    DeleteNodeFromBottom($treeID, dragMenuNodeIDArray.toString());
                                }
                            }
                            else {
                                //VIS.ADialog.info("NeverDelete");
                                VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);
                                $bsyDiv[0].style.visibility = "hidden";

                            }

                            //DeleteNodeFromBottom($treeID, selectedItemArray);
                            nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                            nodeItemDelwithdrag.css("display", "none");
                            $deleteChild.css("display", "inherit");
                        }
                        else {
                            DailogForItemDelete();
                        }
                        //  $bsyDiv[0].style.visibility = "hidden";
                    }

                    nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                    nodeItemDelwithdrag.css("display", "none");
                    $deleteChild.css("display", "inherit");

                    $secoundDiv.css("width", "100%");
                    $mData.css("overflow", "auto");
                    //$rightDataDiv.css("overflow", "auto");

                    onDisableDiv.css("display", "none");
                    $squenceDailog.css("display", "inherit");

                }
            });
        };

        function CreateTemplateTree() {
            TemplateTree = "<div>" +

                                "<div style='float:left;margin-top:3px'>" +
                                    "<img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='height:20px' >" +
                                 "</div>" +

                                "<div style='float:left;margin:3px 10px 0 10px'>" +
                                     "<p>#= item.text #</p>" +
                                "</div>" +

                            "</div>"
        };

        function LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu) {
            //$bsyDiv[0].style.visibility = "visible";

            if ($ulRight.find("li").length == 0) {
                $pathInfo.empty();
            }

            if (onscrollCheck != false) {
                $ulRight.empty();
            }

            $bsyDivMenu[0].style.visibility = "visible";
            searchRightext = VIS.Utility.encodeText(searchRightext);

            if (searchRightext == null) {
                searchRightext = "";
            }

            leftstopdiv.css("display", "inherit");
            onDisableDiv.css("display", "inherit");

            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/LoadMenuData",
                type: 'Post',
                data: { pageLength: pageLength, pageNo: pageNo, treeID: $treeID, searchtext: searchRightext, demandsMenu: $demandsMenu },
                success: function (data) {
                    var res = JSON.parse(data);
                    var summImage;
                    var nonSummImage;
                    var checkBox;
                    var unlink = $("<span></span>");
                    if (res) {
                        for (var i = 0; i < res.length; i++) {
                            if ($ulRight.find("li li").length > 0) {

                                var lisItem = $ulRight.find('[data-id="' + res[i]["ID"] + '"]');
                                if (lisItem && lisItem.length > 0) {
                                    continue;
                                }

                            }


                            if (res[i]["ID"] == 0) {
                                continue;
                            }

                            


                            if (res[i]["issummary"] == 'Y') {
                                if (res[i]["disabled"] == true) {
                                    checkBox = $('<input class="vis-tm-chkbox" disabled type="checkbox" />');
                                    summImage = "<i class='" + res[i]["classopacity"] + " fa fa-folder-o summNonsumImage' data-imgsrc='Images/summary.png'></i>";
                                }
                                else {
                                    checkBox = $('<input  class="vis-tm-chkbox" type="checkbox" />');
                                    summImage = "<i class='vis vis-m-window summNonsumImage' data-imgsrc='Images/folder.png'></i>";
                                }


                                $ulRight.append($('<li class="vis-tm-upperLi">').append($("<div class='vis-tm-upperdivchkandimg'>").append(checkBox).append(summImage)).append($("<div class='vis-tm-upperdivli'>").append($("<li class='" + res[i]["classopacity"] + " vis-tm-textli ' checkSummary='checkSummary'  style='cursor:default;' id='" + res[i]["ID"] + "' data-id='" + res[i]["ID"] + "'  style='cursor:pointer'  >" + VIS.Utility.encodeText(res[i].Name) + " (" + VIS.Utility.encodeText(res[i].description) + ")" + "</li>").append($("<span title='" + VIS.Msg.getMsg("UnlinkNode") + "' class='" + res[i]["unlinkClass"] + "'></span>")))));

                            }
                            else {
                                if (res[i]["disabled"] == true) {
                                    checkBox = $('<input  class="vis-tm-chkbox"  disabled type="checkbox" />');
                                    nonSummImage = "<i class='" + res[i]["classopacity"] + " vis vis-m-window summNonsumImage' data-imgsrc='Images/nonSummary.png'></i>";
                                }
                                else {
                                    checkBox = $('<input  class="vis-tm-chkbox" type="checkbox" />');
                                    nonSummImage = "<i class='vis vis-m-window summNonsumImage' data-imgsrc='Images/mWindow.png'></i>";
                                }


                                $ulRight.append($('<li class="vis-tm-upperLi">').append($("<div class='vis-tm-upperdivchkandimg'>").append(checkBox).append(nonSummImage)).append($("<div class='vis-tm-upperdivli'>").append($("<li class='" + res[i]["classopacity"] + " vis-tm-textli ' style='cursor:default;' id='" + res[i]["ID"] + "' data-id='" + res[i]["ID"] + "'   style='cursor:pointer'  >" + VIS.Utility.encodeText(res[i].Name) + " (" + VIS.Utility.encodeText(res[i].description) + ")" + "</li>").append($("<span title='" + VIS.Msg.getMsg("UnlinkNode") + "' class='" + res[i]["unlinkClass"] + "'></span>")))));

                            }
                        }
                        if ($ulRight.find("li").length > 0) {
                            $recordeNotFoundRight.css("display", "none");
                        }
                        else {
                            $recordeNotFoundRight.css("display", "inherit");
                        }
                    }
                    else {
                        if ($ulRight.find("li").length > 0) {
                            $recordeNotFoundRight.css("display", "none");
                        }
                        else {
                            $recordeNotFoundRight.css("display", "inherit");
                        }
                    }
                    DragMenu();
                    SelectAllRightPanel();

                    $rightMenuDemand.prop("disabled", false);
                    leftstopdiv.css("display", "none");
                    onDisableDiv.css("display", "none");
                    treeCmbDisable.css("display", "none");
                    $bsyDivMenu[0].style.visibility = "hidden";




                    //$('a.taphover').on('touchstart', function (e) {
                    //    'use strict'; //satisfy the code inspectors
                    //    var link = $(this); //preselect the link
                    //    if (link.hasClass('hover')) {
                    //        return true;
                    //    } else {
                    //        link.addClass('hover');
                    //        $('a.taphover').not(this).removeClass('hover');
                    //        e.preventDefault();
                    //        return false; //extra, and to make sure the function has consistent return points
                    //    }
                    //});


                    var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
                    if (ismobile) {

                        //$ulRight.bind('touchstart', function () {                            
                        $($ulRight.find("li")).draggable("destroy");
                        //});
                        //$ulRight.bind('touchend', function () {                            
                        //    DragMenu();                            
                        //});

                    }






                    //var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());


                    //if (onscrollCheck != false) {
                    //    GetCountForRestriction();
                    //}


                },
                error: function (e) {
                    console.log(e);
                    leftstopdiv.css("display", "none");
                    onDisableDiv.css("display", "none");
                    $rightMenuDemand.prop("disabled", false);
                    treeCmbDisable.css("display", "none");
                    $bsyDiv[0].style.visibility = "hidden";
                    $bsyDivMenu[0].style.visibility = "hidden";
                },
            });

        };
        var isdrag = false;
        var containerdataflag = null;
        function DragMenu(e) {
            chksearchvalues = e;
            // initTouchHandler(e);

            //var dragitemprovide = null;

            //if (ismobile)
            //{
            //    dragitemprovide = $($ulRight.find("input:checked")).parent().parent();
            //}
            //else
            //{
            //    dragitemprovide = $($ulRight.find("li"));
            //}


            //dragitemprovide.draggable({
            $($ulRight.find("li")).draggable({

                //$($ulRight.find(".divfordrags")).draggable({
                // stack: ".vis-tm-upperLi",
                //z-index:100,
                cursorAt: { left: -10, top: -10 },
                helper: function () {
                    getIDFromContainer = [];
                    var selected = $($ulRight.find("input:checked")).parent().parent();



                    if (selected.length === 0) {
                        selected = $(this);
                    }
                    var container = $('<div/>');

                    if ($($ulRight.find("input:checked")).parent().parent().length == 0) {
                        if ($(this).hasClass("vis-tm-textli")) {
                            if ($(this).parent().parent().find("input").attr("disabled") == "disabled") {
                                window.setTimeout(function () {
                                    //  return false;
                                }, 200);
                            }
                            else {
                                selected = $(this).parent().parent();
                                container = $('<div style="background-color:lightblue;"/>').attr('id', 'draggingContainer');
                            }
                        }
                        else if ($(this).hasClass("vis-tm-upperLi")) {
                            if ($(this).find("input").attr("disabled") == "disabled") {
                                // return false;
                            }
                            container = $('<div style="background-color:lightblue;"/>').attr('id', 'draggingContainer');
                        }
                    }
                    else {
                        if ($(this).hasClass("vis-tm-textli")) {
                            if ($(this).parent().parent().find("input").attr("disabled") == "disabled") {
                                //return false;
                            }
                            container = $('<div style="background-color:lightblue;"/>').attr('id', 'draggingContainer');
                        }
                        else if ($(this).hasClass("vis-tm-upperLi")) {
                            if ($(this).find("input").attr("disabled") == "disabled") {
                                // return false;
                            }
                            container = $('<div/>').attr('id', 'draggingContainer');
                        }
                    }

                    if ($(this).parent().parent().find("input").attr("disabled") != "disabled" && $(this).parent().parent().find("input").attr("disabled") != undefined) {
                        container.append(selected.clone());
                    }

                    if (($($ulRight.find("input:checked")).length > 0 || selected.find("input").attr("disabled") == undefined) && !selected.hasClass("vis-tm-opacity")) {
                        container.append(selected.clone());
                    }
                    getIDFromContainer = container;
                    getIDFromContainer.css({ "width": "350px" });

                    getIDFromContainer.css({ "z-index": 2 });

                    if (getIDFromContainer.find(".vis-tm-upperLi").length > 5) {
                        getIDFromContainer.css({ "max-height": "250px", "overflow": "auto" });
                        getIDFromContainer.css({ "overflow": "hidden" });

                    }
                    containerdataflag = container;
                    return container;
                },
                //scroll: 'true',
                // revert: false,
                start: function (event, ui) {

                    onDisableDiv.css("display", "inherit");

                    if (containerdataflag.find("li").length > 0) {
                        mMouseRestrict.css("display", "inherit");
                    }


                    $midChildDrag = true;

                    if (containerdataflag.find(".vis-tm-textli").hasClass("vis-tm-textli")) {
                        isdrag = true;
                    }

                    //isdrag = true;
                    $dragMenuNodeID = $(this).attr("id");
                    $checkMorRdragable = false;
                    $dragtrue = true;
                    dragMenuNodeIDArray = [];
                    ExistItem = [];
                    middivDragFlag = true;

                    //getIDFromContainer.find(".vis-tm-upperLi").length
                    //for (var j = 0; j < $($(getIDFromContainer.find("li li"))).length; j++) {
                    if (getIDFromContainer.find(".vis-tm-upperLi").length == 0) {
                        var IDPush = getIDFromContainer.find("li").attr("id");
                        dragMenuNodeIDArray.push(IDPush);


                    }
                    else {
                        var upperLi = getIDFromContainer.find(".vis-tm-upperLi");



                        for (var j = 0; j < upperLi.length; j++) {
                            var IDPush = $(upperLi.find("li")[j]).attr("id")
                            dragMenuNodeIDArray.push(IDPush);

                            var imgSrc = $(upperLi[j]).find("i").data("imgsrc");
                            var finalImg = imgSrc.substr(imgSrc.lastIndexOf("/") + 1);


                            if (finalImg == "mWindow.png" || finalImg == "folder.png") {
                                ExistItem.push("new");
                            }
                            else {
                                ExistItem.push("update");
                            }

                            //if ($(upperLi[j]).find(".vis-tm-opacity").length == 0) {
                            //    ExistItem.push("new");
                            //}
                            //else {
                            //    ExistItem.push("update");
                            //}


                        }
                    }
                    DropMenu();
                    DropInMidDiv(e);
                    OnHoverTree();

                },
                //containment: $root,
                drag: function (event, ui) {
                },
                stop: function () {
                    dragMenuNodeIDArray = [];
                    isdrag = false;
                    onDisableDiv.css("display", "none");
                    mMouseRestrict.css("display", "none");


                    //var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
                    //if (ismobile)
                    //{

                    //        containerdataflag.empty();

                    //    $($ulRight.find("li")).draggable("destroy");
                    //}

                }
            });

            //$ulRight.on("drop", function (e) {
            //    alert('s');
            //});


            //$($ulRight.find("li")).draggable({
            //    //zIndex: 2,
            //    revert: "invalid",
            //    helper: "clone",
            //    //containment: $root.parent,
            //    start: function (event, ui) {
            //        $dragMenuNodeID = $(this).attr("id");
            //    },
            //    drag: function (event, ui) {
            //    },
            //    stop: function () {
            //    }
            //});
        };

        var validationValue = null;

        function DropMenu(e) {

            // if (dragMenuNodeIDArray.length > 1)
            // {
            // $leftTreeDiv.find(".k-in").parent().parent().droppable({
            //                $leftTreeDiv.droppable({
            // $leftTreeDiv.find(".k-in").droppable({treechild
            //$leftTreeDiv.find(".k-in").droppable({

            //$treeBackDiv.droppable({

            leftMianDataDiv.droppable({

                drop: function (event, ui) {

                    var checkValidation = true;


                    if (mouseEnter == false) {
                        checkValidation = false;
                        //return;
                    }
                    //                    $bsyDiv[0].style.visibility = "visible";
                    // if ($checkMorRdragable != true)
                    // {
                    //$bsyDiv[0].style.visibility = "visible";
                    if ($(ui.draggable).hasClass("ui-dialog")) {
                        $bsyDiv[0].style.visibility = "hidden";
                        checkValidation = false;
                        //return;
                    }

                    if (!$leftTreeDiv.find("span").hasClass("k-state-hover") && !$leftTreeDiv.find("span").hasClass("k-state-selected")) {
                        $bsyDiv[0].style.visibility = "hidden";
                        checkValidation = false;
                        // return;
                    }

                    $dragtrue = false;

                    var issummary, SummaryID, nodid;
                    if ($leftTreeDiv.find("span").hasClass("k-state-hover")) {
                        $getLifromTree = $($leftTreeDiv.find("li").find("span")).parent().find(".k-state-hover").parent().parent();
                    }
                    else if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {
                        $getLifromTree = $($leftTreeDiv.find("li").find("span")).parent().find(".k-state-selected").parent().parent();
                    }


                    if ($leftTreeDiv.find("span").hasClass("k-state-hover")) {
                        var hover = $leftTreeKeno.find(".k-state-hover").find(".treechild");
                        $dropableItem = hover;

                        issummary = hover.attr("data-issummary");
                        SummaryID = hover.attr("data-nodeid");
                        nodid = hover.attr("data-parentid");
                    }
                    else if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {
                        //if ($midChildDrag == false) {
                        //    $bsyDiv[0].style.visibility = "hidden";
                        //    return;
                        //}
                        var selected = $leftTreeDiv.find(".k-state-selected").find(".treechild");

                        selected.parent().css("border", "none");

                        $dropableItem = selected;
                        SummaryID = selected.attr("data-nodeid");
                        issummary = selected.attr("data-issummary");
                        nodid = selected.attr("data-parentid");
                    }

                    var getAllChildNodeID = [];
                    if (issummary == "true") {
                        //var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
                        //table_id = VIS.DB.executeDataSet(table_id, null, null);
                        //if (table_id.tables[0].rows.length > 0) {
                        //    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
                        //}

                        //var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
                        //tablename = VIS.DB.executeDataSet(tablename, null, null);
                        //if (tablename.tables[0].rows.length > 0) {
                        //    tablename = tablename.tables[0].rows[0].cells["tablename"];
                        //}

                        //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                        //tree = VIS.DB.executeDataSet(tree, null, null);
                        //treeType = tree.tables[0].rows[0].cells["treetype"];
                        //var tbname = "";

                        //if (treeType == "PR") {
                        //    tbname = "ad_treenodepr"
                        //}
                        //else if (treeType == "BP") {
                        //    tbname = "ad_treenodebp"
                        //}
                        //else if (treeType == "MM") {
                        //    tbname = "ad_treenodemm"
                        //}
                        //else {
                        //    tbname = "ad_treenode"
                        //}
                        //var sqlqryval = null;

                        //TreeTableName();

                        //var sqlqry = "SELECT " + tableTreeName + ".node_id AS node_id FROM " + tablename + " mp" +
                        //             " INNER JOIN " + tableTreeName + " " + tableTreeName + " ON mp." + tablename + "_ID=" + tableTreeName + ".node_id" +
                        //             "  WHERE " + tableTreeName + ".ad_tree_id=" + $treeID + " AND " + tableTreeName + ".parent_id=" + SummaryID;

                        //sqlqryval = VIS.DB.executeDataSet(sqlqry, null, null);


                        ////var sqlGetParent = "SELECT parent_id FROM " + tableTreeName + " WHERE node_id=" + SummaryID + " AND ad_tree_id=" + $treeID;
                        //var sqlGetParent = "SELECT parent_id FROM " + tableTreeName + " WHERE node_id=" + SummaryID + " AND ad_tree_id=" + $treeID;

                        //var sqlExecute = VIS.DB.executeDataSet(sqlGetParent, null, null);
                        //if (sqlExecute.tables[0].rows.length > 0)
                        //{
                        //    sqlExecuteParentID = sqlExecute.tables[0].rows[0].cells["parent_id"];                            
                        //    if (sqlExecuteParentID == dragMenuNodeIDArray[0])
                        //    {
                        //        $bsyDiv[0].style.visibility = "hidden";
                        //        return;
                        //    }
                        //}

                        //if (SummaryID == dragMenuNodeIDArray[0])
                        //{
                        //    $bsyDiv[0].style.visibility = "hidden";
                        //    return;
                        //}



                        //for (var i = 0; i < sqlqryval.tables[0].rows.length; i++)
                        //{
                        //    sqlqryNodeID = sqlqryval.tables[0].rows[i].cells["node_id"];
                        //    getAllChildNodeID.push(sqlqryNodeID);
                        //}
                        //var indexget = null;
                        // for (var i = 0; i < dragMenuNodeIDArray.length; i++)
                        // {
                        //if (getAllChildNodeID != 0)
                        //{
                        //    for (var j = 0; j < getAllChildNodeID.length; j++)
                        //    {
                        //        var index = dragMenuNodeIDArray.indexOf(getAllChildNodeID[j].toString());

                        //        if (index > -1)
                        //        {
                        //            position = $.inArray(getAllChildNodeID[j].toString(), dragMenuNodeIDArray);
                        //            if (~position) dragMenuNodeIDArray.splice(position, 1);

                        //            ExistItem.splice(position, 1);
                        //        }
                        //    }
                        //}

                        //if (dragMenuNodeIDArray[i] == SummaryID)
                        //{
                        //    var index = dragMenuNodeIDArray.indexOf(SummaryID);

                        //    if (index > -1)
                        //    {
                        //        position = $.inArray(SummaryID, dragMenuNodeIDArray);
                        //        if (~position) dragMenuNodeIDArray.splice(position, 1);

                        //        ExistItem.splice(position, 1);

                        //    }
                        //}
                        // }


                        if (middivDragFlag == false && $dropableItem.parent().hasClass("k-state-selected")) {
                            checkValidation = false;
                            //   return;
                        }

                        validationValue = checkValidation;
                        if (checkValidation == true) {
                            validationValue = checkValidation;
                            SaveDataOnDrop(SummaryID, nodid, dragMenuNodeIDArray, $checkMorRdragable, ExistItem);

                        }
                        else if (checkValidation == false) {
                            $dragtrue = true;
                            $bsyDiv[0].style.visibility = "hidden";
                        }


                    }
                    else {
                        if (issummary == "false" && !$chkSummaryLevel.is(":checked")) {
                            SummaryIDs = $($getLifromTree.parent().parent().find(".treechild")[0]).attr("data-nodeid");

                            $dropableItem = $($getLifromTree.parent().parent().find(".treechild")[0]);

                            SaveDataOnDrop(SummaryIDs, nodid, dragMenuNodeIDArray, $checkMorRdragable, ExistItem);
                        }
                    }





                    // }
                    //  $bsyDiv[0].style.visibility = "hidden";
                }
            });

            //if (validationValue == true) {
            //    window.setTimeout(function () {

            //        if ($cmbSearch.val() != "") {
            //            SearchNodeInTree(e);
            //        }
            //    }, 300);
            //}


            // }
            //else {
            //    $leftTreeDiv.find(".k-in").droppable({
            //        drop: function (event, ui) {
            //            var issummary, SummaryID, nodid;
            //            if ($leftTreeDiv.find("span").hasClass("k-state-hover")) {
            //                $getLifromTree = $($leftTreeDiv.find("li").find("span")).parent().find(".k-state-hover").parent().parent();
            //            }
            //            else if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {
            //                $getLifromTree = $($leftTreeDiv.find("li").find("span")).parent().find(".k-state-selected").parent().parent();
            //            }
            //            //                        if (($(this).find(".k-state-selected").length != 1))
            //            if ($leftTreeDiv.find("span").hasClass("k-state-hover")) {
            //                var hover = $leftTreeKeno.find(".k-state-hover").find(".treechild");
            //                issummary = hover.attr("data-issummary");
            //                SummaryID = hover.attr("data-nodeid");
            //                nodid = hover.attr("data-parentid");
            //            }
            //            else {
            //                var selected = $leftTreeDiv.find(".k-state-selected").find(".treechild");
            //                SummaryID = selected.attr("data-nodeid");
            //                issummary = selected.attr("data-issummary");
            //                nodid = selected.attr("data-parentid");
            //            }

            //            if (issummary == "true") {
            //                SaveDataOnDrop(SummaryID, nodid, dragMenuNodeIDArray, $checkMorRdragable);
            //            }
            //        }
            //    });
            //}
        };

        var selectClass = true;
        function OnHoverTree(e) {

            $leftTreeDiv.data("kendoTreeView").wrapper.find(".k-in").on({
                mouseenter: function () {
                    if (isdrag == true) {
                        mouseEnter = true;
                        if ($(this).hasClass("k-state-selected")) {
                            //$(this).css("border", "2px solid #1aa0ed");

                            //if ($(this).find(".treechild").attr("data-issummary") == "true") {
                            $(this).css("border", "2px solid #bac0cc");
                            // }

                            //selectClass = false;
                            //$(this).removeClass("k-state-selected");
                            //$(this).removeClass("k-state-hover");
                            //$(this).addClass("vis-tm-onhoverselected");                           
                        }
                        else {
                            //  selectClass = true;
                        }
                    }
                },
                mouseleave: function () {
                    if (isdrag == true) {

                        if ($(this).hasClass("k-state-selected")) {
                            $(this).css("border", "none");
                        }

                        //if ($(this).hasClass("vis-tm-onhoverselected")) {
                        //    var sel = $(this);
                        //    sel.addClass("k-state-selected");
                        //    sel.removeClass("k-state-hover");
                        //    //    sel.removeClass("k-state-hover");
                        //}

                    }


                    ////Importanat...
                    mouseEnter = false;
                }
            });


            //$leftTreeDiv.data("kendoTreeView").wrapper.find(".treechild").parent().on({
            //    mouseenter: function () {
            //        if ($(this).find(".treechild").attr("data-nodeid") == dragMenuNodeIDArray[0]) {
            //          //  $(this).css({ 'cursor': 'not-allowed' });
            //        }
            //    },
            //    mouseleave: function () {
            //       // $(this).css({ 'cursor': 'auto' });
            //    }
            //});
        };

        var middivDragFlag = true;
        function DropInMidDiv(e) {
            //$ulMid.find("li").droppable({
            //chksearchvalues = e;
            $ulMid.parent().droppable({
                drop: function (event, ui) {
                    var midlistdrag = true;
                    if ($dragtrue == false) {
                        midlistdrag = false;
                        //return;
                    }

                    if ($(ui.draggable).hasClass("ui-dialog")) {
                        $bsyDiv[0].style.visibility = "hidden";
                        midlistdrag = false;
                    }

                    if (midlistdrag == true) {
                        if (!$checkMorRdragable) {
                            if ($leftTreeDiv.find(".k-state-selected").length == 1) {


                                //var upperLi = $(getIDFromContainer.find(".vis-tm-upperLi").find("li"));
                                //for (var j = 0; j < upperLi.length; j++)
                                //{
                                //    var checkSummary= $(getIDFromContainer.find(".vis-tm-upperLi").find("li")[j]).attr("checkSummary");
                                //    if (checkSummary == "checkSummary")
                                //    {
                                //        VIS.ADialog.info("VIS_SummaryCanNotDropOverHere");
                                //        return;
                                //    }
                                //}



                                var selected = $leftTreeDiv.find(".k-state-selected").find(".treechild");
                                $dropableItem = selected;
                                var SummaryID = selected.attr("data-nodeid");
                                var issummary = selected.attr("data-issummary");
                                var nodid = selected.attr("data-parentid")
                                if (issummary == "true") {
                                    if (containerdataflag.find(".vis-tm-textli").hasClass("vis-tm-textli")) {
                                        $bsyDiv[0].style.visibility = "visible";
                                        SaveDataOnDrop(SummaryID, nodid, dragMenuNodeIDArray, $checkMorRdragable, ExistItem);

                                    }
                                };
                            }
                            else {
                                if (containerdataflag.find(".vis-tm-textli").hasClass("vis-tm-textli")) {
                                    VIS.ADialog.info("SelectAnySummaryTreeNode");
                                }
                            }
                        }
                    }
                    if ($leftTreeDiv.find(".k-state-selected").length == 0) {
                        if (containerdataflag.find(".vis-tm-textli").hasClass("vis-tm-textli")) {
                            VIS.ADialog.info("SelectAnySummaryTreeNode");
                        }
                    }
                },
                tolerance: 'pointer'
            });
        };
        var removefromtree = null;
        function SaveDataOnDrop(SummaryID, nodid, dragMenuNodeIDArray, $checkMorRdragable, ExistItem) {

            var dragNodeIDArray = dragMenuNodeIDArray.toString();
            if (dragNodeIDArray == "") {
                return;
            }

            $bsyDiv[0].style.visibility = "visible";
            $pathInfo.empty();
            ExistItem = ExistItem.toString();

            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/SaveDataOnDrop",
                type: 'Post',
                data: { summaryid: SummaryID, nodid: nodid, treeID: $treeID, dragMenuNodeID: dragNodeIDArray, checkMorRdragable: $checkMorRdragable, IsExistItem: ExistItem },
                success: function (data) {
                    var getRunFunction = true;
                    var res = JSON.parse(data);
                    if ($checkMorRdragable) {
                        for (var j = 0; j < getIDFromContainer.find("li").find(".VIS-tm-MLi").length; j++) {
                            if ($leftTreeDiv.find("div").find("div[data-nodeid='" + SummaryID + "']").parent().hasClass("k-state-selected")) {
                                if (middivDragFlag == false) {
                                    continue;
                                }
                            }
                            var getmidDragID = $(getIDFromContainer.find("li").find(".VIS-tm-MLi")[j]).attr("id");
                            //$($ulMid.find("li").find("li[id='" + getmidDragID + "']").parent()).remove();
                            $($ulMid.find("li").find("li[data-id='" + getmidDragID + "']").parent()).remove();
                            // TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                        }
                    }

                    if (res == VIS.Msg.getMsg("VIS_DataSave")) {
                        var menuselectedcolor = $ulRight.find(".vis-tm-menuselectedcolor");
                        //menuselectedcolor.removeClass("vis-tm-menuselectedcolor");
                        var getIDfromContainer = getIDFromContainer.find("li .vis-tm-textli").attr("id");


                        if ($leftTreeDiv.find("div").find("div[data-nodeid='" + SummaryID + "']").parent().hasClass("k-state-selected")) {
                            //for (var y = 0; y < getIDFromContainer.find("li li").length ; y++) {

                            for (var y = getIDFromContainer.find("li li").length - 1; y >= 0 ; y--) {
                                var getmidDragID = getIDFromContainer.find("li li").attr("id");


                                if (middivDragFlag == false) {
                                    continue;
                                }

                                //if (getmidDragID == $dropableItem.attr("data-nodeid")) {
                                //    continue;
                                //}

                                //var childOfParentofDroppable = $dropableItem.parent().parent().parent().children().find("div[data-nodeid='" + getmidDragID + "']");
                                //if (childOfParentofDroppable && childOfParentofDroppable.length > 0) {
                                //    continue;
                                //}

                                //if ($dropableItem.data("nodeid") == getIDFromContainer.find("li li").data('nodepid')) {
                                //    continue;
                                //}



                                var inputchklen = $ulMid.find("input:checked").length;;
                                var checkinput = $ulMid.find("input").length;

                                if (inputchklen == checkinput) {
                                    if (inputchklen != 0 && checkinput != 0) {
                                        $chkAllCheckOrNot.prop("checked", false);
                                    }
                                    else {
                                        $chkAllCheckOrNot.prop("checked", false);
                                    }
                                }
                                else {
                                    $chkAllCheckOrNot.prop("checked", false);
                                }

                                var Id = $(getIDFromContainer.find("li li")[y]).attr("id")
                                var text = $(getIDFromContainer.find("li li")[y]).text()
                                text = text.substring(0, text.lastIndexOf("("));


                                //if ($leftTreeDiv.find("div").find("div[data-nodeid='" + SummaryID + "']").parent().hasClass("k-state-selected"))
                                //{


                                var src = $(getIDFromContainer.find("li li")[y]).parent().parent().find("i").data("imgsrc");
                                src = src.substring(src.lastIndexOf("/") + 1);
                                var summImage = null;
                                if (src == "nonSummary.png") {
                                    summImage = "<i class='summNonsumImage vis vis-m-window'  data-imgsrc='Images/mWindow.png'></i>";
                                }
                                else if (src == "summary.png") {
                                    summImage = "<i class='fa fa-folder-o summNonsumImage' data-imgsrc='Images/folder.png'></i>";
                                }
                                else if (src == "folder.png") {
                                    summImage = "<i class='fa fa-folder-o summNonsumImage' data-imgsrc='Images/folder.png'></i>";
                                    continue;
                                }
                                if (src == "mWindow.png") {
                                    summImage = "<i class='summNonsumImage vis vis-m-window' data-imgsrc='Images/mWindow.png'></i>";
                                }


                                if ($cmbSearch.val() != "" && $chktreeNode.is(":checked")) {
                                    if (!text.contains($cmbSearch.val())) {
                                        continue;
                                    }
                                }


                                var checkBox = $('<input class="VIS-tm-checkbox" type="checkbox" />');
                                $ulMid.prepend($('<li class="VIS-tm-topMLi">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-id='" + Id + "'  id='" + Id + "' style='cursor:default'  >" + VIS.Utility.encodeText(text) + "</li>")));
                                //  }
                                //$squenceDailog.removeClass("vis-tm-delete");
                                //isulMidData = true;

                            }
                        }



                        for (var k = 0; k < getIDFromContainer.find("li .vis-tm-textli").length ; k++) {


                            //var getNodeIDForIcon = $($($ulRight.find("li"))).find("li[id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("id") + "']").parent().parent();
                            var getNodeIDForIcon = $($($ulRight.find("li"))).find("li[data-id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("data-id") + "']").parent().parent();
                            getNodeIDForIcon.find("input").prop("disabled", "true");
                            getNodeIDForIcon.find("input").prop("checked", false);
                            getNodeIDForIcon.find("i").addClass("vis-tm-opacity");
                            getNodeIDForIcon.find(".vis-tm-textli").addClass("vis-tm-opacity");
                            getNodeIDForIcon.find("input").css("cursor", "not-allowed");

                            if (!getNodeIDForIcon.find("span").hasClass("VIS-Tm-glyphiconglyphicon-link")) {
                                getNodeIDForIcon.find("span").addClass("VIS-Tm-glyphiconglyphicon-link");
                            }

                            if ($rightMenuDemand.val() == "Unlinked") {
                                getNodeIDForIcon.remove();
                            }


                            //if (getNodeIDForIcon.find("span").attr("class") == "null") {
                            //    getNodeIDForIcon.find("span").addClass("glyphicon glyphicon-link");
                            //}

                            //var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
                            //table_id = VIS.DB.executeDataSet(table_id, null, null);
                            //if (table_id.tables[0].rows.length > 0) {
                            //    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
                            //}

                            //var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
                            //tablename = VIS.DB.executeDataSet(tablename, null, null);
                            //if (tablename.tables[0].rows.length > 0) {
                            //    tablename = tablename.tables[0].rows[0].cells["tablename"];
                            //}


                            //var imgSource = $($($ulRight.find("li"))).find("li[id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("id") + "']").parent().parent().find("img").attr("src");
                            //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("id") + "']").parent().parent().find("img");

                            var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("data-id") + "']").parent().parent().find("img").attr("src");
                            var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + $(getIDFromContainer.find(".vis-tm-textli")[k]).attr("data-id") + "']").parent().parent().find("img");
                            var setLinkedImage;
                            var src = imgSource;
                            if (src != null) {//folder.png
                                src = src.substring(src.lastIndexOf("/") + 1);

                                if (src == "mWindow.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/nonSummary.png";
                                }
                                else if (src == "folder.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/summary.png";
                                }

                                changeImgSource.attr("src", setLinkedImage);
                            }
                            getRunFunction = false;
                        }


                        //                        var getRunFunction = true;
                        for (var i = 0; i < $ulRight.find(".vis-tm-menuselectedcolor").length; i++) {
                            //var Id = $($ulRight.find(".vis-tm-menuselectedcolor").find(".vis-tm-textli")[i]).attr("id");
                            //var text = $($ulRight.find(".vis-tm-menuselectedcolor").find(".vis-tm-textli")[i]).text();
                            //text = text.substring(0, text.lastIndexOf("("));

                            // //$($($ulRight.find("li"))).find("li[id='" + getIDFromContainer.find(".vis-tm-textli").attr("id") + "']");
                            //if ($leftTreeDiv.find("div").find("div[data-nodeid='" + SummaryID + "']").parent().hasClass("k-state-selected")) {


                            //    var src = $($ulRight.find(".vis-tm-menuselectedcolor").find(".vis-tm-textli")[i]).parent().parent().find("img").attr("src");
                            //    src = src.substring(src.lastIndexOf("/") + 1);
                            //    var summImage = null;
                            //    if (src == "nonSummary.png") {
                            //        summImage = "<img class='summNonsumImage' style='float:left;margin-right:10px;margin-top:3px'  src='" + VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png'></img>";
                            //    }
                            //    else if (src == "summary.png") {
                            //        summImage = "<img class=' summNonsumImage ' style='float:left;margin-right:10px;margin-top:3px' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/folder.png'></img>";
                            //    }


                            //    var checkBox = $('<input class="VIS-tm-checkbox" style="float:left; margin-right: 10px;margin-left:0" type="checkbox" />');
                            //    $ulMid.append($('<li class="VIS-tm-topMLi" style="padding:5px 0px">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-issummary='" + res[i]["issummary"] + "' id='" + Id + "' style='cursor:pointer'  >" + VIS.Utility.encodeText(text) + "</li>")));
                            //}



                            //var checkBox = $('<input class="VIS-tm-checkbox" style="float:left; margin-right: 10px;margin-left:0" type="checkbox" />');
                            //$ulMid.append($('<li class="VIS-tm-topMLi" style="padding:5px 0px">').append(checkBox).append($("<li class='VIS-tm-MLi' id='" + Id + "' style='cursor:pointer'  >" + text + "</li>")));
                            getRunFunction = false;
                        }
                        if (getRunFunction != false) {
                            if (getIDFromContainer.find(".vis-tm-textli").length == 1) {
                                //var getLiByID = $($($ulRight.find("li"))).find("li[id='" + getIDFromContainer.find(".vis-tm-textli").attr("id") + "']");
                                //var imgSource = $($($ulRight.find("li"))).find("li[id='" + getIDFromContainer.find(".vis-tm-textli").attr("id") + "']").parent().parent().find("img").attr("src");
                                //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + getIDFromContainer.find(".vis-tm-textli").attr("id") + "']").parent().parent().find("img");



                                var getLiByID = $($($ulRight.find("li"))).find("li[data-id='" + getIDFromContainer.find(".vis-tm-textli").attr("data-id") + "']");
                                var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + getIDFromContainer.find(".vis-tm-textli").attr("data-id") + "']").parent().parent().find("img").attr("src");
                                var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + getIDFromContainer.find(".vis-tm-textli").attr("data-id") + "']").parent().parent().find("img");

                                var setLinkedImage;
                                var src = imgSource;
                                if (src != null) {
                                    src = src.substring(src.lastIndexOf("/") + 1);

                                    if (src == "mWindow.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/nonSummary.png";
                                    }
                                    else if (src == "folder.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/summary.png";
                                    }

                                    changeImgSource.attr("src", setLinkedImage);
                                }
                                //var Id = getLiByID.attr("id");
                                //var text = getLiByID.text();
                                //text = text.substring(0, text.lastIndexOf("("));

                                // var checkBox = $('<input class="VIS-tm-checkbox" style="float:left; margin-right: 10px;margin-left:0" type="checkbox" />');
                                //   $ulMid.append($('<li class="VIS-tm-topMLi" style="padding:5px 0px">').append(checkBox).append($("<li class='VIS-tm-MLi' id='" + Id + "' style='cursor:pointer'  >" + text + "</li>")));

                            }
                        }
                        // data-getparentnode='#= item.getparentnode #'       data-IsSummary='#= item.IsSummary #' data-TableName='#= item.TableName #'
                        //data-NodeID='#= item.NodeID #'   data-TreeParentID='#= item.TreeParentID #' data-ParentID='#= item.ParentID #'  


                        //var imgsrc = getIDFromContainer.find("img").attr("src").substring(getIDFromContainer.find("img").attr("src").lastIndexOf("/") + 1);
                        //if (imgsrc)
                        //{

                        //}

                        var lstNewChildren = [];

                        for (var t = 0; t < getIDFromContainer.find(".vis-tm-textli").length ; t++) {
                            var IsSummaryOrNot = null;
                            var ImageSource = null;
                            if ($(getIDFromContainer.find(".vis-tm-textli")[t]).attr("checkSummary") == "checkSummary") {
                                IsSummaryOrNot = "true";
                                ImageSource = "Areas/VIS/Images/folder.png";

                            }
                            else {
                                IsSummaryOrNot = "false";
                                ImageSource = "Areas/VIS/Images/mWindow.png";
                            }

                            if ($chkSummaryLevel.is(":checked")) {
                                if (IsSummaryOrNot == "false") {
                                    continue;
                                }
                            }


                            if ($chkSummaryLevel.is(":checked")) {
                                if (IsSummaryOrNot == "true") {
                                    $treeExpandColapse.css("display", "inherit");
                                    $treeCollapseColapse.css("display", "none");
                                }
                            }


                            if (!$chkSummaryLevel.is(":checked")) {
                                $treeExpandColapse.css("display", "inherit");
                                $treeCollapseColapse.css("display", "none");
                            }


                            var textForTree = $(getIDFromContainer.find(".vis-tm-textli")[t]).text();
                            textForTree = textForTree.substring(0, textForTree.lastIndexOf("("));

                            var newChild = $leftTreeDiv.data("kendoTreeView").append({
                                //text: VIS.Utility.encodeText($(getIDFromContainer.find(".vis-tm-textli")[t]).text()),
                                text: VIS.Utility.encodeText(textForTree),
                                'IsSummary': IsSummaryOrNot,
                                'getparentnode': $dropableItem.attr("data-nodeid"),
                                'TableName': $dropableItem.attr("data-tablename"),
                                'NodeID': $(getIDFromContainer.find(".vis-tm-textli")[t]).attr("id"),
                                'TreeParentID': $dropableItem.attr("data-nodeid"),
                                'ParentID': $dropableItem.attr("data-nodeid"),//,
                                'ImageSource': ImageSource,
                            }, $dropableItem);
                            lstNewChildren.push(newChild);
                        }

                        var $drpableItemLis = $dropableItem.parent().parent().parent();

                        if (lstNewChildren && lstNewChildren.length > 0) {
                            for (var h = lstNewChildren.length - 1; h >= 0; h--) {
                                moveDraggedItemToFirstPalce($drpableItemLis);
                            }
                        }



                        //if (tableTreeName == null)
                        //{
                        //    TreeTableName();
                        //}


                        //var childElements = $dropableItem.parent().parent().parent().children('ul').find('li');
                        //var queries = [];
                        //if (childElements.length > 0) {
                        //    for (var v = 0; v < childElements.length; v++) {
                        //        var cEle = childElements[v];
                        //        var nodeIDD= cEle.find(".treechild").attr("data-nodeid");
                        //     var   sql = "UPDATE ";
                        //     sql += tableTreeName + " SET Parent_ID=" + $dropableItem.attr("data-nodeid") + ", SeqNo=" + v + ", Updated=SysDate" +
                        //                          " WHERE AD_Tree_ID=" + $treeID + " AND Node_ID=" + nodeIDD;
                        //        queries.push(sql);
                        //    }
                        //}

                        //console.log(queries);
                        //VIS.DB.executeQueries(queries)


                        menuselectedcolor.removeClass("vis-tm-menuselectedcolor");

                        if ($ulMid.find("li").length > 0) {
                            $recordeNotFound.css("display", "none");
                            $bsyDivforbottom[0].style.visibility = "hidden";
                        }
                    }
                    else {
                        // VIS.ADialog.info("VIS_DataNotSaved");
                    }

                    //if ($checkMorRdragable) {
                    //    for (var j = 0; j < getIDFromContainer.find("li").find(".VIS-tm-MLi").length; j++) {
                    //        var getmidDragID = $(getIDFromContainer.find("li").find(".VIS-tm-MLi")[j]).attr("id");

                    //        if (getmidDragID == $dropableItem.attr("data-nodeid")) {
                    //            continue;
                    //        }

                    //        // var childOfParentofDroppable = $dropableItem.parent().parent().parent().children().find("div[data-nodeid='" + getmidDragID + "']");
                    //        //if (childOfParentofDroppable && childOfParentofDroppable.length > 0) {
                    //        //    continue;
                    //        //}

                    //        if ($dropableItem.data("nodeid") == getIDFromContainer.find("li li").data('nodepid')) {
                    //            continue;
                    //        }


                    //        //$($ulMid.find("li").find("li[id='" + getmidDragID + "']").parent()).remove();
                    //        // TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                    //        $($ulMid.find("li").find("li[data-id='" + getmidDragID + "']").parent()).remove();
                    //        removefromtree = $leftTreeKeno.find("div").find("div[data-nodeid='" + getmidDragID + "']").parent().parent().parent();
                    //        //$leftTreeDiv.data("kendoTreeView").remove(removefromtree);
                    //        //  moveDraggedItemToFirstPalce(removefromtree);

                    //        $leftTreeDiv.data("kendoTreeView").detach(removefromtree);

                    //        var par = $dropableItem.parent().parent().parent();
                    //        if (par) {
                    //            par = $(par).children('ul');
                    //            if (par && par.length > 0) {
                    //                par = par.children();
                    //                if (par && par.length > 0) {
                    //                    $(par).prepend(removefromtree);
                    //                }
                    //                else {
                    //                    $(par).prepend(removefromtree);
                    //                }
                    //            }
                    //            else {
                    //                $dropableItem.parent().parent().parent().append(removefromtree);
                    //            }
                    //        }

                    //        //$dropableItem.parent().parent().parent().children('ul').children().prepend(removefromtree);
                    //    }

                    //    //var lstNewChildren = [];

                    //    //for (var t = 0; t < getIDFromContainer.find("li").find(".VIS-tm-MLi").length ; t++) {
                    //    //    var getmidDragID = $(getIDFromContainer.find("li").find(".VIS-tm-MLi")[t]).attr("id");
                    //    //    if (getmidDragID == $dropableItem.attr("data-nodeid")) {
                    //    //        continue;
                    //    //    }
                    //    //    //var childOfParentofDroppable = $dropableItem.parent().parent().parent().children().find("div[data-nodeid='" + getmidDragID + "']");
                    //    //    //if (childOfParentofDroppable && childOfParentofDroppable.length > 0) {
                    //    //    //    continue;
                    //    //    //}

                    //    //    if ($dropableItem.data("nodeid") == getIDFromContainer.find("li li").data('nodepid')) {
                    //    //        continue;
                    //    //    }


                    //    //    var imgSource = $(getIDFromContainer.find(".VIS-tm-topMLi")[t]).find("img").attr("src");
                    //    //    var changeImgSource = $(getIDFromContainer.find(".VIS-tm-topMLi")[t]).find("img");
                    //    //    var setLinkedImage;
                    //    //    var src = imgSource;
                    //    //    if (src != null) {
                    //    //        src = src.substring(src.lastIndexOf("/") + 1);
                    //    //    }

                    //    //    var IsSummaryOrNot = null;
                    //    //    var ImageSource = null;
                    //    //    if (src == "folder.png") {
                    //    //        IsSummaryOrNot = "true";
                    //    //        ImageSource = "Areas/VIS/Images/folder.png";

                    //    //    }
                    //    //    else {
                    //    //        IsSummaryOrNot = "false";
                    //    //        ImageSource = "Areas/VIS/Images/mWindow.png";
                    //    //    }

                    //    //    // if ($chkSummaryLevel.is(":checked")) {
                    //    //    if (IsSummaryOrNot == "false") {
                    //    //        continue;
                    //    //    }
                    //    //    // }

                    //    //    var textForTree = $(getIDFromContainer.find("li").find(".VIS-tm-MLi")[t]).text();
                    //    //    //var newChild = $leftTreeDiv.data("kendoTreeView").append({
                    //    //    //    text: VIS.Utility.encodeText(textForTree),
                    //    //    //    'IsSummary': IsSummaryOrNot,
                    //    //    //    'getparentnode': SummaryID,
                    //    //    //    'TableName': $dropableItem.attr("data-tablename"),
                    //    //    //    'NodeID': $(getIDFromContainer.find("li").find(".VIS-tm-MLi")[t]).attr("id"),
                    //    //    //    'TreeParentID': $dropableItem.attr("data-nodeid"),
                    //    //    //    'ParentID': SummaryID,
                    //    //    //    'ImageSource': ImageSource,
                    //    //    //}, $dropableItem);
                    //    //    //lstNewChildren.push(newChild);


                    //    // //   $dropableItem= removefromtree

                    //    //}

                    //    //   moveDraggedItemToFirstPalce(removefromtree);

                    //    //var $drpableItemLis = $dropableItem.parent().parent().parent();

                    //    //if (lstNewChildren && lstNewChildren.length > 0) {
                    //    //    for (var h = lstNewChildren.length - 1; h >= 0; h--) {
                    //    //        moveDraggedItemToFirstPalce($drpableItemLis);
                    //    //    }
                    //    //}
                    //}
                    mListDrag();
                    AllSelectChkValue();

                    window.setTimeout(function () {
                        if ($cmbSearch.val() != "") {
                            //getEvalueforsearch

                            SearchNodeInTree(getEvalueforsearch);

                            //SearchNodeInTree(chksearchvalues);
                        }
                    }, 200);

                    HasScrollarOrNot();
                    chkCountForrestriction = true;
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
        };
        var currentHeight;
        var pressTimer = null;
        var topObj;
        var bottomObj;
        var getTreeNodeChkValue = false;

        var expandCollapse = false;
        var animateSearchFlag = false;
        function EventHandling() {
            $cmbSelectTree.on("change", OnTreeChange);
            $chkSummaryLevel.on("click", IsSummaryLevelCheck)
            $ulBackDiv.on("scroll", GetAllMenuData);


            //var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
            //if (ismobile)
            //{

            //    $ulRight.bind('touchstart', function (event)
            //    {
            //        if (event.preventDefault) event.preventDefault();
            //        $ulRight.find("li").css("-webkit-touch-callout", "none");
            //        DragMenu();
            //    });


            //    $ulRight.bind('touchend', function (event)
            //    {
            //        if (event.preventDefault) event.preventDefault();
            //        $ulRight.find("li").css("-webkit-touch-callout", "none");
            //        $($ulRight.find("li")).draggable("destroy");
            //    });


            //    //$ulRight.bind('taphold', function (event) {
            //    //    if (event.stopPropagation) event.stopPropagation();
            //    //    if (event.preventDefault) event.preventDefault();
            //    //    $ulRight.find("li").css("-webkit-touch-callout", "none");
            //    //    DragMenu();
            //    //});
            //}




            // var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
            // if (ismobile)
            // {
            //     $ulRight.find("li").on("taphold", function (e)
            //     {
            //         // e.stopPropagation();                     
            //         e.preventDefault();
            //         alert("hello");
            //        var pressTimer

            //        $ulRight.find("span").mouseup(function (e) 
            //        {
            //            clearTimeout(pressTimer)
            //            alert("mouseup");
            //            return false;
            //        }).mousedown(function ()
            //        {
            //            pressTimer = window.setTimeout(function (e)
            //            {
            //                e.preventDefault();
            //                alert("timer");
            //                alert("ok");

            //            }, 1000)
            //            return false;
            //        });
            //    });
            //};






            $ulRight.on("click", GetNodePath);
            $ulMid.on("click", MidSelectDesign);
            $cmbSearch.on("keyup", SearchNodeInTree);
            $cmbSearch.on("keypress", AnimateSearchNode);
            $btnSearch.on("click", AnimateNodeWithButton);

            $cmbSearch.on('input', function (e) {
                if ('' == this.value) {
                    //$checkSearchOrNot.css("display", "none");
                    $leftTreeDiv.find("span").removeClass("VIS-TM-highlight");


                    pageNoForChild = 1;
                    pageSizeForChild = 50;
                    searchChildNode = $cmbSearch.val();
                    if ($chktreeNode.is(":checked")) {
                        getTreeNodeChkValue = "true";
                    }
                    else {
                        getTreeNodeChkValue = "false";
                    }
                    var selectedNodeID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
                    GetDataTreeNodeSelect(selectedNodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);

                }
            });


            $cmbRightSearch.on('input', function (e) {
                if ('' == this.value) {
                    if ($cmbSelectTree.val() != "") {
                        $ulRight.empty();
                        window.setTimeout(function () {
                            $ulRight.empty();
                            searchRightext = "";
                            $demandsMenu = $rightMenuDemand.val();
                            pageNo = 1;
                            LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                        }, 200);
                    }
                }
            });


            $cmbRefresh.on("click", CmbRefresh);
            $treeRefresh.on("click", TreeRefresh);
            $btnRightSearch.on("click", RightPanelSearch);
            // $cmbRightSearch.on("keypress", RighPanelSearchOnEnter);

            $cmbRightSearch.on("keyup", RighPanelSearchOnEnter);


            $treeBackDiv.on("resize", ResizeBottomDiv);
            $treeBackDiv.find(".ui-resizable-e").css("right", "0px");
            $treeBackDiv.find(".ui-resizable-s").css("bottom", "0px");
            FillValueInCombo();
            $deleteChild.on("click", DeleteBottomDivData);
            $rightMenuDemand.on("change", MenuWithDemands);

            $deleteArea.mouseover(function (e) {
                $deleteArea.addClass("selectchangecolor");
                $deleteArea.css("background-color", "#ff0000");
                //$deleteArea.animate({
                //    backgroundColor: "#ff0000",
                //}, 10);
            });


            $deleteArea.mouseleave(function () {
                $deleteArea.removeClass("selectchangecolor");
                $deleteArea.css("background-color", "#FFC0CB");
                //gatDenideImg.css("display", "inherit");
                //gatDenideImg.parent().removeClass("glyphicon glyphicon-trash");
                //$deleteArea.animate({
                //    backgroundColor: "#FFC0CB",
                //});
            });

            $mData.on("scroll", GetChildDataOfTreeNode);


            //$treeNodeSearch
            //$chktreeNode =
            //$lblNodetext =
            //$searchDiv.append($cmbSearch).append($treeNodeSearch).append($btnSearch);

            window.setTimeout(function () {
               // $cmbSearch.width($searchDiv.width() - ($treeNodeSearch.width() + 40 + $btnSearch.width()));
            }, 200);


            if ($chktreeNode.is(":checked")) {
                lastSeletedIndex = -1;
            }

            $chktreeNode.on("click", function (e) {
                if ($chktreeNode.is(":checked")) {
                    getTreeNodeChkValue = "true";
                }
                else {
                    getTreeNodeChkValue = "false";

                }
                SearchNodeInTree(e);
            })


            $chkAllCheckOrNot.on("click", NodeItemSecOrDesec);
            // $checkSearchOrNot.on("click", IsSearchOrNot);


            //nodeItemDelwithdrag.mouseover(function (e) {
            //   // nodeItemDelwithdrag.addClass("selectchangecolor");
            //    window.setTimeout(function () {
            //        nodeItemDelwithdrag.animate({
            //            backgroundColor: "#ff0000",
            //        }, 200);
            //    }, 100);
            //});
            //nodeItemDelwithdrag.mouseleave(function () {
            //    // nodeItemDelwithdrag.removeClass("selectchangecolor");
            //    window.setTimeout(function () {
            //        nodeItemDelwithdrag.css("background-color", "#FFC0CB");
            //    }, 100);
            //    //nodeItemDelwithdrag.animate({
            //    //    backgroundColor: "#FFC0CB",
            //    //});
            //});

            // $mData.height($secoundDiv.height() - $lblMh4.height() + 10);


            $treeExpandColapse.on("click", TreeExpandCollapse);
            $treeCollapseColapse.on("click", TreeCollapse);

            $squenceDailog.on("click", OpenSquenceDailog);

            //$ulSeqParentDiv.on("scroll", GetSequenceData);    

            $chkAllRightPannel.on("click", SelectAllRightPanel)


            crossImages.on("click", EmptySearchText);

            rightCrossImage.on("click", EmptyRightSearchText);


            ZoomTreeWindow.on("click", CreateNewTree);


            mMouseRestrict.mouseover(function (e) {
                mMouseRestrict.css({ 'cursor': 'not-allowed' });
            });
            mMouseRestrict.mouseleave(function () {
                mMouseRestrict.css({ 'cursor': 'default' });
            });


            unlinkeAllNode.on("click", OpenW2overlay)

            $chkTrace.on("click", StopDefaultW2overlay);

        };

        var menuArray = [];
        var bindornot = "true";
        var convertmenuArray = null;
        var msgShowforbindingWindow = null;
        function CreateRestrictionforDelete() {
            menuArray = [];
            var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            table_id = executeDataSet(table_id, null, null);
            if (table_id.tables[0].rows.length > 0) {
                table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            }

            var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
            tablename = executeDataSet(tablename, null, null);
            if (tablename.tables[0].rows.length > 0) {
                tablename = tablename.tables[0].rows[0].cells["tablename"];
            }

            bindornot = "true";

            if (tablename == "AD_Menu") {
                var rolCheck = "SELECT count(*) FROM AD_Role WHERE ad_tree_menu_id=" + $treeID;
                var checkCount = executeScalar(rolCheck);
                if (checkCount > 0) {
                    bindornot = "false";
                }
                else {
                    var tenantCheck = "SELECT count(*) FROM AD_ClientInfo WHERE ad_tree_menu_id=" + $treeID;
                    var checktenant = executeScalar(tenantCheck);
                    if (checktenant > 0) {
                        bindornot = "false";
                    }
                }
            }

            //if(bindornot == false)
            //{
            var getIdes = "SELECT ad_menu_id FROM AD_Menu WHERE ad_window_id IN (SELECT ad_window_id FROM ad_window WHERE name IN ('Role','Tenant','Tree'))";

            var formID = "SELECT ad_menu_id FROM AD_Menu WHERE ad_form_id IN (SELECT ad_form_id FROM ad_form WHERE name IN ('Tree Maintenance'))";




            var ds1 = executeDataSet(formID, null, null);

            if (ds1 != null && ds1.tables[0].rows.length > 0) {
                for (var i = 0; i < ds1.tables[0].rows.length; i++) {
                    menuArray.push(ds1.tables[0].rows[i].cells["ad_menu_id"]);
                }
            }



            var ds = executeDataSet(getIdes, null, null);

            if (ds != null && ds.tables[0].rows.length > 0) {
                for (var i = 0; i < ds.tables[0].rows.length; i++) {
                    menuArray.push(ds.tables[0].rows[i].cells["ad_menu_id"]);
                }
            }
            // }
            convertmenuArray = menuArray;
            menuArray = menuArray.toString();

            var getnamebyID = "SELECT name FROM ad_menu WHERE ad_menu_id IN(" + menuArray + ") ORDER BY upper(name)";
            var dss = executeDataSet(getnamebyID, null, null);

            var messagess = "";
            if (dss != null && dss.tables[0].rows.length > 0) {
                for (var m = 0; m < dss.tables[0].rows.length; m++) {
                    messagess += dss.tables[0].rows[m].cells["name"];
                    messagess += ",";
                }
            }


            msgShowforbindingWindow = messagess + " " + VIS.Msg.getMsg("MenuBinded");


            $bsyDivforbottom[0].style.visibility = "hidden";
        };


        function UpdateRollCheckSeq(idforupdate) {
            TreeTableName();

            var maxSeq = "SELECT MAX(seqno) FROM " + tableTreeName + " WHERE AD_Tree_ID=" + $treeID;
            var seq = executeScalar(maxSeq);
            seq += 1;
            var increaseSqe = "update " + tableTreeName + " set seqno=" + seq + ",Updated=Sysdate,parent_ID=0 WHERE AD_Tree_ID=" + $treeID + " AND node_id=" + idforupdate;

            executeQuery(increaseSqe, null, null);


        };


        function StopDefaultW2overlay(e) {
            //  e.stopPropagation();
        };

        function RemoeveLinkedNode(e) {
            if ($cmbSelectTree.val() != "") {
                if ($(this).find("span").hasClass("vis-tm-delete")) {
                    return;
                }

                VIS.ADialog.confirm("RemoveUnlinkedItem", true, "", "Confirm", function (result) {
                    if (!result) {
                        return;
                    }
                    else {
                        RemoveLinkedItemFromTree();
                        $treeExpandColapse.css("display", "none");
                        $treeCollapseColapse.css("display", "inherit");

                        var selText = $cmbSelectTree.find("option:selected").text();
                        $lblMh4.text(selText);
                        chkCountForrestriction = false;
                    }
                });
            }
        };


        function RemoveLinkedItemFromTree() {
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/RemoveLinkedItemFromTree",
                type: 'Post',
                data: { treeID: $treeID, menuId: menuArray },
                success: function (data) {
                    var res = JSON.parse(data);

                    if (res == "count") {
                        $ulRight.empty();
                        searchRightext = $cmbRightSearch.val();
                        $demandsMenu = $rightMenuDemand.val();
                        pageNo = 1;
                        LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                        window.setTimeout(function () {
                            //   changeSeqFlag = false;
                            TreeRefresh();
                            window.setTimeout(function () {
                                chkCountForrestriction = false;
                                $bsyDiv[0].style.visibility = "hidden";
                            }, 200);
                        }, 200);
                    }
                    else if (res == "menubind") {
                        //VIS.ADialog.info("NeverDelete");
                        $bsyDiv[0].style.visibility = "hidden";
                        VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);
                    }

                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
        };


        var $outerP = null;
        function OpenW2overlay() {
            if ($cmbSelectTree.val() != "") {
                var $pintohomeOverlay = $('<ul class="vis-apanel-tm-ul"></ul>');

                var $outerDiv = $('<div>');
                var $traceDiv = $('<div style="padding:0 10px;min-width:150px">');
                var imgSpan = $('<span class="VIS-Tm-glyphiconglyphicon-link VIS-Tm-unlinkforbottmo vis vis-link"></span>');
                $outerP = $('<p style="line-height:19px;cursor:pointer;">' + VIS.Msg.getMsg("RemoveLinkedItem") + '</p>').append(imgSpan);

                $outerDiv.append($traceDiv);
                $traceDiv.append($('<div style="margin-top: 10px;" ></div>').append($chkTrace).append($lblTrace));
                $traceDiv.append($('<div style="float: left;margin-bottom: 6px;margin-top: 5px;width:100%" ></div>').append($outerP));



                $(this).w2overlay($outerDiv);


                //TreeTableName();
                //var sqlQry = "SELECT Count(*) as Count FROM " + tableTreeName + " WHERE AD_Tree_ID=" + $treeID;
                //sqlQry = VIS.DB.executeDataSet(sqlQry, null, null);
                //sqlQry = sqlQry.tables[0].rows[0].cells["count"];

                //if (sqlQry == 0) {
                //    imgSpan.addClass("vis-tm-delete");
                //}
                //else
                //{
                //    imgSpan.removeClass("vis-tm-delete");
                //}

                if (chkCountForrestriction == true) {
                    imgSpan.removeClass("vis-tm-delete");
                }
                else {
                    imgSpan.addClass("vis-tm-delete");
                }

                $outerP.on("click", RemoeveLinkedNode);
            }
        };





        //function touchHandler(event) {
        //    var self = this;
        //    var touches = event.changedTouches,
        //        first = touches[0],
        //        type = "";

        //    switch (event.type) {
        //        case "touchstart":
        //            type = "mousedown";
        //            window.startY = event.pageY;
        //            break;
        //        case "touchmove":
        //            type = "mousemove";
        //            break;
        //        case "touchend":
        //            type = "mouseup";
        //            break;
        //        default:
        //            return;
        //    }
        //    var simulatedEvent = document.createEvent("MouseEvent");
        //    simulatedEvent.initMouseEvent(type, true, true, window, 1, first.screenX, first.screenY, first.clientX, first.clientY, false, false, false, false, 0 /*left*/, null);

        //    first.target.dispatchEvent(simulatedEvent);

        //    var scrollables = [];
        //    var clickedInScrollArea = false;
        //    // check if any of the parents has is-scollable class
        //    var parentEls = jQuery(event.target).parents().map(function () {
        //        try {
        //            if (jQuery(this).hasClass('scrolldrag')) {
        //                clickedInScrollArea = true;
        //                // get vertical direction of touch event
        //                var direction = (window.startY < first.clientY) ? 'down' : 'up';
        //                // calculate stuff... :o)
        //                if (((jQuery(this).scrollTop() <= 0) && (direction === 'down')) || ((jQuery(this).height() <= jQuery(this).scrollTop()) && (direction === 'up'))) {

        //                } else {
        //                    scrollables.push(this);
        //                }
        //            }
        //        } catch (e) { }
        //    });
        //    // if not, prevent default to prevent bouncing
        //    if ((scrollables.length === 0) && (type === 'mousemove')) {
        //        event.preventDefault();
        //    }

        //}

        //function initTouchHandler(e) {
        //    document.addEventListener("touchstart", touchHandler, true);
        //    document.addEventListener("touchmove", touchHandler, true);
        //    document.addEventListener("touchend", touchHandler, true);
        //    document.addEventListener("touchcancel", touchHandler, true);

        //}




        function HasScrollarOrNot(e) {
            if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {
                if ($mData.find("ul").height() < $mData.height()) {
                    pageNoForChild = 1;
                    pageSizeForChild = 50;

                    if ($chktreeNode.is(":checked")) {
                        searchChildNode = $cmbSearch.val();
                    }
                    else {
                        searchChildNode = "";
                    }
                    if ($chktreeNode.is(":checked")) {
                        getTreeNodeChkValue = "true";
                    }
                    else {
                        getTreeNodeChkValue = "false";
                    }
                    $scrollBottom = false;
                    var selectedNodeID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
                    GetDataTreeNodeSelect(selectedNodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                }
            }
        };



        function AllSelectChkRightSide() {
            $chkAllRightPannel.prop("checked", false);
        };


        function CreateNewTree() {
            var sql = "SELECT AD_Window_ID FROM AD_Window WHERE Name='Tree'";
            var n_win = executeScalar(sql);

            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction("AD_Tree_ID", VIS.Query.prototype.EQUAL, $treeID);
            VIS.viewManager.startWindow(n_win, zoomQuery);
        };

        function EmptyRightSearchText() {
            rightCrossImage.css("display", "none");
            $cmbRightSearch.val("");

            if ($cmbSelectTree.val() != "") {
                $ulRight.empty();
                window.setTimeout(function () {
                    $ulRight.empty();
                    searchRightext = "";
                    $demandsMenu = $rightMenuDemand.val();
                    pageNo = 1;
                    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                }, 200);
            }


        };


        function EmptySearchText(e) {


            $cmbSearch.val("");
            $cmbSearch.focus();
            crossImages.css("display", "none");


            $leftTreeDiv.find("span").removeClass("VIS-TM-highlight");

            pageNoForChild = 1;
            pageSizeForChild = 50;
            searchChildNode = $cmbSearch.val();
            if ($chktreeNode.is(":checked")) {
                getTreeNodeChkValue = "true";
            }
            else {
                getTreeNodeChkValue = "false";
            }
            $scrollBottom = true;
            var selectedNodeID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
            if ($cmbSelectTree.val() != "") {
                GetDataTreeNodeSelect(selectedNodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
            }


        };


        function SelectAllRightPanel(e) {

            chksearchvalues = e;
            if ($chkAllRightPannel.is(":checked")) {
                $bsyDivMenu[0].style.visibility = "visible";

                $rightmenuScroll = false;
                for (var i = 0; i < $ulRight.find("input").length ; i++) {
                    if ($($ulRight.find("input")[i]).attr("disabled") == "disabled") {
                        $($ulRight.find("input")[i]).parent().parent().removeClass("vis-tm-menuselectedcolor");
                    }
                    if ($($ulRight.find("input")[i]).attr("disabled") != "disabled") {
                        $($ulRight.find("input")[i]).prop("checked", true);
                        $($ulRight.find("input")[i]).parent().parent().addClass("vis-tm-menuselectedcolor");
                    }
                }
                $bsyDivMenu[0].style.visibility = "hidden";
            }
            else {
                if ($rightmenuScroll == false) {
                    $bsyDivMenu[0].style.visibility = "visible";
                    $ulRight.find("input").prop("checked", false);
                    $ulRight.find(".vis-tm-menuselectedcolor").removeClass("vis-tm-menuselectedcolor");
                    $bsyDivMenu[0].style.visibility = "hidden";
                }
            }
        };

        function SequenceLiDesign(e) {

            if ($(e.target).hasClass("VIS-tm-MLi")) {
                $(this).find("li").removeClass("vis-tm-menuselectedcolor");
                $(this).find("li").find("input").prop("checked", false)
                $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().find("input").prop("checked", true);
                $chkAllCheckOrNot.prop("checked", false);
            }

            if ($(e.target).hasClass(" summNonsumImage ")) {
                $(this).find("li").removeClass("vis-tm-menuselectedcolor");
                $(this).find("li").find("input").prop("checked", false)
                $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().find("input").prop("checked", true);
                $chkAllCheckOrNot.prop("checked", false);
            }

            if ($(e.target).hasClass("VIS-tm-checkbox")) {

                if ($(e.target).parent().find("input").prop("checked")) {
                    $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                }
                else {
                    $(e.target).parent().removeClass("vis-tm-menuselectedcolor");
                    $(e.target).parent().find("li").removeClass("vis-tm-menuselectedcolor");
                    $chkAllCheckOrNot.prop("checked", false);
                }
            }
        };

        var scrolFlagforDailog = true;
        function GetSequenceData(e) {
            // if ($chktreeNode.is(":checked")) {
            //    searchChildNode = $cmbSearch.val();
            //}
            // else {
            searchChildNode = "";
            //}
            var elem = $(e.currentTarget);

            if (elem[0].scrollHeight - elem.scrollTop() == elem.outerHeight()) {
                scrolFlagforDailog = false;
                dailogpageNoForChild += 1;
                $bsyDiv[0].style.visibility = "visible";
                FillSequenceDailog(getParentID, $treeID, dailogpageNoForChild, dailogpageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
            }
        };

        var dailogpageNoForChild = 1;
        var dailogpageSizeForChild = 50;

        var changeSeqFlag = false;
        var onseqSelectID = 0;

        function OpenSquenceDailog(e, ui) {
            $bsyDiv[0].style.visibility = "visible";

            if ($cmbSelectTree.val() != "") {
                $cmbSearch.prop("disabled", false);
                $btnSearch.prop("disabled", false);
            }

            $setorderflagss = false;

            if (isulMidData == false) {
                $setorderflagss = true;
                $squenceDailog.addClass("vis-tm-delete");
                $bsyDiv[0].style.visibility = "hidden";
                return;
            }



            if ($leftTreeDiv.find(".k-state-selected").length == 0) {
                $bsyDiv[0].style.visibility = "hidden";
                $setorderflagss = true;
                return;
            }

            if ($cmbSelectTree.val() == "") {
                $bsyDiv[0].style.visibility = "hidden";
                $setorderflagss = true;
                return;
            }


            SquenceDailogDesign();

            scrolFlagforDailog = true;

            //pageNoForChild = 1;
            //pageSizeForChild = 50;


            dailogpageNoForChild = 1;
            dailogpageSizeForChild = 50;


            // if ($chktreeNode.is(":checked")) {
            //searchChildNode = $cmbSearch.val();
            //  }
            // else {
            searchChildNode = "";
            // }
            // if ($chktreeNode.is(":checked")) {
            // getTreeNodeChkValue = "true";
            // }
            // else {
            getTreeNodeChkValue = "false";
            // }
            var selectedNodeID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
            $bsyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {
                FillSequenceDailog(selectedNodeID, $treeID, dailogpageNoForChild, dailogpageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
            }, 200);


            $ulSeq.on("click", SequenceLiDesign);
            $ulSeqParentDiv.on("scroll", GetSequenceData);

            var createTab = new VIS.ChildDialog();
            createTab.setHeight(550);
            createTab.setWidth(450);
            createTab.setEnableResize(false);
            createTab.setTitle(VIS.Msg.getMsg('SetOrder'));
            createTab.setModal(true);
            createTab.setContent($seqContainer);
            $ulSeq.empty();
            createTab.show();
            createTab.onClose = function () {
                changeSeqFlag = true;
            };
            createTab.onOkClick = function (e) {
                if (isSorttingflag == false) {
                    isSorttingflag = true;
                    changeSeqFlag = true;

                    $bsyDivforbottom[0].style.visibility = "visible";

                    var selectedNodeIDss = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");

                    onseqSelectID = selectedNodeIDss;

                    var selectedNodeID = $leftTreeDiv.find(".k-state-selected").find(".treechild").attr("data-nodeid");
                    var ItemsID = [];
                    var ItemsIDTostring = "";
                    for (var l = 0; l < $ulSeq.find("li li").length; l++) {
                        var id = $($ulSeq.find("li li")[l]).attr("data-id");
                        ItemsID.push(id);
                    }
                    ItemsIDTostring = ItemsID.toString();
                    $.ajax({
                        url: VIS.Application.contextUrl + "TreeMaintenance/UpdateItemSeqNo",
                        type: 'Post',
                        //   async: false,
                        data: { treeID: $treeID, itemsid: ItemsIDTostring, ParentID: selectedNodeID },
                        success: function (data) {
                            var res = JSON.parse(data);
                            TreeRefresh();
                            $ulMid.empty();
                            pageNoForChild = 1;
                            pageSizeForChild = 50;

                            if ($chktreeNode.is(":checked")) {
                                searchChildNode = $cmbSearch.val();
                            }
                            else {
                                searchChildNode = "";
                            }
                            if ($chktreeNode.is(":checked")) {
                                getTreeNodeChkValue = "true";
                            }
                            else {
                                getTreeNodeChkValue = "false";
                            }
                            $scrollBottom = true;
                            // searchflag = false;


                            GetDataTreeNodeSelect(selectedNodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);

                            $treeExpandColapse.css("display", "inherit");
                            $treeCollapseColapse.css("display", "none");



                            //$bsyDivforbottom[0].style.visibility = "hidden";

                        },
                        error: function (e) {
                            console.log(e);
                            $bsyDivforbottom[0].style.visibility = "hidden";
                        },
                    });



                }
                else {
                    //elem[0].scrollHeight - elem.scrollTop() - 10 == elem.outerHeight() - 10
                    //$mData[0].scrollHeight = eleScrollHeight;
                    //$mData.scrollTop(eleScrollTop);

                    //if (eleScrollHeight - eleScrollTop > eleOutterHeight)
                    //{
                    //    eleOutterHeight = eleOutterHeight + (eleScrollHeight - eleScrollTop);
                    //}

                    //$mData.outerHeight(eleOutterHeight);
                }
                // $mData.off("scroll");
                //$mData.on("scroll", GetChildDataOfTreeNode);

                //$mData.height(leftMianDataDiv.height() - ($treeBackDiv.height() + $mTopHeader.height() + 60));
                //$mData.trigger("scroll");

                if ($chktreeNode.is(":checked")) {
                    searchChildNode = $cmbSearch.val();
                }
                else {
                    searchChildNode = "";
                }
                if ($chktreeNode.is(":checked")) {
                    getTreeNodeChkValue = "true";
                }
                else {
                    getTreeNodeChkValue = "false";
                }
                $setorderflagss = true;
            };
            createTab.onCancelClick = function () {
                $bsyDiv[0].style.visibility = "hidden";
                //$mData[0].scrollHeight = eleScrollHeight;
                //$mData.scrollTop(eleScrollTop);
                //$mData.outerHeight(eleOutterHeight);

                if ($chktreeNode.is(":checked")) {
                    searchChildNode = $cmbSearch.val();
                }
                else {
                    searchChildNode = "";
                }
                if ($chktreeNode.is(":checked")) {
                    getTreeNodeChkValue = "true";
                }
                else {
                    getTreeNodeChkValue = "false";
                }

                isSorttingflag = true;
                $setorderflagss = true;
                changeSeqFlag = false;
            };

        };

        var $ulSeq = $("<ul style='padding:0;list-style: none'>");;
        var $seqContainer = $('<div>');
        var $ulSeqParentDiv = $('<div style="padding: 10px;height:425px;overflow:auto">');
        function SquenceDailogDesign() {

            $seqContainer.append($ulSeqParentDiv.append($ulSeq));
        };

        function FillSequenceDailog(selectedNodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e) {
            //$bsyDivforbottom[0].style.visibility = "visible";
            if (searchChildNode == null) {
                searchChildNode = "";
            }

            if (scrolFlagforDailog == true) {
                $ulSeq.empty();
            }


            DropMenu();
            $.ajax({
                //                 url: VIS.Application.contextUrl + "TreeMaintenance/GetDataTreeNodeSelect",
                url: VIS.Application.contextUrl + "TreeMaintenance/FillSequenceDailog",
                type: 'Post',
                //async:false,
                data: { nodeID: selectedNodeID, treeID: $treeID, pageNo: pageNoForChild, pageLength: pageSizeForChild, searchChildNode: searchChildNode, getTreeNodeChkValue: getTreeNodeChkValue },
                success: function (data) {
                    $bsyDiv[0].style.visibility = "visible";
                    //sorttableLi();                    
                    var res = JSON.parse(data);
                    var summImage = null;
                    var nonSummImage = null;
                    if (res.length > 0) {
                        //$bsyDivforbottom[0].style.visibility = "visible";
                        //if ($scrollBottom == true) {
                        //    $ulMid.empty();
                        //}
                        for (var i = 0; i < res.length; i++) {
                            if ($scrollBottom == false) {
                                if ($ulSeq.find("li li").length > 0) {
                                    var lisItem = $ulSeq.find('[data-id="' + res[i]["parentID"] + '"]');
                                    if (lisItem && lisItem.length > 0) {
                                        continue;
                                    }

                                }
                            }

                            if (res[i]["parentID"] == 0) {
                                continue;
                            }

                            var checkBox = $('<input class="VIS-tm-checkbox" type="checkbox" />');
                            if (res[i]["issummary"] == "Y") {
                                // continue;
                                summImage = "<i class='fa fa-folder-o summNonsumImage' data-imgsrc='Images/folder.png'></i>";
                                $ulSeq.append($('<li class="VIS-tm-topMLi">').append(checkBox).append(summImage).append($("<li class='VIS-tm-MLi' data-NodePID='" + res[i]["NodeParentID"] + "' data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>")));
                            }
                            else {
                                nonSummImage = "<i class='vis vis-m-window summNonsumImage' data-imgsrc='Images/mWindow.png'></i>";
                                var li = $('<li class="VIS-tm-topMLi" >');
                                li.append(checkBox);
                                li.append(nonSummImage);
                                li.append($("<li class='VIS-tm-MLi'  data-NodePID='" + res[i]["NodeParentID"] + "'  data-id='" + res[i]["parentID"] + "' data-issummary='" + res[i]["issummary"] + "' id='" + res[i]["parentID"] + "'   >" + VIS.Utility.encodeText(res[i]["name"]) + "</li>"));
                                $ulSeq.append(li);
                                //LiSorttable(li);
                            }
                        }
                    }
                    else {
                    }
                    //window.setTimeout(function () {
                    dailogLiSorttable();
                    $ulSeq.sortable("refresh");
                    //}, 200);
                    window.setTimeout(function () {
                        $bsyDiv[0].style.visibility = "hidden";
                    }, 500);
                },
                error: function (e) {
                    console.log(e);
                    $ulSeq.empty();
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
        };

        //var overflowHidden = null;
        var elementsort = null;
        var siblingdata = null;
        var isSorttingflag = true;
        function dailogLiSorttable(e) {
            var helper = null;
            $ulSeq.sortable({
                cursorAt: { left: -10, top: -10 },
                helper: function (e, item) {
                    helper = $('<li />');
                    if (!item.hasClass("vis-tm-menuselectedcolor")) {
                        item.addClass('vis-tm-menuselectedcolor').siblings().removeClass('vis-tm-menuselectedcolor');
                    }
                    var elements = item.parent().children('.vis-tm-menuselectedcolor').clone();
                    elementsort = item.data('multidrag', elements).siblings('.vis-tm-menuselectedcolor');
                    item.data('multidrag', elements).siblings('.vis-tm-menuselectedcolor').hide();
                    //item.data('multidrag', elements).siblings('.vis-tm-menuselectedcolor').remove();

                    // helper.css({ "max-height": "250px", "overflow": "auto" });
                    siblingdata = helper.append(elements);
                    return helper.append(elements);
                },
                start: function (event, ui) {
                    //$(event.target).find(".ui-sortable-placeholder").css("height", "5px");

                    isSorttingflag = false;
                    $ulSeq.find("input").prop("checked", false);
                    GetPIDforItems = ui.item.find("li").attr("id");

                    $(this).parent().parent().parent().css("overflow", "hidden");


                },
                change: function (event, ui) {
                },
                update: function (event, ui) {
                },
                stop: function (event, ui) {
                    ui.item.after(ui.item.data('multidrag')).remove();

                    window.setTimeout(function () {
                        elementsort.remove();
                    }, 100);

                    // elementsort.remove();
                    $ulSeq.find("input").prop("checked", false);
                    $ulSeq.find(".vis-tm-menuselectedcolor").find("input").prop("checked", true);

                    //  overflowHidden.css("overflow", "auto")



                    //UpdateBottomItemSeqe(getParentID, $treeID);

                    //if (nodeItemDelwithdrag.hasClass("selectchangecolor"))
                    //{
                    //    $bsyDiv[0].style.visibility = "visible";
                    //    TreeTableName();
                    //    var findchilds = "Select node_id from " + tableTreeName + " where parent_id=" + GetPIDforItems + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
                    //    var child = VIS.DB.executeDataSet(findchilds, null, null);

                    //    if (child != null && child.tables[0].rows.length == 0) {
                    //        var selectedItemArray = [];
                    //        if ($ulMid.find("input:checked").length == 0) {
                    //            selectedItemArray.push(GetPIDforItems);
                    //        }

                    //        for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                    //            //var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                    //            var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("data-id");
                    //            selectedItemArray.push(getID);
                    //        }

                    //        bottomchildselectedID = selectedItemArray;
                    //        selectedItemArray = selectedItemArray.toString();
                    //        DeleteNodeFromBottom($treeID, selectedItemArray);
                    //        nodeItemDelwithdrag.removeClass("VIS-TM-ondragdrop");
                    //        nodeItemDelwithdrag.css("display", "none");
                    //    }
                    //}
                    //  else {

                    //}
                }
            });
        };

        function TreeCollapse() {
            if ($cmbSelectTree.val() != "") {

                // $leftTreeKeno.toggle(".k-item");


                // window.setTimeout(function ()
                //{
                if ($leftTreeDiv.find("li").length > 1) {
                    $bsyDivTree[0].style.visibility = "visible";

                    window.setTimeout(function () {
                        $treeCollapseColapse.css("display", "none");
                        $treeExpandColapse.css("display", "inherit");
                        $treeExpandColapse.css("float", "right");
                        $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                        $bsyDivTree[0].style.visibility = "hidden";
                    }, 150);
                }

                //}, 150);
            }
        };



        function TreeExpandCollapse() {
            if ($cmbSelectTree.val() != "") {
                $bsyDivTree[0].style.visibility = "visible";
                window.setTimeout(function () {
                    $treeCollapseColapse.css("display", "inherit");
                    $treeExpandColapse.css("display", "none");
                    $treeCollapseColapse.css("float", "right");

                    $leftTreeKeno.data("kendoTreeView").collapse(".k-item");
                    $bsyDivTree[0].style.visibility = "hidden";
                }, 150);

                //if (expandCollapse == false)
                //{
                //    expandCollapse = true;
                //    $bsyDivTree[0].style.visibility = "visible";
                //    // for (var p = 0; p < 30; p++)
                //    //  {
                //    $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                //    //   }
                //    $bsyDivTree[0].style.visibility = "hidden";
                //}
                //else {
                //    $bsyDivTree[0].style.visibility = "visible";
                //    expandCollapse = false;
                //    $leftTreeKeno.data("kendoTreeView").collapse(".k-item");
                //    $bsyDivTree[0].style.visibility = "hidden";
                //}
            }
        };

        function NodeItemSecOrDesec() {
            if ($chkAllCheckOrNot.is(":checked")) {
                $ulMid.find("input").prop("checked", true);
                $ulMid.find("li").addClass("vis-tm-menuselectedcolor");
                $deleteChild.removeClass("vis-tm-delete");
                if ($ulMid.find("input:checked").length == 0) {
                    $deleteChild.addClass("vis-tm-delete");
                }

            }
            else {
                $ulMid.find("input").prop("checked", false);
                $ulMid.find("li").removeClass("vis-tm-menuselectedcolor");
                $deleteChild.addClass("vis-tm-delete");
            }
        };

        function IsSearchOrNot() {
            if ($cmbSearch.val() !== "") {
                $cmbSearch.focus();
                $cmbSearch.select();
            }

        };

        var pagenumafteronCancel = 0;

        var eleScrollHeight = 0;
        var eleScrollTop = 0;
        var eleOutterHeight = 0;

        function GetChildDataOfTreeNode(e) {
            searchChildNode = $cmbSearch.val();
            var elem = $(e.currentTarget);

            // if (elem.scrollTop() + elem.innerHeight() >= e.currentTarget.scrollHeight)
            // {

            eleScrollHeight = elem[0].scrollHeight;
            eleScrollTop = elem.scrollTop();
            eleOutterHeight = elem.outerHeight();

            if (elem[0].scrollHeight - elem.scrollTop() - 10 == elem.outerHeight() - 10) {



                if ($ulMid.find("li").length > 0) {
                    pageNoForChild += 1;
                    $scrollBottom = false;
                    $bsyDivforbottom[0].style.visibility = "visible";
                    GetDataTreeNodeSelect(getParentID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                    $bsyDivforbottom[0].style.visibility = "hidden";

                    if ($ulMid.find("li").length > 0) {
                        $recordeNotFound.css("display", "none");
                        $bsyDivforbottom[0].style.visibility = "hidden";
                    }
                }
            }
        };



        var selectedItemArray = [];
        function DeleteBottomDivData() {
            selectedItemArray = [];

            if ($ulMid.find("input:checked").length > 0) {

                VIS.ADialog.confirm("DoYouWantToUnlink", true, "", "Confirm", function (result) {
                    if (!result) {
                        $bsyDiv[0].style.visibility = "hidden";
                        return;
                        //cancel
                    }
                    else {

                        $bsyDiv[0].style.visibility = "visible";
                        // var selectedItem = $ulMid.find("input:selected");
                        window.setTimeout(function () {
                            var selectedItemss = [];

                            for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                                //var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                                var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("data-id");
                                if (bindornot == "false") {
                                    if ($.inArray(parseInt(getID), convertmenuArray) >= 0) {
                                        continue;
                                    }
                                }

                                selectedItemArray.push(getID);
                                selectedItemss.push(parseInt(getID));
                            }

                            bottomchildselectedID = selectedItemArray;

                            selectedItemArray = selectedItemArray.toString();

                            //if (bindornot == "false")
                            //{   
                            //var someFlag = true;
                            //for (var i = 0; i < selectedItemss.length; i++)
                            //{
                            //    if ($.inArray(selectedItemss[i], convertmenuArray) < 0)
                            //    {
                            //        DeleteNodeFromBottom($treeID, selectedItemArray);
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        VIS.ADialog.info("NeverDelete");
                            //        $bsyDiv[0].style.visibility = "hidden";
                            //        break;
                            //    }

                            //}
                            // }
                            // else {
                            if (selectedItemArray.length > 0) {
                                DeleteNodeFromBottom($treeID, selectedItemArray);
                            }
                            else {
                                //VIS.ADialog.info("NeverDelete");
                                VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);
                                $bsyDiv[0].style.visibility = "hidden";

                            }
                            ///}


                        }, 200);
                    }
                });


                //var msg = "" + VIS.Msg.getMsg("DeleteIt") + "";
                //var r = VIS.ADialog.ask(msg);
                //if (r == true)
                //{
                //    $bsyDiv[0].style.visibility = "visible";
                //    // var selectedItem = $ulMid.find("input:selected");

                //    for (var i = 0; i < $ulMid.find("input:checked").length; i++) {
                //        var getID = $($ulMid.find("input:checked")[i]).parent().find("li").attr("id");
                //        selectedItemArray.push(getID);
                //    }

                //    bottomchildselectedID = selectedItemArray;

                //    selectedItemArray = selectedItemArray.toString();

                //    DeleteNodeFromBottom($treeID, selectedItemArray);
                //}
            }

            $bsyDiv[0].style.visibility = "hidden";
        };

        function DeleteNodeFromBottom($treeID, selectedItemArray) {
            $bsyDiv[0].style.visibility = "visible";

            var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            table_id = executeDataSet(table_id, null, null);
            if (table_id.tables[0].rows.length > 0) {
                table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            }
            var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            tree = executeDataSet(tree, null, null);
            treeType = tree.tables[0].rows[0].cells["treetype"];
            var tbname = "";
            if (treeType == "PR") {
                tbname = "AD_TreeNodePR"
            }
            else if (treeType == "BP") {
                tbname = "AD_TreeNodeBP"
            }
            else if (treeType == "MM") {
                tbname = "AD_TreeNodeMM"
            }
            else {
                tbname = "AD_TreeNode"
            }

            getIDFromChildLevel = [];

            //GetChildCount();
            var strGetIDs = "";
            //for (var j = 0; j < $ulMid.find("input:checked").length ; j++) {                

            //    //FindChildsID(tbname, bottomchildselectedID[j], $treeID);
            //   // getIDFromChildLevel.push(bottomchildselectedID[j]);
            //}

            if ($ulMid.find("input:checked").length > 0) {
                selectedItemArray = selectedItemArray.toString();
                GetAllChildesIDs(tbname, selectedItemArray, $treeID)
            }

            if ($ulMid.find("input:checked").length == 0) {
                ////selectedItemArray.push(GetPIDforItems);

                //FindChildsID(tbname, GetPIDforItems, $treeID);
                // getIDFromChildLevel.push(GetPIDforItems);

                //selectedItemArray = GetPIDforItems.toString();
                GetAllChildesIDs(tbname, selectedItemArray, $treeID)

            }




            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/DeleteNodeFromBottom",
                type: 'Post',
                data: { nodeid: selectedItemArray, treeID: $treeID, menuArray: menuArray },
                success: function (data) {
                    var res = JSON.parse(data);
                    if (res == "") {
                        $chkAllCheckOrNot.prop("checked", false);
                        var getMidCheckBoxValue = true;

                        //if ($ulMid.find("input:checked").length == 0) {
                        //    $ulMid.find("li").find("li[id='" + GetPIDforItems + "']").parent().remove();
                        //    $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + GetPIDforItems + "']").parent().parent().parent());
                        //}



                        for (var i = 0; i < $ulMid.find("input:checked").length ; i++) {
                            getMidCheckBoxValue = false;

                            if (bindornot == "false") {
                                var id = $($ulMid.find("input:checked").parent().find("li")[i]).attr("data-id");
                                if ($.inArray(parseInt(id), convertmenuArray) >= 0) {
                                    continue;
                                }
                            }

                            //var dragMenunodearr = dragMenuNodeIDArray.toString();
                            //dragMenunodearr = dragMenunodearr.split(',');selectedItemArray

                            var dragMenunodearr = selectedItemArray.toString();
                            dragMenunodearr = dragMenunodearr.split(',');


                            //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + bottomchildselectedID[i] + "']").parent().parent().parent());
                            //var getLi = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent();

                            //var getLi = $ulRight.find("li").find("li[id='" + bottomchildselectedID[i] + "']").parent().parent();
                            //var getLi = $ulRight.find("li").find("li[data-id='" + bottomchildselectedID[i] + "']").parent().parent();

                            var getLi = $ulRight.find("li").find("li[data-id='" + dragMenunodearr[i] + "']").parent().parent();
                            getLi.find("input").prop("disabled", false);
                            getLi.find("i").removeClass("vis-tm-opacity");
                            getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                            getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                            getLi.find("input").css("cursor", "pointer");

                            if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                                $pathInfo.empty();
                                getLi.find("input").prop("checked", true);
                            }

                            if ($rightMenuDemand.val() == "Linked") {
                                getLi.remove();
                            }


                            //var selectedID = $($ulMid.find("input:checked").parent().find(".VIS-tm-MLi")[i]).attr("id");
                            var selectedID = $($ulMid.find("input:checked").parent().find(".VIS-tm-MLi")[i]).attr("data-id");

                            //var imgSource = $($($ulRight.find("li"))).find("li[id='" + selectedID + "']").parent().parent().find("img").attr("src");
                            //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + selectedID + "']").parent().parent().find("img");

                            var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedID + "']").parent().parent().find("img").attr("src");
                            var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedID + "']").parent().parent().find("img");

                            var setLinkedImage;
                            var src = imgSource;
                            if (src != null) {
                                src = src.substring(src.lastIndexOf("/") + 1);

                                if (src == "nonSummary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                    changeImgSource.attr("src", setLinkedImage);
                                }
                                else if (src == "summary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                    changeImgSource.attr("src", setLinkedImage);
                                    GetChildCounts(selectedID);

                                    $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + bottomchildselectedID[i] + "']").parent().parent().parent());

                                    //if ($getchildCount == true) {
                                    //    pageNo = 1;
                                    //    $ulRight.empty();
                                    //    searchRightext = $cmbRightSearch.val();
                                    //    $demandsMenu = $rightMenuDemand.val();
                                    //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                                    //    $ulMid.find("input:checked").parent().remove();
                                    //    $bsyDiv[0].style.visibility = "hidden";
                                    //    return;
                                    //}

                                    //if ($ulMid.find("input:checked").length > 15) {
                                    //    pageNo = 1;
                                    //    $ulRight.empty();
                                    //    searchRightext = $cmbRightSearch.val();
                                    //    $demandsMenu = $rightMenuDemand.val();
                                    //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                                    //    $ulMid.find("input:checked").parent().remove();
                                    //    $bsyDiv[0].style.visibility = "hidden";
                                    //    return;
                                    //}

                                    //FindChildsID(tbname, selectedItemArray[i], $treeID);
                                    //getIDFromChildLevel;
                                    for (var i = 0; i < getIDFromChildLevel.length; i++) {
                                        var selectedItem = getIDFromChildLevel[i];
                                        $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItem + "']").parent().parent().parent());


                                        $($ulMid.find("input:checked")[i]).parent().remove();

                                        //var getLiFromRight = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent();

                                        var getLiFromRight = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent();
                                        getLiFromRight.find("input").prop("disabled", false);
                                        getLiFromRight.removeClass("vis-tm-menuselectedcolor");
                                        getLiFromRight.find("i").removeClass("vis-tm-opacity");
                                        getLiFromRight.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                        getLiFromRight.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                        getLiFromRight.find("input").css("cursor", "pointer");


                                        //var imgSource = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                        //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent().find("img");

                                        var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                        var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent().find("img");
                                        var setLinkedImage;
                                        var src = imgSource;
                                        if (src != null) {
                                            src = src.substring(src.lastIndexOf("/") + 1);

                                            if (src == "nonSummary.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                            }
                                            else if (src == "summary.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                            }
                                            else if (src == "mWindow.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                            }
                                            else if (src == "folder.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                            }
                                        }
                                        changeImgSource.attr("src", setLinkedImage);
                                    }


                                }

                                //changeImgSource.attr("src", setLinkedImage);
                            }
                        }

                        if (getMidCheckBoxValue == true) {

                            //===================================================================================
                            if ($ulMid.find("input:checked").length == 0) {
                                //var getLi = $ulRight.find("li").find("li[id='" + GetPIDforItems + "']").parent().parent();

                                //var getLi = $ulRight.find("li").find("li[data-id='" + GetPIDforItems + "']").parent().parent();
                                var getLi = $ulRight.find("li").find("li[data-id='" + getIDFromChildLevel + "']").parent().parent();
                                getLi.find("input").prop("disabled", false);
                                getLi.find("i").removeClass("vis-tm-opacity");
                                getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                getLi.find("input").css("cursor", "pointer");

                                if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                                    $pathInfo.empty();
                                    getLi.find("input").prop("checked", true);
                                }

                                if ($rightMenuDemand.val() == "Linked") {
                                    getLi.remove();
                                }

                                //var imgSource = $($($ulRight.find("li"))).find("li[id='" + GetPIDforItems + "']").parent().parent().find("img").attr("src");
                                //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + GetPIDforItems + "']").parent().parent().find("img");

                                // var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + GetPIDforItems + "']").parent().parent().find("img").attr("src");
                                //var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + GetPIDforItems + "']").parent().parent().find("img");

                                var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + getIDFromChildLevel + "']").parent().parent().find("img").attr("src");
                                var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + getIDFromChildLevel + "']").parent().parent().find("img");


                                var setLinkedImage;
                                var src = imgSource;
                                if (src != null) {
                                    src = src.substring(src.lastIndexOf("/") + 1);

                                    if (src == "nonSummary.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                        changeImgSource.attr("src", setLinkedImage);
                                    }
                                    else if (src == "summary.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                        changeImgSource.attr("src", setLinkedImage);
                                        //GetChildCounts(GetPIDforItems);

                                        GetChildCounts(getIDFromChildLevel);

                                        $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + getIDFromChildLevel + "']").parent().parent().parent());
                                        //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + GetPIDforItems + "']").parent().parent().parent());

                                        //if ($getchildCount == true) {
                                        //    pageNo = 1;
                                        //    $ulRight.empty();
                                        //    searchRightext = $cmbRightSearch.val();
                                        //    $demandsMenu = $rightMenuDemand.val();
                                        //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                                        //    $ulMid.find("input:checked").parent().remove();
                                        //    $bsyDiv[0].style.visibility = "hidden";
                                        //    return;
                                        //}

                                        for (var i = 0; i < getIDFromChildLevel.length; i++) {
                                            var selectedItem = getIDFromChildLevel[i];
                                            $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItem + "']").parent().parent().parent());

                                            $($ulMid.find("input:checked")[i]).parent().remove();

                                            //var getLiFromRight = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent();

                                            var getLiFromRight = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent();
                                            getLiFromRight.find("input").prop("disabled", false);
                                            getLiFromRight.removeClass("vis-tm-menuselectedcolor");
                                            getLiFromRight.find("i").removeClass("vis-tm-opacity");
                                            getLiFromRight.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                            getLiFromRight.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                            getLiFromRight.find("input").css("cursor", "pointer");

                                            //var imgSource = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                            //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + selectedItem + "']").parent().parent().find("img");

                                            var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                            var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + selectedItem + "']").parent().parent().find("img");
                                            var setLinkedImage;
                                            var src = imgSource;
                                            if (src != null) {
                                                src = src.substring(src.lastIndexOf("/") + 1);

                                                if (src == "nonSummary.png") {
                                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                                }
                                                else if (src == "summary.png") {
                                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                                }
                                                else if (src == "mWindow.png") {
                                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                                }
                                                else if (src == "folder.png") {
                                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                                }
                                            }
                                            changeImgSource.attr("src", setLinkedImage);
                                        }
                                    }
                                }
                                //$ulMid.find("li").find("li[id='" + GetPIDforItems + "']").parent().remove();

                                //$ulMid.find("li").find("li[data-id='" + GetPIDforItems + "']").parent().remove();
                                //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + GetPIDforItems + "']").parent().parent().parent());

                                $ulMid.find("li").find("li[data-id='" + getIDFromChildLevel + "']").parent().remove();
                                $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + getIDFromChildLevel + "']").parent().parent().parent());


                            }
                        }


                        //---------------------------------------------


                        $deleteChild.addClass("vis-tm-delete");

                        var selectedArrayOndel = [];
                        for (var t = 0; t < $ulMid.find("input:checked").length ; t++) {
                            var id = $($ulMid.find("input:checked").parent().find("li")[t]).attr("data-id");
                            if (bindornot == "false") {
                                if ($.inArray(parseInt(id), convertmenuArray) >= 0) {
                                    continue;
                                }
                            }
                            selectedArrayOndel.push(id);
                            //$ulMid.find("li").find("li[data-id='" + id + "']").parent().remove();
                        }

                        for (var a = 0; a < selectedArrayOndel.length ; a++) {
                            $ulMid.find("li").find("li[data-id='" + selectedArrayOndel[a] + "']").parent().remove();
                        }


                        if ($ulMid.find("input:checked").length > 0) {
                            VIS.ADialog.info("NeverDelete");
                        }



                        //    $ulMid.find("input:checked").parent().remove();

                        HasScrollarOrNot();
                    }

                    AllSelectChkValue();
                    AllSelectChkRightSide();
                    GetCountOnTreeChanges();
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });

            AllSelectChkValue();

        };


        function AllSelectChkValue() {
            if ($ulMid.find("input").length == 0) {
                $chkAllCheckOrNot.prop("disabled", true);
                $chkAllCheckOrNot.css("cursor", "not-allowed")
                $chkAllCheckOrNot.prop("checked", false);
                $deleteChild.addClass("vis-tm-delete");
                $squenceDailog.addClass("vis-tm-delete");
                isulMidData = false;
            }
            else {
                $squenceDailog.removeClass("vis-tm-delete");
                isulMidData = true;
                $chkAllCheckOrNot.prop("disabled", false);
                $chkAllCheckOrNot.css("cursor", "pointer");
            }
        };

        function FillValueInCombo() {
            var all = "" + VIS.Msg.getMsg("All") + "";
            var linked = "" + VIS.Msg.getMsg("Linked") + "";
            var unLinked = "" + VIS.Msg.getMsg("UnLinked") + "";

            $rightMenuDemand.append($("<Option value='All'>" + all + "</option>"));
            $rightMenuDemand.append($("<Option value='Linked'>" + linked + "</option>"));
            $rightMenuDemand.append($("<Option value='Unlinked'>" + unLinked + "</option>"));
        };

        function MenuWithDemands() {
            if ($cmbSelectTree.val() != "") {
                $pathInfo.empty();
                $ulRight.empty();
                $demandsMenu = $rightMenuDemand.val();
                window.setTimeout(function () {
                    $rightMenuDemand.prop("disabled", true);
                    treeCmbDisable.css("display", "inherit");
                    onscrollCheck = false;
                    $demandsMenu = $rightMenuDemand.val();
                    searchRightext = $cmbRightSearch.val();
                    // $ulRight.empty();
                    pageNo = 1;
                    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                }, 200);
            }
        };

        function ResizeBottomDiv(evt, ui) {
            //if ($(ui.originalElement[0].firstChild).hasClass("ui-resizable-n"))
            //{
            //    return;
            //}


            var currentheight = ui.size.height;
            $treeBackDiv.css("height", currentheight);


            $treeBackDiv.css("overflow-x", "none");
            // $treeBackDiv.css("overflow-y", "auto");
            $treeBackDiv.css("width", "100%");

            window.setTimeout(function () {
                $mData.height(leftMianDataDiv.height() - ($treeBackDiv.height() + $mTopHeader.height() + 45));
            }, 200);



            $secoundDiv.css("overflow", "hidden");
            $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height())

            nodeItemDelwithdrag.height($secoundDiv.height() - 10);

            topTreeDiv.height(currentheight - 10);
            $treeBackDiv.addClass("VIS-TM-resizediv");
            topTreeDiv.addClass("VIS-TM-resizediv");

        };

        var dailogContainer = null;
        var $chkLinkOrNot = null;
        function DailogDeleteDesign() {
            dailogContainer = $('<div>');
            $chkLinkOrNot = $('<input style="margin-right: 10px;" type="checkbox"/>');
            var $topMsg = $('<span>' + VIS.Msg.getMsg("WantToUnlinked") + '<span/>');
            var $askMsg = $('<span style="font-size: 16px;">' + VIS.Msg.getMsg("UnlinkedWithChild") + '<span/>');

            //  dailogContainer.append($('<div>').append($topMsg));
            //dailogContainer.append($('<div style="padding: 10px;">').append($chkLinkOrNot).append($askMsg));
            dailogContainer.append($('<div style="padding: 10px;">').append($askMsg));

        };


        function DailogForDelete(e) {
            chksearchvalues = e;
            TreeTableName();


            var findchilds = "Select node_id from " + tableTreeName + " where parent_id=" + $dragTreeDataNodeID + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
            var child = executeDataSet(findchilds, null, null);

            if (child != null && child.tables[0].rows.length == 0) {
                $chkValueFromDailog = true;

                $deleteArea.removeClass("VIS-TM-ondragdrop");
                $deleteArea.css("display", "none");
                $deleteImage.css("display", "none");

                if (bindornot == "false") {
                    if ($.inArray(parseInt(delNodId), convertmenuArray) >= 0) {
                        //VIS.ADialog.info("NeverDelete");
                        VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);
                        return;
                    }
                    else {
                        DeleteNodeFromTree($treeID, delNodId, getMovingdiv, $chkValueFromDailog);
                    }
                }
                else {
                    DeleteNodeFromTree($treeID, delNodId, getMovingdiv, $chkValueFromDailog);
                }


                return;
            }

            $bsyDiv[0].style.visibility = "hidden";
            DailogDeleteDesign();



            VIS.ADialog.confirm("ChildAlsoWillbeRemove", true, "", "Confirm", function (result) {
                if (!result) {
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                }
                else {
                    $chkValueFromDailog = true;
                    DeleteNodeFromTree($treeID, delNodId, getMovingdiv, $chkValueFromDailog);
                    $deleteArea.removeClass("VIS-TM-ondragdrop");
                    $deleteArea.css("display", "none");
                    $deleteImage.css("display", "none");
                }
            });



            //var createTab = new VIS.ChildDialog();
            ////  createTab.setHeight(150);
            //// createTab.setWidth(350);
            //createTab.setEnableResize(false);
            //createTab.setTitle(VIS.Msg.getMsg('UnlinkRecord'));
            //createTab.setModal(true);
            //createTab.setContent(dailogContainer);
            //createTab.show();
            //createTab.onClose = function () {

            //};
            //createTab.onOkClick = function (e) {
            //    //  if ($chkLinkOrNot.is(":checked")) {
            //    $chkValueFromDailog = true;
            //    //}
            //    //else {
            //    //    $chkValueFromDailog = false;
            //    //}
            //    DeleteNodeFromTree($treeID, delNodId, getMovingdiv, $chkValueFromDailog);
            //    //$ulMid.empty();
            //    $deleteArea.removeClass("VIS-TM-ondragdrop");
            //    $deleteArea.css("display", "none");
            //    $deleteImage.css("display", "none");
            //    // $treeRefresh.css("display", "inherit");
            //};
            //createTab.onCancelClick = function () {
            //    $deleteArea.removeClass("VIS-TM-ondragdrop");
            //    $deleteArea.css("display", "none");
            //    $deleteImage.css("display", "none");
            //    // $treeRefresh.css("display", "inherit");
            //};
        };


        var flagDelete = true;
        var gettbnameForAppentintree = null;
        function DeleteNodeFromTree($treeID, delNodId, getMovingdiv, $chkValueFromDailog) {
            if (delNodId != 0) {
                //var msg = "" + VIS.Msg.getMsg("VIS_TM_DeleteIt") + "";
                //var r = VIS.ADialog.ask(msg);
                //if (r == true)
                //{


                flagDelete = false;
                $bsyDiv[0].style.visibility = "visible";

                var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
                table_id = executeDataSet(table_id, null, null);
                if (table_id.tables[0].rows.length > 0) {
                    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
                }

                var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
                tablename = executeDataSet(tablename, null, null);
                if (tablename.tables[0].rows.length > 0) {
                    tablename = tablename.tables[0].rows[0].cells["tablename"];
                }
                var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                tree = executeDataSet(tree, null, null);
                treeType = tree.tables[0].rows[0].cells["treetype"];
                var tbname = "";
                // var tablename = "";
                if (treeType == "PR") {
                    tbname = "AD_TreeNodePR"
                }
                else if (treeType == "BP") {
                    tbname = "AD_TreeNodeBP"
                }
                else if (treeType == "MM") {
                    tbname = "AD_TreeNodeMM"
                }
                else {
                    tbname = "AD_TreeNode"
                }

                selectedItemArray = delNodId.toString();
                GetAllChildesIDs(tbname, selectedItemArray, $treeID)


                //if ($chkValueFromDailog == true)
                //{
                //    GetChildCounts(delNodId);
                //    if ($getchildCount == false)
                //    {
                //        FindChildsID(tbname, delNodId, $treeID);                     
                //    }                   
                //}

                //var sqlqry = "SELECT " + tbname + ".node_id AS node_id, mp.name as name,mp.issummary as summary  FROM " + tablename + " mp" +
                //             " INNER JOIN " + tbname + " " + tbname + " ON mp." + tablename + "_ID=" + tbname + ".node_id" +
                //             "  WHERE " + tbname + ".ad_tree_id=" + $treeID + " AND " + tbname + ".parent_id=" + delNodId;

                //sqlqry = VIS.DB.executeDataSet(sqlqry, null, null);


                //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                //tree = VIS.DB.executeDataSet(tree, null, null);
                //treeType = tree.tables[0].rows[0].cells["treetype"];
                //var tbname = "";
                //var tablename = "";
                //if (treeType == "PR") {
                //    tbname = "ad_treenodepr"
                //}
                //else if (treeType == "BP") {
                //    tbname = "ad_treenodebp"
                //}
                //else if (treeType == "MM") {
                //    tbname = "ad_treenodemm"
                //}
                //else {
                //    tbname = "ad_treenode"
                //}
                var getfirstParent = "select parent_ID  from " + tbname + " WHERE NODE_ID=" + delNodId + " AND AD_Tree_ID=" + $treeID + " AND IsActive='Y'";
                getfirstParent = executeDataSet(getfirstParent, null, null);

                $.ajax({
                    url: VIS.Application.contextUrl + "TreeMaintenance/DeleteNodeFromTree",
                    type: 'Post',
                    data: { nodeid: delNodId, treeID: $treeID, unlinkchild: $chkValueFromDailog, menuArray: menuArray },
                    success: function (data) {
                        var res = JSON.parse(data);
                        if (res == "") {
                            if ($ulMid.find("input:checked").length == 0) {
                                $chkAllCheckOrNot.prop("checked", false);
                            }

                            $deleteResult = res;
                            //$leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().remove();

                            //                            $ulMid.find("li").find("li[id='" + delNodId + "']").parent().remove();
                            $ulMid.find("li").find("li[data-id='" + delNodId + "']").parent().remove();

                            // var getLi = $ulRight.find("li").find("li[id='" + delNodId + "']").parent().parent();

                            var getLi = $ulRight.find("li").find("li[data-id='" + delNodId + "']").parent().parent();
                            getLi.find("input").prop("disabled", false);
                            getLi.find("i").removeClass("vis-tm-opacity");
                            getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                            getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                            getLi.find("input").css("cursor", "pointer");

                            if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                                $pathInfo.empty();
                                getLi.find("input").prop("checked", true);
                            }

                            //var imgSource = $($($ulRight.find("li"))).find("li[id='" + delNodId + "']").parent().parent().find("img").attr("src");
                            //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + delNodId + "']").parent().parent().find("img");

                            var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + delNodId + "']").parent().parent().find("img").attr("src");
                            var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + delNodId + "']").parent().parent().find("img");
                            var setLinkedImage;
                            var src = imgSource;
                            if (src != null) {
                                src = src.substring(src.lastIndexOf("/") + 1);

                                if (src == "nonSummary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                }
                                else if (src == "summary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                }

                                changeImgSource.attr("src", setLinkedImage);
                            }


                            //if ($chkValueFromDailog == true)
                            //{
                            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                            //tree = VIS.DB.executeDataSet(tree, null, null);
                            //treeType = tree.tables[0].rows[0].cells["treetype"];
                            //var tbname = "";
                            //var tablename = "";
                            //if (treeType == "PR") {
                            //    tbname = "ad_treenodepr"
                            //}
                            //else if (treeType == "BP") {
                            //    tbname = "ad_treenodebp"
                            //}
                            //else if (treeType == "MM") {
                            //    tbname = "ad_treenodemm"
                            //}
                            //else {
                            //    tbname = "ad_treenode"
                            //}

                            gettbnameForAppentintree = tbname;
                            if ($chkValueFromDailog == true) {
                                // FindChildsID(tbname, delNodId, $treeID);

                                //$leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().remove();
                                if (bindornot == "false") {

                                    GetIDsOnRolBindTree();

                                    //var draggingArrayss = [];
                                    //for (var w = 0; w < draggingItemsss.find(".treechild").length; w++)
                                    //{
                                    //    var idss = $(draggingItemsss.find(".treechild")[w]).attr("data-nodeid");
                                    //    draggingArrayss.push(parseInt(idss));
                                    //}

                                    //for (var s = 0; s < getIDforwithlongQuery.length; s++)
                                    //{
                                    //    if ($.inArray(parseInt(getIDforwithlongQuery[s]), convertmenuArray) >= 0)
                                    //    {
                                    //        var id = parseInt(getIDforwithlongQuery[s]);
                                    //        UpdateRollCheckSeq(id)
                                    //        var getrestrictIDss = $($($leftTreeDiv.find(".treechild")[0]).parent().parent().parent().find("ul")[0]);
                                    //        var matchResult = $leftTreeDiv.find("div").find("div[data-nodeid='" + id + "']").parent().parent().parent();
                                    //        $leftTreeDiv.find("div").find("div[data-nodeid='" + id + "']").parent().parent().parent().remove();
                                    //        getrestrictIDss.append(matchResult);
                                    //    }
                                    //}
                                }

                                $leftTreeDiv.data("kendoTreeView").remove(draggingItemsss);





                                //childlevel
                                //sqlqry


                                //if ($getchildCount == true) {
                                //    pageNo = 1;
                                //    $ulRight.empty();
                                //    searchRightext = $cmbRightSearch.val();
                                //    $demandsMenu = $rightMenuDemand.val();
                                //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                                //    $bsyDiv[0].style.visibility = "hidden";
                                //    return;
                                //}




                                //for (var i = 0; i < childlevel.tables[0].rows.length; i++) {
                                //    //childlevel.tables[0].rows[i].cells["node_id"]
                                //    sqlqryNodeID = childlevel.tables[0].rows[i].cells["node_id"];


                                for (var i = 0; i < getIDFromChildLevel.length; i++) {
                                    var sqlqryNodeID = getIDFromChildLevel[i];
                                    $ulMid.find("li").find("li[data-id='" + sqlqryNodeID + "']").parent().remove();

                                    if (bindornot == "false") {
                                        if ($.inArray(parseInt(sqlqryNodeID), convertmenuArray) >= 0) {
                                            continue;
                                        }
                                    }





                                    //$ulMid.find("li").find("li[id='" + sqlqryNodeID + "']").parent().remove();


                                    // var getLi = $ulRight.find("li").find("li[id='" + sqlqryNodeID + "']").parent().parent();
                                    var getLi = $ulRight.find("li").find("li[data-id='" + sqlqryNodeID + "']").parent().parent();
                                    getLi.find("input").prop("disabled", false);
                                    getLi.find("i").removeClass("vis-tm-opacity");
                                    getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                    getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                    getLi.find("input").css("cursor", "pointer");

                                    if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                                        getLi.find("input").prop("checked", true);
                                        $pathInfo.empty();
                                    }

                                    if ($rightMenuDemand.val() == "Linked") {
                                        getLi.remove();
                                    }

                                    //var imgSource = $($($ulRight.find("li"))).find("li[id='" + sqlqryNodeID + "']").parent().parent().find("img").attr("src");
                                    //var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + sqlqryNodeID + "']").parent().parent().find("img");

                                    var imgSource = $($($ulRight.find("li"))).find("li[data-id='" + sqlqryNodeID + "']").parent().parent().find("img").attr("src");
                                    var changeImgSource = $($($ulRight.find("li"))).find("li[data-id='" + sqlqryNodeID + "']").parent().parent().find("img");
                                    var setLinkedImage;
                                    var src = imgSource;
                                    if (src != null) {
                                        src = src.substring(src.lastIndexOf("/") + 1);

                                        if (src == "nonSummary.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                        }
                                        else if (src == "summary.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                        }

                                        changeImgSource.attr("src", setLinkedImage);
                                    }
                                }



                                //for (var i = 0; i < sqlqry.tables[0].rows.length; i++) {

                                //    sqlqryNodeID = sqlqry.tables[0].rows[i].cells["node_id"];

                                //    var getLi = $ulRight.find("li").find("li[id='" + sqlqryNodeID + "']").parent().parent();
                                //    getLi.find("input").prop("disabled", false);
                                //    getLi.find("img").removeClass("vis-tm-opacity");
                                //    getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                //    getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");


                                //    var imgSource = $($($ulRight.find("li"))).find("li[id='" + sqlqryNodeID + "']").parent().parent().find("img").attr("src");
                                //    var changeImgSource = $($($ulRight.find("li"))).find("li[id='" + sqlqryNodeID + "']").parent().parent().find("img");
                                //    var setLinkedImage;
                                //    var src = imgSource;
                                //    if (src != null)
                                //    {
                                //        src = src.substring(src.lastIndexOf("/") + 1);

                                //        if (src == "nonSummary.png") {
                                //            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                //        }
                                //        else if (src == "summary.png") {
                                //            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                //        }

                                //        changeImgSource.attr("src", setLinkedImage);
                                //    }
                                //}



                            }



                            if ($chkValueFromDailog == false) {
                                var ImageSource = null;

                                //$leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().remove();
                                //   $leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent();

                                var id = getfirstParent.tables[0].rows[0].cells["parent_id"];
                                $dropableItem = $leftTreeDiv.find("div").find("div[data-nodeid='" + id + "']");

                                //    var x = $($leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().find("ul")[0]);
                                //    $dropableItem.parent().parent().parent().parent().append(x);

                                // $leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().empty();
                                // $leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().remove();
                                //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().remove());

                                // $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + delNodId + "']").parent().parent().parent().parent().remove());


                                //$leftTreeDiv.data('kendoTreeView').dataSource.read();
                                //$dropableItem.parent().parent().parent().parent().css("padding-left", "16px");
                                // x.css("padding-left", "16px");

                                //for (var t = 0; t < sqlqry.tables[0].rows.length; t++)
                                //{
                                //    var action = sqlqry.tables[0].rows[t].cells["action"]
                                //    if (action == "R") {
                                //        ImageSource = "Areas/VIS/Images/mReport.png";
                                //    }
                                //    else if (action == "P") {
                                //        ImageSource = "Areas/VIS/Images/mProcess.png";
                                //    }
                                //    else if (action == "F") {
                                //        ImageSource = "Areas/VIS/Images/login/mWorkFlow.png";
                                //    }
                                //    else if (action == "B") {
                                //        ImageSource = "Areas/VIS/Images/login/mWorkbench.png";
                                //    }
                                //    else if (action == null) {
                                //        ImageSource = "Areas/VIS/Images/login/folder.png";
                                //    }
                                //    else {
                                //        ImageSource = "Areas/VIS/Images/mWindow.png";
                                //    }

                                //    var newChild = $leftTreeDiv.data("kendoTreeView").append({
                                //        text: VIS.Utility.encodeText(sqlqry.tables[0].rows[t].cells["name"]),
                                //        'IsSummary': sqlqry.tables[0].rows[t].cells["summary"],
                                //        'getparentnode': 0,
                                //        'TableName': gettbnameForAppentintree,
                                //        'NodeID':  sqlqry.tables[0].rows[t].cells["node_id"],
                                //        'TreeParentID': 0,
                                //        'ParentID': 0,
                                //        'ImageSource': ImageSource,
                                //    }, $dropableItem);
                                //}
                            }

                            HasScrollarOrNot();
                        }

                        if (movingNode.find("span").hasClass("k-state-selected")) {
                            $lblMh4.text(VIS.Msg.getMsg("SelectTree"));
                        }

                        AllSelectChkValue();
                        AllSelectChkRightSide();
                        GetCountOnTreeChanges();
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                    error: function (e) {
                        console.log(e);
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                });
                //}
                //else {
                //    $bsyDiv[0].style.visibility = "hidden";
                //    return;
                //}
            }
            else {
                VIS.ADialog.info("VIS_SelectTreeNode");
            }
        };


        function GetIDsOnRolBindTree() {
            var draggingArrayss = [];
            for (var w = 0; w < getIDforwithlongQuery.length; w++) {
                var idss = getIDforwithlongQuery[w];
                draggingArrayss.push(parseInt(idss));
            }

            for (var s = 0; s < draggingArrayss.length; s++) {
                if ($.inArray(draggingArrayss[s], convertmenuArray) >= 0) {
                    UpdateRollCheckSeq(draggingArrayss[s])
                    // var getrestrictIDss = $($($leftTreeDiv.find(".treechild")[0]).parent().parent().parent().find("ul")[0]);
                    var matchResult = $leftTreeDiv.find("div").find("div[data-nodeid='" + draggingArrayss[s] + "']").parent().parent().parent();
                    $leftTreeDiv.find("div").find("div[data-nodeid='" + draggingArrayss[s] + "']").parent().parent().parent().remove();
                    //getrestrictIDss.append(matchResult);

                    if (!$chkSummaryLevel.is(":checked")) {
                        var newChild = $leftTreeDiv.data("kendoTreeView").append({
                            text: VIS.Utility.encodeText(matchResult.text()),
                            'IsSummary': matchResult.find(".treechild").attr("data-issummary"),
                            'getparentnode': 0,
                            'TableName': matchResult.find(".treechild").attr("data-tablename"),
                            'NodeID': matchResult.find(".treechild").attr("data-nodeid"),
                            'TreeParentID': matchResult.find(".treechild").attr("data-nodeid"),
                            'ParentID': 0,//,
                            'ImageSource': "Areas/VIS/Images/mWindow.png",
                        }, $($($leftTreeDiv.find(".treechild")[0])));
                    }

                }
            }

            if ($cmbSearch.val() != "") {
                window.setTimeout(function () {
                    SearchNodeInTree(getEvalueforsearch);
                }, 200);
            }


        };




        //and tnp.isactive='Y' and tnp.ad_tree_id=0
        //long query
        var getIDforwithlongQuery = "";
        function GetAllChildesIDs(tbname, delNodId, $treeID) {
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "TreeMaintenance/SelectAllChildNodes",
                type: 'Post',
                async: false,
                data: { TableName: tbname, treeID: $treeID, nodeID: delNodId },
                success: function (data) {
                    $bsyDiv[0].style.visibility = "visible";
                    var result = JSON.parse(data);
                    getIDFromChildLevel = [];
                    if (result != null) {
                        getIDFromChildLevel = result.split(',');
                        getIDforwithlongQuery = getIDFromChildLevel;
                    }
                    //  $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
        };



        var getIDFromChildLevel = [];
        function FindChildsID(tbname, delNodId, $treeID) {
            //$.ajax({
            //    url: VIS.Application.contextUrl + "TreeMaintenance/SelectAllChildNodes",
            //    type: 'Post',
            //    data: { TableName: tbname, treeID: $treeID, nodeID: delNodId },
            //    success: function (data) {
            //        var result = JSON.parse(data);                    
            //    },
            //    error: function (e) {
            //        console.log(e);               
            //    },
            //});


            var childsId = "SELECT tnp.node_id FROM " + tbname + " tnp WHERE  tnp.isactive='Y' AND tnp.ad_tree_id= " + $treeID + "  AND tnp.parent_id = " + delNodId +

            " UNION " +
            "SELECT tnp.node_id FROM " + tbname + " tnp  WHERE tnp.parent_id IN" +
              "(SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.isactive='Y' AND tnp.ad_tree_id= " + $treeID + " AND tnp.parent_id = " + delNodId + "  )" + " AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID +

            " UNION " +
            "SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.parent_id IN" +
              "(SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.parent_id IN" +
                "(SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.isactive='Y' AND tnp.ad_tree_id= " + $treeID + " AND tnp.parent_id = " + delNodId + " )  AND tnp.isactive='Y'  AND tnp.ad_tree_id=" + $treeID + "   )  AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + " " +

            " UNION " +
            "SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.parent_id IN" +
            "(SELECT tnp.node_id  FROM " + tbname + " tnp  WHERE tnp.parent_id IN" +
                "(SELECT tnp.node_id FROM " + tbname + " tnp  WHERE tnp.parent_id IN" +
            "(SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.isactive='Y' AND tnp.ad_tree_id= " + $treeID + " AND tnp.parent_id = " + delNodId + "   ) AND tnp.isactive='Y'  AND tnp.ad_tree_id=" + $treeID + "   )  AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + " )" + "AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + "  AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + "    " +

            " UNION " +
            "SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.parent_id IN" +
            "(SELECT tnp.node_id  FROM " + tbname + " tnp  WHERE tnp.parent_id IN" +
                "(SELECT tnp.node_id  FROM " + tbname + " tnp   WHERE tnp.parent_id IN" +
                    "(SELECT tnp.node_id  FROM " + tbname + " tnp  WHERE tnp.parent_id IN" +
            "(SELECT tnp.node_id FROM " + tbname + " tnp WHERE tnp.isactive='Y' AND tnp.ad_tree_id= " + $treeID + " AND tnp.parent_id = " + delNodId + "    ) AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + "  ) AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + " ) AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + " ) AND tnp.isactive='Y'  AND tnp.ad_tree_id= " + $treeID + "  ";

            childlevel = executeDataSet(childsId, null, null);

            for (var i = 0; i < childlevel.tables[0].rows.length; i++) {
                var selectedItem = childlevel.tables[0].rows[i].cells["node_id"];

                getIDFromChildLevel.push(selectedItem);
            }
        };


        function RightPanelSearch() {
            if ($cmbSelectTree.val() != "") {
                $ulRight.empty();
                searchRightext = $cmbRightSearch.val();
                $demandsMenu = $rightMenuDemand.val();
                pageNo = 1;
                LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
            }

        };

        function RighPanelSearchOnEnter(e) {
            if (e.which != 13 && e.which != 8 && e.which != 46) {
                rightCrossImage.css("display", "inherit");
            }



            if (e.which == 8 || e.which == 46) {

                if ($cmbRightSearch.val() == "") {
                    rightCrossImage.css("display", "none");
                }

            }

            if (e.which == 13) {
                if ($cmbSelectTree.val() != "") {
                    $ulRight.empty();
                    searchRightext = $cmbRightSearch.val();
                    $demandsMenu = $rightMenuDemand.val();
                    pageNo = 1;
                    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                }
            }
        };



        function CmbRefresh(e) {
            if ($cmbSelectTree.val() != "") {
                treeCmbDisable.css("display", "inherit");
            }
            chksearchvalues = e;
            $cmbSelectTree.empty();
            GetTreeNameForCombo();
            changeSeqFlag = false;
            TreeRefresh();
            $squenceDailog.addClass("vis-tm-delete");

            $treeExpandColapse.css("display", "none");
            $treeCollapseColapse.css("display", "inherit");
            $treeCollapseColapse.css("float", "right");
            $lblMh4.text(VIS.Msg.getMsg("SelectTree"));

            AllSelectChkValue();

            // }
        };

        function TreeRefresh() {
            $ulMid.empty();
            expandCollapse = false;
            if ($cmbSelectTree.val() != "") {
                if ($chkSummaryLevel.is(":checked")) {
                    $isSummary = true;
                    if (selectedNode != null) {
                        TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                    }
                }
                else {
                    $isSummary = false;
                    if (selectedNode != null) {
                        TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                    }
                }
            }

            $secoundDiv.css("overflow", "hidden");

        };

        var getIvalueonEnter = 0;
        var nextIndex;
        function AnimateSearchNode(e) {
            animateSearchFlag = true;
            if (e.which == 13) {
                if ($cmbSearch.val() != "") {
                    if ($chkSummaryLevel.is(":checked")) {
                        $cmbSearch.prop("disabled", true);
                        $btnSearch.prop("disabled", true);
                    }
                }

                if (getTreeNodeChkValue == "true") {

                    if ($leftTreeDiv.find(".k-state-selected").length == 1) {


                        pageNoForChild = 1;
                        pageSizeForChild = 50;


                        if ($chktreeNode.is(":checked")) {
                            getTreeNodeChkValue = "true";
                        }
                        else {
                            getTreeNodeChkValue = "false";
                        }
                        var selectNodeText = $leftTreeDiv.find(".k-state-selected").find("div").attr("data-nodeid");
                        // $lblMh4.text($leftTreeDiv.find(".k-state-selected").find("div").text());

                        searchChildNode = $cmbSearch.val();
                        $ulMid.empty();
                        GetDataTreeNodeSelect(selectNodeText, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                    }


                }
                if ($cmbSearch.val() != "") {
                    if ($cmbSelectTree.val() != "") {
                        $deleteChild.removeClass("vis-tm-delete");
                        $bsyDiv[0].style.visibility = "visible";
                        $leftTreeKeno.find("span.k-in").find(".treechild").find("p").each(function (index) {
                            var text = $(this).text().toUpperCase();


                            //  treeValues.push($(this).parent().parent().attr("data-nodeid"));
                            //  searchedItem.push(text);


                            if (text == $cmbSearch.val().toUpperCase()) {
                                e.preventDefault();
                                var classname = $(this).parent().parent().parent().parent().parent().attr("class");
                                $leftTreeKeno.data("kendoTreeView").expand("." + classname);
                                $leftTreeKeno.find("span").removeClass("k-state-selected");
                                $(this).parent().parent().parent().addClass("k-state-selected");
                                this.parentNode.parentNode.parentNode.scrollIntoView();
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }
                        });

                        SearchNext(e);
                        $deleteChild.addClass("vis-tm-delete");
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                }
            }
            else {
                lastSeletedIndex = -1;
            }
        };

        var lastSeletedIndex = -1;
        var treeText;
        var textSearch;
        var calculateIndex;
        var calculateIndex;
        var searchNode = true;
        function SearchNext(e) {

            if ($chkSummaryLevel.is(":checked")) {
                $cmbSearch.prop("disabled", true);
                $btnSearch.prop("disabled", true);
            }


            treeText = searchedItem;
            textSearch = $cmbSearch.val();
            calculateIndex = containsTextInArray(treeText, textSearch, lastSeletedIndex);

            if (calculateIndex > -1) {
                var val = treeValues[calculateIndex];

                currentNode = val;
                lastSeletedIndex = calculateIndex;
                setSelectedNode(val, e);
            }
            else {
                searchNode = false;
                if ($cmbSearch.val() != "") {
                    $cmbSearch.prop("disabled", false);
                    $btnSearch.prop("disabled", false);
                }
            }
        }

        function containsTextInArray(arry, text, lastSelIndex) {
            for (var i = lastSelIndex + 1; i < arry.length; i++) {
                if (i == -1)
                    continue;
                if (arry[i].trim().toLowerCase().contains(text.trim().toLowerCase()))
                    return i;
            };
            if (lastSelIndex > -1) {
                lastSelIndex = -1;
                return containsTextInArray(arry, text, lastSelIndex);
            }
            return -1;
        };

        function setSelectedNode(nodeID, e) {
            if (nodeID != -1) {				//	new is -1
                currentNode = nodeID;
                return selectID(nodeID, true, e);     //  show selection
            }
            currentNode = 0;
            return false;
        };

        function selectID(nodeID, show, e) {
            if ($chktreeNode.is(":checked")) {
                $ulMid.find(".vis-tm-menuselectedcolor").removeClass("vis-tm-menuselectedcolor");
                $mData.find("li").find("li[id='" + nodeID + "']").parent().find("input").prop("checked", true);
                $mData.find("li").find("li[id='" + nodeID + "']").parent().addClass("vis-tm-menuselectedcolor");

                if ($mData.find("li").find("li[id='" + nodeID + "']").parent().hasClass("vis-tm-menuselectedcolor")) {
                    $mData.find("li").find("li[id='" + nodeID + "']").parent()[0].scrollIntoView();


                    $mData.height($secoundDiv.height() - ($mTopHeader.height() + 50));
                }
            }
            else {
                if (animateSearchFlag == true) {
                    $leftTreeDiv.find(".k-state-selected").removeClass("k-state-selected");
                    $($leftTreeDiv.find("div").find("div[data-nodeid='" + nodeID + "']").parent()).addClass("k-state-selected");
                    if ($($leftTreeDiv.find("div").find("div[data-nodeid='" + nodeID + "']").parent()).hasClass("k-state-selected")) {
                        $($leftTreeDiv.find("div").find("div[data-nodeid='" + nodeID + "']").parent()).addClass("k-state-selected")[0].scrollIntoView();
                        SelectedNodeColor();
                    }


                    window.setTimeout(function () {
                        pageNoForChild = 1;
                        pageSizeForChild = 50;

                        if ($chktreeNode.is(":checked")) {
                            searchChildNode = $cmbSearch.val();
                        }
                        else {
                            searchChildNode = "";
                        }
                        if ($chktreeNode.is(":checked")) {
                            getTreeNodeChkValue = "true";
                        }
                        else {
                            getTreeNodeChkValue = "false";
                        }
                        var selectNodeText = $leftTreeDiv.find("div").find("div[data-nodeid='" + nodeID + "']").parent().text();
                        $lblMh4.text(selectNodeText);
                        $ulMid.empty();


                        GetDataTreeNodeSelect(nodeID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                    }, 400);
                }



            }
            //var classname = $(this).parent().parent().parent().parent().parent().attr("class");
            //$leftTreeKeno.data("kendoTreeView").expand("." + classname);
            //$leftTreeKeno.find("span").removeClass("k-state-selected");
            //$(this).parent().parent().parent().addClass("k-state-selected");
            //this.parentNode.parentNode.parentNode.scrollIntoView();

        };

        function AnimateNodeWithButton(e) {
            if ($cmbSearch.val() != "") {
                $cmbSearch.prop("disabled", true);
                $btnSearch.prop("disabled", true);
            }

            if (getTreeNodeChkValue == "true") {

                if ($leftTreeDiv.find(".k-state-selected").length == 1) {
                    pageNoForChild = 1;
                    pageSizeForChild = 50;


                    if ($chktreeNode.is(":checked")) {
                        getTreeNodeChkValue = "true";
                    }
                    else {
                        getTreeNodeChkValue = "false";
                    }
                    var selectNodeText = $leftTreeDiv.find(".k-state-selected").find("div").attr("data-nodeid");
                    // $lblMh4.text($leftTreeDiv.find(".k-state-selected").find("div").text());

                    searchChildNode = $cmbSearch.val();
                    $ulMid.empty();
                    GetDataTreeNodeSelect(selectNodeText, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);


                    //searchChildNode = $cmbSearch.val();
                    //$ulMid.empty();
                    //GetDataTreeNodeSelect(getParentID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);
                }


            }
            if ($cmbSearch.val() != "") {
                $bsyDiv[0].style.visibility = "visible";

                $leftTreeKeno.find("span.k-in").find(".treechild").find("p").each(function (index) {
                    var text = $(this).text().toUpperCase();
                    if (text == $cmbSearch.val().toUpperCase()) {
                        e.preventDefault();
                        var classname = $(this).parent().parent().parent().parent().parent().attr("class");
                        $leftTreeKeno.data("kendoTreeView").expand("." + classname);
                        $leftTreeKeno.find("span").removeClass("k-state-selected");
                        $(this).parent().parent().parent().addClass("k-state-selected");
                        this.parentNode.parentNode.parentNode.scrollIntoView();
                    }
                });

                SearchNext(e);
                $bsyDiv[0].style.visibility = "hidden";
            }
        };

        var searchedItem = [];
        var treeValues = [];
        var chksearchvalues = null;


        var chksearchvalueflag = true;
        var getEvalueforsearch = null;

        function SearchNodeInTree(e) {
            // $leftTreeKeno.data("kendoTreeView").expand(".k-item")
            //   searchedItem = [];
            //var treeValues = [];


            //if ($cmbSearch.val() != "") {
            //    $checkSearchOrNot.css("display", "inline-block");
            //} else {
            //    $checkSearchOrNot.css("display", "none");
            //}
            if ($cmbSelectTree.val() != "") {
                crossImages.css("display", "inherit");

                if (chksearchvalueflag == true) {
                    chksearchvalueflag = false;
                    getEvalueforsearch = e;
                }


            }

            if ($cmbSelectTree.val() == "") {
                return false;
            }


            if ($($cmbSearch).val() == "" || !searchflag) {
                if ($($cmbSearch).val() == "") {
                    $ulMid.find("span").removeClass("VIS-TM-highlight");
                    $leftTreeDiv.find("span").removeClass("VIS-TM-highlight");
                    crossImages.css("display", "none");
                    animateSearchFlag = true;

                    chksearchvalueflag = true;
                }
                searchflag = true;
                return;
            }

            if (e != undefined) {

                if (e.which != 13) {


                    var term = $cmbSearch.val().toUpperCase();
                    var tlen = term.length;
                    if ($chktreeNode.is(":checked")) {
                        lastSeletedIndex = -1;
                        treeValues = [];
                        searchedItem = [];
                        $leftTreeDiv.find("span").removeClass("VIS-TM-highlight");


                        $ulMid.find("li li").each(function (index, val) {
                            var s = $(val);
                            var text = VIS.Utility.decodeText($(this).text());
                            treeValues.push(s.attr("id"));
                            searchedItem.push(text);

                            var html = '';
                            var q = 0;
                            if (term == "") {
                                if (text.toUpperCase().indexOf(term, q) == 0) {
                                    $(this).find("span").removeClass("VIS-TM-highlight");
                                    $ulMid.find(".vis-tm-menuselectedcolor").removeClass("vis-tm-menuselectedcolor");
                                    $deleteChild.addClass("vis-tm-delete");
                                    $ulMid.find("input").prop("checked", false);
                                }
                            }
                            else {
                                while ((p = text.toUpperCase().indexOf(term, q)) >= 0) {
                                    html += text.substring(q, p) + '<span class="VIS-TM-highlight">' + text.substr(p, tlen) + '</span>';
                                    q = p + tlen;
                                }

                                if (text.toUpperCase().indexOf(term, q) == -1) {
                                    $(this).find("span").removeClass("VIS-TM-highlight");
                                }

                                if (q > 0) {
                                    html += text.substring(q);
                                    //$(this).html(html);
                                }
                            }
                        });
                    }

                    else {
                        treeValues = [];
                        searchedItem = [];
                        $ulMid.find("li").find("span").removeClass("VIS-TM-highlight");

                        // for (var p = 0; p < 15; p++) {
                        $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                        // }

                        $treeCollapseColapse.css("display", "none");
                        $treeExpandColapse.css("display", "inherit");


                        if ($($cmbSearch).val() == "") {
                            $leftTreeKeno.find("span").removeClass("VIS-TM-highlight");
                        }

                        $('span.k-in > span.highlight').each(function () {
                            $(this).parent().text($(this).parent().text());
                        });

                        if ($.trim($cmbSearch.val()) == '') { return; }

                        var term = $cmbSearch.val().toUpperCase();
                        var tlen = term.length;

                        $leftTreeKeno.find("span.k-in").each(function (index, val) {
                            var s = $(val);
                            var text = VIS.Utility.decodeText($(this).text());
                            treeValues.push(s.find(".treechild").attr("data-nodeid"));
                            searchedItem.push(text);

                            var html = '';
                            var q = 0;
                            while ((p = text.toUpperCase().indexOf(term, q)) >= 0) {
                                html += text.substring(q, p) + '<span class="VIS-TM-highlight">' + text.substr(p, tlen) + '</span>';
                                q = p + tlen;
                            }

                            if (text.toUpperCase().indexOf(term, q) == -1) {
                                $(this).find("span").removeClass("VIS-TM-highlight");
                                //$leftTreeDiv.find(".k-state-selected").removeClass("k-state-selected");
                            }

                            if (q > 0) {
                                html += text.substring(q);
                                $(this).find("p").html(html);
                            }
                        });
                    }
                }
            }

        };

        function SelectedNodeColor() {
            $leftTreeDiv.find(".k-state-hover").removeClass("k-state-hover");
            $leftTreeDiv.find(".vis-tm-selectedkendoNode").removeClass("vis-tm-selectedkendoNode");
            $leftTreeDiv.find(".k-state-selected").find("p").addClass("vis-tm-selectedkendoNode");
        };


        function MidSelectDesign(e) {

            if ($(e.target).hasClass("summNonsumImage")) {
                $(e.target).parent().parent().find("li").removeClass("vis-tm-menuselectedcolor");
                $(e.target).parent().parent().find("input").prop("checked", false);
                $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().find("input").prop("checked", true);
                $deleteChild.removeClass("vis-tm-delete");
                var getlen = $ulMid.find("input").length;
                var chklen = $ulMid.find("input:checked").length;
                if (getlen == chklen) {
                    $chkAllCheckOrNot.prop("checked", true);
                }
            }

            if ($(e.target).hasClass("VIS-tm-MLi")) {
                $(this).find("li").removeClass("vis-tm-menuselectedcolor");
                $(this).find("li").find("input").prop("checked", false)
                $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().find("input").prop("checked", true);
                $deleteChild.removeClass("vis-tm-delete");
                $chkAllCheckOrNot.prop("checked", false);
                var getlen = $ulMid.find("input").length;
                var chklen = $ulMid.find("input:checked").length;
                if (getlen == chklen) {
                    $chkAllCheckOrNot.prop("checked", true);
                }
            }

            if ($(e.target).hasClass("VIS-tm-checkbox")) {

                if ($(e.target).parent().find("input").prop("checked")) {
                    $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                    $deleteChild.removeClass("vis-tm-delete");
                    var getlen = $ulMid.find("input").length;
                    var chklen = $ulMid.find("input:checked").length;
                    if (getlen == chklen) {
                        $chkAllCheckOrNot.prop("checked", true);
                    }
                }
                else {
                    $(e.target).parent().removeClass("vis-tm-menuselectedcolor");
                    $(e.target).parent().find("li").removeClass("vis-tm-menuselectedcolor");
                    $chkAllCheckOrNot.prop("checked", false);
                    if ($ulMid.find("input:checked").length > 0) {
                        $deleteChild.removeClass("vis-tm-delete");
                    }
                    else {
                        $deleteChild.addClass("vis-tm-delete");
                    }
                }
            }


        };
        var getSelectedLiImg = null;
        var existItss = -1;
        //var bottomchildselectedID = 0;
        var sellenContain = null;
        var traceNameFlag = true;

        function AllItemsSelectOrNots(e) {

            chksearchvalues = e;
            var sellenght = $ulRight.find("input:checked").length;

            var disabledlen = $ulRight.find("[disabled]").length;
            var totalinput = $ulRight.find("input").length;


            var enableInput = totalinput - disabledlen;

            if (sellenght == enableInput) {
                if (e != "disabled") {
                    $chkAllRightPannel.prop("checked", true);
                }
            }
            else {
                $chkAllRightPannel.prop("checked", false);
            }


        };

        function TouchStartStop(events) {
            //events.bind('touchstart', function (event) {                
            //    events.preventDefault();
            //    alert("touchStart");
            //    //DragMenu();

            //});
            //events.bind('touchend', function (event) {      
            // $($ulRight.find("li")).draggable("destroy");
            //alert("touchend");
            //});



            //events.bind('taphold', function (event) {
            //    events.preventDefault();
            //    alert("taphold");               
            //    //DragMenu();
            //});

        };

        function GetNodePath(e) {
            chksearchvalues = e;
            var timeoutLongTouch;
            var $mydiv = $ulRight;

            //// Listen to mousedown event
            //$mydiv.on('mousedown.LongTouch', function () {
            //    timeoutLongTouch = setTimeout(function () {
            //        $mydiv.trigger('contextmenu');
            //    }, 1000);
            //})
            //// Listen to mouseup event
            //.on('mouseup.LongTouch', function () {
            //    // Prevent long touch 
            //    clearTimeout(timeoutLongTouch);
            //});



            //var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
            //if (ismobile) {
            //    $ulRight.find("li").on("taphold", function (e) {
            //        // e.stopPropagation();                     
            //        e.preventDefault();
            //        alert("hello");
            //        var pressTimer

            //        $ulRight.find("span").mouseup(function (e) {
            //            clearTimeout(pressTimer)
            //            alert("mouseup");
            //            return false;
            //        }).mousedown(function () {
            //            pressTimer = window.setTimeout(function (e) {
            //                e.preventDefault();
            //                alert("timer");
            //                alert("ok");

            //            }, 1000)
            //            return false;
            //        });
            //    });
            //};











            $bsyDiv[0].style.visibility = "visible";
            // $($leftTreeDiv.find(".k-state-selected")).removeClass("k-state-selected");

            if ($ulRight.find("input:checked").length > 1) {
                $pathInfo.empty();
            }

            if ($(e.target).hasClass("VIS-Tm-glyphiconglyphicon-link")) {
                //var msg = "" + VIS.Msg.getMsg("VIS_TM_DeleteIt") + "";
                //var r = VIS.ADialog.ask(msg);
                //  if (r == true)
                // {
                selectedItemArray = [];
                var parent = $(e.target).parent();
                var getID = $(e.target).parent().attr("id")
                selectedItemArray.push(getID);



                selectedItemArray = selectedItemArray.toString();
                window.setTimeout(function () {
                    chksearchvalues = e;
                }, 200);

                if (bindornot == "false") {
                    if ($.inArray(parseInt(getID), convertmenuArray) < 0) {
                        DeleteNodeWithIcon($treeID, selectedItemArray, parent);//not found
                    }
                    else {
                        //VIS.ADialog.info("NeverDelete");

                        VIS.ADialog.info("NeverDelete", true, msgShowforbindingWindow);


                        $bsyDiv[0].style.visibility = "hidden";
                    }
                }
                else {
                    DeleteNodeWithIcon($treeID, selectedItemArray, parent);
                }

                // }
                //  else
                //   {

                return;
                //      $bsyDiv[0].style.visibility = "hidden";
                // }
            }


            if ($(e.target).hasClass("vis-tm-textli")) {
                $(this).find("input").prop("checked", false);
                $(this).find("li").removeClass("vis-tm-menuselectedcolor");
                $(e.target).parent().parent().addClass("vis-tm-menuselectedcolor");
                $($(e.target).parent().parent()).find("input").prop("checked", true);

                getSelectedLiImg = $($(e.target).parent().parent()).find("img").attr("src");

                e.target.id = $(e.target).attr("id");

                if ($(e.target).parent().parent().find("input").attr("disabled") == "disabled") {
                    $($(e.target).parent().parent()).find("input").prop("checked", false);
                }

                $chkAllRightPannel.prop("checked", false);

                AllItemsSelectOrNots($(e.target).parent().parent().find("input").attr("disabled"));



                if (ismobile) {

                    TouchStartStop($(e.target).parent().parent());


                }




                //else {
                //    $(this).find("input").prop("checked", false);
                //    $(this).find("li").removeClass("vis-tm-menuselectedcolor");
                //    $(e.target).parent().parent().addClass("vis-tm-menuselectedcolor");
                //    $($(e.target).parent().parent()).find("input").prop("checked", true);
                //}
            }

            if ($(e.target).hasClass("vis-tm-upperLi")) {
                $(e.target).parent().find("li").removeClass("vis-tm-menuselectedcolor");
                $(e.target).addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().find("input").prop("checked", false)
                $(e.target).find("input").prop("checked", true);

                getSelectedLiImg = $(e.target).find("img").attr("src");

                e.target.id = $(e.target).find(".vis-tm-textli").attr("id");

                if ($(e.target).find("input").attr("disabled") == "disabled") {
                    $(e.target).find("input").prop("checked", false);
                }
                $chkAllRightPannel.prop("checked", false);
                AllItemsSelectOrNots($(e.target).find("input").attr("disabled"));




                if (ismobile) {

                    TouchStartStop($(e.target));
                }




                //else {
                //    $(e.target).parent().find("li").removeClass("vis-tm-menuselectedcolor");
                //    $(e.target).addClass("vis-tm-menuselectedcolor");
                //    $(e.target).parent().find("input").prop("checked", false);
                //    $(e.target).find("input").prop("checked", true);
                //}
            }
            if ($(e.target).hasClass("vis-tm-upperdivchkandimg")) {
                $(e.target).parent().parent().find("li").removeClass("vis-tm-menuselectedcolor");
                $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().parent().find("input").prop("checked", false);
                $(e.target).find("input").prop("checked", true);

                getSelectedLiImg = $(e.target).find("img").attr("src");

                e.target.id = $(e.target).parent().find(".vis-tm-textli").attr("id");

                if ($(e.target).find("input").attr("disabled") == "disabled") {
                    $(e.target).find("input").prop("checked", false);
                }
                //else {
                //    $(e.target).parent().parent().find("li").removeClass("vis-tm-menuselectedcolor");
                //    $(e.target).parent().parent().find("input").prop("checked", false);
                //    $(e.target).parent().addClass("vis-tm-menuselectedcolor");
                //    $(e.target).find("input").prop("checked", true);
                //}
                $chkAllRightPannel.prop("checked", false);
                AllItemsSelectOrNots($(e.target).find("input").attr("disabled"));



                if (ismobile) {
                    TouchStartStop($(e.target).parent().parent());

                }
            }

            if ($(e.target).hasClass("vis-tm-chkbox")) {
                $bsyDiv[0].style.visibility = "visible";

                if ($(e.target).attr("disabled") != "disabled") {
                    if ($(e.target).prop("checked")) {
                        $(e.target).parent().parent().addClass("vis-tm-menuselectedcolor");
                        e.target.id = e.target.id = $(e.target).parent().parent().find(".vis-tm-textli").attr("id");

                        getSelectedLiImg = $(e.target).parent().parent().find("img").attr("src");

                        for (var i = 0; i < $ulRight.find(".vis-tm-menuselectedcolor").length; i++) {
                            if ($($ulRight.find(".vis-tm-menuselectedcolor").find("input")[i]).attr("disabled") == "disabled") {
                                $($ulRight.find(".vis-tm-menuselectedcolor").find("input")[i]).parent().parent().removeClass("vis-tm-menuselectedcolor");
                            }
                        }


                        var sellenght = $ulRight.find("input:checked").length;

                        var disabledlen = $ulRight.find("[disabled]").length;
                        var totalinput = $ulRight.find("input").length;


                        var enableInput = totalinput - disabledlen;

                        if (sellenght == enableInput) {
                            $chkAllRightPannel.prop("checked", true);
                        }
                        else {
                            $chkAllRightPannel.prop("checked", false);
                        }



                        //var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
                        //if (ismobile) {
                        //    $ulRight.find(".vis-tm-menuselectedcolor").find("li").draggable({});
                        //    $ulRight.find(".vis-tm-menuselectedcolor").draggable({});
                        //}



                        //DragMenu();



                    }
                    else {
                        $(e.target).parent().parent().removeClass("vis-tm-menuselectedcolor");
                        getSelectedLiImg = $(e.target).parent().parent().find("img").attr("src");
                        e.target.id = 0;

                        $chkAllRightPannel.prop("checked", false);

                    }
                }
                $bsyDiv[0].style.visibility = "hidden";




                if (ismobile) {
                    TouchStartStop($(e.target).parent().parent());



                }


            }


            if ($(e.target).hasClass("summNonsumImage")) {
                $(e.target).parent().parent().parent().find("li").removeClass("vis-tm-menuselectedcolor");
                $(e.target).parent().parent().addClass("vis-tm-menuselectedcolor");
                $(e.target).parent().parent().parent().find("input").prop("checked", false)
                $(e.target).parent().find("input").prop("checked", true);

                getSelectedLiImg = $(e.target).parent().find("img").attr("src");

                e.target.id = $(e.target).parent().parent().find(".vis-tm-textli").attr("id");

                if ($(e.target).parent().find("input").attr("disabled") == "disabled") {
                    $(e.target).parent().find("input").prop("checked", false);
                }
                $chkAllRightPannel.prop("checked", false);
                //else {
                //    $(e.target).parent().parent().parent().find("li").removeClass("vis-tm-menuselectedcolor");
                //    $(e.target).parent().parent().addClass("vis-tm-menuselectedcolor");
                //    $(e.target).parent().parent().parent().find("input").prop("checked", false)
                //    $(e.target).parent().find("input").prop("checked", true);
                //}
                AllItemsSelectOrNots($(e.target).parent().find("input").attr("disabled"));


                if (ismobile) {
                    TouchStartStop($(e.target).parent());
                }


            }


            if (e.target.id == "" || e.target.id == "undefined") {
                e.target.id = $(e.target).find(".vis-tm-menuselectedcolor").find("li").attr("id");
            }



            //  if ($(e.target).hasClass("vis-tm-textli")) {
            //$ulRight.find("input:checked").length
            if ($ulRight.find(".vis-tm-menuselectedcolor").length == 1 && $ulRight.find(".vis-tm-menuselectedcolor").find("li").hasClass("vis-tm-opacity") && (e.target.id != "")) {
                $.ajax({
                    url: VIS.Application.contextUrl + "TreeMaintenance/GetNodePath",
                    type: 'Post',
                    data: { node_ID: e.target.id, treeID: $treeID },
                    success: function (data) {
                        $chkAllRightPannel.prop("checked", false);
                        var res = JSON.parse(data);
                        res = res.replaceAll("\\\\", " \\\ ");
                        $pathInfo.empty();
                        $pathInfo.append(VIS.Utility.encodeText(res));

                        if ($chkTrace.is(":checked")) {
                            $chkAllCheckOrNot.prop("checked", false);
                            var selectedListId = $(e.target).parent().parent().find("li").attr("id");

                            var srcForSetLable = $(e.target).parent().parent().find("img").attr("src");

                            srcForSetLable = srcForSetLable.substring(srcForSetLable.lastIndexOf("/") + 1);

                            if (srcForSetLable == "summary.png") {
                                var textforlable = $(e.target).text();
                                textforlable = textforlable.substring(0, textforlable.lastIndexOf("("));
                                $lblMh4.text(textforlable);
                                existItss = -1;
                            }

                            getSelectedLiImg.lastIndexOf("/")

                            getSelectedLiImg = getSelectedLiImg.substring(getSelectedLiImg.lastIndexOf("/") + 1);

                            if (getSelectedLiImg != "mWindow.png" && getSelectedLiImg != "folder.png") {
                                if ($chkSummaryLevel.is(":checked")) {
                                    if ($chkTrace.is(":checked")) {
                                        $bsyDiv[0].style.visibility = "visible";
                                        if (getSelectedLiImg != "nonSummary.png") {
                                            // for (var p = 0; p < 15; p++) {
                                            $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                                            // }
                                        }
                                        else {
                                            TreeTableName();
                                            var findparent = "Select parent_id from " + tableTreeName + " where node_id=" + e.target.id + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
                                            var parent = executeDataSet(findparent, null, null);

                                            if (parent != null && parent.tables[0].rows.length > 0) {
                                                var parent_id = parent.tables[0].rows[0].cells["parent_id"];

                                                if (parent_id == null) {
                                                    parent_id = 0;
                                                }

                                                if (existItss != parent_id) {
                                                    $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                                                    window.setTimeout(function () {
                                                        $($leftTreeDiv.find(".k-state-selected")).removeClass("k-state-selected");
                                                        $($leftTreeDiv.find("div").find("div[data-nodeid='" + parent_id + "']").parent()).addClass("k-state-selected")[0].scrollIntoView();

                                                        var textforlable = $($leftTreeDiv.find(".k-state-selected")).text();
                                                        $lblMh4.text(textforlable);
                                                        existItss = parent_id;
                                                        SelectedNodeColor();
                                                        //var textforlable = $($leftTreeDiv.find(".k-state-selected")).text();
                                                        ////textforlable = textforlable.substring(0, textforlable.lastIndexOf("("));
                                                        //$lblMh4.text(textforlable);


                                                    }, 200);
                                                    $ulMid.empty();
                                                    pageNoForChild = 1;
                                                    pageSizeForChild = 50;

                                                    searchChildNode = $cmbSearch.val();
                                                    if ($chktreeNode.is(":checked")) {
                                                        getTreeNodeChkValue = "true";
                                                    }
                                                    else {
                                                        getTreeNodeChkValue = "false";
                                                    }

                                                    GetDataTreeNodeSelect(parent_id, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);

                                                    existItss = parent_id;
                                                }

                                            }

                                        }
                                        $bsyDiv[0].style.visibility = "hidden";
                                    }
                                }
                                else {
                                    // for (var p = 0; p < 15; p++) {
                                    $leftTreeKeno.data("kendoTreeView").expand(".k-item");
                                    // }
                                }
                            }

                            if ($($leftTreeDiv.find("div").find("div[data-nodeid='" + e.target.id + "']").parent()).addClass("k-state-selected").length != 0) {
                                $($leftTreeDiv.find(".k-state-selected")).removeClass("k-state-selected");
                                $($leftTreeDiv.find(".k-state-focused")).removeClass("k-state-focused");

                                window.setTimeout(function () {
                                    $($leftTreeDiv.find("div").find("div[data-nodeid='" + e.target.id + "']").parent()).addClass("k-state-selected")[0].scrollIntoView();
                                    SelectedNodeColor();
                                }, 200);

                                $ulMid.empty();

                                pageNoForChild = 1;
                                pageSizeForChild = 50;

                                searchChildNode = $cmbSearch.val();
                                if ($chktreeNode.is(":checked")) {
                                    getTreeNodeChkValue = "true";
                                }
                                else {
                                    getTreeNodeChkValue = "false";
                                }
                                GetDataTreeNodeSelect(e.target.id, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);

                                //GetDataTreeNodeSelect(getParentID, $treeID, pageNoForChild, pageSizeForChild, searchChildNode, getTreeNodeChkValue, e);

                            }
                            else {
                                if ($ulRight.find(".vis-tm-menuselectedcolor").length == 1) {
                                    var src = $ulRight.find(".vis-tm-menuselectedcolor").find("img").attr("src");
                                    src = src.substring(src.lastIndexOf("/") + 1);
                                    if (src != "nonSummary.png" && src != "summary.png") {
                                        $pathInfo.empty();
                                    }
                                    else {

                                    }
                                    //else if (src != "summary.png") {
                                    //    $pathInfo.empty();
                                    //}
                                }

                            }
                        }
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                    error: function (e) {
                        console.log(e);
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                });
            }
            else {
                $pathInfo.empty();
            }
            $bsyDiv[0].style.visibility = "hidden";
        };

        function GetChildCounts(selectedItemArray) {
            var getfullIL = $leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent();
            if (getfullIL.children("ul").length > 0) {
                if ($($(getfullIL.children("ul")[0]).find("ul")[0]).length > 0) {
                    if ($($($(getfullIL.children("ul")[0]).find("ul")[0]).find("ul")[0]).length > 0) {
                        if ($($($($(getfullIL.children("ul")[0]).find("ul")[0]).find("ul")[0]).find("ul")[0]).length > 0) {
                            if ($($($($($(getfullIL.children("ul")[0]).find("ul")[0]).find("ul")[0]).find("ul")[0]).find("ul")[0]).length > 0) {
                                $getchildCount = true;
                            }
                            else {
                                $getchildCount = false;
                            }
                        }
                        else {
                            $getchildCount = false;
                        }
                    }
                    else {
                        $getchildCount = false;
                    }
                }
                else {
                    $getchildCount = false;
                }
            }
        };


        var delNodWithIcon = true;
        function DeleteNodeWithIcon($treeID, selectedItemArray, parent) {
            $bsyDiv[0].style.visibility = "visible";


            var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            table_id = executeDataSet(table_id, null, null);
            if (table_id.tables[0].rows.length > 0) {
                table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            }

            var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            tree = executeDataSet(tree, null, null);
            treeType = tree.tables[0].rows[0].cells["treetype"];
            var tbname = "";
            if (treeType == "PR") {
                tbname = "AD_TreeNodePR"
            }
            else if (treeType == "BP") {
                tbname = "AD_TreeNodeBP"
            }
            else if (treeType == "MM") {
                tbname = "AD_TreeNodeMM"
            }
            else {
                tbname = "AD_TreeNode"
            }

            var findchild = "Select node_id from " + tbname + " where parent_id=" + selectedItemArray + " AND isactive='Y' and ad_tree_id=" + $treeID + "";
            var child = executeDataSet(findchild, null, null);

            if (child != null && child.tables[0].rows.length > 0) {
                VIS.ADialog.confirm("ChildAlsoWillbeRemove", true, "", "Confirm", function (result) {
                    if (!result) {
                        $bsyDiv[0].style.visibility = "hidden";
                        return;
                        //cancel
                    }
                    else {

                        //GetChildCounts(selectedItemArray);
                        // if ($getchildCount == false)
                        // {
                        selectedItemArray = selectedItemArray.toString();
                        GetAllChildesIDs(tbname, selectedItemArray, $treeID)

                        // FindChildsID(tbname, selectedItemArray, $treeID);
                        // }





                        $.ajax({
                            url: VIS.Application.contextUrl + "TreeMaintenance/DeleteNodeFromBottom",
                            type: 'Post',
                            data: { nodeid: selectedItemArray, treeID: $treeID, menuArray: menuArray },
                            success: function (data) {
                                var res = JSON.parse(data);
                                if (res == "") {

                                    if ($ulMid.find("input:checked").length == 0) {
                                        $chkAllCheckOrNot.prop("checked", false);
                                    }

                                    $pathInfo.empty();
                                    //var getLi = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent();

                                    var getLi = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent();
                                    getLi.find("input").prop("disabled", false);
                                    getLi.find("i").removeClass("vis-tm-opacity");
                                    getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                    getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                    getLi.find("input").css("cursor", "pointer");
                                    if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                                        $pathInfo.empty();
                                        getLi.find("input").prop("checked", true);
                                    }


                                    if (bindornot == "false") {
                                        GetIDsOnRolBindTree();
                                        //for (var f = 0; f < getIDforwithlongQuery.length; f++)
                                        //{
                                        //    if ($.inArray(parseInt(getIDforwithlongQuery[f]), convertmenuArray) >= 0)
                                        //    {
                                        //        UpdateRollCheckSeq(getIDforwithlongQuery[f]);
                                        //        var getrestrictIDss = $($($leftTreeDiv.find(".treechild")[0]).parent().parent().parent().find("ul")[0]);
                                        //        var matchResult = $leftTreeDiv.find("div").find("div[data-nodeid='" + getIDforwithlongQuery[f] + "']").parent().parent().parent();
                                        //        $leftTreeDiv.find("div").find("div[data-nodeid='" + getIDforwithlongQuery[f] + "']").parent().parent().parent().remove();
                                        //        getrestrictIDss.append(matchResult);
                                        //    }
                                        //}
                                    }






                                    //var getchildss = $leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().find(".treechild");

                                    //for (var f = 0; f < getchildss.length; f++)
                                    //{
                                    //    var idss = $(getchildss[f]).attr("data-nodeid");

                                    //    if (bindornot == "false")
                                    //    {
                                    //        if ($.inArray(parseInt(idss), convertmenuArray) >= 0)
                                    //        {
                                    //            UpdateRollCheckSeq(idss);
                                    //            var getrestrictIDss = $($($leftTreeDiv.find(".treechild")[0]).parent().parent().parent().find("ul")[0]);
                                    //            var matchResult = $leftTreeDiv.find("div").find("div[data-nodeid='" + idss + "']").parent().parent().parent();
                                    //            $leftTreeDiv.find("div").find("div[data-nodeid='" + idss + "']").parent().parent().parent().remove();
                                    //            getrestrictIDss.append(matchResult);
                                    //        }
                                    //    }

                                    //}

                                    $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent());

                                    //  $leftTreeDiv.data("kendoTreeView").element.find("li:has(li a.sfSel)");


                                    //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove());

                                    // $leftTreeDiv.data('kendoTreeView').dataSource.read();

                                    // $leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove();

                                    ////////$ulMid.find("li").find("li[id='" + selectedItemArray + "']").parent().remove();
                                    $ulMid.find("li").find("li[data-id='" + selectedItemArray + "']").parent().remove();


                                    //var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
                                    //table_id = VIS.DB.executeDataSet(table_id, null, null);
                                    //if (table_id.tables[0].rows.length > 0) {
                                    //    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
                                    //}

                                    ////var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
                                    ////tablename = VIS.DB.executeDataSet(tablename, null, null);
                                    ////if (tablename.tables[0].rows.length > 0) {
                                    ////    tablename = tablename.tables[0].rows[0].cells["tablename"];
                                    ////}
                                    //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                                    //tree = VIS.DB.executeDataSet(tree, null, null);
                                    //treeType = tree.tables[0].rows[0].cells["treetype"];
                                    //var tbname = "";
                                    //// var tablename = "";
                                    //if (treeType == "PR") {
                                    //    tbname = "ad_treenodepr"
                                    //}
                                    //else if (treeType == "BP") {
                                    //    tbname = "ad_treenodebp"
                                    //}
                                    //else if (treeType == "MM") {
                                    //    tbname = "ad_treenodemm"
                                    //}
                                    //else {
                                    //    tbname = "ad_treenode"
                                    //}
                                    // if ($chkValueFromDailog == true) {

                                    // }

                                    //if ($getchildCount == true) {
                                    //    pageNo = 1;
                                    //    $ulRight.empty();
                                    //    searchRightext = $cmbRightSearch.val();
                                    //    $demandsMenu = $rightMenuDemand.val();
                                    //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                                    //    $bsyDiv[0].style.visibility = "hidden";
                                    //    return;
                                    //}


                                    //for (var i = 0; i < childlevel.tables[0].rows.length; i++) {
                                    //    var selectedItem = childlevel.tables[0].rows[i].cells["node_id"];


                                    for (var i = 0; i < getIDFromChildLevel.length; i++) {
                                        var selectedItem = getIDFromChildLevel[i];
                                        $ulMid.find("li").find("li[data-id='" + selectedItem + "']").parent().remove();
                                        if (bindornot == "false") {
                                            if ($.inArray(parseInt(selectedItem), convertmenuArray) >= 0) {
                                                continue;
                                            }
                                        }




                                        //var chkenable = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent();

                                        var chkenable = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent();
                                        chkenable.find("input").prop("disabled", false);
                                        chkenable.find("i").removeClass("vis-tm-opacity");
                                        chkenable.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                        chkenable.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                                        chkenable.find("input").css("cursor", "pointer");

                                        if (chkenable.hasClass("vis-tm-menuselectedcolor")) {
                                            $pathInfo.empty();
                                            chkenable.find("input").prop("checked", true);
                                        }

                                        if ($rightMenuDemand.val() == "Linked") {
                                            chkenable.remove();
                                        }

                                        //chkenable.removeClass("vis-tm-menuselectedcolor");

                                        // $ulRight.find(".vis-tm-menuselectedcolor").removeClass("vis-tm-menuselectedcolor");

                                        ////$ulMid.find("li").find("li[id='" + selectedItem + "']").parent().remove();


                                        //var imgSource = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                        //var changeImgSource = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent().find("img");

                                        var imgSource = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                        var changeImgSource = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent().find("img");
                                        var setLinkedImage;
                                        var src = imgSource;
                                        if (src != null) {
                                            src = src.substring(src.lastIndexOf("/") + 1);
                                            if (src == "nonSummary.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                            }
                                            else if (src == "summary.png") {
                                                setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                            }

                                            changeImgSource.attr("src", setLinkedImage);
                                        }
                                    }
                                    //var imgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img").attr("src");
                                    //var changeImgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img");

                                    var imgSource = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent().find("img").attr("src");
                                    var changeImgSource = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent().find("img");
                                    var setLinkedImage;
                                    var src = imgSource;
                                    if (src != null) {
                                        src = src.substring(src.lastIndexOf("/") + 1);
                                        if (src == "nonSummary.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                        }
                                        else if (src == "summary.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                        }
                                        else if (src == "folder.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                        }//mWindow.png
                                        else if (src == "mWindow.png") {
                                            setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                        }

                                        changeImgSource.attr("src", setLinkedImage);
                                    }

                                    if ($ulMid.find("li").length == 0 && !$leftTreeDiv.find("span").hasClass("k-state-selected")) {
                                        $lblMh4.text(VIS.Msg.getMsg("SelectTree"));
                                    }
                                    HasScrollarOrNot();
                                }
                                AllSelectChkValue();
                                AllSelectChkRightSide();
                                GetCountOnTreeChanges();
                                $bsyDiv[0].style.visibility = "hidden";
                            },
                            error: function (e) {
                                console.log(e);
                                $bsyDiv[0].style.visibility = "hidden";
                            },
                        });
                    }
                });
            }
            else {
                //GetChildCounts(selectedItemArray);
                //if ($getchildCount == false) {
                //    FindChildsID(tbname, selectedItemArray, $treeID);
                //}

                selectedItemArray = selectedItemArray.toString();
                GetAllChildesIDs(tbname, selectedItemArray, $treeID)

                //FindChildsID(tbname, selectedItemArray, $treeID);

                $.ajax({
                    url: VIS.Application.contextUrl + "TreeMaintenance/DeleteNodeFromBottom",
                    type: 'Post',
                    data: { nodeid: selectedItemArray, treeID: $treeID, menuArray: menuArray },
                    success: function (data) {
                        var res = JSON.parse(data);
                        if (res == "") {
                            $pathInfo.empty();
                            //var getLi = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent();

                            var getLi = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent();
                            getLi.find("input").prop("disabled", false);
                            getLi.find("i").removeClass("vis-tm-opacity");

                            //if (getLi.hasClass("vis-tm-menuselectedcolor")) {
                            //    getLi.find("input").prop("checked", true);
                            //}
                            //else {
                            //    getLi.removeClass("vis-tm-menuselectedcolor");
                            //}


                            getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                            getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");
                            getLi.find("input").css("cursor", "pointer");

                            $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent());

                            //  $leftTreeDiv.data("kendoTreeView").element.find("li:has(li a.sfSel)");


                            //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove());

                            // $leftTreeDiv.data('kendoTreeView').dataSource.read();

                            // $leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove();


                            /////$ulMid.find("li").find("li[id='" + selectedItemArray + "']").parent().remove();
                            $ulMid.find("li").find("li[data-id='" + selectedItemArray + "']").parent().remove();


                            //var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
                            //table_id = VIS.DB.executeDataSet(table_id, null, null);
                            //if (table_id.tables[0].rows.length > 0) {
                            //    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
                            //}

                            ////var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
                            ////tablename = VIS.DB.executeDataSet(tablename, null, null);
                            ////if (tablename.tables[0].rows.length > 0) {
                            ////    tablename = tablename.tables[0].rows[0].cells["tablename"];
                            ////}
                            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
                            //tree = VIS.DB.executeDataSet(tree, null, null);
                            //treeType = tree.tables[0].rows[0].cells["treetype"];
                            //var tbname = "";
                            //// var tablename = "";
                            //if (treeType == "PR") {
                            //    tbname = "ad_treenodepr"
                            //}
                            //else if (treeType == "BP") {
                            //    tbname = "ad_treenodebp"
                            //}
                            //else if (treeType == "MM") {
                            //    tbname = "ad_treenodemm"
                            //}
                            //else {
                            //    tbname = "ad_treenode"
                            //}
                            // if ($chkValueFromDailog == true) {

                            // }

                            //if ($getchildCount == true) {
                            //    pageNo = 1;
                            //    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                            //    $bsyDiv[0].style.visibility = "hidden";
                            //    return;
                            //}

                            //for (var i = 0; i < childlevel.tables[0].rows.length; i++) {
                            //    var selectedItem = childlevel.tables[0].rows[i].cells["node_id"];


                            for (var i = 0; i < getIDFromChildLevel.length; i++) {
                                var selectedItem = getIDFromChildLevel[i];


                                if (bindornot == "false") {
                                    if ($.inArray(parseInt(selectedItem), convertmenuArray) >= 0) {
                                        continue;
                                    }
                                }



                                //var chkenable = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent();

                                var chkenable = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent();
                                chkenable.find("input").prop("disabled", false);
                                chkenable.find("i").removeClass("vis-tm-opacity");
                                chkenable.find(".vis-tm-textli").removeClass("vis-tm-opacity");
                                chkenable.removeClass("vis-tm-menuselectedcolor");
                                //$rightMenuDemand.val();
                                if ($rightMenuDemand.val() == "Linked") {
                                    chkenable.remove();
                                }


                                //var imgSource = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                //var changeImgSource = $ulRight.find("li").find("li[id='" + selectedItem + "']").parent().parent().find("img");

                                var imgSource = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent().find("img").attr("src");
                                var changeImgSource = $ulRight.find("li").find("li[data-id='" + selectedItem + "']").parent().parent().find("img");
                                var setLinkedImage;
                                var src = imgSource;
                                if (src != null) {
                                    src = src.substring(src.lastIndexOf("/") + 1);
                                    if (src == "nonSummary.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                    }
                                    else if (src == "summary.png") {
                                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                    }

                                    changeImgSource.attr("src", setLinkedImage);
                                }
                            }
                            //var imgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img").attr("src");
                            //var changeImgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img");

                            var imgSource = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent().find("img").attr("src");
                            var changeImgSource = $ulRight.find("li").find("li[data-id='" + selectedItemArray + "']").parent().parent().find("img");
                            var setLinkedImage;
                            var src = imgSource;
                            if (src != null) {
                                src = src.substring(src.lastIndexOf("/") + 1);
                                if (src == "nonSummary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
                                }
                                else if (src == "summary.png") {
                                    setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
                                }

                                changeImgSource.attr("src", setLinkedImage);
                            }
                            HasScrollarOrNot();
                        }
                        if ($ulMid.find("li").length == 0 && !$leftTreeDiv.find("span").hasClass("k-state-selected")) {
                            $lblMh4.text(VIS.Msg.getMsg("SelectTree"));
                        }
                        AllSelectChkValue();
                        AllSelectChkRightSide();
                        GetCountOnTreeChanges();
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                    error: function (e) {
                        console.log(e);
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                });
            }




            //$.ajax({
            //    url: VIS.Application.contextUrl + "TreeMaintenance/DeleteNodeFromBottom",
            //    type: 'Post',
            //    data: { nodeid: selectedItemArray, treeID: $treeID },
            //    success: function (data) {
            //        var res = JSON.parse(data);
            //        if (res == "")
            //        {

            //            var getLi = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent();
            //            getLi.find("input").prop("disabled", false);
            //            getLi.find("img").removeClass("vis-tm-opacity");
            //            getLi.find(".vis-tm-textli").removeClass("vis-tm-opacity");
            //            getLi.find("span").removeClass("VIS-Tm-glyphiconglyphicon-link");

            //            $leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent());

            //          //  $leftTreeDiv.data("kendoTreeView").element.find("li:has(li a.sfSel)");


            //            //$leftTreeDiv.data("kendoTreeView").remove($leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove());

            //           // $leftTreeDiv.data('kendoTreeView').dataSource.read();

            //           // $leftTreeDiv.find("div").find("div[data-nodeid='" + selectedItemArray + "']").parent().parent().parent().remove();
            //            $ulMid.find("li").find("li[id='" + selectedItemArray + "']").parent().remove();



            //            //var table_id = "SELECT ad_table_id FROM ad_tree WHERE ad_tree_id=" + $treeID;
            //            //table_id = VIS.DB.executeDataSet(table_id, null, null);
            //            //if (table_id.tables[0].rows.length > 0) {
            //            //    table_id = table_id.tables[0].rows[0].cells["ad_table_id"];
            //            //}

            //            ////var tablename = "SELECT tablename FROM ad_table WHERE ad_table_id=" + table_id;
            //            ////tablename = VIS.DB.executeDataSet(tablename, null, null);
            //            ////if (tablename.tables[0].rows.length > 0) {
            //            ////    tablename = tablename.tables[0].rows[0].cells["tablename"];
            //            ////}
            //            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            //            //tree = VIS.DB.executeDataSet(tree, null, null);
            //            //treeType = tree.tables[0].rows[0].cells["treetype"];
            //            //var tbname = "";
            //            //// var tablename = "";
            //            //if (treeType == "PR") {
            //            //    tbname = "ad_treenodepr"
            //            //}
            //            //else if (treeType == "BP") {
            //            //    tbname = "ad_treenodebp"
            //            //}
            //            //else if (treeType == "MM") {
            //            //    tbname = "ad_treenodemm"
            //            //}
            //            //else {
            //            //    tbname = "ad_treenode"
            //            //}
            //           // if ($chkValueFromDailog == true) {

            //           // }


            //           // for (var i = 0; i < childlevel.tables[0].rows.length; i++)
            //           // {
            //                var sqlqryNodeID = childlevel.tables[0].rows[i].cells["node_id"];
            //                var imgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img").attr("src");
            //                var changeImgSource = $ulRight.find("li").find("li[id='" + selectedItemArray + "']").parent().parent().find("img");
            //                var setLinkedImage;
            //                var src = imgSource;
            //                if (src != null)
            //                {
            //                    src = src.substring(src.lastIndexOf("/") + 1);
            //                    if (src == "nonSummary.png") {
            //                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/mWindow.png";
            //                    }
            //                    else if (src == "summary.png") {
            //                        setLinkedImage = VIS.Application.contextUrl + "Areas/VIS/Images/folder.png";
            //                    }

            //                    changeImgSource.attr("src", setLinkedImage);
            //                }
            //           // }

            //        }

            //        $bsyDiv[0].style.visibility = "hidden";
            //    },
            //    error: function (e) {
            //        console.log(e);
            //        $bsyDiv[0].style.visibility = "hidden";
            //    },
            //});
        };

        var ChkSummayOnTreeChange = null;

        function GetCountForRestriction() {
            //var tree = " SELECT treetype FROM ad_tree WHERE ad_tree_id=" + $treeID;
            //tree = VIS.DB.executeDataSet(tree, null, null);
            //treeType = tree.tables[0].rows[0].cells["treetype"];
            //var tbname = "";
            //var tablename = "";
            //if (treeType == "PR") {
            //    tbname = "ad_treenodepr"
            //}
            //else if (treeType == "BP") {
            //    tbname = "ad_treenodebp"
            //}
            //else if (treeType == "MM") {
            //    tbname = "ad_treenodemm"
            //}
            //else {
            //    tbname = "ad_treenode"
            //}



            TreeTableName();


            var sqlQry = "SELECT Count(*) as Count FROM " + tableTreeName + " WHERE isactive ='Y' AND AD_Tree_ID=" + $treeID;
            sqlQry = executeDataSet(sqlQry, null, null);
            sqlQry = sqlQry.tables[0].rows[0].cells["count"];

            if (sqlQry > 5000) {
                $chkSummaryLevel.prop("checked", true);
                $chkSummaryLevel.prop("disabled", true);


                $bsyDivforbottom[0].style.visibility = "hidden";
                $isSummary = true;
                ChkSummayOnTreeChange = true;
                $treeNodeSearch.css("display", "inherit");
                //$cmbSearch.css("border-right", "1px solid #ccc");
                $secoundDiv.css("display", "inherit");
                //$treeBackDiv.css("border-bottom", "1px solid #1aa0ed");
                $treeBackDiv.css("height", "70%");
                topTreeDiv.css("height", "100%");
                $treeBackDiv.resizable({
                    handles: 'n,s,se'
                });
                $treeBackDiv.find(".ui-resizable-e").css("right", "0px");
                $treeBackDiv.find(".ui-resizable-s").css("bottom", "0px");
                topTreeDiv.addClass("VIS-TM-resizediv");
                $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height() - 20);
                //$cmbSearch.width($searchDiv.width() - ($treeNodeSearch.width() + 40 + $btnSearch.width()));
                crossImages.css("right", "145px");
            }
            else {
                $chkSummaryLevel.prop("disabled", false);
                ChkSummayOnTreeChange = false;
            }
        };


     


        function OnTreeChange() {
            expandCollapse = false;
            clickontreenode = true;
            changeSeqFlag = false;
            validationValue = false;
            if ($cmbSelectTree.val() != "") {
                //$bsyDiv[0].style.visibility = "visible";
                //$cmbSearch.prop("disabled", true);
                //$cmbRightSearch.prop("disabled", true);
                //$chkAllCheckOrNot.prop("disabled", true);
                //$chkTrace.prop("disabled", true);
                //$rightMenuDemand.prop("disabled", true);


                //treeCmbDisable.css("display", "inherit");
                $bsyDivTree[0].style.visibility = "visible";

                unlinkeAllNode.removeClass("vis-tm-opacity");

                $chkSummaryLevel.prop("disabled", false)


                treeCmbDisable.css("display", "inherit");
                $rightMenuDemand.prop("disabled", true);

                $chkTrace.prop("disabled", false);
                $chkTrace.css("cursor", "pointer");
                // $chkTrace.prop("checked", false);

                $chktreeNode.prop("disabled", false);
                $chktreeNode.css("cursor", "pointer");
                // $chktreeNode.prop("checked", false);

                $cmbSearch.prop("disabled", false);
                $cmbRightSearch.prop("disabled", false);
                // $rightMenuDemand.prop("disabled", false);

                $chkAllRightPannel.prop("disabled", false);
                $chkAllRightPannel.css("cursor", "pointer");
                //   $chkAllRightPannel.prop("checked", false);


                $squenceDailog.addClass("vis-tm-delete");
                $treeExpandColapse.css("display", "none");
                $treeCollapseColapse.css("display", "inherit");
                // $treeCollapseColapse.css("float", "right");

                $deleteChild.addClass("vis-tm-delete");
                //$leftTreeDiv.height($treeBackDiv.height()-10);
                $scrollBottom = true;
                onscrollCheck = true
                $ulRight.empty();
                $ulMid.empty();
                searchRightext = "";
                $cmbRightSearch.val("");
                $cmbSearch.val("");

                $recordeNotFound.css("display", "inherit");
                pageLength = 50;
                pageNo = 1;

                $rightMenuDemand.val($($rightMenuDemand.find("option")[0]).attr("value"));
                //var selText = VIS.Utility.encodeText($cmbSelectTree.find("option:selected").text());

                var selText = $cmbSelectTree.find("option:selected").text();
                $lblRh4.text(VIS.Msg.getMsg("AllItems"));
                //                $lblRh4.text(selText);
                $lblMh4.text(selText);
                selectedNode = $cmbSelectTree.find("option:selected");
                $cmbSelectedType = selectedNode.attr("data-type");
                $treeID = selectedNode.val();//AD_Tree_id
                $cmbIsallnodes = selectedNode.attr("data-isallnodes");

                window.setTimeout(function () {
                    TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                }, 100);
                //$demandsMenu = "All";
                //pageNo = 1;
                //LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                window.setTimeout(function () {
                    $ulRight.empty();
                    $demandsMenu = "All";
                    pageNo = 1;
                    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                    DropMenu();
                    DropInMidDiv();
                }, 200);
                SetTreeHeight();




                if ($chkSummaryLevel.is(":checked")) {
                    $isSummary = true;
                    $secoundDiv.css("display", "inherit");
                    //$treeBackDiv.css("border-bottom", "1px solid #1aa0ed");
                    $treeBackDiv.css("height", "70%");
                    topTreeDiv.css("height", "100%");
                    $treeBackDiv.resizable({});
                    $treeBackDiv.find(".ui-resizable-e").css("right", "0px");
                    $treeBackDiv.find(".ui-resizable-s").css("bottom", "0px");
                    topTreeDiv.addClass("VIS-TM-resizediv");
                    $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height() - 20);
                    $mData.height(leftMianDataDiv.height() - ($treeBackDiv.height() + $mTopHeader.height() + 40));
                    $treeNodeSearch.css("display", "inherit");
                    //$cmbSearch.css("border-right", "1px solid #ccc");
                }
                else {
                    //$cmbSearch.css("border-right", "none");
                    $treeNodeSearch.css("display", "none");
                    $isSummary = false;
                    $secoundDiv.css("display", "none");
                    //$treeBackDiv.css("border-bottom", "0px");
                    $treeBackDiv.css("height", "100%");
                    topTreeDiv.css("height", "100%");
                    $treeBackDiv.resizable('destroy')
                    topTreeDiv.removeClass("VIS-TM-resizediv");
                    $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height());
                }

                //window.setTimeout(function () {
                //    GetCountForRestriction();
                //}, 300);

                // getTreeTableName();
                // $bsyDiv[0].style.visibility = "hidden";
            }
            else {
                unlinkeAllNode.addClass("vis-tm-opacity");
                $chkSummaryLevel.prop("disabled", true);
                $chkSummaryLevel.prop("checked", true);
                treeCmbDisable.css("display", "none");
                $treeID = 0;
                $ulRight.empty();
                $ulMid.empty();
                $leftTreeKeno.data("kendoTreeView").destroy();
                $leftTreeKeno.empty();
                $pathInfo.empty();
                searchRightext = "";
                $cmbRightSearch.val("");
                selectedNode = null;
                $deleteChild.addClass("vis-tm-delete");
                $cmbSearch.val("");
                window.setTimeout(function () {
                    $ulRight.empty();
                }, 200);

                $lblRh4.text(VIS.Msg.getMsg("SelectTree"));
                $lblMh4.text(VIS.Msg.getMsg("SelectTree"));
                //$recordeNotFoundRight.css("display", "inherit");
                $recordeNotFound.css("display", "none");

                $squenceDailog.addClass("vis-tm-delete");

                $chktreeNode.prop("disabled", true);
                $chkTrace.prop("disabled", true);
                $chktreeNode.css("cursor", "not-allowed");
                $chkTrace.css("cursor", "not-allowed");

                $cmbSearch.prop("disabled", true);
                $cmbRightSearch.prop("disabled", true);
                $rightMenuDemand.prop("disabled", true);

                $chkAllRightPannel.prop("disabled", true);
                $chkAllRightPannel.css("cursor", "not-allowed");

                rightCrossImage.css("display", "none");

                crossImages.css("display", "none");



                //$cmbSearch.prop("disabled", false);
                //$cmbRightSearch.prop("disabled", false);
                //$chkAllCheckOrNot.prop("disabled", false);
                //$chkTrace.prop("disabled", false);
                //$rightMenuDemand.prop("disabled", false);
            }


            $chkTrace.prop("checked", false);
            $chkAllRightPannel.prop("checked", false);
            $chktreeNode.prop("checked", false);

            $ulRight.empty();
            $chkAllCheckOrNot.prop("checked", false);
            AllSelectChkValue();
        };

        function SetTreeHeight() {
            topTreeDiv.height($treeBackDiv.height() - 10);

            //window.setTimeout(function () {
            //    $($leftTreeKeno.find("ul")[0]).css({ "overflow": "auto", "height": "265px" });
            //}, 200);

        };
        var onscrollCheck = true;
        function GetAllMenuData(e) {
            var elem = $(e.currentTarget);
            if (elem[0].scrollHeight - elem.scrollTop() == elem.outerHeight()) {
                if ($ulRight.find("li").length > 0) {

                    $demandsMenu = $rightMenuDemand.val();
                    onscrollCheck = false;
                    $rightmenuScroll = true;
                    pageNo += 1;
                    LoadMenuData($treeID, $cmbSelectedType, searchRightext, $demandsMenu);
                }
            }

            //var lastScrollTop = 0;
            //var st = $(this).scrollTop();
            //if (st > lastScrollTop)
            //{
            //    // downscroll code
            //    var s = "";
            //}
            //else
            //{
            //    var s = ""; // upscroll code
            //}
            //lastScrollTop = st;





            //if ($ulBackDiv[0].scrollHeight - $ulBackDiv.scrollTop() == $ulBackDiv.outerHeight()) {
            //    pageNo += 1;
            //    LoadMenuData($treeID, $cmbSelectedType);
            //}
        };

        function IsSummaryLevelCheck(e) {
            if ($cmbSelectTree.val() != "") {
                $ulMid.empty();
                if ($chkSummaryLevel.is(":checked")) {
                    $bsyDivforbottom[0].style.visibility = "hidden";
                    $isSummary = true;
                    $treeNodeSearch.css("display", "inherit");
                    //$cmbSearch.css("border-right", "1px solid #ccc");
                    $secoundDiv.css("display", "inherit");
                    //$treeBackDiv.css("border-bottom", "1px solid #1aa0ed");
                    $treeBackDiv.css("height", "70%");
                    topTreeDiv.css("height", "100%");
                    $treeBackDiv.resizable({
                        handles: 'n,s,se'
                    });
                    $treeBackDiv.find(".ui-resizable-e").css("right", "0px");
                    $treeBackDiv.find(".ui-resizable-s").css("bottom", "0px");
                    topTreeDiv.addClass("VIS-TM-resizediv");
                    if (selectedNode != null) {
                        TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                    }
                    $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height() - 20);

                    //$cmbSearch.width($searchDiv.width() - ($treeNodeSearch.width() + 40 + $btnSearch.width()));
                    crossImages.css("right", "145px");
                }
                else {
                    AskForMoreRecorde();

                    if ($cmbSearch.val() != "") {
                        $chktreeNode.prop("checked", false);
                    }


                    //$cmbSearch.width($searchDiv.width() - ($btnSearch.width() + 30));
                    crossImages.css("right", "35px");

                    //$treeNodeSearch.css("display", "none");
                    //$cmbSearch.css("border-right", "none");
                    //$isSummary = false;
                    //$secoundDiv.css("display", "none");
                    //$treeBackDiv.css("border-bottom", "0px");
                    //$treeBackDiv.css("height", "100%");
                    //topTreeDiv.css("height", "100%");
                    //$treeBackDiv.resizable('destroy')
                    //topTreeDiv.removeClass("VIS-TM-resizediv");
                    //if (selectedNode != null)
                    //{
                    //    TreeDataOnCmbChange($cmbSelectedType, $treeID, $cmbIsallnodes, $isSummary);
                    //}

                }
                $treeExpandColapse.css("display", "none");
                $treeCollapseColapse.css("display", "inherit");

                AllSelectChkValue();

                window.setTimeout(function () {
                    if ($cmbSearch.val() != "") {
                        window.setTimeout(function () {
                            SearchNodeInTree(e);
                        }, 200);
                    }
                }, 200);

                lastSeletedIndex = -1;
                changeSeqFlag = false;

            }


        };

        function createBusyIndicator() {
            //$bsyDiv = $("<div class='BusyDiv'>");
            $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDiv.css("position", "absolute");
            //$bsyDiv.css("bottom", "0");
            //$bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            //$bsyDiv.css("background-position", "center center");
            //$bsyDiv.css("width", "98%");
            //$bsyDiv.css("height", "98%");
            //$bsyDiv.css('text-align', 'center');
            //$bsyDiv.css('z-index', '1000');
            $bsyDiv[0].style.visibility = "visible";
            $root.append($bsyDiv);
        };


        //function createBusywithoutimage() {
        //    $bsyDivtreechnage = $("<div class='BusyDiv'>");
        //    $bsyDivtreechnage.css("position", "absolute");
        //    $bsyDivtreechnage.css("bottom", "0");
        //    $bsyDivtreechnage.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
        //    $bsyDivtreechnage.css("background-position", "center center");
        //    $bsyDivtreechnage.css("width", "98%");
        //    $bsyDivtreechnage.css("height", "98%");
        //    $bsyDivtreechnage.css('text-align', 'center');
        //    $bsyDivtreechnage.css('z-index', '1000');
        //    $bsyDivtreechnage[0].style.visibility = "visible";
        //    //$root.append($bsyDivtreechnage);
        //};




        function createBusyIndicatorFroBottomDiv() {
            var $topdiv = null;
            $topdiv = $('<div style="position: relative; width: 100%; height: 100%; z-index: -1;">');
            $bsyDivforbottom = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDivforbottom.css("position", "absolute");
            //$bsyDivforbottom.css("bottom", "0");
            //$bsyDivforbottom.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            //$bsyDivforbottom.css("background-position", "center center");
            //$bsyDivforbottom.css("width", "98%");
            //$bsyDivforbottom.css("height", "98%");
            //$bsyDivforbottom.css('text-align', 'center');
            //$bsyDivforbottom.css('z-index', '1000');
            //$bsyDivforbottom[0].style.visibility = "visible";
            $topdiv[0].style.visibility = "visible";
            $topdiv.append($bsyDivforbottom);
            $secoundDiv.append($topdiv);
        };


        function createBusyIndicatorForMenu() {
            $bsyDivMenu = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDivMenu.css("position", "absolute");
            //$bsyDivMenu.css("bottom", "0");
            //$bsyDivMenu.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            //$bsyDivMenu.css("background-position", "center center");
            //$bsyDivMenu.css("width", "98%");
            //$bsyDivMenu.css("height", "98%");
            //$bsyDivMenu.css('text-align', 'center');
            //$bsyDivMenu.css('z-index', '1000');
            $bsyDivMenu[0].style.visibility = "visible";
            $rightDataDiv.append($bsyDivMenu);
        };


        function createBusyIndicatorForTree() {
            // $recodeCount = $('<span style="display:none;position: relative;top: 60%;">' + VIS.Msg.getMsg("ItWillTakeTime") + '<span/>');
            $bsyDivTree = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDivTree.css("position", "absolute");
            //$bsyDivTree.css("bottom", "0");
            //$bsyDivTree.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            //$bsyDivTree.css("background-position", "center center");
            //$bsyDivTree.css("width", "98%");
            //$bsyDivTree.css("height", "98%");
            //$bsyDivTree.css('text-align', 'center');
            //$bsyDivTree.css('z-index', '1000');
            $bsyDivTree[0].style.visibility = "visible";
            // $bsyDivTree.append($recodeCount);
            $treeBackDiv.append($bsyDivTree);
        };




        this.Initialize = function () {
            createBusyIndicator();
            createBusyIndicatorForMenu();
            createBusyIndicatorForTree();
            createBusyIndicatorFroBottomDiv();
            GetTreeNameForCombo();
            EventHandling();

            window.setTimeout(function () {
                $bsyDiv[0].style.visibility = "hidden";
            }, 200)
            $bsyDiv[0].style.visibility = "visible";
            $bsyDivMenu[0].style.visibility = "hidden";
            $bsyDivTree[0].style.visibility = "hidden";
            $bsyDivforbottom[0].style.visibility = "hidden";

        };

        this.disposeComponent = function () {
            $leftTreeKeno = null;
            $isSummary = null;
            selectedNode = null;
            $cmbSelectedType = null;
            // $cmbSelectedID = null;
            $treeID = null;
            $cmbIsallnodes = null;
            TemplateTree = null;
            $dragMenuNodeID = null;
            $treeDataObjectForMatch = null;
            pageLength = null;
            pageNo = null;
            // rightMenuSelectedID = null;
            getIDFromContainer = null;
            dragMenuNodeIDArray = null;
            $checkMorRdragable = null;
            $dragTreeDataNodeID = null;
            getParentID = null;
            $bsyDiv = null;
            $getLifromTree = null;
            $getDataForTree = null;
            $ldiv = null;
            $mdiv = null;
            $rdiv = null;
            $lTopLeftDiv = null;
            $lTopRightDiv = null;
            $chkSummaryLevel = null;
            $lblSummaryLevel = null;
            $lblSelectTree = null;
            $lTopMidDiv = null;
            $cmbSelectTree = null;
            $treeRefresh = null;
            $cmbRefresh = null;
            $treeBackDiv = null;
            $leftTreeDiv = null;
            $searchDiv = null;
            $cmbSearch = null;
            $btnSearch = null;
            $mTopHeader = null;
            $lblMh4 = null;
            $mData = null;
            $recordeNotFound = null;
            $ulMid = null;
            $rTopHeard = null;
            $lblRh4 = null;
            $rightDataDiv = null;
            $ulBackDiv = null;
            $ulRight = null;
            $pathRightlist = null;
            $pathInfo = null;
        };
        this.getRoot = function () {
            if (window.kendo == undefined || window.kendo == null) {
                //VIS.ADialog.info("PleaseInstallKendoUIModule");

                var divNoModule = $('<div><p style="text-align: center;line-height: 600px;">' + VIS.Msg.getMsg("PleaseInstallKendoUIModule") + '</p></div>');
                return divNoModule;
            }
            window.setTimeout(function () {
                $deleteChild.addClass("vis-tm-delete");
                $treeBackDiv.find(".ui-resizable-n").css("display", "none");
            }, 200);



            $bsyDiv[0].style.visibility = "visible";
            return $root;
        };

        this.formSizeChanged = function (height, width) {
            $secoundDiv.height(leftMianDataDiv.height() - $treeBackDiv.height() - 20);
            //$cmbSearch.width($searchDiv.width() - ($treeNodeSearch.width() + 40 + $btnSearch.width()));
            $mData.height($secoundDiv.height());
            $secoundDiv.css("overflow", "hidden");



            //$leftTreeDiv.height($treeBackDiv.height());
        };


        this.sizeChanged = function (height, width) {
            this.formSizeChanged(height, width);
        };
    };


    VTreeMaintenance.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
        //this.formSizeChanged();
    };

    VTreeMaintenance.prototype.sizeChanged = function (height, width) {

        this.formSizeChanged(height, width);
    };

    VTreeMaintenance.prototype.refresh = function (height, width) {

        this.formSizeChanged();
    };


    VTreeMaintenance.prototype.dispose = function () {

        this.disposeComponent();
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps.AForms.VTreeMaintenance = VTreeMaintenance;
    //VAdvantage.Apps.AForms.VTreeMaintenance

}(VIS, jQuery));