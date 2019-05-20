using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountGroupTrl:X_C_AccountGroup_Trl
    {
        public MAccountGroupTrl(Ctx ctx, int X_C_AccountGroup_Trl_ID, Trx trxName)
            : base(ctx, X_C_AccountGroup_Trl_ID, trxName)
        { }
    }
}
