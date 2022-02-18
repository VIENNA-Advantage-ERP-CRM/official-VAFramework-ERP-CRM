/********************************************************
 * Module Name    :     Application
 * Purpose        :     Get Favourite Nodes And shows in Wellcome Screen
 * Author         :     Lakhwinder
 * Date           :     10-Aug-2014
  ******************************************************/
; (function (VIS, $) {

    function FavouriteHelper() {
        var $root = null;
        var $menu = null;
        var nodeID = null;
        var btn = null;
        var barNode = null;
        var menudiv = null;
        var liAddfav = null;
        var liRemovefav = null;
        var lifavZoom = null;
        var action, actionid, nodeName, isFav;
        var isloaded = false;
        function initialize() {
            $root = $('<div style="width:100%;height:100%">');//, {
            $menu = $("<ul class='vis-apanel-rb-ul' style='width:100%;height:100%'>");
            liAddfav = $("<li data-isfav='no' data-id='favaction'>").append(VIS.Msg.getMsg('AddFav'));
            liRemovefav = $("<li data-isfav='yes' data-id='favaction'>").append(VIS.Msg.getMsg('RemoveFav'));
            lifavZoom = $("<li data-isfav='" + btn.data('isfav') + "' data-id='favaction'>").append(VIS.Msg.getMsg('OpenNewTab'));
            $menu.append(liAddfav);
            $menu.append(liRemovefav);
            $menu.append(lifavZoom);
            $root.append($menu);
            bindEvent();
        };
        function showOverlay(_btn) {
            btn = _btn;
            nodeID = btn.data('value');
            action = btn.data('action');
            actionid = btn.data('actionid');
            nodeName = btn.data('name');
            isFav = btn.attr('data-isfav');
            if (!isloaded) {
                initialize();
                isloaded = true;
            }
            if (isFav == 'yes') {
                liAddfav.css('display', 'none');
                liRemovefav.css('display', 'block');
            }
            else {
                liAddfav.css('display', 'block');
                liRemovefav.css('display', 'none');
            }
            btn.w2overlay($root.clone(true), { css: { height: '300px' } });
        };

        function bindEvent() {
            liAddfav.on('click', addRemoveFav);
            liRemovefav.on('click', addRemoveFav);
            lifavZoom.on('click', openNewTab);
        };
        function addRemoveFav() {
            if (isFav == 'yes') {
                //btn.removeClass("vis-favitmchecked");
                //btn.addClass("vis-favitmunchecked");
                btn.find('span').remove();
                var otherSpan = $('#vis_divTree').find('[data-value="' + btn.data('value') + '"]');
                if (otherSpan && otherSpan.length > 0) {
                    if ($(otherSpan[1]).find('span.fa.fa-star').length > 0) {
                        $(otherSpan[1]).attr('data-isfav', 'no');
                        $(otherSpan[1]).find('span.fa.fa-star').remove();
                    }

                    if (VIS.Application.isMobile || VIS.Application.isIOS) {
                        if ($(otherSpan[5])) {
                            $(otherSpan[5]).attr('data-isfav', 'no');
                            $(otherSpan[5]).find('span.fa.fa-star').remove();
                        }
                    }
                    else {
                        if ($(otherSpan[3])) {
                            $(otherSpan[3]).attr('data-isfav', 'no');
                            $(otherSpan[3]).find('span.fa.fa-star').remove();
                        }
                    }
                }
                btn.attr('data-isfav', 'no');
                removeFav(nodeID);
                VIS.favMgr.removeFavourite(nodeID);
            }
            else {
                barNode = {};
                barNode.Action = action;//aAction.data('action');
                barNode.WindowID = actionid;//aAction.data('actionid');
                barNode.FormID = actionid; //aAction.data('actionid');
                barNode.ProcessID = actionid; //aAction.data('actionid');
                barNode.NodeID = nodeID;//btn.data('value');
                barNode.Name = nodeName;// $(aAction).parent().children(0).text();
                //btn.removeClass("vis-favitmunchecked");
                //btn.addClass("vis-favitmchecked");
                btn.attr('data-isfav', 'yes');
                btn.prepend('<span class="fa fa-star"></span>');
                var otherSpan = $('#vis_divTree').find('[data-value="' + btn.data('value') + '"]');
                if (otherSpan && otherSpan.length > 2) {

                    if ($(otherSpan[1]).find('span.fa.fa-star').length == 0) {
                        $(otherSpan[1]).prepend('<span class="fa fa-star"></span>');
                        $(otherSpan[1]).attr('data-isfav', 'yes');
                    }
                    else {
                        if (VIS.Application.isMobile || VIS.Application.isIOS) {
                            $(otherSpan[5]).prepend('<span class="fa fa-star"></span>');
                            $(otherSpan[5]).attr('data-isfav', 'yes');
                        }
                        else {
                            $(otherSpan[3]).prepend('<span class="fa fa-star"></span>');
                            $(otherSpan[3]).attr('data-isfav', 'yes');
                        }
                    }
                }

                addToFav(nodeID);
                VIS.favMgr.addFavourite(barNode);
            }

        };

        function openNewTab() {
            //alert(action + " " + actionid);
            VIS.viewManager.startActionInNewTab(action, actionid);
        };
        function addToFav(nodeID) {

            $.ajax({
                url: VIS.Application.contextUrl + "Home/SetNodeFavourite/?nodeID=" + nodeID,
                dataType: "json"
            });
        };

        function removeFav(nodeID) {
            $.ajax({
                url: VIS.Application.contextUrl + "Home/RemoveNodeFavourite/?nodeID=" + nodeID,
                dataType: "json"

            });
        };

        function removeFavourite(nodeID) {
            if (!menudiv) menudiv = $('#vis_divTree');
            var li = menudiv.children(0).find("[data-value='" + nodeID + "']");
            var btnPic = $(li.children()[1]);
            btnPic.removeClass("vis-favitmchecked");
            btnPic.addClass("vis-favitmunchecked");
            btnPic.data('isfav', 'no');
        }
        var favHelper = {
            removeFavourite: removeFavourite,
            showOverlay: showOverlay
        };
        return favHelper;
    };

    VIS.FavouriteHelper = FavouriteHelper();
})(VIS, jQuery);