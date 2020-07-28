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
    public class MSchedulerRecipient : X_AD_SchedulerRecipient
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_SchedulerRecipient_ID">scheduler id</param>
        /// <param name="trxName">optional trans name</param>
        public MSchedulerRecipient(Ctx ctx, int AD_SchedulerRecipient_ID,
            Trx trxName)
            : base(ctx, AD_SchedulerRecipient_ID, trxName)
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
