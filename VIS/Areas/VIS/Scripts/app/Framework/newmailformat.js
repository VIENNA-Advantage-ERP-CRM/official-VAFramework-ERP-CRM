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
            $maingrid = $('<div class="vis-forms-container" ></div>');

            var $containerdiv = $('<div class="vis-emailsavewrap"></div>');
            var $divinputwrap = $('<div class="input-group vis-input-wrap"></div>');

            var namediv = $('<div class="vis-control-wrap"></div>');

            var $label = $('<label>' + VIS.Msg.getMsg('Name') + '</label>');


            $name = $('<input type="text" data-placeholder="" placeholder=" ">');
            namediv.append($name).append($label);



            //

            labelshow = $('<label class="vis-ec-col-lblchkbox"><input type="checkbox">' + VIS.Msg.getMsg('ForAllWindows') + '</label>');

            $showforall = $(labelshow.find('input')[0]);
            
            var checkdiv = $('<div style="display: flex"></div>');
            $showforall
            if (VIS.Application.isRTL) {
                //$label.css({ "float": "right", "margin-right": "0px", "margin-left": "0px" })
                //$name.css({ "float": "right", "margin-right": "10px", "margin-left": "0px" });
                //$showforall.css("float", "left");
                //labelshow.css({ "float": "left", "margin-right": "5px" });
                //checkdiv.css({ "float": "right", "margin-bottom": "-5px" });
            }


            checkdiv.append(labelshow);
            $containerdiv.append($divinputwrap);
            $divinputwrap.append(namediv);
            $containerdiv.append(checkdiv);
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
