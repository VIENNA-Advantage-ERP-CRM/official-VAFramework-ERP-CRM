using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABFinRptConfig : X_VAB_FinRptConfig
    {
        public MVABFinRptConfig(Ctx ctx, int VAB_FinRptConfig_ID, Trx trxName)
            : base(ctx, VAB_FinRptConfig_ID, trxName)
        {
        }

        public MVABFinRptConfig(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	
    }
}
