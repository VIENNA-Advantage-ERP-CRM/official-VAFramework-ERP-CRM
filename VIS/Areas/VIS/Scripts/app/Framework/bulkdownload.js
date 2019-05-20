; (function (VIS, $) {
    //var $record_id, $chat_id, $table_id, $description, $chatclose;
    function BulkDownload(windowNo, forFile) {

        this.onClose = null; //outer apanel close function

        var $maindiv = $('<div></div>'); //layout
        var $div = $('<div style="overflow-y:auto;margin-bottom:15px"></div>');
        var $createbar = $('<p id="forreportcreate"></p>' + '<div class="vis-ui-progress-bar hide" id="reportcreate"><div class="progress-bar vis-ui-progress" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div></div><br /><p class="hide" id="reportcreateresult"></p> <div style="margin-bottom: 15px;"></div>');
        var $downloadbar = $('<p id="forreportdownload" class="hide"></p>' + '<div class="vis-ui-progress-bar hide" id="reportdownload"><div class="progress-bar vis-ui-progress" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div></div><br /><p class="hide" id="reportdownloadresult"></p>');
        var $bulkbusyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        $maindiv.append($bulkbusyDiv).append($createbar).append($downloadbar);

        this.winNo = windowNo;
        var bd = null;
        this.prop = { WindowNo: windowNo };
        init($div, windowNo, this.prop, forFile);
        var self = this;
        this.show = function () {
            bd = new VIS.ChildDialog();
            bd.setContent($maindiv);
            bd.setHeight(450);
            bd.setWidth(650);
            bd.setTitle(VIS.Msg.getMsg("Reports"));
            bd.setModal(true);

            //Disposing Everything on Close
            bd.onClose = function () {
                if (self.onClose) self.onClose();
                self.dispose();
            };
            bd.show();
            events();
            bd.hidebuttons();
        };

        this.onClose = function () {
            bd.close();
        };

        this.setBulkBusy = function (isBusy) {
            $bulkbusyDiv.css("display", isBusy ? 'block' : 'none');
        };
        this.dispose = function () {
            this.prop = null;
            $div = null;
            $maindiv = null;
            bd = null;
            self = null;
            this.setBulkBusy(false);

        };
        function events() {
        };
        function init($container, windowNo, prop, forFile) {
            $createbar.filter('p#forreportcreate').text(VIS.Msg.getMsg('Generating' + forFile + 'File'));
            $downloadbar.filter('p#forreportdownload').text(VIS.Msg.getMsg('Downloading' + forFile + 'File'));
        };
    };

    VIS.BulkDownload = BulkDownload;

})(VIS, jQuery);