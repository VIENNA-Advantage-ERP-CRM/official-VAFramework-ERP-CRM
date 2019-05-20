; (function (VIS, $) {

    function favMgr() {

        var favDiv;
        var favContainer;
        var ul, atab;

        var mgr = {
            init: init,
            addFavourite: addFavouriteMenu,
            removeFavourite: removeFavourite
        };

        return mgr;

        function init(_favDiv) {
            favDiv = _favDiv;
            favContainer = favDiv.find("#vis_favScroll");
            getFavouriteNode();
            atab = $('#userFav-Tab');
            ul = $('<ul class="vis-userFavourites-ListMenu">');
            ul.on("click", function (e) {
                var $target = $(e.target);
                if ($target.data('btn') == 'action') {
                    VIS.viewManager.startAction($target.data("action"), $target.data("id"));
                }
                else if ($target.data('btn') == 'zoom') {
                    VIS.viewManager.startActionInNewTab($target.data("action"), $target.data("id"));
                   // alert($target.data("action") + " " + $target.data("id"));
                }
                else if ($target.data('btn') == 'remove') {
                    // e.stopPropagation()
                    removeFav($target.data("id"));
                    var divCon = $($target).parent().parent();
                    divCon.empty();
                    divCon.remove();
                    setCount(ul.find('li').length);
                }
            });
        };

        function getFavouriteNode() {
            $.ajax({
                url: VIS.Application.contextUrl + "Home/GetFavouriteNode",
                dataType: "json",
                error: function () {
                    alert(VIS.Msg.getMsg('ERRORGetFavouriteNode'));
                },
                success: function (data) {
                    favContainer.empty();
                    if (data.result) {
                        for (var itm = 0, j = data.result.length; itm < j; itm++) {
                            addFavourite(data.result[itm]);
                        }
                    }
                    setCount(ul.find('li').length);
                    favContainer.append(ul);
                }
            });
        };

        function addFavouriteMenu(barNode) {
            addFavourite(barNode);
            setCount(ul.find('li').length);
        }

        function addFavourite(barNode) {
            if (ul == null) return;

            var li = $('<li data-nodeid="' + barNode.NodeID + '" >');
            var id = 0;
            if (barNode.Action == "W") {
                id = barNode.WindowID;
            }
            else if (barNode.Action == "X") {
                id = barNode.FormID;
            }
            else if (barNode.Action == "P") {
                id = barNode.ProcessID;
            }
            else if (barNode.Action == "R") {
                id = barNode.ProcessID;
            }
            var aNode = $('<a href="#" data-id="' + id + '" data-action="' + barNode.Action + '" data-nodeid="' + barNode.NodeID + '" data-btn="action" style="overflow: auto;width: 100%;height: 100%;padding-top: 17px;" >');
            //aNode.on('click', function () {
            //    var id = $(this).data("id");
            //    var action = $(this).data("action");
            //    VIS.viewManager.startAction(action, id);
            //});

            var btnZoom = $("<a data-id='" + id + "' data-action='" + barNode.Action + "' data-btn='zoom' style='height: 25px;width: 25px;float: right;margin-right: -10px;'>").append($("<img data-id='" + id + "' data-action='" + barNode.Action + "' data-btn='zoom' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/zoomfav.png' >"));
            //btnZoom.on('click', function (e) {

            //});
            li.append(btnZoom);

            var btnRemove = $("<a data-id='" + barNode.NodeID + "'  data-btn='remove' style='height: 25px;width: 25px;float: right;'>").append($("<img data-id='" + barNode.NodeID + "'  data-btn='remove' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/removefav.png' >"));
            li.append(btnRemove);

            li.append(aNode);
            //var btnRemove = $("<span data-id='" + barNode.NodeID + "' class='favouritesIcons icon-favourite-large vis-span-bottom'>");
            //btnRemove.on('click', function (e) {
            //    e.stopPropagation()
            //    removeFav($(this).data("id"));
            //    var divCon = $(this).parent();
            //    divCon.empty();
            //    divCon.remove();
            //    setCount(ul.find('li').length);
            //});
            // aNode.append(btnRemove);
           // aNode.append("<i>");
            var name = barNode.Name;
            if (name.length > 27) {
                name = name.substr(0, 27);
                aNode.append(name);
                var aFName = $("<a href='#' class='VIS_Pref_tooltip' style='display: inline-block;'>").append('...');
                var span = $("<span style='width: inherit;'>");
                span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                span.append($("<label class='VIS_Pref_Label_Font'>").append(barNode.Name));
                aFName.append(span);
                aNode.append(aFName);
            }
            else {
                aNode.append(barNode.Name);
            }
            ul.append(li);

        };

        function removeFavourite(nodeID) {
            if (ul == null) {
                return;
            }
            var arr = ul.find('li');
            var current = null;
            for (var itm = 0, len = arr.length; itm < len; itm++) {
                current = $(arr[itm]);
                if (current.data('nodeid') === nodeID) {
                    current.remove();
                }
            }
            arr = current = null;
            setCount(ul.find('li').length);
        };

        function removeFav(nodeID) {
            VIS.FavouriteHelper.removeFavourite(nodeID);
            $.ajax({
                url: VIS.Application.contextUrl + "Home/RemoveNodeFavourite/?nodeID=" + nodeID,
                dataType: "json"
            });
        };

        function setCount(count) {
            atab.empty();
            atab.html("<span class='favouritesIcons icon-favourite' /><span class='favouriteTabLabel-Large'>" + VIS.Msg.getMsg('Favourites') + " - " + "</span><strong>" + count + "</strong>");
        };
    };

    VIS.favMgr = favMgr();
}(VIS, jQuery));