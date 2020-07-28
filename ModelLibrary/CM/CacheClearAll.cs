/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Deepak
 * Date           : 16 Feb 2010
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;

namespace VAdvantage.CM
{
    public class CacheClearAll : ProcessEngine.SvrProcess
    {
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
                //else if (name.equals("CM_WebProject_ID"))
                //p_CM_WebProject_ID = ((BigDecimal)para[i].getParameter()).intValue();
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        } // prepare


        protected override String DoIt()
        {
            MTemplate thisTemplate = new MTemplate(GetCtx(), GetRecord_ID(), Get_Trx());
            thisTemplate.SetIsValid(true);
            thisTemplate.Save();
            return null;
        }

    }
}
