; (function (VIS, $) {
    //****************************************************//
    //**            ThemeMgr                            **//
    //**************************************************//

    
    function themeMgr() {

        function init() {

            var li = $("#vis_theme");
                
            li.on("click", "div.vis-theme-rec", function (e) {
                var clr = $(e.currentTarget).data("color");
                applyTheme(clr);
            });

            //Get Saved or deafult theme 
            if (VIS.Application.theme && VIS.Application.theme != "") {
                applyTheme(VIS.Application.theme);
            }
            else {
                var def = li.find("div.vis-theme-rec");
                if (def.length > 0) {
                    applyTheme($(def[0]).data("color"));
                }
            }
        }

        function applyTheme(clr) {
            var root = document.documentElement;
            var clrs = clr.split("|");
            root.style.setProperty('--v-c-primary', clrs[0]);
            root.style.setProperty('--v-c-on-primary', clrs[1]);
            root.style.setProperty('--v-c-secondary', clrs[2]);
            root.style.setProperty('--v-c-on-secondary', clrs[3]);
        }

        return {
            init: init,
            applyTheme: applyTheme
        }
    }

    //Assignment Gobal Namespace
    VIS.themeMgr = themeMgr();

}(VIS, jQuery));