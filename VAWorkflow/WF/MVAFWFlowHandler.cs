/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAFWFlowHandler
 * Purpose        : 
 * Class Used     : MVAFWFlowHandler inherits X_VAF_WFlowHandler, ViennaProcessor classes
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
    public class MVAFWFlowHandler : X_VAF_WFlowHandler,ViennaProcessor
    {
        //Static Logger	
        private static VLogger _log	= VLogger.GetVLogger (typeof(MVAFWFlowHandler).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlowHandler_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandler(Ctx ctx, int VAF_WFlowHandler_ID, Trx trxName)
            : base(ctx, VAF_WFlowHandler_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandler(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Active
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>active processors</returns>
        public static MVAFWFlowHandler[] GetActive(Ctx ctx)
        {
            List<MVAFWFlowHandler> list = new List<MVAFWFlowHandler>();
            String sql = "SELECT * FROM VAF_WFlowHandler WHERE IsActive='Y'";
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    list.Add(new MVAFWFlowHandler(ctx, dr, null));
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
            MVAFWFlowHandler[] retValue = new MVAFWFlowHandler[list.Count];
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
            List<MVAFWFlowHandlerLog> list = new List<MVAFWFlowHandlerLog>();
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
                    list.Add(new MVAFWFlowHandlerLog(GetCtx(), dr, Get_TrxName()));
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
            MVAFWFlowHandlerLog[] retValue = new MVAFWFlowHandlerLog[list.Count];
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
