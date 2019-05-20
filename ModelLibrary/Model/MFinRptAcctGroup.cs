using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MFinRptAcctGroup : X_C_FinRptAcctGroup
    {
        public MFinRptAcctGroup(Ctx ctx, int C_FinRptAcctGroup_ID, Trx trxName)
            : base(ctx, C_FinRptAcctGroup_ID, trxName)
        {

        }

        public MFinRptAcctGroup(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	
    }
}
