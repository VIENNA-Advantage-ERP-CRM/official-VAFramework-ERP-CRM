/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RfQClose
 * Purpose        : Close RfQ and Responses
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
  ******************************************************/
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
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RfQClose : ProcessEngine.SvrProcess
    {
        //RfQ
        private int _C_RfQ_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
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
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            _C_RfQ_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// VFramwork.Process.SvrProcess#doIt()
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MRfQ rfq = new MRfQ(GetCtx(), _C_RfQ_ID, Get_TrxName());
            if (rfq.Get_ID() == 0)
            {
                throw new ArgumentException("No RfQ found");
            }
            log.Info("doIt - " + rfq);
            rfq.SetProcessed(true);
            rfq.Save();
            //
            int counter = 0;
            MRfQResponse[] responses = rfq.GetResponses(false, false);
            for (int i = 0; i < responses.Length; i++)
            {
                responses[i].SetProcessed(false);
                responses[i].Save();
                counter++;
            }
            //
            return "# " + counter;
        }
    }
}
