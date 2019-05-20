/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDiscountSchema
 * Purpose        : Discount Calculation purposes
 * Class Used     : X_M_DiscountSchemaBreak
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MDiscountSchemaBreak : X_M_DiscountSchemaBreak
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_DiscountSchemaBreak_ID"></param>
        /// <param name="trxName"></param>
        public MDiscountSchemaBreak(Ctx ctx, int M_DiscountSchemaBreak_ID, Trx trxName)
            : base(ctx, M_DiscountSchemaBreak_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MDiscountSchemaBreak(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Criteria apply
        /// </summary>
        /// <param name="Value">amt or qty</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Product_Category_ID">category</param>
        /// <returns>true if criteria met</returns>
        public bool Applies(Decimal Value, int M_Product_ID, int M_Product_Category_ID)
        {
            if (!IsActive())
                return false;

            //	below break value
            if (Value.CompareTo(GetBreakValue()) < 0)
                return false;

            //	No Product / Category 
            if (GetM_Product_ID() == 0
                && GetM_Product_Category_ID() == 0)
                return true;

            //	Product
            if (GetM_Product_ID() == M_Product_ID)
                return true;

            //	Category
            if (M_Product_Category_ID != 0)
                return GetM_Product_Category_ID() == M_Product_Category_ID;

            //	Look up Category of Product
            return MProductCategory.IsCategory(GetM_Product_Category_ID(), M_Product_ID);
        }

        /// <summary>
        /// 	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDiscountSchemaBreak[");
            sb.Append(Get_ID()).Append("-Seq=").Append(GetSeqNo());
            if (GetM_Product_Category_ID() != 0)
                sb.Append(",M_Product_Category_ID=").Append(GetM_Product_Category_ID());
            if (GetM_Product_ID() != 0)
                sb.Append(",M_Product_ID=").Append(GetM_Product_ID());
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
