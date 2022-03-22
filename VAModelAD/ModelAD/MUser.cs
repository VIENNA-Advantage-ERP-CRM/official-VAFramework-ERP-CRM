/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_User
 * Chronological Development
 * 
 * Veena Pandey     14-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Net;
using System.Threading;
using System.Reflection;
using System.IO;

namespace VAdvantage.Model
{
    /// <summary>
    /// User Model
    /// </summary>
    public class MUser : X_AD_User
    {
        /**	Cache					*/
        private static CCache<int, MUser> cache = new CCache<int, MUser>("AD_User", 30, 60);
        //	Static Logger			
        private static VLogger _log = VLogger.GetVLogger(typeof(MUser).FullName);
        /**	Roles of User with Org	*/
        private MRole[] _roles = null;
        private int rolesAD_Org_ID = -1;
        /** Is Administrator		*/
        private bool? _isAdministrator = null;
        /** Is System Admin			*/
        private bool? _isSystemAdministrator = null;
        /** User Access Rights		*/
        private X_AD_UserBPAccess[] _bpAccess = null;
        /** User Preference			*/
        private MUserPreference _preference = null;
        private bool updateYellowFinUser = true;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MUser(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_User_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MUser(Ctx ctx, int AD_User_ID, Trx trxName)
            : base(ctx, AD_User_ID, trxName)
        {
            if (AD_User_ID == 0)
            {
                SetIsFullBPAccess(true);
                SetNotificationType(NOTIFICATIONTYPE_EMail);
                SetIsEMailBounced(false);
            }
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="partner">partner</param>
        public MUser(X_C_BPartner partner)
            : this(partner.GetCtx(), 0, partner.Get_TrxName())
        {
            SetClientOrg(partner);
            SetC_BPartner_ID(partner.GetC_BPartner_ID());
            SetName(partner.GetName());
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="partner">partner</param>
        public MUser(PO partner)
            : this(partner.GetCtx(), 0, partner.Get_TrxName())
        {
            SetClientOrg(partner);
            SetC_BPartner_ID((int)partner.Get_Value("C_BPartner_ID"));
            SetName(partner.Get_Value("Name").ToString());
        }

        /// <summary>
        /// Get Current User (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>user</returns>
        public static MUser Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetAD_User_ID());
        }

        /// <summary>
        /// Get User (cached). Also loads Admninistrator (0)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_User_ID">id</param>
        /// <returns>user</returns>
        public static MUser Get(Ctx ctx, int AD_User_ID)
        {
            int key = AD_User_ID;
            MUser retValue = (MUser)cache[key];
            if (retValue == null)
            {
                retValue = new MUser(ctx, AD_User_ID, null);
                if (AD_User_ID == 0)
                {
                    Trx trxName = null;
                    retValue.Load(trxName);	//	load System Record
                }
                cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Get User from email
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="email">emal</param>
        /// <param name="trxName">transaction</param>
        /// <returns>user or null</returns>
        public static MUser Get(Ctx ctx, String email, Trx trxName)
        {
            if (email == null || email.Length == 0)
                return null;

            int AD_Client_ID = ctx.GetAD_Client_ID();
            MUser retValue = null;
            String sql = "SELECT * FROM AD_User "
                + "WHERE EMail='" + email + "' AND AD_Client_ID=" + AD_Client_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count != 0 && ds.Tables[0].Rows.Count > 1)
                    {
                        _log.Warning("More then one user with EMail = " + email);
                    }
                    retValue = new MUser(ctx, ds.Tables[0].Rows[0], trxName);
                }
                else
                {
                    _log.Fine("No record");
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get active User with name/pwd
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="email">emal</param>
        /// <param name="password">password</param>
        /// <param name="trxName">transaction</param>
        /// <returns>user or null</returns>
        public static MUser Get(Ctx ctx, String name, String password, Trx trxName)
        {
            if (name == null || name.Length == 0 || password == null || password.Length == 0)
            {
                _log.Warning("Invalid Name/Password = " + name + "/" + password);
                return null;
            }
            int AD_Client_ID = ctx.GetAD_Client_ID();

            MUser retValue = null;
            String sql = "SELECT * FROM AD_User "
                + "WHERE Name='" + name + "' AND Password='" + password + "' AND IsActive='Y' AND AD_Client_ID=" + AD_Client_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count != 0 && ds.Tables[0].Rows.Count > 1)
                    {
                        _log.Warning("More then one user with Name/Password = " + name);
                    }
                    retValue = new MUser(ctx, ds.Tables[0].Rows[0], trxName);
                }
                else
                {
                    _log.Fine("No record");
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Name of AD_User
        /// </summary>
        /// <param name="AD_User_ID">id</param>
        /// <returns>Name of user or ?</returns>
        public static String GetNameOfUser(int AD_User_ID)
        {
            String name = "?";
            //	Get ID
            String sql = "SELECT Name FROM AD_User WHERE AD_User_ID=" + AD_User_ID;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    name = dr[0].ToString();
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return name;
        }

        /// <summary>
        /// Get active Users of BPartner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_ID">id</param>
        /// <returns>array of users</returns>
        public static MUser[] GetOfBPartner(Ctx ctx, int C_BPartner_ID)
        {
            List<MUser> list = new List<MUser>();
            String sql = "SELECT * FROM AD_User WHERE C_BPartner_ID=" + C_BPartner_ID + " AND IsActive='Y'";

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MUser(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MUser[] retValue = new MUser[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get active BP Access records
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>access list</returns>
        public X_AD_UserBPAccess[] GetBPAccess(bool requery)
        {
            if (_bpAccess != null && !requery)
                return _bpAccess;
            String sql = "SELECT * FROM AD_UserBPAccess WHERE AD_User_ID=" + GetAD_User_ID() + " AND IsActive='Y'";
            List<X_AD_UserBPAccess> list = new List<X_AD_UserBPAccess>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new X_AD_UserBPAccess(GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _bpAccess = new X_AD_UserBPAccess[list.Count];
            _bpAccess = list.ToArray();
            return _bpAccess;
        }

        /// <summary>
        /// Get User Preference
        /// </summary>
        /// <returns>user preference</returns>
        public MUserPreference GetPreference()
        {
            if (_preference == null)
                _preference = MUserPreference.GetOfUser(this, true);
            return _preference;
        }

        /// <summary>
        /// Get User Roles for Org
        /// </summary>
        /// <param name="AD_Org_ID">org id</param>
        /// <returns>array of roles</returns>
        public MRole[] GetRoles(int AD_Org_ID)
        {
            if (_roles != null && rolesAD_Org_ID == AD_Org_ID)
                return _roles;

            List<MRole> list = new List<MRole>();

            //String sql = "SELECT * FROM AD_Role r "
            //    + "WHERE r.IsActive='Y'"
            //    + " AND EXISTS (SELECT * FROM AD_Role_OrgAccess ro"
            //        + " WHERE r.AD_Role_ID=ro.AD_Role_ID AND ro.IsActive='Y' AND ro.AD_Org_ID=@orgid)"
            //    + " AND EXISTS (SELECT * FROM AD_User_Roles ur"
            //        + " WHERE r.AD_Role_ID=ur.AD_Role_ID AND ur.IsActive='Y' AND ur.AD_User_ID=@userid) "
            //    + "ORDER BY AD_Role_ID";

            // SqlParameter[] param = new SqlParameter[2];
            // param[0] = new SqlParameter("@orgid", AD_Org_ID);
            // param[1] = new SqlParameter("@userid", GetAD_User_ID());


            // Commented code above to resolve issue in Fetching Roles of selected user against Organization
            // and rewritten query 
            String sql = @"SELECT * FROM AD_Role r WHERE r.IsActive='Y' 
                        AND EXISTS (SELECT * FROM AD_User_Roles ur WHERE r.AD_Role_ID=ur.AD_Role_ID AND ur.IsActive='Y' AND ur.AD_User_ID = @userid)
                        AND ((r.isaccessallorgs = 'Y') OR (r.IsUseUserOrgAccess <> 'Y' AND EXISTS (SELECT * FROM AD_Role_OrgAccess ro WHERE r.AD_Role_ID=ro.AD_Role_ID AND ro.IsActive='Y' AND ro.AD_Org_ID=@orgid)) 
                        OR (r.IsUseUserOrgAccess = 'Y' AND EXISTS (SELECT * FROM AD_User_OrgAccess uo WHERE uo.AD_User_ID=@userid1 AND uo.IsActive='Y' AND uo.AD_Org_ID=@orgid1))) ORDER BY AD_Role_ID";

            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@userid", GetAD_User_ID());

            param[1] = new SqlParameter("@orgid", AD_Org_ID);

            param[2] = new SqlParameter("@userid1", GetAD_User_ID());
            param[3] = new SqlParameter("@orgid1", AD_Org_ID);

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MRole(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            rolesAD_Org_ID = AD_Org_ID;
            _roles = new MRole[list.Count];
            _roles = list.ToArray();
            return _roles;
        }

        /// <summary>
        /// Get Users with Role
        /// </summary>
        /// <param name="role">role</param>
        /// <returns>array of users</returns>
        public static MUser[] GetWithRole(MRole role)
        {
            List<MUser> list = new List<MUser>();
            String sql = "SELECT * FROM AD_User u "
                + "WHERE u.IsActive='Y'"
                + " AND EXISTS (SELECT * FROM AD_User_Roles ur "
                    + "WHERE ur.AD_User_ID=u.AD_User_ID AND ur.AD_Role_ID=" + role.GetAD_Role_ID() + " AND ur.IsActive='Y')";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MUser(role.GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MUser[] retValue = new MUser[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Is User an Administrator?
        /// </summary>
        /// <returns>true id Admin</returns>
        public bool IsAdministrator()
        {
            if (_isAdministrator == null)
            {
                _isAdministrator = false;
                MRole[] roles = GetRoles(0);
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i].IsAdministrator())
                    {
                        _isAdministrator = true;
                        break;
                    }
                }
            }
            return (bool)_isAdministrator;
        }

        /// <summary>
        /// Is User a System Administrator?
        /// </summary>
        /// <returns>true if System Admin</returns>
        public bool IsSystemAdministrator()
        {
            if (_isSystemAdministrator == null)
            {
                _isSystemAdministrator = false;
                MRole[] roles = GetRoles(0);
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i].GetAD_Role_ID() == 0)
                    {
                        _isSystemAdministrator = true;
                        break;
                    }
                }
            }

            return (bool)_isSystemAdministrator;
        }

        /// <summary>
        /// User is SalesRep
        /// </summary>
        /// <param name="AD_User_ID">id</param>
        /// <returns>true if sales rep</returns>
        public static bool IsSalesRep(int AD_User_ID)
        {
            if (AD_User_ID == 0)
                return false;



            String sql = "SELECT MAX(AD_User_ID) FROM AD_User u"
                + " INNER JOIN C_BPartner bp ON (u.C_BPartner_ID=bp.C_BPartner_ID) "
                + "WHERE bp.IsSalesRep='Y' AND AD_User_ID=" + AD_User_ID;
            int no = DataBase.DB.GetSQLValue(null, sql);
            return no == AD_User_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasRole()
        {
            if (_roles != null && _roles.Length > 0)
                return true;

            int roleCount = 0;
            String sql = "SELECT COUNT(*) FROM AD_User_Roles ur "
                    + "WHERE ur.AD_User_ID=@userid AND ur.IsActive='Y'";

            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@userid", GetAD_User_ID());
                idr = ExecuteQuery.ExecuteReader(sql, param);
                while (idr.Read())
                {
                    roleCount = idr[0].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(idr[0].ToString());
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return roleCount != 0;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord || (Is_ValueChanged("VA037_BIUserName") || Is_ValueChanged("VA037_BIPassword") || Is_ValueChanged("Name") || Is_ValueChanged("LastName") || Is_ValueChanged("VA037_YellowfinRoles_ID") || Is_ValueChanged("EMail") || Is_ValueChanged("VA037_BIUser") || Is_ValueChanged("VA037_IsLinkExistingUser")))
            {
                updateYellowFinUser = true;
            }
            else
            {
                updateYellowFinUser = false;
            }
            //	New Address invalidates verification
            if (!newRecord && Is_ValueChanged("EMail"))
                SetEMailVerifyDate(null);
            if (newRecord || base.GetValue() == null || Is_ValueChanged("Value"))
            {

                SetValue(base.GetValue());
                if (newRecord)
                {




                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_User WHERE lower(Value) = lower('" + GetValue() + "')"));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "SearchShouldBeUnique", true));
                        return false;
                    }
                }
                else
                {
                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_User WHERE AD_User_ID !=" + GetAD_User_ID() + " AND lower(Value) = lower('" + GetValue() + "')"));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "SearchShouldBeUnique", true));
                        return false;
                    }
                }
            }

            if (newRecord || base.GetLDAPUser() == null || Is_ValueChanged("LDAPUser"))
            {
                string ldapuser = base.GetLDAPUser();
                if (ldapuser == null || ldapuser.Trim().Length == 0)
                {
                    String result = CleanValue(GetValue());
                    SetLDAPUser(result);
                }
                else
                {
                    SetLDAPUser(CleanValue(ldapuser));
                }

                if (newRecord)
                {
                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_User WHERE lower(Ldapuser) = lower('" + GetLDAPUser() + "')"));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "LDAPShouldBeUnique", true));
                        return false;
                    }
                }
                else
                {
                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_User WHERE AD_User_ID !=" + GetAD_User_ID() + " AND lower(Ldapuser) = lower('" + GetLDAPUser() + "')"));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "LDAPShouldBeUnique", true));
                        return false;
                    }
                }


            }

            //	MUser operator = MUser.get(getCtx(), getCtx().getAD_User_ID());
            //	You cannot change a password of a user with a role unless you are the user
            if (!newRecord && Is_ValueChanged("Password")
                    && HasRole() && GetCtx().GetAD_User_ID() != GetAD_User_ID())
            {
                log.SaveError("Warning", Msg.GetMsg(GetCtx(), "UserCannotUpdate", true));
                return false;
            }

            // VIS0008 Change for 2FA
            if (!newRecord && Is_ValueChanged("TwoFAMethod") 
                && (Util.GetValueOfString(Get_ValueOld("TwoFAMethod")) == X_AD_User.TWOFAMETHOD_GoogleAuthenticator))
            {
                SetTokenKey2FA("");
            }

            if (Is_ValueChanged("Password"))
            {
                string pwd = GetPassword();
                string oldPwd = Util.GetValueOfString(Get_ValueOld("Password"));
                if ((pwd == null && GetCtx().GetAD_User_ID() == GetAD_User_ID()) || (pwd == null && !HasRole() && GetCtx().GetAD_User_ID() != GetAD_User_ID()))
                {
                    log.SaveInfo("Info", Msg.GetMsg(GetCtx(), "PasswordSetNull", true) + " " + GetName());
                    return true;
                }

                if (pwd != null || !newRecord)
                {
                    string validated = Common.Common.ValidatePassword(oldPwd, pwd, pwd);
                    if (validated.Length > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), validated, true));
                        return false;
                    }
                }
                if (!newRecord && GetCtx().GetAD_User_ID() == GetAD_User_ID())
                {
                    int validity = GetCtx().GetContextAsInt("#" + Common.Common.Password_Valid_Upto_Key);
                    base.SetPasswordExpireOn(DateTime.Now.AddMonths(validity));
                }
            }
            return true;
        }

        string sql = "SELECT count(AD_User_ID) FROM AD_USER WHERE IsLoginUser='Y' AND IsActive = 'Y' AND AD_Client_ID=";

        protected override bool AfterSave(bool newRecord, bool success)
        {
            log.Info("Aftersave start" + Util.GetValueOfString(GetCtx().GetContext("VerifyVersionRecord")));

            if (Util.GetValueOfString(GetCtx().GetContext("VerifyVersionRecord")) == "Y")
            {
                return success;
            }
            log.Info("Aftersave start" + "");
            UpdateCustomerUser(Env.GetApplicationURL(GetCtx()), GetAD_User_ID(), GetName(), GetAD_Client_ID(), 0, IsLoginUser(), false);
            // Following Work is For Creating and Updating Yellowfin User if Yellowfin Module exists...................
            #region
            if (success)
            {
                log.Info("Aftersave success");
                // For Saving YellowFin user.......................
                object ModuleId = DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where IsActive='Y' AND prefix='VA037_'");
                if (ModuleId != null && ModuleId != DBNull.Value)
                {
                    //Update/Save in Case user change following value........Password,FirstName,LastName,YellowfinRole,Email,
                    if (updateYellowFinUser)
                    {
                        if (IsVA037_BIUser() == true)
                        {
                            log.SaveError("Aftersave IsVA037_BIUser", "");
                            string Error = "";
                            var Dll = Assembly.Load("VA037");
                            var BIUser = Dll.GetType("VA037.BIProcess.BIUsers");
                            var objBIUser = Activator.CreateInstance(BIUser);
                            var BICreateUser = BIUser.GetMethod("CreateBIUser");
                            object[] args = new object[] { GetCtx(), GetAD_User_ID(), "" };
                            BICreateUser.Invoke(objBIUser, args);
                            string value = (string)BIUser.GetProperty("UserInfo").GetValue(objBIUser, null);
                            Error = (string)args[2];
                            if (Error != "")
                            {
                                log.SaveError("Aftersave Error", "");
                                SetVA037_BIUser(false);
                                Save();
                                log.SaveWarning(Error, "");
                                return true;
                            }
                            else
                            {
                                log.SaveError("Aftersave SaveInfo", "");
                                log.SaveInfo(value, "");
                                return true;
                            }
                        }
                    }
                }
                #region

                log.SaveError("Aftersave createJasperUser", "");
                // The following work for creating JasperUser................
                createJasperUser();
                //End
                #endregion
            }
            #endregion
            //End
            //UpdateCustomerUser(GetCtx().Get("#AppFullUrl"), GetAD_User_ID(), GetName(), GetAD_Client_ID(), 0, IsLoginUser(), false);
            return success;
        }

        public bool createJasperUser()
        {
            object ModuleId = DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where prefix='VA039_'");
            if (ModuleId != null && ModuleId != DBNull.Value)
            {

                if (IsVA039_IsJasperUser() == true)
                {
                    log.SaveError("Aftersave IsVA039_IsJasperUser", "");
                    string Error = "";
                    var Dll = Assembly.Load("VA039");
                    var JasperUser = Dll.GetType("VA039.Classes.Users");
                    var objJasperUser = Activator.CreateInstance(JasperUser);
                    var BICreateUser = JasperUser.GetMethod("CreateUser");
                    object[] args = new object[] { GetCtx(), GetAD_User_ID(), "" };
                    BICreateUser.Invoke(objJasperUser, args);
                    Error = (string)args[2];
                    if (Error != "")
                    {
                        log.SaveError("Aftersave Error", "");
                        SetVA039_IsJasperUser(false);
                        Save();
                        log.SaveWarning(Error, "");
                        return true;
                    }
                    log.SaveError("Aftersave END", "");
                }


            }
            return true;
        }

        public bool UpdateCustomerUser(string appUrl, int AD_User_ID, string Name, int AD_Client_ID, int count, bool isLoginUser, bool isDelete)
        {

            Thread thread = new Thread(new ParameterizedThreadStart(WorkThreadFunction));
            thread.Start(new ThreadParameter() { appUrl = appUrl, AD_User_ID = AD_User_ID, Name = Name, AD_Client_ID = AD_Client_ID, count = count, isLoginUser = isLoginUser, isDelete = isDelete });


            return true;
        }

        private void WorkThreadFunction(object data)
        {
            ThreadParameter tp = (ThreadParameter)data;

            var client = ServerEndPoint.GetCloudClient();
            if (client != null)
            {
                int count = Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql + tp.AD_Client_ID));

                client.CreateCustomerUserCompleted += (se, ev) =>
                {
                    client.Close();
                    //VLogger.Get().Info("URL => " + applicationURL);
                    if (ev.Result != null)
                    {
                        log.SaveInfo("UserCloudResult=> ", "Result=> " + ev.Result.ToString());
                    }
                    else
                    {
                        log.SaveInfo("UserCloudResult=> ", "Result => " + "Null");
                    }
                };
                //log.SaveInfo("Url=> ", "Url=> " + Envs.GetApplicationURL());
                //log.SaveInfo("UserID=> ", "UserID=> " + GetAD_User_ID());
                //log.SaveInfo("Name=> ", "Name=> " + GetName());
                //log.SaveInfo("teanant=> ", "teanant=> " + GetAD_Client_ID());
                //log.SaveInfo("Count=> ", "Count=> " + count);
                //log.SaveInfo("isLogin=> ", "isLogin=> " + IsLoginUser());
                try
                {
                    ServicePointManager.Expect100Continue = false;
                    client.CreateCustomerUserAsync(tp.appUrl, tp.AD_User_ID, tp.Name, tp.AD_Client_ID, count, tp.isLoginUser, false,
                        ServerEndPoint.GetAccesskey());
                }
                catch
                {
                }

                //try
                //{
                //    string res = client.CreateCustomerUser(appUrl, AD_User_ID, Name, AD_Client_ID, count, isLoginUser, false, ServerEndPoint.GetAccesskey());
                //    if (res != null)
                //    {
                //        log.SaveInfo("UserCloudResult=> ", "Result=> " + res);
                //    }
                //    else
                //    {
                //        log.SaveInfo("UserCloudResult=> ", "Result => " + "Null");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    log.SaveError("UserCloudResult=> ", "Result => " + ex.Message);
                //}
            }
        }

        protected override bool AfterDelete(bool success)
        {
            UpdateCustomerUser(Env.GetApplicationURL(GetCtx()), GetAD_User_ID(), GetName(), GetAD_Client_ID(), 0, IsLoginUser(), true);
            // UpdateCustomerUser(GetCtx().Get("#AppFullUrl"), GetAD_User_ID(), GetName(), GetAD_Client_ID(), 0, IsLoginUser(), false);
            //return true;
            return success;
        }

        protected override bool BeforeDelete()
        {
            try
            {
                //string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where ad_user_ID=" + GetAD_User_ID() + " and AD_Client_ID=" + GetAD_Client_ID();
                //object ID = DB.ExecuteScalar(sql);
                //if (ID == null || ID == DBNull.Value)
                //{
                //    ID = 0;
                //    return true;
                //}
                //sql = "Select Gmail_UID from ad_user where ad_user_ID=" + GetAD_User_ID();
                //object UEmailID = DB.ExecuteScalar(sql);
                //if (UEmailID == null || UEmailID == DBNull.Value)
                //{
                //    return true;
                //}

                //X_WSP_DeletedUserLog dlUser = new X_WSP_DeletedUserLog(Envs.GetCtx(), 0, null);
                //dlUser.SetAD_User_ID(Envs.GetCtx().GetAD_User_ID());
                //dlUser.SetAD_Client_ID(GetAD_Client_ID());
                //dlUser.SetAD_Org_ID(GetAD_Org_ID());
                //dlUser.SetWSP_IsDeletedFromGmail(false);
                //dlUser.SetWSP_GmailConfiguration_ID(Convert.ToInt32(ID));
                //dlUser.SetWSP_Gmail_UID(UEmailID.ToString());
                //if (dlUser.Save())
                //{
                //    return true;
                //}
                return true;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, " ErrorInBeforeDeleteMethod");
                return false;
            }
        }




        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">description to be added</param>
        public void AddDescription(String description)
        {
            if (description == null || description.Length == 0)
                return;
            String descr = GetDescription();
            if (descr == null || descr.Length == 0)
                SetDescription(description);
            else
                SetDescription(descr + " - " + description);
        }

        /// <summary>
        /// Get First Name
        /// </summary>
        /// <returns>first name</returns>
        public String GetFirstName()
        {
            return GetName(GetName(), true);
        }

        /// <summary>
        /// Get Last Name
        /// </summary>
        /// <returns>last name</returns>
        public String GetLastName()
        {
            return GetName(GetName(), false);
        }

        /// <summary>
        /// Get First/Last Name
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="getFirst">if true first name is returned</param>
        /// <returns>first/last name</returns>
        private String GetName(String name, bool getFirst)
        {
            if (name == null || name.Length == 0)
                return "";
            String first = null;
            String last = null;

            //	double names not handled gracefully nor titles 
            //	nor (former) aristrocratic world de/la/von 
            bool lastFirst = name.IndexOf(',') != -1;
            StringTokenizer st = null;
            if (lastFirst)
                st = new StringTokenizer(name, ",");
            else
                st = new StringTokenizer(name, " ");
            while (st.HasMoreTokens())
            {
                String s = st.NextToken().Trim();
                if (lastFirst)
                {
                    if (last == null)
                        last = s;
                    else if (first == null)
                        first = s;
                }
                else
                {
                    if (first == null)
                        first = s;
                    else
                        last = s;
                }
            }
            if (getFirst)
            {
                if (first == null)
                    return "";
                return first.Trim();
            }
            if (last == null)
                return "";
            return last.Trim();
        }

        /// <summary>
        /// Get Value - 7 bit lower case alpha numerics max length 8
        /// </summary>
        /// <returns>value</returns>
        public new String GetValue()
        {
            String s = base.GetValue();
            if (s != null)
                return s;
            SetValue(null);
            return base.GetValue();
        }

        /// <summary>
        /// Set Value - 7 bit lower case alpha numerics max length 8
        /// </summary>
        /// <param name="value">value</param>
        public new void SetValue(String value)
        {
            if (value == null || value.Trim().Length == 0)
                value = GetLDAPUser();
            if (value == null || value.Length == 0)
                value = GetName();
            if (value == null || value.Length == 0)
                value = "noname";
            //
            String result = CleanValue(value);
            //if (result.Length > 8)
            //{
            //    String first = GetName(value, true);
            //    String last = GetName(value, false);
            //    if (last.Length > 0)
            //    {
            //        String temp = last;
            //        if (first.Length > 0)
            //            temp = first.Substring(0, 1) + last;
            //        result = CleanValue(temp);
            //    }
            //    else
            //        result = CleanValue(first);
            //}
            if (result.Length > 60)
                result = result.Substring(0, 60);
            base.SetValue(result);
        }

        /// <summary>
        /// clean value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>lower case cleaned value</returns>
        private String CleanValue(String value)
        {
            char[] chars = value.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                //ch = Char.ToLower(ch);
                if ((ch >= '0' && ch <= '9')		//	digits
                    || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '.' || ch == '@' || ch == '_')	//	characters
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get AD_User_ID
        /// </summary>
        /// <param name="email">mail</param>
        /// <param name="AD_Client_ID">client</param>
        /// <returns> user id or 0</returns>
        /// <author>Raghu</author>
        public static int GetAD_User_ID(String email, int AD_Client_ID)
        {
            int AD_User_ID = 0;
            IDataReader idr = null;
            String sql = "SELECT AD_User_ID FROM AD_User "
                + "WHERE UPPER(EMail)=@param1"
                + " AND AD_Client_ID=@param2";
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", email.ToUpper());
                param[1] = new SqlParameter("@param2", AD_Client_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                while (idr.Read())
                {
                    AD_User_ID = Utility.Util.GetValueOfInt(idr[0].ToString());//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, email, e);
            }

            return AD_User_ID;
        }


        /// <summary>
        /// Get Notification via EMail
        /// </summary>
        /// <returns>true if email</returns>
        public bool IsNotificationEMail()
        {
            String s = GetNotificationType();
            return s == null || NOTIFICATIONTYPE_EMail.Equals(s);
        }

        /// <summary>
        /// Get Notification via Note
        /// </summary>
        /// <returns>true if note</returns>
        public bool IsNotificationNote()
        {
            String s = GetNotificationType();
            return s != null && NOTIFICATIONTYPE_Notice.Equals(s);
        }

        /// <summary>
        /// Is it an Online Access User
        /// </summary>
        /// <returns>true if it has an email and password</returns>
        public bool IsOnline()
        {
            if (GetEMail() == null || GetPassword() == null)
                return false;
            return true;
        }

        /// <summary>
        /// Set EMail - reset validation
        /// </summary>
        /// <param name="eMail">email</param>
        public new void SetEMail(String eMail)
        {
            base.SetEMail(eMail);
            SetEMailVerifyDate(null);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MUser[")
                .Append(Get_ID())
                .Append(",Name=").Append(GetName())
                .Append(",EMailUserID=").Append(GetEMailUser())
                .Append("]");
            return sb.ToString();
        }

        public bool IsEMailValid()
        {
            return !IsEMailBounced()
                && (ValidateEmail(null) == null);
        }	//	isEMailValid

        private String ValidateEmail(System.Net.IPAddress ia)
        {
            if (ia == null)
                return "NoEmail";
            if (true)
                return null;
        }

        public static bool GetIsEmployee(Ctx ctx, int AD_USER_ID)
        {
            MUser user = MUser.Get(ctx, AD_USER_ID);
            X_C_BPartner bp = new X_C_BPartner(ctx, user.GetC_BPartner_ID(), null);
            user = null;
            if (bp == null)
                return false;
            return bp.IsEmployee();
        }


    }


    public class ThreadParameter
    {
        public string appUrl { get; set; }
        public int AD_User_ID { get; set; }
        public string Name { get; set; }
        public int AD_Client_ID { get; set; }
        public int count { get; set; }
        public bool isLoginUser { get; set; }
        public bool isDelete { get; set; }

    }
}