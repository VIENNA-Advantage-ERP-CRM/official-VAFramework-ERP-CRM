using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MVABAccountGroupBatch : X_VAB_AccountGroupBatch
    {
        public MVABAccountGroupBatch(Ctx ctx, int VAB_AccountGroupBatch_ID, Trx trxName)
            : base(ctx, VAB_AccountGroupBatch_ID, trxName)
        {

        }
    }
}
