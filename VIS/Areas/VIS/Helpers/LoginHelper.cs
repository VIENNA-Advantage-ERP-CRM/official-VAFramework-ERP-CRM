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



            DataSet dsUserInfo = DB.ExecuteDataset("SELECT AD_User_ID, Value, Password,IsLoginUser,FailedLoginCount, IsOnlyLDAP FROM AD_User WHERE Value=@username", param);
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
                string sqlEnc = "SELECT isencrypted FROM ad_column WHERE ad_table_id=(SELECT ad_table_id FROM ad_table WHERE tablename='AD_User') AND columnname='Password'";
                char isEncrypted = Convert.ToChar(DB.ExecuteScalar(sqlEnc));
                string originalpwd = model.Login1Model.Password;
                if (isEncrypted == 'Y' && model.Login1Model.Password != null)
                {
                    model.Login1Model.Password = SecureEngine.Encrypt(model.Login1Model.Password);
                }

                //  DataSet dsUserInfo = DB.ExecuteDataset("SELECT AD_User_ID, Value, Password,IsLoginUser,FailedLoginCount FROM AD_User WHERE Value=@username", param);
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
                            int count = DB.ExecuteQuery("UPDATE AD_User Set FAILEDLOGINCOUNT=FAILEDLOGINCOUNT+1 WHERE Value=@username ", param);

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
            DB.ExecuteQuery("UPDATE AD_User SET FailedLoginCount=0 WHERE Value=@username", param);

            int AD_User_ID = Util.GetValueOfInt(dr[0].ToString()); //User Id

            if (!cache["SuperUserVal"].Equals(model.Login1Model.UserValue))
            {
                String Token2FAKey = Util.GetValueOfString(dr["TokenKey2FA"]);
                // VIS0008 Change done to handle VA 2FA alongwith Google Authenticator
                string method2FA = Util.GetValueOfString(dr["TwoFAMethod"]);
                if (method2FA != "")
                {
                    model.Login1Model.QRFirstTime = false;
                    model.Login1Model.NoLoginSet = false;
                    string userSKey = Util.GetValueOfString(dr["Value"]);
                    int ADUserID = Util.GetValueOfInt(dr["AD_User_ID"]);
                    if (method2FA == X_AD_User.TWOFAMETHOD_GoogleAuthenticator)
                    {
                        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                        SetupCode setupInfo = null;
                        // if token key don't exist for user, then create new
                        if (Token2FAKey.Trim() == "")
                        {
                            model.Login1Model.QRFirstTime = true;
                            Token2FAKey = userSKey;
                            // get Random Number
                            model.Login1Model.TokenKey2FA = GetRndNum(16);
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
                    else if (method2FA == X_AD_User.TWOFAMETHOD_VAMobileApp)
                    {
                        if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VA074_MobileLinked_ID) FROM VA074_MobileLinked 
                            WHERE VA074_AD_User_ID = " + ADUserID + " AND VA074_PushNotiToken IS NOT NULL AND IsActive = 'Y'")) > 0)
                        {
                            SendPushNotToken(ADUserID, userSKey);
                        }
                        else
                        {
                            model.Login1Model.NoLoginSet = true;
                        }
                    }
                }

                model.Login1Model.TwoFAMethod = method2FA;
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
                AD_User_ID = Util.GetValueOfInt(dr[0].ToString());
                int AD_Role_ID = Util.GetValueOfInt(dr[1].ToString());

                String Name = dr[2].ToString();
                KeyNamePair p = new KeyNamePair(AD_Role_ID, Name);
                username = Util.GetValueOfString(dr["username"].ToString());
                roles.Add(p);

                usersRoles.Add(AD_Role_ID);
            }
            while (dr.Read());

            dr.Close();
            model.Login1Model.AD_User_ID = AD_User_ID;
            model.Login1Model.DisplayName = username;

            IDataReader drLogin = null;

            if (model.Login2Model == null)
            {

                try
                {
                    //* Change sub query into ineer join */

                    drLogin = DB.ExecuteReader(" SELECT l.AD_Role_ID," +
                                               " (SELECT r.Name FROM AD_ROLE r WHERE r.AD_Role_ID=l.AD_ROLE_ID) as RoleName," +

                                               " l.AD_Org_ID," +
                                               " (SELECT o.Name FROM AD_Org o WHERE o.AD_Org_ID=l.AD_Org_ID) as OrgName," +
                                               " l.AD_Client_ID," +
                                               " (SELECT c.Name FROM AD_Client c WHERE c.AD_Client_ID=l.AD_Client_ID) as ClientName," +
                                               " l.M_Warehouse_ID," +
                                               " (SELECT m.Name FROM M_Warehouse m WHERE m.M_Warehouse_Id = l.M_Warehouse_ID) as WarehouseName" +
                                               " FROM AD_LoginSetting l WHERE l.IsActive = 'Y' AND l.AD_User_ID=" + AD_User_ID);
                    if (drLogin.Read())
                    {


                        bool deleteRecord = false;

                        //Delete Login Setting 
                        if (deleteRecord)
                        {
                            DB.ExecuteQuery("DELETE FROM AD_LoginSetting WHERE AD_User_ID = " + AD_User_ID);
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
            StringBuilder sql = new StringBuilder("SELECT u.AD_User_ID, r.AD_Role_ID,r.Name,")
               // .Append(" u.ConnectionProfile, u.Password,u.FailedLoginCount,u.PasswordExpireOn, u.Is2FAEnabled, u.TokenKey2FA, u.Value ")	//	4,5
               .Append(" u.ConnectionProfile, u.Password, u.FailedLoginCount, u.PasswordExpireOn, u.Is2FAEnabled, u.TokenKey2FA, u.TwoFAMethod, u.Value, u.Name as username, u.Created ")	//	4,5
               .Append("FROM AD_User u")
               .Append(" INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive='Y')")
               .Append(" INNER JOIN AD_Role r ON (ur.AD_Role_ID=r.AD_Role_ID AND r.IsActive='Y') ");
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
            sql.Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE u.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')")
            .Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE r.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')");

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
                DataSet ds = DB.ExecuteDataset("SELECT Name, Value FROM AD_SysConfig WHERE IsActive='Y' AND Name in ('FAILED_LOGIN_COUNT','PASSWORD_VALID_UPTO') ");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cache[ds.Tables[0].Rows[i]["Name"].ToString()] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    }
                }

                //Save SuperUser's key in cache
                cache["SuperUserVal"] = DB.ExecuteScalar("SELECT value from AD_User where AD_User_ID=100").ToString();

            }
        }

        public static string ValidatePassword(string oldPassword, string NewPassword, string ConfirmNewPasseword)
        {
            return Common.ValidatePassword(oldPassword, NewPassword, ConfirmNewPasseword);
        }

        public static bool UpdatePassword(string newPwd, int AD_User_ID)
        {
            int pwdValidity = Util.GetValueOfInt(cache[Common.Password_Valid_Upto_Key]);
            return Common.UpdatePasswordAndValidity(newPwd, AD_User_ID, AD_User_ID, pwdValidity, null);

        }

        // VIS0008 Enhance function to return value according to paramenter passed
        /// <summary>
        /// Generate Random number of 16 characters and return
        /// </summary>
        /// <param name="digits">int value for number of characters</param>
        /// <returns>string value Random Number</returns>
        public static string GetRndNum(int digits)
        {
            Random RNG = new Random();
            StringBuilder builder = new StringBuilder();
            while (builder.Length < digits)
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
            int AD_Role_ID = role;

            List<KeyNamePair> list = new List<KeyNamePair>();
            //KeyNamePair[] retValue = null;
            String sql = "SELECT DISTINCT r.UserLevel, r.ConnectionProfile, "	//	1..2
                + " c.AD_Client_ID,c.Name "								//	3..4 
                + "FROM AD_Role r"
                + " INNER JOIN AD_Client c ON (r.AD_Client_ID=c.AD_Client_ID) "
                + "WHERE r.AD_Role_ID=@roleid"		//	#1
                + " AND r.IsActive='Y' AND c.IsActive='Y'";

            //	get Role details
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@roleid", AD_Role_ID);

                dr = DB.ExecuteReader(sql, param);
                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                //list.Add(new KeyNamePair(-1, "Select"));

                do
                {
                    int AD_Client_ID = Util.GetValueOfInt(dr[2].ToString());
                    String Name = dr[3].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Client_ID, Name);
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
        /// <param name="AD_Role_ID">role id </param>
        /// <param name="AD_User_ID">user id</param>
        /// <param name="AD_Client_ID"> client id</param>
        /// <returns></returns>
        public static List<KeyNamePair> GetOrgs(int AD_Role_ID, int AD_User_ID, int AD_Client_ID)
        {
            List<KeyNamePair> list = new List<KeyNamePair>();

            String sql = "SELECT o.AD_Org_ID,o.Name,o.IsSummary "	//	1..3
                + "FROM AD_Role r, AD_Client c"
                + " INNER JOIN AD_Org o ON (c.AD_Client_ID=o.AD_Client_ID OR o.AD_Org_ID=0) "
                + "WHERE r.AD_Role_ID='" + AD_Role_ID + "'" 	//	#1
                + " AND c.AD_Client_ID='" + AD_Client_ID + "'"	//	#2
                + " AND o.IsActive='Y' AND o.IsSummary='N' AND o.IsCostCenter='N' AND o.IsProfitCenter='N' "
                + " AND (r.IsAccessAllOrgs='Y' "
                    + "OR (r.IsUseUserOrgAccess='N' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_Role_OrgAccess ra "
                        + "WHERE ra.AD_Role_ID=r.AD_Role_ID AND ra.IsActive='Y')) "
                    + "OR (r.IsUseUserOrgAccess='Y' AND o.AD_Org_ID IN (SELECT AD_Org_ID FROM AD_User_OrgAccess ua "
                        + "WHERE ua.AD_User_ID='" + AD_User_ID + "' AND ua.IsActive='Y'))"		//	#3
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
                    int AD_Org_ID = Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    bool summary = "Y".Equals(dr[2].ToString());
                    if (summary)
                    {
                        if (role == null)
                        {
                            ctx.SetAD_Client_ID(AD_Client_ID);
                            role = MRole.Get(ctx, AD_Role_ID, AD_User_ID, false);
                        }
                        GetOrgsAddSummary(list, AD_Org_ID, Name, role, ctx);
                    }
                    else
                    {
                        KeyNamePair p = new KeyNamePair(AD_Org_ID, Name);
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
            if (role.GetAD_Tree_Org_ID() == 0)
            {
                return;
            }
            //	Summary Org - Get Dependents
            MTree tree = MTree.Get(ctx, role.GetAD_Tree_Org_ID(), null);
            String sql = "SELECT AD_Client_ID, AD_Org_ID, Name, IsSummary FROM AD_Org "
                + "WHERE IsActive='Y' AND AD_Org_ID IN (SELECT Node_ID FROM "
                + tree.GetNodeTableName()
                + " WHERE AD_Tree_ID='" + tree.GetAD_Tree_ID() + "' AND Parent_ID='" + Summary_Org_ID + "' AND IsActive='Y') "
                + "ORDER BY Name";

            IDataReader dr = DB.ExecuteReader(sql);
            try
            {
                while (dr.Read())
                {
                    //	int AD_Client_ID = rs.getInt(1);
                    int AD_Org_ID = Util.GetValueOfInt(dr[1].ToString());
                    String Name = dr[2].ToString();
                    bool summary = "Y".Equals(dr[3].ToString());
                    //
                    if (summary)
                        GetOrgsAddSummary(list, AD_Org_ID, Name, role, ctx);
                    else
                    {
                        KeyNamePair p = new KeyNamePair(AD_Org_ID, Name);
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
                + "WHERE AD_Org_ID=@p1 AND IsActive='Y' "
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

        /// <summary>
        /// Fetch Login Context
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <returns>LoginContext</returns>
        internal static LoginContext GetLoginContext(LoginModel model)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##AD_User_ID", model.Login1Model.AD_User_ID.ToString());
            ctx.SetContext("##AD_User_Value", model.Login1Model.UserValue);
            ctx.SetContext("##AD_User_Name", model.Login1Model.DisplayName);
            ctx.SetContext("#AD_Language", model.Login1Model.LoginLanguage);

            ctx.SetContext("#AD_Role_ID", model.Login2Model.Role);
            ctx.SetContext("#AD_Role_Name", model.Login2Model.RoleName);

            ctx.SetContext("#AD_Client_ID", model.Login2Model.Client);
            ctx.SetContext("#AD_Client_Name", model.Login2Model.ClientName);

            ctx.SetContext("#AD_Org_ID", model.Login2Model.Org);
            ctx.SetContext("#AD_Org_Name", model.Login2Model.OrgName);

            ctx.SetContext("#M_Warehouse_ID", model.Login2Model.Warehouse);
            ctx.SetContext("#M_Warehouse_Name", model.Login2Model.WarehouseName);
            ctx.SetContext("#Date", model.Login2Model.Date.ToString());
            ctx.SetContext("#AD_ChangeLogBatch", "");


            //{
            //    ____AD_User_ID = model.Login1Model.AD_User_ID.ToString(),
            //    ____AD_User_Name = model.Login1Model.UserName,
            //    __AD_Language = model.Login1Model.LoginLanguage,

            //    __AD_Role_ID = model.Login2Model.Role,
            //    __AD_Role_Name = model.Login2Model.RoleName,

            //    __AD_Client_ID = model.Login2Model.Client,
            //    __AD_Client_Name = model.Login2Model.ClientName,

            //    __AD_Org_ID = model.Login2Model.Org,
            //    __AD_Org_Name = model.Login2Model.OrgName,

            //    __AD_Warehouse_ID = model.Login2Model.Warehouse,
            //    __AD_Warehouse_Name = model.Login2Model.WarehouseName,
            //    __Date = model.Login2Model.Date

            //};
            return ctx;

        }

        /// <summary>
        /// Fetching Login Context
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <param name="ctxLogIn">Login Context</param>
        /// <returns>LoginContext</returns>
        internal static LoginContext GetLoginContext(LoginModel model, Ctx ctxLogIn)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##AD_User_ID", ctxLogIn.GetAD_User_ID().ToString());
            ctx.SetContext("##AD_User_Name", ctxLogIn.GetContext("##AD_User_Name"));


            ctx.SetContext("##AD_User_Value", ctxLogIn.GetContext("##AD_User_Value"));

            //ctx.SetContext("#AD_Language", ctxLogIn.GetContext("#AD_Language"));
            ctx.SetContext("#AD_Language", model.Login2Model.LoginLanguage);

            ctx.SetContext("#AD_Role_ID", model.Login2Model.Role);
            ctx.SetContext("#AD_Role_Name", model.Login2Model.RoleName);

            ctx.SetContext("#AD_Client_ID", model.Login2Model.Client);
            ctx.SetContext("#AD_Client_Name", model.Login2Model.ClientName);

            ctx.SetContext("#AD_Org_ID", model.Login2Model.Org);
            ctx.SetContext("#AD_Org_Name", model.Login2Model.OrgName);

            ctx.SetContext("#M_Warehouse_ID", model.Login2Model.Warehouse);
            ctx.SetContext("#M_Warehouse_Name", model.Login2Model.WarehouseName);
            //ctx.SetContext("#Date", model.Login2Model.Date.ToString());
            return ctx;
        }

        /// <summary>
        /// Function to save login details in Login setting table
        /// </summary>
        /// <param name="model">Login Model</param>
        internal static void SaveLoginSetting(LoginModel model)
        {
            try
            {
                int id = MSequence.GetNextID(Convert.ToInt32(model.Login2Model.Client), "AD_LoginSetting", (Trx)null);
                if (id > 0)
                {
                    string sql = "INSERT INTO AD_LoginSetting " +
                                 "(AD_Client_ID,AD_LoginSetting_ID,AD_Org_ID,AD_Role_ID,AD_User_ID,Created,CreatedBy,IsActive,M_WareHouse_ID,Updated,UpdatedBy) " +
                          " VALUES (" + model.Login2Model.Client + "," + id + "," + model.Login2Model.Org + "," + model.Login2Model.Role + "," + model.Login1Model.AD_User_ID + ",sysdate," + model.Login1Model.AD_User_ID + ",'Y',";
                    if (!String.IsNullOrEmpty(model.Login2Model.Warehouse) && model.Login2Model.Warehouse != "-1")
                    {
                        sql += model.Login2Model.Warehouse + ",";
                    }
                    else
                    {
                        sql += "null,";
                    }
                    sql += "sysdate," + model.Login1Model.AD_User_ID + ")";

                    DB.ExecuteQuery(sql);
                }
            }
            catch
            {
            }
        }

        // VIS0008 Enhancement for VA Mobile App 2 factor Authentication
        /// <summary>
        /// Validate OTP from Google Authenticator or VA Mobile APP
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <param name="TwoFAMethod">Two Factor Auth Method</param>
        /// <returns>true/false</returns>
        public static bool Validate2FAOTP(LoginModel model, string TwoFAMethod)
        {
            bool isValid = false;
            if (TwoFAMethod == X_AD_User.TWOFAMETHOD_GoogleAuthenticator)
            {
                DataSet dsUser = DB.ExecuteDataset(@"SELECT Value, TokenKey2FA, Created, Is2FAEnabled, TwoFAMethod, AD_User_ID FROM AD_User WHERE AD_User_ID = " + model.Login1Model.AD_User_ID);
                if (dsUser != null && dsUser.Tables[0].Rows.Count > 0)
                {
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    string Token2FAKey = Util.GetValueOfString(dsUser.Tables[0].Rows[0]["Value"]);
                    int ADUserID = Util.GetValueOfInt(dsUser.Tables[0].Rows[0]["AD_User_ID"]);
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
                        int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE AD_USER SET TokenKey2FA = '" + encKey + @"' WHERE 
                                    AD_USER_ID = " + model.Login1Model.AD_User_ID));
                    }
                }
            }
            else if (TwoFAMethod == X_AD_User.TWOFAMETHOD_VAMobileApp)
            {
                if (_dictVAMobTokens.ContainsKey(model.Login1Model.UserValue))
                {
                    // Get token details for the login user
                    List<dynamic> tknDetails = _dictVAMobTokens[model.Login1Model.UserValue];
                    if (tknDetails != null && tknDetails.Count > 1)
                    {
                        int totSeconds = Util.GetValueOfInt((System.DateTime.Now - tknDetails[1]).TotalSeconds);
                        string TokenNo = tknDetails[0];
                        // Checked time duration after token generation (60 secs)
                        if (totSeconds <= 60 && TokenNo == model.Login1Model.OTP2FA)
                        {
                            _dictVAMobTokens.Remove(model.Login1Model.UserValue);
                            return true;
                        }
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// function to check token details based on the token passed in the parameter
        /// </summary>
        /// <param name="TokenNum"></param>
        /// <returns>Dictionary of values like Success, User Value and password</returns>
        public static Dictionary<string, object> GetTokenDetails(string TokenNum)
        {
            Dictionary<string, object> retRes = new Dictionary<string, object>();
            retRes.Add("Success", false);

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@p1", TokenNum);
            DataSet ds = DB.ExecuteDataset("SELECT AuthTokenExpireTime, Value, Password, IsAuthTokenOneTime, AD_User_ID FROM AD_User WHERE IsActive = 'Y' AND AuthToken = @p1", param);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DateTime? expTime = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["AuthTokenExpireTime"]);
                DateTime? curTime = System.DateTime.Now.ToUniversalTime();
                if (curTime <= expTime)
                {
                    if (Util.GetValueOfString(ds.Tables[0].Rows[0]["IsAuthTokenOneTime"]).Equals("Y"))
                    {
                        int count = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE AD_User SET AuthTokenExpireTime = " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + @" 
                        WHERE AD_User_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"])));
                    }
                    retRes.Add("User", ds.Tables[0].Rows[0]["Value"]);
                    retRes.Add("Password", ds.Tables[0].Rows[0]["Password"].ToString());
                    retRes["Success"] = true;
                    return retRes;
                }
            }

            return retRes;
        }

        internal static Dictionary<string, List<dynamic>> _dictVAMobTokens = new Dictionary<string, List<dynamic>>();

        /// <summary>
        /// Check if VA Mobile app is scanned for the user passed in the parameter
        /// </summary>
        /// <param name="AD_User_ID">User ID</param>
        /// <returns>true/false</returns>
        public static bool IsDeviceLinked(Ctx ctx, int AD_User_ID)
        {
            bool isLinked = true;
            if (X_AD_User.TWOFAMETHOD_VAMobileApp == Util.GetValueOfString(DB.ExecuteScalar("SELECT TwoFAMethod FROM AD_User WHERE AD_User_ID = " + AD_User_ID)))
            {
                isLinked = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VA074_MobileLinked_ID) FROM VA074_MobileLinked 
                            WHERE VA074_AD_User_ID = " + AD_User_ID + " AND VA074_PushNotiToken IS NOT NULL AND IsActive = 'Y'")) > 0;
            }
            if (isLinked)
                VAdvantage.PushNotif.PushNotification.SendNotificationToUser(AD_User_ID, 0, 0, "", Msg.GetMsg(ctx, "OTPLoginSuccess"), "OTH");
            return isLinked;
        }

        /// <summary>
        /// Send push notificaiton token to user
        /// </summary>
        /// <param name="ADUserID">User ID</param>
        /// <param name="userSKey">User Search Key</param>
        public static void SendPushNotToken(int ADUserID, string userSKey)
        {
            string TokenNo = GetRndNum(6);
            List<dynamic> tokenDetails = new List<dynamic>();
            if (_dictVAMobTokens.ContainsKey(userSKey))
            {
                tokenDetails = _dictVAMobTokens[userSKey];
                tokenDetails.Clear();
                tokenDetails.Add(TokenNo);
                tokenDetails.Add(System.DateTime.Now);
                _dictVAMobTokens[userSKey] = tokenDetails;
            }
            else
            {
                tokenDetails.Add(TokenNo);
                tokenDetails.Add(System.DateTime.Now);
                _dictVAMobTokens.Add(userSKey, tokenDetails);
            }
            VAdvantage.PushNotif.PushNotification.SendNotificationToUser(ADUserID, 0, 0, "", TokenNo, "OTP");
        }
    }
}
