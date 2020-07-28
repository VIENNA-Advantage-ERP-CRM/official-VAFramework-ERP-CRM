using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    class MElementValueTemp:X_VACTWZ_ELEMENTVALUE
    {

          /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Activity_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MElementValueTemp(Ctx ctx, int C_ElementValueTemp, Trx trxName)
            : base(ctx, C_ElementValueTemp, trxName)
        {
            //super(ctx, C_Activity_ID, trxName);
        }
    }
}
