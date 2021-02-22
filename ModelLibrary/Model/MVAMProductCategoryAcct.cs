/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_ProductCategory_Acct
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
    public class MVAMProductCategoryAcct : X_VAM_ProductCategory_Acct
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductCategoryAcct).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductCategoryAcct(Ctx ctx, int ignored, Trx trxName)
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
        public MVAMProductCategoryAcct(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get Category Acct
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductCategory_ID">category</param>
        /// <param name="VAB_AccountBook_ID">acct schema</param>
        /// <param name="trxName">transaction</param>
        /// <returns>category acct</returns>
        public static MVAMProductCategoryAcct Get(Ctx ctx, int VAM_ProductCategory_ID, 
            int VAB_AccountBook_ID, Trx trxName)
        {
            MVAMProductCategoryAcct retValue = null;
            String sql = "SELECT * FROM  VAM_ProductCategory_Acct "
                + "WHERE VAM_ProductCategory_ID=" + VAM_ProductCategory_ID + " AND VAB_AccountBook_ID=" + VAB_AccountBook_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[0].Rows)
                    {
                        retValue = new MVAMProductCategoryAcct(ctx, rs, trxName);
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
                MVAMVAMProductCostElement.GetMaterialCostElement(this, GetCostingMethod());
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
            StringBuilder sb = new StringBuilder("MVAMProductCategoryAcct[");
            sb.Append(Get_ID())
                .Append(",VAM_ProductCategory_ID=").Append(GetVAM_ProductCategory_ID())
                .Append(",VAB_AccountBook_ID=").Append(GetVAB_AccountBook_ID())
                .Append(",CostingLevel=").Append(GetCostingLevel())
                .Append(",CostingMethod=").Append(GetCostingMethod())
                .Append("]");
            return sb.ToString();
        }
    }
}
