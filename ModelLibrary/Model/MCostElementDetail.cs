/********************************************************
 * Module Name    : 
 * Purpose        : Model for Cost Element Detail.
 * Class Used     : X_M_CostElementDetail
 * Chronological    Development
 * Amit Bansal      03-May-2016
**********************************************************/

using System;
using System.Collections.Generic;
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
    public class MCostElementDetail : X_M_CostElementDetail
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostElementDetail).FullName);

        public MCostElementDetail(Ctx ctx, int M_CostElementDetail_ID, Trx trxName)
            : base(ctx, M_CostElementDetail_ID, trxName)
        {

        }

        public MCostElementDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            return base.BeforeSave(newRecord);
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            return base.AfterSave(newRecord, success);
        }

        public static bool CreateCostElementDetail(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, MProduct Product, int M_ASI_ID,
                                                     MVABAccountBook mas, int M_costElement_ID, string windowName, MCostDetail cd, decimal amt, decimal qty)
        {
            try
            {
                MCostElementDetail ced = new MCostElementDetail(ctx, 0, cd.Get_Trx());
                ced.SetVAF_Client_ID(VAF_Client_ID);
                ced.SetVAF_Org_ID(VAF_Org_ID);
                ced.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                ced.SetM_CostElement_ID(M_costElement_ID);
                ced.SetM_Product_ID(Product.GetM_Product_ID());
                ced.SetM_AttributeSetInstance_ID(M_ASI_ID);
                ced.SetQty(qty);
                ced.SetAmt(amt);
                //Refrences
                ced.SetVAB_OrderLine_ID(cd.GetVAB_OrderLine_ID());
                ced.SetM_InOutLine_ID(cd.GetM_InOutLine_ID());
                if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                {
                    // not to bind Invoiceline refernece on cost element detail
                }
                else
                {
                    ced.SetVAB_InvoiceLine_ID(cd.GetVAB_InvoiceLine_ID());
                }
                ced.Set_Value("VAFAM_AssetDisposal_ID", cd.Get_Value("VAFAM_AssetDisposal_ID"));
                ced.SetM_InventoryLine_ID(cd.GetM_InventoryLine_ID());
                ced.SetM_MovementLine_ID(cd.GetM_MovementLine_ID());
                ced.SetVAB_ProjectSupply_ID(cd.GetVAB_ProjectSupply_ID());
                ced.SetIsSOTrx(cd.IsSOTrx());
                ced.SetA_Asset_ID(cd.GetA_Asset_ID());
                ced.SetM_ProductionLine_ID(cd.GetM_ProductionLine_ID());
                ced.SetM_WorkOrderResourceTxnLine_ID(cd.GetM_WorkOrderResourceTxnLine_ID());
                ced.SetM_WorkOrderTransactionLine_ID(cd.GetM_WorkOrderTransactionLine_ID());
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
                ced.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());
                if (!ced.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error Occured during costing " + pp.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Error Occured during costing " + ex.ToString());
                return false;
            }
            return true;
        }
    }
}
