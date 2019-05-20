/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctProcessorLog
 * Purpose        : Accounting Processor Log
 * Class Used     : X_C_AcctProcessorLog, ViennaProcessorLog
 * Chronological    Development
 * Raghunandan     07-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAcctProcessorLog : X_C_AcctProcessorLog, ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_AcctProcessorLog_ID"></param>
        /// <param name="trxName"></param>
        public MAcctProcessorLog(Ctx ctx, int C_AcctProcessorLog_ID, Trx trxName)
            : base(ctx, C_AcctProcessorLog_ID, trxName)
        {
            
        }	

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MAcctProcessorLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { 
            
        }	

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="summary"></param>
        public MAcctProcessorLog(MAcctProcessor parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetC_AcctProcessor_ID(parent.GetC_AcctProcessor_ID());
            SetSummary(summary);
        }
    }
}
