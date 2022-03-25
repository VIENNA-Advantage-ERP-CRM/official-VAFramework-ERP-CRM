; VIS = window.VIS || {};

; (function (VIS, $) {

    VIS.AcctViewerMenu = function () {
        this.frame;
        this.windowNo;
        var $bsyDiv;
        var $selfObject = this; //scoped self pointer
        var _btnResult = "";
        var $root = $('<div>');

        /** Initialize the structure of form. **/
        this.initalize = function () {
            // Added button to trigger click event to close the dailog
            $root.append('<a id="VIS_btnResult_' + $selfObject.windowNo + '">button</a>');
            _btnResult = $root.find("#VIS_btnResult_" + $selfObject.windowNo);
            bindEvents();
        };

        /** Method to open the account viewer form called on init. **/
        this.openAccountViewer = function () {
            // Open account viewer form 
            //_btnResult.trigger("click");


            var obj = new VIS.AcctViewer(VIS.context.getAD_Client_ID(), 1, 1, this.windowNo, 1);
            if (obj != null) {
                obj.setIsMenu(true);
                obj.showDialog();
            }
            //obj = null;

            //Click event for button
            //window.setTimeout(function () {
                _btnResult.trigger("click");
          //  }, 10);
        };

        /** Create busy indicator **/
        function createBusyIndicator() {
            $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDiv.css({
            //    "position": "absolute", "width": "98%", "height": "97%", 'text-align': 'center', 'z-index': '999'
            //});
        };

        /** Initialize events of controls **/
        function bindEvents() {
            _btnResult.on("click", function () {
                $selfObject.frame.dispose();
            });
        }
        this.getRoot = function () {
            return $root;
        };

    };

    VIS.AcctViewerMenu.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        //$($(this.frame.parent.getContentGrid()).parent()).parent().css("display", "none");
        this.windowNo = windowNo;
        this.initalize();
        this.openAccountViewer();
        this.frame.getContentGrid().append(this.getRoot());
    };


    VIS.AcctViewerMenu.prototype.dispose = function () {
        this.disposeComponent();
        this.frame = null;
    };


})(VIS, jQuery);