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
    public class CopyFromForecast : SvrProcess
    {
        private int C_Forecast_ID = 0;
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
                else if (name.Equals("C_Forecast_ID") )
                {
                    C_Forecast_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Copy Team Forecast Lines 
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override string DoIt()
        {
            int To_Forecast_ID = GetRecord_ID();
            log.Info(Msg.GetMsg(GetCtx(),"FromFrorcast") + C_Forecast_ID +" "+ Msg.GetMsg(GetCtx(), "To") +" "+ To_Forecast_ID);
            if (To_Forecast_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(),"TargetForecastID"));
            }
            if (C_Forecast_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "SourceForecast_ID"));
            }
            MForecast from =  new MForecast(GetCtx(), C_Forecast_ID, Get_Trx());
            MForecast to = new MForecast(GetCtx(), To_Forecast_ID, Get_Trx());

        
            string no = to.CopyLinesFrom(from);       

            return "@Copied@="+ no;
        }
    }
}
