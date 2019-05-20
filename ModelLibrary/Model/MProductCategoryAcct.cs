/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_Product_Category_Acct
 * Chronological Development
 * Veena Pandey     17-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MProductCategoryAcct : X_M_Product_Category_Acct
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductCategoryAcct).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MProductCategoryAcct(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProductCategoryAcct(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get Category Acct
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_Category_ID">category</param>
        /// <param name="C_AcctSchema_ID">acct schema</param>
        /// <param name="trxName">transaction</param>
        /// <returns>category acct</returns>
        public static MProductCategoryAcct Get(Ctx ctx, int M_Product_Category_ID, 
            int C_AcctSchema_ID, Trx trxName)
        {
            MProductCategoryAcct retValue = null;
            String sql = "SELECT * FROM  M_Product_Category_Acct "
                + "WHERE M_Product_Category_ID=" + M_Product_Category_ID + " AND C_AcctSchema_ID=" + C_AcctSchema_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[0].Rows)
                    {
                        retValue = new MProductCategoryAcct(ctx, rs, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Check Costing Setup
        /// </summary>
        public void CheckCosting()
        {
            //	Create Cost Elements
            if (GetCostingMethod() != null && GetCostingMethod().Length > 0)
                MCostElement.GetMaterialCostElement(this, GetCostingMethod());
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new record</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            CheckCosting();
            return success;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProductCategoryAcct[");
            sb.Append(Get_ID())
                .Append(",M_Product_Category_ID=").Append(GetM_Product_Category_ID())
                .Append(",C_AcctSchema_ID=").Append(GetC_AcctSchema_ID())
                .Append(",CostingLevel=").Append(GetCostingLevel())
                .Append(",CostingMethod=").Append(GetCostingMethod())
                .Append("]");
            return sb.ToString();
        }
    }
}
