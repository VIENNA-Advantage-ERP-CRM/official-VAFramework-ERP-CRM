using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MVABCurrencySource:X_VAB_CurrencySource
    {
        public MVABCurrencySource(Ctx ctx, int VAB_CurrencySource_ID, Trx trx)
            : base(ctx, VAB_CurrencySource_ID, trx)
        {
        }
        public MVABCurrencySource(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }
    }
}
