; (function (VIS, $) {

    function VImageForm(ad_image_id, textLength) {
        var $self = this;
        var $root = $("<div>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>')
        var windowNo = VIS.Env.getWindowNo();
        var Okbtn = null;
        var cancelbtn = null;
        var delbtn = null;
        this.onClose = null;

        var lblImageSelect = null;
        var fileUpload = null;
        var holderDiv = null;
        var imgUsrImage = null;
        var lblfileUpload = null;
        var chkDataBaseSave = null;
        var change = false;
        var w = null;
        var h = null;

        if (ad_image_id == null || ad_image_id == 'null')
        {
            ad_image_id = 0;
        }

        var load = function () {
            $root.load(VIS.Application.contextUrl + 'VImageForm/Index/?windowno=' + windowNo + '&ad_image_id=' + ad_image_id, function (event) {
                $busyDiv[0].style.visibility = 'visible';
                init($root);
                //remove image
                $busyDiv[0].style.visibility = 'hidden';
            });
        };

        var init = function (root) {
            Okbtn = $root.find("#btnOk_" + windowNo);
            cancelbtn = $root.find("#btnCancel_" + windowNo);
            delbtn = $root.find("#btnImgDelete_" + windowNo);

            //if (VIS.Application.isRTL) {
            //    Okbtn.css("margin-right", "-128px");
            //    cancelbtn.css("margin-right", "110px");
            //    delbtn.css("margin-right", "55px");
            //}
            // if ad_image_id > 0, it is for update or delete
            if (ad_image_id > 0) {
                delbtn.css("display", "inline-block");
            }
            else {
                delbtn.css("display", "none");
            }

            //lblImageSelect = root.find("#lblImageSelect_" + windowNo);
            fileUpload = root.find("#fileUpload_" + windowNo);
            holderDiv = root.find("#holderDiv_" + windowNo);
            imgUsrImage = root.find("#imgUsrImage_" + windowNo);
            holderDiv.css("width", w - 48);
            if (h > 180)
                holderDiv.css("height", h - 180);
            if (textLength > 0) {
                imgUsrImage.hide();
            } else {

                if (imgUsrImage[0].width > 300) { // holder width
                    holderDiv.css("overflow", "auto");
                }
            }

            lblfileUpload = root.find("#lblfileUpload_" + windowNo);
            chkDataBaseSave = root.find("#chkDataBaseSave_" + windowNo);

            fileUpload.on("change", function () {
                imgUsrImage.show();
                var file = fileUpload[0].files[0],
                    reader = new FileReader();
                reader.onload = function (event) {
                    imgUsrImage.removeAttr("src").attr("src", event.target.result);

                    // if (imgUsrImage[0].width > 300) { // holder width
                    holderDiv.css("overflow", "auto");
                    //
                    // }
                };
                reader.readAsDataURL(file);
                change = true;
                reader = null;
                file = null;
                return false;
            });

            // set change to true if isdatabase checkbox is changed true/false
            chkDataBaseSave.on("change", function () {
                change = true;
                return false;
            });

            Okbtn.on("click", function () {
                if (change) {
                    var xhr = new XMLHttpRequest();
                    var fd = new FormData();
                    fd.append("file", fileUpload[0].files[0]);
                    fd.append("isDatabaseSave", chkDataBaseSave.prop("checked"));
                    fd.append("ad_image_id", ad_image_id);
                    xhr.open("POST", VIS.Application.contextUrl + "VImageForm/SaveImage", true);
                    xhr.send(fd);
                    xhr.addEventListener("load", function (event) {
                        var dd = event.target.response;
                        dd = JSON.parse(dd);
                        var id = dd.result;
                        if ($self.onClose)
                            $self.onClose(id, change);
                        xhr = null;
                        fd = null;
                        $root.dialog('close');

                    }, false);
                }
                else {
                    if ($self.onClose)
                        $self.onClose(ad_image_id, change);
                    $root.dialog('close');
                }

            });

            cancelbtn.on("click", function () {
                $root.dialog('close');
            });
            // delete image function
            delbtn.on("click", function () {
                change = true;
                var xhr = new XMLHttpRequest();
                var fd = new FormData();
                fd.append("ad_image_id", ad_image_id);
                xhr.open("POST", VIS.Application.contextUrl + "VImageForm/DeleteImage", true);
                xhr.send(fd);
                xhr.addEventListener("load", function (event) {
                    var dd = event.target.response;
                    dd = JSON.parse(dd);
                    var id = dd.result;
                    if ($self.onClose)
                        $self.onClose(id, change);
                    xhr = null;
                    fd = null;
                    $root.dialog('close');

                }, false);
            });

        };

        this.showDialog = function () {
            load();
            w = $(window).width() - 150;
            h = $(window).height() - 40;

            $busyDiv.height(h);
            $busyDiv.width(w);
            $root.append($busyDiv);
            $root.dialog({
                modal: true,
                title: VIS.Msg.getMsg("ImageDialog"),
                width: w,
                height: h,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });
            // $root.siblings('div.ui-dialog-titlebar').remove();
        };

        this.disposeComponent = function () {
            $self = null;
            if (Okbtn)
                Okbtn.off("click");
            if (cancelbtn)
                cancelbtn.off("click");
            if (delbtn)
                delbtn.off("click");

            $busyDiv = null;
            windowNo = null;
            Okbtn = null;
            cancelbtn = null;
            delbtn = null;
            lblImageSelect = null;
            fileUpload = null;
            holderDiv = null;
            imgUsrImage = null;
            lblfileUpload = null;
            chkDataBaseSave = null;
            change = false;
            w = null;
            h = null;

            this.disposeComponent = null;
        };

    }

    //dispose call
    VImageForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    //Load form into VIS
    VIS.VImageForm = VImageForm;

})(VIS, jQuery);