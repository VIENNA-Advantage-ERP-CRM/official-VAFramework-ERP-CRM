/******************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRequestProcessorLog
 * Purpose        : Request Processor Log
 * Class Used     : X_R_RequestProcessorLog,ViennaProcessorLog
 * Chronological    Development
 * Raghunandan      21-Jan-2010
  *****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Threading;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MRequestProcessorLog : X_R_RequestProcessorLog, ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="R_RequestProcessorLog_ID"></param>
        /// <param name="trxName"></param>
        public MRequestProcessorLog(Ctx ctx, int R_RequestProcessorLog_ID, Trx trxName)
            : base(ctx, R_RequestProcessorLog_ID, trxName)
        {

            if (R_RequestProcessorLog_ID == 0)
            {
                SetIsError(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MRequestProcessorLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// 	Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="summary"></param>
        public MRequestProcessorLog(MRequestProcessor parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetR_RequestProcessor_ID(parent.GetR_RequestProcessor_ID());
            SetSummary(summary);
        }
    }
}
