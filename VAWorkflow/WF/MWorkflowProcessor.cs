/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWorkflowProcessor
 * Purpose        : 
 * Class Used     : MWorkflowProcessor inherits X_VAF_WFlowHandler, ViennaProcessor classes
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
    public class MWorkflowProcessor : X_VAF_WFlowHandler,ViennaProcessor
    {
        //Static Logger	
        private static VLogger _log	= VLogger.GetVLogger (typeof(MWorkflowProcessor).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlowHandler_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWorkflowProcessor(Ctx ctx, int VAF_WFlowHandler_ID, Trx trxName)
            : base(ctx, VAF_WFlowHandler_ID, trxName)
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
            String sql = "SELECT * FROM VAF_WFlowHandler WHERE IsActive='Y'";
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
                + "FROM VAF_WFlowHandlerLog "
                + "WHERE VAF_WFlowHandler_ID=" + GetVAF_WFlowHandler_ID() + "ORDER BY Created DESC";
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
            String sql = "DELETE FROM VAF_WFlowHandlerLog "
                + "WHERE VAF_WFlowHandler_ID=" + GetVAF_WFlowHandler_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            int no = DB.ExecuteQuery(sql);
            return no;
        }
        
        /**
	 * 	Get Date Next Run
	 *	@param requery requery
	 *	@return date next run
	 */
	
    }
}
