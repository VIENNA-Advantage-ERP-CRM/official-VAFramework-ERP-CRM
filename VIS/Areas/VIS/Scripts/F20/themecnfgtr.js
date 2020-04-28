; (function (VIS, $) {

    VIS.ThemeCnfgtor = function () {

        var $root = $("<div class='vis-forms-container'>");
        var $busyDiv = $("<div class='vis-apanel-busy'>");
        
        var windowNo = VIS.Env.getWindowNo();

        var $divTheme = null;
        var $clrPrimary = null;
        var $clrOnPrinmary = null;
        var $clrSecondary = null;
        var $clrOnSecondary = null;

        var $self = this;

        function load() {

            $root.load(VIS.Application.contextUrl + 'Theme/ThemeCnfgtr/?windowNo=' + windowNo, function (event) {

                $busyDiv[0].style.visibility = 'visible';

                $self.init();
                //var divget = $root.find("#content_" + windowNo);
                //if (divget != null) {
                    // divget.tabs();
                    //divget.tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
                    //divget.find("li").removeClass("ui-corner-top").addClass("ui-corner-left");
                //}
                //remove image
                $busyDiv[0].style.visibility = 'hidden';
            });
        };

        this.init = function () {

            $divTheme = $root.find("divTheme_" + windowNo);

            

            $clrPrimary = $divTheme.find()
            // all element s

            $root.find(".vis-thed-clrpickerouterwrap").find('input').on('change', function (e) {
                if (this.name == "primary") {
                    alert('primary');
                }
            });

        };

        function showDialog() {
            var w = $(window).width() - 150;
            var h = $(window).height() - 40;
            $busyDiv.height(h);
            $busyDiv.width(w);
            $root.append($busyDiv);
            $root.dialog({
                modal: false,
                title: VIS.Msg.getMsg("Theme"),
                width: w,
                height: h,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();

                    $root.dialog("destroy");

                    $("#ui-datepicker-div").remove()
                    $root = null;
                    $self = null;
                }
            });
        };

        this.show = function () {
            load();
            showDialog();
        };

        this.dispose = function () {

        };
    }


}(VIS, jQuery));
