; (function (Market, $) {

    function DependencyDialog(modDependencyList, name, parent) {

        var ch, contentDiv, root, busy;
        var self = this;

        var maxLabel = Math.max.apply(Math, modDependencyList.map(function (o) { return o.Lable; }))

        function init() {

            root = $('<div class="market-dp-da" >' +
                            '<div class="market-app-detail"> ' +
                             '<div class="market-about-app vis-pull-left" style="width:100%">' +
                                '<img src="' + VIS.Application.contextUrl + 'Areas/Market/Images/big_app_image.png">' +
                                '<div class="market-app-detail-inner">' +
                                '<p class="market-title">' + name + '</p>' +
                              '</div>' +
                              '</div>' +
                                ' <div class="market-app-rating">' +

                                '</div>' +
                              '</div>' +
                       '</div>');


            var contentDiv = '<div class="market-app-description">' +
                            '<table style="width:400px;">' +
                            '<tr style="margin-bottom:5px"><th style="width:50%">' + VIS.Msg.getMsg("Name") + '</th><th style="width:30%">' + VIS.Msg.getMsg("VERSIONNO") +
                            '</th><th style="width:20%">' + VIS.Msg.getMsg("Action") + '</th></tr>';

            for (var i = 0; i < modDependencyList.length; i++) {
                contentDiv += '<tr>';
                contentDiv += '<td><p class="market-author" style="padding-right:5px"><small>' + modDependencyList[i].Name + '</small></p></td><td ><p class="market-author"><small>'
                              + modDependencyList[i].VersionNo +
                              '</small></p></td><td ><p class="market-install-detail"><a href="javascript:void(0);" class="market-button-Install"';
                if (maxLabel != modDependencyList[i].Lable)
                    contentDiv += ' style="background-color:lightgray" disable';
                else
                    contentDiv += ' data-val=' + i;

                contentDiv += '>' + VIS.Msg.getMsg('Install') + '</a></p></td></tr>';
            }
            contentDiv += '</table></div>';

            busy = $('<div class="vis-apanel-busy" style="height:90%;width:80%;display:none">');
            root.append($(contentDiv));
            root.append(busy);
            bindEvents();
        };

        init();

        var fetching = false;
        function bindEvents() {
            root.find('.market-button-Install').on("click", function (e) {
                if (fetching)
                    return;
                fetching = true;
                var mod = modDependencyList[$(this).data("val")];

                if (mod) {

                    var data = { "AD_ModuleInfo_ID": mod.ModuleID, "tokenKey": parent.prop.tokenKey };
                    busy.show();
                    $.ajax({
                        url: VIS.Application.contextUrl + "Market/Module/GetModuleInfo",
                        type: "POST",
                        datatype: "json",
                        contentType: "application/json; charset=utf-8",
                        async: true,
                        data: JSON.stringify(data)
                    }).done(function (json) {
                        if (json && json.result && json.result != "null") {
                            mod = JSON.parse(json.result);
                            parent.installModule(mod, true);
                            ch.dispose();
                            fetching = false;
                        }
                    })
            .fail(function (xhr, e) {
                busy.hide();
                fetching = false;
            });



                }
            });
        };

        this.getRoot = function () {
            return root;
        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            //ch.setHeight(800);
            //ch.setWidth(1600);
            ch.setTitle(VIS.Msg.getMsg("Market_DependencyFound"));
            ch.setModal(true);
            ch.show();
            ch.setContent(root);
            ch.hideButtons();
            ch.onClose = function () {
                self.dispose();
            }
            ch.getRoot().dialog({ position: { at: "center center", of: window } });
        };


        this.disposeComponent = function () {
            root.remove();
            parnet = null;
            this.getRoot = null;
            this.disposeComponent = null;
            ch = null;
            self = null;
        };



    };




    DependencyDialog.prototype.dispose = function () {
        this.disposeComponent();
    };

    Market.DependencyDialog = DependencyDialog;



    Market.PrivateKey = '';


    function PrivateKeyDialog(parent, sText) {

        var ch, contentDiv, root;
        var self = this;
        var tmpKey = 0;


        function init() {




            root = $('<div class="market-app-description" style="margin-top: 0;float:left;padding-top:30px;padding-bottom:0px;">' +
            	                        '<div class="comm-data">' +
                	                          '<div class="col50 vis-pull-right" style="width:100%">' +
                    	                           '<label class="market-enter-key">' + VIS.Msg.getMsg("EnterKey") + '</label>' +
                                                    '<input type="text" class="market-input-key" style="width:100%" value=' + Market.PrivateKey + '>' +
                                                    '<a class="market-validate">' + VIS.Msg.getMsg("OK") + '</a>' +
                                               '</div>' +
                                          '</div>' + //<!-- end of comm-data -->
                                    '</div>');





            // root.append($(contentDiv));
            bindEvents();
        };

        init();


        function bindEvents() {
            root.find('.market-validate').on("click", function (e) {
                var mod = $(".market-input-key").val();
                if (mod && mod.length > 0) {
                    Market.PrivateKey = mod;
                    parent.getPrivateModule(sText, true);
                    ch.close();
                    return;
                }
                alert("enterKey");
            });
        };

        this.getRoot = function () {
            return root;
        };

        this.show = function () {

            ch = new VIS.ChildDialog();
            //ch.setHeight(800);
            //ch.setWidth(1600);
            ch.setTitle("Market_PrivateKey");
            ch.setModal(true);
            ch.show();
            ch.setContent(root);
            ch.hideButtons();
            ch.onClose = function () {
                self.disposeComponent();
            }
            ch.getRoot().dialog({ position: { at: "center center", of: window } });
        };


        this.disposeComponent = function () {
            root.remove();
            parnet = null;
            this.getRoot = null;
            this.disposeComponent = null;
            ch = null;
            self = null;
        };

    };


    Market.PrivateKeyDialog = PrivateKeyDialog;


})(Market, jQuery);