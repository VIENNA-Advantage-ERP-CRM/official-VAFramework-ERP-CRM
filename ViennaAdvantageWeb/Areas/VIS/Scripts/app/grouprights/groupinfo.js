; (function (VIS, $) {

    function groupInfo(groupID) {

        var $root = null;
        var ch = null;
        this.intialize = function () {
            createLayout();
        };

        function createLayout() {
            $root = $('<div>');
            getGroupInfo();

        };

        function getGroupInfo() {
            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetGroupChildInfo",
                data: { groupID: groupID },
                success: function (result) {
                    var data = JSON.parse(result);

                    var script = '<div class="vis-group-gi-content">' +
                                        '<label>' + data.GroupName + '</label>';
                    if (data.Description != null && data.Description != undefined && data.Description != "") {
                        script += '<p>' + data.Description + '</p>';
                    }

                    if (data.WindowName != null && data.WindowName != undefined && data.WindowName != "") {
                        script += '<div class="vis-group-gi-data">' +
                             '<label>' + VIS.Msg.getMsg("Windows") + ':</label>' +
                                '<p>' + data.WindowName + '</p>';
                        '</div>';
                    }

                    if (data.FormName != null && data.FormName != undefined && data.FormName != "") {
                        script += '<div class="vis-group-gi-data">' +
                             '<label>' + VIS.Msg.getMsg("VIS_Forms") + ':</label>' +
                                  '<p>' + data.FormName + '</p>';
                        '</div>';
                    }

                    if (data.ProcessName != null && data.ProcessName != undefined && data.ProcessName != "") {
                        script += '<div class="vis-group-gi-data">' +
                                    '<label>' + VIS.Msg.getMsg("VIS_Processes") + ':</label>' +
                                      '<p>' + data.ProcessName + '</p>';
                        '</div>';
                    }

                    if (data.WorkflowName != null && data.WorkflowName != undefined && data.WorkflowName != "") {
                        script += '<div class="vis-group-gi-data">' +
                                '<label>' + VIS.Msg.getMsg("VIS_Workflows") + ':</label>' +
                                  '<p>' + data.WorkflowName + '</p>';
                        '</div>';
                    }



                    script += '</div>';

                    $root.append(script);

                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorGettingGroupInfo");
                }
            });

            var script = "";

        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setContent($root);

            ch.setHeight(500);
            ch.setWidth(550);
            ch.setTitle(VIS.Msg.getMsg("VIS_Groupinfo"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {

                if (self.onClose) self.onClose();
                self.dispose();
            };
            ch.show();
            ch.hideButtons();
        };


        this.dispose = function () {
            self = null;
            $root.remove();
            $root = null;
            ch = null;
        };


    };
    VIS.groupInfo = groupInfo;


})(VIS, jQuery);