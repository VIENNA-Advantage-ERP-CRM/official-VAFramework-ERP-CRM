; (function (VIS, $) {
    //var $record_id, $chat_id, $table_id, $description, $chatclose;
    function Chat(record_id, chat_id, table_id, description, windowNo) {

        this.onClose = null; //outer apanel close function

        var $maindiv = $('<div></div>'); //layout
        var $div = $('<div style="overflow-y:auto;height:216px;margin-right:-2px;margin-bottom:15px"></div>');
        var $inputChat = $('<textarea  id="input-chat-new" style="Height:70px;width:624px;text-wrap:normal;margin-top:0px;font-size: 10pt;resize: none;margin-bottom:-10px;margin-right:3px"  maxlength="500" />');
        //  var $buttonsdiv = $('<div style="overflow:auto"></div>');
        // var $btnOK = $('<button>');
        // var $btnCancel = $('<button>');
        $maindiv.append($div).append($inputChat);//.append($buttonsdiv); //ui
        this.winNo = windowNo;

        var ch = null;
        this.prop = { WindowNo: windowNo, ChatID: chat_id, AD_Table_ID: table_id, Record_ID: record_id, Description: description, TrxName: null, ChatText: "", page: 0, pageSize: 10 };

        init($div, windowNo, this.prop);
        var self = this;
        createButtons();
        //events();
      
        
        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setContent($maindiv);
            ch.setHeight(428);
            ch.setWidth(655);
            ch.setTitle(VIS.Msg.getMsg("Chat"));
            ch.setModal(true);
            //Ok Button Click
            //  ch.onOkClick =

            //Disposing Everything on Close
            ch.onClose = function () {

                if (self.onClose) self.onClose();
                self.dispose();
            };
            ch.show();
            events();
            //ch.hidebuttons();
        };

        this.dispose = function () {
            this.prop = null;
            $div = null;
            $maindiv.remove();
            $maindiv = null;

            ch = null;
            self = null;

        };

        function createButtons() {
            //Ok Button
            //$btnOK.addClass("VIS_Pref_btn-2");
            //$btnOK.text(VIS.Msg.getMsg("OK"));
            //$btnOK.css({ "margin-top": "0px", "margin-bottom": "0px" });


            ////Cancel Button
            //$btnCancel.addClass("VIS_Pref_btn-2");
            //$btnCancel.text(VIS.Msg.getMsg("Cancel"));
            //$btnCancel.css({ "margin-top": "0px", "margin-bottom": "0px", "margin-left": "5px" });

            // $buttonsdiv.append($btnCancel).append($btnOK);
        }

        function events() {
            ch.onOkClick = function (e) {
                var text = $inputChat.val();
                if ($.trim(text) == "" || text == "" || text == null) {
                    VIS.ADialog.info(VIS.Msg.getMsg("EnterData"));
                    if (e != undefined) {
                        e.preventDefault();
                    }
                    return false;
                }
                //  $btnOK.prop('disabled', true);
                self.prop.ChatText = text;
                VIS.dataContext.saveChat(self.prop);
                //ch.close();
                //if (self.onClose) self.onClose();
                //self.dispose();
            };

            ch.onCancelClick = function () {
                // ch.close();
                //if (self.onClose) self.onClose();
                //  self.dispose();
            };

        };

        function init($container, windowNo, prop) {

            VIS.dataContext.getChatRecords(prop, function (result) {

                var data = JSON.parse(result);
                var htmll = "";
                for (var chat in data.subChat) {

                    var d = new Date(data.subChat[chat].ChatDate);
                    var date = d.toLocaleString();

                    //     (Globalize.format(date, 'G', Globalize.cultureSelector));


                    var str = '   <div style="margin-bottom: 15px; border: 1px solid DarkGray; min-height: 50px; border-radius: 4px;overflow-y:auto;margin-right:5px">';


                    if (VIS.Application.isRTL) {
                        str += '<div style="overflow: auto;height: 48px;width: 48px;display: inline;float: right;margin: 5px;position:relative;line-height:45px;text-align:center;">';
                    }
                    else {
                        str += '<div style="overflow: auto;height: 48px;width: 48px;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;">';
                    }

                    var ispic = false;


                    if (data.subChat[chat].AD_Image_ID == 0) {
                        str += "<img  data-uID='" + data.subChat[chat].AD_User_ID + "'  style=\"cursor:pointer;height: 48px;width: 48px;\" src= '" + VIS.Application.contextUrl + "Areas/VIS/Images/Home/userAvatar.png'/>";
                        ispic = true;
                    }
                    else {
                        for (var a in data.userimages) {
                            if (data.userimages[a].AD_Image_ID == data.subChat[chat].AD_Image_ID && data.userimages[a].UserImg != "NoRecordFound" && data.userimages[a].UserImg != "FileDoesn'tExist") {

                                str += '<img  data-uID="' + data.subChat[chat].AD_User_ID + '"  style="cursor:pointer;vertical-align: middle;" src="' + VIS.Application.contextUrl + data.userimages[a].UserImg + '" />';
                                ispic = true;
                                break;
                            }

                        }
                    }


                    if (ispic == false) {
                        str += "<img   data-uID='" + data.subChat[chat].AD_User_ID + "'  style=\"cursor:pointer;height: 48px;width: 48px;\" src= '" + VIS.Application.contextUrl + "Areas/VIS/Images/Home/userAvatar.png'/>";
                    }


                    str += '</div><div style="overflow-y;margin-bottom:5px"><div style="overflow:auto">';

                    if (VIS.Application.isRTL) {
                        str += '<span data-uID="' + data.subChat[chat].AD_User_ID + '"  style="float: right; color: #3892DA;;font-size: 10pt;margin-top:2px;cursor:pointer ">';
                    }
                    else {
                        str += '<span data-uID="' + data.subChat[chat].AD_User_ID + '"  style="float: left; color: #3892DA;;font-size: 10pt;margin-top:2px;cursor:pointer ">';
                    }

                    if (VIS.Env.getCtx().getAD_User_ID() == data.subChat[chat].AD_User_ID) {
                        str += "Me";
                    }
                    else {
                        str += data.subChat[chat].UserName;
                    }

                    if (VIS.Application.isRTL) {
                        str += '</span><span style="display: block; text-align: right;float:left; margin-left: 5px;margin-top:2px;font-size: 10pt;">' + date + '</span></div><div  style="overflow:auto">';
                    }
                    else {
                        str += '</span><span style="display: block; text-align: right;float:right; margin-right: 5px;margin-top:2px;font-size: 10pt;">' + date + '</span></div><div  style="overflow:auto">';
                    }

                          //+ '<textarea readonly style="width:640px">' + data[chat].ChatData + '</textarea>'
                    if (VIS.Application.isRTL) {
                        str += '<span style="width:526px;font-size: 10pt;margin-right:5px">' + VIS.Utility.encodeText(data.subChat[chat].ChatData) + '</span></div>'
                    }
                    else {
                        str += '<span style="width:526px;font-size: 10pt;">' + VIS.Utility.encodeText(data.subChat[chat].ChatData) + '</span></div>'
                    }
                    str += '</div>'
                    str += '            </div>  ';

                    htmll += str;
                }
                $container.html(htmll);
            });


            $container.on("click", function (e) {
                if ($(e.target).is("span") || $(e.target).is("img")) {
                    var uID = $(e.target).data("uid");
                    if (uID != undefined && uID != null && uID > 0) {
                        var contactInfo = new VIS.ContactInfo(uID, windowNo);
                        contactInfo.show();
                        ch.close();
                    }

                }
            });

        };



    };

    VIS.Chat = Chat;

})(VIS, jQuery);