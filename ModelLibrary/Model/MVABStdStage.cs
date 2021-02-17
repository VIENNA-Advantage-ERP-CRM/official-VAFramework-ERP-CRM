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
    public class MVABStdStage : X_VAB_Std_Stage 
    {
        public MVABStdStage(Ctx ctx, int VAB_Std_Stage_ID, Trx trxName)
            : base(ctx, VAB_Std_Stage_ID, trxName)
        {
            
        }

       public MVABStdStage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
