/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RfQResponseCComplete
 * Purpose        : Check if Response is Complete
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     11-Aug.-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RfQResponseCComplete : ProcessEngine.SvrProcess
    {
        //RfQ Response				
        private int _C_RfQResponse_ID = 0;

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
            _C_RfQResponse_ID = GetRecord_ID();
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MRfQResponse response = new MRfQResponse(GetCtx(), _C_RfQResponse_ID, Get_TrxName());
            log.Info("doIt - " + response);
            //
            String error = response.CheckComplete();
            if (error != null && error.Length > 0)
            {
                throw new Exception(error);
            }
            //
            response.Save();
            return "OK";
        }
    }
}
