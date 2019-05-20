using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MFinRptAcctSubGroup : X_C_FinRptAcctSubGroup
    {
        public MFinRptAcctSubGroup(Ctx ctx, int C_FinRptAcctSubGroup_ID, Trx trxName)
            : base(ctx, C_FinRptAcctSubGroup_ID, trxName)
        {

        }

        public MFinRptAcctSubGroup(Ctx ctx, DataRow dr, Trx trxName)
             : base(ctx, dr, trxName)
         {
         }
    }
}
