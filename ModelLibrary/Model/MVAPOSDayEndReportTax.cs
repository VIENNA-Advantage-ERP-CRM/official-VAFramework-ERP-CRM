using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViennaAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;


namespace ViennaAdvantageWeb.Models
{
    public class MVAPOSDayEndReportTax :X_VAPOS_DayEndReportTax
    {
           public MVAPOSDayEndReportTax(Ctx ctx, int VAPOS_DayEndReportTax_ID, Trx trxName)
            : base(ctx, VAPOS_DayEndReportTax_ID, trxName)
        {

        }
           public MVAPOSDayEndReportTax(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }
    }
}