/********************************************************
 * Module Name    : 
 * Purpose        : Commission Run Amount Detail Model
 * Class Used     : X_VAB_WorkCommissionDetail
 * Chronological    Development
 * Veena        09-Nov-2009
**********************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Commission Run Amount Detail Model
    /// </summary>
    public class MCommissionDetail : X_VAB_WorkCommissionDetail
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MCommissionDetail(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCommissionDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="camt">commission amt</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="amt">amount</param>
        /// <param name="qty">quantity</param>
        public MCommissionDetail(MCommissionAmt camt, int VAB_Currency_ID, Decimal amt, Decimal qty)
            : this(camt.GetCtx(), 0, camt.Get_TrxName())
	    {
            SetClientOrg(camt);
            SetVAB_WorkCommission_Amt_ID(camt.GetVAB_WorkCommission_Amt_ID());
            SetVAB_Currency_ID(VAB_Currency_ID);
            SetActualAmt(amt);
            SetActualQty(qty);
            SetConvertedAmt(Env.ZERO);
	    }

        /// <summary>
        /// Set Line IDs
        /// </summary>
        /// <param name="C_OrderLine_ID">order</param>
        /// <param name="VAB_InvoiceLine_ID">invoice</param>
        public void SetLineIDs(int C_OrderLine_ID, int VAB_InvoiceLine_ID)
        {
            if (C_OrderLine_ID != 0)
                SetC_OrderLine_ID(C_OrderLine_ID);
            if (VAB_InvoiceLine_ID != 0)
                SetVAB_InvoiceLine_ID(VAB_InvoiceLine_ID);
        }


        /// <summary>
        /// Set Converted Amt
        /// </summary>
        /// <param name="date">date for conversion</param>
        public void SetConvertedAmt(DateTime? date)
        {
            Decimal amt = MConversionRate.ConvertBase(GetCtx(),
                GetActualAmt(), GetVAB_Currency_ID(), date, 0, 	//	type
                GetVAF_Client_ID(), GetVAF_Org_ID());
            //if (amt != null)
                SetConvertedAmt(amt);
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!newRecord)
                UpdateAmtHeader();
            return success;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override Boolean AfterDelete(Boolean success)
        {
            if (success)
                UpdateAmtHeader();
            return success;
        }

        /// <summary>
        /// Update Amt Header
        /// </summary>
        private void UpdateAmtHeader()
        {
            MCommissionAmt amt = new MCommissionAmt(GetCtx(), GetVAB_WorkCommission_Amt_ID(), Get_TrxName());
            amt.CalculateCommission();
            amt.Save();
        }
    }
}
