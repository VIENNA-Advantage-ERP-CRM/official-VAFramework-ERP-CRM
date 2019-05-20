/********************************************************
 * Module Name    : 
 * Purpose        : Commission Run Amounts
 * Class Used     : X_C_CommissionAmt
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
    /// Commission Run Amounts
    /// </summary>
    public class MCommissionAmt : X_C_CommissionAmt
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_CommissionAmt_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCommissionAmt(Ctx ctx, int C_CommissionAmt_ID, Trx trxName)
            : base(ctx, C_CommissionAmt_ID, trxName)
        {
            if (C_CommissionAmt_ID == 0)
            {
                //	SetC_CommissionRun_ID (0);
                //	SetC_CommissionLine_ID (0);
                SetActualQty(Env.ZERO);
                SetCommissionAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCommissionAmt(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="run">parent</param>
        /// <param name="C_CommissionLine_ID">line</param>
        public MCommissionAmt(MCommissionRun run, int C_CommissionLine_ID)
            : this(run.GetCtx(), 0, run.Get_TrxName())
        {
            SetClientOrg(run);
            SetC_CommissionRun_ID(run.GetC_CommissionRun_ID());
            SetC_CommissionLine_ID(C_CommissionLine_ID);
        }

        /// <summary>
        /// Get Details
        /// </summary>
        /// <returns>array of details</returns>
        public MCommissionDetail[] GetDetails()
        {
            String sql = "SELECT * FROM C_CommissionDetail WHERE C_CommissionAmt_ID=@camtid";
            List<MCommissionDetail> list = new List<MCommissionDetail>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@camtid", GetC_CommissionAmt_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MCommissionDetail(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //	Convert
            MCommissionDetail[] retValue = new MCommissionDetail[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Calculate Commission
        /// </summary>
        public void CalculateCommission()
        {
            MCommissionDetail[] details = GetDetails();
            Decimal convertedAmt = Env.ZERO;
            Decimal actualQty = Env.ZERO;
            for (int i = 0; i < details.Length; i++)
            {
                MCommissionDetail detail = details[i];
                Decimal amt1 = detail.GetConvertedAmt();
                //if (amt1 == null)
                //    amt1 = Env.ZERO;
                convertedAmt = Decimal.Add(convertedAmt, amt1);
                actualQty = Decimal.Add(actualQty, detail.GetActualQty());
            }
            SetConvertedAmt(convertedAmt);
            SetActualQty(actualQty);
            //
            MCommissionLine cl = new MCommissionLine(GetCtx(), GetC_CommissionLine_ID(), Get_TrxName());
            //	Qty
            Decimal qty = Decimal.Subtract(GetActualQty(), cl.GetQtySubtract());
            if (cl.IsPositiveOnly() && Env.Signum(qty) < 0)
                qty = Env.ZERO;
            qty = Decimal.Multiply(qty, cl.GetQtyMultiplier());
            //	Amt
            Decimal amt = Decimal.Subtract(GetConvertedAmt(), cl.GetAmtSubtract());
            if (cl.IsPositiveOnly() && Env.Signum(amt) < 0)
                amt = Env.ZERO;
            amt = Decimal.Multiply(amt, cl.GetAmtMultiplier());
            //
            SetCommissionAmt(Decimal.Add(amt, qty));
        }


        public void CalculatecommissionwithNewLogic()
        {
            MCommissionDetail[] details = GetDetails();
            if (details.Length > 0)
            {
                Decimal convertedAmt = Env.ZERO;
                Decimal actualQty = Env.ZERO;
                Decimal commCalcAmt = Env.ZERO;
                Decimal TotalCommissionAmt = Env.ZERO;
                for (int i = 0; i < details.Length; i++)
                {
                    MCommissionDetail detail = details[i];
                    convertedAmt = Decimal.Add(convertedAmt, detail.GetConvertedAmt());
                    actualQty = Decimal.Add(actualQty, detail.GetActualQty());
                    MCommissionAmt camt = new MCommissionAmt(GetCtx(), detail.GetC_CommissionAmt_ID(), Get_TrxName());
                    MCommissionLine line = new MCommissionLine(GetCtx(), camt.GetC_CommissionLine_ID(), Get_TrxName());

                    SetConvertedAmt(convertedAmt);
                    SetActualQty(actualQty);

                    if (line.GetC_CommissionType() == "A")
                    {
                        if (line.IsPositiveOnly() && Env.Signum(detail.GetConvertedAmt()) < 0)
                            commCalcAmt = Env.ZERO;

                        if (detail.GetConvertedAmt() >= line.GetC_TargetAmount())
                        {
                            commCalcAmt = CalculateCommisionAmt(detail.GetConvertedAmt(), line.GetC_TargetPercentage());
                        }
                        else if (detail.GetConvertedAmt() < line.GetC_TargetAmount() && detail.GetConvertedAmt() >= line.GetC_ThresholdAmount())
                        {
                            commCalcAmt = CalculateCommisionAmt(detail.GetConvertedAmt(), line.GetC_ThresholdPercentage());
                        }
                        else
                        {
                            commCalcAmt = 0;
                        }
                    }
                    else if (line.GetC_CommissionType() == "Q")
                    {
                        if (line.IsPositiveOnly() && Env.Signum(detail.GetActualQty()) < 0)
                            actualQty = Env.ZERO;

                        if (detail.GetActualQty() >= line.GetC_TargetQuantity())
                        {
                            commCalcAmt = CalculateCommisionAmt(detail.GetConvertedAmt(), line.GetC_TargetPercentage());
                        }
                        else if (detail.GetActualQty() < line.GetC_TargetQuantity() && detail.GetActualQty() >= line.GetC_ThresholdQuantity())
                        {
                            commCalcAmt = CalculateCommisionAmt(detail.GetConvertedAmt(), line.GetC_ThresholdPercentage());
                        }
                        else
                        {
                            commCalcAmt = 0;
                        }
                    }
                    TotalCommissionAmt = TotalCommissionAmt + commCalcAmt;
                }
                SetCommissionAmt(TotalCommissionAmt);
            }
        }

        public decimal CalculateCommisionAmt(decimal Ammount, decimal percentage) {
             //percentComplete = Decimal.Divide(percentage, Ammount) * 100;
             decimal percentComplete = Decimal.Divide(Decimal.Multiply(percentage, Ammount), 100);
            return percentComplete;
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
                UpdateRunHeader();
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
                UpdateRunHeader();
            return success;
        }

        /// <summary>
        /// Update Amt Header
        /// </summary>
        private void UpdateRunHeader()
        {
            MCommissionRun run = new MCommissionRun(GetCtx(), GetC_CommissionRun_ID(), Get_TrxName());
            run.UpdateFromAmt();
            run.Save();
        }
    }
}
