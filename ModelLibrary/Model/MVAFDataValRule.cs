/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_DataVal_Rule
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
    public class MVAFDataValRule : X_VAF_DataVal_Rule
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_DataVal_Rule_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFDataValRule(Context ctx, int VAF_DataVal_Rule_ID, Trx trxName)
            : base(ctx, VAF_DataVal_Rule_ID, trxName)
        {
        }

        public MVAFDataValRule(Ctx ctx, int VAF_DataVal_Rule_ID, Trx trxName)
            : base(ctx, VAF_DataVal_Rule_ID, trxName)
        {
        }



        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAFDataValRule(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MVAFDataValRule(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAFDataValRule[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
