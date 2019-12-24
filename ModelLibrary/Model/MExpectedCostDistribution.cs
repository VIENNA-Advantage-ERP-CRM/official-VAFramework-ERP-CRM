/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_ExpectedCostDistribution
 * Chronological Development
 * Amit Bansal     04-Dec-2019
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MExpectedCostDistribution : X_C_ExpectedCostDistribution
    {
        #region Variable
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MExpectedCost).FullName);
        #endregion


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ExpectedCostDistribution_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MExpectedCostDistribution(Ctx ctx, int C_ExpectedCostDistribution_ID, Trx trxName)
           : base(ctx, C_ExpectedCostDistribution_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MExpectedCostDistribution(Ctx ctx, DataRow dr, Trx trxName)
           : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Set Amt
        /// </summary>
        /// <param name="Amt">Amount</param>
        /// <param name="precision">precision</param>
        public void SetAmt(Decimal Amt, int precision)
        {
            if (Env.Scale(Amt) > precision)
            {
                Amt = Decimal.Round(Amt, precision, MidpointRounding.AwayFromZero);
            }
            base.SetAmt(Amt);
        }
    }
}
