using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountGroupBatch : X_C_AccountGroupBatch
    {
        public MAccountGroupBatch(Ctx ctx, int C_AccountGroupBatch_ID, Trx trxName)
            : base(ctx, C_AccountGroupBatch_ID, trxName)
        {

        }
    }
}
