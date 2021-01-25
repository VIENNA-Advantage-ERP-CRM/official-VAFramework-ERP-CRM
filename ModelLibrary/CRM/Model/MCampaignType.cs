using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

using VAdvantage.Model;

namespace VAdvantage.Model
{
    public class MCampaignType : X_VAB_PromotionType
    {

        public MCampaignType(Ctx ctx, int VAB_PromotionType_ID, Trx trxName)
            : base(ctx, VAB_PromotionType_ID, trxName)
        {
            
        }


        public MCampaignType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

    }

    
}
