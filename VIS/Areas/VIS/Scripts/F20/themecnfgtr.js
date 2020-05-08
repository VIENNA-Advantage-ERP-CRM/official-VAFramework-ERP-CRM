; (function (VIS, $) {

    VIS.ThemeCnfgtor = function () {

        var $root = $("<div class='vis-forms-container'>");
        var $busyDiv = $("<div class='vis-apanel-busy'>");

        var windowNo = VIS.Env.getWindowNo();

        var $divTheme = null;
        var $divCtrl = null;
        var $divthSave = null;
        var $ulTheme = null;

        var $clrPrimary = null;
        var $clrOnPrinmary = null;
        var $clrSecondary = null;
        var $clrOnSecondary = null;
        var $txtName = null;

        var $self = this;

        function load() {

            setThemeColor("0, 152, 247", "255, 255, 255", "238, 238, 238", "51, 51, 51");

            setBusy(true);
            $root.load(VIS.Application.contextUrl + 'Theme/ThemeCnfgtr/?windowNo=' + windowNo, function (event) {
                init();

                fillThemeList();
            });
        };

        function setThemeColor(p, onP, s, onS) {
            document.documentElement.style.setProperty("--v-c-th-primary", p);
            document.documentElement.style.setProperty("--v-c-th-on-primary", onP);
            document.documentElement.style.setProperty("--v-c-th-secondary", s);
            document.documentElement.style.setProperty("--v-c-th-on-secondary", onS);
        }

        function init() {

            $divTheme = $root.find("#divTheme_" + windowNo);
            $divCtrl = $root.find(".vis-thed-clrpickerouterwrap");

            $divthSave = $root.find(".vis-thed-botsavesec");

            $clrPrimary = $divCtrl.find('input[name="primary"]');
            $clrOnPrinmary = $divCtrl.find('input[name="onprimary"]');
            $clrSecondary = $divCtrl.find('input[name="secondary"]');
            $clrOnSecondary = $divCtrl.find('input[name="onsecondary"]');

            $txtName = $divthSave.find("input");

            $ulTheme = $root.find(".vis-thed-savedlistwrap");


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

            $divthSave.find('a').on('click', function (e) {
                if ($txtName.val().trim() == "") {
                    VIS.ADialog.error("EnterName");
                    return;
                }
                var $btn = $(this);
                var isDef = $btn.data('action') == "sandd";
                setBusy(true);
                saveThemeData(hexToRgbComma($clrPrimary.val()), hexToRgbComma($clrOnPrinmary.val()),
                    hexToRgbComma($clrSecondary.val()), hexToRgbComma($clrOnSecondary.val()), isDef, $txtName.val());
            });

            $ulTheme.on("click", "LI", function (e) {
                setBusy(true);
                var id = $(e.currentTarget).data("id");
                if (e.target.className.indexOf('vis-delete') > -1) {

                    VIS.ADialog.confirm("DeleteRecord?", true, "", "Confirm", function (ret) {
                        if (ret)
                            VIS.dataContext.postJSONData(VIS.Application.contextUrl + 'Theme/Delete', { id: id }, function (data) {
                                fillThemeList();

                            });
                        else
                            setBusy(false);

                    });
                }
                else {
                    VIS.dataContext.postJSONData(VIS.Application.contextUrl + 'Theme/SetDefault', { id: id }, function (data) {
                        fillThemeList();
                    });
                }
            });
        };

        function setBusy(isBusy) {
            if (isBusy)
                $busyDiv[0].style.visibility = 'visible';
            else
                $busyDiv[0].style.visibility = 'hidden';
        };

        function saveThemeData(pri, onpri, sec, onsec, isdef, name) {
            var data = {};
            data.Primary = pri;
            data.OnPrimary = onpri;
            data.Seconadary = sec;
            data.OnSecondary = onsec;
            data.IsDefault = isdef;
            data.Name = name;
            setBusy(true);
            VIS.dataContext.postJSONData(VIS.Application.contextUrl + 'Theme/Save', data, saveThemeDataCmplted);

        }

        function saveThemeDataCmplted(ret) {
            if (ret > 0) {
                fillThemeList();
                $txtName.val("");
            }
            else {
                setBusy(false);
            }
        }

        function fillThemeList() {
            setBusy(true);
            $ulTheme.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + 'Theme/GetList', { id: 0 }, function (data) {

                var htm = [];
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];

                    htm.push('<li class="vis-thed-savedlistitem');
                    if (item.IsDefault)
                        htm.push(' vis-thed-selectedlistitem');
                    htm.push('" data-id="' + item.Id + '">');
                    htm.push('<div class="vis-theme-rec" style="width:80px">');
                    htm.push('<span class="vis-theme-color" style="background-color:rgba(' + item.Primary + ',1)"></span>');
                    htm.push('<span class="vis-theme-color" style="background-color:rgba(' + item.OnPrimary + ',1)"></span>');
                    htm.push('<span class="vis-theme-color" style="background-color:rgba(' + item.Seconadary + ',1)"></span>');
                    htm.push('<span class="vis-theme-color" style="background-color:rgba(' + item.OnSecondary + ',1)"></span>');
                    htm.push('</div>');

                    htm.push('<a class="vis-thed-themename">' + item.Name + '</a>');

                    if (item.IsDefault)
                        htm.push('<i class="vis vis-markx"></i>');
                    htm.push('<span style="margin:0 3px 0 3px" class="vis vis-delete"></span>');
                    htm.push('</li>');

                    if (item.IsDefault) {
                        $clrPrimary.val(regbCommaToHex(item.Primary));
                        $clrOnPrinmary.val(regbCommaToHex(item.OnPrimary));
                        $clrSecondary.val(regbCommaToHex(item.Seconadary));
                        $clrOnSecondary.val(regbCommaToHex(item.OnSecondary));
                        setThemeColor(item.Primary, item.OnPrimary, item.Seconadary, item.OnSecondary);
                    }
                }
                $ulTheme.append(htm.join(' '));
                setBusy(false);
            });
        }

        function componentToHex(c) {
            var hex = c.toString(16);
            return hex.length == 1 ? "0" + hex : hex;
        };

        //function rgbToHex(r, g, b) {
        //    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
        //}
        function rgbToHex(red, green, blue) {
            var rgb = blue | (green << 8) | (red << 16);
            return '#' + (0x1000000 + rgb).toString(16).slice(1)
        }

        function regbCommaToHex(rgb) {
            var rgb = rgb.split(',');
            return rgbToHex(rgb[0], rgb[1], rgb[2]);
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
            var h = $(window).height()-10;
            $busyDiv.height(h);
            $busyDiv.width(w);
            $root.append($busyDiv);

            var ch = new VIS.ChildDialog();
            ch.setContent($root);
            ch.setWidth(w);
            ch.setHeight(h);
            ch.setTitle(VIS.Msg.getMsg("ThemeConfig"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {
                $self.dispose();
                $("#ui-datepicker-div").remove()
                $root = null;
                $self = null;
            };
            ch.show();
            ch.hideButtons();
        };

        this.show = function () {
            load();
            showDialog();
        };

        this.dispose = function () {

            $root.remove();
            $root = null;
            $self = null;
        };
    }


}(VIS, jQuery));
