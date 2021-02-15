using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MVABFinRptAcctGroup : X_VAB_FinRptAcctGroup
    {
        public MVABFinRptAcctGroup(Ctx ctx, int VAB_FinRptAcctGroup_ID, Trx trxName)
            : base(ctx, VAB_FinRptAcctGroup_ID, trxName)
        {

        }

        public MVABFinRptAcctGroup(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	
    }
}
