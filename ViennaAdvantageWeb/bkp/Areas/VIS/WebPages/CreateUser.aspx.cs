using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Areas.VIS.WebPages
{
    public partial class CreateUser : System.Web.UI.Page
    {
        string usernotSaved = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpRequest q = Request;
            string lang = q.QueryString["lang"];
            lblEmail.InnerText =VAdvantage.Utility.Util.CleanMnemonic( Msg.GetMsg(lang, "EMail"));
            lblHeader.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_LoginInfo"));
            lblMobile.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "Mobile"));
            lblName.InnerText =VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "Name"));
            lblPwd.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "Password"));
            confirmpasswordlbl.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "NewPasswordConfirm"));
            lblSubHeader.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_subHeader"));
            lblUID.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_UserID"));
            lblurl.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_clickUrl"));
            lblContent.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_LoginPageContent"));
            Button1.Text = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "Save"));
            sendMail.InnerText = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_UserSaved"));
            usernotSaved = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(lang, "VIS_ErrorSavingUser"));
            if (!IsPostBack)
            {
                sendMail.Visible = false;
                
                string mailID = q.QueryString["mailID"];
                string url = q.QueryString["URL"];
                if (mailID != "0")
                {
                    email.Value = SecureEngine.Decrypt(mailID);
                }
                parentUrl.InnerText = url;
                parentUrl.HRef = url;



         
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(Name.Value))
            {
                return;
            }
            HttpRequest q = Request;
            string lang = q.QueryString["lang"];
            usernotSaved = Msg.GetMsg(lang, "VIS_ErrorSavingUser");
            Button1.Enabled = false;
            int AD_Client_ID = 0;
            int AD_Org_ID = 0;

            int inviteID = Convert.ToInt32(SecureEngine.Decrypt(q.QueryString["inviteID"]));
            String sql = "SELECT * FROM AD_InviteUser WHERE AD_InviteUser_ID=" + inviteID;
            DataSet dsIUser = DB.ExecuteDataset(sql);
            if (dsIUser != null && dsIUser.Tables[0].Rows.Count > 0)
            {
                AD_Org_ID= Convert.ToInt32(dsIUser.Tables[0].Rows[0]["AD_Org_ID"]);
                AD_Client_ID = Convert.ToInt32(dsIUser.Tables[0].Rows[0]["AD_Client_ID"]);
            }

            sql = "SELECT AD_Role_ID FROM ad_inviteuser_role WHERE AD_InviteUser_ID= " + inviteID;
            DataSet ds = DB.ExecuteDataset(sql);



            Ctx ctx = new Ctx();
            ctx.SetAD_Client_ID(AD_Client_ID);
            ctx.SetAD_Org_ID(AD_Org_ID);


            MUser user = new MUser(ctx, 0, null);
            user.SetAD_Client_ID(AD_Client_ID);
            user.SetAD_Org_ID(AD_Org_ID);
            user.SetIsLoginUser(true);
            user.SetName(Name.Value);
            user.SetValue(userIDs.Value);
            user.SetEMail(email.Value);
            user.SetPassword(passwords.Value);
            user.SetMobile(mobile.Value);
            if (user.Save())
            {

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        MUserRoles uRoles = new MUserRoles(ctx, user.GetAD_User_ID(), Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Role_ID"]), null);
                        uRoles.SetAD_Client_ID(AD_Client_ID);
                        uRoles.SetAD_Org_ID(AD_Org_ID);
                        uRoles.Save();
                    }
                }

                sendMail.Visible = true;
                Name.Value = "";
                userIDs.Value = "";
                email.Value = "";
                passwords.Value = "";
                mobile.Value = "";
            }
            else
            {
                Button1.Enabled = true;
                sendMail.InnerText = usernotSaved;
                sendMail.Visible = true;
            }
        }
    }
}