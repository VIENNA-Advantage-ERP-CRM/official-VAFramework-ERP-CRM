/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_ShippingMethod
 * Chronological Development
 * Veena Pandey     14-Sept-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAMShippingMethod : X_VAM_ShippingMethod
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ShippingMethod_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMShippingMethod(Ctx ctx, int VAM_ShippingMethod_ID, Trx trxName)
            : base(ctx, VAM_ShippingMethod_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAMShippingMethod(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
