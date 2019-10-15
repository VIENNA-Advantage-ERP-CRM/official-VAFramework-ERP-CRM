; (function (VIS, $) {
    //****************************************************//
    //**            ThemeMgr                            **//
    //**************************************************//

    
    function themeMgr() {

        function init() {

            var li = $("#vis_theme");
                var root = document.documentElement;
            li.on("click", "div.vis-theme-rec", function (e) {
                var clr = $(e.currentTarget).data("color");
                var clrs = clr.split("|");
                root.style.setProperty('--v-c-primary', clrs[0]);
                root.style.setProperty('--v-c-on-primary', clrs[1]);
                root.style.setProperty('--v-c-secondary', clrs[2]);
                root.style.setProperty('--v-c-on-secondary', clrs[3]);
                });
        }
        return {
            init:init
        }
    }

    //Assignment Gobal Namespace
    VIS.themeMgr = themeMgr();

}(VIS, jQuery));