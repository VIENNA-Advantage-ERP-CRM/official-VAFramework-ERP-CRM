//(function (VIS, $) {

//    VIS.importcontacts = function (self) {


//        var userName;
//        var password;
//        var role;
//        var chkUpdateExisting;
//        getsaveddetail();
//        //****************************
//        //Get Saved detail of gmail credentials and open Gmail Contact form
//        //****************************
//        function getsaveddetail() {
//            var url = VIS.Application.contextUrl + "GmailContacts/GetSavedDetail";
//            $.ajax({
//                type: "GET",
//                async: false,
//                url: url,
//                dataType: "json",

//                success: function (data) {

//                    var dd = JSON.parse(data);
//                    userName = dd["Username"];
//                    password = dd["Password"];
//                    role = dd["Role"];
//                    chkUpdateExisting = dd["IsUpdate"];
//                    if (dd == null || dd.length == 0 || userName == "" || password == "") {
//                        VIS.contactSettings(self);
//                    }
//                    else {
//                        c = new VIS.CFrame();
//                        var gmailContactForm = new VIS.gmailcontacts(userName, password, role, Boolean(chkUpdateExisting), c.windowNo);

//                        c.setName(VIS.Msg.getMsg("MyGmailContacts"));
//                        c.setTitle(VIS.Msg.getMsg("MyGmailContacts"));
//                        c.hideHeader(true);
//                        c.setContent(gmailContactForm);
//                        c.show();
//                        gmailContactForm.loadSavedData();
//                    }

//                }
//            });
//        };
//    };


//    VIS.importcontacts.prototype.dispose = function (frame) {
//        /*CleanUp Code */
//        //dispose this component


//        //call frame dispose function
//        if (frame != null)
//            frame.dispose();
//        frame = null;
//    };

//})(VIS, jQuery);