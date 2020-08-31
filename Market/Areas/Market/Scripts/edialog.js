; var Market = window.Market || {};

Market.KeyStatus = {};
Market.KeyHeader = {};

Market.KeyStatus.NotRegisterd = "N";
Market.KeyStatus.Renew = "R";
Market.KeyStatus.Upgrade = "U";

Market.KeyHeader.Expired = "E";
Market.KeyHeader.MExpired = "M";
Market.KeyHeader.Community = "C";
Market.KeyHeader.Professional = "P";

; (function (VIS, $) {
    

    function KeyDialog(header, isCloudMarket) {

        /*Public Variable*/
        this.tokenKey, this.tokenKeyStatus, this.isCloudMarket, this.isValid, this.checkEntKey = false;
        this.isProfessinalEdition = false;

        this.onClose = null;

        var ch = null, btnText = "", lblTxt, headerTxt, dw, hdr;
        var busy, btnValidate, btnGetKey, txtKey;
        var self = this;
        var isLoginDialog;

        var root = null;

        function init() {

            if (Market.KeyHeader.Professional == header) {
                headerTxt = VIS.Msg.getMsg("ProfessionalHeader");
                btnText = VIS.Msg.getMsg("GetProfessionalKey")
                lblTxt = VIS.Msg.getMsg("EnterProfessionalKey");
            }
            else if (Market.KeyHeader.Expired == header) {
                headerTxt = VIS.Msg.getMsg("KeyExpiredHeader");
                btnText = VIS.Msg.getMsg("RenewKey")
                lblTxt = VIS.Msg.getMsg("EnterProfessionalKey");
            }
            else if (Market.KeyHeader.MExpired == header) {
                headerTxt = VIS.Msg.getMsg("MarketExpiredHeader");
                btnText = VIS.Msg.getMsg("RenewKey")
                lblTxt = VIS.Msg.getMsg("EnterProfessionalKey");
            }
            else {
                headerTxt = VIS.Msg.getMsg("CommunityHeader");
                btnText = VIS.Msg.getMsg("GetCommunityKey")
                lblTxt = VIS.Msg.getMsg("EnterCommunityKey");
            }


            if (isCloudMarket) {
                lblTxt = VIS.Msg.getMsg("CloudProfessionalText");

            }


            var str = '<div style="max-width:500px;"> ' +
                                    '<div  style="position:relative" style="width:100%;float:left;">' +
                                       '<div class="vis-pull-left">' +
                                            '<p class="market-kd-title">' + headerTxt + '</p>' +
                                       '</div>' +
                                       '<div class="vis-pull-right">' +
                                         '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/logo.jpg">' +
                                       '</div>' +
                                    '</div>' +

                                    '<div class="market-app-description" style="margin-top: 0;float:left;padding-top:30px;padding-bottom:0px;">' +
                                         '<div class="comm-data">' +
                                              '<div class="col50 vis-pull-left" style="width:40%">' +
                                                      '<a class="market-community-key">' + btnText + '</a>' +
                                              '</div>' +
                                              '<div style="width: 20px; margin-right: 5%;margin-left:5%" class="vis-pull-left" >' +
                                                  '<div class="market-divide"></div>' +
                                                      '<span class="market-span-or">' + VIS.Msg.getMsg('or') + '</span>' +
                                                      '<div class="market-divide"></div>' +
                                               '</div>' +
                                               '<div class="col50 vis-pull-right" style="width:40%">' +
                                                    '<label class="market-enter-key" ' + (isCloudMarket ? 'style="min-width:200px"' : ' ') + '>' + lblTxt + '</label>' +
                                                     '<input type="text" class="market-input-key" style="width:100%;' + (isCloudMarket ? "display:none" : "") + '">' +
                                                     '<a class="market-validate" ' + (isCloudMarket ? 'style="display:none"' : ' ') + '>' + VIS.Msg.getMsg("Validate") + '</a>' +
                                                '</div>' +
                                           '</div>' + //<!-- end of comm-data -->
                                     '</div>' +//<!-- end of popup-description -->
                                     '<div class="vis-apanel-busy" style="height:90%;width:90%;" ></div>' +
                      '</div>';

            root = $(str);

            busy = root.find(".vis-apanel-busy").hide();
            btnValidate = root.find(".market-validate");
            btnGetKey = root.find(".market-community-key");
            txtKey = root.find(".market-input-key");
            hdr = root.find(".market-kd-title");
            bindEvents();

        };

        init();

        function bindEvents() {
            btnValidate.on("click", function () {
                var val = txtKey.val();
                if (!val || val.length < 1) {
                    var msg = VIS.Msg.getMsg("EnterKey");
                    VIS.ADialog.error("Error?", true, msg);
                    return;
                }
                setIsBusy(true);

                var info = {};
                info.Url = VIS.Application.contextUrl;
                info.Client_ID = VIS.Env.getCtx().getAD_Client_ID();
                info.UserName = VIS.Env.getCtx().getAD_User_Name();
                info.RoleName = VIS.Env.getCtx().getAD_Role_Name();
                info.ClientName = VIS.Env.getCtx().getAD_Client_Name();
                info.Token = val;
                info.IsDemoCheck = false;
                info.IsEntCheck = self.checkEntKey;

                $.ajax({
                    url: VIS.Application.contextUrl + 'Market/Module/InitLoginOrValidate',
                    type: "POST",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    data: JSON.stringify(info)
                }).done(function (json) {
                    var ed = JSON.parse(json);

                    self.isValid = ed.IsValid;
                    self.isProfessinalEdition = ed.IsAllowWork;
                    self.tokenKey = ed.Token;
                    self.isMarketValid = !ed.IsMarketExpired || !ed.IsAllowWork;;

                    //  Market.isKeyRegistered = ed.IsRegistered;
                    //  Market.isProfessionalKey = ed.IsAllowWork;
                    Market.keyMgr.init(ed.IsRegistered, ed.IsAllowWork, ed.IsExpired);
                    // End 
                    if (ed.IsValid) {

                        setIsBusy(false);
                        if (dw) {
                            self.dispose();
                            dw.remove();
                        }
                        else
                            if (ed.IsMarketExpired && !isLoginDialog && ed.IsAllowWork) {
                                setIsBusy(false);
                                VIS.ADialog.error("Error?", true, VIS.Msg.getMsg("Market_MarketExpired"));
                                return;
                            }
                        ch.close();
                    }
                    else {
                        setIsBusy(false);
                        VIS.ADialog.error("Error?", true, VIS.Msg.getMsg(ed.Message));
                    }

                }).fail(function (xhe, e) {
                    VIS.ADialog.error("Error?", true, e);
                });
            });


            btnGetKey.on("click", function () {

                var url = "https://login.viennaadvantage.com/buy.aspx?key";
                if (self.tokenKeyStatus == Market.KeyStatus.Upgrade) {
                    url += "key=" + self.tokenKey + "&request=U";

                }
                else if (self.tokenKeyStatus == Market.KeyStatus.Renew) {
                    url += "key=" + self.tokenKey + "&request=R";
                }
                else {
                    url = "http://register.viennaadvantage.com";
                }

                VIS.Env.startBrowser(url);
            });
        };

        function setIsBusy(isBusy) {
            if (isBusy)
                busy.show();
            else busy.hide();
        };


        this.show = function (hide) {
            isLoginDialog = hide;
            ch = new VIS.ChildDialog();
            //ch.setHeight(800);
            //ch.setWidth(1600);
            ch.setTitle("");

            if (hide && ch.removeCloseBtn)
                ch.removeCloseBtn(hide);
            ch.setModal(true);
            ch.show();
            ch.setContent(root);

            if (!ch.removeCloseBtn && hide) {
                ch.getRoot().dialog({ closeOnEscape: false });
                $('.ui-dialog-titlebar-close').remove();
            }


            ch.hideButtons();
            ch.onClose = function () {
                if (self.onClose)
                    self.onClose();
                self.dispose();
            };
            ch.getRoot().dialog({ position: { at: "center center", of: window } });
        };

        this.setHeader = function (msg) {
            hdr.text(VIS.Msg.getMsg(msg));
        };

        this.closeDialog = function () {
            if (ch)
                ch.close();
        };

        this.showModel = function () {

            // ch.setContent(root);
            //dw = $('<div style="height:100%;width:100%;background-color:light-grey;opacity:.5"></div>');
            dw = $('<div class="vis-wakeup-main"></div>'
                   + '<div class="vis-wakeup" style="height: 278px; width: 520px;"></div>'
                   + '<div class="vis-wakeup-content" style="height: auto; width: auto;" >'
                   + '</div></div></div>');
            $('body').append(dw).find('.vis-wakeup-content').append(root);;
        };

        this.dispose = function () {
            self = null;
            ch = null;
            this.show = null;
            root.remove();
            ch = btnText = lblTxt = headerTxt = root = null;
            busy = btnValidate = btnGetKey = null;
        };

        this.getRoot = function () {
            return root;
        };
    };

    KeyDialog.prototype.getIsValid = function () {
        return this.isValid;
    };

    KeyDialog.prototype.getIsMarketValid = function () {
        return this.isMarketValid;
    };

    Market.KeyDialog = KeyDialog;

})(VIS, jQuery);