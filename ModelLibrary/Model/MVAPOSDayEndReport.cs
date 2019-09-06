using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViennaAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace ViennaAdvantage.Model
{
    public class MVAPOSDayEndReport : X_VAPOS_DayEndReport
    {
        public MVAPOSDayEndReport(Ctx ctx, int VAPOS_DayEndReport_ID, Trx trxName)
            : base(ctx, VAPOS_DayEndReport_ID, trxName)
        {

        }
        public MVAPOSDayEndReport(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }
    }
}