/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ProductCategoryAcctCopy
 * Purpose        : Copy Product Catergory Default Accounts 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      11-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ProductCategoryAcctCopy : ProcessEngine.SvrProcess
    {
        //Product Categpory			
        private int _VAM_ProductCategory_ID = 0;
        //Acct Schema					
        private int _VAB_AccountBook_ID = 0;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAM_ProductCategory_ID"))
                {
                    _VAM_ProductCategory_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_AccountBook_ID"))
                {
                    _VAB_AccountBook_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("VAB_AccountBook_ID=" + _VAB_AccountBook_ID);
            if (_VAB_AccountBook_ID == 0)
            {
                throw new Exception("VAB_AccountBook_ID=0");
            }
            MVABAccountBook as1 = MVABAccountBook.Get(GetCtx(), _VAB_AccountBook_ID);
            if (as1.Get_ID() == 0)
            {
                throw new Exception("Not Found - VAB_AccountBook_ID=" + _VAB_AccountBook_ID);
            }

            //	Update
            String sql = "UPDATE VAM_Product_Acct pa "
                + "SET (P_Revenue_Acct,P_Expense_Acct,P_CostAdjustment_Acct,P_InventoryClearing_Acct,P_Asset_Acct,P_COGS_Acct,"
                + " P_PurchasePriceVariance_Acct,P_InvoicePriceVariance_Acct,"
                + " P_TradeDiscountRec_Acct,P_TradeDiscountGrant_Acct "
                // Added **************** Lokesh Chauhan **************** 
                    + "  ,P_Resource_Absorption_Acct , P_MaterialOverhd_Acct "
                // Added **************** 
                + " )="
                 + " (SELECT P_Revenue_Acct,P_Expense_Acct,P_CostAdjustment_Acct,P_InventoryClearing_Acct,P_Asset_Acct,P_COGS_Acct,"
                 + " P_PurchasePriceVariance_Acct,P_InvoicePriceVariance_Acct,"
                 + " P_TradeDiscountRec_Acct,P_TradeDiscountGrant_Acct"
                // Added **************** Lokesh Chauhan **************** 
                    + " ,P_Resource_Absorption_Acct , P_MaterialOverhd_Acct "
                // Added **************** 
                 + " FROM VAM_ProductCategory_Acct pca"
                 + " WHERE pca.VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID
                 + " AND pca.VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                 + "), Updated=SysDate, UpdatedBy=0 "
                + "WHERE pa.VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                + " AND EXISTS (SELECT * FROM VAM_Product p "
                    + "WHERE p.VAM_Product_ID=pa.VAM_Product_ID"
                    + " AND p.VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID + ")";
            int updated = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(updated), "@Updated@");

            //	Insert new Products
            sql = "INSERT INTO VAM_Product_Acct "
                + "(VAM_Product_ID, VAB_AccountBook_ID,"
                + " VAF_Client_ID, VAF_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                + " P_Revenue_Acct, P_Expense_Acct, P_CostAdjustment_Acct, P_InventoryClearing_Acct, P_Asset_Acct, P_CoGs_Acct,"
                + " P_PurchasePriceVariance_Acct, P_InvoicePriceVariance_Acct,"
                + " P_TradeDiscountRec_Acct, P_TradeDiscountGrant_Acct "
                //Added
                + "  ,P_Resource_Absorption_Acct, P_MaterialOverhd_Acct "
                + ") "
                + "SELECT p.VAM_Product_ID, acct.VAB_AccountBook_ID,"
                + " p.VAF_Client_ID, p.VAF_Org_ID, 'Y', SysDate, 0, SysDate, 0,"
                + " acct.P_Revenue_Acct, acct.P_Expense_Acct, acct.P_CostAdjustment_Acct, acct.P_InventoryClearing_Acct, acct.P_Asset_Acct, acct.P_CoGs_Acct,"
                + " acct.P_PurchasePriceVariance_Acct, acct.P_InvoicePriceVariance_Acct,"
                + " acct.P_TradeDiscountRec_Acct, acct.P_TradeDiscountGrant_Acct "
                + " ,acct.P_Resource_Absorption_Acct, acct.P_MaterialOverhd_Acct"
                + " FROM VAM_Product p"
                + " INNER JOIN VAM_ProductCategory_Acct acct ON (acct.VAM_ProductCategory_ID=p.VAM_ProductCategory_ID)"
                + "WHERE acct.VAB_AccountBook_ID=" + _VAB_AccountBook_ID			//	#
                + " AND p.VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID	//	#
                + " AND NOT EXISTS (SELECT * FROM VAM_Product_Acct pa "
                    + "WHERE pa.VAM_Product_ID=p.VAM_Product_ID"
                    + " AND pa.VAB_AccountBook_ID=acct.VAB_AccountBook_ID)";
            int created = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(created), "@Created@");

            return "@Created@=" + created + ", @Updated@=" + updated;
        }

    }
}
