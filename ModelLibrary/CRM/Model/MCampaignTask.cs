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
    public class MCampaignTask : X_VAB_PromotionTask
    {
         public MCampaignTask(Ctx ctx, int VAB_PromotionTask_ID, Trx trxName)
            : base(ctx, VAB_PromotionTask_ID, trxName)
        {
            
        }

         public MCampaignTask(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
