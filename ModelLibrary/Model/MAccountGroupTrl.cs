using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountGroupTrl:X_VAB_AccountGroup_TL
    {
        public MAccountGroupTrl(Ctx ctx, int X_VAB_AccountGroup_TL_ID, Trx trxName)
            : base(ctx, X_VAB_AccountGroup_TL_ID, trxName)
        { }
    }
}
