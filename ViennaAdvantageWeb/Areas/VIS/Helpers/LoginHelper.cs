using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
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
        /// <summary>
        /// return is credential provide by user is right or not
        /// </summary>
        /// <param name="model">login model class</param>
        /// <param name="roles">out roles , has role list of user</param>
        /// <param name="ctx" ></param>
        /// <returns>true if athenicated</returns>
        public static bool Login(LoginModel model, out List<KeyNamePair> roles)
        {
            roles = null;
            // loginModel = null;
            //bool isMatch = false;

            SecureEngine.Encrypt("t"); //Initialize 

            //	Cannot use encrypted password
            if (model.Login1Model.Password != null && SecureEngine.IsEncrypted(model.Login1Model.Password))
            {
                //log.warning("Cannot use Encrypted Password");
                return false;
            }
            //	Authentification
            bool authenticated = false;
            bool isLDAP = false;
            MSystem system = MSystem.Get(new Ctx());

            if (system != null && system.IsLDAP())
            {
                authenticated = system.IsLDAP(model.Login1Model.UserName, model.Login1Model.Password);
                if (authenticated)
                {
                    model.Login1Model.Password = null;
                }
                isLDAP = true;
                // if not authenticated, use AD_User as backup
            }



            StringBuilder sql = new StringBuilder("SELECT u.AD_User_ID, r.AD_Role_ID,r.Name,")
               .Append(" u.ConnectionProfile, u.Password ")	//	4,5
               .Append("FROM AD_User u")
               .Append(" INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive='Y')")
               .Append(" INNER JOIN AD_Role r ON (ur.AD_Role_ID=r.AD_Role_ID AND r.IsActive='Y') ");
            //.Append("WHERE COALESCE(u.LDAPUser,u.Name)=@username")		//	#1
            if (isLDAP && authenticated)
            {
                sql.Append(" WHERE (COALESCE(u.LDAPUser,u.Value)=@username)");
            }
            else if (isLDAP && !authenticated && model.Login1Model.Password == null)// If user not authenicated using LDAP, then if LDAP user is available
            {
                sql.Append(" WHERE (u.LDAPUser=@username OR u.Name=@username OR u.Value=@username)");
            }
            else
            {
                sql.Append(" WHERE (u.Name=@username OR u.Value=@username)");
            }

            sql.Append(" AND u.IsActive='Y' ")
             .Append(" AND u.IsLoginUser='Y' ")
             .Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE u.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')")
             .Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE r.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')");
            string sqlEnc = "select isencrypted from ad_column where ad_table_id=(select ad_table_id from ad_table where tablename='AD_User') and columnname='Password'";
            char isEncrypted = Convert.ToChar(DB.ExecuteScalar(sqlEnc));
            if (model.Login1Model.Password != null)
            {
                if (isEncrypted == 'Y')
                {
                    sql.Append(" AND (u.Password='" + SecureEngine.Encrypt(model.Login1Model.Password) + "')");	//  #2/3
                }
                else
                {
                    sql.Append(" AND (u.Password='" + model.Login1Model.Password + "')");	//  #2/3
                }
            }
            sql.Append(" ORDER BY r.Name");
            IDataReader dr = null;
            //try
            //{
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@username", model.Login1Model.UserName);
            //	execute a query
            dr = DB.ExecuteReader(sql.ToString(), param);

            if (!dr.Read())		//	no record found
            {
                dr.Close();
                return false;
            }

            int AD_User_ID = Util.GetValueOfInt(dr[0].ToString()); //User Id

            roles = new List<KeyNamePair>(); //roles

            List<int> usersRoles = new List<int>();


            do	//	read all roles
            {
                AD_User_ID = Util.GetValueOfInt(dr[0].ToString());
                int AD_Role_ID = Util.GetValueOfInt(dr[1].ToString());

                String Name = dr[2].ToString();
                KeyNamePair p = new KeyNamePair(AD_Role_ID, Name);

                roles.Add(p);

                usersRoles.Add(AD_Role_ID);
            }
            while (dr.Read());

            dr.Close();
            model.Login1Model.AD_User_ID = AD_User_ID;


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
                        //1 firt check  - Check role exist
                        //if (usersRoles.Contains(Util.GetValueOfInt(drLogin[0])))
                        //{
                        //    //check for Org Access Setting
                        //    bool isUseUserOrgAccess = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsUseUserOrgAccess FROM AD_ROLE WHERE AD_ROLE_ID = " + drLogin[0].ToString())) == "Y";
                        //    if (isUseUserOrgAccess) //User User Org
                        //    {
                        //        if (Convert.ToInt32(DB.ExecuteScalar("SELECT Count(1) FROM AD_User_OrgAccess WHERE AD_User_ID = " + AD_User_ID + " AND AD_ORG_ID= " + drLogin[2].ToString() + " AND IsActive='Y'")) < 1)
                        //        {
                        //            deleteRecord = true;
                        //        }
                        //    }
                        //    else //User Role Org Access
                        //    {
                        //        if (Convert.ToInt32(DB.ExecuteScalar("SELECT Count(1) FROM AD_Role_OrgAccess WHERE AD_Role_ID = " + drLogin[0] + " AND AD_ORG_ID= " + drLogin[2].ToString() + " AND IsActive='Y'")) < 1)
                        //        {
                        //            deleteRecord = true;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    deleteRecord = true;
                        //}

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
                + " AND o.IsActive='Y' AND o.IsSummary='N'"
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

        internal static LoginContext GetLoginContext(LoginModel model)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##AD_User_ID", model.Login1Model.AD_User_ID.ToString());
            ctx.SetContext("##AD_User_Name", model.Login1Model.UserName);
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

        internal static LoginContext GetLoginContext(LoginModel model, Ctx ctxLogIn)
        {
            LoginContext ctx = new LoginContext();
            ctx.SetContext("##AD_User_ID", ctxLogIn.GetAD_User_ID().ToString());
            ctx.SetContext("##AD_User_Name", ctxLogIn.GetContext("##AD_User_Name"));
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
    }
}