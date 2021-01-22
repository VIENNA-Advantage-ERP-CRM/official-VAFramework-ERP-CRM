using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Helpers
{
    /// <summary>
    /// help to log in and to get login data
    /// </summary>
    public class LoginHelper
    {

        /**	Cache					*/
        private static CCache<string, object> cache = new CCache<string, object>("LoginHelper", 30, 60);
        /// <summary>
        /// return is credential provide by user is right or not
        /// </summary>
        /// <param name="model">login model class</param>
        /// <param name="roles">out roles , has role list of user</param>
        /// <param name="ctx" ></param>
        /// <returns>true if athenicated</returns>
        public static bool Login(LoginModel model, out List<KeyNamePair> roles)
        {
            // loginModel = null;
            //bool isMatch = false;
            roles = null;
            SecureEngine.Encrypt("t"); //Initialize 

            //	Cannot use encrypted password
            //if ())
            //{
            //    //log.warning("Cannot use Encrypted Password");
            //    return false;
            //}
            //	Authentification
            bool authenticated = false;
            bool isLDAP = false;
            MSystem system = MSystem.Get(new Ctx());
            string output = "";
            if (system != null && system.IsLDAP())
            {

                authenticated = system.IsLDAP(model.Login1Model.UserValue, model.Login1Model.Password, out output);

                isLDAP = true;
            }
            //Save Failed Login Count and Password validty in cache
            GetSysConfigForlogin();


            int fCount = Util.GetValueOfInt(cache[Common.Failed_Login_Count_Key]);
            int passwordValidUpto = Util.GetValueOfInt(cache[Common.Password_Valid_Upto_Key]);
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@username", model.Login1Model.UserValue);



            DataSet dsUserInfo = DB.ExecuteDataset("SELECT VAF_UserContact_ID, Value, Password,IsLoginUser,FailedLoginCount, IsOnlyLDAP FROM VAF_UserContact WHERE Value=@username", param);
            if (dsUserInfo != null && dsUserInfo.Tables[0].Rows.Count > 0)
            {
                // skipped Login user check for SuperUser (100)
                if (!cache["SuperUserVal"].Equals(model.Login1Model.UserValue)
                    && !dsUserInfo.Tables[0].Rows[0]["IsLoginUser"].ToString().Equals("Y"))
                {
                    throw new Exception("NotLoginUser");
                }

                // output length will be greater than 0 if there is any error while ldap auth. 
                //output check is applied to becuase after first login, when user redriect to home page, this functioexecutes again and password is null on that time.
                // so ldap reject auth , but user is actually authenticated. so to avoid error, this check is used.
                if (!cache["SuperUserVal"].Equals(model.Login1Model.UserValue) && dsUserInfo.Tables[0].Rows[0]["IsOnlyLDAP"].ToString().Equals("Y")
                    && isLDAP && !authenticated)
                {
                    throw new Exception(output);
                }
            }
            else
            {
                throw new Exception("UserNotFound");
            }

            //if authenticated by LDAP or password is null(Means request from home page)
            if (!authenticated && model.Login1Model.Password != null)
            {
                string sqlEnc = "SELECT isencrypted FROM vaf_column WHERE vaf_tableview_id=(SELECT vaf_tableview_id FROM vaf_tableview WHERE tablename='VAF_UserContact') AND columnname='Password'";
                char isEncrypted = Convert.ToChar(DB.ExecuteScalar(sqlEnc));
                string originalpwd = model.Login1Model.Password;
                if (isEncrypted == 'Y' && model.Login1Model.Password != null)
                {
                    model.Login1Model.Password = SecureEngine.Encrypt(model.Login1Model.Password);
                }

                //  DataSet dsUserInfo = DB.ExecuteDataset("SELECT VAF_UserContact_ID, Value, Password,IsLoginUser,FailedLoginCount FROM VAF_UserContact WHERE Value=@username", param);
                if (dsUserInfo != null && dsUserInfo.Tables[0].Rows.Count > 0)
                {
                    //if username or password is not matching
                    if ((!dsUserInfo.Tables[0].Rows[0]["Value"].Equals(model.Login1Model.UserValue) ||
                        !dsUserInfo.Tables[0].Rows[0]["Password"].Equals(model.Login1Model.Password))
                        || (originalpwd != null && SecureEngine.IsEncrypted(originalpwd)))
                    {
                        //if current user is Not superuser, then increase failed login count
                        if (!cache["SuperUserVal"].Equals(model.Login1Model.UserValue))
                        {
                            param[0] = new SqlParameter("@username", model.Login1Model.UserValue);
                            int count = DB.ExecuteQuery("UPDATE VAF_UserContact Set FAILEDLOGINCOUNT=FAILEDLOGINCOUNT+1 WHERE Value=@username ", param);

                            if (fCount > 0 && fCount <= Util.GetValueOfInt(dsUserInfo.Tables[0].Rows[0]["FailedLoginCount"]) + 1)
                            {
                                throw new Exception("MaxFailedLoginAttempts");
                            }
                        }

                        throw new Exception("UserPwdError");
                    }
                    else// if username and password matched, then check if account is locked or not
                    {
                        if (fCount > 0 && fCount <= Util.GetValueOfInt(dsUserInfo.Tables[0].Rows[0]["FailedLoginCount"]))
                        {
                            throw new Exception("MaxFailedLoginAttempts");
                        }
                    }
                }
            }

            IDataReader dr = GetRoles(model.Login1Model.UserValue, authenticated, isLDAP);

            if (!dr.Read())		//	no record found, then return msaage that role not found.
            {
                dr.Close();
                throw new Exception("RoleNotDefined");
            }

            // if user logged in successfully, then set failed login count to 0
            DB.ExecuteQuery("UPDATE VAF_UserContact SET FailedLoginCount=0 WHERE Value=@username", param);

            int VAF_UserContact_ID = Util.GetValueOfInt(dr[0].ToString()); //User Id

            if (!cache["SuperUserVal"].Equals(model.Login1Model.UserValue))
            {
                String Token2FAKey = Util.GetValueOfString(dr["TokenKey2FA"]);
                bool enable2FA = Util.GetValueOfString(dr["Is2FAEnabled"]) == "Y";
                if (enable2FA)
                {
                    model.Login1Model.QRFirstTime = false;
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    SetupCode setupInfo = null;
                    string userSKey = Util.GetValueOfString(dr["Value"]);
                    int ADUserID = Util.GetValueOfInt(dr["VAF_UserContact_ID"]);
                    // if token key don't exist for user, then create new
                    if (Token2FAKey.Trim() == "")
                    {
                        model.Login1Model.QRFirstTime = true;
                        Token2FAKey = userSKey;
                        // get Random Number
                        model.Login1Model.TokenKey2FA = GetRndNum();
                        // create Token key based on Value, UserID and Random Number
                        Token2FAKey = userSKey + ADUserID.ToString() + model.Login1Model.TokenKey2FA;
                    }
                    else
                    {
                        // Decrypt token key saved in database
                        string decKey = SecureEngine.Decrypt(Token2FAKey);
                        Token2FAKey = userSKey + ADUserID.ToString() + decKey;
                    }

                    string url = Util.GetValueOfString(HttpContext.Current.Request.Url.AbsoluteUri).Replace("VIS/Account/JsonLogin", "").Replace("https://", "").Replace("http://", "");

                    setupInfo = tfa.GenerateSetupCode("VA ", url + " " + userSKey, Token2FAKey, 150, 150);
                    model.Login1Model.QRCodeURL = setupInfo.QrCodeSetupImageUrl;
                }

                model.Login1Model.Is2FAEnabled = enable2FA;
            }


            if (!authenticated)
            {
                DateTime? pwdExpireDate = Util.GetValueOfDateTime(dr["PasswordExpireOn"]);
                if (pwdExpireDate == null || (passwordValidUpto > 0 && (DateTime.Compare(DateTime.Now, Convert.ToDateTime(pwdExpireDate)) > 0)))
                {
                    model.Login1Model.ResetPwd = true;
                    //if (SecureEngine.IsEncrypted(model.Login1Model.Password))
                    //    model.Login1Model.Password = SecureEngine.Decrypt(model.Login1Model.Password);
                }
            }

            roles = new List<KeyNamePair>(); //roles

            List<int> usersRoles = new List<int>();
            string username = "";

            do	//	read all roles
            {
                VAF_UserContact_ID = Util.GetValueOfInt(dr[0].ToString());
                int VAF_Role_ID = Util.GetValueOfInt(dr[1].ToString());

                String Name = dr[2].ToString();
                KeyNamePair p = new KeyNamePair(VAF_Role_ID, Name);
                username = Util.GetValueOfString(dr["username"].ToString());
                roles.Add(p);

                usersRoles.Add(VAF_Role_ID);
            }
            while (dr.Read());

            dr.Close();
            model.Login1Model.VAF_UserContact_ID = VAF_UserContact_ID;
            model.Login1Model.DisplayName = username;

            IDataReader drLogin = null;

            if (model.Login2Model == null)
            {

                try
                {
                    //* Change sub query into ineer join */

                    drLogin = DB.ExecuteReader(" SELECT l.VAF_Role_ID," +
                                               " (SELECT r.Name FROM VAF_ROLE r WHERE r.VAF_Role_ID=l.VAF_ROLE_ID) as RoleName," +

                                               " l.VAF_Org_ID," +
                                               " (SELECT o.Name FROM VAF_Org o WHERE o.VAF_Org_ID=l.VAF_Org_ID) as OrgName," +
                                               " l.VAF_Client_ID," +
                                               " (SELECT c.Name FROM VAF_Client c WHERE c.VAF_Client_ID=l.VAF_Client_ID) as ClientName," +
                                               " l.M_Warehouse_ID," +
                                               " (SELECT m.Name FROM M_Warehouse m WHERE m.M_Warehouse_Id = l.M_Warehouse_ID) as WarehouseName" +
                                               " FROM VAF_LoginSetting l WHERE l.IsActive = 'Y' AND l.VAF_UserContact_ID=" + VAF_UserContact_ID);
                    if (drLogin.Read())
                    {


                        bool deleteRecord = false;

                        //Delete Login Setting 
                        if (deleteRecord)
                        {
                            DB.ExecuteQuery("DELETE FROM VAF_LoginSetting WHERE VAF_UserContact_ID = " + VAF_UserContact_ID);
                        }
                        else
                        {
                            model.Login2Model = new Login2Model();
                            model.Login2Model.Role = drLogin[0].ToString();
                            model.Login2Model.RoleName = drLogin[1].ToString();
                            model.Login2Model.Org = drLogin[2].ToString();
                            model.Login2Model.OrgName = drLogin[3].ToString();
                            model.Login2Model.Client = drLogin[4].ToString();
                            model.Login2Model.ClientName = drLogin[5].ToString();
                            model.Login2Model.Warehouse = drLogin[6].ToString();
                            model.Login2Model.WarehouseName = drLogin[7].ToString();
                            model.Login2Model.Date = System.DateTime.Now.Date;
                        }
                    }
                    drLogin.Close();
                }
                catch
                {
                    if (drLogin != null)
                    {
                        drLogin.Close();
                    }
                }
            }
            return true;
        }
        public static IDataReader GetRoles(string uname, bool authenticated, bool isLDAP)
        {
            GetSysConfigForlogin();

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@username", uname);
            StringBuilder sql = new StringBuilder("SELECT u.VAF_UserContact_ID, r.VAF_Role_ID,r.Name,")
               // .Append(" u.ConnectionProfile, u.Password,u.FailedLoginCount,u.PasswordExpireOn, u.Is2FAEnabled, u.TokenKey2FA, u.Value ")	//	4,5
               .Append(" u.ConnectionProfile, u.Password, u.FailedLoginCount, u.PasswordExpireOn, u.Is2FAEnabled, u.TokenKey2FA, u.Value, u.Name as username, u.Created ")	//	4,5
               .Append("FROM VAF_UserContact u")
               .Append(" INNER JOIN VAF_UserContact_Roles ur ON (u.VAF_UserContact_ID=ur.VAF_UserContact_ID AND ur.IsActive='Y')")
               .Append(" INNER JOIN VAF_Role r ON (ur.VAF_Role_ID=r.VAF_Role_ID AND r.IsActive='Y') ");
            if (isLDAP && authenticated)
            {
                sql.Append(" WHERE (COALESCE(u.LDAPUser,u.Value)=@username)");
            }
            else
            {
                sql.Append(" WHERE (u.Value=@username)");
            }

            sql.Append(" AND u.IsActive='Y' ");
            // allowed SuperUser to login if it's not login user // no need to check Login user for SuperUser
            if (!cache["SuperUserVal"].Equals(uname))
                sql.Append(" AND u.IsLoginUser='Y' ");
            sql.Append(" AND EXISTS (SELECT * FROM VAF_Client c WHERE u.VAF_Client_ID=c.VAF_Client_ID AND c.IsActive='Y')")
            .Append(" AND EXISTS (SELECT * FROM VAF_Client c WHERE r.VAF_Client_ID=c.VAF_Client_ID AND c.IsActive='Y')");

            sql.Append(" ORDER BY r.Name");
            IDataReader dr = null;
            //	execute a query
            dr = DB.ExecuteReader(sql.ToString(), param);
            return dr;
        }

        private static void GetSysConfigForlogin()
        {
            //if nothing found in cache, then add
            if (cache.Count == 0)
            {
                //Set default Values
                cache[Common.Failed_Login_Count_Key] = Common.GetFailed_Login_Count;
                cache[Common.Password_Valid_Upto_Key] = Common.GetPassword_Valid_Upto;

                //then check setting in System Config, if found, then will replace default values.
                DataSet ds = DB.ExecuteDataset("SELECT Name, Value FROM VAF_SysConfig WHERE IsActive='Y' AND Name in ('FAILED_LOGIN_COUNT','PASSWORD_VALID_UPTO') ");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cache[ds.Tables[0].Rows[i]["Name"].ToString()] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    }
                }

                //Save SuperUser's key in cache
                cache["SuperUserVal"] = DB.ExecuteScalar("SELECT value from VAF_UserContact where VAF_UserContact_ID=100").ToString();

            }
        }

        public static string ValidatePassword(string oldPassword, string NewPassword, string ConfirmNewPasseword)
        {
            return Common.ValidatePassword(oldPassword, NewPassword, ConfirmNewPasseword);
        }

        public static bool UpdatePassword(string newPwd, int VAF_UserContact_ID)
        {
            int pwdValidity = Util.GetValueOfInt(cache[Common.Password_Valid_Upto_Key]);
            return Common.UpdatePasswordAndValidity(newPwd, VAF_UserContact_ID, VAF_UserContact_ID, pwdValidity, null);

        }

        /// <summary>
        /// Generate Random number of 16 characters and return
        /// </summary>
        /// <returns></returns>
        public static string GetRndNum()
        {
            Random RNG = new Random();
            StringBuilder builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// get client aginst role of user
        /// </summary>
        /// <param name="role">role id</param>
        /// <returns>client list </returns>
        public static List<KeyNamePair> GetClients(int role)
        {
            int VAF_Role_ID = role;

            List<KeyNamePair> list = new List<KeyNamePair>();
            //KeyNamePair[] retValue = null;
            String sql = "SELECT DISTINCT r.UserLevel, r.ConnectionProfile, "	//	1..2
                + " c.VAF_Client_ID,c.Name "								//	3..4 
                + "FROM VAF_Role r"
                + " INNER JOIN VAF_Client c ON (r.VAF_Client_ID=c.VAF_Client_ID) "
                + "WHERE r.VAF_Role_ID=@roleid"		//	#1
                + " AND r.IsActive='Y' AND c.IsActive='Y'";

            //	get Role details
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@roleid", VAF_Role_ID);

                dr = DB.ExecuteReader(sql, param);
                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                //list.Add(new KeyNamePair(-1, "Select"));

                do
                {
                    int VAF_Client_ID = Util.GetValueOfInt(dr[2].ToString());
                    String Name = dr[3].ToString();
                    KeyNamePair p = new KeyNamePair(VAF_Client_ID, Name);
                    list.Add(p);
                }
                while (dr.Read());
                dr.Close();
                //
                // retValue = new KeyNamePair[list.Count];
                // retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                //retValue = null;
            }
            return list;
        }

        /// <summary>
        /// return org access list aginst client and role of user
        /// </summary>
        /// <param name="VAF_Role_ID">role id </param>
        /// <param name="VAF_UserContact_ID">user id</param>
        /// <param name="VAF_Client_ID"> client id</param>
        /// <returns></returns>
        public static List<KeyNamePair> GetOrgs(int VAF_Role_ID, int VAF_UserContact_ID, int VAF_Client_ID)
        {
            List<KeyNamePair> list = new List<KeyNamePair>();

            String sql = "SELECT o.VAF_Org_ID,o.Name,o.IsSummary "	//	1..3
                + "FROM VAF_Role r, VAF_Client c"
                + " INNER JOIN VAF_Org o ON (c.VAF_Client_ID=o.VAF_Client_ID OR o.VAF_Org_ID=0) "
                + "WHERE r.VAF_Role_ID='" + VAF_Role_ID + "'" 	//	#1
                + " AND c.VAF_Client_ID='" + VAF_Client_ID + "'"	//	#2
                + " AND o.IsActive='Y' AND o.IsSummary='N' AND o.IsCostCenter='N' AND o.IsProfitCenter='N' "
                + " AND (r.IsAccessAllOrgs='Y' "
                    + "OR (r.IsUseUserOrgAccess='N' AND o.VAF_Org_ID IN (SELECT VAF_Org_ID FROM VAF_Role_OrgRights ra "
                        + "WHERE ra.VAF_Role_ID=r.VAF_Role_ID AND ra.IsActive='Y')) "
                    + "OR (r.IsUseUserOrgAccess='Y' AND o.VAF_Org_ID IN (SELECT VAF_Org_ID FROM VAF_UserContact_OrgRights ua "
                        + "WHERE ua.VAF_UserContact_ID='" + VAF_UserContact_ID + "' AND ua.IsActive='Y'))"		//	#3
                    + ") "
                + "ORDER BY o.Name";
            //
            MRole role = null;
            IDataReader dr = null;

            //list.Add(new KeyNamePair(-1, "Select"));
            try
            {

                dr = DB.ExecuteReader(sql);
                //  load Orgs
                Ctx ctx = new Ctx();
                while (dr.Read())
                {
                    int VAF_Org_ID = Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    bool summary = "Y".Equals(dr[2].ToString());
                    if (summary)
                    {
                        if (role == null)
                        {
                            ctx.SetVAF_Client_ID(VAF_Client_ID);
                            role = MRole.Get(ctx, VAF_Role_ID, VAF_UserContact_ID, false);
                        }
                        GetOrgsAddSummary(list, VAF_Org_ID, Name, role, ctx);
                    }
                    else
                    {
                        KeyNamePair p = new KeyNamePair(VAF_Org_ID, Name);
                        if (!list.Contains(p))
                            list.Add(p);
                    }
                }
                dr.Close();

                //
                //retValue = new KeyNamePair[list.Count];
                // retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            //	No Orgs
            return list;
        }   //  getOrgs

        public static void SetSysConfigInContext(Ctx ctx)
        {
            if (cache.Count > 0)
            {
                ctx.SetContext("#" + Common.Failed_Login_Count_Key, cache[Common.Failed_Login_Count_Key].ToString());
                ctx.SetContext("#" + Common.Password_Valid_Upto_Key, cache[Common.Password_Valid_Upto_Key].ToString());
            }
        }


        private static void GetOrgsAddSummary(List<KeyNamePair> list, int Summary_Org_ID, String Summary_Name, MRole role, Ctx ctx)
        {
            if (role == null)
            {
                return;
            }
            //	Do we look for trees?
            if (role.GetVAF_TreeInfo_Org_ID() == 0)
            {
                return;
            }
            //	Summary Org - Get Dependents
            MTree tree = MTree.Get(ctx, role.GetVAF_TreeInfo_Org_ID(), null);
            String sql = "SELECT VAF_Client_ID, VAF_Org_ID, Name, IsSummary FROM VAF_Org "
                + "WHERE IsActive='Y' AND VAF_Org_ID IN (SELECT Node_ID FROM "
                + tree.GetNodeTableName()
                + " WHERE VAF_TreeInfo_ID='" + tree.GetVAF_TreeInfo_ID() + "' AND Parent_ID='" + Summary_Org_ID + "' AND IsActive='Y') "
                + "ORDER BY Name";

            IDataReader dr = DB.ExecuteReader(sql);
            try
            {
                while (dr.Read())
                {
                    //	int VAF_Client_ID = rs.getInt(1);
                    int VAF_Org_ID = Util.GetValueOfInt(dr[1].ToString());
                    String Name = dr[2].ToString();
                    bool summary = "Y".Equals(dr[3].ToString());
                    //
                    if (summary)
                        GetOrgsAddSummary(list, VAF_Org_ID, Name, role, ctx);
                    else
                    {
                        KeyNamePair p = new KeyNamePair(VAF_Org_ID, Name);
                        if (!list.Contains(p))
                            list.Add(p);
                    }
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
        }	//	ge

        /// <summary>
        /// return warehouse of org
        /// </summary>
        /// <param name="org">org id</param>
        /// <returns>warehouse list</returns>
        public static List<KeyNamePair> GetWarehouse(int org)
        {
            List<KeyNamePair> list = new List<KeyNamePair>();

            String sql = "SELECT M_Warehouse_ID, Name FROM M_Warehouse "
                + "WHERE VAF_Org_ID=@p1 AND IsActive='Y' "
                + "ORDER BY Name";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@p1", org);
                dr = DB.ExecuteReader(sql, param);

                //list.Add(new KeyNamePair(-1, "Select"));

                if (!dr.Read())
                {
                    dr.Close();
                    return list;
                }

                //  load Warehouses
                do
                {
                    int AD_Warehouse_ID = Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Warehouse_ID, Name);
                    list.Add(p);
                }
                while (dr.Read());

                dr.Close();
                //
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            return list;
        }

        internal static LoginContext GetLoginContext(LoginModel model)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##VAF_UserContact_ID", model.Login1Model.VAF_UserContact_ID.ToString());
            ctx.SetContext("##VAF_UserContact_Value", model.Login1Model.UserValue);
            ctx.SetContext("##VAF_UserContact_Name", model.Login1Model.DisplayName);
            ctx.SetContext("#VAF_Language", model.Login1Model.LoginLanguage);

            ctx.SetContext("#VAF_Role_ID", model.Login2Model.Role);
            ctx.SetContext("#VAF_Role_Name", model.Login2Model.RoleName);

            ctx.SetContext("#VAF_Client_ID", model.Login2Model.Client);
            ctx.SetContext("#VAF_Client_Name", model.Login2Model.ClientName);

            ctx.SetContext("#VAF_Org_ID", model.Login2Model.Org);
            ctx.SetContext("#VAF_Org_Name", model.Login2Model.OrgName);

            ctx.SetContext("#M_Warehouse_ID", model.Login2Model.Warehouse);
            ctx.SetContext("#M_Warehouse_Name", model.Login2Model.WarehouseName);
            ctx.SetContext("#Date", model.Login2Model.Date.ToString());
            ctx.SetContext("#VAF_AlterLogBatch", "");


            //{
            //    ____VAF_UserContact_ID = model.Login1Model.VAF_UserContact_ID.ToString(),
            //    ____VAF_UserContact_Name = model.Login1Model.UserName,
            //    __VAF_Language = model.Login1Model.LoginLanguage,

            //    __VAF_Role_ID = model.Login2Model.Role,
            //    __VAF_Role_Name = model.Login2Model.RoleName,

            //    __VAF_Client_ID = model.Login2Model.Client,
            //    __VAF_Client_Name = model.Login2Model.ClientName,

            //    __VAF_Org_ID = model.Login2Model.Org,
            //    __VAF_Org_Name = model.Login2Model.OrgName,

            //    __AD_Warehouse_ID = model.Login2Model.Warehouse,
            //    __AD_Warehouse_Name = model.Login2Model.WarehouseName,
            //    __Date = model.Login2Model.Date

            //};
            return ctx;

        }

        internal static LoginContext GetLoginContext(LoginModel model, Ctx ctxLogIn)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##VAF_UserContact_ID", ctxLogIn.GetVAF_UserContact_ID().ToString());
            ctx.SetContext("##VAF_UserContact_Name", ctxLogIn.GetContext("##VAF_UserContact_Name"));


            ctx.SetContext("##VAF_UserContact_Value", ctxLogIn.GetContext("##VAF_UserContact_Value"));

            //ctx.SetContext("#VAF_Language", ctxLogIn.GetContext("#VAF_Language"));
            ctx.SetContext("#VAF_Language", model.Login2Model.LoginLanguage);

            ctx.SetContext("#VAF_Role_ID", model.Login2Model.Role);
            ctx.SetContext("#VAF_Role_Name", model.Login2Model.RoleName);

            ctx.SetContext("#VAF_Client_ID", model.Login2Model.Client);
            ctx.SetContext("#VAF_Client_Name", model.Login2Model.ClientName);

            ctx.SetContext("#VAF_Org_ID", model.Login2Model.Org);
            ctx.SetContext("#VAF_Org_Name", model.Login2Model.OrgName);

            ctx.SetContext("#M_Warehouse_ID", model.Login2Model.Warehouse);
            ctx.SetContext("#M_Warehouse_Name", model.Login2Model.WarehouseName);
            //ctx.SetContext("#Date", model.Login2Model.Date.ToString());
            return ctx;
        }


        internal static void SaveLoginSetting(LoginModel model)
        {
            try
            {
                int id = MSequence.GetNextID(Convert.ToInt32(model.Login2Model.Client), "VAF_LoginSetting", (Trx)null);
                if (id > 0)
                {
                    string sql = "INSERT INTO VAF_LoginSetting " +
                                 "(VAF_Client_ID,VAF_LoginSetting_ID,VAF_Org_ID,VAF_Role_ID,VAF_UserContact_ID,Created,CreatedBy,IsActive,M_WareHouse_ID,Updated,UpdatedBy) " +
                          " VALUES (" + model.Login2Model.Client + "," + id + "," + model.Login2Model.Org + "," + model.Login2Model.Role + "," + model.Login1Model.VAF_UserContact_ID + ",sysdate," + model.Login1Model.VAF_UserContact_ID + ",'Y',";
                    if (!String.IsNullOrEmpty(model.Login2Model.Warehouse) && model.Login2Model.Warehouse != "-1")
                    {
                        sql += model.Login2Model.Warehouse + ",";
                    }
                    else
                    {
                        sql += "null,";
                    }
                    sql += "sysdate," + model.Login1Model.VAF_UserContact_ID + ")";

                    DB.ExecuteQuery(sql);
                }
            }
            catch
            {
            }



        }

        /// <summary>`
        /// Validate OTP from Google Authenticator
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true/false</returns>
        public static bool Validate2FAOTP(LoginModel model)
        {
            bool isValid = false;
            DataSet dsUser = DB.ExecuteDataset(@"SELECT Value, TokenKey2FA, Created, Is2FAEnabled, VAF_UserContact_ID FROM VAF_UserContact WHERE VAF_UserContact_ID = " + model.Login1Model.VAF_UserContact_ID);
            if (dsUser != null && dsUser.Tables[0].Rows.Count > 0)
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                string Token2FAKey = Util.GetValueOfString(dsUser.Tables[0].Rows[0]["Value"]);
                int ADUserID = Util.GetValueOfInt(dsUser.Tables[0].Rows[0]["VAF_UserContact_ID"]);
                if (model.Login1Model.TokenKey2FA != null && model.Login1Model.TokenKey2FA != "")
                    Token2FAKey = Token2FAKey.ToString() + ADUserID.ToString() + model.Login1Model.TokenKey2FA;
                else if (Util.GetValueOfString(dsUser.Tables[0].Rows[0]["TokenKey2FA"]) != "")
                {
                    string decKey = Util.GetValueOfString(dsUser.Tables[0].Rows[0]["TokenKey2FA"]);
                    decKey = SecureEngine.Decrypt(decKey);
                    Token2FAKey = Token2FAKey.ToString() + ADUserID.ToString() + decKey;
                }

                isValid = tfa.ValidateTwoFactorPIN(Token2FAKey, model.Login1Model.OTP2FA);
                if (isValid && Util.GetValueOfString(dsUser.Tables[0].Rows[0]["TokenKey2FA"]).Trim() == "")
                {
                    string encKey = SecureEngine.Encrypt(model.Login1Model.TokenKey2FA);
                    int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VAF_USERCONTACT SET TokenKey2FA = '" + encKey + @"' WHERE 
                                    VAF_USERCONTACT_ID = " + model.Login1Model.VAF_UserContact_ID));
                }
            }
            return isValid;
        }
    }
}