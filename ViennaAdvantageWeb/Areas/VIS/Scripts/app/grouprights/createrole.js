; (function (VIS, $) {

    function createrole(groupID) {

        var $root = $('<div>');
        var ch = null;
        var $txtRole = null;
        var $cmbUserLevel = null;
        var $btnOrgAccess = null;
        var $btnSave = null;
        var orgAccesses = null;
        var self = this;
        var $bsyDiv = null;

        this.intialize = function () {
            //createBusyIndicator();
            createLayout();
            eventHandling();
        };

        function createLayout() {


            var script = '<div class="vis-group-assinRole-content">' +
        	'<div class="vis-group-assinRole-data">' +
            	'<label>' + VIS.Msg.getMsg("Role") + '</label><input tabindex="1" maxlength="60" class="vis-group-role-text vis-group-mandatory" type="text">' +
            '</div>' +
            '<div class="vis-group-assinRole-data">' +
            	'<label>' + VIS.Msg.getMsg("VIS_UserLevel") + '</label>' +
                '<select  tabindex="2"  class="vis-group-role-select" >' +

                '</select>' +
            '</div>' +
            '<div class="vis-group-assinRole-data">' +
                '<a style="float:left"  tabindex="3"  class="vis-group-btn vis-group-orgAccess vis-group-pointer vis-group-grayBtn">' + VIS.Msg.getMsg("VIS_OrgAccess") + '</a>' +
                '<a  tabindex="4"  class="vis-group-btn vis-group-Save vis-group-pointer vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg("Save") + '</a>' +
            '</div>' +
        '</div>';

            $root.append(script);
            $txtRole = $root.find('.vis-group-role-text');
            $cmbUserLevel = $root.find('.vis-group-role-select');
            $btnOrgAccess = $root.find('.vis-group-orgAccess');
            $btnSave = $root.find('.vis-group-Save');

            createBusyIndicator();
            $bsyDiv[0].style.visibility = "visible";


            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetUserLevel",
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data != null && data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $cmbUserLevel.append('<option value="' + data[i].ID + '">' + data[i].Name + '</option>');
                        }
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });



        };

        /*
         Create busyIndicator
     */
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        }

        function eventHandling() {
            $btnOrgAccess.on("click", orgAccess);
            $btnOrgAccess.on("keydown", orgAccessTabClick);
            $btnSave.on("click", save);
            $btnSave.on("keydown",saveTabClick);
            $txtRole.on("change", checkRoleName);
        };

        function orgAccessTabClick(e)
        {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            orgAccess();
        };

        function saveTabClick(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            save();
        };

        function checkRoleName() {
            if ($txtRole.val().length > 0) {
                $txtRole.removeClass('vis-group-mandatory');
            }
            else {
                $txtRole.addClass('vis-group-mandatory');
            }
        };

        function orgAccess() {
            var orgaccess = new VIS.orgAccess(self.setSelectedOrg);
            window.setTimeout(function () {
                orgaccess.intialize();
                orgaccess.show();
            }, 20);
        };

        function save() {
            var roleName = $txtRole.val();
            var usrLEvel = $cmbUserLevel.val();
            if (roleName == null || roleName == undefined || roleName == "") {
                VIS.ADialog.info("VIS_PleaseName");
                return;
            }

            if (orgAccesses == undefined || orgAccesses == null || orgAccesses == "" || orgAccesses.length == 0) {
                VIS.ADialog.info("VIS_SelectOrgAccess");
                return;
            }


            if (usrLEvel == null || usrLEvel == undefined || usrLEvel == "" || usrLEvel == -1) {
                VIS.ADialog.info("VIS_SelectUserLevel");
                return;
            }
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "Group/AddNewRole",
                data: { name: roleName, userLevel: usrLEvel, orgID: JSON.stringify(orgAccesses) },
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data == false) {
                        VIS.ADialog.error("VIS_ErrorSavingRole");
                    }
                    else {
                        ch.close();
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorSavingRole");
                    $bsyDiv[0].style.visibility = "hidden";
                }

            });
            //AddNewRole
        };

        this.setSelectedOrg = function (value) {
            orgAccesses = value;
        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setContent($root);

            //ch.setHeight(428);
            ch.setWidth(550);
            ch.setTitle(VIS.Msg.getMsg("VIS_CreateRole"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {
                self.dispose();
                self = null;
            };
            ch.show();
            ch.hideButtons();
        };


        this.dispose = function () {

            $txtRole.remove();
            $txtRole = null;
            $cmbUserLevel.remove();
            $cmbUserLevel = null;
            $btnOrgAccess.remove();
            $btnOrgAccess = null;
            $btnSave.remove();
            $btnSave = null;

            $root.remove();
            $root = null;
            ch = null;
        };
    };
    VIS.createrole = createrole;


})(VIS, jQuery);