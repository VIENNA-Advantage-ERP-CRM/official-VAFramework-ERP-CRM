; (function (VIS, $) {

    function createUser() {

        var $root = null;
        var $rootChild = null
        //  var $btnOrgAccess = null;
        var $btnCreateUser = null;
        var $btnInviteUser = null;
        var $btnSave = null;
        var $txtEmail = null;
        var $txtName = null;
        var $txtUID = null;
        var $txtPwd = null;
        var $txtMobile = null;
        var ch = null;
        var self = this;
        var orgAccesses = null;
        var $bsyDiv = null;
        var rolemodtmp, roletheModTmp = null;
        var $divRoleGroup = null;
        var roleAssigned = [];
        var createContainer = null;
        var $btnFinish = null;


        this.intialize = function () {
            createLayout();
            eventHandling();
            createBusyIndicator();
            roleTemplate();
            loadRoles();
        };

        /*
           Create busyIndicator
       */
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        }

        function createLayout() {

            $root = $('<div>');

            var str = '  <div style="overflow:hidden" class="vis-group-assinRole-content"><div class="vis-group-assinRole-data">' +
            	'<label>' + VIS.Msg.getMsg('EMail') + '</label><input  tabindex="1" class="vis-group-email" type="text" name="Name"></div>' +

            '<div style="width:auto;float:right;margin-top:5px" class="vis-group-assinRole-data">' +
              //'<a style="float:left" class="vis-group-btn vis-group-orgAcess vis-group-grayBtn">' + VIS.Msg.getMsg('VIS_OrgAccess') + '</a>' +
                               ' <a tabindex="2" class="vis-group-btn vis-group-pointer vis-group-create vis-group-grayBtn" style="float: left;">' + VIS.Msg.getMsg('Create') + '</a>' +
               '<a tabindex="3" class="vis-group-btn vis-group-pointer vis-group-invite vis-group-blueBtn" style="float: right;">' + VIS.Msg.getMsg('VIS_Invite') + '</a>' +


         '</div>' +




      '</div>';

            createContainer = $('<div class="vis-group-assignContainer" style="visibility:hidden">' +
         '<div class="vis-group-assinRole-data">' +
              '<label>' + VIS.Msg.getMsg('Name') + '</label>' +
             '<input  tabindex="4" type="text" maxlength="60"  class="vis-group-name vis-group-mandatory"  name="Name">' +
          '</div>' +


          '<div class="vis-group-assinRole-data">' +
              '<label>' + VIS.Msg.getMsg('VIS_UserID') + '</label>' +
             '<input tabindex="5" type="text" maxlength="80"  class="vis-group-uid"  name="Name">' +
          '</div>' +

          '<div class="vis-group-assinRole-data">' +
              '<label>' + VIS.Msg.getMsg('Password') + '</label>' +
             '<input tabindex="6" type="password" maxlength="250"  class="vis-group-pwd"  name="Name">' +
          '</div>' +

          '<div class="vis-group-assinRole-data">' +
              '<label>' + VIS.Msg.getMsg('Mobile') + '</label>' +
             '<input tabindex="7" type="text"  maxlength="50" class="vis-group-mobile"  name="Name">' +
          '</div>' +
                        '<a tabindex="8" class="vis-group-btn vis-group-Save vis-group-pointer vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg('Save') + '</a>' +

                        '</div>');


            $divRoleGroup = $('<div class="vis-group-roleContainer" ><div> <label >' + VIS.Msg.getMsg("SelectRole") + '</label> </div></div>');



            $root.append(str);
            $rootChild = $root.find('.vis-group-assinRole-content');
            $rootChild.height('100px');
            $btnCreateUser = $root.find('.vis-group-create');
            //  $btnOrgAccess = $root.find('.vis-group-orgAcess');
            $btnInviteUser = $root.find('.vis-group-invite');
            $btnSave = createContainer.find('.vis-group-Save');
            $txtName = createContainer.find('.vis-group-name');
            $txtUID = createContainer.find('.vis-group-uid');
            $txtMobile = createContainer.find('.vis-group-mobile');
            $txtPwd = createContainer.find('.vis-group-pwd');
            $txtEmail = $root.find('.vis-group-email');
            //$divRoleGroup = chooseRoleContainer.find('.vis-group-roleContainer');
            // $btnOrgAccess.hide();
        };

        function eventHandling() {
            $txtName.on("change", nameTextChanged);
            $btnCreateUser.on("click", create);
            $btnCreateUser.on("keydown", createTabClick);
            // $btnOrgAccess.on("click", orgAccess);
            $btnInviteUser.on("click", invite);
            $btnInviteUser.on("keydown", inviteTabClick);
            $btnSave.on("click", save);
            $btnSave.on("keydown", saveTabClick);
        };

        function createTabClick(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            create();
        };

        function saveTabClick(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            save();
        };

        function inviteTabClick(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            invite();
        };

        function nameTextChanged(e) {
            if ($txtName.val().length > 0) {
                $txtName.removeClass('vis-group-mandatory');
            }
            else {
                $txtName.addClass('vis-group-mandatory');
            }
        };

        /*
        Create Roles Template
        */
        function roleTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
                '{{#each this}}' +
                '<div class="vis-group-user-wrap vis-group-role-pad"  data-UID="{{AD_Role_ID}}">' +
                    	'<div class="vis-group-user-profile">' +
                            '<input tabindex="7"  type="checkbox" data-UID="{{AD_Role_ID}}" >' +
                            '<label>{{Name}}</label>' +
                        '</div>' +
                '</div>' +
                '{{/each}}​' +
                '</script>';

            rolemodtmp = $(script).html();
            roletheModTmp = Handlebars.compile(rolemodtmp);
        };


       // /*
       //Create Roles Template
       //*/
       // function roleTemplate() {
       //     var script = ' <script type="text/x-handlebars-template">' +
       //         '{{#each this}}' +
       //         '<div class="vis-group-user-wrap vis-group-role-pad"  data-UID="{{AD_Role_ID}}">' +
       //             	'<div class="vis-group-user-profile">' +
       //                 '{{#if IsAssignedToUser}}' +
       //                 	'<input tabindex="7" type="checkbox" data-UID="{{AD_Role_ID}}">' +
       //                     '<label style="color: #535353;font-weight: bold;">{{Name}}</label>' +
       //                     '{{else}}' +
       //                     '<input tabindex="7"  type="checkbox" data-UID="{{AD_Role_ID}}" >' +
       //                     '<label>{{Name}}</label>' +
       //                     '{{/if}}' +

       //                 '</div>' +
       //         '</div>' +
       //         '{{/each}}​' +
       //         '</script>';

       //     rolemodtmp = $(script).html();
       //     roletheModTmp = Handlebars.compile(rolemodtmp);
       // };

        /*
            Load User Roles
        */
        function loadRoles(target) {
            $bsyDiv[0].style.visibility = "visible";

            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetRoleInfo",
                data: ({ AD_User_ID: 0 }),
                success: function (result) {
                    var data = JSON.parse(result);

                    var $divRoles = $('<div style="height:230px;overflow:auto;margin-bottom:10px">');

                    $divRoles.append(roletheModTmp(data));
                    $divRoleGroup.append($divRoles);
                    $btnFinish = $('<a tabindex="400" class="vis-group-btn vis-group-pointer vis-group-finish vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg('VIS_Finish') + '</a>');
                    $divRoleGroup.append($btnFinish);
                    //$divRoleGroup.off("click");
                    //$divRoleGroup.on("click", roleContaierClick);
                    $btnFinish.on("click", finish);
                    $btnFinish.on("keydown", finishTabClick);

                    // To Show First Role sleected By Default
                    if ($($divRoleGroup.children('.vis-group-user-wrap')).length > 0) {
                        $($($divRoleGroup).children('.vis-group-user-wrap')[0]).trigger('click');
                    }

                    for (var i = 0; i < data.length; i++) {
                        roleAssigned.push({ AD_Role_ID: data[i].AD_Role_ID, IsAssignedToUser: false })
                    }

                    $($divRoleGroup.find('input')).off("click");
                    $($divRoleGroup.find('input')).on("click", roleCheckboxClick);
                    $bsyDiv[0].style.visibility = "hidden";

                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorLoadingRoles");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        function finishTabClick(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            finish();
        };

        /*
        Fire when user click on role checkbox
    */
        function roleCheckboxClick(e) {
            var target = $(e.target);

            //this is used to set if role is assigned or assigned to user.
            var element = $.grep(roleAssigned, function (ele, index) {
                return ele.AD_Role_ID == target.data('uid');
            });
            if (element != null && element.length > 0) {
                element[0].IsAssignedToUser = target.prop('checked');
            }

            e.stopPropagation();
        }


        function create() {
            $rootChild.find('.vis-group-roleContainer').css("display", "none");
            $rootChild.find('.vis-group-assignContainer').css("display", "inherit");
            $rootChild.append(createContainer);
            //$root.find('.vis-group-assignContainer').css({ "visibility": "visible", "margin-top": "-10px", "overflow": "auto", "float": "left", "height": "284px" });
            $root.find('.vis-group-assignContainer').css({ "visibility": "visible" });
            // $root.height('390px');
            $rootChild.height('369px');
            // $btnOrgAccess.show();
        };

        function orgAccess() {
            $bsyDiv[0].style.visibility = "visible";
            if ($($root.find('.vis-group-assignContainer')[0]).css('visibility') == 'visible') {
                var orgaccess = new VIS.orgAccess(self.setSelectedOrg);
                orgaccess.intialize();
                orgaccess.show();
                $bsyDiv[0].style.visibility = "hidden";
            }
        };

        function invite() {
            $rootChild.find('.vis-group-assignContainer').css("display", "none");
            $rootChild.find('.vis-group-roleContainer').css("display", "inherit");

            $rootChild.append($divRoleGroup);
            $rootChild.height('385px');


        };

        function save() {
            if ($($root.find('.vis-group-assignContainer')[0]).css('visibility') == 'visible') {
                var name = $txtName.val();
                if (name == null || name == undefined || name == "") {
                    VIS.ADialog.info("VIS_PleaseName");
                    return;
                };
                $bsyDiv[0].style.visibility = "visible";
                if (orgAccesses == null || orgAccesses == undefined) {
                    orgAccesses = null;
                }

                var pwd = VIS.secureEngine.encrypt($txtPwd.val());

                $.ajax({
                    url: VIS.Application.contextUrl + "Group/SaveNewUser",
                    type: "GET",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    data: ({ Name: name, Email: $txtEmail.val(), Value: $txtUID.val(), password: pwd, mobile: $txtMobile.val(), OrgID: JSON.stringify(orgAccesses) }),
                    success: function (result) {
                        var data = JSON.parse(result);
                        $bsyDiv[0].style.visibility = "hidden";
                        if (data == false) {
                            VIS.ADialog.error("VIS_ErrorSavingUser");
                        }
                        else {
                            ch.close();
                        }
                    },
                    error: function () {
                        $bsyDiv[0].style.visibility = "hidden";
                        VIS.ADialog.error("VIS_ErrorSavingUser");
                    }
                });
            }
        };

        function finish() {

            var emails = $txtEmail.val();
            if (emails == null || emails == undefined || emails == "") {
                VIS.ADialog.error("NotValidEmail");
                return;
            }

            var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
            if (emails.indexOf(';') == -1) {
                var isValidEmail = pattern.test(emails);
                if (!isValidEmail) {
                    VIS.ADialog.error("NotValidEmail", true, ":" + emails);
                    return;
                }
            }
            else {
                var mails = emails.split(';');
                for (var i = 0; i < mails.length; i++) {
                    var isValidEmail = pattern.test(mails[i]);
                    if (!isValidEmail) {
                        VIS.ADialog.error("NotValidEmail", true, ":" + mails[i]);
                        return;
                    }
                }
            }

            var elements = $.grep(roleAssigned, function (ele, index) {
                return ele.IsAssignedToUser == true;
            });

            if (elements == null || elements == undefined || elements.length == 0) {
                VIS.ADialog.info("VIS_SelectRole");
                return;
            }

            if (emails != null && emails != undefined && emails != "" && emails.length > 0) {
                $bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    url: VIS.Application.contextUrl + "Group/InviteUsers",
                    type: "POST",
                    datatype: "json",
                    async: true,
                    data: ({ email: emails, roles: JSON.stringify(elements) }),
                    success: function (result) {
                        var data = JSON.parse(result);
                        VIS.ADialog.info(data);
                        $bsyDiv[0].style.visibility = "hidden";
                        ch.close();
                    },
                    error: function () {
                        VIS.ADialog.error("VIS_ErrorInvitingUsers");
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });
            }
        };

        this.setSelectedOrg = function (value) {
            orgAccesses = value;
        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setContent($root);

            //ch.setHeight(115);
            ch.setWidth(550);
            ch.setTitle(VIS.Msg.getMsg("VIS_CreateUser"));
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
            this.prop = null;
            $div = null;

            ch = null;

            self = null;
            $root.remove();
            $root = null;
            //  $btnOrgAccess.remove();
            //  $btnOrgAccess = null;
            $btnCreateUser.remove();
            $btnCreateUser = null;
            $btnInviteUser.remove();
            $btnInviteUser = null;
            $btnSave.remove();
            $btnSave = null;
            $txtEmail.remove();
            $txtEmail = null;
            $txtName.remove();
            $txtName = null;
            $txtUID.remove();
            $txtUID = null;
            $txtPwd.remove();
            $txtPwd = null;
            $txtMobile.remove();
            $txtMobile = null;
            ch = null;
            self = null;
        };



    };

    VIS.createUser = createUser;


})(VIS, jQuery);