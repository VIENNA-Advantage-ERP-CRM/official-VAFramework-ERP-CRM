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

        /// <summary>
        /// This function is used to Create cost Element Detail
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">Client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <param name="Product">Product</param>
        /// <param name="M_ASI_ID">Attribute Set Instance</param>
        /// <param name="mas">Accounting Schema</param>
        /// <param name="M_costElement_ID">Cost Element</param>
        /// <param name="windowName">WindowName</param>
        /// <param name="cd">Cost Detail</param>
        /// <param name="amt">Amount</param>
        /// <param name="qty">Quantity</param>
        /// <returns>true, when success</returns>
        public static bool CreateCostElementDetail(Ctx ctx, int AD_Client_ID, int AD_Org_ID, MProduct Product, int M_ASI_ID,
                                                     MAcctSchema mas, int M_costElement_ID, string windowName, MCostDetail cd, decimal amt, decimal qty)
        {
            try
            {
                MCostElementDetail ced = new MCostElementDetail(ctx, 0, cd.Get_Trx());
                ced.SetAD_Client_ID(AD_Client_ID);
                ced.SetAD_Org_ID(AD_Org_ID);
                ced.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                ced.SetM_CostElement_ID(M_costElement_ID);
                ced.SetM_Product_ID(Product.GetM_Product_ID());
                ced.SetM_AttributeSetInstance_ID(M_ASI_ID);
                ced.SetQty(qty);
                ced.SetAmt(amt);
                //Refrences
                ced.SetC_OrderLine_ID(cd.GetC_OrderLine_ID());
                ced.SetM_InOutLine_ID(cd.GetM_InOutLine_ID());
                if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                {
                    // not to bind Invoiceline refernece on cost element detail
                }
                else
                {
                    ced.SetC_InvoiceLine_ID(cd.GetC_InvoiceLine_ID());
                }
                ced.Set_Value("VAFAM_AssetDisposal_ID", cd.Get_Value("VAFAM_AssetDisposal_ID"));
                ced.SetM_InventoryLine_ID(cd.GetM_InventoryLine_ID());
                ced.SetM_MovementLine_ID(cd.GetM_MovementLine_ID());
                ced.SetC_ProjectIssue_ID(cd.GetC_ProjectIssue_ID());
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
                if (ced.Get_ColumnIndex("C_ProvisionalInvoiceLine_ID") > -1)
                {
                    ced.Set_Value("C_ProvisionalInvoiceLine_ID", cd.Get_Value("C_ProvisionalInvoiceLine_ID"));
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
