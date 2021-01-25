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
        /// <param name="VAB_BillingCode_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MElementValueTemp(Ctx ctx, int VAB_Acct_ElementValueTmp, Trx trxName)
            : base(ctx, VAB_Acct_ElementValueTmp, trxName)
        {
            //super(ctx, VAB_BillingCode_ID, trxName);
        }
    }
}
