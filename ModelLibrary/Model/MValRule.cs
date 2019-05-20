/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Val_Rule
 * Chronological Development
 * Veena Pandey     29-Aug-09
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
    public class MValRule : X_AD_Val_Rule
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Val_Rule_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MValRule(Context ctx, int AD_Val_Rule_ID, Trx trxName)
            : base(ctx, AD_Val_Rule_ID, trxName)
        {
        }

        public MValRule(Ctx ctx, int AD_Val_Rule_ID, Trx trxName)
            : base(ctx, AD_Val_Rule_ID, trxName)
        {
        }



        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MValRule(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MValRule(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MValRule[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
