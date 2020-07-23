/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_Shipper
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
    public class MShipper : X_M_Shipper
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Shipper_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MShipper(Ctx ctx, int M_Shipper_ID, Trx trxName)
            : base(ctx, M_Shipper_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MShipper(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
