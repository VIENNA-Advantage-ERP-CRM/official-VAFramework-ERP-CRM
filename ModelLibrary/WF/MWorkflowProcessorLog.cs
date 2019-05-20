/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWorkflowProcessorLog
 * Purpose        : 
 * Class Used     : MWorkflowProcessorLog inherits the class X_AD_WorkflowProcessorLog, ViennaProcessorLog
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
    public class MWorkflowProcessorLog : X_AD_WorkflowProcessorLog,ViennaProcessorLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="AD_WorkflowProcessorLog_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWorkflowProcessorLog(Ctx ctx, int AD_WorkflowProcessorLog_ID, Trx trxName)
            : base(ctx, AD_WorkflowProcessorLog_ID, trxName)
        {
            if (AD_WorkflowProcessorLog_ID == 0)
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
        public MWorkflowProcessorLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="Summary">Summary</param>
        public MWorkflowProcessorLog(MWorkflowProcessor parent, String Summary)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            SetClientOrg(parent);
            SetAD_WorkflowProcessor_ID(parent.GetAD_WorkflowProcessor_ID());
            SetSummary(Summary);
        }
    }
}
