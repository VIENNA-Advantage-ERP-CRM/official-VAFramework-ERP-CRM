using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MVABAccountSubGroup:X_VAB_AccountSubGroup
    {
        public MVABAccountSubGroup(Ctx ctx, int VAB_AccountSubGroup_ID, Trx trxName)
            : base(ctx, VAB_AccountSubGroup_ID, trxName)
        {
        }
    }
}
