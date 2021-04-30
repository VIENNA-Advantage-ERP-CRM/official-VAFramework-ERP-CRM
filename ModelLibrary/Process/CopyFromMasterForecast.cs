
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Copy lines of One Master forecast to another 
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class CopyFromMasterForecast : SvrProcess
    {
        private int C_MasterForecast_ID = 0;
       
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
                else if (name.Equals("C_MasterForecast_ID"))
                {
                    C_MasterForecast_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Copy Master Forecast Lines 
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override string DoIt()
        {
            int To_MasterForecast_ID = GetRecord_ID();
            log.Info(Msg.GetMsg(GetCtx(), "FromFrorcast") + C_MasterForecast_ID + " " + Msg.GetMsg(GetCtx(), "To") + " " + To_MasterForecast_ID);
            if (To_MasterForecast_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "TargetMasterForecast"));
            }
            if (C_MasterForecast_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "SourceMasterForecast"));
            }
            MMasterForecast from = new MMasterForecast(GetCtx(), C_MasterForecast_ID, Get_Trx());
            MMasterForecast to = new MMasterForecast(GetCtx(), To_MasterForecast_ID, Get_Trx());


            string no = to.CopyLinesFrom(from);

            return "@Copied@=" + no;
        }
    }
}