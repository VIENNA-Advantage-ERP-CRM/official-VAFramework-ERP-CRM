; (function (VIS, $) {

    function orgAccess(setSelectedOrg) {
        var $root = null;
        var groupmodtmp, grouptheModTmp = null;
        var ch = null;
        var orgAssigned = [];
        var self = this;


        this.intialize = function () {
            createTemplate();
            createLayout();
        };

        function createLayout() {
            $root = $('<div>');
            $.ajax({
                url: VIS.Application.contextUrl + "Group/GetOrgAccess",
                success: function (result) {
                    var data = JSON.parse(result);
                    $root.append(grouptheModTmp(data));
                    $($root.find('input')).on("click", CheckboxClick);

                },
                error: function () {
                    VIS.ADialog.error('VIS_ErrorLoadingOrg');
                }
            });
        };

        function createTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
              '{{#each this}}' +
           '<div style="border-bottom:1px solid lightgray" class="vis-group-assinRole-check">' +
            '<div style="width:100%" class="vis-group-user-profile">' +
                           '<input type="Checkbox" data-uid="{{OrgID}}"> ' +
                            '<p style="margin-bottom:2px">{{OrgName}}</p>' +
                            '<span style="margin-left:30px;float:left">{{Description}}</span>' +
                        '</div>' +
                        '</div>' +
                         '{{/each}}​' +
               '</script>';
            groupmodtmp = $(script).html();
            grouptheModTmp = Handlebars.compile(groupmodtmp);
        };


        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setContent($root);

            ch.setWidth(450);
            ch.setHeight(350);
            ch.setTitle(VIS.Msg.getMsg("VIS_OrgAccess"));
            ch.setModal(true);
            ch.onClose = function () {

                self.dispose();
            };
            ch.show();
            events();

        };

        this.dispose = function () {
            this.prop = null;
            $div = null;
            $root.remove();
            $root = null;
            groupmodtmp = null;
            grouptheModTmp = null;
            ch = null;
            self = null;
            orgAssigned = null;

        };

        function events() {


            ch.onOkClick = function (e) {
                setSelectedOrg(orgAssigned);
                self.dispose();
            };
            ch.onCancelClick = function () {
                self.dispose();
                // ch.close();
                //if (self.onClose) self.onClose();
                //  self.dispose();
            };

        };


        /*
          Fire when user click on role checkbox
      */
        function CheckboxClick(e) {
            var target = $(e.target);
            if (target.prop('checked')) {
                if ($.inArray(target.data('uid')) == -1) {
                    orgAssigned.push(target.data('uid'));
                }
            }
            else {
                orgAssigned.splice(orgAssigned.indexOf(target.data('uid')), 1);
            }



        }

    };

    VIS.orgAccess = orgAccess;


})(VIS, jQuery);