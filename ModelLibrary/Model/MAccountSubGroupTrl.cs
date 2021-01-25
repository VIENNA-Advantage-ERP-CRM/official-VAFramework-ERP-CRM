using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MAccountSubGroupTrl:X_VAB_AccountSubGroup_TL
    {
        public MAccountSubGroupTrl(Ctx ctx,int VAB_AccountSubGroup_TL_ID,Trx trxName)
            :base(ctx,VAB_AccountSubGroup_TL_ID,trxName)
        {
        }
    }
}
