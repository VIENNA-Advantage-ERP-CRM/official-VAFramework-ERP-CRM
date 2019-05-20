/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_FieldGroup
 * Chronological Development
 * Veena Pandey     31-Aug-09
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
    public class MFieldGroup : X_AD_FieldGroup
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_FieldGroup_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MFieldGroup(Context ctx, int AD_FieldGroup_ID, Trx trxName)
            : base(ctx, AD_FieldGroup_ID, trxName)
        {
        }


        public MFieldGroup(Ctx ctx, int AD_FieldGroup_ID, Trx trxName)
            : base(ctx, AD_FieldGroup_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MFieldGroup(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MFieldGroup(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MFieldGroup[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
