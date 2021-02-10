using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Utility;
using System.IO;
using System.Data;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    public sealed class MCloudSetup
    {
        static private List<string> lstTableName = null;
        static private bool ISTENATRUNNINGFORERP = false;

        public static void GetAllTable()
        {
            if (lstTableName.Count == 0)
            {
                lstTableName = new List<string>();
                DataSet ds = DB.ExecuteDataset("select tablename from vaf_tableview where isactive='Y'");

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 350)
                    {
                        ISTENATRUNNINGFORERP = true;
                    }
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstTableName.Add(Convert.ToString(ds.Tables[0].Rows[i]["TABLENAME"]));
                    }

                }
            }

        }


        /// <summary>
        /// Constructor
        /// </summary>
        public MCloudSetup(Ctx ctx, int WindowNo)
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
            m_ctx = ctx;	//	copy
            m_lang = Env.GetVAF_Language(m_ctx);
            m_WindowNo = WindowNo;
        }   //  MSetup

        /**	Logger			*/
        internal VLogger log = null;

        private Trx m_trx = Trx.Get("Setup");
        private Ctx m_ctx;
        private String m_lang;
        private int m_WindowNo;
        private StringBuilder m_info;
        //
        private String m_clientName;
        //	private String          m_orgName;
        //
        private String m_stdColumns = "VAF_Client_ID,VAF_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy";
        private String m_stdValues;
        private String m_stdValuesOrg;
        //
        private NaturalAccountMap<String, MElementValue> m_nap = null;
        //
        private MVAFClient m_client;
        private MVAFOrg m_org;
        private MVABAccountBook m_as;
        //
        private int VAF_UserContact_ID;
        private String VAF_UserContact_Name;
        private int VAF_UserContact_U_ID;
        private String VAF_UserContact_U_Name;
        private MVABCalendar m_calendar;
        private int m_VAF_TreeInfo_Account_ID;
        private int VAB_ProjectCycle_ID;
        //
        private bool m_hasProject = false;
        private bool m_hasMCampaign = false;
        private bool m_hasSRegion = false;
        /** Account Creation OK		*/
        private bool m_accountsOK = false;


        /// <summary>
        /// Create new Client/Tenant
        /// </summary>
        /// <param name="bp">optional bp</param>
        /// <param name="clientName">optional client</param>
        /// <returns>info</returns>
        public bool CreateClient(String clientName, String orgName, String userClient, String userOrg)
        {
            log.Info(clientName);
            m_trx.Start();

            //  info header
            m_info = new StringBuilder();
            //  Standarc columns
            String name = null;
            String sql = null;
            int no = 0;

            /**
             *  Create Client
             */
            name = clientName;
            if (name == null || name.Length == 0)
                name = "newClient";
            m_clientName = name;
            m_client = new MVAFClient(m_ctx, 0, true, m_trx);
            m_client.SetValue(m_clientName);
            m_client.SetName(m_clientName);
            if (!m_client.Save())
            {
                String err = "Client NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            int VAF_Client_ID = m_client.GetVAF_Client_ID();
            m_ctx.SetContext(m_WindowNo, "VAF_Client_ID", VAF_Client_ID);
            m_ctx.SetContext("#VAF_Client_ID", VAF_Client_ID);

            //	Standard Values
            m_stdValues = VAF_Client_ID.ToString() + ",0,'Y',SysDate,0,SysDate,0";
            //  Info - Client
            m_info.Append(Msg.Translate(m_lang, "VAF_Client_ID")).Append("=").Append(name).Append("\n");

            //	Setup Sequences
            if (!MVAFRecordSeq.CheckClientSequences(m_ctx, VAF_Client_ID, m_trx))
            {
                String err = "Sequences NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }

            //  Trees and Client Info
            if (!m_client.SetupClientInfo(m_lang))
            {
                String err = "Client Info NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_VAF_TreeInfo_Account_ID = m_client.GetSetup_VAF_TreeInfo_Account_ID();

            /**
             *  Create Org
             */
            name = orgName;
            if (name == null || name.Length == 0)
                name = "newOrg";
            m_org = new MVAFOrg(m_client, name);
            if (!m_org.Save())
            {
                String err = "Organization NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_ctx.SetContext(m_WindowNo, "VAF_Org_ID", GetVAF_Org_ID());
            m_ctx.SetVAF_Org_ID(GetVAF_Org_ID());
            m_stdValuesOrg = VAF_Client_ID + "," + GetVAF_Org_ID() + ",'Y',SysDate,0,SysDate,0";
            //  Info
            m_info.Append(Msg.Translate(m_lang, "VAF_Org_ID")).Append("=").Append(name).Append("\n");

            /**
             *  Create Roles
             *  - Admin
             *  - User
             */
            name = m_clientName + " Admin";
            MVAFRole admin = new MVAFRole(m_ctx, 0, m_trx);
            admin.SetClientOrg(m_client);
            admin.SetName(name);
            admin.SetUserLevel(MVAFRole.USERLEVEL_ClientPlusOrganization);
            admin.SetPreferenceType(MVAFRole.PREFERENCETYPE_Client);
            admin.SetIsShowAcct(true);
            if (!admin.Save())
            {
                String err = "Admin Role A NOT inserted";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_ctx.SetContext("#Admin_Role_ID", admin.GetVAF_Role_ID());
            //	OrgAccess x, 0
            MVAFRoleOrgRights adminClientAccess = new MVAFRoleOrgRights(admin, 0);
            if (!adminClientAccess.Save())
                log.Log(Level.SEVERE, "Admin Role_OrgAccess 0 NOT created");
            //  OrgAccess x,y
            MVAFRoleOrgRights adminOrgAccess = new MVAFRoleOrgRights(admin, m_org.GetVAF_Org_ID());
            if (!adminOrgAccess.Save())
                log.Log(Level.SEVERE, "Admin Role_OrgAccess NOT created");

            //  Info - Admin Role
            m_info.Append(Msg.Translate(m_lang, "VAF_Role_ID")).Append("=").Append(name).Append("\n");


            MVAFRole user = new MVAFRole(m_ctx, 0, m_trx);
            if (userOrg != null && userOrg.Length > 0)                  //////////////////stop UserOrgCreation
            {
                name = m_clientName + " User";

                user.SetClientOrg(m_client);
                user.SetName(name);
                if (!user.Save())
                {
                    String err = "User Role A NOT inserted";
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                    m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                    return false;
                }
                //  OrgAccess x,y
                MVAFRoleOrgRights userOrgAccess = new MVAFRoleOrgRights(user, m_org.GetVAF_Org_ID());
                if (!userOrgAccess.Save())
                    log.Log(Level.SEVERE, "User Role_OrgAccess NOT created");
            }
            //  Info - Client Role
            m_info.Append(Msg.Translate(m_lang, "VAF_Role_ID")).Append("=").Append(name).Append("\n");

            /**
             *  Create Users
             *  - Client
             *  - Org
             */
            name = userClient;
            if (name == null || name.Length == 0)
                name = m_clientName + "Client";
            VAF_UserContact_ID = GetNextID(VAF_Client_ID, "VAF_UserContact");
            ///////////
            m_ctx.SetContext("#VAF_UserContact_A_ID", VAF_UserContact_ID);
            //////////////
            VAF_UserContact_Name = name;
            name = CoreLibrary.DataBase.DB.TO_STRING(name);
            sql = "INSERT INTO VAF_UserContact(" + m_stdColumns + ",VAF_UserContact_ID,"
                + " Value,Name,Description,Password)"
                + " VALUES (" + m_stdValuesOrg + "," + VAF_UserContact_ID + ","
                + name + "," + name + "," + name + "," + name + ")";
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
            if (no != 1)
            {
                String err = "Admin User NOT inserted - " + VAF_UserContact_Name;
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                return false;
            }
            //  Info
            m_info.Append(Msg.Translate(m_lang, "VAF_UserContact_ID")).Append("=").Append(VAF_UserContact_Name).Append("/").Append(VAF_UserContact_Name).Append("\n");


            if (userOrg != null && userOrg.Length > 0)                  //////////////////stop UserOrgCreation
            {
                name = userOrg;
                if (name == null || name.Length == 0)
                    name = m_clientName + "Org";
                VAF_UserContact_U_ID = GetNextID(VAF_Client_ID, "VAF_UserContact");

                ////////////////////////////
                m_ctx.SetContext("#VAF_UserContact_U_ID", VAF_UserContact_U_ID);
                ////////////////////////////

                VAF_UserContact_U_Name = name;
                name = CoreLibrary.DataBase.DB.TO_STRING(name);
                sql = "INSERT INTO VAF_UserContact(" + m_stdColumns + ",VAF_UserContact_ID,"
                    + "Value,Name,Description,Password)"
                    + " VALUES (" + m_stdValuesOrg + "," + VAF_UserContact_U_ID + ","
                    + name + "," + name + "," + name + "," + name + ")";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                {
                    String err = "Org User NOT inserted - " + VAF_UserContact_U_Name;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
                //  Info
                m_info.Append(Msg.Translate(m_lang, "VAF_UserContact_ID")).Append("=").Append(VAF_UserContact_U_Name).Append("/").Append(VAF_UserContact_U_Name).Append("\n");


                sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                    + " VALUES (" + m_stdValues + "," + VAF_UserContact_ID + "," + user.GetVAF_Role_ID() + ")";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "UserRole ClientUser+User NOT inserted");
                //  OrgUser             - User
                sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                    + " VALUES (" + m_stdValues + "," + VAF_UserContact_U_ID + "," + user.GetVAF_Role_ID() + ")";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "UserRole OrgUser+Org NOT inserted");

            }
            else
            {
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
            }
            /**
             *  Create User-Role
             */
            //  ClientUser          - Admin & User
            sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                + " VALUES (" + m_stdValues + "," + VAF_UserContact_ID + "," + admin.GetVAF_Role_ID() + ")";
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
            if (no != 1)
                log.Log(Level.SEVERE, "UserRole ClientUser+Admin NOT inserted");

            //	Processors
            MVA_AccountHanlder ap = new MVA_AccountHanlder(m_client, VAF_UserContact_ID);
            ap.Save();

            MRequestProcessor rp = new MRequestProcessor(m_client, VAF_UserContact_ID);
            rp.Save();

            log.Info("fini");
            return true;
        }   //  createClient

        public bool CreateClient(String clientName, String orgName, String userClient, String userOrg, string adminUserPwd)
        {
            log.Info(clientName);
            m_trx.Start();

            //  info header
            m_info = new StringBuilder();
            //  Standarc columns
            String name = null;
            String sql = null;
            int no = 0;
            GetAllTable();
            /**
             *  Create Client
             */
            name = clientName;
            if (name == null || name.Length == 0)
                name = "newClient";
            m_clientName = name;
            m_client = new MVAFClient(m_ctx, 0, true, m_trx);
            m_client.SetValue(m_clientName);
            m_client.SetName(m_clientName);
            m_client.SetIsPostImmediate(true);
            m_client.SetIsCostImmediate(true);
            if (!m_client.Save())
            {
                String err = "Client NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            int VAF_Client_ID = m_client.GetVAF_Client_ID();
            m_ctx.SetContext(m_WindowNo, "VAF_Client_ID", VAF_Client_ID);
            m_ctx.SetContext("#VAF_Client_ID", VAF_Client_ID);

            //	Standard Values
            m_stdValues = VAF_Client_ID.ToString() + ",0,'Y',SysDate,0,SysDate,0";
            //  Info - Client
            m_info.Append(Msg.Translate(m_lang, "VAF_Client_ID")).Append("=").Append(name).Append("\n");

            //	Setup Sequences
            if (!MVAFRecordSeq.CheckClientSequences(m_ctx, VAF_Client_ID, m_trx))
            {
                String err = "Sequences NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }

            //  Trees and Client Info
            if (!m_client.SetupClientInfo(m_lang) && ISTENATRUNNINGFORERP)
            {
                String err = "Client Info NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_VAF_TreeInfo_Account_ID = m_client.GetSetup_VAF_TreeInfo_Account_ID();

            /**
             *  Create Org
             */
            name = orgName;
            if (name == null || name.Length == 0)
                name = "newOrg";
            m_org = new MVAFOrg(m_client, name);
            m_org.SetIsLegalEntity(true);
            if (!m_org.Save())
            {
                String err = "Organization NOT created";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_ctx.SetContext(m_WindowNo, "VAF_Org_ID", GetVAF_Org_ID());
            m_ctx.SetVAF_Org_ID(GetVAF_Org_ID());
            m_stdValuesOrg = VAF_Client_ID + "," + GetVAF_Org_ID() + ",'Y',SysDate,0,SysDate,0";
            //  Info
            m_info.Append(Msg.Translate(m_lang, "VAF_Org_ID")).Append("=").Append(name).Append("\n");

            /**
             *  Create Roles
             *  - Admin
             *  - User
             */
            name = m_clientName + " Admin";
            MVAFRole admin = new MVAFRole(m_ctx, 0, m_trx);
            admin.SetClientOrg(m_client);
            admin.SetName(name);
            admin.SetUserLevel(MVAFRole.USERLEVEL_ClientPlusOrganization);
            admin.SetPreferenceType(MVAFRole.PREFERENCETYPE_Client);
            admin.SetIsShowAcct(true);
            admin.SetIsAdministrator(true);
            admin.SetIsManual(false);
            if (!admin.Save())
            {
                String err = "Admin Role A NOT inserted";
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                return false;
            }
            m_ctx.SetContext("#Admin_Role_ID", admin.GetVAF_Role_ID());
            //	OrgAccess x, 0
            MVAFRoleOrgRights adminClientAccess = new MVAFRoleOrgRights(admin, 0);
            if (!adminClientAccess.Save())
                log.Log(Level.SEVERE, "Admin Role_OrgAccess 0 NOT created");
            //  OrgAccess x,y
            MVAFRoleOrgRights adminOrgAccess = new MVAFRoleOrgRights(admin, m_org.GetVAF_Org_ID());
            if (!adminOrgAccess.Save())
                log.Log(Level.SEVERE, "Admin Role_OrgAccess NOT created");

            //  Info - Admin Role
            m_info.Append(Msg.Translate(m_lang, "VAF_Role_ID")).Append("=").Append(name).Append("\n");


            MVAFRole user = new MVAFRole(m_ctx, 0, m_trx);
            if (userOrg != null && userOrg.Length > 0)                  //////////////////stop UserOrgCreation
            {
                name = m_clientName + " User";

                user.SetClientOrg(m_client);
                user.SetName(name);
                if (!user.Save())
                {
                    String err = "User Role A NOT inserted";
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                    m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
                    return false;
                }
                //  OrgAccess x,y
                MVAFRoleOrgRights userOrgAccess = new MVAFRoleOrgRights(user, m_org.GetVAF_Org_ID());
                if (!userOrgAccess.Save())
                    log.Log(Level.SEVERE, "User Role_OrgAccess NOT created");
            }
            //  Info - Client Role
            m_info.Append(Msg.Translate(m_lang, "VAF_Role_ID")).Append("=").Append(name).Append("\n");

            /**
             *  Create Users
             *  - Client
             *  - Org
             */
            name = userClient;
            if (name == null || name.Length == 0)
                name = m_clientName + "Client";
            VAF_UserContact_ID = GetNextID(VAF_Client_ID, "VAF_UserContact");
            ///////////
            m_ctx.SetContext("#VAF_UserContact_A_ID", VAF_UserContact_ID);
            //////////////
            VAF_UserContact_Name = name;
            name = CoreLibrary.DataBase.DB.TO_STRING(name);
            ////sql = "INSERT INTO VAF_UserContact(" + m_stdColumns + ",VAF_UserContact_ID,"
            ////    + " Value,Name,Description,Password)"
            ////    + " VALUES (" + m_stdValuesOrg + "," + VAF_UserContact_ID + ","         // change for genrate new password for admin user
            ////    + name + "," + name + "," + name + "," + name + ")";            // other than name

            sql = "INSERT INTO VAF_UserContact(" + m_stdColumns + ",VAF_UserContact_ID,"
                + " Value,Name,Description,Password,IsLoginUser)"
                + " VALUES (" + m_stdValuesOrg + "," + VAF_UserContact_ID + ","
                + name + "," + name + "," + name + ",'" + adminUserPwd + "','Y')";
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
            if (no != 1)
            {
                String err = "Admin User NOT inserted - " + VAF_UserContact_Name;
                log.Log(Level.SEVERE, err);
                m_info.Append(err);
                m_trx.Rollback();
                m_trx.Close();
                m_ctx.SetContext("#VAF_UserContact_A_ID", 0);
                return false;
            }

            //Save Default Login Settings for Admin User
            //string str =
            SetupDefaultLogin(m_trx, m_client.GetVAF_Client_ID(), admin.GetVAF_Role_ID(), m_org.GetVAF_Org_ID(), VAF_UserContact_ID, 0);
            
            
            //  Info
            m_info.Append(Msg.Translate(m_lang, "VAF_UserContact_ID")).Append("=").Append(VAF_UserContact_Name).Append("/").Append(VAF_UserContact_Name).Append("\n");


            if (userOrg != null && userOrg.Length > 0)                  //////////////////stop UserOrgCreation
            {
                name = userOrg;
                if (name == null || name.Length == 0)
                    name = m_clientName + "Org";
                VAF_UserContact_U_ID = GetNextID(VAF_Client_ID, "VAF_UserContact");

                ////////////////////////////
                m_ctx.SetContext("#VAF_UserContact_U_ID", VAF_UserContact_U_ID);
                ////////////////////////////

                VAF_UserContact_U_Name = name;
                name = CoreLibrary.DataBase.DB.TO_STRING(name);
                sql = "INSERT INTO VAF_UserContact(" + m_stdColumns + ",VAF_UserContact_ID,"
                    + "Value,Name,Description,Password,IsLoginUser)"
                    + " VALUES (" + m_stdValuesOrg + "," + VAF_UserContact_U_ID + ","
                    + name + "," + name + "," + name + "," + name + ",'Y')";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                {
                    String err = "Org User NOT inserted - " + VAF_UserContact_U_Name;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }

                //Save Default Login Settings for Org User
                //str =
                SetupDefaultLogin(m_trx, m_client.GetVAF_Client_ID(), user.GetVAF_Role_ID(), m_org.GetVAF_Org_ID(), VAF_UserContact_U_ID, 0);
                //  Info
                m_info.Append(Msg.Translate(m_lang, "VAF_UserContact_ID")).Append("=").Append(VAF_UserContact_U_Name).Append("/").Append(VAF_UserContact_U_Name).Append("\n");


                sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                    + " VALUES (" + m_stdValues + "," + VAF_UserContact_ID + "," + user.GetVAF_Role_ID() + ")";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "UserRole ClientUser+User NOT inserted");
                //  OrgUser             - User
                sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                    + " VALUES (" + m_stdValues + "," + VAF_UserContact_U_ID + "," + user.GetVAF_Role_ID() + ")";
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "UserRole OrgUser+Org NOT inserted");

            }
            else
            {
                m_ctx.SetContext("#VAF_UserContact_U_ID", 0);
            }
            /**
             *  Create User-Role
             */
            //  ClientUser          - Admin & User
            sql = "INSERT INTO VAF_UserContact_Roles(" + m_stdColumns + ",VAF_UserContact_ID,VAF_Role_ID)"
                + " VALUES (" + m_stdValues + "," + VAF_UserContact_ID + "," + admin.GetVAF_Role_ID() + ")";
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, m_trx);
            if (no != 1)
                log.Log(Level.SEVERE, "UserRole ClientUser+Admin NOT inserted");

            //	Processors
            if (lstTableName.Contains("VAB_AccountHanlder")) // Update by Paramjeet Singh
            {
                MVA_AccountHanlder ap = new MVA_AccountHanlder(m_client, VAF_UserContact_ID);
                ap.Save();
            }
            if (lstTableName.Contains("VAR_Req_Handler")) // Update by Paramjeet Singh
            {
                MRequestProcessor rp = new MRequestProcessor(m_client, VAF_UserContact_ID);
                rp.Save();
            }

            ///////////////////////////////////////////
            ///////Create Default Roles
            CreateDefaultRoles(VAF_UserContact_ID);
            ///////////////////////////////////////////
            /////////Create AccountGroup/////////////

            CreateAccountingGroup();
            ////////CopyPrintFormat
            //CopyPrintFormat();
            ////////Create CurrencySource//////
            //CreateCurrencySource();
            CreateKpi(admin.GetVAF_Role_ID()); // Update by Paramjeet Singh
            CreateKPIPane(); // Update by Paramjeet Singh
            CreateChartPane(); // Update by Paramjeet Singh
            CreateView(admin.GetVAF_Role_ID()); // Update by Paramjeet Singh
            CreateTopMenu(admin.GetVAF_Role_ID());
            CreateAppointmentCategory(); // Update by Paramjeet Singh
            CreateCostElement();
            CopyRoleCenter(admin.GetVAF_Role_ID()); // Update by Paramjeet Singh
            CopyDashBoard(admin.GetVAF_Role_ID()); // Update by Paramjeet Singh
            CopyOrgType();


            log.Info("fini");
            return true;
        }   //  createClient
        string tableName = string.Empty;
        private void CreateDefaultRoles(int adminUserID)
        {
            string sql = @"select * from VAF_Role where vaf_client_id=0 and vaf_org_id=0 and name!='Sys Admin' and name!='System Administrator' AND IsForNewTenant='Y'";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                MVAFRole role = null;
                DataSet dsComm = null;
                X_VAF_Role_OrgRights orgAcess = null;
                X_VAF_UserContact_Roles userRole = null;
                X_VAF_Screen_Rights winAcess = null;
                X_VAF_Job_Rights processAcess = null;
                X_VAF_Page_Rights formAcess = null;
                X_VAF_WFlow_Rights workAccess = null;
                X_VAF_Task_Rights taskAcess = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    role = new MVAFRole(m_ctx, 0, m_trx);
                    role.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                    role.SetVAF_Org_ID(0);
                    role.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                    if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                    {
                        role.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                    }
                    role.SetIsActive(true);
                    if (ds.Tables[0].Rows[i]["IsAdministrator"] != null && ds.Tables[0].Rows[i]["IsAdministrator"] != DBNull.Value)
                    {
                        role.SetIsAdministrator(ds.Tables[0].Rows[i]["IsAdministrator"].ToString().Equals('Y') ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["UserLevel"] != null && ds.Tables[0].Rows[i]["UserLevel"] != DBNull.Value)
                    {
                        role.SetUserLevel(ds.Tables[0].Rows[i]["UserLevel"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["IsManual"] != null && ds.Tables[0].Rows[i]["IsManual"] != DBNull.Value)
                    {
                        role.SetIsManual(ds.Tables[0].Rows[i]["IsManual"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["VAB_Currency_ID"] != null && ds.Tables[0].Rows[i]["VAB_Currency_ID"] != DBNull.Value)
                    {
                        role.SetVAB_Currency_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Currency_ID"]));
                    }
                    if (ds.Tables[0].Rows[i]["AmtApproval"] != null && ds.Tables[0].Rows[i]["AmtApproval"] != DBNull.Value)
                    {
                        role.SetAmtApproval(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtApproval"]));
                    }
                    if (ds.Tables[0].Rows[i]["AmtApproval"] != null && ds.Tables[0].Rows[i]["AmtApproval"] != DBNull.Value)
                    {
                        role.SetAmtApproval(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtApproval"]));
                    }
                    if (ds.Tables[0].Rows[i]["IsCanApproveOwnDoc"] != null && ds.Tables[0].Rows[i]["IsCanApproveOwnDoc"] != DBNull.Value)
                    {
                        role.SetIsCanApproveOwnDoc(ds.Tables[0].Rows[i]["IsCanApproveOwnDoc"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["Supervisor_ID"] != null && ds.Tables[0].Rows[i]["Supervisor_ID"] != DBNull.Value)
                    {
                        role.SetSupervisor_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Supervisor_ID"]));
                    }
                    if (ds.Tables[0].Rows[i]["VAF_TreeInfo_Menu_ID"] != null && ds.Tables[0].Rows[i]["VAF_TreeInfo_Menu_ID"] != DBNull.Value)
                    {
                        role.SetVAF_TreeInfo_Menu_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TreeInfo_Menu_ID"]));
                    }
                    if (ds.Tables[0].Rows[i]["PreferenceType"] != null && ds.Tables[0].Rows[i]["PreferenceType"] != DBNull.Value)
                    {
                        role.SetPreferenceType(ds.Tables[0].Rows[i]["PreferenceType"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["IsChangeLog"] != null && ds.Tables[0].Rows[i]["IsChangeLog"] != DBNull.Value)
                    {
                        role.SetIsChangeLog(ds.Tables[0].Rows[i]["IsChangeLog"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsShowAcct"] != null && ds.Tables[0].Rows[i]["IsShowAcct"] != DBNull.Value)
                    {
                        role.SetIsChangeLog(ds.Tables[0].Rows[i]["IsShowAcct"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsAccessAllOrgs"] != null && ds.Tables[0].Rows[i]["IsAccessAllOrgs"] != DBNull.Value)
                    {
                        role.SetIsAccessAllOrgs(ds.Tables[0].Rows[i]["IsAccessAllOrgs"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsUseBPRestrictions"] != null && ds.Tables[0].Rows[i]["IsUseBPRestrictions"] != DBNull.Value)
                    {
                        role.SetIsUseBPRestrictions(ds.Tables[0].Rows[i]["IsUseBPRestrictions"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["VAF_TreeInfo_Org_ID"] != null && ds.Tables[0].Rows[i]["VAF_TreeInfo_Org_ID"] != DBNull.Value)
                    {
                        role.SetVAF_TreeInfo_Org_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TreeInfo_Org_ID"]));
                    }
                    if (ds.Tables[0].Rows[i]["IsUseUserOrgAccess"] != null && ds.Tables[0].Rows[i]["IsUseUserOrgAccess"] != DBNull.Value)
                    {
                        role.SetIsUseUserOrgAccess(ds.Tables[0].Rows[i]["IsUseUserOrgAccess"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsCanReport"] != null && ds.Tables[0].Rows[i]["IsCanReport"] != DBNull.Value)
                    {
                        role.SetIsCanReport(ds.Tables[0].Rows[i]["IsCanReport"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsCanExport"] != null && ds.Tables[0].Rows[i]["IsCanExport"] != DBNull.Value)
                    {
                        role.SetIsCanExport(ds.Tables[0].Rows[i]["IsCanExport"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsPersonalLock"] != null && ds.Tables[0].Rows[i]["IsPersonalLock"] != DBNull.Value)
                    {
                        role.SetIsPersonalLock(ds.Tables[0].Rows[i]["IsPersonalLock"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["IsPersonalAccess"] != null && ds.Tables[0].Rows[i]["IsPersonalAccess"] != DBNull.Value)
                    {
                        role.SetIsPersonalAccess(ds.Tables[0].Rows[i]["IsPersonalAccess"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["OverwritePriceLimit"] != null && ds.Tables[0].Rows[i]["OverwritePriceLimit"] != DBNull.Value)
                    {
                        role.SetOverwritePriceLimit(ds.Tables[0].Rows[i]["OverwritePriceLimit"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["OverrideReturnPolicy"] != null && ds.Tables[0].Rows[i]["OverrideReturnPolicy"] != DBNull.Value)
                    {
                        role.SetOverrideReturnPolicy(ds.Tables[0].Rows[i]["OverrideReturnPolicy"].ToString().Equals("Y") ? true : false);
                    }
                    if (ds.Tables[0].Rows[i]["ConfirmQueryRecords"] != null && ds.Tables[0].Rows[i]["ConfirmQueryRecords"] != DBNull.Value)
                    {
                        role.SetConfirmQueryRecords(Util.GetValueOfInt(ds.Tables[0].Rows[i]["ConfirmQueryRecords"]));
                    }
                    if (ds.Tables[0].Rows[i]["MaxQueryRecords"] != null && ds.Tables[0].Rows[i]["MaxQueryRecords"] != DBNull.Value)
                    {
                        role.SetMaxQueryRecords(Util.GetValueOfInt(ds.Tables[0].Rows[i]["MaxQueryRecords"]));
                    }
                    if (ds.Tables[0].Rows[i]["ConnectionProfile"] != null && ds.Tables[0].Rows[i]["ConnectionProfile"] != DBNull.Value)
                    {
                        role.SetConnectionProfile(ds.Tables[0].Rows[i]["ConnectionProfile"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["DisplayClientOrg"] != null && ds.Tables[0].Rows[i]["DisplayClientOrg"] != DBNull.Value)
                    {
                        role.SetDisplayClientOrg(ds.Tables[0].Rows[i]["DisplayClientOrg"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["WinUserDefLevel"] != null && ds.Tables[0].Rows[i]["WinUserDefLevel"] != DBNull.Value)
                    {
                        role.SetWinUserDefLevel(ds.Tables[0].Rows[i]["WinUserDefLevel"].ToString());
                    }
                    if (!role.Save(m_trx))
                    {
                        log.Info(role.GetName() + " RoleNotSaved");
                    }
                    else
                    {
                        /////////Save OrgAccess
                        dsComm = DB.ExecuteDataset("Select * From VAF_Role_OrgRights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                orgAcess = new X_VAF_Role_OrgRights(m_ctx, 0, m_trx);
                                orgAcess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                orgAcess.SetIsActive(true);
                                orgAcess.SetVAF_Org_ID(0);
                                orgAcess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadOnly"] != null && dsComm.Tables[0].Rows[j]["IsReadOnly"] != DBNull.Value)
                                {
                                    orgAcess.SetIsReadOnly(dsComm.Tables[0].Rows[j]["IsReadOnly"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    orgAcess.SetIsReadOnly(false);
                                }
                                if (!orgAcess.Save(m_trx))
                                {
                                    log.Info(role.GetName() + " OrgAcessNotSaved");
                                }
                            }
                        }
                        /////////////Save UserAssignment////
                        userRole = new X_VAF_UserContact_Roles(m_ctx, 0, m_trx);
                        userRole.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        userRole.SetVAF_Org_ID(0);
                        userRole.SetIsActive(true);
                        userRole.SetVAF_Role_ID(role.GetVAF_Role_ID());
                        userRole.SetVAF_UserContact_ID(adminUserID);
                        if (!userRole.Save(m_trx))
                        {
                            log.Info(role.GetName() + " UserAccessNotSaved");
                        }
                        /////////////Window Access
                        dsComm = DB.ExecuteDataset("Select * From VAF_Screen_Rights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                winAcess = new X_VAF_Screen_Rights(m_ctx, 0, m_trx);
                                winAcess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                winAcess.SetIsActive(true);
                                winAcess.SetVAF_Org_ID(0);
                                winAcess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadWrite"] != null && dsComm.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                {
                                    winAcess.SetIsReadWrite(dsComm.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    winAcess.SetIsReadWrite(false);
                                }
                                if (dsComm.Tables[0].Rows[j]["VAF_Screen_ID"] != null && dsComm.Tables[0].Rows[j]["VAF_Screen_ID"] != DBNull.Value)
                                {
                                    winAcess.SetVAF_Screen_ID(Util.GetValueOfInt(dsComm.Tables[0].Rows[j]["VAF_Screen_ID"]));
                                }
                                if (!winAcess.Save(m_trx))
                                {
                                    log.Info(" WindowAcessNotSaved");
                                }
                            }
                        }
                        ////////Save PRocess Acceess
                        dsComm = DB.ExecuteDataset("Select * From VAF_Job_Rights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                processAcess = new X_VAF_Job_Rights(m_ctx, 0, m_trx);
                                processAcess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                processAcess.SetIsActive(true);
                                processAcess.SetVAF_Org_ID(0);
                                processAcess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadWrite"] != null && dsComm.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                {
                                    processAcess.SetIsReadWrite(dsComm.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    processAcess.SetIsReadWrite(false);
                                }
                                if (dsComm.Tables[0].Rows[j]["VAF_Job_ID"] != null && dsComm.Tables[0].Rows[j]["VAF_Job_ID"] != DBNull.Value)
                                {
                                    processAcess.SetVAF_Job_ID(Util.GetValueOfInt(dsComm.Tables[0].Rows[j]["VAF_Job_ID"]));
                                }
                                if (!processAcess.Save(m_trx))
                                {
                                    log.Info(" WindowAcessNotSaved");
                                }
                            }
                        }

                        ////////Save FormAccess 
                        dsComm = DB.ExecuteDataset("Select * From VAF_Page_Rights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                formAcess = new X_VAF_Page_Rights(m_ctx, 0, m_trx);
                                formAcess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                formAcess.SetIsActive(true);
                                formAcess.SetVAF_Org_ID(0);
                                formAcess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadWrite"] != null && dsComm.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                {
                                    formAcess.SetIsReadWrite(dsComm.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    formAcess.SetIsReadWrite(false);
                                }
                                if (dsComm.Tables[0].Rows[j]["VAF_Page_ID"] != null && dsComm.Tables[0].Rows[j]["VAF_Page_ID"] != DBNull.Value)
                                {
                                    formAcess.SetVAF_Page_ID(Util.GetValueOfInt(dsComm.Tables[0].Rows[j]["VAF_Page_ID"]));
                                }
                                if (!formAcess.Save(m_trx))
                                {
                                    log.Info(" WindowAcessNotSaved");
                                }
                            }
                        }
                        /////////////Save WorkFlow Access
                        dsComm = DB.ExecuteDataset("Select * From VAF_WFlow_Rights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                workAccess = new X_VAF_WFlow_Rights(m_ctx, 0, m_trx);
                                workAccess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                workAccess.SetIsActive(true);
                                workAccess.SetVAF_Org_ID(0);
                                workAccess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadWrite"] != null && dsComm.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                {
                                    workAccess.SetIsReadWrite(dsComm.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    workAccess.SetIsReadWrite(false);
                                }
                                if (dsComm.Tables[0].Rows[j]["VAF_Workflow_ID"] != null && dsComm.Tables[0].Rows[j]["VAF_Workflow_ID"] != DBNull.Value)
                                {
                                    workAccess.SetVAF_Workflow_ID(Util.GetValueOfInt(dsComm.Tables[0].Rows[j]["VAF_Workflow_ID"]));
                                }
                                if (!workAccess.Save(m_trx))
                                {
                                    log.Info(" WindowAcessNotSaved");
                                }
                            }
                        }
                        /////////Save TaskAcess
                        dsComm = DB.ExecuteDataset("Select * From VAF_Task_Rights WHERE VAF_Role_ID=" + ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                        if (dsComm != null)
                        {
                            for (int j = 0; j < dsComm.Tables[0].Rows.Count; j++)
                            {
                                taskAcess = new X_VAF_Task_Rights(m_ctx, 0, m_trx);
                                taskAcess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                taskAcess.SetIsActive(true);
                                taskAcess.SetVAF_Org_ID(0);
                                taskAcess.SetVAF_Role_ID(role.GetVAF_Role_ID());
                                if (dsComm.Tables[0].Rows[j]["IsReadWrite"] != null && dsComm.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                {
                                    taskAcess.SetIsReadWrite(dsComm.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y"));
                                }
                                else
                                {
                                    taskAcess.SetIsReadWrite(false);
                                }
                                if (dsComm.Tables[0].Rows[j]["VAF_Task_ID"] != null && dsComm.Tables[0].Rows[j]["VAF_Task_ID"] != DBNull.Value)
                                {
                                    taskAcess.SetVAF_Task_ID(Util.GetValueOfInt(dsComm.Tables[0].Rows[j]["VAF_Task_ID"]));
                                }
                                if (!taskAcess.Save(m_trx))
                                {
                                    log.Info(" WindowAcessNotSaved");
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CreateAccountingGroup()
        {
            tableName = "VAB_AccountGroupBatch";
            if (lstTableName.Contains(tableName))// Update by Paramjeet Singh
            {
                string sqlBatch = @"select * from VAB_AccountGroupBatch where vaf_client_id=0 and vaf_org_id=0 AND IsForNewTenant='Y' ";
                DataSet dsBatch = DB.ExecuteDataset(sqlBatch);
                if (dsBatch != null)
                {
                    MVABAccountGroupBatch acctGrpBatch = null;

                    for (int bat = 0; bat < dsBatch.Tables[0].Rows.Count; bat++)
                    {

                        acctGrpBatch = new MVABAccountGroupBatch(m_ctx, 0, m_trx);
                        acctGrpBatch.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        acctGrpBatch.SetVAF_Org_ID(0);
                        if (dsBatch.Tables[0].Rows[bat]["Value"] != null && dsBatch.Tables[0].Rows[bat]["Value"] != DBNull.Value)
                        {
                            acctGrpBatch.SetValue(dsBatch.Tables[0].Rows[bat]["Value"].ToString());
                        }
                        if (dsBatch.Tables[0].Rows[bat]["Name"] != null && dsBatch.Tables[0].Rows[bat]["Name"] != DBNull.Value)
                        {
                            acctGrpBatch.SetName(dsBatch.Tables[0].Rows[bat]["Name"].ToString());
                        }
                        if (dsBatch.Tables[0].Rows[bat]["Description"] != null && dsBatch.Tables[0].Rows[bat]["Description"] != DBNull.Value)
                        {
                            acctGrpBatch.SetDescription(dsBatch.Tables[0].Rows[bat]["Description"].ToString());
                        }
                        if (!acctGrpBatch.Save(m_trx))
                        {
                            log.Info(acctGrpBatch.GetName() + " AccountGroupBatchNotSaved");
                        }
                        else
                        {
                            if (dsBatch.Tables[0].Rows[bat]["VAB_AccountGroupBatch_ID"] != null && dsBatch.Tables[0].Rows[bat]["VAB_AccountGroupBatch_ID"] != DBNull.Value)
                            {

                                string sql = @"select * from VAB_AccountGroup where vaf_client_id=0 and vaf_org_id=0 AND VAB_AccountGroupBatch_ID = " + Util.GetValueOfInt(dsBatch.Tables[0].Rows[bat]["VAB_AccountGroupBatch_ID"]);

                                DataSet ds = DB.ExecuteDataset(sql);
                                if (ds != null)
                                {

                                    MVABAccountGroup acct = null;
                                    string sqlTrl = "";
                                    DataSet dstrl = null;
                                    string sqlSub = "";
                                    DataSet dsSub = null;
                                    string sqlSubTrl = "";
                                    DataSet dsSubTrl = null;
                                    MVABAccountGroupTL acctTrl = null;
                                    MVABAccountSubGroup acctS = null;
                                    MVABAccountSubGroupTL acctStrl = null;
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        acct = new MVABAccountGroup(m_ctx, 0, m_trx);
                                        acct.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                        acct.SetVAF_Org_ID(0);

                                        // Change Added AccountGroupBatch
                                        acct.SetVAB_AccountGroupBatch_ID(acctGrpBatch.GetVAB_AccountGroupBatch_ID());
                                        if (ds.Tables[0].Rows[i]["Line"] != null && ds.Tables[0].Rows[i]["Line"] != DBNull.Value)
                                        {
                                            acct.SetLine(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Line"]));
                                        }
                                        if (ds.Tables[0].Rows[i]["Value"] != null && ds.Tables[0].Rows[i]["Value"] != DBNull.Value)
                                        {
                                            acct.SetValue(ds.Tables[0].Rows[i]["Value"].ToString());
                                        }
                                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                                        {
                                            acct.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                                        }
                                        if (ds.Tables[0].Rows[i]["PrintName"] != null && ds.Tables[0].Rows[i]["PrintName"] != DBNull.Value)
                                        {
                                            acct.SetPrintName(ds.Tables[0].Rows[i]["PrintName"].ToString());
                                        }
                                        if (ds.Tables[0].Rows[i]["HasSubGroup"] != null && ds.Tables[0].Rows[i]["HasSubGroup"] != DBNull.Value)
                                        {
                                            acct.SetHasSubGroup(ds.Tables[0].Rows[i]["HasSubGroup"].ToString().Equals("Y") ? true : false);
                                        }
                                        if (ds.Tables[0].Rows[i]["IsMemoGroup"] != null && ds.Tables[0].Rows[i]["IsMemoGroup"] != DBNull.Value)
                                        {
                                            acct.SetIsMemoGroup(ds.Tables[0].Rows[i]["IsMemoGroup"].ToString().Equals("Y") ? true : false);
                                        }
                                        if (ds.Tables[0].Rows[i]["ShowInProfitLoss"] != null && ds.Tables[0].Rows[i]["ShowInProfitLoss"] != DBNull.Value)
                                        {
                                            acct.SetShowInProfitLoss(ds.Tables[0].Rows[i]["ShowInProfitLoss"].ToString().Equals("Y") ? true : false);
                                        }
                                        if (ds.Tables[0].Rows[i]["ShowInBalanceSheet"] != null && ds.Tables[0].Rows[i]["ShowInBalanceSheet"] != DBNull.Value)
                                        {
                                            acct.SetShowInBalanceSheet(ds.Tables[0].Rows[i]["ShowInBalanceSheet"].ToString().Equals("Y") ? true : false);
                                        }
                                        if (ds.Tables[0].Rows[i]["ShowInCashFlow"] != null && ds.Tables[0].Rows[i]["ShowInCashFlow"] != DBNull.Value)
                                        {
                                            acct.SetShowInCashFlow(ds.Tables[0].Rows[i]["ShowInCashFlow"].ToString().Equals("Y") ? true : false);
                                        }
                                        acct.SetIsActive(true);
                                        if (!acct.Save(m_trx))
                                        {
                                            log.Info(acct.GetName() + " AccountGroupNotSaved");

                                        }
                                        else
                                        {
                                            ///////////Save Translation
                                            if (ds.Tables[0].Rows[i]["VAB_AccountGroup_ID"] != null && ds.Tables[0].Rows[i]["VAB_AccountGroup_ID"] != DBNull.Value)
                                            {
                                                sqlTrl = "SELECT * FROM VAB_AccountGroup_TL WHERE vaf_client_id=0 and vaf_org_id=0 and VAB_AccountGroup_ID=" + ds.Tables[0].Rows[i]["VAB_AccountGroup_ID"];
                                                dstrl = DB.ExecuteDataset(sqlTrl);
                                                if (dstrl != null)
                                                {
                                                    for (int j = 0; j < dstrl.Tables[0].Rows.Count; j++)
                                                    {
                                                        acctTrl = new MVABAccountGroupTL(m_ctx, 0, m_trx);
                                                        acctTrl.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                                        acctTrl.SetVAF_Org_ID(0);
                                                        if (dstrl.Tables[0].Rows[j]["VAF_Language"] != null && dstrl.Tables[0].Rows[j]["VAF_Language"] != DBNull.Value)
                                                        {
                                                            acctTrl.SetVAF_Language(dstrl.Tables[0].Rows[i]["VAF_Language"].ToString());
                                                        }
                                                        acctTrl.SetVAB_AccountGroup_ID(acct.Get_ID());
                                                        if (dstrl.Tables[0].Rows[j]["Name"] != null && dstrl.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                                        {
                                                            acctTrl.SetName(dstrl.Tables[0].Rows[i]["Name"].ToString());
                                                        }
                                                        if (dstrl.Tables[0].Rows[j]["Description"] != null && dstrl.Tables[0].Rows[j]["Description"] != DBNull.Value)
                                                        {
                                                            acctTrl.SetDescription(dstrl.Tables[0].Rows[i]["Description"].ToString());
                                                        }
                                                        acctTrl.SetIsActive(true);
                                                        if (dstrl.Tables[0].Rows[j]["IsTranslated"] != null && dstrl.Tables[0].Rows[j]["IsTranslated"] != DBNull.Value)
                                                        {
                                                            acctTrl.SetIsTranslated(dstrl.Tables[0].Rows[i]["IsTranslated"].ToString().Equals("Y"));
                                                        }
                                                        if (!acctTrl.Save(m_trx))
                                                        {
                                                            log.Info(acctTrl.GetName() + " AccountGroupTrlNotSaved");
                                                        }
                                                    }
                                                }
                                                /////////Save AccountSubGroup
                                                sqlSub = "SELECT * FROM VAB_AccountSubGroup WHERE vaf_client_id=0 and vaf_org_id=0 and VAB_AccountGroup_ID=" + ds.Tables[0].Rows[i]["VAB_AccountGroup_ID"];
                                                dsSub = DB.ExecuteDataset(sqlSub);
                                                if (dsSub != null)
                                                {
                                                    for (int j = 0; j < dsSub.Tables[0].Rows.Count; j++)
                                                    {
                                                        acctS = new MVABAccountSubGroup(m_ctx, 0, m_trx);
                                                        acctS.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                                        acctS.SetVAF_Org_ID(0);
                                                        acctS.SetVAB_AccountGroup_ID(acct.Get_ID());
                                                        if (dsSub.Tables[0].Rows[j]["Line"] != null && dsSub.Tables[0].Rows[j]["Line"] != DBNull.Value)
                                                        {
                                                            acctS.SetLine(Util.GetValueOfInt(dsSub.Tables[0].Rows[j]["Line"]));
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["Value"] != null && dsSub.Tables[0].Rows[j]["Value"] != DBNull.Value)
                                                        {
                                                            acctS.SetValue(dsSub.Tables[0].Rows[j]["Value"].ToString());
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["Name"] != null && dsSub.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                                        {
                                                            acctS.SetName(dsSub.Tables[0].Rows[j]["Name"].ToString());
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["PrintName"] != null && dsSub.Tables[0].Rows[j]["PrintName"] != DBNull.Value)
                                                        {
                                                            acctS.SetName(dsSub.Tables[0].Rows[j]["PrintName"].ToString());
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["ShowInCashFlow"] != null && dsSub.Tables[0].Rows[j]["ShowInCashFlow"] != DBNull.Value)
                                                        {
                                                            acctS.SetShowInCashFlow(dsSub.Tables[0].Rows[j]["ShowInCashFlow"].ToString().Equals("Y"));
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["ShowInBalanceSheet"] != null && dsSub.Tables[0].Rows[j]["ShowInBalanceSheet"] != DBNull.Value)
                                                        {
                                                            acctS.SetShowInBalanceSheet(dsSub.Tables[0].Rows[j]["ShowInBalanceSheet"].ToString().Equals("Y"));
                                                        }
                                                        if (dsSub.Tables[0].Rows[j]["ShowInProfitLoss"] != null && dsSub.Tables[0].Rows[j]["ShowInProfitLoss"] != DBNull.Value)
                                                        {
                                                            acctS.SetShowInProfitLoss(dsSub.Tables[0].Rows[j]["ShowInProfitLoss"].ToString().Equals("Y"));
                                                        }
                                                        acctS.SetIsActive(true);
                                                        if (!acctS.Save(m_trx))
                                                        {
                                                            log.Info(acctS.GetName() + " AccountSubGroupNotSaved");
                                                        }
                                                        else
                                                        {
                                                            //////Save AccountSub Gruup Translation
                                                            sqlSubTrl = "SELECT * FROM VAB_AccountSubGroup_TL WHERE vaf_client_id=0 and vaf_org_id=0 and VAB_AccountSubGroup_ID=" + dsSub.Tables[0].Rows[j]["VAB_AccountSubGroup_ID"];
                                                            dsSubTrl = DB.ExecuteDataset(sqlSubTrl);
                                                            if (dsSubTrl != null)
                                                            {
                                                                for (int k = 0; k < dsSubTrl.Tables[0].Rows.Count; k++)
                                                                {
                                                                    acctStrl.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                                                    acctStrl.SetVAF_Org_ID(0);
                                                                    if (dsSubTrl.Tables[0].Rows[k]["VAF_Language"] != null && dsSubTrl.Tables[0].Rows[k]["VAF_Language"] != DBNull.Value)
                                                                    {
                                                                        acctStrl.SetVAF_Language(dsSubTrl.Tables[0].Rows[k]["VAF_Language"].ToString());
                                                                    }
                                                                    acctStrl.SetVAB_AccountSubGroup_ID(acctS.Get_ID());
                                                                    if (dsSubTrl.Tables[0].Rows[k]["Name"] != null && dsSubTrl.Tables[0].Rows[k]["Name"] != DBNull.Value)
                                                                    {
                                                                        acctStrl.SetName(dsSubTrl.Tables[0].Rows[k]["Name"].ToString());
                                                                    }
                                                                    if (dsSubTrl.Tables[0].Rows[k]["Description"] != null && dsSubTrl.Tables[0].Rows[k]["Description"] != DBNull.Value)
                                                                    {
                                                                        acctStrl.SetDescription(dsSubTrl.Tables[0].Rows[k]["Description"].ToString());
                                                                    }
                                                                    acctStrl.SetIsActive(true);
                                                                    if (dsSubTrl.Tables[0].Rows[k]["IsTranslated"] != null && dsSubTrl.Tables[0].Rows[k]["IsTranslated"] != DBNull.Value)
                                                                    {
                                                                        acctStrl.SetIsTranslated(dsSubTrl.Tables[0].Rows[k]["IsTranslated"].ToString().Equals("Y"));
                                                                    }
                                                                    if (!acctStrl.Save(m_trx))
                                                                    {
                                                                        log.Info(acctTrl.GetName() + " AccountGroupTrlNotSaved");
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        private void CreateKpi(int role_ID)
        {
            tableName = "VARC_KPI";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VARC_KPI Where VAF_Client_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                DataSet dsAccess = null;
                DataSet dsUsrQry = null;
                if (ds != null)
                {
                    X_VARC_KPI kpi = null;
                    X_VARC_KPIRights kpiA = null;
                    X_VAF_UserSearch qry = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        kpi = new X_VARC_KPI(m_ctx, 0, m_trx);
                        kpi.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        kpi.SetVAF_Org_ID(0);
                        if (ds.Tables[0].Rows[i]["SEARCHKEY"] != null && ds.Tables[0].Rows[i]["SEARCHKEY"] != DBNull.Value)
                        {
                            kpi.SetSEARCHKEY(ds.Tables[0].Rows[i]["SEARCHKEY"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            kpi.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                        {
                            kpi.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["VAF_UserContact_ID"] != null && ds.Tables[0].Rows[i]["VAF_UserContact_ID"] != DBNull.Value)
                        {
                            kpi.SetVAF_UserContact_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["VAF_TableView_ID"] != null && ds.Tables[0].Rows[i]["VAF_TableView_ID"] != DBNull.Value)
                        {
                            kpi.SetVAF_TableView_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["VAF_Tab_ID"] != null && ds.Tables[0].Rows[i]["VAF_Tab_ID"] != DBNull.Value)
                        {
                            kpi.SetVAF_Tab_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Tab_ID"]));
                        }
                        //if (ds.Tables[0].Rows[i]["VAF_Role_ID"] != null && ds.Tables[0].Rows[i]["VAF_Role_ID"] != DBNull.Value)
                        //{
                        //    kpi.SetVAF_Role_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Role_ID"]));
                        //}
                        kpi.SetVAF_Role_ID(role_ID);
                        if (ds.Tables[0].Rows[i]["Record_ID"] != null && ds.Tables[0].Rows[i]["Record_ID"] != DBNull.Value)
                        {
                            kpi.SetRecord_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != null && ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != DBNull.Value)
                        {
                            //kpi.SetVAF_UserSearch_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserSearch_ID"]));
                            dsUsrQry = DB.ExecuteDataset("SELECT * FROM VAF_UserSearch Where VAF_UserSearch_ID=" + ds.Tables[0].Rows[i]["VAF_UserSearch_ID"]);
                            if (dsUsrQry != null && dsUsrQry.Tables[0].Rows.Count > 0)
                            {
                                qry = new X_VAF_UserSearch(m_ctx, 0, m_trx);
                                qry.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                qry.SetVAF_Org_ID(0);
                                qry.SetIsActive(true);
                                if (dsUsrQry.Tables[0].Rows[0]["Name"] != null && dsUsrQry.Tables[0].Rows[0]["Name"] != DBNull.Value)
                                {
                                    qry.SetName(dsUsrQry.Tables[0].Rows[0]["Name"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["Description"] != null && dsUsrQry.Tables[0].Rows[0]["Description"] != DBNull.Value)
                                {
                                    qry.SetDescription(dsUsrQry.Tables[0].Rows[0]["Description"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_TableView_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"]));
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_Tab_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"]));
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["Code"] != null && dsUsrQry.Tables[0].Rows[0]["Code"] != DBNull.Value)
                                {
                                    qry.SetCode(dsUsrQry.Tables[0].Rows[0]["Code"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_UserContact_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"]));
                                }
                                if (!qry.Save(m_trx))
                                {
                                    log.Info(qry.GetName() + " UserQueryNotSaved");
                                }
                                kpi.SetVAF_UserSearch_ID(qry.Get_ID());
                            }
                        }
                        if (ds.Tables[0].Rows[i]["VAF_Column_ID"] != null && ds.Tables[0].Rows[i]["VAF_Column_ID"] != DBNull.Value)
                        {
                            kpi.SetVAF_Column_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Column_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["IsMinimum"] != null && ds.Tables[0].Rows[i]["IsMinimum"] != DBNull.Value)
                        {
                            kpi.SetIsMinimum(ds.Tables[0].Rows[i]["IsMinimum"].ToString().Equals("Y"));
                        }
                        if (ds.Tables[0].Rows[i]["IsMaximum"] != null && ds.Tables[0].Rows[i]["IsMaximum"] != DBNull.Value)
                        {
                            kpi.SetIsMaximum(ds.Tables[0].Rows[i]["IsMaximum"].ToString().Equals("Y"));
                        }
                        if (ds.Tables[0].Rows[i]["IsCount"] != null && ds.Tables[0].Rows[i]["IsCount"] != DBNull.Value)
                        {
                            kpi.SetIsCount(ds.Tables[0].Rows[i]["IsCount"].ToString().Equals("Y"));
                        }
                        if (ds.Tables[0].Rows[i]["IsSum"] != null && ds.Tables[0].Rows[i]["IsSum"] != DBNull.Value)
                        {
                            kpi.SetIsSum(ds.Tables[0].Rows[i]["IsSum"].ToString().Equals("Y"));
                        }
                        kpi.SetIsActive(true);
                        if (!kpi.Save(m_trx))
                        {
                            log.Info(kpi.GetName() + " KpiNotSaved");
                        }
                        else
                        {
                            dsAccess = DB.ExecuteDataset("SELECT VAF_USERCONTACT_ID,VAF_ROLE_ID FROM VARC_KPIRIGHTS WHERE VARC_KPI_ID=" + ds.Tables[0].Rows[i]["VARC_KPI_ID"]);
                            if (dsAccess != null)
                            {
                                for (int j = 0; j < dsAccess.Tables[0].Rows.Count; j++)
                                {
                                    kpiA = new X_VARC_KPIRights(m_ctx, 0, m_trx);
                                    kpiA.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                    kpiA.SetVAF_Org_ID(0);
                                    kpiA.SetVARC_KPI_ID(kpi.Get_ID());
                                    kpiA.SetIsActive(true);
                                    if (dsAccess.Tables[0].Rows[j]["VAF_USERCONTACT_ID"] != null && dsAccess.Tables[0].Rows[j]["VAF_USERCONTACT_ID"] != DBNull.Value)
                                    {
                                        kpiA.SetVAF_UserContact_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["VAF_USERCONTACT_ID"]));
                                    }
                                    //if (dsAccess.Tables[0].Rows[j]["VAF_Role_ID"] != null && dsAccess.Tables[0].Rows[j]["VAF_Role_ID"] != DBNull.Value)
                                    //{
                                    //    kpiA.SetVAF_Role_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["VAF_Role_ID"]));
                                    //}
                                    kpiA.SetVAF_Role_ID(role_ID);
                                    if (!kpiA.Save(m_trx))
                                    {
                                        log.Info(kpiA.GetVARC_KPI_ID() + " KpiNotSaved");
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        private void CreateKPIPane()
        {
            tableName = "VARC_KPIPane";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VARC_KPIPane WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                if (ds != null)
                {
                    X_VARC_KPIPane pane = null;
                    X_VARC_KPICenter center = null;
                    DataSet dsCenter = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        pane = new X_VARC_KPIPane(m_ctx, 0, m_trx);
                        pane.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        pane.SetVAF_Org_ID(0);
                        if (ds.Tables[0].Rows[i]["SeqNo"] != null && ds.Tables[0].Rows[i]["SeqNo"] != DBNull.Value)
                        {
                            pane.SetSeqNo(Util.GetValueOfInt(ds.Tables[0].Rows[i]["SeqNo"]));
                        }
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            pane.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                        {
                            pane.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["BG_Color_ID"] != null && ds.Tables[0].Rows[i]["BG_Color_ID"] != DBNull.Value)
                        {
                            pane.SetBG_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["BG_Color_ID"]));
                        }
                        pane.SetIsActive(true);
                        if (!pane.Save(m_trx))
                        {
                            log.Info(pane.GetName() + " KPIPaneNotSaved");
                        }
                        else
                        {
                            dsCenter = DB.ExecuteDataset("SELECT * FROM VARC_KPICenter WHERE VARC_KPIPANE_ID=" + ds.Tables[0].Rows[i]["VARC_KPIPANE_ID"]);
                            if (dsCenter != null)
                            {
                                for (int j = 0; j < dsCenter.Tables[0].Rows.Count; j++)
                                {
                                    center = new X_VARC_KPICenter(m_ctx, 0, m_trx);
                                    center.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                    center.SetVAF_Org_ID(0);
                                    center.SetVARC_KPIPane_ID(pane.Get_ID());
                                    center.SetIsActive(true);
                                    if (dsCenter.Tables[0].Rows[j]["SeqNo"] != null && dsCenter.Tables[0].Rows[j]["SeqNo"] != DBNull.Value)
                                    {
                                        center.SetSeqNo(Util.GetValueOfInt(dsCenter.Tables[0].Rows[j]["SeqNo"]));
                                    }
                                    if (dsCenter.Tables[0].Rows[j]["Name"] != null && dsCenter.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                    {
                                        center.SetName(dsCenter.Tables[0].Rows[j]["Name"].ToString());
                                    }
                                    //if (dsCenter.Tables[0].Rows[j]["VARC_KPI_ID"] != null && dsCenter.Tables[0].Rows[j]["VARC_KPI_ID"] != DBNull.Value)
                                    //{
                                    //    center.SetVARC_KPI_ID(Util.GetValueOfInt(dsCenter.Tables[0].Rows[j]["VARC_KPI_ID"]));
                                    //}
                                    if (dsCenter.Tables[0].Rows[j]["Font_Color_ID"] != null && dsCenter.Tables[0].Rows[j]["Font_Color_ID"] != DBNull.Value)
                                    {
                                        center.SetFont_Color_ID(Util.GetValueOfInt(dsCenter.Tables[0].Rows[j]["Font_Color_ID"]));
                                    }
                                    if (!center.Save(m_trx))
                                    {
                                        log.Info(center.GetName() + " KPICenterNotSaved");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CreateChartPane()
        {
            tableName = "VARC_ChartPane";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VARC_ChartPane WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                if (ds != null)
                {
                    X_VARC_ChartPane chart = null;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        chart = new X_VARC_ChartPane(m_ctx, 0, m_trx);
                        chart.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        chart.SetVAF_Org_ID(0);
                        if (ds.Tables[0].Rows[i]["SeqNo"] != null && ds.Tables[0].Rows[i]["SeqNo"] != DBNull.Value)
                        {
                            chart.SetSeqNo(Util.GetValueOfInt(ds.Tables[0].Rows[i]["SeqNo"]));
                        }
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            chart.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["D_Chart_ID"] != null && ds.Tables[0].Rows[i]["D_Chart_ID"] != DBNull.Value)
                        {
                            chart.SetD_Chart_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["D_Chart_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["Rowspan"] != null && ds.Tables[0].Rows[i]["Rowspan"] != DBNull.Value)
                        {
                            chart.SetRowspan(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Rowspan"]));
                        }
                        if (ds.Tables[0].Rows[i]["Colspan"] != null && ds.Tables[0].Rows[i]["Colspan"] != DBNull.Value)
                        {
                            chart.SetColspan(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Colspan"]));
                        }
                        if (!chart.Save(m_trx))
                        {
                            log.Info(chart.GetName() + " ChartNotSaved");
                        }
                    }
                }
            }
        }
        private void CreateView(int adminRole_ID)
        {
            tableName = "VARC_View";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VARC_View WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                DataSet dsUsrQry = null;
                if (ds != null)
                {
                    X_VARC_View view = null;
                    X_VARC_ViewRights vAccess = null;
                    X_VARC_ViewColumn vCol = null;
                    X_VARC_ViewPane vPane = null;
                    X_VAF_UserSearch qry = null;
                    DataSet dsAccess = null;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        view = new X_VARC_View(m_ctx, 0, m_trx);
                        view.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        view.SetVAF_Org_ID(0);
                        view.SetIsActive(true);
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            view.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Title"] != null && ds.Tables[0].Rows[i]["Title"] != DBNull.Value)
                        {
                            view.SetTitle(ds.Tables[0].Rows[i]["Title"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                        {
                            view.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["VAF_TableView_ID"] != null && ds.Tables[0].Rows[i]["VAF_TableView_ID"] != DBNull.Value)
                        {
                            view.SetVAF_TableView_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["VAF_Tab_ID"] != null && ds.Tables[0].Rows[i]["VAF_Tab_ID"] != DBNull.Value)
                        {
                            view.SetVAF_Tab_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Tab_ID"]));
                        }
                        //if (ds.Tables[0].Rows[i]["VAF_Role_ID"] != null && ds.Tables[0].Rows[i]["VAF_Role_ID"] != DBNull.Value)
                        //{
                        //view.SetVAF_Role_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Role_ID"]));
                        //}
                        view.SetVAF_Role_ID(adminRole_ID);
                        //if (ds.Tables[0].Rows[i]["VAF_UserContact_ID"] != null && ds.Tables[0].Rows[i]["VAF_UserContact_ID"] != DBNull.Value)
                        //{
                        //    view.SetVAF_UserContact_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                        //}
                        if (ds.Tables[0].Rows[i]["Record_ID"] != null && ds.Tables[0].Rows[i]["Record_ID"] != DBNull.Value)
                        {
                            view.SetRecord_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != null && ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != DBNull.Value)
                        {
                            //view.SetVAF_UserSearch_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserSearch_ID"]));
                            dsUsrQry = DB.ExecuteDataset("SELECT * FROM VAF_UserSearch Where VAF_UserSearch_ID=" + ds.Tables[0].Rows[i]["VAF_UserSearch_ID"]);
                            if (dsUsrQry != null && dsUsrQry.Tables[0].Rows.Count > 0)
                            {
                                qry = new X_VAF_UserSearch(m_ctx, 0, m_trx);
                                qry.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                qry.SetVAF_Org_ID(0);
                                qry.SetIsActive(true);
                                if (dsUsrQry.Tables[0].Rows[0]["Name"] != null && dsUsrQry.Tables[0].Rows[0]["Name"] != DBNull.Value)
                                {
                                    qry.SetName(dsUsrQry.Tables[0].Rows[0]["Name"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["Description"] != null && dsUsrQry.Tables[0].Rows[0]["Description"] != DBNull.Value)
                                {
                                    qry.SetDescription(dsUsrQry.Tables[0].Rows[0]["Description"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_TableView_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_TableView_ID"]));
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_Tab_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_Tab_ID"]));
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["Code"] != null && dsUsrQry.Tables[0].Rows[0]["Code"] != DBNull.Value)
                                {
                                    qry.SetCode(dsUsrQry.Tables[0].Rows[0]["Code"].ToString());
                                }
                                if (dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"] != null && dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"] != DBNull.Value)
                                {
                                    qry.SetVAF_UserContact_ID(Convert.ToInt32(dsUsrQry.Tables[0].Rows[0]["VAF_UserContact_ID"]));
                                }
                                if (!qry.Save(m_trx))
                                {
                                    log.Info(qry.GetName() + " UserQueryNotSaved");
                                }
                                view.SetVAF_UserSearch_ID(qry.Get_ID());
                            }
                        }
                        //if (ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != null && ds.Tables[0].Rows[i]["VAF_UserSearch_ID"] != DBNull.Value)
                        //{
                        //    view.SetVAF_UserSearch_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserSearch_ID"]));
                        //}
                        if (ds.Tables[0].Rows[i]["MinValue"] != null && ds.Tables[0].Rows[i]["MinValue"] != DBNull.Value)
                        {
                            view.SetMinValue(Util.GetValueOfInt(ds.Tables[0].Rows[i]["MinValue"]));
                        }
                        if (ds.Tables[0].Rows[i]["MaxValue"] != null && ds.Tables[0].Rows[i]["MaxValue"] != DBNull.Value)
                        {
                            view.SetMaxValue(Util.GetValueOfInt(ds.Tables[0].Rows[i]["MaxValue"]));
                        }
                        if (ds.Tables[0].Rows[i]["Font_Color_ID"] != null && ds.Tables[0].Rows[i]["Font_Color_ID"] != DBNull.Value)
                        {
                            view.SetFont_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Font_Color_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["HeaderFont_Color_ID"] != null && ds.Tables[0].Rows[i]["HeaderFont_Color_ID"] != DBNull.Value)
                        {
                            view.SetHeaderFont_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["HeaderFont_Color_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["BG_Color_ID"] != null && ds.Tables[0].Rows[i]["BG_Color_ID"] != DBNull.Value)
                        {
                            view.SetBG_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["BG_Color_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["HeaderBG_Color_ID"] != null && ds.Tables[0].Rows[i]["HeaderBG_Color_ID"] != DBNull.Value)
                        {
                            view.SetHeaderBG_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["HeaderBG_Color_ID"]));
                        }
                        if (!view.Save(m_trx))
                        {
                            log.Info(view.GetName() + " KPIPaneNotSaved");
                        }
                        else
                        {
                            dsAccess = DB.ExecuteDataset("Select * from VARC_ViewRights WHERE VARC_View_ID= " + ds.Tables[0].Rows[i]["VARC_View_ID"]);
                            if (dsAccess != null)
                            {
                                for (int j = 0; j < dsAccess.Tables[0].Rows.Count; j++)
                                {
                                    vAccess = new X_VARC_ViewRights(m_ctx, 0, m_trx);
                                    vAccess.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                    vAccess.SetVAF_Org_ID(0);
                                    vAccess.SetIsActive(true);
                                    vAccess.SetVARC_View_ID(view.Get_ID());
                                    //if (dsAccess.Tables[0].Rows[j]["VAF_UserContact_ID"] != null && dsAccess.Tables[0].Rows[j]["VAF_UserContact_ID"] != DBNull.Value)
                                    //{
                                    //    vAccess.SetVAF_UserContact_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["VAF_UserContact_ID"]));
                                    //}
                                    //if (dsAccess.Tables[0].Rows[j]["VAF_Role_ID"] != null && dsAccess.Tables[0].Rows[j]["VAF_Role_ID"] != DBNull.Value)
                                    //{
                                    //    vAccess.SetVAF_Role_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["VAF_Role_ID"]));
                                    //}
                                    vAccess.SetVAF_Role_ID(adminRole_ID);
                                    if (!vAccess.Save(m_trx))
                                    {
                                        log.Info(view.GetName() + " KPIPaneNotSaved");
                                    }
                                }
                            }

                            dsAccess = DB.ExecuteDataset("Select * from VARC_ViewColumn WHERE  VARC_View_ID =" + ds.Tables[0].Rows[i]["VARC_View_ID"]);
                            if (dsAccess != null)
                            {
                                for (int j = 0; j < dsAccess.Tables[0].Rows.Count; j++)
                                {
                                    vCol = new X_VARC_ViewColumn(m_ctx, 0, m_trx);
                                    vCol.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                    vCol.SetVAF_Org_ID(0);
                                    vCol.SetIsActive(true);
                                    vCol.SetVARC_View_ID(view.Get_ID());
                                    if (dsAccess.Tables[0].Rows[j]["SeqNo"] != null && dsAccess.Tables[0].Rows[j]["SeqNo"] != DBNull.Value)
                                    {
                                        vCol.SetSeqNo(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["SeqNo"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["Description"] != null && dsAccess.Tables[0].Rows[j]["Description"] != DBNull.Value)
                                    {
                                        vCol.SetDescription(dsAccess.Tables[0].Rows[j]["Description"].ToString());
                                    }

                                    if (dsAccess.Tables[0].Rows[j]["VAF_Field_ID"] != null && dsAccess.Tables[0].Rows[j]["VAF_Field_ID"] != DBNull.Value)
                                    {
                                        vCol.SetVAF_Field_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["VAF_Field_ID"]));
                                    }
                                    if (!vCol.Save(m_trx))
                                    {
                                        log.Info(view.GetName() + " KPIPaneNotSaved");
                                    }
                                }
                            }

                            dsAccess = DB.ExecuteDataset("Select * from VARC_ViewPane WHERE  VARC_View_ID= " + ds.Tables[0].Rows[i]["VARC_View_ID"]);
                            if (dsAccess != null)
                            {
                                for (int j = 0; j < dsAccess.Tables[0].Rows.Count; j++)
                                {
                                    vPane = new X_VARC_ViewPane(m_ctx, 0, m_trx);
                                    vPane.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                    vPane.SetVAF_Org_ID(0);
                                    vPane.SetIsActive(true);
                                    vPane.SetVARC_View_ID(view.Get_ID());
                                    if (dsAccess.Tables[0].Rows[j]["SeqNo"] != null && dsAccess.Tables[0].Rows[j]["SeqNo"] != DBNull.Value)
                                    {
                                        vPane.SetSeqNo(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["SeqNo"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["Name"] != null && dsAccess.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                    {
                                        vPane.SetName(dsAccess.Tables[0].Rows[j]["Name"].ToString());
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["Rowspan"] != null && dsAccess.Tables[0].Rows[j]["Rowspan"] != DBNull.Value)
                                    {
                                        vPane.SetRowspan(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["Rowspan"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["Colspan"] != null && dsAccess.Tables[0].Rows[j]["Colspan"] != DBNull.Value)
                                    {
                                        vPane.SetColspan(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["Colspan"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["MinValue"] != null && dsAccess.Tables[0].Rows[j]["MinValue"] != DBNull.Value)
                                    {
                                        vPane.SetMinValue(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["MinValue"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["MaxValue"] != null && dsAccess.Tables[0].Rows[j]["MaxValue"] != DBNull.Value)
                                    {
                                        vPane.SetMaxValue(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["MaxValue"]));
                                    }


                                    if (dsAccess.Tables[0].Rows[j]["Font_Color_ID"] != null && dsAccess.Tables[0].Rows[j]["Font_Color_ID"] != DBNull.Value)
                                    {
                                        vPane.SetFont_Color_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["Font_Color_ID"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["HeaderFont_Color_ID"] != null && dsAccess.Tables[0].Rows[j]["HeaderFont_Color_ID"] != DBNull.Value)
                                    {
                                        vPane.SetHeaderFont_Color_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["HeaderFont_Color_ID"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["BG_Color_ID"] != null && dsAccess.Tables[0].Rows[j]["BG_Color_ID"] != DBNull.Value)
                                    {
                                        vPane.SetBG_Color_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["BG_Color_ID"]));
                                    }
                                    if (dsAccess.Tables[0].Rows[j]["HeaderBG_Color_ID"] != null && dsAccess.Tables[0].Rows[j]["HeaderBG_Color_ID"] != DBNull.Value)
                                    {
                                        vPane.SetHeaderBG_Color_ID(Util.GetValueOfInt(dsAccess.Tables[0].Rows[j]["HeaderBG_Color_ID"]));
                                    }
                                    if (!vPane.Save(m_trx))
                                    {
                                        log.Info(view.GetName() + " KPIPaneNotSaved");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CreateTopMenu(int role_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT * FROM VAF_Module WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
            if (ds != null)
            {
                X_VAF_Module mod = null;
                X_VAF_ModuleRole role = null;
                X_VAF_ModuleFavourite fav = null;
                DataSet dsRole = null;
                DataSet dsFav = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    mod = new X_VAF_Module(m_ctx, 0, m_trx);
                    mod.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                    mod.SetVAF_Org_ID(0);
                    mod.SetIsActive(true);
                    if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                    {
                        mod.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["SeqNo"] != null && ds.Tables[0].Rows[i]["SeqNo"] != DBNull.Value)
                    {
                        mod.SetSeqNo(Util.GetValueOfInt(ds.Tables[0].Rows[i]["SeqNo"]));
                    }
                    if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                    {
                        mod.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["VAF_Image_ID"] != null && ds.Tables[0].Rows[i]["VAF_Image_ID"] != DBNull.Value)
                    {
                        mod.SetVAF_Image_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Image_ID"]));
                    }
                    if (!mod.Save(m_trx))
                    {
                        log.Info(mod.GetName() + " TopMenuNotSaved");
                    }
                    else
                    {
                        dsRole = DB.ExecuteDataset("SELECT VAF_ModuleRole_ID,VAF_Role_ID FROM VAF_ModuleRole WHERE VAF_Module_ID=" + ds.Tables[0].Rows[i]["VAF_Module_ID"]);
                        if (dsRole != null)
                        {
                            for (int j = 0; j < dsRole.Tables[0].Rows.Count; j++)
                            {
                                role = new X_VAF_ModuleRole(m_ctx, 0, m_trx);
                                role.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                role.SetVAF_Org_ID(0);
                                role.SetIsActive(true);
                                role.SetVAF_Module_ID(mod.Get_ID());
                                //if (dsRole.Tables[0].Rows[j]["VAF_Role_ID"] != null && dsRole.Tables[0].Rows[j]["VAF_Role_ID"] != DBNull.Value)
                                //{
                                //    role.SetVAF_Role_ID(Util.GetValueOfInt(dsRole.Tables[0].Rows[j]["VAF_Role_ID"]));
                                //}
                                role.SetVAF_Role_ID(role_ID);
                                if (!role.Save(m_trx))
                                {
                                    log.Info(mod.GetName() + " TopMenuRoleNotSaved");
                                }
                                else
                                {
                                    dsFav = DB.ExecuteDataset("SELECT VAF_MenuConfig_ID,SeqNo FROM VAF_ModuleFavourite WHERE VAF_ModuleRole_ID=" + dsRole.Tables[0].Rows[j]["VAF_ModuleRole_ID"]);
                                    if (dsFav != null)
                                    {
                                        for (int k = 0; k < dsFav.Tables[0].Rows.Count; k++)
                                        {
                                            fav = new X_VAF_ModuleFavourite(m_ctx, 0, m_trx);
                                            fav.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                                            fav.SetVAF_Org_ID(0);
                                            fav.SetIsActive(true);
                                            fav.SetVAF_ModuleRole_ID(role.Get_ID());
                                            if (dsFav.Tables[0].Rows[k]["VAF_MenuConfig_ID"] != null && dsFav.Tables[0].Rows[k]["VAF_MenuConfig_ID"] != DBNull.Value)
                                            {
                                                fav.SetVAF_MenuConfig_ID(Util.GetValueOfInt(dsFav.Tables[0].Rows[k]["VAF_MenuConfig_ID"]));
                                            }
                                            if (dsFav.Tables[0].Rows[k]["SeqNo"] != null && dsFav.Tables[0].Rows[k]["SeqNo"] != DBNull.Value)
                                            {
                                                fav.SetSeqNo(Util.GetValueOfInt(dsFav.Tables[0].Rows[k]["SeqNo"]));
                                            }
                                            if (!fav.Save(m_trx))
                                            {
                                                log.Info(fav.GetVAF_ModuleRole_ID() + " TopMenuRoleFavNotSaved");
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
            }

        }
        private void CreateAppointmentCategory()
        {
            // throw new NotImplementedException();
            tableName = "appointmentcategory";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("Select * from appointmentcategory WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                if (ds != null)
                {
                    X_AppointmentCategory app = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        app = new X_AppointmentCategory(m_ctx, 0, m_trx);
                        app.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                        app.SetVAF_Org_ID(0);
                        app.SetIsActive(true);
                        if (ds.Tables[0].Rows[i]["Value"] != null && ds.Tables[0].Rows[i]["Value"] != DBNull.Value)
                        {
                            app.SetValue(ds.Tables[0].Rows[i]["Value"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            app.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["VAF_Image_ID"] != null && ds.Tables[0].Rows[i]["VAF_Image_ID"] != DBNull.Value)
                        {
                            app.SetName(ds.Tables[0].Rows[i]["VAF_Image_ID"].ToString());
                        }
                        if (!app.Save(m_trx))
                        {
                            log.Info(app.GetName() + " AppiontmentCategoryNotSaved");
                        }

                    }
                }
            }
        }
        private void CreateCostElement()
        {
            if (!lstTableName.Contains("VAM_ProductCostElement"))
            {
                return;
            }
            DataSet ds = DB.ExecuteDataset("select * from VAF_CtrlRef_List where value in ('A','F','I','p','i') and VAF_Control_Ref_id=122");
            if (ds != null)
            {

                MVAMProductCostElement cost = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cost = new MVAMProductCostElement(m_ctx, 0, m_trx);
                    //tableName = cost.Get_TableName();
                    //if (lstTableName.Contains(tableName))
                    //{

                    cost.SetVAF_Client_ID(m_client.Get_ID());
                    cost.SetVAF_Org_ID(0);
                    cost.SetIsActive(true);
                    cost.SetCostElementType("M");
                    cost.SetIsCalculated(true);
                    if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                    {
                        cost.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        //if (cost.GetName().Equals("Fifo"))
                        //{
                        //    cost.SetCostingMethod("F");
                        //}
                        //else if (cost.GetName().Equals("Last Invoice"))
                        //{
                        //    cost.SetCostingMethod("i");
                        //}
                        //else if (cost.GetName().Equals("Average Invoice"))
                        //{
                        //    cost.SetCostingMethod("I");
                        //}
                        //else if (cost.GetName().Equals("Last PO"))
                        //{
                        //    cost.SetCostingMethod("p");
                        //}
                    }
                    if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                    {
                        cost.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["Value"] != null && ds.Tables[0].Rows[i]["Value"] != DBNull.Value)
                    {
                        cost.SetCostingMethod(ds.Tables[0].Rows[i]["Value"].ToString());
                    }
                    if (!cost.Save(m_trx))
                    {
                        log.Info(cost.GetName() + " CostElementNotSaved");
                    }
                    // }
                }
            }

        }
        private void CopyRoleCenter(int role_ID)
        {
            tableName = "VARC_RoleCenterManager";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VARC_RoleCenterManager WHERE VAF_CLIENT_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                if (ds != null)
                {
                    X_VARC_RoleCenterManager rcmngr = null;
                    X_VARC_RoleCenterTab tab = null;
                    X_VARC_TabPanels panel = null;
                    DataSet rcTab = null;
                    DataSet dsPanel = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        rcmngr = new X_VARC_RoleCenterManager(m_ctx, 0, m_trx);
                        rcmngr.SetVAF_Client_ID(m_client.Get_ID());
                        rcmngr.SetVAF_Org_ID(0);
                        rcmngr.SetIsActive(true);
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            rcmngr.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        //if (ds.Tables[0].Rows[i]["VAF_Role_ID"] != null && ds.Tables[0].Rows[i]["VAF_Role_ID"] != DBNull.Value)
                        //{
                        //    rcmngr.SetVAF_Role_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Role_ID"]));
                        //}
                        rcmngr.SetVAF_Role_ID(role_ID);
                        if (!rcmngr.Save(m_trx))
                        {
                            log.Info(rcmngr.GetName() + " RoleCenterNotSaved");
                        }
                        else
                        {
                            rcTab = DB.ExecuteDataset("SELECT * FROM VARC_RoleCenterTab WHERE VARC_RoleCenterManager_ID=" + ds.Tables[0].Rows[i]["VARC_RoleCenterManager_ID"]);
                            if (rcTab != null)
                            {
                                for (int j = 0; j < rcTab.Tables[0].Rows.Count; j++)
                                {
                                    tab = new X_VARC_RoleCenterTab(m_ctx, 0, m_trx);
                                    tab.SetVAF_Client_ID(m_client.Get_ID());
                                    tab.SetVAF_Org_ID(0);
                                    tab.SetIsActive(true);
                                    tab.SetVARC_RoleCenterManager_ID(rcmngr.Get_ID());
                                    if (rcTab.Tables[0].Rows[j]["Name"] != null && rcTab.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                    {
                                        tab.SetName(rcTab.Tables[0].Rows[j]["Name"].ToString());
                                    }
                                    if (rcTab.Tables[0].Rows[j]["SeqNo"] != null && rcTab.Tables[0].Rows[j]["SeqNo"] != DBNull.Value)
                                    {
                                        tab.SetSeqNo(Util.GetValueOfInt(rcTab.Tables[0].Rows[j]["SeqNo"]));
                                    }
                                    if (rcTab.Tables[0].Rows[j]["VAF_Image_ID"] != null && rcTab.Tables[0].Rows[j]["VAF_Image_ID"] != DBNull.Value)
                                    {
                                        tab.SetVAF_Image_ID(Util.GetValueOfInt(rcTab.Tables[0].Rows[j]["VAF_Image_ID"]));
                                    }
                                    if (!tab.Save(m_trx))
                                    {
                                        log.Info(tab.GetName() + " RoleCenterTabNotSaved");
                                    }
                                    else
                                    {
                                        dsPanel = DB.ExecuteDataset("SELECT * FROM VARC_TabPanels WHERE VARC_RoleCenterTab_ID =" + rcTab.Tables[0].Rows[j]["VARC_RoleCenterTab_ID"]);
                                        if (dsPanel != null)
                                        {
                                            for (int k = 0; k < dsPanel.Tables[0].Rows.Count; k++)
                                            {
                                                panel = new X_VARC_TabPanels(m_ctx, 0, m_trx);
                                                panel.SetVAF_Client_ID(m_client.Get_ID());
                                                panel.SetVAF_Org_ID(0);
                                                panel.SetVARC_RoleCenterTab_ID(tab.Get_ID());
                                                panel.SetIsActive(true);
                                                if (dsPanel.Tables[0].Rows[k]["SeqNo"] != null && dsPanel.Tables[0].Rows[k]["SeqNo"] != DBNull.Value)
                                                {
                                                    panel.SetSeqNo(Util.GetValueOfInt(dsPanel.Tables[0].Rows[k]["SeqNo"]));
                                                }
                                                if (dsPanel.Tables[0].Rows[k]["RoleCenterPanels"] != null && dsPanel.Tables[0].Rows[k]["RoleCenterPanels"] != DBNull.Value)
                                                {
                                                    panel.SetRoleCenterPanels(dsPanel.Tables[0].Rows[k]["RoleCenterPanels"].ToString());
                                                }
                                                if (dsPanel.Tables[0].Rows[k]["Record_ID"] != null && dsPanel.Tables[0].Rows[k]["Record_ID"] != DBNull.Value)
                                                {
                                                    panel.SetRecord_ID(Util.GetValueOfInt(dsPanel.Tables[0].Rows[k]["Record_ID"]));
                                                }
                                                if (dsPanel.Tables[0].Rows[k]["VAF_UserSearch_ID"] != null && dsPanel.Tables[0].Rows[k]["VAF_UserSearch_ID"] != DBNull.Value)
                                                {
                                                    panel.SetVAF_UserSearch_ID(Util.GetValueOfInt(dsPanel.Tables[0].Rows[k]["VAF_UserSearch_ID"]));
                                                }
                                                if (dsPanel.Tables[0].Rows[k]["Rowspan"] != null && dsPanel.Tables[0].Rows[k]["Rowspan"] != DBNull.Value)
                                                {
                                                    panel.SetRowspan(Util.GetValueOfInt(dsPanel.Tables[0].Rows[k]["Rowspan"]));
                                                }
                                                if (dsPanel.Tables[0].Rows[k]["Colspan"] != null && dsPanel.Tables[0].Rows[k]["Colspan"] != DBNull.Value)
                                                {
                                                    panel.SetColspan(Util.GetValueOfInt(dsPanel.Tables[0].Rows[k]["Colspan"]));
                                                }
                                                if (!panel.Save(m_trx))
                                                {
                                                    log.Info(panel.GetSeqNo() + " RoleCenterPanelNotSaved");
                                                }

                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
            }
        }
        private void CopyDashBoard(int role_ID)
        {
            tableName = "D_Chart";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                DataSet ds = DB.ExecuteDataset("SELECT * FROM D_Chart WHERE vaf_client_ID=0 AND VAF_ORG_ID=0 AND IsForNewTenant='Y'");
                if (ds != null)
                {
                    X_D_Chart chart = null;
                    X_D_ChartRights cAcess = null;
                    X_D_Series series = null;
                    X_D_SeriesFilter filter = null;
                    DataSet dsAs = null;
                    DataSet dss = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        chart = new X_D_Chart(m_ctx, 0, m_trx);
                        chart.SetVAF_Client_ID(m_client.Get_ID());
                        chart.SetVAF_Org_ID(0);
                        chart.SetIsActive(true);
                        if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                        {
                            chart.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["ChartType"] != null && ds.Tables[0].Rows[i]["ChartType"] != DBNull.Value)
                        {
                            chart.SetChartType(ds.Tables[0].Rows[i]["ChartType"].ToString());
                        }
                        if (ds.Tables[0].Rows[i]["Ad_Chart_BG_Color_ID"] != null && ds.Tables[0].Rows[i]["Ad_Chart_BG_Color_ID"] != DBNull.Value)
                        {
                            chart.SetAd_Chart_BG_Color_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Ad_Chart_BG_Color_ID"]));
                        }
                        if (ds.Tables[0].Rows[i]["Enable3D"] != null && ds.Tables[0].Rows[i]["Enable3D"] != DBNull.Value)
                        {
                            chart.SetEnable3D((ds.Tables[0].Rows[i]["Enable3D"].ToString().Equals("Y")));
                        }
                        if (ds.Tables[0].Rows[i]["SeqNo"] != null && ds.Tables[0].Rows[i]["SeqNo"] != DBNull.Value)
                        {
                            chart.SetSeqNo(Util.GetValueOfInt(ds.Tables[0].Rows[i]["SeqNo"]));
                        }
                        if (ds.Tables[0].Rows[i]["IsShowLegend"] != null && ds.Tables[0].Rows[i]["IsShowLegend"] != DBNull.Value)
                        {
                            chart.SetIsShowLegend((ds.Tables[0].Rows[i]["IsShowLegend"].ToString().Equals("Y")));
                        }
                        if (ds.Tables[0].Rows[i]["IsShowZeroSeries"] != null && ds.Tables[0].Rows[i]["IsShowZeroSeries"] != DBNull.Value)
                        {
                            chart.SetIsShowZeroSeries((ds.Tables[0].Rows[i]["IsShowZeroSeries"].ToString().Equals("Y")));
                        }
                        if (!chart.Save(m_trx))
                        {
                            log.Info(chart.GetName() + " DashboardNotSaved");
                        }
                        else
                        {
                            dsAs = DB.ExecuteDataset("SELECT * FROM D_ChartRights WHERE D_CHART_ID =" + ds.Tables[0].Rows[i]["D_CHART_ID"]);
                            if (dsAs != null)
                            {
                                for (int j = 0; j < dsAs.Tables[0].Rows.Count; j++)
                                {
                                    cAcess = new X_D_ChartRights(m_ctx, 0, m_trx);
                                    cAcess.SetVAF_Client_ID(m_client.Get_ID());
                                    cAcess.SetVAF_Org_ID(0);
                                    cAcess.SetIsActive(true);
                                    cAcess.SetD_Chart_ID(chart.Get_ID());
                                    //if (dsAs.Tables[0].Rows[j]["VAF_ROLE_ID"] != null && dsAs.Tables[0].Rows[j]["VAF_ROLE_ID"] != DBNull.Value)
                                    //{
                                    //    cAcess.SetVAF_Role_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["VAF_ROLE_ID"]));
                                    //}
                                    cAcess.SetVAF_Role_ID(role_ID);
                                    if (dsAs.Tables[0].Rows[j]["IsReadWrite"] != null && dsAs.Tables[0].Rows[j]["IsReadWrite"] != DBNull.Value)
                                    {
                                        cAcess.SetIsReadWrite((dsAs.Tables[0].Rows[j]["IsReadWrite"].ToString().Equals("Y")));
                                    }
                                    if (!cAcess.Save(m_trx))
                                    {
                                        log.Info(chart.GetName() + " DashboardNotSaved");
                                    }
                                }
                            }
                            dsAs = DB.ExecuteDataset("SELECT * FROM D_Series WHERE D_CHART_ID =" + ds.Tables[0].Rows[i]["D_CHART_ID"]);
                            if (dsAs != null)
                            {
                                for (int j = 0; j < dsAs.Tables[0].Rows.Count; j++)
                                {
                                    series = new X_D_Series(m_ctx, 0, m_trx);
                                    series.SetVAF_Client_ID(m_client.Get_ID());
                                    series.SetVAF_Org_ID(0);
                                    series.SetIsActive(true);
                                    series.SetD_Chart_ID(chart.Get_ID());
                                    if (dsAs.Tables[0].Rows[j]["Name"] != null && dsAs.Tables[0].Rows[j]["Name"] != DBNull.Value)
                                    {
                                        series.SetName(dsAs.Tables[0].Rows[j]["Name"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["AD_Series_Color_ID"] != null && dsAs.Tables[0].Rows[j]["AD_Series_Color_ID"] != DBNull.Value)
                                    {
                                        series.SetAD_Series_Color_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["AD_Series_Color_ID"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsLogarithmic"] != null && dsAs.Tables[0].Rows[j]["IsLogarithmic"] != DBNull.Value)
                                    {
                                        series.SetIsLogarithmic(dsAs.Tables[0].Rows[j]["IsLogarithmic"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsShowLabel"] != null && dsAs.Tables[0].Rows[j]["IsShowLabel"] != DBNull.Value)
                                    {
                                        series.SetIsShowLabel(dsAs.Tables[0].Rows[j]["IsShowLabel"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["LogarithmicBase"] != null && dsAs.Tables[0].Rows[j]["LogarithmicBase"] != DBNull.Value)
                                    {
                                        series.SetLogarithmicBase(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["LogarithmicBase"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["VAF_TableView_ID"] != null && dsAs.Tables[0].Rows[j]["VAF_TableView_ID"] != DBNull.Value)
                                    {
                                        series.SetVAF_TableView_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["VAF_TableView_ID"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsShowXAsix"] != null && dsAs.Tables[0].Rows[j]["IsShowXAsix"] != DBNull.Value)
                                    {
                                        series.SetIsShowXAsix(dsAs.Tables[0].Rows[j]["IsShowXAsix"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["DataType_X"] != null && dsAs.Tables[0].Rows[j]["DataType_X"] != DBNull.Value)
                                    {
                                        series.SetDataType_X(dsAs.Tables[0].Rows[j]["DataType_X"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["VAF_Column_X_ID"] != null && dsAs.Tables[0].Rows[j]["VAF_Column_X_ID"] != DBNull.Value)
                                    {
                                        series.SetVAF_Column_X_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["VAF_Column_X_ID"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["DateTimeTypes"] != null && dsAs.Tables[0].Rows[j]["DateTimeTypes"] != DBNull.Value)
                                    {
                                        series.SetDateTimeTypes(dsAs.Tables[0].Rows[j]["DateTimeTypes"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["AD_DateColumn_ID"] != null && dsAs.Tables[0].Rows[j]["AD_DateColumn_ID"] != DBNull.Value)
                                    {
                                        series.SetAD_DateColumn_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["AD_DateColumn_ID"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["DateFrom"] != null && dsAs.Tables[0].Rows[j]["DateFrom"] != DBNull.Value)
                                    {
                                        series.SetDateFrom(Util.GetValueOfDateTime(dsAs.Tables[0].Rows[j]["DateFrom"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["DateTo"] != null && dsAs.Tables[0].Rows[j]["DateTo"] != DBNull.Value)
                                    {
                                        series.SetDateTo(Util.GetValueOfDateTime(dsAs.Tables[0].Rows[j]["DateTo"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["LastNValue"] != null && dsAs.Tables[0].Rows[j]["LastNValue"] != DBNull.Value)
                                    {
                                        series.SetLastNValue(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["LastNValue"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["YAxisLabel"] != null && dsAs.Tables[0].Rows[j]["YAxisLabel"] != DBNull.Value)
                                    {
                                        series.SetYAxisLabel(dsAs.Tables[0].Rows[j]["YAxisLabel"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsShowYAxis"] != null && dsAs.Tables[0].Rows[j]["IsShowYAxis"] != DBNull.Value)
                                    {
                                        series.SetIsShowYAxis(dsAs.Tables[0].Rows[j]["IsShowYAxis"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["VAF_Column_Y_ID"] != null && dsAs.Tables[0].Rows[j]["VAF_Column_Y_ID"] != DBNull.Value)
                                    {
                                        series.SetVAF_Column_Y_ID(Util.GetValueOfInt(dsAs.Tables[0].Rows[j]["VAF_Column_Y_ID"]));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsSum"] != null && dsAs.Tables[0].Rows[j]["IsSum"] != DBNull.Value)
                                    {
                                        series.SetIsSum(dsAs.Tables[0].Rows[j]["IsSum"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsAvg"] != null && dsAs.Tables[0].Rows[j]["IsAvg"] != DBNull.Value)
                                    {
                                        series.SetIsAvg(dsAs.Tables[0].Rows[j]["IsAvg"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsCount"] != null && dsAs.Tables[0].Rows[j]["IsCount"] != DBNull.Value)
                                    {
                                        series.SetIsCount(dsAs.Tables[0].Rows[j]["IsCount"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsNone"] != null && dsAs.Tables[0].Rows[j]["IsNone"] != DBNull.Value)
                                    {
                                        series.SetIsNone(dsAs.Tables[0].Rows[j]["IsNone"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["WhereClause"] != null && dsAs.Tables[0].Rows[j]["WhereClause"] != DBNull.Value)
                                    {
                                        series.SetWhereClause(dsAs.Tables[0].Rows[j]["WhereClause"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["OrderByColumn"] != null && dsAs.Tables[0].Rows[j]["OrderByColumn"] != DBNull.Value)
                                    {
                                        series.SetOrderByColumn(dsAs.Tables[0].Rows[j]["OrderByColumn"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsOrderByAsc"] != null && dsAs.Tables[0].Rows[j]["IsOrderByAsc"] != DBNull.Value)
                                    {
                                        series.SetIsOrderByAsc(dsAs.Tables[0].Rows[j]["IsOrderByAsc"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["IsSetAlert"] != null && dsAs.Tables[0].Rows[j]["IsSetAlert"] != DBNull.Value)
                                    {
                                        series.SetIsSetAlert(dsAs.Tables[0].Rows[j]["IsSetAlert"].ToString().Equals("Y"));
                                    }
                                    if (dsAs.Tables[0].Rows[j]["WhereCondition"] != null && dsAs.Tables[0].Rows[j]["WhereCondition"] != DBNull.Value)
                                    {
                                        series.SetWhereCondition(dsAs.Tables[0].Rows[j]["WhereCondition"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["AlertValue"] != null && dsAs.Tables[0].Rows[j]["AlertValue"] != DBNull.Value)
                                    {
                                        series.SetAlertValue(dsAs.Tables[0].Rows[j]["AlertValue"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["ValueTo"] != null && dsAs.Tables[0].Rows[j]["ValueTo"] != DBNull.Value)
                                    {
                                        series.SetValueTo(dsAs.Tables[0].Rows[j]["ValueTo"].ToString());
                                    }
                                    if (dsAs.Tables[0].Rows[j]["AlertMessage"] != null && dsAs.Tables[0].Rows[j]["AlertMessage"] != DBNull.Value)
                                    {
                                        series.SetAlertMessage(dsAs.Tables[0].Rows[j]["AlertMessage"].ToString());
                                    }
                                    if (!series.Save(m_trx))
                                    {
                                        log.Info(series.GetName() + " DashboardSeriesNotSaved");
                                    }
                                    else
                                    {
                                        dss = DB.ExecuteDataset("SELECT * FROM D_SeriesFilter WHERE D_Series_ID= " + dsAs.Tables[0].Rows[j]["D_Series_ID"]);
                                        if (dss != null)
                                        {
                                            for (int k = 0; k < dss.Tables[0].Rows.Count; k++)
                                            {
                                                filter = new X_D_SeriesFilter(m_ctx, 0, m_trx);
                                                filter.SetVAF_Client_ID(m_client.Get_ID());
                                                filter.SetVAF_Org_ID(0);
                                                filter.SetD_Series_ID(series.Get_ID());
                                                filter.SetIsActive(true);
                                                if (dss.Tables[0].Rows[k]["VAF_TableView_ID"] != null && dss.Tables[0].Rows[k]["VAF_TableView_ID"] != DBNull.Value)
                                                {
                                                    filter.SetVAF_TableView_ID(Util.GetValueOfInt(dss.Tables[0].Rows[k]["VAF_TableView_ID"]));
                                                }
                                                if (dss.Tables[0].Rows[k]["VAF_Column_ID"] != null && dss.Tables[0].Rows[k]["VAF_Column_ID"] != DBNull.Value)
                                                {
                                                    filter.SetVAF_Column_ID(Util.GetValueOfInt(dss.Tables[0].Rows[k]["VAF_Column_ID"]));
                                                }
                                                if (dss.Tables[0].Rows[k]["WhereCondition"] != null && dss.Tables[0].Rows[k]["WhereCondition"] != DBNull.Value)
                                                {
                                                    filter.SetWhereCondition((dss.Tables[0].Rows[k]["WhereCondition"].ToString()));
                                                }
                                                if (dss.Tables[0].Rows[k]["WhereValue"] != null && dss.Tables[0].Rows[k]["WhereValue"] != DBNull.Value)
                                                {
                                                    filter.SetWhereValue((dss.Tables[0].Rows[k]["WhereValue"].ToString()));
                                                }
                                                if (dss.Tables[0].Rows[k]["ValueTo"] != null && dss.Tables[0].Rows[k]["ValueTo"] != DBNull.Value)
                                                {
                                                    filter.SetValueTo((dss.Tables[0].Rows[k]["ValueTo"].ToString()));
                                                }
                                                if (!filter.Save(m_trx))
                                                {
                                                    log.Info(filter.GetName() + " DashboardfilterNotSaved");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
        private void CopyOrgType()
        {
            DataSet ds = DB.ExecuteDataset(@"Select * From VAF_OrgCategory Where ISACTIVE='Y' AND vaf_client_ID=0 AND IsForNewTenant='Y' ");
            if (ds != null)
            {
                X_VAF_OrgCategory orgType = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    orgType = new X_VAF_OrgCategory(m_ctx, 0, m_trx);
                    orgType.SetVAF_Client_ID(m_client.GetVAF_Client_ID());
                    orgType.SetVAF_Org_ID(0);
                    orgType.SetIsActive(true);
                    if (ds.Tables[0].Rows[i]["Name"] != null && ds.Tables[0].Rows[i]["Name"] != DBNull.Value)
                    {
                        orgType.SetName(ds.Tables[0].Rows[i]["Name"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Description"] != null && ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                    {
                        orgType.SetDescription(ds.Tables[0].Rows[i]["Description"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["VAF_Print_Rpt_Colour_ID"] != null && ds.Tables[0].Rows[i]["VAF_Print_Rpt_Colour_ID"] != DBNull.Value)
                    {
                        orgType.SetVAF_Print_Rpt_Colour_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Print_Rpt_Colour_ID"]));
                    }
                    if (!orgType.Save(m_trx))
                    {
                        log.Info(orgType.GetName() + " OrgTypeNotSaved");
                    }
                }
            }

        }



        public bool CreateAccounting(KeyNamePair currency, bool hasProduct, bool hasBPartner, bool hasProject, bool hasMCampaign, bool hasSRegion, FileStream AccountingFile)
        {
            log.Info(m_client.ToString());
            //
            m_hasProject = hasProject;
            m_hasMCampaign = hasMCampaign;
            m_hasSRegion = hasSRegion;

            //  Standard variables
            m_info = new StringBuilder();
            String name = null;
            StringBuilder sqlCmd = null;
            int no = 0;

            /**
             *  Create Calendar
             */
            if (lstTableName.Contains("VAB_Calender")) // Update by Paramjeet Singh
            {
                m_calendar = new MVABCalendar(m_client);



                if (!m_calendar.Save())
                {
                    String err = "Calendar NOT inserted";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }

                //  Info
                m_info.Append(Msg.Translate(m_lang, "VAB_Calender_ID")).Append("=").Append(m_calendar.GetName()).Append("\n");

                if (m_calendar.CreateYear(m_client.GetLocale()) == null)
                    log.Log(Level.SEVERE, "Year NOT inserted");
            }

            //	Create Account Elements
            name = m_clientName + " " + Msg.Translate(m_lang, "Account_ID");
            int VAB_Acct_Element_ID = 0;
            int VAB_Element_ID = 0;
            if (lstTableName.Contains("VAB_Element")) // Update by Paramjeet Singh
            {
                MElement element = new MElement(m_client, name, MElement.ELEMENTTYPE_Account, m_VAF_TreeInfo_Account_ID);



                if (!element.Save())
                {
                    String err = "Acct Element NOT inserted";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }

                VAB_Element_ID = element.GetVAB_Element_ID();

                m_info.Append(Msg.Translate(m_lang, "VAB_Element_ID")).Append("=").Append(name).Append("\n");

                //	Create Account Values
                m_nap = new NaturalAccountMap<String, MElementValue>(m_ctx, m_trx);
                MVAFTreeInfo tree = MVAFTreeInfo.Get(m_ctx, m_VAF_TreeInfo_Account_ID, m_trx);
                String errMsg = m_nap.ParseFile(AccountingFile, GetVAF_Client_ID(), GetVAF_Org_ID(), VAB_Element_ID, tree);
                if (errMsg.Length != 0)
                {
                    log.Log(Level.SEVERE, errMsg);
                    //result = errMsg;
                    m_info.Append(errMsg);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }

                //if (m_nap.SaveAccounts(GetVAF_Client_ID(), GetVAF_Org_ID(), VAB_Element_ID))
                //    m_info.Append(Msg.Translate(m_lang, "VAB_Acct_Element_ID")).Append(" # ").Append(m_nap.Count).Append("\n");
                //else
                //{
                //    String err = "Acct Element Values NOT inserted";
                //    log.Log(Level.SEVERE, err);
                //    m_info.Append(err);
                //    m_trx.Rollback();
                //    m_trx.Close();
                //    return false;
                //}

                VAB_Acct_Element_ID = m_nap.GetVAB_Acct_Element_ID("DEFAULT_ACCT");
                log.Fine("VAB_Acct_Element_ID=" + VAB_Acct_Element_ID);
            }
            /**
             *  Create AccountingSchema
             */
            if (lstTableName.Contains("VAB_AccountBook"))// Update by Paramjeet Singh
            {
                m_as = new MVABAccountBook(m_client, currency);
                if (!m_as.Save())
                {
                    String err = "AcctSchema NOT inserted";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }

                //  Info
                m_info.Append(Msg.Translate(m_lang, "VAB_AccountBook_ID")).Append("=").Append(m_as.GetName()).Append("\n");
            }
            /**
             *  Create AccountingSchema Elements (Structure)
             */
            String sql2 = null;
            if (Env.IsBaseLanguage(m_lang, "VAF_Control_Ref"))	//	Get ElementTypes & Name
                sql2 = "SELECT Value, Name FROM VAF_CtrlRef_List WHERE VAF_Control_Ref_ID=181";
            else
                sql2 = "SELECT l.Value, t.Name FROM VAF_CtrlRef_List l, VAF_CtrlRef_TL t "
                    + "WHERE l.VAF_Control_Ref_ID=181 AND l.VAF_CtrlRef_List_ID=t.VAF_CtrlRef_List_ID";
            //
            int Element_OO = 0, Element_AC = 0, Element_PR = 0, Element_BP = 0, Element_PJ = 0,
                Element_MC = 0, Element_SR = 0;

            try
            {
                if (lstTableName.Contains("VAB_AccountBook_Element"))// Update by Paramjeet Singh
                {
                    int VAF_Client_ID = m_client.GetVAF_Client_ID();
                    DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql2, null, m_trx);

                    for (int count = 0; count <= ds.Tables[0].Rows.Count - 1; count++)
                    {
                        String ElementType = ds.Tables[0].Rows[count][0].ToString();
                        name = ds.Tables[0].Rows[count][1].ToString();
                        //
                        String IsMandatory = null;
                        String IsBalanced = "N";
                        int SeqNo = 0;
                        int VAB_AccountBook_Element_ID = 0;

                        if (ElementType.Equals("OO"))
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_OO = VAB_AccountBook_Element_ID;
                            IsMandatory = "Y";
                            IsBalanced = "Y";
                            SeqNo = 10;
                        }
                        else if (ElementType.Equals("AC"))
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_AC = VAB_AccountBook_Element_ID;
                            IsMandatory = "Y";
                            SeqNo = 20;
                        }
                        else if (ElementType.Equals("PR") && hasProduct)
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_PR = VAB_AccountBook_Element_ID;
                            IsMandatory = "N";
                            SeqNo = 30;
                        }
                        else if (ElementType.Equals("BP") && hasBPartner)
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_BP = VAB_AccountBook_Element_ID;
                            IsMandatory = "N";
                            SeqNo = 40;
                        }
                        else if (ElementType.Equals("PJ") && hasProject)
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_PJ = VAB_AccountBook_Element_ID;
                            IsMandatory = "N";
                            SeqNo = 50;
                        }
                        else if (ElementType.Equals("MC") && hasMCampaign)
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_MC = VAB_AccountBook_Element_ID;
                            IsMandatory = "N";
                            SeqNo = 60;
                        }
                        else if (ElementType.Equals("SR") && hasSRegion)
                        {
                            VAB_AccountBook_Element_ID = GetNextID(VAF_Client_ID, "VAB_AccountBook_Element");
                            Element_SR = VAB_AccountBook_Element_ID;
                            IsMandatory = "N";
                            SeqNo = 70;
                        }
                        //	Not OT, LF, LT, U1, U2, AY

                        if (IsMandatory != null)
                        {
                            //tableName = "VAB_AccountBook_Element";
                            //if (lstTableName.Contains(tableName))// Update by Paramjeet Singh
                            //{
                            sqlCmd = new StringBuilder("INSERT INTO VAB_AccountBook_Element(");
                            sqlCmd.Append(m_stdColumns).Append(",VAB_AccountBook_Element_ID,VAB_AccountBook_ID,")
                                .Append("ElementType,Name,SeqNo,IsMandatory,IsBalanced) VALUES (");
                            sqlCmd.Append(m_stdValues).Append(",").Append(VAB_AccountBook_Element_ID).Append(",").Append(m_as.GetVAB_AccountBook_ID()).Append(",")
                                .Append("'").Append(ElementType).Append("','").Append(name).Append("',").Append(SeqNo).Append(",'")
                                .Append(IsMandatory).Append("','").Append(IsBalanced).Append("')");
                            try
                            {
                                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                            }
                            catch
                            {
                            }

                            if (no == 1)
                                m_info.Append(Msg.Translate(m_lang, "VAB_AccountBook_Element_ID")).Append("=").Append(name).Append("\n");
                            //}
                            /** Default value for mandatory elements: OO and AC */
                            if (ElementType.Equals("OO"))
                            {
                                sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET Org_ID=");
                                sqlCmd.Append(GetVAF_Org_ID()).Append(" WHERE VAB_AccountBook_Element_ID=").Append(VAB_AccountBook_Element_ID);
                                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                                if (no != 1)
                                    log.Log(Level.SEVERE, "Default Org in AcctSchamaElement NOT updated");
                            }
                            if (ElementType.Equals("AC"))
                            {
                                sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET VAB_Acct_Element_ID=");
                                sqlCmd.Append(VAB_Acct_Element_ID).Append(", VAB_Element_ID=").Append(VAB_Element_ID);
                                sqlCmd.Append(" WHERE VAB_AccountBook_Element_ID=").Append(VAB_AccountBook_Element_ID);
                                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                                if (no != 1)
                                    log.Log(Level.SEVERE, "Default Account in AcctSchamaElement NOT updated");
                            }
                        }
                    }
                }
                //rs.Close();

            }
            catch (Exception e1)
            {
                //if (rs != null)
                //{
                //    rs.Close();
                //}
                log.Log(Level.SEVERE, "Elements", e1);
                //result = e1.Message + " Catch1";
                m_info.Append(e1.Message);
                m_trx.Rollback();
                m_trx.Close();
                return false;
            }
            //  Create AcctSchema


            //  Create GL Accounts
            m_accountsOK = true;
            tableName = "VAB_AccountBook_GL";
            if (lstTableName.Contains(tableName))// Update by Paramjeet Singh
            {
                sqlCmd = new StringBuilder("INSERT INTO VAB_AccountBook_GL (");
                sqlCmd.Append(m_stdColumns).Append(",VAB_AccountBook_ID,"
                    /*jz
                        + "USESUSPENSEBALANCING,SUSPENSEBALANCING_Acct,"
                        + "USESUSPENSEERROR,SUSPENSEERROR_Acct,"
                        + "USECURRENCYBALANCING,CURRENCYBALANCING_Acct,"
                        + "RETAINEDEARNING_Acct,INCOMESUMMARY_Acct,"
                        + "INTERCOMPANYDUETO_Acct,INTERCOMPANYDUEFROM_Acct,"
                        + "PPVOFFSET_Acct, CommitmentOffset_Acct) VALUES (");
                    sqlCmd.Append(m_stdValues).Append(",").Append(m_as.GetVAB_AccountBook_ID()).Append(",")
                        .Append("'Y',").Append(GetAcct("SUSPENSEBALANCING_Acct")).Append(",")
                        .Append("'Y',").Append(GetAcct("SUSPENSEERROR_Acct")).Append(",")
                        .Append("'Y',").Append(GetAcct("CURRENCYBALANCING_Acct")).Append(",");
                    //  RETAINEDEARNING_Acct,INCOMESUMMARY_Acct,
                    sqlCmd.Append(GetAcct("RETAINEDEARNING_Acct")).Append(",")
                        .Append(GetAcct("INCOMESUMMARY_Acct")).Append(",")
                    //  INTERCOMPANYDUETO_Acct,INTERCOMPANYDUEFROM_Acct)
                        .Append(GetAcct("INTERCOMPANYDUETO_Acct")).Append(",")
                        .Append(GetAcct("INTERCOMPANYDUEFROM_Acct")).Append(",")
                        .Append(GetAcct("PPVOFFSET_Acct")).Append(",")
                        */
                    + "UseSuspenseBalancing,SuspenseBalancing_Acct,"
                    + "UseSuspenseError,SuspenseError_Acct,"
                    + "UseCurrencyBalancing,CurrencyBalancing_Acct,"
                    + "RetainedEarning_Acct,IncomeSummary_Acct,"
                    + "IntercompanyDueTo_Acct,IntercompanyDueFrom_Acct,"
                    + "PPVOffset_Acct, CommitmentOffset_Acct) VALUES (");
                sqlCmd.Append(m_stdValues).Append(",").Append(m_as.GetVAB_AccountBook_ID()).Append(",")
                    .Append("'Y',").Append(GetAcct("SuspenseBalancing_Acct")).Append(",")
                    .Append("'Y',").Append(GetAcct("SuspenseError_Acct")).Append(",")
                    .Append("'Y',").Append(GetAcct("CurrencyBalancing_Acct")).Append(",");
                //  RETAINEDEARNING_Acct,INCOMESUMMARY_Acct,
                sqlCmd.Append(GetAcct("RetainedEarning_Acct")).Append(",")
                    .Append(GetAcct("INCOMESUMMARY_Acct")).Append(",")
                    //  INTERCOMPANYDUETO_Acct,INTERCOMPANYDUEFROM_Acct)
                    .Append(GetAcct("IntercompanyDueTo_Acct")).Append(",")
                    .Append(GetAcct("IntercompanyDueFrom_Acct")).Append(",")
                    .Append(GetAcct("PPVOffset_Acct")).Append(",")
                    .Append(GetAcct("CommitmentOffset_Acct"))
                    .Append(")");
                if (m_accountsOK)
                    no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                else
                    no = -1;
                if (no != 1)
                {
                    String err = "GL Accounts NOT inserted";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
            }
            //	Create Std Accounts
            tableName = "VAB_AccountBook_GL";
            if (lstTableName.Contains(tableName))// Update by Paramjeet Singh
            {
                sqlCmd = new StringBuilder("INSERT INTO VAB_AccountBook_Default (");
                sqlCmd.Append(m_stdColumns).Append(",VAB_AccountBook_ID,"
                    + "W_Inventory_Acct,W_Differences_Acct,W_Revaluation_Acct,W_InvActualAdjust_Acct, "
                    + "P_Revenue_Acct,P_Expense_Acct,P_CostAdjustment_Acct,P_InventoryClearing_Acct,P_Asset_Acct,P_COGS_Acct, "
                    + "P_PurchasePriceVariance_Acct,P_InvoicePriceVariance_Acct,P_TradeDiscountRec_Acct,P_TradeDiscountGrant_Acct, "
                    + "C_Receivable_Acct,C_Receivable_Services_Acct,C_Prepayment_Acct, "
                    + "V_Liability_Acct,V_Liability_Services_Acct,V_Prepayment_Acct, "
                    + "PayDiscount_Exp_Acct,PayDiscount_Rev_Acct,WriteOff_Acct, "
                    + "UnrealizedGain_Acct,UnrealizedLoss_Acct,RealizedGain_Acct,RealizedLoss_Acct, "
                    + "Withholding_Acct,E_Prepayment_Acct,E_Expense_Acct, "
                    + "PJ_Asset_Acct,PJ_WIP_Acct,"
                    + "T_Expense_Acct,T_Liability_Acct,T_Receivables_Acct,T_Due_Acct,T_Credit_Acct, "
                    + "B_InTransit_Acct,B_Asset_Acct,B_Expense_Acct,B_InterestRev_Acct,B_InterestExp_Acct,"
                    + "B_Unidentified_Acct,B_SettlementGain_Acct,B_SettlementLoss_Acct,"
                    + "B_RevaluationGain_Acct,B_RevaluationLoss_Acct,B_PaymentSelect_Acct,B_UnallocatedCash_Acct, "
                    + "Ch_Expense_Acct,Ch_Revenue_Acct, "
                    + "UnEarnedRevenue_Acct,NotInvoicedReceivables_Acct,NotInvoicedRevenue_Acct,NotInvoicedReceipts_Acct, "
                    + "CB_Asset_Acct,CB_CashTransfer_Acct,CB_Differences_Acct,CB_Expense_Acct,CB_Receipt_Acct,"
                    + "WO_MATERIAL_ACCT,WO_MATERIALOVERHD_ACCT,WO_RESOURCE_ACCT,WC_OVERHEAD_ACCT,P_MATERIALOVERHD_ACCT,"
                    + "WO_MATERIALVARIANCE_ACCT,WO_MATERIALOVERHDVARIANCE_ACCT,WO_RESOURCEVARIANCE_ACCT,WO_OVERHDVARIANCE_ACCT,"
                    + "WO_SCRAP_ACCT,P_Resource_Absorption_Acct,Overhead_Absorption_Acct) VALUES (");
                //+ "ASSET_DEPRECIATION_ACCT,ASSET_DISP_REVENUE_ACCT) VALUES (");
                sqlCmd.Append(m_stdValues).Append(",").Append(m_as.GetVAB_AccountBook_ID()).Append(",");
                //  W_INVENTORY_Acct,W_Differences_Acct,W_REVALUATION_Acct,W_INVACTUALADJUST_Acct
                sqlCmd.Append(GetAcct("W_Inventory_Acct")).Append(",");
                sqlCmd.Append(GetAcct("W_Differences_Acct")).Append(",");
                sqlCmd.Append(GetAcct("W_Revaluation_Acct")).Append(",");
                sqlCmd.Append(GetAcct("W_InvActualAdjust_Acct")).Append(", ");
                //  P_Revenue_Acct,P_Expense_Acct,P_Asset_Acct,P_COGS_Acct,
                sqlCmd.Append(GetAcct("P_Revenue_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_Expense_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_CostAdjustment_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_InventoryClearing_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_Asset_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_COGS_Acct")).Append(", ");
                //  P_PURCHASEPRICEVARIANCE_Acct,P_INVOICEPRICEVARIANCE_Acct,P_TRADEDISCOUNTREC_Acct,P_TRADEDISCOUNTGRANT_Acct,
                sqlCmd.Append(GetAcct("P_PurchasePriceVariance_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_InvoicePriceVariance_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_TradeDiscountRec_Acct")).Append(",");
                sqlCmd.Append(GetAcct("P_TradeDiscountGrant_Acct")).Append(", ");
                //  C_RECEIVABLE_Acct,C_Receivable_Services_Acct,C_PREPAYMENT_Acct,
                sqlCmd.Append(GetAcct("C_Receivable_Acct")).Append(",");
                sqlCmd.Append(GetAcct("C_Receivable_Services_Acct")).Append(",");
                sqlCmd.Append(GetAcct("C_Prepayment_Acct")).Append(", ");
                //  V_LIABILITY_Acct,V_LIABILITY_Services_Acct,V_Prepayment_Acct,
                sqlCmd.Append(GetAcct("V_Liability_Acct")).Append(",");
                sqlCmd.Append(GetAcct("V_Liability_Services_Acct")).Append(",");
                sqlCmd.Append(GetAcct("V_Prepayment_Acct")).Append(", ");
                //  PAYDISCOUNT_EXP_Acct,PAYDISCOUNT_REV_Acct,WRITEOFF_Acct,
                sqlCmd.Append(GetAcct("PayDiscount_Exp_Acct")).Append(",");
                sqlCmd.Append(GetAcct("PayDiscount_Rev_Acct")).Append(",");
                sqlCmd.Append(GetAcct("WriteOff_Acct")).Append(", ");
                //  UNREALIZEDGAIN_Acct,UNREALIZEDLOSS_Acct,REALIZEDGAIN_Acct,REALIZEDLOSS_Acct,
                sqlCmd.Append(GetAcct("UnrealizedGain_Acct")).Append(",");
                sqlCmd.Append(GetAcct("UnrealizedLoss_Acct")).Append(",");
                sqlCmd.Append(GetAcct("RealizedGain_Acct")).Append(",");
                sqlCmd.Append(GetAcct("RealizedLoss_Acct")).Append(", ");
                //  WITHHOLDING_Acct,E_Prepayment_Acct,E_Expense_Acct,
                sqlCmd.Append(GetAcct("Withholding_Acct")).Append(",");
                sqlCmd.Append(GetAcct("E_Prepayment_Acct")).Append(",");
                sqlCmd.Append(GetAcct("E_Expense_Acct")).Append(", ");
                //  PJ_Asset_Acct,PJ_WIP_Acct,
                sqlCmd.Append(GetAcct("PJ_Asset_Acct")).Append(",");
                sqlCmd.Append(GetAcct("PJ_WIP_Acct")).Append(",");
                //  T_Expense_Acct,T_Liability_Acct,T_Receivables_Acct,T_DUE_Acct,T_CREDIT_Acct,
                sqlCmd.Append(GetAcct("T_Expense_Acct")).Append(",");
                sqlCmd.Append(GetAcct("T_Liability_Acct")).Append(",");
                sqlCmd.Append(GetAcct("T_Receivables_Acct")).Append(",");
                sqlCmd.Append(GetAcct("T_Due_Acct")).Append(",");
                sqlCmd.Append(GetAcct("T_Credit_Acct")).Append(", ");
                //  B_INTRANSIT_Acct,B_Asset_Acct,B_Expense_Acct,B_INTERESTREV_Acct,B_INTERESTEXP_Acct,
                sqlCmd.Append(GetAcct("B_InTransit_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_Asset_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_Expense_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_InterestREV_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_InterestEXP_Acct")).Append(",");
                //  B_UNIDENTIFIED_Acct,B_SETTLEMENTGAIN_Acct,B_SETTLEMENTLOSS_Acct,
                sqlCmd.Append(GetAcct("B_Unidentified_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_SettlementGain_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_SettlementLoss_Acct")).Append(",");
                //  B_RevaluationGain_Acct,B_RevaluationLoss_Acct,B_PAYMENTSELECT_Acct,B_UnallocatedCash_Acct,
                sqlCmd.Append(GetAcct("B_RevaluationGain_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_RevaluationLoss_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_PaymentSelect_Acct")).Append(",");
                sqlCmd.Append(GetAcct("B_UnallocatedCash_Acct")).Append(", ");
                //  CH_Expense_Acct,CH_Revenue_Acct,
                sqlCmd.Append(GetAcct("Ch_Expense_Acct")).Append(",");
                sqlCmd.Append(GetAcct("Ch_Revenue_Acct")).Append(", ");
                //  UnEarnedRevenue_Acct,NotInvoicedReceivables_Acct,NotInvoicedRevenue_Acct,NotInvoicedReceipts_Acct,
                sqlCmd.Append(GetAcct("UnEarnedRevenue_Acct")).Append(",");
                sqlCmd.Append(GetAcct("NotInvoicedReceivables_Acct")).Append(",");
                sqlCmd.Append(GetAcct("NotInvoicedRevenue_Acct")).Append(",");
                sqlCmd.Append(GetAcct("NotInvoicedReceipts_Acct")).Append(", ");
                //  CB_Asset_Acct,CB_CashTransfer_Acct,CB_Differences_Acct,CB_Expense_Acct,CB_Receipt_Acct)
                sqlCmd.Append(GetAcct("CB_Asset_Acct")).Append(",");
                sqlCmd.Append(GetAcct("CB_CashTransfer_Acct")).Append(",");
                sqlCmd.Append(GetAcct("CB_Differences_Acct")).Append(",");
                sqlCmd.Append(GetAcct("CB_Expense_Acct")).Append(",");
                sqlCmd.Append(GetAcct("CB_Receipt_Acct")).Append(",");

                //Manufacturing
                sqlCmd.Append(GetAcct("WO_MATERIAL_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_MATERIALOVERHD_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_RESOURCE_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WC_OVERHEAD_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("P_MATERIALOVERHD_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_MATERIALVARIANCE_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_MATERIALOVERHDVARIANCE_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_RESOURCEVARIANCE_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_OVERHDVARIANCE_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("WO_SCRAP_ACCT")).Append(",");
                sqlCmd.Append(GetAcct("P_Resource_Absorption_Acct")).Append(",");
                sqlCmd.Append(GetAcct("Overhead_Absorption_Acct")).Append(")");

                //FixAsset
                //sqlCmd.Append(GetAcct("ASSET_DEPRECIATION_ACCT")).Append(",");
                //sqlCmd.Append(GetAcct("ASSET_DISP_REVENUE_ACCT")).Append(")");

                if (m_accountsOK)
                    no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                else
                    no = -1;
                if (no != 1)
                {
                    String err = "Default Accounts Not inserted";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
            }
            //  GL Categories
            tableName = "VAGL_Group";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                CreateGLCategory("Standard", MGLCategory.CATEGORYTYPE_Manual, true);
                int GL_None = CreateGLCategory("None", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_GL = CreateGLCategory("Manual", MGLCategory.CATEGORYTYPE_Manual, false);
                int GL_ARI = CreateGLCategory("AR Invoice", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_ARR = CreateGLCategory("AR Receipt", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_MM = CreateGLCategory("Material Management", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_API = CreateGLCategory("AP Invoice", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_APP = CreateGLCategory("AP Payment", MGLCategory.CATEGORYTYPE_Document, false);
                int GL_CASH = CreateGLCategory("Cash/Payments", MGLCategory.CATEGORYTYPE_Document, false);

                //	Base DocumentTypes
                int ii = CreateDocType("GL Journal", Msg.GetElement(m_ctx, "VAGL_JRNL_ID"),
                    MDocBaseType.DOCBASETYPE_GLJOURNAL, null, 0, 0,
                    1000, GL_GL);
                if (ii == 0)
                {
                    String err = "Document Type not created";
                    //result = err;
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
                CreateDocType("GL Journal Batch", Msg.GetElement(m_ctx, "VAGL_BatchJRNL_ID"),
                    MDocBaseType.DOCBASETYPE_GLJOURNAL, null, 0, 0,
                    100, GL_GL);
                //	MDocBaseType.DOCBASETYPE_GLDocument
                //
                int DT_I = CreateDocType("AR Invoice", Msg.GetElement(m_ctx, "VAB_Invoice_ID", true),
                    MDocBaseType.DOCBASETYPE_ARINVOICE, null, 0, 0,
                    100000, GL_ARI);
                int DT_II = CreateDocType("AR Invoice Indirect", Msg.GetElement(m_ctx, "VAB_Invoice_ID", true),
                    MDocBaseType.DOCBASETYPE_ARINVOICE, null, 0, 0,
                    150000, GL_ARI);
                //int DT_IC = CreateDocType("AR Credit Memo", Msg.GetMsg(m_ctx, "CreditMemo"),
                //    MDocBaseType.DOCBASETYPE_ARCREDITMEMO, null, 0, 0,
                //    170000, GL_ARI);
                int DT_IC = CreateDocType("AR Credit Memo", Msg.GetMsg(m_ctx, "CreditMemo"),
                  MDocBaseType.DOCBASETYPE_ARCREDITMEMO, null, 0, 0,
                  170000, GL_ARI, true, false);
                //	MDocBaseType.DOCBASETYPE_ARProFormaInvoice

                CreateDocType("AP Invoice", Msg.GetElement(m_ctx, "VAB_Invoice_ID", false),
                    MDocBaseType.DOCBASETYPE_APINVOICE, null, 0, 0,
                    0, GL_API);
                //CreateDocType("AP CreditMemo", Msg.GetMsg(m_ctx, "CreditMemo"),
                //    MDocBaseType.DOCBASETYPE_APCREDITMEMO, null, 0, 0,
                //    0, GL_API);
                CreateDocType("AP CreditMemo", Msg.GetMsg(m_ctx, "CreditMemo"),
                    MDocBaseType.DOCBASETYPE_APCREDITMEMO, null, 0, 0,
                    0, GL_API, true, false);
                CreateDocType("Match Invoice", Msg.GetElement(m_ctx, "VAM_MatchInvoice_ID", false),
                    MDocBaseType.DOCBASETYPE_MATCHINVOICE, null, 0, 0,
                    390000, GL_API);

                CreateDocType("AR Receipt", Msg.GetElement(m_ctx, "VAB_Payment_ID", true),
                    MDocBaseType.DOCBASETYPE_ARRECEIPT, null, 0, 0,
                    0, GL_ARR);
                CreateDocType("AP Payment", Msg.GetElement(m_ctx, "VAB_Payment_ID", false),
                    MDocBaseType.DOCBASETYPE_APPAYMENT, null, 0, 0,
                    0, GL_APP);
                CreateDocType("Allocation", "Allocation",
                    MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, null, 0, 0,
                    490000, GL_CASH);

                int DT_S = CreateDocType("MM Shipment", "Delivery Note",
                    MDocBaseType.DOCBASETYPE_MATERIALDELIVERY, null, 0, 0,
                    500000, GL_MM);
                int DT_SI = CreateDocType("MM Shipment Indirect", "Delivery Note",
                    MDocBaseType.DOCBASETYPE_MATERIALDELIVERY, null, 0, 0,
                    550000, GL_MM);
                int DT_SR = CreateDocType("MM Customer Return", "Customer Return",
                    MDocBaseType.DOCBASETYPE_MATERIALDELIVERY, null, 0, 0,
                    590000, GL_MM, true, false);

                CreateDocType("MM Receipt", "Vendor Delivery",
                    MDocBaseType.DOCBASETYPE_MATERIALRECEIPT, null, 0, 0,
                    0, GL_MM);
                CreateDocType("MM Vendor Return", "Vendor Return",
                    MDocBaseType.DOCBASETYPE_MATERIALRECEIPT, null, 0, 0,
                    0, GL_MM, true, false);

                CreateDocType("Purchase Order", Msg.GetElement(m_ctx, "VAB_Order_ID", false),
                    MDocBaseType.DOCBASETYPE_PURCHASEORDER, null, 0, 0,
                    800000, GL_None);
                CreateDocType("Vendor RMA", "Vendor RMA",
                    MDocBaseType.DOCBASETYPE_PURCHASEORDER, null, 0, 0,
                    890000, GL_None, true, false);
                CreateDocType("Match PO", Msg.GetElement(m_ctx, "VAM_MatchPO_ID", false),
                    MDocBaseType.DOCBASETYPE_MATCHPO, null, 0, 0,
                    880000, GL_None);
                CreateDocType("Purchase Requisition", Msg.GetElement(m_ctx, "VAM_Requisition_ID", false),
                    MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION, null, 0, 0,
                    900000, GL_None);

                CreateDocType("Bank Statement", Msg.GetElement(m_ctx, "VAB_BankStatemet_ID", true),
                    MDocBaseType.DOCBASETYPE_BANKSTATEMENT, null, 0, 0,
                    700000, GL_CASH);
                CreateDocType("Cash Journal", Msg.GetElement(m_ctx, "VAB_CashJRNL_ID", true),
                    MDocBaseType.DOCBASETYPE_CASHJOURNAL, null, 0, 0,
                    750000, GL_CASH);

                CreateDocType("Material Movement", Msg.GetElement(m_ctx, "VAM_InventoryTransfer_ID", false),
                    MDocBaseType.DOCBASETYPE_MATERIALMOVEMENT, null, 0, 0,
                    610000, GL_MM);
                CreateDocType("Physical Inventory", Msg.GetElement(m_ctx, "VAM_Inventory_ID", false),
                    MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY, null, 0, 0,
                    620000, GL_MM);
                CreateDocType("Material Production", Msg.GetElement(m_ctx, "VAM_Production_ID", false),
                    MDocBaseType.DOCBASETYPE_MATERIALPRODUCTION, null, 0, 0,
                    630000, GL_MM);
                CreateDocType("Project Issue", Msg.GetElement(m_ctx, "VAB_ProjectSupply_ID", false),
                    MDocBaseType.DOCBASETYPE_PROJECTISSUE, null, 0, 0,
                    640000, GL_MM);

                //  Order Entry
                CreateDocType("Binding offer", "Quotation",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_Quotation, 0, 0,
                    10000, GL_None);
                CreateDocType("Non binding offer", "Proposal",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_Proposal, 0, 0,
                    20000, GL_None);
                CreateDocType("Prepay Order", "Prepay Order",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_PrepayOrder, DT_S, DT_I,
                    30000, GL_None);
                CreateDocType("Standard Order", "Order Confirmation",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_StandardOrder, DT_S, DT_I,
                    50000, GL_None);
                CreateDocType("Customer RMA", "Customer RMA",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_StandardOrder, DT_SR, DT_IC,
                    59000, GL_None, true, false);
                CreateDocType("Credit Order", "Order Confirmation",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_OnCreditOrder, DT_SI, DT_I,
                    60000, GL_None);   //  RE
                CreateDocType("Warehouse Order", "Order Confirmation",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_WarehouseOrder, DT_S, DT_I,
                    70000, GL_None);    //  LS
                int DT = CreateDocType("POS Order", "Order Confirmation",
                    MDocBaseType.DOCBASETYPE_SALESORDER, MDocType.DOCSUBTYPESO_POSOrder, DT_SI, DT_II,
                    80000, GL_None);    // Bar
                //	POS As Default for window SO
                //CreatePreference("VAB_DocTypesTarGet_ID", DT.ToString(), 143);
                CreatePreference("VAB_DocTypesTarget_ID", DT.ToString(), 143);//13feb2013 lakhwinder

                //  Update ClientInfo
                sqlCmd = new StringBuilder("UPDATE VAF_ClientDetail SET ");
                sqlCmd.Append("VAB_AccountBook1_ID=").Append(m_as.GetVAB_AccountBook_ID())
                    .Append(", VAB_Calender_ID=").Append(m_calendar.GetVAB_Calender_ID())
                    .Append(" WHERE VAF_Client_ID=").Append(m_client.GetVAF_Client_ID());
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                {
                    String err = "ClientInfo not updated";
                    //result = err;
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
            }
            //	Validate Completeness
            DocumentTypeVerify.CreateDocumentTypes(m_ctx, GetVAF_Client_ID(), null, m_trx);
            DocumentTypeVerify.CreatePeriodControls(m_ctx, GetVAF_Client_ID(), null, m_trx);
            //
            log.Info("fini");
            //result = "";
            return true;
        }   //  createAccounting



        /// <summary>
        /// Get Account ID for key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>VAB_Acct_ValidParameter_ID</returns>
        private int GetAcct(String key)
        {
            log.Fine(key);
            //  Element
            int VAB_Acct_Element_ID = m_nap.GetVAB_Acct_Element_ID(key.ToUpper());
            if (VAB_Acct_Element_ID == 0)
            {
                log.Severe("Account not defined: " + key);
                m_accountsOK = false;
                return 0;
            }

            MVABAccount vc = MVABAccount.GetDefault(m_as, true);	//	optional null
            vc.SetVAF_Org_ID(0);		//	will be overwritten
            vc.SetAccount_ID(VAB_Acct_Element_ID);
            if (!vc.Save())
            {
                log.Severe("Not Saved - Key=" + key + ", VAB_Acct_Element_ID=" + VAB_Acct_Element_ID);
                m_accountsOK = false;
                return 0;
            }
            int VAB_Acct_ValidParameter_ID = vc.GetVAB_Acct_ValidParameter_ID();
            if (VAB_Acct_ValidParameter_ID == 0)
            {
                log.Severe("No account - Key=" + key + ", VAB_Acct_Element_ID=" + VAB_Acct_Element_ID);
                m_accountsOK = false;
            }
            return VAB_Acct_ValidParameter_ID;
        }   //  GetAcct


        /// <summary>
        /// Create GL Category
        /// </summary>
        /// <param name="Name">name</param>
        /// <param name="CategoryType">category type MGLCategory.CATEGORYTYPE_*</param>
        /// <param name="isDefault">is default value</param>
        /// <returns>VAGL_Group_ID</returns>
        private int CreateGLCategory(String Name, String CategoryType, bool isDefault)
        {
            MGLCategory cat = new MGLCategory(m_ctx, 0, m_trx);
            cat.SetName(Name);
            cat.SetCategoryType(CategoryType);
            cat.SetIsDefault(isDefault);
            if (!cat.Save())
            {
                log.Log(Level.SEVERE, "GL Category NOT created - " + Name);
                return 0;
            }
            //
            return cat.GetVAGL_Group_ID();
        }   //  createGLCategory

        /// <summary>
        /// Create Document Types with Sequence
        /// </summary>
        /// <param name="Name">name</param>
        /// <param name="PrintName">print name</param>
        /// <param name="DocBaseType">document base type</param>
        /// <param name="DocSubTypeSO">sales order sub type</param>
        /// <param name="VAB_DocTypesShipment_ID">shippent doc</param>
        /// <param name="VAB_DocTypesInvoice_ID">invoice doc</param>
        /// <param name="StartNo">doc no</param>
        /// <param name="VAGL_Group_ID">gl category</param>
        /// <returns>doc type or 0 for error</returns>
        private int CreateDocType(String Name, String PrintName, String DocBaseType, String DocSubTypeSO, int VAB_DocTypesShipment_ID, int VAB_DocTypesInvoice_ID, int StartNo, int VAGL_Group_ID)
        {
            return CreateDocType(Name, PrintName, DocBaseType, DocSubTypeSO,
                    VAB_DocTypesShipment_ID, VAB_DocTypesInvoice_ID,
                    StartNo, VAGL_Group_ID, false, true);
        }	//	CreateDocType

        /// <summary>
        /// Create Document Types with Sequence
        /// </summary>
        /// <param name="Name">name</param>
        /// <param name="PrintName">print name</param>
        /// <param name="DocBaseType">document base type</param>
        /// <param name="DocSubTypeSO">sales order sub type</param>
        /// <param name="VAB_DocTypesShipment_ID">shippent doc</param>
        /// <param name="VAB_DocTypesInvoice_ID">invoice doc</param>
        /// <param name="StartNo">doc no</param>
        /// <param name="VAGL_Group_ID">gl category</param>
        /// <param name="IsCreateCounter"></param>
        /// <param name="isReturnTrx">optinal trx</param>
        /// <returns>doc type or 0 for error</returns>
        private int CreateDocType(String Name, String PrintName, String DocBaseType, String DocSubTypeSO, int VAB_DocTypesShipment_ID, int VAB_DocTypesInvoice_ID, int StartNo, int VAGL_Group_ID, bool isReturnTrx, bool IsCreateCounter)
        {
            MVAFRecordSeq sequence = null;
            if (StartNo != 0)
            {
                sequence = new MVAFRecordSeq(m_ctx, GetVAF_Client_ID(), Name, StartNo, m_trx);
                if (!sequence.Save())
                {
                    log.Log(Level.SEVERE, "Sequence NOT created - " + Name);
                    return 0;
                }
            }

            MDocType dt = new MDocType(m_ctx, DocBaseType, Name, m_trx);
            if (PrintName != null && PrintName.Length > 0)
                dt.SetPrintName(PrintName);	//	Defaults to Name
            if (DocSubTypeSO != null)
                dt.SetDocSubTypeSO(DocSubTypeSO);
            if (VAB_DocTypesShipment_ID != 0)
                dt.SetVAB_DocTypesShipment_ID(VAB_DocTypesShipment_ID);
            if (VAB_DocTypesInvoice_ID != 0)
                dt.SetVAB_DocTypesInvoice_ID(VAB_DocTypesInvoice_ID);
            if (VAGL_Group_ID != 0)
                dt.SetVAGL_Group_ID(VAGL_Group_ID);
            if (sequence == null)
                dt.SetIsDocNoControlled(false);
            else
            {
                dt.SetIsDocNoControlled(true);
                dt.SetDocNoSequence_ID(sequence.GetVAF_Record_Seq_ID());
            }
            dt.SetIsSOTrx();
            dt.SetIsReturnTrx(isReturnTrx);
            dt.SetIsCreateCounter(IsCreateCounter);
            if (!dt.Save())
            {
                log.Log(Level.SEVERE, "DocType NOT created - " + Name);
                return 0;
            }
            //
            return dt.GetVAB_DocTypes_ID();
        }   //  CreateDocType


        public bool CreateEntities(int VAB_Country_ID, String City, int VAB_RegionState_ID, int VAB_Currency_ID)
        {
            if (m_as == null && ISTENATRUNNINGFORERP)
            {
                log.Severe("No AcctountingSChema");
                m_trx.Rollback();
                m_trx.Close();
                return false;
            }
            log.Info("VAB_Country_ID=" + VAB_Country_ID
                + ", City=" + City + ", VAB_RegionState_ID=" + VAB_RegionState_ID);
            m_info.Append("\n----\n");
            //
            String defaultName = Msg.Translate(m_lang, "Standard");
            String defaultEntry = "'" + defaultName + "',";
            StringBuilder sqlCmd = null;
            int no = 0;

            //	Create Marketing Channel/Campaign
            tableName = "VAB_MarketingChannel";
            if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            {
                int VAB_MarketingChannel_ID = GetNextID(GetVAF_Client_ID(), "VAB_MarketingChannel");
                sqlCmd = new StringBuilder("INSERT INTO VAB_MarketingChannel ");
                sqlCmd.Append("(VAB_MarketingChannel_ID,Name,");
                sqlCmd.Append(m_stdColumns).Append(") VALUES (");
                sqlCmd.Append(VAB_MarketingChannel_ID).Append(",").Append(defaultEntry);
                sqlCmd.Append(m_stdValues).Append(")");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "Channel NOT inserted");
                int VAB_Promotion_ID = GetNextID(GetVAF_Client_ID(), "VAB_Promotion");
                sqlCmd = new StringBuilder("INSERT INTO VAB_Promotion ");
                sqlCmd.Append("(VAB_Promotion_ID,VAB_MarketingChannel_ID,").Append(m_stdColumns).Append(",");
                sqlCmd.Append(" Value,Name,Costs) VALUES (");
                sqlCmd.Append(VAB_Promotion_ID).Append(",").Append(VAB_MarketingChannel_ID).Append(",").Append(m_stdValues).Append(",");
                sqlCmd.Append(defaultEntry).Append(defaultEntry).Append("0)");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no == 1)
                    m_info.Append(Msg.Translate(m_lang, "VAB_Promotion_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "Campaign NOT inserted");

                if (m_hasMCampaign)
                {
                    //  Default
                    if (lstTableName.Contains("VAB_AccountBook_Element"))
                    {
                        sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET ");
                        sqlCmd.Append("VAB_Promotion_ID=").Append(VAB_Promotion_ID);

                        sqlCmd.Append(" WHERE VAB_AccountBook_ID=").Append(m_as.GetVAB_AccountBook_ID());
                        sqlCmd.Append(" AND ElementType='MC'");
                        no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                        if (no != 1)
                            log.Log(Level.SEVERE, "AcctSchema ELement Campaign NOT updated");
                    }
                }

                //	Create Sales Region
                int VAB_SalesRegionState_ID = 0;
                if (lstTableName.Contains("VAB_SalesRegionState"))
                {
                    VAB_SalesRegionState_ID = GetNextID(GetVAF_Client_ID(), "VAB_SalesRegionState");
                    sqlCmd = new StringBuilder("INSERT INTO VAB_SalesRegionState ");
                    sqlCmd.Append("(VAB_SalesRegionState_ID,").Append(m_stdColumns).Append(",");
                    sqlCmd.Append(" Value,Name,IsSummary) VALUES (");
                    sqlCmd.Append(VAB_SalesRegionState_ID).Append(",").Append(m_stdValues).Append(", ");
                    sqlCmd.Append(defaultEntry).Append(defaultEntry).Append("'N')");
                    no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                    if (no == 1)
                        m_info.Append(Msg.Translate(m_lang, "VAB_SalesRegionState_ID")).Append("=").Append(defaultName).Append("\n");
                    else
                        log.Log(Level.SEVERE, "SalesRegion NOT inserted");
                }

                if (m_hasSRegion)
                {
                    //  Default
                    if (lstTableName.Contains("VAB_SalesRegionState") && VAB_SalesRegionState_ID > 0)
                    {
                        sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET ");
                        sqlCmd.Append("VAB_SalesRegionState_ID=").Append(VAB_SalesRegionState_ID);
                        sqlCmd.Append(" WHERE VAB_AccountBook_ID=").Append(m_as.GetVAB_AccountBook_ID());
                        sqlCmd.Append(" AND ElementType='SR'");
                        no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                        if (no != 1)
                            log.Log(Level.SEVERE, "AcctSchema ELement SalesRegion NOT updated");
                    }
                }
            }

            /**
             *  Business Partner
             */
            //  Create BP Group
            MVABBusinessPartner bp = null;
            MVABBPartCategory bpg = null;
            if (lstTableName.Contains("VAB_BPart_Category"))// Update by Paramjeet Singh
            {
                bpg = new MVABBPartCategory(m_ctx, 0, m_trx);



                bpg.SetValue(defaultName);
                bpg.SetName(defaultName);
                bpg.SetIsDefault(true);
                if (bpg.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAB_BPart_Category_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "BP Group NOT inserted");

                //	Create BPartner
                bp = new MVABBusinessPartner(m_ctx, 0, m_trx);
                bp.SetValue(defaultName);
                bp.SetName(defaultName);
                bp.SetBPGroup(bpg);
                if (bp.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAB_BusinessPartner_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "BPartner NOT inserted");
            }
            //  Location for Standard BP
            MVABAddress bpLoc = new MVABAddress(m_ctx, VAB_Country_ID, VAB_RegionState_ID, City, m_trx);
            bpLoc.Save();
            MProduct product = null;
            if (lstTableName.Contains("VAM_Product") && bp != null) // Update by Paramjeet Singh
            {


                MVABBPartLocation bpl = new MVABBPartLocation(bp);
                bpl.SetVAB_Address_ID(bpLoc.GetVAB_Address_ID());
                if (!bpl.Save())
                    log.Log(Level.SEVERE, "BP_Location (Standard) NOT inserted");
                //  Default

                sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET ");
                sqlCmd.Append("VAB_BusinessPartner_ID=").Append(bp.GetVAB_BusinessPartner_ID());
                sqlCmd.Append(" WHERE VAB_AccountBook_ID=").Append(m_as.GetVAB_AccountBook_ID());
                sqlCmd.Append(" AND ElementType='BP'");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "AcctSchema Element BPartner NOT updated");

                CreatePreference("VAB_BusinessPartner_ID", bp.GetVAB_BusinessPartner_ID().ToString(), 143);

                /**
                 *  Product
                 */
                //  Create Product Category
                MProductCategory pc = new MProductCategory(m_ctx, 0, m_trx);
                pc.SetValue(defaultName);
                pc.SetName(defaultName);
                pc.SetIsDefault(true);
                if (pc.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAM_ProductCategory_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "Product Category NOT inserted");

                //  UOM (EA)
                int VAB_UOM_ID = 100;

                //  TaxCategory
                int VAB_TaxCategory_ID = GetNextID(GetVAF_Client_ID(), "VAB_TaxCategory");
                sqlCmd = new StringBuilder("INSERT INTO VAB_TaxCategory ");
                sqlCmd.Append("(VAB_TaxCategory_ID,").Append(m_stdColumns).Append(",");
                sqlCmd.Append(" Name,IsDefault) VALUES (");
                sqlCmd.Append(VAB_TaxCategory_ID).Append(",").Append(m_stdValues).Append(", ");
                if (VAB_Country_ID == 100)    // US
                    sqlCmd.Append("'Sales Tax','Y')");
                else
                    sqlCmd.Append(defaultEntry).Append("'Y')");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "TaxCategory NOT inserted");

                //  Tax - Zero Rate
                MTax tax = new MTax(m_ctx, "Standard", Env.ZERO, VAB_TaxCategory_ID, m_trx);
                tax.SetIsDefault(true);
                if (tax.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAB_TaxRate_ID"))
                        .Append("=").Append(tax.GetName()).Append("\n");
                else
                    log.Log(Level.SEVERE, "Tax NOT inserted");

                //	Create Product
                product = new MProduct(m_ctx, 0, m_trx);
                product.SetValue(defaultName);
                product.SetName(defaultName);
                product.SetVAB_UOM_ID(VAB_UOM_ID);
                product.SetVAM_ProductCategory_ID(pc.GetVAM_ProductCategory_ID());
                product.SetVAB_TaxCategory_ID(VAB_TaxCategory_ID);
                if (product.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAM_Product_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "Product NOT inserted");
                //  Default

                sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET ");
                sqlCmd.Append("VAM_Product_ID=").Append(product.GetVAM_Product_ID());
                sqlCmd.Append(" WHERE VAB_AccountBook_ID=").Append(m_as.GetVAB_AccountBook_ID());
                sqlCmd.Append(" AND ElementType='PR'");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, null);
                if (no != 1)
                    log.Log(Level.SEVERE, "AcctSchema Element Product NOT updated");
            }

            /**
             *  Location, Warehouse, Locator
             */
            //  Location (Company)
            MVABAddress loc = new MVABAddress(m_ctx, VAB_Country_ID, VAB_RegionState_ID, City, m_trx);
            loc.Save();


            sqlCmd = new StringBuilder("UPDATE VAF_OrgDetail SET VAB_Address_ID=");
            sqlCmd.Append(loc.GetVAB_Address_ID()).Append(" WHERE VAF_Org_ID=").Append(GetVAF_Org_ID());
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
            if (no != 1)
                log.Log(Level.SEVERE, "Location NOT inserted");

            CreatePreference("VAB_Country_ID", VAB_Country_ID.ToString(), 0);

            //  Default Warehouse
            MWarehouse wh = new MWarehouse(m_ctx, 0, m_trx);
            wh.SetValue(defaultName);
            wh.SetName(defaultName);
            wh.SetVAB_Address_ID(loc.GetVAB_Address_ID());
            if (!wh.Save())
            {
                log.Log(Level.SEVERE, "Warehouse NOT inserted");
            }
            else
            {
                CoreLibrary.DataBase.DB.ExecuteQuery("UPDATE VAF_LoginSetting SET VAM_Warehouse_ID=" + wh.GetVAM_Warehouse_ID() + " WHERE VAF_Client_ID=" + m_client.GetVAF_Client_ID(), null, m_trx);
            }

            //   Locator
            if (lstTableName.Contains("VAM_Locator") && bp != null) // Update by Paramjeet Singh
            {
                MLocator locator = new MLocator(wh, defaultName);
                locator.SetIsDefault(true);
                if (!locator.Save())
                    log.Log(Level.SEVERE, "Locator NOT inserted");

            }
            //  Update ClientInfo
            //if (lstTableName.Contains(tableName)) // Update by Paramjeet Singh
            //{


            if (bp != null && product != null)
            {
                sqlCmd = new StringBuilder("UPDATE VAF_ClientDetail SET ");
                if (bp != null)
                {
                    sqlCmd.Append("VAB_BusinessPartnerCashTrx_ID=").Append(bp.GetVAB_BusinessPartner_ID());
                }

                if (product != null)
                {
                    sqlCmd.Append(",VAM_ProductFreight_ID=").Append(product.GetVAM_Product_ID());
                }
                //		sqlCmd.Append("VAB_UOM_Volume_ID=");
                //		sqlCmd.Append(",VAB_UOM_Weight_ID=");
                //		sqlCmd.Append(",VAB_UOM_Length_ID=");
                //		sqlCmd.Append(",VAB_UOM_Time_ID=");
                sqlCmd.Append(" WHERE VAF_Client_ID=").Append(GetVAF_Client_ID());
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                {
                    String err = "ClientInfo not updated";
                    log.Log(Level.SEVERE, err);
                    m_info.Append(err);
                    m_trx.Rollback();
                    m_trx.Close();
                    return false;
                }
            }

            /**
             *  Other
             */
            //  PriceList

            if (lstTableName.Contains("VAM_PriceList"))
            {
                MPriceList pl = new MPriceList(m_ctx, 0, m_trx);
                pl.SetName(defaultName);
                pl.SetVAB_Currency_ID(VAB_Currency_ID);
                pl.SetIsDefault(true);
                if (!pl.Save())
                    log.Log(Level.SEVERE, "PriceList NOT inserted");
                //  Price List
                MVAMDiscountCalculation ds = new MVAMDiscountCalculation(m_ctx, 0, m_trx);
                ds.SetName(defaultName);
                ds.SetDiscountType(MVAMDiscountCalculation.DISCOUNTTYPE_Pricelist);
                if (!ds.Save())
                    log.Log(Level.SEVERE, "DiscountSchema NOT inserted");
                //  PriceList Version
                MPriceListVersion plv = new MPriceListVersion(pl);
                plv.SetName();
                plv.SetVAM_DiscountCalculation_ID(ds.GetVAM_DiscountCalculation_ID());
                if (!plv.Save())
                    log.Log(Level.SEVERE, "PriceList_Version NOT inserted");
                //  ProductPrice
                MProductPrice pp = new MProductPrice(plv, product.GetVAM_Product_ID(),
                    Env.ONE, Env.ONE, Env.ONE);
                if (!pp.Save())
                    log.Log(Level.SEVERE, "ProductPrice NOT inserted");

            }

            ////	Create Sales Rep for Client-User
            //MBPartner bpCU = new MBPartner(m_ctx, 0, m_trx);
            //bpCU.SetValue(VAF_UserContact_U_Name);
            //bpCU.SetName(VAF_UserContact_U_Name);
            //bpCU.SetBPGroup(bpg);
            //bpCU.SetIsEmployee(true);
            //bpCU.SetIsSalesRep(true);
            //if (bpCU.Save())
            //    m_info.Append(Msg.Translate(m_lang, "SalesRep_ID")).Append("=").Append(VAF_UserContact_U_Name).Append("\n");
            //else
            //    log.Log(Level.SEVERE, "SalesRep (User) NOT inserted");
            ////  Location for Client-User

            //MLocation bpLocCU = new MLocation(m_ctx, VAB_Country_ID, VAB_RegionState_ID, City, m_trx);
            //bpLocCU.Save();
            //MBPartnerLocation bplCU = new MBPartnerLocation(bpCU);
            //bplCU.SetVAB_Address_ID(bpLocCU.GetVAB_Address_ID());
            //if (!bplCU.Save())
            //    log.Log(Level.SEVERE, "BP_Location (User) NOT inserted");


            ////  Update User
            //sqlCmd = new StringBuilder("UPDATE VAF_UserContact SET VAB_BusinessPartner_ID=");
            //sqlCmd.Append(bpCU.GetVAB_BusinessPartner_ID()).Append(" WHERE VAF_UserContact_ID=").Append(VAF_UserContact_U_ID);
            //no = BaseLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
            //if (no != 1)
            //    log.Log(Level.SEVERE, "User of SalesRep (User) NOT updated");


            //	Create Sales Rep for Client-Admin
            MVABBusinessPartner bpCA = null;
            if (lstTableName.Contains("VAB_BusinessPartner"))
            {
                bpCA = new MVABBusinessPartner(m_ctx, 0, m_trx);
                bpCA.SetValue(VAF_UserContact_Name);
                bpCA.SetName(VAF_UserContact_Name);
                bpCA.SetBPGroup(bpg);
                bpCA.SetIsEmployee(true);
                bpCA.SetIsSalesRep(true);
                if (bpCA.Save())
                    m_info.Append(Msg.Translate(m_lang, "SalesRep_ID")).Append("=").Append(VAF_UserContact_Name).Append("\n");
                else
                    log.Log(Level.SEVERE, "SalesRep (Admin) NOT inserted");

                if (lstTableName.Contains("VAB_BPart_Location"))
                {
                    //  Location for Client-Admin
                    MVABAddress bpLocCA = new MVABAddress(m_ctx, VAB_Country_ID, VAB_RegionState_ID, City, m_trx);
                    bpLocCA.Save();
                    MVABBPartLocation bplCA = new MVABBPartLocation(bpCA);
                    bplCA.SetVAB_Address_ID(bpLocCA.GetVAB_Address_ID());
                    if (!bplCA.Save())
                        log.Log(Level.SEVERE, "BP_Location (Admin) NOT inserted");
                }
            }

            //  Update User
            sqlCmd = new StringBuilder("UPDATE VAF_UserContact SET VAB_BusinessPartner_ID=");
            if (bpCA != null)
            {
                sqlCmd.Append(bpCA.GetVAB_BusinessPartner_ID());
            }
            sqlCmd.Append(" WHERE VAF_UserContact_ID=").Append(VAF_UserContact_ID);
            no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
            if (no != 1)
                log.Log(Level.SEVERE, "User of SalesRep (Admin) NOT updated");


            //  Payment Term
            if (lstTableName.Contains("VAB_PaymentTerm"))
            {
                int VAB_PaymentTerm_ID = GetNextID(GetVAF_Client_ID(), "VAB_PaymentTerm");
                sqlCmd = new StringBuilder("INSERT INTO VAB_PaymentTerm ");
                sqlCmd.Append("(VAB_PaymentTerm_ID,").Append(m_stdColumns).Append(",");
                sqlCmd.Append("Value,Name,NetDays,GraceDays,DiscountDays,Discount,DiscountDays2,Discount2,IsDefault) VALUES (");
                sqlCmd.Append(VAB_PaymentTerm_ID).Append(",").Append(m_stdValues).Append(",");
                sqlCmd.Append("'Immediate','Immediate',0,0,0,0,0,0,'Y')");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "PaymentTerm NOT inserted");
            }
            //  Project Cycle
            if (lstTableName.Contains("VAB_ProjectCycle"))
            {
                VAB_ProjectCycle_ID = GetNextID(GetVAF_Client_ID(), "VAB_ProjectCycle");
                sqlCmd = new StringBuilder("INSERT INTO VAB_ProjectCycle ");
                sqlCmd.Append("(VAB_ProjectCycle_ID,").Append(m_stdColumns).Append(",");
                sqlCmd.Append(" Name,VAB_Currency_ID) VALUES (");
                sqlCmd.Append(VAB_ProjectCycle_ID).Append(",").Append(m_stdValues).Append(", ");
                sqlCmd.Append(defaultEntry).Append(VAB_Currency_ID).Append(")");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no != 1)
                    log.Log(Level.SEVERE, "Cycle NOT inserted");
            }
            /**
             *  Organization level data	===========================================
             */

            //	Create Default Project
            if (lstTableName.Contains("VAB_Project"))
            {
                int VAB_Project_ID = GetNextID(GetVAF_Client_ID(), "VAB_Project");
                sqlCmd = new StringBuilder("INSERT INTO VAB_Project ");
                sqlCmd.Append("(VAB_Project_ID,").Append(m_stdColumns).Append(",");
                sqlCmd.Append(" Value,Name,VAB_Currency_ID,IsSummary) VALUES (");
                sqlCmd.Append(VAB_Project_ID).Append(",").Append(m_stdValuesOrg).Append(", ");
                sqlCmd.Append(defaultEntry).Append(defaultEntry).Append(VAB_Currency_ID).Append(",'N')");
                no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                if (no == 1)
                    m_info.Append(Msg.Translate(m_lang, "VAB_Project_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "Project NOT inserted");


                //  Default Project
                if (m_hasProject)
                {
                    sqlCmd = new StringBuilder("UPDATE VAB_AccountBook_Element SET ");
                    sqlCmd.Append("VAB_Project_ID=").Append(VAB_Project_ID);
                    sqlCmd.Append(" WHERE VAB_AccountBook_ID=").Append(m_as.GetVAB_AccountBook_ID());
                    sqlCmd.Append(" AND ElementType='PJ'");
                    no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
                    if (no != 1)
                        log.Log(Level.SEVERE, "AcctSchema ELement Project NOT updated");
                }
            }
            //  CashBook
            if (lstTableName.Contains("VAB_CashBook"))
            {
                MVABCashBook cb = new MVABCashBook(m_ctx, 0, m_trx);
                cb.SetName(defaultName);
                cb.SetVAB_Currency_ID(VAB_Currency_ID);
                if (cb.Save())
                    m_info.Append(Msg.Translate(m_lang, "VAB_CashBook_ID")).Append("=").Append(defaultName).Append("\n");
                else
                    log.Log(Level.SEVERE, "CashBook NOT inserted");
                //
            }
            // }
            m_trx.Commit();
            m_trx.Close();



            log.Info("fini");
            return true;
        }   //  createEntities


        /// <summary>
        /// Create Preference
        /// </summary>
        /// <param name="Attribute">attribute</param>
        /// <param name="Value">value</param>
        /// <param name="VAF_Screen_ID">window id</param>
        private void CreatePreference(String Attributes, String Value, int VAF_Screen_ID)
        {
            int VAF_ValuePreference_ID = GetNextID(GetVAF_Client_ID(), "VAF_ValuePreference");
            StringBuilder sqlCmd = new StringBuilder("INSERT INTO VAF_ValuePreference ");
            sqlCmd.Append("(VAF_ValuePreference_ID,").Append(m_stdColumns).Append(",");
            sqlCmd.Append("Attribute,Value,VAF_Screen_ID) VALUES (");
            sqlCmd.Append(VAF_ValuePreference_ID).Append(",").Append(m_stdValues).Append(",");
            sqlCmd.Append("'").Append(Attributes).Append("','").Append(Value).Append("',");
            if (VAF_Screen_ID == 0)
                sqlCmd.Append("NULL )");  //jz nullif
            else
                sqlCmd.Append(VAF_Screen_ID).Append(")");
            int no = CoreLibrary.DataBase.DB.ExecuteQuery(sqlCmd.ToString(), null, m_trx);
            if (no != 1)
                log.Log(Level.SEVERE, "Preference NOT inserted - " + Attributes);
        }   //  createPreference


        /// <summary>
        /// Get Next ID
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <returns>ID</returns>
        private int GetNextID(int VAF_Client_ID, String TableName)
        {
            //	TODO: Exception 
            return DataBase.DB.GetNextID(VAF_Client_ID, TableName, m_trx);
        }	//	GetNextID


        /// <summary>
        /// Get Client
        /// </summary>
        /// <returns>VAF_Client_ID</returns>
        public int GetVAF_Client_ID()
        {
            return m_client.GetVAF_Client_ID();
        }


        /// <summary>
        /// Get VAF_Org_ID
        /// </summary>
        /// <returns>VAF_Org_ID</returns>
        public int GetVAF_Org_ID()
        {
            return m_org.GetVAF_Org_ID();
        }


        /// <summary>
        /// Get VAF_UserContact_ID
        /// </summary>
        /// <returns>VAF_UserContact_ID</returns>
        public int GetVAF_UserContact_ID()
        {
            return VAF_UserContact_ID;
        }

        /// <summary>
        /// Get Info
        /// </summary>
        /// <returns>Info</returns>
        public String GetInfo()
        {
            return m_info.ToString();
        }

        int Order_PrintFormat_ID = 0;
        int Invoice_PrintFormat_ID = 0;
        int Shipment_PrintFormat_ID = 0;
        int Remittance_PrintFormat_ID = 0;
        int OrderLine_PrintFormat_ID = 0;
        int InvoiceLine_PrintFormat_ID = 0;
        int ShipmentLine_PrintFormat_ID = 0;
        int RemittanceLine_PrintFormat_ID = 0;
        public void SetupPrintForm(int VAF_Client_ID)
        {
            log.Config("VAF_Client_ID=" + VAF_Client_ID);
            //Ctx ctx = Env.GetCtx();
            VLogMgt.Enable(false);

            //    //Order Template
            //int Order_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 100, VAF_Client_ID).Get_ID();
            //int OrderLine_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 101, VAF_Client_ID).Get_ID();
            //UpdatePrintFormatHeader(Order_PrintFormat_ID, OrderLine_PrintFormat_ID);
            ////	Invoice
            //int Invoice_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 102, VAF_Client_ID).Get_ID();
            //int InvoiceLine_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 103, VAF_Client_ID).Get_ID();
            //UpdatePrintFormatHeader(Invoice_PrintFormat_ID, InvoiceLine_PrintFormat_ID);
            ////	Shipment
            //int Shipment_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 104, VAF_Client_ID).Get_ID();
            //int ShipmentLine_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 105, VAF_Client_ID).Get_ID();
            //UpdatePrintFormatHeader(Shipment_PrintFormat_ID, ShipmentLine_PrintFormat_ID);
            ////	Check
            //int Check_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 106, VAF_Client_ID).Get_ID();
            //int RemittanceLine_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 107, VAF_Client_ID).Get_ID();
            //UpdatePrintFormatHeader(Check_PrintFormat_ID, RemittanceLine_PrintFormat_ID);
            ////	Remittance
            //int Remittance_PrintFormat_ID = MPrintFormat.CopyToClient(ctx, 108, VAF_Client_ID).Get_ID();
            //UpdatePrintFormatHeader(Remittance_PrintFormat_ID, RemittanceLine_PrintFormat_ID);


            //CopyPrintFormat();
            UpdatePrintFormatHeader(Order_PrintFormat_ID, OrderLine_PrintFormat_ID);
            UpdatePrintFormatHeader(Invoice_PrintFormat_ID, InvoiceLine_PrintFormat_ID);
            UpdatePrintFormatHeader(Shipment_PrintFormat_ID, ShipmentLine_PrintFormat_ID);
            UpdatePrintFormatHeader(Remittance_PrintFormat_ID, RemittanceLine_PrintFormat_ID);
            //int Order_PrintFormat_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_PRINT_RPT_LAYOUT_ID FROM VAF_PRINT_RPT_LAYOUT WHERE REF_PRINTFORMAT='Order Print Format' AND VAF_CLIENT_ID=" + VAF_Client_ID));
            //int Invoice_PrintFormat_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_PRINT_RPT_LAYOUT_ID FROM VAF_PRINT_RPT_LAYOUT WHERE REF_PRINTFORMAT='Invoice Print Format' AND VAF_CLIENT_ID=" + VAF_Client_ID));
            //int Shipment_PrintFormat_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_PRINT_RPT_LAYOUT_ID FROM VAF_PRINT_RPT_LAYOUT WHERE REF_PRINTFORMAT='Shipment Print Format' AND VAF_CLIENT_ID=" + VAF_Client_ID));
            //int Remittance_PrintFormat_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_PRINT_RPT_LAYOUT_ID FROM VAF_PRINT_RPT_LAYOUT WHERE REF_PRINTFORMAT='Remittance Print Format' AND VAF_CLIENT_ID=" + VAF_Client_ID));
            //	TODO: MPrintForm	
            //	MPrintForm form = new MPrintForm(); 
            int VAF_Print_Rpt_Page_ID = DataBase.DB.GetNextID(VAF_Client_ID, "VAF_Print_Rpt_Page", null);
            String sql = "INSERT INTO VAF_Print_Rpt_Page(VAF_Client_ID,VAF_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,VAF_Print_Rpt_Page_ID,"
                + "Name,Order_PrintFormat_ID,Invoice_PrintFormat_ID,Remittance_PrintFormat_ID,Shipment_PrintFormat_ID)"
                //
                + " VALUES (" + VAF_Client_ID + ",0,'Y',SysDate,0,SysDate,0," + VAF_Print_Rpt_Page_ID + ","
                + "'" + Msg.Translate(m_ctx, "Standard") + "',"
                + Order_PrintFormat_ID + "," + Invoice_PrintFormat_ID + ","
                + Remittance_PrintFormat_ID + "," + Shipment_PrintFormat_ID + ")";
            int no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null);
            if (no != 1)
                log.Log(Level.SEVERE, "PrintForm NOT inserted");
            //
            VLogMgt.Enable(true);

        }	//	createDocuments



        private void UpdatePrintFormatHeader(int Header_ID, int Line_ID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE VAF_Print_Rpt_LItem SET VAF_Print_Rpt_LayoutChild_ID=")
                .Append(Line_ID)
                .Append(" WHERE VAF_Print_Rpt_LayoutChild_ID IS NOT NULL AND VAF_Print_Rpt_Layout_ID=")
                .Append(Header_ID);
            CoreLibrary.DataBase.DB.ExecuteQuery(sb.ToString(), null);
        }	//	updatePrintFormatHeader




        private string SetupDefaultLogin(Trx trx, int VAF_Client_ID, int VAF_Role_ID, int VAF_Org_ID, int VAF_UserContact_ID, int VAM_Warehouse_ID)
        {
            int VAF_LoginSetting_ID = MVAFRecordSeq.GetNextID(m_ctx.GetVAF_Client_ID(), "VAF_LoginSetting", trx);
            StringBuilder sql = new StringBuilder("");
            sql.Append("INSERT INTO VAF_LoginSetting (VAF_CLIENT_ID,VAF_LOGINSETTING_ID,VAF_ORG_ID,VAF_ROLE_ID,VAF_USERCONTACT_ID,CREATED,CREATEDBY,EXPORT_ID,VAM_Warehouse_ID,UPDATED,UPDATEDBY)");
            sql.Append(" VALUES (" + VAF_Client_ID + "," + VAF_LoginSetting_ID + "," + VAF_Org_ID + "," + VAF_Role_ID + "," + VAF_UserContact_ID + ",");
            sql.Append(GlobalVariable.TO_DATE(DateTime.Now, false) + "," + m_ctx.GetVAF_UserContact_ID() + ",NULL,");
            if (VAM_Warehouse_ID == 0)
                sql.Append("NULL");
            else
                sql.Append(VAM_Warehouse_ID);

            sql.Append("," + GlobalVariable.TO_DATE(DateTime.Now, false) + "," + m_ctx.GetVAF_UserContact_ID() + ")");
            int s = VAdvantage.DataBase.DB.ExecuteQuery(sql.ToString(), null, trx);
            if (s == -1)
            {
                log.Log(Level.SEVERE, "Error While Saving Login Settings.");
                //return "Error While Saving Login Settings.";
            }
            return "OK";
        }
    }
}
