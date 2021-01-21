/********************************************************
 * Module Name    : Scheduler
 * Purpose        : Schedule the Events
 * Author         : Jagmohan Bhatt
 * Date           : 04-Nov-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class MSchedulerLog : X_VAF_JobRun_Log, ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="summary">summary</param>
        public MSchedulerLog(Ctx ctx, int VAF_JobRun_Log_ID, Trx trxName)
            : base(ctx, VAF_JobRun_Log_ID, trxName)
        {

            if (VAF_JobRun_Log_ID == 0)
                SetIsError(false);
        }	//	MSchedulerLog


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">data row</param>
        /// <param name="trxName">optional trans name</param>
        public MSchedulerLog(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            
        }	//	MSchedulerLog


        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="summary">summary</param>
        public MSchedulerLog(X_VAF_JobRun_Plan parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_JobRun_Plan_ID(parent.GetVAF_JobRun_Plan_ID());
            SetSummary(summary);
        }	//	MSchedulerLog

    }
}
