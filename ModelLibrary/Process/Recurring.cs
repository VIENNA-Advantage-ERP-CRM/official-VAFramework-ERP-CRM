/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Recurring
 * Purpose        : Recurring process
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class Recurring : ProcessEngine.SvrProcess
    {
        /// <summary>
        /// Prepare - e.g., get Parameters.
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
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MRecurring rec = new MRecurring(GetCtx(), GetRecord_ID(), Get_TrxName());
            log.Info(rec.ToString());
            return rec.ExecuteRun();
        }	//	doIt

    }	//	Recurring
}
