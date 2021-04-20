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
                else if (name.Equals("C_Forecast_ID"))
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
            log.Info(Msg.GetMsg(GetCtx(), "FromFrorcast") + C_MasterForecast_ID + " to " + To_MasterForecast_ID);
            if (To_MasterForecast_ID == 0)
            {
                throw new ArgumentException("Target To_MasterForecast_ID == 0");
            }
            if (C_MasterForecast_ID == 0)
            {
                throw new ArgumentException("Source C_MasterForecast_ID == 0");
            }
            MMasterForecast from = new MMasterForecast(GetCtx(), C_MasterForecast_ID, Get_Trx());
            MMasterForecast to = new MMasterForecast(GetCtx(), To_MasterForecast_ID, Get_Trx());


            string no = to.CopyLinesFrom(from);

            return "@Copied@=" + no;
        }
    }
}