; (function (VIS, $) {

    var $root = $('<div/>');//, {

    $root.css('width', '100%');
    $root.css('height', '100%');

    var $menu = $("<ul class='vis-apanel-rb-ul'>");
    $menu.css('width', '100%');
    $menu.css('height', '100%');

    $root.append($menu);

    $menu.on('click', "LI", function (e) {

        var id = $(e.target).data("id");
        var InfoWindow = new VIS.InfoWindow(id, null, 0, null, null);
        InfoWindow.show();

    });

    function load() {

        $.ajax({
            url: VIS.Application.contextUrl + "InfoMenu/LoadMenu",
            dataType: "json",
            error: function () {
                alert('No DATA');
            },
            success: function (data) {


                var infoWinMenu = data.result;

                for (var item in infoWinMenu) {
                    var li = $("<li data-id='" + infoWinMenu[item].AD_InfoWindow_ID + "' >");
                    li.append(infoWinMenu[item].Name);
                    $menu.append(li);
                }
            }
        });
    };

    function hide() {

    }


    load();

    VIS.InfoMenu = {
        show: function ($btnInfo) {

            $btnInfo.w2overlay($root.clone(true), { css: { height: '300px' } });
        }
    };


})(VIS, jQuery);