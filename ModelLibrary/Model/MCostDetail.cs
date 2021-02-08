/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_ProductCostDetail
 * Chronological Development
 * Veena Pandey     18-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCostDetail : X_VAM_ProductCostDetail
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostDetail).FullName);
        private bool _isExpectedLandeCostCalculated = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductCostDetail_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(Ctx ctx, int VAM_ProductCostDetail_ID, Trx trxName)
            : base(ctx, VAM_ProductCostDetail_ID, trxName)
        {
            if (VAM_ProductCostDetail_ID == 0)
            {
                //	setVAB_AccountBook_ID (0);
                //	setVAM_Product_ID (0);
                SetVAM_PFeature_SetInstance_ID(0);
                //	setVAB_OrderLine_ID (0);
                //	setVAM_Inv_InOutLine_ID(0);
                //	setVAB_InvoiceLine_ID (0);
                SetProcessed(false);
                SetAmt(Env.ZERO);
                SetQty(Env.ZERO);
                SetIsSOTrx(false);
                SetDeltaAmt(Env.ZERO);
                SetDeltaQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            int VAM_ProductCostElement_ID, Decimal Amt, Decimal Qty, String Description, Trx trxName)
            : this(mas.GetCtx(), 0, trxName)
        {
            SetClientOrg(mas.GetVAF_Client_ID(), VAF_Org_ID);
            SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            //
            SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
            //
            SetAmt(Amt);
            SetQty(Qty);
            SetDescription(Description);
        }

        /// <summary>
        /// Is used to create Cost detail with respective reference
        /// </summary>
        /// <param name="mas">Accounting SChema</param>
        /// <param name="VAF_Org_ID">Organization</param>
        /// <param name="VAM_Product_ID">Product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">AttributeSetInstance</param>
        /// <param name="WindowName">calling window</param>
        /// <param name="inventoryLine">inventory line reference</param>
        /// <param name="inoutline">inout line reference</param>
        /// <param name="movementline">movement line reference</param>
        /// <param name="invoiceline">invoice line reference </param>
        /// <param name="po">Production execution reference</param>
        /// <param name="VAM_ProductCostElement_ID">cost element</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">description</param>
        /// <param name="trxName">trasnaction</param>
        /// <param name="VAM_Warehouse_ID">Optional parameter -- Warehouse ID</param>
        /// <returns>MCostDetail Object</returns>
        public static MCostDetail CreateCostDetail(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, string WindowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
            MInvoiceLine invoiceline, PO po, int VAM_ProductCostElement_ID, Decimal Amt, Decimal Qty, String Description, Trx trxName, int VAM_Warehouse_ID = 0)
        {
            try
            {
                Amt = Decimal.Round(Amt, mas.GetCostingPrecision());
                MCostDetail cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                cd.SetProcessed(true);
                cd.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                if (WindowName == "Physical Inventory" || WindowName == "Internal Use Inventory")
                {
                    cd.SetVAM_InventoryLine_ID(inventoryLine.GetVAM_InventoryLine_ID());
                }
                if (WindowName == "AssetDisposal")
                {
                    cd.Set_Value("VAFAM_AssetDisposal_ID", po.Get_Value("VAFAM_AssetDisposal_ID"));
                }
                else if (WindowName == "Production Execution" || WindowName.Equals("PE-FinishGood"))
                {
                    cd.SetVAMFG_M_WrkOdrTrnsctionLine_ID(Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                }
                else if (WindowName == "Inventory Move")
                {
                    cd.SetVAM_InvTrf_Line_ID(movementline.GetVAM_InvTrf_Line_ID());
                }
                else if (WindowName == "Material Receipt" || WindowName == "Shipment" || WindowName == "Customer Return" || WindowName == "Return To Vendor")
                {
                    cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                    if (inoutline.GetVAB_OrderLine_ID() > 0)
                    {
                        cd.SetVAB_OrderLine_ID(inoutline.GetVAB_OrderLine_ID());
                    }
                    if (WindowName == "Material Receipt" || WindowName == "Return To Vendor")
                    {
                        cd.SetIsSOTrx(false);
                    }
                    else
                    {
                        cd.SetIsSOTrx(true);
                    }
                }
                else if (WindowName == "Invoice(Vendor)" || WindowName == "Invoice(Vendor)-Return" || WindowName == "LandedCost")
                {
                    cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                    cd.SetIsSOTrx(false);
                    if (invoiceline.GetVAB_OrderLine_ID() > 0)
                    {
                        cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                    }
                    if (invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                    {
                        if (inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                        {
                            cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                        }
                        else
                        {
                            cd.SetVAM_Inv_InOutLine_ID(invoiceline.GetVAM_Inv_InOutLine_ID());
                        }
                    }
                }
                else if (WindowName == "Invoice(Customer)")
                {
                    cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                    cd.SetIsSOTrx(true);
                    if (invoiceline.GetVAB_OrderLine_ID() > 0)
                    {
                        cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                    }
                    if (invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                    {
                        cd.SetVAM_Inv_InOutLine_ID(invoiceline.GetVAM_Inv_InOutLine_ID());
                    }
                    else if (invoiceline.GetVAB_OrderLine_ID() > 0)
                    {
                        cd.SetVAM_Inv_InOutLine_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Inv_InOutLine_id FROM VAM_Inv_InOutLine WHERE isactive     = 'Y' AND VAB_Orderline_id = " + invoiceline.GetVAB_OrderLine_ID(), null, null)));
                    }
                }
                bool ok = cd.Save();
                if (ok)
                {
                    return cd;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateProductCost(String windowName, MCostDetail cd, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, int cq_VAF_Org_ID, string optionalStrCd = "process")
        {
            int VAF_Org_ID = 0;
            // Get Org based on Costing Level
            dynamic pc = null;
            String cl = null;
            string costingMethod = null;
            int costElementId = 0;
            MVAFClient client = MVAFClient.Get(GetCtx(), cd.GetVAF_Client_ID());
            int VAM_Warehouse_ID = 0; // is used to calculate cost with warehouse level or not

            if (product != null)
            {
                pc = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pc != null)
                {
                    cl = pc.GetCostingLevel();
                    costingMethod = pc.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costElementId = pc.GetVAM_ProductCostElement_ID();
                    }
                }
            }
            if (cl == null || costingMethod == null)
            {
                if (cl == null)
                {
                    cl = acctSchema.GetCostingLevel();
                }
                if (costingMethod == null)
                {
                    costingMethod = acctSchema.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costElementId = acctSchema.GetVAM_ProductCostElement_ID();
                    }
                }
            }

            // set Organization for product costs
            if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_BatchLot)
            {
                VAF_Org_ID = 0;
            }
            else
            {
                VAF_Org_ID = cd.GetVAF_Org_ID();
            }
            // set ASI as ZERO in case of costing levele either "Client" or "Organization" or "Warehouse"
            if (cl != MProductCategory.COSTINGLEVEL_BatchLot && cl != MProductCategory.COSTINGLEVEL_OrgPlusBatch && cl != MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_ASI_ID = 0;
            }
            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                VAM_Warehouse_ID = cd.GetVAM_Warehouse_ID();
            }

            // get Cost element id of selected costing method
            if (costingMethod != "C")
            {
                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT DISTINCT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' 
                            AND CostingMethod = '" + costingMethod + "' AND VAF_Client_ID = " + product.GetVAF_Client_ID()));
            }
            else if (costingMethod == "C") // costing method on cost combination - Element line
            {
                string sql = @"SELECT  cel.M_Ref_CostElement
                                    FROM VAM_ProductCostElement ce INNER JOIN VAM_ProductCostElementLine cel ON ce.VAM_ProductCostElement_ID  = cel.VAM_ProductCostElement_ID
                                    WHERE ce.VAF_Client_ID   =" + product.GetVAF_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.VAM_ProductCostElement_ID = " + costElementId + @"
                                    AND  CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE costingmethod IS NOT NULL  )
                                    ORDER BY ce.VAM_ProductCostElement_ID";
                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            }

            // Get All Material Costing Element
            // when calculate cost from process, then no need to calculate cost for define costing method either on product category or on Accounting Schema
            if (GetVAM_ProductCostElement_ID() == 0 && optionalStrCd == "process")
            {
                MCostElement[] ces = MCostElement.GetCostingMethods(this);
                try
                {
                    for (int i = 0; i < ces.Length; i++)
                    {
                        MCostElement ce = ces[i];
                        if (ce.GetVAM_ProductCostElement_ID() != costElementId)
                        {
                            if (!UpdateCost(acctSchema, product, ce, VAF_Org_ID, M_ASI_ID, 0, cq_VAF_Org_ID, windowName, cd, costingMethod, costElementId, VAM_Warehouse_ID))
                            {
                                return false;
                            }
                        }
                        else if (!client.IsCostImmediate())
                        {
                            if (!UpdateCost(acctSchema, product, ce, VAF_Org_ID, M_ASI_ID, 0, cq_VAF_Org_ID, windowName, cd, costingMethod, costElementId, VAM_Warehouse_ID))
                            {
                                return false;
                            }
                        }
                        else if (client.IsCostImmediate())
                        {
                            // if cost not calculated on completion for defined costing method then calculate during schedular run
                            string isCostImmediate = "N";
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAM_InventoryLine WHERE VAM_InventoryLine_ID =  " + cd.GetVAM_InventoryLine_ID(), null, Get_Trx()));
                            }
                            if (windowName == "AssetDisposal")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAFAM_AssetDisposal WHERE VAFAM_AssetDisposal_ID = " + cd.Get_Value("VAFAM_AssetDisposal_ID"), null, Get_Trx()));
                            }
                            else if (windowName == "Inventory Move")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAM_InvTrf_Line WHERE VAM_InvTrf_Line_ID =  " + cd.GetVAM_InvTrf_Line_ID(), null, Get_Trx()));
                            }
                            else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID(), null, Get_Trx()));
                            }
                            else if (windowName == "Invoice(Vendor)" || windowName == "Match IV" || windowName == "Product Cost IV" || windowName == "Product Cost IV Form" || windowName == "Invoice(Vendor)-Return")
                            {
                                // isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAB_InvoiceLine WHERE VAB_InvoiceLine_ID =  " + cd.GetVAB_InvoiceLine_ID(), null, Get_Trx()));
                                string docStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM VAB_Invoice WHERE VAB_Invoice_ID = (SELECT VAB_Invoice_ID FROM VAB_InvoiceLine WHERE VAB_InvoiceLine_ID = " + cd.GetVAB_InvoiceLine_ID() + ")", null, Get_Trx()));
                                if (docStatus == "VO" || docStatus == "RE")
                                {
                                    isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT isreversedcostcalculated FROM VAM_MatchInvoiceoiceCostTrack WHERE Rev_VAB_InvoiceLine_ID =  " + cd.GetVAB_InvoiceLine_ID() + " AND VAM_Inv_InOutLine_ID = " + cd.GetVAM_Inv_InOutLine_ID(), null, Get_Trx()));
                                }
                                else
                                {
                                    isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAM_MatchInvoice WHERE VAB_InvoiceLine_ID =  " + cd.GetVAB_InvoiceLine_ID() + " AND VAM_Inv_InOutLine_ID = " + cd.GetVAM_Inv_InOutLine_ID(), null, Get_Trx()));
                                }
                            }
                            else if (windowName == "Invoice(Customer)" || windowName == "Invoice(APC)")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAB_InvoiceLine WHERE VAB_InvoiceLine_ID =  " + cd.GetVAB_InvoiceLine_ID(), null, Get_Trx()));
                            }
                            if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAMFG_M_WrkOdrTrnsctionLine WHERE VAMFG_M_WrkOdrTrnsctionLine_ID =  " + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID(), null, Get_Trx()));
                            }
                            if (isCostImmediate == "N")
                            {
                                if (!UpdateCost(acctSchema, product, ce, VAF_Org_ID, M_ASI_ID, 0, cq_VAF_Org_ID, windowName, cd, costingMethod, costElementId, VAM_Warehouse_ID))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // when calculate cost on completion, then calculate cost of defined costing method either on product category or on Accounting Schema
                MCostElement ce = null;
                if (optionalStrCd == "window" && GetVAM_ProductCostElement_ID() == 0)
                {
                    ce = MCostElement.Get(GetCtx(), costElementId);
                }
                else
                {
                    ce = MCostElement.Get(GetCtx(), GetVAM_ProductCostElement_ID());
                }
                if (!UpdateCost(acctSchema, product, ce, VAF_Org_ID, M_ASI_ID, 0, cq_VAF_Org_ID, windowName, cd, costingMethod, costElementId, VAM_Warehouse_ID))
                {
                    return false;
                }
            }
            return true;
        }

        private bool UpdateCost(MVABAccountBook mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int VAA_Asset_ID, int cq_VAF_Org_ID,
                                string windowName, MCostDetail cd, string costingMethod = "", int costCominationelement = 0, int VAM_Warehouse_ID = 0)
        {
            MCost cost = null;
            MInOutLine inoutline = null;
            MInOut inout = null;
            MInvoice invoice = null;
            MInvoiceLine invoiceline = null;
            MMovement movement = null;
            MMovementLine movementline = null;
            MCost costFrom = null;
            decimal amtWithSurcharge = 0;

            if (VAA_Asset_ID == 0)
            {
                if (windowName == "Inventory Move")
                {
                    cost = MCost.Get(product, M_ASI_ID, mas, cq_VAF_Org_ID, ce.GetVAM_ProductCostElement_ID(), VAM_Warehouse_ID);
                    //change 10-5-2016
                    if (cd.GetVAM_InvTrf_Line_ID() > 0)
                    {
                        int M_SourceWarehouse_ID = 0;
                        MMovementLine movementlineFrom = new MMovementLine(GetCtx(), cd.GetVAM_InvTrf_Line_ID(), Get_TrxName());
                        // when costing level is warehouse then need to get source warehouse
                        if (VAM_Warehouse_ID > 0)
                        {
                            M_SourceWarehouse_ID = MLocator.Get(GetCtx(), movementlineFrom.GetVAM_Locator_ID()).GetVAM_Warehouse_ID();
                        }
                        costFrom = MCost.Get(product, M_ASI_ID, mas, movementlineFrom.GetVAF_Org_ID(), ce.GetVAM_ProductCostElement_ID(), M_SourceWarehouse_ID);
                    }
                    //end
                }
                else
                {
                    cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), VAM_Warehouse_ID);
                }
            }
            else
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), VAA_Asset_ID, VAM_Warehouse_ID);
            }

            //Decimal qty = 0;
            //Decimal amt = 0;
            //if (ce.IsWeightedAverageCost() && ((GetVAB_InvoiceLine_ID() != 0 && windowName != "Material Receipt") || (windowName == "Product Cost IV") || (windowName == "Product Cost IV Form")))
            //{
            //    invoiceline = new MInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_Trx());
            //    invoice = new MInvoice(GetCtx(), invoiceline.GetVAB_Invoice_ID(), Get_Trx());
            //    if (GetVAB_InvoiceLine_ID() > 0 && ((invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->")) || (invoice.GetDescription() == null)))
            //    {
            //        amt = Decimal.Divide(GetAmt(), GetQty());

            //        string sql = "SELECT SUM(QTY) FROM VAM_MatchInvoice WHERE IsCostCalculated = 'N' AND VAB_InvoiceLine_ID = " + GetVAB_InvoiceLine_ID();
            //        qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, cd.Get_Trx()));

            //        amt = Decimal.Multiply(amt, qty);

            //        sql = "UPDATE VAM_MatchInvoice SET IsCostCalculated = 'Y' WHERE VAB_InvoiceLine_ID = " + GetVAB_InvoiceLine_ID();
            //        DB.ExecuteQuery(sql, null, cd.Get_Trx());
            //    }
            //    else if (GetVAB_InvoiceLine_ID() > 0 && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
            //    {
            //        // on reversal - get qty from new table of VAM_MatchInvoiceoiceCostTrack
            //        qty = GetQty();
            //    }
            //}
            //else
            //{
            //    qty = GetQty();
            //    amt = GetAmt();
            //}

            Decimal qty = GetQty();
            decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();

            amt = Decimal.Add(amt, Decimal.Round(Decimal.Divide(Decimal.Multiply(amt, ce.GetSurchargePercentage()), 100), mas.GetCostingPrecision()));

            //this region is used for reducing MR price from Accumulation Amount on Product costs
            if (windowName == "Match IV")
            {
                #region Match IV
                Decimal MRPrice = 0;
                string sql;

                if (ce.IsAveragePO() || ce.IsLastPOPrice() || ce.IsWeightedAverageCost() || ce.IsWeightedAveragePO())
                {

                }
                else
                {
                    //MClient client = MClient.Get(GetCtx(), cd.GetVAF_Client_ID());
                    if (ce.IsFifo() || ce.IsLifo())
                    {
                        sql = @"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                     where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) =  " + M_ASI_ID +
                                     @" AND ced.VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND cq.VAM_ProductCostElement_ID = " + ce.GetVAM_ProductCostElement_ID() +
                                     @" AND ced.VAF_Client_ID = " + cd.GetVAF_Client_ID();
                        MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        if (MRPrice == 0)
                        {
                            sql = @"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id
                                     where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) =  " + M_ASI_ID +
                                     @" AND ced.VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) =  " + cd.GetVAB_OrderLine_ID() +
                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND cq.VAM_ProductCostElement_ID = " + ce.GetVAM_ProductCostElement_ID() +
                                     @" AND ced.VAF_Client_ID = " + cd.GetVAF_Client_ID();
                            MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }
                        if (MRPrice == 0)
                        {
                            // this is used because we remove the cost queue
                            sql = @"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced LEFT JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                     where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) =  " + M_ASI_ID +
                                        @" AND ced.VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                        @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND NVL(cq.VAM_ProductCostElement_ID , 0)  = 0 " +
                                        @" AND ced.VAF_Client_ID = " + cd.GetVAF_Client_ID();
                            MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }
                    }
                    else
                    {
                        // case handleed here as    MR Completed with 100 price,  Then Inv with MR completed with 9120 price , then run process 
                        // check record exist on cost element detail only for MR then consider that amount else check with invoiceline ref
                        if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                     " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " + ce.GetVAM_ProductCostElement_ID() +
                                     " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                     " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = 0", null, Get_Trx())) > 0)
                        {
                            sql = @"SELECT Amt/Qty FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                         " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " + ce.GetVAM_ProductCostElement_ID() +
                                         " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                         " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = 0";
                        }
                        else
                        {
                            sql = @"SELECT Amt/Qty FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                             " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " + ce.GetVAM_ProductCostElement_ID() +
                                             " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                             " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = " + cd.GetVAB_InvoiceLine_ID();
                        }
                        MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                        if (MRPrice == 0)
                        {
                            if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                         " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " +
                                         " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND VAF_Client_ID = " + ce.GetVAF_Client_ID() + ")" +
                                         " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                         " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = 0", null, Get_Trx())) > 0)
                            {
                                sql = @"SELECT Amt/Qty FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                             " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " +
                                             " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND VAF_Client_ID = " + ce.GetVAF_Client_ID() + ")" +
                                             " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                             " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = 0";
                            }
                            else
                            {
                                sql = @"SELECT Amt/Qty FROM VAM_ProductCostElementDetail WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() +
                                                 " AND VAB_AccountBook_ID = " + mas.GetVAB_AccountBook_ID() + " AND VAM_ProductCostElement_ID = " +
                                                 " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND VAF_Client_ID = " + ce.GetVAF_Client_ID() + ")" +
                                                 " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + M_ASI_ID + " AND VAM_Inv_InOutLine_ID =  " + cd.GetVAM_Inv_InOutLine_ID() +
                                                 " AND NVL(VAB_OrderLIne_ID , 0) = 0 AND NVL(VAB_InvoiceLine_ID , 0) = " + cd.GetVAB_InvoiceLine_ID();
                            }
                            MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }
                    }

                    MRPrice = Decimal.Round(MRPrice, mas.GetCostingPrecision());
                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPrice * GetQty())));
                    //}
                    //end
                }
                return cost.Save();
                #endregion
            }

            if (VAA_Asset_ID > 0)
            {
                qty = 1;
                amt = Decimal.Round(Decimal.Divide(GetAmt(), GetQty()), precision, MidpointRounding.AwayFromZero);

            }

            Decimal price = amt;
            if (Env.Signum(qty) != 0)
                price = Decimal.Round(Decimal.Divide(amt, qty), precision, MidpointRounding.AwayFromZero);

            if (windowName == "Product Cost IV")
            {
                #region Product Cost IV
                if (ce.IsAveragePO() || ce.IsLastPOPrice() || ce.IsWeightedAveragePO())
                {
                    return cost.Save();
                }

                amtWithSurcharge = Decimal.Add(GetAmt(), Decimal.Round(Decimal.Divide(Decimal.Multiply(GetAmt(), ce.GetSurchargePercentage()), 100), mas.GetCostingPrecision()));
                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amtWithSurcharge));

                if (ce.IsAverageInvoice())
                {
                    //if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    //{
                    //    cost.SetCurrentQty(0);
                    //}
                    //else
                    //{
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    //}

                    //cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentCostPrice(0);
                    }
                }
                else if (ce.IsLastInvoice())
                {
                    //if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    //{
                    //    cost.SetCurrentQty(0);
                    //}
                    //else
                    //{
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    //}
                    //cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    if (GetVAB_InvoiceLine_ID() > 0)
                    {
                        invoiceline = new MInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_Trx());
                        invoice = new MInvoice(GetCtx(), invoiceline.GetVAB_Invoice_ID(), Get_Trx());
                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            // this check is used to get previous invoice price from cost element detail 
                            // if invoice found then set that invoice price else 0
                            string sql = @"select * from (
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.VAB_AccountBook_id ,  ced.VAB_InvoiceLine_id , 
                                           ced.VAM_ProductCostElementDetail_id,  ced.VAM_ProductCostElement_ID,  row_number() over(order by ced.VAM_ProductCostElementDetail_id desc nulls last) rnm
                                           FROM VAM_ProductCostElementDetail ced inner join VAB_InvoiceLine il on il.VAB_InvoiceLine_id = ced.VAB_InvoiceLine_id
                                           inner join VAB_Invoice i on i.VAB_Invoice_id = il.VAB_Invoice_id 
                                           WHERE ced.VAB_InvoiceLine_id > 0 AND ced.qty > 0 AND ced.VAM_ProductCostElement_ID in ( " + ce.GetVAM_ProductCostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.VAB_AccountBook_ID = " + GetVAB_AccountBook_ID() +
                                           @" AND ced.VAM_Product_ID = " + GetVAM_Product_ID() + @" AND ced.VAF_Org_ID = " + Org_ID +
                                           @" AND NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.VAM_ProductCostElementDetail_id DESC ) where rnm <=1";
                            DataSet ds = DB.ExecuteDataset(sql, null, null);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count == 1)
                                {
                                    price = Decimal.Round(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["price"]), mas.GetCostingPrecision());

                                }
                                else
                                {
                                    price = 0;
                                }
                            }
                            else
                            {
                                price = 0;
                            }
                        }
                    }
                    cost.SetCurrentCostPrice(price);
                }
                else if (ce.IsWeightedAverageCost())
                {
                    // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                    if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    {
                        cost.SetCurrentQty(0);
                        cost.SetCurrentCostPrice(0);
                    }
                    else
                    {
                        price = Decimal.Round(Decimal.Divide(
                                               Decimal.Add(
                                               Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amtWithSurcharge),
                                               Decimal.Add(cost.GetCurrentQty(), qty))
                                               , precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    log.Finer("Inv - WeightedAverageCost - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    //if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    //{
                    //    cost.SetCurrentQty(0);
                    //}
                    //else
                    //{
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    //}
                    //cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                    if (cost.GetCurrentCostPrice() == 0)
                    {
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                }
                else if (ce.IsLifo() || ce.IsFifo())
                {
                    //if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    //{
                    //    cost.SetCurrentQty(0);
                    //}
                    //else
                    //{
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    //}
                    //cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                    decimal totalPrice = 0;
                    decimal totalQty = 0;
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        for (int j = 0; j < cQueue.Length; j++)
                        {
                            totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                            totalQty += cQueue[j].GetCurrentQty();
                        }
                        cost.SetCurrentCostPrice(Decimal.Round((totalPrice / totalQty), precision));
                    }
                    else if (cQueue.Length == 0)
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                }
                //change 3-5-2016
                if (ce.IsLifo() || ce.IsFifo() || ce.IsAverageInvoice() || ce.IsLastInvoice() || ce.IsStandardCosting() || ce.IsWeightedAverageCost())
                {
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                    //                                             mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, GetAmt(), GetQty());
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                    //                                             mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, amtWithSurcharge, GetQty());
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }

                return cost.Save();
                #endregion
            }

            if (windowName == "Product Cost IV Form")
            {
                #region Product Cost IV Form
                if (ce.IsAveragePO() || ce.IsLastPOPrice() || ce.IsWeightedAveragePO())
                {
                    return cost.Save();
                }

                amtWithSurcharge = Decimal.Add(GetAmt(), Decimal.Round(Decimal.Divide(Decimal.Multiply(GetAmt(), ce.GetSurchargePercentage()), 100), mas.GetCostingPrecision()));
                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amtWithSurcharge));

                if (ce.IsAverageInvoice())
                {
                    if (cd.GetVAB_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }

                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }

                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentCostPrice(0);
                    }
                }
                else if (ce.IsLastInvoice())
                {
                    if (cd.GetVAB_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    cost.SetCurrentCostPrice(price);
                }
                else if (ce.IsWeightedAverageCost())
                {
                    // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                    if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                    {
                        cost.SetCurrentQty(0);
                        cost.SetCurrentCostPrice(0);
                    }
                    else
                    {
                        price = Decimal.Round(Decimal.Divide(
                                               Decimal.Add(
                                               Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amtWithSurcharge),
                                               Decimal.Add(cost.GetCurrentQty(), qty))
                                               , precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    log.Finer("Inv - WeightedAverageCost - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (cd.GetVAB_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    if (cost.GetCurrentCostPrice() == 0)
                    {
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                }
                else if (ce.IsLifo() || ce.IsFifo())
                {
                    if (cd.GetVAB_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    decimal totalPrice = 0;
                    decimal totalQty = 0;
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        for (int j = 0; j < cQueue.Length; j++)
                        {
                            totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                            totalQty += cQueue[j].GetCurrentQty();
                        }
                        cost.SetCurrentCostPrice(Decimal.Round((totalPrice / totalQty), precision));
                    }
                    else if (cQueue.Length == 0)
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                }
                if (ce.IsLifo() || ce.IsFifo() || ce.IsAverageInvoice() || ce.IsLastInvoice() || ce.IsStandardCosting() || ce.IsWeightedAverageCost())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }

                return cost.Save();
                #endregion
            }


            /** All Costing Methods
            if (ce.isAverageInvoice())
            else if (ce.isAveragePO())
            else if (ce.isFifo())
            else if (ce.isLifo())
            else if (ce.isLastInvoice())
            else if (ce.isLastPOPrice())
            else if (ce.isStandardCosting())
            else if (ce.isUserDefined())
            else if (ce.IsWeightedAverageCost())
            else if (!ce.isCostingMethod())
            **/

            // calculate weighted Average cost from MR record if IscostCalculated is true on invoice linked with that MR
            //            if (windowName == "Material Receipt" && ce.IsWeightedAverageCost())
            //            {
            //                string sql = @"SELECT il.VAB_InvoiceLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice i  ON i.VAB_Invoice_ID = il.VAB_Invoice_ID
            //                               WHERE il.IsCostCalculated = 'Y' AND il.ISREVERSEDCOSTCALCULATED = 'N' AND i.DocStatus IN ('CO' , 'CL') AND il.IsActive = 'Y' 
            //                               AND il.VAM_Inv_InOutLine_ID = " + GetVAM_Inv_InOutLine_ID();
            //                int invLineId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, cd.Get_Trx()));
            //                if (invLineId > 0)
            //                {
            //                    invoiceline = new MInvoiceLine(GetCtx(), invLineId, cd.Get_Trx());
            //                    invoice = new MInvoice(GetCtx(), invoiceline.GetVAB_Invoice_ID(), cd.Get_Trx());
            //                    inoutline = new MInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
            //                    inout = new MInOut(GetCtx(), inoutline.GetVAM_Inv_InOut_ID(), Get_Trx());
            //                    string IsCostCalculated = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsCostCalculated FROM VAM_MatchInvoice WHERE VAB_InvoiceLine_ID = " + invLineId +
            //                        @" AND VAM_Inv_InOutLine_ID = " + GetVAM_Inv_InOutLine_ID(), null, cd.Get_Trx()));
            //                    if (IsCostCalculated == "N" && inout != null && ((inout.GetDescription() != null && !inout.GetDescription().Contains("{->")) || inout.GetDescription() == null))
            //                    {
            //                        // handle cost adjustment on normal loss
            //                        amt = Decimal.Round(Decimal.Divide(invoiceline.GetLineNetAmt(), product.IsCostAdjustmentOnLost() ? inoutline.GetMovementQty() : invoiceline.GetQtyInvoiced()), mas.GetCostingPrecision());
            //                        if (invoice.GetVAB_Currency_ID() != mas.GetVAB_Currency_ID())
            //                        {
            //                            amt = MConversionRate.Convert(GetCtx(), amt, invoice.GetVAB_Currency_ID(), mas.GetVAB_Currency_ID(),
            //                                                                        invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), cd.GetVAF_Client_ID(), cd.GetVAF_Org_ID());
            //                        }
            //                        amt = Decimal.Multiply(amt, qty);
            //                        price = Decimal.Round(Decimal.Divide(
            //                                                              Decimal.Add(
            //                                                              Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
            //                                                              Decimal.Add(cost.GetCurrentQty(), qty))
            //                                                              , precision, MidpointRounding.AwayFromZero);
            //                        cost.SetCurrentCostPrice(price);
            //                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
            //                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
            //                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
            //                        log.Finer("Inv - WeightedAverageCost - " + cost);

            //                        MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
            //                                                           mas, ce.GetVAM_ProductCostElement_ID(), "Invoice(Vendor)", cd, cost.GetCurrentCostPrice() * qty, qty);

            //                        DB.ExecuteQuery("UPDATE VAM_MatchInvoice SET IsCostCalculated = 'Y' WHERE VAB_InvoiceLine_ID = " + invLineId +
            //                        @" AND VAM_Inv_InOutLine_ID = " + GetVAM_Inv_InOutLine_ID(), null, cd.Get_Trx());

            //                        return cost.Save();
            //                    }
            //                }
            //            }

            //	*** Purchase Order Detail Record ***
            if (GetVAB_OrderLine_ID() != 0 && windowName == "Material Receipt")
            {
                #region Material Receipt with Purchase Order
                MVABOrderLine oLine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), null);
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                inoutline = new MInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
                inout = new MInOut(GetCtx(), inoutline.GetVAM_Inv_InOut_ID(), Get_Trx());
                if (ce.IsCostingMethod() && isReturnTrx && inout != null && inout.GetDescription() != null && !inout.GetDescription().Contains("{->")) // -ve Entry on completion of MR
                {
                    if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                    {
                        return false;
                        cost.SetCurrentQty(0);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                }
                else if (ce.IsCostingMethod() && !isReturnTrx && inout != null && inout.GetDescription() != null && inout.GetDescription().Contains("{->")) // +ve Entry Reverse Case
                {
                    if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                    {
                        return false;
                        cost.SetCurrentQty(0);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                }
                else if (ce.IsAveragePO())
                {
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    /**********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                    /**********************************/

                    log.Finer("PO - AveragePO - " + cost);
                }
                else if (ce.IsWeightedAveragePO())
                {
                    #region Weighted Av PO
                    // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                    if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                    {
                        return false;
                    }
                    else if (Decimal.Add(cost.GetCurrentQty(), qty) == 0)
                    {
                        cost.SetCurrentQty(0);
                    }
                    else
                    {
                        price = Decimal.Round(Decimal.Divide(
                                               Decimal.Add(
                                               Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                               Decimal.Add(cost.GetCurrentQty(), qty))
                                               , precision, MidpointRounding.AwayFromZero);
                        cost.SetCurrentCostPrice(price);
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    log.Finer("PO - WeightedAveragePO - " + cost);
                    #endregion
                }
                else if (ce.IsLastPOPrice())
                {
                    if (!isReturnTrx)
                    {
                        if (Env.Signum(qty) != 0)
                        {
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                            cost.SetCurrentCostPrice(cCosts);
                        }
                    }
                    cost.Add(amt, qty);

                    // this check is used to get previous invoice price from cost element detail 
                    // if invoice found then set that invoice price else 0
                    // this block is executed for reverse record
                    if (isReturnTrx)
                    {
                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            string sql = @"select * from (
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.VAB_AccountBook_id ,  ced.VAM_Inv_InOutLine_id , 
                                           ced.VAM_ProductCostElementDetail_id,  ced.VAM_ProductCostElement_ID,  row_number() over(order by ced.VAM_ProductCostElementDetail_id desc nulls last) rnm
                                           FROM VAM_ProductCostElementDetail ced inner join VAM_Inv_InOutLine il on il.VAM_Inv_InOutLine_id = ced.VAM_Inv_InOutLine_id
                                           inner join VAM_Inv_InOut i on i.VAM_Inv_InOut_id = il.VAM_Inv_InOut_id 
                                           WHERE ced.VAM_Inv_InOutLine_id > 0 AND ced.VAB_Orderline_ID > 0 AND ced.qty > 0 AND ced.VAM_ProductCostElement_ID in ( " + ce.GetVAM_ProductCostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.VAB_AccountBook_ID = " + GetVAB_AccountBook_ID() +
                                           @" AND ced.VAM_Product_ID = " + GetVAM_Product_ID() + @" AND ced.VAF_Org_ID = " + Org_ID +
                                           @" AND NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.VAM_ProductCostElementDetail_id DESC ) where rnm <=1";
                            DataSet ds = DB.ExecuteDataset(sql, null, null);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count == 1)
                                {
                                    price = Decimal.Round(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["price"]), mas.GetCostingPrecision());
                                }
                                else
                                {
                                    price = 0;
                                }
                            }
                            else
                            {
                                price = 0;
                            }
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    log.Finer("PO - LastPO - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    log.Finer("PO - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    Decimal cCosts = Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt);
                    Decimal qty1 = Decimal.Add(cost.GetCurrentQty(), qty);
                    if (qty1.CompareTo(Decimal.Zero) == 0)
                    {
                        qty1 = Decimal.One;
                    }
                    cCosts = Decimal.Round(Decimal.Divide(cCosts, qty1), precision, MidpointRounding.AwayFromZero);
                    cost.SetCurrentCostPrice(cCosts);
                    cost.Add(amt, qty);
                    log.Finer("PO - " + ce + " - " + cost);
                }
                else if (inout != null && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    if (ce.IsFifo() || ce.IsLifo())
                    {
                        decimal totalPrice = 0;
                        decimal totalQty = 0;
                        //MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        //    mas, cq_VAF_Org_ID, ce, Get_TrxName());
                        MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                        if (cQueue != null && cQueue.Length > 0)
                        {
                            for (int j = 0; j < cQueue.Length; j++)
                            {
                                totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                                totalQty += cQueue[j].GetCurrentQty();
                            }
                            cost.SetCurrentCostPrice(Decimal.Round((totalPrice / totalQty), precision));
                        }
                        else if (cQueue.Length == 0)
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                }
                //change 3-5-2016
                if (ce.IsAveragePO() || ce.IsLastPOPrice() || ce.IsWeightedAveragePO())
                {
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                    //                                             mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, amt, qty);
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                #endregion
            }

            //	*** AP Invoice Detail Record ***
            else if (GetVAB_InvoiceLine_ID() != 0 && windowName != "Material Receipt" && windowName != "Invoice(Vendor)-Return")
            {
                #region AP Invoice Detail Record
                bool isReturnTrx = Env.Signum(qty) < 0;
                if (GetVAM_Inv_InOutLine_ID() > 0 || GetVAB_OrderLine_ID() > 0)
                {
                    invoiceline = new MInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_Trx());
                    invoice = new MInvoice(GetCtx(), invoiceline.GetVAB_Invoice_ID(), Get_Trx());
                    // checking invoice contain reverse invoice ref or not
                }
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAB_LCostDistribution WHERE  VAB_InvoiceLine_ID = " + GetVAB_InvoiceLine_ID(), null, Get_Trx())) <= 0)
                {
                    if (GetVAB_OrderLine_ID() == 0) // if invoice created without orderline  then no impact on cost
                    {
                        // 20-4-2016
                        if (windowName == "Invoice(APC)")
                        {
                            if (ce.IsAverageInvoice() || ce.IsAveragePO())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                if (Env.Signum(cost.GetCumulatedQty()) != 0)
                                {
                                    price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                                    cost.SetCurrentCostPrice(price);
                                }
                                else
                                {
                                    cost.SetCurrentCostPrice(0);
                                }
                            }
                            else if (ce.IsWeightedAverageCost())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                price = Decimal.Round(Decimal.Divide(
                                        Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt), cost.GetCurrentQty()), precision);
                                cost.SetCurrentCostPrice(price);
                            }
                            else if (ce.IsFifo() || ce.IsLifo() || ce.IsStandardCosting() || ce.IsLastInvoice() || ce.IsLastPOPrice())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            }
                            //change 3-5-2016
                            MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                            mas, ce.GetVAM_ProductCostElement_ID(), "Invoice(APC)", cd, cost.GetCurrentCostPrice() * qty, qty);
                        }
                        return cost.Save();
                        // end
                    }
                }
                if (ce.IsAverageInvoice() && windowName != "Invoice(Customer)")
                {
                    if (invoice != null && !invoice.IsSOTrx() && invoice.IsReturnTrx()) // Invoice Vendor with Vendor RMA
                    {
                    }
                    else
                    {
                        #region Av Invoice
                        //if (!isReturnTrx)
                        //    cost.SetWeightedAverage(amt, qty);
                        //else
                        //    cost.Add(amt, qty);

                        /**********************************/
                        if (isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        //else if (isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") && windowName != "Invoice(Customer)")
                        //{
                        //    // change 9-5-2016
                        //    // when invoice (vendor) -> reverse ->  Acc. Amt & Current cost to update based on current cost else Invoice price
                        //    if (cost.GetCurrentCostPrice() != 0)
                        //    {
                        //        amt = cost.GetCurrentCostPrice() * qty;
                        //    }
                        //    cost.Add(amt, qty);
                        //    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        //    {
                        //        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        //        cost.SetCurrentCostPrice(price);
                        //    }
                        //    else
                        //    {
                        //        cost.SetCurrentCostPrice(0);
                        //    }
                        //}
                        else
                        {
                            cost.Add(amt, qty);
                            if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                            else
                            {
                                cost.SetCurrentCostPrice(0);
                            }

                            /**********************************/
                            log.Finer("Inv - AverageInv - " + cost);
                        }
                        #endregion
                    }
                }
                else if (ce.IsWeightedAverageCost() && windowName != "Invoice(Customer)")
                {
                    if (invoice != null && !invoice.IsSOTrx() && invoice.IsReturnTrx()) // Invoice Vendor with Vendor RMA
                    {
                    }
                    else
                    {
                        #region Weighted Av Invoice
                        if (isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else
                        {
                            // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else if (Decimal.Add(cost.GetCurrentQty(), qty) == 0)
                            {
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                price = Decimal.Round(Decimal.Divide(
                                                       Decimal.Add(
                                                       Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                       Decimal.Add(cost.GetCurrentQty(), qty))
                                                       , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            log.Finer("Inv - WeightedAverageCost - " + cost);
                        }
                        #endregion
                    }
                }
                else if ((ce.IsFifo() || ce.IsLifo()) && windowName != "Invoice(Customer)")
                {
                    #region Lifo / Fifo
                    //	Real ASI - costing level Org

                    // commented by Amit because cost queue is created from complete 16-12-2015
                    //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                    //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                    //cq.SetCosts(amt, qty, precision);
                    //cq.Save(Get_TrxName());

                    //end
                    //	Get Costs - costing level Org/ASI
                    decimal totalPrice = 0;
                    decimal totalQty = 0;
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        for (int j = 0; j < cQueue.Length; j++)
                        {
                            totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                            totalQty += cQueue[j].GetCurrentQty();
                        }
                        cost.SetCurrentCostPrice(Decimal.Round((totalPrice / totalQty), precision));
                    }
                    if (cQueue.Length == 0)
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                    if (invoice != null && ((invoice.IsSOTrx() && invoice.IsReturnTrx()) || (!invoice.IsSOTrx() && invoice.IsReturnTrx())))
                    {
                        // Nothing happen when Invoice Vendor is created  for Return to vendor
                        // Nothing happen when Invoice Customer is created  for Customer Return
                    }
                    else if ((isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->"))  // if Invoice is -ve and try to complete
                        || (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->")))    //if Invoice become +ve after reverse and its reverse entr) 
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    //else if (isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") && windowName != "Invoice(Customer)")
                    //{
                    //    // change 9-5-2016
                    //    // when invoice (vendor) -> reverse ->  Acc. Amt, Acc. Qty, On Hand Qty & Current cost to update based on current cost else Invoice price
                    //    if (cost.GetCurrentCostPrice() != 0)
                    //    {
                    //        amt = cost.GetCurrentCostPrice() * qty;
                    //    }
                    //    cost.Add(amt, qty);
                    //}
                    else
                    {
                        if (windowName != "Invoice(Customer)")
                            cost.Add(amt, qty);
                    }
                    log.Finer("Inv - FiFo/LiFo - " + cost);
                    #endregion
                }
                else if (ce.IsLastInvoice() && windowName != "Invoice(Customer)")
                {
                    if (invoice != null && !invoice.IsSOTrx() && invoice.IsReturnTrx()) // Invoice Vendor with Vendor RMA
                    {
                    }
                    else
                    {
                        #region last Invoice
                        if (isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->")) // -ve entry for completion
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->")) // +ve Entry for reverse
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        //else if (isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") && windowName != "Invoice(Customer)")
                        //{
                        //    // change 9-5-2016
                        //    // when invoice (vendor) -> reverse ->  Acc. Amt, Acc. Qty, On Hand Qty & Current cost to update based on current cost else Invoice price
                        //    if (cost.GetCurrentCostPrice() != 0)
                        //    {
                        //        amt = cost.GetCurrentCostPrice() * qty;
                        //    }
                        //    cost.Add(amt, qty);
                        //}
                        else
                        {
                            if (!isReturnTrx)
                            {
                                if (Env.Signum(qty) != 0)
                                    cost.SetCurrentCostPrice(price);
                                else
                                {
                                    Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                                    cost.SetCurrentCostPrice(cCosts);
                                }
                            }
                            cost.Add(amt, qty);

                            // this check is used to get previous invoice price from cost element detail 
                            // if invoice found then set that invoice price else 0
                            // this block is executed for reverse record
                            if (GetVAB_InvoiceLine_ID() > 0 && isReturnTrx)
                            {
                                invoiceline = new MInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_Trx());
                                invoice = new MInvoice(GetCtx(), invoiceline.GetVAB_Invoice_ID(), Get_Trx());
                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                {
                                    string sql = @"select * from (
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.VAB_AccountBook_id ,  ced.VAB_InvoiceLine_id , 
                                           ced.VAM_ProductCostElementDetail_id,  ced.VAM_ProductCostElement_ID,  row_number() over(order by ced.VAM_ProductCostElementDetail_id desc nulls last) rnm
                                           FROM VAM_ProductCostElementDetail ced inner join VAB_InvoiceLine il on il.VAB_InvoiceLine_id = ced.VAB_InvoiceLine_id
                                           inner join VAB_Invoice i on i.VAB_Invoice_id = il.VAB_Invoice_id 
                                           WHERE ced.VAB_InvoiceLine_id > 0 AND ced.VAM_Inv_InOutLine_ID > 0 AND ced.qty > 0 AND ced.VAM_ProductCostElement_ID in ( " + ce.GetVAM_ProductCostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.VAB_AccountBook_ID = " + GetVAB_AccountBook_ID() +
                                                   @" AND ced.VAM_Product_ID = " + GetVAM_Product_ID() + @" AND ced.VAF_Org_ID = " + Org_ID +
                                                   @" AND NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.VAM_ProductCostElementDetail_id DESC ) where rnm <=1";
                                    DataSet ds = DB.ExecuteDataset(sql, null, null);
                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                    {
                                        if (ds.Tables[0].Rows.Count == 1)
                                        {
                                            price = Decimal.Round(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["price"]), mas.GetCostingPrecision());
                                        }
                                        else
                                        {
                                            price = 0;
                                        }
                                    }
                                    else
                                    {
                                        price = 0;
                                    }
                                }
                                cost.SetCurrentCostPrice(price);
                            }
                        }
                        log.Finer("Inv - LastInv - " + cost);
                        #endregion
                    }
                }
                else if (ce.IsStandardCosting() && windowName != "Invoice(Customer)")
                {
                    if (invoice != null && !invoice.IsSOTrx() && invoice.IsReturnTrx()) // Invoice Vendor with Vendor RMA
                    {
                    }
                    else
                    {
                        #region Std Costing
                        if (isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                                cost.SetCurrentQty(0);
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        //else if (isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") && windowName != "Invoice(Customer)")
                        //{
                        //    // change 9-5-2016
                        //    // when invoice (vendor) -> reverse ->  Acc. Amt, Acc. Qty, On Hand Qty & Current cost to update based on current cost else Invoice price
                        //    if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                        //    {
                        //        cost.SetCurrentCostPrice(price);
                        //        //	seed initial price
                        //        if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                        //        {
                        //            cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                        //                    mas, Org_ID, ce.GetCostingMethod(), GetVAB_OrderLine_ID()));
                        //        }
                        //    }
                        //    if (cost.GetCurrentCostPrice() != 0)
                        //    {
                        //        amt = cost.GetCurrentCostPrice() * qty;
                        //    }
                        //    cost.Add(amt, qty);
                        //}
                        else
                        {
                            if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                            {
                                cost.SetCurrentCostPrice(price);
                                //	seed initial price
                                if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                                {
                                    cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                            mas, Org_ID, ce.GetCostingMethod(), GetVAB_OrderLine_ID()));
                                }
                            }
                            cost.Add(amt, qty);
                        }
                        log.Finer("Inv - Standard - " + cost);
                        #endregion
                    }
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    cost.Add(amt, qty);
                    log.Finer("Inv - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())		//	Cost Adjustments
                {
                    // when expected cost already calculated, and during actual adjustment - current qty not available then no effects comes
                    if (GetExpectedCostCalculated() && cost.GetCurrentQty() == 0)
                    {
                        return true;
                    }

                    Decimal cCosts = Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt);
                    // when expected cost already calculated, then not to add qty 
                    //Decimal qty1 = Decimal.Add(cost.GetCurrentQty(), GetExpectedCostCalculated() ? 0 : qty);

                    Decimal qty1 = MCost.GetproductCostAndQtyMaterial(cd.GetVAF_Client_ID(), cost.GetVAF_Org_ID(), cost.GetVAM_Product_ID()
                        , cost.GetVAM_PFeature_SetInstance_ID(), cost.Get_Trx(), cost.GetVAM_Warehouse_ID(), true);
                    if (qty1.CompareTo(Decimal.Zero) == 0)
                    {
                        qty1 = cost.GetCurrentQty();
                    }
                    if (qty1.CompareTo(Decimal.Zero) == 0)
                    {
                        qty1 = Decimal.One;
                    }
                    // need to divide cost with Current qty, because we already add qty in freight
                    cCosts = Decimal.Round(Decimal.Divide(cCosts, qty1), precision, MidpointRounding.AwayFromZero);
                    cost.SetCurrentCostPrice(cCosts);
                    // when expected cost already calculated, then not to add qty 
                    //cost.Add(amt, GetExpectedCostCalculated() ? 0 : qty);
                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), GetExpectedCostCalculated() ? 0 : qty));
                    log.Finer("Inv - none - " + cost);
                }
                //change 3-5-2016
                if (ce.IsWeightedAverageCost() && windowName != "Invoice(Customer)")
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                else if ((ce.IsFifo() || ce.IsLifo() || ce.IsStandardCosting() || ce.IsLastInvoice() || ce.IsAverageInvoice()) && windowName != "Invoice(Customer)")
                {
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                    //                                mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, amt, qty);

                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                #endregion
            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetVAM_Inv_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetVAM_InvTrf_Line_ID() != 0
                || GetVAM_InventoryLine_ID() != 0
                || GetVAM_ProductionLine_ID() != 0
                || GetVAB_ProjectSupply_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetVAMFG_M_WrkOdrTrnsctionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0
                || Util.GetValueOfInt(Get_Value("VAFAM_AssetDisposal_ID")) != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                if (GetVAM_Inv_InOutLine_ID() > 0)
                {
                    inoutline = new MInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
                    inout = new MInOut(GetCtx(), inoutline.GetVAM_Inv_InOut_ID(), Get_Trx());
                }
                if (GetVAM_InvTrf_Line_ID() > 0)
                {
                    movementline = new MMovementLine(GetCtx(), GetVAM_InvTrf_Line_ID(), Get_Trx());
                    movement = new MMovement(GetCtx(), movementline.GetVAM_InventoryTransfer_ID(), Get_Trx());
                }

                // if current cost price avialble then add that amount else the same scenario
                if (windowName.Equals("Shipment"))
                {
                    amt = cost.GetCurrentCostPrice() * qty;
                }
                else if (windowName.Equals("Invoice(Vendor)-Return"))
                {
                    // In case of Invoice against Return To Vendor - impacts come with Invoice Amount
                    amt = GetAmt();
                }
                else if (windowName.Equals("Return To Vendor") && GetVAB_OrderLine_ID() > 0)
                {
                    amt = GetAmt();
                }
                else if (!windowName.Equals("PE-FinishGood") && cost.GetCurrentCostPrice() != 0)
                {
                    amt = cost.GetCurrentCostPrice() * qty;
                }

                //
                if (ce.IsAverageInvoice())
                {
                    #region Av. invoice

                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (addition)
                    {
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            // with this impact we take an impact on current cost price which is Accumulation Amt/Accumulation Qty
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                //if (Env.Signum(cost.GetCumulatedQty()) != 0)
                                //{
                                //    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                                //}
                                //else
                                //{
                                //    cost.SetCurrentCostPrice(0);
                                //}
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                // get cost based on Accounting Schema
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                //if (primaryActSchemaCurrencyID != mas.GetVAB_Currency_ID())
                                //{
                                //    costCombination = MConversionRate.Convert(movement.GetCtx(), costCombination, primaryActSchemaCurrencyID, mas.GetVAB_Currency_ID(),
                                //                                     movement.GetMovementDate(), 0, movement.GetVAF_Client_ID(), movement.GetVAF_Org_ID());
                                //}
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                            if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            }
                            cost.SetCurrentCostPrice(price);
                            //end
                        }
                        else
                        {
                            // change 28-4-2016
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            //end
                            cost.Add(amt, qty);
                            if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            }
                            cost.SetCurrentCostPrice(price);
                        }
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        //end
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                        // now we will get cost(CurrentCostPrice) from the document line itself - product come with that price on To Warehouse
                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetVAF_Client_ID(), cost.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), cost.GetVAM_Product_ID(),
                        //       cost.GetVAM_PFeature_SetInstance_ID(), cost.Get_Trx(), cost.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));
                        //}
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(movementline.GetCurrentCostPrice(), qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            Decimal convertAmount = MVABExchangeRate.Convert(GetCtx(), movementline.GetCurrentCostPrice(),
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(convertAmount, qty)));
                        }

                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to decrease ( Accumulation Qty & Accumulation Amt)
                        // with this impact we take an impact on current cost price which is Accumulation Amt/Accumulation Qty
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            //if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            //{
                            //    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                            //}
                            //else
                            //{
                            //    cost.SetCurrentCostPrice(0);
                            //}
                        }
                    }
                    else
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    /**********************************/
                    log.Finer("QtyAdjust - AverageInv - " + cost);
                    #endregion
                }
                else if (ce.IsWeightedAverageCost())
                {
                    #region Weighted Av. invoice
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        if (Decimal.Add(cost.GetCurrentQty(), qty) != 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(
                                Decimal.Add(
                                Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                amt),
                                Decimal.Add(cost.GetCurrentQty(), qty)), precision, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }

                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else if (addition)
                    {
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return"))// || windowName == "Return To Vendor"
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            // with this impact we take an impact on current cost price 
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                //if (Env.Signum(Decimal.Add(cost.GetCurrentQty(), qty)) != 0)
                                //{
                                //    cost.SetCurrentCostPrice(Decimal.Round(
                                //        Decimal.Divide(
                                //        Decimal.Add(
                                //        Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty())
                                //        , amt)
                                //        , Decimal.Add(cost.GetCurrentQty(), qty))
                                //        , precision, MidpointRounding.AwayFromZero));
                                //}
                                //else
                                //{
                                //    cost.SetCurrentCostPrice(0);
                                //}
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            // Quantity to be increased
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {

                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));

                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                                  Decimal.Multiply(costCombination, qty)),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));

                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                                  Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }

                            // Quantity to be increased into Moving Org
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Physical Inventory"))
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));

                            // Quantity to be increased
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);

                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Shipment"))
                        {
                            // Quantity to be decreased
                            if (Decimal.Subtract(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Subtract(cost.GetCurrentQty(), qty));
                            }
                        }
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // no to do because we are not taken any impact on MR Completion costing
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                        // with this impact we take an impact on current cost price 
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            //if (Env.Signum(Decimal.Add(cost.GetCurrentQty(), qty)) != 0)
                            //{
                            //    cost.SetCurrentCostPrice(Decimal.Round(
                            //        Decimal.Divide(
                            //        Decimal.Add(
                            //        Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty())
                            //        , amt)
                            //        , Decimal.Add(cost.GetCurrentQty(), qty))
                            //        , precision, MidpointRounding.AwayFromZero));
                            //}
                            //else
                            //{
                            //    cost.SetCurrentCostPrice(0);
                            //}
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse

                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                        if (Decimal.Add(cost.GetCurrentQty(), qty) > 0)
                        {
                            price = Decimal.Round(Decimal.Divide(
                                              Decimal.Add(
                                              Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                              Decimal.Multiply(affectedAmount, qty)),
                                              Decimal.Add(cost.GetCurrentQty(), qty))
                                              , precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetVAF_Client_ID(), cost.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), cost.GetVAM_Product_ID(),
                        //       cost.GetVAM_PFeature_SetInstance_ID(), cost.Get_Trx(), cost.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));

                        //    // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                        //    if (Decimal.Add(cost.GetCurrentQty(), qty) > 0)
                        //    {
                        //        price = Decimal.Round(Decimal.Divide(
                        //                          Decimal.Add(
                        //                          Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                        //                          Decimal.Multiply(costCombination, qty)),
                        //                          Decimal.Add(cost.GetCurrentQty(), qty))
                        //                          , precision, MidpointRounding.AwayFromZero);
                        //        cost.SetCurrentCostPrice(price);
                        //    }
                        //    else
                        //    {
                        //        cost.SetCurrentCostPrice(0);
                        //    }
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));

                        //    // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                        //    if (Decimal.Add(cost.GetCurrentQty(), qty) > 0)
                        //    {
                        //        price = Decimal.Round(Decimal.Divide(
                        //                          Decimal.Add(
                        //                          Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                        //                          Decimal.Multiply(cost.GetCurrentCostPrice(), qty)),
                        //                          Decimal.Add(cost.GetCurrentQty(), qty))
                        //                          , precision, MidpointRounding.AwayFromZero);
                        //        cost.SetCurrentCostPrice(price);
                        //    }
                        //    else
                        //    {
                        //        cost.SetCurrentCostPrice(0);
                        //    }
                        //}

                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else if (!windowName.Equals("Return To Vendor"))
                    {
                        // for Physical Inventory / Internal use Inventory / Inventory move / Production Execution / Asset Disposal
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    /**********************************/
                    log.Finer("QtyAdjust - WeightedAverageInv - " + cost);
                    #endregion
                }
                else if (ce.IsAveragePO())
                {
                    #region Av. Purchase Order
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (addition)
                    {
                        #region when qty > 0
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                                // Accumulation qty / amt and Currentcostprice affceted
                                if (windowName.Equals("Return To Vendor") && GetVAB_OrderLine_ID() > 0)
                                {
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                    //if (Env.Signum(cost.GetCumulatedQty()) != 0)
                                    //{
                                    //    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                                    //}
                                }
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }                    
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                            if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            }
                            cost.SetCurrentCostPrice(price);
                        }
                        else if (!windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            cost.Add(amt, qty);
                            if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            }
                            cost.SetCurrentCostPrice(price);
                        }
                        #endregion
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        //end
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetVAF_Client_ID(), cost.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), cost.GetVAM_Product_ID(),
                        //       cost.GetVAM_PFeature_SetInstance_ID(), cost.Get_Trx(), cost.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));
                        //}

                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else if (windowName.Equals("Return To Vendor"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        if (GetVAB_OrderLine_ID() > 0)
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            //if (Env.Signum(cost.GetCumulatedQty()) != 0)
                            //{
                            //    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero));
                            //}
                        }
                    }
                    else if (!windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    /**********************************/

                    log.Finer("QtyAdjust - AveragePO - " + cost);
                    #endregion
                }
                else if (ce.IsWeightedAveragePO())
                {
                    #region Weighted Av. PO
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        if (Env.Signum(Decimal.Add(cost.GetCurrentQty(), qty)) != 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(
                                Decimal.Add(
                                Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                amt),
                                Decimal.Add(cost.GetCurrentQty(), qty)), precision, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }

                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else if (addition)
                    {
                        #region addition > 0
                        if (windowName.Equals("Shipment") &&
                            (inout.GetReversalDoc_ID() > 0 || (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Shipment"))
                        {
                            // Quantity to be decreased
                            if (Decimal.Subtract(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Subtract(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Return To Vendor"))
                        {
                            // when we reverse  Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            // with this impact we take an impact on current cost price 
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (GetVAB_OrderLine_ID() > 0)
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                //if (Env.Signum(Decimal.Add(cost.GetCurrentQty(), qty)) != 0)
                                //{
                                //    cost.SetCurrentCostPrice(Decimal.Round(
                                //        Decimal.Divide(
                                //        Decimal.Add(
                                //        Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty())
                                //        , amt)
                                //        , Decimal.Add(cost.GetCurrentQty(), qty))
                                //        , precision, MidpointRounding.AwayFromZero));
                                //}
                                //else
                                //{
                                //    cost.SetCurrentCostPrice(0);
                                //}
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            // Quantity to be increased
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move") && movement != null &&
                                    movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));

                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                                  Decimal.Multiply(costCombination, qty)),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));

                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                                  Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                            // Quantity to be increased into Moving Org
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Physical Inventory"))
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));

                            // Quantity to be increased
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                                price = Decimal.Round(Decimal.Divide(
                                                  Decimal.Add(
                                                  Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                  Decimal.Add(cost.GetCurrentQty(), qty))
                                                  , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);

                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        #endregion
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // no to do because we are not taken any impact on Independent MR Completion costing
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // nothing haapne from invoice record
                    }
                    else if (windowName.Equals("Return To Vendor"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                        // with this impact we take an impact on current cost price 
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else if (GetVAB_OrderLine_ID() > 0)
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            // not to take impact on cost during return
                            //if (Env.Signum(Decimal.Add(cost.GetCurrentQty(), qty)) != 0)
                            //{
                            //    cost.SetCurrentCostPrice(Decimal.Round(
                            //        Decimal.Divide(
                            //        Decimal.Add(
                            //        Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty())
                            //        , amt)
                            //        , Decimal.Add(cost.GetCurrentQty(), qty))
                            //        , precision, MidpointRounding.AwayFromZero));
                            //}
                            //else
                            //{
                            //    cost.SetCurrentCostPrice(0);
                            //}
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else if (windowName.Equals("Inventory Move") && movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                    {
                        // reverse entry of Inventory Move
                        // impact on qty , cummulative qty , cumulative amt , current cost price
                        // this is to warehouse
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        // Formula : ((CurrentQty * CurrentCostPrice) + (amt * qty)) / (CurrentQty + qty)
                        if (Decimal.Add(cost.GetCurrentQty(), qty) > 0)
                        {
                            price = Decimal.Round(Decimal.Divide(
                                              Decimal.Add(
                                              Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()),
                                              Decimal.Multiply(affectedAmount, qty)),
                                              Decimal.Add(cost.GetCurrentQty(), qty))
                                              , precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else
                    {
                        // for Physical Inventory / Internal use Inventory / Inventory move / Production Execution
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    /**********************************/
                    log.Finer("QtyAdjust - WeightedAveragePO - " + cost);
                    #endregion
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    #region Lifo / Fifo
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    else if (addition)
                    {
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }                    
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                        }
                        else if (!windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            cost.Add(amt, qty);
                        }
                        //	Real ASI - costing level Org
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                        //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                        //cq.SetCosts(amt, qty, precision);
                        //cq.Save();
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetVAF_Client_ID(), cost.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), cost.GetVAM_Product_ID(),
                        //       cost.GetVAM_PFeature_SetInstance_ID(), cost.Get_Trx(), cost.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));
                        //}
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        }
                    }
                    else
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        //	Adjust Queue - costing level Org/ASI
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue.AdjustQty(product, M_ASI_ID,
                        //    mas, Org_ID, ce, Decimal.Negate(qty), Get_TrxName());
                    }
                    //	Get Costs - costing level Org/ASI
                    decimal totalPrice = 0;
                    decimal totalQty = 0;
                    MCostQueue[] cQueue;
                    if (windowName == "Inventory Move")
                    {
                        cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                            mas, cq_VAF_Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                    }
                    else
                    {
                        cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                            mas, Org_ID, ce, Get_TrxName(), cost.GetVAM_Warehouse_ID());
                    }
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        for (int j = 0; j < cQueue.Length; j++)
                        {
                            totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                            totalQty += cQueue[j].GetCurrentQty();
                        }
                        cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(totalPrice, totalQty), precision));
                    }
                    else
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                    log.Finer("QtyAdjust - FiFo/Lifo - " + cost);
                    #endregion
                }
                else if (ce.IsLastInvoice())
                {
                    #region Last Invoice
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    else if (addition)
                    {
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory")|| windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }                     
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                        }
                        else
                        {
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            cost.Add(amt, qty);
                        }
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        cost.Add(amt, qty);
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                        //       costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                        //}
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to decrease ( Accumulation Qty & Accumulation Amt)
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        }
                    }
                    else
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    log.Finer("QtyAdjust - LastInv - " + cost);
                    #endregion
                }
                else if (ce.IsLastPOPrice())
                {
                    #region Last PO
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    else if (addition)
                    {
                        #region when qty > 0
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                                //Accumulation qty / amt  affeceted
                                if (windowName.Equals("Return To Vendor") && GetVAB_OrderLine_ID() > 0)
                                {
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                }
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory")|| windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }                    
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                        }
                        else if (!windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            cost.Add(amt, qty);
                        }
                        #endregion
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        cost.Add(amt, qty);
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                        //       costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                        //}
                    }
                    else if (windowName.Equals("Return To Vendor"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        if (GetVAB_OrderLine_ID() > 0)
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                        }
                    }
                    else if (!windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    log.Finer("QtyAdjust - LastPO - " + cost);
                    #endregion
                }
                else if (ce.IsStandardCosting())
                {
                    #region Std Costing
                    if (windowName.Equals("PE-FinishGood"))
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                            cost.SetCurrentCostPrice(price);
                    }
                    else if (addition)
                    {
                        if (windowName.Equals("Shipment") && inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        if (windowName.Equals("Customer Return") || windowName.Equals("Return To Vendor"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Invoice(Vendor)-Return"))
                        {
                            // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                            // with this impact we take an impact on current cost price as 0 if Accumulation Qty become 0 
                            if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                //if (Env.Signum(cost.GetCumulatedQty()) == 0)
                                //{
                                //    cost.SetCurrentCostPrice(0);
                                //}
                            }
                        }
                        else if (windowName.Equals("Internal Use Inventory") || windowName.Equals("AssetDisposal"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                      
                        else if (windowName.Equals("Production Execution"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                        {
                            // reverse entry of Inventory Move
                            // only impact on qty 
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (windowName.Equals("Inventory Move"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                                   costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                            }
                            else
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costFrom.GetCurrentCostPrice(), qty)));
                            }
                            if (Env.Signum(cost.GetCumulatedQty()) != 0 && Env.Signum(cost.GetCurrentCostPrice()) == 0)
                            {
                                price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                        }
                        else
                        {
                            // if current cost price avialble then add that amount else the same scenario
                            if (cost.GetCurrentCostPrice() != 0)
                            {
                                amt = cost.GetCurrentCostPrice() * qty;
                            }
                            cost.Add(amt, qty);
                            //	Initial
                            if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                                cost.SetCurrentCostPrice(price);
                        }
                    }
                    else if (windowName.Equals("Material Receipt"))
                    {
                        // if current cost price avialble then add that amount else the same scenario
                        if (cost.GetCurrentCostPrice() != 0)
                        {
                            amt = cost.GetCurrentCostPrice() * qty;
                        }
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                                cost.SetCurrentCostPrice(price);
                        }
                        else
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else if (movement != null && movement.GetDescription() != null && movement.GetDescription().Contains("{->") && windowName.Equals("Inventory Move"))
                    {
                        // reverse entry of Inventory Move
                        //  impact on qty , cummulative qty , cumulative amt , current cost price
                        //this is to warehouse
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));

                        Decimal affectedAmount = movementline.GetCurrentCostPrice();
                        if (mas.GetVAB_Currency_ID() == GetCtx().GetContextAsInt("$VAB_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MVABExchangeRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$VAB_Currency_ID"), mas.GetVAB_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetVAF_Client_ID(), cost.GetVAF_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetVAM_ProductCostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetVAF_Client_ID(), costFrom.GetVAF_Org_ID(), mas.GetVAB_AccountBook_ID(), costFrom.GetVAM_Product_ID(),
                        //       costFrom.GetVAM_PFeature_SetInstance_ID(), costFrom.Get_Trx(), costFrom.GetVAM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));
                        //}

                        if (Env.Signum(cost.GetCumulatedQty()) != 0 && Env.Signum(cost.GetCurrentCostPrice()) == 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                            cost.SetCurrentCostPrice(price);
                        }
                    }
                    else if (windowName.Equals("Invoice(Vendor)-Return"))
                    {
                        // when we reverse Invoice which is against Return To Vendor then we have to increase ( Accumulation Qty & Accumulation Amt)
                        // with this impact we take an impact on current cost price as 0 if Accumulation Qty become 0 
                        if (Decimal.Add(cost.GetCumulatedQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            //if (Env.Signum(cost.GetCumulatedQty()) == 0)
                            //{
                            //    cost.SetCurrentCostPrice(0);
                            //}
                        }
                    }
                    else
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    log.Finer("QtyAdjust - Standard - " + cost);
                    #endregion
                }
                else if (ce.IsUserDefined())
                {
                    #region User Defined
                    //	Interface
                    if (addition)
                        cost.Add(amt, qty);
                    else
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                        {
                            return false;
                        }
                        else
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    log.Finer("QtyAdjust - UserDef - " + cost);
                    #endregion
                }
                else if (!ce.IsCostingMethod())
                {
                    //	Should not happen
                    log.Finer("QtyAdjust - ?none? - " + cost);
                }
                else
                {
                    log.Warning("QtyAdjust - " + ce + " - " + cost);
                }

                // if current cost price avialble then add that amount else the same scenario
                if (cost.GetCurrentCostPrice() != 0)
                {
                    amt = cost.GetCurrentCostPrice() * qty;
                }

                if (ce.IsLastInvoice() || ce.IsLastPOPrice() || ce.IsWeightedAverageCost() || ce.IsWeightedAveragePO())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                else if (!ce.IsWeightedAverageCost())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetVAM_ProductCostElement_ID(), windowName, cd, amt, qty);
                }
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }
            if (VAA_Asset_ID != 0)
            {
                cost.SetIsAssetCost(true);
            }
            return cost.Save();
        }

        /// <summary>
        /// This function is used to calculate cost combination
        /// </summary>
        /// <param name="cd">cost detail</param>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">attribute set instance</param>
        /// <param name="cq_VAF_Org_ID">org</param>
        /// <param name="windowName">window name</param>
        /// <param name="optionalStrcc">optional para : process or window</param>
        /// <returns>true, when calculated</returns>
        public bool CreateCostForCombination(MCostDetail cd, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, int cq_VAF_Org_ID, string windowName, string optionalStrcc = "process")
        {
            string sql;
            int VAF_Org_ID = 0;
            // Get Org based on Costing Level
            dynamic pc = null;
            String cl = null;
            MCostElement ce = null;
            string costingMethod = null;
            int costElementId1 = 0;
            Decimal AccQty = 0;
            int VAM_Warehouse_ID = 0;
            if (product != null)
            {
                pc = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pc != null)
                {
                    cl = pc.GetCostingLevel();
                    costingMethod = pc.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costElementId1 = pc.GetVAM_ProductCostElement_ID();
                    }
                }
            }
            if (cl == null)
            {
                cl = acctSchema.GetCostingLevel();
            }
            if (costingMethod == null)
            {
                costingMethod = acctSchema.GetCostingMethod();
                if (costingMethod == "C")
                {
                    costElementId1 = acctSchema.GetVAM_ProductCostElement_ID();
                }
            }
            if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_BatchLot)
            {
                VAF_Org_ID = 0;
                // during Inventory Move, not to calculate cost combination
                if (windowName.Equals("Inventory Move"))
                    return true;
            }
            else
            {
                VAF_Org_ID = cd.GetVAF_Org_ID();
            }
            if (cl != MProductCategory.COSTINGLEVEL_BatchLot && cl != MProductCategory.COSTINGLEVEL_OrgPlusBatch && cl != MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_ASI_ID = 0;
            }
            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                VAM_Warehouse_ID = cd.GetVAM_Warehouse_ID();
            }

            // Freight Distribution
            if (!FreightDistribution(windowName, cd, acctSchema, VAF_Org_ID, product, M_ASI_ID, VAM_Warehouse_ID))
            {
                return false;
            }

            // when we complete a record, and costing method is not any combination, then no need to calculate
            if (optionalStrcc == "window" && costingMethod != "C")
            {
                return true;
            }

            // Get Cost element of Cost Combination type
            sql = @"SELECT ce.VAM_ProductCostElement_ID ,  ce.Name ,  cel.lineno ,  cel.m_ref_costelement , 
                      (SELECT CASE  WHEN costingmethod IS NOT NULL THEN 1  ELSE 0 END  FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_id = CAST(cel.M_Ref_CostElement AS INTEGER) ) AS iscostMethod 
                            FROM VAM_ProductCostElement ce INNER JOIN VAM_ProductCostElementLine cel ON ce.VAM_ProductCostElement_ID = cel.VAM_ProductCostElement_ID "
                          + "WHERE ce.VAF_Client_ID=" + GetVAF_Client_ID()
                          + " AND ce.IsActive='Y' AND ce.CostElementType='C' AND cel.IsActive='Y' ";
            if (optionalStrcc == "window" && costingMethod == "C")
            {
                sql += " AND ce.VAM_ProductCostElement_ID = " + costElementId1;
            }
            sql += "ORDER BY ce.VAM_ProductCostElement_ID , iscostMethod DESC";
            DataSet ds = new DataSet();
            ds = DB.ExecuteDataset(sql, null, null);
            try
            {
                MCost costCombination = null;
                MCost cost = null;
                int costElementId = 0;
                bool isCurrentCostprice = true; // is used to calculate - Current cost price for combination cost or not

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    MMovementLine moveline = null;
                    // Cost combination not calculate for this Transaction
                    if (windowName.Equals("Return To Vendor") || windowName.Equals("Shipment") ||
                        windowName.Equals("Internal Use Inventory") || windowName.Equals("Production Execution") || windowName.Equals("AssetDisposal"))
                    {
                        isCurrentCostprice = false;
                    }
                    else if (windowName.Equals("Inventory Move"))
                    {
                        // cost combination not calculated when Transaction is Origna (not reverse transaction) and for which org we are reducing product
                        moveline = new MMovementLine(GetCtx(), cd.GetVAM_InvTrf_Line_ID(), Get_Trx());
                        if (moveline.Get_ColumnIndex("ReversalDoc_ID") > 0 && moveline.GetReversalDoc_ID() == 0 && cd.GetQty() < 0)
                            isCurrentCostprice = false;
                    }

                    // Update Landed cost with Actual qty 
                    //if (windowName == "Shipment" ||
                    //    windowName == "Internal Use Inventory" || windowName == "Physical Inventory" ||
                    //    windowName == "Production Execution" ||
                    //    windowName == "Inventory Move")
                    //{
                    //    if (windowName == "Inventory Move" && moveline != null && moveline.Get_ColumnIndex("ReversalDoc_ID") > 0 && moveline.GetReversalDoc_ID() > 0 && cd.GetQty() < 0)
                    //    {
                    //        // when we reverse Inventory move, for those cost detail which decrease the QTY, not to decrease current qty for Landed cost like Feight etc
                    //    }
                    //    else
                    //    {
                    //        // UpdateFreightWithActualQty(ds, VAF_Org_ID, cd, acctSchema, product, M_ASI_ID, windowName, VAM_Warehouse_ID);
                    //    }
                    //}

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        costCombination = MCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]), VAM_Warehouse_ID);

                        //If cost combination is already calculated on completion, then not to re-calculate through process
                        if (windowName.Equals("LandedCostAllocation"))
                        {
                            costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]);
                            isCurrentCostprice = true;
                        }

                        if (isCurrentCostprice)
                        {
                            costCombination.SetCurrentCostPrice(0);
                        }
                        costCombination.SetCurrentQty(0);
                        costCombination.SetCumulatedAmt(0);
                        costCombination.SetCumulatedQty(0);
                        costCombination.Save();
                    }
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]);
                        }
                        if (costElementId != Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]))
                        {
                            if (windowName == "Inventory Move" &&
                                (cl.Equals(MProductCategory.COSTINGLEVEL_Client) || cl.Equals(MProductCategory.COSTINGLEVEL_BatchLot)))
                            {
                                // do not create Cost combination entry in case of ineventory move and costing level is client or Batch/lot
                            }
                            else if (windowName.Equals("LandedCostAllocation") && (cl == "C" || cl == "B"))
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), 0, product, M_ASI_ID,
                                              acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                            else
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                               acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                        }
                        costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]);

                        //If cost combination is already calculated on completion, then not to re-calculate through process
                        if (windowName.Equals("LandedCostAllocation"))
                        {
                            isCurrentCostprice = true;
                        }

                        // created object of Cost elemnt for checking iscalculated = true/ false
                        ce = MCostElement.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]));

                        costCombination = MCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]), VAM_Warehouse_ID);
                        cost = MCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]), VAM_Warehouse_ID);
                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["iscostMethod"]) == 1)
                        {
                            AccQty = cost.GetCurrentQty();
                        }

                        // if m_ref_costelement is of Freight type then current cost against this record is :: 
                        // Formula : (Freight Current Cost * Freight Current Qty) / Curent Qty (whose costing method is available)
                        if (isCurrentCostprice)
                        {
                            //costCombination.SetCurrentCostPrice(Decimal.Round(Decimal.Add(costCombination.GetCurrentCostPrice(),
                            //    Util.GetValueOfInt(ds.Tables[0].Rows[i]["iscostMethod"]) == 1 ? cost.GetCurrentCostPrice() :
                            //    Decimal.Divide(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), AccQty == 0 ? 1 : AccQty))
                            //    , acctSchema.GetCostingPrecision()));
                            costCombination.SetCurrentCostPrice(Decimal.Round(Decimal.Add(costCombination.GetCurrentCostPrice(),
                                                                cost.GetCurrentCostPrice()), acctSchema.GetCostingPrecision()));
                        }

                        costCombination.SetCumulatedAmt(Decimal.Add(costCombination.GetCumulatedAmt(), cost.GetCumulatedAmt()));
                        // if calculated = true then we added qty else not and costing method is Standard Costing
                        if (ce.IsCalculated() || ce.GetCostingMethod() == MCostElement.COSTINGMETHOD_StandardCosting)
                        {
                            costCombination.SetCurrentQty(Decimal.Add(costCombination.GetCurrentQty(), cost.GetCurrentQty()));
                            costCombination.SetCumulatedQty(Decimal.Add(costCombination.GetCumulatedQty(), cost.GetCumulatedQty()));
                        }
                        costCombination.Save();
                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            if (windowName == "Inventory Move" && (cl == "C" || cl == "B"))
                            {
                                // do not create Cost combination entry in case of ineventory move and costing level is client or Batch/lot
                            }
                            else if (windowName == "LandedCostAllocation" && (cl == "C" || cl == "B"))
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), 0, product, M_ASI_ID,
                                              acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                            else
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product, M_ASI_ID,
                                           acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                        }
                    }
                }
            }
            catch
            {
                _log.Info("Error occured during CreateCostForCombination.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// This funcion is used to distribute freight into current quantity
        /// </summary>
        /// <param name="windowName">window Name</param>
        /// <param name="cd">cost detail object</param>
        /// <param name="acctSchema">accounting schema object</param>
        /// <param name="VAF_Org_ID">organization</param>
        /// <param name="product">product object</param>
        /// <param name="M_ASI_ID">Attribute set instance</param>
        /// <param name="VAM_Warehouse_ID">warehouse reference</param>
        /// <returns>true when success</returns>
        public bool FreightDistribution(String windowName, MCostDetail cd, MVABAccountBook acctSchema, int VAF_Org_ID, MProduct product, int M_ASI_ID, int VAM_Warehouse_ID)
        {
            // is used to get current qty of defined costing method on Product category or Accounting schema
            Decimal Qty = MCost.GetproductCostAndQtyMaterial(cd.GetVAF_Client_ID(), VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, cd.Get_Trx(), VAM_Warehouse_ID, true);
            if (Qty == 0 && cd.GetVAM_ProductCostElement_ID() > 0)
            {
                // check, is this kind of Custome or Freight (bypass)
                int count = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_ID = " + cd.GetVAM_ProductCostElement_ID() +
                           @" AND CostElementType = '" + X_VAM_ProductCostElement.COSTELEMENTTYPE_Material + "' AND CostingMethod IS NULL"));
                if (count > 0)
                {
                    return true;
                }
            }

            bool isReversed = false;
            if (windowName.Equals("Inventory Move"))
            {
                isReversed = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM VAM_InvTrf_Line WHERE VAM_InvTrf_Line_ID = "
                            + cd.GetVAM_InvTrf_Line_ID(), null, cd.Get_Trx())) > 0 ? true : false;
            }

            // Get Element which belongs to Landed Cost
            String sql = "SELECT VAM_ProductCostElement_ID , Name FROM VAM_ProductCostElement WHERE CostElementType = '" + MCostElement.COSTELEMENTTYPE_Material +
                        @"' AND CostingMethod IS NULL AND IsActive = 'Y' AND VAF_Client_ID = " + cd.GetVAF_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql, null, cd.Get_Trx());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                MCost cost = null;
                Decimal currentCost = 0.0M;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cost = MCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, Convert.ToInt32(ds.Tables[0].Rows[i]["VAM_ProductCostElement_ID"]), VAM_Warehouse_ID);
                    if ((cd.GetQty() < 0 ||
                        windowName.Equals("Internal Use Inventory") ||
                        windowName.Equals("AssetDisposal") ||
                        windowName.Equals("Physical Inventory") ||
                        windowName.Equals("Return To Vendor") ||
                        windowName.Equals("Shipment") ||
                        windowName.Equals("Production Execution") ||
                        windowName.Equals("Inventory Move") ||
                        windowName.Equals("Invoice(Vendor)-Return"))
                        // 
                        && !(windowName.Equals("Material Receipt") ||
                             windowName.Equals("Customer Return") ||
                             windowName.Equals("PE-FinishGood") ||
                             windowName.Equals("Invoice(Vendor)") ||
                             (windowName.Equals("Inventory Move") && ((cd.GetQty() > 0 && !isReversed) || (cd.GetQty() < 0 && isReversed)))
                             ))
                    {
                        // not to distribute cost in this case, but when qty become ZERO, than set current cost as ZERO
                        if (windowName.Equals("Physical Inventory") && cost.GetCurrentQty().Equals(0))
                        {
                            cost.SetCurrentCostPrice(0);
                        }
                    }
                    else
                    {
                        if (!Qty.Equals(0))
                        {
                            currentCost = Decimal.Round(Decimal.Divide(
                                                         Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), Qty),
                                                         acctSchema.GetCostingPrecision());

                            cost.SetCurrentCostPrice(currentCost);
                        }
                    }
                    cost.SetCurrentQty(Qty);
                    if (!cost.Save(cd.Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Info("Error occured during save/update a record on VAM_ProductCost for Cost Element = " + Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) +
                            @". Error Name : " + (pp != null ? pp.GetName() : ""));
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Is used to create entry on VAM_FreightImpact during consumption
        /// when we consume product, and landed cost is calculated against the same product then we have to reduce the same qty from the freight
        /// but when we REVERSE record, qty will increase, then we check FREIGHT qty is affected -- if YES, then increase the current qty for landed cost.
        /// </summary>
        /// <param name="dsCostElementLine">dataset of cost element against cost combination for which we are calculate cost</param>
        /// <param name="VAF_Org_ID">Org for geeting Product cost entry</param>
        /// <param name="cd">cost detail of transaction</param>
        /// <param name="acctSchema">Accounting schema</param>
        /// <param name="product">Product, whose cost is to be calculated</param>
        /// <param name="M_ASI_ID">Attribute Set Instance, whose cost is to be calculated</param>
        /// <param name="windowName">consume/Increase by which window</param>
        /// <returns>true or false</returns>
        public bool UpdateFreightWithActualQty(DataSet dsCostElementLine, int VAF_Org_ID, MCostDetail cd, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, string windowName, int VAM_Warehouse_ID = 0)
        {
            MCost cost = null;
            for (int i = 0; i < dsCostElementLine.Tables[0].Rows.Count; i++)
            {
                // when costing method is not of landed cost type, then continue.
                if (Util.GetValueOfInt(dsCostElementLine.Tables[0].Rows[i]["iscostMethod"]) == 1)
                    continue;

                int landedVAM_ProductCostElement_ID = Util.GetValueOfInt(dsCostElementLine.Tables[0].Rows[i]["m_ref_costelement"]);

                // when we consume qty then we check, already consume qty agaisnt landed cost -- if consumend then not to take impacts
                if (cd.GetQty() < 0)
                {
                    if (IsAlreadyFreightImpacted(windowName, cd, landedVAM_ProductCostElement_ID) > 0)
                        continue;
                }

                // object of Landed cost allocation - cost element
                cost = MCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, landedVAM_ProductCostElement_ID, VAM_Warehouse_ID);
                if (cost.GetCumulatedQty() <= 0)
                {
                    cost.Save();
                    continue;
                }

                if (cd.GetQty() < 0)
                {
                    // Create entry for Freight Impacts 
                    if (cost.GetCurrentQty().CompareTo(Math.Abs(cd.GetQty())) >= 0)
                    {
                        CreateFreightImpacts(windowName, cd, Math.Abs(cd.GetQty()), landedVAM_ProductCostElement_ID);
                    }
                    else
                    {
                        CreateFreightImpacts(windowName, cd, cost.GetCurrentQty(), landedVAM_ProductCostElement_ID);
                    }

                    Decimal qty = Decimal.Add(cost.GetCurrentQty(), cd.GetQty());
                    if (qty <= 0)
                        qty = 0; // update current qty with ZERO
                    else if (qty > cost.GetCumulatedQty())
                        qty = cost.GetCumulatedQty(); // update current qty with accumulative qty of same object (because we not get freight of that qty)

                    // set current qty for landed cost allocation
                    cost.SetCurrentQty(qty);
                    cost.Save();
                }
                else if (cd.GetQty() > 0)
                {
                    Decimal qty = GetFreightImpactQty(windowName, cd);
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    cost.Save();

                    // after impact, delete the record
                    int freightImpactID = IsAlreadyFreightImpacted(windowName, cd, landedVAM_ProductCostElement_ID);
                    DB.ExecuteQuery("Delete FROM VAM_FreightImpact WHERE VAM_FreightImpact_ID = " + freightImpactID, null, Get_Trx());
                }
            }
            return true;
        }

        /// <summary>
        /// Is used to create entry on VAM_FreightImpact
        /// when we consume product, and landed cost is calculated against the same product then we have to reduce the same qty from the freight
        /// will create this entry -- with those qty which is affected or reduce from the freight
        /// </summary>
        /// <param name="windowName">consume by which window</param>
        /// <param name="cd">cost detail record against line</param>
        /// <param name="qty">qty which is to be affected or reduce from freight</param>
        /// <returns>record save or not</returns>
        /// <writer>Amit Bansal</writer>
        public bool CreateFreightImpacts(string windowName, MCostDetail cd, Decimal qty, int landedVAM_ProductCostElement_ID)
        {
            int tableId = 0;
            int recordId = 0;
            if (windowName == "Shipment")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inv_InOut");
                recordId = cd.GetVAM_Inv_InOutLine_ID();
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inventory");
                recordId = cd.GetVAM_InventoryLine_ID();
            }
            else if (windowName == "Production Execution")
            {
                tableId = MVAFTableView.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
                recordId = cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID();
            }
            else if (windowName == "Inventory Move")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_InventoryTransfer");
                recordId = cd.GetVAM_InvTrf_Line_ID();
            }

            X_VAM_FreightImpact freightImpact = new X_VAM_FreightImpact(GetCtx(), 0, Get_Trx());
            freightImpact.SetVAF_Org_ID(cd.GetVAF_Org_ID());
            freightImpact.SetVAF_TableView_ID(tableId);
            freightImpact.SetRecord_ID(recordId);
            freightImpact.SetVAB_AccountBook_ID(cd.GetVAB_AccountBook_ID());
            freightImpact.SetVAM_ProductCostElement_ID(landedVAM_ProductCostElement_ID);
            freightImpact.SetVAM_Product_ID(cd.GetVAM_Product_ID()); // when we re-calculate, at that tym we have to delete record from from VAM_FreightImpact based on product
            freightImpact.SetQty(qty);
            if (!freightImpact.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured during saving a record in VAM_FreightImpact. Error Name : " + (pp != null ? pp.GetName() : ""));
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Is used to check and get qty from VAM_FreightImpact 
        /// if we found the record entry of Orignal record (based on Transaction refernce on cost detail, we try to search its Orignal record)
        /// Just only for REVERSE RECORD
        /// </summary>
        /// <param name="windowName">consume by which window</param>
        /// <param name="cd">cost detail record against line</param>
        /// <returns>qty - which is to be reduce from the landed cost allocation</returns>
        /// <writer>Amit Bansal</writer>
        public Decimal GetFreightImpactQty(string windowName, MCostDetail cd)
        {
            Decimal qty = 0.0M;
            int tableId = 0;
            String sql = "";
            if (windowName == "Shipment")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inv_InOut");
                sql = @"SELECT Qty FROM VAM_FreightImpact WHERE VAF_TableView_ID = " + tableId +
                      @" AND Record_ID = (SELECT ReversalDoc_ID FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOutLine_ID =" + cd.GetVAM_Inv_InOutLine_ID() + " )";
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inventory");
                sql = @"SELECT Qty FROM VAM_FreightImpact WHERE VAF_TableView_ID = " + tableId +
                     @" AND Record_ID = (SELECT ReversalDoc_ID FROM VAM_InventoryLine WHERE VAM_InventoryLine_ID =" + cd.GetVAM_InventoryLine_ID() + " )";
            }
            else if (windowName == "Production Execution") // not handled its reverse
            {
                tableId = MVAFTableView.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
                sql = @"SELECT Qty FROM VAM_FreightImpact WHERE VAF_TableView_ID = " + tableId +
                      @" AND Record_ID = (SELECT ReversalDoc_ID FROM VAMFG_M_WrkOdrTrnsctionLine WHERE VAMFG_M_WrkOdrTrnsctionLine_ID =" + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID() + " )";
            }
            else if (windowName == "Inventory Move")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_InventoryTransfer");
                sql = @"SELECT Qty FROM VAM_FreightImpact WHERE VAF_TableView_ID = " + tableId +
                    @" AND Record_ID = (SELECT ReversalDoc_ID FROM VAM_InvTrf_Line WHERE VAM_InvTrf_Line_ID =" + cd.GetVAM_InvTrf_Line_ID() + " )";
            }
            if (!String.IsNullOrEmpty(sql))
                qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

            return qty;
        }

        /// <summary>
        /// Is used to get FreightImpact ID -- check freight id based on recordID , tableID, accountSchemaID, costElementID
        /// </summary>
        /// <param name="windowName">impact from which window</param>
        /// <param name="cd">cost detail record for getting detial</param>
        /// <param name="landedVAM_ProductCostElement_ID">landed cost element detail</param>
        /// <returns>VAM_FreightImpact_ID</returns>
        public int IsAlreadyFreightImpacted(string windowName, MCostDetail cd, int landedVAM_ProductCostElement_ID)
        {
            int tableId = 0;
            int recordId = 0;
            if (windowName == "Shipment")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inv_InOut");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOutLine_ID =" + cd.GetVAM_Inv_InOutLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetVAM_Inv_InOutLine_ID();
                }
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_Inventory");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM VAM_InventoryLine WHERE VAM_InventoryLine_ID =" + cd.GetVAM_InventoryLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetVAM_InventoryLine_ID();
                }
            }
            else if (windowName == "Production Execution")
            {
                tableId = MVAFTableView.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM VAMFG_M_WrkOdrTrnsctionLine WHERE VAMFG_M_WrkOdrTrnsctionLine_ID =" + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID();
                }
            }
            else if (windowName == "Inventory Move")
            {
                tableId = MVAFTableView.Get_Table_ID("VAM_InventoryTransfer");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM VAM_InvTrf_Line WHERE VAM_InvTrf_Line_ID =" + cd.GetVAM_InvTrf_Line_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetVAM_InvTrf_Line_ID();
                }
            }
            int freightImpactId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_FreightImpact_ID FROM VAM_FreightImpact WHERE Record_ID = " + recordId +
                                                    @" AND VAF_TableView_ID = " + tableId + @" AND VAB_AccountBook_ID = " + cd.GetVAB_AccountBook_ID() +
                                                    @" AND VAM_ProductCostElement_ID = " + landedVAM_ProductCostElement_ID, null, Get_Trx()));

            return freightImpactId;
        }

        /// <summary>
        /// setter property 
        /// </summary>
        /// <param name="IsExpecetdCostCalculated">set is expected cost calculated or not</param>
        public void SetExpectedCostCalculated(bool IsExpecetdCostCalculated)
        {
            _isExpectedLandeCostCalculated = IsExpecetdCostCalculated;
        }

        /// <summary>
        /// Getter Property
        /// </summary>
        /// <returns>true, when expected cost calculated</returns>
        public bool GetExpectedCostCalculated()
        {
            return _isExpectedLandeCostCalculated;
        }
        //end

        /// <summary>
        /// Create New Order Cost Detail for Physical Inventory.
        /// Called from Doc_Inventory
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAM_InventoryLine_ID">order</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateInventory(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAM_InventoryLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAM_InventoryLine_ID=" + VAM_InventoryLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAM_InventoryLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                VAM_InventoryLine_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetVAM_InventoryLine_ID(VAM_InventoryLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetVAM_InventoryLine_ID(VAM_InventoryLine_ID);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }




            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Invoice Cost Detail for AP Invoices.
        /// Called from DoVAB_Invoice - for Invoice Adjustments
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAB_InvoiceLine_ID">invoice</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateInvoice(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAB_InvoiceLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAB_InvoiceLine_ID=" + VAB_InvoiceLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAB_InvoiceLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                VAB_InvoiceLine_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetVAB_InvoiceLine_ID(VAB_InvoiceLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,                              VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetVAB_InvoiceLine_ID(VAB_InvoiceLine_ID);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);

                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Order Cost Detail for Movements.
        /// Called from Doc_Movement
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAM_InvTrf_Line_ID">movement</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="from">if true the from (reduction)</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateMovement(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAM_InvTrf_Line_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, bool from, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAM_InvTrf_Line_ID=" + VAM_InvTrf_Line_ID
                + " AND IsSOTrx=" + (from ? "'Y'" : "'N'")
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAM_InvTrf_Line_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2 AND IsSOTrx="
                + (from ? "'Y'" : "'N'"),
                VAM_InvTrf_Line_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetVAM_InvTrf_Line_ID(VAM_InvTrf_Line_ID);
                cd.SetIsSOTrx(from);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,                        VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetVAM_InvTrf_Line_ID(VAM_InvTrf_Line_ID);
                    cd.SetIsSOTrx(from);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Order Cost Detail for Purchase Orders.
        /// Called from Doc_MatchPO
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAB_OrderLine_ID">order</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateOrder(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAB_OrderLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {

            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAB_OrderLine_ID=" + VAB_OrderLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAB_OrderLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                VAB_OrderLine_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew cost deatil for selected product
            {
                cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                cd.SetVAB_OrderLine_ID(VAB_OrderLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetVAB_OrderLine_ID(VAB_OrderLine_ID);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                {
                    try
                    {
                        cd.Process();
                    }
                    catch { }
                }
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Cost by deleting old cost
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="mas"></param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="VAM_ProductCostElement_ID"></param>
        /// <param name="Amt"></param>
        /// <param name="Qty"></param>
        /// <param name="Description"></param>
        /// <param name="trxName"></param>
        /// <param name="RectifyPostedRecords"></param>
        /// <param name="Id"></param>
        //private static void CostSetByProcess(MCostDetail cd, MAcctSchema mas, int VAF_Org_ID, int VAM_Product_ID,
        //    int VAM_PFeature_SetInstance_ID, int VAM_ProductCostElement_ID, Decimal Amt, Decimal Qty,
        //    String Description, Trx trx, bool RectifyPostedRecords)
        //{
        //    if (RectifyPostedRecords)
        //    {
        //        cd.SetProcessed(false);
        //        if (cd.Delete(true, trxName))
        //        {
        //            cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
        //                VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
        //        }
        //    }
        //}

        /// <summary>
        /// Create New Order Cost Detail for Production.
        ///	Called from Doc_Production
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAM_ProductionLine_ID">production line</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateProduction(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAM_ProductionLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAM_ProductionLine_ID=" + VAM_ProductionLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAM_ProductionLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                VAM_ProductionLine_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetVAM_ProductionLine_ID(VAM_ProductionLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);

                    cd.SetVAM_ProductionLine_ID(VAM_ProductionLine_ID);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Shipment Cost Detail for SO Shipments.
        ///	Called from Doc_MInOut - for SO Shipments
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="VAM_Inv_InOutLine_ID">shipment</param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">is sales order</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateShipment(MVABAccountBook mas, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, int VAM_Inv_InOutLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, bool IsSOTrx, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "VAM_Inv_InOutLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                VAM_Inv_InOutLine_ID, VAM_PFeature_SetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }



            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(mas.GetCtx(), mas.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Get Cost Detail
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="whereClause">where clause</param>
        /// <param name="ID">1st parameter</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost detail</returns>
        private static MCostDetail Get(Ctx ctx, String whereClause, int ID,
            int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            String sql = "SELECT * FROM VAM_ProductCostDetail WHERE " + whereClause;
            MCostDetail retValue = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", ID);
                param[1] = new SqlParameter("@param2", VAM_PFeature_SetInstance_ID);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MCostDetail(ctx, dr, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql + " - " + ID, e);
            }
            return retValue;
        }

        /// <summary>
        /// Process Cost Details for product
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool ProcessProduct(MProduct product, Trx trxName)
        {
            String sql = "SELECT * FROM VAM_ProductCostDetail "
                + "WHERE VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND Processed='N' "
                + "ORDER BY VAB_AccountBook_ID, VAM_ProductCostElement_ID, VAF_Org_ID, VAM_PFeature_SetInstance_ID, Created";
            int counterOK = 0;
            int counterError = 0;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MCostDetail cd = new MCostDetail(product.GetCtx(), dr, trxName);
                        if (cd.Process())	//	saves
                            counterOK++;
                        else
                            counterError++;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
                counterError++;
            }

            _log.Config(product.GetValue() + ": OK=" + counterOK + ", Errors=" + counterError);
            return counterError == 0;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new record</param>
        /// <param name="success">success</param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>false if processed</returns>
        protected override bool BeforeDelete()
        {
            return !IsProcessed();
        }

        /// <summary>
        /// Is this a Delta Record (previously processed)?
        /// </summary>
        /// <returns>true if delta is not null</returns>
        public bool IsDelta()
        {
            return !(Env.Signum(GetDeltaAmt()) == 0 && Env.Signum(GetDeltaQty()) == 0);
        }

        /// <summary>
        /// Is Invoice
        /// </summary>
        /// <returns>true if invoice line</returns>
        public bool IsInvoice()
        {
            return GetVAB_InvoiceLine_ID() != 0;
        }

        /// <summary>
        /// Is Order
        /// </summary>
        /// <returns>true if order line</returns>
        public bool IsOrder()
        {
            return GetVAB_OrderLine_ID() != 0;
        }

        /// <summary>
        /// Is Shipment
        /// </summary>
        /// <returns>true if sales order shipment</returns>
        public bool IsShipment()
        {
            return IsSOTrx() && GetVAM_Inv_InOutLine_ID() != 0;
        }

        /// <summary>
        /// Process Cost Detail Record.
        /// The record is saved if processed.
        /// </summary>
        /// <returns>true if processed</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Process()
        {
            if (IsProcessed())
            {
                log.Info("Already processed");
                return true;
            }
            bool ok = false;

            //	get costing level for product
            MVABAccountBook mas = new MVABAccountBook(GetCtx(), GetVAB_AccountBook_ID(), null);
            String CostingLevel = mas.GetCostingLevel();
            MProduct product = MProduct.Get(GetCtx(), GetVAM_Product_ID());




            dynamic pca = null;
            if (mas.GetFRPT_LocAcct_ID() > 0)
            {
                pca = MProductCategory.Get(GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pca != null)
                {
                    //if (pca.GetFRPT_CostingLevel() != null)
                    //    CostingLevel = pca.GetFRPT_CostingLevel();
                    if (pca.GetCostingLevel() != null)
                        CostingLevel = pca.GetCostingLevel();
                }
            }
            else
            {
                pca = MProductCategoryAcct.Get(GetCtx(),
                   product.GetVAM_ProductCategory_ID(), GetVAB_AccountBook_ID(), null);
                if (pca != null)
                {
                    if (pca.GetCostingLevel() != null)
                        CostingLevel = pca.GetCostingLevel();
                }
            }

            //	Org Element
            int Org_ID = GetVAF_Org_ID();
            int M_ASI_ID = GetVAM_PFeature_SetInstance_ID();
            if (MVABAccountBook.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                Org_ID = 0;
                M_ASI_ID = 0;
            }
            else if (MVABAccountBook.COSTINGLEVEL_Organization.Equals(CostingLevel))
                M_ASI_ID = 0;
            else if (MVABAccountBook.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                Org_ID = 0;

            //	Create Material Cost elements
            if (GetVAM_ProductCostElement_ID() == 0)
            {
                MCostElement[] ces = MCostElement.GetCostingMethods(this);
                try
                {
                    for (int i = 0; i < ces.Length; i++)
                    {
                        MCostElement ce = ces[i];
                        ok = Process(mas, product, ce, Org_ID, M_ASI_ID);
                        if (!ok)
                            break;
                    }
                }
                catch { }
            }	//	Material Cost elements
            else
            {
                MCostElement ce = MCostElement.Get(GetCtx(), GetVAM_ProductCostElement_ID());
                ok = Process(mas, product, ce, Org_ID, M_ASI_ID);
            }

            //	Save it
            if (ok)
            {
                SetDeltaAmt(Convert.ToDecimal(null));
                SetDeltaQty(Convert.ToDecimal(null));
                SetProcessed(true);
                ok = Save();
            }
            log.Info(ok + " - " + ToString());
            return ok;
        }

        /// <summary>
        /// Process cost detail for cost record
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="ce">cost element</param>
        /// <param name="Org_ID">org - corrected for costing level</param>
        /// <param name="M_ASI_ID">asi corrected for costing level</param>
        /// <returns>true if cost ok</returns>
        private bool Process(MVABAccountBook mas, MProduct product, MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            string qry = "";
            if (SaveCost(mas, product, ce, Org_ID, M_ASI_ID, 0))
            {
                if (product.IsOneAssetPerUOM())
                {
                    if (GetVAB_OrderLine_ID() > 0)
                    {
                        qry = @"SELECT ast.VAA_Asset_ID from VAA_Asset ast INNER JOIN VAM_Inv_InOutLine inl ON (ast.VAM_Inv_InOutLine_ID = inl.VAM_Inv_InOutLine_ID)
                            INNER JOIN VAB_OrderLine odl ON (inl.VAB_OrderLine_ID = odl.VAB_OrderLine_ID) Where ast.IsActive='Y' 
                            AND ast.VAM_Product_ID=" + product.GetVAM_Product_ID() + " AND inl.VAB_OrderLine_ID=" + GetVAB_OrderLine_ID();
                    }
                    else
                    {
                        qry = @"SELECT ast.VAA_Asset_ID from VAA_Asset ast INNER JOIN VAM_Inv_InOutLine inl ON (ast.VAM_Inv_InOutLine_ID = inl.VAM_Inv_InOutLine_ID)
                            INNER JOIN VAB_InvoiceLine inv ON (inv.VAM_Inv_InOutLine_ID = inl.VAM_Inv_InOutLine_ID) WHERE ast.IsActive ='Y' 
                            AND ast.VAM_Product_ID=" + product.GetVAM_Product_ID() + " AND inv.VAB_InvoiceLine_ID =" + GetVAB_InvoiceLine_ID();
                    }
                    DataSet ds = DB.ExecuteDataset(qry, null, null);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int VAA_Asset_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]);
                                if (!SaveCost(mas, product, ce, Org_ID, M_ASI_ID, VAA_Asset_ID))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SaveCost(MVABAccountBook mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int VAA_Asset_ID)
        {
            MCost cost = null;
            if (VAA_Asset_ID == 0)
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetVAM_ProductCostElement_ID());
            }
            else
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), VAA_Asset_ID);
            }
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as, Org_ID, ce.getVAM_ProductCostElement_ID());

            Decimal qty = GetQty();
            Decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();

            if (VAA_Asset_ID > 0)
            {
                qty = 1;
                amt = Decimal.Round(Decimal.Divide(GetAmt(), GetQty()), precision, MidpointRounding.AwayFromZero);

            }

            Decimal price = amt;
            if (Env.Signum(qty) != 0)
                price = Decimal.Round(Decimal.Divide(amt, qty), precision, MidpointRounding.AwayFromZero);


            /** All Costing Methods
            if (ce.isAverageInvoice())
            else if (ce.isAveragePO())
            else if (ce.isFifo())
            else if (ce.isLifo())
            else if (ce.isLastInvoice())
            else if (ce.isLastPOPrice())
            else if (ce.isStandardCosting())
            else if (ce.isUserDefined())
            else if (!ce.isCostingMethod())
            **/

            //	*** Purchase Order Detail Record ***
            if (GetVAB_OrderLine_ID() != 0)
            {
                MVABOrderLine oLine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), null);
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                if (ce.IsAveragePO())
                {
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    /**********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /**********************************/

                    log.Finer("PO - AveragePO - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    if (!isReturnTrx)
                    {
                        if (Env.Signum(qty) != 0)
                            cost.SetCurrentCostPrice(price);
                        else
                        {
                            Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                            cost.SetCurrentCostPrice(cCosts);
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("PO - LastPO - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    log.Finer("PO - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    log.Finer("PO - " + ce + " - " + cost);
                }
                //	else
                //		log.warning("PO - " + ce + " - " + cost);
            }

            //	*** AP Invoice Detail Record ***
            else if (GetVAB_InvoiceLine_ID() != 0)
            {
                bool isReturnTrx = Env.Signum(qty) < 0;
                if (ce.IsAverageInvoice())
                {
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    /**********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /**********************************/
                    log.Finer("Inv - AverageInv - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    //	Real ASI - costing level Org

                    // commented by Amit because cost queue is created from complete 16-12-2015
                    //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                    //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                    //cq.SetCosts(amt, qty, precision);
                    //cq.Save(Get_TrxName());

                    //end
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.Add(amt, qty);
                    log.Finer("Inv - FiFo/LiFo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    if (!isReturnTrx)
                    {
                        if (Env.Signum(qty) != 0)
                            cost.SetCurrentCostPrice(price);
                        else
                        {
                            Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                            cost.SetCurrentCostPrice(cCosts);
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - LastInv - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                    {
                        cost.SetCurrentCostPrice(price);
                        //	seed initial price
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                        {
                            cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                    mas, Org_ID, ce.GetCostingMethod(), GetVAB_OrderLine_ID()));
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    cost.Add(amt, qty);
                    log.Finer("Inv - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())		//	Cost Adjustments
                {
                    //Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //cost.SetCurrentCostPrice(cCosts);
                    //cost.Add(amt, qty);
                    //log.Finer("Inv - none - " + cost);
                    Decimal cCosts = Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt);
                    Decimal qty1 = Decimal.Add(cost.GetCurrentQty(), qty);
                    if (qty1.CompareTo(Decimal.Zero) == 0)
                    {
                        qty1 = Decimal.One;
                    }
                    cCosts = Decimal.Round(Decimal.Divide(cCosts, qty1), precision, MidpointRounding.AwayFromZero);
                    cost.SetCurrentCostPrice(cCosts);
                    cost.Add(amt, qty);
                    log.Finer("Inv - none - " + cost);
                }
                //	else
                //		log.warning("Inv - " + ce + " - " + cost);
            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetVAM_Inv_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetVAM_InvTrf_Line_ID() != 0
                || GetVAM_InventoryLine_ID() != 0
                || GetVAM_ProductionLine_ID() != 0
                || GetVAB_ProjectSupply_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                //
                if (ce.IsAverageInvoice())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /**********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /**********************************/
                    log.Finer("QtyAdjust - AverageInv - " + cost);
                }
                else if (ce.IsAveragePO())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));

                    /**********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /**********************************/

                    log.Finer("QtyAdjust - AveragePO - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    if (addition)
                    {
                        //	Real ASI - costing level Org
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                        //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                        //cq.SetCosts(amt, qty, precision);
                        //cq.Save();
                    }
                    else
                    {
                        //	Adjust Queue - costing level Org/ASI
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue.AdjustQty(product, M_ASI_ID,
                        //    mas, Org_ID, ce, Decimal.Negate(qty), Get_TrxName());
                    }
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - FiFo/Lifo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastInv - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastPO - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        //	Initial
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0
                            && cost.Get_ID() == 0)
                            cost.SetCurrentCostPrice(price);
                    }
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    if (addition)
                        cost.Add(amt, qty);
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    //	Should not happen
                    log.Finer("QtyAdjust - ?none? - " + cost);
                }
                else
                {
                    log.Warning("QtyAdjust - " + ce + " - " + cost);
                }
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }
            if (VAA_Asset_ID != 0)
            {
                cost.SetIsAssetCost(true);
            }
            return cost.Save();
        }

        /// <summary>
        /// Set Amt
        /// </summary>
        /// <param name="amt">amount</param>
        public new void SetAmt(Decimal? amt)
        {
            //if (IsProcessed())
            //    throw new Exception("Cannot change Amt - processed");
            if (amt == null)
                base.SetAmt(Env.ZERO);
            else
                base.SetAmt(amt);
        }

        /// <summary>
        /// Set Qty
        /// </summary>
        /// <param name="qty">quantity</param>
        public new void SetQty(Decimal? qty)
        {
            //    if (IsProcessed())
            //        throw new Exception("Cannot change Qty - processed");
            if (qty == null)
                base.SetQty(Env.ZERO);
            else
                base.SetQty(qty);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCostDetail[");
            sb.Append(Get_ID());
            if (GetVAB_OrderLine_ID() != 0)
                sb.Append(",VAB_OrderLine_ID=").Append(GetVAB_OrderLine_ID());
            if (GetVAM_Inv_InOutLine_ID() != 0)
                sb.Append(",VAM_Inv_InOutLine_ID=").Append(GetVAM_Inv_InOutLine_ID());
            if (GetVAB_InvoiceLine_ID() != 0)
                sb.Append(",VAB_InvoiceLine_ID=").Append(GetVAB_InvoiceLine_ID());
            if (GetVAB_ProjectSupply_ID() != 0)
                sb.Append(",VAB_ProjectSupply_ID=").Append(GetVAB_ProjectSupply_ID());
            if (GetVAM_InvTrf_Line_ID() != 0)
                sb.Append(",VAM_InvTrf_Line_ID=").Append(GetVAM_InvTrf_Line_ID());
            if (GetVAM_InventoryLine_ID() != 0)
                sb.Append(",VAM_InventoryLine_ID=").Append(GetVAM_InventoryLine_ID());
            if (GetVAM_ProductionLine_ID() != 0)
                sb.Append(",VAM_ProductionLine_ID=").Append(GetVAM_ProductionLine_ID());
            sb.Append(",Amt=").Append(GetAmt())
                .Append(",Qty=").Append(GetQty());
            if (IsDelta())
                sb.Append(",DeltaAmt=").Append(GetDeltaAmt())
                    .Append(",DeltaQty=").Append(GetDeltaQty());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Process Reversed cost detail for cost record
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="ce">cost element</param>
        /// <param name="Org_ID">org - corrected for costing level</param>
        /// <param name="M_ASI_ID">asi corrected for costing level</param>
        /// <returns>true if cost ok</returns>
        public bool ProcessReversedCost(VAdvantage.Model.MVABAccountBook mas, MProduct product, VAdvantage.Model.MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            MCost cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetVAM_ProductCostElement_ID());
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as1, Org_ID, ce.getVAM_ProductCostElement_ID());

            Decimal qty = GetQty();
            Decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();
            Decimal price = amt;
            //if (Env.Signum(qty) != 0)
            //    price = Decimal.Round(Decimal.Divide(amt, qty), precision, MidpointRounding.AwayFromZero);


            /** All Costing Methods
            if (ce.isAverageInvoice())
            else if (ce.isAveragePO())
            else if (ce.isFifo())
            else if (ce.isLifo())
            else if (ce.isLastInvoice())
            else if (ce.isLastPOPrice())
            else if (ce.isStandardCosting())
            else if (ce.isUserDefined())
            else if (!ce.isCostingMethod())
            **/

            //	*** Purchase Order Detail Record ***
            if (GetVAB_OrderLine_ID() != 0)
            {
                MVABOrderLine oLine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), null);
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                if (ce.IsAveragePO())
                {
                    /*********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /*********************************/
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    log.Finer("VAdvantage.Model.PO - AveragePO - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {


                    string lastPrice = "select round(amt/ qty,6) as Price from VAM_ProductCostdetail where VAM_Product_id="
                        + cost.GetVAM_Product_ID() + " and VAB_Orderline_id is NOT NULL and VAB_Orderline_id<> @param1"
                        + " ORDER BY VAM_ProductCostdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetVAB_OrderLine_ID()));

                    cost.Add(amt, qty);


                    //if (!isReturnTrx)
                    //{
                    //    if (Env.Signum(qty) != 0)
                    //        cost.SetCurrentCostPrice(price);
                    //    else
                    //    {
                    //        Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //        cost.SetCurrentCostPrice(cCosts);
                    //    }
                    //}

                    log.Finer("VAdvantage.Model.PO - LastPO - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    log.Finer("VAdvantage.Model.PO - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    log.Finer("VAdvantage.Model.PO - " + ce + " - " + cost);
                }
                //	else
                //		log.warning("VAdvantage.Model.PO - " + ce + " - " + cost);
            }

            //	*** AP Invoice Detail Record ***
            else if (GetVAB_InvoiceLine_ID() != 0)
            {
                bool isReturnTrx = Env.Signum(qty) < 0;
                if (ce.IsAverageInvoice())
                {
                    /*********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /*********************************/

                    //if (!isReturnTrx)
                    //{
                    //cost.SetWeightedAverage(amt, qty);
                    //}
                    //else
                    //{
                    //    cost.Add(amt, qty);
                    //}
                    log.Finer("Inv - AverageInv - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    //	Real ASI - costing level Org
                    // commented by Amit because cost queue is created from complete 16-12-2015
                    //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                    //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                    //cq.SetCosts(amt, qty, precision);
                    //cq.Save();
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - FiFo/LiFo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {

                    string lastPrice = "select round(amt/ qty,6) as Price from VAM_ProductCostdetail where VAM_Product_id="
                        + cost.GetVAM_Product_ID() + " and VAB_InvoiceLine_id is NOT NULL and VAB_InvoiceLine_id<> @param1"
                        + " ORDER BY VAM_ProductCostdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetVAB_InvoiceLine_ID()));

                    cost.Add(amt, qty);
                    //if (!isReturnTrx)
                    //{
                    //    if (Env.Signum(qty) != 0)
                    //        cost.SetCurrentCostPrice(price);
                    //    else
                    //    {
                    //        Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //        cost.SetCurrentCostPrice(cCosts);
                    //    }
                    //}
                    log.Finer("Inv - LastInv - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                    {
                        cost.SetCurrentCostPrice(price);
                        //	seed initial price
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                        {
                            cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                    mas, Org_ID, ce.GetCostingMethod(), GetVAB_OrderLine_ID()));
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    cost.Add(amt, qty);
                    log.Finer("Inv - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())		//	Cost Adjustments
                {
                    Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    cost.SetCurrentCostPrice(cCosts);
                    cost.Add(amt, qty);
                    log.Finer("Inv - none - " + cost);
                }
                //	else
                //		log.warning("Inv - " + ce + " - " + cost);

            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetVAM_Inv_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetVAM_InvTrf_Line_ID() != 0
                || GetVAM_InventoryLine_ID() != 0
                || GetVAM_ProductionLine_ID() != 0
                || GetVAB_ProjectSupply_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                //
                if (ce.IsAverageInvoice())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /*********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /*********************************/
                    log.Finer("QtyAdjust - AverageInv - " + cost);
                }
                else if (ce.IsAveragePO())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /*********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /*********************************/
                    log.Finer("QtyAdjust - AveragePO - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    if (addition)
                    {
                        //	Real ASI - costing level Org
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue cq = MCostQueue.Get(product, GetVAM_PFeature_SetInstance_ID(),
                        //    mas, Org_ID, ce.GetVAM_ProductCostElement_ID(), Get_TrxName());
                        //cq.SetCosts(amt, qty, precision);
                        //cq.Save();
                    }
                    else
                    {
                        //	Adjust Queue - costing level Org/ASI
                        // commented by Amit because cost queue is created from complete 16-12-2015
                        //MCostQueue.AdjustQty(product, M_ASI_ID,
                        //    mas, Org_ID, ce, Decimal.Negate(qty), Get_TrxName());
                    }
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - FiFo/Lifo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastInv - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastPO - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        //	Initial
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0
                            && cost.Get_ID() == 0)
                            cost.SetCurrentCostPrice(price);
                    }
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    if (addition)
                        cost.Add(amt, qty);
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    //	Should not happen
                    log.Finer("QtyAdjust - ?none? - " + cost);
                }
                else
                {
                    log.Warning("QtyAdjust - " + ce + " - " + cost);
                }
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }

            if (cost.GetCurrentQty() == 0)
            {
                cost.SetCurrentCostPrice(0);
            }
            return cost.Save();
        }


        /**
       * 	Create New Work Order Resource Transaction Cost detail
       * 	Called from Doc_WorkOrderTransaction - for Work Order Transactions  
       *	@param as1 accounting schema
       *	@param VAF_Org_ID org
       *	@param VAM_Product_ID product
       *	@param VAM_PFeature_SetInstance_ID asi
       *	@param VAM_Inv_InOutLine_ID shipment
       *	@param VAM_ProductCostElement_ID optional cost element for Freight
       *	@param Amt amt
       *	@param Qty qty
       *	@param Description optional description
       *	@param IsSOTrx sales order
       *	@param trx transaction
       *	@return true if no error
       */
        public static Boolean CreateWorkOrderResourceTransaction(MVABAccountBook as1, int VAF_Org_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            int M_WorkOrderResourceTransactionLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderResourceTxnLine_ID= " + M_WorkOrderResourceTransactionLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
                + " AND VAB_AccountBook_ID = " + as1.GetVAB_AccountBook_ID();


            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);
            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderResourceTxnLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                    M_WorkOrderResourceTransactionLine_ID, VAM_PFeature_SetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, VAF_Org_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trx);
                cd.SetM_WorkOrderResourceTxnLine_ID(M_WorkOrderResourceTransactionLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trx))
                    {
                        cd = new MCostDetail(as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trx);
                    }
                    // CostSetByProcess(cd, as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderResourceTxnLine_ID(M_WorkOrderResourceTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }



            Boolean ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(as1.GetCtx(), as1.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Work Order Transaction Cost detail
        /// Called from Doc_WorkOrderTransaction - for Work Order Transactions  
        /// </summary>
        /// <param name="as1"></param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="M_WorkOrderTransactionLine_ID"></param>
        /// <param name="VAM_ProductCostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amt</param>
        /// <param name="Qty">qty</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">sales order</param>
        /// <param name="trx">transaction</param>
        /// <returns>true if no error</returns>
        public static Boolean CreateWorkOrderTransaction(MVABAccountBook as1, int VAF_Org_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            int M_WorkOrderTransactionLine_ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderTransactionLine_ID= " + M_WorkOrderTransactionLine_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
                + " AND VAB_AccountBook_ID = " + as1.GetVAB_AccountBook_ID()
                + " AND VAM_ProductCostElement_ID = " + VAM_ProductCostElement_ID;

            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);

            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderTransactionLine_ID=@param1 AND VAM_PFeature_SetInstance_ID=@param2",
                    M_WorkOrderTransactionLine_ID, VAM_PFeature_SetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_ProductCostElement_ID, Amt, Qty, Description, trx);

                cd.SetM_WorkOrderTransactionLine_ID(M_WorkOrderTransactionLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trx))
                    {
                        cd = new MCostDetail(as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trx);
                    }
                    //CostSetByProcess(cd, as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderTransactionLine_ID(M_WorkOrderTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);

                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }

            Boolean ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(as1.GetCtx(), as1.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create Cost for current transaction against the product and there Attribute
        /// against the accounting schema and CostElement defind on the current record
        /// </summary>
        /// <param name="as1">account schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute instance set on product</param>
        /// <param name="costLineColName">Column name of VAM_ProductCostDetail table to which value is to be set</param>
        /// <param name="ID">value that is set for the costLineColName</param>
        /// <param name="VAM_ProductCostElement_ID">cost element set on product category/accounting schema</param>
        /// <param name="Amt">Amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">discription</param>
        /// <param name="IsSOTrx">is sales transaction</param>
        /// <param name="trx">transaction</param>
        /// <param name="RectifyPostedRecords"></param>
        /// <returns>on success true else false</returns>
        /// <Created by>Raghu</Created>
        /// <date>25-09-2015</date>
        public static Boolean CreateCostTransaction(MVABAccountBook as1, int VAF_Org_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, string costLineColName, int ID, int VAM_ProductCostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM VAM_ProductCostDetail "
               + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
               + " AND " + costLineColName + "= " + ID
               + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
               + " AND VAB_AccountBook_ID = " + as1.GetVAB_AccountBook_ID()
               + " AND VAM_ProductCostElement_ID = " + VAM_ProductCostElement_ID;

            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);

            MCostDetail cd = Get(as1.GetCtx(), costLineColName + "=@param1 AND VAM_PFeature_SetInstance_ID=@param2", ID, VAM_PFeature_SetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductCostElement_ID, Amt, Qty, Description, trx);
                cd.Set_CustomColumn(costLineColName, ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trx))
                    {
                        cd = new MCostDetail(as1, VAF_Org_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                            VAM_ProductCostElement_ID, Amt, Qty, Description, trx);
                    }
                    cd.Set_CustomColumn(costLineColName, ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    cd.SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
                    cd.SetVAM_Product_ID(VAM_Product_ID);
                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }

            Boolean ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MVAFClient client = MVAFClient.Get(as1.GetCtx(), as1.GetVAF_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }
    }
}