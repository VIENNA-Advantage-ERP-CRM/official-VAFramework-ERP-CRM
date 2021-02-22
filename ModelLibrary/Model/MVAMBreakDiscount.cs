/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMDiscountCalculation
 * Purpose        : Discount Calculation purposes
 * Class Used     : X_VAM_BreakDiscount
 * Chronological    Development
 * Raghunandan     10-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MVAMBreakDiscount : X_VAM_BreakDiscount
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_BreakDiscount_ID"></param>
        /// <param name="trxName"></param>
        public MVAMBreakDiscount(Ctx ctx, int VAM_BreakDiscount_ID, Trx trxName)
            : base(ctx, VAM_BreakDiscount_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVAMBreakDiscount(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new record</param>
        /// <returns>true if validated</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // JID_0487: System should not allow to save same product category or product with same break value.
            string sql = "SELECT COUNT(VAM_BreakDiscount_ID) FROM VAM_BreakDiscount WHERE VAM_DiscountCalculation_ID = " + GetVAM_DiscountCalculation_ID()
                + " AND NVL(VAM_Product_ID, 0) = " + GetVAM_Product_ID() + " AND NVL(VAM_ProductCategory_ID, 0) = " + GetVAM_ProductCategory_ID() + " AND BreakValue = " + GetBreakValue() 
                + " AND IsActive='Y'  AND VAF_Client_ID=" + GetVAF_Client_ID() + " AND VAM_BreakDiscount_ID != " + Get_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (count > 0)
            {
                log.SaveError(Msg.GetMsg(GetCtx(), "DiscountBreakUnique"), "");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Criteria apply
        /// </summary>
        /// <param name="Value">amt or qty</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_ProductCategory_ID">category</param>
        /// <returns>true if criteria met</returns>
        public bool Applies(Decimal Value, int VAM_Product_ID, int VAM_ProductCategory_ID)
        {
            if (!IsActive())
                return false;

            //	below break value
            if (Value.CompareTo(GetBreakValue()) < 0)
                return false;

            //	No Product / Category 
            if (GetVAM_Product_ID() == 0
                && GetVAM_ProductCategory_ID() == 0)
                return true;

            //	Product
            if (GetVAM_Product_ID() == VAM_Product_ID)
                return true;

            //	Category
            if (VAM_ProductCategory_ID != 0)
                return GetVAM_ProductCategory_ID() == VAM_ProductCategory_ID;

            //	Look up Category of Product
            return MVAMProductCategory.IsCategory(GetVAM_ProductCategory_ID(), VAM_Product_ID);
        }

        /// <summary>
        /// 	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMBreakDiscount[");
            sb.Append(Get_ID()).Append("-Seq=").Append(GetSeqNo());
            if (GetVAM_ProductCategory_ID() != 0)
                sb.Append(",VAM_ProductCategory_ID=").Append(GetVAM_ProductCategory_ID());
            if (GetVAM_Product_ID() != 0)
                sb.Append(",VAM_Product_ID=").Append(GetVAM_Product_ID());
            sb.Append(",Break=").Append(GetBreakValue());
            if (IsBPartnerFlatDiscount())
                sb.Append(",FlatDiscount");
            else
                sb.Append(",Discount=").Append(GetBreakDiscount());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
