using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MForecastLineHistory : X_C_ForecastLineHistory
    {
        public MForecastLineHistory(Ctx ctx, int C_ForecastLineHistory_ID, Trx trxName)
  : base(ctx, C_ForecastLineHistory_ID, trxName)
        {
        }

        public MForecastLineHistory(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

    }
}
