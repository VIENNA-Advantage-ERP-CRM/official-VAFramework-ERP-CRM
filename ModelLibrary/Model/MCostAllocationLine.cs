using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MCostAllocationLine:X_M_CostAllocationLine
    {
        public MCostAllocationLine(Ctx ctx, int M_CostAllocationLine_ID, Trx trx) : base(ctx, M_CostAllocationLine_ID, trx) { }
        public MCostAllocationLine(Ctx ctx, DataRow dr, Trx trx) : base(ctx, dr, trx) { }

    }
}
