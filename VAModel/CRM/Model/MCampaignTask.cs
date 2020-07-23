using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MCampaignTask : X_C_CampaignTask
    {
         public MCampaignTask(Ctx ctx, int C_CampaignTask_ID, Trx trxName)
            : base(ctx, C_CampaignTask_ID, trxName)
        {
            
        }

         public MCampaignTask(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
