using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MCostQueueTransaction : X_M_CostQueueTransaction
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostQueueTransaction).FullName);

        public MCostQueueTransaction(Ctx ctx, int M_CostQueueTransaction_ID, Trx trxName)
            : base(ctx, M_CostQueueTransaction_ID, trxName)
        {

        }

        public MCostQueueTransaction(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// This function will create child record of Cost Queue, which will contain transaction affects 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">Client ID</param>
        /// <param name="AD_Org_ID">Organization ID</param>
        /// <param name="M_CostQueue_ID">Cost Queue ID</param>
        /// <param name="cd">Cost Detail Reference</param>
        /// <param name="qty">qty</param>
        /// <returns>true, when success</returns>
        public static bool CreateCostQueueTransaction(Ctx ctx, int AD_Client_ID, int AD_Org_ID, int M_CostQueue_ID, MCostDetail cd, decimal qty)
        {
            try
            {
                MCostQueueTransaction ced = new MCostQueueTransaction(ctx, 0, cd.Get_Trx());
                ced.SetAD_Client_ID(AD_Client_ID);
                ced.SetAD_Org_ID(AD_Org_ID);
                ced.SetM_CostQueue_ID(M_CostQueue_ID);
                ced.SetM_Product_ID(cd.GetM_Product_ID());
                ced.SetM_AttributeSetInstance_ID(cd.GetM_AttributeSetInstance_ID());
                ced.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());

                // date and qty 
                ced.SetMovementQty(qty);
                ced.SetMovementDate(DateTime.Now);

                //Refrences
                ced.SetM_InOutLine_ID(cd.GetM_InOutLine_ID());
                if (ced.GetM_InOutLine_ID() > 0)
                {
                    DataSet ds = DB.ExecuteDataset(@"SELECT M_InOut.IsSOTrx, M_InOut.IsReturnTrx FROM M_InOutLine
                                    INNER JOIN M_InOut ON M_InOutLine.M_InOut_ID = M_InOut.M_InOut_ID 
                                    WHERE M_InOutLine.M_InOutLine_ID = " + ced.GetM_InOutLine_ID(), null, cd.Get_Trx());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ced.SetIsSOTrx(Util.GetValueOfString(ds.Tables[0].Rows[0]["IsSOTrx"]).Equals("N") ? false : true);
                        ced.SetIsReturnTrx(Util.GetValueOfString(ds.Tables[0].Rows[0]["IsReturnTrx"]).Equals("N") ? false : true);
                    }
                }
                ced.SetM_InventoryLine_ID(cd.GetM_InventoryLine_ID());
                if (ced.GetM_InventoryLine_ID() > 0)
                {
                    bool isInternalUse = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT M_Inventory.IsInternalUse  FROM M_InventoryLine
                                    INNER JOIN M_Inventory ON M_InventoryLine.M_Inventory_ID = M_Inventory.M_Inventory_ID 
                                    WHERE M_InventoryLine.M_InventoryLine_ID = " + ced.GetM_InventoryLine_ID(), null, cd.Get_Trx())).Equals("N") ? false : true;
                    ced.SetIsInternalUse(isInternalUse);
                }
                ced.SetM_MovementLine_ID(cd.GetM_MovementLine_ID());
                ced.SetC_ProjectIssue_ID(cd.GetC_ProjectIssue_ID());
                //ced.SetA_Asset_ID(cd.GetA_Asset_ID());
                ced.SetM_ProductionLine_ID(cd.GetM_ProductionLine_ID());
                if (Env.IsModuleInstalled("VAFAM_") && ced.Get_ColumnIndex("VAFAM_AssetDisposal_ID") > -1)
                {
                    ced.Set_Value("VAFAM_AssetDisposal_ID", cd.Get_Value("VAFAM_AssetDisposal_ID"));
                }
                if (Env.IsModuleInstalled("VAMFG_"))
                {
                    if (ced.Get_ColumnIndex("VAMFG_M_WrkOdrRscTxnLine_ID") > -1)
                    {
                        ced.Set_Value("VAMFG_M_WrkOdrRscTxnLine_ID", cd.GetVAMFG_M_WrkOdrRscTxnLine_ID());
                    }
                    if (ced.Get_ColumnIndex("VAMFG_M_WrkOdrTrnsctionLine_ID") > -1)
                    {
                        ced.Set_Value("VAMFG_M_WrkOdrTrnsctionLine_ID", cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID());
                    }
                }
                if (!ced.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    string error = "";
                    if (pp != null)
                    {
                        error = pp.GetValue();
                        if (String.IsNullOrEmpty(error))
                        {
                            error = pp.GetValue();
                        }
                    }
                    _log.Info("Costing Engine : Error Occured during saving a record on Cost Queue Transaction -> " + error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Costing Engine : Exception Occured during saving a record on Cost Queue Transaction " + ex.Message);
                return false;
            }
            return true;
        }
    }
}
