using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Utility;

using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace VAdvantage.Login
{
    public class LoginProcess
    {
        public LoginProcess(Ctx ctx)
        {
            if (ctx == null)
                throw new ArgumentException("Context missing");
            m_ctx = ctx;
        }	//	Login



        // private Context m_ctx = null;
        private Ctx m_ctx = null;

        /** List of Roles		*/
        private List<KeyNamePair> m_roles = new List<KeyNamePair>();
        /** List of Users for Roles		*/
        private List<int> m_users = new List<int>();
        /** The Current User	*/
        private KeyNamePair m_user = null;
        /** The Current Role	*/
        private KeyNamePair m_role = null;
        /** The Current Org		*/
        private KeyNamePair m_org = null;
        /** Web Store Login		*/
        private X_W_Store m_store = null;





        /// <summary>
        /// Get Roles for the user with email in client with the web store.
        /// If the user does not have roles and the web store has a default role, it will return that.
        /// </summary>
        /// <param name="eMail">email add</param>
        /// <param name="password">password</param>
        /// <param name="W_Store_ID">web store</param>
        /// <returns></returns>
        private KeyNamePair[] GetRolesByEmail(String eMail, String password, int W_Store_ID)
        {
            long start = CommonFunctions.CurrentTimeMillis();
            if (eMail == null || eMail.Length == 0
                || password == null || password.Length == 0
                || W_Store_ID == 0)
            {
                return null;
            }
            //	Cannot use encrypted password
            if (SecureEngine.IsEncrypted(password))
            {
                return null;
            }

            KeyNamePair[] retValue = null;
            List<KeyNamePair> list = new List<KeyNamePair>();
            //
            String sql = "SELECT u.AD_User_ID, r.AD_Role_ID, u.Name "
                + "FROM AD_User u"
                + " INNER JOIN W_Store ws ON (u.AD_Client_ID=ws.AD_Client_ID) "
                + " INNER JOIN AD_Role r ON (ws.AD_Role_ID=r.AD_Role_ID) "
                + "WHERE u.EMail='" + eMail + "'"
                + " AND (u.Password='" + password + "' OR u.Password='" + password + "')"
                + " AND ws.W_Store_ID='" + W_Store_ID + "'"
                + " AND (r.IsActive='Y' OR r.IsActive IS NULL)"
                + " AND u.isActive='Y' AND ws.IsActive='Y'"
                + " AND u.AD_Client_ID=ws.AD_Client_ID "
                + "ORDER BY r.Name";
            m_roles.Clear();
            m_users.Clear();
            IDataReader dr = null;
            try
            {

                //	execute a query
                dr = DataBase.DB.ExecuteReader(sql);

                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                int AD_User_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                m_ctx.SetAD_User_ID(AD_User_ID);
                m_user = new KeyNamePair(AD_User_ID, eMail);
                m_users.Add(AD_User_ID);	//	for role
                //
                int AD_Role_ID = Utility.Util.GetValueOfInt(dr[1].ToString());
                m_ctx.SetAD_Role_ID(AD_Role_ID);
                String Name = dr[2].ToString();
                m_ctx.SetContext("##AD_User_Name", Name);
                if (AD_Role_ID == 0)	//	User is a Sys Admin
                    m_ctx.SetContext("#SysAdmin", "Y");
                KeyNamePair p = new KeyNamePair(AD_Role_ID, Name);
                m_roles.Add(p);
                list.Add(p);

                dr.Close();
                //
                retValue = new KeyNamePair[list.Count];
                retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                retValue = null;
                m_ctx.SetContext("##AD_User_Name", eMail);
            }

            return retValue;
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <returns>user id</returns>
        public int GetAD_User_ID()
        {
            if (m_user != null)
                return m_user.GetKey();
            return -1;
        }	//	getAD_User_ID


        public int GetAD_Role_ID()
        {
            if (m_role != null)
                return m_role.GetKey();
            return -1;
        }	//	getAD_Role_ID



        /// <summary>
        /// Load Preferences into Context for selected client.
        /// <para>
        /// Sets Org info in context and loads relevant field from
        /// - AD_Client/Info,
        /// - C_AcctSchema,
        /// - C_AcctSchema_Elements
        /// - AD_Preference
        /// </para>
        /// Assumes that the context is set for #AD_Client_ID, ##AD_User_ID, #AD_Role_ID
        /// </summary>
        /// <param name="org">org information</param>
        /// <param name="warehouse">optional warehouse information</param>
        /// <param name="timestamp">optional date</param>
        /// <param name="printerName">optional printer info</param>
        /// <returns>AD_Message of error (NoValidAcctInfo) or ""</returns>
        public String LoadPreferences(KeyNamePair org,
            KeyNamePair warehouse, DateTime timestamp, String printerName)
        {
            m_org = org;

            if (m_ctx == null || org == null)
                throw new ArgumentException("Required parameter missing");
            if (m_ctx.GetContext("#AD_Client_ID").Length == 0)
                throw new Exception("Missing Context #AD_Client_ID");
            if (m_ctx.GetContext("##AD_User_ID").Length == 0)
                throw new Exception("Missing Context ##AD_User_ID");
            if (m_ctx.GetContext("#AD_Role_ID").Length == 0)
                throw new Exception("Missing Context #AD_Role_ID");


            //  Org Info - assumes that it is valid
            m_ctx.SetAD_Org_ID(org.GetKey());
            m_ctx.SetContext("#AD_Org_Name", org.GetName());
            Ini.SetProperty(Ini.P_ORG, org.GetName());

            //  Warehouse Info
            if (warehouse != null)
            {
                m_ctx.SetContext("#M_Warehouse_ID", warehouse.GetKey());
                Ini.SetProperty(Ini.P_WAREHOUSE, warehouse.GetName());
            }

            //	Date (default today)
            long today = CommonFunctions.CurrentTimeMillis();
            if (timestamp != null)
                today = CommonFunctions.CurrentTimeMillis(timestamp);
            m_ctx.SetContext("#Date", today.ToString());

            //	Load User/Role Info
            MUser user = MUser.Get(m_ctx, GetAD_User_ID());
            MUserPreference preference = user.GetPreference();
            MRole role = MRole.GetDefault(m_ctx, true);

            //	Optional Printer
            if (printerName == null)
                printerName = "";
            if (printerName.Length == 0 && preference.GetPrinterName() != null)
                printerName = preference.GetPrinterName();
            m_ctx.SetPrinterName(printerName);
            if (preference.GetPrinterName() == null && printerName.Length > 0)
                preference.SetPrinterName(printerName);

            //	Other
            m_ctx.SetAutoCommit(preference.IsAutoCommit());
            m_ctx.SetAutoNew(Ini.IsPropertyBool(Ini.P_A_NEW));
            if (role.IsShowAcct())
                m_ctx.SetContext("#ShowAcct", preference.IsShowAcct());
            else
                m_ctx.SetContext("#ShowAcct", "N");
            m_ctx.SetContext("#ShowTrl", preference.IsShowTrl());
            m_ctx.SetContext("#ShowAdvanced", preference.IsShowAdvanced());

            String retValue = "";
            int AD_Client_ID = m_ctx.GetAD_Client_ID();
            //	int AD_Org_ID =  org.getKey();
            //	int AD_User_ID =  Env.getAD_User_ID (m_ctx);
            int AD_Role_ID = m_ctx.GetAD_Role_ID();

            //	Other Settings
            m_ctx.SetContext("#YYYY", "Y");

            //	AccountSchema Info (first)
            String sql = "SELECT a.C_AcctSchema_ID, a.C_Currency_ID, a.HasAlias, c.ISO_Code, c.StdPrecision, t.AutoArchive "    // Get AutoArchive from Tenant header
                + "FROM C_AcctSchema a"
                + " INNER JOIN AD_ClientInfo ci ON (a.C_AcctSchema_ID=ci.C_AcctSchema1_ID)"
                + " INNER JOIN AD_Client t ON (ci.AD_Client_ID=t.AD_Client_ID)"
                + " INNER JOIN C_Currency c ON (a.C_Currency_ID=c.C_Currency_ID) "
                + "WHERE ci.AD_Client_ID='" + AD_Client_ID + "'";
            IDataReader dr = null;
            try
            {
                int C_AcctSchema_ID = 0;
                dr = DataBase.DB.ExecuteReader(sql);

                if (!dr.Read())
                {
                    //  No Warning for System
                    if (AD_Role_ID != 0)
                        retValue = "NoValidAcctInfo";
                }
                else
                {
                    //	Accounting Info
                    C_AcctSchema_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    m_ctx.SetContext("$C_AcctSchema_ID", C_AcctSchema_ID);
                    m_ctx.SetContext("$C_Currency_ID", Utility.Util.GetValueOfInt(dr[1].ToString()));
                    m_ctx.SetContext("$HasAlias", dr[2].ToString());
                    m_ctx.SetContext("$CurrencyISO", dr[3].ToString());
                    m_ctx.SetStdPrecision(Utility.Util.GetValueOfInt(dr[4].ToString()));
                    m_ctx.SetContext("$AutoArchive", dr[5].ToString());
                }
                dr.Close();

                //	Accounting Elements
                sql = "SELECT ElementType "
                    + "FROM C_AcctSchema_Element "
                    + "WHERE C_AcctSchema_ID='" + C_AcctSchema_ID + "'"
                    + " AND IsActive='Y'";

                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                    m_ctx.SetContext("$Element_" + dr["ElementType"].ToString(), "Y");
                dr.Close();


                //	This reads all relevant window neutral defaults
                //	overwriting superseeded ones.  Window specific is read in Maintain
                sql = "SELECT Attribute, Value, AD_Window_ID "
                    + "FROM AD_Preference "
                    + "WHERE AD_Client_ID IN (0, @#AD_Client_ID@)"
                    + " AND AD_Org_ID IN (0, @#AD_Org_ID@)"
                    + " AND (AD_User_ID IS NULL OR AD_User_ID=0 OR AD_User_ID=@##AD_User_ID@)"
                    + " AND IsActive='Y' "
                    + "ORDER BY Attribute, AD_Client_ID, AD_User_ID DESC, AD_Org_ID";
                //	the last one overwrites - System - Client - User - Org - Window
                sql = Utility.Env.ParseContext(m_ctx, 0, sql, false);
                if (sql.Length == 0)
                { }
                else
                {
                    dr = DataBase.DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string AD_Window_ID = dr[2].ToString();
                        String at = "";
                        if (string.IsNullOrEmpty(AD_Window_ID))
                            at = "P|" + dr[0].ToString();
                        else
                            at = "P" + AD_Window_ID + "|" + dr[0].ToString();
                        String va = dr[1].ToString();
                        m_ctx.SetContext(at, va);
                    }
                    dr.Close();
                }

                //	Default Values
                sql = "SELECT t.TableName, c.ColumnName "
                    + "FROM AD_Column c "
                    + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                    + "WHERE c.IsKey='Y' AND t.IsActive='Y'"
                    + " AND EXISTS (SELECT * FROM AD_Column cc "
                    + " WHERE ColumnName = 'IsDefault' AND t.AD_Table_ID=cc.AD_Table_ID AND cc.IsActive='Y')";

                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                    LoadDefault(dr[0].ToString(), dr[1].ToString());
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            Ini.SaveProperties(Ini.IsClient());
            //	Country
            m_ctx.SetContext("#C_Country_ID", MCountry.GetDefault(m_ctx).GetC_Country_ID());

            m_ctx.SetShowClientOrg(Ini.IsShowClientOrg() ? "Y" : "N");
            m_ctx.SetShowMiniGrid(Ini.GetProperty(Ini.P_Show_Mini_Grid));
            return retValue;
        }	//	loadPreferences


        private void LoadDefault(String TableName, String ColumnName)
        {
            if (TableName.StartsWith("AD_Window")
                || TableName.StartsWith("AD_PrintFormat")
                || TableName.StartsWith("AD_Workflow"))
                return;
            String value = null;
            //
            String sql = "SELECT " + ColumnName + " FROM " + TableName	//	most specific first
                + " WHERE IsDefault='Y' AND IsActive='Y' ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";
            sql = MRole.GetDefault(m_ctx, false).AddAccessSQL(sql, TableName, MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            IDataReader dr = null;
            try
            {

                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    value = dr[0].ToString();
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                return;
            }

            //	Set Context Value
            if (value != null && value.Length != 0)
            {
                if (TableName.Equals("C_DocType"))
                    m_ctx.SetContext("#C_DocTypeTarget_ID", value);
                else
                    m_ctx.SetContext("#" + ColumnName, value);
            }
        }	//	loadDefault


        /// <summary>
        /// Validate Login.
        /// Creates session and calls ModelValidationEngine
        /// </summary>
        /// <param name="org">log-in org</param>
        /// <returns>error message</returns>
        public String ValidateLogin(KeyNamePair org)
        {
            String info = m_user + ",R:" + m_role.ToString() + ",O=" + m_org.ToString();
            int AD_Client_ID = m_ctx.GetAD_Client_ID();
            int AD_Org_ID = org.GetKey();
            int AD_Role_ID = m_ctx.GetAD_Role_ID();
            int AD_User_ID = m_ctx.GetAD_User_ID();
            //
            MSession session = MSession.Get(m_ctx, true);
            if (AD_Client_ID != session.GetAD_Client_ID())
                session.SetAD_Client_ID(AD_Client_ID);
            if (AD_Org_ID != session.GetAD_Org_ID())
                session.SetAD_Org_ID(AD_Org_ID);
            if (AD_Role_ID != session.GetAD_Role_ID())
                session.SetAD_Role_ID(AD_Role_ID);
            //
            String error = ModelValidationEngine.Get().LoginComplete(AD_Client_ID, AD_Org_ID, AD_Role_ID, AD_User_ID);
            if (error != null && error.Length > 0)
            {
                session.SetDescription(error);
                session.Save();
                return error;
            }
            //	Log
            session.Save();
            return null;
        }	//	validateLogin


        public KeyNamePair[] GetRoles(String app_user, String app_pwd)
        {
            return GetRoles(app_user, app_pwd, false, false);
        }   //  login


        /// <summary>
        /// Actual DB login procedure.
        /// </summary>
        /// <param name="app_user">user</param>
        /// <param name="app_pwd">pwd</param>
        /// <param name="force">ignore pwd</param>
        /// <param name="ignore_pwd">If true, indicates that the user had previously authenticated successfully, and therefore
        /// there is no need to check password again.  This differs from the <b>force</b> parameter in that <b>force</b>
        /// will force a login with System Administrator privileges.
        /// </param>
        /// <returns>array or null if in error.</returns>
        private KeyNamePair[] GetRoles(String app_user, String app_pwd, bool force, bool ignore_pwd)
        {
            long start = CommonFunctions.CurrentTimeMillis();
            if (app_user == null)
            {
                return null;
            }

            //	Authenticate


            KeyNamePair[] retValue = null;
            List<KeyNamePair> list = new List<KeyNamePair>();
            //
            StringBuilder sql = new StringBuilder("SELECT u.AD_User_ID, r.AD_Role_ID,r.Name,")
                .Append(" u.ConnectionProfile, u.Password ")	//	4,5
                .Append("FROM AD_User u")
                .Append(" INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive='Y')")
                .Append(" INNER JOIN AD_Role r ON (ur.AD_Role_ID=r.AD_Role_ID AND r.IsActive='Y') ")
                .Append("WHERE COALESCE(u.LDAPUser,u.Name)=@username")		//	#1
                .Append(" AND u.IsActive='Y'")
                .Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE u.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')")
                .Append(" AND EXISTS (SELECT * FROM AD_Client c WHERE r.AD_Client_ID=c.AD_Client_ID AND c.IsActive='Y')");
            if (app_pwd != null)
                sql.Append(" AND (u.Password='" + app_pwd + "' OR u.Password='" + app_pwd + "')");	//  #2/3
            sql.Append(" ORDER BY r.Name");
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@username", app_user);
                //	execute a query
                dr = DataBase.DB.ExecuteReader(sql.ToString(), param);

                if (!dr.Read())		//	no record found
                {
                    if (force)
                    {
                        m_ctx.SetAD_User_ID(0);
                        m_ctx.SetContext("##AD_User_Name", "System (force)");
                        m_ctx.SetContext("##AD_User_Description", "System Forced Login");
                        m_ctx.SetContext("#User_Level", "S  ");  	//	Format 'SCO'
                        m_ctx.SetContext("#User_Client", "0");		//	Format c1, c2, ...
                        m_ctx.SetContext("#User_Org", "0"); 		//	Format o1, o2, ...
                        m_user = new KeyNamePair(0, app_user + " (force)");
                        dr.Close();
                        retValue = new KeyNamePair[] { new KeyNamePair(0, "System Administrator (force)") };
                        return retValue;
                    }
                    else
                    {
                        dr.Close();
                        return null;
                    }
                }

                int AD_User_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                m_ctx.SetAD_User_ID(AD_User_ID);
                m_user = new KeyNamePair(AD_User_ID, app_user);
                m_ctx.SetContext("##AD_User_Name", app_user);

                if (MUser.IsSalesRep(AD_User_ID))
                    m_ctx.SetContext("#SalesRep_ID", AD_User_ID);
                //
                Ini.SetProperty(Ini.P_UID, app_user);

                if (Ini.IsPropertyBool(Ini.P_STORE_PWD))
                    Ini.SetProperty(Ini.P_PWD, app_pwd);


                m_roles.Clear();
                m_users.Clear();
                do	//	read all roles
                {
                    AD_User_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    m_users.Add(AD_User_ID);	//	for role
                    //
                    int AD_Role_ID = Utility.Util.GetValueOfInt(dr[1].ToString());
                    if (AD_Role_ID == 0)	//	User is a Sys Admin
                        m_ctx.SetContext("#SysAdmin", "Y");
                    String Name = dr[2].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Role_ID, Name);
                    m_roles.Add(p);
                    list.Add(p);
                }
                while (dr.Read());

                dr.Close();
                //
                retValue = new KeyNamePair[list.Count];
                retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                retValue = null;
            }
            long ms = CommonFunctions.CurrentTimeMillis() - start;

            return retValue;
        }	//	getRoles


        /// <summary>
        /// Load Clients.
        /// <para>
        /// Sets Role info in context and loads its clients
        /// </para>
        /// </summary>
        /// <param name="role"> role information</param>
        /// <returns>list of valid client KeyNodePairs or null if in error</returns>
        public KeyNamePair[] GetClients(KeyNamePair role)
        {
            if (role == null)
                throw new Exception("Role missing");
            m_role = role;
            //	Web Store Login
            if (m_store != null)
                return new KeyNamePair[] { new KeyNamePair(m_store.GetAD_Client_ID(), m_store.GetName() + " Tenant") };

            //	Set User for Role
            int AD_Role_ID = role.GetKey();
            for (int i = 0; i < m_roles.Count; i++)
            {
                if (AD_Role_ID == m_roles[i].GetKey())
                {
                    int AD_User_ID = m_users[i];
                    m_ctx.SetAD_User_ID(AD_User_ID);
                    if (MUser.IsSalesRep(AD_User_ID))
                        m_ctx.SetContext("#SalesRep_ID", AD_User_ID);
                    m_user = new KeyNamePair(AD_User_ID, m_user.GetName());
                    break;
                }
            }

            List<KeyNamePair> list = new List<KeyNamePair>();
            KeyNamePair[] retValue = null;
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
                param[0] = new SqlParameter("@roleid", role.GetKey());

                dr = DataBase.DB.ExecuteReader(sql, param);
                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                //  Role Info
                m_ctx.SetAD_Role_ID(role.GetKey());
                m_ctx.SetContext("#AD_Role_Name", role.GetName());
                Ini.SetProperty(Ini.P_ROLE, role.GetName());
                //	User Level
                m_ctx.SetContext("#User_Level", dr[0].ToString());  	//	Format 'SCO'

                //  load Clients
                do
                {
                    int AD_Client_ID = Utility.Util.GetValueOfInt(dr[2].ToString());
                    String Name = dr[3].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Client_ID, Name);
                    list.Add(p);
                }
                while (dr.Read());
                dr.Close();
                //
                retValue = new KeyNamePair[list.Count];
                retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                retValue = null;
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public KeyNamePair[] GetOrgs(KeyNamePair client)
        {
            if (client == null)
                throw new ArgumentException("Client missing");
            //	Web Store Login
            if (m_store != null)
                return new KeyNamePair[] { new KeyNamePair(m_store.GetAD_Org_ID(), m_store.GetName() + " Org") };

            if (m_ctx.GetContext("#AD_Role_ID").Length == 0)	//	could be number 0
                throw new Exception("Missing Context #AD_Role_ID");

            int AD_Role_ID = m_ctx.GetAD_Role_ID();
            int AD_User_ID = m_ctx.GetAD_User_ID();
            //	s_log.fine("Client: " + client.toStringX() + ", AD_Role_ID=" + AD_Role_ID);

            //	get Client details for role
            List<KeyNamePair> list = new List<KeyNamePair>();
            KeyNamePair[] retValue = null;
            //
            String sql = "SELECT o.AD_Org_ID,o.Name,o.IsSummary "	//	1..3
                + "FROM AD_Role r, AD_Client c"
                + " INNER JOIN AD_Org o ON (c.AD_Client_ID=o.AD_Client_ID OR o.AD_Org_ID=0) "
                + "WHERE r.AD_Role_ID='" + AD_Role_ID + "'" 	//	#1
                + " AND c.AD_Client_ID='" + client.GetKey() + "'"	//	#2
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
            try
            {

                dr = DataBase.DB.ExecuteReader(sql);
                //  load Orgs
                while (dr.Read())
                {
                    int AD_Org_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    bool summary = "Y".Equals(dr[2].ToString());
                    if (summary)
                    {
                        if (role == null)
                            role = MRole.Get(m_ctx, AD_Role_ID, AD_User_ID, false);
                        GetOrgsAddSummary(list, AD_Org_ID, Name, role);
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
                retValue = new KeyNamePair[list.Count];
                retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                retValue = null;
            }

            //	No Orgs
            if (retValue == null || retValue.Length == 0)
            {
                return null;
            }

            //  Client Info
            m_ctx.SetContext("#AD_Client_ID", client.GetKey());
            m_ctx.SetContext("#AD_Client_Name", client.GetName());
            Ini.SetProperty(Ini.P_CLIENT, client.GetName());
            return retValue;
        }   //  getOrgs


        /// <summary>
        /// Get Orgs - Add Summary Org
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="Summary_Org_ID">summary org</param>
        /// <param name="Summary_Name">name</param>
        /// <param name="role"></param>
        private void GetOrgsAddSummary(List<KeyNamePair> list, int Summary_Org_ID, String Summary_Name, MRole role)
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
            MTree tree = MTree.Get(m_ctx, role.GetAD_Tree_Org_ID(), null);
            String sql = "SELECT AD_Client_ID, AD_Org_ID, Name, IsSummary FROM AD_Org "
                + "WHERE IsActive='Y' AND AD_Org_ID IN (SELECT Node_ID FROM "
                + tree.GetNodeTableName()
                + " WHERE AD_Tree_ID='" + tree.GetAD_Tree_ID() + "' AND Parent_ID='" + Summary_Org_ID + "' AND IsActive='Y') "
                + "ORDER BY Name";

            IDataReader dr = DataBase.DB.ExecuteReader(sql);
            try
            {
                while (dr.Read())
                {
                    //	int AD_Client_ID = rs.getInt(1);
                    int AD_Org_ID = Utility.Util.GetValueOfInt(dr[1].ToString());
                    String Name = dr[2].ToString();
                    bool summary = "Y".Equals(dr[3].ToString());
                    //
                    if (summary)
                        GetOrgsAddSummary(list, AD_Org_ID, Name, role);
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
        }	//	getOrgAddSummary


        /// <summary>
        ///  Load Warehouses
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public KeyNamePair[] GetWarehouses(KeyNamePair org)
        {
            ;
            if (org == null)
                throw new Exception("Org missing");
            m_org = org;
            if (m_store != null)
                return new KeyNamePair[] { new KeyNamePair(m_store.GetM_Warehouse_ID(), m_store.GetName() + " Warehouse") };

            //	s_log.info("loadWarehouses - Org: " + org.toStringX());

            List<KeyNamePair> list = new List<KeyNamePair>();
            KeyNamePair[] retValue = null;
            String sql = "SELECT M_Warehouse_ID, Name FROM M_Warehouse "
                + "WHERE AD_Org_ID=@p1 AND IsActive='Y' "
                + "ORDER BY Name";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@p1", org.GetKey());
                dr = DataBase.DB.ExecuteReader(sql, param);
                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                //  load Warehouses
                do
                {
                    int AD_Warehouse_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Warehouse_ID, Name);
                    list.Add(p);
                }
                while (dr.Read());

                dr.Close();
                //
                retValue = new KeyNamePair[list.Count];
                retValue = list.ToArray();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
                retValue = null;
            }

            return retValue;
        }   //  getWarehouses




        /* HTML5 */

        /// <summary>
        /// Load Preferences into Context for selected client.
        /// <para>
        /// Sets Org info in context and loads relevant field from
        /// - AD_Client/Info,
        /// - C_AcctSchema,
        /// - C_AcctSchema_Elements
        /// - AD_Preference
        /// </para>
        /// Assumes that the context is set for #AD_Client_ID, ##AD_User_ID, #AD_Role_ID
        /// </summary>
        /// <param name="org">org information</param>
        /// <param name="warehouse">optional warehouse information</param>
        /// <param name="timestamp">optional date</param>
        /// <param name="printerName">optional printer info</param>
        /// <returns>AD_Message of error (NoValidAcctInfo) or ""</returns>
        public String LoadPreferences(string date, String printerName)
        {
            if (m_ctx.GetContext("#AD_Client_ID").Length == 0)
                throw new Exception("Missing Context #AD_Client_ID");
            if (m_ctx.GetContext("##AD_User_ID").Length == 0)
                throw new Exception("Missing Context ##AD_User_ID");
            if (m_ctx.GetContext("#AD_Role_ID").Length == 0)
                throw new Exception("Missing Context #AD_Role_ID");

            string dateS = m_ctx.GetContext("#Date");

            DateTime dt = DateTime.Now;
            long today = CommonFunctions.CurrentTimeMillis();
            if (DateTime.TryParse(dateS, out dt))
            {
                today = CommonFunctions.CurrentTimeMillis(dt);
            }

            m_ctx.SetContext("#Date", today.ToString());

            //	Load User/Role Infos
            MUser user = MUser.Get(m_ctx, m_ctx.GetAD_User_ID());

            MUserPreference preference = user.GetPreference();
            MRole role = MRole.GetDefault(m_ctx);

            //	Optional Printer
            if (printerName == null)
                printerName = "";
            if (printerName.Length == 0 && preference.GetPrinterName() != null)
                printerName = preference.GetPrinterName();
            m_ctx.SetPrinterName(printerName);
            if (preference.GetPrinterName() == null && printerName.Length > 0)
                preference.SetPrinterName(printerName);

            //	Other
            m_ctx.SetAutoCommit(preference.IsAutoCommit());
            m_ctx.SetAutoNew(Ini.IsPropertyBool(Ini.P_A_NEW));
            if (role.IsShowAcct())
                m_ctx.SetContext("#ShowAcct", preference.IsShowAcct());
            else
                m_ctx.SetContext("#ShowAcct", "N");
            m_ctx.SetContext("#ShowTrl", preference.IsShowTrl());
            m_ctx.SetContext("#ShowAdvanced", preference.IsShowAdvanced());

            String retValue = "";
            int AD_Client_ID = m_ctx.GetAD_Client_ID();
            //	int AD_Org_ID =  org.getKey();
            //	int AD_User_ID =  Env.getAD_User_ID (m_ctx);
            int AD_Role_ID = m_ctx.GetAD_Role_ID();

            //	Other Settings
            m_ctx.SetContext("#YYYY", "Y");


            //LoadSysConfig();

            string sql = "";
            IDataReader dr = null;
            bool checkNonItem = true;           // to identify that on Tenant there exista new column "IsAllowNonItem".
            //	AccountSchema Info (first)
            try
            {
                sql = "SELECT a.C_AcctSchema_ID, a.C_Currency_ID, a.HasAlias, c.ISO_Code, c.StdPrecision, t.AutoArchive, t.IsAllowNonItem "  // 6. Get "Alloe Non Item on Ship/Receipt" from Tenant header
                    + "FROM C_AcctSchema a"
                    + " INNER JOIN AD_ClientInfo ci ON (a.C_AcctSchema_ID=ci.C_AcctSchema1_ID)"
                    + " INNER JOIN AD_Client t ON (ci.AD_Client_ID=t.AD_Client_ID)"
                    + " INNER JOIN C_Currency c ON (a.C_Currency_ID=c.C_Currency_ID) "
                    + "WHERE ci.AD_Client_ID='" + AD_Client_ID + "'";

                dr = DataBase.DB.ExecuteReader(sql);
            }
            catch
            {
                checkNonItem = false;
                sql = "SELECT a.C_AcctSchema_ID, a.C_Currency_ID, a.HasAlias, c.ISO_Code, c.StdPrecision, t.AutoArchive "       // 5. Get AutoArchive from Tenant header
                    + "FROM C_AcctSchema a"
                    + " INNER JOIN AD_ClientInfo ci ON (a.C_AcctSchema_ID=ci.C_AcctSchema1_ID)"
                    + " INNER JOIN AD_Client t ON (ci.AD_Client_ID=t.AD_Client_ID)"
                    + " INNER JOIN C_Currency c ON (a.C_Currency_ID=c.C_Currency_ID) "
                    + "WHERE ci.AD_Client_ID='" + AD_Client_ID + "'";
            }

            try
            {
                int C_AcctSchema_ID = 0;
                if (!checkNonItem)
                {
                    dr = DataBase.DB.ExecuteReader(sql);
                }

                if (!dr.Read())
                {
                    //  No Warning for System
                    if (AD_Role_ID != 0)
                        retValue = "NoValidAcctInfo";
                }
                else
                {
                    //	Accounting Info
                    C_AcctSchema_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    m_ctx.SetContext("$C_AcctSchema_ID", C_AcctSchema_ID);
                    m_ctx.SetContext("$C_Currency_ID", Utility.Util.GetValueOfInt(dr[1].ToString()));
                    m_ctx.SetContext("$HasAlias", dr[2].ToString());
                    m_ctx.SetContext("$CurrencyISO", dr[3].ToString());
                    m_ctx.SetStdPrecision(Utility.Util.GetValueOfInt(dr[4].ToString()));
                    m_ctx.SetContext("$AutoArchive", dr[5].ToString());

                    // 
                    if (checkNonItem)       // Get "Alloe Non Item on Ship/Receipt" from Tenant header if exist.
                    {
                        m_ctx.SetContext("$AllowNonItem", dr[6].ToString());
                    }
                }
                dr.Close();

                //	Accounting Elements
                sql = "SELECT ElementType "
                    + "FROM C_AcctSchema_Element "
                    + "WHERE C_AcctSchema_ID='" + C_AcctSchema_ID + "'"
                    + " AND IsActive='Y'";

                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                    m_ctx.SetContext("$Element_" + dr["ElementType"].ToString(), "Y");
                dr.Close();


                //	This reads all relevant window neutral defaults
                //	overwriting superseeded ones.  Window specific is read in Maintain
                sql = "SELECT Attribute, Value, AD_Window_ID "
                    + "FROM AD_Preference "
                    + "WHERE AD_Client_ID IN (0, @#AD_Client_ID@)"
                    + " AND AD_Org_ID IN (0, @#AD_Org_ID@)"
                    + " AND (AD_User_ID IS NULL OR AD_User_ID=0 OR AD_User_ID=@##AD_User_ID@)"
                    + " AND IsActive='Y' "
                    + "ORDER BY Attribute, AD_Client_ID, AD_User_ID DESC, AD_Org_ID";
                //	the last one overwrites - System - Client - User - Org - Window
                sql = Utility.Env.ParseContext(m_ctx, 0, sql, false);
                if (sql.Length == 0)
                { }
                else
                {
                    dr = DataBase.DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string AD_Window_ID = dr[2].ToString();
                        String at = "";
                        if (string.IsNullOrEmpty(AD_Window_ID))
                            at = "P|" + dr[0].ToString();
                        else
                            at = "P" + AD_Window_ID + "|" + dr[0].ToString();
                        String va = dr[1].ToString();
                        m_ctx.SetContext(at, va);
                    }
                    dr.Close();
                }

                //	Default Values
                sql = "SELECT t.TableName, c.ColumnName "
                    + "FROM AD_Column c "
                    + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                    + "WHERE c.IsKey='Y' AND t.IsActive='Y'"
                    + " AND EXISTS (SELECT * FROM AD_Column cc "
                    + " WHERE ColumnName = 'IsDefault' AND t.AD_Table_ID=cc.AD_Table_ID AND cc.IsActive='Y')";

                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                    LoadDefault(dr[0].ToString(), dr[1].ToString());
                dr.Close();


            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            //Ini.SaveProperties(Ini.IsClient());
            //	Country
            m_ctx.SetContext("#C_Country_ID", MCountry.GetDefault(m_ctx).GetC_Country_ID());

            m_ctx.SetShowClientOrg(Ini.IsShowClientOrg() ? "Y" : "N");
            m_ctx.SetShowMiniGrid(Ini.GetProperty(Ini.P_Show_Mini_Grid));
            return retValue;
        }	//	loadPreferences

        /// <summary>
        /// Load System Config
        /// </summary>
        public void LoadSysConfig()
        {
            //	Report Page Size Element
            m_ctx.SetContext("#REPORT_PAGE_SIZE", "500");
            string sql = "SELECT NAME, VALUE FROM AD_SysConfig WHERE NAME = 'REPORT_PAGE_SIZE'";
            IDataReader dr = DataBase.DB.ExecuteReader(sql);
            while (dr.Read())
                if (!string.IsNullOrEmpty(dr[1].ToString()))
                {
                    Regex regex = new Regex(@"^[1-9]\d*$");
                    if (regex.IsMatch(dr[1].ToString()))
                        m_ctx.SetContext("#REPORT_PAGE_SIZE", (dr[1].ToString()));
                }
            dr.Close();

            //	Bulk Report Download
            m_ctx.SetContext("#BULK_REPORT_DOWNLOAD", "N");
            sql = "SELECT NAME, VALUE FROM AD_SysConfig WHERE NAME = 'BULK_REPORT_DOWNLOAD'";
            dr = DataBase.DB.ExecuteReader(sql);
            while (dr.Read())
                if (!string.IsNullOrEmpty(dr[1].ToString()))
                {
                    Regex regex = new Regex(@"Y|N");
                    if (regex.IsMatch(dr[1].ToString()))
                        m_ctx.SetContext("#BULK_REPORT_DOWNLOAD", (dr[1].ToString()));
                }
            dr.Close();

            // Set Default Value of System Config in Context
            sql = "SELECT NAME, VALUE FROM AD_SysConfig WHERE ISACTIVE = 'Y' AND NAME NOT IN ('REPORT_PAGE_SIZE' , 'BULK_REPORT_DOWNLOAD')";
            dr = DataBase.DB.ExecuteReader(sql);
            while (dr.Read())
                if (!string.IsNullOrEmpty(dr[1].ToString()))
                {
                    m_ctx.SetContext("#" + dr[0].ToString(), (dr[1].ToString()));
                }
            dr.Close();


            //	BULK Location name load
            m_ctx.SetContext("#LOCATION_NAME_BULK_REQUEST", "Y");
            sql = "SELECT NAME, VALUE FROM AD_SysConfig WHERE NAME = 'LOCATION_NAME_BULK_REQUEST'";
            DataSet ds = DataBase.DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0][1].ToString()))
                {
                    Regex regex = new Regex(@"Y|N");
                    if (regex.IsMatch(ds.Tables[0].Rows[0][1].ToString()))
                        m_ctx.SetContext("#LOCATION_NAME_BULK_REQUEST", (ds.Tables[0].Rows[0][1].ToString()));
                }
            }

        }

    }
}