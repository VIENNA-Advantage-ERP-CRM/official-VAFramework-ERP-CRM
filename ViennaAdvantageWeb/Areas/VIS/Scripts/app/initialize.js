; (function ($, V) {
    /* Initlial Setting for App */
    VIS.VApp = function () {
        /* 
          Set Login Cultue to Globalize js
        */
        function setCulture() {
            var lang = V.context.getAD_Language().replace("_", "-");
            Globalize.culture(lang);
        };
       
        function startApp() {
            //Remove loading div
            document.getElementById("vis_mainConatiner").removeChild(document.getElementById("vis_busyGrid"));

            /* get Validation token and set in ajax default setting*/
            var token = $("#vis_antiForgeryToken").val();
            //prject label token setting
            $.ajaxSetup({
                headers: {
                    'VerificationToken': token
                }
                //,
                //type: "POST"
            });
        };

        /* Intilize App Logger js */
        function initLogger() {
            var VL = V.Logging;
            VL.VLogMgt.initialize(true);
            VL.VLogMgt.setLevel(VIS.Ini.getTraceLevel());
        };

        /* load local context and overwrite in  server context class */
        function loadLocalprefIntoContext() {
            VIS.Ini.updateLocalContextFromIni();
        };

        /* Entery Point of App */
        function init() {
            startApp();
            setCulture();
            loadLocalprefIntoContext();
            initLogger();
            VIS.desktopMgr.start(); //Invoke Desktop mgr 's start function

            //Set Page Size of Window
            VIS.Env.setWINDOW_PAGE_SIZE(VIS.Ini.getLocalStorage(VIS.IniConstants.WIN_PAGESIZE));
            //_ctx.SetPrintNativeDigits(VAdvantage.DataBase.Ini.IsPrintNativeDigits() ? "Y" : "N");
        }
        return {
            init: init
        }
    }();


    


}
)(jQuery, VIS);