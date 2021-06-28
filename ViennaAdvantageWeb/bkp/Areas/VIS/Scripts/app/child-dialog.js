
; (function (VIS, $) {

    function ChildDialog() {
        var $a = $("<div></div>");
        var title = "Dialog";
        var height = "auto";
        var width = "auto";
        var modal = true;
        var removeClose = false;
        var resize = true;
        // my: "center top", at: "center top+75", of: "#mycontainer
        var position = { at: "center center", of: window };

        this.onOkClick = null;
        this.onCancelClick = null;
        this.onClose = null;


        this.setTitle = function (t) {
            title = t;
        }

        this.setModal = function (m) {
            modal = m;
        }
        this.changeTitle = function (t) {
            $a.dialog('widget').find(".ui-dialog-title").text(t);
        }

        //this.removeCloseBtn = function (remove) {
        //    removeClose = remove;
        //}

        this.setHeight = function (h) { height = h; };
        this.setWidth = function (w) { width = w; };
        this.setPosition = function (p) { position = p; };

        this.setEnableResize = function (isResize) { resize = isResize; };

        var self = this;

        this.setContent = function (content) {
            $a.empty();

            //content.css("margin-left", "-6px");
            //content.css("margin-right", "-7px");
            //content.css("margin-top", "3px");
            $a.append(content);
        };
        this.getRoot = function () {
            return $a;
        };


        function onClosing() {
            if (self.onClose)
                self.onClose();
            self.dispose();
            self = null;
        }

        this.hidebuttons = function () {
            $a.dialog('widget').find('.ui-dialog-buttonpane .ui-button').hide();
            $a.dialog('widget').find('.ui-widget-content').css('border', '0px');
            //$a.dialog('widget').removeClass('.ui-dialog');
            //$a.dialog('widget').removeClass('.ui-dialog-buttonpane');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('padding', '0px');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('margin-top', '0px');
            $a.dialog('widget').find('.ui-dialog .ui-dialog-buttonpane').hide();
        };

        this.hideButtons = function () {
            $a.dialog('widget').find('.ui-dialog-buttonpane .ui-button').hide();
            $a.dialog('widget').find('.ui-widget-content').css('border', '0px');
            //$a.dialog('widget').removeClass('.ui-dialog');
            //$a.dialog('widget').removeClass('.ui-dialog-buttonpane');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('padding', '0px');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('margin-top', '0px');
            $a.dialog('widget').find('.ui-dialog .ui-dialog-buttonpane').hide();
        };



        this.close = function () {
            $a.dialog("close");
            this.dispose();
        }


        this.show = function () {
            var styleCancel = "";
            var styleOK = "margin-right:12px";
            if (VIS.Application.isRTL) {
                styleCancel = "margin-right:7px;";
                styleOK = "";
            }
            $a.dialog({
                height: height,
                width: width,
                title: title,
                modal: modal,
                resizable: resize,
                closeOnEscape: !removeClose,
                closeText: VIS.Msg.getMsg("close"),
                //zIndex: 999999,
                 position: position ,
                buttons: [
                                {
                                    text: VIS.Msg.getMsg("Ok"),
                                    click: function (evt) {
                                        var buttonDomElement = evt.target;
                                        // Disable the button 
                                        //$(buttonDomElement).css('background-color', 'red');
                                        //$(buttonDomElement).css('color', 'blue');
                                        $(buttonDomElement).attr('disabled', true);

                                        var res = true;
                                        if (self.onOkClick) {
                                            res = self.onOkClick(evt);
                                        }
                                        if (res == true || res == undefined) {
                                            if ($a != null)
                                                $a.dialog("close");
                                        }
                                        else {
                                            $(buttonDomElement).attr('disabled', false);
                                        }
                                    },
                                    style: styleOK
                                },
                {
                    text: VIS.Msg.getMsg("Cancel"),
                    click: function () { if (self.onCancelClick) self.onCancelClick(); if ($a) $a.dialog("close"); },
                    style: styleCancel
                }
                ]
                ,
                close: onClosing
            });
            //Work Done to Prevent Alt+Left Arrow event due to which back fuctionality of browser executes.
            $(document).on('keydown', function (e) {
                if (event.altKey && event.keyCode == 37) {
                    e.preventDefault();
                    return false;
                }
                e.stopPropagation();
            });

            //window.setTimeout(function () {
            //    $('.ui-widget-content').css("z-index", "999999");
            //});
            //$('.ui-dialog .ui-dialog-buttonpane').css("margin-top", "-10px");
            $('.ui-dialog .ui-dialog-buttonpane').css("border-width", "0 0 0");

            if (removeClose)
                $('.ui-dialog-titlebar-close').remove();


            return $a;
        };

        //$('.ui-dialog .ui-dialog-buttonpane').css("margin", "0px");
        //$('.ui-dialog .ui-dialog-buttonpane').css("padding", "0px");
        this.dispose = function () {
            if ($a != null) {
                $a.dialog('destroy');
                $a.remove();
            }
            $a = null;
            this.Show = null;
            this.setContent = null;
            this.setTitle = null;
        }
    };

    VIS.ChildDialog = ChildDialog;

})(VIS, jQuery);