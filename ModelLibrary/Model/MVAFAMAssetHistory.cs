using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using VAdvantage.Utility;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFAMAssetHistory : X_VAFAM_AssetHistory
    {
        public MVAFAMAssetHistory(Ctx ctx, int VAFAM_AssetHistory_ID, Trx trxName)
            : base(ctx, VAFAM_AssetHistory_ID, trxName)
        {
        }
         public MVAFAMAssetHistory(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
