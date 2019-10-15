; (function (VIS, $) {
    //****************************************************//
    //**            ThemeMgr                            **//
    //**************************************************//

    
    function themeMgr() {

        function init() {

                var cmb = $("#vis_cmbtheme");
                var root = document.documentElement;
                cmb.on("change", function (e) {

                    switch (this.selectedIndex) {
                        case 0:
                            root.style.setProperty('--color', 'black');
                            root.style.setProperty('--bgcolor', 'white');
                            root.style.setProperty('--hdrbgcolor', '#58B1E2');
                            break
                        case 1:
                            root.style.setProperty('--bgcolor', '#B3E5FC');
                            root.style.setProperty('--color', '#37474F');
                            root.style.setProperty('--hdrbgcolor', '#4f6098');
                            break
                        case 2:
                            root.style.setProperty('--bgcolor', 'lightpink');
                            root.style.setProperty('--color', 'red');
                            root.style.setProperty('--hdrbgcolor', '#a56969');
                            break
                    }
                });
        }
        return {
            init:init
        }
    }

    //Assignment Gobal Namespace
    VIS.themeMgr = themeMgr();

}(VIS, jQuery));