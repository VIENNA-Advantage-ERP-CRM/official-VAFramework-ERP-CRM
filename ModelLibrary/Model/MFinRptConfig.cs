using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MFinRptConfig : X_C_FinRptConfig
    {
        public MFinRptConfig(Ctx ctx, int C_FinRptConfig_ID, Trx trxName)
            : base(ctx, C_FinRptConfig_ID, trxName)
        {
        }

        public MFinRptConfig(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	
    }
}
