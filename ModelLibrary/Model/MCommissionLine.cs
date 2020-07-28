/********************************************************
 * Module Name    : 
 * Purpose        : Commission Line Model
 * Class Used     : X_C_CommissionLine
 * Chronological    Development
 * Veena        09-Nov-2009
**********************************************************/

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
    /// <summary>
    /// Commission Line Model
    /// </summary>
    public class MCommissionLine : X_C_CommissionLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_CommissionLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCommissionLine(Ctx ctx, int C_CommissionLine_ID, Trx trxName)
            : base(ctx, C_CommissionLine_ID, trxName)
        {
            if (C_CommissionLine_ID == 0)
            {
                //	SetC_Commission_ID (0);
                SetLine(0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_CommissionLine WHERE C_Commission_ID=@C_Commission_ID@
                SetAmtMultiplier(Env.ZERO);
                SetAmtSubtract(Env.ZERO);
                SetCommissionOrders(false);
                SetIsPositiveOnly(false);
                SetQtyMultiplier(Env.ZERO);
                SetQtySubtract(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCommissionLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
