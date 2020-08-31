/* global object */
; var Market = window.Market || {};


; (function (VIS, $) {
    Market.ImpModule = {}; // form's namespace

    /* property Model */
    function MarketProp() {

        this.isFeatured = false,
        this.isPaid = false,
        this.isFree = false,
        this.isMy = false,
        this.isPlanned = false,
         this.isPrivate = false,
        /* call from instance directly */
        this.isRegisterd = false,
        this.isKeyExpired = false,
        this.isMarketExpired = false,
        this.moduleEdition = '',
        this.tokenKey = '',
        this.isProfessinalEdition = false
    };
    MarketProp.prototype.setIsFree = function () {
        this.setAllFalse();
        this.isFree = true;
    };
    MarketProp.prototype.getIsFree = function () {
        return this.isFree;
    };
    MarketProp.prototype.setIsPlanned = function () {
        this.setAllFalse();
        this.isPlanned = true;
    };
    MarketProp.prototype.getIsPlanned = function () {
        return this.isPlanned;
    };
    MarketProp.prototype.setIsMy = function () {
        this.setAllFalse();
        this.isMy = true;
    };
    MarketProp.prototype.getIsMy = function () {
        return this.isMy;
    };
    MarketProp.prototype.setIsFeatured = function () {
        this.setAllFalse();
        this.isFeatured = true;

    };
    MarketProp.prototype.getIsFeatured = function () {
        return this.isFeatured;
    };
    MarketProp.prototype.setIsPaid = function () {
        this.setAllFalse();
        this.isPaid = true;
    };
    MarketProp.prototype.getIsPaid = function () {
        return this.isPaid;
    };
    MarketProp.prototype.setIsPaid = function () {
        this.setAllFalse();
        this.isPaid = true;
    };
    MarketProp.prototype.getIsPaid = function () {
        return this.isPaid;
    };

    MarketProp.prototype.setIsPrivate = function () {
        this.setAllFalse();
        this.isPrivate = true;
    };

    MarketProp.prototype.getIsPrivate = function () {
        return this.isPrivate;
    };

    MarketProp.prototype.setAllFalse = function () {
        this.isPlanned = false;
        this.isMy = false;
        this.isFeatured = false;
        this.isFree = false;
        this.isPaid = false;
        this.isPrivate = false;
    };

    /* Model calss for Market */

    function MarketModel(_host) {

        var isCloudMarket = false;
        var hostUrl = "";
        var host = _host;

        this.getModules = function (sqlWhere, pageIndex, pageSize, modType,
          splKey, showVendorPrivate, targetFunction /* CR1*/) {

            var data = {
                SqlString: sqlWhere, PageIndex: pageIndex, PageSize: pageSize, ModType: modType,
                IsCloudMarket: isCloudMarket, HostURL: hostUrl, TokenKey: splKey, IsKeyExpired: true /* Use as Html5 flag */,
                IsProfessionalEdition: showVendorPrivate /* Use as VendorPrivateKey flag */,
            };
            var result = null;
            $.ajax({
                url: VIS.Application.contextUrl + 'Market/Module/GetMarketModules',
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                if (host.getMarketModulesCompleted)
                    host.getMarketModulesCompleted(result);
            })
            return result;
        };

        function setCloudMarketFlag() {

            var url = VIS.Application.contextFullUrl;
            if (!url)
                url = VIS.Application.contextUrl;
            if (url) {
                if (url.toLower().indexOf("softwareonthecloud.com") > -1) {
                    isCloudMarket = true;
                    hostUrl = url;
                    return true;
                }
            }
            return false;
        }

        this.getIsSystemAdmin = function () {
            var AD_User_ID = VIS.Env.getCtx().getAD_User_ID();
            var AD_Role_ID = VIS.Env.getCtx().getAD_Role_ID();

            if ((AD_User_ID == 0 || AD_User_ID == 100) && AD_Role_ID == 0) {
                return true;
            }
            return false;
        }

        this.getIsCloudMarket = function () {
            return isCloudMarket;
        }


        setCloudMarketFlag();






        this.dispose = function () {
            host = null;
            this.getModules = null;
            this.dispose = null;
            this.getIsSystemAdmin = null;
            this.getIsCloudMarket = null;
        }

    };




    /* Market UI form 
      - show all available module
      - installed , paid or free module
      - show module detail 
      */

    function Market_() {
        /* variables */
        var toolBar, btnClose, moduleList, leftBar, detailView, txtSearch, btnSearch, btnAction;
        var modtmp, modtmpScript, theModTmp, theModTmpRight, chkVendor;
        var self = this;
        this.currentIndex = 0, this.Page_Size = 10, this.searchText = "", this.isBusy = false, this.totalCount = 0;
        this.selectedItem = null;

        /* root div of form */
        var root = $("<div>");
        var busy = $("<div class='vis-apanel-busy'>");
        var root_market = $("<div class='market-body'>").append(busy);
        var header, tab, cntnr;


        root.append(root_market);

        /* Init Model class */
        this.model = new MarketModel(this);
        this.prop = new MarketProp();
        this.mList = [];



        /* Call Controller*/
        root_market.load(VIS.Application.contextUrl + "Market/Module/Index", function () {
            init();
        });

        /* functions  private or privildged */


        /*find and set elements value  */
        function init() {
            toolBar = root.find(".market-app-menu-a");
            btnClose = root.find(".market-close-img");
            moduleList = root.find(".market-main-app-list");
            detailView = root.find(".market-right-content");
            busy = root.find(".vis-apanel-busy");
            txtSearch = root.find(".market-search-input");
            btnSearch = root.find(".market-sear");
            leftBar = root.find(".market-left-sidebar");


            /*left */
            modtmp = root.find(".market-module-template");
            modtmpScript = modtmp.html();
            theModTmp = Handlebars.compile(modtmpScript);
            /*right*/
            modtmp = root.find(".market-module-tmp-right");
            modtmpScript = modtmp.html();
            theModTmpRight = Handlebars.compile(modtmpScript);

            chkVendor = root.find(".market-cb");

            header = root.find(".market-header");
            tab = root.find(".market-app-menu");
            cntnr = root.find(".market-main-container");



            bindEvents();
            registerHelper();

            self.prop.setIsFeatured(true);
            self.getModules();

        };

        function setReviewWidth() {
            //var count = root.find('.market-reviews-right-inner .market-rw-block').length;
            //var width = 220 * count + 10;
            //root.find('.market-reviews-right-inner').css('width', width);
        };

        /* */
        function registerHelper() {
            Handlebars.registerHelper('Rate', function (rating) {
                if (!rating)
                    rating = 1;
                var r = rating / 2;
                var rs = Math.floor(r).toString();
                if (r.toString().indexOf(".") > 0) {
                    rs += "+";
                }

                return rs;
            });
            Handlebars.registerHelper('market_star', function (rating) {
                if (!rating)
                    rating = 1;
                var r = rating / 2;
                var ret = "";

                if (r < 1) {
                    ret = 'half-star';
                }
                else if (r >= 1 && r < 2) {
                    if (r == 1)
                        ret = 'one-star';
                    else
                        ret = 'one-half-stars';
                }
                else if (r >= 2 && r < 3) {
                    if (r == 2)
                        ret = 'two-stars';
                    else
                        ret = 'two-half-stars';
                }
                else if (r >= 3 && r < 4) {
                    if (r == 3)
                        ret = 'three-stars';
                    else
                        ret = 'three-half-stars';
                }
                else if (r >= 4 && r < 5) {
                    if (r == 4)
                        ret = 'four-stars';
                    else
                        ret = 'four-half-stars';
                }
                else if (r >= 5) {
                    ret = 'full-stars'
                }

                return ret;
            });
        };

        /* bind events */
        function bindEvents() {
            toolBar.on("click", function () {
                var a = $(this);
                a.addClass('market-a-active');
                a.parent().siblings().children().removeClass('market-a-active');

                var action = a.data("action");

                if (action == "All") self.prop.setIsFeatured(true);
                else if (action == "Paid") self.prop.setIsPaid(true);
                else if (action == "Free") self.prop.setIsFree(true);
                else if (action == "Planned") self.prop.setIsPlanned(true);
                else if (action == "My") self.prop.setIsMy(true);
                else if (action == "Private") self.prop.setIsPrivate(true);

                self.setSearchText("");
                self.currentIndex = 0;
                self.getModules();
                self.bindData();
            });
            btnClose.on("click", function () {
                self.dispose();
            });

            moduleList.on("click", function (e) {
                //console.log(e);
                var t = $(e.target);
                var li = t;

                while (li[0].nodeName != "LI") {
                    li = $(li.parent());
                }

                var index = li.index();

                moduleList.find("LI").removeClass("market-selected-module");
                li.addClass("market-selected-module");

                //if (t[0].nodeName == "A") {
                //    //ButtonClick
                //    console.log("click =>" + index);
                //    return;
                //}

                var data = self.mList[index];

                self.bindData(data);
                setReviewWidth();
            });

            if (!VIS.Application.isMobile) {
                txtSearch.on("keyup", function (e) {
                    if (!(e.keyCode === 13)) {
                        return;
                    }
                    self.performSearch(txtSearch.val());
                    txtSearch.focus();
                });
            }

            btnSearch.on("click", function () {
                self.performSearch(txtSearch.val());
                txtSearch.focus();
            });

            leftBar.on("scroll", function () {
                if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                    self.currentIndex += 1;
                    self.getModules();
                    self.isTabScroll = true;
                };
            });

            detailView.on("click", function (e) {
                var t = $(e.target);
                //alert(t);
                if (t.hasClass("market-button-Install-detail")) {
                    self.installModule(self.selectedItem);
                }

                if (t.hasClass("market-video-pic")) {
                    var id = t.data("id");
                    if (id) {
                        //VIS.Env.startBrowser("http://www.youtube.com/v/" + id + "?autoplay=1");
                        VIS.Env.startBrowser("https://www.youtube.com/watch?v=" + id);
                    }
                }
                if (t.hasClass("market-button-version-detail")) {
                   // alert("showdetail");
                    self.getVersionHistory(self.selectedItem.AD_Module_ID,0);
                }
            });
        };
        /* remove events(clean up) */
        function unBindEvents() {
            toolBar.off("click");
            btnClose.off("click");
        };

        this.setSize = function (height, width) {
            if (cntnr)
                cntnr.height(height - (tab.height() + header.height()));
        };


        this.bindList = function (theData, isAppend) {
            if (!isAppend) {
                moduleList.empty();
                this.bindData(this.mList.length > 0 ? this.mList[0] : null);
            }
            moduleList.append(theModTmp(theData));
            if (!isAppend)
                moduleList.find("LI").first().addClass("market-selected-module");
        };

        this.bindData = function (theData) {
            this.selectedItem = theData;
            detailView.empty();
            if (theData) {
                detailView.append(theModTmpRight(theData));

            }
        };

        this.showVendorPrivateMod = function () {
            return chkVendor.prop("checked");
        }

        this.getRoot = function () {
            return root;
        };

        this.setBusyPanel = function (isBusy) {
            busy.hide()
            if (isBusy)
                busy.show();
        };

        this.disposeList = function () {
            //this.mList = null;

            //todo:
        };
        this.setSearchText = function (text) {
            if (!text)
                text = "";
            this.searchText = text;
            txtSearch.val(text);
        };

        /* dispose local variable or controls */
        this.disposeComponent = function () {
            self = null;
            unBindEvents();
            toolBar = null;
            btnClose = null;
            root.remove();
            root = null;
            this.mList.length = 0;
            this.mList = null;
            this.selectedItem = null;
            busy = null;
        };
    };

    /* functions */
    Market_.prototype.getModules = function () {

        var appendText = false;
        if (this.searchText && this.searchText.length > 0) {
            appendText = true;
        }
        if (this.prop.getIsFeatured()) {
            this.getFeaturedModule(appendText);
        }
        else if (this.prop.getIsPaid()) {
            this.getPaidModule(appendText);
        }
        else if (this.prop.getIsFree()) {
            this.getFreeModule(appendText);
        }
        else if (this.prop.getIsMy()) {
            this.getMyModule(appendText);
        }
        else if (this.prop.getIsPlanned()) {
            this.getPlannedModule(appendText);
        }
        else if (this.prop.getIsPrivate()) {
            this.getPrivateModule(appendText);
        }
    };

    Market_.prototype.getFeaturedModule = function (appendSearchText) {
        this.setIsBusy(true);
        this.disposeList();
        var sqlWhere = "";
        if (appendSearchText) {
            sqlWhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }
        this.model.getModules(sqlWhere, (this.currentIndex * this.Page_Size), this.Page_Size, "FTR");
    };

    Market_.prototype.getFreeModule = function (appendSearchText) {
        this.setIsBusy(true);
        this.disposeList();

        var sqlWhere = "";
        if (appendSearchText) {
            sqlWhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }
        this.model.getModules(sqlWhere, (this.currentIndex * this.Page_Size), this.Page_Size, "FREE");
    };

    Market_.prototype.getPaidModule = function (appendSearchText) {
        this.setIsBusy(true);
        this.disposeList();
        var sqlWhere = "";
        if (appendSearchText) {
            sqlWhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }
        this.model.getModules(sqlWhere, (this.currentIndex * this.Page_Size), this.Page_Size, "PAID");
    };

    Market_.prototype.getMyModule = function (appendSearchText) {
        this.setIsBusy(true);
        this.disposeList();
        var sqlWhere = "";
        if (appendSearchText) {
            sqlWhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }
        //this.model.getModules(sqlWhere, (this.currentIndex * this.Page_Size), this.Page_Size, "MY");
        this.model.getModules(sqlWhere, (this.currentIndex * 50), 50, "MY");
    };

    Market_.prototype.getPlannedModule = function (appendSearchText) {
        this.setIsBusy(true);
        this.disposeList();
        var sqlwhere = "";
        if (appendSearchText) {
            sqlwhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }
        this.model.getModules(sqlwhere, (this.currentIndex * this.Page_Size), this.Page_Size, "PLAN");
    };

    Market_.prototype.getPrivateModule = function (appendSearchText, skipDialog) {
        this.setIsBusy(true);
        this.disposeList();
        var sqlwhere = "";
        if (appendSearchText) {
            sqlwhere += " AND UPPER(m.Name) LIKE '%" + this.searchText.toUpper() + "%' ";
        }
        else {
            this.setSearchText("");
        }



        var showVendorKeyDialog = this.showVendorPrivateMod();

        if (showVendorKeyDialog && (!Market.PrivateKey || Market.PrivateKey.length < 10)) {
            this.bindList(null);
            this.bindData(null);
            var m = new Market.PrivateKeyDialog(this, appendSearchText);
            m.show();
            m = null;
            this.setIsBusy(false);
            return;
        }

        if (!showVendorKeyDialog && !this.getIsTokenKeyValidated()) {
            this.bindList(null);
            this.bindData(null);
            this.setIsBusy(false);
            return;
        }

        this.model.getModules(sqlwhere, (this.currentIndex * 50), 50, "SPL", Market.PrivateKey, showVendorKeyDialog);
    };


    Market_.prototype.getIsPaidEdition = function (mEditionPayment) {
        if (!mEditionPayment || mEditionPayment.length < 1) {
            return false;
        }

        else {
            if (mEditionPayment.toUpper() == "PP" || mEditionPayment.toUpper() == "CP") {
                return true;
            }
        }
        return false;
    };

    Market_.prototype.setIsBusy = function (isBusy) {
        this.isBusy = isBusy;
        this.setBusyPanel(isBusy);
    };

    Market_.prototype.performSearch = function (txt) {
        this.searchText = txt;
        this.currentIndex = 0;
        this.getModules();

    };

    Market_.prototype.getMarketModulesCompleted = function (e) {
        if ((e.error && e.error.length > 0 && e.error != "null")) {
            //VAdvantage.Logging.VLogger.Get().Severe(e.customError.Message);
            alert(JSON.parse(e.error).Message);
            //console.log(e.error);
            //MList = null;
            this.setIsBusy(false);
            return;
        }

        try {

            var mc = JSON.parse(e.result);
            var p = this.prop; //propeties
            p.isRegisterd = mc.IsRegistered;
            p.isProfessinalEdition = mc.IsProfessionalEdition;
            p.isKeyExpired = mc.IsKeyExpired;
            p.isMarketExpired = mc.IsMarketExpired;
            p.tokenKey = mc.TokenKey;
            //MList = new List<ModuleInfo>();
            var mInfo = null;
            var _depInfo = "";
            for (var i = 0; i < mc.lstModuleInfo.length; i++) {
                mInfo = mc.lstModuleInfo[i];
                //_depInfo = "";
                ////Lakhwinder
                //if (!(mInfo.ModuleDependency == null || mInfo.ModuleDependency.length == 0)) {
                //    var dinfo = [];
                //    var info = null;
                //    for (var j = 0; j < mInfo.ModuleDependency.length; j++) {
                //        info = {};
                //        info.Name = mInfo.ModuleDependency[j].Name;
                //        info.ModuleID = mInfo.ModuleDependency[j].ModuleID;
                //        info.Prefix = mInfo.ModuleDependency[j].Prefix;
                //        info.VersionNo = mInfo.ModuleDependency[j].VersionNo;
                //        info.RootModuleName = mInfo.ModuleDependency[j].RootModuleName;
                //        info.Lable = mInfo.ModuleDependency[j].Lable;
                //        dinfo.push(info);
                //        if (info.Lable == 1) {
                //            _depInfo += (info.Name + " " + info.VersionNo + "\t\n");
                //        }
                //    }
                //    mInfo.DependencyInfo = dinfo;
                //}
                //if (_depInfo.length > 0) {
                //    if (mInfo.ColNameValues.DEPENDENCYINFO || mInfo.ColNameValues.DEPENDENCYINFO == "") {
                //        mInfo.ColNameValues["DEPENDENCYINFO"] = _depInfo;
                //    }
                //}

                //mInfo.ModMedia = new List<MarketSvc.MService.MMedia>();

                //mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = mInfo.Image, isVideo = false });

                //for (int img = 0; img < mInfo.Images.Count; img++)
                //{
                //    mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = mInfo.Images[img] });
                //}

                //for (int v = 0; v < mInfo.Videos.Count; v++)
                //{
                //    if (mInfo.Videos[v] != null && mInfo.Videos[v].ToString() != "")
                //    {
                //        mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = "http://i.ytimg.com/vi/" + mInfo.Videos[v] + "/default.jpg", isVideo = true, id = mInfo.Videos[v].ToString() });
                //    }
                //}


                if (i == 0) {
                    this.defaultItem = mInfo;
                    this.totalCount = mInfo.TotalCount;
                }
            }

            if (this.isTabScroll)
                this.mList.push.apply(this.mList, mc.lstModuleInfo);
            else
                this.mList = mc.lstModuleInfo;
            this.bindList(mc.lstModuleInfo, this.isTabScroll);
            this.isTabScroll = false;
            mc = null;
        }
        catch (e) {
        }
        this.setIsBusy(false);
    };

    Market_.prototype.installModule = function (mInfo, skipModDependency) {
        this.setIsBusy(true);
        //Special Check  Systewm Admin

        if (!this.model.getIsSystemAdmin()) {
            this.setIsBusy(false);
            VIS.ADialog.warn("NotAuthorized", true, VIS.Msg.getMsg("LoginAsAdministrator"), "");
            return;
        }

        //Lakhwinder
        //Check Parent Module Installed or not
        if (mInfo.ModuleDependency != null && mInfo.ModuleDependency.length > 0) {

            if (this.getAllParentModule(mInfo)) {
                this.setIsBusy(false);
                return;
            }
        }

        //1. Check For Token Key

        if (!this.prop.isRegisterd) //has not key
        {
            this.openRegisterKeyDialog(!this.hasModuleInstalledAccess(mInfo.ModuleEdition) ? Market.KeyHeader.Professional : Market.KeyHeader.Community, mInfo, Market.KeyStatus.NotRegisterd);
        }
        else if (this.prop.isKeyExpired) // key is expired
        {
            this.openRegisterKeyDialog(Market.KeyHeader.Expired, mInfo, Market.KeyStatus.Renew);
        }

        else if (this.prop.isMarketExpired && this.prop.isProfessinalEdition) // key is expired
        {
            this.openRegisterKeyDialog(Market.KeyHeader.MExpired, mInfo, Market.KeyStatus.Renew);
        }

        else if (!this.hasModuleInstalledAccess(mInfo.ModuleEdition)) // match module and key edition
        {
            this.openRegisterKeyDialog(Market.KeyHeader.Professional, mInfo, Market.KeyStatus.Upgrade);
        }
            // else if (IsPaid  || (IsFeatured && IsPaidEdition(mInfo.ModuleEdition))) // ispaid
        else if ((this.prop.getIsPaid() && !mInfo.IsPaymentDone) || (this.prop.getIsFeatured() && this.getIsPaidEdition(mInfo.ModuleEdition) && !mInfo.IsPaymentDone)) // ispaid
        {
            if (!mInfo.PaymentUrl || mInfo.PaymentUrl.length < 1) {
                mInfo.PaymentUrl = "http://viennaadvantage.com";
            }
            var url1 = mInfo.PaymentUrl;
            if (mInfo.PaymentUrl.indexOf("?id=") < 0) {
                url1 += "?id=" + mInfo.AD_Module_ID;
            }

            url1 += "&key=" + this.prop.tokenKey + "&flag=" + (this.model.getIsCloudMarket() ? "Y" : "N").toString();

            VIS.Env.startBrowser(url1);
            //Envs.StartBrowser(mInfo.PaymentUrl+"?id="+mInfo.AD_Module_ID+"&key="+TokenKey);
            //VAdvantage.Classes.ShowMessage.Info("Redirect", true, " Paymnet URL ", "");
            mInfo.IsEnabled = false;
            //ButtonEnabled = false;
            this.setIsBusy(false);
            return;
        }

        else {
            // if (mInfo.IsPaidSubscription) //Check for payment is done or not

            if (this.getIsPaidEdition(mInfo.ModuleEdition)) {
                if (!mInfo.PaymentUrl || mInfo.PaymentUrl.length < 1) {
                    mInfo.PaymentUrl = "http://viennaadvantage.com";
                }

                var url1 = mInfo.PaymentUrl;
                if (mInfo.PaymentUrl.indexOf("?id=") < 0) {
                    url1 += "?id=" + mInfo.AD_Module_ID;
                }

                url1 += "&key=" + this.prop.tokenKey;

                if (!mInfo.IsPaymentDone) {
                    VIS.Env.startBrowser(url1);
                    //VAdvantage.Classes.ShowMessage.Info("Redirect", true, " paymnet not recieved yet", "");
                    this.setIsBusy(false);
                    return;
                }

                if (mInfo.IsMSubsExpired) {
                    if (VIS.ADialog.ask("SubscriptionExpired", true, VIS.Msg.getMsg("IsResubscribeModule"))) {
                        VIS.Env.startBrowser(url1);

                    }
                    this.setIsBusy(false);
                    return;
                }
            }

            //}
            /*******************************************/
            //Check Already Installed Module Here

            //bool? isInstalled = VAdvantage.Utility.Envs.IsModuleAlreadyInstalled(mInfo.Prefix, mInfo.Version, mInfo.Name);

            // if (isInstalled != null && isInstalled.Value) suggested by karan as a  end User
            //{
            //    VAdvantage.Classes.ShowMessage.Info("Info", true, VAdvantage.Utility.Msg.GetMsg("ModuleIsAlreadyInstalled"), "");
            //    IsBusy = false;
            //}

            //else
            //{

            var m = new Market.ModuleDialog(mInfo);
            m.tokenKey = this.prop.tokenKey;
            m.isCloudMarket = this.model.isCloudMarket;
            m.show();
        }
        this.setIsBusy(false);
    };

    Market_.prototype.getVersionHistory = function (AD_Module_ID, pageNo) {
        var hDialog = new Market.HistoryDialog(AD_Module_ID);
        hDialog.show();
       // this.setIsBusy(true);
       
    };

    Market_.prototype.getAllParentModule = function (mInfo) {
        var _dependencyInfo = [];

        for (var i = 0; i < mInfo.ModuleDependency.length; i++) {
            var isInstalled = mInfo.DependentModInstalled[i];

            if (!isInstalled)
                _dependencyInfo.push(mInfo.ModuleDependency[i]);
        }
        if (_dependencyInfo.length > 0) {
            var dep = new Market.DependencyDialog(_dependencyInfo, mInfo.Name + " " + mInfo.VersionNo, this);
            dep.show();
            dep = null;
            return true;
        }

        return false;
    };

    Market_.prototype.hasModuleInstalledAccess = function (mEditionPayment) {
        if (!mEditionPayment) {
            return true;;
        }
        if (this.prop.isProfessinalEdition) //Is professional key
        {
            return true;
        }
        else //is community key
        {
            if (mEditionPayment.toUpper() == "PF" || mEditionPayment.toUpper() == "PP") {
                return false;
            }
        }
        return true;
    };

    Market_.prototype.getIsTokenKeyValidated = function () {

        //if (!this.model.getIsSystemAdmin()) {
        //    this.setIsBusy(false);
        //    VIS.ADialog.warn("NotAuthorized", true, VIS.Msg.getMsg("LoginAsAdministrator"), "");
        //    return;
        //}

        //1. Check For Token Key

        if (!this.prop.isRegisterd) //has not key
        {
            this.openRegisterKeyDialog(!this.hasModuleInstalledAccess() ? Market.KeyHeader.Professional : Market.KeyHeader.Community, null, Market.KeyStatus.NotRegisterd, true);
        }
        else if (this.prop.isKeyExpired) // key is expired
        {
            this.openRegisterKeyDialog(Market.KeyHeader.Expired, null, Market.KeyStatus.Renew, true);
        }
        else if (this.prop.isMarketExpired && this.prop.isProfessinalEdition) // key is expired
        {
            this.openRegisterKeyDialog(Market.KeyHeader.MExpired, null, Market.KeyStatus.Renew, true);
        }
        else
            return true;
        return false;
    };


    Market_.prototype.openRegisterKeyDialog = function (keyHeader, mInfo, keyStatus, isPrivateTab) {

        var d = new Market.KeyDialog(keyHeader, this.model.getIsCloudMarket());
        var self = this;
        d.onClose = function () {
            self.setIsBusy(false);
            if (d.getIsValid() && d.getIsMarketValid()) {
                self.prop.isRegisterd = d.isValid;
                self.prop.isProfessinalEdition = d.isProfessinalEdition;
                self.prop.tokenKey = d.tokenKey;
                self.prop.isKeyExpired = false;
                self.prop.isMarketExpired = false;
                //Recall Install Module function continue with installation
                if (isPrivateTab) {
                    self.getPrivateModule();
                }
                else
                    self.installModule(mInfo);
                self = null;
            };
        };
        //else
        //{

        //}
        //};
        d.tokenKey = this.prop.tokenKey;
        d.tokenKeyStatus = keyStatus;
        d.isCloudMarket = this.model.getIsCloudMarket();
        d.show();
    };


    /* end */

    /* show form 
     - invoke from shortcut.js class 
     */
    Market_.prototype.show = function () {
        var c = new VIS.CFrame();
        c.setName(VIS.Msg.getMsg("Market"));
        c.setTitle(VIS.Msg.getMsg("Market"));
        c.hideHeader(true);
        // this.initializeComponent();
        c.setContent(this);
        c.show();
    };

    /* dispose 
    - call when close this form 
    */
    Market_.prototype.dispose = function () {
        this.disposeComponent();
    };

    /* Size changed
    - change size of this form according to screen size changed 
    - call from viewmanager.js class 
    */
    Market_.prototype.sizeChanged = function (height, width) {
        this.getRoot().css({ "width": width, "height": height });
        window.setTimeout(function (t) {
            t.setSize(height, width);
        }, 10, this);
    };

    /* assign in global namespace*/
    Market.ImpModule.Market_ = Market_;


    var isDialogClosed = false;
    var d  = null;
    Market.checkTokenKey = function (onlyTimeOut) {

        var model = new MarketModel(null);
        // if (model.getIsCloudMarket())
        //   return;

        var info = {};
        info.Url = VIS.Application.contextFullUrl;
        info.Client_ID = VIS.Env.getCtx().getAD_Client_ID();
        info.UserName = VIS.Env.getCtx().getAD_User_Name();
        info.RoleName = VIS.Env.getCtx().getAD_Role_Name();
        info.ClientName = VIS.Env.getCtx().getAD_Client_Name();
        info.IsDemoCheck = true;
        info.IsEntCheck = self.checkEntKey;
        info.IsAllowWork = model.getIsCloudMarket();


        $.ajax({
            url: VIS.Application.contextUrl + 'Market/Module/InitLoginOrValidate',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: true,
            data: JSON.stringify(info)
        }).done(function (json) {
            var ed = JSON.parse(json);

            //self.isValid = ed.IsValid;
            // self.isProfessinalEdition = ed.IsAllowWork;
            // self.tokenKey = ed.Token;
            // End 

            // Market.isKeyRegistered = ed.IsRegistered;
            //  Market.isProfessionalKey = ed.IsAllowWork;
            Market.keyMgr.init(ed.IsRegistered, ed.IsAllowWork, ed.IsExpired);

            if (!ed.IsRegistered || !ed.IsAllowWork || ed.IsValid || model.getIsCloudMarket()) {

            }
            else {

                if (!onlyTimeOut) {
                    d = new Market.KeyDialog(Market.KeyHeader.Expired, model.getIsCloudMarket());
                    d.show(true);
                    isDialogClosed = false;
                    d.tokenKeyStatus = Market.KeyStatus.Renew;
                    d.setHeader(ed.Message);
                    d.onClose = function () {
                        isDialogClosed = true;
                    };
                }

                window.setTimeout(function () {
                    if (isDialogClosed)
                        return;
                    //if (!jQuery.contains(document, d.getRoot()[0]))
                    //    Market.checkTokenKey(false);
                    //else if (!d.getRoot().is(":visible")) {
                    //    var prnt = d.getRoot();
                    //    while (!prnt.is(":visible")) {
                    //        prnt.show();
                    //        prnt = prnt.parent();
                    //        if (prnt.attr("role") == "dialog") {
                    //            prnt.next().show();
                    //        }
                    //    }
                    //    Market.checkTokenKey(true);
                    //}
                    //else {
                    //forcefully reset 
                    d.closeDialog();
                    isDialogClosed = false;
                    d = null;
                    Market.checkTokenKey(false);

                        //Market.checkTokenKey(true);
                    //}
                }, 1000 * 30);
            }

        }).fail(function (xhe, e) {
            VIS.ADialog.error("Error?", true, e);
        });


    };





    function keyMgr() {
        var isReg = false;
        var isProf = false;
        var isExp = false;

        function init(isRe, isPro, isEx) {
            isReg = isRe;
            isProf = isPro;
            isExp = isEx;
        };


        function getIsRegistered() {
            return isReg;
        };
        function getIsProfessional() {
            return isProf;
        };
        function getIsExpired() {
            return isExp;
        };

        return {

            getIsRegistered: getIsRegistered,
            getIsProfessional: getIsProfessional,
            getIsExpired: getIsExpired,
            init: init
        };

    };
    Market.keyMgr = keyMgr();



    Market.checkTokenKey();

    /* Property Model */

    /* 
    /// Class contain Module Properties 
    */

})(VIS, jQuery);