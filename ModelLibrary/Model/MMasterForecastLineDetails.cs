
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    MasterForecast LineDetails
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMasterForecastLineDetails:X_C_MasterForecastLineDetails
    {
        private static VLogger log = VLogger.GetVLogger(typeof(MMasterForecastLineDetails).FullName);

        public MMasterForecastLineDetails(Ctx ctx, int C_MasterForecastLineDetails_ID, Trx trxName) :
           base(ctx, C_MasterForecastLineDetails_ID, null)
        {

        }

        public MMasterForecastLineDetails(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">New record</param>
        /// <param name="success">Success</param>
        /// <returns>true/false</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //update Amounts at master forecast line  
            string _sql = "UPDATE C_MasterForecastLine SET " +
            "Price= (SELECT NVL(SUM(TotaAmt),0)/ NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "),"+
            "PlannedRevenue =(SELECT SUM(TotaAmt) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")" +
            " WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID();

            if (DB.ExecuteQuery(_sql, null, Get_Trx()) < 0)
            {
                log.SaveError("ForecastLineNotUpdated", "");
                return false;
            }
            return true;
        }

    }
}
