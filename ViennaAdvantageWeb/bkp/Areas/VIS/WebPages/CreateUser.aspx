<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="VIS.Areas.VIS.WebPages.CreateUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" rel="stylesheet" href="../Content/GroupStyle.css" />
    <link type="text/css" rel="stylesheet" href="../Content/VIS.all.min.css" />
    <meta name="description" content="ERP ">
    <meta name="author" content="Vienna">
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />

    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimum-scale=1.0, maximum-scale=1.0">
</head>
<body>
    <form id="form1" runat="server">
        <%--        <div>
            <label style="font-size:xx-large">Enter Information </label>
            <div style="margin-left: 120px" class="vis-group-assinRole-content">
                <div class="vis-group-assinRole-data">
                    <label>EMail  </label>
                    <input runat="server"  style="width: 300px; display: block;" class="vis-group-email" type="text" id="email" />
                </div>
                <div class="vis-group-assignContainer">
                    <div class="vis-group-assinRole-data">
                        <label>Name  </label>
                        <input runat="server"  style="width: 300px; display: block;" type="text" class="vis-group-name vis-group-mandatory" id="Name" />
                    </div>


                    <div class="vis-group-assinRole-data">
                        <label>User ID  </label>
                        <input runat="server"  style="width: 300px; display: block;" type="text" class="vis-group-uid" id="userIDs" />
                    </div>

                    <div class="vis-group-assinRole-data">
                        <label>Password  </label>
                        <input runat="server"  style="width: 300px; display: block;" type="password" class="vis-group-pwd" id="passwords" />
                    </div>

                    <div class="vis-group-assinRole-data">
                        <label>Mobile </label>
                        <input  runat="server"  style="width: 300px; display: block;" type="text" class="vis-group-mobile" id="mobile" />
                    </div>
                </div>
               
                <asp:Button  class="vis-group-btn vis-group-Save vis-group-grayBtn"  ID="Button1" runat="server"  OnClick="Button1_Click" OnClientClick="return checkName()" Text="Save" />

                <label runat="server" id="sendMail" >User Saved </label>

            </div>
        </div>--%>

        <div class="vis-group-main-wrap">
            <div class="vis-group-header">
                <img src="../Images/V-logo.png" alt="logo" />
                <h2 runat="server" id="lblHeader">Enter your login information</h2>
            </div>
            <div class="vis-group-content-wrap">
                <div class="vis-group-left-wrap">
                    <img src="../Images/group-tab-mob.png" alt="" />
                    <p runat="server" id="lblurl">After save check here: </p>
                    <a runat="server" id="parentUrl"></a>
                </div>
                <div class="vis-group-form-wrap">
                    <div class="vis-group-header-wrap">
                        <h3 class="vis-group-tittle" runat="server" id="lblSubHeader">Enter Information</h3>
                        <div class="vis-group-clear-both"></div>
                    </div>
                    <!-- end of header -->
                    <div class="vis-group-form-content">
                        <p runat="server" id="lblContent"></p>
                        <div class="vis-group-form-data">
                            <label runat="server" id="lblEmail">Email</label>
                            <sup>*</sup>
                            <input runat="server" id="email" type="text" placeholder="example@email.com" />
                        </div>

                        <div class="vis-group-form-data">
                            <label runat="server" id="lblName">Name</label>
                            <sup>*</sup>
                            <input runat="server" type="text" id="Name" />
                        </div>

                        <div class="vis-group-form-data">
                            <label runat="server" id="lblUID">User ID</label>
                            <input runat="server" type="text" id="userIDs" />
                        </div>

                        <div class="vis-group-form-data">
                            <label runat="server" id="lblPwd">Password</label>
                            <input runat="server" type="password" id="passwords" />
                            <%--<asp:TextBox runat="server"  id="passwordss" Width="100%"></asp:TextBox>--%>
                        </div>

                        <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="passwordss" ControlToCompare="confirmpassword"  ErrorMessage="Password not macthing"></asp:CompareValidator>--%>

                        <div class="vis-group-form-data">
                            <label runat="server" id="confirmpasswordlbl">Password</label>
                            <input runat="server" type="password" id="confirmpassword" />
                            <%--<asp:TextBox runat="server"  id="confirmpassword" Width="100%"></asp:TextBox>--%>
                        </div>





                        <div class="vis-group-form-data">
                            <label runat="server" id="lblMobile">Mobile</label>
                            <input runat="server" type="text" id="mobile" />
                        </div>

                        <asp:Button class="vis-group-btn vis-group-Save vis-group-grayBtn" ID="Button1" runat="server" OnClick="Button1_Click" OnClientClick="return checkName()" />
                        <label runat="server" id="sendMail"></label>

                    </div>
                    <!-- end of form-content -->

                </div>
                <!-- end of form-wrap -->
            </div>

        </div>

    </form>

    <script>

        function checkName() {
            var nametxt = document.getElementById("Name").value;
            if (nametxt.length == 0) {
                alert("<%= VAdvantage.Utility.Msg.GetMsg(((HttpRequest)Request)["lang"],"VIS_PleaseName") %>");
                return false;
            }

            var password = document.getElementById("passwords").value;
            var confirmPassword = document.getElementById("confirmpassword").value;

            if (password !== confirmPassword) {
                alert("<%= VAdvantage.Utility.Msg.GetMsg(((HttpRequest)Request)["lang"],"PasswordNotMatching") %>");
                return false;
            }

            var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);

            var emailtxt = document.getElementById("email").value;
            if (emailtxt.length > 0) {
                var isValidEmail = pattern.test(document.getElementById("email").value);
                if (!isValidEmail) {
                    alert("<%= VAdvantage.Utility.Msg.GetMsg(((HttpRequest)Request)["lang"],"NotValidEmail") %>" + ':' + document.getElementById("email").value);
                    return false;
                }
            }
            else {
                alert("<%= VAdvantage.Utility.Msg.GetMsg(((HttpRequest)Request)["lang"],"NotValidEmail") %>");
                return false;
            }

            //if ($('.vis-group-name') != undefined && $('.vis-group-name') != null) {
            //    var name = $('.vis-group-name').val();
            //    if (name.length == 0) {
            //        alert("Please Enter Name");
            //        return false;
            //    }
            //}
        };
        //$('.vis-group-Save').on("click", function ()
        //{


        //});


    </script>

</body>

</html>
