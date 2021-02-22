/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctProcessorLog
 * Purpose        : Accounting Processor Log
 * Class Used     : X_VAB_AccountHanlderLog, ViennaProcessorLog
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
    public class MVABAccountHanlderLog : X_VAB_AccountHanlderLog, ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_AccountHanlderLog_ID"></param>
        /// <param name="trxName"></param>
        public MVABAccountHanlderLog(Ctx ctx, int VAB_AccountHanlderLog_ID, Trx trxName)
            : base(ctx, VAB_AccountHanlderLog_ID, trxName)
        {
            
        }	

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MVABAccountHanlderLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { 
            
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MVABAccountHanlderLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="summary"></param>
        public MVABAccountHanlderLog(MVABAccountHanlder parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAB_AccountHanlder_ID(parent.GetVAB_AccountHanlder_ID());
            SetSummary(summary);
        }
    }
}
