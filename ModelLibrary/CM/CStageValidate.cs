using System;
using System.Collections.Generic;
//using System.Linq;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Text;
using VAdvantage.ProcessEngine;

namespace VAdvantage.CM
{
    public class CStageValidate : ProcessEngine.SvrProcess
    {
        private int p_CM_CStage_ID = 0;

        /**
         *  Prepare - e.g., get Parameters.
         */
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_CM_CStage_ID = GetRecord_ID();
        }	//	prepare

        /**
         * 	Process
         *	@return info
         *	@throws Exception
         */
        protected override String DoIt()
        {
            log.Info("CM_CStage_ID=" + p_CM_CStage_ID);
            MCStage stage = new MCStage(GetCtx(), p_CM_CStage_ID, Get_Trx());
            return stage.Validate();
        }	//	doIt

    }
}
