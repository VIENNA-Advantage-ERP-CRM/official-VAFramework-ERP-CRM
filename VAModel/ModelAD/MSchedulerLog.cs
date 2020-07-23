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
    public class MSchedulerLog : X_AD_SchedulerLog, ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="summary">summary</param>
        public MSchedulerLog(Ctx ctx, int AD_SchedulerLog_ID, Trx trxName)
            : base(ctx, AD_SchedulerLog_ID, trxName)
        {

            if (AD_SchedulerLog_ID == 0)
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
        public MSchedulerLog(MScheduler parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_Scheduler_ID(parent.GetAD_Scheduler_ID());
            SetSummary(summary);
        }	//	MSchedulerLog

    }
}
