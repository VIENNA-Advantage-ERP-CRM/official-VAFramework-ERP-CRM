using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MSalesOffice : X_C_SalesOffice
    {
        public MSalesOffice(Ctx ctx, int C_SalesOffice_ID, Trx trxName)
            : base(ctx, C_SalesOffice_ID, trxName)
        {

        }

        public MSalesOffice(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
    }
}
