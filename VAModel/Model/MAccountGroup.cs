using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    class MAccountGroup:X_C_AccountGroup
    {
        public MAccountGroup(Ctx ctx, int C_AccountGroup_ID, Trx trxName)
            : base(ctx, C_AccountGroup_ID, trxName)
        {
        }
    }
}
