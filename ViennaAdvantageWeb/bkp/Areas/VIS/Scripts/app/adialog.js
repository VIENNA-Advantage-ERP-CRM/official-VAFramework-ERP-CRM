; (function (VIS, $) {

    /**
     *  Info Dialog Management
     *  
     */
    var $body = $('body');
    var $prodBuzzer = $('#vis_buzzer');

    function AD() {

        var _overLay = $('<div id="overlayMsgDialog" class="web_dialog_overlay"></div>');

        var $mainDivParent = $('<div class="vis-PopupWrap-alertmain" tabIndex=1>');
        //<div class="vis-confirm-popup-check"><input type="checkbox"><label>Background</label></div>
        var $mainDiv = $('<div id="VAPOS_ErrorInfo" class="vis-PopupWrap-alert">' +
              '<input class="vis-Dialog-buttons-text" type="number"  tabindex="-30" style="z-index:-44;position:absolute"  autofocus="autofocus"  > ' +
            '       <div class="vis-popup-headerContainer">                                           ' +
            '       <div class="vis-PopupHeader-alert">' +
            '           <h4 id="VAPOS_CLInformation">Information</h4>' +
            '           <span id="btnCloseInfo" class="fa fa-times"></span>                     ' +
            '       </div>  </div>                                                                                   ' +
            '       <div class="vis-PopupContent-alert">                                                     ' +

            '               <div class="form-group vis-PopupInput-alert" style="width: 100%">                 ' +
            '               <img class="vis-alert-img" style="float:left"  />                                                   ' +
            '                   <label style="width: 90%;padding-left: 10px;word-break: break-word" id="VAPOS_lblErrorInfo"></label>                  ' +
            '                 <div class="vis-confirm-customUI" style="display:none"> </div>         ' +
            '               </div>                                                                           ' +
            '                     <div class="vis-Dialog-buttons" style="display:none">                                           ' +
            '<input class="vis-Dialog-buttons-OK " type="button" value="' + VIS.Msg.getMsg("OK") + '" > <input  class="vis-Dialog-buttons-Cancel"  type="button" value="' + VIS.Msg.getMsg("Cancel") + '" >' +
            '                                               </div>                                           ' +
            '                                                                                                ' +

            '       </div>                                                                                   ' +
            '   </div>');

        $mainDivParent.append($mainDiv);

        $body.append(_overLay).append($mainDivParent);

        var _hideOverlay = true;
        var _header = $mainDiv.find(".vis-PopupHeader-alert");

        var _headerContainer = $mainDiv.find('.vis-popup-headerContainer');

        var _btnCloseInfo = $mainDiv.find("#btnCloseInfo"); //btn close

        var _headerText = $mainDiv.find("#VAPOS_CLInformation"); //headertext
        var _headerImg = $mainDiv.find(".vis-alert-img"); //header img

        var _content = $mainDiv.find(".vis-PopupContent-alert");

        var _contentMsg = $mainDiv.find("#VAPOS_lblErrorInfo"); //label

        var _customUI = $mainDiv.find('.vis-confirm-customUI');

        //  var _overLay = _main.find("#overlayMsgDialog");
        var _busyInd = $mainDiv.find("#VAPOS_busyInd");

        var _btnOK = null;
        var _btnCancel = null;
        //var _txtBx = $mainDiv.find(".vis-Dialog-buttons-text");;

        var _callback = null;

        _btnCloseInfo.on("click", function (e) {
            closeIt(e);
        });

        function handleKeys(askUser) {
            if (askUser) {
                window.setTimeout(function () {
                    _btnCancel.focus();
                    $mainDivParent.on("keydown", function (e) {
                        if (e.keyCode == 9) {
                            if (_btnOK.is(':focus')) {
                                _btnCancel.focus();
                            }
                            else if (_btnCancel.is(':focus')) {
                                _btnOK.focus();
                            }
                            else {
                                _btnCancel.focus();
                            }
                            e.preventDefault();
                            e.stopPropagation();
                            //return false;
                        }
                        else if (e.keyCode == 27) {
                            e.preventDefault();
                            e.stopPropagation();
                            closeIt(e);
                            //  _txtBx.off("keydown");
                            return false;
                        }
                    });
                }, 100);
            }
            else {
                $mainDivParent.focus();
                $mainDivParent.on("keydown", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    closeIt(e);
                    //  _txtBx.off("keydown");
                    return false;
                });
            }
        };



        function closeIt(e) {
            e.stopPropagation();

            if (_callback)
                _callback();
            _callback = null;

            disposeEvents();
        };



        function disposeEvents() {
            _overLay.hide();

            $mainDivParent.css({ "position": "inherit", "display": "none" });
            $mainDiv.find('.vis-Dialog-buttons').css("display", "none");
            //$mainDiv.fadeOut(300);

            if (_btnOK) {
                _btnOK.off("click");
            }

            if (_btnCancel) {
                _btnCancel.off("click");
            }

            $mainDivParent.off("keydown");

            //$mainDivParent.remove();
            //$mainDiv.remove();
            //$mainDivParent = null;
            //$mainDiv = null;
            //_btnOK.remove();
            //_btnCancel.remove();
            //_btnCancel = null;
            //_btnOK = null;
        };


        function info(msg, header, callback) {
            _callback = callback;
            try {
                // $('#prodError')[0].play();
            }
            catch (ex) {
            }
            //$mainDiv.find('.vis-Dialog-buttons').css("display", "none");
            $mainDivParent.css({ "position": "absolute", "display": "flex" });
            $mainDiv.show();
            //_btnCloseInfo.removeClass();
            //_btnCloseInfo.addClass("vis-alert-close vis-alert-close-info");
            _header.removeClass();
            _header.addClass("vis-PopupHeader-alert vis-PopupHeader-alert-info");
            _content.removeClass();
            _content.addClass("vis-PopupContent-alert vis-PopupContent-alert-info");
            _headerContainer.removeClass();
            _headerContainer.addClass("vis-popup-headerContainer vis-popup-headerContainer-info");
            _headerImg.attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/info-icon.png");

            _prodFound = true;
            _contentMsg.text(msg);
            if (!header) {
                _headerText.text(VIS.Msg.getMsg("Info"));
            }
            else {
                _headerText.text(VIS.Msg.getMsg(header));
            }
            //_hideOverlay = hideOverlay;
            _overLay.show();
            $mainDiv.fadeIn(300);
            handleKeys(false);
            //_txtBx.on("keydown", function (e) {
            //    clss(e);
            //});

            //_txtBx.on("focusin", function (e) {
            //    e.stopPropagation();
            //});

            //    _txtBx.focus();

        };


        function clss(e) {
            if (e.keyCode == 27) {
                e.preventDefault();
                e.stopPropagation();
                closeIt(e);
                //  _txtBx.off("keydown");
                return false;
            }
        };

        function ask(msg, header, callback) {
            _callback = callback;
            try {
                // $('#prodError')[0].play();
            }
            catch (ex) {
            }
            $mainDivParent.css({ "position": "absolute", "display": "flex" });
            $mainDiv.show();
            //_btnCloseInfo.removeClass();
            //_btnCloseInfo.addClass("vis-alert-close vis-alert-close-info");
            _header.removeClass();
            _header.addClass("vis-PopupHeader-alert vis-PopupHeader-alert-info");
            _content.removeClass();
            _content.addClass("vis-PopupContent-alert vis-PopupContent-alert-info vis-PopupContent-alert-Confirm");
            _headerContainer.removeClass();
            _headerContainer.addClass("vis-popup-headerContainer vis-popup-headerContainer-info");

            _headerImg.attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/confirm-icon.png");

            var $btnsDiv = $mainDiv.find('.vis-Dialog-buttons');

            $btnsDiv.css("display", "inherit");

            _btnOK = $btnsDiv.find(".vis-Dialog-buttons-OK");
            _btnCancel = $btnsDiv.find(".vis-Dialog-buttons-Cancel");

            _prodFound = true;
            _contentMsg.text(msg);
            if (!header) {
                _headerText.text(VIS.Msg.getMsg("Confirm"));
            }
            else {
                _headerText.text(VIS.Msg.getMsg(header));
            }
            //_hideOverlay = hideOverlay;
            _overLay.show();
            $mainDiv.fadeIn(300);

            _btnOK.one("click", function () {
                disposeEvents();
                $btnsDiv.css("display", "none");
                _callback(true);
                // _btnCloseInfo.trigger("click");

            });

            _btnCancel.one("click", function () {
                disposeEvents();
                $btnsDiv.css("display", "none");
                _callback(false);
                // _btnCloseInfo.trigger("click");

            });
            handleKeys(true);
            //_txtBx.on("keydown", function (e) {
            //    clss(e);
            //});

            //_txtBx.on("focusin", function (e) {
            //    e.stopPropagation();
            //});

            //_txtBx.focus();

        };

        function askCustomUI(msg, header, $rootDiv, callback) {
            _callback = callback;
            try {
                // $('#prodError')[0].play();
            }
            catch (ex) {
            }
            $mainDivParent.css({ "position": "absolute", "display": "flex" });
            $mainDiv.show();
            //_btnCloseInfo.removeClass();
            //_btnCloseInfo.addClass("vis-alert-close vis-alert-close-info");
            _header.removeClass();
            _header.addClass("vis-PopupHeader-alert vis-PopupHeader-alert-info");
            _content.removeClass();
            _content.addClass("vis-PopupContent-alert vis-PopupContent-alert-info vis-PopupContent-alert-Confirm");
            _headerContainer.removeClass();
            _headerContainer.addClass("vis-popup-headerContainer vis-popup-headerContainer-info");

            _headerImg.attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/confirm-icon.png");

            var $btnsDiv = $mainDiv.find('.vis-Dialog-buttons');

            $btnsDiv.css("display", "inherit");

            _btnOK = $btnsDiv.find(".vis-Dialog-buttons-OK");
            _btnCancel = $btnsDiv.find(".vis-Dialog-buttons-Cancel");

            _prodFound = true;
            _contentMsg.text(msg);
            _customUI.empty();
            _customUI.css('display', 'block');
            _customUI.append($rootDiv);
            if (!header) {
                _headerText.text(VIS.Msg.getMsg("Confirm"));
            }
            else {
                _headerText.text(VIS.Msg.getMsg(header));
            }
            //_hideOverlay = hideOverlay;
            _overLay.show();
            $mainDiv.fadeIn(300);

            _btnOK.one("click", function () {
                disposeEvents();
                $btnsDiv.css("display", "none");
                _customUI.css('display', 'none');
                _customUI.empty();
                _callback(true);
                // _btnCloseInfo.trigger("click");

            });

            _btnCancel.one("click", function () {
                disposeEvents();
                $btnsDiv.css("display", "none");
                _customUI.css('display', 'none');
                _customUI.empty();
                _callback(false);
                // _btnCloseInfo.trigger("click");

            });
            handleKeys(true);
            //_txtBx.on("keydown", function (e) {
            //    clss(e);
            //});

            //_txtBx.on("focusin", function (e) {
            //    e.stopPropagation();
            //});

            //_txtBx.focus();

        };

        function warn(msg, header, callback) {
            _callback = callback;
            try {
                //$prodBuzzer[0].play();
            }
            catch (ex) {
            }
            //$mainDiv.find('.vis-Dialog-buttons').css("display", "none");
            _prodFound = true;
            $mainDivParent.css({ "position": "absolute", "display": "flex" });
            $mainDiv.show();
            //_btnCloseInfo.removeClass();
            //_btnCloseInfo.addClass("vis-alert-close vis-alert-close-warn");
            _header.removeClass();
            _header.addClass("vis-PopupHeader-alert vis-PopupHeader-alert-warn");
            _content.removeClass();
            _content.addClass("vis-PopupContent-alert vis-PopupContent-alert-warn");

            _headerContainer.removeClass();
            _headerContainer.addClass("vis-popup-headerContainer vis-popup-headerContainer-warn");
            _headerImg.attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/warning-icon.png");



            _contentMsg.text(msg);
            // _hideOverlay = hideOverlay;
            if (!header) {
                _headerText.text(VIS.Msg.getMsg("Warning"));
            }
            else {
                _headerText.text(VIS.Msg.getMsg(header));
            }

            _overLay.show();
            handleKeys(false);
            //_txtBx.on("keydown", function (e) {
            //    clss(e);
            //});

            //_txtBx.on("focusin", function (e) {
            //    e.stopPropagation();
            //});

            //_txtBx.focus();




            // _busyInd.hide();
        };

        function error(msg, header, callback) {
            _callback = callback;
            try {
                //$prodBuzzer[0].play();
            }
            catch (ex) {
            }
            //$btnsDiv.css("display", "none");
            _prodFound = true;
            $mainDivParent.css({ "position": "absolute", "display": "flex" });
            $mainDiv.show();
            //_btnCloseInfo.removeClass();
            //_btnCloseInfo.addClass("vis-alert-close vis-alert-close-error");
            _header.removeClass();
            _header.addClass("vis-PopupHeader-alert vis-PopupHeader-alert-error");
            _content.removeClass();
            _content.addClass("vis-PopupContent-alert vis-PopupContent-alert-error");
            _headerContainer.removeClass();
            _headerContainer.addClass("vis-popup-headerContainer vis-popup-headerContainer-error");
            _headerImg.attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/error-icon.png");

            _contentMsg.text(msg);
            // _hideOverlay = hideOverlay;
            if (!header) {
                _headerText.text(VIS.Msg.getMsg("Error"));
            }
            else {
                _headerText.text(VIS.Msg.getMsg(header));
            }
            _overLay.show();
            $mainDiv.fadeIn(300);
            handleKeys(false);
            //_txtBx.on("keydown", function (e) {
            //    clss(e);
            //});

            //_txtBx.on("focusin", function (e) {
            //    e.stopPropagation();
            //});

            //_txtBx.focus();



            //_busyInd.hide();
        };

        return {
            info: info,
            ask: ask,
            error: error,
            warn: warn,
            askCustomUI: askCustomUI
        }

    };


    VIS.ADialog = {

        /**
         *  Show PAIN message with info 
         *  @method info
         *  @param keyName Keyword Name
         *  @param isTextMsg if true returns Message Text, if false returns Message Tip
         *     and if null then returns both message text and tip.
         *  @param extraMsg extra message to be displayed
         */
        info: function (keyName, isMsgText, extraMsg, header) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }

            VIS.ADialogUI.info(content, header);



            //var $p = $('<p>');
            //$p.text(content);
            //div.append($p);

            //div.dialog();

            //alert(content);
            content = null;
        },

        /**
       *	Ask Question with question icon and (OK) (Cancel) buttons
       *    @method ask
       *	@param	keyName	Message to be translated
       *	@param	msg			Additional clear text message
       *	@return true, if OK
       */

        ask: function (keyName, isMsgText, extraMsg, header) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            var retValue = false;
            // opens message window
            //Message d = new Message(header, content.ToString(), Message.MessageType.QUESTION);
            if (confirm(content))// d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // if user clicks on OK button change the value
                retValue = true;
            }
            return retValue;
        },


        /**
	     *	Display error with error icon
         *  @method error
	     *	@param	keyName	Message to be translated
         *  @param  isMsgText 
	     *	@param	extraMsg			Additional message
	     */
        error: function (keyName, isMsgText, extraMsg, header) {
            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }

            // if user has given statusbar label, then show the messsage on status bar also

            VIS.ADialogUI.error(content, header);

            content = null;
        },

        /**
	     *	Display warning with warning icon
         *  @method warn
	     *	@param	keyName	Message to be translated
         *  @param  isMsgText 
	     *	@param	extraMsg			Additional message
	     */
        warn: function (keyName, isMsgText, extraMsg, header) {
            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }

            //  alert(content);

            VIS.ADialogUI.warn(content, header);
            content = null;
        },


        confirm: function (keyName, isMsgText, extraMsg, header, callback) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            var retValue = false;
            // opens message window
            //Message d = new Message(header, content.ToString(), Message.MessageType.QUESTION);
            //if (confirm(content))// d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    // if user clicks on OK button change the value
            //    retValue = true;
            //}

            VIS.ADialogUI.ask(content, header, callback);

            return retValue;
        },

        confirmCustomUI: function (keyName, isMsgText, extraMsg, header, $rootDiv, callback) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            var retValue = false;
            // opens message window
            //Message d = new Message(header, content.ToString(), Message.MessageType.QUESTION);
            //if (confirm(content))// d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    // if user clicks on OK button change the value
            //    retValue = true;
            //}

            VIS.ADialogUI.askCustomUI(content, header,$rootDiv, callback);

            return retValue;
        },


    };


    VIS.ADialogUI = AD();


}(VIS, jQuery));