/********************************************************
 * Module Name    : Scheduler
 * Purpose        : Schedule the Events
 * Author         : Jagmohan Bhatt
 * Date           : 03-Nov-2009
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
    public class MSchedulerRecipient : X_VAF_JobRun_Recipient
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_JobRun_Recipient_ID">scheduler id</param>
        /// <param name="trxName">optional trans name</param>
        public MSchedulerRecipient(Ctx ctx, int VAF_JobRun_Recipient_ID,
            Trx trxName)
            : base(ctx, VAF_JobRun_Recipient_ID, trxName)
        {
            
        }	//	MSchedulerRecipient


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">optional trans name</param>
        public MSchedulerRecipient(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            
        }	//	MSchedulerRecipient
    }
}
