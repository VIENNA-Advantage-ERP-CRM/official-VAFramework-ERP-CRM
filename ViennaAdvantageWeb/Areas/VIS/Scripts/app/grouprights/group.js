; (function (VIS, $) {

    function GroupForm() {
        this.frame = null;
        this.windowNo = null;
        var self = this;
        var $root = $('<div class="vis-group-assign-content">');
        var $menu = $('<ul class="vis-apanel-rb-ul"></ul>');
        var $leftPanel = null;
        var $middlePanel = null;
        var $rightPanel = null;
        var $divUserGroup = null;
        var $searchUser = null;
        var $searchUserBtn = null;
        var $btnCreateUser = null;
        var $btnSortUsers = null;
        // var $btnImportUser = null;
        var $btnCreateRole = null;
        var $btnSaveRoles = null;
        var $divRoleGroup = null;
        var $bsyDiv = null;
        var $searchRole = null;
        var $searchRoleBtn = null;
        var $searchGroup = null;
        var $searchGroupBtn = null;
        var $btnCreateGroup = null;
        var $btnSaveGroup = null;
        var $divGroupsGroup = null;
        var usermodtmp, usertheModTmp, rolemodtmp, roletheModTmp, groupmodtmp, grouptheModTmp;

        var sortby = -1;
        var userID = 0;
        var roleAssigned = [];
        var groupAssigned = [];
        var roleChanged = [];
        var groupChanged = [];
        var roleID = 0;
        var groupID = 0;
        var groupWindowID = 0;
        var roleWindowID = 0;
        var userWindowID = 0;
        var pageNo = 0;
        var PAGESIZE = 20;
        var roleContainerClicked = false;
        var groupContainerClicked = false;

        /*
            Initialize Components
        */
        this.initializeComponent = function () {
            createPanels();
            eventHandling();
            createSortingOverlay();
        };

        /*
            Create layouts of Left , Middle and Right Panels
        */
        function createPanels() {
            $root.append('<div class="col-lg-4 col-md-4"><div class="vis-group-content-wrap"></div></div><div class="col-lg-4 col-md-4"><div class="vis-group-content-wrap"></div></div><div class="col-lg-4 col-md-4 last-child"><div class="vis-group-content-wrap"></div></div>');
            var panels = $root.find('.vis-group-content-wrap');
            $leftPanel = $(panels[0]);
            $middlePanel = $(panels[1]);
            $rightPanel = $(panels[2]);
            panels = null;
            createBusyIndicator();
            leftPanel();
            middlePanel();
            rightPanel();
        };

        /*
            Create busyIndicator
        */
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        }

        /*
            Create Event Handlars
        */
        function eventHandling() {
            $btnCreateUser.on("click", creatNewUser);
            $searchUserBtn.on("click", loadUser);
            $searchUser.on("keydown", searchUsers);
            //  $btnImportUser.on("click", importUser);
            $btnSaveRoles.on("click", saveUserRoles);
            $btnSaveGroup.on("click", saveUserGroups);
            $searchGroupBtn.on("click", loadGroup);
            $searchGroup.on("keydown", searchGroups);
            $searchRoleBtn.on("click", loadRoles);
            $searchRole.on("keydown", searchRoles);
            $btnCreateGroup.on("click", createGroup);
            $btnCreateRole.on("click", createNewRole);
            $divUserGroup.on("scroll", userScroll);
            $root.on("click", rootClick);

        };

        /*
           Root Click Event Used for asking user to save roles and groups which may left unsaved.
        */
        function rootClick(e) {
            var target = $(e.target);
            // In Case of Role...
            if (!target.hasClass("vis-group-roless") && target.parents(".vis-group-roless").length == 0 && !target.hasClass('vis-group-ass-btns vis-group-save-btn') && roleChanged.length > 0) {


                VIS.ADialog.confirm('VIS_SaveRole', true, "", "Confirm", function (result) {
                    if (result) {
                        saveUserRoles();
                    }
                    else {
                        roleChanged = [];
                    }
                });

                e.stopPropagation();
                return false;

            }

                // In Case of Group
            else if (!target.hasClass("vis-group-groupss") && target.parents(".vis-group-groupss").length == 0 && !target.hasClass('vis-group-ass-btns vis-group-save-btn') && groupChanged.length > 0) {
                // if (VIS.ADialog.ask('VIS_SaveGroup')) {

                VIS.ADialog.confirm('VIS_SaveRole', true, "", "Confirm", function (result) {
                    if (result) {
                        saveUserGroups();
                    }
                    else {
                        groupChanged = [];
                    }
                });
                e.stopPropagation();
                return false;
            }
            //roleContainerClicked = false;
            //groupContainerClicked = false;
        };

        /*
            Append items in Left Panel
        */
        function leftPanel() {
            $bsyDiv[0].style.visibility = "visible";
            $leftPanel.append('<div class="vis-group-content-head"><h4 class="vis-group-content-tittle vis-group-createUser">' + VIS.Msg.getMsg("VIS_CreateInviteUser") + '</h4>' +
                  '<span class=" vis-group-pointer vis-group-head-right">' + VIS.Msg.getMsg("SortBy") + '</span><div class="vis-group-clear-both"></div></div>');
            $btnSortUsers = $leftPanel.find('.vis-group-head-right');

            /***** end of content-head *****/

            $leftPanel.append('<div class="vis-group-content-headDown"><input class="vis-group-add-btn vis-group-pointer vis-group-addLeft vis-group-ass-btns" type="button">' +
                    '<input class="vis-group-SearchText" value="" placeholder="' + VIS.Msg.getMsg("Search") + '" type="text"><input class=" vis-group-pointer vis-group-ass-btns vis-group-search-icon" type="button">' +
                 //   '<input class="vis-group-importUser" type="button" title="' + VIS.Msg.getMsg("VIS_Importuser") + '"><div class="vis-group-clear-both"></div> </div>');
                    '<div class="vis-group-clear-both"></div> </div>');
            $searchUser = $leftPanel.find('.vis-group-SearchText');
            $searchUserBtn = $leftPanel.find('.vis-group-search-icon');
            // $btnImportUser = $leftPanel.find('.vis-group-importUser');
            $btnCreateUser = $leftPanel.find('.vis-group-add-btn');


            /*****  end of content-headDown  *****/
            $divUserGroup = $('<div class="vis-group-users-container">').append('<div style="border-top: 1px solid #dfdfdf; width: 97%;"></div>');
            $divUserGroup.height($($root.parent()).height() - 95);
            $leftPanel.append($divUserGroup);

            createUsersList();
        };

        /*
            Append Items in middle Panel
        */
        function middlePanel() {
            $middlePanel.append(' <div class="vis-group-content-head">' +
                      '<h4 class="vis-group-content-tittle vis-group-role">' + VIS.Msg.getMsg("Role") + '</h4>' +
                      '<h7 class="vis-group-SaveMessage">' + VIS.Msg.getMsg("VIS_RoleSaved") + '</h7>' +

                      '<div class="vis-group-top-right">' +
                          '<input class="vis-group-ass-btns vis-group-pointer vis-group-add-btn" type="button">' +
                          '<input class="vis-group-ass-btns vis-group-pointer vis-group-save-btn" type="button">' +
                      '</div>' +
                      '<div class="vis-group-content-headDown"><span><input class="vis-group-SearchText" value="" placeholder="' + VIS.Msg.getMsg('Search') + '" type="text">' +
                      '<input class="vis-group-ass-btns vis-group-pointer vis-group-search-icon" type="button"> </span></div>' +
                      '<div class="vis-group-clear-both"></div>' +
                  '</div>');

            $btnSaveRoles = $middlePanel.find('.vis-group-save-btn');
            $btnCreateRole = $middlePanel.find('.vis-group-add-btn');
            $searchRole = $middlePanel.find('.vis-group-SearchText');
            $searchRoleBtn = $middlePanel.find('.vis-group-ass-btns');
            /**end of content-head**/

            $divRoleGroup = $('<div class="vis-group-role-container">');
            $divRoleGroup.append('<div style="border-top: 1px solid #dfdfdf; width: 97%;"></div>');
            $middlePanel.append($divRoleGroup);
            $divRoleGroup.height($($root.parent()).height() - 95);
            roleTemplate();

        };

        /*
            Append Items in right Panel
        */
        function rightPanel() {
            $rightPanel.append('<div class="vis-group-content-head">' +
                       '<h4 class="vis-group-content-tittle vis-group-rights">' + VIS.Msg.getMsg("VIS_Groups") + '</h4>' +
                         '<h7 class="vis-group-SaveMessage">' + VIS.Msg.getMsg("VIS_GroupSaved") + '</h7>' +
                       '<div class="vis-group-top-right">' +
                           '<div class="vis-group-top-right">' +
                           '<input class="vis-group-ass-btns vis-group-pointer vis-group-add-btn" type="button">' +
                           '<input class="vis-group-ass-btns vis-group-pointer vis-group-save-btn" type="button">' +
                           '</div>' +
                       '</div>' +

                       '<div class="vis-group-content-headDown"><span>' +

                       '<input class="vis-group-SearchText"  value="" placeholder="' + VIS.Msg.getMsg('Search') + '" type="text">' +
                      '<input class="vis-group-ass-btns vis-group-pointer vis-group-search-icon" type="button"> ' +
                       '</span>' +
                       '</div>' +

                       '<div class="vis-group-clear-both"></div>' +
                   '</div>');
            $searchGroup = $rightPanel.find('.vis-group-SearchText');
            $searchGroupBtn = $rightPanel.find('.vis-group-ass-btns');
            $btnCreateGroup = $rightPanel.find('.vis-group-add-btn');
            $btnSaveGroup = $rightPanel.find('.vis-group-save-btn');


            $divGroupsGroup = $('<div class="vis-group-role-container">').append('<div style="vis-group-border-top: 1px solid #dfdfdf; width: 97%;"></div>');

            $rightPanel.append($divGroupsGroup);
            $divGroupsGroup.height($($root.parent()).height() - 95);
            groupTemplate();

        };

        /*
            Create Overlay for sorting Users
        */
        function createSortingOverlay() {
            $menu.append('<li value=1>' + VIS.Msg.getMsg("Name") + '</li>');
            $menu.append('<li value=2>' + VIS.Msg.getMsg("SearchKey") + '</li>');
            $menu.append('<li value=3>' + VIS.Msg.getMsg("EMail") + '</li>');

            $btnSortUsers.on("click", function () {
                $(this).w2overlay($menu.clone(true));
            });

            $menu.on(VIS.Events.onTouchStartOrClick, "LI", function (e) {
                sortby = $(this).val();
                loadUser();
            });
        };

        /*
            createUsersList
        */
        function createUsersList() {
            userTemplate();
            loadUser();
        };

        /*
            Create Role List
        */

        function createRoleList() {
            roleTemplate();
        };

        /*
            Create Users Template
        */
        function userTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
                 '{{#each this}}' +
            '<div class="vis-group-user-wrap"  data-UID="{{AD_UserID}}">' +
             '<div class="vis-group-user-profile vis-group-pro-width">' +
                        	'<div style=" height:46px;width:46px" class="vis-group-user-img">' +
                            '{{#if UserImage}}' +
                            	'<img src="' + VIS.Application.contextUrl + '{{UserImage}}" alt="user-img">' +
                                '{{else}}' +
                                '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/home/defaultUser46X46.PNG" alt="user-img">' +
                                '{{/if}}' +
                            '</div>' +
                                        '<div class="vis-group-user-text">' +
                                            '{{#if IsActive }}' +
                            	            '<p style="font-weight: bold">{{Username}}</p>' +
                                            '{{else}}' +
                                            '<p>{{Username}}</p>' +
                                             '{{/if}}' +
                                            '<span>{{Email}}</span>' +
                                        '</div>' +
                        '</div>' +

                        '<div class="vis-group-user-right">' +
                        	'<ul>' +
                            '{{#if IsActive }}' +
                                        '{{#if IsUpdate}}' +
                                            '<li><span class="vis-group-user-ico vis-group-pointer vis-group-activeUser" data-UID="{{AD_UserID}}"></span></li>' +           // if selected user can be updated
                                        '{{else}}' +
                                            '<li><span disabled class="vis-group-user-ico vis-group-pointer vis-group-activeUser" data-UID="{{AD_UserID}}"></span></li>' +           // if selected user cannot be updated
                                            '{{/if}}' +
                                 '{{else}}' +
                                        '{{#if IsUpdate}}' +
                                        '<li><span class="vis-group-user-ico vis-group-pointer vis-group-inactiveUser"  data-UID="{{AD_UserID}}"></span></li>' +           // if selected user can be updated
                                        '{{else}}' +
                                        '<li><span disabled class="vis-group-user-ico vis-group-pointer vis-group-inactiveUser"  data-UID="{{AD_UserID}}"></span></li>' +           // if selected user cannot be updated
                                         '{{/if}}' +
                                 '{{/if}}' +
                            	'<li><span class="vis-group-user-ico vis-group-pointer vis-group-edit" data-UID="{{AD_UserID}}-{{UserTableID}}-{{UserWindowID}}"></span></li>' +
                            '</ul>' +
                            '<p>{{Country}}</p>' +
                        '</div>' +
            '</div>' +

            //'{{else}}' +

            // '<div class="vis-group-user-wrap"  data-UID="{{AD_UserID}}">' +
            // '<div class="vis-group-user-profile vis-group-pro-width">' +
            //            	'<div style=" height:46px;width:46px" class="vis-group-user-img">' +
            //                '{{#if UserImage}}' +
            //                	'<img src="' + VIS.Application.contextUrl + '{{UserImage}}" alt="user-img">' +
            //                    '{{else}}' +
            //                    '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/home/defaultUser46X46.PNG" alt="user-img">' +
            //                    '{{/if}}' +
            //                '</div>' +
            //                            '<div class="vis-group-user-text">' +
            //                                '{{#if IsActive }}' +
            //                	            '<p style="font-weight: bold">{{Username}}</p>' +
            //                                '{{else}}' +
            //                                '<p>{{Username}}</p>' +
            //                                 '{{/if}}' +
            //                                '<span>{{Email}}</span>' +
            //                            '</div>' +
            //            '</div>' +

            //            '<div class="vis-group-user-right">' +
            //            	'<ul>' +
            //                '{{#if IsActive }}' +
            //                    '<li><span disabled class="vis-group-user-ico vis-group-pointer vis-group-activeUser" data-UID="{{AD_UserID}}"></span></li>' +
            //                     '{{else}}' +
            //                     '<li><span disabled class="vis-group-user-ico vis-group-pointer vis-group-inactiveUser"  data-UID="{{AD_UserID}}"></span></li>' +
            //                     '{{/if}}' +
            //                	'<li><span class="vis-group-user-ico vis-group-pointer vis-group-edit" data-UID="{{AD_UserID}}-{{UserTableID}}-{{UserWindowID}}"></span></li>' +
            //                '</ul>' +
            //                '<p>{{Country}}</p>' +
            //            '</div>' +
            //'</div>' +

            // '{{/if}}' +

            '{{/each}}​' +
            '</script>';
            usermodtmp = $(script).html();
            usertheModTmp = Handlebars.compile(usermodtmp);
        };

        /*
            Create Roles Template
        */
        function roleTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
                '{{#each this}}' +
                '<div class="vis-group-user-wrap vis-group-roless vis-group-role-pad"  data-UID="{{AD_Role_ID}}">' +
                    	'<div class="vis-group-user-profile">' +
                        '{{#if IsAssignedToUser}}' +
                        '{{#if IsUpdate}}' +
                        	'<input type="checkbox" data-UID="{{AD_Role_ID}}" checked="">' +            // if selected user can be updated
                            '{{else}}' +
                            '<input disabled type="checkbox" data-UID="{{AD_Role_ID}}" checked="">' +            // if selected user cannot be updated
                            '{{/if}}' +
                            '<label style="color: #535353;font-weight: bold;">{{Name}}</label>' +
                            '{{else}}' +
                            '{{#if IsUpdate}}' +
                            '<input type="checkbox" data-UID="{{AD_Role_ID}}" >' +                                  // if selected user can be updated
                            '{{else}}' +
                             '<input disabled type="checkbox" data-UID="{{AD_Role_ID}}" >' +                 // if selected user cannot be updated
                             '{{/if}}' +
                            '<label>{{Name}}</label>' +
                            '{{/if}}' +

                        '</div>' +
                            '<div class="vis-group-user-right">' +
                        	'<ul>' +
                            	'<li><span class="vis-group-user-ico vis-group-pointer vis-group-edit"  data-UID="{{AD_Role_ID}}-{{roleWindowID}}"></span></li>' +
                            '</ul>' +
                            '</div>' +
                '</div>' +
                '{{/each}}​' +
                '</script>';

            rolemodtmp = $(script).html();
            roletheModTmp = Handlebars.compile(rolemodtmp);
        };

        /*
           Create Group Template
        */
        function groupTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
                '{{#each this}}' +
                '<div class="vis-group-user-wrap vis-group-groupss vis-group-role-pad" data-UID="{{AD_Group_ID}}">' +
                '<div class="vis-group-user-profile">' +
                    '{{#if IsAssignedToUser}}' +
                        	'<input type="checkbox" data-UID="{{AD_Group_ID}}" checked="">' +
                            '<label style="color: #535353;font-weight: bold;">{{Name}}</label>' +
                            '{{else}}' +
                            '<input type="checkbox" data-UID="{{AD_Group_ID}}" >' +
                            '<label>{{Name}}</label>' +
                            '{{/if}}' +

                '</div>' +
                '<div class="vis-group-user-right">' +
                    '<ul>' +
                        '<li><span class="vis-group-user-ico vis-group-info vis-group-pointer"   data-UID="{{AD_Group_ID}}-{{GroupWindowID}}"></span></li>' +
                        '<li><span class="vis-group-user-ico vis-group-edit vis-group-pointer"   data-UID="{{AD_Group_ID}}-{{GroupWindowID}}"></span></li>' +
                    '</ul>                           ' +
                '</div>' +
            '</div>' +
            '{{/each}}​' +
               '</script>';
            groupmodtmp = $(script).html();
            grouptheModTmp = Handlebars.compile(groupmodtmp);
        };

        /*
            Create New User
        */
        function creatNewUser() {
            var newUser = new VIS.createUser();
            newUser.intialize();
            newUser.show();
        };

        /*
            Create New Role
        */
        function createNewRole() {
            var newUser = new VIS.createrole();
            newUser.intialize();
            newUser.show();
        };

        /*
           Create New Group
        */
        function createGroup() {
            $bsyDiv[0].style.visibility = "visible";
            if (groupWindowID == 0) {
                $.ajax({
                    url: VIS.Application.contextUrl + "Group/GetWindowIds",
                    async: false,
                    success: function (result) {
                        var data = JSON.parse(result);
                        if (data != null && data != undefined) {
                            groupWindowID = data.GroupWindowID;
                            roleWindowID = data.RoleWindowID;
                            userWindowID = data.UserWindowID;
                        }
                    },
                    error: function () {
                        VIS.Msg.getMsg("VIS_GroupWindowNotFound");
                    }
                });
            }

            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction("AD_GroupInfo_ID", VIS.Query.prototype.EQUAL, 0);
            VIS.viewManager.startWindow(groupWindowID, zoomQuery);
            $bsyDiv[0].style.visibility = "hidden";
        };

        /*
            Load Users From Db and Append them to left Panel
        */
        function loadUser() {
            pageNo = 1;
            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetUserInfo",
                type: "GET",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: ({ searchText: $searchUser.val(), sortBy: sortby, pageNo: pageNo, pageSize: PAGESIZE }),
                success: function (result) {
                    var data = JSON.parse(result);
                    $divUserGroup.find('.vis-group-user-wrap').remove();
                    $divUserGroup.append(usertheModTmp(data));
                    $divUserGroup.off("click");
                    $divUserGroup.on("click", userContaierClick);

                    // To Show First User sleected By Default
                    if ($($divUserGroup.children('.vis-group-user-wrap')).length > 0) {
                        $($divUserGroup.children('.vis-group-user-wrap')[0]).trigger('click');
                    }
                    pageNo++;

                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorLoadingContacts");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        /*
           User Scroll
        */
        function userScroll() {
            if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                $.ajax({
                    url: VIS.Application.contextUrl + "Group/GetUserInfo",
                    type: "GET",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    data: ({ searchText: $searchUser.val(), sortBy: sortby, pageNo: pageNo, pageSize: PAGESIZE }),
                    success: function (result) {
                        pageNo++;
                        var data = JSON.parse(result);
                        $divUserGroup.append(usertheModTmp(data));
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                    error: function () {
                        VIS.ADialog.error("VIS_ErrorLoadingContacts");
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });
            }
        };

        /*
             Search Users
        */
        function searchUsers(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            if ($searchUser == undefined || $searchUser == null) {
                return;
            }
            loadUser();
        };

        /*
          Search Groups
        */
        function searchGroups(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            if ($searchGroup == undefined || $searchGroup == null) {
                return;
            }
            loadGroup();
        };

        /*
         Search Roles
       */
        function searchRoles(e) {
            if (e.keyCode != undefined && e.keyCode != 13) {
                return;
            }
            if ($searchRole == undefined || $searchRole == null) {
                return;
            }
            loadRoles();
        };

        /*
            Click Event of icons on User info
        */
        function userContaierClick(e) {

            if (roleChanged.length > 0) {
                return;
            }

            var target = $(e.target);

            $($($divUserGroup.children('.vis-group-selected-op'))[0]).removeClass('vis-group-selected-op vis-group-selected-opbackground');

            if (target.hasClass('vis-group-user-wrap')) {
                target.addClass('vis-group-selected-op vis-group-selected-opbackground');
                userID = target.data("uid");
            }
            else {
                if ($(target.parents('.vis-group-user-wrap')).length > 0) {     // length will be 0 if user's search result single or more record and there is some blank space... on click blank space nothing should be happened.
                    $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                    userID = $(target.parents().find('.vis-group-selected-op')).data("uid");
                }
                else {
                    return;
                }
            }

            //if (target.hasClass('vis-group-selected-op')) {
            //    userID = target.data("uid");
            //}
            //else {
            //    userID = $(target.parents().find('.vis-group-selected-op')).data("uid");
            //}

            if (target.hasClass('vis-group-user-ico vis-group-pointer vis-group-edit')) {
                var UserID = target.data("uid").split('-');
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_User_ID", VIS.Query.prototype.EQUAL, UserID[0]);
                VIS.viewManager.startWindow(UserID[2], zoomQuery);
            }
            else if (target.hasClass('vis-group-user-ico vis-group-pointer vis-group-activeUser') && target.attr("disabled") != "disabled") {
                activeUser(target, false);
            }
            else if (target.hasClass('vis-group-user-ico vis-group-pointer vis-group-inactiveUser') && target.attr("disabled") != "disabled") {
                activeUser(target, true);
            }





            loadRoles(target);

        };

        /*
            Import Gmail Contacts
        */
        function importUser() {
            //if (window.WSP) {
            $bsyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {
                VIS.importcontacts(self);
                $bsyDiv[0].style.visibility = "hidden";
            }, 100);
            //}
            //else {
            //    VIS.ADialog.info('PleaseInstallWSPModule');
            //}
        };

        /*
            Load User Roles
        */
        function loadRoles(target) {
            $bsyDiv[0].style.visibility = "visible";
            var name = $searchRole.val();
            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetRoleInfo",
                data: ({ AD_User_ID: userID, name: name }),
                success: function (result) {
                    var data = JSON.parse(result);
                    $divRoleGroup.find('.vis-group-user-wrap').remove();
                    $divRoleGroup.append(roletheModTmp(data));
                    $divRoleGroup.off("click");
                    $divRoleGroup.on("click", roleContaierClick);

                    // To Show First Role sleected By Default
                    if ($($divRoleGroup.children('.vis-group-user-wrap')).length > 0) {
                        $($($divRoleGroup).children('.vis-group-user-wrap')[0]).trigger('click');
                    }

                    roleAssigned = [];


                    for (var i = 0; i < data.length; i++) {
                        roleAssigned.push({ AD_Role_ID: data[i].AD_Role_ID, IsAssignedToUser: data[i].IsAssignedToUser })
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

        /*
         Load Role Group
     */
        function loadGroup(target) {

            $bsyDiv[0].style.visibility = "visible";
            var name = $searchGroup.val();
            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetGroupInfo",
                data: ({ AD_Role_ID: roleID, name: name }),
                success: function (result) {
                    var data = JSON.parse(result);

                    $divGroupsGroup.find('.vis-group-user-wrap').remove();
                    $divGroupsGroup.append(grouptheModTmp(data));
                    $divGroupsGroup.off("click");
                    $divGroupsGroup.on("click", groupContaierClick);

                    // To Show First Group sleected By Default
                    if ($($divGroupsGroup.children('.vis-group-user-wrap')).length > 0) {
                        $($($divGroupsGroup).children('.vis-group-user-wrap')[0]).trigger('click');
                    }

                    groupAssigned = [];

                    $bsyDiv[0].style.visibility = "hidden";

                    for (var i = 0; i < data.length; i++) {
                        if (i == 0) {
                            groupWindowID = data[i].GroupWindowID;
                        }
                        groupAssigned.push({ AD_Group_ID: data[i].AD_Group_ID, IsAssignedToUser: data[i].IsAssignedToUser })
                    }

                    $($divGroupsGroup.find('input')).off("click");
                    $($divGroupsGroup.find('input')).on("click", groupCheckboxClick);

                },
                error: function () {
                    VIS.ADialog.error("VIS_ErrorLoadingGroups");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        /*
            role Container Click
        */
        function roleContaierClick(e) {
            roleContainerClicked = true;
            if (groupChanged.length > 0) {
                // if (VIS.ADialog.ask('VIS_SaveGroup')) {
                VIS.ADialog.confirm('VIS_SaveGroup', true, "", "Confirm", function (result) {
                    if (result) {
                        saveUserGroups();
                    }
                    else {
                        groupChanged = [];
                    }
                });
                return;
            }

            var target = $(e.target);
            $($($divRoleGroup.children('.vis-group-selected-op'))[0]).removeClass('vis-group-selected-op vis-group-selected-opbackground');

            if (target.hasClass('vis-group-user-wrap')) {
                target.addClass('vis-group-selected-op vis-group-selected-opbackground');
                roleID = target.data("uid");
            }
            else {
                //$(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                //roleID = $(target.parents().find('vis-group-selected-op vis-group-selected-opbackground')).data("uid");
                roleID = $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground').data("uid");
            }

            if (target.hasClass('vis-group-user-ico vis-group-pointer vis-group-edit')) {
                roleID = target.data("uid");
                roleID = roleID.split('-');
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_Role_ID", VIS.Query.prototype.EQUAL, roleID[0]);
                VIS.viewManager.startWindow(roleID[1], zoomQuery);
                roleID = roleID[0];
            }
            if (roleID != undefined) {
                loadGroup(target);
            }
            if (e.originalEvent === undefined) {
                e.stopPropagation();
            }

        };

        /*
           group Container Click
       */
        function groupContaierClick(e) {
            groupContainerClicked = true;
            var target = $(e.target);
            $($($divGroupsGroup.children('.vis-group-selected-op'))[0]).removeClass('vis-group-selected-op');
            if (target.hasClass('vis-group-user-wrap')) {
                target.addClass('vis-group-selected-op');
                groupID = target.data("uid");
            }
            else {
                $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op');
                groupID = $(target.parents().find('.vis-group-selected-op')).data("uid");
            }

            if (target.hasClass('vis-group-user-ico vis-group-edit vis-group-pointer')) {
                groupID = target.data("uid");
                groupID = groupID.split('-');
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_GroupInfo_ID", VIS.Query.prototype.EQUAL, groupID[0]);
                VIS.viewManager.startWindow(groupID[1], zoomQuery);
            }
            else if (target.hasClass('vis-group-user-ico vis-group-info vis-group-pointer')) {
                groupID = target.data("uid");
                groupID = groupID.split('-');
                getGroupInfo(groupID[0]);
            }
            if (e.originalEvent === undefined) {
                e.stopPropagation();
            }
        };

        /*
          group Group Info
        */
        function getGroupInfo(groupid) {
            var groupInfo = new VIS.groupInfo(groupid);
            groupInfo.intialize();
            groupInfo.show();
        };

        /*
            Fire when user click on role checkbox
        */
        function roleCheckboxClick(e) {

            if (groupChanged.length > 0) {

                VIS.ADialog.confirm('VIS_SaveGroup', true, "", "Confirm", function (result) {
                    if (result) {
                        saveUserGroups();
                    }
                    else {
                        groupChanged = [];
                    }
                });
                return;
            }


            var target = $(e.target);

            //this is used to set if role is assigned or assigned to user.
            var element = $.grep(roleAssigned, function (ele, index) {
                return ele.AD_Role_ID == target.data('uid');
            });
            if (element != null && element.length > 0) {
                element[0].IsAssignedToUser = target.prop('checked');
            }

            // this is used to track an array of if any unsaved Role left or not..
            element = $.grep(roleChanged, function (ele, index) {
                return ele == target.data('uid');
            });
            if (element == null || element.length == 0) {
                roleChanged.push(target.data('uid'));
            }
            else {
                roleChanged.splice(roleChanged.indexOf(target.data('uid')), 1);
            }


            //if (target.prop('checked')) {
            //    $(target.siblings('label')).addClass("vis-group-selectedcheckbox");
            //}
            //else {
            //    $(target.siblings('label')).removeClass("vis-group-selectedcheckbox");
            //}
            e.stopPropagation();
        }

        /*
         Fire when user click on group checkbox
        */
        function groupCheckboxClick(e) {



            if (roleChanged.length > 0) {

                VIS.ADialog.confirm('VIS_SaveRole', true, "", "Confirm", function (result) {
                    if (result) {
                        saveUserRoles();
                    }
                    else {
                        roleChanged = [];
                    }

                });



                return;
            }


            var target = $(e.target);

            //this is used to set if Group is assigned or assigned to role.
            var element = $.grep(groupAssigned, function (ele, index) {
                return ele.AD_Group_ID == target.data('uid');
            });
            if (element != null && element.length > 0) {
                element[0].IsAssignedToUser = target.prop('checked');
            }

            // this is used to track an array of if any unsaved Role left or not..
            element = $.grep(groupChanged, function (ele, index) {
                return ele == target.data('uid');
            });
            if (element == null || element.length == 0) {
                groupChanged.push(target.data('uid'));
            }
            else {
                groupChanged.splice(groupChanged.indexOf(target.data('uid')), 1);
            }


            //if (target.prop('checked')) {
            //    $(target.siblings('label')).addClass("vis-group-selectedcheckbox");
            //}
            //else {
            //    $(target.siblings('label')).removeClass("vis-group-selectedcheckbox");
            //}
            e.stopPropagation();
        };

        /*
            Save User's Roles
        */
        function saveUserRoles() {
            roleChanged = [];
            $.ajax({
                url: VIS.Application.contextUrl + "Group/UpdateUserRoles",
                type: "Post",
                data: { AD_User_ID: userID, roles: JSON.stringify(roleAssigned) },
                success: function () {
                    $($($middlePanel.find('h7'))[0]).show();
                    $($($middlePanel.find('h7'))[0]).fadeOut(1600, "linear");
                },
                error: function () {
                    VIS.ADialog.error(VIS.Msg.getMsg('VIS_ErrorSavingRole'));
                }
            });
        };

        /*
        Save User's Group
        */
        function saveUserGroups() {
            groupChanged = [];
            $.ajax({
                url: VIS.Application.contextUrl + "Group/UpdateUserGroups",
                type: "Post",
                data: { AD_Role_ID: roleID, AD_Group_ID: groupID, groups: JSON.stringify(groupAssigned) },
                success: function () {
                    $($($rightPanel.find('h7'))[0]).show();
                    $($($rightPanel.find('h7'))[0]).fadeOut(1600, "linear");
                },
                error: function () {
                    VIS.ADialog.error(VIS.Msg.getMsg('VIS_ErrorSavingGroups'));
                }
            });
        };

        /*
            Set User Active And InActive
        */
        function activeUser(target, activate) {
            $bsyDiv[0].style.visibility = "visible";
            var url = '';
            if (activate) {
                target.removeClass('vis-group-user-ico vis-group-pointer vis-group-inactiveUser');
                target.addClass('vis-group-user-ico vis-group-pointer vis-group-activeUser');
                url = VIS.Application.contextUrl + "Group/ActiveUser";
            }
            else {
                target.removeClass('vis-group-user-ico vis-group-pointer vis-group-activeUser')
                target.addClass('vis-group-user-ico vis-group-pointer vis-group-inactiveUser');
                url = VIS.Application.contextUrl + "Group/InActiveUser";
            }


            $.ajax({
                url: url,
                data: ({ AD_User_ID: target.data('uid') }),
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data < 1) {
                        VIS.ADialog.error("VIS_UserNotUpdated");
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function () {
                    VIS.ADialog.error("VIS_UserNotUpdated");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });

        };

        /*************************************/

        this.disposeComponent = function () {
            if ($btnCreateUser != null && $btnCreateUser != undefined) {
                $btnCreateUser.off("click");
            }
            $searchUserBtn.off("click");
            $searchUser.off("keydown");
            //  $btnImportUser.on("click", importUser);
            $btnSaveRoles.off("click");
            $btnSaveGroup.off("click");
            $searchGroupBtn.off("click");
            $searchGroup.off("keydown");
            $searchRoleBtn.off("click");
            $searchRole.off("keydown");
            $btnCreateGroup.off("click");
            $btnCreateRole.off("click");
            $divUserGroup.off("scroll");
            $root.off("click");





            this.frame = null;
            this.windowNo = null;
            $leftPanel.remove();
            $leftPanel = null;
            $middlePanel.remove();
            $middlePanel = null;
            $rightPanel.remove();
            $rightPanel = null;
            $divUserGroup.remove();
            $divUserGroup = null;
            $searchUser.remove();
            $searchUser = null;
            $searchUserBtn.remove();
            $searchUserBtn = null;
            $btnCreateUser.remove();
            $btnCreateUser = null;
            $btnSortUsers.remove();
            $btnSortUsers = null;
            // $btnImportUser.remove();
            //$btnImportUser = null;
            $btnCreateRole.remove();
            $btnCreateRole = null;
            $btnSaveRoles.remove();
            $btnSaveRoles = null;
            $divRoleGroup.remove();
            $divRoleGroup = null;
            $bsyDiv.remove();
            $bsyDiv = null;
            $searchGroup.remove();
            $searchGroup = null;
            $searchGroupBtn.remove();
            $searchGroupBtn = null;
            $searchRole.remove();
            $searchRole = null;
            $searchRoleBtn.remove();
            $searchRoleBtn = null;
            $btnCreateGroup.remove();
            $btnCreateGroup = null;
            $btnSaveGroup.remove();
            $btnSaveGroup = null;
            $divGroupsGroup.remove();
            $divGroupsGroup = null;
            usermodtmp.remove();
            usertheModTmp.remove();
            rolemodtmp.remove();
            roletheModTmp.remove();
            groupmodtmp.remove();
            grouptheModTmp.remove();

            roleAssigned = null;
            groupAssigned = null;
            roleChanged = null;
            groupChanged = null;
            $root.remove();
            $root = null;
            $menu = null;
            self = null;
        };

        /*************************************/

        this.getRoot = function () {
            return $root;
        };

        /*************************************/



    };


    GroupForm.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        frame.setTitle("VIS_Group");
        this.frame.getContentGrid().append(this.getRoot());
        this.initializeComponent();

    };

    GroupForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.GroupForm = GroupForm;

})(VIS, jQuery);