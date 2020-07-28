/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WorkflowValidate
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      06-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;

namespace VAdvantage.WF
{
    public class WorkflowValidate : ProcessEngine.SvrProcess
    {
        private int p_AD_Worlflow_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            p_AD_Worlflow_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            MWorkflow WF = MWorkflow.Get(GetCtx(), p_AD_Worlflow_ID);
            log.Info("WF=" + WF);
            String msg = WF.Validate();
            WF.Save();
            if (msg.Length > 0)
            {
                throw new Exception(Utility.Msg.ParseTranslation(GetCtx(), "WorflowNotValid") + " - " + msg);
            }
            return WF.IsValid() ? "@OK@" : "@Error@";
        }

    }
}
