; (function (VIS, $) {

    VIS.Newmailformat = function (isEmail) {
        var $maingrid;
        var $name;
        var $showforall;
        var ch;
        var onClose = null;
        var self = this;
        init();
        function init() {
            $maingrid = $('<div  ></div>');

            var $containerdiv = $('<div></div>');

            var namediv = $('<div style="overflow:auto;margin-bottom:10px"></div>');

            var $label = $('<p  style="float:left;margin-right:10px"  >' + VIS.Msg.getMsg('Name') + '</p>');


            $name = $('<input type="text" style="float:left;margin-right:4px" ></input>');
            namediv.append($label).append($name);





            $showforall = $('<input type="checkbox" style="float:right"></input>');
            var labelshow = $('<p  style="float:right;margin-bottom:0px;margin-right:4px"  >' + VIS.Msg.getMsg('ForAllWindows') + '</p>');
            var checkdiv = $('<div></div>');

            if (VIS.Application.isRTL) {
                $label.css({ "float": "right", "margin-right": "0px", "margin-left": "0px" })
                $name.css({ "float": "right", "margin-right": "10px", "margin-left": "0px" });
                $showforall.css("float", "left");
                labelshow.css({ "float": "left", "margin-right": "5px" });
                checkdiv.css({ "float": "right", "margin-bottom": "-5px" });
            }


            checkdiv.append(labelshow).append($showforall);
            $containerdiv.append(namediv).append(checkdiv);
            $maingrid.append($containerdiv);
        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            //ch.setHeight(400);
            //ch.setWidth(800);
            if (isEmail) {
                ch.setTitle(VIS.Msg.getMsg("EmailFormats"));
            }
            else {
                ch.setTitle(VIS.Msg.getMsg("LetterFormats"));
            }
            ch.setModal(true);
            ch.setContent($maingrid);
            ch.show();
            ch.onOkClick = ok;
            ch.onClose = dispose;
        };


        function ok() {

            if ($name.val() == null || $name.val() == "") {
                VIS.ADialog.info("FileNameMendatory");
                return false;
            }

            self.onClose();
            // dispose();
            return true;
        }

        function dispose() {
            $maingrid.remove();
            $maingrid = null;
            $name.remove();
            $name = null;
            $showforall.remove();
            $showforall = null;
            ch = null;
        };

        this.getName = function () {
            return $name.val();
        };

        this.saveForAll = function () {
            return $showforall.prop("checked");
        }


    };
})(VIS, jQuery);
