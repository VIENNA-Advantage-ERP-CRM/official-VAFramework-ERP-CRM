using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountSubGroupTrl:X_C_AccountSubGroup_Trl
    {
        public MAccountSubGroupTrl(Ctx ctx,int C_AccountSubGroup_Trl_ID,Trx trxName)
            :base(ctx,C_AccountSubGroup_Trl_ID,trxName)
        {
        }
    }
}
