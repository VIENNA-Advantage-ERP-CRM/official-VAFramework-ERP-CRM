
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Tax Exempt Reason Model class
 * Employee Code  :    1052
 * Date           :    8-Dec-2021
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MTaxExemptReason : X_C_TaxExemptReason
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_TaxExemptReason_ID">Tax Exempt Reason</param>
        /// <param name="trxName">transaction</param>
        /// <writer>1052</writer>
        public MTaxExemptReason(Ctx ctx, int C_TaxExemptReason_ID, Trx trxName)
           : base(ctx, C_TaxExemptReason_ID, trxName)
        {
           
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        /// <writer>1052</writer>
        public MTaxExemptReason(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <writer>1052</writer>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (IsDefault())
            {
                string sql = "SELECT COUNT(C_TaxExemptReason_ID) FROM C_TaxExemptReason WHERE IsActive='Y' AND IsDefault='Y' AND AD_Client_ID=" + GetAD_Client_ID();
                if (GetAD_Org_ID() != 0)
                {
                    //only check for org if * is not selected 
                    sql += " AND AD_Org_ID IN(0," + GetAD_Org_ID() + ")";
                }
                if (!newRecord)
                {
                    sql += " AND C_TaxExemptReason_ID!= " + GetC_TaxExemptReason_ID();
                }
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) >= 1)
                {
                    log.SaveError("DefaultRecordFound", "");
                    return false;
                }
            }
            return true;
        }
    }
}
