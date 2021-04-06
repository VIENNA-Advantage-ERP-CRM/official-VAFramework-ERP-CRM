/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_ContextScope
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
    public class MVAFContextScope : X_VAF_ContextScope
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_ContextScope_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFContextScope(Context ctx, int VAF_ContextScope_ID, Trx trxName)
            : base(ctx, VAF_ContextScope_ID, trxName)
        {
            if (VAF_ContextScope_ID == 0)
            {
                SetRecordType(RecordType_UserMaintained); // U
                SetIsSOTrx(false);
            }
        }


        public MVAFContextScope(Ctx ctx, int VAF_ContextScope_ID, Trx trxName)
            : base(ctx, VAF_ContextScope_ID, trxName)
        {
            if (VAF_ContextScope_ID == 0)
            {
                SetRecordType(RecordType_UserMaintained); // U
                SetIsSOTrx(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAFContextScope(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <param name="trxName">transaction</param>
        public MVAFContextScope(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCtxArea[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
