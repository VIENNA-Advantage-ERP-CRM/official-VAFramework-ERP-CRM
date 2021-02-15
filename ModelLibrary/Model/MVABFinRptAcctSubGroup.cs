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
    class MVABFinRptAcctSubGroup : X_VAB_FinRptAcctSubGroup
    {
        public MVABFinRptAcctSubGroup(Ctx ctx, int VAB_FinRptAcctSubGroup_ID, Trx trxName)
            : base(ctx, VAB_FinRptAcctSubGroup_ID, trxName)
        {

        }

        public MVABFinRptAcctSubGroup(Ctx ctx, DataRow dr, Trx trxName)
             : base(ctx, dr, trxName)
         {
         }
    }
}
