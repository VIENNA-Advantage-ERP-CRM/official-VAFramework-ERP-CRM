/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_CostDetail
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
using ModelLibrary.Classes;

namespace VAdvantage.Model
{
    public class MCostDetail : X_M_CostDetail
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostDetail).FullName);
        private bool _isExpectedLandeCostCalculated = false;
        private bool _IsCalculateExpectedLandedCostFromInvoice = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_CostDetail_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(Ctx ctx, int M_CostDetail_ID, Trx trxName)
            : base(ctx, M_CostDetail_ID, trxName)
        {
            if (M_CostDetail_ID == 0)
            {
                //	setC_AcctSchema_ID (0);
                //	setM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);
                //	setC_OrderLine_ID (0);
                //	setM_InOutLine_ID(0);
                //	setC_InvoiceLine_ID (0);
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
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_CostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(MAcctSchema mas, int AD_Org_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_CostElement_ID, Decimal Amt, Decimal Qty, String Description, Trx trxName)
            : this(mas.GetCtx(), 0, trxName)
        {
            SetClientOrg(mas.GetAD_Client_ID(), AD_Org_ID);
            SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            SetM_CostElement_ID(M_CostElement_ID);
            //
            SetAmt(Amt);
            SetQty(Qty);
            SetDescription(Description);
        }

        /// <summary>
        /// Is used to create Cost detail with respective reference
        /// </summary>
        /// <param name="mas">Accounting SChema</param>
        /// <param name="AD_Org_ID">Organization</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInstance</param>
        /// <param name="WindowName">calling window</param>
        /// <param name="inventoryLine">inventory line reference</param>
        /// <param name="inoutline">inout line reference</param>
        /// <param name="movementline">movement line reference</param>
        /// <param name="invoiceline">invoice line reference </param>
        /// <param name="po">Production execution reference</param>
        /// <param name="M_CostElement_ID">cost element</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">description</param>
        /// <param name="trxName">trasnaction</param>
        /// <param name="M_Warehouse_ID">Optional parameter -- Warehouse ID</param>
        /// <returns>MCostDetail Object</returns>
        public static MCostDetail CreateCostDetail(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, string WindowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
            MInvoiceLine invoiceline, PO po, int M_CostElement_ID, Decimal Amt, Decimal Qty, String Description, Trx trxName, int M_Warehouse_ID = 0)
        {
            try
            {
                Amt = Decimal.Round(Amt, mas.GetCostingPrecision());
                MCostDetail cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID, Amt, Qty, Description, trxName);
                cd.SetProcessed(true);
                cd.SetM_Warehouse_ID(M_Warehouse_ID);
                if (WindowName == "Physical Inventory" || WindowName == "Internal Use Inventory")
                {
                    cd.SetM_InventoryLine_ID(inventoryLine.GetM_InventoryLine_ID());
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
                    cd.SetM_MovementLine_ID(movementline.GetM_MovementLine_ID());
                }
                else if (WindowName == "Material Receipt" || WindowName == "Shipment" || WindowName == "Customer Return" || WindowName == "Return To Vendor")
                {
                    cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                    if (inoutline.GetC_OrderLine_ID() > 0)
                    {
                        cd.SetC_OrderLine_ID(inoutline.GetC_OrderLine_ID());
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
                    cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                    cd.SetIsSOTrx(false);
                    if (invoiceline.GetC_OrderLine_ID() > 0)
                    {
                        cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
                    }
                    if (invoiceline.GetM_InOutLine_ID() > 0)
                    {
                        if (inoutline != null && inoutline.GetM_InOutLine_ID() > 0)
                        {
                            cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                        }
                        else
                        {
                            cd.SetM_InOutLine_ID(invoiceline.GetM_InOutLine_ID());
                        }
                    }
                }
                else if (WindowName == "Invoice(Customer)")
                {
                    cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                    cd.SetIsSOTrx(true);
                    if (invoiceline.GetC_OrderLine_ID() > 0)
                    {
                        cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
                    }
                    if (invoiceline.GetM_InOutLine_ID() > 0)
                    {
                        cd.SetM_InOutLine_ID(invoiceline.GetM_InOutLine_ID());
                    }
                    else if (invoiceline.GetC_OrderLine_ID() > 0)
                    {
                        cd.SetM_InOutLine_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT m_inoutline_id FROM m_inoutline WHERE isactive     = 'Y' AND c_orderline_id = " + invoiceline.GetC_OrderLine_ID(), null, null)));
                    }
                }
                else if (WindowName.Equals("ProvisionalInvoice"))
                {
                    cd.Set_Value("C_ProvisionalInvoiceLine_ID", po.Get_Value("C_ProvisionalInvoiceLine_ID"));
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

        public bool UpdateProductCost(String windowName, MCostDetail cd, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, int cq_AD_Org_ID, string optionalStrCd = "process")
        {
            return UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, cq_AD_Org_ID, null, optionalStrCd);
        }

        /// <summary>
        /// Create or Update Product Cost
        /// </summary>
        /// <param name="windowName">Window Name</param>
        /// <param name="cd">cost Detail</param>
        /// <param name="acctSchema">Accounting Schema</param>
        /// <param name="product">Product</param>
        /// <param name="M_ASI_ID">ASI</param>
        /// <param name="cq_AD_Org_ID">queue Organization</param>
        /// <param name="costingCheck">Costing Check</param>
        /// <param name="optionalStrCd">optional String</param>
        /// <returns>true, when success</returns>
        public bool UpdateProductCost(String windowName, MCostDetail cd, MAcctSchema acctSchema, MProduct product,
            int M_ASI_ID, int cq_AD_Org_ID, CostingCheck costingCheck, string optionalStrCd = "process")
        {
            int AD_Org_ID = 0;
            // Get Org based on Costing Level
            dynamic pc = null;
            String cl = null;
            string costingMethod = null;
            int costElementId = 0;
            MClient client = MClient.Get(GetCtx(), cd.GetAD_Client_ID());
            int M_Warehouse_ID = 0; // is used to calculate cost with warehouse level or not

            if (product != null)
            {
                pc = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
                if (pc != null)
                {
                    cl = pc.GetCostingLevel();
                    costingMethod = pc.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costElementId = pc.GetM_CostElement_ID();
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
                        costElementId = acctSchema.GetM_CostElement_ID();
                    }
                }
            }

            // set Organization for product costs
            if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_BatchLot)
            {
                AD_Org_ID = 0;
            }
            else
            {
                AD_Org_ID = cd.GetAD_Org_ID();
            }
            // set ASI as ZERO in case of costing levele either "Client" or "Organization" or "Warehouse"
            if (cl != MProductCategory.COSTINGLEVEL_BatchLot && cl != MProductCategory.COSTINGLEVEL_OrgPlusBatch && cl != MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_ASI_ID = 0;
            }
            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_Warehouse_ID = cd.GetM_Warehouse_ID();
            }

            // get Cost element id of selected costing method
            if (costingMethod != "C")
            {
                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT DISTINCT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' 
                            AND CostingMethod = '" + costingMethod + "' AND AD_Client_ID = " + product.GetAD_Client_ID()));
            }
            else if (costingMethod == "C") // costing method on cost combination - Element line
            {
                string sql = @"SELECT  cel.M_Ref_CostElement
                                    FROM M_CostElement ce INNER JOIN m_costelementline cel ON ce.M_CostElement_ID  = cel.M_CostElement_ID
                                    WHERE ce.AD_Client_ID   =" + product.GetAD_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.M_CostElement_ID = " + costElementId + @"
                                    AND  CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT M_CostElement_ID FROM M_CostELEMENT WHERE costingmethod IS NOT NULL  )
                                    ORDER BY ce.M_CostElement_ID";
                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            }

            // Get All Material Costing Element
            // when calculate cost from process, then no need to calculate cost for define costing method either on product category or on Accounting Schema
            if (GetM_CostElement_ID() == 0 && optionalStrCd == "process")
            {
                MCostElement[] ces = MCostElement.GetCostingMethods(this);
                try
                {
                    for (int i = 0; i < ces.Length; i++)
                    {
                        MCostElement ce = ces[i];

                        if (ce.GetM_CostElement_ID() != costElementId)
                        {
                            if (!UpdateCost(acctSchema, product, ce, AD_Org_ID, M_ASI_ID, 0, cq_AD_Org_ID, windowName, cd, costingCheck, costingMethod, costElementId, M_Warehouse_ID))
                            {
                                return false;
                            }
                        }
                        else if (!client.IsCostImmediate())
                        {
                            if (!UpdateCost(acctSchema, product, ce, AD_Org_ID, M_ASI_ID, 0, cq_AD_Org_ID, windowName, cd, costingCheck, costingMethod, costElementId, M_Warehouse_ID))
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
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM M_InventoryLine WHERE M_InventoryLine_ID =  " + cd.GetM_InventoryLine_ID(), null, Get_Trx()));
                            }
                            if (windowName == "AssetDisposal")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAFAM_AssetDisposal WHERE VAFAM_AssetDisposal_ID = " + cd.Get_Value("VAFAM_AssetDisposal_ID"), null, Get_Trx()));
                            }
                            else if (windowName == "Inventory Move")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM M_MovementLine WHERE M_MovementLine_ID =  " + cd.GetM_MovementLine_ID(), null, Get_Trx()));
                            }
                            else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM M_InOutLine WHERE M_InOutLine_ID =  " + cd.GetM_InOutLine_ID(), null, Get_Trx()));
                            }
                            else if (windowName == "Invoice(Vendor)" || windowName == "Match IV" || windowName == "Product Cost IV" || windowName == "Product Cost IV Form" || windowName == "Invoice(Vendor)-Return")
                            {
                                // isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM C_InvoiceLine WHERE C_InvoiceLine_ID =  " + cd.GetC_InvoiceLine_ID(), null, Get_Trx()));
                                string docStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM C_invoice WHERE C_Invoice_ID = (SELECT C_Invoice_ID FROM C_invoiceLine WHERE C_InvoiceLine_ID = " + cd.GetC_InvoiceLine_ID() + ")", null, Get_Trx()));
                                if (docStatus == "VO" || docStatus == "RE")
                                {
                                    isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT isreversedcostcalculated FROM M_MatchInvCostTrack WHERE Rev_C_InvoiceLine_ID =  " + cd.GetC_InvoiceLine_ID() + " AND M_inoutline_ID = " + cd.GetM_InOutLine_ID(), null, Get_Trx()));
                                }
                                else
                                {
                                    isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM M_matchInv WHERE C_InvoiceLine_ID =  " + cd.GetC_InvoiceLine_ID() + " AND M_inoutline_ID = " + cd.GetM_InOutLine_ID(), null, Get_Trx()));
                                }
                            }
                            else if (windowName == "Invoice(Customer)" || windowName == "Invoice(APC)")
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM C_InvoiceLine WHERE C_InvoiceLine_ID =  " + cd.GetC_InvoiceLine_ID(), null, Get_Trx()));
                            }
                            if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                isCostImmediate = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCostImmediate FROM VAMFG_M_WrkOdrTrnsctionLine WHERE VAMFG_M_WrkOdrTrnsctionLine_ID =  " + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID(), null, Get_Trx()));
                            }
                            if (isCostImmediate == "N")
                            {
                                if (!UpdateCost(acctSchema, product, ce, AD_Org_ID, M_ASI_ID, 0, cq_AD_Org_ID, windowName, cd, costingCheck, costingMethod, costElementId, M_Warehouse_ID))
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
                if (optionalStrCd == "window" && GetM_CostElement_ID() == 0)
                {
                    ce = MCostElement.Get(GetCtx(), costElementId);
                }
                else
                {
                    ce = MCostElement.Get(GetCtx(), GetM_CostElement_ID());
                }
                if (!UpdateCost(acctSchema, product, ce, AD_Org_ID, M_ASI_ID, 0, cq_AD_Org_ID, windowName, cd, costingCheck, costingMethod, costElementId, M_Warehouse_ID))
                {
                    return false;
                }
            }
            return true;
        }

        private bool UpdateCost(MAcctSchema mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int A_Asset_ID, int cq_AD_Org_ID,
                                string windowName, MCostDetail cd, string costingMethod = "", int costCominationelement = 0, int M_Warehouse_ID = 0)
        {
            return UpdateCost(mas, product, ce, Org_ID, M_ASI_ID, A_Asset_ID, cq_AD_Org_ID,
                                        windowName, cd, null, costingMethod, costCominationelement, M_Warehouse_ID);
        }

        /// <summary>
        /// Update or Create product Cost
        /// </summary>
        /// <param name="mas">Accouting Schema</param>
        /// <param name="product">Product</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="M_ASI_ID">ASI</param>
        /// <param name="A_Asset_ID">Asset</param>
        /// <param name="cq_AD_Org_ID">Queue Organization</param>
        /// <param name="windowName">Window Name</param>
        /// <param name="cd">Cost Detail</param>
        /// <param name="costingCheck">Costing Check</param>
        /// <param name="costingMethod">Costing Method</param>
        /// <param name="costCominationelement">Combination Element ID</param>
        /// <param name="M_Warehouse_ID">Warehouse ID</param>
        /// <returns></returns>
        private bool UpdateCost(MAcctSchema mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int A_Asset_ID, int cq_AD_Org_ID,
           string windowName, MCostDetail cd, CostingCheck costingCheck, string costingMethod = "", int costCominationelement = 0, int M_Warehouse_ID = 0)
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

            if (A_Asset_ID == 0)
            {
                if (windowName == "Inventory Move")
                {
                    cost = MCost.Get(product, M_ASI_ID, mas, cq_AD_Org_ID, ce.GetM_CostElement_ID(), M_Warehouse_ID);
                    //change 10-5-2016
                    if (cd.GetM_MovementLine_ID() > 0)
                    {
                        int M_SourceWarehouse_ID = 0;
                        MMovementLine movementlineFrom = new MMovementLine(GetCtx(), cd.GetM_MovementLine_ID(), Get_TrxName());
                        // when costing level is warehouse then need to get source warehouse
                        if (M_Warehouse_ID > 0)
                        {
                            M_SourceWarehouse_ID = MLocator.Get(GetCtx(), movementlineFrom.GetM_Locator_ID()).GetM_Warehouse_ID();
                        }
                        costFrom = MCost.Get(product, M_ASI_ID, mas, movementlineFrom.GetAD_Org_ID(), ce.GetM_CostElement_ID(), M_SourceWarehouse_ID);
                    }
                    //end
                }
                else
                {
                    cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID(), M_Warehouse_ID);
                }
            }
            else
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID(), A_Asset_ID, M_Warehouse_ID);
            }

            //Decimal qty = 0;
            //Decimal amt = 0;
            //if (ce.IsWeightedAverageCost() && ((GetC_InvoiceLine_ID() != 0 && windowName != "Material Receipt") || (windowName == "Product Cost IV") || (windowName == "Product Cost IV Form")))
            //{
            //    invoiceline = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_Trx());
            //    invoice = new MInvoice(GetCtx(), invoiceline.GetC_Invoice_ID(), Get_Trx());
            //    if (GetC_InvoiceLine_ID() > 0 && ((invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->")) || (invoice.GetDescription() == null)))
            //    {
            //        amt = Decimal.Divide(GetAmt(), GetQty());

            //        string sql = "SELECT SUM(QTY) FROM M_MatchInv WHERE IsCostCalculated = 'N' AND C_InvoiceLine_ID = " + GetC_InvoiceLine_ID();
            //        qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, cd.Get_Trx()));

            //        amt = Decimal.Multiply(amt, qty);

            //        sql = "UPDATE M_MatchInv SET IsCostCalculated = 'Y' WHERE C_InvoiceLine_ID = " + GetC_InvoiceLine_ID();
            //        DB.ExecuteQuery(sql, null, cd.Get_Trx());
            //    }
            //    else if (GetC_InvoiceLine_ID() > 0 && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
            //    {
            //        // on reversal - get qty from new table of M_MatchInvCostTrack
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
                    //MClient client = MClient.Get(GetCtx(), cd.GetAD_Client_ID());
                    if (ce.IsFifo() || ce.IsLifo())
                    {
                        sql = @"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                     where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) IN (  " + M_ASI_ID +
                                     @"," + cd.GetM_AttributeSetInstance_ID() + " )" +
                                     @" AND ced.M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                     @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND cq.M_CostElement_ID = " + ce.GetM_CostElement_ID() +
                                     @" AND ced.AD_Client_ID = " + cd.GetAD_Client_ID();
                        MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        if (MRPrice == 0)
                        {
                            sql = @"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id
                                     where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) IN (  " + M_ASI_ID +
                                     @"," + cd.GetM_AttributeSetInstance_ID() + " )" +
                                     @" AND ced.M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) =  " + cd.GetC_OrderLine_ID() +
                                     @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND cq.M_CostElement_ID = " + ce.GetM_CostElement_ID() +
                                     @" AND ced.AD_Client_ID = " + cd.GetAD_Client_ID();
                            MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }
                        if (MRPrice == 0)
                        {
                            // this is used because we remove the cost queue
                            sql = @"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced LEFT JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                     where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) IN (  " + M_ASI_ID +
                                     @"," + cd.GetM_AttributeSetInstance_ID() + " )" +
                                        @" AND ced.M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                        @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND NVL(cq.M_CostElement_ID , 0)  = 0 " +
                                        @" AND ced.AD_Client_ID = " + cd.GetAD_Client_ID();
                            MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }
                    }
                    else
                    {
                        // case handleed here as    MR Completed with 100 price,  Then Inv with MR completed with 9120 price , then run process 
                        // check record exist on cost element detail only for MR then consider that amount else check with invoiceline ref
                        if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                     " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " + ce.GetM_CostElement_ID() +
                                     " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                     @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                     " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = 0", null, Get_Trx())) > 0)
                        {
                            sql = @"SELECT Amt/Qty FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                         " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " + ce.GetM_CostElement_ID() +
                                         " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                            @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                         " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = 0";
                        }
                        else
                        {
                            sql = @"SELECT Amt/Qty FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                             " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " + ce.GetM_CostElement_ID() +
                                             " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                                @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                             " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = " + cd.GetC_InvoiceLine_ID();
                        }
                        MRPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                        if (MRPrice == 0)
                        {
                            if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                         " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " +
                                         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND AD_Client_ID = " + ce.GetAD_Client_ID() + ")" +
                                         " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                           @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                         " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = 0", null, Get_Trx())) > 0)
                            {
                                sql = @"SELECT Amt/Qty FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                             " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " +
                                             " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND AD_Client_ID = " + ce.GetAD_Client_ID() + ")" +
                                             " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                             @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                             " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = 0";
                            }
                            else
                            {
                                sql = @"SELECT Amt/Qty FROM M_CostElementDetail WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() +
                                                 " AND C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + " AND M_CostElement_ID = " +
                                                 " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'I' AND AD_Client_ID = " + ce.GetAD_Client_ID() + ")" +
                                                 " AND NVL(M_AttributeSetInstance_ID, 0) IN (  " + M_ASI_ID +
                                                 @"," + cd.GetM_AttributeSetInstance_ID() + " )" + " AND M_InOutLine_ID =  " + cd.GetM_InOutLine_ID() +
                                                 " AND NVL(C_OrderLIne_ID , 0) = 0 AND NVL(C_InvoiceLine_ID , 0) = " + cd.GetC_InvoiceLine_ID();
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

            if (A_Asset_ID > 0)
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
                    if (GetC_InvoiceLine_ID() > 0)
                    {
                        invoiceline = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_Trx());
                        invoice = new MInvoice(GetCtx(), invoiceline.GetC_Invoice_ID(), Get_Trx());
                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            // this check is used to get previous invoice price from cost element detail 
                            // if invoice found then set that invoice price else 0
                            string sql = @"select * from (
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.c_acctschema_id ,  ced.c_invoiceline_id , 
                                           ced.m_costelementdetail_id,  ced.M_CostElement_ID,  row_number() over(order by ced.m_costelementdetail_id desc nulls last) rnm
                                           FROM m_costelementdetail ced inner join c_invoiceline il on il.c_invoiceline_id = ced.c_invoiceline_id
                                           inner join c_invoice i on i.c_invoice_id = il.c_invoice_id 
                                           WHERE ced.c_invoiceline_id > 0 AND ced.qty > 0 AND ced.M_CostElement_ID in ( " + ce.GetM_CostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.C_AcctSchema_ID = " + GetC_AcctSchema_ID() +
                                           @" AND ced.M_Product_ID = " + GetM_Product_ID() + @" AND ced.AD_Org_ID = " + Org_ID +
                                           @" AND NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.m_costelementdetail_id DESC )t where rnm <=1";
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
                        mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        //for (int j = 0; j < cQueue.Length; j++)
                        //{
                        //    totalPrice += Decimal.Multiply(cQueue[j].GetCurrentCostPrice(), cQueue[j].GetCurrentQty());
                        //    totalQty += cQueue[j].GetCurrentQty();
                        //}
                        //cost.SetCurrentCostPrice(Decimal.Round((totalPrice / totalQty), precision));
                        cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                    }
                    else if (cQueue.Length == 0)
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                }
                //change 3-5-2016
                if (ce.IsLifo() || ce.IsFifo() || ce.IsAverageInvoice() || ce.IsLastInvoice() || ce.IsStandardCosting() || ce.IsWeightedAverageCost())
                {
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                    //                                             mas, ce.GetM_CostElement_ID(), windowName, cd, GetAmt(), GetQty());
                    //MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                    //                                             mas, ce.GetM_CostElement_ID(), windowName, cd, amtWithSurcharge, GetQty());
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
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
                    if (cd.GetC_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }

                        if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
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
                    if (cd.GetC_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }

                        if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
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
                    if (cd.GetC_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }

                        if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
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
                    if (cd.GetC_OrderLine_ID() > 0)
                    {
                        if (Decimal.Add(cost.GetCurrentQty(), qty) <= 0)
                        {
                            cost.SetCurrentQty(0);
                        }
                        else
                        {
                            if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                        if (costingCheck != null && costingCheck.isMatchFromForm.Equals("N"))
                            cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                    }
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                    }
                    else if (cQueue.Length == 0)
                    {
                        cost.SetCurrentCostPrice(0);
                    }

                }
                if (ce.IsLifo() || ce.IsFifo() || ce.IsAverageInvoice() || ce.IsLastInvoice() || ce.IsStandardCosting() || ce.IsWeightedAverageCost())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
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
            //                string sql = @"SELECT il.C_InvoiceLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice i  ON i.C_Invoice_ID = il.C_Invoice_ID
            //                               WHERE il.IsCostCalculated = 'Y' AND il.ISREVERSEDCOSTCALCULATED = 'N' AND i.DocStatus IN ('CO' , 'CL') AND il.IsActive = 'Y' 
            //                               AND il.M_InoutLine_ID = " + GetM_InOutLine_ID();
            //                int invLineId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, cd.Get_Trx()));
            //                if (invLineId > 0)
            //                {
            //                    invoiceline = new MInvoiceLine(GetCtx(), invLineId, cd.Get_Trx());
            //                    invoice = new MInvoice(GetCtx(), invoiceline.GetC_Invoice_ID(), cd.Get_Trx());
            //                    inoutline = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
            //                    inout = new MInOut(GetCtx(), inoutline.GetM_InOut_ID(), Get_Trx());
            //                    string IsCostCalculated = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsCostCalculated FROM M_MatchInv WHERE C_InvoiceLine_ID = " + invLineId +
            //                        @" AND M_InoutLine_ID = " + GetM_InOutLine_ID(), null, cd.Get_Trx()));
            //                    if (IsCostCalculated == "N" && inout != null && ((inout.GetDescription() != null && !inout.GetDescription().Contains("{->")) || inout.GetDescription() == null))
            //                    {
            //                        // handle cost adjustment on normal loss
            //                        amt = Decimal.Round(Decimal.Divide(invoiceline.GetLineNetAmt(), product.IsCostAdjustmentOnLost() ? inoutline.GetMovementQty() : invoiceline.GetQtyInvoiced()), mas.GetCostingPrecision());
            //                        if (invoice.GetC_Currency_ID() != mas.GetC_Currency_ID())
            //                        {
            //                            amt = MConversionRate.Convert(GetCtx(), amt, invoice.GetC_Currency_ID(), mas.GetC_Currency_ID(),
            //                                                                        invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), cd.GetAD_Client_ID(), cd.GetAD_Org_ID());
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

            //                        MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
            //                                                           mas, ce.GetM_CostElement_ID(), "Invoice(Vendor)", cd, cost.GetCurrentCostPrice() * qty, qty);

            //                        DB.ExecuteQuery("UPDATE M_MatchInv SET IsCostCalculated = 'Y' WHERE C_InvoiceLine_ID = " + invLineId +
            //                        @" AND M_InoutLine_ID = " + GetM_InOutLine_ID(), null, cd.Get_Trx());

            //                        return cost.Save();
            //                    }
            //                }
            //            }

            //	*** Purchase Order Detail Record ***
            if (GetC_OrderLine_ID() != 0 && windowName == "Material Receipt")
            {
                MOrderLine oLine = null;
                #region Material Receipt with Purchase Order
                if (costingCheck != null && costingCheck.orderline != null)
                {
                    oLine = costingCheck.orderline;
                }
                if (oLine == null || oLine.Get_ID() <= 0 || oLine.Get_ID() != GetC_OrderLine_ID())
                {
                    oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), null);
                }
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                if (costingCheck != null && costingCheck.inoutline != null)
                {
                    inoutline = costingCheck.inoutline;
                }
                if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != GetM_InOutLine_ID())
                {
                    inoutline = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
                }

                if (costingCheck != null && costingCheck.inout != null)
                {
                    inout = costingCheck.inout;
                }
                if (inout == null || inout.Get_ID() <= 0 || inout.Get_ID() != inoutline.GetM_InOut_ID())
                {
                    inout = new MInOut(GetCtx(), inoutline.GetM_InOut_ID(), Get_Trx());
                }

                if (ce.IsCostingMethod() && isReturnTrx && inout != null && inout.GetDescription() != null && !inout.GetDescription().Contains("{->")) // -ve Entry on completion of MR
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
                else if (ce.IsCostingMethod() && !isReturnTrx && inout != null && inout.GetDescription() != null && inout.GetDescription().Contains("{->")) // +ve Entry Reverse Case
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
                else if (ce.IsAveragePO())
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

                    log.Finer("PO - AveragePO - " + cost);
                }
                else if (ce.IsWeightedAveragePO() || ce.IsProvisionalWeightedAverage())
                {
                    #region Weighted Av PO / Provisional Weighted Average
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
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.c_acctschema_id ,  ced.m_inoutline_id , 
                                           ced.m_costelementdetail_id,  ced.M_CostElement_ID,  row_number() over(order by ced.m_costelementdetail_id desc nulls last) rnm
                                           FROM m_costelementdetail ced inner join m_inoutline il on il.m_inoutline_id = ced.m_inoutline_id
                                           inner join m_inout i on i.m_inout_id = il.m_inout_id 
                                           WHERE ced.m_inoutline_id > 0 AND ced.C_Orderline_ID > 0 AND ced.qty > 0 AND ced.M_CostElement_ID in ( " + ce.GetM_CostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.C_AcctSchema_ID = " + GetC_AcctSchema_ID() +
                                           @" AND ced.M_Product_ID = " + GetM_Product_ID() + @" AND ced.AD_Org_ID = " + Org_ID +
                                           @" AND NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.m_costelementdetail_id DESC ) where rnm <=1";
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
                        MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                        if (cQueue != null && cQueue.Length > 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                        }
                        else if (cQueue.Length == 0)
                        {
                            // not to set CC as ZERO, bcz if user want to do physical inventory then then can enter stock with previous CC.
                            // cost.SetCurrentCostPrice(0);
                        }
                    }
                }
                //change 3-5-2016
                if (ce.IsAveragePO() || ce.IsLastPOPrice() || ce.IsWeightedAveragePO())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                #endregion
            }

            //	*** AP Invoice Detail Record ***
            else if (GetC_InvoiceLine_ID() != 0 && windowName != "Material Receipt" && windowName != "Invoice(Vendor)-Return")
            {
                #region AP Invoice Detail Record
                bool isReturnTrx = Env.Signum(qty) < 0;

                if (GetM_InOutLine_ID() > 0 || GetC_OrderLine_ID() > 0)
                {
                    if (costingCheck != null && costingCheck.invoiceline != null)
                    {
                        invoiceline = costingCheck.invoiceline;
                    }
                    if (invoiceline == null || invoiceline.Get_ID() <= 0 || invoiceline.Get_ID() != GetC_InvoiceLine_ID())
                    {
                        invoiceline = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_Trx());
                    }

                    if (costingCheck != null && costingCheck.invoice != null)
                    {
                        invoice = costingCheck.invoice;
                    }
                    if (invoice == null || invoice.Get_ID() <= 0 || invoice.Get_ID() != invoiceline.GetC_Invoice_ID())
                    {
                        invoice = new MInvoice(GetCtx(), invoiceline.GetC_Invoice_ID(), Get_Trx());
                    }
                    // checking invoice contain reverse invoice ref or not
                }
                bool isProvisnalInvcalculated = invoiceline != null ? !((invoiceline.Get_ColumnIndex("C_ProvisionalInvoiceLine_ID") >= 0 &&
                                   invoiceline.Get_ValueAsInt("C_ProvisionalInvoiceLine_ID") <= 0)
                                   || invoiceline.Get_ColumnIndex("C_ProvisionalInvoiceLine_ID") < 0) : false;

                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_LandedCostAllocation_ID) FROM C_LandedCostAllocation WHERE  C_InvoiceLine_ID = " + GetC_InvoiceLine_ID(), null, Get_Trx())) <= 0)
                {
                    if (GetC_OrderLine_ID() == 0) // if invoice created without orderline  then no impact on cost
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
                            else if (ce.IsWeightedAverageCost() || ce.IsWeightedAveragePO())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                price = Decimal.Round(Decimal.Divide(
                                        Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt), cost.GetCurrentQty()), precision);
                                cost.SetCurrentCostPrice(price);
                            }
                            else if (ce.IsStandardCosting() || ce.IsLastInvoice() || ce.IsLastPOPrice())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            }
                            else if (ce.IsFifo() || ce.IsLifo())
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));

                                // we have to reduce price
                                if (amt < 0 && price > 0)
                                {
                                    price = decimal.Negate(price);
                                }

                                // get Cost Queue List Detail
                                MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                                                      mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                                if (cQueue != null && cQueue.Length > 0)
                                {
                                    cost.SetCurrentCostPrice(Decimal.Round((cQueue[0].GetCurrentCostPrice() + price), precision));

                                    DB.ExecuteQuery("Update M_CostQueue SET CurrentCostPrice = " + (cQueue[0].GetCurrentCostPrice() + price) +
                                                    @" WHERE M_CostQueue_ID = " + cQueue[0].GetM_CostQueue_ID(), null, Get_Trx());
                                }
                            }
                            //change 3-5-2016
                            MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                            mas, ce.GetM_CostElement_ID(), "Invoice(APC)", cd, cost.GetCurrentCostPrice() * qty, qty);
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
                        if (isReturnTrx && invoice != null && invoice.GetDescription() != null && !invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else
                        {
                            if (!isProvisnalInvcalculated)
                            {
                                cost.Add(amt, qty);
                            }
                            else if (isProvisnalInvcalculated)
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
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
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (!isProvisnalInvcalculated)
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
                            }
                            else if (Decimal.Add(cost.GetCurrentQty(), qty) == 0 && !isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(0);
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                price = Decimal.Round(Decimal.Divide(
                                                       Decimal.Add(
                                                       Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                       Decimal.Add(cost.GetCurrentQty(), qty))
                                                       , precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                            else if (isProvisnalInvcalculated)
                            {
                                price = Decimal.Round(Decimal.Divide(
                                                          Decimal.Add(
                                                          Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt),
                                                          cost.GetCurrentQty()), precision, MidpointRounding.AwayFromZero);
                                cost.SetCurrentCostPrice(price);
                            }
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            if (!isProvisnalInvcalculated)
                            {
                                cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
                            }
                            log.Finer("Inv - WeightedAverageCost - " + cost);
                        }
                        #endregion
                    }
                }
                else if ((ce.IsFifo() || ce.IsLifo()) && windowName != "Invoice(Customer)")
                {
                    #region Lifo / Fifo
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                    }
                    if (cQueue.Length == 0)
                    {
                        // not to set CC as ZERO, bcz if user want to do physical inventory then then can enter stock with previous CC.
                        // cost.SetCurrentCostPrice(0);
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
                        }
                        else if (!isProvisnalInvcalculated)
                        {
                            cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                        }
                    }
                    else
                    {
                        if (windowName != "Invoice(Customer)" && !isProvisnalInvcalculated)
                        {
                            cost.Add(amt, qty);
                        }
                        else if (isProvisnalInvcalculated)
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                        }
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
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->")) // +ve Entry for reverse
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else
                        {
                            if (!isProvisnalInvcalculated)
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
                            }
                            else if (isProvisnalInvcalculated)
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                                // we have to reduce price
                                if (amt < 0 && price > 0)
                                {
                                    price = decimal.Negate(price);
                                }
                                cost.SetCurrentCostPrice(Decimal.Add(cost.GetCurrentCostPrice(), price));
                            }
                            // this check is used to get previous invoice price from cost element detail 
                            // if invoice found then set that invoice price else 0
                            // this block is executed for reverse record
                            // this block wll not execute when provisonal invoice linked with invoice
                            if (GetC_InvoiceLine_ID() > 0 && isReturnTrx && !isProvisnalInvcalculated)
                            {
                                invoiceline = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_Trx());
                                invoice = new MInvoice(GetCtx(), invoiceline.GetC_Invoice_ID(), Get_Trx());
                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                {
                                    string sql = @"select * from (
                                           SELECT ced.qty , ced.amt , ced.amt/ced.qty AS price , ced.c_acctschema_id ,  ced.c_invoiceline_id , 
                                           ced.m_costelementdetail_id,  ced.M_CostElement_ID,  row_number() over(order by ced.m_costelementdetail_id desc nulls last) rnm
                                           FROM m_costelementdetail ced inner join c_invoiceline il on il.c_invoiceline_id = ced.c_invoiceline_id
                                           inner join c_invoice i on i.c_invoice_id = il.c_invoice_id 
                                           WHERE ced.c_invoiceline_id > 0 AND ced.M_Inoutline_ID > 0 AND ced.qty > 0 AND ced.M_CostElement_ID in ( " + ce.GetM_CostElement_ID() + @" ) 
                                           and i.docstatus in ('CO' , 'CL') AND ced.C_AcctSchema_ID = " + GetC_AcctSchema_ID() +
                                                   @" AND ced.M_Product_ID = " + GetM_Product_ID() + @" AND ced.AD_Org_ID = " + Org_ID +
                                                   @" AND NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @"
                                           ORDER BY ced.m_costelementdetail_id DESC ) where rnm <=1";
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
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else if (!isReturnTrx && invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            if (Decimal.Add(cost.GetCurrentQty(), qty) < 0)
                            {
                                return false;
                            }
                            else if (!isProvisnalInvcalculated)
                            {
                                cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                            }
                        }
                        else
                        {
                            if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                            {
                                cost.SetCurrentCostPrice(price);
                                //	seed initial price
                                if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                                {
                                    cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                            mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
                                }
                            }
                            if (!isProvisnalInvcalculated)
                            {
                                cost.Add(amt, qty);
                            }
                            else if (isProvisnalInvcalculated)
                            {
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), amt));
                            }
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
                    // system calculating expected landed cost on invoice completion
                    if (IsCalculateExpectedLandedCostFromInvoice())
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
                    }
                    else
                    {
                        // when expected cost already calculated, and during actual adjustment - current qty not available then no effects comes
                        if (GetExpectedCostCalculated() && cost.GetCurrentQty() == 0)
                        {
                            return true;
                        }

                        Decimal cCosts = Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt);
                        // when expected cost already calculated, then not to add qty 
                        //Decimal qty1 = Decimal.Add(cost.GetCurrentQty(), GetExpectedCostCalculated() ? 0 : qty);

                        Decimal qty1 = MCost.GetproductCostAndQtyMaterial(cd.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID()
                            , cost.GetM_AttributeSetInstance_ID(), cost.Get_Trx(), cost.GetM_Warehouse_ID(), true);
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
                }
                //change 3-5-2016
                if (ce.IsWeightedAverageCost() && windowName != "Invoice(Customer)")
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                else if ((ce.IsFifo() || ce.IsLifo() || ce.IsStandardCosting() || ce.IsLastInvoice() || ce.IsAverageInvoice()) && windowName != "Invoice(Customer)")
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                }
                #endregion
            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetM_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetM_MovementLine_ID() != 0
                || GetM_InventoryLine_ID() != 0
                || GetM_ProductionLine_ID() != 0
                || GetC_ProjectIssue_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetVAMFG_M_WrkOdrTrnsctionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0
                || Util.GetValueOfInt(Get_Value("VAFAM_AssetDisposal_ID")) != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                if (GetM_InOutLine_ID() > 0)
                {
                    if (costingCheck != null && costingCheck.inoutline != null)
                    {
                        inoutline = costingCheck.inoutline;
                    }
                    if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != GetM_InOutLine_ID())
                    {
                        inoutline = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
                    }

                    if (costingCheck != null && costingCheck.inout != null)
                    {
                        inout = costingCheck.inout;
                    }
                    if (inout == null || inout.Get_ID() <= 0 || inout.Get_ID() != inoutline.GetM_InOut_ID())
                    {
                        inout = new MInOut(GetCtx(), inoutline.GetM_InOut_ID(), Get_Trx());
                    }
                }
                if (GetM_MovementLine_ID() > 0)
                {
                    movementline = new MMovementLine(GetCtx(), GetM_MovementLine_ID(), Get_Trx());
                    movement = new MMovement(GetCtx(), movementline.GetM_Movement_ID(), Get_Trx());
                }

                // if current cost price avialble then add that amount else the same scenario
                if (windowName.Equals("Shipment"))
                {
                    amt = cost.GetCurrentCostPrice() * qty;
                }
                else if (windowName.Equals("Invoice(Vendor)-Return") || windowName.Equals("Material Receipt"))
                {
                    // In case of Invoice against Return To Vendor - impacts come with Invoice Amount
                    amt = GetAmt();
                }
                else if (windowName.Equals("Return To Vendor") && GetC_OrderLine_ID() > 0)
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                // get cost based on Accounting Schema
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
                                //if (primaryActSchemaCurrencyID != mas.GetC_Currency_ID())
                                //{
                                //    costCombination = MConversionRate.Convert(movement.GetCtx(), costCombination, primaryActSchemaCurrencyID, mas.GetC_Currency_ID(),
                                //                                     movement.GetMovementDate(), 0, movement.GetAD_Client_ID(), movement.GetAD_Org_ID());
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
                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), cost.GetM_Product_ID(),
                        //       cost.GetM_AttributeSetInstance_ID(), cost.Get_Trx(), cost.GetM_Warehouse_ID());
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(costCombination, qty)));
                        //}
                        //else
                        //{
                        //    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(cost.GetCurrentCostPrice(), qty)));
                        //}
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(movementline.GetCurrentCostPrice(), qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            Decimal convertAmount = MConversionRate.Convert(GetCtx(), movementline.GetCurrentCostPrice(),
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
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

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), cost.GetM_Product_ID(),
                        //       cost.GetM_AttributeSetInstance_ID(), cost.Get_Trx(), cost.GetM_Warehouse_ID());
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
                                if (windowName.Equals("Return To Vendor") && GetC_OrderLine_ID() > 0)
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), cost.GetM_Product_ID(),
                        //       cost.GetM_AttributeSetInstance_ID(), cost.Get_Trx(), cost.GetM_Warehouse_ID());
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
                        if (GetC_OrderLine_ID() > 0)
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
                            else if (GetC_OrderLine_ID() > 0)
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        else if (GetC_OrderLine_ID() > 0)
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                            if (amt == 0 && cost.GetCurrentCostPrice() != 0)
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), cost.GetM_Product_ID(),
                        //       cost.GetM_AttributeSetInstance_ID(), cost.Get_Trx(), cost.GetM_Warehouse_ID());
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

                    }

                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue;
                    if (windowName == "Inventory Move")
                    {
                        cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                            mas, cq_AD_Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                    }
                    else
                    {
                        cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                            mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                    }
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        // we have to update current cost price either of first record having current qty <> 0 (IN FIFO) and vice versa for LIFO
                        cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                    }
                    else
                    {
                        // not to set CC as ZERO, bcz if user want to do physical inventory then then can enter stock with previous CC.
                        // cost.SetCurrentCostPrice(0);
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                        //       costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                                if (windowName.Equals("Return To Vendor") && GetC_OrderLine_ID() > 0)
                                {
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), qty));
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                        //       costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (GetC_OrderLine_ID() > 0)
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
                            if (ce.GetM_CostElement_ID() == costCominationelement)
                            {
                                decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                                   costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                        if (mas.GetC_Currency_ID() == GetCtx().GetContextAsInt("$C_Currency_ID"))
                        {
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }
                        else
                        {
                            // convert currenctCostPrice on movementline in accounting schema currency
                            affectedAmount = MConversionRate.Convert(GetCtx(), affectedAmount,
                                GetCtx().GetContextAsInt("$C_Currency_ID"), mas.GetC_Currency_ID(), movement.GetMovementDate(),
                                0, movement.GetAD_Client_ID(), cost.GetAD_Org_ID());
                            cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Multiply(affectedAmount, qty)));
                        }

                        //if (ce.GetM_CostElement_ID() == costCominationelement)
                        //{
                        //    decimal costCombination = MCost.GetproductCostBasedonAcctSchema(costFrom.GetAD_Client_ID(), costFrom.GetAD_Org_ID(), mas.GetC_AcctSchema_ID(), costFrom.GetM_Product_ID(),
                        //       costFrom.GetM_AttributeSetInstance_ID(), costFrom.Get_Trx(), costFrom.GetM_Warehouse_ID());
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
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd,
                                                    (cost.GetCurrentCostPrice() * qty) != 0 ? (cost.GetCurrentCostPrice() * qty) : amt, qty);
                }
                else if (!ce.IsWeightedAverageCost())
                {
                    MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                    mas, ce.GetM_CostElement_ID(), windowName, cd, amt, qty);
                }
            }
            else if (GetC_ProvisionalInvoiceLine_ID() != 0)
            {
                #region AP Invoice Detail Record
                bool isReturnTrx = Env.Signum(qty) < 0;
                DataSet dsProvisionlInvoice = DB.ExecuteDataset(@"SELECT IsSOTrx,IsReturnTrx, IsReversal FROM C_ProvisionalInvoice pi INNER JOIN 
                                                C_ProvisionalInvoiceLine pil ON pi.C_ProvisionalInvoice_ID = pil.C_ProvisionalInvoice_ID 
                                                WHERE C_ProvisionalInvoiceLine_ID = " + GetC_ProvisionalInvoiceLine_ID(), null, Get_Trx());
                if (dsProvisionlInvoice != null && dsProvisionlInvoice.Tables.Count > 0 && dsProvisionlInvoice.Tables[0].Rows.Count > 0)
                {
                    bool RecordIsSOtrx = Util.GetValueOfString(dsProvisionlInvoice.Tables[0].Rows[0]["IsSOTrx"]).Equals("Y");
                    bool RecordIsReturnTrx = Util.GetValueOfString(dsProvisionlInvoice.Tables[0].Rows[0]["IsReturnTrx"]).Equals("Y");
                    bool RecordIsReversal = Util.GetValueOfString(dsProvisionlInvoice.Tables[0].Rows[0]["IsReversal"]).Equals("Y");
                    if (ce.IsAverageInvoice() && windowName != "Invoice(Customer)")
                    {
                        if (!RecordIsSOtrx && RecordIsReturnTrx) // Invoice Vendor with Vendor RMA
                        {
                        }
                        else
                        {
                            #region Av Invoice

                            if ((isReturnTrx && !RecordIsReversal) || (!isReturnTrx && RecordIsReversal))
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
                                log.Finer("Provisional Invoice - AverageInv - " + cost);
                            }
                            #endregion
                        }
                    }
                    else if (ce.IsWeightedAverageCost() && windowName != "Invoice(Customer)")
                    {
                        if (!RecordIsSOtrx && RecordIsReturnTrx) // Invoice Vendor with Vendor RMA
                        {
                        }
                        else
                        {
                            #region Weighted Av Invoice
                            if ((isReturnTrx && !RecordIsReversal) || (!isReturnTrx && RecordIsReversal))
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
                            else
                            {
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
                                log.Finer("Inv - WeightedAverageCost - " + cost);
                            }
                            #endregion
                        }
                    }
                    else if ((ce.IsFifo() || ce.IsLifo()) && windowName != "Invoice(Customer)")
                    {
                        #region Lifo / Fifo
                        MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                            mas, Org_ID, ce, Get_TrxName(), cost.GetM_Warehouse_ID());
                        if (cQueue != null && cQueue.Length > 0)
                        {
                            cost.SetCurrentCostPrice(Decimal.Round(cQueue[0].GetCurrentCostPrice(), precision));
                        }
                        if (cQueue.Length == 0)
                        {
                            // not to set CC as ZERO, bcz if user want to do physical inventory then then can enter stock with previous CC.
                            // cost.SetCurrentCostPrice(0);
                        }

                        if (((RecordIsSOtrx && RecordIsReturnTrx)
                            || (!RecordIsSOtrx && RecordIsReturnTrx)))
                        {
                            // Nothing happen when Invoice Vendor is created  for Return to vendor
                            // Nothing happen when Invoice Customer is created  for Customer Return
                        }
                        else if ((isReturnTrx && !RecordIsReversal)  // if Invoice is -ve and try to complete
                            || (!isReturnTrx && RecordIsReversal))    //if Invoice become +ve after reverse and its reverse entr) 
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
                        else
                        {
                            cost.Add(amt, qty);
                        }
                        log.Finer("Inv - FiFo/LiFo - " + cost);
                        #endregion
                    }
                    else if (ce.IsLastInvoice() && windowName != "Invoice(Customer)")
                    {
                        if (!RecordIsSOtrx && RecordIsReturnTrx) // Invoice Vendor with Vendor RMA
                        {
                        }
                        else
                        {
                            #region last Invoice
                            if ((isReturnTrx && !RecordIsReversal) // -ve entry for completion
                                || (!isReturnTrx && RecordIsReversal)) // +ve Entry for reverse
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
                            }
                            log.Finer("Inv - LastInv - " + cost);
                            #endregion
                        }
                    }
                    else if (ce.IsStandardCosting() && windowName != "Invoice(Customer)")
                    {
                        if (!RecordIsSOtrx && RecordIsReturnTrx) // Invoice Vendor with Vendor RMA
                        {
                        }
                        else
                        {
                            #region Std Costing
                            if ((isReturnTrx && !RecordIsReversal) || (!isReturnTrx && RecordIsReversal))
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
                            else
                            {
                                if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                                {
                                    cost.SetCurrentCostPrice(price);
                                    //	seed initial price
                                    if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                                    {
                                        cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                                mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
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
                    else if (!ce.IsCostingMethod())     //	Cost Adjustments
                    {
                        // system calculating expected landed cost on invoice completion
                        if (IsCalculateExpectedLandedCostFromInvoice())
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
                        }
                    }

                    if (ce.IsWeightedAverageCost() && windowName != "Invoice(Customer)")
                    {
                        MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                        mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                    }
                    else if ((ce.IsFifo() || ce.IsLifo() || ce.IsStandardCosting() || ce.IsLastInvoice() || ce.IsAverageInvoice()) && windowName != "Invoice(Customer)")
                    {
                        MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                                        mas, ce.GetM_CostElement_ID(), windowName, cd, (cost.GetCurrentCostPrice() * qty), qty);
                    }
                }
                #endregion
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }
            if (A_Asset_ID != 0)
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
        /// <param name="cq_AD_Org_ID">org</param>
        /// <param name="windowName">window name</param>
        /// <param name="optionalStrcc">optional para : process or window</param>
        /// <returns>true, when calculated</returns>
        public bool CreateCostForCombination(MCostDetail cd, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, int cq_AD_Org_ID, string windowName, string optionalStrcc = "process")
        {
            string sql;
            int AD_Org_ID = 0;
            // Get Org based on Costing Level
            dynamic pc = null;
            String cl = null;
            MCostElement ce = null;
            string costingMethod = null;
            int costElementId1 = 0;
            Decimal AccQty = 0;
            int M_Warehouse_ID = 0;
            if (product != null)
            {
                pc = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
                if (pc != null)
                {
                    cl = pc.GetCostingLevel();
                    costingMethod = pc.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costElementId1 = pc.GetM_CostElement_ID();
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
                    costElementId1 = acctSchema.GetM_CostElement_ID();
                }
            }
            if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_BatchLot)
            {
                AD_Org_ID = 0;
                // during Inventory Move, not to calculate cost combination
                if (windowName.Equals("Inventory Move"))
                    return true;
            }
            else
            {
                AD_Org_ID = cd.GetAD_Org_ID();
            }
            if (cl != MProductCategory.COSTINGLEVEL_BatchLot && cl != MProductCategory.COSTINGLEVEL_OrgPlusBatch && cl != MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_ASI_ID = 0;
            }
            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
            {
                M_Warehouse_ID = cd.GetM_Warehouse_ID();
            }

            // Freight Distribution
            if (!FreightDistribution(windowName, cd, acctSchema, AD_Org_ID, product, M_ASI_ID, M_Warehouse_ID))
            {
                return false;
            }

            // when we complete a record, and costing method is not any combination, then no need to calculate
            if (optionalStrcc == "window" && costingMethod != "C")
            {
                return true;
            }

            // Get Cost element of Cost Combination type
            sql = @"SELECT ce.M_CostElement_ID ,  ce.Name ,  cel.lineno ,  cel.m_ref_costelement , 
                      (SELECT CASE  WHEN costingmethod IS NOT NULL THEN 1  ELSE 0 END  FROM m_costelement WHERE m_costelement_id = CAST(cel.M_Ref_CostElement AS INTEGER) ) AS iscostMethod 
                            FROM M_CostElement ce INNER JOIN m_costelementline cel ON ce.M_CostElement_ID = cel.M_CostElement_ID "
                          + "WHERE ce.AD_Client_ID=" + GetAD_Client_ID()
                          + " AND ce.IsActive='Y' AND ce.CostElementType='C' AND cel.IsActive='Y' ";
            if (optionalStrcc == "window" && costingMethod == "C")
            {
                sql += " AND ce.M_CostElement_ID = " + costElementId1;
            }
            sql += "ORDER BY ce.M_CostElement_ID , iscostMethod DESC";
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
                        moveline = new MMovementLine(GetCtx(), cd.GetM_MovementLine_ID(), Get_Trx());
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
                    //        // UpdateFreightWithActualQty(ds, AD_Org_ID, cd, acctSchema, product, M_ASI_ID, windowName, M_Warehouse_ID);
                    //    }
                    //}

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        costCombination = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]), M_Warehouse_ID);

                        //If cost combination is already calculated on completion, then not to re-calculate through process
                        if (windowName.Equals("LandedCostAllocation"))
                        {
                            costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]);
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
                            costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]);
                        }
                        if (costElementId != Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]))
                        {
                            if (windowName == "Inventory Move" &&
                                (cl.Equals(MProductCategory.COSTINGLEVEL_Client) || cl.Equals(MProductCategory.COSTINGLEVEL_BatchLot)))
                            {
                                // do not create Cost combination entry in case of ineventory move and costing level is client or Batch/lot
                            }
                            else if (windowName.Equals("LandedCostAllocation") && (cl == "C" || cl == "B"))
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), 0, product, M_ASI_ID,
                                              acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                            else
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
                                               acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                        }
                        costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]);

                        //If cost combination is already calculated on completion, then not to re-calculate through process
                        if (windowName.Equals("LandedCostAllocation"))
                        {
                            isCurrentCostprice = true;
                        }

                        // created object of Cost elemnt for checking iscalculated = true/ false
                        ce = MCostElement.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]));
                        // if m_ref_costelement is of Freight type then current cost against this record is :: 

                        costCombination = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]), M_Warehouse_ID);
                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]), M_Warehouse_ID);
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
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), 0, product, M_ASI_ID,
                                              acctSchema, costElementId, "Cost Comination", cd, (costCombination.GetCurrentCostPrice() * cd.GetQty()), cd.GetQty());
                            }
                            else
                            {
                                MCostElementDetail.CreateCostElementDetail(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, M_ASI_ID,
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
        /// <param name="AD_Org_ID">organization</param>
        /// <param name="product">product object</param>
        /// <param name="M_ASI_ID">Attribute set instance</param>
        /// <param name="M_Warehouse_ID">warehouse reference</param>
        /// <returns>true when success</returns>
        public bool FreightDistribution(String windowName, MCostDetail cd, MAcctSchema acctSchema, int AD_Org_ID, MProduct product, int M_ASI_ID, int M_Warehouse_ID)
        {
            // is used to get current qty of defined costing method on Product category or Accounting schema
            Decimal Qty = MCost.GetproductCostAndQtyMaterial(cd.GetAD_Client_ID(), AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, cd.Get_Trx(), M_Warehouse_ID, true);
            if (Qty == 0 && cd.GetM_CostElement_ID() > 0)
            {
                // check, is this kind of Custome or Freight (bypass)
                int count = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(M_CostElement_ID) FROM M_CostElement WHERE M_CostElement_ID = " + cd.GetM_CostElement_ID() +
                           @" AND CostElementType = '" + X_M_CostElement.COSTELEMENTTYPE_Material + "' AND CostingMethod IS NULL"));
                if (count > 0)
                {
                    return true;
                }
            }

            bool isReversed = false;
            if (windowName.Equals("Inventory Move"))
            {
                isReversed = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM M_MovementLine WHERE M_MovementLine_ID = "
                            + cd.GetM_MovementLine_ID(), null, cd.Get_Trx())) > 0 ? true : false;
            }

            // Get Element which belongs to Landed Cost
            String sql = "SELECT M_CostElement_ID , Name FROM M_CostElement WHERE CostElementType = '" + MCostElement.COSTELEMENTTYPE_Material +
                        @"' AND CostingMethod IS NULL AND IsActive = 'Y' AND AD_Client_ID = " + cd.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql, null, cd.Get_Trx());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                MCost cost = null;
                Decimal currentCost = 0.0M;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, Convert.ToInt32(ds.Tables[0].Rows[i]["M_CostElement_ID"]), M_Warehouse_ID);
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
                        _log.Info("Error occured during save/update a record on M_Cost for Cost Element = " + Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) +
                            @". Error Name : " + (pp != null ? pp.GetName() : ""));
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Is used to create entry on M_FreightImpact during consumption
        /// when we consume product, and landed cost is calculated against the same product then we have to reduce the same qty from the freight
        /// but when we REVERSE record, qty will increase, then we check FREIGHT qty is affected -- if YES, then increase the current qty for landed cost.
        /// </summary>
        /// <param name="dsCostElementLine">dataset of cost element against cost combination for which we are calculate cost</param>
        /// <param name="AD_Org_ID">Org for geeting Product cost entry</param>
        /// <param name="cd">cost detail of transaction</param>
        /// <param name="acctSchema">Accounting schema</param>
        /// <param name="product">Product, whose cost is to be calculated</param>
        /// <param name="M_ASI_ID">Attribute Set Instance, whose cost is to be calculated</param>
        /// <param name="windowName">consume/Increase by which window</param>
        /// <returns>true or false</returns>
        public bool UpdateFreightWithActualQty(DataSet dsCostElementLine, int AD_Org_ID, MCostDetail cd, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, string windowName, int M_Warehouse_ID = 0)
        {
            MCost cost = null;
            for (int i = 0; i < dsCostElementLine.Tables[0].Rows.Count; i++)
            {
                // when costing method is not of landed cost type, then continue.
                if (Util.GetValueOfInt(dsCostElementLine.Tables[0].Rows[i]["iscostMethod"]) == 1)
                    continue;

                int landedM_CostElement_ID = Util.GetValueOfInt(dsCostElementLine.Tables[0].Rows[i]["m_ref_costelement"]);

                // when we consume qty then we check, already consume qty agaisnt landed cost -- if consumend then not to take impacts
                if (cd.GetQty() < 0)
                {
                    if (IsAlreadyFreightImpacted(windowName, cd, landedM_CostElement_ID) > 0)
                        continue;
                }

                // object of Landed cost allocation - cost element
                cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, landedM_CostElement_ID, M_Warehouse_ID);
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
                        CreateFreightImpacts(windowName, cd, Math.Abs(cd.GetQty()), landedM_CostElement_ID);
                    }
                    else
                    {
                        CreateFreightImpacts(windowName, cd, cost.GetCurrentQty(), landedM_CostElement_ID);
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
                    int freightImpactID = IsAlreadyFreightImpacted(windowName, cd, landedM_CostElement_ID);
                    DB.ExecuteQuery("Delete FROM M_FreightImpact WHERE M_FreightImpact_ID = " + freightImpactID, null, Get_Trx());
                }
            }
            return true;
        }

        /// <summary>
        /// Is used to create entry on M_FreightImpact
        /// when we consume product, and landed cost is calculated against the same product then we have to reduce the same qty from the freight
        /// will create this entry -- with those qty which is affected or reduce from the freight
        /// </summary>
        /// <param name="windowName">consume by which window</param>
        /// <param name="cd">cost detail record against line</param>
        /// <param name="qty">qty which is to be affected or reduce from freight</param>
        /// <returns>record save or not</returns>
        /// <writer>Amit Bansal</writer>
        public bool CreateFreightImpacts(string windowName, MCostDetail cd, Decimal qty, int landedM_CostElement_ID)
        {
            int tableId = 0;
            int recordId = 0;
            if (windowName == "Shipment")
            {
                tableId = MTable.Get_Table_ID("M_InOut");
                recordId = cd.GetM_InOutLine_ID();
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MTable.Get_Table_ID("M_Inventory");
                recordId = cd.GetM_InventoryLine_ID();
            }
            else if (windowName == "Production Execution")
            {
                tableId = MTable.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
                recordId = cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID();
            }
            else if (windowName == "Inventory Move")
            {
                tableId = MTable.Get_Table_ID("M_Movement");
                recordId = cd.GetM_MovementLine_ID();
            }

            X_M_FreightImpact freightImpact = new X_M_FreightImpact(GetCtx(), 0, Get_Trx());
            freightImpact.SetAD_Org_ID(cd.GetAD_Org_ID());
            freightImpact.SetAD_Table_ID(tableId);
            freightImpact.SetRecord_ID(recordId);
            freightImpact.SetC_AcctSchema_ID(cd.GetC_AcctSchema_ID());
            freightImpact.SetM_CostElement_ID(landedM_CostElement_ID);
            freightImpact.SetM_Product_ID(cd.GetM_Product_ID()); // when we re-calculate, at that tym we have to delete record from from m_freightimpact based on product
            freightImpact.SetQty(qty);
            if (!freightImpact.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured during saving a record in M_FreightImpact. Error Name : " + (pp != null ? pp.GetName() : ""));
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Is used to check and get qty from M_FreightImpact 
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
                tableId = MTable.Get_Table_ID("M_InOut");
                sql = @"SELECT Qty FROM M_FreightImpact WHERE AD_Table_ID = " + tableId +
                      @" AND Record_ID = (SELECT ReversalDoc_ID FROM M_InOutline WHERE M_InOutLine_ID =" + cd.GetM_InOutLine_ID() + " )";
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MTable.Get_Table_ID("M_Inventory");
                sql = @"SELECT Qty FROM M_FreightImpact WHERE AD_Table_ID = " + tableId +
                     @" AND Record_ID = (SELECT ReversalDoc_ID FROM M_Inventoryline WHERE M_InventoryLine_ID =" + cd.GetM_InventoryLine_ID() + " )";
            }
            else if (windowName == "Production Execution") // not handled its reverse
            {
                tableId = MTable.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
                sql = @"SELECT Qty FROM M_FreightImpact WHERE AD_Table_ID = " + tableId +
                      @" AND Record_ID = (SELECT ReversalDoc_ID FROM VAMFG_M_WrkOdrTrnsctionLine WHERE VAMFG_M_WrkOdrTrnsctionLine_ID =" + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID() + " )";
            }
            else if (windowName == "Inventory Move")
            {
                tableId = MTable.Get_Table_ID("M_Movement");
                sql = @"SELECT Qty FROM M_FreightImpact WHERE AD_Table_ID = " + tableId +
                    @" AND Record_ID = (SELECT ReversalDoc_ID FROM M_Movementline WHERE M_MovementLine_ID =" + cd.GetM_MovementLine_ID() + " )";
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
        /// <param name="landedM_CostElement_ID">landed cost element detail</param>
        /// <returns>M_FreightImpact_ID</returns>
        public int IsAlreadyFreightImpacted(string windowName, MCostDetail cd, int landedM_CostElement_ID)
        {
            int tableId = 0;
            int recordId = 0;
            if (windowName == "Shipment")
            {
                tableId = MTable.Get_Table_ID("M_InOut");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM M_InOutline WHERE M_InOutLine_ID =" + cd.GetM_InOutLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetM_InOutLine_ID();
                }
            }
            else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
            {
                tableId = MTable.Get_Table_ID("M_Inventory");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM M_Inventoryline WHERE M_InventoryLine_ID =" + cd.GetM_InventoryLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetM_InventoryLine_ID();
                }
            }
            else if (windowName == "Production Execution")
            {
                tableId = MTable.Get_Table_ID("VAMFG_M_WrkOdrTransaction");
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
                tableId = MTable.Get_Table_ID("M_Movement");
                if (cd.GetQty() > 0)
                {
                    recordId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ReversalDoc_ID FROM M_Movementline WHERE M_MovementLine_ID =" + cd.GetM_MovementLine_ID(), null, Get_Trx()));
                }
                else
                {
                    recordId = cd.GetM_MovementLine_ID();
                }
            }
            int freightImpactId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_FreightImpact_ID FROM M_FreightImpact WHERE Record_ID = " + recordId +
                                                    @" AND AD_Table_ID = " + tableId + @" AND C_AcctSchema_ID = " + cd.GetC_AcctSchema_ID() +
                                                    @" AND M_CostElement_ID = " + landedM_CostElement_ID, null, Get_Trx()));

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

        /// <summary>
        /// Set expected landed cost to be  calculated from invoice vendor
        /// </summary>
        /// <param name="IsCalculateExpectedCostFromInvoice">set is expected landed cost calculated or not from invoice</param>
        public void SetCalculateExpectedLandedCostFromInvoice(bool IsCalculateExpectedCostFromInvoice)
        {
            _IsCalculateExpectedLandedCostFromInvoice = IsCalculateExpectedCostFromInvoice;
        }

        /// <summary>
        /// Getter Property
        /// </summary>
        /// <returns>true, when expected landed costto be  calculated from invoice vendor</returns>
        public bool IsCalculateExpectedLandedCostFromInvoice()
        {
            return _IsCalculateExpectedLandedCostFromInvoice;
        }
        //end

        /// <summary>
        /// Create New Order Cost Detail for Physical Inventory.
        /// Called from Doc_Inventory
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_InventoryLine_ID">order</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateInventory(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_InventoryLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_InventoryLine_ID=" + M_InventoryLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_InventoryLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_InventoryLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_InventoryLine_ID(M_InventoryLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_InventoryLine_ID(M_InventoryLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Invoice Cost Detail for AP Invoices.
        /// Called from Doc_Invoice - for Invoice Adjustments
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="C_InvoiceLine_ID">invoice</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateInvoice(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int C_InvoiceLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND C_InvoiceLine_ID=" + C_InvoiceLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "C_InvoiceLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                C_InvoiceLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,                              M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);

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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_MovementLine_ID">movement</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="from">if true the from (reduction)</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateMovement(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_MovementLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, bool from, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_MovementLine_ID=" + M_MovementLine_ID
                + " AND IsSOTrx=" + (from ? "'Y'" : "'N'")
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_MovementLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2 AND IsSOTrx="
                + (from ? "'Y'" : "'N'"),
                M_MovementLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_MovementLine_ID(M_MovementLine_ID);
                cd.SetIsSOTrx(from);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,                        M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_MovementLine_ID(M_MovementLine_ID);
                    cd.SetIsSOTrx(from);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="C_OrderLine_ID">order</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateOrder(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int C_OrderLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {

            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND C_OrderLine_ID=" + C_OrderLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "C_OrderLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                C_OrderLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew cost deatil for selected product
            {
                cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID, Amt, Qty, Description, trxName);
                cd.SetC_OrderLine_ID(C_OrderLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetC_OrderLine_ID(C_OrderLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_CostElement_ID"></param>
        /// <param name="Amt"></param>
        /// <param name="Qty"></param>
        /// <param name="Description"></param>
        /// <param name="trxName"></param>
        /// <param name="RectifyPostedRecords"></param>
        /// <param name="Id"></param>
        //private static void CostSetByProcess(MCostDetail cd, MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
        //    int M_AttributeSetInstance_ID, int M_CostElement_ID, Decimal Amt, Decimal Qty,
        //    String Description, Trx trx, bool RectifyPostedRecords)
        //{
        //    if (RectifyPostedRecords)
        //    {
        //        cd.SetProcessed(false);
        //        if (cd.Delete(true, trxName))
        //        {
        //            cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
        //                M_CostElement_ID, Amt, Qty, Description, trxName);
        //        }
        //    }
        //}

        /// <summary>
        /// Create New Order Cost Detail for Production.
        ///	Called from Doc_Production
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_ProductionLine_ID">production line</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateProduction(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_ProductionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_ProductionLine_ID=" + M_ProductionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_ProductionLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_ProductionLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_ProductionLine_ID(M_ProductionLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);

                    cd.SetM_ProductionLine_ID(M_ProductionLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_InOutLine_ID">shipment</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">is sales order</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateShipment(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_InOutLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, bool IsSOTrx, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_InOutLine_ID=" + M_InOutLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_InOutLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_InOutLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_InOutLine_ID(M_InOutLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_InOutLine_ID(M_InOutLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
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
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost detail</returns>
        private static MCostDetail Get(Ctx ctx, String whereClause, int ID,
            int M_AttributeSetInstance_ID, Trx trxName)
        {
            String sql = "SELECT * FROM M_CostDetail WHERE " + whereClause;
            MCostDetail retValue = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", ID);
                param[1] = new SqlParameter("@param2", M_AttributeSetInstance_ID);
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
            String sql = "SELECT * FROM M_CostDetail "
                + "WHERE M_Product_ID=" + product.GetM_Product_ID()
                + " AND Processed='N' "
                + "ORDER BY C_AcctSchema_ID, M_CostElement_ID, AD_Org_ID, M_AttributeSetInstance_ID, Created";
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
            return GetC_InvoiceLine_ID() != 0;
        }

        /// <summary>
        /// Is Order
        /// </summary>
        /// <returns>true if order line</returns>
        public bool IsOrder()
        {
            return GetC_OrderLine_ID() != 0;
        }

        /// <summary>
        /// Is Shipment
        /// </summary>
        /// <returns>true if sales order shipment</returns>
        public bool IsShipment()
        {
            return IsSOTrx() && GetM_InOutLine_ID() != 0;
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
            MAcctSchema mas = new MAcctSchema(GetCtx(), GetC_AcctSchema_ID(), null);
            String CostingLevel = mas.GetCostingLevel();
            MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());




            dynamic pca = null;
            if (mas.GetFRPT_LocAcct_ID() > 0)
            {
                pca = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
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
                   product.GetM_Product_Category_ID(), GetC_AcctSchema_ID(), null);
                if (pca != null)
                {
                    if (pca.GetCostingLevel() != null)
                        CostingLevel = pca.GetCostingLevel();
                }
            }

            //	Org Element
            int Org_ID = GetAD_Org_ID();
            int M_ASI_ID = GetM_AttributeSetInstance_ID();
            if (MAcctSchema.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                Org_ID = 0;
                M_ASI_ID = 0;
            }
            else if (MAcctSchema.COSTINGLEVEL_Organization.Equals(CostingLevel))
                M_ASI_ID = 0;
            else if (MAcctSchema.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                Org_ID = 0;

            //	Create Material Cost elements
            if (GetM_CostElement_ID() == 0)
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
                MCostElement ce = MCostElement.Get(GetCtx(), GetM_CostElement_ID());
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
        private bool Process(MAcctSchema mas, MProduct product, MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            string qry = "";
            if (SaveCost(mas, product, ce, Org_ID, M_ASI_ID, 0))
            {
                if (product.IsOneAssetPerUOM())
                {
                    if (GetC_OrderLine_ID() > 0)
                    {
                        qry = @"SELECT ast.A_Asset_ID from A_Asset ast INNER JOIN M_InOutLine inl ON (ast.M_InOutLine_ID = inl.M_InOutLine_ID)
                            INNER JOIN C_OrderLine odl ON (inl.C_OrderLine_ID = odl.C_OrderLine_ID) Where ast.IsActive='Y' 
                            AND ast.M_Product_ID=" + product.GetM_Product_ID() + " AND inl.C_OrderLine_ID=" + GetC_OrderLine_ID();
                    }
                    else
                    {
                        qry = @"SELECT ast.A_Asset_ID from A_Asset ast INNER JOIN M_InOutLine inl ON (ast.M_InOutLine_ID = inl.M_InOutLine_ID)
                            INNER JOIN C_InvoiceLine inv ON (inv.M_InOutLine_ID = inl.M_InOutLine_ID) WHERE ast.IsActive ='Y' 
                            AND ast.M_Product_ID=" + product.GetM_Product_ID() + " AND inv.C_InvoiceLine_ID =" + GetC_InvoiceLine_ID();
                    }
                    DataSet ds = DB.ExecuteDataset(qry, null, null);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int A_Asset_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]);
                                if (!SaveCost(mas, product, ce, Org_ID, M_ASI_ID, A_Asset_ID))
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

        private bool SaveCost(MAcctSchema mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int A_Asset_ID)
        {
            MCost cost = null;
            if (A_Asset_ID == 0)
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID());
            }
            else
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID(), A_Asset_ID);
            }
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as, Org_ID, ce.getM_CostElement_ID());

            Decimal qty = GetQty();
            Decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();

            if (A_Asset_ID > 0)
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
            if (GetC_OrderLine_ID() != 0)
            {
                MOrderLine oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), null);
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
            else if (GetC_InvoiceLine_ID() != 0)
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
                    //MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                    //    mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
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
                                    mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
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
            else if (GetM_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetM_MovementLine_ID() != 0
                || GetM_InventoryLine_ID() != 0
                || GetM_ProductionLine_ID() != 0
                || GetC_ProjectIssue_ID() != 0
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
                        //MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                        //    mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
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
            if (A_Asset_ID != 0)
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
            if (GetC_OrderLine_ID() != 0)
                sb.Append(",C_OrderLine_ID=").Append(GetC_OrderLine_ID());
            if (GetM_InOutLine_ID() != 0)
                sb.Append(",M_InOutLine_ID=").Append(GetM_InOutLine_ID());
            if (GetC_InvoiceLine_ID() != 0)
                sb.Append(",C_InvoiceLine_ID=").Append(GetC_InvoiceLine_ID());
            if (GetC_ProjectIssue_ID() != 0)
                sb.Append(",C_ProjectIssue_ID=").Append(GetC_ProjectIssue_ID());
            if (GetM_MovementLine_ID() != 0)
                sb.Append(",M_MovementLine_ID=").Append(GetM_MovementLine_ID());
            if (GetM_InventoryLine_ID() != 0)
                sb.Append(",M_InventoryLine_ID=").Append(GetM_InventoryLine_ID());
            if (GetM_ProductionLine_ID() != 0)
                sb.Append(",M_ProductionLine_ID=").Append(GetM_ProductionLine_ID());
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
        public bool ProcessReversedCost(VAdvantage.Model.MAcctSchema mas, MProduct product, VAdvantage.Model.MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            MCost cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID());
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as1, Org_ID, ce.getM_CostElement_ID());

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
            if (GetC_OrderLine_ID() != 0)
            {
                MOrderLine oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), null);
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


                    string lastPrice = "select round(amt/ qty,6) as Price from m_costdetail where m_product_id="
                        + cost.GetM_Product_ID() + " and c_orderline_id is NOT NULL and c_orderline_id<> @param1"
                        + " ORDER BY m_costdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetC_OrderLine_ID()));

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
            else if (GetC_InvoiceLine_ID() != 0)
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
                    //MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                    //    mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
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

                    string lastPrice = "select round(amt/ qty,6) as Price from m_costdetail where m_product_id="
                        + cost.GetM_Product_ID() + " and c_invoiceline_id is NOT NULL and c_invoiceline_id<> @param1"
                        + " ORDER BY m_costdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetC_InvoiceLine_ID()));

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
                                    mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
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
            else if (GetM_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetM_MovementLine_ID() != 0
                || GetM_InventoryLine_ID() != 0
                || GetM_ProductionLine_ID() != 0
                || GetC_ProjectIssue_ID() != 0
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
                        //MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                        //    mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
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
       *	@param AD_Org_ID org
       *	@param M_Product_ID product
       *	@param M_AttributeSetInstance_ID asi
       *	@param M_InOutLine_ID shipment
       *	@param M_CostElement_ID optional cost element for Freight
       *	@param Amt amt
       *	@param Qty qty
       *	@param Description optional description
       *	@param IsSOTrx sales order
       *	@param trx transaction
       *	@return true if no error
       */
        public static Boolean CreateWorkOrderResourceTransaction(MAcctSchema as1, int AD_Org_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_WorkOrderResourceTransactionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderResourceTxnLine_ID= " + M_WorkOrderResourceTransactionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
                + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID();


            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);
            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderResourceTxnLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                    M_WorkOrderResourceTransactionLine_ID, M_AttributeSetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx);
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
                        cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trx);
                    }
                    // CostSetByProcess(cd, as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderResourceTxnLine_ID(M_WorkOrderResourceTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(as1.GetCtx(), as1.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_WorkOrderTransactionLine_ID"></param>
        /// <param name="M_CostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amt</param>
        /// <param name="Qty">qty</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">sales order</param>
        /// <param name="trx">transaction</param>
        /// <returns>true if no error</returns>
        public static Boolean CreateWorkOrderTransaction(MAcctSchema as1, int AD_Org_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_WorkOrderTransactionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderTransactionLine_ID= " + M_WorkOrderTransactionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
                + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID()
                + " AND M_CostElement_ID = " + M_CostElement_ID;

            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);

            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderTransactionLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                    M_WorkOrderTransactionLine_ID, M_AttributeSetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID, Amt, Qty, Description, trx);

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
                        cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trx);
                    }
                    //CostSetByProcess(cd, as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderTransactionLine_ID(M_WorkOrderTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);

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
                MClient client = MClient.Get(as1.GetCtx(), as1.GetAD_Client_ID());
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
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute instance set on product</param>
        /// <param name="costLineColName">Column name of M_CostDetail table to which value is to be set</param>
        /// <param name="ID">value that is set for the costLineColName</param>
        /// <param name="M_CostElement_ID">cost element set on product category/accounting schema</param>
        /// <param name="Amt">Amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">discription</param>
        /// <param name="IsSOTrx">is sales transaction</param>
        /// <param name="trx">transaction</param>
        /// <param name="RectifyPostedRecords"></param>
        /// <returns>on success true else false</returns>
        /// <Created by>Raghu</Created>
        /// <date>25-09-2015</date>
        public static Boolean CreateCostTransaction(MAcctSchema as1, int AD_Org_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, string costLineColName, int ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
               + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
               + " AND " + costLineColName + "= " + ID
               + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
               + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID()
               + " AND M_CostElement_ID = " + M_CostElement_ID;

            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);

            MCostDetail cd = Get(as1.GetCtx(), costLineColName + "=@param1 AND M_AttributeSetInstance_ID=@param2", ID, M_AttributeSetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx);
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
                        cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trx);
                    }
                    cd.Set_CustomColumn(costLineColName, ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    cd.SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
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
                MClient client = MClient.Get(as1.GetCtx(), as1.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }
    }
}