using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Login;


namespace MarketSvc.InstallModule
{
    /// <summary>
    /// Summary description for Context
    /// </summary>
    public class ContextBuilder
    {
        public ContextBuilder()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Set context values in Env
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_pwd"></param>
        /// <param name="role"></param>
        /// <param name="client"></param>
        /// <param name="org"></param>
        /// <param name="ware"></param>
        /// <param name="dtTime"></param>
        public void CreateContext(string _user, string _pwd, KeyNamePair role, KeyNamePair client, KeyNamePair org, KeyNamePair ware, DateTime dtTime)
        {
            Ctx _ctx = VAdvantage.Utility.Env.GetCtx();
            //_connectionOK = TryConnection();

            if (TryConnection(_ctx, _user, _pwd))
            {
                Language l = VAdvantage.Login.Language.GetLoginLanguage();
                l = VAdvantage.Utility.Env.VerifyLanguage(_ctx, l);
                _ctx.SetContext(VAdvantage.Utility.Env.LANGUAGE, l.GetAD_Language());
                _ctx.SetContext("AD_User_Name", _user);

                Msg.GetMsg(_ctx, "0");

                //	Set Defaults
                MUser user = MUser.Get(_ctx);
                MUserPreference preference = user.GetPreference();

                Ini.SetProperty(Ini.P_UID, _user);
                Ini.SetProperty(Ini.P_PWD, _pwd);
                LoginProcess _login = new LoginProcess(_ctx);

                String printText = "print"; // "cmbPrinter.Text

                _login.GetClients(role);
                _login.GetOrgs(client);

                _login.LoadPreferences(org, ware, dtTime, printText);




            }

        }
        /// <summary>
        /// 	Try to connect.
        /// </summary>
        /// <returns></returns>
        private bool TryConnection(Ctx _ctx, string _user, string _pwd)
        {
            LoginProcess _login = new LoginProcess(_ctx);

            KeyNamePair[] roles = _login.GetRoles(_user, _pwd);

            if (roles == null || roles.Length == 0)
            {
                return false;
            }

            String iniDefault = Ini.GetProperty(Ini.P_ROLE);

            return true;
        }

        public void CreateContextFromID(int AD_User_ID)
        {
            //Get User Info//
            DataSet ds = DB.ExecuteDataset(" SELECT ad_user.Name, ad_user.password, ad_user.ad_client_id," +
               " (SELECT name from ad_client where ad_client.ad_client_id= ad_user.ad_client_id) as clientname," +
                " ad_user.ad_org_id,  (SELECT name from ad_client where ad_client.ad_client_id= ad_user.ad_client_id) as orgname," +
                    " ad_role.ad_role_id, ad_role.name as rolename FROM AD_User  Inner join ad_role" +
                    " on (ad_role.ad_client_id= ad_user.ad_client_id and ad_role.ad_org_id= ad_user.ad_org_id)" +
                     " WHERE AD_User_ID=" + AD_User_ID + " and ad_user.IsActive='Y'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                string user = ds.Tables[0].Rows[0][0].ToString();
                string password = ds.Tables[0].Rows[0][1].ToString();
                KeyNamePair client = new KeyNamePair(ds.Tables[0].Rows[0][2], ds.Tables[0].Rows[0][3].ToString());
                KeyNamePair org = new KeyNamePair(ds.Tables[0].Rows[0][4], ds.Tables[0].Rows[0][5].ToString());
                KeyNamePair role = new KeyNamePair(ds.Tables[0].Rows[0][6], ds.Tables[0].Rows[0][7].ToString());
                CreateContext(user, password, role, client, org, new KeyNamePair(), DateTime.Now);

            }
        }
    }
}