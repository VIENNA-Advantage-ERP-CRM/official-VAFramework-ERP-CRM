using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MFinRptConfig : X_VAB_FinRptConfig
    {
        public MFinRptConfig(Ctx ctx, int VAB_FinRptConfig_ID, Trx trxName)
            : base(ctx, VAB_FinRptConfig_ID, trxName)
        {
        }

        public MFinRptConfig(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	
    }
}
