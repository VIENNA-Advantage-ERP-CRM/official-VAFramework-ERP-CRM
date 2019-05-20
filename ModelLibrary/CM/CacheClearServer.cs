/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WebProjectDeploy
 * Purpose        : Deploy Web Project
 * Class Used     : X_CM_Container
 * Chronological    Development
 * Deepak           12-Feb-2010
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

namespace VAdvantage.CM
{
    public class CacheClearServer : ProcessEngine.SvrProcess
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
