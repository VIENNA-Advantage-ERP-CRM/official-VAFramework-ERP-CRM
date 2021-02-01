/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWorkflowProcessorLog
 * Purpose        : 
 * Class Used     : MWorkflowProcessorLog inherits the class X_VAF_WFlowHandlerLog, ViennaProcessorLog
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
using VAdvantage.Utility;

namespace VAdvantage.WF
{
    public class MVAFWFlowHandlerLog : X_VAF_WFlowHandlerLog,ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="VAF_WFlowHandlerLog_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandlerLog(Ctx ctx, int VAF_WFlowHandlerLog_ID, Trx trxName)
            : base(ctx, VAF_WFlowHandlerLog_ID, trxName)
        {
            if (VAF_WFlowHandlerLog_ID == 0)
            {
                SetIsError(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandlerLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="Summary">Summary</param>
        public MVAFWFlowHandlerLog(MWorkflowProcessor parent, String Summary)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            SetClientOrg(parent);
            SetVAF_WFlowHandler_ID(parent.GetVAF_WFlowHandler_ID());
            SetSummary(Summary);
        }
    }
}
