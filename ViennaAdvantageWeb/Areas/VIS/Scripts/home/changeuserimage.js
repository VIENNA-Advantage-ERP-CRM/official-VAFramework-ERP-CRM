; (function (VIS, $) {
    function changeUserImage() {
       // debugger;
        var $txtChangeStatus = $("#vis-textStatus");
        var $imgOkStatus = $("#vis-img-OKStatus");
        var $imgCancelStatus = $("#vis-img-CancelStatus");
        var $labelStatus = $("#vis-labelStatus");
        $("#vis-file-input").change(function () {
            debugger;
            if (document.getElementById('vis-file-input').files[0]!=null)
            {
            var xhr = new XMLHttpRequest();
            var fd = new FormData();
            fd.append("file", document.getElementById('vis-file-input').files[0]);
            xhr.open("POST", VIS.Application.contextUrl + "Home/SaveImageAsByte", true);
            xhr.send(fd);
            xhr.addEventListener("load", function (event) {                
                var dd = event.target.response;
                var res = JSON.parse(dd);
                var a = JSON.parse(res);             
                $("#imgUsrImage").attr('src', "data:image/jpg;base64," + a);
            }, false);
        }
        });
        function saveStatus() {
            $txtChangeStatus = $("#vis-textStatus");
            $.ajax(
                {
                    url: VIS.Application.contextUrl + "Home/SaveStatus",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    data: { status: $txtChangeStatus.val() },
                    success: function (data) {
                       // debugger;
                        if (JSON.parse(data).length > 0) {
                            $txtChangeStatus.css("visibility", "hidden");
                            $imgCancelStatus.css("visibility", "hidden");
                            $imgOkStatus.css("visibility", "hidden");
                            $labelStatus.css("visibility", "visible");
                            $labelStatus.text(JSON.parse(data));
                        }
                        else {
                            $txtChangeStatus.css("visibility", "visible");
                            $imgCancelStatus.css("visibility", "hidden");
                            $imgOkStatus.css("visibility", "hidden");
                            $labelStatus.css("visibility", "visible");
                            $labelStatus.text(JSON.parse(data));
                        }
                    }
                });
        }
        $imgOkStatus.on("click", function () {
            //debugger;
            saveStatus();
        });
        $imgCancelStatus.on("click", function () {
           // debugger;
            $txtChangeStatus.css("visibility", "hidden");
            $imgCancelStatus.css("visibility", "hidden");
            $imgOkStatus.css("visibility", "hidden");
            $labelStatus.css("visibility", "visible");
        });
        $txtChangeStatus.on("focus", function () {
          //  debugger;
            $imgCancelStatus.css("visibility", "visible");
            $imgOkStatus.css("visibility", "visible");
        });
        $txtChangeStatus.on("keypress", function (e) {
            //debugger;
            var key = e.keyCode || e.which;
            if (key == 13) {
                saveStatus();
            }
        });
        $txtChangeStatus.on("blur", function () {
           // debugger;
            //$("#vis-img-CancelStatus").css("visibility", "hidden");
            //$("#vis-img-OKStatus").css("visibility", "hidden");
        });
        $labelStatus.on("click", function () {
            $labelStatus.css("visibility", "hidden");
            $txtChangeStatus.css("visibility", "visible");
            //$imgCancelStatus.css("visibility", "visible");
            //$imgOkStatus.css("visibility", "visible");
            
            $txtChangeStatus.val($labelStatus.text());
            $txtChangeStatus.focus();

        });
    };
    VIS.changeUserImage = changeUserImage;
})(VIS, jQuery);