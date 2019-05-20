; (function (VIS, $) {

    function shortcutMgr() {

        var linkDiv;
        var linkConatiner;

        var Items = null;

        var log = VIS.Logging.VLogger.getVLogger("ShortCut");

        var mgr = {
            init: init
        };

        return mgr;

        function init(_linkDiv, _favDiv) {
            linkDiv = _linkDiv;
            linkConatiner = linkDiv.find("#vis_linkScroll");
            VIS.favMgr.init(_favDiv);

            getShortcutData();
            bindEvents();
        };

        function getShortcutData() {

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Home/GetShortcutItems", null, function (data) {
                Items = data;
                var itm = null;
                var html = '<ul class="vis-userLinks-ListMenu">';
                for (var i = 0; i < data.length; i++) {
                    itm = data[i];
                    html += '<li data-index="' + i + '" ><a data-index="' + i + '" href="javascript:void(0)"><img data-index="' + i + '" style="width: 54px;height: 63px;margin-bottom: 10px;margin-right: auto;margin-left: auto;display:block;" src="';

                    if (itm.HasImage) {
                        if (itm.IsImageByteArray) {
                            html += 'data:image/*;base64,' + itm.IconBytes;
                        }
                        else {
                            html += VIS.Application.contextUrl + itm.IconUrl;
                        }
                    }
                    html += '" />';
                    html += itm.ShortcutName + '</a></li>';
                }
                html += '</ul>';
                linkConatiner.empty();
                linkConatiner.html(html);
                setCount(data.length);
            });
        };

        function bindEvents() {
           
            linkConatiner.on("click", function (e) {
                var index = $(e.target).data("index");
                if ((index || index == "0") && index < Items.length) {
                    var dsi = Items[index];

                    //1   Contain Child ShortCut

                    if (dsi.HasChild) {
                        // alert("setting Dialog");
                        var sd = new SettingDialog(dsi.KeyID);
                        sd.show();
                        sd = null;
                    }

                        //2 If URL

                    else if (dsi.Url || dsi.Url.length > 0) {
                        VIS.Env.startBrowser(dsi.Url);
                    }

                        // 3 Special Class
                    else if (dsi.SpecialAction && dsi.SpecialAction.length > 0) {
                        //check name has moduleprefix
                        var className = dsi.SpecialAction;
                        //Get form Name
                        var formName = dsi.ActionName; // className.Substring(className.LastIndexOf('.') + 1);

                        try {

                            //className = "VIS.Apps.TestForm";
                            var type = VIS.Utility.getFunctionByName(className, window);
                            var o = new type();
                            o.show();
                            o = null;
                        }
                        catch (e) {
                            log.log(VIS.Logging.Level.WARNING, "Class=" + className + ", Action Class Name=" + className, e)
                            return false;
                        }
                    }

                    else //Entity Action
                    {
                        if (dsi.Action == null || dsi.Action.length <= 0 || dsi.ActionID < 1) {
                            return;
                        }
                        VIS.viewManager.startAction(dsi.Action, dsi.ActionID);
                    }
                   
                }
            });
        }

        function setCount(count) {
            var atab = $('#userLinks-Tab');
            atab.empty();
            atab.append($("<span class='vis-linksIcons vis-icon-link'>"));
            atab.append(VIS.Msg.getMsg('Links') + " - ");
            atab.append($("<strong>").append(count));
            atab = null;
        }
    };


    /* 
    * Setting Dialog
    */
    function SettingDialog(id) {

        var $root = $("<div>");
        var $divScroll = $('<div  class="scrollerVertical" style="width:auto">').html('<div class="vis-apanel-busy" style="height:280px;position:static"> </div>');
        var log = VIS.Logging.VLogger.getVLogger("SettingDialog");
        $root.append($divScroll);
        var ch = null;
        var Items = null;

        function init() {
                                                                                               
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Home/GetSettingItems", { "AD_Shortcut_ID": id }, function (data) {
                Items = data;
                var itm = null;
                var html = '<ul class="vis-userLinks-ListMenu">';
                for (var i = 0; i < data.length; i++) {
                    itm = data[i];
                    html += '<li data-index="' + i + '" ><a data-index="' + i + '" href="javascript:void(0)"><img data-index="' + i + '" style="width: 54px;margin-bottom: 10px;height: 63px;display:block;" src="';

                    if (itm.HasImage) {
                        if (itm.IsImageByteArray) {
                            html += 'data:image/*;base64,' + itm.IconBytes;
                        }
                        else {
                            html += VIS.Application.contextUrl + itm.IconUrl;
                        }
                    }
                    html += '" />';
                    html += itm.ShortcutName + '</a></li>';
                }
                html += '</ul>';
                $divScroll.empty();
                $divScroll.html(html);
                bindEvents();
            });
        };

        function bindEvents() {
            $divScroll.on("click", function (e) {
                var index = $(e.target).data("index");
                if ((index || index == "0") && index < Items.length) {
                    var dsi = Items[index];

                    //1   Contain Child ShortCut

                    if (dsi.HasChild) {
                        alert("setting Dialog");
                        //SettingDialog sd = new SettingDialog(dsi.KeyID);
                        //sd.Show();
                    }

                        //2 If URL

                    else if (dsi.Url || dsi.Url.length > 0) {
                        VIS.Env.startBrowser(dsi.Url);
                    }

                        // 3 Special Class
                    else if (dsi.SpecialAction && dsi.SpecialAction.length > 0) {
                        //check name has moduleprefix
                        var className = dsi.SpecialAction;
                        //Get form Name
                        var formName = dsi.ActionName; // className.Substring(className.LastIndexOf('.') + 1);

                        try {

                            //className = "VIS.Apps.TestForm";
                            var type = VIS.Utility.getFunctionByName(className, window);
                            var o = new type();
                            o.show();
                            o = null;
                        }
                        catch (e) {
                             log.log(VIS.Logging.Level.WARNING, "Class=" + className + ", Action Class Name=" + className, e)
                            return false;
                        }
                    }

                    else //Entity Action
                    {
                        if (dsi.Action == null || dsi.Action.length <= 0 || dsi.ActionID < 1) {
                            return;
                        }
                        VIS.viewManager.startAction(dsi.Action, dsi.ActionID);
                    }
                    ch.close();
                }
            });
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setWidth(540);
            ch.setHeight(400);
            ch.setTitle(VIS.Msg.getMsg("Setting"));
            ch.setContent($root);
            ch.show();
        };

        this.disposeComponent = function () {
            $divScroll.off("click");
            $divScroll.remove();
            if (Items)
                Items.length = 0;
            Items = null;
            this.show = null;
            $root = null;
            if (ch)
                ch.dispose();

        };

        init();
    };

    SettingDialog.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.shortcutMgr = shortcutMgr();
}(VIS, jQuery));