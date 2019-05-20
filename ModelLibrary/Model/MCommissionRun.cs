/********************************************************
 * Module Name    : 
 * Purpose        : Commission Run
 * Class Used     : X_C_CommissionRun
 * Chronological    Development
 * Veena        07-Nov-2009
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
    /// Commission Run
    /// </summary>
    public class MCommissionRun : X_C_CommissionRun
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_CommissionRun_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCommissionRun(Ctx ctx, int C_CommissionRun_ID, Trx trxName)
            : base(ctx, C_CommissionRun_ID, trxName)
        {
            if (C_CommissionRun_ID == 0)
            {
                //	SetC_Commission_ID (0);
                //	SetDocumentNo (null);
                //	SetStartDate (new Timestamp(System.currentTimeMillis()));
                SetGrandTotal(Env.ZERO);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCommissionRun(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="commission">parent</param>
        public MCommissionRun(MCommission commission)
            : this(commission.GetCtx(), 0, commission.Get_TrxName())
	    {
            SetClientOrg(commission);
            SetC_Commission_ID(commission.GetC_Commission_ID());
	    }

        /// <summary>
        /// Get amounts
        /// </summary>
        /// <returns>array of amounts</returns>
        public MCommissionAmt[] GetAmts()
        {
            String sql = "SELECT * FROM C_CommissionAmt WHERE C_CommissionRun_ID=@crunid";
            List<MCommissionAmt> list = new List<MCommissionAmt>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@crunid", GetC_CommissionRun_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MCommissionAmt(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //	Convert
            MCommissionAmt[] retValue = new MCommissionAmt[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Update From Amt
        /// </summary>
        public void UpdateFromAmt()
        {
            MCommissionAmt[] amts = GetAmts();
            Decimal grandTotal = Env.ZERO;
            for (int i = 0; i < amts.Length; i++)
            {
                MCommissionAmt amt = amts[i];
                grandTotal = Decimal.Add(grandTotal, amt.GetCommissionAmt());
            }
            SetGrandTotal(grandTotal);
        }
    }
}
