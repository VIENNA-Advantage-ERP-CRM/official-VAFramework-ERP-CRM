/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWorkflowProcessor
 * Purpose        : 
 * Class Used     : MWorkflowProcessor inherits X_AD_WorkflowProcessor, ViennaProcessor classes
 * Chronological    Development
 * Raghunandan      05-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using VAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.WF;
using VAdvantage.Logging;
using VAdvantage.Utility;
namespace VAdvantage.WF
{
    public class MWorkflowProcessor : X_AD_WorkflowProcessor, ViennaProcessor
    {
        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MWorkflowProcessor).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WorkflowProcessor_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWorkflowProcessor(Ctx ctx, int AD_WorkflowProcessor_ID, Trx trxName)
            : base(ctx, AD_WorkflowProcessor_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MWorkflowProcessor(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Active
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>active processors</returns>
        public static MWorkflowProcessor[] GetActive(Ctx ctx)
        {
            List<MWorkflowProcessor> list = new List<MWorkflowProcessor>();
            String sql = "SELECT * FROM AD_WorkflowProcessor WHERE IsActive='Y'";
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    list.Add(new MWorkflowProcessor(ctx, dr, null));
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                if (pstmt != null)
                {
                    pstmt = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            MWorkflowProcessor[] retValue = new MWorkflowProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Active Workflow Processors
        /// VIS0060 - 21-Oct-2021
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ExecuteProcess"></param>
        /// <returns>active processors</returns>
        public static MWorkflowProcessor[] GetActive(Ctx ctx, string ExecuteProcess, string machineIP)
        {
            List<MWorkflowProcessor> list = new List<MWorkflowProcessor>();
            String sql = "SELECT * FROM AD_WorkflowProcessor WHERE IsActive='Y'";

            string scheduleIP = null;
            try
            {
                // string machineIP = Classes.CommonFunctions.GetMachineIPPort();
                _log.SaveError("Console VServer Machine IP : " + machineIP, "Console VServer Machine IP : " + machineIP);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM AD_Schedule WHERE 
                                                       AD_Schedule_ID = (SELECT AD_Schedule_ID FROM AD_WorkflowProcessor WHERE AD_WorkflowProcessor_ID =" + dr["AD_WorkflowProcessor_ID"] + " )"));

                        if (ExecuteProcess.Equals("2") && (string.IsNullOrEmpty(scheduleIP) || machineIP.Equals(scheduleIP)))
                        {
                            list.Add(new MWorkflowProcessor(new Ctx(), dr, null));
                        }
                        else if (!string.IsNullOrEmpty(scheduleIP) && machineIP.Equals(scheduleIP))
                        {
                            list.Add(new MWorkflowProcessor(ctx, dr, null));
                        }
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MWorkflowProcessor[] retValue = new MWorkflowProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Server ID
        /// </summary>
        /// <returns>id</returns>
        public String GetServerID()
        {
            return "WorkflowProcessor" + Get_ID();
        }

        /// <summary>
        /// Get Date Next Run
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>date next run</returns>
        public DateTime? GetDateNextRun(bool requery)
        {
            if (requery)
                Load(Get_TrxName());
            return GetDateNextRun();
        }

        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>logs</returns>
        public ViennaProcessorLog[] GetLogs()
        {
            List<MWorkflowProcessorLog> list = new List<MWorkflowProcessorLog>();
            String sql = "SELECT * "
                + "FROM AD_WorkflowProcessorLog "
                + "WHERE AD_WorkflowProcessor_ID=" + GetAD_WorkflowProcessor_ID() + "ORDER BY Created DESC";
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    list.Add(new MWorkflowProcessorLog(GetCtx(), dr, Get_TrxName()));
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                if (pstmt != null)
                {
                    pstmt = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
            MWorkflowProcessorLog[] retValue = new MWorkflowProcessorLog[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Delete old Request Log
        /// </summary>
        /// <returns>number of records</returns>
        public int DeleteLog()
        {
            if (GetKeepLogDays() < 1)
                return 0;
            String sql = "DELETE FROM AD_WorkflowProcessorLog "
                + "WHERE AD_WorkflowProcessor_ID=" + GetAD_WorkflowProcessor_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            int no = DB.ExecuteQuery(sql);
            return no;
        }

        /// <summary>
        /// Before Save logic for 
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetEsclReminder() > 0)
            {
                if (!(IsEsclAppSupervisor() || GetAD_WF_Responsible_ID() > 0))
                {
                    // PreferNode
                    // VIS0008 Added to check preference at node level as well
                    if (!IsPreferNode())
                    {
                        log.SaveError("VIS_EsclNotProvided", "");
                        return false;
                    }
                }
                if (IsEsclForward())
                {
                    if (IsEsclAppSupervisor() && GetAD_WF_Responsible_ID() > 0)
                    {
                        log.SaveError("VIS_CantSetBothEsc", "");
                        return false;
                    }
                }
            }

            return true;
        }

        /**
	 * 	Get Date Next Run
	 *	@param requery requery
	 *	@return date next run
	 */
    }
}
