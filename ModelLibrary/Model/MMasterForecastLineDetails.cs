
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

        protected override bool AfterSave(bool newRecord, bool success)
        {            
            return true;
        }

    }
}
