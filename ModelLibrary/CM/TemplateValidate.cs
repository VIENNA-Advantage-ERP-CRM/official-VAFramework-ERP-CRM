using System;
using System.Collections.Generic;
//using System.Linq;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.Logging; 
using System.Text;
using VAdvantage.ProcessEngine;

namespace VAdvantage.CM
{
    public class TemplateValidate : ProcessEngine.SvrProcess
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
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
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
