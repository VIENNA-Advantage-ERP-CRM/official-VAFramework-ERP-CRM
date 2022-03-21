/********************************************************
 * Module Name    : Process
 * Purpose        : 
 * Class Used     : X_AD_Process_Access
 * Chronological Development
 * Jagmohan Bhatt    07-May-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Model;
using System.IO;
using VAdvantage.Utility;
using VAdvantage.Controller;
using VAdvantage.ProcessEngine;


namespace VAdvantage.Model
{
    public class MProcess : X_AD_Process
    {
        private static CCache<int, MProcess> _cache = new CCache<int, MProcess>("AD_Process", 20);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MProcess).FullName);

        /// <summary>
        /// Get MProcess from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Process_ID">AD_Process_ID</param>
        /// <returns>MProcess</returns>
        public static MProcess Get(Ctx ctx, int AD_Process_ID)
        {
            int key = AD_Process_ID;
            MProcess retValue = null;
            if(_cache.ContainsKey(key))
                retValue = (MProcess)_cache[key];
            if (retValue != null)
            {
                retValue.p_ctx = ctx;
                return retValue;
            }
            retValue = new MProcess(ctx, AD_Process_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }


        /// <summary>
        /// Get MProcess by Value
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="value">value</param>
        /// <returns>MProcess</returns>

        public static MProcess GetByValue(Ctx ctx, string value)
        {
            MProcess retValue = null;
            String sql = "SELECT * FROM AD_Process p "
                + "WHERE p.Value LIKE @like";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@like", value);
            DataSet ds;

            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MProcess(ctx, dr, null);
                        //	Save in cache
                        int key = retValue.GetAD_Process_ID();
                        _cache.Add(key, retValue);
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {
                ds = null;
                s_log.Log(Level.SEVERE, sql, e);
            }
           
            return retValue;
        }

        /// <summary>
        /// Get MProcess from Menu
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Menu_ID">menu id</param>
        /// <returns>MProcess</returns>
        public static MProcess GetFromMenu(Ctx ctx, int AD_Menu_ID)
        {
            MProcess retValue = null;
            String sql = "SELECT * FROM AD_Process p "
                + "WHERE EXISTS (SELECT * FROM AD_Menu m "
                    + "WHERE m.AD_Process_ID=p.AD_Process_ID AND m.AD_Menu_ID=@menuid)";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@menuid", AD_Menu_ID.ToString());
            DataTable dt=null;
            IDataReader idr=null; 
            try
            {
                idr= DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                int totCount = dt.Rows.Count;

                for (int i = 0; i < totCount; i++)
                {
                    DataRow dr = dt.Rows[i];
                    retValue = new MProcess(ctx, dr, null);
                    //	Save in cache
                    int key = retValue.GetAD_Process_ID();
                    _cache.Add(key, retValue);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return retValue;
        }	//	getFromMenu


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="ignored"> no transaction</param>
        public MProcess(Ctx ctx, DataRow rs, Trx ignored)
            : base(ctx, rs, null)
        {

        }	//	MProcess

        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="ctx">context</param>
        ///// <param name="AD_Process_ID">ad process id</param>
        ///// <param name="ignored">no transaction</param>
        //public MProcess(Ctx ctx, int AD_Process_ID, String ignored)
        //    : base(ctx, AD_Process_ID, null)
        //{
        //    if (AD_Process_ID == 0)
        //    {
        //        //	setValue (null);
        //        //	setName (null);
        //        SetIsReport(false);
        //        SetIsServerProcess(false);
        //        SetAccessLevel(ACCESSLEVEL_All);
        //        SetEntityType(ENTITYTYPE_UserMaintained);
        //        SetIsBetaFunctionality(false);
        //    }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Process_ID">ad process id</param>
        /// <param name="ignored">no transaction</param>
        public MProcess(Ctx ctx, int AD_Process_ID, Trx ignored)
            : base(ctx, AD_Process_ID, null)
        {
            if (AD_Process_ID == 0)
            {
                //	setValue (null);
                //	setName (null);
                SetIsReport(false);
                SetIsServerProcess(false);
                SetAccessLevel(ACCESSLEVEL_All);
                SetEntityType(ENTITYTYPE_UserMaintained);
                SetIsBetaFunctionality(false);
            }
        }


        //public MProcess(Ctx ctx, DataRow rs, String ignored)
        //    : base(ctx, rs, null)
        //{
        //}	//	MProcess

        private MProcessPara[] _parameters = null;

        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <returns>Parameters</returns>
        public MProcessPara[] GetParameters()
        {
            if (_parameters != null)
                return _parameters;
            List<MProcessPara> list = new List<MProcessPara>();
            //
            String sql = "SELECT * FROM AD_Process_Para WHERE AD_Process_ID=@processid ORDER BY SeqNo";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@processid", GetAD_Process_ID());
            DataTable dt=null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                int totCount = dt.Rows.Count;
                for (int i = 0; i < totCount; i++)
                {
                    DataRow dr = dt.Rows[i];
                    list.Add(new MProcessPara(GetCtx(), dr, null));
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
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            //
            _parameters = new MProcessPara[list.Count];
            _parameters = list.ToArray();
            return _parameters;
        }

        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <param name="name">param name</param>
        /// <returns>ProcessPara</returns>
        public MProcessPara GetParameter(String name)
        {
            GetParameters();
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (_parameters[i].GetColumnName().Equals(name))
                    return _parameters[i];
            }
            return null;
        }	//	getParameter


        /**************************************************************************
         * 	Process SQL Procedures w/o parameter
         *	@param Record_ID record
         *	@return Process Instance
         */
        public MPInstance ProcessIt(int Record_ID)
        {
            MPInstance pInstance = new MPInstance(this, Record_ID);
            //	Lock
            pInstance.SetIsProcessing(true);
            //pInstance.save();

            bool ok = true;

            //	PL/SQL Procedure
            String ProcedureName = GetProcedureName();
            //	String Classname = getClassname();
            if (ProcedureName != null && ProcedureName.Length > 0)
                ok = StartProcess(ProcedureName, pInstance);
            //	else if (Classname != null && Classname.length() > 0)
            //		ok = startClass(Classname, pi, trx);


            //	Unlock
            pInstance.SetResult(ok ? MPInstance.RESULT_OK : MPInstance.RESULT_ERROR);
            pInstance.SetIsProcessing(false);
            //pInstance.save();
            //
            //pInstance.log();
            return pInstance;
        }	//	process


        /**
         * 	Process It (sync)
         *	@param pi Process Info
         *	@param trx transaction
         *	@return true if OK
         */
        public bool ProcessIt(ProcessInfo pi, Trx trx)
        {
            if (pi.GetAD_PInstance_ID() == 0)
            {
                MPInstance pInstance = new MPInstance(this, pi.GetRecord_ID());
                //	Lock
                pInstance.SetIsProcessing(true);
                pInstance.Save();
            }

            bool ok = false;

            //	 Class
            String Classname = GetClassname();
            if (Classname != null && Classname.Length > 0)
                ok = StartClass(Classname, pi, trx);
                // checked if procedure is linked with Process 
                // then execute procedure
            else if (GetProcedureName() != null && GetProcedureName().Trim().Length > 0)
            {
                ProcessCtl pc = new ProcessCtl(GetCtx(), null, pi, trx);
                ok = pc.StartDBProcess(GetProcedureName());
            }
            else
            {
                if (!IsReport())
                {
                    String msg = "No Classname for " + GetName();
                    pi.SetSummary(msg, ok);
                }
                else
                {
                    String msg = Msg.GetMsg(GetCtx(), "Success") + " " + GetValue();
                    pi.SetSummary(msg, ok);
                }
                //log.warning(msg);
            }

            return ok;
        }


        //public bool IsJavaProcess()
        //{
        //    String Classname = GetClassname();
        //    return (Classname != null && Classname.Length > 0);
        //}

        private bool StartProcess(String ProcedureName, MPInstance pInstance)
        {
            int AD_PInstance_ID = pInstance.GetAD_PInstance_ID();
            //  execute on this thread/connection
            String sql = "{call " + ProcedureName + "(@instanceid)}";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@instanceid", AD_PInstance_ID);
                int i = DataBase.DB.ExecuteQuery(sql, param);
            }
            catch (Exception e)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
                pInstance.SetResult(MPInstance.RESULT_ERROR);
                pInstance.SetErrorMsg(e.Message);
                return false;
            }
            pInstance.SetResult(MPInstance.RESULT_OK);
            return true;
        }

        /// <summary>
        /// Starts the class
        /// </summary>
        /// <param name="Classname">class name</param>
        /// <param name="pi">process info</param>
        /// <param name="trx">trx</param>
        /// <returns>status in bool</returns>
        private bool StartClass(String className, ProcessInfo pi, Trx trx)
        {
            bool retValue = false;
            ProcessEngine.ProcessCall myObject = null;

            //System.Reflection.Assembly asm = null;
            Type type = null;
           
            try
            {
                /*********** Module ********************/

               

               // string cName = className.Substring(className.LastIndexOf('.') + 1);
                //Name Of process
                string cName = pi.GetTitle();


                type = Utility.ModuleTypeConatiner.GetClassType(className, cName);

                
                myObject = (ProcessEngine.ProcessCall)Activator.CreateInstance(type);

                if (myObject == null)
                    retValue = false;
                else
                    retValue = myObject.StartProcess(GetCtx(), pi, trx);
            }
            catch (Exception e)
            {
                pi.SetSummary("Error Start Class " + className, true);
                log.Log(VAdvantage.Logging.Level.SEVERE, className, e);
                //throw new Exception(e.ToString());
            }
                return retValue;
        }
        /**
	 * 	Is it a Workflow
	 *	@return true if Workflow
	 */

    

        
        public bool isWorkflow()
        {
            return GetAD_Workflow_ID() > 0;
        }	//	isWorkflow


        /**
         * 	Update Statistics
         *	@param seconds sec
         */

      
        public void AddStatistics(decimal seconds)
        {
            SetStatistic_Count(GetStatistic_Count() + 1);
            SetStatistic_Seconds(GetStatistic_Seconds() +seconds);
        }	//	addStatistics

   
        /// <summary>
        ///   After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)	//	Add to all automatic roles
            {
                MRole[] roles = MRole.GetOf(GetCtx(), "IsManual='N'");
                MProcessAccess pa;
                for (int i = 0; i < roles.Length; i++)
                {
                    pa = new MProcessAccess(this, roles[i].GetAD_Role_ID());
                    pa.Save();
                    pa = null;
                }
            }
            //	Menu/Workflow
            else if (Is_ValueChanged("IsActive") || Is_ValueChanged("Name")
                || Is_ValueChanged("Description") || Is_ValueChanged("Help"))
            {
                MMenu[] menues = MMenu.Get(GetCtx(), "AD_Process_ID=" + GetAD_Process_ID());
                for (int i = 0; i < menues.Length; i++)
                {
                    menues[i].SetIsActive(IsActive());
                    menues[i].SetName(GetName());
                    menues[i].SetDescription(GetDescription());
                    menues[i].Save();
                }
                X_AD_WF_Node[] nodes = MWindow.GetWFNodes(GetCtx(), "AD_Process_ID=" + GetAD_Process_ID());
                for (int i = 0; i < nodes.Length; i++)
                {
                    bool changed = false;
                    if (nodes[i].IsActive() != IsActive())
                    {
                        nodes[i].SetIsActive(IsActive());
                        changed = true;
                    }
                    if (nodes[i].IsCentrallyMaintained())
                    {
                        nodes[i].SetName(GetName());
                        nodes[i].SetDescription(GetDescription());
                        nodes[i].SetHelp(GetHelp());
                        changed = true;
                    }
                    if (changed)
                        nodes[i].Save();
                }
            }
            return success;
        }

        /// <summary>
        /// Get AD_Process_ID by Value
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="value"></param>
        /// <returns>MProcess or null</returns>
        /// /// by raghu 07-march
        public static int GetIDByValue(Ctx ctx, String value)
        {
            int retValue = -1;
            String sql = "SELECT AD_Process_ID FROM AD_Process p "
                + "WHERE p.Value LIKE @param1";

            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", value);
                idr = DB.ExecuteReader(sql, param);
                if (idr.Read())
                {
                    retValue = Util.GetValueOfInt(idr[0]);
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return retValue;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns></returns>
        /// by raghu 07-march
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProcess[")
                .Append(Get_ID())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}
