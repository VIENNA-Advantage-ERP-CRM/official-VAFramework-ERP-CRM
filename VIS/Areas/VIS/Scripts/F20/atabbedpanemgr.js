; (function (VIS, $) {
//****************************************************//
//**             VTabbedPane                       **//
//**************************************************//

/**
 *  multiptab and link tab view manager Pane - Window Tab
 *
 */


    function Tpmgr(apanel, vTabbedPane) {
        
        /** List of dependent Variables		*/
        this.oldTabIndex = -1;
        this.tabs = []; /*tab elements*/
        this.tabsIds = []; /* ids of tabs*/
         this.tabLevelsItems = [];/* all tabs with tabitem by level */
        this.tabItems = [];
        this.count = 0;
        this.apanel = apanel;
        this.vTabbedPane = vTabbedPane;
    };





    VIS.ATabPaneMgr = Tpmgr;



}(VIS, jQuery));