; (function (VIS, $) {

    function shortcutMgr() {

        var linkDiv;
        var linkConatiner;

        var Items = null;

        var log = VIS.Logging.VLogger.getVLogger("ShortCut");

        var atab = $('#userLinks-Tab');
        var mgr = {
            init: init
        };

        return mgr;
        // initialization 
        function init(_linkDiv, _favDiv) {
            linkDiv = _linkDiv;
            linkConatiner = linkDiv.find("#vis_linkScroll");
            VIS.favMgr.init(_favDiv);

            getShortcutData();
            bindEvents();
        };

        // get shortcut data
        function getShortcutData() {

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Home/GetShortcutItems", null, function (data) {
                Items = data;
                var itm = null;
                var html = ['<ul class="vis-userLinks-ListMenu">'];
                if (data) {
                    for (var i = 0; i < data.length; i++) {
                        itm = data[i];

                        html.push('<li data-index="' + i + '" ><a data-index="' + i + '" href="javascript:void(0)">');
                         

                        if (itm.HasImage) {
                            if (!itm.IsImageByteArray && itm.IconUrl.indexOf('.') < 0) {
                                html.push('<i data-index="' + i + '" class="' + itm.IconUrl + '"');
                            }
                            else {
                                html.push('<img data-index="' + i + '"  src="');
                                if (itm.IsImageByteArray) {
                                    html.push('data:image/*;base64,' + itm.IconBytes);
                                }
                                else {
                                    html.push(VIS.Application.contextUrl + itm.IconUrl);
                                }
                            }
                        }
                        else {
                            html.push('<i data-index="' + i + '" class="vis vis-shortcut"');
                        }
                        html.push('" />');
                        html.push(itm.ShortcutName + '</a></li>');
                    }
                }
                html.push('</ul>');
                linkConatiner.empty();
                linkConatiner.html(html.join(' '));
                html = [];
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
           
            atab.empty();
            atab.append($("<span><i class='fa fa-link' aria-hidden='true' style='transform: rotate(90deg);'></i></span>"));
            atab.append(VIS.Msg.getMsg('Links') + " - ");
            atab.append($("<strong>").append(count));
           // atab = null;
        }
    };


    /* 
    * Setting Dialog
    */
    function SettingDialog(id) {

        var $root = $("<div>");
        //var $divScroll = $('<div  class="scrollerVertical" style="width:auto">').html('<div class="vis-apanel-busy" style="height:280px;position:static"> </div>');
        // Manish 29/6/2017
        //var $divScroll = $('<div style="width:auto;z-index:1">').html('<div class="vis-apanel-busy" style="height:280px;position:static"> </div>');

        var $divScroll = $('<div style="width:auto;z-index:1">').html('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        
        // end 29/6/2017

        var log = VIS.Logging.VLogger.getVLogger("SettingDialog");
        $root.append($divScroll);
        var ch = null;
        var Items = null;

        // get settings
        function init() {
                                                                                               
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Home/GetSettingItems", { "AD_Shortcut_ID": id }, function (data) {
                Items = data;
                var itm = null;
                var html = ['<ul class="vis-userLinks-ListMenu">'];
                if (data) {
                    for (var i = 0; i < data.length; i++) {
                        itm = data[i];
                        html.push('<li data-index="' + i + '" ><a data-index="' + i + '" href="javascript:void(0)">');


                        if (itm.HasImage) {
                            if (!itm.IsImageByteArray && itm.IconUrl.indexOf('.') < 0) {
                                html.push('<i data-index="' + i + '" class="' + itm.IconUrl + '"');
                            }
                            else {
                                html.push('<img data-index="' + i + '"  src="');
                                if (itm.IsImageByteArray) {
                                    html.push('data:image/*;base64,' + itm.IconBytes);
                                }
                                else {
                                    html.push(VIS.Application.contextUrl + itm.IconUrl);
                                }
                            }
                        }
                        else {
                            html.push('<i data-index="' + i + '" class="vis vis-shortcut"');
                        }
                        html.push('" />');
                        html.push(itm.ShortcutName + '</a></li>');
                    }
                }
                html.push('</ul>');
                $divScroll.empty();
                $divScroll.html(html.join(' '));
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
                        //alert("setting Dialog");
                        VIS.Msg.getMsg("settingDialog")
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