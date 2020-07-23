using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountSubGroup:X_C_AccountSubGroup
    {
        public MAccountSubGroup(Ctx ctx, int C_AccountSubGroup_ID, Trx trxName)
            : base(ctx, C_AccountSubGroup_ID, trxName)
        {
        }
    }
}
