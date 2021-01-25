using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MCurrencySource:X_VAB_CurrencySource
    {
        public MCurrencySource(Ctx ctx, int VAB_CurrencySource_ID, Trx trx)
            : base(ctx, VAB_CurrencySource_ID, trx)
        {
        }
        public MCurrencySource(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }
    }
}
