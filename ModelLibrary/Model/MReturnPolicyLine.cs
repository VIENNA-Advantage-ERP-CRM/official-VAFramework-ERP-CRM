/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_ReturnPolicyLine
 * Chronological Development
 * Veena Pandey     19-June-2009
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
    public class MReturnPolicyLine : X_M_ReturnPolicyLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_ReturnPolicyLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReturnPolicyLine(Ctx ctx, int M_ReturnPolicyLine_ID, Trx trxName)
            : base(ctx, M_ReturnPolicyLine_ID, trxName)
        {
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MReturnPolicyLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}