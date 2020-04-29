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

            document.documentElement.style.setProperty("--v-c-th-primary", "0, 152, 247");
            document.documentElement.style.setProperty("--v-c-th-on-primary", "255, 255, 255");
            document.documentElement.style.setProperty("--v-c-th-secondary", "238, 238, 238");
            document.documentElement.style.setProperty("--v-c-th-on-secondary", "51, 51, 51");
               
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

            $divTheme = $root.find("#divTheme_" + windowNo);
            $divCtrl = $root.find(".vis-thed-clrpickerouterwrap");

            $clrPrimary = $divCtrl.find('input[name="primary"');
            $clrOnPrinmary = $divCtrl.find('input[name="onprimary"');
            $clrSecondary = $divCtrl.find('input[name="secondary"');
            $clrOnSecondary = $divCtrl.find('input[name="onsecondary"');
            
            // all element s

                $divCtrl.find('input').on('change', function (e) {
                var val = null;
                if (this.name == "primary") {
                    document.documentElement.style.setProperty('--v-c-th-primary', hexToRgbComma(this.value));
                }
                else if (this.name == "onprimary") {
                    document.documentElement.style.setProperty('--v-c-th-on-primary', hexToRgbComma(this.value));
                }
                else if (this.name == "secondary") {
                    document.documentElement.style.setProperty('--v-c-th-secondary', hexToRgbComma(this.value));
                }
                else if (this.name == "onsecondary") {
                    document.documentElement.style.setProperty('--v-c-th-on-secondary', hexToRgbComma(this.value));
                }
            });
        };

        function componentToHex(c) {
            var hex = c.toString(16);
            return hex.length == 1 ? "0" + hex : hex;
        }

        function rgbToHex(r, g, b) {
            return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
        }

        function hexToRgbComma(hex) {
            var ret = hexToRgb(hex);
            if (ret)
                return ret.r + "," + ret.g + "," + ret.b;
            return ret;
        }

        function hexToRgb(hex) {
            // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
            var shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
            hex = hex.replace(shorthandRegex, function (m, r, g, b) {
                return r + r + g + g + b + b;
            });

            var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
            return result ? {
                r: parseInt(result[1], 16),
                g: parseInt(result[2], 16),
                b: parseInt(result[3], 16)
            } : null;
        }

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
