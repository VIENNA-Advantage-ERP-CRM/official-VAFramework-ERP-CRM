using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Logging;

using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MPhase : X_C_Phase 
    {
        public MPhase(Ctx ctx, int C_Phase_ID, Trx trxName)
            : base(ctx, C_Phase_ID, trxName)
        {
            
        }

       public MPhase(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
