/********************************************************
 * Module Name    : VIS
 * Purpose        : Assign roles to user and windows, forms, processes to Roles
 * Chronological Development
 * Karan          3-June-2015
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.MailBox;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;
using VIS.DBase;

namespace VIS.Models
{
    public class CreateUser
    {
        Ctx ctx = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        public CreateUser(Ctx ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Save New User and organization accesses provided to it.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Email"></param>
        /// <param name="Value"></param>
        /// <param name="password"></param>
        /// <param name="mobile"></param>
        /// <param name="OrgID"></param>
        /// <returns></returns>
        public String SaveNewUser(string Name, string Email, string Value, string password, string mobile, List<int> OrgID)
        {
            string msg;
            string info= "";
            MUser user = new MUser(ctx, 0, null);
            user.SetName(Name);
            user.SetIsLoginUser(true);
            user.SetEMail(Email);

            if (!string.IsNullOrEmpty(password))
            {
                string key = ctx.GetSecureKey();
                password = SecureEngineBridge.DecryptByClientKey(password, key);
                user.SetPassword(password);
            }

            if (!String.IsNullOrEmpty(mobile))
            {
                user.SetMobile(mobile);
            }

            if (!string.IsNullOrEmpty(Value))
            {
                user.SetValue(Value);
            }
            if (user.Save())
            {
                if (OrgID != null)
                {
                    for (int i = 0; i < OrgID.Count; i++)
                    {
                        MOrg org = new MOrg(ctx, OrgID[i], null);
                        MUserOrgAccess roles = new MUserOrgAccess(org, user.GetAD_User_ID());
                        roles.SetAD_Client_ID(ctx.GetAD_Client_ID());
                        roles.SetAD_Org_ID(OrgID[i]);
                        roles.SetIsReadOnly(false);
                        roles.Save();
                    }
                }

               
            }
            else
            {
                ValueNamePair ppE = VAdvantage.Logging.VLogger.RetrieveError();

                if (ppE != null)
                {
                    msg = ppE.GetValue();
                    info = ppE.GetName();
                }
            }

                

                return  info;

        }


        /// <summary>
        /// Get All Orgs assigned to login User , Role and client.
        /// </summary>
        /// <returns></returns>
        public List<OrgAccessinfo> GetOrgAccess()
        {
            List<OrgAccessinfo> orgaccess = new List<OrgAccessinfo>();


            VAdvantage.Model.MRole.OrgAccess[] accesss = MRole.GetDefault(ctx).GetOrgAccess();
            StringBuilder orgs = new StringBuilder();
            if (accesss.Length > 0)
            {
                for (int i = 0; i < accesss.Length; i++)
                {
                    if (i > 0)
                    {
                        orgs.Append(",");
                    }
                    orgs.Append(accesss[i].AD_Org_ID);
                }
            }

            String sql = "SELECT o.AD_Org_ID,o.Name,o.Description   "	//	1..3
                + "FROM AD_Role r, AD_Client c"
                + " INNER JOIN AD_Org o ON (c.AD_Client_ID=o.AD_Client_ID OR o.AD_Org_ID=0) "
                + "WHERE r.AD_Role_ID='" + ctx.GetAD_Role_ID() + "'" 	//	#1
                + " AND c.AD_Client_ID='" + ctx.GetAD_Client_ID() + "'"	//	#2
                + " AND o.IsActive='Y' AND o.IsSummary='N'"
                + " AND (r.IsAccessAllOrgs='Y' "
                    + "OR (r.IsUseUserOrgAccess='N' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_Role_OrgAccess ra "
                        + "WHERE ra.AD_Role_ID=r.AD_Role_ID AND ra.IsActive='Y')) "
                    + "OR (r.IsUseUserOrgAccess='Y' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_User_OrgAccess ua "
                        + "WHERE ua.AD_User_ID='" + ctx.GetAD_User_ID() + "' AND ua.IsActive='Y'))"		//	#3
                    + ") "
                + "ORDER BY o.Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    OrgAccessinfo access = new OrgAccessinfo()
                    {
                        OrgName = ds.Tables[0].Rows[i]["Name"].ToString(),
                        Description = ds.Tables[0].Rows[i]["Description"].ToString(),
                        OrgID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"])
                    };
                    orgaccess.Add(access);
                }
            }
            return orgaccess;
        }


        /// <summary>
        /// Invite New Users by sending mail.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public string InviteUsers(string email, List<RolesInfo> infos)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }

            EMail objMail = new EMail(ctx, "", "", "", "", "", "", true, false);
            string isConfigExist = objMail.IsConfigurationExist(ctx);
            if (isConfigExist != "OK")
            {
                return isConfigExist;
            }




            X_AD_InviteUser iuser = new X_AD_InviteUser(ctx, 0, null);
            if (iuser.Save())
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    X_AD_InviteUser_Role userRole = new X_AD_InviteUser_Role(ctx, 0, null);
                    userRole.SetAD_InviteUser_ID(iuser.GetAD_InviteUser_ID());
                    userRole.SetAD_Role_ID(infos[i].AD_Role_ID);
                    userRole.Save();
                }
            }
            else
            {
                return Msg.GetMsg(ctx, "VIS_InviteUsernotSaved");
            }

            var emails = email.Split(';');

            string url = (HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.Url.AbsolutePath).Substring(0, (HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.Url.AbsolutePath).LastIndexOf("/"));
            string hostUrl = url.Substring(0, url.LastIndexOf("/"));

            if (hostUrl.IndexOf("http") == -1)
            {
                hostUrl = HttpContext.Current.Request.Url.Scheme + "://" + hostUrl;
            }
            if (HttpContext.Current.Request.Url.Port > 0 && HttpContext.Current.Request.Url.Port != 80)
            {
                url = url.Substring(0, url.LastIndexOf("/")) + ":" + HttpContext.Current.Request.Url.Port.ToString() + "/Areas/VIS/WebPages/CreateUser.aspx";
                hostUrl += ":" + HttpContext.Current.Request.Url.Port.ToString();
            }
            else
            {
                url = url.Substring(0, url.LastIndexOf("/")) + "/Areas/VIS/WebPages/CreateUser.aspx";
            }
            string queryString = "?inviteID=" + SecureEngine.Encrypt(iuser.GetAD_InviteUser_ID().ToString()) + "&URL=" + hostUrl + "&lang=" + ctx.GetAD_Language();
            if (emails.Length == 1)
            {
                queryString += "&mailID=" + SecureEngine.Encrypt(emails[0].ToString());
            }
            else
            {
                queryString += "&mailID=0";
            }

            objMail.SetSubject(Msg.GetMsg(ctx, "VIS_CreateUser"));
            //<label >Hello</label><br>" +
            //                    "<label >Please Click to create user with vienna Advantage</label>>

            string html = " <html><body> " + Msg.GetMsg(ctx, "VIS_InviteMailMessage") + "   <br>" +
                    "<a href='http://" + url + queryString + "'>click here </a> </body></html>  ";

            objMail.SetMessageHTML(html);

            for (int i = 0; i < emails.Count(); i++)
            {
                objMail.AddTo(emails[i], "");
            }

            string res1 = objMail.Send();
            StringBuilder res = new StringBuilder();
            if (res1 != "OK")           // if mail not sent....
            {
                if (res1 == "AuthenticationFailed.")
                {
                    res.Append("AuthenticationFailed");
                    return res.ToString();
                }
                else if (res1 == "ConfigurationIncompleteOrNotFound")
                {
                    res.Append("ConfigurationIncompleteOrNotFound");
                    return res.ToString();
                }
                else
                {
                    res.Append(" " + Msg.GetMsg(ctx, "MailNotSentTo") + ": " + email);
                }
            }
            else
            {
                {
                    if (!res.ToString().Contains("MailSent"))
                    {
                        res.Append("MailSent");
                    }
                }

            }

            return res.ToString();

        }

    }

    public class OrgAccessinfo
    {
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public string Description { get; set; }
    }
}