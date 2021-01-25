/********************************************************
 * Module Name    : 
 * Purpose        : Commission Line Model
 * Class Used     : X_VAB_WorkCommissionLine
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
    public class MCommissionLine : X_VAB_WorkCommissionLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_WorkCommissionLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCommissionLine(Ctx ctx, int VAB_WorkCommissionLine_ID, Trx trxName)
            : base(ctx, VAB_WorkCommissionLine_ID, trxName)
        {
            if (VAB_WorkCommissionLine_ID == 0)
            {
                //	SetVAB_WorkCommission_ID (0);
                SetLine(0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_WorkCommissionLine WHERE VAB_WorkCommission_ID=@VAB_WorkCommission_ID@
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
