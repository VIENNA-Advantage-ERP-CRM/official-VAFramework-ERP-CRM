/********************************************************
 * Module Name    : Access Module 1
 * Purpose        : Role model.
                    Includes AD_User runtime info for Personal Access 
 * Class Used     : context.cs,
 * Created By     : Harwinder 
 * Date           : --
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using VAdvantage.Classes;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Runtime.CompilerServices;
using VAModelAD.Model;

namespace VAdvantage.Model
{
    /// <summary>
    /// intend to store access priviliged
    /// </summary>
    public class MRole : X_AD_Role
    {

        #region declaration

        // Dictinary 
        private Dictionary<int, bool> _dcWindow_access = null;
        private Dictionary<int, bool> _dcForm_access = null;
        private Dictionary<int, bool> _dcProcess_access = null;
        private Dictionary<string, string> _dcTask_access = null;
        private Dictionary<string, string> _dcWorkflow_access = null;

        //	Default Role 
        private static MRole _defaultRole = null;

        //	Access SQL Read Write
        public const bool SQL_RW = true;
        //	Access SQL Read Only		*/
        public const bool SQL_RO = false;
        //	Access SQL Fully Qualified	
        public const bool SQL_FULLYQUALIFIED = true;
        //	Access SQL Not Fully Qualified	
        public const bool SQL_NOTQUALIFIED = false;

        //	The AD_User_ID of the SuperUser
        public const int SUPERUSER_USER_ID = 100;
        //	The AD_User_ID of the System Administrator	
        public const int SYSTEM_USER_ID = 0;

        //  User
        private int _AD_User_ID = -1;
        // private int _AD_Role_ID = 0;
        // Log						
        private static VLogger s_log = VLogger.GetVLogger(typeof(MRole).FullName);

        private static readonly object _lock = new object();

        #endregion

        /**	Cache						*/
        private static CCache<int, MRole> s_cache = new CCache<int, MRole>("AD_Role", 10);

        /// <summary>
        ///Get Default (Client) Role
        /// </summary>
        /// <returns></returns>
        public static MRole GetDefault(Ctx ctx)
        {
            if (_defaultRole == null)/// && Ini.isClient())
                return GetDefault(ctx, false);
            return _defaultRole;
        }

        /// <summary>
        /// Get Roles Of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>roles of client</returns>
        public static MRole[] GetOfClient(Ctx ctx)
        {
            String sql = "SELECT * FROM AD_Role WHERE AD_Client_ID=" + ctx.GetAD_Client_ID();
            List<MRole> list = new List<MRole>();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MRole(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MRole[] retValue = new MRole[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
     * 	Get Roles With where clause
     *	@param ctx context
     *	@param whereClause where clause
     *	@return roles of client
     */
        public static MRole[] GetOf(Ctx ctx, String whereClause)
        {
            String sql = "SELECT * FROM AD_Role";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;
            List<MRole> list = new List<MRole>();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MRole(ctx, dr, null));
                        dr = null;
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                ds = null;
            }

            MRole[] retValue = new MRole[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Over max Query
        /// </summary>
        /// <param name="noRecords">records</param>
        /// <returns>true if over max query</returns>
        public bool IsQueryMax(int noRecords)
        {
            int max = GetMaxQueryRecords();
            return max > 0 && noRecords > max;
        }

        /// <summary>
        /// 	Get/Set Default Role.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="reload">if true forces load</param>
        /// <returns></returns>
        //  [MethodImpl(MethodImplOptions.Synchronized)]
        public static MRole GetDefault(Ctx ctx, bool reload)
        {
            int AD_Role_ID = ctx.GetAD_Role_ID();
            int AD_User_ID = ctx.GetAD_User_ID();
            //if (!Ini.IsClient())	//	none for Server
            //    AD_User_ID = 0; //Form Preference

            MRole role = (MRole)s_cache.Get(AD_Role_ID);
            lock (_lock)
            {
                if (reload || role == null)
                {
                    role = Get(ctx, AD_Role_ID, AD_User_ID, true);
                    s_cache[AD_Role_ID] = role;
                }
                else if (role.GetAD_Role_ID() != AD_Role_ID
                || role.GetAD_User_ID() != AD_User_ID)
                {
                    role = Get(ctx, AD_Role_ID, AD_User_ID, true);
                    s_cache[AD_Role_ID] = role;
                }
            }
            return role;


            //if (reload || _defaultRole == null)
            //{
            //    _defaultRole = Get(ctx, AD_Role_ID, AD_User_ID, reload);
            //}
            //else if (_defaultRole.GetAD_Role_ID() != AD_Role_ID
            //|| _defaultRole.GetAD_User_ID() != AD_User_ID)
            //{
            //    _defaultRole = Get(ctx, AD_Role_ID, AD_User_ID, true);
            //}
            ////	Update Cache
            //if (reload && _defaultRole != null)
            //    Get(ctx, AD_Role_ID, AD_User_ID, true);
            //return _defaultRole;

        }

        /// <summary>
        /// Get Role for User
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Role_ID"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="reload"></param>
        /// <returns></returns>
        public static MRole Get(Ctx ctx, int AD_Role_ID, int AD_User_ID, bool reload)
        {
            s_log.Info("AD_Role_ID=" + AD_Role_ID + ", AD_User_ID=" + AD_User_ID + ", reload=" + reload);
            MRole role = new MRole(ctx, AD_Role_ID, null);
            if (AD_Role_ID == 0)
            {
                Trx trxName = null;
                role.Load(trxName);			//	special Handling
            }
            role.SetAD_User_ID(AD_User_ID);
            role.LoadAccess(reload);
            s_log.Info(role.ToString());
            return role;
        }	//	get

        /// <summary>
        /// Get Role For User
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Role_ID"></param>
        /// <returns></returns>
        public static MRole Get(Ctx ctx, int AD_Role_ID)
        {
            string key = AD_Role_ID.ToString();
            Trx trxName = null;
            MRole role = new MRole(ctx, AD_Role_ID, trxName);
            if (AD_Role_ID == 0)	//	System Role
                role.Load(trxName);	//	special Handling

            return role;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Role_ID"></param>
        /// <param name="trxName"></param>
        public MRole(Ctx ctx, int AD_Role_ID, Trx trxName)
            : base(ctx, AD_Role_ID, trxName)
        {
            if (AD_Role_ID == 0)
            {
                SetIsCanExport(true);
                SetIsCanReport(true);
                SetIsManual(false);
                SetIsPersonalAccess(false);
                SetIsPersonalLock(false);
                SetIsShowAcct(false);
                SetIsAccessAllOrgs(false);
                SetIsAdministrator(false);
                SetUserLevel(USERLEVEL_Organization);
                SetPreferenceType(PREFERENCETYPE_Organization);
                SetIsChangeLog(false);
                SetOverwritePriceLimit(false);
                SetOverrideReturnPolicy(false);
                SetIsUseUserOrgAccess(false);
                SetIsCanApproveOwnDoc(false);
                SetMaxQueryRecords(0);
                SetConfirmQueryRecords(0);
                SetDisplayClientOrg(DISPLAYCLIENTORG_AlwaysTenantOrganziation);
                SetWinUserDefLevel(WINUSERDEFLEVEL_UserOnly);
            }
        }	//	

        /// <summary>
        ///	 Get Logged in user
        ///	 AD_User_ID user requesting info
        /// </summary>
        /// <returns></returns>
        public int GetAD_User_ID()
        {
            return _AD_User_ID;
        }	//

        public MRole(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //nothing
        }

        /// <summary>
        /// Set Logged in user
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void SetAD_User_ID(int AD_User_ID)
        {
            _AD_User_ID = AD_User_ID;
        }	//	setAD_User_ID

        /// <summary>
        /// Check User has access To window against role
        /// </summary>
        /// <param name="AD_Window_ID"></param>
        /// <returns></returns>
        //public static bool? GetWindowAccess1(int AD_Window_ID)
        //{
        //    bool? blnReturn = null;
        //    if (_dcWindow_access == null)
        //    {
        //        _dcWindow_access = new Dictionary<string, string>();
        //        string strQry = "SELECT AD_Window_ID, IsReadWrite FROM AD_Window_Access WHERE AD_Role_ID=" + ctx.GetAD_Role_ID() + " AND IsActive='Y'";
        //        try
        //        {
        //            IDataReader dr = DataBase.DB.ExecuteReader(strQry);
        //            while (dr.Read())
        //            {
        //                _dcWindow_access.Add(dr[0].ToString(), dr[1].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log
        //            _dcWindow_access = null;
        //        }
        //    }
        //    if (_dcWindow_access.ContainsKey(AD_Window_ID.ToString()))
        //    {
        //        blnReturn = true;
        //    }

        //    return blnReturn;
        //    // bool? retValue = dtWindow.
        //}	//	getWindowAc

        /// <summary>
        /// Check Form Access against Role
        /// </summary>
        /// <param name="AD_Form_ID"></param>
        /// <returns></returns>
        //public static bool? GetFormAccess1(int AD_Form_ID)
        //{
        //    bool? blnReturn = null;
        //    if (_dcForm_access == null)
        //    {
        //        _dcForm_access = new Dictionary<string, string>();

        //        string strQry = "SELECT AD_Form_ID, IsReadWrite FROM AD_Form_Access "
        //         + "WHERE AD_Role_ID=" + ctx.GetAD_Role_ID() + " AND IsActive='Y'";
        //        try
        //        {
        //            IDataReader dr = DataBase.DB.ExecuteReader(strQry);
        //            while (dr.Read())
        //            {
        //                _dcForm_access.Add(dr[0].ToString(), dr[1].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log
        //            _dcForm_access = null;
        //        }
        //    }
        //    if (_dcForm_access.ContainsKey(AD_Form_ID.ToString()))
        //    {
        //        blnReturn = true;
        //    }
        //    return blnReturn;
        //}

        /// <summary>
        /// Check Process Access against role
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <returns></returns>
        //public static bool? GetProcessAccess1(int AD_Process_ID)
        //{
        //    bool? blnReturn = null;
        //    if (_dcProcess_access == null)
        //    {
        //        _dcProcess_access = new Dictionary<string, string>();
        //        string strQry = "SELECT AD_Process_ID, IsReadWrite FROM AD_Process_Access " +
        //                        " WHERE AD_Role_ID=" + ctx.GetAD_Role_ID() + " AND IsActive='Y'";
        //        try
        //        {
        //            IDataReader dr = DataBase.DB.ExecuteReader(strQry);
        //            while (dr.Read())
        //            {
        //                _dcProcess_access.Add(dr[0].ToString(), dr[1].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log
        //            _dcProcess_access = null;
        //        }
        //    }
        //    if (_dcProcess_access.ContainsKey(AD_Process_ID.ToString()))
        //    {
        //        blnReturn = true;
        //    }
        //    return blnReturn;
        //}

        /// <summary>
        /// Check WorkFlow Access Gainst role
        /// </summary>
        /// <param name="AD_Workflow_ID"></param>
        /// <returns></returns>
        //public static bool? GetWorkflowAccess1(int AD_Workflow_ID)
        //{
        //    bool? blnReturn = null;
        //    if (_dcWorkflow_access == null)
        //    {
        //        _dcWorkflow_access = new Dictionary<string, string>();

        //        string strQry = "SELECT AD_Workflow_ID, IsReadWrite FROM AD_Workflow_Access "
        //         + "WHERE AD_Role_ID=" + ctx.GetAD_Role_ID() + " AND IsActive='Y'";
        //        try
        //        {
        //            IDataReader dr = DataBase.DB.ExecuteReader(strQry);
        //            while (dr.Read())
        //            {
        //                _dcWorkflow_access.Add(dr[0].ToString(), dr[1].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log
        //            _dcWorkflow_access = null;
        //        }
        //    }
        //    if (_dcWorkflow_access.ContainsKey(AD_Workflow_ID.ToString()))
        //    {
        //        blnReturn = true;
        //    }
        //    return blnReturn;
        //}

        /// <summary>
        /// Check Task Access Against role
        /// </summary>
        /// <param name="AD_Task_ID"></param>
        /// <returns></returns>
        //public static bool? GetTaskAccess1(int AD_Task_ID)
        //{
        //    bool? blnReturn = null;
        //    if (_dcTask_access == null)
        //    {
        //        _dcTask_access = new Dictionary<string, string>();
        //        string strQry = "SELECT AD_Task_ID, IsReadWrite FROM AD_Task_Access "
        //         + "WHERE AD_Role_ID=" + ctx.GetAD_Role_ID() + " AND IsActive='Y'";
        //        try
        //        {
        //            IDataReader dr = DataBase.DB.ExecuteReader(strQry);
        //            while (dr.Read())
        //            {
        //                _dcTask_access.Add(dr[0].ToString(), dr[1].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log
        //            _dcTask_access = null;
        //        }
        //    }
        //    if (_dcTask_access.ContainsKey(AD_Task_ID.ToString()))
        //    {
        //        blnReturn = true;
        //    }
        //    return blnReturn;
        //}



        /// <summary>
        /// Check User has access To window against role
        /// </summary>
        /// <param name="AD_Window_ID"></param>
        /// <returns></returns>
        public bool? GetWindowAccess(int AD_Window_ID)
        {
            bool? blnReturn = null;
            if (_dcWindow_access == null)
            {
                _dcWindow_access = new Dictionary<int, bool>();
                string strQry = "SELECT AD_Window_ID, IsReadWrite FROM AD_Window_Access WHERE AD_Role_ID=" + GetCtx().GetAD_Role_ID() + " AND IsActive='Y'";
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                    while (dr.Read())
                    {
                        _dcWindow_access.Add(dr.GetInt32(0), dr[1].ToString() == "Y");
                    }
                    dr.Close();
                    dr = null;
                    log.Fine("#" + _dcWindow_access.Count);
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, strQry, e);
                    _dcWindow_access = null;
                }
            }

            if (_dcWindow_access.ContainsKey(AD_Window_ID))
            {
                blnReturn = _dcWindow_access[AD_Window_ID];
            }

            return blnReturn;
            // bool? retValue = dtWindow.
        }	//	getWindowAc

        public Dictionary<int, bool> GetWindowAccess()
        {
            return _dcWindow_access;
        }

        /// <summary>
        /// Check Form Access against Role
        /// </summary>
        /// <param name="AD_Form_ID"></param>
        /// <returns></returns>
        public bool? GetFormAccess(int AD_Form_ID)
        {
            bool? blnReturn = null;
            if (_dcForm_access == null)
            {
                _dcForm_access = new Dictionary<int, bool>();

                string strQry = "SELECT AD_Form_ID, IsReadWrite FROM AD_Form_Access "
                 + "WHERE AD_Role_ID=" + GetCtx().GetAD_Role_ID() + " AND IsActive='Y'";
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                    while (dr.Read())
                    {
                        _dcForm_access.Add(dr.GetInt32(0), dr[1].ToString() == "Y");
                    }
                    dr.Close();
                    dr = null;
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, strQry, e);
                    _dcForm_access = null;
                }
            }
            if (_dcForm_access.ContainsKey(AD_Form_ID))
            {
                blnReturn = _dcForm_access[AD_Form_ID];
            }
            return blnReturn;
        }

        public Dictionary<int, bool> GetFormAccess()
        {
            return _dcForm_access;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if it can be saved</returns>
        /// <author>raghu</author>
        protected override bool BeforeSave(bool newRecord)
        {
            //	if (newRecord || is_ValueChanged("UserLevel"))
            //	{
            if (GetAD_Client_ID() == 0)
            {
                SetUserLevel(USERLEVEL_System);
            }
            else if (GetUserLevel().Trim().Equals(USERLEVEL_System.Trim()))
            {
                log.SaveWarning("AccessTableNoUpdate", Msg.GetElement(GetCtx(), "UserLevel"));
                return false;
            }
            //	}

            // JID_1585: Set "Use User Org Access" check to false in case of "Access All Orgs" check is true.
            if (IsAccessAllOrgs())
            {
                SetIsUseUserOrgAccess(false);
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        /// <author>raghu</author>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord && success)
            {
                //	Add Role to SuperUser
                MUserRoles su = new MUserRoles(GetCtx(), SUPERUSER_USER_ID, GetAD_Role_ID(), Get_TrxName());
                su.Save();
                //Add Role to User
                if (GetCreatedBy() != SUPERUSER_USER_ID)
                {
                    MUserRoles ur = new MUserRoles(GetCtx(), GetCreatedBy(), GetAD_Role_ID(), Get_TrxName());
                    ur.Save();
                }
                UpdateAccessRecords();
            }
            //
            else if (Is_ValueChanged("UserLevel"))
            {
                UpdateAccessRecords();
            }
                // if user change setting of Check Document Action Access on Role window
            else if (Is_ValueChanged("CheckDocActionAccess"))
            {
                AddDocActionAccess();
            }

            //	Default Role changed
            if (_defaultRole != null
                && _defaultRole.Get_ID() == Get_ID())
            {
                _defaultRole = this;
            }
            if (success)
            {
                UpdateLoginSettings();
            }

            return success;
        }

        protected override bool AfterDelete(bool success)
        {
            DB.ExecuteQuery("DELETE FROM AD_Role_Group WHERE AD_Role_ID=" + GetAD_Role_ID());
            return success;
        }

        /// <summary>
        /// Create Access Records
        /// </summary>
        /// <returns>info</returns>
        public String UpdateAccessRecords()
        {
            if (IsManual())
            {
                return "-";
            }

            String roleClientOrgUser = GetAD_Role_ID() + ","
                + GetAD_Client_ID() + "," + GetAD_Org_ID() + ",'Y', SysDate,"
                + GetUpdatedBy() + ", SysDate," + GetUpdatedBy()
                + ",'Y' ";	//	IsReadWrite

            String sqlWindow = "INSERT INTO AD_Window_Access "
                + "(AD_Window_ID, AD_Role_ID,"
                + " AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,IsReadWrite) "
                + "SELECT DISTINCT w.AD_Window_ID, " + roleClientOrgUser
                + "FROM AD_Window w"
                + " INNER JOIN AD_Tab t ON (w.AD_Window_ID=t.AD_Window_ID)"
                + " INNER JOIN AD_Table tt ON (t.AD_Table_ID=tt.AD_Table_ID) "
                + "WHERE t.SeqNo=(SELECT MIN(SeqNo) FROM AD_Tab xt "	// only check first tab
                    + "WHERE xt.AD_Window_ID=w.AD_Window_ID)"
                + "AND tt.AccessLevel IN ";

            String sqlProcess = "INSERT INTO AD_Process_Access "
                + "(AD_Process_ID, AD_Role_ID,"
                + " AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,IsReadWrite) "
                + "SELECT DISTINCT p.AD_Process_ID, " + roleClientOrgUser
                + "FROM AD_Process p "
                + "WHERE AccessLevel IN ";

            String sqlForm = "INSERT INTO AD_Form_Access "
                + "(AD_Form_ID, AD_Role_ID,"
                + " AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,IsReadWrite) "
                + "SELECT f.AD_Form_ID, " + roleClientOrgUser
                + "FROM AD_Form f "
                + "WHERE AccessLevel IN ";

            String sqlWorkflow = "INSERT INTO AD_Workflow_Access "
                + "(AD_Workflow_ID, AD_Role_ID,"
                + " AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,IsReadWrite) "
                + "SELECT w.AD_Workflow_ID, " + roleClientOrgUser
                + "FROM AD_Workflow w "
                + "WHERE AccessLevel IN ";           


            /**
             *	Fill AD_xx_Access
             *	---------------------------------------------------------------------------
             *	SCO# Levels			S__ 100		4	System info
             *						SCO	111		7	System shared info
             *						SC_ 110		6	System/Client info
             *						_CO	011		3	Client shared info
             *						_C_	011		2	Client
             *						__O	001		1	Organization info
             *	Roles:
             *		S		4,7,6
             *		_CO		7,6,3,2,1
             *		__O		3,1,7
             */
            String roleAccessLevel = null;
            String roleAccessLevelWin = null;
            String UserLevel = GetUserLevel().Trim();
            if (USERLEVEL_System.Trim().Equals(UserLevel))
            {
                roleAccessLevel = "('4','7','6')";
            }
            else if (USERLEVEL_Client.Trim().Equals(UserLevel))
            {
                roleAccessLevel = "('7','6','3','2')";
            }
            else if (USERLEVEL_ClientPlusOrganization.Trim().Equals(UserLevel))
            {
                roleAccessLevel = "('7','6','3','2','1')";
            }
            else //	if (USERLEVEL_Organization.trim().equals(UserLevel))
            {
                roleAccessLevel = "('3','1','7')";
                roleAccessLevelWin = roleAccessLevel
                    + " AND w.Name NOT LIKE '%(all)%'";
            }
            if (roleAccessLevelWin == null)
            {
                roleAccessLevelWin = roleAccessLevel;
            }
            //
            String whereDel = " WHERE AD_Role_ID=" + GetAD_Role_ID();
            //
            int winDel = DataBase.DB.ExecuteQuery("DELETE FROM AD_Window_Access" + whereDel, null, Get_TrxName());
            int win = DataBase.DB.ExecuteQuery(sqlWindow + roleAccessLevelWin, null, Get_TrxName());
            int procDel = DataBase.DB.ExecuteQuery("DELETE FROM AD_Process_Access" + whereDel, null, Get_TrxName());
            int proc = DataBase.DB.ExecuteQuery(sqlProcess + roleAccessLevel, null, Get_TrxName());
            int formDel = DataBase.DB.ExecuteQuery("DELETE FROM AD_Form_Access" + whereDel, null, Get_TrxName());
            int form = DataBase.DB.ExecuteQuery(sqlForm + roleAccessLevel, null, Get_TrxName());
            int wfDel = DataBase.DB.ExecuteQuery("DELETE FROM AD_Workflow_Access" + whereDel, null, Get_TrxName());
            int wf = DataBase.DB.ExecuteQuery(sqlWorkflow + roleAccessLevel, null, Get_TrxName());

            // called function to add Document action access
            string daAccess = AddDocActionAccess();

            log.Fine("AD_Window_ID=" + winDel + "+" + win
                + ", AD_Process_ID=" + procDel + "+" + proc
                + ", AD_Form_ID=" + formDel + "+" + form
                + ", AD_Workflow_ID=" + wfDel + "+" + wf
                + daAccess);

            LoadAccess(true);
            return "@AD_Window_ID@ #" + win
                + " -  @AD_Process_ID@ #" + proc
                + " -  @AD_Form_ID@ #" + form
                + " -  @AD_Workflow_ID@ #" + wf
                + daAccess;
                
        }

        /// <summary>
        /// Add Access in Document Action Access Table if Manual checkbox is false 
        /// and Check Document Action Access checkbox is true on Role window
        /// </summary>
        /// <returns>String Message</returns>
        public string AddDocActionAccess()
        {
            if (IsManual())
                return "";

            string sqlcheck = "SELECT COUNT(AD_Role_ID) FROM AD_Document_Action_Access WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Role_ID = " + GetAD_Role_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sqlcheck));

            // Check applied if any record found on Document Action Access, then no need to insert all records again, user will do that manually
            if (count <= 0)
            {
                String sqlDocAction = "INSERT INTO AD_Document_Action_Access "
                    + "(AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,"
                    + "C_DocType_ID , AD_Ref_List_ID, AD_Role_ID) "
                    + "(SELECT "
                    + GetAD_Client_ID() + ",0,'Y', SysDate,"
                    + GetUpdatedBy() + ", SysDate," + GetUpdatedBy()
                    + ", doctype.C_DocType_ID, action.AD_Ref_List_ID, rol.AD_Role_ID "
                    + "FROM AD_Client client "
                    + "INNER JOIN C_DocType doctype ON (doctype.AD_Client_ID=client.AD_Client_ID) "
                    + "INNER JOIN AD_Ref_List action ON (action.AD_Reference_ID=135) "
                    + "INNER JOIN AD_Role rol ON (rol.AD_Client_ID=client.AD_Client_ID "
                    + "AND rol.AD_Role_ID=" + GetAD_Role_ID()
                    + ") LEFT JOIN AD_Document_Action_Access da ON "
                    + "(da.AD_Role_ID=" + GetAD_Role_ID()
                    + " AND da.C_DocType_ID=doctype.C_DocType_ID AND da.AD_Ref_List_ID=action.AD_Ref_List_ID) "
                    + "WHERE (da.C_DocType_ID IS NULL AND da.AD_Ref_List_ID IS NULL)) ";

                // change done here to assign Document Action access based on setting on Role window
                int daAccDel = 0;
                int daAcc = 0;
                if (IsCheckDocActionAccess())
                {
                    daAccDel = DataBase.DB.ExecuteQuery("DELETE FROM AD_Document_Action_Access WHERE AD_Role_ID=" + GetAD_Role_ID(), null, Get_TrxName());
                    daAcc = DataBase.DB.ExecuteQuery(sqlDocAction, null, Get_TrxName());
                }

                return " @DocumentAccess@ " + daAcc;
            }
            return "";
        }

        /// <summary>
        /// Check Process Access against role
         /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <returns></returns>
        public bool? GetProcessAccess(int AD_Process_ID)
        {
            bool? blnReturn = null;
            if (_dcProcess_access == null)
            {
                _dcProcess_access = new Dictionary<int, bool>();
                string strQry = "SELECT AD_Process_ID, IsReadWrite FROM AD_Process_Access " +
                                " WHERE AD_Role_ID=" + GetCtx().GetAD_Role_ID() + " AND IsActive='Y'";
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                    while (dr.Read())
                    {
                        _dcProcess_access.Add(dr.GetInt32(0), dr[1].ToString() == "Y");
                    }
                    dr.Close();
                    dr = null;
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, strQry, e);
                    _dcProcess_access = null;
                }
            }
            if (_dcProcess_access.ContainsKey(AD_Process_ID))
            {
                blnReturn = true;
            }
            return blnReturn;
        }

        /// <summary>
        /// Get RoleID from workflow
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <param name="Ad_Role_ID"></param>
        /// <returns>true If role exist</returns>
        public bool? GetProcessAccess(int AD_Process_ID, int Ad_Role_ID)
        {
            bool? blnReturn = null;

            _dcProcess_access = new Dictionary<int, bool>();
            string strQry = "SELECT AD_Process_ID, IsReadWrite FROM AD_Process_Access " +
                            " WHERE AD_Role_ID=" + Ad_Role_ID + " AND IsActive='Y'";
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                while (dr.Read())
                {
                    _dcProcess_access.Add(dr.GetInt32(0), dr[1].ToString() == "Y");
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, strQry, e);
                _dcProcess_access = null;
            }

            if (_dcProcess_access.ContainsKey(AD_Process_ID))
            {
                blnReturn = true;
            }
            return blnReturn;
        }

        public Dictionary<int, bool> GetProcessAccess()
        {
            return _dcProcess_access;
        }

        /// <summary>
        /// Check WorkFlow Access Gainst role
        /// </summary>
        /// <param name="AD_Workflow_ID"></param>
        /// <returns></returns>
        public bool? GetWorkflowAccess(int AD_Workflow_ID)
        {
            bool? blnReturn = null;
            if (_dcWorkflow_access == null)
            {
                _dcWorkflow_access = new Dictionary<string, string>();

                string strQry = "SELECT AD_Workflow_ID, IsReadWrite FROM AD_Workflow_Access "
                 + "WHERE AD_Role_ID=" + GetCtx().GetAD_Role_ID() + " AND IsActive='Y'";

                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                    while (dr.Read())
                    {
                        _dcWorkflow_access.Add(dr[0].ToString(), dr[1].ToString());
                    }
                    dr.Close();
                    dr = null;
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, strQry, e);
                    _dcWorkflow_access = null;
                }
            }
            if (_dcWorkflow_access.ContainsKey(AD_Workflow_ID.ToString()))
            {
                blnReturn = true;
            }
            return blnReturn;
        }

        /// <summary>
        /// Check Task Access Against role
        /// </summary>
        /// <param name="AD_Task_ID"></param>
        /// <returns></returns>
        public bool? GetTaskAccess(int AD_Task_ID)
        {
            bool? blnReturn = null;
            if (_dcTask_access == null)
            {
                _dcTask_access = new Dictionary<string, string>();
                string strQry = "SELECT AD_Task_ID, IsReadWrite FROM AD_Task_Access "
                 + "WHERE AD_Role_ID=" + GetCtx().GetAD_Role_ID() + " AND IsActive='Y'";
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(strQry, null, Get_TrxName());
                    while (dr.Read())
                    {
                        _dcTask_access.Add(dr[0].ToString(), dr[1].ToString());
                    }
                    dr.Close();
                    dr = null;
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, strQry, e);
                    _dcTask_access = null;
                }
            }
            if (_dcTask_access.ContainsKey(AD_Task_ID.ToString()))
            {
                blnReturn = true;
            }
            return blnReturn;
        }

        /// <summary>
        /// Show (Value) Preference Menu
        /// </summary>
        /// <returns>bool type true if preference type is not None</returns>
        public bool IsShowPreference()
        {
            return !X_AD_Role.PREFERENCETYPE_None.Equals(GetPreferenceType());
        }

        public bool IsShowVisualEditor()
        {
            return GetAD_Role_ID() == 0;

        }

        /// <summary>
        ///	Display Client
        /// </summary>
        /// <returns></returns>
        public bool IsDisplayClient()
        {
            string s = GetDisplayClientOrg();
            return s == null
                || DISPLAYCLIENTORG_AlwaysTenantOrganziation.Equals(s);
        }	//	isDisplayClient

        /// <summary>
        /// Display Org
        /// </summary>
        /// <returns>true if Org should be displayed</returns>
        public bool IsDisplayOrg()
        {
            String s = GetDisplayClientOrg();
            return s == null
                || DISPLAYCLIENTORG_AlwaysTenantOrganziation.Equals(s)
                || DISPLAYCLIENTORG_OnlyOrganization.Equals(s);
        }	//	isDisplayOrg



        /// <summary>
        ///	Get Display ClientOrg flag
        /// </summary>
        /// <returns></returns>
        public new string GetDisplayClientOrg()
        {
            string s = base.GetDisplayClientOrg();
            if (s == null)
                return DISPLAYCLIENTORG_AlwaysTenantOrganziation;
            return s;
        }	//	getDisplayC


        /***********************************************************************/

        // Access Management

        /***********************************************************************/

        #region "Declaration"
        // ListArray of Table Access
        private MTableAccess[] _tableAccess = null;

        //List-Array OF Column Access
        private MColumnAccess[] _columnAccess = null;

        // List-Array of Record Access
        private MRecordAccess[] _recordAccess = null;
        // List-Array of Dependent Record Access
        private MRecordAccess[] _recordDependentAccess = null;

        //	Table Data Access Level	
        private Dictionary<int, string> _tableAccessLevel = null;
        //	Table Name
        private Dictionary<string, int> _tableName = null;

        //	Positive List of Organizational Access
        private OrgAccess[] _orgAccess = null;
        // List of Table Access

        #endregion

        /// <summary>
        /// check user Table-Access to Table
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="isAccess"></param>
        /// <returns></returns>
        public bool IsTableAccess(int AD_Table_ID, bool isAccess)
        {
            if (!IsTableAccessLevel(AD_Table_ID, isAccess))
                return false;
            LoadTableAccess(false);
            bool hasAccess = true;

            for (int i = 0; i < _tableAccess.Length; i++)
            {
                if (!X_AD_Table_Access.ACCESSTYPERULE_Accessing.Equals(_tableAccess[i].GetAccessTypeRule()))
                {
                    continue;
                }
                if (_tableAccess[i].IsExclude())           //	Exclude
                //	If you Exclude Access to a table and select Read Only, 
                //	you can only read data (otherwise no access).
                {
                    if (_tableAccess[i].GetAD_Table_ID() == AD_Table_ID)
                    {
                        if (isAccess)
                        {
                            hasAccess = _tableAccess[i].IsReadOnly();
                        }
                        else
                        {
                            hasAccess = false;
                        }
                        log.Fine("Exclude AD_Table_ID=" + AD_Table_ID
                        + " (ro=" + isAccess + ",TableAccessRO=" + _tableAccess[i].IsReadOnly() + ") = " + hasAccess);
                        return hasAccess;
                    }
                }

                else //	Include
                //	If you Include Access to a table and select Read Only, 
                //	you can only read data (otherwise full access).
                {
                    hasAccess = false;
                    if (_tableAccess[i].GetAD_Table_ID() == AD_Table_ID)
                    {
                        if (!isAccess)	//	rw only if not r/o
                        {
                            hasAccess = !_tableAccess[i].IsReadOnly();
                        }
                        else
                        {
                            hasAccess = true;
                        }
                        log.Fine("Include AD_Table_ID=" + AD_Table_ID
                        + " (ro=" + isAccess + ",TableAccessRO=" + _tableAccess[i].IsReadOnly() + ") = " + hasAccess);
                        return hasAccess;
                    }
                }

            }//	for all Table Access
            if (!hasAccess)
            {
                log.Fine("AD_Table_ID=" + AD_Table_ID
                + "(ro=" + isAccess + ") = " + hasAccess);
                return hasAccess;
            }
            return hasAccess;
        }

        public bool IsTableReadOnly(int AD_Table_ID)
        {
            bool readOnly = false;
            for (int i = 0; i < _tableAccess.Length; i++)
            {
                if (!X_AD_Table_Access.ACCESSTYPERULE_Accessing.Equals(_tableAccess[i].GetAccessTypeRule()))
                {
                    continue;
                }

                if (_tableAccess[i].GetAD_Table_ID() == AD_Table_ID)
                {
                    readOnly = _tableAccess[i].IsReadOnly();
                }
                return readOnly;
            }
            return readOnly;
        }



        /// <summary>
        ///check user column-Access to Column
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="AD_Column_ID"></param>
        /// <param name="ro"></param>
        /// <returns></returns>
        public bool IsColumnAccess(int AD_Table_ID, int AD_Column_ID, bool ro)
        {
            if (!IsTableAccess(AD_Table_ID, ro))		//	No Access to Table		
                return false;
            LoadColumnAccess(false);

            bool retValue = true;		//	assuming exclusive
            for (int i = 0; i < _columnAccess.Length; i++)
            {
                if (_columnAccess[i].IsExclude())		//	Exclude
                //	If you Exclude Access to a column and select Read Only, 
                //	you can only read data (otherwise no access).
                {
                    if (_columnAccess[i].GetAD_Table_ID() == AD_Table_ID
                        && _columnAccess[i].GetAD_Column_ID() == AD_Column_ID)
                    {
                        if (ro)		//	just R/O Access requested
                            retValue = _columnAccess[i].IsReadOnly();
                        else
                            retValue = false;
                        if (!retValue)
                        {
                            log.Fine("Exclude AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID
                                + " (ro=" + ro + ",ColumnAccessRO=" + _columnAccess[i].IsReadOnly() + ") = " + retValue);
                        }
                        return retValue;
                    }
                }
                else								//	Include
                //	If you Include Access to a column and select Read Only, 
                //	you can only read data (otherwise full access).
                {
                    if (_columnAccess[i].GetAD_Table_ID() == AD_Table_ID)
                    {
                        retValue = false;
                        if (_columnAccess[i].GetAD_Column_ID() == AD_Column_ID)
                        {
                            if (!ro)	//	rw only if not r/o
                                retValue = !_columnAccess[i].IsReadOnly();
                            else
                                retValue = true;
                            if (!retValue)
                            {
                                log.Fine("Include AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID
                                    + " (ro=" + ro + ",ColumnAccessRO=" + _columnAccess[i].IsReadOnly() + ") = " + retValue);
                            }
                            return retValue;
                        }
                    }	//	same table
                }	//	include
            }	//	for all Table Access
            if (!retValue)
            {
                log.Fine("AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID
                    + " (ro=" + ro + ") = " + retValue);
            }
            return retValue;
        }	//	isColumnAccess

        /// <summary>
        /// Access to Table based on Role User Level Table Access Level
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="isAccess"></param>
        /// <returns></returns>
        public bool IsTableAccessLevel(int AD_Table_ID, bool isAccess)
        {
            if (isAccess)				//	role can always read
                return true;

            LoadTableInfo(false);

            //	AccessLevel
            //		1 = Org - 2 = Client - 4 = System
            //		3 = Org+Client - 6 = Client+System - 7 = All

            try
            {
                string strRoleAccessLevel = null;
                _tableAccessLevel.TryGetValue(AD_Table_ID, out strRoleAccessLevel);

                if (strRoleAccessLevel == null)
                {
                    // log no access tableid
                    log.Fine("NO - No AccessLevel - AD_Table_ID=" + AD_Table_ID);
                    return false;
                }
                //7
                if (strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_All))
                {
                    return true;
                }
                //	User Level = SCO
                string userLevel = GetUserLevel().Trim();

                //S,4,6
                if (userLevel.IndexOf('S') != -1 && (strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_SystemOnly)
                        || strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_SystemPlusClient)))
                {
                    return true;
                }//8**********************************************************8//
                if (userLevel.IndexOf('C') != -1 && (strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_ClientOnly)
                        || strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_SystemPlusClient)))
                {
                    return true;
                }
                if (userLevel.IndexOf('O') != -1 && (strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_Organization)
                       || strRoleAccessLevel.Equals(X_AD_Table.ACCESSLEVEL_ClientPlusOrganization)))
                {
                    return true;
                }
                log.Fine("NO - AD_Table_ID=" + AD_Table_ID
            + ", UserLevel=" + userLevel + ", AccessLevel=" + strRoleAccessLevel);
                return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        ///chck User has View Access 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="strAccesLevel"></param>
        /// <returns></returns>
        public bool CanView(Ctx ctx, string strAccesLevel)
        {
            string userLevel = GetUserLevel().Trim(); //Format 'SCO'

            bool retValue = true;
            if (X_AD_Table.ACCESSLEVEL_All.Equals(strAccesLevel))
            { retValue = true; }

            //	4 - System data 
            else if (X_AD_Table.ACCESSLEVEL_SystemOnly.Equals(strAccesLevel)
                        && userLevel.IndexOf('S') == -1)
            { retValue = false; }

            //	2 - Client data requires C
            else if (X_AD_Table.ACCESSLEVEL_ClientOnly.Equals(strAccesLevel)
                        && userLevel.IndexOf('C') == -1)
            { retValue = false; }

            //	1 - Organization data requires O
            else if (X_AD_Table.ACCESSLEVEL_Organization.Equals(strAccesLevel)
                         && userLevel.IndexOf('O') == -1)
            { retValue = false; }

            //	3 - Client Shared requires C or O
            else if (X_AD_Table.ACCESSLEVEL_ClientPlusOrganization.Equals(strAccesLevel)
                         && (!(userLevel.IndexOf('C') != -1 || userLevel.IndexOf('O') != -1)))
            { retValue = false; }

            //	6 - System/Client requires S or C
            else if (X_AD_Table.ACCESSLEVEL_SystemPlusClient.Equals(strAccesLevel)
                        && (!(userLevel.IndexOf('S') != -1 || userLevel.IndexOf('C') != -1)))
            { retValue = false; }

            if (retValue)
            {
                return retValue;
            }
            //  Notification
            log.SaveWarning("AccessTableNoView",
                "Required=" + strAccesLevel + "("
                + GetTableLevelString(Env.GetAD_Language(ctx), strAccesLevel)
                + ") != UserLevel=" + userLevel);
            log.Info(ToString());
            return retValue;
        }

        /// <summary>
        /// Returns clear text String of TableLevel
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <param name="TableLevel">level</param>
        /// <returns>info</returns>
        private String GetTableLevelString(String AD_Language, String TableLevel)
        {
            String level = TableLevel + "??";
            if (TableLevel.Equals("1"))
            {
                level = "AccessOrg";
            }
            else if (TableLevel.Equals("2"))
            {
                level = "AccessClient";
            }
            else if (TableLevel.Equals("3"))
            {
                level = "AccessClientOrg";
            }
            else if (TableLevel.Equals("4"))
            {
                level = "AccessSystem";
            }
            else if (TableLevel.Equals("6"))
            {
                level = "AccessSystemClient";
            }
            else if (TableLevel.Equals("7"))
            {
                level = "AccessShared";
            }
            return Msg.GetMsg(AD_Language, level);
        }

        /// <summary>
        /// Load Access Info
        /// </summary>
        /// <param name="reload"></param>
        public void LoadAccess(bool reload)
        {
            LoadOrgAccess(reload);
            LoadTableAccess(reload);
            LoadTableInfo(reload);
            LoadColumnAccess(reload);
            LoadRecordAccess(reload);
            if (reload)
            {
                _dcTask_access = null;
                GetTaskAccess(0);
                _dcWindow_access = null;
                GetWindowAccess(0);
                _dcWorkflow_access = null;
                GetWorkflowAccess(0);
                _dcForm_access = null;
                GetFormAccess(0);
                _dcProcess_access = null;
                GetProcessAccess(0);
            }
        }

        /// <summary>
        /// Load Table Access
        /// </summary>
        /// <param name="reload"></param>
        private void LoadTableAccess(bool reload)
        {
            if (_tableAccess != null && !reload)
            {
                return;
            }
            List<MTableAccess> list = new List<MTableAccess>();

            string sql = "SELECT * FROM AD_Table_Access WHERE " +
                         " AD_Role_ID = " + GetAD_Role_ID().ToString() + " AND IsActive='Y'";

            DataTable dt = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                foreach (DataRow rs in dt.Rows)
                {
                    list.Add(new MTableAccess(GetCtx(), rs, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dt = null;
            }

            _tableAccess = list.ToArray();
            log.Fine("#" + _tableAccess.Length);
            list = null;
        }

        public MTableAccess[] GetTableAccess()
        {
            return _tableAccess;
        }


        /// <summary>
        /// Load Table Access and Name
        /// </summary>
        /// <param name="reload"></param>
        private void LoadTableInfo(bool reload)
        {
            if (_tableAccessLevel != null && _tableName != null && !reload)
                return;
            _tableAccessLevel = new Dictionary<int, string>(300);
            _tableName = new Dictionary<string, int>(300);

            String sql = "SELECT AD_Table_ID, AccessLevel, TableName "
                + "FROM AD_Table WHERE IsActive='Y'";
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (dr.Read())
                {
                    int ii = Utility.Util.GetValueOfInt(dr[0]);
                    _tableAccessLevel.Add(ii, CommonFunctions.GetString(dr[1]));
                    _tableName.Add(CommonFunctions.GetString(dr[2]), ii);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);

            }
            log.Fine("#" + _tableAccessLevel.Count);
        }

        public Dictionary<int, string> GetTableAccessLevel()
        {
            return _tableAccessLevel;
        }
        public Dictionary<string, int> GetTableNames()
        {
            return _tableName;
        }

        /// <summary>
        ///	Load Column Access
        /// </summary>
        /// <param name="reload"></param>
        private void LoadColumnAccess(bool reload)
        {
            if (_columnAccess != null && !reload)
                return;
            List<MColumnAccess> list = new List<MColumnAccess>();

            string sql = "SELECT * FROM AD_Column_Access "
                + "WHERE AD_Role_ID=" + GetAD_Role_ID() + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                foreach (DataRow rs in dt.Rows)
                {
                    list.Add(new MColumnAccess(GetCtx(), rs, Get_TrxName()));
                }


            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dt = null;
            }

            _columnAccess = list.ToArray();
            log.Fine("#" + _columnAccess.Length);
        }	//	loadColumnAccess


        public MColumnAccess[] GetColumnAccess()
        {
            return _columnAccess;
        }

        /// <summary>
        /// Load Record Access
        /// </summary>
        /// <param name="reload"></param>
        private void LoadRecordAccess(bool reload)
        {
            if (!(reload || _recordAccess == null || _recordDependentAccess == null))
                return;
            List<MRecordAccess> list = new List<MRecordAccess>();
            List<MRecordAccess> dependent = new List<MRecordAccess>();

            string sql = "SELECT * FROM AD_Record_Access "
                + "WHERE AD_Role_ID=" + GetAD_Role_ID() + " AND IsActive='Y' ORDER BY AD_Table_ID";

            DataTable dt = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                foreach (DataRow rs in dt.Rows)
                {
                    MRecordAccess ra = new MRecordAccess(GetCtx(), rs, Get_TrxName());
                    list.Add(ra);
                    if (ra.IsDependentEntities())
                        dependent.Add(ra);
                }


            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (dr != null)
                {
                    dr.Close();
                }
            }
            _recordAccess = list.ToArray();
            _recordDependentAccess = dependent.ToArray();
            log.Fine("#" + _recordAccess.Length + " - Dependent #" + _recordDependentAccess.Length);
        }	//	loadRecordAccess


        public MRecordAccess[] GetRecordAccess()
        {
            return _recordAccess;
        }
        public MRecordAccess[] GetRecordDependentAccess()
        {
            return _recordDependentAccess;
        }

        /// <summary>
        /// Load Org Access
        /// </summary>
        /// <param name="reload"></param>
        private void LoadOrgAccess(bool reload)
        {
            if (!(reload || _orgAccess == null))
                return;
            //
            List<OrgAccess> list = new List<OrgAccess>();

            if (IsUseUserOrgAccess())
                LoadOrgAccessUser(list);
            else
                LoadOrgAccessRole(list);

            _orgAccess = list.ToArray();
            log.Fine("#" + _orgAccess.Length + (reload ? " - reload" : ""));
            //if (false)//Ini.isClient())
            //{
            //    StringBuilder sb = new StringBuilder();
            //    for (int i = 0; i < _orgAccess.Length; i++)
            //    {
            //        if (i > 0)
            //            sb.Append(",");
            //        sb.Append(_orgAccess[i].AD_Org_ID);
            //    }
            //    GetCtx().SetContext("#User_Org", sb.ToString());
            //}
        }	//	loadOrgAccess

        public OrgAccess[] GetOrgAccess()
        {
            return _orgAccess;
        }


        /**	Cache						*/
        private static CCache<int, String> s_clientcache = new CCache<int, String>("AD_Role_ClientC", 10);
        /**	Cache						*/
        private static CCache<int, List<String>> s_orgcache = new CCache<int, List<String>>("AD_Role_OrgC", 10);

        internal static string GetClientName(int AD_Client_ID)
        {
            String name = s_clientcache[AD_Client_ID];
            if (name == null)
            {
                name = Util.GetValueOfString(
                    DB.ExecuteScalar("SELECT Name FROM AD_Client WHERE AD_Client_ID=" + AD_Client_ID));
                s_clientcache[AD_Client_ID] = name;
            }
            return name;
        }

        internal static string GetOrgInfo(int AD_Org_ID, bool nameOnly)
        {
            List<String> lst = s_orgcache[AD_Org_ID];
            string retval = "";
            if (lst == null)
            {
                IDataReader dr = null;
                try
                {
                    dr = DB.ExecuteReader("SELECT Name,IsSummary FROM AD_Org WHERE AD_Org_ID=" + AD_Org_ID);
                    lst = new List<string>();
                    if (dr.Read())
                    {
                        lst.Add(dr[0].ToString());
                        lst.Add(dr[1].ToString());
                        s_orgcache[AD_Org_ID] = lst;
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }

            }
            if (lst != null)
            {
                if (nameOnly)
                    retval = lst[0];
                else
                    retval = lst[1];
            }
            return retval;
        }


        /// <summary>
        ///	Load Org Access User
        /// </summary>
        /// <param name="list"></param>
        private void LoadOrgAccessUser(List<OrgAccess> list)
        {
            string sql = "SELECT * FROM AD_User_OrgAccess "
                + "WHERE AD_User_ID=" + GetCtx().GetAD_User_ID() + " AND IsActive='Y'";

            DataTable dt = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                foreach (DataRow rs in dt.Rows)
                {
                    MUserOrgAccess oa = new MUserOrgAccess(GetCtx(), rs, Get_TrxName());
                    LoadOrgAccessAdd(list, new OrgAccess(oa.GetAD_Client_ID(), oa.GetAD_Org_ID(), oa.IsReadOnly()));
                }

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dt = null;
            }
        }

        /// <summary>
        /// Load Org Access Role
        /// </summary>
        /// <param name="list"></param>
        private void LoadOrgAccessRole(List<OrgAccess> list)
        {
            string sql = "SELECT * FROM AD_Role_OrgAccess "
                + "WHERE AD_Role_ID=" + GetAD_Role_ID() + " AND IsActive='Y'";
            try
            {
                IDataReader dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(dr);
                foreach (DataRow rs in dt.Rows)
                {
                    MRoleOrgAccess oa = new MRoleOrgAccess(GetCtx(), rs, Get_TrxName());
                    LoadOrgAccessAdd(list, new OrgAccess(oa.GetAD_Client_ID(), oa.GetAD_Org_ID(), oa.IsReadOnly()));
                }
                dr.Dispose();
                dt.Dispose();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /// <summary>
        /// Load Org Access Add Tree to List
        /// </summary>
        /// <param name="list"></param>
        /// <param name="oa"></param>
        private void LoadOrgAccessAdd(List<OrgAccess> list, OrgAccess oa)
        {
            if (list.Contains(oa))
            {
                return;
            }
            list.Add(oa);
            //	Do we look for trees?
            if (GetAD_Tree_Org_ID() == 0)
            {
                return;
            }
            //
            //MOrg org = MOrg.Get(GetCtx(), oa.AD_Org_ID);
            bool isSummary = GetOrgInfo(oa.AD_Org_ID, false) == "Y";
            if (!isSummary)
            {
                return;
            }
            //	Summary Org - Get Dependents
            MTree tree = MTree.Get(GetCtx(), GetAD_Tree_Org_ID(), Get_TrxName());
            String sql = "SELECT AD_Client_ID, AD_Org_ID FROM AD_Org "
                + "WHERE IsActive='Y' AND AD_Org_ID IN (SELECT Node_ID FROM "
                + tree.GetNodeTableName()
                + " WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Parent_ID=" + oa.AD_Org_ID + " AND IsActive='Y')";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int AD_Client_ID = Utility.Util.GetValueOfInt(dr[0].ToString());// rs.getInt(1);
                    int AD_Org_ID = Utility.Util.GetValueOfInt(dr[1].ToString());// rs.getInt(2);
                    LoadOrgAccessAdd(list, new OrgAccess(AD_Client_ID, AD_Org_ID, oa.readOnly));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {

                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
        }

        /// <summary>
        ///Get Table ID from name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private int GetAD_Table_ID(string tableName)
        {
            LoadTableInfo(false);
            int ii = -1;
            _tableName.TryGetValue(tableName, out ii);
            if (ii != -1)
            {
                return ii;
            }
            //	log.log(Level.WARNING,"getAD_Table_ID - not found (" + tableName + ")");
            return 0;
        }

        /// <summary>
        /// Add AccessSql Where Clause to sql query
        /// </summary>
        /// <param name="inSql"></param>
        /// <param name="TableNameIn"></param>
        /// <param name="fullyQualified"></param>
        /// <param name="rw">if false, includes System Data</param>
        /// <returns></returns>
        public string AddAccessSQL(string inSql, string TableNameIn,
                                    bool fullyQualified, bool rw)
        {
            if (fullyQualified && Util.IsEmpty(TableNameIn))
                fullyQualified = false;

            StringBuilder retSQL = new StringBuilder();

            //	Cut off last ORDER BY clause
            string orderBy = "";
            int posOrder = inSql.LastIndexOf(" ORDER BY ");
            if (posOrder != -1)
            {
                orderBy = inSql.Substring(posOrder);
                retSQL.Append(inSql.Substring(0, posOrder));
            }
            else
                retSQL.Append(inSql);

            //	Parse inSql
            AccessSqlParser asp = new AccessSqlParser(retSQL.ToString());
            AccessSqlParser.TableInfo[] ti = asp.GetTableInfo(asp.GetMainSqlIndex());

            //  Do we have to add WHERE or AND
            if (asp.GetMainSql().IndexOf(" WHERE ") == -1)
                retSQL.Append(" WHERE ");
            else
                retSQL.Append(" AND ");

            //	Use First Table
            string tableName = "";
            if (ti.Length > 0 &&
                (ti[0].GetTableName().Equals(TableNameIn)
                || ti[0].GetSynonym().Equals(TableNameIn)))
            {
                tableName = ti[0].GetSynonym();
                if (tableName.Length == 0)
                    tableName = ti[0].GetTableName();
            }
            //	Check for error condition
            if (TableNameIn != null
                && (tableName == null || tableName.Length == 0))
            {
                string msg = "TableName not correctly parsed - TableNameIn="
                    + TableNameIn + " - " + asp;
                if (ti.Length > 0)
                    msg += " - #1 " + ti[0];
                msg += "\n = " + inSql;
                log.Log(Level.SEVERE, msg);
                //Trace.printStack();
                tableName = TableNameIn;
            }

            //	Client Access
            if (fullyQualified)
                retSQL.Append(tableName).Append(".");
            retSQL.Append(GetClientWhere(rw));

            //	Org Access
            if (!IsAccessAllOrgs())
            {
                retSQL.Append(" AND ");
                if (fullyQualified && !Util.IsEmpty(tableName))
                    retSQL.Append(GetOrgWhere(tableName, rw));
                else
                    retSQL.Append(GetOrgWhere(null, rw));
            }

            if (IsUseBPRestrictions())
            {
                string documentWhere = GetDocWhere(tableName);
                if (documentWhere.Length > 0)
                {
                    retSQL.Append(" AND ");
                    retSQL.Append(documentWhere);
                }
            }
            int AD_Table_ID = 0;
            //	** Data Access	**
            for (int i = 0; i < ti.Length; i++)
            {
                string TableName = ti[i].GetTableName();
                AD_Table_ID = GetAD_Table_ID(TableName);

                // Org Access
                //if (AD_Table_ID != 0 && !IsAccessAllOrgs())
                //{
                //    String TableSynonym = ti[i].GetSynonym();
                //    if ( String.IsNullOrEmpty( TableSynonym))
                //        TableSynonym = TableName;

                //    retSQL.Append(" AND ");
                //    retSQL.Append(GetOrgWhere(TableSynonym, rw));
                //}



                //	Data Table Access
                if (AD_Table_ID != 0 && !IsTableAccess(AD_Table_ID, !rw))
                {
                    retSQL.Append(" AND 1=3");	//	prevent access at all
                    log.Fine("No access to AD_Table_ID=" + AD_Table_ID
                      + " - " + TableName + " - " + retSQL);
                    break;	//	no need to check further 
                }

                //	Data Column Access
                //	Data Record Access
                String keyColumnName = "";

                if (fullyQualified)
                {
                    keyColumnName = ti[i].GetSynonym();	//	table synonym
                    if (keyColumnName.Length == 0)
                        keyColumnName = TableName;
                    keyColumnName += ".";
                }
                keyColumnName += TableName + "_ID";	//	derived from table

                // log.Fine("addAccessSQL - " + TableName + "(" + AD_Table_ID + ") " + keyColumnName);
                string recordWhere = GetRecordWhere(AD_Table_ID, keyColumnName, rw);
                if (recordWhere.Length > 0)
                {
                    retSQL.Append(" AND ").Append(recordWhere);
                    log.Finest("Record access - " + recordWhere);
                }
            }	//	for all table info

            //	Dependent Records (only for main inSql)
            string mainSql = asp.GetMainSql();
            LoadRecordAccess(false);
            AD_Table_ID = 0;
            String whereColumnName = null;
            List<int> includes = new List<int>();
            List<int> excludes = new List<int>();
            bool isIncludeNull = false;
            for (int i = 0; i < _recordDependentAccess.Length; i++)
            {
                String columnName = _recordDependentAccess[i].GetKeyColumnName(asp.GetTableInfo(asp.GetMainSqlIndex()));
                if (columnName == null)
                    continue;	//	no key column
                int posColumn = mainSql.IndexOf(columnName);
                if (posColumn == -1)
                    continue;
                //	we found the column name - make sure it's a column name
                // string charCheck = mainSql.Substring(posColumn - 1, posColumn);	//	before
                //Updated by Raghu--14-Feb-2012 to run lock functinality for dependent records--Both for Before and after
                string charCheck = mainSql.Substring(posColumn - 1, 1);	//	before
                if (!(charCheck == "," || charCheck == "." || charCheck == " " || charCheck == "("))
                    continue;
                // charCheck = mainSql.Substring(posColumn, (posColumn + columnName.Length));	//	after
                charCheck = mainSql.Substring((posColumn + columnName.Length), 1);	//	after
                if (!(charCheck == "," || charCheck == " " || charCheck == ")"))
                    continue;

                if (AD_Table_ID != 0 && AD_Table_ID != _recordDependentAccess[i].GetAD_Table_ID())
                    retSQL.Append(GetDependentAccess(whereColumnName, includes, excludes, isIncludeNull));

                AD_Table_ID = _recordDependentAccess[i].GetAD_Table_ID();
                //	*** we found the column in the main query
                if (_recordDependentAccess[i].IsExclude())
                {
                    excludes.Add(_recordDependentAccess[i].GetRecord_ID());
                    log.Fine("Exclude " + columnName + " - " + _recordDependentAccess[i]);
                }
                else if (!rw || !_recordDependentAccess[i].IsReadOnly())
                {
                    includes.Add(_recordDependentAccess[i].GetRecord_ID());
                    log.Fine("Include " + columnName + " - " + _recordDependentAccess[i]);
                }

                isIncludeNull = isIncludeNull || _recordDependentAccess[i].IsIncludeNull();
                whereColumnName = GetDependentRecordWhereColumn(mainSql, columnName);

            }	//	for all dependent records
            retSQL.Append(GetDependentAccess(whereColumnName, includes, excludes, isIncludeNull));
            //
            retSQL.Append(orderBy);
            log.Finest(retSQL.ToString());
            return retSQL.ToString();
        }	//	addAccessSQL

        /// <summary>
        /// Get Client Where Clause Value 
        /// </summary>
        /// <param name="rw"></param>
        /// <returns></returns>
        public string GetClientWhere(bool rw)
        {
            //	All Orgs - use Client of Role
            if (IsAccessAllOrgs())
            {
                if (rw || GetAD_Client_ID() == 0)
                    return "AD_Client_ID=" + GetAD_Client_ID();
                return "AD_Client_ID IN (0," + GetAD_Client_ID() + ")";
            }

            //	Get Client from Org List
            LoadOrgAccess(false);
            //	Unique Strings
            List<string> set = new List<string>();
            if (!rw)
                set.Add("0");
            //	Positive List
            for (int i = 0; i < _orgAccess.Length; i++)
            {
                if (!set.Contains(_orgAccess[i].AD_Client_ID.ToString()))
                {
                    set.Add(_orgAccess[i].AD_Client_ID.ToString());
                }
            }
            //
            StringBuilder sb = new StringBuilder();

            bool oneOnly = true;
            for (int j = 0; j < set.Count; j++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                    oneOnly = false;
                }
                sb.Append(set[j]);
            }
            if (oneOnly)
            {
                if (sb.Length > 0)
                    return "AD_Client_ID=" + sb.ToString();
                else
                {
                    log.Log(Level.SEVERE, "No Access Org records");
                    return "AD_Client_ID=-1";	//	No Access Record
                }
            }
            return "AD_Client_ID IN(" + sb.ToString() + ")";
        }	//	getClientWhereValue

        /// <summary>
        ///Get Org Where Clause Value 
        /// </summary>
        /// <param name="rw"></param>
        /// <returns>@return "AD_Org_ID=0" or "AD_Org_ID IN(0,1)" or null (if access all org)</returns>
        public string GetOrgWhere(bool rw)
        {
            if (IsAccessAllOrgs())
                return null;
            LoadOrgAccess(false);

            //	Unique Strings
            List<String> set = new List<String>();
            if (!rw)
                set.Add("0");
            //	Positive List
            for (int i = 0; i < _orgAccess.Length; i++)
            {
                if (!set.Contains(_orgAccess[i].AD_Org_ID.ToString()))
                {
                    if (!rw)
                        set.Add(_orgAccess[i].AD_Org_ID.ToString());
                    else if (!_orgAccess[i].readOnly)	//	rw
                        set.Add(_orgAccess[i].AD_Org_ID.ToString());
                }
            }
            //
            StringBuilder sb = new StringBuilder();
            bool oneOnly = true;

            for (int j = 0; j < set.Count; j++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                    oneOnly = false;
                }
                sb.Append(set[j]);
            }
            if (oneOnly)
            {
                if (sb.Length > 0)
                    return "AD_Org_ID=" + sb.ToString();
                else
                {
                    log.Log(Level.SEVERE, "No Access Org records");
                    return "AD_Org_ID=-1";	//	No Access Record
                }
            }
            return "AD_Org_ID IN(" + sb.ToString() + ")";
        }	//	getOrgWhereValue

        /**
	 * Get Org Where Clause Value
	 * 
	 * @param rw
	 *            read write
	 * @return "AD_Org_ID=0" or "AD_Org_ID IN(0,1)" or null (if access all org)
	 */
        public String GetOrgWhere(String tableName, bool rw)
        {
            if (IsAccessAllOrgs())
                return null;
            LoadOrgAccess(false);
            // Unique Strings
            List<String> set = new List<string>();
            if (!rw)
                set.Add("0");
            // Positive List
            for (int i = 0; i < _orgAccess.Length; i++)
            {
                if (!rw)
                    set.Add(_orgAccess[i].AD_Org_ID.ToString());
                else if (!_orgAccess[i].readOnly) // rw
                    set.Add(_orgAccess[i].AD_Org_ID.ToString());
            }
            //
            if (set.Count == 1)
            {
                return "COALESCE(" + (tableName == null ? "" : tableName + ".") + "AD_Org_ID,0)=" + set[0];
            }
            else if (set.Count == 0)
            {
                log.Log(Level.SEVERE, "No Access Org records");
                return (tableName == null ? "" : tableName + ".") + "AD_Org_ID=-1"; // No Access Record
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            int count = 0;
            int total = set.Count;
            while (count < total)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append(set[count]);
                count++;
                //if there are 999 orgs already, or it reaches the end , reset
                //we do this 'cuz IN() cannot contain more than 1000 values
                if (count % 999 == 0 || count == total)
                {
                    if (sql.Length > 0)
                        sql.Append(" OR ");

                    sql.Append("COALESCE(" + (tableName == null ? "" : tableName + ".") + "AD_Org_ID,0) IN(").Append(sb.ToString()).Append(")");
                    sb = new StringBuilder();
                }
            }
            return "(" + sql.ToString() + ")";
        } // getOrgWhereValue


        /// <summary>
        ///Get Doc Where Clause Value 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string GetDocWhere(string TableName)
        {
            if (!IsUseBPRestrictions())
                return "";

            bool hasBPColumn = false;
            string sql = "SELECT count(*) FROM AD_Table t "
                            + "INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                            + "WHERE t.TableName='" + TableName
                            + "' AND c.ColumnName='C_BPartner_ID' ";
            try
            {
                string ret = DataBase.DB.ExecuteScalar(sql).ToString();
                if (ret != "")
                {
                    hasBPColumn = int.Parse(ret) != 0;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            if (!hasBPColumn)
                return "";

            int AD_User_ID = GetCtx().GetAD_User_ID();

            string docAccess = "(EXISTS (SELECT 1 FROM C_BPartner bp INNER JOIN AD_User u "
                                        + "ON (u.C_BPartner_ID=bp.C_BPartner_ID) "
                                        + " WHERE u.AD_User_ID=" + AD_User_ID
                                        + " AND bp.C_BPartner_ID=" + TableName + ".C_BPartner_ID)"
                                + " OR EXISTS (SELECT 1 FROM C_BP_Relation bpr INNER JOIN AD_User u "
                                        + "ON (u.C_BPartner_ID=bpr.C_BPartnerRelation_ID) "
                                        + " WHERE u.AD_User_ID=" + AD_User_ID
                                        + " AND bpr.C_BPartner_ID=" + TableName + ".C_BPartner_ID)";

            bool hasUserColumn = false;
            string sql1 = "SELECT count(*) FROM AD_Table t "
                            + "INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                            + "WHERE t.tableName='" + TableName
                            + "' AND c.ColumnName='AD_User_ID' ";
            try
            {

                string ret = DataBase.DB.ExecuteScalar(sql).ToString();
                if (ret != "")
                {
                    hasUserColumn = int.Parse(ret) != 0;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            if (hasUserColumn)
                docAccess += " OR " + TableName + ".AD_User_ID =" + AD_User_ID;
            docAccess += ")";

            return docAccess;
        }

        /// <summary>
        ///Return Where clause for Record Access
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="keyColumnName"></param>
        /// <param name="rw"></param>
        /// <returns></returns>
        private string GetRecordWhere(int AD_Table_ID, string keyColumnName, bool rw)
        {
            LoadRecordAccess(false);
            //
            StringBuilder sbInclude = new StringBuilder();
            StringBuilder sbExclude = new StringBuilder();
            bool isIncludeNull = false;
            //	Role Access
            for (int i = 0; i < _recordAccess.Length; i++)
            {
                if (_recordAccess[i].GetAD_Table_ID() == AD_Table_ID)
                {
                    //	NOT IN (x)
                    if (_recordAccess[i].IsExclude())
                    {
                        if (sbExclude.Length == 0)
                            sbExclude.Append(keyColumnName)
                                .Append(" NOT IN (");
                        else
                            sbExclude.Append(",");
                        sbExclude.Append(_recordAccess[i].GetRecord_ID());
                    }
                    //	IN (x)
                    else if (!rw || !_recordAccess[i].IsReadOnly())	//	include
                    {
                        if (sbInclude.Length == 0)
                            sbInclude.Append(keyColumnName)
                                .Append(" IN (");
                        else
                            sbInclude.Append(",");
                        sbInclude.Append(_recordAccess[i].GetRecord_ID());
                    }
                    isIncludeNull = isIncludeNull || _recordAccess[i].IsIncludeNull();
                }
            }	//	for all Table Access

            StringBuilder sb = new StringBuilder();
            if (sbExclude.Length > 0)
            {
                if (isIncludeNull)
                {
                    sb.Append("(");
                }
                sb.Append(sbExclude).Append(")");
                if (isIncludeNull)
                {
                    sb.Append(" OR ").Append(keyColumnName).Append(" IS NULL )");
                }
                isIncludeNull = false;
            }
            if (sbInclude.Length > 0)
            {
                if (sb.Length > 0)
                    sb.Append(" AND ");

                if (isIncludeNull)
                {
                    sb.Append("(");
                }
                sb.Append(sbInclude).Append(")");
                if (isIncludeNull)
                {
                    sb.Append(" OR ").Append(keyColumnName).Append(" IS NULL )");
                }
            }

            //Don't ignore Privacy Access
            if (!IsPersonalAccess())
            {
                //Lakhwinder
                MTable table = MTable.Get(GetCtx(), AD_Table_ID);
                if (!table.IsView() && table.Haskey(AD_Table_ID))
                {
                    string lockedIDs = MPrivateAccess.GetLockedRecordWhere(AD_Table_ID, _AD_User_ID);
                    if (lockedIDs != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(" AND ");
                        sb.Append(keyColumnName).Append(lockedIDs);
                    }
                }
            }
            //
            return sb.ToString();
        }	//	getRecordWhere

        /// <summary>
        ///Get Dependent Access 
        /// </summary>
        /// <param name="whereColumnName"></param>
        /// <param name="includes"></param>
        /// <param name="excludes"></param>
        /// <param name="isIncludeNull"></param>
        /// <returns></returns>
        private string GetDependentAccess(string whereColumnName,
                    List<int> includes, List<int> excludes,bool isIncludeNull)
        {
            if (includes.Count == 0 && excludes.Count == 0)
                return "";
            if (includes.Count != 0 && excludes.Count != 0)
            {
                log.Warning("Mixing Include and Excluse rules - Will not return values");
            }
            StringBuilder where = new StringBuilder(" AND ");
            if (isIncludeNull)
                where.Append(" ( ");
            if (includes.Count == 1)
                where.Append(whereColumnName).Append("=").Append(includes[0]);
            else if (includes.Count > 1)
            {
                where.Append(whereColumnName).Append(" IN (");
                for (int ii = 0; ii < includes.Count; ii++)
                {
                    if (ii > 0)
                        where.Append(",");
                    where.Append(includes[ii]);
                }
                where.Append(")");
            }
            else if (excludes.Count == 1)
                where.Append(whereColumnName).Append("<>").Append(excludes[0]);
            else if (excludes.Count > 1)
            {
                where.Append(whereColumnName).Append(" NOT IN (");
                for (int ii = 0; ii < excludes.Count; ii++)
                {
                    if (ii > 0)
                        where.Append(",");
                    where.Append(excludes[ii]);
                }
                where.Append(")");
            }
            if (isIncludeNull)
                where.Append(" OR ").Append(whereColumnName).Append(" IS NULL ").Append( " ) ");
            log.Finest(where.ToString());
            return where.ToString();
        }	//	getDependent

        /// <summary>
        ///Get Dependent Record Where clause
        /// </summary>
        /// <param name="mainSql"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string GetDependentRecordWhereColumn(string mainSql, string columnName)
        {
            string retValue = columnName;	//	if nothing else found
            int index = mainSql.IndexOf(columnName);
            //	see if there are table synonym
            int offset = index - 1;
            char c = char.Parse(mainSql.Substring(offset, 1));
            if (c == '.')
            {
                StringBuilder sb = new StringBuilder();
                while (c != ' ' && c != ',' && c != '(')	//	delimeter
                {
                    sb.Insert(0, c);
                    c = char.Parse(mainSql.Substring(--offset, 1));
                }
                sb.Append(columnName);
                return sb.ToString();
            }
            return retValue;
        }	//	getDep

        /// <summary>
        ///UPADATE - Can user Update the record.
        /// Access error info (AccessTableNoUpdate) is saved in the log-yet imolement
        /// </summary>
        /// <param name="AD_Client_ID">AD_Client_ID comntext to derive client/org/user level</param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Table_ID">AD_Table_ID table</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="createError">createError bool</param>
        /// <returns>true if you can update</returns>
        public bool CanUpdate(int AD_Client_ID, int AD_Org_ID,
            int AD_Table_ID, int Record_ID, bool createError)
        {
            string userLevel = GetUserLevel().Trim();	//	Format 'SCO'
            bool retValue = true;
            string whatMissing = "";

            //	System == Client=0 & Org=0
            if (AD_Client_ID == 0 && AD_Org_ID == 0
                && userLevel.IndexOf('S') == -1)
            {
                retValue = false;
                whatMissing += "S";
            }

            //	Client == Client!=0 & Org=0
            else if (AD_Client_ID != 0 && AD_Org_ID == 0
                && userLevel.IndexOf('C') == -1)
            {
                if (userLevel.IndexOf('O') == -1 && IsOrgAccess(AD_Org_ID, true))
                { }                 //	Client+Org with access to *
                else
                {
                    retValue = false;
                    whatMissing += "C";
                }
            }

            //	Organization == Client!=0 & Org!=0
            else if (AD_Client_ID != 0 && AD_Org_ID != 0
                && userLevel.IndexOf('O') == -1)
            {
                retValue = false;
                whatMissing += "O";
            }

            //	Data Access
            if (retValue)
                retValue = IsTableAccess(AD_Table_ID, false);

            if (retValue && Record_ID != 0)
                retValue = IsRecordAccess(AD_Table_ID, Record_ID, false);

            if (!retValue && createError)
            {
                //Log Error 
                log.SaveWarning("AccessTableNoUpdate",
                    "AD_Client_ID=" + AD_Client_ID
                    + ", AD_Org_ID=" + AD_Org_ID + ", UserLevel=" + userLevel
                    + " => missing=" + whatMissing);
                log.Warning(ToString());
            }
            return retValue;
        }	//	canUpdate

        /// <summary>
        ///	check Access to Org
        /// </summary>
        /// <param name="AD_Org_ID"></param>
        /// <param name="rw"></param>
        /// <returns>true if access</returns>
        public bool IsOrgAccess(int AD_Org_ID, bool rw)
        {
            if (IsAccessAllOrgs())
                return true;
            if (AD_Org_ID == 0 && !rw)		//	can always read common org
                return true;
            LoadOrgAccess(false);

            //	Positive List
            for (int i = 0; i < _orgAccess.Length; i++)
            {
                if (_orgAccess[i].AD_Org_ID == AD_Org_ID)
                {
                    if (!rw)
                        return true;
                    if (!_orgAccess[i].readOnly)	//	rw
                        return true;
                    return false;
                }
            }
            return false;
        }	//	isOrgAcce

        /// <summary>
        ///Access to Record (no check of table)
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="ro"></param>
        /// <returns></returns>
        public bool IsRecordAccess(int AD_Table_ID, int Record_ID, bool ro)
        {
            //	if (!isTableAccess(AD_Table_ID, ro))		//	No Access to Table
            //		return false;
            LoadRecordAccess(false);
            bool negativeList = true;
            for (int i = 0; i < _recordAccess.Length; i++)
            {
                MRecordAccess ra = _recordAccess[i];
                if (ra.Get_Table_ID() != AD_Table_ID)
                    continue;

                if (ra.IsExclude())		//	Exclude
                //	If you Exclude Access to a column and select Read Only, 
                //	you can only read data (otherwise no access).
                {
                    if (ra.GetRecord_ID() == Record_ID)
                    {
                        if (ro)
                            return ra.IsReadOnly();
                        else
                            return false;
                    }
                }
                else								//	Include
                //	If you Include Access to a column and select Read Only, 
                //	you can only read data (otherwise full access).
                {
                    negativeList = false;	//	has to be defined
                    if (ra.GetRecord_ID() == Record_ID)
                    {
                        if (!ro)
                            return !ra.IsReadOnly();
                        else	//	ro
                            return true;
                    }
                }
            }	//	for all Table Access
            return negativeList;
        }	//	isRecordAccess


        /// <summary>
        /// Can Report on table
        /// </summary>
        /// <param name="AD_Table_ID">table</param>
        /// <returns>true if access</returns>
        public bool IsCanReport(int AD_Table_ID)
        {
            if (!IsCanReport())						//	Role Level block
            {
                log.Warning("Role denied");
                return false;
            }
            if (!IsTableAccess(AD_Table_ID, true))	//	No R/O Access to Table
                return false;
            //
            bool canReport = true;
            for (int i = 0; i < _tableAccess.Length; i++)
            {
                if (!X_AD_Table_Access.ACCESSTYPERULE_Reporting.Equals(_tableAccess[i].GetAccessTypeRule()))
                    continue;
                if (_tableAccess[i].IsExclude())		//	Exclude
                {
                    if (_tableAccess[i].GetAD_Table_ID() == AD_Table_ID)
                    {
                        canReport = _tableAccess[i].IsCanReport();
                        log.Fine("Exclude " + AD_Table_ID + " - " + canReport);
                        return canReport;
                    }
                }
                else									//	Include
                {
                    canReport = false;
                    if (_tableAccess[i].GetAD_Table_ID() == AD_Table_ID)
                    {
                        canReport = _tableAccess[i].IsCanReport();
                        log.Fine("Include " + AD_Table_ID + " - " + canReport);
                        return canReport;
                    }
                }
            }	//	for all Table Access
            log.Fine(AD_Table_ID + " - " + canReport);
            return canReport;
        }

        /// <summary>
        /// Can Export Table
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <returns> true if access</returns>
        /// <author>raghu</author>
        public bool IsCanExport(int AD_Table_ID)
        {
            if (!IsCanExport())						//	Role Level block
            {
                log.Warning("Role denied");
                return false;
            }
            if (!IsTableAccess(AD_Table_ID, true))	//	No R/O Access to Table
            {
                return false;
            }
            if (!IsCanReport(AD_Table_ID))			//	We cannot Export if we cannot report
            {
                return false;
            }
            bool canExport = true;
            for (int i = 0; i < _tableAccess.Length; i++)
            {
                if (!X_AD_Table_Access.ACCESSTYPERULE_Exporting.Equals(_tableAccess[i].GetAccessTypeRule()))
                {
                    continue;
                }
                if (_tableAccess[i].IsExclude())		//	Exclude
                {
                    canExport = _tableAccess[i].IsCanExport();
                    log.Fine("Exclude " + AD_Table_ID + " - " + canExport);
                    return canExport;
                }
                else									//	Include
                {
                    canExport = false;
                    canExport = _tableAccess[i].IsCanExport();
                    log.Fine("Include " + AD_Table_ID + " - " + canExport);
                    return canExport;
                }
            }	//	for all Table Access
            log.Fine(AD_Table_ID + " - " + canExport);
            return canExport;
        }



        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRole[");
            sb.Append(GetAD_Role_ID()).Append(",").Append(GetName())
                .Append(",UserLevel=").Append(GetUserLevel())
                .Append(",").Append(GetClientWhere(false))
                .Append(",").Append(GetOrgWhere(false))
                .Append("]");
            return sb.ToString();
        }	//	ToString



        public String ToStringX(Ctx ctx)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Msg.Translate(ctx, "AD_Role_ID")).Append("=").Append(GetName())
                .Append(" - ").Append(Msg.Translate(ctx, "IsCanExport")).Append("=").Append(IsCanExport())
                .Append(" - ").Append(Msg.Translate(ctx, "IsCanReport")).Append("=").Append(IsCanReport())
                .Append(Utility.Env.NL).Append(Utility.Env.NL);
            //
            for (int i = 0; i < _orgAccess.Length; i++)
                sb.Append(_orgAccess[i].ToString()).Append(Utility.Env.NL);
            sb.Append(Utility.Env.NL);
            //
            LoadTableAccess(false);
            for (int i = 0; i < _tableAccess.Length; i++)
                sb.Append(_tableAccess[i].ToStringX(ctx)).Append(Utility.Env.NL);
            if (_tableAccess.Length > 0)
                sb.Append(Utility.Env.NL);
            //
            LoadColumnAccess(false);
            for (int i = 0; i < _columnAccess.Length; i++)
                sb.Append(_columnAccess[i].ToStringX(ctx)).Append(Utility.Env.NL);
            if (_columnAccess.Length > 0)
                sb.Append(Utility.Env.NL);
            //
            LoadRecordAccess(false);
            for (int i = 0; i < _recordAccess.Length; i++)
                sb.Append(_recordAccess[i].ToStringX(ctx)).Append(Utility.Env.NL);
            return sb.ToString();

        }	//	toStringX


        /// <summary>
        ///Require Query
        /// </summary>
        /// <param name="noRecords"></param>
        /// <returns></returns>
        public bool IsQueryRequire(int noRecords)
        {
            if (noRecords < 2)
                return false;
            int max = GetMaxQueryRecords();
            if (max > 0 && noRecords > max)
                return true;
            int qu = GetConfirmQueryRecords();
            return (noRecords > qu);
        }

        /// <summary>
        /// Get Confirm Query Records
        /// </summary>
        /// <returns>entered records or 500 (default)</returns>
        public new int GetConfirmQueryRecords()
        {

            int no = base.GetConfirmQueryRecords();
            if (no == 0)
                return 500;
            return no;
        }

        private void UpdateLoginSettings()
        {
            if (!IsActive())
            {
                DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_Role_ID = " + GetAD_Role_ID());
                return;
            }

            if (!IsUseUserOrgAccess())
            {
                DataSet ds = DB.ExecuteDataset("SELECT AD_User_ID,AD_ORg_ID From ad_loginsetting WHERE AD_Role_ID = " + GetAD_Role_ID());

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(DB.ExecuteScalar("SELECT count(1) FROM AD_Role_OrgAccess WHERE IsActive='Y' AND AD_Role_ID = " + GetAD_Role_ID() + " AND AD_ORg_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]), null, null)) == 0)
                    {
                        DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_ORg_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]) + " AND AD_User_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_User_ID"]) + " AND AD_Role_ID = " + GetAD_Role_ID());
                    }
                }
            }
            else
            {
                DataSet ds = DB.ExecuteDataset("SELECT AD_User_ID,AD_ORg_ID From ad_loginsetting WHERE AD_Role_ID = " + GetAD_Role_ID());

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(DB.ExecuteScalar("SELECT count(1) FROM AD_User_OrgAccess WHERE IsActive='Y' AND AD_User_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_User_ID"]) + " AND AD_ORg_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]), null, null)) == 0)
                    {
                        DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_ORg_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]) + " AND AD_User_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_User_ID"]) + " AND AD_Role_ID = " + GetAD_Role_ID());
                    }
                }
            }

        }


        /**
	 * Checks the access rights of the given role/client for the given document actions.
	 * @param clientId
	 * @param docTypeId
	 * @param options
	 * @param maxIndex
	 * @return number of valid actions in the String[] options
	 * @see metas-2009_0021_AP1_G94
	 */
        public string[] checkActionAccess(int clientId, int docTypeId, String[] options, ref int maxIndex)
        {
            if (maxIndex <= 0)
                return options;

            // Code Commented // as no need to check records on Document Action Access for validation of Doc Actions in dialog
            //string sqlcheck = "SELECT COUNT(*) FROM AD_Document_Action_Access WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID();
            //int count = Util.GetValueOfInt(DB.ExecuteScalar(sqlcheck));

            //if (count == 0)
            //{
            //    maxIndex = maxIndex;
            //    return options;
            //}

            // Check applied based on the Doc Action Access Checkbox to display options in the Doc Action Dialog
            if(!IsCheckDocActionAccess())
            {
                return options;
            }


            //
            List<String> validOptions = new List<string>();
            List<Object> param = new List<object>();
            param.Add(clientId);
            param.Add(docTypeId);
            //
            StringBuilder sql_values = new StringBuilder();
            for (int i = 0; i < maxIndex; i++)
            {
                if (sql_values.Length > 0)
                    sql_values.Append(",");
                sql_values.Append("'" + options[i] + "'");
                //param.Add(options[i]);
            }
            //
            String sql = "SELECT DISTINCT rl.Value FROM AD_Document_Action_Access a"
                    + " INNER JOIN AD_Ref_List rl ON (rl.AD_Reference_ID=135 and rl.AD_Ref_List_ID=a.AD_Ref_List_ID)"
                    + " WHERE a.IsActive='Y' AND a.AD_Client_ID=" + clientId + " AND a.C_DocType_ID=" + docTypeId // #1,2
                        + " AND rl.Value IN (" + sql_values + ")"
                        + " AND " + GetIncludedRolesWhereClause("a.AD_Role_ID", param)
            ;
            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql, null, null);
                while (dr.Read())
                {
                    String op = dr.GetString(0);
                    validOptions.Add(op);
                }
                options = validOptions.ToArray();
            }
            catch (SqlException e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            //
            //int newMaxIndex = validOptions.Count();
            //return newMaxIndex;
            maxIndex = validOptions.Count();
            return validOptions.ToArray();
        }


        /**
	 * Get Role Where Clause.
	 * It will look something like myalias.AD_Role_ID IN (?, ?, ?).
	 * @param roleColumnSQL role columnname or role column SQL (e.g. myalias.AD_Role_ID) 
	 * @param params a list where the method will put SQL parameters.
	 * 				If null, this method will generate a not parametrized query 
	 * @return role SQL where clause
	 */
        public String GetIncludedRolesWhereClause(String roleColumnSQL, List<Object> param)
        {
            StringBuilder whereClause = new StringBuilder();
            // if (param != null)
            //{
            whereClause.Append(GetAD_Role_ID());
            // param.Add(GetAD_Role_ID());
            // }
            // else
            // {
            //  whereClause.Append(GetAD_Role_ID());
            //}
            //
            foreach (MRole role in GetIncludedRoles(true))
            {
                // if (param != null)
                //  {
                //      whereClause.Append("," + role.GetAD_Role_ID());
                //      param.Add(role.GetAD_Role_ID());
                //  }
                //   else
                //   {
                whereClause.Append(",").Append(role.GetAD_Role_ID());
                // }
            }
            //
            whereClause.Insert(0, roleColumnSQL + " IN (").Append(")");
            return whereClause.ToString();
        }

        private List<MRole> m_includedRoles = null;
        /**
	 * 
	 * @return unmodifiable list of included roles
	 * @see metas-2009_0021_AP1_G94
	 */
        public List<MRole> GetIncludedRoles(Boolean recursive)
        {
            if (!recursive)
            {
                List<MRole> list = this.m_includedRoles;
                if (list == null)
                    list = new List<MRole>();
                return list;
            }
            else
            {
                List<MRole> list = new List<MRole>();
                if (m_includedRoles != null)
                {
                    foreach (MRole role in m_includedRoles)
                    {
                        list.Add(role);
                        list.AddRange(role.GetIncludedRoles(true));
                    }
                }
                return list;
            }
        }



        //	MRole



        /// <summary>
        /// Class contain Org Access properties
        /// </summary>
        public class OrgAccess
        {
            /// <summary>
            /// Org Access constructor
            /// </summary>
            /// <param name="ad_Client_ID"></param>
            /// <param name="ad_Org_ID"></param>
            /// <param name="readoly"></param>
            public OrgAccess(int ad_Client_ID, int ad_Org_ID, bool readoly)
            {
                this.AD_Client_ID = ad_Client_ID;
                this.AD_Org_ID = ad_Org_ID;
                this.readOnly = readoly;
            }
            /** Client				*/
            public int AD_Client_ID = 0;
            /** Organization		*/
            public int AD_Org_ID = 0;
            /** Read Only			*/
            public bool readOnly = true;


            /// <summary>
            ///	Equals
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(Object obj)
            {
                if (obj != null && obj.GetType().Equals(typeof(OrgAccess)))
                {
                    OrgAccess comp = (OrgAccess)obj;
                    return comp.AD_Client_ID == AD_Client_ID
                        && comp.AD_Org_ID == AD_Org_ID;
                }
                return false;
            }	//	equals


            public override String ToString()
            {
                String clientName = "System";
                if (AD_Client_ID != 0)
                    clientName = GetClientName(AD_Client_ID);//MClient.Get(Env.GetContext(), AD_Client_ID).GetName();
                String orgName = "*";
                if (AD_Org_ID != 0)
                    orgName = GetOrgInfo(AD_Org_ID, true); //MOrg.Get(Env.GetContext(), AD_Org_ID).GetName();
                StringBuilder sb = new StringBuilder();
                sb.Append(Msg.Translate(Env.GetContext(), "AD_Client_ID")).Append("=")
                    .Append(clientName).Append(" - ")
                    .Append(Msg.Translate(Env.GetContext(), "AD_Org_ID")).Append("=")
                    .Append(orgName);
                if (readOnly)
                    sb.Append(" r/o");
                return sb.ToString();
            }	//	ToString

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }


        }	//	OrgAccess
    }

    /********************************************************************/

    //        class Org access

    /********************************************************************/


}

