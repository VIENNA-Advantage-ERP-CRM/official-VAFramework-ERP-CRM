/********************************************************
* Project Name   : VAdvantage
* Class Name     : RecurringMenu
* Purpose        : Recurring process
* Class Used     : ProcessEngine.SvrProcess
* Chronological    Development
* Amit Bansal      03-nov-2016
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
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    class RecurringMenu : SvrProcess
    {

        int _C_Recurring_ID = 0;
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
                else if (name.Equals("C_Recurring_ID"))
                {
                    _C_Recurring_ID = para[i].GetParameterAsInt();
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
            if (_C_Recurring_ID > 0)
            {
                MRecurring rec = new MRecurring(GetCtx(), _C_Recurring_ID, Get_TrxName());
                log.Info(rec.ToString());
                return rec.ExecuteRun();
            }
            else
            {
                return "Select Recurring";
            }
        }	//	doIt
    }
}
