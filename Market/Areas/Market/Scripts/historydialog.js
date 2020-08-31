; (function (VIS, $) {
    function HistoryDialog(moduleID) {
        var $root = $('<div>');
        var $contentWapper = $("<div style='height: 97%;width: 95%;position: absolute;overflow:auto;'>");
        var innerDiv = $("<div>");
        $contentWapper.append(innerDiv);
        var pageNo = 0;
        var fireRequest = true;
        var $bsyDiv = $("<div class='vis-apanel-busy' style='height:98%;width:95%;position:absolute;'>");
        $root.append($contentWapper);
        $root.append($bsyDiv);
        $root.on("scroll", function () {
            if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                pageNo += 1;
                init();
            };
        });
        var init = function () {
            if (fireRequest == false) {
                return;
            }
            $bsyDiv[0].style.visibility = "visible";
            
            $.ajax({
                url: VIS.Application.contextUrl + "Module/GetModuleVersionHistory",
                dataType: "json",
                type: "POST",
                //async: false,
                data: {
                    AD_Module_ID: moduleID,
                    PageNo: pageNo
                },

                success: function (dyndata) {
                    var res = JSON.parse(dyndata.result);
                    var html = "";
                    if (res && res.length>0) {                       
                        if (res.length < 10) {
                            fireRequest = false;                           
                        }
                        for (var i = 0; i < res.length; i++) {
                            //html += "<li>" + res[i].Version + ":" + res[i].Detail + "</li>";
                            html += "<div style='border-top: 1px solid #dfdfdf;border-bottom: 1px solid #dfdfdf;padding: 10px;margin-right: 10px;float: left;width: 97%;min-height: 80px;'>" +
                                "<div style='float: left;width: 100%;overflow: hidden;'><p>" + res[i].Version + "</p></div>" +
                                "<span style='margin-top: 5px;float: left;'>" + res[i].Detail + "</span>" +
                                "</div>";
                        }
                    }
                    else {
                        fireRequest = false;
                        if (pageNo == 0) {
                            html += VIS.Msg.getMsg("FRPT_NoHistory");
                        }
                    }
                    innerDiv.append(html);
                   // $root.append(innerDiv.append(html));
                    if (pageNo == 0) {
                       
                        $root.dialog({
                            width: 620,
                            height: 400,
                            resizable: false,
                            modal: true,
                            close: onClosing,
                            title: VIS.Msg.getMsg('Market_VersionHistory')
                        });
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });


        };
        this.show = function () { init(); };
        function onClosing() {
            dispose();
        };
        var dispose = function () { };

    };
    Market.HistoryDialog = HistoryDialog;
})(VIS, jQuery);